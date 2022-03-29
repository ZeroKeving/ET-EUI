
using System;

namespace ET
{
    /// <summary>
    /// 获得Realm网关负载均衡服务器的令牌和地址请求处理
    /// </summary>
    public class C2A_GetRealmKeyHandler : AMRpcHandler<C2A_GetRealmKey, A2C_GetRealmKey>
    {
        protected override async ETTask Run(Session session, C2A_GetRealmKey request, A2C_GetRealmKey response, Action reply)
        {
            if (session.DomainScene().SceneType != SceneType.Account)//这条请求消息是否请求到的是登录服务器,如果不是则断开链接
            {
                Log.Error($"请求的Scene错误，当前Scene为：{session.DomainScene().SceneType}");
                session.Dispose();//断开连接
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

            if (token == null || token != request.Token)//如果令牌错误则断开连接
            {
                response.Error = ErrorCode.ERR_TokenError;
                reply();
                session.Disconnect().Coroutine();
                return;
            }

            using (session.AddComponent<SessionLockingComponent>())//会话组件锁(因为下面用到了异步逻辑),同一个用户的会话组件重复访问时会抢这把锁
            {
                using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginAccount, request.AccountId))//协程锁，所有用户都会抢这把锁，传入唯一id,用户账号名的哈希值（防止不同地址的用户同时用相同的账号去登录）
                {
                    StartSceneConfig realmStartSceneConfig = RealmGateAddressHelper.GetRealm(request.ServerId);//获得Realm配置数据
                    R2A_GetRealmKey r2A_GetRealmKey = (R2A_GetRealmKey)await MessageHelper.CallActor(realmStartSceneConfig.InstanceId, new A2R_GetRealmKey()
                    {
                        AccountId = request.AccountId,
                    });//向Realm服务器发送消息(realmStartSceneConfig.InstanceId当中包含了realm的地址)

                    if(r2A_GetRealmKey.Error != ErrorCode.ERR_Success)//如果返回错误码
                    {
                        response.Error = r2A_GetRealmKey.Error;
                        reply();
                        session?.Disconnect().Coroutine();//延迟1秒断开连接
                        return;
                    }

                    response.RealmKey = r2A_GetRealmKey.RealmKey;
                    response.RealmAddress = realmStartSceneConfig.OuterIPPort.ToString();
                    reply();
                    session?.Disconnect().Coroutine();//延迟1秒后断开
                }
            }

            await ETTask.CompletedTask;
        }
    }
}