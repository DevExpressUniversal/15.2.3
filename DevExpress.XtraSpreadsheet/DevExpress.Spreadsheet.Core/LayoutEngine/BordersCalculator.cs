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
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Export.Xl;
using DevExpress.Compatibility.System.Drawing;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Layout.Engine {
	#region BorderLineEnumeratorInfo
	public class BorderLineEnumeratorInfo {
		int secondaryModelIndex;
		BorderSideAccessor accessor;
		public BorderLineEnumeratorInfo(int secondaryModelIndex, BorderSideAccessor accessor) {
			this.secondaryModelIndex = secondaryModelIndex;
			this.accessor = accessor;
		}
		public int SecondaryModelIndex { get { return secondaryModelIndex; } }
		public BorderSideAccessor Accessor { get { return accessor; } }
	}
	#endregion
	#region BordersInfo
	public class BordersInfo {
		#region Fields
		readonly List<BorderLineBox> cellBorders = new List<BorderLineBox>();
		readonly List<BorderLineBox> gridBorders = new List<BorderLineBox>();
		#endregion
		#region Properties
		public List<BorderLineBox> CellBorders { get { return cellBorders; } }
		public List<BorderLineBox> GridBorders { get { return gridBorders; } }
		public int Count { get { return CellBorders.Count + GridBorders.Count; } }
		#endregion
		public void AddBorderLineBox(BorderLineBox box) {
			if (HasGridStyle(box))
				gridBorders.Add(box);
			else
				cellBorders.Add(box);
		}
		public void AddBorderLineBox(BorderLineBox addedBox, BorderLineBox lastBox) {
			bool hasLastGridStyle = HasGridStyle(lastBox);
			if (HasGridStyle(addedBox)) {
				if (!hasLastGridStyle || !CheckEqualsGridLine(addedBox, lastBox))
					gridBorders.Add(addedBox);
			} else {
				if (hasLastGridStyle || CheckCellDifferences(addedBox, lastBox))
					cellBorders.Add(addedBox);
			}
		}
		bool HasGridStyle(BorderLineBox box) {
			return box.LineStyle == SpecialBorderLineStyle.DefaultGrid || box.LineStyle == SpecialBorderLineStyle.PrintGrid;
		}
		bool CheckEqualsGridLine(BorderLineBox addedBox, BorderLineBox lastBox) {
			return addedBox.LastGridIndex == lastBox.LastGridIndex;
		}
		bool CheckCellDifferences(BorderLineBox addedBox, BorderLineBox lastBox) {
			return
				addedBox.LineStyle != lastBox.LineStyle || addedBox.ColorIndex != lastBox.ColorIndex ||
				addedBox.FirstGridIndex > lastBox.LastGridIndex;
		}
	}
	#endregion
	#region PageBordersCalculator (abstract class)
	public abstract class PageBordersCalculator {
		#region Fields
		static readonly BordersInfo empty = new BordersInfo();
		readonly Page page;
		readonly Page previousPage;
		readonly Page nextPage;
		readonly Worksheet sheet;
		bool printGridLines;
		IActualBorderInfo gridLinesBorder;
		#endregion
		protected PageBordersCalculator(Page page, Page previousPage, Page nextPage) {
			Guard.ArgumentNotNull(page, "page");
			this.page = page;
			this.previousPage = previousPage;
			this.nextPage = nextPage;
			this.sheet = (Worksheet)page.Sheet;
		}
		#region Properties
		protected BordersInfo Empty { get { return empty; } }
		protected Page Page { get { return page; } }
		protected Page PreviousPage { get { return previousPage; } }
		protected Page NextPage { get { return nextPage; } }
		protected Worksheet Sheet { get { return sheet; } }
		public bool PrintGridLines { get { return printGridLines; } set { printGridLines = value; } }
		public IActualBorderInfo GridLinesBorder { get { return gridLinesBorder == null ? sheet.ActiveView.DefaultGridBorder : gridLinesBorder; } set { gridLinesBorder = value; } }
		public IActualBorderInfo EmptyBorder { get { return sheet.ActiveView.EmptyBorder; } }
		protected abstract SpreadsheetDirection Direction { get; }
		#endregion
		public void CalculateBorders() {
			PageGrid grid = Direction.GetSecondaryGrid(Page);
			int lastIndex = grid.ActualLastIndex + 1; 
			for (int i = grid.ActualFirstIndex; i <= lastIndex; i++) { 
				BordersInfo borders = CalculateBorders(i);
				if (borders.Count > 0)
					AppendBorder(i, borders);
			}
		}
		protected BorderLineEnumeratorInfo CalculatePrimaryEnumeratorInfo(int gridIndex) {
			if (gridIndex >= Direction.GetSecondaryGrid(Page).Count) {
				if (NextPage == null)
					return GetBorderLineEnumeratorInfo(Page, Direction.GetSecondaryGrid(Page).Count - 1, Direction.FarSecondaryBorderAccessor);
				else
					return GetBorderLineEnumeratorInfo(NextPage, 0, Direction.NearSecondaryBorderAccessor);
			}
			else
				return GetBorderLineEnumeratorInfo(Page, gridIndex, Direction.NearSecondaryBorderAccessor);
		}
		protected BorderLineEnumeratorInfo CalculateSecondaryEnumeratorInfo(int gridIndex) {
			if (gridIndex == 0) {
				if (PreviousPage == null)
					return null;
				else
					return GetBorderLineEnumeratorInfo(PreviousPage, Direction.GetSecondaryGrid(PreviousPage).Last.Index, Direction.FarSecondaryBorderAccessor);
			}
			else if (gridIndex >= Direction.GetSecondaryGrid(Page).Count) {
				if (NextPage == null)
					return null;
				else
					return GetBorderLineEnumeratorInfo(Page, Direction.GetSecondaryGrid(Page).Last.Index, Direction.FarSecondaryBorderAccessor);
			}
			else {
				return GetBorderLineEnumeratorInfo(Page, gridIndex - 1, Direction.FarSecondaryBorderAccessor);
			}
		}
		BorderLineEnumeratorInfo GetBorderLineEnumeratorInfo(Page page, int gridIndex, BorderSideAccessor accessor) {
			PageGridItem gridItem = Direction.GetSecondaryGrid(page)[gridIndex];
			return new BorderLineEnumeratorInfo(gridItem.ModelIndex, accessor);
		}
		protected IEnumerator<BorderLineBox> CreateBorderLineEnumerator(BorderLineEnumeratorInfo primaryInfo, BorderLineEnumeratorInfo secondaryInfo) {
			Debug.Assert(primaryInfo != null);
			if (secondaryInfo == null) {
				using (CellsOrderedEnumeratorInfo cellsEnumeratorInfo = GetCells(primaryInfo)) {
					IEnumerator<ICell> cells = cellsEnumeratorInfo.Enumerator;
					if (cells == null)
						return null;
					UpdateInfinityModelIndices(cellsEnumeratorInfo);
					return new SingleBorderLineEnumerator(Direction, cells, primaryInfo.Accessor, cellsEnumeratorInfo.InfinityStartModelIndex, cellsEnumeratorInfo.InfinityEndModelIndex);
				}
			}
			else {
				using (CellsOrderedEnumeratorInfo primaryCellsEnumeratorInfo = GetCells(primaryInfo)) {
					using (CellsOrderedEnumeratorInfo secondaryCellsEnumeratorInfo = GetCells(secondaryInfo)) {
						IEnumerator<ICell> primaryCells = primaryCellsEnumeratorInfo.Enumerator;
						IEnumerator<ICell> secondaryCells = secondaryCellsEnumeratorInfo.Enumerator;
						if (primaryCells != null && secondaryCells != null) {
							UpdateInfinityModelIndices(primaryCellsEnumeratorInfo);
							UpdateInfinityModelIndices(secondaryCellsEnumeratorInfo);
							Debug.Assert(primaryCellsEnumeratorInfo.InfinityEndModelIndex == secondaryCellsEnumeratorInfo.InfinityEndModelIndex);
							return new BorderLineEnumerator(Direction, primaryCells, primaryInfo.Accessor, secondaryCells, secondaryInfo.Accessor, Math.Max(primaryCellsEnumeratorInfo.InfinityStartModelIndex, secondaryCellsEnumeratorInfo.InfinityStartModelIndex), primaryCellsEnumeratorInfo.InfinityEndModelIndex);
						}
						else if (primaryCells == null && secondaryCells == null)
							return null;
						else if (primaryCells != null) {
							UpdateInfinityModelIndices(primaryCellsEnumeratorInfo);
							return new SingleBorderLineEnumerator(Direction, primaryCells, primaryInfo.Accessor, primaryCellsEnumeratorInfo.InfinityStartModelIndex, primaryCellsEnumeratorInfo.InfinityEndModelIndex);
						}
						else {
							UpdateInfinityModelIndices(secondaryCellsEnumeratorInfo);
							return new SingleBorderLineEnumerator(Direction, secondaryCells, secondaryInfo.Accessor, secondaryCellsEnumeratorInfo.InfinityStartModelIndex, secondaryCellsEnumeratorInfo.InfinityEndModelIndex);
						}
					}
				}
			}
		}
		protected void UpdateInfinityModelIndices(CellsOrderedEnumeratorInfo info) {
			PageGrid primaryGrid = Direction.GetPrimaryGrid(Page);
			info.InfinityEndModelIndex = Math.Min(info.InfinityEndModelIndex, primaryGrid.Last.ModelIndex);
			info.InfinityStartModelIndex = Math.Min(info.InfinityStartModelIndex, primaryGrid.Last.ModelIndex + 1);
			info.InfinityEndModelIndex = Math.Max(info.InfinityStartModelIndex, info.InfinityEndModelIndex);
		}
		protected abstract CellsOrderedEnumeratorInfo GetCells(BorderLineEnumeratorInfo info);
		public BordersInfo CalculateBorders(int secondaryGridIndex) {
			BorderLineEnumeratorInfo primaryInfo = CalculatePrimaryEnumeratorInfo(secondaryGridIndex);
			Debug.Assert(primaryInfo != null);
			BorderLineEnumeratorInfo secondaryInfo = CalculateSecondaryEnumeratorInfo(secondaryGridIndex);
			IEnumerator<BorderLineBox> enumerator = CreateBorderLineEnumerator(primaryInfo, secondaryInfo);
			if (enumerator == null)
				return Empty;
			return CalculateBordersCore(secondaryGridIndex, new Enumerable<BorderLineBox>(enumerator));
		}
		protected BordersInfo CalculateBordersCore(int secondaryGridIndex, IEnumerable<BorderLineBox> boxes) {
			BorderLineBox currentBox = new BorderLineBox(); 
			if (PrintGridLines) {
				currentBox.LineStyle = GridLinesBorder.LeftLineStyle;
				currentBox.ColorIndex = GridLinesBorder.LeftColorIndex;
			}
			currentBox.LastGridIndex = -1;
			BorderLineBox? lastBox = null;
			PageGrid pagePrimaryGrid = Direction.GetPrimaryGrid(Page);
			BordersInfo result = new BordersInfo();
			foreach (BorderLineBox box in boxes) {
				int modelIndex = box.FirstGridIndex;
				XlBorderLineStyle currentLineStyle = box.LineStyle;
				int currentLineColorIndex = box.ColorIndex;
				if (currentLineStyle == XlBorderLineStyle.None && PrintGridLines) {
					currentLineStyle = GridLinesBorder.LeftLineStyle;
					currentLineColorIndex = GridLinesBorder.LeftColorIndex;
				}
				int currentGridIndex = pagePrimaryGrid.TryCalculateIndex(modelIndex);
				currentLineStyle = ApplyBorderBreak(currentLineStyle, modelIndex, secondaryGridIndex);
				if (currentLineStyle != currentBox.LineStyle || currentLineColorIndex != currentBox.ColorIndex || currentBox.LastGridIndex != currentGridIndex - 1) {
					if (!IsEmptyLineStyle(currentBox.LineStyle)) { 
						if (currentBox.LastGridIndex < 0) {
							Debug.Assert(PrintGridLines);
							currentBox.LastGridIndex = currentGridIndex - 1;
						}
						if (currentBox.LastGridIndex >= 0) {
							result.AddBorderLineBox(currentBox);
							lastBox = currentBox;
						}
					}
					currentBox.FirstGridIndex = Math.Max(0, currentGridIndex);
					currentBox.LineStyle = currentLineStyle;
					currentBox.ColorIndex = currentLineColorIndex;
				}
				if (box.FirstGridIndex == box.LastGridIndex)
					currentBox.LastGridIndex = currentGridIndex;
				else
					currentBox.LastGridIndex = pagePrimaryGrid.CalculateIndex(box.LastGridIndex);
			}
			if (!IsEmptyLineStyle(currentBox.LineStyle)) { 
				if (currentBox.LastGridIndex < 0) {
					Debug.Assert(PrintGridLines);
					currentBox.LastGridIndex = pagePrimaryGrid.Count - 1;
				}
				if (currentBox.LastGridIndex >= 0) {
					if (!lastBox.HasValue)
						result.AddBorderLineBox(currentBox);
					else
						result.AddBorderLineBox(currentBox, lastBox.Value);
				}
			}
			return result.Count == 0 ? Empty : result;
		}
		bool IsEmptyLineStyle(XlBorderLineStyle lineStyle) {
			return lineStyle == XlBorderLineStyle.None || lineStyle == SpecialBorderLineStyle.ForcedNone || lineStyle == SpecialBorderLineStyle.NoneOverrideDefaultGrid;
		}
		protected abstract XlBorderLineStyle ApplyBorderBreak(XlBorderLineStyle originalBorderLine, int primaryModelIndex, int secondaryGridIndex);
		protected abstract void AppendBorder(int gridIndex, BordersInfo borders);
	}
	#endregion
	#region HorizontalBordersCalculator
	public class HorizontalBordersCalculator : PageBordersCalculator {
		public HorizontalBordersCalculator(Page page, Page previousPage, Page nextPage)
			: base(page, previousPage, nextPage) {
		}
		protected override SpreadsheetDirection Direction { get { return SpreadsheetDirection.Horizontal; } }
		protected override void AppendBorder(int gridIndex, BordersInfo borders) {
			if (borders.CellBorders.Count > 0)
				Page.HorizontalBorders.Add(new PageHorizontalBorders(gridIndex, borders.CellBorders));
			if (borders.GridBorders.Count > 0)
				Page.HorizontalGridBorders.Add(new PageHorizontalBorders(gridIndex, borders.GridBorders));
		}
		protected override CellsOrderedEnumeratorInfo GetCells(BorderLineEnumeratorInfo info) {
			IActualBorderInfo borderInfo = PrintGridLines ? GridLinesBorder : EmptyBorder;
			return CreateEnumeratorInfo(GetCellsPrintGridLines(info, borderInfo));
		}
		CellsOrderedEnumeratorInfo CreateEnumeratorInfo(IOrderedEnumerator<ICell> enumerator) {
			CellsOrderedEnumeratorInfo result = new CellsOrderedEnumeratorInfo(enumerator);
			result.InfinityStartModelIndex = Int32.MaxValue;
			result.InfinityEndModelIndex = Int32.MaxValue;
			return result;
		}
		IOrderedEnumerator<ICell> GetCellsPrintGridLines(BorderLineEnumeratorInfo info, IActualBorderInfo borderInfo) {
			PageGrid primaryGrid = Direction.GetPrimaryGrid(Page);
			Row row = Sheet.Rows.TryGetRow(info.SecondaryModelIndex);
			if (row == null) {
				return new CellsEnumeratorProviderFakeActualBorderForNonExistingCells(new List<ICell>(), primaryGrid.ActualFirst.ModelIndex, primaryGrid.ActualLast.ModelIndex, false, Sheet, info.SecondaryModelIndex, borderInfo);
			}
			else
				return row.Cells.GetAllCellsProvideFakeActualBorderEnumerator(primaryGrid.ActualFirst.ModelIndex, primaryGrid.ActualLast.ModelIndex, false, row.Index, borderInfo);
		}
		internal static IOrderedEnumerator<ICell> GetCellsSimple(int rowIndex, Worksheet sheet, int leftColumn, int rightColumn, IActualBorderInfo gridLinesBorder) {
			Row row = sheet.Rows.TryGetRow(rowIndex);
			if (row == null)
				return null;
			if (row.HasVisibleBorder) {
				IOrderedEnumerator<ICell> existingCells = row.Cells.GetExistingCellsEnumerator(leftColumn, rightColumn, false);
				IOrderedEnumerator<ICell> continousCells = new ContinuousCellsEnumeratorProvideRowBorder(row.Cells.InnerList, leftColumn, rightColumn, false, sheet, row.Index);
				return new JoinedOrderedEnumerator<ICell>(existingCells, continousCells);
			}
			else {
				IOrderedEnumerator<ICell> existingCells = row.Cells.GetExistingCellsEnumerator(leftColumn, rightColumn, false);
				return new CellsEnumeratorReplaceBordersInsideMergedCell(existingCells, new CellRange(sheet, new CellPosition(leftColumn, row.Index), new CellPosition(rightColumn, row.Index)), gridLinesBorder);
			}
		}
		protected override XlBorderLineStyle ApplyBorderBreak(XlBorderLineStyle originalBorderLine, int primaryModelIndex, int secondaryGridIndex) {
			return originalBorderLine;
		}
	}
	#endregion
	#region VerticalBordersCalculator
	public class VerticalBordersCalculator : PageBordersCalculator {
		readonly RowVerticalBorderBreaks verticalBorderBreaks;
		public VerticalBordersCalculator(Page page, Page previousPage, Page nextPage, RowVerticalBorderBreaks verticalBorderBreaks)
			: base(page, previousPage, nextPage) {
			this.verticalBorderBreaks = verticalBorderBreaks;
		}
		protected override SpreadsheetDirection Direction { get { return SpreadsheetDirection.Vertical; } }
		protected override void AppendBorder(int gridIndex, BordersInfo borders) {
			if (borders.CellBorders.Count > 0)
				Page.VerticalBorders.Add(new PageVerticalBorders(gridIndex, borders.CellBorders));
			if (borders.GridBorders.Count > 0)
				Page.VerticalGridBorders.Add(new PageVerticalBorders(gridIndex, borders.GridBorders));
		}
		protected override CellsOrderedEnumeratorInfo GetCells(BorderLineEnumeratorInfo info) {
			PageGrid primaryGrid = Direction.GetPrimaryGrid(Page);
			IActualBorderInfo borderInfo = PrintGridLines ? GridLinesBorder : EmptyBorder;
			return ColumnCollection.GetAllCellsProvideFakeActualBorderEnumeratorInfo(Sheet, info.SecondaryModelIndex, primaryGrid.ActualFirst.ModelIndex, primaryGrid.ActualLast.ModelIndex, false, borderInfo);
		}
		internal static CellsOrderedEnumeratorInfo GetCellsSimple(int columnIndex, Worksheet sheet, int topRow, int bottomRow, IActualBorderInfo baseActualBorder) {
			IColumnRange columnRange = sheet.Columns.TryGetColumnRange(columnIndex);
			if (columnRange != null && columnRange.HasVisibleBorder)
				return ColumnCollection.GetAllCellsProvideColumnBorderEnumeratorInfo(sheet, columnIndex, topRow, bottomRow, false);
			else {
				CellsOrderedEnumeratorInfo result = ColumnCollection.GetExistingCellsEnumeratorInfo(sheet, columnIndex, topRow, bottomRow, false);
				result.Enumerator = new CellsEnumeratorReplaceBordersInsideMergedCell(result.Enumerator, new CellRange(sheet, new CellPosition(columnIndex, topRow), new CellPosition(columnIndex, bottomRow)), baseActualBorder);
				return result;
			}
		}
		protected override XlBorderLineStyle ApplyBorderBreak(XlBorderLineStyle originalBorderLine, int primaryModelIndex,  int secondaryGridIndex) {
			if (verticalBorderBreaks == null)
				return originalBorderLine;
			if (originalBorderLine == XlBorderLineStyle.None)
				return originalBorderLine;
			List<int> breaks;
			if (!verticalBorderBreaks.TryGetValue(primaryModelIndex, out breaks))
				return originalBorderLine;
			if (breaks.BinarySearch(secondaryGridIndex) >= 0)
				return XlBorderLineStyle.None;
			else
				return originalBorderLine;
		}
	}
	#endregion
	#region SpreadsheetDirection (abstract class)
	public abstract class SpreadsheetDirection {
		static readonly SpreadsheetDirection horizontal = new SpreadsheetHorizontalDirection();
		static readonly SpreadsheetDirection vertical = new SpreadsheetVerticalDirection();
		public static SpreadsheetDirection Horizontal { get { return horizontal; } }
		public static SpreadsheetDirection Vertical { get { return vertical; } }
		public abstract BorderSideAccessor NearPrimaryBorderAccessor { get; }
		public abstract BorderSideAccessor FarPrimaryBorderAccessor { get; }
		public abstract BorderSideAccessor NearSecondaryBorderAccessor { get; }
		public abstract BorderSideAccessor FarSecondaryBorderAccessor { get; }
		public abstract int GetPrimaryDirectionModelIndex(ICell cell);
		public abstract bool IsPrimaryContainerVisible(ICell cell);
		public abstract PageGrid GetPrimaryGrid(Page page);
		public abstract PageGrid GetSecondaryGrid(Page page);
	}
	#endregion
	#region SpreadsheetHorizontalDirection
	public class SpreadsheetHorizontalDirection : SpreadsheetDirection {
		public override BorderSideAccessor NearPrimaryBorderAccessor { get { return BorderSideAccessor.Left; } }
		public override BorderSideAccessor FarPrimaryBorderAccessor { get { return BorderSideAccessor.Right; } }
		public override BorderSideAccessor NearSecondaryBorderAccessor { get { return BorderSideAccessor.Top; } }
		public override BorderSideAccessor FarSecondaryBorderAccessor { get { return BorderSideAccessor.Bottom; } }
		public override int GetPrimaryDirectionModelIndex(ICell cell) {
			return cell.ColumnIndex;
		}
		public override bool IsPrimaryContainerVisible(ICell cell) {
			IColumnRange columnRange = cell.Worksheet.Columns.TryGetColumnRange(cell.ColumnIndex); 
			if (columnRange == null || columnRange.IsVisible)
				return true;
			else
				return false;
		}
		public override PageGrid GetPrimaryGrid(Page page) {
			return page.GridColumns;
		}
		public override PageGrid GetSecondaryGrid(Page page) {
			return page.GridRows;
		}
	}
	#endregion
	#region SpreadsheetVerticalDirection
	public class SpreadsheetVerticalDirection : SpreadsheetDirection {
		public override BorderSideAccessor NearPrimaryBorderAccessor { get { return BorderSideAccessor.Top; } }
		public override BorderSideAccessor FarPrimaryBorderAccessor { get { return BorderSideAccessor.Bottom; } }
		public override BorderSideAccessor NearSecondaryBorderAccessor { get { return BorderSideAccessor.Left; } }
		public override BorderSideAccessor FarSecondaryBorderAccessor { get { return BorderSideAccessor.Right; } }
		public override int GetPrimaryDirectionModelIndex(ICell cell) {
			return cell.RowIndex;
		}
		public override bool IsPrimaryContainerVisible(ICell cell) {
			Row row = cell.Worksheet.Rows.TryGetRow(cell.RowIndex); 
			if (row == null || row.IsVisible)
				return true;
			else
				return false;
		}
		public override PageGrid GetPrimaryGrid(Page page) {
			return page.GridRows;
		}
		public override PageGrid GetSecondaryGrid(Page page) {
			return page.GridColumns;
		}
	}
	#endregion
	#region SingleBorderLineEnumerator
	public class SingleBorderLineEnumerator : IEnumerator<BorderLineBox> {
		readonly SpreadsheetDirection direction;
		readonly IEnumerator<ICell> primaryCells;
		readonly BorderSideAccessor primaryAccessor;
		readonly int infinityStartModelIndex;
		readonly int infinityEndModelIndex;
		BorderLineBox current;
		public SingleBorderLineEnumerator(SpreadsheetDirection direction, IEnumerator<ICell> primaryCells, BorderSideAccessor primaryAccessor, int infinityStartModelIndex, int infinityEndModelIndex) {
			Guard.ArgumentNotNull(direction, "direction");
			Guard.ArgumentNotNull(primaryCells, "primaryCells");
			Guard.ArgumentNotNull(primaryAccessor, "primaryAccessor");
			this.direction = direction;
			this.primaryCells = primaryCells;
			this.primaryAccessor = primaryAccessor;
			this.infinityStartModelIndex = infinityStartModelIndex;
			this.infinityEndModelIndex = infinityEndModelIndex;
		}
		#region IEnumerator<BorderLineBox> Members
		public BorderLineBox Current { get { return current; } }
		#endregion
		#region IDisposable Members
		public void Dispose() {
		}
		#endregion
		#region IEnumerator Members
		object IEnumerator.Current { get { return current; } }
		public bool MoveNext() {
			ICell cell = MoveToNextVisibleCell();
			if (cell == null)
				return false;
			int modelIndex = direction.GetPrimaryDirectionModelIndex(cell);
			if (modelIndex > infinityStartModelIndex)
				return false;
			IActualBorderInfo actualBorder = cell.ActualBorder;
			current.LineStyle = primaryAccessor.GetLineStyle(actualBorder);
			current.ColorIndex = primaryAccessor.GetLineColorIndex(actualBorder);
			if (modelIndex == infinityStartModelIndex) {
				current.FirstGridIndex = modelIndex;
				current.LastGridIndex = infinityEndModelIndex;
				Debug.Assert(current.FirstGridIndex <= current.LastGridIndex);
				return true;
			}
			else {
				current.FirstGridIndex = modelIndex;
				current.LastGridIndex = modelIndex;
				return true;
			}
		}
		public void Reset() {
			primaryCells.Reset();
		}
		#endregion
		ICell MoveToNextVisibleCell() {
			for (; ; ) {
				if (!primaryCells.MoveNext())
					return null;
				if (direction.IsPrimaryContainerVisible(primaryCells.Current))
					return primaryCells.Current;
			}
		}
	}
	#endregion
	#region BorderLineEnumerator
	public class BorderLineEnumerator : IEnumerator<BorderLineBox> {
		readonly SpreadsheetDirection direction;
		readonly BorderSideAccessor primaryAccessor;
		readonly BorderSideAccessor secondaryAccessor;
		readonly int infinityStartModelIndex;
		readonly int infinityEndModelIndex;
		IEnumerator<ICell> primaryCells;
		IEnumerator<ICell> secondaryCells;
		ICell primaryCell;
		ICell secondaryCell;
		BorderSideAccessor currentAccessor;
		BorderLineBox current;
		public BorderLineEnumerator(SpreadsheetDirection direction, IEnumerator<ICell> primaryCells, BorderSideAccessor primaryAccessor, IEnumerator<ICell> secondaryCells, BorderSideAccessor secondaryAccessor, int infinityStartModelIndex, int infinityEndModelIndex) {
			Guard.ArgumentNotNull(direction, "direction");
			Guard.ArgumentNotNull(primaryCells, "primaryCells");
			Guard.ArgumentNotNull(primaryAccessor, "primaryAccessor");
			Guard.ArgumentNotNull(secondaryCells, "secondaryCells");
			Guard.ArgumentNotNull(secondaryAccessor, "secondaryAccessor");
			this.direction = direction;
			this.primaryCells = primaryCells;
			this.primaryAccessor = primaryAccessor;
			this.secondaryCells = secondaryCells;
			this.secondaryAccessor = secondaryAccessor;
			this.infinityStartModelIndex = infinityStartModelIndex;
			this.infinityEndModelIndex = infinityEndModelIndex;
		}
		#region IEnumerator<BorderLineBox> Members
		public BorderLineBox Current { get { return current; } }
		#endregion
		#region IDisposable Members
		public void Dispose() {
		}
		#endregion
		#region IEnumerator Members
		object IEnumerator.Current { get { return current; } }
		public bool MoveNext() {
			ICell cell = MoveToNextVisibleCell();
			if (cell == null)
				return false;
			int correctionCell = cell.Worksheet.MergedCells.MaxCellHeight;
			int modelIndex = direction.GetPrimaryDirectionModelIndex(cell);
			if (modelIndex > Math.Max(infinityStartModelIndex + correctionCell, infinityEndModelIndex))
				return false;
			IActualBorderInfo actualBorder = cell.ActualBorder;
			current.LineStyle = currentAccessor.GetLineStyle(actualBorder);
			current.ColorIndex = currentAccessor.GetLineColorIndex(actualBorder);
			if (modelIndex == infinityStartModelIndex + correctionCell) {
				current.FirstGridIndex = modelIndex;
				current.LastGridIndex = infinityEndModelIndex;
				Debug.Assert(current.FirstGridIndex <= current.LastGridIndex);
				return true;
			}
			else {
				current.FirstGridIndex = modelIndex;
				current.LastGridIndex = modelIndex;
				return true;
			}
		}
		public void Reset() {
			primaryCells.Reset();
			secondaryCells.Reset();
		}
		#endregion
		ICell MoveToNextVisibleCell() {
			Debug.Assert(primaryCell == null || secondaryCell == null);
			if (primaryCell == null && secondaryCell == null) { 
				this.primaryCell = MoveToNextPrimaryCell();
				this.secondaryCell = MoveToNextSecondaryCell();
				if (primaryCell == null && secondaryCell == null)
					return null;
				return ReadCurrentCell();
			}
			else if (secondaryCell == null) {
				this.secondaryCell = MoveToNextSecondaryCell();
				return ReadCurrentCell();
			}
			else { 
				this.primaryCell = MoveToNextPrimaryCell();
				return ReadCurrentCell();
			}
		}
		ICell ReadCurrentCell() {
			if (primaryCell == null)
				return ReadSecondaryCell();
			if (secondaryCell == null)
				return ReadPrimaryCell();
			int primaryModelIndex = direction.GetPrimaryDirectionModelIndex(primaryCell);
			int secondaryModelIndex = direction.GetPrimaryDirectionModelIndex(secondaryCell);
			if (primaryModelIndex == secondaryModelIndex) {
				if (ShouldUsePrimaryCell(primaryCell, secondaryCell)) {
					ReadSecondaryCell();
					return ReadPrimaryCell();
				}
				else {
					ReadPrimaryCell();
					return ReadSecondaryCell();
				}
			}
			else if (primaryModelIndex < secondaryModelIndex)
				return ReadPrimaryCell();
			else
				return ReadSecondaryCell();
		}
		bool ShouldUsePrimaryCell(ICell primaryCell, ICell secondaryCell) {
			IActualBorderInfo primaryBorder = primaryCell.ActualBorder;
			XlBorderLineStyle primaryLineStyle = primaryAccessor.GetLineStyle(primaryBorder);
			if (primaryLineStyle == XlBorderLineStyle.None) 
				return false;
			XlBorderLineStyle secondaryLineStyle = secondaryAccessor.GetLineStyle(secondaryCell.ActualBorder);
			if (secondaryLineStyle == XlBorderLineStyle.None) 
				return true;
			int primaryLineWeight = GetLineWeight(primaryLineStyle);
			int secondaryLineWeight = GetLineWeight(secondaryLineStyle);
			if (primaryLineWeight > secondaryLineWeight)
				return true;
			if (primaryLineWeight < secondaryLineWeight)
				return false;
			Color primaryColor = primaryAccessor.GetLineColor(primaryBorder);
			Color secondaryColor = secondaryAccessor.GetLineColor(secondaryCell.ActualBorder);
			if (primaryColor.ToArgb() < secondaryColor.ToArgb())
				return true;
			if (primaryColor.ToArgb() > secondaryColor.ToArgb())
				return false;
			return primaryCell.ShouldUseInLayout;
		}
		int GetLineWeight(XlBorderLineStyle lineStyle) {
			if (lineStyle == SpecialBorderLineStyle.ForcedNone)
				return Int32.MaxValue;
			if (lineStyle == SpecialBorderLineStyle.DefaultGrid || lineStyle == SpecialBorderLineStyle.PrintGrid)
				return 1;
			if (lineStyle == SpecialBorderLineStyle.NoneOverrideDefaultGrid)
				return 2;
			return 3 * BorderInfo.LineWeightTable[lineStyle];
		}
		ICell ReadPrimaryCell() {
			this.currentAccessor = primaryAccessor;
			ICell result = primaryCell;
			this.primaryCell = null;
			return result;
		}
		ICell ReadSecondaryCell() {
			currentAccessor = secondaryAccessor;
			ICell result = secondaryCell;
			secondaryCell = null;
			return result;
		}
		ICell MoveToNextPrimaryCell() {
			ICell result = MoveToNextVisibleCellCore(primaryCells);
			if (result == null)
				primaryCells = null;
			return result;
		}
		ICell MoveToNextSecondaryCell() {
			ICell result = MoveToNextVisibleCellCore(secondaryCells);
			if (result == null)
				secondaryCells = null;
			return result;
		}
		ICell MoveToNextVisibleCellCore(IEnumerator<ICell> enumerator) {
			if (enumerator == null)
				return null;
			for (; ; ) {
				if (!enumerator.MoveNext())
					return null;
				if (direction.IsPrimaryContainerVisible(enumerator.Current))
					return enumerator.Current;
			}
		}
	}
	#endregion
	#region AdjacentBorderLineEnumerator
	public class AdjacentBorderLineEnumerator : IEnumerator<ICell> {
		readonly IEnumerator<ICell> primaryCells;
		readonly IEnumerator<ICell> secondaryCells;
		public AdjacentBorderLineEnumerator(IEnumerator<ICell> primaryCells, IEnumerator<ICell> secondaryCells) {
			Guard.ArgumentNotNull(primaryCells, "primaryCells");
			Guard.ArgumentNotNull(secondaryCells, "secondaryCells");
			this.primaryCells = primaryCells;
			this.secondaryCells = secondaryCells;
		}
		#region IEnumerator<ICell> Members
		public ICell Current { get { return primaryCells.Current; } }
		#endregion
		#region IDisposable Members
		public void Dispose() {
		}
		#endregion
		#region IEnumerator Members
		object IEnumerator.Current { get { return primaryCells.Current; } }
		public bool MoveNext() {
			primaryCells.MoveNext();
			return secondaryCells.MoveNext();
		}
		public void Reset() {
			primaryCells.Reset();
			secondaryCells.Reset();
		}
		#endregion
	}
	#endregion
}
