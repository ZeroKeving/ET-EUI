
namespace ET
{
    /// <summary>
    /// 游戏区服信息管理组件系统初始化
    /// </summary>
    public class ServerInfoManagerComponentAwakeSystem : AwakeSystem<ServerInfoManagerComponent>
    {
        public override void Awake(ServerInfoManagerComponent self)
        {
            self.Awake().Coroutine();
        }
    }

    /// <summary>
    /// 游戏区服信息管理组件系统销毁
    /// </summary>
    public class ServerInfoManagerComponentDestroySystem : DestroySystem<ServerInfoManagerComponent>
    {
        public override void Destroy(ServerInfoManagerComponent self)
        {
            foreach (var serverInfo in self.ServerInfos)
            {
                serverInfo?.Dispose();
            }
            self.ServerInfos.Clear();
        }
    }

    /// <summary>
    /// 游戏区服信息管理组件系统热重载
    /// </summary>
    public class ServerInfoManagerComponentLoadSystem : LoadSystem<ServerInfoManagerComponent>
    {
        public override void Load(ServerInfoManagerComponent self)
        {
            self.Awake().Coroutine();
        }
    }


    /// <summary>
    /// 游戏区服信息管理组件系统
    /// </summary>
    public static class ServerInfoManagerComponentSystem
    {
        public static async ETTask Awake(this ServerInfoManagerComponent self)
        {
            var serverInfoList = await DBManagerComponent.Instance.GetZoneDB(self.DomainZone()).Query<ServerInfo>(d => true);//在数据库中获取游戏区服信息列表

            if (self.ServerInfos.Count != 0)//如果原本存有服务器区服信息的话就进行重置
            {
                foreach (var serverInfo in self.ServerInfos)
                {
                    serverInfo?.Dispose();
                }
            }
            self.ServerInfos.Clear();//重置原本的信息

            if (serverInfoList == null || serverInfoList.Count <= 0)//判断数据库中是否有游戏区服信息，如果没有
            {
                var serverInfoConfig = ServerInfoConfigCategory.Instance.GetAll();//获取游戏区服信息

                foreach (var info in serverInfoConfig.Values)
                {
                    ServerInfo newServerInfo = self.AddChildWithId<ServerInfo>(info.Id);//使用遍历到的游戏区服配置id来创建游戏区服信息
                    newServerInfo.ServerName = info.ServerName;
                    newServerInfo.Status = (int)ServerStatus.Normal;
                    self.ServerInfos.Add(newServerInfo);
                    await DBManagerComponent.Instance.GetZoneDB(self.DomainZone()).Save(newServerInfo);//将配置里的区服存入数据库
                }
            }

            foreach (var serverInfo in serverInfoList)//将列表中的服务器区服信息全部存入到游戏区服信息管理组件
            {
                self.AddChild(serverInfo);
                self.ServerInfos.Add(serverInfo);
            }

            await ETTask.CompletedTask;
        }
    }
}