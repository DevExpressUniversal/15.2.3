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
using DevExpress.Office;
using DevExpress.Spreadsheet.Drawings;
namespace DevExpress.Spreadsheet.Charts {
	public enum BarShape {
		Auto = DevExpress.XtraSpreadsheet.Model.BarShape.Auto,
		Box = DevExpress.XtraSpreadsheet.Model.BarShape.Box,
		Cone = DevExpress.XtraSpreadsheet.Model.BarShape.Cone,
		ConeToMax = DevExpress.XtraSpreadsheet.Model.BarShape.ConeToMax,
		Cylinder = DevExpress.XtraSpreadsheet.Model.BarShape.Cylinder,
		Pyramid = DevExpress.XtraSpreadsheet.Model.BarShape.Pyramid,
		PyramidToMax = DevExpress.XtraSpreadsheet.Model.BarShape.PyramidToMax
	}
	public interface Series : ShapeFormat {
		ChartType SeriesType { get; }
		int Order { get; }
		ChartText SeriesName { get; }
		ChartData Arguments { get; set; }
		ChartData Values { get; set; }
		ChartData BubbleSize { get; set; }
		AxisGroup AxisGroup { get; set; }
		ChartView View { get; }
		int Explosion { get; set; }
		bool InvertIfNegative { get; set; }
		bool Smooth { get; set; }
		BarShape Shape { get; set; }
		Marker Marker { get; }
		DataPointCollection CustomDataPoints { get; }
		DataLabelCollection CustomDataLabels { get; }
		bool UseCustomDataLabels { get; set; }
		ErrorBarsCollection ErrorBars { get; }
		TrendlineCollection Trendlines { get; }
		void BringToFront();
		void BringForward();
		void SendToBack();
		void SendBackward();
		void ChangeType(ChartType chartType);
		void Delete();
	}
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Spreadsheet;
	using DevExpress.Spreadsheet.Charts;
	using DevExpress.Utils;
	using DevExpress.Office.Utils;
	#region NativeSeries
	partial class NativeSeries : NativeObjectBase, Series {
		#region Fields
		readonly NativeChart nativeChart;
		Model.ISeries modelSeries;
		NativeChartText nativeChartText;
		NativeChartView nativeView;
		NativeShapeFormat nativeShapeFormat;
		NativeDataPointCollection nativeDataPointCollection;
		NativeDataLabelCollection nativeDataLabelCollection;
		NativeErrorBarsCollection nativeErrorBarsCollection;
		NativeTrendlineCollection nativeTrendlineCollection;
		NativeMarker nativeMarker;
		#endregion
		public NativeSeries(Model.ISeries modelSeries, NativeChart nativeChart) {
			this.nativeChart = nativeChart;
			SetModelSeriesCore(modelSeries);
		}
		#region Properties
		internal Model.ISeries ModelSeries { get { return modelSeries; } }
		NativeWorkbook NativeWorkbook { get { return nativeChart.NativeWorkbook; } }
		Model.SeriesWithDataLabelsAndPoints ModelSeriesWithDataLabelsAndPoints { get { return modelSeries as Model.SeriesWithDataLabelsAndPoints; } }
		Model.SeriesWithErrorBarsAndTrendlines ModelSeriesWithErrorBarsAndTrendlines { get { return modelSeries as Model.SeriesWithErrorBarsAndTrendlines; } }
		Model.SeriesWithMarkerAndSmooth ModelSeriesWithMarkerAndSmooth { get { return modelSeries as Model.SeriesWithMarkerAndSmooth; } }
		Model.ISeriesWithMarker ModelSeriesWithMarker { get { return modelSeries as Model.ISeriesWithMarker; } }
		Model.ISupportsInvertIfNegative ModelSeriesWithInvertIfNegative { get { return modelSeries as Model.ISupportsInvertIfNegative; } }
		Model.BubbleSeries ModelBubbleSeries { get { return modelSeries as Model.BubbleSeries; } }
		Model.PieSeries ModelPieSeries { get { return modelSeries as Model.PieSeries; } }
		Model.BarSeries ModelBarSeries { get { return modelSeries as Model.BarSeries; } }
		#endregion
		void SetModelSeriesCore(Model.ISeries modelSeries) {
			this.modelSeries = modelSeries;
		}
		protected override void SetIsValid(bool value) {
			base.SetIsValid(value);
			SetIsValidNativeObjects(value);
		}
		void SetIsValidNativeView(bool value) {
			if (nativeView != null)
				nativeView.IsValid = value;
		}
		void SetIsValidNativeObjects(bool value) {
			if (nativeChartText != null)
				nativeChartText.IsValid = value;
			SetIsValidNativeView(value);
			if (nativeShapeFormat != null)
				nativeShapeFormat.IsValid = value;
			if (nativeDataPointCollection != null)
				nativeDataPointCollection.IsValid = value;
			if (nativeDataLabelCollection != null)
				nativeDataLabelCollection.IsValid = value;
			if (nativeErrorBarsCollection != null)
				nativeErrorBarsCollection.IsValid = value;
			if (nativeTrendlineCollection != null)
				nativeTrendlineCollection.IsValid = value;
			if (nativeMarker != null)
				nativeMarker.IsValid = value;
		}
		#region Series Members
		public ChartType SeriesType {
			get {
				CheckValid();
				return (ChartType)modelSeries.ChartType;
			}
		}
		public int Order {
			get {
				CheckValid();
				return modelSeries.Order;
			}
		}
		public ChartText SeriesName {
			get {
				CheckValid();
				if (nativeChartText == null)
					nativeChartText = new NativeChartText(modelSeries as Model.IChartTextOwner);
				return nativeChartText;
			}
		}
		public ChartData Arguments {
			get {
				CheckValid();
				return ChartDataConverter.ToChartData(modelSeries.Arguments, NativeWorkbook);
			}
			set {
				CheckValid();
				ExecuteSeriesRangesCommand(Model.ChartSeriesRefererenceAccessorType.Arguments, value, false);
			}
		}
		public ChartData Values {
			get {
				CheckValid();
				return ChartDataConverter.ToChartData(modelSeries.Values, NativeWorkbook);
			}
			set {
				CheckValid();
				ExecuteSeriesRangesCommand(Model.ChartSeriesRefererenceAccessorType.Values, value, true);
			}
		}
		public ChartData BubbleSize {
			get {
				CheckValid();
				Model.BubbleSeries modelSeries = ModelBubbleSeries;
				return modelSeries != null ? ChartDataConverter.ToChartData(modelSeries.BubbleSize, NativeWorkbook) : ChartData.Empty;
			}
			set {
				CheckValid();
				if (ModelBubbleSeries != null)
					ExecuteSeriesRangesCommand(Model.ChartSeriesRefererenceAccessorType.BubbleSize, value, true);
			}
		}
		public AxisGroup AxisGroup {
			get {
				CheckValid();
				Model.IChartView modelView = modelSeries.View;
				Model.AxisGroup modelViewAxes = modelView.Axes;
				Model.Chart modelChart = modelView.Parent as Model.Chart;
				if (modelChart != null && Object.ReferenceEquals(modelChart.SecondaryAxes, modelViewAxes))
					return AxisGroup.Secondary;
				return AxisGroup.Primary;
			}
			set {
				CheckValid();
				Model.IChartView modelView = modelSeries.View;
				Model.Chart modelChart = modelView.Parent as Model.Chart;
				if (modelChart == null || AxisGroup == value)
					return;
				Model.AxisGroup axes = value == AxisGroup.Primary ? modelChart.PrimaryAxes : modelChart.SecondaryAxes;
				Model.SwitchPrimarySecondaryAxesCommand command = new Model.SwitchPrimarySecondaryAxesCommand(modelSeries, axes, ApiErrorHandler.Instance);
				command.Execute();
				SetIsValidNativeView(false);
				nativeView = null;
			}
		}
		public ChartView View {
			get {
				CheckValid();
				if (nativeView == null)
					nativeView = new NativeChartView(modelSeries.View, nativeChart);
				return nativeView;
			}
		}
		public int Explosion {
			get {
				CheckValid();
				Model.PieSeries modelSeries = ModelPieSeries;
				return modelSeries != null ? modelSeries.Explosion : 0;
			}
			set {
				CheckValid();
				Model.PieSeries modelSeries = ModelPieSeries;
				if (modelSeries != null)
					modelSeries.Explosion = value;
			}
		}
		public bool InvertIfNegative {
			get {
				CheckValid();
				Model.ISupportsInvertIfNegative modelSeries = ModelSeriesWithInvertIfNegative;
				return modelSeries != null ? modelSeries.InvertIfNegative : false;
			}
			set {
				CheckValid();
				Model.ISupportsInvertIfNegative modelSeries = ModelSeriesWithInvertIfNegative;
				if (modelSeries != null)
					modelSeries.InvertIfNegative = value;
			}
		}
		public bool Smooth {
			get {
				CheckValid();
				Model.SeriesWithMarkerAndSmooth modelSeries = ModelSeriesWithMarkerAndSmooth;
				return modelSeries != null ? modelSeries.Smooth : false;
			}
			set {
				CheckValid();
				Model.SeriesWithMarkerAndSmooth modelSeries = ModelSeriesWithMarkerAndSmooth;
				if (modelSeries != null)
					modelSeries.Smooth = value;
			}
		}
		public BarShape Shape {
			get {
				CheckValid();
				Model.BarSeries modelSeries = ModelBarSeries;
				return modelSeries != null ? (BarShape)modelSeries.Shape : BarShape.Auto;
			}
			set {
				CheckValid();
				Model.BarSeries modelSeries = ModelBarSeries;
				if (modelSeries != null)
					modelSeries.Shape = (Model.BarShape)value;
			}
		}
		public Marker Marker {
			get {
				CheckValid();
				Model.ISeriesWithMarker modelSeries = ModelSeriesWithMarker;
				if (modelSeries == null)
					return null;
				if (nativeMarker == null)
					nativeMarker = new NativeMarker(modelSeries.Marker, NativeWorkbook);
				return nativeMarker;
			}
		}
		public DataPointCollection CustomDataPoints {
			get {
				CheckValid();
				Model.SeriesWithDataLabelsAndPoints modelSeries = ModelSeriesWithDataLabelsAndPoints;
				if (modelSeries == null)
					return null;
				if (nativeDataPointCollection == null)
					nativeDataPointCollection = new NativeDataPointCollection(modelSeries.DataPoints, NativeWorkbook);
				return nativeDataPointCollection;
			}
		}
		public DataLabelCollection CustomDataLabels {
			get {
				CheckValid();
				Model.SeriesWithDataLabelsAndPoints modelSeries = ModelSeriesWithDataLabelsAndPoints;
				if (modelSeries == null)
					return null;
				if (nativeDataLabelCollection == null)
					nativeDataLabelCollection = new NativeDataLabelCollection(modelSeries.DataLabels, NativeWorkbook);
				return nativeDataLabelCollection;
			}
		}
		public bool UseCustomDataLabels {
			get {
				CheckValid();
				Model.SeriesWithDataLabelsAndPoints modelSeries = ModelSeriesWithDataLabelsAndPoints;
				return modelSeries != null ? modelSeries.DataLabels.Apply : false;
			}
			set {
				CheckValid();
				Model.SeriesWithDataLabelsAndPoints modelSeries = ModelSeriesWithDataLabelsAndPoints;
				if (modelSeries != null)
					modelSeries.DataLabels.Apply = value;
			}
		}
		public TrendlineCollection Trendlines {
			get {
				CheckValid();
				Model.SeriesWithErrorBarsAndTrendlines modelSeries = ModelSeriesWithErrorBarsAndTrendlines;
				if (modelSeries == null)
					return null;
				if (nativeTrendlineCollection == null)
					nativeTrendlineCollection = new NativeTrendlineCollection(modelSeries.Trendlines, NativeWorkbook);
				return nativeTrendlineCollection;
			}
		}
		public ErrorBarsCollection ErrorBars {
			get {
				CheckValid();
				Model.SeriesWithErrorBarsAndTrendlines modelSeries = ModelSeriesWithErrorBarsAndTrendlines;
				if (modelSeries == null)
					return null;
				if (nativeErrorBarsCollection == null)
					nativeErrorBarsCollection = new NativeErrorBarsCollection(modelSeries.ErrorBars, NativeWorkbook);
				return nativeErrorBarsCollection;
			}
		}
		public void BringToFront() {
			CheckValid();
			ExecuteSeriesOrderCommand(Model.ChangeSeriesOrderType.BringToFront);
		}
		public void BringForward() {
			CheckValid();
			ExecuteSeriesOrderCommand(Model.ChangeSeriesOrderType.BringForward);
		}
		public void SendToBack() {
			CheckValid();
			ExecuteSeriesOrderCommand(Model.ChangeSeriesOrderType.SendToBack);
		}
		public void SendBackward() {
			CheckValid();
			ExecuteSeriesOrderCommand(Model.ChangeSeriesOrderType.SendBackward);
		}
		public void ChangeType(ChartType chartType) {
			CheckValid();
			NativeSeriesCollection nativeSeries = nativeChart.Series as NativeSeriesCollection;
			NativeViewSeriesCollection nativeViewSeries = View.Series as NativeViewSeriesCollection;
			if (nativeSeries == null || nativeViewSeries == null)
				return;
			Model.ChartType modelChartType = (Model.ChartType)chartType;
			bool check = Model.ChangeSeriesChartTypeCommand.CheckChangeCompatibleChartType(modelSeries, modelChartType);
			if (check) {
				nativeViewSeries.UnsubscribeEvents();
				nativeSeries.UnsubscribeEvents();
			}
			Model.ChangeSeriesChartTypeCommand command = new Model.ChangeSeriesChartTypeCommand(ApiErrorHandler.Instance, modelSeries, modelChartType);
			command.Execute();
			if (check) {
				nativeSeries.SubscribeEvents();
				nativeViewSeries.SubscribeEvents();
			}
			Model.ISeries newSeries = command.NewSeries;
			base.SetIsValid(true);
			if (Object.ReferenceEquals(modelSeries, newSeries))
				return;
			SetModelSeriesCore(newSeries);
			SetIsValidNativeObjects(false);
			nativeChartText = null;
			nativeView = null;
			nativeShapeFormat = null;
			nativeDataPointCollection = null;
			nativeDataLabelCollection = null;
			nativeErrorBarsCollection = null;
			nativeTrendlineCollection = null;
			nativeMarker = null;
		}
		public void Delete() {
			CheckValid();
			Model.RemoveSeriesCommand command = new Model.RemoveSeriesCommand(ApiErrorHandler.Instance, modelSeries as Model.SeriesBase);
			command.Execute();
		}
		public ShapeFill Fill {
			get {
				CreateNativeShapeFormat();
				return nativeShapeFormat.Fill;
			}
		}
		public ShapeOutline Outline {
			get {
				CreateNativeShapeFormat();
				return nativeShapeFormat.Outline;
			}
		}
		public void ResetToMatchStyle() {
			CheckValid();
			modelSeries.ResetToStyle();
		}
		#endregion
		void CreateNativeShapeFormat() {
			CheckValid();
			if (nativeShapeFormat == null) 
				nativeShapeFormat = new NativeShapeFormat(modelSeries.ShapeProperties, NativeWorkbook);
		}
		void ExecuteSeriesRangesCommand(Model.ChartSeriesRefererenceAccessorType accessorType, ChartData data, bool isNumber) {
			Model.IDataReference reference = ChartDataConverter.ToDataReference(data, NativeWorkbook.ModelWorkbook, modelSeries.View.SeriesDirection, isNumber);
			Model.ModifySeriesRangesCommand command = new Model.ModifySeriesRangesCommand(ApiErrorHandler.Instance, modelSeries, accessorType, reference);
			command.Execute();
		}
		void ExecuteSeriesOrderCommand(Model.ChangeSeriesOrderType orderType) {
			Model.ChangeSeriesOrderCommand command = new Model.ChangeSeriesOrderCommand(ApiErrorHandler.Instance, modelSeries, orderType);
			command.Execute();
		}
	}
	#endregion
}
