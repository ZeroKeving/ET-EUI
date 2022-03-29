namespace ET
{
	public  class DlgHintWindow :Entity,IAwake,IUILogic
	{

		public DlgHintWindowViewComponent View { get => this.Parent.GetComponent<DlgHintWindowViewComponent>();} 

		 

	}
}
