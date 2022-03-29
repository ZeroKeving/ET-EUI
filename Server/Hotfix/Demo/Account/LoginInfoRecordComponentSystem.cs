
namespace ET
{
    /// <summary>
    /// 登录信息记录组件销毁系统
    /// </summary>
    public class LoginInfoRecordComponentDestorySystem : DestroySystem<LoginInfoRecordComponent>
    {
        public override void Destroy(LoginInfoRecordComponent self)
        {
            self.AccountLoginInfoDict.Clear();
        }
    }

    /// <summary>
    /// 登录信息记录组件系统
    /// </summary>
    public static class LoginInfoRecordComponentSystem
    {
        /// <summary>
        /// 添加登录信息记录
        /// </summary>
        /// <param name="self"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Add(this LoginInfoRecordComponent self,long key,int value)
        {
            if(self.AccountLoginInfoDict.ContainsKey(key))//检查有没有相同的key，如果有则覆盖写入
            {
                self.AccountLoginInfoDict[key] = value;
                return;
            }
            self.AccountLoginInfoDict.Add(key, value);
        }

        /// <summary>
        /// 移除登录信息记录
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
        /// 获取指定的登录信息记录
        /// </summary>
        /// <param name="self"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int Get(this LoginInfoRecordComponent self, long key)
        {
            if(!self.AccountLoginInfoDict.TryGetValue(key,out int value))//如果没有这个值的情况下返回-1，有则返回值
            {
                return -1;
            }
            return value;
        }

        /// <summary>
        /// 是否存在指定的登录信息记录
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