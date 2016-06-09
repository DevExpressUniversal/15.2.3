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
using DevExpress.Data.IO;
using DevExpress.Web.Cookies;
using DevExpress.Web.Data;
using DevExpress.Web.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
namespace DevExpress.Web.Internal {
	public class GridViewColumnHelper : GridColumnHelper {
		ReadOnlyGridColumnCollection<GridViewColumn> allColumns;
		ReadOnlyGridColumnCollection<GridViewColumn> allVisibleColumns;
		ReadOnlyGridColumnCollection<GridViewDataColumn> allDataColumns;
		ReadOnlyGridColumnCollection<GridViewDataColumn> allVisibleDataColumns;
		ReadOnlyCollection<GridViewDataColumn> filterControlColumns;
		ReadOnlyCollection<GridViewDataColumn> editColumns;
		ReadOnlyCollection<GridViewDataColumn> searchPanelColumns;
		ReadOnlyCollection<GridViewColumn> adaptiveColumns;
		GridViewColumnVisualTreeNode visualTree;
		List<GridViewColumnVisualTreeNode> leafs;
		List<List<GridViewColumnVisualTreeNode>> layout;
		ReadOnlyCollection<GridViewColumn> fixedColumns;
		public GridViewColumnHelper(ASPxGridView grid)
			: base(grid) {
		}
		public new ReadOnlyGridColumnCollection<GridViewColumn> AllColumns {
			get {
				if(allColumns == null)
					allColumns = new ReadOnlyGridColumnCollection<GridViewColumn>(base.AllColumns.OfType<GridViewColumn>().ToList());
				return allColumns;
			}
		}
		public new ReadOnlyGridColumnCollection<GridViewColumn> AllVisibleColumns {
			get {
				if(allVisibleColumns == null)
					allVisibleColumns = new ReadOnlyGridColumnCollection<GridViewColumn>(base.AllVisibleColumns.OfType<GridViewColumn>().ToList());
				return allVisibleColumns;
			}
		}
		public new ReadOnlyGridColumnCollection<GridViewDataColumn> AllDataColumns {
			get {
				if(allDataColumns == null)
					allDataColumns = new ReadOnlyGridColumnCollection<GridViewDataColumn>(base.AllDataColumns.OfType<GridViewDataColumn>().ToList());
				return allDataColumns;
			}
		}
		public new ReadOnlyGridColumnCollection<GridViewDataColumn> AllVisibleDataColumns {
			get {
				if(allVisibleDataColumns == null)
					allVisibleDataColumns = new ReadOnlyGridColumnCollection<GridViewDataColumn>(base.AllVisibleDataColumns.OfType<GridViewDataColumn>().ToList());
				return allVisibleDataColumns;
			}
		}
		public new ReadOnlyCollection<GridViewDataColumn> FilterControlColumns {
			get {
				if(filterControlColumns == null)
					filterControlColumns = new ReadOnlyGridColumnCollection<GridViewDataColumn>(base.FilterControlColumns.OfType<GridViewDataColumn>().ToList());
				return filterControlColumns;
			}
		}
		public new ReadOnlyCollection<GridViewDataColumn> EditColumns {
			get {
				if(editColumns == null)
					editColumns = new ReadOnlyGridColumnCollection<GridViewDataColumn>(base.EditColumns.OfType<GridViewDataColumn>().ToList());
				return editColumns;
			}
		}
		public new ReadOnlyCollection<GridViewDataColumn> SearchPanelColumns {
			get {
				if(searchPanelColumns == null)
					searchPanelColumns = base.SearchPanelColumns.OfType<GridViewDataColumn>().ToList().AsReadOnly();
				return searchPanelColumns;
			}
		}
		public ReadOnlyCollection<GridViewColumn> AdaptiveColumns {
			get {
				if(adaptiveColumns == null)
					adaptiveColumns = CreateAdaptiveColumns().AsReadOnly();
				return adaptiveColumns;
			}
		}
		protected new internal GridViewColumnVisualTreeNode VisualTree {
			get {
				if(visualTree == null)
					visualTree = base.VisualTree as GridViewColumnVisualTreeNode;
				return visualTree;
			}
		}
		public new List<GridViewColumnVisualTreeNode> Leafs {
			get {
				if(leafs == null)
					leafs = base.Leafs.OfType<GridViewColumnVisualTreeNode>().ToList();
				return leafs;
			}
		}
		public new List<List<GridViewColumnVisualTreeNode>> Layout {
			get {
				if(layout == null)
					layout = base.Layout.Select(l => l.OfType<GridViewColumnVisualTreeNode>().ToList()).ToList();
				return layout;
			}
		}
		public ReadOnlyCollection<GridViewColumn> FixedColumns {
			get {
				if(fixedColumns == null)
					fixedColumns = new ReadOnlyCollection<GridViewColumn>(CreateFixedColumnList());
				return fixedColumns;
			}
		}
		protected new ASPxGridView Grid { get { return base.Grid as ASPxGridView; } }
		public override void Invalidate() {
			base.Invalidate();
			this.allColumns = null;
			this.allVisibleColumns = null;
			this.allDataColumns = null;
			this.allVisibleDataColumns = null;
			this.filterControlColumns = null;
			this.editColumns = null;
			this.searchPanelColumns = null;
			this.adaptiveColumns = null;
			this.visualTree = null;
			this.leafs = null;
			this.layout = null;
			this.fixedColumns = null;
		}
		protected List<GridViewColumn> CreateAdaptiveColumns() {
			var columns = AllVisibleColumns.Where(c => !(c is GridViewBandColumn)).Reverse().OrderByDescending(col => col.AdaptivePriority).ToList();
			var notSpecifiedCommandColumns = columns.Where(c => c is GridViewCommandColumn && c.AdaptivePriority == 0).ToList();
			if(notSpecifiedCommandColumns.Count > 0) {
				columns.RemoveAll(c => notSpecifiedCommandColumns.Contains(c));
				columns.AddRange(notSpecifiedCommandColumns);
			}
			return columns;
		}
		public new GridViewColumn FindColumnByKey(string key) {
			return base.FindColumnByKey(key) as GridViewColumn;
		}
		public new GridViewColumn FindColumnByString(string caption) {
			return base.FindColumnByString(caption) as GridViewColumn;
		}
		public override bool IsLeaf(IWebGridColumn column) {
			if(!(column is GridViewBandColumn))
				return true;
			return base.IsLeaf(column);
		}
		protected override bool ShowGroupedColumns { get { return Grid.Settings.ShowGroupedColumns; } }
		protected internal override bool UseColumnInVisualTree(IWebGridColumn column) {
			if(Grid.Settings.ShowGroupedColumns || Grid.GroupCount == 0)
				return true;
			var dataColumn = column as IWebGridDataColumn;
			if(dataColumn != null && dataColumn.Adapter.GroupIndex > -1)
				return false;
			return true;
		}
		protected override GridColumnVisualTreeNode CreateTreeNode(IWebGridColumn column, GridColumnVisualTreeNode parent) {
			return new GridViewColumnVisualTreeNode(column, parent);
		}
		protected override bool CanEditColumn(IWebGridDataColumn column) {
			if(!base.CanEditColumn(column)) return false;
			var dataColumn = column as GridViewDataColumn;
			return dataColumn != null && dataColumn.EditFormSettings.Visible != Utils.DefaultBoolean.False;
		}
		protected override bool HasEditTemplate(IWebGridDataColumn column) {
			return (column as GridViewDataColumn).EditItemTemplate != null;
		}
		List<GridViewColumn> CreateFixedColumnList() {
			var result = new List<GridViewColumn>();
			foreach(GridViewColumnVisualTreeNode node in VisualTree.Children) {
				if(node.Column.FixedStyle == GridViewColumnFixedStyle.None)
					continue;
				if(node.Children.Count > 0)
					result.AddRange(FindLeafNodes(node).Select(n => n.Column).OfType<GridViewColumn>());
				else
					result.Add(node.Column);
			}
			return result;
		}
	}
	public class GridViewColumnVisualTreeNode : GridColumnVisualTreeNode {
		List<GridViewColumnVisualTreeNode> childrenEx;
		public GridViewColumnVisualTreeNode(IWebGridColumn column, GridColumnVisualTreeNode parent)
			: base(column, parent) {
		}
		public new GridViewColumn Column { get { return base.Column as GridViewColumn; } }
		public new GridViewColumnVisualTreeNode Parent { get { return base.Parent as GridViewColumnVisualTreeNode; } }
		public List<GridViewColumnVisualTreeNode> ChildrenEx { 
			get {
				if(childrenEx == null || childrenEx.Count != Children.Count)
					childrenEx = Children.OfType<GridViewColumnVisualTreeNode>().ToList();
				return childrenEx;
			}
		}
	}
	public class GridViewFilterHelper : GridFilterHelper {
		public GridViewFilterHelper(ASPxGridView grid)
			: base(grid) {
		}
		protected new ASPxGridView Grid { get { return base.Grid as ASPxGridView; } }
		public virtual CriteriaOperator CreateAutoFilter(IWebGridDataColumn column, string value) {
			if(!column.Adapter.HasAutoFilter) return null;
			CriteriaOperator res = CreateAutoFilterCore(column, value);
			var e = new ASPxGridViewAutoFilterEventArgs(column as GridViewDataColumn, res, GridViewAutoFilterEventKind.CreateCriteria, value);
			Grid.RaiseProcessColumnAutoFilter(e);
			return e.Criteria;
		}
		public Dictionary<string, CriteriaOperator> CreateMultiColumnAutoFilter(Dictionary<GridViewDataColumn, string> values) {
			var args = new ASPxGridViewOnClickRowFilterEventArgs(GridViewAutoFilterEventKind.CreateCriteria);
			foreach(var pair in values) {
				var column = pair.Key;
				var fieldName = column.FieldName;
				args.Values.Add(fieldName, pair.Value);
				var criteriaOperator = CreateAutoFilterCore(column, pair.Value);
				args.Criteria.Add(column.FieldName, criteriaOperator);
			}
			Grid.RaiseProcessOnClickRowFilter(args);
			return args.Criteria;
		}
		public string GetColumnAutoFilterText(GridViewDataColumn column, CriteriaOperator criteria) {
			return GetColumnAutoFilterText(GetColumnAutoFilterCondition(column), column.FieldName, criteria, GetEditKind(column) == GridColumnEditKind.DateEdit);
		}
		public AutoFilterCondition GetColumnAutoFilterCondition(IWebGridDataColumn column) {
			var adapter = column.Adapter;
			return GetColumnAutoFilterCondition(adapter.AdvancedSettings.AutoFilterCondition, adapter.FilterMode, GetEditKind(column), adapter.DataType, IsServerMode);
		}
		public AutoFilterCondition GetDefaultAutoFilterCondition(GridViewDataColumn column) {
			return GetDefaultAutoFilterCondition(column.GetDataType(), GetEditKind(column), column.Settings.FilterMode, IsServerMode);
		}
		public FilterRowTypeKind GetFilterRowTypeKind(GridViewDataColumn column) {
			return GetFilterRowTypeKind(column.GetDataType(), GetEditKind(column), column.Settings.FilterMode, IsServerMode);
		}
		protected virtual CriteriaOperator CreateAutoFilterCore(IWebGridDataColumn column, string value) {
			return CreateAutoFilter(GetColumnAutoFilterCondition(column), column.FieldName, column.Adapter.DataType, value, GetEditKind(column) == GridColumnEditKind.DateEdit);
		}
	}
	public class GridViewBatchEditHelper : GridBatchEditHelper {
		public GridViewBatchEditHelper(ASPxGridView grid)
			: base(grid) {
		}
		public new ASPxGridView Grid { get { return base.Grid as ASPxGridView; } protected set { base.Grid = value; } }
		public new GridViewRenderHelper RenderHelper { get { return base.RenderHelper as GridViewRenderHelper; } }
		protected override bool HasEditItemTemplate(IWebGridDataColumn column) {
			var dataColumn = column as GridViewDataColumn;
			return dataColumn != null && dataColumn.EditItemTemplate != null;
		}
		public override void CreateTemplateEditor(IWebGridDataColumn column, WebControl container) {
			RenderHelper.AddEditItemTemplateControl(-1, column as GridViewDataColumn, container); 
		}
	}
	public class GridViewEndlessPagingHelper : GridEndlessPagingHelper {
		const string GroupStateKey = "groupState";
		public GridViewEndlessPagingHelper(ASPxGridView grid)
			: base(grid) {
			ClientGroupInfoList = new List<EndlessPagingGroupInfo>();
		}
		public new ASPxGridView Grid { get { return base.Grid as ASPxGridView; } }
		public new GridViewRenderHelper RenderHelper { get { return base.RenderHelper as GridViewRenderHelper; } }
		protected new GridViewColumnsState ClientColumnsState { get { return base.ClientColumnsState as GridViewColumnsState; } set { base.ClientColumnsState = value; } }
		protected internal List<EndlessPagingGroupInfo> ClientGroupInfoList { get; set; }
		protected override bool ShowNewRowAtBottom { get { return Grid.SettingsEditing.NewItemRowPosition == GridViewNewItemRowPosition.Bottom; } }
		public override int ValidateVisibleIndex(string command, int clientIndex, ref int offset) {
			if(ShouldLoadFirstPage)
				return clientIndex;
			switch(command) {
				case GridViewCallbackCommand.ExpandRow:
				case GridViewCallbackCommand.CollapseRow:
				case GridViewCallbackCommand.ShowDetailRow:
				case GridViewCallbackCommand.HideDetailRow:
					var index = GetServerIndex(clientIndex);
					if(index < 0) {
						LoadFirstPage();
						return clientIndex;
					}
					offset = index - clientIndex;
					return index;
			}
			return clientIndex;
		}
		protected override void ProcessCallbackCore(string command, params object[] args) {
			base.ProcessCallbackCore(command, args);
			switch(command) {
				case GridViewCallbackCommand.ExpandRow:
				case GridViewCallbackCommand.CollapseRow:
					GroupRowExpandChanged(Convert.ToInt32(args[0]), Convert.ToInt32(args[1]));
					break;
				case GridViewCallbackCommand.ShowDetailRow:
				case GridViewCallbackCommand.HideDetailRow:
					DetailRowExpandChanged(Convert.ToInt32(args[0]), Convert.ToInt32(args[1]));
					break;
			}
			CheckTotalSummaryValueChanged(command);
		}
		protected void GroupRowExpandChanged(int serverIndex, int offset) {
			LoadRowsFromIndex(serverIndex - offset, serverIndex);
			RemoveEditForm = Grid.IsNewRowEditing && ShowNewRowAtBottom;
		}
		protected void DetailRowExpandChanged(int serverIndex, int offset) {
			ReplaceRow(serverIndex - offset, serverIndex);
			RemoveEditForm = false;
		}
		protected void CheckTotalSummaryValueChanged(string command) {
			if(!Grid.Settings.ShowFooter)
				return;
			var editComplete = command == GridViewCallbackCommand.UpdateEdit && !Grid.IsEditing;
			if(!editComplete && command != GridViewCallbackCommand.DeleteRow)
				return;
			var visibleSummaryItems = Grid.ColumnHelper.AllVisibleColumns.SelectMany(c => Grid.GetVisibleTotalSummaryItems(c)).ToList();
			if(visibleSummaryItems.Count > 0)
				LoadFirstPage();
		}
		protected override bool CanUsePartialLoad() {
			if(!base.CanUsePartialLoad())
				return false;
			foreach(var state in ClientColumnsState.FilterConditionList) {
				if(state.Condition != state.Column.Settings.AutoFilterCondition)
					return false;
			}
			for(var i = 0; i < ClientColumnsState.SortList.Count; i++) {
				var state = ClientColumnsState.SortList[i];
				if(i < ClientColumnsState.GroupCount && state.Column.Adapter.GroupIndex != i)
					return false;
			}
			return true;
		}
		protected override IDictionary GetCallbackResultObject(string inlineScript) {
			var result = base.GetCallbackResultObject(inlineScript);
			result.Add(GroupStateKey, GetGroupStateCallbackInfo().ToArray());
			return result;
		}
		protected internal GridEndlessPagingCallbackInfo GetGroupStateCallbackInfo() {
			var info = new GridEndlessPagingCallbackInfo();
			if(!PartialLoad || !PassKeysToClient)
				return info;
			var isFit = new Func<EndlessPagingGroupInfo, bool>((g) => {
				return g.Index >= RemoveIndex && g.Index < RemoveIndex + RemoveCount;
			});
			var first = ClientGroupInfoList.FirstOrDefault(isFit);
			var last = ClientGroupInfoList.LastOrDefault(isFit);
			if(first != null && last != null) {
				info.RemoveIndex = ClientGroupInfoList.IndexOf(first);
				info.RemoveCount = ClientGroupInfoList.IndexOf(last) - info.RemoveIndex + 1;
			}
			var groupInfo = ClientGroupInfoList.FirstOrDefault(g => g.Index >= AddIndex);
			info.AddIndex = groupInfo != null ? ClientGroupInfoList.IndexOf(groupInfo) : ClientGroupInfoList.Count;
			info.Content = GetGroupState();
			return info;
		}
		public override void LoadClientState(GridColumnsState columnsState, ArrayList clientKeyValues, ArrayList groupState) {
			base.LoadClientState(columnsState, clientKeyValues, groupState);
			LoadGroupState(groupState);
		}
		protected void LoadGroupState(ArrayList state) {
			if(state == null) return;
			ClientGroupInfoList.AddRange(state.OfType<string>().Select(s => EndlessPagingGroupInfo.Deserialize(s)));
		}
		protected override internal IList GetGroupState() {
			var groupInfoList = new List<string>();
			for(var i = DataProxy.VisibleStartIndex; i < DataProxy.VisibleStartIndex + DataProxy.VisibleRowCountOnPage; i++) {
				if(DataProxy.GetRowType(i) == WebRowType.Data)
					continue;
				var info = new EndlessPagingGroupInfo() { Key = DataProxy.GetGroupRowValue(i), Index = i };
				var parentRows = DataProxy.GetParentGroupRows(i);
				if(parentRows.Count > 0)
					info.ParentIndex = parentRows.Last();
				groupInfoList.Add(EndlessPagingGroupInfo.Serialize(info));
			}
			return groupInfoList;
		}
		protected override int GetServerIndex(int clientIndex) {
			if(IsGroupRow(clientIndex))
				return GetGroupRowServerIndex(clientIndex);
			return base.GetServerIndex(clientIndex);
		}
		protected bool IsGroupRow(int index) {
			return ClientGroupInfoList.Any(g => g.Index == index);
		}
		int GetGroupRowServerIndex(int clientIndex) {
			var groupLine = GetGroupLine(clientIndex);
			var index = clientIndex;
			for(var i = 0; i < groupLine.Count; i++) {
				index = FindGroupServerIndex(groupLine[i], index, i);
				if(index < 0)
					return -1;
			}
			return index;
		}
		List<EndlessPagingGroupInfo> GetGroupLine(int clientIndex) {
			var list = new List<EndlessPagingGroupInfo>();
			var index = clientIndex;
			while(index >= 0) {
				var groupInfo = ClientGroupInfoList.FirstOrDefault(g => g.Index == index);
				if(groupInfo == null)
					break;
				index = groupInfo.ParentIndex;
				list.Add(groupInfo);
			}
			list.Reverse();
			return list;
		}
		int FindGroupServerIndex(EndlessPagingGroupInfo info, int startIndex, int level) {
			DataProxy.DoOwnerDataBinding();
			bool up = level == 0;
			bool down = true;
			for(var i = 0; up || down; i++) {
				var count = (up && down ? 2 : 1);
				for(var j = 0; j < count; j++) {
					var delta = i;
					if(up && down && j == 0 || up && !down)
						delta = -i;
					var index = startIndex + delta;
					if(DataProxy.GetRowType(index) == WebRowType.Data)
						continue;
					if(DataProxy.GetGroupRowInfo(index).Level != level)
						continue;
					var groupValue = DataProxy.GetGroupRowValue(index);
					if(object.Equals(groupValue, info.Key))
						return index;
					if(up && index == 0)
						up = false;
					if(down && index == DataProxy.VisibleRowCount - 1)
						down = false;
				}
			}
			return -1;
		}
		protected override string RenderDataTable(bool skipFirstRow) {
			if(DataItemsContainer != null)
				new BottomBorderRemovalHelper(DataItemsContainer.Items.OfType<TableRow>().ToList(), RenderHelper).Run();
			return base.RenderDataTable(skipFirstRow);
		}
	}
	public class EndlessPagingGroupInfo {
		public EndlessPagingGroupInfo() {
			ParentIndex = -1;
		}
		public object Key { get; set; }
		public int Index { get; set; }
		public int ParentIndex { get; set; }
		public static string Serialize(EndlessPagingGroupInfo info) {
			using(var stream = new MemoryStream())
			using(var writer = new TypedBinaryWriter(stream)) {
				writer.WriteTypedObject(info.Key);
				writer.WriteObject(info.Index);
				writer.WriteObject(info.ParentIndex);
				return Convert.ToBase64String(stream.ToArray());
			}
		}
		public static EndlessPagingGroupInfo Deserialize(string source) {
			using(var stream = new MemoryStream(Convert.FromBase64String(source)))
			using(var reader = new TypedBinaryReader(stream)) {
				var info = new EndlessPagingGroupInfo();
				info.Key = reader.ReadTypedObject();
				info.Index = reader.ReadObject<int>();
				info.ParentIndex = reader.ReadObject<int>();
				return info;
			}
		}
	}
}
