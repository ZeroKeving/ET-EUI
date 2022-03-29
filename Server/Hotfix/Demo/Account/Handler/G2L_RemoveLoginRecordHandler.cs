
using System;

namespace ET
{
    /// <summary>
    /// Gate网关服务器通知登录中心服务器移除登录信息请求处理
    /// </summary>
    public class G2L_RemoveLoginRecordHandler : AMActorRpcHandler<Scene, G2L_RemoveLoginRecord, L2G_RemoveLoginRecord>
    {
        protected override async ETTask Run(Scene scene, G2L_RemoveLoginRecord request, L2G_RemoveLoginRecord response, Action reply)
        {
            long accountId = request.AccountId;
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginCenterLock,accountId.GetHashCode()))//协程锁
            {
                int zone = scene.GetComponent<LoginInfoRecordComponent>().Get(accountId);//获取登录信息记录组件
                if(request.ServerId == zone)
                {
                    scene.GetComponent<LoginInfoRecordComponent>().Remove(accountId);//消除登录信息记录
                }
            }
            reply();
        }
    }
}