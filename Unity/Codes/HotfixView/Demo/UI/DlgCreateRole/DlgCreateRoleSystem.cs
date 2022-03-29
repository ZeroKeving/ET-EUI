using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
    /// <summary>
    /// 创建角色界面
    /// </summary>
    public static class DlgCreateRoleSystem
    {

        public static void RegisterUIEvent(this DlgCreateRole self)
        {
            self.View.E_CreateRoleButton.AddListenerAsync(() => { return self.OnCreateRoleClickHandler(); });
        }

        public static void ShowWindow(this DlgCreateRole self, Entity contextData = null)
        {

        }

        /// <summary>
        /// 创建角色按键
        /// </summary>
        /// <param name="self"></param>
        public static async ETTask OnCreateRoleClickHandler(this DlgCreateRole self)
        {
            string name = self.View.EIputTextText.text;

            if(string.IsNullOrEmpty(name))
            {
                UIPopUpHelper.CreateHintWindow(self, HintCode.RoleNameIsNull);
                return;
            }

            try
            {
                int errorCode = await LoginHelper.CreateRole(self.ZoneScene(), name);//创建游戏角色
                if (errorCode != ErrorCode.ERR_Success)//如果返回错误码
                {
                    UIPopUpHelper.CreateErrorWindow(self, errorCode);//错误信息弹窗
                    return;
                }


                //进入游戏流程（先获取Realm网关服务器令牌和地址）
                errorCode = await LoginHelper.GetRealmKey(self.ZoneScene());//获取Realm网关负载均衡服务器的令牌
                if (errorCode != ErrorCode.ERR_Success)
                {
                    UIPopUpHelper.CreateErrorWindow(self, errorCode);//错误信息弹窗
                    return;
                }

                errorCode = await LoginHelper.EnterGame(self.ZoneScene());//进入游戏
                if (errorCode != ErrorCode.ERR_Success)
                {
                    UIPopUpHelper.CreateErrorWindow(self, errorCode);//错误信息弹窗
                    return;
                }


                self.DomainScene().GetComponent<UIComponent>().HideWindow(WindowID.WindowID_CreateRole);//隐藏角色创建界面
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
        }

    }
}
