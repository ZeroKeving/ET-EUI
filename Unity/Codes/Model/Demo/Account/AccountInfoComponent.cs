
namespace ET
{
    /// <summary>
    /// 登录信息组件
    /// </summary>
    public class AccountInfoComponent:Entity,IAwake,IDestroy
    {
        public string Token;//登录标志令牌
        public long AccountId;//登录Id
        public string RealmKey;//Realm网关负载均衡服务器的令牌
        public string RealmAddress;//Realm网关负载均衡服务器的地址
    }
}