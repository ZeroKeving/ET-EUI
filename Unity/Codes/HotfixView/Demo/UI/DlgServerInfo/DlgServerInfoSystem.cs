using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
    /// <summary>
    /// 游戏区服UI系统
    /// </summary>
    public static class DlgServerInfoSystem
    {
        /// <summary>
        /// 事件登记
        /// </summary>
        /// <param name="self"></param>
        public static void RegisterUIEvent(this DlgServerInfo self)
        {
            self.View.E_EnterMapButton.AddListenerAsync(() => { return self.OnEnterMapClickHandler(); });
            self.View.ELoopScrollList_ServerInfoLoopVerticalScrollRect.AddItemRefreshListener((Transform transform, int index) => { self.OnLoopScrollListItemRefreshHandler(transform, index); });
            self.View.EDeleteRoleButton.AddListenerAsync(() => { return self.OnDeleteRoleClickHandler(); });
        }

        /// <summary>
        /// 窗口显示
        /// </summary>
        /// <param name="self"></param>
        /// <param name="contextData"></param>
        public static void ShowWindow(this DlgServerInfo self, Entity contextData = null)
        {
            //显示游戏区服滑动列表
            int count = self.ZoneScene().GetComponent<ServerInfosComponent>().ServerInfosList.Count;//获取游戏区服信息
            Debug.Log(count);
            self.AddUIScrollItems(ref self.Scroll_Item_ServerDict, count);//添加列表项
            self.View.ELoopScrollList_ServerInfoLoopVerticalScrollRect.SetVisible(true, count);//将其显示出来
        }

        /// <summary>
        /// 隐藏窗口时调用
        /// </summary>
        /// <param name="self"></param>
        public static void HideWindow(this DlgServerInfo self)
        {
            //为了节省资源在窗口被隐藏时，把滑动列表项释放
            self.RemoveUIScrollItems(ref self.Scroll_Item_ServerDict);
        }

        /// <summary>
        /// 进入游戏按钮事件
        /// </summary>
        /// <param name="self"></param>
        public static async ETTask OnEnterMapClickHandler(this DlgServerInfo self)
        {
            try
            {
                int errorCode = await LoginHelper.GetRoles(self.ZoneScene());//获取该账号的游戏角色
                if (errorCode != ErrorCode.ERR_Success)
                {
                    if(errorCode <= HintCode.Null)//如果不是提示码
                    {
                        UIPopUpHelper.CreateErrorWindow(self, errorCode);//错误信息弹窗
                        return;
                    }
                    self.DomainScene().GetComponent<UIComponent>().HideWindow(WindowID.WindowID_ServerInfo);//隐藏游戏区服界面
                    self.DomainScene().GetComponent<UIComponent>().ShowWindow(WindowID.WindowID_CreateRole);//显示角色创建界面
                    return;
                }

                //进入游戏流程（先获取Realm网关服务器令牌和地址）
                errorCode = await LoginHelper.GetRealmKey(self.ZoneScene()); //获取Realm网关负载均衡服务器的令牌
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

                self.DomainScene().GetComponent<UIComponent>().HideWindow(WindowID.WindowID_ServerInfo);//隐藏游戏区服界面
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
        }

        /// <summary>
        /// 删除该服务器所有角色
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static async ETTask OnDeleteRoleClickHandler(this DlgServerInfo self)
        {
            try
            {
                int errorCode = await LoginHelper.DeletAllRole(self.ZoneScene());//删除游戏角色
                if (errorCode != ErrorCode.ERR_Success)//如果返回错误码
                {
                    UIPopUpHelper.CreateErrorWindow(self, errorCode);//错误信息弹窗
                    return;
                }

                UIPopUpHelper.CreateHintWindow(self, HintCode.RoleDeteteSucceed);//角色删除成功提示
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
        }

        /// <summary>
        /// 游戏区服滑动列表刷新处理事件
        /// </summary>
        /// <param name="self"></param>
        /// <param name="transform"></param>
        /// <param name="index"></param>
        public static void OnLoopScrollListItemRefreshHandler(this DlgServerInfo self, Transform transform, int index)
        {
            Scroll_Item_Server scroll_Item_Server = self.Scroll_Item_ServerDict[index].BindTrans(transform);//先获取UI列表项的实体，然后在绑定UI层
            ServerInfosComponent serverInfos = self.ZoneScene().GetComponent<ServerInfosComponent>();
            scroll_Item_Server.ETextText.SetText(serverInfos.ServerInfosList[index].Id.ToString() + "." + serverInfos.ServerInfosList[index].ServerName);//显示区服名称
            scroll_Item_Server.EButtonImage.color = serverInfos.CurrentServerId == serverInfos.ServerInfosList[index].Id ? Color.blue : Color.green;//选中的区服显示为蓝色
            scroll_Item_Server.EButtonButton.AddListener(() => { self.OnSelectServerItemHandler(serverInfos.ServerInfosList[index].Id); });
        }

        /// <summary>
        /// 选中游戏区服滑动列表项处理事件
        /// </summary>
        /// <param name="self"></param>
        /// <param name="severId"></param>
        public static void OnSelectServerItemHandler(this DlgServerInfo self,long serverId)
        {
            self.ZoneScene().GetComponent<ServerInfosComponent>().CurrentServerId = int.Parse(serverId.ToString());
            self.View.ELoopScrollList_ServerInfoLoopVerticalScrollRect.RefillCells();//刷新列表项
        }

    }
}
