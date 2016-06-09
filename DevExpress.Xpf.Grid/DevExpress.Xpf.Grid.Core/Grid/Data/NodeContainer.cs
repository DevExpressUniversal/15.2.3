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
using DevExpress.Xpf.Grid.Native;
using System.Text;
using System.Collections;
namespace DevExpress.Xpf.Grid {
	public class NodeContainerPrintInfo {
		public bool IsGroupRowsContainer { get; set; }
	}
	public abstract class NodeContainer {
		internal static readonly IEnumerator<RowNode> EmptyEnumerator = ((IList<RowNode>)new RowNode[0]).GetEnumerator();
		int startScrollIndex;
		IEnumerator<RowNode> rowNodeEnumerator;
		IEnumerator<RowNode> RowNodeEnumerator {
			get { 
				if(rowNodeEnumerator == null)
					rowNodeEnumerator = GetRowDataEnumerator();
				return rowNodeEnumerator;
			} 
		}
		RowNode currentItem;
		RowNode CurrentItem { 
			get {
				if(currentItem == null)
					GetNextItem();
				return currentItem;
			} 
		}
		public IList<RowNode> Items { get; private set; }
		internal int StartScrollIndex { get { return startScrollIndex; } }
		internal bool Initialized { get { return RowNodeEnumerator != null; } }
		public virtual int ItemCount {
			get {
				int result = 0;
				if(Items != null) {
					foreach(var item in Items) {
						result += item.ItemsCount;
					}
				}
				return result;
			}
		}
		internal bool IsEnumeratorFinished { get { return CurrentItem == null; } }
		protected internal virtual bool IsEnumeratorValid { get { return true; } }
		public NodeContainer() {
			Items = new List<RowNode>();
		}
		internal void ReGenerateItemsCore(int startScrollIndex, int itemsCount) {
			this.startScrollIndex = startScrollIndex;
			OnDataChangedCore();
			GenerateItems(itemsCount);
		}
		internal virtual void ReGenerateExpandItems(int startScrollIndex, int itemsCount) {
			ReGenerateItemsCore(startScrollIndex, itemsCount);
		}
		internal bool IsFinished {
			get {
				if(!Initialized)
					return false;
				foreach(RowNode item in Items) {
					if(!item.IsFinished)
						return false;
				}
				return IsEnumeratorFinished;
			}
		}
		void GetNextItem() {
			if(RowNodeEnumerator.MoveNext())
				currentItem = RowNodeEnumerator.Current;
			else
				currentItem = null;
		}
		protected internal virtual void OnDataChangedCore() {
			Items = new List<RowNode>();
			rowNodeEnumerator = null;
			currentItem = null;
		}
		internal int oldVisibleRowCount;
		internal virtual int GenerateItems(int count) {
			if(!Initialized)
				return 0;
			int generatedCount = 0;
			foreach(RowNode item in Items) {
				generatedCount += item.GenerateItems(count - generatedCount);
			}
			while(generatedCount < count) {
				if(IsEnumeratorFinished)
					break;
				RowNode current = CurrentItem;
				GetNextItem();
				Items.Add(current);
				generatedCount += current.ItemsCount;
				generatedCount += current.GenerateItems(count - generatedCount);
			}
			OnItemsGenerated();
			return generatedCount;
		}
		protected virtual void OnItemsGenerated() {
		}
		protected abstract IEnumerator<RowNode> GetRowDataEnumerator();
		internal virtual RowNode GetNodeToScroll() {
			return Items.Count > 0 ? Items[0].GetNodeToScroll() : null;
		}
#if DEBUGTEST
		public void Print() {
			Print(0);
			System.Diagnostics.Debug.WriteLine("");
		}
		public void Print(int level) {
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < level; i++) {
				sb.Append("\t");
			}
			sb.Append(string.Format("Type={0}, Items.Count={1}, StartScrollIndex={2}, IsFinished={3}, ItemCount={4}, ", GetType().Name, Items.Count, StartScrollIndex, IsFinished, ItemCount));
			sb.Append(GetContainerSpecificString());
			sb.Append(string.Format("HashCode={0}, ", GetHashCode()));
			System.Diagnostics.Debug.WriteLine(sb.ToString());
			for(int i = 0; i < Items.Count; i++) {
				Items[i].Print(level + 1);
			}
		}
		protected virtual string GetContainerSpecificString() {
			return string.Empty;
		}
#endif
	}
	public class DataNodeContainer : NodeContainer {
		internal int DetailStartScrollIndex { get; set; }
		protected internal readonly DataTreeBuilder treeBuilder;
		readonly DataRowNode parentNode;
		internal int parentVisibleIndex;
		protected DataIteratorBase DataIterator { get { return treeBuilder.View.DataIterator; } }
		internal bool IsGroupRowsContainer { get { return DataIterator.IsGroupRowsContainer(this); } }
		internal int TotalVisibleCount { get { return treeBuilder.VisibleCount  + treeBuilder.View.CalcGroupSummaryVisibleRowCount() ; } }
		internal bool EnumerateOnlySelectedRows { get; set; }
		public int GroupLevel { get; private set; }
		public NodeContainerPrintInfo PrintInfo { get; set; }
		public DataNodeContainer(DataTreeBuilder treeBuilder, int level, DataRowNode parentNode) 
			: base() {
			GroupLevel = level;
			this.treeBuilder = treeBuilder;
			this.parentNode = parentNode;
		}
		protected internal override void OnDataChangedCore() {
			this.parentVisibleIndex = DataIterator.GetRowParentIndex(this, StartScrollIndex, GroupLevel);
			base.OnDataChangedCore();
		}
		protected override IEnumerator<RowNode> GetRowDataEnumerator() {
			if(parentNode != null && treeBuilder.View.DataProviderBase.GetChildRowCount(parentNode.ControllerValues.RowHandle.Value) == 0) {
				return EmptyEnumerator;
			}
			return GetRowDataEnumeratorCore();
		}
		protected internal override bool IsEnumeratorValid { get { return lastVisibleCount == TotalVisibleCount; } }
		int lastVisibleCount;
		IEnumerator<RowNode> GetRowDataEnumeratorCore() {
			lastVisibleCount = TotalVisibleCount;
			for(int i = StartScrollIndex; i < lastVisibleCount; i++) {
				bool shouldBreak = false;
				RowNode node = DataIterator.GetRowNodeForCurrentLevel(this, i, (i == StartScrollIndex) ? DetailStartScrollIndex : 0, ref shouldBreak);
				if(node != null && IsNodeSelected(node)) {
					yield return node;
				}
				if(shouldBreak)
					break;
			}
		}
		List<int> selectedRows;
		internal List<int> SelectedRows {
			get {
				if(selectedRows == null)
					selectedRows = new List<int>(treeBuilder.View.DataControl.DataProviderBase.Selection.GetSelectedRows());
				return selectedRows;
			}
		}
		bool IsNodeSelected(RowNode node) {
			if(!EnumerateOnlySelectedRows)
				return true;
			DataRowNode dataRowNode = node as DataRowNode;
			if(dataRowNode == null)
				return false;
			int rowHandle = dataRowNode.RowHandle.Value;
			return SelectedRows.Contains(rowHandle);
		}
		GridContainerRowsLocation GetRowsLocation() {
			return GetRowsLocation(GetHasTop(), GetHasBottom());
		}
		internal RowPosition GetRowPosition(RowNode node) {
			GridContainerRowsLocation rowsLocation = GetRowsLocation();
			if(Items.Count == 0)
				return RowPosition.Top;
			bool isFirst = Items[0] == node;
			bool isLast = Items[Items.Count - 1] == node;
			if(rowsLocation == GridContainerRowsLocation.TopOnly)
				return isFirst ? RowPosition.Top : RowPosition.Middle;
			if(rowsLocation == GridContainerRowsLocation.BottomOnly)
				return isLast ? RowPosition.Bottom : RowPosition.Middle;
			if(rowsLocation == GridContainerRowsLocation.TopAndBottom) {
				if(isFirst && isLast)
					return RowPosition.Single;
				if(isFirst)
					return RowPosition.Top;
				if(isLast)
					return RowPosition.Bottom;
				return RowPosition.Middle;
			}
			return RowPosition.Middle;
		}
		GridContainerRowsLocation GetRowsLocation(bool hasTop, bool hasBottom) {
			if(hasTop && hasBottom)
				return GridContainerRowsLocation.TopAndBottom;
			if(hasTop)
				return GridContainerRowsLocation.TopOnly;
			if(hasBottom)
				return GridContainerRowsLocation.BottomOnly;
			return GridContainerRowsLocation.Middle;
		}
		bool GetHasTop() {
			return DataIterator.GetHasTop(this);
		}
		bool GetHasBottom() {
			return DataIterator.GetHasBottom(this);
		}
		protected override void OnItemsGenerated() {
			treeBuilder.SetRowStateDirty();
		}
		public int CurrentLevelItemCount {
			get {
				if(!IsGroupRowsContainer)
					return Items.Count;
				int result = 0;
				if(Items != null) {
					foreach(RowNode item in Items) {
						DataRowNode dataItem = item as DataRowNode;
						if(dataItem != null)
							result += dataItem.CurrentLevelItemCount;
					}
				}
				return result;
			}
		}
	}
	public class DetailNodeContainer : DataNodeContainer {
		public DetailNodeContainer(DataTreeBuilder treeBuilder, int level)
			: base(treeBuilder, level, null) {
		}
		internal bool IsScrolling { get; set; }
		internal override void ReGenerateExpandItems(int commonStartScrollIndex, int itemsCount) {
			ReGenerateItems(commonStartScrollIndex, itemsCount, false);
		}
		internal void ReGenerateItems(int commonStartScrollIndex, int itemsCount, bool checkShouldRegenerateItems = true) {
			int startScrollIndex = 0;
			int detailStartScrollIndex = 0;
			MasterRowScrollInfo masterRowScrollInfo = treeBuilder.View.DataControl.MasterDetailProvider.CalcMasterRowScrollInfo(commonStartScrollIndex);
			if(masterRowScrollInfo != null) {
				startScrollIndex = masterRowScrollInfo.StartScrollIndex;
				detailStartScrollIndex = masterRowScrollInfo.DetailStartScrollIndex;
			}	
			if(checkShouldRegenerateItems) {
				if(!ShouldRegenerateItems(detailStartScrollIndex, startScrollIndex))
					return;
				IsScrolling = true;
			}
			this.DetailStartScrollIndex = detailStartScrollIndex;
			ReGenerateItemsCore(startScrollIndex, itemsCount);
		}
		bool ShouldRegenerateItems(int detailStartScrollIndex, int startScrollIndex) {
			return (DetailStartScrollIndex != detailStartScrollIndex) ||
				(StartScrollIndex != startScrollIndex) ||
				(parentVisibleIndex != DataIterator.GetRowParentIndex(this, startScrollIndex, GroupLevel));
		}
#if DEBUGTEST  && !SL
		protected override string GetContainerSpecificString() {
			return string.Format("DetailStartScrollIndex={0}, ", DetailStartScrollIndex);
		}
#endif
		internal void ReGenerateMasterRootItems() {
			treeBuilder.View.DataControl.InvalidateDetailScrollInfoCache();
			treeBuilder.View.DataProviderBase.InvalidateVisibleIndicesCache();
			treeBuilder.MasterRootNodeContainer.ReGenerateItemsCore();
		}
		internal void OnMasterRooDataChanged() {
			if(treeBuilder.View.DataControl == null)
				return;
			treeBuilder.View.DataControl.InvalidateDetailScrollInfoCache();
			treeBuilder.View.DataProviderBase.InvalidateVisibleIndicesCache();
			treeBuilder.MasterRootNodeContainer.OnDataChanged();
		}
	}
	public class MasterNodeContainer : DetailNodeContainer {
		int maxItemCount;
		public MasterNodeContainer(DataTreeBuilder treeBuilder, int level)
			: base(treeBuilder, level) {
		}
		public override int ItemCount { get { return Math.Min(base.ItemCount, maxItemCount); } }
		protected override void OnItemsGenerated() {
			base.OnItemsGenerated();
			treeBuilder.SynchronizeMasterNode();
			oldVisibleRowCount = treeBuilder.View.DataControl.VisibleRowCount;
			foreach(RowNode node in Items) {
				UpdateLineLevel(node);
			}
			treeBuilder.View.UpdateCellMergingPanels();
		}
		void UpdateLineLevel(RowNode node) {
			RowDataBase data = node.GetRowData();
			if(data != null) {
				data.UpdateLineLevel();
			}
			if(node.NodesContainer != null) {
				foreach(RowNode childNode in node.NodesContainer.Items) {
					UpdateLineLevel(childNode);
				}
			}
		}
		protected internal override void OnDataChangedCore() {
			maxItemCount = 0;
			base.OnDataChangedCore();
		}
		internal override int GenerateItems(int count) {
			maxItemCount += count;
			return base.GenerateItems(count);
		}
		internal void OnDataChanged() {
			OnDataChangedCore();
			OnItemsGenerated();
		}
		internal void ReGenerateItemsCore() {
			if(treeBuilder.View.DataControl == null)
				return;
			treeBuilder.View.layoutUpdatedLocker.DoLockedAction(() => {
				ReGenerateItemsCore(StartScrollIndex, ItemCount);
			});
			treeBuilder.ForceLayout();
		}
	}
}
