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
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Commands.Internal;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region EditingFillUpCommand
	public class EditingFillUpCommand : EditingFillCommandBase {
		public EditingFillUpCommand(ISpreadsheetControl control)
			: base(control) {
		}
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_EditingFillUp; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_EditingFillUpDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.EditingFillUp; } }
		public override string ImageName { get { return "EditingFillUp"; } }
		protected override RangeFillManager CreateRangeFillManager() {
			return new RangeFillUpManager();
		}
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	public class RangeFillUpManager : RangeFillManager {
		protected override CellRange CalculateSourceRange(CellRange range) {
			int offset;
			if (range.Height <= 1) {
				if (range.BottomRight.Row + 1 >= (range.Worksheet as Worksheet).MaxRowCount)
					return null;
				offset = 1;
			}
			else
				offset = 0;
			CellPosition bottomRight = range.BottomRight;
			return new CellRange(range.Worksheet, new CellPosition(range.TopLeft.Column, bottomRight.Row + offset), new CellPosition(bottomRight.Column, bottomRight.Row + offset));
		}
		protected override Row CalculateSourceRow(CellRange range) {
			Worksheet sheet = (Worksheet)range.Worksheet;
			if (range.Width != sheet.MaxColumnCount)
				return null;
			Row row = (Row)sheet.Rows.TryGetRow(range.BottomRight.Row);
			if (row == null)
				return new Row(range.BottomRight.Row, sheet);
			else
				return row;
		}
		protected override Column CalculateSourceColumn(CellRange range) {
			return null;
		}
		protected override CellRange CalculateVectorTargetRange(ICell sourceCell, CellRange range) {
			return new CellRange(range.Worksheet, new CellPosition(sourceCell.ColumnIndex, range.TopLeft.Row), new CellPosition(sourceCell.ColumnIndex, range.BottomRight.Row));
		}
	}
}
