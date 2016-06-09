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

using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DevExpress.Office;
#if SL
using System.Windows;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	#region ConditionalFormattingCollection
	public class ConditionalFormattingCollection : UndoableCollection<ConditionalFormatting>, IComparer<ConditionalFormatting> {
		#region Fields
		readonly Worksheet worksheet;
		ConditionalFormattingCachedRTree tree;
		ConditionalFormattingCachedRTree formattingTree;
		ConditionalFormattingCachedRTree paintingTree;
		static ReadOnlyCollection<ConditionalFormatting> empty = new ReadOnlyCollection<ConditionalFormatting>(
			new List<ConditionalFormatting>());
		#endregion
		public ConditionalFormattingCollection(Worksheet worksheet)
			: base(worksheet.Workbook) {
			this.worksheet = worksheet;
		}
		#region Properties
		Worksheet Worksheet { get { return worksheet; } }
		ConditionalFormattingCachedRTree InnerTree { get { return tree; } }
		ConditionalFormattingCachedRTree Tree {
			get {
				if (tree == null)
					tree = CreateTree();
				return tree;
			}
		}
		ConditionalFormattingCachedRTree InnerFormattingTree { get { return formattingTree; } }
		ConditionalFormattingCachedRTree ColorScaleTree {
			get {
				if (formattingTree == null)
					formattingTree = CreateFormattingTree();
				return formattingTree;
			}
		}
		ConditionalFormattingCachedRTree InnerPaintingTree { get { return paintingTree; } }
		ConditionalFormattingCachedRTree PaintingTree {
			get {
				if (paintingTree == null)
					paintingTree = CreatePaintingTree();
				return paintingTree;
			}
		}
		#endregion
		public override int AddCore(ConditionalFormatting item) {
			int index = base.AddCore(item);
			InsertToTree(item);
			return index;
		}
		public override void AddRangeCore(IEnumerable<ConditionalFormatting> collection) {
			base.AddRangeCore(collection);
			ClearTrees();
		}
		protected  override void InsertCore(int index, ConditionalFormatting item) {
			base.InsertCore(index, item);
			InsertToTree(item);
		}
		public override void RemoveAtCore(int index) {
			RemoveFromTree(InnerList[index]);
			base.RemoveAtCore(index);
		}
		public override void ClearCore() {
			base.ClearCore();
			ClearTrees();
		}
		internal void RemoveInsertNoApiInvalidation(int oldIndex, int newIndex) {
			ValueChecker.CheckValue(oldIndex, 0, Count, "oldIndex");
			ValueChecker.CheckValue(newIndex, 0, Count, "newIndex");
			if (oldIndex == newIndex)
				return;
			RemoveInsertNoApiInvalidationConditionalFormattingHistoryItem historyItem = new RemoveInsertNoApiInvalidationConditionalFormattingHistoryItem(Worksheet, oldIndex, newIndex);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		internal void RemoveInsertNoApiInvalidationCore(int oldIndex, int newIndex) {
			RaiseRemoveInsertNoApiInvalidation(oldIndex, newIndex);
			ConditionalFormatting item = InnerList[oldIndex];
			InnerList.RemoveAt(oldIndex);
			InnerList.Insert(newIndex, item);
			ClearTrees();
		}
		public override bool Contains(ConditionalFormatting item) {
			return IndexOf(item) >= 0;
		}
		public override int IndexOf(ConditionalFormatting item) {
			int index = InnerList.BinarySearch(item);
			if (index >= 0 && InnerList[index].Equals(item))
				return index;
			return -1;
		}
		public int AddWithPrioritiesCorrection(ConditionalFormatting item) {
			if (!object.ReferenceEquals(item.Sheet, worksheet))
				return -1;
			DocumentModel book = Worksheet.Workbook;
			book.BeginUpdate();
			try {
				int itemPriority = item.Priority;
				if (InnerList.Count == 0 || InnerList[InnerList.Count - 1].Priority < itemPriority) {
					Add(item);
					return Count - 1;
				}
				int indexInCollection = InnerList.BinarySearch(item);
				if (indexInCollection < 0)
					indexInCollection = ~indexInCollection;
				else
					if (object.ReferenceEquals(this[indexInCollection], item))
						return indexInCollection;
				Insert(indexInCollection, item);
				IncrementPriorities(indexInCollection + 1, Count - 1);
				ClearTrees();
				return indexInCollection;
			}
			finally {
				Touch(item.CellRangeInternalNoHistory);
				book.EndUpdate();
			}
		}
		internal void ChangePriorityWithPrioritiesCorrection(ConditionalFormatting conditionalFormatting, int newPriorityValue) {
			DocumentModel book = Worksheet.Workbook;
			book.BeginUpdate();
			try {
				int index = IndexOf(conditionalFormatting);
				int oldValue = conditionalFormatting.Priority;
				conditionalFormatting.SetPriority(newPriorityValue);
				if (index < 0)
					return;
				int newIndex;
				if (newPriorityValue < oldValue) {
					if (index == 0)
						return;
					newIndex = InnerList.BinarySearch(0, index, conditionalFormatting, this);
					if (newIndex < 0)
						newIndex = ~newIndex;
					else
						IncrementPriorities(newIndex, index - 1);
				}
				else {
					int lastItemIndex = InnerList.Count - 1;
					if (index == lastItemIndex)
						return;
					newIndex = InnerList.BinarySearch(index + 1, lastItemIndex - index, conditionalFormatting, this);
					if (newIndex < 0)
						newIndex = ~newIndex;
					else
						IncrementPriorities(newIndex, Count - 1);
					--newIndex;
				}
				RemoveInsertNoApiInvalidation(index, newIndex);
			}
			finally {
				book.EndUpdate();
			}
		}
		void IncrementPriorities(int startIndex, int endIndex) {
			ConditionalFormatting prev = InnerList[startIndex];
			ConditionalFormatting current;
			for (int i = startIndex; i <= endIndex; ++i) {
				current = this[i];
				int currentPriority = current.Priority;
				if (currentPriority > prev.Priority)
					return;
				current.SetPriority(currentPriority + 1);
				prev = current;
			}
		}
		public int Compare(ConditionalFormatting x, ConditionalFormatting y) {
			return x.CompareTo(y);
		}
		public void SubtractCellRange(CellRangeBase range) {
			this.SubtractCellRange(range, null);
		}
		public void SubtractCellRange(CellRangeBase range, System.Predicate<ConditionalFormatting> filter) {
			bool renewTrees = false;
			for (int i = 0; i < InnerList.Count;++i) {
				ConditionalFormatting item = InnerList[i];
				if (item.IsValid && item.CellRange.Intersects(range)) {
					if (filter != null)
						if (!filter(item))
							continue;
					CellRangeBase newRange = item.CellRange.ExcludeRange(range);
					if (newRange == null) {
						item.InvalidateOrMarkDeleted();
						Worksheet.RemoveConditionalFormattingAtWithHistoryAndNotification(i);
						--i;
					}
					else
						item.SetCellRange(newRange);
					renewTrees = true;
				}
			}
			if (renewTrees)
				ClearTrees();
		}
		#region Tree Utils
		ConditionalFormattingCachedRTree CreateTree() {
			ConditionalFormattingCachedRTree result = new ConditionalFormattingCachedRTree();
			int count = InnerList.Count;
			for (int i = 0; i < count; i++) {
				ConditionalFormatting item = InnerList[i];
				CellRangeBase range = item.CellRange;
				if (range != null)
					InsertToTree(result, item);
			}
			return result;
		}
		ConditionalFormattingCachedRTree CreateFormattingTree() {
			ConditionalFormattingCachedRTree result = new ConditionalFormattingCachedRTree();
			int count = InnerList.Count;
			for (int i = 0; i < count; i++) {
				ConditionalFormatting item = InnerList[i];
				if (item.Type == ConditionalFormattingType.ColorScale || item.Type == ConditionalFormattingType.Formula) {
					CellRangeBase range = item.CellRange;
					if (range != null)
						InsertToTree(result, item);
				}
			}
			return result;
		}
		ConditionalFormattingCachedRTree CreatePaintingTree() {
			ConditionalFormattingCachedRTree result = new ConditionalFormattingCachedRTree();
			int count = InnerList.Count;
			for (int i = 0; i < count; i++) {
				ConditionalFormatting item = InnerList[i];
				if (item.Type == ConditionalFormattingType.DataBar || item.Type == ConditionalFormattingType.IconSet)
					if(item.CellRangeInternalNoHistory != null)
						InsertToTree(result, item);
			}
			return result;
		}
		void InsertToTree(ConditionalFormatting item) {
			if (item.CellRange == null)
				return;
			if (InnerTree != null)
				InsertToTree(InnerTree, item);
			switch (item.Type) {
				case ConditionalFormattingType.ColorScale:
				case ConditionalFormattingType.Formula:
					if (InnerFormattingTree != null)
						InsertToTree(InnerFormattingTree, item);
					break;
				case ConditionalFormattingType.DataBar:
				case ConditionalFormattingType.IconSet:
					if (InnerPaintingTree != null)
						InsertToTree(InnerPaintingTree, item);
					break;
			}
			Touch(item.CellRange);
		}
		void Touch(CellRangeBase cellRange) {
			if (cellRange == null)
				return;
			foreach (ICellBase cellInfo in cellRange.GetExistingCellsEnumerable()) {
				ICell cell = cellInfo as ICell;
				if (cell != null) {
					if (cell.ConditionalFormatAccumulator == null)
						cell.ConditionalFormatAccumulator = new ConditionalFormattingFormatAccumulator();
					cell.ConditionalFormatAccumulator.Reset(-1);
				}
			}
		}
		void InsertToTree(ConditionalFormattingCachedRTree tree, ConditionalFormatting item) {
			tree.Insert(item);
		}
		void RemoveFromTree(ConditionalFormatting item) {
			if (item.CellRangeInternalNoHistory == null)
				return;
			if (InnerTree != null)
				InnerTree.Remove(item);
			switch(item.Type) {
				case ConditionalFormattingType.ColorScale:
				case ConditionalFormattingType.Formula:
					if (InnerFormattingTree != null)
						InnerFormattingTree.Remove(item);
					break;
				case ConditionalFormattingType.DataBar:
				case ConditionalFormattingType.IconSet:
					if (InnerPaintingTree != null)
						InnerPaintingTree.Remove(item);
					break;
			}
		}
		void ReplaceInTree(ConditionalFormatting item, CellRangeBase previousRange) {
			if (previousRange != null)
				Touch(previousRange);
			ClearTrees();
		}
		void ClearTrees() {
			if (InnerTree != null)
				InnerTree.Clear();
			tree = null;
			if (InnerFormattingTree != null)
				InnerFormattingTree.Clear();
			formattingTree = null;
			if (InnerPaintingTree != null)
				InnerPaintingTree.Clear();
			paintingTree = null;
		}
		#endregion
		public List<ConditionalFormatting> Select(CellRangeBase cellRange, bool exact) {
			List<ConditionalFormatting> result = new List<ConditionalFormatting>();
			foreach (ConditionalFormatting item in this) {
				CellRangeBase itemRange = item.CellRange;
				bool match = exact ? itemRange.Equals(cellRange) : itemRange.Intersects(cellRange);
				if (match)
					result.Add(item);
			}
			return result;
		}
		public void OnRangeRemoving(RemoveRangeNotificationContext notificationContext) {
			for (int i = Count - 1; i >= 0; --i) {
				ConditionalFormatting formatting = this[i];
				formatting.OnRangeRemoving(notificationContext);
				if (formatting.CellRange == null)
					RemoveAt(i);
			}
		}
		public void OnRangeInserting(InsertRangeNotificationContext notificationContext) {
			for (int i = Count - 1; i >= 0; --i)
				this[i].OnRangeInserting(notificationContext);
		}
		public void NotifyItemCellRangeChanged(ConditionalFormatting item, CellRangeBase previousRange) {
			if (item == null)
				return;
			if (!Contains(item))
				return;
			ReplaceInTree(item, previousRange);
		}
		public IList<ConditionalFormatting> GetBackColorConditionalFormatting(ICell cell) {
			return ColorScaleTree.SearchAll(cell.ColumnIndex, cell.RowIndex);
		}
		public bool CheckCell(CellKey key) {
			if (this.Count <= 0)
				return false;
			ConditionalFormatting condFmt = Tree.Search(key.ColumnIndex, key.RowIndex);
			return condFmt != null;
		}
		public IList<ConditionalFormatting> GetConditionalFormattings(CellKey cellKey) {
			IList<ConditionalFormatting> result = Tree.SearchAll(cellKey.ColumnIndex, cellKey.RowIndex);
			return result != null ? result : empty;
		}
		public IList<ConditionalFormatting> GetPaintConditionalFormattings(CellKey cellKey) {
			IList<ConditionalFormatting> result = PaintingTree.SearchAll(cellKey.ColumnIndex, cellKey.RowIndex);
			return result != null ? result : empty;
		}
		public static void Visit(IEnumerable<ConditionalFormatting> list, IConditionalFormattingVisitor visitor) {
			if (list == null)
				return;
			foreach (ConditionalFormatting formatting in list)
				if (formatting.Visit(visitor) && formatting.StopIfTrue)
					break;
		}
		public IConditionalFormattingPainters GetPainters(ICell cell, int priority) {
			IConditionalFormattingPainters result = new ConditionalFormattingValuablePainters();
			IList<ConditionalFormatting> list = PaintingTree.SearchAll(cell.ColumnIndex, cell.RowIndex);
			if (list != null) {
				foreach (ConditionalFormatting item in list) {
					if (item.Priority >= priority)
						break;
					if ((item.Type == ConditionalFormattingType.DataBar) && (result.DataBar == null))
						result.DataBar = item as DataBarConditionalFormatting;
					if ((item.Type == ConditionalFormattingType.IconSet) && (result.IconSet == null))
						result.IconSet = item as IconSetConditionalFormatting;
				}
			}
			return result;
		}
		#region OnRemoveInsertNoApiInvalidation
		ConditionalFormattingRemoveInsertNoApiInvalidationEventHandler onRemoveInsertNoApiInvalidation;
		internal event ConditionalFormattingRemoveInsertNoApiInvalidationEventHandler OnRemoveInsertNoApiInvalidation { add { onRemoveInsertNoApiInvalidation += value; } remove { onRemoveInsertNoApiInvalidation -= value; } }
		void RaiseRemoveInsertNoApiInvalidation(int oldIndex, int newIndex) {
			if (onRemoveInsertNoApiInvalidation != null)
				onRemoveInsertNoApiInvalidation(this, new ConditionalFormattingRemoveInsertNoApiInvalidationEventArgs(oldIndex, newIndex));
		}
		#endregion
	}
	#endregion
	public class ConditionalFormattingCachedRTree : CellRangeWithUnionCachedRTreeBase<ConditionalFormatting> {
		protected override CellRangeBase GetItemRange(ConditionalFormatting item) {
			return item.CellRangeInternalNoHistory;
		}
	}
	public interface IConditionalFormattingPainters {
		DataBarConditionalFormatting DataBar { get; set; }
		IconSetConditionalFormatting IconSet { get; set; }
		bool ShowValues { get; }
	}
	public class ConditionalFormattingValuablePainters : IConditionalFormattingPainters {
		DataBarConditionalFormatting dataBar;
		IconSetConditionalFormatting iconSet;
		public ConditionalFormattingValuablePainters() {
			dataBar = null;
			iconSet = null;
		}
		public DataBarConditionalFormatting DataBar { get { return dataBar; } set { dataBar = value; } }
		public IconSetConditionalFormatting IconSet { get { return iconSet; } set { iconSet = value; } }
		public bool ShowValues { get { return GetShowValue(); } }
		bool GetShowValue() {
			if (DataBar == null)
				return IconSet == null || IconSet.ShowValue;
			else
				if (IconSet == null)
					return DataBar.ShowValue;
			return (IconSet.Priority < DataBar.Priority) ? IconSet.ShowValue : DataBar.ShowValue;
		}
	}
	public class ConditionalFormattingDummyPainters : IConditionalFormattingPainters {
		static readonly ConditionalFormattingDummyPainters instance = new ConditionalFormattingDummyPainters();
		#region IConditionalFormattingPainters Members
		public DataBarConditionalFormatting DataBar { get { return null; } set { ; } }
		public IconSetConditionalFormatting IconSet { get { return null; } set { ; } }
		public bool ShowValues { get { return true; } }
		#endregion
		public static ConditionalFormattingDummyPainters GetInstance() {
			return instance;
		}
	}
}
