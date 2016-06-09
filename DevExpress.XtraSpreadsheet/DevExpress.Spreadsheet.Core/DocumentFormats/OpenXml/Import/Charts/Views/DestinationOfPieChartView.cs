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
using DevExpress.XtraSpreadsheet.Export.OpenXml;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using System.Collections.Generic;
using System.Xml;
using DevExpress.Office.Utils;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region OfPieChartViewDestination
	public class OfPieChartViewDestination : PieChartViewDestinationBase {
		internal static Dictionary<string, ChartOfPieType> chartOfPieTypeTable = DictionaryUtils.CreateBackTranslationTable(OpenXmlExporter.ChartOfPieTypeTable);
		internal static Dictionary<string, OfPieSplitType> ofPieSplitTypeTable = DictionaryUtils.CreateBackTranslationTable(OpenXmlExporter.OfPieSplitTypeTable);
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("ofPieType", OnOfPieType);
			AddCommonHandlers(result);
			result.Add("gapWidth", OnGapWidth);
			result.Add("splitType", OnSplitType);
			result.Add("splitPos", OnSplitPosition);
			result.Add("custSplit", OnCustomSplit);
			result.Add("secondPieSize", OnSecondPieSize);
			result.Add("serLines", OnSeriesLines);
			return result;
		}
		static OfPieChartViewDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (OfPieChartViewDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly OfPieChartView view;
		#endregion
		public OfPieChartViewDestination(SpreadsheetMLBaseImporter importer, OfPieChartView view, List<int> viewAxesList)
			: base(importer, view, viewAxesList) {
			this.view = view;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnCustomSplit(SpreadsheetMLBaseImporter importer, XmlReader reader) { 
			return new OfPieCustomSplitDestination(importer, GetThis(importer).view.SecondPiePoints);
		}
		static Destination OnGapWidth(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			OfPieChartView view = GetThis(importer).view;
			return new IntegerValueDestination(importer,
				delegate(int value) {
					if (value < 0 || value > 500)
						importer.ThrowInvalidFile();
					view.SetGapWidthCore(value);
				},
				"val", 150);
		}
		static Destination OnOfPieType(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			OfPieChartView view = GetThis(importer).view;
			return new EnumValueDestination<ChartOfPieType>(importer,
				chartOfPieTypeTable,
				delegate(ChartOfPieType value) { view.OfPieType = value; },
				"val", ChartOfPieType.Pie);
		}
		static Destination OnSecondPieSize(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			OfPieChartView view = GetThis(importer).view;
			return new IntegerValueDestination(importer,
				delegate(int value) {
					if (value < 5 || value > 200)
						importer.ThrowInvalidFile();
					view.SetSecondPieSizeCore(value);
				},
				"val", 75);
		}
		static Destination OnSeriesLines(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			SeriesLinesCollection seriesLines = GetThis(importer).view.SeriesLines;
			ShapeProperties shapeProperties = new ShapeProperties(importer.DocumentModel);
			shapeProperties.Parent = seriesLines.Parent;
			seriesLines.AddCore(shapeProperties);
			return new InnerShapePropertiesDestination(importer, shapeProperties);
		}
		static Destination OnSplitPosition(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			OfPieChartView view = GetThis(importer).view;
			return new FloatValueDestination(importer,
				delegate(float value) { view.SetSplitPosCore(value); },
				"val");
		}
		static Destination OnSplitType(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			OfPieChartView view = GetThis(importer).view;
			return new EnumValueDestination<OfPieSplitType>(importer,
				ofPieSplitTypeTable,
				delegate(OfPieSplitType value) { view.SetSplitTypeCore(value); },
				"val", OfPieSplitType.Auto);
		}
		#endregion
	}
	#endregion
	#region OfPieCustomSplitDestination
	public class OfPieCustomSplitDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("secondPiePt", OnSecondPiePoint);
			return result;
		}
		static OfPieCustomSplitDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (OfPieCustomSplitDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly SecondPiePointCollection collection;
		#endregion
		public OfPieCustomSplitDestination(SpreadsheetMLBaseImporter importer, SecondPiePointCollection collection)
			: base(importer) {
			this.collection = collection;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnSecondPiePoint(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			SecondPiePointCollection collection = GetThis(importer).collection;
			return new IntegerValueDestination(importer,
				delegate(int value) {
					if (value < 0)
						importer.ThrowInvalidFile();
					collection.AddCore(value);
				},
				"val", -1);
		}
		#endregion
	}
	#endregion
}
