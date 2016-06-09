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
using System.Xml;
using System.Globalization;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Export.OpenXml;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region BarChartViewDestinationBase
	public abstract class BarChartViewDestinationBase : ChartViewDestinationBase {
		#region Static
		internal static Dictionary<string, BarChartDirection> barChartDirectionTable = DictionaryUtils.CreateBackTranslationTable(OpenXmlExporter.BarChartDirectionTable);
		internal static Dictionary<string, BarChartGrouping> barChartGroupingTable = DictionaryUtils.CreateBackTranslationTable(OpenXmlExporter.BarChartGroupingTable);
		#endregion
		#region Handler Table
		protected static void AddCommonHadlers(ElementHandlerTable<SpreadsheetMLBaseImporter> table) {
			table.Add("gapWidth", OnGapWidth);
			table.Add("barDir", OnBarDirection);
			table.Add("grouping", OnBarGroupping);
			table.Add("dLbls", OnDataLabels);
			table.Add("ser", OnBarChartSeries);
			table.Add("varyColors", OnVaryColours);
		}
		static BarChartViewDestinationBase GetThis(SpreadsheetMLBaseImporter importer) {
			return (BarChartViewDestinationBase)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly BarChartViewBase view;
		#endregion
		protected BarChartViewDestinationBase(SpreadsheetMLBaseImporter importer, BarChartViewBase view, List<int> viewAxesList)
			: base(importer, view, viewAxesList) {
			this.view = view;
		}
		#region Handlers
		static Destination OnGapWidth(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			BarChartViewBase view = GetThis(importer).view;
			return new IntegerValueDestination(importer,
				delegate(int value) {
					if (value < 0 || value > 500)
						importer.ThrowInvalidFile();
					view.SetGapWidthCore(value);
				},
				"val",
				150);
		}
		static Destination OnBarDirection(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			BarChartViewBase view = GetThis(importer).view;
			return new EnumValueDestination<BarChartDirection>(importer,
				barChartDirectionTable,
				delegate(BarChartDirection value) { view.BarDirection = value; },
				"val",
				BarChartDirection.Column);
		}
		static Destination OnBarGroupping(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			BarChartViewBase view = GetThis(importer).view;
			return new EnumValueDestination<BarChartGrouping>(importer,
				barChartGroupingTable,
				delegate(BarChartGrouping value) { view.Grouping = value; },
				"val",
				BarChartGrouping.Clustered);
		}
		static Destination OnDataLabels(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DataLabelsDestination(importer, GetThis(importer).view.DataLabels);
		}
		static Destination OnBarChartSeries(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			BarChartViewBase view = GetThis(importer).view;
			BarSeries series = new BarSeries(view);
			view.Series.AddCore(series);
			return new BarChartSeriesDestination(importer, series, view.Series.Count <= 1);
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
	#region BarChartViewDestination
	public class BarChartViewDestination : BarChartViewDestinationBase {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			AddAxisIdHandler(result);
			AddCommonHadlers(result);
			result.Add("overlap", OnOverlap);
			result.Add("serLines", OnSeriesLines);
			return result;
		}
		static BarChartViewDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (BarChartViewDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly BarChartView view;
		#endregion
		public BarChartViewDestination(SpreadsheetMLBaseImporter importer, BarChartView view, List<int> viewAxesList)
			: base(importer, view, viewAxesList) {
			this.view = view;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnOverlap(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			BarChartView view = GetThis(importer).view;
			return new IntegerValueDestination(importer,
				delegate(int value) {
					if (value < -100 || value > 100)
						importer.ThrowInvalidFile();
					view.SetOverlapCore(value);
				},
				"val",
				150);
		}
		static Destination OnSeriesLines(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			SeriesLinesCollection seriesLines = GetThis(importer).view.SeriesLines;
			ShapeProperties shapeProperties = new ShapeProperties(importer.DocumentModel);
			shapeProperties.Parent = seriesLines.Parent;
			seriesLines.AddCore(shapeProperties);
			return new InnerShapePropertiesDestination(importer, shapeProperties);
		}
		#endregion
	}
	#endregion
	#region Bar3DChartViewDestination
	public class Bar3DChartViewDestination : BarChartViewDestinationBase {
		static Dictionary<string, BarShape> shapeTable = DictionaryUtils.CreateBackTranslationTable<BarShape>(OpenXmlExporter.BarChartShapeTable);
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			AddAxisIdHandler(result);
			AddCommonHadlers(result);
			result.Add("gapDepth", OnGapDepth);
			result.Add("shape", OnShape);
			return result;
		}
		static Bar3DChartViewDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (Bar3DChartViewDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly Bar3DChartView view;
		#endregion
		public Bar3DChartViewDestination(SpreadsheetMLBaseImporter importer, Bar3DChartView view, List<int> viewAxesList)
			: base(importer, view, viewAxesList) {
			this.view = view;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		protected internal Bar3DChartView View { get { return view; } }
		#endregion
		#region Handlers
		static Destination OnGapDepth(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Bar3DChartView view = GetThis(importer).view;
			return new IntegerValueDestination(importer,
				delegate(int value) {
					if (value < 0 || value > 500)
						importer.ThrowInvalidFile();
					view.SetGapDepthCore(value);
				},
				"val",
				150);
		}
		static Destination OnShape(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Bar3DChartView view = GetThis(importer).view;
			return new EnumValueDestination<BarShape>(importer,
				shapeTable,
				delegate(BarShape value) { view.Shape = value; },
				"val",
				BarShape.Box);
		}
		#endregion
	}
	#endregion
}
