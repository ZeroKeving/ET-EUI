
using UnityEngine;
using UnityEngine.UI;
namespace ET
{
	[ObjectSystem]
	public class DlgErrorWindowViewComponentAwakeSystem : AwakeSystem<DlgErrorWindowViewComponent> 
	{
		public override void Awake(DlgErrorWindowViewComponent self)
		{
			self.uiTransform = self.GetParent<UIBaseWindow>().uiTransform;
		}
	}


	[ObjectSystem]
	public class DlgErrorWindowViewComponentDestroySystem : DestroySystem<DlgErrorWindowViewComponent> 
	{
		public override void Destroy(DlgErrorWindowViewComponent self)
		{
			self.DestroyWidget();
		}
	}
}
