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
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Xml;
using DevExpress.Utils.Zip;
using DevExpress.Export.Xl;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Office.Model;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		#region ExportStyles
		#region Translation tables
		internal static readonly Dictionary<XlPatternType, string> patternTypeTable = CreatePatternTypeTable();
		internal static readonly Dictionary<XlBorderLineStyle, string> borderLineStylesTable = CreateBorderLineStylesTable();
		internal static readonly Dictionary<XlHorizontalAlignment, string> horizontalAlignmentTable = CreateHorizontalAlignmentTable();
		internal static readonly Dictionary<XlVerticalAlignment, string> verticalAlignmentTable = CreateVerticalAlignmentTable();
		internal static readonly Dictionary<XlReadingOrder, string> readingOrderTable = CreateReadingOrderTable();
		internal static readonly Dictionary<int, string> tableStyleElementTypeTable = CreateTableStyleElementTypeTable();
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
		static Dictionary<int, string> CreateTableStyleElementTypeTable() {
			Dictionary<int, string> result = new Dictionary<int, string>();
			result.Add(TableStyle.WholeTableIndex, "wholeTable");
			result.Add(TableStyle.HeaderRowIndex, "headerRow");
			result.Add(TableStyle.TotalRowIndex, "totalRow");
			result.Add(TableStyle.FirstColumnIndex, "firstColumn");
			result.Add(TableStyle.LastColumnIndex, "lastColumn");
			result.Add(TableStyle.FirstRowStripeIndex, "firstRowStripe");
			result.Add(TableStyle.SecondRowStripeIndex, "secondRowStripe");
			result.Add(TableStyle.FirstColumnStripeIndex, "firstColumnStripe");
			result.Add(TableStyle.SecondColumnStripeIndex, "secondColumnStripe");
			result.Add(TableStyle.FirstHeaderCellIndex, "firstHeaderCell");
			result.Add(TableStyle.LastHeaderCellIndex, "lastHeaderCell");
			result.Add(TableStyle.FirstTotalCellIndex, "firstTotalCell");
			result.Add(TableStyle.LastTotalCellIndex, "lastTotalCell");
			result.Add(TableStyle.FirstSubtotalColumnIndex, "firstSubtotalColumn");
			result.Add(TableStyle.SecondSubtotalColumnIndex, "secondSubtotalColumn");
			result.Add(TableStyle.ThirdSubtotalColumnIndex, "thirdSubtotalColumn");
			result.Add(TableStyle.FirstSubtotalRowIndex, "firstSubtotalRow");
			result.Add(TableStyle.SecondSubtotalRowIndex, "secondSubtotalRow");
			result.Add(TableStyle.ThirdSubtotalRowIndex, "thirdSubtotalRow");
			result.Add(TableStyle.BlankRowIndex, "blankRow");
			result.Add(TableStyle.FirstColumnSubheadingIndex, "firstColumnSubheading");
			result.Add(TableStyle.SecondColumnSubheadingIndex, "secondColumnSubheading");
			result.Add(TableStyle.ThirdColumnSubheadingIndex, "thirdColumnSubheading");
			result.Add(TableStyle.FirstRowSubheadingIndex, "firstRowSubheading");
			result.Add(TableStyle.SecondRowSubheadingIndex, "secondRowSubheading");
			result.Add(TableStyle.ThirdRowSubheadingIndex, "thirdRowSubheading");
			result.Add(TableStyle.PageFieldLabelsIndex, "pageFieldLabels");
			result.Add(TableStyle.PageFieldValuesIndex, "pageFieldValues");
			return result;
		}
		#endregion
		protected internal virtual CompressedStream ExportStyles() {
			return CreateXmlContent(GenerateStylesXmlContent);
		}
		protected internal virtual void GenerateStylesXmlContent(XmlWriter writer) {
			DocumentContentWriter = writer;
			GenerateStylesContent();
		}
		protected internal virtual void GenerateStylesContent() {
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
				GenerateTableStylesContent();
				GenerateColorsContent();
			}
			finally {
				WriteShEndElement();
			}
		}
		#region ExportNumberFormats
		protected internal virtual void GenerateNumberFormatsContent() {
			List<int> customNumberFormats = ExportStyleSheet.GetCustomNumberFormats();
			if (customNumberFormats.Count == 0)
				return;
			WriteShStartElement("numFmts");
			try {
				WriteIntValue("count", customNumberFormats.Count);
				for (int i = 0; i < customNumberFormats.Count; ++i) {
					int index = customNumberFormats[i];
					NumberFormat numberFormat = Workbook.Cache.NumberFormatCache[index];
					WriteNumberFormat(index, numberFormat.FormatCode);
				}
			}
			finally {
				WriteShEndElement();
			}
		}
		void WriteNumberFormat(int id, string formatCode) {
			WriteShStartElement("numFmt");
			try {
				WriteIntValue("numFmtId", id);
				WriteStringValue("formatCode", formatCode);
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
		#region ExportFonts
		protected internal virtual void GenerateFontsContent() {
			WriteShStartElement("fonts");
			try {
				WriteIntValue("count", this.exportStyleSheet.FontInfoTable.Count);
				Dictionary<int, int> fontInfoTable = this.exportStyleSheet.FontInfoTable;
				int normalFontIndex = Workbook.StyleSheet.CellStyles.Normal.FormatInfo.FontIndex;
				foreach (int index in fontInfoTable.Keys)
					WriteFont(Workbook.Cache.FontInfoCache[index], index == normalFontIndex);
			}
			finally {
				WriteShEndElement();
			}
		}
		void WriteFont(RunFontInfo font, bool isDefaultFont) {
			RunFontInfo defaultItem = Workbook.GetDefaultRunFontInfoZeroItemInCache();
			WriteShStartElement("font");
			try {
				if(font.Bold != defaultItem.Bold)
					WriteBoolFontValue("b", font.Bold);
				if(font.Italic != defaultItem.Italic)
					WriteBoolFontValue("i", font.Italic);
				if(font.StrikeThrough != defaultItem.StrikeThrough)
					WriteBoolFontValue("strike", font.StrikeThrough);
				if(font.Condense != defaultItem.Condense)
					WriteBoolFontValue("condense", font.Condense);
				if(font.Extend != defaultItem.Extend)
					WriteBoolFontValue("extend", font.Extend);
				if(font.Outline != defaultItem.Outline)
					WriteBoolFontValue("outline", font.Outline);
				if(font.Shadow != defaultItem.Shadow)
					WriteBoolFontValue("shadow", font.Shadow);
				if(font.Underline != defaultItem.Underline)
					WriteUnderlineType(font.Underline);
				if(font.Script != defaultItem.Script)
					WriteVerticalAlignment(font.Script);
				WriteFontSize(font.Size);
				ColorModelInfo colorInfo = Workbook.Cache.ColorModelInfoCache[font.ColorIndex];
				if ((font.ColorIndex != defaultItem.ColorIndex || isDefaultFont) && colorInfo.ColorType != ColorType.Auto) 
					WriteColor(colorInfo, "color");
				WriteFontName(font.Name);
				if (font.FontFamily != defaultItem.FontFamily || isDefaultFont)
					WriteFontFamily(font.FontFamily);
				if (font.Charset != defaultItem.Charset)
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
				WriteStringValue("val", ConvertFontScheme(schemeStyle));
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
				WriteStringValue("val", ConvertVerticalAlignment(script));
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
		#region ExportBorders
		protected internal virtual void GenerateBordersContent() {
			WriteShStartElement("borders");
			try {
				Dictionary<int, int> borderInfoTable = this.exportStyleSheet.BorderInfoTable;
				WriteIntValue("count", borderInfoTable.Count);
				foreach (int index in borderInfoTable.Keys)
					WriteBorders(Workbook.Cache.BorderInfoCache[index]);
			}
			finally {
				WriteShEndElement();
			}
		}
		void WriteBorders(BorderInfo border) {
			BorderInfo defaultItem = Workbook.Cache.BorderInfoCache.DefaultItem;
			WriteShStartElement("border");
			try {
				if (defaultItem.DiagonalUp != border.DiagonalUp)
					WriteBoolValue("diagonalUp", border.DiagonalUp);
				if (defaultItem.DiagonalDown != border.DiagonalDown)
					WriteBoolValue("diagonalDown", border.DiagonalDown);
				if (defaultItem.Outline != border.Outline)
					WriteBoolValue("outline", border.Outline);
				if (defaultItem.LeftLineStyle != border.LeftLineStyle || defaultItem.LeftColorIndex != border.LeftColorIndex)
					WriteBorder(border.LeftLineStyle, border.LeftColorIndex, "left");
				if (defaultItem.RightLineStyle != border.RightLineStyle || defaultItem.RightColorIndex != border.RightColorIndex)
					WriteBorder(border.RightLineStyle, border.RightColorIndex, "right");
				if (defaultItem.TopLineStyle != border.TopLineStyle || defaultItem.TopColorIndex != border.TopColorIndex)
					WriteBorder(border.TopLineStyle, border.TopColorIndex, "top");
				if (defaultItem.BottomLineStyle != border.BottomLineStyle || defaultItem.BottomColorIndex != border.BottomColorIndex)
					WriteBorder(border.BottomLineStyle, border.BottomColorIndex, "bottom");
				if (defaultItem.DiagonalLineStyle != border.DiagonalLineStyle || defaultItem.DiagonalColorIndex != border.DiagonalColorIndex)
					WriteBorder(border.DiagonalLineStyle, border.DiagonalColorIndex, "diagonal");
				if (defaultItem.VerticalLineStyle != border.VerticalLineStyle || defaultItem.VerticalColorIndex != border.VerticalColorIndex)
					WriteBorder(border.VerticalLineStyle, border.VerticalColorIndex, "vertical");
				if (defaultItem.HorizontalLineStyle != border.HorizontalLineStyle || defaultItem.HorizontalColorIndex != border.HorizontalColorIndex)
					WriteBorder(border.HorizontalLineStyle, border.HorizontalColorIndex, "horizontal");
			}
			finally {
				WriteShEndElement();
			}
		}
		void WriteBorder(XlBorderLineStyle lineStyle, int colorIndex, string tag) {
			WriteShStartElement(tag);
			try {
				if (lineStyle != XlBorderLineStyle.None)
					WriteStringValue("style", borderLineStylesTable[lineStyle]);
				WriteColor(Workbook.Cache.ColorModelInfoCache[colorIndex], "color");
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
		#region ExportCellStyleFormats
		protected internal virtual void GenerateCellStyleFormatsContent() {
			WriteShStartElement("cellStyleXfs");
			try {
				CellStyleCollection cellStyles = Workbook.StyleSheet.CellStyles;
				Dictionary<int, int> cellStyleIndexTable = exportStyleSheet.CellStyleIndexTable;
				WriteIntValue("count", cellStyleIndexTable.Count);
				foreach (int index in cellStyleIndexTable.Keys)
					WriteCellStyleFormat(cellStyles[index].FormatInfo);
			}
			finally {
				WriteShEndElement();
			}
		}
		void WriteCellStyleFormat(CellStyleFormat cellStyleFormat) {
			WriteShStartElement("xf");
			try {
				WriteCellStyleFormatIds(cellStyleFormat);
				WriteStyleFormatFlags(cellStyleFormat);
				WriteAlignment(cellStyleFormat);
				WriteProtection(cellStyleFormat, CellFormatFlagsInfo.DefaultStyle);
			}
			finally {
				WriteShEndElement();
			}
		}
		void WriteCellStyleFormatIds(CellStyleFormat format) {
			WriteIntValue("numFmtId", ExportStyleSheet.GetNumberFormatId(format.NumberFormatIndex));
			WriteIntValue("fontId", ExportStyleSheet.FontInfoTable[format.FontIndex]);
			WriteIntValue("fillId", ExportStyleSheet.FillInfoHelper.GetFillId(format));
			WriteIntValue("borderId", ExportStyleSheet.BorderInfoTable[format.BorderIndex]);
		}
		void WriteStyleFormatFlags(CellFormatBase format) {
			WriteCellFormatFlagsCore(format, CellFormatFlagsInfo.DefaultStyle);
			CellFormatFlagsInfo defaultInfo = CellFormatFlagsInfo.DefaultStyle;
			if (defaultInfo.ApplyNumberFormat != format.ApplyNumberFormat)
				WriteBoolValue("applyNumberFormat", format.ApplyNumberFormat);
			if (defaultInfo.ApplyFont != format.ApplyFont)
				WriteBoolValue("applyFont", format.ApplyFont);
			if (defaultInfo.ApplyFill != format.ApplyFill)
				WriteBoolValue("applyFill", format.ApplyFill);
			if (defaultInfo.ApplyBorder != format.ApplyBorder)
				WriteBoolValue("applyBorder", format.ApplyBorder);
			if (defaultInfo.ApplyAlignment != format.ApplyAlignment)
				WriteBoolValue("applyAlignment", format.ApplyAlignment);
			if (defaultInfo.ApplyProtection != format.ApplyProtection)
				WriteBoolValue("applyProtection", format.ApplyProtection);
		}
		void WriteAlignment(FormatBase format) {
			CellAlignmentInfoCache cache = Workbook.Cache.CellAlignmentInfoCache;
			CellAlignmentInfo defaultItem = cache.DefaultItem;
			CellAlignmentInfo info = GetActualAlignment(format);
			if (cache.GetItemIndex(info) == cache.GetItemIndex(defaultItem))
				return;
			WriteAlignmentCore(info, defaultItem);
		}
		CellAlignmentInfo GetActualAlignment(FormatBase format) {
			CellFormatBase formatBase = format as CellFormatBase;
			if(formatBase != null)
				return formatBase.GetActualAlignment();
			return format.AlignmentInfo;
		}
		void WriteAlignmentCore(CellAlignmentInfo info, CellAlignmentInfo defaultItem) {
			WriteShStartElement("alignment");
			try {
				if (defaultItem.HorizontalAlignment != info.HorizontalAlignment)
					WriteStringValue("horizontal", horizontalAlignmentTable[info.HorizontalAlignment]);
				if (defaultItem.VerticalAlignment != info.VerticalAlignment)
					WriteStringValue("vertical", verticalAlignmentTable[info.VerticalAlignment]);
				if (defaultItem.TextRotation != info.TextRotation)
					WriteIntValue("textRotation", Workbook.UnitConverter.ModelUnitsToDegree(info.TextRotation));
				if (defaultItem.WrapText != info.WrapText)
					WriteBoolValue("wrapText", info.WrapText);
				if (defaultItem.Indent != info.Indent)
					WriteIntValue("indent", info.Indent);
				if (defaultItem.RelativeIndent != info.RelativeIndent)
					WriteIntValue("relativeIndent", info.RelativeIndent);
				if (defaultItem.JustifyLastLine != info.JustifyLastLine)
					WriteBoolValue("justifyLastLine", info.JustifyLastLine);
				if (defaultItem.ShrinkToFit != info.ShrinkToFit)
					WriteBoolValue("shrinkToFit", info.ShrinkToFit);
				if (defaultItem.ReadingOrder != info.ReadingOrder)
					WriteStringValue("readingOrder", readingOrderTable[info.ReadingOrder]);
			} finally {
				WriteShEndElement();
			}
		}
		void WriteProtection(CellFormatBase format, CellFormatFlagsInfo defaultItem) {
			ICellProtectionInfo protection = format.Protection;
			if(defaultItem.Locked == protection.Locked && defaultItem.Hidden == protection.Hidden)
				return;
			WriteShStartElement("protection");
			try {
				if (defaultItem.Locked != protection.Locked)
					WriteBoolValue("locked", protection.Locked);
				if (defaultItem.Hidden != protection.Hidden)
					WriteBoolValue("hidden", protection.Hidden);
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
		#region ExportCellFormats
		protected internal virtual void GenerateCellFormatsContent() {
			WriteShStartElement("cellXfs");
			try {
				Dictionary<int, int> cellFormatTable = exportStyleSheet.CellFormatTable;
				WriteIntValue("count", cellFormatTable.Count);
				foreach (int index in cellFormatTable.Keys)
					WriteCellFormat((CellFormat)Workbook.Cache.CellFormatCache[index]);
			}
			finally {
				WriteShEndElement();
			}
		}
		void WriteCellFormat(CellFormat cellFormat) {
			WriteShStartElement("xf");
			try {
				WriteCellFormatIds(cellFormat);
				WriteIntValue("xfId", exportStyleSheet.CellStyleIndexTable[cellFormat.StyleIndex]);
				WriteCellFormatFlags(cellFormat);
				WriteAlignment(cellFormat);
				WriteProtection(cellFormat, CellFormatFlagsInfo.DefaultFormat);
			}
			finally {
				WriteShEndElement();
			}
		}
		void WriteCellFormatIds(CellFormat format) {
			WriteIntValue("numFmtId", GetCellNumberFormatId(format));
			WriteIntValue("fontId", GetCellFontId(format));
			WriteIntValue("fillId", GetCellFillId(format));
			WriteIntValue("borderId", GetCellBorderId(format));
		}
		int GetCellNumberFormatId(CellFormat format) {
			int index = format.GetActualNumberFormatIndex();
			return ExportStyleSheet.GetNumberFormatId(index);
		}
		int GetCellFontId(CellFormat format) {
			int fontIndex = format.GetActualFontIndex();
			Dictionary<int, int> fontInfoTable = ExportStyleSheet.FontInfoTable;
			return fontInfoTable.ContainsKey(fontIndex) ? fontInfoTable[fontIndex] : 0;
		}
		int GetCellFillId(CellFormat format) {
			FormatBase fillOwner = format.GetActualFillOwner();
			return ExportStyleSheet.FillInfoHelper.GetFillId(fillOwner);
		}
		int GetCellBorderId(CellFormat format) {
			int borderIndex = format.GetActualBorderIndex();
			Dictionary<int, int> borderInfoTable = ExportStyleSheet.BorderInfoTable;
			return borderInfoTable.ContainsKey(borderIndex) ? borderInfoTable[borderIndex] : 0;
		}
		void WriteCellFormatFlagsCore(CellFormatBase format, CellFormatFlagsInfo defaultInfo) {
			if (defaultInfo.QuotePrefix != format.QuotePrefix)
				WriteBoolValue("quotePrefix", format.QuotePrefix);
			if (defaultInfo.PivotButton != format.PivotButton)
				WriteBoolValue("pivotButton", format.PivotButton);
		}
		void WriteCellFormatFlags(CellFormat format) {
			WriteCellFormatFlagsCore(format, CellFormatFlagsInfo.DefaultFormat);
			WriteApplyValue("applyNumberFormat", format.ApplyNumberFormat);
			WriteApplyValue("applyFont", format.ApplyFont);
			WriteApplyValue("applyFill", format.ApplyFill);
			WriteApplyValue("applyBorder", format.ApplyBorder);
			WriteApplyValue("applyAlignment", format.ApplyAlignment);
			WriteApplyValue("applyProtection", format.ApplyProtection);
		}
		void WriteApplyValue(string nameAttribute, bool value) {
			if (value)
				WriteBoolValue(nameAttribute, value);
		}
		#endregion
		#region ExportCellStyles
		protected internal virtual void GenerateCellStylesContent() {
			WriteShStartElement("cellStyles");
			try {
				CellStyleCollection cellStyles = Workbook.StyleSheet.CellStyles;
				Dictionary<int, int> cellStyleIndexTable = exportStyleSheet.CellStyleIndexTable;
				WriteIntValue("count", cellStyleIndexTable.Count);
				int xfId = 0;
				foreach (int index in cellStyleIndexTable.Keys) {
					WriteCellStyle(cellStyles[index], xfId);
					xfId++;
				}
			}
			finally {
				WriteShEndElement();
			}
		}
		void WriteCellStyle(CellStyleBase cellStyle, int xfId) {
			WriteShStartElement("cellStyle");
			try {
				if (!String.IsNullOrEmpty(cellStyle.Name))
					WriteStringValue("name", EncodeXmlChars(cellStyle.Name));
				WriteIntValue("xfId", xfId);
				BuiltInCellStyle builtInCellStyle = cellStyle as BuiltInCellStyle;
				if (builtInCellStyle != null) {
					WriteIntValue("builtinId", builtInCellStyle.BuiltInId);
					if (cellStyle.IsHidden)
						WriteBoolValue("hidden", true);
					if (builtInCellStyle.CustomBuiltIn)
						WriteBoolValue("customBuiltin", true);
				}
				OutlineCellStyle outlineInCellStyle = cellStyle as OutlineCellStyle;
				if (outlineInCellStyle != null) {
					WriteIntValue("builtinId", outlineInCellStyle.BuiltInId);
					WriteIntValue("iLevel", outlineInCellStyle.OutlineLevel - 1);
					if (cellStyle.IsHidden)
						WriteBoolValue("hidden", true);
					if (outlineInCellStyle.CustomBuiltIn)
						WriteBoolValue("customBuiltin", true);
				}
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
		#region ExportDifferentialFormats
		protected internal virtual void GenerateDifferentialFormatsContent() {
			WriteShStartElement("dxfs");
			try {
				WriteIntValue("count", exportStyleSheet.DifferentialFormatTable.Count);
				WriteDifferentialFormats(exportStyleSheet.DifferentialFormatTable);
			}
			finally {
				WriteShEndElement();
			}
		}
		void WriteDifferentialFormats(Dictionary<int, int> table) {
			foreach (int index in table.Keys)
				WriteDifferentialFormat((DifferentialFormat)Workbook.Cache.CellFormatCache[index]);
		}
		void WriteDifferentialFormat(DifferentialFormat format) {
			WriteShStartElement("dxf");
			try {
				WriteDifferentialFont(format);
				WriteDifferentialNumberFormat(format);
				WriteDifferentialFill(format);
				WriteDifferentialAlignment(format);
				WriteDifferentialBorder(format);
				WriteDifferentialProtection(format);
			}
			finally {
				WriteShEndElement();
			}
		}
		#region WriteDifferentialNumberFormat
		void WriteDifferentialNumberFormat(DifferentialFormat format) {
			if (format.MultiOptionsInfo.ApplyNumberFormat)
				WriteDifferentialNumberFormat(format.NumberFormatId, format.FormatString);
		}
		void WriteDifferentialNumberFormat(int id, string formatCode) {
			id = ExportStyleSheet.GetNumberFormatId(id);
			WriteShStartElement("numFmt");
			try {
				WriteIntValue("numFmtId", id);
				WriteStringValue("formatCode", id != NumberFormatCollection.DefaultItemIndex ? formatCode : "General");
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion 
		#region WriteDifferentialFont
		void WriteDifferentialFont(DifferentialFormat format) {
			if (format.MultiOptionsInfo.ApplyFontNone)
				return;
			WriteShStartElement("font");
			try {
				if(format.MultiOptionsInfo.ApplyFontBold)
					WriteBoolFontValue("b", format.Font.Bold);
				if(format.MultiOptionsInfo.ApplyFontItalic)
					WriteBoolFontValue("i", format.Font.Italic);
				if(format.MultiOptionsInfo.ApplyFontStrikeThrough)
					WriteBoolFontValue("strike", format.Font.StrikeThrough);
				if(format.MultiOptionsInfo.ApplyFontCondense)
					WriteBoolFontValue("condense", format.Font.Condense);
				if(format.MultiOptionsInfo.ApplyFontExtend)
					WriteBoolFontValue("extend", format.Font.Extend);
				if(format.MultiOptionsInfo.ApplyFontOutline)
					WriteBoolFontValue("outline", format.Font.Outline);
				if(format.MultiOptionsInfo.ApplyFontShadow)
					WriteBoolFontValue("shadow", format.Font.Shadow);
				if(format.MultiOptionsInfo.ApplyFontUnderline)
					WriteUnderlineType(format.Font.Underline);
				if(format.MultiOptionsInfo.ApplyFontScript)
					WriteVerticalAlignment(format.Font.Script);
				if (format.MultiOptionsInfo.ApplyFontSize)
					WriteFontSize(format.Font.Size);
				if(format.MultiOptionsInfo.ApplyFontColor)
					WriteColor(Workbook.Cache.ColorModelInfoCache[format.FontInfo.ColorIndex], "color");
				if(format.MultiOptionsInfo.ApplyFontName)
					WriteFontName(format.Font.Name);
				if (format.MultiOptionsInfo.ApplyFontFamily)
					WriteFontFamily(format.Font.FontFamily);
				if (format.MultiOptionsInfo.ApplyFontCharset)
					WriteCharset(format.Font.Charset);
				if (format.MultiOptionsInfo.ApplyFontSchemeStyle)
					WriteSchemeStyle(format.Font.SchemeStyle);
			} finally {
				WriteShEndElement();
			}
		}
		#endregion
		#region WriteDifferentialFill
		void WriteDifferentialFill(DifferentialFormat format) {
			bool hasGradientFill = format.Fill.FillType == ModelFillType.Gradient;
			if (format.MultiOptionsInfo.ApplyFillNone && !hasGradientFill)
				return;
			WriteShStartElement("fill");
			try {
				if (hasGradientFill)
					WriteGradientFill(format.FillIndex, format.GradientStopInfoCollection);
				else
					WritePatternFill(format);
			} finally {
				WriteShEndElement();
			}
		}
		void WritePatternFill(DifferentialFormat format) {
			WriteShStartElement("patternFill");
			try {
				if (format.MultiOptionsInfo.ApplyFillPatternType)
					WriteStringValue("patternType", patternTypeTable[format.Fill.PatternType]);
				if (format.MultiOptionsInfo.ApplyFillForeColor)
					WriteColor(Workbook.Cache.ColorModelInfoCache[format.FillInfo.ForeColorIndex], "fgColor");
				if (format.MultiOptionsInfo.ApplyFillBackColor)
					WriteColor(Workbook.Cache.ColorModelInfoCache[format.FillInfo.BackColorIndex], "bgColor");
			} finally {
				WriteShEndElement();
			}
		}
		#endregion
		#region WriteDifferentialAlignment
		void WriteDifferentialAlignment(DifferentialFormat format) {
			if (format.MultiOptionsInfo.ApplyAlignmentNone)
				return;
			WriteShStartElement("alignment");
			try {
				if (format.MultiOptionsInfo.ApplyAlignmentHorizontal)
					WriteStringValue("horizontal", horizontalAlignmentTable[format.Alignment.Horizontal]);
				if (format.MultiOptionsInfo.ApplyAlignmentVertical)
					WriteStringValue("vertical", verticalAlignmentTable[format.Alignment.Vertical]);
				if (format.MultiOptionsInfo.ApplyAlignmentTextRotation)
					WriteIntValue("textRotation", Workbook.UnitConverter.ModelUnitsToDegree(format.Alignment.TextRotation));
				if (format.MultiOptionsInfo.ApplyAlignmentWrapText)
					WriteBoolValue("wrapText", format.Alignment.WrapText);
				if (format.MultiOptionsInfo.ApplyAlignmentIndent)
					WriteIntValue("indent", format.Alignment.Indent);
				if (format.MultiOptionsInfo.ApplyAlignmentRelativeIndent)
					WriteIntValue("relativeIndent", format.Alignment.RelativeIndent);
				if (format.MultiOptionsInfo.ApplyAlignmentJustifyLastLine)
					WriteBoolValue("justifyLastLine", format.Alignment.JustifyLastLine);
				if (format.MultiOptionsInfo.ApplyAlignmentShrinkToFit)
					WriteBoolValue("shrinkToFit", format.Alignment.ShrinkToFit);
				if (format.MultiOptionsInfo.ApplyAlignmentReadingOrder)
					WriteStringValue("readingOrder", readingOrderTable[format.Alignment.ReadingOrder]);
			} finally {
				WriteShEndElement();
			}
		}
		#endregion
		#region WriteDifferentialBorder
		void WriteDifferentialBorder(DifferentialFormat format) {
			if (format.BorderOptionsInfo.ApplyNone)
				return;
			WriteShStartElement("border");
			try {
				if (format.BorderOptionsInfo.ApplyDiagonalUp)
					WriteBoolValue("diagonalUp", format.BorderInfo.DiagonalUp);
				if (format.BorderOptionsInfo.ApplyDiagonalDown)
					WriteBoolValue("diagonalDown", format.BorderInfo.DiagonalDown);
				if (format.BorderOptionsInfo.ApplyOutline)
					WriteBoolValue("outline", format.Border.Outline);
				if (format.BorderOptionsInfo.ApplyLeftLineStyle || format.BorderOptionsInfo.ApplyLeftColor)
					WriteBorder(format.Border.LeftLineStyle, format.BorderInfo.LeftColorIndex, "left");
				if (format.BorderOptionsInfo.ApplyRightLineStyle || format.BorderOptionsInfo.ApplyRightColor)
					WriteBorder(format.Border.RightLineStyle, format.BorderInfo.RightColorIndex, "right");
				if (format.BorderOptionsInfo.ApplyTopLineStyle || format.BorderOptionsInfo.ApplyTopColor)
					WriteBorder(format.Border.TopLineStyle, format.BorderInfo.TopColorIndex, "top");
				if (format.BorderOptionsInfo.ApplyBottomLineStyle || format.BorderOptionsInfo.ApplyBottomColor)
					WriteBorder(format.Border.BottomLineStyle, format.BorderInfo.BottomColorIndex, "bottom");
				if (format.BorderOptionsInfo.ApplyDiagonalLineStyle || format.BorderOptionsInfo.ApplyDiagonalColor)
					WriteBorder(format.BorderInfo.DiagonalLineStyle, format.BorderInfo.DiagonalColorIndex, "diagonal");
				if (format.BorderOptionsInfo.ApplyVerticalLineStyle || format.BorderOptionsInfo.ApplyVerticalColor)
					WriteBorder(format.Border.VerticalLineStyle, format.BorderInfo.VerticalColorIndex, "vertical");
				if (format.BorderOptionsInfo.ApplyHorizontalLineStyle || format.BorderOptionsInfo.ApplyHorizontalColor)
					WriteBorder(format.Border.HorizontalLineStyle, format.BorderInfo.HorizontalColorIndex, "horizontal");
			} finally {
				WriteShEndElement();
			}
		}
		#endregion
		#region WriteDifferentialProtection
		void WriteDifferentialProtection(DifferentialFormat format) {
			if (!format.MultiOptionsInfo.ApplyProtectionHidden && !format.MultiOptionsInfo.ApplyProtectionLocked)
				return;
			WriteShStartElement("protection");
			try {
				if (format.MultiOptionsInfo.ApplyProtectionLocked)
					WriteBoolValue("locked", format.Protection.Locked);
				if (format.MultiOptionsInfo.ApplyProtectionHidden)
					WriteBoolValue("hidden", format.Protection.Hidden);
			} finally {
				WriteShEndElement();
			}
		}
		#endregion
		#endregion
		#region ExportTableStyles
		protected internal virtual void GenerateTableStylesContent() {
			TableStyleCollection tableStyles = Workbook.StyleSheet.TableStyles;
			IList<ImportExportTableStyleInfo> tableStyleInfoes = exportStyleSheet.GetCustomTableStyleInfoes(tableStyles);
			WriteShStartElement("tableStyles");
			try {
				int count = tableStyleInfoes.Count;
				WriteIntValue("count", count);
				WriteDefaultTableStyleName("defaultTableStyle", tableStyles.CachedDefaultTableStyleName);
				WriteDefaultTableStyleName("defaultPivotStyle", tableStyles.CachedDefaultPivotStyleName);
				for (int i = 0; i < count; i++)
					WriteTableStyleInfo(tableStyleInfoes[i]);
			} finally {
				WriteShEndElement();
			}
		}
		void WriteTableStyleInfo(ImportExportTableStyleInfo info) {
			WriteShStartElement("tableStyle");
			try {
				WriteStringValue("name", info.Name);
				WriteBoolValue("table", info.IsTable, true);
				WriteBoolValue("pivot", info.IsPivot, true);
				Dictionary<int, ImportExportTableStyleElementInfo> registeredTableStyleElementTable = info.RegisteredTableStyleElementTable;
				int count = registeredTableStyleElementTable.Count;
				WriteIntValue("count", count);
				foreach (int elementIndex in registeredTableStyleElementTable.Keys)
					WriteTableStyleElementInfo(elementIndex, registeredTableStyleElementTable[elementIndex]);
			} finally {
				WriteShEndElement();
			}
		}
		void WriteDefaultTableStyleName(string attr, string name) {
			WriteStringValue(attr, name, !TableStyleName.CheckDefaultStyleName(name));
		}
		void WriteTableStyleElementInfo(int elementIndex, ImportExportTableStyleElementInfo info) {
			WriteShStartElement("tableStyleElement");
			try {
				WriteStringValue("type", tableStyleElementTypeTable[elementIndex]);
				int size = info.StripeSize.Value;
				WriteIntValue("size", size, size != StripeSizeInfo.DefaultValue);
				int dxfId = info.DxfId.Value;
				WriteIntValue("dxfId", dxfId, dxfId >= 0);
			} finally {
				WriteShEndElement();
			}
		}
		#endregion
		#region ExportColors
		protected internal virtual void GenerateColorsContent() {
			bool exportIndexedColors = Workbook.StyleSheet.Palette.IsCustomIndexedColorTable;
			bool exportCustomColors = Workbook.StyleSheet.CustomColors.Count > 0;
			if (exportIndexedColors || exportCustomColors) {
				WriteShStartElement("colors");
				try {
					if (exportIndexedColors)
						GenerateIndexedColorsContent();
					if (exportCustomColors)
						GenerateCustomColorsContent();
				}
				finally {
					WriteShEndElement();
				}
			}
		}
		protected internal virtual void GenerateIndexedColorsContent() {
			WriteShStartElement("indexedColors");
			try {
				int count = Palette.DefaultForegroundColorIndex - 1;
				Palette palette = Workbook.StyleSheet.Palette;
				for (int i = 0; i < count; i++)
					WriteRgbColor("rgbColor", palette[i]);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual void GenerateCustomColorsContent() {
			WriteShStartElement("mruColors");
			try {
				List<Color> customColors = Workbook.StyleSheet.CustomColors;
				int count = customColors.Count;
				for (int i = 0; i < count; i++)
					WriteRgbColor("color", customColors[i]);
			}
			finally {
				WriteShEndElement();
			}
		}
		void WriteRgbColor(string nameAttribute, Color value) {
			WriteShStartElement(nameAttribute);
			try {
				WriteStringValue("rgb", ConvertARGBColorToString(value));
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
		#endregion
	}
}
