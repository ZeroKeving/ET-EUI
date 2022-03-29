
namespace ET
{
    /// <summary>
    /// UI弹窗助手类
    /// </summary>
    public static class UIPopUpHelper
    {
        /// <summary>
        /// 创建一个错误弹窗
        /// </summary>
        /// <param name="self"></param>
        /// <param name="errorCode"></param>
        public static void CreateErrorWindow(Entity self, int errorCode)
        {
            self.DomainScene().GetComponent<UIComponent>().ShowWindow(WindowID.WindowID_ErrorWindow, WindowID.WindowID_Invaild, new ShowWindowData() { contextData = new ErrorData() { errorCode = errorCode } });//显示错误信息弹窗
            Log.Error(errorCode.ToString());//打印错误码
        }

        /// <summary>
        /// 创建一个信息提示弹窗
        /// </summary>
        /// <param name="self"></param>
        /// <param name="errorCode"></param>
        public static void CreateHintWindow(Entity self, int hintCode)
        {
            self.DomainScene().GetComponent<UIComponent>().ShowWindow(WindowID.WindowID_HintWindow, WindowID.WindowID_Invaild, new ShowWindowData() { contextData = new HintData() { hintCode = hintCode } });//显示提示信息弹窗
        }

    }
}