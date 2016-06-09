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
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardWin.ServiceModel;
using DevExpress.LookAndFeel;
using DevExpress.XtraBars;
namespace DevExpress.DashboardWin.Native {
	public static class FormatRuleControlBarItemsCreator {
		static BarButtonItem CreateCommandBarItem(BarManager manager, DataItemMenuItemCommand command, Image image, bool enabled, ItemClickEventHandler clickEventHandler) {
			BarButtonItem item = new BarButtonItem() {
				Manager = manager,
				Tag = command,
				Caption = command.Caption,
				Glyph = image,
				Enabled = command.CanExecute
			};
			item.ItemClick += clickEventHandler;
			return item;
		}
		static BarSubItem CreateBarSubItem(BarManager manager, Image image, string caption) {
			return new BarSubItem() { Manager = manager, Glyph = image, Caption = caption };
		}
		public static void OnBarItemClicked(object sender, ItemClickEventArgs e) {
			DataItemMenuItemCommand command = e.Item.Tag as DataItemMenuItemCommand;
			if(command != null && command.CanExecute)
				command.Execute();
		}
		public static IList<BarItem> CreateFormatRuleControlItems(BarManager manager, DashboardDesigner designer, DataDashboardItem dashboardItem, DataItem dataItem) {
			return CreateFormatRuleControlItems(manager, designer, dashboardItem, dataItem, OnBarItemClicked);
		}
		public static IList<BarItem> CreateFormatRuleControlItems(BarManager manager, DashboardDesigner designer, DataDashboardItem dashboardItem, DataItem dataItem, ItemClickEventHandler clickEventHandler) {
			List<BarItem> items = new List<BarItem>();
			BarSubItem addRuleItem = CreateBarSubItem(manager, null, DashboardWinLocalizer.GetString(DashboardWinStringId.MenuFormatRulesAdd));
			items.Add(addRuleItem);
			foreach(BarItem item in CreateValueFormatRuleItemContent(manager, designer, dashboardItem, dataItem, clickEventHandler))
				addRuleItem.ItemLinks.Add(item);
			IList<BarItem> rangeFormatRuleBarItems = CreateRangeFormatRuleItemContent(manager, designer, dashboardItem, dataItem, clickEventHandler);
			for(int i = 0; i < rangeFormatRuleBarItems.Count; i++) {
				BarItemLink link = addRuleItem.ItemLinks.Add(rangeFormatRuleBarItems[i]);
				if(i == 0)
					link.BeginGroup = true;
			}
			IList<BarItem> barFormatRuleBarItems = CreateBarFormatRuleItemContent(manager, designer, dashboardItem, dataItem, clickEventHandler);
			for(int i = 0; i < barFormatRuleBarItems.Count; i++) {
				BarItemLink link = addRuleItem.ItemLinks.Add(barFormatRuleBarItems[i]);
				if(i == 0)
					link.BeginGroup = true;
			}
			FormatRuleManagerRulesMenuItemCommand editRuleItemCommand = new FormatRuleManagerRulesMenuItemCommand(designer, dashboardItem, dataItem);
			items.Add(CreateCommandBarItem(manager, editRuleItemCommand, ImageHelper.GetEditorsMenuImage("ManageRules_16x16.png"), false, clickEventHandler));
			ClearDataItemFormatConditionRulesMenuItemCommand clearRulesItemCommand = new ClearDataItemFormatConditionRulesMenuItemCommand(designer, dashboardItem, dataItem);
			items.Add(CreateCommandBarItem(manager, clearRulesItemCommand, ImageHelper.GetEditorsMenuImage("ClearRules_16x16.png"), false, clickEventHandler));
			return items;
		}
		public static IList<BarItem> CreateValueFormatRuleItemContent(BarManager manager, IServiceProvider provider, DataDashboardItem dashboardItem, DataItem dataItem) {
			return CreateValueFormatRuleItemContent(manager, provider, dashboardItem, dataItem, OnBarItemClicked);
		}
		public static IList<BarItem> CreateRangeFormatRuleItemContent(BarManager manager, IServiceProvider provider, DataDashboardItem dashboardItem, DataItem dataItem) {
			return CreateRangeFormatRuleItemContent(manager, provider, dashboardItem, dataItem, OnBarItemClicked);
		}
		public static IList<BarItem> CreateBarFormatRuleItemContent(BarManager manager, IServiceProvider provider, DataDashboardItem dashboardItem, DataItem dataItem) {
			return CreateBarFormatRuleItemContent(manager, provider, dashboardItem, dataItem, OnBarItemClicked);
		}
		public static IList<BarItem> CreateValueFormatRuleItemContent(BarManager manager, IServiceProvider provider, DataDashboardItem dashboardItem, DataItem dataItem, ItemClickEventHandler clickEventHandler) {
			List<BarItem> items = new List<BarItem>();
			DataFieldType actualDataFieldType = dataItem.ActualDataFieldType;
			BarSubItem valueItem = CreateBarSubItem(manager, ImageHelper.GetImage("ConditionalFormatting.Value"), DashboardWinLocalizer.GetString(DashboardWinStringId.CommandFormatRuleValue));
			EnumManager.Iterate<DashboardFormatCondition>((type) => {
				if(actualDataFieldType != DataFieldType.Text || (actualDataFieldType == DataFieldType.Text && (type == DashboardFormatCondition.ContainsText || type == DashboardFormatCondition.Equal || type == DashboardFormatCondition.NotEqual))) {
					FormatRuleValueMenuItemCommand valueConditionItemCommand = new FormatRuleValueMenuItemCommand(provider, dashboardItem, dataItem, type);
					valueItem.AddItem(CreateCommandBarItem(manager, valueConditionItemCommand, type.Icon(), true, clickEventHandler));
				}
			});
			items.Add(valueItem);
			if(actualDataFieldType != DataFieldType.Text) {
				if(!(dataItem is Dimension || actualDataFieldType == DataFieldType.DateTime)) {
					BarSubItem topBottomItem = CreateBarSubItem(manager, ImageHelper.GetImage("ConditionalFormatting.TopBottom"), DashboardWinLocalizer.GetString(DashboardWinStringId.CommandFormatRuleTopBottom));
					EnumManager.Iterate<DashboardFormatConditionTopBottomType>((type) => {
						FormatRuleTopBottomMenuItemCommand topBottomConditionItemCommand = new FormatRuleTopBottomMenuItemCommand(provider, dashboardItem, dataItem, type);
						topBottomItem.AddItem(CreateCommandBarItem(manager, topBottomConditionItemCommand, type.Icon(), true, clickEventHandler));
					});
					items.Add(topBottomItem);
					BarSubItem avgItem = CreateBarSubItem(manager, ImageHelper.GetImage("ConditionalFormatting.Average"), DashboardWinLocalizer.GetString(DashboardWinStringId.CommandFormatRuleAboveBelowAverage));
					EnumManager.Iterate<DashboardFormatConditionAboveBelowType>((type) => {
						FormatRuleAboveBelowAverageMenuItemCommand avgConditionItemCommand = new FormatRuleAboveBelowAverageMenuItemCommand(provider, dashboardItem, dataItem, type);
						avgItem.AddItem(CreateCommandBarItem(manager, avgConditionItemCommand, type.Icon(), true, clickEventHandler));
					});
					items.Add(avgItem);
				}
				if(actualDataFieldType == DataFieldType.DateTime) {
					FormatRuleDateOccurringMenuItemCommand dateTimeItemCommand = new FormatRuleDateOccurringMenuItemCommand(provider, dashboardItem, dataItem);
					items.Add(CreateCommandBarItem(manager, dateTimeItemCommand, ImageHelper.GetImage("ConditionalFormatting.DateOccurring"), true, clickEventHandler));
				}
			}
			FormatRuleExpressionMenuItemCommand expressionItemCommand = new FormatRuleExpressionMenuItemCommand(provider, dashboardItem, dataItem);
			items.Add(CreateCommandBarItem(manager, expressionItemCommand, ImageHelper.GetImage("ConditionalFormatting.Expression"), true, clickEventHandler));
			return items;
		}
		public static IList<BarItem> CreateRangeFormatRuleItemContent(BarManager manager, IServiceProvider provider, DataDashboardItem dashboardItem, DataItem dataItem, ItemClickEventHandler clickEventHandler) {
			List<BarItem> items = new List<BarItem>();
			DataFieldType actualDataFieldType = dataItem.ActualDataFieldType;
			UserLookAndFeel lookAndFeel = provider.RequestServiceStrictly<IDashboardGuiContextService>().LookAndFeel;
			if(actualDataFieldType != DataFieldType.Text) {
				BarSubItem iconItem = CreateBarSubItem(manager, ImageHelper.GetImage("ConditionalFormatting.RangeIcons"), DashboardWinLocalizer.GetString(DashboardWinStringId.CommandFormatRuleRangeIcons));
				iconItem.ItemLinks.AddRange(new RangeIconSetFormatRuleMenuCreator(lookAndFeel).Initialize((rangeIconType, barItem) => {
					barItem.Tag = new FormatRuleRangeSetMenuItemCommand(provider, dashboardItem, dataItem, rangeIconType);
					clickEventHandler.Invoke(barItem, new ItemClickEventArgs(barItem, null));
				}));
				items.Add(iconItem);
				BarSubItem colorItem = CreateBarSubItem(manager, ImageHelper.GetImage("ConditionalFormatting.RangeColors"), DashboardWinLocalizer.GetString(DashboardWinStringId.CommandFormatRuleRangeColors));
				colorItem.ItemLinks.AddRange(new RangeColorSetFormatRuleMenuCreator(lookAndFeel).Initialize((rangeIconType, barItem) => {
					barItem.Tag = new FormatRuleRangeSetMenuItemCommand(provider, dashboardItem, dataItem, rangeIconType);
					clickEventHandler.Invoke(barItem, new ItemClickEventArgs(barItem, null));
				}));
				items.Add(colorItem);
				BarSubItem gradientItem = CreateBarSubItem(manager, ImageHelper.GetImage("ConditionalFormatting.RangeGradients"), DashboardWinLocalizer.GetString(DashboardWinStringId.CommandFormatRuleRangeGradient));
				gradientItem.ItemLinks.AddRange(new RangeGradientFormatRuleMenuCreator(lookAndFeel).Initialize((rangeGradientType, barItem) => {
					barItem.Tag = new FormatRuleRangeGradientMenuItemCommand(provider, dashboardItem, dataItem, rangeGradientType);
					clickEventHandler.Invoke(barItem, new ItemClickEventArgs(barItem, null));
				}));
				items.Add(gradientItem);
			}
			return items;
		}
		public static IList<BarItem> CreateBarFormatRuleItemContent(BarManager manager, IServiceProvider provider, DataDashboardItem dashboardItem, DataItem dataItem, ItemClickEventHandler clickEventHandler) {
			List<BarItem> items = new List<BarItem>();
			DataFieldType actualDataFieldType = dataItem.ActualDataFieldType;
			UserLookAndFeel lookAndFeel = provider.RequestServiceStrictly<IDashboardGuiContextService>().LookAndFeel;
			if(actualDataFieldType != DataFieldType.Text && dashboardItem.IsBarConditionalFormattingCalculateAllowed(dataItem)) {
				FormatRuleBarMenuItemCommand barItemCommand = new FormatRuleBarMenuItemCommand(provider, dashboardItem, dataItem);
				items.Add(CreateCommandBarItem(manager, barItemCommand, ImageHelper.GetImage("ConditionalFormatting.Bar"), true, clickEventHandler));
				BarSubItem colorBarItem = CreateBarSubItem(manager, ImageHelper.GetImage("ConditionalFormatting.ColorRangeBar"), DashboardWinLocalizer.GetString(DashboardWinStringId.CommandFormatRuleBarRangeColors));
				colorBarItem.ItemLinks.AddRange(new RangeColorSetFormatRuleMenuCreator(lookAndFeel).Initialize((rangeIconType, barItem) => {
					barItem.Tag = new FormatRuleColorRangeBarMenuItemCommand(provider, dashboardItem, dataItem, rangeIconType);
					clickEventHandler.Invoke(barItem, new ItemClickEventArgs(barItem, null));
				}));
				items.Add(colorBarItem);
				BarSubItem gradientBarItem = CreateBarSubItem(manager, ImageHelper.GetImage("ConditionalFormatting.GradientRangeBar"), DashboardWinLocalizer.GetString(DashboardWinStringId.CommandFormatRuleGradientRangeBar));
				gradientBarItem.ItemLinks.AddRange(new RangeGradientFormatRuleMenuCreator(lookAndFeel).Initialize((rangeGradientType, barItem) => {
					barItem.Tag = new FormatRuleGradientRangeBarMenuItemCommand(provider, dashboardItem, dataItem, rangeGradientType);
					clickEventHandler.Invoke(barItem, new ItemClickEventArgs(barItem, null));
				}));
				items.Add(gradientBarItem);
			}
			return items;
		}
	}
}
