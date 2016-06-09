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
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Import;
using DevExpress.XtraSpreadsheet.Import.Xls;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Export.Xl;
#if !SL
using System.Drawing;
using DevExpress.Office.Model;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Internal {
	#region XlsExtendedFormatInfoAdapter
	public class XlsExtendedFormatInfoAdapter : IXFContentReceiver {
		XlsExtendedFormatInfo info;
		XlsImportStyleSheet styleSheet;
		public XlsExtendedFormatInfoAdapter(XlsExtendedFormatInfo info, XlsImportStyleSheet styleSheet) {
			Guard.ArgumentNotNull(info, "info");
			Guard.ArgumentNotNull(styleSheet, "styleSheet");
			this.info = info;
			this.styleSheet = styleSheet;
		}
		#region Fill
		public void SetFillPatternType(XlPatternType patternType) {
			info.FillPatternType = patternType;
		}
		public void SetForegroundColor(ColorModelInfo colorInfo) {
			info.ForegroundColor.CopyFrom(colorInfo);
		}
		public void SetBackgroundColor(ColorModelInfo colorInfo) {
			info.BackgroundColor.CopyFrom(colorInfo);
		}
		public void SetXFGradient(GradientFillInfo gradientFillInfo) {
			info.GradientFillInfo = gradientFillInfo;
			info.XFGradStops.Clear();
			info.FillType = ModelFillType.Gradient;
		}
		public void SetXFGradientStop(XFGradStop stop) {
			info.XFGradStops.Add(stop);
			info.FillType = ModelFillType.Gradient;
		}
		public void SetXFGradient(GradientFillInfo gradientFillInfo, List<XFGradStop> stops) {
			info.GradientFillInfo = gradientFillInfo;
			info.SetXFGradientStops(stops);
			info.FillType = ModelFillType.Gradient;
		}
		#endregion
		#region Border
		public void SetTopBorderColor(ColorModelInfo colorInfo) {
			info.TopBorderColor.CopyFrom(colorInfo);
		}
		public void SetBottomBorderColor(ColorModelInfo colorInfo) {
			info.BottomBorderColor.CopyFrom(colorInfo);
		}
		public void SetLeftBorderColor(ColorModelInfo colorInfo) {
			info.LeftBorderColor.CopyFrom(colorInfo);
		}
		public void SetRightBorderColor(ColorModelInfo colorInfo) {
			info.RightBorderColor.CopyFrom(colorInfo);
		}
		public void SetDiagonalBorderColor(ColorModelInfo colorInfo) {
			info.DiagonalBorderColor.CopyFrom(colorInfo);
		}
		public void SetVerticalBorderColor(ColorModelInfo colorInfo) {
			info.VerticalBorderColor.CopyFrom(colorInfo);
		}
		public void SetHorizontalBorderColor(ColorModelInfo colorInfo) {
			info.HorizontalBorderColor.CopyFrom(colorInfo);
		}
		public void SetTopBorderLineStyle(XlBorderLineStyle lineStyle) {
			info.TopBorderLineStyle = lineStyle;
		}
		public void SetBottomBorderLineStyle(XlBorderLineStyle lineStyle) {
			info.BottomBorderLineStyle = lineStyle;
		}
		public void SetLeftBorderLineStyle(XlBorderLineStyle lineStyle) {
			info.LeftBorderLineStyle = lineStyle;
		}
		public void SetRightBorderLineStyle(XlBorderLineStyle lineStyle) {
			info.RightBorderLineStyle = lineStyle;
		}
		public void SetDiagonalBorderLineStyle(XlBorderLineStyle lineStyle) {
			info.DiagonalBorderLineStyle = lineStyle;
		}
		public void SetVerticalBorderLineStyle(XlBorderLineStyle lineStyle) {
			info.VerticalBorderLineStyle = lineStyle;
		}
		public void SetHorizontalBorderLineStyle(XlBorderLineStyle lineStyle) {
			info.HorizontalBorderLineStyle = lineStyle;
		}
		public void SetDiagonalUpBorder(bool flag) {
			info.DiagonalUpBorder = flag;
		}
		public void SetDiagonalDownBorder(bool flag) {
			info.DiagonalDownBorder = flag;
		}
		#endregion
		#region Protection
		public void SetLockedProtection(bool flag) {
			info.IsLocked = flag;
		}
		public void SetHiddenProtection(bool flag) {
			info.IsHidden = flag;
		}
		#endregion
		#region Alignment
		public void SetHorizontalAlignment(XlHorizontalAlignment alignment) {
			info.HorizontalAlignment = alignment;
		}
		public void SetVerticalAlignment(XlVerticalAlignment alignment) {
			info.VerticalAlignment = alignment;
		}
		public void SetTextRotation(int rotation) {
			info.TextRotation = rotation;
		}
		public void SetIndent(int indentation) {
			info.Indent = (byte)indentation;
		}
		public void SetRelativeIndent(int relativeIndentation) {
			info.RelativeIndent = relativeIndentation;
		}
		public void SetReadingOrder(XlReadingOrder readingOrder) {
			info.ReadingOrder = readingOrder;
		}
		public void SetShrinkToFit(bool flag) {
			info.ShrinkToFit = flag;
		}
		public void SetTextWrapped(bool flag) {
			info.WrapText = flag;
		}
		public void SetTextJustifyDistributed(bool flag) {
			info.JustifyDistributed = flag;
		}
		#endregion
		#region Font
		public void SetFontName(string fontName) {
			XlsFontInfo font = styleSheet.GetFontInfo(info.FontId);
			if(font != null) {
				font.Name = fontName;
				font.IsRegistered = false;
			}
		}
		public void SetTextBold(bool flag) {
			XlsFontInfo font = styleSheet.GetFontInfo(info.FontId);
			if(font != null) {
				font.Bold = flag;
				font.IsRegistered = false;
			}
		}
		public void SetFontUnderline(XlUnderlineType underlineType) {
			XlsFontInfo font = styleSheet.GetFontInfo(info.FontId);
			if(font != null) {
				font.Underline = underlineType;
				font.IsRegistered = false;
			}
		}
		public void SetFontScript(XlScriptType scriptType) {
			XlsFontInfo font = styleSheet.GetFontInfo(info.FontId);
			if(font != null) {
				font.Script = scriptType;
				font.IsRegistered = false;
			}
		}
		public void SetCharset(int charset) {
			XlsFontInfo font = styleSheet.GetFontInfo(info.FontId);
			if(font != null) {
				font.Charset = charset;
				font.IsRegistered = false;
			}
		}
		public void SetFontFamily(int fontFamily) {
			XlsFontInfo font = styleSheet.GetFontInfo(info.FontId);
			if(font != null) {
				font.FontFamily = fontFamily;
				font.IsRegistered = false;
			}
		}
		public void SetTextSizeInTwips(int sizeInTwips) {
			XlsFontInfo font = styleSheet.GetFontInfo(info.FontId);
			if(font != null) {
				font.Size = sizeInTwips / 20.0; 
				font.IsRegistered = false;
			}
		}
		public void SetFontScheme(XlFontSchemeStyles fontScheme) {
			XlsFontInfo font = styleSheet.GetFontInfo(info.FontId);
			if(font != null) {
				font.SchemeStyle = fontScheme;
				font.IsRegistered = false;
			}
		}
		public void SetTextColor(ColorModelInfo colorInfo) {
			XlsFontInfo font = styleSheet.GetFontInfo(info.FontId);
			if(font != null) {
				font.FontColor.CopyFrom(colorInfo);
				font.IsRegistered = false;
			}
		}
		public void SetTextItalic(bool flag) {
			XlsFontInfo font = styleSheet.GetFontInfo(info.FontId);
			if(font != null) {
				font.Italic = flag;
				font.IsRegistered = false;
			}
		}
		public void SetTextStrikethrough(bool flag) {
			XlsFontInfo font = styleSheet.GetFontInfo(info.FontId);
			if(font != null) {
				font.StrikeThrough = flag;
				font.IsRegistered = false;
			}
		}
		public void SetHasOutlineStyle(bool flag) {
			XlsFontInfo font = styleSheet.GetFontInfo(info.FontId);
			if(font != null) {
				font.Outline = flag;
				font.IsRegistered = false;
			}
		}
		public void SetHasShadowStyle(bool flag) {
			XlsFontInfo font = styleSheet.GetFontInfo(info.FontId);
			if(font != null) {
				font.Shadow = flag;
				font.IsRegistered = false;
			}
		}
		public void SetTextCondensed(bool flag) {
			XlsFontInfo font = styleSheet.GetFontInfo(info.FontId);
			if(font != null) {
				font.Condense = flag;
				font.IsRegistered = false;
			}
		}
		public void SetTextExtended(bool flag) {
			XlsFontInfo font = styleSheet.GetFontInfo(info.FontId);
			if(font != null) {
				font.Extend = flag;
				font.IsRegistered = false;
			}
		}
		#endregion
		#region NumberFormat
		public void SetNumberFormatCode(string numberFormatCode) {
			info.NumberFormatCode = numberFormatCode;
		}
		public void SetNumberFormatId(int numberFormatId) {
			info.NumberFormatId = numberFormatId;
		}
		#endregion
	}
	#endregion
	#region XlsDifferentionFormatInfoAdapter
	public class XlsDifferentialFormatInfoAdapter : IXFContentReceiver {
		XlsDifferentialFormatInfo info;
		public XlsDifferentialFormatInfoAdapter(XlsDifferentialFormatInfo info) {
			Guard.ArgumentNotNull(info, "info");
			this.info = info;
		}
		#region Fill
		public void SetFillPatternType(XlPatternType patternType) {
			info.FillPatternType = patternType;
		}
		public void SetForegroundColor(ColorModelInfo colorInfo) {
			info.FillForegroundColor = colorInfo.Clone();
		}
		public void SetBackgroundColor(ColorModelInfo colorInfo) {
			info.FillBackgroundColor = colorInfo.Clone();
		}
		public void SetXFGradient(GradientFillInfo gradientFillInfo) {
			info.GradientFillInfo = gradientFillInfo;
			info.FillType = ModelFillType.Gradient;
		}
		public void SetXFGradientStop(XFGradStop stop) {
			info.XFGradStops.Add(stop);
			info.FillType = ModelFillType.Gradient;
		}
		public void SetXFGradient(GradientFillInfo gradientFillInfo, List<XFGradStop> stops) {
			info.GradientFillInfo = gradientFillInfo;
			info.SetXFGradientStops(stops);
			info.FillType = ModelFillType.Gradient;
		}
		#endregion
		#region Border
		public void SetTopBorderColor(ColorModelInfo colorInfo) {
			info.BorderTopColor = colorInfo.Clone();
		}
		public void SetBottomBorderColor(ColorModelInfo colorInfo) {
			info.BorderBottomColor = colorInfo.Clone();
		}
		public void SetLeftBorderColor(ColorModelInfo colorInfo) {
			info.BorderLeftColor = colorInfo.Clone();
		}
		public void SetRightBorderColor(ColorModelInfo colorInfo) {
			info.BorderRightColor = colorInfo.Clone();
		}
		public void SetDiagonalBorderColor(ColorModelInfo colorInfo) {
			info.BorderDiagonalColor = colorInfo.Clone();
		}
		public void SetVerticalBorderColor(ColorModelInfo colorInfo) {
			info.BorderVerticalColor = colorInfo.Clone();
		}
		public void SetHorizontalBorderColor(ColorModelInfo colorInfo) {
			info.BorderHorizontalColor = colorInfo.Clone();
		}
		public void SetTopBorderLineStyle(XlBorderLineStyle lineStyle) {
			info.BorderTopLineStyle = lineStyle;
		}
		public void SetBottomBorderLineStyle(XlBorderLineStyle lineStyle) {
			info.BorderBottomLineStyle = lineStyle;
		}
		public void SetLeftBorderLineStyle(XlBorderLineStyle lineStyle) {
			info.BorderLeftLineStyle = lineStyle;
		}
		public void SetRightBorderLineStyle(XlBorderLineStyle lineStyle) {
			info.BorderRightLineStyle = lineStyle;
		}
		public void SetDiagonalBorderLineStyle(XlBorderLineStyle lineStyle) {
			info.BorderDiagonalLineStyle = lineStyle;
		}
		public void SetVerticalBorderLineStyle(XlBorderLineStyle lineStyle) {
			info.BorderVerticalLineStyle = lineStyle;
		}
		public void SetHorizontalBorderLineStyle(XlBorderLineStyle lineStyle) {
			info.BorderHorizontalLineStyle = lineStyle;
		}
		public void SetDiagonalUpBorder(bool flag) {
			info.BorderDiagonalUp = flag;
		}
		public void SetDiagonalDownBorder(bool flag) {
			info.BorderDiagonalDown = flag;
		}
		#endregion
		#region Protection
		public void SetLockedProtection(bool flag) {
			info.ProtectionLocked = flag;
		}
		public void SetHiddenProtection(bool flag) {
			info.ProtectionHidden = flag;
		}
		#endregion
		#region Alignment
		public void SetHorizontalAlignment(XlHorizontalAlignment alignment) {
			info.AlignmentHorizontal = alignment;
		}
		public void SetVerticalAlignment(XlVerticalAlignment alignment) {
			info.AlignmentVertical = alignment;
		}
		public void SetIndent(int indentation) {
			info.AlignmentIndent = (byte)indentation;
		}
		public void SetReadingOrder(XlReadingOrder readingOrder) {
			info.AlignmentReadingOrder = readingOrder;
		}
		public void SetRelativeIndent(int relativeIndentation) {
			info.AlignmentRelativeIndent = relativeIndentation;
		}
		public void SetTextRotation(int rotation) {
			info.AlignmentTextRotation = rotation;
		}
		public void SetShrinkToFit(bool flag) {
			info.AlignmentShrinkToFit = flag;
		}
		public void SetTextJustifyDistributed(bool flag) {
			info.AlignmentJustifyLastLine = flag;
		}
		public void SetTextWrapped(bool flag) {
			info.AlignmentWrapText = flag;
		}
		#endregion
		#region Font
		public void SetFontName(string fontName) {
			info.FontName = fontName;
		}
		public void SetCharset(int charset) {
			info.FontCharset = charset;
		}
		public void SetFontFamily(int fontFamily) {
			info.FontFamily = fontFamily;
		}
		public void SetFontUnderline(XlUnderlineType underlineType) {
			info.FontUnderline = underlineType;
		}
		public void SetFontScheme(XlFontSchemeStyles fontScheme) {
			info.FontSchemeStyle = fontScheme;
		}
		public void SetFontScript(XlScriptType scriptType) {
			info.FontScript = scriptType;
		}
		public void SetTextColor(ColorModelInfo colorInfo) {
			info.FontColor = colorInfo.Clone();
		}
		public void SetTextSizeInTwips(int sizeInTwips) {
			info.FontSize = sizeInTwips / 20.0; 
		}
		public void SetTextBold(bool flag) {
			info.FontBold = flag;
		}
		public void SetTextItalic(bool flag) {
			info.FontItalic = flag;
		}
		public void SetTextStrikethrough(bool flag) {
			info.FontStrikeThrough = flag;
		}
		public void SetTextCondensed(bool flag) {
			info.FontCondense = flag;
		}
		public void SetTextExtended(bool flag) {
			info.FontExtend = flag;
		}
		public void SetHasOutlineStyle(bool flag) {
			info.FontOutline = flag;
		}
		public void SetHasShadowStyle(bool flag) {
			info.FontShadow = flag;
		}
		#endregion
		#region NumberFormat
		public void SetNumberFormatCode(string numberFormatCode) {
			info.NumberFormatCode = numberFormatCode;
		}
		public void SetNumberFormatId(int numberFormatId) {
			info.NumberFormatId = numberFormatId;
		}
		#endregion
	}
	#endregion
}
