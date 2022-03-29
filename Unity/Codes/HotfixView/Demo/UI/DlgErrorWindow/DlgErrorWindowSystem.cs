using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
	/// <summary>
	/// 错误信息弹窗
	/// </summary>
	public static class DlgErrorWindowSystem
	{

		public static void RegisterUIEvent(this DlgErrorWindow self)
		{
			self.View.EButtonButton.AddListener(() => { self.OnCloseButtonClickHandler(); });//关闭界面
			self.View.E_PanelButton.AddListener(() => { self.OnCloseButtonClickHandler(); });//关闭界面
		}

		public static void ShowWindow(this DlgErrorWindow self, Entity contextData = null)
		{
			if(contextData != null)
            {
				string errorText = null;
				switch((contextData as ErrorData).errorCode)
                {
					case 200002:
						errorText = "网络异常";
						break;
					case 200003:
						errorText = "登录信息错误";
						break;
					case 200004:
						errorText = "登录账号格式错误";
						break;
					case 200005:
						errorText = "登录密码格式错误";
						break;
					case 200006:
						errorText = "该账号属于被封禁";
						break;
					case 200007:
						errorText = "登录密码错误";
						break;
					case 200008:
						errorText = "频繁请求多次";
						break;
					case 200009:
						errorText = "账号认证错误";
						break;
					case 200010:
						errorText = "游戏角色名字重复";
						break;
					case 200011:
						errorText = "角色名称格式错误";
						break;
					case 200012:
						errorText = "该账号未创建角色";
						break;
					default:
						errorText = "未知异常";
						break;
				}
				self.View.ETextText.SetText(errorText);
			}
		}

		public static void OnCloseButtonClickHandler(this DlgErrorWindow self)
        {
			self.ZoneScene().GetComponent<UIComponent>().HideWindow(WindowID.WindowID_ErrorWindow); ;//隐藏错误窗口
		}

	}
}
