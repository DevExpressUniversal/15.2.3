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

using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base.General;
using System;
namespace DevExpress.ExpressApp.Notifications {
	public class NotificationsMessageListViewController : ObjectViewController<ListView, Notification> {
		protected NotificationsService service;
		private void NotificationMessageListViewController_CustomProcessSelectedItem(object sender, CustomProcessListViewSelectedItemEventArgs e) {
			View view = CreateDetailView();
			PrepareShowViewParameters(e.InnerArgs.ShowViewParameters, view);
			e.Handled = true;
		}
		protected bool HasMember(Object targetObject, String memberName) {
			bool result = false;
			ITypeInfo requestedTypeInfo = XafTypesInfo.Instance.FindTypeInfo(targetObject.GetType());
			if(requestedTypeInfo != null) {
				result = requestedTypeInfo.FindMember(memberName) != null;
			}
			return result;
		}
		protected virtual string FieldMapping(string fieldName) {
			if(fieldName == "State") {
				fieldName = "IsPostponed";
			}
			if(fieldName == "Subject") {
				fieldName = "NotificationMessage";
			}
			return fieldName;
		}
		protected bool NeedProtectedContent(Notification item, string fieldName) {
			fieldName = FieldMapping(fieldName);
			IObjectSpace objectSpace = View.CollectionSource.ObjectSpace;
			object sourceObject = item.NotificationSource;
			Type objectType = objectSpace.GetObjectType(sourceObject);
			return HasMember(sourceObject, fieldName) && !DataManipulationRight.CanRead(objectType, fieldName, sourceObject, View.CollectionSource, objectSpace);
		}
		protected override void OnViewControlsCreated() {
			base.OnViewControlsCreated();
			if(Frame != null) {
				if(Frame.Template is ISupportActionsToolbarVisibility) {
					((ISupportActionsToolbarVisibility)(Frame.Template)).SetVisible(false);
				}
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			service = Application.Modules.FindModule<NotificationsModule>().NotificationsService;
			if(Frame.GetController<ListViewProcessCurrentObjectController>() != null) {
				Frame.GetController<ListViewProcessCurrentObjectController>().CustomProcessSelectedItem += NotificationMessageListViewController_CustomProcessSelectedItem;
			}
		}
		protected View CreateDetailView() {
			Object obj = ViewCurrentObject.NotificationSource;
			IObjectSpace objectSpace = Application.CreateObjectSpace(obj.GetType());
			Object objectInTargetObjectSpace = objectSpace.GetObject(obj);
			View view = Application.CreateDetailView(objectSpace, objectInTargetObjectSpace);
			ProcessDetailView(view);
			return view;
		}
		protected virtual void ProcessDetailView(View view) {
			view.ObjectSpace.Committed += delegate(object obj, EventArgs arg) {
				view.Closed += delegate(object s, EventArgs args) {
					INotificationItem sourceObject = ViewCurrentObject.Source;
					if(sourceObject != null) {
						service.SetItemChanged(sourceObject);
					}
					service.Refresh();
				};
			};
		}
		protected void PrepareShowViewParameters(ShowViewParameters showViewParameters, View view) {
			showViewParameters.CreatedView = view;
			NotificationsOnCommitController notificationsOnCommitController = Application.CreateController<NotificationsOnCommitController>();
			notificationsOnCommitController.Active.SetItemValue("DetailViewFromNotificationsWindow", false);
			showViewParameters.Controllers.Add(notificationsOnCommitController);
			showViewParameters.Context = TemplateContext.View;
		}
		protected override void OnDeactivated() {
			if(Frame.GetController<ListViewProcessCurrentObjectController>() != null) {
				Frame.GetController<ListViewProcessCurrentObjectController>().CustomProcessSelectedItem -= NotificationMessageListViewController_CustomProcessSelectedItem;
			}
			base.OnDeactivated();
		}
		public NotificationsMessageListViewController() {
			this.TargetViewNesting = Nesting.Nested;
		}
	}
}
