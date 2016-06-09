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
using System.ComponentModel;
using System.Drawing;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Spreadsheet.Drawings;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.Spreadsheet.Charts {
	#region ChartStyle
	public enum ChartStyle {
		Gray = 1,
		GrayOutline = 9,
		GrayGradient = 17,
		GrayBevel = 25,
		GrayArea = 33,
		GrayDark = 41,
		Color = 2,
		ColorOutline = 10,
		ColorGradient = 18,
		ColorBevel = 26,
		ColorArea = 34,
		ColorDark = 42,
		Accent1 = 3,
		Accent1Outline = 11,
		Accent1Gradient = 19,
		Accent1Bevel = 27,
		Accent1Area = 35,
		Accent1Dark = 43,
		Accent2 = 4,
		Accent2Outline = 12,
		Accent2Gradient = 20,
		Accent2Bevel = 28,
		Accent2Area = 36,
		Accent2Dark = 44,
		Accent3 = 5,
		Accent3Outline = 13,
		Accent3Gradient = 21,
		Accent3Bevel = 29,
		Accent3Area = 37,
		Accent3Dark = 45,
		Accent4 = 6,
		Accent4Outline = 14,
		Accent4Gradient = 22,
		Accent4Bevel = 30,
		Accent4Area = 38,
		Accent4Dark = 46,
		Accent5 = 7,
		Accent5Outline = 15,
		Accent5Gradient = 23,
		Accent5Bevel = 31,
		Accent5Area = 39,
		Accent5Dark = 47,
		Accent6 = 8,
		Accent6Outline = 16,
		Accent6Gradient = 24,
		Accent6Bevel = 32,
		Accent6Area = 40,
		Accent6Dark = 48
	}
	#endregion
	public interface Chart : Shape, ShapeFormat {
		ChartStyle Style { get; set; }
		ChartTitle Title { get; }
		PlotArea PlotArea { get; }
		AxisCollection PrimaryAxes { get; }
		AxisCollection SecondaryAxes { get; }
		ChartViewCollection Views { get; }
		SeriesCollection Series { get; }
		Legend Legend { get; }
		DataTableOptions DataTable { get; }
		ChartOptions Options { get; }
		View3DOptions View3D { get; }
		ShapeTextFont Font { get; }
		void ChangeType(ChartType chartType); 
		void SelectData(Range range);
		void SelectData(Range range, ChartDataDirection direction);
		void SwitchRowColumn();
		Chart CopyTo(Worksheet worksheet);
		OfficeImage GetImage();
		OfficeImage GetImage(Size size);
	}
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Spreadsheet;
	using DevExpress.Spreadsheet.Charts;
	using DevExpress.XtraSpreadsheet.Localization;
	partial class NativeChart : NativeDrawingObject, Chart {
		#region Fields
		readonly Model.Chart modelChart;
		NativeChartTitle title;
		NativePlotArea plotArea;
		NativeAxisCollection primaryAxes;
		NativeAxisCollection secondaryAxes;
		NativeChartViewCollection views;
		NativeSeriesCollection series;
		NativeLegend legend;
		NativeDataTableOptions dataTable;
		NativeChartOptions options;
		NativeView3DOptions view3D;
		NativeShapeTextFont font;
		NativeShapeFormat shapeFormat;
		#endregion
		public NativeChart(Model.Chart modelChart, NativeWorksheet nativeWorksheet) 
			: base(modelChart.DrawingObject, nativeWorksheet) {
			this.modelChart = modelChart;
		}
		#region Properties
		protected internal Model.Chart ModelChart { get { return modelChart; } }
		#region Chart Members
		#region Style
		public ChartStyle Style {
			get {
				CheckValid();
				return (ChartStyle)modelChart.Style;
			}
			set {
				CheckValid();
				modelChart.Style = (int)value;
			}
		}
		#endregion
		#region Title
		public ChartTitle Title {
			get {
				CheckValid();
				if (title == null)
					title = new NativeChartTitle(modelChart.Title, NativeWorkbook);
				return title;
			}
		}
		#endregion
		#region PlotArea
		public PlotArea PlotArea {
			get {
				CheckValid();
				if (plotArea == null)
					plotArea = new NativePlotArea(modelChart.PlotArea, NativeWorkbook);
				return plotArea;
			}
		}
		#endregion
		#region PrimaryAxes
		public AxisCollection PrimaryAxes {
			get {
				CheckValid();
				if (primaryAxes == null)
					primaryAxes = new NativeAxisCollection(modelChart.PrimaryAxes, NativeWorkbook);
				return primaryAxes;
			}
		}
		#endregion
		#region SecondaryAxes
		public AxisCollection SecondaryAxes {
			get {
				CheckValid();
				if (secondaryAxes == null)
					secondaryAxes = new NativeAxisCollection(modelChart.SecondaryAxes, NativeWorkbook);
				return secondaryAxes;
			}
		}
		#endregion
		#region Views
		public ChartViewCollection Views {
			get {
				CheckValid();
				if (views == null)
					views = new NativeChartViewCollection(modelChart.Views, this);
				if (series == null)
					series = new NativeSeriesCollection(this);
				return views;
			}
		}
		#endregion
		#region Series
		public SeriesCollection Series {
			get {
				CheckValid();
				if (series == null)
					series = new NativeSeriesCollection(this);
				if (views == null)
					views = new NativeChartViewCollection(modelChart.Views, this);
				return series;
			}
		}
		#endregion
		#region Legend
		public Legend Legend {
			get {
				CheckValid();
				if (legend == null)
					legend = new NativeLegend(modelChart.Legend, NativeWorkbook);
				return legend;
			}
		}
		#endregion
		#region DataTable
		public DataTableOptions DataTable {
			get {
				CheckValid();
				if (dataTable == null)
					dataTable = new NativeDataTableOptions(modelChart.DataTable, NativeWorkbook);
				return dataTable;
			}
		}
		#endregion
		#region Options
		public ChartOptions Options {
			get {
				CheckValid();
				if (options == null)
					options = new NativeChartOptions(modelChart);
				return options;
			}
		}
		#endregion
		#region View3D
		public View3DOptions View3D {
			get {
				CheckValid();
				if (view3D == null)
					view3D = new NativeView3DOptions(modelChart, NativeWorkbook);
				return view3D;
			}
		}
		#endregion
		#region Font
		public ShapeTextFont Font {
			get {
				CheckValid();
				if (font == null)
					font = new NativeShapeTextFont(modelChart.TextProperties);
				return font;
			}
		}
		#endregion
		public void ChangeType(ChartType chartType) {
			CheckValid();
			Model.ChartViewCollection modelViews = modelChart.Views;
			if (chartType == (ChartType)modelViews[0].ChartType && modelViews.Count == 1)
				return;
			Model.ChangeChartTypeCommand command = new Model.ChangeChartTypeCommand(ApiErrorHandler.Instance, modelChart, (Model.ChartType)chartType);
			command.Execute();
		}
		#region SelectData
		public void SelectData(Range range) {
			CheckValid();
			Model.ModifyChartRangesCommand command;
			if (range == null)
				command = new Model.ModifyChartRangesCommand(modelChart.Worksheet, ApiErrorHandler.Instance);
			else {
				NativeWorksheet rangeWorksheet = (NativeWorksheet)range.Worksheet;
				command = new Model.ModifyChartRangesCommand(rangeWorksheet.ModelWorksheet, ApiErrorHandler.Instance);
				command.DataRange = rangeWorksheet.GetModelSingleRange(range);
			}
			command.Chart = modelChart;
			command.Execute();
		}
		public void SelectData(Range range, ChartDataDirection direction) {
			CheckValid();
			Model.ModifyChartRangeFromDirectionCommand command;
			if (range == null)
				command = new Model.ModifyChartRangeFromDirectionCommand(modelChart.Worksheet, Model.ChartViewSeriesDirection.Horizontal, ApiErrorHandler.Instance);
			else {
				Model.ChartViewSeriesDirection modelDirection = direction == ChartDataDirection.Column ? Model.ChartViewSeriesDirection.Vertical : Model.ChartViewSeriesDirection.Horizontal;
				NativeWorksheet rangeWorksheet = (NativeWorksheet)range.Worksheet;
				command = new Model.ModifyChartRangeFromDirectionCommand(rangeWorksheet.ModelWorksheet, modelDirection, ApiErrorHandler.Instance);
				command.DataRange = rangeWorksheet.GetModelSingleRange(range);
			}
			command.Chart = modelChart;
			command.Execute();
		}
		#endregion
		#region SwitchRowColumn
		public void SwitchRowColumn() {
			CheckValid();
			Model.SwitchChartRowColumnCommand command = new Model.SwitchChartRowColumnCommand(modelChart, ApiErrorHandler.Instance);
			command.Execute();
		}
		#endregion
		#region CopyTo
		public Chart CopyTo(Worksheet worksheet) {
			CheckValid();
			NativeWorksheet nativeWorksheet = worksheet as NativeWorksheet;
			if (nativeWorksheet == null)
				return null;
			Model.CopyChartCommand command = new Model.CopyChartCommand(modelChart, nativeWorksheet.ModelWorksheet, ApiErrorHandler.Instance);
			if (!command.Execute())
				return null;
			return worksheet.Charts[worksheet.Charts.Count - 1];
		}
		#endregion
		#region GetImage
		public OfficeImage GetImage() {
			CheckValid();
			Model.DocumentModel documentModel = ModelChart.DocumentModel;
			float width = documentModel.UnitConverter.ModelUnitsToPixelsF(ModelChart.Width, Model.DocumentModel.DpiX);
			float height = documentModel.UnitConverter.ModelUnitsToPixelsF(ModelChart.Height, Model.DocumentModel.DpiY);
			return ModelChart.GetChartImage(Size.Round(new SizeF(width, height)));
		}
		public OfficeImage GetImage(Size size) {
			CheckValid();
			return ModelChart.GetChartImage(size);
		}
		#endregion
		#region ShapeFormat Members
		public ShapeFill Fill {
			get {
				CheckValid();
				CheckShapeFormat();
				return shapeFormat.Fill;
			}
		}
		public ShapeOutline Outline {
			get {
				CheckValid();
				CheckShapeFormat();
				return shapeFormat.Outline;
			}
		}
		public void ResetToMatchStyle() {
			CheckValid();
			modelChart.DocumentModel.BeginUpdate();
			try {
				modelChart.ResetToStyle();
			}
			finally {
				modelChart.DocumentModel.EndUpdate();
			}
		}
		void CheckShapeFormat() {
			if (shapeFormat == null)
				shapeFormat = new NativeShapeFormat(modelChart.ShapeProperties, NativeWorkbook);
		}
		#endregion
		#endregion
		#region NativeDrawingObject Members
		public override int ZOrderPosition {
			get {
				CheckValid();
				return modelChart.ZOrder;
			}
			set {
				CheckValid();
				modelChart.ZOrder = value;
			}
		}
		public override void Delete() {
			CheckValid();
			int index = Worksheet.ModelWorksheet.DrawingObjects.IndexOf(this.modelChart);
			if (index >= 0)
				Worksheet.ModelWorksheet.DrawingObjects.RemoveAt(index);
		}
		public override void IncrementRotation(int degrees) {
			CheckValid();
		}
		public override int Rotation {
			get {
				CheckValid();
				return NativeWorkbook.UnitConverter.Converter.ModelUnitsToDegree(modelChart.ShapeProperties.Transform2D.Rotation);
			}
			set {
				CheckValid();
			}
		}
		public override void RemoveHyperlink() {
			CheckValid();
		}
		public override ShapeHyperlink InsertHyperlink(string uri, bool isExternal) {
			CheckValid();
			return null;
		}
		#endregion
		#endregion
		#region SetIsValid
		protected override void SetIsValid(bool value) {
			base.SetIsValid(value);
			if (title != null)
				title.IsValid = value;
			if (plotArea != null)
				plotArea.IsValid = value;
			if (primaryAxes != null)
				primaryAxes.IsValid = value;
			if (secondaryAxes != null)
				secondaryAxes.IsValid = value;
			if (views != null)
				views.IsValid = value;
			if (series != null)
				series.IsValid = value;
			if (legend != null)
				legend.IsValid = value;
			if (dataTable != null)
				dataTable.IsValid = value;
			if (options != null)
				options.IsValid = value;
			if (view3D != null)
				view3D.IsValid = value;
			if (font != null)
				font.IsValid = value;
			if (shapeFormat != null)
				shapeFormat.IsValid = value;
		}
		#endregion
		public override bool Equals(object obj) {
			if (!IsValid)
				return false;
			NativeChart other = obj as NativeChart;
			if (other == null || !other.IsValid)
				return false;
			return object.ReferenceEquals(modelChart, other.modelChart);
		}
		public override int GetHashCode() {
			if (!IsValid)
				return -1;
			return modelChart.GetHashCode();
		}
	}
}
