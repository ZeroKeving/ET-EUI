
using UnityEngine;
using UnityEngine.UI;
namespace ET
{
	public  class DlgHintWindowViewComponent : Entity,IAwake,IDestroy 
	{
		public UnityEngine.UI.Button E_PanelButton
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_PanelButton == null )
     			{
		    		this.m_E_PanelButton = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"E_Panel");
     			}
     			return this.m_E_PanelButton;
     		}
     	}

		public UnityEngine.UI.Image E_PanelImage
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_PanelImage == null )
     			{
		    		this.m_E_PanelImage = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"E_Panel");
     			}
     			return this.m_E_PanelImage;
     		}
     	}

		public UnityEngine.UI.Text ETextText
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_ETextText == null )
     			{
		    		this.m_ETextText = UIFindHelper.FindDeepChild<UnityEngine.UI.Text>(this.uiTransform.gameObject,"EText");
     			}
     			return this.m_ETextText;
     		}
     	}

		public UnityEngine.UI.Button EButtonButton
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EButtonButton == null )
     			{
		    		this.m_EButtonButton = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"EButton");
     			}
     			return this.m_EButtonButton;
     		}
     	}

		public UnityEngine.UI.Image EButtonImage
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Log.Error("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EButtonImage == null )
     			{
		    		this.m_EButtonImage = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"EButton");
     			}
     			return this.m_EButtonImage;
     		}
     	}

		public void DestroyWidget()
		{
			this.m_E_PanelButton = null;
			this.m_E_PanelImage = null;
			this.m_ETextText = null;
			this.m_EButtonButton = null;
			this.m_EButtonImage = null;
			this.uiTransform = null;
		}

		private UnityEngine.UI.Button m_E_PanelButton = null;
		private UnityEngine.UI.Image m_E_PanelImage = null;
		private UnityEngine.UI.Text m_ETextText = null;
		private UnityEngine.UI.Button m_EButtonButton = null;
		private UnityEngine.UI.Image m_EButtonImage = null;
		public Transform uiTransform = null;
	}
}
