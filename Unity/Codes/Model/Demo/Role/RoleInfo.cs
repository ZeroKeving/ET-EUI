

namespace ET
{
    /// <summary>
    /// 角色账号状态
    /// </summary>
    public enum RoleInfoState
    {
        Normal = 0,//正常状态
        Freeze = 1,//冻结状态
        Banned = 2,//封禁状态
    }

    /// <summary>
    /// 游戏角色信息
    /// </summary>
    public class RoleInfo : Entity,IAwake
    {
        public long UserId;//角色唯一id
        public string Name;//角色名称
        public int ServerId;//所在区服Id
        public int State;//角色当前状态
        public long AccountId;//账号id
        public long LastLoginTime;//角色上次登录的时间
        public long CreateTime;//角色创建时间
    }
}