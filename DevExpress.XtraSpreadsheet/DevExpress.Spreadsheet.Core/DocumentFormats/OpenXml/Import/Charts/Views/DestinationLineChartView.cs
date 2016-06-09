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
	#region LineChartViewDestinationBase
	public abstract class LineChartViewDestinationBase : ChartViewDestinationBase {
		#region Handler Table
		protected static void AddCommonHandlers(ElementHandlerTable<SpreadsheetMLBaseImporter> table) {
			table.Add("dLbls", OnDataLabels);
			table.Add("dropLines", OnDropLines);
			table.Add("grouping", OnGroupping);
			table.Add("ser", OnLineChartSeries);
			table.Add("varyColors", OnVaryColours);
		}
		static LineChartViewDestinationBase GetThis(SpreadsheetMLBaseImporter importer) {
			return (LineChartViewDestinationBase)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly ChartViewWithGroupingAndDropLines view;
		#endregion
		protected LineChartViewDestinationBase(SpreadsheetMLBaseImporter importer, ChartViewWithGroupingAndDropLines view, List<int> viewAxesList)
			: base(importer, view, viewAxesList) {
			this.view = view;
		}
		#region Handlers
		static Destination OnGroupping(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ChartViewWithGroupingAndDropLines view = GetThis(importer).view;
			return new EnumValueDestination<ChartGrouping>(importer,
				chartGroupingTable,
				delegate(ChartGrouping value) { view.Grouping = value; },
				"val",
				ChartGrouping.Standard);
		}
		static Destination OnDataLabels(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DataLabelsDestination(importer, GetThis(importer).view.DataLabels);
		}
		static Destination OnDropLines(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ChartViewWithGroupingAndDropLines view = GetThis(importer).view;
			view.ShowDropLines = true;
			return new InnerShapePropertiesDestination(importer, view.DropLinesProperties);
		}
		static Destination OnLineChartSeries(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ChartViewBase view = GetThis(importer).view;
			LineSeries series = new LineSeries(view);
			view.Series.AddCore(series);
			return new LineChartSeriesDestination(importer, series, view.Series.Count <= 1);
		}
		static Destination OnVaryColours(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ChartViewWithVaryColors view = GetThis(importer).view;
			return new OnOffValueDestination(importer,
				delegate(bool value) { view.VaryColors = value; },
				"val",
				true);
		}
		#endregion
	}
	#endregion
	#region LineChartViewDestination
	public class LineChartViewDestination : LineChartViewDestinationBase {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			AddAxisIdHandler(result);
			AddCommonHandlers(result);
			result.Add("hiLowLines", OnHiLowLines);
			result.Add("marker", OnMarker);
			result.Add("smooth", OnSmooth);
			result.Add("upDownBars", OnUpDownBars);
			return result;
		}
		static LineChartViewDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (LineChartViewDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly LineChartView view;
		#endregion
		public LineChartViewDestination(SpreadsheetMLBaseImporter importer, LineChartView view, List<int> viewAxesList)
			: base(importer, view, viewAxesList) {
			this.view = view;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		protected internal LineChartView View { get { return view; } }
		#endregion
		#region Handlers
		static Destination OnHiLowLines(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			LineChartView view = GetThis(importer).view;
			view.ShowHiLowLines = true;
			return new InnerShapePropertiesDestination(importer, view.HiLowLinesProperties);
		}
		static Destination OnMarker(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			LineChartView view = GetThis(importer).view;
			return new OnOffValueDestination(importer,
				delegate(bool value) { view.ShowMarker = value; },
				"val",
				true);
		}
		static Destination OnSmooth(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			LineChartView view = GetThis(importer).view;
			return new OnOffValueDestination(importer,
				delegate(bool value) { view.Smooth = value; },
				"val",
				true);
		}
		static Destination OnUpDownBars(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			LineChartView view = GetThis(importer).view;
			view.ShowUpDownBars = true;
			return new UpDownBarsDestination(importer, view.UpDownBars);
		}
		#endregion
	}
	#endregion
	#region Line3DChartViewDestination
	public class Line3DChartViewDestination : LineChartViewDestinationBase {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			AddAxisIdHandler(result);
			AddCommonHandlers(result);
			result.Add("gapDepth", OnGapDepth);
			return result;
		}
		static Line3DChartViewDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (Line3DChartViewDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly Line3DChartView view;
		#endregion
		public Line3DChartViewDestination(SpreadsheetMLBaseImporter importer, Line3DChartView view, List<int> viewAxesList)
			: base(importer, view, viewAxesList) {
			this.view = view;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		protected internal Line3DChartView View { get { return view; } }
		#endregion
		#region Handlers
		static Destination OnGapDepth(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Line3DChartView view = GetThis(importer).view;
			return new IntegerValueDestination(importer,
				delegate(int value) {
					if (value < 0 || value > 500)
						importer.ThrowInvalidFile();
					view.SetGapDepthCore(value);
				},
				"val",
				150);
		}
		#endregion
	}
	#endregion
}
