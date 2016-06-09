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
using DevExpress.Office;
using DevExpress.Office.Services;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraSpreadsheet.Import.Xls;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Export.Xl;
using DevExpress.XtraExport.Xls;
using DevExpress.Office.Model;
using DevExpress.Office.Drawing;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Internal {
	#region XlsFontInfo
	public class XlsFontInfo {
		ColorModelInfo fontColor = new ColorModelInfo();
		#region Properties
		public bool Bold { get; set; }
		public short Boldness { get; set; }
		public bool Condense { get; set; }
		public bool Extend { get; set; }
		public bool Italic { get; set; }
		public bool Outline { get; set; }
		public bool Shadow { get; set; }
		public bool StrikeThrough { get; set; }
		public int Charset { get; set; }
		public int FontFamily { get; set; }
		public string Name { get; set; }
		public double Size { get; set; }
		public ColorModelInfo FontColor { 
			get { return fontColor; } 
			set {
				Guard.ArgumentNotNull(value, "FontColor");
				fontColor = value; 
			} 
		}
		public int FontColorIndex { get; set; }
		public XlFontSchemeStyles SchemeStyle { get; set; }
		public XlScriptType Script { get; set; }
		public XlUnderlineType Underline { get; set; }
		protected internal bool IsRegistered { get; set; }
		#endregion
		protected internal void Register(XlsImportStyleSheet styleSheet) {
			RunFontInfo info = CreateRunFontInfo(styleSheet);
			styleSheet.RegisterFont(info);
			IsRegistered = true;
		}
		protected internal RunFontInfo CreateRunFontInfo(XlsImportStyleSheet styleSheet) {
			RunFontInfo info = new RunFontInfo();
			info.Bold = Bold;
			info.Condense = Condense;
			info.Extend = Extend;
			info.Italic = Italic;
			info.Outline = Outline;
			info.Shadow = Shadow;
			info.StrikeThrough = StrikeThrough;
			info.Charset = Charset;
			info.FontFamily = ConvertFontFamily(FontFamily);
			info.Name = ConvertFontName(Name);
			info.Size = ConvertFontSize(Size);
			info.ColorIndex = styleSheet.RegisterColor(styleSheet.ConvertFontAutomaticColor(FontColor));
			info.SchemeStyle = SchemeStyle;
			info.Script = Script;
			info.Underline = Underline;
			return info;
		}
		int ConvertFontFamily(int value) {
			if(value > 5) {
				int family = (value & 0xf0) >> 4;
				int pitch = value & 0x0f;
				if((family >= 1 && family <= 5) && (pitch >= 0 && pitch <= 2))
					value = family;
				else 
					value = 0;
			}
			return value;
		}
		double ConvertFontSize(double value) {
			if(value < 1.0)
				value = 1.0;
			return value;
		}
		string ConvertFontName(string value) {
			if(value.Length > XlsDefs.MaxFontNameLength) {
				string[] parts = value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
				value = parts[0];
				for(int i = 1; i < parts.Length && value.Length < XlsDefs.MaxFontNameLength; i++) {
					if((value.Length + parts[i].Length + 1) < XlsDefs.MaxFontNameLength)
						value += ";" + parts[i];
				}
			}
			if(value.Length > XlsDefs.MaxFontNameLength)
				value = value.Remove(XlsDefs.MaxFontNameLength);
			if(string.IsNullOrEmpty(value))
				value = "Arial";
			return value;
		}
	}
	#endregion
	#region XlsDifferentialFormatInfo
	public class XlsDifferentialFormatInfo {
		#region Static members
		public static XlsDifferentialFormatInfo FromFormat(DifferentialFormat format) {
			XlsDifferentialFormatInfo result = new XlsDifferentialFormatInfo();
			result.CalculateProperties(format);
			return result;
		}
		#endregion
		#region Fields
		readonly List<XFGradStop> xfGradStops = new List<XFGradStop>();
		RunFontInfo fontInfo = RunFontInfo.CreateDefault(); 
		FillInfo fillInfo = new FillInfo();
		BorderInfo borderInfo = new BorderInfo();
		CellAlignmentInfo alignmentInfo = new CellAlignmentInfo();
		string numberFormatCode = NumberFormat.Generic.FormatCode;
		int numberFormatId = NumberFormatCollection.DefaultItemIndex;
		CellFormatFlagsInfo flagsInfo = CellFormatFlagsInfo.DefaultFormat;
		MultiOptionsInfo multiOptionsInfo;
		BorderOptionsInfo borderOptionsInfo;
		ColorModelInfo fontColor = new ColorModelInfo();
		ColorModelInfo fillForegroundColor = new ColorModelInfo();
		ColorModelInfo fillBackgroundColor = new ColorModelInfo();
		ColorModelInfo borderLeftColor = new ColorModelInfo();
		ColorModelInfo borderRightColor = new ColorModelInfo();
		ColorModelInfo borderTopColor = new ColorModelInfo();
		ColorModelInfo borderBottomColor = new ColorModelInfo();
		ColorModelInfo borderDiagonalColor = new ColorModelInfo();
		ColorModelInfo borderVerticalColor = new ColorModelInfo();
		ColorModelInfo borderHorizontalColor = new ColorModelInfo();
		XFPropertiesBase properties;
		bool isForegroundBackgroundColorSwapped = true;
		bool hasNumberFormatId;
		#endregion
		#region Properties
		#region FontProperties
		public bool FontBold { 
			get { return fontInfo.Bold; }
			set {
				fontInfo.Bold = value;
				multiOptionsInfo.ApplyFontBold = true;
			}
		}
		public bool FontCondense {
			get { return fontInfo.Condense; }
			set {
				fontInfo.Condense = value;
				multiOptionsInfo.ApplyFontCondense = true;
			}
		}
		public bool FontExtend {
			get { return fontInfo.Extend; }
			set {
				fontInfo.Extend = value;
				multiOptionsInfo.ApplyFontExtend = true;
			}
		}
		public bool FontItalic {
			get { return fontInfo.Italic; }
			set {
				fontInfo.Italic = value;
				multiOptionsInfo.ApplyFontItalic = true;
			}
		}
		public bool FontOutline {
			get { return fontInfo.Outline; }
			set {
				fontInfo.Outline = value;
				multiOptionsInfo.ApplyFontOutline = true;
			}
		}
		public bool FontShadow {
			get { return fontInfo.Shadow; }
			set {
				fontInfo.Shadow = value;
				multiOptionsInfo.ApplyFontShadow = true;
			}
		}
		public bool FontStrikeThrough {
			get { return fontInfo.StrikeThrough; }
			set {
				fontInfo.StrikeThrough = value;
				multiOptionsInfo.ApplyFontStrikeThrough = true;
			}
		}
		public int FontCharset {
			get { return fontInfo.Charset; }
			set {
				fontInfo.Charset = value;
				multiOptionsInfo.ApplyFontCharset = true;
			}
		}
		public int FontFamily {
			get { return fontInfo.FontFamily; }
			set {
				fontInfo.FontFamily = value;
				multiOptionsInfo.ApplyFontFamily = true;
			}
		}
		public string FontName {
			get { return fontInfo.Name; }
			set {
				fontInfo.Name = value;
				multiOptionsInfo.ApplyFontName = true;
			}
		}
		public double FontSize {
			get { return fontInfo.Size; }
			set {
				fontInfo.Size = value;
				multiOptionsInfo.ApplyFontSize = true;
			}
		}
		public ColorModelInfo FontColor {
			get { return fontColor; }
			set {
				Guard.ArgumentNotNull(value, "FontColor");
				fontColor = value;
				multiOptionsInfo.ApplyFontColor = true;
			}
		}
		public XlFontSchemeStyles FontSchemeStyle {
			get { return fontInfo.SchemeStyle; }
			set {
				fontInfo.SchemeStyle = value;
				multiOptionsInfo.ApplyFontSchemeStyle = true;
			}
		}
		public XlScriptType FontScript {
			get { return fontInfo.Script; }
			set {
				fontInfo.Script = value;
				multiOptionsInfo.ApplyFontScript = true;
			}
		}
		public XlUnderlineType FontUnderline {
			get { return fontInfo.Underline; }
			set {
				fontInfo.Underline = value;
				multiOptionsInfo.ApplyFontUnderline = true;
			}
		}
		#endregion
		#region NumberFormatProperties
		public int NumberFormatId {
			get { return numberFormatId; }
			set {
				numberFormatId = value;
				hasNumberFormatId = true;
				SetApplyNumberFormat();
			}
		}
		public string NumberFormatCode {
			get { return numberFormatCode; }
			set {
				Guard.ArgumentNotNull(value, "FormatCode");
				numberFormatCode = value;
				SetApplyNumberFormat();
			}
		}
		void SetApplyNumberFormat() {
			multiOptionsInfo.ApplyNumberFormat = true;
		}
		#endregion
		#region FillProperties
		public XlPatternType FillPatternType {
			get { return fillInfo.PatternType; }
			set {
				fillInfo.PatternType = value;
				multiOptionsInfo.ApplyFillPatternType = true;
			}
		}
		public ColorModelInfo FillForegroundColor {
			get { return fillForegroundColor; }
			set {
				Guard.ArgumentNotNull(value, "fillForegroundColor");
				fillForegroundColor = value;
				multiOptionsInfo.ApplyFillForeColor = true;
			}
		}
		public ColorModelInfo FillBackgroundColor {
			get { return fillBackgroundColor; }
			set {
				Guard.ArgumentNotNull(value, "fillBackgroundColor");
				fillBackgroundColor = value;
				multiOptionsInfo.ApplyFillBackColor = true;
			}
		}
		#endregion
		#region AlignmentProperties
		public XlHorizontalAlignment AlignmentHorizontal {
			get { return alignmentInfo.HorizontalAlignment; }
			set {
				alignmentInfo.HorizontalAlignment = value;
				multiOptionsInfo.ApplyAlignmentHorizontal = true;
			}
		}
		public XlVerticalAlignment AlignmentVertical {
			get { return alignmentInfo.VerticalAlignment; }
			set {
				alignmentInfo.VerticalAlignment = value;
				multiOptionsInfo.ApplyAlignmentVertical = true;
			}
		}
		public bool AlignmentWrapText {
			get { return alignmentInfo.WrapText; }
			set {
				alignmentInfo.WrapText = value;
				multiOptionsInfo.ApplyAlignmentWrapText = true;
			}
		}
		public bool AlignmentShrinkToFit {
			get { return alignmentInfo.ShrinkToFit; }
			set {
				alignmentInfo.ShrinkToFit = value;
				multiOptionsInfo.ApplyAlignmentShrinkToFit = true;
			}
		}
		public bool AlignmentJustifyLastLine {
			get { return alignmentInfo.JustifyLastLine; }
			set {
				alignmentInfo.JustifyLastLine = value;
				multiOptionsInfo.ApplyAlignmentJustifyLastLine = true;
			}
		}
		public XlReadingOrder AlignmentReadingOrder {
			get { return alignmentInfo.ReadingOrder; }
			set {
				alignmentInfo.ReadingOrder = value;
				multiOptionsInfo.ApplyAlignmentReadingOrder = true;
			}
		}
		public int AlignmentTextRotation {
			get { return alignmentInfo.TextRotation; }
			set {
				alignmentInfo.TextRotation = value;
				multiOptionsInfo.ApplyAlignmentTextRotation = true;
			}
		}
		public int AlignmentRelativeIndent {
			get { return alignmentInfo.RelativeIndent; }
			set {
				alignmentInfo.RelativeIndent = value;
				multiOptionsInfo.ApplyAlignmentRelativeIndent = true;
			}
		}
		public byte AlignmentIndent {
			get { return alignmentInfo.Indent; }
			set {
				alignmentInfo.Indent = value;
				multiOptionsInfo.ApplyAlignmentIndent = true;
			}
		}
		#endregion
		#region BorderProperties
		public XlBorderLineStyle BorderLeftLineStyle {
			get { return borderInfo.LeftLineStyle; }
			set {
				borderInfo.LeftLineStyle = value;
				borderOptionsInfo.ApplyLeftLineStyle = true;
			}
		}
		public ColorModelInfo BorderLeftColor {
			get { return borderLeftColor; }
			set {
				Guard.ArgumentNotNull(value, "BorderLeftColor");
				borderLeftColor = value;
				borderOptionsInfo.ApplyLeftColor = true;
			}
		}
		public XlBorderLineStyle BorderRightLineStyle {
			get { return borderInfo.RightLineStyle; }
			set {
				borderInfo.RightLineStyle = value;
				borderOptionsInfo.ApplyRightLineStyle = true;
			}
		}
		public ColorModelInfo BorderRightColor {
			get { return borderRightColor; }
			set {
				Guard.ArgumentNotNull(value, "BorderRightColor");
				borderRightColor = value;
				borderOptionsInfo.ApplyRightColor = true;
			}
		}
		public XlBorderLineStyle BorderTopLineStyle {
			get { return borderInfo.TopLineStyle; }
			set {
				borderInfo.TopLineStyle = value;
				borderOptionsInfo.ApplyTopLineStyle = true;
			}
		}
		public ColorModelInfo BorderTopColor {
			get { return borderTopColor; }
			set {
				Guard.ArgumentNotNull(value, "BorderTopColor");
				borderTopColor = value;
				borderOptionsInfo.ApplyTopColor = true;
			}
		}
		public XlBorderLineStyle BorderBottomLineStyle {
			get { return borderInfo.BottomLineStyle; }
			set {
				borderInfo.BottomLineStyle = value;
				borderOptionsInfo.ApplyBottomLineStyle = true;
			}
		}
		public ColorModelInfo BorderBottomColor {
			get { return borderBottomColor; }
			set {
				Guard.ArgumentNotNull(value, "BorderBottomColor");
				borderBottomColor = value;
				borderOptionsInfo.ApplyBottomColor = true;
			}
		}
		public XlBorderLineStyle BorderHorizontalLineStyle {
			get { return borderInfo.HorizontalLineStyle; }
			set {
				borderInfo.HorizontalLineStyle = value;
				borderOptionsInfo.ApplyHorizontalLineStyle = true;
			}
		}
		public ColorModelInfo BorderHorizontalColor {
			get { return borderHorizontalColor; }
			set {
				Guard.ArgumentNotNull(value, "BorderHorizontalColor");
				borderHorizontalColor = value;
				borderOptionsInfo.ApplyHorizontalColor = true;
			}
		}
		public XlBorderLineStyle BorderVerticalLineStyle {
			get { return borderInfo.VerticalLineStyle; }
			set {
				borderInfo.VerticalLineStyle = value;
				borderOptionsInfo.ApplyVerticalLineStyle = true;
			}
		}
		public ColorModelInfo BorderVerticalColor {
			get { return borderVerticalColor; }
			set {
				Guard.ArgumentNotNull(value, "BorderVerticalColor");
				borderVerticalColor = value;
				borderOptionsInfo.ApplyVerticalColor = true;
			}
		}
		public XlBorderLineStyle BorderDiagonalLineStyle {
			get { return borderInfo.DiagonalLineStyle; }
			set {
				borderInfo.DiagonalLineStyle = value;
				borderOptionsInfo.ApplyDiagonalLineStyle = true;
			}
		}
		public ColorModelInfo BorderDiagonalColor {
			get { return borderDiagonalColor; }
			set {
				Guard.ArgumentNotNull(value, "BorderDiagonalColor");
				borderDiagonalColor = value;
				borderOptionsInfo.ApplyDiagonalColor = true;
			}
		}
		public bool BorderOutline {
			get { return borderInfo.Outline; }
			set {
				borderInfo.Outline = value;
				borderOptionsInfo.ApplyOutline = true;
			}
		}
		public bool BorderDiagonalUp {
			get { return borderInfo.DiagonalUp; }
			set {
				borderInfo.DiagonalUp = value;
				borderOptionsInfo.ApplyDiagonalUp = true;
			}
		}
		public bool BorderDiagonalDown {
			get { return borderInfo.DiagonalDown; }
			set {
				borderInfo.DiagonalDown = value;
				borderOptionsInfo.ApplyDiagonalDown = true;
			}
		}
		#endregion
		#region ProtectionProperties
		public bool ProtectionHidden {
			get { return flagsInfo.Hidden; }
			set {
				flagsInfo.Hidden = value;
				multiOptionsInfo.ApplyProtectionHidden = true;
			}
		}
		public bool ProtectionLocked {
			get { return flagsInfo.Locked; }
			set {
				flagsInfo.Locked = value;
				multiOptionsInfo.ApplyProtectionLocked = true;
			}
		}
		#endregion
		public RunFontInfo FontInfo { get { return fontInfo; } }
		public FillInfo FillInfo { get { return fillInfo; } }
		public ModelFillType FillType { get; set; }
		public GradientFillInfo GradientFillInfo { get; set; }
		public BorderInfo BorderInfo { get { return borderInfo; } }
		public CellAlignmentInfo AlignmentInfo { get { return alignmentInfo; } }
		public CellFormatFlagsInfo CellFormatFlagsInfo { get { return flagsInfo; } }
		public MultiOptionsInfo MultiOptionsInfo { get { return multiOptionsInfo; } }
		public BorderOptionsInfo BorderOptionsInfo { get { return borderOptionsInfo; } }
		public bool IsForegroundBackgroundColorSwapped { 
			get { return isForegroundBackgroundColorSwapped; } 
			set { isForegroundBackgroundColorSwapped = value; } 
		}
		protected internal List<XFGradStop> XFGradStops { get { return xfGradStops; } }
		#endregion
		protected internal void Register(XlsImportStyleSheet styleSheet) {
			styleSheet.RegisterDifferentialFormat(GetDifferentialFormat(styleSheet));
		}
		protected internal DifferentialFormat GetDifferentialFormat(XlsImportStyleSheet styleSheet) {
			DocumentCache cache = styleSheet.DocumentModel.Cache;
			DifferentialFormat format = new DifferentialFormat(styleSheet.DocumentModel);
			fontInfo.ColorIndex = styleSheet.RegisterColor(FontColor);
			borderInfo.LeftColorIndex = styleSheet.RegisterColor(BorderLeftColor);
			borderInfo.RightColorIndex = styleSheet.RegisterColor(BorderRightColor);
			borderInfo.TopColorIndex = styleSheet.RegisterColor(BorderTopColor);
			borderInfo.BottomColorIndex = styleSheet.RegisterColor(BorderBottomColor);
			borderInfo.DiagonalColorIndex = styleSheet.RegisterColor(BorderDiagonalColor);
			borderInfo.VerticalColorIndex = styleSheet.RegisterColor(BorderVerticalColor);
			borderInfo.HorizontalColorIndex = styleSheet.RegisterColor(BorderHorizontalColor);
			int fontIndex = cache.FontInfoCache.AddItem(fontInfo);
			int alignmentIndex = cache.CellAlignmentInfoCache.AddItem(alignmentInfo);
			int borderIndex = cache.BorderInfoCache.AddItem(borderInfo);
			int cellFormatFlagsIndex = flagsInfo.PackedValues;
			int multiOptionsIndex = multiOptionsInfo.PackedValues;
			int borderOptionsIndex = borderOptionsInfo.PackedValues;
			AssignNumberFormatIndexes(format);
			format.AssignFontIndex(fontIndex);
			format.AssignAlignmentIndex(alignmentIndex);
			format.AssignBorderIndex(borderIndex);
			format.AssignCellFormatFlagsIndex(cellFormatFlagsIndex);
			format.AssignMultiOptionsIndex(multiOptionsIndex);
			format.AssignBorderOptionsIndex(borderOptionsIndex);
			if (FillType == ModelFillType.Gradient)
				AssignGradientFillInfo(format);
			else {
				fillInfo.ForeColorIndex = styleSheet.RegisterColor(FillForegroundColor);
				fillInfo.BackColorIndex = styleSheet.RegisterColor(FillBackgroundColor);
				int fillIndex = cache.FillInfoCache.AddItem(fillInfo);
				format.AssignFillIndex(fillIndex);
			}
			return format;
		}
		void AssignNumberFormatIndexes(DifferentialFormat format) {
			format.BeginUpdate();
			try {
				format.FormatString = NumberFormatCode;
				if (hasNumberFormatId) 
					format.NumberFormatId = NumberFormatId;
			} finally {
				format.EndUpdate();
			}
		}
		protected void CalculateProperties(DifferentialFormat format) {
			ColorModelInfoCache cache = format.DocumentModel.Cache.ColorModelInfoCache;
			fontInfo = format.FontInfo;
			borderInfo = format.BorderInfo;
			alignmentInfo = format.AlignmentInfo;
			numberFormatCode = format.FormatString;
			numberFormatId = format.NumberFormatIndex;
			flagsInfo = format.CellFormatFlagsInfo;
			multiOptionsInfo = format.MultiOptionsInfo;
			borderOptionsInfo = format.BorderOptionsInfo;
			fontColor = cache[format.FontInfo.ColorIndex];
			borderLeftColor = cache[format.BorderInfo.LeftColorIndex];
			borderRightColor = cache[format.BorderInfo.RightColorIndex];
			borderTopColor = cache[format.BorderInfo.TopColorIndex];
			borderBottomColor = cache[format.BorderInfo.BottomColorIndex];
			borderDiagonalColor = cache[format.BorderInfo.DiagonalColorIndex];
			borderVerticalColor = cache[format.BorderInfo.VerticalColorIndex];
			borderHorizontalColor = cache[format.BorderInfo.HorizontalColorIndex];
			if (CellFormatFlagsInfo.FillType == ModelFillType.Gradient) {
				GradientFillInfo = format.GradientFillInfo;
				GradientStopInfoCache stopCache = format.DocumentModel.Cache.GradientStopInfoCache;
				GradientStopInfoCollection stops = format.GradientStopInfoCollection;
				int count = stops.Count;
				for (int i = 0; i < count; i++) {
					int formatIndex = stops[i];
					DevExpress.XtraSpreadsheet.Model.GradientStopInfo stop = stopCache[formatIndex];
					XFGradStops.Add(new XFGradStop(stop.Position, cache[stop.ColorIndex]));
				}
			}
			else {
				fillInfo = format.FillInfo;
				fillForegroundColor = cache[format.FillInfo.ForeColorIndex];
				fillBackgroundColor = cache[format.FillInfo.BackColorIndex];
			}
		}
		protected internal void CreateDifferentialFormatProperties(XlsCommandDifferentialFormat command) {
			this.properties = command.Properties;
			if (BorderOptionsInfo.ApplyOutline)
				command.NewBorders = BorderOutline;
			AssignFill();
			AssignColorProperty(new XFPropTextColor(), MultiOptionsInfo.ApplyFontColor, FontColor);
			AssignBorders();
			AssignAlignmentWithoutRelativeIndent();
			AssignFontWithoutColor();
			AssignNumberFormat();
			if (MultiOptionsInfo.ApplyAlignmentRelativeIndent) {
				XFPropRelativeIndentation property = new XFPropRelativeIndentation();
				property.Value = AlignmentRelativeIndent;
				properties.Add(property);
			}
			AssignProtection();
		}
		protected internal void CreateDifferentialFormatExtProperties(XFExtProperties extProperties) {
			this.properties = extProperties;
			if (CellFormatFlagsInfo.FillType == ModelFillType.Pattern) {
				AssignColorProperty(new XFExtPropForegroundColor(), MultiOptionsInfo.ApplyFillForeColor, FillForegroundColor);
				AssignColorProperty(new XFExtPropBackgroundColor(), MultiOptionsInfo.ApplyFillBackColor, FillBackgroundColor);
			}
			else {
				if (ShouldAssignExtGradientStopProperty()) {
					XFExtPropGradient property = new XFExtPropGradient();
					property.Gradient.Info = GradientFillInfo;
					int count = XFGradStops.Count;
					for (int i = 0; i < count; i++)
						property.Stops.Add(XFGradStops[i]);
					properties.Add(property);
				}
			}
			AssignColorProperty(new XFExtPropTopBorderColor(), BorderOptionsInfo.ApplyTopColor, BorderTopColor);
			AssignColorProperty(new XFExtPropBottomBorderColor(), BorderOptionsInfo.ApplyBottomColor, BorderBottomColor);
			AssignColorProperty(new XFExtPropLeftBorderColor(), BorderOptionsInfo.ApplyLeftColor, BorderLeftColor);
			AssignColorProperty(new XFExtPropRightBorderColor(), BorderOptionsInfo.ApplyRightColor, BorderRightColor);
			AssignColorProperty(new XFExtPropDiagonalBorderColor(), BorderOptionsInfo.ApplyDiagonalColor, BorderDiagonalColor);
			AssignColorProperty(new XFExtPropTextColor(), MultiOptionsInfo.ApplyFontColor, FontColor);
			if (MultiOptionsInfo.ApplyFontSchemeStyle && FontSchemeStyle != XlFontSchemeStyles.None) {
				XFExtPropFontScheme fontSchemeProperty = new XFExtPropFontScheme();
				fontSchemeProperty.Value = FontSchemeStyle;
				properties.Add(fontSchemeProperty);
			}
		}
		protected internal void UpdateFromExtProperties(XlsDifferentialFormatInfo formatInfo) {
			MultiOptionsInfo multiOptions = formatInfo.MultiOptionsInfo;
			BorderOptionsInfo borderOptions = formatInfo.BorderOptionsInfo;
			if (multiOptions.ApplyFillForeColor) 
				FillForegroundColor = formatInfo.FillForegroundColor;
			if (multiOptions.ApplyFillBackColor)
				FillBackgroundColor = formatInfo.FillBackgroundColor;
			if (borderOptions.ApplyTopColor)
				BorderTopColor = formatInfo.BorderTopColor;
			if (borderOptions.ApplyBottomColor)
				BorderBottomColor = formatInfo.BorderBottomColor;
			if (borderOptions.ApplyLeftColor)
				BorderLeftColor = formatInfo.BorderLeftColor;
			if (borderOptions.ApplyRightColor)
				BorderRightColor = formatInfo.BorderRightColor;
			if (borderOptions.ApplyDiagonalColor)
				BorderDiagonalColor = formatInfo.BorderDiagonalColor;
			if (multiOptions.ApplyFontColor)
				FontColor = formatInfo.FontColor;
			if (multiOptions.ApplyFontSchemeStyle)
				FontSchemeStyle = formatInfo.FontSchemeStyle;
		}
		#region AssignProperties
		void AssignBoolProperty(XFPropBoolBase property, bool apply, bool value) {
			if (apply) {
				property.Value = value;
				properties.Add(property);
			}
		}
		void AssignColorProperty(XFPropColorBase property, bool apply, ColorModelInfo value) {
			if (apply) {
				property.ColorInfo = value;
				properties.Add(property);
			}
		}
		void AssignColorProperty(XFExtPropFullColorBase property, bool apply, ColorModelInfo value) {
			if (apply && ShouldAssignExtColorProperty(value)) {
				property.ColorInfo = value;
				properties.Add(property);
			}
		}
		bool ShouldAssignExtColorProperty(ColorModelInfo info) {
			return info.ColorType != ColorType.Index && info.ColorType != ColorType.Auto;
		}
		bool ShouldAssignExtGradientStopProperty() {
			if (flagsInfo.FillType == ModelFillType.Pattern)
				return false;
			int count = XFGradStops.Count;
			for (int i = 0; i < count; i++) {
				XFGradStop stop = XFGradStops[i];
				if (stop.ColorInfo.ColorType == ColorType.Auto) 
					return false;
			}
			return true;
		}
		void AssignBorderProperty(XFPropBorderBase property, bool applyLineStyle, bool applyColor, XlBorderLineStyle lineStyle, ColorModelInfo colorInfo) {
			if (applyLineStyle)
				property.LineStyle = lineStyle;
			if (applyColor)
				property.ColorInfo = colorInfo;
			if (applyLineStyle || applyColor)
				properties.Add(property);
		}
		void AssignFill() {
			if (CellFormatFlagsInfo.FillType == ModelFillType.Gradient) 
				AssignGradientFill();
			else
				AssignPatternFill();
		}
		void AssignPatternFill() {
			if (MultiOptionsInfo.ApplyFillPatternType) {
				XFPropFillPattern property = new XFPropFillPattern();
				property.FillPatternType = FillPatternType;
				properties.Add(property);
			}
			AssignColorProperty(new XFPropForegroundColor(), MultiOptionsInfo.ApplyFillForeColor, FillForegroundColor);
			AssignColorProperty(new XFPropBackgroundColor(), MultiOptionsInfo.ApplyFillBackColor, FillBackgroundColor);
		}
		void AssignGradientFill() {
			XFPropGradient property = new XFPropGradient();
			property.Gradient.Info = GradientFillInfo;
			properties.Add(property);
			int count = XFGradStops.Count;
			for (int i = 0; i < count; i++) {
				XFPropGradientStop stopProperty = new XFPropGradientStop();
				XFGradStop stop = XFGradStops[i];
				stopProperty.Position = stop.Position;
				stopProperty.ColorInfo = stop.ColorInfo;
				properties.Add(stopProperty);
			}
		}
		void AssignBorders() {
			AssignBorderProperty(new XFPropTopBorder(), BorderOptionsInfo.ApplyTopLineStyle, BorderOptionsInfo.ApplyTopColor, BorderTopLineStyle, BorderTopColor);
			AssignBorderProperty(new XFPropBottomBorder(), BorderOptionsInfo.ApplyBottomLineStyle, BorderOptionsInfo.ApplyBottomColor, BorderBottomLineStyle, BorderBottomColor);
			AssignBorderProperty(new XFPropLeftBorder(), BorderOptionsInfo.ApplyLeftLineStyle, BorderOptionsInfo.ApplyLeftColor, BorderLeftLineStyle, BorderLeftColor);
			AssignBorderProperty(new XFPropRightBorder(), BorderOptionsInfo.ApplyRightLineStyle, BorderOptionsInfo.ApplyRightColor, BorderRightLineStyle, BorderRightColor);
			AssignBorderProperty(new XFPropDiagonalBorder(), BorderOptionsInfo.ApplyDiagonalLineStyle, BorderOptionsInfo.ApplyDiagonalColor, BorderDiagonalLineStyle, BorderDiagonalColor);
			AssignBorderProperty(new XFPropVerticalBorder(), BorderOptionsInfo.ApplyVerticalLineStyle, BorderOptionsInfo.ApplyVerticalColor, BorderVerticalLineStyle, BorderVerticalColor);
			AssignBorderProperty(new XFPropHorizontalBorder(), BorderOptionsInfo.ApplyHorizontalLineStyle, BorderOptionsInfo.ApplyHorizontalColor, BorderHorizontalLineStyle, BorderHorizontalColor);
			AssignBoolProperty(new XFPropDiagonalUpBorder(), BorderOptionsInfo.ApplyDiagonalUp, BorderDiagonalUp);
			AssignBoolProperty(new XFPropDiagonalDownBorder(), BorderOptionsInfo.ApplyDiagonalDown, BorderDiagonalDown);
		}
		void AssignAlignmentWithoutRelativeIndent() {
			if (MultiOptionsInfo.ApplyAlignmentHorizontal) {
				XFPropHorizontalAlignment property = new XFPropHorizontalAlignment();
				property.Value = AlignmentInfo.HorizontalAlignment;
				properties.Add(property);
			}
			if (MultiOptionsInfo.ApplyAlignmentVertical) {
				XFPropVerticalAlignment property = new XFPropVerticalAlignment();
				property.Value = AlignmentInfo.VerticalAlignment;
				properties.Add(property);
			}
			if (MultiOptionsInfo.ApplyAlignmentTextRotation) {
				XFPropTextRotation property = new XFPropTextRotation();
				property.Value = AlignmentInfo.TextRotation;
				properties.Add(property);
			}
			if (MultiOptionsInfo.ApplyAlignmentIndent) {
				XFPropIndentation property = new XFPropIndentation();
				property.Value = AlignmentInfo.Indent;
				properties.Add(property);
			}
			if (MultiOptionsInfo.ApplyAlignmentReadingOrder) {
				XFPropReadingOrder property = new XFPropReadingOrder();
				property.Value = AlignmentInfo.ReadingOrder;
				properties.Add(property);
			}
			AssignBoolProperty(new XFPropTextWrapped(), MultiOptionsInfo.ApplyAlignmentWrapText, AlignmentInfo.WrapText);
			AssignBoolProperty(new XFPropTextJustifyDistributed(), MultiOptionsInfo.ApplyAlignmentJustifyLastLine, AlignmentInfo.JustifyLastLine);
			AssignBoolProperty(new XFPropShrinkToFit(), MultiOptionsInfo.ApplyAlignmentShrinkToFit, AlignmentInfo.ShrinkToFit);
		}
		void AssignFontWithoutColor() {
			if (MultiOptionsInfo.ApplyFontName) {
				XFPropFontName property = new XFPropFontName();
				property.Value = FontInfo.Name;
				properties.Add(property);
			}
			AssignBoolProperty(new XFPropFontBold(), MultiOptionsInfo.ApplyFontBold, FontInfo.Bold);
			if (MultiOptionsInfo.ApplyFontUnderline) {
				XFPropFontUnderline property = new XFPropFontUnderline();
				property.Underline = FontInfo.Underline;
				properties.Add(property);
			}
			if (MultiOptionsInfo.ApplyFontScript) {
				XFPropFontScript property = new XFPropFontScript();
				property.Script = FontInfo.Script;
				properties.Add(property);
			}
			AssignBoolProperty(new XFPropTextItalic(), MultiOptionsInfo.ApplyFontItalic, FontInfo.Italic);
			AssignBoolProperty(new XFPropTextStrikethrough(), MultiOptionsInfo.ApplyFontStrikeThrough, FontInfo.StrikeThrough);
			AssignBoolProperty(new XFPropHasOutlineStyle(), MultiOptionsInfo.ApplyFontOutline, FontInfo.Outline);
			AssignBoolProperty(new XFPropHasShadowStyle(), MultiOptionsInfo.ApplyFontShadow, FontInfo.Shadow);
			AssignBoolProperty(new XFPropTextCondensed(), MultiOptionsInfo.ApplyFontCondense, FontInfo.Condense);
			AssignBoolProperty(new XFPropTextExtended(), MultiOptionsInfo.ApplyFontExtend, FontInfo.Extend);
			if (MultiOptionsInfo.ApplyFontCharset) {
				XFPropCharset property = new XFPropCharset();
				property.Value = FontInfo.Charset;
				properties.Add(property);
			}
			if (MultiOptionsInfo.ApplyFontFamily) {
				XFPropFontFamily property = new XFPropFontFamily();
				property.Value = FontInfo.FontFamily;
				properties.Add(property);
			}
			if (MultiOptionsInfo.ApplyFontSize) {
				XFPropTextSizeInTwips property = new XFPropTextSizeInTwips();
				property.Value = (int)(FontInfo.Size * 20.0);
				properties.Add(property);
			}
			if (MultiOptionsInfo.ApplyFontSchemeStyle) {
				XFPropFontScheme property = new XFPropFontScheme();
				property.Value = FontInfo.SchemeStyle;
				properties.Add(property);
			}
		}
		void AssignNumberFormat() {
			if (MultiOptionsInfo.ApplyNumberFormat) {
				XFPropNumberFormat numberFormatProperty = new XFPropNumberFormat();
				numberFormatProperty.Value = NumberFormatCode;
				properties.Add(numberFormatProperty);
				XFPropNumberFormatId numberFormatIdProperty = new XFPropNumberFormatId();
				numberFormatIdProperty.Value = NumberFormatId;
				properties.Add(numberFormatIdProperty);
			}
		}
		void AssignProtection() {
			AssignBoolProperty(new XFPropLockedProtection(), MultiOptionsInfo.ApplyProtectionLocked, CellFormatFlagsInfo.Locked);
			AssignBoolProperty(new XFPropHiddenProtection(), MultiOptionsInfo.ApplyProtectionHidden, CellFormatFlagsInfo.Hidden);
		}
		#endregion 
		protected internal void SetXFGradientStops(List<XFGradStop> stops) {
			xfGradStops.Clear();
			int count = stops.Count;
			for (int i = 0; i < count; i++)
				xfGradStops.Add(stops[i]);
		}
		protected internal void AssignGradientFillInfo(DifferentialFormat format) {
			DocumentModel documentModel = format.DocumentModel;
			format.AssignGradientFillInfoIndex(documentModel.Cache.GradientFillInfoCache.AddItem(GradientFillInfo));
			format.AssignCellFormatFlagsIndex(format.CellFormatFlagsIndex + CellFormatFlagsInfo.MaskFillType);
			int count = XFGradStops.Count;
			for (int i = 0; i < count; i++)
				format.GradientStopInfoCollection.AddCore(GetGradientStopInfoIndex(documentModel, XFGradStops[i]));
		}
		int GetGradientStopInfoIndex(DocumentModel documentModel, XFGradStop stop) {
			DevExpress.XtraSpreadsheet.Model.GradientStopInfo info = new DevExpress.XtraSpreadsheet.Model.GradientStopInfo();
			info.Position = stop.Position;
			info.SetColorIndex(documentModel, stop.ColorInfo);
			return documentModel.Cache.GradientStopInfoCache.GetItemIndex(info);
		}
	}
	#endregion
	#region XlsExtendedFormatInfo
	public class XlsExtendedFormatInfo {
		#region Fields
		readonly List<XFGradStop> xfGradStops = new List<XFGradStop>();
		ColorModelInfo foregroundColor = new ColorModelInfo();
		ColorModelInfo backgroundColor = new ColorModelInfo();
		ColorModelInfo leftBorderColor = new ColorModelInfo();
		ColorModelInfo rightBorderColor = new ColorModelInfo();
		ColorModelInfo topBorderColor = new ColorModelInfo();
		ColorModelInfo bottomBorderColor = new ColorModelInfo();
		ColorModelInfo diagonalBorderColor = new ColorModelInfo();
		ColorModelInfo verticalBorderColor = new ColorModelInfo();
		ColorModelInfo horizontalBorderColor = new ColorModelInfo();
		#endregion
		#region Properties
		public int FontId { get; set; }
		public int NumberFormatId { get; set; }
		public string NumberFormatCode { get; set; }
		public bool IsLocked { get; set; }
		public bool IsHidden { get; set; }
		public bool IsStyleFormat { get; set; }
		public int StyleXFIndex { get; set; }
		public bool QuotePrefix { get; set; }
		public bool WrapText { get; set; }
		public XlHorizontalAlignment HorizontalAlignment { get; set; }
		public XlVerticalAlignment VerticalAlignment { get; set; }
		public int TextRotation { get; set; }
		public byte Indent { get; set; }
		public bool ShrinkToFit { get; set; }
		public XlReadingOrder ReadingOrder { get; set; }
		public bool JustifyDistributed { get; set; }
		public int RelativeIndent { get; set; }
		public bool ApplyNumberFormat { get; set; }
		public bool ApplyFont { get; set; }
		public bool ApplyAlignment { get; set; }
		public bool ApplyBorder { get; set; }
		public bool ApplyFill { get; set; }
		public bool ApplyProtection { get; set; }
		public XlPatternType FillPatternType { get; set; }
		public ColorModelInfo ForegroundColor {
			get { return foregroundColor; }
			set {
				Guard.ArgumentNotNull(value, "ForegroundColor");
				foregroundColor = value;
			}
		}
		public ColorModelInfo BackgroundColor {
			get { return backgroundColor; }
			set {
				Guard.ArgumentNotNull(value, "BackgroundColor");
				backgroundColor = value;
			}
		}
		public ColorModelInfo LeftBorderColor {
			get { return leftBorderColor; }
			set {
				Guard.ArgumentNotNull(value, "LeftBorderColor");
				leftBorderColor = value;
			}
		}
		public ColorModelInfo RightBorderColor {
			get { return rightBorderColor; }
			set {
				Guard.ArgumentNotNull(value, "RightBorderColor");
				rightBorderColor = value;
			}
		}
		public ColorModelInfo TopBorderColor {
			get { return topBorderColor; }
			set {
				Guard.ArgumentNotNull(value, "TopBorderColor");
				topBorderColor = value;
			}
		}
		public ColorModelInfo BottomBorderColor {
			get { return bottomBorderColor; }
			set {
				Guard.ArgumentNotNull(value, "BottomBorderColor");
				bottomBorderColor = value;
			}
		}
		public ColorModelInfo DiagonalBorderColor {
			get { return diagonalBorderColor; }
			set {
				Guard.ArgumentNotNull(value, "DiagonalBorderColor");
				diagonalBorderColor = value;
			}
		}
		public ColorModelInfo VerticalBorderColor {
			get { return verticalBorderColor; }
			set {
				Guard.ArgumentNotNull(value, "VerticalBorderColor");
				verticalBorderColor = value;
			}
		}
		public ColorModelInfo HorizontalBorderColor {
			get { return horizontalBorderColor; }
			set {
				Guard.ArgumentNotNull(value, "HorizontalBorderColor");
				horizontalBorderColor = value;
			}
		}
		public GradientFillInfo GradientFillInfo { get; set; }
		public XlBorderLineStyle LeftBorderLineStyle { get; set; }
		public XlBorderLineStyle RightBorderLineStyle { get; set; }
		public XlBorderLineStyle TopBorderLineStyle { get; set; }
		public XlBorderLineStyle BottomBorderLineStyle { get; set; }
		public XlBorderLineStyle DiagonalBorderLineStyle { get; set; }
		public XlBorderLineStyle VerticalBorderLineStyle { get; set; }
		public XlBorderLineStyle HorizontalBorderLineStyle { get; set; }
		public bool DiagonalDownBorder { get; set; }
		public bool DiagonalUpBorder { get; set; }
		public bool HasExtension { get; set; }
		public bool PivotButton { get; set; }
		protected internal bool IsRegistered { get; set; }
		protected internal int FillId { get; set; }
		protected internal int BorderId { get; set; }
		protected internal ModelFillType FillType { get; set; }
		protected internal List<XFGradStop> XFGradStops { get { return xfGradStops; } }
		#endregion
		protected internal void Register(XlsImportStyleSheet styleSheet) {
			ImportCellFormatInfo cellFormatInfo = new ImportCellFormatInfo(IsStyleFormat ? CellFormatFlagsInfo.DefaultStyle : CellFormatFlagsInfo.DefaultFormat);
			cellFormatInfo.FontId = FontId;
			cellFormatInfo.NumberFormatId = NumberFormatId;
			cellFormatInfo.QuotePrefix = QuotePrefix;
			cellFormatInfo.IsLocked = IsLocked;
			cellFormatInfo.IsHidden = IsHidden;
			if(!IsStyleFormat) {
				cellFormatInfo.StyleId = styleSheet.GetCellStyleFormatIndex(StyleXFIndex);
				cellFormatInfo.HasExtension = HasExtension;
				cellFormatInfo.PivotButton = PivotButton;
			}
			cellFormatInfo.ApplyNumberFormat = ApplyNumberFormat;
			cellFormatInfo.ApplyFont = ApplyFont;
			cellFormatInfo.ApplyAlignment = ApplyAlignment;
			cellFormatInfo.ApplyBorder = ApplyBorder;
			cellFormatInfo.ApplyFill = ApplyFill;
			cellFormatInfo.ApplyProtection = ApplyProtection;
			cellFormatInfo.FillType = FillType;
			CellAlignmentInfo cellAlignmentInfo = CreateAlignmentInfo(styleSheet);
			BorderInfo borderInfo = CreateBorderInfo(styleSheet);
			if (FillType == ModelFillType.Gradient) {
				GradientStopInfoCollection stops = CreateGradientStopInfoCollection(styleSheet.DocumentModel);
				styleSheet.RegisterGradientFill(GradientFillInfo, stops);
			}
			else
				styleSheet.RegisterPatternFill(CreateFillInfo(styleSheet));
			cellFormatInfo.AlignmentIndex = styleSheet.RegisterCellAlignment(cellAlignmentInfo);
			styleSheet.RegisterBorder(borderInfo);
			cellFormatInfo.BorderId = BorderId = styleSheet.BorderInfoTable.Count - 1;
			cellFormatInfo.FillId = FillId = styleSheet.FillInfoTable.Count - 1;
			if (IsStyleFormat)
				styleSheet.RegisterCellStyleFormat(cellFormatInfo);
			else
				styleSheet.RegisterCellFormat(cellFormatInfo);
			IsRegistered = true;
		}
		protected internal bool Update(FormatBase format, XlsImportStyleSheet styleSheet) {
			if (IsRegistered) return false;
			format.BeginUpdate();
			try {
				format.Protection.Hidden = IsHidden;
				format.Protection.Locked = IsLocked;
			}
			finally {
				format.EndUpdate();
			}
			DocumentCache cache = styleSheet.DocumentModel.Cache;
			XlsFontInfo font = styleSheet.GetFontInfo(FontId);
			if(font != null && !font.IsRegistered) {
				RunFontInfo fontInfo = font.CreateRunFontInfo(styleSheet);
				int fontIndex = cache.FontInfoCache.GetItemIndex(fontInfo);
				styleSheet.FontInfoTable[FontId] = fontIndex;
				format.AssignFontIndex(fontIndex);
			}
			if (FillType == ModelFillType.Gradient) {
				GradientStopInfoCollection stops = CreateGradientStopInfoCollection(styleSheet.DocumentModel);
				int fillIndex = styleSheet.DocumentModel.Cache.GradientFillInfoCache.GetItemIndex(GradientFillInfo);
				styleSheet.FillInfoTable[FillId] = fillIndex;
				styleSheet.GradientStopInfoTable[FillId] = stops;
				AssignGradientFillInfo(format, fillIndex, stops);
			}
			else {
				FillInfo fillInfo = CreateFillInfo(styleSheet);
				int fillIndex = cache.FillInfoCache.GetItemIndex(fillInfo);
				styleSheet.FillInfoTable[FillId] = fillIndex;
				format.AssignFillIndex(fillIndex);
			}
			BorderInfo borderInfo = CreateBorderInfo(styleSheet);
			int borderIndex = cache.BorderInfoCache.GetItemIndex(borderInfo);
			styleSheet.BorderInfoTable[BorderId] = borderIndex;
			format.AssignBorderIndex(borderIndex);
			CellAlignmentInfo alignmentInfo = CreateAlignmentInfo(styleSheet);
			format.AssignAlignmentIndex(cache.CellAlignmentInfoCache.GetItemIndex(alignmentInfo));
			NumberFormat numberFormatInfo = NumberFormatParser.Parse(NumberFormatCode);
			if(!string.IsNullOrEmpty(NumberFormatCode))
				format.AssignNumberFormatIndex(cache.NumberFormatCache.GetItemIndex(numberFormatInfo));
			return true;
		}
		#region Internals
		FillInfo CreateFillInfo(XlsImportStyleSheet styleSheet) {
			FillInfo fillInfo = new Model.FillInfo();
			if (FillType == ModelFillType.Pattern) {
				fillInfo.PatternType = FillPatternType;
				fillInfo.ForeColorIndex = styleSheet.RegisterColor(ForegroundColor);
				fillInfo.BackColorIndex = styleSheet.RegisterColor(BackgroundColor);
			}
			return fillInfo;
		}
		BorderInfo CreateBorderInfo(XlsImportStyleSheet styleSheet) {
			BorderInfo borderInfo = new Model.BorderInfo();
			borderInfo.LeftColorIndex = styleSheet.RegisterColor(LeftBorderColor);
			borderInfo.RightColorIndex = styleSheet.RegisterColor(RightBorderColor);
			borderInfo.TopColorIndex = styleSheet.RegisterColor(TopBorderColor);
			borderInfo.BottomColorIndex = styleSheet.RegisterColor(BottomBorderColor);
			borderInfo.DiagonalColorIndex = styleSheet.RegisterColor(DiagonalBorderColor);
			borderInfo.VerticalColorIndex = styleSheet.RegisterColor(VerticalBorderColor);
			borderInfo.HorizontalColorIndex = styleSheet.RegisterColor(HorizontalBorderColor);
			borderInfo.LeftLineStyle = CheckBorderLineStyle(LeftBorderLineStyle, styleSheet);
			borderInfo.RightLineStyle = CheckBorderLineStyle(RightBorderLineStyle, styleSheet);
			borderInfo.TopLineStyle = CheckBorderLineStyle(TopBorderLineStyle, styleSheet);
			borderInfo.BottomLineStyle = CheckBorderLineStyle(BottomBorderLineStyle, styleSheet);
			borderInfo.DiagonalLineStyle = CheckBorderLineStyle(DiagonalBorderLineStyle, styleSheet);
			borderInfo.VerticalLineStyle = CheckBorderLineStyle(VerticalBorderLineStyle, styleSheet);
			borderInfo.HorizontalLineStyle = CheckBorderLineStyle(HorizontalBorderLineStyle, styleSheet);
			borderInfo.DiagonalDown = DiagonalDownBorder;
			borderInfo.DiagonalUp = DiagonalUpBorder;
			return borderInfo;
		}
		XlBorderLineStyle CheckBorderLineStyle(XlBorderLineStyle lineStyle, XlsImportStyleSheet styleSheet) {
			if ((int)lineStyle > (int)XlBorderLineStyle.SlantDashDot) {
				lineStyle = XlBorderLineStyle.None;
				string message = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_InvalidBorderStyleRemoved);
				styleSheet.DocumentModel.LogMessage(LogCategory.Info, message);
			}
			return lineStyle;
		}
		CellAlignmentInfo CreateAlignmentInfo(XlsImportStyleSheet styleSheet) {
			CellAlignmentInfo alignmentInfo = new Model.CellAlignmentInfo();
			alignmentInfo.HorizontalAlignment = HorizontalAlignment;
			alignmentInfo.VerticalAlignment = VerticalAlignment;
			alignmentInfo.WrapText = WrapText;
			alignmentInfo.TextRotation = styleSheet.DocumentModel.UnitConverter.DegreeToModelUnits(TextRotation);
			alignmentInfo.Indent = Indent;
			alignmentInfo.ShrinkToFit = ShrinkToFit;
			alignmentInfo.ReadingOrder = ReadingOrder;
			alignmentInfo.JustifyLastLine = JustifyDistributed;
			alignmentInfo.RelativeIndent = RelativeIndent;
			return alignmentInfo;
		}
		protected internal void AssignGradientFillInfo(FormatBase format, int fillIndex, GradientStopInfoCollection stops) {
			format.AssignGradientFillInfoIndex(fillIndex);
			format.AssignCellFormatFlagsIndex(format.CellFormatFlagsIndex + CellFormatFlagsInfo.MaskFillType);
			format.GradientStopInfoCollection.CopyFrom(stops);
		}
		GradientStopInfoCollection CreateGradientStopInfoCollection(DocumentModel documentModel) {
			GradientStopInfoCollection result = new GradientStopInfoCollection(documentModel);
			int count = xfGradStops.Count;
			for (int i = 0; i < count; i++)
				result.AddCore(GetGradientStopInfoIndex(documentModel, xfGradStops[i]));
			return result;
		}
		int GetGradientStopInfoIndex(DocumentModel documentModel, XFGradStop stop) {
			DevExpress.XtraSpreadsheet.Model.GradientStopInfo info = new DevExpress.XtraSpreadsheet.Model.GradientStopInfo();
			info.Position = stop.Position;
			info.SetColorIndex(documentModel, stop.ColorInfo);
			return documentModel.Cache.GradientStopInfoCache.GetItemIndex(info);
		}
		#endregion
		protected internal void SetXFGradientStops(List<XFGradStop> stops) {
			xfGradStops.Clear();
			int count = stops.Count;
			for (int i = 0; i < count; i++)
				xfGradStops.Add(stops[i]);
		}
	}
	#endregion
	#region XlsStyleInfo
	public class XlsStyleInfo {
		#region Properties
		public int StyleXFIndex { get; set; }
		public bool IsBuiltIn { get; set; }
		public bool IsHidden { get; set; }
		public bool CustomBuiltIn { get; set; }
		public int BuiltInId { get; set; }
		public int OutlineLevel { get; set; }
		public string Name { get; set; }
		#endregion
		protected internal void Register(XlsImportStyleSheet styleSheet) {
			ImportCellStyleInfo info = new ImportCellStyleInfo();
			info.StyleFormatId = styleSheet.GetCellStyleFormatIndex(StyleXFIndex);
			info.IsHidden = IsHidden;
			info.BuiltInId = BuiltInId;
			info.OutlineLevel = OutlineLevel;
			info.CustomBuiltIn = CustomBuiltIn;
			info.Name = Name;
			styleSheet.RegisterCellStyle(info);
		}
		public override string ToString() {
			return string.Format("{0}, builtIn={1}, builtInId={2}, hidden={3}, custom={4}, xf={5}", Name, IsBuiltIn, BuiltInId, IsHidden, CustomBuiltIn, StyleXFIndex);
		}
	}
	#endregion
	#region XlsImportStyleSheet
	public class XlsImportStyleSheet : SpreadsheetMLImportStyleSheet {
		readonly Dictionary<int, int> xfCellFormatTable = new Dictionary<int, int>();
		readonly Dictionary<int, int> xfCellFormatIndexTable = new Dictionary<int, int>();
		readonly Dictionary<int, int> xfCellStyleFormatIndexTable = new Dictionary<int, int>();
		readonly Dictionary<int, int> xfCellStyleIndexTable = new Dictionary<int, int>();
		int xfCount;
		int xfCellStyleCount;
		readonly List<XlsFontInfo> fonts = new List<XlsFontInfo>();
		readonly List<XlsExtendedFormatInfo> extendedFormats = new List<XlsExtendedFormatInfo>();
		readonly List<XlsStyleInfo> styles = new List<XlsStyleInfo>();
		readonly Dictionary<int, int> tableStyleElementTypeIndexCollection = new Dictionary<int, int>();
		TableStyle activeTableStyle;
		int activeTableStyleElementFormatCount;
		bool formatsRegistered;
		readonly MemoryStream themeStream = new MemoryStream();
		public XlsImportStyleSheet(DocumentModelImporter importer)
			: base(importer) {
		}
		protected internal Dictionary<int, int> XFCellFormatTable { get { return xfCellFormatTable; } }
		protected internal Dictionary<int, int> XFCellFormatIndexTable { get { return xfCellFormatIndexTable; } }
		protected internal Dictionary<int, int> XFCellStyleFormatIndexTable { get { return xfCellStyleFormatIndexTable; } }
		protected internal Dictionary<int, int> XFCellStyleIndexTable { get { return xfCellStyleIndexTable; } }
		protected internal IList<XlsFontInfo> Fonts { get { return fonts; } }
		protected internal IList<XlsExtendedFormatInfo> ExtendedFormats { get { return extendedFormats; } }
		IList<XlsStyleInfo> Styles { get { return styles; } }
		protected internal TableStyle ActiveTableStyle { get { return activeTableStyle; } }
		protected internal MemoryStream ThemeStream { get { return themeStream; } }
		protected internal bool IsFormatsRegistered { get { return formatsRegistered; } }
		public void RegisterFormats() {
			if(!this.formatsRegistered) {
				this.formatsRegistered = true;
				foreach(XlsFontInfo font in Fonts)
					font.Register(this);
				foreach (XlsExtendedFormatInfo format in ExtendedFormats) {
					string formatCode = format.NumberFormatCode;
					if (!string.IsNullOrEmpty(formatCode)) {
						if (formatCode == "GENERAL")
							formatCode = string.Empty;
						RegisterNumberFormat(format.NumberFormatId, formatCode);
					}
					format.Register(this);
				}
			}
		}
		public void RegisterStyles() {
			int count = ExtendedFormats.Count;
			for(int i = 0; i < count; i++) {
				XlsExtendedFormatInfo info = ExtendedFormats[i];
				int formatIndex = info.IsStyleFormat ? CellStyleFormatTable[GetCellStyleFormatIndex(i)] : GetCellFormatIndex(i);
				FormatBase format = DocumentModel.Cache.CellFormatCache[formatIndex];
				if(info.Update(format, this)) {
					formatIndex = DocumentModel.Cache.CellFormatCache.GetItemIndex(format);
					if(info.IsStyleFormat)
						CellStyleFormatTable[GetCellStyleFormatIndex(i)] = formatIndex;
					else {
						int index = 0;
						XFCellFormatIndexTable.TryGetValue(i, out index);
						CellFormatTable[index] = formatIndex;
						XFCellFormatTable[i] = formatIndex;
					}
				}
			}
			CheckCustomStyleNames();
			foreach(XlsStyleInfo style in Styles)
				style.Register(this);
			ShouldAddNormalStyle = false;
			Update();
		}
		void CheckCustomStyleNames() {
			HashSet<string> uniqueStyleNames = new HashSet<string>();
			foreach (XlsStyleInfo style in Styles)
				CheckCustomStyleName(style, uniqueStyleNames);
		}
		protected internal void CheckCustomStyleName(XlsStyleInfo style, HashSet<string> uniqueStyleNames) {
			const string formatString = "{0} {1}";
			if (style.BuiltInId != Int32.MinValue)
				return;
			string originalName = style.Name;
			style.Name = style.Name.Replace('\0', ' ');
			if (BuiltInCellStyleCalculator.IsBuiltInStyle(style.Name)) {
				int index = 1;
				string name = string.Format(formatString, style.Name, index);
				while (IsExistentStyleName(name, uniqueStyleNames)) {
					index++;
					name = string.Format(formatString, style.Name, index);
				}
				style.Name = name;
				uniqueStyleNames.Add(name);
			}
			else {
				int index = 0;
				string name = style.Name;
				while (!IsUniqueStyleName(style, name, uniqueStyleNames)) {
					index++;
					name = string.Format(formatString, style.Name, index);
				}
				style.Name = name;
				uniqueStyleNames.Add(name);
			}
			if (style.Name != originalName) {
				ILogService logService = DocumentModel.GetService<ILogService>();
				if (logService != null) {
					string message = string.Format(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_StyleNameHasBeenChanged), originalName.Replace("\0", @"\0"), style.Name);
					logService.LogMessage(LogCategory.Info, message);
				}
			}
		}
		public void RegisterTheme() {
			if (DocumentModel.Properties.IsDefaultThemeVersion) {
				DocumentModel.OfficeTheme = OfficeThemeBuilder<DevExpress.Spreadsheet.DocumentFormat>.CreateTheme(OfficeThemePreset.Office);
				return;
			}
			if (ThemeStream.Length > 0) {
				XlsThemeImporter importer = new XlsThemeImporter(DocumentModel);
				importer.Import(ThemeStream);
			}
		}
		public override int RegisterCellStyleFormat(ImportCellFormatInfo info) {
			int index = base.RegisterCellStyleFormat(info);
			XFCellStyleFormatIndexTable.Add(xfCount, CellStyleFormatTable.Count - 1);
			xfCount++;
			return index;
		}
		public override int RegisterCellFormat(ImportCellFormatInfo info) {
			int index = base.RegisterCellFormat(info);
			XFCellFormatTable.Add(xfCount, index);
			XFCellFormatIndexTable.Add(xfCount, CellFormatTable.Count - 1);
			xfCount++;
			return index;
		}
		public override int RegisterCellStyle(ImportCellStyleInfo info) {
			int index = base.RegisterCellStyle(info);
			XFCellStyleIndexTable.Add(xfCellStyleCount, index);
			xfCellStyleCount++;
			return index;
		}
		public override int GetCellFormatIndex(int index) {
			if(XFCellFormatTable.ContainsKey(index))
				return XFCellFormatTable[index];
			return DocumentModel.StyleSheet.DefaultCellFormatIndex; 
		}
		public int GetCellStyleFormatIndex(int index) {
			if(XFCellStyleFormatIndexTable.ContainsKey(index))
				return XFCellStyleFormatIndexTable[index];
			return 0; 
		}
		public int GetFontInfoIndex(int index) {
			if (FontInfoTable.ContainsKey(index))
				return FontInfoTable[index];
			return 0;
		}
		protected internal XlsExtendedFormatInfo GetExtendedFormatInfo(int index) {
			return IsValidXFIndex(index) ? extendedFormats[index] : null;
		}
		protected internal bool IsValidXFIndex(int index) {
			return (index >= 0) && (index < extendedFormats.Count);
		}
		protected internal int GetPaletteColorIndex(int colorIndex, bool foreground) {
			if(colorIndex != 0) {
				if(!DocumentModel.StyleSheet.Palette.IsValidColorIndex(colorIndex))
					colorIndex = foreground ? Palette.DefaultForegroundColorIndex : Palette.DefaultBackgroundColorIndex;
			}
			return colorIndex;
		}
		protected internal int GetBorderColorIndex(int colorIndex) {
			if(colorIndex != 0) {
				if(!DocumentModel.StyleSheet.Palette.IsValidColorIndex(colorIndex))
					colorIndex = 0;
				if((colorIndex >= Palette.SystemWindowFrameColorIndex && colorIndex <= Palette.SystemHighlightColorIndex) || (colorIndex >= Palette.SystemControlScrollColorIndex))
					colorIndex = 0;
			}
			return colorIndex;
		}
		protected internal int RegisterColor(ColorModelInfo colorInfo) {
			ColorModelInfo info = new ColorModelInfo();
			info.CopyFrom(colorInfo);
			ColorModelInfoCache cache = DocumentModel.Cache.ColorModelInfoCache;
			return cache.GetItemIndex(info);
		}
		protected internal XlsFontInfo GetFontInfo(int index) {
			if((index >= 0) && (index < fonts.Count))
				return fonts[index];
			return null;
		}
		protected internal XlsStyleInfo GetLastStyle() {
			int count = styles.Count;
			if(count > 0) 
				return styles[count - 1];
			return null;
		}
		protected internal XlsStyleInfo GetStyleInfo(string name) {
			int count = styles.Count;
			for(int i = 0; i < count; i++) {
				if(styles[i].Name == name)
					return styles[i];
			}
			return null;
		}
		protected internal void SetDefaultPivotStyleName(string name) {
			if (String.IsNullOrEmpty(name))
				return;
			TableStyleCollection tableStyles = DocumentModel.StyleSheet.TableStyles;
			tableStyles.SetDefaultPivotStyleName(name);
			if (TableStyleName.CheckPredefinedPivotStyleName(name)) {
				PredefinedPivotStyleId id = (PredefinedPivotStyleId)TableStyleName.PredefinedPivotNamesTable[name];
				TableStyle style = TableStyle.CreatePivotPredefinedStyle(DocumentModel, id);
				DocumentModel.StyleSheet.TableStyles.Add(style);
				DocumentModel.StyleSheet.TableStyles.SetDefaultPivotStyleName(style.Name.Name);
			}
		}
		protected internal void SetDefaultTableStyleName(string name) {
			if (String.IsNullOrEmpty(name)) 
				return;
			TableStyleCollection tableStyles = DocumentModel.StyleSheet.TableStyles;
			tableStyles.SetDefaultTableStyleName(name);
			if (TableStyleName.CheckPredefinedTableStyleName(name)) {
				PredefinedTableStyleId id = (PredefinedTableStyleId)TableStyleName.PredefinedTableNamesTable[name];
				TableStyle style = TableStyle.CreateTablePredefinedStyle(DocumentModel, id);
				DocumentModel.StyleSheet.TableStyles.Add(style);
				DocumentModel.StyleSheet.TableStyles.SetDefaultTableStyleName(style.Name.Name);
			}
		}
		protected internal void SetActiveTableStyle(string name, bool isPivot, bool isTable, int elementCount) {
			RegisterActiveTableStyle();
			if (String.IsNullOrEmpty(name))
				return;
			activeTableStyle = TableStyle.CreateTableStyle(DocumentModel, name);
			activeTableStyle.IsHidden = !isTable;
			tableStyleElementTypeIndexCollection.Clear();
			if (elementCount > 0)
				activeTableStyleElementFormatCount = elementCount;
			else 
				UnregisterActiveTableStyle();
		}
		void RegisterActiveTableStyle() {
			if (activeTableStyle != null)
				DocumentModel.StyleSheet.TableStyles.Add(activeTableStyle);
		}
		void UnregisterActiveTableStyle() {
			RegisterActiveTableStyle();
			activeTableStyle = null;
		}
		protected internal void RegisterTableStyleElementFormat(int typeIndex, int stripeSize, int dxfId) {
			if (typeIndex >= TableStyle.ElementsCount || ActiveTableStyle == null || !DifferentialFormatTable.ContainsKey(dxfId))
				return;
			if (tableStyleElementTypeIndexCollection.ContainsKey(typeIndex))
				return;
			tableStyleElementTypeIndexCollection.Add(typeIndex, tableStyleElementTypeIndexCollection.Count + 1);
			int formatIndex = GetTableStyleElementFormatIndex(typeIndex, stripeSize, dxfId);
			ActiveTableStyle.AssignFormatIndex(typeIndex, formatIndex);
			CheckRegisterLastTableStyleElementRecords();
		}
		int GetTableStyleElementFormatIndex(int typeIndex, int stripeSize, int dxfId) {
			TableStyleElementFormat info = new TableStyleElementFormat(DocumentModel);
			int differentialFormatIndex = DifferentialFormatTable[dxfId];
			info.AssignDifferentialFormatIndex(differentialFormatIndex);
			if (stripeSize >= StripeSizeInfo.DefaultValue)
				info.AssignStripeSizeInfoIndex(stripeSize);
			return DocumentModel.Cache.TableStyleElementFormatCache.AddItem(info);
		}
		void CheckRegisterLastTableStyleElementRecords() {
			if (tableStyleElementTypeIndexCollection.ContainsValue(activeTableStyleElementFormatCount))
				UnregisterActiveTableStyle();
		}
		public void AddStyle(XlsStyleInfo info) {
			Styles.Add(info);
		}
		protected internal bool IsUniqueStyleName(XlsStyleInfo info, string name, HashSet<string> uniqueStyleNames) {
			return !uniqueStyleNames.Contains(name);
		}
		protected internal bool IsExistentStyleName(string name, HashSet<string> uniqueStyleNames) {
			return uniqueStyleNames.Contains(name);
		}
		protected internal ColorModelInfo ConvertFontAutomaticColor(ColorModelInfo colorInfo) {
			ColorModelInfo result = colorInfo.Clone();
			if(result.ColorType == ColorType.Index) {
				if(result.ColorIndex == Palette.FontAutomaticColorIndex)
					result.Auto = true;
			}
			return result;
		}
	}
	#endregion
}
