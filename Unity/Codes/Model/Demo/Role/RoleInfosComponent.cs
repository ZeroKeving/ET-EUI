
using System.Collections.Generic;

namespace ET
{
    /// <summary>
    /// ��ɫ��Ϣ���
    /// </summary>
    public class RoleInfosComponent : Entity,IAwake,IDestroy
    {
        public List<RoleInfo> RoleInfos = new List<RoleInfo>();//��ɫ�б�
        public long CurrentRoleId = 0;//��ǰ��ɫ
    }
}