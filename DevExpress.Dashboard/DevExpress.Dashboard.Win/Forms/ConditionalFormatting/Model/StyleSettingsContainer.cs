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

using System;
using System.Drawing;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardWin.Native {
	public class StyleSettingsContainer {
		const StyleMode DefaultStyleMode = StyleMode.Appearance;
		const FormatConditionAppearanceType DefaultAppearanceType = FormatConditionAppearanceType.None;
		const FormatConditionAppearanceType DefaultPredefinedBarColor = FormatConditionAppearanceType.None;
		public const FormatConditionIconType DefaultIconType = FormatConditionIconType.None;
		public const float DefaultFontSize = -1;
		public static readonly FontStyle? DefaultFontStyle = null;
		public static readonly string DefaultFontFamily = null;
		public static readonly Color? DefaultForeColor = null;
		public static readonly Color? DefaultBackColor = null;
		public static readonly Color? DefaultBarColor = null;
		public static readonly string DefaultShortTextPreview = string.Empty;
		static readonly StyleSettingsContainer Empty = new StyleSettingsContainer(DefaultStyleMode, DefaultAppearanceType, DefaultPredefinedBarColor);
		public static StyleSettingsContainer CreateDefaultEmpty(StyleMode styleMode) {
			return CreateDefault(styleMode, false);
		}
		public static StyleSettingsContainer CreateDefaultCustom(StyleMode styleMode) {
			return CreateDefault(styleMode == StyleMode.GradientGenerated ? StyleMode.GradientStop : styleMode, true);
		}
		static StyleSettingsContainer CreateDefault(StyleMode styleMode, bool isCustom) {
			switch(styleMode) {
				case StyleMode.Icon:
					return new StyleSettingsContainer(FormatConditionIconType.None, null);
				case StyleMode.Appearance:
					return new StyleSettingsContainer(styleMode, isCustom ? FormatConditionAppearanceType.Custom : FormatConditionAppearanceType.None, FormatConditionAppearanceType.None);
				case StyleMode.Bar:
					return new StyleSettingsContainer(styleMode, FormatConditionAppearanceType.None, isCustom ? FormatConditionAppearanceType.Custom : FormatConditionAppearanceType.None);
				case StyleMode.GradientStop:
				case StyleMode.GradientNonemptyStop:
				case StyleMode.GradientGenerated:
					return new StyleSettingsContainer(styleMode, isCustom ? FormatConditionAppearanceType.Custom : FormatConditionAppearanceType.None, FormatConditionAppearanceType.None);
				case StyleMode.BarGradientStop:
				case StyleMode.BarGradientNonemptyStop:
				case StyleMode.BarGradientGenerated:
					return new StyleSettingsContainer(styleMode, FormatConditionAppearanceType.None, isCustom ? FormatConditionAppearanceType.Custom : FormatConditionAppearanceType.None);
				default:
					throw new ArgumentException("Undefined Style Mode");
			}
		}
		internal static StyleSettingsContainer ToStyleContainer(FormatConditionColorScheme scheme, FormatConditionIconType iconType) {
			StyleSettingsContainer styleSettingsContainer = new StyleSettingsContainer(iconType, iconType.ToImage(scheme));
			styleSettingsContainer.isPredefinedPalette = true;
			return styleSettingsContainer;
		}
		internal static StyleSettingsContainer AppearanceStyleTypeToStyleContainer(FormatConditionColorScheme scheme, FormatConditionAppearanceType predefinedType) {
			AppearanceSettings actualAppearanceSettings = predefinedType.ToAppearanceSettings(scheme);
			actualAppearanceSettings.AppearanceType = predefinedType;
			StyleSettingsContainer styleSettingsContainer = StyleSettingsContainer.ToStyleContainer(actualAppearanceSettings);
			styleSettingsContainer.isPredefinedPalette = true;
			return styleSettingsContainer;
		}
		internal static StyleSettingsContainer BarStyleTypeToStyleContainer(FormatConditionColorScheme scheme, FormatConditionAppearanceType predefinedType) {
			BarStyleSettings actualAppearanceSettings = predefinedType.ToBarStyleSettings(scheme);
			actualAppearanceSettings.PredefinedColor = predefinedType;
			StyleSettingsContainer styleSettingsContainer = StyleSettingsContainer.ToStyleContainer(actualAppearanceSettings);
			styleSettingsContainer.isPredefinedPalette = true;
			return styleSettingsContainer;
		}
		public static StyleSettingsContainer ToStyleContainer(IStyleSettings styleSettings, StyleMode styleMode) {
			StyleSettingsContainer style = ToStyleContainer(styleSettings);
			style.mode = styleMode;
			if(styleMode == StyleMode.GradientGenerated)
				style.appearanceType = FormatConditionAppearanceType.Custom;
			if(styleMode == StyleMode.BarGradientGenerated)
				style.predefinedBarColor = FormatConditionAppearanceType.Custom;
			return style;
		}
		public static StyleSettingsContainer ToStyleContainer(IStyleSettings styleSettings) {
			return new StyleSettingsContainer(styleSettings);
		}
		static bool CompareColors(Color? color1, Color? color2) {
			if(!color1.HasValue && !color2.HasValue) return true;
			if(color1.HasValue && !color2.HasValue) return false;
			if(!color1.HasValue && color2.HasValue) return false;
			return color1.Value.ToArgb() == color2.Value.ToArgb();
		}
		readonly AppearanceSettings initialAppearanceStyleClone = null;
		readonly BarStyleSettings initialBarStyleClone = null;
		StyleMode mode = DefaultStyleMode;
		FormatConditionAppearanceType appearanceType = DefaultAppearanceType;
		FormatConditionAppearanceType predefinedBarColor = DefaultPredefinedBarColor;
		bool isPredefinedPalette = false;
		Color? color = DefaultForeColor;
		Color? backColor = DefaultBackColor;
		Color? barColor = DefaultBarColor;
		string fontFamily = DefaultFontFamily;
		float fontSize = DefaultFontSize;
		FontStyle? fontStyle = DefaultFontStyle;
		FormatConditionIconType iconType = DefaultIconType;
		Image image = null;
		public StyleMode Mode { get { return mode; } }
		public FormatConditionAppearanceType AppearanceType { get { return appearanceType; } }
		public FormatConditionAppearanceType PredefinedBarColor { get { return predefinedBarColor; } }
		public Color? ForeColor { get { return color; } set { color = value; } }
		public Color? BackColor { get { return backColor; } set { backColor = value; } }
		public Color? BarColor { get { return barColor; } set { barColor = value; } }
		public string FontFamily { get { return fontFamily; } set { fontFamily = value; } }
		public float FontSize { get { return fontSize; } set { fontSize = value; } }
		public FontStyle? FontStyle { get { return fontStyle; } set { fontStyle = value; } }
		public FormatConditionIconType IconType { get { return iconType; } set { iconType = value; } }
		public Image Image { get { return image; } set { image = value; } }
		public string ShortTextPreview {
			get {
				switch(AppearanceType) {
					case FormatConditionAppearanceType.FontBold: return "B";
					case FormatConditionAppearanceType.FontItalic: return "I";
					case FormatConditionAppearanceType.FontUnderline: return "U";
					case FormatConditionAppearanceType.FontGrayed: return "Gr";
					case FormatConditionAppearanceType.FontRed: return "R";
					case FormatConditionAppearanceType.FontYellow: return "Y";
					case FormatConditionAppearanceType.FontGreen: return "G";
					case FormatConditionAppearanceType.FontBlue: return "B";
					case FormatConditionAppearanceType.Custom: return IsEmpty || Mode == StyleMode.GradientStop || Mode == StyleMode.GradientNonemptyStop || Mode == StyleMode.GradientGenerated ? DefaultShortTextPreview : "T";
					case FormatConditionAppearanceType.None: return string.Empty;
					default:
						return DefaultShortTextPreview;
				}
			}
		}
		public bool IsEmpty { get { return object.Equals(this, Empty); } }
		public bool IsFontSizeDefault { get { return FontSize == DefaultFontSize; } }
		public bool IsFontStyleDefault { get { return FontStyle == DefaultFontStyle; } }
		public bool IsFontFamilyDefault { get { return FontFamily == DefaultFontFamily; } }
		public bool IsColorDefault { get { return ForeColor == DefaultForeColor; } }
		public bool IsBackColorDefault { get { return BackColor == DefaultBackColor; } }
		public bool IsBarColorDefault { get { return BarColor == DefaultBarColor; } }
		public StyleSettingsContainer(StyleMode styleMode, FormatConditionAppearanceType appearanceType, FormatConditionAppearanceType predefinedBarColor) {
			this.mode = styleMode;
			this.appearanceType = appearanceType;
			this.predefinedBarColor = predefinedBarColor;
		}
		public StyleSettingsContainer(StyleMode styleMode, Color? barColor) {
			this.mode = styleMode;
			this.barColor = barColor;
			this.predefinedBarColor = FormatConditionAppearanceType.Custom;
		}
		public StyleSettingsContainer(StyleMode styleMode, Color? backColor, Color? color, string fontFamily, float fontSize, FontStyle? fontStyle)
			: this(styleMode, FormatConditionAppearanceType.Custom, FormatConditionAppearanceType.Custom) {
			this.BackColor = backColor;
			this.ForeColor = color;
			this.FontFamily = fontFamily;
			this.FontSize = fontSize;
			this.FontStyle = fontStyle;
		}
		StyleSettingsContainer(IStyleSettings styleSettings)
			: this(DefaultStyleMode, DefaultAppearanceType, DefaultPredefinedBarColor) {
			if(styleSettings == null) return;
			AppearanceSettings appearanceSettings = styleSettings as AppearanceSettings;
			BarStyleSettings barSettings = styleSettings as BarStyleSettings;
			if(appearanceSettings != null) {
				this.mode = StyleMode.Appearance;
				this.appearanceType = appearanceSettings.AppearanceType;
				if(!IsNonPredefined(this.appearanceType)) {
					this.initialAppearanceStyleClone = (AppearanceSettings)styleSettings.Clone();
				}
				this.BackColor = appearanceSettings.BackColor;
				this.ForeColor = appearanceSettings.ForeColor;
				this.FontFamily = appearanceSettings.FontFamily;
				this.FontSize = appearanceSettings.FontSize;
				this.FontStyle = appearanceSettings.FontStyle;
			}
			else if(barSettings != null) {
				this.mode = StyleMode.Bar;
				this.predefinedBarColor = barSettings.PredefinedColor;
				if(!IsNonPredefined(this.predefinedBarColor)) {
					this.initialBarStyleClone = (BarStyleSettings)styleSettings.Clone();
				}
				this.BarColor = barSettings.Color;
			}
			else {
				this.mode = StyleMode.Icon;
				this.IconType = ((IconSettings)styleSettings).IconType;
			}
		}
		StyleSettingsContainer(FormatConditionIconType iconType, Image image)
			: this(StyleMode.Icon, FormatConditionAppearanceType.None, FormatConditionAppearanceType.None) {
			this.IconType = iconType;
			this.Image = image;
		}
		public void Assign(StyleSettingsContainer style) {
			mode = style.mode;
			appearanceType = style.appearanceType;
			ForeColor = style.ForeColor;
			BackColor = style.BackColor;
			FontFamily = style.FontFamily;
			FontSize = style.FontSize;
			FontStyle = style.FontStyle;
			IconType = style.IconType;
			Image = style.Image;
		}
		public override bool Equals(object obj) {
			StyleSettingsContainer style = obj as StyleSettingsContainer;
			if(style == null) return false;
			bool isCustomAppearance = IsNonPredefined(AppearanceType);
			bool isCustomBarColor = IsNonPredefined(PredefinedBarColor);
			return 
				(((!isCustomAppearance && object.Equals(appearanceType, style.appearanceType) ||
				(isCustomAppearance &&
				CompareColors(ForeColor, style.ForeColor) &&
				CompareColors(BackColor, style.BackColor) &&
				object.Equals(FontFamily, style.FontFamily) &&
				object.Equals(FontSize, style.FontSize) &&
				object.Equals(FontStyle, style.FontStyle))) &&
				object.Equals(IconType, style.IconType)) && 
				((!isCustomBarColor && object.Equals(predefinedBarColor, style.predefinedBarColor)) ||
				(isCustomBarColor && CompareColors(BarColor, style.BarColor))));
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public StyleSettingsBase ToCoreStyle() {
			if(mode == StyleMode.GradientGenerated || mode == StyleMode.BarGradientGenerated) return null;
			if(mode == StyleMode.Icon) {
				return new IconSettings() {
					IconType = this.IconType
				};
			}
			if(mode == StyleMode.Bar || mode == StyleMode.BarGradientNonemptyStop || mode == StyleMode.BarGradientStop) {
				if(IsNonPredefined(PredefinedBarColor)) {
					BarStyleSettings settings = new BarStyleSettings(PredefinedBarColor);
					settings.Color = BarColor;
					return settings;
				}
				else if(isPredefinedPalette)
					return new BarStyleSettings(PredefinedBarColor);
				else {
					initialBarStyleClone.PredefinedColor = PredefinedBarColor;
					initialBarStyleClone.Color = BarColor;
					return initialBarStyleClone;
				}
			}
			if(IsNonPredefined(AppearanceType)) {
				return new AppearanceSettings(AppearanceType) {
					BackColor = this.BackColor,
					ForeColor = this.ForeColor,
					FontFamily = this.FontFamily,
					FontSize = this.FontSize,
					FontStyle = this.FontStyle
				};
			} else {
				if(isPredefinedPalette) {
					return new AppearanceSettings(AppearanceType);
				} else {
					initialAppearanceStyleClone.AppearanceType = AppearanceType;
					return initialAppearanceStyleClone;
				}
			}
		}
		bool IsNonPredefined(FormatConditionAppearanceType type) {
			return type == FormatConditionAppearanceType.None || type == FormatConditionAppearanceType.Custom;
		}
	}
}
