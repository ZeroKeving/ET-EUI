
using UnityEngine;
using UnityEngine.UI;
namespace ET
{
	[ObjectSystem]
	public class DlgHintWindowViewComponentAwakeSystem : AwakeSystem<DlgHintWindowViewComponent> 
	{
		public override void Awake(DlgHintWindowViewComponent self)
		{
			self.uiTransform = self.GetParent<UIBaseWindow>().uiTransform;
		}
	}


	[ObjectSystem]
	public class DlgHintWindowViewComponentDestroySystem : DestroySystem<DlgHintWindowViewComponent> 
	{
		public override void Destroy(DlgHintWindowViewComponent self)
		{
			self.DestroyWidget();
		}
	}
}
