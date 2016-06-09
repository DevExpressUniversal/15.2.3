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

using System.Xml;
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	public abstract class SeriesBaseDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		protected static void AddCommonHandlers(ElementHandlerTable<SpreadsheetMLBaseImporter> table) {
			table.Add("idx", OnIndex);
			table.Add("order", OnOrder);
			table.Add("spPr", OnShapeProperties);
			table.Add("tx", OnSeriesText);
		}
		protected static void AddCatValHandlers(ElementHandlerTable<SpreadsheetMLBaseImporter> table) {
			table.Add("cat", OnArguments);
			table.Add("val", OnValues);
		}
		protected static void AddXYValHandlers(ElementHandlerTable<SpreadsheetMLBaseImporter> table) {
			table.Add("xVal", OnArguments);
			table.Add("yVal", OnValues);
		}
		protected static void AddDataLabelsAndPointsHandlers(ElementHandlerTable<SpreadsheetMLBaseImporter> table) {
			table.Add("dPt", OnDataPoint);
			table.Add("dLbls", OnDataLabels);
		}
		protected static void AddTrendlinesAndErrorBarsHandlers(ElementHandlerTable<SpreadsheetMLBaseImporter> table) {
			table.Add("trendline", OnTrendline);
			table.Add("errBars", OnErrorBars);
		}
		protected static void AddPictureOptionsHandler(ElementHandlerTable<SpreadsheetMLBaseImporter> table) {
			table.Add("pictureOptions", OnPictureOptions);
		}
		static SeriesBaseDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (SeriesBaseDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly SeriesBase series;
		bool isFirstSeries;
		IOpenXmlChartDataReference arguments;
		IOpenXmlChartDataReference values;
		#endregion
		protected SeriesBaseDestination(SpreadsheetMLBaseImporter importer, SeriesBase series, bool isFirstSeries)
			: base(importer) {
			this.series = series;
			this.isFirstSeries = isFirstSeries;
		}
		protected virtual bool ArgumentAxisIsNumber { get { return false; } }
		public override void ProcessElementClose(XmlReader reader) {
			base.ProcessElementClose(reader);
			ChartViewSeriesDirection seriesDirection = ChartViewSeriesDirection.Vertical;
			if (isFirstSeries) {
				if (values != null && values.TryDetectDirectionByReference(Importer.DocumentModel.DataContext, out seriesDirection))
					series.View.Parent.SeriesDirection = seriesDirection;
			}
			else
				seriesDirection = series.View.SeriesDirection;
			if (values != null)
				series.Values = values.ToDataReference(Importer.DocumentModel, seriesDirection, true);
			if (arguments != null)
				series.Arguments = arguments.ToDataReference(Importer.DocumentModel, seriesDirection, ArgumentAxisIsNumber);
		}
		#region Handlers
		#region SeriesShared
		static Destination OnIndex(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new IntegerValueDestination(importer,
				delegate(int value) {
					if (value < 0)
						importer.ThrowInvalidFile();
					GetThis(importer).series.SetIndexCore(value);
				},
				"val", -1);
		}
		static Destination OnOrder(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new IntegerValueDestination(importer,
				delegate(int value) {
					GetThis(importer).series.SetOrderCore(value);
				},
				"val", 0);
		}
		static Destination OnShapeProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ChartShapePropertiesDestination(importer, GetThis(importer).series.ShapeProperties);
		}
		static Destination OnSeriesText(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			SeriesBase series = GetThis(importer).series;
			return new ChartSeriesTextDestination(importer, series.Parent, delegate(IChartText value) { series.SetTextCore(value); });
		}
		#endregion
		#region Arguments and values
		static Destination OnArguments(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			SeriesBaseDestination thisDestination = GetThis(importer);
			return new ChartDataReferenceDestination(importer,
				DataReferenceTypeOpenXml.All,
				delegate(IOpenXmlChartDataReference value) { thisDestination.arguments = value; });
		}
		static Destination OnValues(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			SeriesBaseDestination thisDestination = GetThis(importer);
			return new ChartDataReferenceDestination(importer,
				DataReferenceTypeOpenXml.NumberReference | DataReferenceTypeOpenXml.NumberLiteral,
				delegate(IOpenXmlChartDataReference value) { thisDestination.values = value; });
		}
		#endregion
		#region DataLabels and Points
		static Destination OnDataPoint(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			SeriesWithDataLabelsAndPoints series = (SeriesWithDataLabelsAndPoints)GetThis(importer).series;
			DataPoint point = new DataPoint(series.Parent, 0);
			series.DataPoints.AddWithoutHistoryAndNotifications(point);
			return new ChartDataPointDestination(importer, point);
		}
		static Destination OnDataLabels(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			SeriesWithDataLabelsAndPoints series = (SeriesWithDataLabelsAndPoints)GetThis(importer).series;
			return new DataLabelsDestination(importer, series.DataLabels);
		}
		#endregion
		#region Trendlines and ErrorBars
		static Destination OnTrendline(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			SeriesWithErrorBarsAndTrendlines series = (SeriesWithErrorBarsAndTrendlines)GetThis(importer).series;
			Trendline trendline = new Trendline(series.Parent);
			series.Trendlines.AddCore(trendline);
			return new TrendlineDestination(importer, trendline);
		}
		static Destination OnErrorBars(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			SeriesWithErrorBarsAndTrendlines series = (SeriesWithErrorBarsAndTrendlines)GetThis(importer).series;
			ErrorBars errorBars = new ErrorBars(series.Parent);
			series.ErrorBars.AddCore(errorBars);
			return new ErrorBarsDestination(importer, errorBars);
		}
		#endregion
		#region PictureOptions
		static Destination OnPictureOptions(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			SeriesWithPictureOptions series = (SeriesWithPictureOptions)GetThis(importer).series;
			return new PictureOptionsDestination(importer, series.PictureOptions);
		}
		#endregion
		#endregion
	}
}
