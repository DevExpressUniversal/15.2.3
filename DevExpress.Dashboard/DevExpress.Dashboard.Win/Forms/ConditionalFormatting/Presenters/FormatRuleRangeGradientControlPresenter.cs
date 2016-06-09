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
using DevExpress.DashboardWin.Localization;
namespace DevExpress.DashboardWin.Native {
	class FormatRuleRangeGradientControlPresenter : FormatRuleRangeBaseControlPresenter {
		protected override string Caption {
			get { return DashboardWinLocalizer.GetString(DashboardWinStringId.CommandFormatRuleRangeGradient); }
		}
		protected override string DescriptionFormat {
			get { return DashboardWinLocalizer.GetString(DashboardWinStringId.CommandFormatRuleRangeGradientDescription); }
		}
		IFormatRuleControlRangeGradientView SpecificView { get { return (IFormatRuleControlRangeGradientView)base.View; } }
		FormatConditionRangeGradient SpecificCondition {
			get { return (FormatConditionRangeGradient)base.Rule.Condition; }
			set { base.Rule.Condition = value; }
		}
		public FormatRuleRangeGradientControlPresenter(IServiceProvider serviceProvider, DataDashboardItem dashboardItem, DataItem dataItem, DashboardItemFormatRule initialFormatRule)
			: base(serviceProvider, dashboardItem, dataItem, initialFormatRule) {
		}
		public FormatRuleRangeGradientControlPresenter(IServiceProvider serviceProvider, DataDashboardItem dashboardItem, DataItem dataItem, FormatConditionRangeGradientPredefinedType type)
			: this(serviceProvider, dashboardItem, dataItem, null) {
			DashboardFormatConditionValueType valueType = IsPercentsSupported ? DashboardFormatConditionValueType.Percent : DashboardFormatConditionValueType.Number;
			CreateCondition(type, valueType);
			if(!IsPercentsSupported)
				SetNumericValuesToCondition(SpecificCondition.RangeSet.Count);
		}
		protected virtual void CreateCondition(FormatConditionRangeGradientPredefinedType type, DashboardFormatConditionValueType valueType) {
			 SpecificCondition =  new FormatConditionRangeGradient(type) { ValueType = valueType };
		}
		protected override FormatRuleControlViewInitializationContext CreateViewInitializationContext() {
			Dimension dimension = DataItem as Dimension;
			return new FormatRuleControlViewRangeGradientContext() {
				PredefinedType = SpecificCondition.ActualPredefinedType,
				DataType = DataItem.ActualDataFieldType,
				DateTimeGroupInterval = dimension != null ? dimension.DateTimeGroupInterval : DateTimeGroupInterval.None,
				IsPercentsSupported = IsPercentsSupported
			};
		}
		protected override void SubscribeViewEvents() {
			base.SubscribeViewEvents();
			if(SpecificView != null) {
				SpecificView.RangeGradientViewGenerating += OnRangeGradientViewGenerating;
			}
		}
		protected override void UnsubscribeViewEvents() {
			base.UnsubscribeViewEvents();
			if(SpecificView != null) {
				SpecificView.RangeGradientViewGenerating -= OnRangeGradientViewGenerating;
			}
		}
		protected override StyleMode GetRangeStyleMode(int rangeIndex) {
			if(rangeIndex == 0 || rangeIndex == SpecificCondition.RangeSet.Count - 1) {
				return StyleMode.GradientNonemptyStop;
			} else
				return SpecificCondition.SortedRanges[rangeIndex].StyleSettings == null ? StyleMode.GradientGenerated : StyleMode.GradientStop;
		}
		protected override void OnRangeViewChanging(object sender, FormatRuleRangeViewChangingEventArgs e) {
			int correctIndex = SpecificCondition.RangeSet.Count - e.Index - 1; 
			RangeInfo range = SpecificCondition.SortedRanges[correctIndex];
			if(e.Style != null)
				range.StyleSettings = e.Style.ToCoreStyle();
			if(e.Value != null)
				range.Value = CorrectRangeInfoValue(e.Value);
			if(e.ValueComparison.HasValue)
				range.ValueComparison = e.ValueComparison.Value;
			UpdateViewRanges();
		}
		void OnRangeGradientViewGenerating(object sender, FormatRuleRangeGradientViewGeneratingEventArgs e) {
			Dictionary<int, StyleSettingsBase> customStyles = new Dictionary<int, StyleSettingsBase>();
			for(int i = 0; i < SpecificCondition.SegmentCount; i++) {
				if(SpecificCondition.RangeSet[i].StyleSettings != null)
					customStyles[i] = SpecificCondition.RangeSet[i].StyleSettings;
			}
			int oldCount = SpecificCondition.SegmentCount;
			CreateCondition(e, customStyles, oldCount);
			foreach(KeyValuePair<int, StyleSettingsBase> pair in customStyles) {
				int index = (e.Count - 1) * pair.Key / (oldCount - 1);
				SpecificCondition.RangeSet[index].StyleSettings = pair.Value;
			}
			if(!SpecificView.IsPercent)
				SetNumericValuesToCondition(e.Count);
			UpdateViewRanges();
		}
		protected virtual void CreateCondition(FormatRuleRangeGradientViewGeneratingEventArgs e, Dictionary<int, StyleSettingsBase> customStyles, int oldCount) {
			SpecificCondition = new FormatConditionRangeGradient(customStyles[0], customStyles[oldCount - 1], e.Count);
		}
	}
}
