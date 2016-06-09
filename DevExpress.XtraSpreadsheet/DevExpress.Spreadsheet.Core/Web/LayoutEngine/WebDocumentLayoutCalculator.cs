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
using System.Drawing;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraSpreadsheet.Layout.Engine {
	public enum PanesType {
		MainPane = 0,
		TopRightPane,
		BottomLeftPane,
		FrozenPane
	}
	public class WebDocumentLayoutCalculator : DocumentLayoutCalculator {
		WebDocumentLayout layout;
		DocumentLayoutAnchor anchor;
		protected CellPosition LastVisibleCell {
			get;
			private set;
		}
		public DocumentLayoutAnchor Anchor {
			get {
				if(anchor == null)
					anchor = new DocumentLayoutAnchor();
				return anchor;
			}
			set { anchor = value; }
		}
		public int FrozenPaneWidth { get; private set; }
		public int FrozenPaneHeight { get; private set; }
		public WebDocumentLayoutCalculator(WebDocumentLayout layout, Worksheet sheet)
			: base(layout, sheet, Rectangle.Empty, 1.0f) {
			this.layout = layout;
		}
		protected void SetCalculationFields(CellPosition anchorCell, Rectangle viewBounds) {
			ViewBounds = viewBounds;
			Anchor.CellPosition = anchorCell;
		}
		public void CalculateLayout(Dictionary<TilePosition, CellRange> tileRanges) {
			layout.HeaderOffset = HeaderOffset;
			layout.TileRanges = tileRanges;
			layout.PreliminaryPages = new Dictionary<TilePosition, PreliminaryPage>();
			layout.TilePages = new Dictionary<TilePosition, Page>();
			PreliminaryPage page = null;
			int previousTileRow = -1;
			CellRange previousRange = null;
			foreach(KeyValuePair<TilePosition, CellRange> tileRange in tileRanges) {
				page = CalculateLimitedLayout(tileRange.Value, 1f, null, null);
				layout.PreliminaryPages.Add(tileRange.Key, page);
				previousTileRow = tileRange.Key.TileRow;
				previousRange = tileRange.Value;
			}
			FinalWebDocumentLayoutCalculator calculator = new FinalWebDocumentLayoutCalculator(layout, Sheet, VerticalBorderBreaks);
			calculator.GenerateBlocks();
		}
		public CellPosition GetLastVisibleCell() {
			CellPosition lastFilledCell = Sheet.GetPrintRange().BottomRight;
			int colIndex = Math.Max(lastFilledCell.Column, LastVisibleCell.Column + 1),
				rowIndex = Math.Max(lastFilledCell.Row, LastVisibleCell.Row + 1);
			return new CellPosition(colIndex, rowIndex);
		}
		public Dictionary<PanesType, CellRange> CalculateWebBoundingRanges(Rectangle viewBounds) {
			SetCalculationFields(new CellPosition(), new Rectangle(0, 0, viewBounds.Left, viewBounds.Top));
			CellPosition anchorCell = GetAnchorByScrollPosition();
			return CalculateBoundingRanges(anchorCell, viewBounds);
		}
		public Dictionary<PanesType, CellRange> CalculateBoundingRanges(CellPosition anchorCell, Rectangle viewBounds) {
			Dictionary<PanesType, CellRange> ranges = new Dictionary<PanesType, CellRange>();
			SetCalculationFields(anchorCell, viewBounds);
			if(Sheet.ActiveView.IsFrozen()) {
				bool isColumnFrozen = !Sheet.ActiveView.IsOnlyRowsFrozen(),
					isRowFrozen = !Sheet.ActiveView.IsOnlyColumnsFrozen();
				CellRange range = null;
				int accumulatedWidth = 0,
					accumulatedHeight = 0;
				int frozenPaneMaxVisibleColumnIndex = -1,
					frozenPaneMaxVisibleRowIndex = -1;
				if(isColumnFrozen) {
					frozenPaneMaxVisibleColumnIndex = GetFrozenPaneMaxVisibleModelColumnIndex(ref accumulatedWidth);
					FrozenPaneWidth = accumulatedWidth;
				}
				if(isRowFrozen) {
					frozenPaneMaxVisibleRowIndex = GetFrozenPaneMaxVisibleModelRowIndex(ref accumulatedHeight);
					FrozenPaneHeight = accumulatedHeight;
				}
				if(isColumnFrozen && isRowFrozen) {
					range = new CellRange(Sheet, Sheet.ActiveView.TopLeftCell, new CellPosition(frozenPaneMaxVisibleColumnIndex, frozenPaneMaxVisibleRowIndex));
					ranges.Add(PanesType.FrozenPane, range);
				}
				int maxVisibleColumnIndex = GetTopLeftPaneMaxVisibleModelColumnIndex(ref accumulatedWidth);
				int maxVisibleRowIndex = GetBottomRightPaneMaxVisibleModelRowIndex(ref accumulatedHeight);
				if(isRowFrozen) {
					range = new CellRange(Sheet, new CellPosition(Anchor.CellPosition.Column, Sheet.ActiveView.TopLeftCell.Row),
													new CellPosition(maxVisibleColumnIndex, frozenPaneMaxVisibleRowIndex));
					ranges.Add(PanesType.TopRightPane, range);
				}
				if(isColumnFrozen) {
					range = new CellRange(Sheet, new CellPosition(Sheet.ActiveView.TopLeftCell.Column, Anchor.CellPosition.Row),
													new CellPosition(frozenPaneMaxVisibleColumnIndex, maxVisibleRowIndex));
					ranges.Add(PanesType.BottomLeftPane, range);
				}
				range = new CellRange(Sheet, Anchor.CellPosition, new CellPosition(maxVisibleColumnIndex, maxVisibleRowIndex));
				ranges.Add(PanesType.MainPane, range);
			} else {
				ranges.Add(PanesType.MainPane, CalculateBoundingRange(Anchor));
			}
			LastVisibleCell = ranges[PanesType.MainPane].BottomRight;
			return ranges;
		}
		protected IColumnWidthCalculationService GetCalculationService() {
			return Sheet.Workbook.GetService<IColumnWidthCalculationService>();
		}
		private int GetFrozenPaneMaxVisibleModelColumnIndex(ref int accumulatedWidth) {
			IColumnWidthCalculationService gridCalculator = GetCalculationService();
			int colIndex,
				firstFrozenColumn = this.Sheet.ActiveView.FrozenCell.Column;
			for(colIndex = Sheet.ActiveView.TopLeftCell.Column; colIndex < firstFrozenColumn; colIndex++) {
				accumulatedWidth += gridCalculator.CalculateColumnWidthTmp(Sheet, colIndex);
				if(!IsColumnFitToViewBounds(colIndex, accumulatedWidth))
					return colIndex;
			}
			return colIndex - 1;
		}
		private int GetFrozenPaneMaxVisibleModelRowIndex(ref int accumulatedHeight) {
			IColumnWidthCalculationService gridCalculator = GetCalculationService();
			int rowIndex,
				firstFrozenRow = this.Sheet.ActiveView.FrozenCell.Row;
			for(rowIndex = Sheet.ActiveView.TopLeftCell.Row; rowIndex < firstFrozenRow; rowIndex++) {
				accumulatedHeight += gridCalculator.CalculateRowHeight(Sheet, rowIndex);
				if(!IsRowFitToViewBounds(rowIndex, accumulatedHeight))
					return rowIndex;
			}
			return rowIndex - 1;
		}
		private int GetTopLeftPaneMaxVisibleModelColumnIndex(ref int accumulatedWidth) {
			IColumnWidthCalculationService gridCalculator = GetCalculationService();
			accumulatedWidth += ViewBounds.Left;
			int columnIndex = Anchor.CellPosition.Column;
			while(IsColumnFitToViewBounds(columnIndex, accumulatedWidth)) {
				accumulatedWidth += gridCalculator.CalculateColumnWidthTmp(Sheet, columnIndex);
				columnIndex++;
			}
			return columnIndex - 1;
		}
		private int GetBottomRightPaneMaxVisibleModelRowIndex(ref int accumulatedHeight) {
			IColumnWidthCalculationService gridCalculator = GetCalculationService();
			accumulatedHeight += ViewBounds.Top;
			int rowIndex = Anchor.CellPosition.Row;
			while(IsRowFitToViewBounds(rowIndex, accumulatedHeight)) {
				accumulatedHeight += gridCalculator.CalculateRowHeight(Sheet, rowIndex);
				rowIndex++;
			}
			return rowIndex - 1;
		}
		protected CellPosition GetAnchorByScrollPosition() {
			IColumnWidthCalculationService gridCalculator = this.Sheet.Workbook.GetService<IColumnWidthCalculationService>();
			CellPosition frozenCell = Sheet.ActiveView.FrozenCell;
			int currentExtent = 0,
				row = frozenCell.Row,
				column = frozenCell.Column;
			while(IsRowFitToViewBounds(row, currentExtent)) {
				currentExtent += gridCalculator.CalculateRowHeight(Sheet, row);
				row++;
			}
			currentExtent = 0;
			while(IsColumnFitToViewBounds(column, currentExtent)) {
				currentExtent += gridCalculator.CalculateColumnWidthTmp(Sheet, column);
				column++;
			}
			row = Math.Max(row - 1, 0);
			column = Math.Max(column - 1, 0);
			return new CellPosition(column, row);
		}
	}
}
