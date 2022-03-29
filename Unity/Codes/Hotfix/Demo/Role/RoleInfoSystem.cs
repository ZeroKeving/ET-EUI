
namespace ET
{
    /// <summary>
    /// 角色信息系统
    /// </summary>
    public static class RoleInfoSystem
    {
        /// <summary>
        /// 将消息内容存入角色信息系统
        /// </summary>
        /// <param name="self"></param>
        /// <param name="roleInfoProto"></param>
        public static void FormMessage(this RoleInfo self,RoleInfoProto roleInfoProto)
        {
            self.Id = roleInfoProto.Id;
            self.State = roleInfoProto.State;
            self.AccountId = roleInfoProto.AccountId;
            self.CreateTime = roleInfoProto.CreateTime;
            self.ServerId = roleInfoProto.ServerId;
            self.LastLoginTime = roleInfoProto.LastLoginTime;
            self.Name = roleInfoProto.Name;
        }

        /// <summary>
        /// 将角色信息系统的内容写入消息体
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static RoleInfoProto ToMessage(this RoleInfo self)
        {
            return new RoleInfoProto()
            {
                Id = self.Id,
                State = self.State,
                AccountId = self.AccountId,
                CreateTime = self.CreateTime,
                ServerId = self.ServerId,
                LastLoginTime = self.LastLoginTime,
                Name = self.Name,
            };
        }
    }
}