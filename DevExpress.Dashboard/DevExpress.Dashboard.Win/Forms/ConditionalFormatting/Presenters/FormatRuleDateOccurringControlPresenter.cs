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
using DevExpress.DashboardWin.Localization;
namespace DevExpress.DashboardWin.Native {
	class FormatRuleDateOccurringControlPresenter : FormatRuleControlPresenter {
		protected override string Caption {
			get { return DashboardWinLocalizer.GetString(DashboardWinStringId.CommandFormatRuleDateOccurring); }
		}
		protected override string DescriptionFormat {
			get { return DashboardWinLocalizer.GetString(DashboardWinStringId.CommandFormatRuleDateOccurringDescription); }
		}
		IFormatRuleControlDateOccurringView SpecificView { get { return (IFormatRuleControlDateOccurringView)base.View; } }
		FormatConditionDateOccuring SpecificCondition {
			get { return (FormatConditionDateOccuring)base.Rule.Condition; }
			set { base.Rule.Condition = value; }
		}
		public FormatRuleDateOccurringControlPresenter(IServiceProvider serviceProvider, DataDashboardItem dashboardItem, DataItem dataItem, DashboardItemFormatRule initialFormatRule)
			: base(serviceProvider, dashboardItem, dataItem, initialFormatRule) {
		}
		public FormatRuleDateOccurringControlPresenter(IServiceProvider serviceProvider, DataDashboardItem dashboardItem, DataItem dataItem)
			: this(serviceProvider, dashboardItem, dataItem, null) {
			SpecificCondition = new FormatConditionDateOccuring();
		}
		protected override bool? InitializeView() {
			bool? result = base.InitializeView();
			FormatConditionDateOccuring condition = SpecificCondition;
			SpecificView.DateType = condition.DateType;
			SpecificView.Style = StyleSettingsContainer.ToStyleContainer(SpecificCondition.StyleSettings);
			return result;
		}
		protected override void ApplyView() {
			base.ApplyView();
			FormatConditionDateOccuring condition = SpecificCondition;
			condition.DateType = SpecificView.DateType;
			condition.StyleSettings = SpecificView.Style.ToCoreStyle();
		}
	}
}
