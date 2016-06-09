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
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	public interface ICellBase : ISheetPosition
#if BTREE
, IIndexedObject
#endif
 {
		CellKey Key { get; }
		int ColumnIndex { get; }
		int RowIndex { get; }
		IWorksheet Sheet { get; }
		VariantValue Value { get; set; }
		CellPosition Position { get; }
		CellRange GetRange();
		void SetValue(VariantValue value);
		void AssignValue(VariantValue value);
		void AssignValueCore(VariantValue value);
		bool IsVisible();
		void OffsetRowIndex(int offset, bool needChangeRow);
		void OffsetRowIndexCore(int offset, bool needChangeRow);
		void OffsetRowIndexInternal(int offset, bool needChangeRow);
		void OffsetColumnIndex(int offset);
		void OffsetColumnIndexCore(int offset);
		void CheckIntegrity(CheckIntegrityFlags flags);
	}
	#region CellBase (abstract class)
	public abstract class CellBase : ICellBase {
		#region Fields
		CellKey key;
		#endregion
		protected CellBase(CellPosition position, int sheetId) {
			Guard.ArgumentNotNull(position, "position");
			this.key = new CellKey(sheetId, position.Column, position.Row);
		}
		#region Properties
		public CellKey Key { get { return key; } }
		public int ColumnIndex { get { return key.ColumnIndex; } }  
		public int RowIndex { get { return key.RowIndex; } }  
		public abstract IWorksheet Sheet { get; }
		public abstract VariantValue Value { get; set; }
		public CellPosition Position { get { return new CellPosition(ColumnIndex, RowIndex); } }
		#endregion
		public abstract void SetValue(VariantValue value);
		public abstract void AssignValue(VariantValue value);
		public abstract void AssignValueCore(VariantValue value);
		public abstract bool IsVisible();
		public virtual void OffsetRowIndex(int offset, bool needChangeRow) {
			OffsetRowIndexCore(offset, needChangeRow);
		}
		public void OffsetRowIndexCore(int offset, bool needChangeRow) {
			this.key = new CellKey(key.SheetId, ColumnIndex, RowIndex + offset);
			if (needChangeRow) {
				IRowCollection rows = Sheet.Rows as IRowCollection;
				rows[RowIndex - offset].Cells.RemoveAtColumnIndex(ColumnIndex);
				rows[RowIndex].Cells.TryToInsertCell(this as ICell);
			}
		}
		public void OffsetRowIndexInternal(int offset, bool needChangeRow) {
			this.key = new CellKey(key.SheetId, ColumnIndex, RowIndex + offset);
			if (needChangeRow) {
				IRowCollection rows = Sheet.Rows as IRowCollection;
				rows[RowIndex - offset].Cells.RemoveAtColumnIndexInternal(ColumnIndex);
				rows[RowIndex].Cells.TryToInsertCellInternal(this as ICell);
			}
		}
		public virtual void OffsetColumnIndex(int offset) {
			OffsetColumnIndexCore(offset);
		}
		public virtual void OffsetColumnIndexCore(int offset) {
			this.key = new CellKey(key.SheetId, ColumnIndex + offset, RowIndex);
		}
		public CellRange GetRange() {
			CellPosition topLeft = this.Position;
			CellRange range = new CellRange(Sheet, topLeft, topLeft);
			return range;
		}
		#region ISheetPosition Members
		public int SheetId { get { return Sheet.SheetId; } }
		public int LeftColumnIndex { get { return ColumnIndex; } }
		public int RightColumnIndex { get { return ColumnIndex; } }
		public int TopRowIndex { get { return RowIndex; } }
		public int BottomRowIndex { get { return RowIndex; } }
		#endregion
		#region Check Integrity
		public virtual void CheckIntegrity(CheckIntegrityFlags flags) {
			if (Sheet == null)
				IntegrityChecks.Fail("CellBase: sheet should not be null");
			if (RowIndex < 0)
				IntegrityChecks.Fail(String.Format("CellBase: Invalid RowIndex={0}", RowIndex));
			if (ColumnIndex < 0)
				IntegrityChecks.Fail(String.Format("CellBase: Invalid ColumnIndex={0}", ColumnIndex));
		}
		#endregion
#if BTREE
		#region IIndexedObject Members
		long IIndexedObject.Index { get { return (long)RowIndex * (long)IndicesChecker.MaxColumnCount + ColumnIndex; } }
		#endregion
#endif
	}
	#endregion
}
