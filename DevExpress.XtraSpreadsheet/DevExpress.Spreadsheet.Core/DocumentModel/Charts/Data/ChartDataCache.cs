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
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region ChartDataCache
	public class ChartDataCache : ICellTable {
		#region Fields
		readonly DocumentModel documentModel;
		readonly ChartCacheRowCollection rows;
		#endregion
		public ChartDataCache(DocumentModel documentModel) {
			this.documentModel = documentModel;
			rows = new ChartCacheRowCollection();
		}
		public ChartCacheRowCollection Rows { get { return rows; } }
		#region ICellTable Members
		public string Name { get { return "ChartCacheTable"; } }
		public int SheetId { get { return -1; } }
		public IModelWorkbook Workbook { get { return documentModel; } }
		IRowCollectionBase ICellTable.Rows { get { return rows; } }
		public ChartCacheCell TryGetCell(int column, int row) {
			ChartCacheRow rowObject = rows.TryGetRow(row);
			if (rowObject == null)
				return null;
			return rowObject.Cells.TryGetCell(column);
		}
		ICellBase ICellTable.TryGetCell(int column, int row) {
			return TryGetCell(column, row);
		}
		public ChartCacheCell GetCell(int column, int row) {
			return rows[row].Cells[column];
		}
		ICellBase ICellTable.GetCell(int column, int row) {
			return GetCell(column, row);
		}
		public VariantValue GetCalculatedCellValue(int column, int row) {
			CellBase cell = TryGetCell(column, row);
			if (cell == null)
				return VariantValue.Empty;
			return cell.Value;
		}
		public int MaxRowCount { get { return IndicesChecker.MaxRowCount; } }
		public int MaxColumnCount { get { return IndicesChecker.MaxColumnCount; } }
		public bool ReadOnly { get { return true; } }
		#endregion
	}
	#endregion
	#region ChartCacheRowCollection
	public class ChartCacheRowCollection : RowCollectionBase<ChartCacheRow>, IRowCollectionBase {
		public ChartCacheRowCollection()
			: base() {
		}
		public override ChartCacheRow CreateRow(int index) {
			return new ChartCacheRow(index);
		}
		protected override bool CanRemove(int position) {
			return false;
		}
		#region IRowCollection Members
		IEnumerable IRowCollectionBase.GetExistingRows(int topRow, int bottomRow, bool reverseOrder) {
			return GetExistingRows(topRow, bottomRow, reverseOrder);
		}
		IEnumerable IRowCollectionBase.GetExistingVisibleRows(int topRow, int bottomRow, bool reverseOrder) {
			return GetExistingVisibleRows(topRow, bottomRow, reverseOrder);
		}
		IEnumerator IRowCollectionBase.GetExistingRowsEnumerator(int topRow, int bottomRow) {
			return GetExistingRowsEnumerator(topRow, bottomRow);
		}
		IEnumerator IRowCollectionBase.GetExistingRowsEnumerator(int topRow, int bottomRow, bool reverseOrder) {
			return GetExistingRowsEnumerator(topRow, bottomRow, reverseOrder);
		}
		IRowBase IRowCollectionBase.TryGetRow(int index) {
			return this.TryGetRow(index);
		}
		#endregion
		protected override void OffsetCellRowIndices(int rowIndex, int offset) {
		}
	}
	#endregion
	#region ChartCacheRow
	public class ChartCacheRow : ICellPosition, IRowBase {
		#region Fields
		readonly int index;
		readonly ChartCacheCellCollection cells;
		#endregion
		public ChartCacheRow(int index) {
			this.index = index;
			this.cells = new ChartCacheCellCollection(this);
		}
		#region Properties
		public IWorksheet Sheet { get { return null; } }
		public int Index { get { return index; } }
		public ChartCacheCellCollection Cells { get { return cells; } }
		public bool IsVisible { get { return true; } }
		public bool HasVisibleFill { get { return false; } }
		public bool HasVisibleBorder { get { return false; } }
		#endregion
#if BTREE
		#region IIndexedObject Members
		long IIndexedObject.Index { get { return Index; } }
		#endregion
#endif
		#region ICellPosition Members
		public CellPosition GetCellPosition(int index) {
			return new CellPosition(index, Index);
		}
		#endregion
		#region IRow Members
		ICellCollectionBase IRowBase.Cells { get { return cells; } }
		#endregion
		public void OffsetRowIndex(int offset) {
		}
		public override string ToString() {
			return String.Format("Range=[{2}:{2}], Index=[{0}]", Index, Index + 1, Index + 1);
		}
		#region Check Integrity
		public void CheckIntegrity(CheckIntegrityFlags flags) {
			if (index < 0)
				IntegrityChecks.Fail(String.Format("RowBase: invalid index={0}", index));
		}
		#endregion
	}
	#endregion
	#region ChartCacheCellCollection
	public class ChartCacheCellCollection : CellCollectionBase<ChartCacheCell> {
		public ChartCacheCellCollection(ICellPosition owner)
			: base(owner) {
		}
		protected override void AfterInsertCell(int innerIndex, ChartCacheCell cell) {
		}
		protected override void BeforeRemoveCell(ChartCacheCell cell) {
		}
		public override ChartCacheCell CreateNewCellCore(CellPosition position) {
			return new ChartCacheCell(position);
		}
		public override void OffsetRowIndex(int offset) {
		}
		public override void OffsetColumnIndex(int offset) {
		}
	}
	#endregion
	#region ChartCacheCell
	public class ChartCacheCell : CellBase, IFormatStringAccessor {
		#region Fields
		VariantValue value;
		string formatString;
		#endregion
		public ChartCacheCell(CellPosition position)
			: base(position, 0) {
			formatString = string.Empty;
		}
		#region Properties
		public override IWorksheet Sheet { get { return null; } }
		public override VariantValue Value { get { return value; } set { this.value = value; } }
		#region IFormatStringAccessor Members
		public string FormatString {
			get { return formatString; }
			set {
				if (value == null)
					value = string.Empty;
				formatString = value;
			}
		}
		#endregion
		#endregion
		public override void SetValue(VariantValue value) {
			AssignValue(value);
		}
		public override void AssignValue(VariantValue value) {
			this.value = value;
		}
		public override void AssignValueCore(VariantValue value) {
			this.value = value;
		}
		public override void OffsetRowIndex(int offset, bool needChangeRow) {
		}
		public override void OffsetColumnIndex(int offset) {
		}
		public override bool IsVisible() {
			return true;
		}
	}
	#endregion
}
