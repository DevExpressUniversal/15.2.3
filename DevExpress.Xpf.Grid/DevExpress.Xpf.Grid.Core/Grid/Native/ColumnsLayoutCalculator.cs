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

using System.Collections.Generic;
using System.Windows;
using System;
using System.Linq;
using System.Collections;
namespace DevExpress.Xpf.Grid.Native {
	public class LayoutAssigner {
		public static readonly LayoutAssigner Default = new LayoutAssigner();
		public virtual double GetWidth(BaseColumn column) {
			return column.ActualHeaderWidth;
		}
		public virtual void SetWidth(BaseColumn column, double value) {
			column.ActualHeaderWidth = value;
		}
		public virtual ColumnPosition GetColumnPosition(BaseColumn column) {
			return column.ColumnPosition;
		}
		public virtual void SetColumnPosition(BaseColumn column, ColumnPosition position) {
			column.ColumnPosition = position;
		}
		public virtual bool GetHasRightSibling(BaseColumn column) {
			return column.HasRightSibling;
		}
		public virtual void SetHasRightSibling(BaseColumn column, bool value) {
			column.HasRightSibling = value;
		}
		public virtual bool GetHasLeftSibling(BaseColumn column) {
			return column.HasLeftSibling;
		}
		public virtual void SetHasLeftSibling(BaseColumn column, bool value) {
			column.HasLeftSibling = value;
		}
		public virtual void CreateLayout(ColumnsLayoutCalculator calculator) {
			calculator.CreateLayout();
		}
		public virtual bool UseDataAreaIndent { get { return true; } }
		public virtual bool UseFixedColumnIndents { get { return true; } }
		public virtual bool UseDetailButtonsIndents { get { return true; } }
	}
	public class ColumnsLayoutCalculator {
#if DEBUGTEST
		internal static int CalcLayoutCount { get; private set; }
#endif
		GridViewInfo viewInfo;
		double[] extWidth;
		public ColumnsLayoutCalculator(GridViewInfo viewInfo) {
			this.viewInfo = viewInfo;
		}
		protected double[] ExtWidth {
			get {
				if(extWidth == null && VisibleColumns.Count > 0) extWidth = new double[VisibleColumns.Count];
				return extWidth;
			}
		}
		protected virtual GridViewInfo ViewInfo { get { return viewInfo; } }
		protected TableViewBehavior TableViewBehavior { get { return ViewInfo.TableView.TableViewBehavior; } }
		protected IList<ColumnBase> VisibleColumns { get { return ViewInfo.VisibleColumns; } }
#if DEBUGTEST
		protected virtual double GetColumnFixedWidth(int index) {
			return AutoWidthHelper.GetColumnFixedWidth(VisibleColumns[index], ViewInfo);
		}
		protected virtual double CalcColumnsFixedWidth() {
			return AutoWidthHelper.CalcColumnsFixedWidth(VisibleColumns, ViewInfo);
		}
#endif
		public void CalcActualLayout(Size arrangeBounds) {
			CalcActualLayout(arrangeBounds, LayoutAssigner.Default, TableViewBehavior.TableView.ActualShowIndicator, false, false);
		}
		double rest;
		public void CalcActualLayout(Size arrangeBounds, LayoutAssigner layoutAssigner, bool showIndicator, bool needRoundingLastColumn, bool ignoreDetailButtons) {
			if(ViewInfo.GridView.IsLockUpdateColumnsLayout)
				return;
			rest = 0;
			double arrangeWidth = GetArrangeWidth(arrangeBounds, layoutAssigner, showIndicator, ignoreDetailButtons);
			CalcActualLayoutCore(arrangeWidth, layoutAssigner, showIndicator, needRoundingLastColumn, ignoreDetailButtons);
			layoutAssigner.CreateLayout(this);
			UpdateAdditionalRowDataWidth();
#if DEBUGTEST
			CalcLayoutCount++;
#endif
		}
		protected double GetActualColumnWidth(double columnWidth, bool isLastColumn = false) {
			if(isLastColumn)
				return Math.Floor(columnWidth + rest);
			double actualColumnWidth = Math.Round(columnWidth + rest);
			rest += columnWidth - actualColumnWidth;
			return actualColumnWidth;
		}
		protected virtual void UpdateAdditionalRowDataWidth() {
			for(int i = 0; i < VisibleColumns.Count; i++) {
				VisibleColumns[i].ActualAdditionalRowDataWidth = VisibleColumns[i].ActualHeaderWidth - (!ViewInfo.TableView.ShowIndicator && i == 0 ? ViewInfo.TableView.LeftDataAreaIndent : 0);
			}
		}
		double CorrectNewWidth(BaseColumn column, double width) {
			if(!TableViewBehavior.BestFitLocker.IsLocked && column.Fixed != FixedStyle.None) {
				double totalFixedWidth = CalcTotalFixedWidth();
				double horizontalViewport = CalcHorizontalViewPort();
				double newTotalFixedWidth = totalFixedWidth + width - column.ActualHeaderWidth;
				for(int i = 0; i < VisibleColumns.Count; i++)
					if(VisibleColumns[i].Fixed != FixedStyle.None)
						VisibleColumns[i].SetActualWidth(VisibleColumns[i].ActualHeaderWidth - ViewInfo.GetHeaderIndentsWidth(VisibleColumns[i]));
				if(newTotalFixedWidth > horizontalViewport && horizontalViewport > 0)
					width = horizontalViewport - totalFixedWidth + column.ActualHeaderWidth; 
			}
			return width;
		}
		double CalcTotalFixedWidth() {
			double totalFixedWidth = 0;
			for(int i = 0; i < VisibleColumns.Count; i++)
				if(VisibleColumns[i].Fixed != FixedStyle.None)
					totalFixedWidth += VisibleColumns[i].ActualHeaderWidth - ViewInfo.GetHeaderIndentsWidth(VisibleColumns[i]);
			return totalFixedWidth;
		}
		double CalcAutoWidthHorizontalViewPort() {
			bool hasFixedLeftColumns = VisibleColumns.Where(c => c.Fixed == FixedStyle.Left).ToList().Count > 0;
			bool hasFixedRightColumns = VisibleColumns.Where(c => c.Fixed == FixedStyle.Right).ToList().Count > 0;
			bool hasFixedColumns = hasFixedLeftColumns || hasFixedRightColumns;
			double firstColumnIndent = 0;
			if(viewInfo.TableView.ActualShowIndicator)
				firstColumnIndent += ViewInfo.TableView.IndicatorHeaderWidth;
			if(!viewInfo.TableView.ActualShowIndicator && !hasFixedLeftColumns && hasFixedRightColumns)
				firstColumnIndent += ViewInfo.TableView.LeftDataAreaIndent;
			if(!viewInfo.TableView.ActualShowDetailButtons && !hasFixedLeftColumns && hasFixedRightColumns)
				firstColumnIndent += ViewInfo.TotalGroupAreaIndent;
			if(viewInfo.TableView.ActualShowDetailButtons && hasFixedColumns)
				firstColumnIndent += ViewInfo.TotalGroupAreaIndent;
			if(viewInfo.TableView.ActualShowDetailButtons)
				firstColumnIndent += ViewInfo.TableView.ActualExpandDetailButtonWidth;
			return CorrectValueOnFixedLineWidth(ViewInfo.ColumnsLayoutSize.Width - firstColumnIndent - GridViewInfo.FixedNoneMinWidth);
		}
		double CalcHorizontalViewPort() {
			double leftIndent = ViewInfo.TableView.LeftDataAreaIndent + ViewInfo.TotalGroupAreaIndent;
			if(viewInfo.TableView.ActualShowIndicator)
				leftIndent += ViewInfo.TableView.IndicatorWidth;
			if(viewInfo.TableView.ActualShowDetailButtons)
				leftIndent += ViewInfo.TableView.ActualExpandDetailButtonWidth;
			return CorrectValueOnFixedLineWidth(ViewInfo.ColumnsLayoutSize.Width - leftIndent - GridViewInfo.FixedNoneMinWidth);
		}
		internal virtual void CorrectFixedColumnsWidth() {
			double totalFixedWidth = CalcTotalFixedWidth();
			double horizontalViewport = CalcHorizontalViewPort();
			if(horizontalViewport < totalFixedWidth && horizontalViewport > 0) {
				List<ColumnBase> fixedColumns = VisibleColumns.Where(c => c.Fixed != FixedStyle.None).ToList();
				AutoWidthHelper.CalcColumnLayout(fixedColumns, CalcAutoWidthHorizontalViewPort(), viewInfo, LayoutAssigner.Default, false);
			}
		}
		double CorrectValueOnFixedLineWidth(double value) {
			if(TableViewBehavior.HasFixedLeftElements)
				value -= ViewInfo.TableView.FixedLineWidth;
			if(TableViewBehavior.HasFixedRightElements)
				value -= ViewInfo.TableView.FixedLineWidth;
			return value;
		}
		protected internal virtual void CreateLayout() {
			CorrectFixedColumnsWidth();
			double totalFixedNoneSize = 0;
			double totalFixedLeftSize = 0;
			double totalFixedRightSize = 0;
			UpdateColumnDataWidths(out totalFixedNoneSize, out totalFixedLeftSize, out totalFixedRightSize);
			double leftIndent = ViewInfo.TotalGroupAreaIndent + totalFixedLeftSize;
			double fixedLeftIndent = 0;
			double scrollableLeftIndent = 0;
			if(ViewInfo.TableView.ActualAllowTreeIndentScrolling)
				scrollableLeftIndent = leftIndent;
			else
				fixedLeftIndent = leftIndent;
			double rightIndent = ViewInfo.RightGroupAreaIndent + totalFixedRightSize;
			double viewport = Math.Max(0, CorrectValueOnFixedLineWidth(ViewInfo.ColumnsLayoutSize.Width - fixedLeftIndent - rightIndent));
			if(ViewInfo.TableView.ActualShowIndicator && viewport >= ViewInfo.TableView.IndicatorWidth)
				viewport -= ViewInfo.TableView.IndicatorWidth;
			viewport = Math.Max(viewport, GridViewInfo.FixedNoneMinWidth);
			viewport = CorrectDetailExpandButtonAndVerticalScrollBarWidth(viewport);
			if(double.IsInfinity(viewport)) {
				viewport = totalFixedNoneSize;
			}
			TableViewBehavior.HorizontalExtent = totalFixedNoneSize + scrollableLeftIndent;
			TableViewBehavior.LeftIndent = leftIndent;
			TableViewBehavior.RightIndent = rightIndent;
			TableViewBehavior.TableView.SetHorizontalViewport(viewport);
			ViewInfo.TableView.IndicatorHeaderWidth = ViewInfo.TableView.IndicatorWidth + ViewInfo.TableView.LeftDataAreaIndent;
			ViewInfo.TableView.SetActualExpandDetailHeaderWidth(ViewInfo.TableView.ActualExpandDetailButtonWidth + ViewInfo.TotalGroupAreaIndent);
			double fixedNoneContentWidth = Math.Min(viewport, totalFixedNoneSize + scrollableLeftIndent);
			if(totalFixedLeftSize == 0) {
				fixedNoneContentWidth += fixedLeftIndent;
				if(ViewInfo.TableView.ActualShowDetailButtons)
					fixedNoneContentWidth -= ViewInfo.TotalGroupAreaIndent;
				if(!ViewInfo.TableView.ActualShowIndicator)
					fixedNoneContentWidth += ViewInfo.TableView.LeftDataAreaIndent;
			}
			if(totalFixedRightSize == 0)
				fixedNoneContentWidth += TableViewBehavior.RightIndent;
			ViewInfo.TableView.FixedNoneContentWidth = fixedNoneContentWidth;
			ViewInfo.TableView.FixedLeftContentWidth = Math.Max(0, totalFixedLeftSize);
			ViewInfo.TableView.FixedRightContentWidth = Math.Max(0, totalFixedRightSize);
			ViewInfo.TableView.TotalGroupAreaIndent = ViewInfo.TotalGroupAreaIndent;
			ViewInfo.TableView.TotalSummaryFixedNoneContentWidth = GetTotalSummaryFixedNoneContentWidth();
			ViewInfo.TableView.VerticalScrollBarWidth = ViewInfo.VerticalScrollBarWidth;
			UpdateColumnsActualAllowResizing();
			TableViewBehavior.FillByLastFixedColumn(GetArrangeWidth(ViewInfo.ColumnsLayoutSize, LayoutAssigner.Default, TableViewBehavior.TableView.ShowIndicator, false));
		}
		double GetTotalSummaryFixedNoneContentWidth() {
			double result = ViewInfo.TableView.FixedNoneContentWidth;
			if(ViewInfo.TableView.FixedLeftContentWidth != 0 && ViewInfo.TableView.FixedRightContentWidth != 0)
				return result;
			if(ViewInfo.TableView.ViewBase.IsRootView && ViewInfo.TableView.ActualShowIndicator)
				result += ViewInfo.TableView.ActualIndicatorWidth;
			if(ViewInfo.TableView.ActualShowDetailButtons)
				result += ViewInfo.TableView.ActualExpandDetailHeaderWidth;
			if(ViewInfo.TableView.FixedRightContentWidth != 0)
				return result;
			return result + ViewInfo.VerticalScrollBarWidth;
		}
		void UpdateColumnDataWidths(out double totalFixedNoneSize, out double totalFixedLeftSize, out double totalFixedRightSize) {
			totalFixedNoneSize = 0;
			totalFixedLeftSize = 0;
			totalFixedRightSize = 0;
			for(int i = 0; i < VisibleColumns.Count; i++) {
				VisibleColumns[i].ActualDataWidth = VisibleColumns[i].ActualHeaderWidth - ViewInfo.GetHeaderIndentsWidth(VisibleColumns[i]);
			}
			TableViewBehavior.UpdateColumnDataWidths(out totalFixedNoneSize, out totalFixedLeftSize, out totalFixedRightSize);
		}
		protected double CorrectDetailExpandButtonAndVerticalScrollBarWidth(double value) {
			double indent = ViewInfo.TableView.TableViewBehavior.GetTotalLeftIndent(false, false) + ViewInfo.TableView.TableViewBehavior.GetTotalRightIndent();
			return Math.Max(0, value - indent);
		}
		protected internal virtual void UpdateHasLeftRightSibling(IList<ColumnBase> columns) {
			for(int i = 0; i < columns.Count; i++) {
				columns[i].HasRightSibling = i < (columns.Count - 1);
				columns[i].HasLeftSibling = i != 0;
			}
		}
		protected virtual void UpdateColumnsActualAllowResizing() {
			for(int i = 0; i < VisibleColumns.Count; i++)
				VisibleColumns[i].UpdateActualAllowResizing();
		}
		protected virtual void CalcActualLayoutCore(double arrangeWidth, LayoutAssigner layoutAssigner, bool showIndicator, bool needRoundingLastColumn, bool ignoreDetailButtons) {
			SetActualColumnWidth(VisibleColumns, layoutAssigner);
		}
		protected void SetActualColumnWidth(IEnumerable columns, LayoutAssigner layoutAssigner) {
			foreach(BaseColumn column in columns)
				layoutAssigner.SetWidth(column, GetActualColumnWidth(ViewInfo.GetColumnHeaderWidth(column)));
		}
		protected virtual double GetArrangeWidth(Size arrangeBounds, LayoutAssigner layoutAssigner, bool showIndicator, bool ignoreDetailButtons) {
			double arrangeWidth = arrangeBounds.Width - ViewInfo.RightGroupAreaIndent;
			if(showIndicator)
				arrangeWidth -= ViewInfo.TableView.ActualIndicatorWidth;
			if(TableViewBehavior.HasFixedLeftElements && layoutAssigner.UseFixedColumnIndents)
				arrangeWidth -= ViewInfo.TableView.FixedLineWidth;
			if(TableViewBehavior.HasFixedRightElements && layoutAssigner.UseFixedColumnIndents)
				arrangeWidth -= ViewInfo.TableView.FixedLineWidth;
			if(layoutAssigner.UseDetailButtonsIndents)
				arrangeWidth = CorrectDetailExpandButtonAndVerticalScrollBarWidth(arrangeWidth);
			if(ViewInfo.TableView.ActualShowDetailButtons && arrangeWidth >= ViewInfo.TotalGroupAreaIndent && !ignoreDetailButtons)
				arrangeWidth -= ViewInfo.TotalGroupAreaIndent;
			if(!ViewInfo.TableView.ShowIndicator && layoutAssigner.UseDataAreaIndent)
				arrangeWidth += ViewInfo.TableView.LeftDataAreaIndent;
			return arrangeWidth;
		}
		protected int GetVisibleIndex(BaseColumn column) { return ColumnBase.GetVisibleIndex(column); }
		public void ApplyResize(BaseColumn resizeColumn, double newWidth, double maxWidth) {
			int columnIndex = GetVisibleIndex(resizeColumn);
			ApplyResize(resizeColumn, newWidth, maxWidth, ViewInfo.GetHeaderIndentsWidth(resizeColumn));
		}
		public void ApplyResize(BaseColumn resizeColumn, double newWidth, double maxWidth, double indentWidth) {
			ApplyResize(resizeColumn, newWidth, maxWidth, indentWidth, true);
		}
		public virtual void ApplyResize(BaseColumn resizeColumn, double newWidth, double maxWidth, double indentWidth, bool correctWidths) {
			if(double.IsNaN(newWidth)) {
				SetDefaultWidths();
				return;
			}
			if(!resizeColumn.GetAllowResizing()) return;
			ViewInfo.GridView.UpdateAllDependentViews(view => view.BeginUpdateColumnsLayout());
			ApplyResizeCore(resizeColumn, newWidth, maxWidth, indentWidth, correctWidths);
			ViewInfo.GridView.UpdateAllDependentViews(view => view.EndUpdateColumnsLayout());
		}		
		protected virtual void ApplyResizeCore(BaseColumn resizeColumn, double newWidth, double maxWidth, double indentWidth, bool correctWidths) {
			newWidth = CorrectNewWidth(resizeColumn, newWidth);
			resizeColumn.SetActualWidth(Math.Max(0, newWidth - indentWidth));
		}
		public virtual double CalcColumnMaxWidth(ColumnBase column) { return double.MaxValue; }
		protected virtual void SetDefaultWidths() {
			foreach(ColumnBase column in VisibleColumns) {
				column.SetActualWidth(column.Width);
			}
			ViewInfo.GridView.RebuildVisibleColumns();
		}
	}
}
