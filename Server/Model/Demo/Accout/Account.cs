
namespace ET
{
    //��Ϸ�˺�����
    public enum AccountType
    {
        General = 0,//��ͨ���

        BlackList = 100,//���������
    }

    /// <summary>
    /// �˺�����
    /// </summary>
    public class Account : Entity,IAwake
    {
        public string AccountName;//�˺�

        public string Password;//����

        public long CreateTime;//�˺Ŵ���ʱ��

        public int AccountType;//�˺�����
    }
}