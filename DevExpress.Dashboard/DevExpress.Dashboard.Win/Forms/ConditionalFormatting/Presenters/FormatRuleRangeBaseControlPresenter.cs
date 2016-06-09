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
using System.Drawing;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
namespace DevExpress.DashboardWin.Native {
	abstract class FormatRuleRangeBaseControlPresenter : FormatRuleControlPresenter {
		IFormatRuleControlRangeBaseView SpecificView { get { return (IFormatRuleControlRangeBaseView)base.View; } }
		FormatConditionRangeBase SpecificCondition {
			get { return (FormatConditionRangeBase)base.Rule.Condition; }
		}
		protected bool IsPercentsSupported { get { return DataItem.ActualDataFieldType != DataFieldType.DateTime; } }
		protected FormatRuleRangeBaseControlPresenter(IServiceProvider serviceProvider, DataDashboardItem dashboardItem, DataItem dataItem, DashboardItemFormatRule initialFormatRule)
			: base(serviceProvider, dashboardItem, dataItem, initialFormatRule) {
		}
		protected abstract StyleMode GetRangeStyleMode(int rangeIndex);
		protected override bool? InitializeView() {
			bool? result = base.InitializeView();
			FormatConditionRangeBase condition = SpecificCondition;
			SpecificView.IsPercent = condition.ValueType == DashboardFormatConditionValueType.Percent;
			UpdateViewRanges();
			return result;
		}
		protected override void ApplyView() {
			base.ApplyView();
			FormatConditionRangeBase condition = SpecificCondition;
			List<RangeInfo> ranges = new List<RangeInfo>();
			IList<IFormatRuleRangeView> viewRanges = SpecificView.Ranges;
			for(int i = 0; i < viewRanges.Count; i++) {
				IFormatRuleRangeView range = viewRanges[i];
				ranges.Add(new RangeInfo() {
					StyleSettings = range.Style.ToCoreStyle(),
					ValueComparison = range.ComparisonTypeItem.ComparisonType,
					Value = CorrectRangeInfoValue(range.RightValue)
				});
			}
			ranges.Reverse();
			condition.RangeSet.Clear();
			condition.RangeSet.AddRange(ranges);
			SetConditionValueType();
		}
		protected override void SubscribeViewEvents() {
			base.SubscribeViewEvents();
			if(SpecificView != null) {
				SpecificView.RangeViewChanged += OnRangeViewChanging;
				SpecificView.UsePercentChanged += OnUsePercentChanging;
			}
		}
		protected override void UnsubscribeViewEvents() {
			base.UnsubscribeViewEvents();
			if(SpecificView != null) {
				SpecificView.RangeViewChanged -= OnRangeViewChanging;
				SpecificView.UsePercentChanged -= OnUsePercentChanging;
			}
		}
		protected abstract void OnRangeViewChanging(object sender, FormatRuleRangeViewChangingEventArgs e);
		void OnUsePercentChanging(object sender, EventArgs e) {
			SetConditionValueType();
			UpdateIntersectionLevelModes();
		}
		void SetConditionValueType() {
			SpecificCondition.ValueType = SpecificView.IsPercent ? DashboardFormatConditionValueType.Percent : DashboardFormatConditionValueType.Number;
		}
		protected void UpdateViewRanges() {
			RangeSet rangeSet = SpecificCondition.SortedRanges;
			List<IFormatRuleRangeView> ranges = new List<IFormatRuleRangeView>();
			for(int i = 0; i < rangeSet.Count; i++) {
				RangeInfo range = rangeSet[i];
				object nextValue = null;
				if(i < rangeSet.Count - 1)
					nextValue = rangeSet[i + 1].Value;
				StyleMode styleMode = GetRangeStyleMode(i);
				IStyleSettings styleSettings = GetStyleSettings(range, styleMode);
				ranges.Add(new DashboardFormatRuleRangeView(
					StyleSettingsContainer.ToStyleContainer(styleSettings, styleMode),
					ComparisonTypeItem.Wrap(range.ValueComparison),
					nextValue,
					range.Value
				));
			}
			ranges.Reverse();
			SpecificView.Ranges = ranges;
		}
		protected virtual IStyleSettings GetStyleSettings(RangeInfo range, StyleMode styleMode) {
			IStyleSettings styleSettings = styleMode == StyleMode.GradientGenerated ? PrepareAutoGeneratedAppearance(range) : range.ActualStyleSettings;
			return styleSettings;
		}
		protected virtual IStyleSettings PrepareAutoGeneratedAppearance(RangeInfo range) {
			int index = ((RangeIndexSettings)range.ActualStyleSettings).Index;
			ConditionModel gradientModel = SpecificCondition.CreateModel();
			Color backColor = FormatConditionalStyleSettingsProvider.GetBackColor(index, gradientModel, IsDarkSkin, StyleSettingsContainerPainter.GetDefaultBackColor(LookAndFeel));
			return new AppearanceSettings(backColor);
		}
		protected object CorrectRangeInfoValue(object value) {
			if(MinInfinityItem.Is(value))
				return SpecificView.IsPercent ? 0 : RangeInfo.NegativeInfinity;
			else
				return SpecificView.IsPercent ? ConvertToPercent(value) : value;
		}
		protected void SetNumericValuesToCondition(int count) {
			object[] values = new object[count];
			for(int i = 0; i < count; i++) {
				if(DataItem.DataFieldType == DataFieldType.DateTime) {
					int dateIndex = count - i - 1;
					Dimension dimension = DataItem as Dimension;
					if(dimension != null) {
						switch(dimension.DateTimeGroupInterval) {
							case DateTimeGroupInterval.DateHour:
								values[i] = DateTime.Now.AddHours(-dateIndex);
								break;
							case DateTimeGroupInterval.DateHourMinute:
								values[i] = DateTime.Now.AddMinutes(-dateIndex);
								break;
							case DateTimeGroupInterval.DateHourMinuteSecond:
								values[i] = DateTime.Now.AddSeconds(-dateIndex);
								break;
							case DateTimeGroupInterval.DayMonthYear:
							case DateTimeGroupInterval.None:
								values[i] = DateTime.Today.AddDays(-dateIndex);
								break;
							case DateTimeGroupInterval.MonthYear:
								values[i] = DateTime.Today.AddMonths(-dateIndex);
								break;
							case DateTimeGroupInterval.QuarterYear:
								values[i] = DateTime.Today.AddMonths(-dateIndex * 3);
								break;
							case DateTimeGroupInterval.Year:
								values[i] = DateTime.Today.AddYears(-dateIndex).Year;
								break;
							case DateTimeGroupInterval.WeekOfMonth:
							case DateTimeGroupInterval.WeekOfYear:
							case DateTimeGroupInterval.Second:
							case DateTimeGroupInterval.Quarter:
							case DateTimeGroupInterval.Hour:
							case DateTimeGroupInterval.Minute:
							case DateTimeGroupInterval.Month:
							case DateTimeGroupInterval.DayOfWeek:
							case DateTimeGroupInterval.DayOfYear:
							case DateTimeGroupInterval.Day:
							default:
								values[i] = i;
								break;
						}
					}
					else
						values[i] = DateTime.Today.AddDays(-dateIndex);
				}
				else
					values[i] = i;
			}
			SpecificCondition.SetValues(values);
		}
	}
}
