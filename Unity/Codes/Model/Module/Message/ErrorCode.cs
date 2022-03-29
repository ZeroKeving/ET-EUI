namespace ET
{
    public static partial class ErrorCode
    {
        public const int ERR_Success = 0;

        // 1-11004 是SocketError请看SocketError定义
        //-----------------------------------
        // 100000-109999是Core层的错误

        // 110000以下的错误请看ErrorCore.cs

        // 这里配置逻辑层的错误码
        // 110000 - 200000是抛异常的错误
        // 200001以上不抛异常
        // 300001以上用于客户端提示

        public const int ERR_NetWorkError = 200002;//网络异常
        public const int ERR_LoginInfoIsNull = 200003;//登录信息错误
        public const int ERR_LoginAccountNameFormError = 200004;//登录账号格式错误
        public const int ERR_LoginPasswordFormError = 200005;//登录密码格式错误
        public const int ERR_LoginBlackListError = 200006;//登录账号属于黑名单
        public const int ERR_LoginPasswordError = 200007;//登录密码错误
        public const int ERR_RequestRepeatedly = 200008;//频繁请求多次
        public const int ERR_TokenError = 200009;//登录令牌错误

        public const int ERR_RoleNameRepeat = 200010;//游戏角色名字重复
        public const int ERR_RoleNameFormError = 200011;//角色名称格式错误
        public const int ERR_RoleNotExist = 200012;//游戏角色不存在

        public const int ERR_RequestSceneTypeError = 200013;//请求服务器类型错误
        public const int ERR_ConnectGateKeyError = 200014;//Gate网关令牌错误
        public const int ERR_OtherAccountLogin = 200015;//其他玩家登录该账号

        public const int ERR_SessionPlayerError = 200016;//与玩家通信错误
        public const int ERR_NonePlayerError = 200017;//未找到玩家映射
        public const int ERR_PlayerSessionError = 200018;//玩家会话连接错误
        public const int ERR_SessionStateError = 200019;//会话状态错误
        public const int ERR_EnterGameError = 200020;//玩家进入游戏逻辑服出现问题
        public const int ERR_ReEnterGameError = 200021;//二次登录失败
        public const int ERR_ReEnterGameError2 = 200022;//二次登录失败底层错误
    }
}