namespace ET
{
    public static class CoroutineLockType
    {
        public const int None = 0;
        public const int Location = 1;                  // location进程上使用
        public const int ActorLocationSender = 2;       // ActorLocationSender中队列消息 
        public const int Mailbox = 3;                   // Mailbox中队列
        public const int UnitId = 4;                    // Map服务器上线下线时使用
        public const int DB = 5;
        public const int Resources = 6;
        public const int ResourcesLoader = 7;

        public const int LoginAccount = 8;//登录账号的协程锁类型（获得Realm均衡负载服务器的令牌和地址时也要用）
        public const int LoginCenterLock = 9;//登录中心服务器的协程锁类型（移除登录信息也要用）
        public const int GateLoginLock = 10;//网关服务器登录锁
        public const int CreateRole = 11;//登录服务器创建角色锁(角色信息查询和删除时也要用，防止在查询时创建)
        public const int LoginRealm = 12;//登录Realm服务器锁
        public const int LoginGate = 13;//登录Gate网关服务器锁（顶号踢人也要用,进入游戏也要用）

        public const int Max = 100; // 这个必须最大
    }
}