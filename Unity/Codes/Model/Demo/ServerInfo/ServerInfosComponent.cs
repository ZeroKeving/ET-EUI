
using System.Collections.Generic;

namespace ET
{
    /// <summary>
    /// ������Ϣ���
    /// </summary>
    public class ServerInfosComponent :Entity,IAwake,IDestroy
    {
        public List<ServerInfo> ServerInfosList = new List<ServerInfo>();

        public int CurrentServerId = 1;//��ǰ���Ҫ����ķ�����id
    }
}