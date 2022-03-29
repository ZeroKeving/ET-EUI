
namespace ET
{
    public enum ServerStatus
    {
        Normal = 0,//正常状态
        Stop = 1,//停服状态
    }

    /// <summary>
    /// 服务器区服信息
    /// </summary>
    public class ServerInfo : Entity,IAwake
    {
        public int Status;
        public string ServerName;
    }
}