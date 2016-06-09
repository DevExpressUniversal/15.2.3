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
using DevExpress.Persistent.Base.General;
using DevExpress.Persistent.Validation;
namespace DevExpress.ExpressApp.Reports.Win {
	public enum ReportType { 
		Standard = 0,
		Label = 1
	}
	public interface INewXafReportWizardParameters {
		string ReportName { get; }
		Type DataType { get; }
		ReportType ReportType { get; }
		XafReport Report { get; }
		IReportData ReportData { get; }
		void AssignTo(IReportData reportData);
	}
	[DomainComponent]
	public class NewXafReportWizardParameters : INewXafReportWizardParameters {
		private ReportType reportType = ReportType.Standard;
		private XafReport report;
		public NewXafReportWizardParameters(XafReport report) {
			Guard.ArgumentNotNull(report, "report");
			this.report = report;
		}
		[Browsable(false)]
		public XafReport Report {
			get { return report; }
		}
		public ReportType ReportType {
			get { return reportType; }
			set { reportType = value; }
		}
		[RuleRequiredField("ReportNameIsRequired", "Accept")]
		public string ReportName {
			get { return report.ReportName; }
			set { report.ReportName = value; }
		}
		[TypeConverter(typeof(ReportDataTypeConverter))]
		[RuleRequiredField("DataTypeIsRequired", "Accept")]
		public Type DataType {
			get { return report.DataType; }
			set { report.DataType = value; }
		}
		public virtual void AssignTo(IReportData reportData) { }
		#region Obsolete 11.1
		[Browsable(false)]
		[Obsolete("Use overriden ReportData.SaveXtraReport() instead.", true)]
		public IReportData ReportData {
			get { return null; }
		}
		#endregion
	}
}
