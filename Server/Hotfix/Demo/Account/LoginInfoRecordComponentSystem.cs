
namespace ET
{
    /// <summary>
    /// ��¼��Ϣ��¼�������ϵͳ
    /// </summary>
    public class LoginInfoRecordComponentDestorySystem : DestroySystem<LoginInfoRecordComponent>
    {
        public override void Destroy(LoginInfoRecordComponent self)
        {
            self.AccountLoginInfoDict.Clear();
        }
    }

    /// <summary>
    /// ��¼��Ϣ��¼���ϵͳ
    /// </summary>
    public static class LoginInfoRecordComponentSystem
    {
        /// <summary>
        /// ��ӵ�¼��Ϣ��¼
        /// </summary>
        /// <param name="self"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Add(this LoginInfoRecordComponent self,long key,int value)
        {
            if(self.AccountLoginInfoDict.ContainsKey(key))//�����û����ͬ��key��������򸲸�д��
            {
                self.AccountLoginInfoDict[key] = value;
                return;
            }
            self.AccountLoginInfoDict.Add(key, value);
        }

        /// <summary>
        /// �Ƴ���¼��Ϣ��¼
        /// </summary>
        /// <param name="self"></param>
        /// <param name="key"></param>
        public static void Remove(this LoginInfoRecordComponent self, long key)
        {
            if(self.AccountLoginInfoDict.ContainsKey(key))
            {
                self.AccountLoginInfoDict.Remove(key);
            }
        }

        /// <summary>
        /// ��ȡָ���ĵ�¼��Ϣ��¼
        /// </summary>
        /// <param name="self"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int Get(this LoginInfoRecordComponent self, long key)
        {
            if(!self.AccountLoginInfoDict.TryGetValue(key,out int value))//���û�����ֵ������·���-1�����򷵻�ֵ
            {
                return -1;
            }
            return value;
        }

        /// <summary>
        /// �Ƿ����ָ���ĵ�¼��Ϣ��¼
        /// </summary>
        /// <param name="self"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsExist(this LoginInfoRecordComponent self, long key)
        {
            return self.AccountLoginInfoDict.ContainsKey(key);
        }
    }
}