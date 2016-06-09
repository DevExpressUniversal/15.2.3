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
using System.Linq;
using System.Text;
using DevExpress.Xpf.Bars;
using System.Windows.Media;
using System.Windows.Input;
using System.Collections;
using DevExpress.Data;
using DevExpress.Xpf.Core.Native;
using System.Windows;
using System.Reflection;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Utils.Themes;
using DevExpress.Xpf.Core.ConditionalFormattingManager;
using DevExpress.Xpf.Core.ConditionalFormatting.Themes;
namespace DevExpress.Xpf.Core.ConditionalFormatting.Native {
	public class DefaultConditionalFormattingMenuItemNames {
		public const string ConditionalFormatting = "ConditionalFormatting",
							ConditionalFormatting_HighlightCellsRules = "ConditionalFormatting_HighlightCellsRules",
							ConditionalFormatting_HighlightCellsRules_GreaterThan = "ConditionalFormatting_HighlightCellsRules_GreaterThan",
							ConditionalFormatting_HighlightCellsRules_LessThan = "ConditionalFormatting_HighlightCellsRules_LessThan",
							ConditionalFormatting_HighlightCellsRules_Between = "ConditionalFormatting_HighlightCellsRules_Between",
							ConditionalFormatting_HighlightCellsRules_EqualTo = "ConditionalFormatting_HighlightCellsRules_EqualTo",
							ConditionalFormatting_HighlightCellsRules_TextThatContains = "ConditionalFormatting_HighlightCellsRules_TextThatContains",
							ConditionalFormatting_HighlightCellsRules_ADateOccurring = "ConditionalFormatting_HighlightCellsRules_ADateOccurring",
							ConditionalFormatting_HighlightCellsRules_CustomCondition = "ConditionalFormatting_HighlightCellsRules_CustomCondition",
							ConditionalFormatting_TopBottomRules = "ConditionalFormatting_TopBottomRules",
							ConditionalFormatting_TopBottomRules_Top10Items = "ConditionalFormatting_TopBottomRules_Top10Items",
							ConditionalFormatting_TopBottomRules_Bottom10Items = "ConditionalFormatting_TopBottomRules_Bottom10Items",
							ConditionalFormatting_TopBottomRules_Top10Percent = "ConditionalFormatting_TopBottomRules_Top10Percent",
							ConditionalFormatting_TopBottomRules_Bottom10Percent = "ConditionalFormatting_TopBottomRules_Bottom10Percent",
							ConditionalFormatting_TopBottomRules_AboveAverage = "ConditionalFormatting_TopBottomRules_AboveAverage",
							ConditionalFormatting_TopBottomRules_BelowAverage = "ConditionalFormatting_TopBottomRules_BelowAverage",
							ConditionalFormatting_DataBars = "ConditionalFormatting_DataBars",
							ConditionalFormatting_ColorScales = "ConditionalFormatting_ColorScales",
							ConditionalFormatting_IconSets = "ConditionalFormatting_IconSets",
							ConditionalFormatting_ClearRules = "ConditionalFormatting_ClearRules",
							ConditionalFormatting_ClearRules_FromAllColumns = "ConditionalFormatting_ClearRules_FromAllColumns",
							ConditionalFormatting_ClearRules_FromCurrentColumns = "ConditionalFormatting_ClearRules_FromCurrentColumns",
							ConditionalFormatting_ManageRules = "Manage_Rules";
	}
	public class ConditionalFormattingDialogDirector {
		#region Nested Classes
		class FormatsViewModel {
			public FormatsViewModel(IEnumerable groups) {
				this.FormatConditionGroups = groups;
			}
			public IEnumerable FormatConditionGroups { get; private set; }
		}
		#endregion
		public void CreateMenuItems(IDataColumnInfo info) {
			this.info = info;
			BarSubItem rootItem = CreateBarSubItem(DefaultConditionalFormattingMenuItemNames.ConditionalFormatting, ConditionalFormattingStringId.MenuColumnConditionalFormatting, true, GetConditionalFormattingMenuImage(string.Empty), null);
			Type fieldType = info.FieldType;
			if(fieldType == null)
				return;
			BarSubItem highlightCellsRulesItem = CreateBarSubItem(rootItem.ItemLinks, DefaultConditionalFormattingMenuItemNames.ConditionalFormatting_HighlightCellsRules, ConditionalFormattingStringId.MenuColumnConditionalFormatting_HighlightCellsRules, false, GetConditionalFormattingMenuImage("HighlightCellsRules"), null);
			foreach(var item in ConditionalFormattingMenuHelper.GetAvailableHighlightItems(fieldType)) {
				CreateFormatDialogBarButtonItem(item, highlightCellsRulesItem, "HighlightCellsRules");
			}
			var availableTopBottomRules = ConditionalFormattingMenuHelper.GetAvailableTopBottomRuleItems(fieldType, IsServerMode).ToArray();
			if(availableTopBottomRules.Any()) {
				BarSubItem topBottomRulesItem = CreateBarSubItem(rootItem.ItemLinks, DefaultConditionalFormattingMenuItemNames.ConditionalFormatting_TopBottomRules, ConditionalFormattingStringId.MenuColumnConditionalFormatting_TopBottomRules, false, GetConditionalFormattingMenuImage("TopBottomRules"), null);
				foreach(var item in availableTopBottomRules) {
					CreateFormatDialogBarButtonItem(item, topBottomRulesItem, "TopBottomRules");
				}
			}
			bool startNewIndicatorsGroup = true;
			if(ConditionalFormattingMenuHelper.ShowDatBarMenu(fieldType)) {
				var dataBarsItem = CreateBarSplitButtonItem(rootItem.ItemLinks, DefaultConditionalFormattingMenuItemNames.ConditionalFormatting_DataBars, ConditionalFormattingStringId.MenuColumnConditionalFormatting_DataBars, startNewIndicatorsGroup, GetConditionalFormattingMenuImage("SolidBlueDataBar"));
				ConfigureSplitItem(dataBarsItem, ConditionalFormattingThemeKeys.DataBarMenuItemContent, GetGroupedFormatItems(FormatsOwner.PredefinedDataBarFormats, x => new[] { x.Icon }, CreateDataBarCondition));
				startNewIndicatorsGroup = false;
			}
			if(ConditionalFormattingMenuHelper.ShowColorScaleMenu(fieldType)) {
				var colorScalesItem = CreateBarSplitButtonItem(rootItem.ItemLinks, DefaultConditionalFormattingMenuItemNames.ConditionalFormatting_ColorScales, ConditionalFormattingStringId.MenuColumnConditionalFormatting_ColorScales, startNewIndicatorsGroup, GetConditionalFormattingMenuImage("GreenYellowRed"));
				ConfigureSplitItem(colorScalesItem, ConditionalFormattingThemeKeys.ColorScaleMenuItemContent, GetColorScaleGroups());
				startNewIndicatorsGroup = false;
			}
			if(ConditionalFormattingMenuHelper.ShowIconSetMenu(fieldType)) {
				var iconSetsItem = CreateBarSplitButtonItem(rootItem.ItemLinks, DefaultConditionalFormattingMenuItemNames.ConditionalFormatting_IconSets, ConditionalFormattingStringId.MenuColumnConditionalFormatting_IconSets, startNewIndicatorsGroup, GetConditionalFormattingMenuImage("IconSetArrows5", "ConditionalFormattins"));
				ConfigureSplitItem(iconSetsItem, ConditionalFormattingThemeKeys.IconSetMenuItemContent, GetGroupedFormatItems(FormatsOwner.PredefinedIconSetFormats, x => (x.Format as IconSetFormat).Elements.Select(y => y.Icon), CreateIconCondition));
			}
			BarSubItem clearRulesItem = CreateBarSubItem(rootItem.ItemLinks, DefaultConditionalFormattingMenuItemNames.ConditionalFormatting_ClearRules, ConditionalFormattingStringId.MenuColumnConditionalFormatting_ClearRules, true, GetConditionalFormattingMenuImage("ClearRules"), null);
			CreateClearMenuItems(info, clearRulesItem);
			if(AllowConditionalFormattingManager && GridAssemblyHelper.Instance.IsGridAvailable)
				CreateBarButtonItem(rootItem.ItemLinks, DefaultConditionalFormattingMenuItemNames.ConditionalFormatting_ManageRules, ConditionalFormattingStringId.MenuColumnConditionalFormatting_ManageRules, false, GetConditionalFormattingMenuImage("ManageRules"), commands.ShowConditionalFormattingManager, info);
		}
		protected virtual void CreateClearMenuItems(IDataColumnInfo info, BarSubItem clearRulesItem) {
			CreateBarButtonItem(clearRulesItem.ItemLinks, DefaultConditionalFormattingMenuItemNames.ConditionalFormatting_ClearRules_FromAllColumns, ConditionalFormattingStringId.MenuColumnConditionalFormatting_ClearRules_FromAllColumns, false, null, commands.ClearFormatConditionsFromAllColumns);
			CreateBarButtonItem(clearRulesItem.ItemLinks, DefaultConditionalFormattingMenuItemNames.ConditionalFormatting_ClearRules_FromCurrentColumns, ConditionalFormattingStringId.MenuColumnConditionalFormatting_ClearRules_FromCurrentColumns, false, null, commands.ClearFormatConditionsFromColumn, info);
		}
		IDialogContext context;
		FrameworkElement resourceOwner;
		IConditionalFormattingCommands commands;
		IConditionalFormattingDialogBuilder builder;
		IDataColumnInfo info;
		IFormatsOwner FormatsOwner { get { return context.PredefinedFormatsOwner; } }
		protected FrameworkElement ResourceOwner { get { return resourceOwner; } }
		protected IConditionalFormattingDialogBuilder Builder { get { return builder; } }
		protected IConditionalFormattingCommands Commands { get { return commands; } }
		public bool AllowConditionalFormattingManager { get; set; }
		public bool IsServerMode { get; set; }
		public ConditionalFormattingDialogDirector(IDialogContext context, IConditionalFormattingCommands commands, IConditionalFormattingDialogBuilder builder, FrameworkElement resourceOwner) {
			this.context = context;
			this.commands = commands;
			this.builder = builder;
			this.resourceOwner = resourceOwner;
			AllowConditionalFormattingManager = true;
		}
		ImageSource GetConditionalFormattingMenuImage(string name, string prefix = "ConditionalFormatting") {
			Uri uri = new Uri(ConditionalFormatResourceHelper.BasePathCore + "Menu/" + prefix + name + "_16x16.png", UriKind.Absolute);
			return new BitmapImage(uri);
		}
		void CreateFormatDialogBarButtonItem(FormatConditionDialogType dialogType, BarSubItem parent, string groupName) {
			string name = (string)typeof(DefaultConditionalFormattingMenuItemNames).GetField("ConditionalFormatting_" + groupName + "_" + dialogType, BindingFlags.Static | BindingFlags.Public).GetValue(null);
			string caption = ConditionalFormattingLocalizer.GetString((ConditionalFormattingStringId)Enum.Parse(typeof(ConditionalFormattingStringId), "MenuColumnConditionalFormatting_" + groupName + "_" + dialogType, false));
			ImageSource image1 = GetConditionalFormattingMenuImage(dialogType.ToString());
			CreateBarButtonItem(parent.ItemLinks, name, caption, false, image1, GetShowDialogCommand(dialogType), info);
		}
		ICommand GetShowDialogCommand(FormatConditionDialogType dialogType) {
			switch(dialogType) {
				case FormatConditionDialogType.GreaterThan:
					return commands.ShowGreaterThanFormatConditionDialog;
				case FormatConditionDialogType.LessThan:
					return commands.ShowLessThanFormatConditionDialog;
				case FormatConditionDialogType.Between:
					return commands.ShowBetweenFormatConditionDialog;
				case FormatConditionDialogType.EqualTo:
					return commands.ShowEqualToFormatConditionDialog;
				case FormatConditionDialogType.TextThatContains:
					return commands.ShowTextThatContainsFormatConditionDialog;
				case FormatConditionDialogType.ADateOccurring:
					return commands.ShowADateOccurringFormatConditionDialog;
				case FormatConditionDialogType.CustomCondition:
					return commands.ShowCustomConditionFormatConditionDialog;
				case FormatConditionDialogType.Top10Items:
					return commands.ShowTop10ItemsFormatConditionDialog;
				case FormatConditionDialogType.Bottom10Items:
					return commands.ShowBottom10ItemsFormatConditionDialog;
				case FormatConditionDialogType.Top10Percent:
					return commands.ShowTop10PercentFormatConditionDialog;
				case FormatConditionDialogType.Bottom10Percent:
					return commands.ShowBottom10PercentFormatConditionDialog;
				case FormatConditionDialogType.AboveAverage:
					return commands.ShowAboveAverageFormatConditionDialog;
				case FormatConditionDialogType.BelowAverage:
					return commands.ShowBelowAverageFormatConditionDialog;
				default:
					throw new InvalidOperationException();
			}
		}
		void ConfigureSplitItem(BarSplitButtonItem item, ConditionalFormattingThemeKeys templateKey, IEnumerable groups) {
			item.ActAsDropDown = true;
			var content = TemplateHelper.LoadFromTemplate<FrameworkElement>((DataTemplate)FindResource(templateKey));
			content.DataContext = new FormatsViewModel(groups);
			item.PopupControl = new PopupControlContainer() { Content = content };
		}
		IEnumerable GetColorScaleGroups() {
			yield return new { Header = string.Empty, Items = ConvertToBindableItems(FormatsOwner.PredefinedColorScaleFormats, x => new[] { x.Icon }, CreateColorScaleCondition) };
		}
		IEnumerable GetGroupedFormatItems(IEnumerable<FormatInfo> formatInfo, Func<FormatInfo, IEnumerable<ImageSource>> iconsExtractor, Func<string, string, DependencyObject> conditionCreator) {
			return formatInfo
					.GroupBy(x => x.GroupName)
					.Select(x => new {
						Header = x.Key,
						Items = ConvertToBindableItems(x, iconsExtractor, conditionCreator),
					}).ToArray();
		}
		IEnumerable ConvertToBindableItems(IEnumerable<FormatInfo> formatInfo, Func<FormatInfo, IEnumerable<ImageSource>> iconsExtractor, Func<string, string, DependencyObject> conditionCreator) {
			return formatInfo.Select(x => new {
				Name = x.DisplayName,
				Description = x.Description,
				Icons = iconsExtractor(x).ToArray(),
				Command = commands.AddFormatCondition,
				Format = x.Format,
				FormatCondition = conditionCreator(x.FormatName, info.FieldName)
			}).ToArray();
		}
		BarButtonItem CreateBarButtonItem(BarItemLinkCollection links, string name, ConditionalFormattingStringId id, bool beginGroup, ImageSource image, ICommand command, object commandParameter = null) {
			return builder.CreateBarButtonItem(links, name, Localize(id), beginGroup, image, command, commandParameter);
		}
		BarButtonItem CreateBarButtonItem(BarItemLinkCollection links, string name, string content, bool beginGroup, ImageSource image, ICommand command, object commandParameter = null) {
			return builder.CreateBarButtonItem(links, name, content, beginGroup, image, command, commandParameter);
		}
		BarSplitButtonItem CreateBarSplitButtonItem(BarItemLinkCollection links, string name, ConditionalFormattingStringId id, bool beginGroup, ImageSource image) {
			return builder.CreateBarSplitButtonItem(links, name, Localize(id), beginGroup, image);
		}
		BarSubItem CreateBarSubItem(string name, ConditionalFormattingStringId id, bool beginGroup, ImageSource image, ICommand command) {
			return builder.CreateBarSubItem(name, Localize(id), beginGroup, image, command);
		}
		BarSubItem CreateBarSubItem(BarItemLinkCollection links, string name, ConditionalFormattingStringId id, bool beginGroup, ImageSource image, ICommand command) {
			return builder.CreateBarSubItem(links, name, Localize(id), beginGroup, image, command);
		}
		string Localize(ConditionalFormattingStringId id) {
			return ConditionalFormattingLocalizer.GetString(id);
		}
		object FindResource(ConditionalFormattingThemeKeys key) {
			return ResourceOwner.FindResource(new ConditionalFormattingThemeKeyExtension { ResourceKey = key });
		}
		DependencyObject CreateColorScaleCondition(string formatName, string field) {
			return CreateCondition(formatName, field, new ColorScaleEditUnit());
		}
		DependencyObject CreateDataBarCondition(string formatName, string field) {
			return CreateCondition(formatName, field, new DataBarEditUnit());
		}
		DependencyObject CreateIconCondition(string formatName, string field) {
			return CreateCondition(formatName, field, new IconSetEditUnit());
		}
		DependencyObject CreateCondition(string formatName, string field, BaseEditUnit unit) {
			unit.PredefinedFormatName = formatName;
			unit.FieldName = field;
			return unit.BuildCondition(context.Builder).GetCurrentValue() as DependencyObject;
		}
	}
	public interface IConditionalFormattingDialogBuilder {
		BarButtonItem CreateBarButtonItem(BarItemLinkCollection links, string name, string content, bool beginGroup, ImageSource image, ICommand command, object commandParameter);
		BarSplitButtonItem CreateBarSplitButtonItem(BarItemLinkCollection links, string name, string content, bool beginGroup, ImageSource image);
		BarSubItem CreateBarSubItem(string name, string content, bool beginGroup, ImageSource image, ICommand command);
		BarSubItem CreateBarSubItem(BarItemLinkCollection links, string name, string content, bool beginGroup, ImageSource image, ICommand command);
	}
}
