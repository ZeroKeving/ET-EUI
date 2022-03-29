using System;

namespace ET
{
    [ObjectSystem]
    public class UIEventComponentAwakeSystem : AwakeSystem<UIEventComponent>
    {
        public override void Awake(UIEventComponent self)
        {
            UIEventComponent.Instance = self;
            self.Awake();
        }
    }
    
    [ObjectSystem]
    public class UIEventComponentDestroySystem : DestroySystem<UIEventComponent>
    {
        public override void Destroy(UIEventComponent self)
        {
            self.UIEventHandlers.Clear();
            self.AsyncButtonIsClicked = false;//初始化异步按钮点击锁
            UIEventComponent.Instance = null;
        }
    }
    
    
    public static class UIEventComponentSystem
    {
        public static void Awake(this UIEventComponent self)
        {
            self.UIEventHandlers.Clear();
            foreach (Type v in Game.EventSystem.GetTypes(typeof (AUIEventAttribute)))
            {
                AUIEventAttribute attr = v.GetCustomAttributes(typeof (AUIEventAttribute), false)[0] as AUIEventAttribute;
                self.UIEventHandlers.Add(attr.WindowID, Activator.CreateInstance(v) as IAUIEventHandler);
            }
        }
        
        public static IAUIEventHandler GetUIEventHandler(this UIEventComponent self,WindowID windowID)
        {
            if (self.UIEventHandlers.TryGetValue(windowID, out IAUIEventHandler handler))
            {
                return handler;
            }
            Log.Error($"windowId : {windowID} is not have any uiEvent");
            return null;
        }

        /// <summary>
        /// 设置异步按钮点击事件锁
        /// </summary>
        /// <param name="self"></param>
        /// <param name="asyncButtonIsClicked"></param>
        public static void SetUIAsyncButtonClicked(this UIEventComponent self, bool asyncButtonIsClicked)
        {
            self.AsyncButtonIsClicked = asyncButtonIsClicked;
        }
    }

}