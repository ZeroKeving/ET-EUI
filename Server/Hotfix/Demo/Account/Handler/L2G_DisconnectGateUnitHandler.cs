
using System;

namespace ET
{
    /// <summary>
    /// ��¼���ķ�������Gate���ط��������Ͷ���������Ϣ�Ĵ���
    /// </summary>
    public class L2G_DisconnectGateUnitHandler : AMActorRpcHandler<Scene, L2G_DisconnectGateUnit, G2L_DisconnectGateUnit>
    {
        protected override async ETTask Run(Scene scene, L2G_DisconnectGateUnit request, G2L_DisconnectGateUnit response, Action reply)
        {
            long accountId = request.AccountId;

            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginGate, accountId.GetHashCode()))//Э����
            {
                PlayerComponent playerComponent = scene.GetComponent<PlayerComponent>();//���������
                Player player = playerComponent.Get(accountId);//��ȡgate�����ϸ��˺����ӵ���Ϸ��ɫ

                if(player == null)//��������������ӵ���Ϸ��ɫ,ֱ�ӻظ�
                {
                    reply();
                    return;
                }

                scene.GetComponent<GateSessionKeyComponent>().Remove(accountId);//�Ƴ��ý�ɫ��Gate���ص�¼����
                Session gateSession = Game.EventSystem.Get(player.SessionInstanceId) as Session;//ͨ��palyer�ĻỰid�õ��Ự����
                if(gateSession != null && !gateSession.IsDisposed)//����Ự���Ӳ�Ϊ��û�б��ͷ�
                {
                    gateSession.Send(new A2C_Disconnect() { Error = ErrorCode.ERR_OtherAccountLogin });
                    gateSession?.Disconnect().Coroutine();//�ӳ�1���Ͽ�
                }

                player.SessionInstanceId = 0;//��ո���ҵĻỰ����id
                player.AddComponent<PlayerOfflineOutTimeComponent>();//�����������϶�ʱ�߳����
            }

            reply();

        }
    }
}