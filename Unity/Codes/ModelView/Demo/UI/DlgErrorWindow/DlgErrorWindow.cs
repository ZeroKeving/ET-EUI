namespace ET
{
	public  class DlgErrorWindow :Entity,IAwake,IUILogic
	{

		public DlgErrorWindowViewComponent View { get => this.Parent.GetComponent<DlgErrorWindowViewComponent>();} 

		 

	}
}
