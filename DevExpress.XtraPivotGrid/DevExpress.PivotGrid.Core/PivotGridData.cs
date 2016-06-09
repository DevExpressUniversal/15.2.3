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
using System.Drawing;
using System.IO;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.IO;
using DevExpress.Data.PivotGrid;
using DevExpress.Data.Utils;
using DevExpress.PivotGrid;
using DevExpress.PivotGrid.DataCalculation;
using DevExpress.PivotGrid.Events;
using DevExpress.PivotGrid.QueryMode.Sorting;
using DevExpress.Utils;
using DevExpress.Utils.Extensions.Helpers;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPivotGrid.Customization;
using DevExpress.XtraPivotGrid.Fields;
using DevExpress.XtraPivotGrid.Localization;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System;
using DevExpress.Compatibility.System.ComponentModel;
using System.Threading;
#if !DXPORTABLE
using DevExpress.PivotGrid.Printing;
using DevExpress.XtraPivotGrid.Design;
#endif
namespace DevExpress.XtraPivotGrid.Data {
	public class PivotGridData : IDisposable, IPrefilterOwnerBase, IFilteredComponentBase, IPivotGridDataSourceOwner, IPivotGridFieldsProvider
#if !SL && !DXPORTABLE
, IPivotGridOptionsPrintOwner
#endif
 {
		public const string WebResourcePath = "DevExpress.XtraPivotGrid.";
		public const string PivotGridImagesResourcePath = WebResourcePath + "Images.";
		public const int InvalidKPIValue = -2;
		public static bool IsValidKPIState(int state) {
			return state == 0 || state == -1 || state == 1;
		}
		IPivotGridEventsImplementorBase eventsImplementor;
		readonly PivotGridEmptyEventsImplementorBase emptyEventsImplementor;
		protected internal IPivotClipboardAccessor pivotClipboard;
		IPivotGridDataSource pivotDataSource;
		PivotGridFieldCollectionBase fields;
		PivotGridGroupCollection groups;
		int lockUpdateCount;
		PivotGridFieldReadOnlyCollections fieldCollections;
		PivotGridOptionsViewBase optionsView;
		PivotGridOptionsFilterBase optionsFilter;
		PivotGridOptionsCustomization optionsCustomization;
		PivotGridOptionsDataField optionsDataField;
#if !SL
		PivotGridOptionsPrint optionsPrint;
#endif
		PivotGridOptionsChartDataSourceBase optionsChartDataSource;
		PivotGridOptionsData optionsData;
		PivotGridOptionsSelection optionsSelection;
		PivotGridOptionsBehaviorBase optionsBehavior;
		PivotGridOptionsOLAP optionsOLAP;
		bool disposing;
		bool isDeserializing;
		PivotGridFieldBase dataField;
		internal PivotVisualItemsBase visualItems;
		readonly PrefilterBase prefilter;
		readonly List<WeakReference> dataSourceList;
		PivotChartDataSourceBase chartDataSource;
		PivotCellValuesProvider cellValuesProvider;
		PivotDataSourceObjectLevelHelper dataSourceObjectLeveHelper;
		bool hasRunningTotals;
		IList<AggregationLevel> aggregations;
		static readonly Dictionary<string, object> obsoleteProps;
		static PivotGridData() {
			obsoleteProps = new Dictionary<string, object>();
		}
		internal static bool IsPropertyObsolete(string propName) {
			return obsoleteProps.ContainsKey(propName);
		}
		public CancellationToken CancellationToken { get; set; }
		internal List<WeakReference> DataSourceList { get { return dataSourceList; } }
		public PivotDataSourceObjectLevelHelper DataSourceObjectLevelHelper {
			get { return dataSourceObjectLeveHelper; }
		}
		ICellValuesProvider ICellValueProvider {
			get { return CellValuesProvider; }
		}
		public PivotCellValuesProvider CellValuesProvider {
			get {
				cellValuesProvider.EnsureIsValid();
				return cellValuesProvider;
			}
		}
		public virtual bool IsDesignMode {
			get { return false; }
		}
		public virtual bool IsLocked {
			get { return false; }
		}
		public virtual string OLAPConnectionString {
			get { return OLAPDataSource != null ? OLAPDataSource.FullConnectionString : null; }
			set {
				if(disposing || OLAPConnectionString == value)
					return;
				if(value != null)
					ListSource = null;
				if(string.IsNullOrEmpty(value)) {
					SetPivotDataSource(CreateListDataSource(), false);
				} else {
					SetPivotDataSource(CreateOLAPDataSource(), false);
					OLAPDataSource.FullConnectionString = value;
				}
				NotifyPivotDataSourcesChanged();
				OnDataSourceChanged();
			}
		}
#if !SL
		public virtual bool IsServerMode { get { return IsOLAP || PivotDataSource is DevExpress.PivotGrid.ServerMode.ServerModeDataSource; } }
#else
		public virtual bool IsServerMode { get { return false; } }
#endif
		public virtual bool IsOLAP { get { return OLAPDataSource != null; } }
		public virtual bool DelayFieldsGroupingByHierarchies { get { return false; } }
		public PivotGridData() {
			this.emptyEventsImplementor = new PivotGridEmptyEventsImplementorBase();
			PivotDataSource = CreateListDataSource();
			this.lockUpdateCount = 0;
			this.fields = CreateFieldCollection();
			this.fieldCollections = new PivotGridFieldReadOnlyCollections();
			this.groups = CreateGroupCollection();
			this.dataField = CreateDataField();
			this.pivotClipboard = CreateClipboardAccessor();
			this.visualItems = CreateVisualItems();
			this.disposing = false;
			this.prefilter = CreatePrefilter();
			this.headerImages = null;
			this.valueImages = null;
			this.dataSourceList = new List<WeakReference>();
			this.dataSourceObjectLeveHelper = new PivotDataSourceObjectLevelHelper((IPivotGridDataSourceOwner)this, OptionsData);
			this.cellValuesProvider = new PivotCellValuesProvider((IPivotGridFieldsProvider)this, (IPivotGridDataSourceOwner)this, DataSourceObjectLevelHelper);
			this.chartDataSource = CreateChartDataSource();
			SetAggregations(new AggregationLevel[] { });
		}
		public void Dispose() {
			bool firstDisposing = !this.disposing;
			this.disposing = true;
			Dispose(firstDisposing);
		}
		protected virtual void Dispose(bool disposing) {
			if(!disposing)
				return;
			SetPivotDataSource(null, false);
			NotifyPivotDataSourcesChanged();
			this.groups.Clear();
			this.fields.ClearAndDispose();
			this.visualItems.Dispose();
			EventsImplementor = null;
			this.layoutChangedEvent = null;
			if(this.optionsChartDataSource != null)
				this.optionsChartDataSource.OptionsChanged -= OnOptionsChartDataSourceChanged;
		}
		protected virtual IPivotListDataSource CreateListDataSource() {
			return new PivotGridNativeDataSource(this);
		}
		protected virtual IPivotOLAPDataSource CreateOLAPDataSource() {
#if !SL && !DXPORTABLE
			switch(OLAPDataProvider) {
				case OLAPDataProvider.Adomd:
					return new PivotGridAdomdDataSource();
				case OLAPDataProvider.Default:
				case OLAPDataProvider.OleDb:
					return new PivotGridOLAPDataSource(this);
				case OLAPDataProvider.Xmla:
#endif
#if DXPORTABLE
					throw new ArgumentException("OLAPDataProvider");
#else
					return new PivotGridXmlaDataSource();
#endif
#if !SL && !DXPORTABLE
				default:
					throw new ArgumentException("OLAPDataProvider");
			}
#endif
		}
#if !SL
		protected virtual IPivotGridDataSource CreateFileDataSource() {
			return new PivotGridFileDataSource(this);
		}
#endif
		protected virtual IPivotClipboardAccessor CreateClipboardAccessor() {
			return new PivotClipboardEmptyAccessor();
		}
		public bool Disposing { get { return disposing; } }
		public virtual bool IsDeserializing { get { return isDeserializing; } }
		public void SetIsDeserializing(bool value) {
			if(IsDeserializing == value)
				return;
			isDeserializing = value;
		}
		public PivotGridFieldBase DataField { get { return dataField; } }
		protected virtual PivotGridFieldBase CreateDataField() {
			return new PivotGridFieldBase(this);
		}
		PivotGridFieldReadOnlyCollections FieldCollections {
			get { return fieldCollections; }
		}
		public virtual PivotVisualItemsBase VisualItems {
			get { return GetVisualItems(true); }
		}
		public PivotVisualItemsBase VisualItemsInternal {
			get { return GetVisualItems(false); }
		}
		PivotVisualItemsBase GetVisualItems(bool ensure) {
			if(ensure && visualItems != null && !Disposing)
				visualItems.EnsureIsCalculated();
			return visualItems;
		}
		public virtual PivotFieldItemCollection FieldItems {
			get {
				if(visualItems == null)
					return null;
				if(!Disposing)
					visualItems.EnsureFieldItems();
				return visualItems.FieldItems;
			}
		}
		protected internal virtual bool IsInMainThread { get { return true; } }
		public bool IsInProcessing { get { return IsLocked && IsInMainThread; } }
		public PivotGridFieldBase GetField(PivotFieldItemBase item) {
			if(item == null)
				return null;
			if(item.IsDataField)
				return DataField;
			if(item.IsRowTreeFieldItem)
				return VisualItems.RowTreeField;
			return Fields[item.Index];
		}
		public PivotFieldItemBase GetFieldItem(PivotGridFieldBase field) {
			if(field == null)
				return null;
			int index = Fields.IndexOf(field);
			if(index < 0) {
				if(field.IsDataField)
					return this.FieldItems.DataFieldItem;
				if(field.IsRowTreeField)
					return this.FieldItems.RowTreeFieldItem;
			}
			return this.FieldItems[index];
		}
		public PivotFieldItemBase GetFieldItem(PivotFieldItemBase item) {
			if(item == null)
				return null;
			if(item.IsDataField)
				return this.FieldItems.DataFieldItem;
			if(item.IsRowTreeFieldItem)
				return this.FieldItems.RowTreeFieldItem;
			return this.FieldItems[item.Index];
		}
		protected internal virtual PivotVisualItemsBase CreateVisualItems() {
			return new PivotVisualItemsBase(this);
		}
		protected internal virtual PivotGridData CreateEmptyInstance() {
			return null;
		}
		public bool IsDataBound {
			get {
				bool res = (ListSource != null);
				res = res || !string.IsNullOrEmpty(OLAPConnectionString) || IsServerMode;
				return res;
			}
		}
		public IList ListSource {
			get { return ListDataSource != null ? ListDataSource.ListSource : null; }
			set { SetListSource(value, true); }
		}
		public void SetListSource(IList value, bool restoreFileDataSource) {
			if(disposing || ListSource == value)
				return;
			LockEndRefresh();
			ResetListSourceChangedCounter();
			if(value != null)
				OLAPConnectionString = null;
#if !SL
			if(value is PivotFileDataSource && restoreFileDataSource)
				SetPivotDataSource(CreateFileDataSource(), false);
			else
#endif
				SetPivotDataSource(CreateListDataSource(), false);
			ListDataSource.SetListSource(value);
			NotifyPivotDataSourcesChanged();
			if(!GetIsListSourceChaged())
				OnDataSourceChanged();
			UnlockEndRefresh();
		}
		public virtual void SetControlDataSource(IList ds) { }
		public PrefilterBase Prefilter {
			get { return prefilter; }
		}
		protected virtual PrefilterBase CreatePrefilter() {
			return new PrefilterBase(this);
		}
		protected internal PivotChartDataSourceBase ChartDataSourceInternal { get { return chartDataSource; } }
		protected virtual PivotChartDataSourceBase CreateChartDataSource() {
			return new PivotChartDataSourceBase(this);
		}
		public bool CaseSensitive {
			get { return ListDataSource != null ? ListDataSource.CaseSensitive : true; }
			set {
				if(ListDataSource == null)
					return;
				ListDataSource.CaseSensitive = value;
				foreach(PivotGridFieldBase field in Fields)
					field.EnsureCaseSensitive();
			}
		}
		public bool AutoExpandGroups {
			get { return PivotDataSource.AutoExpandGroups; }
			set {
				PivotDataSource.AutoExpandGroups = value;
			}
		}
		public void SetAutoExpandGroups(bool value, bool reloadData) {
			PivotDataSource.SetAutoExpandGroups(value, reloadData);
		}
		public bool OlapEnableUnboundFields { get; set; }
		public PivotGridFieldCollectionBase Fields { get { return fields; } }
		public PivotGridGroupCollection Groups { get { return groups; } }
		public virtual IPivotGridEventsImplementorBase EventsImplementor {
			get { return eventsImplementor; }
			set { eventsImplementor = value; }
		}
		IPivotGridEventsImplementorBase EventsImpl {
			get {
				if(EventsImplementor != null)
					return EventsImplementor;
				return emptyEventsImplementor;
			}
		}
		IPivotGridEventsImplementorBase GetEventsImpl(bool forceEmpty) {
			if(forceEmpty)
				return emptyEventsImplementor;
			return EventsImpl;
		}
		protected object SerializableObject {
			get { return eventsImplementor; }
		}
		WeakEventHandler<EventArgs, LayoutChangedEventHandler> layoutChangedEvent;
		public event LayoutChangedEventHandler LayoutChangedEvent {
			add { layoutChangedEvent += value; }
			remove { layoutChangedEvent -= value; }
		}
		WeakEventHandler<FieldSizeChangedEventArgs, FieldSizeChangedEventHandler> fieldSizeChanged;
		public event FieldSizeChangedEventHandler FieldSizeChanged {
			add { fieldSizeChanged += value; }
			remove { fieldSizeChanged -= value; }
		}
		WeakEventHandler<FieldChangingEventArgs, FieldChangingEventHandler> fieldAreaChangingEvent;
		public event FieldChangingEventHandler FieldAreaChanging {
			add { fieldAreaChangingEvent += value; }
			remove { fieldAreaChangingEvent -= value; }
		}
		WeakEventHandler<FieldChangedEventArgs, EventHandler<FieldChangedEventArgs>> fieldAreaChangedEvent;
		public event EventHandler<FieldChangedEventArgs> FieldAreaChanged {
			add { fieldAreaChangedEvent += value; }
			remove { fieldAreaChangedEvent -= value; }
		}
		WeakEventHandler<FieldChangedEventArgs, EventHandler<FieldChangedEventArgs>> fieldVisibleChangedEvent;
		public event EventHandler<FieldChangedEventArgs> FieldVisibleChanged {
			add { fieldVisibleChangedEvent += value; }
			remove { fieldVisibleChangedEvent -= value; }
		}
		public virtual bool IsLoading { get { return false; } }
		public virtual void SetIsLoading(bool isLoading) { }
		public virtual bool IsControlReady { get { return true; } }
		public virtual bool CanRaiseListChanged { get { return !IsDeserializing && !IsLoading; } }
		public void RaiseInvalidOperationOnLockedUpdateException() {
			if(this.IsLockUpdate) {
				throw new InvalidOperationOnLockedUpdateException();
			}
		}
		public virtual void DoActionInMainThread(Action action) {
			action();
		}
		protected virtual PivotGridFieldCollectionBase CreateFieldCollection() {
			return new PivotGridFieldCollectionBase(this);
		}
		protected virtual PivotGridGroupCollection CreateGroupCollection() {
			return new PivotGridGroupCollection(this);
		}
		public virtual bool AllowHideFields { get { return OptionsCustomization.AllowHideFields != AllowHideFieldsType.Never; } }
		public virtual bool ShowCustomizationTree {
			get { return OptionsView.GroupFieldsInCustomizationWindow && (System.Linq.Enumerable.Any(FieldItems, item => !string.IsNullOrEmpty(item.DisplayFolder)) || IsOLAP); }
		}
#region Access for native data source
		internal int? GetCustomSortRowsAccess(int listSourceRow1, int listSourceRow2, object value1, object value2, PivotGridFieldBase field, PivotSortOrder sortOrder) {
			return GetCustomSortRows(listSourceRow1, listSourceRow2, value1, value2, field, sortOrder);
		}
		internal object GetUnboundValueAccess(PivotGridFieldBase field, int listSourceRowIndex, object expValue) {
			return GetUnboundValue(field, listSourceRowIndex, expValue);
		}
		internal void OnCalcCustomSummaryAccess(PivotGridFieldBase field, PivotCustomSummaryInfo customSummaryInfo) {
			OnCalcCustomSummary(field, customSummaryInfo);
		}
#endregion
		protected virtual int? GetCustomSortRows(int listSourceRow1, int listSourceRow2, object value1, object value2, PivotGridFieldBase field, PivotSortOrder sortOrder) {
			return EventsImpl.GetCustomSortRows(listSourceRow1, listSourceRow2, value1, value2, field, sortOrder);
		}
		protected virtual object GetUnboundValue(PivotGridFieldBase field, int listSourceRowIndex, object expValue) {
			return EventsImpl.GetUnboundValue(field, listSourceRowIndex, expValue);
		}
		public int GetFieldCountByArea(PivotArea area) {
			return GetFieldCountByArea(area, true);
		}
		public int GetFieldCountByArea(PivotArea area, bool useFieldCollections) {
			if(useFieldCollections)
				return GetFieldCollection(area).Count;
			else {
				int res = 0;
				for(int i = 0; i < Fields.Count; i++) {
					if(Fields[i].Area == area && Fields[i].Visible)
						res++;
				}
				return res;
			}
		}
		public int DataFieldCount {
			get { return GetFieldCollection(PivotArea.DataArea).Count; }
		}
		internal bool GetAllowFilterBySummary(PivotGridFieldBase field) {
			Guard.ArgumentNotNull(field, "field");
			return field.Area == PivotArea.DataArea && field.Visible && CellValuesProvider.GetLastCalculation(field) == null &&
				(field.SummaryType == PivotSummaryType.Count || (field.DataType != typeof(DateTime) && field.DataType != typeof(string) && field.DataType != typeof(bool))) &&
				(!field.IsUnbound || (field.IsUnbound && field.UnboundType == UnboundColumnType.Integer || field.UnboundType == UnboundColumnType.Decimal)) &&
				!HasRunningTotals;
		}
		public bool HasNonVariationSummary {
			get {
				for(int i = DataFieldCount - 1; i >= 0; i--) {
					PivotGridFieldBase field = GetFieldByArea(PivotArea.DataArea, i);
					if(field.SummaryDisplayType != PivotSummaryDisplayType.AbsoluteVariation &&
						field.SummaryDisplayType != PivotSummaryDisplayType.PercentVariation)
						return true;
				}
				return false;
			}
		}
		protected bool HasRunningTotals { get { return hasRunningTotals; } }
		protected internal virtual string GetCustomFieldValueText(PivotFieldValueItem item, string defaultText) {
			return GetEventsImpl(Disposing).FieldValueDisplayText(item, defaultText);
		}
		public virtual string GetCustomFilterFieldValueText(PivotGridFieldBase field, object value) {
			if(IsOLAP && field.ActualOLAPFilterByUniqueName) {
				IOLAPMember member = null;
				member = GetOLAPMemberByUniqueName(field.FieldName, value);
				if(member != null)
					return GetEventsImpl(Disposing).FieldValueDisplayText(field, member);
			}
			return GetEventsImpl(Disposing).FieldValueDisplayText(field, value);
		}
		public virtual string GetCustomFieldValueText(PivotGridFieldBase field, object value) {
			return GetEventsImpl(Disposing).FieldValueDisplayText(field, value);
		}
		protected internal virtual string GetCustomCellText(PivotGridCellItem item) {
			return EventsImpl.CustomCellDisplayText(item);
		}
		protected internal virtual object GetCustomCellValue(PivotGridCellItem item) {
			return EventsImpl.CustomCellValue(item);
		}
		public virtual object GetCustomChartDataSourceData(PivotChartItemType itemType, PivotChartItemDataMember itemDataMember, PivotFieldValueItem fieldValueItem, PivotGridCellItem cellItem, object value) {
			return EventsImpl.CustomChartDataSourceData(itemType, itemDataMember, fieldValueItem, cellItem, value);
		}
		public void RaiseCustomChartDataSourceRows(IList<PivotChartDataSourceRowBase> rows) {
			EventsImpl.CustomChartDataSourceRows(rows);
		}
		public virtual bool OnCustomFilterPopupItems(PivotGridFilterItems items) {
			return EventsImpl.CustomFilterPopupItems(items);
		}
		public virtual bool OnCustomFieldValueCells(PivotVisualItemsBase items) {
			return EventsImpl.CustomFieldValueCells(items);
		}
		public int ColumnFieldCount { get { return GetFieldCollection(PivotArea.ColumnArea).Count; } }
		public int RowFieldCount { get { return GetFieldCollection(PivotArea.RowArea).Count; } }
		public List<PivotGridFieldBase> GetFieldsByArea(PivotArea area, bool includeDataField) {
			return Fields.GetFieldsByArea(area, includeDataField);
		}
		public PivotGridFieldBase GetFieldByArea(PivotArea area, int index) {
			if(index < 0 || index >= GetFieldCollection(area).Count)
				return null;
			return GetFieldCollection(area)[index];
		}
		public PivotGridFieldBase GetLastFieldByArea(PivotArea area) {
			int count = GetFieldCountByArea(area);
			if(count > 0)
				return GetFieldByArea(area, count - 1);
			return null;
		}
		public PivotGridFieldBase GetLastFieldByArea(bool isColumn) {
			return GetLastFieldByArea(isColumn ? PivotArea.ColumnArea : PivotArea.RowArea);
		}
		public Type GetFieldTypeByArea(PivotArea area, int index) {
			return GetFieldType(GetFieldByArea(area, index));
		}
		public Type GetFieldType(PivotGridFieldBase field) {
			return GetFieldType(field, true);
		}
		public virtual Type GetFieldType(PivotGridFieldBase field, bool raw) {
			Type res = PivotDataSource.GetFieldType(field, raw);
			if(raw || res == null)
				return res;
			PivotCalculationBase calc = CellValuesProvider.GetLastCalculation(field);
			if(calc == null || calc is PivotRunningTotalCalculationBase)
				return res;
			return calc.FieldType;
		}
		public virtual bool IsFieldTypeCheckRequired(PivotGridFieldBase field) {
			return PivotDataSource.IsFieldTypeCheckRequired(field);
		}
		public PivotGridFieldBase GetFieldByLevel(bool isColumn, int level) {
			if(level < 0 || level >= GetFieldLevelCollection(isColumn).Count)
				return null;
			return GetFieldLevelCollection(isColumn)[level];
		}
		[Browsable(false)]
		public PivotGridFieldBase GetFieldByNameOrDataControllerColumnName(string name) {
			return this.Fields.GetFieldByNameOrDataControllerColumnName(name);
		}
		protected PivotGridFieldReadOnlyCollection GetFieldCollection(PivotArea area) {
			return FieldCollections[area];
		}
		public bool IsFieldCollectionsEmpty {
			get { return FieldCollections.IsEmpty; }
		}
		protected PivotGridFieldReadOnlyCollection GetFieldCollection(bool isColumn) {
			return GetFieldCollection(isColumn ? PivotArea.ColumnArea : PivotArea.RowArea);
		}
		protected internal PivotGridFieldReadOnlyCollection GetFieldLevelCollection(bool isColumn) {
			return FieldCollections.GetFieldLevelCollection(isColumn);
		}
		protected PivotGridFieldReadOnlyCollection GetFieldLevelCollection(PivotArea area) {
			return FieldCollections.GetFieldLevelCollection(area == PivotArea.ColumnArea);
		}
		public void ChangeFieldSortOrder(PivotGridFieldBase field) {
			field.ChangeSortOrder();
		}
		public void SetFieldSorting(PivotGridFieldBase field, PivotSortOrder sortOrder,
				PivotSortMode? actualSortMode, PivotSortMode? sortMode, bool reset) {
			field.SetSorting(sortOrder, actualSortMode, sortMode, reset);
		}
		public void SetFieldAreaPosition(PivotGridFieldBase field, PivotArea newArea, int newAreaIndex) {
			if(!DataField.Visible || field == DataField) {
				field.SetAreaPosition(newArea, newAreaIndex);
				return;
			}
			int correctedNewAreaIndex = CorrectNewAreaIndex(field, newArea, newAreaIndex);
			int newDataFieldIndex = GetNewDataFieldIndex(field, newAreaIndex, newArea);
			bool dataFieldAreaChanging = newDataFieldIndex != DataField.AreaIndex;
			if(!dataFieldAreaChanging)
				field.SetAreaPosition(newArea, correctedNewAreaIndex);
			else {
				BeginUpdate();
				PivotArea oldArea = field.Area;
				int oldAreaIndex = field.AreaIndex;
				bool oldVisible = field.Visible;
				field.SetAreaPosition(newArea, correctedNewAreaIndex);
				OptionsDataField.AreaIndexCore = newDataFieldIndex;
				EndUpdate();
				if(oldArea != field.Area)
					OnFieldAreaChanged(field);
				if(oldVisible != field.Visible)
					OnFieldVisibleChanged(field, oldAreaIndex, false);
				if(oldAreaIndex != field.AreaIndex)
					OnFieldAreaIndexChanged(field, false, false);
			}
		}
		int CorrectNewAreaIndex(PivotGridFieldBase field, PivotArea newArea, int newAreaIndex) {
			List<PivotGridFieldBase> fields = GetFieldsByArea(newArea, true);
			fields.Remove(field);
			if(newAreaIndex < 0)
				newAreaIndex = 0;
			if(newAreaIndex > fields.Count)
				newAreaIndex = fields.Count;
			fields.Insert(newAreaIndex, field);
			fields.Remove(DataField);
			return fields.IndexOf(field);
		}
		int GetNewDataFieldIndex(PivotGridFieldBase field, int newAreaIndex, PivotArea newArea) {
			List<PivotGridFieldBase> fields = GetFieldsByArea(DataField.Area, true);
			if(field.Area == DataField.Area)
				fields.Remove(field);
			if(newArea == DataField.Area)
				fields.Insert((newAreaIndex < fields.Count) ? newAreaIndex : fields.Count, field);
			return fields.IndexOf(DataField);
		}
		protected virtual PivotGridOptionsViewBase CreateOptionsView() { return new PivotGridOptionsViewBase(new EventHandler(OnOptionsViewChanged)); }
		protected virtual PivotGridOptionsCustomization CreateOptionsCustomization() { return new PivotGridOptionsCustomization(new EventHandler(OnOptionsChanged)); }
		protected virtual PivotGridOptionsDataField CreateOptionsDataField() { return new PivotGridOptionsDataField(this, new EventHandler(OnOptionsChanged)); }
		protected virtual PivotGridOptionsFilterBase CreateOptionsFilter() { return new PivotGridOptionsFilterBase(OnOptionsFilterChanged); }
		internal void FieldCollectionChanged() {
			InvalidateFieldItems();
		}
		protected void InvalidateFieldItems() {
			if(VisualItemsInternal != null)
				VisualItemsInternal.Invalidate();
		}
		protected virtual void OnOptionsViewChanged(object sender, EventArgs e) {
			InvalidateFieldItems();
			if(!IsLoading && !IsDeserializing)
				((PivotGridOptionsViewBase)sender).Validate();
			LayoutChanged();
		}
		public PivotGridOptionsViewBase OptionsView {
			get {
				if(optionsView == null) {
					optionsView = CreateOptionsView();
				}
				return optionsView;
			}
		}
		public PivotGridOptionsCustomization OptionsCustomization {
			get {
				if(optionsCustomization == null) {
					optionsCustomization = CreateOptionsCustomization();
				}
				return optionsCustomization;
			}
		}
		public PivotGridOptionsFilterBase OptionsFilter {
			get {
				if(optionsFilter == null) {
					optionsFilter = CreateOptionsFilter();
				}
				return optionsFilter;
			}
		}
		public PivotGridOptionsDataField OptionsDataField {
			get {
				if(optionsDataField == null) {
					optionsDataField = CreateOptionsDataField();
				}
				return optionsDataField;
			}
		}
#if !SL
		public virtual PivotGridOptionsPrint OptionsPrint {
			get {
				if(optionsPrint == null)
					optionsPrint = CreatePivotGridOptionsPrint();
				return optionsPrint;
			}
		}
		protected virtual PivotGridOptionsPrint CreatePivotGridOptionsPrint() {
			return new PivotGridOptionsPrint(OnOptionsPrintChanged);
		}
#endif
		protected virtual void OnOptionsPrintChanged(object sender, EventArgs e) {
			LayoutChanged();
		}
		public PivotGridOptionsChartDataSourceBase OptionsChartDataSource {
			get {
				if(optionsChartDataSource == null) {
					optionsChartDataSource = CreateOptionsChartDataSource();
					optionsChartDataSource.OptionsChanged += OnOptionsChartDataSourceChanged;
				}
				return optionsChartDataSource;
			}
		}
		protected void OnOptionsChartDataSourceChanged(object sender, EventArgs e) {
			ChartDataSourceInternal.InvalidateChartData();
		}
		protected virtual PivotGridOptionsChartDataSourceBase CreateOptionsChartDataSource() {
			return new PivotGridOptionsChartDataSourceBase(this);
		}
		public PivotGridOptionsData OptionsData {
			get {
				if(optionsData == null)
					optionsData = CreateOptionsData();
				return optionsData;
			}
		}
		protected virtual PivotGridOptionsData CreateOptionsData() {
			return new PivotGridOptionsData(this, new EventHandler(OnOptionsDataChanged));
		}
		protected virtual void OnOptionsDataChanged(object sender, EventArgs e) {
			InvalidateFieldItems();
			LayoutChanged();
		}
		public PivotGridOptionsSelection OptionsSelection {
			get {
				if(optionsSelection == null)
					optionsSelection = CreateOptionsSelection();
				return optionsSelection;
			}
		}
		protected virtual PivotGridOptionsSelection CreateOptionsSelection() {
			return new PivotGridOptionsSelection();
		}
		public PivotGridOptionsBehaviorBase OptionsBehavior {
			get {
				if(optionsBehavior == null)
					optionsBehavior = CreateOptionsBehavior();
				return optionsBehavior;
			}
		}
		protected virtual PivotGridOptionsBehaviorBase CreateOptionsBehavior() {
			return new PivotGridOptionsBehaviorBase(OnOptionsChanged);
		}
		public PivotGridOptionsOLAP OptionsOLAP {
			get {
				if(optionsOLAP == null)
					optionsOLAP = CreateOptionsOLAP();
				return optionsOLAP;
			}
		}
		protected virtual PivotGridOptionsOLAP CreateOptionsOLAP() {
			return new PivotGridOptionsOLAP(OnOptionsOLAPChanged);
		}
		public IPivotGridDataSource PivotDataSource {
			get { return pivotDataSource; }
			set { SetPivotDataSource(value, true); }
		}
		protected virtual void SetPivotDataSource(IPivotGridDataSource value, bool notifyDataSourceChanged) {
			if(pivotDataSource == value)
				return;
			IPivotGridDataSourceAsyncExpand asyncExpand = pivotDataSource as IPivotGridDataSourceAsyncExpand;
			if(pivotDataSource != null) {
				IPivotListDataSource listDataSource = pivotDataSource as IPivotListDataSource;
				if(listDataSource != null)
					listDataSource.ListSourceChanged -= OnListSourceChanged;
				pivotDataSource.DataChanged -= HandleDataChanged;
				pivotDataSource.LayoutChanged -= HandleLayoutChanged;
				if(asyncExpand != null)
					asyncExpand.ExpandValueDenied -= HandleExpandValueDenied;
				pivotDataSource.Dispose();
			}
			pivotDataSource = value;
			this.olapDataSource = pivotDataSource as IPivotOLAPDataSource;
			this.listDataSource = pivotDataSource as IPivotListDataSource;
			asyncExpand = pivotDataSource as IPivotGridDataSourceAsyncExpand;
			if(pivotDataSource != null) {
				pivotDataSource.Owner = this;
				pivotDataSource.DataChanged += HandleDataChanged;
				pivotDataSource.LayoutChanged += HandleLayoutChanged;
				if(asyncExpand != null)
					asyncExpand.ExpandValueDenied += HandleExpandValueDenied;
				pivotDataSource.AutoExpandGroups = OptionsData.AutoExpandGroupsInternal;
				pivotDataSource.CustomObjectConverter = OptionsData.CustomObjectConverter;
				if(ListDataSource != null) {
					ListDataSource.ListSourceChanged += OnListSourceChanged;
					ListDataSource.CaseSensitive = OptionsData.CaseSensitiveCore;
				}
				if(OLAPDataSource != null) {
					OLAPDataSource.OLAPGroupsChanged += OnOLAPGroupsChanged;
					OLAPDataSource.OLAPQueryTimeout += OnOLAPQueryTimeout;
				}
				IQueryDataSource queryDataSource = pivotDataSource as IQueryDataSource;
				if(queryDataSource != null)
					queryDataSource.QueryException += OnQueryException;
				CheckFields();
				pivotDataSource.OnInitialized();
			}
			if(notifyDataSourceChanged) {
				NotifyPivotDataSourcesChanged();
				OnDataSourceChanged();
			}
		}
		int listSourceChangedCounter;
		void ResetListSourceChangedCounter() {
			this.listSourceChangedCounter = 0;
		}
		bool GetIsListSourceChaged() {
			return this.listSourceChangedCounter > 0;
		}
		protected virtual void OnListSourceChanged(IPivotGridDataSource dataSource) {
			this.listSourceChangedCounter++;
			OnDataSourceChanged();
		}
		int lockEndRefreshCounter = -1;
		void LockEndRefresh() {
			lockEndRefreshCounter = 0;
		}
		void UnlockEndRefresh() {
			if(lockEndRefreshCounter > 0) {
				lockEndRefreshCounter = -1;
				OnAfterRefresh();
			} else {
				lockEndRefreshCounter = -1;
			}
		}
		void HandleDataChanged(object sender, PivotDataSourceEventArgs e) {
			OnDataChanged(e.PivotDataSource);
		}
		void HandleLayoutChanged(object sender, PivotDataSourceEventArgs e) {
			OnLayoutChanged(e.PivotDataSource);
		}
		void HandleExpandValueDenied(object sender, PivotDataSourceExpandValueDeniedEventArgs e) {
			PivotGridFieldBase field = GetFieldByArea(e.IsColumn ? PivotArea.ColumnArea : PivotArea.RowArea, e.Level);
			DenyExpandValue(field, e.VisibleIndex);
			OnGroupRowCollapsed();
		}
		protected virtual void OnDataChanged(IPivotGridDataSource dataSource) {
			DoRefresh();
		}
		protected virtual void OnLayoutChanged(IPivotGridDataSource dataSource) {
			LayoutChanged();
		}
		IPivotOLAPDataSource olapDataSource;
		public IPivotOLAPDataSource OLAPDataSource {
			get { return olapDataSource; }
		}
		public IQueryDataSource QueryDataSource {
			get { return pivotDataSource as IQueryDataSource; }
		}
#if !SL
		public DevExpress.PivotGrid.ServerMode.ServerModeDataSource ServerModeDataSource {
			get { return pivotDataSource as DevExpress.PivotGrid.ServerMode.ServerModeDataSource; }
		}
		OLAPDataProvider olapDataProvider = OLAPDataProvider.Default;
		public OLAPDataProvider OLAPDataProvider {
			get { return olapDataProvider; }
			set {
				if(olapDataProvider == value)
					return;
				olapDataProvider = value;
				if(string.IsNullOrEmpty(OLAPConnectionString))
					return;
				if(OLAPDataSource != null)
					DisconnectOLAP();
				string str = OLAPConnectionString;
				OLAPConnectionString = null;
				OLAPConnectionString = str;
			}
		}
#endif
		IPivotListDataSource listDataSource;
		public IPivotListDataSource ListDataSource {
			get { return listDataSource; }
		}
		public bool IsCapabilitySupported(PivotDataSourceCaps capability) {
			return (PivotDataSource.Capabilities & capability) == capability;
		}
		public void CheckField(PivotGridFieldBase field) {
			if(PivotDataSource == null || disposing)
				return;
			CheckFieldForUnbound(field);
			CheckFieldForOLAPMeasures(field);
		}
		void CheckFieldForUnbound(PivotGridFieldBase field) {
			if(field.IsUnbound && !IsCapabilitySupported(PivotDataSourceCaps.UnboundColumns) && !isDeserializing)
				throw new Exception("Current data source doesn't support unbound fields");
		}
		void CheckFieldForOLAPMeasures(PivotGridFieldBase field) {
			if(!(IsOLAP && field.IsOLAPMeasure && !OptionsOLAP.AllowDuplicatedMeasures))
				return;
			PivotGridFieldBase nField = Fields.GetFieldByFieldName(field.FieldName);
			if(nField != null && nField != field) {
				if(field.SummaryDisplayType == PivotSummaryDisplayType.Default)
					field.SummaryDisplayType = PivotSummaryDisplayType.AbsoluteVariation;
			}
		}
		void CheckFields() {
			if(disposing || PivotDataSource == null || Fields == null)
				return;
			for(int i = 0; i < Fields.Count; i++) {
				CheckField(Fields[i]);
			}
		}
		public int CompareValues(object val1, object val2) {
			if(ListDataSource != null)
				return ListDataSource.CompareValues(val1, val2);
			else
				return Comparer.DefaultInvariant.Compare(val1, val2);
		}
		public void BeginUpdate() {
			this.lockUpdateCount++;
			OnBeginUpdate();
		}
		public void CancelUpdate() {
			this.lockUpdateCount--;
		}
		public virtual void EndUpdate() {
			if(this.lockUpdateCount == 1 && IsOLAP)
				Fields.GroupFieldsByHierarchies();
			if(--this.lockUpdateCount == 0) {
				DoRefresh();
			}
			if(lockUpdateCount < 0) {
				lockUpdateCount = 0;
				throw new InvalidOperationException("Please call the BeginUpdate method before the EndUpdate method is called.");
			}
		}
		void OnBeginUpdate() { }
		public bool IsLockUpdate { get { return this.lockUpdateCount > 0; } }
		public void FireChanged() {
			FireChanged(null);
		}
		public void FireChanged(object obj) {
			if(obj == null) {
				FireChanged(null);
			} else {
				FireChanged(new object[] { obj });
			}
		}
		public virtual void FireChanged(object[] objs) { }
		public virtual void FocusedCellChanged(Point oldValue, Point newValue) {
			ChartDataSourceInternal.OnFocusedCellChanged();
		}
		public virtual void CellSelectionChanged() {
			ChartDataSourceInternal.OnCellSelectionChanged();
		}
		public virtual void MakeCellVisible(Point cell) { }
		public virtual void RetrieveFields() {
			RetrieveFields(PivotArea.FilterArea, true);
		}
		public virtual void RetrieveFields(PivotArea area, bool visible) {
			BeginUpdate();
			try {
				Fields.LockUpdateAreaIndexes();
				Fields.ClearAndDispose();
				PivotDataSource.RetrieveFields(area, visible);
				Fields.UnlockUpdateAreaIndexes();
			} finally {
				EndUpdate();
			}
		}
		protected virtual void RetrieveFieldCore(PivotArea area, string fieldName, string caption, string displayFolder, bool visible) {
			PivotGridFieldBase field = Fields.Add(fieldName, area);
			if(!string.IsNullOrEmpty(caption))
				field.Caption = caption;
			if(!string.IsNullOrEmpty(displayFolder))
				field.DisplayFolder = displayFolder;
			field.Visible = visible;
			field.Name = Fields.GenerateName(fieldName);
		}
		PivotCustomFieldListDelegate customFieldList;
		public event PivotCustomFieldListDelegate CustomFieldList {
			add { customFieldList += value; }
			remove { customFieldList -= value; }
		}
		public virtual string[] GetFieldList() {
			string[] result = PivotDataSource.GetFieldList();
			if(customFieldList != null)
				customFieldList(this, ref result);
			if(result != null)
				Array.Sort(result);
			return result;
		}
		public string GetFieldName(PivotGridFieldBase field) {
			switch(OptionsDataField.FieldNaming) {
				case DataFieldNaming.FieldName:
					if(!string.IsNullOrEmpty(field.FieldName))
						return field.FieldName;
					if(field.IsUnbound && !string.IsNullOrEmpty(field.UnboundFieldName))
						return field.UnboundFieldName;
					break;
				case DataFieldNaming.Name:
					if(!string.IsNullOrEmpty(field.Name))
						return field.Name;
					if(!string.IsNullOrEmpty(field.FieldName))
						return field.FieldName;
					break;
			}
			return PivotGridFieldBase.NamePrefix + field.Index;
		}
		public string GetDataControllerColumnName(PivotGridFieldBase field) {
			if(ListDataSource != null)
				return ListDataSource.GetFieldName(field);
			else
				return
#if !SL
					IsServerMode && !IsOLAP ? field.SameFieldNameCount > 1 || string.IsNullOrEmpty(field.FieldName) ? field.Name : field.FieldName : 
#endif
 field.FieldName;
		}
		public virtual void ReloadData() {
			ClearDenyExpandValues();
			PivotDataSource.ReloadData();
		}
		internal bool CanDoRefresh { get { return !IsLockUpdate && !IsLoading && !Disposing; } }
		internal protected virtual bool LockRefresh { get { return false; } }
		public virtual void UpdateDataSources() {
			ResetPivotDataSources();
			RefreshPivotDataSources();
		}
		protected void NotifyPivotDataSourcesChanged() {
			if(DataSourceList == null || DataSourceList.Count == 0)
				return;
			for(int i = DataSourceList.Count - 1; i >= 0; i--) {
				PivotDataSource ds = (PivotDataSource)DataSourceList[i].Target;
				if(ds == null)
					DataSourceList.RemoveAt(i);
				else {
					ds.Clear(Disposing);
					if(!(ds.IsLiveWhenDataSourceChanged) || Disposing)
						DataSourceList.RemoveAt(i);
				}
			}
		}
		protected void ResetPivotDataSources() {
			if(DataSourceList == null)
				return;
			for(int i = DataSourceList.Count - 1; i >= 0; i--) {
				PivotDataSource ds = (PivotDataSource)DataSourceList[i].Target;
				if(ds == null)
					DataSourceList.RemoveAt(i);
				else {
					if(!ds.IsLive)
						DataSourceList.RemoveAt(i);
					else
						ds.ResetData();
				}
			}
		}
		protected void RefreshPivotDataSources() {
			if(DataSourceList == null)
				return;
			for(int i = DataSourceList.Count - 1; i >= 0; i--) {
				PivotDataSource ds = (PivotDataSource)DataSourceList[i].Target;
				if(ds == null)
					DataSourceList.RemoveAt(i);
				else
					ds.Refresh();
			}
		}
		public virtual void OnSortOrderChanged(PivotGridFieldBase field) {
			if(!CanDoRefresh || !field.IsColumnOrRow || !field.Visible)
				return;
			if(field.ActualSortMode == PivotSortMode.Custom || field.RunningTotal || HasRunningTotalFieldsHigherLevel(field)) {
				DoRefresh();
			} else {
				PivotDataSource.ChangeFieldSortOrder(field);
				LayoutChanged();
			}
		}
		internal void OnSummaryTypeChanged(PivotGridFieldBase field, PivotSummaryType oldSummaryType) {
			if(!CanDoRefresh || field.Area != PivotArea.DataArea)
				return;
			if(PivotDataSource.ChangeFieldSummaryType(field, oldSummaryType))
				LayoutChanged();
			else
				DoRefresh();
		}
		internal void OnCalculationsChanged(PivotGridFieldBase field) {
			if(field.Visible && field.Area == PivotArea.DataArea) {
				cellValuesProvider.Invalidate();
				LayoutChanged();
			}
		}
		internal void CorrectCalculations() {
			cellValuesProvider.Invalidate();
		}
		bool HasRunningTotalFieldsHigherLevel(PivotGridFieldBase field) {
			if(!field.IsColumnOrRow)
				return false;
			int fieldsCount = GetFieldCountByArea(field.Area);
			for(int i = field.AreaIndex + 1; i < fieldsCount; i++) {
				PivotGridFieldBase higherLevelField = GetFieldByArea(field.Area, i);
				if(higherLevelField.RunningTotal && higherLevelField.Visible)
					return true;
			}
			return false;
		}
		public virtual void OnSortModeChanged(PivotGridFieldBase field) {
			if(!field.IsColumnOrRow || !field.Visible)
				return;
			DoRefresh();
		}
		public virtual void OnFieldUnboundExpressionChanged(PivotGridFieldBase field) {
			FireChanged(field);
			GetEventsImpl(IsLoading).FieldUnboundExpressionChanged(field);
		}
		public void DisconnectOLAP() {
			if(IsOLAP)
				OLAPDataSource.Disconnect();
		}
		public void ConnectOLAP() {
			if(IsOLAP)
				OLAPDataSource.Connect();
		}
		public void EnsureOlapConnected() {
			if(IsOLAP)
				OLAPDataSource.EnsureConnected();
		}
		public string SaveQueryDataSourceColumns() {
			IQueryDataSource queryDataSource = QueryDataSource;
			if(queryDataSource == null)
				return string.Empty;
			return queryDataSource.SaveColumns();
		}
		public void RestoreQueryDataSourceColumns(string savedColumns) {
			IQueryDataSource queryDataSource = QueryDataSource;
			if(queryDataSource == null)
				return;
			BeginUpdate();
			queryDataSource.RestoreColumns(savedColumns);
			CancelUpdate();
		}
		public string SaveQueryDataSourceState() {
			IQueryDataSource queryDataSource = QueryDataSource;
			if(queryDataSource == null)
				return string.Empty;
			return queryDataSource.SaveFieldValuesAndCellsToString();
		}
		public void RestoreQueryDataSourceState(string savedState) {
			IQueryDataSource queryDataSource = QueryDataSource;
			if(queryDataSource == null)
				return;
			BeginUpdate();
			queryDataSource.RestoreFieldValuesAndCellsFromString(savedState);
			CancelUpdate();
		}
		public void DoRefresh() {
			if(CanDoRefresh && !isRefreshing) {
				ResetPivotDataSources();
				DoRefreshCore();
				RefreshPivotDataSources();
			}
		}
		bool isRefreshing;
		public bool IsRefreshing { get { return isRefreshing; } }
		protected virtual void DoRefreshCore() {
			isRefreshing = true;
			try {
				OnBeforeRefresh();
				if(PivotDataSource != null)
					PivotDataSource.DoRefresh();
				OnAfterRefresh();
			} finally {
				isRefreshing = false;
			}
		}
		protected void OnBeforeRefresh() {
			GetEventsImpl(Disposing).BeginRefresh();
			ClearDenyExpandValues();
			ClearCaches();
			Fields.UpdateAreaIndexes();
			Prefilter.UpdateState(Fields);
		}
		protected virtual void RaiseEndRefresh() {
			EventsImpl.EndRefresh();
		}
		protected void OnAfterRefresh() {
			OptionsDataField.CheckArea();
			if(!Disposing) {
				if(lockEndRefreshCounter < 0)
					RaiseEndRefresh();
				else
					lockEndRefreshCounter++;
			}
		}
		public PivotGridFieldReadOnlyCollection GetSortedFields(bool updateCollections) {
			PivotGridFieldReadOnlyCollection sortedFields = new PivotGridFieldReadOnlyCollection(Fields);
			sortedFields.Sort();
			if(updateCollections) {
				AddFieldsIntoFieldCollections(sortedFields);
				UpdateHasRunningTotals(sortedFields);
			}
			return sortedFields;
		}
		public PivotGridFieldReadOnlyCollection GetSortedFields() {
			return GetSortedFields(true);
		}
		void UpdateHasRunningTotals(PivotGridFieldReadOnlyCollection sortedFields) {
			hasRunningTotals = false;
			for(int i = 0; i < sortedFields.Count; i++) {
				if(sortedFields[i].RunningTotal) {
					hasRunningTotals = true;
					break;
				}
			}
		}
		public void EnsureFieldCollections() {
			if(IsFieldCollectionsEmpty)
				GetSortedFields();
		}
		void ClearFieldCollections() {
			FieldCollections.Clear();
			InvalidateFieldItems();
		}
		void AddFieldsIntoFieldCollections(PivotGridFieldReadOnlyCollection sortedFields) {
			ClearFieldCollections();
			foreach(PivotGridFieldBase field in sortedFields) {
				if(!field.Visible)
					continue;
				FieldCollections[field.Area].Add(field);
				if(field.IsColumnOrRow) {
					GetFieldLevelCollection(field.Area == PivotArea.ColumnArea).Add(field);
				}
			}
			if(OptionsDataField.DataFieldsLocationArea != PivotDataArea.None) {
				int index = DataField.Visible ? DataField.AreaIndex : -1;
				GetFieldLevelCollection(OptionsDataField.DataFieldsLocationArea == PivotDataArea.ColumnArea).Insert(index, DataField);
			} else {
				if(GetFieldLevelCollection(true).Count == 0) {
					GetFieldLevelCollection(true).Add(DataField);
				}
			}
		}
		public PivotGridFieldBase GetFieldByPivotColumnInfo(PivotColumnInfo columnInfo) {
			return columnInfo != null ? columnInfo.Tag as PivotGridFieldBase : null;
		}
		public XmlXtraSerializer CreateXmlXtraSerializer() {
			XmlXtraSerializer serializer = new XmlXtraSerializer();
			PrepareXtraSerializer(serializer);
			return serializer;
		}
#if !SL
		public Base64XtraSerializer CreateBase64XtraSerializer() {
			Base64XtraSerializer serializer = new Base64XtraSerializer();
			PrepareXtraSerializer(serializer);
			return serializer;
		}
		public PivotXtraSerializer CreatePivotXtraSerializer() {
			PivotXtraSerializer serializer = new PivotXtraSerializer();
			PrepareXtraSerializer(serializer);
			return serializer;
		}
#endif
		void PrepareXtraSerializer(XtraSerializer serializer) {
			serializer.CustomObjectConverter = OptionsData.CustomObjectConverter;
		}
		public virtual void SaveCollapsedStateToStream(Stream stream) {
			PivotDataSource.SaveCollapsedStateToStream(stream);
		}
		public virtual void WebSaveCollapsedStateToStream(Stream stream) {
			PivotDataSource.WebSaveCollapsedStateToStream(stream);
		}
		public void SaveDataToStream(Stream stream, bool compress) {
			PivotDataSource.SaveDataToStream(stream, compress);
		}
		public virtual string AppName { get { return "PivotGrid"; } }
		public void SavePivotGridToFile(string path, bool compress) {
			using(FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write)) {
				SavePivotGridToStream(stream, compress);
			}
		}
		public void SavePivotGridToStream(Stream stream, bool compress) {
			SaveDataToStream(stream, compress);
			SaveFieldsToStream(stream);
			SaveCollapsedStateToStream(stream);
		}
		protected void SaveFieldsToStream(Stream stream) {
			BinaryWriter writer = new BinaryWriter(stream);
			long startPosition = stream.Position;
			writer.Write(0L);
			SaveFieldsToStreamCore(stream);
			long endPosition = stream.Position;
			stream.Position = startPosition;
			writer.Write(endPosition);
			stream.Position = endPosition;
		}
		public virtual void SaveFieldsToStreamCore(Stream stream) {
			CreateXmlXtraSerializer().SerializeObject(SerializableObject, stream, AppName, OptionsLayoutBase.FullLayout);
		}
		public virtual void LoadFieldsFromStreamCore(MemoryStream layoutStream) {
			CreateXmlXtraSerializer().DeserializeObject(SerializableObject, layoutStream, AppName, OptionsLayoutBase.FullLayout);
		}
		public void SaveFilterValuesToStream(TypedBinaryWriter writer) {
			if(!IsDataBound)
				return;
			for(int i = 0; i < Fields.Count; i++)
				Fields[i].FilterValues.SaveToStream(writer, Fields[i].ActualOLAPFilterByUniqueName ? typeof(string) : GetFieldType(Fields[i]));
		}
		public void SaveGroupFilterValuesToStream(TypedBinaryWriter writer) {
			if(!IsDataBound)
				return;
			foreach(PivotGridGroup group in Groups)
				group.FilterValues.SaveToStream(writer);
		}
		public string SaveFilterValuesIsEmptyState() {
			return PivotGridSerializeHelper.ToBase64String(writer => {
				foreach(PivotGridFieldBase field in Fields)
					writer.WriteObject(field.FilterValues.SavedIsEmpty);
			}, null);
		}
		public bool RestoreFilterValuesIsEmptyState(string value) {
			if(string.IsNullOrEmpty(value))
				return false;
			using(MemoryStream stream = new MemoryStream(Convert.FromBase64String(value))) {
				TypedBinaryReader reader = new TypedBinaryReader(stream);
				foreach(PivotGridFieldBase field in Fields) {
					field.FilterValues.SavedIsEmpty = (bool?)reader.ReadObject(typeof(bool?));
				}
				return true;
			}
		}
		public void LoadFilterValuesFromStream(Stream stream) {
			TypedBinaryReader reader = TypedBinaryReader.CreateReader(stream, OptionsData.CustomObjectConverter);
			for(int i = 0; i < Fields.Count; i++) {
				Fields[i].FilterValues.LoadFromStream(reader);
			}
		}
		public void LoadGroupFilterValuesFromStream(Stream stream) {
			TypedBinaryReader reader = TypedBinaryReader.CreateReader(stream, OptionsData.CustomObjectConverter);
			foreach(PivotGridGroup group in Groups) {
				group.FilterValues.LoadFromStream(reader);
			}
		}
		public void LoadCollapsedStateFromStream(Stream stream) {
			PivotDataSource.LoadCollapsedStateFromStream(stream);
			ClearCaches();
		}
		public void WebLoadCollapsedStateFromStream(Stream stream) {
			PivotDataSource.WebLoadCollapsedStateFromStream(stream);
			ClearCaches();
		}
		public virtual void LayoutChanged() {
			if(CanDoRefresh)
				LayoutChangedCore();
		}
		protected virtual void LayoutChangedCore() {
			bool isChartInvalid = ChartDataSourceInternal != null ? ChartDataSourceInternal.InvalidateChartData(false) : false;
			LayoutChangedInternal();
			Invalidate();
			if(isChartInvalid)
				RaiseChartDataSourceListChanged();
			UpdateDataSources();
		}
		protected virtual void RaiseChartDataSourceListChanged() {
			ChartDataSourceInternal.RaiseListChanged();
		}
		public virtual void Invalidate() {
		}
		protected virtual void LayoutChangedInternal() {
			ClearCaches();
			layoutChangedEvent.SafeRaise(this, EventArgs.Empty);
			GetEventsImpl(Disposing).LayoutChanged();
		}
		protected virtual void ClearCaches() {
			cellValuesProvider.Invalidate();
			if(visualItems != null)
				visualItems.Clear();
			isDataFieldsVisible = null;
		}
		public virtual object GetCustomGroupInterval(PivotGridFieldBase field, object value) {
			return GetEventsImpl(Disposing).CustomGroupInterval(field, value);
		}
		public object GetListSourceRowValue(int listSourceRow, string fieldName) {
			if(ListDataSource != null)
				return ListDataSource.GetListSourceRowValue(listSourceRow, fieldName);
			return null;
		}
		public PivotDrillDownDataSource GetDrillDownDataSource(int columnIndex, int rowIndex, int dataIndex) {
			return GetDrillDownDataSource(columnIndex, rowIndex, dataIndex, OptionsData.DrillDownMaxRowCount);
		}
		public PivotDrillDownDataSource GetDrillDownDataSource(int columnIndex, int rowIndex, int dataIndex, int maxRowCount) {
			if(maxRowCount < 0)
				maxRowCount = OptionsData.DrillDownMaxRowCount;
			PivotDrillDownDataSource ds = PivotDataSource.GetDrillDownDataSource(columnIndex, rowIndex, dataIndex, maxRowCount);
			if(ds != null)
				DataSourceList.Add(new WeakReference(ds));
			return ds;
		}
		public PivotDrillDownDataSource GetDrillDownDataSource(GroupRowInfo groupRow, VisibleListSourceRowCollection visibleListSourceRows) {
			if(ListDataSource != null) {
				PivotDrillDownDataSource ds = ListDataSource.GetDrillDownDataSource(groupRow, visibleListSourceRows);
				if(ds != null)
					DataSourceList.Add(new WeakReference(ds));
				return ds;
			} else
				return null;
		}
		public PivotDrillDownDataSource CreateDrillDownDataSource(int columnIndex, int rowIndex) {
			return CreateDrillDownDataSource(columnIndex, rowIndex, -1);
		}
		public PivotDrillDownDataSource CreateDrillDownDataSource(int columnIndex, int rowIndex, int maxRowCount) {
			return VisualItems.CreateDrillDownDataSource(columnIndex, rowIndex, maxRowCount);
		}
		[Obsolete("This method is now obsolete. Use the CreateQueryModeDrillDownDataSource method instead.")]
		public PivotDrillDownDataSource CreateOLAPDrillDownDataSource(int columnIndex, int rowIndex, int maxRowCount, List<string> customColumns) {
			return CreateQueryModeDrillDownDataSource(columnIndex, rowIndex, maxRowCount, customColumns);
		}
		public PivotDrillDownDataSource CreateQueryModeDrillDownDataSource(int columnIndex, int rowIndex, int maxRowCount, List<string> customColumns) {
			return VisualItems.CreateQueryModeDrillDownDataSource(columnIndex, rowIndex, maxRowCount, customColumns);
		}
		[Obsolete("This method is now obsolete. Use the CreateQueryModeDrillDownDataSource method instead.")]
		public virtual PivotDrillDownDataSource GetOLAPDrillDownDataSource(int columnIndex, int rowIndex, int dataIndex, int maxRowCount, List<string> customColumns) {
			return GetQueryModeDrillDownDataSource(columnIndex, rowIndex, dataIndex, maxRowCount, customColumns);
		}
		public virtual PivotDrillDownDataSource GetQueryModeDrillDownDataSource(int columnIndex, int rowIndex, int dataIndex, int maxRowCount, List<string> customColumns) {
			if(QueryDataSource == null)
				return null;
			if(maxRowCount < 0)
				maxRowCount = OptionsData.DrillDownMaxRowCount;
			PivotDrillDownDataSource ds = QueryDataSource.GetDrillDownDataSource(columnIndex, rowIndex, dataIndex, maxRowCount, customColumns);
			if(ds != null)
				DataSourceList.Add(new WeakReference(ds));
			return ds;
		}
		public PivotSummaryDataSource CreateSummaryDataSource(int columnIndex, int rowIndex) {
			PivotSummaryDataSource ds = CreateSummaryDataSourceCore(columnIndex, rowIndex);
			if(ds != null)
				DataSourceList.Add(new WeakReference(ds));
			return ds;
		}
		protected virtual PivotSummaryDataSource CreateSummaryDataSourceCore(int columnIndex, int rowIndex) {
			return new PivotSummaryDataSource(this, columnIndex, rowIndex);
		}
		public PivotSummaryDataSource CreateSummaryDataSource() {
			return CreateSummaryDataSource(-1, -1);
		}
		public Rectangle GetCellChildrenBounds(int columnIndex, int rowIndex) {
			if(columnIndex < -1 || columnIndex >= ColumnCellCount ||
				rowIndex < -1 || rowIndex >= RowCellCount ||
				(ColumnCellCount == 0 && RowCellCount == 0))
				return Rectangle.Empty;
			Rectangle result = new Rectangle();
			if(columnIndex == -1) {
				result.X = ColumnCellCount == 0 ? -1 : 0;
				result.Width = ColumnCellCount == 0 ? 1 : ColumnCellCount;
			} else {
				int nextIndex = PivotDataSource.GetNextOrPrevVisibleIndex(true, columnIndex, true);
				if(nextIndex < 0)
					nextIndex = ColumnCellCount;
				result.X = columnIndex;
				result.Width = nextIndex - columnIndex;
			}
			if(rowIndex == -1) {
				result.Y = RowCellCount == 0 ? -1 : 0;
				result.Height = RowCellCount == 0 ? 1 : RowCellCount;
			} else {
				int nextIndex = PivotDataSource.GetNextOrPrevVisibleIndex(false, rowIndex, true);
				if(nextIndex < 0)
					nextIndex = RowCellCount;
				result.Y = rowIndex;
				result.Height = nextIndex - rowIndex;
			}
			return result;
		}
		public List<Point> GetCellChildren(int columnIndex, int rowIndex) {
			int maxColumnLevel = GetMaxObjectLevel(true),
				maxRowLevel = GetMaxObjectLevel(false);
			List<Point> result = new List<Point>();
			Rectangle bounds = GetCellChildrenBounds(columnIndex, rowIndex);
			for(int y = bounds.Top; y < bounds.Bottom; y++)
				for(int x = bounds.Left; x < bounds.Right; x++)
					if(IsCellFit(x, y, maxColumnLevel, maxRowLevel)) {
						result.Add(new Point(x, y));
					}
			return result;
		}
		protected bool IsCellFit(int x, int y, int maxColumnLevel, int maxRowLevel) {
			return IsIndexFit(true, x, maxColumnLevel) && IsIndexFit(false, y, maxRowLevel)
					&& !IsSummaryEmpty(x, y);
		}
		bool IsIndexFit(bool isColumn, int index, int maxLevel) {
			int level = GetObjectLevel(isColumn, index);
			if(level == maxLevel)
				return true;
			int nextObjLevel = GetObjectLevel(isColumn, index + 1);
			return nextObjLevel == level || nextObjLevel < level ||
				index == GetCellCount(isColumn) - 1;
		}
		bool IsSummaryEmpty(int columnIndex, int rowIndex) {
			for(int i = 0; i < GetFieldCountByArea(PivotArea.DataArea); i++)
				if(GetCellValue(columnIndex, rowIndex, i) != null)
					return false;
			return true;
		}
		bool IsDataField(PivotGridFieldBase field) {
			return field != null && field.Area == PivotArea.DataArea && field.Visible;
		}
		public PivotSummaryValue GetCellSummaryValue(int columnIndex, int rowIndex, PivotGridFieldBase field) {
			if(field == null || field.Area != PivotArea.DataArea || !field.Visible)
				return null;
			if(ListDataSource != null)
				return ListDataSource.GetCellSummaryValue(columnIndex, rowIndex, field.AreaIndex);
			return null;
		}
		public object GetCellValue(object[] columnValues, object[] rowValues, PivotGridFieldBase field) {
			if(!IsDataField(field))
				return null;
			int columnIndex = PivotDataSource.GetVisibleIndexByValues(true, columnValues);
			if(columnIndex < 0 && columnValues != null && columnValues.Length > 0)
				return null;
			int rowIndex = PivotDataSource.GetVisibleIndexByValues(false, rowValues);
			if(rowIndex < 0 && rowValues != null && rowValues.Length > 0)
				return null;
			return GetCellValue(columnIndex, rowIndex, field.AreaIndex);
		}
		public object GetCellValue(int columnIndex, int rowIndex, PivotGridFieldBase field) {
			if(!IsDataField(field))
				return null;
			return GetCellValue(columnIndex, rowIndex, field.AreaIndex);
		}
		public object GetCellValue(int columnIndex, int rowIndex, int dataIndex) {
			PivotGridFieldBase dataField = GetFieldByArea(PivotArea.DataArea, dataIndex);
			if(dataField == null)
				return null;
			return GetCellValue(columnIndex, rowIndex, dataIndex, dataField.SummaryType);
		}
		public object GetCellValue(int columnIndex, int rowIndex, int dataIndex, PivotSummaryType summaryType) {
			return PivotCellValue.GetValue(GetCellValueEx(columnIndex, rowIndex, dataIndex, summaryType));
		}
		public PivotCellValue GetCellValueEx(int columnIndex, int rowIndex, int dataIndex, PivotSummaryType summaryType) {
			return GetCellValue(new PivotDataCoord(columnIndex, rowIndex, dataIndex, summaryType));
		}
		PivotCellValue GetCellValue(PivotDataCoord coord) {
			return ICellValueProvider.GetCellValue(coord);
		}
		int GetFieldIndex(PivotGridFieldBase field, int columnRowIndex) {
			if(!field.IsColumnOrRow)
				throw new Exception("This method can be called for the Column and Row fields only");
			int level = field.AreaIndex;
			bool isColumn = field.Area == PivotArea.ColumnArea;
			for(int i = columnRowIndex; i >= 0; i--) {
				int curLevel = GetObjectLevel(isColumn, i);
				if(curLevel == level)
					return i;
				if(curLevel < level)
					return -1;
			}
			return -1;
		}
		public object GetNextOrPrevRowCellValue(int columnIndex, int rowIndex, PivotGridFieldBase field, bool isNext) {
			PivotDataCoord coord = new PivotDataCoord(columnIndex, rowIndex, field.AreaIndex, field.SummaryType);
			return GetNextOrPrevCellValue(coord, isNext, false);
		}
		public object GetNextOrPrevColumnCellValue(int columnIndex, int rowIndex, PivotGridFieldBase field, bool isNext) {
			PivotDataCoord coord = new PivotDataCoord(columnIndex, rowIndex, field.AreaIndex, field.SummaryType);
			return GetNextOrPrevCellValue(coord, isNext, true);
		}
		object GetNextOrPrevCellValue(PivotDataCoord coord, bool isNext, bool isColumnIndex) {
			int nextCol = coord.Col, nextRow = coord.Row;
			if(isColumnIndex)
				nextCol = PivotDataSource.GetNextOrPrevVisibleIndex(true, coord.Col, isNext);
			else
				nextRow = PivotDataSource.GetNextOrPrevVisibleIndex(false, coord.Row, isNext);
			if((isColumnIndex && nextCol < 0) || (!isColumnIndex && nextRow < 0))
				return null;
			return PivotCellValue.GetValue(GetCellValue(new PivotDataCoord(nextCol, nextRow, coord.Data, coord.Summary)));
		}
		public int ColumnCellCount { get { return GetCellCount(true); } }
		public int RowCellCount { get { return GetCellCount(false); } }
		public int GetCellCount(bool isColumn) { return PivotDataSource.GetCellCount(isColumn); }
		public int GetLevelCount(bool isColumn) {
			return IsServerMode ?
				QueryDataSource.GetLevelCount(isColumn) :
						GetFieldCountByArea(isColumn ? PivotArea.ColumnArea : PivotArea.RowArea);
		}
		public int GetObjectLevel(bool byColumn, int visibleIndex) {
			return DataSourceObjectLevelHelper.GetObjectLevel(byColumn, visibleIndex);
		}
		public int GetColumnLevel(int columnIndex) {
			return DataSourceObjectLevelHelper.GetColumnLevel(columnIndex);
		}
		public int GetParentIndex(bool byColumn, int visibleIndex) {
			return DataSourceObjectLevelHelper.GetParentIndex(byColumn, visibleIndex);
		}
		public int GetPrevIndex(bool byColumn, int currentIndex) {
			return DataSourceObjectLevelHelper.GetPrevIndex(byColumn, currentIndex);
		}
		internal int GetPrevIndex(bool byColumn, int currentIndex, bool stopOnParent) {
			return DataSourceObjectLevelHelper.GetPrevIndex(byColumn, currentIndex, stopOnParent);
		}
		public int GetMaxObjectLevel(bool isColumn) {
			return DataSourceObjectLevelHelper.GetMaxObjectLevel(isColumn);
		}
		public int GetRowLevel(int rowIndex) {
			return DataSourceObjectLevelHelper.GetRowLevel(rowIndex);
		}
		public bool IsColumnCollapsed(int columnIndex) {
			return IsObjectCollapsed(true, columnIndex);
		}
		public bool IsRowCollapsed(int rowIndex) {
			return IsObjectCollapsed(false, rowIndex);
		}
		public bool IsObjectCollapsed(bool isColumn, int index) {
			return PivotDataSource.IsObjectCollapsed(isColumn, index);
		}
		public bool IsObjectCollapsed(bool isColumn, object[] values) {
			return PivotDataSource.IsObjectCollapsed(isColumn, values);
		}
		public int GetFieldValueIndex(bool isColumn, object[] values) {
			return VisualItems.GetLastLevelIndex(isColumn, values);
		}
		public int GetFieldValueIndex(bool isColumn, object[] values, PivotGridFieldBase dataField) {
			return VisualItems.GetLastLevelIndex(isColumn, values, GetFieldItem(dataField));
		}
		public bool ChangeExpanded(bool isColumn, int visibleIndex, bool expanded) {
			if(!PivotDataSource.ChangeExpanded(isColumn, visibleIndex, expanded)) {
				int level = GetObjectLevel(isColumn, visibleIndex);
				PivotGridFieldBase field = level >= 0 && level < GetLevelCount(isColumn) ? GetFieldByLevel(isColumn, level) : null;
				AfterFieldValueChangeNotExpanded(null, field);
				DenyExpandValue(field, visibleIndex);
				OnGroupRowCollapsed();
				return false;
			} else {
				OnGroupRowCollapsed();
				return true;
			}
		}
		protected bool ChangeExpandedCore(bool isColumn, int visibleIndex, bool expanded) {
			return PivotDataSource.ChangeExpanded(isColumn, visibleIndex, expanded);
		}
		public bool ChangeExpanded(bool isColumn, object[] values) {
			int visibleIndex = PivotDataSource.GetVisibleIndexByValues(isColumn, values);
			if(visibleIndex < 0)
				return false;
			bool collapsed = IsObjectCollapsed(isColumn, visibleIndex);
			return ChangeExpanded(isColumn, visibleIndex, collapsed);
		}
		public virtual bool ChangeExpanded(bool isColumn, object[] values, bool expanded) {
			int visibleIndex = PivotDataSource.GetVisibleIndexByValues(isColumn, values);
			if(visibleIndex < 0)
				return false;
			return ChangeExpanded(isColumn, visibleIndex, expanded);
		}
		public bool ChangeExpandedByUniqueValues(bool isColumn, object[] uniqueValues, bool expanded) {
			if(!IsOLAP)
				return false;
			int visibleIndex = OLAPDataSource.GetVisibleIndexByUniqueValues(isColumn, uniqueValues);
			if(visibleIndex < 0)
				return false;
			return ChangeExpanded(isColumn, visibleIndex, expanded);
		}
		public virtual void ChangeExpandedAll(bool expanded) {
			if(!ChangeExpandedAllCore(true, expanded) || !ChangeExpandedAllCore(false, expanded))
				AfterFieldValueChangeNotExpanded(null, null);
			OnGroupRowCollapsed();
		}
		public virtual void ChangeExpandedAll(bool isColumn, bool expanded) {
			if(!ChangeExpandedAllCore(isColumn, expanded))
				AfterFieldValueChangeNotExpanded(null, null);
			else
				OnGroupRowCollapsed();
		}
		protected bool ChangeExpandedAllCore(bool isColumn, bool expanded) {
			return PivotDataSource.ChangeExpandedAll(isColumn, expanded);
		}
		public virtual void ChangeFieldExpanded(PivotGridFieldBase field, bool expanded) {
			if(CanNotChangeExpanded(field))
				return;
			bool value = !PivotDataSource.ChangeFieldExpanded(field, expanded);
			if(IsOLAP && expanded)
				foreach(int index in OLAPDataSource.GetFieldNotExpandedIndexes(field))
					DenyExpandValues.DenyExpandValue(field, index);
			if(value)
				AfterFieldValueChangeNotExpanded(null, field);
			else
				OnGroupRowCollapsed();
		}
		public virtual void ChangeFieldExpanded(PivotGridFieldBase field, bool expanded, object value) {
			if(CanNotChangeExpanded(field))
				return;
			if(!PivotDataSource.ChangeFieldExpanded(field, expanded, value))
				AfterFieldValueChangeNotExpanded(null, field);
			else
				OnGroupRowCollapsed();
		}
		bool CanNotChangeExpanded(PivotGridFieldBase field) {
			return field == null || (!IsServerMode && field.ColumnHandle < 0) || !field.IsColumnOrRow || (field.AreaIndex >= GetLevelCount(field.IsColumn) - 1);
		}
		public bool IsFieldValueExpanded(PivotGridFieldBase field, int columnIndex, int rowIndex) {
			if(!field.IsColumnOrRow)
				return false;
			bool isColumn = field.Area == PivotArea.ColumnArea;
			int columnRowIndex = GetFieldIndex(field, isColumn ? columnIndex : rowIndex);
			return !IsObjectCollapsed(isColumn, columnRowIndex);
		}
		public bool IsAreaAllowed(PivotGridFieldBase field, PivotArea area) {
			return PivotDataSource.IsAreaAllowed(field, area);
		}
		public bool IsUnboundExpressionValid(PivotGridFieldBase field) {
			return PivotDataSource.IsUnboundExpressionValid(field);
		}
		public string GetKPIName(string fieldName, PivotKPIType kpiType) {
			return IsOLAP ? OLAPDataSource.GetKPIName(fieldName, kpiType) : null;
		}
		public PivotKPIGraphic GetKPIGraphic(PivotGridFieldBase field) {
			if(field.KPIGraphic == PivotKPIGraphic.ServerDefined)
				return IsOLAP && !field.IsDataField ? OLAPDataSource.GetKPIGraphic(field.FieldName) : PivotKPIGraphic.None;
			return field.KPIGraphic;
		}
		public PivotKPIType GetKPIType(PivotGridFieldBase field) {
			if(!IsOLAP)
				return PivotKPIType.None;
			return OLAPDataSource.GetKPIType(field.FieldName);
		}
		public static string GetKPITooltipText(PivotKPIType type, int value) {
			switch(type) {
				case PivotKPIType.Status:
					return GetStatusTooltipText(value);
				case PivotKPIType.Trend:
					return GetTrendTooltipText(value);
			}
			return string.Empty;
		}
		public List<string> GetOLAPKPIList() {
			if(!IsOLAP)
				return null;
			return OLAPDataSource.GetKPIList();
		}
		public PivotOLAPKPIMeasures GetOLAPKPIMeasures(string kpiName) {
			if(!IsOLAP)
				return null;
			return OLAPDataSource.GetKPIMeasures(kpiName);
		}
		public PivotOLAPKPIValue GetOLAPKPIValue(string kpiName) {
			if(!IsOLAP)
				return null;
			return OLAPDataSource.GetKPIValue(kpiName);
		}
		public PivotKPIGraphic GetOLAPKPIServerGraphic(string kpiName, PivotKPIType kpiType) {
			if(!IsOLAP)
				return PivotKPIGraphic.None;
			return OLAPDataSource.GetKPIServerDefinedGraphic(kpiName, kpiType);
		}
		public string GetMeasureServerDefinedFormatString(PivotGridFieldBase field) {
			if(!IsOLAP)
				return null;
			return OLAPDataSource.GetMeasureServerDefinedFormatString(field.FieldName);
		}
		public IOLAPMember GetOLAPMember(PivotGridFieldBase field, int visibleIndex) {
			if(!field.IsColumnOrRow)
				return null;
			return GetOLAPMember(field.IsColumn, visibleIndex);
		}
		public IOLAPMember GetOLAPMember(bool isColumn, int visibleIndex) {
			if(!IsOLAP)
				return null;
			return OLAPDataSource.GetMember(isColumn, visibleIndex);
		}
		public IOLAPMember GetOLAPMemberByValue(string fieldName, object value) {
			if(!IsOLAP)
				return null;
			return OLAPDataSource.GetMemberByValue(fieldName, value);
		}
		public IOLAPMember GetOLAPMemberByUniqueName(string fieldName, object value) {
			if(!IsOLAP)
				return null;
			return OLAPDataSource.GetMemberByUniqueName(fieldName, value);
		}
		public IOLAPMember[] GetOLAPColumnMembers(string fieldName) {
			if(!IsOLAP)
				return null;
			return OLAPDataSource.GetUniqueMembers(fieldName);
		}
		public Dictionary<string, DevExpress.PivotGrid.OLAP.OLAPDataType> GetOlapMemberProperties(string fieldName) {
			if(olapDataSource == null)
				return null;
			return olapDataSource.GetProperties(fieldName);
		}
		public string GetOlapDefaultSortProperty(string fieldName) {
			if(olapDataSource == null)
				return null;
			return olapDataSource.GetDefaultSortProperty(fieldName);
		}
		public string GetOLAPDrillDownColumnName(string fieldName) {
			if(!IsOLAP)
				return null;
			return OLAPDataSource.GetDrillDownColumnName(fieldName);
		}
		public OLAPDataSourceBase CreateOLAPDataSourceClone() {
			ICloneable cloneable = OLAPDataSource as ICloneable;
			return cloneable == null ? null : cloneable.Clone() as OLAPDataSourceBase;
		}
		public bool GetOlapIsUserHierarchy(string hierarchyName) {
			if(!IsOLAP)
				return false;
			return OLAPDataSource.GetOlapIsUserHierarchy(hierarchyName);
		}
		protected static string GetStatusTooltipText(int state) {
			switch(state) {
				case -1:
					return PivotGridLocalizer.GetString(PivotGridStringId.StatusBad);
				case 0:
					return PivotGridLocalizer.GetString(PivotGridStringId.StatusNeutral);
				case 1:
					return PivotGridLocalizer.GetString(PivotGridStringId.StatusGood);
			}
			throw new ArgumentException("Invalid state");
		}
		protected static string GetTrendTooltipText(int state) {
			switch(state) {
				case -1:
					return PivotGridLocalizer.GetString(PivotGridStringId.TrendGoingDown);
				case 0:
					return PivotGridLocalizer.GetString(PivotGridStringId.TrendNoChange);
				case 1:
					return PivotGridLocalizer.GetString(PivotGridStringId.TrendGoingUp);
			}
			throw new ArgumentException("Invalid state");
		}
		public object GetColumnValue(int columnIndex) {
			return GetObjectValue(true, columnIndex);
		}
		public object GetRowValue(int rowIndex) {
			return GetObjectValue(false, rowIndex);
		}
		public object GetObjectValue(bool isColumn, int index) {
			return PivotDataSource.GetFieldValue(isColumn, index, GetObjectLevel(isColumn, index));
		}
		public bool GetIsOthersValue(bool isColumn, int visibleIndex) {
			int levelIndex = GetObjectLevel(isColumn, visibleIndex);
			return PivotDataSource.GetIsOthersFieldValue(isColumn, visibleIndex, levelIndex);
		}
		public bool GetIsOthersValue(bool isColumn, int visibleIndex, int levelIndex) {
			return PivotDataSource.GetIsOthersFieldValue(isColumn, visibleIndex, levelIndex);
		}
		public bool GetIsOthersValue(PivotGridFieldBase field, int columnIndex, int rowIndex) {
			if(!field.IsColumnOrRow)
				return false;
			return field.Area == PivotArea.ColumnArea ? GetIsOthersValue(true, columnIndex, field.AreaIndex) : GetIsOthersValue(false, rowIndex, field.AreaIndex);
		}
		Nullable<bool> isDataFieldsVisible;
		public bool GetIsDataFieldsVisible(bool isCoumn) {
			if(!GetIsDataLocatedInThisArea(isCoumn) || DataFieldCount <= 1)
				return false;
			if(!isDataFieldsVisible.HasValue)
				isDataFieldsVisible = GetIsDataFieldsVisibleCore();
			return isDataFieldsVisible.Value;
		}
		protected bool GetIsDataFieldsVisibleCore() {
			int foundFields = 0;
			for(int i = 0; i < GetFieldCountByArea(PivotArea.DataArea); i++) {
				PivotGridFieldBase field = GetFieldByArea(PivotArea.DataArea, i);
				foreach(PivotGridValueType valueType in Helpers.GetEnumValues(typeof(PivotGridValueType))) {
					if(!field.CanShowValueType(valueType))
						continue;
					bool alreadyFound = ((foundFields >> (int)valueType) & 1) == 1;
					if(alreadyFound)
						return true;
					else
						foundFields |= 1 << (int)valueType;
				}
			}
			return false;
		}
		public bool GetIsDataLocatedInThisArea(bool isColumn) {
			if(isColumn)
				return OptionsDataField.DataFieldArea == PivotArea.ColumnArea;
			else
				return OptionsDataField.DataFieldArea == PivotArea.RowArea;
		}
		internal bool ShouldRemoveGrandTotalHeader(bool isColumn) {
			PivotArea area = isColumn ? PivotArea.ColumnArea : PivotArea.RowArea;
			return (OptionsDataField.DataFieldArea == area) && (DataFieldCount > 1) &&
						ShouldRemoveAnyAreaGrandTotalHeader(isColumn);
		}
		bool ShouldRemoveAnyAreaGrandTotalHeader(bool isColumn) {
			return (!OptionsView.ShowColumnGrandTotalHeader && isColumn) ||
						(!OptionsView.ShowRowGrandTotalHeader && !isColumn);
		}
		public object GetFieldValue(PivotGridFieldBase field, int columnIndex, int rowIndex) {
			if(!field.IsColumnOrRow)
				return null;
			return PivotDataSource.GetFieldValue(field.Area == PivotArea.ColumnArea, field.Area == PivotArea.ColumnArea ? columnIndex : rowIndex, field.AreaIndex);
		}
		public string GetHierarchyCaption(string columnName) {
			if(string.IsNullOrEmpty(columnName))
				return null;
			return pivotDataSource.GetLocalizedFieldCaption(columnName);
		}
		public string GetHierarchyName(string dimensionName) {
			if(OLAPDataSource != null)
				return OLAPDataSource.GetHierarchyName(dimensionName);
			return null;
		}
		public DefaultBoolean GetFieldIsAggregatable(string dimensionName) {
			if(IsOLAP)
				return OLAPDataSource.GetColumnIsAggregatable(dimensionName);
			return DefaultBoolean.True;
		}
		public string GetHierarchyDisplayFolder(string columnName) {
			if(IsServerMode && !string.IsNullOrEmpty(columnName))
				return QueryDataSource.GetColumnDisplayFolder(columnName);
			return string.Empty;
		}
		public virtual object[] GetUniqueFieldValues(PivotGridFieldBase field) {
			if(field == null)
				return new object[0];
			return PivotDataSource.GetUniqueFieldValues(field);
		}
		public virtual object[] GetSortedUniqueValues(PivotGridFieldBase field) {
			if(field == null)
				return new object[0];
			return PivotDataSource.GetSortedUniqueValues(field);
		}
		public virtual List<object> GetVisibleFieldValues(PivotGridFieldBase field) {
			if(field == null)
				return new List<object>();
			return PivotDataSource.GetVisibleFieldValues(field);
		}
		public object[] GetAvailableFieldValues(PivotGridFieldBase field) {
			return GetAvailableFieldValues(field, false);
		}
		public object[] GetAvailableFieldValues(PivotGridFieldBase field, bool deferUpdates) {
			if(field == null)
				return new object[0];
			return PivotDataSource.GetAvailableFieldValues(field, deferUpdates, GetCustomizationFormFields());
		}
		public virtual List<object> GetSortedUniqueGroupValues(PivotGridGroup group, object[] parentValues) {
			if(group == null)
				return null;
			return PivotDataSource.GetSortedUniqueGroupValues(group, parentValues);
		}
		public virtual bool GetIsEmptyGroupFilter(PivotGridGroup group) {
			if(group == null || !IsDataBound)
				return true;
			return PivotDataSource.GetIsEmptyGroupFilter(group);
		}
		public int GetFieldHierarchyLevel(PivotGridFieldBase field) {
			if(!IsOLAP)
				return -1;
			return OLAPDataSource.GetFieldHierarchyLevel(field.FieldName);
		}
		public bool? IsGroupFilterValueChecked(PivotGridGroup group, object[] parentValues, object value) {
			if(group == null)
				return false;
			return PivotDataSource.IsGroupFilterValueChecked(group, parentValues, value);
		}
		public PivotSummaryInterval GetSummaryInterval(PivotGridFieldBase dataField, bool visibleValuesOnly,
				bool customLevel, PivotGridFieldBase rowField, PivotGridFieldBase columnField) {
			if(dataField == null || (dataField.Area != PivotArea.DataArea) || !dataField.CanApplySummaryFilter || (customLevel && rowField == null && columnField == null)) {
				return PivotSummaryInterval.Empty;
			}
			return PivotDataSource.GetSummaryInterval(dataField, visibleValuesOnly, customLevel, rowField, columnField);
		}
		public bool HasNullValues(PivotGridFieldBase field) {
			if(field == null)
				return false;
			return PivotDataSource.HasNullValues(field);
		}
		public List<PivotGridFieldSortCondition> GetFieldSortConditions(bool isColumn, int visibleIndex) {
			List<PivotGridFieldSortCondition> res = new List<PivotGridFieldSortCondition>();
			if(visibleIndex < 0 || visibleIndex >= GetCellCount(isColumn))
				return res;
			res.Add(GetFieldSortCondition(isColumn, visibleIndex));
			int parentIndex = GetParentIndex(isColumn, visibleIndex);
			while(parentIndex >= 0) {
				res.Add(GetFieldSortCondition(isColumn, parentIndex));
				parentIndex = GetParentIndex(isColumn, parentIndex);
			}
			return res;
		}
		protected PivotGridFieldSortCondition GetFieldSortCondition(bool isColumn, int visibleIndex) {
			int level = GetObjectLevel(isColumn, visibleIndex);
			PivotGridFieldBase field = GetFieldByArea(isColumn ? PivotArea.ColumnArea : PivotArea.RowArea, level);
			object value = IsOLAP ? null : GetObjectValue(isColumn, visibleIndex);
			string uniqueName = null;
			if(IsOLAP)
				uniqueName = GetOLAPMember(isColumn, visibleIndex).UniqueName;
			return new PivotGridFieldSortCondition(field, value, uniqueName);
		}
		public bool IsFieldSortedBySummary(PivotGridFieldBase field, PivotGridFieldBase dataField, List<PivotGridFieldSortCondition> itemConditions) {
			if(!CheckDataField(field.SortBySummaryInfo.Field, field.SortBySummaryInfo.FieldName, dataField))
				return false;
			if(itemConditions == null)
				return true;
			if(itemConditions.Count != field.SortBySummaryInfo.Conditions.Count)
				return false;
			for(int i = 0; i < itemConditions.Count; i++) {
				PivotGridFieldSortCondition condition = field.SortBySummaryInfo.Conditions[itemConditions[i].Field];
				if(condition == null || !condition.IsEqual(itemConditions[i], CompareValues, IsOLAP))
					return false;
			}
			return true;
		}
		bool CheckDataField(PivotGridFieldBase actual, string actualFieldName, PivotGridFieldBase expected) {
			return (expected != null && actual == expected) ||
				(expected != null && actual == null && actualFieldName == expected.FieldName && !string.IsNullOrEmpty(actualFieldName)) ||
				(expected == null && actual != null && actual.Visible && actual.Area == PivotArea.DataArea);
		}
		public virtual void OnGroupFilteringChanged(PivotGridGroup group) {
			DoRefresh();
			AfterGroupFilteringChanged(group);
		}
		protected virtual void AfterGroupFilteringChanged(PivotGridGroup group) {
			GetEventsImpl(IsLoading).GroupFilterChanged(group);
		}
		public virtual bool OnFieldAreaChanging(PivotGridFieldBase field, PivotArea newArea, int newAreaIndex) {
			if(!field.CanChangeLocationTo(newArea, newAreaIndex))
				return false;
			bool result = GetEventsImpl(Disposing).FieldAreaChanging(field, newArea, newAreaIndex);
			if(!result)
				return result;
			if(!Disposing)
				return RaiseFieldAreaChanging(field, newArea, newAreaIndex);
			return result;
		}
		public virtual void OnFieldAreaChanged(PivotGridFieldBase field) {
			FireChanged(field);
			GetEventsImpl(IsLoading).FieldAreaChanged(field);
			if(!IsLoading)
				RaiseFieldAreaChanged(field);
		}
		public virtual void OnFieldVisibleChanged(PivotGridFieldBase field, int oldAreaIndex, bool doRefresh) {
			if(IsLockUpdate)
				return;
			if(doRefresh) {
				if((field.Area != PivotArea.DataArea || field.Visible || oldAreaIndex < 0) &&
					(field.Area != PivotArea.FilterArea || IsOLAP || field.NeedApplyFilterOnShowInFilterArea)) {
					if(!field.IsUnbound) {
						DoRefresh();
					} else {
						ReloadData();
					}
				} else {
					OnBeforeRefresh();
					if(field.Area == PivotArea.DataArea && PivotDataSource != null)
						HideDataField(field, oldAreaIndex);
					GetSortedFields();
					LayoutChanged();
					OnAfterRefresh();
				}
			}
			FireChanged(field);
			GetEventsImpl(IsLoading).FieldAreaChanged(field);
			GetEventsImpl(IsLoading).FieldVisibleChanged(field);
			if(!IsLoading)
				RaiseFieldVisibleChanged(field);
		}
		internal void HideDataField(PivotGridFieldBase field, int oldAreaIndex) {
			if(field.Group != null && field.Group.Fields.IndexOf(field) == 0)
				for(int i = field.Group.Fields.Count - 1; i >= 0; i--)
					PivotDataSource.HideDataField(field.Group[i], oldAreaIndex + i);
			else
				PivotDataSource.HideDataField(field, oldAreaIndex);
		}
		internal void MoveDataField(PivotGridFieldBase field, int oldAreaIndex, int areaIndex) {
			if(field.Group != null && field.Group.Fields.IndexOf(field) == 0)
				for(int i = 0; i < field.Group.Fields.Count; i++)
					PivotDataSource.MoveDataField(field.Group[i], oldAreaIndex, areaIndex + i + (i == 0 || areaIndex <= oldAreaIndex ? 0 : -1));
			else
				PivotDataSource.MoveDataField(field, oldAreaIndex, areaIndex);
		}
		public virtual bool OnFieldFilteringChanging(PivotGridFieldBase field, PivotFilterType filterType, bool showBlanks, IList<object> values) {
			return GetEventsImpl(Disposing).FieldFilterChanging(field, filterType, showBlanks, values);
		}
		public virtual void OnFieldFilteringChanged(PivotGridFieldBase field) {
			BeforeFieldFilteringChanged(field);
			DoRefresh();
			AfterFieldFilteringChanged(field);
		}
		protected virtual void BeforeFieldFilteringChanged(PivotGridFieldBase field) {
			field.OnFilteredValueChanged();
		}
		protected virtual void AfterFieldFilteringChanged(PivotGridFieldBase field) {
			GetEventsImpl(IsLoading).FieldFilterChanged(field);
		}
		public virtual void OnFieldSizeChanged(PivotGridFieldBase field, bool widthChanged, bool heightChanged) {
			FireChanged(field);
			GetEventsImpl(IsLoading).FieldWidthChanged(field);
			GetEventsImpl(IsLoading).LayoutChanged();
			RaiseFieldSizeChanged(field, widthChanged, heightChanged);
		}
		public virtual void OnFieldExpandedInFieldsGroupChanged(PivotGridFieldBase field) {
			FireChanged(field);
			GetEventsImpl(IsLoading).FieldExpandedInFieldsGroupChanged(field);
		}
		public virtual void OnFieldAreaIndexChanged(PivotGridFieldBase field, bool doRefresh, bool doLayoutChanged) {
			if(!field.Visible)
				return;
			if(field.Area != PivotArea.FilterArea && doRefresh)
				DoRefresh();
			else if(doLayoutChanged)
				LayoutChanged();
			FireChanged(field);
			GetEventsImpl(IsLoading).FieldAreaChanged(field);
			GetEventsImpl(IsLoading).FieldAreaIndexChanged(field);
		}
		public virtual void OnFieldPropertyChanged(PivotGridFieldBase field, PivotFieldPropertyName propertyName) {
			EventsImpl.FieldPropertyChanged(field, propertyName);
		}
		public virtual void OnGroupsChanged() { }
		protected virtual void OnGroupRowCollapsed() {
			LayoutChanged();
		}
		public virtual void OnGroupsCleared() {
			OnClearedOrChangedCore();
		}
		public virtual void OnColumnsClear() {
			OnClearedOrChangedCore();
		}
		public virtual void OnColumnRemove(PivotGridFieldBase field) {
			OnClearedOrChangedCore();
		}
		void OnClearedOrChangedCore() {
			FieldCollectionChanged();
			DoRefresh();
		}
		public virtual void OnColumnInsert(PivotGridFieldBase field) {
			FieldCollectionChanged();
			if(field.IsUnbound || (field.IsComplex && !IsServerMode))
				ReloadData();
			else {
				if(!field.IsUnbound && !field.Visible && (string.IsNullOrEmpty(field.FieldName) || field.Area != PivotArea.FilterArea))
					LayoutChanged();
				else
					DoRefresh();
			}
		}
		internal void OnColumnNameChanged(PivotGridFieldBase field) {
			OnClearedOrChangedCore();
		}
		protected virtual void OnDataSourceChanged() {
			RaiseDataSourceChanged();
		}
		void RaiseDataSourceChanged() {
			EventsImpl.DataSourceChanged();
		}
		protected virtual void OnOLAPGroupsChanged(IPivotOLAPDataSource dataSource) {
			BeginUpdate();
			if(!DelayFieldsGroupingByHierarchies && dataSource.PopulateColumns())
				Fields.GroupFieldsByHierarchies();
			CancelUpdate();
		}
		protected virtual void OnOLAPQueryTimeout(IPivotOLAPDataSource dataSource) {
			EventsImpl.OLAPQueryTimeout();
		}
		protected virtual bool OnQueryException(IQueryDataSource dataSource, Exception ex) {
			return EventsImpl.QueryException(ex);
		}
		protected virtual int? QuerySorting(IQueryMemberProvider value0, IQueryMemberProvider value1, PivotGridFieldBase field, ICustomSortHelper helper) {
			return EventsImpl.QuerySorting(value0, value1, field, helper);
		}
		protected virtual void OnCalcCustomSummary(PivotGridFieldBase field, PivotCustomSummaryInfo customSummaryInfo) {
			EventsImpl.CalcCustomSummary(field, customSummaryInfo);
		}
		public virtual void OnDeserializationComplete() {
			FixFieldsOrder();
			Fields.OnGridDeserialized();
			FixAreaIndexes();
			ClearCaches();
			ClearFieldCollections();
		}
		void FixFieldsOrder() {
			for(int i = 0; i < Fields.Count; i++)
				if(Fields[i].IndexInternal < 0 || Fields[i].IndexInternal >= Fields.Count)
					return;
			BeginUpdate();
			List<PivotGridFieldBase> fields = new List<PivotGridFieldBase>(Fields.Count);
			foreach(PivotGridFieldBase field in Fields)
				fields.Add(field);
			fields.Sort(new FieldsInternalIndexComparer());
			Fields.Clear();
			for(int i = 0; i < fields.Count; i++)
				Fields.Add(fields[i]);
			CancelUpdate();
		}
		void FixAreaIndexes() {
			int dataFieldAreaIndex = OptionsDataField.AreaIndex;
			Fields.UpdateAreaIndexes();
			if(dataFieldAreaIndex != -1)
				OptionsDataField.AreaIndex = dataFieldAreaIndex;
		}
		protected void OnOptionsChanged(object sender, EventArgs e) {
			InvalidateFieldItems();
			LayoutChanged();
		}
		protected virtual void OnOptionsFilterChanged(object sender, PivotOptionsFilterEventArgs e) {
			InvalidateFieldItems();
			CustomizationFormFields.ClearDefereFilters(this);
			if(e.IsUpdateRequired) {
				DoRefresh();
			}
		}
		protected virtual void OnOptionsOLAPChanged(object sender, EventArgs e) {
			if(IsOLAP)
				DoRefresh();
		}
		public void ChangeExpanded(PivotFieldValueItem item) {
			ChangeExpanded(item, true);
		}
		public void ChangeExpanded(PivotFieldValueItem item, bool raiseEvents) {
			if(raiseEvents && !BeforeFieldValueChangeExpanded(item))
				return;
			if(ChangeExpandedCore(item.IsColumn, item.VisibleIndex, item.IsCollapsed)) {
				OnGroupRowCollapsed();
				if(raiseEvents)
					AfterFieldValueChangeExpanded(item);
			} else {
				SetExpandValueDenied(item);
			}
		}
		void SetExpandValueDenied(PivotFieldValueItem item) {
			DenyExpandValue(GetField(item.Field), item.VisibleIndex);
			OnGroupRowCollapsed();
			AfterFieldValueChangeNotExpanded(item, GetField(item.Field));
		}
		public virtual bool BeforeFieldValueChangeExpanded(PivotFieldValueItem item) {
			return EventsImpl.BeforeFieldValueChangeExpanded(item);
		}
		public virtual void AfterFieldValueChangeExpanded(PivotFieldValueItem item) {
			EventsImpl.AfterFieldValueChangeExpanded(item);
		}
		public virtual void AfterFieldValueChangeNotExpanded(PivotFieldValueItem item, PivotGridFieldBase field) {
			EventsImpl.AfterFieldValueChangeNotExpanded(item, field);
		}
		virtual protected internal void OnInternalProblem() { }
		protected virtual void RaiseFieldSizeChanged(PivotGridFieldBase field, bool widthChanged, bool heightChanged) {
			if(fieldSizeChanged != null)
				fieldSizeChanged.Raise(this, new FieldSizeChangedEventArgs(field, widthChanged, heightChanged));
		}
		protected virtual bool RaiseFieldAreaChanging(PivotGridFieldBase field, PivotArea newArea, int newAreaIndex) {
			if(fieldAreaChangingEvent != null) {
				FieldChangingEventArgs e = new FieldChangingEventArgs(field, newArea, newAreaIndex);
				fieldAreaChangingEvent.Raise(this, e);
				return !e.Cancel;
			} else
				return true;
		}
		protected virtual void RaiseFieldAreaChanged(PivotGridFieldBase field) {
			if(fieldAreaChangedEvent != null)
				fieldAreaChangedEvent.Raise(this, new FieldChangedEventArgs(field));
		}
		protected virtual void RaiseFieldVisibleChanged(PivotGridFieldBase field) {
			if(fieldVisibleChangedEvent != null)
				fieldVisibleChangedEvent.Raise(this, new FieldChangedEventArgs(field));
		}
		object headerImages, valueImages;
		public object HeaderImages {
			get { return headerImages; }
			set {
				if(HeaderImages == value)
					return;
				headerImages = value;
				LayoutChanged();
			}
		}
		public object ValueImages {
			get { return valueImages; }
			set {
				if(ValueImages == value)
					return;
				valueImages = value;
				LayoutChanged();
			}
		}
#if !SL && !DXPORTABLE
		public object GetDesignOLAPDataSourceObject() {
			if(!IsOLAP)
				throw new Exception("This method can't be used in non OLAP mode");
			return new OLAPDataSourceObject(OLAPDataSource.CubeName, GetFieldList());
		}
#endif
#region Deny Expand Empty Values
		PivotDenyExpandValues denyExpandValues;
		protected internal PivotDenyExpandValues DenyExpandValues {
			get {
				if(denyExpandValues == null)
					denyExpandValues = CreateDenyExpandValues();
				return denyExpandValues;
			}
		}
		protected internal virtual PivotDenyExpandValues CreateDenyExpandValues() {
			return new PivotDenyExpandValues(this);
		}
		public virtual void DenyExpandValue(PivotGridFieldBase field, int visibleIndex) {
			DenyExpandValues.DenyExpandValue(field, visibleIndex);
		}
		public bool CanExpandValue(PivotFieldValueItem item) {
			return DenyExpandValues.CanExpandValue(item);
		}
		protected virtual void ClearDenyExpandValues() {
			if(denyExpandValues != null)
				DenyExpandValues.ClearDenyExpandValues();
		}
#endregion
#region events
		public virtual void OnPopupMenuShowing(object e) { }
		public virtual void OnPopupMenuItemClick(object e) { }
#endregion
#region FieldsCustomization
		public virtual bool IsFieldCustomizationShowing { get { return false; } }
#endregion
#region IPrefilterOwnerBase Members
		void IPrefilterOwnerBase.CriteriaChanged() {
			PrefilterCriteriaChanged();
		}
		protected virtual void PrefilterCriteriaChanged() {
			Prefilter.PatchPrefilterColumnNames(Fields, true);
			DoRefresh();
			GetEventsImpl(Disposing).PrefilterCriteriaChanged();
		}
#endregion
#region IFilteredComponentBase Members
		CriteriaOperator IFilteredComponentBase.RowCriteria {
			get { return Prefilter.Criteria; }
			set {
				Prefilter.Criteria = value;
				Prefilter.Enabled = true;
			}
		}
		event EventHandler IFilteredComponentBase.PropertiesChanged { add { ; } remove { ; } }
		event EventHandler IFilteredComponentBase.RowFilterChanged { add { ; } remove { ; } }
#endregion
#region IPivotGridDataSourceOwner
		IPivotGridDataSource IPivotGridDataSourceOwner.DataSource { get { return PivotDataSource; } }
		bool IPivotGridDataSourceOwner.IsDesignMode { get { return IsDesignMode; } }
		void IPivotGridDataSourceOwner.CreateField(PivotArea area, string fieldName, string caption, string displayFolder, bool visible) {
			RetrieveFieldCore(area, fieldName, caption, displayFolder, visible);
		}
		PivotGridFieldReadOnlyCollection IPivotGridDataSourceOwner.GetSortedFields() {
			return GetSortedFields();
		}
		CriteriaOperator IPivotGridDataSourceOwner.PrefilterCriteria { get { return Prefilter.Criteria; } }
		int? IPivotGridDataSourceOwner.GetCustomFieldSort(IQueryMemberProvider value0, IQueryMemberProvider value1, PivotGridFieldBase field, ICustomSortHelper helper) {
			return QuerySorting(value0, value1, field, helper);
		}
		string IPivotGridDataSourceOwner.GetCustomFieldText(PivotGridFieldBase field, object value) {
			return GetCustomFieldValueText(field, value);
		}
		IList<AggregationLevel> IPivotGridDataSourceOwner.GetAggregations() {
			return GetAggregations(true);
		}
#endregion
		internal void CustomObjectConverterChanged() {
			PivotDataSource.CustomObjectConverter = OptionsData.CustomObjectConverter;
		}
#region IPivotGridFieldsProvider Members
		public PivotDataArea DataFieldArea {
			get { return OptionsDataField.Area; }
		}
#endregion
		public virtual DevExpress.XtraPivotGrid.Customization.CustomizationFormFields GetCustomizationFormFields() {
			return new DevExpress.XtraPivotGrid.Customization.CustomizationFormFields(this) { DeferUpdates = true };
		}
#if !SL && !DXPORTABLE
		public Bitmap GetKPIBitmap(PivotKPIGraphic graphic, int state) {
			if(state != 0 && state != -1 && state != 1)
				throw new ArgumentException("state");
			if(graphic == PivotKPIGraphic.None || graphic == PivotKPIGraphic.ServerDefined)
				throw new ArgumentException("graphic");
			return DevExpress.Utils.ResourceImageHelper.CreateBitmapFromResources(
							PivotGridData.PivotGridImagesResourcePath + graphic.ToString() + "." + state.ToString() + ".png",
							typeof(PivotGridData).Assembly);
		}
#endif
		public string GetFilterCriteriaFieldName(PivotGridFieldBase field) {
			if(OLAPDataSource != null)
				return field.Name;
#if !SL
			if(ServerModeDataSource != null)
				return ((DevExpress.PivotGrid.ServerMode.IServerModeHelpersOwner)ServerModeDataSource).CubeColumns.GetFieldCubeColumnsName(field);
#endif
			return string.IsNullOrEmpty(field.Name) ? field.FieldName : field.Name;
		}
		[Browsable(false)]
		public virtual IList<AggregationLevel> GetAggregations(bool datasourceLevel) {
			return datasourceLevel ? this.aggregations : new AggregationLevel[] { };
		}
		[Browsable(false)]
		public void SetAggregations(IList<AggregationLevel> aggregations) {
			this.aggregations = aggregations;
		}
		internal void OnFieldSelectedAtDesignTimeChanged() {
			foreach(PivotGridFieldBase field in Fields) {
				PivotFieldItemBase fi = GetFieldItem(field);
				if(fi != null)
					new PivotGridFieldUISelectedAtDesignTimeAction(fi, this).SetSelectedAtDesignTimeAction(field.SelectedAtDesignTime);
			}
		}
	}
	static class PivotGridDataExtensions {
		public static bool IsForceNullInCriteria(this PivotGridData data) {
			return data != null && data.IsServerMode && !data.IsOLAP;
		}
	}
	public delegate void PivotCustomFieldListDelegate(PivotGridData data, ref string[] fieldList);
	public interface IPivotClipboardAccessor {
		void SetDataObject(string clipbloarObject);
	}
	class PivotClipboardEmptyAccessor : IPivotClipboardAccessor {
		public void SetDataObject(string selectionString) { }
	}
}
