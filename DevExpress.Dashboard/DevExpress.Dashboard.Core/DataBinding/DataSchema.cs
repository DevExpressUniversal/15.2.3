#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Diagnostics.CodeAnalysis;
using DevExpress.DataAccess.Native.Data;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DashboardCommon.DB {
#if DEBUG
	[SuppressMessage("Microsoft.Design", "CA1039")]
#endif
	public class DataSchema : ReadOnlyTypedList {
		public PropertiesRepository TableDescriptors { get { return Properties; } }
		public SchemaTableTypeDescriptor AddTable(string name, string alias) {
			SchemaTableTypeDescriptor schemaTable = new SchemaTableTypeDescriptor(name, alias);
			TableDescriptors.Add(new SchemaTablePropertyDescriptor(schemaTable));
			return schemaTable;
		}
		protected override PropertyDescriptorCollection GetItemPropertiesByListAccessors(PropertyDescriptor[] listAccessors) {
			return listAccessors.Length == 1 ? ((SchemaTablePropertyDescriptor)listAccessors[0]).SchemaTable.ColumnDescriptors : new PropertyDescriptorCollection(null);
		}
		protected override object GetItemValue(int index) {
			return null;
		}
		protected override int GetItemsCount() {
			return 0;
		}
	}
	public class SchemaTablePropertyDescriptor : ReadOnlyPropertyDescriptor {
		readonly SchemaTableTypeDescriptor schemaTable;
		public SchemaTableTypeDescriptor SchemaTable { get { return schemaTable; } }
		public override Type PropertyType { get { return typeof(SchemaTableTypeDescriptor); } }
		public override string DisplayName { get { return schemaTable.Alias; } }
		public SchemaTablePropertyDescriptor(SchemaTableTypeDescriptor schemaTable)
			: base(schemaTable.Name) {
			this.schemaTable = schemaTable;
		}
		public override object GetValue(object component) {
			return schemaTable;
		}
	}
	public class SchemaColumnPropertyDescriptor : TypedPropertyDescriptor {
		readonly string alias;
		readonly bool includeTableNameInDataMember;
		public override string DisplayName { get { return alias; } }
		public bool IncludeTableNameInDataMember { get { return includeTableNameInDataMember; } }
		public SchemaColumnPropertyDescriptor(string name, Type type, string alias, bool includeTableNameInDataMember)
			: base(name, type) {
			this.alias = alias;
			this.includeTableNameInDataMember = includeTableNameInDataMember;
		}
		public override object GetValue(object component) {
			return null;
		}
	}
	public class SchemaTableTypeDescriptor : ICustomTypeDescriptor {
		readonly PropertyDescriptorCollection columnDescriptors = new PropertyDescriptorCollection(null);
		readonly string name;
		readonly string alias;
		string TypeName { get { return GetType().Name; } }
		public PropertyDescriptorCollection ColumnDescriptors { get { return columnDescriptors; } }
		public string Name { get { return name; } }
		public string Alias { get { return alias; } }
		public SchemaTableTypeDescriptor(string name, string alias) {
			this.name = name;
			this.alias = alias;
		}
		public void AddColumn(string name, Type type, string alias, bool includeTableNameInSchema) {
			columnDescriptors.Add(new SchemaColumnPropertyDescriptor(name, type, alias, includeTableNameInSchema));
		}	
		public void AddColumn(string name, Type type, string alias) {
			AddColumn(name, type, alias, true);
		}
		#region ICustomTypeDescriptor members
		AttributeCollection ICustomTypeDescriptor.GetAttributes() {
#if !DXPORTABLE
			return TypeDescriptor.GetAttributes(this, true);
#else
			return TypeDescriptor.GetAttributes(this);
#endif
		}
		string ICustomTypeDescriptor.GetClassName() {
			return TypeName;
		}
		string ICustomTypeDescriptor.GetComponentName() {
			return TypeName;
		}
		TypeConverter ICustomTypeDescriptor.GetConverter() {
#if !DXPORTABLE
			return TypeDescriptor.GetConverter(this, true);
#else
			return TypeDescriptor.GetConverter(this);
#endif
		}
		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() {
#if !DXPORTABLE
			return TypeDescriptor.GetDefaultProperty(this, true);
#else
			return null;
#endif
		}
		object ICustomTypeDescriptor.GetEditor(Type editorBaseType) {
#if !DXPORTABLE
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
#else
			return TypeDescriptor.GetEditor(this, editorBaseType);
#endif
		}
#if !DXRESTRICTED
		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() {
			return TypeDescriptor.GetDefaultEvent(this, true);
		}
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents() {
			return TypeDescriptor.GetEvents(this, true);
		}
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes) {
			return TypeDescriptor.GetEvents(this, attributes, true);
		}
#endif
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() {
			return columnDescriptors;
		}
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes) {
			return columnDescriptors;
		}
		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) {
			return this;
		}
#endregion
	}
}
