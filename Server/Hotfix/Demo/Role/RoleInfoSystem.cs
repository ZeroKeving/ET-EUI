
namespace ET
{
    /// <summary>
    /// ��ɫ��Ϣϵͳ
    /// </summary>
    public static class RoleInfoSystem
    {
        /// <summary>
        /// ����Ϣ���ݴ����ɫ��Ϣϵͳ
        /// </summary>
        /// <param name="self"></param>
        /// <param name="roleInfoProto"></param>
        public static void FormMessage(this RoleInfo self,RoleInfoProto roleInfoProto)
        {
            self.Id = roleInfoProto.Id;
            self.Name = roleInfoProto.Name;
            self.State = roleInfoProto.State;
            self.AccountId = roleInfoProto.AccountId;
            self.CreateTime = roleInfoProto.CreateTime;
            self.ServerId = roleInfoProto.ServerId;
            self.LastLoginTime = roleInfoProto.LastLoginTime;
        }

        /// <summary>
        /// ����ɫ��Ϣϵͳ������д����Ϣ��
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static RoleInfoProto ToMessage(this RoleInfo self)
        {
            return new RoleInfoProto()
            {
                Id = self.Id,
                Name = self.Name,
                State = self.State,
                AccountId = self.AccountId,
                CreateTime = self.CreateTime,
                ServerId = self.ServerId,
                LastLoginTime = self.LastLoginTime,
            };
        }
    }
}