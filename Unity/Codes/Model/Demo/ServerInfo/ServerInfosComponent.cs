
using System.Collections.Generic;

namespace ET
{
    /// <summary>
    /// 区服信息组件
    /// </summary>
    public class ServerInfosComponent :Entity,IAwake,IDestroy
    {
        public List<ServerInfo> ServerInfosList = new List<ServerInfo>();

        public int CurrentServerId = 1;//当前玩家要进入的服务器id
    }
}