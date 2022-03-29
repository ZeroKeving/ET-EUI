
namespace ET
{
    public static class ServerInfoSystem
    {
        /// <summary>
        /// ��ȡ��Ϣ������
        /// </summary>
        /// <param name="self"></param>
        /// <param name="serverInfoProto"></param>
        public static void FromMessage(this ServerInfo self,ServerInfoProto serverInfoProto)
        {
            self.Id = serverInfoProto.Id;
            self.Status = serverInfoProto.Status;
            self.ServerName = serverInfoProto.ServerName;
        }

        /// <summary>
        /// �����ݱ�дΪ��Ϣ����
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static ServerInfoProto ToMessage(this ServerInfo self)
        {
            return new ServerInfoProto() { Id = (int)self.Id, ServerName = self.ServerName, Status = self.Status };
        }
    }
}