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
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI.Design;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Design.Converters {
	public class LayoutItemFieldNameConverter : StringListConverterBase {
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return ConverterHelper.DiscoverObjectInstance<LayoutItem>(context.Instance) != null;
		}
		protected override void FillList(ITypeDescriptorContext context, List<string> list) {
			var valuesFromSchema = new DataSourceViewSchemaConverter().GetStandardValues(context);
			if(valuesFromSchema.Count > 0) {
				list.AddRange(valuesFromSchema.OfType<string>());
				return;
			}
			var layoutItem = ConverterHelper.DiscoverObjectInstance<LayoutItem>(context.Instance);
			if(layoutItem != null && layoutItem.FormLayout.DataType != null) {
				var properties = layoutItem.FormLayout.DataType.GetProperties();
				foreach(var property in properties) {
					if(NestedControlHelper.IsAllowedDataType(property.PropertyType))
						list.Add(property.Name);
				}
			}
		}
	}
	public class LayoutItemColumnNameConverter : TypeConverter {
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return ConverterHelper.DiscoverObjectInstance<ColumnLayoutItem>(context.Instance) != null;
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			var item = ConverterHelper.DiscoverObjectInstance<ColumnLayoutItem>(context.Instance);
			var owner = item.Owner as IFormLayoutOwner;
			var columnNames = owner != null ? owner.GetColumnNames() : new string[] { string.Empty };
			return new StandardValuesCollection(columnNames);
		}
	}
}
