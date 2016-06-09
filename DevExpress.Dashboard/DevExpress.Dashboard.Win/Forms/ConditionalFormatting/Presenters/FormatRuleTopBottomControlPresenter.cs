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
using DevExpress.DashboardCommon;
namespace DevExpress.DashboardWin.Native {
	class FormatRuleTopBottomControlPresenter : FormatRuleControlPresenter {
		protected override string Caption {
			get { return SpecificCondition.TopBottom.Localize(); }
		}
		protected override string DescriptionFormat {
			get { return SpecificCondition.TopBottom.Descript(); }
		}
		IFormatRuleControlTopBottomView SpecificView { get { return (IFormatRuleControlTopBottomView)base.View; } }
		FormatConditionTopBottom SpecificCondition {
			get { return (FormatConditionTopBottom)base.Rule.Condition; }
			set { base.Rule.Condition = value; }
		}
		public FormatRuleTopBottomControlPresenter(IServiceProvider serviceProvider, DataDashboardItem dashboardItem, DataItem dataItem, DashboardItemFormatRule initialFormatRule)
			: base(serviceProvider, dashboardItem, dataItem, initialFormatRule) {
		}
		public FormatRuleTopBottomControlPresenter(IServiceProvider serviceProvider, DataDashboardItem dashboardItem, DataItem dataItem, DashboardFormatConditionTopBottomType type)
			: this(serviceProvider, dashboardItem, dataItem, null) {
			SpecificCondition = new FormatConditionTopBottom() { TopBottom = type };
		}
		protected override bool? InitializeView() {
			bool? result = base.InitializeView();
			FormatConditionTopBottom condition = SpecificCondition;
			SpecificView.Rank = condition.Rank;
			SpecificView.IsPercent = condition.RankType == DashboardFormatConditionValueType.Percent;
			SpecificView.Style = StyleSettingsContainer.ToStyleContainer(SpecificCondition.StyleSettings);
			return result;
		}
		protected override void ApplyView() {
			base.ApplyView();
			FormatConditionTopBottom condition = SpecificCondition;
			condition.Rank = SpecificView.Rank;
			condition.RankType = SpecificView.IsPercent ? DashboardFormatConditionValueType.Percent : DashboardFormatConditionValueType.Number;
			condition.StyleSettings = SpecificView.Style.ToCoreStyle();
		}
	}
}
