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
namespace DevExpress.Web.Internal {
	public static class ASPxPanelElementsCss {
		public static string Control { get { return "dxpnlControl"; } }
		public static string ExpandBar { get { return "dxpnl-bar"; } }
		public static string ExpandButton { get { return "dxpnl-btn"; } }
	}
	public static class ASPxNavBarElementsCss {
		public static string Control { get { return "dxnbLite"; } }
		public static string GroupHeader { get { return "dxnb-header"; } }
		public static string GroupHeaderCollapsed { get { return "dxnb-headerCollapsed"; } }
		public static string ExpandImage { get { return "dxWeb_nbExpand"; } }
		public static string CollapseImage { get { return "dxWeb_nbCollapse"; } }
		public static string GroupContent { get { return "dxnb-content"; } }
		public static string Item { get { return "dxnb-item"; } }
	}
	public static class ASPxCloudControlElementsCss {
		public static string Control { get { return "dxccControl"; } }
	}
	public static class ASPxPagerElementsCss {
		public static string Control { get { return "dxpLite"; } }
		public static string Summary { get { return "dxp-summary;"; } }
		public static string Button { get { return "dxp-button;"; } }
		public static string FirstButtonImage { get { return "dxWeb_pFirst;dxWeb_pFirstDisabled"; } }
		public static string PreviousButtonImage { get { return "dxWeb_pPrev;dxWeb_pPrevDisabled"; } }
		public static string NextButtonImage { get { return "dxWeb_pNext;dxWeb_pNextDisabled"; } }
		public static string LastButtonImage { get { return "dxWeb_pLast;dxWeb_pLastDisabled"; } }
		public static string AllButtonImage { get { return "dxWeb_pAll;dxWeb_pAllDisabled"; } }
		public static string CurrentPageNumber { get { return "dxp-current"; } }
		public static string PageNumber { get { return "dxp-num"; } }
		public static string PageSizeItem { get { return "dxp-pageSizeItem"; } }
		public static string PageSizeItemEditor { get { return "dxp-comboBox"; } }
		public static string PageSizeItemButton { get { return "dxp-dropDownButton"; } }
	}
	public static class ASPxDataViewElementsCss {
		public static string Control { get { return "dxdvControl"; } }
		public static string Content { get { return "dxdvContent"; } }
		public static string Item { get { return "dxdvItem"; } }
	}
	public static class ASPxHeadlineElementsCss {
		public static string Control { get { return "dxhlControl"; } }
		public static string Header { get { return "dxhlHeader"; } }
		public static string Date { get { return "dxhlDate;dxhlDateHeader"; } }
		public static string Content { get { return "dxhlContent"; } }
		public static string Tail { get { return "dxhlTailDiv;dxhlContent dxhl"; } }
	}
	public static class ASPxFileManagerElementsCss {
		public static string Control { get { return "dxfmControl"; } }
		public static string Toolbar { get { return "dxfm-toolbar"; } }
		public static string ToolbarItem { get { return "dxm-item"; } }
		public static string Folder { get { return "dxtv-nd"; } }
		public static string File { get { return "dxfm-file"; } }
		public static string Filter { get { return "dxfm-filter"; } }
		public static string UploadPanel { get { return "dxfm-uploadPanel"; } }
		public static string CreateButtonImage { get { return "dxWeb_fmCreateButton;dxWeb_fmCreateButtonDisabled"; } }
		public static string RenameButtonImage { get { return "dxWeb_fmRenameButton;dxWeb_fmRenameButtonDisabled"; } }
		public static string MoveButtonImage { get { return "dxWeb_fmMoveButton;dxWeb_fmMoveButtonDisabled"; } }
		public static string DeleteButtonImage { get { return "dxWeb_fmDeleteButton;dxWeb_fmDeleteButtonDisabled"; } }
		public static string RefreshButtonImage { get { return "dxWeb_fmRefreshButton;dxWeb_fmRefreshButtonDisabled"; } }
		public static string DownloadButtonImage { get { return "dxWeb_fmDwnlButton;dxWeb_fmDwnlButtonDisabled"; } }
		public static string CopyButtonImage { get { return "dxWeb_fmCopyButton;dxWeb_fmCopyButtonDisabled"; } }
	}
	public static class ASPxFormLayoutElementsCss {
		public static string Control { get { return "dxflFormLayout"; } }
		public static string GroupBox { get { return "dxflGroupBox"; } }
		public static string GroupBoxCaption { get { return "dxflGroupBoxCaption"; } }
		public static string Group { get { return "dxflGroup"; } }
		public static string GroupCell { get { return "dxflGroupCell"; } }
		public static string Item { get { return "dxflItem"; } }
		public static string ItemCaptionCell { get { return "dxflCaptionCell"; } }
		public static string ItemNestedControlCell { get { return "dxflNestedControlCell"; } }
		public static string RequiredLabel { get { return "dxflRequired"; } }
		public static string OptionalLabel { get { return "dxflOptional"; } }
	}
	public static class ASPxImageGalleryElementsCss {
		public static string Control { get { return "dxigControl"; } }
		public static string Content { get { return "dxigContent"; } }
		public static string Item { get { return "dxigItem"; } }
		public static string ItemText { get { return "dxig-thumbnailTextArea"; } }
		public static string FullscreenViewerItemText { get { return "dxig-fullscreenViewerTextArea"; } }
		public static string FullscreenViewerPrevButtonImage { get { return "dxWeb_igPrevButton"; } }
		public static string FullscreenViewerNextButtonImage { get { return "dxWeb_igNextButton"; } }
		public static string FullscreenViewerPlayButtonImage { get { return "dxWeb_igPlayButton"; } }
		public static string FullscreenViewerPauseButtonImage { get { return "dxWeb_igPauseButton"; } }
		public static string FullscreenViewerCloseButtonImage { get { return "dxWeb_igCloseButton"; } }
	}
	public static class ASPxImageSliderElementsCss {
		public static string Control { get { return "dxisControl"; } }
		public static string Item { get { return "dxis-item"; } }
		public static string PrevButtonHorizontal { get { return "dxis-prevBtnHor"; } }
		public static string NextButtonHorizontal { get { return "dxis-nextBtnHor"; } }
		public static string PrevButtonVertical { get { return "dxis-prevBtnVert"; } }
		public static string NextButtonVertical { get { return "dxis-nextBtnVert"; } }
		public static string NavigationBarItem { get { return "dxis-nbItem"; } }
		public static string NavigationBarSelectedItem { get { return "dxis-nbSelectedItem"; } }
		public static string NavigationBarDotItem { get { return "dxis-nbDotItem"; } }
		public static string NavigationBarPrevButton { get { return "dxis-prevPageBtnHor;dxis-prevPageBtnVert"; } }
		public static string NavigationBarNextButton { get { return "dxis-nextPageBtnHor;dxis-nextPageBtnVert"; } }
		public static string PlayButtonImage { get { return "dxWeb_isPlayBtn"; } }
		public static string PauseButtonImage { get { return "dxWeb_isPauseBtn"; } }
	}
	public static class ASPxImageZoomElementsCss {
		public static string Control { get { return "dxizControl"; } }
		public static string Lens { get { return "dxiz-lens"; } }
		public static string Hint { get { return "dxiz-hint"; } }
		public static string HintImage { get { return "dxWeb_izHint"; } }
		public static string CloseButton { get { return "dxiz-EWCloseButton"; } }
		public static string CloseButtonImage { get { return "dxWeb_izEWCloseButton"; } }
	}
	public static class ASPxImageZoomNavigatorElementsCss {
		public static string Control { get { return "dxisControl"; } }
		public static string NavigationBarItem { get { return "dxis-nbItem"; } }
		public static string NavigationBarSelectedItem { get { return "dxis-nbSelectedItem"; } }
		public static string NavigationBarPrevButton { get { return "dxis-prevPageBtnHorOutside;dxis-prevPageBtnVertOutside"; } }
		public static string NavigationBarNextButton { get { return "dxis-nextPageBtnHorOutside;dxis-nextPageBtnVertOutside"; } }
	}
	public static class ASPxLoadingPanelElementsCss {
		public static string LoadingDiv { get { return "dxlpLoadingDiv"; } }
		public static string LoadingPanel { get { return "dxlpLoadingPanel;dxlpLoadingPanelWithContent"; } }
	}
	public static class ASPxPopupMenuElementsCss {
		public static string Control { get { return "dxm-popup"; } }
		public static string Item { get { return "dxm-popup dxm-item"; } }
		public static string PopOutImage { get { return "dxWeb_mVerticalPopOut;dxWeb_mHorizontalPopOut"; } }
		public static string ScrollUpButtonImage { get { return "dxWeb_mScrollUp"; } }
		public static string ScrollDownButtonImage { get { return "dxWeb_mScrollDown"; } }
		public static string CheckedImage { get { return "dxWeb_mSubMenuItemChecked"; } }
	}
	public static class ASPxMenuElementsCss {
		public static string Control { get { return "dxmLite"; } }
		public static string RootMenu { get { return "dxm-main"; } }
		public static string RootItem { get { return "dxm-main dxm-item"; } }
	}
	public static class ASPxNewsControlElementsCss {
		public static string Control { get { return "dxncControl"; } }
		public static string PagerPanel { get { return "dxncPagerPanel"; } }
		public static string Content { get { return "dxncContent"; } }
		public static string BackToTopPanel { get { return "dxncBackToTop"; } }
		public static string BackToTopImage { get { return "dxWeb_ncBackToTop"; } }
	}
	public static class ASPxTabControlElementsCss {
		public static string Control { get { return "dxtcControl;dxtcLite"; } }
		public static string TabPanel { get { return "dxtc-wrapper;dxtc-strip"; } }
		public static string Tab { get { return "dxtc-tab"; } }
		public static string ActiveTab { get { return "dxtc-activeTab"; } }
		public static string ScrollButton { get { return "dxtc-sb"; } }
		public static string ScrollLeftButtonImage { get { return "dxWeb_tcScrollLeft"; } }
		public static string ScrollRightButtonImage { get { return "dxWeb_tcScrollRight"; } }
	}
	public static class ASPxPageControlElementsCss {
		public static string Content { get { return "dxtc-content"; } }
	}
	public static class ASPxPopupControlElementsCss {
		public static string Control { get { return "dxpcLite"; } }
		public static string Header { get { return "dxpc-header"; } }
		public static string CloseButtonImage { get { return "dxWeb_pcCloseButton"; } }
		public static string PinButtonImage { get { return "dxWeb_pcPinButton"; } }
		public static string RefreshButtonImage { get { return "dxWeb_pcRefreshButton"; } }
		public static string CollapseButtonImage { get { return "dxWeb_pcCollapseButton"; } }
		public static string MaximizeButtonImage { get { return "dxWeb_pcMaximizeButton"; } }
		public static string Content { get { return "dxpc-content"; } }
		public static string Footer { get { return "dxpc-footer"; } }
		public static string SizeGripImage { get { return "dxWeb_pcSizeGrip"; } }
	}
	public static class ASPxDockPanelElementsCss {
		public static string Control { get { return "dxdpLite"; } }
	}
	public static class ASPxRoundPanelElementsCss {
		public static string Control { get { return "dxrpControl;dxrpControlGB"; } }
		public static string Header { get { return "dxrpHeader"; } }
		public static string Content { get { return "dxrpcontent"; } }
		public static string CollapseButtonImage { get { return "dxWeb_rpCollapseButton"; } }
	}
	public static class ASPxSiteMapElementsCss {
		public static string Control { get { return "dxsmControl"; } }
		public static string Header { get { return "dxrpHeader"; } }
		public static string CategoryLevel { get { return "dxsmCategoryLevel"; } }
		public static string BulletImage { get { return "dxWeb_smBullet"; } }
		public static string RootLevelNode { get { return "dxsmLevel0;dxsmLevel0Categorized;dxsmLevel0Flow"; } }
		public static string FirstLevelNode { get { return "dxsmLevel1;dxsmLevel1Categorized;dxsmLevel1Flow"; } }
		public static string SecondLevelNode { get { return "dxsmLevel2;dxsmLevel2Categorized;dxsmLevel2Flow"; } }
		public static string ThirdLevelNode { get { return "dxsmLevel3;dxsmLevel3Categorized;dxsmLevel3Flow"; } }
		public static string FourthLevelNode { get { return "dxsmLevel4;dxsmLevel4Categorized;dxsmLevel4Flow"; } }
		public static string OtherLevelNode { get { return "dxsmLevelOther"; } }
	}
	public static class ASPxTitleIndexElementsCss {
		public static string Control { get { return "dxtiControl"; } }
		public static string IndexPanel { get { return "dxtiIndexPanel"; } }
		public static string IndexPanelItem { get { return "dxtiIndexPanelItem"; } }
		public static string FilterPanel { get { return "dxtiFilterBox"; } }
		public static string FilterInput { get { return "dxtiFilterBoxEdit"; } }
		public static string FilterPanelInfo { get { return "dxtiFilterBoxInfoText"; } }
		public static string GroupHeader { get { return "dxtiGroupHeader;dxtiGroupHeaderCategorized"; } }
		public static string GroupHeaderText { get { return "dxtiGroupHeaderText"; } }
		public static string Item { get { return "dxtiItem"; } }
		public static string BackToTopPanel { get { return "dxtiBackToTop;dxtiBackToTopRtl"; } }
		public static string BackToTopImage { get { return "dxWeb_tiBackToTop"; } }
	}
	public static class ASPxSplitterElementsCss {
		public static string Control { get { return "dxsplControl"; } }
		public static string Pane { get { return "dxsplPane"; } }
		public static string VerticalSeparator { get { return "dxsplVSeparator"; } }
		public static string HorizontalSeparator { get { return "dxsplHSeparator"; } }
		public static string VerticalCollapseForwardButtonImage { get { return "dxWeb_splVCollapseForwardButton"; } }
		public static string VerticalCollapseBackwardButtonImage { get { return "dxWeb_splVCollapseBackwardButton"; } }
		public static string VerticalSeparatorButtonImage { get { return "dxWeb_splVSeparator"; } }
		public static string HorizontalCollapseForwardButtonImage { get { return "dxWeb_splHCollapseForwardButton"; } }
		public static string HorizontalCollapseBackwardButtonImage { get { return "dxWeb_splHCollapseBackwardButton"; } }
		public static string HorizontalSeparatorButtonImage { get { return "dxWeb_splHSeparator"; } }
	}
	public static class ASPxTreeViewElementsCss {
		public static string Control { get { return "dxtvControl"; } }
		public static string Node { get { return "dxtv-nd"; } }
		public static string NodeText { get { return "dxtv-ndTxt"; } }
		public static string ExpandButtonImage { get { return "dxWeb_tvExpBtn"; } }
		public static string CollapseButtonImage { get { return "dxWeb_tvColBtn"; } }
	}
	public static class ASPxUploadControlElementsCss {
		public static string Control { get { return "dxucControl"; } }
		public static string PathTextBox { get { return "dxucTextBox"; } }
		public static string ClearFileSelectionImage { get { return "dxWeb_ucClearButton;dxWeb_ucClearButtonDisabled"; } }
		public static string BrowseButton { get { return "dxucBrowseButton"; } }
		public static string Button { get { return "dxucButton"; } }
		public static string ErrorFrame { get { return "dxucErrorCell"; } }
	}
	public static class ASPxButtonElementsCss {
		public static string Control { get { return "dxbButton"; } }
	}
	public static class ASPxImageElementsCss {
		public static string Control { get { return "dxeImage"; } }
		public static string Caption { get { return "dxeCaption"; } }
	}
	public static class ASPxButtonEditElementsCss {
		public static string Control { get { return "dxeButtonEdit"; } }
		public static string Button { get { return "dxeButtonEditButton"; } }
		public static string EllipsisImage { get { return "dxEditors_edtEllipsis;dxEditors_edtEllipsisDisabled"; } }
		public static string ClearButton { get { return ASPxButtonEditElementsCss.Button; } }
		public static string ClearButtonImage { get { return "dxEditors_edtClear;dxEditors_edtClearDisabled"; } }
	}
	public static class ASPxCalendarElementsCss {
		public static string Control { get { return "dxeCalendar"; } }
		public static string Header { get { return "dxeCalendarHeader"; } }
		public static string PreviousYearButtonImage { get { return "dxEditors_edtCalendarPrevYear;dxEditors_edtCalendarPrevYearDisabled"; } }
		public static string NextYearButtonImage { get { return "dxEditors_edtCalendarNextYear;dxEditors_edtCalendarNextYearDisabled"; } }
		public static string PreviousMonthButtonImage { get { return "dxEditors_edtCalendarPrevMonth;dxEditors_edtCalendarPrevMonthDisabled"; } }
		public static string NextMonthButtonImage { get { return "dxEditors_edtCalendarNextMonth;dxEditors_edtCalendarNextMonthDisabled"; } }
		public static string DayHeader { get { return "dxeCalendarDayHeader"; } }
		public static string WeekNumber { get { return "dxeCalendarWeekNumber"; } }
		public static string Day { get { return "dxeCalendarDay"; } }
		public static string Today { get { return "dxeCalendarToday"; } }
		public static string SelectedDay { get { return "dxeCalendarSelected"; } }
		public static string OtherDay { get { return "dxeCalendarOtherMonth"; } }
		public static string Weekend { get { return "dxeCalendarWeekend"; } }
		public static string Footer { get { return "dxeCalendarFooter"; } }
		public static string Button { get { return "dxeCalendarButton"; } }
		public static string FastNavigationContent { get { return "dxeCalendarFastNav"; } }
		public static string FastNavigationFooter { get { return "dxeCalendarFastNavFooter"; } }
		public static string FastNavigationMonthArea { get { return "dxeCalendarFastNavMonthArea"; } }
		public static string FastNavigationMonth { get { return "dxeCalendarFastNavMonth"; } }
		public static string FastNavigationYearArea { get { return "dxeCalendarFastNavYearArea"; } }
		public static string FastNavigationYear { get { return "dxeCalendarFastNavYear"; } }
		public static string FastNavigationPrevYearButtonImage { get { return "dxEditors_edtCalendarFNPrevYear"; } }
		public static string FastNavigationNextYearButtonImage { get { return "dxEditors_edtCalendarFNNextYear"; } }
	}
	public static class ASPxCaptchaElementsCss {
		public static string Control { get { return "dxcaControl"; } }
		public static string TextBoxLabel { get { return "dxcaTextBoxLabel"; } }
		public static string TextBox { get { return "dxeTextBox"; } }
		public static string RefreshButtonImage { get { return "dxEditors_caRefresh"; } }
		public static string RefreshButtonText { get { return "dxcaRefreshButtonText"; } }
	}
	public static class ASPxCheckBoxElementsCss {
		public static string Control { get { return "dxeBase"; } }
		public static string CheckedImage { get { return "dxWeb_edtCheckBoxChecked"; } }
		public static string UncheckedImage { get { return "dxWeb_edtCheckBoxUnchecked"; } }
		public static string GrayedImage { get { return "dxWeb_edtCheckBoxGrayed"; } }
	}
	public static class ASPxCheckBoxListElementsCss {
		public static string Control { get { return "dxeCheckBoxList"; } }
	}
	public static class ASPxDropDownEditElementsCss {
		public static string Control { get { return ASPxButtonEditElementsCss.Control; } }
		public static string DropDownButton { get { return ASPxButtonEditElementsCss.Button; } }
		public static string DropDownButtonImage { get { return "dxEditors_edtDropDown;dxEditors_edtDropDownDisabled"; } }
		public static string DropDownWindow { get { return "dxeDropDownWindow"; } }
	}
	public static class ASPxColorEditElementsCss {
		public static string ColorTable { get { return "dxeColorTable"; } }
		public static string ColorCell { get { return "dxeColorTableCell"; } }
		public static string ColorCellDiv { get { return "dxeColorTableCellDiv"; } }
		public static string SelectedColorCell { get { return "dxeColorTableCellSelected"; } }
	}
	public static class ASPxComboBoxElementsCss {
		public static string ListBox { get { return "dxeListBox"; } }
	}
	public static class ASPxDateEditElementsCss {
		public static string Calendar { get { return "dxeCalendar"; } }
	}
	public static class ASPxFilterControlElementsCss {
		public static string Control { get { return "dxfcTable"; } }
		public static string GroupType { get { return "dxfcGroupType"; } }
		public static string AddGroupImage { get { return "dxEditors_fcadd"; } }
		public static string PropertyName { get { return "dxfcPropertyName"; } }
		public static string Operator { get { return "dxfcOperation"; } }
		public static string Value { get { return "dxfcValue"; } }
		public static string RemoveButtonImage { get { return "dxEditors_fcremove"; } }
	}
	public static class ASPxHyperLinkElementsCss {
		public static string Control { get { return "dxeHyperlink"; } }
	}
	public static class ASPxLabelElementsCss {
		public static string Control { get { return "dxeBase"; } }
	}
	public static class ASPxListBoxElementsCss {
		public static string Control { get { return "dxeListBox"; } }
		public static string Item { get { return "dxeListBoxItem"; } }
	}
	public static class ASPxMemoElementsCss {
		public static string Control { get { return "dxeMemo"; } }
		public static string EditArea { get { return "dxeMemoEditArea"; } }
	}
	public static class ASPxProgressBarElementsCss {
		public static string Control { get { return "dxeProgressBar"; } }
		public static string Indicator { get { return "dxeProgressBarIndicator"; } }
	}
	public static class ASPxRadioButtonElementsCss {
		public static string Control { get { return "dxeBase"; } }
		public static string CheckedImage { get { return "dxEditors_edtRadioButtonChecked"; } }
		public static string UncheckedImage { get { return "dxEditors_edtRadioButtonUnchecked"; } }
	}
	public static class ASPxRadioButtonListElementsCss {
		public static string Control { get { return "dxeRadioButtonList"; } }
	}
	public static class ASPxTextBoxElementsCss {
		public static string Control { get { return "dxeTextBox"; } }
		public static string EditArea { get { return "dxeEditArea"; } }
	}
	public static class ASPxSpinEditEditElementsCss {
		public static string Control { get { return ASPxButtonEditElementsCss.Control; } }
		public static string LargeIncrementButton { get { return "dxeSpinLargeIncButton"; } }
		public static string LargeIncrementImage { get { return "dxEditors_edtSpinEditLargeIncImage"; } }
		public static string LargeDecrementButton { get { return "dxeSpinLargeDecButton"; } }
		public static string LargeDecrementImage { get { return "dxEditors_edtSpinEditLargeDecImage"; } }
		public static string IncrementButton { get { return "dxeSpinIncButton"; } }
		public static string IncrementImage { get { return "dxEditors_edtSpinEditIncrementImage"; } }
		public static string DecrementButton { get { return "dxeSpinDecButton"; } }
		public static string DecrementImage { get { return "dxEditors_edtSpinEditDecrementImage"; } }
	}
	public static class ASPxTrackBarElementsCss {
		public static string Control { get { return "dxeTrackBar"; } }
		public static string DecrementButton { get { return "dxEditors_edtTBDecBtn"; } }
		public static string IncrementButton { get { return "dxEditors_edtTBIncBtn"; } }
		public static string LargeTick { get { return "dxeTBLargeTick"; } }
		public static string SmallTick { get { return "dxeTBSmallTick"; } }
		public static string Label { get { return "dxeTBRBLabel;dxeTBLTLabel"; } }
		public static string SelectedTick { get { return "dxeTBSelectedTick"; } }
		public static string Item { get { return "dxeTBItem"; } }
		public static string Track { get { return "dxeTBTrack"; } }
		public static string BarHighlight { get { return "dxeTBBarHighlight"; } }
		public static string MainDragHandler { get { return "dxEditors_edtTBMainDH"; } }
		public static string SecondaryDragHandler { get { return "dxEditors_edtTBSecondaryDH"; } }
		public static string ValueTooltip { get { return "dxeTBValueToolTip"; } }
	}
	public static class ASPxValidationSummaryElementsCss {
		public static string Control { get { return "dxvsValidationSummary"; } }
	}
	public static class ASPxGridViewElementsCss {
		public static string Control { get { return "dxgvControl"; } }
		public static string PagerPanel { get { return "dxgvPagerTopPanel;dxgvPagerBottomPanel"; } }
		public static string TitlePanel { get { return "dxgvTitlePanel"; } }
		public static string GroupPanel { get { return "dxgvGroupPanel"; } }
		public static string GroupHeader { get { return "dxgvHeader"; } }
		public static string GroupHeaderSortUpImage { get { return "dxGridView_gvHeaderSortUp"; } }
		public static string GroupHeaderSortDownImage { get { return "dxGridView_gvHeaderSortDown"; } }
		public static string GroupHeaderFilterImage { get { return "dxGridView_gvHeaderFilter"; } }
		public static string GroupFooter { get { return "dxgvGroupFooter"; } }
		public static string FilterRow { get { return "dxgvFilterRow"; } }
		public static string FilterRowButtonImage { get { return "dxGridView_gvFilterRowButton"; } }
		public static string GroupRow { get { return "dxgvGroupRow"; } }
		public static string FocusedRow { get { return "dxgvFocusedRow;dxgvFocusedGroupRow"; } }
		public static string SelectedRow { get { return "dxgvSelectedRow"; } }
		public static string ExpandedButtonImage { get { return "dxGridView_gvExpandedButton"; } }
		public static string CollapsedButtonImage { get { return "dxGridView_gvCollapsedButton"; } }
		public static string DataRow { get { return "dxgvDataRow"; } }
		public static string PreviewRow { get { return "dxgvPreviewRow"; } }
		public static string Footer { get { return "dxgvFooter"; } }
		public static string FilterBar { get { return "dxgvFilterBar"; } }
		public static string FilterBarExpression { get { return "dxgvFilterBarExpressionCell"; } }
		public static string CommandColumnItem { get { return "dxgvCommandColumnItem"; } }
		public static string AdaptiveHeaderPanel { get { return "dxgvAdaptiveHeaderPanel"; } }		
	}
	public static class ASPxCardViewElementsCss {
		public static string Control { get { return "dxcvControl"; } }
		public static string PagerPanel { get { return "dxcvPagerTopPanel;dxcvPagerBottomPanel"; } }
		public static string TitlePanel { get { return "dxcvTitlePanel"; } }
		public static string HeaderPanel { get { return "dxcvНeaderPanel"; } }
		public static string FocusedCard { get { return "dxcvFocusedCard"; } }
		public static string SelectedCard { get { return "dxcvSelectedCard"; } }
		public static string Card { get { return "dxcvCard;dxcvFlowCard"; } }
		public static string FilterBar { get { return "dxcvFilterBar"; } }
		public static string FilterBarExpression { get { return "dxcvFilterBarExpressionCell"; } }
		public static string CommandItem { get { return "dxcvCommandItem"; } }
		public static string SummaryPanel { get { return "dxcvSummaryPanel"; } }
		public static string SummaryItem { get { return "dxcvSummaryItem"; } }
	}
	public static class ASPxPivotGridElementsCss {
		public static string Control { get { return "dxpgControl"; } }
		public static string PagerPanel { get { return "dxpgTopPager;dxpgBottomPager"; } }
		public static string FilterArea { get { return "dxpgFilterArea"; } }
		public static string DataArea { get { return "dxpgDataArea"; } }
		public static string ColumnArea { get { return "dxpgColumnArea"; } }
		public static string RowArea { get { return "dxpgRowArea"; } }
		public static string Header { get { return "dxpgHeader"; } }
		public static string HeaderText { get { return "dxpgHeaderText"; } }
		public static string HeaderFilterButtonImage { get { return "dxPivotGrid_pgFilterButton"; } }
		public static string HeaderExpandedButtonImage { get { return "dxPivotGrid_pgExpandedButton"; } }
		public static string HeaderCollapsedButtonImage { get { return "dxPivotGrid_pgCollapsedButton"; } }
		public static string HeaderSortUpButtonImage { get { return "dxPivotGrid_pgSortUpButton"; } }
		public static string HeaderSortDownButtonImage { get { return "dxPivotGrid_pgSortDownButton"; } }
		public static string GroupSeparatorImage { get { return "dxPivotGrid_pgGroupSeparator"; } }
		public static string ColumnFieldValue { get { return "dxpgColumnFieldValue"; } }
		public static string ColumnTotalFieldValue { get { return "dxpgColumnTotalFieldValue"; } }
		public static string ColumnGrandTotalFieldValue { get { return "dxpgColumnGrandTotalFieldValue"; } }
		public static string RowFieldValue { get { return "dxpgRowFieldValue"; } }
		public static string RowTotalFieldValue { get { return "dxpgRowTotalFieldValue"; } }
		public static string RowGrandTotalFieldValue { get { return "dxpgRowGrandTotalFieldValue"; } }
		public static string Cell { get { return "dxpgCell"; } }
		public static string TotalCell { get { return "dxpgTotalCell"; } }
		public static string GrandTotalCell { get { return "dxpgGrandTotalCell"; } }
		public static string PrefilterPanel { get { return "dxpgPrefilterPanel"; } }
		public static string PrefilterButtonImage { get { return "dxPivotGrid_pgPrefilterButton"; } }
		public static string PrefilterPanelLink { get { return "dxpgPrefilterPanelLink"; } }
	}
	public static class ASPxTreeListElementsCss {
		public static string Control { get { return "dxtlControl"; } }
		public static string PagerPanel { get { return "dxtlPagerTopPanel;dxtlPagerBottomPanel"; } }
		public static string Header { get { return "dxtlHeader"; } }
		public static string Node { get { return "dxtlNode"; } }
		public static string SelectedNode { get { return "dxtlSelectedNode"; } }
		public static string FocusedNode { get { return "dxtlFocusedNode"; } }
		public static string ExpandedButtonImage { get { return "dxTreeList_CollapsedButton"; } }
		public static string CollapsedButtonImage { get { return "dxTreeList_ExpandedButton"; } }
		public static string Preview { get { return "dxtlPreview"; } }
		public static string GroupFooter { get { return "dxtlGroupFooter"; } }
		public static string Footer { get { return "dxtlFooter"; } }
	}
	public static class ASPxHtmlEditorElementsCss {
		public static string Control { get { return "dxheControl"; } }
		public static string Toolbar { get { return "dxtbControl"; } }
		public static string ToolbarItemPopOutImage { get { return "dxHtmlEditor_heToolbarPopOut"; } }
		public static string DesignViewArea { get { return "dxheDesignViewArea"; } }
		public static string StatusBar { get { return "dxheStatusBar"; } }
		public static string StatusBarTab { get { return "dxheStatusBarTab"; } }
		public static string StatusBarActiveTab { get { return "dxheStatusBarActiveTab"; } }
	}
	public static class ASPxSpellCheckerElementsCss {
		public static string TextContainer { get { return "dxwscCheckedTextContainer"; } }
		public static string ErrorWord { get { return "dxwscErrorWord"; } }
	}
	public static class ASPxSchedulerElementsCss {
		public static string Control { get { return "dxscControl"; } }
		public static string ToolbarContainer { get { return "dxscToolbarContainer"; } }
		public static string Toolbar { get { return "dxscToolbar"; } }
		public static string ViewNavigatorButton { get { return "dxscViewNavigatorButton"; } }
		public static string ViewNavigatorBackwardButtonImage { get { return "dxScheduler_ViewNavigator_Backward"; } }
		public static string ViewNavigatorForwardButtonImage { get { return "dxScheduler_ViewNavigator_Forward"; } }
		public static string ViewNavigatorGoToDateButton { get { return "dxscViewNavigatorGotoDateButton"; } }
		public static string ViewNavigatorGoToDateButtonImage { get { return "dxScheduler_ViewNavigator_Down"; } }
		public static string VisibleInterval { get { return "dxscViewVisibleInterval"; } }
		public static string ViewSelector { get { return "dxscViewSelector"; } }
		public static string ViewSelectorButton { get { return "dxscViewSelectorButton"; } }
		public static string ResourceNavigator { get { return "dxscResourceNavigator"; } }
		public static string ResourceNavigatorButton { get { return "dxscResourceNavigatorButton"; } }
		public static string ResourceNavigatorFirstButtonImage { get { return "dxScheduler_ResourceNavigator_First;dxScheduler_ResourceNavigator_FirstDisabled"; } }
		public static string ResourceNavigatorPrevPageButtonImage { get { return "dxScheduler_ResourceNavigator_PrevPage;dxScheduler_ResourceNavigator_PrevPageDisabled"; } }
		public static string ResourceNavigatorPrevButtonImage { get { return "dxScheduler_ResourceNavigator_Prev;dxScheduler_ResourceNavigator_PrevDisabled"; } }
		public static string ResourceNavigatorLastButtonImage { get { return "dxScheduler_ResourceNavigator_Last;dxScheduler_ResourceNavigator_LastDisabled"; } }
		public static string ResourceNavigatorNextPageButtonImage { get { return "dxScheduler_ResourceNavigator_NextPage;dxScheduler_ResourceNavigator_NextPageDisabled"; } }
		public static string ResourceNavigatorNextButtonImage { get { return "dxScheduler_ResourceNavigator_Next;dxScheduler_ResourceNavigator_NextDisabled"; } }
		public static string ResourceNavigatorIncreaseButtonImage { get { return "dxScheduler_ResourceNavigator_Increase;dxScheduler_ResourceNavigator_IncreaseDisabled"; } }
		public static string ResourceNavigatorDecreaseButtonImage { get { return "dxScheduler_ResourceNavigator_Decrease;dxScheduler_ResourceNavigator_DecreaseDisabled"; } }
		public static string ResourceHeader { get { return "dxscHorizontalResourceHeader;dxscVerticalResourceHeader"; } }
		public static string DateHeader { get { return "dxscDateHeader;dxscAlternateDateHeader"; } }
		public static string TimeRulerHoursItem { get { return "dxscTimeRulerHoursItem"; } }
		public static string TimeRulerMinuteItem { get { return "dxscTimeRulerMinuteItem"; } }
		public static string TimeCell { get { return "dxscTimeCellBody"; } }
		public static string TimeMarker { get { return "dxscTimeMarker"; } }
		public static string NavigationButton { get { return "dxscNavigationButton"; } }
		public static string BackwardNavigationButtonImage { get { return "dxScheduler_NavigationButton_Backward;dxScheduler_NavigationButton_BackwardDisabled"; } }
		public static string ForwardNavigationButtonImage { get { return "dxScheduler_NavigationButton_Forward;dxScheduler_NavigationButton_ForwardDisabled"; } }
		public static string Appointment { get { return "dxscAppointment"; } }
		public static string ReminderImage { get { return "dxScheduler_Appointment_Reminder"; } }
		public static string RecurrenceImage { get { return "dxScheduler_Appointment_Recurrence"; } }
		public static string NoRecurrenceImage { get { return "dxScheduler_Appointment_NoRecurrence"; } }
	}
	public static class ASPxSpreadsheetElementsCss {
		public static string Control { get { return "dxssControl"; } }
		public static string ColumnsHeader { get { return "dxss-colHeader"; } }
		public static string RowsHeader { get { return "dxss-rowHeader"; } }
		public static string ContentArea { get { return "dxss-grid"; } }
		public static string ContentAreaVerticalGridLine { get { return "dxss-v"; } }
		public static string ContentAreaHorizontalGridLine { get { return "dxss-h"; } }
	}
	public static class ASPxRichEditElementsCss {
		public static string Control { get { return "dxreControl"; } }
		public static string Ruler { get { return "dxre-ruler"; } }
		public static string RulerMajorDivision { get { return "dxre-rulerMajorDivision"; } }
		public static string RulerMinorDivision { get { return "dxre-rulerMinorDivision"; } }
		public static string View { get { return "dxre-view"; } }
		public static string StatusBar { get { return "dxre-bar"; } }
	}
	public static class ASPxTokenBoxElementsCss {
		public static string Input { get { return "dxeTokenBoxInput"; } }
		public static string Token { get { return "dxeToken"; } }
		public static string TokenText { get { return "dxeTokenText"; } }
		public static string TokenRemoveButton { get { return "dxeTokenRemoveButton"; } }
	}
	public static class ASPxRibbonElementsCss {
		public static string FileTab { get { return "dxr-fileTab"; } }
		public static string ActiveTab { get { return "dxtc-activeTab"; } }
		public static string InactiveTab { get { return "dxtc-tab[dxr-fileTab]"; } }
		public static string TabContent { get { return "dxr-tabContent"; } }
		public static string Group { get { return "dxr-group"; } }
		public static string GroupLabel { get { return "dxr-groupLabel"; } }
		public static string GroupSeparator { get { return "dxr-groupSep"; } }
		public static string SmallTextItem { get { return "dxr-blHorItems dxr-item"; } }
		public static string SmallItem { get { return "dxr-blRegItems dxr-item[dxr-edtItem]"; } }
		public static string LargeItem { get { return "dxr-blLrgItems dxr-item"; } }
		public static string MinimizeButton { get { return "dxr-minBtn"; } }
	}
	public static class ReportToolbarElementsCss {
		public static string SearchButtonImage { get { return "dxXtraReports_BtnSearch;dxXtraReports_BtnSearchDisabled"; } }
		public static string PrintButtonImage { get { return "dxXtraReports_BtnPrint;dxXtraReports_BtnPrintDisabled"; } }
		public static string PrintPageButtonImage { get { return "dxXtraReports_BtnPrintPage;dxXtraReports_BtnPrintPageDisabled"; } }
		public static string FirstPageButtonImage { get { return "dxXtraReports_BtnFirstPage;dxXtraReports_BtnFirstPageDisabled"; } }
		public static string PrevPageButtonImage { get { return "dxXtraReports_BtnPrevPage;dxXtraReports_BtnPrevPageDisabled"; } }
		public static string NextPageButtonImage { get { return "dxXtraReports_BtnNextPage;dxXtraReports_BtnNextPageDisabled"; } }
		public static string LastPageButtonImage { get { return "dxXtraReports_BtnLastPage;dxXtraReports_BtnLastPageDisabled"; } }
		public static string SaveButtonImage { get { return "dxXtraReports_BtnSave;dxXtraReports_BtnSaveDisabled"; } }
		public static string SaveWindowButtonImage { get { return "dxXtraReports_BtnSaveWindow;dxXtraReports_BtnSaveWindowDisabled"; } }
	}
	public static class WebChartControlElementsCss {
		public static string LoadingPanelControl { get { return "dxchartsuiLoadingPanel"; } }
	}
}
