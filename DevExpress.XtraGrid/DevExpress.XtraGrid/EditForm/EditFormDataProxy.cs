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
using System.Linq;
using System.Text;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
namespace DevExpress.XtraGrid.EditForm.Helpers {
	public abstract class EditFormDataProxyCore : ICustomTypeDescriptor, IDXDataErrorInfo, IDisposable {
		EditFormController owner;
		bool isModified;
		public EditFormDataProxyCore(EditFormController owner) {
			Enabled = true;
			this.owner = owner;
			this.isModified = false;
		}
		protected GridView View { get { return Owner.View; } }
		protected EditFormController Owner { get { return owner; } }
		public bool IsModified { 
			get { return isModified; }
			set {
				if(IsModified == value) return;
				isModified = value;
				if(value) OnModified();
			}
		}
		protected virtual void OnModified() {
			Owner.OnModified();
		}
		public bool Enabled { get; set; }
		public virtual void PushData() { }
		PropertyDescriptorCollection properties;
		#region ICustomTypeDescriptor Members
		AttributeCollection ICustomTypeDescriptor.GetAttributes() { return TypeDescriptor.GetAttributes(this, true); }
		string ICustomTypeDescriptor.GetClassName() { return "DataProxy"; }
		string ICustomTypeDescriptor.GetComponentName() { return "DataProxy"; }
		TypeConverter ICustomTypeDescriptor.GetConverter() { return TypeDescriptor.GetConverter(this, true); }
		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() { return TypeDescriptor.GetDefaultEvent(this, true); }
		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() { return TypeDescriptor.GetDefaultProperty(this, true); }
		object ICustomTypeDescriptor.GetEditor(Type editorBaseType) { return TypeDescriptor.GetEditor(this, editorBaseType, true); }
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes) { return TypeDescriptor.GetEvents(this, attributes, true); }
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents() { return TypeDescriptor.GetEvents(this, true); }
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes) { return GetProperties(); }
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() { return GetProperties(); }
		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) { return this; }
		#endregion
		protected virtual PropertyDescriptorCollection GetProperties() {
			if(properties == null) properties = CreateProperties();
			return properties;
		}
		protected abstract PropertyDescriptorCollection CreateProperties();
		#region IDXDataErrorInfo Members
		public virtual void GetError(ErrorInfo info) {
		}
		public virtual void GetPropertyError(string propertyName, ErrorInfo info) {
		}
		#endregion
		public virtual void Dispose() {
		}
	}
	public abstract class EditFormDataProxyCoreEx : EditFormDataProxyCore {
		public EditFormDataProxyCoreEx(EditFormController owner)
			: base(owner) {
		}
		protected override PropertyDescriptorCollection CreateProperties() {
			List<PropertyDescriptor> res = new List<PropertyDescriptor>();
			foreach(GridColumn gc in View.Columns) {
				if(string.IsNullOrEmpty(gc.FieldName)) continue;
				res.Add(new GridPropertyDescriptor(this, gc.FieldName));
			}
			return new PropertyDescriptorCollection(res.ToArray());
		}
		protected internal abstract void SetValue(string name, object value);
		protected internal abstract object GetValue(string name);
		protected internal virtual Type GetPropertyType(string name) {
			var column = View.Columns[name];
			if(column == null) return typeof(object);
			return column.ColumnType;
		}
		protected internal virtual bool IsReadOnly(string name) {
			var column = View.Columns[name];
			return column == null || column.ReadOnly;
		}
		protected class GridPropertyDescriptor : PropertyDescriptor {
			class GPTypeConverter : TypeConverter {
				TypeConverter baseConverter;
				public GPTypeConverter(GridPropertyDescriptor owner, TypeConverter baseConverter) {
					this.baseConverter = baseConverter;
				}
				public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
					return baseConverter.CanConvertFrom(context, sourceType);
				}
				public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
					return baseConverter.CanConvertTo(context, destinationType);
				}
				public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
					return baseConverter.ConvertFrom(context, culture, value);
				}
				public override object CreateInstance(ITypeDescriptorContext context, System.Collections.IDictionary propertyValues) {
					return baseConverter.CreateInstance(context, propertyValues);
				}
				public override bool GetCreateInstanceSupported(ITypeDescriptorContext context) {
					return baseConverter.GetCreateInstanceSupported(context);
				}
				public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
					return baseConverter.ConvertTo(context, culture, value, destinationType);
				}
				public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
					return baseConverter.GetProperties(context, value, attributes);
				}
				public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
					return baseConverter.GetPropertiesSupported(context);
				}
				public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
					return baseConverter.GetStandardValues(context);
				}
				public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
					return baseConverter.GetStandardValuesExclusive(context);
				}
				public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
					return baseConverter.GetStandardValuesSupported(context);
				}
				public override bool IsValid(ITypeDescriptorContext context, object value) {
					return baseConverter.IsValid(context, value);
				}
			}
			EditFormDataProxyCoreEx owner;
			string columnName;
			public GridPropertyDescriptor(EditFormDataProxyCoreEx owner, string name)
				: base(EditFormController.FieldNameConverter(name), null) {
				this.owner = owner;
				this.columnName = name;
			}
			public override bool CanResetValue(object component) { return false; }
			public override Type ComponentType { get { return typeof(GridView); } }
			public override object GetValue(object component) { return owner.GetValue(columnName); }
			public override bool IsReadOnly { get { return owner.IsReadOnly(columnName); } }
			public override Type PropertyType { get { return owner.GetPropertyType(columnName); } }
			public override void ResetValue(object component) { }
			public override void SetValue(object component, object value) {
				owner.SetValue(columnName, value);
				OnValueChanged(component, EventArgs.Empty);
			}
			GPTypeConverter converter;
			public override TypeConverter Converter {
				get {
					if(converter == null) converter = new GPTypeConverter(this, base.Converter);
					return converter;
				}
			}
			public override bool ShouldSerializeValue(object component) { return false; }
		}
	}
	public class EditFormDataProxyGridBuffered : EditFormDataProxyCoreEx {
		int row;
		Dictionary<string, object> values;
		public EditFormDataProxyGridBuffered(Dictionary<string, object> values, EditFormController owner, int row)
			: base(owner) {
			this.row = row;
			this.values = values;
		}
		public override void Dispose() {
			base.Dispose();
			if(values != null) values.Clear();
		}
		protected int Row { get { return row; } }
		public override void PushData() {
			View.SetFocusedRowModified();
			ValuesCache v = new ValuesCache();
			v.PushValues(View, Row, values);
		}
		protected internal override void SetValue(string name, object value) {
			if(!Enabled) return;
			var column = View.Columns[name];
			if(column != null) Owner.ValidateCachedValue(Row, column, value);
			values[name] = value;
			IsModified = true;
		}
		protected internal override object GetValue(string name) {
			if(values.ContainsKey(name)) return values[name];
			return null;
		}
	}
	public class EditFormDataProxyGridDirect : EditFormDataProxyCoreEx {
		int row;
		public EditFormDataProxyGridDirect(EditFormController owner, int row)
			: base(owner) {
			this.row = row;
		}
		protected int Row { get { return row; } }
		protected internal override void SetValue(string name, object value) {
			if(!Enabled) return;
			var column = View.Columns[name];
			if(column != null) {
				IsModified = true;
				Owner.SetRowCellValue(Row, column, value);
			}
		}
		protected internal override object GetValue(string name) { return View.GetRowCellValue(Row, name); }
		public override void GetError(ErrorInfo info) {
		}
		public override void GetPropertyError(string propertyName, ErrorInfo info) {
			propertyName = EditFormController.FromFieldNameConverter(propertyName);
			var column = View.Columns[propertyName];
			if(column != null) {
				info.ErrorText = View.GetColumnError(column);
				info.ErrorType = View.GetColumnErrorType(column);
			}
		}
	}
}
