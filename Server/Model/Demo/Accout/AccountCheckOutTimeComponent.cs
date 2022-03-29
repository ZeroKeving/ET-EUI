
namespace ET
{
    /// <summary>
    /// 登录一段时间无操作的超时用户踢出组件
    /// </summary>
    public class AccountCheckOutTimeComponent :Entity,IAwake<long>,IDestroy
    {
        public long Timer = 0;
        public long AccountId = 0;
    }
}