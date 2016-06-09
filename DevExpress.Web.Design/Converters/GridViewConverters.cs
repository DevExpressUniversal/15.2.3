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
using System.Web.UI.Design;
using System.Windows.Forms;
using DevExpress.Utils.Design;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Design {
	public class GridViewColumnEditPropertiesConverter : ExpandableObjectConverter {
		internal const string DefaultName = "Default";
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if(sourceType.Equals(typeof(string))) return true;
			return base.CanConvertFrom(context, sourceType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
			if(value is string) {
				if(value.ToString() == DefaultName) return null;
				return EditRegistrationInfo.CreateProperties(value.ToString());
			}
			return value;
		}
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
			if(destinationType.Equals(typeof(string))) {
				if(value is string) return value.ToString();
				if(value == null) return DefaultName;
				return EditRegistrationInfo.GetEditName(value as EditPropertiesBase);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	public class GridViewColumnsConverter : StringListConverterBase {
		protected override void FillList(ITypeDescriptorContext context, List<string> list) {
			var grid = GridViewFieldConverterHelper.GetGrid(context);
			if(grid != null) {
				foreach(var column in grid.ColumnHelper.AllDataColumns)
					list.Add(!string.IsNullOrEmpty(column.Name) ? column.Name : column.ToString());
			}
		}
	}
	public class GridViewFieldConverter : StringListConverterBase {
		protected override void FillList(ITypeDescriptorContext context, List<string> list) {
			var schema = GridViewFieldConverterHelper.GetDataSourceSchema(context);
			if(schema != null) {
				var fields = schema.GetFields();
				foreach(var field in fields)
					list.Add(field.Name);
			}
		}
	}
	internal static class GridViewFieldConverterHelper {
		public static IDataSourceViewSchema GetDataSourceSchema(ITypeDescriptorContext context) {
			var grid = GetGrid(context);
			if(grid == null)
				return null;
			if(IsGridViewBelongLookup(grid)) {
				var designer = GetControlDesigner<ASPxLookupDesigner>((grid as GridViewWrapper).GridLookup);
				if(designer != null)
					return designer.Helper.DataSourceSchema;
			}
			else {
				var designer = GetControlDesigner<GridDesignerBase>(grid);
				if(designer != null)
					return designer.BaseHelper.DataSourceSchema;
			}
			return null;
		}
		public static bool IsGridViewBelongLookup(ASPxGridBase grid) {
			return grid is GridViewWrapper;
		}
		public static ASPxGridBase GetGrid(ITypeDescriptorContext context) {
			if(context == null) return null;
			var instance = context.Instance;
			var grid = ConverterHelper.DiscoverObjectInstance<ASPxGridBase>(instance, GetGridFromInstance);
			return grid != null ? grid : GetGridFromInstance(instance);
		}
		static ASPxGridBase GetGridFromInstance(object instance) {
			if(instance is ASPxGridBase)
				return (ASPxGridBase)instance;
			var grid = GridViewFieldConverterHelper.GetGridByContextInstance(instance);
			if(grid != null)
				return grid;
			var collectionItem = instance as CollectionItem;
			if(collectionItem == null && instance is IDXObjectWrapper)
				collectionItem = ((IDXObjectWrapper)instance).SourceObject as CollectionItem;
			if(collectionItem != null && collectionItem.Collection != null)
				return (ASPxGridBase)collectionItem.Collection.Owner;
			return null;
		}
		public static ASPxGridBase GetGridByContextInstance(object instance) {
			if(instance is IDesignTimePropertiesOwner)
				instance = ((IDesignTimePropertiesOwner)instance).Owner;
			if(instance is ASPxGridBase)
				return (ASPxGridBase)instance;
			if(instance is ASPxGridLookup)
				return ((ASPxGridLookup)instance).GridView;
			return (ASPxGridBase)GetObjectByObjectWrapperHierarchy(instance as IDXObjectWrapper, typeof(ASPxGridBase));
		}
		public static object GetObjectByObjectWrapperHierarchy(IDXObjectWrapper objectWrapper, Type validateType) {
			if(objectWrapper == null) return null;
			var sourceObject = objectWrapper.SourceObject;
			if(sourceObject == null) return null;
			return validateType.IsAssignableFrom(sourceObject.GetType()) ? sourceObject : GetObjectByObjectWrapperHierarchy(sourceObject as IDXObjectWrapper, validateType);
		}
		static T GetControlDesigner<T>(ASPxWebControl control) where T : ASPxWebControlDesigner {
			if(control == null || control.Site == null)
				return null;
			var host = control.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;
			return host != null ? host.GetDesigner(control) as T : null;
		}
	}
	public class GridViewIDConverter : StringListConverterBase {
		protected override void FillList(ITypeDescriptorContext context, List<string> list) {
			var service = (IDesignerHost)context.GetService(typeof(IDesignerHost));
			if(service == null || service.Container == null)
				return;
			foreach(var component in service.Container.Components) {
				var grid = component as ASPxGridBase;
				if(grid != null && !string.IsNullOrEmpty(grid.ID))
					list.Add(grid.ID);
			}
		}
	}
	public class GridDataSourceIDConverter : DXDataSourceIDConverter<ASPxGridBase> { }
	public sealed class LookupTextFormatStringUIEditor : DropDownUITypeEditorBase {
		protected override void ApplySelectedValue(ListBox valueList, ITypeDescriptorContext context) {
			GridLookupProperties props = GetLookupProperties(context);
			props.TextFormatString = valueList.SelectedItem.ToString();
		}
		protected override void FillValueList(ListBox valueList, ITypeDescriptorContext context) {
			GridLookupProperties props = GetLookupProperties(context);
			valueList.Items.Add(props.DefaultTextFormatString);
		}
		protected override PropertyDescriptor GetChangedPropertyDescriptor(object component) {
			return null;
		}
		protected override object GetComponent(ITypeDescriptorContext context) {
			return GetLookupProperties(context).GridLookup;
		}
		protected override void SetInitiallySelectedValue(ListBox valueList, ITypeDescriptorContext context) {
			GridLookupProperties props = GetLookupProperties(context);
			valueList.SelectedItem = props.TextFormatString;
		}
		GridLookupProperties GetLookupProperties(ITypeDescriptorContext context) {
			ASPxGridLookup lookup = context.Instance as ASPxGridLookup;
			if(lookup != null)
				return lookup.Properties;
			else {
				GridLookupProperties props = context.Instance as GridLookupProperties;
				if(props != null)
					return props;
				else
					throw new ArgumentException("Unable to extract the GridLookupProperties from the type descriptor context.");
			}
		}
	}
}
