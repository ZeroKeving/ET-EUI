
using System;

namespace ET
{
    /// <summary>
    /// Realm���ظ��ؾ������������ȡRealm�����������ƺ͵�ַ��Ϣ����
    /// </summary>
    public class A2R_GetRealmKeyHandler : AMActorRpcHandler<Scene, A2R_GetRealmKey, R2A_GetRealmKey>
    {
        protected override async ETTask Run(Scene scene, A2R_GetRealmKey request, R2A_GetRealmKey response, Action reply)
        {
            if (scene.SceneType != SceneType.Realm)//����������Ϣ�Ƿ����󵽵���Realm������,��������򱨴�
            {
                Log.Error($"�����Scene���󣬵�ǰSceneΪ��{scene.SceneType}");
                response.Error = ErrorCode.ERR_RequestSceneTypeError;
                reply();
                return;
            }

            string key = TimeHelper.ServerNow().ToString() + RandomHelper.RandInt64().ToString();//�������������
            scene.GetComponent<TokenComponent>().Remove(request.AccountId);//�Ƴ���¼����
            scene.GetComponent<TokenComponent>().Add(request.AccountId, key);//���Realm��������
            response.RealmKey = key.ToString();
            reply();

            await ETTask.CompletedTask;
        }
    }
}