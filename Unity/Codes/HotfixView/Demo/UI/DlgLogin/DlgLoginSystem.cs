using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ET
{
    public static class DlgLoginSystem
    {
        /// <summary>
        /// 登记UI事件
        /// </summary>
        /// <param name="self"></param>
        public static void RegisterUIEvent(this DlgLogin self)
        {
            self.View.E_LoginButton.AddListenerAsync(() => { return self.OnLoginClickHandler(); });//异步监听登录按键
        }

        public static void ShowWindow(this DlgLogin self, Entity contextData = null)
        {

        }

        /// <summary>
        /// 异步登录按钮处理
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static async ETTask OnLoginClickHandler(this DlgLogin self)
        {
            try
            {
                //登录游戏
                int errorCode = await LoginHelper.Login(self.DomainScene(),ConstValue.LoginAddress,
                self.View.E_AccountInputField.GetComponent<InputField>().text,
                self.View.E_PasswordInputField.GetComponent<InputField>().text);
                if (errorCode != ErrorCode.ERR_Success)//如果获取的不是一个成功的错误码
                {
                    UIPopUpHelper.CreateErrorWindow(self, errorCode);
                    return;
                }
                //获取游戏区服
                int serverInfoErrorCode = await LoginHelper.GetServerInfos(self.DomainScene());
                if (serverInfoErrorCode != ErrorCode.ERR_Success)//如果获取到错误
                {
                    UIPopUpHelper.CreateErrorWindow(self, serverInfoErrorCode);
                    return;
                }

                //显示登录之后的页面逻辑
                self.DomainScene().GetComponent<UIComponent>().ShowWindow(WindowID.WindowID_ServerInfo);//显示游戏区服界面
                self.DomainScene().GetComponent<UIComponent>().CloseWindow(WindowID.WindowID_Login);//隐藏登录窗口

            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }

        }

        public static void HideWindow(this DlgLogin self)
        {

        }

    }
}
