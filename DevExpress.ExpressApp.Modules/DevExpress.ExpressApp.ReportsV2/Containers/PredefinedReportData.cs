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
using System.Diagnostics.CodeAnalysis;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.XtraReports.UI;
namespace DevExpress.ExpressApp.ReportsV2 {
	[DomainComponent]
	public class PredefinedReportDataContainer : IReportDataV2, IInplaceReportV2 {
		private string displayName;
		private Type dataType;
		private Type reportType;
		private Type parametersObjectType = null;
		private bool isInplaceReport = false;
		protected PredefinedReportDataContainer() {
		}
		public PredefinedReportDataContainer(Type reportType, string displayName)
			: this(reportType, displayName, null) {
		}
		public PredefinedReportDataContainer(Type reportType, string displayName, Type dataType)
			: this(reportType, displayName, dataType, null) {
		}
		public PredefinedReportDataContainer(Type reportType, string displayName, Type dataType, Type parametersObjectType)
			: this(reportType, displayName, dataType, parametersObjectType, false) {
		}
		public PredefinedReportDataContainer(Type reportType, string displayName, Type dataType, bool isInplaceReport)
			: this(reportType, displayName, dataType, null, isInplaceReport) {
		}
		public PredefinedReportDataContainer(Type reportType, string displayName, Type dataType, Type parametersObjectType, bool isInplaceReport)
			: this() {
			Guard.TypeArgumentIs(typeof(XtraReport), reportType, "reportType");
			if(parametersObjectType != null) {
				Guard.TypeArgumentIs(typeof(ReportParametersObjectBase), parametersObjectType, "parametersObjectType");
			}
			this.reportType = reportType;
			this.displayName = displayName;
			this.dataType = dataType;
			this.parametersObjectType = parametersObjectType;
			this.isInplaceReport = isInplaceReport;
		}
		public string DisplayName {
			get { return displayName; }
			set { displayName = value; }
		}
		public Type ParametersObjectType {
			get { return parametersObjectType; }
			set { parametersObjectType = value; }
		}
		public Type DataType {
			get { return dataType; }
			set { dataType = value; }
		}
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters")]
		public Type PredefinedReportType {
			get { return reportType; }
			set { throw new NotImplementedException(); }
		}
		#region IInplaceReportV2 Members
		public bool IsInplaceReport {
			get { return isInplaceReport; }
			set { throw new NotImplementedException(); }
		}
		[Browsable(false)]
		public string DataTypeName {
			get { return dataType != null ? dataType.FullName : null; }
		}
		#endregion
		#region IReportDataV2 Members
		bool IReportDataV2.IsPredefined {
			get { return true; }
		}
		byte[] IReportDataV2.Content {
			get { throw new NotImplementedException(); }
		}
		#endregion
	}
}
