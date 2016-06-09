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
using System.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Compatibility.System.Drawing;
using System.Collections.Generic;
using System;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.Viewer;
namespace DevExpress.DashboardCommon {
	public abstract class FormatConditionalStyleSettingsProvider {
		public static Color GetBackColor(int rangeIndex, ConditionModel condition, bool isDarkSkin, Color defaultBackColor) {
			return Color.FromArgb(GetStyleSettingsModel(rangeIndex, condition, isDarkSkin, defaultBackColor, (type, scheme) => { return type.ToBarStyleSettingsModel(scheme); }).Color.Value);
		}
		static StyleSettingsModel GetStyleSettingsModel(int rangeIndex, ConditionModel condition, bool isDarkSkin, Color defaultBackColor, Func<FormatConditionAppearanceType, FormatConditionColorScheme, StyleSettingsModel> toSettingsModel) {
			int leftIndex = condition.FixedColors.Keys.Where(key => key < rangeIndex).Max();
			int rightIndex = condition.FixedColors.Keys.Where(key => key > rangeIndex).Min();
			StyleSettingsModel leftModel = PrepareStyle(condition.FixedColors[leftIndex], isDarkSkin, defaultBackColor, toSettingsModel);
			StyleSettingsModel rightModel = PrepareStyle(condition.FixedColors[rightIndex], isDarkSkin, defaultBackColor, toSettingsModel);
			Color leftColor = Color.FromArgb(leftModel.Color.Value);
			Color rightColor = Color.FromArgb(rightModel.Color.Value);
			Color resultColor = ValueManager.CalculateColor(leftColor, rightColor, rangeIndex - leftIndex, rightIndex - leftIndex + 1);
			StyleSettingsModel result = CopyModel(leftModel);
			result.Color = resultColor.ToArgb();
			return result;
		}
		static StyleSettingsModel PrepareStyle(StyleSettingsModel style, bool isDarkSkin, Color defaultBackColor, Func<FormatConditionAppearanceType, FormatConditionColorScheme, StyleSettingsModel> toSettingsModel) {
			FormatConditionColorScheme scheme = isDarkSkin ? FormatConditionColorScheme.Dark : FormatConditionColorScheme.Light;
			bool gradientTransparent = style.AppearanceType == FormatConditionAppearanceType.GradientTransparent;
			if(style.AppearanceType != FormatConditionAppearanceType.Custom) {
				style = toSettingsModel(style.AppearanceType, scheme);
			}
			if(gradientTransparent)
				style.Color = defaultBackColor.ToArgb();
			return style;
		}
		static StyleSettingsModel CopyModel(StyleSettingsModel style) {
			StyleSettingsModel model = new StyleSettingsModel() {
				AppearanceType = style.AppearanceType,
				Color = style.Color,
				ForeColor = style.ForeColor,
				FontFamily = style.FontFamily,
				FontSize = style.FontSize,
				FontStyle = style.FontStyle,
				IconType = style.IconType,
				Image = style.Image,
				RangeIndex = style.RangeIndex,
				RuleIndex = style.RuleIndex
			};
			return model;
		}
		readonly ConditionalFormattingModel cfModel;
		readonly MultiDimensionalData mdData;
		protected FormatConditionalStyleSettingsProvider(ConditionalFormattingModel cfModel, MultiDimensionalData data) {
			this.cfModel = cfModel;
			this.mdData = data;
		}
		protected MultiDimensionalData MDData { get { return mdData; } }
		protected ConditionalFormattingModel CFModel { get { return cfModel; } }
		public void FillStyleSettings(AxisPoint columnPoint, AxisPoint rowPoint, CustomDrawCellEventArgsBase args, string dataId) {
			IEnumerable<FormatRuleModelBase> rules = GetRuleModels(args, dataId);
			Dictionary<int, int> styleIndexes = new Dictionary<int, int>();
			decimal? normalizedValue = null;
			decimal? zeroPosition = null;
			foreach(FormatRuleModelBase rule in rules) {
				AxisPoint rowAxisPoint = CorrectRowAxisPoint(rowPoint, rule);
				List<int> indexes = GetStyleIndexes(rule, rowAxisPoint, columnPoint);
				if(indexes.Count != 0) {
					int ruleIndex = cfModel.RuleModels.IndexOf(rule);
					foreach(int index in indexes)
						if(!styleIndexes.Keys.Contains(index)) {
							styleIndexes.Add(index, ruleIndex);
						}
				}
				decimal? value1 = GetNormalizedValue(rule, rowAxisPoint, columnPoint);
				if(value1.HasValue) {
					normalizedValue = value1;
					decimal? value2 = GetZeroPosition(rule);
					if(value2.HasValue)
						zeroPosition = value2;
				}
			}
			List<int> sortedIndexes = styleIndexes.Keys.ToList();
			sortedIndexes.Sort();
			foreach(int index in sortedIndexes) {
				StyleSettingsModel styleModel = cfModel.FormatConditionStyleSettings[index];
				ConditionModel conditionModel = cfModel.RuleModels[styleIndexes[index]].ConditionModel;
				FillStyleSettings(args.StyleSettings, styleModel, conditionModel, args.IsDarkSkin, args.DefaultBackColor, args.IgnoreColorAndBackColor);
			}
			BarStyleSettingsInfo barInfo = args.StyleSettings.Bar;
			if(barInfo != null && normalizedValue.HasValue && zeroPosition.HasValue) {
				barInfo.NormalizedValue = normalizedValue.Value;
				barInfo.ZeroPosition = zeroPosition.Value;
			}
		}
		protected abstract List<int> GetStyleIndexes(FormatRuleModelBase rule, AxisPoint rowPoint, AxisPoint columnPoint);
		protected abstract IEnumerable<FormatRuleModelBase> GetRuleModels(CustomDrawCellEventArgsBase e, string dataId);
		protected virtual AxisPoint CorrectRowAxisPoint(AxisPoint axisPoint, FormatRuleModelBase rule) {
			return axisPoint;
		}
		decimal? GetNormalizedValue(FormatRuleModelBase rule, AxisPoint rowPoint, AxisPoint columnPoint) {
			MeasureDescriptor descriptor = mdData.GetNormalizedValueMeasureDescriptorByID(rule.NormalizedValueMeasureId);
			return GetDecimalValue(rowPoint, columnPoint, descriptor);
		}
		decimal? GetZeroPosition(FormatRuleModelBase rule) {
			MeasureDescriptor descriptor = mdData.GetZeroPositionMeasureDescriptorByID(rule.ZeroPositionMeasureId);
			return GetDecimalValue(null, null, descriptor);
		}
		decimal? GetDecimalValue(AxisPoint rowPoint, AxisPoint columnPoint, MeasureDescriptor descriptor) {
			if(descriptor == null)
				return null;
			MeasureValue value = mdData.GetValue(columnPoint, rowPoint, descriptor);
			return value.Value != null ? (decimal?)value.Value : null;
		}
		void FillStyleSettings(StyleSettingsInfo info, StyleSettingsModel style, ConditionModel condition, bool isDarkSkin, Color defaultBackColor, bool ignoreColorAndBackColor) {
			BarConditionModel barCondition = condition as BarConditionModel;
			if(barCondition != null) {
				ApplyBarModel(info, style, barCondition, isDarkSkin, defaultBackColor);
				return;
			}
			if(style.RangeIndex.HasValue)
				style = GetStyleSettingsModel(style.RangeIndex.Value, condition, isDarkSkin, defaultBackColor, (type, scheme) => { return type.ToAppearanceSettingsModel(scheme); });
			if(style.AppearanceType != FormatConditionAppearanceType.None) {
				style = PrepareStyle(style, isDarkSkin, defaultBackColor, (type, scheme) => { return type.ToAppearanceSettingsModel(scheme); });
				if(style.ForeColor.HasValue && !ignoreColorAndBackColor)
					info.ForeColor = Color.FromArgb(style.ForeColor.Value);
				if(style.Color.HasValue && !ignoreColorAndBackColor)
					info.BackColor = Color.FromArgb(style.Color.Value);
				Font argsFont = info.Font;
				float fontSize = style.FontSize > 0 ? style.FontSize : argsFont.Size;
				FontStyle fontStyle = style.FontStyle.HasValue ? (FontStyle)style.FontStyle.Value : argsFont.Style;
				string fontFamily = style.FontFamily ?? argsFont.FontFamily.Name;
				info.Font = new Font(fontFamily, fontSize, fontStyle);
			}
			FormatConditionImageProviderBase provider = new FormatConditionImageProviderBase();
			Image image = provider.GetImage(style.IconType, style.Image, isDarkSkin ? FormatConditionColorScheme.Dark : FormatConditionColorScheme.Light);
			if(image != null)
				info.Image = image;
		}
		void ApplyBarModel(StyleSettingsInfo info, StyleSettingsModel style, BarConditionModel condition, bool isDarkSkin, Color defaultBackColor) {
			if(style.RangeIndex.HasValue)
				style = GetStyleSettingsModel(style.RangeIndex.Value, condition, isDarkSkin, defaultBackColor, (type, scheme) => { return type.ToBarStyleSettingsModel(scheme); });
			BarStyleSettingsInfo barInfo = new BarStyleSettingsInfo();
			info.Bar = barInfo;
			if(style.AppearanceType == FormatConditionAppearanceType.Custom) {
				if(style.Color.HasValue)
					barInfo.Color = Color.FromArgb(style.Color.Value);
			}
			else if(style.AppearanceType != FormatConditionAppearanceType.None) {
				barInfo.Color = style.AppearanceType.ToBackColor(isDarkSkin ? FormatConditionColorScheme.Dark : FormatConditionColorScheme.Light);
			}
			BarOptionsModel barModel = condition.BarOptions;
			barInfo.ShowBarOnly = barModel.ShowBarOnly;
			barInfo.AllowNegativeAxis = barModel.AllowNegativeAxis;
			barInfo.DrawAxis = barModel.DrawAxis;
		}
	}
	public class PivotFormatConditionalStyleSettingsProvider : FormatConditionalStyleSettingsProvider {
		HashSet<CollapseStateCacheKey> collapseStateCache = new HashSet<CollapseStateCacheKey>();
		Dictionary<ConditionalFormattingAxisPointCacheKey, List<int>> conditionalFormattingAxisPointCache = new Dictionary<ConditionalFormattingAxisPointCacheKey, List<int>>();
		public PivotFormatConditionalStyleSettingsProvider(ConditionalFormattingModel cfModel, MultiDimensionalData data)
			: base(cfModel, data) {
		}
		public void UpdateCollapseStateCache(bool isColumn, object[] values, bool collapse) {
			CollapseStateCacheKey key = new CollapseStateCacheKey(isColumn, values);
			conditionalFormattingAxisPointCache.Clear();
			if(collapse)
				collapseStateCache.Add(key);
			else
				collapseStateCache.Remove(key);
		}
		protected override List<int> GetStyleIndexes(FormatRuleModelBase rule, AxisPoint rowPoint, AxisPoint columnPoint) {
			PivotFormatRuleModel pivotRule = rule as PivotFormatRuleModel;
			MeasureDescriptor descriptor = MDData.GetFormatConditionMeasureDescriptorByID(rule.FormatConditionMeasureId);
			List<int> styleIndexes = new List<int>();
			if(rule.ApplyToRow) {
				List<int> indexes = FindStyleIndexesOnAxis(rowPoint, columnPoint, descriptor, true);
				if(indexes != null)
					styleIndexes.AddRange(indexes);
			}
			if(pivotRule.ApplyToColumn) {
				List<int> indexes = FindStyleIndexesOnAxis(rowPoint, columnPoint, descriptor, false);
				if(indexes != null)
					styleIndexes.AddRange(indexes);
			}
			if(!rule.ApplyToRow && !pivotRule.ApplyToColumn) {
				MeasureValue value = MDData.GetValue(columnPoint, rowPoint, descriptor);
				List<int> indexes = value.Value as List<int>;
				if(indexes != null)
					styleIndexes.AddRange(indexes);
			}
			return styleIndexes;
		}
		protected override IEnumerable<FormatRuleModelBase> GetRuleModels(CustomDrawCellEventArgsBase args, string dataId) {
			Func<PivotFormatRuleModel, bool> predicate;
			if(((PivotCustomDrawCellEventArgsBase)args).IsDataArea)
				predicate = rule => rule.ApplyToRow || rule.ApplyToDataId == dataId;
			else
				predicate = rule => rule.ApplyToDataId == dataId;
			return CFModel.RuleModels.Cast<PivotFormatRuleModel>().Where(predicate);
		}
		List<int> FindStyleIndexesOnAxis(AxisPoint rowAxisPoint, AxisPoint columnAxisPoint, MeasureDescriptor descriptor, bool isRowAxis) {
			AxisPoint columnPoint = columnAxisPoint != null ? columnAxisPoint : MDData.GetAxisRoot(DashboardDataAxisNames.PivotColumnAxis);
			AxisPoint rowPoint = rowAxisPoint != null ? rowAxisPoint : MDData.GetAxisRoot(DashboardDataAxisNames.PivotRowAxis);
			AxisPoint slicePoint = isRowAxis ? rowPoint : columnPoint;
			ConditionalFormattingAxisPointCacheKey currentKey = new ConditionalFormattingAxisPointCacheKey(slicePoint, descriptor.ID);
			if(conditionalFormattingAxisPointCache.Keys.Contains(currentKey))
				return conditionalFormattingAxisPointCache[currentKey];
			MultiDimensionalData slice = MDData.GetSlice(slicePoint);
			AxisPoint intersectingRootPoint = slice.GetAxis(isRowAxis ? columnPoint.AxisName : rowPoint.AxisName).RootPoint;
			List<int> styleIndexes = null;
			IList<AxisPoint> intersectingPoints = intersectingRootPoint.GetAllAxisPoints(point => {
				object[] values = point.RootPath.Select(p => p.UniqueValue).ToArray();
				CollapseStateCacheKey key = new CollapseStateCacheKey(isRowAxis, values);
				return !collapseStateCache.Contains(key);
			});
			foreach(AxisPoint intersectingPoint in intersectingPoints) {
				MeasureValue value = slice.GetValue(intersectingPoint, descriptor);
				styleIndexes = value.Value as List<int>;
				if(styleIndexes != null)
					break;
			}
			conditionalFormattingAxisPointCache.Add(currentKey, styleIndexes);
			return styleIndexes;
		}
	}
	public class GridFormatConditionalStyleSettingsProvider : FormatConditionalStyleSettingsProvider {
		public GridFormatConditionalStyleSettingsProvider(ConditionalFormattingModel cfModel, MultiDimensionalData data)
			: base(cfModel, data) {
		}
		protected override List<int> GetStyleIndexes(FormatRuleModelBase rule, AxisPoint rowPoint, AxisPoint columnPoint) {
			MeasureDescriptor descriptor = MDData.GetFormatConditionMeasureDescriptorByID(rule.FormatConditionMeasureId);
			MeasureValue value = MDData.GetValue(rowPoint, descriptor);
			List<int> styleIndexes = value.Value as List<int>;
			return styleIndexes != null ? styleIndexes : new List<int>();
		}
		protected override IEnumerable<FormatRuleModelBase> GetRuleModels(CustomDrawCellEventArgsBase args, string dataId) {
			return CFModel.RuleModels.Cast<GridFormatRuleModel>().Where(rule => rule.ApplyToRow || rule.ApplyToDataId == dataId);
		}
		protected override AxisPoint CorrectRowAxisPoint(AxisPoint axisPoint, FormatRuleModelBase rule) {
			GridFormatRuleModel gridRule = rule as GridFormatRuleModel;
			GridDimensionMDDataPropertyDescriptorBase dimensionDescriptor = new GridDimensionValueMDDataPropertyDescriptor(gridRule.CalcByDataId, typeof(object));
			AxisPoint correctedAxisPoint = dimensionDescriptor.GetAxisPoint(axisPoint);
			return correctedAxisPoint != null ? correctedAxisPoint : axisPoint;
		}
	}
	class FormatConditionImageProviderBase {
		public Image GetImage(FormatConditionIconType iconType, FormatConditionImageModel imageViewModel, FormatConditionColorScheme scheme) {
			switch(iconType) {
				case FormatConditionIconType.None:
					return null;
				default:
					return iconType.ToImage(scheme);
			}
		}
		protected virtual Image GetImage(string url) {
			return null;
		}
	}
}
