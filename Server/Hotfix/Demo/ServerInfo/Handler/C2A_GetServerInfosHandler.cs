
using System;

namespace ET
{
    /// <summary>
    /// 客户端向登录服务器查询游戏区服信息的处理
    /// </summary>
    public class C2A_GetServerInfosHandler : AMRpcHandler<C2A_GetSeverInfos, A2C_GetSeverInfos>
    {
        protected override async ETTask Run(Session session, C2A_GetSeverInfos request, A2C_GetSeverInfos response, Action reply)
        {
            if (session.DomainScene().SceneType != SceneType.Account)//这条请求消息是否请求到的是登录服务器,如果不是则断开链接
            {
                Log.Error($"请求的Scene错误，当前Scene为：{session.DomainScene().SceneType}");
                session.Dispose();//断开连接
                return;
            }

            string token = session.DomainScene().GetComponent<TokenComponent>().Get(request.AccountId);

            if(token == null || token != request.Token)//如果登录令牌错误
            {
                response.Error = ErrorCode.ERR_TokenError;//登录令牌错误码
                reply();//发送返回消息
                session?.Disconnect().Coroutine();//延迟一秒断开连接
                return;
            }

            foreach (var serverInfo in session.DomainScene().GetComponent<ServerInfoManagerComponent>().ServerInfos)//将游戏区服信息添加入回复消息内
            {
                response.serverInfoList.Add(serverInfo.ToMessage());
            }

            reply();//发送返回消息

            await ETTask.CompletedTask;
        }
    }
}