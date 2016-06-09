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

using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using System.Collections.Generic;
using System.Xml;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region StockChartViewDestination
	public class StockChartViewDestination : ChartViewDestinationBase {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			AddAxisIdHandler(result);
			result.Add("ser", OnSeries);
			result.Add("dLbls", OnDataLabels);
			result.Add("dropLines", OnDropLines);
			result.Add("hiLowLines", OnHiLowLines);
			result.Add("upDownBars", OnUpDownBars);
			return result;
		}
		static StockChartViewDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (StockChartViewDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly StockChartView view;
		#endregion
		public StockChartViewDestination(SpreadsheetMLBaseImporter importer, StockChartView view, List<int> viewAxesList)
			: base(importer, view, viewAxesList) {
			this.view = view;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		protected internal StockChartView View { get { return view; } }
		#endregion
		#region Handlers
		static Destination OnDataLabels(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DataLabelsDestination(importer, GetThis(importer).view.DataLabels);
		}
		static Destination OnDropLines(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			StockChartView view = GetThis(importer).view;
			view.ShowDropLines = true;
			return new InnerShapePropertiesDestination(importer, view.DropLinesProperties);
		}
		static Destination OnHiLowLines(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			StockChartView view = GetThis(importer).view;
			view.ShowHiLowLines = true;
			return new InnerShapePropertiesDestination(importer, view.HiLowLinesProperties);
		}
		static Destination OnSeries(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			StockChartView view = GetThis(importer).view;
			LineSeries series = new LineSeries(view);
			view.Series.AddCore(series);
			return new LineChartSeriesDestination(importer, series, view.Series.Count <= 1);
		}
		static Destination OnUpDownBars(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			StockChartView view = GetThis(importer).view;
			view.ShowUpDownBars = true;
			return new UpDownBarsDestination(importer, view.UpDownBars);
		}
		#endregion
	}
	#endregion
}
