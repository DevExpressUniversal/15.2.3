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
using System.IO;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Export.Xls;
using DevExpress.Export.Xl;
using DevExpress.XtraExport.Xls;
#if !SL
using System.Drawing;
using System.Collections;
using DevExpress.Office.Model;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region DxfN12InfoBase
	public abstract class DxfN12InfoBase {
		#region Fields
		DxfNInfo differentialFormatInfo;
		XFExtProperties extProperties;
		#endregion
		#region Properties
		public DxfNInfo DifferentialFormatInfo { get { return differentialFormatInfo; } }
		public XFExtProperties ExtProperties { get { return extProperties; } }
		#endregion
		protected DxfN12InfoBase() {
			this.differentialFormatInfo = new DxfNInfo();
			this.extProperties = new XFExtProperties();
		}
		protected DxfN12InfoBase(DxfNInfo differentialFormatInfo, XFExtProperties extProperties) {
			Guard.ArgumentNotNull(differentialFormatInfo, "differentialFormatInfo");
			this.differentialFormatInfo = differentialFormatInfo;
			if(extProperties != null && extProperties.Count > 0)
				this.extProperties = extProperties;
			else
				this.extProperties = new XFExtProperties();
		}
		protected void ReadXFExtNoFRTStructure(XlsReader reader) {
			reader.ReadUInt16(); 
			reader.ReadUInt16(); 
			extProperties.Read(reader);
		}
		protected void ReadXFExtNoFRTStructure(BinaryReader reader) {
			using(XlsReader xlsReader = new XlsReader(reader))
				ReadXFExtNoFRTStructure(xlsReader);
		}
		protected void WriteXFExtNoFRTStructure(BinaryWriter writer) {
			writer.Write((ushort)0x0000); 
			writer.Write((ushort)0xffff); 
			extProperties.Write(writer);
		}
		public DifferentialFormat GetDifferentialFormat(XlsContentBuilder contentBuilder) {
			DocumentModel documentModel = contentBuilder.DocumentModel;
			XlsDifferentialFormatInfo info = new XlsDifferentialFormatInfo();
			DifferentialFormatInfo.AssignProperties(info, documentModel);
			if(ExtProperties.Count > 0)
				ExtProperties.ApplyContent(new XlsDifferentialFormatInfoAdapter(info));
			return info.GetDifferentialFormat(contentBuilder.StyleSheet);
		}
		protected void CalculateProperties(DifferentialFormat format) {
			XlsDifferentialFormatInfo info = XlsDifferentialFormatInfo.FromFormat(format);
			CalculateExtProperties(format, info);
			DifferentialFormatInfo.CalculateProperties(format);
		}
		void CalculateExtProperties(DifferentialFormat format, XlsDifferentialFormatInfo info) {
			AssignFillForeColor(format, info);
			AssignFillBackColor(format, info);
			AssignBorderTopColor(format, info);
			AssignBorderBottomColor(format, info);
			AssignBorderLeftColor(format, info);
			AssignBorderRightColor(format, info);
			AssignBorderDiagonalColor(format, info);
			AssignFontColor(format, info);
			AssignFontSchemeStyle(format, info);
			info.CreateDifferentialFormatExtProperties(ExtProperties);
		}
		#region Assign properties
		void AssignFillForeColor(DifferentialFormat format, XlsDifferentialFormatInfo info) {
			if(format.MultiOptionsInfo.ApplyFillForeColor) {
				ColorModelInfo colorInfo = format.FillInfo.GetForeColorModelInfo(format.DocumentModel);
				if(colorInfo.ColorType != ColorType.Index)
					info.FillForegroundColor = colorInfo;
			}
		}
		void AssignFillBackColor(DifferentialFormat format, XlsDifferentialFormatInfo info) {
			if(format.MultiOptionsInfo.ApplyFillBackColor) {
				ColorModelInfo colorInfo = format.FillInfo.GetBackColorModelInfo(format.DocumentModel);
				if(colorInfo.ColorType != ColorType.Index)
					info.FillBackgroundColor = colorInfo;
			}
		}
		void AssignBorderTopColor(DifferentialFormat format, XlsDifferentialFormatInfo info) {
			if(format.BorderOptionsInfo.ApplyTopColor) {
				ColorModelInfo colorInfo = BorderInfo.TopBorderAccessor.GetColorModelInfo(format.BorderInfo, format.DocumentModel);
				if(colorInfo.ColorType != ColorType.Index)
					info.BorderTopColor = colorInfo;
			}
		}
		void AssignBorderBottomColor(DifferentialFormat format, XlsDifferentialFormatInfo info) {
			if(format.BorderOptionsInfo.ApplyBottomColor) {
				ColorModelInfo colorInfo = BorderInfo.BottomBorderAccessor.GetColorModelInfo(format.BorderInfo, format.DocumentModel);
				if(colorInfo.ColorType != ColorType.Index)
					info.BorderBottomColor = colorInfo;
			}
		}
		void AssignBorderLeftColor(DifferentialFormat format, XlsDifferentialFormatInfo info) {
			if(format.BorderOptionsInfo.ApplyLeftColor) {
				ColorModelInfo colorInfo = BorderInfo.LeftBorderAccessor.GetColorModelInfo(format.BorderInfo, format.DocumentModel);
				if(colorInfo.ColorType != ColorType.Index)
					info.BorderLeftColor = colorInfo;
			}
		}
		void AssignBorderRightColor(DifferentialFormat format, XlsDifferentialFormatInfo info) {
			if(format.BorderOptionsInfo.ApplyRightColor) {
				ColorModelInfo colorInfo = BorderInfo.RightBorderAccessor.GetColorModelInfo(format.BorderInfo, format.DocumentModel);
				if(colorInfo.ColorType != ColorType.Index)
					info.BorderRightColor = colorInfo;
			}
		}
		void AssignBorderDiagonalColor(DifferentialFormat format, XlsDifferentialFormatInfo info) {
			if(format.BorderOptionsInfo.ApplyDiagonalColor) {
				ColorModelInfo colorInfo = BorderInfo.DiagonalBorderAccessor.GetColorModelInfo(format.BorderInfo, format.DocumentModel);
				if(colorInfo.ColorType != ColorType.Index)
					info.BorderDiagonalColor = colorInfo;
			}
		}
		void AssignFontColor(DifferentialFormat format, XlsDifferentialFormatInfo info) {
			if(format.MultiOptionsInfo.ApplyFontColor) {
				ColorModelInfo colorInfo = format.FontInfo.GetColorModelInfo(format.DocumentModel);
				if(colorInfo.ColorType != ColorType.Index)
					info.FontColor = colorInfo;
			}
		}
		void AssignFontSchemeStyle(DifferentialFormat format, XlsDifferentialFormatInfo info) {
			if(format.MultiOptionsInfo.ApplyFontSchemeStyle)
				info.FontSchemeStyle = format.Font.SchemeStyle;
		}
		#endregion
	}
	#endregion
	#region DxfN12Info
	public class DxfN12Info : DxfN12InfoBase {
		#region Static members
		public static DxfN12Info FromStream(XlsReader reader) {
			DxfN12Info result = new DxfN12Info();
			result.Read(reader);
			return result;
		}
		public static DxfN12Info FromXlsFormats(DxfNInfo differentialFormatInfo, XFExtProperties extProperties) {
			return new DxfN12Info(differentialFormatInfo, extProperties);
		}
		public static DxfN12Info FromDifferentialFormat(DifferentialFormat format) {
			Guard.ArgumentNotNull(format, "DifferentialFormat");
			DxfN12Info result = new DxfN12Info();
			result.CalculateProperties(format);
			return result;
		}
		#endregion
		public DxfN12Info() : base() {
		}
		protected DxfN12Info(DxfNInfo differentialFormatInfo, XFExtProperties extProperties) 
			: base(differentialFormatInfo, extProperties) {
		}
		public bool IsEmpty { get; set; }
		void Read(XlsReader reader) {
			int size = reader.ReadInt32();
			if(size == 0) {
				IsEmpty = true;
				reader.ReadUInt16(); 
			}
			else {
				IsEmpty = false;
				using(XlsCommandStream stream = new XlsCommandStream(reader, size)) {
					using(BinaryReader binaryReader = new BinaryReader(stream))
						DifferentialFormatInfo.Read(binaryReader);
				}
				if(size > DifferentialFormatInfo.GetSize())
					ReadXFExtNoFRTStructure(reader);
			}
		}
		public void Write(BinaryWriter writer) {
			if(IsEmpty) {
				writer.Write((int)0); 
				writer.Write((ushort)0); 
			}
			else {
				int cbDxf = GetSize() - 4;
				writer.Write(cbDxf);
				DifferentialFormatInfo.Write(writer);
				if(ExtProperties.Count > 0)
					WriteXFExtNoFRTStructure(writer);
			}
		}
		protected internal int GetSize() {
			if(IsEmpty)
				return 6;
			int result = DifferentialFormatInfo.GetSize() + 4;
			if (ExtProperties.Count > 0)
				result += ExtProperties.GetSize() + 4;
			return result;
		}
	}
	#endregion
	#region DxfN12ListInfo
	public class DxfN12ListInfo : DxfN12InfoBase {
		#region Static members
		public static DxfN12ListInfo FromStream(BinaryReader reader, int size) {
			DxfN12ListInfo result = new DxfN12ListInfo();
			result.size = size; 
			result.Read(reader);
			return result;
		}
		public static DxfN12ListInfo FromStream(XlsReader reader, int infoSize, int blockSize) {
			DxfN12ListInfo result = new DxfN12ListInfo();
			result.size = infoSize;
			using (XlsCommandStream stream = new XlsCommandStream(reader, blockSize)) {
				using (BinaryReader binaryReader = new BinaryReader(stream))
					result.Read(binaryReader);
			}
			return result;
		}
		public static DxfN12ListInfo FromXlsFormats(DxfNInfo differentialFormatInfo, XFExtProperties extProperties) {
			return new DxfN12ListInfo(differentialFormatInfo, extProperties);
		}
		public static DxfN12ListInfo FromDifferentialFormat(DifferentialFormat format) {
			Guard.ArgumentNotNull(format, "DifferentialFormat");
			DxfN12ListInfo result = new DxfN12ListInfo();
			result.CalculateProperties(format);
			return result;
		}
		#endregion
		#region Fields
		int size;
		#endregion
		public DxfN12ListInfo() : base() {
		}
		protected DxfN12ListInfo(DxfNInfo differentialFormatInfo, XFExtProperties extProperties) 
			: base(differentialFormatInfo, extProperties) {
			this.size = differentialFormatInfo.GetSize();
			if(extProperties != null && extProperties.Count > 0)
				this.size += 4 + extProperties.GetSize();
		}
		void Read(BinaryReader reader) {
			DifferentialFormatInfo.Read(reader);
			if (size > DifferentialFormatInfo.GetSize())
				ReadXFExtNoFRTStructure(reader);
		}
		public void Write(BinaryWriter writer) {
			DifferentialFormatInfo.Write(writer);
			if (ExtProperties.Count > 0)
				WriteXFExtNoFRTStructure(writer);
		}
		protected internal short GetSize() {
			int result = DifferentialFormatInfo.GetSize();
			if (ExtProperties.Count > 0)
				result += 4 + ExtProperties.GetSize();
			return (short)(result);
		}
	}
	#endregion
	#region DxfNInfo // DXFN structure
	public class DxfNInfo : XlsDxfN {
		#region Fields
		readonly XlsDifferentialFormatInfo info = new XlsDifferentialFormatInfo();
		#endregion
		#region Properties
		public XlsDifferentialFormatInfo Info { get { return info; } }
		#endregion
		protected internal void AssignProperties(XlsDifferentialFormatInfo formatInfo, DocumentModel documentModel) {
			if (FlagsInfo.IncludeNumberFormat)
				AssingProperties(NumberFormatInfo, formatInfo, documentModel.Cache.NumberFormatCache);
			if (FlagsInfo.IncludeFont)
				FontInfo.AssignProperties(formatInfo);
			if (FlagsInfo.IncludeAlignment)
				AlignmentInfo.AssignProperties(formatInfo, FlagsInfo, documentModel);
			if (FlagsInfo.IncludeBorder)
				BorderInfo.AssignProperties(formatInfo, FlagsInfo);
			if (FlagsInfo.IncludeFill)
				FillInfo.AssignProperties(formatInfo, FlagsInfo);
			if (FlagsInfo.IncludeProtection)
				ProtectionInfo.AssignProperties(formatInfo, FlagsInfo);
		}
		internal void CalculateProperties(DifferentialFormat format) {
			CalculateNumberFormatProperties(format);
			CalculateFontProperties(format);
			CalculateAlignmentProperties(format);
			CalculateBorderProperties(format);
			CalculateFillProperties(format);
			CalculateProtectionProperties(format);
		}
		void CalculateNumberFormatProperties(DifferentialFormat format) {
			if (format.MultiOptionsInfo.ApplyNumberFormat)
				NumberFormatInfo = CreateDxfNum(format.NumberFormatId, format.FormatString, !string.IsNullOrEmpty(format.FormatString), FlagsInfo);
		}
		void CalculateFontProperties(DifferentialFormat format) {
			if (!format.MultiOptionsInfo.ApplyFontNone) {
				FlagsInfo.IncludeFont = true;
				FontInfo.CalculateProperties(format, FlagsInfo);
			}
		}
		void CalculateAlignmentProperties(DifferentialFormat format) {
			if (!format.MultiOptionsInfo.ApplyAlignmentNone) {
				FlagsInfo.IncludeAlignment = true;
				AlignmentInfo.CalculateProperties(format, FlagsInfo);
			}
			else
				FlagsInfo.AlignmentReadingOrderZeroInited = true;
		}
		void CalculateBorderProperties(DifferentialFormat format) {
			if (!format.BorderOptionsInfo.ApplyNone) {
				FlagsInfo.IncludeBorder = true;
				BorderInfo.CalculateProperties(format, FlagsInfo);
			}
			else
				FlagsInfo.NewBorder = true;
		}
		void CalculateFillProperties(DifferentialFormat format) {
			if (!format.MultiOptionsInfo.ApplyFillNone) {
				FlagsInfo.IncludeFill = true;
				FillInfo.CalculateProperties(format, FlagsInfo);
			}
		}
		void CalculateProtectionProperties(DifferentialFormat format) {
			if (!format.MultiOptionsInfo.ApplyProtectionNone) {
				FlagsInfo.IncludeProtection = true;
				ProtectionInfo.CalculateProperties(format, FlagsInfo);
			}
		}
		XlsDxfNum CreateDxfNum(int id, string formatCode, bool isCustom, XlsDxfNFlags flagsInfo) {
			XlsDxfNum result = CreateDxfNum(isCustom);
			CalculateProperties(result, id, formatCode);
			flagsInfo.IncludeNumberFormat = true;
			flagsInfo.NumberFormatNinch = false;
			flagsInfo.UserDefinedNumberFormat = isCustom;
			return result;
		}
		void AssingProperties(XlsDxfNum dxfNum, XlsDifferentialFormatInfo formatInfo, NumberFormatCollection cache) {
			if (dxfNum.IsCustom)
				formatInfo.NumberFormatCode = dxfNum.NumberFormatCode;
			else {
				formatInfo.NumberFormatCode = cache[dxfNum.NumberFormatId].FormatCode;
				formatInfo.NumberFormatId = dxfNum.NumberFormatId;
			}
		}
		void CalculateProperties(XlsDxfNum dxfNum, int id, string formatCode) {
			dxfNum.NumberFormatId = id;
			dxfNum.NumberFormatCode = formatCode.Replace("&quot;", "\"");
		}
	}
	#endregion
	#region XlsDxfFont extensions
	static class XlsDxfFontExtensions {
		public static void CalculateProperties(this XlsDxfFont font, DifferentialFormat format, XlsDxfNFlags flagsInfo) {
			if (format.MultiOptionsInfo.ApplyFontBold) {
				font.FontBold = format.Font.Bold;
				font.FontBoldNinch = false;
				font.IsDefaultFont = false;
			}
			if(format.MultiOptionsInfo.ApplyFontCharset) {
				font.FontCharset = format.Font.Charset;
				font.IsDefaultFont = false;
			}
			if(format.MultiOptionsInfo.ApplyFontFamily) {
				font.FontFamily = format.Font.FontFamily;
				font.IsDefaultFont = false;
			}
			if (format.MultiOptionsInfo.ApplyFontItalic) {
				font.FontItalic = format.Font.Italic;
				font.FontItalicNinch = false;
				font.IsDefaultFont = false;
			}
			if(format.MultiOptionsInfo.ApplyFontSize) {
				font.FontSize = (int)(format.Font.Size * 20.0);
				font.IsDefaultFont = false;
			}
			if (format.MultiOptionsInfo.ApplyFontScript) {
				font.FontScript = format.Font.Script;
				font.FontScriptNinch = false;
				font.IsDefaultFont = false;
			}
			if (format.MultiOptionsInfo.ApplyFontUnderline) {
				font.FontUnderline = format.Font.Underline;
				font.FontUnderlineNinch = false;
				font.IsDefaultFont = false;
			}
			if (format.MultiOptionsInfo.ApplyFontStrikeThrough) {
				font.FontStrikeThrough = format.Font.StrikeThrough;
				font.FontStrikeThroughNinch = false;
				font.IsDefaultFont = false;
			}
			if (format.MultiOptionsInfo.ApplyFontColor) {
				DocumentModel workbook = format.DocumentModel;
				ColorModelInfo colorInfo = format.FontInfo.GetColorModelInfo(workbook);
				font.FontColorIndex = workbook.StyleSheet.Palette.GetFontColorIndex(workbook.OfficeTheme.Colors, colorInfo);
				font.IsDefaultFont = false;
			}
			if(format.MultiOptionsInfo.ApplyFontName) {
				font.FontName = format.Font.Name;
				font.IsDefaultFont = false;
			}
		}
		public static void AssignProperties(this XlsDxfFont font, XlsDifferentialFormatInfo formatInfo) {
			if (font.FontBold.HasValue && !font.FontBoldNinch)
				formatInfo.FontBold = font.FontBold.Value;
			formatInfo.FontCharset = font.FontCharset;
			formatInfo.FontFamily = font.FontFamily;
			if (!font.FontItalicNinch)
				formatInfo.FontItalic = font.FontItalic;
			if (font.FontSize != XlsDxfFont.DefaultIntValue)
				formatInfo.FontSize = font.FontSize / 20.0;
			if (font.FontScript.HasValue && !font.FontScriptNinch)
				formatInfo.FontScript = font.FontScript.Value;
			if (font.FontUnderline.HasValue && !font.FontUnderlineNinch)
				formatInfo.FontUnderline = font.FontUnderline.Value;
			if (!font.FontStrikeThroughNinch)
				formatInfo.FontStrikeThrough = font.FontStrikeThrough;
			if (font.FontColorIndex != XlsDxfFont.DefaultIntValue) {
				if (font.FontColorIndex == Palette.FontAutomaticColorIndex)
					formatInfo.FontColor = ColorModelInfo.CreateAutomatic();
				else
					formatInfo.FontColor = ColorModelInfo.Create(font.FontColorIndex);
			}
			if (!String.IsNullOrEmpty(font.FontName))
				formatInfo.FontName = font.FontName;
		}
	}
	#endregion
	#region XlsDxfAlign extensions
	static class XlsDxfAlignExtensions {
		public static void CalculateProperties(this XlsDxfAlign info, DifferentialFormat format, XlsDxfNFlags FlagsInfo) {
			if (format.MultiOptionsInfo.ApplyAlignmentHorizontal) {
				info.HorizontalAlignment = format.Alignment.Horizontal;
				FlagsInfo.AlignmentHorizontalNinch = false;
			}
			if (format.MultiOptionsInfo.ApplyAlignmentIndent) {
				info.Indent = format.Alignment.Indent;
				FlagsInfo.AlignmentIndentNinch = false;
			}
			if (format.MultiOptionsInfo.ApplyAlignmentJustifyLastLine) {
				info.JustifyLastLine = format.Alignment.JustifyLastLine;
				FlagsInfo.AlignmentJustifyLastLineNinch = false;
			}
			if (format.MultiOptionsInfo.ApplyAlignmentReadingOrder) {
				info.ReadingOrder = format.Alignment.ReadingOrder;
				FlagsInfo.AlignmentReadingOrderNinch = false;
				FlagsInfo.AlignmentReadingOrderZeroInited = true;
			}
			if (format.MultiOptionsInfo.ApplyAlignmentRelativeIndent)
				info.RelativeIndent = format.Alignment.RelativeIndent;
			if (format.MultiOptionsInfo.ApplyAlignmentShrinkToFit) {
				info.ShrinkToFit = format.Alignment.ShrinkToFit;
				FlagsInfo.AlignmentShrinkToFitNinch = false;
			}
			if (format.MultiOptionsInfo.ApplyAlignmentTextRotation) {
				info.TextRotation = format.DocumentModel.UnitConverter.ModelUnitsToDegree(format.Alignment.TextRotation);
				FlagsInfo.AlignmentTextRotationNinch = false;
			}
			if (format.MultiOptionsInfo.ApplyAlignmentVertical) {
				info.VerticalAlignment = format.Alignment.Vertical;
				FlagsInfo.AlignmentVerticalNinch = false;
			}
			if (format.MultiOptionsInfo.ApplyAlignmentWrapText) {
				info.WrapText = format.Alignment.WrapText;
				FlagsInfo.AlignmentWrapTextNinch = false;
			}
		}
		public static void AssignProperties(this XlsDxfAlign info, XlsDifferentialFormatInfo formatInfo, XlsDxfNFlags FlagsInfo, DocumentModel documentModel) {
			if (!FlagsInfo.AlignmentHorizontalNinch)
				formatInfo.AlignmentHorizontal = info.HorizontalAlignment;
			if (!FlagsInfo.AlignmentIndentNinch)
				formatInfo.AlignmentIndent = info.Indent;
			if (!FlagsInfo.AlignmentJustifyLastLineNinch)
				formatInfo.AlignmentJustifyLastLine = info.JustifyLastLine;
			if (!FlagsInfo.AlignmentReadingOrderNinch && FlagsInfo.AlignmentReadingOrderZeroInited)
				formatInfo.AlignmentReadingOrder = info.ReadingOrder;
			if (info.RelativeIndent != XlsDxfAlign.DefaultRelativeIndent)
				formatInfo.AlignmentRelativeIndent = info.RelativeIndent;
			if (!FlagsInfo.AlignmentShrinkToFitNinch)
				formatInfo.AlignmentShrinkToFit = info.ShrinkToFit;
			if (!FlagsInfo.AlignmentTextRotationNinch)
				formatInfo.AlignmentTextRotation = documentModel.UnitConverter.DegreeToModelUnits(info.TextRotation);
			if (!FlagsInfo.AlignmentVerticalNinch)
				formatInfo.AlignmentVertical = info.VerticalAlignment;
			if (!FlagsInfo.AlignmentWrapTextNinch)
				formatInfo.AlignmentWrapText = info.WrapText;
		}
	}
	#endregion
	#region XlsDxfBorder extensions
	static class XlsDxfBorderExtensions {
		static bool HasColorIndex(DifferentialFormat format, BorderOptionsBorderAccessor borderOptionsAccessor) {
			return borderOptionsAccessor.GetApplyColor(format.BorderOptionsInfo);
		}
		static int GetColorIndex(DifferentialFormat format, BorderInfoBorderAccessor borderInfoAccessor) {
			DocumentModel documentModel = format.DocumentModel;
			BorderInfo borderInfo = format.BorderInfo;
			ColorModelInfo colorInfo = borderInfoAccessor.GetColorModelInfo(borderInfo, documentModel);
			return documentModel.StyleSheet.Palette.GetColorIndex(documentModel.OfficeTheme.Colors, colorInfo, true);
		}
		public static void CalculateProperties(this XlsDxfBorder info, DifferentialFormat format, XlsDxfNFlags flagsInfo) {
			BorderOptionsInfo options = format.BorderOptionsInfo;
			XlBorderLineStyle leftLineStyle = format.Border.LeftLineStyle;
			bool applyLeftBorder = options.ApplyLeftLineStyle && leftLineStyle != XlBorderLineStyle.None;
			flagsInfo.BorderLeftNinch = !applyLeftBorder;
			if (flagsInfo.BorderLeftNinch)
				info.LeftColorIndex = 0;
			else {
				info.LeftLineStyle = leftLineStyle;
				if (HasColorIndex(format, BorderOptionsInfo.LeftBorderAccessor))
					info.LeftColorIndex = GetColorIndex(format, BorderInfo.LeftBorderAccessor);
			}
			XlBorderLineStyle rightLineStyle = format.Border.RightLineStyle;
			bool applyRightBorder = options.ApplyRightLineStyle && rightLineStyle != XlBorderLineStyle.None;
			flagsInfo.BorderRightNinch = !applyRightBorder;
			if (flagsInfo.BorderRightNinch)
				info.RightColorIndex = 0;
			else {
				info.RightLineStyle = rightLineStyle;
				if (HasColorIndex(format, BorderOptionsInfo.RightBorderAccessor))
					info.RightColorIndex = GetColorIndex(format, BorderInfo.RightBorderAccessor);
			}
			XlBorderLineStyle topLineStyle = format.Border.TopLineStyle;
			bool applyTopBorder = options.ApplyTopLineStyle && topLineStyle != XlBorderLineStyle.None;
			flagsInfo.BorderTopNinch = !applyTopBorder;
			if (flagsInfo.BorderTopNinch)
				info.TopColorIndex = 0;
			else {
				info.TopLineStyle = topLineStyle;
				if (HasColorIndex(format, BorderOptionsInfo.TopBorderAccessor))
					info.TopColorIndex = GetColorIndex(format, BorderInfo.TopBorderAccessor);
			}
			XlBorderLineStyle bottomLineStyle = format.Border.BottomLineStyle;
			bool applyBottomBorder = options.ApplyBottomLineStyle && bottomLineStyle != XlBorderLineStyle.None;
			flagsInfo.BorderBottomNinch = !applyBottomBorder;
			if (flagsInfo.BorderBottomNinch)
				info.BottomColorIndex = 0;
			else {
				info.BottomLineStyle = bottomLineStyle;
				if (HasColorIndex(format, BorderOptionsInfo.BottomBorderAccessor))
					info.BottomColorIndex = GetColorIndex(format, BorderInfo.BottomBorderAccessor);
			}
			XlBorderLineStyle diagonalUpLineStyle = format.Border.DiagonalUpLineStyle;
			bool applyDiagonalUpBorder = options.ApplyDiagonalLineStyle && diagonalUpLineStyle != XlBorderLineStyle.None;
			flagsInfo.BorderDiagonalUpNinch = !applyDiagonalUpBorder;
			if (flagsInfo.BorderDiagonalUpNinch)
				info.DiagonalColorIndex = 0;
			else {
				info.DiagonalLineStyle = diagonalUpLineStyle;
				if (HasColorIndex(format, BorderOptionsInfo.DiagonalBorderAccessor))
					info.DiagonalColorIndex = GetColorIndex(format, BorderInfo.DiagonalBorderAccessor);
			}
			XlBorderLineStyle diagonalDownLineStyle = format.Border.DiagonalDownLineStyle;
			bool applyDiagonalDownBorder = options.ApplyDiagonalLineStyle && diagonalDownLineStyle != XlBorderLineStyle.None;
			flagsInfo.BorderDiagonalDownNinch = !applyDiagonalDownBorder;
			if (flagsInfo.BorderDiagonalDownNinch)
				info.DiagonalColorIndex = 0;
			else {
				info.DiagonalLineStyle = diagonalDownLineStyle;
				if (HasColorIndex(format, BorderOptionsInfo.DiagonalBorderAccessor))
					info.DiagonalColorIndex = GetColorIndex(format, BorderInfo.DiagonalBorderAccessor);
			} 
			info.DiagonalUp = applyDiagonalUpBorder;
			info.DiagonalDown = applyDiagonalDownBorder;
			flagsInfo.NewBorder = format.Border.Outline;
		}
		static public void AssignProperties(this XlsDxfBorder info, XlsDifferentialFormatInfo formatInfo, XlsDxfNFlags flagsInfo) {
			AssignTopBorder(info, formatInfo, flagsInfo);
			AssignBottomBorder(info, formatInfo, flagsInfo);
			AssignLeftBorder(info, formatInfo, flagsInfo);
			AssignRightBorder(info, formatInfo, flagsInfo);
			AssignDiagonalBorder(info, formatInfo, flagsInfo);
			AssignDiagonalUp(info, formatInfo, flagsInfo);
			AssignDiagonalDown(info, formatInfo, flagsInfo);
			formatInfo.BorderOutline = flagsInfo.NewBorder;		   
		}
		static void AssignTopBorder(XlsDxfBorder info, XlsDifferentialFormatInfo formatInfo, XlsDxfNFlags flagsInfo) {
			if (!flagsInfo.BorderTopNinch) {
				formatInfo.BorderTopLineStyle = info.TopLineStyle;
				if(info.TopLineStyle != XlBorderLineStyle.None) {
					if(info.TopColorIndex == Palette.DefaultForegroundColorIndex)
						formatInfo.BorderTopColor = ColorModelInfo.CreateAutomatic();
					else
						formatInfo.BorderTopColor = ColorModelInfo.Create(info.TopColorIndex);
				}
			}
		}
		static void AssignBottomBorder(XlsDxfBorder info, XlsDifferentialFormatInfo formatInfo, XlsDxfNFlags flagsInfo) {
			if (!flagsInfo.BorderBottomNinch) {
				formatInfo.BorderBottomLineStyle = info.BottomLineStyle;
				if(info.BottomLineStyle != XlBorderLineStyle.None) {
					if(info.BottomColorIndex == Palette.DefaultForegroundColorIndex)
						formatInfo.BorderBottomColor = ColorModelInfo.CreateAutomatic();
					else
						formatInfo.BorderBottomColor = ColorModelInfo.Create(info.BottomColorIndex);
				}
			}
		}
		static void AssignLeftBorder(XlsDxfBorder info, XlsDifferentialFormatInfo formatInfo, XlsDxfNFlags flagsInfo) {
			if (!flagsInfo.BorderLeftNinch) {
				formatInfo.BorderLeftLineStyle = info.LeftLineStyle;
				if(info.LeftLineStyle != XlBorderLineStyle.None) {
					if(info.LeftColorIndex == Palette.DefaultForegroundColorIndex)
						formatInfo.BorderLeftColor = ColorModelInfo.CreateAutomatic();
					else
						formatInfo.BorderLeftColor = ColorModelInfo.Create(info.LeftColorIndex);
				}
			}
		}
		static void AssignRightBorder(XlsDxfBorder info, XlsDifferentialFormatInfo formatInfo, XlsDxfNFlags flagsInfo) {
			if (!flagsInfo.BorderRightNinch) {
				formatInfo.BorderRightLineStyle = info.RightLineStyle;
				if(info.RightLineStyle != XlBorderLineStyle.None) {
					if(info.RightColorIndex == Palette.DefaultForegroundColorIndex)
						formatInfo.BorderRightColor = ColorModelInfo.CreateAutomatic();
					else
						formatInfo.BorderRightColor = ColorModelInfo.Create(info.RightColorIndex);
				}
			}
		}
		static void AssignDiagonalBorder(XlsDxfBorder info, XlsDifferentialFormatInfo formatInfo, XlsDxfNFlags flagsInfo) {
			if (!flagsInfo.BorderDiagonalUpNinch || !flagsInfo.BorderDiagonalDownNinch) {
				formatInfo.BorderDiagonalLineStyle = info.DiagonalLineStyle;
				if(info.DiagonalLineStyle != XlBorderLineStyle.None) {
					if(info.DiagonalColorIndex == Palette.DefaultForegroundColorIndex)
						formatInfo.BorderDiagonalColor = ColorModelInfo.CreateAutomatic();
					else
						formatInfo.BorderDiagonalColor = ColorModelInfo.Create(info.DiagonalColorIndex);
				}
			}
		}
		static void AssignDiagonalUp(XlsDxfBorder info, XlsDifferentialFormatInfo formatInfo, XlsDxfNFlags flagsInfo) {
			if (!flagsInfo.BorderDiagonalUpNinch)
				formatInfo.BorderDiagonalUp = info.DiagonalUp;
		}
		static void AssignDiagonalDown(XlsDxfBorder info, XlsDifferentialFormatInfo formatInfo, XlsDxfNFlags flagsInfo) {
			if (!flagsInfo.BorderDiagonalDownNinch)
				formatInfo.BorderDiagonalDown = info.DiagonalDown;
		}
	}
	#endregion
	#region XlsDxfFill extensions
	static class XlsDxfFillExtensions {
		public static void CalculateProperties(this XlsDxfFill info, DifferentialFormat format, XlsDxfNFlags flagsInfo) {
			if (format.MultiOptionsInfo.ApplyFillPatternType) {
				info.PatternType = format.Fill.PatternType;
				flagsInfo.FillPatternTypeNinch = false;
			}
			DocumentModel documentModel = format.DocumentModel;
			if (format.MultiOptionsInfo.ApplyFillForeColor) {
				ColorModelInfo colorInfo = format.FillInfo.GetForeColorModelInfo(documentModel);
				int colorIndex = documentModel.StyleSheet.Palette.GetColorIndex(documentModel.OfficeTheme.Colors, colorInfo, true);
				info.ForeColorIndex = colorIndex;
				flagsInfo.FillForegroundColorNinch = false;
			}
			else
				info.ForeColorIndex = 64;
			if (format.MultiOptionsInfo.ApplyFillBackColor) {
				ColorModelInfo colorInfo = format.FillInfo.GetBackColorModelInfo(documentModel);
				int colorIndex = documentModel.StyleSheet.Palette.GetColorIndex(documentModel.OfficeTheme.Colors, colorInfo, false);
				info.BackColorIndex = colorIndex;
				flagsInfo.FillBackgroundColorNinch = false;
			}
		}
		public static void AssignProperties(this XlsDxfFill info, XlsDifferentialFormatInfo formatInfo, XlsDxfNFlags flagsInfo) {
			if (!flagsInfo.FillPatternTypeNinch)
				formatInfo.FillPatternType = info.PatternType;
			if(!flagsInfo.FillForegroundColorNinch) {
				if(info.ForeColorIndex == Palette.DefaultForegroundColorIndex)
					formatInfo.FillForegroundColor = ColorModelInfo.CreateAutomatic();
				else
					formatInfo.FillForegroundColor = ColorModelInfo.Create(info.ForeColorIndex);
			}
			if(!flagsInfo.FillBackgroundColorNinch) {
				if(info.BackColorIndex == Palette.DefaultBackgroundColorIndex)
					formatInfo.FillBackgroundColor = ColorModelInfo.CreateAutomatic();
				else
					formatInfo.FillBackgroundColor = ColorModelInfo.Create(info.BackColorIndex);
			}
		}
	}
	#endregion
	#region XlsDxfProtection extensions
	static class XlsDxfProtectionExtensions {
		public static void CalculateProperties(this XlsDxfProtection info, DifferentialFormat format, XlsDxfNFlags FlagsInfo) {
			if (format.MultiOptionsInfo.ApplyProtectionLocked) {
				info.Locked = format.Protection.Locked;
				FlagsInfo.ProtectionLockedNinch = false;
			}
			if (format.MultiOptionsInfo.ApplyProtectionHidden) {
				info.Hidden = format.Protection.Hidden;
				FlagsInfo.ProtectionHiddenNinch = false;
			}
		}
		public static void AssignProperties(this XlsDxfProtection info, XlsDifferentialFormatInfo formatInfo, XlsDxfNFlags FlagsInfo) {
			if (!FlagsInfo.ProtectionLockedNinch)
				formatInfo.ProtectionLocked = info.Locked;
			if (!FlagsInfo.ProtectionHiddenNinch)
				formatInfo.ProtectionHidden = info.Hidden;
		}
	}
	#endregion
}
