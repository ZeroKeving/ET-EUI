
using System.Collections.Generic;

namespace ET
{
    /// <summary>
    /// 游戏区服信息管理组件
    /// </summary>
    public class ServerInfoManagerComponent : Entity,IAwake,IDestroy,ILoad
    {
        public List<ServerInfo> ServerInfos = new List<ServerInfo>();
    }
}