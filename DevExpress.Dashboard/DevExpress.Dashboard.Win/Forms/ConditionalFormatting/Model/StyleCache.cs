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
using System.Collections.Generic;
using DevExpress.DashboardCommon;
using AppearancesDictionary = System.Collections.Generic.IDictionary<DevExpress.DashboardCommon.FormatConditionAppearanceType, DevExpress.DashboardWin.Native.StyleSettingsContainer>;
using IconsDictionary = System.Collections.Generic.IDictionary<DevExpress.DashboardCommon.FormatConditionIconType, DevExpress.DashboardWin.Native.StyleSettingsContainer>;
namespace DevExpress.DashboardWin.Native {
	static class StyleCache {
		public static ICollection<StyleSettingsContainer> GetPredefinedStyleTypes(FormatConditionColorScheme scheme, StyleMode styleMode) {
			switch(styleMode) {
				case StyleMode.Icon:
					return predefinedIconStyleTypes[scheme].Values;
				case StyleMode.Appearance:
					return predefinedAppearanceStyleTypes[scheme].Values;
				case StyleMode.Bar:
					return predefinedBarStyleTypes[scheme].Values;
				case StyleMode.GradientStop:
				case StyleMode.GradientNonemptyStop:
				case StyleMode.GradientGenerated:
					return predefinedGradientColorStyleTypes[scheme].Values;
				case StyleMode.BarGradientStop:
				case StyleMode.BarGradientNonemptyStop:
				case StyleMode.BarGradientGenerated:
					return predefinedGradientBarColorStyleTypes[scheme].Values;
				default:
					throw new ArgumentException("Undefined Style Mode");
			}
		}
		public static StyleSettingsContainer GetPredefinedStyleSettingsContainer(FormatConditionColorScheme scheme, FormatConditionAppearanceType appearanceType) {
			StyleSettingsContainer styleSettingsContainer;
			if(predefinedAppearanceStyleTypes[scheme].TryGetValue(appearanceType, out styleSettingsContainer))
				return styleSettingsContainer;
			else
				return predefinedGradientColorStyleTypes[scheme][appearanceType];
		}
		public static StyleSettingsContainer GetPredefinedBarStyleSettingsContainer(FormatConditionColorScheme scheme, FormatConditionAppearanceType appearanceType) {
			StyleSettingsContainer styleSettingsContainer;
			if(predefinedBarStyleTypes[scheme].TryGetValue(appearanceType, out styleSettingsContainer))
				return styleSettingsContainer;
			else
				return predefinedGradientBarColorStyleTypes[scheme][appearanceType];
		}
		public static StyleSettingsContainer GetPredefinedStyleSettingsContainer(FormatConditionColorScheme scheme, FormatConditionIconType iconType) {
			return predefinedIconStyleTypes[scheme][iconType];
		}
		readonly static Dictionary<FormatConditionColorScheme, AppearancesDictionary> predefinedAppearanceStyleTypes;
		readonly static Dictionary<FormatConditionColorScheme, AppearancesDictionary> predefinedBarStyleTypes;
		readonly static Dictionary<FormatConditionColorScheme, AppearancesDictionary> predefinedGradientColorStyleTypes;
		readonly static Dictionary<FormatConditionColorScheme, AppearancesDictionary> predefinedGradientBarColorStyleTypes;
		readonly static Dictionary<FormatConditionColorScheme, IconsDictionary> predefinedIconStyleTypes;
		static StyleCache() {
			predefinedAppearanceStyleTypes = new Dictionary<FormatConditionColorScheme, AppearancesDictionary>(2);
			predefinedBarStyleTypes = new Dictionary<FormatConditionColorScheme, AppearancesDictionary>(2);
			predefinedGradientColorStyleTypes = new Dictionary<FormatConditionColorScheme, AppearancesDictionary>(2);
			predefinedGradientBarColorStyleTypes = new Dictionary<FormatConditionColorScheme, AppearancesDictionary>(2);
			predefinedIconStyleTypes = new Dictionary<FormatConditionColorScheme, IconsDictionary>(2);
			foreach(FormatConditionColorScheme scheme in Enum.GetValues(typeof(FormatConditionColorScheme))) {
				predefinedAppearanceStyleTypes.Add(scheme, CreateAppearanceStyleTypes(scheme));
				predefinedBarStyleTypes.Add(scheme, CreateBarStyleTypes(scheme));
				predefinedGradientColorStyleTypes.Add(scheme, CreateGradientStyleTypes(scheme));
				predefinedGradientBarColorStyleTypes.Add(scheme, CreateGradientBarStyleTypes(scheme));
				predefinedIconStyleTypes.Add(scheme, CreateIconStyleTypes(scheme));
			}
		}
		static IconsDictionary CreateIconStyleTypes(FormatConditionColorScheme scheme) {
			IconsDictionary iconStyleTypes = CreateIconsDictionary();
			EnumManager.Iterate<FormatConditionIconGroups>((iconsGroup) => {
				foreach(FormatConditionIconType iconType in iconsGroup.ToIconTypes())
					iconStyleTypes.Add(iconType, StyleSettingsContainer.ToStyleContainer(scheme, iconType));
			});
			return iconStyleTypes;
		}
		static AppearancesDictionary CreateAppearanceStyleTypes(FormatConditionColorScheme scheme) {
			AppearancesDictionary appearanceStyleTypes = CreateAppearancesDictionary();
			AddAppearanceStyleTypes(appearanceStyleTypes, scheme, FormatConditionAppearanceTypeGroups.BackColors);
			AddAppearanceStyleTypes(appearanceStyleTypes, scheme, FormatConditionAppearanceTypeGroups.BackColorsWithFont);
			AddAppearanceStyleTypes(appearanceStyleTypes, scheme, FormatConditionAppearanceTypeGroups.Fonts);
			return appearanceStyleTypes;
		}
		static AppearancesDictionary CreateBarStyleTypes(FormatConditionColorScheme scheme) {
			AppearancesDictionary barStyleTypes = CreateAppearancesDictionary();
			AddBarStyleTypes(barStyleTypes, scheme, FormatConditionAppearanceTypeGroups.BackColors);
			AddBarStyleTypes(barStyleTypes, scheme, FormatConditionAppearanceTypeGroups.BackColorsWithFont);
			return barStyleTypes;
		}
		static AppearancesDictionary CreateGradientStyleTypes(FormatConditionColorScheme scheme) {
			AppearancesDictionary gradientStyleTypes = CreateAppearancesDictionary();
			AddAppearanceStyleTypes(gradientStyleTypes, scheme, FormatConditionAppearanceTypeGroups.GradientColors);
			return gradientStyleTypes;
		}
		static AppearancesDictionary CreateGradientBarStyleTypes(FormatConditionColorScheme scheme) {
			AppearancesDictionary gradientStyleTypes = CreateAppearancesDictionary();
			AddBarStyleTypes(gradientStyleTypes, scheme, FormatConditionAppearanceTypeGroups.GradientColors);
			return gradientStyleTypes;
		}
		static void AddAppearanceStyleTypes(AppearancesDictionary appearanceStyles, FormatConditionColorScheme scheme, FormatConditionAppearanceTypeGroups appearancesGroup) {
			IList<FormatConditionAppearanceType> appearanceTypes = appearancesGroup.ToAppearanceTypes();
			for(int i = 0; i < appearanceTypes.Count; i++) {
				appearanceStyles.Add(appearanceTypes[i], StyleSettingsContainer.AppearanceStyleTypeToStyleContainer(scheme, appearanceTypes[i]));
			}
		}
		static void AddBarStyleTypes(AppearancesDictionary barStyles, FormatConditionColorScheme scheme, FormatConditionAppearanceTypeGroups group) {
			IList<FormatConditionAppearanceType> barStyleTypes = group.ToBarStyleTypes();
			for(int i = 0; i < barStyleTypes.Count; i++) {
				barStyles.Add(barStyleTypes[i], StyleSettingsContainer.BarStyleTypeToStyleContainer(scheme, barStyleTypes[i]));
			}
		}
		static AppearancesDictionary CreateAppearancesDictionary() {
			return new Dictionary<FormatConditionAppearanceType, StyleSettingsContainer>();
		}
		static IconsDictionary CreateIconsDictionary() {
			return new Dictionary<FormatConditionIconType, StyleSettingsContainer>();
		}
	}
}
