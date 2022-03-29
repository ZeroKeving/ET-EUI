
using System;

namespace ET
{
    /// <summary>
    /// Gate���ط���������ȡ��¼Gate������������������
    /// </summary>
    public class R2G_GetLoginGateKeyHandler : AMActorRpcHandler<Scene, R2G_GetLoginGateKey, G2R_GetLoginGateKey>
    {
        protected override async ETTask Run(Scene scene, R2G_GetLoginGateKey request, G2R_GetLoginGateKey response, Action reply)
        {
            if (scene.SceneType != SceneType.Gate)//����������Ϣ�Ƿ����󵽵���Gate���ط�����,��������򱨴�
            {
                Log.Error($"�����Scene���󣬵�ǰSceneΪ��{scene.SceneType}");
                response.Error = ErrorCode.ERR_RequestSceneTypeError;
                reply();
                return;
            }

            string key = RandomHelper.RandInt64().ToString() + TimeHelper.ServerNow().ToString();//�������������
            scene.GetComponent<TokenComponent>().Remove(request.AccountId);//�Ƴ���¼����
            scene.GetComponent<GateSessionKeyComponent>().Add(request.AccountId, key);//���Gate��������
            response.GateSessionKey = key;
            reply();

            await ETTask.CompletedTask;
        }
    }
}