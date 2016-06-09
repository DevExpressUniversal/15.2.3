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

using System.Collections.Generic;
using System.Xml;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region TrendlineDestination
	public class TrendlineDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static
		internal static Dictionary<string, ChartTrendlineType> trendlineTypeTable = DictionaryUtils.CreateBackTranslationTable(DevExpress.XtraSpreadsheet.Export.OpenXml.OpenXmlExporter.TrendlineTypeTable);
		#endregion
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("spPr", OnShapeProperties);
			result.Add("name", OnName);
			result.Add("trendlineType", OnTrendlineType);
			result.Add("order", OnOrder);
			result.Add("period", OnPeriod);
			result.Add("forward", OnForward);
			result.Add("backward", OnBackward);
			result.Add("intercept", OnIntercept);
			result.Add("dispRSqr", OnDisplayRSquaredValue);
			result.Add("dispEq", OnDisplayEquation);
			result.Add("trendlineLbl", OnTrendlineLabel);
			return result;
		}
		static TrendlineDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (TrendlineDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly Trendline trendline;
		#endregion
		public TrendlineDestination(SpreadsheetMLBaseImporter importer, Trendline trendline)
			: base(importer) {
			this.trendline = trendline;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnShapeProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ChartShapePropertiesDestination(importer, GetThis(importer).trendline.ShapeProperties);
		}
		static Destination OnName(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Trendline trendline = GetThis(importer).trendline;
			trendline.HasCustomName = true;
			return new StringValueTagDestination(importer, delegate(string value) { trendline.SetNameCore(value); return true; });
		}
		static Destination OnTrendlineType(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Trendline trendline = GetThis(importer).trendline;
			return new EnumValueDestination<ChartTrendlineType>(importer,
				trendlineTypeTable,
				delegate(ChartTrendlineType value) { trendline.TrendlineType = value; },
				"val",
				ChartTrendlineType.Linear);
		}
		static Destination OnOrder(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Trendline trendline = GetThis(importer).trendline;
			return new IntegerValueDestination(importer,
				delegate(int value) {
					if (value < 2 || value > 6)
						importer.ThrowInvalidFile();
					trendline.Order = value;
				},
				"val", 2);
		}
		static Destination OnPeriod(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Trendline trendline = GetThis(importer).trendline;
			return new IntegerValueDestination(importer,
				delegate(int value) {
					if (value < 2 || value > 255)
						importer.ThrowInvalidFile();
					trendline.Period = value;
				},
				"val", 2);
		}
		static Destination OnForward(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Trendline trendline = GetThis(importer).trendline;
			return new FloatValueDestination(importer,
				delegate(float value) { trendline.Forward = value; },
				"val");
		}
		static Destination OnBackward(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Trendline trendline = GetThis(importer).trendline;
			return new FloatValueDestination(importer,
				delegate(float value) { trendline.Backward = value; },
				"val");
		}
		static Destination OnIntercept(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Trendline trendline = GetThis(importer).trendline;
			trendline.HasIntercept = true;
			return new FloatValueDestination(importer,
				delegate(float value) { trendline.Intercept = value; },
				"val");
		}
		static Destination OnDisplayRSquaredValue(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Trendline trendline = GetThis(importer).trendline;
			return new OnOffValueDestination(importer,
				delegate(bool value) { trendline.DisplayRSquare = value; },
				"val", true);
		}
		static Destination OnDisplayEquation(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Trendline trendline = GetThis(importer).trendline;
			return new OnOffValueDestination(importer,
				delegate(bool value) { trendline.DisplayEquation = value; },
				"val", true);
		}
		static Destination OnTrendlineLabel(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new TrendlineLabelDestination(importer, GetThis(importer).trendline.Label);
		}
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			trendline.BeginUpdate();
		}
		public override void ProcessElementClose(XmlReader reader) {
			trendline.EndUpdate();
			base.ProcessElementClose(reader);
		}
	}
	#endregion
	#region TrendlineLabelDestination
	public class TrendlineLabelDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("spPr", OnShapeProperties);
			result.Add("layout", OnLayout);
			result.Add("tx", OnChartText);
			result.Add("numFmt", OnNumberFormat);
			result.Add("txPr", OnTextProperties);
			return result;
		}
		static TrendlineLabelDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (TrendlineLabelDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly TrendlineLabel label;
		#endregion
		public TrendlineLabelDestination(SpreadsheetMLBaseImporter importer, TrendlineLabel label)
			: base(importer) {
			this.label = label;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnShapeProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ChartShapePropertiesDestination(importer, GetThis(importer).label.ShapeProperties);
		}
		static Destination OnLayout(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new LayoutDestination(importer, GetThis(importer).label.Layout);
		}
		static Destination OnChartText(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			TrendlineLabel label = GetThis(importer).label;
			return new ChartTextDestination(importer, label.Parent, delegate(IChartText value) { label.SetTextCore(value); });
		}
		static Destination OnNumberFormat(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			TrendlineLabel label = GetThis(importer).label;
			return new ChartNumberFormatDestination(importer, label.NumberFormat);
		}
		static Destination OnTextProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			TrendlineLabel label = GetThis(importer).label;
			return new TextPropertiesDestination(importer, label.TextProperties);
		}
		#endregion
	}
	#endregion
}
