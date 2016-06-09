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

using DevExpress.Utils.Localization;
using DevExpress.Web.Internal;
using DevExpress.Web.Localization;
namespace DevExpress.Web.Localization {
	public enum ASPxGridViewStringId {
		GroupPanel, EmptyDataRow, EmptyHeaders,
		ConfirmDelete, ConfirmOnLosingBatchChanges, GroupContinuedOnNextPage,
		CustomizationWindowCaption, PopupEditFormCaption,
		HeaderFilterShowAllItem, HeaderFilterShowBlanksItem, HeaderFilterShowNonBlanksItem,
		HeaderFilterSelectAll, HeaderFilterOkButton, HeaderFilterCancelButton,
		HeaderFilterYesterday, HeaderFilterToday, HeaderFilterTomorrow,
		HeaderFilterLastWeek, HeaderFilterThisWeek, HeaderFilterNextWeek,
		HeaderFilterLastMonth, HeaderFilterThisMonth, HeaderFilterNextMonth,
		HeaderFilterLastYear, HeaderFilterThisYear, HeaderFilterNextYear,
		SearchPanelEditorNullText,
		CommandEdit, CommandNew, CommandDelete, CommandSelect, CommandCancel,
		CommandUpdate, CommandShowAdaptiveDetail, CommandHideAdaptiveDetail, CommandClearFilter, CommandApplyFilter,
		CommandBatchEditUpdate, CommandBatchEditCancel,
		CommandSelectAllOnPage, CommandSelectAllOnAllPages, CommandDeselectAllOnPage, CommandDeselectAllOnAllPages,
		CommandApplySearchPanelFilter, CommandClearSearchPanelFilter,
		AutoFilterBeginsWith, AutoFilterContains, AutoFilterDoesNotContain, AutoFilterEndsWith,
		AutoFilterEquals, AutoFilterGreater, AutoFilterGreaterOrEqual, AutoFilterLess,
		AutoFilterLessOrEqual, AutoFilterNotEqual, AutoFilterLike, AutoFilterLikeToolTip,
		Alt_HeaderFilterButton, Alt_HeaderFilterButtonActive,
		Alt_SortedAscending, Alt_SortedDescending,
		Alt_DragAndDropHideColumnIcon,
		Alt_Expand, Alt_Collapse, Alt_FilterRowButton,
		Outlook_Older, Outlook_LastMonth, Outlook_EarlierThisMonth,
		Outlook_ThreeWeeksAgo, Outlook_TwoWeeksAgo, Outlook_LastWeek,
		Outlook_Yesterday, Outlook_Today, Outlook_Tomorrow,
		Outlook_NextWeek, Outlook_TwoWeeksAway, Outlook_ThreeWeeksAway,
		Outlook_LaterThisMonth, Outlook_NextMonth, Outlook_BeyondNextMonth,
		Summary_Sum, Summary_Sum_OtherColumn,
		Summary_Min, Summary_Min_OtherColumn,
		Summary_Max, Summary_Max_OtherColumn,
		Summary_Average, Summary_Average_OtherColumn,
		Summary_Count,
		ContextMenu_FullExpand, ContextMenu_FullCollapse,
		ContextMenu_SortAscending, ContextMenu_SortDescending, ContextMenu_ClearSorting,
		ContextMenu_ClearFilter, ContextMenu_ShowFilterEditor, ContextMenu_ShowFilterRow, ContextMenu_ShowFilterRowMenu,
		ContextMenu_GroupByColumn, ContextMenu_UngroupColumn, ContextMenu_ClearGrouping, ContextMenu_ShowGroupPanel, ContextMenu_ShowSearchPanel,
		ContextMenu_ShowColumn, ContextMenu_HideColumn, ContextMenu_ShowCustomizationWindow, ContextMenu_ShowFooter,
		ContextMenu_NewRow, ContextMenu_EditRow, ContextMenu_DeleteRow,
		ContextMenu_ExpandRow, ContextMenu_CollapseRow,
		ContextMenu_ExpandDetailRow, ContextMenu_CollapseDetailRow, ContextMenu_Refresh,
		ContextMenu_SummarySum, ContextMenu_SummaryMin, ContextMenu_SummaryMax, 
		ContextMenu_SummaryAverage, ContextMenu_SummaryCount, ContextMenu_SummaryNone,
		CardView_CustomizationWindowCaption, 
		CardView_EndlessPagingShowMoreCardsButton,
		CardView_PagerRowPerPage
	}
	public class ASPxGridViewResLocalizer : ASPxResLocalizerBase<ASPxGridViewStringId> {
		public ASPxGridViewResLocalizer(ASPxGridViewLocalizer localizer)
			: base(localizer) {
		}
		public ASPxGridViewResLocalizer()
			: base(new ASPxGridViewLocalizer()) {
		}
		protected override string GlobalResourceAssemblyName {
			get { return AssemblyInfo.SRAssemblyASPxGridView; }
		}
		protected override string ResxName {
			get { return "DevExpress.Web.Grid.LocalizationRes"; }
		}
	}
	public class ASPxGridViewLocalizer : XtraLocalizer<ASPxGridViewStringId> {
		static ASPxGridViewLocalizer() {
			ASPxActiveLocalizerProvider<ASPxGridViewStringId> provider = new ASPxActiveLocalizerProvider<ASPxGridViewStringId>(CreateResLocalizerInstance);
			SetActiveLocalizerProvider(provider);
		}
		static XtraLocalizer<ASPxGridViewStringId> CreateResLocalizerInstance() {
			return new ASPxGridViewResLocalizer();
		}
		public static string GetString(ASPxGridViewStringId id) {
			return Active.GetLocalizedString(id);
		}
		public override XtraLocalizer<ASPxGridViewStringId> CreateResXLocalizer() {
			return CreateResLocalizerInstance();
		}
		protected override void PopulateStringTable() {
			AddString(ASPxGridViewStringId.GroupPanel, StringResources.GridView_GroupPanel);
			AddString(ASPxGridViewStringId.EmptyDataRow, StringResources.GridView_EmptyDataRow);
			AddString(ASPxGridViewStringId.HeaderFilterShowAllItem, StringResources.GridView_HeaderFilterShowAllItem);
			AddString(ASPxGridViewStringId.HeaderFilterShowBlanksItem, StringResources.GridView_HeaderFilterShowBlanksItem);
			AddString(ASPxGridViewStringId.HeaderFilterShowNonBlanksItem, StringResources.GridView_HeaderFilterShowNonBlanksItem);
			AddString(ASPxGridViewStringId.EmptyHeaders, StringResources.GridView_EmptyHeaders);
			AddString(ASPxGridViewStringId.ConfirmDelete, StringResources.DataEditing_ConfirmDelete);
			AddString(ASPxGridViewStringId.ConfirmOnLosingBatchChanges, StringResources.GridView_ConfirmOnLosingBatchChanges);
			AddString(ASPxGridViewStringId.CustomizationWindowCaption, StringResources.CustomizationWindowCaption);
			AddString(ASPxGridViewStringId.PopupEditFormCaption, StringResources.PopupEditFormCaption);
			AddString(ASPxGridViewStringId.GroupContinuedOnNextPage, StringResources.GridView_GroupContinuedOnNextPage);
			AddString(ASPxGridViewStringId.HeaderFilterSelectAll, "(Select All)");
			AddString(ASPxGridViewStringId.HeaderFilterYesterday, "Yesterday");
			AddString(ASPxGridViewStringId.HeaderFilterToday, "Today");
			AddString(ASPxGridViewStringId.HeaderFilterTomorrow, "Tomorrow");
			AddString(ASPxGridViewStringId.HeaderFilterLastWeek, "Last Week");
			AddString(ASPxGridViewStringId.HeaderFilterThisWeek, "This Week");
			AddString(ASPxGridViewStringId.HeaderFilterNextWeek, "Next Week");
			AddString(ASPxGridViewStringId.HeaderFilterLastMonth, "Last Month");
			AddString(ASPxGridViewStringId.HeaderFilterThisMonth, "This Month");
			AddString(ASPxGridViewStringId.HeaderFilterNextMonth, "Next Month");
			AddString(ASPxGridViewStringId.HeaderFilterLastYear, "Last Year");
			AddString(ASPxGridViewStringId.HeaderFilterThisYear, "This Year");
			AddString(ASPxGridViewStringId.HeaderFilterNextYear, "Next Year");
			AddString(ASPxGridViewStringId.HeaderFilterOkButton, "OK");
			AddString(ASPxGridViewStringId.HeaderFilterCancelButton, "Cancel");
			AddString(ASPxGridViewStringId.SearchPanelEditorNullText, "Enter text to search...");
			AddString(ASPxGridViewStringId.CommandEdit, StringResources.DataEditing_CommandEdit);
			AddString(ASPxGridViewStringId.CommandNew, StringResources.DataEditing_CommandNew);
			AddString(ASPxGridViewStringId.CommandDelete, StringResources.DataEditing_CommandDelete);
			AddString(ASPxGridViewStringId.CommandSelect, StringResources.DataEditing_CommandSelect);
			AddString(ASPxGridViewStringId.CommandCancel, StringResources.DataEditing_CommandCancel);
			AddString(ASPxGridViewStringId.CommandUpdate, StringResources.DataEditing_CommandUpdate);
			AddString(ASPxGridViewStringId.CommandClearFilter, StringResources.GridView_CommandClearFilter);
			AddString(ASPxGridViewStringId.CommandApplyFilter, StringResources.GridView_CommandApplyFilter);
			AddString(ASPxGridViewStringId.CommandShowAdaptiveDetail, StringResources.GridView_CommandShowAdaptiveDetail);
			AddString(ASPxGridViewStringId.CommandHideAdaptiveDetail, StringResources.GridView_CommandHideAdaptiveDetail);
			AddString(ASPxGridViewStringId.CommandBatchEditUpdate, StringResources.GridView_CommandBatchEditUpdate);
			AddString(ASPxGridViewStringId.CommandBatchEditCancel, StringResources.GridView_CommandBatchEditCancel);
			AddString(ASPxGridViewStringId.CommandSelectAllOnPage, "Select all rows on this page");
			AddString(ASPxGridViewStringId.CommandSelectAllOnAllPages, "Select all rows on all pages");
			AddString(ASPxGridViewStringId.CommandDeselectAllOnPage, "Deselect all rows on this page");
			AddString(ASPxGridViewStringId.CommandDeselectAllOnAllPages, "Deselect all rows on all pages");
			AddString(ASPxGridViewStringId.CommandApplySearchPanelFilter, "Search");
			AddString(ASPxGridViewStringId.CommandClearSearchPanelFilter, "Clear");
			AddString(ASPxGridViewStringId.AutoFilterBeginsWith, StringResources.FilterMenu_BeginsWith);
			AddString(ASPxGridViewStringId.AutoFilterContains, StringResources.FilterMenu_Contains);
			AddString(ASPxGridViewStringId.AutoFilterDoesNotContain, StringResources.FilterMenu_DoesNotContain);
			AddString(ASPxGridViewStringId.AutoFilterEndsWith, StringResources.FilterMenu_EndsWith);
			AddString(ASPxGridViewStringId.AutoFilterEquals, StringResources.FilterMenu_Equals);
			AddString(ASPxGridViewStringId.AutoFilterGreater, StringResources.FilterMenu_Greater);
			AddString(ASPxGridViewStringId.AutoFilterGreaterOrEqual, StringResources.FilterMenu_GreaterOrEqual);
			AddString(ASPxGridViewStringId.AutoFilterLess, StringResources.FilterMenu_Less);
			AddString(ASPxGridViewStringId.AutoFilterLessOrEqual, StringResources.FilterMenu_LessOrEqual);
			AddString(ASPxGridViewStringId.AutoFilterNotEqual, StringResources.FilterMenu_NotEqual);
			AddString(ASPxGridViewStringId.AutoFilterLike, "Like ('%', '_')");
			AddString(ASPxGridViewStringId.AutoFilterLikeToolTip, "Two wildcard symbols are supported:\n '%' substitutes zero or more characters;\n '_' substitutes a single character.");
			AddString(ASPxGridViewStringId.Alt_HeaderFilterButton, StringResources.Alt_FilterButton);
			AddString(ASPxGridViewStringId.Alt_HeaderFilterButtonActive, StringResources.Alt_FilterButtonActive);
			AddString(ASPxGridViewStringId.Alt_SortedAscending, StringResources.Alt_SortedAscending);
			AddString(ASPxGridViewStringId.Alt_SortedDescending, StringResources.Alt_SortedDescending);
			AddString(ASPxGridViewStringId.Alt_DragAndDropHideColumnIcon, StringResources.Alt_DragAndDropHideColumnIcon);
			AddString(ASPxGridViewStringId.Alt_Expand, StringResources.Alt_ExpandButton);
			AddString(ASPxGridViewStringId.Alt_Collapse, StringResources.Alt_CollapseButton);
			AddString(ASPxGridViewStringId.Alt_FilterRowButton, StringResources.Alt_FilterRowButton);
			AddString(ASPxGridViewStringId.Outlook_Older, "Older");
			AddString(ASPxGridViewStringId.Outlook_LastMonth, "Last Month");
			AddString(ASPxGridViewStringId.Outlook_EarlierThisMonth, "Earlier this Month");
			AddString(ASPxGridViewStringId.Outlook_ThreeWeeksAgo, "Three Weeks Ago");
			AddString(ASPxGridViewStringId.Outlook_TwoWeeksAgo, "Two Weeks Ago");
			AddString(ASPxGridViewStringId.Outlook_LastWeek, "Last Week");
			AddString(ASPxGridViewStringId.Outlook_Yesterday, "Yesterday");
			AddString(ASPxGridViewStringId.Outlook_Today, "Today");
			AddString(ASPxGridViewStringId.Outlook_Tomorrow, "Tomorrow");
			AddString(ASPxGridViewStringId.Outlook_NextWeek, "Next Week");
			AddString(ASPxGridViewStringId.Outlook_TwoWeeksAway, "Two Weeks Away");
			AddString(ASPxGridViewStringId.Outlook_ThreeWeeksAway, "Three Weeks Away");
			AddString(ASPxGridViewStringId.Outlook_LaterThisMonth, "Later this Month");
			AddString(ASPxGridViewStringId.Outlook_NextMonth, "Next Month");
			AddString(ASPxGridViewStringId.Outlook_BeyondNextMonth, "Beyond Next Month");
			AddString(ASPxGridViewStringId.Summary_Sum, "Sum={0}");
			AddString(ASPxGridViewStringId.Summary_Sum_OtherColumn, "Sum of {1} is {0}");
			AddString(ASPxGridViewStringId.Summary_Min, "Min={0}");
			AddString(ASPxGridViewStringId.Summary_Min_OtherColumn, "Min of {1} is {0}");
			AddString(ASPxGridViewStringId.Summary_Max, "Max={0}");
			AddString(ASPxGridViewStringId.Summary_Max_OtherColumn, "Max of {1} is {0}");
			AddString(ASPxGridViewStringId.Summary_Average, "Avg={0}");
			AddString(ASPxGridViewStringId.Summary_Average_OtherColumn, "Avg of {1} is {0}");
			AddString(ASPxGridViewStringId.Summary_Count, "Count={0}");
			AddString(ASPxGridViewStringId.ContextMenu_FullExpand, "Expand All");
			AddString(ASPxGridViewStringId.ContextMenu_FullCollapse, "Collapse All");
			AddString(ASPxGridViewStringId.ContextMenu_SortAscending, "Sort Ascending");
			AddString(ASPxGridViewStringId.ContextMenu_SortDescending, "Sort Descending");
			AddString(ASPxGridViewStringId.ContextMenu_ClearSorting, "Clear Sorting");
			AddString(ASPxGridViewStringId.ContextMenu_ClearFilter, "Clear Filter");
			AddString(ASPxGridViewStringId.ContextMenu_ShowFilterEditor, "Filter Builder...");
			AddString(ASPxGridViewStringId.ContextMenu_ShowFilterRow, "Filter Row"); 
			AddString(ASPxGridViewStringId.ContextMenu_ShowFilterRowMenu, "Filter Row Menu"); 
			AddString(ASPxGridViewStringId.ContextMenu_GroupByColumn, "Group By This Column"); 
			AddString(ASPxGridViewStringId.ContextMenu_UngroupColumn, "Ungroup");
			AddString(ASPxGridViewStringId.ContextMenu_ClearGrouping, "Clear Grouping");
			AddString(ASPxGridViewStringId.ContextMenu_ShowGroupPanel, "Group Panel");
			AddString(ASPxGridViewStringId.ContextMenu_ShowSearchPanel, "Search Panel");
			AddString(ASPxGridViewStringId.ContextMenu_ShowColumn, "Show Column");
			AddString(ASPxGridViewStringId.ContextMenu_HideColumn, "Hide Column");
			AddString(ASPxGridViewStringId.ContextMenu_ShowFooter, "Footer");
			AddString(ASPxGridViewStringId.ContextMenu_ShowCustomizationWindow, "Column Chooser");
			AddString(ASPxGridViewStringId.ContextMenu_NewRow, "New");
			AddString(ASPxGridViewStringId.ContextMenu_EditRow, "Edit");
			AddString(ASPxGridViewStringId.ContextMenu_DeleteRow, "Delete");
			AddString(ASPxGridViewStringId.ContextMenu_ExpandRow, "Expand");
			AddString(ASPxGridViewStringId.ContextMenu_CollapseRow, "Collapse");
			AddString(ASPxGridViewStringId.ContextMenu_ExpandDetailRow, "Expand Detail");
			AddString(ASPxGridViewStringId.ContextMenu_CollapseDetailRow, "Collapse Detail");
			AddString(ASPxGridViewStringId.ContextMenu_Refresh, "Refresh");
			AddString(ASPxGridViewStringId.ContextMenu_SummarySum, "Sum");
			AddString(ASPxGridViewStringId.ContextMenu_SummaryMin, "Min");
			AddString(ASPxGridViewStringId.ContextMenu_SummaryMax, "Max");
			AddString(ASPxGridViewStringId.ContextMenu_SummaryAverage, "Average");
			AddString(ASPxGridViewStringId.ContextMenu_SummaryCount, "Count");
			AddString(ASPxGridViewStringId.ContextMenu_SummaryNone, "None");
			AddString(ASPxGridViewStringId.CardView_CustomizationWindowCaption, "Customization");
			AddString(ASPxGridViewStringId.CardView_EndlessPagingShowMoreCardsButton, "Show more cards...");
			AddString(ASPxGridViewStringId.CardView_PagerRowPerPage, "Rows per page:");
		}
	}
}
