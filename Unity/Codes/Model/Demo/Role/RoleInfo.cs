

namespace ET
{
    /// <summary>
    /// ��ɫ�˺�״̬
    /// </summary>
    public enum RoleInfoState
    {
        Normal = 0,//����״̬
        Freeze = 1,//����״̬
        Banned = 2,//���״̬
    }

    /// <summary>
    /// ��Ϸ��ɫ��Ϣ
    /// </summary>
    public class RoleInfo : Entity,IAwake
    {
        public long UserId;//��ɫΨһid
        public string Name;//��ɫ����
        public int ServerId;//��������Id
        public int State;//��ɫ��ǰ״̬
        public long AccountId;//�˺�id
        public long LastLoginTime;//��ɫ�ϴε�¼��ʱ��
        public long CreateTime;//��ɫ����ʱ��
    }
}