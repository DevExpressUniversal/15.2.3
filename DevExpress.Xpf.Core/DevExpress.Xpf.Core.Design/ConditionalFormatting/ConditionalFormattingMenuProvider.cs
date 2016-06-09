#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using DevExpress.Xpf.Core.ConditionalFormatting;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
namespace DevExpress.Xpf.Core.Design.ConditionalFormatting {
	public abstract class ConditionalFormattingContextMenuProviderBase : PrimarySelectionContextMenuProvider {
		ConditionalFormattingLocalizer designLocalizer;
		MenuGroup conditionalFormattingMenuGroup;
		public ConditionalFormattingContextMenuProviderBase() {
			designLocalizer = new ConditionalFormattingLocalizer();
			CreateConditionalFormattingMenuItems();
			UpdateItemStatus += OnUpdateItemStatus;
		}
		void OnUpdateItemStatus(object sender, MenuActionEventArgs e) {
			SetIsMenuActionEnabled(conditionalFormattingMenuGroup.Items, IsConditionalFormattingMenuAllowed(GetPrimarySelection(e)));
		}
		void SetIsMenuActionEnabled(ObservableCollection<MenuBase> menus, bool isEnabled) {
			foreach(MenuBase menu in menus) {
				if(menu is MenuAction)
					((MenuAction)menu).Enabled = isEnabled;
				else if(menu is MenuGroup)
					SetIsMenuActionEnabled(((MenuGroup)menu).Items, isEnabled);
			}
		}
		void CreateConditionalFormattingMenuItems() {
			MenuGroup highlightCellsRules = CreateDropDownMenuGroup(ConditionalFormattingStringId.MenuColumnConditionalFormatting_HighlightCellsRules);
			CreateFormatConditionDialogMenuAction(ConditionalFormattingStringId.MenuColumnConditionalFormatting_HighlightCellsRules_GreaterThan, FormatConditionDialogType.GreaterThan, highlightCellsRules);
			CreateFormatConditionDialogMenuAction(ConditionalFormattingStringId.MenuColumnConditionalFormatting_HighlightCellsRules_LessThan, FormatConditionDialogType.LessThan, highlightCellsRules);
			CreateFormatConditionDialogMenuAction(ConditionalFormattingStringId.MenuColumnConditionalFormatting_HighlightCellsRules_Between, FormatConditionDialogType.Between, highlightCellsRules);
			CreateFormatConditionDialogMenuAction(ConditionalFormattingStringId.MenuColumnConditionalFormatting_HighlightCellsRules_EqualTo, FormatConditionDialogType.EqualTo, highlightCellsRules);
			CreateFormatConditionDialogMenuAction(ConditionalFormattingStringId.MenuColumnConditionalFormatting_HighlightCellsRules_TextThatContains, FormatConditionDialogType.TextThatContains, highlightCellsRules);
			CreateFormatConditionDialogMenuAction(ConditionalFormattingStringId.MenuColumnConditionalFormatting_HighlightCellsRules_ADateOccurring, FormatConditionDialogType.ADateOccurring, highlightCellsRules);
			CreateFormatConditionDialogMenuAction(ConditionalFormattingStringId.MenuColumnConditionalFormatting_HighlightCellsRules_CustomCondition, FormatConditionDialogType.CustomCondition, highlightCellsRules);
			MenuGroup topBottomRules = CreateDropDownMenuGroup(ConditionalFormattingStringId.MenuColumnConditionalFormatting_TopBottomRules);
			CreateFormatConditionDialogMenuAction(ConditionalFormattingStringId.MenuColumnConditionalFormatting_TopBottomRules_Top10Items, FormatConditionDialogType.Top10Items, topBottomRules);
			CreateFormatConditionDialogMenuAction(ConditionalFormattingStringId.MenuColumnConditionalFormatting_TopBottomRules_Top10Percent, FormatConditionDialogType.Top10Percent, topBottomRules);
			CreateFormatConditionDialogMenuAction(ConditionalFormattingStringId.MenuColumnConditionalFormatting_TopBottomRules_Bottom10Items, FormatConditionDialogType.Bottom10Items, topBottomRules);
			CreateFormatConditionDialogMenuAction(ConditionalFormattingStringId.MenuColumnConditionalFormatting_TopBottomRules_Bottom10Percent, FormatConditionDialogType.Bottom10Percent, topBottomRules);
			CreateFormatConditionDialogMenuAction(ConditionalFormattingStringId.MenuColumnConditionalFormatting_TopBottomRules_AboveAverage, FormatConditionDialogType.AboveAverage, topBottomRules);
			CreateFormatConditionDialogMenuAction(ConditionalFormattingStringId.MenuColumnConditionalFormatting_TopBottomRules_BelowAverage, FormatConditionDialogType.BelowAverage, topBottomRules);
			MenuGroup dataBars = CreateDropDownMenuGroup(ConditionalFormattingStringId.MenuColumnConditionalFormatting_DataBars);
			foreach(DesignFormatInfo info in FormatConditionDesignDialogHelper.GetDataBarDesignInfo())
				CreateDataBarFormatConditionMenuAction(info.NameID, info.FormatName, dataBars);
			MenuGroup colorScales = CreateDropDownMenuGroup(ConditionalFormattingStringId.MenuColumnConditionalFormatting_ColorScales);
			foreach(DesignFormatInfo info in FormatConditionDesignDialogHelper.GetColorScaleDesignInfo())
				CreateColorScaleFormatConditionMenuAction(info.NameID, info.FormatName, colorScales);
			MenuGroup iconSets = CreateDropDownMenuGroup(ConditionalFormattingStringId.MenuColumnConditionalFormatting_IconSets);
			foreach(DesignFormatInfo info in FormatConditionDesignDialogHelper.GetIconSetDesignInfo())
				CreateIconSetFormatConditionMenuAction(info.NameID, info.FormatName, iconSets);
			MenuGroup clearRules = CreateDropDownMenuGroup(ConditionalFormattingStringId.MenuColumnConditionalFormatting_ClearRules);
			CreateClearConditionRules(clearRules);
			conditionalFormattingMenuGroup = CreateDropDownMenuGroup(ConditionalFormattingStringId.MenuColumnConditionalFormatting);
			conditionalFormattingMenuGroup.Items.Add(highlightCellsRules);
			conditionalFormattingMenuGroup.Items.Add(topBottomRules);
			conditionalFormattingMenuGroup.Items.Add(dataBars);
			conditionalFormattingMenuGroup.Items.Add(colorScales);
			conditionalFormattingMenuGroup.Items.Add(iconSets);
			conditionalFormattingMenuGroup.Items.Add(clearRules);
			CreateFormatConditionMenuAction(ConditionalFormattingStringId.MenuColumnConditionalFormatting_ManageRules, x => x.ShowConditionalFormattingManager, conditionalFormattingMenuGroup);
			Items.Add(conditionalFormattingMenuGroup);
		}
		void CreateFormatConditionDialogMenuAction(ConditionalFormattingStringId id, FormatConditionDialogType dialogType, MenuGroup group) {
			CreateFormatConditionMenuAction(id, CreateDialogCommandSelector(dialogType), group);
		}
		void CreateDataBarFormatConditionMenuAction(ConditionalFormattingStringId id, string predefinedFormatName, MenuGroup group) {
			CreateIndicatorFormatConditionMenuAction(id, group, x => CreateDataBarFormatCondition(x, predefinedFormatName));
		}
		void CreateColorScaleFormatConditionMenuAction(ConditionalFormattingStringId id, string predefinedFormatName, MenuGroup group) {
			CreateIndicatorFormatConditionMenuAction(id, group, x => CreateColorScaleFormatCondition(x, predefinedFormatName));
		}
		void CreateIconSetFormatConditionMenuAction(ConditionalFormattingStringId id, string predefinedFormatName, MenuGroup group) {
			CreateIndicatorFormatConditionMenuAction(id, group, x => CreateIconSetFormatCondition(x, predefinedFormatName));
		}
		void CreateIndicatorFormatConditionMenuAction(ConditionalFormattingStringId id, MenuGroup group, Func<ModelItem, object> conditionCreator) {
			FormatConditionMenuAction action = new FormatConditionMenuAction(GetLocalizedString(id), x => x.AddFormatCondition, conditionCreator);
			SetupFormatConditionMenuAction(action, group);
		}
		void SetupFormatConditionMenuAction(FormatConditionMenuAction action, MenuGroup group) {
			action.Execute += OnExecuteFormatConditionMenuAction;
			group.Items.Add(action);
		}
		protected void CreateFormatConditionMenuAction(ConditionalFormattingStringId id, Func<IConditionalFormattingCommands, ICommand> commandSelector, MenuGroup group) {
			CreateFormatConditionMenuAction(GetLocalizedString(id), commandSelector, group);
		}
		protected void CreateFormatConditionMenuAction(string caption, Func<IConditionalFormattingCommands, ICommand> commandSelector, MenuGroup group) {
			FormatConditionMenuAction action = new FormatConditionMenuAction(caption, commandSelector, x => x.GetCurrentValue());
			SetupFormatConditionMenuAction(action, group);
		}
		Func<IConditionalFormattingCommands, ICommand> CreateDialogCommandSelector(FormatConditionDialogType dialogType) {
			switch(dialogType) {
				case FormatConditionDialogType.ADateOccurring:
					return x => x.ShowADateOccurringFormatConditionDialog;
				case FormatConditionDialogType.AboveAverage:
					return x => x.ShowAboveAverageFormatConditionDialog;
				case FormatConditionDialogType.BelowAverage:
					return x => x.ShowBelowAverageFormatConditionDialog;
				case FormatConditionDialogType.Between:
					return x => x.ShowBetweenFormatConditionDialog;
				case FormatConditionDialogType.Bottom10Items:
					return x => x.ShowBottom10ItemsFormatConditionDialog;
				case FormatConditionDialogType.Bottom10Percent:
					return x => x.ShowBottom10PercentFormatConditionDialog;
				case FormatConditionDialogType.CustomCondition:
					return x => x.ShowCustomConditionFormatConditionDialog;
				case FormatConditionDialogType.EqualTo:
					return x => x.ShowEqualToFormatConditionDialog;
				case FormatConditionDialogType.GreaterThan:
					return x => x.ShowGreaterThanFormatConditionDialog;
				case FormatConditionDialogType.LessThan:
					return x => x.ShowLessThanFormatConditionDialog;
				case FormatConditionDialogType.TextThatContains:
					return x => x.ShowTextThatContainsFormatConditionDialog;
				case FormatConditionDialogType.Top10Items:
					return x => x.ShowTop10ItemsFormatConditionDialog;
				case FormatConditionDialogType.Top10Percent:
					return x => x.ShowTop10PercentFormatConditionDialog;
				default:
					throw new InvalidOperationException();
			}
		}
		void OnExecuteFormatConditionMenuAction(object sender, MenuActionEventArgs e) {
			FormatConditionMenuAction action = (FormatConditionMenuAction)sender;
			ModelItem primarySelection = GetPrimarySelection(e);
			BeforeExecuteFormatConditionCommand(primarySelection);
			IConditionalFormattingCommands commands = GetCommands(primarySelection);
			if(commands != null)
				action.ExecuteCommand(commands, primarySelection);
		}
		MenuGroup CreateDropDownMenuGroup(ConditionalFormattingStringId id) {
			return new MenuGroup(GetLocalizedString(id)) { HasDropDown = true };
		}
		string GetLocalizedString(ConditionalFormattingStringId id) {
			return designLocalizer.GetLocalizedString(id);
		}
		ModelItem GetPrimarySelection(MenuActionEventArgs e) {
			return e.Selection.PrimarySelection;
		}
		protected abstract object CreateDataBarFormatCondition(ModelItem primarySelection, string formatName);
		protected abstract object CreateColorScaleFormatCondition(ModelItem primarySelection, string formatName);
		protected abstract object CreateIconSetFormatCondition(ModelItem primarySelection, string formatName);
		protected virtual IConditionalFormattingCommands GetCommands(ModelItem primarySelection) {
			return null;
		}
		protected virtual bool IsConditionalFormattingMenuAllowed(ModelItem primarySelection) {
			return true;
		}
		protected virtual void BeforeExecuteFormatConditionCommand(ModelItem primarySelection) { }
		protected abstract void CreateClearConditionRules(MenuGroup clearRules);
	}
	class FormatConditionMenuAction : MenuAction {
		Func<IConditionalFormattingCommands, ICommand> commandSelector;
		Func<ModelItem, object> parameterSelector;
		public FormatConditionMenuAction(string displayName, Func<IConditionalFormattingCommands, ICommand> commandSelector, Func<ModelItem, object> parameterSelector)
			: base(displayName) {
			this.commandSelector = commandSelector;
			this.parameterSelector = parameterSelector;
		}
		public void ExecuteCommand(IConditionalFormattingCommands commands, ModelItem primarySelection) {
			commandSelector(commands).Execute(parameterSelector(primarySelection));
		}
	}
}
