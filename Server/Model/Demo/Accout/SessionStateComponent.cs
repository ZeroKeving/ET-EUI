
namespace ET
{
    public enum SessionState
    {
        Normal,//��ͨ״̬
        Game,//��Ϸ״̬
    }

    /// <summary>
    /// �Ự״̬���
    /// </summary>
    public class SessionStateComponent : Entity, IAwake
    {
        public SessionState State;//�Ự״̬
    }
}