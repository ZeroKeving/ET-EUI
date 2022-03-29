
using System.Collections.Generic;

namespace ET
{
    /// <summary>
    /// 登录会话连接组件
    /// </summary>
    public class AccountSessionsComponent : Entity,IAwake,IDestroy
    {
        public Dictionary<long,long> AccountSessionDict =  new Dictionary<long,long>();//登录会话连接字典
    }
}