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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DevExpress.Skins;
using System.Collections;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using System.ComponentModel;
namespace DevExpress.Skins {
	public class RibbonSkins {
		public static string
			SkinSupportGlassTabHeader = "SupportGlassTabHeader",
			SkinSupportTransparentPages = "SupportTransparentPages",
			SkinDrawCategoryBackground = "DrawCategoryBackground",
			SkinTabHeaderBackground = "TabHeaderBackground",
			SkinTabHeaderBackground2010 = "TabHeaderBackground2010",
			SkinTabHeaderBackgroundCaption = "TabHeaderBackgroundCaption",
			SkinTabHeaderPage = "TabHeaderPage",
			SkinTabHeaderPage2014 = "TabHeaderPage2014",
			SkinApplicationTabHeaderPage = "ApplicationTabHeaderPage",
			SkinPageSeparator = "PageSeparator",
			SkinSeparator = "Separator",
			SkinTabPanel = "TabPanel",
			SkinTabPanel2014 = "TabPanel2014",
			SkinTabPanelNoGroupBorders = "TabPanelNoGroupBorders",
			SkinTabPanelGroupMinimized = "TabPanelGroupMinimized",
			SkinTabPanelGroupMinimizedGlyph = "TabPanelGroupMinimizedGlyph",
			SkinTabPanelGroupBody = "TabPanelGroupBody",
			SkinTabPanelGroupCaption = "TabPanelGroupCaption",
			SkinTabPanelGroupCaption2014 = "TabPanelGroupCaption2014",
			SkinTabPanelGroupButton = "TabPanelGroupButton",
			SkinLargeButton = "LargeButton",
			SkinLargeSplitButton = "LargeSplitButton",
			SkinLargeSplitButton2 = "LargeSplitButton2",
			SkinButton = "Button",
			SkinSplitButton = "SplitButton",
			SkinSplitButton2 = "SplitButton2",
			SkinButtonArrow = "ButtonArrow",
			SkinButtonGroup = "ButtonGroup",
			SkinButtonGroupButton = "ButtonGroupButton",
			SkinButtonGroupSplitButton = "ButtonGroupSplitButton",
			SkinButtonGroupSplitButton2 = "ButtonGroupSplitButton2",
			SkinButtonGroupSeparator = "ButtonGroupSeparator",
			SkinStatusBarBackground = "StatusBarBackground",
			SkinStatusBarFormBackground = "StatusBarFormBackground",
			SkinStatusBarButton = "StatusBarButton",
			SkinStatusBarSeparator = "StatusBarSeparator",
			SkinQuickToolbarBelowBackground = "QuickToolbarBelowBackground",
			SkinQuickToolbarInCaptionBackground = "QuickToolbarInCaptionBackground",
			SkinQuickToolbarInCaptionBackground2010 = "QuickToolbarInCaptionBackground2010",
			SkinQuickToolbarAboveBackground = "QuickToolbarAboveBackground",
			SkinQuickToolbarAboveBackground2 = "QuickToolbarAboveBackground2",
			SkinQuickToolbarDropDown = "QuickToolbarDropDown",
			SkinQuickToolbarGlyph = "QuickToolbarGlyph",
			SkinQuickToolbarButtonGlyph = "QuickToolbarButtonGlyph",
			SkinGalleryHoverImageBackground = "GalleryHoverImageBackground",
			SkinPinUnPinButton = "PinUnPinButton";
		public static string
			SkinFormDecoratorFrameLeft = "FormDecoratorFrameLeft",
			SkinFormDecoratorFrameRight = "FormDecoratorFrameRight",
			SkinFormDecoratorFrameTop = "FormDecoratorFrameTop",
			SkinFormDecoratorFrameBottom = "FormDecoratorFrameBottom",
			SkinFormDecoratorGlowFrameLeft = "FormDecoratorGlowFrameLeft",
			SkinFormDecoratorGlowFrameRight = "FormDecoratorGlowFrameRight",
			SkinFormDecoratorGlowFrameTop = "FormDecoratorGlowFrameTop",
			SkinFormDecoratorGlowFrameBottom = "FormDecoratorGlowFrameBottom",
			SkinFormFrameLeft = "FormFrameLeft",
			SkinFormFrameRight = "FormFrameRight",
			SkinFormFrameLeftTop = "FormFrameLeftTop",
			SkinFormFrameRightTop = "FormFrameRightTop",
			SkinFormFrameLeftNoRibbon = "FormFrameLeftNoRibbon",
			SkinFormFrameRightNoRibbon = "FormFrameRightNoRibbon",
			SkinFormFrameBottom = "FormFrameBottom",
			SkinFormFrameBottomWithoutStatusBar = "FormFrameBottomWithoutStatusBar",
			SkinFormCaption = "FormCaption",
			SkinFormCaptionNoRibbon = "FormCaptionNoRibbon",
			SkinFormCaptionMinimized = "FormCaptionMinimized",
			SkinFormApplicationButton = "FormApplicationButton",
			SkinApplicationButton2010 = "ApplicationButton2010",
			SkinApplicationButtonContainerControl = "ApplicationButtonContainerControl";
		public static string
			SkinPopupGalleryImageBackground = "PopupGalleryImageBackground",
			SkinPopupGalleryBackground = "PopupGalleryBackground",
			SkinPopupGalleryPopupButton = "PopupGalleryButton",
			SkinPopupGalleryFilterPanel = "PopupGalleryFilterPanel",
			SkinPopupGalleryGroupCaption = "PopupGalleryGroupCaption",
			SkinPopupGallerySizerPanel = "PopupGallerySizerPanel",
			SkinPopupGalleryItemCaption = "PopupGalleryItemCaption",
			SkinPopupGalleryItemSubCaption = "PopupGalleryItemSubCaption",
			SkinPopupGallerySizerGrips = "PopupGallerySizerGrips",
			SkinPopupGalleryMenuSeparator = "PopupGalleryMenuSeparator",
			SkinGalleryImageBackground = "GalleryImageBackground",
			SkinGalleryPane = "GalleryPane",
			SkinGalleryButton = "GalleryButton",
			SkinGalleryButtonUp = "GalleryButtonUp",
			SkinGalleryButtonDown = "GalleryButtonDown",
			SkinGalleryButtonDropDown = "GalleryButtonDropDown",
			SkinGalleryButtonDropDownTouch = "GalleryButtonDropDownTouch",
			SkinAppMenuBackgroundTop = "AppMenuBackgroundTop",
			SkinAppMenuBackground = "AppMenuBackground",
			SkinAppMenuBackgroundBottom = "AppMenuBackgroundBottom",
			SkinAppMenuItemDescription = "AppMenuItemDescription",
			SkinAppMenuPane = "AppMenuPane",
			SkinBackstageViewControlLeftPane = "BackstageViewControlLeftPane",
			SkinBackstageViewControlButton = "BackstageViewControlButton",
			SkinBackstageViewControlTab = "BackstageViewControlTab",
			SkinBackstageViewSeparator = "BackstageViewSeparator",
			SkinBackstageViewImage = "BackstageViewImage",
			SkinBackstageViewControlTabArrow = "BackstageViewControlTabArrow",
			SkinBackstageViewBackButton = "BackstageViewBackButton",
			SkinMacStyleGalleryButtonDropDown = "MacStyleGalleryButtonDropDown",
			SkinMacTabHeaderBackground = "MacTabHeaderBackground",
			SkinMacTabHeaderPage = "MacTabHeaderPage",
			SkinMacContextTabHeaderPage = "MacContextTabHeaderPage";
		public static string
			OptPageGroupCaptionLocationTop = "PageGroupCaptionLocationTop",
			OptAllowDrawApplicationIcon = "AllowDrawApplicationIcon",
			OptRibbonFormCornerRadius = "RibbonFormCornerRadius",
			OptAppMenuFileLabelDescriptionColor = "AppMenuFileLabelDescriptionColor",
			OptIsColoredBackstageView = "IsColoredBackstageView",
			OptBackstageViewIndentBetweenItems = "BackstageViewIndentBetweenItems",
			OptBackstageViewButtonIndent = "BackstageViewButtonIndent",
			OptBackstageViewControlItemGlyphToCaptionIndent = "BackstageViewControlItemGlyphToCaptionIndent",
			OptPageCategoryTextShadowColor = "PageCategoryTextShadowColor",
			OptPageCategoryTextShadowOffset = "PageCategoryTextShadowOffset",
			OptPageGroupCaptionVerticalIndent = "PageGroupCaptionVerticalIndent",
			OptPageGroupCaptionHorizontalIndent = "PageGroupCaptionHorizontalIndent",
			OptPageGroupCaptionSeparatorHorizontalIndent = "PageGroupCaptionSeparatorHorizontalIndent",
			OptPageHeaderTopIndent = "PageHeaderTopIndent",
			OptBaseColor = "BaseColor",		  
			OptApplicationContainerControlTopIndent = "ApplicationContainerControlTopIndent",
			OptIndentBetweenPages = "IndentBetweenPages",
			OptIndentBetweenMacPages = "IndentBetweenMacPages",
			OptIndentBetweenCategoryPages = "IndentBetweenCategoryPages",
			OptApplicationMenuCornerRadius = "ApplicationMenuCornerRadius",
			OptDecoratorOffset = "DecoratorOffset",
			OptOffice2010ApplicationIconIndent = "Office2010ApplicationIconIndent",
			OptOffice2010ApplicationButtonLeftIndent = "Office2010ApplicationButtonLeftIndent",
			OptOffice2010ApplicationButtonRightIndent = "Office2010ApplicationButtonRightIndent",
			OptTabHeaderLeftXIndent = "LeftXIndent",
			OptTabHeaderRightXIndent = "RightXIndent",
			OptTabHeaderDownGrow = "TabHeaderDownGrow",
			OptIndentBetweenColumns = "IndentBetweenColumns",
			OptIndentBetweenRows = "IndentBetweenRows",
			OptIndentBetweenButtonGroups = "IndentBetweenButtonGroups",
			OptIndentBetweenButtonGroupRows = "IndentBetweenButtonGroupRows",
			OptIndentBetweenStatusBarColumns = "IndentBetweenStatusBarColumns",
			OptIndentBetweenPageGroups = "IndentBetweenPageGroups",
			OptColorButtonDisabled = "ButtonDisabled",
			OptColorEditorBackground = "EditorBackground",
			OptFormCaptionTextColor2 = "ForeColor2",
			OptFormCaptionTextColorDisabled = "ForeColorInactive",
			OptFormApplicationButtonLeftIndent = "FormApplicationButtonLeftIndent",
			OptFormApplicationButtonRightIndent = "FormApplicationButtonRightIndent",
			OptMacStyleGalleryCommandButtonTopIndent = "MacStyleGalleryCommandButtonTopIndent",
			OptUseRibbonItemsWithBackstageView = "UseRibbonItemsWithBackstageView",
			OptAllowGlassCaption = "AllowGlassCaption";
		public static string
			SkinKeyTipWindow = "KeyTipWindow";
		public static string
			SkinContextTabCategory = "ContextTabCategory",
			SkinContextTabCategory2 = "ContextTabCategory2",
			SkinContextTabCategorySeparator = "ContextTabCategorySeparator",
			SkinContextTabHeaderPage = "ContextTabHeaderPage",
			SkinContextTabHeaderPage2014 = "ContextTabHeaderPage2014",
			SkinContextTabHeaderPageMask = "ContextTabHeaderPageMask",
			SkinContextTabHeaderPageMask2014 = "ContextTabHeaderPageMask2014",
			SkinContextMacTabHeaderPageMask = "MacContextTabHeaderPageMask",
			SkinContextTabPanel = "ContextTabPanel",
			SkinContextTabPanel2014 = "ContextTabPanel2014",
			SkinContextTabPanelNoGroupBorders = "ContextTabPanelNoGroupBorders",
			SkinContextTabPanelBorder = "ContextTabPanelBorder";
		public static string 
			SkinTabPanelRegionRadius = "TabPanelRegionRadius",
			SkinTabPanelBottomIndent = "TabPanelBottomIndent", 
			SkinTabPanelRightIndent = "TabPanelRightIndent",
			SkinTabPanelTopIndent = "TabPanelTopIndent", 
			SkinTabPanelLeftIndent = "TabPanelLeftIndent", 
			SkinTabPanelGroupRegionRadius = "TabPanelGroupRegionRadius",
			SkinTabPanelGroupBottomIndent = "TabPanelGroupBottomIndent", 
			SkinTabPanelGroupRightIndent = "TabPanelGroupRightIndent",
			SkinTabPanelGroupTopIndent = "TabPanelGroupTopIndent", 
			SkinTabPanelGroupLeftIndent = "TabPanelGroupLeftIndent";
		public static string
			SkinLeftScrollButton = "LeftScrollButton",
			SkinRightScrollButton = "RightScrollButton";
		public static string
			SkinForeColorInCaptionQuickAccessToolbar = "ForeColorInCaptionQuickAccessToolbar",
			SkinForeColorInTopQuickAccessToolbar = "ForeColorInTopQuickAccessToolbar",
			SkinForeColorInBottomQuickAccessToolbar = "ForeColorInBottomQuickAccessToolbar",
			SkinForeColorInPageHeader = "ForeColorInPageHeader",
			SkinForeColorDisabledInCaptionQuickAccessToolbar = "ForeColorDisabledInCaptionQuickAccessToolbar",
			SkinForeColorDisabledInTopQuickAccessToolbar = "ForeColorDisabledInTopQuickAccessToolbar",
			SkinForeColorDisabledInBottomQuickAccessToolbar = "ForeColorDisabledInBottomQuickAccessToolbar",
			SkinForeColorDisabledInPageHeader = "ForeColorDisabledInPageHeader",
			SkinForeColorInBackstageViewTitle = "ForeColorInBackstageViewTitle",
			SkinForeColorDisabledInBackstageView = "ForeColorDisabledInBackstageView";
		public static string
			SkinFormButtonMaximize = FormSkins.SkinFormButtonMaximize,
			SkinFormButtonMinimize = FormSkins.SkinFormButtonMinimize,
			SkinFormButtonRestore = FormSkins.SkinFormButtonRestore,
			SkinFormButtonClose = FormSkins.SkinFormButtonClose,
			SkinFormButtonFullScreen = FormSkins.SkinFormButtonFullScreen;
		public static string
			SkinRadialMenuColor = "RadialMenuColor";
		public static Skin GetSkin(ISkinProvider provider) {
			return SkinManager.Default.GetSkin(SkinProductId.Ribbon, provider);
		}
	}
	public class PrintingSkins {
		public static string
			SkinBackgroundPreview = "PreviewBackground",
			SkinBorderPage = "PageBorder";
		public static string
			OptForeColor = "ForeColor",
			OptPageVerticalIndent = "PageVerticalIndent",
			OptPageHorizontalIndent = "PageHorizontalIndent";
		public static Skin GetSkin(ISkinProvider provider) {
			return SkinManager.Default.GetSkin(SkinProductId.Printing, provider);
		}
	}
	public class ReportsSkins {
		public static string 
			SkinBandHeaderLevel0 = "BandHeaderLevel0",
			SkinBandHeaderLevel1 = "BandHeaderLevel1",
			SkinBandHeaderLevel2 = "BandHeaderLevel2",
			SkinBandButtonLevel0 = "BandButtonLevel0",
			SkinBandButtonLevel1 = "BandButtonLevel1",
			SkinBandButtonLevel2 = "BandButtonLevel2",
			SkinCornerPanel = "CornerPanel",
			SkinPopupFormCaption = "PopupFormCaption",
			SkinPopupFormBackground = "PopupFormBackground",
			SkinRulerBackground = "RulerBackground",
			SkinSmartTag = "SmartTag",
			SkinRulerShadow = "RulerShadow",
			SkinRulerSelection = "RulerSelection",
			SkinRulerSection = "RulerSection",
			SkinRulerRightMargin = "RulerRightMargin",
			SkinBandButtonSelected = "BandButtonSelected",
			SkinBandHeaderSelected = "BandHeaderSelected",
			SkinSlider = "Slider",
			SkinSliderSelected = "SliderSelected",
			SkinComponentTrayBackground = "ComponentTrayBackground",
			SkinScriptControl = "ScriptControl",
			SkinContrastCheckButton = "ContrastCheckButton";
		public static Skin GetSkin(ISkinProvider provider) {
			return SkinManager.Default.GetSkin(SkinProductId.Reports, provider); 
		}
	}
	public partial class EditorsSkins {
		public static string SkinTileItemSelected = "TileItemSelected",
			SkinTileItemChecked = "TileItemChecked",
			OptTileItemBorderIndent = "Indent",
			OptRangeControlRuleColor = "RuleColor",
			OptRangeControlBorderColor = "BorderColor",
			OptRangeControlBackColor = "BackColor",
			OptSwitchWidth = "SwitchWidth",
			OptTextMargin = "TextMargin",
			OptRangeControlOutOfRangeColorMask = "OutOfRangeColorMask",
			OptRangeControlScrollAreaColor = "ScrollAreaColor",
			OptRangeControlViewPortPreviewColor = "ViewPortPreviewColor",
			OptRangeControlRangePreviewColor = "RangePreviewColor",
			OptRangeControlScrollAreaHeight = "ScrollAreaHeight",
			OptRangeControlLabelColor = "LabelColor",
			OptRangeControlElementColor = "ElementColor",
			OptRangeControlHeaderPadding = "HeaderPadding",
			OptRangeControlDefaultElementColor = "DefaultElementColor",
			OptRangeControlElementBaseColor = "ElementBaseColor",
			OptRangeControlElementForeColor = "ElementForeColor",
			OptRangeControlElementFontSize = "ElementFontSize",
			OptButtonPanelNormalForeColor = "ButtonPanelNormalForeColor";
		public static string
			SkinRangeControlSizingGlyph = "RangeControlSizingGlyph",
			SkinRangeControlBorder = "RangeControlBorder",
			SkinRangeControlLeftThumb = "RangeControlLeftThumb",
			SkinRangeControlRightThumb = "RangeControlRightThumb",
			SkinRangeControlRulerHeader = "RangeControlRulerHeader",
			SkinSliderLeftArrow = "SliderLeftArrow",
			SkinSliderRightArrow = "SliderRightArrow",
			SkinSliderUpArrow = "SliderUpArrow",
			SkinSliderDownArrow = "SliderDownArrow",
			SkinNavigator = "Navigator",
			SkinPopupFormBorder = "PopupFormBorder",
			SkinEditorButton = "EditorButton",
			SkinCalendarNavigationButton = "CalendarNavigationButton",
			SkinCalendarNavigationButtonForeColor = "CalendarNavigationButtonForeColor",
			SkinCalendarNavigationButtonHighlightedForeColor = "CalendarNavigationButtonHighlightedForeColor",
			SkinCalendarNavigationButtonPressedForeColor = "CalendarNavigationButtonPressedForeColor",
			SkinCalendarNormalCellColor = "CalendarNormalCellColor",
			SkinCalendarHighlightedCellColor = "CalendarHighlightedCellColor",
			SkinCalendarPressedCellColor = "CalendarPressedCellColor",
			SkinCalendarDisabledCellColor = "CalendarDisabledCellColor",
			SkinCalendarSelectedCellColor = "CalendarSelectedCellColor",
			SkinCalendarWeekNumberForeColor = "CalendarWeekNumberForeColor",
			SkinCalendarWeekDayForeColor = "CalendarWeekDayForeColor",
			SkinCalendarHeaderForeColor = "CalendarHeaderForeColor",
			SkinCalendarHeaderHighlightedForeColor = "CalendarHeaderHighlightedForeColor",
			SkinCalendarHeaderPressedForeColor = "CalendarHeaderPressedForeColor",
			SkinCalendarSpecialCellColor = "CalendarSpecialCellColor",
			SkinCalendarHighlightedSpecialCellColor = "CalendarHighlightedSpecialCellColor",
			SkinCalendarPressedSpecialCellColor = "CalendarPressedSpecialCellColor",
			SkinCalendarSelectedSpecialCellColor = "CalendarSelectedSpecialCellColor",
			SkinCalendarInactiveCellColor = "CalendarInactiveCellColor",
			SkinCalendarInactiveSpecialCellColor = "CalendarInactiveSpecialCellColor",
			SkinCalendarDisabledSpecialCellColor = "CalendarDisabledSpecialCellColor",
			SkinCalendarTodayCellColor = "CalendarTodayCellColor",
			SkinCalendarHolidayCellColor = "CalendarHolidayCellColor",
			SkinCalendarTodayFrameColor = "CalendarTodayFrameColor",
			SkinCalendarMonthHeaderFontSize = "CalendarMonthHeaderFontSize",
			SkinComboButton = "ComboButton",
			SkinNavigatorButton = "NavigatorButton",
			SkinCloseButton = "CloseButton",
			SkinSearchButton = "SearchButton",
			SkinClearButton = "ClearButton",
			SkinCheckBox = "CheckBox",
			SkinTextBox = "TextBox",
			SkinTokenEdit = "TokenEdit",
			SkinTokenEditCloseButton = "TokenEditCloseButton",
			SkinToggleSwitch = "ToggleSwitch",
			SkinToggleSwitchThumb = "ToggleSwitchThumb",
			SkinRadioButton = "RadioButton",
			SkinSpinDown = "SpinDown",
			SkinSpinUp = "SpinUp",
			SkinSpinLeft = "SpinLeft",
			SkinSpinRight = "SpinRight",
			SkinProgressBorder = "ProgressBorder",
			SkinProgressBorderVert = "ProgressBorderVert",
			SkinProgressChunk = "ProgressChunk",
			SkinProgressChunkVert = "ProgressChunkVert",
			SkinProgressFlowIndicator = "ProgressFlowIndicator",
			SkinProgressFlowIndicatorVert = "ProgressFlowIndicatorVert",
			SkinTrackBarTickLine = "TrackBarTickLine",
			SkinTrackBarThumb = "TrackBarThumb",
			SkinTrackBarThumbBoth = "TrackBarThumbBoth",
			SkinTrackBarTrack = "TrackBarTrack",
			SkinRangeTrackBarLeftThumb = "RangeTrackBarLeftThumb",
			SkinRangeTrackBarRightThumb = "RangeTrackBarRightThumb",
			SkinRangeTrackBarThumbBoth = "RangeTrackBarThumbBoth",
			SkinDateEditClockFace = "ClockFace",
			SkinDateEditClockGlass = "ClockGlass",
			SkinZoomTrackBarZoomInButton = "ZoomTrackBarZoomInButton",
			SkinZoomTrackBarZoomOutButton = "ZoomTrackBarZoomOutButton",
			SkinZoomTrackBarMiddleLine = "ZoomTrackBarMiddleLine",
			SkinProgressBarEmptyTextColor = "ProgressBarEmptyTextColor",
			SkinProgressBarFilledTextColor = "ProgressBarFilledTextColor",
			SkinFilterControlGroupOperatorTextColor = "FilterControlGroupOperatorTextColor",
			SkinFilterControlFieldNameTextColor = "FilterControlFieldNameTextColor",
			SkinFilterControlOperatorTextColor = "FilterControlOperatorTextColor",
			SkinFilterControlValueTextColor = "FilterControlValueTextColor",
			SkinFilterControlEmptyValueTextColor = "FilterControlEmptyValueTextColor",
			SkinHyperlinkTextColor = "HyperLinkTextColor",
			SkinHyperlinkTextColorPressed = "HyperLinkTextColorPressed",
			SkinHyperlinkTextColorVisited = "HyperLinkTextColorVisited",
			SkinCalcEditOperationTextColor = "CalcEditOperationTextColor",
			SkinCalcEditDigitTextColor = "CalcEditDigitTextColor",
			SkinSplashForm = "SplashForm",
			SkinBeakFormBorderColor = "BeakFormBorderColor",
			SkinRatingIndicator = "RatingIndicator",
			SkinRatingIndicatorColor = "RatingIndicatorColor",
			SkinContextItemCheckBox = "ContextItemCheckBox",
			SkinContextItemRatingIndicator = "ContextItemRatingIndicator",
			SkinContextItemTrackBarTrack = "ContextItemTrackBarTrack",
			SkinContextItemTrackBarThumb = "ContextItemTrackBarThumb",
			SkinContextItemTrackBarMiddleLine = "ContextItemTrackBarMiddleLine",
			SkinContextItemZoomInButton = "ContextItemZoomInButton",
			SkinContextItemZoomOutButton = "ContextItemZoomOutButton",
			SkinButtonPanel = "ButtonPanel",
			SkinFormatRuleDirectional = "FormatRuleDirectional",
			SkinFormatRuleIndicators = "FormatRuleIndicators",
			SkinFormatRuleRatings = "FormatRuleRatings",
			SkinFormatRuleShapes = "FormatRuleShapes",
			SkinFormatRuleColorScale2Set1ColorBegin = "FormatColorScale2Set1ColorBegin",
			SkinFormatRuleColorScale2Set1ColorEnd = "FormatColorScale2Set1ColorEnd",
			SkinFormatRuleColorScale2Set2ColorBegin = "FormatColorScale2Set2ColorBegin",
			SkinFormatRuleColorScale2Set2ColorEnd = "FormatColorScale2Set2ColorEnd",
			SkinFormatRuleColorScale2Set3ColorBegin = "FormatColorScale2Set3ColorBegin",
			SkinFormatRuleColorScale2Set3ColorEnd = "FormatColorScale2Set3ColorEnd",
			SkinFormatRuleColorScale2Set4ColorBegin = "FormatColorScale2Set4ColorBegin",
			SkinFormatRuleColorScale2Set4ColorEnd = "FormatColorScale2Set4ColorEnd",
			SkinFormatRuleColorScale3Set1ColorBegin = "FormatColorScale3Set1ColorBegin",
			SkinFormatRuleColorScale3Set1ColorMid = "FormatColorScale3Set1ColorMid",
			SkinFormatRuleColorScale3Set1ColorEnd = "FormatColorScale3Set1ColorEnd",
			SkinFormatRuleColorScale3Set2ColorBegin = "FormatColorScale3Set2ColorBegin",
			SkinFormatRuleColorScale3Set2ColorMid = "FormatColorScale3Set2ColorMid",
			SkinFormatRuleColorScale3Set2ColorEnd = "FormatColorScale3Set2ColorEnd",
			SkinFormatRuleColorScale3Set3ColorBegin = "FormatColorScale3Set3ColorBegin",
			SkinFormatRuleColorScale3Set3ColorMid = "FormatColorScale3Set3ColorMid",
			SkinFormatRuleColorScale3Set3ColorEnd = "FormatColorScale3Set3ColorEnd",
			SkinFormatRuleColorScale3Set4ColorBegin = "FormatColorScale3Set4ColorBegin",
			SkinFormatRuleColorScale3Set4ColorMid = "FormatColorScale3Set4ColorMid",
			SkinFormatRuleColorScale3Set4ColorEnd = "FormatColorScale3Set4ColorEnd",
			SkinFormatRuleDataBarSet1Positive = "FormatRuleDataBarSet1Positive",
			SkinFormatRuleDataBarSet2Positive = "FormatRuleDataBarSet2Positive",
			SkinFormatRuleDataBarSet3Positive = "FormatRuleDataBarSet3Positive",
			SkinFormatRuleDataBarSet4Positive = "FormatRuleDataBarSet4Positive",
			SkinFormatRuleDataBarSet1Negative = "FormatRuleDataBarSet1Negative",
			SkinFormatRuleDataBarSet2Negative = "FormatRuleDataBarSet2Negative",
			SkinFormatRuleDataBarSet3Negative = "FormatRuleDataBarSet3Negative",
			SkinFormatRuleDataBarSet4Negative = "FormatRuleDataBarSet4Negative",
			SkinFormatRuleDataBarAxisColorSet1 = "FormatRuleDataBarAxisColorSet1",
			SkinFormatRuleDataBarAxisColorSet2 = "FormatRuleDataBarAxisColorSet2",
			SkinFormatRuleDataBarAxisColorSet3 = "FormatRuleDataBarAxisColorSet3",
			SkinFormatRuleDataBarAxisColorSet4 = "FormatRuleDataBarAxisColorSet4";
		public static string OptIsSingleLineBorder = "IsSingleLineBorder",
			OptNewCalendarPadding = "CalendarPadding",
			OptNewCalendarTextIndent = "TextIndent",
			OptNewCalendarIndent = "CalendarIndent";
		public static Skin GetSkin(ISkinProvider provider) { return SkinManager.Default.GetSkin(SkinProductId.Editors, provider); }
	}
	public class VGridSkins {
		public static string
			SkinGridLine = "GridLine",
			SkinCategory = "Category",
			SkinCategoryButton = "CategoryButton",
			SkinRow = "Row",
			SkinBorder = "Border",
			SkinBandBorder = "BandBorder",
			SkinRowDisabled = "RowDisabled",
			SkinRecordValue = "RecordValue",
			SkinFixedLine = "FixedLine",
			SkinRowReadOnly = "RowReadOnly",
			SkinRecordValueReadOnly = "RecordValueReadOnly";
		public static Skin GetSkin(ISkinProvider provider) {
			return SkinManager.Default.GetSkin(SkinProductId.VGrid, provider); 
		}
	}
	public class GridSkins {
		public static string
			SkinViewCaption = "ViewCaption",
			SkinIndicator = "Indicator",
			SkinBorder = "Border",
			SkinIndicatorImages = "IndicatorImages",
			SkinHeader = "Header",
			SkinHeaderSpecial = "HeaderSpecial",
			SkinHeaderLeft = "HeaderLeft",
			SkinHeaderRight = "HeaderRight",
			SkinFooterPanel = "FooterPanel",
			SkinFooterCell = "FooterCell",
			SkinPlusMinus = "PlusMinus",
			SkinPlusMinusEx = "PlusMinusEx",
			SkinSortShape = "SortShape",
			SkinSmartFilterButton = "SmartFilterButton",
			SkinFieldHeaderButton = "FieldHeaderButton",
			SkinFilterButton = "FilterButton",
			SkinFilterButtonActive = "FilterButtonActive",
			SkinGridGroupPanel = "GridGroupPanel",
			SkinGridFilterPanel = "GridFilterPanel",
			SkinGridSpecialRowIndent = "GridSpecialRowIndent",
			SkinGridPreview = "GridPreview",
			SkinGridTopNewRow = "GridTopNewRow",
			SkinGridOddRow = "GridOddRow",
			SkinGridEvenRow = "GridEvenRow",
			SkinGridRow = "GridRow",
			SkinGridCell = "GridCell",
			SkinGridLine = "GridLine",
			SkinGridFixedLine = "GridFixedLine",
			SkinGridGroupRow = "GroupRow",
			SkinGridGroupLevel = "GroupRowLevel",
			SkinGridEmptyArea = "GridEmptyArea",
			SkinCardEmptyArea = "CardEmptyArea",
			SkinCardSeparator = "CardSeparator",
			SkinCardOpenButton = "CardOpenButton",
			SkinCardCloseButton = "CardCloseButton",
			SkinWinExplorerViewItem = "WinExplorerViewItem",
			SkinWinExplorerViewGroupHeader = "WinExplorerViewGroupHeader",
			SkinWinExplorerViewGroupCaptionLine = "WinExplorerViewGroupCaptionLine",
			SkinWinExplorerViewItemSeparator = "WinExplorerViewItemSeparator",
			SkinWinExplorerViewGroupOpenButton = "WinExplorerViewGroupOpenButton",
			SkinWinExplorerViewGroupCloseButton = "WinExplorerViewGroupCloseButton",
			SkinCard = "Card",
			SkinCardSelected = "CardSelected",
			SkinCardCaption = "CardCaption",
			SkinCardCaptionSelected = "CardCaptionSelected",
			SkinCardCaptionHideSelection = "CardCaptionHideSelection",
			SkinLayoutViewCard = "LayoutViewCard",
			SkinLayoutViewCardSelected = "LayoutViewCardSelected",
			SkinLayoutViewCardHideSelection = "LayoutViewCardHideSelection",
			SkinLayoutViewCardCaption = "LayoutViewCardCaption",
			SkinLayoutViewCardCaptionSelected = "LayoutViewCardCaptionSelected",
			SkinLayoutViewCardCaptionHideSelection = "LayoutViewCardCaptionHideSelection",
			SkinLayoutViewCardExpandButton = "LayoutViewCardExpandButton",
			SkinLayoutViewCardBorder = "LayoutViewCardBorder",
			SkinLayoutViewCardBorderSelected = "LayoutViewCardBorderSelected",
			SkinLayoutViewCardBorderHideSelection = "LayoutViewCardBorderHideSelection",
			SkinLayoutViewCardNoBorder = "LayoutViewCardNoBorder",
			SkinLayoutViewCardNoBorderSelected = "LayoutViewCardNoBorderSelected",
			SkinLayoutViewCardNoBorderHideSelection = "LayoutViewCardNoBorderHideSelection";
		public static string
			OptCardCaptionExpandButtonIndent = "CardCaptionExpandButtonIndent",
			OptHeaderRequireHorzOffset = "HeaderRequireHorzOffset",
			OptHeaderRequireVertOffset = "HeaderRequireVertOffset",
			OptBorderIndent = "BorderIndent",
			OptFilterButtonOffsetX = "OffsetX",
			OptFilterButtonOffsetY = "OffsetY",
			OptColorCardFocusedFieldForeColor = "CardFocusedFieldForeColor",
			OptColorCardFocusedFieldBackColor = "CardFocusedFieldBackColor",
			OptColorLayoutCardFocusedFieldForeColor = "LayoutCardFocusedFieldForeColor",
			OptColorLayoutCardFocusedFieldBackColor = "LayoutCardFocusedFieldBackColor",
			OptShowTreeLine = "ShowTreeLine",
			OptFindPanelBorder = "FindPanelBorder",
			OptDrawRightColumnWithCenterHeader = "DrawRightColumnWithCenterHeader",
			OptShowVerticalLines = "ShowVerticalLines",
			OptShowPreviewRowLines = "ShowPreviewRowLines",
			OptColorPreviewFocusedForeColor = "PreviewRowFocusedForeColor",
			OptShowHorizontalLines = "ShowHorizontalLines",
			OptGridGroupDrawMode = "GroupDrawMode", 
			OptItemListViewItemHorizontalIndent = "ItemListViewItemHorizontalIndent",
			OptWinExplorerViewItemVerticalIndent = "WinExplorerViewItemVerticalIndent",
			OptWinExplorerViewGroupToItemIndent = "WinExplorerViewGroupToItemIndent",
			OptWinExplorerViewImageToTextIndent = "WinExplorerViewImageToTextIndent",
			OptWinExplorerViewItemWidth = "WinExplorerViewItemWidth",
			OptWinExplorerViewGroupCaptionButtonIndent = "WinExplorerViewGroupCaptionButtonIndent",
			OptWinExplorerViewGroupIndent = "WinExplorerViewGroupIndent",
			OptWinExplorerViewItemImageMargins = "WinExplorerViewItemImageMargins",
			OptWinExplorerViewItemCheckBoxMargins = "WinExplorerViewItemCheckBoxMargins";
		public static Skin GetSkin(ISkinProvider provider) {
			return SkinManager.Default.GetSkin(SkinProductId.Grid, provider); 
		}
	}
	public class NavBarSkins {
		public static string 
			SkinGroupOpenButton = "GroupOpenButton",
			SkinGroupCloseButton= "GroupCloseButton",
			SkinItem = "Item",
			SkinBackground = "Background",
			SkinGroupHeader = "GroupHeader",
			SkinGroupFooter = "GroupFooter",
			SkinGroupClient = "GroupClient",
			SkinScrollUp = "ScrollUpButton",
			SkinScrollDown="ScrollDownButton",
			SkinGroupClientLinksStrip = "GroupClientLinksStrip";
		public static Skin GetSkin(ISkinProvider provider) {
			return SkinManager.Default.GetSkin(SkinProductId.NavBar, provider); 
		}
	}
	public class NavPaneSkins {
		public static string 
			SkinCaption = "Caption",
			SkinItem = "Item",
			SkinItemSelected = "ItemSelected",
			SkinExpandButton = "ExpandButton",
			SkinCollapseButton = "CollapseButton",
			SkinCollapsedGroupClient = "CollapsedGroupClient",
			SkinCollapsedGroupClientWithBorder = "CollapsedGroupClientWithBorder",
			SkinNavPaneFormBorder = "NavPaneFormBorder",
			SkinNavPaneFormSizeGrip = "NavPaneFormSizeGrip",
			SkinNavPaneFormLeftSizeGrip = "NavPaneFormLeftSizeGrip",
			SkinGroupButton = "GroupButton",
			SkinGroupButtonSelected = "GroupButtonSelected",
			SkinGroupClient = "GroupClient",
			SkinGroupClientWithBorder = "GroupClientWithBorder",
			SkinOverflowPanel = "OverflowPanel",
			SkinOverflowPanelItem = "OverflowPanelItem",
			SkinOverflowPanelExpandItem = "OverflowPanelExpandItem",
			SkinSplitter = "Splitter",
			SkinOfficeNavigationBar = "OfficeNavigationBar",
			SkinOfficeNavigationBarItem = "OfficeNavigationBarItem",
			SkinOfficeNavigationBarSkinningItem = "OfficeNavigationBarSkinningItem",
			SkinScrollUp = "ScrollUpButton",
			SkinScrollDown="ScrollDownButton";
		public static string 
			OptGroupRequireOffset = "GroupRequireOffset",
			OptGroupClientTopIndent = "GroupClientTopIndent",
			OptFontSize = "FontSize";
		public static Skin GetSkin(ISkinProvider provider) {
			return SkinManager.Default.GetSkin(SkinProductId.NavPane, provider); 
		}
	}
	public class AccordionControlSkins {
		public static string
		  SkinBackground = "Background",
		  SkinItem = "Item",
		  SkinRootGroup = "RootGroup",
		  SkinGroup = "Group",
		  SkinGroupOpenButton = "GroupOpenButton",
		  SkinGroupCloseButton = "GroupCloseButton",
		  SkinRootGroupOpenButton = "RootGroupOpenButton",
		  SkinRootGroupCloseButton = "RootGroupCloseButton",
		  SkinSearchControl = "SearchControl",
		  SkinExpandButton = "ExpandButton",
		  SkinSeparator = "Separator",
		  SkinMinimizedElement = "MinimizedElement",
		  OptGlyphToTextIndent = "GlyphToTextIndent";
		public static Skin GetSkin(ISkinProvider provider) {
			return SkinManager.Default.GetSkin(SkinProductId.AccordionControl, provider);
		}
	}
	public class DockingSkins {
		public static string
			SkinTabHeaderBackground = "TabHeaderBackground",
			SkinTabHeader = "TabHeader",
			SkinTabHeaderHideBar = "TabHeaderHideBar",
			SkinTabHeaderLine = "TabHeaderLine",
			SkinHideBar = "HideBar",
			SkinHideBarBottom = "HideBarBottom",
			SkinHideBarLeft = "HideBarLeft",
			SkinHideBarRight = "HideBarRight",
			SkinDockWindowButton = "DockWindowButton",
			SkinDockWindow = "DockWindowCaption",
			SkinDockWindowBorder = "DockWindowBorder",
			SkinFloatingWindow = "FloatingWindowCaption",
			SkinFloatingWindowBorder = "FloatingWindowBorder",
			SkinUnFocusCaptionColor = "UnFocusCaptionColor",
			SkinNativeMdiViewBackground = "NativeMdiViewBackground",
			SkinDocumentGroupTabHeader = "DocumentGroupTabHeader",
			SkinDocumentGroupTabPane = "DocumentGroupTabPane",
			SkinDocumentGroupTabHeaderButton = "DocumentGroupTabHeaderButton",
			SkinDocumentGroupTabPageButton = "DocumentGroupTabPageButton",
			SkinDocumentGroupActiveTabPageButton = "DocumentGroupActiveTabPageButton",
			SkinDocumentGroupInactiveTabPageButton = "DocumentGroupInactiveTabPageButton",
			SkinDocumentSelector = "DocumentSelector",
			SkinDocumentSelectorHeader = "DocumentSelectorHeader",
			SkinDocumentSelectorFooter = "DocumentSelectorFooter",
			SkinFloatDocument = "FloatDocumentCaption",
			SkinFloatDocumentBorder = "FloatDocumentBorder",
			SkinCenterDockGuidePanel = "CenterDockGuidePanel",
			SkinSideDockGuide = "SideDockGuide",
			SkinCenterDockGuide = "CenterDockGuide";
		public static string
			HideBarTextColor = "HideBarTextColor",
			HideBarTextColorActive = "HideBarTextColorActive",
			TabHeaderLineHGrow = "TabHeaderLineHGrow",
			DocumentGroupHeaderTextColor = "DocumentGroupHeaderTextColor",
			DocumentGroupHeaderTextColorActive = "DocumentGroupHeaderTextColorActive",
			DocumentGroupHeaderTextColorGroupInactive = "DocumentGroupHeaderTextColorGroupInactive",
			DocumentGroupHeaderTextColorHot = "DocumentGroupHeaderTextColorHot",
			DocumentGroupHeaderTextColorDisabled = "DocumentGroupHeaderTextColorDisabled",
			DocumentGroupHeaderButtonTextColor = "DocumentGroupHeaderButtonTextColor",
			DocumentGroupHeaderButtonTextColorHot = "DocumentGroupHeaderButtonTextColorHot",
			DocumentSelectorForeColor = "DocumentSelectorForeColor",
			DocumentSelectorHorizontalContentMargin = "DocumentSelectorHorizontalContentMargin",
			DocumentSelectorVerticalContentMargin = "DocumentSelectorVerticalContentMargin",
			DocumentGroupBackColor = "DocumentGroupBackColor",
			DocumentGroupRootMargin = "DocumentGroupRootMargin";
		public static Skin GetSkin(ISkinProvider provider) {
			return SkinManager.Default.GetSkin(SkinProductId.Docking, provider); 
		}
	}
	public class MetroUISkins {
		public static string
			SkinActionsBar = "ActionsBar",
			SkinActionsBarButton = "ActionsBarButton",
			SkinHeader = "Header",
			SkinItemHeader = "ItemHeader",
			SkinSlideGroup = "SlideGroup",
			SkinSlideGroupItemHeader = "SlideGroupItemHeader",
			SkinPageGroup = "PageGroup",
			SkinPageGroupItemHeader = "PageGroupItemHeader",
			SkinPageGroupItemHeaderButton = "PageGroupItemHeaderButton",
			SkinTabbedGroup = "TabbedGroup",
			SkinTabbedGroupItemHeader = "TabbedGroupItemHeader",
			SkinTabbedGroupItemHeaderButton = "TabbedGroupItemHeaderButton",
			SkinTabbedGroupItemHeaderTile = "TabbedGroupItemHeaderTile",
			SkinSplitGroup = "SplitGroup",
			SkinTileContainer = "TileContainer",
			SkinOverviewContainer = "OverviewContainer",
			SkinPage = "Page",
			SkinOverviewTile = "OverviewTile"
			;
		public static string
			HeaderButtonToTextInterval = "HeaderButtonToTextInterval",
			HeaderButtonVerticalOffset = "HeaderButtonVerticalOffset",
			HeaderButtonHorizontalOffset = "HeaderButtonHorizontalOffset",
			SlideGroupInterval = "SlideGroupInterval",
			PageGroupButtonsInterval = "PageGroupButtonsInterval",
			TabbedGroupButtonsInterval = "TabbedGroupButtonsInterval"
			;
		public static string
			ActionsBarButtonHotColor = "ActionsBarButtonHotColor",
			ActionsBarButtonPressedColor = "ActionsBarButtonPressedColor",
			HeaderButtonHotColor = "HeaderButtonHotColor",
			HeaderButtonPressedColor = "HeaderButtonPressedColor",
			PageGroupButtonHotColor = "PageGroupButtonHotColor",
			PageGroupButtonPressedColor = "PageGroupButtonPressedColor",
			TabbedGroupButtonHotColor = "TabbedGroupButtonHotColor",
			TabbedGroupButtonPressedColor = "TabbedGroupButtonPressedColor",
			TabbedGroupTileHotColor = "TabbedGroupTileHotColor",
			TabbedGroupTilePressedColor = "TabbedGroupTilePressedColor",
			OverviewTileHotColor = "OverviewTileHotColor",
			OverviewTilePressedColor = "OverviewTilePressedColor"
			;
		public static Skin GetSkin(ISkinProvider provider) {
			return SkinManager.Default.GetSkin(SkinProductId.MetroUI, provider);
		}
	}
	public class TabSkinProperties {
		public static string
			TabHeaderTextColor = "TabHeaderTextColor",
			TabHeaderTextColorActive = "TabHeaderTextColorActive",
			TabHeaderTextColorHot = "TabHeaderTextColorHot",
			TabHeaderTextColorDisabled = "TabHeaderTextColorDisabled",
			TabHeaderTextColorTabInactive = "TabHeaderTextColorTabInactive",
			TabHeaderButtonTextColor = "TabHeaderButtonTextColor",
			TabHeaderButtonTextColorHot = "TabHeaderButtonTextColorHot",
			SelectedHeaderUpGrow = "SelectedHeaderUpGrow",
			SelectedHeaderHGrow = "SelectedHeaderHGrow",
			SelectedHeaderDownGrow = "SelectedHeaderDownGrow",
			SelectedHeaderDownGrowBottomRight = "SelectedHeaderDownGrowBottomRight",
			UpperHeaderDownGrow = "UpperHeaderDownGrow",
			UpperHeaderDownGrowBottomRight = "UpperHeaderDownGrowBottomRight",
			HeaderDownGrow = "HeaderDownGrow",
			HeaderDownGrowBottomRight = "HeaderDownGrowBottomRight",
			RowIndentNear = "RowIndentNear",
			RowIndentFar = "RowIndentFar",
			LeftContentIndent = "LeftContentIndent",
			BottomContentIndent = "BottomContentIndent",
			RightContentIndent = "RightContentIndent",
			ColoredTabAdjustForeColor = "ColoredTabAdjustForeColor",
			ColoredTabBaseForeColor = "ColoredTabBaseForeColor",
			BorderToTabHeadersIndent = "BorderToTabHeadersIndent",
			ColoredTabClipMargin = "ColoredTabClipMargin";
	}
	public class NavigationPaneSkins {
		public static string
		  SkinPaneButtonsBackground = "PaneButtonsBackground",
		  SkinTabPaneButtonsBackground = "TabPaneButtonsBackground",
		  SkinPaneButtonRegularState = "PaneButtonRegularState",
		  SkinTabPaneButton = "TabPaneButton",
		  SkinPaneButtonCollapsedState = "PaneButtonCollapsedState",
		  SkinPaneButtonArrow = "PaneButtonArrow",
		  SkinTabPaneButtonArrow = "TabPaneButtonArrow",
		  SkinPageCaption = "PageCaption",
		  SkinPageBackground = "PageBackground",
		  SkinPageButtons = "PageButton";
		public static string
			SelectedPageOverlapValue = "SelectedPageOverlapValue",
			SelectedPageExpandValue = "SelectedPageExpandValue",
			SelectedTabPageOverlapValue = "SelectedTabPageOverlapValue",
			SelectedTabPageExpandValue = "SelectedTabPageExpandValue";
		public static string
			DefaultNavigationPageButtonNormalColor = "DefaultNavigationPageButtonNormalColor",
			DefaultNavigationPageButtonHotColor = "DefaultNavigationPageButtonHotColor",
			DefaultNavigationPageButtonPressedColor = "DefaultNavigationPageButtonPressedColor";
		public static string
			PaneButtonRegularStateNormalColor = "PaneButtonRegularStateNormalColor",
			PaneButtonRegularStateHotColor = "PaneButtonRegularStateHotColor",
			PaneButtonRegularStatePressedColor = "PaneButtonRegularStatePressedColor",
			PaneButtonCollapsedStateNormalColor = "PaneButtonCollapsedStateNormalColor",
			PaneButtonCollapsedStateHotColor = "PaneButtonCollapsedStateHotColor";
		public static Skin GetSkin(ISkinProvider provider) {
			return SkinManager.Default.GetSkin(SkinProductId.NavigationPane, provider);
		}
	}
	public class BarSkins {
		public static string
			SkinDock = "Dock",
			SkinBar = "Bar",
			SkinBarVertical = "BarVertical",
			SkinBarCustomize = "BarCustomize",
			SkinMainMenuCustomize = "MainMenuCustomize",
			SkinMainMenuDrag = "MainMenuDrag",
			SkinBarCustomizeVertical = "BarCustomizeVertical",
			SkinBarDrag = "BarDrag",
			SkinBarDragVertical = "BarDragVertical",
			SkinBarSeparator = "BarSeparator",
			SkinBarVerticalSeparator = "BarVerticalSeparator",
			SkinPopupMenu = "PopupMenu",
			SkinPopupMenuSeparator = "PopupMenuSeparator",
			SkinPopupMenuSideStrip = "PopupMenuSideStrip",
			SkinPopupMenuSideStripNonRecent = "PopupMenuSideStripNonRecent",
			SkinPopupMenuLinkSelected = "PopupMenuLinkSelected",
			SkinPopupMenuSplitButton = "PopupMenuSplitButton",
			SkinPopupMenuSplitButton2 = "PopupMenuSplitButton2",
			SkinPopupMenuCheck = "PopupMenuCheck",
			SkinPopupMenuExpandButton = "PopupMenuExpandButton",
			SkinLinkBorderPainter = "LinkBorderPainter",
			SkinMainMenu = "MainMenu",
			SkinMainMenuVertical = "MainMenuVertical",
			SkinFloatingBar = "FloatingBar",
			SkinFloatingBarTitle = "FloatingBarTitle",
			SkinStatusBar = "StatusBar",
			SkinLinkSelected = "LinkSelected",
			SkinInStatusBarLinkSelected = "InStatusBarLinkSelected",
			SkinMainMenuLinkSelected = "MainMenuLinkSelected",
			SkinDockWindowButtonsHot = "DockWindowButtonsHot",
			SkinDockWindowButtonsPressed = "DockWindowButtonsPressed",
			SkinDockWindowButtonsSelected = "DockWindowButtonsSelected",
			SkinDockWindowButtons = "DockWindowButtons",
			SkinAlertBarItem = "AlertBarItem",
			SkinAlertCaptionTop = "AlertCaptionTop",
			SkinAlertWindow = "AlertWindow",
			SkinAlertWindowCornerRadius = "CornerRadius",
			SkinCloseButtonGlyph = "CloseButtonGlyph",
			SkinRestoreButtonGlyph = "RestoreButtonGlyph",
			SkinMinimizeButtonGlyph = "MinimizeButtonGlyph",
			SkinCollapseButtonGlyph = "CollapseButtonGlyph",
			SkinExpandButtonGlyph = "ExpandButtonGlyph",
			SkinNavigationHeaderArrowBack = "NavigationHeaderArrowBack";
			public static string 
			SkinBarDockedRowIndent = "BarDockedRowIndent",
			SkinBarDockedRowBarIndent = "BarDockedRowBarIndent";
			public static string
				OptFloatingBarTitleLeftIndent = "TitleLeftIndent",
				OptFloatingBarTitleRightIndent = "TitleRightIndent",
				OptFloatingBarTitleTopIndent = "TitleTopIndent",
				OptFloatingBarTitleBottomIndent = "TitleBottomIndent",
				OptBarNoBorderForeColor = "NoBorderForeColor",
				OptBarNoBorderForeColorHot = "NoBorderForeColorHot",
				OptBarNoBorderForeColorPressed = "NoBorderForeColorPressed",
				OptBarNoBorderForeColorDisabled = "NoBorderForeColorDisabled";
		public static string
			DockWindowButtonsForeColor = "DockWindowButtonsForeColor",
			ColorLinkDisabledForeColor = "ColorLinkDisabledForeColor",
			ColorInStatusBarLinkCheckedForeColor = "ColorInStatusBarLinkCheckedForeColor";
		public static Skin GetSkin(ISkinProvider provider) {
			return SkinManager.Default.GetSkin(SkinProductId.Bars, provider); 
		}
	}
	public class TabSkins {
		public static string 
			SkinTabPane = "TabPane",
			SkinTabHeader = "TabHeader",
			SkinTabHeaderLine = "TabHeaderLine",
			SkinTabPageButton = "TabPageButton",
			SkinTabHeaderButton = "TabHeaderButton";
		public static Skin GetSkin(ISkinProvider provider) {
			return SkinManager.Default.GetSkin(SkinProductId.Tab, provider); 
		}
	}
	public class SchedulerSkins {
		public static string
			SkinHeader = "Header",
			SkinHeaderVertical = "HeaderVertical",
			SkinHeaderSelected = "HeaderSelected",
			SkinHeaderAlternate = "HeaderAlternate",
			SkinHeaderAlternateSelected = "HeaderAlternateSelected",
			SkinHeaderResource = "HeaderResource",
			SkinHeaderResourceVertical = "HeaderResourceVertical",
			SkinDefaultTimeLine = "DefaultTimeLine",
			SkinRulerHeader = "RulerHeader",
			SkinRuler = "Ruler",
			SkinRulerHourLine = "RulerHourLine",
			SkinRulerMinLine = "RulerMinLine",
			SkinGroupSeparatorVertical = "GroupSeparatorVertical",
			SkinGroupSeparatorHorizontal = "GroupSeparatorHorizontal",
			SkinAllDayArea = "AllDayArea",
			SkinAllDayAreaSelected = "AllDayAreaSelected",
			SkinAppointmentBorder = "AppointmentBorder",
			SkinAppointment = "Appointment",
			SkinCurrentTimeIndicator = "CurrentTimeIndicator",
			SkinBorder = "Border",
			SkinMoreButton = "MoreButton",
			SkinUpperLeftCorner = "UpperLeftCorner",
			SkinCellBorder = "CellBorder",
			SkinAppointmentLeftBorder = "AppointmentLeftBorder",
			SkinAppointmentRightBorder = "AppointmentRightBorder",
			SkinAppointmentNoLeftBorder = "AppointmentNoLeftBorder",
			SkinAppointmentNoRightBorder = "AppointmentNoRightBorder",
			SkinAppointmentContent = "AppointmentContent",
			SkinAppointmentSameDayContent = "AppointmentSameDayContent",
			SkinAppointmentSameDayLeftBorder = "AppointmentSameDayLeftBorder",
			SkinAppointmentSameDayRightBorder = "AppointmentSameDayRightBorder",
			SkinAppointmentSameDayNoLeftBorder = "AppointmentSameDayNoLeftBorder",
			SkinAppointmentSameDayNoRightBorder = "AppointmentSameDayNoRightBorder",
			SkinAppointmentStatusTopMask = "AppointmentStatusTopMask",
			SkinAppointmentStatusLeftMask = "AppointmentStatusLeftMask",
			SkinAppointmentRightShadow = "AppointmentRightShadow",
			SkinAppointmentBottomShadow = "AppointmentBottomShadow",
			SkinNavigationButtonPrev = "NavButtonPrev",
			SkinNavigationButtonNext = "NavButtonNext",
			SkinNavigationButtonPrevArrow = "NavButtonPrevArrow",
			SkinNavigationButtonNextArrow = "NavButtonNextArrow",
			SkinDependency = "Dependency",
			SkinSelectedDependency = "SelectedDependency",
			SkinDependencyCorner = "DependencyCorner",
			SkinTimeIndicator = "TimeIndicator";
		public static string
			OptHeaderRequireHorzOffset = "HeaderRequireHorzOffset",
			OptHeaderRequireVertOffset = "HeaderRequireVertOffset",
			OptPaintResourceHeaderWithResourceColor = "PaintResourceHeaderWithResourceColor",
			OptPaintHotTrackedWithResourceColor = "PaintHotTrackedWithResourceColor",
			OptAppointmentVerticalStatusLineWidth = "AppointmentStatusLineWidth",
			OptAppointmentHorizontalStatusLineHeight = "AppointmentStatusLineHeight",
			OptDayViewAppointmentLeftPadding = "DayViewAppointmentLeftPadding",
			OptMonthViewCellHeaderForeColor = "MonthViewCellHeaderForeColor",
			OptMonthViewSelectedCellHeaderForeColor = "MonthViewSelectedCellHeaderForeColor";
		public static string
			PropBaseColor = "BaseColor",
			PropAlternateColor = "AlternateColor",
			PropContentVerticalGap = "ContentVerticalGap",
			PropOpacity = "Opacity",
			PropSelectedOpacity = "SelectedOpacity",
			PropSelectedColor = "SelectedColor";
	   public static string
			ColorRatioCell = "ColorRatioCell",
			ColorRatioCellBorder = "ColorRatioCellBorder",
			ColorRatioCellBorderDark = "ColorRatioCellBorderDark",
			ColorRatioCellLight = "ColorRatioCellLight",
			ColorRatioCellLightBorder = "ColorRatioCellLightBorder",
			ColorRatioCellLightBorderDark = "ColorRatioCellLightBorderDark",
			ColorResource01 = "ResourceColor01",
			ColorResource02 = "ResourceColor02",
			ColorResource03 = "ResourceColor03",
			ColorResource04 = "ResourceColor04",
			ColorResource05 = "ResourceColor05",
			ColorResource06 = "ResourceColor06",
			ColorResource07 = "ResourceColor07",
			ColorResource08 = "ResourceColor08",
			ColorResource09 = "ResourceColor09",
			ColorResource10 = "ResourceColor10",
			ColorResource11 = "ResourceColor11",
			ColorResource12 = "ResourceColor12";
		public static Skin GetSkin(ISkinProvider provider) {
			return SkinManager.Default.GetSkin(SkinProductId.Scheduler, provider);
		}
	}
	public class RichEditSkins {
		public static string
			SkinRulerTabTypeBackground = "RulerTabTypeBackground",
			SkinRulerTab = "RulerTab",
			SkinRulerIndent = "RulerIndent",
			SkinRulerIndentBottom = "RulerIndentBottom",
			SkinRulerColumnResizer = "RulerColumnResizer",
			SkinRulerDefaultTabColor = "RulerDefaultTabColor",
			SkinHorizontalRulerBackground = "HorizontalRulerBackground",
			SkinVerticalRulerBackground = "VerticalRulerBackground",
			SkinHorizontalCornerPanel = "CornerPanel",
			SkinCommentBorder = "CommentBorder",
			SkinCommentMoreButton = "CommentMoreButton",
			SkinCommentsArea = "CommentsArea",
			SkinActiveCommentBorder = "ActiveCommentBorder";
		public static Skin GetSkin(ISkinProvider provider) {
			return SkinManager.Default.GetSkin(SkinProductId.RichEdit, provider);
		}
	}
	public class SpreadsheetSkins {
		public static string
			SkinAddNewWorksheetButton = "AddNewWorksheetButton",
			SkinTabHeader = "TabHeader",
			SkinColoredTabHeader = "ColoredTabHeader";
		public static string
			ColorSelectionBorder = "ColorSelectionBorder",
			ColorSelectAllTriangle = "ColorSelectAllTriangle",
			ColorFrozenPanesSeparator = "ColorFrozenPanesSeparator";
		public static string
		   OptSelectionBorderWidth = "SelectionBorderWidth",
		   OptColoredTabUseMultiplyMode = "ColoredTabUseMultiplyMode";
		public static Skin GetSkin(ISkinProvider provider) {
			return SkinManager.Default.GetSkin(SkinProductId.Spreadsheet, provider);
		}
	}
	public class PdfViewerSkins {
		public static string
			Selection = "Selection",
			SearchPanel = "SearchPanel";
		public static Skin GetSkin(ISkinProvider provider) {
			return SkinManager.Default.GetSkin(SkinProductId.PdfViewer, provider);
		}
	}
	public class GaugesSkins {
		public static string
			SkinBackground = "Background",
			SkinRangeBar = "RangeBar",
			SkinLabel = "Label";
		public static Skin GetSkin(ISkinProvider provider) {
			return SkinManager.Default.GetSkin(SkinProductId.Gauges, provider);
		}
	}
	public class MapSkins {
		public static string
			PropMapBackColor = "MapBackColor",
			PropPanelBackColor = "PanelBackColor",
			PropPanelBackColorAlpha = "PanelBackColorAlpha",
			PropPanelTextColor = "PanelTextColor",
			PropPanelHotTrackedTextColor = "PanelHotTrackedTextColor",
			PropPanelPressedTextColor = "PanelPressedTextColor",
			PropPanelPressedTextColorAlpha = "PanelPressedTextColorAlpha",
			PropOverlayBackColor = "OverlayBackColor",
			PropOverlayBackColorAlpha = "OverlayBackColorAlpha",
			PropOverlayTextColor = "OverlayTextColor";
		public static string
			SkinShape = "Shape",
			SkinCustomElement = "CustomElement",
			SkinSelectedRegion = "SelectedRegion",
			SkinPushpin = "Pushpin",
			SkinCallout = "Callout";
		public static string
			PropBaseColor = "BaseColor",
			PropHighlightedBaseColor = "HighlightedBaseColor",
			PropSelectedBaseColor = "SelectedBaseColor",
			PropElementBorderColor = "BorderColor",
			PropElementBorderColorAlpha = "BorderColorAlpha",
			PropBorderWidth = "BorderWidth",
			PropHighlightedBorderWidth = "HighlightedBorderWidth",
			PropSelectedBorderWidth = "SelectedBorderWidth",
			PropElementBackColor = "BackColor",
			PropElementBackColorAlpha = "BackColorAlpha",
			PropElementSelectedColor = "SelectedColor",
			PropElementHighlightedColor = "HighlightedColor",
			PropSelectedBorderColor = "SelectedBorderColor",
			PropSelectedBorderColorAlpha = "SelectedBorderColorAlpha",
			PropHighlightedBorderColor = "HighlightedBorderColor",
			PropHighlightedBorderColorAlpha = "HighlightedBorderColorAlpha";
			public static string 
				PropTextColor = "TextColor",
				PropTextGlowColor = "TextGlowColor";
			public static string
					PropImage = "Image",
					PropTextOriginX = "TextOriginX",
					PropTextOriginY = "TextOriginY",
					PropPointerX = "PointerX",
					PropPointerY = "PointerY",
					PropPointerHeight = "PointerHeight",
					PropHighlightingEffectiveArea = "HighlightingEffectiveArea";
			public static string
				ColorCriteria01 = "Criteria01",
				ColorCriteria02 = "Criteria02",
				ColorCriteria03 = "Criteria03",
				ColorGradient01 = "Gradient01",
				ColorGradient02 = "Gradient02",
				ColorPalette01 = "Palette01",
				ColorPalette02 = "Palette02",
				ColorPalette03 = "Palette03",
				ColorPalette04 = "Palette04",
				ColorPalette05 = "Palette05",
				ColorPalette06 = "Palette06",
				ColorPalette07 = "Palette07",
				ColorPalette08 = "Palette08",
				ColorPalette09 = "Palette09",
				ColorPalette10 = "Palette10";
		public static Skin GetSkin(ISkinProvider provider) {
			return SkinManager.Default.GetSkin(SkinProductId.Map, provider);
		}
	}
	public class ChartSkins {
		public static string
			SkinBackground = "Background",
			SkinLegend = "Legend",
			SkinXYDiagram = "XYDiagram",
			SkinXYDiagramInterlaced = "XYDiagramInterlaced",
			SkinRadarDiagram = "RadarDiagram",
			SkinRadarDiagramInterlaced = "RadarDiagramInterlaced",
			SkinXYDiagram3D = "XYDiagram3D",
			SkinXYDiagram3DInterlaced = "XYDiagram3DInterlaced",
			SkinStrip = "Strip",
			SkinTextAnnotation = "TextAnnotation",
			SkinImageAnnotation = "ImageAnnotation",
			SkinBar = "Bar",
			SkinArea = "Area",
			SkinPie = "Pie",
			SkinFunnel = "Funnel",
			SkinBar3D = "Bar3D",
			SkinPie3D = "Pie3D",
			SkinMarker = "Marker",
			SkinSeriesLabel2D = "SeriesLabel2D",
			SkinSeriesLabel3D = "SeriesLabel3D",
			SkinScrollBar = "ScrollBar";
		public static string
			PropertyPaletteName = "PaletteName";
		public static string
			ColorChartTitle = "ColorChartTitle",
			ColorAxis = "ColorAxis",
			ColorAxisLabel = "ColorAxisLabel",
			ColorGridLines = "ColorGridLines",
			ColorMinorGridLines = "ColorMinorGridLines",
			ColorAxisTitle = "ColorAxisTitle",
			ColorConstantLine = "ColorConstantLine",
			ColorConstantLineTitle = "ColorConstantLineTitle",
			ColorLine3DMarker = "ColorLine3DMarker",
			ColorArea3DMarker = "ColorArea3DMarker",
			ColorSeriesLabelConnector = "ColorSeriesLabelConnector",
			ShowSeriesLabelBorder = "ShowSeriesLabelBorder",
			ShowBarBorder = "ShowBarBorder",
			ShowSeriesLabelConnector = "ShowSeriesLabelConnector",
			ShowBubbleSeriesLabelConnector = "ShowBubbleSeriesLabelConnector",
			ColorPaneBounds = "ColorPaneBounds";
		public static Skin GetSkin(ISkinProvider provider) {
			return SkinManager.Default.GetSkin(SkinProductId.Chart, provider);
		}
	}
	public enum ChartPaletteName {
		NatureColors,
		PastelKit,
		InAFog,
		TerracottaPie,
		NorthernLights,
		Chameleon,
		TheTrees,
		Mixed,
		Office,
		BlackAndWhite,
		Grayscale,
		Apex,
		Aspect,
		Civic,
		Concourse,
		Equity,
		Flow,
		Foundry,
		Median,
		Metro,
		Module,
		Opulent,
		Oriel,
		Origin,
		Paper,
		Solstice,
		Technic,
		Trek,
		Urban,
		Verve,
		Office2013,
		BlueWarm,
		Blue,
		BlueII,
		BlueGreen,
		Green,
		GreenYellow,
		Yellow,
		YellowOrange,
		Orange,
		OrangeRed,
		RedOrange,
		Red,
		RedViolet,
		Violet,
		VioletII,
		Marquee,
		Slipstream
	}
	public class SparklineSkins {
		public static string
			SkinLine = "Line",
			SkinArea = "Area",
			SkinBar = "Bar",
			SkinWinLoss = "WinLoss";
		public static string
			Color = "Color",
			ColorMarker = "ColorMarker",
			ColorMaxPoint = "ColorMaxPoint",
			ColorMinPoint = "ColorMinPoint",
			ColorStartPoint = "ColorStartPoint",
			ColorEndPoint = "ColorEndPoint",
			ColorNegativePoint = "ColorNegativePoint";
		public static Skin GetSkin(ISkinProvider provider) {
			return SkinManager.Default.GetSkin(SkinProductId.Sparkline, provider);
		}
	}
	public class DashboardSkins {
		public static string SkinDragArea = "DragArea";
		public static string SkinDragGroup = "DragGroup";
		public static string SkinDragGroupOptionsButton = "DragGroupOptionsButton";
		public static string SkinDragItem = "DragItem";
		public static string SkinDragItemOptionsButton = "DragItemOptionsButton";
		public static string SkinDashboardItemCaptionTop = "DashboardItemCaptionTop";
		public static string SkinDashboardItemTop = "DashboardItemTop";
		public static string SkinDashboardItemPanel = "DashboardItemPanel";
		public static string SkinCard = "Card";
		public static string SkinDashboardItemBackground = "DashboardItemBackground";
		public static string SkinDashboardItemSpacing = "DashboardItemSpacing";
		public static string SkinDashboardDragHintGlyph = "DashboardDragHintGlyph";
		public static string SkinGauge = "Gauge";
		public static Skin GetSkin(ISkinProvider provider) {
			return SkinManager.Default.GetSkin(SkinProductId.Dashboard, provider);
		}
	}
	public class CommonSkins {
		public static string
			SkinLabel = "Label",
			SkinLabelLine = "LabelLine",
			SkinLabelLineVert = "LabelLineVert",
			SkinSplitter = "Splitter",
			SkinSplitterHorz = "SplitterHorz",
			SkinButton = "Button",
			SkinTextBorder = "TextBorder",
			SkinSizeGrip = "SizeGrip",
			SkinScrollButton = "ScrollButton",
			SkinScrollButtonThumb = "ScrollThumbButton",
			SkinScrollButtonThumbVert = "ScrollThumbButtonVert",
			SkinScrollButtonTouchThumb = "ScrollTouchThumbButton",
			SkinScrollButtonTouchThumbVert = "ScrollTouchThumbButtonVert",
			SkinScrollTouchBackground = "SkinScrollTouchBackground",
			SkinScrollTouchBackgroundVert = "SkinScrollTouchBackgroundVert",
			SkinLoadingBig = "LoadingBig",
			SkinScrollShaft = "ScrollShaft",
			SkinScrollShaftVert = "ScrollShaftVert",
			SkinGroupPanel = "GroupPanel",
			SkinGroupPanelNoBorder = "GroupPanelNoBorder",
			SkinGroupPanelTop = "GroupPanelTop",
			SkinGroupPanelCaptionTop = "GroupPanelCaptionTop",
			SkinGroupPanelBottom = "GroupPanelBottom",
			SkinGroupPanelCaptionBottom = "GroupPanelCaptionBottom",
			SkinGroupPanelRight = "GroupPanelRight",
			SkinGroupPanelCaptionRight = "GroupPanelCaptionRight",
			SkinGroupPanelLeft = "GroupPanelLeft",
			SkinGroupPanelCaptionLeft = "GroupPanelCaptionLeft",
			SkinGroupPanelExpandButton = "GroupExpandButton",
			SkinLayoutItemBackground = "LayoutItemBackground",
			SkinLayoutGroupWithoutBordersPadding = "LayoutGroupWithoutBordersPadding",
			SkinLayoutGroupWithoutBordersSpacing = "LayoutGroupWithoutBordersSpacing",
			SkinLayoutRootGroupPadding = "LayoutRootGroupPadding",
			SkinLayoutResizingGlyph = "LayoutResizingGlyph",
			SkinLayoutRootGroupSpacing = "LayoutRootGroupSpacing",
			SkinLayoutRootGroupWithoutBordersPadding = "LayoutRootGroupWithoutBordersPadding",
			SkinLayoutRootGroupWithoutBordersSpacing = "LayoutRootGroupWithoutBordersSpacing",
			SkinLayoutItemPadding = "LayoutItemPadding",
			SkinLayoutGroupPadding = "LayoutGroupPadding",
			SkinLayoutTabbedGroupPadding = "LayoutTabbedGroupPadding",
			SkinLayoutItemSpacing = "LayoutItemSpacing",
			SkinLayoutGroupSpacing = "LayoutGroupSpacing",
			SkinLayoutTabbedGroupSpacing = "LayoutTabbedGroupSpacing",
			SkinLayoutItemTextToControlDistance = "LayoutItemTextToControlDistance",
			SkinLayoutItemBackgroundSpacingProperty = "Spacing",
			SkinToolTipWindow = "ToolTipWindow",
			SkinToolTipWindowCornerRadius = "CornerRadius",
			SkinToolTipSeparator = "ToolTipSeparator",
			SkinToolTipItem = "ToolTipItem",
			SkinToolTipTitleItem = "ToolTipTitleItem",
			SkinForm = "Form",
			SkinDropDownButton1 = "DropDownButton1",
			SkinDropDownButton2 = "DropDownButton2",
			SkinHighlightedItem = "HighlightedItem",
			SkinBackButton = "BackButton",
			SkinDropArrows = "DropArrows",
			SkinSelection = "Selection",
			SkinSelectionOpacity = "SelectionOpacity",
			SkinInformationColor = "Information",
			SkinWarningColor = "Warning",
			SkinCriticalColor = "Critical",
			SkinQuestionColor = "Question",
			SkinHighlightAlternateColor = "HighlightAlternate",
			SkinDesignTimeSelection = "DesignTimeSelection",
			SkinDefaultFontSize = "DefaultFontSize",
			SkinDefaultFontName = "DefaultFontName",
			SkinDropDown = "DropDown",
			SkinTextControl = "TextControl",
			SkinCheckButtonPressedForeColor = "CheckButtonPressedForeColor",
			SkinUseCustomGroupCaptionDrawing = "UseCustomGroupCaptionDrawing",
			SkinCustomDrawCaptionOffset = "CustomDrawCaptionOffset",
			SkinGroupPanelButtonToTextBlockDistance = "GroupPanelButtonInterval",
			SkinGroupPanelCaptionImageToTextDistance = "GroupPanelCaptionImageToTextDistance",
			SkinGroupPanelButtonToBorderDistance = "ButtonToBorderInterval",
			SkinGroupPanelExpandButtonForeColorHot = "ForeColorHot",
			SkinGroupPanelExpandButtonForeColorPressed = "ForeColorPressed";
		public static string OptScrollButtonCorner = "CornerSize",
			OptGroupCaptionTextPadding = "GroupCaptionTextPadding",
			OptGroupControlFullHeader = "GroupControlFullHeader";
		public static Skin GetSkin(ISkinProvider provider) {
			return SkinManager.Default.GetSkin(SkinProductId.Common, provider); 
		}
		public static string OptAllowHoverAnimation = "AllowHoverAnimation";
	}
	public class FormSkins {
		public static string
			SkinFormFrameLeft = "FormFrameLeft",
			SkinFormFrameRight = "FormFrameRight",
			SkinFormFrameBottom = "FormFrameBottom",
			SkinFormCaption = "FormCaption",
			SkinFormCaptionMinimized = "FormCaptionMinimized",
			SkinFormMdiBar = "FormMdiBar",
			SkinFormButtonMdiClose = "FormButtonMdiClose",
			SkinFormButtonMdiRestore = "FormButtonMdiRestore",
			SkinFormButtonMdiMinimize = "FormButtonMdiMinimize",
			SkinFormButtonClose = "FormButtonClose",
			SkinFormButtonMinimize = "FormButtonMinimize",
			SkinFormButtonMaximize = "FormButtonMaximize",
			SkinFormButtonFullScreen = "FormButtonFullScreen",
			SkinFormButtonHelp = "FormButtonHelp",
			SkinFormButtonRestore = "FormButtonRestore",
			SkinFormDecoratorFrameLeft = "FormDecoratorFrameLeft",
			SkinFormDecoratorFrameRight = "FormDecoratorFrameRight",
			SkinFormDecoratorFrameTop = "FormDecoratorFrameTop",
			SkinFormDecoratorFrameBottom = "FormDecoratorFrameBottom",
			SkinToolbarDecoratorFrameLeft = "ToolbarDecoratorFrameLeft",
			SkinToolbarDecoratorFrameRight = "ToolbarDecoratorFrameRight",
			SkinToolbarDecoratorFrameTop = "ToolbarDecoratorFrameTop",
			SkinToolbarDecoratorFrameBottom = "ToolbarDecoratorFrameBottom",
			SkinBeakFormShadowFrameLeft = "BeakFormShadowFrameLeft",
			SkinBeakFormShadowFrameRight = "BeakFormShadowFrameRight",
			SkinBeakFormShadowFrameTop = "BeakFormShadowFrameTop",
			SkinBeakFormShadowFrameBottom = "BeakFormShadowFrameBottom",
			SkinBeakFormLeftArrow = "BeakFormLeftArrow",
			SkinBeakFormUpArrow = "BeakFormUpArrow",
			SkinBeakFormRightArrow = "BeakFormRightArrow",
			SkinBeakFormDownArrow = "BeakFormDownArrow",
			SkinBeakFormBorder = "BeakFormBorder",
			SkinFormDecoratorGlowFrameLeft = "FormDecoratorGlowFrameLeft",
			SkinFormDecoratorGlowFrameRight = "FormDecoratorGlowFrameRight",
			SkinFormDecoratorGlowFrameTop = "FormDecoratorGlowFrameTop",
			SkinFormDecoratorGlowFrameBottom = "FormDecoratorGlowFrameBottom",
			SkinTabFormPage = "TabFormPage",
			SkinTabFormAddPage = "TabFormAddPage";
		public static string
			OptThickness = "Thickness",
			OptDecoratorOffset = "DecoratorOffset",
			OptBeakFormShadowOffset = "BeakFormShadowOffset",
			OptToolbarShadowOffset = "ToolbarShadowOffset",
			OptInactiveColor = "InactiveColor",
			OptTextShadowColor = "TextShadowColor";
		public static Skin GetSkin(ISkinProvider provider) {
			return SkinManager.Default.GetSkin(SkinProductId.Form, provider); 
		}
	}
	public class CommonColors {
		static KnownColor[] Colors = new KnownColor[] {
			KnownColor.Control, KnownColor.ControlText, KnownColor.ControlDark, KnownColor.GrayText, 
			KnownColor.Window, KnownColor.WindowText, KnownColor.Highlight, KnownColor.HighlightText, 
			KnownColor.Info, KnownColor.InfoText, KnownColor.Menu, KnownColor.MenuText, KnownColor.InactiveCaption, KnownColor.InactiveCaptionText,
			KnownColor.ControlLight 
		};
		public static string[] GetSystemColorNames() {
			return new string[] { Control, ControlText,  DisabledControl, DisabledText, 
									Window, WindowText, Highlight, HighlightText, 
									Info, InfoText, Menu, MenuText, HideSelection, HighlightText, ReadOnly
								};
		}
		static Hashtable colorMapTable = null;
		protected static Hashtable ColorMapTable {
			get {
				if(colorMapTable == null) {
					string[] systemNames = GetSystemColorNames();
					colorMapTable = new Hashtable();
					for(int n = 0; n < Colors.Length; n++) {
						colorMapTable[Colors[n]] = systemNames[n];
					}
				}
				return colorMapTable;
			}
		}
		public static Color GetSystemColor(string name) {
			int index = Array.IndexOf(GetSystemColorNames(), name);
			if(index == -1) return Color.Empty;
			return Color.FromKnownColor(Colors[index]);
		}
		public static string GetSystemColorName(KnownColor color, bool alwaysReturnColor) {
			object res = ColorMapTable[color];
			if(res == null) return alwaysReturnColor ? Control : null;
			return (string)res;
		}
		public static string GetSystemColorName(KnownColor color) {
			return GetSystemColorName(color, true);
		}
		public static bool IsSystemColor(Color color) {
			if(!color.IsKnownColor) return false;
			return ColorMapTable.Contains(color.ToKnownColor());
		}
		public static Color GetInformationColor(ISkinProvider skinProvider) { 
			Color ret = GetCommonPredefinedColor(skinProvider, CommonSkins.SkinInformationColor);
			return ret == Color.Empty ? Color.Green : ret;
		}
		public static Color GetWarningColor(ISkinProvider skinProvider) {
			Color ret = GetCommonPredefinedColor(skinProvider, CommonSkins.SkinWarningColor);
			return ret == Color.Empty ? Color.Yellow : ret;
		}
		public static Color GetCriticalColor(ISkinProvider skinProvider) {
			Color ret = GetCommonPredefinedColor(skinProvider, CommonSkins.SkinCriticalColor);
			return ret == Color.Empty ? Color.Red : ret;
		}
		public static Color GetQuestionColor(ISkinProvider skinProvider) {
			Color ret = GetCommonPredefinedColor(skinProvider, CommonSkins.SkinQuestionColor);
			return ret == Color.Empty ? Color.Blue : ret;
		}
		static Color GetCommonPredefinedColor(ISkinProvider skinProvider, object colorName) {
			Skin skin = CommonSkins.GetSkin(skinProvider);
			Color ret = skin.Colors.GetColor(colorName);
			return ret;
		}
		public static string 
			ReadOnly = "ReadOnly",
			Control = "Control", 
			ControlText = "ControlText", 
			DisabledControl = "DisabledControl", 
			DisabledText = "DisabledText",
			Window = "Window",
			WindowText = "WindowText",
			Highlight = "Highlight",
			HighlightText = "HighlightText",
			Info = "Info",
			InfoText = "InfoText",
			Menu = "Menu",
			MenuText = "MenuText",
			HideSelection = "HideSelection",
			HighlightSearch = "HighlightSearch",
			HighlightSearchText = "HighlightSearchText";
	}
	public class CommonSkinsRegistration { 
	}
	enum SkinType { Standard, Bonus, Theme, Custom}
	public enum SkinAppearanceType { None, Default, Office2013, Gray }
	class SkinObject {
		string name;
		SkinType type;
		int order;
		SkinAppearanceType appearanceType;
		public SkinObject(string name, SkinType type) : this(name, type, -1) { }
		public SkinObject(string name, SkinType type, SkinAppearanceType appearanceType)
			: this(name, type, appearanceType, -1) {
		}
		public SkinObject(string name, SkinType type, int order)
			: this(name, type, SkinAppearanceType.Default, order) {
		}
		public SkinObject(string name, SkinType type, SkinAppearanceType appearanceType, int order) {
			this.name = name;
			this.type = type;
			this.order = order;
			this.appearanceType = appearanceType;
		}
		public string SkinName { get { return name; } }
		public SkinType SkinType { get { return type; } }
		public int Order { get { return order; } }
		public SkinAppearanceType AppearanceType { get { return appearanceType; } }
	}
	public enum SkinIconsSize { Small, Large }
	public static class SkinCollectionHelper {
		[ThreadStatic]
		static ImageCollection skinIconsSmall = null;
		[ThreadStatic]
		static ImageCollection skinIconsLarge = null;
		static SkinObject[] orderedSkins = new SkinObject[] {
			new SkinObject("DevExpress Style", SkinType.Standard, 0),
			new SkinObject("Caramel", SkinType.Bonus),
			new SkinObject("Money Twins", SkinType.Bonus),
			new SkinObject("DevExpress Dark Style", SkinType.Standard, SkinAppearanceType.Gray, 1),
			new SkinObject("iMaginary", SkinType.Bonus),
			new SkinObject("Lilian", SkinType.Bonus),
			new SkinObject("Black", SkinType.Bonus, SkinAppearanceType.Gray),
			new SkinObject("Blue", SkinType.Bonus),
			new SkinObject("Office 2010 Blue", SkinType.Standard, 7),
			new SkinObject("Office 2010 Black", SkinType.Standard, SkinAppearanceType.Gray, 8),
			new SkinObject("Office 2010 Silver", SkinType.Standard, 9),
			new SkinObject("Office 2007 Blue", SkinType.Bonus),
			new SkinObject("Office 2007 Black", SkinType.Bonus, SkinAppearanceType.Gray),
			new SkinObject("Office 2007 Silver", SkinType.Bonus),
			new SkinObject("Office 2007 Green", SkinType.Bonus),
			new SkinObject("Office 2007 Pink", SkinType.Bonus),
			new SkinObject("Seven", SkinType.Bonus),
			new SkinObject("Seven Classic", SkinType.Standard, 13),
			new SkinObject("Darkroom", SkinType.Bonus, SkinAppearanceType.Gray),
			new SkinObject("McSkin", SkinType.Theme, 5),
			new SkinObject("Sharp", SkinType.Bonus, SkinAppearanceType.Gray),
			new SkinObject("Sharp Plus", SkinType.Bonus, SkinAppearanceType.Gray),
			new SkinObject("Foggy", SkinType.Bonus),
			new SkinObject("Dark Side", SkinType.Bonus, SkinAppearanceType.Gray),
			new SkinObject("Xmas 2008 Blue", SkinType.Theme, 4),
			new SkinObject("Springtime", SkinType.Theme, 1),
			new SkinObject("Summer 2008", SkinType.Theme, 2),
			new SkinObject("Pumpkin", SkinType.Theme, 0),
			new SkinObject("Valentine", SkinType.Theme, 3),
			new SkinObject("Stardust", SkinType.Bonus),
			new SkinObject("Coffee", SkinType.Bonus),
			new SkinObject("Glass Oceans", SkinType.Bonus),
			new SkinObject("High Contrast", SkinType.Bonus, SkinAppearanceType.Gray),
			new SkinObject("Liquid Sky", SkinType.Bonus),
			new SkinObject("London Liquid Sky", SkinType.Bonus),
			new SkinObject("The Asphalt World", SkinType.Bonus),
			new SkinObject("Blueprint", SkinType.Theme),
			new SkinObject("Whiteprint", SkinType.Theme),
			new SkinObject("VS2010", SkinType.Standard),
			new SkinObject("Metropolis", SkinType.Bonus),
			new SkinObject("Metropolis Dark", SkinType.Bonus, SkinAppearanceType.Gray),
			new SkinObject("Office 2013", SkinType.Standard, SkinAppearanceType.Office2013, 4),
			new SkinObject("Office 2013 Dark Gray", SkinType.Standard, SkinAppearanceType.Office2013, 5),
			new SkinObject("Office 2013 Light Gray", SkinType.Standard, SkinAppearanceType.Office2013, 6),
			new SkinObject("Visual Studio 2013 Blue", SkinType.Standard, SkinAppearanceType.Office2013, 10),
			new SkinObject("Visual Studio 2013 Dark", SkinType.Standard, SkinAppearanceType.Gray, 11),
			new SkinObject("Visual Studio 2013 Light", SkinType.Standard, SkinAppearanceType.Office2013, 12),
			new SkinObject("Office 2016 Colorful", SkinType.Standard, SkinAppearanceType.Office2013, 2),
			new SkinObject("Office 2016 Dark", SkinType.Standard, SkinAppearanceType.Office2013, 3)
		};
		static ArrayList skinIconNames = null;
		static ArrayList SkinIconNames {
			get {
				if(skinIconNames == null) {
					skinIconNames = new ArrayList();
					skinIconNames.Add(string.Empty);
					foreach(SkinObject obj in orderedSkins)
						skinIconNames.Add(obj.SkinName);
				}
				return skinIconNames;
			}
		}
		static bool ContainsName(string name, SkinType type) {
			foreach(SkinObject obj in orderedSkins) {
				if(obj.SkinName == name && obj.SkinType == type)
					return true;
			}
			return false;
		}
		static SkinObject GetSkinObject(string name) {
			foreach(SkinObject obj in orderedSkins)
				if(obj.SkinName == name) return obj;
			return null;
		}
		public static bool IsCustomSkin(string name) {
			foreach(SkinObject obj in orderedSkins) 
				if(obj.SkinName == name) return false;
			return true;
		}
		public static bool IsBonusSkin(string name) {
			return ContainsName(name, SkinType.Bonus);
		}
		public static bool IsStandardSkins(string name) {
			return ContainsName(name, SkinType.Standard);
		}
		public static bool IsThemeSkin(string name) {
			return ContainsName(name, SkinType.Theme);
		}
		public static int GetSkinOrder(string name) {
			SkinObject skinObj = GetSkinObject(name);
			return skinObj != null ? skinObj.Order : -1;
		}
		public static SkinAppearanceType GetSkinAppearanceType(string name) {
			SkinObject skinObj = GetSkinObject(name);
			return skinObj != null ? skinObj.AppearanceType : SkinAppearanceType.None;
		}
		public static ImageCollection SkinIconsSmall {
			get {
				if(skinIconsSmall == null)
					skinIconsSmall = ImageHelper.CreateImageCollectionFromResources("DevExpress.Utils.Skins.Skins_Icons_16x16.png", typeof(SkinCollectionHelper).Assembly, new Size(16, 16), Color.Empty);
				return skinIconsSmall;
			}
		}
		public static ImageCollection SkinIconsLarge {
			get {
				if(skinIconsLarge == null)
					skinIconsLarge = ImageHelper.CreateImageCollectionFromResources("DevExpress.Utils.Skins.Skins_Icons_48x48.png", typeof(SkinCollectionHelper).Assembly, new Size(48, 48), Color.Empty);
				return skinIconsLarge;
			}
		}
		public static int GetSkinIndexByName(string name) {
			for(int i = 0; i < SkinIconNames.Count; i++)
				if(SkinIconNames[i].Equals(name)) return i;
			return 0;
		}
		public static Image GetSkinIcon(string name, SkinIconsSize size) {
			int index = GetSkinIndexByName(name);
			if(index >= SkinIconsSmall.Images.Count) index = 0;
			return (size == SkinIconsSize.Small) ? SkinIconsSmall.Images[index] : SkinIconsLarge.Images[index];
		}
	}
}
