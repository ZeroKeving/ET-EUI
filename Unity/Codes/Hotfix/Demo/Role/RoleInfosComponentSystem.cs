
namespace ET
{
    /// <summary>
    /// ��ɫ��Ϣ�������
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
    /// ��ɫ��Ϣ���ϵͳ
    /// </summary>
    public static class RoleInfosComponentSystem
    {
    }
}