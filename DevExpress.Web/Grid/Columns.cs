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

using DevExpress.Data;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Data.Helpers;
using DevExpress.Utils;
using DevExpress.Web.Data;
using DevExpress.Web.FilterControl;
using DevExpress.XtraGrid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace DevExpress.Web.Internal {
	public interface IWebColumn {
		bool GetAllowEllipsisInText();
	}
	public interface IWebGridColumn : IWebColumn {
		string Name { get; set; }
		string Caption { get; set; }
		int VisibleIndex { get; set; }
		bool Visible { get; set; }
		GridExportAppearanceBase ExportCellStyle { get; }
	}
	public interface IWebGridDataColumn : IWebGridColumn {
		string FieldName { get; set; }
		UnboundColumnType UnboundType { get; set; }
		string UnboundExpression { get; set; }
		int SortIndex { get; set; }
		ColumnSortOrder SortOrder { get; set; }
		bool ReadOnly { get; set; }
		EditPropertiesBase PropertiesEdit { get; set; }
		GridDataColumnAdapter Adapter { get; }
	}
	public interface IWebGridDataColumnAdapterOwner {
		WebColumnBase Column { get; }
		bool HasGrouping { get; }
		bool HasAutoFilter { get; }
		Func<GridDataColumnAdapter, GridDataColumnSettings> CreateSettings { get; }
		Func<GridDataColumnAdapter, GridDataColumnHeaderFilterSettings> CreateSettingsHeaderFilter { get; }
	}
	public class GridDataColumnAdapter : StateManager, IFilterColumn, IDateEditIDResolver {
		EditPropertiesBase propertiesEdit;
		GridDataColumnSettings settings;
		GridDataColumnHeaderFilterSettings settingsHeaderFilter;
		public GridDataColumnAdapter(IWebGridDataColumnAdapterOwner owner) {
			Owner = owner;
		}
		public IWebGridDataColumnAdapterOwner Owner { get; private set; }
		public bool HasGrouping { get { return Owner.HasGrouping; } }
		public bool HasAutoFilter { get { return Owner.HasAutoFilter; } }
		public WebColumnBase Column { get { return Owner.Column; } }
		public GridDataColumnSettings Settings {
			get {
				if(settings == null)
					settings = Owner.CreateSettings(this);
				return settings;
			}
		}
		public GridViewDataColumnSettings AdvancedSettings { get { return Settings as GridViewDataColumnSettings; } }
		public GridDataColumnHeaderFilterSettings SettingsHeaderFilter {
			get {
				if(settingsHeaderFilter == null)
					settingsHeaderFilter = Owner.CreateSettingsHeaderFilter(this);
				return settingsHeaderFilter;
			}
		}
		public ASPxGridBase Grid {
			get {
				var collection = Column.Collection as GridColumnCollection;
				return collection != null ? collection.Grid : null;
			}
		}
		protected ASPxGridView GridView { get { return Grid as ASPxGridView; } }
		public WebDataProxy DataProxy { get { return Grid != null ? Grid.DataProxy : null; } }
		public EditPropertiesBase PropertiesEdit {
			get {
				if(propertiesEdit == null)
					propertiesEdit = CreateEditProperties();
				return propertiesEdit;
			}
			set {
				propertiesEdit = value;
				LayoutChanged();
			}
		}
		public virtual string FieldName {
			get { return GetStringProperty("FieldName", string.Empty); }
			set {
				if(value == null) value = string.Empty;
				if(value == FieldName) return;
				SetStringProperty("FieldName", string.Empty, value);
				OnColumnBindingChanged();
			}
		}
		public UnboundColumnType UnboundType {
			get { return (UnboundColumnType)GetEnumProperty("UnboundType", UnboundColumnType.Bound); }
			set {
				if(UnboundType == value) return;
				SetEnumProperty("UnboundType", UnboundColumnType.Bound, (int)value);
				OnColumnBindingChanged();
			}
		}
		public virtual string UnboundExpression {
			get { return GetStringProperty("UnboundExpression", string.Empty); }
			set {
				if(value == null) value = string.Empty;
				if(UnboundExpression == value) return;
				SetStringProperty("UnboundExpression", string.Empty, value);
				OnColumnBindingChanged();
			}
		}
		public int SortIndex {
			get { return GetIntProperty("SortIndex", -1); }
			set {
				if(Grid != null)
					value = Grid.SortBy(Column as IWebGridColumn, value);
				SetSortIndex(value);
			}
		}
		public virtual ColumnSortOrder SortOrder {
			get { return (ColumnSortOrder)GetEnumProperty("SortOrder", ColumnSortOrder.None); }
			set {
				if(Grid != null)
					value = Grid.SortBy(Column as IWebGridColumn, value);
				else {
					SortIndex = value == ColumnSortOrder.None ? -1 : (SortIndex == -1 ? 0 : SortIndex);
				}
				SetSortOrder(value);
			}
		}
		public int GroupIndex { 
			get {
				if(!HasGrouping) return -1;
				return GetIntProperty("GroupIndex", -1);
			}
			set {
				if(!HasGrouping) return;
				if(GroupIndex == value) return;
				if(GridView != null) {
					value = GridView.GroupBy(Column as IWebGridColumn, value);
				} else if(SortOrder == ColumnSortOrder.None && value > -1) {
					SortAscending();
				}
				SetGroupIndex(value);
			}
		}
		public virtual bool ReadOnly { get { return GetBoolProperty("ReadOnly", false); } set { SetBoolProperty("ReadOnly", false, value); } }
		public string FilterExpression { get { return Grid != null ? Grid.GetColumnFilterString(Column as IWebGridDataColumn) : string.Empty; } }
		public bool AutoGenerated { get; set; }
		public void SortAscending() {
			SortOrder = ColumnSortOrder.Ascending;
		}
		public void SortDescending() {
			SortOrder = ColumnSortOrder.Descending;
		}
		public void UnSort() {
			SortIndex = -1;
		}
		public void AutoFilterBy(string value) { 
			if(!HasAutoFilter) return;
			if(GridView != null)
				GridView.AutoFilterByColumn(Column as IWebGridColumn, value);
		}
		public void GroupBy() { 
			if(!HasGrouping) return;
			if(GridView != null)
				GridView.GroupBy(Column as IWebGridColumn);
		}
		public void UnGroup() { 
			if(!HasGrouping) return;
			GroupIndex = -1;
		}
		public bool ShowInFilterControl {
			get {
				if(!CheckServerActionType(ColumnServerActionType.Filter))
					return false;
				if(Settings.ShowInFilterControl == DefaultBoolean.Default)
					return Column.Visible && !string.IsNullOrEmpty(FieldName);
				return Settings.ShowInFilterControl == DefaultBoolean.True;
			}
		}
		protected virtual EditPropertiesBase CreateEditProperties() {
			var webColumnInfo = Column as IWebColumnInfo;
			return webColumnInfo != null ? webColumnInfo.CreateEditProperties() : null;
		}
		public virtual void Assign(object source) {
			var src = source as GridDataColumnAdapter;
			if(src == null) return;
			AssignEditProperties(src);
			Settings.Assign(src.Settings);
			SettingsHeaderFilter.Assign(src.SettingsHeaderFilter);
			ReadOnly = src.ReadOnly;
			FieldName = src.FieldName;
			UnboundExpression = src.UnboundExpression;
			UnboundType = src.UnboundType;
			AutoGenerated = src.AutoGenerated;
			SetSortIndex(src.SortIndex);
			SetSortOrder(src.SortOrder);
			SetGroupIndex(src.GroupIndex);
		}
		protected virtual void AssignEditProperties(GridDataColumnAdapter src) {
			if(PropertiesEdit == null)
				PropertiesEdit = src.PropertiesEdit;
			if(PropertiesEdit != null)
				PropertiesEdit.Assign(src.PropertiesEdit);
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.MergedBaseAndNewStateManagedObjects(base.GetStateManagedObjects(), PropertiesEdit, Settings, SettingsHeaderFilter);
		}
		protected bool CheckServerActionType(ColumnServerActionType type) {
			if(Grid == null || !DataProxy.IsServerMode) return true;
			var actionsSupport = DataProxy.GetData() as IColumnsServerActions;
			if(actionsSupport == null) return true;
			return actionsSupport.AllowAction(FieldName, type);
		}
		protected internal ColumnSortOrder UngroupedSortOrder { get; set; }
		protected internal void SetSortIndex(int value) {
			SetIntProperty("SortIndex", -1, value);
		}
		protected internal void SetSortOrder(ColumnSortOrder value) {
			SetEnumProperty("SortOrder", ColumnSortOrder.None, value);
		}
		protected internal void SetGroupIndex(int value) {
			if(!HasGrouping) return;
			SetIntProperty("GroupIndex", -1, value);
		}
		protected virtual void OnColumnBindingChanged() {
			if(!IsLoading() && Grid != null)
				Grid.OnColumnBindingChanged();
		}
		public virtual string GetDisplayName() {
			if(!String.IsNullOrEmpty(Column.Caption))
				return Column.Caption;
			if(Grid != null) {
				var info = GetColumnInfo();
				if(info != null)
					return CommonUtils.SplitPascalCaseString(info.Caption);
			}
			if(!String.IsNullOrEmpty(FieldName))
				return CommonUtils.SplitPascalCaseString(FieldName);
			if(IsDesignMode())
				return "Column";
			return string.Empty;
		}
		public DataColumnInfo GetColumnInfo() {
			return Grid != null ? DataProxy.GetColumnInfo(FieldName) : null;
		}
		public bool HasData { get { return !string.IsNullOrEmpty(FieldName) || UnboundType != UnboundColumnType.Bound; } }
		public GridHeaderFilterMode HeaderFilterMode {
			get {
				if(SettingsHeaderFilter.ModeChanged)
					return SettingsHeaderFilter.Mode;
				if(!Settings.HeaderFilterModeChanged)
					return GridHeaderFilterMode.Default;
				return Settings.HeaderFilterMode == Web.HeaderFilterMode.List ? GridHeaderFilterMode.List : GridHeaderFilterMode.CheckedList;
			}
		}
		public bool IsMultiSelectHeaderFilter { get { return IsDateRangeHeaderFilterMode || HeaderFilterMode == GridHeaderFilterMode.CheckedList; } }
		public bool IsDateRangeHeaderFilterMode { get { return IsDateTime && (HeaderFilterMode == GridHeaderFilterMode.Default || IsDateRangeHeaderFilterModeSelected); } }
		bool IsDateRangeHeaderFilterModeSelected { get { return HeaderFilterMode == GridHeaderFilterMode.DateRangePicker || HeaderFilterMode == GridHeaderFilterMode.DateRangeCalendar; } }
		public bool AllowSort {
			get {
				if(PropertiesEdit is ImageEditPropertiesBase)
					return HasData && Settings.AllowSort == DefaultBoolean.True;
				return AllowSortInternal;
			}
		}
		protected bool AllowSortInternal {
			get {
				if(!HasData || !CheckServerActionType(ColumnServerActionType.Sort))
					return false;
				if(Settings.AllowSort == DefaultBoolean.Default) {
					if(!IsPossibleCompareColumnValues())
						return false;
					if(Grid != null)
						return Grid.SettingsBehavior.AllowSort;
				}
				return Settings.AllowSort != DefaultBoolean.False;
			}
		}
		protected virtual bool IsPossibleCompareColumnValues() {
			var info = GetColumnInfo();
			return info == null || info.AllowSort;
		}
		public bool HasFilterButton {
			get {
				if(!HasData || !CheckServerActionType(ColumnServerActionType.Filter))
					return false;
				if(Grid != null && Settings.AllowHeaderFilter == DefaultBoolean.Default)
					return Grid.Settings.ShowHeaderFilterButton;
				return Settings.AllowHeaderFilter == DefaultBoolean.True;
			}
		}
		public bool IsFiltered { get { return !string.IsNullOrEmpty(FilterExpression); } }
		public ColumnFilterMode FilterMode {
			get {
				if(Grid != null) {
					var prop = Grid.RenderHelper.GetColumnEdit(Column as IWebGridDataColumn);
					return GridViewFilterHelper.GetColumnFilterMode(Settings.FilterMode, prop is CheckBoxProperties, DataProxy.IsServerMode);
				}
				return Settings.FilterMode;
			}
		}
		public bool AllowDragDrop {
			get {
				if(Grid != null && Settings.AllowDragDrop == DefaultBoolean.Default)
					return Grid.SettingsBehavior.AllowDragDrop;
				return Settings.AllowDragDrop != DefaultBoolean.False;
			}
		}
		public bool AllowSearchPanelFilter {
			get {
				var prop = PropertiesEdit;
				if(prop is ImageEditPropertiesBase || prop is ProgressBarProperties)
					return false;
				if(prop is CheckBoxProperties)
					return AllowSearchPanelFilterInternal && !(prop as CheckBoxProperties).UseDisplayImages;
				return AllowSearchPanelFilterInternal;
			}
		}
		protected bool AllowSearchPanelFilterInternal {
			get {
				if(Grid != null && CheckServerActionType(ColumnServerActionType.Filter))
					return Grid.DataProxy.AllowSearchPanelFilter(GetColumnInfo());
				return false;
			}
		}
		public bool IsDateTime { get { return DataType == typeof(DateTime); } }
		public virtual Type DataType {
			get {
				if(PropertiesEdit is DateEditProperties && DataTypeInternal == typeof(object))
					return typeof(DateTime);
				return DataTypeInternal;
			}
		}
		protected Type DataTypeInternal {
			get {
				if(Grid == null)
					return typeof(object);
				var type = ReflectionUtils.StripNullableType(DataProxy.GetFieldType(FieldName));
				if(type == null)
					return typeof(object);
				return type;
			}
		}
		public bool AllowGroup {
			get {
				if(!HasGrouping) return false;
				if(PropertiesEdit is ImageEditPropertiesBase)
					return false;
				return AllowGroupInternal;
			}
		}
		protected bool AllowGroupInternal {
			get {
				if(!HasGrouping) return false;
				if(!HasData || !CheckServerActionType(ColumnServerActionType.Group))
					return false;
				if(AdvancedSettings.AllowGroup == DefaultBoolean.Default) {
					var info = GetColumnInfo();
					if(info != null && !info.AllowSort)
						return false;
					if(GridView != null)
						return GridView.SettingsBehavior.AllowGroup;
				}
				return AdvancedSettings.AllowGroup != DefaultBoolean.False;
			}
		}
		public bool AllowAutoFilter {
			get {
				if(!HasAutoFilter) return false;
				if(PropertiesEdit is ImageEditPropertiesBase || PropertiesEdit is TimeEditProperties)
					return false;
				return AllowAutoFilterInternal;
			}
		}
		protected bool AllowAutoFilterInternal {
			get {
				if(!HasAutoFilter) return false;
				if(!CheckServerActionType(ColumnServerActionType.Filter))
					return false;
				return AdvancedSettings.AllowAutoFilter != DefaultBoolean.False && HasData;
			}
		}
		public void ResetPropertiesEdit() {
			PropertiesEdit = null;
		}
		public EditPropertiesBase ExportPropertiesEdit { get { return Grid != null ? Grid.RenderHelper.GetColumnEdit(Column as IWebGridDataColumn) : PropertiesEdit; } }
		#region IFilterColumn Members
		string IFilterablePropertyInfo.PropertyName { get { return FieldName; } }
		string IFilterablePropertyInfo.DisplayName { get { return GetDisplayName(); } }
		int IFilterColumn.Index { get { return Column.Index; } }
		FilterColumnClauseClass IFilterColumn.ClauseClass { get { return GetClauseClass(); } }
		EditPropertiesBase IFilterColumn.PropertiesEdit {
			get {
				if(FilterMode == ColumnFilterMode.DisplayText)
					return EditRegistrationInfo.CreatePropertiesByDataType(typeof(string));
				return PropertiesEdit;
			}
		}
		Type IFilterablePropertyInfo.PropertyType {
			get {
				if(FilterMode == ColumnFilterMode.DisplayText)
					return typeof(string);
				return DataType;
			}
		}
		#endregion
		protected virtual FilterColumnClauseClass GetClauseClass() {
			if(FilterMode == ColumnFilterMode.DisplayText)
				return FilterColumnClauseClass.String;
			if(PropertiesEdit != null && PropertiesEdit.GetEditorType() != EditorType.Generic) {
				if(PropertiesEdit.GetEditorType() == EditorType.Blob)
					return FilterColumnClauseClass.Blob;
				return FilterColumnClauseClass.Lookup;
			}
			if(DataType == typeof(string))
				return FilterColumnClauseClass.String;
			return FilterColumnClauseClass.Generic;
		}
		public void OnColumnChanged() {
			Column.OnColumnChanged();
		}
		protected bool IsLoading() {
			return (Column as IWebControlObject).IsLoading();
		}
		protected bool IsDesignMode() {
			return (Column as IWebControlObject).IsDesignMode();
		}
		protected void LayoutChanged() {
			(Column as IWebControlObject).LayoutChanged();
		}
		#region IDateEditIDResolver Members
		string IDateEditIDResolver.GetDateEditIdByDataItemName(string dataItemName) {
			if(Grid.RenderHelper.IsBatchEditCellMode)
				return string.Empty;
			var targetColumn = GridColumnHelper.FindColumnByStringRecursive(Grid.ColumnHelper.AllDataColumns, dataItemName) as IWebGridDataColumn;
			if(targetColumn != null && targetColumn.PropertiesEdit is DateEditProperties && targetColumn != Column)
				return Grid.RenderHelper.GetEditorId(targetColumn);
			return string.Empty;
		}
		string[] IDateEditIDResolver.GetPossibleDataItemNames() {
			var dateColumns = Grid.ColumnHelper.AllDataColumns.Where(c => c.PropertiesEdit is DateEditProperties && c != Column);
			return dateColumns.Select(c => c.ToString()).Where(n => !string.IsNullOrEmpty(n)).ToArray();
		}
		#endregion
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class GridDataColumnSettings : StateManager {
		internal GridDataColumnSettings(GridDataColumnAdapter columnAdapter) {
			ColumnAdapter = columnAdapter;
		}
		public GridDataColumnSettings() { }
		protected internal DefaultBoolean AllowDragDrop {
			get { return GetDefaultBooleanProperty("AllowDragDrop", DefaultBoolean.Default); }
			set {
				if(value == AllowDragDrop) return;
				SetDefaultBooleanProperty("AllowDragDrop", DefaultBoolean.Default, value);
				OnChanged();
			}
		}
		protected internal DefaultBoolean AllowEllipsisInText {
			get { return GetDefaultBooleanProperty("AllowEllipsisInText", DefaultBoolean.Default); }
			set {
				if(value == AllowEllipsisInText) return;
				SetDefaultBooleanProperty("AllowEllipsisInText", DefaultBoolean.Default, value);
				OnChanged();
			}
		}
		protected internal ColumnFilterMode FilterMode {
			get { return (ColumnFilterMode)GetEnumProperty("FilterMode", ColumnFilterMode.Value); }
			set {
				if(FilterMode == value) return;
				SetEnumProperty("FilterMode", ColumnFilterMode.Value, value);
				OnChanged();
			}
		}
		protected internal ColumnSortMode SortMode {
			get { return (ColumnSortMode)GetEnumProperty("SortMode", ColumnSortMode.Default); }
			set {
				if(SortMode == value) return;
				SetEnumProperty("SortMode", ColumnSortMode.Default, value);
				OnChanged();
			}
		}
		protected internal DefaultBoolean AllowHeaderFilter {
			get { return GetDefaultBooleanProperty("AllowHeaderFilter", DefaultBoolean.Default); }
			set {
				if(value == AllowHeaderFilter) return;
				SetDefaultBooleanProperty("AllowHeaderFilter", DefaultBoolean.Default, value);
				OnChanged();
			}
		}
		protected internal DefaultBoolean ShowInFilterControl {
			get { return GetDefaultBooleanProperty("ShowInFilterControl", DefaultBoolean.Default); }
			set {
				if(value == ShowInFilterControl) return;
				SetDefaultBooleanProperty("ShowInFilterControl", DefaultBoolean.Default, value);
				OnCollectionChanged();
			}
		}
		protected internal DefaultBoolean AllowSort {
			get { return GetDefaultBooleanProperty("AllowSort", DefaultBoolean.Default); }
			set {
				if(value == AllowSort) return;
				SetDefaultBooleanProperty("AllowSort", DefaultBoolean.Default, value);
				OnChanged();
			}
		}
		protected internal HeaderFilterMode HeaderFilterMode {
			get { return (HeaderFilterMode)GetEnumProperty("HeaderFilterMode", HeaderFilterMode.List); }
			set {
				if(HeaderFilterModeChanged && HeaderFilterMode == value)
					return;
				SetEnumProperty("HeaderFilterMode", HeaderFilterMode.List, value);
				HeaderFilterModeChanged = true;
				OnChanged();
			}
		}
		protected internal bool HeaderFilterModeChanged {
			get { return GetBoolProperty("HeaderFilterModeChanged", false); }
			set { SetBoolProperty("HeaderFilterModeChanged", false, value); }
		}
		protected internal DefaultBoolean AllowFilterBySearchPanel {
			get { return (DefaultBoolean)GetEnumProperty("AllowFilterBySearchPanel", DefaultBoolean.Default); }
			set {
				if(AllowFilterBySearchPanel == value) return;
				SetEnumProperty("AllowFilterBySearchPanel", DefaultBoolean.Default, value);
				OnFilterChanged();
			}
		}
		public virtual void Assign(GridDataColumnSettings source) {
			if(source == null) return;
			AllowDragDrop = source.AllowDragDrop;
			AllowEllipsisInText = source.AllowEllipsisInText;
			AllowSort = source.AllowSort;
			AllowHeaderFilter = source.AllowHeaderFilter;
			FilterMode = source.FilterMode;
			SortMode = source.SortMode;
			ShowInFilterControl = source.ShowInFilterControl;
			if(source.HeaderFilterModeChanged)
				HeaderFilterMode = source.HeaderFilterMode;
			AllowFilterBySearchPanel = source.AllowFilterBySearchPanel;
		}
		protected GridDataColumnAdapter ColumnAdapter { get; private set; }
		protected ASPxGridBase Grid { get { return ColumnAdapter != null ? ColumnAdapter.Grid : null; } }
		protected void OnChanged() {
			if(ColumnAdapter != null)
				ColumnAdapter.OnColumnChanged();
		}
		protected void OnCollectionChanged() {
			if(Grid != null)
				(Grid as IWebColumnsOwner).OnColumnCollectionChanged();
		}
		protected void OnFilterChanged() {
			if(Grid != null)
				Grid.OnFilterChanged();
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public abstract class GridDataColumnHeaderFilterSettings : StateManager {
		internal GridDataColumnHeaderFilterSettings(GridDataColumnAdapter columnAdapter)
			: this() {
			ColumnAdapter = columnAdapter;
		}
		public GridDataColumnHeaderFilterSettings() {
			DateRangeCalendarSettings = CreateCalendarSettings();
			DateRangePickerSettings = CreateDateRangePickerSettings();
			DateRangePeriodsSettings = CreateDateRangePeriodsSettings();
		}
		protected internal GridHeaderFilterMode Mode {
			get { return (GridHeaderFilterMode)GetEnumProperty("Mode", GridHeaderFilterMode.Default); }
			set {
				if(ModeChanged && Mode == value)
					return;
				SetEnumProperty("Mode", GridHeaderFilterMode.Default, value);
				ModeChanged = true;
				OnChanged();
			}
		}
		protected internal bool ModeChanged {
			get { return GetBoolProperty("ModeChanged", false); }
			set { SetBoolProperty("ModeChanged", false, value); }
		}
		protected internal GridColumnDateRangeCalendarSettings DateRangeCalendarSettings { get; private set; }
		protected internal GridColumnDateRangePickerSettings DateRangePickerSettings { get; private set; }
		protected internal GridColumnDateRangePeriodsSettings DateRangePeriodsSettings { get; private set; }
		protected GridDataColumnAdapter ColumnAdapter { get; private set; }
		protected abstract GridColumnDateRangeCalendarSettings CreateCalendarSettings();
		protected abstract GridColumnDateRangePickerSettings CreateDateRangePickerSettings();
		protected abstract GridColumnDateRangePeriodsSettings CreateDateRangePeriodsSettings();
		public virtual void Assign(GridDataColumnHeaderFilterSettings source) {
			if(source == null)
				return;
			if(source.ModeChanged)
				Mode = source.Mode;
			DateRangeCalendarSettings.Assign(source.DateRangeCalendarSettings);
			DateRangePickerSettings.Assign(source.DateRangePickerSettings);
			DateRangePeriodsSettings.Assign(source.DateRangePeriodsSettings);
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { DateRangeCalendarSettings, DateRangePickerSettings });
		}
		protected void OnChanged() {
			if(ColumnAdapter != null)
				ColumnAdapter.OnColumnChanged();
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public abstract class GridColumnDateRangeCalendarSettings : StateManager {
		public GridColumnDateRangeCalendarSettings() {
			Properties = new CalendarProperties();
			EnableMultiSelect = true;
		}
		protected internal string ClearButtonText { get { return Properties.ClearButtonText; } set { Properties.ClearButtonText = value; } }
		protected internal string TodayButtonText { get { return Properties.TodayButtonText; } set { Properties.TodayButtonText = value; } }
		protected internal bool ShowClearButton { get { return Properties.ShowClearButton; } set { Properties.ShowClearButton = value; } }
		protected internal bool ShowTodayButton { get { return Properties.ShowTodayButton; } set { Properties.ShowTodayButton = value; } }
		protected internal bool ShowHeader { get { return Properties.ShowHeader; } set { Properties.ShowHeader = value; } }
		protected internal bool ShowDayHeaders { get { return Properties.ShowDayHeaders; } set { Properties.ShowDayHeaders = value; } }
		protected internal bool ShowWeekNumbers { get { return Properties.ShowWeekNumbers; } set { Properties.ShowWeekNumbers = value; } }
		protected internal bool HighlightWeekends { get { return Properties.HighlightWeekends; } set { Properties.HighlightWeekends = value; } }
		protected internal bool HighlightToday { get { return Properties.HighlightToday; } set { Properties.HighlightToday = value; } }
		protected internal bool EnableMonthNavigation { get { return Properties.EnableMonthNavigation; } set { Properties.EnableMonthNavigation = value; } }
		protected internal bool EnableYearNavigation { get { return Properties.EnableYearNavigation; } set { Properties.EnableYearNavigation = value; } }
		protected internal bool EnableMultiSelect { get { return Properties.EnableMultiSelect; } set { Properties.EnableMultiSelect = value; } }
		protected internal FirstDayOfWeek FirstDayOfWeek { get { return Properties.FirstDayOfWeek; } set { Properties.FirstDayOfWeek = value; } }
		protected internal DateTime MinDate { get { return Properties.MinDate; } set { Properties.MinDate = value; } }
		protected internal DateTime MaxDate { get { return Properties.MaxDate; } set { Properties.MaxDate = value; } }
		protected CalendarProperties Properties { get; private set; }
		public virtual void Assign(GridColumnDateRangeCalendarSettings source) {
			if(source == null)
				return;
			Properties.Assign(source.Properties);
		}
		protected internal void AssignToControl(ASPxCalendar calendar) {
			if(calendar == null)
				return;
			calendar.Properties.Assign(Properties);
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public abstract class GridColumnDateRangePickerSettings : StateManager {
		protected const string DefaultDisplayFormatString = "d";
		protected internal string DisplayFormatString {
			get { return GetStringProperty("DisplayFormatString", DefaultDisplayFormatString); }
			set { SetStringProperty("DisplayFormatString", DefaultDisplayFormatString, value); }
		}
		protected internal int MinDayCount {
			get { return GetIntProperty("MinDayCount", 0); }
			set { SetIntProperty("MinDayCount", 0, value); }
		}
		protected internal int MaxDayCount {
			get { return GetIntProperty("MaxDayCount", 0); }
			set { SetIntProperty("MaxDayCount", 0, value); }
		}
		public virtual void Assign(GridColumnDateRangePickerSettings source) {
			if(source == null)
				return;
			DisplayFormatString = source.DisplayFormatString;
			MinDayCount = source.MinDayCount;
			MaxDayCount = source.MaxDayCount;
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public abstract class GridColumnDateRangePeriodsSettings : StateManager {
		protected internal int RepeatColumns {
			get { return GetIntProperty("RepeatColumns", 2); }
			set { SetIntProperty("RepeatColumns", 2, value); }
		}
		protected internal bool ShowDaysSection {
			get { return GetBoolProperty("ShowDaysSection", true); }
			set { SetBoolProperty("ShowDaysSection", true, value); }
		}
		protected internal bool ShowWeeksSection {
			get { return GetBoolProperty("ShowWeeksSection", true); }
			set { SetBoolProperty("ShowWeeksSection", true, value); }
		}
		protected internal bool ShowMonthsSection {
			get { return GetBoolProperty("ShowMonthsSection", true); }
			set { SetBoolProperty("ShowMonthsSection", true, value); }
		}
		protected internal bool ShowYearsSection {
			get { return GetBoolProperty("ShowYearsSection", true); }
			set { SetBoolProperty("ShowYearsSection", true, value); }
		}
		protected internal bool ShowPastPeriods {
			get { return GetBoolProperty("ShowPastPeriods ", true); }
			set { SetBoolProperty("ShowPastPeriods ", true, value); }
		}
		protected internal bool ShowFuturePeriods {
			get { return GetBoolProperty("ShowFuturePeriods ", true); }
			set { SetBoolProperty("ShowFuturePeriods ", true, value); }
		}
		protected internal bool ShowPresentPeriods {
			get { return GetBoolProperty("ShowPresentPeriods ", true); }
			set { SetBoolProperty("ShowPresentPeriods ", true, value); }
		}
		public virtual void Assign(GridColumnDateRangePeriodsSettings source) {
			if(source == null)
				return;
			RepeatColumns = source.RepeatColumns;
			ShowDaysSection = source.ShowDaysSection;
			ShowWeeksSection = source.ShowWeeksSection;
			ShowMonthsSection = source.ShowMonthsSection;
			ShowYearsSection = source.ShowYearsSection;
			ShowPastPeriods = source.ShowPastPeriods;
			ShowFuturePeriods = source.ShowFuturePeriods;
			ShowPresentPeriods = source.ShowPresentPeriods;
		}
	}
	public class GridColumnCollection : WebColumnCollectionBase {  
		public GridColumnCollection(IWebControlObject webControlObject)
			: base(webControlObject) {
		}
		[Browsable(false)]
		protected internal ASPxGridBase Grid { get { return FindOwnerGrid(); } }
		protected IList List { get { return this as IList; } }
		public IWebGridColumn this[int index] { get { return List[index] as IWebGridColumn; } }
		public IWebGridColumn this[string ID_FieldName_Caption] { get { return GridColumnHelper.FindColumnByStringRecursive(this, ID_FieldName_Caption); } }
		public void Add(IWebGridColumn column) {
			List.Add(column);
		}
		public void AddRange(params IWebGridColumn[] columns) {
			BeginUpdate();
			try {
				foreach(var column in columns)
					List.Add(column);
			} finally {
				EndUpdate();
			}
		}
		public void Insert(int index, IWebGridColumn column) {
			List.Insert(index, column);
		}
		public void Remove(IWebGridColumn column) {
			List.Remove(column);
		}
		public int IndexOf(IWebGridColumn column) {
			return List.IndexOf(column);
		}
		protected override void OnClearComplete() {
			base.OnClearComplete();
			if(Grid != null) {
				Grid.ClearSort();
				Grid.ResetControlHierarchy();
			}
		}
		protected override void OnChanged() {
			base.OnChanged();
			var columnsOwner = Owner as IWebColumnsOwner;
			if(columnsOwner != null)
				columnsOwner.OnColumnCollectionChanged();
		}
		protected override void OnInsertComplete(int index, object value) {
			base.OnInsertComplete(index, value);
			var column = value as IWebGridDataColumn;
			if(Grid != null && column != null) {
				if(!column.Adapter.AutoGenerated)
					Grid.AutoGenerateColumns = false;
				if(column.UnboundType != UnboundColumnType.Bound)
					Grid.OnColumnBindingChanged();
			}
		}
		public override string ToString() { return string.Empty; }
		protected override Type GetKnownType() {
			return typeof(IWebGridColumn);
		}
		ASPxGridBase FindOwnerGrid() { 
			IWebControlObject current = Owner;
			while(current != null) {
				var grid = current as ASPxGridBase;
				if(grid != null)
					return grid;
				CollectionItem item = current as CollectionItem;
				if(item != null && item.Collection != null) {
					current = item.Collection.Owner;
				} else {
					current = null;
				}
			}
			return null;
		}
	}
	public class ReadOnlyGridColumnCollection<T> : System.Collections.ObjectModel.ReadOnlyCollection<T> where T : WebColumnBase {
		public ReadOnlyGridColumnCollection(IList<T> list)
			: base(list) {
		}
		public T this[string ID_FieldName_Caption] {
			get { return GridColumnHelper.FindColumnByStringRecursiveInternal(this, ID_FieldName_Caption) as T; }
		}
	}
	public abstract class GridCustomCommandButton : CollectionItem {
		public GridCustomCommandButton() {
			Image = new ImageProperties();
			Styles = new ButtonControlStyles(null);
		}
		[ Category("Behavior"), DefaultValue(""), NotifyParentProperty(true), Localizable(false)]
		public string ID {
			get { return GetStringProperty("ID", string.Empty); }
			set {
				if(value == ID) return;
				SetStringProperty("ID", string.Empty, value);
				LayoutChanged();
			}
		}
		[ Category("Appearance"), DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public string Text {
			get { return GetStringProperty("Text", string.Empty); }
			set {
				if(value == Text) return;
				SetStringProperty("Text", string.Empty, value);
				LayoutChanged();
			}
		}
		[ Category("Appearance"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties Image { get; private set; }
		[ Category("Appearance"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ButtonControlStyles Styles { get; private set; }
		public override void Assign(CollectionItem source) {
			var src = source as GridCustomCommandButton;
			if(src == null) return;
			ID = src.ID;
			Text = src.Text;
			Image.Assign(src.Image);
			Styles.CopyFrom(src.Styles);
		}
		protected override IStateManager[] GetStateManagedObjects() { return new IStateManager[] { Image, Styles }; }
		protected internal string GetText() { return !string.IsNullOrEmpty(Text) ? Text : ID; }
		protected internal string GetID() { return !string.IsNullOrEmpty(ID) ? ID : Text; }
	}
}
