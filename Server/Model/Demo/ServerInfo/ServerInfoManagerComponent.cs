
using System.Collections.Generic;

namespace ET
{
    /// <summary>
    /// ��Ϸ������Ϣ�������
    /// </summary>
    public class ServerInfoManagerComponent : Entity,IAwake,IDestroy,ILoad
    {
        public List<ServerInfo> ServerInfos = new List<ServerInfo>();
    }
}