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
using System.Globalization;
using System.IO;
using System.Text;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Export.Xl;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraExport.Xls {
	using DevExpress.XtraExport.Implementation;
	#region XlsDataAwareExporter
	public partial class XlsDataAwareExporter {
		#region Fields
		readonly XlsPalette palette = new XlsPalette();
		readonly Dictionary<XlFont, int> fontTable = new Dictionary<XlFont, int>();
		readonly List<XlFont> fontList = new List<XlFont>();
		readonly List<XlsXf> xfList = new List<XlsXf>();
		readonly Dictionary<XlsXf, int> xfTable = new Dictionary<XlsXf, int>();
		readonly List<XlsContentXFExt> xfExtensions = new List<XlsContentXFExt>();
		readonly List<ExcelNumberFormat> predefinedNumberFormats = new List<ExcelNumberFormat>();
		readonly Dictionary<string, ExcelNumberFormat> numberFormatTable = new Dictionary<string, ExcelNumberFormat>();
		readonly Dictionary<XlNetNumberFormat, ExcelNumberFormat> netNumberFormatTable = new Dictionary<XlNetNumberFormat, ExcelNumberFormat>();
		readonly XlExportNumberFormatConverter numberFormatConverter = new XlExportNumberFormatConverter();
		int customNumberFormatId;
		XlFont defaultFont;
		XlCellAlignment defaultAlignment;
		XlBorder defaultBorder;
		XlFill defaultFill;
		int xfCount;
		int xfCrc;
		#endregion
		#region Initialize
		void InitializeStyles() {
			InitFonts();
			InitNumberFormats();
			this.defaultBorder = new XlBorder();
			this.defaultAlignment = new XlCellAlignment();
			this.defaultFill = XlFill.NoFill();
			InitXfs();
		}
		void InitFonts() {
			fontTable.Clear();
			fontList.Clear();
			XlFont font = XlFont.BodyFont();
			fontTable.Add(font, 0);
			fontList.Add(font);
			this.defaultFont = font;
			font = new XlFont();
			font.CopyFrom(this.defaultFont);
			font.Bold = true;
			fontTable.Add(font, 1);
			fontList.Add(font);
			font = new XlFont();
			font.CopyFrom(this.defaultFont);
			font.Italic = true;
			fontTable.Add(font, 2);
			fontList.Add(font);
			font = new XlFont();
			font.CopyFrom(this.defaultFont);
			font.Bold = true;
			font.Italic = true;
			fontTable.Add(font, 3);
			fontList.Add(font);
		}
		void InitNumberFormats() {
			predefinedNumberFormats.Clear();
			AddPredefined(5, "_($#,##0_);($#,##0)");
			AddPredefined(6, "_($#,##0_);[Red]($#,##0)");
			AddPredefined(7, "_($#,##0.00_);($#,##0.00)");
			AddPredefined(8, "_($#,##0.00_);[Red]($#,##0.00)");
			AddPredefined(41, @"_(* #,##0_);_(* \(#,##0\);_(* ""-""_);_(@_)");
			AddPredefined(42, @"_(""$""* #,##0_);_(""$""* \(#,##0\);_(""$""* ""-""_);_(@_)");
			AddPredefined(43, @"_(* #,##0.00_);_(* \(#,##0.00\);_(* ""-""??_);_(@_)");
			AddPredefined(44, @"_(""$""* #,##0.00_);_(""$""* \(#,##0.00\);_(""$""* ""-""??_);_(@_)");
			numberFormatTable.Clear();
			netNumberFormatTable.Clear();
			customNumberFormatId = 164;
		}
		void AddPredefined(int id, string formatCode) {
			predefinedNumberFormats.Add(new ExcelNumberFormat(id, formatCode));
		}
		void InitXfs() {
			xfList.Clear();
			xfTable.Clear();
			XlsXf xf = new XlsXf();
			xf.IsStyleFormat = true;
			xf.Fill = this.defaultFill;
			xf.Border = this.defaultBorder;
			xf.Alignment = this.defaultAlignment;
			xfTable.Add(xf, xfList.Count);
			xfList.Add(xf);
			for(int i = 0; i < 2; i++) {
				xf = new XlsXf();
				xf.IsStyleFormat = true;
				xf.Fill = this.defaultFill;
				xf.Border = this.defaultBorder;
				xf.Alignment = this.defaultAlignment;
				xf.FontId = 1;
				xf.ApplyAlignment = false;
				xf.ApplyBorder = false;
				xf.ApplyFill = false;
				xf.ApplyNumberFormat = false;
				xf.ApplyProtection = false;
				if (!xfTable.ContainsKey(xf))
					xfTable.Add(xf, xfList.Count);
				xfList.Add(xf);
			}
			for(int i = 0; i < 2; i++) {
				xf = new XlsXf();
				xf.IsStyleFormat = true;
				xf.Fill = this.defaultFill;
				xf.Border = this.defaultBorder;
				xf.Alignment = this.defaultAlignment;
				xf.FontId = 2;
				xf.ApplyAlignment = false;
				xf.ApplyBorder = false;
				xf.ApplyFill = false;
				xf.ApplyNumberFormat = false;
				xf.ApplyProtection = false;
				if(!xfTable.ContainsKey(xf))
					xfTable.Add(xf, xfList.Count);
				xfList.Add(xf);
			}
			for(int i = 0; i < 10; i++) {
				xf = new XlsXf();
				xf.IsStyleFormat = true;
				xf.Fill = this.defaultFill;
				xf.Border = this.defaultBorder;
				xf.Alignment = this.defaultAlignment;
				xf.ApplyAlignment = false;
				xf.ApplyBorder = false;
				xf.ApplyFill = false;
				xf.ApplyNumberFormat = false;
				xf.ApplyProtection = false;
				if(!xfTable.ContainsKey(xf))
					xfTable.Add(xf, xfList.Count);
				xfList.Add(xf);
			}
			xf = new XlsXf();
			xf.IsStyleFormat = false;
			xf.Fill = this.defaultFill;
			xf.Border = this.defaultBorder;
			xf.Alignment = this.defaultAlignment;
			xfTable.Add(xf, xfList.Count);
			xfList.Add(xf);
		}
		#endregion
		#region WriteFonts
		protected void WriteFonts() {
			XlsContentFont content = new XlsContentFont();
			foreach(XlFont font in fontTable.Keys) {
				content.Size = font.Size;
				content.Bold = font.Bold;
				content.Italic = font.Italic;
				content.StrikeThrough = font.StrikeThrough;
				content.Outline = font.Outline;
				content.Shadow = font.Shadow;
				content.Condense = font.Condense;
				content.Extend = font.Extend;
				content.Script = font.Script;
				content.Underline = font.Underline;
				content.FontFamily = (int)font.FontFamily;
				content.Charset = font.Charset;
				content.FontName = font.Name;
				content.ColorIndex = palette.GetFontColorIndex(font.Color, currentDocument.Theme);
				WriteContent(XlsRecordType.Font, content);
			}
		}
		#endregion
		#region WriteNumberFormats
		protected void WriteNumberFormats() {
			XlsContentNumberFormat content = new XlsContentNumberFormat();
			WriteNumberFormats(content, predefinedNumberFormats);
			WriteNumberFormats(content, numberFormatTable.Values);
		}
		void WriteNumberFormats(XlsContentNumberFormat content, IEnumerable<ExcelNumberFormat> formats) {
			foreach(ExcelNumberFormat format in formats) {
				content.FormatId = format.Id;
				content.FormatCode = format.FormatString;
				WriteContent(XlsRecordType.Format, content);
			}
		}
		#endregion
		#region WriteXFs
		protected void WriteXFs() {
			xfExtensions.Clear();
			xfCount = 0;
			xfCrc = 0;
			XlsContentXF content = new XlsContentXF();
			foreach(XlsXf format in xfList) {
				content.IsStyleFormat = format.IsStyleFormat;
				content.IsHidden = format.IsHidden;
				content.IsLocked = format.IsLocked;
				content.FontId = format.FontId;
				content.NumberFormatId = format.NumberFormatId;
				content.StyleId = format.IsStyleFormat ? 0xfff : format.StyleId;
				content.ApplyAlignment = format.ApplyAlignment;
				content.ApplyBorder = format.ApplyBorder;
				content.ApplyFill = format.ApplyFill;
				content.ApplyFont = format.ApplyFont;
				content.ApplyNumberFormat = format.ApplyNumberFormat;
				content.ApplyProtection = format.ApplyProtection;
				XlBorder border = format.Border;
				content.BorderTopColorIndex = GetBorderColorIndex(border.TopLineStyle, border.TopColor);
				content.BorderBottomColorIndex = GetBorderColorIndex(border.BottomLineStyle, border.BottomColor);
				content.BorderLeftColorIndex = GetBorderColorIndex(border.LeftLineStyle, border.LeftColor);
				content.BorderRightColorIndex = GetBorderColorIndex(border.RightLineStyle, border.RightColor);
				XlBorderLineStyle diagonalLineStyle = border.DiagonalUpLineStyle != XlBorderLineStyle.None ? border.DiagonalUpLineStyle : border.DiagonalDownLineStyle;
				content.BorderDiagonalColorIndex = GetBorderColorIndex(diagonalLineStyle, border.DiagonalColor);
				content.BorderTopLineStyle = border.TopLineStyle;
				content.BorderBottomLineStyle = border.BottomLineStyle;
				content.BorderLeftLineStyle = border.LeftLineStyle;
				content.BorderRightLineStyle = border.RightLineStyle;
				content.BorderDiagonalLineStyle = diagonalLineStyle;
				content.BorderDiagonalDown = border.DiagonalDownLineStyle != XlBorderLineStyle.None;
				content.BorderDiagonalUp = border.DiagonalUpLineStyle != XlBorderLineStyle.None;
				XlFill fill = format.Fill;
				content.FillBackColorIndex = palette.GetBackgroundColorIndex(fill.BackColor, currentDocument.Theme);
				content.FillForeColorIndex = palette.GetForegroundColorIndex(fill.ForeColor, currentDocument.Theme);
				content.FillPatternType = fill.PatternType;
				XlCellAlignment alignment = format.Alignment;
				content.HorizontalAlignment = alignment.HorizontalAlignment;
				content.VerticalAlignment = alignment.VerticalAlignment;
				content.Indent = Math.Min(alignment.Indent, (byte)15);
				content.PivotButton = false;
				content.QuotePrefix = format.QuotePrefix;
				content.ReadingOrder = alignment.ReadingOrder;
				content.ShrinkToFit = alignment.ShrinkToFit;
				content.TextRotation = alignment.TextRotation;
				content.WrapText = alignment.WrapText;
				content.HasExtension = PrepareXFExtension(format, xfCount);
				content.CrcValue = xfCrc;
				WriteContent(XlsRecordType.XF, content);
				xfCrc = content.CrcValue;
				xfCount++;
			}
		}
		int GetBorderColorIndex(XlBorderLineStyle lineStyle, XlColor color) {
			return lineStyle != XlBorderLineStyle.None ? palette.GetForegroundColorIndex(color, currentDocument.Theme) : XlsPalette.DefaultForegroundColorIndex;
		}
		#endregion
		#region WriteXFExtensions
		protected void WriteXFExtensions() {
			if((xfExtensions.Count > 0) && (xfCount <= XlsDefs.MaxXFCount)) {
				WriteXFCrc();
				foreach(XlsContentXFExt content in xfExtensions)
					WriteContent(XlsRecordType.XFExt, content);
				xfExtensions.Clear();
			}
		}
		void WriteXFCrc() {
			XlsContentXFCrc content = new XlsContentXFCrc();
			content.XFCount = xfCount;
			content.XFCRC = xfCrc;
			WriteContent(XlsRecordType.XFCrc, content);
		}
		bool PrepareXFExtension(XlsXf format, int xfIndex) {
			XlsContentXFExt content = new XlsContentXFExt();
			content.XFIndex = xfIndex;
			PrepareXFExtProperties(format, content.Properties);
			bool result = content.Properties.Count > 0;
			if(result)
				xfExtensions.Add(content);
			return result;
		}
		void PrepareXFExtProperties(XlsXf format, XfProperties properties) {
			XlFill fill = format.Fill;
			if(!fill.ForeColor.IsEmpty)
				properties.Add(new XfPropFullColor(XfExtType.ForegroundColor, fill.ForeColor));
			if(!fill.BackColor.IsEmpty)
				properties.Add(new XfPropFullColor(XfExtType.BackgroundColor, fill.BackColor));
			XlBorder border = format.Border;
			if(border.TopLineStyle != XlBorderLineStyle.None && !border.TopColor.IsEmpty)
				properties.Add(new XfPropFullColor(XfExtType.TopBorderColor, border.TopColor));
			if(border.BottomLineStyle != XlBorderLineStyle.None && !border.BottomColor.IsEmpty)
				properties.Add(new XfPropFullColor(XfExtType.BottomBorderColor, border.BottomColor));
			if(border.LeftLineStyle != XlBorderLineStyle.None && !border.LeftColor.IsEmpty)
				properties.Add(new XfPropFullColor(XfExtType.LeftBorderColor, border.LeftColor));
			if(border.RightLineStyle != XlBorderLineStyle.None && !border.RightColor.IsEmpty)
				properties.Add(new XfPropFullColor(XfExtType.RightBorderColor, border.RightColor));
			if((border.DiagonalDownLineStyle != XlBorderLineStyle.None || border.DiagonalUpLineStyle != XlBorderLineStyle.None) && !border.DiagonalColor.IsEmpty)
				properties.Add(new XfPropFullColor(XfExtType.DiagonalBorderColor, border.DiagonalColor));
			XlFont font = fontList[format.FontId];
			if (!font.Color.IsEmpty)
				properties.Add(new XfPropFullColor(XfExtType.TextColor, font.Color));
			if(font.SchemeStyle != XlFontSchemeStyles.None)
				properties.Add(new XfPropByte(XfExtType.FontScheme, (byte)(font.SchemeStyle == XlFontSchemeStyles.Minor ? 0x02 : 0x01)));
			XlCellAlignment alignment = format.Alignment;
			if(alignment.Indent > 15)
				properties.Add(new XfPropUInt16(XfExtType.IndentLevel, Math.Min((int)alignment.Indent, 250)));
		}
		#endregion
		#region WriteStyles
		protected void WriteStyles() {
			XlsContentStyle content = new XlsContentStyle();
			content.BuiltInId = 0;
			content.IsBuiltIn = true;
			content.IsHidden = false;
			content.OutlineLevel = 0;
			content.StyleFormatId = 0;
			content.StyleName = "Normal";
			WriteContent(XlsRecordType.Style, content);
			WriteNormalStyleExt();
		}
		void WriteNormalStyleExt() {
			XlsContentStyleExt content = new XlsContentStyleExt();
			content.BuiltInId = 0;
			content.Category = XlStyleCategory.GoodBadNeutralStyle;
			content.CustomBuiltIn = false;
			content.IsBuiltIn = true;
			content.IsHidden = false;
			content.OutlineLevel = 0;
			content.StyleName = "Normal";
			content.Properties.Add(new XfPropByte(XfPropType.FontScheme, 2)); 
			WriteContent(XlsRecordType.StyleExt, content);
		}
		#endregion
		#region RegisterFormatting
		protected int RegisterFormatting(XlCellFormatting formatting) {
			if(formatting == null)
				return -1;
			int fontId = RegisterFont(formatting.Font);
			int numberFormatId = RegisterNumberFormat(formatting.NetFormatString, formatting.IsDateTimeFormatString, formatting.NumberFormat);
			XlsXf xf = new XlsXf();
			xf.FontId = Math.Max(0, fontId);
			xf.NumberFormatId = Math.Max(0, numberFormatId);
			xf.ApplyFont = (fontId >= 0);
			xf.ApplyFill = formatting.Fill != null;
			xf.ApplyNumberFormat = (numberFormatId >= 0);
			xf.ApplyBorder = formatting.Border != null;
			xf.ApplyAlignment = formatting.Alignment != null;
			xf.Fill = formatting.Fill != null ? formatting.Fill : this.defaultFill;
			xf.Border = formatting.Border != null ? formatting.Border : this.defaultBorder;
			xf.Alignment = formatting.Alignment != null ? formatting.Alignment : this.defaultAlignment;
			int index;
			if(xfTable.TryGetValue(xf, out index))
				return index;
			index = xfList.Count;
			xfTable.Add(xf, index);
			xfList.Add(xf);
			return index;
		}
		int RegisterFont(XlFont font) {
			if(font == null)
				return -1;
			int index;
			if(fontTable.TryGetValue(font, out index))
				return index;
			index = fontTable.Count;
			fontTable.Add(font, index);
			fontList.Add(font);
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
				if(numberFormatTable.ContainsKey(formatCode))
					numberFormat = numberFormatTable[formatCode];
				else {
					numberFormat = new ExcelNumberFormat(this.customNumberFormatId, formatCode);
					numberFormatTable.Add(formatCode, numberFormat);
					this.customNumberFormatId++;
				}
				return numberFormat.Id;
			}
			XlNetNumberFormat netFormat = new XlNetNumberFormat() { FormatString = netFormatString, IsDateTimeFormat = isDateTimeFormatString };
			if(netNumberFormatTable.TryGetValue(netFormat, out numberFormat))
				return numberFormat.Id;
			numberFormat = ConvertNetFormatStringToXlFormatCode(netFormatString, isDateTimeFormatString);
			if(numberFormat == null)
				return -1;
			if(String.IsNullOrEmpty(numberFormat.FormatString))
				return 0;
			if(numberFormat.Id >= 0)
				return numberFormat.Id;
			if(numberFormatTable.ContainsKey(numberFormat.FormatString))
				numberFormat = numberFormatTable[numberFormat.FormatString];
			else {
				numberFormat.Id = this.customNumberFormatId;
				numberFormatTable.Add(numberFormat.FormatString, numberFormat);
				this.customNumberFormatId++;
			}
			netNumberFormatTable.Add(netFormat, numberFormat);
			return numberFormat.Id;
		}
		ExcelNumberFormat ConvertNetFormatStringToXlFormatCode(string netFormatString, bool isDateTimeFormatString) {
			CultureInfo culture = options.Culture;
			if(culture == null)
				culture = CultureInfo.InvariantCulture;
			return numberFormatConverter.Convert(netFormatString, isDateTimeFormatString, culture);
		}
		#endregion
	}
	#endregion
	#region XlsXf
	internal class XlsXf {
		#region Fields
		const long MaskIsStyle		   = 0x0000000000000001; 
		const long MaskIsLocked		  = 0x0000000000000002; 
		const long MaskIsHidden		  = 0x0000000000000004; 
		const long MaskQuotePrefix	   = 0x0000000000000008; 
		const long MaskApplyNumberFormat = 0x0000000000000010; 
		const long MaskApplyFont		 = 0x0000000000000020; 
		const long MaskApplyFill		 = 0x0000000000000040; 
		const long MaskApplyBorder	   = 0x0000000000000080; 
		const long MaskApplyProtection   = 0x0000000000000100; 
		const long MaskApplyAlignment	= 0x0000000000000200; 
		const long MaskNumberFormatId	= 0x00000000001FF000; 
		const long MaskFontId			= 0x00000003FF000000; 
		const long MaskStyleId		   = 0x0001FFF000000000; 
		long packedValues = 0x000003f2;
		#endregion
		#region Properties
		public bool IsStyleFormat { get { return GetBooleanValue(MaskIsStyle); } set { SetBooleanValue(MaskIsStyle, value); } }
		public bool IsLocked { get { return GetBooleanValue(MaskIsLocked); } set { SetBooleanValue(MaskIsLocked, value); } }
		public bool IsHidden { get { return GetBooleanValue(MaskIsHidden); } set { SetBooleanValue(MaskIsHidden, value); } }
		public bool ApplyNumberFormat { get { return GetBooleanValue(MaskApplyNumberFormat); } set { SetBooleanValue(MaskApplyNumberFormat, value); } }
		public bool ApplyFont { get { return GetBooleanValue(MaskApplyFont); } set { SetBooleanValue(MaskApplyFont, value); } }
		public bool ApplyFill { get { return GetBooleanValue(MaskApplyFill); } set { SetBooleanValue(MaskApplyFill, value); } }
		public bool ApplyBorder { get { return GetBooleanValue(MaskApplyBorder); } set { SetBooleanValue(MaskApplyBorder, value); } }
		public bool ApplyAlignment { get { return GetBooleanValue(MaskApplyAlignment); } set { SetBooleanValue(MaskApplyAlignment, value); } }
		public bool ApplyProtection { get { return GetBooleanValue(MaskApplyProtection); } set { SetBooleanValue(MaskApplyProtection, value); } }
		public bool QuotePrefix { get { return GetBooleanValue(MaskQuotePrefix); } set { SetBooleanValue(MaskQuotePrefix, value); } }
		public int FontId {
			get { return (int)((packedValues & MaskFontId) >> 24); }
			set {
				packedValues &= ~MaskFontId;
				packedValues |= ((long)value << 24) & MaskFontId;
			}
		}
		public int NumberFormatId {
			get { return (int)((packedValues & MaskNumberFormatId) >> 12); }
			set {
				packedValues &= ~MaskNumberFormatId;
				packedValues |= ((long)value << 12) & MaskNumberFormatId;
			}
		}
		public int StyleId {
			get { return (int)((packedValues & MaskStyleId) >> 36); }
			set {
				packedValues &= ~MaskStyleId;
				packedValues |= ((long)value << 36) & MaskStyleId;
			}
		}
		public XlFill Fill { get; set; }
		public XlBorder Border { get; set; }
		public XlCellAlignment Alignment { get; set; }
		#endregion
		public override bool Equals(object obj) {
			XlsXf other = obj as XlsXf;
			if(other == null)
				return false;
			return packedValues == other.packedValues &&
				Fill.Equals(other.Fill) &&
				Border.Equals(other.Border) &&
				Alignment.Equals(other.Alignment);
		}
		public override int GetHashCode() {
			return packedValues.GetHashCode() ^ Fill.GetHashCode() ^ Border.GetHashCode() ^ Alignment.GetHashCode();
		}
		#region GetBooleanValue/SetBooleanValue helpers
		void SetBooleanValue(long mask, bool bitVal) {
			if(bitVal)
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
}
