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
namespace DevExpress.DashboardCommon {
	static class FormatConditionRangeGenerator {
		const int DefaultRangeCount = 10;
		const int DefaultBarRangeCount = 5;
		public static void Generate(FormatConditionRangeSet condition, FormatConditionRangeSetPredefinedType type) {
			if(!type.IsPredefined())
				return;
			condition.ValueType = DashboardFormatConditionValueType.Percent;
			condition.RangeSet.Clear();
			IList<FormatConditionIconType> iconTypes = type.ToIconTypes();
			if(iconTypes != null)
				FillRangeSet(condition.RangeSet, iconTypes.Count, (i) => { return new IconSettings(iconTypes[i]); });
			else {
				IList<FormatConditionAppearanceType> types = type.ToColorTypes();
				FillRangeSet(condition.RangeSet, types.Count, (i) => { return new AppearanceSettings(types[i]); });
			}				
		}
		public static void Generate(FormatConditionColorRangeBar condition, FormatConditionRangeSetPredefinedType type) {
			if(!type.IsPredefined())
				return;
			condition.ValueType = DashboardFormatConditionValueType.Percent;
			condition.RangeSet.Clear();
			IList<FormatConditionAppearanceType> colorTypes = type.ToColorTypes();
			if(colorTypes != null) 
				FillRangeSet(condition.RangeSet, colorTypes.Count, (i) => { return new BarStyleSettings(colorTypes[i]); });
		}
		public static void GenerateGradientRangeSet(FormatConditionRangeGradient condition, FormatConditionRangeGradientPredefinedType type, IList<StyleSettingsBase> styleSettings) {
			GenerateCore(condition, type, styleSettings, DefaultRangeCount);
		}
		public static void GenerateBarGradientRangeSet(FormatConditionGradientRangeBar condition, FormatConditionRangeGradientPredefinedType type, IList<StyleSettingsBase> styleSettings) {
			GenerateCore(condition, type, styleSettings, DefaultBarRangeCount);
		}
		static void GenerateCore(FormatConditionRangeGradient condition, FormatConditionRangeGradientPredefinedType type, IList<StyleSettingsBase> styleSettings, int segmentCount) {
			if(!type.IsPredefined())
				return;
			int colorsCount = styleSettings.Count;
			condition.Generate(styleSettings[0], styleSettings[colorsCount - 1], segmentCount);
			if(colorsCount == 3)
				condition.RangeSet[segmentCount / 2].StyleSettings = styleSettings[1];
			if(colorsCount == 4) {
				condition.RangeSet[segmentCount / 3].StyleSettings = styleSettings[1];
				condition.RangeSet[2 * segmentCount / 3].StyleSettings = styleSettings[2];
			}
		}
		public static FormatConditionRangeSetPredefinedType GetPredefinedType(IEnumerable<StyleSettingsBase> styles) {
			return GetPredefinedTypeCore(styles, (type) => { return new FormatConditionRangeSet(type); });
		}
		public static FormatConditionRangeSetPredefinedType GetPredefinedBarType(IEnumerable<StyleSettingsBase> styles) {
			return GetPredefinedTypeCore(styles, (type) => { return new FormatConditionColorRangeBar(type); });
		}
		public static FormatConditionRangeSetPredefinedType GetPredefinedType(FormatConditionRangeSet condition) {
			return GetPredefinedTypeCore(condition.RangeSet.ActualStyles, (type) => { return new FormatConditionRangeSet(type); });
		}
		public static FormatConditionRangeSetPredefinedType GetPredefinedType(FormatConditionColorRangeBar condition) {
			return GetPredefinedTypeCore(condition.RangeSet.ActualStyles, (type) => { return new FormatConditionColorRangeBar(type); });
		}
		public static FormatConditionRangeGradientPredefinedType GetPredefinedType(FormatConditionRangeGradient condition) {
			return GetPredefinedTypeCore(condition.RangeSet.ActualStyles, (type) => { return new FormatConditionRangeGradient(type); });
		}
		public static FormatConditionRangeGradientPredefinedType GetPredefinedType(FormatConditionGradientRangeBar condition) {
			return GetPredefinedTypeCore(condition.RangeSet.ActualStyles, (type) => { return new FormatConditionGradientRangeBar(type); });
		}
		static FormatConditionRangeGradientPredefinedType GetPredefinedTypeCore(IEnumerable<StyleSettingsBase> actualStyles, Func<FormatConditionRangeGradientPredefinedType, FormatConditionRangeBase> createFormatConditionRange) {
			foreach(FormatConditionRangeGradientPredefinedType rangePredefinedType in Enum.GetValues(typeof(FormatConditionRangeGradientPredefinedType))) {
				if(rangePredefinedType != FormatConditionRangeGradientPredefinedType.None
					&& rangePredefinedType != FormatConditionRangeGradientPredefinedType.Custom
					&& IsStylesEqual(actualStyles, createFormatConditionRange(rangePredefinedType).RangeSet.ActualStyles))
					return rangePredefinedType;
			}
			return FormatConditionRangeGradientPredefinedType.Custom;
		}
		static FormatConditionRangeSetPredefinedType GetPredefinedTypeCore(IEnumerable<StyleSettingsBase> actualStyles, Func<FormatConditionRangeSetPredefinedType, FormatConditionRangeBase> func) {
			foreach(FormatConditionRangeSetPredefinedType rangePredefinedType in Enum.GetValues(typeof(FormatConditionRangeSetPredefinedType))) {
				if(rangePredefinedType != FormatConditionRangeSetPredefinedType.None
					&& rangePredefinedType != FormatConditionRangeSetPredefinedType.Custom
					&& IsStylesEqual(actualStyles, func(rangePredefinedType).RangeSet.ActualStyles))
					return rangePredefinedType;
			}
			return FormatConditionRangeSetPredefinedType.Custom;
		}
		static void FillRangeSet(RangeSet rangeSet, int segmentCount, Func<int, StyleSettingsBase> createStyleSettingsByPredefinedTypeIndex) {
			for(int i = 0; i < segmentCount; i++) {
				rangeSet.Add(new RangeInfo() {
					Value = ValueManager.CalculateRangePercent(i, segmentCount),
					StyleSettings = createStyleSettingsByPredefinedTypeIndex(i)
				});
			}
		}
		static bool IsStylesEqual(IEnumerable<StyleSettingsBase> actual, IEnumerable<StyleSettingsBase> predefined) {
			List<StyleSettingsBase> listActual = new List<StyleSettingsBase>(actual);
			List<StyleSettingsBase> listPredefined = new List<StyleSettingsBase>(predefined);
			if(listActual.Count != listPredefined.Count)
				return false;
			for(int i = 0; i < listActual.Count; i++) {
				if(!object.Equals(listActual[i], listPredefined[i]))
					return false;
			}
			return true;
		}
	}
}
