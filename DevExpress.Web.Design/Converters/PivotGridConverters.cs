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
using System.Web.UI.Design;
using System.Web.UI.Design.WebControls;
namespace DevExpress.Web.ASPxPivotGrid.Design {
	public class FieldNameTypeConverter : DataSourceViewSchemaConverter {
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context, Type typeFilter) {
			if(context.Instance is PivotGridField) {
				ASPxPivotGrid grid = ((PivotGridField)context.Instance).PivotGrid;
				if(grid != null && !String.IsNullOrEmpty(grid.OLAPConnectionString)) {
					grid.Data.OLAPConnectionString = grid.OLAPConnectionString;
					return new StandardValuesCollection(grid.Data.GetFieldList());
				}
			}
			return base.GetStandardValues(context, typeFilter);
		}
	}
	public class PivotGridDataSourceIDConverter : DataSourceIDConverter {
		ASPxPivotGrid pivotGrid;
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			this.pivotGrid = GetPivotGrid(context);
			StandardValuesCollection res = base.GetStandardValues(context);
			this.pivotGrid = null;
			return res;
		}
		protected ASPxPivotGrid GetPivotGrid(ITypeDescriptorContext context) {
			ASPxPivotGrid res = context.Instance as ASPxPivotGrid;
			if(res != null)
				return res;
			PivotGridDataWebControlActionList list = context.Instance as PivotGridDataWebControlActionList;
			if(list != null)
				return list.Component as ASPxPivotGrid;
			return null;
		}
		protected override bool IsValidDataSource(IComponent component) {
			return component != pivotGrid && base.IsValidDataSource(component);
		}
	}
	public class PivotGridIDConverter : TypeConverter {
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
						List<string> gridList = new List<string>();
						foreach(IComponent component in rootComponent.Site.Container.Components) {
							System.Web.UI.Control grid = component as System.Web.UI.Control;
							if(((grid != null) && !string.IsNullOrEmpty(grid.ID)) && grid.GetType().Name == "ASPxPivotGrid") {
								gridList.Add(grid.ID);
							}
						}
						gridList.Sort(StringComparer.OrdinalIgnoreCase);
						gridList.Insert(0, string.Empty);
						values = gridList.ToArray();
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
