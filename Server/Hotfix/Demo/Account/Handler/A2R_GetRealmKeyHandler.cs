
using System;

namespace ET
{
    /// <summary>
    /// Realm网关负载均衡服务器：获取Realm服务器的令牌和地址消息处理
    /// </summary>
    public class A2R_GetRealmKeyHandler : AMActorRpcHandler<Scene, A2R_GetRealmKey, R2A_GetRealmKey>
    {
        protected override async ETTask Run(Scene scene, A2R_GetRealmKey request, R2A_GetRealmKey response, Action reply)
        {
            if (scene.SceneType != SceneType.Realm)//这条请求消息是否请求到的是Realm服务器,如果不是则报错
            {
                Log.Error($"请求的Scene错误，当前Scene为：{scene.SceneType}");
                response.Error = ErrorCode.ERR_RequestSceneTypeError;
                reply();
                return;
            }

            string key = TimeHelper.ServerNow().ToString() + RandomHelper.RandInt64().ToString();//生成随机的令牌
            scene.GetComponent<TokenComponent>().Remove(request.AccountId);//移除登录令牌
            scene.GetComponent<TokenComponent>().Add(request.AccountId, key);//添加Realm网关令牌
            response.RealmKey = key.ToString();
            reply();

            await ETTask.CompletedTask;
        }
    }
}