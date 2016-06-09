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
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.XtraReports.UI;
namespace DevExpress.ExpressApp.ReportsV2 {
	[DomainComponent]
	public class NewReportWizardParameters : INewReportWizardParameters {
		private ReportType reportType = ReportType.Standard;
		private XtraReport report;
		private Type dataType;
		private string displayName;
		private Type reportDataType;
		public NewReportWizardParameters(XtraReport report, Type reportDataType) {
			Guard.ArgumentNotNull(report, "report");
			this.report = report;
			this.reportDataType = reportDataType;
		}
		[Browsable(false)]
		public XtraReport Report {
			get { return report; }
		}
		[Browsable(false)]
		public Type ReportDataType {
			get { return reportDataType; }
		}
		public ReportType ReportType {
			get { return reportType; }
			set { reportType = value; }
		}
		[RuleRequiredField("DisplayNameIsRequired", "Accept")]
		public string DisplayName {
			get { return displayName; }
			set {
				displayName = value;
				report.DisplayName = displayName;
			}
		}
		[TypeConverter(typeof(ReportDataTypeConverter))]
		[RuleRequiredField("DataTypeIsRequiredV2", "Accept")]
		public Type DataType {
			get { return dataType; }
			set {
				if(dataType != value) {
					dataType = value;
					DataTypeChanged();
				}
			}
		}
		public virtual void AssignData(IReportDataV2Writable reportData) {
			reportData.SetDisplayName(DisplayName);
			if(!string.IsNullOrEmpty(report.DisplayName)) {
				reportData.SetDisplayName(report.DisplayName);
			}
		}
		protected virtual void DataTypeChanged() {
			QueryRootReportDataSourceComponentEventArgs raiseQueryRootReportDataSourceComponentArgs = new QueryRootReportDataSourceComponentEventArgs(Report, DataType);
			ReportServiceController.RaiseQueryRootReportDataSourceComponent(raiseQueryRootReportDataSourceComponentArgs);
			if(!raiseQueryRootReportDataSourceComponentArgs.Handled) {
				raiseQueryRootReportDataSourceComponentArgs.DataSource = raiseQueryRootReportDataSourceComponentArgs.GetDefaultDataSource();
			}
			if(Report.DataSource != null && Report.DataSource is IComponent) {
				Report.ComponentStorage.Remove((IComponent)Report.DataSource);
			}
			Report.DataSource = raiseQueryRootReportDataSourceComponentArgs.DataSource;
			if(Report.DataSource is IComponent) {
				Report.ComponentStorage.Add((IComponent)Report.DataSource);
			}
		}
	}
}
