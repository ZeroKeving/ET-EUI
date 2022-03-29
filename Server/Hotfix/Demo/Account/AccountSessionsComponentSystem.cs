
namespace ET
{
    /// <summary>
    /// 登录会话组件系统销毁函数
    /// </summary>
    public class AccountSessionsComponentDestroySystem : DestroySystem<AccountSessionsComponent>
    {
        public override void Destroy(AccountSessionsComponent self)
        {
            self.AccountSessionDict.Clear();
        }
    }

    /// <summary>
    /// 登录会话组件系统
    /// </summary>
    public static class AccountSessionsComponentSystem
    {
        /// <summary>
        /// 获取指定登录id的会话
        /// </summary>
        /// <param name="self"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public static long Get(this AccountSessionsComponent self,long accountId)
        {
            if(!self.AccountSessionDict.TryGetValue(accountId,out long instanceId))//获取到实例id
            {
                return 0;
            }
            return instanceId;
        }

        /// <summary>
        /// 向登录会话组件里添加指定登录会话
        /// </summary>
        /// <param name="self"></param>
        /// <param name="accountId"></param>
        /// <param name="sessionInstanceId"></param>
        public static void Add(this AccountSessionsComponent self,long accountId,long sessionInstanceId)
        {
            if(self.AccountSessionDict.ContainsKey(accountId))//如果纯在对应键
            {
                self.AccountSessionDict[accountId] = sessionInstanceId;//对实例id进行覆盖
                return;
            }
            self.AccountSessionDict.Add(accountId,sessionInstanceId);//如果不存在对应键，就添加
        }

        /// <summary>
        /// 移除登录会话组件里指定的登录会话
        /// </summary>
        /// <param name="self"></param>
        /// <param name="accountId"></param>
        public static void Remove(this AccountSessionsComponent self, long accountId)
        {
            if(self.AccountSessionDict.ContainsKey((accountId)))//判断登录会话组件里是否有该会话
            {
                self.AccountSessionDict.Remove((accountId));//移除该会话
            }
        }
    }
}