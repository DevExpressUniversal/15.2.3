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

using System.Collections.Generic;
using DevExpress.XtraGauges.Presets.Localization;
namespace DevExpress.XtraGauges.Presets.Styles {
	public sealed class Theme {
		string nameCore;
		Theme(string name) {
			nameCore = name;
		}
		public string Name {
			get { return nameCore; }
		}
		public override string ToString() {
			return nameCore;
		}
		public override bool Equals(object obj) {
			if(!(obj is Theme)) return false;
			Theme theme = obj as Theme;
			return theme.nameCore == nameCore;
		}
		public override int GetHashCode() {
			return nameCore.GetHashCode();
		}
		public static Theme Default { get { return themes["Default"]; } }
		public static Theme White { get { return themes["Style1"]; } }
		public static Theme DarkNight { get { return themes["Style2"]; } }
		public static Theme DeepFire { get { return themes["Style3"]; } }
		public static Theme IceColdZone { get { return themes["Style4"]; } }
		public static Theme GothicMat { get { return themes["Style5"]; } }
		public static Theme ShiningDark { get { return themes["Style6"]; } }
		public static Theme AfricaSunset { get { return themes["Style7"]; } }
		public static Theme Mechanical { get { return themes["Style8"]; } }
		public static Theme SilverBlur { get { return themes["Style9"]; } }
		public static Theme PureDark { get { return themes["Style10"]; } }
		public static Theme CleanWhite { get { return themes["Style11"]; } }
		public static Theme SportCar { get { return themes["Style12"]; } }
		public static Theme Military { get { return themes["Style13"]; } }
		public static Theme Retro { get { return themes["Style14"]; } }
		public static Theme Disco { get { return themes["Style15"]; } }
		public static Theme Clever { get { return themes["Style16"]; } }
		public static Theme Cosmic { get { return themes["Style17"]; } }
		public static Theme Smart { get { return themes["Style18"]; } }
		public static Theme Progressive { get { return themes["Style19"]; } }
		public static Theme Eco { get { return themes["Style20"]; } }
		public static Theme MagicLight { get { return themes["Style21"]; } }
		public static Theme iStyle { get { return themes["Style22"]; } }
		public static Theme Future { get { return themes["Style23"]; } }
		public static Theme YellowSubmarine { get { return themes["Style24"]; } }
		public static Theme Classic { get { return themes["Style25"]; } }
		public static Theme Red { get { return themes["Style26"]; } }
		public static Theme FlatLight { get { return themes["Style27"]; } }
		public static Theme FlatDark { get { return themes["Style28"]; } }
		public static Theme Ignis { get { return themes["Style29"]; } }
		public static Theme Haze { get { return themes["Style30"]; } }
		public static Theme FromString(string source) {
			Theme result;
			return themes.TryGetValue(source, out result) ? result : null;
		}
		public static string CheckName(string name) {
			Theme result;
			if(themes.TryGetValue(name, out result))
				return MakeShort(result.Name);
			return null;
		}
		static string MakeShort(string fullName) {
			string[] parts = fullName.Split(' ', '-');
			return string.Join(string.Empty, parts);
		}
		#region static
		static IDictionary<string, Theme> themes;
		static Theme() {
			themes = new Dictionary<string, Theme>();
			themes.Add("Style1", new Theme(GetLocalizedThemeName(GaugesPresetsStringId.ThemeWhite)));
			themes.Add("Style2", new Theme(GetLocalizedThemeName(GaugesPresetsStringId.ThemeDarkNight)));
			themes.Add("Style3", new Theme(GetLocalizedThemeName(GaugesPresetsStringId.ThemeDeepFire)));
			themes.Add("Style4", new Theme(GetLocalizedThemeName(GaugesPresetsStringId.ThemeIceColdZone)));
			themes.Add("Style5", new Theme(GetLocalizedThemeName(GaugesPresetsStringId.ThemeGothicMat)));
			themes.Add("Style6", new Theme(GetLocalizedThemeName(GaugesPresetsStringId.ThemeShiningDark)));
			themes.Add("Style7", new Theme(GetLocalizedThemeName(GaugesPresetsStringId.ThemeAfricaSunset)));
			themes.Add("Style8", new Theme(GetLocalizedThemeName(GaugesPresetsStringId.ThemeMechanical)));
			themes.Add("Style9", new Theme(GetLocalizedThemeName(GaugesPresetsStringId.ThemeSilverBlur)));
			themes.Add("Style10", new Theme(GetLocalizedThemeName(GaugesPresetsStringId.ThemePureDark)));
			themes.Add("Style11", new Theme(GetLocalizedThemeName(GaugesPresetsStringId.ThemeCleanWhite)));
			themes.Add("Style12", new Theme(GetLocalizedThemeName(GaugesPresetsStringId.ThemeSportCar)));
			themes.Add("Style13", new Theme(GetLocalizedThemeName(GaugesPresetsStringId.ThemeMilitary)));
			themes.Add("Style14", new Theme(GetLocalizedThemeName(GaugesPresetsStringId.ThemeRetro)));
			themes.Add("Style15", new Theme(GetLocalizedThemeName(GaugesPresetsStringId.ThemeDisco)));
			themes.Add("Style16", new Theme(GetLocalizedThemeName(GaugesPresetsStringId.ThemeClever)));
			themes.Add("Style17", new Theme(GetLocalizedThemeName(GaugesPresetsStringId.ThemeCosmic)));
			themes.Add("Style18", new Theme(GetLocalizedThemeName(GaugesPresetsStringId.ThemeSmart)));
			themes.Add("Style19", new Theme(GetLocalizedThemeName(GaugesPresetsStringId.ThemeProgressive)));
			themes.Add("Style20", new Theme(GetLocalizedThemeName(GaugesPresetsStringId.ThemeEco)));
			themes.Add("Style21", new Theme(GetLocalizedThemeName(GaugesPresetsStringId.ThemeMagicLight)));
			themes.Add("Style22", new Theme(GetLocalizedThemeName(GaugesPresetsStringId.ThemeiStyle)));
			themes.Add("Style23", new Theme(GetLocalizedThemeName(GaugesPresetsStringId.ThemeFuture)));
			themes.Add("Style24", new Theme(GetLocalizedThemeName(GaugesPresetsStringId.ThemeYellowSubmarine)));
			themes.Add("Style25", new Theme(GetLocalizedThemeName(GaugesPresetsStringId.ThemeClassic)));
			themes.Add("Style26", new Theme(GetLocalizedThemeName(GaugesPresetsStringId.ThemeRed)));
			themes.Add("Style27", new Theme(GetLocalizedThemeName(GaugesPresetsStringId.ThemeFlatLight)));
			themes.Add("Style28", new Theme(GetLocalizedThemeName(GaugesPresetsStringId.ThemeFlatDark)));
			themes.Add("Style29", new Theme(GetLocalizedThemeName(GaugesPresetsStringId.ThemeIgnis)));
			themes.Add("Style30", new Theme(GetLocalizedThemeName(GaugesPresetsStringId.ThemeHaze)));
			AddAliases("Default", themes["Style1"]);
			AddAliases("White", themes["Style1"]);
			AddAliases("DarkNight", themes["Style2"]);
			AddAliases("DeepFire", themes["Style3"]);
			AddAliases("IceColdZone", themes["Style4"]);
			AddAliases("GothicMat", themes["Style5"]);
			AddAliases("ShiningDark", themes["Style6"]);
			AddAliases("AfricaSunset", themes["Style7"]);
			AddAliases("Mechanical", themes["Style8"]);
			AddAliases("SilverBlur", themes["Style9"]);
			AddAliases("PureDark", themes["Style10"]);
			AddAliases("CleanWhite", themes["Style11"]);
			AddAliases("SportCar", themes["Style12"]);
			AddAliases("Military", themes["Style13"]);
			AddAliases("Retro", themes["Style14"]);
			AddAliases("Disco", themes["Style15"]);
			AddAliases("Clever", themes["Style16"]);
			AddAliases("Cosmic", themes["Style17"]);
			AddAliases("Smart", themes["Style18"]);
			AddAliases("Progressive", themes["Style19"]);
			AddAliases("Eco", themes["Style20"]);
			AddAliases("MagicLight", themes["Style21"]);
			AddAliases("iStyle", themes["Style22"]);
			AddAliases("Future", themes["Style23"]);
			AddAliases("YellowSubmarine", themes["Style24"]);
			AddAliases("Classic", themes["Style25"]);
			AddAliases("Red", themes["Style26"]);
			AddAliases("FlatLight", themes["Style27"]);
			AddAliases("FlatDark", themes["Style28"]);
			AddAliases("Ignis", themes["Style29"]);
			AddAliases("Haze", themes["Style30"]);
			AddAliases(GetLocalizedThemeName(GaugesPresetsStringId.ThemeDefault), themes["Style1"]);
			AddAliases(GetLocalizedThemeName(GaugesPresetsStringId.ThemeWhite), themes["Style1"]);
			AddAliases(GetLocalizedThemeName(GaugesPresetsStringId.ThemeDarkNight), themes["Style2"]);
			AddAliases(GetLocalizedThemeName(GaugesPresetsStringId.ThemeDeepFire), themes["Style3"]);
			AddAliases(GetLocalizedThemeName(GaugesPresetsStringId.ThemeIceColdZone), themes["Style4"]);
			AddAliases(GetLocalizedThemeName(GaugesPresetsStringId.ThemeGothicMat), themes["Style5"]);
			AddAliases(GetLocalizedThemeName(GaugesPresetsStringId.ThemeShiningDark), themes["Style6"]);
			AddAliases(GetLocalizedThemeName(GaugesPresetsStringId.ThemeAfricaSunset), themes["Style7"]);
			AddAliases(GetLocalizedThemeName(GaugesPresetsStringId.ThemeMechanical), themes["Style8"]);
			AddAliases(GetLocalizedThemeName(GaugesPresetsStringId.ThemeSilverBlur), themes["Style9"]);
			AddAliases(GetLocalizedThemeName(GaugesPresetsStringId.ThemePureDark), themes["Style10"]);
			AddAliases(GetLocalizedThemeName(GaugesPresetsStringId.ThemeCleanWhite), themes["Style11"]);
			AddAliases(GetLocalizedThemeName(GaugesPresetsStringId.ThemeSportCar), themes["Style12"]);
			AddAliases(GetLocalizedThemeName(GaugesPresetsStringId.ThemeMilitary), themes["Style13"]);
			AddAliases(GetLocalizedThemeName(GaugesPresetsStringId.ThemeRetro), themes["Style14"]);
			AddAliases(GetLocalizedThemeName(GaugesPresetsStringId.ThemeDisco), themes["Style15"]);
			AddAliases(GetLocalizedThemeName(GaugesPresetsStringId.ThemeClever), themes["Style16"]);
			AddAliases(GetLocalizedThemeName(GaugesPresetsStringId.ThemeCosmic), themes["Style17"]);
			AddAliases(GetLocalizedThemeName(GaugesPresetsStringId.ThemeSmart), themes["Style18"]);
			AddAliases(GetLocalizedThemeName(GaugesPresetsStringId.ThemeProgressive), themes["Style19"]);
			AddAliases(GetLocalizedThemeName(GaugesPresetsStringId.ThemeEco), themes["Style20"]);
			AddAliases(GetLocalizedThemeName(GaugesPresetsStringId.ThemeMagicLight), themes["Style21"]);
			AddAliases(GetLocalizedThemeName(GaugesPresetsStringId.ThemeiStyle), themes["Style22"]);
			AddAliases(GetLocalizedThemeName(GaugesPresetsStringId.ThemeFuture), themes["Style23"]);
			AddAliases(GetLocalizedThemeName(GaugesPresetsStringId.ThemeYellowSubmarine), themes["Style24"]);
			AddAliases(GetLocalizedThemeName(GaugesPresetsStringId.ThemeClassic), themes["Style25"]);
			AddAliases(GetLocalizedThemeName(GaugesPresetsStringId.ThemeRed), themes["Style26"]);
			AddAliases(GetLocalizedThemeName(GaugesPresetsStringId.ThemeFlatLight), themes["Style27"]);
			AddAliases(GetLocalizedThemeName(GaugesPresetsStringId.ThemeFlatDark), themes["Style28"]);
			AddAliases(GetLocalizedThemeName(GaugesPresetsStringId.ThemeIgnis), themes["Style29"]);
			AddAliases(GetLocalizedThemeName(GaugesPresetsStringId.ThemeHaze), themes["Style30"]);
		}
		static string GetLocalizedThemeName(GaugesPresetsStringId id) {
			return LocalizedThemeNamesMap.GetLocalizedThemeName(id);
		}
		static void AddAliases(string themeName, Theme theme) {
			AddAlias(themeName, theme);
			string shortAlias = MakeShort(themeName);
			if(shortAlias != themeName) {
				AddAlias(shortAlias, theme);
			}
		}
		static void AddAlias(string alias, Theme theme) {
			if(!themes.ContainsKey(alias)) {
				themes.Add(alias, theme);
			}
		}
		#endregion static
	}
	static class LocalizedThemeNamesMap {
		readonly static GaugesPresetsStringId[] localizableThemes = new GaugesPresetsStringId[] { 
			GaugesPresetsStringId.ThemeDefault,
			GaugesPresetsStringId.ThemeWhite,
			GaugesPresetsStringId.ThemeDarkNight,
			GaugesPresetsStringId.ThemeDeepFire,
			GaugesPresetsStringId.ThemeIceColdZone,
			GaugesPresetsStringId.ThemeGothicMat,
			GaugesPresetsStringId.ThemeShiningDark,
			GaugesPresetsStringId.ThemeAfricaSunset,
			GaugesPresetsStringId.ThemeMechanical,
			GaugesPresetsStringId.ThemeSilverBlur,
			GaugesPresetsStringId.ThemePureDark,
			GaugesPresetsStringId.ThemeCleanWhite,
			GaugesPresetsStringId.ThemeSportCar,
			GaugesPresetsStringId.ThemeMilitary,
			GaugesPresetsStringId.ThemeRetro,
			GaugesPresetsStringId.ThemeDisco,
			GaugesPresetsStringId.ThemeClever,
			GaugesPresetsStringId.ThemeCosmic,
			GaugesPresetsStringId.ThemeSmart,
			GaugesPresetsStringId.ThemeProgressive,
			GaugesPresetsStringId.ThemeEco,
			GaugesPresetsStringId.ThemeMagicLight,
			GaugesPresetsStringId.ThemeiStyle,
			GaugesPresetsStringId.ThemeFuture,
			GaugesPresetsStringId.ThemeYellowSubmarine,
			GaugesPresetsStringId.ThemeClassic,
			GaugesPresetsStringId.ThemeRed,
			GaugesPresetsStringId.ThemeFlatLight,
			GaugesPresetsStringId.ThemeFlatDark,
			GaugesPresetsStringId.ThemeIgnis,
			GaugesPresetsStringId.ThemeHaze,
		};
		[System.ThreadStatic]
		static IDictionary<GaugesPresetsStringId, string> namesMap;
		internal static string GetLocalizedThemeName(GaugesPresetsStringId id) {
			if(namesMap == null)
				namesMap = EsureNamesMap();
			return namesMap[id];
		}
		static IDictionary<GaugesPresetsStringId, string> EsureNamesMap() {
			var map = new Dictionary<GaugesPresetsStringId, string>();
			var identityMap = new Dictionary<string, GaugesPresetsStringId>();
			for(int i = 0; i < localizableThemes.Length; i++) {
				GaugesPresetsStringId id = localizableThemes[i];
				string name = GaugesPresetsLocalizer.GetString(id);
				GaugesPresetsStringId existingId;
				if(identityMap.TryGetValue(name, out existingId)) {
					map.Remove(existingId);
					map.Add(existingId, ResolveCollision(name, existingId));
					map.Add(id, ResolveCollision(name, id));
				}
				else {
					identityMap.Add(name, id);
					map.Add(id, name);
				}
			}
			return map;
		}
		static string ResolveCollision(string name, GaugesPresetsStringId id) {
			return name + "(" + id.ToString().Replace("Theme", "") + ")";
		}
	}
	class ThemeNameResolutionService : Core.Styles.IThemeNameResolutionService {
		string Core.Styles.IThemeNameResolutionService.Resolve(string themeName) {
			return Theme.CheckName(themeName);
		}
	}
}
