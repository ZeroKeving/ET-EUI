
using System;

namespace ET
{
    /// <summary>
    /// gate网关服务器向登录中心服务器添加登录记录消息处理
    /// </summary>
    public class G2L_AddLoginRecordHandler : AMActorRpcHandler<Scene, G2L_AddLoginRecord, L2G_AddLoginRecord>
    {
        protected override async ETTask Run(Scene scene, G2L_AddLoginRecord request, L2G_AddLoginRecord response, Action reply)
        {
            long accountId = request.AccountId;
            using(await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginCenterLock,accountId.GetHashCode()))
            {
                scene.GetComponent<LoginInfoRecordComponent>().Remove(accountId);//移除登录信息
                scene.GetComponent<LoginInfoRecordComponent>().Add(accountId,request.ServerId);//记录新的登录信息
            }
            reply();
        }
    }
}