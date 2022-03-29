
using System;

namespace ET
{
    /// <summary>
    /// 不活跃用户踢出定时器任务函数
    /// </summary>
    [Timer(TimerType.AccountSessionCheckOutTime)]
    public class AccountSessionCheckOutTime : ATimer<AccountCheckOutTimeComponent>
    {
        public override void Run(AccountCheckOutTimeComponent self)
        {
            try
            {
                self.DeleteSession();//10分钟后踢出不活跃登录用户
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
        }
    }

    /// <summary>
    /// 不活跃用户踢出组件初始化
    /// </summary>
    public class AccountCheckOutTimeComponentAwakeSystem : AwakeSystem<AccountCheckOutTimeComponent, long>
    {
        public override void Awake(AccountCheckOutTimeComponent self, long account)
        {
            self.AccountId = account;
            TimerComponent.Instance.Remove(ref self.Timer);//关闭计时器以达到重置目的
            self.Timer = TimerComponent.Instance.NewOnceTimer(TimeHelper.ServerNow()+600000,TimerType.AccountSessionCheckOutTime,self);//开始计时
        }
    }

    /// <summary>
    /// 不活跃用户踢出组件销毁
    /// </summary>
    public class AccountCheckOutTimeComponentDestroySystem : DestroySystem<AccountCheckOutTimeComponent>
    {
        /// <summary>
        /// 将没进行完的计时器重置并关闭
        /// </summary>
        /// <param name="self"></param>
        public override void Destroy(AccountCheckOutTimeComponent self)
        {
            self.AccountId = 0;
            TimerComponent.Instance.Remove(ref self.Timer);//关闭计时器
        }
    }


    /// <summary>
    /// 登录一段时间无操作的不活跃用户踢出组件系统
    /// </summary>
    public static class AccountCheckOutTimeComponentSystem
    {
        /// <summary>
        /// 删除会话
        /// </summary>
        /// <param name="self"></param>
        public static void DeleteSession(this AccountCheckOutTimeComponent self)
        {
            Session session = self.GetParent<Session>();

            long sessionInstanceId = session.DomainScene().GetComponent<AccountSessionsComponent>().Get(self.AccountId);//获取会话实例id
            if(session.InstanceId == sessionInstanceId)//如果正在使用的会话id和会话字典里的id相同
            {
                session.DomainScene().GetComponent<AccountSessionsComponent>().Remove(self.AccountId);//将该会话移除
            }
            session?.Send(new A2C_Disconnect() { Error = 1 });//不活跃用户踢出
            session?.Disconnect().Coroutine();//延迟1秒后断开
        }
    }
}