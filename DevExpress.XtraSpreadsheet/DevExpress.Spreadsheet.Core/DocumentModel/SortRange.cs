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

using System.Diagnostics;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Commands.Internal;
using DevExpress.XtraSpreadsheet.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Model {
	#region RangeSortEngine
	public class RangeSortEngine {
		readonly Worksheet sheet;
		readonly DocumentModel documentModel;
		public RangeSortEngine(Worksheet sheet) {
			Guard.ArgumentNotNull(sheet, "sheet");
			this.sheet = sheet;
			this.documentModel = sheet.Workbook;
		}
		#region Properties
		public DocumentModel DocumentModel { get { return documentModel; } }
		public Worksheet Sheet { get { return sheet; } }
		IRowCollection Rows { get { return sheet.Rows; } }
		#endregion
		public void Sort(CellRange range, IList<ModelSortField> sortFields) {
			Debug.Assert(Object.ReferenceEquals(Sheet, range.Worksheet));
			if (range.Height <= 1)
				return;
			DocumentModel.BeginUpdate();
			try {
				Sheet.UnMergeCells(range, null);
				PerformSort(range, sortFields);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		void PerformSort(CellRange range, IList<ModelSortField> sortFields) {
			int topRow = range.TopLeft.Row;
			int bottomRow = range.BottomRight.Row;
			if (Rows != null) {
				if (Rows.Count <= 0)
					bottomRow = topRow;
				else
					bottomRow = Math.Min(Rows.Last.Index, bottomRow);
			}
			IComparer<Row> comparer = CreateComparer(range, sortFields);
			BTreeSorter<Row> tree = CreateTree(topRow, bottomRow, comparer);
			List<int> sortedIndexes = GetSortedIndexes(tree);
			PerformSortCore(range, topRow, sortedIndexes);
		}
		void PerformSortCore(CellRange range, int currentRowId, List<int> sortedIndexes) {
			BufferedCellContent[] buffer = CreateBuffer(range);
			while (sortedIndexes.Count > 0) {
				Row row = Rows.TryGetRow(currentRowId);
				if (row != null && !row.IsVisible) {
					currentRowId++;
					continue;
				}
				int sourceRowIndex = sortedIndexes[0];
				ExcangeRowValues(row, range, sourceRowIndex, currentRowId, buffer);
				ChangeIndex(sortedIndexes, sourceRowIndex, currentRowId);
				currentRowId++;
			}
		}
		#region PrepareSort Methods
		IComparer<Row> CreateComparer(CellRange range, IList<ModelSortField> sortFields) {
			List<IComparer<Row>> comparers = new List<IComparer<Row>>();
			int count = sortFields.Count;
			for (int i = 0; i < count; i++) {
				ModelSortField sortField = sortFields[i];
				comparers.Add(new DataSortComparer(range, range.TopLeft.Column + sortField.ColumnOffset, sortField.Comparer));
			}
			return new RowDataSortComparer(comparers);
		}
		BTreeSorter<Row> CreateTree(int topRow, int bottomRow, IComparer<Row> comparer) {
			BTreeSorter<Row> tree = new BTreeSorter<Row>(comparer);
			for (int i = topRow; i <= bottomRow; i++) {
				Row row = Rows.TryGetRow(i);
				if (row == null || !row.IsVisible)
					continue;
				tree.Insert(row);
			}
			return tree;
		}
		List<int> GetSortedIndexes(BTreeSorter<Row> tree) {
			List<int> result = new List<int>();
			foreach (Row sortedRow in tree)
				result.Add(sortedRow.Index);
			return result;
		}
		void ChangeIndex(List<int> sortedIndexes, int sourceRowIndex, int targetRowIndex) {
			int position = sortedIndexes.IndexOf(targetRowIndex);
			if (position > 0)
				sortedIndexes[position] = sourceRowIndex;
			sortedIndexes.RemoveAt(0);
		}
		BufferedCellContent[] CreateBuffer(CellRange range) {
			int count = range.Width;
			BufferedCellContent[] result = new BufferedCellContent[count];
			for (int i = 0; i < count; i++)
				result[i] = new BufferedCellContent();
			return result;
		}
		#endregion
		void ExcangeRowValues(Row row, CellRange range, int sourceRowIndex, int targetRowIndex, BufferedCellContent[] buffer) {
			int leftColumn = range.TopLeft.Column;
			int rightColumn = range.BottomRight.Column;
			ClearBuffer(buffer);
			ReplaceThingsPRNVisitor firstVisitor = CreateFormulaRowCorrectionVisitor(range, targetRowIndex - sourceRowIndex);
			if (row == null) {
				CopyRowValues(sourceRowIndex, targetRowIndex, leftColumn, rightColumn, firstVisitor);
				CopyFromBuffer(buffer, sourceRowIndex, leftColumn);
				DeleteRow(sourceRowIndex, range);
			}
			else {
				ReplaceThingsPRNVisitor secondVisitor = CreateFormulaRowCorrectionVisitor(range, sourceRowIndex - targetRowIndex);
				CopyToBuffer(buffer, targetRowIndex, leftColumn, rightColumn, secondVisitor);
				CopyRowValues(sourceRowIndex, targetRowIndex, leftColumn, rightColumn, firstVisitor);
				CopyFromBuffer(buffer, sourceRowIndex, leftColumn);
			}
		}
		void DeleteRow(int rowIndex, CellRange range) {
			int offsetIndex = rowIndex - range.TopLeft.Row;
			CellRange rangeToClear = range.GetSubRowRange(offsetIndex, offsetIndex);
			Sheet.ClearCellsNoShift(rangeToClear);
			Rows.CheckForEmptyRows(rowIndex, rowIndex);
		}
		ReplaceThingsPRNVisitor CreateFormulaRowCorrectionVisitor(CellRange range, int rowOffset) {
			CellPositionOffset offset = new CellPositionOffset(0, rowOffset);
			return new SortRangeFormulaReferencesVisitor(range.Worksheet.SheetId, offset, range.Worksheet.Workbook.DataContext);
		}
		void ClearBuffer(BufferedCellContent[] buffer) {
			int count = buffer.Length;
			for (int i = 0; i < count; i++)
				buffer[i].Clear();
		}
		void CopyToBuffer(BufferedCellContent[] buffer, int targetRowIndex, int leftColumn, int rightColumn, ReplaceThingsPRNVisitor visitor) {
			Row row = Rows[targetRowIndex];
			foreach (ICell cell in row.Cells.GetExistingCells(leftColumn, rightColumn, false)) {
				BufferedCellContent content = buffer[cell.ColumnIndex - leftColumn];
				content.CopyFromCell(cell);
				content.ProcessFormula(cell, visitor);
			}
		}
		void CopyRowValues(int sourceRowIndex, int targetRowIndex, int leftColumn, int rightColumn, ReplaceThingsPRNVisitor visitor) {
			ICellCollection sourceCells = Sheet.Rows[sourceRowIndex].Cells;
			ICellCollection targetCells = Sheet.Rows[targetRowIndex].Cells;
			BufferedCellContent buffer = new BufferedCellContent();
			for (int i = leftColumn; i <= rightColumn; i++) {
				ICell sourceCell = sourceCells.TryGetCell(i);
				ICell targetCell = targetCells.TryGetCell(i);
				if (sourceCell == null && targetCell == null)
					continue;
				Sheet.CellTags.ExchangeValues(new CellPosition(i, sourceRowIndex), new CellPosition(i, targetRowIndex));
				if (sourceCell == null) {
					Sheet.ClearCellsNoShift(targetCell.GetRange());
					continue;
				}
				buffer.CopyFromCell(sourceCell);
				buffer.ProcessFormula(sourceCell, visitor);
				if (targetCell == null)
					Sheet.ClearCellsNoShift(sourceCell.GetRange());
				buffer.CopyToCell(targetCells[i]);
			}
		}
		void CopyFromBuffer(BufferedCellContent[] buffer, int sourceRowIndex, int leftColumn) {
			Row row = Rows[sourceRowIndex];
			ICellCollection cells = row.Cells;
			int count = buffer.Length;
			for (int i = 0; i < count; i++) {
				BufferedCellContent bufferContent = buffer[i];
				ICell cell = cells.TryGetCell(i + leftColumn);
				if (cell != null)
					bufferContent.CopyToCell(cell);
				else {
					if (!bufferContent.IsEmpty)
						bufferContent.CopyToCell(cells[i + leftColumn]);
				}
			}
		}
	}
	#endregion
	public class ModelSortField {
		public IComparer<VariantValue> Comparer { get; set; }
		public int ColumnOffset { get; set; }
	}
	#region BufferedCellContent
	public class BufferedCellContent {
		public Comment Comment { get; set; }
		public VariantValue Value { get; set; }
		public FormulaBase Formula { get; set; }
		public int FormatIndex { get; set; }
		public CellFormatApplyOptions ApplyFormattingOptions { get; set; }
		public bool IsEmpty { get { return Formula == null && Value.IsEmpty; } }
		public void Clear() {
			Value = VariantValue.Empty;
			Formula = null;
		}
		public void CopyFromCell(ICell cell) {
			Comment = cell.Worksheet.GetComment(cell.Position);
			Formula = cell.GetFormula();
			CopyFormatting(cell);
			if (Formula == null)
				Value = cell.Value;
		}
		public void CopyToCell(ICell cell) {
			if (Comment != null)
				Comment.Reference = cell.Position;
			ApplyFormatting(cell);
			if (Formula == null)
				cell.Value = Value;
			else
				cell.SetFormula(Formula);
		}
		public void ProcessFormula(ICell cell, ReplaceThingsPRNVisitor visitor) {
			if (Formula != null) {
				SharedFormulaRef sharedFormula = Formula as SharedFormulaRef;
				if (sharedFormula != null) {
					ParsedExpression expression = sharedFormula.GetNormalCellFormula(cell);
					Formula = new Formula(cell, expression);
				}
				Formula.UpdateFormula(visitor, cell);
			}
		}
		void CopyFormatting(ICell cell) {
			FormatIndex = cell.FormatIndex;
			ApplyFormattingOptions = CellFormatApplyOptions.None;
			if (cell.ApplyNumberFormat)
				ApplyFormattingOptions |= CellFormatApplyOptions.ApplyNumberFormat;
			if (cell.ApplyFont)
				ApplyFormattingOptions |= CellFormatApplyOptions.ApplyFont;
			if (cell.ApplyFill)
				ApplyFormattingOptions |= CellFormatApplyOptions.ApplyFill;
			if (cell.ApplyAlignment)
				ApplyFormattingOptions |= CellFormatApplyOptions.ApplyAlignment;
			if (cell.ApplyProtection)
				ApplyFormattingOptions |= CellFormatApplyOptions.ApplyProtection;
			if (cell.Style != cell.DocumentModel.StyleSheet.CellStyles.Normal)
				ApplyFormattingOptions |= CellFormatApplyOptions.ApplyStyle;
		}
		bool GetApplyClearOptions(bool apply, CellFormatApplyOptions options) {
			return apply && (ApplyFormattingOptions & options) == 0;
		}
		void ApplyFormatting(ICell cell) {
			CellFormat format = cell.DocumentModel.Cache.CellFormatCache[FormatIndex] as CellFormat;
			if (format == null)
				return;
			CellFormatApplyOptions clearOptions = CellFormatApplyOptions.None;
			if (GetApplyClearOptions(cell.ApplyNumberFormat, CellFormatApplyOptions.ApplyNumberFormat))
				clearOptions |= CellFormatApplyOptions.ApplyNumberFormat;
			if (GetApplyClearOptions(cell.ApplyFont, CellFormatApplyOptions.ApplyFont))
				clearOptions |= CellFormatApplyOptions.ApplyFont;
			if (GetApplyClearOptions(cell.ApplyFill, CellFormatApplyOptions.ApplyFill))
				clearOptions |= CellFormatApplyOptions.ApplyFill;
			if (GetApplyClearOptions(cell.ApplyAlignment, CellFormatApplyOptions.ApplyAlignment))
				clearOptions |= CellFormatApplyOptions.ApplyAlignment;
			if (GetApplyClearOptions(cell.ApplyProtection, CellFormatApplyOptions.ApplyProtection))
				clearOptions |= CellFormatApplyOptions.ApplyProtection;
			if (GetApplyClearOptions(cell.Style != cell.DocumentModel.StyleSheet.CellStyles.Normal, CellFormatApplyOptions.ApplyStyle))
				clearOptions |= CellFormatApplyOptions.ApplyStyle;
			if (ApplyFormattingOptions != CellFormatApplyOptions.None)
				cell.ApplyFormat(format, ApplyFormattingOptions);
			if (clearOptions != CellFormatApplyOptions.None)
				cell.ClearFormat(clearOptions);
		}
	}
	#endregion
	#region DataSortComparer
	public class DataSortComparer : IComparer<Row> {
		readonly int columnIndex;
		readonly Worksheet sheet;
		readonly IComparer<VariantValue> comparer;
		public DataSortComparer(CellRange range, int columnIndex, IComparer<VariantValue> comparer) {
			Guard.ArgumentNotNull(range, "range");
			Guard.ArgumentNotNull(comparer, "comparer");
			this.columnIndex = columnIndex;
			this.sheet = (Worksheet)range.Worksheet;
			this.comparer = comparer;
		}
		protected IComparer<VariantValue> Comparer { get { return comparer; } }
		protected Worksheet Sheet { get { return sheet; } }
		public int Compare(Row x, Row y) {
			ICell cellX = x == null ? null : x.Cells.TryGetCell(columnIndex);
			ICell cellY = y == null ? null : y.Cells.TryGetCell(columnIndex);
			if (cellX == cellY)
				return 0;
			VariantValue valueX = cellX != null ? cellX.Value : VariantValue.Empty;
			VariantValue valueY = cellY != null ? cellY.Value : VariantValue.Empty;
			return Compare(valueX, valueY);
		}
		int Compare(VariantValue valueX, VariantValue valueY) {
			return Comparer.Compare(valueX, valueY);
		}
	}
	#endregion
	#region ComplexDataSortComparer
	public class RowDataSortComparer : IComparer<Row> {
		readonly IList<IComparer<Row>> comparers;
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006")]
		public RowDataSortComparer(IList<IComparer<Row>> comparers) {
			Guard.ArgumentNotNull(comparers, "comparers");
			this.comparers = comparers;
		}
		public int Compare(Row x, Row y) {
			int count = comparers.Count;
			for (int i = 0; i < count; i++) {
				int result = comparers[i].Compare(x, y);
				if (result != 0)
					return result;
			}
			return DoubleComparer.Compare(x.Index, y.Index);
		}
	}
	#endregion
	#region SortVariantValueComparer
	public class SortVariantValueComparer : DefaultVariantValueComparer {
		public SortVariantValueComparer(SharedStringTable stringTable)
			: base(stringTable) {
		}
		protected override int CompareErrors(ICellError x, ICellError y) {
			return 0; 
		}
	}
	#endregion
	#region DescendingSortVariantValueComparer
	public class DescendingSortVariantValueComparer : SortVariantValueComparer {
		public DescendingSortVariantValueComparer(SharedStringTable stringTable)
			: base(stringTable) {
		}
		public override int Compare(VariantValue x, VariantValue y) {
			return -base.Compare(x, y);
		}
		protected override VariantValueType GetSortType(VariantValue value) {
			VariantValueType result = base.GetSortType(value);
			if (result == VariantValueType.None)
				result = VariantValueType.Missing; 
			return result;
		}
	}
	#endregion
	#region ApiCellValueComparer
	public class ApiCellValueComparer : IComparer<VariantValue> {
		readonly WorkbookDataContext dataContext;
		readonly IComparer<DevExpress.Spreadsheet.CellValue> comparer;
		public ApiCellValueComparer(WorkbookDataContext dataContext, IComparer<DevExpress.Spreadsheet.CellValue> comparer) {
			Guard.ArgumentNotNull(dataContext, "dataContext");
			Guard.ArgumentNotNull(comparer, "comparer");
			this.dataContext = dataContext;
			this.comparer = comparer;
		}
		public int Compare(VariantValue x, VariantValue y) {
			DevExpress.Spreadsheet.CellValue valueX = new Spreadsheet.CellValue(x, dataContext);
			DevExpress.Spreadsheet.CellValue valueY = new Spreadsheet.CellValue(y, dataContext);
			return comparer.Compare(valueX, valueY);
		}
	}
	#endregion
	#region BTreeSorter
	public class BTreeSorter<TValue> : IEnumerable<TValue> where TValue : class {
		#region Fields
		readonly IComparer<TValue> comparer;
		BTreeNode<TValue> root;
		const int defaultMaxLeafSize = 1000;
		int maxLeafSize;
		#endregion
		public BTreeSorter(IComparer<TValue> comaprer)
			: this(comaprer, defaultMaxLeafSize) {
		}
		public BTreeSorter(IComparer<TValue> comaprer, int maxLeafSize) {
			Debug.Assert(maxLeafSize >= 2);
			this.root = new BTreeNode<TValue>();
			this.comparer = comaprer;
			this.maxLeafSize = maxLeafSize;
		}
		#region Properties
		public IComparer<TValue> Comparer { get { return comparer; } }
		public BTreeNode<TValue> Root { get { return root; } }
		#endregion
		#region FindNextLeaf
		BTreeNode<TValue> FindFirstLeaf() {
			BTreeNode<TValue> result = root;
			while (!result.IsLeaf)
				result = result.Left;
			return result;
		}
		BTreeNode<TValue> FindNextLeaf(BTreeNode<TValue> node) {
			if (node == null || node == root)
				return null;
			BTreeNode<TValue> parent = node.Parent;
			if (Object.ReferenceEquals(parent.Left, node))
				return FindNextToTheRight(parent.Right);
			else
				return parent == root ? null : FindNextLeaf(parent);
		}
		BTreeNode<TValue> FindNextToTheRight(BTreeNode<TValue> node) {
			if (!node.IsLeaf)
				return FindNextToTheRight(node.Left);
			return node;
		}
		#endregion
		#region Insert
		public void Insert(TValue value) {
			Insert(root, value);
		}
		void Insert(BTreeNode<TValue> node, TValue value) {
			if (node.IsLeaf) {
				node.Values.Add(value);
				node.IsSorted = false;
				if (node.Values.Count == maxLeafSize)
					Split(node);
			}
			else {
				BTreeNode<TValue> next = FindNextToTheRight(node.Right);
				if (comparer.Compare(value, next.Values[0]) < 0)
					Insert(node.Left, value);
				else
					Insert(node.Right, value);
			}
		}
		void Split(BTreeNode<TValue> node) {
			node.GetSortedValues(comparer);
			node.IsLeaf = false;
			int halfSize = maxLeafSize % 2 == 0 ? maxLeafSize / 2 : maxLeafSize / 2 + 1;
			node.Left = CreateLeafNode(node, 0, halfSize);
			node.Right = CreateLeafNode(node, halfSize, maxLeafSize - halfSize);
			node.Values.Clear();
		}
		BTreeNode<TValue> CreateLeafNode(BTreeNode<TValue> parent, int startIndex, int count) {
			BTreeNode<TValue> result = new BTreeNode<TValue>(parent);
			result.Values.AddRange(parent.Values.GetRange(startIndex, count));
			result.IsSorted = true;
			return result;
		}
		#endregion
		public IEnumerator<TValue> GetEnumerator() {
			return new BTreeEnumerator(this);
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		#region BTreeEnumerator
		class BTreeEnumerator : IEnumerator<TValue> {
			readonly BTreeSorter<TValue> sorter;
			IEnumerator<TValue> currentEnumerator;
			BTreeNode<TValue> currentLeaf;
			public BTreeEnumerator(BTreeSorter<TValue> sorter) {
				this.sorter = sorter;
				ResetInnerEnumerator();
			}
			public TValue Current { get { return currentEnumerator.Current; } }
			public void Dispose() {
				currentEnumerator.Dispose();
			}
			IEnumerator<TValue> GetInnerEnumerator(BTreeNode<TValue> node) {
				return node.GetSortedValues(sorter.Comparer).GetEnumerator();
			}
			void ResetInnerEnumerator() {
				this.currentLeaf = sorter.FindFirstLeaf();
				this.currentEnumerator = GetInnerEnumerator(currentLeaf);
			}
			object System.Collections.IEnumerator.Current { get { return ((BTreeEnumerator)this).Current; } }
			public bool MoveNext() {
				if (!currentEnumerator.MoveNext()) {
					currentLeaf = sorter.FindNextLeaf(currentLeaf);
					if (currentLeaf != null) {
						currentEnumerator = GetInnerEnumerator(currentLeaf);
						return currentEnumerator.MoveNext();
					}
					return false;
				}
				return true;
			}
			public void Reset() {
				ResetInnerEnumerator();
				currentEnumerator.Reset();
			}
		}
		#endregion
	}
	#endregion
	#region BTreeNode
	public class BTreeNode<T> {
		#region Fields
		readonly List<T> values;
		readonly BTreeNode<T> parent;
		BTreeNode<T> left;
		BTreeNode<T> right;
		#endregion
		public BTreeNode() : this(null) { }
		public BTreeNode(BTreeNode<T> parent) {
			this.values = new List<T>();
			this.parent = parent;
			IsLeaf = true;
		}
		#region Properties
		public List<T> Values { get { return values; } }
		public BTreeNode<T> Parent { get { return parent; } }
		public BTreeNode<T> Left { get { return left; } set { left = value; } }
		public BTreeNode<T> Right { get { return right; } set { right = value; } }
		public bool IsLeaf { get; set; }
		public bool IsSorted { get; set; }
		#endregion
		public List<T> GetSortedValues(IComparer<T> comparer) {
			if (!IsSorted) {
				values.Sort(comparer);
				IsSorted = true;
			}
			return values;
		}
	}
	#endregion
}
