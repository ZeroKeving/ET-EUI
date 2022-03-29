
namespace ET
{
    //游戏账号类型
    public enum AccountType
    {
        General = 0,//普通玩家

        BlackList = 100,//黑名单玩家
    }

    /// <summary>
    /// 账号数据
    /// </summary>
    public class Account : Entity,IAwake
    {
        public string AccountName;//账号

        public string Password;//密码

        public long CreateTime;//账号创建时间

        public int AccountType;//账号类型
    }
}