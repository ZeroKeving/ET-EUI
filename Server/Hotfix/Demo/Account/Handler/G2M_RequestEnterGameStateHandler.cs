
using System;

namespace ET
{
    /// <summary>
    /// ���ط���������Ϸ�߼����������������Ϸ״̬��Ϣ����
    /// </summary>
    public class G2M_RequestEnterGameStateHandler : AMActorLocationRpcHandler<Unit, G2M_RequestEnterGameState, M2G_RequestEnterGameState>
    {
        /// <summary>
        /// ����Ҳ���unit�����ֱ�ӱ������룬����ҵ�����ֱ�ӻظ�����
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="reply"></param>
        /// <returns></returns>
        protected override async ETTask Run(Unit unit, G2M_RequestEnterGameState request, M2G_RequestEnterGameState response, Action reply)
        {
            reply();

            await ETTask.CompletedTask;
        }
    }
}