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
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Utils;
using ModelUnit = System.Int32;
using LayoutUnit = System.Int32;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.Office;
using DevExpress.Office.Utils;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Tables.Native {
	#region TableGrid
	public class TableGrid {
		readonly TableGridColumnCollection columnsCollection;
		public TableGrid() {
			this.columnsCollection = new TableGridColumnCollection();
		}
		public TableGrid(List<TableGridInterval> intervals) {
			this.columnsCollection = new TableGridColumnCollection();
			int count = intervals.Count;
			for (int i = 0; i < count; i++)
				columnsCollection.Add(new TableGridColumn(intervals[i].Width, intervals[i].IntervalType == TableGridIntervalType.PercentBased));
		}
		public TableGridColumn this[int index] { get { return columnsCollection[index]; } }
		public TableGridColumnCollection Columns { get { return columnsCollection; } }
		public TableGrid Copy() {
			TableGrid result = new TableGrid();
			int colCount = Columns.Count;
			for (int i = 0; i < colCount; i++)
				result.Columns.Add(Columns[i]);
			return result;
		}
	}
	#endregion
	#region TableGridColumn
	public class TableGridColumn {
		LayoutUnit width;
		LayoutUnit minWidth;
		LayoutUnit maxWidth;
		LayoutUnit preferredWidth;
		LayoutUnit totalHorizontalMargins;
		bool percentBased;
		public TableGridColumn() {
		}
		public TableGridColumn(LayoutUnit width, bool percentBased) {
			Guard.ArgumentNonNegative(width, "width");
			this.width = width;
			this.percentBased = percentBased;
		}
		public LayoutUnit Width {
			get { return width; }
			set {
				Guard.ArgumentNonNegative(value, "value");
				width = value;
			}
		}
		public LayoutUnit MinWidth {
			get { return minWidth; }
			set {
				Guard.ArgumentNonNegative(value, "value");
				minWidth = value;
			}
		}
		public LayoutUnit TotalHorizontalMargins {
			get { return totalHorizontalMargins; }
			set {
				Guard.ArgumentNonNegative(value, "value");
				totalHorizontalMargins = value;
			}
		}
		public LayoutUnit MaxWidth {
			get { return maxWidth; }
			set {
				Guard.ArgumentNonNegative(value, "value");
				maxWidth = value;
			}
		}
		public LayoutUnit PreferredWidth {
			get { return preferredWidth; }
			set {
				Guard.ArgumentNonNegative(value, "value");
				this.preferredWidth = value;
			}
		}
		public bool PercentBased {
			get { return percentBased; }
			set { percentBased = value; }
		}
	}
	#endregion
	#region TableGridColumnCollection
	public class TableGridColumnCollection : List<TableGridColumn> {
	}
	#endregion
	#region WidthsContentInfo
	public struct WidthsContentInfo {
		public static readonly WidthsContentInfo Empty = new WidthsContentInfo(0, 0);
		int minWidth;
		int maxWidth;
		public WidthsContentInfo(int minWidth, int maxWidth) {
			this.minWidth = minWidth;
			this.maxWidth = maxWidth;
		}
		public int MinWidth { get { return minWidth; } }
		public int MaxWidth { get { return maxWidth; } }
		public static WidthsContentInfo Max(WidthsContentInfo val1, WidthsContentInfo val2) {
			return new WidthsContentInfo(Math.Max(val1.MinWidth, val2.MinWidth), Math.Max(val1.MaxWidth, val2.MaxWidth));
		}
		public override bool Equals(object obj) {
			return obj is WidthsContentInfo && (WidthsContentInfo)obj == this;
		}
		public override LayoutUnit GetHashCode() {
			return MinWidth & MaxWidth;
		}
		public static bool operator ==(WidthsContentInfo info1, WidthsContentInfo info2) {
			return info1.MinWidth == info2.MinWidth && info1.MaxWidth == info2.MaxWidth;
		}
		public static bool operator !=(WidthsContentInfo info1, WidthsContentInfo info2) {
			return info1.MinWidth != info2.MinWidth || info1.MaxWidth != info2.MaxWidth;
		}
	}
	#endregion
	#region WidthsInfo
	public struct WidthsInfo {
		public static readonly WidthsInfo Empty = new WidthsInfo(0, 0, 0);
		int minWidth;
		int maxWidth;
		int totalHorizontalMargins;
		public WidthsInfo(int minWidth, int maxWidth, int totalHorizontalMargins) {
			this.minWidth = minWidth;
			this.maxWidth = maxWidth;
			this.totalHorizontalMargins = totalHorizontalMargins;
		}
		public int MinWidth { get { return minWidth; } }
		public int MaxWidth { get { return maxWidth; } }
		public int TotalHorizontalMargins { get { return totalHorizontalMargins; } }
	}
	#endregion
	#region TableWidthsCalculatorBase (abstract class)
	public abstract class TableWidthsCalculatorBase {
		protected struct CellPosition {
			readonly int firstColumnIndex;
			readonly int columnSpan;
			public CellPosition(int firstColumnIndex, int columnSpan) {
				this.firstColumnIndex = firstColumnIndex;
				this.columnSpan = columnSpan;
			}
			public int FirstColumnIndex { get { return firstColumnIndex; } }
			public int ColumnSpan { get { return columnSpan; } }
			public int EndColumnIndex { get { return firstColumnIndex + columnSpan - 1; } }
			public override int GetHashCode() {
				return firstColumnIndex + (columnSpan << 16);
			}
			public override bool Equals(object obj) {
				if (!(obj is CellPosition))
					return false;
				CellPosition other = (CellPosition)obj;				
				return firstColumnIndex == other.firstColumnIndex && columnSpan == other.columnSpan;
			}
		}
		protected class ColumnWidthInfoDictionary {
			Dictionary<CellPosition, WidthsContentInfo> innerDictionary;
			bool containsCellWithSpan;
			public ColumnWidthInfoDictionary () {
				this.innerDictionary = new Dictionary<CellPosition,WidthsContentInfo>();
			}
			public void Register(int firstColumnIndex, int columnSpan, WidthsInfo cellWidths) {
				CellPosition position = new CellPosition(firstColumnIndex, columnSpan);
				WidthsContentInfo existingInfo;
				if (innerDictionary.TryGetValue(position, out existingInfo)) {
					WidthsContentInfo conentWidths = new WidthsContentInfo(Math.Max(cellWidths.MinWidth, existingInfo.MinWidth), Math.Max(cellWidths.MaxWidth, existingInfo.MaxWidth));
					innerDictionary[position] = conentWidths;
				}
				else
					innerDictionary.Add(position, new WidthsContentInfo(cellWidths.MinWidth, cellWidths.MaxWidth));
				containsCellWithSpan |= columnSpan > 1;
			}
			public WidthsContentInfo CalculateTotalContentWidth() {
				List<KeyValuePair<CellPosition, WidthsContentInfo>> positions = new List<KeyValuePair<CellPosition, WidthsContentInfo>>(this.innerDictionary);
				positions.Sort((pair1, pair2) => CellPositionComparer(pair1.Key, pair2.Key));
				int maxColumnIndex = positions[positions.Count - 1].Key.EndColumnIndex;
				WidthsContentInfo[] summaryWidths = new WidthsContentInfo[maxColumnIndex + 2];
				foreach (var pair in positions) {
					int prevColumnIndex = pair.Key.FirstColumnIndex;
					int endColumnIndex = pair.Key.EndColumnIndex + 1;
					WidthsContentInfo contentInfo = pair.Value;
					WidthsContentInfo existingStartSummaryInfo = summaryWidths[prevColumnIndex];
					WidthsContentInfo newSumaryInfo = new WidthsContentInfo(existingStartSummaryInfo.MinWidth + contentInfo.MinWidth, existingStartSummaryInfo.MaxWidth + contentInfo.MaxWidth);
					int lastIndex =containsCellWithSpan ? summaryWidths.Length - 1: endColumnIndex;
					for (int i = endColumnIndex; i <= lastIndex; i++) {
						WidthsContentInfo existingEndSummaryInfo = summaryWidths[i];
						if (newSumaryInfo.MinWidth <= existingEndSummaryInfo.MinWidth && newSumaryInfo.MaxWidth <= existingEndSummaryInfo.MaxWidth)
							break;
						summaryWidths[i] = new WidthsContentInfo(Math.Max(existingEndSummaryInfo.MinWidth, newSumaryInfo.MinWidth), Math.Max(existingEndSummaryInfo.MaxWidth, newSumaryInfo.MaxWidth));
					}
				}
				return summaryWidths[summaryWidths.Length - 1];
			}
			int CellPositionComparer(CellPosition position1, CellPosition position2) {
				int result = position1.EndColumnIndex - position2.EndColumnIndex;
				if (result == 0)
					return position1.ColumnSpan - position2.ColumnSpan;
				else
					return result;
			}
		}
		readonly DocumentModelUnitToLayoutUnitConverter converter;
		readonly int percentBaseWidth;
		protected TableWidthsCalculatorBase(DocumentModelUnitToLayoutUnitConverter converter, int percentBaseWidth) {
			Guard.ArgumentNotNull(converter, "converter");
			this.converter = converter;
			this.percentBaseWidth = percentBaseWidth;
		}
		protected internal virtual DocumentModelUnitToLayoutUnitConverter Converter { get { return converter; } }
		protected virtual int PercentBaseWidth { get { return percentBaseWidth; } }
		protected WidthsContentInfo CalculateTableWidths(Table table, int percentBaseWidth, bool simpleView) {
			ColumnWidthInfoDictionary columnWidths = new ColumnWidthInfoDictionary();
			TableRowCollection rows = table.Rows;
			int rowCount = rows.Count;
			for (int i = 0; i < rowCount; i++) {
				CalculateRowWidths(columnWidths, rows[i], percentBaseWidth, simpleView);
			}
			WidthsContentInfo result = columnWidths.CalculateTotalContentWidth();
			if (table.PreferredWidth.Type != WidthUnitType.Nil && table.PreferredWidth.Type != WidthUnitType.Auto) {
				LayoutUnit tableWidth = GetActualWidth(table.PreferredWidth, percentBaseWidth);
				result = new WidthsContentInfo(Math.Max(result.MinWidth, tableWidth), Math.Max(result.MinWidth, tableWidth));
			}
			return result;
		}
		void CalculateRowWidths(ColumnWidthInfoDictionary columnWidths, TableRow row, int percentBaseWidth, bool simpleView) {
			TableCellCollection cells = row.Cells;
			WidthsContentInfo result = new WidthsContentInfo(0, 0);
			int cellCount = cells.Count;
			int columnIndex = row.GridBefore;
			WidthsContentInfo prevValue = new WidthsContentInfo(0, 0);
			for (int i = 0; i < cellCount; i++) {
				TableCell cell = cells[i];
				WidthsInfo cellWidths = CalculateCellWidths(cell, percentBaseWidth, simpleView);
				columnWidths.Register(columnIndex, cell.ColumnSpan, cellWidths);
				columnIndex += cell.ColumnSpan;
			}
		}
		public WidthsInfo CalculateCellWidths(TableCell cell, int percentBaseWidth, bool simpleView) {
			if (cell.VerticalMerging == MergingState.Continue)
				cell = cell.Table.GetFirstCellInVerticalMergingGroup(cell);
			PreferredWidth cellPreferredWidth = cell.PreferredWidth;
			LayoutUnit preferredWidth = GetActualWidth(cellPreferredWidth, percentBaseWidth);
			ModelUnit horizontalMargins = cell.GetActualLeftMargin().Value + cell.GetActualRightMargin().Value;
			BorderInfo leftBorder = GetLeftBorder(cell);
			BorderInfo rightBorder = GetRightBorder(cell);
			TableBorderCalculator calculator = new TableBorderCalculator();
			ModelUnit bordersWidth = calculator.GetActualWidth(leftBorder) + calculator.GetActualWidth(rightBorder);
			ModelUnit spacing = cell.Row.CellSpacing.Value;
			if (spacing > 0) {
				if ((cell.Row.GridBefore == 0 && cell.IsFirstCellInRow) || (cell.Row.GridAfter == 0 && cell.IsLastCellInRow))
					spacing = spacing * 3;
				else
					spacing = spacing * 2;
			}
			WidthsContentInfo contentWidths = new WidthsContentInfo(0, 0);
			if (cell.Table.TableLayout == TableLayoutType.Autofit || simpleView) {
				contentWidths = CalculateCellContentWidthsCore(cell, percentBaseWidth, simpleView);
				if (cell.NoWrap) {
					int maxWidth = Math.Max(contentWidths.MinWidth, contentWidths.MaxWidth);
					contentWidths = new WidthsContentInfo(maxWidth, maxWidth);
				}
				LayoutUnit resultMinWidth = GetMinWidth(contentWidths, horizontalMargins, bordersWidth, spacing);
				resultMinWidth = Math.Min(Int16.MaxValue, resultMinWidth);
				WidthUnit unit = cell.PreferredWidth;
				LayoutUnit resultMaxWidth = GetMaxWidth(contentWidths, horizontalMargins, bordersWidth, spacing);
				resultMaxWidth = Math.Min(Int16.MaxValue, resultMaxWidth);
				cell.LayoutProperties.ContentWidthsInfo = new WidthsContentInfo(resultMinWidth, resultMaxWidth);
				if (unit.Type == WidthUnitType.ModelUnits || unit.Type == WidthUnitType.FiftiethsOfPercent) {
					resultMaxWidth = Math.Max(resultMinWidth, preferredWidth);
				}
				WidthsContentInfo widthsInfoResult = new WidthsContentInfo(resultMinWidth, resultMaxWidth);
				cell.LayoutProperties.ContainerWidthsInfo = widthsInfoResult;
				return new WidthsInfo(widthsInfoResult.MinWidth, widthsInfoResult.MaxWidth, converter.ToLayoutUnits(horizontalMargins));
			}
			else {
				LayoutUnit outerWidth = converter.ToLayoutUnits(bordersWidth + spacing + horizontalMargins);
				LayoutUnit result = Math.Max(outerWidth, preferredWidth);
				cell.LayoutProperties.ContainerWidthsInfo = WidthsContentInfo.Empty;
				return new WidthsInfo(result, result, converter.ToLayoutUnits(horizontalMargins));
			}
		}
		protected virtual LayoutUnit GetMaxWidth(WidthsContentInfo contentWidths, ModelUnit horizontalMargins, ModelUnit bordersWidth, ModelUnit spacing) {
			return contentWidths.MaxWidth + converter.ToLayoutUnits(horizontalMargins + bordersWidth + spacing);
		}
		protected virtual LayoutUnit GetMinWidth(WidthsContentInfo contentWidths, ModelUnit horizontalMargins, ModelUnit bordersWidth, ModelUnit spacing) {
			return contentWidths.MinWidth + converter.ToLayoutUnits(horizontalMargins + bordersWidth + spacing);
		}
		BorderInfo GetLeftBorder(TableCell cell) {
			TableBorderCalculator calculator = new TableBorderCalculator();
			BorderInfo leftCellBorder = !cell.IsFirstCellInRow ? cell.Previous.GetActualRightCellBorder().Info : null;
			BorderInfo rightCellBorder = cell.GetActualLeftCellBorder().Info;
			return calculator.GetVerticalBorderSource(cell.Table, leftCellBorder, rightCellBorder);
		}
		BorderInfo GetRightBorder(TableCell cell) {
			TableBorderCalculator calculator = new TableBorderCalculator();
			BorderInfo leftCellBorder = cell.GetActualRightCellBorder().Info;
			BorderInfo rightCellBorder = !cell.IsLastCellInRow ? cell.Next.GetActualLeftCellBorder().Info : null;
			return calculator.GetVerticalBorderSource(cell.Table, leftCellBorder, rightCellBorder);
		}
		protected LayoutUnit GetActualWidth(WidthUnit unit, int percentBaseWidth) {
			if (unit.Type == WidthUnitType.ModelUnits)
				return Converter.ToLayoutUnits(unit.Value);
			if (unit.Type == WidthUnitType.FiftiethsOfPercent)
				return unit.Value * percentBaseWidth / 5000;
			return 0;
		}
		protected virtual WidthsContentInfo CalculateCellContentWidthsCore(TableCell cell, int percentBaseWidth, bool simpleView) {
			ParagraphIndex startParagraphIndex = cell.StartParagraphIndex;
			ParagraphIndex endParagraphIndex = cell.EndParagraphIndex;
			PieceTable pieceTable = cell.PieceTable;
			ParagraphCollection paragraphs = pieceTable.Paragraphs;
			WidthsContentInfo result = new WidthsContentInfo(0, 0);
			ParagraphIndex paragraphIndex = startParagraphIndex;
			while (paragraphIndex <= endParagraphIndex) {
				Paragraph paragraph = paragraphs[paragraphIndex];
				TableCell paragraphCell = paragraph.GetCell();
				if (paragraphCell == cell) {
					WidthsContentInfo paragraphWidths = CalculateParagraphWidths(paragraph);
					result = WidthsContentInfo.Max(paragraphWidths, result);
					paragraphIndex++;
				}
				else {
					Table innerTable = paragraphCell.Table;
					while (innerTable.NestedLevel > cell.Table.NestedLevel + 1)
						innerTable = innerTable.ParentCell.Table;
					WidthsContentInfo tableWidths = CalculateTableWidths(innerTable, 0, simpleView);
					result = WidthsContentInfo.Max(tableWidths, result);
					paragraphIndex = innerTable.Rows.Last.Cells.Last.EndParagraphIndex + 1;
				}
			}
			return result;
		}
		protected virtual WidthsContentInfo CalculateParagraphWidths(Paragraph paragraph) {
			ParagraphBoxCollection boxes = paragraph.BoxCollection;
			LayoutUnit maxWidth = 0;
			LayoutUnit minWidth = 0;
			LayoutUnit lineWidth = 0;
			LayoutUnit maxFloatingObjectWidth = 0;
			PieceTable pieceTable = paragraph.PieceTable;
			int wordWidth = 0;
			for (int i = 0; i < boxes.Count; i++) {
				Box box = boxes[i];
				if (box.IsLineBreak) {
					maxWidth = Math.Max(lineWidth, maxWidth);
					lineWidth = 0;
				}
				else {
					FloatingObjectAnchorBox anchorBox = box as FloatingObjectAnchorBox;
					if (anchorBox != null) {
						FloatingObjectAnchorRun anchorRun = anchorBox.GetFloatingObjectRun(pieceTable);
						if (anchorRun.FloatingObjectProperties.TextWrapType != FloatingObjectTextWrapType.None) {
							FloatingObjectSizeController controller = new FloatingObjectSizeController(pieceTable);
							controller.UpdateFloatingObjectBox(anchorBox);
							int width = anchorBox.ShapeBounds.Width;
							maxFloatingObjectWidth = Math.Max(width, maxFloatingObjectWidth);
						}
					}
					lineWidth += box.Bounds.Width;
				}
				if (box.IsLineBreak || !box.IsNotWhiteSpaceBox) {
					minWidth = Math.Max(wordWidth, minWidth);
					wordWidth = 0;
				}
				else
					wordWidth += box.Bounds.Width;
			}
			return new WidthsContentInfo(Math.Max(maxFloatingObjectWidth, minWidth), Math.Max(maxFloatingObjectWidth, maxWidth));
		}
		public abstract bool CanUseCachedTableLayoutInfo(TableLayoutInfo tableLayoutInfo);
		public abstract TableLayoutInfo CreateTableLayoutInfo(TableGrid tableGrid, LayoutUnit maxTableWidth, bool allowTablesToExtendIntoMargins, bool simpleView, LayoutUnit percentBaseWidth);
	}
	#endregion
	#region TableWidthsCalculator
	public class TableWidthsCalculator : TableWidthsCalculatorBase {
		PieceTable pieceTable;
		BoxMeasurer measurer;
		public TableWidthsCalculator(PieceTable pieceTable, BoxMeasurer measurer, int percentBaseWidth)
			: base(pieceTable.DocumentModel.ToDocumentLayoutUnitConverter, percentBaseWidth) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			Guard.ArgumentNotNull(measurer, "measurer");
			this.pieceTable = pieceTable;
			this.measurer = measurer;
		}
		protected override WidthsContentInfo CalculateParagraphWidths(Paragraph paragraph) {
			EnsureParagraphBoxes(paragraph);
			return base.CalculateParagraphWidths(paragraph);
		}
		void EnsureParagraphBoxes(Paragraph paragraph) {
			if (!paragraph.BoxCollection.IsValid) {
				paragraph.BoxCollection.Clear();
				IVisibleTextFilter visibleTextFilter = pieceTable.VisibleTextFilter;
				ParagraphCharacterIterator characterIterator = new ParagraphCharacterIterator(paragraph, pieceTable, visibleTextFilter);
				if (characterIterator.RunIndex <= paragraph.LastRunIndex) {
					ParagraphCharacterFormatter preFormatter = new ParagraphCharacterFormatter(pieceTable, measurer);
					preFormatter.Format(characterIterator);
				}
			}
			paragraph.BoxCollection.ParagraphStartRunIndex = paragraph.FirstRunIndex;
		}
		public override bool CanUseCachedTableLayoutInfo(TableLayoutInfo tableLayoutInfo) {
			return true;
		}
		public override TableLayoutInfo CreateTableLayoutInfo(TableGrid tableGrid, LayoutUnit maxTableWidth, bool allowTablesToExtendIntoMargins, bool simpleView, LayoutUnit percentBaseWidth) {
			return new TableLayoutInfo(tableGrid, maxTableWidth, allowTablesToExtendIntoMargins, simpleView, percentBaseWidth);
		}
	}
	#endregion
	#region TableGridCalculator
	public class TableGridCalculator {
		DocumentModelUnitToLayoutUnitConverter converter;
		int maxTableWidth;
		TableWidthsCalculatorBase tableWidthsCalculator;
		bool allowTablesToExtendIntoMargins;
		bool simpleView;
		bool flex;
		public TableGridCalculator(DocumentModel documentModel, TableWidthsCalculatorBase tableWidthsCalculator, int maxTableWidth)
			: this(documentModel, tableWidthsCalculator, maxTableWidth, false, false) {
		}
		public TableGridCalculator(DocumentModel documentModel, TableWidthsCalculatorBase tableWidthsCalculator, int maxTableWidth, bool allowTablesToExtendIntoMargins, bool simpleView) {
			this.tableWidthsCalculator = tableWidthsCalculator;
			this.maxTableWidth = maxTableWidth;
			this.simpleView = simpleView;
			if (tableWidthsCalculator != null)
				this.converter = tableWidthsCalculator.Converter; 
			this.allowTablesToExtendIntoMargins = allowTablesToExtendIntoMargins;
		}
		public bool AllowTablesToExtendIntoMargins { get { return allowTablesToExtendIntoMargins; } }
		public bool SimpleView { get { return simpleView; } }
		public TableGrid CalculateTableGrid(Table table, int percentBaseWidth) {
			TableLayoutInfo cachedTableLayoutInfo = table.CachedTableLayoutInfo;
			if (CanUseCachedTableLayoutInfo(cachedTableLayoutInfo, percentBaseWidth))
				return cachedTableLayoutInfo.TableGrid;
			List<TableGridInterval> gridIntervals = CalculateGridIntervals(table);
			if (table.PreferredWidth.Type == WidthUnitType.Auto)
				percentBaseWidth = CalculateEstimatedTableWidth(table, gridIntervals, percentBaseWidth);
			ApplyPercentWidth(gridIntervals, percentBaseWidth);
			TableGrid result = new TableGrid(gridIntervals);
			ApplyCellContentWidth(result, table, percentBaseWidth);
			AutofitTableLayoutCalculator autoFitCalculator = new AutofitTableLayoutCalculator();
			AutofitTable(result, table, percentBaseWidth);
			if (table.PreferredWidth.Type == WidthUnitType.ModelUnits && table.PreferredWidth.Value > 0 && table.TableLayout == TableLayoutType.Fixed) {
				LayoutUnit newWidth = this.tableWidthsCalculator.Converter.ToLayoutUnits(table.PreferredWidth.Value);
				autoFitCalculator.CompressTableGrid(result, 0, result.Columns.Count - 1, newWidth);
			}
			int[] margins = new CompressHelper(table, result).Work();
			TableGridColumnCollection columns = result.Columns;
			int count = columns.Count;
			LayoutUnit totalTableWidth = 0;
			LayoutUnit totalDelta = 0;
			LayoutUnit availableToCompress = 0;
			for (int i = 0; i < count; i++) {
				TableGridColumn column = columns[i];
				totalTableWidth += column.Width;
				if (column.Width < margins[i]) {
					totalDelta += margins[i] - column.Width;
					column.Width = margins[i];
				}
				else
					availableToCompress += column.Width - margins[i];
			}
			if (totalDelta > 0) {
				totalDelta = Math.Min(totalDelta, availableToCompress);
				for (int i = 0; i < count && availableToCompress > 0; i++) {
					TableGridColumn column = columns[i];
					if (column.Width > margins[i]) {
						int diff = column.Width - margins[i];
						int delta = totalDelta * diff / availableToCompress;
						availableToCompress -= diff;
						totalDelta -= delta;
						column.Width -= delta;
					}
				}
			}
			ReplaceCachedTableLayoutInfoIfNeeded(table, result, percentBaseWidth);
			return result;
		}
		LayoutUnit CalculateEstimatedTableWidth(Table table, List<TableGridInterval> gridIntervals, int percentBaseWidth) {
			int columnsInModelUnitsWidth = 0;
			int totalPercentWidth = 0;
			foreach (TableGridInterval interval in gridIntervals) {
				if (interval.IntervalType == TableGridIntervalType.PercentBased)
					totalPercentWidth += interval.Width;
				else
					columnsInModelUnitsWidth += interval.Width;
			}
			int estimatedTableWidth = 0;
			if (totalPercentWidth == 0 || columnsInModelUnitsWidth > 0) {
				if (totalPercentWidth > 0) {
					int restOfWidthInPercent = 5000 - totalPercentWidth;
					if (restOfWidthInPercent <= 0)
						return percentBaseWidth;
					estimatedTableWidth = columnsInModelUnitsWidth * 5000 / restOfWidthInPercent;
				}
				else
					estimatedTableWidth = columnsInModelUnitsWidth;
				return Math.Min(estimatedTableWidth, percentBaseWidth);
			}
			else {
				TableRowCollection rows = table.Rows;
				int rowCount = rows.Count;
				for (int rowIndex = 0; rowIndex < rowCount; rowIndex++) {
					TableRow row = rows[rowIndex];
					TableCellCollection cells = row.Cells;
					int columnIndex = row.GridBefore;
					int cellCount = cells.Count;
					for (int cellIndex = 0; cellIndex < cellCount; cellIndex++) {
						TableCell cell = cells[cellIndex];
						TableGridInterval interval = gridIntervals[columnIndex];
						if (interval.IntervalType == TableGridIntervalType.PercentBased) {
							WidthsInfo info = CalculateCellWidthsInfo(cell, percentBaseWidth);
							estimatedTableWidth = Math.Max(estimatedTableWidth, info.MaxWidth);
							if (interval.Width > 0)
								estimatedTableWidth = Math.Max(info.MinWidth * 5000 / interval.Width, estimatedTableWidth);
						}
						columnIndex += cell.ColumnSpan;
					}
				}
				return Math.Min(estimatedTableWidth, percentBaseWidth) * 5000 / totalPercentWidth;
			}
		}
		class CompressHelper {
			Table table;
			TableGrid result;
			int colsCount;
			int rowsCount;
			bool?[][] m;	
			int[][] spans;  
			public CompressHelper(Table table, TableGrid result) {
				this.table = table;
				this.result = result;
				this.colsCount = result.Columns.Count;
				this.rowsCount = table.Rows.Count;
				InitMatrices();
			}
			void InitMatrices() {
				spans = new int[colsCount][];
				for (int i = 0; i < colsCount; i++) {
					spans[i] = new int[rowsCount];
				}
				for (int i = 0; i < rowsCount; i++) {
					TableRow row = table.Rows[i];
					int p = 0;
					row.Cells.ForEach(cell => {
						int colSpan = cell.ColumnSpan;
						for (int j = colSpan; j > 0; j--) {
							spans[p++][i] = j;
						}
					});
				}
				m = new bool?[colsCount][];
				for (int i = 0; i < colsCount; i++) {
					m[i] = new bool?[rowsCount];
				}
			}
			public int[] Work() {
				for (int i = 0; i < colsCount; i++) {
					for (int j = 0; j < rowsCount; j++) {
						if (spans[i][j] == 1 && (i == 0 || spans[i - 1][j] == 1)) {
							MarkColumn(i);
							break;
						}
					}
				}
				for (int i = 0; i < colsCount; i++) {
					for (int j = 0; j < rowsCount; j++) {
						if (m[i][j] == null) {
							MarkColumn(i);
							break;
						}
					}
				}
				int[] ret = new int[colsCount];
				for (int i = 0; i < colsCount; i++)
					ret[i] = (bool)m[i][0] ? 0 : result.Columns[i].TotalHorizontalMargins;
				return ret;
			}
			void MarkColumn(int col) {
				for (int i = 0; i < rowsCount; i++) {
					m[col][i] = true;
					for (int j = 1; (col - j >= 0) && (spans[col - j][i] == spans[col][i] + j); j++)
						if (m[col][i] == null)
							m[col][i] = false;
					for (int j = 1; (col + j < colsCount) && (spans[col + j][i] == spans[col][i] - j); j++)
						if (m[col][i] == null)
							m[col][i] = false;
				}
			}
		}
		protected virtual bool CanUseCachedTableLayoutInfo(TableLayoutInfo tableLayoutInfo, LayoutUnit percentBaseWidth) {
			if (tableLayoutInfo == null)
				return false;
			return tableWidthsCalculator.CanUseCachedTableLayoutInfo(tableLayoutInfo) && tableLayoutInfo.CanUseTableGrid(maxTableWidth, allowTablesToExtendIntoMargins, simpleView, percentBaseWidth);
		}
		protected virtual void ReplaceCachedTableLayoutInfoIfNeeded(Table table, TableGrid tableGrid, LayoutUnit percentBaseWidth) {
			TableLayoutInfo cachedInfo = tableWidthsCalculator.CreateTableLayoutInfo(tableGrid, maxTableWidth, allowTablesToExtendIntoMargins, simpleView, percentBaseWidth);
			if (cachedInfo != null)
				table.CachedTableLayoutInfo = cachedInfo;
		}
		private void ApplyPercentWidth(List<TableGridInterval> intervals, int percentBaseWidth) {
			int totalPercentWidth = 0;
			int totalUnitWidth = 0;
			int count = intervals.Count;
			int maxPercentWidth = 5000;
			int unsetCount = 0;
			for (int i = 0; i < count; i++) {
				switch (intervals[i].IntervalType) {
					case TableGridIntervalType.PercentBased:
						totalPercentWidth += intervals[i].Width;
						if (totalPercentWidth > maxPercentWidth)
							intervals[i].Width = Math.Max(0, maxPercentWidth - totalPercentWidth + intervals[i].Width);
						break;
					case TableGridIntervalType.ModelUnit:
						totalUnitWidth += intervals[i].Width;
						break;
					case TableGridIntervalType.NotSet:
						unsetCount++;
						break;
				}
			}
			this.flex = (totalUnitWidth == 0);
			if (totalPercentWidth == 0)
				return;
			int restUnitWidth = Math.Max(0, percentBaseWidth - totalUnitWidth);
			if (restUnitWidth > 0 && unsetCount > 0) {
				int restPercentWidth = 100 * 50 - totalPercentWidth;
				if (restPercentWidth > 0) {
					for (int i = 0; i < count && unsetCount > 0; i++) {
						if (intervals[i].IntervalType != TableGridIntervalType.NotSet)
							continue;
						intervals[i].IntervalType = TableGridIntervalType.PercentBased;
						int percentWidth = restPercentWidth / unsetCount;
						intervals[i].Width = percentWidth;
						unsetCount--;
						restPercentWidth -= percentWidth;
					}
					totalPercentWidth = 100 * 50;
				}
			}
			for (int i = 0; i < count; i++) {
				if (intervals[i].IntervalType != TableGridIntervalType.PercentBased)
					continue;
				intervals[i].IntervalType = TableGridIntervalType.ModelUnit;
				int newWidth = totalPercentWidth > 0 ? restUnitWidth * intervals[i].Width / totalPercentWidth : 0;
				totalPercentWidth -= intervals[i].Width;
				intervals[i].Width = Math.Max(1, newWidth);
				restUnitWidth -= newWidth;
			}
		}
		public List<TableGridInterval> CalculateGridIntervals(Table table) {
			List<TableGridInterval> currentRow = CreateIntervals(table.Rows[0]);
			int rowCount = table.Rows.Count;
			for (int i = 1; i < rowCount; i++) {
				List<TableGridInterval> nextRow = CreateIntervals(table.Rows[i]);
				currentRow = CalculateTableGridCore(currentRow, nextRow);
			}
			return currentRow;
		}
		LayoutUnit GetActualWidth(WidthUnit unit, int percentBaseWidth) {
			if (unit.Type == WidthUnitType.ModelUnits)
				return converter.ToLayoutUnits(unit.Value);
			if (unit.Type == WidthUnitType.FiftiethsOfPercent)
				return unit.Value * percentBaseWidth / 5000;
			return 0;
		}
		void AutofitTable(TableGrid grid, Table table, int percentBaseWidth) {
			LayoutUnit width = GetTotalGridWidth(grid);
			LayoutUnit minWidth = GetTotalMinWidthCore(grid, 0, grid.Columns.Count - 1);
			if (table.PreferredWidth.Type == WidthUnitType.ModelUnits || table.PreferredWidth.Type == WidthUnitType.FiftiethsOfPercent) {
				LayoutUnit preferredTableWidth  = GetActualWidth(table.PreferredWidth, percentBaseWidth);
				if (width >= preferredTableWidth) {
					if (minWidth < maxTableWidth || preferredTableWidth > maxTableWidth)
						CompressTableGridToPreferredWidth(grid, width, preferredTableWidth);
					else
						CompressTableGridToColumnWidth(grid, width, maxTableWidth);
				}
				else if (width < preferredTableWidth) {
					EnlargeTableGridToPreferredWidth(grid, width, preferredTableWidth);
				}
			}
			else {
				if (width > maxTableWidth && (table.TableLayout == TableLayoutType.Autofit || SimpleView))
					CompressTableGridToColumnWidth(grid, width, maxTableWidth);
				else { 
					CompressRelativelySizedTable(grid, table);
				}
			}
		}
		void ApplyCellContentWidth(TableGrid grid, Table table, int percentBaseWidth) {
			TableRowCollection rows = table.Rows;
			int rowCount = rows.Count;
			for (int rowIndex = 0; rowIndex < rowCount; rowIndex++) {
				TableRow row = rows[rowIndex];
				TableCellCollection cells = row.Cells;
				int columnIndex = row.GridBefore;
				int cellCount = cells.Count;
				for (int cellIndex = 0; cellIndex < cellCount; cellIndex++) {
					TableCell cell = cells[cellIndex];
					if (cell.ColumnSpan == 1)
						ApplyCellContentWidthWithoutSpan(grid, cell, columnIndex, percentBaseWidth);
					columnIndex += cell.ColumnSpan;
				}
			}
			TableGridColumnCollection columns = grid.Columns;
			int columnCount = columns.Count;
			for (int i = 0; i < columnCount; i++) {
				if (grid[i].MinWidth == 0 && grid[i].MaxWidth == 0) {
					grid[i].MinWidth = grid[i].Width;
					grid[i].MaxWidth = grid[i].Width;
				}
			}
			for (int rowIndex = 0; rowIndex < rowCount; rowIndex++) {
				TableRow row = rows[rowIndex];
				TableCellCollection cells = row.Cells;
				int columnIndex = row.GridBefore;
				int cellCount = cells.Count;
				for (int cellIndex = 0; cellIndex < cellCount; cellIndex++) {
					TableCell cell = cells[cellIndex];
					if (cell.ColumnSpan > 1)
						ApplyCellContentWidthWithSpan(grid, cell, columnIndex, percentBaseWidth);
					columnIndex += cell.ColumnSpan;
				}
			}
			for (int i = 0; i < columnCount; i++) {
				TableGridColumn column = columns[i];
				column.MaxWidth = Math.Max(column.MaxWidth, 1);
				column.Width = Math.Max(column.MaxWidth, 1);
			}
		}
		void CompressRelativelySizedTable(TableGrid grid, Table table) {
			if( !(table.PreferredWidth.Type == WidthUnitType.Auto && table.TableLayout == TableLayoutType.Autofit && this.flex))
				return;
			TableRowCollection rows = table.Rows;
			int rowCount = rows.Count;
			TableGridColumnCollection columns = grid.Columns;
			int columnCount = columns.Count;
			double ratio = 0.0;
			for(int i = 0; i < rowCount; i++) {
				TableRow row = rows[i];
				int colCount = row.Cells.Count;
				for(int j = 0; j < colCount; j++) {
					TableCell cell = row.Cells[j];
					if(cell.VerticalMerging == MergingState.Continue)
						continue;
					if(cell.LayoutProperties.ContentWidthsInfo.MaxWidth >= cell.LayoutProperties.ContainerWidthsInfo.MaxWidth)
						return;
					double q = (double) cell.LayoutProperties.ContentWidthsInfo.MaxWidth / (double) cell.LayoutProperties.ContainerWidthsInfo.MaxWidth;
					if(q > ratio)
						ratio = q;
				}
			}
			for(int i = 0; i < columnCount; i++) {
				TableGridColumn column = columns[i];
				column.Width = Math.Max((int)(column.Width * ratio), column.MinWidth);
			}
		}
		void EnlargeTableGridToPreferredWidth(TableGrid grid, LayoutUnit oldWidth, LayoutUnit newWidth) {
#if DEBUG || DEBUGTEST
			CheckAllPreferredWidthEqualToWidth(grid);
#endif
			LayoutUnit totalMaxWidth = GetTotalMaxWidthCore(grid, 0, grid.Columns.Count - 1);
			LayoutUnit totalPreferredWidth = GetTotalPreferredWidth(grid, 0, grid.Columns.Count - 1);
			bool hasColumnsWithoutPreferredWidth = HasColumnsWithoutPreferredWidth(grid, 0, grid.Columns.Count - 1);
			LayoutUnit totalDelta = newWidth - oldWidth;
			LayoutUnit rest = totalMaxWidth;
			if (hasColumnsWithoutPreferredWidth)
				rest -= totalPreferredWidth;
			for (int i = grid.Columns.Count - 1; i >= 0; i--) {
				if (!hasColumnsWithoutPreferredWidth || grid.Columns[i].PreferredWidth == 0) {
					int delta = grid.Columns[i].MaxWidth * totalDelta / rest;
					grid.Columns[i].Width = grid.Columns[i].Width + delta;
					rest -= grid.Columns[i].MaxWidth;
					totalDelta -= delta;
				}
			}
			return;
		}
#if DEBUG || DEBUGTEST
		void CheckAllPreferredWidthEqualToWidth(TableGrid grid) {
		}
#endif
		void CompressTableGridToColumnWidth(TableGrid grid, LayoutUnit oldWidth, LayoutUnit newWidth) {
			LayoutUnit totalMinWidth = GetTotalMinWidthCore(grid, 0, grid.Columns.Count - 1);
			if (totalMinWidth <= newWidth) {
				CompressTableGridToPreferredWidth(grid, oldWidth, newWidth);
				return;
			}
			if (AllowTablesToExtendIntoMargins) {
				CompressTableGridToPreferredWidth(grid, oldWidth, totalMinWidth);
				return;
			}
			LayoutUnit totalWidth = newWidth;
			LayoutUnit rest = totalMinWidth;
			for (int i = 0; i < grid.Columns.Count; i++) {
				int newColumnWidth = rest > 0 ? Math.Max(grid.Columns[i].MinWidth * totalWidth / rest, 1) : 1;
				grid.Columns[i].Width = newColumnWidth;
				totalWidth -= newColumnWidth;
				rest -= grid.Columns[i].MinWidth;
			}
		}
		void CompressTableGridToPreferredWidth(TableGrid grid, LayoutUnit oldWidth, LayoutUnit newWidth) {
			LayoutUnit totalMinWidth = 0;
			LayoutUnit totalMaxWidth = 0;
			LayoutUnit totalPreferredWidth = 0;
			for (int columnIndex = 0; columnIndex < grid.Columns.Count; columnIndex++) {
				TableGridColumn column = grid[columnIndex];
				totalMinWidth += column.MinWidth;
				totalMaxWidth += column.MaxWidth;
				totalPreferredWidth += column.PreferredWidth;
			}
			if (newWidth <= totalMinWidth) {
				for (int i = 0; i < grid.Columns.Count; i++)
					grid.Columns[i].Width = grid.Columns[i].MinWidth;
				return;
			}
			LayoutUnit totalMinWidthForNonPreferredWidthColumn = 0;
			LayoutUnit totalDelta = oldWidth - newWidth;
			for (int i = 0; i < grid.Columns.Count; i++) { 
				TableGridColumn column = grid.Columns[i];
				if (column.MaxWidth < column.MinWidth) {
					totalDelta += column.MinWidth - column.Width;
					int delta = column.MinWidth - column.MaxWidth;
					totalMinWidth -= delta;
					totalMaxWidth += delta;
					LayoutUnit minWidth = column.MinWidth;
					grid.Columns[i].MinWidth = column.MaxWidth;
					grid.Columns[i].MaxWidth = minWidth;
					grid.Columns[i].Width = column.MaxWidth;
				}
				if (column.PreferredWidth == 0)
					totalMinWidthForNonPreferredWidthColumn += column.MinWidth;
			}
			LayoutUnit totalMaxWidthForNonPreferredWidthColumn = totalMaxWidth - totalPreferredWidth;
			if (newWidth - totalMinWidthForNonPreferredWidthColumn < totalPreferredWidth) {
				for (int i = 0; i < grid.Columns.Count; i++) {
					TableGridColumn column = grid.Columns[i];
					if (column.PreferredWidth == 0) {
						totalDelta -= column.Width - column.MinWidth;
						grid.Columns[i].Width = column.MinWidth;
					}
				}
				LayoutUnit rest = totalPreferredWidth - (totalMinWidth - totalMinWidthForNonPreferredWidthColumn);
				for (int i = grid.Columns.Count - 1; i >= 0 && totalDelta > 0; i--) {
					if (grid.Columns[i].PreferredWidth > 0) {
						Debug.Assert(rest > 0);
						int delta = (int)(((float)(grid.Columns[i].PreferredWidth - grid.Columns[i].MinWidth) / rest) * totalDelta);
						grid.Columns[i].Width = Math.Max(grid.Columns[i].Width - delta, 1);
						rest -= grid.Columns[i].PreferredWidth - grid.Columns[i].MinWidth;
						totalDelta -= delta;
					}
				}
			}
			else {
				LayoutUnit rest = totalMaxWidthForNonPreferredWidthColumn - totalMinWidthForNonPreferredWidthColumn;
				for (int i = grid.Columns.Count - 1; i >= 0 && totalDelta > 0; i--) {
					if (grid.Columns[i].PreferredWidth == 0) {
						Debug.Assert(rest > 0);
						int delta = (int)(((float)(grid.Columns[i].MaxWidth - grid.Columns[i].MinWidth) / rest) * totalDelta);
						grid.Columns[i].Width = grid.Columns[i].Width - delta;
						rest -= grid.Columns[i].MaxWidth - grid.Columns[i].MinWidth;
						totalDelta -= delta;
					}
				}
			}
		}
		void ApplyCellContentWidthWithSpan(TableGrid grid, TableCell cell, int startColumnIndex, int percentBaseWidth) {
			Debug.Assert(cell.ColumnSpan > 1);
			int endColumnIndex = startColumnIndex + cell.ColumnSpan - 1;
			WidthsInfo info = CalculateCellWidthsInfo(cell, percentBaseWidth);
			LayoutUnit gridWidth = GetTotalWidthCore(grid, startColumnIndex, endColumnIndex);
			LayoutUnit cellMinWidth = Math.Max(1, info.MinWidth);
			LayoutUnit cellMaxWidth = Math.Max(1, info.MaxWidth);
			if (gridWidth > 0)
				cellMaxWidth = Math.Max(cellMinWidth, gridWidth);
			LayoutUnit preferredWidth = GetActualWidth(cell.PreferredWidth, percentBaseWidth);
			if (preferredWidth > 0) {
				preferredWidth = Math.Max(cellMinWidth, preferredWidth);
				cellMaxWidth = preferredWidth;
			}
			LayoutUnit gridMinWidth = GetTotalMinWidthCore(grid, startColumnIndex, endColumnIndex);
			if (cellMinWidth > gridMinWidth) {
				EnlargeColumnsMinWidth(grid, startColumnIndex, endColumnIndex, gridMinWidth, cellMinWidth);
			}
			LayoutUnit gridMaxWidth = GetTotalMaxWidthCore(grid, startColumnIndex, endColumnIndex);
			if (cellMaxWidth > gridMaxWidth) {
				EnlargeColumnsMaxWidth(grid, startColumnIndex, endColumnIndex, gridMaxWidth, cellMaxWidth);
			}
			LayoutUnit gridTotalMargins = GetTotalHorizontalMarginsCore(grid, startColumnIndex, endColumnIndex);
			if (info.TotalHorizontalMargins > gridTotalMargins) {
				EnlargeColumnsHorizontalMargins(grid, startColumnIndex, endColumnIndex, gridTotalMargins, info.TotalHorizontalMargins);
			}
		}
		protected virtual void EnlargeColumnsHorizontalMargins(TableGrid grid, LayoutUnit startColumnIndex, LayoutUnit endColumnIndex, LayoutUnit oldWidth, LayoutUnit newWidth) {
			LayoutUnit totalDelta = newWidth - oldWidth;
			bool equalSpace = oldWidth == 0;
			int totalCount = endColumnIndex - startColumnIndex + 1;
			for (int i = endColumnIndex; i >= startColumnIndex && totalDelta > 0; i--) {
				int delta = equalSpace ? totalDelta / totalCount : totalDelta * grid.Columns[i].TotalHorizontalMargins / oldWidth;
				totalDelta -= delta;
				oldWidth -= grid.Columns[i].TotalHorizontalMargins;
				grid.Columns[i].TotalHorizontalMargins += delta;
				totalCount--;
			}
		}
		void EnlargeColumnsMinWidth(TableGrid grid, int startColumnIndex, int endColumnIndex, int oldWidth, int newWidth) {
			bool hasColumnsWithoutPreferredWidth = HasColumnsWithoutPreferredWidth(grid, startColumnIndex, endColumnIndex);
			int zeroMinWidthCount = 0;
			int existingMinWidth = 0;
			for (int i = endColumnIndex; i >= startColumnIndex; i--)
				if (grid.Columns[i].MinWidth == 0 && grid.Columns[i].MaxWidth == 0)
					zeroMinWidthCount++;
				else
					existingMinWidth += grid.Columns[i].MinWidth;
			LayoutUnit rest = GetTotalMaxWidthCore(grid, startColumnIndex, endColumnIndex) + GetTotalMinWidthCore(grid, startColumnIndex, endColumnIndex);
			bool equalSpace = rest == 0;
			if (equalSpace || zeroMinWidthCount > 0) {
				rest = endColumnIndex - startColumnIndex + 1;
				newWidth -= existingMinWidth;
			}
			for (int i = endColumnIndex; i >= startColumnIndex; i--) {
				if (!hasColumnsWithoutPreferredWidth || grid.Columns[i].PreferredWidth == 0) {
					if (zeroMinWidthCount > 0 && (grid.Columns[i].MinWidth > 0 || grid.Columns[i].MaxWidth > 0))
						continue;
					int factor = (equalSpace || zeroMinWidthCount > 0) ? 1 : (grid.Columns[i].MinWidth + grid.Columns[i].MaxWidth);
					int newMinWidth = factor * newWidth / rest;
					rest -= factor;
					newWidth -= newMinWidth;
					grid.Columns[i].MinWidth = Math.Max(newMinWidth, grid.Columns[i].MinWidth);
					grid.Columns[i].MaxWidth = Math.Max(grid.Columns[i].MinWidth, grid.Columns[i].MaxWidth);
				}
			}
		}
		void EnlargeColumnsMaxWidth(TableGrid grid, int startColumnIndex, int endColumnIndex, int oldWidth, int newWidth) {
			bool hasColumnsWithoutPreferredWidth = HasColumnsWithoutPreferredWidth(grid, startColumnIndex, endColumnIndex);
			LayoutUnit rest = oldWidth;
			for (int i = endColumnIndex; i >= startColumnIndex; i--) {
				if (!hasColumnsWithoutPreferredWidth || grid.Columns[i].PreferredWidth == 0) {
					int newMaxWidth = rest != 0 ? grid.Columns[i].MaxWidth * newWidth / rest : 0;
					rest -= grid.Columns[i].MaxWidth;
					newWidth -= newMaxWidth;
					if (rest < 0)
						rest = 0;
					if (newWidth < 0)
						newWidth = 0;
					grid.Columns[i].MaxWidth = Math.Max(1, newMaxWidth);
				}
			}
		}
		void ApplyCellContentWidthWithoutSpan(TableGrid grid, TableCell cell, int columnIndex, int percentBaseWidth) {
			Debug.Assert(cell.ColumnSpan == 1);
			WidthsInfo info = CalculateCellWidthsInfo(cell, percentBaseWidth);
			LayoutUnit cellMinWidth = Math.Max(1, info.MinWidth);
			LayoutUnit cellMaxWidth = Math.Max(1, info.MaxWidth);
			if (grid[columnIndex].Width > 0)
				cellMaxWidth = Math.Max(cellMinWidth, grid[columnIndex].Width);
			LayoutUnit preferredWidth = GetActualWidth(cell.PreferredWidth, percentBaseWidth);
			grid[columnIndex].MinWidth = Math.Max(cellMinWidth, grid[columnIndex].MinWidth);
			grid[columnIndex].MaxWidth = Math.Max(cellMaxWidth, grid[columnIndex].MaxWidth);
			grid[columnIndex].TotalHorizontalMargins = Math.Max(grid[columnIndex].TotalHorizontalMargins, info.TotalHorizontalMargins);
			if (preferredWidth <= 0 && cell.NoWrap)
				preferredWidth = grid[columnIndex].MaxWidth;
			if (preferredWidth > 0) {
				if (!cell.NoWrap)
					preferredWidth = Math.Max(preferredWidth, cellMinWidth);
				grid[columnIndex].PreferredWidth = Math.Max(grid[columnIndex].PreferredWidth, preferredWidth);
				grid[columnIndex].MaxWidth = grid[columnIndex].PreferredWidth;
			}
			else {
				LayoutUnit totalColumnsMinWidth = GetTotalColumnsMinWidth(grid, columnIndex, columnIndex);
				if (cellMinWidth > totalColumnsMinWidth) {
					EnlargeColumnsWidths(grid, columnIndex, columnIndex, totalColumnsMinWidth, cellMinWidth);
				}
			}
		}
		protected virtual WidthsInfo CalculateCellWidthsInfo(TableCell cell, int percentBaseWidth) {
			return tableWidthsCalculator.CalculateCellWidths(cell, percentBaseWidth, simpleView);
		}
		void EnlargeColumnsWidths(TableGrid grid, int startColumnIndex, int endColumnIndex, LayoutUnit oldMinWidth, LayoutUnit newMinWidth) {
			LayoutUnit rest = newMinWidth - oldMinWidth;
			LayoutUnit availableWidth = 0;
			TableGridColumnCollection columns = grid.Columns;
			LayoutUnit totalRestMaxWidth = 0;
			for (int i = startColumnIndex; i <= endColumnIndex; i++) {
				availableWidth += columns[i].MaxWidth - columns[i].MinWidth;
				totalRestMaxWidth += columns[i].MaxWidth;
			}
			if (availableWidth < (newMinWidth - oldMinWidth)) {
				for (int i = startColumnIndex; i <= endColumnIndex; i++) {
					LayoutUnit delta = columns[i].MaxWidth * rest / totalRestMaxWidth;
					LayoutUnit newMaxWidth = columns[i].MaxWidth + delta;
					totalRestMaxWidth -= columns[i].MaxWidth;
					rest -= delta;
					columns[i].MaxWidth = newMaxWidth;
					columns[i].MinWidth = columns[i].MaxWidth;
				}
			}
			else {
				for (int i = startColumnIndex; i <= endColumnIndex; i++) {
					LayoutUnit delta = (columns[i].MaxWidth - columns[i].MinWidth) * rest / availableWidth;
					availableWidth -= columns[i].MaxWidth - columns[i].MinWidth;
					rest -= delta;
					columns[i].MinWidth += delta;
				}
			}
		}
		LayoutUnit GetTotalColumnsMinWidth(TableGrid grid, int startColumnIndex, int endColumnIndex) {
			LayoutUnit result = 0;
			for (int i = startColumnIndex; i <= endColumnIndex; i++) {
				result += grid.Columns[i].MinWidth;
			}
			return result;
		}
		LayoutUnit GetTotalGridWidth(TableGrid grid) {
			return GetTotalWidthCore(grid, 0, grid.Columns.Count - 1);
		}
		LayoutUnit GetTotalWidthCore(TableGrid grid, int startColumnIndex, int endColumnIndex) {
			LayoutUnit result = 0;
			for (int columnIndex = startColumnIndex; columnIndex <= endColumnIndex; columnIndex++)
				result += grid[columnIndex].Width;
			return result;
		}
		LayoutUnit GetTotalMinWidthCore(TableGrid grid, int startColumnIndex, int endColumnIndex) {
			LayoutUnit result = 0;
			for (int columnIndex = startColumnIndex; columnIndex <= endColumnIndex; columnIndex++)
				result += grid[columnIndex].MinWidth;
			return result;
		}
		LayoutUnit GetTotalHorizontalMarginsCore(TableGrid grid, int startColumnIndex, int endColumnIndex) {
			LayoutUnit result = 0;
			for (int columnIndex = startColumnIndex; columnIndex <= endColumnIndex; columnIndex++)
				result += grid[columnIndex].TotalHorizontalMargins;
			return result;
		}
		LayoutUnit GetTotalMaxWidthCore(TableGrid grid, int startColumnIndex, int endColumnIndex) {
			LayoutUnit result = 0;
			for (int columnIndex = startColumnIndex; columnIndex <= endColumnIndex; columnIndex++)
				result += grid[columnIndex].MaxWidth;
			return result;
		}
		bool HasColumnsWithoutPreferredWidth(TableGrid grid, int startColumnIndex, int endColumnIndex) {
			for (int columnIndex = startColumnIndex; columnIndex <= endColumnIndex; columnIndex++)
				if (grid[columnIndex].PreferredWidth == 0)
					return true;
			return false;
		}
		LayoutUnit GetTotalPreferredWidth(TableGrid grid, int startColumnIndex, int endColumnIndex) {
			LayoutUnit result = 0;
			for (int columnIndex = startColumnIndex; columnIndex <= endColumnIndex; columnIndex++)
				result += grid[columnIndex].PreferredWidth;
			return result;
		}
		protected internal virtual TableGridInterval CreateTableGridInterval(DocumentModelUnitToLayoutUnitConverter unitConverter, WidthUnit width, int columnSpan) {
			if (width.Type == WidthUnitType.ModelUnits)
				return new TableGridInterval(unitConverter.ToLayoutUnits(width.Value), columnSpan, TableGridIntervalType.ModelUnit);
			if (width.Type == WidthUnitType.FiftiethsOfPercent)
				return new TableGridInterval(width.Value, columnSpan, TableGridIntervalType.PercentBased);
			return new TableGridInterval(0, columnSpan, TableGridIntervalType.NotSet);
		}
		protected internal virtual List<TableGridInterval> CreateIntervals(TableRow row) {
			List<TableGridInterval> result = new List<TableGridInterval>();
			DocumentModelUnitToLayoutUnitConverter unitConverter = this.converter;
			if (row.Properties.GridBefore > 0)
				result.Add(CreateTableGridInterval(unitConverter, row.Properties.WidthBefore, row.Properties.GridBefore));
			ConvertToIntervals(row.Cells, result, unitConverter);
			if (row.Properties.GridAfter > 0)
				result.Add(CreateTableGridInterval(unitConverter, row.Properties.WidthAfter, row.Properties.GridAfter));
			return result;
		}
		protected internal virtual void ConvertToIntervals(TableCellCollection cells, List<TableGridInterval> intervals, DocumentModelUnitToLayoutUnitConverter unitConverter) {
			int cellsCount = cells.Count;
			for (int i = 0; i < cellsCount; i++)
				intervals.Add(CreateTableGridInterval(unitConverter, cells[i].PreferredWidth, cells[i].ColumnSpan));
		}
		protected internal virtual List<TableGridInterval> CalculateTableGridCore(List<TableGridInterval> currentRow, List<TableGridInterval> nextRow) {
			Guard.ArgumentNotNull(currentRow, "currentRow");
			Guard.ArgumentNotNull(nextRow, "nextRow");
			List<TableGridInterval> result;
			result = CalculateTableGridPartially(currentRow, nextRow);
			return result;
		}
		protected internal virtual List<TableGridInterval> CalculateTableGridPartially(List<TableGridInterval> currentRow, List<TableGridInterval> nextRow) {
			List<TableGridInterval> result = new List<TableGridInterval>();
			TableGridIntervalIterator currentRowIterator = new TableGridIntervalIterator(currentRow);
			TableGridIntervalIterator nextRowIterator = new TableGridIntervalIterator(nextRow);
			while (!currentRowIterator.EndOfIntervals && !nextRowIterator.EndOfIntervals) {
				int currentRowSpan = currentRowIterator.CurrentInterval.ColumnSpan;
				int nextRowSpan = nextRowIterator.CurrentInterval.ColumnSpan;
				if (currentRowSpan > nextRowSpan)
					ProcessDependedIntervals(currentRowIterator, nextRowIterator, result);
				else if (currentRowSpan < nextRowSpan)
					ProcessDependedIntervals(nextRowIterator, currentRowIterator, result);
				else
					ProcessIntervals(currentRowIterator, nextRowIterator, result);
			}
			CopyRestIntervals(currentRowIterator, result);
			CopyRestIntervals(nextRowIterator, result);
			return result;
		}
		void ProcessIntervals(TableGridIntervalIterator currentRowIterator, TableGridIntervalIterator nextRowIterator, List<TableGridInterval> result) {
			TableGridInterval currentRowInterval = currentRowIterator.CurrentInterval;
			TableGridInterval nextRowInterval = nextRowIterator.CurrentInterval;
			TableGridInterval newInterval = CalculateNewInterval(currentRowInterval, nextRowInterval);
			result.Add(newInterval);
			nextRowIterator.Advance(newInterval);
			currentRowIterator.Advance(newInterval);
		}
		void ProcessDependedIntervals(TableGridIntervalIterator masterRowIterator, TableGridIntervalIterator slaveRowIterator, List<TableGridInterval> result) {
			List<TableGridInterval> deferredIntervals = new List<TableGridInterval>();
			int autoSizeIntervalsCount = 0;
			TableGridInterval restMasterInterval = masterRowIterator.CurrentInterval;
			do {
				TableGridInterval slaveInterval = slaveRowIterator.CurrentInterval;
				if (slaveInterval.IntervalType == TableGridIntervalType.NotSet)
					autoSizeIntervalsCount++;
				deferredIntervals.Add(slaveInterval);
				slaveRowIterator.Advance(slaveInterval);
				restMasterInterval = TableGridIntervalIterator.SubsctractIntervals(restMasterInterval, slaveInterval);
			} while (restMasterInterval.ColumnSpan > 0 && !slaveRowIterator.EndOfIntervals && slaveRowIterator.CurrentInterval.ColumnSpan <= restMasterInterval.ColumnSpan);
			bool calculateNotSetIntervals = restMasterInterval.IntervalType == TableGridIntervalType.ModelUnit && autoSizeIntervalsCount > 0;
			int restWidth = restMasterInterval.Width;
			int newWidth = calculateNotSetIntervals && restWidth > 0 ? restWidth / autoSizeIntervalsCount : 0;
			int count = deferredIntervals.Count;
			for (int i = 0; i < count; i++) {
				TableGridInterval interval = deferredIntervals[i];
				if (calculateNotSetIntervals && interval.IntervalType == TableGridIntervalType.NotSet) {
					interval.IntervalType = TableGridIntervalType.ModelUnit;
					interval.Width = newWidth;
				}
				if (masterRowIterator.CurrentInterval.ColumnSpan > 1)
					result.Add(interval);
				else
					result.Add(CalculateNewInterval(masterRowIterator.CurrentInterval, interval));
				masterRowIterator.Advance(interval);
			}
		}
		TableGridInterval CalculateNewInterval(TableGridInterval currentRowInterval, TableGridInterval nextRowInterval) {
			int currentRowSpan = currentRowInterval.ColumnSpan;
			if (currentRowInterval.IntervalType == nextRowInterval.IntervalType) {
				LayoutUnit width = Math.Max(currentRowInterval.Width, nextRowInterval.Width);
				return new TableGridInterval(width, currentRowSpan, nextRowInterval.IntervalType);
			}
			else {
				if (currentRowInterval.IntervalType == TableGridIntervalType.PercentBased)
					return CalculateNewIntervalFromMixedIntervals(currentRowInterval, nextRowInterval, currentRowSpan);
				else if (nextRowInterval.IntervalType == TableGridIntervalType.PercentBased)
					return CalculateNewIntervalFromMixedIntervals(nextRowInterval, currentRowInterval, currentRowSpan);
				else if(currentRowInterval.IntervalType == TableGridIntervalType.ModelUnit) {
					Debug.Assert(nextRowInterval.IntervalType == TableGridIntervalType.NotSet);
					return new TableGridInterval(currentRowInterval.Width, currentRowSpan, TableGridIntervalType.ModelUnit);
				}
				else {
					Debug.Assert(currentRowInterval.IntervalType == TableGridIntervalType.NotSet);
					Debug.Assert(nextRowInterval.IntervalType == TableGridIntervalType.ModelUnit);
					return new TableGridInterval(nextRowInterval.Width, currentRowSpan, TableGridIntervalType.ModelUnit);
				}
			}
		}
		TableGridInterval CalculateNewIntervalFromMixedIntervals(TableGridInterval percentWidthInterval, TableGridInterval valueWidthInterval, int columnSpan) {
			if (valueWidthInterval.Width > 0)
				return new TableGridInterval(valueWidthInterval.Width, columnSpan, valueWidthInterval.IntervalType);
			else
				return new TableGridInterval(percentWidthInterval.Width, columnSpan, percentWidthInterval.IntervalType);
		}
		void CopyRestIntervals(TableGridIntervalIterator source, List<TableGridInterval> collection) {
			while (!source.EndOfIntervals) {
				TableGridInterval interval = source.CurrentInterval;
				collection.Add(interval);
				source.Advance(interval);
			}
		}
	}
	#endregion
	#region TableGridIntervalIterator
	public class TableGridIntervalIterator {
		#region Fields
		readonly List<TableGridInterval> intervals;
		int currentIntervalIndex;
		TableGridInterval currentInterval;
		#endregion
		public TableGridIntervalIterator(List<TableGridInterval> intervals) {
			Guard.ArgumentNotNull(intervals, "intervals");
			this.intervals = intervals;
			Reset();
		}
		#region Properties
		public List<TableGridInterval> Intervals { get { return intervals; } }
		internal int CurrentIntervalIndex { get { return currentIntervalIndex; } }
		public bool EndOfIntervals { get { return CurrentInterval == null; } }
		public TableGridInterval CurrentInterval { get { return currentInterval; } }
		#endregion
		public void Reset() {
			this.currentIntervalIndex = -1;
			this.currentInterval = null;
			MoveNextInterval();
		}
		internal bool MoveNextInterval() {
			this.currentIntervalIndex++;
			if (this.currentIntervalIndex >= intervals.Count) {
				currentInterval = null;
				return false;
			}
			TableGridInterval interval = Intervals[CurrentIntervalIndex];
			currentInterval = new TableGridInterval(interval.Width, interval.ColumnSpan, interval.IntervalType);
			return true;
		}
		public bool Advance(TableGridInterval interval) {
			Debug.Assert(!EndOfIntervals);
			Debug.Assert(CurrentInterval.ColumnSpan >= interval.ColumnSpan);
			if (CurrentInterval.ColumnSpan == interval.ColumnSpan)
				return MoveNextInterval();
			this.currentInterval = SubsctractIntervals(CurrentInterval, interval);
			return true;
		}
		internal static TableGridInterval SubsctractIntervals(TableGridInterval interval1, TableGridInterval interval2) {
			int columnSpan = interval1.ColumnSpan - interval2.ColumnSpan;
			if (interval1.IntervalType == interval2.IntervalType)
				return new TableGridInterval(Math.Max(interval1.Width - interval2.Width, 0), columnSpan, interval1.IntervalType);
			else {
				if (interval1.IntervalType == TableGridIntervalType.PercentBased)
					return new TableGridInterval(interval2.Width, columnSpan, interval2.IntervalType);
				else if (interval1.IntervalType == TableGridIntervalType.ModelUnit)
					return new TableGridInterval(interval1.Width, columnSpan, interval1.IntervalType);
				else
					return new TableGridInterval(0, columnSpan, TableGridIntervalType.NotSet);
			}
		}
	}
	#endregion
	public enum TableGridIntervalType {
		PercentBased,
		ModelUnit,
		NotSet
	}
	#region TableGridInterval
	public class TableGridInterval {
		LayoutUnit width;
		readonly int colSpan;
		TableGridIntervalType intervalType;
		public TableGridInterval(LayoutUnit width, int colSpan, TableGridIntervalType intervalType) {
			this.width = width;
			this.colSpan = colSpan;
			this.intervalType = intervalType;
		}
		public LayoutUnit Width {
			get { return width; }
			set { width = value; }
		}
		public int ColumnSpan {
			get { return colSpan; }
		}
		public TableGridIntervalType IntervalType {
			get { return intervalType; }
			set { intervalType = value; }
		}
		public override bool Equals(object obj) {
			TableGridInterval interval = obj as TableGridInterval;
			if (Object.ReferenceEquals(interval, null))
				return false;
			return (Width == interval.Width && ColumnSpan == interval.ColumnSpan && IntervalType == interval.IntervalType);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override string ToString() {
			return String.Format("Width = {0}, ColumnSpan = {1}", Width, ColumnSpan);
		}
	}
	public class AutofitTableLayoutCalculator {
		public void CompressTableGrid(TableGrid grid, int startIndex, int endIndex, LayoutUnit newTableWidth) {
			List<int> deltas = new List<int>();
			LayoutUnit deltasTotalWidth = 0;
			LayoutUnit initialTableWidth = 0;
			for (int i = startIndex; i <= endIndex; i++) {
				initialTableWidth += grid[i].Width;
				LayoutUnit delta = Math.Max(grid[i].Width - grid[i].MinWidth, 0);
				deltas.Add(delta);
				deltasTotalWidth += delta;
			}
			LayoutUnit deltaTableWidth = initialTableWidth - newTableWidth;
			if (deltasTotalWidth > deltaTableWidth)
				CompressProportionallyWidthCore(grid, deltas, deltasTotalWidth, deltaTableWidth);
			else {
				for (int i = startIndex; i <= endIndex; i++)
					grid.Columns[i].Width -= deltas[i];
				ChangeColumnsProportionally(grid, startIndex, endIndex, initialTableWidth - deltasTotalWidth, newTableWidth);
			}
		}
		public void EnlargeProportionallyAverageWidth(TableGrid grid, int startIndex, int endIndex, LayoutUnit newWidth) {
			List<int> averageWidths = new List<int>();
			LayoutUnit totalWidth = 0;
			for (int i = startIndex; i <= endIndex; i++) {
				LayoutUnit averageWidth = grid[i].MaxWidth + grid[i].MinWidth;
				averageWidths.Add(averageWidth);
				totalWidth += averageWidth;
			}
			LayoutUnit rest = newWidth;
			for (int i = startIndex; i <= endIndex; i++) {
				LayoutUnit width = (i != endIndex) ? averageWidths[i] * newWidth / totalWidth : rest;
				if (width > grid[i].MaxWidth)
					grid[i].MaxWidth = grid[i].MinWidth = grid[i].Width = width;
				else
					grid[i].MinWidth = width;
				rest -= width;
			}
		}
		public void ChangeColumnsProportionally(TableGrid grid, int startIndex, int endIndex, LayoutUnit initialWidth, LayoutUnit newWidth) {
			LayoutUnit deltaTableWidth = Math.Abs(newWidth - initialWidth);
			LayoutUnit rest = deltaTableWidth;
			for (int i = startIndex; i <= endIndex; i++) {
				LayoutUnit delta = (i != endIndex) ? grid[i].Width * deltaTableWidth / initialWidth : rest;
				if (initialWidth > newWidth)
					grid[i].Width = Math.Max(1, grid[i].Width - delta);
				else
					grid[i].Width += delta;
				rest -= delta;
			}
		}
		void CompressProportionallyWidthCore(TableGrid grid, List<LayoutUnit> items, LayoutUnit totalItemsWidth, LayoutUnit deltaTableWidth) {
			int colCount = grid.Columns.Count;
			if (totalItemsWidth == 0) {
				LayoutUnit rest = deltaTableWidth;
				totalItemsWidth = colCount;
				for (int i = 0; i < colCount; i++) {
					LayoutUnit delta = (i != colCount - 1) ? deltaTableWidth / totalItemsWidth : rest;
					grid.Columns[i].Width = Math.Max(grid.Columns[i].Width - delta, 0);
					rest -= delta;
				}
				return;
			}
			for (int i = 0; i < colCount; i++) {
				LayoutUnit delta = items[i] * deltaTableWidth / totalItemsWidth;
				grid.Columns[i].Width -= delta;
				deltaTableWidth -= delta;
				totalItemsWidth -= items[i];
				if (totalItemsWidth == 0) {
					Debug.Assert(deltaTableWidth == 0);
					break;
				}
			}
		}
	}
	#endregion
	#region SelectedCellsIntervalInRow
	public class SelectedCellsIntervalInRow {
		TableRow row;
		int startCellIndex;
		int endCellIndex;
		public SelectedCellsIntervalInRow(TableRow row, int startCellIndex, int endCellIndex) {
			this.row = row;
			this.startCellIndex = startCellIndex;
			this.endCellIndex = endCellIndex;
		}
		public TableRow Row { get { return row; } }
		public int NormalizedStartCellIndex { get { return Math.Min(startCellIndex, endCellIndex); } }
		public int NormalizedEndCellIndex { get { return Math.Max(startCellIndex, endCellIndex); } }
		public TableCell NormalizedStartCell { get { return Row.Cells[NormalizedStartCellIndex]; } }
		public TableCell NormalizedEndCell { get { return Row.Cells[NormalizedEndCellIndex]; } }
		internal int StartCellIndex { get { return startCellIndex; } set { startCellIndex = value; } }
		internal int EndCellIndex { get { return endCellIndex; } set { endCellIndex = value; } }
		internal TableCell StartCell { get { return StartCellIndex < Row.Cells.Count ? Row.Cells[StartCellIndex] : null; } }
		internal TableCell EndCell { get { return EndCellIndex < Row.Cells.Count ? Row.Cells[EndCellIndex] : null; } }
		internal bool IsContainsOnlyOneCell { get { return StartCellIndex == EndCellIndex; } }
		public TableCell LeftCell { get { return (startCellIndex < endCellIndex) ? Row.Cells[StartCellIndex] : row.Cells[EndCellIndex]; } }
		public int NormalizedLength { get { return Math.Abs(EndCellIndex - StartCellIndex); } }
		public Table Table { get { return Row.Table; } }
		public int GetNormalizedColumnSpan() {
			int startCellIndex = NormalizedStartCellIndex;
			int endCellIndex = NormalizedEndCellIndex;
			int result = 0;
			TableCellCollection cells = row.Cells;
			for (int i = startCellIndex; i <= endCellIndex; i++) {
				result += cells[i].ColumnSpan;
			}
			return result;
		}
		public virtual bool ContainsCell(TableCell cell) {
			int end = NormalizedEndCellIndex;
			TableCellCollection cells = Row.Cells;
			for (int i = NormalizedStartCellIndex; i <= end; i++) {
				if (cells[i] == cell)
					return true;
			}
			return false;
		}
	}
	#endregion
	#region ISelectedTableStructureBase 
	public interface ISelectedTableStructureBase {
		DocumentLogPosition OriginalStartLogPosition { get; }
		TableCell FirstSelectedCell { get;}
		bool IsNotEmpty { get; }
		bool SelectedOnlyOneCell{ get; }
		void SetFirstSelectedCell(TableCell startCell, DocumentLogPosition pos);
		int RowsCount { get; }
		ISelectedTableStructureBase CloneWithShift(PieceTable targetPieceTable, int delta);
		void CopyProperties(ISelectedTableStructureBase selectedCells);
	}
	#endregion
	#region SelectedCellsCollection
	public class SelectedCellsCollection : ISelectedTableStructureBase {
		#region Fields
		List<SelectedCellsIntervalInRow> innerList;
		DocumentLogPosition originalStartLogPosition;
		#endregion
		public SelectedCellsCollection() {
			this.innerList = new List<SelectedCellsIntervalInRow>();
			this.originalStartLogPosition = DocumentLogPosition.Zero;
		}
		public SelectedCellsCollection(TableCell firstSelectedCell, DocumentLogPosition originalStartLogPosition)
			: this() {
			SetFirstSelectedCell(firstSelectedCell, originalStartLogPosition);
		}
		public SelectedCellsCollection(StartSelectedCellInTable old)
			: this(old.FirstSelectedCell, old.OriginalStartLogPosition) {
		}
		#region Properties
		public SelectedCellsIntervalInRow NormalizedFirst { get { return RowsCount > 0 ? this[GetNormalizedTopRowIndex()] : null; } }
		public SelectedCellsIntervalInRow NormalizedLast { get { return this[GetNormalizedBottomRowIndex()]; } }
		protected internal SelectedCellsIntervalInRow First { get { return RowsCount > 0 ? this[0] : null; } }
		protected internal SelectedCellsIntervalInRow Last { get { return RowsCount > 0 ? this[RowsCount - 1] : null; } }
		public SelectedCellsIntervalInRow this[int index] { get { return innerList[index]; } set { innerList[index] = value; } }
		public TableCell TopLeftCell { get { return NormalizedFirst != null ? NormalizedFirst.NormalizedStartCell : null; } }
		public TableCell FirstSelectedCell { get { return First != null ? First.StartCell : null; } }
		public bool SelectedOnlyOneCell { get { return RowsCount == 1 && First.NormalizedLength == 0; } }
		public bool IsNotEmpty {
			get {
				if (RowsCount == 0)
					return false;
				for (int i = 0; i < RowsCount; i++) {
					if (this[i].StartCell == null || this[i].EndCell == null)
						return false;
				}
				return true;
			}
		}
		public int RowsCount { get { return innerList.Count; } }
		public DocumentLogPosition OriginalStartLogPosition { get { return originalStartLogPosition; } }
		#endregion
		public ISelectedTableStructureBase CloneWithShift(PieceTable targetPieceTable, int delta) {
			SelectedCellsCollection result = new SelectedCellsCollection();
			result.originalStartLogPosition = originalStartLogPosition + delta;
			foreach (SelectedCellsIntervalInRow interval in innerList) {
				ParagraphIndex originalParagraphIndex = interval.Row.FirstCell.StartParagraphIndex;
				Paragraph paragraph = interval.Row.PieceTable.Paragraphs[originalParagraphIndex];
				DocumentLogPosition position = paragraph.LogPosition + delta;
				TableRow row = targetPieceTable.FindParagraph(position).GetCell().Row;
				SelectedCellsIntervalInRow newInterval = new SelectedCellsIntervalInRow(row, interval.StartCellIndex, interval.EndCellIndex);
				result.innerList.Add(newInterval);
			}
			return result;
		}
		public void CopyProperties(ISelectedTableStructureBase selectedCells) {
			SelectedCellsCollection targetSelectedCells = (SelectedCellsCollection)selectedCells;
			if (targetSelectedCells != null) {
				Table targetTable = targetSelectedCells.innerList[0].Row.Table;
				Table sourceTable = this.innerList[0].Row.Table;
				targetTable.CopyProperties(sourceTable);
				targetTable.TableStyle.Copy(sourceTable.DocumentModel);
				for (int i = 0; i < targetTable.Rows.Count; i++) {
					TableRow currentRow = targetTable.Rows[i];
					currentRow.Properties.CopyFrom(sourceTable.Rows[i].Properties);
					for (int j = 0; j < currentRow.Cells.Count; j++) {
						TableCell currentCell = currentRow.Cells[j];
						currentCell.Properties.ResetAllUse();
						currentCell.Properties.CopyFrom(sourceTable.Rows[i].Cells[j].Properties);
					}
				}
			}
		}
		public bool IsSquare() {
			if (!IsNotEmpty)
				return false;
			int startColumnIndexInFirstInterval = TopLeftCell.GetStartColumnIndexConsiderRowGrid();
			int endColumnIndexInFirstInterval = NormalizedFirst.NormalizedEndCell.GetEndColumnIndexConsiderRowGrid();
			int bottomRowIndex = GetBottomRowIndex();
			for (int index = GetTopRowIndex(); index <= bottomRowIndex; index++) {
				SelectedCellsIntervalInRow currentInterval = this[index];
				int startColumnIndex = currentInterval.NormalizedStartCell.GetStartColumnIndexConsiderRowGrid();
				int endColumnIndex = currentInterval.NormalizedEndCell.GetEndColumnIndexConsiderRowGrid();
				if (startColumnIndexInFirstInterval != startColumnIndex || endColumnIndexInFirstInterval != endColumnIndex)
					return false;
			}
			int selectedRowsCountInFirstColumn = GetSelectedRowsCount(startColumnIndexInFirstInterval);
			for (int i = startColumnIndexInFirstInterval + 1; i <= endColumnIndexInFirstInterval; i++) {
				if (selectedRowsCountInFirstColumn != GetSelectedRowsCount(i))
					return false;
			}
			return true;
		}
		public int GetTopRowIndex() {
			return Math.Min(innerList.IndexOf(First), innerList.IndexOf(Last));
		}
		protected internal int GetNormalizedTopRowIndex() {
			if (First.Row.IndexInTable < Last.Row.IndexInTable)
				return 0;
			return RowsCount - 1;
		}
		public int GetBottomRowIndex() {
			return Math.Max(innerList.IndexOf(First), innerList.IndexOf(Last));
		}
		protected internal int GetNormalizedBottomRowIndex() {
			if (First.Row.IndexInTable < Last.Row.IndexInTable)
				return RowsCount - 1;
			return 0;
		}
		protected internal virtual int GetSelectedColumnsCountInRow(SelectedCellsIntervalInRow selectionInterval) {
			int result = 0;
			TableCellCollection cells = selectionInterval.Row.Cells;
			for (int cellId = selectionInterval.NormalizedStartCellIndex; cellId <= selectionInterval.NormalizedEndCellIndex; cellId++) {
				result += cells[cellId].ColumnSpan;
			}
			return result;
		}
		protected internal virtual int GetSelectedRowsCount(int columnIndex) {
			int result = 0;
			int bottomRowIndex = GetBottomRowIndex();
			for (int i = GetTopRowIndex(); i <= bottomRowIndex; i++) {
				TableCell currentCell = TableCellVerticalBorderCalculator.GetCellByColumnIndex(this[i].Row, columnIndex);
				if (currentCell == null)
					continue;
				result += GetSelectedRowsCountCore(currentCell);
			}
			return result;
		}
		protected internal virtual int GetSelectedRowsCountCore(TableCell cell) {
			switch (cell.VerticalMerging) {
				case MergingState.Restart:
					return GetMergedCellsCount(cell);
				case MergingState.Continue:
					return 0;
				default: 
					return 1;
			}
		}
		protected internal virtual int GetMergedCellsCount(TableCell currentCell) {
			int startColumnIndex = TableCellVerticalBorderCalculator.GetStartColumnIndex(currentCell, false);
			return TableCellVerticalBorderCalculator.GetVerticalSpanCells(currentCell, startColumnIndex, false).Count;
		}
		protected internal virtual void AddSelectedCells(TableRow row, int start, int end) {
			innerList.Add(new SelectedCellsIntervalInRow(row, start, end));
		}
		protected internal virtual void Clear() {
			innerList.Clear();
		}
		protected internal virtual void Add(SelectedCellsIntervalInRow item) {
			innerList.Add(item);
		}
		protected internal virtual void Remove(SelectedCellsIntervalInRow item) {
			innerList.Remove(item);
		}
		public void SetFirstSelectedCell(TableCell startCell, DocumentLogPosition pos) {
			int startCellIndex = startCell.IndexInRow;
			Add(new SelectedCellsIntervalInRow(startCell.Row, startCellIndex, startCellIndex));
			SetOriginalStartLogPosition(pos);
		}
		public void SetOriginalStartLogPosition(DocumentLogPosition pos) {
			this.originalStartLogPosition = pos;
		}
		public List<TableRow> GetSelectedTableRows() {
			List<TableRow> result = new List<TableRow>();
			int bottomRowIndex = GetBottomRowIndex();
			for (int i = GetTopRowIndex(); i <= bottomRowIndex; i++) {
				TableRow row = innerList[i].Row;
				result.Add(row);
			}
			return result;
		}
		protected internal virtual bool IsSelectedEntireTable() {
			if (RowsCount == 0)
				return false;
			if (NormalizedFirst.NormalizedStartCell.IsFirstCellInTable && NormalizedLast.NormalizedEndCell.IsLastCellInTable)
				return true;
			return false;
		}
		protected internal virtual bool IsSelectedEntireTableRows() {
			if (RowsCount == 0)
				return false;
			int bottomRowIndex = GetBottomRowIndex();
			for (int i = GetTopRowIndex(); i <= bottomRowIndex; i++) {
				SelectedCellsIntervalInRow currentInterval = innerList[i];
				if (!currentInterval.NormalizedStartCell.IsFirstCellInRow || !currentInterval.NormalizedEndCell.IsLastCellInRow)
					return false;
			}
			return true;
		}
		protected internal virtual bool IsSelectedEntireTableColumns() {
			if (RowsCount == 0)
				return false;
			if (RowsCount == FirstSelectedCell.Table.Rows.Count)
				return true;
			return false;
		}
		public virtual bool IsWholeCellSelected(TableCell cell) {
			if (this.First.Table != cell.Table)
				return false;
			if(SelectedOnlyOneCell) {
				TableCell selectedCell = First.NormalizedStartCell;
				if (selectedCell != cell)
					return false;
				DocumentModel documentModel = selectedCell.DocumentModel;
				Selection selection = documentModel.Selection;
				PieceTable pieceTable = cell.PieceTable;
				if (selection.PieceTable != pieceTable)
					return false;
				ParagraphCollection paragraphs = pieceTable.Paragraphs;
				Paragraph startParagraph = paragraphs[cell.StartParagraphIndex];
				Paragraph endParagraph = paragraphs[cell.EndParagraphIndex];
				int cellContentLength = endParagraph.EndLogPosition - startParagraph.LogPosition + 1;
				return selection.NormalizedStart == startParagraph.LogPosition && selection.Length == cellContentLength;
			}
			TableRow row = cell.Row;
			int count = this.RowsCount;
			for (int i = 0; i < count; i++) {
				SelectedCellsIntervalInRow selectedCellsInRow = this[i];
				if (selectedCellsInRow.Row != row)
					continue;
				return selectedCellsInRow.ContainsCell(cell);
			}
			return false;
		}
	}
	#endregion
	#region StartSelectedCellInTable
	public class StartSelectedCellInTable : ISelectedTableStructureBase {
		TableCell firstSelectedCell;
		DocumentLogPosition originalStartLogPostition;
		public StartSelectedCellInTable() {
		}
		public StartSelectedCellInTable(ISelectedTableStructureBase old)
			: this(old.FirstSelectedCell, old.OriginalStartLogPosition) {
		}
		public StartSelectedCellInTable(TableCell firstSelectedCell, DocumentLogPosition originalStartLogPosition) {
			this.firstSelectedCell = firstSelectedCell;
			this.originalStartLogPostition = originalStartLogPosition;
		}
		#region ISelectedTableStructureBase Members
		public TableCell FirstSelectedCell { get { return firstSelectedCell; } }
		public bool IsNotEmpty { get { return firstSelectedCell != null; } }
		public DocumentLogPosition OriginalStartLogPosition { get { return originalStartLogPostition; } }
		public void SetFirstSelectedCell(TableCell startCell, DocumentLogPosition pos) {
			this.firstSelectedCell = startCell;
			this.originalStartLogPostition = pos;
		}
		public int RowsCount { get { return (firstSelectedCell == null) ? 0 : 1; } }
		public bool SelectedOnlyOneCell { get { return true; } }
		#endregion
		public ISelectedTableStructureBase CloneWithShift(PieceTable targetPieceTable, int delta) {
			ParagraphIndex originalParagraphIndex = firstSelectedCell.StartParagraphIndex;
			Paragraph paragraph = firstSelectedCell.PieceTable.Paragraphs[originalParagraphIndex];
			DocumentLogPosition position = paragraph.LogPosition + delta;
			TableCell cell = targetPieceTable.FindParagraph(position).GetCell();
			return new StartSelectedCellInTable(cell, originalStartLogPostition + delta);
		}
		public void CopyProperties(ISelectedTableStructureBase selectedCells) {
			TableCell currentCell = selectedCells.FirstSelectedCell;
			currentCell.Properties.CopyFrom(this.FirstSelectedCell.Properties);
		}
	}
	#endregion
	#region TableStructureBySelectionCalculator
	public class TableStructureBySelectionCalculator {
		#region Fields
		readonly PieceTable pieceTable;
		#endregion
		public TableStructureBySelectionCalculator(PieceTable pieceTable) {
			this.pieceTable = pieceTable;
		}
		#region Properties
		public PieceTable ActivePieceTable { get { return pieceTable; } }
		#endregion
		bool CalculateSelectedCells(SelectedCellsCollection result, TableRow row, int leftColumnIndex, int rightColumnIndex, bool isLeftToRight) {
			int columnIndex = row.GridBefore;
			int start = Int32.MaxValue;
			int end = Int32.MinValue;
			int count = row.Cells.Count;
			for (int cellIndex = 0; cellIndex != count; cellIndex++) {
				columnIndex += row.Cells[cellIndex].ColumnSpan;
				if (leftColumnIndex < columnIndex)
					start = Math.Min(start, cellIndex);
				if (rightColumnIndex < columnIndex) {
					end = Math.Max(end, cellIndex);
					break;
				}
				end = Math.Max(end, cellIndex);
			}
			if (start == Int32.MaxValue || end == Int32.MinValue)
				return false;
			if (isLeftToRight)
				result.AddSelectedCells(row, start, end);
			else
				result.AddSelectedCells(row, end, start);
			return true;
		}
		#region SetStartCell
		public ISelectedTableStructureBase SetStartCell(DocumentLogPosition pos) {
			TableCell startCell = ActivePieceTable.FindParagraph(pos).GetCell();
			if (startCell == null)
				return new StartSelectedCellInTable(null, pos);
			SelectedCellsCollection result = new SelectedCellsCollection();
			result.SetFirstSelectedCell(startCell, pos);
			return result;
		}
		#endregion
		#region Calculate
		public SelectedCellsCollection Calculate(TableCell startCell, TableCell endCell) {
			return Calculate(startCell, endCell, false);
		}
		public SelectedCellsCollection Calculate(TableCell startCell, TableCell endCell, bool isColumnSelected) {
			Table table = null;
			if (startCell.Table == endCell.Table)
				table = startCell.Table;
			else
				Exceptions.ThrowInternalException();
			return CalculateCore(startCell, endCell, table, isColumnSelected);
		}
		SelectedCellsCollection CalculateCore(TableCell startCell, TableCell endCell, Table table, bool isColumnSelected) {
			int startCellColumnIndex = startCell.GetStartColumnIndexConsiderRowGrid();
			int endCellColumnIndex = endCell.GetStartColumnIndexConsiderRowGrid();
			int startCellRowIndex = startCell.RowIndex;
			int endCellRowIndex = endCell.RowIndex;
			bool isLeftToRightDirection;
			if (endCellRowIndex != startCellRowIndex)
				isLeftToRightDirection = endCellRowIndex >= startCellRowIndex;
			else
				isLeftToRightDirection = endCellColumnIndex >= startCellColumnIndex;
			int normalizedLeft = Math.Min(startCellColumnIndex, endCellColumnIndex);
			int normalizedRight = Math.Max(startCellColumnIndex, endCellColumnIndex);
			if (isColumnSelected)
				endCellRowIndex = startCell.Table.Rows.Count - 1;
			SelectedCellsCollection result = new SelectedCellsCollection();
			if (isLeftToRightDirection) { 
				int rightColumnIndex = isColumnSelected ? normalizedRight + startCell.ColumnSpan - 1 : normalizedRight + endCell.ColumnSpan - 1;
				for (int i = startCellRowIndex; i <= endCellRowIndex; i++)
					if (!CalculateSelectedCells(result, table.Rows[i], normalizedLeft, rightColumnIndex, isLeftToRightDirection))
						break;
			}
			else { 
				int rightColumnIndex = normalizedRight + startCell.ColumnSpan - 1;
				for (int i = startCellRowIndex; i >= endCellRowIndex; i--)
					if (!CalculateSelectedCells(result, table.Rows[i], normalizedLeft, rightColumnIndex, isLeftToRightDirection))
						break;
			}
			return result;
		}
		#endregion
	} 
	#endregion
}
