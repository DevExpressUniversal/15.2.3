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
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.ReportsV2;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.XtraReports.UI;
namespace DevExpress.Persistent.BaseImpl {
	[ObjectValidatorIgnoreIssue(typeof(ObjectValidatorCollectionAlreadyLoaded))]
	public class ReportDataV2 : BaseObject, IReportDataV2Writable, IInplaceReportV2 {
		private string displayName = "";
		private bool isInplaceReport = false;
#if MediumTrust
		private string dataTypeName = string.Empty;
#else
		[Persistent("ObjectTypeName"), Size(512)]
		private string dataTypeName = string.Empty;
#endif
		private string parametersObjectTypeName = string.Empty;
		private Type predefinedReportType;
		[Size(SizeAttribute.Unlimited), Delayed(true), Persistent("Content"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public byte[] Content {
			get {
				return GetDelayedPropertyValue<byte[]>("Content");
			}
			set {
				if(((IReportDataV2)this).IsPredefined) {
					throw new NotImplementedException();
				}
				SetDelayedPropertyValue<byte[]>("Content", value);
			}
		}
		protected override void OnSaving() {
			if(String.IsNullOrEmpty(displayName) || (displayName.Trim() == "")) {
				throw new Exception(ReportsModuleV2.GetEmptyDisplayNameErrorMessage());
			}
			base.OnSaving();
		}
		public ReportDataV2(Session session) : base(session) { }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ReportDataV2(Session session, Type dataType)
			: base(session) {
			Guard.ArgumentNotNull(dataType, "dataType");
			this.dataTypeName = dataType.FullName;
		}
		[Persistent("Name")]
		public string DisplayName {
			get { return displayName; }
			set { SetPropertyValue("DisplayName", ref displayName, value); }
		}
#if MediumTrust
		[Browsable(false)]
		[Persistent("ObjectTypeName")]
		public string DataTypeName {
			get { return dataTypeName; }
			set { SetPropertyValue("ObjectTypeName", ref dataTypeName, value); }
		}
#else
		[Browsable(false)]
		[PersistentAlias("dataTypeName")]
		public string DataTypeName {
			get { return dataTypeName; }
		}
#endif
		[SettingsBindable(true)]
		[VisibleInListView(false)]
		[TypeConverter(typeof(ReportParametersObjectTypeConverter))]
		[Localizable(true)]
		public Type ParametersObjectType {
			get {
				if(!string.IsNullOrEmpty(ParametersObjectTypeName)) {
					ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(ParametersObjectTypeName);
					if(typeInfo != null) {
						return typeInfo.Type;
					}
				}
				return null;
			}
			set {
				((IReportDataV2Writable)this).SetParametersObjectType(value);
			}
		}
		[Size(512)]
		[Browsable(false)]
		public string ParametersObjectTypeName {
			get { return parametersObjectTypeName; }
			set { SetPropertyValue(ReportsModuleV2.ParametersObjectTypeNameMemberName, ref parametersObjectTypeName, value); }
		}
		[NonPersistent, System.ComponentModel.DisplayName("Data Type")]
		public string DataTypeCaption {
			get { return CaptionHelper.GetClassCaption(dataTypeName); }
		}
		Type IReportDataV2.DataType {
			get {
				if(!string.IsNullOrEmpty(DataTypeName)) {
					ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(DataTypeName);
					if(typeInfo != null) {
						return typeInfo.Type;
					}
				}
				return null;
			}
		}
		[VisibleInListView(false)]
		public bool IsInplaceReport {
			get { return isInplaceReport; }
			set { SetPropertyValue(ReportsModuleV2.IsInplaceReportMemberName, ref isInplaceReport, value); }
		}
		[Browsable(false)]
		[ValueConverter(typeof(TypeToStringConverter))]
		[Size(512)]
		public Type PredefinedReportType {
			get { return predefinedReportType; }
			set { SetPropertyValue(ReportsModuleV2.PredefinedReportTypeMemberName, ref predefinedReportType, value); }
		}
		void IReportDataV2Writable.SetContent(byte[] content) {
			Content = content;
		}
		void IReportDataV2Writable.SetPredefinedReportType(Type reportType) {
			if(reportType != null) {
				Guard.TypeArgumentIs(typeof(XtraReport), reportType, "reportType");
			}
			PredefinedReportType = reportType;
		}
		void IReportDataV2Writable.SetParametersObjectType(Type parametersObjectType) {
			if(parametersObjectType != null) {
				Guard.TypeArgumentIs(typeof(ReportParametersObjectBase), parametersObjectType, "parametersObjectType");
			}
			ParametersObjectTypeName = parametersObjectType != null ? parametersObjectType.FullName : string.Empty;
		}
		void IReportDataV2Writable.SetDataType(Type newDataType) {
			dataTypeName = newDataType != null ? newDataType.FullName : string.Empty;
		}
		void IReportDataV2Writable.SetDisplayName(string displayName) {
			DisplayName = displayName;
		}
		[VisibleInListView(false)]
		[VisibleInDetailView(false)]
		[NonPersistent]
		public bool IsPredefined {
			get { return PredefinedReportType != null; }
		}
	}
}
