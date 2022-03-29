
using System;

namespace ET
{
    /// <summary>
    /// 删除该服务器所有角色消息处理
    /// </summary>
    public class C2A_DeleteAllRoleHandler : AMRpcHandler<C2A_DeleteAllRole, A2C_DeleteAllRole>
    {
        protected override async ETTask Run(Session session, C2A_DeleteAllRole request, A2C_DeleteAllRole response, Action reply)
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
                using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.CreateRole, request.AccountId))//协程锁，所有用户都会抢这把锁，传入唯一id,用户账号名的哈希值（防止不同地址的用户同时用相同的账号去登录）
                {
                    var roleInfos = await DBManagerComponent.Instance.GetZoneDB(request.ServerId).Query<RoleInfo>(d => d.AccountId == request.AccountId && d.ServerId == request.ServerId);
                    if (roleInfos == null || roleInfos.Count <= 0)//如果角色不存在
                    {
                        response.Error = ErrorCode.ERR_RoleNotExist;
                        reply();
                        return;
                    }

                    foreach (var roleInfo in roleInfos)
                    {
                        session.AddChild(roleInfo);

                        roleInfo.State = (int)RoleInfoState.Freeze;//将角色冻结

                        await DBManagerComponent.Instance.GetZoneDB(request.ServerId).Save(roleInfo);//将改动后的数据存入服务器
                    }

                    reply();
                }
            }
            await ETTask.CompletedTask;
        }
    }
}