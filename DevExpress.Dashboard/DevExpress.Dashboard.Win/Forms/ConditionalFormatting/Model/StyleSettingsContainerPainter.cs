#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using System.Drawing;
using DevExpress.DashboardCommon;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Frames;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.ViewInfo;
namespace DevExpress.DashboardWin.Native {
	static class StyleSettingsContainerPainter {
		const int RangeStopSquareSize = 5;
		static Color LightBorderColor = Color.FromArgb(70, Color.Black);
		static Color DarkBorderColor = Color.FromArgb(70, Color.White);
		static Color WhiteTransparentSquareColor = Color.FromArgb(90, Color.White);
		static Color BlackTransparentSquareColor = Color.FromArgb(90, 128, 128, 128);
		public static Color GetDefaultForeColor(UserLookAndFeel lookAndFeel) {
			Skin skin = GetSkin(lookAndFeel);
			return skin.GetSystemColor(SystemColors.WindowText);
		}
		public static Color GetDefaultBackColor(UserLookAndFeel lookAndFeel) {
			Skin skin = GetSkin(lookAndFeel);
			return skin.GetSystemColor(SystemColors.Window);
		}
		public static Color GetDefaultBorderColor(FormatConditionColorScheme scheme) {
			return (scheme == FormatConditionColorScheme.Dark) ? DarkBorderColor : LightBorderColor;
		}
		public static void Draw(ControlGraphicsInfoArgs info, Rectangle bounds, string text) {
			GraphicsCache cache = info.Cache;
			AppearanceObject appearance = info.ViewInfo.PaintAppearance;
			FormatConditionColorScheme scheme = DashboardWinHelper.IsDarkScheme(info.ViewInfo.LookAndFeel) ? FormatConditionColorScheme.Dark : FormatConditionColorScheme.Light;
			cache.DrawRectangle(GetBorderPen(cache, scheme), bounds);
			using(StringFormat stringFormatCenter = CreateCenteredStringFormat()) {
				appearance.DrawString(cache, text, bounds, stringFormatCenter);
			}
		}
		public static void Draw(StyleSettingsContainer style, ControlGraphicsInfoArgs info, Rectangle bounds, bool isPreview) {
			Draw(style, info, bounds, null, isPreview);
		}
		public static void Draw(StyleSettingsContainer style, ControlGraphicsInfoArgs info, Rectangle bounds) {
			Draw(style, info, bounds, null, false);
		}
		public static void Draw(StyleSettingsContainer style, ControlGraphicsInfoArgs info, Rectangle bounds, string text, bool isPreview) {
			if(bounds.IsEmpty || style == null) return;
			if(string.IsNullOrEmpty(text)) 
				text = style.ShortTextPreview;
			GraphicsCache cache = info.Cache;
			BaseControlViewInfo viewInfo = info.ViewInfo;
			StyleMode styleMode = style.Mode;
			FormatConditionColorScheme scheme = DashboardWinHelper.IsDarkScheme(viewInfo.LookAndFeel) ? FormatConditionColorScheme.Dark : FormatConditionColorScheme.Light;
			if(styleMode == StyleMode.Icon && (style.IconType != FormatConditionIconType.None || style.Image != null)) {
				Image image = StyleCache.GetPredefinedStyleSettingsContainer(scheme, style.IconType).Image;
				if(image != null) {
					cache.Paint.DrawImage(cache.Graphics, image, GetCenteredLocation(bounds, image.Size));
				}
			}
			else if(styleMode == StyleMode.Bar || styleMode == StyleMode.BarGradientGenerated || styleMode == StyleMode.BarGradientNonemptyStop || styleMode == StyleMode.BarGradientStop) {
				if(style.PredefinedBarColor != FormatConditionAppearanceType.None && style.PredefinedBarColor != FormatConditionAppearanceType.Custom)
					style = StyleCache.GetPredefinedBarStyleSettingsContainer(scheme, style.PredefinedBarColor);
				DrawBackground(style, bounds, text, isPreview, cache, viewInfo, style.PredefinedBarColor, style.BarColor, scheme);
			}
			else {
				if(style.AppearanceType != FormatConditionAppearanceType.None && style.AppearanceType != FormatConditionAppearanceType.Custom)
					style = StyleCache.GetPredefinedStyleSettingsContainer(scheme, style.AppearanceType);
				DrawBackground(style, bounds, text, isPreview, cache, viewInfo, style.AppearanceType, style.BackColor, scheme);
				if(!string.IsNullOrEmpty(text)) {
					AppearanceObject appearance = viewInfo.PaintAppearance;
					Brush brush = cache.GetSolidBrush(style.ForeColor.HasValue ? style.ForeColor.Value : GetDefaultForeColor(viewInfo.LookAndFeel));
					Font font = appearance.GetFont();
					FontFamily fontFamily = CreateFontFamily(style.FontFamily);
					if(fontFamily != null) {
						font = new Font(fontFamily, style.IsFontSizeDefault ? font.Size : style.FontSize, style.IsFontStyleDefault ? FontServiceBase.GetFirstAvailableFontStyle(fontFamily) : style.FontStyle.Value);
					}
					else {
						if(!style.IsFontStyleDefault || !style.IsFontSizeDefault) {
							font = cache.Cache.GetFont(font, style.IsFontSizeDefault ? font.Size : style.FontSize, style.IsFontStyleDefault ? font.Style : style.FontStyle.Value);
						}
					}
					using(StringFormat stringFormatCenter = CreateCenteredStringFormat()) {
						appearance.DrawString(cache, text, bounds, font, brush, stringFormatCenter);
					}
					if(fontFamily != null) {
						fontFamily.Dispose();
						font.Dispose();
					}
				}
			}
			if(!isPreview && (styleMode == StyleMode.GradientStop || styleMode == StyleMode.GradientNonemptyStop || styleMode == StyleMode.BarGradientStop || styleMode == StyleMode.BarGradientNonemptyStop))
				DrawGradientStop(cache, viewInfo, bounds, scheme);
		}
		static void DrawBackground(StyleSettingsContainer style, Rectangle bounds, string text, bool isPreview, GraphicsCache cache, BaseControlViewInfo viewInfo, FormatConditionAppearanceType appearanceType, Color? backColor, FormatConditionColorScheme scheme) {
			if(appearanceType == FormatConditionAppearanceType.None) {
				DrawNone(cache, bounds, scheme);
			}
			else {
				if(isPreview && appearanceType == FormatConditionAppearanceType.GradientTransparent) {
					DrawTransparent(cache, viewInfo, bounds, scheme);
				}
				else {
					cache.FillRectangle(cache.GetSolidBrush(backColor.HasValue ? backColor.Value : GetDefaultBackColor(viewInfo.LookAndFeel)), bounds);
				}
			}
			cache.DrawRectangle(GetBorderPen(cache, scheme), bounds);
		}
		static Point GetCenteredLocation(Rectangle bounds, Size elementSize) {
			return new Point(bounds.X + (bounds.Width - elementSize.Width) / 2, bounds.Y + (bounds.Height - elementSize.Height) / 2);
		}
		static Skin GetSkin(UserLookAndFeel lookAndFeel) {
			return CommonSkins.GetSkin(lookAndFeel).CommonSkin;
		}
		static FontFamily CreateFontFamily(string fontFamilyName) {
			if(string.IsNullOrEmpty(fontFamilyName)) return null;
			try {
				return new FontFamily(fontFamilyName);
			} catch {
				return new FontFamily(AppearanceObject.DefaultFont.Name);
			}
		}
		static StringFormat CreateCenteredStringFormat() {
			return new StringFormat(StringFormatFlags.NoWrap) {
				Alignment = StringAlignment.Center,
				LineAlignment = StringAlignment.Center,
				Trimming = StringTrimming.Word
			};
		}
		static Pen GetBorderPen(GraphicsCache cache, FormatConditionColorScheme scheme) {
			Color color = GetDefaultBorderColor(scheme);
			return cache.GetPen(color);
		}
		static void DrawGradientStop(GraphicsCache cache, BaseControlViewInfo viewInfo, Rectangle bounds, FormatConditionColorScheme scheme) {
			Size circleSize = new Size(RangeStopSquareSize, RangeStopSquareSize);
			Rectangle rect = new Rectangle(GetCenteredLocation(bounds, circleSize), circleSize);
			cache.Graphics.FillRectangle(cache.GetSolidBrush(GetDefaultBackColor(viewInfo.LookAndFeel)), new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 1, rect.Height - 1));
			cache.Graphics.DrawRectangle(GetBorderPen(cache, scheme), rect);
		}
		static void DrawTransparent(GraphicsCache cache, BaseControlViewInfo viewInfo, Rectangle bounds, FormatConditionColorScheme scheme) {
			int squareCount = 4;
			Size squareSize = new Size(bounds.Width / squareCount, bounds.Height / squareCount);
			for(int i = 0; i < squareCount; i++) {
				for(int j = 0; j < squareCount; j++) {
					Rectangle rect = new Rectangle(new Point(bounds.X + i * squareSize.Width, bounds.Y + j * squareSize.Height), squareSize);
					cache.Graphics.FillRectangle((i + j) % 2 == 0 ? cache.GetSolidBrush(BlackTransparentSquareColor) : cache.GetSolidBrush(WhiteTransparentSquareColor), rect);
				}
			}
		}
		static void DrawNone(GraphicsCache cache, Rectangle bounds, FormatConditionColorScheme scheme) {
			Pen pen = GetBorderPen(cache, scheme);
			bounds.Inflate(-1, -1);
			cache.Graphics.DrawLine(pen, bounds.Left, bounds.Top, bounds.Right - 1, bounds.Bottom - 1);
			cache.Graphics.DrawLine(pen, bounds.Left, bounds.Bottom - 1, bounds.Right - 1, bounds.Top);
		}
	}
}
