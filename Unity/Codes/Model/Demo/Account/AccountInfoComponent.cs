
namespace ET
{
    /// <summary>
    /// ��¼��Ϣ���
    /// </summary>
    public class AccountInfoComponent:Entity,IAwake,IDestroy
    {
        public string Token;//��¼��־����
        public long AccountId;//��¼Id
        public string RealmKey;//Realm���ظ��ؾ��������������
        public string RealmAddress;//Realm���ظ��ؾ���������ĵ�ַ
    }
}