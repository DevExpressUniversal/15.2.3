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

#define EXPORT_STYLE_EXTENSIONS
#define EXPORT_XF_EXTENSIONS
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using DevExpress.Utils;
using DevExpress.Office.Services;
using DevExpress.XtraSpreadsheet.Model;
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
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
using DevExpress.Office.Model;
using DevExpress.Office.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Export.Xls {
	public class XlsStylesExporter : XlsExporterBase {
		#region Fields
		int xfCrc;
		int xfCount;
		List<XlsCommandFormatExt> xfExtensions = new List<XlsCommandFormatExt>();
		#endregion
		public XlsStylesExporter(BinaryWriter writer, DocumentModel documentModel, ExportXlsStyleSheet exportStyleSheet)
			: base(writer, documentModel, exportStyleSheet) {
		}
		public override void WriteContent() {
			WriteFonts();
			WriteNumberFormats();
			WriteXFs();
			WriteXFExtensions();
			WriteDifferentialFormats();
			WriteStyles();
			WriteTableStyles();
			WritePalette();
		}
		protected void WriteFonts() {
			XlsCommandFont command = new XlsCommandFont();
			List<RunFontInfo> fontList = ExportStyleSheet.FontList;
			int count = fontList.Count;
			for (int i = 0; i < count; i++) {
				RunFontInfo fontInfo = fontList[i].Clone();
				fontInfo.ColorIndex = GetFontColorIndex(fontInfo.ColorIndex);
				command.SetFontInfo(fontInfo);
				command.Write(StreamWriter);
			}
			Dictionary<RunFontInfo, int> chartFontTable = ExportStyleSheet.ChartFontTable;
			foreach (RunFontInfo fontInfo in chartFontTable.Keys) {
				command.SetFontInfo(fontInfo);
				command.Write(StreamWriter);
			}
		}
		protected void WriteNumberFormats() {
			List<int> customNumberFormats = ExportStyleSheet.GetCustomNumberFormats();
			XlsCommandNumberFormat command = new XlsCommandNumberFormat();
			for (int i = 0; i < customNumberFormats.Count; ++i) {
				int index = customNumberFormats[i];
				NumberFormat format = DocumentModel.Cache.NumberFormatCache[index];
				command.FormatId = index;
				command.FormatCode = ConvertFormatCode(format.FormatCode);
				command.Write(StreamWriter);
			}
		}
		protected void WriteXFs() {
			xfCount = 0;
			foreach (int formatIndex in ExportStyleSheet.XFList) {
				CellFormatBase format = (CellFormatBase)DocumentModel.Cache.CellFormatCache[formatIndex];
				XlsCommandFormat command = new XlsCommandFormat();
				XlsContentXF content = command.Content;
				CellStyleFormat styleFormat = format as CellStyleFormat;
				content.IsStyleFormat = (styleFormat != null) ? true : false;
				if (styleFormat != null)
					content.StyleId = 0xfff;
				else {
					CellFormat cellFormat = (CellFormat)format;
					content.StyleId = ExportStyleSheet.GetXFIndex(cellFormat.Style);
				}
				CellFormatFlagsInfo flagsInfo = format.GetActualFormatFlags();
				content.IsHidden = flagsInfo.Hidden;
				content.IsLocked = flagsInfo.Locked;
				content.ApplyAlignment = format.ApplyAlignment;
				content.ApplyBorder = format.ApplyBorder;
				content.ApplyFill = format.ApplyFill;
				content.ApplyFont = format.ApplyFont;
				content.ApplyNumberFormat = format.ApplyNumberFormat;
				content.ApplyProtection = format.ApplyProtection;
				BorderInfo border = DocumentModel.Cache.BorderInfoCache[format.GetActualBorderIndex()];
				Palette palette = DocumentModel.StyleSheet.Palette;
				ThemeDrawingColorCollection colors = DocumentModel.OfficeTheme.Colors;
				ColorModelInfoCache cache = DocumentModel.Cache.ColorModelInfoCache;
				content.BorderTopColorIndex = palette.GetColorIndex(colors, cache, border.TopColorIndex, true);
				content.BorderBottomColorIndex = palette.GetColorIndex(colors, cache, border.BottomColorIndex, true);
				content.BorderLeftColorIndex = palette.GetColorIndex(colors, cache, border.LeftColorIndex, true);
				content.BorderRightColorIndex = palette.GetColorIndex(colors, cache, border.RightColorIndex, true);
				content.BorderDiagonalColorIndex = palette.GetColorIndex(colors, cache, border.DiagonalColorIndex, true);
				content.BorderTopLineStyle = content.BorderTopColorIndex == 0 ? XlBorderLineStyle.None : border.TopLineStyle;
				content.BorderBottomLineStyle = content.BorderBottomColorIndex == 0 ? XlBorderLineStyle.None : border.BottomLineStyle;
				content.BorderLeftLineStyle = content.BorderLeftColorIndex == 0 ? XlBorderLineStyle.None : border.LeftLineStyle;
				content.BorderRightLineStyle = content.BorderRightColorIndex == 0 ? XlBorderLineStyle.None : border.RightLineStyle;
				XlBorderLineStyle diagonalBorderLineStyle = format.GetBorderDiagonalLineStyle();
				content.BorderDiagonalLineStyle = content.BorderDiagonalColorIndex == 0 ? XlBorderLineStyle.None : diagonalBorderLineStyle;
				content.BorderDiagonalDown = border.DiagonalDownLineStyle != XlBorderLineStyle.None;
				content.BorderDiagonalUp = border.DiagonalUpLineStyle != XlBorderLineStyle.None;
				FillInfo fill = format.GetActualFillOwner().FillInfo;
				content.FillBackColorIndex = palette.GetColorIndex(DocumentModel.OfficeTheme.Colors, DocumentModel.Cache.ColorModelInfoCache, fill.BackColorIndex, false);
				content.FillForeColorIndex = palette.GetColorIndex(DocumentModel.OfficeTheme.Colors, DocumentModel.Cache.ColorModelInfoCache, fill.ForeColorIndex, true);
				content.FillPatternType = (XlPatternType)fill.PatternType;
				content.FontId = ExportStyleSheet.GetFontIndex(format.GetActualFontIndex());
				content.NumberFormatId = ExportStyleSheet.GetNumberFormatId(format.GetActualNumberFormatIndex());
				CellAlignmentInfo alignment = format.GetActualAlignment();
				content.HorizontalAlignment = alignment.HorizontalAlignment;
				content.VerticalAlignment = alignment.VerticalAlignment;
				content.Indent = Math.Min(alignment.Indent, (byte)15);
				content.PivotButton = format.PivotButton;
				content.QuotePrefix = format.QuotePrefix;
				content.ReadingOrder = alignment.ReadingOrder;
				content.ShrinkToFit = alignment.ShrinkToFit;
				content.TextRotation = DocumentModel.UnitConverter.ModelUnitsToDegree(alignment.TextRotation);
				content.WrapText = alignment.WrapText;
				content.HasExtension = PrepareXFExtension(format, xfCount);
				content.CrcValue = xfCrc;
				command.Write(StreamWriter);
				xfCrc = content.CrcValue;
				int maxCount = (styleFormat != null) ? XlsDefs.MaxStyleXFCount : XlsDefs.MaxCellXFCount;
				xfCount++;
				if (xfCount >= maxCount) {
					string message = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_XFTableHasBeenTruncated);
					LogMessage(LogCategory.Warning, message);
					return;
				}
			}
		}
		protected void WriteXFCrc() {
			XlsCommandFormatCrc command = new XlsCommandFormatCrc();
			command.XFCount = xfCount;
			command.XFCRC = xfCrc;
			command.Write(StreamWriter);
		}
		protected void WriteXFExtensions() {
			if ((xfExtensions.Count > 0) && (xfCount <= XlsDefs.MaxXFCount)) {
				WriteXFCrc();
				foreach (XlsCommandFormatExt command in xfExtensions)
					command.Write(StreamWriter);
			}
		}
		protected bool PrepareXFExtension(CellFormatBase format, int xfIndex) {
			XlsCommandFormatExt command = new XlsCommandFormatExt();
			command.XFIndex = xfIndex;
			FillXFExtProperties(format, command.Properties);
			bool result = command.Properties.Count > 0;
			if (result)
				xfExtensions.Add(command);
			return result;
		}
		void WriteDifferentialFormats() {
			Dictionary<int, int> differentialFormatTable = ExportStyleSheet.DifferentialFormatTable;
			foreach (int index in differentialFormatTable.Keys)
				WriteDifferentialFormat((DifferentialFormat)DocumentModel.Cache.CellFormatCache[index]);
		}
		void WriteDifferentialFormat(DifferentialFormat format) {
			XlsCommandDifferentialFormat command = new XlsCommandDifferentialFormat();
			command.AssignDifferentialFormat(XlsDifferentialFormatInfo.FromFormat(format));
			command.Write(StreamWriter);
		}
		protected void WriteStyles() {
			List<CellStyleBase> styles = ExportStyleSheet.CellStyles;
			int count = styles.Count;
			for (int i = 0; i < count; i++) {
				CellStyleBase style = styles[i];
				BuiltInCellStyle builtInStyle = style as BuiltInCellStyle;
				if (ExportStyleSheet.ClipboardMode && (builtInStyle == null || builtInStyle.BuiltInId != 0))
					continue;
				if (!style.IsRegistered) {
					if (builtInStyle == null)
						continue;
					if (builtInStyle.BuiltInId < 3)
						continue;
					if (builtInStyle.BuiltInId > 7 && builtInStyle.BuiltInId < 10)
						continue;
					if (builtInStyle.BuiltInId > 11 && builtInStyle.BuiltInId < 15)
						continue;
				}
				XlsCommandCellStyle command = CreateXlsCommandCellStyle();
				bool isBuiltIn = false;
				bool customBuiltIn = false;
				int builtInId = 0;
				if (builtInStyle != null) {
					isBuiltIn = true;
					builtInId = builtInStyle.BuiltInId;
					customBuiltIn = builtInStyle.CustomBuiltIn;
				} else {
					OutlineCellStyle outlineStyle = style as OutlineCellStyle;
					if (outlineStyle != null) {
						isBuiltIn = true;
						builtInId = outlineStyle.BuiltInId;
						customBuiltIn = outlineStyle.CustomBuiltIn;
						command.OutlineLevel = outlineStyle.OutlineLevel;
					}
				}
				command.StyleFormatId = ExportStyleSheet.GetXFIndex(style);
				command.IsBuiltIn = isBuiltIn && builtInId <= 9;
				command.IsHidden = style.IsHidden;
				command.BuiltInId = builtInId;
				command.StyleName = style.Name;
				command.Write(StreamWriter);
#if EXPORT_STYLE_EXTENSIONS
				XlsCommandCellStyleExt commandExt = new XlsCommandCellStyleExt();
				commandExt.IsHidden = command.IsHidden;
				commandExt.IsBuiltIn = isBuiltIn;
				commandExt.BuiltInId = builtInId;
				commandExt.CustomBuiltIn = customBuiltIn;
				commandExt.OutlineLevel = command.OutlineLevel;
				commandExt.StyleName = style.Name;
				if (isBuiltIn)
					commandExt.Category = GetStyleCategory(builtInId);
				CellFormatBase format = (CellFormatBase)DocumentModel.Cache.CellFormatCache[style.FormatIndex];
				FillXFProperties(format, commandExt.Properties);
				if (isBuiltIn || (commandExt.Properties.Count > 0))
					commandExt.Write(StreamWriter);
#endif
			}
		}
		void WriteTableStyles() {
			XlsCommandTableStyles command = new XlsCommandTableStyles();
			TableStyleCollection tableStyles = DocumentModel.StyleSheet.TableStyles;
			IList<ImportExportTableStyleInfo> tableStyleInfoes = ExportStyleSheet.GetCustomTableStyleInfoes(tableStyles);
			int tableStyleInfoCount = tableStyleInfoes.Count;
			command.CustomTableStylesCount = tableStyleInfoCount;
			string cachedDefaultTableStyleName = tableStyles.CachedDefaultTableStyleName;
			if (!TableStyleName.CheckDefaultStyleName(cachedDefaultTableStyleName))
				command.DefaultTableStyleName = cachedDefaultTableStyleName;
			string cachedDefaultPivotStyleName = tableStyles.CachedDefaultPivotStyleName;
			if (!TableStyleName.CheckDefaultStyleName(cachedDefaultPivotStyleName))
				command.DefaultPivotTableStyleName = cachedDefaultPivotStyleName;
			command.Write(StreamWriter);
			for (int i = 0; i < tableStyleInfoCount; i++)
				WriteTableStyleInfo(tableStyleInfoes[i]);
		}
		void WriteTableStyleInfo(ImportExportTableStyleInfo info) {
			XlsCommandTableStyle command = new XlsCommandTableStyle();
			command.TableStyleName = info.Name;
			command.IsTable = info.IsTable;
			command.IsPivot = info.IsPivot;
			Dictionary<int, ImportExportTableStyleElementInfo> registeredTableStyleElementTable = info.RegisteredTableStyleElementTable;
			command.TableStyleElementRecords = registeredTableStyleElementTable.Count;
			command.Write(StreamWriter);
			foreach (int elementIndex in registeredTableStyleElementTable.Keys)
				WriteTableStyleElementInfo(elementIndex, registeredTableStyleElementTable[elementIndex]);
		}
		void WriteTableStyleElementInfo(int elementIndex, ImportExportTableStyleElementInfo info) {
			XlsCommandTableStyleElement command = new XlsCommandTableStyleElement();
			command.TypeIndex = elementIndex;
			int size = info.StripeSize.Value;
			if (size != StripeSizeInfo.DefaultValue)
				command.StripeSize = size;
			int dxfId = info.DxfId.Value;
			if (dxfId >= 0)
				command.DxfId = dxfId;
			command.Write(StreamWriter);
		}
		protected void WritePalette() {
			Palette palette = DocumentModel.StyleSheet.Palette;
			if (!palette.IsCustomIndexedColorTable)
				return;
			XlsCommandPalette command = new XlsCommandPalette();
			for (int i = 0; i < 56; i++)
				command.Colors.Add(palette[i + Palette.BuiltInColorsCount]);
			command.Write(StreamWriter);
		}
		protected virtual XlsCommandCellStyle CreateXlsCommandCellStyle() {
			return new XlsCommandCellStyle();
		}
		string ConvertFormatCode(string code) {
			if (string.IsNullOrEmpty(code))
				return "GENERAL";
			return code.Replace("&quot;", "\"");
		}
		int GetFontColorIndex(int index) {
			if (!DocumentModel.Cache.ColorModelInfoCache.IsIndexValid(index))
				return Palette.FontAutomaticColorIndex;
			ColorModelInfo colorInfo = DocumentModel.Cache.ColorModelInfoCache[index];
			ColorModelInfo defaultItem = DocumentModel.Cache.ColorModelInfoCache.DefaultItem;
			if (Object.ReferenceEquals(defaultItem, colorInfo))
				return Palette.FontAutomaticColorIndex;
			if (colorInfo.ColorType == ColorType.Auto)
				return Palette.FontAutomaticColorIndex;
			if (colorInfo.ColorType == ColorType.Index) {
				if (!DocumentModel.StyleSheet.Palette.IsValidColorIndex(colorInfo.ColorIndex))
					return Palette.FontAutomaticColorIndex;
				return colorInfo.ColorIndex;
			}
			Color color = colorInfo.ToRgb(DocumentModel.StyleSheet.Palette, DocumentModel.OfficeTheme.Colors);
			return DocumentModel.StyleSheet.Palette.GetNearestColorIndex(color);
		}
		#region Extended properties
		protected void FillXFExtProperties(CellFormatBase format, XFExtProperties properties) {
#if EXPORT_XF_EXTENSIONS
			ColorModelInfoCache colorCache = DocumentModel.Cache.ColorModelInfoCache;
			if (format.Fill.FillType == ModelFillType.Pattern) {
				FillInfo fill = format.GetActualFillOwner().FillInfo;
				AddColorProperty(properties, colorCache[fill.ForeColorIndex], typeof(XFExtPropForegroundColor));
				AddColorProperty(properties, colorCache[fill.BackColorIndex], typeof(XFExtPropBackgroundColor));
			}
			else {
				GradientFillInfo fillInfo = DocumentModel.Cache.GradientFillInfoCache[format.GradientFillInfoIndex];
				XFExtPropGradient prop = new XFExtPropGradient();
				prop.Gradient.Info = fillInfo;
				GradientStopInfoCollection stops = format.GradientStopInfoCollection;
				int count = stops.Count;
				for (int i = 0; i < count; i++) {
					DevExpress.XtraSpreadsheet.Model.GradientStopInfo info = DocumentModel.Cache.GradientStopInfoCache[stops[i]];
					XFGradStop stop = new XFGradStop();
					stop.ColorInfo.CopyFrom(info.GetColorModelInfo(DocumentModel));
					stop.Position = info.Position;
					prop.Stops.Add(stop);
				}
				properties.Add(prop);
			}
			BorderInfo border = DocumentModel.Cache.BorderInfoCache[format.GetActualBorderIndex()];
			AddColorProperty(properties, colorCache[border.TopColorIndex], typeof(XFExtPropTopBorderColor));
			AddColorProperty(properties, colorCache[border.BottomColorIndex], typeof(XFExtPropBottomBorderColor));
			AddColorProperty(properties, colorCache[border.LeftColorIndex], typeof(XFExtPropLeftBorderColor));
			AddColorProperty(properties, colorCache[border.RightColorIndex], typeof(XFExtPropRightBorderColor));
			AddColorProperty(properties, colorCache[border.DiagonalColorIndex], typeof(XFExtPropDiagonalBorderColor));
			RunFontInfo fontInfo = DocumentModel.Cache.FontInfoCache[format.GetActualFontIndex()];
			AddColorProperty(properties, colorCache[fontInfo.ColorIndex], typeof(XFExtPropTextColor));
			CellAlignmentInfo alignment = format.GetActualAlignment();
			if (alignment.Indent > 15) {
				XFExtPropIndentLevel prop = new XFExtPropIndentLevel();
				prop.Value = Math.Min((int)alignment.Indent, 250);
				properties.Add(prop);
			}
#endif
		}
		protected void FillXFProperties(CellFormatBase format, XFProperties properties) {
#if EXPORT_STYLE_EXTENSIONS
			ColorModelInfoCache colorCache = DocumentModel.Cache.ColorModelInfoCache;
			if (format.ApplyFill) {
				if (format.Fill.FillType == ModelFillType.Pattern) {
					FillInfo fill = DocumentModel.Cache.FillInfoCache[format.FillIndex];
					AddColorProperty(properties, colorCache[fill.ForeColorIndex], typeof(XFPropForegroundColor));
					AddColorProperty(properties, colorCache[fill.BackColorIndex], typeof(XFPropBackgroundColor));
				}
				else {
					GradientFillInfo fillInfo = DocumentModel.Cache.GradientFillInfoCache[format.GradientFillInfoIndex];
					XFPropGradient prop = new XFPropGradient();
					prop.Gradient.Info = fillInfo;
					properties.Add(prop);
					GradientStopInfoCollection stops = format.GradientStopInfoCollection;
					int count = stops.Count;
					for (int i = 0; i < count; i++) {
						DevExpress.XtraSpreadsheet.Model.GradientStopInfo info = DocumentModel.Cache.GradientStopInfoCache[stops[i]];
						XFPropGradientStop stopProp = new XFPropGradientStop();
						stopProp.ColorInfo.CopyFrom(info.GetColorModelInfo(DocumentModel));
						stopProp.Position = info.Position;
						properties.Add(stopProp);
					}
				}
			}
			if (format.ApplyBorder) {
				BorderInfo border = DocumentModel.Cache.BorderInfoCache[format.BorderIndex];
				AddBorderProperty(properties, colorCache[border.TopColorIndex], border.TopLineStyle, typeof(XFPropTopBorder));
				AddBorderProperty(properties, colorCache[border.BottomColorIndex], border.BottomLineStyle, typeof(XFPropBottomBorder));
				AddBorderProperty(properties, colorCache[border.LeftColorIndex], border.LeftLineStyle, typeof(XFPropLeftBorder));
				AddBorderProperty(properties, colorCache[border.RightColorIndex], border.RightLineStyle, typeof(XFPropRightBorder));
				AddBorderProperty(properties, colorCache[border.DiagonalColorIndex], border.DiagonalLineStyle, typeof(XFPropDiagonalBorder));
				AddBorderPropertyExt(properties, colorCache[border.VerticalColorIndex], border.VerticalLineStyle, typeof(XFPropVerticalBorder));
				AddBorderPropertyExt(properties, colorCache[border.HorizontalColorIndex], border.HorizontalLineStyle, typeof(XFPropHorizontalBorder));
			}
			if (format.ApplyFont) {
				RunFontInfo fontInfo = DocumentModel.Cache.FontInfoCache[format.FontIndex];
				AddColorProperty(properties, colorCache[fontInfo.ColorIndex], typeof(XFPropTextColor));
				{
					XFPropFontScheme prop = new XFPropFontScheme();
					prop.Value = fontInfo.SchemeStyle;
					properties.Add(prop);
				}
			}
			if (format.ApplyAlignment) {
				CellAlignmentInfo cellAlignmentInfo = format.AlignmentInfo;
				{
					XFPropRelativeIndentation prop = new XFPropRelativeIndentation();
					prop.Value = cellAlignmentInfo.RelativeIndent;
					properties.Add(prop);
				}
				if (cellAlignmentInfo.JustifyLastLine) {
					XFPropHorizontalAlignment propAlign = new XFPropHorizontalAlignment();
					propAlign.Value = XlHorizontalAlignment.Distributed;
					properties.Add(propAlign);
					XFPropTextJustifyDistributed prop = new XFPropTextJustifyDistributed();
					prop.Value = cellAlignmentInfo.JustifyLastLine;
					properties.Add(prop);
				}
			}
#endif
		}
		void AddColorProperty(XFExtProperties properties, ColorModelInfo color, Type propType) {
			if (IsExtColor(color)) {
				XFExtPropFullColorBase prop = XFExtPropertyFactory.CreateProperty(propType) as XFExtPropFullColorBase;
				if (prop != null) {
					prop.ColorInfo.CopyFrom(color);
					properties.Add(prop);
				}
			}
		}
		void AddColorProperty(XFProperties properties, ColorModelInfo color, Type propType) {
			if (IsExtColor(color)) {
				XFPropColorBase prop = XFPropertyFactory.CreateProperty(propType) as XFPropColorBase;
				if (prop != null) {
					prop.ColorInfo.CopyFrom(color);
					properties.Add(prop);
				}
			}
		}
		void AddBorderProperty(XFProperties properties, ColorModelInfo color, XlBorderLineStyle lineStyle, Type propType) {
			if (IsExtColor(color)) {
				XFPropBorderBase prop = XFPropertyFactory.CreateProperty(propType) as XFPropBorderBase;
				if (prop != null) {
					prop.ColorInfo.CopyFrom(color);
					prop.LineStyle = lineStyle;
					properties.Add(prop);
				}
			}
		}
		void AddBorderPropertyExt(XFProperties properties, ColorModelInfo color, XlBorderLineStyle lineStyle, Type propType) {
			if (IsExtColor(color) || color.ColorType == ColorType.Index || lineStyle != XlBorderLineStyle.None) {
				XFPropBorderBase prop = XFPropertyFactory.CreateProperty(propType) as XFPropBorderBase;
				if (prop != null) {
					prop.ColorInfo.CopyFrom(color);
					prop.LineStyle = lineStyle;
					properties.Add(prop);
				}
			}
		}
		protected bool IsExtColor(ColorModelInfo colorInfo) {
			ColorModelInfo defaultItem = DocumentModel.Cache.ColorModelInfoCache.DefaultItem;
			if (Object.ReferenceEquals(defaultItem, colorInfo))
				return false;
			return colorInfo.ColorType == ColorType.Auto || colorInfo.ColorType == ColorType.Rgb || colorInfo.ColorType == ColorType.Theme;
		}
		protected StyleCategory GetStyleCategory(int builtInId) {
			return BuiltInCellStyleCalculator.GetStyleCategory(builtInId);
		}
		#endregion
	}
}
