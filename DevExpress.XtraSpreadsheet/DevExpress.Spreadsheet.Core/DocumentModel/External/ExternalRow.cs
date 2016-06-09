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
using DevExpress.Utils;
using System.Collections.Generic;
using System.Collections;
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Model.External {
	#region ExternalRow
	public class ExternalRow : IExternalCellContainer, IRowBase {
		#region Fields
		readonly int index;
		readonly ExternalWorksheet sheet;
		readonly ExternalCellCollection cells;
		#endregion
		public ExternalRow(int index, ExternalWorksheet sheet) {
			Guard.ArgumentNotNull(sheet, "sheet");
			this.index = index;
			this.sheet = sheet;
			this.cells = new ExternalCellCollection(this);
		}
		#region Properties
		public IWorksheet Sheet { get { return sheet; } }
		ExternalWorksheet IExternalCellContainer.Sheet { get { return sheet; } }
		public int Index { get { return index; } }
		public ExternalCellCollection Cells { get { return cells; } }
		public bool IsVisible { get { return true; } }
		public bool HasVisibleFill { get { return false; } }
		public bool HasVisibleBorder { get { return false; } }
		#endregion
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
			return String.Format("Range=[{2}:{2}], Index=[{0}] sheet='{1}'", Index, Sheet.Name, Index + 1, Index + 1);
		}
		#region Check Integrity
		public void CheckIntegrity(CheckIntegrityFlags flags) {
			if (sheet == null)
				IntegrityChecks.Fail("RowBase: sheet should not be null");
			if (index < 0)
				IntegrityChecks.Fail(String.Format("RowBase: invalid index={0}", index));
		}
		#endregion
		#if BTREE
		#region IIndexedObject Members
		long IIndexedObject.Index { get { return Index; } }
		#endregion
		#endif
	}
	#endregion
	#region ExternalRowCollection
	public class ExternalRowCollection : CachedRowCollectionBase<ExternalRow>, IRowCollectionBase {
		public ExternalRowCollection(ExternalWorksheet sheet)
			: base(sheet) {
		}
		#region Properties
		public new ExternalWorksheet Sheet { get { return (ExternalWorksheet)base.Sheet; } }
		#endregion
		public override ExternalRow CreateRow(int index) {
			return new ExternalRow(index, Sheet);
		}
		protected override bool CanRemove(int position) {
			return true;
		}
		#region IRowCollection Members
		IEnumerable IRowCollectionBase.GetExistingRows(int topRow, int bottomRow, bool reverseOrder) {
			return GetExistingRows(topRow, bottomRow, reverseOrder);
		}
		IEnumerable IRowCollectionBase.GetExistingVisibleRows(int topRow, int bottomRow, bool reverseOrder) {
			return GetExistingRows(topRow, bottomRow, reverseOrder);
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
}
