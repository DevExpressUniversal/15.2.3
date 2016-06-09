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
using System.Linq;
using System.Text;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.ReportsV2 {
	public class CopyPredefinedReportsController : ReportDataViewController {
		public static readonly string CopyPredefinedReportActionId = "CopyPredefinedReport";
		private SimpleAction copyPredefinedReportAction;
		public CopyPredefinedReportsController()
			: base() {
			this.copyPredefinedReportAction = new SimpleAction(this, "CopyPredefinedReport", PredefinedCategory.Reports);
			this.copyPredefinedReportAction.Caption = "Copy Predefined Report";
			this.copyPredefinedReportAction.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;
			this.copyPredefinedReportAction.ImageName = "Action_Copy";
			this.copyPredefinedReportAction.Execute += new SimpleActionExecuteEventHandler(CopyPredefinedReportAction_Execute);
		}
		public SimpleAction CopyPredefinedReportAction {
			get { return copyPredefinedReportAction; }
		}
		protected override void OnActivated() {
			base.OnActivated();
			UpdateActionActive();
			if(View != null) {
				View.SelectionChanged += new EventHandler(View_SelectionChanged);
				View.CurrentObjectChanged += new EventHandler(View_CurrentObjectChanged);
				UpdateActionState();
			}
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			View.SelectionChanged -= new EventHandler(View_SelectionChanged);
			View.CurrentObjectChanged -= new EventHandler(View_CurrentObjectChanged);
		}
		private void View_SelectionChanged(object sender, EventArgs e) {
			UpdateActionState();
		}
		private void View_CurrentObjectChanged(object sender, EventArgs e) {
			UpdateActionState();
		}
		protected virtual void CopyPredefinedReports(SimpleActionExecuteEventArgs e) {
			List<IReportDataV2> reports = new List<IReportDataV2>();
			foreach(object item in e.SelectedObjects) {
				IReportDataV2 reportData = GetReportData(item);
				Guard.ArgumentNotNull(reportData, "reportData");
				reports.Add(reportData);
			}
			ReportDataProvider.ReportsStorage.CopyReports(reports.ToArray());
			if(View != null)
				View.ObjectSpace.Refresh();
		}
		private void CopyPredefinedReportAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			CopyPredefinedReports(e);
		}
		private void UpdateActionState() {
			if(View.SelectedObjects.Count == 1) {
				bool isPredefined = View.CurrentObject != null ? CurrentReportData.IsPredefined : false;
				copyPredefinedReportAction.Enabled.SetItemValue("User-Defined", isPredefined);
			}
		}
		private void UpdateActionActive() {
			bool result = false;
			if(ReportDataProvider.ReportsStorage != null) {
				Type reportDataType = ReportDataProvider.ReportsStorage.ReportDataType;
				if(reportDataType != null) {
					result = DataManipulationRight.CanEdit(reportDataType, null, null, null, null);
				}
				copyPredefinedReportAction.Active.SetItemValue("Security", result);
			}
		}
	}
}
