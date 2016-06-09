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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;
using System.Web.UI;
namespace DevExpress.Web.ASPxTreeList.Design {
	public class TreeListColumnTypeConverter : TypeConverter {
		const string UndefindedValue = "(None)";
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			ASPxTreeList treeList = GetTreeList(context);
			return new StandardValuesCollection(CreateValues(treeList));
		}
		private static List<string> CreateValues(ASPxTreeList treeList) {
			List<string> result = new List<string>();
			result.Add("");
			if(treeList == null)
				return result;
			foreach(TreeListColumn column in treeList.Columns) {
				if(column is TreeListDataColumn)
					result.Add(!string.IsNullOrEmpty(column.Name) ? column.Name : column.GetCaption());
			}
			return result;
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return sourceType == typeof(string);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
			if(value.ToString() == UndefindedValue)
				return String.Empty;
			return value;
		}
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
			if(value.ToString() == String.Empty)
				return UndefindedValue;
			return value;
		}
		ASPxTreeList GetTreeList(ITypeDescriptorContext context) {
			if(context == null || context.Instance == null)
				return null;
			TreeListSummaryItem summaryItem = context.Instance as TreeListSummaryItem;
			if(summaryItem != null)
				return summaryItem.TreeList;
			return null;
		}
	}
}
namespace DevExpress.Web.ASPxTreeList.Export.Design {
	public class ASPxTreeListIDConverter : TypeConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return (sourceType == typeof(string));
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			if(value == null) {
				return string.Empty;
			}
			if(value.GetType() != typeof(string)) {
				throw base.GetConvertFromException(value);
			}
			return (string)value;
		}
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			string[] values = null;
			if(context != null) {
				IDesignerHost service = (IDesignerHost)context.GetService(typeof(IDesignerHost));
				if(service != null) {
					IComponent rootComponent = service.RootComponent;
					if(rootComponent != null) {
						List<string> list = new List<string>();
						foreach(IComponent component in rootComponent.Site.Container.Components) {
							Control treeList = component as Control;
							if(((treeList != null) && !string.IsNullOrEmpty(treeList.ID)) && treeList.GetType().Name == "ASPxTreeList")
								list.Add(treeList.ID);
						}
						list.Sort(StringComparer.OrdinalIgnoreCase);
						list.Insert(0, string.Empty);
						values = list.ToArray();
					}
				}
			}
			return new TypeConverter.StandardValuesCollection(values);
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return false;
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
	}
}
