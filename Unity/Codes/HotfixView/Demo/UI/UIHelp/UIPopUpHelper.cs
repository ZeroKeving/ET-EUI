
namespace ET
{
    /// <summary>
    /// UI����������
    /// </summary>
    public static class UIPopUpHelper
    {
        /// <summary>
        /// ����һ�����󵯴�
        /// </summary>
        /// <param name="self"></param>
        /// <param name="errorCode"></param>
        public static void CreateErrorWindow(Entity self, int errorCode)
        {
            self.DomainScene().GetComponent<UIComponent>().ShowWindow(WindowID.WindowID_ErrorWindow, WindowID.WindowID_Invaild, new ShowWindowData() { contextData = new ErrorData() { errorCode = errorCode } });//��ʾ������Ϣ����
            Log.Error(errorCode.ToString());//��ӡ������
        }

        /// <summary>
        /// ����һ����Ϣ��ʾ����
        /// </summary>
        /// <param name="self"></param>
        /// <param name="errorCode"></param>
        public static void CreateHintWindow(Entity self, int hintCode)
        {
            self.DomainScene().GetComponent<UIComponent>().ShowWindow(WindowID.WindowID_HintWindow, WindowID.WindowID_Invaild, new ShowWindowData() { contextData = new HintData() { hintCode = hintCode } });//��ʾ��ʾ��Ϣ����
        }

    }
}