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
using System.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
namespace DevExpress.DashboardCommon {
	public class FormatConditionRangeGradient : FormatConditionRangeBase {
		public StyleSettingsBase StartStyle {
			get { return SortedRanges.Count != 0 ? SortedRanges.First().StyleSettings : null; }
		}
		public StyleSettingsBase EndStyle {
			get { return SortedRanges.Count != 0 ? SortedRanges.Last().StyleSettings : null; }
		}
		public int SegmentCount {
			get { return RangeSet.Count; }
		}
		public virtual FormatConditionRangeGradientPredefinedType ActualPredefinedType {
			get { return FormatConditionRangeGenerator.GetPredefinedType(this); }
		}
		public override bool IsValid {
			get {
				foreach(RangeInfo range in RangeSet)
					if(range.StyleSettings != null && !IsGradientStop(range.StyleSettings))
						return false;
				return IsGradientStop(StartStyle) && IsGradientStop(EndStyle);
			}
		}
		protected virtual string GradientStopException { get { return "Use colors with the 'Gradient' prefix from the FormatConditionAppearanceType enumeration to initialize the AppearanceSettings.AppearanceType property or set the AppearanceSettings.AppearanceType property to Custom and specify the AppearanceSettings.BackColor property."; } }
		public FormatConditionRangeGradient() {
		}
		public FormatConditionRangeGradient(StyleSettingsBase startStyle, StyleSettingsBase endStyle, int segmentCount)
			: this() {
			Generate(startStyle, endStyle, segmentCount);
		}
		public FormatConditionRangeGradient(StyleSettingsBase startStyle, StyleSettingsBase endStyle, object[] values)
			: this() {
			Generate(startStyle, endStyle, values);
		}
		public FormatConditionRangeGradient(FormatConditionRangeGradientPredefinedType type)
			: this() {
			Generate(type);
		}
		public virtual void Generate(FormatConditionRangeGradientPredefinedType type) {
			FormatConditionRangeGenerator.GenerateGradientRangeSet(this, type, type.ToAppearanceSettings());
		}
		public virtual void Generate(StyleSettingsBase startStyle, StyleSettingsBase endStyle, int segmentCount) {
			ValueType = DashboardFormatConditionValueType.Percent;
			GenerateInternal(startStyle, endStyle, segmentCount);
			decimal[] values = ValueManager.CalculateRangePercentValues(segmentCount);
			SetValues(values);
		}
		public virtual void Generate(StyleSettingsBase startStyle, StyleSettingsBase endStyle, params object[] values) {
			ValueType = DashboardFormatConditionValueType.Number;
			GenerateInternal(startStyle, endStyle, values.Length);
			SetValues(values);
		}
		internal StyleSettingsBase GetActualStyleSettings(RangeInfo rangeInfo) {
			int index = SortedRanges.IndexOf(rangeInfo);
			return GetStyleSettings(index);
		}
		override internal ConditionModel CreateModel() {
			return new ConditionModel() {
				FixedColors = GetFixedColors()
			};
		}
		protected Dictionary<int, StyleSettingsModel> GetFixedColors() {
			Dictionary<int, StyleSettingsModel> fixedColorStyles = new Dictionary<int, StyleSettingsModel>();
			RangeSet ranges = SortedRanges;
			for(int i = 0; i < ranges.Count; i++) {
				StyleSettingsBase colorStyle = ranges[i].StyleSettings;
				if(IsGradientStop(colorStyle))
					fixedColorStyles.Add(i, ((IStyleSettings)colorStyle).CreateViewModel());
			}
			return fixedColorStyles;
		}
		void GenerateInternal(StyleSettingsBase startStyle, StyleSettingsBase endStyle, int rangeCount) {
			RangeSet.Clear();
			for(int i = 0; i < rangeCount; i++) {
				RangeInfo info = new RangeInfo();
				RangeSet.Add(info);
			}
			RangeSet[0].StyleSettings = ValidateStyle(startStyle);
			RangeSet[RangeSet.Count - 1].StyleSettings = ValidateStyle(endStyle);
		}
		protected virtual bool IsGradientStop(StyleSettingsBase style) {
			IBackColorStyleSettings colorStyle = style as IBackColorStyleSettings;
			return colorStyle != null && 
				(colorStyle.BackColor.HasValue && colorStyle.AppearanceType == FormatConditionAppearanceType.Custom ||
				colorStyle.AppearanceType != FormatConditionAppearanceType.None && FormatConditionAppearanceTypeGroups.GradientColors.ToAppearanceTypes().Contains(colorStyle.AppearanceType));
		}
		protected virtual StyleSettingsBase ValidateStyle(StyleSettingsBase style) {
			if(!IsGradientStop(style))
				throw new ArgumentException(GradientStopException);
			return style;
		}
		StyleSettingsBase GetStyleSettings(int index) {
			if(!IsValid)
				return null;
			RangeSet rangeSet = SortedRanges;
			if(rangeSet[index].StyleSettings != null)
				return rangeSet[index].StyleSettings;
			return GetRangeIndexSettings(index);
		}
		protected virtual StyleSettingsBase GetRangeIndexSettings(int index) {
			return new RangeIndexSettings(this, index);
		}
		protected override FormatConditionBase CreateInstance() {
			return new FormatConditionRangeGradient();
		}
	}
}
