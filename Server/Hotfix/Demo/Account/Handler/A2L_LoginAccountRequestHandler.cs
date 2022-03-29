
using System;

namespace ET
{
    /// <summary>
    /// ��¼���������¼���ķ��������͵��˺ŵ�¼������Ϣ����
    /// </summary>
    public class A2L_LoginAccountRequestHandler : AMActorRpcHandler<Scene, A2L_LoginAccountRequest, L2A_LoginAccountResponse>
    {
        protected override async ETTask Run(Scene scene, A2L_LoginAccountRequest request, L2A_LoginAccountResponse response, Action reply)
        {
            long accountId = request.AccountId;//�˺�id

            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginCenterLock, accountId.GetHashCode()))//Э����
            {
                if (!scene.GetComponent<LoginInfoRecordComponent>().IsExist(accountId))//����ڵ�¼���ķ�������¼�в�����
                {
                    reply();//�ظ���Ϣ
                    return;
                }

                int zone = scene.GetComponent<LoginInfoRecordComponent>().Get(accountId);//��ǰ�˺ŵ�¼������
                StartSceneConfig gateConfig = RealmGateAddressHelper.GetGate(zone, accountId);//ͨ��������Ϣ��ȡ�������õ�ַ

                var g2L_DisconnectGateUnit = (G2L_DisconnectGateUnit)await MessageHelper.CallActor(gateConfig.InstanceId, new L2G_DisconnectGateUnit() { AccountId = accountId });//��Gate���ط�����������Ϣ���ȴ��ظ���MessageHelper�ڲ�Ҳ�ǵ���ActorMessageSenderComponent�ĵ�������

                response.Error = g2L_DisconnectGateUnit.Error;//�����ط����Ĵ����뷢�ص�¼������
                reply();//�ظ���Ϣ

            }

        }
    }
}