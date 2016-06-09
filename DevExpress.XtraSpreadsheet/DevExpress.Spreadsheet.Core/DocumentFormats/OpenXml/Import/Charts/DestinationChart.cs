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
	#region ChartDestination
	public class ChartDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		static Dictionary<string, DisplayBlanksAs> displayBlanksAsTable = DictionaryUtils.CreateBackTranslationTable(DevExpress.XtraSpreadsheet.Export.OpenXml.OpenXmlExporter.DisplayBlanksAsTable);
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("title", OnTitle);
			result.Add("autoTitleDeleted", OnAutoTitleDeleted);
			result.Add("view3D", OnView3D);
			result.Add("floor", OnFloor);
			result.Add("sideWall", OnSideWall);
			result.Add("backWall", OnBackWall);
			result.Add("plotArea", OnPlotArea);
			result.Add("legend", OnLegend);
			result.Add("plotVisOnly", OnPlotVisOnly);
			result.Add("dispBlanksAs", OnDispBlanksAs);
			result.Add("showDLblsOverMax", OnShowDlblsOverMax);
			result.Add("pivotFmts", OnPivotFormats);
			return result;
		}
		static ChartDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ChartDestination)importer.PeekDestination();
		}
		#endregion
		readonly Chart chart;
		public ChartDestination(SpreadsheetMLBaseImporter importer, Chart chart)
			: base(importer) {
			this.chart = chart;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		public override void ProcessElementClose(XmlReader reader) {
			base.ProcessElementClose(reader);
			chart.UpdateDataReferences();
		}
		#region Handlers
		static Destination OnTitle(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ChartTitleDestination(importer, GetThis(importer).chart.Title);
		}
		static Destination OnAutoTitleDeleted(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Chart chart = GetThis(importer).chart;
			return new OnOffValueDestination(importer,
				delegate(bool value) { chart.AutoTitleDeleted = value; },
				"val", true);
		}
		static Destination OnView3D(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Chart chart = GetThis(importer).chart;
			return new View3DDestination(importer, chart.View3D);
		}
		static Destination OnFloor(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Chart chart = GetThis(importer).chart;
			return new SurfaceDestination(importer, chart.Floor);
		}
		static Destination OnSideWall(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Chart chart = GetThis(importer).chart;
			return new SurfaceDestination(importer, chart.SideWall);
		}
		static Destination OnBackWall(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Chart chart = GetThis(importer).chart;
			return new SurfaceDestination(importer, chart.BackWall);
		}
		static Destination OnPlotArea(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Chart chart = GetThis(importer).chart;
			return new PlotAreaDestination(importer, chart);
		}
		static Destination OnLegend(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Chart chart = GetThis(importer).chart;
			return new LegendDestination(importer, chart.Legend);
		}
		static Destination OnPlotVisOnly(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Chart chart = GetThis(importer).chart;
			return new OnOffValueDestination(importer,
				delegate(bool value) { chart.PlotVisibleOnly = value; },
				"val", true);
		}
		static Destination OnDispBlanksAs(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Chart chart = GetThis(importer).chart;
			return new EnumValueDestination<DisplayBlanksAs>(importer,
				displayBlanksAsTable,
				delegate(DisplayBlanksAs value) { chart.DispBlanksAs = value; },
				"val",
				DisplayBlanksAs.Zero);
		}
		static Destination OnShowDlblsOverMax(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Chart chart = GetThis(importer).chart;
			return new OnOffValueDestination(importer,
				delegate(bool value) { chart.ShowDataLabelsOverMax = value; },
				"val", true);
		}
		static Destination OnPivotFormats(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Chart chart = GetThis(importer).chart;
			return new PivotFormatsDestination(importer, chart.PivotFormats);
		}
		#endregion
	}
	#endregion
	#region PivotFormatsDestination
	public class PivotFormatsDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("pivotFmt", OnPivotFormat);
			return result;
		}
		static PivotFormatsDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotFormatsDestination)importer.PeekDestination();
		}
		readonly ChartPivotFormatCollection pivotFormats;
		public PivotFormatsDestination(SpreadsheetMLBaseImporter importer, ChartPivotFormatCollection pivotFormats)
			: base(importer) {
			this.pivotFormats = pivotFormats;
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		static Destination OnPivotFormat(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new PivotFormatDestination(importer, GetThis(importer).pivotFormats);
		}
	}
	#endregion
	#region PivotFormatDestination
	public class PivotFormatDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("idx", OnId);
			result.Add("spPr", OnShapeProperties);
			result.Add("txPr", OnTextProperties);
			result.Add("marker", OnMarker);
			result.Add("dLbl", OnDataLabel);
			return result;
		}
		static PivotFormatDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotFormatDestination)importer.PeekDestination();
		}
		#endregion
		readonly ChartPivotFormatCollection pivotFormats;
		public PivotFormatDestination(SpreadsheetMLBaseImporter importer, ChartPivotFormatCollection pivotFormats)
			: base(importer) {
			this.pivotFormats = pivotFormats;
		}
		protected internal ChartPivotFormat PivotFormat { get; set; }
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementClose(XmlReader reader) {
			pivotFormats.AddCore(PivotFormat);
		}
		static Destination OnId(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotFormatDestination destination = GetThis(importer);
			return new IntegerValueDestination(importer,
				delegate(int value) { destination.PivotFormat = new ChartPivotFormat(destination.pivotFormats.Parent, value); },
				"val", -1);
		}
		static Destination OnShapeProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ChartShapePropertiesDestination(importer, GetThis(importer).PivotFormat.ShapeProperties);
		}
		static Destination OnTextProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new TextPropertiesDestination(importer, GetThis(importer).PivotFormat.TextProperties);
		}
		static Destination OnMarker(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ChartMarkerDestination(importer, GetThis(importer).PivotFormat.Marker);
		}
		static Destination OnDataLabel(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DataLabelDestination(importer, GetThis(importer).PivotFormat.DataLabel);
		}
	}
	#endregion
	#region PivotSourceDestination
	public class PivotSourceDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("fmtId", OnFmtId);
			result.Add("name", OnName);
			return result;
		}
		static PivotSourceDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotSourceDestination)importer.PeekDestination();
		}
		#endregion
		string name;
		int fmtId;
		Chart chart;
		public PivotSourceDestination(SpreadsheetMLBaseImporter importer, Chart chart)
			: base(importer) {
			this.chart = chart;
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementClose(XmlReader reader) {
			ChartPivotSource source = new ChartPivotSource(fmtId, name);
			chart.SetPivotSourceCore(source);
		}
		static Destination OnFmtId(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotSourceDestination destination = GetThis(importer);
			return new IntegerValueDestination(importer,
				delegate(int value) { destination.fmtId = value; },
				"val", -1);
		}
		static Destination OnName(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotSourceDestination destination = GetThis(importer);
			return new StringValueTagDestination(importer, delegate(string value) { destination.name = value; return true; });
		}
	}
	#endregion
}
