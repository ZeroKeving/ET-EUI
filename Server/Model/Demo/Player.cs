namespace ET
{
    /// <summary>
    /// 玩家当前状态
    /// </summary>
    public enum PlayerState
    {
        Disconnect,//断开状态
        Gate,//连接着Gate网关的状态
        Game,//玩家已经进入到游戏服务器的状态
    }


    [ObjectSystem]
    public class PlayerSystem : AwakeSystem<Player, long, long>
    {
        public override void Awake(Player self, long a, long roleId)
        {
            self.AccountId = a;
            self.UnitId = roleId;
        }
    }

    public sealed class Player : Entity, IAwake<string>, IAwake<long, long>
    {
        public long AccountId { get; set; }//玩家账号id

        public long SessionInstanceId { get; set; }//玩家会话连接id

        public long UnitId { get; set; }

        public PlayerState PlayerState { get; set; }//玩家状态的枚举


    }
}