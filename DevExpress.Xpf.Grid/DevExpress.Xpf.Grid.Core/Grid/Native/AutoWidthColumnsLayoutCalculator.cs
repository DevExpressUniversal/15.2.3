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

using System.Windows;
using System;
using System.Collections.Generic;
using System.Collections;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Grid.Native {
	class AutoWidthHelper {
		static double rest;
		public static void CalcColumnLayout(IList columns, double width, GridViewInfo viewInfo, LayoutAssigner layoutAssigner, bool needRoundingLastColumn, bool allowFixedWidth = true) {
			rest = 0;
			double totalWidth = viewInfo.GetDesiredColumnsWidth(columns);
			double fixedWidth = CalcColumnsFixedWidth(columns, viewInfo);
			if(totalWidth <= fixedWidth) {
				int unfixedVisibleColumnsCount = GetUnfixedColumnsCount(columns);
				double delta = width - totalWidth;
				if (unfixedVisibleColumnsCount != 0)
					delta = delta / unfixedVisibleColumnsCount;
				else if (allowFixedWidth)
					delta = 0;
				else
					delta = delta / columns.Count;
				delta = Math.Max(0, delta);
				if(double.IsInfinity(delta)) delta = 0;
				for(int i = 0; i < columns.Count; i++) {
					bool isLast = (i == columns.Count - 1) && !needRoundingLastColumn;
					BaseColumn column = (BaseColumn)columns[i];
					column.HeaderWidth = viewInfo.GetColumnHeaderWidth(column) + (IsFixedWidth(columns, column) && allowFixedWidth && unfixedVisibleColumnsCount != 0 ? 0 : delta);
					layoutAssigner.SetWidth(column, GetActualColumnWidth(column.HeaderWidth, isLast));
				}
				return;
			}
			double ratio = (width - fixedWidth) / (totalWidth - fixedWidth);
			ratio = Math.Max(0.0, ratio);
			if(double.IsInfinity(ratio)) ratio = 1d;
			for(int i = 0; i < columns.Count; i++) {
				double columnFixedWidth = GetColumnFixedWidth((BaseColumn)columns[i], viewInfo, IsFixedWidth(columns, (BaseColumn)columns[i]));
				bool isLast = (i == columns.Count - 1) && !needRoundingLastColumn;
				BaseColumn column = (BaseColumn)columns[i];
				column.HeaderWidth = Math.Max(0.0, (viewInfo.GetColumnHeaderWidth((BaseColumn)columns[i]) - columnFixedWidth)) * ratio + columnFixedWidth;
				layoutAssigner.SetWidth(column, GetActualColumnWidth(column.HeaderWidth, isLast));
			}
		}
		static bool IsFixedWidth(IList columns, BaseColumn column) {
			return column.FixedWidth;
		}
		static int GetUnfixedColumnsCount(IList columns) {
			int unfixedVisibleColumnsCount = 0;
			for(int i = 0; i < columns.Count; i++) {
				if(!IsFixedWidth(columns, (BaseColumn)columns[i]))
					unfixedVisibleColumnsCount++;
			}
			return unfixedVisibleColumnsCount;
		}
		public static double CalcColumnsFixedWidth(IEnumerable columns, GridViewInfo viewInfo) {
			double res = 0;
			foreach(BaseColumn column in columns) {
				res += GetColumnFixedWidth(column, viewInfo, IsFixedWidth(columns as IList, column));
			}
			return res;
		}
		public static double GetColumnFixedWidth(BaseColumn column, GridViewInfo viewInfo, bool allowFixedWidth = true) {
			if(column.FixedWidth && allowFixedWidth) return viewInfo.GetDesiredColumnWidth(column);
			return viewInfo.GetColumnFixedWidthCore(column);
		}
		static double GetActualColumnWidth(double columnWidth, bool isLastColumn = false) {
			double actualColumnWidth = DXArranger.Round((columnWidth + rest), 4);
			if(isLastColumn)
				return DXArranger.Floor(actualColumnWidth);
			actualColumnWidth = DXArranger.Round(actualColumnWidth, 0);
			rest += columnWidth - actualColumnWidth;
			return actualColumnWidth;
		}
	}
	public class AutoWidthColumnsLayoutCalculator : ColumnsLayoutCalculator {
		public AutoWidthColumnsLayoutCalculator(GridViewInfo viewInfo) : base(viewInfo) { }
		protected override void CalcActualLayoutCore(double arrangeWidth, LayoutAssigner layoutAssigner, bool showIndicator, bool needRoundingLastColumn, bool ignoreDetailButtons) {
			AutoWidthHelper.CalcColumnLayout(VisibleColumns as IList, arrangeWidth, ViewInfo, layoutAssigner, needRoundingLastColumn);
		}
		protected virtual void ExtractWidth() {
			for(int i = 0; i < VisibleColumns.Count; i++)
				ExtWidth[i] = VisibleColumns[i].HeaderWidth - ViewInfo.GetHeaderIndentsWidth(VisibleColumns[i]);
		}
		protected virtual double CalcMaxWidth(int resizeColumn) {
			double width = CalcColumnsExtWidth(0) - CalcColumnsExtWidth(0, resizeColumn, false) - CalcColumnsMinWidth(resizeColumn + 1);
			if(Math.Abs(width - ExtWidth[resizeColumn]) < 1.0) {
				if(resizeColumn < VisibleColumns.Count - 1 && VisibleColumns[resizeColumn + 1].GetAllowResizing()) {
					width += ExtWidth[resizeColumn + 1] - VisibleColumns[resizeColumn + 1].MinWidth;
				}
			}
			return width;
		}
		protected virtual double CalcColumnsExtWidth(int startColumn) { return CalcColumnsExtWidth(startColumn, ExtWidth.Length - startColumn, false); }
		protected virtual double CalcColumnsExtWidth(int startColumn, bool skipFixedWidth) { return CalcColumnsExtWidth(startColumn, ExtWidth.Length - startColumn, skipFixedWidth); }
		protected virtual double CalcColumnsExtWidth(int startColumn, int count, bool skipFixedWidth) {
			double res = 0.0;
			for(int i = startColumn; i < startColumn + count; i++) {
				if(skipFixedWidth && VisibleColumns[i].FixedWidth)
					continue;
				res += ExtWidth[i];
			}
			return res;
		}
		protected virtual double CalcColumnsMinWidth(int startColumn) { return CalcColumnsMinWidth(startColumn, VisibleColumns.Count - startColumn); }
		protected virtual double CalcColumnsMinWidth(int startColumn, int count) {
			double res = 0.0;
			for(int i = startColumn; i < startColumn + count; i++)
				res += VisibleColumns[i].FixedWidth ? ExtWidth[i] : VisibleColumns[i].MinWidth;
			return res;
		}
		public override double CalcColumnMaxWidth(ColumnBase column) {
			ExtractWidth();
			return CalcMaxWidth(GetVisibleIndex(column));
		}
		protected virtual int CalcNonFixedWidthColumnCount(int startIndex, int columnCount) {
			int count = 0;
			for(int i = startIndex; i < columnCount; i++) {
				if(!VisibleColumns[i].FixedWidth)
					count++;
			}
			return count;
		}
		protected virtual double GetDeltaWidth(int incStartIndex, double totalIncValue, int columnCount) {
			int count = CalcNonFixedWidthColumnCount(incStartIndex, columnCount);
			if(count == 0) return 0.0;
			return totalIncValue / count;
		}
		protected virtual void IncreaseWidths(int incStartIndex, double totalIncValue, int columnCount) {
			if(incStartIndex >= columnCount) return;
			double delta = GetDeltaWidth(incStartIndex, totalIncValue, columnCount);
			double res = 0.0;
			for(int i = incStartIndex; i < columnCount; i++) {
				if(VisibleColumns[i].FixedWidth) continue;
				res += delta;
				ExtWidth[i] += delta;
			}
			if(res == 0.0 && VisibleColumns[incStartIndex].GetAllowResizing()) {
				ExtWidth[incStartIndex] += totalIncValue;
			}
		}
		protected virtual double GetColumnSizeableWidth(int columnIndex) {
			if(VisibleColumns[columnIndex].FixedWidth) return 0.0;
			return Math.Max(0.0, ExtWidth[columnIndex] - VisibleColumns[columnIndex].MinWidth);
		}
		protected virtual double DecreaseWidth(int columnIndex, double delta) {
			double columnDelta = GetColumnSizeableWidth(columnIndex);
			if(columnDelta == 0.0) return delta;
			delta = Math.Min(columnDelta, delta);
			ExtWidth[columnIndex] -= delta;
			return delta;
		}
		protected virtual double CalcTotalColumnsSizeableWidth(int startIndex, int columnCount) {
			double res = 0.0;
			for(int i = startIndex; i < columnCount; i++) {
				res += GetColumnSizeableWidth(i);
			}
			return res;
		}
		protected virtual void DecreaseWidths(int decStartIndex, double totalDecValue, int columnCount) {
			if(decStartIndex >= columnCount) return;
			double totalColDecWidth = CalcTotalColumnsSizeableWidth(decStartIndex, columnCount);
			if(totalColDecWidth < totalDecValue) {
				if(VisibleColumns[decStartIndex].GetAllowResizing()) {
					ExtWidth[decStartIndex] = Math.Max(VisibleColumns[decStartIndex].MinWidth, ExtWidth[decStartIndex] - totalDecValue);
				}
			}
			for(int i = decStartIndex; i < columnCount; i++) {
				DecreaseWidth(i, GetColumnSizeableWidth(i) / totalColDecWidth * totalDecValue);
			}
		}
		protected virtual void UpdateWidths(int columnsIndex, double delta) {
			if(columnsIndex == VisibleColumns.Count - 1) {
				if(delta < 0)
					IncreaseWidths(0, -delta, VisibleColumns.Count - 1);
				else
					DecreaseWidths(0, delta, VisibleColumns.Count - 1);
			} else {
				int startIndex = columnsIndex + 1;
				if(delta < 0)
					IncreaseWidths(startIndex, -delta, VisibleColumns.Count);
				else
					DecreaseWidths(startIndex, delta, VisibleColumns.Count);
			}
		}
		protected virtual void UpdateColumnsWidthFromExtWidth() {
			for(int i = 0; i < VisibleColumns.Count; i++)
				VisibleColumns[i].SetActualWidth(ExtWidth[i]);
		}
		protected override void ApplyResizeCore(BaseColumn resizeColumn, double newWidth, double maxWidth, double indentWidth, bool correctWidths) {
			ExtractWidth();
			newWidth = Math.Max(resizeColumn.MinWidth, newWidth - indentWidth);
			newWidth = Math.Min(maxWidth, newWidth);
			bool calcLayout = resizeColumn.Visible && ViewInfo.GridView.IsColumnVisibleInHeaders(resizeColumn);
			if(correctWidths && calcLayout) {
				int index = GetVisibleIndex(resizeColumn);
				UpdateWidths(index, newWidth - ExtWidth[index]);
				ExtWidth[index] = newWidth;
				UpdateColumnsWidthFromExtWidth();
			} else {
				resizeColumn.SetActualWidth(newWidth);
			}
			UpdateColumnsActualAllowResizing();
		}
	}
}
