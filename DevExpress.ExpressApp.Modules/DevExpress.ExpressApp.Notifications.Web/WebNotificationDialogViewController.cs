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
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Controls;
using DevExpress.ExpressApp.Web.Layout;
using DevExpress.ExpressApp.Web.Templates;
namespace DevExpress.ExpressApp.Notifications.Web {
	public class WebNotificationsDialogViewController : NotificationsDialogViewController {
		private SimpleAction closeView;
		private void XafPopupWindowControl_CustomizePopupControl(object sender, CustomizePopupControlEventArgs e) {
			SetPopupMinSize(e);
		}
		private void LayoutManager_ItemCreated(object sender, ItemCreatedEventArgs e) {
			DeleteCaption(e);
		}
		private void Frame_TemplateChanged(object sender, System.EventArgs e) {
			if(Frame.Template != null) {
				((Page)Frame.Template).LoadComplete += (s, args) => {
					CustomizePopupLayout();
				};
			}
		}
		protected virtual void CloseView_Execute(object sender, SimpleActionExecuteEventArgs e) {
			WebWindow.CurrentRequestWindow.RegisterStartupScript("startNotificationsTimer", "window.top.requestInProgress = false;");
			((PopupWindow)this.Frame).Close(true);
		}
		protected virtual void SetPopupMinSize(CustomizePopupControlEventArgs e) {
			e.PopupControl.MinWidth = 570;
			e.PopupControl.MinHeight = 350;
		}
		protected virtual void DeleteCaption(ItemCreatedEventArgs e) {
			if(e.ModelLayoutElement.Id == "ShowNotificationsWindow") {
				((LayoutItemTemplateContainer)e.TemplateContainer).CaptionWidth = new Unit(0);
			}
		}
		protected virtual void CustomizePopupLayout() {
			BaseXafPage page = Frame.Template as BaseXafPage;
			((HtmlContainerControl)page.TemplateContent.Parent).Attributes.Add("class", "NotificationsPopupWindowCustomization");
		}
		protected override void OnActivated() {
			base.OnActivated();
			((DevExpress.ExpressApp.Web.PopupWindow)Frame).RefreshParentWindowOnCloseButtonClick = true;
			((BaseXafPage)WebWindow.CurrentRequestPage).XafPopupWindowControl.CustomizePopupControl += XafPopupWindowControl_CustomizePopupControl;
			((WebLayoutManager)View.LayoutManager).ItemCreated += new EventHandler<ItemCreatedEventArgs>(LayoutManager_ItemCreated);
			Frame.TemplateChanged += Frame_TemplateChanged;
		}
		protected override void OnDeactivated() {
			((BaseXafPage)WebWindow.CurrentRequestPage).XafPopupWindowControl.CustomizePopupControl -= XafPopupWindowControl_CustomizePopupControl;
			((WebLayoutManager)View.LayoutManager).ItemCreated -= new EventHandler<ItemCreatedEventArgs>(LayoutManager_ItemCreated);
			Frame.TemplateChanged -= Frame_TemplateChanged;
			base.OnDeactivated();
		}
		public WebNotificationsDialogViewController() {
			closeView = new SimpleAction(this, "CloseView", DialogController.DialogActionContainerName);
			Actions.Add(closeView);
			closeView.Active["NewStyle"] = WebApplicationStyleManager.IsNewStyle;
			closeView.Execute += CloseView_Execute;
		}
		public SimpleAction CloseView {
			get { return closeView; }
		}
	}
}
