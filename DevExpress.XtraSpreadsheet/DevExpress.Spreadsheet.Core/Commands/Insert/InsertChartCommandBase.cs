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
using System.Globalization;
using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Commands.Internal;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region InsertChartCommandBase (abstract class)
	public abstract class InsertChartCommandBase : SpreadsheetMenuItemSimpleCommand {
		static readonly List<SpreadsheetCommandId> insertChartCommands = CreateInsertChartCommands();
		#region CreateInsertChartCommands
		static List<SpreadsheetCommandId> CreateInsertChartCommands() {
			List<SpreadsheetCommandId> result = new List<SpreadsheetCommandId>();
			result.Add(SpreadsheetCommandId.InsertChartColumnClustered2D);
			result.Add(SpreadsheetCommandId.InsertChartColumnStacked2D);
			result.Add(SpreadsheetCommandId.InsertChartColumnPercentStacked2D);
			result.Add(SpreadsheetCommandId.InsertChartColumnClustered3D);
			result.Add(SpreadsheetCommandId.InsertChartColumnStacked3D);
			result.Add(SpreadsheetCommandId.InsertChartColumnPercentStacked3D);
			result.Add(SpreadsheetCommandId.InsertChartCylinderClustered);
			result.Add(SpreadsheetCommandId.InsertChartCylinderStacked);
			result.Add(SpreadsheetCommandId.InsertChartCylinderPercentStacked);
			result.Add(SpreadsheetCommandId.InsertChartConeClustered);
			result.Add(SpreadsheetCommandId.InsertChartConeStacked);
			result.Add(SpreadsheetCommandId.InsertChartConePercentStacked);
			result.Add(SpreadsheetCommandId.InsertChartPyramidClustered);
			result.Add(SpreadsheetCommandId.InsertChartPyramidStacked);
			result.Add(SpreadsheetCommandId.InsertChartPyramidPercentStacked);
			result.Add(SpreadsheetCommandId.InsertChartBarClustered2D);
			result.Add(SpreadsheetCommandId.InsertChartBarStacked2D);
			result.Add(SpreadsheetCommandId.InsertChartBarPercentStacked2D);
			result.Add(SpreadsheetCommandId.InsertChartBarClustered3D);
			result.Add(SpreadsheetCommandId.InsertChartBarStacked3D);
			result.Add(SpreadsheetCommandId.InsertChartBarPercentStacked3D);
			result.Add(SpreadsheetCommandId.InsertChartHorizontalCylinderClustered);
			result.Add(SpreadsheetCommandId.InsertChartHorizontalCylinderStacked);
			result.Add(SpreadsheetCommandId.InsertChartHorizontalCylinderPercentStacked);
			result.Add(SpreadsheetCommandId.InsertChartHorizontalConeClustered);
			result.Add(SpreadsheetCommandId.InsertChartHorizontalConeStacked);
			result.Add(SpreadsheetCommandId.InsertChartHorizontalConePercentStacked);
			result.Add(SpreadsheetCommandId.InsertChartHorizontalPyramidClustered);
			result.Add(SpreadsheetCommandId.InsertChartHorizontalPyramidStacked);
			result.Add(SpreadsheetCommandId.InsertChartHorizontalPyramidPercentStacked);
			result.Add(SpreadsheetCommandId.InsertChartColumn3D);
			result.Add(SpreadsheetCommandId.InsertChartCylinder);
			result.Add(SpreadsheetCommandId.InsertChartCone);
			result.Add(SpreadsheetCommandId.InsertChartPyramid);
			result.Add(SpreadsheetCommandId.InsertChartPie2D);
			result.Add(SpreadsheetCommandId.InsertChartPie3D);
			result.Add(SpreadsheetCommandId.InsertChartPieExploded2D);
			result.Add(SpreadsheetCommandId.InsertChartPieExploded3D);
			result.Add(SpreadsheetCommandId.InsertChartLine);
			result.Add(SpreadsheetCommandId.InsertChartStackedLine);
			result.Add(SpreadsheetCommandId.InsertChartPercentStackedLine);
			result.Add(SpreadsheetCommandId.InsertChartLineWithMarkers);
			result.Add(SpreadsheetCommandId.InsertChartStackedLineWithMarkers);
			result.Add(SpreadsheetCommandId.InsertChartPercentStackedLineWithMarkers);
			result.Add(SpreadsheetCommandId.InsertChartLine3D);
			result.Add(SpreadsheetCommandId.InsertChartArea);
			result.Add(SpreadsheetCommandId.InsertChartStackedArea);
			result.Add(SpreadsheetCommandId.InsertChartPercentStackedArea);
			result.Add(SpreadsheetCommandId.InsertChartArea3D);
			result.Add(SpreadsheetCommandId.InsertChartStackedArea3D);
			result.Add(SpreadsheetCommandId.InsertChartPercentStackedArea3D);
			result.Add(SpreadsheetCommandId.InsertChartScatterMarkers);
			result.Add(SpreadsheetCommandId.InsertChartScatterLines);
			result.Add(SpreadsheetCommandId.InsertChartScatterSmoothLines);
			result.Add(SpreadsheetCommandId.InsertChartScatterLinesAndMarkers);
			result.Add(SpreadsheetCommandId.InsertChartScatterSmoothLinesAndMarkers);
			result.Add(SpreadsheetCommandId.InsertChartBubble);
			result.Add(SpreadsheetCommandId.InsertChartBubble3D);
			result.Add(SpreadsheetCommandId.InsertChartDoughnut2D);
			result.Add(SpreadsheetCommandId.InsertChartDoughnutExploded2D);
			result.Add(SpreadsheetCommandId.InsertChartRadar);
			result.Add(SpreadsheetCommandId.InsertChartRadarWithMarkers);
			result.Add(SpreadsheetCommandId.InsertChartRadarFilled);
			result.Add(SpreadsheetCommandId.InsertChartStockHighLowClose);
			result.Add(SpreadsheetCommandId.InsertChartStockOpenHighLowClose);
			return result;
		}
		#endregion
		ChartSeriesRangeModelBase rangeModel;
		protected InsertChartCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected SheetViewSelection Selection { get { return DocumentModel.ActiveSheet.Selection; } }
		protected virtual ChartLayoutModifier Preset { get { return null; } }
		protected virtual AxisCrossBetween ValueAxisCrossBetween { get { return AxisCrossBetween.Between; } }
		protected abstract ChartViewType ViewType { get; }
		protected internal override void ExecuteCore() {
			CellRange range = Selection.ActiveRange;
			if (range == null)
				return;
			bool isColumnRange = range.IsColumnRangeInterval();
			bool isRowRange = range.IsRowRangeInterval();
			bool isWholeWorksheet = range.IsWholeWorksheetRange();
			if (isColumnRange || isRowRange || isWholeWorksheet) {
				Worksheet sheet = range.Worksheet as Worksheet;
				CellRange usedRange = sheet.GetUsedRange();
				if (isWholeWorksheet)
					range = new CellRange(sheet, new CellPosition(0, 0), usedRange.BottomRight);
				else if (isColumnRange)
					range = new CellRange(sheet, new CellPosition(range.TopLeft.Column, 0), new CellPosition(range.BottomRight.Column, usedRange.BottomRight.Row));
				else 
					range = new CellRange(sheet, new CellPosition(0, range.TopLeft.Row), new CellPosition(usedRange.BottomRight.Column, range.BottomRight.Row));
			}
			if (!ValidateRange(range))
				return;
			this.rangeModel = ChartRangesCalculator.CreateModel(range, ViewType);
			CellRange seriesRange = rangeModel.Data.SeriesValuesRange;
			CellRange seriesNamesRange = rangeModel.Data.SeriesNameRange;
			CellRange axisLabelsRange = null;
			ChartDataReference arguments = rangeModel.Data.SeriesArguments as ChartDataReference;
			if (arguments != null)
				axisLabelsRange = arguments.CachedValue.CellRangeValue as CellRange;
			DocumentModel.BeginUpdate();
			try {
				InsertChart(seriesRange, axisLabelsRange, seriesNamesRange);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		bool ValidateRange(CellRange range) {
			Worksheet sheet = range.Worksheet as Worksheet;
			if (sheet != null && sheet.PivotTables.ContainsItemsInRange(range, true)) {
				IModelErrorInfo error = new ModelErrorInfo(ModelErrorType.ChartDataRangeIntersectPivotTable);
				if (ErrorHandler.HandleError(error) == ErrorHandlingResult.Abort)
					return false;
			}
			return true;
		}
		protected virtual void InsertChart(CellRange seriesRange, CellRange axisLabelsRange, CellRange seriesNamesRange) {
			Chart chart = new Chart(ActiveSheet);
			chart.BeginUpdate();
			try {
				SetupDrawingObjectProperties(chart);
				CalculateChartPosition(chart.DrawingObject);
				SetupChart(chart);
				ChangeChartType(chart, seriesRange, axisLabelsRange, seriesNamesRange);
				ApplyPreset(chart);
			}
			finally {
				chart.EndUpdate();
			}
			ActiveSheet.InsertChart(chart);
			ActiveSheet.Selection.SetSelectedDrawingIndex(chart.IndexInCollection);
		}
		void SetupDrawingObjectProperties(Chart chart) {
			int id = ActiveSheet.DrawingObjects.GetMaxDrawingId();
			chart.DrawingObject.Properties.Id = id + 1;
			chart.DrawingObject.Properties.Name = string.Format("Chart {0}", id);
		}
		public void ChangeChartType(Chart chart) {
				ChangeChartTypeCore(chart);
		}
		protected virtual void ChangeChartTypeCore(Chart chart) {
			chart.BeginUpdate();
			try {
				List<AxisBase> savedAxes = new List<AxisBase>(chart.PrimaryAxes.InnerList);
				chart.PrimaryAxes.Clear();
				chart.SecondaryAxes.Clear();
				CreateAxes(chart);
				IChartView view = CreateChartView(chart);
				view.Axes = chart.PrimaryAxes;
				RestoreAxes(chart, savedAxes, IsPercentStacked(view) || IsPercentStacked(chart));
				CreateViewSeries(view, chart);
				SetupViewDataLabels(view as ChartViewWithDataLabels);
				bool was3DChart = chart.Is3DChart;
				chart.Views.Clear();
				chart.Views.Add(view);
				if (chart.Is3DChart && !was3DChart)
					SetupView3D(chart.View3D);
				ChartAxisHelper.CheckArgumentAxis(chart);
			}
			finally {
				chart.EndUpdate();
			}
			chart.UpdateDataReferences();
		}
		void ChangeChartType(Chart chart, CellRange seriesRange, CellRange axisLabelsRange, CellRange seriesNamesRange) {
			chart.BeginUpdate();
			try {
				List<AxisBase> savedAxes = new List<AxisBase>(chart.PrimaryAxes.InnerList);
				chart.PrimaryAxes.Clear();
				chart.SecondaryAxes.Clear();
				CreateAxes(chart);
				IChartView view = CreateChartView(chart);
				view.Axes = chart.PrimaryAxes;
				RestoreAxes(chart, savedAxes, IsPercentStacked(view) || IsPercentStacked(chart));
				ChartViewSeriesDirection direction = this.rangeModel.Direction;
				chart.SeriesDirection = direction;
				CreateViewSeries(view, seriesRange, axisLabelsRange, seriesNamesRange, direction);
				SetupViewDataLabels(view as ChartViewWithDataLabels);
				bool was3DChart = chart.Is3DChart;
				chart.Views.Clear();
				chart.Views.Add(view);
				if (chart.Is3DChart && !was3DChart)
					SetupView3D(chart.View3D);
			}
			finally {
				chart.EndUpdate();
			}
		}
		bool IsPercentStacked(IChartView view) {
			BarChartViewBase barView = view as BarChartViewBase;
			if (barView != null)
				return barView.Grouping == BarChartGrouping.PercentStacked;
			ChartViewWithGroupingAndDropLines groupView = view as ChartViewWithGroupingAndDropLines;
			if (groupView != null)
				return groupView.Grouping == ChartGrouping.PercentStacked;
			return false;
		}
		bool IsPercentStacked(Chart chart) {
			if (chart.Views.Count > 0)
				return IsPercentStacked(chart.Views[0]);
			return false;
		}
		void RestoreAxes(Chart chart, List<AxisBase> savedAxes, bool percentStacked) {
			for (int i = 0; i < chart.PrimaryAxes.Count && i < savedAxes.Count; i++) {
				AxisBase axis = chart.PrimaryAxes[i];
				AxisBase savedAxis = savedAxes[i];
				axis.BeginUpdate();
				try {
					axis.CopyLayout(savedAxis, percentStacked);
				}
				finally {
					axis.EndUpdate();
				}
			}
		}
		public static CellRange GetRange(FormulaReferencedRanges ranges) {
			if (ranges == null)
				return null;
			if (ranges.Count == 1)
				return ranges[0].CellRange as CellRange;
			else
				return null;
		}
		public void CreateViewSeries(IChartView view, Chart chart) {
			view.Series.Clear();
			List<ISeries> seriesList = chart.GetSeriesList();
			int count = seriesList.Count;
			for (int i = 0; i < count; i++) {
				ISeries originalSeries = seriesList[i];
				if (i == 0)
					ChartAxisHelper.CheckArgumentAxis(view, originalSeries.Arguments);
				SeriesBase series = CreateSeriesCore(view);
				series.Index = originalSeries.Index;
				series.Order = i;
				SetupSeries(series, 
					originalSeries.Arguments.CloneTo(chart.DocumentModel), 
					originalSeries.Values.CloneTo(chart.DocumentModel), 
					originalSeries.Text.CloneTo(chart));
				view.Series.Add(series);
			}
		}
		public void CreateViewSeries(IChartView view, CellRange seriesRange, CellRange axisLabelsRange, CellRange seriesNamesRange, ChartViewSeriesDirection direction) {
			view.Series.Clear();
			IDataReference arguments = CreateArgumentData(axisLabelsRange, direction);
			ChartAxisHelper.CheckArgumentAxis(view, arguments);
			if (direction == ChartViewSeriesDirection.Horizontal)
				CreateHorizontalSeries(seriesRange, arguments, view, seriesNamesRange);
			else
				CreateVerticalSeries(seriesRange, arguments, view, seriesNamesRange);
		}
		void SetupViewDataLabels(ChartViewWithDataLabels view) {
			if (view != null)
				view.DataLabels.Apply = true;
		}
		protected void ApplyPreset(Chart chart) {
			if (Preset != null && Preset.CanModifyChart(chart))
				Preset.ModifyChart(chart);
		}
		protected internal virtual void SetupChart(Chart chart) {
			chart.Culture = CultureInfo.CurrentCulture;
			chart.DispBlanksAs = DisplayBlanksAs.Gap;
			SetupView3D(chart.View3D);
		}
		protected virtual void SetupView3D(View3DOptions view3D) {
		}
		protected void CalculateChartPosition(DrawingObject drawingObject) {
			int chartWidth = DocumentModel.LayoutUnitConverter.TwipsToLayoutUnits(5 * 1440); 
			int chartHeight = DocumentModel.LayoutUnitConverter.TwipsToLayoutUnits(3 * 1440); 
			drawingObject.AnchorType = AnchorType.TwoCell;
			drawingObject.From = CalculateTopLeftAnchor(drawingObject, chartWidth, chartHeight);
			drawingObject.To = CalculateBottomRightAnchor(drawingObject, chartWidth, chartHeight);
		}
		AnchorPoint CalculateTopLeftAnchor(DrawingObject drawingObject, int chartWidth, int chartHeight) {
			IList<Page> pages = InnerControl.DesignDocumentLayout.Pages;
			Page page = pages[pages.Count - 1];
			PageGrid columns = page.GridColumns;
			PageGrid rows = page.GridRows;
			int horizontalOffset = ((columns.ActualLast.Far - columns.ActualFirst.Near) - chartWidth) / 2;
			int verticalOffset = ((rows.ActualLast.Far - rows.ActualFirst.Near) - chartHeight) / 2;
			int leftColumnIndex = Math.Max(columns.ActualFirstIndex, columns.LookupItemIndexByPosition(columns.ActualFirst.Near + horizontalOffset));
			int leftColumnOffset = DocumentModel.ToDocumentLayoutUnitConverter.ToModelUnits(horizontalOffset - (columns[leftColumnIndex].Near - columns.ActualFirst.Near));
			int topRowIndex = Math.Max(rows.ActualFirstIndex, rows.LookupItemIndexByPosition(rows.ActualFirst.Near + verticalOffset));
			int topRowOffset = DocumentModel.ToDocumentLayoutUnitConverter.ToModelUnits(verticalOffset - (rows[topRowIndex].Near - rows.ActualFirst.Near));
			return new AnchorPoint(ActiveSheet.SheetId, columns[leftColumnIndex].ModelIndex, rows[topRowIndex].ModelIndex, leftColumnOffset, topRowOffset);
		}
		AnchorPoint CalculateBottomRightAnchor(DrawingObject drawingObject, int chartWidth, int chartHeight) {
			IList<Page> pages = InnerControl.DesignDocumentLayout.Pages;
			Page page = pages[pages.Count - 1];
			PageGrid columns = page.GridColumns;
			PageGrid rows = page.GridRows;
			int horizontalOffset = ((columns.ActualLast.Far - columns.ActualFirst.Near) - chartWidth) / 2;
			int verticalOffset = ((rows.ActualLast.Far - rows.ActualFirst.Near) - chartHeight) / 2;
			int rightColumnIndex = columns.LookupItemIndexByPosition(columns.ActualFirst.Near + horizontalOffset + chartWidth);
			if (rightColumnIndex < 0)
				rightColumnIndex = columns.ActualLastIndex;
			int rightColumnOffset = DocumentModel.ToDocumentLayoutUnitConverter.ToModelUnits(columns.ActualFirst.Near + horizontalOffset + chartWidth - columns[rightColumnIndex].Near);
			int bottomRowIndex = rows.LookupItemIndexByPosition(rows.ActualFirst.Near + verticalOffset + chartHeight);
			if (bottomRowIndex < 0)
				bottomRowIndex = rows.ActualLastIndex;
			int bottomRowOffset = DocumentModel.ToDocumentLayoutUnitConverter.ToModelUnits(rows.ActualFirst.Near + verticalOffset + chartHeight - rows[bottomRowIndex].Near);
			return new AnchorPoint(ActiveSheet.SheetId, columns[rightColumnIndex].ModelIndex, rows[bottomRowIndex].ModelIndex, rightColumnOffset, bottomRowOffset);
		}
		protected virtual void CreateHorizontalSeries(CellRange seriesRange, IDataReference arguments, IChartView view, CellRange seriesNamesRange) {
			int firstRow = seriesRange.TopLeft.Row;
			int lastRow = seriesRange.BottomRight.Row;
			int index = 0;
			for (int i = firstRow; i <= lastRow; i++, index++) {
				CellRange range = CreateSeriesRange(seriesRange.Worksheet, seriesRange.TopLeft.Column, i, seriesRange.BottomRight.Column, i);
				CellRange nameRange = CreateHorizontalSeriesNameRange(seriesNamesRange, i);
				SeriesBase series = CreateSeries(range, nameRange, arguments, view);
				AddSeries(view, series, index);
			}
		}
		protected virtual void CreateVerticalSeries(CellRange seriesRange, IDataReference arguments, IChartView view, CellRange seriesNamesRange) {
			int firstColumn = seriesRange.TopLeft.Column;
			int lastColumn = seriesRange.BottomRight.Column;
			int index = 0;
			for (int i = firstColumn; i <= lastColumn; i++, index++) {
				CellRange range = CreateSeriesRange(seriesRange.Worksheet, i, seriesRange.TopLeft.Row, i, seriesRange.BottomRight.Row);
				CellRange nameRange = CreateVerticalSeriesNameRange(seriesNamesRange, i);
				SeriesBase series = CreateSeries(range, nameRange, arguments, view);
				AddSeries(view, series, index);
			}
		}
		protected CellRange CreateSeriesRange(ICellTable sheet, int left, int top, int right, int bottom) {
			CellPosition topLeft = new CellPosition(left, top, PositionType.Absolute, PositionType.Absolute);
			CellPosition bottomRight = new CellPosition(right, bottom, PositionType.Absolute, PositionType.Absolute);
			return new CellRange(sheet, topLeft, bottomRight);
		}
		protected CellRange CreateHorizontalSeriesNameRange(CellRange seriesNamesRange, int serieIndex) {
			if (seriesNamesRange == null)
				return null;
			return new CellRange(seriesNamesRange.Worksheet, new CellPosition(seriesNamesRange.TopLeft.Column, serieIndex, PositionType.Absolute, PositionType.Absolute), new CellPosition(seriesNamesRange.BottomRight.Column, serieIndex, PositionType.Absolute, PositionType.Absolute));
		}
		protected CellRange CreateVerticalSeriesNameRange(CellRange seriesNamesRange, int serieIndex) {
			if (seriesNamesRange == null)
				return null;
			return new CellRange(seriesNamesRange.Worksheet, new CellPosition(serieIndex, seriesNamesRange.TopLeft.Row, PositionType.Absolute, PositionType.Absolute), new CellPosition(serieIndex, seriesNamesRange.BottomRight.Row, PositionType.Absolute, PositionType.Absolute));
		}
		protected void AddSeries(IChartView view, SeriesBase series, int index) {
			series.Index = index;
			series.Order = index;
			view.Series.Add(series);
		}
		protected SeriesBase CreateSeries(CellRange valuesRange, CellRange nameRange, IDataReference arguments, IChartView view) {
			SeriesBase series = CreateSeriesCore(view);
			SetupSeries(series, arguments, valuesRange, nameRange);
			return series;
		}
		protected virtual IDataReference CreateArgumentData(CellRange argumentRange, ChartViewSeriesDirection seriesDirection) {
			if (argumentRange == null)
				return DataReference.Empty;
			bool isNumber = true;
			foreach (CellBase cell in argumentRange.GetExistingCellsEnumerable()) {
				if (cell.Value.IsText) {
					isNumber = false;
					break;
				}
			}
			ChartDataReference dataReference = new ChartDataReference(DocumentModel, seriesDirection, isNumber);
			dataReference.SetRange(argumentRange);
			return dataReference;
		}
		protected void CreateTwoPrimaryAxes(Chart chart) {
			CreateTwoPrimaryAxes(chart, false, true);
		}
		protected void CreateTwoPrimaryAxes(Chart chart, bool firstValueAxis, bool secondValueAxis) {
			CreateTwoPrimaryAxes(chart, firstValueAxis, secondValueAxis, AxisPosition.Bottom, AxisPosition.Left);
		}
		protected void CreateTwoPrimaryAxes(Chart chart, bool firstValueAxis, bool secondValueAxis, AxisPosition firstAxisPosition, AxisPosition secondAxisPosition) {
			CreateTwoPrimaryAxes(chart, firstValueAxis, secondValueAxis, firstAxisPosition, secondAxisPosition, ValueAxisCrossBetween);
		}
		public static void CreateTwoPrimaryAxes(Chart chart, bool firstValueAxis, bool secondValueAxis, AxisPosition firstAxisPosition, AxisPosition secondAxisPosition, AxisCrossBetween crossBetween) {
			AxisBase categoryAxis = CreateAxis(chart, firstValueAxis);
			categoryAxis.Position = firstAxisPosition;
			categoryAxis.TickLabelPos = TickLabelPosition.NextTo;
			categoryAxis.MajorTickMark = TickMark.Outside;
			categoryAxis.MinorTickMark = TickMark.None;
			AxisBase valueAxis = CreateAxis(chart, secondValueAxis);
			valueAxis.Position = secondAxisPosition;
			valueAxis.TickLabelPos = TickLabelPosition.NextTo;
			valueAxis.MajorTickMark = TickMark.Outside;
			valueAxis.MinorTickMark = TickMark.None;
			valueAxis.ShowMajorGridlines = true;
			valueAxis.CrossesAxis = categoryAxis;
			categoryAxis.CrossesAxis = valueAxis;
			ValueAxis valAxis = valueAxis as ValueAxis;
			if (valAxis != null)
				valAxis.CrossBetween = crossBetween;
			chart.PrimaryAxes.Add(categoryAxis);
			chart.PrimaryAxes.Add(valueAxis);
		}
		static AxisBase CreateAxis(Chart chart, bool valueAxis) {
			if (valueAxis)
				return new ValueAxis(chart);
			else
				return new CategoryAxis(chart);
		}
		protected void CreateThreePrimaryAxes(Chart chart) {
			CreateThreePrimaryAxes(chart, AxisPosition.Bottom, AxisPosition.Left);
		}
		protected void CreateThreePrimaryAxes(Chart chart, AxisPosition firstAxisPosition, AxisPosition secondAxisPosition) {
			CreateThreePrimaryAxes(chart, firstAxisPosition, secondAxisPosition, ValueAxisCrossBetween);
		}
		public static void CreateThreePrimaryAxes(Chart chart, AxisPosition firstAxisPosition, AxisPosition secondAxisPosition, AxisCrossBetween crossBetween) {
			AxisBase categoryAxis = CreateAxis(chart, false);
			categoryAxis.Position = firstAxisPosition;
			categoryAxis.TickLabelPos = TickLabelPosition.NextTo;
			categoryAxis.MajorTickMark = TickMark.Outside;
			categoryAxis.MinorTickMark = TickMark.None;
			ValueAxis valueAxis = new ValueAxis(chart);
			valueAxis.Position = secondAxisPosition;
			valueAxis.TickLabelPos = TickLabelPosition.NextTo;
			valueAxis.MajorTickMark = TickMark.Outside;
			valueAxis.MinorTickMark = TickMark.None;
			valueAxis.ShowMajorGridlines = true;
			AxisBase seriesAxis = new SeriesAxis(chart);
			seriesAxis.Position = AxisPosition.Bottom;
			seriesAxis.TickLabelPos = TickLabelPosition.NextTo;
			seriesAxis.MajorTickMark = TickMark.Outside;
			seriesAxis.MinorTickMark = TickMark.None;
			categoryAxis.CrossesAxis = valueAxis;
			valueAxis.CrossesAxis = categoryAxis;
			seriesAxis.CrossesAxis = valueAxis;
			valueAxis.CrossBetween = crossBetween;
			chart.PrimaryAxes.Add(categoryAxis);
			chart.PrimaryAxes.Add(valueAxis);
			chart.PrimaryAxes.Add(seriesAxis);
		}
		protected virtual void CreateAxes(Chart chart) {
			CreateTwoPrimaryAxes(chart);
		}
		protected ValueAxis FindValueAxis(Chart chart) {
			foreach (AxisBase axis in chart.PrimaryAxes) {
				ValueAxis valueAxis = axis as ValueAxis;
				if (valueAxis != null)
					return valueAxis;
			}
			return null;
		}
		public void ApplyPercentFormatOnValueAxis(Chart chart, bool apply) {
			if (apply) {
				ValueAxis axis = FindValueAxis(chart);
				if (axis != null) {
					axis.NumberFormat.SourceLinked = false;
					axis.NumberFormat.NumberFormatCode = "0%";
				}
			}
		}
		protected virtual void SetupSeries(SeriesBase series, IDataReference arguments, CellRange valuesRange, CellRange nameRange) {
			ChartDataReference valueDataReference = new ChartDataReference(DocumentModel, series.View.SeriesDirection, true);
			valueDataReference.SetRange(valuesRange);
			series.Arguments = arguments;
			series.Values = valueDataReference;
			if (nameRange != null) {
				ChartTextRef text = new ChartTextRef(series.Parent);
				text.SetRange(nameRange);
				series.Text = text;
			}
		}
		protected virtual void SetupSeries(SeriesBase series, IDataReference arguments, IDataReference values, IChartText seriesText) {
			series.Arguments = arguments;
			series.Values = values;
			series.Text = seriesText;
		}
		#region Chart ranges calculation
		#endregion
		public bool IsCompatibleChart(Chart chart) {
			if (chart.Views.Count <= 0)
				return false;
			return IsCompatibleView(chart.Views[0]);
		}
		public static InsertChartCommandBase CreateCompatibleCommandId(ISpreadsheetControl control, Chart chart) {
			foreach (SpreadsheetCommandId id in insertChartCommands) {
				InsertChartCommandBase command = control.CreateCommand(id) as InsertChartCommandBase;
				if (command != null && command.IsCompatibleChart(chart))
					return command;
			}
			return null;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, InnerControl.DocumentModel.DocumentCapabilities.Charts, !ActiveSheet.Selection.IsDrawingSelected && !InnerControl.IsAnyInplaceEditorActive);
			ApplyActiveSheetProtection(state, !Protection.ObjectsLocked);
		}
		protected internal abstract SeriesBase CreateSeriesCore(IChartView view);
		protected internal abstract IChartView CreateChartView(IChart parent);
		protected internal abstract bool IsCompatibleView(IChartView chartView);
	}
	#endregion
	#region InsertChartCommandGroupBase (abstract class)
	public abstract class InsertChartCommandGroupBase : SpreadsheetCommandGroup {
		protected InsertChartCommandGroupBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, InnerControl.DocumentModel.DocumentCapabilities.Charts, !ActiveSheet.Selection.IsDrawingSelected && !InnerControl.IsAnyInplaceEditorActive);
			ApplyActiveSheetProtection(state, !Protection.ObjectsLocked);
		}
	}
	#endregion
}
