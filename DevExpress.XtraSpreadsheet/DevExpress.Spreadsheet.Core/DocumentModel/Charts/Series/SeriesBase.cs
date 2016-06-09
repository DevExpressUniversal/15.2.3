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
using System.Text;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraSpreadsheet.Model.History;
using ChartsModel = DevExpress.Charts.Model;
using System.Collections.Generic;
using DevExpress.Office.Drawing;
namespace DevExpress.XtraSpreadsheet.Model {
	#region SeriesBase
	public abstract class SeriesBase : ISeries, IChartTextOwner {
		#region Fields
		readonly IChartView view;
		IDataReference arguments;
		IDataReference values;
		int index;
		int order;
		IChartText text;
		ShapeProperties shapeProperties;
		#endregion
		protected SeriesBase(IChartView view) {
			Guard.ArgumentNotNull(view, "view");
			if(!IsCompatible(view))
				throw new ArgumentException(string.Format("Series and view are incompatible. Series type: {0}. View type {1}", SeriesType, view.ViewType));
			this.view = view;
			this.arguments = DataReference.Empty;
			this.values = DataReference.Empty;
			this.text = ChartText.Empty;
			this.shapeProperties = new ShapeProperties(DocumentModel) { Parent = view.Parent };
		}
		#region Properties
		public IChart Parent { get { return view.Parent; } }
		protected internal DocumentModel DocumentModel { get { return view.Parent.DocumentModel; } }
		#region Index
		public int Index {
			get { return index; }
			set {
				Guard.ArgumentNonNegative(value, "Index");
				if(index == value)
					return;
				SetIndex(value);
			}
		}
		void SetIndex(int value) {
			SeriesIndexPropertyChangedHistoryItem historyItem = new SeriesIndexPropertyChangedHistoryItem(DocumentModel, this, index, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetIndexCore(int value) {
			this.index = value;
			Parent.Invalidate();
		}
		#endregion
		#region Order
		public int Order {
			get { return order; }
			set {
				Guard.ArgumentNonNegative(value, "Order");
				if(order == value)
					return;
				SetOrder(value);
			}
		}
		void SetOrder(int value) {
			SeriesOrderPropertyChangedHistoryItem historyItem = new SeriesOrderPropertyChangedHistoryItem(DocumentModel, this, order, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetOrderCore(int value) {
			this.order = value;
			Parent.Invalidate();
		}
		#endregion
		#region Text
		public IChartText Text {
			get { return text; }
			set {
				if(value == null)
					value = ChartText.Empty;
				if(text.Equals(value))
					return;
				SetText(value);
			}
		}
		void SetText(IChartText value) {
			ChartTextPropertyChangedHistoryItem historyItem = new ChartTextPropertyChangedHistoryItem(DocumentModel, this, text, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		public void SetTextCore(IChartText value) {
			text = value;
			Parent.Invalidate();
		}
		#endregion
		public ShapeProperties ShapeProperties { get { return shapeProperties; } }
		protected virtual bool IsLinesOnly { get { return false; } }
		#endregion
		#region ISeries Members
		public IChartView View { get { return view; } }
		#region Arguments
		public IDataReference Arguments {
			get { return arguments; }
			set {
				if(value == null)
					value = DataReference.Empty;
				if(arguments.Equals(value))
					return;
				SetArguments(value);
			}
		}
		void SetArguments(IDataReference value) {
			SeriesArgumentsPropertyChangedHistoryItem historyItem = new SeriesArgumentsPropertyChangedHistoryItem(DocumentModel, this, arguments, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetArgumentsCore(IDataReference value) {
			this.arguments = value;
			Parent.Invalidate();
		}
		#endregion
		#region Values
		public IDataReference Values {
			get { return values; }
			set {
				if(value == null)
					value = DataReference.Empty;
				if(!value.Equals(DataReference.Empty) && !value.IsNumber)
					throw new ArgumentException("String reference is not allowed for series values!");
				if(values.Equals(value))
					return;
				SetValues(value);
			}
		}
		void SetValues(IDataReference value) {
			SeriesValuesPropertyChangedHistoryItem historyItem = new SeriesValuesPropertyChangedHistoryItem(DocumentModel, this, values, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetValuesCore(IDataReference value) {
			this.values = value;
			Parent.Invalidate();
		}
		#endregion
		public abstract ChartType ChartType { get; }
		public abstract ChartSeriesType SeriesType { get; }
		public abstract ISeries CloneTo(IChartView view);
		public virtual string GetSeriesText() {
			if(Text.TextType == ChartTextType.None || Text.TextType == ChartTextType.Auto)
				return string.Format("Series {0}", Index + 1);
			return Text.PlainText;
		}
		public abstract bool IsCompatible(IChartView view);
		public abstract void Visit(ISeriesVisitor visitor);
		public virtual void ResetToStyle() {
			ShapeProperties.ResetToStyle(IsLinesOnly && ShapeProperties.Outline.Fill.FillType == DrawingFillType.None);
		}
		public virtual void OnDataChanged() {
			if (arguments != null)
				arguments.OnContentVersionChanged();
			if (values != null)
				values.OnContentVersionChanged();
		}
		public virtual IEnumerable<IDataReference> GetDataReferences() {
			yield return arguments;
			yield return values;
		}
		#endregion
		#region ISupportsCopyFrom<ISeries> Members
		public virtual void CopyFrom(ISeries value) {
			SeriesBase series = value as SeriesBase;
			if (series != null)
				CopyFromCore(series);
		}
		void CopyFromCore(SeriesBase value) {
			Index = value.Index;
			Order = value.Order;
			Arguments = value.Arguments.CloneTo(DocumentModel);
			Values = value.Values.CloneTo(DocumentModel);
			Text = value.Text.CloneTo(Parent);
			shapeProperties.CopyFrom(value.shapeProperties);
		}
		public bool IsContained { get { return View.Series.Contains(this); } }
		#endregion
		#region Notifications
		public virtual void OnRangeInserting(InsertRangeNotificationContext context) {
			text.OnRangeInserting(context);
		}
		public virtual void OnRangeRemoving(RemoveRangeNotificationContext context) {
			text.OnRangeRemoving(context);
		}
		#endregion
	}
	#endregion
	#region SeriesWithDataLabelsAndPoints
	public abstract class SeriesWithDataLabelsAndPoints : SeriesBase, ChartsModel.IDataLabelFormatter {
		#region Fields
		DataLabels dataLabels;
		DataPointCollection dataPoints;
		#endregion
		protected SeriesWithDataLabelsAndPoints(IChartView view)
			: base(view) {
			this.dataLabels = new DataLabels(Parent);
			this.dataPoints = new DataPointCollection(Parent);
		}
		#region Properties
		public DataLabels DataLabels { get { return dataLabels; } }
		public DataPointCollection DataPoints { get { return dataPoints; } }
		#endregion
		#region IDataLabelFormatter Members
		public string GetDataLabelText(ChartsModel.LabelPointData pointData) {
			ChartDataRow dataRow = pointData.Context as ChartDataRow;
			if (dataRow == null)
				return string.Empty;
			int pointIndex = dataRow.Index;
			DataLabels actualDataLabels = null;
			ChartViewWithDataLabels parentView = View as ChartViewWithDataLabels;
			if (parentView != null)
				actualDataLabels = parentView.DataLabels;
			if (DataLabels.Apply)
				actualDataLabels = DataLabels;
			if (actualDataLabels != null) {
				DataLabel dataLabel = actualDataLabels.Labels.FindByItemIndex(pointIndex);
				if (dataLabel != null) {
					if (dataLabel.Text.TextType != ChartTextType.None && dataLabel.Text.TextType != ChartTextType.Auto)
						return dataLabel.Text.PlainText;
					return GetDataLabelText(pointIndex, pointData.NormalizedValue, dataLabel, actualDataLabels);
				}
				else
					return GetDataLabelText(pointIndex, pointData.NormalizedValue, actualDataLabels);
			}
			return string.Empty;
		}
		string GetDataLabelText(int pointIndex, double normalizedValue, DataLabelBase dataLabel) {
			return GetDataLabelText(pointIndex, normalizedValue, dataLabel, dataLabel);
		}
		string GetDataLabelText(int pointIndex, double normalizedValue, DataLabelBase dataLabel, DataLabelBase dataLabels) {
			if (dataLabel.Delete)
				return string.Empty;
			StringBuilder sb = new StringBuilder();
			if (dataLabel.ShowSeriesName)
				AppendText(sb, GetSeriesText(), dataLabel.Separator);
			if (dataLabel.ShowCategoryName)
				AppendText(sb, GetArgumentText(pointIndex), dataLabel.Separator);
			if (dataLabel.ShowValue)
				AppendText(sb, GetValueText(pointIndex, dataLabel, dataLabels), dataLabel.Separator);
			if (dataLabel.ShowPercent)
				AppendText(sb, GetPercentText(normalizedValue, dataLabel), dataLabel.Separator);
			if (dataLabel.ShowBubbleSize)
				AppendText(sb, GetBubbleSizeText(pointIndex), dataLabel.Separator);
			return sb.ToString();
		}
		void AppendText(StringBuilder sb, string text, string separator) {
			if (sb.Length > 0)
				sb.Append(separator);
			sb.Append(text);
		}
		string GetArgumentText(int pointIndex) {
			return Arguments.GetDisplayText(pointIndex);
		}
		string GetValueText(int pointIndex, DataLabelBase dataLabel, DataLabelBase dataLabels) {
			VariantValue value = ConvertToVariantValue(Values[pointIndex]);
			if (dataLabel.ShowPercent)
				return NumberFormat.Generic.Format(value, DocumentModel.DataContext).Text;
			if(string.IsNullOrEmpty(dataLabel.NumberFormat.NumberFormatCode) && !string.IsNullOrEmpty(dataLabels.NumberFormat.NumberFormatCode))
				return dataLabels.NumberFormat.Formatter.Format(value, DocumentModel.DataContext).Text;
			return dataLabel.NumberFormat.Formatter.Format(value, DocumentModel.DataContext).Text;
		}
		string GetPercentText(double value, DataLabelBase dataLabel) {
			if (string.IsNullOrEmpty(dataLabel.NumberFormat.NumberFormatCode) ||
				StringExtensions.CompareInvariantCultureIgnoreCase(dataLabel.NumberFormat.NumberFormatCode, "GENERAL") == 0) {
				string format = "P0";
				if (value < 0.0005)
					format = "P2";
				else if (value < 0.005)
					format = "P1";
				return value.ToString(format, DocumentModel.Culture);
			}
			return dataLabel.NumberFormat.Formatter.Format(value, DocumentModel.DataContext).Text;
		}
		string GetBubbleSizeText(int pointIndex) {
			BubbleSeries bubbleSeries = this as BubbleSeries;
			if (bubbleSeries != null) {
				double value = ConvertToDouble(bubbleSeries.BubbleSize[pointIndex]);
				return value.ToString(DocumentModel.Culture);
			}
			return string.Empty;
		}
		double ConvertToDouble(object value) {
			if (value == null)
				return 0.0;
			if (DXConvert.IsDBNull(value))
				return 0.0;
			Type type = value.GetType();
			if (type == typeof(double))
				return Convert.ToDouble(value);
			if (type == typeof(bool))
				return ((bool)value) ? 1.0 : 0.0;
			return 0.0;
		}
		VariantValue ConvertToVariantValue(object value) {
			if (value == null)
				return VariantValue.Empty;
			if (DXConvert.IsDBNull(value))
				return VariantValue.Empty;
			Type type = value.GetType();
			if (type == typeof(string))
				return (string)value;
			if (type == typeof(DateTime)) {
				VariantValue result = new VariantValue();
				result.SetDateTime((DateTime)value, DocumentModel.DataContext);
				return result;
			}
			if (type == typeof(bool))
				return (bool)value;
			if (type == typeof(double))
				return Convert.ToDouble(value);
			return VariantValue.Empty;
		}
		#endregion
		public override void CopyFrom(ISeries value) {
			base.CopyFrom(value);
			SeriesWithDataLabelsAndPoints series = value as SeriesWithDataLabelsAndPoints;
			if (series != null)
				CopyFromCore(series);
		}
		void CopyFromCore(SeriesWithDataLabelsAndPoints value) {
			dataLabels.CopyFrom(value.dataLabels);
			dataLabels.Labels.ForEach(FixLabelPosition);
			dataPoints.CopyFrom(value.dataPoints);
		}
		void FixLabelPosition(DataLabel label) {
			DataLabelPosition position = label.LabelPosition;
			if (!IsCompatibleLabelPosition(position))
				label.LabelPosition = DataLabelPosition.Default;
		}
		protected abstract bool IsCompatibleLabelPosition(DataLabelPosition position);
		public override void ResetToStyle() {
			base.ResetToStyle();
			DataLabels.ResetToStyle();
			int count = DataPoints.Count;
			for (int i = count - 1; i >= 0; i--) {
				DataPoint point = DataPoints[i];
				point.ResetToStyle(GetMarkerSymbolToReset(), IsLinesOnly && ShapeProperties.Outline.Fill.FillType == DrawingFillType.None);
				if (CanRemoveOnResetToStyle(point))
					DataPoints.RemoveAt(i);
			}
		}
		protected virtual MarkerStyle GetMarkerSymbolToReset() {
			return MarkerStyle.Auto;
		}
		protected virtual bool CanRemoveOnResetToStyle(DataPoint dataPoint) {
			return false;
		}
		public override void OnRangeInserting(InsertRangeNotificationContext context) {
			base.OnRangeInserting(context);
			DataLabels.OnRangeInserting(context);
		}
		public override void OnRangeRemoving(RemoveRangeNotificationContext context) {
			base.OnRangeRemoving(context);
			DataLabels.OnRangeRemoving(context);
		}
	}
	#endregion
	#region SeriesWithErrorBarsAndTrendlines
	public abstract class SeriesWithErrorBarsAndTrendlines : SeriesWithDataLabelsAndPoints {
		#region Fields
		ErrorBarsCollection errorBars;
		TrendlineCollection trendlines;
		#endregion
		protected SeriesWithErrorBarsAndTrendlines(IChartView view)
			: base(view) {
			this.errorBars = new ErrorBarsCollection(Parent);
			this.trendlines = new TrendlineCollection(Parent);
		}
		#region Properties
		public ErrorBarsCollection ErrorBars { get { return errorBars; } }
		public TrendlineCollection Trendlines { get { return trendlines; } }
		#endregion
		public override void CopyFrom(ISeries value) {
			base.CopyFrom(value);
			SeriesWithErrorBarsAndTrendlines series = value as SeriesWithErrorBarsAndTrendlines;
			if (series != null)
				CopyFromCore(series);
		}
		void CopyFromCore(SeriesWithErrorBarsAndTrendlines value) {
			errorBars.CopyFrom(value.ErrorBars);
			trendlines.CopyFrom(value.Trendlines);
		}
		public override void ResetToStyle() {
			base.ResetToStyle();
			foreach (ErrorBars errorBars in ErrorBars)
				errorBars.ResetToStyle();
			foreach (Trendline trendLine in trendlines)
				trendLine.ResetToStyle();
		}
		public override void OnRangeInserting(InsertRangeNotificationContext context) {
			base.OnRangeInserting(context);
			errorBars.OnRangeInserting(context);
			trendlines.OnRangeInserting(context);
		}
		public override void OnRangeRemoving(RemoveRangeNotificationContext context) {
			base.OnRangeRemoving(context);
			errorBars.OnRangeRemoving(context);
			trendlines.OnRangeRemoving(context);
		}
	}
	#endregion
	#region SeriesWithPictureOptions
	public abstract class SeriesWithPictureOptions : SeriesWithErrorBarsAndTrendlines {
		#region Fields
		PictureOptions pictureOptions;
		#endregion
		protected SeriesWithPictureOptions(IChartView view)
			: base(view) {
			this.pictureOptions = new PictureOptions(Parent);
		}
		#region Properties
		public PictureOptions PictureOptions { get { return pictureOptions; } }
		#endregion
		public override void CopyFrom(ISeries value) {
			base.CopyFrom(value);
			SeriesWithPictureOptions series = value as SeriesWithPictureOptions;
			if (series != null)
				CopyFromCore(series);
		}
		void CopyFromCore(SeriesWithPictureOptions value) {
			this.pictureOptions.CopyFrom(value.pictureOptions);
		}
	}
	#endregion
	#region ISeriesWithMarker
	public interface ISeriesWithMarker {
		Marker Marker { get; }
	}
	#endregion
	#region SeriesWithMarkerAndSmooth
	public abstract class SeriesWithMarkerAndSmooth : SeriesWithErrorBarsAndTrendlines, ISeriesWithMarker {
		#region Fields
		Marker marker;
		bool smooth = false;
		#endregion
		protected SeriesWithMarkerAndSmooth(IChartView view)
			: base(view) {
			this.marker = new Marker(Parent);
		}
		#region Properties
		public Marker Marker { get { return marker; } }
		#region Smooth
		public bool Smooth {
			get { return smooth; }
			set { SetSmooth(value); }
		}
		void SetSmooth(bool value) {
			if (smooth == value)
				return;
			SeriesSmoothPropertyChangedHistoryItem historyItem = new SeriesSmoothPropertyChangedHistoryItem(DocumentModel, this, smooth, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		public void SetSmoothCore(bool value) {
			this.smooth = value;
			Parent.Invalidate();
		}
		#endregion
		#endregion
		public override void CopyFrom(ISeries value) {
			base.CopyFrom(value);
			CopyFromSeriesWithMarker(value as ISeriesWithMarker);
			SeriesWithMarkerAndSmooth series = value as SeriesWithMarkerAndSmooth;
			if (series != null)
				CopyFromCore(series);
		}
		public void CopyFromSeriesWithMarker(ISeriesWithMarker value) {
			if (value != null) 
				marker.CopyFrom(value.Marker);
		}
		void CopyFromCore(SeriesWithMarkerAndSmooth value) {
			SetSmooth(value.Smooth);
		}
		public override void ResetToStyle() {
			Marker.ResetToStyle(GetMarkerSymbolToReset());
			base.ResetToStyle();
		}
		protected override MarkerStyle GetMarkerSymbolToReset() {
			return Marker.Symbol != MarkerStyle.None ? MarkerStyle.Auto : MarkerStyle.None;
		}
		protected override bool CanRemoveOnResetToStyle(DataPoint dataPoint) {
			return (dataPoint.Marker.Symbol == Marker.Symbol) && (Marker.Symbol == MarkerStyle.Auto || Marker.Symbol == MarkerStyle.None) && dataPoint.ShapeProperties.IsAutomatic;
		}
	}
	#endregion
}
