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

using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.Export.Xl;
using DevExpress.XtraSpreadsheet.Model.External;
using DevExpress.XtraSpreadsheet.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraSpreadsheet.Model {
	#region SelectedRangeTypeForBorderPreview
	public enum SelectedRangeTypeForBorderPreview {
		Cell = 0,
		Row = 1,
		Column = 2,
		Table = 3
	}
	#endregion
	#region CalculationRangeTypeForBorderPreview
	public class CalculationRangeTypeForBorderPreview {
		#region Fields
		IList<CellRange> selectedRanges;
		bool isRow;
		bool isColumn;
		#endregion
		public CalculationRangeTypeForBorderPreview(Worksheet sheet) {
			this.selectedRanges = sheet.Selection.SelectedRanges;
		}
		#region Properties
		public SelectedRangeTypeForBorderPreview RangeType { get { return CalculationTypeOfRange(); } }
		#endregion
		protected internal SelectedRangeTypeForBorderPreview CalculationTypeOfRange() {
			isRow = false;
			isColumn = false;
			foreach (CellRange range in selectedRanges) {
				if (range.TopRowIndex != range.BottomRowIndex && !range.IsMerged)
					isColumn = true;
				if (range.LeftColumnIndex != range.RightColumnIndex && !range.IsMerged)
					isRow = true;
			}
			if (isRow && isColumn)
				return SelectedRangeTypeForBorderPreview.Table;
			else if (isColumn)
				return SelectedRangeTypeForBorderPreview.Column;
			else if (isRow)
				return SelectedRangeTypeForBorderPreview.Row;
			else
				return SelectedRangeTypeForBorderPreview.Cell;
		}
	}
	#endregion
	#region CalculationBordersBase
	public class CalculationBordersBase {
		#region Fields
		ICell topLeftCell;
		ICell bottomLeftCell;
		ICell topRightCell;
		ICell bottomRightCell;
		bool applyDiagonalDownColor = false;
		bool applyDiagonalUpColor = false;
		#endregion
		#region Properties
		public ICell TopLeftCell {
			get { return topLeftCell; }
			set {
				if (TopLeftCell == value)
					return;
				topLeftCell = value;
			}
		}
		public ICell BottomLeftCell {
			get { return bottomLeftCell; }
			set {
				if (BottomLeftCell == value)
					return;
				bottomLeftCell = value;
			}
		}
		public ICell TopRightCell {
			get { return topRightCell; }
			set {
				if (TopRightCell == value)
					return;
				topRightCell = value;
			}
		}
		public ICell BottomRightCell {
			get { return bottomRightCell; }
			set {
				if (BottomRightCell == value)
					return;
				bottomRightCell = value;
			}
		}
		#endregion
		protected void SetLeftBorder(IActualBorderInfo info, MergedBorderInfo borderInfo, MergedBorderOptionsInfo optionsInfo) {
			SetOutlineBorder(info, MergedBorderInfoAccessor.Left, BorderSideAccessor.Left, borderInfo, optionsInfo);
		}
		protected void SetRightBorder(IActualBorderInfo info, MergedBorderInfo borderInfo, MergedBorderOptionsInfo optionsInfo) {
			SetOutlineBorder(info, MergedBorderInfoAccessor.Right, BorderSideAccessor.Right, borderInfo, optionsInfo);
		}
		protected void SetTopBorder(IActualBorderInfo info, MergedBorderInfo borderInfo, MergedBorderOptionsInfo optionsInfo) {
			SetOutlineBorder(info, MergedBorderInfoAccessor.Top, BorderSideAccessor.Top, borderInfo, optionsInfo);
		}
		protected void SetBottomBorder(IActualBorderInfo info, MergedBorderInfo borderInfo, MergedBorderOptionsInfo optionsInfo) {
			SetOutlineBorder(info, MergedBorderInfoAccessor.Bottom, BorderSideAccessor.Bottom, borderInfo, optionsInfo);
		}
		protected void SetDiagonalBorders(IActualBorderInfo info, MergedBorderInfo borderInfo, MergedBorderOptionsInfo optionsInfo) {
			if (borderInfo.DiagonalUpLineStyle != SpecialBorderLineStyle.OutsideComplexBorder || borderInfo.DiagonalDownLineStyle != SpecialBorderLineStyle.OutsideComplexBorder) {
				XlBorderLineStyle actualUpStyle = info.DiagonalUpLineStyle;
				XlBorderLineStyle actualDownStyle = info.DiagonalDownLineStyle;
				Color actualColor = info.DiagonalColor;
				bool hasDiagonalUpStyle = borderInfo.DiagonalUpLineStyle.HasValue;
				bool hasDiagonalDownStyle = borderInfo.DiagonalDownLineStyle.HasValue;
				if (!hasDiagonalUpStyle && !hasDiagonalDownStyle && actualUpStyle != XlBorderLineStyle.None && actualDownStyle != XlBorderLineStyle.None) {
					borderInfo.DiagonalUpLineStyle = actualUpStyle;
					borderInfo.DiagonalDownLineStyle = actualDownStyle;
					borderInfo.DiagonalColor = actualColor;
					optionsInfo.ApplyDiagonalUpBorder = true;
					optionsInfo.ApplyDiagonalDownBorder = true;
					applyDiagonalUpColor = true;
					applyDiagonalDownColor = true;
				}
				if (!hasDiagonalUpStyle && !hasDiagonalDownStyle && actualUpStyle != XlBorderLineStyle.None && actualDownStyle == XlBorderLineStyle.None) {
					borderInfo.DiagonalUpLineStyle = actualUpStyle;
					borderInfo.DiagonalColor = actualColor;
					applyDiagonalUpColor = true;
					optionsInfo.ApplyDiagonalUpBorder = true;
				}
				if (!hasDiagonalUpStyle && !hasDiagonalDownStyle && actualUpStyle == XlBorderLineStyle.None && actualDownStyle != XlBorderLineStyle.None) {
					borderInfo.DiagonalDownLineStyle = actualDownStyle;
					borderInfo.DiagonalColor = actualColor;
					applyDiagonalDownColor = true;
					optionsInfo.ApplyDiagonalDownBorder = true;
				}
				if (hasDiagonalUpStyle && (actualUpStyle != borderInfo.DiagonalUpLineStyle.Value || (actualColor != borderInfo.DiagonalColor.Value && applyDiagonalUpColor))) {
					borderInfo.DiagonalUpLineStyle = SpecialBorderLineStyle.OutsideComplexBorder;
					borderInfo.DiagonalColor = Color.Empty;
					optionsInfo.ApplyDiagonalUpBorder = false;
				}
				if (hasDiagonalDownStyle && (actualDownStyle != borderInfo.DiagonalDownLineStyle.Value || (actualColor != borderInfo.DiagonalColor.Value && applyDiagonalDownColor))) {
					borderInfo.DiagonalDownLineStyle = SpecialBorderLineStyle.OutsideComplexBorder;
					borderInfo.DiagonalColor = Color.Empty;
					optionsInfo.ApplyDiagonalDownBorder = false;
				}
				if (!hasDiagonalUpStyle && actualUpStyle == XlBorderLineStyle.None) {
					borderInfo.DiagonalUpLineStyle = XlBorderLineStyle.None;
					if (borderInfo.DiagonalColor == null) {
						borderInfo.DiagonalColor = Color.Empty;
						applyDiagonalUpColor = false;
					}
					optionsInfo.ApplyDiagonalUpBorder = false;
				}
				if (!hasDiagonalDownStyle && actualDownStyle == XlBorderLineStyle.None) {
					borderInfo.DiagonalDownLineStyle = XlBorderLineStyle.None;
					if (borderInfo.DiagonalColor == null) {
						borderInfo.DiagonalColor = Color.Empty;
						applyDiagonalDownColor = false;
					}
					optionsInfo.ApplyDiagonalDownBorder = false;
				}
			}
		}
		protected void SetVerticalBorder(IActualBorderInfo info, MergedBorderInfo borderInfo, MergedBorderOptionsInfo optionsInfo) {
			SetInsideBorder(info, info, MergedBorderInfoAccessor.Vertical, BorderSideAccessor.Left, BorderSideAccessor.Right, borderInfo, optionsInfo);
		}
		protected void SetHorizontalBorder(IActualBorderInfo info, MergedBorderInfo borderInfo, MergedBorderOptionsInfo optionsInfo) {
			SetInsideBorder(info, info, MergedBorderInfoAccessor.Horizontal, BorderSideAccessor.Top, BorderSideAccessor.Bottom, borderInfo, optionsInfo);
		}
		protected void SetOutlineBorder(IActualBorderInfo info, MergedBorderInfoAccessor accessor, BorderSideAccessor actualAcessor, MergedBorderInfo borderInfo, MergedBorderOptionsInfo optionsInfo) {
			SetBordersOutlineRangePosition(info, accessor, actualAcessor, borderInfo, optionsInfo, SpecialBorderLineStyle.OutsideComplexBorder);
		}
		protected void SetInsideBorder(IActualBorderInfo firstInfo, IActualBorderInfo secondInfo, MergedBorderInfoAccessor accessor, BorderSideAccessor firstActualAcessor, BorderSideAccessor secondActualAcessor, MergedBorderInfo borderInfo, MergedBorderOptionsInfo optionsInfo) {
			if (!accessor.Indeterminate(borderInfo)) {
				XlBorderLineStyle? lineStyle = accessor.GetLineStyle(borderInfo);
				Color? color = accessor.GetLineColor(borderInfo);
				XlBorderLineStyle firstActualStyle = firstActualAcessor.GetLineStyle(firstInfo);
				XlBorderLineStyle secondActualStyle = secondActualAcessor.GetLineStyle(secondInfo);
				Color firstActualColor = firstActualAcessor.GetLineColor(firstInfo);
				Color secondActualColor = secondActualAcessor.GetLineColor(secondInfo);
				bool hasStyle = lineStyle.HasValue;
				bool hasFirstNoneStyle = firstActualStyle == XlBorderLineStyle.None;
				bool hasSecondNoneStyle = secondActualStyle == XlBorderLineStyle.None;
				if ((!hasStyle || lineStyle.Value == XlBorderLineStyle.None) && !hasFirstNoneStyle && !hasSecondNoneStyle && firstActualStyle == secondActualStyle && firstActualColor == secondActualColor) {
					accessor.SetBorderLine(borderInfo, firstActualStyle, firstActualAcessor.GetLineColor(firstInfo));
					accessor.SetApplyBorder(optionsInfo, true);
				}
				if (!hasStyle && (firstActualStyle != secondActualStyle || firstActualColor != secondActualColor)) {
					accessor.SetBorderLine(borderInfo, SpecialBorderLineStyle.InsideComplexBorder, Color.Empty);
					accessor.SetApplyBorder(optionsInfo, false);
				}
				if (hasStyle && ((firstActualStyle != lineStyle.Value || firstActualColor != color.Value) || (secondActualStyle != lineStyle.Value || secondActualColor != color.Value))) {
					accessor.SetBorderLine(borderInfo, SpecialBorderLineStyle.InsideComplexBorder, Color.Empty);
					accessor.SetApplyBorder(optionsInfo, false);
				}
				if (!hasStyle && hasFirstNoneStyle && hasSecondNoneStyle) {
					accessor.SetBorderLine(borderInfo, XlBorderLineStyle.None, Color.Empty);
					accessor.SetApplyBorder(optionsInfo, false);
				}
			}
		}
		protected void SetInsideBorderFirstAndLastIndexRC(IActualBorderInfo info, MergedBorderInfoAccessor accessor, BorderSideAccessor actualAcessor, MergedBorderInfo borderInfo, MergedBorderOptionsInfo optionsInfo) {
			SetBordersOutlineRangePosition(info, accessor, actualAcessor, borderInfo, optionsInfo, SpecialBorderLineStyle.InsideComplexBorder);
		}
		protected void SetBordersOutlineRangePosition(IActualBorderInfo info, MergedBorderInfoAccessor accessor, BorderSideAccessor actualAcessor, MergedBorderInfo borderInfo, MergedBorderOptionsInfo optionsInfo, XlBorderLineStyle initializeLine) {
			if (!accessor.Indeterminate(borderInfo)) {
				XlBorderLineStyle? lineStyle = accessor.GetLineStyle(borderInfo);
				Color? color = accessor.GetLineColor(borderInfo);
				XlBorderLineStyle actualLineStyle = actualAcessor.GetLineStyle(info);
				Color actualColor = actualAcessor.GetLineColor(info);
				bool hasStyle = lineStyle.HasValue;
				if (!hasStyle && actualLineStyle != XlBorderLineStyle.None) {
					accessor.SetBorderLine(borderInfo, actualLineStyle, actualColor);
					accessor.SetApplyBorder(optionsInfo, true);
				}
				else if (hasStyle && (actualLineStyle != lineStyle.Value || actualColor != color)) {
					accessor.SetBorderLine(borderInfo, initializeLine, Color.Empty);
					accessor.SetApplyBorder(optionsInfo, false);
				}
				else if (!hasStyle && actualLineStyle == XlBorderLineStyle.None) {
					accessor.SetBorderLine(borderInfo, XlBorderLineStyle.None, Color.Empty);
					accessor.SetApplyBorder(optionsInfo, false);
				}
			}
		}
		protected bool CheckAllIndeterminate(MergedBorderInfo borderInfo) {
			return
				MergedBorderInfoAccessor.Left.Indeterminate(borderInfo) &&
				MergedBorderInfoAccessor.Right.Indeterminate(borderInfo) &&
				MergedBorderInfoAccessor.Top.Indeterminate(borderInfo) &&
				MergedBorderInfoAccessor.Bottom.Indeterminate(borderInfo) &&
				MergedBorderInfoAccessor.Vertical.Indeterminate(borderInfo) &&
				MergedBorderInfoAccessor.Horizontal.Indeterminate(borderInfo);
		}
	}
	#endregion
	#region RowColumnBorderWalkerBase
	public class RowColumnBorderWalkerBase : CalculationBordersBase {
		protected void ColumnProcessBorders(int index, int firstIndex, int lastIndex, IActualBorderInfo info, MergedBorderInfo borderInfo, MergedBorderOptionsInfo optionsInfo, Worksheet sheet) {
			SetColumnVerticalBorderCore(index, firstIndex, lastIndex, info, borderInfo, optionsInfo);
			SetColumnHorizontalBorder(index, info, borderInfo, optionsInfo, sheet);
			SetDiagonalBorders(info, borderInfo, optionsInfo);
		}
		protected void ColumnProcessBordersWithExistingCell(int leftColumnIndex, int rightColumnIndex, ICell cell, MergedBorderInfo borderInfo, MergedBorderOptionsInfo optionsInfo) {
			int columnIndex = cell.ColumnIndex;
			int rowIndex = cell.RowIndex;
			SetColumnVerticalBorderCore(columnIndex, leftColumnIndex, rightColumnIndex, cell, borderInfo, optionsInfo);
			SetColumnHorizontalBorderExistingCell(columnIndex, rowIndex, leftColumnIndex, rightColumnIndex, cell, borderInfo, optionsInfo);
			SetDiagonalBorders(cell, borderInfo, optionsInfo);
		}
		protected virtual void SetColumnVerticalBorderCore(int columnIndex, int firstIndex, int lastIndex, IActualBorderInfo actualInfo, MergedBorderInfo borderInfo, MergedBorderOptionsInfo optionsInfo) {
			if (columnIndex == firstIndex) {
				SetLeftBorder(actualInfo, borderInfo, optionsInfo);
				if (firstIndex != lastIndex)
					SetInsideBorderFirstAndLastIndexRC(actualInfo, MergedBorderInfoAccessor.Vertical, BorderSideAccessor.Right, borderInfo, optionsInfo);
			}
			if (columnIndex == lastIndex) {
				SetRightBorder(actualInfo, borderInfo, optionsInfo);
				if (firstIndex != lastIndex) {
					SetInsideBorderFirstAndLastIndexRC(actualInfo, MergedBorderInfoAccessor.Vertical, BorderSideAccessor.Left, borderInfo, optionsInfo);
				}
			}
			if (columnIndex > firstIndex && columnIndex < lastIndex) {
				SetVerticalBorder(actualInfo, borderInfo, optionsInfo);
			}
		}
		protected void SetColumnHorizontalBorder(int columnIndex, IActualBorderInfo actualInfo, MergedBorderInfo borderInfo, MergedBorderOptionsInfo optionsInfo, Worksheet sheet) {
			SetTopBorder(sheet.GetCellForFormatting(columnIndex, 0), borderInfo, optionsInfo);
			SetBottomBorder(sheet.GetCellForFormatting(columnIndex, IndicesChecker.MaxRowIndex), borderInfo, optionsInfo);
			SetHorizontalBorder(actualInfo, borderInfo, optionsInfo);
		}
		protected void SetColumnHorizontalBorderExistingCell(int columnIndex, int rowIndex, int firstIndex, int secondIndex, IActualBorderInfo actualInfo, MergedBorderInfo borderInfo, MergedBorderOptionsInfo optionsInfo) {
			if (columnIndex >= firstIndex && columnIndex <= secondIndex) {
				if (rowIndex == 0)
					SetBordersOutlineRangePosition(actualInfo, MergedBorderInfoAccessor.Horizontal, BorderSideAccessor.Bottom, borderInfo, optionsInfo, SpecialBorderLineStyle.InsideComplexBorder);
				if (rowIndex == IndicesChecker.MaxRowIndex)
					SetBordersOutlineRangePosition(actualInfo, MergedBorderInfoAccessor.Horizontal, BorderSideAccessor.Top, borderInfo, optionsInfo, SpecialBorderLineStyle.InsideComplexBorder);
				if (rowIndex > 0 && rowIndex < IndicesChecker.MaxRowIndex)
					SetHorizontalBorder(actualInfo, borderInfo, optionsInfo);
			}
		}
		protected void RowProcessBorders(int rowIndex, int firstIndex, int lastIndex, IActualBorderInfo info, MergedBorderInfo borderInfo, MergedBorderOptionsInfo optionsInfo, Worksheet sheet) {
			SetRowHorizontalBorderCore(rowIndex, firstIndex, lastIndex, info, borderInfo, optionsInfo);
			SetRowVerticalBorder(rowIndex, info, borderInfo, optionsInfo, sheet);
			SetDiagonalBorders(info, borderInfo, optionsInfo);
		}
		protected void RowProcessBordersWithExistingCell(int topRowIndex, int bottomRowIndex, ICell cell, MergedBorderInfo borderInfo, MergedBorderOptionsInfo optionsInfo) {
			int columnIndex = cell.ColumnIndex;
			int rowIndex = cell.RowIndex;
			SetRowHorizontalBorderCore(rowIndex, topRowIndex, bottomRowIndex, cell, borderInfo, optionsInfo);
			SetRowVerticalBorderExistingCell(columnIndex, rowIndex, topRowIndex, bottomRowIndex, cell, borderInfo, optionsInfo);
			SetDiagonalBorders(cell, borderInfo, optionsInfo);
		}
		protected virtual void SetRowHorizontalBorderCore(int rowIndex, int firstIndex, int lastIndex, IActualBorderInfo actualInfo, MergedBorderInfo borderInfo, MergedBorderOptionsInfo optionsInfo) {
			if (rowIndex == firstIndex) {
				SetTopBorder(actualInfo, borderInfo, optionsInfo);
				if (firstIndex != lastIndex)
					SetInsideBorderFirstAndLastIndexRC(actualInfo, MergedBorderInfoAccessor.Horizontal, BorderSideAccessor.Bottom, borderInfo, optionsInfo);
			}
			if (rowIndex == lastIndex) {
				SetBottomBorder(actualInfo, borderInfo, optionsInfo);
				if (firstIndex != lastIndex) {
					SetInsideBorderFirstAndLastIndexRC(actualInfo, MergedBorderInfoAccessor.Horizontal, BorderSideAccessor.Top, borderInfo, optionsInfo);
				}
			}
			if (rowIndex > firstIndex && rowIndex < lastIndex) {
				SetHorizontalBorder(actualInfo, borderInfo, optionsInfo);
			}
		}
		protected void SetRowVerticalBorder(int rowIndex, IActualBorderInfo actualInfo, MergedBorderInfo borderInfo, MergedBorderOptionsInfo optionsInfo, Worksheet sheet) {
			SetLeftBorder(sheet.GetCellForFormatting(0, rowIndex), borderInfo, optionsInfo);
			SetRightBorder(sheet.GetCellForFormatting(IndicesChecker.MaxColumnIndex, rowIndex), borderInfo, optionsInfo);
			SetVerticalBorder(actualInfo, borderInfo, optionsInfo);
		}
		protected void SetRowVerticalBorderExistingCell(int columnIndex, int rowIndex, int firstIndex, int secondIndex, IActualBorderInfo actualInfo, MergedBorderInfo borderInfo, MergedBorderOptionsInfo optionsInfo) {
			if (rowIndex >= firstIndex && rowIndex <= secondIndex) {
				if (columnIndex == 0)
					SetBordersOutlineRangePosition(actualInfo, MergedBorderInfoAccessor.Vertical, BorderSideAccessor.Right, borderInfo, optionsInfo, SpecialBorderLineStyle.InsideComplexBorder);
				if (columnIndex == IndicesChecker.MaxColumnIndex)
					SetBordersOutlineRangePosition(actualInfo, MergedBorderInfoAccessor.Vertical, BorderSideAccessor.Left, borderInfo, optionsInfo, SpecialBorderLineStyle.InsideComplexBorder);
				if (columnIndex > 0 && columnIndex < IndicesChecker.MaxColumnIndex)
					SetVerticalBorder(actualInfo, borderInfo, optionsInfo);
			}
		}
		protected void SetBordersExistingRowColumnInterceptCell(ICell currentCell, MergedBorderInfo borderInfo, MergedBorderOptionsInfo optionsInfo, CellRange range) {
			int cellColumnIndex = currentCell.LeftColumnIndex;
			int cellRowIndex = currentCell.TopRowIndex;
			int rangeLeftColumnIndex = range.LeftColumnIndex;
			int rangeRightColumnIndex = range.RightColumnIndex;
			int rangeTopRowIndex = range.TopRowIndex;
			int rangeBottomRowIndex = range.BottomRowIndex;
			if (cellColumnIndex == rangeLeftColumnIndex) {
				SetLeftBorder(currentCell, borderInfo, optionsInfo);
				SetBordersOutlineRangePosition(currentCell, MergedBorderInfoAccessor.Vertical, BorderSideAccessor.Right, borderInfo, optionsInfo, SpecialBorderLineStyle.InsideComplexBorder);
			}
			if (cellColumnIndex == rangeRightColumnIndex) {
				SetRightBorder(currentCell, borderInfo, optionsInfo);
				SetBordersOutlineRangePosition(currentCell, MergedBorderInfoAccessor.Vertical, BorderSideAccessor.Left, borderInfo, optionsInfo, SpecialBorderLineStyle.InsideComplexBorder);
			}
			if (cellColumnIndex > rangeLeftColumnIndex && cellColumnIndex < rangeRightColumnIndex)
				SetVerticalBorder(currentCell, borderInfo, optionsInfo);
			if (cellRowIndex == rangeTopRowIndex) {
				SetTopBorder(currentCell, borderInfo, optionsInfo);
				SetBordersOutlineRangePosition(currentCell, MergedBorderInfoAccessor.Horizontal, BorderSideAccessor.Bottom, borderInfo, optionsInfo, SpecialBorderLineStyle.InsideComplexBorder);
			}
			if (cellRowIndex == rangeBottomRowIndex) {
				SetBottomBorder(currentCell, borderInfo, optionsInfo);
				SetBordersOutlineRangePosition(currentCell, MergedBorderInfoAccessor.Horizontal, BorderSideAccessor.Top, borderInfo, optionsInfo, SpecialBorderLineStyle.InsideComplexBorder);
			}
			if (cellRowIndex > rangeTopRowIndex && cellRowIndex < rangeBottomRowIndex)
				SetHorizontalBorder(currentCell, borderInfo, optionsInfo);
			SetDiagonalBorders(currentCell, borderInfo, optionsInfo);
		}
	}
	#endregion
	#region RangeBordersCalculator
	public static class RangeBordersCalculator {
		delegate void Walker(MergedBorderInfo borderInfo, MergedBorderOptionsInfo optionsInfo, CellRange range);
		public static void CalculateBorderInfo(IList<CellRange> ranges, MergedBorderInfo borderInfo, MergedBorderOptionsInfo optionsInfo) {
			Walker walk;
			int count = ranges.Count;
			for (int i = 0; i < count; i++) {
				CellRange range = ranges[i];
				walk = CreateWalker(range);
				walk(borderInfo, optionsInfo, range);
			}
			ChangeBordersNullableValues(borderInfo, optionsInfo);
		}
		static Walker CreateWalker(CellRange range) {
			int leftIndex = range.LeftColumnIndex;
			int topIndex = range.TopRowIndex;
			int rightIndex = range.RightColumnIndex;
			int bottomIndex = range.BottomRowIndex;
			bool entireAllRowsSelected = leftIndex == 0 && rightIndex == IndicesChecker.MaxColumnCount - 1;
			bool entireAllColumnsSelected = topIndex == 0 && bottomIndex == IndicesChecker.MaxRowCount - 1;
			if (entireAllColumnsSelected && entireAllRowsSelected)
				return new SheetBordersWalker().SheetWalk;
			if (entireAllColumnsSelected)
				return new ColumnsBordersWalker().ColumnWalk;
			if (entireAllRowsSelected)
				return new RowsBordersWalker().RowWalk;
			return new RangeBordersWalker().RangeWalk;
		}
		static void ChangeBordersNullableValues(MergedBorderInfo borderInfo, MergedBorderOptionsInfo optionsInfo) {
			ChangeBorderNullableValues(MergedBorderInfoAccessor.Left, borderInfo, optionsInfo);
			ChangeBorderNullableValues(MergedBorderInfoAccessor.Right, borderInfo, optionsInfo);
			ChangeBorderNullableValues(MergedBorderInfoAccessor.Top, borderInfo, optionsInfo);
			ChangeBorderNullableValues(MergedBorderInfoAccessor.Bottom, borderInfo, optionsInfo);
			ChangeBorderNullableValues(MergedBorderInfoAccessor.Horizontal, borderInfo, optionsInfo);
			ChangeBorderNullableValues(MergedBorderInfoAccessor.Vertical, borderInfo, optionsInfo);
			ChangeDiagonalBordersNullableValues(borderInfo, optionsInfo);
		}
		static void ChangeBorderNullableValues(MergedBorderInfoAccessor accessor, MergedBorderInfo borderInfo, MergedBorderOptionsInfo optionsInfo) {
			if (accessor.NullableValues(borderInfo)) {
				accessor.SetBorderLine(borderInfo, XlBorderLineStyle.None, Color.Empty);
				accessor.SetApplyBorder(optionsInfo, false);
			}
		}
		static void ChangeDiagonalBordersNullableValues(MergedBorderInfo borderInfo, MergedBorderOptionsInfo optionsInfo) {
			if (borderInfo.DiagonalDownLineStyle == null && borderInfo.DiagonalUpLineStyle == null && borderInfo.DiagonalColor == null) {
				borderInfo.DiagonalDownLineStyle = XlBorderLineStyle.None;
				borderInfo.DiagonalUpLineStyle = XlBorderLineStyle.None;
				borderInfo.DiagonalColor = Color.Empty;
				optionsInfo.ApplyDiagonalDownBorder = false;
				optionsInfo.ApplyDiagonalUpBorder = false;
			}
			if (borderInfo.DiagonalDownLineStyle == null && borderInfo.DiagonalUpLineStyle != null) {
				borderInfo.DiagonalDownLineStyle = XlBorderLineStyle.None;
				optionsInfo.ApplyDiagonalDownBorder = false;
			}
			if (borderInfo.DiagonalDownLineStyle != null && borderInfo.DiagonalUpLineStyle == null) {
				borderInfo.DiagonalUpLineStyle = XlBorderLineStyle.None;
				optionsInfo.ApplyDiagonalUpBorder = false;
			}
		}
	}
	#endregion
	#region MergedBorderInfoAccessorBase (absract class)
	public abstract class MergedBorderInfoAccessor {
		readonly static MergedBorderInfoAccessor top = new TopMergedBorderInfoAccessor();
		readonly static MergedBorderInfoAccessor bottom = new BottomMergedBorderInfoAccessor();
		readonly static MergedBorderInfoAccessor left = new LeftMergedBorderInfoAccessor();
		readonly static MergedBorderInfoAccessor right = new RightMergedBorderInfoAccessor();
		readonly static MergedBorderInfoAccessor horizontal = new HorizontalMergedBorderInfoAccessor();
		readonly static MergedBorderInfoAccessor vertical = new VerticalMergedBorderInfoAccessor();
		public static MergedBorderInfoAccessor Top { get { return top; } }
		public static MergedBorderInfoAccessor Bottom { get { return bottom; } }
		public static MergedBorderInfoAccessor Left { get { return left; } }
		public static MergedBorderInfoAccessor Right { get { return right; } }
		public static MergedBorderInfoAccessor Horizontal { get { return horizontal; } }
		public static MergedBorderInfoAccessor Vertical { get { return vertical; } }
		public virtual bool Indeterminate(MergedBorderInfo info) {
			return GetLineStyle(info) == SpecialBorderLineStyle.OutsideComplexBorder;
		}
		public virtual bool NullableValues(MergedBorderInfo info) {
			return GetLineStyle(info) == null && GetLineColor(info) == null;
		}
		public abstract Color? GetLineColor(MergedBorderInfo info);
		public abstract XlBorderLineStyle? GetLineStyle(MergedBorderInfo info);
		public abstract void SetBorderLine(MergedBorderInfo info, XlBorderLineStyle? style, Color? value);
		public abstract bool? GetApplyBorder(MergedBorderOptionsInfo info);
		public abstract void SetApplyBorder(MergedBorderOptionsInfo info, bool? value);
	}
	#endregion
	#region MergedBorderInfoAccessors
	public class TopMergedBorderInfoAccessor : MergedBorderInfoAccessor {
		public override Color? GetLineColor(MergedBorderInfo info) {
			return info.TopColor;
		}
		public override XlBorderLineStyle? GetLineStyle(MergedBorderInfo info) {
			return info.TopLineStyle;
		}
		public override void SetBorderLine(MergedBorderInfo info, XlBorderLineStyle? style, Color? value) {
			info.TopLineStyle = style;
			info.TopColor = value;
		}
		public override bool? GetApplyBorder(MergedBorderOptionsInfo info) {
			return info.ApplyTopBorder;
		}
		public override void SetApplyBorder(MergedBorderOptionsInfo info, bool? value) {
			info.ApplyTopBorder = value;
		}
	}
	public class BottomMergedBorderInfoAccessor : MergedBorderInfoAccessor {
		public override Color? GetLineColor(MergedBorderInfo info) {
			return info.BottomColor;
		}
		public override XlBorderLineStyle? GetLineStyle(MergedBorderInfo info) {
			return info.BottomLineStyle;
		}
		public override void SetBorderLine(MergedBorderInfo info, XlBorderLineStyle? style, Color? value) {
			info.BottomLineStyle = style;
			info.BottomColor = value;
		}
		public override bool? GetApplyBorder(MergedBorderOptionsInfo info) {
			return info.ApplyBottomBorder;
		}
		public override void SetApplyBorder(MergedBorderOptionsInfo info, bool? value) {
			info.ApplyBottomBorder = value;
		}
	}
	public class LeftMergedBorderInfoAccessor : MergedBorderInfoAccessor {
		public override Color? GetLineColor(MergedBorderInfo info) {
			return info.LeftColor;
		}
		public override XlBorderLineStyle? GetLineStyle(MergedBorderInfo info) {
			return info.LeftLineStyle;
		}
		public override void SetBorderLine(MergedBorderInfo info, XlBorderLineStyle? style, Color? value) {
			info.LeftLineStyle = style;
			info.LeftColor = value;
		}
		public override bool? GetApplyBorder(MergedBorderOptionsInfo info) {
			return info.ApplyLeftBorder;
		}
		public override void SetApplyBorder(MergedBorderOptionsInfo info, bool? value) {
			info.ApplyLeftBorder = value;
		}
	}
	public class RightMergedBorderInfoAccessor : MergedBorderInfoAccessor {
		public override Color? GetLineColor(MergedBorderInfo info) {
			return info.RightColor;
		}
		public override XlBorderLineStyle? GetLineStyle(MergedBorderInfo info) {
			return info.RightLineStyle;
		}
		public override void SetBorderLine(MergedBorderInfo info, XlBorderLineStyle? style, Color? value) {
			info.RightLineStyle = style;
			info.RightColor = value;
		}
		public override bool? GetApplyBorder(MergedBorderOptionsInfo info) {
			return info.ApplyRightBorder;
		}
		public override void SetApplyBorder(MergedBorderOptionsInfo info, bool? value) {
			info.ApplyRightBorder = value;
		}
	}
	public class VerticalMergedBorderInfoAccessor : MergedBorderInfoAccessor {
		public override Color? GetLineColor(MergedBorderInfo info) {
			return info.VerticalColor;
		}
		public override XlBorderLineStyle? GetLineStyle(MergedBorderInfo info) {
			return info.VerticalLineStyle;
		}
		public override void SetBorderLine(MergedBorderInfo info, XlBorderLineStyle? style, Color? value) {
			info.VerticalLineStyle = style;
			info.VerticalColor = value;
		}
		public override bool? GetApplyBorder(MergedBorderOptionsInfo info) {
			return info.ApplyVerticalBorder;
		}
		public override void SetApplyBorder(MergedBorderOptionsInfo info, bool? value) {
			info.ApplyVerticalBorder = value;
		}
		public override bool Indeterminate(MergedBorderInfo info) {
			return GetLineStyle(info) == SpecialBorderLineStyle.InsideComplexBorder;
		}
	}
	public class HorizontalMergedBorderInfoAccessor : MergedBorderInfoAccessor {
		public override Color? GetLineColor(MergedBorderInfo info) {
			return info.HorizontalColor;
		}
		public override XlBorderLineStyle? GetLineStyle(MergedBorderInfo info) {
			return info.HorizontalLineStyle;
		}
		public override void SetBorderLine(MergedBorderInfo info, XlBorderLineStyle? style, Color? value) {
			info.HorizontalLineStyle = style;
			info.HorizontalColor = value;
		}
		public override bool? GetApplyBorder(MergedBorderOptionsInfo info) {
			return info.ApplyHorizontalBorder;
		}
		public override void SetApplyBorder(MergedBorderOptionsInfo info, bool? value) {
			info.ApplyHorizontalBorder = value;
		}
		public override bool Indeterminate(MergedBorderInfo info) {
			return GetLineStyle(info) == SpecialBorderLineStyle.InsideComplexBorder;
		}
	}
	#endregion
	#region SheetBordersWalker
	public class SheetBordersWalker : RowColumnBorderWalkerBase {
		protected internal void SheetWalk(MergedBorderInfo borderInfo, MergedBorderOptionsInfo optionsInfo, CellRange range) {
			Worksheet sheet = range.Worksheet as Worksheet;
			int leftColumnIndex = range.LeftColumnIndex;
			int rightColumnIndex = range.RightColumnIndex;
			ColumnCollection columns = sheet.Columns;
			for (int i = leftColumnIndex; i <= rightColumnIndex; i++) {
				IActualBorderInfo actualRowBorderInfo = RangeBordersFormattingHelper.GetColumnActualBorderInfo(sheet, i);
				ColumnProcessBorders(i, leftColumnIndex, rightColumnIndex, actualRowBorderInfo, borderInfo, optionsInfo, sheet);
				if (CheckAllIndeterminate(borderInfo))
					return;
			}
			IEnumerator<Row> existingRows = sheet.Rows.GetExistingRows().GetEnumerator();
			while (existingRows.MoveNext()) {
				Row row = existingRows.Current;
				RowProcessBorders(row.Index, 0, IndicesChecker.MaxRowIndex, row, borderInfo, optionsInfo, sheet);
			}
			IEnumerator<ICellBase> existingCells = range.GetExistingCellsEnumerator(false);
			while (existingCells.MoveNext()) {
				ICell cell = existingCells.Current as ICell;
				if (cell != null)
					SetBordersExistingRowColumnInterceptCell(cell, borderInfo, optionsInfo, range);
			}
		}
		protected override void SetColumnVerticalBorderCore(int columnIndex, int firstIndex, int lastIndex, IActualBorderInfo actualInfo, MergedBorderInfo borderInfo, MergedBorderOptionsInfo optionsInfo) {
			if (columnIndex == firstIndex) {
				SetOutlineBorderCoreExistingRC(actualInfo, MergedBorderInfoAccessor.Left, BorderSideAccessor.Left, borderInfo, optionsInfo);
				if (firstIndex != lastIndex)
					SetInsideBorderFirstAndLastIndexRC(actualInfo, MergedBorderInfoAccessor.Vertical, BorderSideAccessor.Right, borderInfo, optionsInfo);
			}
			if (columnIndex == lastIndex) {
				SetOutlineBorderCoreExistingRC(actualInfo, MergedBorderInfoAccessor.Right, BorderSideAccessor.Right, borderInfo, optionsInfo);
				if (firstIndex != lastIndex) {
					SetInsideBorderFirstAndLastIndexRC(actualInfo, MergedBorderInfoAccessor.Vertical, BorderSideAccessor.Left, borderInfo, optionsInfo);
				}
			}
			if (columnIndex > firstIndex && columnIndex < lastIndex) {
				SetVerticalBorder(actualInfo, borderInfo, optionsInfo);
			}
		}
		protected override void SetRowHorizontalBorderCore(int rowIndex, int firstIndex, int lastIndex, IActualBorderInfo actualInfo, MergedBorderInfo borderInfo, MergedBorderOptionsInfo optionsInfo) {
			if (rowIndex == firstIndex) {
				SetOutlineBorderCoreExistingRC(actualInfo, MergedBorderInfoAccessor.Top, BorderSideAccessor.Top, borderInfo, optionsInfo);
				if (firstIndex != lastIndex)
					SetInsideBorderFirstAndLastIndexRC(actualInfo, MergedBorderInfoAccessor.Horizontal, BorderSideAccessor.Bottom, borderInfo, optionsInfo);
			}
			if (rowIndex == lastIndex) {
				SetOutlineBorderCoreExistingRC(actualInfo, MergedBorderInfoAccessor.Bottom, BorderSideAccessor.Bottom, borderInfo, optionsInfo);
				if (firstIndex != lastIndex) {
					SetInsideBorderFirstAndLastIndexRC(actualInfo, MergedBorderInfoAccessor.Horizontal, BorderSideAccessor.Top, borderInfo, optionsInfo);
				}
			}
			if (rowIndex > firstIndex && rowIndex < lastIndex) {
				SetHorizontalBorder(actualInfo, borderInfo, optionsInfo);
			}
		}
		void SetOutlineBorderCoreExistingRC(IActualBorderInfo info, MergedBorderInfoAccessor accessor, BorderSideAccessor actualAcessor, MergedBorderInfo borderInfo, MergedBorderOptionsInfo optionsInfo) {
			if (!accessor.Indeterminate(borderInfo)) {
				XlBorderLineStyle? lineStyle = accessor.GetLineStyle(borderInfo);
				Color? color = accessor.GetLineColor(borderInfo);
				XlBorderLineStyle actualLineStyle = actualAcessor.GetLineStyle(info);
				Color actualColor = actualAcessor.GetLineColor(info);
				if (actualLineStyle != XlBorderLineStyle.None) {
					accessor.SetBorderLine(borderInfo, actualLineStyle, actualAcessor.GetLineColor(info));
					accessor.SetApplyBorder(optionsInfo, true);
				}
				else if (lineStyle.HasValue && color.HasValue && (actualLineStyle != lineStyle.Value || actualColor != color)) {
					accessor.SetBorderLine(borderInfo, SpecialBorderLineStyle.OutsideComplexBorder, Color.Empty);
					accessor.SetApplyBorder(optionsInfo, false);
				}
				else if (actualLineStyle == XlBorderLineStyle.None) {
					accessor.SetBorderLine(borderInfo, XlBorderLineStyle.None, Color.Empty);
					accessor.SetApplyBorder(optionsInfo, false);
				}
			}
		}
	}
	#endregion
	#region ColumnsBordersWalker
	public class ColumnsBordersWalker : RowColumnBorderWalkerBase {
		protected internal void ColumnWalk(MergedBorderInfo borderInfo, MergedBorderOptionsInfo optionsInfo, CellRange range) {
			Worksheet sheet = range.Worksheet as Worksheet;
			int leftColumnIndex = range.LeftColumnIndex;
			int rightColumnIndex = range.RightColumnIndex;
			for (int i = leftColumnIndex; i <= rightColumnIndex; i++) {
				IActualBorderInfo actualRowBorderInfo = RangeBordersFormattingHelper.GetColumnActualBorderInfo(sheet, i);
				ColumnProcessBorders(i, leftColumnIndex, rightColumnIndex, actualRowBorderInfo, borderInfo, optionsInfo, sheet);
				if (CheckAllIndeterminate(borderInfo))
					return;
			}
			ColumnRowCellsIntersect(sheet, borderInfo, optionsInfo, range);
			IEnumerator<ICellBase> existingCells = range.GetExistingCellsEnumerator(false);
			while (existingCells.MoveNext()) {
				ICell cell = existingCells.Current as ICell;
				if (cell != null) {
					ColumnProcessBordersWithExistingCell(leftColumnIndex, rightColumnIndex, cell, borderInfo, optionsInfo);
				}
			}
		}
		void ColumnRowCellsIntersect(Worksheet sheet, MergedBorderInfo borderInfo, MergedBorderOptionsInfo optionsInfo, CellRange range) {
			IEnumerator<Row> existingRows = sheet.Rows.GetExistingRows().GetEnumerator();
			while (existingRows.MoveNext()) {
				for (int currentColumnIndex = range.LeftColumnIndex; currentColumnIndex <= range.RightColumnIndex; currentColumnIndex++)
					SetBordersExistingRowColumnInterceptCell(sheet.GetCellForFormatting(currentColumnIndex, existingRows.Current.Index), borderInfo, optionsInfo, range);
			}
		}
	}
	#endregion
	#region RowsBordersWalker
	public class RowsBordersWalker : RowColumnBorderWalkerBase {
		protected internal void RowWalk(MergedBorderInfo borderInfo, MergedBorderOptionsInfo optionsInfo, CellRange range) {
			Worksheet sheet = range.Worksheet as Worksheet;
			int topRowIndex = range.TopRowIndex;
			int bottomRowIndex = range.BottomRowIndex;
			for (int i = topRowIndex; i <= bottomRowIndex; i++) {
				IActualBorderInfo actualRowBorderInfo = RangeBordersFormattingHelper.GetRowActualBorderInfo(sheet, i);
				if (actualRowBorderInfo == null)
					continue;
				RowProcessBorders(i, topRowIndex, bottomRowIndex, actualRowBorderInfo, borderInfo, optionsInfo, sheet);
				if (CheckAllIndeterminate(borderInfo))
					return;
			}
			RowColumnCellsIntersect(sheet, borderInfo, optionsInfo, range);
			IEnumerator<ICellBase> existingCells = range.GetExistingCellsEnumerator(false);
			while (existingCells.MoveNext()) {
				ICell cell = existingCells.Current as ICell;
				if (cell != null) {
					RowProcessBordersWithExistingCell(topRowIndex, bottomRowIndex, cell, borderInfo, optionsInfo);
				}
			}
		}
		void RowColumnCellsIntersect(Worksheet sheet, MergedBorderInfo borderInfo, MergedBorderOptionsInfo optionsInfo, CellRange range) {
			IEnumerable<Column> existingColumns = sheet.Columns.GetExistingColumns();
			foreach (Column column in existingColumns) {
				for (int currentRowIndex = range.TopRowIndex; currentRowIndex <= range.BottomRowIndex; currentRowIndex++) {
					int endColumnIndex = column.EndIndex;
					for (int currentColumnIndex = column.StartIndex; currentColumnIndex <= endColumnIndex; currentColumnIndex++)
						SetBordersExistingRowColumnInterceptCell(sheet.GetCellForFormatting(currentColumnIndex, currentRowIndex), borderInfo, optionsInfo, range);
				}
			}
		}
	}
	#endregion
	#region RangeBordersWalker
	public class RangeBordersWalker : CalculationBordersBase {
		protected internal void RangeWalk(MergedBorderInfo borderInfo, MergedBorderOptionsInfo optionsInfo, CellRange range) {
			int leftColumnIndex = range.LeftColumnIndex;
			int rightColumnIndex = range.RightColumnIndex;
			int topRowIndex = range.TopRowIndex;
			int bottomRowIndex = range.BottomRowIndex;
			bool verticalVector = leftColumnIndex == rightColumnIndex;
			bool horizontalVector = topRowIndex == bottomRowIndex;
			if (verticalVector && horizontalVector)
				ProcessCell((range.Worksheet as Worksheet).GetCellForFormatting(leftColumnIndex, bottomRowIndex), borderInfo, optionsInfo);
			else if (verticalVector)
				ProcessVerticalRangeVector(range, borderInfo, optionsInfo);
			else if (horizontalVector)
				ProcessHorizontalRangeVector(range, borderInfo, optionsInfo);
			else
				ProcessTableRangeVector(range, borderInfo, optionsInfo);
		}
		void ProcessCell(ICell cell, MergedBorderInfo borderInfo, MergedBorderOptionsInfo optionsInfo) {
			Worksheet sheet = cell.Worksheet as Worksheet;
			int leftColumnIndex = cell.LeftColumnIndex;
			int topRowIndex = cell.TopRowIndex;
			ProcessBordersForCell(cell, SetLeftBorder, SetRightBorder, SetTopBorder, SetBottomBorder, borderInfo, optionsInfo);
		}
		void ProcessVerticalRangeVector(CellRange range, MergedBorderInfo borderInfo, MergedBorderOptionsInfo optionsInfo) {
			Worksheet sheet = range.Worksheet as Worksheet;
			int columnIndex = range.LeftColumnIndex;
			int topRowIndex = range.TopRowIndex;
			int bottomRowIndex = range.BottomRowIndex;
			for (int i = topRowIndex; i < bottomRowIndex; i++) {
				TopLeftCell = sheet.GetCellForFormatting(columnIndex, i);
				BottomLeftCell = sheet.GetCellForFormatting(columnIndex, i + 1);
				ProcessBordersForVerticalRange(topRowIndex, bottomRowIndex, TopLeftCell, BottomLeftCell, SetLeftBorder, SetRightBorder, SetTopBorder, SetBottomBorder, SetHorizontalBorder, borderInfo, optionsInfo);
				if (CheckAllIndeterminate(borderInfo))
					return;
			}
		}
		void ProcessHorizontalRangeVector(CellRange range, MergedBorderInfo borderInfo, MergedBorderOptionsInfo optionsInfo) {
			Worksheet sheet = range.Worksheet as Worksheet;
			int rowIndex = range.TopRowIndex;
			int leftColumnIndex = range.LeftColumnIndex;
			int rightColumnIndex = range.RightColumnIndex;
			for (int i = leftColumnIndex; i < rightColumnIndex; i++) {
				TopLeftCell = sheet.GetCellForFormatting(i, rowIndex);
				TopRightCell = sheet.GetCellForFormatting(i + 1, rowIndex);
				ProcessBordersForHorizontalRange(leftColumnIndex, rightColumnIndex, TopLeftCell, TopRightCell, SetLeftBorder, SetRightBorder, SetTopBorder, SetBottomBorder, SetVerticalBorder, borderInfo, optionsInfo);
				if (CheckAllIndeterminate(borderInfo))
					return;
			}
		}
		void ProcessTableRangeVector(CellRange range, MergedBorderInfo borderInfo, MergedBorderOptionsInfo optionsInfo) {
			Worksheet sheet = range.Worksheet as Worksheet;
			int leftColumnIndex = range.LeftColumnIndex;
			int rightColumnIndex = range.RightColumnIndex;
			int topRowIndex = range.TopRowIndex;
			int bottomRowIndex = range.BottomRowIndex;
			for (int i = leftColumnIndex; i < rightColumnIndex; i++)
				for (int j = topRowIndex; j < bottomRowIndex; j++) {
					TopLeftCell = sheet.GetCellForFormatting(i, j);
					TopRightCell = sheet.GetCellForFormatting(i + 1, j);
					BottomLeftCell = sheet.GetCellForFormatting(i, j + 1);
					BottomRightCell = sheet.GetCellForFormatting(i + 1, j + 1);
					ProcessBordersForTableRange(leftColumnIndex, rightColumnIndex, topRowIndex, bottomRowIndex, TopLeftCell, TopRightCell, SetLeftBorder, SetRightBorder, SetTopBorder, SetBottomBorder, SetVerticalBorder, borderInfo, optionsInfo);
					ProcessBordersForTableRange(leftColumnIndex, rightColumnIndex, topRowIndex, bottomRowIndex, BottomLeftCell, BottomRightCell, SetLeftBorder, SetRightBorder, SetTopBorder, SetBottomBorder, SetVerticalBorder, borderInfo, optionsInfo);
					ProcessBordersForTableRange(leftColumnIndex, rightColumnIndex, topRowIndex, bottomRowIndex, TopLeftCell, BottomLeftCell, SetLeftBorder, SetRightBorder, SetTopBorder, SetBottomBorder, SetHorizontalBorder, borderInfo, optionsInfo);
					ProcessBordersForTableRange(leftColumnIndex, rightColumnIndex, topRowIndex, bottomRowIndex, TopRightCell, BottomRightCell, SetLeftBorder, SetRightBorder, SetTopBorder, SetBottomBorder, SetHorizontalBorder, borderInfo, optionsInfo);
					if (CheckAllIndeterminate(borderInfo))
						return;
				}
		}
		void ProcessBordersForCell(ICell currentCell,
					Action<ICell, MergedBorderInfo, MergedBorderOptionsInfo> setLeftOutlineBorder,
					Action<ICell, MergedBorderInfo, MergedBorderOptionsInfo> setRightOutlineBorder,
					Action<ICell, MergedBorderInfo, MergedBorderOptionsInfo> setTopOutlineBorder,
					Action<ICell, MergedBorderInfo, MergedBorderOptionsInfo> setBottomOutlineBorder,
					MergedBorderInfo borderInfo, MergedBorderOptionsInfo optionsInfo) {
			SetLeftBorder(currentCell, borderInfo, optionsInfo);
			SetRightBorder(currentCell, borderInfo, optionsInfo);
			SetTopBorder(currentCell, borderInfo, optionsInfo);
			SetBottomBorder(currentCell, borderInfo, optionsInfo);
			SetDiagonalBorders(currentCell, borderInfo, optionsInfo);
		}
		void ProcessBordersForVerticalRange(int topRowIndex, int bottomRowIndex, ICell currentCell, ICell nextCell,
					Action<ICell, MergedBorderInfo, MergedBorderOptionsInfo> setLeftBorder,
					Action<ICell, MergedBorderInfo, MergedBorderOptionsInfo> setRightBorder,
					Action<ICell, MergedBorderInfo, MergedBorderOptionsInfo> setTopBorder,
					Action<ICell, MergedBorderInfo, MergedBorderOptionsInfo> setBottomBorder,
					Action<ICell, ICell, MergedBorderInfo, MergedBorderOptionsInfo> setInsideBorders,
					MergedBorderInfo borderInfo, MergedBorderOptionsInfo optionsInfo) {
			if (currentCell.Position.Row == topRowIndex) {
				setTopBorder(currentCell, borderInfo, optionsInfo);
				setLeftBorder(currentCell, borderInfo, optionsInfo);
				setRightBorder(currentCell, borderInfo, optionsInfo);
			}
			if (nextCell.Position.Row == bottomRowIndex) {
				setBottomBorder(nextCell, borderInfo, optionsInfo);
				setLeftBorder(nextCell, borderInfo, optionsInfo);
				setRightBorder(nextCell, borderInfo, optionsInfo);
			}
			setInsideBorders(currentCell, nextCell, borderInfo, optionsInfo);
			SetDiagonalBorders(currentCell, borderInfo, optionsInfo);
			SetDiagonalBorders(nextCell, borderInfo, optionsInfo);
		}
		void ProcessBordersForHorizontalRange(int leftColumnIndex, int rightColumnIndex, ICell currentCell, ICell nextCell,
					Action<ICell, MergedBorderInfo, MergedBorderOptionsInfo> setLeftBorder,
					Action<ICell, MergedBorderInfo, MergedBorderOptionsInfo> setRightBorder,
					Action<ICell, MergedBorderInfo, MergedBorderOptionsInfo> setTopBorder,
					Action<ICell, MergedBorderInfo, MergedBorderOptionsInfo> setBottomBorder,
					Action<ICell, ICell, MergedBorderInfo, MergedBorderOptionsInfo> setInsideBorders,
					MergedBorderInfo borderInfo, MergedBorderOptionsInfo optionsInfo) {
			if (currentCell.Position.Column == leftColumnIndex) {
				setLeftBorder(currentCell, borderInfo, optionsInfo);
				setTopBorder(currentCell, borderInfo, optionsInfo);
				setBottomBorder(currentCell, borderInfo, optionsInfo);
			}
			if (nextCell.Position.Column == rightColumnIndex) {
				setRightBorder(nextCell, borderInfo, optionsInfo);
				setTopBorder(nextCell, borderInfo, optionsInfo);
				setBottomBorder(nextCell, borderInfo, optionsInfo);
			}
			setInsideBorders(currentCell, nextCell, borderInfo, optionsInfo);
			SetDiagonalBorders(currentCell, borderInfo, optionsInfo);
			SetDiagonalBorders(nextCell, borderInfo, optionsInfo);
		}
		void ProcessBordersForTableRange(int leftColumnIndex, int rightColumnIndex, int topRowIndex, int bottomRowIndex, ICell currentCell, ICell nextCell,
					Action<ICell, MergedBorderInfo, MergedBorderOptionsInfo> setLeftBorder,
					Action<ICell, MergedBorderInfo, MergedBorderOptionsInfo> setRightBorder,
					Action<ICell, MergedBorderInfo, MergedBorderOptionsInfo> setTopBorder,
					Action<ICell, MergedBorderInfo, MergedBorderOptionsInfo> setBottomBorder,
					Action<ICell, ICell, MergedBorderInfo, MergedBorderOptionsInfo> setInsideBorders,
					MergedBorderInfo borderInfo, MergedBorderOptionsInfo optionsInfo) {
			if (currentCell.Position.Column == leftColumnIndex)
				setLeftBorder(currentCell, borderInfo, optionsInfo);
			if (nextCell.Position.Column == rightColumnIndex)
				setRightBorder(nextCell, borderInfo, optionsInfo);
			if (currentCell.Position.Row == topRowIndex)
				setTopBorder(currentCell, borderInfo, optionsInfo);
			if (nextCell.Position.Row == bottomRowIndex)
				setBottomBorder(nextCell, borderInfo, optionsInfo);
			setInsideBorders(currentCell, nextCell, borderInfo, optionsInfo);
			SetDiagonalBorders(currentCell, borderInfo, optionsInfo);
			SetDiagonalBorders(nextCell, borderInfo, optionsInfo);
		}
		void SetVerticalBorder(ICell leftCell, ICell rightCell, MergedBorderInfo borderInfo, MergedBorderOptionsInfo optionsInfo) {
			SetInsideBorder(leftCell, rightCell, MergedBorderInfoAccessor.Vertical, BorderSideAccessor.Right, BorderSideAccessor.Left, borderInfo, optionsInfo);
		}
		void SetHorizontalBorder(ICell topCell, ICell bottomCell, MergedBorderInfo borderInfo, MergedBorderOptionsInfo optionsInfo) {
			SetInsideBorder(topCell, bottomCell, MergedBorderInfoAccessor.Horizontal, BorderSideAccessor.Bottom, BorderSideAccessor.Top, borderInfo, optionsInfo);
		}
	}
	#endregion
}
