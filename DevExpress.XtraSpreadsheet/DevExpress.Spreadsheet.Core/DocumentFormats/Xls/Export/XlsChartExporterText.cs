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
using DevExpress.Export.Xl;
using DevExpress.XtraExport.Xls;
using DevExpress.Office.DrawingML;
#if !SL
using System.Drawing;
using DevExpress.Office.Drawing;
using DevExpress.Office.Model;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Export.Xls {
	public partial class XlsChartExporter {
		#region WriteAttachedLabelForTitle Methods
		void WriteAttachedLabelForTitle(TitleOptions title, XlsChartTextObjectLinkType objectLinkType) {
			if (objectLinkType == XlsChartTextObjectLinkType.Chart && Chart.AutoTitleDeleted) {
				WriteDeletedTitle();
				return;
			}
			if (!title.Visible)
				return;
			ChartElement element = objectLinkType == XlsChartTextObjectLinkType.Chart ? ChartElement.ChartTitle : ChartElement.AxisTitle;
			ChartRichText richText = title.Text as ChartRichText;
			WriteText(title);
			WriteBegin();
			WritePosition(title.Layout);
			if (RichTextWithoutRuns(richText))
				WriteFontX(richText.Paragraphs, element);
			else
				WriteFontX(title.TextProperties.Paragraphs, element);
			WriteChartText(title.Text, element);
			WriteFrame(title.ShapeProperties, element);
			WriteObjectlink(objectLinkType);
			WriteCrtLayout12(title.Layout);
			WriteAttachedLabelExtProperties(title.Overlay);
			WriteEnd();
		}
		void WriteDeletedTitle() {
			WriteDeletedTitleText();
			WriteBegin();
			WritePosition(null);
			WriteBRAI();
			WriteObjectlink(XlsChartTextObjectLinkType.Chart);
			WriteEnd();
		}
		void WriteDeletedTitleText() {
			XlsCommandChartText command = new XlsCommandChartText();
			command.HorizontalAlignment = DrawingTextAlignmentType.Center;
			command.VerticalAlignment = DrawingTextAnchoringType.Center;
			command.IsTransparent = true;
			command.AutoText = true;
			command.IsGenerated = true;
			command.Deleted = true;
			command.TextColorIndex = 8;
			command.Write(StreamWriter);
		}
		void WriteChartText(IChartText text, ChartElement element) {
			if (text.TextType == ChartTextType.Auto)
				WriteBRAI();
			ChartTextExportWalker walker = new ChartTextExportWalker(this, element);
			text.Visit(walker);
		}
		protected internal void WriteRichChartText(ChartRichText text, ChartElement element) {
			if (!RichTextWithoutRuns(text))
				WriteAIRuns(text.Paragraphs, element);
			WriteBRAI();
			WriteSeriesText(text.PlainText);
		}
		protected internal void WriteChartTextRef(ChartTextRef text) {
			WriteReferenceBRAI(text.CachedValue.InlineTextValue, text.Expression);
			WriteSeriesText(text.PlainText);
		}
		void WriteFontX(DrawingTextParagraphCollection paragraphs, ChartElement element) {
			XlsCommandChartFontX command = PrepareFontXCommand(paragraphs, element);
			command.Write(StreamWriter);
		}
		XlsCommandChartFontX PrepareFontXCommand(DrawingTextParagraphCollection paragraphs, ChartElement element) {
			Palette palette = Chart.DocumentModel.StyleSheet.Palette;
			RunFontInfo fontInfo;
			if (paragraphs.Count > 0) {
				DrawingTextParagraph firstParagraph = paragraphs[0];
				DrawingTextRunCollection runs = firstParagraph.Runs;
				DrawingTextParagraphProperties paragraphProperties = firstParagraph.ParagraphProperties;
				if (runs.Count > 0) {
					DrawingTextCharacterProperties runProperties = ((DrawingTextRunBase)runs[0]).RunProperties;
					fontInfo = XlsCharacterPropertiesHelper.GetRunFontInfo(runProperties, paragraphProperties, element, palette);
				}
				else
					fontInfo = XlsCharacterPropertiesHelper.GetRunFontInfo(paragraphProperties.DefaultCharacterProperties, element, palette);
			}
			else
				fontInfo = XlsCharacterPropertiesHelper.GetDefaultFontInfoForChartElement(element);
			return PrepareFontXCommand(fontInfo);
		}
		XlsCommandChartFontX PrepareFontXCommand(RunFontInfo fontInfo) {
			XlsCommandChartFontX command = new XlsCommandChartFontX();
			command.Value = GetFontIndex(fontInfo);
			return command;
		}
		void WriteText(TitleOptions title) {
			IChartText text = title.Text;
			XlsCommandChartText command = PrepareTextProperties(text as ChartRichText, title.TextProperties); 
			command.IsManualDataLabelPos = true;
			command.AutoText = text.TextType == ChartTextType.Auto;
			command.Write(StreamWriter);
		}
		XlsCommandChartText PrepareTextProperties(ChartRichText richText, TextProperties textProperties) {
			return richText != null ? PrepareTextProperties(richText) : PrepareTextProperties(textProperties);
		}
		XlsCommandChartText PrepareTextProperties(TextProperties textProperties) {
			XlsCommandChartText command = new XlsCommandChartText();
			DrawingTextParagraphProperties paragraphProperties = GetFirstParagraphProperties(textProperties.Paragraphs);
			PrepareTextPropertiesBase(command, textProperties.BodyProperties, paragraphProperties);
			IDrawingFill fill = paragraphProperties.DefaultCharacterProperties.Fill;
			if (fill.FillType == DrawingFillType.Solid)
				PrepareSolidFill(command, fill as DrawingSolidFill, false);
			else {
				command.TextColor = DXColor.Black;
				command.TextColorIndex = chart.DocumentModel.StyleSheet.Palette.GetPaletteNearestColorIndex(DXColor.Black);
			}
			return command;
		}
		XlsCommandChartText PrepareTextProperties(ChartRichText richText) {
			XlsCommandChartText command = new XlsCommandChartText();
			DrawingTextParagraphProperties paragraphProperties = GetFirstParagraphProperties(richText.Paragraphs);
			PrepareTextPropertiesBase(command, richText.BodyProperties, paragraphProperties);
			IDrawingFill fill = SelectFill(richText.Paragraphs[0]);
			if (fill.FillType == DrawingFillType.Solid)
				PrepareSolidFill(command, fill as DrawingSolidFill, richText.Lines.Length > 1);
			else {
				if (RichTextWithoutRuns(richText)) {
					command.TextColor = DXColor.FromArgb(186, 186, 151);
					command.TextColorIndex = Palette.FontAutomaticColorIndex;
				}
				else {
					command.TextColor = DXColor.Black;
					command.TextColorIndex = chart.DocumentModel.StyleSheet.Palette.GetPaletteNearestColorIndex(DXColor.Black);
				}
			}
			return command;
		}
		void PrepareSolidFill(XlsCommandChartText command, DrawingSolidFill fill, bool withAIRuns) {
			Palette palette = chart.DocumentModel.StyleSheet.Palette;
			DrawingColor drawingColor = ((DrawingSolidFill)fill).Color;
			int colorIndex = withAIRuns ? palette.GetPaletteNearestColorIndex(drawingColor.OriginalColor.Rgb) : palette.GetPaletteNearestColorIndex(drawingColor.FinalColor);
			command.TextColor = palette[colorIndex];
			command.TextColorIndex = colorIndex;
		}
		IDrawingFill SelectFill(DrawingTextParagraph paragraph) {
			DrawingTextRunCollection runs = paragraph.Runs;
			int runCount = runs.Count;
			if (runCount > 0) {
				DrawingTextRunBase run = runs[0] as DrawingTextRunBase;
				IDrawingFill runFill = run.RunProperties.Fill;
				if (runFill.FillType != DrawingFillType.Automatic)
					return runFill;
			}
			return paragraph.ParagraphProperties.DefaultCharacterProperties.Fill;
		}
		DrawingTextParagraphProperties GetFirstParagraphProperties(DrawingTextParagraphCollection collection) {
			return collection.Count > 0 ? collection[0].ParagraphProperties : new DrawingTextParagraphProperties(chart.DocumentModel);
		}
		void PrepareTextPropertiesBase(XlsCommandChartText command, DrawingTextBodyProperties bodyProperties, DrawingTextParagraphProperties paragraphProperties) {
			if (paragraphProperties.Options.HasFontAlignment)
				command.HorizontalAlignment = GetHorizontalAlignment(paragraphProperties.TextAlignment, paragraphProperties.RightToLeft);
			else
				command.HorizontalAlignment = DrawingTextAlignmentType.Center;
			command.VerticalAlignment = bodyProperties.Anchor;
			command.IsTransparent = true;
			command.TextReadingOrder = XlReadingOrder.Context;
			command.RotationAngle = bodyProperties.Rotation;
		}
		void WriteBRAI() {
			XlsCommandChartDataRef command = PrepareBRAI();
			command.Write(StreamWriter);
		}
		XlsCommandChartDataRef PrepareBRAI() {
			XlsCommandChartDataRef command = new XlsCommandChartDataRef();
			command.DataRef.DataType = XlsChartDataRefType.Literal;
			command.DataRef.IsSourceLinked = true;
			return command;
		}
		void WriteReferenceBRAI(String seriesText, ParsedExpression expression) {
			XlsCommandChartDataRef command = PrepareReferenceBRAI(seriesText, expression);
			command.Write(StreamWriter);
		}
		XlsCommandChartDataRef PrepareReferenceBRAI(String seriesText, ParsedExpression expression) {
			XlsCommandChartDataRef command = new XlsCommandChartDataRef();
			command.DataRef.DataType = XlsChartDataRefType.Reference;
			command.DataRef.IsSourceLinked = true;
			command.DataRef.SeriesText = seriesText;
			command.DataRef.SetExpression(XlsParsedThingConverter.ToXlsExpression(expression, ExportStyleSheet.RPNContext), ExportStyleSheet.RPNContext);
			return command;
		}
		void WriteAIRuns(DrawingTextParagraphCollection collection, ChartElement element) {
			XlsCommandChartTextRuns command = PrepareAIRuns(collection, element);
			if (command == null)
				return;
			command.Write(StreamWriter);
		}
		XlsCommandChartTextRuns PrepareAIRuns(DrawingTextParagraphCollection collection, ChartElement element) {
			int paragraphCount = collection.Count;
			if (paragraphCount < 1)
				return null;
			XlsCommandChartTextRuns command = new XlsCommandChartTextRuns();
			int position = 0;
			Palette palette = Chart.DocumentModel.StyleSheet.Palette;
			for (int i = 0; i < paragraphCount; i++) {
				DrawingTextParagraph paragraph = collection[i];
				DrawingTextParagraphProperties paragraphProperties = collection[0].ParagraphProperties;
				int runCount = paragraph.Runs.Count;
				if (runCount == 0 && i != paragraphCount - 1) {
					RunFontInfo fontInfo = XlsCharacterPropertiesHelper.GetRunFontInfo(paragraph.EndRunProperties, paragraphProperties, element, palette);
					AddRun(command.FormatRuns, position, fontInfo);
				}
				for (int j = 0; j < runCount; j++) {
					IDrawingTextRun run = paragraph.Runs[j];
					DrawingTextCharacterProperties runProperties = ((DrawingTextRunBase)run).RunProperties;
					RunFontInfo fontInfo = XlsCharacterPropertiesHelper.GetRunFontInfo(runProperties, paragraphProperties, element, palette);
					AddRun(command.FormatRuns, position, fontInfo);
					position += run.Text.Length;
				}
				position++;
			}
			AddRun(command.FormatRuns, position - 1, null);
			return command;
		}
		void AddRun(List<XlsFormatRun> runList, int position, RunFontInfo info) {
			XlsFormatRun run = new XlsFormatRun();
			run.CharIndex = position;
			run.FontIndex = GetFontIndex(info);
			runList.Add(run);
		}
		short GetFontIndex(RunFontInfo info) {
			if (info == null)
				return 0;
			Dictionary<RunFontInfo, int> chartFontTable = ExportStyleSheet.ChartFontTable;
			if (chartFontTable.ContainsKey(info))
				return (short)(chartFontTable[info] + ExportStyleSheet.FontList.Count);
			return 0;
		}
		bool RichTextWithoutRuns(ChartRichText richText) {
			if (richText == null)
				return false;
			DrawingTextParagraphCollection collection = richText.Paragraphs;
			int paragraphCount = collection.Count;
			int firstParagraphRunCount = collection[0].Runs.Count;
			return (paragraphCount == 1 && firstParagraphRunCount <= 1) ||
				   (paragraphCount == 2 && firstParagraphRunCount <= 1 && collection[1].Runs.Count == 0);
		}
		#endregion
		#region WriteAttachedLabelForDisplayUnits
		void WriteAttachedLabelForDisplayUnits(DisplayUnitOptions displayUnit) {
			IChartText text = displayUnit.Text;
			ChartElement element = ChartElement.AxisTitle;
			WriteStartObject(XlsChartObjectKind.DisplayUnits);
			WriteFrtWrapper(PrepareTextCommand(displayUnit));
			WriteFrtWrapper(commandBegin);
			WriteFrtWrapper(PreparePositonCommand(displayUnit.Layout));
			WriteWrappedFontX(text as ChartRichText, displayUnit.TextProperties, element);
			WriteWrappedChartText(text, element);
			WriteWrappedFrame(displayUnit.ShapeProperties, element);
			WriteFrtWrapper(PrepareObjectLinkForDisplayUnits());
			WriteCrtLayout12(displayUnit.Layout);
			WriteFrtWrapper(commandEnd);
			WriteEndObject(XlsChartObjectKind.DisplayUnits);
		}
		XlsCommandChartText PrepareTextCommand(DisplayUnitOptions displayUnit) {
			IChartText text = displayUnit.Text;
			XlsCommandChartText command = PrepareTextProperties(text as ChartRichText, displayUnit.TextProperties);
			command.DataLabelPos = DataLabelPosition.Default;
			command.AutoText = text.TextType == ChartTextType.Auto;
			return command;
		}
		XlsCommandChartTextObjectLink PrepareObjectLinkForDisplayUnits() {
			XlsCommandChartTextObjectLink command = new XlsCommandChartTextObjectLink();
			command.LinkType = XlsChartTextObjectLinkType.DispUnitLabel;
			return command;
		}
		#endregion
	}
	#region ChartTextExportWalker
	public class ChartTextExportWalker : IChartTextVisitor {
		readonly XlsChartExporter exporter;
		readonly ChartElement element;
		public ChartTextExportWalker(XlsChartExporter exporter, ChartElement element) {
			this.exporter = exporter;
			this.element = element;
		}
		#region IChartTextVisitor Members
		public void Visit(ChartText value) {
		}
		public void Visit(ChartRichText value) {
			exporter.WriteRichChartText(value, element);
		}
		public void Visit(ChartTextRef value) {
			exporter.WriteChartTextRef(value);
		}
		public void Visit(ChartTextValue value) {
		}
		#endregion
	}
	#endregion
}
