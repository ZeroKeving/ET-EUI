
using System;

namespace ET
{
    /// <summary>
    /// Gate���ط�����֪ͨ��¼���ķ������Ƴ���¼��Ϣ������
    /// </summary>
    public class G2L_RemoveLoginRecordHandler : AMActorRpcHandler<Scene, G2L_RemoveLoginRecord, L2G_RemoveLoginRecord>
    {
        protected override async ETTask Run(Scene scene, G2L_RemoveLoginRecord request, L2G_RemoveLoginRecord response, Action reply)
        {
            long accountId = request.AccountId;
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginCenterLock,accountId.GetHashCode()))//Э����
            {
                int zone = scene.GetComponent<LoginInfoRecordComponent>().Get(accountId);//��ȡ��¼��Ϣ��¼���
                if(request.ServerId == zone)
                {
                    scene.GetComponent<LoginInfoRecordComponent>().Remove(accountId);//������¼��Ϣ��¼
                }
            }
            reply();
        }
    }
}