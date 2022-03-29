
using System;

namespace ET
{
    /// <summary>
    /// Gate网关服务器：获取登录Gate服务器的令牌请求处理
    /// </summary>
    public class R2G_GetLoginGateKeyHandler : AMActorRpcHandler<Scene, R2G_GetLoginGateKey, G2R_GetLoginGateKey>
    {
        protected override async ETTask Run(Scene scene, R2G_GetLoginGateKey request, G2R_GetLoginGateKey response, Action reply)
        {
            if (scene.SceneType != SceneType.Gate)//这条请求消息是否请求到的是Gate网关服务器,如果不是则报错
            {
                Log.Error($"请求的Scene错误，当前Scene为：{scene.SceneType}");
                response.Error = ErrorCode.ERR_RequestSceneTypeError;
                reply();
                return;
            }

            string key = RandomHelper.RandInt64().ToString() + TimeHelper.ServerNow().ToString();//生成随机的令牌
            scene.GetComponent<TokenComponent>().Remove(request.AccountId);//移除登录令牌
            scene.GetComponent<GateSessionKeyComponent>().Add(request.AccountId, key);//添加Gate网关令牌
            response.GateSessionKey = key;
            reply();

            await ETTask.CompletedTask;
        }
    }
}