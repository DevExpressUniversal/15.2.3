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
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Security {
	public class MyDetailsController : WindowController {
		public const string MyDetailsNavigationItemId = "MyDetails";
		public const string MyDetailsActionId = "MyDetails";
		public const string MyDetailsActionImageName = "BO_MyDetails";
		public const string MyDetailsActionActiveKeyUserTypeAvailable = "UserTypeAvailable";
		public const string MyDetailsActionActiveKeyShortcutIsAssigned = "MyDetailsShortcutIsAssigned";
		public const string MyDetailsActionActiveKeySecurity = "Security";
		public const string MyDetailsObjectKey = CriteriaWrapper.ParameterPrefix + CurrentUserIdParameter.ParameterName;
		public static bool CanGenerateMyDetailsNavigationItem = true;
		private SimpleAction myDetailsAction;
		private ViewShortcut myDetailsViewShortcut = null;
		private bool myDetailsViewShortcutHasValue = false;
		private ViewShortcut myDetailsViewShortcutDefault = null;
		private ShowNavigationItemController showNavigationItemController = null;
		private void showNavigationItemController_ItemsInitialized(object sender, EventArgs e) {
			NavigationItemsInitializedHandler();
		}
		private void myDetailsAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			e.ShowViewParameters.CreatedView = ProcessShortcut(MyDetailsViewShortcut);
		}
		private void InitializeCurrentUserKeyRecursive(ChoiceActionItemCollection items) {
			if(items == null || (SecuritySystem.CurrentUserId == null)) {
				return;
			}
			foreach(ChoiceActionItem item in items) {
				ViewShortcut viewShortcut = item.Data as ViewShortcut;
				InitializeMyDetailsShortcut(viewShortcut);
				InitializeCurrentUserKeyRecursive(item.Items);
			}
		}
		private void InitializeMyDetailsShortcut(ViewShortcut viewShortcut) {
			if(SecuritySystem.CurrentUserId == null || SecuritySystem.CurrentUser == null) {
				return;
			}
			if((viewShortcut != null)
				&& (viewShortcut.ObjectKey != null)
				&& (string.Compare(viewShortcut.ObjectKey.ToString(), MyDetailsObjectKey) == 0)) {
				viewShortcut.ObjectKey = SecuritySystem.CurrentUserId.ToString();
				viewShortcut.ObjectClassName = SecuritySystem.CurrentUser.GetType().FullName;
			}
		}
		internal virtual void NavigationItemsInitializedHandler() {
			InitializeCurrentUserKeyRecursive(GetShowNavigationItemActionItems());
			ChoiceActionItem navigationItem = GetShowNavigationItemActionItems().Find(MyDetailsNavigationItemId, ChoiceActionItemFindType.Recursive, ChoiceActionItemFindTarget.Leaf);
			if(navigationItem != null && navigationItem.Data is ViewShortcut) {
				MyDetailsViewShortcutDefault = navigationItem.Data as ViewShortcut;
			}
		}
		#region Methods to access environment
		protected internal virtual View ProcessShortcut(ViewShortcut viewShortcut) {
			Guard.ArgumentNotNull(Application, "Application");
			return Application.ProcessShortcut(viewShortcut);
		}
		protected internal virtual Type GetUserType() {
			return SecurityModule.GetUserType(Application, null);
		}
		protected internal virtual IModelApplication GetApplicationModel() {
			Guard.ArgumentNotNull(Application, "Application");
			Guard.ArgumentNotNull(Application.Model, "Application.ModelApplication");
			return Application.Model;
		}
		protected virtual ChoiceActionItemCollection GetShowNavigationItemActionItems() {
			return Frame.GetController<ShowNavigationItemController>().ShowNavigationItemAction.Items;
		}
		#endregion
		protected override void OnFrameAssigned() {
			base.OnFrameAssigned();
			showNavigationItemController = Frame.GetController<ShowNavigationItemController>();
			if(showNavigationItemController != null) {
				showNavigationItemController.ItemsInitialized += new EventHandler<EventArgs>(showNavigationItemController_ItemsInitialized);
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			if(MyDetailsViewShortcut == null) {
				Type userType = GetUserType();
				if(userType != null) {
					IModelApplication modelApplication = GetApplicationModel();
					IModelClass modelClass = modelApplication.BOModel.GetClass(userType);
					if(modelClass != null && modelClass.DefaultDetailView != null) {
						MyDetailsViewShortcutDefault = new ViewShortcut(modelClass.DefaultDetailView.Id, MyDetailsObjectKey);
					}
				}
			}
			object currentUser = SecuritySystem.CurrentUser;
			if(currentUser != null) {
				IObjectSpace objectSpace = SecuritySystem.LogonObjectSpace; 
				MyDetailsAction.Active[MyDetailsActionActiveKeySecurity] = DataManipulationRight.CanNavigate(currentUser.GetType(), currentUser, objectSpace);
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(showNavigationItemController != null) {
					showNavigationItemController.ItemsInitialized -= new EventHandler<EventArgs>(showNavigationItemController_ItemsInitialized);
					showNavigationItemController = null;
				}
				MyDetailsViewShortcut = null;
			}
			base.Dispose(disposing);
		}
		public MyDetailsController() {
			myDetailsAction = new SimpleAction(this, MyDetailsActionId, "Security");
			myDetailsAction.ImageName = MyDetailsActionImageName;
			myDetailsAction.PaintStyle = Templates.ActionItemPaintStyle.CaptionAndImage;
			myDetailsAction.Execute += new SimpleActionExecuteEventHandler(myDetailsAction_Execute);
			myDetailsAction.Active[MyDetailsActionActiveKeyShortcutIsAssigned] = false;
			TargetWindowType = WindowType.Main;
		}
		private ViewShortcut MyDetailsViewShortcutDefault {
			get { return myDetailsViewShortcutDefault; }
			set {
				myDetailsViewShortcutDefault = value;
				OnMyDetailsViewShortcutChanged();
			}
		}
		public ViewShortcut MyDetailsViewShortcut {
			get {
				if(myDetailsViewShortcutHasValue) {
					return myDetailsViewShortcut;
				}
				return MyDetailsViewShortcutDefault;
			}
			set { 
				myDetailsViewShortcut = value;
				myDetailsViewShortcutHasValue = true;
				OnMyDetailsViewShortcutChanged();
			}
		}
		private void OnMyDetailsViewShortcutChanged() {
			InitializeMyDetailsShortcut(MyDetailsViewShortcut);
			MyDetailsAction.Active[MyDetailsController.MyDetailsActionActiveKeyShortcutIsAssigned] = (MyDetailsViewShortcut != null);
		}
		public SimpleAction MyDetailsAction {
			get { return myDetailsAction; }
		}
	}
	public class NavigationItemsMyDetailsNodeUpdater : ModelNodesGeneratorUpdater<NavigationItemNodeGenerator> {
		internal virtual Type GetUserType() {
			return UserType;
		}
		internal virtual IModelNavigationItem GenerateModelNavigationItem(IModelApplication modelApplication, string navigationItemGroupName, string navigationItemId, string navigationItemCaption, string viewId, string objectKey) {
			return ShowNavigationItemController.GenerateNavigationItem(modelApplication, navigationItemGroupName, navigationItemId, navigationItemCaption, viewId, objectKey);
		}
		[Obsolete("Use 'NavigationItemsMyDetailsNodeUpdater(Type userType)' instead.")]
		public NavigationItemsMyDetailsNodeUpdater(XafApplication application, ApplicationModulesManager modulesManager) {
		}
		public NavigationItemsMyDetailsNodeUpdater(Type userType) {
			this.UserType = userType;
		}
		public override void UpdateNode(ModelNode node) {
			Type userType = GetUserType();
			if(MyDetailsController.CanGenerateMyDetailsNavigationItem && userType != null) {
				IModelApplication modelApplication = node.Application;
				Guard.ArgumentNotNull(modelApplication, "modelApplication");
				IModelClass modelClass = modelApplication.BOModel.GetClass(userType);
				if(modelClass == null) {
					return;
				}
				string defaultDetailViewId = modelClass.DefaultDetailView != null ? modelClass.DefaultDetailView.Id : string.Empty;
				IModelNavigationItem modelNavigationItem = GenerateModelNavigationItem(
					modelApplication, ((IModelClassNavigation)modelClass).NavigationGroupName, MyDetailsController.MyDetailsNavigationItemId, CaptionHelper.ConvertCompoundName(MyDetailsController.MyDetailsNavigationItemId),
				   defaultDetailViewId, MyDetailsController.MyDetailsObjectKey);
				if(modelNavigationItem != null) {
					modelNavigationItem.ImageName = MyDetailsController.MyDetailsActionImageName;
				}
			}
		}
		public Type UserType { get; private set; }
	}
}
