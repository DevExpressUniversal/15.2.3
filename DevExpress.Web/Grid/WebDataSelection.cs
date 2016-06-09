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
using DevExpress.Data;
using System.Collections;
using DevExpress.Data.IO;
using System.ComponentModel;
using System.Linq;
using DevExpress.Data.Helpers;
using DevExpress.Web.Data.Internal;
namespace DevExpress.Web.Data {
	public class GridViewSelection : WebDataSelection {
		public GridViewSelection(WebDataProxy webData)
			: base(webData) {
		}
		public new bool IsRowSelected(int visibleIndex) { return base.IsRowSelected(visibleIndex); }
		public new bool IsRowSelectedByKey(object keyValue) { return base.IsRowSelectedByKey(keyValue); }
		public new void SelectRowByKey(object keyValue) { base.SelectRowByKey(keyValue); }
		public new void UnselectRowByKey(object keyValue) { base.UnselectRowByKey(keyValue); }
		public new void SelectRow(int visibleIndex) { base.SelectRow(visibleIndex); }
		public new void UnselectRow(int visibleIndex) { base.UnselectRow(visibleIndex); }
	}
	public class CardViewSelection : WebDataSelection {
		public CardViewSelection(WebDataProxy webData)
			: base(webData) {
		}
		public bool IsCardSelected(int visibleIndex) { return IsRowSelected(visibleIndex); }
		public bool IsCardSelectedByKey(object keyValue) { return IsRowSelectedByKey(keyValue); }
		public void SelectCardByKey(object keyValue) { SelectRowByKey(keyValue); }
		public void UnselectCardByKey(object keyValue) { UnselectRowByKey(keyValue); }
		public void SelectCard(int visibleIndex) { SelectRow(visibleIndex); }
		public void UnselectCard(int visibleIndex) { UnselectRowByKey(visibleIndex); }
	}
	public class WebDataSelectionBase {
		Dictionary<object, bool> selected;
		WebDataProxy webData;
		bool selectionChangedOnLock = false;
		int cachedTotalRowCount = 0;
		int lockSelection = 0;
		protected internal Dictionary<object, bool> Selected { get { return selected; } }
		protected internal bool HasSelectedKeys { get { return Selected.Count > 0; } }
		protected WebDataProxy WebData { get { return webData; } }
		protected WebRowType GetRowType(int visibleIndex) { return WebData.GetRowType(visibleIndex); }
		protected object GetRowKeyValue(int visibleIndex) { return WebData.GetRowKeyValue(visibleIndex); }
		protected object GetRowKeyValue(int visibleIndex, bool ensureRowDataIsBound) { return WebData.GetRowKeyValue(visibleIndex, ensureRowDataIsBound); }
		protected object GetListSourceRowKeyValue(int listSourceRowIndex) { return WebData.GetListSourceRowKeyValue(listSourceRowIndex); }
		protected virtual bool SaveSelectionViaSelectedKeys { get; set; }
		protected int CachedTotalRowCount { get { return cachedTotalRowCount; } set { cachedTotalRowCount = value; } }
		protected bool SelectionChangedOnLock { get { return selectionChangedOnLock; } }
		protected internal bool IsLockSelection { get { return lockSelection != 0; } }
		protected virtual bool AllowSelectSingleRowOnly { get { return WebData.Owner != null && WebData.Owner.AllowSelectSingleRowOnly; } }
		protected internal void BeginSelection() {
			this.selectionChangedOnLock = false;
			this.lockSelection++;
		}
		protected internal void CancelSelection() {
			this.lockSelection--;
		}
		protected internal void EndSelection() {
			if(--this.lockSelection == 0) {
				if(this.selectionChangedOnLock) {
					this.selectionChangedOnLock = false;
					OnSelectionChanged();
				}
			}
		}
		public WebDataSelectionBase(WebDataProxy webData) {
			this.webData = webData;
			this.selected = new Dictionary<object, bool>();
			SaveSelectionViaSelectedKeys = true;
		}
		protected bool CanSelectRow(int visibleIndex) {
			return GetRowType(visibleIndex) == WebRowType.Data;
		}
		protected void AddRow(object keyValue) {
			if(keyValue == null) return;
			Selected[keyValue] = true;
			OnSelectionChanged();
		}
		protected void RemoveRow(object keyValue) {
			if(!Selected.ContainsKey(keyValue)) return;
			Selected.Remove(keyValue);
			OnSelectionChanged();
		}
		protected virtual void OnSelectionChanged() {
			this.selectionChangedOnLock = true;
			if(IsLockSelection) return;
			FireSelectionChanged();   
		}
		protected virtual void FireSelectionChanged() { }
		protected bool IsRowSelectedByKeyCore(object keyValue, bool isValidateKey) {
			if(keyValue == null)
				return false;
			object serializedKey = SerializeKeyValue(keyValue);
			if(isValidateKey && !WebData.IsValidKey(serializedKey) && Selected.ContainsKey(serializedKey))
				Selected.Remove(selected);
			return SaveSelectionViaSelectedKeys ? Selected.ContainsKey(serializedKey) : !Selected.ContainsKey(serializedKey);
		}
		protected bool IsListSourceRowSelected(int listSourceRowIndex) {
			if(CountCore == 0) return false;
			return IsRowSelectedByKeyCore(GetListSourceRowKeyValue(listSourceRowIndex), false);
		}
		protected virtual void SetSelectionCore(object keyValue, bool selected, bool isValidateKey) {
			object serializedKey = SerializeKeyValue(keyValue);
			if(IsRowSelectedByKeyCore(serializedKey, false) == selected || (isValidateKey && !WebData.IsValidKey(serializedKey)))
				return;
			if(selected && AllowSelectSingleRowOnly) {
				if(!IsRowSelectedByKeyCore(serializedKey, false)) 
					UnselectAllCore();
			}
			if(!SaveSelectionViaSelectedKeys)
				selected = !selected;
			if(selected)
				AddRow(serializedKey);
			else
				RemoveRow(serializedKey);
		}
		protected object SerializeKeyValue(object key) { 
			if(WebData.IsMultipleKeyFields) {
				var keys = key as object[];
				if(keys != null && keys.Length == WebData.KeyFieldNames.Count)
					return WebData.ConvertMulitpleKeyValuesToString(keys);
				return key;
			}
			return WebData.ConvertValue(WebData.KeyFieldName, key);
		}
		List<object> GetCachedSelectedKeys() {
			if(!WebData.IsMultipleKeyFields)
				return Selected.Keys.ToList();
			return Selected.Keys
				.OfType<string>()
				.Select(k => WebData.ConvertMultipleKeyValuesFromString(k.Split(WebDataProxy.MultipleKeyValueSeparator)))
				.ToList<object>();
		}
		protected virtual int CountCore {
			get {
				if(SaveSelectionViaSelectedKeys)
					return Selected.Count;
				return CachedTotalRowCount - Selected.Count;
			}
		}
		protected bool IsRowSelectedCore(int visibleIndex) {
			return IsRowSelectedCore(visibleIndex, false);
		}
		protected bool IsRowSelectedCore(int visibleIndex, bool ensureRowDataIsBound) {
			if(SaveSelectionViaSelectedKeys && Selected.Count == 0 || !CanSelectRow(visibleIndex))
				return false;
			return IsRowSelectedByKeyCore(GetRowKeyValue(visibleIndex, ensureRowDataIsBound), false);
		}
		protected void SetSelectionCore(int visibleIndex, bool selected) {
			if(!CanSelectRow(visibleIndex)) return;
			SetSelectionCore(GetRowKeyValue(visibleIndex), selected, false);
		}
		protected virtual void SelectAllCore() {
			if(IsAllVisibleRowsSelected())
				return;
			try {
				BeginSelection();
				if(AllowSelectSingleRowOnly) {
					SetSelectionCore(0, true);
					return;
				};
				SaveSelectionViaSelectedKeys = WebData.IsFiltered;
				if(SaveSelectionViaSelectedKeys) {
					WebData.DoOwnerDataBinding();
					SelectNonFilteredRows(true);
				}
				else {
					Selected.Clear();
					UpdateCachedParameters();
				}
				OnSelectionChanged();
			}
			finally{
				EndSelection();
			}
		}
		protected void SelectNonFilteredRows(bool selected) {
			BeginSelection();
			for(int i = 0; i < WebData.VisibleRowCount; i++) {
				switch(GetRowType(i)) {
					case WebRowType.Data:
						SetSelectionCore(i, selected);
						break;
					case WebRowType.Group:
						if(!WebData.IsRowExpanded(i)) {
							foreach(object key in WebData.GetChildKeysRecursive(i))
								SetSelectionCore(key, selected, false);
						}
						break;
				}
			}
			EndSelection();
		}
		protected bool IsAllVisibleRowsSelected() {
			if(WebData.VisibleRowCount != CountCore)
				return false;
			for(int i = 0; i < WebData.VisibleRowCount; i++) {
				if(!IsRowSelectedCore(i))
					return false;
			}
			return true;
		}
		protected void UnselectAllCore() {
			if(CountCore == 0) return;
			SaveSelectionViaSelectedKeys = true;
			Selected.Clear();
			OnSelectionChanged();
		}
		protected internal virtual void UpdateCachedParameters() {
			CachedTotalRowCount = WebData.ListSourceRowCount;
		}
		protected internal virtual void SaveState(TypedBinaryWriter writer) {
			writer.WriteObject(SaveSelectionViaSelectedKeys);
			writer.WriteObject(CachedTotalRowCount);
			writer.WriteObject(Selected.Count);
			foreach(KeyValuePair<object, bool> entry in Selected) {
				writer.WriteTypedObject(entry.Key);
			}
		}
		protected void LoadStateCore(TypedBinaryReader reader) {
			BeginSelection();
			try {
				LoadStateFromStream(reader);
			} finally {
				CancelSelection();
			}
		}
		protected virtual void LoadStateFromStream(TypedBinaryReader reader) {
			Selected.Clear();
			SaveSelectionViaSelectedKeys = reader.ReadObject<bool>();
			CachedTotalRowCount = reader.ReadObject<int>();
			int count = reader.ReadObject<int>();
			for(int i = 0; i < count; i++) {
				AddRow(reader.ReadTypedObject());
			}
		}
	}
	public class WebDataSelection: WebDataSelectionBase, IWebDataSelectAllOwner {
		ArrayList pageKeys;
		public WebDataSelection(WebDataProxy webData) : base(webData) {
			this.pageKeys = new ArrayList();
			SelectAllCacheState = new WebDataSelectAllCacheState(webData, this);
		}
		protected ArrayList PageKeys { get { return pageKeys; } set { pageKeys = value; } }
		protected override void FireSelectionChanged() {
			WebData.OnSelectionChanged();
		}
#if !SL
	[DevExpressWebLocalizedDescription("WebDataSelectionCount")]
#endif
		public int Count { get { return CountCore; } }
#if !SL
	[DevExpressWebLocalizedDescription("WebDataSelectionFilteredCount")]
#endif
		public int FilteredCount { get { return FilteredCountCore; } }
		public void SetSelection(int visibleIndex, bool selected) { SetSelectionCore(visibleIndex, selected); }
		public void SetSelectionByKey(object keyValue, bool selected) { SetSelectionCore(keyValue, selected, true); }
		public void SelectAll() { SelectAllCore(); }
		public void UnselectAll() { UnselectAllCore(); }
		public void UnselectAllFiltered() {
			if(WebData.IsFiltered) {
				WebData.DoOwnerDataBinding();
				SelectNonFilteredRows(false);
			} else {
				UnselectAll();
			}
		}
		protected internal bool IsRowSelected(int visibleIndex) { return IsRowSelectedCore(visibleIndex, true); }
		protected internal bool IsRowSelectedByKey(object keyValue) { return IsRowSelectedByKeyCore(keyValue, true); }
		protected internal void SelectRowByKey(object keyValue) { SetSelectionByKey(keyValue, true); }
		protected internal void UnselectRowByKey(object keyValue) { SetSelectionByKey(keyValue, false); }
		protected internal void SelectRow(int visibleIndex) { SetSelection(visibleIndex, true); }
		protected internal void UnselectRow(int visibleIndex) { SetSelection(visibleIndex, false); }
		protected internal int CachedCount { get { return base.CountCore; } }
		protected internal int CachedFilteredCount { get { return -1; } }
		protected override int CountCore {
			get {
				WebData.ValidateSelectedKeys();
				return base.CountCore;
			}
		}
		protected int FilteredCountCore {
			get {
				if(!WebData.IsFiltered)
					return CountCore;
				var filteredSelectedKeys = WebData.GetFilteredSelectedKeys();
				int filteredCount = filteredSelectedKeys != null ? filteredSelectedKeys.Count : 0;
				if(SaveSelectionViaSelectedKeys)
					return filteredCount;
				return WebData.VisibleRowCount - filteredCount;
			}
		}
		protected override bool SaveSelectionViaSelectedKeys {
			get { return WebData.Owner.AlwaysSaveSelectionViaSelectedKeys ? true : base.SaveSelectionViaSelectedKeys; }
			set { base.SaveSelectionViaSelectedKeys = value; } 
		}
		protected WebDataSelectAllCacheState SelectAllCacheState { get; private set; }
		protected internal override void UpdateCachedParameters() {
			base.UpdateCachedParameters();
			SelectAllCacheState.Reset();
		}
		protected internal void LoadState(TypedBinaryReader reader, string pageSelectionResult, ArrayList pageKeyValues) {
			if(reader != null) LoadStateCore(reader);
			WebData.BeginUseCachedProvider();
			try {
				BeginSelection();
				SelectAllCacheState.Lock();
				try {
					LoadStateFromPage(pageSelectionResult, pageKeyValues);
				}
				finally {
					EndSelection();
					SelectAllCacheState.Unlock();
				}
			}
			finally {
				WebData.EndUseCachedProvider();
				WebData.ValidateSelectedKeys();
			}
		}
		void LoadStateFromPage(string pageSelection, ArrayList pageKeyValues) {
			if(pageKeyValues != null)
				PageKeys = pageKeyValues;
			if(string.IsNullOrEmpty(pageSelection)) 
				return;
			if(WebData.KeyFieldNames.Count == 0)
				WebData.ThrowMissingPrimaryKey();
			var rowCount = PageKeys.Count;
			if(pageSelection == "U")
				pageSelection = new string('F', rowCount);
			if(pageSelection.Length < rowCount)
				pageSelection += new string('F', rowCount - pageSelection.Length);
			var firstIndexOnPage = WebData.UseEndlessPaging ? 0 : WebData.VisibleStartIndex;
			for(int i = 0; i < PageKeys.Count; i++) {
				if(CanSelectRow(firstIndexOnPage + i)) {
					bool selected = pageSelection.Length > i ? pageSelection[i] == 'T' : false;
					SetSelectionCore(PageKeys[i], selected, false);
				}
			}
		}
		protected override void LoadStateFromStream(TypedBinaryReader reader) {
			base.LoadStateFromStream(reader);
			SelectAllCacheState.Load(reader);
		}
		protected internal override void SaveState(TypedBinaryWriter writer) {
			base.SaveState(writer);
			SelectAllCacheState.Save(writer);
		}
		public new void BeginSelection() { 
			base.BeginSelection();
		}
		public new void EndSelection() {
			base.EndSelection();
		}
		public new void CancelSelection() {
			base.CancelSelection();
		}
		protected override void OnSelectionChanged() {
			if(!IsLockSelection)
				SelectAllCacheState.Reset();
			base.OnSelectionChanged();
		}
		protected internal virtual List<object> GetSelectedValues(string[] fieldNames, bool searchFilteredValues) {
			int selectedKeyCount = searchFilteredValues ? FilteredCountCore : CountCore;
			if(selectedKeyCount < 1 || fieldNames.Length < 1)
				return new List<object>();
			Dictionary<object, bool> selectedHash = searchFilteredValues ? WebData.GetFilteredSelectedKeys() : Selected;
			if(SaveSelectionViaSelectedKeys) {
				if(fieldNames.Length == 1 && fieldNames[0] == WebData.KeyFieldName)
					return new List<object>(selectedHash.Keys);
				List<object> selectedValues;
				if(TryGetSelectedValuesInServerMode(fieldNames, selectedHash, out selectedValues))
					return selectedValues;
			}
			List<object> list = new List<object>(selectedKeyCount);
			for(int i = 0; i < WebData.VisibleRowCount; i++) {
				if(IsRowSelectedCore(i))
					list.Add(WebData.GetRowValues(i, fieldNames));
			}
			if(list.Count == selectedKeyCount || WebData.Owner.IsForceDataSourcePaging)
				return list;
			list.Clear();
			WebData.DoOwnerDataBinding();
			for(int n = 0; (n < WebData.ListSourceRowCount) && (list.Count < selectedKeyCount); n++) {
				object serializedKey = SerializeKeyValue(GetListSourceRowKeyValue(n));
				bool isSelectedKey = SaveSelectionViaSelectedKeys ? selectedHash.ContainsKey(serializedKey) : !selectedHash.ContainsKey(serializedKey);
				if(isSelectedKey)
					list.Add(WebData.GetListSourceRowValues(n, fieldNames));
			}
			return list;
		}
		bool TryGetSelectedValuesInServerMode(string[] fieldNames, Dictionary<object, bool> selectedHash, out List<object> selectedValues) {
			selectedValues = new List<object>();
			if(!WebData.IsServerMode || !SaveSelectionViaSelectedKeys)
				return false;
			WebData.DoOwnerDataBinding();
			IListServer serverList = WebData.GetServerModeListSource();
			if(serverList == null)
				return false;
			selectedValues = new List<object>(selectedHash.Keys.Count);
			foreach(object key in selectedHash.Keys) {
				object keyValue = key;
				if(WebData.IsMultipleKeyFields)
					keyValue = new ServerModeCompoundKey(WebData.GetMultipleKeyValues(key, true).Values.ToArray());
				int listSourceIndex = serverList.GetRowIndexByKey(keyValue);
				selectedValues.Add(WebData.GetListSourceRowValues(listSourceIndex, fieldNames));
			}
			return true;
		}
		protected internal CheckState? IsSelectedAllRowsWithoutPage() {
			PerformUpdateSelectAllCache();
			return SelectAllCacheState.IsSelectedAllRowsWithoutPage;
		}
		protected internal CheckState IsSelectedAllRows() {
			PerformUpdateSelectAllCache();
			return SelectAllCacheState.IsSelectedAllRows() ?? CheckState.Checked;
		}
		CheckState? IsSelectedAllRowsCore(params int[] excludedPagedIndeces) {
			if(WebData.IsServerMode)
				return null;
			WebData.DoOwnerDataBinding();
			CheckState? checkState = null;
			for(int pageIndex = 0; pageIndex < WebData.PageCount; pageIndex++){
				CheckState? isSelectedAllRowsOnPage = IsSelectedAllRowsOnPageCore(pageIndex, !excludedPagedIndeces.Contains(pageIndex), true);
				if(isSelectedAllRowsOnPage != null)
					checkState = MergeCheckedState(checkState, isSelectedAllRowsOnPage.Value);
			}
			return checkState;
		}
		protected internal CheckState IsSelectedAllRowsOnPage(int pageIndex) {
			CheckState? isSelectedAllRowsOnPage = IsSelectedAllRowsOnPageCore(pageIndex, true, false);
			return isSelectedAllRowsOnPage != null ? isSelectedAllRowsOnPage.Value : CheckState.Unchecked;
		}
		CheckState? IsSelectedAllRowsOnPageCore(int pageIndex, bool considerVisibleDataRows, bool considerKeysInCollapsedGroup) {
			if(!considerVisibleDataRows && !considerKeysInCollapsedGroup)
				return CheckState.Unchecked;
			CheckState? result = null;
			int visibleStartIndex = pageIndex * WebData.PageSize;
			int visibleRowCountOnPage = Math.Min(WebData.VisibleRowCount - visibleStartIndex, WebData.PageSize);
			for(int i = 0; i < visibleRowCountOnPage; i++) {
				int visibleIndex = visibleStartIndex + i;
				switch(WebData.GetRowType(visibleIndex)) {
					case WebRowType.Data:
						if(considerVisibleDataRows)
							result = MergeCheckedState(result, IsRowSelectedCore(visibleIndex) ? CheckState.Checked : CheckState.Unchecked);
						break;
					case WebRowType.Group:
						if(considerKeysInCollapsedGroup && !WebData.IsRowExpanded(visibleIndex))
							result = MergeCheckedState(result, GetCollapsedGroupRowSelectedState(visibleIndex));
						break;
				}
			}
			return result;
		}
		CheckState GetCollapsedGroupRowSelectedState(int groupRowVisibleIndex) {
			var groupKeys = WebData.GetChildKeysRecursive(groupRowVisibleIndex);
			int selectedRowCount = 0;
			foreach(object key in groupKeys) {
				if(IsRowSelectedByKeyCore(key, false))
					selectedRowCount++;
			}
			if(selectedRowCount == groupKeys.Count)
				return CheckState.Checked;
			return selectedRowCount == 0 ? CheckState.Unchecked : CheckState.Indeterminate;
		}
		static CheckState MergeCheckedState(CheckState? val1, CheckState val2) {
			if(val1 == null) return val2;
			if(val1 != val2) return CheckState.Indeterminate;
			return val2;
		}
		void PerformUpdateSelectAllCache() {
			if(!SelectAllCacheState.RequireSynchronize)
				return;
			int[] excludedPageIndeces = WebData.UseEndlessPaging ? Enumerable.Range(0, WebData.PageIndex + 1).ToArray() : new int[] { WebData.PageIndex };
			SelectAllCacheState.Synchronize(IsSelectedAllRowsCore(excludedPageIndeces));
		}
		#region IWebDataSelectAllOwner Members
		void IWebDataSelectAllOwner.PerformUpdateSelectAllCache() {
			PerformUpdateSelectAllCache();
		}
		CheckState? IWebDataSelectAllOwner.IsSelectedAllRowsOnPage(int pageIndex) {
			return IsSelectedAllRowsOnPageCore(pageIndex, true, true);
		}
		CheckState IWebDataSelectAllOwner.MergeCheckedState(CheckState? val1, CheckState val2) {
			return MergeCheckedState(val1, val2);
		}
		#endregion
	}
	public class WebDataDetailRows : WebDataSelectionBase {
		public WebDataDetailRows(WebDataProxy webData) : base(webData) { }
		protected override void FireSelectionChanged() {
			WebData.OnDetailRowsChanged();
		}
#if !SL
	[DevExpressWebLocalizedDescription("WebDataDetailRowsVisibleCount")]
#endif
		public int VisibleCount { get { return CountCore; } }
		public bool IsVisible(int visibleIndex) { return IsRowSelectedCore(visibleIndex); }
		public void ExpandRowByKey(object keyValue) { SetSelectionCore(keyValue, true, true); }
		public void CollapseRowByKey(object keyValue) { SetSelectionCore(keyValue, false, true); }
		public void ExpandRow(int visibleIndex) { SetSelectionCore(visibleIndex, true); }
		public void CollapseRow(int visibleIndex) { SetSelectionCore(visibleIndex, false); }
		public void ExpandAllRows() { 
			SelectAllCore();
			if(WebData.UseEndlessPaging)
				WebData.EndlessPagingHelper.LoadFirstPage();
		}
		public void CollapseAllRows() {
			UnselectAllCore();
			if(WebData.UseEndlessPaging)
				WebData.EndlessPagingHelper.LoadFirstPage();
		}
		protected internal void LoadState(TypedBinaryReader reader) { LoadStateCore(reader); }
		protected override bool AllowSelectSingleRowOnly { get { return WebData.Owner != null && WebData.Owner.AllowOnlyOneMasterRowExpanded; } }
	}
}
namespace DevExpress.Web.Data.Internal {
	public class WebDataSelectAllCacheState {
		public WebDataSelectAllCacheState(WebDataProxy webData, IWebDataSelectAllOwner selection) {
			WebData = webData;
			Selection = selection;
			RequireSynchronize = true;
		}
		public CheckState? IsSelectedAllRowsWithoutPage { get; private set; }
		protected WebDataProxy WebData { get; private set; }
		protected IWebDataSelectAllOwner Selection { get; private set; }
		bool UseSelectAll { get { return WebData.Owner != null && WebData.Owner.UseSelectAll; } }
		public bool RequireSynchronize { get; private set; }
		public void Reset() {
			if(IsLocked) return;
			RequireSynchronize = true;
			if(UseSelectAll)
				WebData.RequireDataBound();
		}
		public void Synchronize(CheckState? isSelectedAllRowsWithoutPage) {
			RequireSynchronize = false;
			IsSelectedAllRowsWithoutPage = isSelectedAllRowsWithoutPage;
		}
		public CheckState? IsSelectedAllRows() {
			CheckState? isSelectedAllRowsOnPage = Selection.IsSelectedAllRowsOnPage(WebData.PageIndex);
			if(isSelectedAllRowsOnPage == null)
				return IsSelectedAllRowsWithoutPage;
			return Selection.MergeCheckedState(IsSelectedAllRowsWithoutPage, isSelectedAllRowsOnPage.Value);
		}
		public void Load(TypedBinaryReader reader) {
			RequireSynchronize = !reader.ReadObject<bool>();
			if(!RequireSynchronize)
				IsSelectedAllRowsWithoutPage = (CheckState?)reader.ReadTypedObject();
		}
		public void Save(TypedBinaryWriter writer) {
			bool useSelectAll = WebData.Owner != null && WebData.Owner.UseSelectAll;
			writer.WriteObject(useSelectAll);
			if(useSelectAll) {
				Selection.PerformUpdateSelectAllCache();
				writer.WriteTypedObject(IsSelectedAllRowsWithoutPage);
			}
		}
		int Locker { get; set; }
		protected bool IsLocked { get { return Locker > 0; } }
		public void Lock() { Locker++; }
		public void Unlock() { Locker--; }
	}
	public interface IWebDataSelectAllOwner {
		void PerformUpdateSelectAllCache();
		CheckState? IsSelectedAllRowsOnPage(int pageIndex);
		CheckState MergeCheckedState(CheckState? val1, CheckState val2);
	}
}
