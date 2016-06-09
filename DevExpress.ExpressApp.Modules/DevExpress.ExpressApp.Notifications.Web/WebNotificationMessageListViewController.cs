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

using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.Web;
using System;
namespace DevExpress.ExpressApp.Notifications.Web {
	public class WebNotificationsMessageListViewController : NotificationsMessageListViewController {
		private ASPxGridView control;
		private void Control_CustomColumnDisplayText(object sender, ASPxGridViewColumnDisplayTextEventArgs e) {
			if(e.Column.UnboundType == Data.UnboundColumnType.Bound) { 
				Notification item = e.Column.Grid.GetRow(e.VisibleRowIndex) as Notification;
				SetDisplayText(item, e);
			}
		}
		private void UpdateSelectionChanged() {
			((ASPxGridListEditor)View.Editor).SelectionChanged += delegate(object sender1, EventArgs e1) {
				((ASPxGridListEditor)View.Editor).RegisterCallbackStartupScript -= WebNotificationMessageListViewController_RegisterCallbackStartupScript;
				((ASPxGridListEditor)View.Editor).RegisterCallbackStartupScript += WebNotificationMessageListViewController_RegisterCallbackStartupScript;
			};
		}
		private void WebNotificationMessageListViewController_RegisterCallbackStartupScript(object sender, RegisterCallbackStartupScriptEventArgs e) {
			if(control != null) {
				ICallbackManagerHolder holder = control.Page as ICallbackManagerHolder;
				e.StartupScript = string.Format("setTimeout(\"{0}\",  0);", holder.CallbackManager.GetScript());
			}
		}
		private void WebNotificationMessageListViewController_CustomExecuteEdit(object sender, System.ComponentModel.HandledEventArgs e) {
			e.Handled = true;
		}
		private void EditAction_Execute(object sender, Actions.SimpleActionExecuteEventArgs e) {
			View view = CreateDetailView();
			PrepareShowViewParameters(e.ShowViewParameters, view);
			((DetailView)e.ShowViewParameters.CreatedView).ViewEditMode = ViewEditMode.Edit;
		}
		protected void SetDisplayText(Notification item, ASPxGridViewColumnDisplayTextEventArgs e) {
			if(NeedProtectedContent(item, e.Column.FieldName)) {
				e.DisplayText = Application.Model.ProtectedContentText;
			}
		}
		protected override void ProcessDetailView(View view) {
			base.ProcessDetailView(view);
			view.ObjectSpace.SetModified(View.CurrentObject);
		}
		protected override void OnActivated() {
			base.OnActivated();
			if(Frame.GetController<ListViewController>() != null) {
				Frame.GetController<ListViewController>().EditAction.Execute += EditAction_Execute;
				Frame.GetController<ListViewController>().CustomExecuteEdit += WebNotificationMessageListViewController_CustomExecuteEdit;
			}
			UpdateSelectionChanged();
		}
		protected override void OnDeactivated() {
			if(Frame.GetController<ListViewController>() != null) {
				Frame.GetController<ListViewController>().EditAction.Execute -= EditAction_Execute;
				Frame.GetController<ListViewController>().CustomExecuteEdit -= WebNotificationMessageListViewController_CustomExecuteEdit;
			}
			if(View.Editor != null) {
				((ASPxGridListEditor)View.Editor).RegisterCallbackStartupScript -= WebNotificationMessageListViewController_RegisterCallbackStartupScript;
			}
			if(control != null) {
				control.CustomColumnDisplayText -= Control_CustomColumnDisplayText;
			}
			base.OnDeactivated();
		}
		protected override void OnViewControlsCreated() {
			base.OnViewControlsCreated();
			control = View.Editor.Control as ASPxGridView;
			if(control != null) {
				control.CustomColumnDisplayText += Control_CustomColumnDisplayText;
				control.SettingsPager.Mode = GridViewPagerMode.ShowAllRecords;
				control.Settings.ShowStatusBar = GridViewStatusBarMode.Hidden;
				control.Width = System.Web.UI.WebControls.Unit.Percentage(100);
			}
		}
	}
}
