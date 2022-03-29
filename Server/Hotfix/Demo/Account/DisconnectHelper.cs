
namespace ET
{
    /// <summary>
    /// �Ͽ�����������
    /// </summary>
    public static class DisconnectHelper
    {
        /// <summary>
        /// ��ԻỰ����һ����չ,�ӳ�1���Ͽ�����
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static async ETTask Disconnect(this Session self)
        {
            if (self == null || self.IsDisposed)//�жϻỰ�Ƿ�Ϊ�ջ��Ƿ񱻶Ͽ�
            {
                return;
            }

            long instanceId = self.InstanceId;//��¼һ�»Ự��id

            await TimerComponent.Instance.WaitAsync(1000);//�ȴ�1��

            if(self.InstanceId != instanceId)//�жϵ�ǰ�Ự��id�Ƿ����֮ǰ�Ựid,������ֱ�ӷ���
            {
                return ;
            }

            self.Dispose();
        }

        /// <summary>
        /// �߳����
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static async ETTask KickPlayer(Player player)
        {
            if(player == null || player.IsDisposed)//�����Ҳ����ڻ��Ѿ����ͷ�
            {
                return;
            }
            long instanceId = player.InstanceId;
            using(await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginGate,player.Account.GetHashCode()))//Э����
            {
                if (player == null || instanceId != player.InstanceId)//�����Ҳ����ڻ��Ѿ����ͷ�(�첽�������ܱ��ظ����ö��,����Ҫ�ж��Ƿ��ظ�ִ��)
                {
                    return;
                }
                switch(player.PlayerState)
                {
                    case PlayerState.Disconnect:
                        break;
                    case PlayerState.Gate:
                        break;
                    case PlayerState.Game:
                        //TODD ֪ͨ��Ϸ�߼�������Unit��ɫ�߼����������ݴ������ݿ�
                        break;
                }

                player.PlayerState = PlayerState.Disconnect;
                player.DomainScene().GetComponent<PlayerComponent>()?.Remove(player.Account);
                player?.Dispose();
                await TimerComponent.Instance.WaitAsync(300);
            }
        }
    }
}