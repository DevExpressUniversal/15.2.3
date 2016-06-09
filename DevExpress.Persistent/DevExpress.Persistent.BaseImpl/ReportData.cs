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
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Reports;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Xpo;
using DevExpress.XtraReports.UI;
namespace DevExpress.Persistent.BaseImpl {
	[ObjectValidatorIgnoreIssue(typeof(ObjectValidatorCollectionAlreadyLoaded))]
	public class ReportData : XPObject, IReportData, IInplaceReport, IXtraReportData {
		private string reportName = "";
		private bool isInplaceReport = false;
#if MediumTrust
		private string dataTypeName = string.Empty;
#else
		[Persistent("ObjectTypeName"), Size(512)]
		private string dataTypeName = string.Empty;
#endif
		[Delayed(true), Persistent("Content"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public byte[] Content {
			get { return GetDelayedPropertyValue<byte[]>("Content"); }
			set { SetDelayedPropertyValue<byte[]>("Content", value); }
		}
		protected virtual XafReport CreateReport() {
			return new XafReport();
		}
		protected override void OnSaving() {
			if(String.IsNullOrEmpty(reportName) || (reportName.Trim() == "")) {
				throw new Exception(ReportsModule.EmptyReportNameErrorMessage);
			}
			base.OnSaving();
		}
		public ReportData(Session session) : base(session) { }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ReportData(Session session, Type dataType)
			: base(session) {
			Guard.ArgumentNotNull(dataType, "dataType");
			this.dataTypeName = dataType.FullName;
		}
		public virtual XtraReport LoadReport(IObjectSpace objectSpace) {
			XafReport result = CreateReport();
			result.ObjectSpace = objectSpace;
			result.CustomDeserializeValue += new EventHandler<CustomDeserializeValueEventArgs>(result_CustomDeserializeValue);
			XafReportSerializationHelper.LoadReport(this, result);  
			return result;
		}
		void result_CustomDeserializeValue(object sender, CustomDeserializeValueEventArgs e) {
			OnCustomDeserializeReportValue((XafReport)sender, e);
		}
		protected virtual void OnCustomDeserializeReportValue(XafReport report, CustomDeserializeValueEventArgs e) {
			if(CustomDeserializeReportValue != null) {
				CustomDeserializeReportValue(report, e);
			}
		}
		public virtual void SaveReport(XtraReport report) {
			XafReport xafReport = report as XafReport;
			if(xafReport == null) {
				throw new ArgumentException("XafReport is expected", "report");
			}
			XafReportSerializationHelper.SaveReport(this, xafReport);  
		}
		[Persistent("Name")]
		public string ReportName {
			get { return reportName; }
			set { SetPropertyValue("ReportName", ref reportName, value); }
		}
#if MediumTrust
		[Browsable(false)]
		[Persistent("ObjectTypeName")]
		public string DataTypeName {
			get { return dataTypeName; }
			set { SetPropertyValue("ObjectTypeName", ref dataTypeName, value); }
		}
#else
		[Browsable(false), PersistentAlias("dataTypeName")]
		public string DataTypeName {
			get { return dataTypeName; }
		}
#endif
		[NonPersistent, System.ComponentModel.DisplayName("Data Type")]
		public string DataTypeCaption {
			get { return CaptionHelper.GetClassCaption(dataTypeName); }
		}
		[VisibleInListView(false)]
		public bool IsInplaceReport {
			get { return isInplaceReport; }
			set { SetPropertyValue(ReportsModule.IsInplaceReportMemberName, ref isInplaceReport, value); }
		}
		public static void MassUpdateDataType(IObjectSpace objectSpace, string oldDataType, Type newDataType) {
			foreach(ReportData reportData in objectSpace.GetObjects<ReportData>(new BinaryOperator("dataTypeName", oldDataType))) {
				XafReport report = (XafReport)reportData.LoadReport(objectSpace);
				report.DataType = newDataType;
				reportData.SaveReport(report);
			}
			objectSpace.CommitChanges();
		}
		public event EventHandler<CustomDeserializeValueEventArgs> CustomDeserializeReportValue;
		#region IXtraReportData Members
		Type IXtraReportData.DataType {
			get {
				if(!string.IsNullOrEmpty(DataTypeName)) {
					ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(DataTypeName);
					if(typeInfo != null) {
						return typeInfo.Type;
					}
				}
				return null;
			}
			set { dataTypeName = value != null ? value.FullName : string.Empty; }
		}
		#endregion
		#region Obsolete, do not delete, used in criteria strings in existing client applications
		[Obsolete("Use the DataTypeCaption property instead", true), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string DataType {
			get { return DataTypeCaption; }
		}
		[Obsolete("Use the ReportName property instead", true), Browsable(false), NonPersistent, EditorBrowsable(EditorBrowsableState.Never)]
		public string Name {
			get { return ReportName; }
			set { ReportName = value; }
		}
		#endregion
	}
}
