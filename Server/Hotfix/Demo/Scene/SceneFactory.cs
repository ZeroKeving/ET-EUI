

using System.Net;

namespace ET
{
    /// <summary>
    /// 场景工厂类
    /// </summary>
    public static class SceneFactory
    {
        public static async ETTask<Scene> Create(Entity parent, string name, SceneType sceneType)
        {
            long instanceId = IdGenerater.Instance.GenerateInstanceId();
            return await Create(parent, instanceId, instanceId, parent.DomainZone(), name, sceneType);
        }
        
        /// <summary>
        /// 创建游戏服务端中的各种zoneScene
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="id"></param>
        /// <param name="instanceId"></param>
        /// <param name="zone"></param>
        /// <param name="name"></param>
        /// <param name="sceneType"></param>
        /// <param name="startSceneConfig"></param>
        /// <returns></returns>
        public static async ETTask<Scene> Create(Entity parent, long id, long instanceId, int zone, string name, SceneType sceneType, StartSceneConfig startSceneConfig = null)
        {
            await ETTask.CompletedTask;
            Scene scene = EntitySceneFactory.CreateScene(id, instanceId, zone, sceneType, name, parent);

            scene.AddComponent<MailBoxComponent, MailboxType>(MailboxType.UnOrderMessageDispatcher);//可以处理和接收消息

            //创建4种zoneScene
            switch (scene.SceneType)
            {
                case SceneType.Realm://网关负载均衡服务器(用来给账号服务器的会话链接动态分配不同的gate网关服务器)
                    scene.AddComponent<NetKcpComponent, IPEndPoint, int>(startSceneConfig.OuterIPPort, SessionStreamDispatcherType.SessionStreamDispatcherServerOuter);
                    scene.AddComponent<TokenComponent>();//添加令牌组件
                    break;
                case SceneType.Gate://Gate网关服务器1
                    scene.AddComponent<NetKcpComponent, IPEndPoint, int>(startSceneConfig.OuterIPPort, SessionStreamDispatcherType.SessionStreamDispatcherServerOuter);
                    scene.AddComponent<PlayerComponent>();
                    scene.AddComponent<GateSessionKeyComponent>();
                    break;
                case SceneType.Map://地图服务器
                    scene.AddComponent<UnitComponent>();
                    scene.AddComponent<AOIManagerComponent>();
                    //scene.AddComponent<DBManagerComponent>();//如果是分布式多进程服务器，可以给Map添加上DB数据库控制组件
                    break;
                case SceneType.Location://位置服务器
                    scene.AddComponent<LocationComponent>();
                    break;
                case SceneType.Account://账号登录服务器
                    scene.AddComponent<NetKcpComponent, IPEndPoint, int>(startSceneConfig.OuterIPPort, SessionStreamDispatcherType.SessionStreamDispatcherServerOuter);//添加NetKcp组件，玩家可以直接连接到账号登录服务器上
                    //scene.AddComponent<DBManagerComponent>();//添加上DB数据库控制组件（因为是单例，所以只需要挂载在账号服务器就可以了，多进程挂在Map服务器，也可以挂在AppStart里）
                    scene.AddComponent<TokenComponent>();//添加登录令牌组件
                    scene.AddComponent<AccountSessionsComponent>();//添加登录会话组件
                    scene.AddComponent<ServerInfoManagerComponent>();//添加游戏区服消息管理组件
                    break;
                case SceneType.LoginCenter://账号中心服务器
                    scene.AddComponent<LoginInfoRecordComponent>();//添加登录信息记录组件
                    break;
            }

            return scene;
        }
    }
}