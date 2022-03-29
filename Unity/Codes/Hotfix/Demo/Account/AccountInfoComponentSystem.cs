
namespace ET
{
    /// <summary>
    /// 登录信息组件删除系统
    /// </summary>
    public class AccountInfoComponentDetroySystem : DestroySystem<AccountInfoComponent>
    {
        /// <summary>
        /// 清空所有登录信息
        /// </summary>
        /// <param name="self"></param>
        public override void Destroy(AccountInfoComponent self)
        {
            self.Token = string.Empty;
            self.AccountId = 0;
        }
    }

    /// <summary>
    /// 登录信息组件系统
    /// </summary>
    public class AccountInfoComponentSystem
    {

    }
}