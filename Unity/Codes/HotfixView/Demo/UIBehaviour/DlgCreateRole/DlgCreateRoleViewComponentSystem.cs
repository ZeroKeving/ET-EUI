
using UnityEngine;
using UnityEngine.UI;
namespace ET
{
	[ObjectSystem]
	public class DlgCreateRoleViewComponentAwakeSystem : AwakeSystem<DlgCreateRoleViewComponent> 
	{
		public override void Awake(DlgCreateRoleViewComponent self)
		{
			self.uiTransform = self.GetParent<UIBaseWindow>().uiTransform;
		}
	}


	[ObjectSystem]
	public class DlgCreateRoleViewComponentDestroySystem : DestroySystem<DlgCreateRoleViewComponent> 
	{
		public override void Destroy(DlgCreateRoleViewComponent self)
		{
			self.DestroyWidget();
		}
	}
}
