
using System;

namespace ET
{
    /// <summary>
    /// Gate网关服务器通知游戏逻辑服务器角色下线请求处理
    /// </summary>
    public class G2M_RequestExitGameHandler : AMActorRpcHandler<Unit, G2M_RequestExitGame, M2G_RequestExitGame>
    {
        protected override async ETTask Run(Unit unit, G2M_RequestExitGame request, M2G_RequestExitGame response, Action reply)
        {
            //TODD 保存玩家数据到数据库，执行相关下线操作

            reply();

            //正式释放Unit
            await unit.RemoveLocation();//通知定位服务器将单位的位置进行移除
            UnitComponent unitComponent = unit.DomainScene().GetComponent<UnitComponent>();
            unitComponent.Remove(unit.Id);

        }
    }
}