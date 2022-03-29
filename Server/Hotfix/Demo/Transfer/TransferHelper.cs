namespace ET
{
    public static class TransferHelper
    {
        /// <summary>
        /// 将指定的单位传送到指定的服务器场景里
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="sceneInstanceId"></param>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        public static async ETTask Transfer(Unit unit, long sceneInstanceId, string sceneName)
        {
            // 通知客户端开始切场景
            M2C_StartSceneChange m2CStartSceneChange = new M2C_StartSceneChange() {SceneInstanceId = sceneInstanceId, SceneName = sceneName};
            MessageHelper.SendToClient(unit, m2CStartSceneChange);
            
            M2M_UnitTransferRequest request = new M2M_UnitTransferRequest();//生成一条逻辑服务器内部通信的请求
            request.Unit = unit;
            foreach (Entity entity in unit.Components.Values)//遍历这个单位的所有组件，如果这些组件可以被传送就添加入
            {
                if (entity is ITransfer)
                {
                    request.Entitys.Add(entity);
                }
            }
            // 删除Mailbox,让发给Unit的ActorLocation消息重发
            unit.RemoveComponent<MailBoxComponent>();
            
            // location加锁
            long oldInstanceId = unit.InstanceId;
            await LocationProxyComponent.Instance.Lock(unit.Id, unit.InstanceId);//通知定位服务器这个单位要开始传送了
            M2M_UnitTransferResponse response = await ActorMessageSenderComponent.Instance.Call(sceneInstanceId, request) as M2M_UnitTransferResponse;//将单位传送到对应游戏逻辑服务器
            await LocationProxyComponent.Instance.UnLock(unit.Id, oldInstanceId, response.NewInstanceId);//通知单位传送结束，并告知新的地址
            unit.Dispose();
        }
    }
}