
using System.Collections.Generic;

namespace ET
{
    /// <summary>
    /// ��¼��Ϣ��¼���
    /// </summary>
    public class LoginInfoRecordComponent : Entity,IAwake,IDestroy
    {
        public Dictionary<long,int> AccountLoginInfoDict = new Dictionary<long,int>();//�˺ŵ�¼��Ϣ�ֵ�
    }
}