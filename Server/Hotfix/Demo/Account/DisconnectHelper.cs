
namespace ET
{
    /// <summary>
    /// 断开连接助手类
    /// </summary>
    public static class DisconnectHelper
    {
        /// <summary>
        /// 针对会话进行一个扩展,延迟1秒后断开连接
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static async ETTask Disconnect(this Session self)
        {
            if (self == null || self.IsDisposed)//判断会话是否为空或是否被断开
            {
                return;
            }

            long instanceId = self.InstanceId;//记录一下会话的id

            await TimerComponent.Instance.WaitAsync(1000);//等待1秒

            if(self.InstanceId != instanceId)//判断当前会话的id是否等于之前会话id,不等于直接返回
            {
                return ;
            }

            self.Dispose();
        }

        /// <summary>
        /// 踢出玩家
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static async ETTask KickPlayer(Player player,bool isException = false)
        {
            if(player == null || player.IsDisposed)//如果玩家不存在或已经被释放
            {
                return;
            }
            long instanceId = player.InstanceId;
            using(await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginGate,player.AccountId.GetHashCode()))//协程锁
            {
                if (player == null || instanceId != player.InstanceId)//如果玩家不存在或已经被释放(异步函数可能被重复调用多次,所以要判断是否重复执行)
                {
                    return;
                }

                if(!isException)//如果发生异常
                {
                    switch (player.PlayerState)
                    {
                        case PlayerState.Disconnect:
                            break;
                        case PlayerState.Gate:
                            break;
                        case PlayerState.Game:
                            //通知游戏逻辑服下线Unit角色逻辑，并将数据存入数据库
                            M2G_RequestExitGame m2G_RequestExitGame = (M2G_RequestExitGame)await MessageHelper.CallLocationActor(player.UnitId, new G2M_RequestExitGame() { });

                            //通知移除账号角色登录信息
                            long LoginCenterConfigSceneId = StartSceneConfigCategory.Instance.LoginCenterConfig.InstanceId;
                            L2G_RemoveLoginRecord l2G_RemoveLoginRecord = (L2G_RemoveLoginRecord)await MessageHelper.CallActor(LoginCenterConfigSceneId, new G2L_RemoveLoginRecord()
                            {
                                AccountId = player.AccountId,
                                ServerId = player.DomainZone(),
                            });

                            break;
                    }
                }

                player.PlayerState = PlayerState.Disconnect;
                player.DomainScene().GetComponent<PlayerComponent>()?.Remove(player.AccountId);
                player?.Dispose();
                await TimerComponent.Instance.WaitAsync(300);
            }
        }
    }
}