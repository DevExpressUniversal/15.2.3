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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Data.Browsing;
using DevExpress.Data.Native;
using System.Windows.Forms.Design;
using System.Web.UI;
using System.Collections.Generic;
using DevExpress.XtraPrinting.Native;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Web.UI.Design;
using DevExpress.Utils.Design;
namespace DevExpress.Data.Browsing.Design {
	#region WebColumnNameEditor
	public class WebColumnNameEditor : ColumnNameEditor {
		protected override object GetDataSource(ITypeDescriptorContext context) {
			if(context == null) return null;
			IDataSourceViewSchemaAccessor instance = context.Instance as IDataSourceViewSchemaAccessor;
			if(instance == null) { 
				IDXObjectWrapper wrapper = context.Instance as IDXObjectWrapper;
				if(wrapper != null)
					instance = wrapper.SourceObject as IDataSourceViewSchemaAccessor;
				if(instance == null)
					return null;
			}
			IDataSourceViewSchema dataSourceViewSchema = instance.DataSourceViewSchema as IDataSourceViewSchema;
			if(dataSourceViewSchema != null)
				return new DataSourceViewSchemaWrapper(dataSourceViewSchema, context.Instance as IDisplayNameProvider);
			return instance.DataSourceViewSchema;
		}
		protected override ColumnNameEditorPicker CreatePicker(object dataSource, string columnName, IServiceProvider serviceProvider) {
			return new WebColumnNameEditorPicker(dataSource, columnName, serviceProvider);
		}
	}
	public class WebColumnNameEditorPicker : ColumnNameEditorPicker {
		public WebColumnNameEditorPicker(object dataSource, string columnName, IServiceProvider serviceProvider)
			: base(dataSource, columnName, serviceProvider) { }
		protected override PickManager CreatePickManager() {
			return new WebColumnNameEditorPickManager();
		}
	}
	class WebColumnNameEditorPickManager : ColumnNameEditorPickManager {
		class WebColumnNameProvider : ColumnNameProvider {
			public override void GetListItemProperties(object dataSource, string dataMember, EventHandler<GetPropertiesEventArgs> action) {
				if(string.IsNullOrEmpty(dataMember))
					base.GetListItemProperties(dataSource, dataMember, action);
				else 
					action(this, CreatePropertiesEventArgs(GetChildProperties(dataSource, dataMember)));
			}
			protected IPropertyDescriptor[] GetChildProperties(object dataSource, string dataMember) {
				string[] dataMemberParts = dataMember.Split('.');
				DataBrowser browser = DataContext.GetDataBrowser(dataSource, dataMemberParts[0], true);
				Type type = browser.DataSourceType;
				PropertyDescriptorCollection properties = GetProperties(type, browser.GetValue());
				for(int i = 1; i < dataMemberParts.Length; i++) {
					PropertyDescriptor prop = properties[dataMemberParts[i]];
					if(prop == null)
						return new IPropertyDescriptor[]{};
					type = prop.PropertyType;
					properties = GetProperties(type, prop.GetValue(null));
				}
				List<IPropertyDescriptor> fakedProperties = new List<IPropertyDescriptor>();
				for(int i = 0; i < properties.Count; i++) {
					PropertyDescriptor property = (PropertyDescriptor)properties[i];
					PropertyDescriptorCollection collection = DataContext.GetListItemProperties(dataSource, GetFullName(dataMember, property.Name));
					fakedProperties.Add(new FakedPropertyDescriptor(property, property.DisplayName, collection != null && collection.Count > 0, TypeSpecifics.Default));
				}			   
				PostFilterProperties(fakedProperties);
				IPropertyDescriptor[] result = fakedProperties.ToArray();
				SortProperties(result);
				return result;
			}
			protected virtual PropertyDescriptorCollection GetProperties(Type type, object value) {
				ICustomTypeDescriptor customDescriptor = value as ICustomTypeDescriptor;
				if(customDescriptor != null)
					return customDescriptor.GetProperties();
				if(type == typeof(string))
					return new PropertyDescriptorCollection(null);	
				return TypeDescriptor.GetProperties(type);
			}
		}
		protected override IPropertiesProvider CreateProvider() {
			return new WebColumnNameProvider();
		}
	}
	public class DataSourceViewSchemaWrapper : ICustomTypeDescriptor, IDisplayNameProvider {
		readonly IDataSourceViewSchema schema;
		readonly IDisplayNameProvider displayNameProvider;
		public DataSourceViewSchemaWrapper(IDataSourceViewSchema schema, IDisplayNameProvider displayNameProvider) {
			if(schema == null)
				throw new ArgumentNullException("schema");
			this.schema = schema;
			this.displayNameProvider = displayNameProvider;
		}
		protected IDataSourceViewSchema Schema { get { return schema; } }
		protected IDisplayNameProvider DisplayNameProvider { get { return displayNameProvider; } }
		protected PropertyDescriptorCollection GetProperties() {
			IDataSourceFieldSchema[] fields = Schema.GetFields();
			SchemaWrapperPropertyDescriptor[] props = new SchemaWrapperPropertyDescriptor[fields.Length];
			for(int i = 0; i < fields.Length; i++) {
				props[i] = new SchemaWrapperPropertyDescriptor(fields[i]);
			}
			return new PropertyDescriptorCollection(props);
		}
		#region ICustomTypeDescriptor Members
		System.ComponentModel.AttributeCollection ICustomTypeDescriptor.GetAttributes() {
			return TypeDescriptor.GetAttributes(this);
		}
		string ICustomTypeDescriptor.GetClassName() {
			return TypeDescriptor.GetClassName(this);
		}
		string ICustomTypeDescriptor.GetComponentName() {
			return DisplayNameProvider != null ? DisplayNameProvider.GetDataSourceDisplayName() : string.Empty;
		}
		TypeConverter ICustomTypeDescriptor.GetConverter() {
			return TypeDescriptor.GetConverter(this);
		}
		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() {
			return TypeDescriptor.GetDefaultEvent(this);
		}
		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() {
			return TypeDescriptor.GetDefaultProperty(this);
		}
		object ICustomTypeDescriptor.GetEditor(Type editorBaseType) {
			return TypeDescriptor.GetEditor(this, editorBaseType);
		}
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes) {
			return TypeDescriptor.GetEvents(attributes);
		}
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents() {
			return TypeDescriptor.GetEvents(this);
		}
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes) {
			return GetProperties();
		}
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() {
			return GetProperties();
		}
		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) {
			if(pd is SchemaWrapperPropertyDescriptor)
				return this;
			return null;
		}
		#endregion
		#region IDisplayNameProvider Members
		string IDisplayNameProvider.GetDataSourceDisplayName() {
			return DisplayNameProvider != null ? DisplayNameProvider.GetDataSourceDisplayName() : string.Empty;
		}
		string IDisplayNameProvider.GetFieldDisplayName(string[] fieldAccessors) {
			return DisplayNameProvider != null ? DisplayNameProvider.GetFieldDisplayName(fieldAccessors) : fieldAccessors[fieldAccessors.Length - 1];
		}
		#endregion
	}
	class SchemaWrapperPropertyDescriptor : PropertyDescriptor {
		readonly IDataSourceFieldSchema field;
		public SchemaWrapperPropertyDescriptor(IDataSourceFieldSchema field)
			: base(field.Name, null) {
			this.field = field;
		}
		protected IDataSourceFieldSchema Field { get { return this.field; } }
		public override bool CanResetValue(object component) { return false; }
		public override Type ComponentType { get { return typeof(DataSourceViewSchemaWrapper); } }
		public override object GetValue(object component) { return null; }
		public override bool IsReadOnly { get { return Field.IsReadOnly; } }
		public override Type PropertyType { get { return Field.DataType; } }
		public override void ResetValue(object component) { }
		public override void SetValue(object component, object value) { }
		public override bool ShouldSerializeValue(object component) { return false; }
	}
	#endregion
}
