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
using System.Diagnostics;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.Office.History;
using DevExpress.XtraSpreadsheet.Utils.Trees;
namespace DevExpress.XtraSpreadsheet.Model {
	public partial class MergedCellCollection { 
		#region Static
		static bool CanMergedCellModifyByRange(CellRangeBase modifyingRange, CellRange mergedCell, RemoveCellMode mode) {
			if (modifyingRange.RangeType == CellRangeType.IntervalRange) {
				return true; 
			}
			if (modifyingRange.RangeType == CellRangeType.UnionRange) {
				foreach (CellRangeBase currentRange in (modifyingRange as CellUnion).InnerCellRanges)
					if (!CanMergedCellModifyByRange(currentRange, mergedCell, mode))
						return false;
				return true;
			}
			VariantValue intersection = modifyingRange.IntersectionWith(mergedCell);
			if (intersection.IsCellRange) {
				CellRange intersectedRange = intersection.CellRangeValue as CellRange;
				if (intersectedRange != null) {
					if (mode == RemoveCellMode.ShiftCellsLeft)
						return intersectedRange.Height == mergedCell.Height;
					if (mode == RemoveCellMode.ShiftCellsUp)
						return intersectedRange.Width == mergedCell.Width;
				}
			}
			return !modifyingRange.Intersects(mergedCell) || modifyingRange.Includes(mergedCell);
		}
		static bool NeedUnmergedCellOnRangeRemove(CellRangeBase removedRange, CellRange mergedCell, RemoveCellMode mode) {
			CellRange unmergedRange = new CellRange(mergedCell.Worksheet,
				mode == RemoveCellMode.ShiftCellsLeft ? new CellPosition(0, mergedCell.TopLeft.Row) : new CellPosition(mergedCell.TopLeft.Column, 0),
				mergedCell.BottomRight);
			if (unmergedRange.Intersects(removedRange)) {
				VariantValue intersection = removedRange.IntersectionWith(unmergedRange);
				if (!intersection.IsCellRange)
					return false;
				return NeedUnmergedCellOnRangeRemoveCore(intersection.CellRangeValue, unmergedRange, mode);
			}
			return false;
		}
		static bool NeedUnmergedCellOnRangeRemoveCore(CellRangeBase intersectionRange, CellRange unmergedRange, RemoveCellMode mode) {
			if (intersectionRange.RangeType == CellRangeType.UnionRange) {
				CellUnion union = (CellUnion)intersectionRange;
				foreach (CellRangeBase innerRange in union.InnerCellRanges)
					if (NeedUnmergedCellOnRangeRemoveCore(innerRange, unmergedRange, mode))
						return true;
				return false;
			}
			if (mode == RemoveCellMode.ShiftCellsLeft)
				return intersectionRange.Height != unmergedRange.Height;
			else
				return intersectionRange.Width != unmergedRange.Width;
		}
		static bool NeedUnmergedCellOnRangeInsert(CellRangeBase insertedRange, CellRange mergedCell, InsertCellMode mode) {
			CellRange unmergedRange = new CellRange(mergedCell.Worksheet,
				mode == InsertCellMode.ShiftCellsRight ? new CellPosition(0, mergedCell.TopLeft.Row) : new CellPosition(mergedCell.TopLeft.Column, 0),
				mode == InsertCellMode.ShiftCellsRight ? new CellPosition(mergedCell.TopLeft.Column - 1, mergedCell.BottomRight.Row) : new CellPosition(mergedCell.BottomRight.Column, mergedCell.TopLeft.Row - 1));
			if (unmergedRange.Intersects(insertedRange)) {
				VariantValue intersection = insertedRange.IntersectionWith(unmergedRange);
				if (!intersection.IsCellRange)
					return false;
				return NeedUnmergedCellOnRangeInsertCore(intersection.CellRangeValue, unmergedRange, mode);
			}
			return false;
		}
		static bool NeedUnmergedCellOnRangeInsertCore(CellRangeBase intersectionRange, CellRange unmergedRange, InsertCellMode mode) {
			if (intersectionRange.RangeType == CellRangeType.UnionRange) {
				CellUnion union = (CellUnion)intersectionRange;
				foreach (CellRangeBase innerRange in union.InnerCellRanges)
					if (NeedUnmergedCellOnRangeInsertCore(innerRange, unmergedRange, mode))
						return true;
				return false;
			}
			if (mode == InsertCellMode.ShiftCellsRight)
				return intersectionRange.Height != unmergedRange.Height;
			else
				return intersectionRange.Width != unmergedRange.Width;
		}
		#endregion
		#region Fields
		readonly Worksheet sheet;
		readonly List<CellRange> innerList;
		CellRangesCachedRTree tree;
		int maxCellHeight;
		List<int> rangeHeights;
		#endregion
		public MergedCellCollection(Worksheet sheet) {
			this.sheet = sheet;
			this.innerList = new List<CellRange>();
			rangeHeights = new List<int>();
			maxCellHeight = 0;
		}
		#region Properties
		public Worksheet Worksheet { get { return this.sheet; } }
		public int Count { get { return this.innerList.Count; } }
		public CellRange this[int index] { get { return this.innerList[index]; } }
		internal int MaxCellHeight { get { return maxCellHeight; } }
		CellRangesCachedRTree Tree {
			get {
				if (tree == null)
					tree = CreateTree();
				return tree;
			}
		}
		CellRangesCachedRTree InnerTree { get { return tree; } }
		public CellRange First { get { return innerList.Count > 0 ? innerList[0] : null; } }
		#endregion
		CellRangesCachedRTree CreateTree() {
			CellRangesCachedRTree result = new CellRangesCachedRTree();
			int count = innerList.Count;
			for (int i = 0; i < count; i++)
				InsertToTree(result, innerList[i]);
			return result;
		}
		public CellRange GetMergedCellRange(ICellBase cell) {
			return GetMergedCellRange(cell.ColumnIndex, cell.RowIndex);
		}
		public CellRange GetMergedCellRange(CellKey key) {
			return GetMergedCellRange(key.ColumnIndex, key.RowIndex);
		}
		public CellRange GetMergedCellRange(CellPosition pos) {
			return GetMergedCellRange(pos.Column, pos.Row);
		}
		public CellRange GetMergedCellRange(int columnIndex, int rowIndex) {
			if (Count <= 0)
				return null;
			return Tree.Search(columnIndex, rowIndex);
		}
		public List<CellRange> GetMergedCellRangesIntersectsButNotCoversByRange(CellRangeBase range) {
			List<CellRange> intersections = GetMergedCellRangesIntersectsRange(range);
			List<CellRange> result = new List<CellRange>();
			foreach (CellRange intersection in intersections) {
				if (!range.Includes(intersection))
					result.Add(intersection);
			}
			return result;
		}
		public bool HasMergedCellRangesIntersectsButNotCoversByRange(CellRange range) {
			List<CellRange> mergedRanges = Tree.Search(range);
			foreach (CellRange intersection in mergedRanges) {
				if (!range.Includes(intersection))
					return true;
			}
			return false;
		}
		public List<CellRange> GetMergedCellRangesIntersectsRange(CellRangeBase range) {
			List<CellRange> result = new List<CellRange>();
			GetMergedCellRangesIntersectsRangeBase(result, range);
			return result;
		}
		protected internal void GetMergedCellRangesIntersectsRangeBase(List<CellRange> where, CellRangeBase range) {
			if (range.RangeType == CellRangeType.UnionRange) {
				CellUnion union = (CellUnion)range;
				foreach (CellRangeBase innerRange in union.InnerCellRanges) {
					GetMergedCellRangesIntersectsRangeBase(where, innerRange);
				}
			}
			else{
				List<CellRange> mergedRanges = Tree.Search((CellRange)range);
				if (where.Count == 0)
					where.AddRange(mergedRanges);
				else {
					foreach (CellRange mergedRange in mergedRanges) {
						bool shouldAdd = true;
						foreach (CellRange existingRange in where) {
							if (existingRange.EqualsPosition(mergedRange)) {
								shouldAdd = false;
								break;
							}
						}
						if (shouldAdd)
							where.Add(mergedRange);
					}
				}
			}
		}
		public bool RangeContainsMergedCellRangesOfDifferentSize(CellRange range) {
			List<Model.CellRange> mergedCells = GetMergedCellRangesIntersectsRange(range);
			int count = mergedCells.Count;
			if (count == 0)
				return false;
			CellRange firstMergedRange = mergedCells[0];
			int firstWidth = firstMergedRange.Width;
			int firstHeight = firstMergedRange.Height;
			CellRangeBase entireMergedCellsRange = firstMergedRange;
			for (int i = 1; i < count; i++) {
				CellRange currentMergedRange = mergedCells[i];
				entireMergedCellsRange = entireMergedCellsRange.MergeWithRange(currentMergedRange);
				if (firstWidth != currentMergedRange.Width || firstHeight != currentMergedRange.Height)
					return true;
			}
			if (entireMergedCellsRange.RangeType == CellRangeType.UnionRange)
				return true;
			return !entireMergedCellsRange.Includes(range);
		}
		public bool IsHostMergedCell(ICellBase cell, CellRange mergedRange) {
			return cell.ColumnIndex == mergedRange.TopLeft.Column && cell.RowIndex == mergedRange.TopLeft.Row;
		}
		public CellRange FindMergedCell(int columnIndex, int rowIndex) {
			return Tree.Search(columnIndex, rowIndex);
		}
		#region Add
		public void Add(CellRange range) {
			Debug.Assert(range != null);
			CellMergeHistoryItem historyItem = new CellMergeHistoryItem(sheet, range);
			Worksheet.Workbook.History.Add(historyItem);
			historyItem.Execute();
		}
		public void AddCore(CellRange range) {
			innerList.Add(range);
			InsertToTree(range);
			AddRangeHeight(range.Height);
		}
		void AddRangeHeight(int rangeHeight) {
			if (!rangeHeights.Contains(rangeHeight))
				rangeHeights.Add(rangeHeight);
			maxCellHeight = Math.Max(maxCellHeight, rangeHeight);
		}
		#endregion
		#region Insert
		public void Insert(int index, CellRange range) {
			innerList.Insert(index, range);
			InsertToTree(range);
			AddRangeHeight(range.Height);
		}
		void InsertToTree(CellRange item) {
			if (InnerTree != null)
				InsertToTree(InnerTree, item);
		}
		void InsertToTree(CellRangesCachedRTree tree, CellRange item) {
			tree.Insert(item);
		}
		#endregion
		#region Remove
		protected internal void Remove(CellRange range) {
			Debug.Assert(range != null);
			CellUnMergeHistoryItem historyItem = new CellUnMergeHistoryItem(sheet, range);
			Worksheet.Workbook.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void RemoveCore(int index, CellRange range) {
			RemoveFromTree(range);
			innerList.RemoveAt(index);
			RemoveRangeHeight();
		}
		protected internal int IndexOf(CellRange range) {
			return innerList.IndexOf(range);
		}
		protected internal void RemoveAtCore(int index) {
			RemoveFromTree(innerList[index]);
			innerList.RemoveAt(index);
		}
		void RemoveFromTree(CellRange item) {
			if (InnerTree == null)
				return;
			InnerTree.Remove(item);
		}
		void RemoveRangeHeight() {
			maxCellHeight = 0;
			foreach (int rangeHeight in rangeHeights)
				maxCellHeight = Math.Max(maxCellHeight, rangeHeight);
		}
		#endregion
		#region Change
		public void ChangeItemRange(int index, CellRange range) {
			CellRange oldRange = innerList[index];
			if(range.Equals(oldRange))
				return;
			DocumentHistory history = Worksheet.Workbook.History;
			MergedCellRangeChangedHistoryItem historyItem = new MergedCellRangeChangedHistoryItem(Worksheet, index, range, oldRange);
			history.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void ChangeItemRangeCore(int index, CellRange range) {
			RemoveFromTree(innerList[index]);
			innerList[index] = range;
			InsertToTree(range);
		}
		#endregion
		public virtual void Clear() {
			this.Worksheet.Workbook.InternalAPI.OnBeforeMergedCellsCleared(this.Worksheet);
			InternalClear();
		}
		public IModelErrorInfo CheckCanRangeInsert(CellRangeBase cellRange, InsertCellMode mode, InsertCellsFormatMode formatMode) {
			bool isIntervalRange = cellRange.RangeType == CellRangeType.IntervalRange;
			for (int i = 0; i < innerList.Count; i++) {
				CellRange item = innerList[i];
				if (!isIntervalRange && !CanMergedCellModifyByRange(cellRange, item, mode == InsertCellMode.ShiftCellsRight ? RemoveCellMode.ShiftCellsLeft: RemoveCellMode.ShiftCellsUp))
					return new ModelErrorInfo(ModelErrorType.MergedCellCanNotBeChanged);
				if (NeedUnmergedCellOnRangeInsert(cellRange, item, mode)) {
					if(formatMode == InsertCellsFormatMode.ClearFormat && isIntervalRange)
						return new ModelErrorInfo(ModelErrorType.MergedCellCanNotBeChanged);
					return new ClarificationErrorInfo(ModelErrorType.UnmergeMergedCellsClarification);
				}
			}
			return null;
		}
		public IModelErrorInfo CheckCanRangeRemove(CellRangeBase cellRange, RemoveCellMode mode) {
			if (mode == RemoveCellMode.Default || cellRange.RangeType == CellRangeType.IntervalRange)
				return null;
			for (int i = 0; i < innerList.Count; i++) {
				CellRange item = innerList[i];
				if (!CanMergedCellModifyByRange(cellRange, item, mode))
					return new ModelErrorInfo(ModelErrorType.MergedCellCanNotBeChanged);
				if (NeedUnmergedCellOnRangeRemove(cellRange, item, mode))
					return new ClarificationErrorInfo(ModelErrorType.UnmergeMergedCellsClarification);
			}
			return null;
		}
		public bool NeedUnmergedCellOnRangeRemove(CellRange range, RemoveCellMode mode) {
			if(range.RangeType == CellRangeType.IntervalRange || mode == RemoveCellMode.Default)
				return false;
			for(int i = 0; i < innerList.Count; i++) {
				CellRange item = innerList[i];
				if(NeedUnmergedCellOnRangeRemove(range, item, mode))
					return true;
			}
			return false;
		}
		public void OnRangeInserting(InsertRangeNotificationContext notificationContext) {
			InsertCellMode mode = notificationContext.Mode;
			for(int i = innerList.Count - 1; i >= 0; i--) {
				CellRange item = innerList[i];
				if (NeedUnmergedCellOnRangeInsert(notificationContext.Range, item, mode)) {
					UnMergeSingleRangeWithoutChecks(item, i);
					continue;
				}
				ChangeItemRange(i, notificationContext.Visitor.ProcessCellRange(item.Clone() as CellRange) as CellRange);
			}
		}
		public void OnRangeRemoving(RemoveRangeNotificationContext notificationContext) {
			RemoveCellMode mode = notificationContext.Mode;
			if (mode == RemoveCellMode.Default || mode == RemoveCellMode.NoShiftOrRangeToPasteCutRange)
				return;
			for (int i = innerList.Count - 1; i >= 0; i--) {
				CellRange item = innerList[i];
				if (notificationContext.Range.Includes(item) || NeedUnmergedCellOnRangeRemove(notificationContext.Range, item, mode)) {
					UnMergeSingleRangeWithoutChecks(item, i);
					continue;
				}
				ChangeItemRange(i, notificationContext.Visitor.ProcessCellRange(item.Clone() as CellRange) as CellRange);
				if(item == null) 
					throw new NullReferenceException();
			}
		}
		void UnMergeSingleRangeWithoutChecks(CellRange range, int index) {
			sheet.Workbook.InternalAPI.OnBeforeMergedCellsRemoved(sheet, range);
			CellUnMergeHistoryItem historyItem = new CellUnMergeHistoryItem(sheet, range, index);
			sheet.Workbook.History.Add(historyItem);
			historyItem.Execute();
		}
		public bool Contains(CellRange range) {
			int count = innerList.Count;
			for (int i = 0; i < count; i++) {
				if (range.Equals(innerList[i]))
					return true;
			}
			return false;
		}
		public void ForEach(Action<CellRange> action) {
			innerList.ForEach(action);
		}
		void InternalClear() {
			innerList.Clear();
			if (InnerTree != null)
				InnerTree.Clear();
			tree = null;
		}
		public IEnumerable<CellRange> GetEVERYMergedRangeSLOWEnumerable() {
			return innerList;
		}
		public ICell TryGetCell(CellRange mergedCell) {
			return sheet.TryGetCell(mergedCell.LeftColumnIndex, mergedCell.TopRowIndex);
		}
		public string GetMergedCellText(CellRange mergedCell) {
			if(this.Contains(mergedCell)){
				ICell cell = TryGetCell(mergedCell);
				if (cell != null)
					return cell.Text;
			}
			return string.Empty;
		}
		public CellPosition CorrectCellPositionToTopLeftInsideMergedRange(CellPosition cellPosition) {
			CellRange found = GetMergedCellRange(cellPosition);
			if (found != null)
				return found.TopLeft;
			return cellPosition;
		}
	}
}
