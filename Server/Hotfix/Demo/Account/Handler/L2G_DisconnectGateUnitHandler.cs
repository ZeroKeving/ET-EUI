
using System;

namespace ET
{
    /// <summary>
    /// 登录中心服务器向Gate网关服务器发送顶号踢人消息的处理
    /// </summary>
    public class L2G_DisconnectGateUnitHandler : AMActorRpcHandler<Scene, L2G_DisconnectGateUnit, G2L_DisconnectGateUnit>
    {
        protected override async ETTask Run(Scene scene, L2G_DisconnectGateUnit request, G2L_DisconnectGateUnit response, Action reply)
        {
            long accountId = request.AccountId;

            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginGate, accountId.GetHashCode()))//协程锁
            {
                PlayerComponent playerComponent = scene.GetComponent<PlayerComponent>();//获得玩家组件
                Player player = playerComponent.Get(accountId);//获取gate网关上该账号连接的游戏角色

                if(player == null)//如果不存在已连接的游戏角色,直接回复
                {
                    reply();
                    return;
                }

                scene.GetComponent<GateSessionKeyComponent>().Remove(accountId);//移除该角色的Gate网关登录令牌
                Session gateSession = Game.EventSystem.Get(player.SessionInstanceId) as Session;//通过palyer的会话id拿到会话连接
                if(gateSession != null && !gateSession.IsDisposed)//如果会话连接不为空没有被释放
                {
                    gateSession.Send(new A2C_Disconnect() { Error = ErrorCode.ERR_OtherAccountLogin });
                    gateSession?.Disconnect().Coroutine();//延迟1秒后断开
                }

                player.SessionInstanceId = 0;//清空该玩家的会话连接id
                player.AddComponent<PlayerOfflineOutTimeComponent>();//给该玩家添加上定时踢出组件
            }

            reply();

        }
    }
}