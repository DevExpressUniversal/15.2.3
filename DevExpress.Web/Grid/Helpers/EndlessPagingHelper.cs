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

using DevExpress.Web.Cookies;
using DevExpress.Web.Data;
using DevExpress.Web.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace DevExpress.Web.Internal {
	public interface IGridEndlessPagingItemsContainer {
		IEnumerable<WebControl> Items { get; }
	}
	public class GridEndlessPagingHelper {
		const string 
			CallbackPrefix = "EP",
			PageKeysStateKey = "pageKeys",
			RemoveEditFormStateKey = "removeEditForm",
			HtmlStateKey = "html",
			DataTableInfoStateKey = "dataTableInfo",
			CommandButtonIDListKey = "cButtonIDs";
		int loadedRowCountOnCallback = -1;
		public GridEndlessPagingHelper(ASPxGridBase grid) {
			Grid = grid;
			PrevEditedRowIndex = -1;
			RemoveIndex = AddIndex = -1;
			ClientKeyValues = new ArrayList();
		}
		public ASPxGridBase Grid { get; private set; }
		public GridRenderHelper RenderHelper { get { return Grid.RenderHelper; } }
		public WebDataProxy DataProxy { get { return Grid.DataProxy; } }
		public int VisibleStartIndex { get; private set; }
		public int VisibleRowCountOnPage { get; private set; }
		public int LoadedRowCount {
			get {
				if(PartialLoad)
					return this.loadedRowCountOnCallback;
				if(!ShouldLoadFirstPage && ClientLoadedRowCount > 0)
					return ClientLoadedRowCount;
				return DataProxy.VisibleRowCountOnPage;
			}
		}
		public int ClientEditingRowVisibleIndex { get { return ClientKeyValues.IndexOf(DataProxy.EditingKeyValue); } }
		public bool PartialLoad { get; private set; }
		public bool ReplacePrevEditedRow { get { return PrevEditedRowIndex >= 0; } }
		public int PrevEditedRowIndex { get; private set; }
		public string CallbackCommand { get; private set; }
		protected IGridEndlessPagingItemsContainer DataItemsContainer { get { return Grid.ContainerControl.DataItemsContainer; } }
		protected GridEndlessPagingUpdatableContainer UpdatableContainer { get { return Grid.ContainerControl.EndlessPagingUpdatableContainer; } }
		protected int RemoveIndex { get; set; }
		protected int RemoveCount { get; set; }
		protected bool RemoveEditForm { get; set; }
		protected int AddIndex { get; set; }
		protected internal ArrayList ClientKeyValues { get; set; }
		protected GridColumnsState ClientColumnsState { get; set; }
		protected string ClientActiveFilter { get; set; }
		protected int ClientPageIndex { get; set; }
		protected int ClientPageSize { get; set; }
		protected object ClientEditingKey { get; set; }
		protected int ClientLoadedRowCount { get { return ClientKeyValues.Count; } }
		protected internal int ClientDataRowCount { get { return ClientKeyValues.OfType<object>().Where(k => k != null).Count(); } }
		public bool ShouldLoadFirstPage { get; set; }
		protected bool PassKeysToClient { get; set; }
		protected virtual bool ShowNewRowAtBottom { get { return false; } } 
		public virtual int ValidateVisibleIndex(string command, int clientIndex, ref int offset) {
			return clientIndex;
		}
		public void ProcessCallback(string command, params object[] args) {
			ValidatePageIndex();
			if(ShouldLoadFirstPage)
				return;
			ProcessCallbackCore(command, args);
			if(!ShouldLoadFirstPage)
				PartialLoad = true;
			DataProxy.CheckFocusedRowChanged();
			Grid.LayoutChanged();
		}
		protected virtual void ProcessCallbackCore(string command, params object[] args) {
			CallbackCommand = command;
			switch(command) {
				case GridViewCallbackCommand.NextPage:
					NextPage();
					break;
				case GridViewCallbackCommand.StartEdit:
					StartEdit();
					break;
				case GridViewCallbackCommand.AddNewRow:
					AddNewRow();
					break;
				case GridViewCallbackCommand.CancelEdit:
					CancelEdit();
					break;
				case GridViewCallbackCommand.UpdateEdit:
					UpdateEdit();
					break;
				case GridViewCallbackCommand.DeleteRow:
					DeleteRow(args[0]);
					break;
				case GridViewCallbackCommand.ShowFilterControl:
					break;
				case GridViewCallbackCommand.CloseFilterControl:
					this.loadedRowCountOnCallback = ClientLoadedRowCount;
					break;
				case GridViewCallbackCommand.Selection:
				case GridViewCallbackCommand.FocusedRow:
					this.loadedRowCountOnCallback = ClientLoadedRowCount;
					break;
			}
		}
		protected void NextPage() {
			var nextRowClientIndex = ClientLoadedRowCount;
			var nextRowServerIndex = GetServerIndex(ClientLoadedRowCount - 1) + 1;
			if(nextRowServerIndex < 0) {
				LoadFirstPage();
				return;
			}
			LoadRowsFromIndex(nextRowClientIndex, nextRowServerIndex);
			RemoveEditForm = Grid.IsNewRowEditing && ShowNewRowAtBottom;
		}
		protected void StartEdit() {
			Grid.DataBindNoControls();
			RemovePrevEditedRow();
			var clientIndex = ClientKeyValues.IndexOf(DataProxy.EditingKeyValue);
			ReplaceRow(clientIndex, DataProxy.EditingRowVisibleIndex);
			RemoveEditForm = true;
		}
		protected void UpdateEdit() {
			if(ClientKeyValues.Count == 0)
				this.LoadFirstPage();
			if(!Grid.IsEditing) {
				var clientIndex = ClientLoadedRowCount - Grid.SettingsPager.PageSize;
				if(clientIndex < 0)
					clientIndex = 0;
				if(ClientEditingKey != null)
					clientIndex = ClientKeyValues.IndexOf(ClientEditingKey);
				var serverIndex = GetServerIndex(clientIndex);
				if(ClientEditingKey != null) {
					var expectedServerIndex = GetServerIndex(clientIndex - 1) + 1;
					if(serverIndex != expectedServerIndex)
						serverIndex = expectedServerIndex;
				}
				LoadRowsFromIndex(clientIndex, serverIndex);
				RemoveEditForm = true;
				return;
			}
			if(Grid.IsNewRowEditing)
				AddNewRow();
			else
				StartEdit();
		}
		protected void CancelEdit() {
			if(ClientKeyValues.Count == 0)
				this.LoadFirstPage();
			if(ClientEditingKey != null) {
				var clientIndex = ClientKeyValues.IndexOf(ClientEditingKey);
				ReplaceRow(clientIndex, GetServerIndex(ClientEditingKey));
			}
			RemoveEditForm = true;
			this.loadedRowCountOnCallback = ClientLoadedRowCount;
		}
		protected void AddNewRow() {
			if(ClientKeyValues.Count == 0)
				this.LoadFirstPage();
			RemovePrevEditedRow();
			VisibleStartIndex = 0;
			VisibleRowCountOnPage = 0;
			this.loadedRowCountOnCallback = ClientLoadedRowCount;
			RemoveIndex = -1;
			RemoveCount = 0;
			RemoveEditForm = true;
			AddIndex = ShowNewRowAtBottom ? ClientLoadedRowCount : 0;
		}
		protected void DeleteRow(object key) {
			if(GetServerIndex(key) >= 0) {
				RemoveEditForm = !Grid.IsEditing;
				this.loadedRowCountOnCallback = ClientLoadedRowCount;
				return;
			}
			RemovePrevEditedRow();
			var clientIndex = ClientKeyValues.IndexOf(key);
			RemoveIndex = clientIndex;
			RemoveCount = 1;
			RemoveEditForm = !Grid.IsEditing;
			AddIndex = -1;
			VisibleRowCountOnPage = 0;
			this.loadedRowCountOnCallback = ClientLoadedRowCount - 1;
			PassKeysToClient = true;
		}
		protected void LoadRowsFromIndex(int clientIndex, int serverIndex) {
			VisibleStartIndex = serverIndex;
			PassKeysToClient = true;
			RemoveIndex = clientIndex;
			RemoveCount = ClientLoadedRowCount - clientIndex;
			AddIndex = clientIndex;
			var newPageIndex = serverIndex / DataProxy.PageSize;
			if(Grid.PageIndexInternal != newPageIndex)
				Grid.PageIndexInternal = newPageIndex;
			VisibleRowCountOnPage = GetLastVisibleIndexOnPage(Grid.PageIndexInternal) - VisibleStartIndex + 1;
			if(VisibleRowCountOnPage < DataProxy.PageSize / 3)
				VisibleRowCountOnPage = GetLastVisibleIndexOnPage(++Grid.PageIndexInternal) - VisibleStartIndex + 1;
			this.loadedRowCountOnCallback = clientIndex + VisibleRowCountOnPage;
		}
		protected void ReplaceRow(int clientIndex, int serverIndex) {
			VisibleStartIndex = serverIndex;
			VisibleRowCountOnPage = 1;
			this.loadedRowCountOnCallback = ClientLoadedRowCount;
			RemoveIndex = clientIndex;
			RemoveCount = 1;
			AddIndex = clientIndex;
		}
		protected void RemovePrevEditedRow() {
			if(ClientEditingKey != null && !object.Equals(ClientEditingKey, DataProxy.EditingKeyValue))
				PrevEditedRowIndex = GetServerIndex(ClientEditingKey);
		}
		protected int GetLastVisibleIndexOnPage(int pageIndex) {
			return Math.Min(((pageIndex + 1) * DataProxy.PageSize) - 1, DataProxy.VisibleRowCount - 1);
		}
		public int CheckFocusedRowVisibleIndex(int index) {
			if(index < 0)
				return -1;
			var leftSide = 0;
			var rightSide = ShouldLoadFirstPage ? 0 : LoadedRowCount - 1;
			if(index < leftSide)
				return leftSide;
			if(index > rightSide)
				return rightSide;
			return index;
		}
		public void ValidatePageIndex() {
			if(!CanUsePartialLoad())
				LoadFirstPage();
		}
		public void LoadFirstPage() {
			ShouldLoadFirstPage = true;
			if(Grid.PageIndexInternal == 0)
				Grid.DataBindNoControls();
			else
				Grid.CommandButtonHelper.ResetIndices();
			Grid.PageIndexInternal = 0;
			PartialLoad = false;
		}
		protected virtual bool CanUsePartialLoad() {
			if(ClientActiveFilter != Grid.FilterHelper.ActiveFilter)
				return false;
			if(ClientPageSize != Grid.PageSize)
				return false;
			if(ClientColumnsState.ColumnStates.Count != Grid.ColumnHelper.AllColumns.Count)
				return false;
			foreach(var state in ClientColumnsState.ColumnStates) {
				if(state.VisibleIndex != state.Column.VisibleIndex)
					return false;
			}
			if(Grid.SortedColumns.Count != ClientColumnsState.SortList.Count)
				return false;
			for(var i = 0; i < ClientColumnsState.SortList.Count; i++) {
				var state = ClientColumnsState.SortList[i];
				if(state.SortOrder != state.Column.SortOrder)
					return false;
				if(state.Column.SortIndex != i)
					return false;
			}
			return true;
		}
		public void OnBeforeGetCallbackResult() {
			ValidatePageIndex();
			if(!PartialLoad && Grid.PageIndexInternal != 0)
				LoadFirstPage();
		}
		public string GetEndlessPagingCallbackResult(string inlineScript) {
			Grid.SyncCallbackState();
			return CallbackPrefix + "|" + HtmlConvertor.ToJSON(GetCallbackResultObject(inlineScript));
		}
		protected virtual IDictionary GetCallbackResultObject(string inlineScript) {
			var result = new Dictionary<string, object>();
			result[PageKeysStateKey] = GetPageKeyValuesCallbackInfo().ToArray();
			result[RemoveEditFormStateKey] = RemoveEditForm;
			result[DataTableInfoStateKey] = GetDataTableCallbackInfoList().Select(i => i.ToArray());
			result[HtmlStateKey] = RenderUtils.GetRenderResult(UpdatableContainer, inlineScript);
			result[CommandButtonIDListKey] = Grid.CommandButtonHelper.CommandButtonClientIDList;
			return result;
		}
		protected internal GridEndlessPagingCallbackInfo GetPageKeyValuesCallbackInfo() {
			var info = new GridEndlessPagingCallbackInfo();
			if(PartialLoad && PassKeysToClient) {
				info.RemoveIndex = RemoveCount > 0 ? RemoveIndex : -1;
				info.RemoveCount = RemoveCount;
				info.AddIndex = AddIndex;
				info.Content = DataProxy.GetPageKeyValuesForScript();
			}
			return info;
		}
		protected internal List<GridEndlessPagingCallbackInfo> GetDataTableCallbackInfoList() {
			var list = new List<GridEndlessPagingCallbackInfo>();
			if(!PartialLoad)
				return list;
			var info = new GridEndlessPagingCallbackInfo();
			list.Add(info);
			info.RemoveIndex = RemoveCount > 0 ? RemoveIndex : -1;
			info.RemoveCount = RemoveCount;
			info.AddIndex = AddIndex;
			info.Content = RenderDataTable(ReplacePrevEditedRow);
			if(ReplacePrevEditedRow) {
				info = new GridEndlessPagingCallbackInfo();
				list.Add(info);
				info.RemoveIndex = ClientKeyValues.IndexOf(ClientEditingKey);
				info.RemoveCount = Grid.SettingsEditing.DisplayEditingRow ? 1 : 0;
				info.AddIndex = info.RemoveIndex;
				info.Content = RenderUtils.GetRenderResult(DataItemsContainer.Items.First());
			}
			if(list.Count == 2 && list[0].AddIndex > list[1].AddIndex)
				list.Reverse();
			return list;
		}
		public virtual void LoadClientState(GridColumnsState columnsState, ArrayList clientKeyValues, ArrayList groupState) {
			ClientColumnsState = columnsState;
			ClientKeyValues = DataProxy.GetKeyValuesFromScript(clientKeyValues);
			ClientActiveFilter = Grid.FilterHelper.ActiveFilter;
			ClientPageIndex = Grid.PageIndex;
			ClientPageSize = Grid.PageSize;
			ClientEditingKey = DataProxy.EditingKeyValue;
		}
		protected virtual internal IList GetGroupState() { return null; }
		protected virtual int GetServerIndex(int clientIndex) {
			if(clientIndex >= 0 && clientIndex < ClientKeyValues.Count)
				return GetServerIndex(ClientKeyValues[clientIndex]);
			return -1;
		}
		int GetServerIndex(object key) {
			return DataProxy.FindVisibleIndexByKey(key, false);
		}
		protected virtual string RenderDataTable(bool skipFirstRow) {
			using(var sw = new StringWriter(CultureInfo.InvariantCulture))
			using(var writer = new HtmlTextWriter(sw)) {
				RenderGridItems(DataItemsContainer, Enumerable.Range(0, skipFirstRow ? 1 : 0), writer);
				return sw.ToString();
			}
		}
		protected virtual void RenderGridItems(IGridEndlessPagingItemsContainer itemsContainer, IEnumerable<int> excludedIndices, HtmlTextWriter writer) {
			if(itemsContainer == null) return;
			var controls = itemsContainer.Items.Where((r, i) => !excludedIndices.Contains(i));
			foreach(var control in controls)
				control.RenderControl(writer);
		}
		public int GetClientSelectedRowCount() {
			if(!PartialLoad)
				return 0;
			var count = 0;
			int[] excludedIndices = RemoveIndex > -1 ? Enumerable.Range(RemoveIndex, RemoveCount).ToArray() : new int[0];
			for(int clientIndex = 0; clientIndex < ClientKeyValues.Count; clientIndex++){
				if(excludedIndices.Contains(clientIndex)) continue;
				var serverIndex = GetServerIndex(clientIndex);
				if(DataProxy.Selection.IsRowSelected(serverIndex))
					count++;
			}
			return count++;
		}
	}
	public class GridEndlessPagingCallbackInfo {
		public GridEndlessPagingCallbackInfo() {
			RemoveIndex = -1;
			AddIndex = -1;
		}
		public int RemoveIndex { get; set; }
		public int RemoveCount { get; set; }
		public int AddIndex { get; set; }
		public object Content { get; set; }
		public object[] ToArray() {
			if(RemoveCount == 0 && AddIndex < 0 && Content == null)
				return null;
			return new object[] { RemoveIndex, RemoveCount, AddIndex, Content };
		}
	}
}
