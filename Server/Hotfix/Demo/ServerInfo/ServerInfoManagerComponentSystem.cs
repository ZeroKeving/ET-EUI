
namespace ET
{
    /// <summary>
    /// ��Ϸ������Ϣ�������ϵͳ��ʼ��
    /// </summary>
    public class ServerInfoManagerComponentAwakeSystem : AwakeSystem<ServerInfoManagerComponent>
    {
        public override void Awake(ServerInfoManagerComponent self)
        {
            self.Awake().Coroutine();
        }
    }

    /// <summary>
    /// ��Ϸ������Ϣ�������ϵͳ����
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
    /// ��Ϸ������Ϣ�������ϵͳ������
    /// </summary>
    public class ServerInfoManagerComponentLoadSystem : LoadSystem<ServerInfoManagerComponent>
    {
        public override void Load(ServerInfoManagerComponent self)
        {
            self.Awake().Coroutine();
        }
    }


    /// <summary>
    /// ��Ϸ������Ϣ�������ϵͳ
    /// </summary>
    public static class ServerInfoManagerComponentSystem
    {
        public static async ETTask Awake(this ServerInfoManagerComponent self)
        {
            var serverInfoList = await DBManagerComponent.Instance.GetZoneDB(self.DomainZone()).Query<ServerInfo>(d => true);//�����ݿ��л�ȡ��Ϸ������Ϣ�б�

            if (self.ServerInfos.Count != 0)//���ԭ�����з�����������Ϣ�Ļ��ͽ�������
            {
                foreach (var serverInfo in self.ServerInfos)
                {
                    serverInfo?.Dispose();
                }
            }
            self.ServerInfos.Clear();//����ԭ������Ϣ

            if (serverInfoList == null || serverInfoList.Count <= 0)//�ж����ݿ����Ƿ�����Ϸ������Ϣ�����û��
            {
                var serverInfoConfig = ServerInfoConfigCategory.Instance.GetAll();//��ȡ��Ϸ������Ϣ

                foreach (var info in serverInfoConfig.Values)
                {
                    ServerInfo newServerInfo = self.AddChildWithId<ServerInfo>(info.Id);//ʹ�ñ���������Ϸ��������id��������Ϸ������Ϣ
                    newServerInfo.ServerName = info.ServerName;
                    newServerInfo.Status = (int)ServerStatus.Normal;
                    self.ServerInfos.Add(newServerInfo);
                    await DBManagerComponent.Instance.GetZoneDB(self.DomainZone()).Save(newServerInfo);//��������������������ݿ�
                }
            }

            foreach (var serverInfo in serverInfoList)//���б��еķ�����������Ϣȫ�����뵽��Ϸ������Ϣ�������
            {
                self.AddChild(serverInfo);
                self.ServerInfos.Add(serverInfo);
            }

            await ETTask.CompletedTask;
        }
    }
}