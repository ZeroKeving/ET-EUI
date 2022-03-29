
namespace ET
{
    /// <summary>
    /// 角色信息组件销毁
    /// </summary>
    public class RoleInfosComponentDestroySystem : DestroySystem<RoleInfosComponent>
    {
        public override void Destroy(RoleInfosComponent self)
        {
            foreach (RoleInfo roleInfo in self.RoleInfos)
            {
                roleInfo?.Dispose();
            }
            self.RoleInfos.Clear();
            self.CurrentRoleId = 0;
        }
    }

    /// <summary>
    /// 角色信息组件系统
    /// </summary>
    public static class RoleInfosComponentSystem
    {
    }
}