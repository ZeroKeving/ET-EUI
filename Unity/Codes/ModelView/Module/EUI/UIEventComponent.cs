using System.Collections.Generic;

namespace ET
{
    public class UIEventComponent : Entity,IAwake,IDestroy
    {
        public static UIEventComponent Instance { get; set; }
        public readonly Dictionary<WindowID, IAUIEventHandler> UIEventHandlers = new Dictionary<WindowID, IAUIEventHandler>();
        public bool AsyncButtonIsClicked { get; set; }//异步按钮是否被点击
    }
}