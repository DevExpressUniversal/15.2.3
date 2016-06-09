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

using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
namespace DevExpress.XtraReports.Native {
	class RepositoryItemMultiValueEdit : RepositoryItemTextEdit {
		internal const string MultiValueEditName = "MultiValueEdit";
		#region static
		static RepositoryItemMultiValueEdit() {
			EditorClassInfo editorInfo = new EditorClassInfo(MultiValueEditName, typeof(MultiValueEdit), typeof(RepositoryItemMultiValueEdit), typeof(TextEditViewInfo), new TextEditPainter(), true, EditImageIndexes.TextEdit, typeof(DevExpress.Accessibility.TextEditAccessible));
			EditorRegistrationInfo.Default.Editors.Add(editorInfo);
		}
		#endregion
		Type valueType;
		TypeConverter baseConverter;
		public Type ValueType {
			get { return valueType; }
			set { valueType = value; }
		}
		public TypeConverter BaseConverter {
			get { return baseConverter; }
			set { baseConverter = value; }
		}
		public RepositoryItemMultiValueEdit() {
			this.valueType = typeof(string);
			this.baseConverter = new StringConverter();
		}
		public RepositoryItemMultiValueEdit(Type valueType, TypeConverter baseConverter) {
			this.valueType = valueType;
			this.baseConverter = baseConverter;
		}
		public override string EditorTypeName {
			get { return MultiValueEditName; }
		}
		public override void Assign(RepositoryItem item) {
			RepositoryItemMultiValueEdit source = item as RepositoryItemMultiValueEdit;
			BeginUpdate();
			try {
				base.Assign(item);
				if(source != null) {
					valueType = source.ValueType;
					baseConverter = source.BaseConverter;
				}
			} finally {
				EndUpdate();
			}
		}
		public override string GetDisplayText(object editValue) {
			TypeConverter converter = new MultiValueConverter(ValueType, BaseConverter);
			if(converter.CanConvertTo(typeof(string))) {
				return (string)converter.ConvertTo(editValue, typeof(string));
			}
			return base.GetDisplayText(editValue);
		}
	}
	class MultiValueEdit : TextEdit {
		public override string EditorTypeName {
			get { return RepositoryItemMultiValueEdit.MultiValueEditName; }
		}
		public new RepositoryItemMultiValueEdit Properties {
			get { return (RepositoryItemMultiValueEdit)base.Properties; }
		}
		protected override object ExtractParsedValue(ConvertEditValueEventArgs e) {
			if(e.Handled)
				return e.Value;
			TypeConverter converter = new MultiValueConverter(Properties.ValueType, Properties.BaseConverter);
			if(converter.CanConvertFrom(e.Value.GetType())) {
				try {
					return converter.ConvertFrom(e.Value);
				} catch {
					return e.Value;
				}
			}
			return base.ExtractParsedValue(e);
		}
		protected override void OnValidatingCore(CancelEventArgs e) {
			base.OnValidatingCore(e);
			if(EditValue is string)
				e.Cancel = true;
		}
	}
	public class MultiValueConverter : TypeConverter {
		Type parameterType;
		TypeConverter baseConverter;
		public MultiValueConverter(Type parameterType, TypeConverter baseConverter) {
			Guard.ArgumentNotNull(parameterType, "parameterType");
			Guard.ArgumentNotNull(baseConverter, "baseConverter");
			this.parameterType = parameterType;
			this.baseConverter = baseConverter;
		}
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
			if(!(value is string)) {
				return base.ConvertFrom(context, culture, value);
			}
			culture = culture ?? CultureInfo.CurrentCulture;
			string listSeparator = culture.TextInfo.ListSeparator;
			string[] stringValueArray = ((string)value).Split(new string[] { listSeparator }, StringSplitOptions.None);
			System.Collections.ArrayList list = new System.Collections.ArrayList();
			if(baseConverter.CanConvertFrom(typeof(string))) {
				foreach(string stringValue in stringValueArray)
					list.Add(baseConverter.ConvertFrom(context, culture, stringValue));
			}
			return list.ToArray(parameterType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType != typeof(string)) {
				return base.ConvertFrom(context, culture, value);
			}
			if(!baseConverter.CanConvertTo(typeof(string)) || !(value is IEnumerable))
				return string.Empty;
			if(value is string)
				return (string)value;
			culture = culture ?? CultureInfo.CurrentCulture;
			List<string> list = new List<string>();
			foreach(object element in (IEnumerable)value)
				list.Add((string)baseConverter.ConvertTo(context, culture, element, destinationType));
			return string.Join(culture.TextInfo.ListSeparator, list.ToArray());
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
		}
	}
	class RepositoryItemMultiValueComboBoxEdit : RepositoryItemCheckedComboBoxEdit {
		static RepositoryItemMultiValueComboBoxEdit() {
			EditorClassInfo info = new EditorClassInfo(typeof(MultiValueComboBoxEdit).Name, typeof(MultiValueComboBoxEdit), typeof(RepositoryItemMultiValueComboBoxEdit), typeof(DevExpress.XtraEditors.ViewInfo.PopupContainerEditViewInfo), new DevExpress.XtraEditors.Drawing.ButtonEditPainter(), false);
			EditorRegistrationInfo.Default.Editors.Add(info);
		}
		public Type ValueType {
			get;
			set;
		}
		public override string EditorTypeName { get { return typeof(MultiValueComboBoxEdit).Name; } }
		public RepositoryItemMultiValueComboBoxEdit() {
			ValueType = typeof(object);
		}
		protected override void PreQueryResultValue(QueryResultValueEventArgs e) {
			base.PreQueryResultValue(e);
			if(e.Value is IEnumerable) {
				ArrayList list = new ArrayList();
				foreach(var item in (IEnumerable)e.Value)
					list.Add(item);
				Type listType = list.Count > 0 ? list[0].GetType() : ValueType;
				e.Value = list.ToArray(listType);
			}
		}
		protected override void PreQueryDisplayText(QueryDisplayTextEventArgs e) {
			base.PreQueryDisplayText(e);
			if(!GetItems().Cast<object>().Where(item => ((IDisplayTextEvaluatorOwner)this).IsItemChecked(e.EditValue, item)).Any())
				e.DisplayText = string.Empty;
		}
		public override void Assign(RepositoryItem item) {
			base.Assign(item);
			ValueType = ((RepositoryItemMultiValueComboBoxEdit)item).ValueType;
		}
	}
	class MultiValueComboBoxEdit : CheckedComboBoxEdit {
		public override string EditorTypeName {
			get { return typeof(MultiValueComboBoxEdit).Name; }
		}
	}
}
