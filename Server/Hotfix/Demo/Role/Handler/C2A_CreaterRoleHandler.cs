
using System;
using System.Text.RegularExpressions;

namespace ET
{
    /// <summary>
    /// �ͻ������¼���������ʹ�����ɫ������
    /// </summary>
    public class C2A_CreaterRoleHandler : AMRpcHandler<C2A_CreateRole, A2C_CreateRole>
    {
        protected override async ETTask Run(Session session, C2A_CreateRole request, A2C_CreateRole response, Action reply)
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

            if(token == null || token != request.Token)//������ƴ�����Ͽ�����
            {
                response.Error = ErrorCode.ERR_TokenError;
                reply();
                session.Disconnect().Coroutine();
                return;
            }

            if(string.IsNullOrEmpty(request.Name) || !Regex.IsMatch(request.Name.Trim(), @"^(?=.*[A-Za-z0-9\u4e00-\u9fa5].*).{1,10}$"))//�ж��ַ����Ƿ�Ϊ�ջ��ʽ�Ƿ���ȷ
            {
                response.Error = ErrorCode.ERR_RoleNameFormError;//��ʽ����
                reply();
                return;
            }

            using (session.AddComponent<SessionLockingComponent>())//�Ự�����(��Ϊ�����õ����첽�߼�),ͬһ���û��ĻỰ����ظ�����ʱ���������
            {
                using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.CreateRole, request.AccountId))//Э�����������û������������������Ψһid,�û��˺����Ĺ�ϣֵ����ֹ��ͬ��ַ���û�ͬʱ����ͬ���˺�ȥ��¼��
                {
                    var roleInfos = await DBManagerComponent.Instance.GetZoneDB(request.ServerId).Query<RoleInfo>(d => d.Name == request.Name && d.ServerId == request.ServerId);//�����ݿ��в�ѯ�÷���������û��ͬ����ɫ

                    if (roleInfos != null && roleInfos.Count > 0)//������������
                    {
                        response.Error = ErrorCode.ERR_RoleNameRepeat;//��ɫ�����ظ�
                        reply();
                        return;
                    }

                    RoleInfo newRoleInfo = session.AddChildWithId<RoleInfo>(IdGenerater.Instance.GenerateUnitId(request.ServerId));//����µĽ�ɫ��Ϣ
                    newRoleInfo.Name = request.Name;
                    newRoleInfo.ServerId = request.ServerId;
                    newRoleInfo.State = (int)RoleInfoState.Normal;
                    newRoleInfo.AccountId = request.AccountId;
                    newRoleInfo.CreateTime = TimeHelper.ServerNow();
                    newRoleInfo.LastLoginTime = 0;

                    await DBManagerComponent.Instance.GetZoneDB(request.ServerId).Save<RoleInfo>(newRoleInfo);//���´����Ľ�ɫ��Ϣ��������ݿ�

                    response.RoleInfo = newRoleInfo.ToMessage();

                    reply();

                    newRoleInfo.Dispose();//�ͷ���ʱ����
                }
            }
            

            await ETTask.CompletedTask;
        }
    }
}