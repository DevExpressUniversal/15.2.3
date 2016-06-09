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
using System.Linq;
using System.Text;
using DevExpress.Xpf.Editors;
using System.Collections.ObjectModel;
using System.Collections;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Core;
using System.Windows.Controls;
using System.Windows;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Editors.Helpers;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
#endif
using System.Collections.Specialized;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Xpf.Grid.Native;
using System.Windows.Data;
using DevExpress.XtraPrinting.Native;
using DevExpress.Data;
using System.ComponentModel;
using DevExpress.Data.Helpers;
using DevExpress.Utils;
namespace DevExpress.Xpf.Grid {
	[DXToolboxBrowsable(false)]
	public class GridControlColumnProviderBase : Decorator, ISearchPanelColumnProviderEx {
		public static readonly DependencyProperty AllowGridExtraFilterProperty;
		public static readonly DependencyProperty HighlightColumnProperty;
		public static readonly DependencyProperty ColumnProviderProperty;
		public static readonly DependencyProperty DataControlBaseProperty;
		internal static readonly DependencyPropertyKey ColumnsPropertyKey;
		public static readonly DependencyProperty ColumnsProperty;
		public static readonly DependencyProperty AllowColumnsHighlightingProperty;
		public static readonly DependencyProperty AllowTextHighlightingProperty;
		public static readonly DependencyProperty FilterByColumnsModeProperty;
		public static readonly DependencyProperty CustomColumnsProperty;
		static GridControlColumnProviderBase() {
			Type ownerType = typeof(GridControlColumnProviderBase);
			DataControlBaseProperty = DependencyPropertyManager.Register("DataControlBase", typeof(DataControlBase), ownerType,
				new FrameworkPropertyMetadata(null, (d, e) => ((GridControlColumnProviderBase)d).GridControlChanged((DataControlBase)e.OldValue, (DataControlBase)e.NewValue)));
			ColumnProviderProperty = DependencyPropertyManager.RegisterAttached("ColumnProvider", typeof(GridControlColumnProviderBase), ownerType,
				new FrameworkPropertyMetadata(null, ColumnProviderPropertyChanged));
			HighlightColumnProperty = DependencyPropertyManager.RegisterAttached("HighlightColumn", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, HighlightColumnPropertyChanged));
			AllowColumnsHighlightingProperty = DependencyPropertyManager.Register("AllowColumnsHighlighting", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, (d, e) => ((GridControlColumnProviderBase)d).AllowColumnsHighlightingChanged((bool)e.NewValue)));
			AllowGridExtraFilterProperty = DependencyPropertyManager.Register("AllowGridExtraFilter", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, (d, e) => ((GridControlColumnProviderBase)d).AllowGridExtraFilterChanged((bool)e.NewValue)));
			AllowTextHighlightingProperty = DependencyPropertyManager.Register("AllowTextHighlighting", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, (d, e) => ((GridControlColumnProviderBase)d).AllowTextHighlightingChanged((bool)e.NewValue)));
			ColumnsPropertyKey = DependencyPropertyManager.RegisterReadOnly("Columns", typeof(IList), ownerType,
				new FrameworkPropertyMetadata(null));
			ColumnsProperty = ColumnsPropertyKey.DependencyProperty;
			FilterByColumnsModeProperty = DependencyPropertyManager.Register("FilterByColumnsMode", typeof(FilterByColumnsMode), ownerType, new FrameworkPropertyMetadata(FilterByColumnsMode.Custom, (d, e) => ((GridControlColumnProviderBase)d).UpdateColumns()));
			CustomColumnsProperty = DependencyPropertyManager.Register("CustomColumns", typeof(ObservableCollection<string>), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((GridControlColumnProviderBase)d).CustomColumnsChanged((ObservableCollection<string>)e.OldValue, (ObservableCollection<string>)e.NewValue)));
		}
		public ObservableCollection<string> CustomColumns {
			get { return (ObservableCollection<string>)GetValue(CustomColumnsProperty); }
			set { this.SetValue(CustomColumnsProperty, value); }
		}
		void CustomColumnsChanged(ObservableCollection<string> oldCollection, ObservableCollection<string> newCollection) {
			if(oldCollection != null)
				oldCollection.CollectionChanged -= CustomColumnsCollectionChanged;
			if(newCollection != null)
				newCollection.CollectionChanged += CustomColumnsCollectionChanged;
			UpdateColumns();
		}
		void CustomColumnsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			UpdateColumns();
		}
		public static bool GetHighlightColumn(DependencyObject d) {
			return (bool)d.GetValue(HighlightColumnProperty);
		}
		public static void SetHighlightColumn(DependencyObject d, bool value) {
			d.SetValue(HighlightColumnProperty, value);
		}
		public static GridControlColumnProviderBase GetColumnProvider(DependencyObject d) {
			return d != null ? d.GetValue(ColumnProviderProperty) as GridControlColumnProviderBase : null;
		}
		public static void SetColumnProvider(DependencyObject d, GridControlColumnProviderBase value) {
			d.SetValue(ColumnProviderProperty, value);
		}
		static void HighlightColumnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if(!(d is GridColumnBase))
				return;
			UpdateCustomColumnHighlighting((GridColumnBase)d);
		}
		static void ColumnProviderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			GridControlColumnProviderBase columnProvider = (GridControlColumnProviderBase)e.NewValue;
			if(columnProvider != null)
				columnProvider.DataControlBase = (DataControlBase)d;
		}
		static void UpdateCustomColumnHighlighting(GridColumnBase column) {
			if(column.OwnerControl == null)
				return;
			GridControlColumnProviderBase provider = GetColumnProvider(column.OwnerControl);
			if(provider != null)
				provider.ApplyHighlighting();
		}
		bool isSearchLookUpMode = false;
		public bool IsSearchLookUpMode { 
			get { return isSearchLookUpMode; }
			set { isSearchLookUpMode = value; } 
		}
		ReadOnlyObservableCollection<CustomFilterColumn> customFilterColumns = null;
		public IList<CustomFilterColumn> CustomFilterColumns { get { return customFilterColumns; } }
		IList oldColumns = null;
		public IList Columns {
			get { return (IList)GetValue(ColumnsProperty); }
			internal set { this.SetValue(ColumnsPropertyKey, value); }
		}
		public DataControlBase DataControlBase {
			get { return (DataControlBase)GetValue(DataControlBaseProperty); }
			set { SetValue(DataControlBaseProperty, value); }
		}
		public bool AllowColumnsHighlighting {
			get { return (bool)GetValue(AllowColumnsHighlightingProperty); }
			set { SetValue(AllowColumnsHighlightingProperty, value); }
		}
		public bool AllowTextHighlighting {
			get { return (bool)GetValue(AllowTextHighlightingProperty); }
			set { SetValue(AllowTextHighlightingProperty, value); }
		}
		public bool AllowGridExtraFilter {
			get { return (bool)GetValue(AllowGridExtraFilterProperty); }
			set { SetValue(AllowGridExtraFilterProperty, value); }
		}
		public FilterByColumnsMode FilterByColumnsMode {
			get { return (FilterByColumnsMode)GetValue(FilterByColumnsModeProperty); }
			set { SetValue(FilterByColumnsModeProperty, value); }
		}
		string oldSearchString = null;
		string searchString = null;
		string SearchString {
			get { return searchString; }
			set {
				if(value == searchString)
					return;
				oldSearchString = searchString;
				searchString = value;
			}
		}
		FilterCondition FilterCondition { get; set; }
		CriteriaOperator filterCriteria = null;
		CriteriaOperator oldFilterCriteria = null;
		CriteriaOperator FilterCriteria {
			get { return filterCriteria; }
			set {
				oldFilterCriteria = filterCriteria;
				filterCriteria = value;
			}
		}
		public GridControlColumnProviderBase() {
			Columns = new List<ColumnBase>();
		}
		public void UpdateColumns() {
			if(DataControlBase != null)
				UpdateColumnNameCollection();
			UpdateGridFilter();
		}
		public void UpdateColumns(FilterByColumnsMode mode) {
			FilterByColumnsMode = mode;
			UpdateColumns();
		}
		public void UpdateColumnNameCollection() {
			if(DataControlBase == null) {
				Columns = null;
				oldColumns = null;
				return;
			}
			oldColumns = Columns;
			Columns = new List<ColumnBase>();
			switch(FilterByColumnsMode) {
				case Editors.FilterByColumnsMode.Default:
					foreach(ColumnBase column in DataControlBase.ColumnsCore) {
						if(String.IsNullOrEmpty(column.FieldName))
							continue;
						if(column.ActualAllowSearchPanel) { 
							Columns.Add(column);
						}
					}
					break;
				case Editors.FilterByColumnsMode.Custom:
					if(CustomColumns != null) {
						foreach(ColumnBase column in DataControlBase.ColumnsCore) {
							if(column.AllowSearchPanel == DefaultBoolean.True) {
								Columns.Add(column);
								continue;
							}
							if(CustomColumns.Contains(column.FieldName) && column.AllowSearchPanel != DefaultBoolean.False) {
								Columns.Add(column);
								continue;
							}
						}
					}
					break;
			}
			UpdateColumnsPrefix();			
		}
		void UpdateColumnsPrefix() {
			columnsForceWithoutPrefix = new List<ColumnBase>();
			List<DataColumnInfo> columnInfos = DataControlBase.DataProviderBase.Columns.ToArray().ToList();
			foreach(ColumnBase column in DataControlBase.ColumnsCore) {
				if(!Columns.Contains(column))
					continue;
				if(ExcludeColumnInServerMode(column))
					continue;
				if(columnInfos.Where(info => ((IDataColumnInfo)info).FieldName == column.FieldName).FirstOrDefault() == null) {
					if(column.IsUnbound)
						columnsForceWithoutPrefix.Add(column);
					Columns.Remove(column);
				}
			}
		}
		bool ExcludeColumnInServerMode(ColumnBase column) {
#if SL
			if(!((ISearchPanelColumnProviderEx)this).IsServerMode)
				return false;
			if(column.FieldType == typeof(TimeSpan) || column.FieldType == typeof(TimeSpan?)) {
				Columns.Remove(column);
				return true;
			}
#endif
			return false;
		}
