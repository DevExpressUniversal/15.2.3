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
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region ChartSpaceDestination
	public class ChartSpaceDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("chart", OnChart);
			result.Add("clrMapOvr", OnColorMapOverride);
			result.Add("date1904", OnDate1904);
			result.Add("externalData", OnExternalData);
			result.Add("lang", OnLang);
			result.Add("pivotSource", OnPivotSource);
			result.Add("printSettings", OnPrintSettings);
			result.Add("protection", OnProtection);
			result.Add("roundedCorners", OnRoundedCorners);
			result.Add("spPr", OnShapeProperties);
			result.Add("txPr", OnTextProperties);
			result.Add("style", OnStyle);
			result.Add("userShapes", OnUserShapes);
			result.Add("AlternateContent", OnAlternateContent);
			return result;
		}
		static ChartSpaceDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ChartSpaceDestination)importer.PeekDestination();
		}
		#endregion
		readonly Chart chart;
		public ChartSpaceDestination(SpreadsheetMLBaseImporter importer, Chart chart)
			: base(importer) {
				this.chart = chart;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnChart(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ChartDestination(importer, GetThis(importer).chart);
		}
		static Destination OnColorMapOverride(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ColorMapOverrideDestination(importer, GetThis(importer).chart.ColorMapOverride);
		}
		static Destination OnDate1904(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Chart chart = GetThis(importer).chart;
			return new OnOffValueDestination(importer, 
				delegate(bool value) { chart.Date1904 = value; }, 
				"val", true);
		}
		static Destination OnExternalData(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new EmptyDestination<SpreadsheetMLBaseImporter>(importer);
		}
		static Destination OnLang(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Chart chart = GetThis(importer).chart;
			return new StringValueDestination(importer,
				delegate(string value) { chart.Culture = new CultureInfo(value); },
				"val");
		}
		static Destination OnPivotSource(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new PivotSourceDestination(importer, GetThis(importer).chart);
		}
		static Destination OnPrintSettings(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new PrintSettingsDestination(importer, GetThis(importer).chart.PrintSettings);
		}
		static Destination OnProtection(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ChartSpaceProtectionDestination(importer, GetThis(importer).chart);
		}
		static Destination OnRoundedCorners(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Chart chart = GetThis(importer).chart;
			return new OnOffValueDestination(importer,
				delegate(bool value) { chart.RoundedCorners = value; },
				"val", true);
		}
		static Destination OnShapeProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ChartShapePropertiesDestination(importer, GetThis(importer).chart.ShapeProperties);
		}
		static Destination OnTextProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Chart chart = GetThis(importer).chart;
			return new TextPropertiesDestination(importer, chart.TextProperties);
		}
		static Destination OnStyle(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Chart chart = GetThis(importer).chart;
			return new IntegerValueDestination(importer,
				delegate(int value) { chart.Style = value; },
				"val", 2);
		}
		static Destination OnUserShapes(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new EmptyDestination<SpreadsheetMLBaseImporter>(importer);
		}
		static Destination OnAlternateContent(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ChartSpaceAlternateContentDestination(importer, GetThis(importer).chart);
		}
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			this.chart.BeginUpdate();
		}
		public override void ProcessElementClose(XmlReader reader) {
			this.chart.EndUpdate();
		}
	}
	#endregion
	#region ChartSpaceAlternateContentDestination
	public class ChartSpaceAlternateContentDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("Choice", OnChoice);
			result.Add("style", OnStyle);
			return result;
		}
		static ChartSpaceAlternateContentDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ChartSpaceAlternateContentDestination)importer.PeekDestination();
		}
		#endregion
		readonly Chart chart;
		public ChartSpaceAlternateContentDestination(SpreadsheetMLBaseImporter importer, Chart chart)
			: base(importer) {
				this.chart = chart;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnChoice(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return GetThis(importer);
		}
		static Destination OnStyle(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Chart chart = GetThis(importer).chart;
			return new IntegerValueDestination(importer,
				delegate(int value) { chart.Style = value - 100; },
				"val", 102);
		}
		#endregion
	}
	#endregion
	#region ChartSpaceProtectionDestination
	public class ChartSpaceProtectionDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("chartObject", OnChartObject);
			result.Add("data", OnData);
			result.Add("formatting", OnFormatting);
			result.Add("selection", OnSelection);
			result.Add("userInterface", OnUserInterface);
			return result;
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811")]
		static ChartSpaceProtectionDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ChartSpaceProtectionDestination)importer.PeekDestination();
		}
		#endregion
		readonly Chart chart;
		ChartSpaceProtection protection;
		public ChartSpaceProtectionDestination(SpreadsheetMLBaseImporter importer, Chart chart)
			: base(importer) {
			this.chart = chart;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnChartObject(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ChartSpaceProtectionDestination thisDestination = GetThis(importer);
			return new OnOffValueDestination(importer,
				delegate(bool value) { thisDestination.SetProtection(ChartSpaceProtection.ChartObject, value); },
				"val", true);
		}
		static Destination OnData(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ChartSpaceProtectionDestination thisDestination = GetThis(importer);
			return new OnOffValueDestination(importer,
				delegate(bool value) { thisDestination.SetProtection(ChartSpaceProtection.Data, value); },
				"val", true);
		}
		static Destination OnFormatting(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ChartSpaceProtectionDestination thisDestination = GetThis(importer);
			return new OnOffValueDestination(importer,
				delegate(bool value) { thisDestination.SetProtection(ChartSpaceProtection.Formatting, value); },
				"val", true);
		}
		static Destination OnSelection(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ChartSpaceProtectionDestination thisDestination = GetThis(importer);
			return new OnOffValueDestination(importer,
				delegate(bool value) { thisDestination.SetProtection(ChartSpaceProtection.Selection, value); },
				"val", true);
		}
		static Destination OnUserInterface(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ChartSpaceProtectionDestination thisDestination = GetThis(importer);
			return new OnOffValueDestination(importer,
				delegate(bool value) { thisDestination.SetProtection(ChartSpaceProtection.UserInterface, value); },
				"val", true);
		}
		#endregion
		void SetProtection(ChartSpaceProtection mask, bool flag) {
			if(flag) { this.protection |= mask; } else { this.protection &= ~mask; }
		}
		public override void ProcessElementOpen(XmlReader reader) {
			this.protection = ChartSpaceProtection.None;
		}
		public override void ProcessElementClose(XmlReader reader) {
			this.chart.Protection = this.protection;
		}
	}
	#endregion
}
