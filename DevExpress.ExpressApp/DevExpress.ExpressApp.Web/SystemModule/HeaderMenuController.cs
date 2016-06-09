#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Linq;
using System.Text;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.ExpressApp.Web.Templates.ActionContainers;
namespace DevExpress.ExpressApp.Web.SystemModule {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class HeaderMenuController : WindowController {
		public HeaderMenuController() : base() {
			this.Active.SetItemValue("NewStyle", WebApplicationStyleManager.IsNewStyle);
		}
		protected override void OnWindowChanging(Window window) {
			base.OnWindowChanging(window);
			if(window != null) {
				window.TemplateChanged -= window_TemplateChanged;
				window.TemplateChanged += window_TemplateChanged;
			}
		}
		private void window_TemplateChanged(object sender, System.EventArgs e) {
			UpdateActionContainerHolder(Window);
		}
		private void UpdateActionContainerHolder(Window window) {
			if(window.Template is IXafSecurityActionContainerHolder) {
				SetupSecurityActionContainer((IXafSecurityActionContainerHolder)window.Template);
			}
		}
		protected virtual void SetupSecurityActionContainer(IXafSecurityActionContainerHolder actionContainerHolder) {
			if(actionContainerHolder.SecurityActionContainerHolder != null) {
				SetupSecurityActionContainer(actionContainerHolder.SecurityActionContainerHolder.ActionContainers[0]);
			}
		}
		protected virtual void SetupSecurityActionContainer(WebActionContainer actionContainer) {
			actionContainer.DefaultItemCaption = GetActionContainerCaption(actionContainer);
			actionContainer.DefaultItemImageUrl = GetActionContainerImageUrl(actionContainer);
		}
		protected virtual string GetActionContainerCaption(WebActionContainer actionContainer) {
			return CaptionHelper.GetLocalizedText("Captions", "MyAccount", "My Account");
		}
		protected virtual string GetActionContainerImageUrl(WebActionContainer actionContainer) {
			return ImageLoader.Instance.GetImageInfo(actionContainer.DefaultItemImageName).ImageUrl;
		}
	}
}
