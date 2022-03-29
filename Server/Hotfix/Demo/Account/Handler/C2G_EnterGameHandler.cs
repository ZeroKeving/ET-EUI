
using System;

namespace ET
{
    /// <summary>
    /// 进入游戏消息处理逻辑
    /// </summary>
    public class C2G_EnterGameHandler : AMRpcHandler<C2G_EnterGame, G2C_EnterGame>
    {
        protected override async ETTask Run(Session session, C2G_EnterGame request, G2C_EnterGame response, Action reply)
        {
            if (session.DomainScene().SceneType != SceneType.Gate)//这条请求消息是否请求到的是Gate网关服务器,如果不是则报错
            {
                Log.Error($"请求的Scene错误，当前Scene为：{session.DomainScene().SceneType}");
                session.Dispose();//断开连接
                return;
            }

            if (session.GetComponent<SessionLockingComponent>() != null)//如果会话组件锁不为空的情况下，说明服务器接收到客户端多次请求
            {
                response.Error = ErrorCode.ERR_RequestRepeatedly;//返回登录请求多次错误码
                reply();//委托发送回复消息response
                return;
            }

            SessionPlayerComponent sessionPlayerComponent = session.GetComponent<SessionPlayerComponent>();//拿到玩家会话组件
            if (sessionPlayerComponent == null)
            {
                response.Error = ErrorCode.ERR_SessionPlayerError;
                reply();
                return;
            }

            Player player = Game.EventSystem.Get(sessionPlayerComponent.PlayerInstanceId) as Player;//获取玩家映射

            if (player == null || player.IsDisposed)
            {
                response.Error = ErrorCode.ERR_NonePlayerError;
                reply();
                return;
            }

            long instanceId = session.InstanceId;//获得会话id
            using (session.AddComponent<SessionLockingComponent>())//会话组件锁(因为下面用到了异步逻辑),同一个用户的会话组件重复访问时会抢这把锁
            {
                using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginGate, player.AccountId.GetHashCode()))//协程锁，所有用户都会抢这把锁，传入唯一id,用户id的哈希值（防止不同地址的用户同时用相同的账号去登录）
                {
                    if (instanceId != session.InstanceId || player.IsDisposed)//如果会话id被改变了或被释放了就返回
                    {
                        response.Error = ErrorCode.ERR_PlayerSessionError;
                        reply();
                        return;
                    }

                    if(session.GetComponent<SessionStateComponent>() != null && session.GetComponent<SessionStateComponent>().State == SessionState.Game)//会话状态已经进入游戏逻辑服
                    {
                        response.Error = ErrorCode.ERR_SessionStateError;
                        reply();
                        return;
                    }

                    if(player.PlayerState == PlayerState.Game)//玩家已经进入到游戏服务器的状态（因为player可以映射任意的一个session，但是session是唯一的，如果有多个客户端顶号，多个session可能映射到一个player上）
                    {
                        try
                        {
                            IActorResponse reqEnter = await MessageHelper.CallLocationActor(player.UnitId, new G2M_RequestEnterGameState());//从网关服务器给游戏逻辑服务器玩家发送一条消息
                            if(reqEnter.Error == ErrorCode.ERR_Success)//二次登录或顶号登录成功
                            {
                                reply();
                                return;
                            }
                            Log.Error("二次登录失败" + reqEnter.Error + "|" +reqEnter.Message);
                            response.Error = ErrorCode.ERR_ReEnterGameError;
                            await DisconnectHelper.KickPlayer(player, true);//异常踢出玩家
                            reply();
                            session?.Disconnect().Coroutine();//延迟1秒断开连接
                        }
                        catch (Exception e)
                        {
                            Log.Error("二次登录失败" + e.ToString());
                            response.Error = ErrorCode.ERR_ReEnterGameError2;
                            await DisconnectHelper.KickPlayer(player,true);//异常踢出玩家
                            reply();
                            session?.Disconnect().Coroutine();//延迟1秒断开连接
                            throw;//抛出异常
                        }
                        return;
                    }

                    try
                    {
                        GateMapComponent gateMapComponent = player.AddComponent<GateMapComponent>();
                        gateMapComponent.Scene = await SceneFactory.Create(gateMapComponent,"GateMap",SceneType.Map);//创建一个Scene给网关地图组件赋值

                        Unit unit = UnitFactory.Create(gateMapComponent.Scene,player.Id,UnitType.Player);//创建一个玩家的映射对象实体
                        unit.AddComponent<UnitGateComponent, long>(session.InstanceId);//给玩家映射挂上网关组件，传入会话实例id
                        long unitId = unit.Id;

                        StartSceneConfig startSceneConfig = StartSceneConfigCategory.Instance.GetBySceneName(session.DomainZone(), "Map1");//获取Map1游戏逻辑服务器的配置
                        await TransferHelper.Transfer(unit, startSceneConfig.InstanceId, startSceneConfig.Name);//将unit对象传送到游戏逻辑服当中

                        //传送成功后将unitId存储并返回
                        player.UnitId = unitId;
                        response.MyId = unitId;

                        reply();

                        //将会话状态改变为游戏状态
                        SessionStateComponent sessionStateComponent = session.GetComponent<SessionStateComponent>();
                        if(sessionStateComponent == null)
                        {
                            sessionStateComponent = session.AddComponent<SessionStateComponent>();
                        }
                        sessionStateComponent.State = SessionState.Game;

                        player.PlayerState = PlayerState.Game;//玩家映射状态改为游戏状态

                    }
                    catch (Exception e)
                    {
                        Log.Error($"角色进入游戏逻辑服出现问题 账号Id:{player.AccountId} 角色Id:{player.Id} 异常信息：{e.ToString()}");
                        response.Error = ErrorCode.ERR_EnterGameError;
                        reply();
                        await DisconnectHelper.KickPlayer(player,true);//将异常玩家踢下线
                        session.Disconnect().Coroutine();//延迟一秒后断开
                    }

                }
            }
        }
    }
}