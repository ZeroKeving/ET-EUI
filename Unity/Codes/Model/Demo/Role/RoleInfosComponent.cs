
using System.Collections.Generic;

namespace ET
{
    /// <summary>
    /// 角色信息组件
    /// </summary>
    public class RoleInfosComponent : Entity,IAwake,IDestroy
    {
        public List<RoleInfo> RoleInfos = new List<RoleInfo>();//角色列表
        public long CurrentRoleId = 0;//当前角色
    }
}