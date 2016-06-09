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
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using DevExpress.Utils;
using DevExpress.Office.Services;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraSpreadsheet.Import.Xls;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Export;
using DevExpress.XtraSpreadsheet.Export.OpenXml;
using DevExpress.Utils.Zip;
using System.Xml;
using DevExpress.Office;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Export.Xls {
	public partial class XlsChartExporter {
		#region SeriesFormats
		void WriteSeries(ISeries series) {
			XlsCommandChartSeries command = new XlsCommandChartSeries();
			ChartDataReference argumentReference = series.Arguments as ChartDataReference;
			command.ArgumentType = (argumentReference != null && !argumentReference.IsNumber) ? XlsSeriesDataType.Text : XlsSeriesDataType.Numeric;
			command.ArgumentCount = GetDataValueCount(series.Arguments);
			command.ValueCount = GetDataValueCount(series.Values);
			BubbleSeries bubbleSeries = series as BubbleSeries;
			command.BubbleSizeCount = (bubbleSeries != null) ? GetDataValueCount(bubbleSeries.BubbleSize) : 0;
			command.Write(StreamWriter);
		}
		void WriteSeriesText(ISeries series) {
			XlsCommandChartDataRef command = new XlsCommandChartDataRef();
			command.DataRef.Id = XlsChartDataRefId.Name;
			command.DataRef.IsSourceLinked = true;
			if (series.Text.TextType == ChartTextType.Ref) {
				command.DataRef.DataType = XlsChartDataRefType.Reference;
				ChartTextRef textRef = series.Text as ChartTextRef;
				ParsedExpression expression = XlsParsedThingConverter.ToXlsExpression(textRef.Expression, ExportStyleSheet.RPNContext);
				if (expression.IsXlsChartFormulaCompliant())
					command.DataRef.SetExpression(expression, ExportStyleSheet.RPNContext);
				else
					command.DataRef.DataType = XlsChartDataRefType.Auto;
			}
			else if (series.Text.TextType == ChartTextType.Value)
				command.DataRef.DataType = XlsChartDataRefType.Literal;
			else
				command.DataRef.DataType = XlsChartDataRefType.Auto;
			command.Write(StreamWriter);
			WriteSeriesText(series.Text.PlainText);
		}
		void WriteSeriesText(string seriesText) {
			XlsCommandChartSeriesText command = PrepareSeriesText(seriesText);
			if (command == null)
				return;
			command.Write(StreamWriter);
		}
		XlsCommandChartSeriesText PrepareSeriesText(string seriesText) {
			if (string.IsNullOrEmpty(seriesText))
				return null;
			if (seriesText.Length > 255)
				seriesText = seriesText.Substring(0, 255);
			XlsCommandChartSeriesText command = new XlsCommandChartSeriesText();
			command.Value = seriesText;
			return command;
		}
		void WriteSeriesDataRef(IDataReference dataRef, XlsChartDataRefId refId) {
			XlsCommandChartDataRef command = new XlsCommandChartDataRef();
			command.DataRef.Id = refId;
			ChartDataReference dataReference = dataRef as ChartDataReference;
			if (dataReference == null)
				command.DataRef.DataType = refId == XlsChartDataRefId.Arguments ? XlsChartDataRefType.Auto : XlsChartDataRefType.Literal;
			else {
				DataReferenceType refType = dataReference.GetReferenceType();
				bool isRef = refType == DataReferenceType.MultiLevelString || refType == DataReferenceType.Number || refType == DataReferenceType.String;
				command.DataRef.DataType = isRef ? XlsChartDataRefType.Reference : XlsChartDataRefType.Literal;
				if (isRef) {
					XlsParsedThingConversionResult conversionResult = new XlsParsedThingConversionResult();
					ParsedExpression expression = XlsParsedThingConverter.ToXlsExpression(dataReference.Expression, ExportStyleSheet.RPNContext, conversionResult);
					if (conversionResult.OutOfXlsLimits)
						command.DataRef.DataType = XlsChartDataRefType.Literal;
					else if (expression.IsXlsChartFormulaCompliant()) 
						command.DataRef.SetExpression(expression, ExportStyleSheet.RPNContext);
					else
						command.DataRef.DataType = XlsChartDataRefType.Auto;
				}
			}
			command.DataRef.IsSourceLinked = true;
			command.Write(StreamWriter);
		}
		void WriteSeriesToView() {
			XlsCommandChartSeriesToView command = new XlsCommandChartSeriesToView();
			command.ViewIndex = viewIndex;
			command.Write(StreamWriter);
		}
		void WriteLegendException(ISeries series, int seriesIndex) {
			bool isSurface = series.SeriesType == ChartSeriesType.Surface;
			bool isSeriesLegend = seriesList.Count > 1 || IsSingleSeriesLegend(series);
			LegendEntryCollection collection = Chart.Legend.Entries;
			int count = collection.Count;
			for (int i = 0; i < count; i++) {
				LegendEntry entry = collection[i];
				if (isSeriesLegend && !isSurface) {
					if (seriesIndex == entry.Index)
						WriteLegendException(entry, XlsChartDefs.PointIndexOfSeries);
				}
				else if (seriesIndex == 0)
					WriteLegendException(entry, entry.Index);
			}
		}
		void WriteLegendException(LegendEntry entry, int index) {
			XlsCommandChartLegendException command = new XlsCommandChartLegendException();
			command.Index = index;
			command.Deleted = entry.Delete;
			command.Formatted = !entry.TextProperties.IsDefault;
			command.Write(StreamWriter);
			if (command.Formatted) {
				WriteBegin();
				WriteText(entry.TextProperties);
				WriteBegin();
				WritePosition(null);
				WriteFontX(entry.TextProperties.Paragraphs, ChartElement.Legend);
				WriteBRAI();
				WriteEnd();
				WriteEnd();
			}
		}
		void WriteText(TextProperties textProperties) {
			XlsCommandChartText command = PrepareTextProperties(textProperties);
			command.AutoText = true;
			command.IsGenerated = true;
			command.DataLabelPos = DataLabelPosition.Default;
			command.Write(StreamWriter);
		}
		bool IsSingleSeriesLegend(ISeries series) {
			bool varyColors = false;
			ChartViewWithVaryColors viewWithVaryColors = series.View as ChartViewWithVaryColors;
			if (viewWithVaryColors != null)
				varyColors = viewWithVaryColors.VaryColors;
			bool hasCustomDataPoints = HasCustomDataPoints(series as SeriesWithDataLabelsAndPoints);
			return seriesList.Count == 1 && !hasCustomDataPoints && !varyColors;
		}
		bool HasCustomDataPoints(SeriesWithDataLabelsAndPoints series) {
			if (series == null)
				return false;
			DataPointCollection collection = series.DataPoints;
			int count = collection.Count;
			for (int i = 0; i < count; i++)
				if (!collection[i].ShapeProperties.IsAutomatic)
					return true;
			return false;
		}
		#endregion
		#region SERIESDATA
		protected void WriteSeriesData() {
			WriteDimensions();
			WriteSeriesDataIndex(dataIndexArguments);
			WriteSeriesDataCache(WriteArgumentCache);
			WriteSeriesDataIndex(dataIndexValues);
			WriteSeriesDataCache(WriteValueCache);
			WriteSeriesDataIndex(dataIndexBubbleSize);
			WriteSeriesDataCache(WriteBubbleSizeCache);
		}
		void WriteDimensions() {
			XlsCommandDimensions command = new XlsCommandDimensions();
			command.Dimensions.FirstRowIndex = 1;
			command.Dimensions.FirstColumnIndex = 1;
			command.Dimensions.LastRowIndex = CalcSeriesDataRowCount();
			command.Dimensions.LastColumnIndex = CalcSeriesDataColCount();
			command.Write(StreamWriter);
		}
		void WriteSeriesDataIndex(short value) {
			XlsCommandChartSeriesDataIndex command = new XlsCommandChartSeriesDataIndex();
			command.Value = value;
			command.Write(StreamWriter);
		}
		void WriteSeriesDataCache(Action<ISeries> action) {
			for (seriesIndex = 0; seriesIndex < seriesList.Count; seriesIndex++) {
				ISeries series = seriesList[seriesIndex];
				viewIndex = viewIndexTable[series.View];
				action(series);
			}
		}
		int CalcSeriesDataRowCount() {
			int result = 0;
			foreach (IChartView view in Chart.Views)
				foreach (ISeries series in view.Series)
					result = Math.Max(result, CalcSeriesDataRowCount(series));
			return result;
		}
		int CalcSeriesDataRowCount(ISeries series) {
			int result = GetDataValueCount(series.Arguments);
			result = Math.Max(result, GetDataValueCount(series.Values));
			BubbleSeries bubbleSeries = series as BubbleSeries;
			if (bubbleSeries != null)
				result = Math.Max(result, GetDataValueCount(bubbleSeries.BubbleSize));
			return result;
		}
		int CalcSeriesDataColCount() {
			int result = 0;
			foreach (IChartView view in Chart.Views)
				result += view.Series.Count;
			return result;
		}
		int GetDataValueCount(IDataReference dataRef) {
			ChartDataReference dataReference = dataRef as ChartDataReference;
			if (dataReference != null) {
				DataReferenceType refType = dataReference.GetReferenceType();
				bool isRef = refType == DataReferenceType.MultiLevelString || refType == DataReferenceType.Number || refType == DataReferenceType.String;
				if (isRef) {
					ParsedExpression expression = XlsParsedThingConverter.ToXlsExpression(dataReference.Expression, ExportStyleSheet.RPNContext);
					if (!expression.IsXlsChartFormulaCompliant())
						return 0;
				}
			}
			return (int)dataRef.ValuesCount;
		}
		void WriteArgumentCache(ISeries series) {
			if (!IsCachedData(series.Arguments))
				return;
			WriteDataReferenceCache(series.Arguments);
		}
		void WriteValueCache(ISeries series) {
			if (!IsCachedData(series.Values))
				return;
			WriteDataReferenceCache(series.Values);
		}
		void WriteBubbleSizeCache(ISeries series) {
			BubbleSeries bubbleSeries = series as BubbleSeries;
			if (bubbleSeries == null || !IsCachedData(bubbleSeries.BubbleSize))
				return;
			WriteDataReferenceCache(bubbleSeries.BubbleSize);
		}
		bool IsCachedData(IDataReference dataReference) {
			ChartDataReference dataRef = dataReference as ChartDataReference;
			if (dataRef == null)
				return false;
			DataReferenceType refType = dataRef.GetReferenceType();
			if (refType == DataReferenceType.NumberLiteral || refType == DataReferenceType.StringLiteral)
				return true;
			XlsParsedThingConversionResult conversionResult = new XlsParsedThingConversionResult();
			ParsedExpression expression = XlsParsedThingConverter.ToXlsExpression(dataRef.Expression, ExportStyleSheet.RPNContext, conversionResult);
			if (conversionResult.OutOfXlsLimits)
				return true;
			if (!expression.IsXlsChartFormulaCompliant())
				return false;
			VariantValue cachedValue = dataRef.CachedValue;
			if (cachedValue.IsCellRange) {
				CellUnion unionRange = cachedValue.CellRangeValue as CellUnion;
				CellRange range = cachedValue.CellRangeValue as CellRange;
				if (unionRange != null) {
					foreach (CellRangeBase innerRange in unionRange.InnerCellRanges) {
						if (!object.ReferenceEquals(innerRange.Worksheet, Chart.Worksheet))
							return true;
					}
					return false;
				}
				else if (range != null)
					return !object.ReferenceEquals(range.Worksheet, Chart.Worksheet);
			}
			return true;
		}
		void WriteDataReferenceCache(IDataReference dataReference) {
			ChartDataReference dataRef = dataReference as ChartDataReference;
			if (dataRef == null)
				return;
			for (int i = 0; i < dataRef.ValuesCount; i++) {
				VariantValue value = dataRef.GetValue(i);
				if (value.IsNumeric)
					WriteNumericCacheValue(i, value.NumericValue);
				else if (value.IsInlineText)
					WriteStringCacheValue(i, value.InlineTextValue);
				else if (value.IsBoolean)
					WriteBoolCacheValue(i, value.BooleanValue);
				else if (value.IsError)
					WriteErrorCacheValue(i, value.ErrorValue.Value);
				else
					WriteEmptyCacheValue(i);
			}
		}
		void WriteEmptyCacheValue(int index) {
			XlsCommandBlank command = new XlsCommandBlank();
			command.RowIndex = index;
			command.ColumnIndex = seriesIndex;
			command.FormatIndex = 0;
			command.Write(StreamWriter);
		}
		void WriteNumericCacheValue(int index, double value) {
			XlsCommandNumber command = new XlsCommandNumber();
			command.RowIndex = index;
			command.ColumnIndex = seriesIndex;
			command.FormatIndex = 0;
			command.Value = value;
			command.Write(StreamWriter);
		}
		void WriteStringCacheValue(int index, string value) {
			XlsCommandLabel command = new XlsCommandLabel();
			command.RowIndex = index;
			command.ColumnIndex = seriesIndex;
			command.FormatIndex = 0;
			command.Value = value;
			command.Write(StreamWriter);
		}
		void WriteBoolCacheValue(int index, bool value) {
			XlsCommandBoolErr command = new XlsCommandBoolErr();
			command.RowIndex = index;
			command.ColumnIndex = seriesIndex;
			command.FormatIndex = 0;
			command.BoolValue = value;
			command.Write(StreamWriter);
		}
		void WriteErrorCacheValue(int index, VariantValue value) {
			XlsCommandBoolErr command = new XlsCommandBoolErr();
			command.RowIndex = index;
			command.ColumnIndex = seriesIndex;
			command.FormatIndex = 0;
			command.ErrorValue = value;
			command.Write(StreamWriter);
		}
		#endregion
	}
}
