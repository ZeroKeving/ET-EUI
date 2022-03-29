
namespace ET
{
    public enum SessionState
    {
        Normal,//普通状态
        Game,//游戏状态
    }

    /// <summary>
    /// 会话状态组件
    /// </summary>
    public class SessionStateComponent : Entity, IAwake
    {
        public SessionState State;//会话状态
    }
}