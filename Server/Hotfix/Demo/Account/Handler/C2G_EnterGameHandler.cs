
using System;

namespace ET
{
    /// <summary>
    /// ������Ϸ��Ϣ�����߼�
    /// </summary>
    public class C2G_EnterGameHandler : AMRpcHandler<C2G_EnterGame, G2C_EnterGame>
    {
        protected override async ETTask Run(Session session, C2G_EnterGame request, G2C_EnterGame response, Action reply)
        {
            if (session.DomainScene().SceneType != SceneType.Gate)//����������Ϣ�Ƿ����󵽵���Gate���ط�����,��������򱨴�
            {
                Log.Error($"�����Scene���󣬵�ǰSceneΪ��{session.DomainScene().SceneType}");
                session.Dispose();//�Ͽ�����
                return;
            }

            if (session.GetComponent<SessionLockingComponent>() != null)//����Ự�������Ϊ�յ�����£�˵�����������յ��ͻ��˶������
            {
                response.Error = ErrorCode.ERR_RequestRepeatedly;//���ص�¼�����δ�����
                reply();//ί�з��ͻظ���Ϣresponse
                return;
            }

            SessionPlayerComponent sessionPlayerComponent = session.GetComponent<SessionPlayerComponent>();//�õ���һỰ���
            if (sessionPlayerComponent == null)
            {
                response.Error = ErrorCode.ERR_SessionPlayerError;
                reply();
                return;
            }

            Player player = Game.EventSystem.Get(sessionPlayerComponent.PlayerInstanceId) as Player;//��ȡ���ӳ��

            if (player == null || player.IsDisposed)
            {
                response.Error = ErrorCode.ERR_NonePlayerError;
                reply();
                return;
            }

            long instanceId = session.InstanceId;//��ûỰid
            using (session.AddComponent<SessionLockingComponent>())//�Ự�����(��Ϊ�����õ����첽�߼�),ͬһ���û��ĻỰ����ظ�����ʱ���������
            {
                using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginGate, player.AccountId.GetHashCode()))//Э�����������û������������������Ψһid,�û�id�Ĺ�ϣֵ����ֹ��ͬ��ַ���û�ͬʱ����ͬ���˺�ȥ��¼��
                {
                    if (instanceId != session.InstanceId || player.IsDisposed)//����Ựid���ı��˻��ͷ��˾ͷ���
                    {
                        response.Error = ErrorCode.ERR_PlayerSessionError;
                        reply();
                        return;
                    }

                    if(session.GetComponent<SessionStateComponent>() != null && session.GetComponent<SessionStateComponent>().State == SessionState.Game)//�Ự״̬�Ѿ�������Ϸ�߼���
                    {
                        response.Error = ErrorCode.ERR_SessionStateError;
                        reply();
                        return;
                    }

                    if(player.PlayerState == PlayerState.Game)//����Ѿ����뵽��Ϸ��������״̬����Ϊplayer����ӳ�������һ��session������session��Ψһ�ģ�����ж���ͻ��˶��ţ����session����ӳ�䵽һ��player�ϣ�
                    {
                        try
                        {
                            IActorResponse reqEnter = await MessageHelper.CallLocationActor(player.UnitId, new G2M_RequestEnterGameState());//�����ط���������Ϸ�߼���������ҷ���һ����Ϣ
                            if(reqEnter.Error == ErrorCode.ERR_Success)//���ε�¼�򶥺ŵ�¼�ɹ�
                            {
                                reply();
                                return;
                            }
                            Log.Error("���ε�¼ʧ��" + reqEnter.Error + "|" +reqEnter.Message);
                            response.Error = ErrorCode.ERR_ReEnterGameError;
                            await DisconnectHelper.KickPlayer(player, true);//�쳣�߳����
                            reply();
                            session?.Disconnect().Coroutine();//�ӳ�1��Ͽ�����
                        }
                        catch (Exception e)
                        {
                            Log.Error("���ε�¼ʧ��" + e.ToString());
                            response.Error = ErrorCode.ERR_ReEnterGameError2;
                            await DisconnectHelper.KickPlayer(player,true);//�쳣�߳����
                            reply();
                            session?.Disconnect().Coroutine();//�ӳ�1��Ͽ�����
                            throw;//�׳��쳣
                        }
                        return;
                    }

                    try
                    {
                        GateMapComponent gateMapComponent = player.AddComponent<GateMapComponent>();
                        gateMapComponent.Scene = await SceneFactory.Create(gateMapComponent,"GateMap",SceneType.Map);//����һ��Scene�����ص�ͼ�����ֵ

                        Unit unit = UnitFactory.Create(gateMapComponent.Scene,player.Id,UnitType.Player);//����һ����ҵ�ӳ�����ʵ��
                        unit.AddComponent<UnitGateComponent, long>(session.InstanceId);//�����ӳ������������������Ựʵ��id
                        long unitId = unit.Id;

                        StartSceneConfig startSceneConfig = StartSceneConfigCategory.Instance.GetBySceneName(session.DomainZone(), "Map1");//��ȡMap1��Ϸ�߼�������������
                        await TransferHelper.Transfer(unit, startSceneConfig.InstanceId, startSceneConfig.Name);//��unit�����͵���Ϸ�߼�������

                        //���ͳɹ���unitId�洢������
                        player.UnitId = unitId;
                        response.MyId = unitId;

                        reply();

                        //���Ự״̬�ı�Ϊ��Ϸ״̬
                        SessionStateComponent sessionStateComponent = session.GetComponent<SessionStateComponent>();
                        if(sessionStateComponent == null)
                        {
                            sessionStateComponent = session.AddComponent<SessionStateComponent>();
                        }
                        sessionStateComponent.State = SessionState.Game;

                        player.PlayerState = PlayerState.Game;//���ӳ��״̬��Ϊ��Ϸ״̬

                    }
                    catch (Exception e)
                    {
                        Log.Error($"��ɫ������Ϸ�߼����������� �˺�Id:{player.AccountId} ��ɫId:{player.Id} �쳣��Ϣ��{e.ToString()}");
                        response.Error = ErrorCode.ERR_EnterGameError;
                        reply();
                        await DisconnectHelper.KickPlayer(player,true);//���쳣���������
                        session.Disconnect().Coroutine();//�ӳ�һ���Ͽ�
                    }

                }
            }
        }
    }
}