
namespace ET
{
    /// <summary>
    /// ��¼�������ϵͳ
    /// </summary>
    public static class TokenComponentSystem
    {
        /// <summary>
        /// ������Ƶ������ֵ䵱��
        /// </summary>
        /// <param name="self"></param>
        /// <param name="key"></param>
        /// <param name="token"></param>
        public static void Add(this TokenComponent self,long key,string token)
        {
            self.TokenDictionary.Add(key,token);
            self.TimeOutRemoveKey(key, token).Coroutine();//����һ��Э��������ʱ����ں�ȥ�Ƴ�ָ������
        }

        /// <summary>
        /// �������ֵ��л�ȡָ������
        /// </summary>
        /// <param name="self"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Get(this TokenComponent self, long key)
        {
            string value = null;

            self.TokenDictionary.TryGetValue(key, out value);

            return value;
        }

        /// <summary>
        /// �������ֵ����Ƴ�ָ������
        /// </summary>
        /// <param name="self"></param>
        /// <param name="key"></param>
        public static void Remove(this TokenComponent self, long key)
        {
            if(self.TokenDictionary.ContainsKey(key))
            {
                self.TokenDictionary.Remove(key);
            }
        }

        /// <summary>
        /// ���ƹ��ں��Ƴ�
        /// </summary>
        /// <param name="self"></param>
        /// <param name="key"></param>
        /// <param name="tokenKey"></param>
        /// <returns></returns>
        private static async ETTask TimeOutRemoveKey(this TokenComponent self,long key,string tokenKey)
        {
            await TimerComponent.Instance.WaitAsync(600000);//�ȴ�10���Ӻ�

            string onlineToken = self.Get(key);

            if(!string.IsNullOrEmpty(onlineToken) && onlineToken == tokenKey)//������Ʋ�Ϊ�����봫����������Ʊ���һ��
            {
                self.Remove(key);//�������ֵ����Ƴ�ָ������
            }
        }
    }
}