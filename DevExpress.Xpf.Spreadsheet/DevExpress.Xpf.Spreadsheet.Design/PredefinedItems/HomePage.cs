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

extern alias Platform;
using System;
using Platform::DevExpress.Xpf.Core.Design;
namespace DevExpress.Xpf.Spreadsheet.Design {
	public static partial class BarInfos {
		#region Clipboard
		public static BarInfo Clipboard { get { return clipboard; } }
		static BarInfo CreateHomeClipboardGroup() {
			return new BarInfo(
				String.Empty,
				"Home",
				"Clipboard",
				new BarInfoItems(
					new string[] { "EditPaste", "EditCut", "EditCopy", "EditPasteSpecial" },
					new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.ButtonSmallWithTextRibbonStyle, BarItemInfos.ButtonSmallWithTextRibbonStyle, BarItemInfos.ButtonSmallWithTextRibbonStyle, }
				),
				String.Empty,
				String.Empty,
				"Caption_PageHome",
				"Caption_GroupClipboard"
			);
		}
		#endregion
		#region Font
		public static BarInfo Font { get { return font; } }
		static BarInfo CreateHomeFontGroup() {
			BarButtonGroupItemInfo fontButtonGroup = new BarButtonGroupItemInfo(
				new BarInfoItems(
					new string[] { "FormatFontName", "FormatFontSize", "FormatIncreaseFontSize", "FormatDecreaseFontSize" },
					new BarItemInfo[] { new BarFontNameItemInfo(), new BarFontSizeItemInfo(), BarItemInfos.Button, BarItemInfos.Button }
				)
			);
			BarButtonGroupItemInfo fontShapeButtonGroup = new BarButtonGroupItemInfo(
				new BarInfoItems(
					new string[] { "FormatFontBold", "FormatFontItalic", "FormatFontUnderline", "FormatFontStrikeout" },
					new BarItemInfo[] { BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Check }
				)
			);
			BarButtonGroupItemInfo fontColorButtonGroup = new BarButtonGroupItemInfo(
				new BarInfoItems(
					new string[] { "FormatFontColor", "FormatFillColor" },
					new BarItemInfo[] { BarItemInfos.ForeColorSplitButton, BarItemInfos.ForeColorSplitButton }
				)
			);
			BarSubItemInfo bordersSubItem = new BarSubItemInfo(
				new BarInfoItems(
					new string[] { "FormatBottomBorder", "FormatTopBorder", "FormatLeftBorder", "FormatRightBorder", "FormatNoBorders", "FormatAllBorders", "FormatOutsideBorders", "FormatThickBorder", "FormatBottomDoubleBorder", "FormatBottomThickBorder", "FormatTopAndBottomBorder", "FormatTopAndThickBottomBorder", "FormatTopAndDoubleBottomBorder" },
					new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button }
				),
				Platform::DevExpress.Xpf.Bars.RibbonItemStyles.SmallWithoutText
			);
			BarButtonGroupItemInfo bordersButtonGroup = new BarButtonGroupItemInfo(
				new BarInfoItems(
					new string[] { "FormatBordersCommandGroup" },
					new BarItemInfo[] { bordersSubItem }
				)
			);
			return new BarInfo(
				String.Empty,
				"Home",
				"Font",
				new BarInfoItems(
					new string[] { "Font", "FontShape", "FontColor", "" },
					new BarItemInfo[] { fontButtonGroup, fontShapeButtonGroup, fontColorButtonGroup, bordersButtonGroup }
				),
				"FormatCellsFont",
				String.Empty,
				"Caption_PageHome",
				"Caption_GroupFont"
			);
		}
		#endregion
		#region Alignment
		public static BarInfo Alignment { get { return alignment; } }
		static BarInfo CreateHomeAlignmentGroup() {
			BarButtonGroupItemInfo verticalAlignmentGroup = new BarButtonGroupItemInfo(
			   new BarInfoItems(
				   new string[] { "FormatAlignmentTop", "FormatAlignmentMiddle", "FormatAlignmentBottom" },
				   new BarItemInfo[] { BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Check }
			   )
			);
			BarButtonGroupItemInfo horizontalAlignmentGroup = new BarButtonGroupItemInfo(
			   new BarInfoItems(
				   new string[] { "FormatAlignmentLeft", "FormatAlignmentCenter", "FormatAlignmentRight" },
				   new BarItemInfo[] { BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Check }
			   )
			);
			BarButtonGroupItemInfo indentGroup = new BarButtonGroupItemInfo(
			   new BarInfoItems(
				   new string[] { "FormatDecreaseIndent", "FormatIncreaseIndent" },
				   new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button }
			   )
			);
			BarSubItemInfo mergeCellsSubItem = new BarSubItemInfo(
				new BarInfoItems(
					new string[] { "EditingMergeAndCenterCells", "EditingMergeCellsAcross", "EditingMergeCells", "EditingUnmergeCells" },
					new BarItemInfo[] { BarItemInfos.Check, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button }
				),
				Platform::DevExpress.Xpf.Bars.RibbonItemStyles.SmallWithText
			);
			return new BarInfo(
				String.Empty,
				"Home",
				"Alignment",
				new BarInfoItems(
					new string[] { "VerticalAlignment", "HorizontalAlignment", "Indent", "FormatWrapText", "EditingMergeCellsCommandGroup" },
					new BarItemInfo[] { verticalAlignmentGroup, horizontalAlignmentGroup, indentGroup, BarItemInfos.CheckSmallWithTextRibbonStyle, mergeCellsSubItem }
				),
				"FormatCellsAlignment",
				String.Empty,
				"Caption_PageHome",
				"Caption_GroupAlignment"
			);
		}
		#endregion
		#region Number
		public static BarInfo Number { get { return number; } }
		static BarInfo CreateHomeNumberGroup() {
			BarSubItemInfo accountingSubItem = new BarSubItemInfo(
				new BarInfoItems(
					new string[] { "FormatNumberAccountingUS", "FormatNumberAccountingUK", "FormatNumberAccountingEuro", "FormatNumberAccountingPRC", "FormatNumberAccountingSwiss" },
					new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button }
				),
				Platform::DevExpress.Xpf.Bars.RibbonItemStyles.SmallWithoutText
			);
			BarButtonGroupItemInfo formatNumberItemGroup = new BarButtonGroupItemInfo(
			   new BarInfoItems(
				   new string[] { "FormatNumberAccountingCommandGroup", "FormatNumberPercent", "FormatNumberAccounting" },
				   new BarItemInfo[] { accountingSubItem, BarItemInfos.Button, BarItemInfos.Button }
			   )
			);
			BarButtonGroupItemInfo formatNumberChangeDecimalsItemGroup = new BarButtonGroupItemInfo(
			   new BarInfoItems(
				   new string[] { "FormatNumberIncreaseDecimal", "FormatNumberDecreaseDecimal" },
				   new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button }
			   )
			);
			return new BarInfo(
				String.Empty,
				"Home",
				"Number",
				new BarInfoItems(
					new string[] { "FormatNumbers", "FormatNumberDecimals" },
					new BarItemInfo[] { formatNumberItemGroup, formatNumberChangeDecimalsItemGroup }
				),
				"FormatCellsNumber",
				String.Empty,
				"Caption_PageHome",
				"Caption_GroupNumber"
			);
		}
		#endregion
		#region Styles
		public static BarInfo Styles { get { return styles; } }
		[System.Diagnostics.CodeAnalysis.SuppressMessage("DevExpress.Design", "DCA0002")]
		static BarInfo CreateHomeStylesGroup() {
			BarSubItemInfo conditionalFormattingHighlightCellsSubItem = new BarSubItemInfo(
			   new BarInfoItems(
				   new string[] { "ConditionalFormattingGreaterThan", "ConditionalFormattingLessThan", "ConditionalFormattingBetween", "ConditionalFormattingEqual", "ConditionalFormattingTextContains", "ConditionalFormattingDateOccurring", "ConditionalFormattingDuplicateValues" },
				   new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button }
			   )
			);
			BarSubItemInfo conditionalFormattingTopBottomSubItem = new BarSubItemInfo(
			   new BarInfoItems(
				   new string[] { "ConditionalFormattingTop10Items", "ConditionalFormattingTop10Percent", "ConditionalFormattingBottom10Items", "ConditionalFormattingBottom10Percent", "ConditionalFormattingAboveAverage", "ConditionalFormattingBelowAverage" },
				   new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button }
			   )
			);
			BarSubItemInfo conditionalFormattingDataBarsGradientFillSubItem = new BarSubItemInfo(
			   new BarInfoItems(
				   new string[] { "ConditionalFormattingDataBarGradientBlue", "ConditionalFormattingDataBarGradientGreen", "ConditionalFormattingDataBarGradientRed", "ConditionalFormattingDataBarGradientOrange", "ConditionalFormattingDataBarGradientLightBlue", "ConditionalFormattingDataBarGradientPurple" },
				   new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button }
			   )
			);
			BarSubItemInfo conditionalFormattingDataBarsSolidFillSubItem = new BarSubItemInfo(
			   new BarInfoItems(
				   new string[] { "ConditionalFormattingDataBarSolidBlue", "ConditionalFormattingDataBarSolidGreen", "ConditionalFormattingDataBarSolidRed", "ConditionalFormattingDataBarSolidOrange", "ConditionalFormattingDataBarSolidLightBlue", "ConditionalFormattingDataBarSolidPurple" },
				   new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button }
			   )
			);
			BarSubItemInfo conditionalFormattingDataBarsSubItem = new BarSubItemInfo(
			   new BarInfoItems(
				   new string[] { "ConditionalFormattingDataBarsGradientFillCommandGroup", "ConditionalFormattingDataBarsSolidFillCommandGroup" },
				   new BarItemInfo[] { conditionalFormattingDataBarsGradientFillSubItem, conditionalFormattingDataBarsSolidFillSubItem, }
			   )
			);
			BarSubItemInfo conditionalFormattingColorScalesSubItem = new BarSubItemInfo(
			   new BarInfoItems(
				   new string[] { "ConditionalFormattingColorScaleGreenYellowRed", "ConditionalFormattingColorScaleRedYellowGreen", "ConditionalFormattingColorScaleGreenWhiteRed", "ConditionalFormattingColorScaleRedWhiteGreen", "ConditionalFormattingColorScaleBlueWhiteRed", "ConditionalFormattingColorScaleRedWhiteBlue", "ConditionalFormattingColorScaleWhiteRed", "ConditionalFormattingColorScaleRedWhite", "ConditionalFormattingColorScaleGreenWhite", "ConditionalFormattingColorScaleWhiteGreen", "ConditionalFormattingColorScaleGreenYellow", "ConditionalFormattingColorScaleYellowGreen" },
				   new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button }
			   )
			);
			BarSubItemInfo conditionalFormattingIconSetsDirectionalSubItem = new BarSubItemInfo(
			   new BarInfoItems(
				   new string[] { "ConditionalFormattingIconSetArrows3Colored", "ConditionalFormattingIconSetArrows3Grayed", "ConditionalFormattingIconSetArrows4Colored", "ConditionalFormattingIconSetArrows4Grayed", "ConditionalFormattingIconSetArrows5Colored", "ConditionalFormattingIconSetArrows5Grayed", "ConditionalFormattingIconSetTriangles3" },
				   new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button }
			   )
			);
			BarSubItemInfo conditionalFormattingIconSetsShapesSubItem = new BarSubItemInfo(
			   new BarInfoItems(
				   new string[] { "ConditionalFormattingIconSetTrafficLights3", "ConditionalFormattingIconSetTrafficLights3Rimmed", "ConditionalFormattingIconSetTrafficLights4", "ConditionalFormattingIconSetSigns3", "ConditionalFormattingIconSetRedToBlack" },
				   new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button }
			   )
			);
			BarSubItemInfo conditionalFormattingIconSetsIndicatorsSubItem = new BarSubItemInfo(
			   new BarInfoItems(
				   new string[] { "ConditionalFormattingIconSetSymbols3Circled", "ConditionalFormattingIconSetSymbols3", "ConditionalFormattingIconSetFlags3" },
				   new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button }
			   )
			);
			BarSubItemInfo conditionalFormattingIconSetsRatingsSubItem = new BarSubItemInfo(
			   new BarInfoItems(
				   new string[] { "ConditionalFormattingIconSetStars3", "ConditionalFormattingIconSetRatings4", "ConditionalFormattingIconSetRatings5", "ConditionalFormattingIconSetQuarters5", "ConditionalFormattingIconSetBoxes5" },
				   new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button }
			   )
			);
			BarSubItemInfo conditionalFormattingIconSetsSubItem = new BarSubItemInfo(
			   new BarInfoItems(
				   new string[] { "ConditionalFormattingIconSetsDirectionalCommandGroup", "ConditionalFormattingIconSetsShapesCommandGroup", "ConditionalFormattingIconSetsIndicatorsCommandGroup", "ConditionalFormattingIconSetsRatingsCommandGroup" },
				   new BarItemInfo[] { conditionalFormattingIconSetsDirectionalSubItem, conditionalFormattingIconSetsShapesSubItem, conditionalFormattingIconSetsIndicatorsSubItem, conditionalFormattingIconSetsRatingsSubItem }
			   )
			);
			BarSubItemInfo conditionalFormattingRemoveSubItem = new BarSubItemInfo(
			   new BarInfoItems(
				   new string[] { "ConditionalFormattingRemoveFromSheet", "ConditionalFormattingRemove" },
				   new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button }
			   )
			);
			BarSubItemInfo conditionalFormattingSubItem = new BarSubItemInfo(
			   new BarInfoItems(
				   new string[] { "ConditionalFormattingHighlightCellsCommandGroup", "ConditionalFormattingTopBottomCommandGroup", "ConditionalFormattingDataBarsCommandGroup", "ConditionalFormattingColorScalesCommandGroup", "ConditionalFormattingIconSetsCommandGroup", "ConditionalFormattingRemoveCommandGroup" },
				   new BarItemInfo[] { conditionalFormattingHighlightCellsSubItem, conditionalFormattingTopBottomSubItem, conditionalFormattingDataBarsSubItem, conditionalFormattingColorScalesSubItem, conditionalFormattingIconSetsSubItem, conditionalFormattingRemoveSubItem }
			   )
			);
			return new BarInfo(
				String.Empty,
				"Home",
				"Styles",
				new BarInfoItems(
					new string[] { "ConditionalFormattingCommandGroup" },
					new BarItemInfo[] { conditionalFormattingSubItem }
				),
				String.Empty,
				String.Empty,
				"Caption_PageHome",
				"Caption_GroupStyles"
			);
		}
		#endregion
		#region Cells
		public static BarInfo Cells { get { return cells; } }
		static BarInfo CreatHomeCellsGroup() {
			BarSubItemInfo insertSubItem = new BarSubItemInfo(
			   new BarInfoItems(
				   new string[] { "InsertSheetRows", "InsertSheetColumns", "InsertSheet" },
				   new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button }
			   )
			);
			BarSubItemInfo removeSubItem = new BarSubItemInfo(
			   new BarInfoItems(
				   new string[] { "RemoveSheetRows", "RemoveSheetColumns", "RemoveSheet" },
				   new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button }
			   )
			);
			BarSubItemInfo hideAndUnhideSubItem = new BarSubItemInfo(
			   new BarInfoItems(
				   new string[] { "FormatHideRows", "FormatHideColumns", "FormatHideSheet", "FormatUnhideRows", "FormatUnhideColumns", "FormatUnhideSheet" },
				   new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button }
			   )
			);
			BarSubItemInfo formatSubItem = new BarSubItemInfo(
			   new BarInfoItems(
				   new string[] { "FormatRowHeight", "FormatAutoFitRowHeight", "FormatColumnWidth", "FormatAutoFitColumnWidth", "FormatDefaultColumnWidth", "FormatHideAndUnhideCommandGroup", "RenameSheet", "MoveOrCopySheet", "FormatTabColor", "ReviewProtectSheet", "FormatCellLocked", "FormatCells" },
				   new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, hideAndUnhideSubItem, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.ColorEditItem, BarItemInfos.Button, BarItemInfos.Check, BarItemInfos.Button }
			   )
			);
			return new BarInfo(
				String.Empty,
				"Home",
				"Cells",
				new BarInfoItems(
					new string[] { "InsertCellsCommandGroup", "RemoveCellsCommandGroup", "FormatCommandGroup" },
					new BarItemInfo[] { insertSubItem, removeSubItem, formatSubItem }
				),
				String.Empty,
				String.Empty,
				"Caption_PageHome",
				"Caption_GroupCells"
			);
		}
		#endregion
		#region Editing
		public static BarInfo Editing { get { return editing; } }
		static BarInfo CreateHomeEditingGroup() {
			BarSubItemInfo autoSumSubItem = new BarSubItemInfo(
			   new BarInfoItems(
				   new string[] { "FunctionsInsertSum", "FunctionsInsertAverage", "FunctionsInsertCountNumbers", "FunctionsInsertMax", "FunctionsInsertMin" },
				   new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button }
			   ),
			   Platform::DevExpress.Xpf.Bars.RibbonItemStyles.SmallWithText
			);
			BarSubItemInfo fillSubItem = new BarSubItemInfo(
			   new BarInfoItems(
				   new string[] { "EditingFillDown", "EditingFillRight", "EditingFillUp", "EditingFillLeft" },
				   new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button }
			   ),
			   Platform::DevExpress.Xpf.Bars.RibbonItemStyles.SmallWithText
			);
			BarSubItemInfo clearSubItem = new BarSubItemInfo(
			   new BarInfoItems(
				   new string[] { "FormatClearAll", "FormatClearFormats", "FormatClearContents", "FormatClearComments", "FormatClearHyperlinks", "FormatRemoveHyperlinks" },
				   new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button }
			   ),
			   Platform::DevExpress.Xpf.Bars.RibbonItemStyles.SmallWithText
			);
			BarSubItemInfo sortAndFilterSubItem = new BarSubItemInfo(
			   new BarInfoItems(
				   new string[] { "DataSortAscending", "DataSortDescending", "DataFilterToggle", "DataFilterClear", "DataFilterReApply" },
				   new BarItemInfo[] { new BarButtonItemInfoOverrideName("biEditingDataSortAscending"), new BarButtonItemInfoOverrideName("biEditingDataSortDescending"), BarItemInfos.Check, BarItemInfos.Button, BarItemInfos.Button }
			   )
			);
			BarSubItemInfo findAndSelectSubItem = new BarSubItemInfo(
			   new BarInfoItems(
				   new string[] { "EditingFind", "EditingReplace", "EditingSelectFormulas", "EditingSelectComments", "EditingSelectConditionalFormatting", "EditingSelectConstants", "EditingSelectDataValidation" },
				   new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button }
			   )
			);
			return new BarInfo(
				String.Empty,
				"Home",
				"Editing",
				new BarInfoItems(
					new string[] { "EditingAutoSumCommandGroup", "EditingFillCommandGroup", "FormatClearCommandGroup", "EditingSortAndFilterCommandGroup", "EditingFindAndSelectCommandGroup" },
					new BarItemInfo[] { autoSumSubItem, fillSubItem, clearSubItem, sortAndFilterSubItem, findAndSelectSubItem }
				),
				String.Empty,
				String.Empty,
				"Caption_PageHome",
				"Caption_GroupEditing"
			);
		}
		#endregion
	}
}
