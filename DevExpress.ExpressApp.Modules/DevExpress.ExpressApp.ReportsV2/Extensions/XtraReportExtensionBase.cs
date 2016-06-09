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
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.ReportsV2;
using DevExpress.XtraReports.Extensions;
using DevExpress.XtraReports.UI;
namespace DevExpress.ExpressApp.ReportsV2 {
	public class XtraReportExtensionBase : ReportDesignExtension {
		public XtraReportExtensionBase() {
		}
		public static XafApplication Application {
			get { return ValueManager.GetValueManager<XafApplication>("XtraReportExtension_XafApplication").Value; }
			set { ValueManager.GetValueManager<XafApplication>("XtraReportExtension_XafApplication").Value = value; }
		}
		protected override bool CanSerialize(object data) {
			if(data != null) {
				ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(data.GetType());
				return typeInfo != null && typeInfo.IsPersistent;
			}
			return base.CanSerialize(data);
		}
		protected override string SerializeData(object data, XtraReport report) {
			if(report != null) {
				IObjectSpace objectSpace = DataSourceBase.CreateObjectSpace(data.GetType(), report);
				if(objectSpace != null) {
					object targetObj = objectSpace.GetObject(data);
					return objectSpace.GetObjectHandle(targetObj);
				}
			}
			return base.SerializeData(data, report);
		}
		protected override bool CanDeserialize(string value, string typeName) {
			if(typeName == "System.Type") {
				return true;
			}
			Type objectType = null;
			string keyString;
			if(ObjectHandleHelper.TryParseObjectHandle(XafTypesInfo.Instance, value, out objectType, out keyString)) {
				ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(objectType);
				if(typeInfo != null) {
					return typeInfo.IsPersistent;
				}
			}
			return base.CanDeserialize(value, typeName);
		}
		protected override object DeserializeData(string value, string typeName, XtraReport report) {
			if(typeName == "System.Type") {
				ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(value);
				if(typeInfo != null) {
					Type type = typeInfo.Type;
					return type;
				}
			}
			if(report != null) {
				ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(typeName);
				if(typeInfo != null) {
					IObjectSpace objectSpace = DataSourceBase.CreateObjectSpace(typeInfo.Type, report);
					if(objectSpace != null) {
						return objectSpace.GetObjectByHandle(value);
					}
				}
			}
			return base.DeserializeData(value, typeName, report);
		}
		public override Type[] GetEditableDataTypes() {
			if(GetCustomEditableDataTypes != null) {
				GetCustomEditableDataTypesEventArgs args = new GetCustomEditableDataTypesEventArgs(Application);
				GetCustomEditableDataTypes(this, args);
				return args.Types;
			}
			else {
				return base.GetEditableDataTypes();
			}
		}
		public override void AddParameterTypes(IDictionary<Type, string> dictionary) {
			if(CustomAddParameterTypes != null) {
				CustomAddParameterTypes(this, new AddCustomParameterTypesEventArgs(Application, dictionary));
			}
			else {
				base.AddParameterTypes(dictionary);
			}
		}
		public event EventHandler<AddCustomParameterTypesEventArgs> CustomAddParameterTypes;
		public event EventHandler<GetCustomEditableDataTypesEventArgs> GetCustomEditableDataTypes;
	}
}
