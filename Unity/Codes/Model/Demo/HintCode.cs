
namespace ET
{
    /// <summary>
    /// 客户端用的提示类型
    /// </summary>
    public static partial class HintCode
    {
        // 300001以上用于客户端提示
        public const int Null = 300001;//空提示码
        public const int RoleIsNull = 300002;//该账号未创建角色
        public const int RoleNameIsNull = 300003;//角色名称不能为空
        public const int RoleDeteteSucceed = 300004;//角色删除成功
    }
}