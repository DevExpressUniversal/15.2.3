#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web.UI;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public class CallbackPanelStyles : DevExpress.Web.PanelStyles {
		protected ASPxCallbackPanel CallbackPanel { get; private set; }
		public CallbackPanelStyles(ASPxCallbackPanel panel)
			: base(panel) {
				CallbackPanel = panel;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CallbackPanelStylesLoadingPanel"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public LoadingPanelStyle LoadingPanel {
			get { return base.LoadingPanelInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CallbackPanelStylesLoadingDiv"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public LoadingDivStyle LoadingDiv {
			get { return base.LoadingDivInternal; }
		}
		protected override string GetLoadingPanelCssClassName() {
			if(CallbackPanel.HideContentOnCallback)
				return base.GetLoadingPanelCssClassName();
			return "LoadingPanelWithContent";
		}
		protected override bool HideLoadingPanelBorder() {
			return CallbackPanel.HideContentOnCallback;
		}
		protected override string GetLoadingDivCssClassName() {
			return "LoadingDivWithContent";
		}
	}
}
