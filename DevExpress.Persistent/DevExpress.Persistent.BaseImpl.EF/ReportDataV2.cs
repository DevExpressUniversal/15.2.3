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
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.ReportsV2;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.XtraReports.UI;
namespace DevExpress.Persistent.BaseImpl.EF {
	public class ReportParametersObjectTypeConverter : LocalizedClassInfoTypeConverter {
		public override List<Type> GetSourceCollection(ITypeDescriptorContext context) {
			ITypeInfo reportParametersObjectBaseTypeInfo = XafTypesInfo.Instance.FindTypeInfo(typeof(ReportParametersObjectBase));
			Guard.ArgumentNotNull(reportParametersObjectBaseTypeInfo, "reportParametersObjectBaseTypeInfo");
			return new List<Type>(Enumerator.Convert<ITypeInfo, Type>(reportParametersObjectBaseTypeInfo.Descendants, XafTypesInfo.CastTypeInfoToType));
		}
	}
	[PredefinedReportTypeMemberName("PredefinedReportTypeName")]
	public class ReportDataV2 : IReportDataV2Writable, IInplaceReportV2, IXafEntityObject {
		private Byte[] content;
		[Browsable(false)]
		public Int32 ID { get; protected set; }
		[Browsable(false)]
		[FieldSize(512)]
		public String DataTypeName { get; set; }
		[VisibleInListView(false)]
		public Boolean IsInplaceReport { get; set; }
		[Browsable(false)]
		[FieldSize(512)]
		public String PredefinedReportTypeName { get; set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), System.ComponentModel.DataAnnotations.MaxLength]
		public byte[] Content {
			get {
				return content;
			}
			set {
				if(!((IReportDataV2)this).IsPredefined) {
					content = value;
				}
			}
		}
		public string DisplayName { get; set; }
		public ReportDataV2() { }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ReportDataV2(Type dataType) {
			Guard.ArgumentNotNull(dataType, "dataType");
			DataTypeName = dataType.FullName;
		}
		[SettingsBindable(true)]
		[VisibleInListView(false)]
		[TypeConverter(typeof(ReportParametersObjectTypeConverter))]
		[Localizable(true)]
		[NotMapped]
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
		[Browsable(false)]
		[FieldSize(512)]
		public string ParametersObjectTypeName { get; set; }
		[System.ComponentModel.DisplayName("Data Type")]
		[NotMapped]
		public string DataTypeCaption {
			get { return CaptionHelper.GetClassCaption(DataTypeName); }
		}
		[NotMapped]
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
		[Browsable(false)]
		[NotMapped]
		public Type PredefinedReportType {
			get {
				if(!string.IsNullOrEmpty(PredefinedReportTypeName)) {
					return ReflectionHelper.FindType(PredefinedReportTypeName);
				}
				return null;
			}
			set {
				PredefinedReportTypeName = value != null ? value.FullName : null;
			}
		}
		[VisibleInListView(false)]
		[VisibleInDetailView(false)]
		[NotMapped]
		public bool IsPredefined {
			get { return PredefinedReportType != null; }
		}
#region IReportDataV2Writable
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
			DataTypeName = newDataType != null ? newDataType.FullName : string.Empty;
		}
		void IReportDataV2Writable.SetDisplayName(string displayName) {
			DisplayName = displayName;
		}
#endregion
#region IXafEntityObject
		void IXafEntityObject.OnCreated() {
		}
		void IXafEntityObject.OnLoaded() {
		}
		void IXafEntityObject.OnSaving() {
			if(String.IsNullOrEmpty(DisplayName) || (DisplayName.Trim() == "")) {
				throw new Exception(ReportsModuleV2.GetEmptyDisplayNameErrorMessage());
			}
		}
#endregion
	}
}
