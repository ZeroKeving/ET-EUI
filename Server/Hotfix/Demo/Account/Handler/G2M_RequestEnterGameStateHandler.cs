
using System;

namespace ET
{
    /// <summary>
    /// 网关服务器向游戏逻辑服务器请求进入游戏状态消息处理
    /// </summary>
    public class G2M_RequestEnterGameStateHandler : AMActorLocationRpcHandler<Unit, G2M_RequestEnterGameState, M2G_RequestEnterGameState>
    {
        /// <summary>
        /// 如果找不到unit对象会直接报错误码，如果找到了那直接回复就行
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