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
using System.IO.Compression;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Data;
using DevExpress.Data.IO;
using DevExpress.Web.Data;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxTreeList.Internal {
	public interface IWebTreeListData {
		string FocusedNodeKey { get; set; }		
		bool IsNodeSelected(string key);
		void SetNodeSelected(string key, bool value);
		bool IsNodeExpanded(string key);
		void SetNodeExpanded(string key, bool value);
		CheckState GetNodeCheckState(string key);
		void RegisterUsedFieldName(string fieldName);
		void RegisterFieldType(string name, Type type);
		void PageToNode(string key);
		string EditingKey { get; }
		object GetEditingValue(string fieldName, object defaultValue);
		void ResetVisibleData();
	}
	public class TreeListDataHelper : IWebTreeListData {
		protected class SummaryStorageItem : Dictionary<TreeListSummaryItem, TreeListSummaryValue> { }
		protected class SummaryStorage : Dictionary<string, SummaryStorageItem> {
		}
		Dictionary<string, TreeListNode> keyMap;
		StringSet expandedKeys, selectedKeys;
		StringSet usedFieldNames;
		bool summaryCalculated;
		SummaryStorage summaryData;
		List<TreeListRowInfo> rows;
		bool rowsCreated;		
		int maxVisibleLevel;
		TreeListRowsCreationFlags rowsCreationFlags;
		Dictionary<TreeListNode, bool> completeVirtualTreeTrace;
		int totalRowCount;
		int pageIndex;
		string focusedNodeKey;
		bool requireCheckPageIndex;
		ASPxTreeList treeList;
		int autoId;
		bool updating;
		int nodeCounter;
		Dictionary<string, Type> fieldTypes;
		public TreeListDataHelper(ASPxTreeList treeList) {
			this.treeList = treeList;
			this.keyMap = new Dictionary<string, TreeListNode>();
			this.expandedKeys = new StringSet();
			this.selectedKeys = new StringSet();
			this.summaryCalculated = false;
			this.summaryData = new SummaryStorage();
			this.rows = new List<TreeListRowInfo>();
			this.rowsCreated = false;			
			this.maxVisibleLevel = 0;
			this.rowsCreationFlags = TreeListRowsCreationFlags.Empty;
			this.completeVirtualTreeTrace = new Dictionary<TreeListNode, bool>();
			this.totalRowCount = 0;
			this.pageIndex = 0;
			this.focusedNodeKey = null;
			this.requireCheckPageIndex = false;
			this.updating = false;
			this.nodeCounter = 0;
			this.usedFieldNames = new StringSet();			
			this.fieldTypes = new Dictionary<string, Type>();
		}
		#region Public Properties
		public TreeListRowsCreationFlags RowsCreationFlags {
			get { return rowsCreationFlags; }
			set {
				if(value == rowsCreationFlags) return;
				rowsCreationFlags = value;
				this.rowsCreated = false;
			}
		}
		public List<TreeListRowInfo> Rows {
			get {
				EnsureRows();
				return rows;
			}
		}
		public int MaxVisibleLevel {
			get {
				EnsureRows();
				return maxVisibleLevel;
			}
		}
		public string FocusedNodeKey {
			get { return focusedNodeKey; }
			set {
				if(value != FocusedNodeKey) {
					focusedNodeKey = value;
					ResetVisibleData();
				}
			}
		}
		public int PageSize {
			get { return TreeList.SettingsPager.PageSize; }
			set {
				if(PageSize != value && value > 0) {
					TreeList.SettingsPager.PageSize = value;
					this.requireCheckPageIndex = true;
					ResetVisibleData();
				}
			}
		}
		protected internal bool IsPageSizeItemVisible() {
			return TreeList.SettingsPager.PageSizeItemSettings.Visible;
		}
		protected internal bool PagerIsValidPageIndex(int newIndex) {
			if(newIndex == -1) {
				if(TreeList.SettingsPager.Visible && TreeList.SettingsPager.Mode == TreeListPagerMode.ShowPager)
					return TreeList.SettingsPager.AllButton.Visible || IsPageSizeAllItemVisible();
				return false;
			}
			return true;
		}
		protected internal bool PagerIsValidPageSize(int newSize) {
			if(newSize == -1)
				return PagerIsValidPageIndex(-1);
			if(TreeList.SettingsPager.PageSizeItemSettings.Visible)
				return newSize == TreeList.InitialPageSize ||
					   Array.Exists<string>(TreeList.SettingsPager.PageSizeItemSettings.Items, delegate(string item) { return item == newSize.ToString(); });
			return false;
		}
		public bool IsPageSizeVisible() {
			return TreeList.SettingsPager.PageSizeItemSettings.Visible;
		}
		public bool IsPageSizeAllItemVisible() {
			return IsPageSizeVisible() && TreeList.SettingsPager.PageSizeItemSettings.ShowAllItem;
		}
		public int TotalRowCount {
			get {
				EnsureRows();
				return totalRowCount;
			}
		}
		public int PageIndex {
			get { return Math.Min(pageIndex, Math.Max(PageCount - 1, 0)); }
			set {
				if(value == PageIndex)
					return;
				int max = PageCount - 1;
				if(value > max) 
					value = max;				
				ForcePageIndex(value);
			}
		}
		public int PageCount {
			get {
				if(IsPageSizeVisible() && this.pageIndex == -1)
					return 1;
				int value = TotalRowCount / PageSize;
				if(TotalRowCount % PageSize > 0)
					value++;
				return value;
			}
		}
		public int SelectionCount { 
			get { 
				int value = SelectedKeys.Count;
				if(SelectedKeys.Contains(TreeListRenderHelper.RootNodeKey))
					value--;
				return value;
			} 
		}
		#endregion
		#region Internal properties
		protected ASPxTreeList TreeList { get { return treeList; } }
		protected Dictionary<string, TreeListNode> KeyMap { get { return keyMap; } }
		protected StringSet ExpandedKeys { get { return expandedKeys; } }
		protected StringSet SelectedKeys { get { return selectedKeys; } }
		protected SummaryStorage SummaryData { get { return summaryData; } }		
		protected StringSet UsedFieldNames { get { return usedFieldNames; } }
		protected bool PagingEnabled { get { return TreeList.SettingsPager.Mode == TreeListPagerMode.ShowPager; } }
		protected bool AutoFocusFirstNode { get { return TreeList.RenderHelper.IsFocusedNodeEnabled && TreeList.SettingsBehavior.FocusNodeOnLoad; } }
		protected Dictionary<string, Type> FieldTypes { get { return fieldTypes; } }		
		protected Dictionary<TreeListNode, bool> CompleteVirtualTreeTrace { get { return completeVirtualTreeTrace; } }
		#endregion
		void ForcePageIndex(int value) {
			this.pageIndex = value;
			FocusedNodeKey = null;
			ResetVisibleData();
		}
		#region Data loading
		public void LoadLinearData(IEnumerable data) {
			ClearNodes();
			if(data == null) return;
			int autoId = 0;
			bool usePk = false;
			bool modeSelected = false;
			Dictionary<object, TreeListNode> tempMap = new Dictionary<object, TreeListNode>();
			string keyFieldName = TreeList.KeyFieldName;
			string parentFieldName = TreeList.ParentFieldName;
			foreach(object item in data) {
				object key = null;
				if(!modeSelected) {
					if(!String.IsNullOrEmpty(keyFieldName))
						key = TreeListUtils.GetPropertyValue(item, keyFieldName);
					if(key != null)
						usePk = true;
					modeSelected = true;
				}
				if(usePk) {
					key = TreeListUtils.GetPropertyValue(item, keyFieldName);
					if(key == null)
						throw new MissingPrimaryKeyException();
				} else {
					key = autoId++;
				}
				TreeListNode node = CreateNode(key);
				node.DataItemInternal = new TreeListBoundNodeDataItem(this, item);
				tempMap[key] = node;				
			}
			foreach(object key in tempMap.Keys) {
				TreeListNode node = tempMap[key];
				object parentKey = null;
				if(!String.IsNullOrEmpty(parentFieldName))
					parentKey = node.GetValue(parentFieldName);
				if(parentKey != null && tempMap.ContainsKey(parentKey)) {
					if(TreeList.IsRootValueAssigned && Object.Equals(parentKey, TreeList.RootValue)) {
						TreeList.RootNode.AppendChild(node);
					} else {
						if(!tempMap[parentKey].AppendChild(node))
							TreeList.RootNode.AppendChild(node);
					}
				} else {
					if(!TreeList.IsRootValueAssigned || Object.Equals(parentKey, TreeList.RootValue))
						TreeList.RootNode.AppendChild(node);
					else
						UnregisterNode(node, true);
				}
			}
		}
		public void LoadVirtualData() {
			LoadVirtualData(TreeList.RootNode);
		}
		public void LoadVirtualData(TreeListNode rootNode) {
			ClearNodes(rootNode);
			CompleteVirtualTree(rootNode);
		}
		protected void CompleteVirtualTree(TreeListNode rootNode) {
			if(!CanCompleteVirtualNode(rootNode))
				return;
			CompleteVirtualTreeTrace[rootNode] = true;
			ClearNodes(rootNode); 
			TreeListNodeIterator iterator = new TreeListNodeIterator(rootNode);
			TreeListNode node = rootNode;
			do {
				if(node.Expanded) {
					object nodeObject = node is TreeListRootNode 
						? null 
						: (node as TreeListVirtualNode).NodeObject;
					TreeListVirtualModeCreateChildrenEventArgs childrenArgs = new TreeListVirtualModeCreateChildrenEventArgs(nodeObject);
					TreeList.RaiseVirtualModeCreateChildren(childrenArgs);
					IList children = childrenArgs.Children;
					if(children != null) {
						foreach(object childObject in children) {
							TreeListVirtualModeNodeCreatingEventArgs nodeArgs = new TreeListVirtualModeNodeCreatingEventArgs(this, childObject);
							TreeList.RaiseVirtualModeNodeCreating(nodeArgs);
							TreeListVirtualNode childNode = CreateVirtualNode(nodeArgs);														
							node.AppendChild(childNode);							
							TreeList.RaiseVirtualModeNodeCreated(childNode, childObject);
						}
					}
				}
				node = iterator.GetNext();
			} while(node != null);
			AddFakeNewNode(false);
			TreeList.DoSortSubtree(rootNode);
			CompleteVirtualTreeTrace.Remove(rootNode);
		}		
		bool CanCompleteVirtualNode(TreeListNode node) {
			foreach(TreeListNode lockedNode in CompleteVirtualTreeTrace.Keys) {
				TreeListNode current = node;
				while(current != null) {
					if(current == lockedNode)
						return false;
					current = current.ParentNode;
				}
			}
			return true;
		}
		public void LoadHierarchicalData(IHierarchicalEnumerable data) {
			ClearNodes();		
			this.autoId = 0;
			LoadHierarchicalDataCore(TreeList.RootNode, data);
		}
		protected void LoadHierarchicalDataCore(TreeListNode parent, IHierarchicalEnumerable data) {
			if(data == null) return;
			foreach(object item in data) {
				IHierarchyData hierarchyData = data.GetHierarchyData(item);
				TreeListNode node = CreateNode(this.autoId++);
				node.DataItemInternal = new TreeListBoundNodeDataItem(this, item);				
				parent.AppendChild(node);
				if(hierarchyData.HasChildren)
					LoadHierarchicalDataCore(node, hierarchyData.GetChildren());
			}
		}
		#endregion
		#region Selection
		public void ClearSelectedKeys() {
			SelectedKeys.Clear();
			ResetVisibleData();
		}
		public void ToggleNodeSelection(string key) {
			if(IsNodeSelected(key))
				SetNodeSelected(key, false);
			else
				SetNodeSelected(key, true);
		}
		public CheckState GetNodeCheckState(string key) {
			if(this.TreeList.SettingsSelection.Recursive) {
				TreeListNode node = GetNodeByKey(key);
				return GetNodeCheckStateByChildren(node);
			}
			return IsNodeSelected(key) ? CheckState.Checked : CheckState.Unchecked;
		}
		public bool IsNodeSelected(string key) {
			return SelectedKeys.Contains(key);
		}
		public void SetNodeSelected(string key, bool value) {
			TreeList.EnsureNodesCreated();
			if(value) {
				SelectedKeys.Add(key);
				UpdateRestSelection(key, true);
				ResetVisibleData();
			} else {
				SelectedKeys.Remove(key);
				UpdateRestSelection(key, false);
				ResetVisibleData();
			}
		}
		protected void UpdateRestSelection(string key, bool value) {
			if(key == TreeListRenderHelper.RootNodeKey && TreeList.SettingsSelection.AllowSelectAll && !value)
				SelectedKeys.Clear();
			if(key == TreeListRenderHelper.RootNodeKey || TreeList.SettingsSelection.Recursive) {
				UpdateChildSelection(key, value);
				UpdateParentSelection(key, value);
			} else if(key != TreeListRenderHelper.RootNodeKey && TreeList.SettingsSelection.AllowSelectAll) {
				if(value)
					SelectIfChildrenSelected(TreeList.RootNode);
				else
					SelectedKeys.Remove(TreeListRenderHelper.RootNodeKey);					
			}
		}
		protected void UpdateChildSelection(string key, bool value) {
			TreeListNode node = GetNodeByKey(key);
			if(node == null)
				return;
			TreeListNodeIterator iterator = new TreeListNodeIterator(node);
			TreeListNode current;
			while(true) {
				current = iterator.GetNext();
				if(current == null)
					break;
				if(value) {
					if(current.AllowSelect)
						SelectedKeys.Add(current.Key);
				} else {
					SelectedKeys.Remove(current.Key);
				}
			}
		}
		protected void UpdateParentSelection(string key, bool value) {
			TreeListNode node = GetNodeByKey(key);
			if(node == null)
				return;
			TreeListNode parent = node.ParentNode;
			while(parent != null) {
				if(value)
					SelectIfChildrenSelected(parent);
				else
					SelectedKeys.Remove(parent.Key);					
				parent = parent.ParentNode;
			}
		}
		protected CheckState GetNodeCheckStateByChildren(TreeListNode node) {
			if(SelectedKeys.Contains(node.Key))
				return CheckState.Checked;
			if(!node.HasChildren)
				return CheckState.Unchecked;
			TreeListNodeIterator iterator = new TreeListNodeIterator(node);
			bool? childrenSelected = null;
			bool hasSelectedChild = false;
			TreeListNode current;
			while(true) {
				current = iterator.GetNext();
				if(current == null)
					break;
				if(!current.AllowSelect)
					continue;
				if(!SelectedKeys.Contains(current.Key)) 
					childrenSelected = false;
				else {
					hasSelectedChild = true;
					childrenSelected = childrenSelected ?? true;
				}
			}
			if(childrenSelected.HasValue && childrenSelected.Value && node.AllowSelect)
				return CheckState.Checked;
			return hasSelectedChild ? CheckState.Indeterminate : CheckState.Unchecked;
		}
		protected void SelectIfChildrenSelected(TreeListNode node) {
			if(GetNodeCheckStateByChildren(node) == CheckState.Checked) 
				SelectedKeys.Add(node.Key);
		}
		#endregion
		#region Expanded keys
		public bool IsNodeExpanded(string key) {
			return ExpandedKeys.Contains(key);
		}
		public void SetNodeExpanded(string key, bool value) {
			if(String.IsNullOrEmpty(key)) return;
			if(value && !IsNodeExpanded(key)) {
				ExpandedKeys.Add(key);
				if(TreeList.IsVirtualMode()) {
					TreeListNode node = GetNodeByKey(key);
					if(node != null)
						CompleteVirtualTree(node);
				}
				ResetVisibleData();				
			} else if (!value && IsNodeExpanded(key)) {
				ExpandedKeys.Remove(key);				
				this.requireCheckPageIndex = true;
				ResetVisibleData();
			}
		}
		public void ClearExpandedKeys() {
			ExpandedKeys.Clear();
			this.requireCheckPageIndex = true;
			ResetVisibleData();
		}
		#endregion
		#region Summary
		public object GetSummaryValue(string nodeKey, TreeListSummaryItem item) {			
			EnsureSummaryData();
			if(!SummaryData.ContainsKey(nodeKey))
				return null;
			if(!SummaryData[nodeKey].ContainsKey(item))
				return null;
			return SummaryData[nodeKey][item].Value;
		}
		protected void EnsureSummaryData() {
			if(!this.summaryCalculated && !this.updating)
				CalcSummary();
		}
		protected void CalcSummary() {
			BeginUpdate();
			try {
				SummaryData.Clear();
				TreeListNodeIterator iterator = TreeList.CreateNodeIterator();
				TreeListNode node;
				while((node = iterator.GetNext()) != null) {
					foreach(TreeListSummaryItem item in TreeList.Summary) {
						if(item.SummaryType == SummaryItemType.None)
							continue;
						UpdateSummaryValue(node, item, node);
					}
				}
				foreach(string key in SummaryData.Keys) {
					SummaryStorageItem storageItem = SummaryData[key];
					foreach(TreeListSummaryValue summaryValue in storageItem.Values)
						summaryValue.Finish();
				}
				this.summaryCalculated = true;
			} finally {
				EndUpdate();
			}
		}		
		protected void UpdateSummaryValue(TreeListNode summaryOwner, TreeListSummaryItem item, TreeListNode node) {
			TreeListNode parent = summaryOwner.ParentNode;
			if(parent == null)
				return;
			string key = parent.Key;
			if(!SummaryData.ContainsKey(key))
				SummaryData[key] = new SummaryStorageItem();
			TreeListSummaryValue value;
			if(!SummaryData[key].ContainsKey(item)) {
				value = CreateSummaryValue(item);
				value.Start();
				SummaryData[key][item] = value;
			} else {
				value = SummaryData[key][item];
			}
			value.Calculate(node);
			if(item.Recursive)
				UpdateSummaryValue(parent, item, node);
		}
		protected TreeListSummaryValue CreateSummaryValue(TreeListSummaryItem item) {
			switch(item.SummaryType) {
				case SummaryItemType.Count:
					return new TreeListSummaryCountValue(item);
				case SummaryItemType.Min:
					return new TreeListSummaryMinValue(item);
				case SummaryItemType.Max:
					return new TreeListSummaryMaxValue(item);
				case SummaryItemType.Sum:
					return new TreeListSummarySumValue(item);
				case SummaryItemType.Average:
					return new TreeListSummaryAvgValue(item);
				case SummaryItemType.Custom:
					return new TreeListSummaryCustomValue(item, TreeList);
				default:
					throw new ArgumentException();
			}
		}
		#endregion
		#region Rows
		protected void CreateRows() {
			BeginUpdate();
			try {
				Rows.Clear();
				int rowIndex = 0;
				int firstIndex = PageIndex * PageSize;
				int lastIndex = firstIndex + PageSize - 1;
				int max = 0;
				TreeListNodeIterator iterator = TreeList.CreateNodeIterator();
				TreeListNode node;
				while(true) {
					node = GetNextVisibleNode(iterator);
					if(node == null) break;
					if(HasRowsCreationFlag(TreeListRowsCreationFlags.IgnorePaging) 
						|| !PagingEnabled 
						|| PageIndex < 0 
						|| rowIndex >= firstIndex && rowIndex <= lastIndex) {
						if(FocusedNodeKey == null && AutoFocusFirstNode)
							focusedNodeKey = node.Key;
						TreeListRowInfo row = TreeListRowInfo.CreateFromNode(node);
						TreeListUtils.CalcRowIndent(node, row.Indents, TreeList.Settings.ShowRoot);
						Rows.Add(row);
						int level = node.Level;
						if(level > max)
							max = level;
					}
					rowIndex++;
				}
				this.maxVisibleLevel = max;
				this.totalRowCount = rowIndex;
				this.rowsCreated = true;
			} finally {
				EndUpdate();
			}
		}
		TreeListNode GetNextVisibleNode(TreeListNodeIterator iterator) {
			if(HasRowsCreationFlag(TreeListRowsCreationFlags.ExpandAll))
				return iterator.GetNext();
			return iterator.GetNextVisible();
		}
		protected void EnsureRows() {
			if(!this.rowsCreated && !this.updating) {
				int savedTotalRowCount = this.totalRowCount;
				CreateRows();
				if(savedTotalRowCount > TotalRowCount)
					this.requireCheckPageIndex = true;
				if(this.requireCheckPageIndex && this.pageIndex >= PageCount) {
					PageIndex = Math.Max(PageCount - 1, 0);
					CreateRows();
					this.requireCheckPageIndex = false;
				}
			}
		}
		bool HasRowsCreationFlag(TreeListRowsCreationFlags flag) {
			return (RowsCreationFlags & flag) == flag;
		}
		#endregion
		#region Callback state
		public void BeginUpdate() {
			this.updating = true;
		}
		public void EndUpdate() {
			this.updating = false;
		}
		public string GetStateString(bool saveData, bool compress) {
			using(MemoryStream stream = new MemoryStream()) {
				if(compress) {
					using(BufferedStream deflator = new BufferedStream(new DeflateStream(stream, CompressionMode.Compress, true))) {
						SaveStateCore(deflator, saveData);
					}
				} else {
					SaveStateCore(stream, saveData);
				}
				return Convert.ToBase64String(stream.ToArray());
			}
		}
		public void SetStateString(string state, bool compressed) {
			using(MemoryStream stream = new MemoryStream(Convert.FromBase64String(state))) {
				stream.Position = 0;
				if(compressed) {
					using(DeflateStream deflator = new DeflateStream(stream, CompressionMode.Decompress, true)) {
						LoadStateCore(deflator);
					}
				} else {
					LoadStateCore(stream);
				}
			}
		}
		protected void SaveStateCore(Stream stream, bool saveData) {
			using(TypedBinaryWriter writer = new TypedBinaryWriter(stream)) {
				SaveColumns(writer);
				SaveParams(writer);				
				SaveNodeStates(writer, true, true);
				SaveFieldTypes(writer);
				if(saveData)
					SaveRows(writer);
				else
					writer.WriteObject(0);
			}
		}
		protected void LoadStateCore(Stream stream) {
			using(TypedBinaryReader reader = new TypedBinaryReader(stream)) {
				LoadColumns(reader);
				BeginUpdate();
				try {
					LoadParams(reader);
					LoadNodeStates(reader);
					LoadFieldTypes(reader);
					LoadRows(reader);
				} finally {
					EndUpdate();
				}
			}
		}
		protected void SaveColumns(TypedBinaryWriter writer) {
			writer.WriteObject(TreeList.Columns.Count);
			foreach(TreeListColumn column in TreeList.Columns)
				SaveColumnState(writer, column);
		}
		protected void LoadColumns(TypedBinaryReader reader) {
			TreeList.ResetVisibleColumns();
			TreeList.ResetSortedColumns();
			int count = reader.ReadObject<int>();
			while(count-- > 0) {
				ColumnState state = ColumnState.CreateFromStream(reader);
				state.Apply(TreeList.Columns);
			}
		}
		class ColumnState {
			int index, visibleIndex, sortIndex;
			bool visible, sortable;
			Unit width;
			ColumnSortOrder sortOrder;
			public static ColumnState CreateFromStream(TypedBinaryReader reader) {
				ColumnState state = new ColumnState();
				state.index = reader.ReadObject<int>();
				state.visible = reader.ReadObject<bool>();
				state.visibleIndex = reader.ReadObject<int>();
				Double widthValue = reader.ReadObject<double>();
				UnitType widthType = UnitType.Pixel;
				if(widthValue > Double.MinValue) {
					widthType = (UnitType)reader.ReadObject<int>();
					state.width = new Unit(widthValue, widthType);
				} else {
					state.width = Unit.Empty;
				}
				state.sortable = reader.ReadObject<bool>();
				if(state.sortable) {
					state.sortIndex = reader.ReadObject<int>();
					state.sortOrder = (ColumnSortOrder)reader.ReadObject<int>();
				}
				return state;
			}
			public void Apply(TreeListColumnCollection columns) {
				if(this.index > columns.Count - 1)
					return;
				TreeListColumn column = columns[this.index];
				column.SetColVisible(this.visible);
				column.SetColVisibleIndex(this.visibleIndex);
				column.Width = this.width;
				if(this.sortable) {
					TreeListDataColumn dataColumn = column as TreeListDataColumn;
					if(dataColumn != null) {
						dataColumn.SetSortIndex(this.sortIndex);
						dataColumn.SetSortOrder(this.sortOrder);
					}
				}
			}
		}
		protected void SaveColumnState(TypedBinaryWriter writer, TreeListColumn column) {
			writer.WriteObject(column.Index);
			writer.WriteObject(column.Visible);
			writer.WriteObject(column.VisibleIndex);
			if(column.Width.IsEmpty) {
				writer.WriteObject(Double.MinValue);
			} else {
				writer.WriteObject(column.Width.Value);
				writer.WriteObject((int)column.Width.Type);
			}
			TreeListDataColumn dataColumn = column as TreeListDataColumn;
			writer.WriteObject(dataColumn != null);
			if(dataColumn != null) {
				writer.WriteObject(dataColumn.SortIndex);
				writer.WriteObject((int)dataColumn.SortOrder);
			}
		}
		protected void SaveParams(TypedBinaryWriter writer) {
			writer.WriteObject(MaxVisibleLevel);
			writer.WriteObject(TotalRowCount);
			writer.WriteObject(PageIndex);
			writer.WriteObject(PageSize);
			writer.WriteObject(FocusedNodeKey);
			writer.WriteObject(EditingKey);
			writer.WriteObject(NewNodeParentKey);
		}
		protected void LoadParams(TypedBinaryReader reader) {
			this.maxVisibleLevel = reader.ReadObject<int>();
			this.totalRowCount = reader.ReadObject<int>();
			PageIndex = reader.ReadObject<int>();
			PageSize = reader.ReadObject<int>();
			this.focusedNodeKey = reader.ReadObject<string>();
			this.editingKey = reader.ReadObject<string>();
			this.newNodeParentKey = reader.ReadObject<string>();
		}
		protected void SaveFieldTypes(TypedBinaryWriter writer) {
			writer.WriteObject(FieldTypes.Count);
			foreach(string name in FieldTypes.Keys) {
				writer.WriteObject(name);
				writer.WriteType(FieldTypes[name]);
			}
		}
		protected void LoadFieldTypes(TypedBinaryReader reader) {
			FieldTypes.Clear();
			int count = reader.ReadObject<int>();
			while(count-- > 0) {
				string name = reader.ReadObject<string>();
				Type type = reader.ReadType();
				FieldTypes[name] = type;
			}
		}
		public void SaveNodeStates(TypedBinaryWriter writer, bool storeExpanded, bool storeSelected) {
			if(storeExpanded) {
				writer.WriteObject(ExpandedKeys.Count);
				foreach(string key in ExpandedKeys)
					writer.WriteObject(key);
			} else {
				writer.WriteObject(-1);
			}
			if(storeSelected) {
				writer.WriteObject(SelectedKeys.Count);
				foreach(string key in SelectedKeys)
					writer.WriteObject(key);
			} else {
				writer.WriteObject(-1);
			}
		}
		public void LoadNodeStates(TypedBinaryReader reader) {
			int count;
			count = reader.ReadObject<int>();
			if(count > -1) {
				ExpandedKeys.Clear();
				while(count-- > 0)
					ExpandedKeys.Add(reader.ReadObject<string>());				
			}
			count = reader.ReadObject<int>();
			if(count > -1) {
				SelectedKeys.Clear();
				while(count-- > 0)
					SelectedKeys.Add(reader.ReadObject<string>());
			}
		}
		protected void SaveRows(TypedBinaryWriter writer) {
			int count = Rows.Count;
			writer.WriteObject(count);
			if(count < 1)
				return;
			IList<string> fieldNames = UsedFieldNames.ToList();
			writer.WriteObject(fieldNames.Count);
			foreach(string fieldName in fieldNames)
				writer.WriteObject(fieldName);
			bool storeSummary = this.summaryCalculated;
			writer.WriteObject(storeSummary);
			foreach(TreeListRowInfo row in Rows)
				SaveRow(row, writer, fieldNames, storeSummary);
			if(storeSummary)
				SaveRowSummary(TreeListRenderHelper.RootNodeKey, writer);
		}
		protected void LoadRows(TypedBinaryReader reader) {
			Rows.Clear();
			SummaryData.Clear();
			int count = reader.ReadObject<int>();
			if(count < 1) {
				ResetVisibleData();
				return;
			}
			int fieldNameCount = reader.ReadObject<int>();
			List<string> fieldNames = new List<string>();
			for(int i = 0; i < fieldNameCount; i++)
				fieldNames.Add(reader.ReadObject<string>());
			bool restoreSummary = reader.ReadObject<bool>();
			this.summaryCalculated = restoreSummary;
			while(count-- > 0) {
				TreeListRowInfo row;
				LoadRow(out row, reader, fieldNames, restoreSummary);
				Rows.Add(row);
			}
			if(restoreSummary)
				LoadRowSummary(TreeListRenderHelper.RootNodeKey, reader);
			this.rowsCreated = true;
		}
		protected void SaveRow(TreeListRowInfo row, TypedBinaryWriter writer, IList<string> fieldNames, bool storeSummary) {
			writer.WriteObject(row.NodeKey);
			writer.WriteObject(row.HasButton);
			writer.WriteObject(row.AllowSelect);
			writer.WriteObject(row.Indents.Count);
			foreach(TreeListRowIndentType indent in row.Indents)
				writer.WriteObject((int)indent);
			foreach(string fieldName in fieldNames)
				writer.WriteTypedObject(row.GetValue(fieldName));
			if(storeSummary)
				SaveRowSummary(row.NodeKey, writer);
		}
		protected void LoadRow(out TreeListRowInfo row, TypedBinaryReader reader, IList<string> fieldNames, bool restoreSummary) {
			string nodeKey = reader.ReadObject<string>();			
			bool hasButton = reader.ReadObject<bool>();
			bool allowSelect = reader.ReadObject<bool>();
			TreeListUnboundNodeDataItem dataItem = new TreeListUnboundNodeDataItem(this);
			row = new TreeListRowInfo(this, nodeKey, dataItem, hasButton, allowSelect);
			int indentCount = reader.ReadObject<int>();
			for(int i = 0; i < indentCount; i++)
				row.Indents.Add((TreeListRowIndentType)reader.ReadObject<int>());
			foreach(string fieldName in fieldNames)
				dataItem.SetValue(fieldName, reader.ReadTypedObject());				
			if(restoreSummary)
				LoadRowSummary(row.NodeKey, reader);
		}
		protected void SaveRowSummary(string nodeKey, TypedBinaryWriter writer) {
			if(nodeKey == TreeListRenderHelper.NewNodeKey)
				return;
			if(!SummaryData.ContainsKey(nodeKey)) {
				writer.WriteObject(0);
				return;
			}
			SummaryStorageItem storageItem = SummaryData[nodeKey];
			writer.WriteObject(storageItem.Count);
			foreach(TreeListSummaryItem item in storageItem.Keys) {
				writer.WriteObject(item.Index);
				writer.WriteTypedObject(storageItem[item].Value);
			}
		}		
		protected void LoadRowSummary(string nodeKey, TypedBinaryReader reader) {
			if(nodeKey == TreeListRenderHelper.NewNodeKey)
				return;
			SummaryData[nodeKey] = new SummaryStorageItem();
			int count = reader.ReadObject<int>();
			while(count-- > 0) {
				int index = reader.ReadObject<int>();
				TreeListSummaryItem item = TreeList.Summary[index];
				SummaryData[nodeKey].Add(item, new TreeListSummaryCachedValue(item, reader.ReadTypedObject()));
			}			
		}
		#endregion
		#region Data fields
		public void ClearFieldTypes() {
			FieldTypes.Clear();
		}
		public void RegisterFieldType(string name, Type type) {
			FieldTypes[name] = type;
		}
		public void RegisterUsedFieldName(string fieldName) {
			UsedFieldNames.Add(fieldName);			
		}
		public Type GetFieldType(string fieldName) {
			if(FieldTypes.ContainsKey(fieldName))
				return FieldTypes[fieldName];
			return typeof(Object);
		}
		void ResetUsedFieldNames() {
			UsedFieldNames.Clear();			
		}
		#endregion
		#region Nodes
		public TreeListNode GetNodeByKey(string key) {
			if(key == TreeListRenderHelper.RootNodeKey)
				return TreeList.RootNode;
			TreeList.EnsureNodesCreated();
			if(!KeyMap.ContainsKey(key))
				return null;
			return KeyMap[key];
		}		
		public TreeListNode CreateNode(object keyObject) {
			TreeListNode node = new TreeListNode(keyObject, this);
			RegisterNode(node);
			return node;
		}		
		public TreeListVirtualNode CreateVirtualNode(TreeListVirtualModeNodeCreatingEventArgs args) {
			TreeListVirtualNode node = new TreeListVirtualNode(args.NodeKeyValue, this, args.NodeObject, args.IsLeaf);
			if(IsNodeSelected(TreeListRenderHelper.RootNodeKey))
				node.Selected = true;
			RegisterNode(node);
			node.DataItemInternal = args.DataItem;			
			return node;
		}
		public int GetRegisteredNodeCount() {
			return KeyMap.Count;
		}
		public void PageToNode(string key) {
			if(!PagingEnabled)
				return;
			TreeListNodeIterator iter = TreeList.CreateNodeIterator();
			TreeListNode node;
			int rowIndex = 0;
			while((node = iter.GetNextVisible()) != null) {
				if(node.Key == key)
					break;
				rowIndex++;
			}
			PageIndex = rowIndex / PageSize;
		}
		public void ClearNodes() {
			ClearNodes(TreeList.RootNode);
		}
		public void ClearNodes(TreeListNode parentNode) {
			if(parentNode == TreeList.RootNode) {
				foreach(TreeListNode node in KeyMap.Values)
					UnregisterNode(node, false);
				KeyMap.Clear();
			} else {
				List<TreeListNode> list = new List<TreeListNode>();
				TreeListNodeIterator iterator = new TreeListNodeIterator(parentNode);
				while(true) {
					TreeListNode node = iterator.GetNext();
					if(node == null) break;
					list.Add(node);
				}
				foreach(TreeListNode node in list)
					UnregisterNode(node, true);
			}
			parentNode.ChildNodes.Clear();
			ResetVisibleData();
		}
		public ICollection<TreeListNode> GetAllNodes() {
			return KeyMap.Values;
		}
		protected void RegisterNode(TreeListNode node) {
			if(KeyMap.ContainsKey(node.Key)) {
				if(TreeList.DesignMode) return; 
				throw new ArgumentException("Duplicate node key");
			}
			KeyMap[node.Key] = node;
			node.OriginalIndex = this.nodeCounter++;
		}
		protected void UnregisterNode(TreeListNode node, bool clearMap) {
			node.SetParentNode(null);
			node.ChildNodes.Clear();
			if(clearMap && node.Key != null)
				KeyMap.Remove(node.Key);
		}
		#endregion
		#region Editing
		string editingKey = null;
		string newNodeParentKey = null;
		Dictionary<string, object> editingValues = null;
		ASPxDataUpdatingEventArgs updatingArgs = null;
		ASPxDataDeletingEventArgs deletingArgs = null;
		ASPxDataInsertingEventArgs insertingArgs = null;
		string editingNodeError = String.Empty;
		Dictionary<string, string> columnErrors;
		bool isNodeMoving = false;
		public string EditingKey { get { return editingKey; } }
		public string NewNodeParentKey { get { return newNodeParentKey; } }
		public bool IsEditing { get { return EditingKey != null || IsNewNodeEditing; } }
		public bool IsNewNodeEditing { get { return NewNodeParentKey != null; } }
		public string EditingNodeError { get { return editingNodeError; } }
		public virtual string DefaultEditingNodeError { get { return String.Empty; } }
		public Dictionary<string, string> ColumnErrors {
			get {
				if(columnErrors == null)
					columnErrors = new Dictionary<string, string>();
				return columnErrors;
			}
		}
		protected ASPxDataUpdatingEventArgs UpdatingArgs { get { return updatingArgs; } }
		protected ASPxDataDeletingEventArgs DeletingArgs { get { return deletingArgs; } }
		protected ASPxDataInsertingEventArgs InsertingArgs { get { return insertingArgs; } }
		protected Dictionary<string, object> EditingValues {
			get {
				if(editingValues == null)
					editingValues = new Dictionary<string, object>();
				return editingValues;
			}
		}
		public void SetEditingValue(string fieldName, object value) {
			EditingValues[fieldName] = ConvertEditingValue(value, fieldName);
		}
		public object GetEditingValue(string fieldName, object defaultValue) {
			if(EditingValues.ContainsKey(fieldName))
				return EditingValues[fieldName];
			return defaultValue;			
		}
		public void StartEdit(string key) {
			CancelEdit();
			this.editingKey = key;
			TreeListNodeEditingEventArgs e = new TreeListNodeEditingEventArgs(key);
			TreeList.RaiseStartNodeEditing(e);
			if(e.Cancel)
				CancelEdit();			
		}
		public void CancelEdit() {
			if(!IsEditing) return;
			TreeListNodeEditingEventArgs e = new TreeListNodeEditingEventArgs(EditingKey);
			TreeList.RaiseCancelNodeEditing(e);
			if(!e.Cancel)
				ResetEditing();
		}
		public bool CommitEdit() {
			if(!IsEditing) 
				return false;
			if(!DoNodeValidation())
				return false;
			if(IsNewNodeEditing)
				DoInsertNodeData();
			else
				DoUpdateNodeData();			
			return true;
		}		
		public void DeleteNode(string key, bool allowRecursive) {
			if(String.IsNullOrEmpty(key))
				throw new ArgumentException();
			CancelEdit();
			TreeListNode node = GetNodeByKey(key);
			if(node == null)
				return;
			if(node.HasChildren && !allowRecursive)
				throw new InvalidOperationException(TreeList.SettingsText.RecursiveDeleteError); 
			List<TreeListNode> nodeList = new List<TreeListNode>();			
			TreeListNodeIterator iterator = new TreeListNodeIterator(node);
			while(node != null) {
				nodeList.Add(iterator.Current);
				node = iterator.GetNext();
			}
			iterator = null;
			DataSourceView view = TreeList.GetDataSourceView();
			for(int i = nodeList.Count - 1; i >= 0; i--) {
				if(!DeleteNodeCore(view, nodeList[i]))
					break;
			}
			if(FocusedNodeKey != null && GetNodeByKey(FocusedNodeKey) == null)
				FocusedNodeKey = null;
		}
		public virtual bool MoveNode(string key, string parentKey) {
			TreeListNode
				node = GetNodeByKey(key),
				newParent = GetNodeByKey(parentKey);
			if(node == null || newParent == null 
				|| node.ParentNode == newParent
				|| TreeListUtils.TestParentChildRelationship(node, newParent))
				return false;
			if(newParent is TreeListRootNode)
				parentKey = null;
			CancelEdit();			
			TreeListNodeDragEventArgs e = new TreeListNodeDragEventArgs(node, newParent);
			TreeList.RaiseProcessDragNode(e);
			if(e.Cancel)
				return false;
			if(!e.Handled) {
				if(String.IsNullOrEmpty(TreeList.ParentFieldName))
					throw new InvalidOperationException("Missing ParentFieldName");
				this.isNodeMoving = true;
				try {
					StartEdit(key);
					SetEditingValue(TreeList.ParentFieldName, parentKey);
					CommitEdit();
				} finally {
					this.isNodeMoving = false;
				}
			}
			if(!String.IsNullOrEmpty(parentKey))
				ExpandedKeys.Add(parentKey);
			this.focusedNodeKey = key;
			return true;
		}
		public void StartEditNewNode(string parentKey) {
			CancelEdit();
			this.newNodeParentKey = parentKey;
			if(!String.IsNullOrEmpty(parentKey))
				ExpandedKeys.Add(parentKey);	
			ASPxDataInitNewRowEventArgs e = new ASPxDataInitNewRowEventArgs();
			TreeList.RaiseInitNewNode(e);
			foreach(DictionaryEntry entry in e.NewValues)
				SetEditingValue(entry.Key.ToString(), entry.Value);			
		}
		public void EnsureNewNodeVisibility() {
			if(!PagingEnabled || !IsNewNodeEditing) return;
			TreeListNodeIterator iterator = TreeList.CreateNodeIterator();
			int index = 0;
			while(true) {
				TreeListNode node = GetNextVisibleNode(iterator);
				if(node == null) return;
				if(node.Key == TreeListRenderHelper.NewNodeKey)
					break;
				index++;
			}
			int newNodePageIndex = index / PageSize;
			if(newNodePageIndex != PageIndex)
				ForcePageIndex(newNodePageIndex);
		}
		public virtual bool DoNodeValidation() {
			if(!IsEditing)
				return false;
			if(this.isNodeMoving)
				return true;
			TreeListNodeValidationEventArgs e = new TreeListNodeValidationEventArgs(IsNewNodeEditing);
			if(IsNewNodeEditing)
				InitializeInsertingDictionaries(e.NewValues, TreeList.GetDataSourceView());
			else
				InitializeUpdatingDictionaries(e.Keys, e.OldValues, e.NewValues, TreeList.GetDataSourceView());
			ApplyEditingValues(e.NewValues);
			TreeList.RaiseNodeValidating(e);
			this.editingNodeError = !String.IsNullOrEmpty(e.NodeError) ? e.NodeError : DefaultEditingNodeError;
			SetColumnErrors(e.Errors);
			return !e.HasErrors;
		}
		public string GetEditingError(string fieldName) {
			if(ColumnErrors.ContainsKey(fieldName))
				return ColumnErrors[fieldName];
			return String.Empty;
		}
		protected void ResetEditing() {
			this.editingKey = null;
			this.newNodeParentKey = null;
			EditingValues.Clear();
			UnlinkUnboundFakeNode();
		}
		protected virtual void SetColumnErrors(Dictionary<string, string> errors) {
			ColumnErrors.Clear();
			if(errors.Count > 0) {
				foreach(KeyValuePair<string, string> pair in errors)
					ColumnErrors.Add(pair.Key, pair.Value);
			}
		}
		public void AddFakeNewNode() {
			AddFakeNewNode(true);
		}
		public void AddFakeNewNode(bool replace) {
			if(!IsNewNodeEditing || TreeList.SettingsEditing.IsPopupEditForm)
				return;
			if(replace) {
				UnlinkUnboundFakeNode();
			} else if(FindFakeNewNode() != null) {
				return;
			}
			TreeListNode parent = GetNodeByKey(NewNodeParentKey);
			if(parent != null) {
				parent.AppendChild(CreateFakeNode());
				if(!TreeList.IsVirtualMode())
					TreeList.DoSortSubtree(parent);
			}
		}
		TreeListNode CreateFakeNode() {
			TreeListNewNode node = new TreeListNewNode(this);
			foreach(string name in FieldTypes.Keys)
				node.SetValue(name, null);
			foreach(TreeListColumn column in TreeList.Columns) {
				TreeListDataColumn dc = column as TreeListDataColumn;
				if(dc != null)
					node.SetValue(dc.FieldName, null);
			}
			return node;
		}
		void UnlinkUnboundFakeNode() {
			if(TreeList.HasDataSource() || TreeList.IsVirtualMode())
				return;
			UnlinkFakeNode();
		}
		void UnlinkFakeNode() {
			TreeListNewNode fakeNode = FindFakeNewNode();
			if(fakeNode != null)
				fakeNode.ParentNode.ChildNodes.Remove(fakeNode);
		}
		TreeListNewNode FindFakeNewNode() {
			if(!TreeList.NodesCreated)
				return null;
			TreeListNodeIterator iterator = TreeList.CreateNodeIterator();
			while(true) {
				TreeListNode node = iterator.GetNext();
				if(node == null) break;
				TreeListNewNode fakeNode = node as TreeListNewNode;
				if(fakeNode != null)
					return fakeNode;
			}
			return null;
		}
		public void RefreshFakeNewNode() {
			if(!TreeList.NodesCreated && !this.rowsCreated)
				return; 
			if(TreeList.NodesCreated) {
				UnlinkFakeNode();
				AddFakeNewNode();
			}
			ResetVisibleData();
		}
		public TreeListRowInfo GetPopupEditFormRowInfo() {
			if(!IsEditing)
				return null;
			if(IsNewNodeEditing)
				return TreeListRowInfo.CreateFromNode(CreateFakeNode());
			foreach(TreeListRowInfo row in Rows) {
				if(row.NodeKey == EditingKey)
					return row;
			}
			if(TreeList.IsVirtualMode())
				return null;
			TreeListNode node = GetNodeByKey(EditingKey);
			if(node != null)
				return TreeListRowInfo.CreateFromNode(node);
			return null;
		}
		protected virtual void DoUpdateNodeData() {
			if(!IsEditing) return;
			DataSourceView view = TreeList.GetDataSourceView();
			ASPxDataUpdatingEventArgs e = new ASPxDataUpdatingEventArgs();
			InitializeUpdatingDictionaries(e.Keys, e.OldValues, e.NewValues, view);
			ApplyEditingValues(e.NewValues);
			TreeList.RaiseNodeUpdating(e);
			if(e.Cancel)
				return;
			if(view == null) {
				ResetEditing();
			} else {
				this.updatingArgs = e;
				view.Update(e.Keys, e.NewValues, e.OldValues, DataSourceUpdateCallback);
			}
		}
		protected virtual void DoInsertNodeData() {
			if(!IsNewNodeEditing) return;
			DataSourceView view = TreeList.GetDataSourceView();
			ASPxDataInsertingEventArgs e = new ASPxDataInsertingEventArgs();
			InitializeInsertingDictionaries(e.NewValues, view);
			ApplyEditingValues(e.NewValues);
			TreeList.RaiseNodeInserting(e);
			if(e.Cancel)
				return;
			if(view == null) {
				ResetEditing();
			} else {
				this.insertingArgs = e;
				view.Insert(e.NewValues, DataSourceInsertCallback);
			}
		}
		void InitializeUpdatingDictionaries(OrderedDictionary keys, OrderedDictionary oldValues, OrderedDictionary values, DataSourceView view) {			
			keys.Add(TreeList.KeyFieldName, ConvertKeyValue(EditingKey));			
			ParameterCollection collection = null;
			if(view != null)
				collection = GetDataSourceParameters(view, GetDataSourceParametersMode.Update);			
			TreeListNode node = null;
			if(!String.IsNullOrEmpty(EditingKey))
				node = GetNodeByKey(EditingKey);
			foreach(string fieldName in GetFilteredFieldNames(collection)) {
				object value = node != null ? node.GetValue(fieldName) : null;
				if(value == DBNull.Value)
					value = null;
				values[fieldName] = value;
				oldValues[fieldName] = value;
			}
		}
		void InitializeInsertingDictionaries(OrderedDictionary values, DataSourceView view) {
			ParameterCollection collection = null;
			if(view != null)
				collection = GetDataSourceParameters(view, GetDataSourceParametersMode.Insert);
			foreach(string fieldName in GetFilteredFieldNames(collection))
				values[fieldName] = null;
			if(!String.IsNullOrEmpty(NewNodeParentKey) && !String.IsNullOrEmpty(TreeList.ParentFieldName))
				values[TreeList.ParentFieldName] = NewNodeParentKey;
		}
		IEnumerable<string> GetFilteredFieldNames(ParameterCollection parameters) {
			List<string> result = new List<string>();
			if(parameters != null && parameters.Count > 0) {
				foreach(Parameter param in parameters)
					result.Add(param.Name);
			} else {
				foreach(string name in EditingValues.Keys)
					result.Add(name);
			}
			return result;
		}
		void ApplyEditingValues(OrderedDictionary values) {
			foreach(string fieldName in EditingValues.Keys)				
				values[fieldName] = EditingValues[fieldName];
			Dictionary<string, object> templateValues = TreeList.GetTemplateEditValues();
			if(templateValues == null)
				return;
			foreach(string fieldName in templateValues.Keys)
				values[fieldName] = ConvertEditingValue(templateValues[fieldName], fieldName);
		}
		bool DeleteNodeCore(DataSourceView view, TreeListNode node) {
			if(node == null)
				throw new ArgumentNullException();
			ASPxDataDeletingEventArgs e = new ASPxDataDeletingEventArgs();
			ParameterCollection deleteParams = null;
			List<string> names = new List<string>();
			if(view != null)
				deleteParams = GetDataSourceParameters(view, GetDataSourceParametersMode.Delete);
			if(deleteParams != null && deleteParams.Count > 0) {
				foreach(Parameter param in deleteParams)
					names.Add(param.Name);
			} else {
				foreach(TreeListColumn column in TreeList.Columns) {
			   		TreeListDataColumn dataColumn = column as TreeListDataColumn;
					if(dataColumn == null) continue;
					names.Add(dataColumn.FieldName);
				}
			}
			foreach(string name in names) {
				if(string.IsNullOrEmpty(name))
					continue;
				e.Values[name] = node.GetValue(name);
			}
			e.Keys.Add(TreeList.KeyFieldName, ConvertKeyValue(node.Key));			
			TreeList.RaiseNodeDeleting(e);
			if(e.Cancel)
				return false;
			this.deletingArgs = e;
			node.Selected = false;
			node.Expanded = false;
			if(view != null)
				view.Delete(e.Keys, e.Values, DataSourceDeleteCallback);
			return true;
		}
		enum GetDataSourceParametersMode { Insert, Update, Delete }
		ParameterCollection GetDataSourceParameters(DataSourceView view, GetDataSourceParametersMode mode) {
			SqlDataSourceView sqlView = view as SqlDataSourceView;
			if(sqlView != null) {
				switch(mode) {
					case GetDataSourceParametersMode.Insert:
						return sqlView.InsertParameters;
					case GetDataSourceParametersMode.Update:
						return sqlView.UpdateParameters;
					case GetDataSourceParametersMode.Delete:
						return sqlView.DeleteParameters;
					default:
						throw new NotImplementedException();
				}				
			}
			ObjectDataSourceView objView = view as ObjectDataSourceView;
			if(objView != null) {
				switch(mode) {
					case GetDataSourceParametersMode.Insert:
						return objView.InsertParameters;
					case GetDataSourceParametersMode.Update:
						return objView.UpdateParameters;
					case GetDataSourceParametersMode.Delete:
						return objView.DeleteParameters;
					default:
						throw new NotImplementedException();
				}				
			}
			return null;
		}
		object ConvertKeyValue(string key) {
			return ConvertEditingValue(key, TreeList.KeyFieldName);
		}
		object ConvertEditingValue(object value, string fieldName) {
			ASPxParseValueEventArgs e = new ASPxParseValueEventArgs(fieldName, value);
			TreeList.RaiseParseValue(e);
			value = e.Value;
			if(value == null || value == DBNull.Value)
				return null;
			if(!FieldTypes.ContainsKey(fieldName))
				return value;
			var fieldType = FieldTypes[fieldName];
			Type type = ReflectionUtils.StripNullableType(fieldType);
			Type valueType = value.GetType();
			if(valueType == typeof(string) && DataUtils.IsFloatType(type))
				value = DataUtils.FixFloatingPoint(value.ToString(), System.Globalization.CultureInfo.InvariantCulture);
			TypeConverter converter = TypeDescriptor.GetConverter(fieldType);
			if(converter != null && converter.CanConvertFrom(valueType)) {
				try {
					return converter.ConvertFrom(null, System.Globalization.CultureInfo.InvariantCulture, value);
				} catch { 
				}
			}
			return Convert.ChangeType(value, type, System.Globalization.CultureInfo.InvariantCulture);
		}
		bool DataSourceUpdateCallback(int affectedRecords, Exception exception) {
			ASPxDataUpdatedEventArgs e = new ASPxDataUpdatedEventArgs(affectedRecords, exception, UpdatingArgs);
			TreeList.RaiseNodeUpdated(e);
			this.updatingArgs = null;
			if(exception != null && !e.ExceptionHandled)
				return false;
			ResetEditing();
			TreeList.RaiseEditingOperationCompleted(TreeListEditingOperation.Update);
			return true;
		}
		bool DataSourceDeleteCallback(int affectedRecords, Exception exception) {
			ASPxDataDeletedEventArgs e = new ASPxDataDeletedEventArgs(affectedRecords, exception, DeletingArgs);
			TreeList.RaiseNodeDeleted(e);
			this.deletingArgs = null;
			bool success = e.Exception == null || e.ExceptionHandled;
			if(success)
				TreeList.RaiseEditingOperationCompleted(TreeListEditingOperation.Delete);
			return success;
		}
		bool DataSourceInsertCallback(int affectedRecords, Exception exception) {
			ASPxDataInsertedEventArgs e = new ASPxDataInsertedEventArgs(affectedRecords, exception, InsertingArgs.NewValues);
			TreeList.RaiseNodeInserted(e);
			this.insertingArgs = null;
			if(exception != null && !e.ExceptionHandled)
				return false;
			ResetEditing();
			TreeList.RaiseEditingOperationCompleted(TreeListEditingOperation.Insert);
			return true;
		}
		#endregion
		public void ResetVisibleData() {			
			this.rowsCreated = false;
			this.summaryCalculated = false;
			ResetUsedFieldNames();
			if(!TreeList.PreRendered)
				(TreeList as IWebControlObject).LayoutChanged();
		}
	}
	public class TreeListRowInfo : IValueProvider {
		IWebTreeListData treeListData;
		string nodeKey;
		TreeListNodeDataItem dataItem;
		List<TreeListRowIndentType> indents;
		bool hasButton;
		bool allowSelect;
		public static TreeListRowInfo CreateFromNode(TreeListNode node) {
			bool hasButton = node.HasChildren;
			if(!hasButton) {
				TreeListVirtualNode vNode = node as TreeListVirtualNode;
				if(vNode != null)
					hasButton = !vNode.Expanded && !vNode.IsLeaf;
			}
			return new TreeListRowInfo(node.TreeListData, node.Key, node.DataItemInternal, hasButton, node.AllowSelect);
		}
		public TreeListRowInfo(IWebTreeListData treeListData, string nodeKey, TreeListNodeDataItem dataItem, bool hasButton, bool allowSelect) {
			this.treeListData = treeListData;
			this.nodeKey = nodeKey;
			this.dataItem = dataItem;
			this.hasButton = hasButton;
			this.indents = new List<TreeListRowIndentType>();
			this.allowSelect = allowSelect;
		}
		public IWebTreeListData TreeListData { get { return treeListData; } }
		public string NodeKey { get { return nodeKey; } }
		public TreeListNodeDataItem DataItem { get { return dataItem; } }
		public List<TreeListRowIndentType> Indents { get { return indents; } }
		public bool HasButton { get { return hasButton; } }
		public virtual bool Selected {
			get {
				return CheckState == CheckState.Checked;
			}
		}
		public virtual CheckState CheckState {
			get {
				if(TreeListData == null) return CheckState.Unchecked;
				return TreeListData.GetNodeCheckState(NodeKey);
			}
		}
		public virtual bool Expanded {
			get {
				if(TreeListData == null) return false;
				return TreeListData.IsNodeExpanded(NodeKey);
			}
		}
		public virtual bool Focused {
			get {
				if(TreeListData == null) return false;
				return NodeKey == TreeListData.FocusedNodeKey;
			}
		}
		public bool AllowSelect { get { return allowSelect; } }
		public bool IsEditing { get { return TreeListData.EditingKey == NodeKey; } }
		public object GetEditingValue(string fieldName) {
			return TreeListData.GetEditingValue(fieldName, GetValue(fieldName));
		}
		public virtual object GetValue(string fieldName) {
			if(DataItem == null)
				return null;
			return DataItem.GetValue(fieldName);			
		}
		object IValueProvider.GetValue(string fieldName) {
			return GetValue(fieldName);
		}		
	}
	public class TreeListRootRowInfo : TreeListRowInfo {
		internal const int RowIndex = Int32.MinValue;
		public TreeListRootRowInfo(IWebTreeListData treeListData)
			: base(treeListData, TreeListRenderHelper.RootNodeKey, null, true, true) {
		}
		public override object GetValue(string fieldName) {
			return null;
		}		
		public override bool Expanded { get { return true; } }
		public override bool Focused { get { return false; } }
	}
	public class TreeListClientLayoutHelper {
		const int UnsavedPageIndex = -16;
		ASPxTreeList treeList;
		public TreeListClientLayoutHelper(ASPxTreeList treeList) {
			this.treeList = treeList;
		}
		protected ASPxTreeList TreeList { get { return treeList; } }
		protected TreeListSettingsCookies Settings { get { return TreeList.SettingsCookies; } }
		#region Save
		public byte[] SaveState() {
			using(MemoryStream stream = new MemoryStream()) {
				using(TypedBinaryWriter writer = new TypedBinaryWriter(stream)) {
					writer.WriteObject(Settings.Version);
					SaveColumnsVisibility(writer);
					SaveColumnsWidth(writer);
					SaveColumnsSorting(writer);
					SaveNodeStates(writer);
					SavePageIndex(writer);
				}
				return stream.ToArray();
			}
		}
		void SaveColumnsVisibility(TypedBinaryWriter writer) {
			if(!Settings.StoreColumnsVisiblePosition) {
				writer.WriteObject(0);
				return;
			}
			writer.WriteObject(TreeList.Columns.Count);
			foreach(TreeListColumn column in TreeList.Columns) {
				writer.WriteObject(column.Index);
				writer.WriteObject(column.Visible);
				writer.WriteObject(column.VisibleIndex);
			}
		}
		void SaveColumnsWidth(TypedBinaryWriter writer) {
			if(!Settings.StoreColumnsWidth) {
				writer.WriteObject(0);
				return;
			}
			writer.WriteObject(TreeList.Columns.Count);
			foreach(TreeListColumn column in TreeList.Columns) {
				writer.WriteObject(column.Index);
				writer.WriteObject(column.Width.ToString());
			}
		}
		void SaveColumnsSorting(TypedBinaryWriter writer) {
			if(!Settings.StoreSorting) {
				writer.WriteObject(0);
				return;
			}
			writer.WriteObject(TreeList.SortedColumns.Count);
			foreach(TreeListDataColumn column in TreeList.SortedColumns) {
				writer.WriteObject(column.Index);
				writer.WriteObject(column.SortIndex);
				writer.WriteObject((int)column.SortOrder);
			}
		}
		void SaveNodeStates(TypedBinaryWriter writer) {
			TreeList.TreeDataHelper.SaveNodeStates(writer, Settings.StoreExpandedNodes, Settings.StoreSelection);
		}
		void SavePageIndex(TypedBinaryWriter writer) {
			writer.WriteObject(Settings.StorePaging ? TreeList.PageIndex : UnsavedPageIndex);
		}
		#endregion
		#region Restore
		public bool RestoreState(byte[] state) {
			try {
				using(MemoryStream stream = new MemoryStream(state)) {
					stream.Position = 0;
					using(TypedBinaryReader reader = new TypedBinaryReader(stream)) {
						string savedVersion = reader.ReadObject<string>();
						if(savedVersion != Settings.Version)
							return false;
						RestoreColumnsVisibility(reader);
						RestoreColumnsWidth(reader);
						RestoreColumnsSorting(reader);
						RestoreNodeStates(reader);
						RestorePageIndex(reader);
					}
					if(TreeList.IsVirtualMode())
						TreeList.RefreshVirtualTree();
					return true;
				}
			} catch {
				return false;
			}
		}
		void RestoreColumnsVisibility(TypedBinaryReader reader) {
			int count = reader.ReadObject<int>();
			while(count-- > 0) {
				int index = reader.ReadObject<int>();
				bool visible = reader.ReadObject<bool>();
				int visibleIndex = reader.ReadObject<int>();
				if(index > TreeList.Columns.Count - 1)
					continue;
				TreeListColumn column = TreeList.Columns[index];
				column.SetColVisible(visible);
				column.SetColVisibleIndex(visibleIndex);
			}
		}
		void RestoreColumnsWidth(TypedBinaryReader reader) {
			int count = reader.ReadObject<int>();
			while(count-- > 0) {
				int index = reader.ReadObject<int>();
				Unit width = Unit.Parse(reader.ReadObject<string>());
				if(index > TreeList.Columns.Count - 1)
					continue;
				TreeListColumn column = TreeList.Columns[index];
				column.Width = width;
			}
		}
		void RestoreColumnsSorting(TypedBinaryReader reader) {
			TreeList.KillSortingInfo();
			int count = reader.ReadObject<int>();
			if(count < 1)
				return;
			while(count-- > 0) {
				int index = reader.ReadObject<int>();
				int sortIndex = reader.ReadObject<int>();
				ColumnSortOrder sortOrder = (ColumnSortOrder)reader.ReadObject<int>();
				if(index > TreeList.Columns.Count - 1)
					continue;
				TreeListDataColumn column = TreeList.Columns[index] as TreeListDataColumn;
				if(column == null)
					continue;
				column.SetSortIndex(sortIndex);
				column.SetSortOrder(sortOrder);
			}
			TreeList.DoSort();
		}
		void RestoreNodeStates(TypedBinaryReader reader) {
			TreeList.TreeDataHelper.LoadNodeStates(reader);
			TreeList.TreeDataHelper.ResetVisibleData();
		}
		void RestorePageIndex(TypedBinaryReader reader) {
			int value = reader.ReadObject<int>();
			if(value == UnsavedPageIndex)
				return;
			TreeList.EnsureNodesCreated(true);
			TreeList.TreeDataHelper.PageIndex = value;
		}
		#endregion
	}	
	public class StringSet : ICollection<string> {		
		Dictionary<string, object> innerStorage;
		public StringSet() 
			: this(0) {
		}
		public StringSet(int capacity) {
			this.innerStorage = new Dictionary<string, object>(capacity);
		}		
		public int Count { get { return innerStorage.Count; } }
		public void Add(string item) {
			innerStorage[item] = null;
		}
		public bool Remove(string item) {
			return innerStorage.Remove(item);			
		}
		public void Clear() {
			innerStorage.Clear();			
		}
		public bool Contains(string item) {
			return innerStorage.ContainsKey(item);
		}
		public List<string> ToList() {
			List<string> list = new List<string>(Count);
			foreach(string item in this)
				list.Add(item);
			return list;
		}
		#region ICollection<T>
		bool ICollection<string>.IsReadOnly { get { return false; } }
		void ICollection<string>.CopyTo(string[] array, int arrayIndex) {
			throw new NotImplementedException();
		}		
		#endregion
		#region IEnumerable<T> Members
		IEnumerator<string> IEnumerable<string>.GetEnumerator() {
			return innerStorage.Keys.GetEnumerator();
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return innerStorage.Keys.GetEnumerator();
		}
		#endregion
	}
}
namespace DevExpress.Web.ASPxTreeList.Internal {
	using System.ComponentModel;
	public abstract class TreeListCustomTypeDescriptor : ICustomTypeDescriptor {
		protected abstract PropertyDescriptorCollection GetProperties();
		#region ICustomTypeDescriptor Members
		string ICustomTypeDescriptor.GetClassName() { return null; }
		string ICustomTypeDescriptor.GetComponentName() { return null; }
		TypeConverter ICustomTypeDescriptor.GetConverter() { return null; }
		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() { return null; }
		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() { return null; }
		object ICustomTypeDescriptor.GetEditor(Type editorBaseType) { return null; }
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes) {
			return new EventDescriptorCollection(null);
		}
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents() {
			return new EventDescriptorCollection(null);
		}
		AttributeCollection ICustomTypeDescriptor.GetAttributes() {
			return new AttributeCollection(null);
		}
		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) {
			return this;
		}
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() {
			return GetProperties();
		}
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes) {
			return GetProperties();
		}
		#endregion   
	}
	public abstract class TreeListPropertyDescriptor : PropertyDescriptor {
		public TreeListPropertyDescriptor(string fieldName)
			: base(fieldName, null) {
		}
		public override bool CanResetValue(object component) { return false; }
		public override bool IsReadOnly { get { return false; } }
		public override Type PropertyType { get { return typeof(Object); } }
		public override void ResetValue(object component) {
		}
		public override bool ShouldSerializeValue(object component) {
			return false;
		}
	}
	public abstract class TreeListNodeDataItem : TreeListCustomTypeDescriptor {
		IWebTreeListData treeListData;
		public TreeListNodeDataItem(IWebTreeListData treeListData) {
			this.treeListData = treeListData;
		}
		protected IWebTreeListData TreeListData { get { return treeListData; } }
		public object GetValue(string fieldName) {
			TreeListData.RegisterUsedFieldName(fieldName);
			return GetValueCore(fieldName);
		}
		public void SetValue(string fieldName, object value) {
			TreeListData.RegisterUsedFieldName(fieldName);
			SetValueCore(fieldName, value);
		}
		public abstract object GetDataObject();
		protected abstract object GetValueCore(string fieldName);
		protected abstract void SetValueCore(string fieldName, object value);
	}
	public class TreeListBoundNodeDataItem : TreeListNodeDataItem {
		object item;
		PropertyDescriptorCollection originalProperties, properties;
		public TreeListBoundNodeDataItem(IWebTreeListData treeListData, object item)
			: base(treeListData) {
			this.item = item;
			this.originalProperties = null;
			this.properties = null;
		}
		protected object Item { get { return item; } }
		protected PropertyDescriptorCollection OriginalProperties { get { return originalProperties; } }
		protected PropertyDescriptorCollection Properties { get { return properties; } }
		public override object GetDataObject() {
			return Item;
		}
		protected override object GetValueCore(string fieldName) {
			return TreeListUtils.GetPropertyValue(Item, fieldName);
		}
		protected override void SetValueCore(string fieldName, object value) {
			ReflectionUtils.SetPropertyValue(Item, fieldName, value);
		}
		protected override PropertyDescriptorCollection GetProperties() {
			ICustomTypeDescriptor customDescriptor = Item as ICustomTypeDescriptor;
			PropertyDescriptorCollection collection = customDescriptor != null
				? customDescriptor.GetProperties()
				: TypeDescriptor.GetProperties(Item);
			if(Properties == null || collection != OriginalProperties) 
				CreatePropertyDescriptorCollection(collection);
			return Properties;
		}
		void CreatePropertyDescriptorCollection(PropertyDescriptorCollection collection) {
			int count = collection.Count;
			PropertyDescriptor[] props = new PropertyDescriptor[count];
			for(int i = 0; i < count; i++) {
				props[i] = new TreeListNodeDataItemPropertyDescriptor(collection[i].Name);
			}
			this.properties = new PropertyDescriptorCollection(props);
			this.originalProperties = collection;
		}
	}
	public class TreeListUnboundNodeDataItem : TreeListNodeDataItem {
		protected class InnerStorageType : Dictionary<string, object> { }
		InnerStorageType innerStorage;
		PropertyDescriptorCollection properties;
		public TreeListUnboundNodeDataItem(IWebTreeListData treeListData)
			: base(treeListData) {
			this.innerStorage = new InnerStorageType();
			this.properties = null;
		}
		protected InnerStorageType InnerStorage { get { return innerStorage; } }
		public override object GetDataObject() {
			return InnerStorage;
		}
		protected override object GetValueCore(string fieldName) {
			if(InnerStorage.ContainsKey(fieldName))
				return InnerStorage[fieldName];
			return null;
		}
		protected override void SetValueCore(string fieldName, object value) {
			if(!InnerStorage.ContainsKey(fieldName))
				this.properties = null;
			InnerStorage[fieldName] = value;
			if(value != null && value != DBNull.Value)
				TreeListData.RegisterFieldType(fieldName, value.GetType());
		}
		protected override PropertyDescriptorCollection GetProperties() {
			if(this.properties == null)
				CreatePropertyDescriptorCollection();
			return this.properties;
		}
		void CreatePropertyDescriptorCollection() {
			List<PropertyDescriptor> props = new List<PropertyDescriptor>(InnerStorage.Count);
			foreach(string fieldName in InnerStorage.Keys) {
				if(!String.IsNullOrEmpty(fieldName))
					props.Add(new TreeListNodeDataItemPropertyDescriptor(fieldName));
			}
			this.properties = new PropertyDescriptorCollection(props.ToArray());
		}
	}
	public class TreeListNodeDataItemPropertyDescriptor : TreeListPropertyDescriptor {
		public TreeListNodeDataItemPropertyDescriptor(string fieldName)
			: base(fieldName) {
		}
		public override Type ComponentType { get { return typeof(TreeListNodeDataItem); } }
		public override object GetValue(object component) {
			return (component as TreeListNodeDataItem).GetValue(Name);
		}
		public override void SetValue(object component, object value) {
			(component as TreeListNodeDataItem).SetValue(Name, value);
		}
	}
	public class TreeListTemplateDataItem : TreeListCustomTypeDescriptor {
		TreeListRowInfo row;
		PropertyDescriptorCollection cachedProps;
		public TreeListTemplateDataItem(TreeListRowInfo row) {
			this.row = row;
		}
		public TreeListRowInfo Row { get { return row; } }
		protected override PropertyDescriptorCollection GetProperties() {
			if(this.cachedProps == null)
				this.cachedProps = CreateProperties();
			return this.cachedProps;
		}
		PropertyDescriptorCollection CreateProperties() {
			List<PropertyDescriptor> list = new List<PropertyDescriptor>();
			foreach(PropertyDescriptor prop in TypeDescriptor.GetProperties(Row.DataItem))
				list.Add(new TreeListTemplateDataItemPropertyDescriptor(prop.Name));
			return new PropertyDescriptorCollection(list.ToArray());
		}
	}
	public class TreeListTemplateDataItemPropertyDescriptor : TreeListPropertyDescriptor {
		public TreeListTemplateDataItemPropertyDescriptor(string fieldName)
			: base(fieldName) {
		}
		public override Type ComponentType { get { return typeof(TreeListTemplateDataItem); } }
		public override object GetValue(object component) {
			TreeListRowInfo row = (component as TreeListTemplateDataItem).Row;
			if(row.IsEditing)
				return row.GetEditingValue(Name);
			return row.GetValue(Name);
		}
		public override void SetValue(object component, object value) {			
		}
	}   
}
