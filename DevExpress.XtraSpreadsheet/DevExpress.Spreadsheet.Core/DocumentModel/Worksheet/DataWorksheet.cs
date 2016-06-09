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
using DevExpress.Spreadsheet;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Localization;
namespace DevExpress.XtraSpreadsheet.Model {
#if DATA_SHEET
	#region Virtual worksheet spike
	#region VirtualWorksheet
	public class VirtualWorksheet : Worksheet {
		#region Static Members
		public static void InvalidOperationError() {
			SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorInvalidOperationForDataSheet);
		}
		#endregion
		readonly IVirtualData data;
		readonly VirtualRowCollection rows;
		public VirtualWorksheet(DocumentModel documentModel, string name, IVirtualData virtualData)
			: base(documentModel, name) {
			this.data = virtualData;
			this.rows = new VirtualRowCollection(this, data.Rows);
		}
		public override IRowCollection Rows { get { return rows; } }
		public IVirtualData Data { get { return data; } }
		public override bool ReadOnly { get { return true; } }
		public override bool IsDataSheet { get { return true; } }
		public override IEnumerable<ICellBase> GetExistingCells() {
			return new Enumerable<ICellBase>(new EnumeratorConverter<IVirtualCell, ICellBase>(data.GetZOrderEnumerator(), ConvertVirtualCellToCellBase));
		}
		ICell ConvertVirtualCellToCellBase(IVirtualCell virtualCell) {
			if (virtualCell == null)
				return null;
			CellPosition position = new CellPosition(virtualCell.ColumnIndex, virtualCell.RowIndex);
			return new FakeCellWithValue(position, this, virtualCell.Value.ModelVariantValue);
		}
		public override ICell TryGetCell(int column, int row) {
			IVirtualCell virtualCell = data.GetCell(column, row);
			if (virtualCell == null)
				return null;
			return ConvertVirtualCellToCellBase(virtualCell);
		}
	}
	#endregion
	#region VirtualRowCollection
	public class VirtualRowCollection : RowCollection {
		readonly DynamicListWrapper<IVirtualRow, Row> innerRows;
		public VirtualRowCollection(VirtualWorksheet sheet, IVirtualRowCollection virtualCollection)
			: base(sheet) {
			innerRows = new DynamicListWrapper<IVirtualRow, Row>(virtualCollection, ConvertVirtualRowToRow);
		}
		#region Properties
		public new VirtualWorksheet Sheet { get { return (VirtualWorksheet)base.Sheet; } }
		#endregion
		Row ConvertVirtualRowToRow(IVirtualRow virtualRow) {
			if (virtualRow == null)
				return null;
			return new VirtualRow(virtualRow.Index, Sheet, virtualRow);
		}
		public override IList<Row> InnerList { get { return innerRows; } }
	}
	#endregion
	#region VirtualRow
	public class VirtualRow : Row {
		readonly VirtualCellCollection cells;
		public VirtualRow(int index, Worksheet sheet, IVirtualRow innerRow)
			: base(index, sheet) {
			this.cells = new VirtualCellCollection(this, innerRow.Cells);
		}
		public override ICellCollection Cells { get { return cells; } }
	}
	#endregion
	#region VirtualCellCollection
	public class VirtualCellCollection : CellCollection {
		readonly DynamicListWrapper<IVirtualCell, ICell> innerCells;
		public VirtualCellCollection(IRow owner, IVirtualCellCollection virtualCollection)
			: base(owner) {
			innerCells = new DynamicListWrapper<IVirtualCell, ICell>(virtualCollection, ConvertVirtualCellToCellBase);
		}
		#region Properties
		protected internal VirtualWorksheet Sheet { get { return (VirtualWorksheet)Owner.Sheet; } }
		#endregion
		ICell ConvertVirtualCellToCellBase(IVirtualCell virtualCell) {
			if (virtualCell == null)
				return null;
			CellPosition position = new CellPosition(virtualCell.ColumnIndex, virtualCell.RowIndex);
			return new FakeCellWithValue(position, Sheet, virtualCell.Value.ModelVariantValue);
		}
		public override IList<ICell> InnerList { get { return innerCells; } }
	}
	#endregion
	#endregion
#endif
}
