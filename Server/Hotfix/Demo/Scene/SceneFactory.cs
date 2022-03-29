

using System.Net;

namespace ET
{
    /// <summary>
    /// ����������
    /// </summary>
    public static class SceneFactory
    {
        public static async ETTask<Scene> Create(Entity parent, string name, SceneType sceneType)
        {
            long instanceId = IdGenerater.Instance.GenerateInstanceId();
            return await Create(parent, instanceId, instanceId, parent.DomainZone(), name, sceneType);
        }
        
        /// <summary>
        /// ������Ϸ������еĸ���zoneScene
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

            scene.AddComponent<MailBoxComponent, MailboxType>(MailboxType.UnOrderMessageDispatcher);//���Դ���ͽ�����Ϣ

            //����4��zoneScene
            switch (scene.SceneType)
            {
                case SceneType.Realm://���ظ��ؾ��������(�������˺ŷ������ĻỰ���Ӷ�̬���䲻ͬ��gate���ط�����)
                    scene.AddComponent<NetKcpComponent, IPEndPoint, int>(startSceneConfig.OuterIPPort, SessionStreamDispatcherType.SessionStreamDispatcherServerOuter);
                    scene.AddComponent<TokenComponent>();//����������
                    break;
                case SceneType.Gate://Gate���ط�����1
                    scene.AddComponent<NetKcpComponent, IPEndPoint, int>(startSceneConfig.OuterIPPort, SessionStreamDispatcherType.SessionStreamDispatcherServerOuter);
                    scene.AddComponent<PlayerComponent>();
                    scene.AddComponent<GateSessionKeyComponent>();
                    break;
                case SceneType.Map://��ͼ������
                    scene.AddComponent<UnitComponent>();
                    scene.AddComponent<AOIManagerComponent>();
                    //scene.AddComponent<DBManagerComponent>();//����Ƿֲ�ʽ����̷����������Ը�Map�����DB���ݿ�������
                    break;
                case SceneType.Location://��λ������
                    scene.AddComponent<LocationComponent>();
                    break;
                case SceneType.Account://�˺ŵ�¼������
                    scene.AddComponent<NetKcpComponent, IPEndPoint, int>(startSceneConfig.OuterIPPort, SessionStreamDispatcherType.SessionStreamDispatcherServerOuter);//���NetKcp�������ҿ���ֱ�����ӵ��˺ŵ�¼��������
                    //scene.AddComponent<DBManagerComponent>();//�����DB���ݿ�����������Ϊ�ǵ���������ֻ��Ҫ�������˺ŷ������Ϳ����ˣ�����̹���Map��������Ҳ���Թ���AppStart�
                    scene.AddComponent<TokenComponent>();//��ӵ�¼�������
                    scene.AddComponent<AccountSessionsComponent>();//��ӵ�¼�Ự���
                    scene.AddComponent<ServerInfoManagerComponent>();//�����Ϸ������Ϣ�������
                    break;
                case SceneType.LoginCenter://�˺����ķ�����
                    scene.AddComponent<LoginInfoRecordComponent>();//��ӵ�¼��Ϣ��¼���
                    break;
            }

            return scene;
        }
    }
}