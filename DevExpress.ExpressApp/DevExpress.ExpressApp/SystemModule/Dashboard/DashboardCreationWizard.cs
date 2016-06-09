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
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.DomainLogics;
namespace DevExpress.ExpressApp.SystemModule {
	[DomainComponent]
	[DisplayName("Dashboard Creation Wizard")]
	public class DashboardCreationInfo {
		private IModelApplication model;
		private string dashboardName;
		private string navigationGroupName;
		private Dictionary<string, string> navigationGroupsCaptionToIdMapping;
		private DashboardOrganizer dashboardOrganizer;
		private void CreateNavigationItem(IModelDashboardView modelDashboardView) {
			if(string.IsNullOrEmpty(NavigationGroupName)) {
				return;
			}
			IModelApplicationNavigationItems navigationModel = modelDashboardView.Application as IModelApplicationNavigationItems;
			if(navigationModel != null) {
				string dashboardGroupId = NavigationGroupName;
				if(navigationGroupsCaptionToIdMapping.ContainsKey(dashboardGroupId)) {
					dashboardGroupId = navigationGroupsCaptionToIdMapping[dashboardGroupId];
				}
				ViewShortcut viewShortcut = new ViewShortcut(modelDashboardView.Id, null);
				IModelNavigationItem navigationItem = ShowNavigationItemController.GenerateNavigationItem(modelDashboardView.Application, viewShortcut, dashboardGroupId);
				navigationItem.ImageName = ModelDashboardViewLogic.DefaultDashboardViewImageName;
				navigationItem.Caption = DashboardName;
			}
		}
		public DashboardCreationInfo(IModelApplication model) {
			this.model = model;
			dashboardOrganizer = new DashboardOrganizer();
			navigationGroupsCaptionToIdMapping = new Dictionary<string, string>();
			List<string> navigationGroupNames = new List<string>();
			foreach(IModelNavigationItem item in ((IModelApplicationNavigationItems)model).NavigationItems.Items) {
				if(item.View == null) {
					navigationGroupNames.Add(item.Caption);
					navigationGroupsCaptionToIdMapping[item.Caption] = item.Id;
				}
			}
			navigationGroupNames.Sort();
			IModelClass thisClassInfo = model.BOModel.GetClass(typeof(DashboardCreationInfo));
			if(thisClassInfo != null) {
				thisClassInfo.OwnMembers["NavigationGroupName"].PredefinedValues = string.Join(";", navigationGroupNames.ToArray());
			}
		}
		public IModelDashboardView CreateDashboardModel() {
			if(!string.IsNullOrEmpty(DashboardName)) {
				if(model.Views[DashboardName] != null){
					throw new InvalidOperationException(string.Format(@"The view with the ""{0}"" id already exists.", DashboardName));
				}
			}
			IModelDashboardView dashboard = model.Views.AddNode<IModelDashboardView>(DashboardName);
			dashboard.Layout.AddNode<IModelLayoutGroup>("Main").ShowCaption = false;
			dashboardOrganizer.SaveDashboardChangesToModel(dashboard);
			if(!string.IsNullOrEmpty(NavigationGroupName)) {
				CreateNavigationItem(dashboard);
			}
			return dashboard;
		}
		[DisplayName("Name")]
		public string DashboardName {
			get { return dashboardName; }
			set { dashboardName = value; }
		}
		[DisplayName("Target Navigation Group")]
		public string NavigationGroupName {
			get { return navigationGroupName; }
			set { navigationGroupName = value; }
		}
		[Aggregated]
		public BindingList<DashboardOrganizationItem> ViewItems {
			get { return dashboardOrganizer.ViewItems; }
		}		
	}
	public class DashboardCreationWizardController : WindowController, IModelExtender {
		public const string RuntimeDashboardCreationEnabledKey = "RuntimeDashboardCreationEnabled";
		private PopupWindowShowAction createDashboard;
		private void ShowDashboardView(IModelDashboardView modelDashboardView) {
			ViewShortcut viewShortcut = new ViewShortcut(modelDashboardView.Id, null);
			ShowViewParameters viewParameters = new ShowViewParameters(Application.ProcessShortcut(viewShortcut));
			viewParameters.TargetWindow = TargetWindow.Current;
			Application.ShowViewStrategy.ShowView(viewParameters, new ShowViewSource(Frame, null));
		}
		private void createDashboard_Execute(object sender, PopupWindowShowActionExecuteEventArgs e) {
			e.CanCloseWindow = true;
			DashboardCreationInfo dashboardCreationInfo = e.PopupWindow.View.CurrentObject as DashboardCreationInfo;
			IModelDashboardView modelDashboardView = dashboardCreationInfo.CreateDashboardModel();
			if(!string.IsNullOrEmpty(dashboardCreationInfo.NavigationGroupName)) {
				ShowNavigationItemController navigationItemController = Frame.GetController<ShowNavigationItemController>();
				navigationItemController.RecreateNavigationItems();
				navigationItemController.UpdateSelectedItem(new ViewShortcut(modelDashboardView.Id, null));
			}
			ShowDashboardView(modelDashboardView);
		}
		private void createDashboard_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e) {
			DashboardCreationInfo dashboardCreationInfo = new DashboardCreationInfo(Application.Model);
			DetailView dashboardCreationInfoView = Application.CreateDetailView(Application.CreateObjectSpace(typeof(DashboardCreationInfo)), dashboardCreationInfo);
			dashboardCreationInfoView.ViewEditMode = ViewEditMode.Edit;
			e.View = dashboardCreationInfoView;
		}
		protected override void UpdateActionActivity(ActionBase action) {
			base.UpdateActionActivity(action);
			IModelOptionsDashboards dashboardOptions = Application.Model.Options as IModelOptionsDashboards;
			if(dashboardOptions != null) {
				createDashboard.Active.SetItemValue(RuntimeDashboardCreationEnabledKey, dashboardOptions.Dashboards.EnableCreation);
			}
		}
		public DashboardCreationWizardController() {
			TargetWindowType = WindowType.Main;
			createDashboard = new PopupWindowShowAction(this, "CreateDashboard", DevExpress.Persistent.Base.PredefinedCategory.Tools);
			createDashboard.ImageName = "Action_CreateDashboard";
			createDashboard.Execute += new PopupWindowShowActionExecuteEventHandler(createDashboard_Execute);
			createDashboard.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(createDashboard_CustomizePopupWindowParams);
		}
		public PopupWindowShowAction CreateDashboardAction {
			get {
				return createDashboard;
			}
		}
		#region IModelExtender Members
		void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
			extenders.Add<IModelOptions, IModelOptionsDashboards>();
		}
		#endregion
	}
}
