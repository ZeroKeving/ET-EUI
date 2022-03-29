
using System;

namespace ET
{
    /// <summary>
    /// 登录服务器向登录中心服务器发送的账号登录请求消息处理
    /// </summary>
    public class A2L_LoginAccountRequestHandler : AMActorRpcHandler<Scene, A2L_LoginAccountRequest, L2A_LoginAccountResponse>
    {
        protected override async ETTask Run(Scene scene, A2L_LoginAccountRequest request, L2A_LoginAccountResponse response, Action reply)
        {
            long accountId = request.AccountId;//账号id

            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginCenterLock, accountId.GetHashCode()))//协程锁
            {
                if (!scene.GetComponent<LoginInfoRecordComponent>().IsExist(accountId))//如果在登录中心服务器记录中不存在
                {
                    reply();//回复消息
                    return;
                }

                int zone = scene.GetComponent<LoginInfoRecordComponent>().Get(accountId);//当前账号登录的区服
                StartSceneConfig gateConfig = RealmGateAddressHelper.GetGate(zone, accountId);//通过区服信息获取网关配置地址

                var g2L_DisconnectGateUnit = (G2L_DisconnectGateUnit)await MessageHelper.CallActor(gateConfig.InstanceId, new L2G_DisconnectGateUnit() { AccountId = accountId });//向Gate网关服务器发送消息并等待回复（MessageHelper内部也是调用ActorMessageSenderComponent的单例对象）

                response.Error = g2L_DisconnectGateUnit.Error;//将网关发来的错误码发回登录服务器
                reply();//回复消息

            }

        }
    }
}