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
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Utils;
using DevExpress.Office.Utils;
namespace DevExpress.XtraSpreadsheet.Layout.Engine {
	public class RowAutoFitCalculator {
		readonly Worksheet sheet;
		readonly IColumnWidthCalculationService service;
		public RowAutoFitCalculator(Worksheet sheet) {
			Guard.ArgumentNotNull(sheet, "sheet");
			this.sheet = sheet;
			this.service = DocumentModel.GetService<IColumnWidthCalculationService>();
			if (service == null)
				Exceptions.ThrowInternalException();
		}
		Worksheet Sheet { get { return sheet; } }
		DocumentModel DocumentModel { get { return sheet.Workbook; } }
		public void TryBestFitRow(CellRangeBase range) {
			DocumentModel.BeginUpdate();
			try {
				TryBestFitRowCore(range);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		void TryBestFitRowCore(CellRangeBase range) {
			if (range.RangeType == CellRangeType.UnionRange) {
				CellUnion union = (CellUnion)range;
				foreach (CellRangeBase innerRange in union.InnerCellRanges)
					TryBestFitRowCore(innerRange);
			}
			else {
				int leftColumn = range.TopLeft.Column;
				int rightColumn = range.BottomRight.Column;
				foreach (Row row in Sheet.Rows.GetExistingRows(range.TopLeft.Row, range.BottomRight.Row, false))
					TryBestFitRowCore(row.Index, leftColumn, rightColumn);
			}
		}
		public void TryBestFitRow(int rowIndex) {
			TryBestFitRowCore(rowIndex, 0, this.sheet.MaxColumnCount - 1);
		}
		void TryBestFitRowCore(int rowIndex, int leftColumn, int rightColumn) {
			Row row = Sheet.Rows.TryGetRow(rowIndex);
			if (row == null)
				return;
			IColumnWidthCalculationService service = DocumentModel.GetService<IColumnWidthCalculationService>();
			if (service == null)
				return;
			int maxCellHeight = service.CalculateRowMaxCellHeight(row, DocumentModel.MaxDigitWidth, DocumentModel.MaxDigitWidthInPixels, leftColumn, rightColumn);
			if (maxCellHeight <= 0)
				return;
			float cellHeightInModelUnits = DocumentModel.ToDocumentLayoutUnitConverter.ToModelUnits((float)maxCellHeight);
			float maxCellHeightInModelUnits = DocumentModel.UnitConverter.TwipsToModelUnits(Row.MaxHeightInTwips);
			row.SetCustomHeight(Math.Min(cellHeightInModelUnits, maxCellHeightInModelUnits));
		}
	}
}
