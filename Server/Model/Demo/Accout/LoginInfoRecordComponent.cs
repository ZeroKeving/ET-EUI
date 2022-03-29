
using System.Collections.Generic;

namespace ET
{
    /// <summary>
    /// 登录信息记录组件
    /// </summary>
    public class LoginInfoRecordComponent : Entity,IAwake,IDestroy
    {
        public Dictionary<long,int> AccountLoginInfoDict = new Dictionary<long,int>();//账号登录信息字典
    }
}