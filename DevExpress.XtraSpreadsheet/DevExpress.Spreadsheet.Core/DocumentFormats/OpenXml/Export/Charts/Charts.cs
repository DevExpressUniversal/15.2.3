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
using DevExpress.Utils.Zip;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraSpreadsheet.Import.OpenXml;
using DevExpress.XtraSpreadsheet.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using DevExpress.Office.Drawing;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		#region Static
		internal static Dictionary<DisplayBlanksAs, string> DisplayBlanksAsTable = CreateDisplayBlanksAsTable();
		internal static Dictionary<ColorSchemeIndex, string> ColorSchemeIndexTable = CreateColorSchemeIndexTable();
		static Dictionary<DisplayBlanksAs, string> CreateDisplayBlanksAsTable() {
			Dictionary<DisplayBlanksAs, string> result = new Dictionary<DisplayBlanksAs, string>();
			result.Add(DisplayBlanksAs.Gap, "gap");
			result.Add(DisplayBlanksAs.Span, "span");
			result.Add(DisplayBlanksAs.Zero, "zero");
			return result;
		}
		static Dictionary<ColorSchemeIndex, string> CreateColorSchemeIndexTable() {
			Dictionary<ColorSchemeIndex, string> result = new Dictionary<ColorSchemeIndex, string>();
			result.Add(ColorSchemeIndex.Accent1, "accent1");
			result.Add(ColorSchemeIndex.Accent2, "accent2");
			result.Add(ColorSchemeIndex.Accent3, "accent3");
			result.Add(ColorSchemeIndex.Accent4, "accent4");
			result.Add(ColorSchemeIndex.Accent5, "accent5");
			result.Add(ColorSchemeIndex.Accent6, "accent6");
			result.Add(ColorSchemeIndex.Dark1, "dk1");
			result.Add(ColorSchemeIndex.Dark2, "dk2");
			result.Add(ColorSchemeIndex.Light1, "lt1");
			result.Add(ColorSchemeIndex.Light2, "lt2");
			result.Add(ColorSchemeIndex.Hyperlink, "hlink");
			result.Add(ColorSchemeIndex.FollowedHyperlink, "folHlink");
			return result;
		}
		#endregion
		#region Fields
		int chartCounter;
		Chart currentChart;
		bool shouldExportCharts = true;
		#endregion
		public bool ShouldExportCharts { get { return shouldExportCharts && Workbook.DocumentCapabilities.ChartsAllowed; } }
		#region Reference from drawings
		protected internal virtual void CreateDrawingChartContent(Chart chart) {
			WriteStartElement("graphicFrame", SpreadsheetDrawingNamespace);
			try {
				if (chart.GraphicFrameInfo.IsPublished)
					WriteBoolValue("fPublished", true);
				WriteStringValue("macro", chart.GraphicFrameInfo.Macro ?? string.Empty);
				ExportGraphicFrameProperties(chart.DrawingObject.Properties);
				ExportXfrmForGraphicFrames(chart.DrawingObject.Transform2D);
				ExportGraphicElement(chart);
			}
			finally {
				WriteEndElement();
			}
		}
		void ExportXfrmForGraphicFrames(Transform2D transform2D) {
			WriteStartElement("xfrm", SpreadsheetDrawingNamespace);
			try {
				ExportXfrmCore(transform2D);
			}
			finally {
				WriteEndElement();
			}
		}
		#region Graphic frame properties
		void ExportGraphicFrameProperties(IDrawingObjectNonVisualProperties properties) {
			WriteStartElement("nvGraphicFramePr", SpreadsheetDrawingNamespace);
			try {
				ExportNonVisualDrawingProperties(properties);
				ExportNonVisualGraphicFrameProperties(properties);
			}
			finally {
				WriteShEndElement();
			}
		}
		void ExportNonVisualGraphicFrameProperties(IDrawingObjectNonVisualProperties properties) {
			WriteStartElement("cNvGraphicFramePr", SpreadsheetDrawingNamespace);
			WriteShEndElement();
		}
		#endregion
		void ExportGraphicElement(Chart chart) {
			WriteStartElement("graphic", DrawingMLNamespace);
			try {
				ExportGraphicDataElement(chart);
			}
			finally {
				WriteShEndElement();
			}
		}
		void ExportGraphicDataElement(Chart chart) {
			WriteStartElement("graphicData", DrawingMLNamespace);
			try {
				WriteStringAttr(null, "uri", null, DrawingMLChartNamespace);
				ExportChartReference(chart);
			}
			finally {
				WriteShEndElement();
			}
		}
		void ExportChartReference(Chart chart) {
			WriteStartElement("c", "chart", DrawingMLChartNamespace);
			try {
				GenerateChartNamespaceAttributes();
				string id = PopulateChartsTable(chart);
				WriteStringAttr(RelsPrefix, "id", null, id);
			}
			finally {
				WriteShEndElement();
			}
		}
		public void GenerateChartNamespaceAttributes() {
			WriteStringAttr("xmlns", "c", null, DrawingMLChartNamespace);
			WriteStringAttr("xmlns", "r", null, RelsNamespace); 
		}
		protected internal virtual bool ShouldExportChart(Chart chart) {
			return shouldExportCharts;
		}
		protected internal virtual string PopulateChartsTable(Chart chart) {
			chartCounter++;
			string fileName = String.Format("chart{0}.xml", chartCounter);
			OpenXmlRelationCollection relations = currentRelations;
			string id = GenerateIdByCollection(relations);
			OpenXmlRelation relation = new OpenXmlRelation(id, "../charts/" + fileName, RelsChartNamepace);
			relations.Add(relation);
			chartPathsTable.Add(chart, fileName);
			Builder.OverriddenContentTypes.Add("/xl/charts/" + fileName, "application/vnd.openxmlformats-officedocument.drawingml.chart+xml");
			return id;
		}
		#endregion
		protected internal virtual void AddChartsPackageContent() {
			foreach (KeyValuePair<Chart, string> pair in ChartPathsTable) {
				ChartRelationPathTable.Add(pair.Value, "xl\\charts\\_rels\\" + pair.Value + ".rels");
				currentRelations = new OpenXmlRelationCollection();
				ChartRelationsTable.Add(pair.Value, currentRelations);
				AddPackageContent(@"xl\charts\" + pair.Value, ExportChart(pair.Key));
			}
		}
		protected internal virtual CompressedStream ExportChart(Chart chart) {
			this.currentChart = chart;
			return CreateXmlContent(GenerateChartXmlContent);
		}
		protected internal virtual void GenerateChartXmlContent(XmlWriter writer) {
			exportedImageTable.Clear();
			exportedExternalImageTable.Clear();
			exportedPictureHyperlinkTable.Clear();
			DocumentContentWriter = writer;
			GenerateChartSpace(currentChart);
		}
		#region Chart relations
		protected internal virtual void AddChartRelationsPackageContent() {
			foreach (KeyValuePair<string, string> valuePair in ChartRelationPathTable) {
				OpenXmlRelationCollection currentChartRelations = ChartRelationsTable[valuePair.Key];
					AddRelationsPackageContent(valuePair.Value, currentChartRelations);
			}
		}
		#endregion
		#region ChartSpace
		protected internal virtual void GenerateChartSpace(Chart chart) {
			WriteStartElement("c", "chartSpace", DrawingMLChartNamespace);
			try {
				GenerateChartNamespaceAttributes();
				WriteStringAttr("xmlns", "a", null, DrawingMLNamespace);
				if (!chart.Date1904)
					GenerateChartSimpleBoolAttributeTag("date1904", chart.Date1904); 
				GenerateChartSpaceLang(chart.Culture);
				if (!chart.RoundedCorners)
					GenerateChartSimpleBoolAttributeTag("roundedCorners", chart.RoundedCorners); 
				GenerateChartStyleAlternateContent(chart.Style);
				GenerateChartSpaceColorMap(chart.ColorMapOverride);
				GenerateChartSpacePivotSource(chart.PivotSource);
				GenerateChartSpaceProtection(chart.Protection);
				GenerateChartSpaceChart(chart);
				GenerateChartShapeProperties(chart.ShapeProperties);
				GenerateTextPropertiesContent(chart.TextProperties);
				GenerateChartSpacePrintSettings(chart.PrintSettings);
			}
			finally {
				WriteEndElement();
			}
		}
		protected internal virtual void GenerateChartSpaceLang(System.Globalization.CultureInfo chartCulture) {
			WriteStartElement("lang", DrawingMLChartNamespace);
			try {
				WriteStringValue("val", chartCulture.Name);
			}
			finally {
				WriteEndElement();
			}
		}
		protected internal virtual void GenerateChartStyleAlternateContent(int chartStyle) {
			WriteStartElement("mc", "AlternateContent", MarkupCompatibilityNamespace);
			try {
				WriteStringAttr("xmlns", "mc", null, MarkupCompatibilityNamespace);
				GenerateChartStyleChoice(chartStyle);
				GenerateChartStyleFallback(chartStyle);
			}
			finally {
				WriteEndElement();
			}
		}
		protected internal virtual void GenerateChartStyleChoice(int chartStyle) {
			WriteStartElement("mc", "Choice", MarkupCompatibilityNamespace);
			try {
				WriteStringAttr("xmlns", "c14", null, OfficeDrawingChart14Namespace);
				WriteStringAttr(null, "Requires", null, "c14");
				GenerateSimpleIntAttributeTag("style", chartStyle + 100, OfficeDrawingChart14Namespace);
			}
			finally {
				WriteEndElement();
			}
		}
		protected internal virtual void GenerateChartStyleFallback(int chartStyle) {
			WriteStartElement("mc", "Fallback", MarkupCompatibilityNamespace);
			try {
				GenerateChartSimpleIntAttributeTag("style", chartStyle);
			}
			finally {
				WriteEndElement();
			}
		}
		protected internal void GenerateChartSpaceColorMap(ColorMapOverride map) {
			if (!map.IsDefined)
				return;
			WriteStartElement("clrMapOvr", DrawingMLChartNamespace);
			try {
				WriteStringValue("bg1", ColorSchemeIndexTable[map.Background1]);
				WriteStringValue("tx1", ColorSchemeIndexTable[map.Text1]);
				WriteStringValue("bg2", ColorSchemeIndexTable[map.Background2]);
				WriteStringValue("tx2", ColorSchemeIndexTable[map.Text2]);
				WriteStringValue("accent1", ColorSchemeIndexTable[map.Accent1]);
				WriteStringValue("accent2", ColorSchemeIndexTable[map.Accent2]);
				WriteStringValue("accent3", ColorSchemeIndexTable[map.Accent3]);
				WriteStringValue("accent4", ColorSchemeIndexTable[map.Accent4]);
				WriteStringValue("accent5", ColorSchemeIndexTable[map.Accent5]);
				WriteStringValue("accent6", ColorSchemeIndexTable[map.Accent6]);
				WriteStringValue("hlink", ColorSchemeIndexTable[map.Hyperlink]);
				WriteStringValue("folHlink", ColorSchemeIndexTable[map.FollowedHyperlink]);
			}
			finally {
				WriteEndElement();
			}
		}
		protected internal void GenerateChartSpacePivotSource(ChartPivotSource pivotSource) {
			if (pivotSource == null)
				return;
			WriteStartElement("pivotSource", DrawingMLChartNamespace);
			try {
				GenerateChartSimpleStringTag("name", pivotSource.Name);
				GenerateChartSimpleIntAttributeTag("fmtId", pivotSource.FormatId);
			}
			finally {
				WriteEndElement();
			}
		}
		protected internal virtual void GenerateChartSpaceProtection(ChartSpaceProtection protection) {
			if (protection == ChartSpaceProtection.None)
				return;
			WriteStartElement("protection", DrawingMLChartNamespace);
			try {
				if ((protection & ChartSpaceProtection.ChartObject) != 0)
					GenerateChartSimpleBoolAttributeTag("chartObject", true);
				if ((protection & ChartSpaceProtection.Data) != 0)
					GenerateChartSimpleBoolAttributeTag("data", true);
				if ((protection & ChartSpaceProtection.Formatting) != 0)
					GenerateChartSimpleBoolAttributeTag("formatting", true);
				if ((protection & ChartSpaceProtection.Selection) != 0)
					GenerateChartSimpleBoolAttributeTag("selection", true);
				if ((protection & ChartSpaceProtection.UserInterface) != 0)
					GenerateChartSimpleBoolAttributeTag("userInterface", true);
			}
			finally {
				WriteEndElement();
			}
		}
		protected internal virtual void GenerateChartSpacePrintSettings(PrintSettings printSettings) {
			WriteStartElement("printSettings", DrawingMLChartNamespace);
			try {
				GenerateChartHeaderFooterContent(printSettings.HeaderFooter);
				GenerateChartPageMarginsContent(printSettings.PageMargins);
				GenerateChartPageSetupContent(printSettings.PageSetup);
			}
			finally {
				WriteEndElement();
			}
		}
		#region Chart
		protected internal virtual void GenerateChartSpaceChart(Chart chart) {
			WriteStartElement("chart", DrawingMLChartNamespace);
			try {
				GenerateChartTitleContent(chart.Title);
				GenerateChartSimpleBoolAttributeTag("autoTitleDeleted", chart.AutoTitleDeleted);
				GenerateChartPivotFormats(chart.PivotFormats);
				if (chart.Is3DChart) {
					GenerateChartView3DContent(chart.View3D);
					GenerateChartSurface("floor", chart.Floor);
					GenerateChartSurface("sideWall", chart.SideWall);
					GenerateChartSurface("backWall", chart.BackWall);
				}
				GenerateChartPlotArea(chart);
				GenerateChartLegend(chart.Legend);
				GenerateChartSimpleBoolAttributeTag("plotVisOnly", chart.PlotVisibleOnly);
				GenerateChartDisplayBlanksAs(chart.DispBlanksAs);
				GenerateChartSimpleBoolAttributeTag("showDLblsOverMax", chart.ShowDataLabelsOverMax);
			}
			finally {
				WriteEndElement();
			}
		}
		protected internal void GenerateChartDisplayBlanksAs(DisplayBlanksAs value) {
			if (value == DisplayBlanksAs.Zero)
				return;
			WriteStartElement("dispBlanksAs", DrawingMLChartNamespace);
			try {
				WriteStringValue("val", DisplayBlanksAsTable[value]);
			}
			finally {
				WriteEndElement();
			}
		}
		protected internal void GenerateChartSurface(string tag, SurfaceOptions options) {
			WriteStartElement(tag, DrawingMLChartNamespace);
			try {
				GenerateChartSimpleIntAttributeTag("thickness", options.Thickness);
				GenerateChartShapeProperties(options.ShapeProperties);
				if (options.ShapeProperties.Fill.FillType == DrawingFillType.Picture)
					GenerateChartPictureOptions(options.PictureOptions);
			}
			finally {
				WriteEndElement();
			}
		}
		#region Title
		protected internal void GenerateChartTitleContent(TitleOptions titleOptions) {
			if (!titleOptions.Visible)
				return;
			WriteStartElement("title", DrawingMLChartNamespace);
			try {
				GenerateChartTextContent(titleOptions.Text);
				GenerateChartLayoutContent(titleOptions.Layout);
				if (!titleOptions.Overlay)
					GenerateChartSimpleBoolAttributeTag("overlay", titleOptions.Overlay);
				GenerateChartShapeProperties(titleOptions.ShapeProperties);
				GenerateTextPropertiesContent(titleOptions.TextProperties);
			}
			finally {
				WriteEndElement();
			}
		}
		protected internal void GenerateChartTextContent(IChartText chartText) {
			if (chartText.TextType == ChartTextType.None || chartText.TextType == ChartTextType.Auto)
				return;
			WriteStartElement("tx", DrawingMLChartNamespace);
			try {
				ChartTextExportWalker exportWalker = new ChartTextExportWalker(this);
				chartText.Visit(exportWalker);
			}
			finally {
				WriteEndElement();
			}
		}
		protected internal void ExportRichChartText(ChartRichText value) {
			WriteStartElement("rich", DrawingMLChartNamespace);
			try {
				GenerateTextPropertiesContentCore(value.TextProperties);
			}
			finally {
				WriteEndElement();
			}
		}
		internal void ExportChartTextValue(ChartTextValue value) {
			GenerateChartDataStringValue(value.PlainText);
		}
		protected internal void ExportChartTextRef(ChartTextRef value) {
			OpenXmlStringReference reference = OpenXmlStringReference.FromChartTextRef(value);
			GenerateStringReference(reference);
		}
		internal void GenerateStringReference(OpenXmlStringReference reference) {
			WriteStartElement("strRef", DrawingMLChartNamespace);
			try {
				GenerateStringRefFormula(reference.FormulaBody);
				if (reference.Points.Count > 0)
					GenerateStringCache(reference.Points);
			}
			finally {
				WriteEndElement();
			}
		}
		internal void GenerateStringRefFormula(string value) {
			WriteStartElement("f", DrawingMLChartNamespace);
			try {
				WriteShString(value);
			}
			finally {
				WriteEndElement();
			}
		}
		internal void GenerateStringCache(IList<StringPoint> points) {
			WriteStartElement("strCache", DrawingMLChartNamespace);
			try {
				int pointCount = points == null ? 0 : points.Count;
				if (pointCount > 0) {
					int lastIndex = points[pointCount - 1].Index;
					GenerateChartSimpleIntAttributeTag("ptCount", lastIndex + 1);
					for (int i = 0; i < pointCount; i++)
						GenerateStrReferencePoint(points[i]);
				}
			}
			finally {
				WriteEndElement();
			}
		}
		internal void GenerateStrReferencePoint(StringPoint point) {
			WriteStartElement("pt", DrawingMLChartNamespace);
			try {
				WriteIntValue("idx", point.Index);
				GenerateChartDataStringValue(point.Value);
			}
			finally {
				WriteEndElement();
			}
		}
		internal void GenerateChartDataStringValue(string value) {
			WriteStartElement("v", DrawingMLChartNamespace);
			try {
				WriteShString(value);
			}
			finally {
				WriteEndElement();
			}
		}
		#endregion
		#region PivotFormats
		protected internal void GenerateChartPivotFormats(ChartPivotFormatCollection pivotFormats) {
			if (pivotFormats.Count == 0)
				return;
			WriteStartElement("pivotFmts", DrawingMLChartNamespace);
			try {
				for (int i = 0; i < pivotFormats.Count; ++i)
					GenerateChartPivotFormat(pivotFormats[i]);
			}
			finally {
				WriteEndElement();
			}
		}
		protected internal void GenerateChartPivotFormat(ChartPivotFormat pivotFormat) {
			WriteStartElement("pivotFmt", DrawingMLChartNamespace);
			try {
				GenerateChartSimpleIntAttributeTag("idx", pivotFormat.Idx);
				GenerateChartShapeProperties(pivotFormat.ShapeProperties);
				GenerateTextPropertiesContent(pivotFormat.TextProperties);
				GenerateMarker(pivotFormat.Marker);
				if (pivotFormat.DataLabel.ItemIndex >= 0)
					GenerateDataLabel(pivotFormat.DataLabel);
			}
			finally {
				WriteEndElement();
			}
		}
		#endregion
		#endregion
		#endregion
	}
	public class ChartTextExportWalker : IChartTextVisitor {
		readonly OpenXmlExporter exporter;
		public ChartTextExportWalker(OpenXmlExporter exporter) {
			this.exporter = exporter;
		}
		#region IChartTextVisitor Members
		public void Visit(ChartText value) {
		}
		public void Visit(ChartRichText value) {
			exporter.ExportRichChartText(value);
		}
		public void Visit(ChartTextRef value) {
			exporter.ExportChartTextRef(value);
		}
		public void Visit(ChartTextValue value) {
			exporter.ExportChartTextValue(value);
		}
		#endregion
	}
}
