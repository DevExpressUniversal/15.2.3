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
using System.IO;
using System.Reflection;
using System.Text;
using System.Globalization;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Drawing;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsChartSeriesBuilder
	public class XlsChartSeriesBuilder : IXlsChartBuilder, IXlsChartDataFormatContainer, IXlsChartDataRefContainer {
		#region Fields
		int viewIndex = -1;
		int parentIndex = -1;
		readonly List<XlsChartDataRef> dataRefs = new List<XlsChartDataRef>();
		readonly List<XlsChartDataFormat> dataFormats = new List<XlsChartDataFormat>();
		readonly List<XlsChartLegendEntry> legendEntries = new List<XlsChartLegendEntry>();
		ISeries chartSeries = null;
		readonly List<XlsChartCachedValue> argumentCache = new List<XlsChartCachedValue>();
		readonly List<XlsChartCachedValue> valueCache = new List<XlsChartCachedValue>();
		readonly List<XlsChartCachedValue> bubbleSizeCache = new List<XlsChartCachedValue>();
		readonly XlsChartTrendline trendline = new XlsChartTrendline();
		#endregion
		#region Properties
		public XlsSeriesDataType ArgumentType { get; set; }
		public int ArgumentCount { get; set; }
		public int ValueCount { get; set; }
		public int BubbleSizeCount { get; set; }
		public int ViewIndex { get { return viewIndex; } set { viewIndex = value; } }
		public int ParentIndex { get { return parentIndex; } set { parentIndex = value; } }
		public List<XlsChartDataRef> DataRefs { get { return dataRefs; } }
		public List<XlsChartDataFormat> DataFormats { get { return dataFormats; } }
		public List<XlsChartLegendEntry> LegendEntries { get { return legendEntries; } }
		public ISeries ChartSeries { get { return chartSeries; } }
		public List<XlsChartCachedValue> ArgumentCache { get { return argumentCache; } }
		public List<XlsChartCachedValue> ValueCache { get { return valueCache; } }
		public List<XlsChartCachedValue> BubbleSizeCache { get { return bubbleSizeCache; } }
		public int SeriesIndex { get; set; }
		public int SeriesOrder { get; set; }
		public XlsChartTrendline Trendline { get { return trendline; } }
		#endregion
		#region IXlsChartBuilder Members
		public void Execute(XlsContentBuilder contentBuilder) {
			if (contentBuilder.CurrentChart == null)
				return;
			contentBuilder.SeriesFormats.Add(this);
		}
		#endregion
		#region IXlsChartDataFormatContainer Members
		void IXlsChartDataFormatContainer.Add(XlsChartDataFormat dataFormat) {
			this.dataFormats.Add(dataFormat);
		}
		#endregion
		#region CreateSeries
		public ISeries CreateSeries(XlsContentBuilder contentBuilder, IChartView view) {
			this.chartSeries = CreateSeriesCore(contentBuilder, view);
			return this.chartSeries;
		}
		ISeries CreateSeriesCore(XlsContentBuilder contentBuilder, IChartView view) {
			switch (view.ViewType) {
				case ChartViewType.Area:
				case ChartViewType.Area3D:
					return CreateAreaSeries(contentBuilder, view);
				case ChartViewType.Bar:
				case ChartViewType.Bar3D:
					return CreateBarSeries(contentBuilder, view);
				case ChartViewType.Bubble:
					return CreateBubbleSeries(contentBuilder, view);
				case ChartViewType.Line:
				case ChartViewType.Line3D:
				case ChartViewType.Stock:
					return CreateLineSeries(contentBuilder, view);
				case ChartViewType.Doughnut:
				case ChartViewType.Pie:
				case ChartViewType.Pie3D:
				case ChartViewType.OfPie:
					return CreatePieSeries(contentBuilder, view);
				case ChartViewType.Surface:
				case ChartViewType.Surface3D:
					return CreateSurfaceSeries(contentBuilder, view);
				case ChartViewType.Scatter:
					return CreateScatterSeries(contentBuilder, view);
				case ChartViewType.Radar:
					return CreateRadarSeries(contentBuilder, view);
			}
			return null;
		}
		ISeries CreateAreaSeries(XlsContentBuilder contentBuilder, IChartView view) {
			AreaSeries series = new AreaSeries(view);
			SetupIndexAndOrder(series);
			SetupSeriesText(contentBuilder, series);
			SetupDataRefs(contentBuilder, series);
			SetupDataFormat(contentBuilder, series);
			return series;
		}
		ISeries CreateBarSeries(XlsContentBuilder contentBuilder, IChartView view) {
			BarSeries series = new BarSeries(view);
			SetupIndexAndOrder(series);
			SetupSeriesText(contentBuilder, series);
			SetupDataRefs(contentBuilder, series);
			SetupDataFormat(contentBuilder, series);
			return series;
		}
		ISeries CreateBubbleSeries(XlsContentBuilder contentBuilder, IChartView view) {
			BubbleSeries series = new BubbleSeries(view);
			SetupIndexAndOrder(series);
			SetupSeriesText(contentBuilder, series);
			SetupDataRefs(contentBuilder, series);
			XlsChartDataRef dataRef = FindDataRef(XlsChartDataRefId.BubbleSize);
			if (dataRef != null && dataRef.DataType != XlsChartDataRefType.Auto)
				series.BubbleSize = CreateDataReference(contentBuilder, dataRef, true);
			SetupDataFormat(contentBuilder, series);
			return series;
		}
		ISeries CreateLineSeries(XlsContentBuilder contentBuilder, IChartView view) {
			LineSeries series = new LineSeries(view);
			SetupIndexAndOrder(series);
			SetupSeriesText(contentBuilder, series);
			SetupDataRefs(contentBuilder, series);
			SetupDataFormat(contentBuilder, series);
			return series;
		}
		ISeries CreatePieSeries(XlsContentBuilder contentBuilder, IChartView view) {
			PieSeries series = new PieSeries(view);
			SetupIndexAndOrder(series);
			SetupSeriesText(contentBuilder, series);
			SetupDataRefs(contentBuilder, series);
			SetupDataFormat(contentBuilder, series);
			return series;
		}
		ISeries CreateSurfaceSeries(XlsContentBuilder contentBuilder, IChartView view) {
			SurfaceSeries series = new SurfaceSeries(view);
			SetupIndexAndOrder(series);
			SetupSeriesText(contentBuilder, series);
			SetupDataRefs(contentBuilder, series);
			SetupDataFormat(contentBuilder, series);
			return series;
		}
		ISeries CreateScatterSeries(XlsContentBuilder contentBuilder, IChartView view) {
			ScatterSeries series = new ScatterSeries(view);
			SetupIndexAndOrder(series);
			SetupSeriesText(contentBuilder, series);
			SetupDataRefs(contentBuilder, series);
			SetupDataFormat(contentBuilder, series);
			return series;
		}
		ISeries CreateRadarSeries(XlsContentBuilder contentBuilder, IChartView view) {
			RadarSeries series = new RadarSeries(view);
			SetupIndexAndOrder(series);
			SetupSeriesText(contentBuilder, series);
			SetupDataRefs(contentBuilder, series);
			SetupDataFormat(contentBuilder, series);
			return series;
		}
		#endregion
		#region SeriesData
		public void SetupDataCache(XlsContentBuilder contentBuilder) {
			if (ChartSeries == null)
				return;
			SetupDataReference(contentBuilder, ChartSeries.Arguments as ChartDataReference, ArgumentCache, XlsChartDataRefId.Arguments);
			SetupDataReference(contentBuilder, ChartSeries.Values as ChartDataReference, ValueCache, XlsChartDataRefId.Values);
			BubbleSeries bubbleSeries = ChartSeries as BubbleSeries;
			if (bubbleSeries != null)
				SetupDataReference(contentBuilder, bubbleSeries.BubbleSize as ChartDataReference, BubbleSizeCache, XlsChartDataRefId.BubbleSize);
			CheckDataRefs(contentBuilder);
		}
		void SetupDataReference(XlsContentBuilder contentBuilder, ChartDataReference dataReference, List<XlsChartCachedValue> dataCache, XlsChartDataRefId dataRefId) {
			if (dataReference == null)
				return;
			XlsChartDataRef dataRef = FindDataRef(dataRefId);
			if (dataRef == null || dataRef.DataType != XlsChartDataRefType.Literal)
				return;
			if (dataCache.Count == 0) {
				dataReference.CachedValue = VariantValue.Empty;
				return;
			}
			int maxIndex = -1;
			foreach (XlsChartCachedValue item in dataCache)
				maxIndex = Math.Max(maxIndex, item.Index);
			int count = maxIndex + 1;
			List<VariantValue> values = new List<VariantValue>(count);
			for (int i = 0; i < count; i++)
				values.Add(VariantValue.Empty);
			foreach (XlsChartCachedValue item in dataCache)
				values[item.Index] = item.Value;
			VariantArray array = new VariantArray();
			array.SetValues(values, count, 1);
			VariantValue cachedValue = VariantValue.FromArray(array);
			if (dataReference.FormulaData.Expression == null || dataReference.FormulaData.Expression.Count == 0) {
				ParsedExpression expression = BasicExpressionCreator.CreateExpressionForVariantValue(cachedValue, OperandDataType.Default, contentBuilder.DocumentModel.DataContext);
				dataReference.FormulaData.SetExpressionCore(expression);
			}
			dataReference.CachedValue = cachedValue;
		}
		bool IsValidDataReference(IDataReference dataReference) {
			ChartDataReference dataRef = dataReference as ChartDataReference;
			if (dataRef == null)
				return true;
			return dataRef.FormulaData.Expression != null && dataRef.FormulaData.Expression.Count > 0;
		}
		void CheckDataRefs(XlsContentBuilder contentBuilder) {
			SeriesBase series = ChartSeries as SeriesBase;
			if (series == null)
				return;
			if (!IsValidDataReference(series.Arguments))
				series.Arguments = DataReference.Empty;
			if (!IsValidDataReference(series.Values))
				series.Values = DataReference.Empty;
			BubbleSeries bubbleSeries = ChartSeries as BubbleSeries;
			if (bubbleSeries != null && !IsValidDataReference(bubbleSeries.BubbleSize))
				bubbleSeries.BubbleSize = DataReference.Empty;
			if (series.Arguments.Equals(DataReference.Empty) && series.Values.Equals(DataReference.Empty)) {
				ChartDataReference dataRef = new ChartDataReference(contentBuilder.DocumentModel, ChartViewSeriesDirection.Vertical, true);
				VariantArray array = VariantArray.Create(1, 1);
				array.SetValue(0, 0, 10);
				VariantValue cachedValue = VariantValue.FromArray(array);
				ParsedExpression expression = BasicExpressionCreator.CreateExpressionForVariantValue(cachedValue, OperandDataType.Default, contentBuilder.DocumentModel.DataContext);
				dataRef.FormulaData.SetExpressionCore(expression);
				dataRef.CachedValue = cachedValue;
				series.Values = dataRef;
			}
		}
		#endregion
		#region Utils
		XlsChartDataRef FindDataRef(XlsChartDataRefId id) {
			foreach (XlsChartDataRef item in dataRefs) {
				if (item.Id == id)
					return item;
			}
			return null;
		}
		ChartDataReference CreateArgumentsDataReference(XlsContentBuilder contentBuilder, XlsChartDataRef dataRef, bool isNumeric, ChartSeriesType seriesType) {
			if (isNumeric && seriesType != ChartSeriesType.Bubble && seriesType != ChartSeriesType.Scatter) {
				FormulaData formulaData = new FormulaData(contentBuilder.DocumentModel, new ChartDataReferenceExpressionFilter());
				formulaData.SetExpressionCore(XlsParsedThingConverter.ToModelExpression(dataRef.Expression, contentBuilder.RPNContext));
				if (dataRef.Expression.Count > 0) {
					WorkbookDataContext context = contentBuilder.DocumentModel.DataContext;
					context.PushCurrentCell(0, 0);
					context.Workbook.SuppressCellValueAssignment = false;
					try {
						VariantValue value = formulaData.CachedValue;
						if (value.IsCellRange) {
							CellRangeBase range = value.CellRangeValue;
							isNumeric = IsNumber(range);
						}
					}
					finally {
						context.PopCurrentCell();
						context.Workbook.SuppressCellValueAssignment = true;
					}
				}
			}
			return CreateDataReference(contentBuilder, dataRef, isNumeric);
		}
		bool IsNumber(CellRangeBase range) {
			foreach (ICellBase cell in range.GetExistingCellsEnumerable()) {
				Cell modelCell = cell as Cell;
				if (modelCell != null && modelCell.ExtractValue().IsText)
					return false;
			}
			return true;
		}
		ChartDataReference CreateDataReference(XlsContentBuilder contentBuilder, XlsChartDataRef dataRef, bool isNumeric) {
			ChartDataReference result = new ChartDataReference(contentBuilder.DocumentModel, ChartViewSeriesDirection.Vertical, isNumeric);
			result.FormulaData.SetExpressionCore(XlsParsedThingConverter.ToModelExpression(dataRef.Expression, contentBuilder.RPNContext));
			if (dataRef.Expression.Count > 0) {
				WorkbookDataContext context = contentBuilder.DocumentModel.DataContext;
				context.PushCurrentCell(0, 0);
				context.Workbook.SuppressCellValueAssignment = false;
				try {
					VariantValue value = result.CachedValue;
					if (value.IsCellRange) {
						CellRangeBase range = value.CellRangeValue;
						CellPosition topLeft = range.TopLeft;
						CellPosition bottomRight = range.BottomRight;
						if ((bottomRight.Column - topLeft.Column) >= (bottomRight.Row - topLeft.Row))
							result.SeriesDirection = ChartViewSeriesDirection.Horizontal;
					}
				}
				finally {
					context.PopCurrentCell();
					context.Workbook.SuppressCellValueAssignment = true;
				}
			}
			return result;
		}
		IChartText CreateSeriesText(XlsContentBuilder contentBuilder, XlsChartDataRef dataRef) {
			if (dataRef.DataType == XlsChartDataRefType.Reference) {
				ChartTextRef textRef = new ChartTextRef(contentBuilder.CurrentChart);
				textRef.FormulaData.SetExpressionCore(XlsParsedThingConverter.ToModelExpression(dataRef.Expression, contentBuilder.RPNContext));
				if(!string.IsNullOrEmpty(dataRef.SeriesText))
					textRef.CachedValue = dataRef.SeriesText;
				return textRef;
			}
			ChartTextValue textValue = new ChartTextValue(contentBuilder.CurrentChart);
			textValue.PlainText = dataRef.SeriesText;
			return textValue;
		}
		void SetupIndexAndOrder(SeriesBase series) {
			series.Index = SeriesOrder;
			series.Order = SeriesIndex;
		}
		void SetupSeriesText(XlsContentBuilder contentBuilder, SeriesBase series) {
			XlsChartDataRef dataRef = FindDataRef(XlsChartDataRefId.Name);
			if (dataRef != null && dataRef.DataType != XlsChartDataRefType.Auto && (dataRef.Expression.Count > 0 || !string.IsNullOrEmpty(dataRef.SeriesText)))
				series.Text = CreateSeriesText(contentBuilder, dataRef);
		}
		void SetupDataRefs(XlsContentBuilder contentBuilder, SeriesBase series) {
			XlsChartDataRef dataRef = FindDataRef(XlsChartDataRefId.Arguments);
			if (dataRef != null && dataRef.DataType != XlsChartDataRefType.Auto)
				series.Arguments = CreateArgumentsDataReference(contentBuilder, dataRef, ArgumentType == XlsSeriesDataType.Numeric, series.SeriesType);
			dataRef = FindDataRef(XlsChartDataRefId.Values);
			if (dataRef != null && dataRef.DataType != XlsChartDataRefType.Auto)
				series.Values = CreateDataReference(contentBuilder, dataRef, true);
		}
		void SetupDataFormat(XlsContentBuilder contentBuilder, SeriesBase series) {
			foreach (XlsChartDataFormat format in dataFormats) {
				if (format.SeriesIndex == SeriesIndex)
					format.SetupSeries(series);
			}
		}
		protected internal void GetOrderFromDataFormat() {
			foreach (XlsChartDataFormat format in dataFormats) {
				if (format.SeriesIndex == SeriesIndex && format.IsSeriesDataFormat) {
					SeriesOrder = format.SeriesOrder;
					break;
				}
			}
		}
		#endregion
		#region IXlsChartDataRefContainer Members
		void IXlsChartDataRefContainer.Add(XlsChartDataRef dataRef) {
			DataRefs.Add(dataRef);
		}
		void IXlsChartDataRefContainer.SetSeriesText(string value) {
			if (DataRefs.Count > 0) {
				XlsChartDataRef dataRef = DataRefs[DataRefs.Count - 1];
				dataRef.SeriesText = value;
			}
		}
		#endregion
	}
	#endregion
	#region XlsChartTrendline
	public class XlsChartTrendline {
		public bool Apply { get; set; }
		public ChartTrendlineType TrendlineType { get; set; }
		public int OrderOrPeriod { get; set; }
		public bool DisplayEquation { get; set; }
		public bool DisplayRSquare { get; set; }
		public bool HasIntercept { get; set; }
		public double Intercept { get; set; }
		public double Backward { get; set; }
		public double Forward { get; set; }
	}
	#endregion
}
