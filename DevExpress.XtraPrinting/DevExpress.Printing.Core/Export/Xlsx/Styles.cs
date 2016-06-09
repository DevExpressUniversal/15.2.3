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
using DevExpress.Office;
using System.Drawing;
using System.Collections.Generic;
using DevExpress.Utils;
using System.Globalization;
using DevExpress.Export.Xl;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraExport.Xlsx {
	using DevExpress.XtraExport.Implementation;
	partial class XlsxDataAwareExporter {
		readonly Dictionary<XlFont, int> fontsTable = new Dictionary<XlFont, int>();
		readonly Dictionary<XlFill, int> fillsTable = new Dictionary<XlFill, int>();
		readonly Dictionary<XlBorder, int> bordersTable = new Dictionary<XlBorder, int>();
		readonly Dictionary<XlCellAlignment, int> alignmentTable = new Dictionary<XlCellAlignment, int>();
		readonly List<XlCellAlignment> alignmentList = new List<XlCellAlignment>();
		readonly Dictionary<string, ExcelNumberFormat> numberFormatsTable = new Dictionary<string, ExcelNumberFormat>();
		readonly Dictionary<XlNetNumberFormat, ExcelNumberFormat> netNumberFormatsTable = new Dictionary<XlNetNumberFormat, ExcelNumberFormat>();
		readonly Dictionary<XlCellXf, int> cellXfTable = new Dictionary<XlCellXf, int>();
		readonly XlExportNumberFormatConverter numberFormatConverter = new XlExportNumberFormatConverter();
		int customNumberFormatId;
		XlFont defaultFont;
		XlCellAlignment defaultAlignment;
		XlBorder defaultBorder;
		readonly Dictionary<XlDxf, int> dxfTable = new Dictionary<XlDxf, int>();
		#region Translation tables
		internal static readonly Dictionary<XlUnderlineType, string> underlineTypeTable = CreateUnderlineTypeTable();
		static Dictionary<XlScriptType, string> verticalAlignmentRunTypeTable = CreateVerticalAlignmentRunTypeTable();
		static Dictionary<XlFontSchemeStyles, string> fontSchemeStyleTable = CreateFontSchemeStyleTable();
		internal static readonly Dictionary<XlPatternType, string> patternTypeTable = CreatePatternTypeTable();
		internal static readonly Dictionary<XlHorizontalAlignment, string> horizontalAlignmentTable = CreateHorizontalAlignmentTable();
		internal static readonly Dictionary<XlVerticalAlignment, string> verticalAlignmentTable = CreateVerticalAlignmentTable();
		internal static readonly Dictionary<XlReadingOrder, string> readingOrderTable = CreateReadingOrderTable();
		internal static readonly Dictionary<XlBorderLineStyle, string> borderLineStylesTable = CreateBorderLineStylesTable();
		static Dictionary<XlUnderlineType, string> CreateUnderlineTypeTable() {
			Dictionary<XlUnderlineType, string> result = new Dictionary<XlUnderlineType, string>();
			result.Add(XlUnderlineType.Single, "single");
			result.Add(XlUnderlineType.Double, "double");
			result.Add(XlUnderlineType.SingleAccounting, "singleAccounting");
			result.Add(XlUnderlineType.DoubleAccounting, "doubleAccounting");
			result.Add(XlUnderlineType.None, "none");
			return result;
		}
		static Dictionary<XlScriptType, string> CreateVerticalAlignmentRunTypeTable() {
			Dictionary<XlScriptType, string> result = new Dictionary<XlScriptType, string>();
			result.Add(XlScriptType.Baseline, "baseline");
			result.Add(XlScriptType.Subscript, "subscript");
			result.Add(XlScriptType.Superscript, "superscript");
			return result;
		}
		static Dictionary<XlFontSchemeStyles, string> CreateFontSchemeStyleTable() {
			Dictionary<XlFontSchemeStyles, string> result = new Dictionary<XlFontSchemeStyles, string>();
			result.Add(XlFontSchemeStyles.None, "none");
			result.Add(XlFontSchemeStyles.Minor, "minor");
			result.Add(XlFontSchemeStyles.Major, "major");
			return result;
		}
		static Dictionary<XlPatternType, string> CreatePatternTypeTable() {
			Dictionary<XlPatternType, string> result = new Dictionary<XlPatternType, string>();
			result.Add(XlPatternType.DarkDown, "darkDown");
			result.Add(XlPatternType.DarkGray, "darkGray");
			result.Add(XlPatternType.DarkGrid, "darkGrid");
			result.Add(XlPatternType.DarkHorizontal, "darkHorizontal");
			result.Add(XlPatternType.DarkTrellis, "darkTrellis");
			result.Add(XlPatternType.DarkUp, "darkUp");
			result.Add(XlPatternType.DarkVertical, "darkVertical");
			result.Add(XlPatternType.Gray0625, "gray0625");
			result.Add(XlPatternType.Gray125, "gray125");
			result.Add(XlPatternType.LightDown, "lightDown");
			result.Add(XlPatternType.LightGray, "lightGray");
			result.Add(XlPatternType.LightGrid, "lightGrid");
			result.Add(XlPatternType.LightHorizontal, "lightHorizontal");
			result.Add(XlPatternType.LightTrellis, "lightTrellis");
			result.Add(XlPatternType.LightUp, "lightUp");
			result.Add(XlPatternType.LightVertical, "lightVertical");
			result.Add(XlPatternType.MediumGray, "mediumGray");
			result.Add(XlPatternType.None, "none");
			result.Add(XlPatternType.Solid, "solid");
			return result;
		}
		static Dictionary<XlReadingOrder, string> CreateReadingOrderTable() {
			Dictionary<XlReadingOrder, string> result = new Dictionary<XlReadingOrder, string>();
			result.Add(XlReadingOrder.Context, "0");
			result.Add(XlReadingOrder.LeftToRight, "1");
			result.Add(XlReadingOrder.RightToLeft, "2");
			return result;
		}
		static Dictionary<XlVerticalAlignment, string> CreateVerticalAlignmentTable() {
			Dictionary<XlVerticalAlignment, string> result = new Dictionary<XlVerticalAlignment, string>();
			result.Add(XlVerticalAlignment.Bottom, "bottom");
			result.Add(XlVerticalAlignment.Center, "center");
			result.Add(XlVerticalAlignment.Distributed, "distributed");
			result.Add(XlVerticalAlignment.Justify, "justify");
			result.Add(XlVerticalAlignment.Top, "top");
			return result;
		}
		static Dictionary<XlHorizontalAlignment, string> CreateHorizontalAlignmentTable() {
			Dictionary<XlHorizontalAlignment, string> result = new Dictionary<XlHorizontalAlignment, string>();
			result.Add(XlHorizontalAlignment.Center, "center");
			result.Add(XlHorizontalAlignment.CenterContinuous, "centerContinuous");
			result.Add(XlHorizontalAlignment.Distributed, "distributed");
			result.Add(XlHorizontalAlignment.Fill, "fill");
			result.Add(XlHorizontalAlignment.General, "general");
			result.Add(XlHorizontalAlignment.Justify, "justify");
			result.Add(XlHorizontalAlignment.Left, "left");
			result.Add(XlHorizontalAlignment.Right, "right");
			return result;
		}
		static Dictionary<XlBorderLineStyle, string> CreateBorderLineStylesTable() {
			Dictionary<XlBorderLineStyle, string> result = new Dictionary<XlBorderLineStyle, string>();
			result.Add(XlBorderLineStyle.DashDot, "dashDot");
			result.Add(XlBorderLineStyle.DashDotDot, "dashDotDot");
			result.Add(XlBorderLineStyle.Dashed, "dashed");
			result.Add(XlBorderLineStyle.Dotted, "dotted");
			result.Add(XlBorderLineStyle.Double, "double");
			result.Add(XlBorderLineStyle.Hair, "hair");
			result.Add(XlBorderLineStyle.Medium, "medium");
			result.Add(XlBorderLineStyle.MediumDashDot, "mediumDashDot");
			result.Add(XlBorderLineStyle.MediumDashDotDot, "mediumDashDotDot");
			result.Add(XlBorderLineStyle.MediumDashed, "mediumDashed");
			result.Add(XlBorderLineStyle.None, "none");
			result.Add(XlBorderLineStyle.SlantDashDot, "slantDashDot");
			result.Add(XlBorderLineStyle.Thick, "thick");
			result.Add(XlBorderLineStyle.Thin, "thin");
			return result;
		}
		#endregion
		#region InitializeStyles
		void InitializeStyles() {
			InitFontTable();
			InitAlignmentTable();
			InitFillTable();
			InitBorderTable();
			InitNumberFormatTable();
			cellXfTable.Clear();
			cellXfTable.Add(new XlCellXf(), 0);
		}
		void InitFontTable() {
			fontsTable.Clear();
			this.defaultFont = XlFont.BodyFont();
			fontsTable.Add(defaultFont, 0);
		}
		void InitAlignmentTable() {
			alignmentTable.Clear();
			alignmentList.Clear();
			this.defaultAlignment = new XlCellAlignment();
			alignmentTable.Add(defaultAlignment, 0);
			alignmentList.Add(defaultAlignment);
		}
		void InitFillTable() {
			fillsTable.Clear();
			fillsTable.Add(XlFill.NoFill(), 0);
			fillsTable.Add(XlFill.PatternFill(XlPatternType.Gray125), 1);
		}
		void InitBorderTable() {
			bordersTable.Clear();
			this.defaultBorder = new XlBorder();
			bordersTable.Add(defaultBorder, 0);
		}
		void InitNumberFormatTable() {
			numberFormatsTable.Clear();
			netNumberFormatsTable.Clear();
			customNumberFormatId = 164;
		}
		#endregion
		void AddStylesContent() {
			BeginWriteXmlContent();
			WriteShStartElement("styleSheet");
			try {
				GenerateNumberFormatsContent();
				GenerateFontsContent();
				GenerateFillsContent();
				GenerateBordersContent();
				GenerateCellStyleFormatsContent();
				GenerateCellFormatsContent();
				GenerateCellStylesContent();
				GenerateDifferentialFormatsContent();
			}
			finally {
				WriteShEndElement();
			}
			AddPackageContent(@"xl/styles.xml", EndWriteXmlContent());
			Builder.OverriddenContentTypes.Add("/xl/styles.xml", XlsxPackageBuilder.StylesContentType);
			Builder.WorkbookRelations.Add(new OpenXmlRelation(Builder.WorkbookRelations.GenerateId(), "styles.xml", XlsxPackageBuilder.RelsStylesNamespace));
		}
		#region NumberFormat
		void GenerateNumberFormatsContent() {
			int count = numberFormatsTable.Count;
			if (count == 0)
				return;
			WriteShStartElement("numFmts");
			try {
				WriteIntValue("count", count);
				foreach (ExcelNumberFormat item in numberFormatsTable.Values)
					WriteNumberFormat(item);
			}
			finally {
				WriteShEndElement();
			}
		}
		void WriteNumberFormat(ExcelNumberFormat format) {
			WriteShStartElement("numFmt");
			try {
				WriteIntValue("numFmtId", format.Id);
				WriteStringValue("formatCode", EncodeXmlChars(format.FormatString));
			}
			finally {
				WriteShEndElement();
			}
		}
		bool IsVolatileNumberFormat(ExcelNumberFormat format) {
			if (format == null) return false;
			int id = format.Id;
			return (id >= 5 && id <= 8) || (id >= 23 && id <= 26) || (id >= 41 && id <= 44) || (id >= 63 && id <= 66) || (id >= 164);
		}
		#endregion
		void WriteCellFormat(XlCellXf xf) {
			WriteShStartElement("xf");
			try {
				WriteIntValue("numFmtId", xf.NumberFormatId);
				WriteIntValue("fontId", xf.FontId);
				WriteIntValue("fillId", xf.FillId);
				WriteIntValue("borderId", xf.BorderId);
				WriteIntValue("xfId", xf.XfId);
				WriteApplyValue("applyNumberFormat", xf.ApplyNumberFormat);
				WriteApplyValue("applyFont", xf.ApplyFont);
				WriteApplyValue("applyFill", xf.ApplyFill);
				WriteApplyValue("applyBorder", xf.ApplyBorder);
				WriteApplyValue("applyAlignment", xf.ApplyAlignment);
				WriteApplyValue("applyProtection", xf.ApplyProtection);
				WriteAlignment(xf);
				if (xf.QuotePrefix)
					WriteBoolValue("quotePrefix", true);
			}
			finally {
				WriteShEndElement();
			}
		}
		void WriteAlignment(XlCellXf xf) {
			if (xf.AlignmentId <= 0)
				return;
			XlCellAlignment info = alignmentList[xf.AlignmentId];
			WriteAlignment(info);
		}
		void WriteAlignment(XlCellAlignment info) {
			WriteShStartElement("alignment");
			try {
				if(defaultAlignment.HorizontalAlignment != info.HorizontalAlignment)
					WriteStringValue("horizontal", horizontalAlignmentTable[info.HorizontalAlignment]);
				if(defaultAlignment.VerticalAlignment != info.VerticalAlignment)
					WriteStringValue("vertical", verticalAlignmentTable[info.VerticalAlignment]);
				if(defaultAlignment.TextRotation != info.TextRotation)
					WriteIntValue("textRotation", info.TextRotation);
				if(defaultAlignment.WrapText != info.WrapText)
					WriteBoolValue("wrapText", info.WrapText);
				if(defaultAlignment.Indent != info.Indent)
					WriteIntValue("indent", info.Indent);
				if(defaultAlignment.RelativeIndent != info.RelativeIndent)
					WriteIntValue("relativeIndent", info.RelativeIndent);
				if(defaultAlignment.JustifyLastLine != info.JustifyLastLine)
					WriteBoolValue("justifyLastLine", info.JustifyLastLine);
				if(defaultAlignment.ShrinkToFit != info.ShrinkToFit)
					WriteBoolValue("shrinkToFit", info.ShrinkToFit);
				if(defaultAlignment.ReadingOrder != info.ReadingOrder)
					WriteStringValue("readingOrder", readingOrderTable[info.ReadingOrder]);
			}
			finally {
				WriteShEndElement();
			}
		}
		void WriteApplyValue(string nameAttribute, bool value) {
			if (value)
				WriteBoolValue(nameAttribute, value);
		}
		#region GenerateFontsContent
		void GenerateFontsContent() {
			WriteShStartElement("fonts");
			try {
				WriteIntValue("count", this.fontsTable.Count);
				int normalFontIndex = 0;
				foreach (XlFont font in fontsTable.Keys)
					WriteFont(font, fontsTable[font] == normalFontIndex);
			}
			finally {
				WriteShEndElement();
			}
		}
		void WriteFont(XlFont font, bool isDefaultFont) {
			WriteShStartElement("font");
			try {
				if (font.Bold != defaultFont.Bold)
					WriteBoolFontValue("b", font.Bold);
				if (font.Italic != defaultFont.Italic)
					WriteBoolFontValue("i", font.Italic);
				if (font.StrikeThrough != defaultFont.StrikeThrough)
					WriteBoolFontValue("strike", font.StrikeThrough);
				if (font.Condense != defaultFont.Condense)
					WriteBoolFontValue("condense", font.Condense);
				if (font.Extend != defaultFont.Extend)
					WriteBoolFontValue("extend", font.Extend);
				if (font.Outline != defaultFont.Outline)
					WriteBoolFontValue("outline", font.Outline);
				if (font.Shadow != defaultFont.Shadow)
					WriteBoolFontValue("shadow", font.Shadow);
				if (font.Underline != defaultFont.Underline)
					WriteUnderlineType(font.Underline);
				if (font.Script != defaultFont.Script)
					WriteVerticalAlignment(font.Script);
				if(font.Size != 0)
					WriteFontSize(font.Size);
				XlColor fontColor = font.Color;
				if ((!fontColor.Equals(defaultFont.Color) || isDefaultFont) && !fontColor.IsEmpty)
					WriteColor(fontColor, "color");
				if (!string.IsNullOrEmpty(font.Name))
					WriteFontName(font.Name);
				if (font.FontFamily != defaultFont.FontFamily || isDefaultFont)
					WriteFontFamily((int)font.FontFamily);
				if (font.Charset != defaultFont.Charset)
					WriteCharset(font.Charset);
				if (font.SchemeStyle != XlFontSchemeStyles.None)
					WriteSchemeStyle(font.SchemeStyle);
			}
			finally {
				WriteShEndElement();
			}
		}
		void WriteSchemeStyle(XlFontSchemeStyles schemeStyle) {
			WriteShStartElement("scheme");
			try {
				WriteStringValue("val", fontSchemeStyleTable[schemeStyle]);
			}
			finally {
				WriteShEndElement();
			}
		}
		void WriteCharset(int charset) {
			WriteShStartElement("charset");
			try {
				WriteIntValue("val", charset);
			}
			finally {
				WriteShEndElement();
			}
		}
		void WriteFontFamily(int fontFamily) {
			WriteShStartElement("family");
			try {
				WriteIntValue("val", fontFamily);
			}
			finally {
				WriteShEndElement();
			}
		}
		void WriteFontName(string name) {
			WriteShStartElement("name");
			try {
				WriteStringValue("val", name);
			}
			finally {
				WriteShEndElement();
			}
		}
		void WriteFontSize(double size) {
			WriteShStartElement("sz");
			try {
				string sz = size.ToString(CultureInfo.InvariantCulture);
				WriteStringValue("val", sz);
			}
			finally {
				WriteShEndElement();
			}
		}
		void WriteVerticalAlignment(XlScriptType script) {
			WriteShStartElement("vertAlign");
			try {
				WriteStringValue("val", verticalAlignmentRunTypeTable[script]);
			}
			finally {
				WriteShEndElement();
			}
		}
		void WriteUnderlineType(XlUnderlineType underlineType) {
			WriteShStartElement("u");
			try {
				if (underlineType != XlUnderlineType.Single)
					WriteStringValue("val", underlineTypeTable[underlineType]);
			}
			finally {
				WriteShEndElement();
			}
		}
		void WriteBoolFontValue(string tag, bool value) {
			WriteShStartElement(tag);
			try {
				if (!value)
					WriteBoolValue("val", value);
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
		#region GenerateFillsContent
		void GenerateFillsContent() {
			WriteShStartElement("fills");
			try {
				WriteIntValue("count", fillsTable.Count);
				foreach (XlFill fill in fillsTable.Keys)
					WriteFill(fill);
			}
			finally {
				WriteShEndElement();
			}
		}
		void WriteFill(XlFill fill) {
			WriteShStartElement("fill");
			try {
				WriteShStartElement("patternFill");
				try {
					WriteStringValue("patternType", patternTypeTable[fill.PatternType]);
					WriteColor(fill.ForeColor, "fgColor");
					WriteColor(fill.BackColor, "bgColor");
				}
				finally {
					WriteShEndElement();
				}
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
		#region GenerateBordersContent
		void GenerateBordersContent() {
			WriteShStartElement("borders");
			try {
				WriteIntValue("count", bordersTable.Count);
				foreach (XlBorder border in bordersTable.Keys)
					WriteBorders(border);
			}
			finally {
				WriteShEndElement();
			}
		}
		void WriteBorders(XlBorder border) {
			XlBorder defaultItem = defaultBorder;
			WriteShStartElement("border");
			try {
				if (defaultItem.DiagonalUp != border.DiagonalUp)
					WriteBoolValue("diagonalUp", border.DiagonalUp);
				if (defaultItem.DiagonalDown != border.DiagonalDown)
					WriteBoolValue("diagonalDown", border.DiagonalDown);
				if (defaultItem.Outline != border.Outline)
					WriteBoolValue("outline", border.Outline);
				if (defaultItem.LeftLineStyle != border.LeftLineStyle || defaultItem.LeftColor != border.LeftColor)
					WriteBorder(border.LeftLineStyle, border.LeftColor, "left");
				if (defaultItem.RightLineStyle != border.RightLineStyle || defaultItem.RightColor != border.RightColor)
					WriteBorder(border.RightLineStyle, border.RightColor, "right");
				if (defaultItem.TopLineStyle != border.TopLineStyle || defaultItem.TopColor != border.TopColor)
					WriteBorder(border.TopLineStyle, border.TopColor, "top");
				if (defaultItem.BottomLineStyle != border.BottomLineStyle || defaultItem.BottomColor != border.BottomColor)
					WriteBorder(border.BottomLineStyle, border.BottomColor, "bottom");
				if (defaultItem.DiagonalLineStyle != border.DiagonalLineStyle || defaultItem.DiagonalColor != border.DiagonalColor)
					WriteBorder(border.DiagonalLineStyle, border.DiagonalColor, "diagonal");
				if (defaultItem.VerticalLineStyle != border.VerticalLineStyle || defaultItem.VerticalColor != border.VerticalColor)
					WriteBorder(border.VerticalLineStyle, border.VerticalColor, "vertical");
				if (defaultItem.HorizontalLineStyle != border.HorizontalLineStyle || defaultItem.HorizontalColor != border.HorizontalColor)
					WriteBorder(border.HorizontalLineStyle, border.HorizontalColor, "horizontal");
			}
			finally {
				WriteShEndElement();
			}
		}
		void WriteBorder(XlBorderLineStyle lineStyle, XlColor color, string tag) {
			WriteShStartElement(tag);
			try {
				if(lineStyle != XlBorderLineStyle.None) {
					WriteStringValue("style", borderLineStylesTable[lineStyle]);
					WriteColor(color, "color");
				}
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
		#region GenerateCellStyleFormatsContent
		void GenerateCellStyleFormatsContent() {
			WriteShStartElement("cellStyleXfs");
			try {
				WriteIntValue("count", 1);
				WriteShStartElement("xf");
				try {
					WriteIntValue("numFmtId", 0);
					WriteIntValue("fontId", 0);
					WriteIntValue("fillId", 0);
					WriteIntValue("borderId", 0);
				}
				finally {
					WriteShEndElement();
				}
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
		#region GenerateCellFormatsContent
		void GenerateCellFormatsContent() {
			WriteShStartElement("cellXfs");
			try {
				WriteIntValue("count", cellXfTable.Count);
				foreach (XlCellXf xf in cellXfTable.Keys)
					WriteCellFormat(xf);
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
		#region GenerateCellStylesContent
		void GenerateCellStylesContent() {
			WriteShStartElement("cellStyles");
			try {
				WriteIntValue("count", 1);
				WriteShStartElement("cellStyle");
				try {
					WriteStringValue("name", "Normal");
					WriteIntValue("xfId", 0);
					WriteIntValue("builtinId", 0);
				}
				finally {
					WriteShEndElement();
				}
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
		#region GenerateDifferentialFormatsContent
		void GenerateDifferentialFormatsContent() {
			if(dxfTable.Count == 0)
				return;
			WriteShStartElement("dxfs");
			try {
				foreach(XlDxf item in dxfTable.Keys)
					GenerateDifferentialFormatContent(item);
			}
			finally {
				WriteShEndElement();
			}
		}
		void GenerateDifferentialFormatContent(XlDxf dxf) {
			WriteShStartElement("dxf");
			try {
				if(dxf.Font != null)
					WriteFont(dxf.Font, false);
				if(dxf.NumberFormat != null)
					WriteNumberFormat(dxf.NumberFormat);
				if(dxf.Fill != null)
					WriteFill(dxf.Fill);
				if(dxf.Alignment != null)
					WriteAlignment(dxf.Alignment);
				if(dxf.Border != null)
					WriteBorders(dxf.Border);
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
		int RegisterFormatting(XlCellFormatting formatting) {
			if (formatting == null)
				return -1;
			int fontId = RegisterFont(formatting.Font);
			int fillId = RegisterFill(formatting.Fill);
			int borderId = RegisterBorder(formatting.Border);
			int numberFormatId = RegisterNumberFormat(formatting.NetFormatString, formatting.IsDateTimeFormatString, formatting.NumberFormat);
			int alignmentId = RegisterAlignment(formatting.Alignment);
			XlCellXf xf = new XlCellXf();
			xf.FontId = Math.Max(0, fontId);
			xf.FillId = Math.Max(0, fillId);
			xf.NumberFormatId = Math.Max(0, numberFormatId);
			xf.BorderId = Math.Max(0, borderId);
			xf.AlignmentId = Math.Max(0, alignmentId);
			xf.ApplyFont = (fontId >= 0);
			xf.ApplyFill = (fillId >= 0);
			xf.ApplyNumberFormat = (numberFormatId >= 0);
			xf.ApplyAlignment = (alignmentId >= 0);
			int index;
			if (cellXfTable.TryGetValue(xf, out index))
				return index;
			index = cellXfTable.Count;
			cellXfTable.Add(xf, index);
			return index;
		}
		int RegisterFont(XlFont font) {
			if (font == null)
				return -1;
			int index;
			if (fontsTable.TryGetValue(font, out index))
				return index;
			index = fontsTable.Count;
			fontsTable.Add(font, index);
			return index;
		}
		int RegisterFill(XlFill fill) {
			if (fill == null)
				return -1;
			int index;
			if (fillsTable.TryGetValue(fill, out index))
				return index;
			index = fillsTable.Count;
			fillsTable.Add(fill, index);
			return index;
		}
		int RegisterBorder(XlBorder border) {
			if (border == null)
				return -1;
			int index;
			if (bordersTable.TryGetValue(border, out index))
				return index;
			index = bordersTable.Count;
			bordersTable.Add(border, index);
			return index;
		}
		int RegisterNumberFormat(string netFormatString, bool isDateTimeFormatString, XlNumberFormat xlNumberFormat) {
			ExcelNumberFormat numberFormat;
			if(String.IsNullOrEmpty(netFormatString)) {
				if(xlNumberFormat == null)
					return -1;
				if(xlNumberFormat.FormatId >= 0)
					return xlNumberFormat.FormatId;
				string formatCode = xlNumberFormat.FormatCode;
				if(numberFormatsTable.ContainsKey(formatCode))
					numberFormat = numberFormatsTable[formatCode];
				else {
					numberFormat = new ExcelNumberFormat(this.customNumberFormatId, formatCode);
					numberFormatsTable.Add(formatCode, numberFormat);
					this.customNumberFormatId++;
				}
				return numberFormat.Id;
			}
			XlNetNumberFormat netFormat = new XlNetNumberFormat() { FormatString = netFormatString, IsDateTimeFormat = isDateTimeFormatString };
			if(netNumberFormatsTable.TryGetValue(netFormat, out numberFormat))
				return numberFormat.Id;
			numberFormat = ConvertNetFormatStringToXlFormatCode(netFormatString, isDateTimeFormatString);
			if(numberFormat == null)
				return -1;
			if(String.IsNullOrEmpty(numberFormat.FormatString))
				return 0;
			if(numberFormat.Id >= 0)
				return numberFormat.Id;
			if(numberFormatsTable.ContainsKey(numberFormat.FormatString))
				numberFormat = numberFormatsTable[numberFormat.FormatString];
			else {
				numberFormat.Id = this.customNumberFormatId;
				numberFormatsTable.Add(numberFormat.FormatString, numberFormat);
				this.customNumberFormatId++;
			}
			netNumberFormatsTable.Add(netFormat, numberFormat);
			return numberFormat.Id;
		}
		ExcelNumberFormat ConvertNetFormatStringToXlFormatCode(string netFormatString, bool isDateTimeFormatString) {
			CultureInfo culture = options.Culture;
			if (culture == null)
				culture = CultureInfo.InvariantCulture;
			return numberFormatConverter.Convert(netFormatString, isDateTimeFormatString, culture);
		}
		int RegisterAlignment(XlCellAlignment alignment) {
			if (alignment == null)
				return -1;
			int index;
			if (alignmentTable.TryGetValue(alignment, out index))
				return index;
			index = alignmentTable.Count;
			alignmentTable.Add(alignment, index);
			alignmentList.Add(alignment);
			return index;
		}
		int RegisterDifferentialFormatting(XlDifferentialFormatting formatting) {
			if(formatting == null)
				return -1;
			XlDxf dxf = new XlDxf();
			dxf.Font = formatting.Font;
			dxf.Fill = formatting.Fill;
			dxf.Alignment = formatting.Alignment;
			dxf.Border = formatting.Border;
			dxf.NumberFormat = CreateNumberFormat(formatting.NetFormatString, formatting.IsDateTimeFormatString, formatting.NumberFormat);
			if(dxf.IsEmpty)
				return -1;
			int index;
			if(dxfTable.TryGetValue(dxf, out index))
				return index;
			index = dxfTable.Count;
			dxfTable.Add(dxf, index);
			return index;
		}
		int RegisterCondFmtDifferentialFormatting(XlDifferentialFormatting formatting) {
			if(formatting == null)
				return -1;
			XlDxf dxf = new XlDxf();
			if(formatting.Font != null) {
				dxf.Font = formatting.Font.Clone();
				dxf.Font.Name = null;
				dxf.Font.Size = 0;
				dxf.Font.SchemeStyle = XlFontSchemeStyles.None;
			}
			dxf.Fill = formatting.Fill;
			dxf.Alignment = formatting.Alignment;
			dxf.Border = formatting.Border;
			dxf.NumberFormat = CreateNumberFormat(formatting.NetFormatString, formatting.IsDateTimeFormatString, formatting.NumberFormat);
			if(dxf.IsEmpty)
				return -1;
			int index;
			if(dxfTable.TryGetValue(dxf, out index))
				return index;
			index = dxfTable.Count;
			dxfTable.Add(dxf, index);
			return index;
		}
		ExcelNumberFormat CreateNumberFormat(string netFormatString, bool isDateTimeFormatString, XlNumberFormat xlNumberFormat) {
			ExcelNumberFormat numberFormat;
			if(String.IsNullOrEmpty(netFormatString)) {
				if (xlNumberFormat == null)
					return null;
				numberFormat = new ExcelNumberFormat(xlNumberFormat.FormatId, xlNumberFormat.FormatCode);
				if(numberFormat.Id < 0) {
					numberFormat.Id = this.customNumberFormatId;
					this.customNumberFormatId++;
				}
				return numberFormat;
			}
			XlNetNumberFormat netFormat = new XlNetNumberFormat() { FormatString = netFormatString, IsDateTimeFormat = isDateTimeFormatString };
			if(netNumberFormatsTable.TryGetValue(netFormat, out numberFormat))
				return numberFormat;
			numberFormat = ConvertNetFormatStringToXlFormatCode(netFormatString, isDateTimeFormatString);
			if(numberFormat != null) {
				if(numberFormat.Id < 0) {
					numberFormat.Id = this.customNumberFormatId;
					this.customNumberFormatId++;
				}
				if(String.IsNullOrEmpty(numberFormat.FormatString))
					numberFormat = new ExcelNumberFormat(0, String.Empty);
			}
			return numberFormat;
		}
	}
	#region XlCellXf
	internal struct XlCellXf {
		#region Fields
		const long MaskNumberFormatId = 0x00000000000000FF; 
		const long MaskFillId = 0x000000000000FF00; 
		const long MaskBorderId = 0x0000000000FF0000; 
		const long MaskAlignmentId = 0x00000000FF000000; 
		const long MaskApplyNumberFormat = 0x0000000100000000; 
		const long MaskApplyFont = 0x0000000200000000; 
		const long MaskApplyFill = 0x0000000400000000; 
		const long MaskApplyBorder = 0x0000000800000000; 
		const long MaskApplyProtection = 0x0000001000000000; 
		const long MaskApplyAlignment = 0x0000002000000000; 
		const long MaskQuotePrefix = 0x0000004000000000; 
		const long MaskFontId = 0x0003FF0000000000; 
		const long MaskXfId = 0x7FFC000000000000; 
		long packedValues;
		#endregion
		#region Properties
		public int NumberFormatId {
			get { return (int)(packedValues & MaskNumberFormatId); }
			set {
				packedValues &= ~MaskNumberFormatId;
				packedValues |= (long)value & MaskNumberFormatId;
			}
		}
		public int FillId {
			get { return (int)((packedValues & MaskFillId) >> 8); }
			set {
				packedValues &= ~MaskFillId;
				packedValues |= ((long)value << 8) & MaskFillId;
			}
		}
		public int BorderId {
			get { return (int)((packedValues & MaskBorderId) >> 16); }
			set {
				packedValues &= ~MaskBorderId;
				packedValues |= ((long)value << 16) & MaskBorderId;
			}
		}
		public int AlignmentId {
			get { return (int)((packedValues & MaskAlignmentId) >> 24); }
			set {
				packedValues &= ~MaskAlignmentId;
				packedValues |= ((long)value << 24) & MaskAlignmentId;
			}
		}
		public int FontId {
			get { return (int)((packedValues & MaskFontId) >> 40); }
			set {
				packedValues &= ~MaskFontId;
				packedValues |= ((long)value << 40) & MaskFontId;
			}
		}
		public int XfId {
			get { return (int)((packedValues & MaskXfId) >> 50); }
			set {
				packedValues &= ~MaskXfId;
				packedValues |= ((long)value << 50) & MaskXfId;
			}
		}
		public bool ApplyNumberFormat { get { return GetBooleanValue(MaskApplyNumberFormat); } set { SetBooleanValue(MaskApplyNumberFormat, value); } }
		public bool ApplyFont { get { return GetBooleanValue(MaskApplyFont); } set { SetBooleanValue(MaskApplyFont, value); } }
		public bool ApplyFill { get { return GetBooleanValue(MaskApplyFill); } set { SetBooleanValue(MaskApplyFill, value); } }
		public bool ApplyBorder { get { return GetBooleanValue(MaskApplyBorder); } set { SetBooleanValue(MaskApplyBorder, value); } }
		public bool ApplyAlignment { get { return GetBooleanValue(MaskApplyAlignment); } set { SetBooleanValue(MaskApplyAlignment, value); } }
		public bool ApplyProtection { get { return GetBooleanValue(MaskApplyProtection); } set { SetBooleanValue(MaskApplyProtection, value); } }
		public bool QuotePrefix { get { return GetBooleanValue(MaskQuotePrefix); } set { SetBooleanValue(MaskQuotePrefix, value); } }
		#endregion
		#region GetBooleanValue/SetBooleanValue helpers
		void SetBooleanValue(long mask, bool bitVal) {
			if (bitVal)
				packedValues |= mask;
			else
				packedValues &= ~mask;
		}
		bool GetBooleanValue(long mask) {
			return (packedValues & mask) != 0;
		}
		#endregion
	}
	#endregion
	#region XlDxf
	internal class XlDxf {
		public XlFont Font { get; set; }
		public ExcelNumberFormat NumberFormat { get; set; }
		public XlFill Fill { get; set; }
		public XlCellAlignment Alignment { get; set; }
		public XlBorder Border { get; set; }
		public bool IsEmpty {
			get {
				return Font == null && NumberFormat == null && Fill == null && Alignment == null && Border == null;
			}
		}
		public override bool Equals(object obj) {
			XlDxf other = obj as XlDxf;
			if(other == null)
				return false;
			if((Font != null && !Font.Equals(other.Font)) || (Font == null && other.Font != null))
				return false;
			if((NumberFormat != null && !NumberFormat.Equals(other.NumberFormat)) || (NumberFormat == null && other.NumberFormat != null))
				return false;
			if((Fill != null && !Fill.Equals(other.Fill)) || (Fill == null && other.Fill != null))
				return false;
			if((Alignment != null && !Alignment.Equals(other.Alignment)) || (Alignment == null && other.Alignment != null))
				return false;
			if((Border != null && !Border.Equals(other.Border)) || (Border == null && other.Border != null))
				return false;
			return true;
		}
		public override int GetHashCode() {
			int result = base.GetHashCode();
			if(Font != null)
				result ^= Font.GetHashCode();
			if(NumberFormat != null)
				result ^= NumberFormat.GetHashCode();
			if(Fill != null)
				result ^= Fill.GetHashCode();
			if(Alignment != null)
				result ^= Alignment.GetHashCode();
			if(Border != null)
				result ^= Border.GetHashCode();
			return result;
		}
	}
	#endregion
}
