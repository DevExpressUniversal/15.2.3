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
	class FormatRuleRangeSetControlPresenter : FormatRuleRangeBaseControlPresenter {
		protected override string Caption {
			get { return DashboardWinLocalizer.GetString(DashboardWinStringId.CommandFormatRuleRangeSet); }
		}
		protected override string DescriptionFormat {
			get { return DashboardWinLocalizer.GetString(DashboardWinStringId.CommandFormatRuleRangeSetDescription); }
		}
		IFormatRuleControlRangeSetView SpecificView { get { return (IFormatRuleControlRangeSetView)base.View; } }
		FormatConditionRangeSet SpecificCondition {
			get { return (FormatConditionRangeSet)base.Rule.Condition; }
			set { base.Rule.Condition = value; }
		}
		public FormatRuleRangeSetControlPresenter(IServiceProvider serviceProvider, DataDashboardItem dashboardItem, DataItem dataItem, DashboardItemFormatRule initialFormatRule)
			: base(serviceProvider, dashboardItem, dataItem, initialFormatRule) {
		}
		public FormatRuleRangeSetControlPresenter(IServiceProvider serviceProvider, DataDashboardItem dashboardItem, DataItem dataItem, FormatConditionRangeSetPredefinedType type)
			: this(serviceProvider, dashboardItem, dataItem, null) {
				CreateCondition(type);
			if(!IsPercentsSupported)
				SetNumericValuesToCondition(SpecificCondition.RangeSet.Count);
		}
		protected virtual void CreateCondition(FormatConditionRangeSetPredefinedType type) {
			SpecificCondition = new FormatConditionRangeSet(type) { ValueType = IsPercentsSupported ? DashboardFormatConditionValueType.Percent : DashboardFormatConditionValueType.Number };
		}
		protected override FormatRuleControlViewInitializationContext CreateViewInitializationContext() {
			Dimension dimension = DataItem as Dimension;
			return new FormatRuleControlViewRangeSetContext() {
				StyleMode = GetRangeStyleMode(0),
				DataType = DataItem.ActualDataFieldType,
				DateTimeGroupInterval = dimension != null ? dimension.DateTimeGroupInterval : DateTimeGroupInterval.None,
				IsPercentsSupported = IsPercentsSupported
			};
		}
		protected override bool? InitializeView() {
			bool? result = base.InitializeView();
			SpecificView.PredefinedType = SpecificCondition.ActualPredefinedType;
			return result;
		}
		protected override void SubscribeViewEvents() {
			base.SubscribeViewEvents();
			if(SpecificView != null) {
				SpecificView.RangeViewCreating += OnRangeViewCreating;
				SpecificView.PredefinedStyleChanged += OnPredefinedStyleChanged;
			}
		}
		protected override void UnsubscribeViewEvents() {
			base.UnsubscribeViewEvents();
			if(SpecificView != null) {
				SpecificView.RangeViewCreating -= OnRangeViewCreating;
				SpecificView.PredefinedStyleChanged -= OnPredefinedStyleChanged;
			}
		}
		protected override StyleMode GetRangeStyleMode(int rangeIndex) {
			if(SpecificCondition.RangeSet.Count == 0)
				return StyleMode.Appearance;
			StyleSettingsBase styleSettings = SpecificCondition.SortedRanges[rangeIndex].ActualStyleSettings;
			if(styleSettings == null) {
				styleSettings = SpecificCondition.RangeSet.FindFirst(range => range.ActualStyleSettings != null).ActualStyleSettings;
			}
			return styleSettings is IconSettings ? StyleMode.Icon : StyleMode.Appearance;
		}
		protected override void OnRangeViewChanging(object sender, FormatRuleRangeViewChangingEventArgs e) {
			SpecificView.PredefinedType = FormatConditionRangeGenerator.GetPredefinedType(SpecificView.Ranges
				.Select(rangeView => rangeView.Style.ToCoreStyle())
				.Reverse()
			);
		}
		IFormatRuleRangeView OnRangeViewCreating(object sender, EventArgs e) {
			return new DashboardFormatRuleRangeView(GetRangeStyleMode(0));
		}
		void OnPredefinedStyleChanged(object sender, FormatRuleRangeSetPredefinedStyleChangedEventArgs e) {
			SpecificCondition.Generate(e.PredefinedStyleType);
			if(!SpecificView.IsPercent)
				SetNumericValuesToCondition(SpecificCondition.RangeSet.Count);
			UpdateViewRanges();
		}
	}
}
