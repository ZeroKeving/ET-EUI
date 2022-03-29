using System.Collections.Generic;

namespace ET
{
	public  class DlgServerInfo :Entity,IAwake,IUILogic
	{

		public DlgServerInfoViewComponent View { get => this.Parent.GetComponent<DlgServerInfoViewComponent>();}

		public Dictionary<int, Scroll_Item_Server> Scroll_Item_ServerDict;//服务器列表

	}
}
