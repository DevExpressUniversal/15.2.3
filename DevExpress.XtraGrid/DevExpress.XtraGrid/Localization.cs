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
using System.Data;
using System.Collections;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Resources;
using System.Globalization;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
namespace DevExpress.XtraGrid.Localization {
	[ToolboxItem(false)]
	public class GridLocalizer : XtraLocalizer<GridStringId> {
		static GridLocalizer() {
			SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<GridStringId>(CreateDefaultLocalizer()));
		}
		public new static XtraLocalizer<GridStringId> Active { 
			get { return XtraLocalizer<GridStringId>.Active; }
			set { XtraLocalizer<GridStringId>.Active = value; }
		}
		public override XtraLocalizer<GridStringId> CreateResXLocalizer() {
			return new GridResLocalizer();
		}
		public static XtraLocalizer<GridStringId> CreateDefaultLocalizer() { return new GridResLocalizer(); }
		#region PopulateStringTable
		protected override void PopulateStringTable() {
			AddString(GridStringId.FileIsNotFoundError, "File {0} is not found");
			AddString(GridStringId.FilterPanelCustomizeButton, "Edit Filter");
			AddString(GridStringId.ColumnViewExceptionMessage, " Do you want to correct the value?");
			AddString(GridStringId.CustomizationCaption, "Customization");
			AddString(GridStringId.CustomizationColumns, "Columns");
			AddString(GridStringId.CustomizationBands, "Bands");
			AddString(GridStringId.PopupFilterAll, "(All)");
			AddString(GridStringId.PopupFilterCustom, "(Custom)");
			AddString(GridStringId.PopupFilterBlanks, "(Blanks)");
			AddString(GridStringId.PopupFilterNonBlanks, "(Non blanks)");
			AddString(GridStringId.CustomFilterDialogFormCaption, "Custom AutoFilter");
			AddString(GridStringId.CustomFilterDialogCaption, "Show rows where:");
			AddString(GridStringId.CustomFilterDialogRadioAnd, "&And");
			AddString(GridStringId.CustomFilterDialogRadioOr, "O&r");
			AddString(GridStringId.CustomFilterDialogOkButton, "&OK");
			AddString(GridStringId.CustomFilterDialogClearFilter, "C&lear Filter");
			AddString(GridStringId.CustomFilterDialogCancelButton, "&Cancel");
			AddString(GridStringId.CustomFilterDialog2FieldCheck, "Field");
			AddString(GridStringId.CustomFilterDialogEmptyValue, "(Enter a value)");
			AddString(GridStringId.CustomFilterDialogEmptyOperator, "(Select an operator)");
			AddString(GridStringId.CustomFilterDialogHint, "Use _ to represent any single character#Use % to represent any series of characters");
			AddString(GridStringId.MenuFooterSum, "Sum");
			AddString(GridStringId.MenuFooterMin, "Min");
			AddString(GridStringId.MenuFooterMax, "Max");
			AddString(GridStringId.MenuFooterCount, "Count");
			AddString(GridStringId.MenuFooterAverage, "Average");
			AddString(GridStringId.MenuFooterNone, "None");
			AddString(GridStringId.MenuFooterSumFormat, "SUM={0:0.##}");
			AddString(GridStringId.MenuFooterMinFormat, "MIN={0}");
			AddString(GridStringId.MenuFooterMaxFormat, "MAX={0}");
			AddString(GridStringId.MenuFooterCountFormat, "{0}");
			AddString(GridStringId.MenuFooterCountGroupFormat, "Count={0}");
			AddString(GridStringId.MenuFooterAverageFormat, "AVG={0:0.##}");
			AddString(GridStringId.MenuFooterCustomFormat, "Custom={0}");
			AddString(GridStringId.MenuColumnSortAscending, "Sort Ascending");
			AddString(GridStringId.MenuColumnSortDescending, "Sort Descending");
			AddString(GridStringId.MenuColumnRemoveColumn, "Hide This Column");
			AddString(GridStringId.MenuColumnShowColumn, "Show This Column");
			AddString(GridStringId.MenuColumnClearSorting, "Clear Sorting");
			AddString(GridStringId.MenuColumnClearAllSorting, "Clear All Sorting");
			AddString(GridStringId.MenuColumnGroup, "Group By This Column");
			AddString(GridStringId.MenuColumnUnGroup, "UnGroup");
			AddString(GridStringId.MenuColumnColumnCustomization, "Column Chooser");
			AddString(GridStringId.MenuColumnBandCustomization, "Column/Band Chooser");
			AddString(GridStringId.MenuColumnBestFit, "Best Fit");
			AddString(GridStringId.MenuColumnFilter, "Can Filter");
			AddString(GridStringId.MenuColumnFilterEditor, "Filter Editor...");
			AddString(GridStringId.MenuColumnAutoFilterRowShow, "Show Auto Filter Row");
			AddString(GridStringId.MenuColumnAutoFilterRowHide, "Hide Auto Filter Row");
			AddString(GridStringId.MenuColumnFindFilterShow, "Show Find Panel");
			AddString(GridStringId.MenuColumnFindFilterHide, "Hide Find Panel");
			AddString(GridStringId.MenuColumnClearFilter, "Clear Filter");
			AddString(GridStringId.MenuColumnBestFitAllColumns, "Best Fit (all columns)");
			AddString(GridStringId.MenuColumnResetGroupSummarySort, "Clear Summary Sorting");
			AddString(GridStringId.MenuColumnGroupSummarySortFormat, "{1} by '{0}' - {2}");
			AddString(GridStringId.MenuColumnSumSummaryTypeDescription, "Sum");
			AddString(GridStringId.MenuColumnMinSummaryTypeDescription, "Min");
			AddString(GridStringId.MenuColumnMaxSummaryTypeDescription, "Max");
			AddString(GridStringId.MenuColumnCountSummaryTypeDescription, "Count");
			AddString(GridStringId.MenuColumnAverageSummaryTypeDescription, "Average");
			AddString(GridStringId.MenuColumnCustomSummaryTypeDescription, "Custom");
			AddString(GridStringId.MenuGroupPanelShow, "Show Group By Box");
			AddString(GridStringId.MenuGroupPanelHide, "Hide Group By Box");
			AddString(GridStringId.MenuColumnSortGroupBySummaryMenu, "Sort by Summary");
			AddString(GridStringId.MenuColumnGroupIntervalMenu, "Group Interval");
			AddString(GridStringId.MenuColumnGroupIntervalNone, "None");
			AddString(GridStringId.MenuColumnGroupIntervalDay, "Day");
			AddString(GridStringId.MenuColumnGroupIntervalMonth, "Month");
			AddString(GridStringId.MenuColumnGroupIntervalYear, "Year");
			AddString(GridStringId.MenuColumnGroupIntervalSmart, "Smart");
			AddString(GridStringId.MenuColumnGroupSummaryEditor, "Group Summary Editor...");
			AddString(GridStringId.MenuColumnExpressionEditor, "Expression Editor...");
			AddString(GridStringId.MenuColumnConditionalFormatting, "Conditional Formatting");
			AddString(GridStringId.MenuColumnFilterMode, "Filter Mode");
			AddString(GridStringId.MenuColumnFilterModeValue, "Value");
			AddString(GridStringId.MenuColumnFilterModeDisplayText, "Display Text");
			AddString(GridStringId.MenuGroupPanelFullExpand, "Full Expand");
			AddString(GridStringId.MenuGroupPanelFullCollapse, "Full Collapse");
			AddString(GridStringId.MenuGroupPanelClearGrouping, "Clear Grouping");
			AddString(GridStringId.PrintDesignerBandedView, "Print Settings (Banded View)");
			AddString(GridStringId.PrintDesignerGridView, "Print Settings (Grid View)");
			AddString(GridStringId.PrintDesignerCardView, "Print Settings (Card View)");
			AddString(GridStringId.PrintDesignerLayoutView, "Print Settings (Layout View)");
			AddString(GridStringId.PrintDesignerDescription, "Set up various printing options for the current view.");
			AddString(GridStringId.PrintDesignerBandHeader, "Band Header");
			AddString(GridStringId.MenuColumnGroupBox, "Group By Box");
			AddString(GridStringId.CardViewNewCard, "New Card");
			AddString(GridStringId.CardViewQuickCustomizationButton, "Customize");
			AddString(GridStringId.CardViewQuickCustomizationButtonFilter, "Filter");
			AddString(GridStringId.CardViewQuickCustomizationButtonSort, "Sort:");
			AddString(GridStringId.CardViewCaptionFormat, "Record N {0}");
			AddString(GridStringId.GridGroupPanelText, "Drag a column header here to group by that column");
			AddString(GridStringId.GridNewRowText, "Click here to add a new row");
			AddString(GridStringId.GridOutlookIntervals,
			   "Older;Last Month;Earlier this Month;Three Weeks Ago;Two Weeks Ago;Last Week;;;;;;;;Yesterday;Today;Tomorrow;;;;;;;;Next Week;Two Weeks Away;Three Weeks Away;Later this Month;Next Month;Beyond Next Month;");
			AddString(GridStringId.FilterBuilderOkButton, "&OK");
			AddString(GridStringId.FilterBuilderCancelButton, "&Cancel");
			AddString(GridStringId.FilterBuilderApplyButton, "&Apply");
			AddString(GridStringId.FilterBuilderCaption, "Filter Editor");
			AddString(GridStringId.CustomizationFormColumnHint, "Drag and drop columns here to customize layout");
			AddString(GridStringId.CustomizationFormBandHint, "Drag and drop bands here to customize layout");
			AddString(GridStringId.LayoutViewSingleModeBtnHint, "One Card");
			AddString(GridStringId.LayoutViewRowModeBtnHint, "One Row");
			AddString(GridStringId.LayoutViewColumnModeBtnHint, "One Column");
			AddString(GridStringId.LayoutViewMultiRowModeBtnHint, "Multiple Rows");
			AddString(GridStringId.LayoutViewMultiColumnModeBtnHint, "Multiple Columns");
			AddString(GridStringId.LayoutViewCarouselModeBtnHint, "Carousel Mode");
			AddString(GridStringId.LayoutViewPanBtnHint, "Panning");
			AddString(GridStringId.LayoutViewCustomizeBtnHint, "Customization");
			AddString(GridStringId.LayoutViewCloseZoomBtnHintClose, "Restore View");
			AddString(GridStringId.LayoutViewCloseZoomBtnHintZoom, "Maximize Detail");
			AddString(GridStringId.LayoutViewButtonApply, @"&Apply");
			AddString(GridStringId.LayoutViewButtonPreview, @"Show &More Cards");
			AddString(GridStringId.LayoutViewButtonSaveLayout, @"Sa&ve Layout...");
			AddString(GridStringId.LayoutViewButtonLoadLayout, @"&Load Layout...");
			AddString(GridStringId.LayoutViewFormLoadLayoutCaption, @"Load Layout");
			AddString(GridStringId.LayoutViewFormSaveLayoutCaption, @"Save Layout");
			AddString(GridStringId.LayoutViewPageTemplateCard, @"Template Card");
			AddString(GridStringId.LayoutViewPageViewLayout, @"View Layout");
			AddString(GridStringId.LayoutViewButtonCustomizeShow, @"&Show Customization");
			AddString(GridStringId.LayoutViewButtonCustomizeHide, @"Hide Customi&zation");
			AddString(GridStringId.LayoutViewButtonReset, @"&Reset Template Card");
			AddString(GridStringId.LayoutViewButtonShrinkToMinimum, @"&Shrink Template Card");
			AddString(GridStringId.LayoutViewGroupCustomization, @"Customization");
			AddString(GridStringId.LayoutViewGroupCaptions, @"Captions");
			AddString(GridStringId.LayoutViewGroupIndents, @"Indents");
			AddString(GridStringId.LayoutViewGroupHiddenItems, @"Hidden Items");
			AddString(GridStringId.LayoutViewGroupTreeStructure, @"Layout Tree View");
			AddString(GridStringId.LayoutViewGroupPropertyGrid, @"Property Grid");
			AddString(GridStringId.LayoutViewLabelTextIndent, @"Text Indents");
			AddString(GridStringId.LayoutViewLabelPadding, @"Padding");
			AddString(GridStringId.LayoutViewLabelSpacing, @"Spacing");
			AddString(GridStringId.LayoutViewLabelCaptionLocation, @"Field Caption Location:");
			AddString(GridStringId.LayoutViewLabelGroupCaptionLocation, @"Group Caption Location:");
			AddString(GridStringId.LayoutViewLabelTextAlignment, @"Field Caption Text Alignment:");
			AddString(GridStringId.LayoutViewGroupView, @"View");
			AddString(GridStringId.LayoutViewGroupLayout, @"Layout");
			AddString(GridStringId.LayoutViewGroupCards, @"Cards");
			AddString(GridStringId.LayoutViewGroupFields, @"Fields");
			AddString(GridStringId.LayoutViewLabelShowLines, @"Show Lines");
			AddString(GridStringId.LayoutViewLabelShowHeaderPanel, @"Show Header Panel");
			AddString(GridStringId.LayoutViewLabelShowFilterPanel, @"Show Filter Panel:");
			AddString(GridStringId.LayoutViewLabelScrollVisibility, @"Scroll Visibility:");
			AddString(GridStringId.LayoutViewLabelViewMode, @"View Mode:");
			AddString(GridStringId.LayoutViewLabelCardArrangeRule, @"Arrange Rule:");
			AddString(GridStringId.LayoutViewLabelCardEdgeAlignment, @"Card Edge Alignment:");
			AddString(GridStringId.LayoutViewGroupIntervals, @"Intervals");
			AddString(GridStringId.LayoutViewLabelHorizontal, @"Horizontal Interval");
			AddString(GridStringId.LayoutViewLabelVertical, @"Vertical Interval");
			AddString(GridStringId.LayoutViewLabelShowCardCaption, @"Show Caption");
			AddString(GridStringId.LayoutViewLabelShowCardExpandButton, @"Show Expand Button");
			AddString(GridStringId.LayoutViewLabelShowCardBorder, @"Show Border");
			AddString(GridStringId.LayoutViewLabelAllowFieldHotTracking, @"Allow Hot-Tracking");
			AddString(GridStringId.LayoutViewLabelShowFieldBorder, @"Show Border");
			AddString(GridStringId.LayoutViewLabelShowFieldHint, @"Show Hint");
			AddString(GridStringId.LayoutViewCustomizationFormCaption, @"LayoutView Customization");
			AddString(GridStringId.LayoutViewButtonOk, @"&OK");
			AddString(GridStringId.LayoutViewButtonCancel, @"&Cancel");
			AddString(GridStringId.LayoutViewCustomizationFormDescription, @"Customize the card layout using drag-and-drop and customization menu, and preview data in the View Layout page.");
			AddString(GridStringId.LayoutModifiedWarning, "The layout has been modified.\r\nDo you want to save the changes?");
			AddString(GridStringId.LayoutViewCardCaptionFormat, "Record [{0} of {1}]");
			AddString(GridStringId.LayoutViewFieldCaptionFormat, "{0}:");
			AddString(GridStringId.GroupSummaryEditorFormCaption, "Group Summary Editor"); 
			AddString(GridStringId.GroupSummaryEditorFormOkButton, "OK");  
			AddString(GridStringId.GroupSummaryEditorFormCancelButton, "Cancel");
			AddString(GridStringId.GroupSummaryEditorFormItemsTabCaption, "Items");
			AddString(GridStringId.GroupSummaryEditorFormOrderTabCaption, "Order");
			AddString(GridStringId.GroupSummaryEditorSummaryMin, "Min");
			AddString(GridStringId.GroupSummaryEditorSummaryMax, "Max");
			AddString(GridStringId.GroupSummaryEditorSummaryAverage, "Average");
			AddString(GridStringId.GroupSummaryEditorSummarySum, "Sum");
			AddString(GridStringId.GroupSummaryEditorSummaryCount, "Count");
			AddString(GridStringId.WindowErrorCaption, "Error");
			AddString(GridStringId.FindControlFindButton, "Find");
			AddString(GridStringId.FindControlClearButton, "Clear");
			AddString(GridStringId.SearchLookUpAddNewButton, "Add New");
			AddString(GridStringId.MenuFooterAddSummaryItem, "Add New Summary");
			AddString(GridStringId.MenuFooterClearSummaryItems, "Clear Summary Items");
			AddString(GridStringId.MenuShowSplitItem, "Split");
			AddString(GridStringId.MenuHideSplitItem, "Remove Split");
			AddString(GridStringId.ServerRequestError, "Error occurred during processing server request ({0}...)");
			AddString(GridStringId.SearchLookUpMissingRows, @"To show all rows, press ENTER or click Find.
To search for rows, type a search string and press ENTER or click Find.");
			AddString(GridStringId.WindowWarningCaption, "Warning");
			AddString(GridStringId.EditFormUpdateButton, "Update");
			AddString(GridStringId.EditFormCancelButton, "Cancel");
			AddString(GridStringId.EditFormCancelMessage, "Do you want to cancel editing?");
			AddString(GridStringId.EditFormSaveMessage, "Your data is modified. Do you want to save the changes?");
			AddString(GridStringId.CheckboxSelectorColumnCaption, "Selection");
			AddString(GridStringId.FindNullPrompt, "Enter text to search...");
			AddString(GridStringId.SearchForBand, "Search for a band...");
		}
		#endregion
	}
	public class GridResLocalizer : XtraResXLocalizer<GridStringId> {
		public GridResLocalizer() : base(new GridLocalizer()) {
		}
		protected override ResourceManager CreateResourceManagerCore() {
			return new ResourceManager("DevExpress.XtraGrid.LocalizationRes", typeof(GridResLocalizer).Assembly);
		}
	}
	#region enum GridStringId
	public enum GridStringId {
		FileIsNotFoundError,
		ColumnViewExceptionMessage,
		CustomizationCaption,
		CustomizationColumns,
		CustomizationBands,
		FilterPanelCustomizeButton,
		PopupFilterAll,
		PopupFilterCustom,
		PopupFilterBlanks,
		PopupFilterNonBlanks,
		CustomFilterDialogFormCaption,
		CustomFilterDialogCaption,
		CustomFilterDialogRadioAnd,
		CustomFilterDialogRadioOr,
		CustomFilterDialogOkButton,
		CustomFilterDialogClearFilter,
		CustomFilterDialog2FieldCheck,
		CustomFilterDialogCancelButton,
		CustomFilterDialogEmptyValue, 
		CustomFilterDialogEmptyOperator, 
		CustomFilterDialogHint,
		WindowErrorCaption,
		MenuFooterSum,
		MenuFooterMin, 
		MenuFooterMax, 
		MenuFooterCount, 
		MenuFooterAverage,
		MenuFooterNone,
		MenuFooterSumFormat,
		MenuFooterMinFormat, 
		MenuFooterMaxFormat, 
		MenuFooterCountFormat, 
		MenuFooterAverageFormat,
		MenuColumnSortAscending, 
		MenuColumnSortDescending,
		MenuColumnShowColumn,
		MenuColumnRemoveColumn,
		MenuColumnGroup, 
		MenuColumnUnGroup, 
		MenuColumnColumnCustomization,
		MenuColumnBandCustomization, 
		MenuColumnBestFit, 
		MenuColumnFilter, 
		MenuColumnClearFilter, 
		MenuColumnBestFitAllColumns, 
		MenuColumnResetGroupSummarySort,
		MenuColumnGroupSummarySortFormat,
		MenuColumnSumSummaryTypeDescription,
		MenuColumnMinSummaryTypeDescription,
		MenuColumnMaxSummaryTypeDescription,
		MenuColumnCountSummaryTypeDescription,
		MenuColumnAverageSummaryTypeDescription,
		MenuColumnCustomSummaryTypeDescription,
		MenuColumnSortGroupBySummaryMenu,
		MenuColumnGroupIntervalMenu,
		MenuColumnGroupIntervalNone,
		MenuColumnGroupIntervalDay,
		MenuColumnGroupIntervalMonth,
		MenuColumnGroupIntervalYear,
		MenuColumnGroupIntervalSmart,
		MenuColumnGroupSummaryEditor,
		MenuColumnExpressionEditor,
		MenuColumnConditionalFormatting,
		MenuColumnFilterMode,
		MenuColumnFilterModeValue,
		MenuColumnFilterModeDisplayText, 
		MenuGroupPanelFullExpand, 
		MenuGroupPanelFullCollapse,
		MenuGroupPanelClearGrouping,
		MenuGroupPanelShow,
		MenuGroupPanelHide,
		PrintDesignerGridView,
		PrintDesignerCardView,
		PrintDesignerLayoutView,
		PrintDesignerBandedView,
		PrintDesignerBandHeader,
		MenuColumnGroupBox,
		CardViewNewCard,
		CardViewQuickCustomizationButton,
		CardViewQuickCustomizationButtonFilter,
		CardViewQuickCustomizationButtonSort,
		CardViewCaptionFormat,
		GridGroupPanelText,
		GridNewRowText,
		GridOutlookIntervals, 
		PrintDesignerDescription,
		MenuFooterCustomFormat,
		MenuFooterCountGroupFormat,
		MenuColumnClearSorting,
		MenuColumnClearAllSorting,
		MenuColumnFilterEditor,
		MenuColumnAutoFilterRowHide,
		MenuColumnAutoFilterRowShow,
		MenuColumnFindFilterHide,
		MenuColumnFindFilterShow,
		FilterBuilderOkButton,
		FilterBuilderCancelButton,
		FilterBuilderApplyButton,
		FilterBuilderCaption,
		CustomizationFormColumnHint,
		CustomizationFormBandHint,
		LayoutViewSingleModeBtnHint,
		LayoutViewRowModeBtnHint,
		LayoutViewColumnModeBtnHint,
		LayoutViewMultiRowModeBtnHint,
		LayoutViewMultiColumnModeBtnHint,
		LayoutViewCarouselModeBtnHint,
		LayoutViewPanBtnHint,
		LayoutViewCustomizeBtnHint,
		LayoutViewCloseZoomBtnHintClose,
		LayoutViewCloseZoomBtnHintZoom,
		LayoutViewButtonApply,
		LayoutViewButtonPreview,
		LayoutViewButtonOk,
		LayoutViewButtonCancel,
		LayoutViewButtonSaveLayout,
		LayoutViewButtonLoadLayout,
		LayoutViewFormLoadLayoutCaption,
		LayoutViewFormSaveLayoutCaption,
		LayoutViewButtonCustomizeShow,
		LayoutViewButtonCustomizeHide,
		LayoutViewButtonReset,
		LayoutViewButtonShrinkToMinimum,
		LayoutViewPageTemplateCard,
		LayoutViewPageViewLayout,
		LayoutViewGroupCustomization,
		LayoutViewGroupCaptions,
		LayoutViewGroupIndents,
		LayoutViewGroupHiddenItems,
		LayoutViewGroupTreeStructure,
		LayoutViewGroupPropertyGrid,
		LayoutViewLabelTextIndent,
		LayoutViewLabelPadding,
		LayoutViewLabelSpacing,
		LayoutViewLabelCaptionLocation,
		LayoutViewLabelGroupCaptionLocation,
		LayoutViewLabelTextAlignment,
		LayoutViewGroupView,
		LayoutViewGroupLayout,
		LayoutViewGroupCards,
		LayoutViewGroupFields,
		LayoutViewLabelShowLines,
		LayoutViewLabelShowHeaderPanel,
		LayoutViewLabelShowFilterPanel,
		LayoutViewLabelScrollVisibility,
		LayoutViewLabelViewMode,
		LayoutViewLabelCardArrangeRule,
		LayoutViewLabelCardEdgeAlignment,
		LayoutViewGroupIntervals,
		LayoutViewLabelHorizontal,
		LayoutViewLabelVertical,
		LayoutViewLabelShowCardCaption,
		LayoutViewLabelShowCardExpandButton,
		LayoutViewLabelShowCardBorder,
		LayoutViewLabelAllowFieldHotTracking,
		LayoutViewLabelShowFieldBorder,
		LayoutViewLabelShowFieldHint,
		LayoutViewCustomizationFormCaption,
		LayoutViewCustomizationFormDescription,
		LayoutModifiedWarning,
		LayoutViewCardCaptionFormat,
		LayoutViewFieldCaptionFormat,
		GroupSummaryEditorFormCaption,
		GroupSummaryEditorFormOkButton,
		GroupSummaryEditorFormCancelButton,
		GroupSummaryEditorFormItemsTabCaption,
		GroupSummaryEditorFormOrderTabCaption,
		GroupSummaryEditorSummaryMin,
		GroupSummaryEditorSummaryMax,
		GroupSummaryEditorSummaryAverage,
		GroupSummaryEditorSummarySum,
		GroupSummaryEditorSummaryCount,
		FindControlFindButton,
		FindControlClearButton,
		SearchLookUpMissingRows,
		SearchLookUpAddNewButton,
		MenuFooterAddSummaryItem,
		MenuFooterClearSummaryItems,
		MenuShowSplitItem, 
		MenuHideSplitItem,
		ServerRequestError,
		WindowWarningCaption,
		EditFormUpdateButton,
		EditFormCancelButton,
		EditFormCancelMessage,
		EditFormSaveMessage,
		CheckboxSelectorColumnCaption,
		FindNullPrompt,
		SearchForBand
	}
	#endregion
}
