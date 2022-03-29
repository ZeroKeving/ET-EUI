
using System;

namespace ET
{
    /// <summary>
    /// 类型为PlayerOfflineOutTime的定时器任务
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
    /// 玩家定时踢出组件系统销毁
    /// </summary>
    public class PlayerOfflineOutTimeComponentDestorySystem : DestroySystem<PlayerOfflineOutTimeComponent>
    {
        public override void Destroy(PlayerOfflineOutTimeComponent self)
        {
            TimerComponent.Instance.Remove(ref self.Timer);//计时移除
        }
    }


    /// <summary>
    /// 玩家定时踢出组件系统初始化
    /// </summary>
    public class PlayerOfflineOutTimeComponentAwakeSystem : AwakeSystem<PlayerOfflineOutTimeComponent>
    {
        public override void Awake(PlayerOfflineOutTimeComponent self)
        {
            self.Timer = TimerComponent.Instance.NewOnceTimer(TimeHelper.ServerNow() + 10000, TimerType.PlayerOfflineOutTime, self);//10秒后会执行一个类型为PlayerOfflineOutTime的定时器任务
        }
    }

    /// <summary>
    /// 玩家定时踢出组件系统
    /// </summary>
    public static class PlayerOfflineOutTimeComponentSystem
    {
        /// <summary>
        /// 踢出玩家
        /// </summary>
        /// <param name="self"></param>
        public static void KickPlayer(this PlayerOfflineOutTimeComponent self)
        {
            DisconnectHelper.KickPlayer(self.GetParent<Player>()).Coroutine();
        }
    }
}