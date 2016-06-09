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
using System.Text;
using DevExpress.XtraScheduler.Services;
using DevExpress.XtraScheduler.Reporting.Forms;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraScheduler.Reporting.Services.Implementation {
	public class ReportPrintService : ISchedulerPrintService {
		SchedulerControlPrintAdapter printAdapter;
		string reportFileName = String.Empty;
		ResourceBaseCollection printResources = new ResourceBaseCollection();
		DateTime endDate;
		DateTime startDate;
		public ReportPrintService(SchedulerControlPrintAdapter adapter) {
			this.printAdapter = adapter;
		}
		public void PageSetup() {
			using(ReportTemplateForm form = new ReportTemplateForm(printAdapter)) {
				form.ShowDialog();
				this.reportFileName = form.ReportFileName;
				this.printResources = form.PrintResources;
				this.startDate = form.StartDate;
				this.endDate = form.EndDate;
			}
		}
		public void Print() {
			if (String.IsNullOrEmpty(this.reportFileName))
				return;
			using (XtraSchedulerReport report = CreateReportCore()) {
				report.SchedulerAdapter.TimeInterval = new TimeInterval(this.startDate, this.endDate);
				report.PrintingSystem.ClearContent();
				report.CreateDocument(true);
				report.Print();
			}
		}
		public void PrintPreview() {
			if (String.IsNullOrEmpty(this.reportFileName))
				return;
			using(XtraSchedulerReport report = CreateReportCore()) {
				report.SchedulerAdapter.TimeInterval = new TimeInterval(this.startDate, this.endDate);
				report.PrintingSystem.ClearContent();
				report.CreateDocument(true);
				report.ShowPreviewDialog();
			}
		}
		XtraSchedulerReport CreateReportCore() {			
			XtraSchedulerReport report = new XtraSchedulerReport();
			report.LoadLayout(reportFileName);
			if(report.SchedulerAdapter != null)
				report.SchedulerAdapter.SetSourceObject(this.printAdapter.SchedulerControl); else
				report.SchedulerAdapter = printAdapter;
			report.SchedulerAdapter.EnableSmartSync = reportFileName.ToLower().Contains("trifold");
			SubscribePrintAdapterEvents(report.SchedulerAdapter);
			report.PrintColorSchema = PrintColorSchema.FullColor;
			return report;
		}
		void SubscribePrintAdapterEvents(SchedulerPrintAdapter adapter) {
			if(adapter != null)
				adapter.ValidateResources += new ResourcesValidationEventHandler(PrintAdapter_ValidateResources);
		}
		void PrintAdapter_ValidateResources(object sender, ResourcesValidationEventArgs e) {
			e.Resources.Clear();
			e.Resources.AddRange(printResources);
		}
	}
}
