
namespace ET
{
    /// <summary>
    /// ��¼�Ự���ϵͳ���ٺ���
    /// </summary>
    public class AccountSessionsComponentDestroySystem : DestroySystem<AccountSessionsComponent>
    {
        public override void Destroy(AccountSessionsComponent self)
        {
            self.AccountSessionDict.Clear();
        }
    }

    /// <summary>
    /// ��¼�Ự���ϵͳ
    /// </summary>
    public static class AccountSessionsComponentSystem
    {
        /// <summary>
        /// ��ȡָ����¼id�ĻỰ
        /// </summary>
        /// <param name="self"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public static long Get(this AccountSessionsComponent self,long accountId)
        {
            if(!self.AccountSessionDict.TryGetValue(accountId,out long instanceId))//��ȡ��ʵ��id
            {
                return 0;
            }
            return instanceId;
        }

        /// <summary>
        /// ���¼�Ự��������ָ����¼�Ự
        /// </summary>
        /// <param name="self"></param>
        /// <param name="accountId"></param>
        /// <param name="sessionInstanceId"></param>
        public static void Add(this AccountSessionsComponent self,long accountId,long sessionInstanceId)
        {
            if(self.AccountSessionDict.ContainsKey(accountId))//������ڶ�Ӧ��
            {
                self.AccountSessionDict[accountId] = sessionInstanceId;//��ʵ��id���и���
                return;
            }
            self.AccountSessionDict.Add(accountId,sessionInstanceId);//��������ڶ�Ӧ���������
        }

        /// <summary>
        /// �Ƴ���¼�Ự�����ָ���ĵ�¼�Ự
        /// </summary>
        /// <param name="self"></param>
        /// <param name="accountId"></param>
        public static void Remove(this AccountSessionsComponent self, long accountId)
        {
            if(self.AccountSessionDict.ContainsKey((accountId)))//�жϵ�¼�Ự������Ƿ��иûỰ
            {
                self.AccountSessionDict.Remove((accountId));//�Ƴ��ûỰ
            }
        }
    }
}