
namespace ET
{
    /// <summary>
    /// ������������Ϣ�������
    /// </summary>
    public class ServerInfosComponentDestroySystem : DestroySystem<ServerInfosComponent>
    {
        public override void Destroy(ServerInfosComponent self)
        {
            foreach (var serverInfo in self.ServerInfosList)
            {
                serverInfo.Dispose();
            }
            self.ServerInfosList.Clear();
        }
    }

    /// <summary>
    /// ������������Ϣ���ϵͳ
    /// </summary>
    public static class ServerInfosComponentSystem
    {
        /// <summary>
        /// ��ӷ�����������Ϣ
        /// </summary>
        /// <param name="self"></param>
        /// <param name="serverInfo"></param>
        public static void Add(this ServerInfosComponent self,ServerInfo serverInfo)
        {
            self.ServerInfosList.Add(serverInfo);
        }
    }
}