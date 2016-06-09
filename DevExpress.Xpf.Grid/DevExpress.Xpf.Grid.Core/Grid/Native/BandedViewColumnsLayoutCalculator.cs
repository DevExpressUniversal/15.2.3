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
using System.Text;
using DevExpress.Xpf.Grid.Native;
using System.Windows;
using System.Collections;
namespace DevExpress.Xpf.Grid.Native {
	public class BandedViewColumnsLayoutCalculator : ColumnsLayoutCalculator {
		public BandedViewColumnsLayoutCalculator(GridViewInfo viewInfo) : base(viewInfo) { }
		protected BandsLayoutBase BandsLayout { get { return ViewInfo.BandsLayout; } }
		protected override void CalcActualLayoutCore(double arrangeWidth, LayoutAssigner layoutAssigner, bool showIndicator, bool needRoundingLastColumn, bool ignoreDetailButtons) {
			ResetActualColumnHeaders(BandsLayout.VisibleBands, layoutAssigner);
			SetActualColumnWidth(GetSizeableColumns(false), layoutAssigner);
			foreach(BandBase band in BandsLayout.VisibleBands) {
				RecalcBandLayout(band, layoutAssigner);
			}
		}
		protected override void ApplyResizeCore(BaseColumn resizeColumn, double newWidth, double maxWidth, double indentWidth, bool correctWidths) {
			ColumnBase сolumnBase = resizeColumn as ColumnBase;
			if(correctWidths) {
				double delta = newWidth - GetColumnHeaderWidth(resizeColumn);
				if(!resizeColumn.IsBand) {
					delta = ChangeColumnSize(сolumnBase, delta, resizeColumn.BandRow.Columns);
					foreach(BandRow row in сolumnBase.ParentBand.ActualRows) {
						if(row != resizeColumn.BandRow)
							ChangeColumnSize(null, delta, row.Columns);
					}
				}
				else {
					delta = ChangeBandSize(resizeColumn.ParentBandInternal, delta);
				}
				OnBandResize(resizeColumn.ParentBandInternal, delta);
			} else {
				if(resizeColumn.Visible) {
					double delta = newWidth - GetColumnWidth(resizeColumn);
					resizeColumn.SetActualWidth(Math.Max(0, GetColumnWidth(resizeColumn) + CorrectColumnDelta(сolumnBase, delta, FixedStyle.None)));
				} else
					resizeColumn.SetActualWidth(newWidth);
			}
		}
		protected virtual void OnBandResize(BandBase band, double delta) {
			if(BandsLayout.GetRootBand(band).Fixed != FixedStyle.None)
				SetDefaultColumnSize(BandsLayout.GetBands(band, false, true));
		}
		protected void SetDefaultColumnSize(IList bands) {
			foreach(BandBase band in bands) {
				if(band.VisibleBands.Count != 0) {
					SetDefaultColumnSize(band.VisibleBands);
				}
				else if(band.ActualRows.Count != 0) {
					foreach(BandRow row in band.ActualRows)
						foreach(BaseColumn column in row.Columns)
							column.SetActualWidth(GetColumnWidth(column));
				}
				else {
					band.SetActualWidth(GetColumnWidth(band));
				}
			}
		}
		protected double ChangeColumnSize(ColumnBase resizeColumn, double delta, IList columns) {
			FixedStyle fixedStyle = FixedStyle.Left;
			if(resizeColumn != null) {
				fixedStyle = BandsLayout.GetRootBand(resizeColumn.ParentBand).Fixed;
				delta = CorrectColumnDelta(resizeColumn, delta, fixedStyle);
			}
			int columnIndex = columns.IndexOf(resizeColumn);
			SetColumnsWidth(columnIndex, delta, columns);
			if(delta > 0)
				return DecreaseColumnsWidth(resizeColumn, delta, columns, columnIndex, fixedStyle == FixedStyle.Right);
			else
				return IncreaseColumnsWidth(resizeColumn, delta, columns, columnIndex, fixedStyle == FixedStyle.Right);
		}
		void SetColumnsWidth(int columnIndex, double delta, IList columns) {
			for(int i = 0; i < columns.Count; i++) {
				BaseColumn column = (BaseColumn)columns[i];
				if(i == columnIndex)
					column.SetActualWidth(Math.Max(0, GetColumnWidth(column) + delta));
				else
					column.SetActualWidth(GetColumnWidth(column));
			}
		}
		double IncreaseColumnsWidth(BaseColumn resizeColumn, double delta, IList columns, int columnIndex, bool isLeft) {
			int unfixedColumnsCount = CalcUnfixedColumnsCount(columnIndex, columns, isLeft);
			double actualDelta = delta;
			for(int i = 0; i < columns.Count; i++) {
				if(BandsLayout.SkipItem(columnIndex, i, isLeft)) continue;
				BaseColumn column = (BaseColumn)columns[i];
				if(!column.FixedWidth) {
					double width = GetColumnWidth(column) - actualDelta / unfixedColumnsCount;
					double actualWidth = Math.Max(width, column.MinWidth);
					unfixedColumnsCount--;
					actualDelta -= GetColumnWidth(column) - actualWidth;
					column.SetActualWidth(actualWidth);
				}
			}
			return -actualDelta;
		}
		protected int CalcUnfixedColumnsCount(int columnIndex, IList columns, bool isLeft) {
			int unfixedColumnsCount = 0;
			for(int i = 0; i < columns.Count; i++) {
				if(BandsLayout.SkipItem(columnIndex, i, isLeft)) continue;
				if(!((BaseColumn)columns[i]).FixedWidth) unfixedColumnsCount++;
			}
			return unfixedColumnsCount;
		}
		int CalcUnfixedColumnsCount(IList bands) {
			int unfixedColumnsCount = 0;
			foreach(BandBase band in bands) {
				if(band.VisibleBands.Count != 0)
					unfixedColumnsCount += CalcUnfixedColumnsCount(band.VisibleBands);
				else if (band.ActualRows.Count != 0) {
					if (CanBeResized(band)) unfixedColumnsCount += CalcUnfixedColumnsCount(-1, GetBandRow(band, false).Columns, false);
				} else
					if (!band.FixedWidth)
						unfixedColumnsCount++;
			}
			return unfixedColumnsCount;
		}
		double CalcSizeableWidth(IList bands) {
			double width = 0;
			foreach(BandBase band in bands) {
				if(band.VisibleBands.Count != 0)
					width += CalcSizeableWidth(band.VisibleBands);
				else if(band.ActualRows.Count != 0)
					width += CalcSizeableWidth(-1, GetBandRow(band, false).Columns, false);
				else
					if(!band.FixedWidth)
						width += GetColumnWidth(band) - band.MinWidth;
			}
			return width;
		}
		double DecreaseColumnsWidth(BaseColumn resizeColumn, double delta, IList columns, int columnIndex, bool isLeft) {
			double sizeableWidth = CalcSizeableWidth(columnIndex, columns, isLeft);
			if(sizeableWidth == 0) return -delta;
			double actualDelta = delta;
			for(int i = 0; i < columns.Count; i++) {
				if(BandsLayout.SkipItem(columnIndex, i, isLeft)) continue;
				BaseColumn column = (BaseColumn)columns[i];
				if(!column.FixedWidth) {
					double width = GetColumnWidth(column) - (GetColumnWidth(column) - column.MinWidth) * Math.Min(sizeableWidth, delta) / sizeableWidth;
					double actualWidth = Math.Max(width, column.MinWidth);
					actualDelta -= GetColumnWidth(column) - actualWidth;
					column.SetActualWidth(actualWidth);
				}
			}
			return -actualDelta;
		}
		protected double CalcSizeableWidth(int columnIndex, IList columns, bool isLeft) {
			double width = 0;
			for(int i = 0; i < columns.Count; i++) {
				if(BandsLayout.SkipItem(columnIndex, i, isLeft)) continue;
				BaseColumn column = (BaseColumn)columns[i];
				if(!column.FixedWidth)
					width += GetColumnWidth(column) - column.MinWidth;
			}
			return width;
		}
		protected virtual double CorrectColumnDelta(ColumnBase column, double delta, FixedStyle fixedStyle) {
			if(fixedStyle != FixedStyle.None)
				delta = Math.Min(delta, TableViewBehavior.HorizontalViewportCore + CalcSizeableWidth(column.BandRow.Columns.IndexOf(column), column.BandRow.Columns, fixedStyle != FixedStyle.Left));
			double minWidth = column.MinWidth;
			int columnIndex = column.BandRow.Columns.IndexOf(column);
			if(column.BandRow.Columns[column.BandRow.Columns.Count - 1] == column) {
				double bandMinWidth = GetBandMinWidth(column.ParentBand, column.BandRow);
				minWidth = Math.Max(bandMinWidth - CalcColumnsWidth(column.BandRow.Columns, (col, index) => index >= columnIndex, true), minWidth);
				minWidth -= ViewInfo.GetHeaderIndentsWidth(column);
			}
			if(delta > 0 && HasFixedSizeRow(column))
				delta = Math.Min(delta, CalcSizeableWidth(columnIndex, column.BandRow.Columns, false));
			if(delta < 0 && column.ParentBand.ActualRows.Count > 1 && !HasUnfixedColumn(column.BandRow.Columns, columnIndex + 1))
				delta = Math.Max(delta, -CalcMinSizeableWidth(column));
			return Math.Max(GetColumnWidth(column) + delta, minWidth) - GetColumnWidth(column);
		}
		bool HasFixedSizeRow(ColumnBase resizeColumn) {
			foreach(BandRow row in resizeColumn.ParentBand.ActualRows) {
				if (row == resizeColumn.BandRow) continue;
				bool hasUnfixed = false;
				foreach(BaseColumn col in row.Columns) {
					if(!col.FixedWidth) hasUnfixed = true;
				}
				if(!hasUnfixed)
					return true;
			}
			return false;
		}
		bool HasUnfixedColumn(List<ColumnBase> columns, int index) {
			for(int i = index; i < columns.Count; i++)
				if(!columns[i].FixedWidth)
					return true;
			return false;
		}
		double CalcMinSizeableWidth(ColumnBase resizeColumn) {
			double minWidth = double.MaxValue;
			foreach(BandRow row in resizeColumn.ParentBand.ActualRows) {
				if(row == resizeColumn.BandRow) continue;
				minWidth = Math.Min(minWidth, CalcSizeableWidth(-1, row.Columns, true));
			}
			return minWidth;
		}
		protected virtual double CorrectBandDelta(BandBase band, double delta) {
			if(!CanBeResized(band)) return 0;
			if(BandsLayout.GetRootBand(band).Fixed != FixedStyle.None)
				delta = Math.Min(delta, TableViewBehavior.HorizontalViewportCore);
			if(BandsLayout.GetRootBand(band).Fixed == FixedStyle.Right) {
				double width = 0;
				foreach(BandBase subBand in BandsLayout.GetBands(band, true, true))
					width += subBand.ActualHeaderWidth;
				foreach(BandBase subBand in BandsLayout.GetBands(band, false, true))
					width += subBand.ActualHeaderWidth;
				delta = Math.Max(delta, GetArrangeWidth(ViewInfo.ColumnsLayoutSize, LayoutAssigner.Default, TableViewBehavior.TableView.ShowIndicator, false) - width - TableViewBehavior.HorizontalExtent - band.ActualHeaderWidth);
			}
			return Math.Max(band.ActualHeaderWidth + delta, GetBandMinWidth(band)) - band.ActualHeaderWidth;
		}
		bool CanBeResized(BandBase band) {
			bool hasSizeableRows = false;
			if(band.VisibleBands.Count != 0) {
				foreach(BandBase subBand in band.VisibleBands)
					if(CanBeResized(subBand))
						hasSizeableRows = true;
			} else if(band.ActualRows.Count != 0) {
				foreach(BandRow row in band.ActualRows) {
					bool hasUnfixed = false;
					foreach(BaseColumn col in row.Columns) {
						if(!col.FixedWidth) hasUnfixed = true;
					}
					if(!hasUnfixed) return false;
				}
				return true;
			} else {
				return !band.FixedWidth;
			}
			return hasSizeableRows;
		}
		internal double GetBandMinWidth(BandBase band, BandRow bandRow = null) {
			double minWidth = 0;
			if(band.VisibleBands.Count != 0) {
				foreach(BandBase subBand in band.VisibleBands) {
					minWidth += GetBandMinWidth(subBand);
				}
			} else if(band.ActualRows.Count != 0) {
				foreach(BandRow row in band.ActualRows) {
					if(row != bandRow)
						minWidth = Math.Max(minWidth, AutoWidthHelper.CalcColumnsFixedWidth(row.Columns, ViewInfo));
				}
			} else {
				minWidth += AutoWidthHelper.GetColumnFixedWidth(band, ViewInfo);
			}
			return minWidth;
		}
		double ChangeBandSize(BandBase band, double delta) {
			delta = CorrectBandDelta(band, delta);
			if(delta > 0)
				IncreaseBandsWidth(new List<BandBase>() { band }, delta);
			else
				DecreaseBandsWidth(new List<BandBase>() { band }, delta);
			return -delta;
		}
		protected void IncreaseBandsWidth(IList bands, double delta) {
			IncreaseBandsWidth(bands, delta, CalcUnfixedColumnsCount(bands));
		}
		void IncreaseBandsWidth(IList bands, double delta, int unfixedColumnsCount) {
			foreach(BandBase band in bands) {
				if(band.VisibleBands.Count != 0) {
					IncreaseBandsWidth(band.VisibleBands, delta, unfixedColumnsCount);
				} else if(band.ActualRows.Count != 0) {
					if(!CanBeResized(band)) continue;
					double oldWidth = 0;
					double width = 0;
					BandRow row = GetBandRow(band, false);
					foreach(BaseColumn column in row.Columns) {
						oldWidth += GetColumnWidth(column);
						double newWidth = GetColumnWidth(column);
						if(!column.FixedWidth) {
							newWidth = CalcColumnWidth(column, delta, unfixedColumnsCount);
							column.SetActualWidth(newWidth);
						}
						width += newWidth;
					}
					foreach(BandRow bandRow in band.ActualRows) {
						if(bandRow != row)
							ChangeColumnSize(null, oldWidth - width, bandRow.Columns);
					}
				} else {
					if(!band.FixedWidth)
						band.SetActualWidth(CalcColumnWidth(band, delta, unfixedColumnsCount));
				}
			}
		}
		double CalcColumnWidth(BaseColumn column, double delta, int unfixedColumnsCount) {
			double columnWidth = GetColumnWidth(column) + delta / unfixedColumnsCount;
			return Math.Max(columnWidth, column.MinWidth);
		}
		protected void DecreaseBandsWidth(IList bands, double delta) {
			double sizeableWidth = CalcSizeableWidth(bands);
			if(sizeableWidth == 0) return;
			DecreaseBandsWidth(bands, delta, sizeableWidth);
		}
		void DecreaseBandsWidth(IList bands, double delta, double sizeableWidth) {
			foreach(BandBase band in bands) {
				if(band.VisibleBands.Count != 0) {
					DecreaseBandsWidth(band.VisibleBands, delta, sizeableWidth);
				} else if(band.ActualRows.Count != 0) {
					double oldWidth = 0;
					double width = 0;
					BandRow row = GetBandRow(band, false);
					foreach(BaseColumn column in row.Columns) {
						oldWidth += GetColumnWidth(column);
						double newWidth = GetColumnWidth(column);
						if(!column.FixedWidth) {
							newWidth = CalcColumnWidth(column, delta, sizeableWidth);
							column.SetActualWidth(newWidth);
						}
						width += newWidth;
					}
					foreach(BandRow bandRow in band.ActualRows) {
						if(bandRow != row)
							ChangeColumnSize(null, oldWidth - width, bandRow.Columns);
					}
				}
				else {
					if(!band.FixedWidth)
						band.SetActualWidth(CalcColumnWidth(band, delta, sizeableWidth));
				}
			}
		}
		double CalcColumnWidth(BaseColumn column, double delta, double sizeableWidth) {
			double columnWidth = GetColumnWidth(column) + (GetColumnWidth(column) - column.MinWidth) * delta / sizeableWidth;
			return Math.Max(columnWidth, column.MinWidth);
		}
		protected double GetColumnHeaderWidth(BaseColumn column) {
			return column.ActualHeaderWidth;
		}
		protected double GetColumnWidth(BaseColumn column) {
			return GetColumnHeaderWidth(column) - ViewInfo.GetHeaderIndentsWidth(column);
		}
		protected double RecalcBandLayout(BandBase band, LayoutAssigner layoutAssigner) {
			if(band.VisibleBands.Count != 0) {
				layoutAssigner.SetWidth(band, 0);
				foreach(BandBase subBand in band.VisibleBands)
					layoutAssigner.SetWidth(band, layoutAssigner.GetWidth(band) + RecalcBandLayout(subBand, layoutAssigner));
			}
			else if(band.ActualRows.Count != 0) {
				layoutAssigner.SetWidth(band, 0);
				foreach(BandRow row in band.ActualRows) {
					double width = 0;
					foreach(BaseColumn column in row.Columns) {
						width += layoutAssigner.GetWidth(column);
					}
					if(!double.IsNaN(width))
						layoutAssigner.SetWidth(band, Math.Max(layoutAssigner.GetWidth(band), width));
				}
				foreach(BandRow row in band.ActualRows) {
					AutoWidthHelper.CalcColumnLayout(row.Columns, layoutAssigner.GetWidth(band), ViewInfo, layoutAssigner, true, false);
				}
			}
			return layoutAssigner.GetWidth(band);
		}
		protected IList GetSizeableColumns(bool useMinWidth) {
			return GetSizeableColumns(BandsLayout.VisibleBands, useMinWidth);
		}
		protected IList GetSizeableColumns(IList bands, bool useMinWidth) {
			List<BaseColumn> columns = new List<BaseColumn>();
			foreach(BandBase band in bands)
				columns.AddRange(GetBandColumns(band, useMinWidth));
			return columns;
		}
		protected void ResetActualColumnHeaders(IEnumerable bands, LayoutAssigner layoutAssigner) {
			foreach(BandBase band in bands) {
				if(band.VisibleBands.Count != 0)
					ResetActualColumnHeaders(band.VisibleBands, layoutAssigner);
				foreach(var row in band.ActualRows) {
					foreach(ColumnBase column in row.Columns)
						layoutAssigner.SetWidth(column, 0);
				}
			}
		}
		protected IEnumerable<BaseColumn> GetBandColumns(BandBase band, bool useMinWidth) {
			if(band.VisibleBands.Count != 0) {
				List<BaseColumn> columns = new List<BaseColumn>();
				foreach(BandBase subBand in band.VisibleBands)
					columns.AddRange(GetBandColumns(subBand, useMinWidth));
				return columns;
			}
			if(band.ActualRows.Count != 0) {
				return GetBandRow(band, useMinWidth).Columns;
			}
			return new BaseColumn[] { band };
		}
		internal BandRow GetBandRow(BandBase band, bool useMinWidth) {
			BandRow found = band.ActualRows[0];
			double width = 0;
			foreach(BandRow row in band.ActualRows) {
				double currentWidth = 0;
				foreach(ColumnBase column in row.Columns) {
					if(useMinWidth)
						currentWidth += AutoWidthHelper.GetColumnFixedWidth(column, ViewInfo);
					else
						currentWidth += ViewInfo.GetColumnDataWidth(column);
				}
				if(currentWidth > width) {
					found = row;
					width = currentWidth;
				}
			}
			return found;
		}
		protected override void UpdateColumnsActualAllowResizing() {
			base.UpdateColumnsActualAllowResizing();
			BandsLayout.ForeachBand(e => e.UpdateActualAllowResizing());
		}
		internal override void CorrectFixedColumnsWidth() {
			double horizontalViewport = GetArrangeWidth(ViewInfo.ColumnsLayoutSize, LayoutAssigner.Default, TableViewBehavior.TableView.ShowIndicator, false) - ViewInfo.TotalGroupAreaIndent;
			double totalFixedWidth = CalcColumnsWidth(BandsLayout.VisibleBands, (band, index) => band.Fixed == FixedStyle.None);
			double unfixedWidth = CalcColumnsWidth(BandsLayout.VisibleBands, (band, index) => band.Fixed != FixedStyle.None);
			if(horizontalViewport < totalFixedWidth) {
				UpdateFixedColumnsWidth(horizontalViewport + ViewInfo.TotalGroupAreaIndent, false);
			}
			if(horizontalViewport > totalFixedWidth + unfixedWidth) {
				UpdateFixedColumnsWidth(horizontalViewport - unfixedWidth - CalcColumnsWidth(BandsLayout.VisibleBands, (band, index) => band.Fixed != FixedStyle.Left), true);
			}
		}
		void UpdateFixedColumnsWidth(double arrangeWidth, bool skipLeft) {
			List<BandBase> fixedBands = new List<BandBase>();
			foreach(BandBase band in BandsLayout.VisibleBands) {
				if(band.Fixed == FixedStyle.None) continue;
				if(skipLeft && band.Fixed == FixedStyle.Left) continue;
				fixedBands.Add(band);
			}
			StretchColumnsToWidth(fixedBands, arrangeWidth, LayoutAssigner.Default, true);
		}
		protected void StretchColumnsToWidth(List<BandBase> bands, double arrangeWidth, LayoutAssigner layoutAssigner, bool needRoundingLastColumn) {
			ResetActualColumnHeaders(bands, layoutAssigner);
			AutoWidthHelper.CalcColumnLayout(GetSizeableColumns(bands, true), arrangeWidth, ViewInfo, layoutAssigner, needRoundingLastColumn, false);
			foreach(BandBase band in bands) {
				RecalcBandLayout(band, layoutAssigner);
			}
		}
		double CalcColumnsWidth(IList list, Func<BaseColumn, int, bool> skipColumn, bool useHeaderIndent = false) {
			double totalFixedWidth = 0;
			for (int i = 0; i < list.Count; i++) {
				BaseColumn column = (BaseColumn)list[i];
				if (!skipColumn(column, i))
					totalFixedWidth += useHeaderIndent ? GetColumnHeaderWidth(column) : GetColumnWidth(column);
			}
			return totalFixedWidth;
		}
	}
}
