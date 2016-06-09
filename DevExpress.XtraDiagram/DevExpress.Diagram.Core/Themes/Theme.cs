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

using DevExpress.Diagram.Core.Localization;
using DevExpress.Diagram.Core.Shapes;
using DevExpress.Diagram.Core.Shapes.Native;
using DevExpress.Diagram.Core.Themes;
using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.ComponentModel;
using DevExpress.Diagram.Core.TypeConverters;
namespace DevExpress.Diagram.Core {
	[TypeConverter(typeof(DiagramThemeTypeConverter))]
	public class DiagramTheme {
		#region Static
		static DiagramFontSettings MergeFontSettings(DiagramFontSettings fontSettings, DiagramFontEffects fontEffects) {
			return new DiagramFontSettings(fontSettings.FontSize, fontSettings.FontFamily, fontEffects);
		}
		static List<DiagramItemStyle> GetDiagramStyles(DiagramColorPalette colorPalette, IEnumerable<DiagramEffect> effects, DiagramFontSettings fontSettings) {
			List<DiagramItemStyle> styles = new List<DiagramItemStyle>();
			var accents = colorPalette.Accents.Concat(new List<Color> { colorPalette.Dark });
			foreach(var effect in effects) {
				foreach(var accent in accents) {
					DiagramItemBrush brush = GetShapeBrush(effect, colorPalette, accent);
					styles.Add(new DiagramItemStyle(brush, MergeFontSettings(fontSettings, effect.FontEffects), effect.LineSettings));
				}
			}
			return styles;
		}
		static List<DiagramItemStyle> GetVariantShapeStyles(DiagramColorPalette colorPalette, DiagramEffectCollection effectCollection, DiagramFontSettings fontSettings) {
			List<DiagramItemStyle> styles = new List<DiagramItemStyle>();
			foreach(DiagramEffect effect in effectCollection.VariantEffects) {
				DiagramItemBrush brush = GetShapeBrush(effect, colorPalette, Colors.Transparent);
				styles.Add(new DiagramItemStyle(brush, MergeFontSettings(fontSettings, effect.FontEffects), effect.LineSettings));
			}
			return styles;
		}
		static DiagramItemBrush GetShapeBrush(DiagramEffect effect, DiagramColorPalette colorPalette, Color accent) {
			EffectContext effectContext = new EffectContext(colorPalette, accent, colorPalette.Light);
			return effect.GetItemBrush(effectContext);
		}
		#endregion
		#region Fields
		readonly string idCore;
		readonly Func<string> getThemeName;
		readonly DiagramEffectCollection effectCollectionCore;
		readonly DiagramColorPalette colorPaletteCore;
		readonly DiagramFontSettings fontSettingsCore;
		readonly ReadOnlyCollection<DiagramItemStyle> variantShapeStyles;
		readonly ReadOnlyCollection<DiagramItemStyle> themeShapeStyles;
		readonly ReadOnlyCollection<DiagramItemStyle> connectorShapeStyles;
		#endregion
		#region Properties
		public string Id { get { return idCore; } }
		public string Name { get { return getThemeName(); } }
		public DiagramEffectCollection EffectCollection { get { return effectCollectionCore; } }
		public DiagramColorPalette ColorPalette { get { return colorPaletteCore; } }
		public DiagramFontSettings FontSettings { get { return fontSettingsCore; } }
		public ReadOnlyCollection<DiagramItemStyle> VariantShapeStyles { get { return variantShapeStyles; } }
		public ReadOnlyCollection<DiagramItemStyle> ThemeShapeStyles { get { return themeShapeStyles; } }
		public ReadOnlyCollection<DiagramItemStyle> ConnectorStyles { get { return connectorShapeStyles; } }
		#endregion
		public DiagramTheme(string themeId, Func<string> getThemeName, DiagramColorPalette colorPalette, DiagramEffectCollection effectCollection, DiagramFontSettings fontSettings) {
			if(colorPalette == null || effectCollection == null || fontSettings == null)
				throw new ArgumentNullException();
			this.idCore = themeId;
			this.getThemeName = getThemeName;
			this.effectCollectionCore = effectCollection;
			this.colorPaletteCore = colorPalette;
			this.fontSettingsCore = fontSettings;
			this.variantShapeStyles = new ReadOnlyCollection<DiagramItemStyle>(GetVariantShapeStyles(colorPalette, effectCollection, fontSettings));
			this.themeShapeStyles = new ReadOnlyCollection<DiagramItemStyle>(GetDiagramStyles(colorPalette, effectCollection.ThemeEffects, fontSettings));
			this.connectorShapeStyles = new ReadOnlyCollection<DiagramItemStyle>(GetDiagramStyles(colorPalette, effectCollection.ConnectorEffects, fontSettings));
		}
		#region Methods
		public override string ToString() {
			return Name;
		}
		public DiagramItemStyle GetDiagramItemStyle(DiagramItemStyleId id) {
			if(id == null)
				return null;
			return id.GetStyle(this);
		}
		#endregion
	}
	public static class ThemeRegistrator {
		readonly static Dictionary<string, DiagramTheme> themeDictionary;
		public static IEnumerable<DiagramTheme> Themes { get { return themeDictionary.Values; } }
		static ThemeRegistrator() {
			themeDictionary = new Dictionary<string, DiagramTheme>();
			LoadThemes();
		}
		public static void RegisterTheme(DiagramTheme theme) {
			if(theme != null)
				themeDictionary[theme.Id] = theme;
		}
		public static void UnregisterTheme(DiagramTheme theme) {
			if(theme != null)
				themeDictionary.Remove(theme.Id);
		}
		public static DiagramTheme GetTheme(string themeId) {
			DiagramTheme value = null;
			if(themeId != null && themeDictionary.TryGetValue(themeId, out value))
				return value;
			return null;
		}
		static void LoadThemes() {
			ImageSourceHelper.RegisterPackScheme();
			var dictionary = new ResourceDictionary { Source = AssemblyHelper.GetResourceUri(typeof(Theme).Assembly, string.Format("Themes/Themes.xaml")) };
			var keys = dictionary.Keys.Cast<ThemeKey>().OrderBy(key => key.Id).ToList();
			foreach(var themeKey in keys) {
				string themeId = themeKey.ResourceKey;
				Func<string> getThemeName = () => DiagramControlLocalizer.GetString(ThemeRegistratorHelper.GetThemeStringId(themeId));
				Theme themeTemplate = (Theme)dictionary[themeKey];
				DiagramTheme theme = DiagramThemeFactory.CreateTheme(themeId, getThemeName, themeTemplate);
				RegisterTheme(theme);
			}
		}
	}
	public class DiagramFontSettings {
		public readonly double FontSize;
		public readonly FontFamily FontFamily;
		public readonly DiagramFontEffects FontEffects;
		public DiagramFontSettings(double fontSize, FontFamily family, DiagramFontEffects effects) {
			this.FontSize = fontSize;
			this.FontFamily = family;
			this.FontEffects = effects;
		}
	}
	public class DiagramFontEffects {
		public readonly bool IsFontBold;
		public readonly bool IsFontItalic;
		public readonly bool IsFontUnderline;
		public readonly bool IsFontStrikethrough;
		public DiagramFontEffects(bool isFontBold, bool isFontItalic, bool isFontUnderline, bool isFontStrikethrough) {
			this.IsFontBold = isFontBold;
			this.IsFontItalic = isFontItalic;
			this.IsFontUnderline = isFontUnderline;
			this.IsFontStrikethrough = isFontStrikethrough;
		}
	}
	[TypeConverter(typeof(DiagramItemStyleIdTypeConverter))]
	public class DiagramItemStyleId {
		internal static DiagramItemStyleId Create(string id, DiagramControlStringId effectStringId, int accentIndex, Func<DiagramTheme, DiagramItemStyle> getStyleCore) {
			Func<string> getName = () => string.Format(DiagramControlLocalizer.GetString(DiagramControlStringId.Themes_ThemeStyleId_Name),
				DiagramControlLocalizer.GetString(effectStringId),
				accentIndex);
			return new DiagramItemStyleId(id, getName, getStyleCore);
		}
		internal static DiagramItemStyleId Create(string id, int variantIndex, Func<DiagramTheme, DiagramItemStyle> getStyleCore) {
			Func<string> getName = () => string.Format(DiagramControlLocalizer.GetString(DiagramControlStringId.Themes_VariantStyleId_Name), variantIndex);
			return new DiagramItemStyleId(id, getName, getStyleCore);
		}
		public static readonly DiagramItemStyleId DefaultStyleId = new DiagramItemStyleId(null, null, null);
		readonly string idCore;
		readonly Func<string> getName;
		readonly Func<DiagramTheme, DiagramItemStyle> getStyleCore;
		public string Name { get { return GetName(); } }
		public string Id { get { return idCore; } }
		public DiagramItemStyleId(string id, Func<string> getName, Func<DiagramTheme, DiagramItemStyle> getStyleCore) {
			this.idCore = id;
			this.getStyleCore = getStyleCore;
			this.getName = getName;
		}
		public override string ToString() {
			return Name;
		}
		public DiagramItemStyle GetStyle(DiagramTheme theme) {
			return getStyleCore(theme);
		}
		string GetName() {
			if(getName != null)
				return getName();
			return string.Empty;
		}
	}
}
