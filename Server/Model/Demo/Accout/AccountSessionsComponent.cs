
using System.Collections.Generic;

namespace ET
{
    /// <summary>
    /// ��¼�Ự�������
    /// </summary>
    public class AccountSessionsComponent : Entity,IAwake,IDestroy
    {
        public Dictionary<long,long> AccountSessionDict =  new Dictionary<long,long>();//��¼�Ự�����ֵ�
    }
}