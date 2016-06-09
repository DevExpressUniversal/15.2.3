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
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using DevExpress.XtraReports.UI;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Reports;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
namespace DevExpress.Persistent.BaseImpl.EF {
	[DefaultProperty("ReportName")]
	public class ReportData : IReportData, IXtraReportData, IInplaceReport {
		[Browsable(false)]
		public Int32 ID { get; protected set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Byte[] Content { get; set; }
		[Browsable(false)]
		public String DataTypeName { get; set; }
		[VisibleInListView(false)]
		public Boolean IsInplaceReport { get; set; }
		public String ReportName { get; set; }
		[NotMapped, Browsable(false)]
		public Type DataType {
			get {
				Type result = null;
				if(!String.IsNullOrWhiteSpace(DataTypeName)) {
					ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(DataTypeName);
					if(typeInfo != null) {
						result = typeInfo.Type;
					}
				}
				return result;
			}
			set {
				if(value != null) {
					DataTypeName = value.FullName;
				}
				else {
					DataTypeName = "";
				}
			}
		}
		[NotMapped, DisplayName("Data Type"), Calculated("DataTypeName")]
		public String DataTypeCaption {
			get { return CaptionHelper.GetClassCaption(DataTypeName); }
		}
		public XtraReport LoadReport(IObjectSpace objectSpace) {
			XafReport result = new XafReport();
			result.ObjectSpace = objectSpace;
			XafReportSerializationHelper.LoadReport(this, result);
			return result;
		}
		public void SaveReport(XtraReport report) {
			XafReport xafReport = report as XafReport;
			if(xafReport == null) {
				throw new ArgumentException("XafReport is expected", "report");
			}
			XafReportSerializationHelper.SaveReport(this, xafReport);
		}
	}
}
