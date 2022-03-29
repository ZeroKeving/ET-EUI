
using System;

namespace ET
{
    /// <summary>
    /// 客户端登录Gate网关服务器请求处理
    /// </summary>
    public class C2G_LoginGameGateHandler : AMRpcHandler<C2G_LoginGameGate, G2C_LoginGameGate>
    {
        protected override async ETTask Run(Session session, C2G_LoginGameGate request, G2C_LoginGameGate response, Action reply)
        {
            if (session.DomainScene().SceneType != SceneType.Gate)//这条请求消息是否请求到的是Gate网关服务器,如果不是则报错
            {
                Log.Error($"请求的Scene错误，当前Scene为：{session.DomainScene().SceneType}");
                session.Dispose();//断开连接
                return;
            }
            session.RemoveComponent<SessionAcceptTimeoutComponent>();//移除链接5秒超时组件（代表链接通过了验证，如果没有通过验证该组件5秒后会断开链接）

            if (session.GetComponent<SessionLockingComponent>() != null)//如果会话组件锁不为空的情况下，说明服务器接收到客户端多次请求
            {
                response.Error = ErrorCode.ERR_RequestRepeatedly;//返回登录请求多次错误码
                reply();//委托发送回复消息response
                session.Disconnect().Coroutine();//延迟1秒后断开连接
                return;
            }

            Scene scene = session.DomainScene();
            string tokenKey = scene.GetComponent<GateSessionKeyComponent>().Get(request.AccountId);//获取Gate网关令牌
            if (tokenKey == null || !tokenKey.Equals(request.Key))//验证Gate网关令牌
            {
                response.Error = ErrorCode.ERR_ConnectGateKeyError;
                response.Message = "Gate Key验证失败";
                reply();
                session?.Disconnect().Coroutine();
                return;
            }

            scene.GetComponent<GateSessionKeyComponent>().Remove(request.AccountId);//将Gate网关令牌移除

            long instanceId = session.InstanceId;//保证协程锁的逻辑不受到异步函数的影响
            using (session.AddComponent<SessionLockingComponent>())//会话组件锁(因为下面用到了异步逻辑),同一个用户的会话组件重复访问时会抢这把锁
            {
                using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginGate, request.AccountId.GetHashCode()))//协程锁，所有用户都会抢这把锁，传入唯一id,用户id的哈希值（防止不同地址的用户同时用相同的账号去登录）
                {
                    if(instanceId != session.InstanceId)//防止多个客户端登录同一个账号
                    {
                        return;
                    }

                    //通知登录中心服 记录本次登录的服务器Zone
                    StartSceneConfig loginCenterConfig = StartSceneConfigCategory.Instance.LoginCenterConfig;
                    L2G_AddLoginRecord l2G_AddLoginRecord = (L2G_AddLoginRecord)await MessageHelper.CallActor(loginCenterConfig.InstanceId, new G2L_AddLoginRecord()
                    {
                        AccountId = request.AccountId,
                        ServerId = scene.Zone,
                    });

                    if(l2G_AddLoginRecord.Error != ErrorCode.ERR_Success)//如果有报错
                    {
                        response.Error = l2G_AddLoginRecord.Error;
                        reply();
                        session?.Disconnect().Coroutine();
                        return;
                    }

                    //获取会话状态组件，并将其状态设置为普通
                    SessionStateComponent sessionStateComponent = session.GetComponent<SessionStateComponent>();
                    if(sessionStateComponent == null)
                    {
                        sessionStateComponent = session.AddComponent<SessionStateComponent>();
                    }
                    sessionStateComponent.State = SessionState.Normal;

                    Player player = scene.GetComponent<PlayerComponent>().Get(request.AccountId);//获得游戏客户端在Gate网关上的一个映射

                    if(player == null)
                    {
                        //添加一个新的GateUnit
                        player = scene.GetComponent<PlayerComponent>().AddChildWithId<Player, long, long>(request.RoleId, request.AccountId, request.RoleId);
                        player.PlayerState = PlayerState.Gate;
                        scene.GetComponent<PlayerComponent>().Add(player);//将该玩家添加进玩家组件
                        session.AddComponent<MailBoxComponent,MailboxType>(MailboxType.GateSession);//给会话添加Actor处理组件
                    }
                    else
                    {
                        player.RemoveComponent<PlayerOfflineOutTimeComponent>();//移除player倒计时踢出组件
                    }

                    //添加会话玩家组件，并将玩家信息存入
                    session.AddComponent<SessionPlayerComponent>().PlayerId = player.Id;
                    session.GetComponent<SessionPlayerComponent>().PlayerInstanceId = player.InstanceId;
                    session.GetComponent<SessionPlayerComponent>().AccountId = request.AccountId;
                    player.SessionInstanceId = session.InstanceId;
                }

                reply();
            }

        }
    }
}