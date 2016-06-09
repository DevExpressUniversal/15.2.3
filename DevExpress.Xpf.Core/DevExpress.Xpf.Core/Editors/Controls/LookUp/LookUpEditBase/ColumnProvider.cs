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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Data.Filtering;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Editors.Filtering;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Settings;
namespace DevExpress.Xpf.Editors {
	public class FilterControlColumnProvider : Decorator, IFilteredComponent {
		public static readonly DependencyProperty HiddenPropertiesProperty;
		public static readonly DependencyProperty UpperCasePropertyNamesProperty;
		public static readonly DependencyProperty AdditionalPropertiesProperty;
		public static readonly DependencyProperty ItemsSourceTypeProperty;
		public static readonly DependencyProperty SourceControlProperty;
		static FilterControlColumnProvider() {
			Type ownerType = typeof(FilterControlColumnProvider);
			ItemsSourceTypeProperty = DependencyProperty.Register("ItemsSourceType", typeof(Type), ownerType, new PropertyMetadata(null, (d, e) => ((FilterControlColumnProvider)d).ItemsSourceTypeChanged()));
			AdditionalPropertiesProperty = DependencyProperty.Register("AdditionalProperties", typeof(Filtering.PropertyInfoCollection), typeof(FilterControlColumnProvider), new PropertyMetadata(null, Update));
			HiddenPropertiesProperty = DependencyProperty.Register("HiddenProperties", typeof(PropertyNameCollection), typeof(FilterControlColumnProvider), new PropertyMetadata(null, Update));
			UpperCasePropertyNamesProperty = DependencyProperty.Register("UpperCasePropertyNames", typeof(bool), typeof(FilterControlColumnProvider), new PropertyMetadata(false, Update));
			SourceControlProperty = DependencyProperty.Register("SourceControl", typeof(IFilteredComponent), typeof(FilterControlColumnProvider), new PropertyMetadata(null, (d, e) => ((FilterControlColumnProvider)d).OnSourceControlChanged(e)));
		}
		void OnSourceControlChanged(DependencyPropertyChangedEventArgs e) {
			(e.OldValue as IFilteredComponent).Do(x => x.PropertiesChanged -= FilterControlColumnProvider_PropertiesChanged);
			(e.OldValue as IFilteredComponent).Do(x => x.RowFilterChanged -= FilterControlColumnProvider_RowFilterChanged);
			(e.NewValue as IFilteredComponent).Do(x => x.PropertiesChanged += FilterControlColumnProvider_PropertiesChanged);
			(e.NewValue as IFilteredComponent).Do(x => x.RowFilterChanged += FilterControlColumnProvider_RowFilterChanged);
			Update();
		}
		void FilterControlColumnProvider_PropertiesChanged(object sender, EventArgs e) {
			filteredComponentPropertiesChanged.Do(x => x(this, EventArgs.Empty));
		}
		void FilterControlColumnProvider_RowFilterChanged(object sender, EventArgs e) {
			rowFilterChanged.Do(x => x(this, EventArgs.Empty));
		}
		static void Update(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((FilterControlColumnProvider)d).Update();
		}
		StandardColumnsProvider provider;
		CriteriaOperator FilterCriteria { get; set; }
		FilterCondition FilterCondition { get; set; }
		string SearchText { get; set; }
		FilterByColumnsMode OverridedFilterByColumnsMode { get; set; }
		ColumnSourceMode ColumnSourceMode {
			get {
				if(SourceControl != null)
					return ColumnSourceMode.SourceControl;
				if(ItemsSourceType != null)
					return ColumnSourceMode.ItemSourceType;
				return ColumnSourceMode.None;
			}
		}
		public FilterControlColumnProvider() {
			provider = new StandardColumnsProvider(this);
			AdditionalProperties = new PropertyInfoCollection();
			HiddenProperties = new PropertyNameCollection();
		}
		void Update() {
			ItemsSourceTypeChanged();
		}
		IEnumerable<FilterColumn> GetAllColumns() {
			IEnumerable<FilterColumn> sourceColumns = GetSourceColums();
			return UpdatedColumns(sourceColumns);
		}
		IEnumerable<FilterColumn> UpdatedColumns(IEnumerable<FilterColumn> sourceColumns) {
			return sourceColumns.Where(x => !HiddenProperties.Contains(x.FieldName)).
				Concat(AdditionalProperties.Select(
				x => {
					var type = x.Type ?? typeof(string);
					var column = provider.GetStandardColumn(new CustomPropertyDescriptor(type));
					return CreateFilterColumn(
						UpdateColumnCaption(x.Caption ?? x.Name),
						column.EditSettings,
						type,
						x.Name ?? string.Empty);
				}));
		}
		internal IEnumerable<FilterColumn> GetSourceColums() {
			switch(ColumnSourceMode) {
				case ColumnSourceMode.None:
					return new List<FilterColumn>() { new FilterColumn() { FieldName = "" }};
				case ColumnSourceMode.SourceControl:
					return SourceControl.CreateFilterColumnCollection();
				case ColumnSourceMode.ItemSourceType:
					return FilteredComponentHelper.GetColumnsByType(this, ItemsSourceType, UpperCasePropertyNames);
				default:
					throw new InvalidOperationException();
			}
		}
		protected void ItemsSourceTypeChanged() {
			filteredComponentPropertiesChanged.Do(x => x(this, EventArgs.Empty));
		}
		public IFilteredComponent SourceControl {
			get { return (IFilteredComponent)GetValue(SourceControlProperty); }
			set { SetValue(SourceControlProperty, value); }
		}
		public PropertyNameCollection HiddenProperties {
			get { return (PropertyNameCollection)GetValue(HiddenPropertiesProperty); }
			set { SetValue(HiddenPropertiesProperty, value); }
		}
		public bool UpperCasePropertyNames {
			get { return (bool)GetValue(UpperCasePropertyNamesProperty); }
			set { SetValue(UpperCasePropertyNamesProperty, value); }
		}
		public Filtering.PropertyInfoCollection AdditionalProperties {
			get { return (Filtering.PropertyInfoCollection)GetValue(AdditionalPropertiesProperty); }
			set { SetValue(AdditionalPropertiesProperty, value); }
		}
		public Type ItemsSourceType {
			get { return (Type)GetValue(ItemsSourceTypeProperty); }
			set { SetValue(ItemsSourceTypeProperty, value); }
		}
		string UpdateColumnCaption(string caption) {
			if(UpperCasePropertyNames)
				return caption != null ? caption.ToUpper() : null;
			return caption;
		}
		protected virtual FilterColumn CreateFilterColumn(string columnCaption, BaseEditSettings editSettings, Type columnType, string fieldName) {
			return new FilterColumn {
				ColumnCaption = columnCaption,
				EditSettings = editSettings,
				ColumnType = columnType,
				FieldName = fieldName
			};
		}
		class CustomPropertyDescriptor : PropertyDescriptor {
			Type type;
			public CustomPropertyDescriptor(Type type)
				: base("name", new Attribute[0]) {
				this.type = type;
			}
			public override bool CanResetValue(object component) { return false; }
			public override Type ComponentType { get { return typeof(object); } }
			public override object GetValue(object component) { return null; }
			public override bool IsReadOnly { get { return true; } }
			public override Type PropertyType { get { return type; } }
			public override void ResetValue(object component) { }
			public override void SetValue(object component, object value) { }
			public override bool ShouldSerializeValue(object component) { return false; }
		}
		#region IFilteredComponent Members
		IEnumerable<FilterColumn> IFilteredComponent.CreateFilterColumnCollection() {
			return GetAllColumns();
		}
		EventHandler filteredComponentPropertiesChanged;
		EventHandler rowFilterChanged;
		event EventHandler IFilteredComponentBase.PropertiesChanged { add { filteredComponentPropertiesChanged += value; } remove { filteredComponentPropertiesChanged -= value; } }
		CriteriaOperator IFilteredComponentBase.RowCriteria { get { return SourceControl.RowCriteria; ; } set { SourceControl.RowCriteria = value; } }
		event EventHandler IFilteredComponentBase.RowFilterChanged { add { rowFilterChanged += value; } remove { rowFilterChanged -= value; } }
		#endregion
	}
}
namespace DevExpress.Xpf.Editors.Native {
	public enum ColumnSourceMode {
		None,
		SourceControl,
		ItemSourceType
	}
}
