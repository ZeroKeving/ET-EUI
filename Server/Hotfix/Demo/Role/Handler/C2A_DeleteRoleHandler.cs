
using System;

namespace ET
{
    /// <summary>
    /// ɾ����ɫ��Ϣ����
    /// </summary>
    public class C2A_DeleteRoleHandler : AMRpcHandler<C2A_DeleteRole, A2C_DeleteRole>
    {
        protected override async ETTask Run(Session session, C2A_DeleteRole request, A2C_DeleteRole response, Action reply)
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
                using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.CreateRole, request.AccountId))//Э�����������û������������������Ψһid,�û��˺����Ĺ�ϣֵ����ֹ��ͬ��ַ���û�ͬʱ����ͬ���˺�ȥ��¼��
                {
                    var roleInfos = await DBManagerComponent.Instance.GetZoneDB(request.ServerId).Query<RoleInfo>(d => d.Id == request.RoleInfoId && d.ServerId == request.ServerId);
                    if (roleInfos == null || roleInfos.Count <= 0)//�����ɫ������
                    {
                        response.Error = ErrorCode.ERR_RoleNotExist;
                        reply();
                        return;
                    }

                    var roleInfo = roleInfos[0];
                    session.AddChild(roleInfo);

                    roleInfo.State = (int)RoleInfoState.Freeze;//����ɫ����

                    await DBManagerComponent.Instance.GetZoneDB(request.ServerId).Save(roleInfo);//���Ķ�������ݴ��������
                    response.DeletedRoleInfoId = roleInfo.Id;
                    roleInfo?.Dispose();

                    reply();
                }
            }
            await ETTask.CompletedTask;
        }
    }
}