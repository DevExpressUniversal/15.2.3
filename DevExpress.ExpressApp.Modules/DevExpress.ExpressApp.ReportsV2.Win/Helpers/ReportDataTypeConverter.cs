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
using System.Globalization;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.ReportsV2;
namespace DevExpress.ExpressApp.ReportsV2.Win {
	public class ReportDataTypeConverterForDesigner : ReportDataTypeConverter {
		public ReportDataTypeConverterForDesigner() : base() { }
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			List<string> result = new List<string>();
			if(!DataSourceBase.IsDesignMode) {
				List<Type> list = GetSourceCollection(context);
				list.Sort(this);
				foreach(Type item in list) {
					result.Add(item.FullName);
				}
			}
			else {
				Guard.ArgumentNotNull(context, "context");
				ITypesInfo typesInfoService = context.GetService(typeof(ITypesInfo)) as ITypesInfo;
				Guard.ArgumentNotNull(typesInfoService, "typesInfoService");
				foreach(ITypeInfo classInfo in typesInfoService.PersistentTypes) {
					if(classInfo.IsVisible && (classInfo.IsPersistent || (classInfo.IsInterface && classInfo.IsDomainComponent))) {
						if(classInfo.Type != null) {
							result.Add(classInfo.Type.FullName);
						}
					}
				}
			}
			result.Insert(0, "");
			return new StandardValuesCollection(result);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object val, Type destType) {
			if(destType == typeof(string)) {
				string fullName = val as string;
				if(!string.IsNullOrEmpty(fullName)) {
					return GetClassCaption(fullName);
				}
				else {
					return CaptionHelper.NoneValue;
				}
			}
			else {
				return base.ConvertTo(context, culture, val, destType);
			}
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object val) {
			if(DataSourceBase.IsDesignMode) {
				return val;
			}
			Guard.ArgumentNotNull(context, "context");
			string result = null;
			if(val != null) {
				string caption = val as string;
				ITypesInfo typesInfoService = context.GetService(typeof(ITypesInfo)) as ITypesInfo;
				foreach(ITypeInfo classInfo in typesInfoService.PersistentTypes) {
					if(classInfo.IsVisible && (GetClassCaption(classInfo.FullName) == caption)) {
						result = classInfo.Type.FullName;
						break;
					}
				}
			}
			return result;
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return false;
		}
	}
}
