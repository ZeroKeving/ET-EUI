
namespace ET
{
    /// <summary>
    /// ��¼��Ϣ���ɾ��ϵͳ
    /// </summary>
    public class AccountInfoComponentDetroySystem : DestroySystem<AccountInfoComponent>
    {
        /// <summary>
        /// ������е�¼��Ϣ
        /// </summary>
        /// <param name="self"></param>
        public override void Destroy(AccountInfoComponent self)
        {
            self.Token = string.Empty;
            self.AccountId = 0;
        }
    }

    /// <summary>
    /// ��¼��Ϣ���ϵͳ
    /// </summary>
    public class AccountInfoComponentSystem
    {

    }
}