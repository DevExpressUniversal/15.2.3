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

using DevExpress.Data.Filtering;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Filtering;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.PropertyGrid.Internal;
using DevExpress.Xpf.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
namespace DevExpress.Xpf.PropertyGrid {
	internal enum RowInfoColumns {
		Header,
		HasPropertyDefinition,
		IsCategory,
		FullPath,
		Path,
		Type,
		EditableObject,
		IsVisible,
		IsAttached,
		IsReadOnly
	}
	public class RowInfo {
		public const string DisplayName = "Header";
		public const string DisplayText = "EditableObject";
		static internal string[] columns = null;
		static internal string[] Columns {
			get {
				if (columns == null)
					columns = Enum.GetValues(typeof(RowInfoColumns)).OfType<RowInfoColumns>().Select(x => x.ToString()).ToArray();
				return columns;
			}
		}
		static internal FilterColumn GetColumn(RowInfoColumns column) {
			FilterColumn result = new FilterColumn();
			switch (column) {
				case RowInfoColumns.FullPath:
				case RowInfoColumns.Path:
				case RowInfoColumns.Type:
				case RowInfoColumns.Header:
					result.ColumnType = typeof(string);
					result.EditSettings = new TextEditSettings();
					break;
				case RowInfoColumns.HasPropertyDefinition:
				case RowInfoColumns.IsCategory:
					result.ColumnType = typeof(bool);
					result.EditSettings = new CheckEditSettings() { IsThreeState = false };
					break;
				case RowInfoColumns.EditableObject:
					result.ColumnType = typeof(object);
					result.EditSettings = new TextEditSettings();
					break;
				case RowInfoColumns.IsAttached:
				case RowInfoColumns.IsReadOnly:
				case RowInfoColumns.IsVisible:
					result.ColumnType = typeof(bool);
					result.EditSettings = new CheckEditSettings();
					break;
				default:
					result.ColumnType = typeof(string);
					result.EditSettings = new TextEditSettings();
					break;
			}
			result.ColumnCaption = column.ToString();
			result.FieldName = column.ToString();
			return result;
		}
		protected DataViewBase view;
		protected RowHandle handle;
		protected RowDataGenerator generator;
		string fullPath;
		string header;
		string path;
		Type type;
		object editableObject;
		bool isCategory;
		bool isCategoryInitialized = false;
		bool hasCustomDefinition;
		bool hasCustomDefinitionInitialized = false;
		bool? isVisible = null;
		bool? isAttached = null;
		bool? isReadOnly = null;		
		Lazy<PropertyDefinitionBase> definition;
		internal RowHandle Handle { get { return handle; } }
		protected internal PropertyDefinitionBase Definition {
			get { return definition.Value; }
		}
		public string Header {
			get {
				if (header == null) {
					InitializeHeader();
				}
				return header;
			}
		}
		public bool HasPropertyDefinition {
			get {
				if (!hasCustomDefinitionInitialized) {
					InitializeHasPropertyDefinition();
				}
				return hasCustomDefinition;
			}
		}
		public bool IsCategory {
			get {
				if (!isCategoryInitialized) {
					InitializeIsCategory();
				}
				return isCategory;
			}
		}
		public string FullPath {
			get {
				if (fullPath == null) {
					InitializeFullPath();
				}
				return fullPath;
			}
		}
		public string Path {
			get {
				if (path == null) {
					InitializePath();
				}
				return path;
			}
		}
		public object EditableObject {
			get {
				if (editableObject == null) {
					InitializeEditableObject();
				}
				return editableObject;
			}
		}
		public Type Type {
			get {
				if (type == null) {
					InitializeType();
				}
				return type;
			}
		}		
		public bool IsReadOnly {
			get {
				if (!isReadOnly.HasValue) {
					InitializeIsReadOnly();
				}
				return isReadOnly.Value; 
			}
		}		
		public bool IsAttached {
			get {
				if (!isAttached.HasValue) {
					InitializeIsAttached();
				}
				return isAttached.Value; 
			}
		}		
		public bool IsVisible {
			get {
				if (!isVisible.HasValue)
					InitializeIsVisible();
				return isVisible.Value;
			}
			set { isVisible = value; }
		}
		protected virtual bool CheckValidSource() {
			return view != null && handle != null && generator != null;
		}
		protected virtual void InitializeIsAttached() {
			if (!CheckValidSource())
				return;
			isAttached = view.IsAttachedProperty(handle);
		}
		protected virtual void InitializeIsReadOnly() {
			if (!CheckValidSource())
				return;
			if (IsCategory) {
				isReadOnly = false;
				return;
			}
			var pDef = (PropertyDefinition)Definition;
			if (pDef != null && (pDef.IsReadOnly.Return(x => x.Value, () => false) || pDef.EditSettings.Return(PropertyGridEditSettingsHelper.GetIsReadOnly, () => false))) {
				isReadOnly = true;
				return;
			}
			var sDef = (PropertyDefinition)generator.GetStandardDefinition(handle);
			if (sDef.IsReadOnly.Return(x => x.Value, () => false) || sDef.EditSettings.Return(PropertyGridEditSettingsHelper.GetIsReadOnly, () => false)) {
				isReadOnly = true;
				return;
			}
			isReadOnly = view.ShouldRenderReadOnly(handle) || view.IsReadOnly(handle);
		}
		protected virtual void InitializeFullPath() {
			if (!CheckValidSource())
				return;
			fullPath = view.GetFieldNameByHandle(handle) ?? "";
		}
		protected virtual void InitializeType() {
			if (!CheckValidSource())
				return;
			type = view.GetPropertyType(handle);
		}
		protected virtual void InitializeIsVisible() {
			if (!CheckValidSource())
				return;
			isVisible = !Definition.If(x => x.Visibility != Visibility.Visible).ReturnSuccess();
		}
		protected virtual void InitializeEditableObject() {
			if (!CheckValidSource())
				return;
			editableObject = view.GetValue(handle);
		}
		protected virtual void InitializePath() {
			if (!CheckValidSource())
				return;
			path = view.GetNameByHandle(handle) ?? "";
		}
		protected virtual void InitializeIsCategory() {
			if (!CheckValidSource())
				return;
			isCategory = view.IsGroupRowHandle(handle);
			isCategoryInitialized = true;
		}
		protected virtual void InitializeHasPropertyDefinition() {
			if (!CheckValidSource())
				return;
			hasCustomDefinition = Definition.If(x => !x.isStandardDefinition).ReturnSuccess();
			if (!hasCustomDefinition) {
				var parentHandle = view.GetParent(handle);
				if (parentHandle != null && !parentHandle.IsRoot && (GetDefinitionIfNonStandard(parentHandle) as CollectionDefinition).ReturnSuccess())
					hasCustomDefinition = true;
			}
			hasCustomDefinitionInitialized = true;
		}
		protected virtual void InitializeHeader() {
			if (!CheckValidSource())
				return;
			header = (Definition.With(x => x.Header) ?? view.GetDisplayName(handle)) ?? "";
		}
		internal RowInfo() : this(null, null, null) {
		}
		internal RowInfo(DataViewBase view, RowHandle handle, RowDataGenerator generator) {
			this.view = view;
			this.handle = handle;
			this.generator = generator;
			this.definition = new Lazy<PropertyDefinitionBase>(GetDefinitionIfNonStandard);
		}
		internal RowInfo(RowHandle handle, RowDataGenerator generator) : this(generator.DataView, handle, generator) { }
		PropertyDefinitionBase GetDefinitionIfNonStandard() {
			return GetDefinitionIfNonStandard(handle);
		}
		PropertyDefinitionBase GetDefinitionIfNonStandard(RowHandle rowHandle) {
			return generator.PropertyBuilder.GetDefinition(view, rowHandle, view is CategoryDataView, false);
		}
	}
	public class FilterColumnProvider : Decorator, ISearchPanelColumnProvider, IFilteredComponent {
		protected static readonly DependencyPropertyKey ColumnsPropertyKey;
		public static readonly DependencyProperty ColumnsProperty;
		protected static readonly DependencyPropertyKey AvailableColumnsPropertyKey;
		public static readonly DependencyProperty AvailableColumnsProperty;
		public static readonly DependencyProperty CustomColumnsProperty;
		public static readonly DependencyProperty OwnerProperty;
		static FilterColumnProvider() {
			ColumnsPropertyKey = DependencyPropertyManager.RegisterReadOnly("Columns", typeof(ReadOnlyObservableCollection<string>), typeof(FilterColumnProvider), new FrameworkPropertyMetadata(null));
			ColumnsProperty = ColumnsPropertyKey.DependencyProperty;
			AvailableColumnsPropertyKey = DependencyPropertyManager.RegisterReadOnly("AvailableColumns", typeof(ReadOnlyObservableCollection<string>), typeof(FilterColumnProvider), new FrameworkPropertyMetadata(null));
			AvailableColumnsProperty = AvailableColumnsPropertyKey.DependencyProperty;
			CustomColumnsProperty = DependencyPropertyManager.Register("CustomColumns", typeof(ObservableCollection<string>), typeof(FilterColumnProvider), new FrameworkPropertyMetadata(null, (d, e) => ((FilterColumnProvider)d).OnCustomColumnsChanged((ObservableCollection<string>)e.OldValue, (ObservableCollection<string>)e.NewValue)));
			OwnerProperty = DependencyPropertyManager.Register("Owner", typeof(PropertyGridControl), typeof(FilterColumnProvider), new FrameworkPropertyMetadata(null, (d, e) => ((FilterColumnProvider)d).OnOwnerChanged((PropertyGridControl)e.OldValue, (PropertyGridControl)e.NewValue)));
		}
		public FilterColumnProvider() {
			filterByColumnsMode = FilterByColumnsMode.Default;
			filterModeChanged = new EventHandler(OnOwnerFilterModeChanged);
		}
		CriteriaOperator rowCriteria;
		FilterCondition filterCondition;
		string searchText;
		FilterByColumnsMode filterByColumnsMode;
		EventHandler rowFilterChanged;
		EventHandler propertiesChanged;
		EventHandler filterModeChanged;
		public ObservableCollection<string> CustomColumns {
			get { return (ObservableCollection<string>)GetValue(CustomColumnsProperty); }
			set { SetValue(CustomColumnsProperty, value); }
		}
		public ReadOnlyObservableCollection<string> AvailableColumns {
			get { return (ReadOnlyObservableCollection<string>)GetValue(AvailableColumnsProperty); }
			protected internal set { this.SetValue(AvailableColumnsPropertyKey, value); }
		}
		public ReadOnlyObservableCollection<string> Columns {
			get { return (ReadOnlyObservableCollection<string>)GetValue(ColumnsProperty); }
			protected internal set { this.SetValue(ColumnsPropertyKey, value); }
		}
		public PropertyGridControl Owner {
			get { return (PropertyGridControl)GetValue(OwnerProperty); }
			set { SetValue(OwnerProperty, value); }
		}
		#region ISearchPanelColumnProvider
		IEnumerable<string> ISearchPanelColumnProvider.Columns {
			get { return Columns; }
		}
		public virtual void UpdateColumns(FilterByColumnsMode mode) {
			filterByColumnsMode = mode;
			AvailableColumns = new ReadOnlyObservableCollection<string>(Owner.Return(x => x.GetAvailableColumns(), () => new ObservableCollection<string>()));
			switch (mode) {
				case FilterByColumnsMode.Custom:
					Columns = new ReadOnlyObservableCollection<string>(CustomColumns.Return(x => x, () => new ObservableCollection<string>()));
					break;
				case FilterByColumnsMode.Default:
					var columnsCollection = new ObservableCollection<string>();
					if (Owner != null) {
						if ((Owner.FilterMode & PropertyGridFilterMode.ByHeader) == PropertyGridFilterMode.ByHeader)
							columnsCollection.Add(RowInfo.DisplayName);
						if ((Owner.FilterMode & PropertyGridFilterMode.ByValue) == PropertyGridFilterMode.ByValue)
							columnsCollection.Add(RowInfo.DisplayText);
					}
					else {
						columnsCollection.Add(RowInfo.DisplayName);
					}
					Columns = new ReadOnlyObservableCollection<string>(columnsCollection);
					break;
			}
		}
		bool ISearchPanelColumnProviderBase.UpdateFilter(string searchText, FilterCondition filterCondition, CriteriaOperator filterCriteria) {
			this.searchText = searchText;
			this.filterCondition = filterCondition;
			((IFilteredComponentBase)this).RowCriteria = filterCriteria;
			return true;
		}
		string ISearchPanelColumnProviderBase.GetSearchText() {
			return searchText;
		}
		#endregion
		#region IFilteredComponent
		IEnumerable<FilterColumn> IFilteredComponent.CreateFilterColumnCollection() {
			return CreateFilterColumnCollectionOverride();
		}
		event EventHandler IFilteredComponentBase.PropertiesChanged {
			add { propertiesChanged += value; }
			remove { propertiesChanged -= value; }
		}
		event EventHandler IFilteredComponentBase.RowFilterChanged {
			add { rowFilterChanged += value; }
			remove { rowFilterChanged -= value; }
		}
		CriteriaOperator IFilteredComponentBase.RowCriteria {
			get { return rowCriteria; }
			set {
				if (CriteriaOperator.Equals(value, rowCriteria))
					return;
				CriteriaOperator oldValue = rowCriteria;
				rowCriteria = value;
				OnRowCriteriaChanged(oldValue);
			}
		}
		#endregion
		protected virtual void OnRowCriteriaChanged(CriteriaOperator oldValue) {
			PerformUpdate();
			RaiseRowFilterChangedChanged();
		}
		protected internal void RaiseRowFilterChangedChanged() {
			if (rowFilterChanged != null)
				rowFilterChanged(this, new EventArgs());
		}
		protected internal void RaiseFilterColumnsChanged() {
			if (propertiesChanged != null)
				propertiesChanged(this, new EventArgs());
		}
		protected virtual IEnumerable<FilterColumn> CreateFilterColumnCollectionOverride() {
			return Enum.GetValues(typeof(RowInfoColumns)).OfType<RowInfoColumns>().Select(x => RowInfo.GetColumn(x)).ToArray();
		}
		protected virtual void OnCustomColumnsChanged(ObservableCollection<string> oldValue, ObservableCollection<string> newValue) {
			PerformUpdate();
		}
		protected virtual void OnOwnerChanged(PropertyGridControl oldValue, PropertyGridControl newValue) {
			if (oldValue != null)
				oldValue.FilterModeChanged -= filterModeChanged;
			if (newValue != null)
				newValue.FilterModeChanged += filterModeChanged;
			PerformUpdate();
		}
		protected virtual void PerformUpdate() {
			if (Owner != null)
				Owner.UserFilterCriteria = ((IFilteredComponentBase)this).RowCriteria;
			UpdateColumns(filterByColumnsMode);
		}
		protected void OnOwnerFilterModeChanged(object sender, EventArgs args) {
			UpdateColumns(filterByColumnsMode);
		}
	}
	public class FilterColumnProviderExtension : System.Windows.Markup.MarkupExtension {
		public FilterColumnProviderExtension() { }
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return new FilterColumnProvider();
		}
	}
}