#if DEBUGTEST
		public int debug_TextHighlightingUpdatesCount = 0;
#endif
		public void UpdateGridFilter(bool force = false) {
			if(DataControlBase == null)
				return;
			if(!force && ReferenceEquals(oldFilterCriteria, null) && ReferenceEquals(FilterCriteria, null)) {
				GridSearchControlBase searchControl = DataControlBase.DataView.SearchControl as GridSearchControlBase;
				if(searchControl == null)
					return;
				if(GridColumnListParser.IsColumnsListsEquals(oldColumns, Columns)) {
					if(SearchString != searchControl.SearchText)
						SearchString = searchControl.SearchText;
				}
				else {
					if(!String.IsNullOrWhiteSpace(searchControl.SearchText)) {
						searchControl.UpdateColumnProvider();
						return;
					}
				}
			}
			if(force)
				DataControlBase.ExtraFilter = null;
			if(DataControlBase.IsLoading || DataControlBase.IsDeserializing)
				UpdateGridFilterCore();
			else {
				if(!updateActionEnqueued) {
					updateActionEnqueued = true;
					DataControlBase.DataView.ImmediateActionsManager.EnqueueAction(UpdateGridFilterCore);
				}
			}
		}
		volatile bool updateActionEnqueued = false;
		void UpdateGridFilterCore() {
			DataControlBase.ExtraFilter = AllowGridExtraFilter ? FilterCriteria : null;
			if(DataControlBase.DataView.SearchControl != null && DataControlBase.VisibleRowCount > 0)
				DataControlBase.DataView.SearchControl.UpdateMRU();
			ApplyHighlighting();
			updateActionEnqueued = false;
		}
		void OnControlFilterChanged(object sender, RoutedEventArgs e) {
			foreach(ColumnBase column in DataControlBase.ColumnsCore) {
				column.UpdateViewInfo();
			}
		}
		void ApplyHighlighting() {
			if(AllowColumnsHighlighting)
				ApplyColumnsHighlighting();
			ApplyTextHighlighting();
		}
		void ApplyColumnsHighlighting() {
			if(DataControlBase == null)
				return;
			foreach(GridColumnBase gridColumn in DataControlBase.ColumnsCore) {
				gridColumn.AllowSearchHeaderHighlighting = AllowColumnsHighlighting && Columns.Contains(gridColumn);
			}
		}
		Dictionary<ColumnBase, TextHighlightingProperties> allHighlighingProperties = new Dictionary<ColumnBase, TextHighlightingProperties>();
		void ApplyTextHighlighting() {
			allHighlighingProperties.Clear();
			if(DataControlBase == null)
				return;
			if(oldSearchString == null && searchString == null)
				return;
			List<FieldAndHighlightingString> highlightingStrings = AllowTextHighlighting ? SearchControlHelper.GetTextHighlightingString(searchString, Columns, FilterCondition) : null;
#if DEBUGTEST
			debug_TextHighlightingUpdatesCount++;
#endif
			if(highlightingStrings == null || highlightingStrings.Count == 0) {
				UpdateViewHighlightingText();
				return;
			}
			foreach(GridColumnBase gridColumn in DataControlBase.ColumnsCore) {
				FieldAndHighlightingString fhrl = highlightingStrings.Where(fhr => fhr.Field == gridColumn.FieldName).FirstOrDefault();
				if(fhrl == null)
					continue;
				allHighlighingProperties.Add(gridColumn, new TextHighlightingProperties(fhrl.HighlightingString, FilterCondition));
			}
			UpdateViewHighlightingText();
		}
		protected internal TextHighlightingProperties GetTextHighlightingProperties(ColumnBase column) {
			TextHighlightingProperties highlighingProperties = null;
			allHighlighingProperties.TryGetValue(column, out highlighingProperties);
			return highlighingProperties;
		}
		protected virtual void GridControlChanged(DataControlBase oldGrid, DataControlBase grid) {
			if(oldGrid != null)
				UnsubscribeFromGridControl(oldGrid);
			if(grid != null)
				SubscribeToGridControl(grid);
			UpdateColumns();
		}
		void UpdateViewHighlightingText() {
			if(DataControlBase == null || DataControlBase.viewCore == null)
				return;
			DataControlBase.viewCore.UpdateEditorHighlightingText();
		}
		void SubscribeToGridControl(DataControlBase grid) {
			DataControlBase = grid;
			grid.ColumnsCore.CollectionChanged += ColumnsCollectionChanged;
			grid.FilterChanged += OnControlFilterChanged;
			SetColumnProvider(grid, this);
			if(grid.viewCore != null)
				grid.viewCore.ApplySearchColumns();
		}
		void UnsubscribeFromGridControl(DataControlBase grid) {
			grid.ColumnsCore.CollectionChanged -= ColumnsCollectionChanged;
			grid.FilterChanged -= OnControlFilterChanged;
			grid.ClearValue(ColumnProviderProperty);
		}
		void ColumnsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			UpdateColumns();
		}
		protected virtual void AllowTextHighlightingChanged(bool value) {
			ApplyHighlighting();
		}
		protected virtual void AllowColumnsHighlightingChanged(bool value) {
			ApplyHighlighting();
		}
		protected virtual void AllowGridExtraFilterChanged(bool value) {
			UpdateGridFilter();
		}
		public List<string> GetAllSearchColumns() { 
			List<string> result = new List<string>();
			if(Columns != null) {
				foreach(ColumnBase column in Columns)
					result.Add(column.FieldName);
			}
			if(ColumnsForceWithoutPrefix != null) {
				foreach(ColumnBase column in ColumnsForceWithoutPrefix)
					result.Add(column.FieldName);
			}
			if(CustomColumns != null) {
				foreach(string column in CustomColumns)
					result.Add(column); 
			}
			return result;
		}
		#region ISearchPanelColumnProviderEx Members
		bool ISearchPanelColumnProviderEx.IsServerMode {
			get { return (DataControlBase == null || DataControlBase.DataProviderBase == null) ? false : DataControlBase.DataProviderBase.IsServerMode || DataControlBase.DataProviderBase.IsAsyncServerMode; }
		}
		string ISearchPanelColumnProviderBase.GetSearchText() {
			return SearchString;
		}
		bool ISearchPanelColumnProviderBase.UpdateFilter(string searchText, FilterCondition filterCondition, DevExpress.Data.Filtering.CriteriaOperator filterCriteria) {
			if(DataControlBase == null)
				return false;
			SearchString = searchText;
			FilterCondition = filterCondition;
			FilterCriteria = filterCriteria;
			UpdateColumns();
			return false;
		}
		List<ColumnBase> columnsForceWithoutPrefix;
		public IList ColumnsForceWithoutPrefix {
			get { return columnsForceWithoutPrefix; }
		}
		#endregion
	}
	public class GridColumnListParser {
		List<ColumnBase> columnsSource;
		List<string> columnsNames;
		List<string> result = null;
		GridColumnListParser(List<ColumnBase> columnsSource, string searchColumns) {
			this.columnsSource = columnsSource;
			columnsNames = SearchControlHelper.ParseColumnsString(searchColumns);
		}
		ObservableCollection<string> GetSearchColumns() {
			if(columnsNames.Count == 1 && columnsNames[0] == "*")
				return null;
			result = new List<string>();
			UpdateAllowColumnInSeachPanelIfNeed(false, false);
			if(columnsNames.Count != 0) {
				for(int i = 0; i < columnsNames.Count; i++)
					columnsNames[i] = columnsNames[i].ToLower();
				UpdateAllowColumnInSeachPanelIfNeed(false, true);
			}
			if(columnsNames.Count != 0) {
				for(int i = 0; i < columnsNames.Count; i++)
					columnsNames[i] = columnsNames[i].Replace(" ", String.Empty);
				UpdateAllowColumnInSeachPanelIfNeed(true, true);
			}
			return new ObservableCollection<string>(result);
		}
		void UpdateAllowColumnInSeachPanelIfNeed(bool removeSpaces, bool isToLower) {
			List<string> result = new List<string>();
			List<ColumnBase> removedColumns = new List<ColumnBase>();
			foreach(ColumnBase column in columnsSource) {
				string fieldName = removeSpaces ? column.FieldName.ToString().Replace(" ", String.Empty) : column.FieldName.ToString();
				fieldName = isToLower ? fieldName.ToLower() : fieldName;
				if(columnsNames.Contains(fieldName)) {
					result.Add(column.FieldName);
					columnsNames.Remove(fieldName);
					removedColumns.Add(column);
				}
			}
			foreach(ColumnBase column in removedColumns)
				if(columnsSource.Contains(column))
					columnsSource.Remove(column);
			this.result.AddRange(result);
		}
		public static bool IsColumnsListsEquals(IList oldColumns, IList newColumns) {
			foreach(ColumnBase column in oldColumns) {
				if(!newColumns.Contains(column))
					return false;
			}
			foreach(ColumnBase column in newColumns) {
				if(!oldColumns.Contains(column))
					return false;
			}
			return true;
		}
		public static ObservableCollection<string> GetSearchColumns(List<ColumnBase> columnsSource, string searchColumns) {
			return new GridColumnListParser(columnsSource, searchColumns).GetSearchColumns();
		}
	}
	[DXToolboxBrowsable(false)]
	public class GridSearchControlBase : SearchControl {
		public static readonly DependencyProperty ViewProperty;
		internal virtual bool IsLogicControl { get { return true; } }
		static GridSearchControlBase() {
			Type ownerType = typeof(GridSearchControlBase);
			ViewProperty = DependencyPropertyManager.Register("View", typeof(DataViewBase), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((GridSearchControlBase)d).ViewChanged((DataViewBase)e.OldValue, (DataViewBase)e.NewValue)));
		}
		public DataViewBase View {
			get { return (DataViewBase)GetValue(ViewProperty); }
			set { SetValue(ViewProperty, value); }
		}
		void ViewChanged(DataViewBase oldView, DataViewBase view) {
			OnViewChanged(oldView, view);
		}
		protected override bool SaveMRUOnStringChanged {
			get { return false; }
		}
		protected void OnViewChanged(DataViewBase oldView, DataViewBase view) {
			if(oldView != null)
				UnsubscribeViewEvens(oldView);
			if(view == null)
				return;
			view.SearchControl = this;
			BindSearchPanel(view);
		}
		protected virtual void UnsubscribeViewEvens(DataViewBase oldView) {
			oldView.SearchControl = null;
		}
		protected virtual void BindSearchPanel(DataViewBase view) {
			Binding bindingSearchTextProperty = new Binding() { Source = view, Path = new PropertyPath(DataViewBase.SearchStringProperty.GetName()), Mode = BindingMode.TwoWay };
			SetBinding(SearchControl.SearchTextProperty, bindingSearchTextProperty);
			Binding bindingSearchPanelSearchConditionProperty = new Binding() { Source = view, Path = new PropertyPath(DataViewBase.SearchPanelFindFilterProperty.GetName()) };
			SetBinding(SearchControl.FilterConditionProperty, bindingSearchPanelSearchConditionProperty);
			Binding bindingSearchPanelFindModeProperty = new Binding() { Source = view, Path = new PropertyPath(DataViewBase.SearchPanelFindModeProperty.GetName()) };
			SetBinding(SearchControl.FindModeProperty, bindingSearchPanelFindModeProperty);
			Binding bindingSearchPanelCriteriaOperatorTypeProperty = new Binding() { Source = view, Path = new PropertyPath(DataViewBase.SearchPanelCriteriaOperatorTypeProperty.GetName()) };
			SetBinding(SearchControl.CriteriaOperatorTypeProperty, bindingSearchPanelCriteriaOperatorTypeProperty);
			Binding bindingSearchDelay = new Binding() { Source = view, Path = new PropertyPath(DataViewBase.SearchDelayProperty.GetName()) };
			SetBinding(SearchControl.SearchTextPostDelayProperty, bindingSearchDelay);
			Binding bindingNullText = new Binding() { Source = view, Path = new PropertyPath(DataViewBase.SearchPanelNullTextProperty.GetName()), Mode = BindingMode.TwoWay };
			SetBinding(SearchControl.NullTextProperty, bindingNullText);
		}
	}
}
