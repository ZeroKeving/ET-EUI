
using System;

namespace ET
{
    /// <summary>
    /// ���Realm���ظ��ؾ�������������ƺ͵�ַ������
    /// </summary>
    public class C2A_GetRealmKeyHandler : AMRpcHandler<C2A_GetRealmKey, A2C_GetRealmKey>
    {
        protected override async ETTask Run(Session session, C2A_GetRealmKey request, A2C_GetRealmKey response, Action reply)
        {
            if (session.DomainScene().SceneType != SceneType.Account)//����������Ϣ�Ƿ����󵽵��ǵ�¼������,���������Ͽ�����
            {
                Log.Error($"�����Scene���󣬵�ǰSceneΪ��{session.DomainScene().SceneType}");
                session.Dispose();//�Ͽ�����
                return;
            }

            if (session.GetComponent<SessionLockingComponent>() != null)//����Ự�������Ϊ�յ�����£�˵�����������յ��ͻ��˶������
            {
                response.Error = ErrorCode.ERR_RequestRepeatedly;//���ص�¼�����δ�����
                reply();//ί�з��ͻظ���Ϣresponse
                session.Disconnect().Coroutine();//�ӳ�1���Ͽ�����
                return;
            }

            string token = session.DomainScene().GetComponent<TokenComponent>().Get(request.AccountId);//��ø��˺ŵĵ�¼����

            if (token == null || token != request.Token)//������ƴ�����Ͽ�����
            {
                response.Error = ErrorCode.ERR_TokenError;
                reply();
                session.Disconnect().Coroutine();
                return;
            }

            using (session.AddComponent<SessionLockingComponent>())//�Ự�����(��Ϊ�����õ����첽�߼�),ͬһ���û��ĻỰ����ظ�����ʱ���������
            {
                using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginAccount, request.AccountId))//Э�����������û������������������Ψһid,�û��˺����Ĺ�ϣֵ����ֹ��ͬ��ַ���û�ͬʱ����ͬ���˺�ȥ��¼��
                {
                    StartSceneConfig realmStartSceneConfig = RealmGateAddressHelper.GetRealm(request.ServerId);//���Realm��������
                    R2A_GetRealmKey r2A_GetRealmKey = (R2A_GetRealmKey)await MessageHelper.CallActor(realmStartSceneConfig.InstanceId, new A2R_GetRealmKey()
                    {
                        AccountId = request.AccountId,
                    });//��Realm������������Ϣ(realmStartSceneConfig.InstanceId���а�����realm�ĵ�ַ)

                    if(r2A_GetRealmKey.Error != ErrorCode.ERR_Success)//������ش�����
                    {
                        response.Error = r2A_GetRealmKey.Error;
                        reply();
                        session?.Disconnect().Coroutine();//�ӳ�1��Ͽ�����
                        return;
                    }

                    response.RealmKey = r2A_GetRealmKey.RealmKey;
                    response.RealmAddress = realmStartSceneConfig.OuterIPPort.ToString();
                    reply();
                    session?.Disconnect().Coroutine();//�ӳ�1���Ͽ�
                }
            }

            await ETTask.CompletedTask;
        }
    }
}