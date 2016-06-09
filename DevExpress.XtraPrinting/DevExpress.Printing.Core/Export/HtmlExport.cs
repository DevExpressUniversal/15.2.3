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
using System.Text;
using DevExpress.Utils;
using DevExpress.XtraPrinting.HtmlExport.Controls;
using System.Globalization;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using DevExpress.XtraPrinting.HtmlExport.Native;
#else
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Drawing;
#endif
namespace DevExpress.XtraPrinting.Export.Web {
	public class ImageEventArgs {
		Image image;
		public Image Image { get { return image; } }
		public ImageEventArgs(Image image) {
			this.image = image;
		}
	}
	public delegate void ImageEventHandler(object sender, ImageEventArgs e);
	public interface IImageRepository : IDisposable {
		event ImageEventHandler RequestImageSource;
		string GetImageSource(Image img, bool autoDisposeImage);
	}
	public class HtmlConvert {
		static readonly Dictionary<GraphicsUnit, string> unitHT = CreateUnitTable();
		static readonly Dictionary<StringAlignment, string> htmlAlignHT = CreateHorizontalAlignmentTable();
		static readonly Dictionary<StringAlignment, string> htmlVAlignHT = CreateVerticalAlignmentTable();
		static Dictionary<GraphicsUnit, string> CreateUnitTable() {
			Dictionary<GraphicsUnit, string> result = new Dictionary<GraphicsUnit, string>();
			result.Add(GraphicsUnit.Inch, "in");
			result.Add(GraphicsUnit.Millimeter, "mm");
			result.Add(GraphicsUnit.Pixel, "px");
			result.Add(GraphicsUnit.Point, "pt");
			return result;
		}
		static Dictionary<StringAlignment, string> CreateHorizontalAlignmentTable() {
			Dictionary<StringAlignment, string> result = new Dictionary<StringAlignment, string>();
			result.Add(StringAlignment.Near, "left");
			result.Add(StringAlignment.Center, "center");
			result.Add(StringAlignment.Far, "right");
			return result;
		}
		static Dictionary<StringAlignment, string> CreateVerticalAlignmentTable() {
			Dictionary<StringAlignment, string> result = new Dictionary<StringAlignment, string>();
			result.Add(StringAlignment.Near, "top");
			result.Add(StringAlignment.Center, "middle");
			result.Add(StringAlignment.Far, "bottom");
			return result;
		}
		public static string ToHtml(int value) {
			return value + "px";
		}
		public static string ToHtml(Color color) {
			Color c = DXColor.Blend(color, DXColor.White);
			if (color == DXColor.Transparent)
				return "transparent";
			if (DXColor.IsTransparentOrEmpty(c))
				c = DXColor.White;
			return DXColor.ToHtml(c);
		}
		public static string ToHtmlAlign(StringAlignment val) {
			string result;
			if (htmlAlignHT.TryGetValue(val, out result))
				return result;
			else
				return String.Empty;
		}
		public static string ToHtmlVAlign(StringAlignment val) {
			string result;
			if (htmlVAlignHT.TryGetValue(val, out result))
				return result;
			else
				return String.Empty;
		}
		static float DocumentsToPointsF(float val) {
			return val * 6.0f / 25.0f;
		}
		public static string FontSizeToString(Font font) {
			return FontSizeToString(font.Size, font.Unit);
		}
		internal static string FontSizeToString(float size, GraphicsUnit unit) {
			if (unit == GraphicsUnit.Document) {
				double fontSizeInPt = Math.Round(DocumentsToPointsF(size), 1);
				return fontSizeInPt.ToString(CultureInfo.InvariantCulture) + "pt";
			}
			string unitType;
			if (!unitHT.TryGetValue(unit, out unitType))
				unitType = "pt";
			var fontSize = unitType == "px" ? (int)size : Math.Round(size, 1);
			return fontSize.ToString(CultureInfo.InvariantCulture) + unitType;
		}
		internal static string FontSizeToStringInPixels(float size, GraphicsUnit unit) {
			int fontSizeInPx = (int)Math.Round(GraphicsUnitConverter.Convert(size, GraphicsDpi.UnitToDpi(unit), GraphicsDpi.DeviceIndependentPixel));
			return fontSizeInPx.ToString() + "px";
		}
	}
	public static class HtmlStyleRender {
		public static string GetHtmlStyle(Font font, Color foreColor, Color backColor) {
			return String.Format("color:{0};background-color:{1};{2}",
				HtmlConvert.ToHtml(foreColor), HtmlConvert.ToHtml(backColor), GetFontHtml(font));
		}
		public static string GetHtmlStyle(string fontFamilyName, float size, GraphicsUnit unit, bool bold, bool italic, bool strikeout, bool underline, Color foreColor, Color backColor) {
			return GetHtmlStyle(fontFamilyName, size, unit, bold, italic, strikeout, underline, foreColor, backColor, false);
		}
		public static string GetHtmlStyle(string fontFamilyName, float size, GraphicsUnit unit, bool bold, bool italic, bool strikeout, bool underline, Color foreColor, Color backColor, bool useFontSizeInPixels) {
			string fontHtml = GetFontHtml(fontFamilyName, size, unit, bold, italic, strikeout, underline, useFontSizeInPixels);
			return String.Format("color:{0};background-color:{1};{2}",
				HtmlConvert.ToHtml(foreColor), HtmlConvert.ToHtml(backColor), fontHtml);
		}
		public static void GetHtmlStyle(Font font, Color foreColor, Color backColor, DXCssStyleCollection style) {
			style.Add("color", HtmlConvert.ToHtml(foreColor));
			style.Add("background-color", HtmlConvert.ToHtml(backColor));
			GetFontHtml(font, style);
		}
		public static void GetHtmlStyle(string fontFamilyName, float size, GraphicsUnit unit, bool bold, bool italic, bool strikeout, bool underline, Color foreColor, Color backColor, DXCssStyleCollection style) {
			GetHtmlStyle(fontFamilyName, size, unit, bold, italic, strikeout, underline, foreColor, backColor, false, style);
		}
		public static void GetHtmlStyle(string fontFamilyName, float size, GraphicsUnit unit, bool bold, bool italic, bool strikeout, bool underline, Color foreColor, Color backColor, bool useFontSizeInPixels, DXCssStyleCollection style) {
			style.Add("color", HtmlConvert.ToHtml(foreColor));
			style.Add("background-color", HtmlConvert.ToHtml(backColor));
			GetFontHtml(fontFamilyName, size, unit, bold, italic, strikeout, underline, useFontSizeInPixels, style);
		}
		public static string GetHtmlStyle(Font font, Color foreColor) {
			return String.Format("color:{0};{1}",
				HtmlConvert.ToHtml(foreColor), GetFontHtml(font));
		}
		public static string GetFontHtml(Font font) {
			if (font == null)
				return String.Empty;
			return GetFontHtml(FontHelper.GetFamilyName(font), font.Size, font.Unit, font.Bold, font.Italic, font.Strikeout, font.Underline);
		}
		public static object GetFontHtmlInPixels(Font font) {
			if(font == null)
				return String.Empty;
			return GetFontHtml(FontHelper.GetFamilyName(font), font.Size, font.Unit, font.Bold, font.Italic, font.Strikeout, font.Underline, true);
		}
		public static string GetFontHtml(string fontFamilyName, float size, GraphicsUnit unit, bool bold, bool italic, bool strikeout, bool underline, bool inPixels) {
			string fontString = inPixels ? HtmlConvert.FontSizeToStringInPixels(size, unit) : HtmlConvert.FontSizeToString(size, unit);
			return String.Format("font-family:{0}; font-size:{1}; font-weight:{2}; font-style:{3}; {4}",
				GetCorrectedFamilyName(fontFamilyName), fontString, GetFontWeight(bold), GetFontStyle(italic), GetTextDecoration(strikeout, underline));
		}
		public static string GetFontHtml(string fontFamilyName, float size, GraphicsUnit unit, bool bold, bool italic, bool strikeout, bool underline) {
			return GetFontHtml(fontFamilyName, size, unit, bold, italic, strikeout, underline, false);
		}
		public static void GetFontHtml(Font font, DXCssStyleCollection style) {
			if (font == null)
				return;
			GetFontHtml(FontHelper.GetFamilyName(font), font.Size, font.Unit, font.Bold, font.Italic, font.Strikeout, font.Underline, false, style);
		}
		public static void GetFontHtml(string fontFamilyName, float size, GraphicsUnit unit, bool bold, bool italic, bool strikeout, bool underline, bool inPixels, DXCssStyleCollection style) {
			string fontString = inPixels ? HtmlConvert.FontSizeToStringInPixels(size, unit) : HtmlConvert.FontSizeToString(size, unit);
			style.Add("font-family", GetCorrectedFamilyName(fontFamilyName));
			style.Add("font-size", fontString);
			style.Add("font-weight", GetFontWeight(bold));
			style.Add("font-style", GetFontStyle(italic));
			string value = GetTextDecorationValue(strikeout, underline);
			if (!String.IsNullOrEmpty(value))
				style.Add("text-decoration", value);
		}
		static string GetCorrectedFamilyName(string familyName) {
			return System.Text.RegularExpressions.Regex.IsMatch(familyName, @"\s\d") ?
				"'" + familyName + "'" :
				familyName;
		}
		static string GetFontWeight(bool isBold) {
			return isBold ? "bold" : "normal";
		}
		static string GetFontStyle(bool isItalic) {
			return isItalic ? "italic" : "normal";
		}
		static string GetTextDecoration(bool strikeout, bool underline) {
			StringBuilder sb = new StringBuilder();
			string value = GetTextDecorationValue(strikeout, underline);
			if (!String.IsNullOrEmpty(value)) {
				sb.Append("text-decoration:");
				sb.Append(value);
				sb.Append(";");
			}
			return sb.ToString();
		}
		static string GetTextDecorationValue(bool strikeout, bool underline) {
			StringBuilder sb = new StringBuilder();
			if (strikeout)
				sb.Append(" line-through");
			if (underline)
				sb.Append(" underline");
			return sb.ToString();
		}
	}
}
