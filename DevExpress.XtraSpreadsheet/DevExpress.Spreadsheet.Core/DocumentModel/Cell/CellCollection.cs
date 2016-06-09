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
using DevExpress.Utils;
using System.Diagnostics;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.Office.History;
namespace DevExpress.XtraSpreadsheet.Model {
	#region CellCollectionBase (abstract class)
	public abstract class CellCollectionBase<T> : ICellCollectionGeneric<T> where T : class, ICellBase {
		#region Fields
		readonly ICellPosition owner;
		readonly List<T> innerList;
		#endregion
		protected CellCollectionBase(ICellPosition owner) {
			Guard.ArgumentNotNull(owner, "owner");
			this.owner = owner;
			this.innerList = new List<T>();
		}
		#region Properties
		public int Count { get { return InnerList.Count; } }
		public T this[int index] { get { return GetCell(index); } }
		public T First { get { return Count > 0 ? InnerList[0] : null; } }
		public T Last { get { return Count > 0 ? InnerList[Count - 1] : null; } }
		public virtual IList<T> InnerList { get { return innerList; } }
		protected internal ICellPosition Owner { get { return owner; } }
		#endregion
		public virtual T GetCell(int index) {
			if (index < 0)
				throw new IndexOutOfRangeException();
			int position = TryGetCellIndex(index);
			if (position < 0) {
				position = ~position;
				T cell = CreateNewCell(index);
				Insert(position, cell);
				return cell;
			}
			else
				return InnerList[position];
		}
		public virtual T GetCellAssumeSequential(int index) {
			if (index < 0)
				throw new IndexOutOfRangeException();
			if (Count <= 0)
				return AppendCell(index);
			T lastCell = Last;
			if (lastCell.ColumnIndex < index)
				return AppendCell(index);
			return GetCell(index);
		}
		T AppendCell(int index) {
			T cell = CreateNewCell(index);
			Insert(Count, cell);
			return cell;
		}
		public int TryGetCellIndex(int index) {
			return Algorithms.BinarySearch(InnerList, new CellComparable<T>(index));
		}
		public void RemoveAtColumnIndex(int index) {
			int position = TryGetCellIndex(index);
			if (position < 0)
				return;
			RemoveAt(position);
		}
		public void RemoveAtColumnIndexInternal(int index) {
			int position = TryGetCellIndex(index);
			if (position < 0)
				return;
			RemoveAtInternal(position);
		}
		public int Add(T item) {
			Guard.ArgumentNotNull(item, "Item");
			Insert(Count, item);
			return Count - 1;
		}
		public virtual void Insert(int index, T item) {
			InsertCore(index, item);
		}
		public virtual void InsertCore(int index, T item) {
			InnerList.Insert(index, item);
			AfterInsertCell(index, item);
		}
		public virtual void InsertInternal(int index, T item) {
			InnerList.Insert(index, item);
		}
		public virtual void RemoveAt(int index) {
			RemoveAtCore(index);
		}
		public void RemoveAtCore(int index) {
			BeforeRemoveCell(InnerList[index]);
			RemoveAtInternal(index);
		}
		protected internal void RemoveAtInternal(int index) {
			InnerList.RemoveAt(index);
		}
		public bool Contains(T item) {
			return InnerList.Contains(item);
		}
		public void ForEach(Action<T> action) {
			InnerList.ForEach(action);
		}
		public virtual T TryGetCell(int index) {
			int position = TryGetCellIndex(index);
			if (position < 0)
				return default(T);
			return InnerList[position];
		}
		public virtual T CreateNewCell(int index) {
			System.Diagnostics.Debug.Assert(owner != null);
			return CreateNewCellCore(owner.GetCellPosition(index));
		}
		public bool Contains(int index) {
			int position = TryGetCellIndex(index);
			return position >= 0;
		}
		public bool TryToInsertCell(T item) {
			int position = TryGetCellIndex(item.ColumnIndex);
			if (position < 0) {
				position = ~position;
				Insert(position, item);
				return true;
			}
			return false;
		}
		public bool TryToInsertCellInternal(T item) {
			int position = TryGetCellIndex(item.ColumnIndex);
			if (position < 0) {
				position = ~position;
				InsertInternal(position, item);
				return true;
			}
			return false;
		}
		public IEnumerable<T> GetExistingCells(int leftColumn, int rightColumn, bool reverseOrder) {
			if (!reverseOrder)
				return new Enumerable<T>(GetExistingCellsEnumerator(leftColumn, rightColumn));
			return new Enumerable<T>(GetExistingCellsEnumerator(leftColumn, rightColumn, reverseOrder));
		}
		public IEnumerable<T> GetExistingNonEmptyCells(int leftColumn, int rightColumn, bool reverseOrder) {
			return new Enumerable<T>(GetExistingNonEmptyCellsEnumerator(leftColumn, rightColumn, reverseOrder));
		}
		public IEnumerable<T> GetExistingVisibleCells(int leftColumn, int rightColumn, bool reverseOrder) {
			return new Enumerable<T>(GetExistingVisibleCellsEnumerator(leftColumn, rightColumn, reverseOrder));
		}
		public IEnumerable<T> GetExistingNonEmptyVisibleCells(int leftColumn, int rightColumn, bool reverseOrder) {
			return new Enumerable<T>(GetExistingNonEmptyVisibleCellsEnumerator(leftColumn, rightColumn, reverseOrder));
		}
		public IOrderedEnumerator<T> GetExistingCellsEnumerator(int leftColumn, int rightColumn, bool reverseOrder) {
			return new CellsEnumerator<T>(InnerList, leftColumn, rightColumn, reverseOrder);
		}
		public IEnumerator<T> GetExistingCellsEnumerator(int leftColumn, int rightColumn) {
			return new CellsEnumeratorFast<T>(InnerList, leftColumn, rightColumn);
		}
		public IOrderedEnumerator<T> GetExistingNonEmptyCellsEnumerator(int leftColumn, int rightColumn, bool reverseOrder) {
			return new NonEmptyCellsEnumerator<T>(InnerList, leftColumn, rightColumn, reverseOrder);
		}
		public IOrderedEnumerator<T> GetExistingVisibleCellsEnumerator(int leftColumn, int rightColumn, bool reverseOrder) {
			return new VisibleCellsEnumerator<T>(InnerList, leftColumn, rightColumn, reverseOrder);
		}
		public IEnumerator<T> GetExistingNonEmptyVisibleCellsEnumerator(int leftColumn, int rightColumn, bool reverseOrder) {
			return new VisibleNonEmptyCellsEnumerator<T>(InnerList, leftColumn, rightColumn, reverseOrder);
		}
		protected abstract void AfterInsertCell(int innerIndex, T cell);
		protected abstract void BeforeRemoveCell(T cell);
		public abstract T CreateNewCellCore(CellPosition position);
		public abstract void OffsetRowIndex(int offset);
		public abstract void OffsetColumnIndex(int offset);
		#region IEnumerable<T> Members
		IEnumerator<T> IEnumerable<T>.GetEnumerator() {
			return InnerList.GetEnumerator();
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return InnerList.GetEnumerator();
		}
		#endregion
		#region ICellCollectionBase Members
		IEnumerable ICellCollectionBase.GetExistingCells(int leftColumn, int rightColumn, bool reverseOrder) {
			return ((ICellCollectionGeneric<T>)this).GetExistingCells(leftColumn, rightColumn, reverseOrder);
		}
		IEnumerable ICellCollectionBase.GetExistingVisibleCells(int leftColumn, int rightColumn, bool reverseOrder) {
			return ((ICellCollectionGeneric<T>)this).GetExistingVisibleCells(leftColumn, rightColumn, reverseOrder);
		}
		IEnumerable ICellCollectionBase.GetExistingNonEmptyVisibleCells(int leftColumn, int rightColumn, bool reverseOrder) {
			return ((ICellCollectionGeneric<T>)this).GetExistingNonEmptyVisibleCells(leftColumn, rightColumn, reverseOrder);
		}
		IEnumerator ICellCollectionBase.GetExistingCellsEnumerator(int leftColumn, int rightColumn, bool reverseOrder) {
			return ((ICellCollectionGeneric<T>)this).GetExistingCellsEnumerator(leftColumn, rightColumn, reverseOrder);
		}
		IEnumerator ICellCollectionBase.GetExistingCellsEnumerator(int leftColumn, int rightColumn) {
			return ((ICellCollectionGeneric<T>)this).GetExistingCellsEnumerator(leftColumn, rightColumn);
		}
		#endregion
	}
	#endregion
	#region ICellCollectionBase
	public interface ICellCollectionBase : IEnumerable {
		IEnumerable GetExistingCells(int leftColumn, int rightColumn, bool reverseOrder);
		IEnumerable GetExistingVisibleCells(int leftColumn, int rightColumn, bool reverseOrder);
		IEnumerable GetExistingNonEmptyVisibleCells(int leftColumn, int rightColumn, bool reverseOrder);
		IEnumerator GetExistingCellsEnumerator(int leftColumn, int rightColumn, bool reverseOrder);
		IEnumerator GetExistingCellsEnumerator(int leftColumn, int rightColumn);
	}
	#endregion
	#region ICellCollectionGeneric
	public interface ICellCollectionGeneric<T> : ICellCollectionBase, IEnumerable<T> {
		T this[int index] { get; }
		int Count { get; }
		T First { get; }
		T Last { get; }
		int Add(T item);
		int TryGetCellIndex(int cachedColumnIndex);
		T GetCell(int index);
		T TryGetCell(int columnIndex);
		void Insert(int index, T item);
		void InsertCore(int position, T cell);
		void InsertInternal(int index, T item);
		void RemoveAtCore(int position);
		T CreateNewCell(int columnIndex);
		T CreateNewCellCore(CellPosition position);
		void ForEach(Action<T> action);
		bool Contains(T cell);
		bool Contains(int index);
		void OffsetRowIndex(int offset);
		void RemoveAtColumnIndex(int index);
		void RemoveAtColumnIndexInternal(int index);
		bool TryToInsertCell(T cell);
		bool TryToInsertCellInternal(T cell);
		new IEnumerable<T> GetExistingCells(int leftColumn, int rightColumn, bool reverseOrder);
		new IEnumerable<T> GetExistingVisibleCells(int leftColumn, int rightColumn, bool reverseOrder);
		new IEnumerable<T> GetExistingNonEmptyVisibleCells(int leftColumn, int rightColumn, bool reverseOrder);
		new IOrderedEnumerator<T> GetExistingCellsEnumerator(int leftColumn, int rightColumn, bool reverseOrder);
		new IEnumerator<T> GetExistingCellsEnumerator(int leftColumn, int rightColumn);
		IEnumerable<T> GetExistingNonEmptyCells(int leftColumn, int rightColumn, bool reverseOrder);
		IOrderedEnumerator<T> GetExistingNonEmptyCellsEnumerator(int leftColumn, int rightColumn, bool reverseOrder);
		IOrderedEnumerator<T> GetExistingVisibleCellsEnumerator(int leftColumn, int rightColumn, bool reverseOrder);
		IEnumerator<T> GetExistingNonEmptyVisibleCellsEnumerator(int leftColumn, int rightColumn, bool reverseOrder);
		IList<T> InnerList { get; }
	}
	#endregion
	#region ICellCollection
	public interface ICellCollection : ICellCollectionGeneric<ICell> {
		IEnumerable<ICell> GetAllCellsWithFakeCellsAsNullInRow(Row modelRow, int near, int far);
		IOrderedEnumerator<ICell> GetAllCellsProvideFakeActualBorderEnumerator(int leftColumn, int rightColumn, bool reverseOrder, int rowIndex, IActualBorderInfo baseActualBorder);
		ICell TryGetFirstCellWith(Func<ICell, bool> condition);
		ICell TryGetLastCellWith(Func<ICell, bool> condition, int firstColumnIndexWhereCellsNotCanBe, int lastColumnIndexWhereCellsNotCanBe);
		ICell GetCellAssumeSequential(int index);
	}
	#endregion
	#region CellCollection
	public class CellCollection : CellCollectionBase<ICell>, ICellCollection {
		public CellCollection(IRow owner)
			: base(owner) {
		}
		#region Properties
		protected internal new IRow Owner { get { return (IRow)base.Owner; } }
		#endregion
		public override void Insert(int index, ICell item) {
			DocumentHistory history = Owner.Sheet.Workbook.History;
			SpreadsheetCellInsertedHistoryItem historyItem = new SpreadsheetCellInsertedHistoryItem(Owner.Sheet.Workbook, Owner, item, index);
			history.Add(historyItem);
			historyItem.Execute();
		}
		public override void RemoveAt(int index) {
			DocumentHistory history = Owner.Sheet.Workbook.History;
			SpreadsheetCellRemovedHistoryItem historyItem = new SpreadsheetCellRemovedHistoryItem(Owner.Sheet.Workbook, Owner, index);
			history.Add(historyItem);
			historyItem.Execute();
		}
		protected override void AfterInsertCell(int innerIndex, ICell cell) {
			Owner.Sheet.Workbook.CalculationChain.OnAfterCellInsert(innerIndex, cell);
		}
		protected override void BeforeRemoveCell(ICell cell) {
			Worksheet sheet = Owner.Sheet;
			sheet.Workbook.RaiseInnerCellRemoving(sheet.Name, cell.Key);
			Owner.Sheet.Workbook.CalculationChain.OnBeforeCellRemove(cell);
		}
		public override ICell CreateNewCellCore(CellPosition position) {
			return Owner.Sheet.CreateCellCore(position);
		}
		public override void OffsetRowIndex(int offset) {
			int count = InnerList.Count;
			for (int i = 0; i < count; i++)
				InnerList[i].OffsetRowIndex(offset, false);
			Owner.Sheet.ResetCachedData();
		}
		public override void OffsetColumnIndex(int offset) {
			int count = InnerList.Count;
			for (int i = 0; i < count; i++)
				InnerList[i].OffsetColumnIndex(offset);
			Owner.Sheet.ResetCachedData();
		}
		public IEnumerable<ICell> GetAllCellsProvideFakeActualBorder(int leftColumn, int rightColumn, bool reverseOrder, int rowIndex, IActualBorderInfo baseActualBorder) {
			return new Enumerable<ICell>(GetAllCellsProvideFakeActualBorderEnumerator(leftColumn, rightColumn, reverseOrder, rowIndex, baseActualBorder));
		}
		public IOrderedEnumerator<ICell> GetAllCellsProvideFakeActualBorderEnumerator(int leftColumn, int rightColumn, bool reverseOrder, int rowIndex, IActualBorderInfo baseActualBorder) {
			return new CellsEnumeratorProviderFakeActualBorderForNonExistingCells(InnerList, leftColumn, rightColumn, reverseOrder, Owner.Sheet, rowIndex, baseActualBorder);
		}
		public IEnumerable<ICell> GetAllCellsWithFakeCellsAsNullInRow(Model.Row modelRow, int leftColumnIndex, int rightColumnIndex) {
			IOrderedEnumerator<ICell> existingCells = GetExistingCellsEnumerator(leftColumnIndex, rightColumnIndex, false);
			IOrderedEnumerator<ICell> continousCells = new Model.ContinuousCellsEnumeratorFakeCellsAsNull(InnerList, leftColumnIndex, rightColumnIndex, false, Owner.Sheet, modelRow.Index);
			JoinedOrderedEnumerator<Model.ICell> joinedEnumerator = new JoinedOrderedEnumerator<Model.ICell>(existingCells, continousCells);
			return new Enumerable<Model.ICell>(joinedEnumerator);
		}
		public ICell TryGetFirstCellWith(Func<ICell, bool> condition) {
			int count = InnerList.Count;
			for (int i = 0; i < count; ++i) {
				ICell cell = InnerList[i];
				if (condition(cell))
					return cell;
			}
			return null;
		}
		public ICell TryGetLastCellWith(Func<ICell, bool> condition, int firstColumnIndexWhereCellsNotCanBe, int lastColumnIndexWhereCellsNotCanBe) {
			int columnIndex = 0;
			for (int i = InnerList.Count - 1; i >= 0; --i) {
				ICell cell = InnerList[i];
				columnIndex = cell.ColumnIndex;
				if (columnIndex >= lastColumnIndexWhereCellsNotCanBe) 
					continue;
				if (columnIndex <= firstColumnIndexWhereCellsNotCanBe)
					break;
				if (condition(cell))
					return cell;
			}
			return null;
		}
	}
	#endregion
	#region CellComparable<T>
	internal class CellComparable<T> : IComparable<T> where T : ICellBase {
		readonly int columnIndex;
		public CellComparable(int columnIndex) {
			this.columnIndex = columnIndex;
		}
		public int CompareTo(T other) {
			return other.ColumnIndex - columnIndex;
		}
	}
	#endregion
	#region CellsEnumerator<T>
	public class CellsEnumerator<T> : SparseOrderedItemsRangeEnumerator<T> where T : class, ICellBase {
		public CellsEnumerator(IList<T> cells, int nearItemIndex, int farItemIndex, bool reverseOrder)
			: base(cells, nearItemIndex, farItemIndex, reverseOrder, null) {
		}
		protected internal override IComparable<T> CreateComparable(int itemIndex) {
			return new CellComparable<T>(itemIndex);
		}
		protected internal override int GetCurrentValueOrder(int itemIndex) {
			return GetItem(itemIndex).ColumnIndex;
		}
	}
	#endregion
	#region CellsEnumeratorFast
	public class CellsEnumeratorFast<T> : IEnumerator<T> where T : class, ICellBase {
		readonly IList<T> items;
		readonly int nearItemIndex;
		readonly int farItemIndex;
		int currentIndex;
		T currentItem;
		public CellsEnumeratorFast(IList<T> items, int nearItemIndex, int farItemIndex) {
			this.items = items;
			this.nearItemIndex = nearItemIndex;
			this.farItemIndex = farItemIndex;
			this.currentIndex = -1;
		}
		protected internal IComparable<T> CreateComparable(int itemIndex) {
			return new CellComparable<T>(itemIndex);
		}
		protected int CalculateActualFirstIndex() {
			int actualFirstIndex = Algorithms.BinarySearch(items, CreateComparable(nearItemIndex));
			if (actualFirstIndex < 0)
				actualFirstIndex = ~actualFirstIndex;
			return actualFirstIndex;
		}
		#region IEnumerator<T> Members
		public T Current { get { return currentItem; } }
		#endregion
		#region IDisposable Members
		public void Dispose() {
			currentIndex = -1;
			currentItem = default(T);
		}
		#endregion
		#region IEnumerator Members
		object IEnumerator.Current { get { return Current; } }
		public bool MoveNext() {
			if (currentIndex < 0)
				currentIndex = CalculateActualFirstIndex();
			else
				currentIndex++;
			if (currentIndex >= items.Count)
				return false;
			currentItem = items[currentIndex];
			return currentItem.ColumnIndex <= farItemIndex;
		}
		public void Reset() {
			currentIndex = -1;
			currentItem = default(T);
		}
		#endregion
	}
	#endregion
	#region ContinuousCellsEnumerator
	public class ContinuousCellsEnumerator : CellsEnumerator<ICell> {
		readonly Worksheet sheet;
		readonly int rowIndex;
		public ContinuousCellsEnumerator(IList<ICell> cells, int nearItemIndex, int farItemIndex, bool reverseOrder, Worksheet sheet, int rowIndex)
			: base(cells, nearItemIndex, farItemIndex, reverseOrder) {
			this.sheet = sheet;
			this.rowIndex = rowIndex;
		}
		protected Worksheet Sheet { get { return sheet; } }
		protected int RowIndex { get { return rowIndex; } }
		protected override int CalculateActualFirstIndex() {
			return NearItemIndex;
		}
		protected override int CalculateActualLastIndex() {
			return FarItemIndex;
		}
		protected override ICell GetItemCore(int index) {
			return new FakeCell(new CellPosition(index, rowIndex, PositionType.Absolute, PositionType.Absolute), sheet);
		}
		public override void OnObjectInserted(int insertedItemValueOrder) {
		}
		public override void OnObjectDeleted(int deletedItemValueOrder) {
		}
	}
	#endregion
	public class ContinuousCellsEnumeratorProvideRowBorder : ContinuousCellsEnumerator {
		readonly Row row;
		public ContinuousCellsEnumeratorProvideRowBorder(IList<ICell> cells, int nearItemIndex, int farItemIndex, bool reverseOrder, Worksheet sheet, int rowIndex)
			: base(cells, nearItemIndex, farItemIndex, reverseOrder, sheet, rowIndex) {
			this.row = Sheet.Rows.TryGetRow(RowIndex);
		}
		protected override ICell GetItemCore(int index) {
			return new FakeCellWithRowFormatting(index, row);
		}
	}
	public class FakeCellsInRowOnExistingColumnsWithFormattingEnumerator : IOrderedEnumerator<ICell>, IDisposable {
		readonly ColumnCollection columnCollection;
		readonly OrderedItemsRangeEnumerator<Column> columnEnumerator;
		readonly int nearColumnIndex;
		readonly int farColumnIndex;
		readonly int defaultFormatIndex;
		readonly int rowIndex;
		readonly bool fakeCellsAsNull;
		public FakeCellsInRowOnExistingColumnsWithFormattingEnumerator(
			Worksheet worksheet,
			int nearColumnIndex,
			int farColumnIndex,
			bool reversedOrder,
			int rowIndex,
			bool fakeCellsAsNull) {
			this.columnCollection = worksheet.Columns;
			this.nearColumnIndex = nearColumnIndex;
			this.farColumnIndex = farColumnIndex;
			this.defaultFormatIndex = worksheet.Workbook.StyleSheet.DefaultCellFormatIndex;
			this.rowIndex = rowIndex;
			this.fakeCellsAsNull = fakeCellsAsNull;
			this.columnEnumerator = columnCollection.GetExistingColumnsEnumerator(nearColumnIndex, farColumnIndex, reversedOrder) as OrderedItemsRangeEnumerator<Column>;
		}
		protected internal Column Column { get { return (columnEnumerator as IOrderedEnumerator<Column>).Current; } }
		Worksheet Worksheet { get { return columnCollection.Sheet; } }
		protected internal int MoveNextInnerIteractionCount { get; set; }
		#region IOrderedEnumerator<ICell> Members
		public int CurrentValueOrder { get { return CellColumnIndex; } }
		public bool IsReverseOrder { get { return columnEnumerator.IsReverseOrder; } }
		#endregion
		public int CellColumnIndex {
			get {
				if (!IsReverseOrder)
					return Column.StartIndex + indexOfCellInsideColumnRange;
				return Column.EndIndex - indexOfCellInsideColumnRange;
			}
		}
		#region IEnumerator<ICell> Members
		public ICell Current { get { return GetCell(CellColumnIndex); } }
		#endregion
		#region IDisposable Members
		public void Dispose() {
			(this.columnEnumerator as IOrderedEnumerator<Column>).Dispose();
		}
		#endregion
		#region IEnumerator Members
		object IEnumerator.Current {
			get { return GetCell(CurrentValueOrder); }
		}
		bool skipMoveColumnEnumerator = false;
		bool lastMoveColumnEnumeratorResult = false;
		int indexOfCellInsideColumnRange = -1;
		int cellColumnBorderIndex;
		public bool MoveNext() {
			for (; ; ) {
				lastMoveColumnEnumeratorResult = skipMoveColumnEnumerator ? lastMoveColumnEnumeratorResult
					: (this.columnEnumerator as IOrderedEnumerator<Column>).MoveNext();
				if (!lastMoveColumnEnumeratorResult) 
					return false;
				bool columnObtainedOnThisIteration = !skipMoveColumnEnumerator;
				if (columnEnumerator.DebugInnerIndex >= columnCollection.InnerList.Count)
					throw new InvalidOperationException();
				if (columnObtainedOnThisIteration && !IsColumnSuitableForIteraction(Column, defaultFormatIndex)) {
					skipMoveColumnEnumerator = false;
					continue;
				}
				if (columnObtainedOnThisIteration) {
					indexOfCellInsideColumnRange = CalculateStartIndexOfCellInsideColumnRange();
					cellColumnBorderIndex = CalculateCellColumnBorderIndex(Column, farColumnIndex);
					skipMoveColumnEnumerator = true;
				}
				indexOfCellInsideColumnRange += 1;
				if (CellColumnIndex == cellColumnBorderIndex) {
					skipMoveColumnEnumerator = false;
					continue;
				}
				return true;
			}
		}
		public void Reset() {
			this.indexOfCellInsideColumnRange = Int32.MinValue;
			this.skipMoveColumnEnumerator = false;
			this.lastMoveColumnEnumeratorResult = false;
			(this.columnEnumerator as IOrderedEnumerator<Column>).Reset();
		}
		#endregion
		ICell GetCell(int cellColumnIndex) {
			ICell result = null;
			if (!fakeCellsAsNull)
				result = new FakeCell(new CellPosition(cellColumnIndex, rowIndex), Worksheet);
			return result;
		}
		protected internal int CalculateCellColumnBorderIndex(IColumnRange column, int farColumnIndex) {
			if (!IsReverseOrder)
				return Math.Min(column.EndIndex, farColumnIndex) + 1;
			return Math.Max(column.StartIndex, nearColumnIndex) - 1;
		}
		int CalculateStartIndexOfCellInsideColumnRange() {
			if (Column.StartIndex > Column.EndIndex)
				Exceptions.ThrowInvalidOperationException(String.Format("An invalid column object exists in {0} worksheet.", Worksheet.Name));
			if( !IsReverseOrder)
				return Math.Max(nearColumnIndex - Column.StartIndex, 0) - 1;
			return Math.Max(Column.EndIndex - farColumnIndex, 0) -1; 
		}
		bool IsColumnSuitableForIteraction(IColumnRange columnRange, int defaultFormatIndex) {
			return columnRange.FormatIndex > defaultFormatIndex;
		}
		void IShiftableEnumerator.OnObjectInserted(int insertedItemValueOrder) {
		}
		void IShiftableEnumerator.OnObjectDeleted(int deletedItemValueOrder) {
		}
	}
	public class CellsEnumeratorReplaceBordersInsideMergedCell : IOrderedEnumerator<ICell> {
		readonly IOrderedEnumerator<ICell> enumerator;
		readonly IActualBorderInfo baseActualBorder;
		CellRangesCachedRTree mergedCellsTree;
		CellRange range;
		Worksheet sheet;
		ICell current;
		public CellsEnumeratorReplaceBordersInsideMergedCell(IOrderedEnumerator<ICell> enumerator, CellRange range, IActualBorderInfo baseActualBorder) {
			this.enumerator = enumerator;
			this.range = range;
			this.sheet = (Worksheet)range.Worksheet;
			this.baseActualBorder = baseActualBorder;
		}
		public int CurrentValueOrder { get { return enumerator.CurrentValueOrder; } }
		public bool IsReverseOrder { get { return enumerator.IsReverseOrder; } }
		ICell GetCurrent() {
			if (mergedCellsTree == null)
				mergedCellsTree = CalculateMergedCells();
			if (current != null)
				return current;
			current = enumerator.Current;
			current = CellPositionWithActualBorderFactory.GetCellOverrideBorderInsideMergedCell(current, mergedCellsTree);
			return current;
		}
		public ICell Current { get { return GetCurrent(); } }
		object IEnumerator.Current { get { return GetCurrent(); } }
		public void Dispose() {
			enumerator.Dispose();
		}
		public bool MoveNext() {
			current = null;
			return enumerator.MoveNext();
		}
		public void Reset() {
			current = null;
			mergedCellsTree = null;
			enumerator.Reset();
		}
		CellRangesCachedRTree CalculateMergedCells() {
			CellRangesCachedRTree result = new CellRangesCachedRTree();
			result.InsertRange(sheet.MergedCells.GetMergedCellRangesIntersectsRange(range));
			return result;
		}
		void IShiftableEnumerator.OnObjectInserted(int insertedItemValueOrder) {
			enumerator.OnObjectInserted(insertedItemValueOrder);
		}
		void IShiftableEnumerator.OnObjectDeleted(int deletedItemValueOrder) {
			enumerator.OnObjectDeleted(deletedItemValueOrder);
		}
	}
	#region CellsEnumeratorProviderFakeActualBorderForNonExistingCells
	public class CellsEnumeratorProviderFakeActualBorderForNonExistingCells : ContinuousCellsEnumerator {
		CellRangesCachedRTree mergedCellsTree;
		readonly IActualBorderInfo baseActualBorder;
		public CellsEnumeratorProviderFakeActualBorderForNonExistingCells(IList<ICell> cells, int nearItemIndex, int farItemIndex, bool reverseOrder, Worksheet sheet, int rowIndex, IActualBorderInfo baseActualBorder)
			: base(cells, nearItemIndex, farItemIndex, reverseOrder, sheet, rowIndex) {
			this.baseActualBorder = baseActualBorder;
		}
		protected override void Reset() {
			base.Reset();
			this.mergedCellsTree = null;
		}
		protected override ICell GetItemCore(int index) {
			if (mergedCellsTree == null)
				mergedCellsTree = CalculateMergedCells();
			int itemIndex = Algorithms.BinarySearch(Items, CreateComparable(index));
			if (itemIndex >= 0)
				return CellPositionWithActualBorderFactory.CreateCellOverriddenBorder(Items[itemIndex], mergedCellsTree);
			return CellPositionWithActualBorderFactory.CreateFakeCellWithActualBorder(Sheet, index, RowIndex, mergedCellsTree, baseActualBorder);
		}
		CellRangesCachedRTree CalculateMergedCells() {
			CellRangesCachedRTree result = new CellRangesCachedRTree();
			CellRange range = new CellRange(Sheet, new CellPosition(NearItemIndex, RowIndex), new CellPosition(FarItemIndex, RowIndex));
			result.InsertRange(Sheet.MergedCells.GetMergedCellRangesIntersectsRange(range));
			return result;
		}
	}
	#endregion
	#region ContinuousCellsEnumeratorFakeCellsAsNull
	public class ContinuousCellsEnumeratorFakeCellsAsNull : ContinuousCellsEnumerator {
		public ContinuousCellsEnumeratorFakeCellsAsNull(IList<ICell> cells, int nearItemIndex, int farItemIndex, bool reversedOrder, Worksheet sheet, int rowIndex)
			: base(cells, nearItemIndex, farItemIndex, reversedOrder, sheet, rowIndex) {
		}
		int cachedColumnIndex;
		protected override ICell GetItemCore(int index) {
			cachedColumnIndex = index;
			return null;
		}
		protected override bool IsValidItem(int index) {
			cachedColumnIndex = index;
			return base.IsValidItem(index);
		}
		protected internal override int GetCurrentValueOrder(int itemIndex) {
			return cachedColumnIndex;
		}
		protected override void Reset() {
			base.Reset();
			if (!IsReverseOrder)
				cachedColumnIndex = NearItemIndex - 1;
			else
				cachedColumnIndex = FarItemIndex + 1;
		}
	}
	#endregion
	#region NonEmptyCellsEnumerator<T>
	public class NonEmptyCellsEnumerator<T> : CellsEnumerator<T> where T : class, ICellBase {
		public NonEmptyCellsEnumerator(IList<T> cells, int nearItemIndex, int farItemIndex, bool reverseOrder)
			: base(cells, nearItemIndex, farItemIndex, reverseOrder) {
		}
		protected override bool IsValidItem(int index) {
			T item = Items[index];
			if (item.Value.IsEmpty)
				return false;
			return base.IsValidItem(index);
		}
	}
	#endregion
	#region VisibleCellsEnumerator<T>
	public class VisibleCellsEnumerator<T> : CellsEnumerator<T> where T : class, ICellBase {
		public VisibleCellsEnumerator(IList<T> cells, int nearItemIndex, int farItemIndex, bool reverseOrder)
			: base(cells, nearItemIndex, farItemIndex, reverseOrder) {
		}
		protected override bool IsValidItem(int index) {
			T item = Items[index];
			return item.Sheet.IsColumnVisible(item.ColumnIndex) && item.IsVisible();
		}
	}
	#endregion
	#region VisibleNonEmptyCellsEnumerator<T>
	public class VisibleNonEmptyCellsEnumerator<T> : VisibleCellsEnumerator<T> where T : class, ICellBase {
		public VisibleNonEmptyCellsEnumerator(IList<T> cells, int nearItemIndex, int farItemIndex, bool reverseOrder)
			: base(cells, nearItemIndex, farItemIndex, reverseOrder) {
		}
		protected override bool IsValidItem(int index) {
			T item = Items[index];
			if (item.Value.IsEmpty)
				return false;
			return base.IsValidItem(index);
		}
	}
	#endregion
	#region ColumnCellsEnumerator
	public class ColumnCellsEnumerator : IOrderedEnumerator<ICell> {
		readonly int columnIndex;
		readonly IEnumerator<Row> rows;
		ICell current;
		public ColumnCellsEnumerator(IEnumerator<Row> rows, int columnIndex) {
			this.rows = rows;
			this.columnIndex = columnIndex;
		}
		#region IEnumerator<ICell> Members
		public ICell Current { get { return current; } }
		#endregion
		#region IDisposable Members
		public void Dispose() {
		}
		#endregion
		#region IEnumerator Members
		object IEnumerator.Current { get { return current; } }
		public bool MoveNext() {
			for (; ; ) {
				if (!rows.MoveNext())
					return false;
				Row row = rows.Current;
				this.current = row.Cells.TryGetCell(columnIndex);
				if (this.current != null && IsValidItem(current))
					return true;
			}
		}
		public void Reset() {
			this.current = null;
			this.rows.Reset();
		}
		#endregion
		#region IOrderedEnumerator<ICell> Members
		public int CurrentValueOrder { get { return current.RowIndex; } }
		public bool IsReverseOrder { get { return false; } }
		#endregion
		protected virtual bool IsValidItem(ICell cell) {
			return true;
		}
		#region IShiftableEnumerator
		void IShiftableEnumerator.OnObjectInserted(int insertedItemValueOrder) {
		}
		void IShiftableEnumerator.OnObjectDeleted(int deletedItemValueOrder) {
		} 
		#endregion
	}
	#endregion
	#region ColumnNonEmptyCellsEnumerator
	public class ColumnNonEmptyCellsEnumerator : ColumnCellsEnumerator {
		public ColumnNonEmptyCellsEnumerator(IEnumerator<Row> rows, int columnIndex)
			: base(rows, columnIndex) {
		}
		protected override bool IsValidItem(ICell cell) {
			return !cell.Value.IsEmpty;
		}
	}
	#endregion
	#region ContinuousColumnCells (abstract class)
	public abstract class ContinuousColumnCells : IOrderedEnumerator<ICell> {
		readonly IEnumerator<Row> rows;
		readonly int columnIndex;
		readonly Worksheet sheet;
		readonly int initialRowIndex;
		ICell current;
		int rowIndex;
		protected ContinuousColumnCells(IEnumerator<Row> rows, int columnIndex, Worksheet sheet, int initialRowIndex) {
			this.rows = rows;
			this.columnIndex = columnIndex;
			this.sheet = sheet;
			this.initialRowIndex = initialRowIndex;
			Reset();
		}
		public Worksheet Sheet { get { return sheet; } }
		public int ColumnIndex { get { return columnIndex; } }
		public int InitialRowIndex { get { return initialRowIndex; } }
		#region IEnumerator<ICell> Members
		ICell IEnumerator<ICell>.Current { get { return current; } }
		#endregion
		#region IDisposable Members
		void IDisposable.Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				rows.Dispose();
			}
		}
		#endregion
		#region IEnumerator Members
		object IEnumerator.Current { get { return current; } }
		bool IEnumerator.MoveNext() {
			if (!rows.MoveNext())
				return false;
			rowIndex++;
			Row row = rows.Current;
			while (row != null && row.IsHidden) {
				rowIndex++;
				if (!rows.MoveNext())
					return false;
				row = rows.Current;
			}
			if (row == null) {
				current = CreateFakeCell(columnIndex, rowIndex);
				return true;
			}
			current = row.Cells.TryGetCell(columnIndex);
			if (current == null)
				current = CreateFakeCell(columnIndex, rowIndex);
			else
				current = CreateCellWrapper(current);
			return true;
		}
		protected virtual ICell CreateFakeCell(int columnIndex, int rowIndex) {
			return null;
		}
		protected virtual ICell CreateCellWrapper(ICell cell) {
			return cell;
		}
		public virtual void Reset() {
			rows.Reset();
			this.current = null;
			this.rowIndex = initialRowIndex - 1;
		}
		#endregion
		#region IOrderedEnumerator<ICell> Members
		public int CurrentValueOrder { get { return current.RowIndex; } }
		public bool IsReverseOrder { get { return false; } }
		#endregion
		#region IShiftableEnumerator
		void IShiftableEnumerator.OnObjectInserted(int insertedItemValueOrder) {
		}
		void IShiftableEnumerator.OnObjectDeleted(int deletedItemValueOrder) {
		}
		#endregion
	}
	#endregion
	#region ContinuousColumnCellsProvideFakeActualBorderEnumerator
	public class ContinuousColumnCellsProvideFakeActualBorderEnumerator : ContinuousColumnCells {
		readonly int bottomRow;
		readonly IActualBorderInfo baseActualBorder;
		CellRangesCachedRTree mergedCellsTree;
		public ContinuousColumnCellsProvideFakeActualBorderEnumerator(IEnumerator<Row> rows, int columnIndex, Worksheet sheet, int topRow, int bottomRow, IActualBorderInfo baseActualBorder)
			: base(rows, columnIndex, sheet, topRow) {
			this.bottomRow = bottomRow;
			this.baseActualBorder = baseActualBorder;
		}
		public override void Reset() {
			base.Reset();
			this.mergedCellsTree = null;
		}
		protected override ICell CreateFakeCell(int columnIndex, int rowIndex) {
			if (mergedCellsTree == null)
				mergedCellsTree = CalculateMergedCells();
			return CellPositionWithActualBorderFactory.CreateFakeCellWithActualBorder(Sheet, columnIndex, rowIndex, mergedCellsTree, baseActualBorder);
		}
		protected override ICell CreateCellWrapper(ICell cell) {
			if (mergedCellsTree == null)
				mergedCellsTree = CalculateMergedCells();
			return CellPositionWithActualBorderFactory.CreateCellOverriddenBorder(cell, mergedCellsTree);
		}
		CellRangesCachedRTree CalculateMergedCells() {
			CellRangesCachedRTree result = new CellRangesCachedRTree();
			CellRange range = new CellRange(Sheet, new CellPosition(ColumnIndex, InitialRowIndex), new CellPosition(ColumnIndex, bottomRow));
			result.InsertRange(Sheet.MergedCells.GetMergedCellRangesIntersectsRange(range));
			return result;
		}
	}
	#endregion
	#region ContinuousColumnCellsProvideColumnBorderEnumerator
	public class ContinuousColumnCellsProvideColumnBorderEnumerator : ContinuousColumnCells {
		readonly IColumnRange columnRange;
		public ContinuousColumnCellsProvideColumnBorderEnumerator(IEnumerator<Row> rows, int columnIndex, Worksheet sheet, int initialRowIndex)
			: base(rows, columnIndex, sheet, initialRowIndex) {
			this.columnRange = sheet.Columns.TryGetColumnRange(columnIndex);
		}
		protected override ICell CreateFakeCell(int columnIndex, int rowIndex) {
			return new FakeCellWithColumnFormatting(columnRange, columnIndex, rowIndex);
		}
	}
	#endregion
}
