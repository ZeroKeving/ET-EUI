
using System;

namespace ET
{
    /// <summary>
    /// �ͻ������¼��������ѯ��Ϸ������Ϣ�Ĵ���
    /// </summary>
    public class C2A_GetServerInfosHandler : AMRpcHandler<C2A_GetSeverInfos, A2C_GetSeverInfos>
    {
        protected override async ETTask Run(Session session, C2A_GetSeverInfos request, A2C_GetSeverInfos response, Action reply)
        {
            if (session.DomainScene().SceneType != SceneType.Account)//����������Ϣ�Ƿ����󵽵��ǵ�¼������,���������Ͽ�����
            {
                Log.Error($"�����Scene���󣬵�ǰSceneΪ��{session.DomainScene().SceneType}");
                session.Dispose();//�Ͽ�����
                return;
            }

            string token = session.DomainScene().GetComponent<TokenComponent>().Get(request.AccountId);

            if(token == null || token != request.Token)//�����¼���ƴ���
            {
                response.Error = ErrorCode.ERR_TokenError;//��¼���ƴ�����
                reply();//���ͷ�����Ϣ
                session?.Disconnect().Coroutine();//�ӳ�һ��Ͽ�����
                return;
            }

            foreach (var serverInfo in session.DomainScene().GetComponent<ServerInfoManagerComponent>().ServerInfos)//����Ϸ������Ϣ�����ظ���Ϣ��
            {
                response.serverInfoList.Add(serverInfo.ToMessage());
            }

            reply();//���ͷ�����Ϣ

            await ETTask.CompletedTask;
        }
    }
}