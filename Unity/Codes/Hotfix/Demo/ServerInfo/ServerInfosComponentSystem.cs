
namespace ET
{
    /// <summary>
    /// 服务器区服信息组件销毁
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
    /// 服务器区服信息组件系统
    /// </summary>
    public static class ServerInfosComponentSystem
    {
        /// <summary>
        /// 添加服务器区服信息
        /// </summary>
        /// <param name="self"></param>
        /// <param name="serverInfo"></param>
        public static void Add(this ServerInfosComponent self,ServerInfo serverInfo)
        {
            self.ServerInfosList.Add(serverInfo);
        }
    }
}