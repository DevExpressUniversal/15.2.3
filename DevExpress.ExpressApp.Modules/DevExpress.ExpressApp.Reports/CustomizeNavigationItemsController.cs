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
using DevExpress.Persistent.Base.General;
namespace DevExpress.ExpressApp.Reports {
	public class CustomizeNavigationItemsController : WindowController, IReportNavigationItemsHandler {
		public const string NavigationItemIdPrefix = "RelatedReportItem_Report_";
		private ShowNavigationItemController showNavigationItemController;
		private ITypeInfo reportDataTypeInfo;
		private void showNavigationItemController_CustomShowNavigationItem(object sender, CustomShowNavigationItemEventArgs e) {
			if(e.ActionArguments.SelectedChoiceActionItem.Id.StartsWith(NavigationItemIdPrefix)) {
				ShowReport((IReportData)e.ActionArguments.SelectedChoiceActionItem.Data);
				e.Handled = true;
			}
		}
		private void showNavigationItemController_NavigationItemCreated(object sender, NavigationItemCreatedEventArgs e) {
			IModelView modelView = ((IModelNavigationItem)e.NavigationItem.Model).View;
			if(modelView is IModelObjectView) {
				ITypeInfo objectTypeInfo = ((IModelObjectView)modelView).ModelClass.TypeInfo;
				List<IReportData> reports = InplaceReportCacheController.GetReportDataList(Frame, XafTypesInfo.CastTypeInfoToType(objectTypeInfo));
				if(reports.Count > 0) {
					ChoiceActionItem reportsNavigationGroup = new ChoiceActionItem("Reports", RelatedReportsGroupCaption, null);
					reportsNavigationGroup.Model.ImageName = "BO_Report";
					e.NavigationItem.Items.Add(reportsNavigationGroup);
					List<ChoiceActionItem> items = new List<ChoiceActionItem>();
					foreach(IReportData reportData in reports) {
						object key = reportDataTypeInfo.KeyMember.GetValue(reportData);
						ChoiceActionItem item = new ChoiceActionItem(NavigationItemIdPrefix + key.ToString(), reportData.ReportName, reportData);
						item.Model.ImageName = "Navigation_Item_Report";
						items.Add(item);
					}
					items.Sort(delegate(ChoiceActionItem left, ChoiceActionItem right) {
						return Comparer<string>.Default.Compare(left.Caption, right.Caption);
					});
					reportsNavigationGroup.Items.AddRange(items);
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
				return navigationItems != null ? navigationItems.RelatedReportsGroupCaption : ReportsModule.DefaultReportDataNavigationItemCaption;
			}
		}
		protected virtual void ShowReport(IReportData reportData) {
			Frame.GetController<ReportServiceController>().ShowPreview(reportData);
		}
		protected override void OnFrameAssigned() {
			base.OnFrameAssigned();
			UnsubscribeFromEvents();
			if(GenerateRelatedReportsGroup) {
				showNavigationItemController = Frame.GetController<ShowNavigationItemController>();
				ReportsModule reportsModule = ReportsModule.FindReportsModule(Application.Modules);
				if(reportsModule != null) {
					reportDataTypeInfo = XafTypesInfo.CastTypeToTypeInfo(reportsModule.ReportDataType);
					if(showNavigationItemController != null && reportDataTypeInfo != null && !isSuspended) {
						showNavigationItemController.NavigationItemCreated += new EventHandler<NavigationItemCreatedEventArgs>(showNavigationItemController_NavigationItemCreated);
						showNavigationItemController.CustomShowNavigationItem += new EventHandler<CustomShowNavigationItemEventArgs>(showNavigationItemController_CustomShowNavigationItem);
					}
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
		#region IReportNavigationItemsHandler Members
		private bool isSuspended = false;
		void IReportNavigationItemsHandler.SuspendEvents() {
			isSuspended = true;
			UnsubscribeFromEvents();
		}
		void IReportNavigationItemsHandler.ProcessNavigationItem(object navigationData) {
			ShowReport((IReportData)navigationData);			
		}
		List<ReportNavigationItemInfo> IReportNavigationItemsHandler.GetReportNavigationItemsInfo(Type targetObjectType) {
			List<IReportData> reports = InplaceReportCacheController.GetReportDataList(Frame, targetObjectType);
			List<ReportNavigationItemInfo> items = new List<ReportNavigationItemInfo>();
			if(reportDataTypeInfo != null) {
				foreach(IReportData reportData in reports) {
					object key = reportDataTypeInfo.KeyMember.GetValue(reportData);
					ReportNavigationItemInfo item = new ReportNavigationItemInfo(key.ToString(), reportData.ReportName, reportData);
					items.Add(item);
				}
			}
			return items;
		}
		#endregion
	}
}
