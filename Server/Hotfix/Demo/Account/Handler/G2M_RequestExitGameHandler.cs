
using System;

namespace ET
{
    /// <summary>
    /// Gate���ط�����֪ͨ��Ϸ�߼���������ɫ����������
    /// </summary>
    public class G2M_RequestExitGameHandler : AMActorRpcHandler<Unit, G2M_RequestExitGame, M2G_RequestExitGame>
    {
        protected override async ETTask Run(Unit unit, G2M_RequestExitGame request, M2G_RequestExitGame response, Action reply)
        {
            //TODD ����������ݵ����ݿ⣬ִ��������߲���

            reply();

            //��ʽ�ͷ�Unit
            await unit.RemoveLocation();//֪ͨ��λ����������λ��λ�ý����Ƴ�
            UnitComponent unitComponent = unit.DomainScene().GetComponent<UnitComponent>();
            unitComponent.Remove(unit.Id);

        }
    }
}