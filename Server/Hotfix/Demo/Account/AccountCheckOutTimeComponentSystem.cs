
using System;

namespace ET
{
    /// <summary>
    /// ����Ծ�û��߳���ʱ��������
    /// </summary>
    [Timer(TimerType.AccountSessionCheckOutTime)]
    public class AccountSessionCheckOutTime : ATimer<AccountCheckOutTimeComponent>
    {
        public override void Run(AccountCheckOutTimeComponent self)
        {
            try
            {
                self.DeleteSession();//10���Ӻ��߳�����Ծ��¼�û�
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
        }
    }

    /// <summary>
    /// ����Ծ�û��߳������ʼ��
    /// </summary>
    public class AccountCheckOutTimeComponentAwakeSystem : AwakeSystem<AccountCheckOutTimeComponent, long>
    {
        public override void Awake(AccountCheckOutTimeComponent self, long account)
        {
            self.AccountId = account;
            TimerComponent.Instance.Remove(ref self.Timer);//�رռ�ʱ���Դﵽ����Ŀ��
            self.Timer = TimerComponent.Instance.NewOnceTimer(TimeHelper.ServerNow()+600000,TimerType.AccountSessionCheckOutTime,self);//��ʼ��ʱ
        }
    }

    /// <summary>
    /// ����Ծ�û��߳��������
    /// </summary>
    public class AccountCheckOutTimeComponentDestroySystem : DestroySystem<AccountCheckOutTimeComponent>
    {
        /// <summary>
        /// ��û������ļ�ʱ�����ò��ر�
        /// </summary>
        /// <param name="self"></param>
        public override void Destroy(AccountCheckOutTimeComponent self)
        {
            self.AccountId = 0;
            TimerComponent.Instance.Remove(ref self.Timer);//�رռ�ʱ��
        }
    }


    /// <summary>
    /// ��¼һ��ʱ���޲����Ĳ���Ծ�û��߳����ϵͳ
    /// </summary>
    public static class AccountCheckOutTimeComponentSystem
    {
        /// <summary>
        /// ɾ���Ự
        /// </summary>
        /// <param name="self"></param>
        public static void DeleteSession(this AccountCheckOutTimeComponent self)
        {
            Session session = self.GetParent<Session>();

            long sessionInstanceId = session.DomainScene().GetComponent<AccountSessionsComponent>().Get(self.AccountId);//��ȡ�Ựʵ��id
            if(session.InstanceId == sessionInstanceId)//�������ʹ�õĻỰid�ͻỰ�ֵ����id��ͬ
            {
                session.DomainScene().GetComponent<AccountSessionsComponent>().Remove(self.AccountId);//���ûỰ�Ƴ�
            }
            session?.Send(new A2C_Disconnect() { Error = 1 });//����Ծ�û��߳�
            session?.Disconnect().Coroutine();//�ӳ�1���Ͽ�
        }
    }
}