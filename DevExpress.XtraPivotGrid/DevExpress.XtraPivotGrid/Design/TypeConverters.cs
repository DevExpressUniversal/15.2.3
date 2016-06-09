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
using System.Data;
using System.ComponentModel.Design;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.ComponentModel.Design.Serialization;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using DevExpress.XtraPivotGrid;
using DevExpress.Data;
using DevExpress.Data.Helpers;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Data.Browsing.Design;
using System.Collections.Generic;
namespace DevExpress.XtraPivotGrid.TypeConverters {
	public class PivotColumnNameEditor : ColumnNameEditor {
		protected override object GetDataSource(ITypeDescriptorContext context) {
			PivotGridViewInfoData data = GetViewInfoData(context);
			if(data == null)
				return null;
			if(!String.IsNullOrEmpty(data.OLAPConnectionString))
				return data.GetDesignOLAPDataSourceObject();
			IDataContainerBase control = data.Control as IDataContainerBase;
			if(control == null)
				return null;
			return GetDataSource(control);
		}
		internal static object GetDataSource(IDataContainerBase control) {
			object dataSource = control.DataSource;
			string dataMember = control.DataMember;
			DataSet ds = dataSource as DataSet;
			if(ds != null) {
				if(dataMember == string.Empty)
					dataSource = ds.Tables.Count > 0 ? ds.Tables[0] : null;
				else
					dataSource = ds.Tables[dataMember];
			}
			return dataSource;
		}
		protected PivotGridViewInfoData GetViewInfoData(ITypeDescriptorContext context) {
			object instanceValue = DXObjectWrapper.GetInstance(context);
			if(instanceValue is IPivotGridViewInfoDataOwner)
				return ((IPivotGridViewInfoDataOwner)instanceValue).DataViewInfo;
			if(instanceValue is PivotGridFieldSortBySummaryInfo) {
				IPivotGridViewInfoDataOwner owner = ((PivotGridFieldSortBySummaryInfo)instanceValue).Owner as IPivotGridViewInfoDataOwner;
				if(owner != null)
					return owner.DataViewInfo;
			}
			return null;
		}
	}
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public class PivotAreaPropertyAttribute : Attribute {
		PivotArea area;
		public PivotAreaPropertyAttribute(PivotArea area) {
			this.area = area;
		}
		public PivotArea Area { get { return area; } }
	}
	public class FormatRuleSettingsTypeEditor : ObjectPickerEditor {
		protected override ObjectPickerControl CreateObjectPickerControl(ITypeDescriptorContext context, object value) {
			object current = null;
			object a = new FormatRuleApplySettingsTypeInfo<FormatRuleFieldIntersectionSettings>();
			object b = new FormatRuleApplySettingsTypeInfo<FormatRuleTotalTypeSettings>();
			if(value is FormatRuleFieldIntersectionSettings)
				current = a;
			else
				current = b;
			var res = new PickerFromValuesControl(this, current, new List<object>() { a, b }, false, 12, 300);
			return res;
		}
		protected override object ConvertFromValue(object oldValue, object newValue) {
			if(newValue != null) {
				IPivotFormatRuleApplySettingsCreator typeInfo = newValue as IPivotFormatRuleApplySettingsCreator;
				if(typeInfo != null) {
					if(typeInfo.Type.IsInstanceOfType(oldValue))
						return oldValue;
					return typeInfo.Create();
				}
			}
			return newValue;
		}
		public override bool IsDropDownResizable { get { return true; } }
	}
	interface IPivotFormatRuleApplySettingsCreator {
		Type Type { get; }
		FormatRuleSettings Create();
	}
	public class FormatRuleApplySettingsTypeInfo<TRule> : IPivotFormatRuleApplySettingsCreator where TRule : FormatRuleSettings, new() {
		public FormatRuleApplySettingsTypeInfo() {
		}
		public FormatRuleSettings Create() {
			return new TRule();
		}
		public override string ToString() {
			return FormatRuleSettings.GetDisplayText(typeof(TRule));
		}
		Type IPivotFormatRuleApplySettingsCreator.Type {
			get { return typeof(TRule); }
		}
		FormatRuleSettings IPivotFormatRuleApplySettingsCreator.Create() {
			return new TRule();
		}
	}
	public class FieldReferenceConverter : System.ComponentModel.TypeConverter {
		protected virtual string None { get { return "(none)"; } }
		protected virtual string GetFieldName(PivotGridField field) {
			IComponent comp = field as IComponent;
			if(comp.Site != null) {
				return comp.Site.Name;
			} else {
				if(!string.IsNullOrEmpty(field.Caption))
					return field.Name;
				else
					return "Field" + field.Index.ToString();
			}
		}
		protected virtual PivotGridViewInfoData GetViewInfoData(ITypeDescriptorContext context) {
			object instanceValue = DXObjectWrapper.GetInstance(context);
			if(instanceValue is IPivotGridViewInfoDataOwner)
				return ((IPivotGridViewInfoDataOwner)instanceValue).DataViewInfo;
			return null;
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType.Equals(typeof(string))) {
				if(value == null)
					return None;
				if(value is PivotGridField) {
					return GetFieldName(value as PivotGridField);
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type type) {
			if(GetViewInfoData(context) != null) {
				if(type != null && type.Equals(typeof(string))) {
					return true;
				}
			}
			return base.CanConvertFrom(context, type);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			if(value == null)
				return null;
			if(value is string) {
				string source = value.ToString();
				if(source == None)
					return null;
				PivotGridViewInfoData data = GetViewInfoData(context);
				if(data != null) {
					foreach(PivotGridField field in data.Fields) {
						if(source == GetFieldName(field))
							return field;
					}
				}
			}
			if(object.Equals(string.Empty, value))
				return true;
			return base.ConvertFrom(context, culture, value);
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			if(context == null || context.Instance == null)
				return null;
			ArrayList array = new ArrayList();
			array.Add(null);
			PivotArea? area = null;
			if(context != null && context.PropertyDescriptor != null && context.PropertyDescriptor.Attributes != null) {
				PivotAreaPropertyAttribute att = context.PropertyDescriptor.Attributes[typeof(PivotAreaPropertyAttribute)] as PivotAreaPropertyAttribute;
				if(att != null)
					area = att.Area;
			}
			PivotGridViewInfoData data = GetViewInfoData(context);
			if(data != null) {
				foreach(PivotGridField field in data.Fields) {
					if(area.HasValue && area.Value != field.Area)
						continue;
					array.Add(field);
				}
			}
			return new StandardValuesCollection(array);
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
	}
	public class FieldEditConverter : ComponentConverter {
		public FieldEditConverter(Type t) : base(t) { }
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return false;
		}
	}
	public class PivotExpressionEditor : DevExpress.XtraEditors.Design.ExpressionEditorBase {
		public PivotExpressionEditor(){ }
		protected override XtraEditors.Design.ExpressionEditorForm CreateForm(object instance, IDesignerHost designerHost, object value) {
			IDataColumnInfo info = instance as IDataColumnInfo;
			if(info == null) {
				IDXObjectWrapper wrapper = instance as IDXObjectWrapper;
				if(wrapper != null)
					info = wrapper.SourceObject as IDataColumnInfo;
				if(info != null)
					return  base.CreateForm(info, designerHost, value);
			}
			return base.CreateForm(instance, designerHost, value);
		}
	}
}
