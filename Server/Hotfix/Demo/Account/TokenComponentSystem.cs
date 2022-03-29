
namespace ET
{
    /// <summary>
    /// 登录令牌组件系统
    /// </summary>
    public static class TokenComponentSystem
    {
        /// <summary>
        /// 添加令牌到令牌字典当中
        /// </summary>
        /// <param name="self"></param>
        /// <param name="key"></param>
        /// <param name="token"></param>
        public static void Add(this TokenComponent self,long key,string token)
        {
            self.TokenDictionary.Add(key,token);
            self.TimeOutRemoveKey(key, token).Coroutine();//启动一个协程在令牌时间过期后去移除指定令牌
        }

        /// <summary>
        /// 从令牌字典中获取指定令牌
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
        /// 从令牌字典中移除指定令牌
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
        /// 令牌过期后移除
        /// </summary>
        /// <param name="self"></param>
        /// <param name="key"></param>
        /// <param name="tokenKey"></param>
        /// <returns></returns>
        private static async ETTask TimeOutRemoveKey(this TokenComponent self,long key,string tokenKey)
        {
            await TimerComponent.Instance.WaitAsync(600000);//等待10分钟后

            string onlineToken = self.Get(key);

            if(!string.IsNullOrEmpty(onlineToken) && onlineToken == tokenKey)//如果令牌不为空且与传入进来的令牌保持一致
            {
                self.Remove(key);//从令牌字典中移除指定令牌
            }
        }
    }
}