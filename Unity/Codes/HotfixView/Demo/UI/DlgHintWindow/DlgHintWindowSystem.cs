using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
	public static  class DlgHintWindowSystem
	{

		public static void RegisterUIEvent(this DlgHintWindow self)
		{
			self.View.EButtonButton.AddListener(() => { self.OnCloseButtonClickHandler(); });//关闭界面
			self.View.E_PanelButton.AddListener(() => { self.OnCloseButtonClickHandler(); });//关闭界面
		}

		public static void ShowWindow(this DlgHintWindow self, Entity contextData = null)
		{
			if (contextData != null)
			{
				string hintText = null;
				switch ((contextData as HintData).hintCode)
				{
					case 300002:
						hintText = "该账号未创建角色";
						break;
					case 300003:
						hintText = "角色名称不能为空";
						break;
					case 300004:
						hintText = "该服务器的角色删除成功";
						break;
					default:
						hintText = "未知提示";
						break;
				}
				self.View.ETextText.SetText(hintText);
			}
		}

		public static void OnCloseButtonClickHandler(this DlgHintWindow self)
		{
			self.ZoneScene().GetComponent<UIComponent>().HideWindow(WindowID.WindowID_HintWindow); ;//隐藏提示窗口
		}



	}
}
