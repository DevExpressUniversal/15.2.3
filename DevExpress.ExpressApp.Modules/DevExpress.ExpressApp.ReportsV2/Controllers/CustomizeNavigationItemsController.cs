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
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.ReportsV2 {
	public class CustomizeNavigationItemsController : WindowController {
		public const string NavigationItemIdPrefix = "RelatedReportItem_ReportV2_";
		private ShowNavigationItemController showNavigationItemController;
		private IReportNavigationItemsHandler reportNavigationItemsHandler;
		private ITypeInfo reportDataTypeInfo;
		private IModelClass reportDataModelClass;
		private void showNavigationItemController_CustomShowNavigationItem(object sender, CustomShowNavigationItemEventArgs e) {
			if(e.ActionArguments.SelectedChoiceActionItem.Id.StartsWith(NavigationItemIdPrefix)) {
				if(e.ActionArguments.SelectedChoiceActionItem.Data is ReportDataInfo) {
					ShowReport((ReportDataInfo)e.ActionArguments.SelectedChoiceActionItem.Data);
				}
				else {
					if(reportNavigationItemsHandler != null) {
						reportNavigationItemsHandler.ProcessNavigationItem(e.ActionArguments.SelectedChoiceActionItem.Data);
					}
				}
				e.Handled = true;
			}
		}
		private List<ChoiceActionItem> CreateReportNavigationItems(Type targetObjectType) {
			List<ChoiceActionItem> items = new List<ChoiceActionItem>();
			InplaceReportsCacheHelper inplaceReportsCache = ReportsModuleV2.FindReportsModule(Application.Modules).InplaceReportsCacheHelper;
			if(inplaceReportsCache != null) {
				List<ReportDataInfo> reports = inplaceReportsCache.GetReportDataInfoList(targetObjectType);
				if(reports.Count > 0) {
					foreach(ReportDataInfo reportData in reports) {
						ChoiceActionItem item = new ChoiceActionItem(NavigationItemIdPrefix + reportData.ReportContainerHandle, reportData.DisplayName, reportData);
						item.Model.ImageName = reportDataModelClass != null ? reportDataModelClass.ImageName : "Navigation_Item_Report";
						items.Add(item);
					}
				}
			}
			return items;
		}
		private List<ChoiceActionItem> CreateReportNavigationItemsFromHandler(Type targetObjectType) {
			List<ChoiceActionItem> items = new List<ChoiceActionItem>();
			if(reportNavigationItemsHandler != null) {
				foreach(ReportNavigationItemInfo reportNavigationItemInfo in reportNavigationItemsHandler.GetReportNavigationItemsInfo(targetObjectType)) {
					ChoiceActionItem item = new ChoiceActionItem(NavigationItemIdPrefix + reportNavigationItemInfo.ItemKey, reportNavigationItemInfo.ItemName, reportNavigationItemInfo.ItemData);
					item.Model.ImageName = reportDataModelClass != null ? reportDataModelClass.ImageName : "Navigation_Item_Report";
					items.Add(item);
				}
			}
			return items;
		}
		private void showNavigationItemController_NavigationItemCreated(object sender, NavigationItemCreatedEventArgs e) {
			if(GenerateRelatedReportsGroup) {
				IModelView modelView = ((IModelNavigationItem)e.NavigationItem.Model).View;
				if(modelView is IModelObjectView) {
					ITypeInfo objectTypeInfo = ((IModelObjectView)modelView).ModelClass.TypeInfo;
					Type targetObjectType = XafTypesInfo.CastTypeInfoToType(objectTypeInfo);
					List<ChoiceActionItem> items = new List<ChoiceActionItem>();
					items.AddRange(CreateReportNavigationItems(targetObjectType));
					items.AddRange(CreateReportNavigationItemsFromHandler(targetObjectType));
					if(items.Count > 0) {
						ChoiceActionItem reportsNavigationGroup = new ChoiceActionItem("Reports", RelatedReportsGroupCaption, null);
						reportsNavigationGroup.Model.ImageName = reportDataModelClass != null ? reportDataModelClass.ImageName : "BO_Report";
						e.NavigationItem.Items.Add(reportsNavigationGroup);
						items.Sort(delegate(ChoiceActionItem left, ChoiceActionItem right) {
							return Comparer<string>.Default.Compare(left.Caption, right.Caption);
						});
						reportsNavigationGroup.Items.AddRange(items);
					}
				}
			}
		}
		private bool GenerateRelatedReportsGroup {
			get {
				IModelNavigationItemsForReports navigationItems = ((IModelApplicationNavigationItems)Application.Model).NavigationItems as IModelNavigationItemsForReports;
				return navigationItems != null ? navigationItems.GenerateRelatedReportsGroup : false;
			}
		}
		private string RelatedReportsGroupCaption {
			get {
				IModelNavigationItemsForReports navigationItems = ((IModelApplicationNavigationItems)Application.Model).NavigationItems as IModelNavigationItemsForReports;
				return navigationItems != null ? navigationItems.RelatedReportsGroupCaption : ReportsModuleV2.DefaultReportDataNavigationItemCaption;
			}
		}
		protected virtual void ShowReport(ReportDataInfo reportDataInfo) {
			Frame.GetController<ReportServiceController>().ShowPreview(reportDataInfo.ReportContainerHandle);
		}
		protected override void OnFrameAssigned() {
			base.OnFrameAssigned();
			UnsubscribeFromEvents();
			SubscribeToEvents();
			reportNavigationItemsHandler = FindReportNavigationItemsHandler();
			if(reportNavigationItemsHandler != null) {
				reportNavigationItemsHandler.SuspendEvents();
			}
		}
		private IReportNavigationItemsHandler FindReportNavigationItemsHandler() {
			foreach(Controller controller in Frame.Controllers) {
				if(controller is IReportNavigationItemsHandler) {
					return (IReportNavigationItemsHandler)controller;
				}
			}
			return null;
		}
		private void SubscribeToEvents() {
			showNavigationItemController = Frame.GetController<ShowNavigationItemController>();
			ReportsModuleV2 reportsModule = ReportsModuleV2.FindReportsModule(Application.Modules);
			if(reportsModule != null) {
				reportDataTypeInfo = XafTypesInfo.CastTypeToTypeInfo(reportsModule.ReportDataType);
				reportDataModelClass = Application.Model.BOModel.GetClass(reportsModule.ReportDataType);
				if(showNavigationItemController != null && reportDataTypeInfo != null) {
					showNavigationItemController.NavigationItemCreated += new EventHandler<NavigationItemCreatedEventArgs>(showNavigationItemController_NavigationItemCreated);
					showNavigationItemController.CustomShowNavigationItem += new EventHandler<CustomShowNavigationItemEventArgs>(showNavigationItemController_CustomShowNavigationItem);
				}
			}
		}
		private void UnsubscribeFromEvents() {
			if(showNavigationItemController != null) {
				showNavigationItemController.CustomShowNavigationItem -= new EventHandler<CustomShowNavigationItemEventArgs>(showNavigationItemController_CustomShowNavigationItem);
				showNavigationItemController.NavigationItemCreated -= new EventHandler<NavigationItemCreatedEventArgs>(showNavigationItemController_NavigationItemCreated);
				showNavigationItemController = null;
			}
		}
		protected override void Dispose(bool disposing) {
			UnsubscribeFromEvents();
			reportDataTypeInfo = null;
			base.Dispose(disposing);
		}
		public CustomizeNavigationItemsController() {
			this.TargetWindowType = WindowType.Main;
		}
	}
}
