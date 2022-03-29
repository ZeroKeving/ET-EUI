
using System;

namespace ET
{
    /// <summary>
    /// Realm服务器：获取Gate网关服务器令牌和地址的请求处理
    /// </summary>
    public class C2R_LoginRealmHandler : AMRpcHandler<C2R_LoginRealm, R2C_LoginRealm>
    {
        protected override async ETTask Run(Session session, C2R_LoginRealm request, R2C_LoginRealm response, Action reply)
        {
            if (session.DomainScene().SceneType != SceneType.Realm)//这条请求消息是否请求到的是Realm服务器,如果不是则报错
            {
                Log.Error($"请求的Scene错误，当前Scene为：{session.DomainScene().SceneType}");
                response.Error = ErrorCode.ERR_RequestSceneTypeError;
                reply();
                return;
            }

            if (session.GetComponent<SessionLockingComponent>() != null)//如果会话组件锁不为空的情况下，说明服务器接收到客户端多次请求
            {
                response.Error = ErrorCode.ERR_RequestRepeatedly;//返回登录请求多次错误码
                reply();//委托发送回复消息response
                session.Disconnect().Coroutine();//延迟1秒后断开连接
                return;
            }

            string token = session.DomainScene().GetComponent<TokenComponent>().Get(request.AccountId);//获得该账号的登录令牌

            if (token == null || token != request.RealmTokenKey)//如果令牌错误则断开连接
            {
                response.Error = ErrorCode.ERR_TokenError;
                reply();
                session?.Disconnect().Coroutine();
                return;
            }

            Scene domainScene = session.DomainScene();
            domainScene.GetComponent<TokenComponent>().Remove(request.AccountId);//移除该令牌

            using (session.AddComponent<SessionLockingComponent>())//会话组件锁(因为下面用到了异步逻辑),同一个用户的会话组件重复访问时会抢这把锁
            {
                using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginRealm, request.AccountId))//协程锁，所有用户都会抢这把锁，传入唯一id,用户账号名的哈希值（防止不同地址的用户同时用相同的账号去登录）
                {
                    StartSceneConfig config = RealmGateAddressHelper.GetGate(domainScene.Zone, request.AccountId);//取模固定分配一个Gate

                    G2R_GetLoginGateKey g2R_GetLoginGateKey = (G2R_GetLoginGateKey)await MessageHelper.CallActor(config.InstanceId, new R2G_GetLoginGateKey()//向Gate网关服务器请求一个key，客户端可以拿着这个key连接Gate网关服务器
                    {
                        AccountId = request.AccountId,
                    });

                    if(g2R_GetLoginGateKey.Error != ErrorCode.ERR_Success)//如果有报错
                    {
                        response.Error = g2R_GetLoginGateKey.Error;
                        reply();
                        return;
                    }

                    response.GateSessionKey = g2R_GetLoginGateKey.GateSessionKey;
                    response.GateAddress = config.OuterIPPort.ToString();
                    reply();

                    session?.Disconnect().Coroutine();//延迟1秒断开连接
                }
            }

        }
    }
}