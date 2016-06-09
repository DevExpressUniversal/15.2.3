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
using System.Linq;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Localization;
namespace DevExpress.DashboardWin.Native {
	class FormatRuleColorRangeBarControlPresenter : FormatRuleRangeSetControlPresenter {
		protected override string Caption {
			get { return DashboardWinLocalizer.GetString(DashboardWinStringId.CommandFormatRuleColorRangeBar); }
		}
		protected override string DescriptionFormat {
			get { return DashboardWinLocalizer.GetString(DashboardWinStringId.CommandFormatRuleColorRangeBarDescription); }
		}
		protected override bool IsApplyToReadOnly { get { return true; } }
		protected override bool IsApplyToColumnSupported { get { return false; } }
		IFormatRuleControlColorRangeBarView SpecificView { get { return (IFormatRuleControlColorRangeBarView)base.View; } }
		FormatConditionColorRangeBar SpecificCondition {
			get { return (FormatConditionColorRangeBar)base.Rule.Condition; }
			set { base.Rule.Condition = value; }
		}
		FormatConditionBarOptions BarOptions { get { return SpecificCondition.BarOptions; } }
		public FormatRuleColorRangeBarControlPresenter(IServiceProvider serviceProvider, DataDashboardItem dashboardItem, DataItem dataItem, DashboardItemFormatRule initialFormatRule)
			: base(serviceProvider, dashboardItem, dataItem, initialFormatRule) {
		}
		public FormatRuleColorRangeBarControlPresenter(IServiceProvider serviceProvider, DataDashboardItem dashboardItem, DataItem dataItem, FormatConditionRangeSetPredefinedType type)
			: base(serviceProvider, dashboardItem, dataItem, type) {
		}
		protected override void CreateCondition(FormatConditionRangeSetPredefinedType type) {
			SpecificCondition = new FormatConditionColorRangeBar(type) { ValueType = IsPercentsSupported ? DashboardFormatConditionValueType.Percent : DashboardFormatConditionValueType.Number };
		}
		protected override StyleMode GetRangeStyleMode(int rangeIndex) {
			return StyleMode.Bar;
		}
		protected override FormatRuleControlViewInitializationContext CreateViewInitializationContext() {
			Dimension dimension = DataItem as Dimension;
			FormatRuleViewColorRangeBarContext context = new FormatRuleViewColorRangeBarContext() {
				StyleMode = GetRangeStyleMode(0),
				DataType = DataItem.ActualDataFieldType,
				DateTimeGroupInterval = dimension != null ? dimension.DateTimeGroupInterval : DateTimeGroupInterval.None,
				IsPercentsSupported = IsPercentsSupported
			};
			IFormatRuleBarOptionsInitializationContext contextBarOptions = context.BarOptions;
			FormatConditionBarOptions barOptions = SpecificCondition.BarOptions;
			contextBarOptions.AllowNegativeAxis = barOptions.AllowNegativeAxis;
			contextBarOptions.DrawAxis = barOptions.DrawAxis;
			contextBarOptions.ShowBarOnly = barOptions.ShowBarOnly;
			return context;
		}
		protected override void ApplyView() {
			base.ApplyView();
			SpecificCondition.BarOptions.AllowNegativeAxis = SpecificView.BarOptions.AllowNegativeAxis;
			SpecificCondition.BarOptions.DrawAxis = SpecificView.BarOptions.DrawAxis;
			SpecificCondition.BarOptions.ShowBarOnly = SpecificView.BarOptions.ShowBarOnly;
		}
		protected override void OnRangeViewChanging(object sender, FormatRuleRangeViewChangingEventArgs e) {
			SpecificView.PredefinedType = FormatConditionRangeGenerator.GetPredefinedBarType(SpecificView.Ranges
				.Select(rangeView => rangeView.Style.ToCoreStyle())
				.Reverse()
			);
		}
	}
}
