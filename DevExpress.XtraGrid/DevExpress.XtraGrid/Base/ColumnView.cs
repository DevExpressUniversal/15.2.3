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
using System.Windows.Forms;
using System.Drawing;
using System.Data;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using DevExpress.Accessibility;
using DevExpress.Data;
using DevExpress.Data.Helpers;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Localization;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraEditors.Repository;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Controls;	
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Helpers;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Container;
using DevExpress.Utils.Serializing;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.FilterEditor;
using DevExpress.Data.Filtering;
using DevExpress.XtraEditors.Filtering;
using DevExpress.XtraGrid.Views.Base.Handler;
using DevExpress.Data.Filtering.Helpers;
using System.Collections.Generic;
using DevExpress.XtraEditors.Design;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraGrid.Controls;
using DevExpress.XtraGrid.Internal;
using DevExpress.XtraGrid.SearchControl;
using DevExpress.XtraGrid.Accessibility;
namespace DevExpress.XtraGrid.Views.Base {
	public enum ShowFilterPanelMode {
		Default,
		ShowAlways,
		Never
	}
	public enum ShowButtonModeEnum { 
		Default,
		ShowAlways,
		ShowForFocusedRow,
		ShowForFocusedCell,
		ShowOnlyInEditor
	}
	public enum UpdateRowResult {
		Updated,
		Error,
		Canceled
	}
	[Designer("DevExpress.XtraGrid.Design.ColumnViewDesigner, " + AssemblyInfo.SRAssemblyGridDesign)]
	public abstract class ColumnView : BaseView, IDataControllerVisualClient2, IDataControllerSort, IDataControllerData2, IDataControllerThreadClient, IDataControllerCurrentSupport, IColumnViewSearchClient {
		bool findPanelVisible = false;
		string findFilterText = string.Empty;
		GridColumn focusedColumn;
		LoadingAnimator loadingAnimator;
		LoadingPanel loadingPanel;
		Dictionary<GridColumn, ColumnDateFilterInfo> dateFilterCache;
		protected const int InvalidRowHandle = DevExpress.Data.DataController.InvalidRow;
		ViewFilter activeFilter, lastValidFilter;
		MRUViewFilterCollection mruFilters;
		bool activeFilterEnabled;
		bool disableCurrencyManager = false;
		object images;
		ErrorInfo errorInfo;
		BaseView prevActiveView; 
		bool focusedRowModified;
		protected bool lockSelectionInvalidate;
		GridFormatRuleCollection formatRules;
		GridColumnVisibleCollection visibleColumns;
		GridColumnSortInfoCollection sortInfo;
		GridColumnCollection columns;
		string incrementalText;
		int focusedRowHandle;
		int viewCaptionHeight = -1;
		int lastDataRowsVisibleCount = -1, lastSelectedCount = 0;
		GridCell selectionAnchor = null;
		ColumnViewOptionsSelection optionsSelection;
		ColumnViewOptionsBehavior optionsBehavior;
		ColumnViewOptionsFilter optionsFilter;
		ColumnViewOptionsFind optionsFind;
		ColumnViewOptionsView optionsView;
		MRUFilterPopup mruFilterPopup = null;
		FilterPopup filterPopup;
		protected int lockFocusedRowChange = 0;
		internal int calculatedRealViewHeight = -1;
		RowCellAlignmentEventArgs alignmentEventArgs = new RowCellAlignmentEventArgs(0, null, HorzAlignment.Default);
		private static readonly object showingEditor = new object();
		private static readonly object hiddenEditor = new object(), shownEditor = new object();
		private static readonly object startSorting = new object(), endSorting = new object(), startGrouping = new object(), endGrouping = new object();
		private static readonly object selectionChanged= new object();
		private static readonly object columnFilterChanged= new object();
		private static readonly object columnChanged = new object();
		private static readonly object columnUnboundExpressionChanged = new object();
		private static readonly object columnPositionChanged = new object();
		private static readonly object focusedRowChanged = new object();
		private static readonly object focusedRowObjectChanged = new object();
		private static readonly object focusedColumnChanged = new object();
		private static readonly object cellValueChanged = new object();
		private static readonly object cellValueChanging = new object();
		private static readonly object initNewRow = new object();
		private static readonly object rowCellDefaultAlignment = new object();
		private static readonly object invalidRowException = new object();
		private static readonly object customDrawEmptyForeground = new object();
		private static readonly object customDrawFilterPanel = new object();
		private static readonly object customColumnSort = new object();
		private static readonly object customUnboundColumnData = new object();
		private static readonly object customColumnDisplayText = new object();
		private static readonly object rowDeleting = new object();
		private static readonly object rowDeleted = new object();
		private static readonly object dataManagerReset = new object();
		private static readonly object beforeLeaveRow = new object();
		private static readonly object validateRow = new object();
		private static readonly object rowUpdated = new object();
		private static readonly object rowLoaded = new object();
		private static readonly object asyncCompleted = new object();
		private static readonly object focusedRowLoaded = new object();
		private static readonly object customRowFilter = new object();
		private static readonly object substituteFilter = new object();
		private static readonly object substituteSortInfo = new object();
		private static readonly object showFilterPopupListBox = new object();
		private static readonly object showFilterPopupCheckedListBox = new object();
		private static readonly object showFilterPopupDate = new object();
		private static readonly object customFilterDialog= new object();
		private static readonly object customFilterDisplayText = new object();
		private static readonly object filterEditorCreated = new object();
		private static readonly object unboundExpressionEditorCreated = new object();
		public ColumnView() {
			this.dateFilterCache = new Dictionary<GridColumn, ColumnDateFilterInfo>();
			this.activeFilter = new ViewFilter();
			this.lastValidFilter = new ViewFilter();
			this.mruFilters = new MRUViewFilterCollection();
			this.activeFilter.Changed += new EventHandler(OnActiveFilterChanged);
			this.activeFilterEnabled = true;
			this.images = null;
			this.lockSelectionInvalidate = false;
			this.sortInfo = new GridColumnSortInfoCollection(this);
			this.focusedRowModified = false;
			this.errorInfo = new ErrorInfoEx();
			this.errorInfo.Changed += new EventHandler(OnErrorInfo_Changed);
			this.prevActiveView = null;
			this.focusedColumn = null;
			this.filterPopup = null;
			this.incrementalText = "";
			this.focusedRowHandle = DevExpress.Data.DataController.InvalidRow;
			this.visibleColumns = new GridColumnVisibleCollection(this);
			this.columns = CreateColumnCollection();
			this.columns.CollectionChanged += new CollectionChangeEventHandler(OnColumnsCollectionChanged);
			this.optionsSelection = CreateOptionsSelection();
			this.optionsBehavior = CreateOptionsBehavior();
			this.optionsFilter = CreateOptionsFilter();
			this.optionsFind = CreateOptionsFind();
			this.optionsView = CreateOptionsView();
			this.optionsView.Changed += new BaseOptionChangedEventHandler(OnOptionChanged); 
			this.optionsSelection.Changed += new BaseOptionChangedEventHandler(OnOptionChanged); 
			this.optionsBehavior.Changed += new BaseOptionChangedEventHandler(OnOptionChanged); 
			this.optionsFilter.Changed += new BaseOptionChangedEventHandler(OnOptionChanged);
			this.optionsFind.Changed += new BaseOptionChangedEventHandler(OnOptionChanged);
			this.formatRules = CreateFormatRuleCollection();
		}
		protected override void SetupDataController() {
			base.SetupDataController();
			DataControllerCore.AddThreadClient(this);
			DataControllerCore.DataClient = this;
			DataControllerCore.SortClient = this;
			DataControllerCore.VisualClient = this;
			DataControllerCore.CurrentClient = this;
			DataControllerCore.SelectionChanged += new SelectionChangedEventHandler(OnDataController_SelectionChanged);
			DataControllerCore.RowDeleted += OnDataController_RowDeleted;
			DataControllerCore.RowDeleting += OnDataController_RowDeleting;
		}
		void OnDataController_RowDeleting(object sender, RowDeletingEventArgs e) {
			RowDeletingEventHandler handler = (RowDeletingEventHandler)this.Events[rowDeleting];
			if(handler != null) handler(this, e);
		}
		void OnDataController_RowDeleted(object sender, RowDeletedEventArgs e) {
			RowDeletedEventHandler handler = (RowDeletedEventHandler)this.Events[rowDeleted];
			if(handler != null) handler(this, e);
		}
		protected virtual GridFormatRuleCollection CreateFormatRuleCollection() {
			return new GridFormatRuleCollection(this);
		}
		protected internal virtual void OnRuleCollectionChanged(FormatConditionCollectionChangedEventArgs e) {
			if(!FormatRules.CheckAllValuesReady())
				UpdateFormatRulesSummary();
			LayoutChanged();
			FireChanged();
		}
		protected virtual void SyncFormatRulesSummary() {
			UpdateFormatRulesSummary();
		}
		protected virtual void UpdateFormatRulesSummary() {
			SummaryItem[] items;
			FormatRules.SummaryInfo.Apply(DataController, out items);
			FormatRules.TryUpdateStateValues();
		}
		protected virtual void EnsureRuleValueProviders() {
			if(!FormatRules.HasValidRules) return;
			FormatRules.EnsureValueProviders();
		}
		protected internal override void DestroyDataController() {
			if(DataControllerCore != null) {
				DataControllerCore.SelectionChanged -= new SelectionChangedEventHandler(OnDataController_SelectionChanged);
			}
			base.DestroyDataController();
		}
		protected DataControllerType requireDataControllerType = DataControllerType.Regular;
		protected internal override void CheckRecreateDataController(object dataSource) {
			this.requireDataControllerType = DataControllerType.Regular;
			if(DisableCurrencyManager) this.requireDataControllerType = DataControllerType.RegularNoCurrencyManager;
			if(DataControllerCore == null) return;
			if(dataSource != null && GridControl != null && GridControl.GetIsServerMode(dataSource)) {
				this.requireDataControllerType = DetectServerModeType(dataSource);
				if(DisableCurrencyManager && this.requireDataControllerType == DataControllerType.Regular) this.requireDataControllerType = DataControllerType.RegularNoCurrencyManager;
			}
			DataControllerType current = DataControllerType.RegularNoCurrencyManager;
			if(DataControllerCore is CurrencyDataController) current = DataControllerType.Regular;
			if(DataControllerCore is ServerModeDataController) current = DataControllerType.ServerMode;
			if(DataControllerCore is AsyncServerModeDataController) current = DataControllerType.AsyncServerMode;
			if(current != requireDataControllerType)
				RecreateDataController();
		}
		protected override BaseGridController CreateDataController() {
			if(useClonedDataController != null) return useClonedDataController;
			if(this.requireDataControllerType == DataControllerType.AsyncServerMode) return new AsyncServerModeDataController();
			if(this.requireDataControllerType == DataControllerType.ServerMode) return new ServerModeDataController();
			if(this.requireDataControllerType == DataControllerType.RegularNoCurrencyManager) return new GridDataController();
			return new CurrencyDataController();
		}
		protected override OptionsLayoutBase CreateOptionsLayout() { return new OptionsLayoutGrid(); }
		protected virtual ColumnViewOptionsBehavior CreateOptionsBehavior() { return new ColumnViewOptionsBehavior(this); }
		protected virtual ColumnViewOptionsFilter CreateOptionsFilter() { return new ColumnViewOptionsFilter(); }
		protected virtual ColumnViewOptionsFind CreateOptionsFind() { return new ColumnViewOptionsFind(); }
		protected virtual ColumnViewOptionsSelection CreateOptionsSelection() { return new ColumnViewOptionsSelection(); }
		protected virtual ColumnViewOptionsView CreateOptionsView() { return new ColumnViewOptionsView(); }
		protected virtual void OnOptionChanged(object sender, BaseOptionChangedEventArgs e) {
			if(sender == OptionsBehavior) {
				UpdateDataControllerOptions();
			}
			if(sender == OptionsSelection) {
				if(e.Name == "MultiSelect") DataController.Selection.Clear();
			}
			if(sender == OptionsFind) {
				if(e.Name != "FindFilterColumns" && e.Name != "HighlightFindResults") DestroyFindPanel();
				if(e.Name == "FindFilterColumns" && IsFindFilterActive) ApplyColumnsFilterCore();
			}
			UpdateNavigator();
			OnPropertiesChanged();
			FireChanged();
		}
		protected internal virtual void AssignActiveFilterFromFilterBuilder(CriteriaOperator newCriteria) {
			ViewFilter newFilter = new ViewFilter(this, newCriteria);
			ActiveFilter.Assign(newFilter, Columns);
		}
		protected virtual void OnActiveFilterChanged(object sender, EventArgs e) {
			ApplyColumnsFilter();
			if(this.GridControl != null)
				this.GridControl.OnFilterChanged();
		}
		protected virtual GridColumnCollection CreateColumnCollection() {
			return new GridColumnCollection(this);
		}
		public virtual int LocateByDisplayText(int startRowHandle, GridColumn column, string text) {
			if(column == null) return GridControl.InvalidRowHandle;
			startRowHandle = Math.Max(0, startRowHandle);
			DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo bev = CreateColumnEditViewInfo(column, 0);
			try {
				for(int n = startRowHandle; n < DataController.VisibleListSourceRowCount; n++) {
					string cellText = GetRowCellDisplayTextCore(n, column, bev, GetRowCellValue(n, column), false);
					if(string.Equals(cellText, text)) return n;
				}
			}
			catch { }
			return GridControl.InvalidRowHandle;
		}
		public int LocateByValue(string fieldName, object val, params OperationCompleted[] completed) { 
			foreach(GridColumn column in Columns) {
				if(column.FieldName == fieldName)
					return LocateByValue(0, column, val, completed);
			}
			try {
				if(!DataController.IsReady) return GridControl.InvalidRowHandle;
				return DataController.FindRowByValue(fieldName, val, completed);
			}
			catch { }
			return GridControl.InvalidRowHandle;
		}
		public virtual int LocateByValue(int startRowHandle, GridColumn column, object val, params OperationCompleted[] completed) {
			if(column == null || !DataController.IsReady) return GridControl.InvalidRowHandle;
			startRowHandle = Math.Max(0, startRowHandle);
			if(IsServerMode) {
				if(startRowHandle != 0) throw new ArgumentException("Argument must be '0' in server mode.", "startRowHandle");
			}
			try {
				if(IsServerMode) return DataController.FindRowByValue(column.FieldName, val, completed);
				for(int n = startRowHandle; n < DataController.VisibleListSourceRowCount; n++) {
					object cellValue = GetRowCellValue(n, column);
					if(object.Equals(val, cellValue)) return n;
				}
			}
			catch { }
			return GridControl.InvalidRowHandle;
		}
		protected ViewFilter LastValidFilter { get { return lastValidFilter; } set { lastValidFilter = value; } }
		void XtraAssignActiveFilter(object val) {
			DestroyFilterData();
			ViewFilter filter = val as ViewFilter;
			if(filter == null) return;
			filter.OnDeserialize(Columns);
			ActiveFilter.Assign(filter, null);
		}
		void XtraAssignMRUFilters(object val) {
			MRUViewFilterCollection filters = val as MRUViewFilterCollection;
			if(filters == null) return;
			filters.OnDeserialize(Columns);
			MRUFilters.Assign(filters);
		}
		bool XtraShouldSerializeActiveFilter() { return false; }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), XtraSerializableProperty(XtraSerializationFlags.DefaultValue | XtraSerializationFlags.UseAssign, 1000), XtraSerializablePropertyId(LayoutIdData)]
		public ViewFilter ActiveFilter { get { return activeFilter; } }
		string defferedActiveFilterString;
		void ApplyDefferedActiveFilterString() {
			if(defferedActiveFilterString == null)
				return;
			string val = defferedActiveFilterString;
			defferedActiveFilterString = null;
			ActiveFilterString = val;
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool DisableCurrencyManager {
			get { return disableCurrencyManager; }
			set {
				if(DisableCurrencyManager == value) return;
				disableCurrencyManager = value;
				RecreateDataController();
			}
		}
		[XtraSerializableProperty(1000), XtraSerializablePropertyId(LayoutIdData)]
		[Browsable(false)]
		[DefaultValue("")]
		public string ActiveFilterString {
			get { return ActiveFilter.Expression; }
			set {
				if(IsLoading && !string.IsNullOrEmpty(value)) {
					this.defferedActiveFilterString = value;
					return;
				}
				this.ActiveFilterCriteria = CriteriaOperator.TryParse(value);
			}
		}
		[XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public CriteriaOperator ActiveFilterCriteria {
			get { return ActiveFilter.Criteria; }
			set {
				ViewFilter newFilter = new ViewFilter(this, value);
				ActiveFilter.Assign(newFilter, this.Columns);
			}
		}
		bool XtraShouldSerializeMRUFilters() { return MRUFilters.Count > 0; }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), XtraSerializableProperty(XtraSerializationFlags.DefaultValue | XtraSerializationFlags.UseAssign, 1000), XtraSerializablePropertyId(LayoutIdData)]
		public MRUViewFilterCollection MRUFilters { get { return mruFilters; } }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewActiveFilterEnabled"),
