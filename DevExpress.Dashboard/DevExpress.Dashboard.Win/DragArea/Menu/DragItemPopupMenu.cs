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
using System.ComponentModel;
using System.Drawing;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.Localization;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Controls;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public class DragItemPopupMenu : PopupMenu {
		public event EventHandler BeforeCommandExecute;
		public event EventHandler AfterCommandExecute;
		void OnBarItemClicked(object sender, ItemClickEventArgs e) {
			DataItemMenuItemCommand command = e.Item.Tag as DataItemMenuItemCommand;
			if(command != null && command.CanExecute) {
				if(BeforeCommandExecute != null)
					BeforeCommandExecute(this, new EventArgs());
				command.Execute();
				if(AfterCommandExecute != null)
					AfterCommandExecute(this, new EventArgs());
			}
		}
		bool IsBasicDiscreteDateTimeGroupInterval(DateTimeGroupInterval groupInterval) {
			return groupInterval == DateTimeGroupInterval.Year ||
				groupInterval == DateTimeGroupInterval.Quarter ||
				groupInterval == DateTimeGroupInterval.Month ||
				groupInterval == DateTimeGroupInterval.Day;
		}
		bool IsBasicContinuousDateTimeGroupInterval(DateTimeGroupInterval groupInterval) {
			return groupInterval == DateTimeGroupInterval.Year ||
				groupInterval == DateTimeGroupInterval.None ||
				groupInterval == DateTimeGroupInterval.MonthYear ||
				groupInterval == DateTimeGroupInterval.QuarterYear ||
				groupInterval == DateTimeGroupInterval.DayMonthYear;
		}
		bool IsBasicSummaryType(SummaryType summaryType) {
			return summaryType == SummaryType.Sum ||
				summaryType == SummaryType.Count ||
				summaryType == SummaryType.CountDistinct ||
				summaryType == SummaryType.Min ||
				summaryType == SummaryType.Max ||
				summaryType == SummaryType.Average;
		}
		readonly DragAreaControl dragArea;
		readonly BarManager barManager;
		DragItem dragItem;
		public DragItemPopupMenu(DragAreaControl dragArea, BarManager barManager)
			: base(barManager) {
			this.dragArea = dragArea;
			this.barManager = barManager;
		}
		void AddSummaryTypeItem(BarItemLinkCollection itemLinks, Measure measure, SummaryType summaryType) {
			AddBarCheckItem(itemLinks, new SummaryTypeMenuCommand(dragArea.Designer, dragArea.DashboardItem, measure, summaryType), false);
		}
		void AddBarCheckItem(BarItemLinkCollection itemLinks, DataItemMenuItemCommand command, bool addSeparator) {
			AddBarCheckItem(itemLinks, command, addSeparator, null);
		}
		void AddBarCheckItem(BarItemLinkCollection itemLinks, DataItemMenuItemCommand command, bool addSeparator, Image glyph) {
			BarCheckItem barCheckItem = new BarCheckItem(barManager, command.Checked);
			barCheckItem.Tag = command;
			barCheckItem.Caption = command.Caption;
			barCheckItem.Enabled = command.CanExecute;
			barCheckItem.Glyph = glyph;
			barCheckItem.CheckedChanged += OnBarItemClicked;
			itemLinks.Add(barCheckItem).BeginGroup = addSeparator;
		}
		BarItemLinkCollection AddBarSubItem(BarItemLinkCollection itemLinks, string caption, bool addSeparator, bool enabled) {
			return AddBarSubItem(itemLinks, caption, addSeparator, enabled, null);
		}
		BarItemLinkCollection AddBarSubItem(BarItemLinkCollection itemLinks, string caption, bool addSeparator, bool enabled, Image glyph) {
			BarSubItem barSubItem = new BarSubItem(barManager, caption);
			barSubItem.Enabled = enabled;
			barSubItem.Glyph = glyph;
			itemLinks.Add(barSubItem).BeginGroup = addSeparator;
			return barSubItem.ItemLinks;
		}
		void AddBarButtonItem(BarItemLinkCollection itemLinks, DataItemMenuItemCommand command, bool addSeparator) {
			AddBarButtonItem(itemLinks, command, addSeparator, null);
		}
		void AddBarButtonItem(BarItemLinkCollection itemLinks, DataItemMenuItemCommand command, bool addSeparator, Image glyph) {
			BarButtonItem barButtonItem = new BarButtonItem(barManager, command.Caption);
			barButtonItem.Tag = command;
			barButtonItem.Glyph = glyph;
			barButtonItem.Enabled = command.CanExecute;
			barButtonItem.ItemClick += OnBarItemClicked;
			itemLinks.Add(barButtonItem).BeginGroup = addSeparator;
		}
		BarItemLinkCollection AddMoreSubItem(BarItemLinkCollection itemLinks, string suffix) {
			string caption = DashboardWinLocalizer.GetString(DashboardWinStringId.MenuMore);
			if(!String.IsNullOrEmpty(suffix))
				caption += String.Format(" ({0})", suffix);
			return AddBarSubItem(itemLinks, caption, false, true);
		}
		BarItemLinkCollection AddColorBySubItem(BarItemLinkCollection itemLinks) {
			string caption = DashboardWinLocalizer.GetString(DashboardWinStringId.MenuColorBy);
			return AddBarSubItem(itemLinks, caption, true, true);
		}
		void FillTextGroupIntervalSubItems(BarItemLinkCollection itemLinks, Dimension dimension, IList<TextGroupInterval> textGroupIntervals) {
			bool insertSeparator = true;
			foreach(TextGroupInterval groupInterval in textGroupIntervals) {
				AddBarCheckItem(itemLinks, new TextGroupIntervalMenuItemCommand(dragArea.Designer, dragArea.DashboardItem, dimension, groupInterval), insertSeparator);
				insertSeparator = false;
			}
		}
		void FillDateTimeGroupIntervalSubItems(BarItemLinkCollection itemLinks, Dimension dimension, IEnumerable<DateTimeGroupInterval> dateTimeGroupIntervals, bool isDiscreteGroupIntervals) {
			bool insertSeparator = true;
			List<DateTimeGroupInterval> additionalGroupIntervals = new List<DateTimeGroupInterval>();
			foreach(DateTimeGroupInterval groupInterval in dateTimeGroupIntervals) {
				bool isBasicGroupInterval = isDiscreteGroupIntervals ? IsBasicDiscreteDateTimeGroupInterval(groupInterval) : IsBasicContinuousDateTimeGroupInterval(groupInterval);
				if(isBasicGroupInterval) {
					AddBarCheckItem(itemLinks, new DateTimeGroupIntervalMenuItemCommand(dragArea.Designer, dragArea.DashboardItem, dimension, groupInterval), insertSeparator);
					insertSeparator = false;
				}
				else
					additionalGroupIntervals.Add(groupInterval);
			}
			string moreSuffix = additionalGroupIntervals.Contains(dimension.DateTimeGroupInterval) ?
				GroupIntervalCaptionProvider.GetDateTimeGroupIntervalCaption(dimension.DateTimeGroupInterval) : null;
			BarItemLinkCollection moreItemLinks = AddMoreSubItem(itemLinks, moreSuffix);
			foreach(DateTimeGroupInterval groupInterval in additionalGroupIntervals)
				AddBarCheckItem(moreItemLinks, new DateTimeGroupIntervalMenuItemCommand(dragArea.Designer, dragArea.DashboardItem, dimension, groupInterval), false);
		}
		void FillGroupIntervalSubItems(BarItemLinkCollection itemLinks, Dimension dimension) {
			DimensionGroupIntervalInfo groupIntervalInfo = dimension.GetDimensionGroupIntervalInfo();
			if(groupIntervalInfo != null) {
				if(groupIntervalInfo.TextGroupIntervals != null)
					FillTextGroupIntervalSubItems(itemLinks, dimension, groupIntervalInfo.TextGroupIntervals);
				if(groupIntervalInfo.DateTimeDiscreteGroupIntervals != null)
					FillDateTimeGroupIntervalSubItems(itemLinks, dimension, groupIntervalInfo.DateTimeDiscreteGroupIntervals, true);
				if(groupIntervalInfo.DateTimeContinuousGroupIntervalsButExactDate != null)
					FillDateTimeGroupIntervalSubItems(itemLinks, dimension, groupIntervalInfo.DateTimeContinuousGroupIntervalsButExactDate, false);
				if(groupIntervalInfo.IsSupportExactDateGroupInterval)
					AddBarCheckItem(itemLinks, new DateTimeGroupIntervalMenuItemCommand(dragArea.Designer, dragArea.DashboardItem, dimension, DateTimeGroupInterval.None), true);
				if(groupIntervalInfo.IsSupportNumericGroupIntervals) {
					AddBarCheckItem(itemLinks, new NumericGroupIntervalMenuItemCommand(dragArea.Designer, dragArea.DashboardItem, dimension, true), true);
					AddBarCheckItem(itemLinks, new NumericGroupIntervalMenuItemCommand(dragArea.Designer, dragArea.DashboardItem, dimension, false), false);
				}
			}
		}
		void FillSortModeSubItems(BarItemLinkCollection itemLinks, Dimension dimension) {
			foreach(DimensionSortMode sortMode in dimension.SortModesAvailable) {
				AddBarCheckItem(itemLinks, new SortModeMenuItemCommand(dragArea.Designer, dragArea.DashboardItem, dimension, sortMode), false);
			}
		}
		BarItemLinkCollection AddFormatBarSubItem(BarItemLinkCollection itemLinks, string suffix) {
			string caption = String.Format("{0} ({1})", DashboardWinLocalizer.GetString(DashboardWinStringId.MenuDateTimeFormatFormat), suffix);
			return AddBarSubItem(itemLinks, caption, false, true);
		}
		void AddDimensionDateTimeFormatMenuItems(BarItemLinkCollection itemLinks, DashboardDesigner designer, DataDashboardItem dashboardItem, Dimension dimension) {
			BarItemLinkCollection formatLinks = AddFormatBarSubItem(itemLinks, DateTimeFormatCaptionProvider.GetDimensionDateTimeFormatCaption(dimension));
			if(dimension.DateTimeGroupInterval == DateTimeGroupInterval.None) {
				AddExactDateFormatMenuItems(formatLinks, designer, dashboardItem, dimension);
			}
			else {
				AddNonExactDateFormatMenuItems(formatLinks, designer, dashboardItem, dimension);
			}
		}
		void AddExactDateFormatMenuItems(BarItemLinkCollection itmeLinks, DashboardDesigner designer, DataDashboardItem dashboardItem, Dimension dimension) {
			foreach(ExactDateFormat exactDateFormat in Enum.GetValues(typeof(ExactDateFormat))) {
				ExactDateFormatMenuItemCommand exactDateCommand = DateTimeFormatCommandGenerator.CreateExactDateFormatCommand(designer, dashboardItem, dimension, exactDateFormat);
				if(exactDateCommand != null) {
					AddBarCheckItem(itmeLinks, exactDateCommand, false);
				}
				else {
					BarItemLinkCollection exactDateLinks = AddBarSubItem(itmeLinks, DateTimeFormatCaptionProvider.GetCaption(exactDateFormat), false, true);
					foreach(DataItemDateTimeFormatMenuItemCommand command in DateTimeFormatCommandGenerator.CreateExactDateFormatSubcommands(designer, dashboardItem, dimension, exactDateFormat)) {
						AddBarCheckItem(exactDateLinks, command, false);
					}
				}
			}
		}
		void AddNonExactDateFormatMenuItems(BarItemLinkCollection itemLinks, DashboardDesigner designer, DataDashboardItem dashboardItem, Dimension dimension) {
			foreach(DataItemDateTimeFormatMenuItemCommand command in DateTimeFormatCommandGenerator.CreateNonExactDateFormatCommands(designer, dashboardItem, dimension)) {
				AddBarCheckItem(itemLinks, command, false);
			}
		}
		void AddMeasureDateTimeFormatMenuItems(BarItemLinkCollection itemLinks, DashboardDesigner designer, DataDashboardItem dashboardItem, Measure measure) {
			BarItemLinkCollection formatLinks = AddFormatBarSubItem(itemLinks, DateTimeFormatCaptionProvider.GetMeasureDateTimeFormatCaption(measure));
			foreach(DataItemDateTimeFormatMenuItemCommand command in DateTimeFormatCommandGenerator.CreateMeasureCommands(designer, dashboardItem, measure)) {
				AddBarCheckItem(formatLinks, command, false);
			}
		}
		void AddMeasureColoringModeMenuItems(BarItemLinkCollection itemLinks, DashboardDesigner designer, DataDashboardItem dashboardItem, Measure measure) {
			BarItemLinkCollection colorByLinks = AddColorBySubItem(itemLinks);
			foreach(ColoringMode coloringMode in Enum.GetValues(typeof(ColoringMode))) {
				AddBarCheckItem(colorByLinks, new MeasureColoringModeMenuItemCommand(dragArea.Designer, dashboardItem, measure, coloringMode), false);
			}
		}
		void AddDimensionColoringModeMenuItems(BarItemLinkCollection itemLinks, DashboardDesigner designer, DataDashboardItem dashboardItem, Dimension dimension) {
			BarItemLinkCollection colorByLinks = AddColorBySubItem(itemLinks);
			foreach(ColoringMode coloringMode in Enum.GetValues(typeof(ColoringMode))) {
				AddBarCheckItem(colorByLinks, new DimensionColoringModeMenuItemCommand(dragArea.Designer, dashboardItem, dimension, coloringMode), false);
			}
		}
		void FillDimensionItems(BarItemLinkCollection itemLinks, Dimension dimension) {
			DashboardDesigner designer = dragArea.Designer;
			DataDashboardItem dashboardItem = dragArea.DashboardItem;
			if(dashboardItem.IsSortingEnabled(dimension)) {
				AddBarCheckItem(itemLinks,
					new SortOrderMenuItemCommand(designer, dashboardItem, dimension, DimensionSortOrder.Ascending), false, ImageHelper.GetImage("SortAscending"));
				AddBarCheckItem(itemLinks,
					new SortOrderMenuItemCommand(designer, dashboardItem, dimension, DimensionSortOrder.Descending), false, ImageHelper.GetImage("SortDescending"));
				if(dimension.IsSortOrderNoneEnabled)
					AddBarCheckItem(itemLinks,
						new SortOrderMenuItemCommand(designer, dashboardItem, dimension, DimensionSortOrder.None), false);
				bool isSortByMeasureEnabled = dashboardItem.IsSortingByMeasureEnabled(dimension, designer.Viewer.SelectedLayoutItem.ItemViewer.ViewModel); 
				bool canSpecifySortMode = dashboardItem.CanSpecifySortMode(dimension);
				bool isSortByOptionsEnabled = isSortByMeasureEnabled || canSpecifySortMode;
				BarItemLinkCollection sortByItemLinks = AddBarSubItem(itemLinks, DashboardWinLocalizer.GetString(DashboardWinStringId.MenuDimensionSortBy), false, isSortByOptionsEnabled);
				if(canSpecifySortMode) {
					FillSortModeSubItems(sortByItemLinks, dimension);
				}
				if(isSortByMeasureEnabled) {
					List<SortByMeasureMenuItemCommand> sortByMeasureCommands = new List<SortByMeasureMenuItemCommand>();
					foreach(Measure measure in dashboardItem.UniqueMeasures)
						sortByMeasureCommands.Add(new SortByMeasureMenuItemCommand(designer, dashboardItem, dimension, measure));
					sortByMeasureCommands.Sort();
					foreach(SortByMeasureMenuItemCommand command in sortByMeasureCommands)
						AddBarCheckItem(sortByItemLinks, command, false);
				}
			}
			if(dashboardItem.CanSpecifyTopNOptions(dimension))
				AddBarCheckItem(itemLinks, new TopNMenuItemCommand(designer, dashboardItem, dimension), false);
			FillGroupIntervalSubItems(itemLinks, dimension);
			if(dashboardItem.CanSpecifyDimensionNumericFormat(dimension))
				AddBarButtonItem(itemLinks, new NumericFormatMenuItemCommand(designer, dashboardItem, dimension), false);
			if(dashboardItem.CanSpecifyDimensionDateTimeFormat(dimension)) {
				AddDimensionDateTimeFormatMenuItems(itemLinks, designer, dashboardItem, dimension);
			}
			if(dashboardItem.CanColorByDimension(dimension)) {
				AddDimensionColoringModeMenuItems(itemLinks, dragArea.Designer, dashboardItem, dimension);
			}
			FillConditionalFormattingItems(itemLinks, dimension, designer, dashboardItem);
			AddBarButtonItem(itemLinks, new RenameDataItemMenuItemCommand(designer, dashboardItem, dimension), true);
		}
		void FillConditionalFormattingItems(BarItemLinkCollection itemLinks, DataItem dataItem, DashboardDesigner designer, DataDashboardItem dashboardItem) {
			if(dashboardItem.FormatRulesInternal == null || !dashboardItem.IsConditionalFormattingCalculateByAllowed(dataItem))
				return;
			IList<BarItem> items = FormatRuleControlBarItemsCreator.CreateFormatRuleControlItems(Manager, designer, dashboardItem, dataItem, OnBarItemClicked);
			for(int i = 0; i < items.Count; i++) {
				BarItemLink link = itemLinks.Add(items[i]);
				if(i == 0)
					link.BeginGroup = true;
			}
		}
		void FillMeasureItemLinks(BarItemLinkCollection itemLinks, Measure measure) {
			List<SummaryType> additionalSummaryTypes = new List<SummaryType>();
			DataDashboardItem dashboardItem = dragArea.DashboardItem;
			IList<SummaryType> summaryTypes = measure.GetSupportedSummaryTypes();
			if(summaryTypes != null) {
				foreach(SummaryType summaryType in summaryTypes)
					if(IsBasicSummaryType(summaryType))
						AddSummaryTypeItem(itemLinks, measure, summaryType);
					else
						additionalSummaryTypes.Add(summaryType);
				if(additionalSummaryTypes.Count > 0) {
					SummaryType measureSummaryType = measure.SummaryType;
					BarItemLinkCollection moreLinks = AddMoreSubItem(itemLinks, IsBasicSummaryType(measureSummaryType) ? null : Measure.GetSummaryTypeCaption(measureSummaryType));
					foreach(SummaryType summaryType in additionalSummaryTypes)
						AddSummaryTypeItem(moreLinks, measure, summaryType);
				}
			}
			if(dashboardItem.CanSpecifyMeasureNumericFormat(measure))
				AddBarButtonItem(itemLinks, new NumericFormatMenuItemCommand(dragArea.Designer, dashboardItem, measure), true);
			if(dashboardItem.CanSpecifyMeasureDateTimeFormat(measure)) {
				AddMeasureDateTimeFormatMenuItems(itemLinks, dragArea.Designer, dashboardItem, measure);
			}
			if(dashboardItem.CanColorByMeasure(measure)) {
				AddMeasureColoringModeMenuItems(itemLinks, dragArea.Designer, dashboardItem, measure);
			}
			FillConditionalFormattingItems(itemLinks, measure, dragArea.Designer, dashboardItem);
			AddBarButtonItem(itemLinks, new RenameDataItemMenuItemCommand(dragArea.Designer, dashboardItem, measure), true);
		}
		BarItemLinkCollection GetItemLinks(DragItem dragItem) {
			BarItemLinkCollection itemLinks = new BarItemLinkCollection();
			if(dragItem.DataItems.Count > 1) {
				foreach(Dimension hierarchyDimension in dragItem.DataItems) {
					if(!dragArea.DashboardItem.IsDimensionHidden(hierarchyDimension)) {
						Image glyph = dragArea.DashboardItem.IsColoringEnabled(hierarchyDimension) ? ImageHelper.GetImage("DragItem.Colorization") : null;
						BarItemLinkCollection hierarchySubItem = AddBarSubItem(itemLinks, hierarchyDimension.DisplayName, false, true, glyph);
						FillDimensionItems(hierarchySubItem, hierarchyDimension);
					}
				}
			}
			else {
				DataItem dataItem = dragItem.DataItem;
				Dimension dimension = dataItem as Dimension;
				if(dimension != null)
					FillDimensionItems(itemLinks, dimension);
				else {
					Measure measure = dataItem as Measure;
					if(measure != null)
						FillMeasureItemLinks(itemLinks, measure);
				}
			}
			return itemLinks;
		}
		protected override void OnCloseUp(CustomPopupBarControl prevControl) {
			base.OnCloseUp(prevControl);
			dragItem.SetPopupButtonState(DragAreaButtonState.Normal);
			dragArea.ForceUpdateSelection();
		}
		public bool HasPopupMenu(DragItem dragItem) {
			bool isHiddenDimensionHierarchy = dragItem.DataItems.Count > 1 && dragArea.DashboardItem.IsDimensionHidden((Dimension)dragItem.DataItems[0]);
			if(isHiddenDimensionHierarchy)
				return false;
			return true;
		}
		public bool Show(DragItem dragItem, Point location) {
			BarItemLinkCollection newItemLinks = GetItemLinks(dragItem);
			if(newItemLinks.Count == 0)
				return false;
			this.dragItem = dragItem;
			ItemLinks.Assign(newItemLinks);
			ShowPopup(location);
			return true;
		}
	}
}
