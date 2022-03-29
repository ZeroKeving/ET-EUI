
using System;
using System.Text.RegularExpressions;

namespace ET
{
    /// <summary>
    /// 客户端向登录服务器发送创建角色请求处理
    /// </summary>
    public class C2A_CreaterRoleHandler : AMRpcHandler<C2A_CreateRole, A2C_CreateRole>
    {
        protected override async ETTask Run(Session session, C2A_CreateRole request, A2C_CreateRole response, Action reply)
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

            if(token == null || token != request.Token)//如果令牌错误则断开连接
            {
                response.Error = ErrorCode.ERR_TokenError;
                reply();
                session.Disconnect().Coroutine();
                return;
            }

            if(string.IsNullOrEmpty(request.Name) || !Regex.IsMatch(request.Name.Trim(), @"^(?=.*[A-Za-z0-9\u4e00-\u9fa5].*).{1,10}$"))//判断字符串是否为空或格式是否正确
            {
                response.Error = ErrorCode.ERR_RoleNameFormError;//格式错误
                reply();
                return;
            }

            using (session.AddComponent<SessionLockingComponent>())//会话组件锁(因为下面用到了异步逻辑),同一个用户的会话组件重复访问时会抢这把锁
            {
                using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.CreateRole, request.AccountId))//协程锁，所有用户都会抢这把锁，传入唯一id,用户账号名的哈希值（防止不同地址的用户同时用相同的账号去登录）
                {
                    var roleInfos = await DBManagerComponent.Instance.GetZoneDB(request.ServerId).Query<RoleInfo>(d => d.Name == request.Name && d.ServerId == request.ServerId);//从数据库中查询该服务器中有没有同名角色

                    if (roleInfos != null && roleInfos.Count > 0)//如果重名的情况
                    {
                        response.Error = ErrorCode.ERR_RoleNameRepeat;//角色名字重复
                        reply();
                        return;
                    }

                    RoleInfo newRoleInfo = session.AddChildWithId<RoleInfo>(IdGenerater.Instance.GenerateUnitId(request.ServerId));//添加新的角色信息
                    newRoleInfo.Name = request.Name;
                    newRoleInfo.ServerId = request.ServerId;
                    newRoleInfo.State = (int)RoleInfoState.Normal;
                    newRoleInfo.AccountId = request.AccountId;
                    newRoleInfo.CreateTime = TimeHelper.ServerNow();
                    newRoleInfo.LastLoginTime = 0;

                    await DBManagerComponent.Instance.GetZoneDB(request.ServerId).Save<RoleInfo>(newRoleInfo);//将新创建的角色信息添加入数据库

                    response.RoleInfo = newRoleInfo.ToMessage();

                    reply();

                    newRoleInfo.Dispose();//释放临时变量
                }
            }
            

            await ETTask.CompletedTask;
        }
    }
}