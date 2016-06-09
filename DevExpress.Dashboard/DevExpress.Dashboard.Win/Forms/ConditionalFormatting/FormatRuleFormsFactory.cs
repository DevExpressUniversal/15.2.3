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
using System.Windows.Forms;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.ServiceModel;
namespace DevExpress.DashboardWin.Native {
	static class FormatRuleViewFactory {
		class FormatRulesFactoryItem {
			public FormatRulesFactoryItem(Type viewControlType, Type presenterType) {
				ViewControlType = viewControlType;
				PresenterType = presenterType;
			}
			public Type ViewControlType { get; set; }
			public Type PresenterType { get; set; }
		}
		readonly static Dictionary<Type, FormatRulesFactoryItem> items = new Dictionary<Type, FormatRulesFactoryItem>();
		static FormatRuleViewFactory() {
			items.Add(typeof(FormatConditionValue), new FormatRulesFactoryItem(typeof(FormatRuleControlValue), typeof(FormatRuleValueControlPresenter)));
			items.Add(typeof(FormatConditionExpression), new FormatRulesFactoryItem(typeof(FormatRuleControlExpression), typeof(FormatRuleExpressionControlPresenter)));
			items.Add(typeof(FormatConditionDateOccuring), new FormatRulesFactoryItem(typeof(FormatRuleControlDateOccurring), typeof(FormatRuleDateOccurringControlPresenter)));
			items.Add(typeof(FormatConditionTopBottom), new FormatRulesFactoryItem(typeof(FormatRuleControlTopBottom), typeof(FormatRuleTopBottomControlPresenter)));
			items.Add(typeof(FormatConditionAverage), new FormatRulesFactoryItem(typeof(FormatRuleControlAboveBelowAverage), typeof(FormatRuleAboveBelowAverageControlPresenter)));
			items.Add(typeof(FormatConditionRangeSet), new FormatRulesFactoryItem(typeof(FormatRuleControlRangeSet), typeof(FormatRuleRangeSetControlPresenter)));
			items.Add(typeof(FormatConditionRangeGradient), new FormatRulesFactoryItem(typeof(FormatRuleControlRangeGradient), typeof(FormatRuleRangeGradientControlPresenter)));
			items.Add(typeof(FormatConditionGradientRangeBar), new FormatRulesFactoryItem(typeof(FormatRuleControlGradientRangeBar), typeof(FormatRuleGradientRangeBarControlPresenter)));
			items.Add(typeof(FormatConditionColorRangeBar), new FormatRulesFactoryItem(typeof(FormatRuleControlColorRangeBar), typeof(FormatRuleColorRangeBarControlPresenter)));
			items.Add(typeof(FormatConditionBar), new FormatRulesFactoryItem(typeof(FormatRuleControlBar), typeof(FormatRuleBarControlPresenter)));
		}
		public static Form CreateFormatRuleManagerForm(IServiceProvider serviceProvider, DataDashboardItem dashboardItem, DataItem dataItem) {
			FormatRuleControlManager control = new FormatRuleControlManager();
			IDashboardGuiContextService guiService = serviceProvider.RequestServiceStrictly<IDashboardGuiContextService>();
			FormatRuleFormBase form = new FormatRuleFormBase(guiService.LookAndFeel, control, true);
			FormatRuleManagerPresenter presenter = new FormatRuleManagerPresenter(serviceProvider, dashboardItem, dataItem);
			presenter.Initialize(control, form);
			return form;
		}
		public static Form CreateControlViewForm(IServiceProvider serviceProvider, DataDashboardItem dashboardItem, DashboardItemFormatRule rule) {
			if(rule == null || !rule.IsValid)
				throw new ArgumentException("Rule is invalid");
			DataItem dataItem = rule.LevelCore.Item;
			return CreateControlViewForm(serviceProvider, rule.Condition.GetType(), new object[] { serviceProvider, dashboardItem, dataItem, rule});
		}
		public static Form CreateControlViewForm<TCondition>(IServiceProvider serviceProvider, DataDashboardItem dashboardItem, DataItem dataItem) where TCondition : FormatConditionBase {
			return CreateControlViewForm(serviceProvider, typeof(TCondition), new object[] { serviceProvider, dashboardItem, dataItem });
		}
		public static Form CreateControlViewForm<TCondition, TEnum>(IServiceProvider serviceProvider, DataDashboardItem dashboardItem, DataItem dataItem, TEnum enumSpecificValue) 
				where TCondition : FormatConditionBase 
				where TEnum : struct {
			return CreateControlViewForm(serviceProvider, typeof(TCondition), new object[] { serviceProvider, dashboardItem, dataItem, enumSpecificValue });
		}
		static Form CreateControlViewForm(IServiceProvider serviceProvider, Type conditionType, params object[] presenterArgs) {
			FormatRulesFactoryItem factoryItem = GetFactoryItem(conditionType);
			IDashboardGuiContextService guiService = serviceProvider.RequestServiceStrictly<IDashboardGuiContextService>();
			FormatRuleControlPresenter presenter = CreatePresenter(factoryItem.PresenterType, presenterArgs);
			FormatRuleControlBase control = CreateControlView(factoryItem.ViewControlType);
			FormatRuleFormBase form = new FormatRuleFormBase(guiService.LookAndFeel, control);
			presenter.Initialize(control, form);
			return form;
		}
		static FormatRulesFactoryItem GetFactoryItem(Type conditionType) {
			FormatRulesFactoryItem factoryItem;
			if(!items.TryGetValue(conditionType, out factoryItem))
				throw new ArgumentException("Undefined type of condition", conditionType.ToString());
			return factoryItem;
		}
		static FormatRuleControlBase CreateControlView(Type controlViewType) {
			return (FormatRuleControlBase)Activator.CreateInstance(controlViewType);
		}
		static FormatRuleControlPresenter CreatePresenter(Type presenterType, params object[] presenterArgs) {
			return (FormatRuleControlPresenter)Activator.CreateInstance(presenterType, presenterArgs);
		}
	}
}
