
using System;

namespace ET
{
    /// <summary>
    /// �ͻ��˵�¼Gate���ط�����������
    /// </summary>
    public class C2G_LoginGameGateHandler : AMRpcHandler<C2G_LoginGameGate, G2C_LoginGameGate>
    {
        protected override async ETTask Run(Session session, C2G_LoginGameGate request, G2C_LoginGameGate response, Action reply)
        {
            if (session.DomainScene().SceneType != SceneType.Gate)//����������Ϣ�Ƿ����󵽵���Gate���ط�����,��������򱨴�
            {
                Log.Error($"�����Scene���󣬵�ǰSceneΪ��{session.DomainScene().SceneType}");
                session.Dispose();//�Ͽ�����
                return;
            }
            session.RemoveComponent<SessionAcceptTimeoutComponent>();//�Ƴ�����5�볬ʱ�������������ͨ������֤�����û��ͨ����֤�����5����Ͽ����ӣ�

            if (session.GetComponent<SessionLockingComponent>() != null)//����Ự�������Ϊ�յ�����£�˵�����������յ��ͻ��˶������
            {
                response.Error = ErrorCode.ERR_RequestRepeatedly;//���ص�¼�����δ�����
                reply();//ί�з��ͻظ���Ϣresponse
                session.Disconnect().Coroutine();//�ӳ�1���Ͽ�����
                return;
            }

            Scene scene = session.DomainScene();
            string tokenKey = scene.GetComponent<GateSessionKeyComponent>().Get(request.AccountId);//��ȡGate��������
            if (tokenKey == null || !tokenKey.Equals(request.Key))//��֤Gate��������
            {
                response.Error = ErrorCode.ERR_ConnectGateKeyError;
                response.Message = "Gate Key��֤ʧ��";
                reply();
                session?.Disconnect().Coroutine();
                return;
            }

            scene.GetComponent<GateSessionKeyComponent>().Remove(request.AccountId);//��Gate���������Ƴ�

            long instanceId = session.InstanceId;//��֤Э�������߼����ܵ��첽������Ӱ��
            using (session.AddComponent<SessionLockingComponent>())//�Ự�����(��Ϊ�����õ����첽�߼�),ͬһ���û��ĻỰ����ظ�����ʱ���������
            {
                using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginGate, request.AccountId.GetHashCode()))//Э�����������û������������������Ψһid,�û�id�Ĺ�ϣֵ����ֹ��ͬ��ַ���û�ͬʱ����ͬ���˺�ȥ��¼��
                {
                    if(instanceId != session.InstanceId)//��ֹ����ͻ��˵�¼ͬһ���˺�
                    {
                        return;
                    }

                    //֪ͨ��¼���ķ� ��¼���ε�¼�ķ�����Zone
                    StartSceneConfig loginCenterConfig = StartSceneConfigCategory.Instance.LoginCenterConfig;
                    L2G_AddLoginRecord l2G_AddLoginRecord = (L2G_AddLoginRecord)await MessageHelper.CallActor(loginCenterConfig.InstanceId, new G2L_AddLoginRecord()
                    {
                        AccountId = request.AccountId,
                        ServerId = scene.Zone,
                    });

                    if(l2G_AddLoginRecord.Error != ErrorCode.ERR_Success)//����б���
                    {
                        response.Error = l2G_AddLoginRecord.Error;
                        reply();
                        session?.Disconnect().Coroutine();
                        return;
                    }

                    Player player = scene.GetComponent<PlayerComponent>().Get(request.AccountId);//�����Ϸ�ͻ�����Gate�����ϵ�һ��ӳ��

                    if(player == null)
                    {
                        //���һ���µ�GateUnit
                        player = scene.GetComponent<PlayerComponent>().AddChildWithId<Player, long, long>(request.RoleId, request.AccountId, request.RoleId);
                        player.PlayerState = PlayerState.Gate;
                        scene.GetComponent<PlayerComponent>().Add(player);//���������ӽ�������
                        session.AddComponent<MailBoxComponent,MailboxType>(MailboxType.GateSession);//���Ự���Actor�������
                    }
                    else
                    {
                        player.RemoveComponent<PlayerOfflineOutTimeComponent>();//�Ƴ�player����ʱ�߳����
                    }

                    //��ӻỰ�����������������Ϣ����
                    session.AddComponent<SessionPlayerComponent>().PlayerId = player.Id;
                    session.GetComponent<SessionPlayerComponent>().PlayerInstanceId = player.InstanceId;
                    session.GetComponent<SessionPlayerComponent>().AccountId = request.AccountId;
                    player.SessionInstanceId = session.InstanceId;
                }

                reply();
            }

        }
    }
}