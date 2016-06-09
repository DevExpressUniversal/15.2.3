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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Data;
using DevExpress.Data.Helpers;
using DevExpress.Data.IO;
using DevExpress.Web.Internal;
using DevExpress.Web;
using DevExpress.XtraGrid;
using System.Linq;
using DevExpress.Data.Filtering;
namespace DevExpress.Web.Data {
	public enum WebRowType { Data, Group }
	public delegate void WebDataProxyOwnerInvoker();
	public class WebDataProxy {
		public const int NewItemRow = ListSourceDataController.NewItemRow;
		public const char MultipleKeyFieldSeparator = ';';
		public const char MultipleKeyValueSeparator = '|';
		public const string MultipleKeyValueEncodedSeparator = "^DXKS^";
		public const string NullKeyStringRepresentation = "/^DXN";
		IWebControlPageSettings pageSettings, printerPageSettings;
		IWebDataOwner owner;
		IWebDataEvents events;
		WebDataProviderBase dataProvider;
		List<string> keyFieldNames = new List<string>();
		Dictionary<string, Type> keyFieldTypes;
		object editingKeyValue;
		object focusedRowKeyValue;
		bool isNewRowEditing, isServerMode;
		Dictionary<string, object> editorValues;
		bool editorValuesParsed;
		WebDataSelection selection;
		WebDataDetailRows detailRows;
		IList totalSummary, groupSummary;
		ASPxGroupSummarySortInfoCollection groupSummarySortInfo;
		int focusedRowIndexInternal = Int32.MinValue;
		int lockPageFocusChange = 0;
		WebDataProxyOwnerInvoker ownerDataBinding;
		bool pageSizeShowAllItem = false;
		public WebDataProxy(IWebDataOwner owner, IWebControlPageSettings pageSettings, IWebDataEvents events) {
			this.owner = owner;
			this.pageSettings = pageSettings;
			this.events = events;
			this.dataProvider = new WebDataControllerProvider(this);
			this.selection = Owner.CreateSelection(this);
			this.detailRows = new WebDataDetailRows(this);
			this.totalSummary = Owner.CreateSummaryItemCollection();
			this.groupSummary = Owner.CreateSummaryItemCollection();
			this.groupSummarySortInfo = new ASPxGroupSummarySortInfoCollection(Owner.WebControl);
			if(Owner != null) Owner.PageIndexChanged += new EventHandler(OnPageIndexChanged);
			this.editorValuesParsed = false;
		}
		public WebDataSelection Selection { get { return selection; } }
		public WebDataDetailRows DetailRows { get { return detailRows; } }
		public ASPxGroupSummarySortInfoCollection GroupSummarySortInfo { get { return groupSummarySortInfo; } }
		public IList TotalSummary { get { return totalSummary; } }
		public IList GroupSummary { get { return groupSummary; } }
		public IList FormatConditionSummary { get { return Owner.GetFormatConditionSummaries(); } }
		public bool IsServerMode {
			get {
				if(IsBound) return DataProvider.ServerMode;
				return isServerMode;
			}
		}
		public WebDataProxyOwnerInvoker OwnerDataBinding { get { return ownerDataBinding; } set { ownerDataBinding = value; } }
		protected internal void DoOwnerDataBinding() {
			if(IsBound) return;
			if(OwnerDataBinding != null) {
				OwnerDataBinding();
			}
		}
		protected internal virtual IWebDataOwner Owner { get { return owner; } }
		protected internal virtual IWebControlPageSettings PageSettings {
			get { return PrinterPageSettings != null ? PrinterPageSettings : pageSettings; } 
		}
		public IWebControlPageSettings PrinterPageSettings {
			get { return printerPageSettings; }
			set {
				if(printerPageSettings == value)
					return;
				printerPageSettings = value;
				Owner.WebControl.LayoutChanged();
			}
		}
		protected internal GridEndlessPagingHelper EndlessPagingHelper { get { return Owner.EndlessPagingHelper; } }
		protected internal bool UseEndlessPaging { get { return EndlessPagingHelper != null; } }
		protected bool IsEndlessPagingPartialLoad { get { return UseEndlessPaging && EndlessPagingHelper.PartialLoad; } }
		protected GridBatchEditHelper BatchEditHelper { get { return Owner.BatchEditHelper; } }
		protected bool AllowBatchEditing { get { return BatchEditHelper != null; } }
		protected bool IsDesignTime { get { return Owner != null ? Owner.IsDesignTime : false; } }
		protected internal virtual IWebDataEvents Events { 
			get {
				if(events == null) events = new WebDataNullEvents();
				return events; 
			} 
		}
		internal void SetEvents(IWebDataEvents events) { this.events = events; } 
		protected WebDataProviderBase DataProvider {  get { return dataProvider; } }
		protected void SetDataProvider(WebDataProviderBase value, bool withLoadData) {
			WebDataProviderBase old = DataProvider;
			this.dataProvider = value;
			if(old != null) this.dataProvider.Assign(old, withLoadData);
		}
		public void SetDataSource(object dataSource) {
			IEnumerable enumerable = dataSource as IEnumerable;
			if(enumerable != null && !(dataSource is IList))
				dataSource = DataUtils.ConvertEnumerableToList(enumerable);
			HintGridIsPagedCore(dataSource);
			CreateWebDataControllerProvider();
			DataProvider.SetDataSource(dataSource);
			DataProvider.ValidateSelectedKeys();
			Selection.UpdateCachedParameters();
			ResetEditingRowVisibleIndex();
		}
		protected internal void HintGridIsPaged() {
			if(!IsBound || !IsServerMode) 
				return;
			var serverModeDataController = BoundDataProvider.DataController as ServerModeDataControllerBase;
			if(serverModeDataController == null) return;
			HintGridIsPagedCore(serverModeDataController.DataSource);
		}
		protected void HintGridIsPagedCore(object dataSource) {
			var hints = dataSource as IListServerHints;
			if(hints != null)
				hints.HintGridIsPaged(PageSize);
		}
		object savedDataSource = null;
		protected internal bool PrepareDataSourceForExport() {
			if(!IsServerMode || this.savedDataSource != null)
				return false;
			this.savedDataSource = BoundDataProvider.DataSource;
			SetDataSource(BoundDataProvider.DataController.GetAllFilteredAndSortedRows());
			BoundDataProvider.DataController.ByPassFilter = true;
			return true;
		}
		protected internal bool RestoreDataSourceAfterExport() {
			if(this.savedDataSource == null)
				return false;
			SetDataSource(this.savedDataSource);
			this.savedDataSource = null;
			return true;
		}
		protected internal virtual DataSourceView GetData() { return Owner.GetData(); }
		public virtual int PageSize { get { return PageSettings.PageSize; } }
		public int PageIndex { get { return PageSettings.PageIndex; } set { PageSettings.PageIndex = value; } }
		public int PageCount {
			get {
				return GetPageCount(VisibleRowCount);
			}
		}
		public bool IsLastPage { get { return PageIndex == PageCount - 1; } }
		protected internal bool PageSizeShowAllItem {
			get { return pageSizeShowAllItem; }
			set { pageSizeShowAllItem = value; }
		}
		protected internal int GetPageCount(int visibleRowCount) {
			if(PageSize <= 0) return 1;
			if(PageSizeShowAllItem) return 1;
			int pageCount = visibleRowCount / PageSize;
			if(visibleRowCount % PageSize > 0) {
				pageCount++;
			}
			return pageCount;
		}
		protected internal int ListSourceRowCount {
			get { return DataProvider.ListSourceRowCount; }
		}
		public int VisibleRowCount {
			get { return DataProvider.VisibleCount; }
		}
		public virtual void SortGroupChanged(List<IWebColumnInfo> sortList, int groupCount, string filterExpression) {
			CreateWebDataControllerProvider();
			DataProvider.SortGroupChanged(sortList, groupCount, filterExpression, GroupSummary, TotalSummary, FormatConditionSummary);
			ResetEditingRowVisibleIndex();
		}
		public virtual object[] GetUniqueColumnValues(string fieldName, int maxCount, bool includeFilteredOut) {
			CreateWebDataControllerProvider();
			return DataProvider.GetUniqueColumnValues(fieldName, maxCount, includeFilteredOut);
		}
		public bool AllowSearchPanelFilter(DataColumnInfo column) {
			if(!IsBound)
				return true;
			return column != null && DataProvider.AllowSearchPanelFilter(column);
		}
		protected internal DataController GetDataController() {
			CreateWebDataControllerProvider();
			return BoundDataProvider.DataController;
		}
		protected WebDataControllerProvider BoundDataProvider { get { return DataProvider as WebDataControllerProvider; } }
		protected virtual void CreateWebDataControllerProvider() {
			if(Owner.IsForceDataSourcePaging) {
				WebRegularDataProvider provider = DataProvider as WebRegularDataProvider;
				if(provider == null)
					SetDataProvider(new WebRegularDataProvider(this), false);
				return;
			}
			if (BoundDataProvider == null)
				SetDataProvider(new WebDataControllerProvider(this), false);
		}
		public virtual int VisibleStartIndex {
			get {
				if(IsEndlessPagingPartialLoad)
					return EndlessPagingHelper.VisibleStartIndex;
				return VisibleStartIndexInternal;
			}
		}
		protected int VisibleStartIndexInternal {
			get {
				if(PageSettings.PagerMode == GridViewPagerMode.ShowAllRecords || PageIndex == -1)
					return 0;
				return Math.Min(PageIndex, PageCount) * PageSize;
			}
		}
		public virtual int VisibleRowCountOnPage { 
			get {
				if(IsEndlessPagingPartialLoad)
					return EndlessPagingHelper.VisibleRowCountOnPage;
				return VisibleRowCountOnPageInternal;
			}
		}
		protected int VisibleRowCountOnPageInternal {
			get {
				if(PageSettings.PagerMode == GridViewPagerMode.ShowAllRecords || PageIndex == -1)
					return VisibleRowCount;
				return Math.Min(VisibleRowCount - VisibleStartIndex, PageSize);
			}
		}
		public bool IsFiltered { get { return DataProvider.IsFiltered; } }
		public virtual string FilterExpression { get { return DataProvider.FilterExpression; }  }
		public bool IsReady { get { return DataProvider.IsReady; } }
		public bool IsBound { get { return DataProvider.IsBound; } }
		public object GetTotalSummaryValue(ASPxSummaryItemBase item) {
			return DataProvider.GetTotalSummaryValue(item);
		}
		public object GetFormatConditionSummaryValue(FormatConditionSummary item) {
			return DataProvider.GetFormatConditionSummaryValue(item);
		}
		public bool IsGroupSummaryExists(int visibleIndex, ASPxSummaryItem item) {
			return DataProvider.IsGroupSummaryExists(visibleIndex, item);
		}
		public object GetGroupSummaryValue(int visibleIndex, ASPxSummaryItem item) {
			return DataProvider.GetGroupSummaryValue(visibleIndex, item);
		}
		public bool IsExpressionFitToRule(string expression, int visibleIndex) {
			return DataProvider.IsExpressionFitToRule(expression, visibleIndex);
		} 
		public WebRowType GetRowType(int visibleIndex) {
			return DataProvider.GetRowType(visibleIndex);
		}
		public object GetRowValues(int visibleIndex, string[] fieldNames) {
			return GetRowValuesCore(visibleIndex, fieldNames, new GetRowValueMethod(GetRowValue));
		}
		public int GetChildRowCount(int groupRowVisibleIndex) {
			DoOwnerDataBinding();
			return BoundDataProvider.GetChildRowCount(groupRowVisibleIndex);
		}
		public object GetChildRow(int groupRowVisibleIndex, int childIndex) {
			DoOwnerDataBinding();
			return BoundDataProvider.GetChildRow(groupRowVisibleIndex, childIndex); ;
		}
		public object GetChildRowValues(int groupRowVisibleIndex, int childIndex, params string[] fieldNames) {
			DoOwnerDataBinding();
			return BoundDataProvider.GetChildRowValues(groupRowVisibleIndex, childIndex, fieldNames); ;
		}
		public IList GetChildKeysRecursive(int groupRowVisibleIndex) {
			DoOwnerDataBinding();
			return BoundDataProvider.GetChildKeysRecursive(groupRowVisibleIndex);
		}
		public object GetGroupRowValue(int visibleIndex) {
			var groupInfo = GetGroupRowInfo(visibleIndex);
			return groupInfo != null ? BoundDataProvider.DataController.GetGroupRowValue(groupInfo) : null;
		}
		public GroupRowInfo GetGroupRowInfo(int visibleIndex) {
			DoOwnerDataBinding();
			if(GetRowType(visibleIndex) == WebRowType.Data)
				return null;
			var dataController = BoundDataProvider.DataController;
			return dataController.GroupInfo.GetGroupRowInfoByHandle(dataController.GetControllerRowHandle(visibleIndex));
		}
		public object GetListSourceRowValues(int listSourceRowIndex, string[] fieldNames) {
			return GetRowValuesCore(listSourceRowIndex, fieldNames, new GetRowValueMethod(GetListSourceRowValue));
		}
		public IListServer GetServerModeListSource() {
			return DataProvider.GetServerModeListSource();
		}
		protected object GetRowValuesCore(int index, string[] fieldNames, GetRowValueMethod getValue) {
			if(fieldNames.Length == 1) return getValue(index, fieldNames[0]);
			object[] res = new object[fieldNames.Length];
			for(int n = 0; n < fieldNames.Length; n++) {
				res[n] = getValue(index, fieldNames[n]);
			}
			return res;
		}
		protected delegate object GetRowValueMethod(int index, string fieldName);
		public object GetRow(int visibleIndex) { return DataProvider.GetRow(visibleIndex); }
		public object GetRowValue(int visibleIndex, string fieldName) {
			if(AllowBatchEditing && visibleIndex == WebDataProxy.NewItemRow)
				return BatchEditHelper.GetNewRowInitValue(fieldName);
			if(IsMultipleKeyFields && fieldName == KeyFieldName)
				return GetRowKeyValue(visibleIndex);
			return DataProvider.GetRowValue(visibleIndex, fieldName, IsDesignTime);
		}
		public object GetListSourceRowValue(int listSourceRowIndex, string fieldName) {
			return DataProvider.GetListSourceRowValue(listSourceRowIndex, fieldName);
		}
		public object GetRowByListSourceIndex(int listSourceIndex) {
			DoOwnerDataBinding();
			var controller = GetDataController();
			if(controller != null)
				return controller.GetRowByListSourceIndex(listSourceIndex);
			return null;
		}
		public int GetChildDataRowCount(int visibleIndex) {
			return DataProvider.GetChildDataRowCount(visibleIndex);
		}
		public object GetRowValueForTemplate(int visibleIndex, string fieldName) {
			if(visibleIndex == EditingRowVisibleIndex) return GetEditingRowValue(visibleIndex, fieldName);
			return GetRowValue(visibleIndex, fieldName);
		}
		public object GetEditingRowValue(int visibleIndex, string fieldName) {
			if(this.editorValues != null && this.editorValues.ContainsKey(fieldName)) {
				return this.editorValues[fieldName];
			}
			return GetRowValue(visibleIndex, fieldName);
		}
		public int GetRowLevel(int visibleIndex) {
			return DataProvider.GetRowLevel(visibleIndex);
		}
		public bool IsRowExpanded(int visibleIndex) {
			return DataProvider.IsRowExpanded(visibleIndex);
		}
		public bool IsFocusedRowEnabled {
			get { return Owner.AllowFocusedRow; }
		}
		public object FocusedRowKeyValue {
			get { return focusedRowKeyValue; }
		}
		public int FocusedRowVisibleIndex {
			get {
				if(!IsFocusedRowEnabled || VisibleRowCount == 0) return -1;
				if(this.focusedRowIndexInternal == Int32.MinValue)
					this.focusedRowIndexInternal = PageIndex * PageSize;
				return focusedRowIndexInternal;
			}
			set {
				if(!IsFocusedRowEnabled) return;
				if(IsReady) {
					value = Math.Min(value, VisibleRowCount - 1);
					value = Math.Max(-1, value);
				}
				if(UseEndlessPaging)
					value = EndlessPagingHelper.CheckFocusedRowVisibleIndex(value);
				if(value == this.focusedRowIndexInternal) return;
				this.focusedRowIndexInternal = value;
				OnFocusedRowChanged();
			}
		}
		protected internal void CheckFocusedRowChanged() {
			if(!IsFocusedRowEnabled) return;
			object key = null;
			if(FocusedRowVisibleIndex < 0) {
				if(this.focusedRowIndexInternal < 0) return;
				this.focusedRowIndexInternal = -1;
			} else {
				int newIndex = FocusedRowVisibleIndex;
				if(UseEndlessPaging)
					newIndex = EndlessPagingHelper.CheckFocusedRowVisibleIndex(FocusedRowVisibleIndex);
				else if(!IsRowVisibleOnPage(FocusedRowVisibleIndex))
					newIndex = FocusedRowVisibleIndex < VisibleStartIndex ? VisibleStartIndex : (VisibleStartIndex + VisibleRowCountOnPage - 1);
				this.focusedRowIndexInternal = newIndex;
				key = GetRowKeyValue(FocusedRowVisibleIndex);
			}
			if(object.Equals(FocusedRowKeyValue, key)) return;
			this.focusedRowKeyValue = key;
			OnFocusedRowChanged();
		}
		public bool IsRowValid(int visibleIndex) {
			return visibleIndex >= 0 && visibleIndex < VisibleRowCount;
		}
		public bool IsRowVisibleOnPage(int visibleIndex) {
			return visibleIndex < 0 || visibleIndex >= VisibleStartIndex && visibleIndex < VisibleStartIndex + VisibleRowCountOnPage;
		}
		public bool IsRowFocused(int visibleIndex) {
			if(!IsFocusedRowEnabled) return false;
			return FocusedRowVisibleIndex == visibleIndex;
		}
		public string GetRowDisplayText(int visibleIndex, string fieldName) {
			object value = GetRowValue(visibleIndex, fieldName);
			if(value == null || value == DBNull.Value) return string.Empty;
			return value.ToString();
		}
		protected internal virtual void OnFocusedRowChanged() {
			this.focusedRowKeyValue = null;
			if(IsRowValid(FocusedRowVisibleIndex)) {
				this.focusedRowKeyValue = GetRowKeyValue(FocusedRowVisibleIndex);
				if(!IsRowVisibleOnPage(FocusedRowVisibleIndex) && !UseEndlessPaging) {
					this.lockPageFocusChange++;
					try {
						PageIndex = PageSize > 0 ? FocusedRowVisibleIndex / PageSize : 0;
					}
					finally {
						this.lockPageFocusChange--;
					}
				}
			}
			Events.OnFocusedRowChanged();
		}
		bool focusBottomRowOnPageChanged = false;
		public void RequireFocusBottomRowOnPageChanged() {
			this.focusBottomRowOnPageChanged = true;
		}
		protected virtual void OnPageIndexChanged(object sender, EventArgs e) {
			if(this.lockPageFocusChange != 0) return;
			if(UseEndlessPaging) {
				CheckFocusedRowChanged();
				return;
			}
			if(IsRowValid(FocusedRowVisibleIndex) && !IsRowVisibleOnPage(FocusedRowVisibleIndex)) {
				int index = VisibleStartIndex;
				if(this.focusBottomRowOnPageChanged) {
					index += VisibleRowCountOnPage - 1;
					this.focusBottomRowOnPageChanged = false;
				}
				FocusedRowVisibleIndex = index;
			}
		}
		public byte[] SaveData() {
			return DataProvider.SaveData(GetVisibleDataColumnsAndIds(), VisibleStartIndexInternal, VisibleRowCountOnPageInternal);
		}
		protected List<string> GetVisibleDataColumnsAndIds() {
			List<string> list = new List<string>();
			foreach(IWebColumnInfo column in Owner.GetColumns()) {
				list.Add(column.FieldName);
			}
			foreach (string field in KeyFieldNames) {
				if (!list.Contains(field)) {
					list.Add(field);
				}
			}
			return list;
		}
		WebDataCachedProvider cachedProvider = null;
		public bool HasCachedProvider { get { return cachedProvider != null; } }
		protected internal void LoadCachedData(byte[] data) {
			if(data == null || data.Length < 1) 
				return;
			this.cachedProvider = new WebDataCachedProvider(this, data);
		}
		protected internal void SetCachedDataProvider() {
			if(!HasCachedProvider) return;
			SetDataProvider(this.cachedProvider, true);
		}
		public void ExpandAll() {
			DataProvider.ExpandAll();
			CheckFocusedRowChanged();
		}
		public void CollapseAll() {
			DataProvider.CollapseAll();
			CheckFocusedRowChanged();
		}
		public List<int> GetSelectedVisibleIndexesIfAllExpanded() {
			List<int> list = new List<int>();
			if(Selection.Count == 0) return list;
			Hashtable groupRows = new Hashtable();
			for(int i = 0; i < VisibleRowCount; i ++) {
				if(GetRowType(i) == WebRowType.Group) {
					groupRows[i] = list.Count;
				}
				if(GetRowType(i) == WebRowType.Data && Selection.IsRowSelected(i)) {
					AddGroupRows(list, groupRows, i);
					list.Add(i);
				}
			}
			return list;
		}
		public object CreateFilterControlPropertyDescriptorsContainer() {
			var propertyDescriptors = DataProvider.GetColumnPropertyTypes().Where(col => !col.Key.Contains(".")).Select(colType => new FilterControlPropertyDescriptor(colType.Key, colType.Value));
			return new FilterControlPropertyDescriptorsContainer(propertyDescriptors);
		}
		void AddGroupRows(List<int> list, Hashtable groupRows, int visibleIndex) {
			if(DataProvider.GroupCount == 0) return;
			List<int> parentRows = GetParentGroupRows(visibleIndex);
			for(int i = parentRows.Count - 1; i >= 0; i--) {
				int parentIndex = parentRows[i];
				if(groupRows.ContainsKey(parentIndex)) {
					list.Insert((int)groupRows[parentIndex], parentIndex);
					groupRows.Remove(parentIndex);
				}
			}
		}
		public void SaveDataState(TypedBinaryWriter writer) {
			writer.WriteObject(PageSizeShowAllItem);
			writer.WriteObject(this.focusedRowIndexInternal); 
			writer.WriteTypedObject(FocusedRowKeyValue);
			writer.WriteObject(KeyFieldName);
			SaveKeyFieldTypes(writer);
			writer.WriteTypedObject(EditingKeyValue);
			writer.WriteObject(IsNewRowEditing);
			writer.WriteObject(IsServerMode);
			SaveRowState(writer);
			Selection.SaveState(writer);
			DetailRows.SaveState(writer);
		}
		public void LoadDataState(TypedBinaryReader reader, string pageSelectionResult, ArrayList pageKeyValues, bool restoreRowsState) {
			PageSizeShowAllItem = reader.ReadObject<bool>();
			this.focusedRowIndexInternal = reader.ReadObject<int>();
			this.focusedRowKeyValue = reader.ReadTypedObject();
			KeyFieldName = reader.ReadObject<string>();
			LoadKeyFieldTypes(reader);
			EditingKeyValue = reader.ReadTypedObject();
			this.isNewRowEditing = reader.ReadObject<bool>();
			this.isServerMode = reader.ReadObject<bool>();
			LoadDataProxyState(reader, pageSelectionResult);
			if(IsBound && restoreRowsState) {
				DataProvider.RestoreRowsState();
			}
			Selection.LoadState(reader, pageSelectionResult, GetKeyValuesFromScript(pageKeyValues));
			DetailRows.LoadState(reader);
		}
		protected void SaveRowState(TypedBinaryWriter writer) {
			MemoryStream stream = new MemoryStream();
			DataProvider.SaveRowState(stream);
			writer.WriteTypedObject((int)stream.Length);
			writer.Write(stream.ToArray());
		}
		protected void SaveKeyFieldTypes(TypedBinaryWriter writer) {
			var keyColumns = KeyFieldNames.Select(n => GetColumnInfo(n)).Where(c => c != null).ToList();
			writer.WriteObject(keyColumns.Count);
			if(keyColumns.Count == 0)
				return;
			foreach(var column in keyColumns) {
				writer.WriteObject(column.Name);
				writer.WriteType(GetKeyColumnType(column));
			}
		}
		protected void LoadKeyFieldTypes(TypedBinaryReader reader) {
			var count = reader.ReadObject<int>();
			for(var i = 0; i < count; i++) {
				var columnName = reader.ReadObject<string>();
				var type = reader.ReadType();
				KeyFieldTypes[columnName] = type;
			}
		}
		protected Type GetKeyColumnType(DataColumnInfo column) {
			var type = ReflectionUtils.StripNullableType(column.Type);
			if(type != typeof(object) || VisibleRowCountOnPage == 0)
				return type;
			var value = GetRowValue(VisibleStartIndex, column.Name);
			var valType = value.GetType();
			return valType;
		}
		protected void LoadDataProxyState(TypedBinaryReader reader, string pageSelectionResult) {
			int length = (int)reader.ReadTypedObject();
			byte[] expandInfo = reader.ReadBytes(length);
			DataProvider.SetRowState(expandInfo);
			this.storedPageSelectionResult = pageSelectionResult;
		}
		ArrayList deletedKeys;
		string storedPageSelectionResult = string.Empty;
		internal void ClearStoredPageSelectionResult() {
			this.storedPageSelectionResult = null;
		}
		public void RestoreRowsState() {
			DataProvider.RestoreRowsState();
			if(!string.IsNullOrEmpty(this.storedPageSelectionResult)) {
				Selection.LoadState(null, this.storedPageSelectionResult, null);
				this.storedPageSelectionResult = string.Empty;
				if(this.deletedKeys != null) {
					foreach(object key in this.deletedKeys)
						Selection.UnselectRowByKey(key);
					this.deletedKeys = null;
				}
			}
		}
		public ArrayList GetPageKeyValuesForScript() {
			if(UseEndlessPaging && VisibleRowCount > 0)
				CheckKeyFieldName();
			if(!HasCorrectKeyFieldName) 
				return new ArrayList();
			ArrayList list = new ArrayList(VisibleRowCountOnPage);
			for(int i = 0; i < VisibleRowCountOnPage; i++)
				list.Add(GetKeyValueForScript(VisibleStartIndex + i));
			return list;
		}
		public ArrayList GetKeyValuesFromScript(ArrayList list) {
			var result = new ArrayList();
			if(!HasCorrectKeyFieldName || list == null)
				return result;
			for(int i = 0; i < list.Count; i++)
				result.Add(GetKeyValueFromScript(list[i] as string));
			return result;
		}
		public void CollapseRow(int visibleIndex, bool recursive) {
			DataProvider.CollapseRow(visibleIndex, recursive);
			CheckFocusedRowChanged();
		}
		public void ExpandRow(int visibleIndex, bool recursive) {
			DataProvider.ExpandRow(visibleIndex, recursive);
			CheckFocusedRowChanged();
		}
		protected internal bool HasAnyColumn { get { return DataProvider.Columns.Count > 0; } }
		public bool IsUnboundField(string fieldName) { return DataProvider.IsUnboundField(fieldName);  }
		public bool HasFieldName(string fieldName) { return DataProvider.HasFieldName(fieldName); }
		public Type GetFieldType(string fieldName) { return DataProvider.GetFieldType(fieldName); }
		public DataColumnInfo GetColumnInfo(string fieldName) {
			if(!HasFieldName(fieldName)) return null;
			return DataProvider.Columns[fieldName]; 
		}
		protected internal List<string> KeyFieldNames { get { return keyFieldNames; } }
		public string KeyFieldName {
			get {
				if(KeyFieldNames.Count == 0) return string.Empty;
				StringBuilder sb = new StringBuilder();
				for(int i = 0; i < KeyFieldNames.Count; i++) {
					sb.Append(keyFieldNames[i]);
					if(i < keyFieldNames.Count - 1)
						sb.Append(MultipleKeyFieldSeparator);
				}
				return sb.ToString();
			}
			set {
				KeyFieldNames.Clear();
				if(string.IsNullOrEmpty(value)) return;
				string[] fields = value.Split(MultipleKeyFieldSeparator);
				foreach(string field in fields) {
					if(string.IsNullOrEmpty(field)) continue;
					KeyFieldNames.Add(field.Trim());
				}
			}
		}
		public bool IsMultipleKeyFields { get { return KeyFieldNames.Count > 1; } }
		public bool HasCorrectKeyFieldName { 
			get {
				if(KeyFieldNames.Count == 0) return false;
				foreach (string field in keyFieldNames) {
					if (!HasFieldName(field)) return false;
				}
				return true;
			} 
		}
		protected Dictionary<string, Type> KeyFieldTypes { 
			get {
				if(keyFieldTypes == null)
					keyFieldTypes = new Dictionary<string, Type>();
				return keyFieldTypes; 
			} 
		}
		internal void ThrowMissingPrimaryKey() {
			throw new MissingPrimaryKeyException(StringResources.GridView_MissingPkError);
		}
		protected internal object GetListSourceRowKeyValue(int listSourceRowIndex) {
			return GetKeyValueCore(listSourceRowIndex, GetListSourceRowValue);
		}
		public object GetRowKeyValue(int visibleIndex) {
			return GetRowKeyValue(visibleIndex, false);
		}
		protected internal object GetRowKeyValue(int visibleIndex, bool ensureRowDataIsBound) {
			if(ensureRowDataIsBound)
				EnsureRowDataIsBound(visibleIndex);
			if(IsMultipleKeyFields && (visibleIndex == NewItemRow || GetRow(visibleIndex) == null))
				return null;
			return GetKeyValueCore(visibleIndex, GetRowValue);
		}
		public string GetKeyValueForScript(object keyValue) {
			return ValueToString(keyValue);
		}
		public string GetKeyValueForScript(int visibleIndex) {
			object key = null;
			if(GetRowType(visibleIndex) == WebRowType.Data)
				key = GetRowKeyValue(visibleIndex);
			return GetKeyValueForScript(key);
		}
		public object GetKeyValueFromScript(string value) {
			return !IsMultipleKeyFields ? KeyStringToValue(KeyFieldName, value) : value;
		}
		protected string ValueToString(object value) {
			var val = DevExpress.Utils.Serializing.Helpers.ObjectConverter.ObjectToString(value);
			return val ?? NullKeyStringRepresentation;
		}
		protected void CheckKeyFieldName() {
			if(!HasCorrectKeyFieldName && !IsDesignTime) 
				ThrowMissingPrimaryKey();
		}
		protected object KeyStringToValue(string columnName, string strValue) {
			if(strValue == NullKeyStringRepresentation)
				return null;
			CheckKeyFieldName();
			Type type = ReflectionUtils.StripNullableType(GetColumnInfo(columnName).Type);
			if(type == typeof(object) && KeyFieldTypes.ContainsKey(columnName))
				type = KeyFieldTypes[columnName];
			return DevExpress.Utils.Serializing.Helpers.ObjectConverter.StringToObject(strValue, type);
		}
		protected delegate object GetKeyValueCallback(int index, string fieldName);
		protected virtual object GetKeyValueCore(int index, GetKeyValueCallback getKeyValue) {
			CheckKeyFieldName();
			if(IsMultipleKeyFields) {
				object[] keyValues = new object[KeyFieldNames.Count];
				for(int i = 0; i < KeyFieldNames.Count; i++)
					keyValues[i] = getKeyValue(index, KeyFieldNames[i]);
				return ConvertMulitpleKeyValuesToString(keyValues);
			}
			return getKeyValue(index, KeyFieldName);
		}
		public void StartEdit(int visibleIndex) {
			CancelEdit();
			EditingKeyValue = GetRowKeyValue(visibleIndex);
			if(!DoStartEdit()) CancelEdit();
		}
		public void CancelEdit() {
			if(!IsEditing) return;
			var e = Owner.CreateStartItemEditingArgs(EditingKeyValue);
			OnCancelEditRow(e);
			if(!e.Cancel)
				StopEdit();
		}
		public void AddNewRow() {
			CancelEdit();
			if(!AllowBatchEditing)
				this.isNewRowEditing = true;
			DoInitNewRow();
		}
		public bool EndEdit() {
			if(AllowBatchEditing)
				return BatchEditHelper.CommitTransaction();
			if(!IsEditing) return false;
			bool autoCreatedEditorsAreValid = Owner.ValidateAutoCreatedEditors();
			bool templatesAreValid = Owner.ValidateEditTemplates();
			bool rowIsValid = ValidateRow();
			if(!autoCreatedEditorsAreValid || !templatesAreValid || !rowIsValid)
				return false;
			if(IsNewRowEditing)
				InsertRow(EditingRowVisibleIndex);
			else
				UpdateRow(EditingRowVisibleIndex);
			return true;
		}
		public void DeleteRow(int visibleIndex) {
			CancelEdit();
			DeleteRowCore(visibleIndex);
		}
		protected void ClearEditCache() {
			this.editorValues = null;
		}
		public bool IsEditing { 
			get {
				if(AllowBatchEditing) return false;
				return  EditingKeyValue != null || IsNewRowEditing; 
			} 
		}
		public bool IsNewRowEditing { 
			get {
				if(AllowBatchEditing) return false;
				return isNewRowEditing;
			} 
		}
		public object EditingKeyValue { 
			get { return editingKeyValue; } 
			set { 
				editingKeyValue = value;
				ResetEditingRowVisibleIndex();
				this.isNewRowEditing = false;
			}  
		}
		public virtual bool IsEditorValuesExists { get { return this.editorValues != null && this.editorValues.Count > 0; } }
		public void SetEditorValues(Dictionary<string, object> values, bool canIgnoreInvalidValues) {
			this.editorValues = values;
			ParseEditorValues(canIgnoreInvalidValues);
		}
		int editingRowVisibleIndex = DataController.InvalidRow;
		void ResetEditingRowVisibleIndex() {
			this.editingRowVisibleIndex = DataController.InvalidRow;
		}
		public int EditingRowVisibleIndex {
			get {
				if(!IsEditing) return -1;
				if(IsNewRowEditing) return WebDataProxy.NewItemRow;
				if(editingRowVisibleIndex == DataController.InvalidRow) {
					editingRowVisibleIndex = FindVisibleIndexByKeyCore(EditingKeyValue, false);
				}
				return editingRowVisibleIndex;
			}
		}
		public bool IsRowEditing(int visibleIndex) {
			if(!IsEditing) return false;
			if(visibleIndex == BaseListSourceDataController.NewItemRow) return true;
			if(GetRowType(visibleIndex) != WebRowType.Data) return false;
			return object.Equals(EditingKeyValue, GetRowKeyValue(visibleIndex));
		}
		public bool HasParentRows { get { return GetParentRows().Count > 0; } }
		public List<int> GetParentRows() {
			return GetParentGroupRows(VisibleStartIndex);
		}
		public List<int> GetParentGroupRows(int visibleIndex) {
			return DataProvider.GetParentGroupRows(visibleIndex);
		}
		public List<int> GetGroupFooterVisibleIndexes(int visibleIndex, bool visibleIfGroupRowExpandedOnly) {
			if(visibleIndex < 0) return null;
			int parentVisibleIndex = -1;
			if(GetRowType(visibleIndex) == WebRowType.Group) {
				if(IsRowExpanded(visibleIndex)) return null;
				if(!visibleIfGroupRowExpandedOnly) {
					parentVisibleIndex = visibleIndex;
				}
			}
			if(IsLastRowInCurrentLevel(visibleIndex)) {
				List<int> res = GetFooterParentGroupRows(visibleIndex);
				if(parentVisibleIndex > -1) {
					res.Insert(0, parentVisibleIndex);
				}
				return res;
			}
			if(parentVisibleIndex < 0) return null;
			List<int> result = new List<int>();
			result.Add(parentVisibleIndex);
			return result;
		}
		public List<int> GetFooterParentGroupRows(int visibleIndex) {
			return DataProvider.GetFooterParentGroupRows(visibleIndex);
		}
		public bool IsLastRowInCurrentLevel(int visibleIndex) {
			return DataProvider.IsLastRowInCurrentLevel(visibleIndex);
		}
		public int RowIsLastInLevel(int visibleIndex) {
			return DataProvider.RowIsLastInLevel(visibleIndex);
		}
		public virtual List<object> GetSelectedValues(string[] fieldNames) {
			return Selection.GetSelectedValues(fieldNames, false);
		}
		public List<object> GetFilteredSelectedValues(string[] fieldNames) {
			return Selection.GetSelectedValues(fieldNames, true);
		}
		public object GetValues(int visibleIndex, string[] fieldNames) {
			EnsureRowDataIsBound(visibleIndex);
			return GetRowValues(visibleIndex, fieldNames);
		}
		void EnsureRowDataIsBound(int visibleIndex) {
			if(!IsBound && OwnerDataBinding != null &&
			   (!IsRowVisibleOnPage(visibleIndex) || VisibleRowCountOnPage == 0))
				DoOwnerDataBinding();
		}
		public int FindVisibleIndexByKey(object keyValue, bool expandGroups, bool withBinding) {
			int visibleIndex = FindVisibleIndexByKeyCore(keyValue, expandGroups);
			if(!IsBound && visibleIndex < 0 && withBinding) {
				DoOwnerDataBinding();
				visibleIndex = FindVisibleIndexByKeyCore(keyValue, expandGroups);
			}
			return visibleIndex;
		}
		public int FindVisibleIndexByKey(object keyValue, bool expandGroups) {			
			return FindVisibleIndexByKey(keyValue, expandGroups, true);
		}
		protected int FindVisibleIndexByKeyCore(object value, bool expandGroups) {
			if(!IsMultipleKeyFields)
				return DataProvider.FindRowByKey(KeyFieldName, value, expandGroups);
			Dictionary<string, object> keyValues = GetMultipleKeyValues(value, !(value is Array));
			return DataProvider.FindRowByKeys(keyValues, expandGroups);
		}
		protected internal void ValidateSelectedKeys() {
			DataProvider.ValidateSelectedKeys();
		}
		protected internal bool IsValidKey(object serializedKey) {
			return DataProvider.IsValidKey(serializedKey);
		}
		protected internal Dictionary<object, bool> GetFilteredSelectedKeys() {
			if(this.DataProvider.ListSourceRowCount > 0)
				DoOwnerDataBinding();
			return DataProvider.GetFilteredSelectedKeys();
		}
		protected internal Dictionary<string, object> GetMultipleKeyValues(object value, bool useStringValues) {
			if(value == null) 
				return null;
			object[] values = useStringValues ?
				ConvertMultipleKeyValuesFromString(value.ToString().Split(MultipleKeyValueSeparator)) : value as object[];
			if(values == null || values.Length != KeyFieldNames.Count)
				return null;
			Dictionary<string, object> dict = new Dictionary<string, object>();
			for(int i = 0; i < values.Length; i++)
				dict[KeyFieldNames[i]] = values[i];
			return dict;
		}
		internal object[] ConvertMultipleKeyValuesFromString(string[] stringValues) {
			if(stringValues == null || stringValues.Length != KeyFieldNames.Count)
				return null;
			ArrayList list = new ArrayList();
			for(int i = 0; i < KeyFieldNames.Count; i++) {
				var stringValue = stringValues[i];
				if(stringValue.IndexOf(MultipleKeyValueEncodedSeparator) > -1)
					stringValue = stringValue.Replace(MultipleKeyValueEncodedSeparator, MultipleKeyValueSeparator.ToString());
				list.Add(KeyStringToValue(KeyFieldNames[i], stringValue));
			}
			return list.ToArray();
		}
		protected internal string ConvertMulitpleKeyValuesToString(object[] keys) {
			var sb = new StringBuilder();
			for(int i = 0; i < KeyFieldNames.Count; i++) {
				if(i > 0)
					sb.Append(MultipleKeyValueSeparator);
				var keyString = ValueToString(keys[i]);
				if(keyString.IndexOf(MultipleKeyValueSeparator) > -1)
					keyString = keyString.Replace(MultipleKeyValueSeparator.ToString(), MultipleKeyValueEncodedSeparator);
				sb.Append(keyString);
			}
			return sb.ToString();
		}
		public bool MakeRowVisible(object keyValue) {
			int visibleIndex = FindVisibleIndexByKey(keyValue, true);
			SetPageIndexByVisibleIndex(visibleIndex);
			return visibleIndex > -1;
		}
		protected void SetPageIndexByVisibleIndex(int visibleIndex) {
			if(visibleIndex > VisibleRowCount - 1 || visibleIndex < 0) return;
			PageIndex = visibleIndex / VisibleRowCountOnPage;
		}
		public object GetValuesByKeyValue(object keyValue, string[] fieldNames) {
			int visibleIndex = FindVisibleIndexByKey(keyValue, false);
			return visibleIndex > -1 ? GetRowValues(visibleIndex, fieldNames) : null;
		}
		public List<object> GetCurrentPageRowValues(string[] fieldNames) {
			List<object> res = new List<object>();
			for(int n = 0; n < VisibleRowCountOnPage; n++) {
				res.Add(GetRowValues(VisibleStartIndex + n, fieldNames));
			}
			return res;
		}
		public object ConvertValue(string fieldName, object value) {
			try {
				return DataProvider.ConvertValue(fieldName, value);
			}
			catch {
				return value;
			}
		}
		void CheckEditDictionaries(OrderedDictionary orderedDictionary, ParameterCollection allowedParameters, bool requireDataColumn) {
			if(allowedParameters != null && allowedParameters.Count == 0) allowedParameters = null;
			Dictionary<string, object> temp = new Dictionary<string, object>();
			foreach(DictionaryEntry entry in orderedDictionary) {
				string field = entry.Key.ToString();
				if(allowedParameters != null && allowedParameters[field] == null)
					continue;
				var usedAsParam = allowedParameters != null && allowedParameters[field] != null;
				var hasFieldName = HasFieldName(field);
				if(!hasFieldName && requireDataColumn && !usedAsParam)
					continue;
				if(hasFieldName && IsUnboundField(field))
					continue;
				temp[field] = entry.Value;
			}
			orderedDictionary.Clear();
			foreach(KeyValuePair<string, object> pair in temp) {
				object val = pair.Value;
				if(val == DBNull.Value)
					val = null;
				orderedDictionary.Add(pair.Key, val);
			}
		}
		void FillEditDictionaries(OrderedDictionary oldValues, OrderedDictionary newValues, OrderedDictionary keys, ParameterCollection updateParameters, int visibleIndex) {
			if(AllowBatchEditing) {
				FillBatchEditDictionaries(oldValues, newValues, keys, updateParameters, visibleIndex);
				return;
			}
			FillValuesForUpdatedParameters(oldValues, newValues, updateParameters, visibleIndex, IsNewRowEditing);
			if(this.editorValues != null) {
				if(!this.editorValuesParsed) 
					ParseEditorValues(false);
				foreach(KeyValuePair<string, object> entry in this.editorValues) {
					if(oldValues != null && !IsNewRowEditing) {
						oldValues[entry.Key] = GetRowValue(EditingRowVisibleIndex, entry.Key);
					}
					newValues[entry.Key] = entry.Value;
				}
			}
			if(keys != null && !IsNewRowEditing) {
				FillEditKeys(keys);
			}
			Dictionary<string, object> values = Owner.GetEditTemplateValues();
			if(values != null) {
				values = ParseValuesCore(values, false, EditingRowVisibleIndex);
				if(this.editorValues == null) this.editorValues = new Dictionary<string, object>();
				foreach(KeyValuePair<string, object> entry in values) {
					newValues[entry.Key] = entry.Value;
					if(!IsNewRowEditing && !oldValues.Contains(entry.Key) && HasFieldName(entry.Key))
						oldValues[entry.Key] = GetRowValue(EditingRowVisibleIndex, entry.Key);
					this.editorValues[entry.Key] = entry.Value;
				}
			}
		}
		void FillBatchEditDictionaries(OrderedDictionary oldValues, OrderedDictionary newValues, OrderedDictionary keys, ParameterCollection updateParameters, int visibleIndex) {
			var isNewRow = visibleIndex < 0;
			FillValuesForUpdatedParameters(oldValues, newValues, updateParameters, visibleIndex, isNewRow);
			if(isNewRow && !BatchEditHelper.ClientInsertedRowValues.ContainsKey(visibleIndex))
				return;
			if(!isNewRow && !BatchEditHelper.ClientUpdatedRowValues.ContainsKey(visibleIndex))
				return;
			var editorValues = isNewRow ? BatchEditHelper.ClientInsertedRowValues[visibleIndex] : BatchEditHelper.ClientUpdatedRowValues[visibleIndex];
			var populateOldValues = oldValues != null && !isNewRow;
			foreach(var pair in editorValues) {
				var fieldName = pair.Key;
				if(populateOldValues)
					oldValues[fieldName] = GetRowValue(visibleIndex, fieldName);
				newValues[fieldName] = pair.Value;
			}
			if(keys != null && !isNewRow)
				FillEditKeysCore(keys, GetRowKeyValue(visibleIndex));
		}
		void FillEditKeys(OrderedDictionary keys) {
			FillEditKeysCore(keys, EditingKeyValue);
		}
		void FillEditKeysCore(OrderedDictionary keys, object value) {
			if (!IsMultipleKeyFields) {
				keys.Add(KeyFieldName, value);
			} else {
				FillMultipleEditKeys(keys, value);
			}
		}
		void FillMultipleEditKeys(OrderedDictionary keys, object value) {
			Dictionary<string, object> values = GetMultipleKeyValues(value, true);
			if (values == null) return;
			foreach (string columnName in values.Keys) {
				keys.Add(columnName, values[columnName]);
			}
		}
		void FillValuesForUpdatedParameters(OrderedDictionary oldValues, OrderedDictionary newValues, ParameterCollection updateParameters, int visibleIndex, bool isNewRow) {
			if(updateParameters == null || updateParameters.Count == 0) return;
			foreach(Parameter par in updateParameters) {
				if(!HasFieldName(par.Name)) continue;
				UpdateOrderedDictionary(newValues, par.Name, visibleIndex);
				if(oldValues != null && !isNewRow)
					UpdateOrderedDictionary(oldValues, par.Name, visibleIndex);
			}
		}
		void UpdateOrderedDictionary(OrderedDictionary values, string fieldName, int visibleIndex) {
			if(values.Contains(fieldName)) return;
			values[fieldName] = GetRowValue(visibleIndex, fieldName);
		}
		void RemoveReadonlyFields(IOrderedDictionary data) {
			ArrayList keys = new ArrayList();
			foreach(object key in data.Keys) {
				string fieldName = (string)key;
				if(DataProvider.Columns[fieldName] == null) continue;
				if(DataProvider.Columns[fieldName].ReadOnly)
					keys.Add(key);
			}
			foreach(object key in keys)
				data.Remove(key);
		}
		void ParseEditorValues(bool canIgnoreInvalidValues) {
			this.editorValues = ParseValuesCore(this.editorValues, canIgnoreInvalidValues, EditingRowVisibleIndex);
			this.editorValuesParsed = true;
		}
		Dictionary<string, object> ParseValuesCore(Dictionary<string, object> values, bool canIgnoreInvalidValues, int visibleIndex) {
			Dictionary<string, object> res = new Dictionary<string, object>();
			foreach(KeyValuePair<string, object> entry in values) {
				object parsedValue;
				if(ParseEditorValue(visibleIndex, entry.Key, entry.Value, canIgnoreInvalidValues, out parsedValue))
					res[entry.Key] = parsedValue;
			}
			return res;
		}
		protected internal bool ParseEditorValue(int visibleIndex, string fieldName, object value, bool ignoreInvalidValue, out object parsedValue) {
			parsedValue = null;
			try {
				ASPxParseValueEventArgs e = new ASPxParseValueEventArgs(fieldName, value, visibleIndex);
				Events.OnParseValue(e);
				parsedValue = DataProvider.ConvertValue(e.FieldName, e.Value);
				return true;
			}
			catch {
				if(!ignoreInvalidValue) throw;
			}
			return false;
		}
		protected bool ValidateRow() { return ValidateRow(false); }
		public bool ValidateRow(bool alwaysFillValues) {
			return ValidateRowCore(EditingRowVisibleIndex, alwaysFillValues, IsNewRowEditing);
		}
		public bool ValidateRowCore(int visibleIndex, bool alwaysFillValues, bool isNew) {
			var args = Owner.CreateItemValidationEventArgs(visibleIndex, isNew);
			FillEditDictionaries(args.OldValues, args.NewValues, args.Keys, null, visibleIndex);
			if(alwaysFillValues && !isNew) {
				CompleteEditDictionaryFromColumns(args.OldValues);
				CompleteEditDictionaryFromColumns(args.NewValues);
			}
			OnValidatingRow(args);
			return !args.HasErrors;
		}
		void CompleteEditDictionaryFromColumns(OrderedDictionary values) {
			foreach(DataColumnInfo info in DataProvider.Columns) {
				if(info.Unbound) continue;
				if(values.Contains(info.Name)) continue;
				values[info.Name] = GetRowValue(EditingRowVisibleIndex, info.Name);
			}
		}
		ASPxDataUpdatingEventArgs updatingArgs = null;
		protected internal void UpdateRow() {
			UpdateRow(-1);
		}
		protected internal void UpdateRow(int visibleIndex) {
			if(!IsEditing && !AllowBatchEditing) return;
			DataSourceView view = GetView();
			ParameterCollection updateParameters = GetUpdateParameters(view);
			var updateRowValues = view != null ? CreateDataUpdateValues(visibleIndex, updateParameters) : null;
			ASPxDataUpdatingEventArgs updateArgs = new ASPxDataUpdatingEventArgs(updateRowValues);
			OnRowUpdating(updateArgs);
			if(updateArgs.Cancel || view == null) return;
			RemoveReadonlyFields(updateArgs.NewValues);
			this.updatingArgs = updateArgs;
			CheckEditDictionaries(updateArgs.NewValues, updateParameters, true);
			CheckEditDictionaries(updateArgs.OldValues, updateParameters, true);
			view.Update(updateArgs.Keys, updateArgs.NewValues, updateArgs.OldValues, new DataSourceViewOperationCallback(HandleDataSourceViewUpdateOperationCallback));
		}
		ParameterCollection GetUpdateParameters(DataSourceView view) {
			SqlDataSourceView sqlView = view as SqlDataSourceView;
			if(sqlView != null) return sqlView.UpdateParameters;
			ObjectDataSourceView objView = view as ObjectDataSourceView;
			if(objView != null) return objView.UpdateParameters;
			return null;
		}
		ParameterCollection GetInsertParameters(DataSourceView view) {
			SqlDataSourceView sqlView = view as SqlDataSourceView;
			if(sqlView != null)
				return sqlView.InsertParameters;
			ObjectDataSourceView objView = view as ObjectDataSourceView;
			if(objView != null)
				return objView.InsertParameters;
			return null;
		}
		protected virtual bool HandleDataSourceViewUpdateOperationCallback(int affectedRecords, Exception e) {
			ASPxDataUpdatedEventArgs args;
			args = this.updatingArgs == null ? new ASPxDataUpdatedEventArgs(affectedRecords, e) :
				new ASPxDataUpdatedEventArgs(affectedRecords, e, this.updatingArgs);
			OnRowUpdated(args);
			this.updatingArgs = null;
			if(e != null && !args.ExceptionHandled) return false;
			StopEdit();
			return true;
		}
		ASPxDataInsertingEventArgs insertingArgs;
		protected internal void InsertRow(int visibleIndex) {
			if(!IsNewRowEditing && !AllowBatchEditing) return;
			DataSourceView view = GetView();
			var insertRowValues = view != null ? CreateDataInsertValues(visibleIndex) : null;
			ASPxDataInsertingEventArgs insertArgs = new ASPxDataInsertingEventArgs(insertRowValues);
			OnRowInserting(insertArgs);
			if(insertArgs.Cancel || view == null) return;
			this.insertingArgs = insertArgs;
			CheckEditDictionaries(insertArgs.NewValues, GetInsertParameters(view), false);
			view.Insert(insertArgs.NewValues, new DataSourceViewOperationCallback(HandleDataSourceViewInsertOperationCallback));
		}
		protected void DeleteRowCore(int visibleIndex) {
			if(DataProvider.GetRowType(visibleIndex) != WebRowType.Data || !DataProvider.IsValidRow(visibleIndex)) 
				return;
			DeleteRowCore(CreateDataDeleteValues(visibleIndex));
		}
		ASPxDataDeletingEventArgs deletingArgs;
		protected internal void DeleteRowCore(ASPxDataDeleteValues deleteRowValues) {
			DataSourceView view = GetView();
			ASPxDataDeletingEventArgs args = new ASPxDataDeletingEventArgs(deleteRowValues);
			OnRowDeleting(args);
			if(args.Cancel || view == null)
				return;
			this.deletingArgs = args;
			CheckEditDictionaries(args.Values, null, true);
			if(this.deletedKeys == null)
				this.deletedKeys = new ArrayList();
			this.deletedKeys.Add(deleteRowValues.RowKey);
			Selection.UnselectRowByKey(deleteRowValues.RowKey);
			view.Delete(args.Keys, args.Values, new DataSourceViewOperationCallback(HandleDataSourceViewDeleteOperationCallback));
		}
		protected internal ASPxDataInsertValues CreateDataInsertValues(int visibleIndex) {
			var result = new ASPxDataInsertValues(visibleIndex);
			FillEditDictionaries(null, result.NewValues, null, null, visibleIndex);
			return result;
		}
		protected internal ASPxDataUpdateValues CreateDataUpdateValues(int visibleIndex, ParameterCollection updateParams) {
			var result = new ASPxDataUpdateValues(GetRowKeyValue(visibleIndex));
			FillEditDictionaries(result.OldValues, result.NewValues, result.Keys, updateParams, visibleIndex);
			return result;
		}
		protected internal ASPxDataDeleteValues CreateDataDeleteValues(int visibleIndex) {
			var result = new ASPxDataDeleteValues(GetRowKeyValue(visibleIndex));
			foreach(var fieldName in GetVisibleDataColumnsAndIds())
				result.Values[fieldName] = GetRowValue(visibleIndex, fieldName);
			FillEditKeysCore(result.Keys, GetRowKeyValue(visibleIndex));
			return result;
		}
		protected virtual bool HandleDataSourceViewDeleteOperationCallback(int affectedRecords, Exception e) {
			ASPxDataDeletedEventArgs args;
			args = this.deletingArgs == null ? new ASPxDataDeletedEventArgs(affectedRecords, e) :
				new ASPxDataDeletedEventArgs(affectedRecords, e, this.deletingArgs);
			OnRowDeleted(args);
			this.deletingArgs = null;
			if(e != null && !args.ExceptionHandled) return false;
			return true;
		}
		protected DataSourceView GetView() {
			DataSourceView view = GetData();
			if(view == null)
				throw new Exception("DataSource returned null");
			return view;
		}
		protected virtual bool HandleDataSourceViewInsertOperationCallback(int affectedRecords, Exception e) {
			ASPxDataInsertedEventArgs args;
			args = this.insertingArgs == null ? new ASPxDataInsertedEventArgs(affectedRecords, e) :
				new ASPxDataInsertedEventArgs(affectedRecords, e, this.insertingArgs.NewValues);
			OnRowInserted(args);
			this.insertingArgs = null;
			if(e != null && !args.ExceptionHandled) return false;
			StopEdit();
			return true;
		}
		void StopEdit() {
			EditingKeyValue = null;
			ClearEditCache();
		}
		protected virtual bool DoStartEdit() {
			if(!IsEditing) return false;
			var e = Owner.CreateStartItemEditingArgs(EditingKeyValue);
			OnStartEditRow(e);
			return !e.Cancel;
		}
		protected internal virtual void DoInitNewRow() {
			if(!IsNewRowEditing && !AllowBatchEditing) return;
			ASPxDataInitNewRowEventArgs e = new ASPxDataInitNewRowEventArgs();
			OnInitNewRow(e);
			if(AllowBatchEditing) {
				BatchEditHelper.SetNewRowInitValues(e.NewValues);
				return;
			}
			if(e.NewValues.Count > 0 && this.editorValues == null) this.editorValues = new Dictionary<string, object>();
			foreach(DictionaryEntry entry in e.NewValues)
				this.editorValues[entry.Key.ToString()] = entry.Value;
		}
		protected virtual void OnRowDeleting(ASPxDataDeletingEventArgs e) { Events.OnRowDeleting(e);  }
		protected virtual void OnRowDeleted(ASPxDataDeletedEventArgs e) { Events.OnRowDeleted(e); }
		protected virtual void OnValidatingRow(ASPxGridDataValidationEventArgs e) { Events.OnRowValidating(e); }
		protected virtual void OnInitNewRow(ASPxDataInitNewRowEventArgs e) { Events.OnInitNewRow(e); }
		protected virtual void OnStartEditRow(ASPxStartItemEditingEventArgs e) { Events.OnStartRowEditing(e); }
		protected virtual void OnCancelEditRow(ASPxStartItemEditingEventArgs e) { Events.OnCancelRowEditing(e); }
		protected virtual void OnRowInserting(ASPxDataInsertingEventArgs e) { Events.OnRowInserting(e); }
		protected virtual void OnRowInserted(ASPxDataInsertedEventArgs e) { Events.OnRowInserted(e); }
		protected virtual void OnRowUpdating(ASPxDataUpdatingEventArgs e) { Events.OnRowUpdating(e); }
		protected virtual void OnRowUpdated(ASPxDataUpdatedEventArgs e) { Events.OnRowUpdated(e); }
		protected internal virtual void OnBatchUpdate(ASPxDataBatchUpdateEventArgs e) { Events.OnBatchUpdate(e); }
		protected internal virtual void UpdateColumnBindings() {
			DataProvider.UpdateColumnBindings();
		}
		public bool IsGroupRowFitOnPage(int visibleIndex) {
			return DataProvider.IsGroupRowFitOnPage(visibleIndex, VisibleStartIndex, VisibleRowCountOnPage);
		}
		public bool IsContinuedFromPrevPage(int visibleIndex) {
			return DataProvider.IsContinuedFromPrevPage(visibleIndex, VisibleStartIndex);
		}
		protected internal virtual void OnSelectionChanged() {
			if(Events != null) Events.OnSelectionChanged();
		}
		protected internal virtual void OnDetailRowsChanged() {
			if(Events != null) Events.OnDetailRowsChanged();
		}
		protected internal virtual int GetSelectedRowCountOnPage() {
			int selCount = Selection.CachedCount;
			if(selCount == 0) return 0;
			int res = 0;
			for(int n = 0; n < VisibleRowCountOnPage; n++) {
				if(Selection.IsRowSelected(VisibleStartIndex + n)) {
					res++;
					if(res == selCount) break;
				}
			}
			if(UseEndlessPaging)
				res += EndlessPagingHelper.GetClientSelectedRowCount();
			return res;
		}
		protected internal virtual int GetCachedSelectedRowCountWithoutCurrentPage() {
			return GetSelectedRowCountWithoutCurrentPageCore(Selection.CachedCount);
		}
		protected internal virtual int GetCachedSelectedFilteredRowCountWithoutCurrentPage() {
			return GetSelectedRowCountWithoutCurrentPageCore(Selection.CachedFilteredCount);
		}
		int GetSelectedRowCountWithoutCurrentPageCore(int selectedRowCount) {
			if(selectedRowCount == 0) return 0;
			return selectedRowCount - GetSelectedRowCountOnPage();
		}
		public object GetRowForTemplate(int visibleIndex) {
			return DataProvider.GetRowForTemplate(visibleIndex);
		}
		protected internal void RequireDataBound() {
			if(Owner != null) Owner.RequireDataBound();
		}
		public virtual void Dispose() {
			DataProvider.Dispose();
		}
		public virtual bool IsReadOnly(string fieldName) {
			DataColumnInfo info = DataProvider.Columns[fieldName];
			return info == null || info.ReadOnly;
		}
		int useCachedProvider = 0;
		WebDataProviderBase savedProvider = null;
		internal void BeginUseCachedProvider() {
			if(this.useCachedProvider++ == 0 && HasCachedProvider) {
				this.savedProvider = DataProvider;
				SetDataProvider(this.cachedProvider, true);
			}
		}
		internal void EndUseCachedProvider() {
			if(--this.useCachedProvider == 0 && this.savedProvider != null) {
				SetDataProvider(this.savedProvider, true);
				this.savedProvider = null;
			}
		}
		protected internal Type forcedDataRowType = null;
		internal void ForceDataRowType(Type type) {
			this.forcedDataRowType = type;
			DataProvider.ForceDataRowType();
		}
	}
	public abstract class WebDataProviderBase {
		byte[] rowsState;
		Dictionary<string, bool> serializedColumns;
		WebDataProxy proxy;
		int groupCount;
		public WebDataProviderBase(WebDataProxy proxy) {
			this.proxy = proxy;
			this.groupCount = 0;
			this.serializedColumns = new Dictionary<string, bool>();
		}
		public WebDataProxy Proxy { get { return proxy; } }
		protected internal abstract DataColumnInfoCollection Columns { get; }
		public virtual int GroupCount { get { return groupCount; } }
		internal void SetGroupCount(int value) { this.groupCount = value; } 
		public virtual object GetRowForTemplate(int visibleIndex) { return GetRow(visibleIndex); }
		public virtual bool ServerMode { get { return false; } }
		public abstract bool IsReady { get; }
		public virtual bool IsBound { get { return IsReady; } }
		public abstract int VisibleCount { get; }
		public abstract int ListSourceRowCount { get; }
		public abstract WebRowType GetRowType(int visibleIndex);
		public abstract object GetRowValue(int visibleIndex, string fieldName, bool isDesignTime);
		public abstract int GetChildDataRowCount(int visibleIndex);
		public abstract object GetRow(int visibleIndex);
		public abstract int GetRowLevel(int visibleIndex);
		public abstract bool IsRowExpanded(int visibleIndex);
		public abstract int FindRowByKey(string keyFieldName, object keyValue, bool expandGroups);
		public abstract int FindRowByKeys(Dictionary<string, object> columnValues, bool expandGroups);
		public abstract bool HasFieldName(string fieldName);
		public abstract bool IsUnboundField(string fieldName);
		public abstract Type GetFieldTypeCore(string fieldName);
		public abstract object GetTotalSummaryValue(ASPxSummaryItemBase item);
		public abstract object GetFormatConditionSummaryValue(FormatConditionSummary item);
		public abstract bool IsGroupSummaryExists(int visibleIndex, ASPxSummaryItem item);
		public abstract object GetGroupSummaryValue(int visibleIndex, ASPxSummaryItem item);
		public abstract bool IsExpressionFitToRule(string expression, int visibleIndex);
		public abstract bool IsGroupRowFitOnPage(int visibleIndex, int visibleStartIndex, int pageSize);
		public abstract List<int> GetParentGroupRows(int visibleIndex);
		public abstract List<int> GetFooterParentGroupRows(int visibleIndex);
		public abstract int RowIsLastInLevel(int visibleIndex);
		public abstract void ValidateSelectedKeys();
		public abstract bool IsValidKey(object serializedKey);
		public abstract Dictionary<object, bool> GetFilteredSelectedKeys();
		public abstract Dictionary<string, Type> GetColumnPropertyTypes();
		public bool IsLastRowInCurrentLevel(int visibleIndex) {
			return RowIsLastInLevel(visibleIndex) > -1;
		}
		public Type GetFieldType(string fieldName) {
			return HasFieldName(fieldName) ? GetFieldTypeCore(fieldName) : null;
		}
		public virtual object ConvertValue(string fieldName, object value) {
			DataColumnInfo column = Columns[fieldName];
			if(column != null) return ConvertColumnValue(column, value);
			return value;
		}
		public virtual bool IsFiltered { get { return !string.IsNullOrEmpty(FilterExpression); } }
		public virtual string FilterExpression { get { return string.Empty; } }
		public virtual void SortGroupChanged(List<IWebColumnInfo> sortList, int groupCount, string filterExpression, IList groupSummary, IList totalSummary, IList formatConditionSummary) { }
		public virtual object[] GetUniqueColumnValues(string fieldName, int maxCount, bool includeFilteredOut) { return null; }
		public virtual void SetDataSource(object dataSource) {}
		public virtual void ExpandAll() { }
		public virtual void CollapseAll() { }
		public virtual void SaveRowState(Stream stream) { }
		public virtual void RestoreRowsState() { }
		public virtual bool IsValidRow(int visibleIndex) {
			return (visibleIndex >= 0 && visibleIndex < VisibleCount);
		}
		protected abstract List<DataColumnInfo> GetSavedColums(List<string> usedFields);
		protected abstract List<object> GetTotalSummary();
		protected abstract List<object> GetGroupSummary(int visibleIndex);
		protected abstract List<object> GetFormatConditionsSummary();
		public abstract bool AllowSearchPanelFilter(DataColumnInfo column);
		protected void AddFieldNameIntoSerializedList(string fieldName) {
			if(string.IsNullOrEmpty(fieldName)) return;
			SerializedColumns[fieldName] = true;
		}
		protected virtual object ConvertColumnValue(DataColumnInfo column, object value) {
			if(value == null) return value;
			if(value is string && DataUtils.IsFloatType(column.GetDataType()))
				value = DataUtils.FixFloatingPoint(value.ToString(), System.Globalization.CultureInfo.InvariantCulture);
			return column.ConvertValue(value);
		}
		protected bool IsRowValuesEquals(Dictionary<string, object> values, int rowIndex) {
			foreach (string columnName in values.Keys) {
				object val = GetRowValue(rowIndex, columnName, false);
				if (!Object.Equals(val, values[columnName])) return false;
			}
			return true;
		}
		protected Dictionary<string, bool> SerializedColumns { get { return serializedColumns; } }
		public List<string> GetSerializedColumns() {
			List<string> usedFields = new List<string>();
			foreach(string key in SerializedColumns.Keys) {
				usedFields.Add(key);
			}
			return usedFields;
		}
		protected virtual List<string> GetNonUsedColumns() {
			List<string> res = new List<string>();
			foreach(DataColumnInfo column in Columns) {
				if(SerializedColumns.ContainsKey(column.Name)) continue;
				res.Add(column.Name);
			}
			return res;
		}
		public virtual bool IsContinuedFromPrevPage(int visibleIndex, int visibleStartIndex) {
			int level = GetRowLevel(visibleIndex);
			if(level == 0) return false;
			for(int n = visibleIndex - 1; n >= visibleStartIndex; n--) {
				int rowLevel = GetRowLevel(n);
				if(rowLevel > level) return false;
			}
			return true;
		}
		public virtual void SetRowState(byte[] rowsState) {
			if(rowsState != null && rowsState.Length == 0) rowsState = null;
			this.rowsState = rowsState;
		}
		protected byte[] RowsState { get { return rowsState; } set { rowsState = value; } }
		protected internal virtual void Assign(WebDataProviderBase source, bool withLoadData) {
			if(source == null) return;
			this.rowsState = source.RowsState;
	   }
		public virtual byte[] SaveData(List<string> usedFields, int visibleStartIndex, int pageSize) {
			foreach(string usedField in usedFields) {
				AddFieldNameIntoSerializedList(usedField);
			}
			using(MemoryStream stream = new MemoryStream())
			using(var writer = new TypedBinaryWriter(stream)) {
				writer.Write(GroupCount);
				writer.Write(VisibleCount);
				writer.Write(ListSourceRowCount);
				writer.Write(visibleStartIndex);
				writer.Write(pageSize);
				writer.Write(IsFiltered);
				List<DataColumnInfo> savedColumns = GetSavedColums(GetSerializedColumns());
				SaveColumns(writer, savedColumns);
				SaveNonUsedColumns(writer, GetNonUsedColumns());
				SaveColumnPropertyTypes(writer, GetColumnPropertyTypes());
				SaveTotalSummary(writer);
				SaveConditionalFormatinSummary(writer);
				SaveParentGroupRows(writer, savedColumns, visibleStartIndex);
				SaveVisibleRows(writer, savedColumns, visibleStartIndex, pageSize);
				return stream.ToArray();
			}
		}
		protected virtual void SaveColumnPropertyTypes(TypedBinaryWriter writer, Dictionary<string, Type> list) {
			int count = Proxy.Owner.CacheFilterBuilderColumns ? list.Count : 0;
			writer.Write(count);
			var propertyTypes = list.ToArray();
			for(int i = 0; i < count; i++) {
				writer.Write(propertyTypes[i].Key);
				writer.WriteType(propertyTypes[i].Value);
			}
		}
		void SaveNonUsedColumns(TypedBinaryWriter writer, List<string> list) {
			writer.Write(list.Count);
			foreach(string name in list) {
				writer.Write(name);
			}
		}
		protected virtual void SaveTotalSummary(TypedBinaryWriter writer) {
			SaveSummary(writer, GetTotalSummary());
		}
		protected virtual void SaveConditionalFormatinSummary(TypedBinaryWriter writer) {
			SaveSummary(writer, GetFormatConditionsSummary(), item => !(item is Array));
		}
		protected virtual void SaveSummary(TypedBinaryWriter writer, List<object> summary, Func<object, bool> isSavePossibleHandler = null) {
			writer.WriteObject(summary.Count);
			for(int n = 0; n < summary.Count; n++) {
				object value = isSavePossibleHandler == null || isSavePossibleHandler(summary[n]) ? summary[n] : null;
				writer.WriteTypedObject(value);
			}
		}
		void SaveColumns(TypedBinaryWriter writer, List<DataColumnInfo> savedColumns) {
			writer.Write(savedColumns.Count);
			foreach(DataColumnInfo columnInfo in savedColumns) {
				SaveColumn(writer, columnInfo);
			}
		}
		void SaveColumn(TypedBinaryWriter writer, DataColumnInfo columnInfo) {
			writer.Write(columnInfo.Name);
			writer.Write(columnInfo.Caption);
			writer.WriteType(columnInfo.Type);
			writer.Write(columnInfo.Unbound);
			writer.Write(columnInfo.ReadOnly);
			writer.Write(AllowSearchPanelFilter(columnInfo));
		}
		protected abstract void SaveParentGroupRows(TypedBinaryWriter writer, List<DataColumnInfo> savedColumns, int visibleStartIndex);
		void SaveVisibleRows(TypedBinaryWriter writer, List<DataColumnInfo> savedColumns, int visibleStartIndex, int pageSize) {
			for(int i = 0; i < pageSize; i++) {
				SaveVisibleRow(writer, savedColumns, i + visibleStartIndex, visibleStartIndex, pageSize);
			}
		}
		void SaveVisibleRow(TypedBinaryWriter writer, List<DataColumnInfo> columns, int visibleIndex, int visibleStartIndex, int pageSize) {
			WebRowType rowType = GetRowType(visibleIndex);
			writer.WriteObject(rowType);
			List<int> footerParentGroups = IsLastRowInCurrentLevel(visibleIndex) ? GetFooterParentGroupRows(visibleIndex) : null;
			int parentGroupCount = footerParentGroups != null ? footerParentGroups.Count : 0;
			writer.WriteObject(parentGroupCount);
			for(int i = 0; i < parentGroupCount; i++) {
				writer.WriteObject(footerParentGroups[i]);
			}
			writer.WriteObject(RowIsLastInLevel(visibleIndex));
			switch(rowType) {
				case WebRowType.Group:
					SaveGroupRow(writer, columns, visibleIndex, visibleStartIndex, pageSize);
					break;
				default:
					SaveDataRow(writer, columns, visibleIndex);
					break;
			}
		}
		void SaveGroupRow(TypedBinaryWriter writer, List<DataColumnInfo> columns, int visibleIndex, int visibleStartIndex, int pageSize) {
			writer.Write(GetRowLevel(visibleIndex));
			writer.WriteObject(IsGroupRowFitOnPage(visibleIndex, visibleStartIndex, pageSize));
			writer.WriteObject(IsRowExpanded(visibleIndex));
			writer.WriteObject(GetChildDataRowCount(visibleIndex));
			writer.WriteTypedObject(GetRowValue(visibleIndex, string.Empty, false));
			SaveSummary(writer, GetGroupSummary(visibleIndex));
			bool saveGroupRowData = !Proxy.IsServerMode; 
			writer.WriteObject(saveGroupRowData);
			if(saveGroupRowData)				
				SaveDataRow(writer, columns, visibleIndex);			
		}
		void SaveDataRow(TypedBinaryWriter writer, List<DataColumnInfo> columns, int visibleIndex) {
			foreach(DataColumnInfo column in columns) {
				object val = GetRowValue(visibleIndex, column.Name, false);
				if(column.Type != typeof(object) && column.Unbound) {
					try {
						val = column.ConvertValue(val);
					} catch {
						val = null;
					}
				}
				writer.WriteTypedObject(val);
			}
		}
		protected void SaveParentGroupRow(TypedBinaryWriter writer, List<DataColumnInfo> columns, int visibleIndex, int level, object groupValue, int childDataRowCount, List<object> summary) {
			writer.WriteObject(visibleIndex);
			writer.WriteObject(level);
			writer.WriteObject(childDataRowCount);
			writer.WriteTypedObject(groupValue);
			SaveSummary(writer, summary);
			SaveDataRow(writer, columns, visibleIndex);
		}
		public virtual void CollapseRow(int visibleIndex, bool recursive) {
			throw new NotImplementedException();
		}
		public virtual void ExpandRow(int visibleIndex, bool recursive) {
			throw new NotImplementedException();
		}
		public virtual object GetListSourceRowValue(int listSourceRowIndex, string fieldName) {
			throw new NotImplementedException();
		}
		public virtual IListServer GetServerModeListSource() {
			return null;
		}
		protected internal virtual void UpdateColumnBindings() { }
		public virtual void Dispose() {
		}
		protected internal virtual void ForceDataRowType() { }
		protected UnboundColumnInfoCollection GetUnboundColumns() {
			UnboundColumnInfoCollection res = new UnboundColumnInfoCollection();
			foreach (IWebColumnInfo col in Proxy.Owner.GetColumns()) {
				if (col.UnboundType == UnboundColumnType.Bound || string.IsNullOrEmpty(col.FieldName)) continue;
				res.Add(new UnboundColumnInfo(col.FieldName, col.UnboundType, col.ReadOnly, col.UnboundExpression));
			}
			if (res.Count > 0) return res;
			return null;
		}
		protected ComplexColumnInfoCollection GetComplexColumns() {
			ComplexColumnInfoCollection res = new ComplexColumnInfoCollection();
			foreach (IWebColumnInfo column in Proxy.Owner.GetColumns()) {
				if (column.UnboundType != UnboundColumnType.Bound) continue;
				if (column.FieldName.Contains(".") && !HasFieldName(column.FieldName))
					res.Add(column.FieldName);
			}
			return res;
		}
		protected virtual PropertyDescriptorCollection PatchPropertyDescriptorCollection(PropertyDescriptorCollection collection) {
			List<IWebColumnInfo> columns = Proxy.Owner.GetColumns();
			if (columns == null || collection == null)
				return collection;
			foreach (IWebColumnInfo col in columns) {
				if (col.UnboundType != UnboundColumnType.Bound) continue;
				if (collection.Find(col.FieldName, false) == null)
					collection.Find(col.FieldName, true);
			}
			if (collection.Find(Proxy.KeyFieldName, false) == null)
				collection.Find(Proxy.KeyFieldName, true);
			return collection;
		}
	}
	public class WebDataControllerProvider : WebDataProviderBase, IDataControllerData2 {
		BaseListSourceDataController dataController;
		int sortGroupChangedInProgress = 0;
		public WebDataControllerProvider(WebDataProxy proxy) : base(proxy) {
			CreateDataController(null);
		}
		protected bool IsSortGroupChangedInProgress { get { return sortGroupChangedInProgress != 0; } }
		public override void Dispose() {
			base.Dispose();
			if(DataController != null) DataController.Dispose();
		}
		protected internal override void ForceDataRowType() {
			if(Proxy == null) return;
			DataController.ForcedDataRowType = Proxy.forcedDataRowType;
		}
		bool IsServerModeSource(object dataSource) {
			IListSource source = dataSource as IListSource;
			if(source != null) dataSource = source.GetList();
			return dataSource is IListServer;
		}
		void CreateDataController(object dataSource) {
			if(this.dataController != null) {
				this.dataController.SortClient = null;
				this.dataController.DataClient = null;
				this.dataController.CustomSummary -= new CustomSummaryEventHandler(OnCustomSummary);
				this.dataController.CustomSummaryExists -= new CustomSummaryExistEventHandler(OnCustomSummaryExists);
			}
			if(IsServerModeSource(dataSource)) {
				this.dataController = new ServerModeDataController();
				((ServerModeDataController)this.dataController).AllowCurrentControllerRow = false;
			}
			else {
				this.dataController = new ListSourceDataController();
			}
			this.dataController.ForcedDataRowType = Proxy.forcedDataRowType;
			this.dataController.AllowNotifications = false;
			this.dataController.DataClient = this;
			this.dataController.SortClient = Proxy.Owner.SortClient;
			this.dataController.SetDataSource(dataSource);
			this.dataController.CustomSummaryExists += new CustomSummaryExistEventHandler(OnCustomSummaryExists);
			this.dataController.CustomSummary += new CustomSummaryEventHandler(OnCustomSummary);
		}
		public object DataSource { get { return DataController.ListSource; } }
		public bool GetServerMode() {
			return (DataSource is IListServer);
		}
		public override bool ServerMode { get { return GetServerMode(); } }
		protected IWebDataEvents Events { get { return Proxy.Events; } }
		public DataController DataController { get { return dataController; } }
		public override void SetDataSource(object dataSource) {
			byte[] savedRowState = null;
			try {
				if(DataController.IsReady) {
					using(MemoryStream ms = new MemoryStream()) {
						SaveRowState(ms);
						savedRowState = ms.ToArray();
					}
				}
			} catch(ObjectDisposedException) {
			}
			CreateDataController(dataSource);
			if(savedRowState != null)
				SetRowState(savedRowState);
		}
		protected override void SaveParentGroupRows(TypedBinaryWriter writer, List<DataColumnInfo> savedColumns, int visibleIndex) {
			List<int> parentRows = GetParentGroupRows(visibleIndex);
			writer.WriteObject(parentRows.Count);
			for(int n = 0; n < parentRows.Count; n ++) {
				GroupRowInfo group = DataController.GroupInfo.GetGroupRowInfoByHandle(DataController.GetControllerRowHandle(parentRows[n]));
				SaveParentGroupRow(writer, savedColumns, parentRows[n], group.Level, DataController.GetGroupRowValue(group), 
					group.ChildControllerRowCount, GetGroupSummaryCore(group.Handle));
			}
		}
		protected override List<object> GetTotalSummary() {
			List<object> res = new List<object>();
			foreach(SummaryItem item in DataController.TotalSummary) {
				if(item.Tag is FormatConditionSummary) continue;
				res.Add(item.SummaryValue);
			}
			return res;
		}
		protected override List<object> GetFormatConditionsSummary() {
			return DataController.TotalSummary.OfType<SummaryItem>()
				.Where(i => i.Tag is FormatConditionSummary)
				.Select(s => s.SummaryValue)
				.ToList();
		}
		public override Dictionary<string, Type> GetColumnPropertyTypes() {
			var columns = new List<DataColumnInfo>();
			columns.AddRange(DataController.Columns);
			columns.AddRange(DataController.DetailColumns);
			return columns.ToDictionary(col => col.PropertyDescriptor.Name, col => col.PropertyDescriptor.PropertyType);
		}
		protected override List<object> GetGroupSummary(int visibleIndex) {
			return GetGroupSummaryCore(GetControllerRowHandle(visibleIndex));
		}
		List<object> GetGroupSummaryCore(int controllerRowHandle) {
			Hashtable hash = DataController.GetGroupSummary(controllerRowHandle);
			List<object> res = new List<object>();
			if(hash == null) return res;
			foreach(SummaryItem item in DataController.GroupSummary) {
				if(hash.ContainsKey(item.Key))
					res.Add(hash[item.Key]);
				else
					res.Add(DBNull.Value);
			}
			return res;
		}
		protected internal override DataColumnInfoCollection Columns { get { return DataController.Columns; } }
		public override bool IsReady { get { return DataController.IsReady; } }
		public override int VisibleCount { get { return DataController.VisibleCount;  } }
		public override int ListSourceRowCount { get { return DataController.ListSourceRowCount; } }
		public override string FilterExpression {
			get { return DataController.FilterExpression; }
		}
		protected GroupRowInfo GetGroupRowInfo(int visibleIndex) {
			return DataController.GroupInfo.GetGroupRowInfoByHandle(GetControllerRowHandle(visibleIndex));
		}
		protected int GetControllerRowHandle(int visibleIndex) {
			return DataController.GetControllerRowHandle(visibleIndex);
		}
		public override object GetTotalSummaryValue(ASPxSummaryItemBase item) {
			SummaryItem dcItem = DataController.TotalSummary.GetSummaryItemByKey(item);
			if(dcItem == null) return null;
			return dcItem.SummaryValue;
		}
		public override object GetFormatConditionSummaryValue(FormatConditionSummary item) {
			SummaryItem dcItem = DataController.TotalSummary.GetSummaryItemByKey(item);
			return dcItem != null ? dcItem.SummaryValue : null;
		}
		public override bool IsGroupSummaryExists(int visibleIndex, ASPxSummaryItem item) {
			Hashtable summary = DataController.GetGroupSummary(GetControllerRowHandle(visibleIndex));
			if(summary == null) return false;
			return summary.ContainsKey(item);
		}
		public override object GetGroupSummaryValue(int visibleIndex, ASPxSummaryItem item) {
			Hashtable summary = DataController.GetGroupSummary(GetControllerRowHandle(visibleIndex));
			if(summary == null) return null;
			return summary[item];
		}
		public override bool IsExpressionFitToRule(string expression, int visibleIndex) {
			int controllerRow = GetControllerRowHandle(visibleIndex);
			if(DataController.IsGroupRowHandle(controllerRow))
				return false;
			Exception exc;
			var evaluator = DataController.CreateExpressionEvaluator(CriteriaOperator.TryParse(expression), true, out exc);
			int listSourceRowIndex = DataController.GetListSourceRowIndex(controllerRow);
			try {
				object isFit = evaluator.Evaluate(listSourceRowIndex);
				return isFit is bool ? (bool)isFit : Convert.ToBoolean(isFit);
			} catch {
				return false;
			}
		}
		public override object GetRowForTemplate(int visibleIndex) {
			if(visibleIndex == ListSourceDataController.NewItemRow) return new WebDataRow(this, visibleIndex);
			if(!IsValidRow(visibleIndex)) return null;
			if(GetRowType(visibleIndex) == WebRowType.Group) {
				return new WebDataGroupRow(this, visibleIndex);
			}
			return new WebDataRow(this, visibleIndex);
		}
		public override object GetRow(int visibleIndex) { 
			if(visibleIndex == ListSourceDataController.NewItemRow) return new WebDataRow(this, visibleIndex);
			return DataController.GetRow(GetControllerRowHandle(visibleIndex)); 
		}
		public override WebRowType GetRowType(int visibleIndex) {
			int controllerRow = GetControllerRowHandle(visibleIndex);
			if(DataController.IsGroupRowHandle(controllerRow)) return WebRowType.Group;
			return WebRowType.Data;
		}
		public override int GetChildDataRowCount(int visibleIndex) {
			GroupRowInfo groupRowInfo = GetGroupRowInfo(visibleIndex);
			if(groupRowInfo == null) return 0;
			return groupRowInfo.ChildControllerRowCount;
		}
		public override object GetRowValue(int visibleIndex, string fieldName, bool isDesignTime) {
			AddFieldNameIntoSerializedList(fieldName);
			int controllerRow = GetControllerRowHandle(visibleIndex);
			if(DataController.IsGroupRowHandle(controllerRow)) {
				if(string.IsNullOrEmpty(fieldName)) return DataController.GetGroupRowValue(controllerRow);
				controllerRow = DataController.GetControllerRowByGroupRow(controllerRow);
			}
			return GetRowValueByControllerRow(controllerRow, fieldName, isDesignTime);
		}
		protected object GetRowValueByControllerRow(int controllerRow, string fieldName, bool isDesignTime) {
			if (string.IsNullOrEmpty(fieldName)) return null;
			if(ServerMode && fieldName == Proxy.KeyFieldName)
				return DataController.GetRowKey(controllerRow);
			DataColumnInfo colInfo = DataController.Columns[fieldName];
			if (colInfo == null && !isDesignTime && dataController.IsReady)
				throw new HttpException(string.Format("A field or property with name '{0}' was not found in the selected data source. "
					+ "Possible causes of this error may be the following: an incorrect or case-insensitive spelling of the grid column name; assigning a wrong or not properly initialized data source to the grid.", fieldName));
			return colInfo != null ? DataController.GetRowValue(controllerRow, colInfo) : null;
		}
		public override object GetListSourceRowValue(int listSourceRowIndex, string fieldName) {
			if(ServerMode && fieldName == Proxy.KeyFieldName) {
				int controllerRow = DataController.GetControllerRow(listSourceRowIndex);
				return DataController.GetRowKey(controllerRow);
			}
			return DataController.GetListSourceRowValue(listSourceRowIndex, fieldName);
		}
		public override IListServer GetServerModeListSource() {
			return DataController.ListSource as IListServer;
		}
		public override int FindRowByKey(string keyFieldName, object value, bool expandGroups) {
			int controllerRowHandle = DataController.FindRowByValue(keyFieldName, value);
			if(expandGroups)
				DataController.MakeRowVisible(controllerRowHandle);
			return DataController.GetVisibleIndex(controllerRowHandle);
		}
		public override int FindRowByKeys(Dictionary<string, object> columnValues, bool expandGroups) {
			if (columnValues == null) return -1;
			int controllerRowHandle = DataController.FindRowByValues(columnValues);
			if(expandGroups)
				DataController.MakeRowVisible(controllerRowHandle);
			return DataController.GetVisibleIndex(controllerRowHandle);
		}
		public int GetChildRowCount(int groupRowVisibleIndex) {
			int controllerRow = GetControllerRowHandle(groupRowVisibleIndex);
			if(DataController.IsGroupRowHandle(controllerRow)) {
				return DataController.GroupInfo.GetChildCount(controllerRow);
			}
			return 0;
		}
		public object GetChildRow(int groupRowVisibleIndex, int childIndex) {
			int controlerRow = GetChildRowControllerHandle(groupRowVisibleIndex, childIndex);
			if (!DataController.IsValidControllerRowHandle(controlerRow)) return null;
			return DataController.GetRow(controlerRow);
		}
		public object GetChildRowValues(int groupRowVisibleIndex, int childIndex, string[] fieldNames) {
			if (fieldNames == null || fieldNames.Length == 0) return null;
			int controlerRow = GetChildRowControllerHandle(groupRowVisibleIndex, childIndex);
			if (!DataController.IsValidControllerRowHandle(controlerRow)) return null;
			if (fieldNames.Length == 1) return GetRowValueByControllerRow(controlerRow, fieldNames[0], false);
			object[] values = new object[fieldNames.Length];
			for(int i = 0; i < fieldNames.Length; i ++) {
				values[i] = GetRowValueByControllerRow(controlerRow, fieldNames[i], false);
			}
			return values;
		}
		int GetChildRowControllerHandle(int groupRowVisibleIndex, int childIndex) {
			int controllerRow = GetControllerRowHandle(groupRowVisibleIndex);
			if (DataController.IsGroupRowHandle(controllerRow)) {
				return DataController.GroupInfo.GetChildRow(controllerRow, childIndex);
			}
			return ListSourceDataController.InvalidRow;
		}
		public IList GetChildKeysRecursive(int groupRowVisibleIndex) {
			ArrayList list = new ArrayList();
			PopulateChildKeysRecursive(GetControllerRowHandle(groupRowVisibleIndex), list);
			return list;
		}
		void PopulateChildKeysRecursive(int groupRowHandle, IList list) {
			GroupRowInfo group = DataController.GroupInfo.GetGroupRowInfoByHandle(groupRowHandle);
			int childCount = group.ChildControllerRowCount;
			for(int i = 0; i < childCount; i++) {
				int childRowHandle = DataController.GroupInfo.GetChildRow(group, i);
				if(DataController.IsGroupRowHandle(childRowHandle))
					PopulateChildKeysRecursive(childRowHandle, list);
				else
					list.Add(GetKeyValueByControllerRow(childRowHandle));
			}
		}
		object GetKeyValueByControllerRow(int listSourceRowIndex) {
			if(!Proxy.IsMultipleKeyFields)
				return GetRowValueByControllerRow(listSourceRowIndex, Proxy.KeyFieldName, Proxy.Owner.IsDesignTime);
			return Proxy.KeyFieldNames.Select(field => GetRowValueByControllerRow(listSourceRowIndex, field, Proxy.Owner.IsDesignTime)).ToArray();
		}
		public override bool IsUnboundField(string fieldName) {
			if(!HasFieldName(fieldName)) return false;
			DataColumnInfo column = DataController.Columns[fieldName];
			return column != null && column.Unbound;
		}
		public override bool HasFieldName(string fieldName) {
			if(DataController.Columns[fieldName] != null) {
				AddFieldNameIntoSerializedList(fieldName);
				return true;
			}
			return false;
		}
		public override Type GetFieldTypeCore(string fieldName) {
			return DataController.Columns[fieldName].Type;
		}
		public override int GetRowLevel(int visibleIndex) {
			int controllerRow = GetControllerRowHandle(visibleIndex);
			return DataController.GetRowLevel(controllerRow);
		}
		public override void SaveRowState(Stream stream) {
			DataController.SaveRowState(stream);
		}
		public override void RestoreRowsState() {
			if(RowsState == null || IsSortGroupChangedInProgress)
				return;
			using(MemoryStream stream = new MemoryStream(RowsState)) {
				DataController.RestoreRowState(stream);
				RowsState = null;
			}
		}
		public override void SortGroupChanged(List<IWebColumnInfo> sortList, int groupCount, string filterExpression, IList groupSummary, IList totalSummary, IList formatConditionSummary) { 
			List<DataColumnSortInfo> infoList = new List<DataColumnSortInfo>();
			foreach(IWebColumnInfo webInfo in sortList) {
				DataColumnInfo colInfo = DataController.Columns[webInfo.FieldName];
				if(colInfo == null) continue;
				infoList.Add(new DataColumnSortInfo(colInfo, webInfo.SortOrder, webInfo.GroupInterval));
			}
			this.sortGroupChangedInProgress++;
			DataController.BeginUpdate();
			try {
				try {
					DataController.FilterExpression = filterExpression;
				} catch {
				}
				SynchronizeSummary(DataController.TotalSummary, totalSummary, formatConditionSummary);
				SynchronizeSummary(DataController.GroupSummary, groupSummary);
				DataController.UpdateSortGroup(infoList.ToArray(), groupCount, new SummarySortInfo[0]);
				DataController.SummarySortInfo.ClearAndAddRange(GetSummarySortInfo(groupSummary));
				SetGroupCount(DataController.SortInfo.GroupCount);
			}
			finally {
				DataController.EndUpdate();
				this.sortGroupChangedInProgress--;
			}
		}
		protected internal SummarySortInfo[] GetSummarySortInfo(IList groupSummary) {
			List<SummarySortInfo> res = new List<SummarySortInfo>();
			foreach(ASPxGroupSummarySortInfo info in Proxy.GroupSummarySortInfo) {
				if(info.SummaryItem == null || info.SummaryItem.SummaryType == SummaryItemType.None) continue;
				int summaryIndex = groupSummary.IndexOf(info.SummaryItem);
				if(summaryIndex < 0 || summaryIndex >= DataController.GroupSummary.Count) continue;
				SummaryItem summary = DataController.GroupSummary[summaryIndex];
				DataColumnInfo column = Columns[info.GroupColumn];
				if(column == null) continue;
				int groupLevel = DataController.SortInfo.GetGroupIndex(column);
				if(groupLevel < 0) continue;
				res.Add(new SummarySortInfo(summary, groupLevel, info.SortOrder));
			}
			return res.ToArray();
		}
		void SynchronizeSummary(SummaryItemCollection controllerSummary, params IList[] summaryList) {
			controllerSummary.Clear();
			if(summaryList == null || summaryList.Length == 0) return;
			foreach(IList summary in summaryList)
			foreach(ASPxSummaryItemBase item in summary) {
				controllerSummary.Add(new SummaryItem(DataController.Columns[item.FieldName], item.SummaryType, item));
			}
		}
		public override void ExpandAll() {
			DataController.ExpandAll();
		}
		public override void CollapseAll() {
			DataController.CollapseAll();
		}
		public override void CollapseRow(int visibleIndex, bool recursive) {
			DataController.CollapseRow(GetControllerRowHandle(visibleIndex), recursive);
		}
		public override void ExpandRow(int visibleIndex, bool recursive) {
			DataController.ExpandRow(GetControllerRowHandle(visibleIndex), recursive);
		}
		public override bool IsRowExpanded(int visibleIndex) {
			return DataController.IsRowExpanded(GetControllerRowHandle(visibleIndex));
		}
		protected override List<DataColumnInfo> GetSavedColums(List<string> usedFields) {
			List<DataColumnInfo> list = new List<DataColumnInfo>();
			foreach(DataColumnInfo columnInfo in DataController.Columns) {
				if(usedFields.IndexOf(columnInfo.Name) < 0) continue;
				list.Add(columnInfo);
			}
			return list;
		}
		protected internal override void UpdateColumnBindings() {
			DataController.RePopulateColumns();
		}
		public override bool AllowSearchPanelFilter(DataColumnInfo column) {
			return DataController.CanFindColumn(column);
		}
		#region IDataControllerData Members
		UnboundColumnInfoCollection IDataControllerData.GetUnboundColumns() {
			return base.GetUnboundColumns();
		}
		object IDataControllerData.GetUnboundData(int listSourceRow, DataColumnInfo column, object value) {
			if (Events == null)
				return value;
			return Events.GetUnboundData(listSourceRow, column.Name, value);
		}
		void IDataControllerData.SetUnboundData(int listSourceRow, DataColumnInfo column, object value) {
			if(Events != null) 
				Events.SetUnboundData(listSourceRow, column.Name, value);
		}
		#endregion
		protected void OnCustomSummaryExists(object sender, CustomSummaryExistEventArgs e) {
			if(Events != null) Events.OnSummaryExists(e);
		}
		protected void OnCustomSummary(object sender, CustomSummaryEventArgs e) {
			var conditionSummary = e.Item as FormatConditionSummary;
			if(conditionSummary != null) {
				ProcessCustomFormatConditionSummary(e);
				return;
			}
			if(Events != null)
				Events.OnCustomSummary(e);
		}
		protected void ProcessCustomFormatConditionSummary(CustomSummaryEventArgs e) {
			if(e.SummaryProcess != CustomSummaryProcess.Start)
				return;
			var conditionSummary = e.Item as FormatConditionSummary;
			var column = DataController.Columns[conditionSummary.FieldName];
			if(column != null && (typeof(IComparable).IsAssignableFrom(column.GetDataType()) || column.UnboundWithExpression))
				e.TotalValue = GetSortedListIndices(column);
			e.TotalValueReady = true;
		}
		int[] GetSortedListIndices(DataColumnInfo column) {
			var existingCollection = DataController.GroupInfo.VisibleListSourceRows;
			var sortedListIndices = existingCollection.CloneThatWouldBeForSureModifiedAndOrForgottenBeforeAnythingHappensToOriginal();
			sortedListIndices.SortRows(new[] { new DataColumnSortInfo(column) });
			return FilterSortedListIndicesByNullValues(sortedListIndices.ToArray(), column);
		}
		int[] FilterSortedListIndicesByNullValues(int[] sortedListIndices, DataColumnInfo column) {
			if(sortedListIndices.Length == 0)
				return sortedListIndices;
			Type columnType = column.Type;
			if(columnType.IsValueType && Nullable.GetUnderlyingType(columnType) == null && !column.UnboundWithExpression)
				return sortedListIndices;
			int firstNonNullIndex = Array.FindIndex(sortedListIndices, i => GetListSourceRowValue(sortedListIndices[i], column.Name) != null);
			if(firstNonNullIndex == -1)
				return new int[0];
			if(firstNonNullIndex > 0) {
				int nonNullElementCount = sortedListIndices.Length - firstNonNullIndex;
				var filteredIndices = new int[nonNullElementCount];
				Array.Copy(sortedListIndices, firstNonNullIndex, filteredIndices, 0, nonNullElementCount);
				sortedListIndices = filteredIndices;
			}
			return sortedListIndices;
		}
		public override bool IsGroupRowFitOnPage(int visibleIndex, int visibleStartIndex, int pageSize) {
			GroupRowInfo group = GetGroupRowInfo(visibleIndex);
			if(group == null || !group.Expanded) return true;
			int remainRows = pageSize - (visibleIndex - visibleStartIndex);
			for(int n = visibleIndex + 1; n <= visibleIndex + remainRows; n++) {
				int level = GetRowLevel(n);
				if(level <= group.Level) return true;
			}
			return false;
		}
		public override List<int> GetParentGroupRows(int visibleIndex) {
			List<int> list = new List<int>();
			GroupRowInfo group = DataController.GetParentGroupRow(GetControllerRowHandle(visibleIndex));
			while(group != null) {
				list.Add(DataController.GetVisibleIndex(group.Handle));
				group = group.ParentGroup;
			}
			list.Reverse();
			return list;
		}
		public override List<int> GetFooterParentGroupRows(int visibleIndex) {
			List<int> list = new List<int>();
			GroupRowInfo group = DataController.GetParentGroupRow(GetControllerRowHandle(visibleIndex));
			while(group != null) {
				visibleIndex = DataController.GetVisibleIndex(group.Handle);
				list.Add(visibleIndex);
				if(!IsLastRowInCurrentLevel(visibleIndex)) break;
				group = group.ParentGroup;
			}
			return list;
		}
		public override object[] GetUniqueColumnValues(string fieldName, int maxCount, bool includeFilteredOut) {
			DataColumnInfo column = DataController.Columns[fieldName];
			if(column == null) return null;
			return DataController.FilterHelper.GetUniqueColumnValues(column.Index, maxCount, includeFilteredOut, true, null);
		}
		public override int RowIsLastInLevel(int visibleIndex) {
			int level = GetRowLevel(visibleIndex);
			if(level <= 0) return -1;
			if(GetRowType(visibleIndex) == WebRowType.Group) {
				GroupRowInfo groupRowInfo = GetGroupRowInfo(visibleIndex);
				if(groupRowInfo.ParentGroup == null) return 0;
				int totalChildGroupCount = DataController.GroupInfo.GetTotalChildrenGroupCount(groupRowInfo);
				if(groupRowInfo.Index + totalChildGroupCount >= DataController.GroupInfo.Count - 1) return groupRowInfo.ParentGroup.Level;
				GroupRowInfo nextGroup = DataController.GroupInfo[groupRowInfo.Index + totalChildGroupCount + 1];
				return groupRowInfo.Level > nextGroup.Level ? groupRowInfo.ParentGroup.Level : -1;
			}
			visibleIndex++;
			if(visibleIndex >= VisibleCount) return 0;
			int isLastInLevel = GetRowLevel(visibleIndex);
			return level > isLastInLevel ? isLastInLevel : -1;
		}
		public override void ValidateSelectedKeys() {
			if(Proxy.KeyFieldNames == null || ServerMode)
				return;
			ValidateSelectedKeys(Proxy.Selection.Selected);
		}
		public override Dictionary<object, bool> GetFilteredSelectedKeys() {
			if(ServerMode)
				return GetFilteredSelectedKeysInServerMode(Proxy.Selection.Selected);
			return GetValidSelectedKeys(Proxy.Selection.Selected, true);
		}
		public override bool IsValidKey(object serializedKey) {
			if(serializedKey == null || serializedKey == DBNull.Value)
				return false;
			if(ServerMode) 
				return true;
			var selectedKey = new Dictionary<object, bool> { { serializedKey, true } };
			return GetValidSelectedKeys(selectedKey, false).Count == 1;
		}
		void ValidateSelectedKeys(Dictionary<object, bool> selectedKeys) {
			var validSelectedKeys = GetValidSelectedKeys(selectedKeys, false);
			if(validSelectedKeys.Count != selectedKeys.Count) {
				selectedKeys.Clear();
				foreach(var keyPairValue in validSelectedKeys) {
					selectedKeys[keyPairValue.Key] = true;
				}
				Proxy.OnSelectionChanged();
			}
		}
		protected Dictionary<object, bool> GetValidSelectedKeys(Dictionary<object, bool> selectedKeys, bool validateInFilteredDataSource) {
			var listSourceDataController = (ListSourceDataController)DataController;
			if(listSourceDataController.ListSource == null)
				return selectedKeys;
			int checkedListSourceCount = validateInFilteredDataSource ? listSourceDataController.VisibleListSourceRowCount : listSourceDataController.ListSource.Count;
			Dictionary<object, bool> actualSelected = new Dictionary<object, bool>();
			for(var i = 0; (i < checkedListSourceCount) && (actualSelected.Count < selectedKeys.Count); i++) {
				var sourceKeyValue = Proxy.KeyFieldNames
					.Select(key => GetValueFromListDataSource(i, key, validateInFilteredDataSource))
					.ToArray<object>();
				object serializedKey = Proxy.IsMultipleKeyFields ? Proxy.ConvertMulitpleKeyValuesToString(sourceKeyValue) : sourceKeyValue[0];
				if(selectedKeys.ContainsKey(serializedKey))
					actualSelected[serializedKey] = true;
			}
			return actualSelected;
		}
		object GetValueFromListDataSource(int index, string fieldName, bool getValueInFilteredDataSource) {
			if(getValueInFilteredDataSource)
				return DataController.GetValueEx(index, fieldName);
			int columnIndex = DataController.Columns[fieldName].Index;
			return DataController.Helper.GetRowValue(index, columnIndex);
		}
		Dictionary<object, bool> GetFilteredSelectedKeysInServerMode(Dictionary<object, bool> selectedKeys) {
			var listSource = ((ServerModeDataController)DataController).ListSourceEx;
			var keyColumn = Proxy.KeyFieldNames.Select(k => Columns[k]).FirstOrDefault();
			if(listSource == null || keyColumn == null)
				return new Dictionary<object, bool>();
			var filteredSelectedKeys = new Dictionary<object, bool>();
			var columnCriteria = ServerModeDataControllerBase.DescriptorToCriteria(keyColumn);
			foreach (var keyPairValue in selectedKeys){
				int listIndex = listSource.LocateByValue(columnCriteria, keyPairValue.Key, -1, false);
				if(listIndex > -1)
					filteredSelectedKeys.Add(keyPairValue.Key, true);
			}
			return filteredSelectedKeys;
		}
		object[] GetKeyValue(object serializedKey) {
			if(!Proxy.IsMultipleKeyFields)
				return new object[] { serializedKey };
			string[] keys = ((string)serializedKey).Split(WebDataProxy.MultipleKeyValueSeparator);
			return Proxy.ConvertMultipleKeyValuesFromString(keys);
		}
		#region IDataControllerData2 Members
		bool IDataControllerData2.CanUseFastProperties { get { return !Proxy.Owner.IsDesignTime; } }
		ComplexColumnInfoCollection IDataControllerData2.GetComplexColumns() {
			return base.GetComplexColumns();
		}
		void IDataControllerData2.SubstituteFilter(SubstituteFilterEventArgs args) {
			if(Events == null)
				return;
			Events.SubstituteFilter(args);
		}
		bool IDataControllerData2.HasUserFilter { get { return false; } }
		bool? IDataControllerData2.IsRowFit(int listSourceRow, bool fit) { return null; }
		PropertyDescriptorCollection IDataControllerData2.PatchPropertyDescriptorCollection(PropertyDescriptorCollection collection) {
			return base.PatchPropertyDescriptorCollection(collection);
		}
		#endregion
	}
	class WebRowPropertyDescriptor : WebPropertyDescriptor {
		DataColumnInfo info;
		WebDescriptorRowBase dataRow;
		int columnIndex;
		public WebRowPropertyDescriptor(WebDescriptorRowBase dataRow, DataColumnInfo info, int columnIndex)
			: base(info.Name, info.Caption, info.Type, false, info.ReadOnly) {
			this.columnIndex = columnIndex;
			this.info = info;
			this.dataRow = dataRow;
		}
		public override object GetValue(object component) {
			return dataRow.GetValue(info, columnIndex);
		}
	}
	class WebDataColumnInfo : DataColumnInfo {
		public WebDataColumnInfo(WebPropertyDescriptor descriptor) : base(descriptor) {
			if(descriptor.Unbound) SetAsUnbound();
			CanFind = descriptor.CanFind;
		}
		public bool CanFind { get; private set; }
	}
	class FilterControlPropertyDescriptorsContainer : ITypedList {
		IEnumerable<PropertyDescriptor> PropertyDescriptors { get; set; }
		public FilterControlPropertyDescriptorsContainer(IEnumerable<PropertyDescriptor> propertyDescriptors) {
			PropertyDescriptors = propertyDescriptors;
		}
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) {
			return new PropertyDescriptorCollection(PropertyDescriptors.ToArray());
		}
		public string GetListName(PropertyDescriptor[] listAccessors) { return string.Empty; }
	}
	class FilterControlPropertyDescriptor : PropertyDescriptor {
		Type Type { get; set; }
		public FilterControlPropertyDescriptor(string name, Type type) : base(name, null) { 
			Type = type;
		}
		public override bool CanResetValue(object component) { throw new NotImplementedException(); }
		public override Type ComponentType { get { throw new NotImplementedException(); } }
		public override object GetValue(object component) { throw new NotImplementedException(); }
		public override bool IsReadOnly { get { throw new NotImplementedException(); } }
		public override Type PropertyType { get { return Type; } }
		public override void ResetValue(object component) { throw new NotImplementedException(); }
		public override void SetValue(object component, object value) { throw new NotImplementedException(); }
		public override bool ShouldSerializeValue(object component) { throw new NotImplementedException(); }
	}
	class WebNonUsedPropertyDescriptor : PropertyDescriptor {
		WebDataCachedProvider provider;
		public WebNonUsedPropertyDescriptor(WebDataCachedProvider provider, string name)
			: base(name, null) {
			this.provider = provider;
		}
		public override Type ComponentType { get { return typeof(Object); } }
		public override Type PropertyType { get { return typeof(object); } }
		public override bool CanResetValue(object component) { return false; }
		public override object GetValue(object component) {
			this.provider.Proxy.RequireDataBound();
			return null; 
		}
		public override void SetValue(object component, object value) { }
		public override bool IsReadOnly { get { return false; } }
		public override bool ShouldSerializeValue(object component) { return false; }
		public override void ResetValue(object component) { }
	}
	class WebPropertyDescriptor : PropertyDescriptor {
		Type propertyType;
		string name, displayName;
		bool unbound;
		bool readOnly;
		public WebPropertyDescriptor(string name, string displayName, Type propertyType, bool unbound, bool readOnly) 
			: this(name, displayName, propertyType, unbound, readOnly, false) { 
		}
		public WebPropertyDescriptor(string name, string displayName, Type propertyType, bool unbound, bool readOnly, bool canFind) : base(name, null) {
			this.displayName = displayName;
			this.unbound = unbound;
			this.name = name;
			this.propertyType = propertyType;
			this.readOnly = readOnly;
			CanFind = canFind;
		}
		public bool Unbound { get { return unbound; } }
		public bool CanFind { get; private set; }
		public override string DisplayName { get { return displayName; } }
		public override Type ComponentType { get { return typeof(Object); } }
		public override Type PropertyType { get { return propertyType; } }
		public override string Name { get { return name; } }
		public override bool CanResetValue(object component) { return false; }
		public override object GetValue(object component) { return null; }
		public override void SetValue(object component, object value) { }
		public override bool IsReadOnly { get { return readOnly; } }
		public override bool ShouldSerializeValue(object component) { return false; }
		public override void ResetValue(object component) { }
	}
	public abstract class WebDescriptorRowBase : ICustomTypeDescriptor {
		WebDataProviderBase owner;
		int visibleIndex;
		public WebDescriptorRowBase(WebDataProviderBase owner, int visibleIndex) {
			this.owner = owner;
			this.visibleIndex = visibleIndex;
		}
		public object this[string fieldName] {
			get { return owner.GetRowValue(VisibleIndex, fieldName, false); }
		}
		public int VisibleIndex { get { return visibleIndex; } }
		protected WebDataProviderBase Owner { get { return owner; } }
		#region ICustomTypeDescriptor Members
		System.ComponentModel.AttributeCollection ICustomTypeDescriptor.GetAttributes() { return new System.ComponentModel.AttributeCollection(null); }
		string ICustomTypeDescriptor.GetClassName() { return null; }
		string ICustomTypeDescriptor.GetComponentName() { return null; }
		TypeConverter ICustomTypeDescriptor.GetConverter() { return null; }
		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() { return null; }
		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() { return null; }
		object ICustomTypeDescriptor.GetEditor(Type editorBaseType) { return null; }
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes) { return new EventDescriptorCollection(null); }
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents() { return new EventDescriptorCollection(null); }
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() { return ((ICustomTypeDescriptor)this).GetProperties(null); }
		PropertyDescriptorCollection properties;
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes) {
			if(properties != null) return properties;
			List<PropertyDescriptor> pds = new List<PropertyDescriptor>();
			for(int n = 0; n < Owner.Columns.Count; n++) {
				DataColumnInfo info = Owner.Columns[n];
				pds.Add(new WebRowPropertyDescriptor(this, info, n));
			}
			AddNonUsedColumns(pds);
			this.properties = new PropertyDescriptorCollection(pds.ToArray());
			return this.properties;
		}
		protected virtual void AddNonUsedColumns(List<PropertyDescriptor> pds) {
		}
		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) { return this; }
		#endregion
		protected internal abstract object GetValue(DataColumnInfo info, int columnIndex);
	}
	public class WebDataRow : WebDescriptorRowBase, System.Xml.XPath.IXPathNavigable {
		public WebDataRow(WebDataProviderBase owner, int visibleIndex) : base(owner, visibleIndex) { }
		protected internal override object GetValue(DataColumnInfo info, int columnIndex) {
			return Owner.Proxy.GetRowValueForTemplate(VisibleIndex, info.Name);
		}
		#region IXPathNavigable Members
		System.Xml.XPath.XPathNavigator System.Xml.XPath.IXPathNavigable.CreateNavigator() {
			object row = Owner.GetRow(VisibleIndex);
			if(row is WebDataRow) return null;
			System.Xml.XPath.IXPathNavigable nav = row as System.Xml.XPath.IXPathNavigable;
			return nav == null ? null : nav.CreateNavigator();
		}
		#endregion
	}
	public class WebDataGroupRow : WebDataRow {
		public WebDataGroupRow(WebDataProviderBase owner, int visibleIndex) : base(owner, visibleIndex) { }
		protected internal override object GetValue(DataColumnInfo info, int columnIndex) {
			return Owner.GetRowValue(VisibleIndex, info.Name, false);
		}
	}
	public abstract class WebCachedRow : WebDescriptorRowBase {
		List<object> data;
		List<int> footerParentGroupRows;
		int rowLastInLevel;
		public WebCachedRow(WebDataCachedProvider owner, int visibleIndex) : base(owner, visibleIndex) {
			this.data = new List<object>(owner.Columns.Count);
			this.footerParentGroupRows = null;
		}
		public virtual object GetValue(int index) {
			if(index >= Data.Count) return null;
			return Data[index]; 
		}
		public abstract WebRowType RowType { get; }
		protected new WebDataCachedProvider Owner { get { return base.Owner as WebDataCachedProvider; } }
		protected internal int RowLastInLevel { get { return rowLastInLevel; } set { rowLastInLevel = value; } }
		protected internal void SetFooterParentGroupRows(List<int> groupRows) {
			this.footerParentGroupRows = new List<int>(groupRows);
		}
		protected internal List<int> GetFooterParentGroupRows() { return footerParentGroupRows != null ? footerParentGroupRows : new List<int>(); }
		protected internal List<object> Data { get { return data; } }
		public void Add(object value) { data.Add(value); }
		protected internal override object GetValue(DataColumnInfo info, int columnIndex) {
			return Owner.Proxy.GetRowValueForTemplate(VisibleIndex, info.Name);
		}
		protected override void AddNonUsedColumns(List<PropertyDescriptor> pds) {
			foreach(KeyValuePair<string, bool> pair in Owner.NonUsedColumns) {
				pds.Add(new WebNonUsedPropertyDescriptor(Owner, pair.Key));
			}
		}
	}
	public class WebCachedGroupRow : WebCachedRow {
		object value;
		int level;
		bool expanded, fitOnPage;
		int childDataRowCount;
		List<object> groupSummary;
		public WebCachedGroupRow(WebDataCachedProvider owner, int visibleIndex, int level, object value, bool expanded, int childDataRowCount, List<object> groupSummary, bool fitOnPage)
			: base(owner, visibleIndex) {
			this.fitOnPage = fitOnPage;
			this.value = value;
			this.level = level;
			this.expanded = expanded;
			this.childDataRowCount = childDataRowCount;
			this.groupSummary = groupSummary;
		}
		public bool FitOnPage { get { return fitOnPage; } }
		public bool Expanded { get { return expanded; } }
		public int Level { get { return level; } }
		public int ChildDataRowCount { get { return childDataRowCount; } }
		public List<object> GroupSummary { get { return groupSummary; } }
		public override object GetValue(int index) {
			if(index < 0) return value;
			return base.GetValue(index);
		}
		public override WebRowType RowType { get { return WebRowType.Group; } }
	}
	public class WebCachedParentGroupRow : WebCachedGroupRow {
		int visibleIndex;
		public WebCachedParentGroupRow(WebDataCachedProvider owner, int visibleIndex, int level, object value, int childDataRowCount, List<object> groupSummary)
			:
			base(owner, visibleIndex, level, value, true, childDataRowCount, groupSummary, false) {
			this.visibleIndex = visibleIndex;
		}
	}
	public class WebCachedDataRow : WebCachedRow {
		public WebCachedDataRow(WebDataCachedProvider owner, int visibleIndex) : base(owner, visibleIndex) { }
		public override WebRowType RowType { get { return WebRowType.Data; } }
	}
	public class WebDataCachedProvider : WebDataProviderBase {
		DataColumnInfoCollection columns;
		int visibleCount;
		int listSourceRowCount;
		int startIndex;
		int savedRowCount;
		bool isFiltered;
		List<WebCachedRow> data;
		List<object> totalSummary, formatConditionsSummary;
		List<WebCachedParentGroupRow> parentGroups;
		byte[] webData;
		Dictionary<string, bool> nonUsedColumns;
		Dictionary<string, Type> columnPropertyTypes;
		public WebDataCachedProvider(WebDataProxy proxy, byte[] webData) : base(proxy) {
			this.webData = webData;
			this.columns = new DataColumnInfoCollection();
			this.nonUsedColumns = new Dictionary<string, bool>();
			this.columnPropertyTypes = new Dictionary<string,Type>();
			this.data = new List<WebCachedRow>();
			this.parentGroups = new List<WebCachedParentGroupRow>();
			this.totalSummary = new List<object>();
			this.formatConditionsSummary = new List<object>();
			using(MemoryStream stream = new MemoryStream(webData)) {
				LoadData(stream);
			}
		}
		protected internal Dictionary<string, bool> NonUsedColumns { get { return nonUsedColumns; } }
		protected List<WebCachedParentGroupRow> ParentGroups { get { return parentGroups; } }
		protected internal override DataColumnInfoCollection Columns { get { return columns; } }
		protected internal List<WebCachedRow> Data { get { return data; } }
		protected List<object> TotalSummary { get { return totalSummary; } }
		protected List<object> FormatConditionsSummary { get { return formatConditionsSummary; } }
		public override int VisibleCount { get { return visibleCount; } }
		public override int ListSourceRowCount { get { return listSourceRowCount; } }
		protected override List<string> GetNonUsedColumns() { return new List<string>(NonUsedColumns.Keys); }
		protected override List<object> GetTotalSummary() { return TotalSummary; }
		protected override List<object> GetGroupSummary(int visibleIndex) {
			WebCachedGroupRow row = GetGroupRow(visibleIndex);
			return row == null ? new List<object>() : row.GroupSummary;
		}
		protected override List<object> GetFormatConditionsSummary() { return FormatConditionsSummary; }
		public override object GetRow(int visibleIndex) {
			return GetCachedRow(visibleIndex);
		}
		public override bool IsFiltered { get {  return isFiltered; } }
		protected WebCachedRow GetCachedRow(int visibleIndex) {
			if(visibleIndex == ListSourceDataController.NewItemRow) return new WebCachedDataRow(this, visibleIndex);
			if(visibleIndex < startIndex || visibleIndex >= startIndex + savedRowCount) {
				for(int n = 0; n < ParentGroups.Count; n++) {
					if(ParentGroups[n].VisibleIndex == visibleIndex) return ParentGroups[n];
				}
				return null;
			}
			return Data[visibleIndex - startIndex];
		}
		protected WebCachedGroupRow GetGroupRow(int visibleIndex) {
			return GetCachedRow(visibleIndex) as WebCachedGroupRow;
		}
		public override int GetChildDataRowCount(int visibleIndex) {
			WebCachedGroupRow groupRow = GetGroupRow(visibleIndex);
			return groupRow == null ? 0 : groupRow.ChildDataRowCount;
		}
		public override bool IsBound { get { return false; } }
		public override bool IsReady { get { return this.webData != null; } }
		public override WebRowType GetRowType(int visibleIndex) { 
			WebCachedRow row = GetCachedRow(visibleIndex);
			return row == null ? WebRowType.Data : row.RowType; 
		}
		public override byte[] SaveData(List<string> usedFields, int visibleStartIndex, int pageSize) {
			return this.webData;
		}
		public override bool IsGroupRowFitOnPage(int visibleIndex, int visibleStartIndex, int pageSize) {
			WebCachedGroupRow groupRow = GetGroupRow(visibleIndex);
			return groupRow != null && groupRow.FitOnPage;
		}
		public override List<int> GetParentGroupRows(int visibleIndex) {
			List<int> list = new List<int>();
			if(visibleIndex == startIndex) {
				for(int i = 0; i < ParentGroups.Count; i++) {
					list.Add(ParentGroups[i].VisibleIndex);
				}
			}
			return list;
		}
		public override List<int> GetFooterParentGroupRows(int visibleIndex) {
			WebCachedRow row = GetCachedRow(visibleIndex);
			return row != null ? row.GetFooterParentGroupRows() : new List<int>();
		}
		public override int RowIsLastInLevel(int visibleIndex) {
			WebCachedRow row = GetCachedRow(visibleIndex);
			return row != null ? row.RowLastInLevel : -1;
		}
		public override int GetRowLevel(int visibleIndex) {
			if(visibleIndex < 0) return 0;
			WebCachedGroupRow groupRow = GetGroupRow(visibleIndex);
			return groupRow == null ? GroupCount : groupRow.Level;
		}
		public override bool IsRowExpanded(int visibleIndex) {
			WebCachedGroupRow groupRow = GetGroupRow(visibleIndex);
			return groupRow == null ? true : groupRow.Expanded;
		}
		public override object GetTotalSummaryValue(ASPxSummaryItemBase item) {
			int i = item.Index;
			if(i >= 0 && i < TotalSummary.Count) return TotalSummary[i];
			return null;
		}
		public override object GetFormatConditionSummaryValue(FormatConditionSummary item) {
			int i = item.Index;
			return (i >= 0 && i < FormatConditionsSummary.Count) ?  FormatConditionsSummary[i] : null;
		}
		public override bool IsGroupSummaryExists(int visibleIndex, ASPxSummaryItem item) {
			List<object> groupSummary = GetGroupSummary(visibleIndex);
			int i = item.Index;
			if(i >= 0 && i < groupSummary.Count) return !(groupSummary[i] is DBNull);
			return false;
		}
		public override bool IsExpressionFitToRule(string expression, int visibleIndex) {
			return false;
		}
		public override object GetGroupSummaryValue(int visibleIndex, ASPxSummaryItem item) {
			if(!IsGroupSummaryExists(visibleIndex, item)) return null;
			List<object> groupSummary = GetGroupSummary(visibleIndex);
			int i = item.Index;
			if(i >= 0 && i < groupSummary.Count) return groupSummary[i];
			return null;
		}
		public override object GetRowValue(int visibleIndex, string fieldName, bool isDesignTime) {
			if(NonUsedColumns.ContainsKey(fieldName)) Proxy.RequireDataBound();
			WebCachedRow row = GetCachedRow(visibleIndex);
			if(row == null) return null;
			if(row is WebCachedGroupRow && string.IsNullOrEmpty(fieldName)) return row.GetValue(-1);
			DataColumnInfo columnInfo = Columns[fieldName];
			if(columnInfo == null) return null;
			return row.GetValue(columnInfo.Index);
		}
		public override int FindRowByKey(string keyFieldName, object value, bool expandGroups) {
			for(int n = startIndex; n < startIndex + savedRowCount; n++) {
				if(GetRowType(n) != WebRowType.Data) continue;
				object val = GetRowValue(n, keyFieldName, false);
				if(Object.Equals(val, value)) return n;
			}
			return -1;
		}
		public override int FindRowByKeys(Dictionary<string, object> columnValues, bool expandGroups) {
			if (columnValues == null) return -1;
			for (int n = startIndex; n < startIndex + savedRowCount; n++) {
				if (GetRowType(n) != WebRowType.Data) continue;
				if (IsRowValuesEquals(columnValues, n)) return n;
			}
			return -1;
		}
		public override void ValidateSelectedKeys() {
		}
		public override bool IsValidKey(object serializedKey) {
			return true;
		}
		public override Dictionary<object, bool> GetFilteredSelectedKeys() {
			return new Dictionary<object, bool>();
		}
		public override bool HasFieldName(string fieldName) { 
			return Columns[fieldName] != null || NonUsedColumns.ContainsKey(fieldName); 
		}
		public override Type GetFieldTypeCore(string fieldName) { return Columns[fieldName].Type; }
		public override bool IsUnboundField(string fieldName) {
			DataColumnInfo column = Columns[fieldName];
			return column != null && column.Unbound;
		}
		public override void SaveRowState(Stream stream) {
			if(RowsState != null) {
				stream.Write(RowsState, 0, RowsState.Length);
			}
		}
		public override void RestoreRowsState() {
		}
		protected override List<DataColumnInfo> GetSavedColums(List<string> usedFields) {
			List<DataColumnInfo> list = new List<DataColumnInfo>();
			foreach(DataColumnInfo columnInfo in Columns) {
				list.Add(columnInfo);
			}
			return list;
		}
		public override Dictionary<string, Type> GetColumnPropertyTypes() {
			return this.columnPropertyTypes;
		}
		protected override void SaveParentGroupRows(TypedBinaryWriter writer, List<DataColumnInfo> savedColumns, int visibleStartIndex) {
			writer.WriteObject(ParentGroups.Count);
			for(int n = 0; n < ParentGroups.Count; n++) {
				WebCachedParentGroupRow group = ParentGroups[n];
				SaveParentGroupRow(writer, savedColumns, group.VisibleIndex, group.Level, group.GetValue(0), group.ChildDataRowCount, group.GroupSummary);
			}
		}
		public override bool AllowSearchPanelFilter(DataColumnInfo column) {
			var webColumn = Columns[column.Name] as WebDataColumnInfo;
			if(webColumn != null)
				return webColumn.CanFind;
			return false;
		}
		protected virtual void LoadData(Stream stream) {
			using(var reader = new TypedBinaryReader(stream)) {
				SetGroupCount(reader.ReadInt32());
				this.visibleCount = reader.ReadInt32();
				this.listSourceRowCount = reader.ReadInt32();
				this.startIndex = reader.ReadInt32();
				this.savedRowCount = reader.ReadInt32();
				this.isFiltered = reader.ReadBoolean();
				LoadColumns(reader);
				LoadNonUsedColumns(reader);
				LoadColumnPropertyTypes(reader);
				LoadTotalSummary(reader);
				LoadFormatConditionsSummary(reader);
				LoadParentGroupRows(reader);
				LoadDataValues(reader);
			}
		}
		void LoadNonUsedColumns(TypedBinaryReader reader) {
			this.nonUsedColumns = new Dictionary<string, bool>();
			int count = reader.ReadInt32();
			for(int n = 0; n < count; n++) {
				string name = reader.ReadString();
				this.nonUsedColumns.Add(name, true);
			}
		}
		private void LoadColumnPropertyTypes(TypedBinaryReader reader) {
			this.columnPropertyTypes = new Dictionary<string,Type>();
			int count = reader.ReadInt32();
			for(int i = 0; i < count; i++)
				this.columnPropertyTypes.Add(reader.ReadString(), reader.ReadType());
		}
		protected void LoadParentGroupRows(TypedBinaryReader reader) {
			ParentGroups.Clear();
			int count = reader.ReadObject<int>();
			for(int n = 0; n < count; n++) {
				ParentGroups.Add(LoadParentGroupRow(reader));
			}
		}
		protected WebCachedParentGroupRow LoadParentGroupRow(TypedBinaryReader reader) {
			int visibleIndex = reader.ReadObject<int>();
			int level = reader.ReadObject<int>();
			int childRowCount = reader.ReadObject<int>();
			object value = reader.ReadTypedObject();
			List<object> summary = new List<object>();
			LoadSummary(reader, summary);
			WebCachedParentGroupRow row = new WebCachedParentGroupRow(this, visibleIndex, level, value, childRowCount, summary);
			LoadDataRowCore(reader, row);
			return row;
		}
		protected void LoadTotalSummary(TypedBinaryReader reader) {
			LoadSummary(reader, TotalSummary);
		}
		protected void LoadFormatConditionsSummary(TypedBinaryReader reader) {
			LoadSummary(reader, FormatConditionsSummary);
		}
		protected void LoadSummary(TypedBinaryReader reader, List<object> summary) {
			summary.Clear();
			int count = reader.ReadObject<int>();
			for(int i = 0; i < count; i++) {
				summary.Add(reader.ReadTypedObject());
			}
		}
		void LoadColumns(TypedBinaryReader reader) {
			int count = reader.ReadInt32();
			for(int i = 0; i < count; i++) {
				WebPropertyDescriptor descriptor = ReadPropertyDescriptor(reader);
				Columns.Add(new WebDataColumnInfo(descriptor));
			}
		}
		void LoadDataValues(TypedBinaryReader reader) {
			for(int i = 0; i < savedRowCount; i++) {
				LoadRecord(reader, i + this.startIndex);
			}
		}
		void LoadRecord(TypedBinaryReader reader, int visibleIndex) {
			WebRowType rowType = (WebRowType)reader.ReadObject(typeof(WebRowType));
			int parentGroupCount = (int)reader.ReadObject(typeof(int));
			List<int> parentGroupRows = parentGroupCount > 0 ? new List<int>() : null;
			for(int i = 0; i < parentGroupCount; i++) {
				parentGroupRows.Add((int)reader.ReadObject(typeof(int)));
			}
			int rowIsLastInLevel = reader.ReadObject<int>();
			WebCachedRow row = null;
			switch(rowType) {
				case WebRowType.Group: 
					row = LoadGroupRow(reader, visibleIndex);
					break;
				default:
					row = LoadDataRow(reader, visibleIndex);
					break;
			}
			if(parentGroupRows != null) {
				row.SetFooterParentGroupRows(parentGroupRows);
			}
			row.RowLastInLevel = rowIsLastInLevel;
		}
		WebCachedRow LoadGroupRow(TypedBinaryReader reader, int visibleIndex) {
			int level = reader.ReadInt32();
			bool fitOnPage = reader.ReadObject<bool>();
			bool expanded = reader.ReadObject<bool>();
			int childDataRowCount = reader.ReadObject<int>();
			object value = reader.ReadTypedObject();
			List<object> groupSummary = new List<object>();
			LoadSummary(reader, groupSummary);
			WebCachedRow row = new WebCachedGroupRow(this, visibleIndex, level, value, expanded, childDataRowCount, groupSummary, fitOnPage);
			if(reader.ReadObject<bool>())
				LoadDataRowCore(reader, row);
			Data.Add(row);
			return row;
		}
		WebCachedRow LoadDataRow(TypedBinaryReader reader, int visibleIndex) {
			WebCachedDataRow row = new WebCachedDataRow(this, visibleIndex);
			LoadDataRowCore(reader, row);
			Data.Add(row);
			return row;
		}
		void LoadDataRowCore(TypedBinaryReader reader, WebCachedRow row) {
			foreach(DataColumnInfo column in Columns)
				row.Add(reader.ReadTypedObject());
		}
		WebPropertyDescriptor ReadPropertyDescriptor(TypedBinaryReader reader) {
			string name = reader.ReadString();
			string displayName = reader.ReadString();
			Type propertyType = reader.ReadType();
			bool unbound = reader.ReadBoolean();
			bool readOnly = reader.ReadBoolean();
			bool canFind = reader.ReadBoolean();
			return new WebPropertyDescriptor(name, displayName, propertyType, unbound, readOnly, canFind);
		}
	}
	public interface IWebControlPageSettings {
		int PageSize { get; }
		int PageIndex { get; set; }
		DevExpress.Web.GridViewPagerMode PagerMode { get; }
	}
	public interface IWebDataOwner  {
		event EventHandler PageIndexChanged;
		bool IsDesignTime { get; }
		List<IWebColumnInfo> GetColumns();
		DataSourceView GetData();
		IWebControlObject WebControl { get; }
		Dictionary<string, object> GetEditTemplateValues();
		bool AllowOnlyOneMasterRowExpanded { get; }
		bool AllowSelectSingleRowOnly { get; }
		void RequireDataBound();
		IDataControllerSort SortClient { get; }
		bool IsForceDataSourcePaging { get; }
		DataSourceSelectArguments SelectArguments { get; }
		bool ValidateEditTemplates();
		bool ValidateAutoCreatedEditors();
		bool AllowFocusedRow { get; }
		GridEndlessPagingHelper EndlessPagingHelper { get; }
		GridBatchEditHelper BatchEditHelper { get; }
		bool AlwaysSaveSelectionViaSelectedKeys { get; }
		WebDataSelection CreateSelection(WebDataProxy proxy);
		IList CreateSummaryItemCollection();
		IList GetFormatConditionSummaries();
		bool UseSelectAll { get; }
		bool CacheFilterBuilderColumns { get; }
		ASPxStartItemEditingEventArgs CreateStartItemEditingArgs(object editingKey);
		ASPxGridDataValidationEventArgs CreateItemValidationEventArgs(int visibleIndex, bool isNew);
	}
	public interface IWebColumnInfo {
		ColumnSortOrder SortOrder { get; }
		string FieldName { get; }
		UnboundColumnType UnboundType { get; }
		string UnboundExpression { get; }
		bool ReadOnly { get; }
		ColumnGroupInterval GroupInterval { get; }
		EditPropertiesBase CreateEditProperties(); 
	}
	public interface IWebDataEvents {
		void OnFocusedRowChanged();
		void OnRowDeleting(ASPxDataDeletingEventArgs e);
		void OnRowDeleted(ASPxDataDeletedEventArgs e);
		void OnRowValidating(ASPxGridDataValidationEventArgs e);
		void OnInitNewRow(ASPxDataInitNewRowEventArgs e);
		void OnRowInserting(ASPxDataInsertingEventArgs e);
		void OnRowInserted(ASPxDataInsertedEventArgs e);
		void OnRowUpdating(ASPxDataUpdatingEventArgs e);
		void OnRowUpdated(ASPxDataUpdatedEventArgs e);
		void OnBatchUpdate(ASPxDataBatchUpdateEventArgs e);
		object GetUnboundData(int listSourceRowIndex, string fieldName, object value);
		void SetUnboundData(int listSourceRowIndex, string fieldName, object value);
		void SubstituteFilter(SubstituteFilterEventArgs args);
		void OnCustomSummary(CustomSummaryEventArgs e);
		void OnSummaryExists(CustomSummaryExistEventArgs e);
		void OnSelectionChanged();
		void OnDetailRowsChanged();
		void OnStartRowEditing(ASPxStartItemEditingEventArgs e);
		void OnCancelRowEditing(ASPxStartItemEditingEventArgs e);
		void OnParseValue(ASPxParseValueEventArgs e);
	}
	public class WebDataNullEvents : IWebDataEvents {
		void IWebDataEvents.OnParseValue(ASPxParseValueEventArgs e) { }
		void IWebDataEvents.OnFocusedRowChanged() { }
		void IWebDataEvents.OnRowDeleting(ASPxDataDeletingEventArgs e) { }
		void IWebDataEvents.OnRowDeleted(ASPxDataDeletedEventArgs e) { }
		void IWebDataEvents.OnRowValidating(ASPxGridDataValidationEventArgs e) { }
		void IWebDataEvents.OnInitNewRow(ASPxDataInitNewRowEventArgs e) { }
		void IWebDataEvents.OnRowInserting(ASPxDataInsertingEventArgs e) { }
		void IWebDataEvents.OnRowInserted(ASPxDataInsertedEventArgs e) { }
		void IWebDataEvents.OnRowUpdating(ASPxDataUpdatingEventArgs e) { }
		void IWebDataEvents.OnRowUpdated(ASPxDataUpdatedEventArgs e) { }
		void IWebDataEvents.OnBatchUpdate(ASPxDataBatchUpdateEventArgs e) { }
		object IWebDataEvents.GetUnboundData(int listSourceRowIndex, string fieldName, object value) { return value; }
		void IWebDataEvents.SetUnboundData(int listSourceRowIndex, string fieldName, object value) { }
		void IWebDataEvents.SubstituteFilter(SubstituteFilterEventArgs e) { }
		void IWebDataEvents.OnCustomSummary(CustomSummaryEventArgs e) { }
		void IWebDataEvents.OnSelectionChanged() { }
		void IWebDataEvents.OnDetailRowsChanged() { }
		void IWebDataEvents.OnStartRowEditing(ASPxStartItemEditingEventArgs e) { }
		void IWebDataEvents.OnCancelRowEditing(ASPxStartItemEditingEventArgs e) { }
		void IWebDataEvents.OnSummaryExists(CustomSummaryExistEventArgs e) { }
	}
	public class WebRegularDataProviderInfo {
		public int TotalRowCount;
		public int StartIndex;
		public int RowCount;
		public bool Ready = false;
		public WebRegularDataProviderInfo() {
			this.RowCount = this.StartIndex = this.TotalRowCount = 0;
		}
	}
	public class WebRegularDataProvider : WebDataProviderBase {
		WebRegularDataProviderInfo info;
		List<object> totalSummary;
		ListSourceDataController dataController;
		public WebRegularDataProvider(WebDataProxy proxy)
			: base(proxy) {
			this.info = new WebRegularDataProviderInfo();
			this.totalSummary = new List<object>();
			this.dataController = new ListSourceDataController();
		}
		public WebRegularDataProviderInfo Info { get { return info; } }
		protected internal override DataColumnInfoCollection Columns { get { return dataController.Columns; } }
		protected List<object> TotalSummary { get { return totalSummary; } }
		public override int VisibleCount { get { return Info.TotalRowCount; } }
		public override int ListSourceRowCount { get { return Info.TotalRowCount; } }
		protected override List<object> GetTotalSummary() { return TotalSummary; }
		protected override List<object> GetGroupSummary(int visibleIndex) {
			return new List<object>();
		}
		protected override List<object> GetFormatConditionsSummary() {
			return new List<object>();
		}
		public override object GetRow(int visibleIndex) {
			return GetRowCore(visibleIndex);
		}
		protected object GetRowCore(int visibleIndex) {
			if(visibleIndex == ListSourceDataController.NewItemRow) return null; 
			if(visibleIndex < Info.StartIndex || visibleIndex >= Info.StartIndex + Info.RowCount) {
				return null;
			}
			return dataController.GetRow(visibleIndex - info.StartIndex);
		}
		public override int GetChildDataRowCount(int visibleIndex) { return 0; }
		public override bool IsReady { get { return Info.Ready; } }
		public override WebRowType GetRowType(int visibleIndex) {
			return WebRowType.Data;
		}
		public override bool IsGroupRowFitOnPage(int visibleIndex, int visibleStartIndex, int pageSize) {
			return false;
		}
		public override List<int> GetParentGroupRows(int visibleIndex) {
			return new List<int>();
		}
		public override List<int> GetFooterParentGroupRows(int visibleIndex) {
			return new List<int>();
		}
		public override int RowIsLastInLevel(int visibleIndex) { return -1; }
		public override int GetRowLevel(int visibleIndex) { return 0;  }
		public override bool IsRowExpanded(int visibleIndex) { return true; }
		public override object GetTotalSummaryValue(ASPxSummaryItemBase item) {
			int i = item.Index;
			if(i >= 0 && i < TotalSummary.Count) return TotalSummary[i];
			return null;
		}
		public override object GetFormatConditionSummaryValue(FormatConditionSummary item) {
			return null;
		}
		public override bool IsGroupSummaryExists(int visibleIndex, ASPxSummaryItem item) {
			return false;
		}
		public override object GetGroupSummaryValue(int visibleIndex, ASPxSummaryItem item) {
			return null;
		}
		public override bool IsExpressionFitToRule(string expression, int visibleIndex) {
			return false;
		}
		public override object GetRowValue(int visibleIndex, string fieldName, bool isDesignTime) {
			object row = GetRow(visibleIndex);
			if(row == null) return null;
			return this.dataController.GetRowValue(visibleIndex - Info.StartIndex, fieldName);
		}
		public override int FindRowByKey(string keyFieldName, object value, bool expandGroups) {
			for(int n = Info.StartIndex; n < Info.StartIndex + info.RowCount; n++) {
				object val = GetRowValue(n, keyFieldName, false);
				if(Object.Equals(val, value)) return n;
			}
			return -1;
		}
		public override Dictionary<string, Type> GetColumnPropertyTypes() {
			return Columns.ToDictionary(col => col.PropertyDescriptor.Name, col => col.PropertyDescriptor.PropertyType);
		}
		public override int FindRowByKeys(Dictionary<string, object> columnValues, bool expandGroups) {
			if (columnValues == null) return -1;
			for (int n = Info.StartIndex; n < Info.StartIndex + info.RowCount; n++) {
				if (IsRowValuesEquals(columnValues, n)) return n;
			}
			return -1;
		}
		public override void ValidateSelectedKeys() {
		}
		public override bool IsValidKey(object serializedKey) {
			return true;
		}
		public override Dictionary<object, bool> GetFilteredSelectedKeys() {
			return null;
		}
		public override bool HasFieldName(string fieldName) {
			return Columns[fieldName] != null;
		}
		public override Type GetFieldTypeCore(string fieldName) { return Columns[fieldName].Type; }
		public override bool IsUnboundField(string fieldName) {
			DataColumnInfo column = Columns[fieldName];
			return column != null && column.Unbound;
		}
		public override void SaveRowState(Stream stream) {
		}
		public override void RestoreRowsState() {
		}
		protected override List<DataColumnInfo> GetSavedColums(List<string> usedFields) {
			List<DataColumnInfo> list = new List<DataColumnInfo>();
			foreach(DataColumnInfo columnInfo in Columns) {
				if(!columnInfo.Browsable) continue; 
				list.Add(columnInfo);
			}
			return list;
		}
		public override bool AllowSearchPanelFilter(DataColumnInfo column) {
			return false;
		}
		protected override void SaveParentGroupRows(TypedBinaryWriter writer, List<DataColumnInfo> savedColumns, int visibleStartIndex) {
			writer.WriteObject(0);
		}
		public override void SetDataSource(object dataSource) {
			if(dataSource == null) {
				this.info = new WebRegularDataProviderInfo();
			}
			this.dataController.SetDataSource(dataSource);
			Info.Ready = true;
			Info.RowCount = this.dataController.VisibleCount;
			Info.TotalRowCount = Proxy.Owner.SelectArguments.TotalRowCount;
			Info.StartIndex = Proxy.Owner.SelectArguments.StartRowIndex;
		}
		string filterExpression = string.Empty;
		public override void SortGroupChanged(List<IWebColumnInfo> sortList, int groupCount, string filterExpression, IList groupSummary, IList totalSummary, IList formatConditionSummary) {
			this.filterExpression = filterExpression;
		}
		public override string FilterExpression {
			get {
				return filterExpression;
			}
		}
	}
}
