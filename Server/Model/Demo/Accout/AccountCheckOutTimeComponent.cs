
namespace ET
{
    /// <summary>
    /// ��¼һ��ʱ���޲����ĳ�ʱ�û��߳����
    /// </summary>
    public class AccountCheckOutTimeComponent :Entity,IAwake<long>,IDestroy
    {
        public long Timer = 0;
        public long AccountId = 0;
    }
}