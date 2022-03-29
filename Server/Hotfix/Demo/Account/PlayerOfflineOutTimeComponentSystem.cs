
using System;

namespace ET
{
    /// <summary>
    /// ����ΪPlayerOfflineOutTime�Ķ�ʱ������
    /// </summary>
    [Timer(TimerType.PlayerOfflineOutTime)]
    public class PlayerOfflineOutTime : ATimer<PlayerOfflineOutTimeComponent>
    {
        public override void Run(PlayerOfflineOutTimeComponent self)
        {
            try
            {
                self.KickPlayer();
            }
            catch (Exception e)
            {
                Log.Error($"PlayerOfflineOutTime error:{self.Id}\n{e}");
                throw;
            }
        }
    }

    /// <summary>
    /// ��Ҷ�ʱ�߳����ϵͳ����
    /// </summary>
    public class PlayerOfflineOutTimeComponentDestorySystem : DestroySystem<PlayerOfflineOutTimeComponent>
    {
        public override void Destroy(PlayerOfflineOutTimeComponent self)
        {
            TimerComponent.Instance.Remove(ref self.Timer);//��ʱ�Ƴ�
        }
    }


    /// <summary>
    /// ��Ҷ�ʱ�߳����ϵͳ��ʼ��
    /// </summary>
    public class PlayerOfflineOutTimeComponentAwakeSystem : AwakeSystem<PlayerOfflineOutTimeComponent>
    {
        public override void Awake(PlayerOfflineOutTimeComponent self)
        {
            self.Timer = TimerComponent.Instance.NewOnceTimer(TimeHelper.ServerNow() + 10000, TimerType.PlayerOfflineOutTime, self);//10����ִ��һ������ΪPlayerOfflineOutTime�Ķ�ʱ������
        }
    }

    /// <summary>
    /// ��Ҷ�ʱ�߳����ϵͳ
    /// </summary>
    public static class PlayerOfflineOutTimeComponentSystem
    {
        /// <summary>
        /// �߳����
        /// </summary>
        /// <param name="self"></param>
        public static void KickPlayer(this PlayerOfflineOutTimeComponent self)
        {
            DisconnectHelper.KickPlayer(self.GetParent<Player>()).Coroutine();
        }
    }
}