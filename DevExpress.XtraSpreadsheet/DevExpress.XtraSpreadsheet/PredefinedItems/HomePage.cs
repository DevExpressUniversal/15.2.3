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
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.Office.UI;
using DevExpress.Utils.Text;
using System.ComponentModel;
using DevExpress.XtraEditors;
using DevExpress.Office.Model;
using DevExpress.Office.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Commands;
using DevExpress.Office.Internal;
using DevExpress.XtraBars.Ribbon;
using System.Collections.Generic;
using DevExpress.XtraBars.Commands;
using DevExpress.Office.Localization;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Design;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraBars.Ribbon.Gallery;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraBars.Commands.Internal;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.Office.Drawing;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.Export.Xl;
namespace DevExpress.XtraSpreadsheet.UI {
	#region SpreadsheetClipboardItemBuilder
	public class SpreadsheetClipboardItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContex) {
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.PasteSelection, RibbonItemStyles.Large));
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.CutSelection, RibbonItemStyles.SmallWithText));
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.CopySelection, RibbonItemStyles.SmallWithText));
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.ShowPasteSpecialForm, RibbonItemStyles.SmallWithoutText | RibbonItemStyles.SmallWithText));
		}
	}
	#endregion
	#region SpreadsheetFontItemBuilder
	public class SpreadsheetFontItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new ChangeFontNameItem());
			items.Add(new ChangeFontSizeItem());
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FormatIncreaseFontSize, HomeButtonGroups.FontNameAndSizeButtonGroup, RibbonItemStyles.SmallWithoutText));
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FormatDecreaseFontSize, HomeButtonGroups.FontNameAndSizeButtonGroup, RibbonItemStyles.SmallWithoutText));
			items.Add(SpreadsheetCommandBarCheckItem.Create(SpreadsheetCommandId.FormatFontBold, HomeButtonGroups.FontStyleButtonGroup, RibbonItemStyles.SmallWithoutText));
			items.Add(SpreadsheetCommandBarCheckItem.Create(SpreadsheetCommandId.FormatFontItalic, HomeButtonGroups.FontStyleButtonGroup, RibbonItemStyles.SmallWithoutText));
			items.Add(SpreadsheetCommandBarCheckItem.Create(SpreadsheetCommandId.FormatFontUnderline, HomeButtonGroups.FontStyleButtonGroup, RibbonItemStyles.SmallWithoutText));
			items.Add(SpreadsheetCommandBarCheckItem.Create(SpreadsheetCommandId.FormatFontStrikeout, HomeButtonGroups.FontStyleButtonGroup, RibbonItemStyles.SmallWithoutText));
			SpreadsheetCommandBarSubItem subItem = SpreadsheetCommandBarSubItem.Create(SpreadsheetCommandId.FormatBordersCommandGroup, HomeButtonGroups.FontBordersButtonGroup, RibbonItemStyles.SmallWithoutText);
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FormatBottomBorder));
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FormatTopBorder));
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FormatLeftBorder));
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FormatRightBorder));
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FormatNoBorders));
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FormatAllBorders));
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FormatOutsideBorders));
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FormatThickBorder));
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FormatBottomDoubleBorder));
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FormatBottomThickBorder));
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FormatTopAndBottomBorder));
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FormatTopAndThickBottomBorder));
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FormatTopAndDoubleBottomBorder));
			subItem.AddBarItem(new ChangeBorderLineColorItem());
			subItem.AddBarItem(new ChangeBorderLineStyleItem());
			items.Add(subItem);
			items.Add(new ChangeCellFillColorItem());
			items.Add(new ChangeFontColorItem());
		}
	}
	#endregion
	#region SpreadsheetAlignmentItemBuilder
	public class SpreadsheetAlignmentItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(SpreadsheetCommandBarCheckItem.Create(SpreadsheetCommandId.FormatAlignmentTop, HomeButtonGroups.FontVerticalAlignmentButtonGroup, RibbonItemStyles.SmallWithoutText));
			items.Add(SpreadsheetCommandBarCheckItem.Create(SpreadsheetCommandId.FormatAlignmentMiddle, HomeButtonGroups.FontVerticalAlignmentButtonGroup, RibbonItemStyles.SmallWithoutText));
			items.Add(SpreadsheetCommandBarCheckItem.Create(SpreadsheetCommandId.FormatAlignmentBottom, HomeButtonGroups.FontVerticalAlignmentButtonGroup, RibbonItemStyles.SmallWithoutText));
			items.Add(SpreadsheetCommandBarCheckItem.Create(SpreadsheetCommandId.FormatAlignmentLeft, HomeButtonGroups.FontHorizontalAlignmentButtonGroup, RibbonItemStyles.SmallWithoutText));
			items.Add(SpreadsheetCommandBarCheckItem.Create(SpreadsheetCommandId.FormatAlignmentCenter, HomeButtonGroups.FontHorizontalAlignmentButtonGroup, RibbonItemStyles.SmallWithoutText));
			items.Add(SpreadsheetCommandBarCheckItem.Create(SpreadsheetCommandId.FormatAlignmentRight, HomeButtonGroups.FontHorizontalAlignmentButtonGroup, RibbonItemStyles.SmallWithoutText));
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FormatDecreaseIndent, HomeButtonGroups.FontIndentButtonGroup, RibbonItemStyles.SmallWithoutText));
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FormatIncreaseIndent, HomeButtonGroups.FontIndentButtonGroup, RibbonItemStyles.SmallWithoutText));
			items.Add(SpreadsheetCommandBarCheckItem.Create(SpreadsheetCommandId.FormatWrapText, RibbonItemStyles.SmallWithText));
			SpreadsheetCommandBarSubItem subItem = SpreadsheetCommandBarSubItem.Create(SpreadsheetCommandId.EditingMergeCellsCommandGroup, RibbonItemStyles.SmallWithText);
			subItem.AddBarItem(SpreadsheetCommandBarCheckItem.Create(SpreadsheetCommandId.EditingMergeAndCenterCells));
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.EditingMergeCellsAcross));
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.EditingMergeCells));
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.EditingUnmergeCells));
			items.Add(subItem);
		}
	}
	#endregion
	#region SpreadsheetNumberItemBuilder
	public class SpreadsheetNumberItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(CreateChangeNumberFormatItem());
			SpreadsheetCommandBarSubItem subItem = SpreadsheetCommandBarSubItem.Create(SpreadsheetCommandId.FormatNumberAccountingCommandGroup, HomeButtonGroups.NumberFormatsButtonGroup, RibbonItemStyles.SmallWithoutText);
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FormatNumberAccountingUS));
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FormatNumberAccountingUK));
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FormatNumberAccountingEuro));
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FormatNumberAccountingPRC));
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FormatNumberAccountingSwiss));
			items.Add(subItem);
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FormatNumberPercent, HomeButtonGroups.NumberFormatsButtonGroup, RibbonItemStyles.SmallWithoutText));
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FormatNumberAccounting, HomeButtonGroups.NumberFormatsButtonGroup, RibbonItemStyles.SmallWithoutText));
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FormatNumberIncreaseDecimal, HomeButtonGroups.ChangeNumberFormatsButtonGroup, RibbonItemStyles.SmallWithoutText));
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FormatNumberDecreaseDecimal, HomeButtonGroups.ChangeNumberFormatsButtonGroup, RibbonItemStyles.SmallWithoutText));
		}
		ChangeNumberFormatItem CreateChangeNumberFormatItem() {
			ChangeNumberFormatItem result = new ChangeNumberFormatItem();
			GalleryItemGroup group = new GalleryItemGroup();
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.FormatNumberGeneral, true, true, true));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.FormatNumberDecimal, true, true, true));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.FormatNumberAccountingCurrency, true, true, true));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.FormatNumberAccountingRegular, true, true, true));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.FormatNumberShortDate, true, true, true));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.FormatNumberLongDate, true, true, true));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.FormatNumberTime, true, true, true));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.FormatNumberPercentage, true, true, true));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.FormatNumberFraction, true, true, true));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.FormatNumberScientific, true, true, true));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.FormatNumberText, true, true, true));
			result.PopupGalleryEdit.Gallery.Groups.Add(group);
			return result;
		}
	}
	#endregion
	#region SpreadsheetStylesItemBuilder
	public class SpreadsheetStylesItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			SpreadsheetCommandBarSubItem conditionalFormattingSubItem = SpreadsheetCommandBarSubItem.Create(SpreadsheetCommandId.ConditionalFormattingCommandGroup);
			SpreadsheetCommandBarSubItem highlightCellsSubItem = SpreadsheetCommandBarSubItem.Create(SpreadsheetCommandId.ConditionalFormattingHighlightCellsRuleCommandGroup);
			highlightCellsSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.ConditionalFormattingGreaterThanRuleCommand, RibbonItemStyles.Large));
			highlightCellsSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.ConditionalFormattingLessThanRuleCommand, RibbonItemStyles.Large));
			highlightCellsSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.ConditionalFormattingBetweenRuleCommand, RibbonItemStyles.Large));
			highlightCellsSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.ConditionalFormattingEqualToRuleCommand, RibbonItemStyles.Large));
			highlightCellsSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.ConditionalFormattingTextContainsRuleCommand, RibbonItemStyles.Large));
			highlightCellsSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.ConditionalFormattingDateOccurringRuleCommand, RibbonItemStyles.Large));
			highlightCellsSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.ConditionalFormattingDuplicateValuesRuleCommand, RibbonItemStyles.Large));
			conditionalFormattingSubItem.AddBarItem(highlightCellsSubItem);
			SpreadsheetCommandBarSubItem topBottomSubItem = SpreadsheetCommandBarSubItem.Create(SpreadsheetCommandId.ConditionalFormattingTopBottomRuleCommandGroup);
			topBottomSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.ConditionalFormattingTop10RuleCommand, RibbonItemStyles.Large));
			topBottomSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.ConditionalFormattingTop10PercentRuleCommand, RibbonItemStyles.Large));
			topBottomSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.ConditionalFormattingBottom10RuleCommand, RibbonItemStyles.Large));
			topBottomSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.ConditionalFormattingBottom10PercentRuleCommand, RibbonItemStyles.Large));
			topBottomSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.ConditionalFormattingAboveAverageRuleCommand, RibbonItemStyles.Large));
			topBottomSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.ConditionalFormattingBelowAverageRuleCommand, RibbonItemStyles.Large));
			conditionalFormattingSubItem.AddBarItem(topBottomSubItem);
			PopulateDataBarsItems(conditionalFormattingSubItem);
			PopulateColorScalesItems(conditionalFormattingSubItem);
			PopulateIconSetsItems(conditionalFormattingSubItem);
			SpreadsheetCommandBarSubItem removeSubItem = SpreadsheetCommandBarSubItem.Create(SpreadsheetCommandId.ConditionalFormattingRemoveCommandGroup);
			removeSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.ConditionalFormattingRemoveFromSheet));
			removeSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.ConditionalFormattingRemove));
			conditionalFormattingSubItem.AddBarItem(removeSubItem);
			items.Add(conditionalFormattingSubItem);
			items.Add(new GalleryFormatAsTableItem());
			items.Add(GetChangeStyleItem(creationContext.IsRibbon));
		}
		void PopulateDataBarsItems(SpreadsheetCommandBarSubItem conditionalFormattingSubItem) {
			SpreadsheetCommandBarButtonGalleryDropDownItem dropDown = SpreadsheetCommandBarButtonGalleryDropDownItem.Create(SpreadsheetCommandId.ConditionalFormattingDataBarsCommandGroup);
			dropDown.RibbonStyle = RibbonItemStyles.Large;
			dropDown.GalleryDropDown.Gallery.ColumnCount = 3;
			dropDown.GalleryDropDown.Gallery.RowCount = 4;
			SpreadsheetCommandGalleryItemGroup groupGradient = SpreadsheetCommandGalleryItemGroup.Create(SpreadsheetCommandId.ConditionalFormattingDataBarsGradientFillCommandGroup);
			groupGradient.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ConditionalFormattingDataBarGradientBlue));
			groupGradient.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ConditionalFormattingDataBarGradientGreen));
			groupGradient.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ConditionalFormattingDataBarGradientRed));
			groupGradient.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ConditionalFormattingDataBarGradientOrange));
			groupGradient.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ConditionalFormattingDataBarGradientLightBlue));
			groupGradient.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ConditionalFormattingDataBarGradientPurple));
			dropDown.GalleryDropDown.Gallery.Groups.Add(groupGradient);
			SpreadsheetCommandGalleryItemGroup groupSolid = SpreadsheetCommandGalleryItemGroup.Create(SpreadsheetCommandId.ConditionalFormattingDataBarsSolidFillCommandGroup);
			groupSolid.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ConditionalFormattingDataBarSolidBlue));
			groupSolid.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ConditionalFormattingDataBarSolidGreen));
			groupSolid.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ConditionalFormattingDataBarSolidRed));
			groupSolid.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ConditionalFormattingDataBarSolidOrange));
			groupSolid.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ConditionalFormattingDataBarSolidLightBlue));
			groupSolid.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ConditionalFormattingDataBarSolidPurple));
			dropDown.GalleryDropDown.Gallery.Groups.Add(groupSolid);
			conditionalFormattingSubItem.AddBarItem(dropDown);
		}
		void PopulateColorScalesItems(SpreadsheetCommandBarSubItem conditionalFormattingSubItem) {
			SpreadsheetCommandBarButtonGalleryDropDownItem dropDown = SpreadsheetCommandBarButtonGalleryDropDownItem.Create(SpreadsheetCommandId.ConditionalFormattingColorScalesCommandGroup);
			dropDown.RibbonStyle = RibbonItemStyles.Large;
			dropDown.GalleryDropDown.Gallery.ColumnCount = 4;
			dropDown.GalleryDropDown.Gallery.RowCount = 3;
			dropDown.GalleryDropDown.Gallery.ShowGroupCaption = false;
			SpreadsheetCommandGalleryItemGroup group = SpreadsheetCommandGalleryItemGroup.Create(SpreadsheetCommandId.ConditionalFormattingColorScalesCommandGroup);
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ConditionalFormattingColorScaleGreenYellowRed));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ConditionalFormattingColorScaleRedYellowGreen));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ConditionalFormattingColorScaleGreenWhiteRed));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ConditionalFormattingColorScaleRedWhiteGreen));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ConditionalFormattingColorScaleBlueWhiteRed));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ConditionalFormattingColorScaleRedWhiteBlue));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ConditionalFormattingColorScaleWhiteRed));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ConditionalFormattingColorScaleRedWhite));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ConditionalFormattingColorScaleGreenWhite));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ConditionalFormattingColorScaleWhiteGreen));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ConditionalFormattingColorScaleGreenYellow));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ConditionalFormattingColorScaleYellowGreen));
			dropDown.GalleryDropDown.Gallery.Groups.Add(group);
			conditionalFormattingSubItem.AddBarItem(dropDown);
		}
		void PopulateIconSetsItems(SpreadsheetCommandBarSubItem conditionalFormattingSubItem) {
			SpreadsheetCommandBarButtonGalleryDropDownItem dropDown = SpreadsheetCommandBarButtonGalleryDropDownItem.Create(SpreadsheetCommandId.ConditionalFormattingIconSetsCommandGroup);
			dropDown.RibbonStyle = RibbonItemStyles.Large;
			dropDown.GalleryDropDown.Gallery.ColumnCount = 4;
			dropDown.GalleryDropDown.Gallery.RowCount = 7;
			SpreadsheetCommandGalleryItemGroup group;
			group = SpreadsheetCommandGalleryItemGroup.Create(SpreadsheetCommandId.ConditionalFormattingIconSetsDirectionalCommandGroup);
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ConditionalFormattingIconSetArrows3Colored));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ConditionalFormattingIconSetArrows3Grayed));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ConditionalFormattingIconSetArrows4Colored));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ConditionalFormattingIconSetArrows4Grayed));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ConditionalFormattingIconSetArrows5Colored));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ConditionalFormattingIconSetArrows5Grayed));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ConditionalFormattingIconSetTriangles3));
			dropDown.GalleryDropDown.Gallery.Groups.Add(group);
			group = SpreadsheetCommandGalleryItemGroup.Create(SpreadsheetCommandId.ConditionalFormattingIconSetsShapesCommandGroup);
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ConditionalFormattingIconSetTrafficLights3));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ConditionalFormattingIconSetTrafficLights3Rimmed));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ConditionalFormattingIconSetTrafficLights4));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ConditionalFormattingIconSetSigns3));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ConditionalFormattingIconSetRedToBlack));
			dropDown.GalleryDropDown.Gallery.Groups.Add(group);
			group = SpreadsheetCommandGalleryItemGroup.Create(SpreadsheetCommandId.ConditionalFormattingIconSetsIndicatorsCommandGroup);
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ConditionalFormattingIconSetSymbols3Circled));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ConditionalFormattingIconSetSymbols3));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ConditionalFormattingIconSetFlags3));
			dropDown.GalleryDropDown.Gallery.Groups.Add(group);
			group = SpreadsheetCommandGalleryItemGroup.Create(SpreadsheetCommandId.ConditionalFormattingIconSetsRatingsCommandGroup);
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ConditionalFormattingIconSetStars3));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ConditionalFormattingIconSetRatings4));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ConditionalFormattingIconSetRatings5));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ConditionalFormattingIconSetQuarters5));
			group.Items.Add(SpreadsheetCommandGalleryItem.Create(SpreadsheetCommandId.ConditionalFormattingIconSetBoxes5));
			dropDown.GalleryDropDown.Gallery.Groups.Add(group);
			conditionalFormattingSubItem.AddBarItem(dropDown);
		}
		BarItem GetChangeStyleItem(bool isRibbon) {
			if (!isRibbon)
				return new ChangeStyleItem();
			return new GalleryChangeStyleItem();
		}
	}
	#endregion
	#region SpreadsheetCellsItemBuilder
	public class SpreadsheetCellsItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			SpreadsheetCommandBarSubItem insertSubItem = SpreadsheetCommandBarSubItem.Create(SpreadsheetCommandId.InsertCellsCommandGroup);
			insertSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.InsertSheetRows));
			insertSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.InsertSheetColumns));
			insertSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.InsertSheet));
			items.Add(insertSubItem);
			SpreadsheetCommandBarSubItem removeSubItem = SpreadsheetCommandBarSubItem.Create(SpreadsheetCommandId.RemoveCellsCommandGroup);
			removeSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.RemoveSheetRows));
			removeSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.RemoveSheetColumns));
			removeSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.RemoveSheet));
			items.Add(removeSubItem);
			SpreadsheetCommandBarSubItem formatSubItem = SpreadsheetCommandBarSubItem.Create(SpreadsheetCommandId.FormatCommandGroup);
			formatSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FormatRowHeight));
			formatSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FormatAutoFitRowHeight));
			formatSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FormatColumnWidth));
			formatSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FormatAutoFitColumnWidth));
			formatSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FormatDefaultColumnWidth));
			SpreadsheetCommandBarSubItem hideAndUnhideSubItem = SpreadsheetCommandBarSubItem.Create(SpreadsheetCommandId.HideAndUnhideCommandGroup);
			hideAndUnhideSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.HideRows));
			hideAndUnhideSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.HideColumns));
			hideAndUnhideSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.HideSheet));
			hideAndUnhideSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.UnhideRows));
			hideAndUnhideSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.UnhideColumns));
			hideAndUnhideSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.UnhideSheet));
			formatSubItem.AddBarItem(hideAndUnhideSubItem);
			formatSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.RenameSheet));
			formatSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.MoveOrCopySheet));
			formatSubItem.AddBarItem(new ChangeSheetTabColorItem());
			formatSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.ReviewProtectSheet));
			formatSubItem.AddBarItem(SpreadsheetCommandBarCheckItem.Create(SpreadsheetCommandId.FormatCellLocked));
			formatSubItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FormatCellsContextMenuItem));
			items.Add(formatSubItem);
		}
	}
	#endregion
	#region SpreadsheetEditingItemBuilder
	public class SpreadsheetEditingItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			SpreadsheetCommandBarSubItem subItem = SpreadsheetCommandBarSubItem.Create(SpreadsheetCommandId.EditingAutoSumCommandGroup, RibbonItemStyles.SmallWithText);
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FunctionsInsertSum));
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FunctionsInsertAverage));
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FunctionsInsertCountNumbers));
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FunctionsInsertMax));
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FunctionsInsertMin));
			items.Add(subItem);
			subItem = SpreadsheetCommandBarSubItem.Create(SpreadsheetCommandId.EditingFillCommandGroup, RibbonItemStyles.SmallWithText);
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.EditingFillDown));
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.EditingFillRight));
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.EditingFillUp));
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.EditingFillLeft));
			items.Add(subItem);
			subItem = SpreadsheetCommandBarSubItem.Create(SpreadsheetCommandId.FormatClearCommandGroup, RibbonItemStyles.SmallWithText);
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FormatClearAll));
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FormatClearFormats));
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FormatClearContents));
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FormatClearComments));
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FormatClearHyperlinks));
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FormatRemoveHyperlinks));
			items.Add(subItem);
			subItem = SpreadsheetCommandBarSubItem.Create(SpreadsheetCommandId.EditingSortAndFilterCommandGroup);
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.DataSortAscending));
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.DataSortDescending));
			subItem.AddBarItem(SpreadsheetCommandBarCheckItem.Create(SpreadsheetCommandId.DataFilterToggle));
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.DataFilterClear));
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.DataFilterReApply));
			items.Add(subItem);
			subItem = SpreadsheetCommandBarSubItem.Create(SpreadsheetCommandId.EditingFindAndSelectCommandGroup);
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.EditingFind));
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.EditingReplace));
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.EditingSelectFormulas));
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.EditingSelectComments));
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.EditingSelectConditionalFormatting));
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.EditingSelectConstants));
			subItem.AddBarItem(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.EditingSelectDataValidation));
			items.Add(subItem);
		}
	}
	#endregion
	#region SpreadsheetClipboardBarCreator
	public class SpreadsheetClipboardBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(HomeRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(ClipboardRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(ClipboardBar); } }
		public override int DockRow { get { return 0; } }
		public override int DockColumn { get { return 1; } }
		public override Bar CreateBar() {
			return new ClipboardBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SpreadsheetClipboardItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new HomeRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new ClipboardRibbonPageGroup();
		}
	}
	#endregion
	#region SpreadsheetFontBarCreator
	public class SpreadsheetFontBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(HomeRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(FontRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(FontBar); } }
		public override int DockRow { get { return 0; } }
		public override int DockColumn { get { return 2; } }
		public override Bar CreateBar() {
			return new FontBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SpreadsheetFontItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new HomeRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new FontRibbonPageGroup();
		}
	}
	#endregion
	#region SpreadsheetAlignmentBarCreator
	public class SpreadsheetAlignmentBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(HomeRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(AlignmentRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(AlignmentBar); } }
		public override int DockRow { get { return 1; } }
		public override int DockColumn { get { return 0; } }
		public override Bar CreateBar() {
			return new AlignmentBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SpreadsheetAlignmentItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new HomeRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new AlignmentRibbonPageGroup();
		}
	}
	#endregion
	#region SpreadsheetNumberBarCreator
	public class SpreadsheetNumberBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(HomeRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(NumberRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(NumberBar); } }
		public override int DockRow { get { return 1; } }
		public override int DockColumn { get { return 1; } }
		public override Bar CreateBar() {
			return new NumberBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SpreadsheetNumberItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new HomeRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new NumberRibbonPageGroup();
		}
	}
	#endregion
	#region SpreadsheetStylesBarCreator
	public class SpreadsheetStylesBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(HomeRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(StylesRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(StylesBar); } }
		public override int DockRow { get { return 0; } }
		public override int DockColumn { get { return 3; } }
		public override Bar CreateBar() {
			return new StylesBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SpreadsheetStylesItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new HomeRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new StylesRibbonPageGroup();
		}
	}
	#endregion
	#region SpreadsheetCellsBarCreator
	public class SpreadsheetCellsBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(HomeRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(CellsRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(CellsBar); } }
		public override int DockRow { get { return 1; } }
		public override int DockColumn { get { return 2; } }
		public override Bar CreateBar() {
			return new CellsBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SpreadsheetCellsItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new HomeRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new CellsRibbonPageGroup();
		}
	}
	#endregion
	#region SpreadsheetEditingBarCreator
	public class SpreadsheetEditingBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(HomeRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(EditingRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(EditingBar); } }
		public override int DockRow { get { return 1; } }
		public override int DockColumn { get { return 3; } }
		public override Bar CreateBar() {
			return new EditingBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SpreadsheetEditingItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new HomeRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new EditingRibbonPageGroup();
		}
	}
	#endregion
	#region ClipboardBar
	public class ClipboardBar : ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public ClipboardBar() {
		}
		public ClipboardBar(BarManager manager)
			: base(manager) {
		}
		public ClipboardBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupClipboard); } }
	}
	#endregion
	#region FontBar
	public class FontBar : ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public FontBar() {
		}
		public FontBar(BarManager manager)
			: base(manager) {
		}
		public FontBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupFont); } }
	}
	#endregion
	#region AlignmentBar
	public class AlignmentBar : ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public AlignmentBar() {
		}
		public AlignmentBar(BarManager manager)
			: base(manager) {
		}
		public AlignmentBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupAlignment); } }
	}
	#endregion
	#region NumberBar
	public class NumberBar : ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public NumberBar() {
		}
		public NumberBar(BarManager manager)
			: base(manager) {
		}
		public NumberBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupNumber); } }
	}
	#endregion
	#region StylesBar
	public class StylesBar : ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public StylesBar() {
		}
		public StylesBar(BarManager manager)
			: base(manager) {
		}
		public StylesBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupStyles); } }
	}
	#endregion
	#region CellsBar
	public class CellsBar : ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public CellsBar() {
		}
		public CellsBar(BarManager manager)
			: base(manager) {
		}
		public CellsBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupCells); } }
	}
	#endregion
	#region EditingBar
	public class EditingBar : ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public EditingBar() {
		}
		public EditingBar(BarManager manager)
			: base(manager) {
		}
		public EditingBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupEditing); } }
	}
	#endregion
	#region HomeRibbonPage
	public class HomeRibbonPage : ControlCommandBasedRibbonPage {
		public HomeRibbonPage() {
		}
		public HomeRibbonPage(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PageHome); } }
	}
	#endregion
	#region ClipboardRibbonPageGroup
	public class ClipboardRibbonPageGroup : SpreadsheetControlRibbonPageGroup {
		public ClipboardRibbonPageGroup() {
		}
		public ClipboardRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupClipboard); } }
	}
	#endregion
	#region FontRibbonPageGroup
	public class FontRibbonPageGroup : SpreadsheetControlRibbonPageGroup {
		public FontRibbonPageGroup() {
		}
		public FontRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupFont); } }
		public override SpreadsheetCommandId CommandId { get { return SpreadsheetCommandId.FormatCellsFont; } }
	}
	#endregion
	#region AlignmentRibbonPageGroup
	public class AlignmentRibbonPageGroup : SpreadsheetControlRibbonPageGroup {
		public AlignmentRibbonPageGroup() {
		}
		public AlignmentRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupAlignment); } }
		public override SpreadsheetCommandId CommandId { get { return SpreadsheetCommandId.FormatCellsAlignment; } }
	}
	#endregion
	#region NumberRibbonPageGroup
	public class NumberRibbonPageGroup : SpreadsheetControlRibbonPageGroup {
		public NumberRibbonPageGroup() {
		}
		public NumberRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupNumber); } }
		public override SpreadsheetCommandId CommandId { get { return SpreadsheetCommandId.FormatCellsNumber; } }
	}
	#endregion
	#region StylesRibbonPageGroup
	public class StylesRibbonPageGroup : SpreadsheetControlRibbonPageGroup {
		public StylesRibbonPageGroup() {
		}
		public StylesRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupStyles); } }
		protected override void SetPage(RibbonPage page) {
			base.SetPage(page);
			AddReduceOperation(page);
		}
		protected internal virtual void AddReduceOperation(RibbonPage page) {
			if (!DesignMode || Ribbon == null)
				return;
			ReduceOperation operation = new ReduceOperation();
			operation.Operation = ReduceOperationType.Gallery;
			operation.Behavior = ReduceOperationBehavior.UntilAvailable;
			operation.Group = this;
			operation.ItemLinkIndex = 2;
			page.ReduceOperations.Add(operation);
		}
	}
	#endregion
	#region CellsRibbonPageGroup
	public class CellsRibbonPageGroup : SpreadsheetControlRibbonPageGroup {
		public CellsRibbonPageGroup() {
		}
		public CellsRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupCells); } }
	}
	#endregion
	#region EditingRibbonPageGroup
	public class EditingRibbonPageGroup : SpreadsheetControlRibbonPageGroup {
		public EditingRibbonPageGroup() {
		}
		public EditingRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupEditing); } }
	}
	#endregion
	#region ChangeFontNameItem
	public class ChangeFontNameItem : SpreadsheetCommandBarEditItem<string>, IBarButtonGroupMember {
		const int defaultWidth = 130;
		public ChangeFontNameItem() {
			Width = defaultWidth;
		}
		public ChangeFontNameItem(BarManager manager, string caption)
			: base(manager, caption) {
			Width = defaultWidth;
		}
		[DefaultValue(defaultWidth)]
		public override int Width { get { return base.Width; } set { base.Width = value; } }
		protected override SpreadsheetCommandId CommandId { get { return SpreadsheetCommandId.FormatFontName; } }
		protected override RepositoryItem CreateEdit() {
			return new RepositoryItemFontEdit();
		}
		#region IBarButtonGroupMember Members
		object IBarButtonGroupMember.ButtonGroupTag { get { return HomeButtonGroups.FontNameAndSizeButtonGroup; } }
		#endregion
	}
	#endregion
	#region ChangeFontSizeItem
	public class ChangeFontSizeItem : SpreadsheetCommandBarEditItem<double>, IBarButtonGroupMember {
		public ChangeFontSizeItem() {
		}
		public ChangeFontSizeItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override SpreadsheetCommandId CommandId { get { return SpreadsheetCommandId.FormatFontSize; } }
		protected override ICommandUIState CreateCommandUIState(Command command) {
			DefaultValueBasedCommandUIState<double> value = new DefaultValueBasedCommandUIState<double>();
			double editValue = 0;
			if (EditValue != null)
				double.TryParse(EditValue.ToString(), out editValue);
			value.Value = editValue;
			return value;
		}
		protected override RepositoryItem CreateEdit() {
			RepositoryItemSpreadsheetFontSizeEdit edit = new RepositoryItemSpreadsheetFontSizeEdit();
			if (Control != null)
				edit.Control = Control;
			return edit;
		}
		protected override void OnControlChanged() {
			RepositoryItemSpreadsheetFontSizeEdit edit = (RepositoryItemSpreadsheetFontSizeEdit)Edit;
			if (edit != null)
				edit.Control = Control;
		}
		protected override void OnEditValidating(object sender, CancelEventArgs e) {
			ComboBoxEdit edit = (ComboBoxEdit)sender;
			double newValue;
			const double minSize = 1;
			const double maxSize = 409.55;
			if (double.TryParse(edit.EditValue.ToString(), out newValue)) {
				if (newValue < minSize || newValue > maxSize) {
					edit.ErrorText = String.Format(OfficeLocalizer.GetString(OfficeStringId.Msg_InvalidFontSize), minSize, maxSize);
					e.Cancel = true;
				}
			}
			else {
				edit.ErrorText = OfficeLocalizer.GetString(OfficeStringId.Msg_InvalidNumber);
				e.Cancel = true;
			}
		}
		#region IBarButtonGroupMember Members
		object IBarButtonGroupMember.ButtonGroupTag { get { return HomeButtonGroups.FontNameAndSizeButtonGroup; } }
		#endregion
	}
	#endregion
	#region ChangeFontColorItem
	public class ChangeFontColorItem : ChangeColorItemBase<SpreadsheetControl, SpreadsheetCommandId>, IBarButtonGroupMember {
		public ChangeFontColorItem() {
		}
		public ChangeFontColorItem(BarManager manager)
			: base(manager) {
		}
		public ChangeFontColorItem(string caption)
			: base(caption) {
		}
		public ChangeFontColorItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override SpreadsheetCommandId CommandId { get { return SpreadsheetCommandId.None; } }
		protected override string DefaultColorButtonCaption { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_ColorAutomatic); } }
		protected override Command CreateCommand() {
			if (Control == null)
				return null;
			Command command = new FormatFontColorCommand(Control);
			command.CommandSourceType = CommandSourceType.Menu;
			return command;
		}
		#region IBarButtonGroupMember Members
		object IBarButtonGroupMember.ButtonGroupTag { get { return HomeButtonGroups.FontColorButtonGroup; } }
		#endregion
	}
	#endregion
	#region ChangeCellFillColorItem
	public class ChangeCellFillColorItem : ChangeColorItemBase<SpreadsheetControl, SpreadsheetCommandId>, IBarButtonGroupMember {
		public ChangeCellFillColorItem() {
		}
		public ChangeCellFillColorItem(BarManager manager)
			: base(manager) {
		}
		public ChangeCellFillColorItem(string caption)
			: base(caption) {
		}
		public ChangeCellFillColorItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override SpreadsheetCommandId CommandId { get { return SpreadsheetCommandId.None; } }
		protected override string DefaultColorButtonCaption { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_NoFill); } }
		protected override Command CreateCommand() {
			if (Control == null)
				return null;
			Command command = new FormatFillColorCommand(Control);
			command.CommandSourceType = CommandSourceType.Menu;
			return command;
		}
		#region IBarButtonGroupMember Members
		object IBarButtonGroupMember.ButtonGroupTag { get { return HomeButtonGroups.FontColorButtonGroup; } }
		#endregion
	}
	#endregion
	#region ChangeNumberFormatItem
	public class ChangeNumberFormatItem : SpreadsheetCommandBarPopupGalleryEditItem<string>, IBarButtonGroupMember {
		#region Fields
		const int defaultWidth = 130;
		const int galleryRowCount = 11;
		const int galleryColumnCount = 1;
		#endregion
		public ChangeNumberFormatItem()
			: base() {
			Width = defaultWidth;
		}
		public ChangeNumberFormatItem(BarManager manager, string caption)
			: base(manager, caption) {
			Width = defaultWidth;
		}
		#region Properties
		[DefaultValue(defaultWidth)]
		public override int Width { get { return base.Width; } set { base.Width = value; } }
		protected override SpreadsheetCommandId CommandId { get { return SpreadsheetCommandId.FormatNumber; } }
		#endregion
		protected override void InitializeGallery(PopupGalleryEditGallery gallery) {
			base.InitializeGallery(gallery);
			gallery.RowCount = galleryRowCount;
			gallery.ColumnCount = galleryColumnCount;
		}
		#region IBarButtonGroupMember Members
		object IBarButtonGroupMember.ButtonGroupTag { get { return HomeButtonGroups.NumberFormatMainButtonGroup; } }
		#endregion
	}
	#endregion
	#region ChangeBorderLineColorItem
	public class ChangeBorderLineColorItem : ChangeColorItemBase<SpreadsheetControl, SpreadsheetCommandId> {
		public ChangeBorderLineColorItem() {
		}
		public ChangeBorderLineColorItem(BarManager manager)
			: base(manager) {
		}
		public ChangeBorderLineColorItem(string caption)
			: base(caption) {
		}
		public ChangeBorderLineColorItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		#region Properties
		protected override SpreadsheetCommandId CommandId { get { return SpreadsheetCommandId.None; } }
		protected override string DefaultColorButtonCaption { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_ColorAutomatic); } }
		public override bool ActAsDropDown { get { return true; } set { } }
		#endregion
		protected override Command CreateCommand() {
			if (Control == null)
				return null;
			Command command = new FormatBorderLineColorCommand(Control);
			command.CommandSourceType = CommandSourceType.Menu;
			return command;
		}
	}
	#endregion
	#region ChangeBorderLineStyleItem
	public class ChangeBorderLineStyleItem : ControlCommandBarButtonGalleryDropDownItem<SpreadsheetControl, SpreadsheetCommandId>, IPatternLinePaintingSupport {
		#region Fields
		const int galleryColumnCount = 1;
		const int galleryRowCount = 14;
		Size itemSize = new Size(136, 26);
		const GalleryItemAutoSizeMode itemAutoSizeMode = GalleryItemAutoSizeMode.None;
		static StringFormat format = CreateFormat();
		GraphicsCache cache;
		SpreadsheetHorizontalPatternLinePainter linePainter;
		UIBorderInfoRepository repository;
		const int lineOffset = 16;
		#endregion
		public ChangeBorderLineStyleItem()
			: base() {
		}
		public ChangeBorderLineStyleItem(string caption)
			: base(caption) {
		}
		public ChangeBorderLineStyleItem(BarManager manager)
			: base(manager, string.Empty) {
		}
		public ChangeBorderLineStyleItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		#region Properties
		protected override SpreadsheetCommandId CommandId { get { return SpreadsheetCommandId.FormatBorderLineStyle; } }
		protected override int GalleryColumnCount { get { return galleryColumnCount; } }
		protected override int GalleryRowCount { get { return galleryRowCount; } }
		protected override bool ShowGroupCaption { get { return false; } }
		protected override bool ShowItemText { get { return true; } }
		#endregion
		static StringFormat CreateFormat() {
			StringFormat result = (StringFormat)StringFormat.GenericDefault.Clone();
			result.LineAlignment = StringAlignment.Center;
			result.Trimming = StringTrimming.EllipsisCharacter;
			return result;
		}
		protected override void InitializeDropDownGallery(InDropDownGallery gallery) {
			base.InitializeDropDownGallery(gallery);
			gallery.ItemSize = itemSize;
			gallery.ItemAutoSizeMode = itemAutoSizeMode;
			gallery.DrawImageBackground = false;
		}
		void PopulateGalleryItems() {
			if (DesignMode)
				return;
			PopulateGalleryItemsCore();
		}
		void PopulateGalleryItemsCore() {
			GalleryItemGroup group = new GalleryItemGroup();
			foreach (UIBorderInfoItem item in repository.Items) {
				GalleryItem galleryItem = new GalleryItem();
				galleryItem.Tag = item.LineStyle;
				group.Items.Add(galleryItem);
			}
			GalleryDropDown.Gallery.Groups.Clear();
			GalleryDropDown.Gallery.Groups.Add(group);
		}
		protected override void SubscribeGalleryEvents() {
			GalleryDropDown.GalleryItemClick += OnGalleryItemClick;
			GalleryDropDown.GalleryCustomDrawItemText += OnGalleryCustomDrawItemText;
		}
		protected override void UnsubscribeGalleryEvents() {
			GalleryDropDown.GalleryItemClick -= OnGalleryItemClick;
			GalleryDropDown.GalleryCustomDrawItemText += OnGalleryCustomDrawItemText;
		}
		void OnGalleryItemClick(object sender, GalleryItemClickEventArgs e) {
			XlBorderLineStyle lineStyle = (XlBorderLineStyle)e.Item.Tag;
			InvokeCommand(lineStyle);
		}
		protected override void InvokeCommand() {
		}
		protected internal void InvokeCommand(XlBorderLineStyle lineStyle) {
			Command command = CreateCommand();
			command.CommandSourceType = CommandSourceType.Menu;
			if (command != null) {
				ICommandUIState state = CreateCommandUIState(lineStyle);
				if (command.CanExecute())
					command.ForceExecute(state);
			}
		}
		protected internal ICommandUIState CreateCommandUIState(XlBorderLineStyle lineStyle) {
			DefaultValueBasedCommandUIState<XlBorderLineStyle> state = new DefaultValueBasedCommandUIState<XlBorderLineStyle>();
			state.Value = lineStyle;
			return state;
		}
		void OnGalleryCustomDrawItemText(object sender, GalleryItemCustomDrawEventArgs e) {
			GalleryItemViewInfo viewInfo = e.ItemInfo as GalleryItemViewInfo;
			if (viewInfo == null)
				return;
			this.cache = e.Cache;
			Rectangle bounds = Rectangle.Inflate(viewInfo.Bounds, -lineOffset, 0);
			XlBorderLineStyle lineStyle = (XlBorderLineStyle)e.Item.Tag;
			if (lineStyle == XlBorderLineStyle.None) {
				DrawNoBorderString(bounds, viewInfo);
				e.Handled = true;
				return;
			}
			BorderLine borderLine = DocumentModel.DefaultBorderLineRepository.GetPatternLineByType(lineStyle);
			int lineThickness = BorderInfo.LinePixelThicknessTable[lineStyle];
			Rectangle borderBounds = new Rectangle(bounds.X, bounds.Y + bounds.Height / 2, bounds.Width, lineThickness);
			borderLine.Draw(linePainter, borderBounds, Color.Black);
			e.Handled = true;
		}
		void DrawNoBorderString(Rectangle bounds, GalleryItemViewInfo viewInfo) {
			string noBorder = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_FormatNoBorders);
			cache.DrawString(noBorder, GetNoBorderStringFont(), GetNoBorderBrush(viewInfo), bounds, format);
		}
		Font GetNoBorderStringFont() {
			return this.Font;
		}
		Brush GetNoBorderBrush(GalleryItemViewInfo viewInfo) {
			Color color = viewInfo.PaintAppearance.ItemCaptionAppearance.GetAppearance(ObjectState.Normal).ForeColor;
			return cache.GetSolidBrush(color);
		}
		protected override void OnControlChanged() {
			if (Control == null)
				return;
			DocumentModel documentModel = Control.DocumentModel;
			CreateLinePainter(documentModel);
			this.repository = documentModel.UiBorderInfoRepository;
			PopulateGalleryItems();
		}
		void CreateLinePainter(DocumentModel documentModel) {
			if (this.linePainter == null)
				this.linePainter = new SpreadsheetHorizontalPatternLinePainter(this, documentModel.LayoutUnitConverter);
		}
		protected override ICommandUIState CreateButtonItemUIState() {
			return new BarButtonGalleryDropDownItemValueUIState<XlBorderLineStyle>(this);
		}
		#region IPatternLinePaintingSupport Members
		public void DrawLine(Pen pen, float x1, float y1, float x2, float y2) {
			cache.Graphics.DrawLine(pen, x1, y1, x2, y2);
		}
		public void DrawLines(Pen pen, PointF[] points) {
			cache.Graphics.DrawLines(pen, points);
		}
		public Brush GetBrush(Color color) {
			return cache.GetSolidBrush(color);
		}
		public Pen GetPen(Color color, float thickness) {
			return new Pen(color, thickness);
		}
		public Pen GetPen(Color color) {
			return new Pen(color);
		}
		public void ReleaseBrush(Brush brush) {
			brush.Dispose();
		}
		public void ReleasePen(Pen pen) {
			pen.Dispose();
		}
		#endregion
	}
	#endregion
	#region GalleryFormatAsTableItem
	public class GalleryFormatAsTableItem : SpreadsheetCommandBarButtonGalleryDropDownItem {
		#region Fields
		IList<string> customStyleNames;
		bool isPivotGallery = false;
		bool hasActiveTable = false;
		#endregion
		public GalleryFormatAsTableItem() {
		}
		#region Properties
		protected override SpreadsheetCommandId CommandId { get { return SpreadsheetCommandId.FormatAsTable; } }
		DocumentModel DocumentModel { get { return Control.DocumentModel; } }
		#endregion
		protected override void Initialize() {
			base.Initialize();
			GalleryDropDown.Gallery.AllowFilter = false;
			GalleryDropDown.Gallery.ColumnCount = GalleryTableStylesItem.MaxDropDownColumnCount;
			GalleryDropDown.Gallery.RowCount = GalleryTableStylesItem.RowCount;
			GalleryDropDown.Gallery.ItemSize = GalleryTableStylesItem.DefaultItemSize;
			GalleryDropDown.Gallery.ItemAutoSizeMode = GalleryTableStylesItem.ItemAutoSizeMode;
			GalleryDropDown.Gallery.ShowItemText = false;
			GalleryDropDown.Gallery.DrawImageBackground = false;
		}
		protected override void SubscribeGalleryEvents() {
			GalleryDropDown.Gallery.CustomDrawItemImage += OnCustomDrawItem;
		}
		protected override void SubscribeControlEvents() {
			base.SubscribeControlEvents();
			Control.ModifiedChanged += OnCollectionChanged;
			Control.DocumentLoaded += OnCollectionChanged;
			Control.EmptyDocumentCreated += OnCollectionChanged;
			DocumentModel.DocumentCleared += OnCollectionChanged;
			DocumentModel.StyleSheet.TableStyles.CollectionChanged += OnCollectionChanged;
			DropDownControl.Popup += DropDownControl_Popup;
		}
		void DropDownControl_Popup(object sender, EventArgs e) {
			if (DesignMode)
				return;
			ITableBase activeTable = DocumentModel.ActiveSheet.TryGetActiveTableBase();
			hasActiveTable = activeTable != null;
			if (hasActiveTable) {
				bool isPivotGallery = activeTable is PivotTable;
				if (isPivotGallery && isPivotGallery != this.isPivotGallery) {
					this.isPivotGallery = true;
					PopulateDropDownPivotStyleGallery();
				} if (!isPivotGallery && isPivotGallery != this.isPivotGallery) {
					this.isPivotGallery = false;
					PopulateDropDownTableStyleGallery();
				}
				if (!isPivotGallery && isPivotGallery == this.isPivotGallery) {
					this.isPivotGallery = false;
				}
			} 
		}
		protected override void UnsubscribeControlEvents() {
			base.UnsubscribeControlEvents();
			Control.ModifiedChanged -= OnCollectionChanged;
			Control.DocumentLoaded -= OnCollectionChanged;
			Control.EmptyDocumentCreated -= OnCollectionChanged;
			DocumentModel.DocumentCleared -= OnCollectionChanged;
			DocumentModel.StyleSheet.TableStyles.CollectionChanged -= OnCollectionChanged;
			DropDownControl.Popup -= DropDownControl_Popup;
		}
		void OnCollectionChanged(object sender, EventArgs e) {
			if (DesignMode)
				return;
			if (hasActiveTable && isPivotGallery)
				PopulateDropDownPivotStyleGallery();
			else
				PopulateDropDownTableStyleGallery();
		}
		void PopulateDropDownTableStyleGallery() {
			customStyleNames = DocumentModel.StyleSheet.TableStyles.GetExistingNonHiddenCustomTableStyleNames();
			PopulateDropDownTableStyleGalleryCore();
		}
		void PopulateDropDownTableStyleGalleryCore() {
			GalleryTableStylesItem.InitDropDownGalleryCore(GalleryDropDown.Gallery, customStyleNames);
		}
		void PopulateDropDownPivotStyleGallery() {
			customStyleNames = DocumentModel.StyleSheet.TableStyles.GetExistingNonHiddenCustomPivotStyleNames();
			GalleryPivotStylesItem.InitDropDownGalleryCore(GalleryDropDown.Gallery, customStyleNames);
		}
		protected override void InitializeDropDownGallery(InDropDownGallery gallery) {
			if (DesignMode)
				return;
			PopulateDropDownTableStyleGalleryCore();
		}
		void OnCustomDrawItem(object sender, GalleryItemCustomDrawEventArgs e) {
			if (Control == null || Control.IsDisposed)
				return;
			if (hasActiveTable && isPivotGallery)
				GalleryPivotStylesItem.OnCustomDrawItemCore(e, DocumentModel);
			else
				GalleryTableStylesItem.OnCustomDrawItemCore(e, DocumentModel);
		}
		public override void InvokeSelectedGalleryItemCommand(GalleryItem item) {
			Command command = CreateCommand();
			if (command != null) {
				ICommandUIState state = CreateCommandUIState(command, item);
				if (command.CanExecute())
					command.ForceExecute(state);
			}
		}
		protected override void InvokeCommand() {
		}
		ICommandUIState CreateCommandUIState(Command command, GalleryItem item) {
			if (!hasActiveTable) {
				DefaultValueBasedCommandUIState<InsertTableViewModel> result = new DefaultValueBasedCommandUIState<InsertTableViewModel>();
				FormatAsTableViewModel viewModel = new FormatAsTableViewModel(Control);
				viewModel.Reference = InsertTableUICommand.GetReferenceCommon(DocumentModel.ActiveSheet.Selection.ActiveRange, DocumentModel.DataContext);
				viewModel.Style = item.Tag as string;
				viewModel.HasHeaders = false;
				result.Value = viewModel;
				return result;
			}
			else {
				DefaultValueBasedCommandUIState<string> result = new DefaultValueBasedCommandUIState<string>();
				result.Value = item.Tag as string;
				return result;
			}
		}
	}
	#endregion
	#region GalleryChangeStyleItem
	public class GalleryChangeStyleItem : SpreadsheetCommandGalleryBarItem {
		#region Fields
		static StringFormat styleNameFormat = CreateStyleNameFormat();
		RepositoryItemSpreadsheetCellStyleEdit repository;
		Size itemSize = new Size(106, 28);
		const GalleryItemAutoSizeMode itemAutoSizeMode = GalleryItemAutoSizeMode.None;
		const int minColumnCount = 1;
		const int maxColumnCount = 5;
		const int maxDropDownColumnCount = 6;
		const int rowCount = 9;
		const int selectionWidth = 3;
		const int maxFontSize = 18;
		#endregion
		static StringFormat CreateStyleNameFormat() {
			StringFormat result = (StringFormat)StringFormat.GenericDefault.Clone();
			result.LineAlignment = StringAlignment.Center;
			result.Trimming = StringTrimming.EllipsisCharacter;
			return result;
		}
		#region GalleryItemComparer
		class GalleryItemComparer : IComparer<GalleryItem> {
			#region IComparer<GalleryItem> Members
			public int Compare(GalleryItem x, GalleryItem y) {
				if (IsNormalText(x.Caption)) {
					if (IsNormalText(y.Caption))
						return 0;
					return -1;
				}
				return x.Caption.CompareTo(y.Caption);
			}
			#endregion
			bool IsNormalText(string caption) {
				string text = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.StyleName_Normal);
				if (String.Compare(caption, text, StringComparison.CurrentCultureIgnoreCase) == 0)
					return true;
				return false;
			}
		}
		#endregion
		public GalleryChangeStyleItem() {
		}
		protected override bool DropDownGalleryShowGroupCaption { get { return true; } }
		protected override void Initialize() {
			base.Initialize();
			Gallery.MinimumColumnCount = minColumnCount;
			Gallery.ColumnCount = maxColumnCount;
			Gallery.RowCount = rowCount;
			Gallery.ItemSize = itemSize;
			Gallery.ItemAutoSizeMode = itemAutoSizeMode;
			Gallery.ShowItemText = true;
			Gallery.DrawImageBackground = false;
			this.repository = new RepositoryItemSpreadsheetCellStyleEdit();
		}
		protected override void SubscribeEvents() {
			base.SubscribeEvents();
			GalleryCustomDrawItemText += OnCustomDrawItemText;
			repository.Items.CollectionChanged += OnRepositoryItemsChanged;
		}
		void OnRepositoryItemsChanged(object sender, CollectionChangeEventArgs e) {
			if (DesignMode)
				return;
			Gallery.BeginUpdate();
			try {
				PopulateGalleryItems();
			}
			finally {
				Gallery.EndUpdate();
			}
		}
		protected internal virtual void PopulateGalleryItems() {
			List<GalleryItem> customItems = new List<GalleryItem>();
			List<GalleryItem> goodBadAndNeutraltems = new List<GalleryItem>();
			List<GalleryItem> dataAndModeltems = new List<GalleryItem>();
			List<GalleryItem> titlesAndHeadingsItems = new List<GalleryItem>();
			List<GalleryItem> themedCellStylesItems = new List<GalleryItem>();
			List<GalleryItem> numberFormatItems = new List<GalleryItem>();
			int count = repository.Items.Count;
			for (int i = 0; i < count; i++) {
				CellStyleBase currenItem = repository.Items[i] as CellStyleBase;
				string caption = currenItem.Name;
				GalleryItem galleryItem = new GalleryItem();
				galleryItem.Caption = caption;
				galleryItem.Hint = caption;
				galleryItem.Tag = currenItem;
				StyleCategory category = BuiltInCellStyleCalculator.GetStyleCategory(currenItem);
				if (category == StyleCategory.CustomStyle)
					customItems.Add(galleryItem);
				else if (category == StyleCategory.GoodBadNeutralStyle)
					goodBadAndNeutraltems.Add(galleryItem);
				else if (category == StyleCategory.DataModelStyle)
					dataAndModeltems.Add(galleryItem);
				else if (category == StyleCategory.TitleAndHeadingStyle)
					titlesAndHeadingsItems.Add(galleryItem);
				else if (category == StyleCategory.ThemedCellStyle)
					themedCellStylesItems.Add(galleryItem);
				else if (category == StyleCategory.NumberFormatStyle)
					numberFormatItems.Add(galleryItem);
			}
			GalleryItemComparer galleryItemComparer = new GalleryItemComparer();
			customItems.Sort(galleryItemComparer);
			goodBadAndNeutraltems.Sort(galleryItemComparer);
			dataAndModeltems.Sort(galleryItemComparer);
			titlesAndHeadingsItems.Sort(galleryItemComparer);
			themedCellStylesItems.Sort(galleryItemComparer);
			numberFormatItems.Sort(galleryItemComparer);
			GalleryItemGroup commonGroup = new GalleryItemGroup();
			AddItemsInGroup(customItems, commonGroup);
			AddItemsInGroup(goodBadAndNeutraltems, commonGroup);
			AddItemsInGroup(dataAndModeltems, commonGroup);
			AddItemsInGroup(titlesAndHeadingsItems, commonGroup);
			AddItemsInGroup(themedCellStylesItems, commonGroup);
			AddItemsInGroup(numberFormatItems, commonGroup);
			Gallery.Groups.Clear();
			Gallery.Groups.Add(commonGroup);
		}
		void AddItemsInGroup(List<GalleryItem> items, GalleryItemGroup group) {
			foreach (GalleryItem item in items)
				group.Items.Add(item);
		}
		protected override void InitDropDownGallery(object sender, InplaceGalleryEventArgs e) {
			base.InitDropDownGallery(sender, e);
			InDropDownGallery popupGallery = e.PopupGallery;
			popupGallery.ColumnCount = maxDropDownColumnCount;
			GalleryItemGroup groupCustom = CreateGalleryItemGroup(XtraSpreadsheetStringId.Caption_StyleGalleryGroupCustom);
			GalleryItemGroup groupGoodBadAndNeutral = CreateGalleryItemGroup(XtraSpreadsheetStringId.Caption_StyleGalleryGroupGoogBadAndNeutral);
			GalleryItemGroup groupDataAndModel = CreateGalleryItemGroup(XtraSpreadsheetStringId.Caption_StyleGalleryGroupDataAndModel);
			GalleryItemGroup groupTitlesAndHeadings = CreateGalleryItemGroup(XtraSpreadsheetStringId.Caption_StyleGalleryGroupTitlesAndHeadings);
			GalleryItemGroup groupThemedCellStyles = CreateGalleryItemGroup(XtraSpreadsheetStringId.Caption_StyleGalleryGroupThemedCellStyles);
			GalleryItemGroup groupNumberFormat = CreateGalleryItemGroup(XtraSpreadsheetStringId.Caption_StyleGalleryGroupNumberFormat);
			System.Diagnostics.Debug.Assert(popupGallery.Groups.Count == 1);
			GalleryItemCollection galleryItems = popupGallery.Groups[0].Items;
			int count = galleryItems.Count;
			for (int i = 0; i < count; i++) {
				GalleryItem currentItem = galleryItems[i];
				StyleCategory category = BuiltInCellStyleCalculator.GetStyleCategory(currentItem.Tag as CellStyleBase);
				if (category == StyleCategory.CustomStyle)
					groupCustom.Items.Add(currentItem);
				else if (category == StyleCategory.GoodBadNeutralStyle)
					groupGoodBadAndNeutral.Items.Add(currentItem);
				else if (category == StyleCategory.DataModelStyle)
					groupDataAndModel.Items.Add(currentItem);
				else if (category == StyleCategory.TitleAndHeadingStyle)
					groupTitlesAndHeadings.Items.Add(currentItem);
				else if (category == StyleCategory.ThemedCellStyle)
					groupThemedCellStyles.Items.Add(currentItem);
				else if (category == StyleCategory.NumberFormatStyle)
					groupNumberFormat.Items.Add(currentItem);
			}
			popupGallery.Groups.Clear();
			AddGalleryItemGroup(groupCustom, popupGallery);
			AddGalleryItemGroup(groupGoodBadAndNeutral, popupGallery);
			AddGalleryItemGroup(groupDataAndModel, popupGallery);
			AddGalleryItemGroup(groupTitlesAndHeadings, popupGallery);
			AddGalleryItemGroup(groupThemedCellStyles, popupGallery);
			AddGalleryItemGroup(groupNumberFormat, popupGallery);
		}
		GalleryItemGroup CreateGalleryItemGroup(XtraSpreadsheetStringId stringId) {
			GalleryItemGroup result = new GalleryItemGroup();
			result.Caption = XtraSpreadsheetLocalizer.GetString(stringId);
			return result;
		}
		void AddGalleryItemGroup(GalleryItemGroup group, InDropDownGallery gallery) {
			if (group.Items.Count > 0)
				gallery.Groups.Add(group);
		}
		void OnCustomDrawItemText(object sender, GalleryItemCustomDrawEventArgs e) {
			if (Control == null || Control.IsDisposed)
				return;
			CellStyleBase cellStyle = e.Item.Tag as CellStyleBase;
			if (cellStyle == null)
				return;
			GalleryItemViewInfo viewInfo = e.ItemInfo as GalleryItemViewInfo;
			if (viewInfo == null)
				return;
			Rectangle bounds = Rectangle.Inflate(viewInfo.Bounds, -selectionWidth, -selectionWidth);
			GraphicsCache cache = e.Cache;
			DrawStyleBackground(cellStyle, cache, bounds);
			DrawStyle(cellStyle, cache, bounds);
			e.Handled = true;
		}
		private static void DrawStyleBackground(CellStyleBase cellStyle, GraphicsCache cache, Rectangle rect) {
			Color backgroundColor = cellStyle.ApplyFill ? Cell.GetBackgroundColor(cellStyle.ActualFill) : DXColor.Empty;
			if (!DXColor.IsTransparentOrEmpty(backgroundColor))
				cache.Graphics.FillRectangle(cache.GetSolidBrush(backgroundColor), rect);
		}
		protected internal virtual void DrawStyle(CellStyleBase cellStyle, GraphicsCache cache, Rectangle rect) {
			Font font = GetFont(cellStyle);
			Rectangle textBounds = CalculateTextBounds(rect, (int)font.GetHeight());
			Color foreColor = cellStyle.ApplyFont ? cellStyle.Font.Color : DXColor.Black;
			TextUtils.DrawString(cache.Graphics, cellStyle.Name, font, foreColor, textBounds, rect, styleNameFormat);
		}
		protected internal virtual Brush CalculateBrush(GraphicsCache cache, CellStyleBase cellStyle) {
			Color foreColor = cellStyle.ApplyFont ? cellStyle.Font.Color : DXColor.Black;
			if (DXColor.IsTransparentOrEmpty(foreColor))
				foreColor = Color.Black;
			return cache.GetSolidBrush(foreColor);
		}
		protected internal Font GetFont(CellStyleBase cellStyle) {
			Font originalFont = cellStyle.ActualFont.GetFontInfo().Font;
			if (originalFont.Size <= maxFontSize)
				return originalFont;
			return new Font(originalFont.Name, maxFontSize, originalFont.Style, originalFont.Unit, originalFont.GdiCharSet, originalFont.GdiVerticalFont);
		}
		protected internal Rectangle CalculateTextBounds(Rectangle rect, int fontHeight) {
			Rectangle textBounds = Rectangle.Inflate(rect, -4, 0);
			int textBoundsHeight = textBounds.Height;
			if (fontHeight > textBoundsHeight) {
				int deltaHeight = fontHeight - textBoundsHeight;
				textBounds.Y -= deltaHeight;
				textBounds.Height += deltaHeight;
			}
			return textBounds;
		}
		protected override SpreadsheetCommandId CommandId { get { return SpreadsheetCommandId.FormatCellStyle; } }
		protected override void InvokeCommand() {
			Command command = CreateCommand();
			if (command != null) {
				ICommandUIState state = CreateCommandUIState(command);
				if (command.CanExecute())
					command.ForceExecute(state);
			}
		}
		protected internal virtual ICommandUIState CreateCommandUIState(Command command) {
			DefaultValueBasedCommandUIState<CellStyleBase> state = new DefaultValueBasedCommandUIState<CellStyleBase>();
			state.Value = SelectedItem.Tag as CellStyleBase;
			return state;
		}
		protected override void OnControlChanged() {
			repository.Control = Control;
		}
		protected override ICommandUIState CreateGalleryItemUIState() {
			return new BarGalleryItemValueUIState<CellStyleBase>(this);
		}
		protected override void OnEnabledChanged() {
			if (!Enabled) {
				GalleryItemProcessorDelegate itemsUpdateCaption = delegate(GalleryItem item) {
					item.Checked = false;
				};
				ForEachGalleryItems(itemsUpdateCaption);
			}
			base.OnEnabledChanged();
		}
	}
	#endregion
	#region ChangeStyleItem
	public class ChangeStyleItem : SpreadsheetCommandBarEditItem<CellStyleBase> {
		const int defaultWidth = 130;
		public ChangeStyleItem() {
			Width = defaultWidth;
		}
		public ChangeStyleItem(BarManager manager, string caption)
			: base(manager, caption) {
			Width = defaultWidth;
		}
		[DefaultValue(defaultWidth)]
		public override int Width { get { return base.Width; } set { base.Width = value; } }
		protected override SpreadsheetCommandId CommandId { get { return SpreadsheetCommandId.FormatCellStyle; } }
		protected override ICommandUIState CreateCommandUIState(Command command) {
			DefaultValueBasedCommandUIState<CellStyleBase> state = new DefaultValueBasedCommandUIState<CellStyleBase>();
			state.Value = EditValue as CellStyleBase;
			return state;
		}
		protected override RepositoryItem CreateEdit() {
			RepositoryItemSpreadsheetCellStyleEdit edit = new RepositoryItemSpreadsheetCellStyleEdit();
			if (Control != null)
				edit.Control = Control;
			return edit;
		}
		protected override void OnControlChanged() {
			RepositoryItemSpreadsheetCellStyleEdit edit = (RepositoryItemSpreadsheetCellStyleEdit)Edit;
			if (edit != null)
				edit.Control = Control;
		}
		protected override void OnEditValidating(object sender, CancelEventArgs e) {
			ComboBoxEdit edit = (ComboBoxEdit)sender;
			CellStyleBase cellStyle = edit.EditValue as CellStyleBase;
			if (cellStyle == null) {
				edit.ErrorText = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_InvalidStyleName);
				e.Cancel = true;
			}
		}
	}
	#endregion
	#region ChangeSheetTabColorItem
	public class ChangeSheetTabColorItem : ChangeColorItemBase<SpreadsheetControl, SpreadsheetCommandId> {
		public ChangeSheetTabColorItem() {
		}
		public ChangeSheetTabColorItem(BarManager manager)
			: base(manager) {
		}
		public ChangeSheetTabColorItem(string caption)
			: base(caption) {
		}
		public ChangeSheetTabColorItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		#region Properties
		protected override SpreadsheetCommandId CommandId { get { return SpreadsheetCommandId.None; } }
		protected override string DefaultColorButtonCaption { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_NoColor); } }
		public override bool ActAsDropDown { get { return true; } set { } }
		#endregion
		protected override Command CreateCommand() {
			if (Control == null)
				return null;
			Command command = new FormatTabColorCommand(Control);
			command.CommandSourceType = CommandSourceType.Menu;
			return command;
		}
	}
	#endregion
	#region HomeButtonGroups
	public static class HomeButtonGroups {
		public static string FontNameAndSizeButtonGroup = "{B0CA3FA8-82D6-4BC4-BD31-D9AE56C1D033}";
		public static string FontStyleButtonGroup = "{56C139FB-52E5-405B-A03F-FA7DCABD1D17}";
		public static string FontBordersButtonGroup = "{DDB05A32-9207-4556-85CB-FE3403A197C7}";
		public static string FontColorButtonGroup = "{C2275623-04A3-41E8-8D6A-EB5C7F8541D1}";
		public static string FontHorizontalAlignmentButtonGroup = "{ECC693B7-EF59-4007-A0DB-A9550214A0F2}";
		public static string FontVerticalAlignmentButtonGroup = "{03A0322B-12A2-4434-A487-8B5AAF64CCFC}";
		public static string FontIndentButtonGroup = "{A5E37DED-106E-44FC-8044-CE3824C08225}";
		public static string ClipboardButtonGroup = "{38BE839D-C66F-4BD9-8687-4C0030D394EE}";
		public static string NumberFormatMainButtonGroup = "{0B3A7A43-3079-4ce0-83A8-3789F5F6DC9F}";
		public static string NumberFormatsButtonGroup = "{508C2CE6-E1C8-4DD1-BA50-6C210FDB31B0}";
		public static string ChangeNumberFormatsButtonGroup = "{BBAB348B-BDB2-487A-A883-EFB9982DC698}";
	}
	#endregion
}
