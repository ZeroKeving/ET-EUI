using System;
using UnityEngine;

namespace ET
{
	[ActorMessageHandler]
	public class M2M_UnitTransferRequestHandler : AMActorRpcHandler<Scene, M2M_UnitTransferRequest, M2M_UnitTransferResponse>
	{
		/// <summary>
		/// 服务器单位传送请求
		/// </summary>
		/// <param name="scene">游戏逻辑服务器</param>
		/// <param name="request"></param>
		/// <param name="response"></param>
		/// <param name="reply"></param>
		/// <returns></returns>
		protected override async ETTask Run(Scene scene, M2M_UnitTransferRequest request, M2M_UnitTransferResponse response, Action reply)
		{
			await ETTask.CompletedTask;
			UnitComponent unitComponent = scene.GetComponent<UnitComponent>();
			Unit unit = request.Unit;

			//将该单位添加进游戏逻辑服务器
			unitComponent.AddChild(unit);
			unitComponent.Add(unit);

			foreach (Entity entity in request.Entitys)//将所有传送过来的组件全部添加到新的单位身上
			{
				unit.AddComponent(entity);
			}
			
			unit.AddComponent<MoveComponent>();
			unit.AddComponent<PathfindingComponent, string>(scene.Name);
			unit.Position = new Vector3(-10, 0, -10);
			
			unit.AddComponent<MailBoxComponent>();
			
			// 通知客户端创建My Unit
			M2C_CreateMyUnit m2CCreateUnits = new M2C_CreateMyUnit();
			m2CCreateUnits.Unit = UnitHelper.CreateUnitInfo(unit);
			MessageHelper.SendToClient(unit, m2CCreateUnits);
			
			// 加入aoi
			unit.AddComponent<AOIEntity, int, Vector3>(9 * 1000, unit.Position);

			response.NewInstanceId = unit.InstanceId;
			
			reply();
		}
	}
}