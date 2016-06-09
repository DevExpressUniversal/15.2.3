#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.ExpressApp.Editors;
namespace DevExpress.ExpressApp.ConditionalAppearance {
	internal static class AppearanceRulesFilterHelper {
		internal static List<AppearanceRule> FilterRules(IViewInfo view, object item, List<AppearanceRule> rulesForProperty) {
			if(rulesForProperty.Count == 0) {
				return rulesForProperty;
			}
			else {
				rulesForProperty = FilterRulesByItem(item, rulesForProperty);
				return FilterRulesByViewEditMode(view, rulesForProperty);
			}
		}
		private static bool IsCustomRule(AppearanceRule rule) {
			bool result = rule.Properties.BackColor.HasValue ||
									rule.Properties.FontColor.HasValue ||
									rule.Properties.FontStyle.HasValue ||
									rule.Properties.Enabled.HasValue ||
									rule.Properties.Visibility.HasValue;
			return !result;
		}
		private static bool RulePropertiesFitToItem(ViewItem viewItem, AppearanceRule ruleProperties) {
			bool result = false;
			if(viewItem == null || viewItem.Control != null) {
				result = ruleProperties.Properties.BackColor.HasValue ||
						ruleProperties.Properties.FontColor.HasValue ||
						ruleProperties.Properties.FontStyle.HasValue ||
						ruleProperties.Properties.Enabled.HasValue;
			}
			return result || (ruleProperties.Properties.Visibility.HasValue && ruleProperties.Properties.Visibility.Value != ViewItemVisibility.Show) || IsCustomRule(ruleProperties);
		}
		private static bool RulePropertiesFitToView(IViewInfo view, AppearanceRule ruleProperties) {
			bool ruleFitToView = view.AllowEdit != ruleProperties.Properties.Enabled;
			bool ruleFitToProperties = ruleProperties.Properties.BackColor.HasValue ||
					ruleProperties.Properties.FontColor.HasValue ||
					ruleProperties.Properties.FontStyle.HasValue ||
					ruleProperties.Properties.Visibility.HasValue;
			return (ruleFitToView && view.AllowEdit) || ruleFitToProperties || IsCustomRule(ruleProperties);
		}
		private static List<AppearanceRule> FilterRulesByItem(object item, List<AppearanceRule> rulesForProperty) {
			Dictionary<string, List<AppearanceRule>> filteredRulesByPropertyName = new Dictionary<string, List<AppearanceRule>>();
			List<AppearanceRule> result = new List<AppearanceRule>();
			ViewItem viewItem = item as ViewItem;
			for(int i = 0; i < rulesForProperty.Count; i++) {
				AppearanceRule rule = rulesForProperty[i];
				if(RulePropertiesFitToItem(viewItem, rule)) {
					result.Add(rule);
				}
				else {
					if(rule.Properties.Visibility.HasValue && rule.Properties.Visibility.Value == ViewItemVisibility.Show) {
						AddRuleToFilteredRules(filteredRulesByPropertyName, rule);
					}
				}
			}
			Predicate<AppearanceRule> isHideRule = delegate(AppearanceRule rule) {
				return rule.Properties.Visibility.HasValue && rule.Properties.Visibility.Value != ViewItemVisibility.Show;
			};
			return CalcRulesSuperposition(rulesForProperty, filteredRulesByPropertyName, result, isHideRule);
		}
		private static List<AppearanceRule> FilterRulesByViewEditMode(IViewInfo view, List<AppearanceRule> rulesForProperty) {
			Dictionary<string, List<AppearanceRule>> filteredRulesByPropertyName = new Dictionary<string, List<AppearanceRule>>();
			List<AppearanceRule> result = new List<AppearanceRule>();
			for(int i = 0; i < rulesForProperty.Count; i++) {
				AppearanceRule rule = rulesForProperty[i];
				if(RulePropertiesFitToView(view, rule)) {
					result.Add(rule);
				}
				else {
					if(view.AllowEdit) {
						if(rule.Properties.Enabled.HasValue && rule.Properties.Enabled.Value) {
							AddRuleToFilteredRules(filteredRulesByPropertyName, rule);
						}
					}
				}
			}
			Predicate<AppearanceRule> isDisabledRule = delegate(AppearanceRule rule) {
				return rule.Properties.Enabled.HasValue && !rule.Properties.Enabled.Value;
			};
			return CalcRulesSuperposition(rulesForProperty, filteredRulesByPropertyName, result, isDisabledRule);
		}
		private static void AddRuleToFilteredRules(Dictionary<string, List<AppearanceRule>> filteredRulesByPropertyName, AppearanceRule ruleProperties) {
			foreach(string targetItem in ruleProperties.TargetItems) {
				List<AppearanceRule> rulesToEnabled = null;
				if(!filteredRulesByPropertyName.TryGetValue(targetItem, out rulesToEnabled)) {
					rulesToEnabled = new List<AppearanceRule>();
					filteredRulesByPropertyName.Add(targetItem, rulesToEnabled);
				}
				rulesToEnabled.Add(ruleProperties);
			}
		}
		private static List<AppearanceRule> CalcRulesSuperposition(List<AppearanceRule> rulesForProperty, Dictionary<string, List<AppearanceRule>> filteredRulesByPropertyName, List<AppearanceRule> result, Predicate<AppearanceRule> ruleIs) {
			if(filteredRulesByPropertyName.Count > 0) {
				bool needAddAllRules = false;
				List<string> disabledList = new List<string>();
				for(int i = 0; i < rulesForProperty.Count; i++) {
					if(ruleIs(rulesForProperty[i])) {
						foreach(string targetItem in rulesForProperty[i].TargetItems) {
							if(targetItem == AppearanceRule.SelectAllString) {
								needAddAllRules = true;
								break;
							}
							else {
								if(!disabledList.Contains(targetItem)) {
									disabledList.Add(targetItem);
								}
							}
						}
					}
					if(needAddAllRules) {
						break;
					}
				}
				if(needAddAllRules) {
					return rulesForProperty;
				}
				foreach(string key in filteredRulesByPropertyName.Keys) {
					if(disabledList.Contains(key)) {
						List<AppearanceRule> rulesToEnabled = filteredRulesByPropertyName[key];
						foreach(AppearanceRule ruleToEnabled in rulesToEnabled) {
							if(!result.Contains(ruleToEnabled)) {
								result.Add(ruleToEnabled);
							}
						}
					}
				}
			}
			return result;
		}
	}
}