#endif
 DefaultValue(true), DXCategory(CategoryName.Behavior), XtraSerializableProperty(), XtraSerializablePropertyId(LayoutIdData)]
		public virtual bool ActiveFilterEnabled {
			get { return activeFilterEnabled; }
			set {
				if(ActiveFilterEnabled == value) return;
				if(!IsLoading && !IsDeserializing && !UpdateCurrentRowAction()) return;
				activeFilterEnabled = value;
				OnActiveFilterEnabledChanged();
			}
		}
		[Browsable(false)]
		public virtual bool IsMultiSelect { get { return OptionsSelection.MultiSelect; } }
		protected virtual void OnFilterEditorCreated(FilterControlEventArgs e) {
			FilterControlEventHandler handler = (FilterControlEventHandler)this.Events[filterEditorCreated];
			if(handler != null)
				handler(this, e);
		}
		protected virtual void OnUnboundExpressionEditorCreated(UnboundExpressionEditorEventArgs e) {
			UnboundExpressionEditorEventHandler handler = (UnboundExpressionEditorEventHandler)this.Events[unboundExpressionEditorCreated];
			if(handler != null)
				handler(this, e);
		}
		public virtual void ShowFilterEditor(GridColumn defaultColumn) {
			if(!UpdateCurrentRowAction()) return;
			using(FilterColumnCollection filterColumns = this.CreateFilterColumnCollection()) {
				FilterColumn filterColumn = GridCriteriaHelper.GetFilterColumnByGridColumn(filterColumns, defaultColumn);
				using(Form builder = CreateFilterBuilderDialog(filterColumns, filterColumn)) {
					if(this.WorkAsLookup)
						builder.TopMost = true;
					FilterControlEventArgs ea = new FilterControlEventArgs(builder as FilterBuilder, ((FilterBuilder)builder).GetIFilterControl());
					OnFilterEditorCreated(ea);
					if(ea.ShowFilterEditor)
						builder.ShowDialog(this.GridControl);
				}
			}
		}
		protected virtual bool UpdateCurrentRowAction() { 
			return UpdateCurrentRow();
		}
		protected virtual Form CreateFilterBuilderDialog(FilterColumnCollection filterColumns, FilterColumn defaultFilterColumn) {
			FilterBuilder result = new FilterBuilder(
								filterColumns,
								this.GridControl.MenuManager,
								this.GridControl.LookAndFeel,
								this,
								defaultFilterColumn);
			return result;
		}
		protected internal virtual void OnFilterCustomizeClick() {
			this.ShowFilterEditor(null);
		}
		protected void RestoreActiveFilter() { 
			ActiveFilter.BeginUpdate();
			try {
				ActiveFilter.Assign(LastValidFilter, null);
			} finally {
				ActiveFilter.CancelUpdate();
			}
		}
		protected internal override void OnLookAndFeelChanged() {
			FormatRules.ResetVisualCacheInternal();
			base.OnLookAndFeelChanged();
		}
		protected override void SetupInfo() {
			FormatRules.ResetVisualCacheInternal();
			base.SetupInfo();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void ConvertFormatConditionToFormatRules() {
			if(FormatConditions.Count == 0) return;
			BeginUpdate();
			try {
				FormatRules.BeginUpdate();
				foreach(StyleFormatCondition condition in FormatConditions) {
					GridFormatRule format = ConvertFormatConditionToFormatRule(condition);
					FormatRules.Add(format);
				}
				FormatConditions.Clear();
			}
			finally {
				FormatRules.EndUpdate();
				EndUpdate();
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		internal GridFormatRule ConvertFormatConditionToFormatRule(StyleFormatCondition condition) {
			GridFormatRule format = new GridFormatRule();
			format.ApplyToRow = condition.ApplyToRow;
			format.Enabled = condition.Enabled;
			format.Tag = condition.Tag;
			format.Name = condition.Name;
			if(condition.Column != null) format.Column = condition.Column;
			else
				if(!string.IsNullOrEmpty(condition.ColumnName)) format.ColumnName = condition.ColumnName;
			if(condition.Condition == FormatConditionEnum.Expression) {
				FormatConditionRuleExpression ruleExpression = new FormatConditionRuleExpression() { Expression = condition.Expression };
				ruleExpression.Appearance.Assign(condition.Appearance);
				format.Rule = ruleExpression;
			} else {
				FormatConditionRuleValue ruleValue = new FormatConditionRuleValue() { Condition = (FormatCondition)condition.Condition, Value1 = condition.Value1, Value2 = condition.Value2 };
				ruleValue.Appearance.Assign(condition.Appearance);
				format.Rule = ruleValue;
			}
			return format;
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Browsable(true),DXCategory(CategoryName.Appearance),
		 XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true, 1000, XtraSerializationFlags.DefaultValue), XtraSerializablePropertyId(LayoutIdFormatRules)]
		public virtual GridFormatRuleCollection FormatRules { get { return formatRules; } }
		internal void XtraClearFormatRules(XtraItemEventArgs e) { FormatRules.Clear(); }
		internal object XtraCreateFormatRulesItem(XtraItemEventArgs e) { return FormatRules.AddInstance(); }
		bool ShouldSerializeOptionsView() { return OptionsView.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewOptionsView"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), XtraSerializablePropertyId(LayoutIdOptionsView)]
		public ColumnViewOptionsView OptionsView { get { return optionsView; } }
		bool ShouldSerializeOptionsLayout() { return OptionsLayout.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewOptionsLayout"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public new OptionsLayoutGrid OptionsLayout { get { return base.OptionsLayout as OptionsLayoutGrid; } }
		bool ShouldSerializeOptionsSelection() { return OptionsSelection.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewOptionsSelection"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public ColumnViewOptionsSelection OptionsSelection { get { return optionsSelection; } }
		bool ShouldSerializeOptionsBehavior() { return OptionsBehavior.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewOptionsBehavior"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public ColumnViewOptionsBehavior OptionsBehavior { get { return optionsBehavior; } }
		bool ShouldSerializeOptionsFilter() { return OptionsFilter.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewOptionsFilter"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public ColumnViewOptionsFilter OptionsFilter { get { return optionsFilter; } }
		bool ShouldSerializeOptionsFind() { return OptionsFind.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewOptionsFind"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public ColumnViewOptionsFind OptionsFind { get { return optionsFind; } }
		[Browsable(false), 
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewIsShowFilterPanel")
#else
	Description("")
#endif
]
		public virtual bool IsShowFilterPanel {
			get {
				if(OptionsView.ShowFilterPanelMode == ShowFilterPanelMode.Never) return false;
				if(OptionsView.ShowFilterPanelMode == ShowFilterPanelMode.ShowAlways) return true;
				return !ActiveFilter.IsEmpty; 
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewViewCaptionHeight"),
#endif
 DefaultValue(-1), DXCategory(CategoryName.Appearance), XtraSerializableProperty()]
		public virtual int ViewCaptionHeight {
			get { return viewCaptionHeight; }
			set {
				if(value < -1) value = -1;
				if(ViewCaptionHeight == value) return;
				viewCaptionHeight = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewImages"),
#endif
 DefaultValue(null), DXCategory(CategoryName.Appearance), TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter))]
		public virtual object Images {
			get { return images; }
			set {
				if(Images == value) return;
				images = value;
				OnPropertiesChanged();
			}
		}
		[Browsable(false)]
		public override bool Editable { get { return OptionsBehavior.Editable; } }
		[Browsable(false), Obsolete(ObsoleteText.SRColumnView_FilterPopupMaxRecordsCount), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int FilterPopupMaxRecordsCount {
			get { return OptionsFilter.ColumnFilterPopupMaxRecordsCount; } 
			set { OptionsFilter.ColumnFilterPopupMaxRecordsCount = value; }
		}
		[Browsable(false), Obsolete(ObsoleteText.SRColumnView_FilterPopupRowCount), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int FilterPopupRowCount { 
			get { return OptionsFilter.ColumnFilterPopupRowCount; }
			set { OptionsFilter.ColumnFilterPopupRowCount = value; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), Obsolete(ObsoleteText.SRObsoleteDefaultEdit)]
		public RepositoryItem DefaultEdit {
			get { return null; }
			set { }
		}
		public string GetIncrementalText() { return IncrementalText;}
		protected internal virtual string IncrementalText { 
			get { return incrementalText; } 
			set { 
				if(value == null) value = "";
				if(IncrementalText == value) return;
				incrementalText = value; 
				RefreshRow(FocusedRowHandle);
			} 
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Obsolete(ObsoleteText.SRColumnView_ShowButtonMode)]
		public ShowButtonModeEnum ShowButtonMode {
			get { return OptionsView.ShowButtonMode; }
			set {
				if(value == ShowButtonModeEnum.ShowForFocusedCell) value = ShowButtonModeEnum.Default;
				OptionsView.ShowButtonMode = value;
			}
		}
		internal void XtraClearColumns(XtraItemEventArgs e) {
			OptionsLayoutGrid optGrid = e.Options as OptionsLayoutGrid;
			bool addNewColumns = (optGrid != null && optGrid.Columns.AddNewColumns);
			if(e.Item.ChildProperties == null || e.Item.ChildProperties.Count == 0) {
				if(!addNewColumns) Columns.DestroyColumns();
				return;
			}
			ArrayList list = new ArrayList();
			foreach(DevExpress.Utils.Serializing.Helpers.XtraPropertyInfo xp in e.Item.ChildProperties) {
				object col = XtraFindColumnsItem(new XtraItemEventArgs(this, Columns, xp));
				if(col != null) list.Add(col);
			}
			for(int n = Columns.Count - 1; n >= 0; n--) {
				GridColumn col = Columns[n];
				if(!list.Contains(col)) {
					if(addNewColumns) continue;
					col.Dispose();
				}
			}
		}
		internal object XtraCreateColumnsItem(XtraItemEventArgs e) {
			OptionsLayoutGrid optGrid = e.Options as OptionsLayoutGrid;
			if(optGrid != null) {
				if(optGrid.Columns.RemoveOldColumns) return null;
				if(!optGrid.Columns.StoreAllOptions) return null;
			}
			GridColumn column = Columns.Add();
			column.Visible = true;
			return column;
		}
		internal object XtraFindColumnsItem(XtraItemEventArgs e) {
			if(e.Item.ChildProperties == null) return null;
			string name = null;
			DevExpress.Utils.Serializing.Helpers.XtraPropertyInfo xp = e.Item.ChildProperties["Name"];
			if(xp != null && xp.Value != null) name = xp.Value.ToString();
			if(name == null || name == string.Empty) return null;
			GridColumn col = Columns.ColumnByName(name);
			return col;
		}
		internal void XtraSetIndexColumnsItem(XtraSetItemIndexEventArgs e) { 
			if(e.Item == null) return;
			int index = Columns.IndexOf(e.Item.Value as GridColumn);
			if(index == -1 || index == e.NewIndex) return;
			Columns.SetItemIndex(e.Item.Value as GridColumn, e.NewIndex);
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
#if DXWhidbey
#endif
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, true, true, 0, XtraSerializationFlags.DefaultValue), XtraSerializablePropertyId(LayoutIdColumns)]
		public virtual GridColumnCollection Columns { get { return columns; } } 
		[Browsable(false)]
		public virtual string FilterPanelText { get { return GetFilterDisplayText(ActiveFilter); } }
		[Browsable(false)]
		[Obsolete("Use the ActiveFilter.Count or ActiveFilter.IsEmpty property instead")]
		public int FilteredColumnsCount { get { return ActiveFilter.Count; } }
		[Browsable(false)]
		public string RowFilter { get { return ActiveFilter.Expression; } }
		[Browsable(false)]
		public override BaseEdit ActiveEditor {	get { return GridControl == null ? null : GridControl.EditorHelper.ActiveEditor; } }
		[Browsable(false)]
		public GridColumnReadOnlyCollection SortedColumns { 
			get { 
				GridColumnReadOnlyCollection sorted = new GridColumnReadOnlyCollection(this);
				for(int n = SortInfo.GroupCount; n < SortInfo.Count; n++) {
					sorted.AddCore(SortInfo[n].Column);
				}
				return sorted;
			} 
		}
		[Browsable(false)]
		public GridColumnReadOnlyCollection GroupedColumns { 
			get { 
				GridColumnReadOnlyCollection res = new GridColumnReadOnlyCollection(this);
				for(int n = 0; n < SortInfo.GroupCount; n++) {
					res.AddCore(SortInfo[n].Column);
				}
				return res;
			} 
		}
		protected override bool CanActionScroll(ScrollNotifyAction action) {
			if(action == ScrollNotifyAction.MouseMove) {
				if(ActiveEditor != null) {
					PopupBaseEdit pb = ActiveEditor as PopupBaseEdit;
					if(pb != null && pb.IsPopupOpen) return false;
				}
			}
			return base.CanActionScroll(action);
		}
		bool XtraShouldSerializeSortInfo() { return SortInfo.Count > 0; }
		void XtraAssignSortInfo(object val) {
			GridColumnSortInfoCollection sortInfo = val as GridColumnSortInfoCollection;
			if(sortInfo == null) return;
			sortInfo.OnDeserialize(Columns);
			SortInfo.Assign(sortInfo, null);
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Browsable(false), XtraSerializableProperty(XtraSerializationFlags.UseAssign, 1000), XtraSerializablePropertyId(LayoutIdData)]
		public virtual GridColumnSortInfoCollection SortInfo { get { return sortInfo; } }
		[Browsable(false), DefaultValue(0)]
		public int GroupCount { get { return SortInfo.GroupCount; } set { SortInfo.GroupCount = value; } }
		[Browsable(false)]
		public virtual GridColumnReadOnlyCollection VisibleColumns { 
			get {
				if(visibleColumns.IsDirty) {
					RefreshVisibleColumnsIndexes();
					visibleColumns.SetDirty(false);
				}
				return VisibleColumnsCore; 
			}
		}
		protected internal virtual Image GetMaxHeightColumnImage() {
			Image res = null;
			for(int n = 0; n < Math.Min(ColumnViewInfo.AutoHeightCalculateMaxColumnCount, VisibleColumns.Count); n++) {
				GridColumn column = VisibleColumns[n];
				if(res == null || (column.Image != null && column.Image.Height > res.Height)) res = column.Image;
			}
			return res;
		}
		protected internal virtual GridColumnVisibleCollection VisibleColumnsCore { get { return visibleColumns; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int FocusedRowHandle {  
			get { return focusedRowHandle;}
			set {
				DoChangeFocusedRowInternal(value, true);
			}
		}
		internal int FocusedListSourceIndex { get { return GetDataSourceRowIndex(FocusedRowHandle); } }
		protected void SetFocusedRowHandleCore(int val) {
			this.focusedRowHandle = val;
		}
		[Browsable(false)]
		public bool IsFirstRow {
			get {
				return FocusedRowHandle == DevExpress.Data.DataController.InvalidRow ||
					FocusedRowHandle == GetVisibleRowHandle(0);
			}
		}
		[Browsable(false)]
		public override bool IsFocusedView { 
			get { 
				if(IsKeyboardFocused) {
					if(GridControl.Focused || IsEditorFocused || IsFindFilterFocused ||
						(this.filterPopup != null && this.filterPopup.IsFocused) ||
						(MRUFilterPopup != null && MRUFilterPopup.IsFocused) || IsElementsContainsFocus)
						return true;
				}
				return false;
			}
		}
		protected virtual bool IsElementsContainsFocus { get { return false; } }
		[Browsable(false)]
		public virtual bool CanShowEditor {
			get { 
				if(!Editable || IsAsyncInProgress) return false;
				if(!IsFocusedRowLoaded) return false;
				if(!RaiseShowingEditor()) return false;
				return true;
			}
		}
		[Browsable(false)]
		public bool IsFocusedRowLoaded { get { return IsRowLoaded(FocusedRowHandle); } }
		public virtual bool IsRowLoaded(int rowHandle) {
			if(!IsServerMode) return true;
			return DataController.IsRowLoaded(rowHandle);
		}
		public virtual void LoadRow(int rowHandle) {
			DataController.LoadRow(rowHandle);
		}
		public virtual void LoadRowHierarchy(int rowHandle, OperationCompleted completed) {
			DataController.LoadRowHierarchy(rowHandle, completed);
		}
		public virtual void EnsureRowLoaded(int rowHandle, OperationCompleted completed) {
			DataController.EnsureRowLoaded(rowHandle, completed);
		}
		protected virtual bool GetAllowAddRows() {
			if(WorkAsLookup || IsAsyncInProgress) return false;
			if(!DataController.AllowNew || !DataController.AllowEdit) return false;
			if(OptionsBehavior.AllowAddRows == DefaultBoolean.Default) return Editable;
			return OptionsBehavior.AllowAddRows == DefaultBoolean.True;
		}
		protected virtual bool GetAllowDeleteRows() {
			if(WorkAsLookup) return false;
			if(!DataController.AllowRemove) return false;
			if(OptionsBehavior.AllowDeleteRows == DefaultBoolean.Default) return Editable;
			return OptionsBehavior.AllowDeleteRows == DefaultBoolean.True;
		}
		protected internal virtual bool GetCanShowEditor(GridColumn column) {
			if(FocusedRowHandle == DevExpress.Data.DataController.InvalidRow) return false;
			if(column == null || !column.OptionsColumn.AllowEdit) return false;
			if(column.RealColumnEdit != null && !column.RealColumnEdit.Editable) return false;
			return CanShowEditor;
		}
		[Browsable(false)]
		public bool IsEmpty { get { return DataController.VisibleCount == 0; } }
		[Browsable(false)]
		public bool IsLastRow {	get { return IsEmpty || GetVisibleIndex(FocusedRowHandle) == DataController.VisibleCount - 1; } }
		[Browsable(false)]
		public bool IsLastVisibleRow { get {return IsEmpty || FocusedRowHandle == GetVisibleRowHandle(RowCount - 1); } }
		[Browsable(false)]
		public bool IsEditorFocused {
			get {
				if(ActiveEditor != null) {
					DevExpress.XtraEditors.BaseEdit be = ActiveEditor as DevExpress.XtraEditors.BaseEdit;
					return be.EditorContainsFocus;
				}
				return false;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual GridColumn FocusedColumn {
			get { return focusedColumn;}
			set {
				if(FocusedColumn == value) return;
				GridColumn prevFocusedColumn = FocusedColumn;
				CloseEditor();
				focusedColumn = value;
				DoAfterFocusedColumnChanged(prevFocusedColumn, FocusedColumn);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual object FocusedValue {
			get {
				if(FocusedColumn == null || FocusedRowHandle == GridControl.InvalidRowHandle) return null;
				return GetRowCellValue(FocusedRowHandle, FocusedColumn);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual object EditingValue {
			get {
				if(ActiveEditor == null) return null;
				DevExpress.XtraEditors.BaseEdit be = ActiveEditor as DevExpress.XtraEditors.BaseEdit;
				if(be != null) {
					return be.EditValue;
				}
				Control rc = ActiveEditor as Control;
				return rc.Text;
			}
			set {
				DevExpress.XtraEditors.BaseEdit be = ActiveEditor as DevExpress.XtraEditors.BaseEdit;
				if(be != null) {
					be.EditValue = value;
				}
			}
		}
		protected internal override bool ValidateEditing() {
			if(EditingValueModified) {
				if(!ValidateEditor()) return false;
			}
			CloseEditor(false);
			return EndEditOnLeave();
		}
		[Browsable(false)]
		public virtual bool EditingValueModified {
			get {
				if(!IsEditing) return false;
				DevExpress.XtraEditors.BaseEdit baseEdit = ActiveEditor as DevExpress.XtraEditors.BaseEdit;
				if(baseEdit != null) {
					return baseEdit.IsModified;
				}
				return false; 
			}
		}
		[Browsable(false)]
		public virtual bool FocusedRowModified { get { return focusedRowModified; }	}
		protected internal override bool IsAllowZoomDetail { get { return true; } }
		public override void NormalView() {
			if(!IsDetailView) {
				BaseView view = this.prevActiveView == null ? GridControl.MainView : this.prevActiveView;
				this.prevActiveView = null;
				GridControl.NormalView(view);
				return;
			}
			if(!IsZoomedView) return;
			if(ParentView != null && ParentView.IsAutoCollapseDetail && !ParentView.CanCollapseMasterRow(SourceRowHandle)) return;
			if(this.prevActiveView != null && this.prevActiveView.DetailLevel >= DetailLevel) {
				this.prevActiveView = null;
				if(this.ParentView != null) this.prevActiveView = this.ParentView;
			}
			BeginUpdate();
			try {
				SetViewRect(Rectangle.Empty);
				BaseView view = this.prevActiveView == null ? GridControl.MainView : this.prevActiveView;
				GridControl.NormalView(view);
			}
			finally {
				this.prevActiveView = null;
				if(ParentView.IsAutoCollapseDetail) {
					ParentView.CollapseDetails(SourceRowHandle);
				}
				else
					EndUpdate();
			}
		}
		protected internal override void ZoomView(BaseView prevView) {
			if(!IsDetailView || IsZoomedView) return;
			if(!ParentView.IsAllowZoomDetail) return;
			BeginUpdate();
			try {
				this.prevActiveView = prevView == null ? GridControl.DefaultView : prevView;
				GridControl.ZoomView(this);
			}
			finally {
				EndUpdateCore(true);
			}
		}
		[Browsable(false)]
		public override bool IsZoomedView {
			get {
				return IsDetailView && GridControl.DefaultView == this;
			}
		}
		protected internal override void OnLoaded() {
			ApplyDefferedActiveFilterString();
			BeginUpdate();
			try {
				base.OnLoaded();
			}
			finally {
				EndUpdate();
			}
		}
		protected virtual bool CanColumnDoServerAction(GridColumn column, ColumnServerActionType action) {
			IColumnsServerActions actions = DataController as IColumnsServerActions;
			return actions != null ? actions.AllowAction(IsSortGroupAction(action) ? column.SortFieldName : column.FieldName, action) : true;
		}
		protected bool IsSortGroupAction(ColumnServerActionType action) {
			return action == ColumnServerActionType.Group || action == ColumnServerActionType.Sort;
		}
		protected bool DataController_CanSortColumn(GridColumn column) {
			return DataController.CanSortColumn(column.SortFieldName) &&
				CanColumnDoServerAction(column, ColumnServerActionType.Sort);
		}
		protected internal virtual bool CanDataFilterColumn(GridColumn column) {
			return CanColumnDoServerAction(column, ColumnServerActionType.Filter);
		}
		protected internal virtual bool CanDataSortColumn(GridColumn column) { 
			if(column == null) return false;
			if(DesignMode || !DataController.IsReady || DataController_CanSortColumn(column)) {
				return true;
			}
			if(!IsInitialized) return true;
			return false;
		} 
		protected internal virtual bool CanDataGroupColumn(GridColumn column) {
			return CanDataSortColumn(column);
		}
		protected internal virtual bool CanDataSummaryColumn(GridColumn column) {
			return CanDataSortColumn(column);
		} 
		protected internal virtual bool IsColumnAllowFilter(GridColumn column) {
			return column.OptionsFilter.AllowFilter && CanDataFilterColumn(column);
		}
		protected internal virtual bool IsColumnFieldNameSortGroupExist(GridColumn column) {
			if(!column.FilterBySortFieldName) return false;
			if(string.IsNullOrEmpty(column.FieldNameSortGroup) || column.FieldNameSortGroup == column.FieldName) return false;
			return this.Columns[column.FieldNameSortGroup] != null;
		}
		protected internal virtual bool IsParentColumnFieldNameSortGroupExist(GridColumn column) {
			foreach(GridColumn parent in Columns) {
				if(parent.FilterBySortFieldName && parent.FieldNameSortGroup == column.FieldName) return true;
			}
			return false;
		}
		protected internal virtual GridColumn GetParentColumnFieldNameSortGroup(GridColumn column) {
			foreach(GridColumn parent in Columns) {
				if(parent.FilterBySortFieldName && parent.FieldNameSortGroup == column.FieldName) return parent;
			}
			return column;
		}
		protected internal virtual GridColumn GetColumnFieldNameSortGroup(GridColumn column) {
			if(IsColumnFieldNameSortGroupExist(column)) return this.Columns[column.FieldNameSortGroup];
			return column;
		}
		protected internal virtual bool IsColumnAllowAutoFilter(GridColumn column) {
			RepositoryItem item = GetRowCellRepositoryItem(GridControl.AutoFilterRowHandle, column);
			if(item != null && !item.AllowInplaceAutoFilter) return false;
			return column.OptionsFilter.AllowAutoFilter && CanDataFilterColumn(column);
		}
		public virtual bool CanGroupColumn(GridColumn column) {	return false; }
		public virtual bool CanResizeColumn(GridColumn column) { return false; }
		public virtual bool CanSortColumn(GridColumn column) { 
			if(!CanDataSortColumn(column)) return false;
			if(column == null) return false;
			 if(!column.GetAllowSort()) return false;
			if(!IsInitialized) return true;
			return true;
		}
		public virtual GridColumn GetVisibleColumn(int visibleIndex) {
			if(visibleIndex < 0 || visibleIndex >= VisibleColumns.Count) return null;
			return VisibleColumns[visibleIndex] as GridColumn;
		}
		public override void SynchronizeData(BaseView viewSource) {
			BeginSynchronization();
			BeginDataUpdate();
			try {
				base.SynchronizeData(viewSource);
				ColumnView cv = viewSource as ColumnView;
				if(cv == null) return;
				DestroyFilterData();
				if(cv.Columns.Count > 0) AssignColumns(cv, true);
				RefreshVisibleColumnsList();
				AssignCVData(cv);
				SyncFormatRulesSummary();
			}
			finally {
				EndDataUpdateCore(true);
				EndSynchronization();
			}
		}
		void AssignCVData(ColumnView cv) {
			SortInfo.Assign(cv.SortInfo, Columns);
			SortInfo.GroupCount = cv.SortInfo.groupCount;
			UpdateSortGroupIndexes();
			SynchronizeSortingAndGrouping();
			this.findFilterText = cv.FindFilterText;
			ActiveFilter.Assign(cv.ActiveFilter, Columns);
			this.activeFilterEnabled = cv.ActiveFilterEnabled;
			ApplyColumnsFilterEx();
		}
		public override void SynchronizeVisual(BaseView viewSource) {
			if(viewSource == null) return;
			BeginSynchronization();
			BeginUpdate();
			try {
				base.SynchronizeVisual(viewSource);
				ColumnView cv = viewSource as ColumnView;
				if(cv == null) return;
				if(cv.Columns.Count > 0) AssignColumns(cv, true);
				SyncCVProperties(cv);
				RefreshVisibleColumnsList();
			}
			finally {
				EndUpdate();
				EndSynchronization();
			}
		}
		void SyncCVProperties(ColumnView sourceView) {
			this.FormatRules.Assign(sourceView.FormatRules);
			this.FormatConditions.Assign(sourceView.FormatConditions);
			this.activeFilterEnabled = sourceView.ActiveFilterEnabled;
			this.viewCaptionHeight = sourceView.ViewCaptionHeight;
			this.images = sourceView.Images;
			this.OptionsView.Assign(sourceView.OptionsView);
			this.OptionsBehavior.Assign(sourceView.OptionsBehavior);
			this.OptionsFilter.Assign(sourceView.OptionsFilter);
			this.OptionsFind.Assign(sourceView.OptionsFind);
			this.OptionsSelection.Assign(sourceView.OptionsSelection);
		}
		protected virtual void AssignColumns(ColumnView cv, bool synchronize) {
			if(synchronize) {
				Columns.Synchronize(cv.Columns);
			} else {
				Columns.Assign(cv.Columns);
			}
		}
		protected internal override bool AllowAssignSplitOptions { get { return OptionsView.AllowAssignSplitOptions; } }
		public override void Assign(BaseView v, bool copyEvents) {
			if(v == null) return;
			BeginUpdate();
			try {
				base.Assign(v, copyEvents);
				ColumnView cv = v as ColumnView;
				if(cv != null) {
					AssignColumns(cv, false);
					SyncCVProperties(cv);
					AssignCVData(cv);
					if(copyEvents) {
						Events.AddHandler(columnFilterChanged, cv.Events[columnFilterChanged]);
						Events.AddHandler(customFilterDialog, cv.Events[customFilterDialog]);
						Events.AddHandler(customFilterDisplayText, cv.Events[customFilterDisplayText]);
						Events.AddHandler(filterEditorCreated, cv.Events[filterEditorCreated]);
						Events.AddHandler(unboundExpressionEditorCreated, cv.Events[unboundExpressionEditorCreated]);
						Events.AddHandler(showFilterPopupListBox, cv.Events[showFilterPopupListBox]);
						Events.AddHandler(showFilterPopupCheckedListBox, cv.Events[showFilterPopupCheckedListBox]);
						Events.AddHandler(showFilterPopupDate, cv.Events[showFilterPopupDate]);
						Events.AddHandler(customDrawFilterPanel, cv.Events[customDrawFilterPanel]);
						Events.AddHandler(selectionChanged, cv.Events[selectionChanged]);
						Events.AddHandler(customColumnSort, cv.Events[customColumnSort]);
						Events.AddHandler(customColumnDisplayText, cv.Events[customColumnDisplayText]);
						Events.AddHandler(customUnboundColumnData, cv.Events[customUnboundColumnData]);
						Events.AddHandler(initNewRow, cv.Events[initNewRow]);
						Events.AddHandler(rowLoaded, cv.Events[rowLoaded]);
						Events.AddHandler(asyncCompleted, cv.Events[asyncCompleted]);
						Events.AddHandler(focusedRowLoaded, cv.Events[focusedRowLoaded]);
						Events.AddHandler(rowUpdated, cv.Events[rowUpdated]);
						Events.AddHandler(rowDeleting, cv.Events[rowDeleting]);
						Events.AddHandler(rowDeleted, cv.Events[rowDeleted]);
						Events.AddHandler(customRowFilter, cv.Events[customRowFilter]);
						Events.AddHandler(substituteFilter, cv.Events[substituteFilter]);
						Events.AddHandler(substituteSortInfo, cv.Events[substituteSortInfo]);
						Events.AddHandler(validateRow, cv.Events[validateRow]);
						Events.AddHandler(beforeLeaveRow, cv.Events[beforeLeaveRow]);
						Events.AddHandler(cellValueChanged, cv.Events[cellValueChanged]);
						Events.AddHandler(cellValueChanging, cv.Events[cellValueChanging]);
						Events.AddHandler(columnChanged, cv.Events[columnChanged]);
						Events.AddHandler(columnUnboundExpressionChanged, cv.Events[columnUnboundExpressionChanged]);
						Events.AddHandler(columnPositionChanged, cv.Events[columnPositionChanged]);
						Events.AddHandler(customDrawEmptyForeground, cv.Events[customDrawEmptyForeground]);
						Events.AddHandler(dataManagerReset, cv.Events[dataManagerReset]);
						Events.AddHandler(endGrouping, cv.Events[endGrouping]);
						Events.AddHandler(endSorting, cv.Events[endSorting]);
						Events.AddHandler(focusedColumnChanged, cv.Events[focusedColumnChanged]);
						Events.AddHandler(focusedRowChanged, cv.Events[focusedRowChanged]);
						Events.AddHandler(focusedRowObjectChanged, cv.Events[focusedRowObjectChanged]);
						Events.AddHandler(hiddenEditor, cv.Events[hiddenEditor]);
						Events.AddHandler(invalidRowException, cv.Events[invalidRowException]);
						Events.AddHandler(rowCellDefaultAlignment, cv.Events[rowCellDefaultAlignment]);
						Events.AddHandler(showingEditor, cv.Events[showingEditor]);
						Events.AddHandler(shownEditor, cv.Events[shownEditor]);
						Events.AddHandler(startGrouping, cv.Events[startGrouping]);
						Events.AddHandler(startSorting, cv.Events[startSorting]);
					}
				}
			}
			finally {
				EndUpdate();
			}
		}
		public virtual void ClearSorting() {
			SortInfo.ClearSorting();
		}
		public override void BeginDataUpdate() {
			base.BeginDataUpdate();
			SortInfo.BeginUpdate();
			if(DataController.LockUpdate == 0 && CanSynchronized) {
				UpdateLastRowsInfo(true, true);
				SetCursor(Cursors.WaitCursor);
			}
			BeginUpdate();
			DataController.BeginSortUpdate();
		}
		protected internal override void EndDataUpdateCore(bool sortOnly) {
			try {
				base.EndDataUpdateCore(sortOnly);
				if(ViewDisposing) return;
				SortInfo.EndUpdate();
				if(DataController.LockUpdate == 1) {
					SynchronizeSortingAndGrouping(true);
				}
				if(ViewDisposing) return;
				BeginSynchronization();
				try {
					if(sortOnly)
						DataController.EndSortUpdate();
					else
						DataController.EndUpdate();
				}
				finally {
					EndSynchronization();
				}
			}
			finally {
				EndUpdate();
				if(!ViewDisposing && DataController.LockUpdate == 0) {
					ResetDefaultCursor();
					if(CanSynchronized) OnViewPropertiesChanged(SynchronizationMode.Data);
					UpdateLastRowsInfo(true, true);
				}
			}
		}
		public void BeginSort() { BeginDataUpdate(); }
		public void EndSort() { EndDataUpdate(); }
		internal object GetRowByDataSourceIndex(int dataSourceIndex) { return DataController.GetRowByListSourceIndex(dataSourceIndex); }
		public int GetRowHandle(int dataSourceIndex) {
			return DataController.GetControllerRow(dataSourceIndex);
		}
		public int GetDataSourceRowIndex(int rowHandle) {
			return DataController.GetListSourceRowIndex(rowHandle);
		}
		public int ViewRowHandleToDataSourceIndex(int rowHandle) {
			return GetDataSourceRowIndex(rowHandle);
		}
		public virtual bool IsDataRow(int rowHandle) {
			if(!IsValidRowHandle(rowHandle)) return false;
			if(rowHandle == GridControl.NewItemRowHandle) return false;
			return true;
		}
		public virtual bool IsValidRowHandle(int rowHandle) {
			return DataController.IsValidControllerRowHandle(rowHandle);
		}
		public override object GetRow(int rowHandle) {
			return DataController.GetRow(rowHandle);
		}
		public virtual DataRow GetDataRow(int rowHandle) {
			object val = GetRow(rowHandle);
			if(val is DataRowView) {
				return (val as DataRowView).Row;
			}
			return null;
		}
		protected internal object GetRowKey(int rowHandle) {
			if(rowHandle == GridControl.AutoFilterRowHandle) return null;
			return DataController.GetRowKey(rowHandle);
		}
		protected internal virtual int ScrollPageSize { get { return 1; } }
		protected internal virtual void DoNavigatorAction(NavigatorButtonType type) {
			switch(type) {
				case NavigatorButtonType.First : MoveFirst(); break;
				case NavigatorButtonType.Last : MoveLastVisible(); break;
				case NavigatorButtonType.Next : DoMoveFocusedRow(1, new KeyEventArgs(Keys.Down)); break;
				case NavigatorButtonType.Prev : DoMoveFocusedRow(-1, new KeyEventArgs(Keys.Up)); break;
				case NavigatorButtonType.NextPage : MoveNextPage(); break;
				case NavigatorButtonType.PrevPage: MovePrevPage(); break;
				case NavigatorButtonType.Edit : NavigatorShowEdit(); break;
				case NavigatorButtonType.CancelEdit: NavigatorCancelEdit(); break;
				case NavigatorButtonType.EndEdit: NavigatorEndEdit(); break;
				case NavigatorButtonType.Remove: DeleteSelectedRows(); break;
				case NavigatorButtonType.Append: 
					try {
						CloseEditor();
						if(!UpdateCurrentRowCore()) return;
						AddNewRow(); 
					}
					catch {
					}
					return;
			}
		}
		protected virtual void NavigatorShowEdit() { ShowEditor(); }
		protected virtual void NavigatorCancelEdit() { HideEditor(); CancelUpdateCurrentRow(); }
		protected virtual void NavigatorEndEdit() {
			if(!PostEditor()) return;
			HideEditor();
			UpdateCurrentRow();
		}
		protected internal virtual int NavigatorRowCount { 
			get { 
				int res = DataRowCount;
				if(IsNewItemRow(FocusedRowHandle)) res++;
				return res; 
			} 
		}
		protected internal virtual int NavigatorPosition { 
			get { 
				if(FocusedRowHandle == GridControl.InvalidRowHandle) return 0;
				if(IsNewItemRow(FocusedRowHandle)) return Math.Max(NavigatorRowCount - 1, 0);
				if(FocusedRowHandle < 0) return DataController.GetControllerRowByGroupRow(FocusedRowHandle);
				return FocusedRowHandle;
			}
		}
		protected internal virtual bool IsNavigatorActionEnabled(NavigatorButtonType type) {
			if(!DataController.IsReady) return false;
			switch(type) {
				case NavigatorButtonType.Append:
					return GetAllowAddRows();
				case NavigatorButtonType.First : return !IsFirstRow;
				case NavigatorButtonType.Last : return !IsLastVisibleRow;
				case NavigatorButtonType.Next : return !IsLastVisibleRow || GridControl.DefaultView != this;
				case NavigatorButtonType.Prev : return !IsFirstRow || GridControl.DefaultView != this;
				case NavigatorButtonType.NextPage : return !IsLastVisibleRow;
				case NavigatorButtonType.PrevPage: return !IsFirstRow;
				case NavigatorButtonType.Edit : return !IsEditing && GetCanShowEditor(FocusedColumn);
				case NavigatorButtonType.CancelEdit: 
				case NavigatorButtonType.EndEdit: return IsEditing || FocusedRowModified || IsNewItemRow(FocusedRowHandle) || DataController.IsCurrentRowModified;
				case NavigatorButtonType.Remove:
					if(!DataController.AllowRemove || RowCount == 0) return false;
					if(IsEditing || DataController.IsNewItemRowEditing) return false;
					if(!GetAllowDeleteRows()) return false;
					if(IsMultiSelect) return SelectedRowsCount > 0;
					return true;
			}
			return false;
		}
		public virtual int GetNextVisibleRow(int rowVisibleIndex) {
			return RowCount > rowVisibleIndex + 1 ? rowVisibleIndex + 1 : DevExpress.Data.DataController.InvalidRow;
		}
		public int GetPrevVisibleRow(int rowVisibleIndex) {
			return rowVisibleIndex > 0 ? rowVisibleIndex - 1 : InvalidRowHandle;
		}
		public virtual int GetVisibleIndex(int rowHandle) {
			return DataController.GetVisibleIndexChecked(rowHandle);
		}
		public virtual int GetVisibleRowHandle(int rowVisibleIndex) {
			if(rowVisibleIndex < 0) return DevExpress.Data.DataController.InvalidRow;
			if(DesignMode) {
				if(rowVisibleIndex < 2) {
					if(SortInfo.GroupCount > 0)
						return 0 - (rowVisibleIndex + 1);
					return rowVisibleIndex;
				}
			}
			return DataController.GetControllerRowHandle(rowVisibleIndex);
		}
		public virtual void MoveFirst() {
			MoveTo(GetVisibleRowHandle(0), Keys.Home);
		}
		public virtual void MoveBy(int delta) {
			DoMoveFocusedRow(delta, new KeyEventArgs(Keys.None));
		}
		public virtual void MoveNext() {
			MoveBy(1);
		}
		public virtual void MoveNextPage() {
			MoveBy(ScrollPageSize);
		}
		public virtual void MovePrevPage() {
			MoveBy(-ScrollPageSize);
		}
		public virtual void MovePrev() {
			MoveBy(-1);
		}
		protected internal void MoveTo(int rowHandle) { MoveTo(rowHandle, Keys.None); }
		protected internal virtual void MoveTo(int rowHandle, Keys byKeyData) {
			int prevFocusedHandle = FocusedRowHandle;
			if(!DoBeforeMoveFocusedRow(0, byKeyData)) return;
			try {
				FocusedRowHandle = rowHandle;
			}
			finally {
				DoAfterMoveFocusedRow(new KeyEventArgs(byKeyData), prevFocusedHandle, null, null);
			}
		}
		protected virtual bool DoBeforeMoveFocusedRow(int delta, Keys byKeyData) {
			return DoBeforeMoveFocusedRow(delta, new KeyEventArgs(byKeyData));
		}
		protected virtual bool DoBeforeMoveFocusedRow(int delta, KeyEventArgs e) {
			if(OptionsBehavior.FocusLeaveOnTab) {
				if((e.KeyData & Keys.Tab) == Keys.Tab) {
					bool moveForward = (e.KeyData & Keys.Shift) != Keys.Shift;
					if(CanLeaveFocusOnTab(moveForward)) {
						LeaveFocusOnTab(moveForward);
						return false;
					}
				}
			}
			return true;
		}
		public virtual void MoveLast() {
			MoveTo(DataRowCount - 1);
		}
		public virtual void MoveLastVisible() {
			MoveTo(GetVisibleRowHandle(RowCount - 1), Keys.End);
		}
		public string GetRowCellDisplayText(int rowHandle, string fieldName) { return GetRowCellDisplayText(rowHandle, Columns[fieldName]); }
		public virtual string GetRowCellDisplayText(int rowHandle, GridColumn column) {
			if(column == null) throw new ArgumentNullException("column");
			return GetRowCellDisplayText(rowHandle, column, GetRowCellValue(rowHandle, column));
		}
		protected internal bool UpdateCurrentRowCore() {
			return UpdateCurrentRowCore(false);
		}
		protected virtual bool UpdateCurrentRowCore(bool force) {
			bool res = DataController.EndCurrentRowEdit(force);
			RefreshRow(FocusedRowHandle);
			if(res) this.SetFocusedRowModifiedCore(false);
			return res;
		}
		public override bool UpdateCurrentRow() {
			return UpdateCurrentRowCore();
		}
		protected internal override bool EndEditOnLeave() {
			return UpdateCurrentRowCore(true);
		}
		public bool IsNewItemRow(int rowHandle) {
			return rowHandle == DevExpress.Data.CurrencyDataController.NewItemRow;
		}
		public virtual void AddNewRow() {
			HideEditor();
			if(IsDefaultState) {
				if(!CheckCanLeaveCurrentRow(true)) return;
				ClearSelectionCore();
				DataController.AddNewRow();
				SetFocusedRowModified();
			}
		}
		public virtual void CancelUpdateCurrentRow() {
			DataController.CancelCurrentRowEdit();
			SetFocusedRowModifiedCore(false);
			ClearColumnErrors();
			RefreshRow(FocusedRowHandle, false);
		}
		public int GetFocusedDataSourceRowIndex() { return GetDataSourceRowIndex(FocusedRowHandle); }
		public object GetFocusedRow() { return GetRow(FocusedRowHandle); }
		public DataRow GetFocusedDataRow() { return GetDataRow(FocusedRowHandle); }
		public int FindRow(object row) { return DataController.FindRowByRowValue(row); }
		public virtual object GetFocusedValue() {
			return GetRowCellValue(FocusedRowHandle, FocusedColumn);
		}
		public virtual string GetFocusedDisplayText() {
			return GetRowCellDisplayText(FocusedRowHandle, FocusedColumn);
		}
		public virtual void SetFocusedValue(object value) {
			SetRowCellValue(FocusedRowHandle, FocusedColumn, value);
		}
		public void SetFocusedRowCellValue(string fieldName, object value) {
			SetRowCellValue(FocusedRowHandle, fieldName, value);
		}
		public void SetFocusedRowCellValue(GridColumn column, object value) {
			SetRowCellValue(FocusedRowHandle, column, value);
		}
		public object GetFocusedRowCellValue(GridColumn column) {
			return GetRowCellValue(FocusedRowHandle, column);
		}
		public object GetFocusedRowCellValue(string fieldName) {
			return GetRowCellValue(FocusedRowHandle, fieldName);
		}
		public string GetFocusedRowCellDisplayText(string fieldName) {
			return GetFocusedRowCellDisplayText(Columns[fieldName]);
		}
		public string GetFocusedRowCellDisplayText(GridColumn column) {
			return GetRowCellDisplayText(FocusedRowHandle, column);
		}
		protected GridColumn CheckColumn(GridColumn column) {
			if(column != null && column.View != this) column = Columns[column.FieldName];
			return column;
		}
		public virtual object GetRowCellValue(int rowHandle, string fieldName) {
			return DataController.GetRowValue(rowHandle, fieldName, null);
		}
		public virtual object GetRowCellValue(int rowHandle, GridColumn column) {
			column = CheckColumn(column);
			if(column == null) return null;
			if(DesignMode) return GetDesignTimeRowCellValue(rowHandle, column);
			return DataController.GetRowValue(rowHandle, column.ColumnHandle, null);
		}
		public object GetListSourceRowCellValue(int listSourceRowIndex, GridColumn column) {
			column = CheckColumn(column);
			if(column == null) return null;
			return DataController.GetListSourceRowValue(listSourceRowIndex, column.ColumnHandle);
		}
		public object GetListSourceRowCellValue(int listSourceRowIndex, string fieldName) {
			return DataController.GetListSourceRowValue(listSourceRowIndex, fieldName);
		}
		protected virtual object GetDesignTimeRowCellValue(int rowHandle, GridColumn column) {
			DataColumnInfo col = DataController.Columns[column.ColumnHandle];
			if(col == null) return "string";
			Type type = Nullable.GetUnderlyingType(col.Type);
			if(type == null)
				type = col.Type;
			int index = Array.IndexOf(types, type);
			if(index == -1) return null;
			return values[index];
		}
		static object[] types = new object[] { typeof(string), typeof(DateTime), typeof(int), typeof(Decimal), typeof(byte),
												 typeof(Int64)};
		static object[] values = new object[] { "string", DateTime.Now.Date, 123, 123, 123, 123};
		protected virtual void SetRowCellValueCore(int rowHandle, GridColumn column, object _value, bool fromEditor) {
			column = CheckColumn(column);
			if(column == null) return;
			try {
				DataController.SetRowValue(rowHandle, column.ColumnHandle, _value);
				UpdateRowAutoHeight(rowHandle);
				RaiseCellValueChanged(new CellValueChangedEventArgs(rowHandle, column, GetRowCellValue(rowHandle, column)));
				if(rowHandle == FocusedRowHandle && column == FocusedColumn) {
					RefreshEditor(!fromEditor);
					SetFocusedRowModified();
					if (fromEditor && (ActiveEditor != null))
						ActiveEditor.IsModified = false;
				}
			}
			catch(Exception e) {
				if(fromEditor) e = new EditorValueException(e, e.Message);
				GridControl.EditorHelper.OnInvalidValueException(GridControl, e, _value);
			}
		}
		public void SetRowCellValue(int rowHandle, string fieldName, object _value) {
			SetRowCellValueCore(rowHandle, Columns[fieldName], _value, false);
		}
		public void SetRowCellValue(int rowHandle, GridColumn column, object _value) {
			SetRowCellValueCore(rowHandle, column, _value, false);
		}
		protected internal override BaseContainerValidateEditorEventArgs CreateValidateEventArgs(object value) {
			return new ColumnViewValidateEditorEventArgs(this, value);
		}
		public override bool ValidateEditor() {
			if(GridControl == null) return false;
			return GridControl.EditorHelper.ValidateEditor(GridControl);
		}
		public virtual void RefreshEditor(bool updateEditorValue) {
			if(ActiveEditor == null || !IsEditing) return;
			BaseEdit be = ActiveEditor;
			DevExpress.XtraEditors.DXErrorProvider.ErrorType errorType;
			string error;
			GetColumnError(FocusedRowHandle, FocusedColumn, out error, out errorType);
			if(!updateEditorValue && string.IsNullOrEmpty(error)) {
				if(string.IsNullOrEmpty(be.ErrorText)) return;
			}
			be.Properties.BeginUpdate();
			try {
				be.Properties.LockEvents();
				be.ErrorText = error;
				if(be.ErrorText != null && be.ErrorText.Length > 0) {
					be.ErrorIcon = DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider.GetErrorIconInternal(errorType);
				}
				if(updateEditorValue) {
					be.EditValue = FocusedValue;
					be.IsModified = false;
				}
			}
			finally {
				be.Properties.UnLockEvents();
				be.Properties.EndUpdate();
			}
		}
		public override void HideEditor() {
			if(!IsEditing || !fAllowCloseEditor) return;
			if(ActiveEditor != null && GridControl != null) {
				ActiveEditor.Modified -= new EventHandler(OnActiveEditor_ValueModified);
				ActiveEditor.LostFocus -= new EventHandler(OnActiveEditor_LostFocus);
				ActiveEditor.GotFocus -= new EventHandler(OnActiveEditor_GotFocus);
				ActiveEditor.EditValueChanging -= new DevExpress.XtraEditors.Controls.ChangingEventHandler(OnActiveEditor_ValueChanging);
				ActiveEditor.EditValueChanged -= new EventHandler(OnActiveEditor_ValueChanged);
				ActiveEditor.MouseDown -= new MouseEventHandler(OnActiveEditor_MouseDown);
				GridControl.EditorHelper.HideEditorCore(GridControl, GridControl.ContainsFocus);
				RaiseHiddenEditor();
				UpdateNavigator();
			}
		}
		protected virtual ToolTipControlInfo GetCellEditToolTipInfo(DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo vi, Point p, int rowHandle, GridColumn column) {
			if(vi == null || column == null) return null;
			ToolTipControlInfo info = vi.GetToolTipInfo(p);
			if(info != null) {
				info.Object = new CellToolTipInfo(rowHandle, column, info.Object);
				return info;
			}
			return null;
		}
		protected internal Point GetCellPoint(object cellInfo, Point point) { return GetCellPoint(cellInfo, point, false); }
		protected internal virtual Point GetCellPoint(object cellInfo, Point point, bool toolTip) { return point; }
		protected virtual object GetCellInfo(BaseHitInfo hitInfo) { return null; }
		protected virtual BaseEditViewInfo GetCellEditInfo(object cellInfo) { return null; }
		protected internal virtual void OnColumnOptionsChanged(GridColumn column, BaseOptionChangedEventArgs e) {
			if(e.Name == "FilterPopupMode") {
				ResetDateFilterCache();
			}
		}
		protected virtual void OnCellMouseEnter(BaseHitInfo hitInfo) {
			if(UpdateCellHotInfo(hitInfo, hitInfo.HitPoint)) InvalidateHitObject(hitInfo);
			UpdateCellMouse(hitInfo);
		}
		protected virtual bool OnCellMouseMove(BaseHitInfo hitInfo) {
			if(UpdateCellHotInfo(hitInfo, hitInfo.HitPoint)) InvalidateHitObject(hitInfo);
			return UpdateCellMouse(hitInfo);
		}
		bool UpdateCellMouse(BaseHitInfo hitInfo) {
			object cellInfo = GetCellInfo(hitInfo);
			BaseEditViewInfo editInfo = GetCellEditInfo(cellInfo);
			if(editInfo != null) {
				Cursor cursor = editInfo.GetMouseCursor(GetCellPoint(cellInfo, hitInfo.HitPoint));
				if(cursor == Cursors.Default) cursor = GridControl.GetDefaultCursor();
				SetCursor(cursor);
				return cursor == GridControl.GetDefaultCursor();
			}
			return true;
		}
		protected virtual void OnCellMouseLeave(BaseHitInfo hitInfo) {
			if(UpdateCellHotInfo(hitInfo, BaseViewInfo.EmptyPoint)) InvalidateHitObject(hitInfo);
			ResetCursor();
		}
		protected virtual void OnRowMouseEnter(int rowHandle) {
		}
		protected virtual void OnRowMouseLeave(int rowHandle) {
		}
		protected virtual void OnHotTrackEnter(BaseHitInfo hitInfo) {
		}
		protected virtual void OnHotTrackLeave(BaseHitInfo hitInfo) {
		}
		protected virtual bool UpdateCellHotInfo(BaseHitInfo hitInfo, Point hitPoint) {
			return false;
		}
		protected virtual internal void DoMouseSortColumn(GridColumn column, Keys key) {
			if(!CanSortColumn(column)) return;
			if(!UpdateCurrentRow()) return;
			ColumnSortOrder newSort = ColumnSortOrder.None;
			BeginSynchronization();
			try {
				BeginDataUpdate();
				try {
					if(key != Keys.Control) {
						switch(column.SortOrder) {
							case ColumnSortOrder.None : newSort = ColumnSortOrder.Ascending; break;
							case ColumnSortOrder.Descending : newSort = ColumnSortOrder.Ascending;break;
							case ColumnSortOrder.Ascending : newSort = ColumnSortOrder.Descending;break;
						}
					}
					if(key != Keys.Shift && key != Keys.Control)  {
						if(column.GroupIndex == -1)
							ClearSorting();
					}
					OnBeforeMouseSortColumn();
					column.SortOrder = newSort;
				}
				finally {
					EndDataUpdateCore(true);
				}
			} finally {
				EndSynchronization();
			}
			if(CanSynchronized) OnViewPropertiesChanged(SynchronizationMode.Data);
		}
		protected virtual void OnBeforeMouseSortColumn() { }
		protected internal override bool CheckCanLeaveCurrentRow(bool raiseUpdateCurrentRow) { 
			return CheckCanLeaveRow(FocusedRowHandle, raiseUpdateCurrentRow);
		}
		protected internal override bool CheckCanLeaveRow(int currentRowHandle, bool raiseUpdateCurrentRow) {
			if(IsInListChangedEvent)
				HideEditor(); 
			else
				CloseEditor();
			if(currentRowHandle != GridControl.InvalidRowHandle) {
				if(raiseUpdateCurrentRow && !UpdateCurrentRow()) {
					if(GridControl.EditorHelper.AllowHideException) throw new HideException();
					return false;
				}
				if(!IsNewItemRow(currentRowHandle) && !RaiseBeforeLeaveRow(new RowAllowEventArgs(currentRowHandle, true))) {
					if(GridControl.EditorHelper.AllowHideException) throw new HideException();
					return false;
				}
			}
			return true;
		}
		protected internal override void OnHotTrackChanged(BaseHitInfo oldInfo, BaseHitInfo newInfo) {
			if(oldInfo != null) OnHotTrackLeave(oldInfo);
			if(newInfo != null) OnHotTrackEnter(newInfo);
			base.OnHotTrackChanged(oldInfo, newInfo);
		}
		protected internal override void OnRowHotTrackChanged(int oldRowHandle, int newRowHandle) {
			if(oldRowHandle != GridControl.InvalidRowHandle) OnRowMouseLeave(oldRowHandle);
			if(newRowHandle != GridControl.InvalidRowHandle) OnRowMouseEnter(newRowHandle);
		}
		protected virtual void OnColumnPopulate(GridColumn column, int visibleIndex) {
			column.SetVisibleCore(visibleIndex > -1, visibleIndex);
		}
		protected virtual void PopulateColumnsCore(DataColumnInfo[] columns) {
			if(columns == null) return;
			BeginUpdate();
			try {
				Columns.DestroyColumns();
				if(columns != null) {
					int vi = 0;
					foreach(DataColumnInfo dcInfo in columns) {
						int columnIndex = vi;
						GridColumn column = PopulateColumn(dcInfo, dcInfo.Index, ref columnIndex);
						if(column != null)
							OnColumnPopulate(column, columnIndex);
					}
					RefreshVisibleColumnsList();
					if(FocusedColumn == null && VisibleColumns.Count > 0) 
						this.FocusedColumn = GetNearestCanFocusedColumn(GetVisibleColumn(0), 0);
				}
				OnPopulateColumns();
			}
			finally { EndUpdate(); }
		}
		public void PopulateColumns(DataColumnInfo[] columns) { 
			PopulateColumnsCore(columns); 
		}
		public virtual void PopulateColumns(object list) {
			PopulateColumnsCore(new MasterDetailHelper().GetDataColumnInfo(list));
		}
		public virtual void PopulateColumns(Type elementType, DevExpress.XtraGrid.Extensions.PopulateColumnsParameters populateColumnsParameters = null) {
			if(elementType == null) return;
			try {
				Type listType = typeof(List<>).MakeGenericType(elementType);
				PopulateColumns(Activator.CreateInstance(listType));
				foreach(GridColumn column in Columns) {
					DevExpress.XtraGrid.Extensions.PopulateColumnParameters parameter = populateColumnsParameters.CustomColumnParameters.FirstOrDefault(e => e.FieldName == column.FieldName);
					if(parameter != null) {
						if(parameter.ColumnVisible.HasValue) column.Visible = parameter.ColumnVisible.Value;
						if(!string.IsNullOrEmpty(parameter.Path)) column.FieldName = parameter.Path;
					}
				}
			}
			catch { }
		}
		public void PopulateColumns(DataTable dt) { 
			if(dt != null) {
				PopulateColumns((object)dt.DefaultView); 
			}
		}
		protected virtual void OnPostRowException(ControllerRowExceptionEventArgs e) {
			try {
				InvalidRowExceptionEventArgs ex = new InvalidRowExceptionEventArgs(e.Exception, e.Exception.Message, FocusedRowHandle, e.Row);
				RaiseInvalidRowException(ex);
				if(ex.ExceptionMode == ExceptionMode.Ignore) {
					RefreshRow(FocusedRowHandle);
					e.Action = ExceptionAction.CancelAction;
					return;
				}
				e.Action = ExceptionAction.RetryAction;
			}
			catch(HideException) {
				e.Action = ExceptionAction.CancelAction;
			}
		}
		protected virtual void ProcessRowActions(MethodIntInvoker method, int parameter) {
			int index = GetVisibleIndex(FocusedRowHandle);
			BeginLockFocusedRowChange();
			try {
				if(method != null) method(parameter);
			}
			finally {
				EndLockFocusedRowChange();
			}
			int row = GetVisibleRowHandle(index);
			if(row == InvalidRowHandle) row = GetVisibleRowHandle(RowCount - 1);
			FocusedRowHandle = row;
			DataController.SyncCurrentRow();
		}
		public virtual void DeleteSelectedRows() {
			if(IsMultiSelect)
				ProcessRowActions(new MethodIntInvoker(DeleteSelectedRowsCore), -1);
			else
				DeleteRow(FocusedRowHandle);
		}
		protected void DeleteSelectedRowsCore(int row) {
			DataController.DeleteSelectedRows();
		}
		public virtual void DeleteRow(int rowHandle) {
			ProcessRowActions(new MethodIntInvoker(DeleteRowCore), rowHandle);
		}
		protected virtual void DeleteRowCore(int rowHandle) {
			HideEditor();
			DataController.DeleteRow(rowHandle);
			if(!IsValidRowHandle(0)) DoChangeFocusedRowInternal(GridControl.InvalidRowHandle, false);
		}
		public override void PopulateColumns() {
			if(GridControl != null && !GridControl.IsLoaded && IsDesignMode) return; 
			BeginUpdate();
			try {
				Columns.DestroyColumns();
				if(DataController.IsReady) {
					for(int i = 0; i < DataController.Columns.Count; i++) {
						int columnIndex = i;
						GridColumn column = PopulateColumn(DataController.Columns[i], i, ref columnIndex);
						if(column != null)
							OnColumnPopulate(column, columnIndex);
					}
					RefreshVisibleColumnsList();
					if(FocusedColumn == null && VisibleColumns.Count > 0) this.FocusedColumn = GetNearestCanFocusedColumn(GetVisibleColumn(0), 0);
				}
				OnPopulateColumns();
			}
			finally { EndUpdate(); }
		}
		protected virtual GridColumn PopulateColumn(DataColumnInfo columnInfo, int columnHandle, ref int columnIndex) {
			if(!columnInfo.Visible) 
				return null;
			var options = DevExpress.Data.Utils.AnnotationAttributes.GetColumnOptions(columnInfo.PropertyDescriptor, columnIndex, columnInfo.ReadOnly);
			if(!options.AutoGenerateField)
				return null;
			GridColumn column = Columns.AddField(columnInfo.Name);
			column.SetColumnAnnotationAttributes(options.Attributes);
			columnIndex = options.ColumnIndex;
			if(options.ReadOnly)
				column.OptionsColumn.ReadOnly = true;
			if(!options.AllowEdit)
				column.OptionsColumn.AllowEdit = false;
			if(!options.AllowFilter)
				column.OptionsFilter.AllowFilter = false;
			column.ColumnHandle = columnHandle;
			return column;
		}
		protected override void DesignerMakeColumnsVisible() {
			BeginUpdate();
			try {
				for(int n = 0; n < Columns.Count; n++) {
					Columns[n].VisibleIndex = n;
				}
			}
			finally {
				EndUpdate();
			}
		}
		protected virtual void OnPopulateColumns() { 
			ApplyColumnsFilterCore();
		}
		protected virtual bool CanUseFixedStyle { get { return false; } } 
		protected internal virtual RepositoryItem GetRowCellRepositoryItem(int rowHandle, GridColumn column) {
			if(column == null) return null;
			RepositoryItem ce = column.ColumnEdit;
			if(ce == null) ce = GetColumnDefaultRepositoryItem(column);
			return ce;
		}
		protected virtual bool GetColumnReadOnly(GridColumn column) {
			if(OptionsBehavior.ReadOnly) return true;
			return column == null || column.ReadOnly;
		}
		protected internal virtual RepositoryItem GetColumnDefaultRepositoryItem(GridColumn column) {
			if(GridControl == null) return null;
			var columnType = (column == null) ? typeof(string) : column.ColumnType;
			var columnAnnotationAttributes = (column == null) ? null : column.ColumnAnnotationAttributes;
			return GridControl.EditorHelper.DefaultRepository.GetRepositoryItem(columnType, columnAnnotationAttributes);
		}
		protected internal virtual RepositoryItem GetColumnDefaultRepositoryItemForEditing(GridColumn column, RepositoryItem editor) {
			if(GridControl == null) return editor;
			var columnType = (column == null) ? typeof(string) : column.ColumnType;
			var columnAnnotationAttributes = (column == null) ? null : column.ColumnAnnotationAttributes;
			return GridControl.EditorHelper.DefaultRepository.GetRepositoryItemForEditing(editor, columnType, columnAnnotationAttributes);
		}
		internal DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo CreateColumnEditViewInfo(GridColumn column, int rowHandle) {
			return CreateColumnEditViewInfo(column, rowHandle, false);
		}
		protected internal virtual DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo CreateColumnEditViewInfo(GridColumn column, int rowHandle, bool forFilter) {
			RepositoryItem ritem = null;
			if(forFilter && column.FilterFieldName != column.FieldName) {
				ritem = GridControl.EditorHelper.DefaultRepository.GetRepositoryItem(typeof(string));
			}
			else {
				ritem = (rowHandle == GridControl.InvalidRowHandle ? column.RealColumnEdit : GetRowCellRepositoryItem(rowHandle, column));
			}
			if(ritem != null) {
				DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo bev = ritem.CreateViewInfo();
				ViewInfo.UpdateEditViewInfo(bev, column, rowHandle);
				return bev;
			}
			return null;
		}
		public virtual string GetDisplayTextByColumnValue(GridColumn column, object val) {
			if(column == null) return string.Empty;
			DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo bev = CreateColumnEditViewInfo(column, GridControl.InvalidRowHandle);
			string res;
			if(bev != null) {
				bev.EditValue = val;
				res = bev.DisplayText;
			} else {
				res = (val == null ? "null" : val.ToString());
			}
			return RaiseCustomColumnDisplayText(GridControl.InvalidRowHandle, column, val, res, false);
		}
		protected internal virtual string GetRowCellDisplayText(int rowHandle, GridColumn column, object value) {
			column = CheckColumn(column);
			if(column == null) return string.Empty;
			DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo bev = CreateColumnEditViewInfo(column, rowHandle);
			return GetRowCellDisplayTextCore(rowHandle, column, bev, value, false);
		}
		protected virtual string GetGroupRowDisplayText(int rowHandle, GridColumn column, object value, FormatInfo format) {
			if(column == null) return string.Empty;
			BaseEditViewInfo bev = CreateColumnEditViewInfo(column, rowHandle);
			if(format != null && !format.IsEmpty) bev.Format = format;
			string res = GetRowCellDisplayTextCore(rowHandle, column, bev, value, true);
			return bev.UpdateGroupValueDisplayText(res, value);
		}
		protected internal virtual string GetRowCellDisplayTextCore(int rowHandle, GridColumn column, DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo bev, object value, bool forGroupRow) {
			string res = "";
			if(bev != null) {
				bev.EditValue = value;
				res = bev.DisplayText;
			}  else
				res = DataController.GetRowDisplayText(rowHandle, column.ColumnHandle);
			if(res == null) res = string.Empty;
			return RaiseCustomColumnDisplayText(rowHandle, column, value, res, forGroupRow);
		}
		protected internal virtual bool CanIncrementalSearch(GridColumn column) {
			DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo bev = CreateColumnEditViewInfo(column, GridControl.InvalidRowHandle);
			if(!bev.IsSupportIncrementalSearch) return false;
			return column.OptionsColumn.AllowIncrementalSearch;
		}
		protected internal virtual int FindRow(FindRowArgs e, OperationCompleted rowReceived) {
			string text = e.Text;
			int startRowHandle = e.StartRowHandle;
			if(text == "" || e.Column == null) return GridControl.InvalidRowHandle;
			text = text.ToLower();
			DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo bev = CreateColumnEditViewInfo(e.Column, GridControl.InvalidRowHandle);
			if(!bev.IsSupportIncrementalSearch || DataController.VisibleListSourceRowCount == 0) return GridControl.InvalidRowHandle;
			if(!DataController.IsValidControllerRowHandle(startRowHandle) || startRowHandle < 0) {
				startRowHandle = e.Down ? 0 : DataController.VisibleListSourceRowCount - 1;
			}
			DataController.CancelWeakFindIncremental();
			return DataController.FindIncremental(text, e.Column.ColumnHandle, startRowHandle, e.Down, e.IgnoreStartRowHandle, e.AllowLoop, 
				delegate(int controllerRow, object value, string textSearch) {
					bev.EditValue = value;
					string display = RaiseCustomColumnDisplayText(controllerRow, e.Column, value, bev.DisplayText, false);
					if(display == null) display = string.Empty;
					return DevExpress.Data.DataController.StringStartsWith(display.ToLower(), textSearch);
				}, rowReceived);
		}
		protected virtual void UpdateRowAutoHeight(int rowHandle) {
			if(!IsAutoHeight) return;
			LayoutChanged();
		}
		protected internal new ColumnViewInfo ViewInfo { get { return base.ViewInfo as ColumnViewInfo; } }
		protected override void Dispose(bool disposing) {
			if(disposing) {
				searchInfo = null;
				dateFilterCache.Clear();
				FilterPopup = null;
				MRUFilterPopup = null;
				fViewDisposing = true;
				HideEditor();
				DestroyColumns();
				DestroyFindPanel();
				if(loadingPanel != null) {
					loadingPanel.Dispose();
					this.loadingPanel = null;
				}
				if(loadingAnimator != null) {
					loadingAnimator.Dispose();
					this.loadingAnimator = null;
				}
			}
			base.Dispose(disposing);
		}
		protected virtual void DestroyColumns() {
			Columns.DestroyColumns();
		}
		protected internal MRUFilterPopup MRUFilterPopup {
			get { return mruFilterPopup; }
			set {
				if(MRUFilterPopup == value) return;
				if(MRUFilterPopup != null) MRUFilterPopup.Dispose();
				mruFilterPopup = value;
			}
		}
		protected internal void DestroyMRUFilterPopup() {
			MRUFilterPopup = null;
		}
		protected internal void ShowMRUFilterPopup() {
			if(!CanShowPopupObjects) return;
			MRUFilterPopup = null;
			if(!IsAllowMRUFilterList) return;
			if(!ViewInfo.IsReady || ViewInfo.FilterPanel.Bounds.IsEmpty) return;
			Rectangle tb = ViewInfo.FilterPanel.TextBounds, fb = ViewInfo.FilterPanel.MRUButtonInfo.Bounds;
			if(tb.IsEmpty) return;
			if(!fb.IsEmpty) {
				tb.Width = fb.Right - tb.X;
				tb.Y = Math.Min(tb.Y, fb.Y);
				tb.Height = Math.Max(tb.Bottom, fb.Bottom) - tb.Y;
			}
			MRUFilterPopup = new MRUFilterPopup(this);
			MRUFilterPopup.Init();
			if(MRUFilterPopup.CanShow) 
				MRUFilterPopup.Show(tb);
			else 
				MRUFilterPopup = null;
		}
		protected virtual void UpdateSortGroupIndexes() {
			foreach(GridColumn column in Columns) {
				column.sortIndex = -1;
				column.SetGroupIndexCore(-1);
				column.sortOrder = ColumnSortOrder.None;
			}
			for(int n = 0; n < SortInfo.Count; n++) {
				SortInfo[n].UpdateColumn();
			}
		}
		protected class ColumnGroupSortIndexComparer : IComparer {
			int IComparer.Compare(object a, object b) {
				GridColumn c1 = (GridColumn)a, c2 = (GridColumn)b;
				int v1 = c1.GroupIndex, v2 = c2.GroupIndex;
				if(c1.GroupIndex == c2.GroupIndex) {
					v1 = c1.SortIndex;
					v2 = c2.SortIndex;
				}
				if(v1 == v2) {
					if(c1.SortOrder == c2.SortOrder) return 0;
					if(c1.SortOrder == ColumnSortOrder.None) return 1;
					return -1;
				}
				if(v1 == -1) return 1;
				if(v2 == -1) return -1;
				return Comparer.Default.Compare(v1, v2);
			}
		}
		protected void UpdateSortGroupCollectionsCore() {
			if(SortInfo.Count > 0) {
				UpdateSortGroupIndexes();
				return;
			}
			SortInfo.BeginUpdate();
			try {
				ArrayList cols = new ArrayList();
				cols.AddRange(Columns);
				cols.Sort(new ColumnGroupSortIndexComparer());
				SortInfo.Clear();
				foreach(GridColumn column in cols) {
					bool add = false;
					if(column.GroupIndex > -1) {
						add = true;
						SortInfo.GroupCount = SortInfo.Count + 1;
					} else {
						if(column.SortIndex > -1 || column.SortOrder != ColumnSortOrder.None) add = true;
					}
					if(add) SortInfo.Add(column, column.SortOrder == ColumnSortOrder.None ? ColumnSortOrder.Ascending : column.SortOrder);
				}
				UpdateSortGroupIndexes();
			}
			finally {
				SortInfo.EndUpdate();
			}
		}
		protected internal virtual void DoColumnChangeGroupIndex(GridColumn column, int newIndex) {
			if(newIndex > SortInfo.GroupCount + 1) newIndex = SortInfo.GroupCount + 1;
			bool sorted = column.SortOrder != ColumnSortOrder.None;
			GridColumnSortInfo columnInfo = SortInfo[column];
			SortInfo.BeginUpdate();
			try {
				if(newIndex < 0) {
					if(columnInfo != null && columnInfo.IsGrouped) {
						RemoveColumnGrouping(columnInfo);
					}
				} else {
					if(!CanDataGroupColumn(column)) return;
					if(columnInfo == null) columnInfo = SortInfo.Add(column, ColumnSortOrder.Ascending);
					if(columnInfo == null) return;
					if(column.GroupIndex == -1) 
						column.sortedBeforeGrouping = sorted;
					int curIndex = columnInfo.Index;
					if(curIndex != newIndex) {
						SortInfo.MoveTo(columnInfo, newIndex);
					}
					if(!columnInfo.IsGrouped) SortInfo.GroupCount = columnInfo.Index + 1;
				}
			}
			finally {
				SortInfo.EndUpdate();
			}
			if(this.DataSource == null) this.LayoutChanged(); 
		}
		void RemoveColumnGrouping(GridColumnSortInfo columnInfo) {
			int prevCount = SortInfo.GroupCount;
			if(!columnInfo.Column.sortedBeforeGrouping) {
				columnInfo.Remove();
				SortInfo.GroupCount = prevCount - 1;
			}
			else {
				SortInfo.BeginUpdate();
				try {
					columnInfo.Remove();
					SortInfo.Add(columnInfo.Column, columnInfo.SortOrder);
					if(SortInfo.GroupCount >= prevCount) 
						SortInfo.GroupCount = prevCount - 1; 
				} finally {
					SortInfo.EndUpdate();
				}
			}
		}
		protected internal void DoColumnChangeSortIndex(GridColumn column, int newIndex) {
			if(newIndex > SortInfo.Count) newIndex = SortInfo.Count;
			GridColumnSortInfo columnInfo = SortInfo[column];
			if(columnInfo != null && columnInfo.IsGrouped) return;
			if(columnInfo == null && newIndex < 0) return;
			SortInfo.BeginUpdate();
			try {
				if(newIndex == -1) {
					if(columnInfo != null) columnInfo.Remove();
				} else {
					if(columnInfo == null) columnInfo = SortInfo.Add(column, ColumnSortOrder.Ascending);
					if(columnInfo == null) return;
					if(columnInfo.SortIndex != newIndex) {
						SortInfo.MoveTo(columnInfo, columnInfo.Index + (columnInfo.SortIndex - newIndex));
					}
					if(columnInfo.IsGrouped) 
						SortInfo.GroupCount = columnInfo.Index - 1;
				}
			} finally {
				SortInfo.EndUpdate();
			}
		}
		protected internal void DoColumnChangeSortOrder(GridColumn column, ColumnSortOrder sortOrder) {
			GridColumnSortInfo columnInfo = SortInfo[column];
			if(columnInfo == null) {
				if(sortOrder != ColumnSortOrder.None) {
					columnInfo = SortInfo.Add(column, sortOrder);
					if(columnInfo != null && columnInfo.IsGrouped)
						SortInfo.GroupCount = columnInfo.Index;
				}
			} else {
				columnInfo.SortOrder = sortOrder;
			}
		}
		protected internal bool IsLoadingPanelVisible { get { return loadingPanel != null && loadingPanel.IsActive; } }
		protected internal LoadingPanel LoadingPanel {
			get {
				if(loadingPanel == null) {
					if(GridControl == null) return null;
					loadingPanel = new LoadingPanel(GridControl);
					loadingPanel.LookAndFeel = ElementsLookAndFeel;
				}
				return loadingPanel;
			}
		}
		protected internal LoadingAnimator LoadingAnimator {
			get {
				if(loadingAnimator == null) {
					if(GridControl == null) return null;
					loadingAnimator = new LoadingAnimator(GridControl, LoadingAnimator.LoadingImage);
				}
				return loadingAnimator;
			}
		}
		protected internal void OnSortGroupPropertyChanged() {
			if(IsLoading || SortInfo.Count == 0) return;
			SetTopRowIndexDirty();
			BeginDataUpdate();
			EndDataUpdateCore(true);
		}
		protected internal virtual GroupSummarySortInfoCollection GroupSummarySortInfoCore { get { return null; } }
		protected void SynchronizeSortingAndGrouping() { SynchronizeSortingAndGrouping(false); }
		protected virtual void SynchronizeSortingAndGrouping(bool force) {
			if(!IsInitialized) return;
			if(DesignMode) {
				LayoutChanged();
				return;
			}
			if(IsLevelDefault || !DataController.IsReady || (DataController.LockUpdate > 0 && !force)) {
				return;
			}
			SummarySortInfo[] summaryInfo = null;
			if(GroupSummarySortInfoCore != null) {
				GroupSummarySortInfoCore.Synchronize();
				if(GroupSummarySortInfoCore.Count > 0) InternalSynchronizeGroupSummary();
				summaryInfo = GroupSummarySortInfoCore.GetSummaryItems();
			}
			DataColumnSortInfo[] info = SortInfo.DataSortInfo;
			if(info.Length == 0 && DataController.SortInfo.Count == 0) return;
			try {
				BeginUpdate();
				bool resetCursor = false;
				if(info.Length > 0 && DataController.LockUpdate == 0) {
					resetCursor = true;
					SetCursor(Cursors.WaitCursor);
				}
				DataController.UpdateSortGroup(info, SortInfo.GroupCount, summaryInfo);
				if(resetCursor) ResetDefaultCursor();
			} finally {
				CancelUpdate();
				LayoutChangedSynchronized();
				MakeRowVisibleCore(FocusedRowHandle, false);
			}
		}
		protected virtual void InternalSynchronizeGroupSummary() { }
		protected virtual void MakeRowVisibleCore(int rowHandle, bool invalidate) { }
		protected virtual bool IsAutoHeight { get { return false; } }
		public GridColumn GetNearestCanFocusedColumn(GridColumn column) {
			if(VisibleColumns.IndexOf(column) == -1) column = null;
			if(column == null) column = VisibleColumns.Count > 0 ? VisibleColumns[0] : null;
			if(column == null) return null;
			return GetNearestCanFocusedColumn(column, VisibleColumns.IndexOf(column) > 0 ? -1 : 1);
		}
		protected internal GridColumn GetNearestCanFocusedColumn(GridColumn col, int delta) {
			return GetNearestCanFocusedColumn(col, delta, false);
		}
		protected internal virtual bool CanShowColumnInCustomizationForm(GridColumn col) {
			return false;
		}
		protected internal GridColumn GetNearestCanFocusedColumn(GridColumn col, int delta, bool changeFocusedRow) {
			return GetNearestCanFocusedColumn(col, delta, changeFocusedRow, new KeyEventArgs(0));
		}
		protected internal virtual GridColumn GetNearestCanFocusedColumn(GridColumn col, int delta, bool changeFocusedRow, KeyEventArgs e) {
			return col;
		}
		protected virtual int InternalFocusLock { get { return GridControl == null ? 0 : GridControl.EditorHelper.InternalFocusLock; } }
		protected virtual object ExtractEditingValue(GridColumn column, object editingValue) {
			return editingValue;
		}
		protected internal override void InitializeNew() {
		}
		protected override void InitializeVisualParameters() {
			base.InitializeVisualParameters();
			RefreshVisibleColumnsList();
			if(FocusedColumn == null && VisibleColumns.Count > 0) FocusedColumn = GetNearestCanFocusedColumn(GetVisibleColumn(0), 0);
		}
		protected override void InitializeDataParameters() {
			base.InitializeDataParameters();
			UpdateColumnHandles();
			UpdateSortGroupCollectionsCore();
		}
		protected internal virtual void UpdateColumnHandles() {
			Columns.UpdateHandlesCore();
		}
		protected override void SynchronizeDataController() {
			base.SynchronizeDataController();
			UpdateLastRowsInfo(!DataController.IsUpdateLocked, true);
			UpdateColumnHandles();
		}
		protected override void InitializeDataController() {
			base.InitializeDataController();
			UpdateDataControllerOptions();
			UpdateColumnHandles();
			SynchronizeSortingAndGrouping(true);
			SyncFormatRulesSummary();
			try {
				ApplyColumnsFilterCore();
			} catch {
			}
		}
		protected internal virtual void SynchronizeLookDataControllerSettings() {
			UpdateDataControllerOptions();
			UpdateColumnHandles();
			UpdateSortGroupCollectionsCore();
			DataController.UpdateSortGroup(SortInfo.DataSortInfo, SortInfo.GroupCount, new SummarySortInfo[0]);
		}
		protected virtual void UpdateDataControllerOptions() {
			DataController.ImmediateUpdateRowPosition = OptionsBehavior.ImmediateUpdateRowPosition;
			DataController.KeepFocusedRowOnUpdate = OptionsBehavior.KeepFocusedRowOnUpdate;
			DataController.ValuesCacheMode = OptionsBehavior.CacheValuesOnRowUpdating;
		}
		protected virtual void ClearViewInfo() { 
			if(ViewInfo == null) return;
			ViewInfo.IsReady = false;
			ViewInfo.Clear();
		}
		protected internal override void OnDetailInitialized() { 
			base.OnDetailInitialized();
			this.MoveFirst();
		}
		protected internal override void OnVisibleChanged() {
			if(FindPanel != null) FindPanel.Visible = IsVisible && GridControl.Visible;
		}
		protected internal override void OnEnter() {
			base.OnEnter();
			UpdateFocusedRowHandleOnEnter();
		}
		protected virtual void UpdateFocusedRowHandleOnEnter() {
			if(FocusedRowHandle == InvalidRowHandle && !WorkAsLookup)
				FocusedRowHandle = GetVisibleRowHandle(0);
		}
		protected override void OnDataController_ListChanged(object sender, ListChangedEventArgs e) {
			ResetDateFilterCache();
			ResetDataBoundSelectionCache();
			base.OnDataController_ListChanged(sender, e);
			if(e.ListChangedType == ListChangedType.Reset) CheckSelectFocusedRow();
		}
		public override void RefreshData() {
			ResetDateFilterCache();
			base.RefreshData();
		}
		void ResetDateFilterCache() {
			foreach(ColumnDateFilterInfo info in this.dateFilterCache.Values) {
				info.Cache = null;
			}
		}
		protected override void OnDataController_DataSourceChanged(object sender, EventArgs e) {
			FormatConditions.ResetEvaluatorsCore();
			FormatRules.ResetEvaluatorsCore();
			this.dateFilterCache.Clear();
			ClearPrevSelectionInfo();
			HideEditor();
			UpdateColumnHandles();
			ClearViewInfo();
			DestroyFilterData();
			if(IsLoading) {
				FocusedRowHandle = DataController.CurrentControllerRow;
				return;
			}
			if(!DataController.IsReady) {
				LayoutChangedSynchronized();
				if(DataController.DataSource == null) FocusedRowHandle = InvalidRowHandle;
				return;
			}
			BeginUpdate();
			try {
				if(Columns.Count == 0 && AllowAutoPopulateColumns) PopulateColumns();
				this.FocusedRowHandle = DataController.CurrentControllerRow;
			}
			finally {
				EndUpdate();
			}
			base.OnDataController_DataSourceChanged(sender, e);
		}
		protected virtual bool AllowAutoPopulateColumns { get { return OptionsBehavior.AutoPopulateColumns; } }
		protected virtual void RaiseColumnFilterChanged() {
			if(WorkAsLookup) OnPropertiesChanged();
			EventHandler handler = (EventHandler)this.Events[columnFilterChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual void RaiseColumnUnboundExpressionChanged(GridColumn column) {
			ColumnEventHandler handler = (ColumnEventHandler)this.Events[columnUnboundExpressionChanged];
			if(handler != null) handler(this, new ColumnEventArgs(column));
		}
		protected virtual void RaiseStartSorting() {
			EventHandler handler = (EventHandler)this.Events[startSorting];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual void RaiseEndSorting() {
			EventHandler handler = (EventHandler)this.Events[endSorting];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual void RaiseStartGrouping() {
			EventHandler handler = (EventHandler)this.Events[startGrouping];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual void RaiseEndGrouping() {
			EventHandler handler = (EventHandler)this.Events[endGrouping];
			if(handler != null) handler(this, EventArgs.Empty);
			UpdateNavigator();
		}
		public void RefreshRow(int rowHandle) {
			RefreshRow(rowHandle, true);
		}
		protected void RefreshRow(int rowHandle, bool updateEditor) {
			RefreshRow(rowHandle, updateEditor, true);
		}
		protected virtual void RefreshRow(int rowHandle, bool updateEditor, bool updateEditorValue) {
			if(updateEditor && rowHandle == FocusedRowHandle)
				RefreshEditor(updateEditorValue);
		}
		protected virtual void RefreshVisibleColumnsIndexes() {
			VisibleColumnsCore.UpdateIndexes();
		}
		protected class ColumnVisibleIndexSorter : IComparer {
			bool useFixedStyle;
			public ColumnVisibleIndexSorter(bool canUseFixedStyle) {
				this.useFixedStyle = canUseFixedStyle;
			}
			int IComparer.Compare(object a, object b) {
				GridColumn c1 = a as GridColumn, c2 = b as GridColumn;
				if(c1 == c2) return 0;
				if(c1 == null) return -1;
				if(c2 == null) return 1;
				if(useFixedStyle && c1.Fixed != c2.Fixed) {
					if(c1.Fixed == FixedStyle.Left) return -1;
					if(c2.Fixed == FixedStyle.Left) return 1;
					if(c1.Fixed == FixedStyle.Right) return 1;
					if(c2.Fixed == FixedStyle.Right) return -1;
				}
				int res = Comparer.Default.Compare(c1.VisibleIndex, c2.VisibleIndex);
				if(res == 0) {
					if(c1.grouped != c2.grouped) {
						return c1.grouped ? -1 : 1;
					}
					res = Comparer.Default.Compare(c1.AbsoluteIndex, c2.AbsoluteIndex);
				}
				return res;
			}
		}
		protected virtual bool CanShowColumn(GridColumn column) {
			return true;
		}
		protected internal virtual void RefreshVisibleColumnsList() {
			ArrayList tempList = new ArrayList();
			foreach(GridColumn column in Columns) {
				if(column.VisibleIndex > -1 && CanShowColumn(column)) tempList.Add(column);
			}
			tempList.Sort(new ColumnVisibleIndexSorter(CanUseFixedStyle));
			foreach(GridColumn column in Columns) {
				column.grouped = false;
			}
			PreProcessVisibleColumnsList(tempList);
			VisibleColumnsCore.ClearCore();
			for(int n = 0; n < tempList.Count; n++) {
				GridColumn column = tempList[n] as GridColumn;
				VisibleColumnsCore.AddCore(column);
				column.SetVisibleCore(true, n);
			}
			VisibleColumnsCore.SetDirty(false);
		}
		protected virtual void PreProcessVisibleColumnsList(ArrayList tempList) {
		}
		protected virtual int CheckRowHandle(int currentRowHandle, int newRowHandle) {
			if(IsValidRowHandle(newRowHandle)) return newRowHandle;
			if(newRowHandle == GridControl.InvalidRowHandle && WorkAsLookup) return newRowHandle;
			if(!IsValidRowHandle(0)) return GridControl.InvalidRowHandle;
			return currentRowHandle;
		}
		protected internal void BeginLockFocusedRowChange() {
			this.lockFocusedRowChange ++;
		}
		protected internal void EndLockFocusedRowChange() {
			this.lockFocusedRowChange --;
		}
		protected internal bool IsFocusedRowChangeLocked {
			get { return lockFocusedRowChange > 0; }
		}
		protected virtual void DoChangeFocusedRowInternal(int newRowHandle, bool updateCurrentRow) {
			if(this.lockFocusedRowChange != 0) return;
			if(!IsValidRowHandle(newRowHandle))
				newRowHandle = DevExpress.Data.DataController.InvalidRow;
			if(FocusedRowHandle == newRowHandle) return;
			int currentRowHandle = FocusedRowHandle;
			BeginLockFocusedRowChange();
			try {
				DoChangeFocusedRow(FocusedRowHandle, newRowHandle, updateCurrentRow);
			}
			finally {
				EndLockFocusedRowChange();
			}
			if(currentRowHandle != FocusedRowHandle)
				RaiseFocusedRowChanged(currentRowHandle, FocusedRowHandle);
		}
		protected virtual void SynchronizeFocusedRow() {
			if(IsSplitView && IsSplitSynchronizeFocus && RowCount == SplitOtherView.RowCount) ((ColumnView)SplitOtherView).FocusedRowHandle = focusedRowHandle;
		}
		protected virtual void DoChangeFocusedRow(int currentRowHandle, int newRowHandle, bool raiseUpdateCurrentRow) { }
		protected virtual void DoAfterFocusedColumnChanged(GridColumn prevFocusedColumn, GridColumn focusedColumn) {
			if(prevFocusedColumn != null && prevFocusedColumn.View == null) prevFocusedColumn = null;
			RaiseFocusedColumnChanged(prevFocusedColumn, focusedColumn);
		}
		protected virtual void SetEditingState() {
		}
		protected internal virtual void RaiseCustomDrawEmptyForeground(CustomDrawEventArgs e) {
			CustomDrawEventHandler handler = (CustomDrawEventHandler)this.Events[customDrawEmptyForeground];
			if(handler != null) handler(this, e);
		}
		protected virtual void RaiseShownEditor() {
			if(ActiveEditor != null && OptionsBehavior.AutoSelectAllInEditor) {
				ActiveEditor.SelectAll();
			} else {
				ActiveEditor.DeselectAll();
			}
			EventHandler handler = (EventHandler)this.Events[shownEditor];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual void RaiseHiddenEditor() {
			EventHandler handler = (EventHandler)this.Events[hiddenEditor];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual void RaiseInvalidRowException(InvalidRowExceptionEventArgs ex) {
			InvalidRowExceptionEventHandler handler = (InvalidRowExceptionEventHandler)this.Events[invalidRowException];
			if(handler != null) handler(this, ex);
			if(ex.ExceptionMode == ExceptionMode.DisplayError) {
				if(GridControl != null) GridControl.Capture = false;
				DialogResult dr = XtraMessageBox.Show(ElementsLookAndFeel, GridControl.FindForm(), ex.ErrorText + GridLocalizer.Active.GetLocalizedString(GridStringId.ColumnViewExceptionMessage), ex.WindowCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Error);
				if(dr == DialogResult.No)
					ex.ExceptionMode = ExceptionMode.Ignore;
			}
			if(ex.ExceptionMode == ExceptionMode.Ignore) CancelUpdateCurrentRow();
			if(ex.ExceptionMode == ExceptionMode.ThrowException) {
				throw(ex.Exception);
			}
		}
		protected virtual bool RaiseShowingEditor() {
			CancelEventHandler handler = (CancelEventHandler)this.Events[showingEditor];
			if(handler != null) {
				CancelEventArgs e = new CancelEventArgs(false);
				handler(this, e);
				if(e.Cancel) return false;
			}
			return true;
		}
		int lockColumnPositionChanged = 0;
		internal void LockRaiseColumnPositionChanged() { lockColumnPositionChanged++; }
		internal void UnlockRaiseColumnPositionChanged() { lockColumnPositionChanged--; }
		protected internal virtual void RaiseColumnPositionChanged(GridColumn column) {
			if(this.lockColumnPositionChanged != 0) return;
			EventHandler handler = (EventHandler)this.Events[columnPositionChanged];
			if(handler != null) handler(column, EventArgs.Empty);
		}
		protected virtual void RaiseCellValueChanged(CellValueChangedEventArgs e) {
			CellValueChangedEventHandler handler = (CellValueChangedEventHandler)this.Events[cellValueChanged];
			if(handler != null) handler(this, e);
		}
		protected virtual void RaiseCellValueChanging(CellValueChangedEventArgs e) {
			CellValueChangedEventHandler handler = (CellValueChangedEventHandler)this.Events[cellValueChanging];
			if(handler != null) handler(this, e);
		}
		public virtual void SetFocusedRowModified() {
			SetFocusedRowModifiedCore(true);
		}
		protected virtual void SetFocusedRowModifiedCore(bool modified) {
			if(modified && FocusedRowHandle != GridControl.AutoFilterRowHandle) DataController.BeginCurrentRowEdit();
			this.focusedRowModified = modified;
		}
		void AccessibleNotifyClients(int row, GridColumn focusedCol) {
			if(IsPrintingOnly) return;
			if(GridControl != null) GridControl.AccessibleNotifyClients(AccessibleEvents.Focus, -1);
		}
		protected virtual void RaiseFocusedRowChanged(int prevFocused, int focusedRowHandle) {
			AccessibleNotifyClients(focusedRowHandle, FocusedColumn);
			UpdateNavigator();
			ClearColumnErrors();
			SetFocusedRowModifiedCore(false);
			FocusedRowChangedEventHandler handler = (FocusedRowChangedEventHandler)this.Events[focusedRowChanged];
			if(handler != null) handler(this, new FocusedRowChangedEventArgs(prevFocused, focusedRowHandle));
		}
		internal void BeginInvoke(MethodInvoker method) {
			if(GridControl == null || !GridControl.IsHandleCreated) return;
			GridControl.BeginInvoke(method);
		}
		FocusedRowObjectChangedEventArgs delayedRowChanged = null;
		protected virtual void RaiseFocusedRowObjectChanged(FocusedRowObjectChangedEventArgs e) {
			if(e.lockCounter == 0) ClearColumnErrors();
			if(IsFocusedRowChangeLocked && (GridControl != null && GridControl.IsHandleCreated) && e.lockCounter < 3 && IsInListChangedEvent) {
				delayedRowChanged = e;
				BeginInvoke(() => {
					if(delayedRowChanged != null) {
						var ee = new FocusedRowObjectChangedEventArgs(FocusedRowHandle, delayedRowChanged.Row) { lockCounter = delayedRowChanged.lockCounter  + 1};
						this.delayedRowChanged = null;
						RaiseFocusedRowObjectChanged(ee);
					}
				});
				return;
			}
			this.delayedRowChanged = null;
			UpdateNavigator();
			SetFocusedRowModifiedCore(false);
			FocusedRowObjectChangedEventHandler handler = (FocusedRowObjectChangedEventHandler)this.Events[focusedRowObjectChanged];
			if(handler != null) handler(this, e);
		}
		protected virtual void RaiseFocusedColumnChanged(GridColumn prevFocused, GridColumn focusedColumn) {
			UpdateNavigator();
			AccessibleNotifyClients(FocusedRowHandle, focusedColumn);
			FocusedColumnChangedEventHandler handler = (FocusedColumnChangedEventHandler)this.Events[focusedColumnChanged];
			if(handler != null) handler(this, new FocusedColumnChangedEventArgs(prevFocused, focusedColumn));
		}
		protected internal override void FocusCreator() {
			if(ParentView == null || ParentInfo == null) return;
			ColumnView parentView = ParentView as ColumnView;
			if(parentView == null) return;
			if(parentView.FocusedRowHandle == ParentInfo.MasterRow.ParentControllerRow) return;
			parentView.MoveTo(ParentInfo.MasterRow.ParentControllerRow);
		}
		protected internal virtual void OnColumnSortInfoCollectionChanged(CollectionChangeEventArgs e) {
			if(SortInfo.IsLockUpdate && e.Action != CollectionChangeAction.Add) {
				UpdateSortGroupIndexes();
				return;
			}
			UpdateSortGroupIndexes();
			if(IsLoading || SortInfo.IsLockUpdate) return;
			SetTopRowIndexDirty();
			SynchronizeSortingAndGrouping();
			FireChanged();
			if(CanSynchronized)
				OnViewPropertiesChanged(SynchronizationMode.Data);
			else
				OnLookupViewPropertiesChanged();
		}
		protected override void OnEndDeserializing(string restoredVersion) {
			DataController.RePopulateColumns();
			UpdateColumnHandles();
			base.OnEndDeserializing(restoredVersion);
		}
		protected internal virtual void FireChangedColumns() {
			if(IsDeserializing || GridControl == null || !IsDesignMode) return;
			foreach(GridColumn col in Columns) {
				GridControl.FireChanged(col);
			}
		}
		protected internal virtual void OnColumnUnboundExpressionChanged(GridColumn column) {
			if(IsLoading || IsDeserializing) return;
			BeginDataUpdate();
			try {
				OnColumnUnboundChanged(column);
			}
			finally {
				EndDataUpdate();
			}
			RaiseColumnUnboundExpressionChanged(column);
		}
		protected internal virtual void OnColumnUnboundChanged(GridColumn column) {
			if(IsLoading  || IsDeserializing) return;
			BeginUpdate();
			try {
				DataController.RePopulateColumns();
				UpdateColumnHandles();
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual void OnColumnChanged(GridColumn column) {
			OnColumnChangedCore(column);
			FireChanged();
			if(column == null) return;
			EventHandler handler = (EventHandler)this.Events[columnChanged];
			if(handler != null) handler(column, EventArgs.Empty);
		}
		protected virtual void OnColumnChangedCore(GridColumn column) {
			if(column == null || column.Visible) {
				OnPropertiesChanged();
			}
		}
		protected virtual void OnColumnsCollectionChanged(object sender, CollectionChangeEventArgs e) {
			GridColumn column = e.Element as GridColumn;
			if(column == null) return;
			switch(e.Action) {
				case CollectionChangeAction.Add :
					OnColumnAdded(column);
					break;
				case CollectionChangeAction.Remove:
					if(ViewDisposing) return;
					if(GridControl != null && GridControl.GridDisposing) return;
					OnColumnDeleted(column);
					break;
			}
			if(this.GridControl != null)
				this.GridControl.OnDataSourceChanged();
			OnLookupViewPropertiesChanged();
			UpdateHeaderAccessible();
		}
		protected virtual void UpdateHeaderAccessible() { 
			if(HasAccessible)
				((ColumnViewAccessibleObject)DXAccessible).UpdateHeaderAccessible(); 
		}
		protected virtual string GenerateColumnName(string fieldName) {
			string res = "col" + fieldName, final;
			final = res = res.Replace(" ", "");
			int n = 1;
			while(true) {
				if(Container == null) break;
				if(Container.Components[final] == null) break;
				final = res + n++.ToString();
			}
			return final;
		}
		protected virtual void OnColumnAdded(GridColumn column) {
			if(Container != null && column.Site == null) {
				try {
					if(!IsLoading && column.FieldName != "") 
						Container.Add(column, GenerateColumnName(column.FieldName));
					else 
						Container.Add(column);
				}
				catch {
					Container.Add(column);
				}
			}
			if(!IsLoading) {
				SetColumnFieldName(column, column.FieldName);
				if(GridControl != null) GridControl.LockFireChanged();
				try {
					if(column.Name == "" && column.FieldName != "") column.SetName(GenerateColumnName(column.FieldName));
				}
				finally {
					if(GridControl != null) GridControl.UnlockFireChanged();
				}
				if(column.UnboundType != UnboundColumnType.Bound || column.FieldName.Contains("."))
					OnColumnUnboundChanged(column);
			}
		}
		protected virtual void OnColumnDeleted(GridColumn column) {
			bool bNeedResort = column.SortOrder != ColumnSortOrder.None ||
				column.GroupIndex > -1;
			VisibleColumnsCore.Hide(column);
			if(column == FocusedColumn) FocusedColumn = null;
			if(bNeedResort) {
				SortInfo.Remove(column);
			}
			MRUFilters.Remove(column);
			ActiveFilter.Remove(column);
			if(column.UnboundType != UnboundColumnType.Bound) OnColumnUnboundChanged(column);
			FormatRules.OnColumnRemoved(column);
		}
		protected virtual bool CheckCalculateLayout() { return false; }
		bool calculatingLayout = false;
		protected override bool CalculateData() {
			EnsureRuleValueProviders();
			return base.CalculateData();
		}
		protected override bool CalculateLayout() {
			if(this.calculatingLayout) return false;
			this.calculatingLayout = true;
			try {
				HideEditor();
				if(!CanCalculateLayout || !IsInitialized) return false;
				ViewInfo.IsReady = false;
				if((!GridControl.IsHandleCreated && !AllowLayoutWithoutHandle) && !IsDesignMode) return false;
				if(CheckCalculateLayout()) return false;
				RefreshVisibleColumnsList();
				if(CanSynchronized && !IsUpdateViewRect) {
					OnViewPropertiesChanged(SynchronizationMode.Visual);
				}
				if(!ForceParentLayoutChanged()) {
					this.calculatedRealViewHeight = -1;
					DoInternalLayout();
					UpdateScrollBars();
				}
			}
			finally {
				this.calculatingLayout = false;
			}
			return true;
		}
		protected virtual void UpdateScrollBars() { }
		protected virtual void DoInternalLayout() {
			ViewInfo.Calc(null, ViewRect);
		}
		protected internal virtual bool GetColumnAllowMerge(GridColumn column) {
			return false;
		}
		protected internal virtual void OnColumnWidthChanged(GridColumn column) { }
		protected internal virtual void OnColumnSizeChanged(GridColumn column) { }
		protected internal virtual int GetColumnAbsoluteIndex(GridColumn column) { return Columns.IndexOf(column);	}
		protected internal virtual int GetColumnMinWidth(GridColumn column) { return column.MinWidth; }
		protected internal virtual int GetColumnMaxWidth(GridColumn column) { return column.MaxWidth; }
		protected internal virtual void OnColumnVisibleIndexChanged(GridColumn column) {
			if(IsLoading || IsDeserializing || IsLockUpdate) return;
			RefreshVisibleColumnsList();
			RaiseColumnPositionChanged(column);
			LayoutChanged();
		}
		protected internal virtual void SetColumnVisibleIndex(GridColumn column, int newValue) {
			bool changed = false;
			if(newValue < 0) {
				changed = VisibleColumnsCore.Hide(column);
			}
			else {
				changed = VisibleColumnsCore.Show(column, newValue);
			}
			if(changed) {
				if(this.FocusedColumn == null && VisibleColumns.Count > 0) this.FocusedColumn = GetNearestCanFocusedColumn(GetVisibleColumn(0), 0);
				if(!IsLockUpdate) RaiseColumnPositionChanged(column);
				LayoutChanged();
			}
		}
		protected internal virtual HorzAlignment GetRowCellDefaultAlignment(int rowHandle, GridColumn column, HorzAlignment currentAlignment) {
			RaiseGetRowCellDefaultAlignment(rowHandle, column, ref currentAlignment);
			return currentAlignment;
		}
		protected virtual void RaiseGetRowCellDefaultAlignment(int rowHandle, GridColumn column, ref HorzAlignment currentAlignment) {
			RowCellAlignmentEventHandler handler = (RowCellAlignmentEventHandler)this.Events[rowCellDefaultAlignment]; 
			if(handler == null) return;
			alignmentEventArgs.HorzAlignment = currentAlignment;
			alignmentEventArgs.rowHandle = rowHandle;
			alignmentEventArgs.column = column;
			if(handler != null) handler(this, alignmentEventArgs);
			currentAlignment = alignmentEventArgs.HorzAlignment;
		}
		protected internal virtual void SetColumnWidth(GridColumn column, int newValue, bool force) {
			if(newValue < GetColumnMinWidth(column)) 
				newValue = GetColumnMinWidth(column);
			int maxWidth = GetColumnMaxWidth(column);
			if(maxWidth > 0 && newValue > maxWidth) newValue = maxWidth;
			if(newValue == column.Width) {
				if(column.VisibleWidth == newValue) return;
				if(!force) return;
			}
			column.width = newValue; 
			column.visibleWidth = newValue; 
			if(IsLoading) return;
			OnColumnSizeChanged(column);
		}
		protected internal override void OnRepositoryItemRemoved(RepositoryItem item) {
			if(ViewDisposing) return;
			bool changed = false;
			BeginUpdate();
			try {
				foreach(GridColumn col in Columns) {
					if(col.ColumnEdit == item) {
						col.ColumnEdit = null;
						changed = true;
					}
				}
			}
			finally {
				if(!changed) 
					CancelUpdate();
				else
					EndUpdate();
			}
		}
		protected internal override void OnEditorsRepositoryChanged() {
			BeginUpdate();
			try {
				foreach(GridColumn col in Columns) {
					col.ColumnEdit = null;
				}
			}
			finally {
				EndUpdate();
			}
			base.OnEditorsRepositoryChanged();
		}
		public int CalcColumnBestWidth(GridColumn column) {
			if(column == null || ViewInfo == null) return 0;
			return ViewInfo.CalcColumnBestWidth(column);
		}
		protected internal virtual string GetColumnError(int rowHandle, GridColumn column) {
			if(rowHandle == FocusedRowHandle) return GetColumnError(column);
			if(column == null) return DataController.GetErrorText(rowHandle);
			return DataController.GetErrorText(rowHandle, column.ColumnHandle);
		}
		public virtual string GetColumnError(GridColumn column) {
			if(FocusedRowHandle == GridControl.InvalidRowHandle) return null;
			string res = ErrorInfo[column];
			if(res != null && res.Length > 0) return res;
			if(column == null) return DataController.GetErrorText(FocusedRowHandle);
			return DataController.GetErrorText(FocusedRowHandle, column.ColumnHandle);
		}
		protected internal virtual DevExpress.XtraEditors.DXErrorProvider.ErrorType GetColumnErrorType(int rowHandle, GridColumn column) {
			if(rowHandle == FocusedRowHandle) return GetColumnErrorType(column);
			if(column == null) return DataController.GetErrorType(rowHandle);
			return DataController.GetErrorType(rowHandle, column.ColumnHandle);
		}
		public virtual DevExpress.XtraEditors.DXErrorProvider.ErrorType GetColumnErrorType(GridColumn column) {
			if(FocusedRowHandle == GridControl.InvalidRowHandle) return DevExpress.XtraEditors.DXErrorProvider.ErrorType.None;
			ErrorInfoEx info = (ErrorInfoEx)errorInfo;
			if(column == null) {
				if(info.ErrorText != null && info.ErrorText.Length > 0) return info.ErrorType;
				else return DataController.GetErrorType(FocusedRowHandle);
			}
			if(info[column] != null && info[column].Length > 0) return info.GetErrorType(column);
			return DataController.GetErrorType(FocusedRowHandle, column.ColumnHandle);
		}
		public virtual void SetColumnError(GridColumn column, string errorText, DevExpress.XtraEditors.DXErrorProvider.ErrorType errorType) {
			if(FocusedRowHandle == GridControl.InvalidRowHandle) return;
			ErrorInfoEx info = (ErrorInfoEx)errorInfo;
			info.SetError(column, errorText, errorType);
		}
		public virtual void SetColumnError(GridColumn column, string errorText) {
			SetColumnError(column, errorText, DevExpress.XtraEditors.DXErrorProvider.ErrorType.Default);
		}
		internal void GetColumnError(int rowHandle, GridColumn column, out string error, out DevExpress.XtraEditors.DXErrorProvider.ErrorType errorType) {
			error = GetColumnError(rowHandle, column);
			errorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.None;
			if(error != null && error.Length > 0) {
				errorType = GetColumnErrorType(rowHandle, column);
				if(errorType == DevExpress.XtraEditors.DXErrorProvider.ErrorType.None) error = null;
			}
		}
		[Browsable(false)]
		public virtual bool HasColumnErrors { get { return ErrorInfo.HasErrors; } }
		public virtual void ClearColumnErrors() {
			ErrorInfo.ClearErrors();
		}
		protected virtual ErrorInfo ErrorInfo { get { return errorInfo; } }
		protected virtual void OnErrorInfo_Changed(object sender, EventArgs e) {
			RefreshRow(FocusedRowHandle, true, false);
		}
		protected internal virtual void SetColumnAbsoluteIndex(GridColumn column, int newValue) {
			Columns.SetItemIndex(column, newValue);
		}
		protected internal virtual void SetColumnFixedStyle(GridColumn column, FixedStyle newValue) {
		}
		protected internal virtual void SetColumnFieldName(GridColumn column, string newValue) {
			if(!DataController.IsReady || DataController.Columns[newValue] == null) {
				column.SetFieldNameCore(newValue);
				column.OnChanged();
				return;
			}
			if(DataController.Columns[newValue] != null) {
				column.SetFieldNameCore(newValue);
				column.ColumnHandle = DataController.Columns.GetColumnIndex(newValue);
			}
		}
		protected virtual void UpdateEditorProperties(BaseEdit editor) { }
		protected virtual void UpdateEditor(RepositoryItem ritem, UpdateEditorInfoArgs args) {
			BaseEdit be = GridControl.EditorHelper.UpdateEditor(ritem, args);
			UpdateEditorProperties(be);
			GridControl.EditorHelper.ShowEditor(be, GridControl);
			if(ActiveEditor != null) {
				SetEditingState();
				ActiveEditor.EditValueChanged += new EventHandler(OnActiveEditor_ValueChanged);
				ActiveEditor.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(OnActiveEditor_ValueChanging);
				ActiveEditor.Modified += new EventHandler(OnActiveEditor_ValueModified);
				ActiveEditor.LostFocus += new EventHandler(OnActiveEditor_LostFocus);
				ActiveEditor.GotFocus += new EventHandler(OnActiveEditor_GotFocus);
				ActiveEditor.MouseDown +=new MouseEventHandler(OnActiveEditor_MouseDown);
				RaiseShownEditor();
				UpdateNavigator();
			}
		}
		protected virtual void OnActiveEditor_MouseDown(object sender, MouseEventArgs e) {
			if(GridControl == null || ActiveEditor == null) return;
			Point screenPoint = ActiveEditor.PointToScreen(e.Location);
			Point gridPoint = PointToClient(screenPoint);
			MouseEventArgs ea = new MouseEventArgs(e.Button, e.Clicks, gridPoint.X, gridPoint.Y, e.Delta);
			DoubleClickChecker.CheckDoubleClick(this, ea, true, RaiseDoubleClick);
		}
		protected virtual void OnActiveEditor_ValueChanged(object sender, EventArgs e) {
		}
		protected virtual void OnActiveEditor_ValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e) {
			RaiseCellValueChanging(new CellValueChangedEventArgs(FocusedRowHandle, FocusedColumn, e.NewValue));
		}
		protected virtual void OnActiveEditor_ValueModified(object sender, EventArgs e) {
			SetFocusedRowModified();
		}
		protected virtual void OnActiveEditor_LostFocus(object sender, EventArgs e) {
			HideHint();
			if(ActiveEditor != null) {
				OnLostFocus();
			}
		}
		protected virtual void OnActiveEditor_GotFocus(object sender, EventArgs e) {
			if(ActiveEditor != null) {
				ViewInfo.PaintAnimatedItems = false;
				Invalidate();
			}
		}
		protected virtual void OnValidatingCurrentRow(ValidateControllerRowEventArgs e) {
			ValidateRowEventArgs vrArgs = CreateValidateRowEventArgs(FocusedRowHandle, e.Row);
			vrArgs.Valid = e.Valid;
			vrArgs.ErrorText = e.ErrorText;
			TryValidateNewItemRowViaAnnotationAttributes(vrArgs);
			RaiseValidateRow(vrArgs);
			if(!vrArgs.Valid)
				throw new WarningException(vrArgs.ErrorText);
		}
		void TryValidateNewItemRowViaAnnotationAttributes(ValidateRowEventArgs vrArgs) {
			if(IsNewItemRow(FocusedRowHandle) && vrArgs.Valid) {
				foreach(GridColumn column in Columns) {
					vrArgs.TryValidateViaAnnotationAttributes(this, column);
					if(!vrArgs.Valid)
						break;
				}
			}
		}
		protected virtual ValidateRowEventArgs CreateValidateRowEventArgs(int rowHandle, object row) {
			return new ValidateRowEventArgs(rowHandle, row);
		}
		protected internal virtual void RaiseValidateRow(ValidateRowEventArgs e) {
			ValidateRowEventHandler handler = (ValidateRowEventHandler)this.Events[validateRow];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseRowUpdated(RowObjectEventArgs e) {
			RowObjectEventHandler handler = (RowObjectEventHandler)this.Events[rowUpdated];
			if(handler != null) handler(this, e);
		}
		protected internal virtual bool HasCustomRowFilter { 
			get {
				return (RowFilterEventHandler)this.Events[customRowFilter] != null;
			}
		}
		RowFilterEventArgs CachedRowFilterEventArgs = new RowFilterEventArgs(-1);
		protected internal virtual int RaiseCustomRowFilter(int listSourceRow, bool fit) {
			RowFilterEventHandler handler = (RowFilterEventHandler)this.Events[customRowFilter];
			if(handler == null) return -1;
			RowFilterEventArgs e = CachedRowFilterEventArgs;
			e.GoTo(listSourceRow, fit);
			handler(this, e);
			if(e.Handled) return e.Visible ? 1 : 0;
			return -1;
		}
		protected internal virtual void RaiseRowLoaded(int rowHandle) {
			RowEventHandler handler = (RowEventHandler)this.Events[rowLoaded];
			if(handler != null) handler(this, new RowEventArgs(rowHandle));
			if(rowHandle == FocusedRowHandle) RaiseFocusedRowLoaded(rowHandle);
		}
		protected internal virtual void RaiseAsyncCompleted() {
			EventHandler handler = (EventHandler)this.Events[asyncCompleted];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected internal virtual void RaiseFocusedRowLoaded(int rowHandle) {
			RowEventHandler handler = (RowEventHandler)this.Events[focusedRowLoaded];
			if(handler != null) handler(this, new RowEventArgs(rowHandle));
		}
		protected internal virtual bool RaiseBeforeLeaveRow(RowAllowEventArgs e) {
			RowAllowEventHandler handler = (RowAllowEventHandler)this.Events[beforeLeaveRow];
			if(handler != null) handler(this, e);
			return e.Allow;
		}
		protected virtual void SelectAnchorRangeCore(bool controlPressed, bool allowCells) {
			if(!controlPressed) {
				ClearSelectionCore();
			}
			SelectRow(SelectionAnchorRowHandle);
			SelectRange(SelectionAnchorRowHandle, FocusedRowHandle);
			ViewInfo.PaintAnimatedItems = false;
			InvalidateSelection(new SelectionChangedEventArgs(CollectionChangeAction.Refresh, InvalidRowHandle));
		}
		protected virtual int GetValidSelectionAnchor(int rowHandle) {
			if(IsNewItemRow(rowHandle)) {
				return GetVisibleRowHandle(RowCount - 1);
			}
			return rowHandle;
		}
		protected virtual void SetSelectionAnchor(int prevFocusedHandle, GridColumn prevFocusedColumn) {
			if(prevFocusedColumn == null) 
				SelectionAnchorRowHandle = GetValidSelectionAnchor(prevFocusedHandle);
			else
				SelectionAnchor = new GridCell(GetValidSelectionAnchor(prevFocusedHandle), prevFocusedColumn);
		}
		internal void DoAfterMoveFocusedRow(KeyEventArgs e, int prevFocusedHandle, GridColumn prevFocusedColumn, BaseHitInfo hitInfo) {
			DoAfterMoveFocusedRow(e, prevFocusedHandle, prevFocusedColumn, hitInfo, true);
		}
		protected internal virtual bool AllowChangeSelectionOnNavigation { get { return true; } }
		protected internal virtual void DoAfterMoveFocusedRow(KeyEventArgs e, int prevFocusedHandle, GridColumn prevFocusedColumn, BaseHitInfo hitInfo, bool allowCells) {
			SynchronizeFocusedRow();
			if(!IsMultiSelect) return;
			if(!CheckAllowChangeSelectionOnNavigation(e)) {
				return;
			}
			bool shiftPressed = e.Shift, 
				controlPressed = e.Control;
			if(!shiftPressed) SelectionAnchorRowHandle = InvalidRowHandle;
			if(FocusedRowHandle != prevFocusedHandle) {
				if(shiftPressed && e.KeyCode != Keys.Tab) {
					if(SelectionAnchorRowHandle == InvalidRowHandle)
						SetSelectionAnchor(prevFocusedHandle, prevFocusedColumn);
					try {
						this.lockSelectionInvalidate = true;
						SelectAnchorRangeCore(controlPressed, allowCells);
					}
					finally {
						this.lockSelectionInvalidate = false;
					}
					return;
				}
			}
			if(controlPressed) {
				if(e.KeyCode == Keys.None || e.KeyCode == Keys.LButton) InvertFocusedRowSelectionCore(hitInfo);
				return;
			}
			if(FocusedRowHandle == prevFocusedHandle) {
				DoAfterMoveFocusedRowColumn(e.KeyCode, prevFocusedHandle, prevFocusedColumn);
				if(e.KeyCode != Keys.None && e.KeyCode != Keys.LButton && e.KeyCode != Keys.RButton) return;
			}
			if(e.KeyCode == Keys.LButton || e.KeyCode == Keys.RButton) {
				if(IsFocusedRowSelectedCore()) return;
			}
			SelectFocusedRowCore();
		}
		protected virtual bool CheckAllowChangeSelectionOnNavigation(KeyEventArgs e) {
			if(e.KeyCode != Keys.None && e.Modifiers == Keys.None && !AllowChangeSelectionOnNavigation) {
				if(e.KeyCode != Keys.LButton && e.KeyCode != Keys.RButton) {
					SelectionAnchorRowHandle = InvalidRowHandle;
					return false;
				}
			}
			return true;
		}
		protected virtual bool IsFocusedRowSelectedCore() { return IsRowSelected(FocusedRowHandle); }
		public virtual EditorShowMode GetShowEditorMode() {
			EditorShowMode res = OptionsBehavior.EditorShowMode;
			if(res == EditorShowMode.Default) return EditorShowMode.MouseDown;
			return res;
		}
		protected virtual void DoAfterMoveFocusedRowColumn(Keys byKey, int prevFocusedHandle, GridColumn prevFocusedColumn) { }
		protected internal virtual void InvertFocusedRowSelectionCore(BaseHitInfo hitInfo) {
			InvertRowSelection(FocusedRowHandle);
		}
		protected internal virtual void SelectFocusedRowCore() {
			if(!IsMultiSelect) return;
			bool lockInvalidate = SelectedRowsCount < 2;
			int prevSelected = InvalidRowHandle;
			try {
				if(lockInvalidate) {
					this.lockSelectionInvalidate = true;
					if(SelectedRowsCount == 1) prevSelected = GetSelectedRows()[0];
				}
				BeginSelection();
				try {
					ClearSelectionCore();
					SelectRow(FocusedRowHandle);
				}
				finally {
					EndSelection();
				}
			} finally {
				if(lockInvalidate) {
					this.lockSelectionInvalidate = false;
					ViewInfo.PaintAnimatedItems = false;
					InvalidateSelection(new SelectionChangedEventArgs(CollectionChangeAction.Remove, prevSelected));
					InvalidateSelection(new SelectionChangedEventArgs(CollectionChangeAction.Add, FocusedRowHandle));
				}
			}
		}
		protected virtual GridCell SelectionAnchor {
			get {
				if(selectionAnchor == null) selectionAnchor = new GridCell(InvalidRowHandle, null);
				return selectionAnchor;
			} 
			set {
				selectionAnchor = value;
				OnSelectionAnchorChanged();
			}
		}
		protected virtual void OnSelectionAnchorChanged() { }
		protected internal virtual int SelectionAnchorRowHandle {
			get { return SelectionAnchor.RowHandle; }
			set {
				SelectionAnchor = new GridCell(GetValidSelectionAnchor(value), FocusedColumn);
			}
		}
		protected virtual void OnDataController_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			if(GridControl == null) return;
			if(DataController.Selection.IsSelectionLocked) return;
			CheckSelectionOnChanged(e);
		}
		protected virtual void CheckSelectionOnChanged(SelectionChangedEventArgs e) {
			if(IsMultiSelect) {
				ViewInfo.PaintAnimatedItems = false;
				InvalidateSelection(e);
				if(IsSameSelection()) return;
				OnSelectionChangedCore(e);
				RaiseSelectionChanged(e);
				UpdateNavigator();
			}
		}
		protected virtual void OnSelectionChangedCore(SelectionChangedEventArgs e) {
		}
		protected void ClearPrevSelectionInfo() { prevSelection = null; }
		object prevSelection = null;
		protected object GetPrevSelection() { return prevSelection; }
		protected virtual bool IsSameSelection() {
			object prev = GetPrevSelection();
			this.prevSelection = GetCurrentSelection();
			if(prev == null) return false;
			return CompareSelection(prev, prevSelection);
		}
		protected virtual bool CompareSelection(object prev, object current) {
			DevExpress.Data.Selection.SelectionController.SelectionInfo s1, s2;
			s1 = prev as DevExpress.Data.Selection.SelectionController.SelectionInfo;
			s2 = current as DevExpress.Data.Selection.SelectionController.SelectionInfo;
			if(s1 != null) return s1.Equals(s2);
			return s2 == null;
		}
		protected virtual object GetCurrentSelection() {
			if(!IsMultiSelect || SelectedRowsCount == 0) return null;
			return DataController.Selection.GetSelectionInfo();
		}
		protected virtual void InvalidateSelection(SelectionChangedEventArgs e) {
			if(this.lockSelectionInvalidate) return;
			ViewInfo.PaintAnimatedItems = false;
			Invalidate();
		}
		public virtual void InvertRowSelection(int rowHandle) {
			if(!IsMultiSelect) return;
			if(IsRowSelected(rowHandle))
				UnselectRow(rowHandle);
			else 
				SelectRow(rowHandle);
		}
		public virtual void SelectAll() {
			if(!IsMultiSelect) return;
			if(IsDataBoundSelection)
				DataBoundSelectAll();
			else
				DataController.Selection.SelectAll();
		}
		public virtual void SelectRange(int startRowHandle, int endRowHandle) {
			if(startRowHandle == InvalidRowHandle || endRowHandle == InvalidRowHandle) return;
			if(startRowHandle == endRowHandle) {
				SelectRow(startRowHandle);
				return;
			}
			int startIndex = GetVisibleIndex(startRowHandle),
				endIndex = GetVisibleIndex(endRowHandle);
			if(startIndex < 0 || endIndex < 0) return;
			if(startIndex > endIndex) {
				int a = endIndex;
				endIndex = startIndex;
				startIndex = a;
			} 
			BeginSelection();
			try {
				for(int n = startIndex; n < endIndex + 1; n++) {
					int rowHandle = GetVisibleRowHandle(n);
					SelectRow(rowHandle);
				}
			}
			finally {
				EndSelection();
			}
		}
		protected virtual bool IsDataBoundSelection {
			get {
				return !string.IsNullOrEmpty(DataBoundSelectionField);
			}
		}
		protected virtual bool GetDataBoundSelectedValue(int rowHandle) {
			if(!IsDataBoundSelection || !CanSelectRow(rowHandle)) return false;
			object val = DataController.GetRowValue(rowHandle, DataBoundSelectionField);
			if(val is bool) return (bool)val;
			if(val == null) return false;
			try {
				if(val is int) return ((int)val) != 0;
				int i = Convert.ToInt32(val);
				return i != 0;
			} catch { }
			return false;
		}
		int cachedDataBoundSelectedCount = -1;
		protected void ResetDataBoundSelectionCache() {
			this.cachedDataBoundSelectedCount = -1;
		}
		protected virtual bool GetDataBoundSelectedRow(int rowHandle) {
			return GetDataBoundSelectedValue(rowHandle);
		}
		protected void SetDataBoundSelectedRow(int rowHandle, bool selected) {
			if(GetDataBoundSelectedRow(rowHandle) == selected) return;
			if(!IsDataBoundSelection || !CanSelectRow(rowHandle)) return;
			ResetDataBoundSelectionCache();
			SetDataBoundSelectedRowCore(rowHandle, selected);
			CheckSelectionOnChanged(new SelectionChangedEventArgs(selected ? CollectionChangeAction.Add : CollectionChangeAction.Remove, rowHandle));
		}
		protected virtual void SetDataBoundSelectedRowCore(int rowHandle, bool selected) {
			DataController.SetRowValue(rowHandle, DataBoundSelectionField, selected);
		}
		protected virtual bool CanSelectRow(int rowHandle) {
			if(!IsDataBoundSelection) return true;
			return rowHandle >= 0 && IsValidRowHandle(rowHandle);
		}
		protected virtual string DataBoundSelectionField {  get { return null; } }
		protected virtual void ClearDataBoundSelection() {
			if(!IsDataBoundSelection) return;
			for(int i = 0; i < DataRowCount; i++) {
				SetDataBoundSelectedRow(i, false);
			}
			ResetDataBoundSelectionCache();
			CheckSelectionOnChanged(new SelectionChangedEventArgs(CollectionChangeAction.Refresh, InvalidRowHandle));
		}
		protected virtual void DataBoundSelectAll() {
			if(!IsDataBoundSelection) return;
			for(int i = 0; i < DataRowCount; i++) {
				SetDataBoundSelectedRow(i, true);
			}
			ResetDataBoundSelectionCache();
			CheckSelectionOnChanged(new SelectionChangedEventArgs(CollectionChangeAction.Refresh, InvalidRowHandle));
		}
		protected virtual int GetDataBoundSelectedCount() {
			if(!IsDataBoundSelection) return 0;
			if(cachedDataBoundSelectedCount != -1) return cachedDataBoundSelectedCount;
			int res = 0;
			for(int i = 0; i < DataRowCount; i++) {
				res += GetDataBoundSelectedRow(i) ? 1 : 0;
			}
			cachedDataBoundSelectedCount = res;
			return res;
		}
		protected virtual int[] GetDataBoundSelection() {
			if(!IsDataBoundSelection) return new int[0];
			List<int> res = new List<int>();
			for(int i = 0; i < DataRowCount; i++) {
				if(GetDataBoundSelectedRow(i)) res.Add(i);
			}
			return res.ToArray();
		}
		public virtual bool IsRowSelected(int rowHandle) {
			if(IsMultiSelect) {
				if(IsDataBoundSelection) return GetDataBoundSelectedRow(rowHandle);
				return DataController.Selection.GetSelected(rowHandle);
			}
			return (rowHandle == FocusedRowHandle);
		}
		public virtual void SelectRow(int rowHandle) {
			if(!IsMultiSelect) return;
			if(IsDataBoundSelection) 
				SetDataBoundSelectedRow(rowHandle, true);
			else
				DataController.Selection.SetSelected(rowHandle, true);
		}
		public virtual void UnselectRow(int rowHandle) {
			if(!IsMultiSelect) return;
			if(IsDataBoundSelection)
				SetDataBoundSelectedRow(rowHandle, false);
			else
				DataController.Selection.SetSelected(rowHandle, false);
		}
		protected virtual bool ClearSelectionAllowed { get { return IsMultiSelect; } }
		public virtual void ClearSelection() {
			if(!ClearSelectionAllowed) return;
			SelectionAnchorRowHandle = InvalidRowHandle;
			ClearSelectionCore();
		}
		protected internal virtual void ClearSelectionCore() {
			if(!ClearSelectionAllowed) return;
			if(IsDataBoundSelection)
				ClearDataBoundSelection();
			else
				DataController.Selection.Clear();
		}
		[Browsable(false)]
		public virtual int SelectedRowsCount {
			get {
				if(IsMultiSelect) {
					if(IsDataBoundSelection) return GetDataBoundSelectedCount();
					return DataController.Selection.Count;
				}
				if(FocusedRowHandle == InvalidRowHandle) return 0;
				return 1;
			}
		}
		public virtual int[] GetSelectedRows() {
			if(!IsMultiSelect) {
				if(FocusedRowHandle == InvalidRowHandle) return new int[] { };
				return new int[] { FocusedRowHandle };
			}
			if(IsDataBoundSelection) return GetDataBoundSelection();
			return DataController.Selection.GetSelectedRows();
		}
		protected override bool OnAllowSerializationProperty(OptionsLayoutBase options, string propertyName, int id) {
			if(!base.OnAllowSerializationProperty(options, propertyName, id)) return false;
			OptionsLayoutGrid optGrid = options as OptionsLayoutGrid;
			if(optGrid == null) return true;
			if(optGrid.StoreDataSettings && id == LayoutIdData) return true;
			if(id == LayoutIdAppearance) return optGrid.StoreAppearance;
			if(id == LayoutIdFormatRules) return optGrid.StoreFormatRules;
			if(optGrid.StoreVisualOptions && id == LayoutIdOptionsView) return true;
			if(optGrid.Columns.StoreLayout && id == LayoutIdColumns) return true;
			if(optGrid.StoreAllOptions) return true;
			return false;
		}
		protected override void OnResetSerializationProperties(OptionsLayoutBase options) {
			base.OnResetSerializationProperties(options);
			OptionsLayoutGrid optGrid = options as OptionsLayoutGrid;
			if(optGrid == null || optGrid.StoreAppearance) {
				Appearance.Reset();
				AppearancePrint.Reset();
			}
			if(optGrid != null && optGrid.StoreAllOptions) optGrid = null;
			if(optGrid == null || optGrid.StoreDataSettings) {
				SortInfo.Clear();
				SortInfo.GroupCount = 0;
				ActiveFilter.Clear();
				MRUFilters.Clear();
			}
			if(optGrid == null || optGrid.StoreVisualOptions) {
				OptionsView.Reset();
			}
			if(optGrid == null || optGrid.StoreAllOptions) {
				OptionsFind.Reset();
				OptionsFilter.Reset();
				OptionsBehavior.Reset();
				OptionsSelection.Reset();
			}
		}
		protected internal virtual void OnFilterPopupCloseUp(GridColumn column) {
		}
		protected internal FilterPopup FilterPopup {
			get { return filterPopup; }
			set {
				if(FilterPopup == value) return;
				if(filterPopup != null) {
					bool focused = GridControlIsFocused;
					filterPopup.Dispose();
					filterPopup = null;
					if(focused) GridControl.Focus();
				}
				filterPopup = value;
			}
		}
 		protected internal virtual void DestroyFilterPopup() {
			FilterPopup = null;
		}
		protected virtual DevExpress.XtraGrid.Filter.FilterCustomDialog CreateCustomFilterDialog(GridColumn column) {
			if(!OptionsFilter.UseNewCustomFilterDialog)
				return new DevExpress.XtraGrid.Filter.FilterCustomDialog(column);
			return new DevExpress.XtraGrid.Filter.FilterCustomDialog2(column, this.Columns);
		}
		protected virtual ColumnFilterInfo DoCustomFilter(GridColumn column, ColumnFilterInfo filterInfo) {
			CustomFilterDialogEventArgs e = new CustomFilterDialogEventArgs(column, filterInfo);
			CustomFilterDialogEventHandler handler = (CustomFilterDialogEventHandler)this.Events[customFilterDialog];
			if(handler != null) handler(this, e);
			if(!e.Handled) {
				if(ColumnsContainedCollector.IsContained(ActiveFilter.NonColumnFilterCriteria, new OperandProperty(column.FilterFieldName))) {
					this.ShowFilterEditor(column);
					filterInfo = null;
				} else {
					using(DevExpress.XtraGrid.Filter.FilterCustomDialog dlg = CreateCustomFilterDialog(column)) {
						dlg.SetRightToLeft(IsRightToLeft);
						InitDialogFormProperties(dlg);
						dlg.UseAsteriskAsWildcard = e.UseAsteriskAsWildcard;
						if(this.WorkAsLookup)
							dlg.TopMost = true; 
						dlg.ShowDialog(GridControl.FindForm());
						filterInfo = null;
					}
				}
			} else {
				filterInfo = e.FilterInfo;
			}
			return filterInfo;
		}
		protected internal virtual string GetFilterDisplayTextByColumn(GridColumn column, object val) {
			string result = null;
			if(column.GetFilterMode() == ColumnFilterMode.Value) {
				try {
					result = GetDisplayTextByColumnValue(
						column.OptionsFilter.FilterBySortField.ToBoolean(false) ? GetColumnFieldNameSortGroup(column) : column, val);
				} catch { }
			}
			if(result == null) {
				if(val != null) {
					return val.ToString();
				} else {
					return string.Empty;
				}
			} else if(result.Length == 0 && val is string) {
				return val.ToString();
			} else {
				return result;
			}
		}
		public virtual void ShowCustomFilterDialog(GridColumn column) {
			ApplyColumnFilter(column, new FilterItem("custom", new FilterItem("custom", 1)));
		}
		static DateTime ObjectToDateTime(object data) {
			FilterItem item = data as FilterItem;
			if(item == null || !(item.Value is DateTime)) return DateTime.MinValue;
			return Convert.ToDateTime(item.Value);
		}
		protected internal virtual void ApplyCheckedColumnFilter(GridColumn column, object stringValues, List<object> checkedValues) {
			bool showBlanks = false;
			CriteriaOperator blanks = new FunctionOperator(FunctionOperatorType.IsNullOrEmpty, new OperandProperty(column.FilterFieldName));
			for(int i = checkedValues.Count - 1; i >= 0; i--) {
				FilterItem item = checkedValues[i] as FilterItem;
				if(item != null && FilterPopup.IsNullValue(item.Value)) {
					showBlanks = true;
					checkedValues.RemoveAt(i);
				}
			}
			if(CheckedColumnFilterPopup.AllowCheckedDateList(column)) {
				List<DateTime> list = checkedValues.ConvertAll<DateTime>(new Converter<object, DateTime>(ObjectToDateTime));
				CriteriaOperator op = MultiselectRoundedDateTimeFilterHelper.DatesToCriteria(column.FilterFieldName, list);
				if(showBlanks) {
					if(ReferenceEquals(op, null))
						op = blanks;
					else op |= blanks;
				}
				if(!ReferenceEquals(op, null)) 
					ActiveFilter.Add(column, new ColumnFilterInfo(op));
				else ActiveFilter.Remove(column);
			} else {
				InOperator oper = new InOperator(new OperandProperty(column.FilterFieldName));
				foreach(object item in checkedValues) {
					FilterItem fi = item as FilterItem;
					if(fi != null)
						oper.Operands.Add(new OperandValue(fi.Value));
				}
				CriteriaOperator op = null;
				switch(oper.Operands.Count) {
					case 0:
						break;
					case 1:
						op = oper.LeftOperand == ((CriteriaOperator)oper.Operands[0]);
						break;
					case 2:
						op = 
							oper.LeftOperand == ((CriteriaOperator)oper.Operands[0]) 
							|
							oper.LeftOperand == ((CriteriaOperator)oper.Operands[1]);
						break;
					default:
						op = oper;
						break;
				}
				if(showBlanks) op |= blanks;
				if(ReferenceEquals(op, null))
					ActiveFilter.Add(column, new ColumnFilterInfo());
				else ActiveFilter.Add(column, new ColumnFilterInfo(op));
			}
		}
		protected internal virtual void ApplyColumnFilter(GridColumn column, FilterItem listBoxItem) {
			if(listBoxItem == null) return;
			string filterField = column.FilterFieldName;
			int predefinedIndex = 1000;
			ColumnFilterInfo newInfo = new ColumnFilterInfo();
			if(listBoxItem.Value is FilterItem) {
				predefinedIndex = (int)(listBoxItem.Value as FilterItem).Value;
			}
			if(predefinedIndex == 1) { 
				newInfo = DoCustomFilter(column, newInfo);
				if(newInfo != null) newInfo.SetFilterKind(ColumnFilterKind.Predefined);
			}
			if(predefinedIndex > 1) {
				object val = listBoxItem.Value;
				if(val is ColumnFilterInfo)
					newInfo = val as ColumnFilterInfo;
				else {
					switch(predefinedIndex) {
						case 2:
							if(column.GetFilterMode() == ColumnFilterMode.DisplayText || column.ColumnType.Equals(typeof(string)))
								newInfo = new ColumnFilterInfo(new FunctionOperator(FunctionOperatorType.IsNullOrEmpty, new OperandProperty(filterField)));
							else
								newInfo = new ColumnFilterInfo(new OperandProperty(filterField).IsNull());
							break;
						case 3:
							if(column.GetFilterMode() == ColumnFilterMode.DisplayText || column.ColumnType.Equals(typeof(string)))
								newInfo = new ColumnFilterInfo(new FunctionOperator(FunctionOperatorType.IsNullOrEmpty, new OperandProperty(filterField)).Not()); 
							else
								newInfo = new ColumnFilterInfo(new OperandProperty(filterField).IsNotNull());
							break;
						default:
							newInfo = new ColumnFilterInfo(column, val);
							break;
					}
					newInfo.SetFilterKind(predefinedIndex < 4 ? ColumnFilterKind.Predefined : ColumnFilterKind.User);
				}
			}
			if(newInfo != null) {
				newInfo.UpdateValueFilterIfNeeded(column);
				ActiveFilter.Add(column, newInfo);
			}
		}
		[Obsolete("This method is obsolete. Use GetFilterDisplayText, OnCustomFilterDisplayText or OnCustomFilterDisplayText instead.", true)]
		protected internal virtual string CalcFilterDisplayText(GridColumn column, bool isEqual, string valueDisplayText, object val) {
			return null;
		}
		protected internal virtual bool IsRoundDateTime(GridColumn column) {
			return GetRowCellRepositoryItem(InvalidRowHandle, column) is RepositoryItemDateEdit;
		}
		protected virtual object[] GetFilterPopupValues(GridColumn column, bool showAll, OperationCompleted completed) {
			return DataController.GetUniqueColumnValues(column.FilterFieldName, OptionsFilter.ColumnFilterPopupMaxRecordsCount, showAll, IsRoundDateTime(column), completed, column.IsCheckedListFilterPopup || column.IsListFilterPopup);
		}
		protected virtual ColumnFilterPopup CreateFilterPopup(GridColumn column, Control ownerControl, object creator) {
			return new ColumnFilterPopup(this, column, ownerControl, creator);
		}
		protected virtual CheckedColumnFilterPopup CreateCheckedFilterPopup(GridColumn column, Control ownerControl, object creator) {
			return new CheckedColumnFilterPopup(this, column, ownerControl, creator);
		}
		protected virtual DateFilterPopup CreateDateFilterPopup(GridColumn column, Control ownerControl, object creator) {
			return new DateFilterPopup(this, column, ownerControl, creator);
		}
		protected internal bool GridControlIsFocused { get { return !IsPrintingOnly && GridControl != null && GridControl.IsFocused; } }
		protected internal virtual bool CanShowPopupObjects { 
			get {
				if(GridControlIsFocused) return true;
				if(!WorkAsLookup) return false;
				if(LookUpOwner.OwnerEdit != null && LookUpOwner.OwnerEdit.IsPopupOpen && LookUpOwner.OwnerEdit.PopupFormCore.ContainsFocus) return true;
				return false;
			} 
		}
		public abstract void ShowFilterPopup(GridColumn column);
		protected internal virtual void ShowFilterPopup(GridColumn column, Rectangle bounds, Control ownerControl, object creator) {
			DestroyFilterPopup();
			if(!DataController.IsReady || !CanShowPopupObjects) return;
			if(!UpdateCurrentRowAction()) return;
			if(column == null) return;
			SetCursor(Cursors.WaitCursor);
			try {
				ShowFilterPopupCore(column, bounds, ownerControl, creator);
			}
			finally {
				ResetDefaultCursor();
			}
		}
		void ShowFilterPopupCore(GridColumn column, Rectangle bounds, Control ownerControl, object creator) {
			FilterPopup = CreateFilterPopupInstance(column, ownerControl, creator);
			if(filterPopup == null) return;
			object[] values = GetFilterPopupValuesCore(column, true, null);
			this.filterPopup.InitData(values);
			RaiseFilterPopupEvent(this.filterPopup);
			if(!CanShowFilterPopup(this.filterPopup)) {
				filterPopup.Dispose();
				this.filterPopup = null;
				return;
			}
			filterPopup.ShowPopup(bounds);
		}
		protected virtual bool CanShowFilterPopup(FilterPopup filterPopup) {
			ColumnFilterPopup columnPopup = filterPopup as ColumnFilterPopup;
			if(columnPopup != null) return columnPopup.Item.Items.Count > 0;
			CheckedColumnFilterPopup checkedPopup = filterPopup as CheckedColumnFilterPopup;
			if(checkedPopup != null) return checkedPopup.Item.Items.Count > 0;
			return true;
		}
		protected internal virtual void RaiseFilterPopupDate(DateFilterPopup filterPopup, List<FilterDateElement> list) {
			FilterPopupDateEventHandler handler = (FilterPopupDateEventHandler)this.Events[showFilterPopupDate];
			if(handler != null) handler(this, new FilterPopupDateEventArgs(filterPopup.Column, list));
		}
		protected virtual void RaiseFilterPopupEvent(FilterPopup filterPopup) {
			if(filterPopup is CheckedColumnFilterPopup) {
				FilterPopupCheckedListBoxEventHandler handler = (FilterPopupCheckedListBoxEventHandler)this.Events[showFilterPopupCheckedListBox];
				if(handler != null) handler(this, new FilterPopupCheckedListBoxEventArgs(filterPopup.Column, (filterPopup as CheckedColumnFilterPopup).Item));
				return;
			}
			if(filterPopup is ColumnFilterPopup) {
				FilterPopupListBoxEventHandler handler = (FilterPopupListBoxEventHandler)this.Events[showFilterPopupListBox];
				if(handler != null) handler(this, new FilterPopupListBoxEventArgs(filterPopup.Column, (filterPopup as ColumnFilterPopup).Item));
				return;
			}
		}
		protected internal virtual object[] GetFilterPopupValuesCore(GridColumn column, bool allowAsync, OperationCompleted asyncCompleted) {
			bool showAll = true;
			if(column.IsDateFilterPopup) {
				ColumnDateFilterInfo info = GetPrevDateFilterInfo(column);
				if((info != null && info.Cache != null) || column.GetFilterPopupMode() == FilterPopupMode.Date) return null;
			}
			if(column.IsListFilterPopup) {
				showAll = ActiveFilter[column] != null || OptionsFilter.ShowAllTableValuesInFilterPopup || Control.ModifierKeys == Keys.Shift;
			}
			if(column.IsCheckedFilterPopup) {
				showAll = ActiveFilter[column] != null || OptionsFilter.ShowAllTableValuesInCheckedFilterPopup || Control.ModifierKeys == Keys.Shift;
			}
			object[] res = null;
			if(allowAsync) {
				if(asyncCompleted == null) {
					asyncCompleted = delegate(object args) {
						res = args as object[];
						if(column.IsDateFilterPopup && (res == null || res.Length == 0)) {
							if(IsServerMode && DataRowCount > 0) res = null;
						}
						OnFilterPopupValuesReady(column, res);
					};
				}
				res = GetFilterPopupValues(column, showAll, asyncCompleted);
			}
			else {
				res = GetFilterPopupValues(column, showAll, null);
			}
			if(column.IsDateFilterPopup && (res == null || res.Length == 0)) {
				if(IsServerMode && DataRowCount > 0) return null;
			}
			return res;
		}
		protected virtual void OnFilterPopupValuesReady(GridColumn column, object[] values) {
			if(this.filterPopup != null && this.filterPopup.Column == column) this.filterPopup.InitData(values);
		}
		protected virtual FilterPopup CreateFilterPopupInstance(GridColumn column, Control ownerControl, object creator) {
			if(column.IsDateFilterPopup) {
				return CreateDateFilterPopup(column, ownerControl, creator);
			}
			if(column.IsCheckedFilterPopup) return CreateCheckedFilterPopup(column, ownerControl, creator);
			return CreateFilterPopup(column, ownerControl, creator);
		}
		protected internal virtual void OnFilterModeChanged() {
			DestroyFilterData();
			OnActiveFilterEnabledChanged();
		}
		protected virtual void OnActiveFilterEnabledChanged() {
			try {
				BeginSynchronization();
				ApplyColumnsFilterEx();
				if(!ActiveFilterEnabled) RaiseColumnFilterChanged();
			} finally {
				EndSynchronization();
				if(CanSynchronized) OnViewPropertiesChanged(SynchronizationMode.Full);
			}
		}
		protected void ApplyColumnsFilterEx() {
			if(!ActiveFilterEnabled) {
				UpdateLastRowsInfo(true, true);
				UpdateDataControllerFilter(CriteriaOperator.Parse(ExtraFilter));
				CheckSelectFocusedRow();
			}
			else 
				ApplyColumnsFilter();
		}
		protected internal virtual string ExtraFilterText { get { return string.Empty; } }
		protected internal virtual string ExtraFilter { get { return string.Empty; } }
		public virtual void ApplyColumnsFilter() {
			if(IsDeserializing) return;
			this.activeFilterEnabled = true;
			ApplyColumnsFilterCore(true, false);
		}
		protected virtual bool AllowUpdateMRUFilters { get { return true; } }
		protected void UpdateMRU() {
			if(OptionsFilter.AllowMRUFilterList) 
				MRUFilters.InsertMRU(ActiveFilter, OptionsFilter.MRUFilterListCount);
		}
		protected void ApplyColumnsFilterCore() { 
			ApplyColumnsFilterCore(false, !string.IsNullOrEmpty(FindFilterText)); 
		}	  
		protected virtual void ApplyColumnsFilterCore(bool updateMRU, bool ignoreActiveFilter) {
			if(IsLoading || (!ActiveFilterEnabled && !ignoreActiveFilter)) return;
			if(WorkAsLookup && !IsLookupPopupVisible) return; 
			if(!AllowUpdateMRUFilters) updateMRU = false;
			CriteriaOperator criteria = null;
			if(ActiveFilterEnabled) criteria = ActiveFilterCriteria & CriteriaOperator.Parse(ExtraFilter);
			if(object.Equals(ConvertGridFilterToDataFilter(criteria), DataController.FilterCriteria)) {
				LayoutChangedSynchronized();
				return;
			}
			UpdateLastRowsInfo(!DataController.IsUpdateLocked, true);
			ViewFilter filter = ActiveFilter.Clone() as ViewFilter;
			if(ActiveFilterEnabled) RestoreActiveFilter();
			ValidateExpression(criteria);
			BeginSynchronization();
			try {
				BeginUpdate();
				try {
					DestroyFilterData();
					UpdateDataControllerFilter(criteria);
					if(ActiveFilterEnabled) {
						LastValidFilter = filter;
						RestoreActiveFilter();
						if(updateMRU)
							UpdateMRU();
					}
				}
				finally {
					EndUpdate();
				}
			}
			catch {
				throw;
			}
			finally {
				EndSynchronization();
			}
			if(CanSynchronized) OnViewPropertiesChanged(SynchronizationMode.Data);
			RaiseColumnFilterChanged();
			OnApplyColumnsFilterComplete();
		}
		protected virtual void OnApplyColumnsFilterComplete() {
			CheckSelectFocusedRow();
		}
		protected internal override void OnDefaultViewStop() {
			BeginUpdate();
			try {
				DestroyFindPanel();
				HideFindPanel();
				base.OnDefaultViewStop();
			}
			finally {
				CancelUpdate();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsFindPanelVisible { get { return (FindPanelVisible || OptionsFind.AlwaysVisible) && CheckAllowFindPanel(); } }
		public void ShowFindPanel() {
			if(GridControl == null) return;
			if(FindPanelVisible && FindPanel != null && FindPanel.Visible)
				FindPanel.FocusFindEdit();
			FindPanelVisible = true;
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), XtraSerializableProperty(1010), XtraSerializablePropertyId(LayoutIdData)]
		public string FindFilterText { 
			get { return findFilterText; }
			set {
				if(!IsDeserializing) {
					ApplyFindFilter(value);
				}
				else {
					this.findFilterText = value;
				}
			}
		}
		public void HideFindPanel() {
			if(FindPanel != null && FindPanel.ContainsFocus && GridControl != null) {
				GridControl.Focus();
			}
			FindPanelVisible = false;
			if(OptionsFind.ClearFindOnClose) ApplyFindFilter("");
		}
		public void ApplyFindFilter(string filter) {
			if(filter == null) filter = string.Empty;
			if(IsFindPanelVisible && FindPanel != null) {
				FindPanel.SetFilterText(filter);
			}
			ApplyFindFilterCore(filter);
		}
		protected internal virtual void ApplyFindFilterCore(string filter) {
			if(filter == null) filter = string.Empty;
			if(FindFilterText == filter) return;
			findFilterText = filter;
			if(!IsDeserializing) ApplyColumnsFilterCore(false, true);
			OnViewPropertiesChanged(SynchronizationMode.Data);
		}
		internal bool CheckShowFindPanelKey(KeyEventArgs e) {
			if(e.KeyCode == Keys.F && e.Control) {
				if(GridControl.IsAttachedToSearchControl) {
					GridControl.SetFocusSearchControl();
					e.Handled = true;
					return true;
				}
				if(OptionsFind.AllowFindPanel) {
					if(UpdateCurrentRowAction()) {
						ShowFindPanel();
						e.Handled = true;
					}
					return true;
				}
			}
			return false;
		}
		FindControl findPanel;
		protected internal FindControl FindPanel { get { return findPanel; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(1020), XtraSerializablePropertyId(LayoutIdOptionsView)]
		public bool FindPanelVisible {
			get { return findPanelVisible; }
			set {
				if(value) value = CheckAllowFindPanel();
				if(FindPanelVisible == value) return;
				findPanelVisible = value;
				OnFindPanelVisibleChanged();
			}
		}
		bool CheckAllowFindPanel() {
			if(GridControl == null) return false;
			if(GridControl.DefaultView != this) return false;
			if(WorkAsLookup) return false;
			if(GridControl.IsAttachedToSearchControl) return false;
			return true;
		}
		protected internal void RequestFindPanel() {
			if(FindPanel != null) return;
			findPanel = CreateFindPanel(findPanelProperties);
			findPanel.Visible = false;
			GridControl.Controls.Add(findPanel);
			findPanel.SetFilterText(FindFilterText);
		}
		protected internal void DestroyFindPanel() {
			if(findPanel != null) {
				this.findPanelProperties = findPanel.SaveProperties();
				findPanel.Dispose();
			}
			this.findPanel = null;
		}
		object findPanelProperties = null;
		void OnFindPanelVisibleChanged() {
			if(!IsFindPanelVisible) DestroyFindPanel();
			LayoutChangedSynchronized();
		}
		protected internal virtual bool IsAllowHighlightFind(GridColumn column) {
			if(column != null && IsFindFilterActive && OptionsFind.HighlightFindResults && IsAllowFindColumn(column)) return true;
			return false;
		}
		protected virtual FindControl CreateFindPanel(object findPanelProperties) {
			return new FindControl(this, findPanelProperties);
		}
		protected internal bool IsFindFilterActive {
			get { return !string.IsNullOrEmpty(FindFilterText); }
		}
		protected internal bool IsFindFilterFocused {
			get { return FindPanel != null && FindPanel.Visible && FindPanel.ContainsFocus; }
		}
		protected virtual void UpdateDataControllerFilter(CriteriaOperator criteria) {
			DataController.FilterCriteria = ConvertGridFilterToDataFilter(criteria);
		}
		protected virtual List<IDataColumnInfo> GetFindToColumnsCollection() {
			List<IDataColumnInfo> res = new List<IDataColumnInfo>();
			foreach(GridColumn column in VisibleColumns) {
				if(!IsAllowFindColumn(column)) continue;
				res.Add(CreateIDataColumnInfoForFilter(column));
			}
			foreach(GridColumn column in GroupedColumns) {
				if(ContainsIDataColumnInfoForFilter(res, column) || !IsAllowFindColumn(column)) continue;
				res.Add(CreateIDataColumnInfoForFilter(column));
			}
			return res;
		}
		const string formatPlaceholder = @"(?<!\{)\{(?<index>[0-9]+).*?\}(?!})";
		static System.Text.RegularExpressions.Regex formatPlaceholderRegex = new System.Text.RegularExpressions.Regex(formatPlaceholder, System.Text.RegularExpressions.RegexOptions.Compiled);
		protected IEnumerable<GridColumn> GetFormatColumns(string format, int defaultPlaceholdersCount) {
			System.Text.RegularExpressions.MatchCollection matches = null;
			try { matches = formatPlaceholderRegex.Matches(format); }
			catch { yield break; }
			foreach(System.Text.RegularExpressions.Match m in matches) {
				int index;
				if(int.TryParse(m.Groups["index"].Value, out index)) {
					index -= defaultPlaceholdersCount;
					if(index >= 0 && index < Columns.Count)
						yield return Columns[index];
				}
			}
		}
		protected virtual bool ContainsIDataColumnInfoForFilter(List<IDataColumnInfo> res, GridColumn column) {
			return GridColumnIDataColumnInfoWrapper.Contains(res, column);
		}
		protected virtual IDataColumnInfo CreateIDataColumnInfoForFilter(GridColumn column) {
			return new GridColumnIDataColumnInfoWrapper(column, GridColumnIDataColumnInfoWrapperEnum.Filter);
		}
		string[] GetFindToColumnNames() {
			List<IDataColumnInfo> columns = GetFindToColumnsCollection();
			List<string> res = new List<string>();
			for(int n = 0; n < columns.Count; n++) {
				res.Add(columns[n].FieldName);
			}
			return res.Count == 0 ? null : res.ToArray();
		}
		internal bool IsAllowFindColumn(GridColumn col) {
			if(col == null || string.IsNullOrEmpty(col.FieldName)) return false;
			if(col.RealColumnEdit != null && !col.RealColumnEdit.AllowInplaceAutoFilter) return false;
			if(!CanColumnDoServerAction(col, ColumnServerActionType.Filter)) return false;
			string findFilterColumns = OptionsFind.FindFilterColumns;
			if(UseSearchInfo && !string.IsNullOrEmpty(searchInfo.Columns))
				findFilterColumns = searchInfo.Columns;
			if(findFilterColumns == "*") {
				if(!DataController.CanFindColumn(col.ColumnInfo)) return false;
				return true;
			}
			return string.Concat(";", findFilterColumns, ";").Contains(string.Concat(";", col.FieldName, ";"));
		}
		FindSearchParserResults lastParserResults = null;
		internal string GetFindMatchedText(GridColumn column, string displayText) {
			return GetFindMatchedText(column.FieldName, displayText);
		}
		internal string GetFindMatchedText(string fieldName, string displayText) {
			if(lastParserResults == null) return string.Empty;
			return lastParserResults.GetMatchedText(fieldName, displayText);
		}
		protected virtual CriteriaOperator ConvertGridFilterToDataFilter(CriteriaOperator criteria) {
			this.lastParserResults = null;		 
			if(!string.IsNullOrEmpty(FindFilterText)) {
				this.lastParserResults = new FindSearchParser().Parse(FindFilterText, GetFindToColumnsCollection());
				if(!IsServerMode) {
					lastParserResults.AppendColumnFieldPrefixes();
				}
				CriteriaOperator findCriteria = DxFtsContainsHelperAlt.Create(lastParserResults, FilterCondition.Contains, IsServerMode);
				return criteria & findCriteria;
			}
			return criteria;
		}
		protected internal override void SetDataSource(BindingContext context, object dataSource, string dataMember) {
			if(dataSource == null) this.sortData = null; 
			base.SetDataSource(context, dataSource, dataMember);
			UpdateLastRowsInfo(true, true);
		}
		void UpdateLastRowsInfo(bool updateVisibleCount, bool updateSelectedCount) {
			if(updateVisibleCount)
				this.lastDataRowsVisibleCount = DataController.IsReady ? DataRowCount : -1;
			if(updateSelectedCount)
				this.lastSelectedCount = DataController.IsReady ? SelectedRowsCount : 0;
		}
		protected virtual void CheckSelectFocusedRow() {
			if(!IsMultiSelect || !IsValidRowHandle(FocusedRowHandle)) return;
			if(SelectedRowsCount == 0 && (this.lastSelectedCount > 0 || this.lastDataRowsVisibleCount == 0)) {
				SelectRow(FocusedRowHandle);
			}
		}
		protected internal virtual bool IsAllowMRUFilterList {
			get { return OptionsFilter.AllowMRUFilterList && MRUFilters.CanShowMRU(ActiveFilter); }
		}
		public virtual void ClearColumnsFilter() {
			if(!UpdateCurrentRowAction()) return;
			ActiveFilter.Clear();
		}
		protected internal virtual bool HasCustomColumnGroupEvent { get { return false; } }
		protected internal virtual void RaiseCustomColumnGroup(CustomColumnSortEventArgs e) { }
		protected internal bool HasCustomColumnSortEvent { get { return this.Events[customColumnSort] != null; } }
		protected internal virtual void RaiseCustomColumnSort(CustomColumnSortEventArgs e) {
			CustomColumnSortEventHandler handler = (CustomColumnSortEventHandler)this.Events[customColumnSort];
			if(handler != null) handler(this, e);
		}
		CustomColumnDisplayTextEventArgs customDisplayTextArgs = null;
		internal string RaiseCustomColumnDisplayText(int rowHandle, GridColumn column, object _value, string displayText, bool forGroupRow) {
			return RaiseCustomColumnDisplayText(InvalidRowHandle,  rowHandle, column, _value, displayText, forGroupRow);
		}
		protected internal string RaiseCustomColumnDisplayText(int listSourceIndex, int rowHandle, GridColumn column, object _value, string displayText, bool forGroupRow) {
			if(this.customDisplayTextArgs == null) customDisplayTextArgs = new CustomColumnDisplayTextEventArgs(0, null, null, forGroupRow);
			this.customDisplayTextArgs.SetArgs(listSourceIndex, rowHandle, column, _value, displayText, forGroupRow);
			RaiseCustomColumnDisplayText(this.customDisplayTextArgs);
			return this.customDisplayTextArgs.DisplayText;
		}
		protected internal virtual void RaiseCustomColumnDisplayText(CustomColumnDisplayTextEventArgs e) {
			CustomColumnDisplayTextEventHandler handler = (CustomColumnDisplayTextEventHandler)this.Events[customColumnDisplayText];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseCustomUnboundColumnData(CustomColumnDataEventArgs e) {
			CustomColumnDataEventHandler handler = (CustomColumnDataEventHandler)this.Events[customUnboundColumnData];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseCustomDrawFilterPanel(CustomDrawObjectEventArgs e) {
			CustomDrawObjectEventHandler handler = (CustomDrawObjectEventHandler)this.Events[customDrawFilterPanel];
			if(handler != null) handler(this, e);
		}
		protected virtual void RaiseSelectionChanged(SelectionChangedEventArgs e) {
			SelectionChangedEventHandler handler = (SelectionChangedEventHandler)this.Events[selectionChanged];
			if(handler != null) handler(this, e);
		}
		protected virtual void RaiseInitNewRow(InitNewRowEventArgs e) {
			InitNewRowEventHandler handler = (InitNewRowEventHandler)this.Events[initNewRow];
			if(handler != null) handler(this, e);
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewSelectionChanged"),
#endif
 DXCategory(CategoryName.PropertyChanged)]
		public event SelectionChangedEventHandler SelectionChanged {
			add { this.Events.AddHandler(selectionChanged, value); }
			remove { this.Events.RemoveHandler(selectionChanged, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewCustomDrawEmptyForeground"),
#endif
 DXCategory(CategoryName.CustomDraw)]
		public event CustomDrawEventHandler CustomDrawEmptyForeground {
			add { this.Events.AddHandler(customDrawEmptyForeground, value); }
			remove { this.Events.RemoveHandler(customDrawEmptyForeground, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewShowingEditor"),
#endif
 DXCategory(CategoryName.Editor)]
		public event CancelEventHandler ShowingEditor {
			add { this.Events.AddHandler(showingEditor, value); }
			remove { this.Events.RemoveHandler(showingEditor, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewInitNewRow"),
#endif
 DXCategory(CategoryName.Data)]
		public event InitNewRowEventHandler InitNewRow {
			add { this.Events.AddHandler(initNewRow, value); }
			remove { this.Events.RemoveHandler(initNewRow, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewHiddenEditor"),
#endif
 DXCategory(CategoryName.Editor)]
		public event EventHandler HiddenEditor {
			add { this.Events.AddHandler(hiddenEditor, value); }
			remove { this.Events.RemoveHandler(hiddenEditor, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewShownEditor"),
#endif
 DXCategory(CategoryName.Editor)]
		public event EventHandler ShownEditor {
			add { this.Events.AddHandler(shownEditor, value); }
			remove { this.Events.RemoveHandler(shownEditor, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewStartSorting"),
#endif
 DXCategory(CategoryName.Sorting)]
		public event EventHandler StartSorting {
			add { this.Events.AddHandler(startSorting, value); }
			remove { this.Events.RemoveHandler(startSorting, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewEndSorting"),
#endif
 DXCategory(CategoryName.Sorting)]
		public event EventHandler EndSorting {
			add { this.Events.AddHandler(endSorting, value); }
			remove { this.Events.RemoveHandler(endSorting, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewStartGrouping"),
#endif
 DXCategory(CategoryName.Sorting)]
		public event EventHandler StartGrouping {
			add { this.Events.AddHandler(startGrouping, value); }
			remove { this.Events.RemoveHandler(startGrouping, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewEndGrouping"),
#endif
 DXCategory(CategoryName.Sorting)]
		public event EventHandler EndGrouping {
			add { this.Events.AddHandler(endGrouping, value); }
			remove { this.Events.RemoveHandler(endGrouping, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewColumnChanged"),
#endif
 DXCategory(CategoryName.PropertyChanged)]
		public event EventHandler ColumnChanged {
			add { this.Events.AddHandler(columnChanged, value); }
			remove { this.Events.RemoveHandler(columnChanged, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewColumnUnboundExpressionChanged"),
#endif
 DXCategory(CategoryName.PropertyChanged)]
		public event ColumnEventHandler ColumnUnboundExpressionChanged {
			add { this.Events.AddHandler(columnUnboundExpressionChanged, value); }
			remove { this.Events.RemoveHandler(columnUnboundExpressionChanged, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewColumnPositionChanged"),
#endif
 DXCategory(CategoryName.PropertyChanged)]
		public event EventHandler ColumnPositionChanged {
			add { this.Events.AddHandler(columnPositionChanged, value); }
			remove { this.Events.RemoveHandler(columnPositionChanged, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewFocusedRowChanged"),
#endif
 DXCategory(CategoryName.PropertyChanged)]
		public event FocusedRowChangedEventHandler FocusedRowChanged {
			add { this.Events.AddHandler(focusedRowChanged, value); }
			remove { this.Events.RemoveHandler(focusedRowChanged, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewFocusedRowObjectChanged"),
#endif
 DXCategory(CategoryName.PropertyChanged)]
		public event FocusedRowObjectChangedEventHandler FocusedRowObjectChanged {
			add { this.Events.AddHandler(focusedRowObjectChanged, value); }
			remove { this.Events.RemoveHandler(focusedRowObjectChanged, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewFocusedColumnChanged"),
#endif
 DXCategory(CategoryName.PropertyChanged)]
		public event FocusedColumnChangedEventHandler FocusedColumnChanged {
			add { this.Events.AddHandler(focusedColumnChanged, value); }
			remove { this.Events.RemoveHandler(focusedColumnChanged, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewCellValueChanged"),
#endif
 DXCategory(CategoryName.PropertyChanged)]
		public event CellValueChangedEventHandler CellValueChanged {
			add { this.Events.AddHandler(cellValueChanged, value); }
			remove { this.Events.RemoveHandler(cellValueChanged, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewCellValueChanging"),
#endif
 DXCategory(CategoryName.PropertyChanged)]
		public event CellValueChangedEventHandler CellValueChanging {
			add { this.Events.AddHandler(cellValueChanging, value); }
			remove { this.Events.RemoveHandler(cellValueChanging, value); }
		}
		[DXCategory(CategoryName.Data), 
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewDataManagerReset")
#else
	Description("")
#endif
]
		public event EventHandler DataManagerReset {
			add { this.Events.AddHandler(dataManagerReset, value); }
			remove { this.Events.RemoveHandler(dataManagerReset, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewRowCellDefaultAlignment"),
#endif
 DXCategory(CategoryName.Appearance)]
		public event RowCellAlignmentEventHandler RowCellDefaultAlignment {
			add { this.Events.AddHandler(rowCellDefaultAlignment, value); }
			remove { this.Events.RemoveHandler(rowCellDefaultAlignment, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewInvalidRowException"),
#endif
 DXCategory(CategoryName.Action)]
		public event InvalidRowExceptionEventHandler InvalidRowException {
			add { this.Events.AddHandler(invalidRowException, value); }
			remove { this.Events.RemoveHandler(invalidRowException, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewBeforeLeaveRow"),
#endif
 DXCategory(CategoryName.Action)]
		public event RowAllowEventHandler BeforeLeaveRow {
			add { this.Events.AddHandler(beforeLeaveRow, value); }
			remove { this.Events.RemoveHandler(beforeLeaveRow, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewRowDeleting"),
#endif
 DXCategory(CategoryName.Action)]
		public event RowDeletingEventHandler RowDeleting {
			add { this.Events.AddHandler(rowDeleting, value); }
			remove { this.Events.RemoveHandler(rowDeleting, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewRowDeleted"),
#endif
 DXCategory(CategoryName.Action)]
		public event RowDeletedEventHandler RowDeleted {
			add { this.Events.AddHandler(rowDeleted, value); }
			remove { this.Events.RemoveHandler(rowDeleted, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewValidateRow"),
#endif
 DXCategory(CategoryName.Action)]
		public event ValidateRowEventHandler ValidateRow {
			add { this.Events.AddHandler(validateRow, value); }
			remove { this.Events.RemoveHandler(validateRow, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewRowUpdated"),
#endif
 DXCategory(CategoryName.Action)]
		public event RowObjectEventHandler RowUpdated {
			add { this.Events.AddHandler(rowUpdated, value); }
			remove { this.Events.RemoveHandler(rowUpdated, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewColumnFilterChanged"),
#endif
 DXCategory(CategoryName.PropertyChanged)]
		public event EventHandler ColumnFilterChanged {
			add { this.Events.AddHandler(columnFilterChanged, value); }
			remove { this.Events.RemoveHandler(columnFilterChanged, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewShowFilterPopupListBox"),
#endif
 DXCategory(CategoryName.Behavior)]
		public event FilterPopupListBoxEventHandler ShowFilterPopupListBox {
			add { this.Events.AddHandler(showFilterPopupListBox, value); }
			remove { this.Events.RemoveHandler(showFilterPopupListBox, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewShowFilterPopupDate"),
#endif
 DXCategory(CategoryName.Behavior)]
		public event FilterPopupDateEventHandler ShowFilterPopupDate {
			add { this.Events.AddHandler(showFilterPopupDate, value); }
			remove { this.Events.RemoveHandler(showFilterPopupDate, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewShowFilterPopupCheckedListBox"),
#endif
 DXCategory(CategoryName.Behavior)]
		public event FilterPopupCheckedListBoxEventHandler ShowFilterPopupCheckedListBox {
			add { this.Events.AddHandler(showFilterPopupCheckedListBox, value); }
			remove { this.Events.RemoveHandler(showFilterPopupCheckedListBox, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewCustomFilterDialog"),
#endif
 DXCategory(CategoryName.Behavior)]
		public event CustomFilterDialogEventHandler CustomFilterDialog {
			add { this.Events.AddHandler(customFilterDialog, value); }
			remove { this.Events.RemoveHandler(customFilterDialog, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewCustomFilterDisplayText"),
#endif
 DXCategory(CategoryName.Behavior)]
		public event ConvertEditValueEventHandler CustomFilterDisplayText {
			add { this.Events.AddHandler(customFilterDisplayText, value); }
			remove { this.Events.RemoveHandler(customFilterDisplayText, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewFilterEditorCreated"),
#endif
 DXCategory(CategoryName.Behavior)]
		public event FilterControlEventHandler FilterEditorCreated {
			add { this.Events.AddHandler(filterEditorCreated, value); }
			remove { this.Events.RemoveHandler(filterEditorCreated, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewUnboundExpressionEditorCreated"),
#endif
 DXCategory(CategoryName.Behavior)]
		public event UnboundExpressionEditorEventHandler UnboundExpressionEditorCreated {
			add { this.Events.AddHandler(unboundExpressionEditorCreated, value); }
			remove { this.Events.RemoveHandler(unboundExpressionEditorCreated, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewCustomDrawFilterPanel"),
#endif
 DXCategory(CategoryName.CustomDraw)]
		public event CustomDrawObjectEventHandler CustomDrawFilterPanel {
			add { this.Events.AddHandler(customDrawFilterPanel, value); }
			remove { this.Events.RemoveHandler(customDrawFilterPanel, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewCustomColumnSort"),
#endif
 DXCategory(CategoryName.Sorting)]
		public event CustomColumnSortEventHandler CustomColumnSort {
			add { this.Events.AddHandler(customColumnSort, value); }
			remove { this.Events.RemoveHandler(customColumnSort, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewCustomUnboundColumnData"),
#endif
 DXCategory(CategoryName.Data)]
		public event CustomColumnDataEventHandler CustomUnboundColumnData {
			add { this.Events.AddHandler(customUnboundColumnData, value); }
			remove { this.Events.RemoveHandler(customUnboundColumnData, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewCustomRowFilter"),
#endif
 DXCategory(CategoryName.Data)]
		public event RowFilterEventHandler CustomRowFilter {
			add { 
				this.Events.AddHandler(customRowFilter, value);
				if(!IsLoading) RefreshData();
			}
			remove {
				this.Events.RemoveHandler(customRowFilter, value);
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewSubstituteFilter"),
#endif
 DXCategory(CategoryName.Data)]
		public event EventHandler<SubstituteFilterEventArgs> SubstituteFilter {
			add {
				this.Events.AddHandler(substituteFilter, value);
				if(!IsLoading) RefreshData();
			}
			remove {
				this.Events.RemoveHandler(substituteFilter, value);
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewSubstituteSortInfo"),
#endif
 DXCategory(CategoryName.Data)]
		public event EventHandler<SubstituteSortInfoEventArgs> SubstituteSortInfo {
			add {
				this.Events.AddHandler(substituteSortInfo, value);
				if(!IsLoading) RefreshData();
			}
			remove {
				this.Events.RemoveHandler(substituteSortInfo, value);
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewRowLoaded"),
#endif
 DXCategory(CategoryName.DataAsync)]
		public event RowEventHandler RowLoaded {
			add { this.Events.AddHandler(rowLoaded, value); }
			remove { this.Events.RemoveHandler(rowLoaded, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewRowLoaded"),
#endif
 DXCategory(CategoryName.DataAsync)]
		public event RowEventHandler FocusedRowLoaded {
			add { this.Events.AddHandler(focusedRowLoaded, value); }
			remove { this.Events.RemoveHandler(focusedRowLoaded, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewAsyncCompleted"),
#endif
 DXCategory(CategoryName.DataAsync)]
		public event EventHandler AsyncCompleted {
			add { this.Events.AddHandler(asyncCompleted, value); }
			remove { this.Events.RemoveHandler(asyncCompleted, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnViewCustomColumnDisplayText"),
#endif
 DXCategory(CategoryName.Data)]
		public event CustomColumnDisplayTextEventHandler CustomColumnDisplayText {
			add { this.Events.AddHandler(customColumnDisplayText, value); }
			remove { this.Events.RemoveHandler(customColumnDisplayText, value); }
		}
		public void WaitForAsyncOperationEnd() {
			if(DataController == null) return;
			DataController.WaitForAsyncEnd();
		}
		#region IDataControllerThreadClient Members
		void IDataControllerThreadClient.OnTotalsReceived() {
			OnAsyncTotalsReceived();
		}
		void IDataControllerThreadClient.OnRowLoaded(int controllerRowHandle) {
			RaiseRowLoaded(controllerRowHandle);
			UpdateRowAutoHeight(controllerRowHandle);
		}
		void IDataControllerThreadClient.OnAsyncBegin() {
			OnAsyncOperationBegin();
		}
		void IDataControllerThreadClient.OnAsyncEnd() {
			OnAsyncOperationEnd();
		}
		#endregion
		#region IDataControllerCurrentSupport Members
		void IDataControllerCurrentSupport.OnCurrentControllerRowChanged(CurrentRowEventArgs e) {
			OnCurrentControllerRowChanged(e);
		}
		void IDataControllerCurrentSupport.OnCurrentControllerRowObjectChanged(CurrentRowChangedEventArgs e) {
			OnCurrentControllerRowObjectChanged(e);
		}
		protected virtual void OnCurrentControllerRowChanged(CurrentRowEventArgs e) { }
		protected virtual void OnCurrentControllerRowObjectChanged(CurrentRowChangedEventArgs e) {
			RaiseFocusedRowObjectChanged(new FocusedRowObjectChangedEventArgs(focusedRowHandle, e.CurrentRow));
		}
		#endregion
		#region IDataControllerVisualClient
		void IDataControllerVisualClient2.TotalSummaryCalculated() {
			FormatRules.ResetValuesReady();
		}
		void IDataControllerVisualClient.RequireSynchronization(IDataSync dataSync) {
			List<GridColumnSortInfo> sort = new List<GridColumnSortInfo>();
			foreach(ListSortInfo info in dataSync.Sort ) {
				GridColumn col = Columns.ColumnByFieldName(info.PropertyName);
				if(col != null) sort.Add(new GridColumnSortInfo(col, info.SortDirection == ListSortDirection.Ascending ? ColumnSortOrder.Ascending : ColumnSortOrder.Descending));
			}
			BeginDataUpdate();
			try {
				SortInfo.ClearAndAddRange(sort.ToArray(), dataSync.GroupCount);
			}
			finally {
				EndDataUpdate();
			}
		}
		void IDataControllerVisualClient.ColumnsRenewed() {
			if(Columns.Count == 0 && !IsLoading) PopulateColumns();
		}
		void IDataControllerVisualClient.UpdateColumns() {
			UpdateColumnHandles();
		}
		bool IDataControllerVisualClient.IsInitializing { 
			get {
				return IsDataInitializing;
			}
		}
		void IDataControllerVisualClient.RequestSynchronization() {
			BeginUpdate();
			try {
				SynchronizeDataController();
			} finally {
				CancelUpdate();
			}
		}
		int IDataControllerVisualClient.VisibleRowCount { get { return VisualClientVisibleRowCount; } }
		int IDataControllerVisualClient.TopRowIndex { get { return VisualClientTopRowIndex; } }
		int IDataControllerVisualClient.PageRowCount { get { return VisualClientPageRowCount; } }
		void IDataControllerVisualClient.UpdateLayout() { 
			VisualClientUpdateLayout();
		}
		void IDataControllerVisualClient.UpdateRowIndexes(int newTopRowIndex) {
			ClearPrevSelectionInfo();
			VisualClientUpdateRowIndexes(newTopRowIndex);
		}
		void IDataControllerVisualClient.UpdateRows(int topRowIndexDelta) { 
			ClearPrevSelectionInfo();
			VisualClientUpdateRows(topRowIndexDelta);
		}
		void IDataControllerVisualClient.UpdateRow(int controllerRowHandle) {
			VisualClientUpdateRow(controllerRowHandle);
		}
		void IDataControllerVisualClient.UpdateScrollBar() {
			VisualClientUpdateScrollBar();
		}
		void IDataControllerVisualClient.UpdateTotalSummary() {
			VisualClientUpdateTotalSummary();
		}
		protected virtual void VisualClientUpdateRow(int controllerRowHandle) {
			RefreshRow(controllerRowHandle, false);
		}
		protected virtual void VisualClientUpdateRowIndexes(int newTopRowIndex) {
			SetLayoutDirty();
		}
		protected virtual void VisualClientUpdateLayout() {
			SetLayoutDirty();
			CheckDataControllerError();
		}
		string lastError = string.Empty;
		protected virtual void CheckDataControllerError() {
			ShowDataControllerError();
		}
		protected void ShowDataControllerError() {
			if(GridControl == null || !GridControl.IsHandleCreated) return;
			if(lastError != DataController.LastErrorText) {
				this.lastError = DataController.LastErrorText;
				if(!string.IsNullOrEmpty(lastError)) OnDataControllerError(lastError);
			}
			if(DataController.LastErrorText == "") {
				if(GridControl.EditorHelper.RealToolTipController.ActiveObject == null)
					HideHint();
				return;
			}
			ToolTipControllerShowEventArgs ee = new ToolTipControllerShowEventArgs();
			ee.AutoHide = false;
			ee.Title = "Error";
			ee.ToolTipLocation = ToolTipLocation.RightTop;
			ee.ToolTip = string.Format(GridLocalizer.Active.GetLocalizedString(GridStringId.ServerRequestError), 
				DataController.LastErrorText.Substring(0, Math.Min(50, DataController.LastErrorText.Length)));
			ee.ToolTipType = ToolTipType.SuperTip;
			ee.IconType = ToolTipIconType.Error;
			ee.IconSize = ToolTipIconSize.Small;
			ToolTipController.DefaultController.ShowHint(ee, GridControl.PointToScreen(new Point(ViewRect.Left, ViewRect.Bottom)));
		}
		protected virtual void OnDataControllerError(string lastError) {
		}
		protected virtual void VisualClientUpdateRows(int topRowIndexDelta) {
			LayoutChangedSynchronized(); 
		}
		protected virtual void VisualClientUpdateTotalSummary() { }
		protected abstract void VisualClientUpdateScrollBar();
		protected abstract int VisualClientTopRowIndex { get ; }
		protected virtual int VisualClientVisibleRowCount { get { return RowCount; } }
		protected abstract int VisualClientPageRowCount { get ; }
		#endregion IDataControllerVisualClient
		#region IDataControllerData
		bool? IDataControllerData2.IsRowFit(int listSourceRow, bool fit) {
			var mangled = RaiseCustomRowFilter(listSourceRow, fit);
			if(mangled == -1)
				return null;
			else
				return mangled != 0;
		}
		ComplexColumnInfoCollection IDataControllerData2.GetComplexColumns() {
			BaseGridController controller = WorkAsLookup ? LookUpOwner.Controller : DataController;
			ComplexColumnInfoCollection res = new ComplexColumnInfoCollection();
			foreach(GridColumn column in Columns) {
				if(column.UnboundType != UnboundColumnType.Bound) continue;
				if(column.FieldName.Contains(".") && controller.FindColumn(column.FieldName) == null) res.Add(column.FieldName);
				if(column.FieldNameSortGroup.Contains(".") && controller.FindColumn(column.FieldNameSortGroup) == null) res.Add(column.FieldNameSortGroup);
			}
			if(WorkAsLookup) {
				string[] fields = new string[] { LookUpOwner.DisplayMember, LookUpOwner.ValueMember };
				foreach(string column in fields) {
					if(column.Contains(".") && controller.Columns[column] == null && res.IndexOf(column) == -1) res.Add(column);
				}
			}
			return res;
		}
		protected virtual void RaiseSubstituteFilter(SubstituteFilterEventArgs args) {
			var handler = (EventHandler<SubstituteFilterEventArgs>)Events[substituteFilter];
			if(handler == null)
				return;
			handler(this, args);
		}
		void IDataControllerData2.SubstituteFilter(SubstituteFilterEventArgs args) {
			RaiseSubstituteFilter(args);
		}
		bool IDataControllerData2.HasUserFilter { get { return HasCustomRowFilter; } }
		bool IDataControllerData2.CanUseFastProperties { get { return !DesignMode; } }
		UnboundColumnInfoCollection IDataControllerData.GetUnboundColumns() {
			UnboundColumnInfoCollection res = new UnboundColumnInfoCollection();
			for(int n = 0; n < Columns.Count; n++) {
				GridColumn column = Columns[n];
				if(column.UnboundType != UnboundColumnType.Bound && column.FieldName != string.Empty) {
					res.Add(new UnboundColumnInfo(column.FieldName, column.UnboundType, false, column.UnboundExpression));
				}
			}
			PopulateCustomUnboundColumns(res);
			if(res.Count > 0) return res;
			return null;
		}
		protected virtual void PopulateCustomUnboundColumns(UnboundColumnInfoCollection unboundCollection) { }
		object IDataControllerData.GetUnboundData(int listSourceRow1, DataColumnInfo column, object value) {
			return GetUnboundDataCore(listSourceRow1, column, value);
		}
		protected virtual object GetUnboundDataCore(int listSourceRow1, DataColumnInfo column, object value) {
			CustomColumnDataEventArgs customData = new CustomColumnDataEventArgs(Columns.ColumnByDataColumn(column), listSourceRow1, value, true);
			RaiseCustomUnboundColumnData(customData);
			return customData.Value;
		}
		void IDataControllerData.SetUnboundData(int listSourceRow1, DataColumnInfo column, object value) {
			SetUnboundDataCore(listSourceRow1, column, value);
		}
		protected virtual void SetUnboundDataCore(int listSourceRow1, DataColumnInfo column, object value) {
			CustomColumnDataEventArgs customData = new CustomColumnDataEventArgs(Columns.ColumnByDataColumn(column), listSourceRow1, value, false);
			RaiseCustomUnboundColumnData(customData);
		}
		PropertyDescriptorCollection IDataControllerData2.PatchPropertyDescriptorCollection(PropertyDescriptorCollection collection) {
			if(Columns == null || collection == null)
				return collection;
			foreach(GridColumn col in  Columns) {
				if(col.UnboundType == UnboundColumnType.Bound && collection.Find(col.FieldName, false) == null)
					collection.Find(col.FieldName, true);
				if(!string.IsNullOrEmpty(col.FieldNameSortGroup) && col.UnboundType == UnboundColumnType.Bound && collection.Find(col.FieldNameSortGroup, false) == null)
					collection.Find(col.FieldNameSortGroup, true);
			}
			return collection;
		}
		#endregion
		#region IDataControllerSort
		GridFilterData filterData = null, findFilterData = null;
		protected GridFilterData FilterData {
			get { 
				if(filterData == null) {
					filterData = new GridFilterData(this);
					filterData.OnStart();
				}
				return filterData;
			}
		}
		protected GridFilterData FindFilterData {
			get {
				if(findFilterData == null) {
					if(!IsFindFilterActive) return null;
					findFilterData = new GridFindFilterData(this);
					findFilterData.OnStart();
				}
				return findFilterData;
			}
		}
		protected void DestroyFilterData() {
			if(this.filterData != null) {
				this.filterData.Dispose();
				this.filterData = null;
			}
			if(this.findFilterData != null) {
				this.findFilterData.Dispose();
				this.findFilterData = null;
			}
		}
		GridSortData sortData = null;
		protected GridSortData SortData {
			get { 
				if(sortData == null) sortData = new GridSortData(this);
				return sortData;
			}
		}
		string[] IDataControllerSort.GetFindByPropertyNames() { 
			if(IsServerMode || !IsFindFilterActive) return new string[0];
			return GetFindToColumnNames();
		}
		string IDataControllerSort.GetDisplayText(int listSourceIndex, DataColumnInfo column, object value, string columnName) {
			GridDataColumnInfo info = FilterData.GetInfo(column) as GridDataColumnInfo;
			if(info != null && info.Column.FieldName == columnName) return info.GetDisplayText(listSourceIndex, value);
			if(FindFilterData != null) {
				info = FindFilterData.GetInfo(column) as GridDataColumnInfo;
				if(info != null && columnName.StartsWith(DxFtsContainsHelper.DxFtsPropertyPrefix)) return info.GetDisplayText(listSourceIndex, value);
			}
			return value == null || value == DBNull.Value ? string.Empty : value.ToString();
		}
		bool IDataControllerSort.RequireDisplayText(DataColumnInfo column) { 
			return FilterData.IsRequired(column);
		}
		ExpressiveSortInfo.Row IDataControllerSort.GetCompareRowsMethodInfo() {
			return null;
		}
		bool? IDataControllerSort.IsEqualGroupValues(int listSourceRow1, int listSourceRow2, object value1, object value2, DataColumnInfo column) {
			if(!SortData.IsRequired(column))
				return null;
			GridDataColumnSortInfo info = SortData.GetSortInfo(column);
			if(info == null)
				return null;
			var cmp = info.CompareGroupValues(listSourceRow1, listSourceRow2, value1, value2);
			if(cmp.HasValue)
				return cmp.Value == 0;
			return null;
		}
		ExpressiveSortInfo.Cell IDataControllerSort.GetSortGroupCellMethodInfo(DataColumnInfo dataColumnInfo, Type baseExtractorType) {
			if(!SortData.IsRequired(dataColumnInfo))
				return null;
			GridDataColumnSortInfo info = SortData.GetSortInfo(dataColumnInfo);
			if(info == null)
				return null;
			return info.GetCompareGroupValuesInfo(baseExtractorType);
		}
		ExpressiveSortInfo.Cell IDataControllerSort.GetSortCellMethodInfo(DataColumnInfo dataColumnInfo, Type baseExtractorType, ColumnSortOrder order) {
			if(!SortData.IsRequired(dataColumnInfo))
				return null;
			GridDataColumnSortInfo info = SortData.GetSortInfo(dataColumnInfo);
			if(info == null)
				return null;
			return info.GetCompareSortValuesInfo(baseExtractorType, order);
		}
		void IDataControllerSort.BeforeGrouping() { OnBeforeGrouping(); }
		void IDataControllerSort.AfterGrouping() { OnAfterGrouping(); }
		void IDataControllerSort.BeforeSorting() { OnBeforeSorting(); }
		void IDataControllerSort.AfterSorting() { OnAfterSorting(); }
		protected virtual void OnBeforeGrouping() { 
			RaiseStartGrouping();
		}
		protected virtual void OnAfterGrouping() { 
			RaiseEndGrouping();
		}
		protected virtual void OnBeforeSorting() {
			SortData.OnStart();
			RaiseStartSorting();
		}
		protected virtual void OnAfterSorting() {
			RaiseEndSorting();
		}
		protected virtual void RaiseSubstituteSortInfo(SubstituteSortInfoEventArgs args) {
			var handler = (EventHandler<SubstituteSortInfoEventArgs>)Events[substituteSortInfo];
			if(handler == null)
				return;
			handler(this, args);
		}
		void IDataControllerSort.SubstituteSortInfo(SubstituteSortInfoEventArgs args) {
			RaiseSubstituteSortInfo(args);
		}
		#endregion IDataControllerSort
		LoadingAnimationInfo notificationInfo;
		internal LoadingAnimationInfo NotificationInfo {
			get {
				if(notificationInfo == null) notificationInfo = new LoadingAnimationInfo();
				return notificationInfo;
			}
		}
		internal bool GetIsNotificationInfoVisible() {
			return (notificationInfo != null) && NotificationInfo.Visible;
		}
		protected virtual bool ForceLoadingPanel { get { return false; } }
		protected internal virtual bool CheckAllowNotifications() {
			if(FilterPopup != null) {
				NotificationInfo.HideImmediate();
			} else {
				NotificationInfo.CheckVisible(IsAsyncInProgress || ForceLoadingPanel);
			}
			return NotificationInfo.IsShouldShow;
		}
		[Browsable(false)]
		public virtual bool IsAsyncInProgress { 
			get {
				AsyncServerModeDataController asyncController = DataController as AsyncServerModeDataController;
				if(asyncController != null) return asyncController.IsBusy;
				return false;
			} 
		}
		protected virtual void OnAsyncTotalsReceived() {
			if(WorkAsLookup) return;
			if(!IsValidRowHandle(FocusedRowHandle)) FocusedRowHandle = GetVisibleRowHandle(0);
		}
		protected virtual void OnAsyncOperationBegin() {
			if(CheckAllowNotifications()) {
				Invalidate();
				return;
			}
			DelayedInvalidate(NotificationInfo.WaitAnimationShowDelay + 10);
		}
		internal void DelayedInvalidate(int delayms) {
			Timer timer = new Timer();
			timer.Interval = delayms;
			timer.Start();
			timer.Tick += delegate(object sender, EventArgs e) {
				Invalidate();
				((Timer)sender).Dispose();
			};
		}
		protected virtual void OnAsyncOperationEnd() {
			if(!CheckAllowNotifications()) {
				Invalidate();
			}
			else {
				DelayedInvalidate(NotificationInfo.WaitAnimationHideDelay + 10);
			}
			RaiseAsyncCompleted();
		}
		protected virtual internal int AccessibleIndex2RowHandle(int index) {
			return GetVisibleRowHandle(index);
		}
		protected virtual internal int RowHandle2AccessibleIndex(int rowHandle) {
			int vindex = GetVisibleIndex(rowHandle);
			if(vindex > -1) return VisibleIndex2AccessibleIndex(vindex);
			return -1;
		}
		protected virtual internal int VisibleIndex2AccessibleIndex(int visibleIndex) {
			return visibleIndex;
		}
		protected internal GridColumn LookUpDisplayColumn { 
			get { 
				return LookUpOwner == null ? null : Columns[LookUpOwner.DisplayMember];
			}
		}
		protected internal virtual FilterColumnCollection CreateFilterColumnCollection() {
			return new ViewFilterColumnCollection(this);
		}
		protected virtual void OnCustomFilterDisplayText(ConvertEditValueEventArgs e) {
			ConvertEditValueEventHandler handler = (ConvertEditValueEventHandler)this.Events[customFilterDisplayText];
			if(handler != null)
				handler(this, e);
		}
		public string GetFilterDisplayText(CriteriaOperator filter) {
			object result;
			ConvertEditValueEventArgs e = new ConvertEditValueEventArgs(filter);
			OnCustomFilterDisplayText(e);
			if(e.Handled) {
				result = e.Value;
			} else {
				using(FilterColumnCollection fcc = CreateFilterColumnCollection()) {
					result = DisplayCriteriaGeneratorPathed.Process(fcc, filter);
				}
			}
			if(result == null) {
				return string.Empty;
			} else {
				CriteriaOperator criteriaResult = result as CriteriaOperator;
				if(!ReferenceEquals(criteriaResult, null))
					result = LocalaizableCriteriaToStringProcessor.Process(Localizer.Active, criteriaResult);
				if(result == null)
					return string.Empty;
				return result.ToString();
			}
		}
		public virtual string GetFilterDisplayText(ViewFilter filter) {
			if(filter.IsLegacyFilterDisplayTextMode()) {
				string result = string.Empty;
				if(!ReferenceEquals(filter.NonColumnFilterCriteria, null))
					result = "(" + GetFilterDisplayText(filter.NonColumnFilterCriteria) + ")";
				foreach(ViewColumnFilterInfo vcfi in filter) {
					ColumnFilterInfo fi = vcfi.Filter;
					string filterString;
					if(fi.DisplayText != null && fi.DisplayText.Length > 0) {
						filterString = fi.DisplayText;
					} else {
						filterString = GetFilterDisplayText(fi.FilterCriteria);
					}
					if(filterString != null && filterString.Length > 0) {
						if(result.Length > 0)
							result += " AND ";
						result += "(" + filterString + ")";
					}
				}
				return result;
			} else {
				return GetFilterDisplayText(filter.Criteria);
			}
		}
		protected internal override void ClearDataProperties() {
			base.ClearDataProperties();
			FindFilterText = string.Empty;
			ActiveFilter.Clear();
			SortInfo.Clear();
		}
		protected internal virtual string GetNonFormattedCaption(string caption) {
			return caption;
		}
		protected internal static object GetRowCellValueCore(int rowHandle, GridColumn column) {
			if(column == null || column.View == null) return null;
			return column.View.GetRowCellValue(rowHandle, column);
		}
		internal ColumnDateFilterInfo GetPrevDateFilterInfo(GridColumn column) {
			if(!dateFilterCache.ContainsKey(column)) return null;
			ColumnDateFilterInfo res = dateFilterCache[column];
			return res;
		}
		internal void OnDateFilterChanged(ColumnDateFilterInfo info) {
			if(info.Column == null) return;
			this.dateFilterCache[info.Column] = info;
		}
		DialogResult GetFormResult(Form frm) {
			if(this.GridControl != null && this.GridControl.FindForm() != null)
				return frm.ShowDialog(this.GridControl.FindForm());
			return frm.ShowDialog();
		}
		public virtual void ShowUnboundExpressionEditor(GridColumn column) {
			using(ExpressionEditorForm form = new UnboundColumnExpressionEditorForm(new GridColumnIDataColumnInfoWrapper(column, GridColumnIDataColumnInfoWrapperEnum.ExpressionEditor), null)) {
				if(this.GridControl != null)
					form.SetMenuManager(this.GridControl.MenuManager);
				form.StartPosition = FormStartPosition.CenterParent;
				InitDialogFormProperties(form);
				UnboundExpressionEditorEventArgs ea = new UnboundExpressionEditorEventArgs(form, column);
				OnUnboundExpressionEditorCreated(ea);
				if(!ea.ShowExpressionEditor) return;
				if(GetFormResult(form) == DialogResult.OK) 
					column.UnboundExpression = form.Expression;
			}
		}
		protected internal virtual void InitDialogFormProperties(XtraForm form) { }
		protected internal override void ResetLookUp(bool sameDataSource) {
			if(ViewInfo == null) return;
			ViewInfo.IsReady = false;
			BeginUpdate();
			try {
				if(!sameDataSource)
					RecreateDataController();
				FocusedRowHandle = 0;
			}
			finally {
				CancelUpdate();
			}
		}
		protected internal virtual void OnColumnSummaryCollectionChanged(GridColumn column, CollectionChangeEventArgs e) {
		}
		protected internal virtual bool CanDesignerSelectColumn(GridColumn gridColumn) {
			return true;
		}	   
		#region IColumnViewSearchClient Members        
		SearchColumnsInfo searchInfo;
		bool UseSearchInfo { get { return GridControl.IsAttachedToSearchControl && searchInfo != null; } }
		void IColumnViewSearchClient.ApplyFindFilter(SearchInfoBase args) {
			this.searchInfo = args as SearchColumnsInfo;
			this.ApplyFindFilter(args != null ? args.SearchText : null);
		}
		void IColumnViewSearchClient.FindPanelVisibilityChange() {
			FindPanelVisible = !GridControl.IsAttachedToSearchControl && OptionsFind.AlwaysVisible;
		}
		#endregion
		protected internal virtual int GetMaxGroupCount() { return -1; }
	}
	public class FindRowArgs {
		int startRowHandle;
		GridColumn column;
		string text;
		bool ignoreStartRowHandle, allowLoop, down;
		public FindRowArgs(int startRowHandle, GridColumn column, string text, bool down) : this(startRowHandle, column, text, false, true, down) {
		}
		public FindRowArgs(int startRowHandle, GridColumn column, string text, bool ignoreStartRowHandle, bool allowLoop, bool down) {
			this.startRowHandle = startRowHandle;
			this.column = column;
			this.text = text;
			this.ignoreStartRowHandle = ignoreStartRowHandle;
			this.down = down;
			this.allowLoop = allowLoop;
		}
		public int StartRowHandle { get { return startRowHandle; } }
		public GridColumn Column { get { return column; } }
		public string Text { get { return text; } }
		public bool IgnoreStartRowHandle { get { return ignoreStartRowHandle; } }
		public bool AllowLoop { get { return allowLoop; } }
		public bool Down { get { return down; } }
	}
	public class ColumnViewValidateEditorEventArgs : BaseContainerValidateEditorEventArgs {
		ColumnView columnView;
		public ColumnViewValidateEditorEventArgs(ColumnView columnView, object value)
			: base(value) {
			this.columnView = columnView;
		}
		protected override bool CanValidateEditorViaAnnotationAttributes() {
			return columnView.FocusedRowHandle != GridControl.AutoFilterRowHandle;
		}
		protected override bool CanValidateRowViaAnnotationAttributes() {
			return columnView.IsDataRow(columnView.FocusedRowHandle) || columnView.IsNewItemRow(columnView.FocusedRowHandle);
		}
		protected override object GetRowObject() {
			return columnView.GetRow(columnView.FocusedRowHandle);
		}
	}
	public class GridCell {
		int rowHandle;
		GridColumn column;
		public GridCell(int rowHandle, GridColumn column) {
			this.column = column;
			this.rowHandle = rowHandle;
		}
#if !SL
	[DevExpressXtraGridLocalizedDescription("GridCellRowHandle")]
#endif
		public int RowHandle { get { return rowHandle; } }
#if !SL
	[DevExpressXtraGridLocalizedDescription("GridCellColumn")]
#endif
		public GridColumn Column { get { return column; } }
		public bool Equals(GridCell anchor) {
			return anchor != null && anchor.RowHandle == RowHandle && anchor.Column == Column; 
		} 
		internal bool IsColumnValid { get { return Column != null && Column.Visible && Column.VisibleIndex >= 0; } }
		public override string ToString() {
			if(rowHandle == GridControl.InvalidRowHandle) return "InvalidRowHandle";
			return string.Format("{0} {1}", rowHandle, Column == null ? "<null>" : Column.GetCaption());
		}
	}
	public class CellToolTipInfo {
		int rowHandle;
		GridColumn column;
		object cellObject;
		public CellToolTipInfo(int rowHandle, GridColumn column, object cellObject) {
			this.rowHandle = rowHandle;
			this.column = column;
			this.cellObject = cellObject;
		}
		public int RowHandle { get { return rowHandle; } }
		public GridColumn Column { get { return column; } }
		public object CellObject { get { return cellObject; } }
		public override bool Equals(object obj) {
			CellToolTipInfo info = obj as CellToolTipInfo;
			if(info == null) return false;
			return info.RowHandle == this.RowHandle && info.Column == this.Column &&
				Object.Equals(info.CellObject, this.CellObject);
		}
		public override int GetHashCode() {
			return string.Format("{0},{1},{2}", this.RowHandle, this.Column == null ? 0 : this.Column.GetHashCode(), this.CellObject == null ? 0 : this.CellObject.GetHashCode()).GetHashCode();
		}
	}
	public delegate void MethodIntInvoker(int value);
}
