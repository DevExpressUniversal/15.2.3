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
using System.Windows.Forms;
using DevExpress.Web.ASPxHtmlEditor.Design;
using DevExpress.Web.ASPxSpreadsheet.Internal;
using DevExpress.Web.Design;
namespace DevExpress.Web.ASPxSpreadsheet.Design {
	public class FileCommonRibbonGroup : GroupBase { };
	public class UndoRibbonGroup : GroupBase { };
	public class ClipboardRibbonGroup : GroupBase { };
	public class FontRibbonGroup : GroupBase { };
	public class AlignmentRibbonGroup : GroupBase { };
	public class NumberRibbonGroup : GroupBase { };
	public class CellsRibbonGroup : GroupBase { };
	public class EditingRibbonGroup : GroupBase { };
	public class StylesRibbonGroup : GroupBase { };
	public class TablesRibbonGroup : GroupBase { };
	public class IllustrationsRibbonGroup : GroupBase { };
	public class ChartsRibbonGroup : GroupBase { };
	public class LinksRibbonGroup : GroupBase { };
	public class PageSetupRibbonGroup : GroupBase { };
	public class PrintRibbonGroup : GroupBase { };
	public class FunctionLibraryRibbonGroup : GroupBase { };
	public class CalculationRibbonGroup : GroupBase { };
	public class DataCommonRibbonGroup : GroupBase { };
	public class DataToolsRibbonGroup : GroupBase { };
	public class CustomItemsRibbonGroup : GroupBase { };
	public class ViewRibbonGroup : GroupBase { };
	public class ShowRibbonGroup : GroupBase { };
	public class WindowRibbonGroup : GroupBase { };
	public class TablePropertiesGroup : GroupBase { };
	public class TableToolsGroup : GroupBase { };
	public class TableStyleOptionsGroup : GroupBase { };
	public class TableStyleGroup : GroupBase { };
	public class PictureArrangeGroup : GroupBase { };
	public class ChartTypeGroup : GroupBase { };
	public class ChartDataGroup : GroupBase { };
	public class ChartLabelsGroup : GroupBase { };
	public class ChartAxesGroup : GroupBase { };
	public class ChartArrangeGroup : GroupBase { };
	public class SpreadsheetRibbonItemsOwner : RibbonGroupItemsOwner {
		public SpreadsheetRibbonItemsOwner(object component, IServiceProvider provider, Collection items)
			: base(component, provider, items) {
		}
		public SpreadsheetRibbonItemsOwner(Collection items)
			: base(null, null, items) {
		}
		public ASPxSpreadsheet Spreadsheet { get { return Component as ASPxSpreadsheet; } }
		public SpreadsheetDefaultRibbon RibbonHelper { get { return new SpreadsheetDefaultRibbon(Spreadsheet); } }
		protected override void FillItemTypes() {
			#region RibbonTabs
			AddItemType(typeof(SRFileTab), "File", string.Empty, TabControlItemImageResource);
			AddItemType(typeof(SRHomeTab), "Home", string.Empty, TabControlItemImageResource);
			AddItemType(typeof(SRInsertTab), "Insert", string.Empty, TabControlItemImageResource);
			AddItemType(typeof(SRPageLayoutTab), "Page Layout", string.Empty, TabControlItemImageResource);
			AddItemType(typeof(SRFormulasTab), "Formulas", string.Empty, TabControlItemImageResource);
			AddItemType(typeof(SRDataTab), "Data", string.Empty, TabControlItemImageResource);
			AddItemType(typeof(SRViewTab), "View", string.Empty, TabControlItemImageResource);
			AddItemType(typeof(RibbonTab), "Custom Tab", string.Empty, TabControlItemImageResource);
			#endregion
			#region RibbonGroups
			AddItemType(typeof(SRFileCommonGroup), "File Common Group", RibbonGroupItemImageResource);			
			AddItemType(typeof(SRUndoGroup), "Undo Group", RibbonGroupItemImageResource);
			AddItemType(typeof(SRClipboardGroup), "Clipboard Group", RibbonGroupItemImageResource);
			AddItemType(typeof(SRFontGroup), "Font Group", RibbonGroupItemImageResource);
			AddItemType(typeof(SRAlignmentGroup), "Alignment Group", RibbonGroupItemImageResource);
			AddItemType(typeof(SRNumberGroup), "Number Group", RibbonGroupItemImageResource);
			AddItemType(typeof(SRCellsGroup), "Cells Group", RibbonGroupItemImageResource);
			AddItemType(typeof(SREditingGroup), "Editing Group", RibbonGroupItemImageResource);
			AddItemType(typeof(SRStylesGroup), "Styles", RibbonGroupItemImageResource);
			AddItemType(typeof(SRTablesGroup), "Tables", RibbonGroupItemImageResource);
			AddItemType(typeof(SRIllustrationsGroup), "Illustrations Group", RibbonGroupItemImageResource);
			AddItemType(typeof(SRChartsGroup), "Charts Group", RibbonGroupItemImageResource);
			AddItemType(typeof(SRLinksGroup), "Links Group", RibbonGroupItemImageResource);
			AddItemType(typeof(SRPageSetupGroup), "Page Setup Group", RibbonGroupItemImageResource);			
			AddItemType(typeof(SRPrintGroup), "Print Group", RibbonGroupItemImageResource);
			AddItemType(typeof(SRFunctionLibraryGroup), "Function Library Group", RibbonGroupItemImageResource);
			AddItemType(typeof(SRCalculationGroup), "Calculation Group", RibbonGroupItemImageResource);
			AddItemType(typeof(SRDataSortAndFilterGroup), "Sort & Filter Group", RibbonGroupItemImageResource);
			AddItemType(typeof(SRDataToolsGroup), "Data Tools", RibbonGroupItemImageResource);
			AddItemType(typeof(SRShowGroup), "Show Group", RibbonGroupItemImageResource);
			AddItemType(typeof(SRViewGroup), "View Group", RibbonGroupItemImageResource);
			AddItemType(typeof(SRWindowGroup), "Window Group", RibbonGroupItemImageResource);
			AddItemType(typeof(RibbonGroup), "Custom Group", RibbonGroupItemImageResource);
			#endregion
			#region RibbonItems
			AddItemType(typeof(FileCommonRibbonGroup), "File Common Group");
			AddGroupItemType(typeof(FileCommonRibbonGroup), typeof(SRFileNewCommand), "New", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(FileCommonRibbonGroup), typeof(SRFileOpenCommand), "Open", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(FileCommonRibbonGroup), typeof(SRFileSaveCommand), "Save", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(FileCommonRibbonGroup), typeof(SRFileSaveAsCommand), "Save As", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(FileCommonRibbonGroup), typeof(SRFilePrintCommand), "Print", RibbonButtonItemImageResource);
			AddItemType(typeof(UndoRibbonGroup), "Undo Group");
			AddGroupItemType(typeof(UndoRibbonGroup), typeof(SRFileUndoCommand), "Undo", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(UndoRibbonGroup), typeof(SRFileRedoCommand), "Redo", RibbonButtonItemImageResource);
			AddItemType(typeof(ClipboardRibbonGroup), "Clipboard");
			AddGroupItemType(typeof(ClipboardRibbonGroup), typeof(SRPasteSelectionCommand), "Paste", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(ClipboardRibbonGroup), typeof(SRCutSelectionCommand), "Cut", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(ClipboardRibbonGroup), typeof(SRCopySelectionCommand), "Copy", RibbonButtonItemImageResource);
			AddItemType(typeof(FontRibbonGroup), "Font");
			AddGroupItemType(typeof(FontRibbonGroup), typeof(SRFormatFontNameCommand), "Font", ComboboxImageResource);
			AddGroupItemType(typeof(FontRibbonGroup), typeof(SRFormatFontSizeCommand), "Font Size", ComboboxImageResource);
			AddGroupItemType(typeof(FontRibbonGroup), typeof(SRFormatIncreaseFontSizeCommand), "Grow Font", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(FontRibbonGroup), typeof(SRFormatDecreaseFontSizeCommand), "Shrink Font", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(FontRibbonGroup), typeof(SRFormatFontBoldCommand), "Bold", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(FontRibbonGroup), typeof(SRFormatFontItalicCommand), "Italic", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(FontRibbonGroup), typeof(SRFormatFontUnderlineCommand), "Underline", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(FontRibbonGroup), typeof(SRFormatFontStrikeoutCommand), "Strike", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(FontRibbonGroup), typeof(SRFormatBordersCommand), "Borders", DropDownButtonImageResource);
			AddGroupItemType(typeof(FontRibbonGroup), typeof(SRFormatFillColorCommand), "Fill Color", ColorEditImageResource);
			AddGroupItemType(typeof(FontRibbonGroup), typeof(SRFormatFontColorCommand), "Font Color", ColorEditImageResource);
			AddGroupItemType(typeof(FontRibbonGroup), typeof(SRFormatBorderLineColorCommand), "Border Line Color", ColorEditImageResource);
			AddItemType(typeof(AlignmentRibbonGroup), "Alignment");
			AddGroupItemType(typeof(AlignmentRibbonGroup), typeof(SRFormatAlignmentTopCommand), "Top Align", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(AlignmentRibbonGroup), typeof(SRFormatAlignmentMiddleCommand), "Middle Align", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(AlignmentRibbonGroup), typeof(SRFormatAlignmentBottomCommand), "Bottom Align", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(AlignmentRibbonGroup), typeof(SRFormatAlignmentLeftCommand), "Align Text Left", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(AlignmentRibbonGroup), typeof(SRFormatAlignmentCenterCommand), "Center", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(AlignmentRibbonGroup), typeof(SRFormatAlignmentRightCommand), "Align Text Right", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(AlignmentRibbonGroup), typeof(SRFormatDecreaseIndentCommand), "Decrease Indent", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(AlignmentRibbonGroup), typeof(SRFormatIncreaseIndentCommand), "IncreaseIndent", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(AlignmentRibbonGroup), typeof(SRFormatWrapTextCommand), "Wrap Text", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(AlignmentRibbonGroup), typeof(SREditingMergeCellsGroupCommand), "Merge Cells", DropDownButtonImageResource);
			AddItemType(typeof(NumberRibbonGroup), "Number");
			AddGroupItemType(typeof(NumberRibbonGroup), typeof(SRFormatNumberAccountingCommand), "Accounting Number Format", DropDownButtonImageResource);
			AddGroupItemType(typeof(NumberRibbonGroup), typeof(SRFormatNumberPercentCommand), "Percent Style", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(NumberRibbonGroup), typeof(SRFormatNumberCommaStyleCommand), "Comma Style", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(NumberRibbonGroup), typeof(SRFormatNumberIncreaseDecimalCommand), "Increase Decimal", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(NumberRibbonGroup), typeof(SRFormatNumberDecreaseDecimalCommand), "Decrease Decimal", RibbonButtonItemImageResource);
			AddItemType(typeof(CellsRibbonGroup), "Cells");
			AddGroupItemType(typeof(CellsRibbonGroup), typeof(SRFormatInsertCommand), "Insert", DropDownButtonImageResource);
			AddGroupItemType(typeof(CellsRibbonGroup), typeof(SRFormatRemoveCommand), "Delete", DropDownButtonImageResource);
			AddGroupItemType(typeof(CellsRibbonGroup), typeof(SRFormatFormatCommand), "Format", DropDownButtonImageResource);
			AddItemType(typeof(EditingRibbonGroup), "Editing");
			AddGroupItemType(typeof(EditingRibbonGroup), typeof(SRFormatAutoSumCommand), "AutoSum", DropDownButtonImageResource);
			AddGroupItemType(typeof(EditingRibbonGroup), typeof(SRFormatFillCommand), "Fill", DropDownButtonImageResource);
			AddGroupItemType(typeof(EditingRibbonGroup), typeof(SRFormatClearCommand), "Clear", DropDownButtonImageResource);
			AddGroupItemType(typeof(EditingRibbonGroup), typeof(SREditingSortAndFilterCommand), "Sort and Filter", DropDownButtonImageResource);
			AddGroupItemType(typeof(EditingRibbonGroup), typeof(SREditingFindAndSelectCommand), "Find and Replace", RibbonButtonItemImageResource);
			AddItemType(typeof(StylesRibbonGroup), "Styles");
			AddGroupItemType(typeof(StylesRibbonGroup), typeof(SRFormatAsTableCommand), "Format As Table", RibbonButtonItemImageResource);
			AddItemType(typeof(TablesRibbonGroup), "Tables");
			AddGroupItemType(typeof(TablesRibbonGroup), typeof(SRInsertTableCommand), "Table", RibbonButtonItemImageResource);
			AddItemType(typeof(IllustrationsRibbonGroup), "Illustrations");
			AddGroupItemType(typeof(IllustrationsRibbonGroup), typeof(SRFormatInsertPictureCommand), "Picture", RibbonButtonItemImageResource);
			AddItemType(typeof(ChartsRibbonGroup), "Charts");
			AddGroupItemType(typeof(ChartsRibbonGroup), typeof(SRInsertChartColumnCommand), "Column", DropDownButtonImageResource);
			AddGroupItemType(typeof(ChartsRibbonGroup), typeof(SRInsertChartLinesCommand), "Line", DropDownButtonImageResource);
			AddGroupItemType(typeof(ChartsRibbonGroup), typeof(SRInsertChartPiesCommand), "Pie", DropDownButtonImageResource);
			AddGroupItemType(typeof(ChartsRibbonGroup), typeof(SRInsertChartBarsCommand), "Bar", DropDownButtonImageResource);
			AddGroupItemType(typeof(ChartsRibbonGroup), typeof(SRInsertChartAreasCommand), "Area", DropDownButtonImageResource);
			AddGroupItemType(typeof(ChartsRibbonGroup), typeof(SRInsertChartScattersCommand), "Scatter", DropDownButtonImageResource);
			AddGroupItemType(typeof(ChartsRibbonGroup), typeof(SRInsertChartOthersCommand), "Other Charts", DropDownButtonImageResource);
			AddItemType(typeof(LinksRibbonGroup), "Links");
			AddGroupItemType(typeof(LinksRibbonGroup), typeof(SRFormatInsertHyperlinkCommand), "Hyperlink", RibbonButtonItemImageResource);
			AddItemType(typeof(PageSetupRibbonGroup), "Page Setup");
			AddGroupItemType(typeof(PageSetupRibbonGroup), typeof(SRPageSetupMarginsCommand), "Margins", DropDownButtonImageResource);
			AddGroupItemType(typeof(PageSetupRibbonGroup), typeof(SRPageSetupOrientationCommand), "Orientation", DropDownButtonImageResource);
			AddGroupItemType(typeof(PageSetupRibbonGroup), typeof(SRPageSetupPaperKindCommand), "Size", DropDownButtonImageResource);
			AddItemType(typeof(PrintRibbonGroup), "Print");
			AddGroupItemType(typeof(PrintRibbonGroup), typeof(SRPrintGridlinesCommand), "Gridlines", CheckColumnImageResource);
			AddGroupItemType(typeof(PrintRibbonGroup), typeof(SRPrintHeadingsCommand), "Headings", CheckColumnImageResource);
			AddItemType(typeof(FunctionLibraryRibbonGroup), "Function Library");
			AddGroupItemType(typeof(FunctionLibraryRibbonGroup), typeof(SRFunctionsAutoSumCommand), "AutoSum", DropDownButtonImageResource);
			AddGroupItemType(typeof(FunctionLibraryRibbonGroup), typeof(SRFunctionsFinancialCommand), "Financial", DropDownButtonImageResource);
			AddGroupItemType(typeof(FunctionLibraryRibbonGroup), typeof(SRFunctionsLogicalCommand), "Logical", DropDownButtonImageResource);
			AddGroupItemType(typeof(FunctionLibraryRibbonGroup), typeof(SRFunctionsTextCommand), "Text", DropDownButtonImageResource);
			AddGroupItemType(typeof(FunctionLibraryRibbonGroup), typeof(SRFunctionsDateAndTimeCommand), "Data & Time", DropDownButtonImageResource);
			AddGroupItemType(typeof(FunctionLibraryRibbonGroup), typeof(SRFunctionsLookupAndReferenceCommand), "Lookup & Reference", DropDownButtonImageResource);
			AddGroupItemType(typeof(FunctionLibraryRibbonGroup), typeof(SRFunctionsMathAndTrigonometryCommand), "Math & Trig", DropDownButtonImageResource);
			AddGroupItemType(typeof(FunctionLibraryRibbonGroup), typeof(SRFunctionsMoreCommand), "More", DropDownButtonImageResource);
			AddItemType(typeof(CalculationRibbonGroup), "Calculation Library");
			AddGroupItemType(typeof(CalculationRibbonGroup), typeof(SRFunctionsCalculationOptionCommand), "Calculation Options", DropDownButtonImageResource);
			AddGroupItemType(typeof(CalculationRibbonGroup), typeof(SRFunctionsCalculateNowCommand), "Calculation Now", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(CalculationRibbonGroup), typeof(SRFunctionsCalculateSheetCommand), "Calculation Sheet", RibbonButtonItemImageResource);
			AddItemType(typeof(DataCommonRibbonGroup), "Common");
			AddGroupItemType(typeof(DataCommonRibbonGroup), typeof(SRDataSortAscendingCommand), "Sort A to Z", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(DataCommonRibbonGroup), typeof(SRDataSortDescendingCommand), "Sort Z to A", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(DataCommonRibbonGroup), typeof(SRDataFilterToggleCommand), "Filter", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(DataCommonRibbonGroup), typeof(SRDataFilterClearCommand), "Clear", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(DataCommonRibbonGroup), typeof(SRDataFilterReApplyCommand), "Reapply", RibbonButtonItemImageResource);
			AddItemType(typeof(DataToolsRibbonGroup), "Data Tools");
			AddGroupItemType(typeof(DataToolsRibbonGroup), typeof(SRDataToolsDataValidationGroupCommand), "Data Validation", RibbonButtonItemImageResource);
			AddItemType(typeof(ViewRibbonGroup), "View Group");
			AddGroupItemType(typeof(ViewRibbonGroup), typeof(SRFullScreenCommand), "View", RibbonToggleButtonItemImageResource);
			AddItemType(typeof(ShowRibbonGroup), "Show");
			AddGroupItemType(typeof(ShowRibbonGroup), typeof(SRViewShowGridlinesCommand), "Gridlines", CheckColumnImageResource);
			AddItemType(typeof(WindowRibbonGroup), "Window");
			AddGroupItemType(typeof(WindowRibbonGroup), typeof(SRViewFreezePanesGroupCommand), "Freeze Panes", DropDownButtonImageResource);
			AddItemType(typeof(CustomItemsRibbonGroup), "Custom Items");
			AddGroupItemType(typeof(CustomItemsRibbonGroup), typeof(RibbonButtonItem), "Button", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(CustomItemsRibbonGroup), typeof(RibbonCheckBoxItem), "Check Box", "Check Box", CheckColumnImageResource);
			AddGroupItemType(typeof(CustomItemsRibbonGroup), typeof(RibbonColorButtonItem), "Color Button", ColorEditImageResource);
			AddGroupItemType(typeof(CustomItemsRibbonGroup), typeof(RibbonDropDownButtonItem), "DropDown Button", DropDownButtonImageResource);
			AddGroupItemType(typeof(CustomItemsRibbonGroup), typeof(RibbonOptionButtonItem), "Option Button", RibbonOptionButtonItemImageResource);
			AddGroupItemType(typeof(CustomItemsRibbonGroup), typeof(RibbonToggleButtonItem), "Toggle Button", RibbonToggleButtonItemImageResource);
			AddGroupItemType(typeof(CustomItemsRibbonGroup), typeof(RibbonComboBoxItem), "Combo Box", "Combo Box", ComboboxImageResource);
			AddGroupItemType(typeof(CustomItemsRibbonGroup), typeof(RibbonDateEditItem), "Date Edit", "Date Edit", DateEditImageResource);
			AddGroupItemType(typeof(CustomItemsRibbonGroup), typeof(RibbonSpinEditItem), "Spin Edit", "Spin Edit", SpinEditImageResource);
			AddGroupItemType(typeof(CustomItemsRibbonGroup), typeof(RibbonTextBoxItem), "Text Box", "Text Box", TextImageResource);
			AddGroupItemType(typeof(CustomItemsRibbonGroup), typeof(RibbonTemplateItem), "Template", RibbonTemplateItemImageResource);
			AddGroupItemType(typeof(CustomItemsRibbonGroup), typeof(ListEditItem), "ListEdit Item", ListEditImageResource);
			#endregion
		}
		protected override bool IsBeginGroup(Type type) {
			return (type == typeof(CustomItemsRibbonGroup)) || (type == typeof(RibbonTab)) || (type == typeof(RibbonGroup));
		}
		protected override void AddItemCore(IDesignTimeColumnAndEditorItem designTimeItem, IDesignTimeCollectionItem target, InsertDirection direction) {
			if(designTimeItem != null) {
				IDesignTimeCollectionItem newItem = CreateDesignTimeCollectionItem(designTimeItem);
				if(newItem != null) {
					MoveItemTo(newItem, target, direction);
					SetFocusedItem(newItem, true);
					return;
				}
			}
			base.AddItemCore(designTimeItem, target, direction);
		}
		protected IDesignTimeCollectionItem CreateDesignTimeCollectionItem(IDesignTimeColumnAndEditorItem designTimeItem) {
			IDesignTimeCollectionItem newItem = null;
			if(typeof(SRTab).IsAssignableFrom(designTimeItem.ColumnType))
				newItem = CreateDefaultRibbonTab(designTimeItem.ColumnType);
			else if(typeof(SRGroup).IsAssignableFrom(designTimeItem.ColumnType))
				newItem = CreateDefaultRibbonGroup(designTimeItem.ColumnType, (GetFocusedRibbonTabItem() as RibbonTab));
			else {
				newItem = CreateNewItem(designTimeItem);
				FillDefaultItems(newItem);
			}
			return newItem;
		}
		IDesignTimeCollectionItem GetFocusedRibbonTabItem() {
			return typeof(RibbonTab).IsAssignableFrom(FocusedItemForAction.GetType()) ? FocusedItemForAction : FocusedItemForActionParent;
		}
		protected virtual RibbonTab CreateDefaultRibbonTab(Type type) {
			var tab = CreateDefaultRibbonTabCore(type);
			Items.Add(tab);
			return tab;
		}
		RibbonTab CreateDefaultRibbonTabCore(Type type) {
			if(type == typeof(SRFileTab))
				return RibbonHelper.CreateFileTab();
			if(type == typeof(SRHomeTab))
				return RibbonHelper.CreateHomeTab();
			if(type == typeof(SRInsertTab))
				return RibbonHelper.CreateInsertTab();
			if(type == typeof(SRPageLayoutTab))
				return RibbonHelper.CreatePageLayoutTab();
			if(type == typeof(SRFormulasTab))
				return RibbonHelper.CreateFormulasTab();
			if(type == typeof(SRDataTab))
				return RibbonHelper.CreateDataTab();
			if(type == typeof(SRViewTab))
				return RibbonHelper.CreateViewTab();
			return null;
		}
		RibbonGroup CreateDefaultRibbonGroup(Type type, RibbonTab ribbonTab) {
			var group = CreateDefaultRibbonGroupCore(type);
			ribbonTab.Groups.Add(group);
			return group;
		}
		protected virtual RibbonGroup CreateDefaultRibbonGroupCore(Type type) {
			if(type == typeof(SRFileCommonGroup))
				return RibbonHelper.CreateFileCommonGroup();
			if(type == typeof(SRUndoGroup))
				return RibbonHelper.CreateUndoGroup();
			if(type == typeof(SRClipboardGroup))
				return RibbonHelper.CreateClipboardGroup();
			if(type == typeof(SRFontGroup))
				return RibbonHelper.CreateFontGroup();
			if(type == typeof(SRAlignmentGroup))
				return RibbonHelper.CreateAlignmentGroup();
			if(type == typeof(SRNumberGroup))
				return RibbonHelper.CreateNumberGroup();
			if(type == typeof(SRCellsGroup))
				return RibbonHelper.CreateCellsGroup();
			if(type == typeof(SREditingGroup))
				return RibbonHelper.CreateEditingGroup();
			if(type == typeof(SRStylesGroup))
				return RibbonHelper.CreateStylesGroup();
			if(type == typeof(SRTablesGroup))
				return RibbonHelper.CreateTablesGroup();
			if(type == typeof(SRIllustrationsGroup))
				return RibbonHelper.CreateIllustrationsGroup();
			if(type == typeof(SRChartsGroup))
				return RibbonHelper.CreateChartsGroup();
			if(type == typeof(SRLinksGroup))
				return RibbonHelper.CreateLinksGroup();
			if(type == typeof(SRPageSetupGroup))
				return RibbonHelper.CreatePageSetupGroup();
			if(type == typeof(SRPrintGroup))
				return RibbonHelper.CreatePrintGroup();
			if(type == typeof(SRFunctionLibraryGroup))
				return RibbonHelper.CreateFunctionLibraryGroup();
			if(type == typeof(SRCalculationGroup))
				return RibbonHelper.CreateCalculationGroup();
			if(type == typeof(SRDataSortAndFilterGroup))
				return RibbonHelper.CreateDataSortAndFilterGroup();
			if(type == typeof(SRDataToolsGroup))
				return RibbonHelper.CreateDataToolsGroup();
			if(type == typeof(SRViewGroup))
				return RibbonHelper.CreateViewGroup();
			if(type == typeof(SRShowGroup))
				return RibbonHelper.CreateShowGroup();
			if (type == typeof(SRWindowGroup))
				return RibbonHelper.CreateWindowGroup();
			return null;
		}
		void FillDefaultItems(IDesignTimeCollectionItem item) {
			if(item == null)
				return;
			if(item.GetType() == typeof(SRFormatFontNameCommand) ||
				item.GetType() == typeof(SRFormatFontSizeCommand))
				(item as SRComboBoxCommandBase).FillItems();
		}
		protected override bool CanCreateSubmenuItem(DesignEditorDescriptorItem parentMenuItem, IDesignTimeColumnAndEditorItem designTimeItem, bool isToolbar) {
			if(parentMenuItem.ActionType == DesignEditorMenuRootItemActionType.CreateDefaultItems)
				return typeof(SRTab).IsAssignableFrom(designTimeItem.ColumnType);
			return base.CanCreateSubmenuItem(parentMenuItem, designTimeItem, isToolbar);
		}
		protected internal override string GetNavBarItemsGroupName() {
			return "Ribbon Items";
		}
	}
	public class SpreadsheetRibbonContextTabCategoriesOwner : SpreadsheetRibbonItemsOwner {
		public SpreadsheetRibbonContextTabCategoriesOwner(object component, IServiceProvider provider, Collection items)
			: base(component, provider, items) {
		}
		public SpreadsheetRibbonContextTabCategoriesOwner(Collection items)
			: base(null, null, items) {
		}
		protected override void FillItemTypes() {
			#region Tab categories
			AddItemType(typeof(SRTableToolsContextTabCategory), "Table Tools", TabControlItemImageResource);
			AddItemType(typeof(SRPictureToolsContextTabCategory), "Picture Tools", TabControlItemImageResource);
			AddItemType(typeof(SRChartToolsContextTabCategory), "Chart Tools", TabControlItemImageResource);
			#endregion
			#region Tabs
			AddItemType(typeof(SRTableDesignContextTab), "Table Design", TabControlItemImageResource);
			AddItemType(typeof(SRPictureFormatContextTab), "Picture Format", TabControlItemImageResource);
			AddItemType(typeof(SRChartDesignContextTab), "Chart Design", TabControlItemImageResource);
			AddItemType(typeof(SRChartLayoutContextTab), "Chart Layout", TabControlItemImageResource);
			AddItemType(typeof(SRChartFormatContextTab), "Chart Format", TabControlItemImageResource);
			#endregion
			#region Groups
			AddItemType(typeof(SRTablePropertiesGroup), "Table Properties Group", RibbonGroupItemImageResource);
			AddItemType(typeof(SRTableToolsGroup), "Table Tools Group", RibbonGroupItemImageResource);
			AddItemType(typeof(SRTableStyleOptionsGroup), "Table Style Options Group", RibbonGroupItemImageResource);
			AddItemType(typeof(SRTableStyleGroup), "Table Style Group", RibbonGroupItemImageResource);
			AddItemType(typeof(SRPictureArrangeGroup), "Picture Arrange", RibbonGroupItemImageResource);
			AddItemType(typeof(SRChartTypeGroup), "Chart Type", RibbonGroupItemImageResource);
			AddItemType(typeof(SRChartDataGroup), "Chart Data", RibbonGroupItemImageResource);
			AddItemType(typeof(SRChartLabelsGroup), "Chart Labels", RibbonGroupItemImageResource);
			AddItemType(typeof(SRChartAxesGroup), "Chart Axes", RibbonGroupItemImageResource);
			AddItemType(typeof(SRChartArrangeGroup), "Chart Arrange", RibbonGroupItemImageResource);
			#endregion
			#region Items
			AddItemType(typeof(TablePropertiesGroup), "Table Properties Group");
			AddGroupItemType(typeof(TablePropertiesGroup), typeof(SRRenameTableCommand), "Table Name", RibbonButtonItemImageResource);
			AddItemType(typeof(TableToolsGroup), "Table Tools Group");
			AddGroupItemType(typeof(TableToolsGroup), typeof(SRConvertToRangeCommand), "Convert to Range", RibbonButtonItemImageResource);
			AddItemType(typeof(TableStyleOptionsGroup), "Table Style Options Group");
			AddGroupItemType(typeof(TableStyleOptionsGroup), typeof(SRToggleHeaderRowCommand), "Header Row", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(TableStyleOptionsGroup), typeof(SRToggleTotalRowCommand), "Total Row", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(TableStyleOptionsGroup), typeof(SRToggleBandedColumnsCommand), "Banded Columns", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(TableStyleOptionsGroup), typeof(SRToggleFirstColumnCommand), "First Column", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(TableStyleOptionsGroup), typeof(SRToggleLastColumnCommand), "Last Column", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(TableStyleOptionsGroup), typeof(SRToggleBandedRowsCommand), "Banded Rows", RibbonButtonItemImageResource);
			AddItemType(typeof(TableStyleGroup), "Table Style Group");
			AddGroupItemType(typeof(TableStyleGroup), typeof(SRModifyTableStyleCommand), "Modify Style", RibbonButtonItemImageResource);
			AddItemType(typeof(PictureArrangeGroup), "Picture Arrange Group");
			AddGroupItemType(typeof(PictureArrangeGroup), typeof(SRArrangeBringForwardGroupCommand), "Bring Forward", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(PictureArrangeGroup), typeof(SRArrangeSendBackwardGroupCommand), "Send Backward", RibbonButtonItemImageResource);
			AddItemType(typeof(ChartTypeGroup), "Chart Type Group");
			AddGroupItemType(typeof(ChartTypeGroup), typeof(SRChangeChartTypeCommand), "Change Chart Type", RibbonButtonItemImageResource);
			AddItemType(typeof(ChartDataGroup), "Chart Data Group");
			AddGroupItemType(typeof(ChartDataGroup), typeof(SRChartSwitchRowColumnCommand), "Switch Row/Column", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(ChartDataGroup), typeof(SRChartSelectDataCommand), "Select Data", RibbonButtonItemImageResource);
			AddItemType(typeof(ChartLabelsGroup), "Chart Labels Group");
			AddGroupItemType(typeof(ChartLabelsGroup), typeof(SRChartTitleCommand), "Chart Title", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(ChartLabelsGroup), typeof(SRChartAxisTitlesCommand), "Axis Titles", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(ChartLabelsGroup), typeof(SRChartLegendCommand), "Legend", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(ChartLabelsGroup), typeof(SRChartDataLabelsCommand), "Data Labels", RibbonButtonItemImageResource);
			AddItemType(typeof(ChartAxesGroup), "Chart Axes Group");
			AddGroupItemType(typeof(ChartAxesGroup), typeof(SRChartAxesCommand), "Axes", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(ChartAxesGroup), typeof(SRChartGridlinesCommand), "Gridlines", RibbonButtonItemImageResource);
			AddItemType(typeof(ChartArrangeGroup), "Chart Arrange Group");
			AddGroupItemType(typeof(ChartArrangeGroup), typeof(SRArrangeBringForwardGroupCommand), "Bring Forward", RibbonButtonItemImageResource);
			AddGroupItemType(typeof(ChartArrangeGroup), typeof(SRArrangeSendBackwardGroupCommand), "Send Backward", RibbonButtonItemImageResource);
			#endregion
		}
		protected override bool IsBeginGroup(Type type) {
			return (type == typeof(RibbonContextTabCategory)) || base.IsBeginGroup(type);
		}
		protected IDesignTimeCollectionItem CreateDesignTimeCollectionItem(IDesignTimeColumnAndEditorItem designTimeItem) {
			IDesignTimeCollectionItem newItem = null;
			if(typeof(SRContextTabCategory) == designTimeItem.ColumnType)
				newItem = CreateDefaultRibbonTabCategory(designTimeItem.ColumnType);
			else 
				newItem = base.CreateDesignTimeCollectionItem(designTimeItem);
			return newItem;
		}
		IDesignTimeCollectionItem GetFocusedRibbonTabCategoryItem() {
			return typeof(RibbonContextTabCategory).IsAssignableFrom(FocusedItemForAction.GetType()) ? FocusedItemForAction : FocusedItemForActionParent;
		}
		RibbonContextTabCategory CreateDefaultRibbonTabCategory(Type type) {
			var tabCategory = CreateDefaultRibbonTabCategoryCore(type);
			Items.Add(tabCategory);
			return tabCategory;
		}
		RibbonContextTabCategory CreateDefaultRibbonTabCategoryCore(Type type) {
			if(type == typeof(SRTableToolsContextTabCategory))
				return RibbonHelper.CreateTableToolsContextTabCategory();
			if(type == typeof(SRPictureToolsContextTabCategory))
				return RibbonHelper.CreatePictureToolsContextTabCategory();
			if(type == typeof(SRChartToolsContextTabCategory))
				return RibbonHelper.CreateChartToolsContextTabCategory();
			return null;
		}
		protected override RibbonTab CreateDefaultRibbonTab(Type type) {
			var tabCategory = GetFocusedRibbonTabCategoryItem() as RibbonContextTabCategory;
			var tab = CreateDefaultRibbonTabCore(type);
			tabCategory.Tabs.Add(tab);
			return tab;
		}
		RibbonTab CreateDefaultRibbonTabCore(Type type) {
			if(type == typeof(SRTableDesignContextTab))
				return RibbonHelper.CreateTableDesignContextTab();
			if(type == typeof(SRPictureFormatContextTab))
				return RibbonHelper.CreatePictureFormatContextTab();
			if(type == typeof(SRChartDesignContextTab))
				return RibbonHelper.CreateChartDesignContextTab();
			if(type == typeof(SRChartLayoutContextTab))
				return RibbonHelper.CreateChartLayoutContextTab();
			if(type == typeof(SRChartFormatContextTab))
				return RibbonHelper.CreateChartFormatContextTab();
			return null;
		}
		protected override RibbonGroup CreateDefaultRibbonGroupCore(Type type) {
			if(type == typeof(SRTablePropertiesGroup))
				return RibbonHelper.CreateTablePropertiesGroup();
			if(type == typeof(SRTableToolsGroup))
				return RibbonHelper.CreateTableToolsGroup();
			if(type == typeof(SRTableStyleOptionsGroup))
				return RibbonHelper.CreateTableStyleOptionsGroup();
			if(type == typeof(SRTableStyleGroup))
				return RibbonHelper.CreateTableStyleGroup();
			if(type == typeof(SRPictureArrangeGroup))
				return RibbonHelper.CreatePictureArrangeGroup();
			if(type == typeof(SRChartTypeGroup))
				return RibbonHelper.CreateChartTypeGroup();
			if(type == typeof(SRChartDataGroup))
				return RibbonHelper.CreateChartDataGroup();
			if(type == typeof(SRChartLabelsGroup))
				return RibbonHelper.CreateChartLabelsGroup();
			if(type == typeof(SRChartAxesGroup))
				return RibbonHelper.CreateChartAxesGroup();
			if(type == typeof(SRChartArrangeGroup))
				return RibbonHelper.CreateChartArrangeGroup();
			return null;
		}
		protected internal override string GetNavBarItemsGroupName() {
			return "Ribbon Context Tabs";
		}
		protected override DesignEditorDescriptorItem CreateDefaultItemsItem() {
			var item = new DesignEditorDescriptorItem() { ActionType = DesignEditorMenuRootItemActionType.CreateDefaultItems };
			item.Caption = "Create Default Tab Categories";
			item.EditorType = DesignEditorDescriptorItemType.Button;
			item.Enabled = true;
			item.BeginGroup = true;
			PopulateChildItems(item);
			return item;
		}
		public void CreateDefaultItems() {
			BeginUpdate();
			(Items as SpreadsheetRibbonContextTabCategoryCollection).CreateDefaultRibbonContextTabCategories();
			SetFocusedItem(Items.OfType<IDesignTimeCollectionItem>().FirstOrDefault(), true);
			EndUpdate();
		}
		protected override bool CanCreateSubmenuItem(DesignEditorDescriptorItem parentMenuItem, IDesignTimeColumnAndEditorItem designTimeItem, bool isToolbar) {
			if(parentMenuItem.ActionType == DesignEditorMenuRootItemActionType.CreateDefaultItems)
				return typeof(RibbonContextTabCategory).IsAssignableFrom(designTimeItem.ColumnType);
			return base.CanCreateSubmenuItem(parentMenuItem, designTimeItem, isToolbar);
		}
		protected override bool IsContextTabCategoriesCollection() {
			return Items != null && Items.GetType() == typeof(SpreadsheetRibbonContextTabCategoryCollection);
		}
	}
	public partial class SpreadsheetRibbonItemsEditorForm : ItemsEditorFrame {
		protected SpreadsheetRibbonItemsOwner SpreadsheetRibbonOwner { get { return ItemsOwner as SpreadsheetRibbonItemsOwner; } }
		protected override void MenuItemClick_CreateDefaultItems(DesignEditorDescriptorItem menuItem) {
			if(SpreadsheetRibbonOwner.TreeListItems.Count > 0) {
				DialogResult dialogResult = ShowDeleteConfirmation();
				if(dialogResult == DialogResult.Cancel)
					return;
				else if(dialogResult == DialogResult.Yes) {
					SpreadsheetRibbonOwner.SetSelection(SpreadsheetRibbonOwner.TreeListItemsHash.Keys.ToList());
					SpreadsheetRibbonOwner.RemoveSelectedItems();
				}
			}
			CreateDefaultItems(menuItem);
		}
		protected virtual DialogResult ShowDeleteConfirmation() {
			return DesignUtils.ShowMessage(SpreadsheetRibbonOwner.Spreadsheet.Site, string.Format(string.Format("Do you want to delete the existing ribbon tabs?")),
					string.Format("Create default tabs for '{0}'", SpreadsheetRibbonOwner.Spreadsheet.GetType().Name), MessageBoxButtons.YesNoCancel);
		}
		protected virtual void CreateDefaultItems(DesignEditorDescriptorItem pressedMenuItem) {
			foreach(var designItemType in GetDefaultRibbonItemTypes(pressedMenuItem)) {
				ItemsOwner.AddItem(designItemType);
				TreeListItems.FindNodeByKeyID(ItemsOwner.FocusedNodeID).ExpandAll();
			}
		}
		protected virtual List<IDesignTimeColumnAndEditorItem> GetDefaultRibbonItemTypes(DesignEditorDescriptorItem pressedMenuItem) {
			if(pressedMenuItem.ParentItem != null)
				return new List<IDesignTimeColumnAndEditorItem>() { pressedMenuItem.ItemType };
			return SpreadsheetRibbonOwner.ColumnsAndEditors.Where(i => typeof(SRTab).IsAssignableFrom(i.ColumnType)).ToList();
		}
	}
	public partial class SpreadsheetContextTabsEditorForm : SpreadsheetRibbonItemsEditorForm {
		protected SpreadsheetRibbonContextTabCategoriesOwner SpreadsheetRibbonOwner { get { return ItemsOwner as SpreadsheetRibbonContextTabCategoriesOwner; } }
		protected override DialogResult ShowDeleteConfirmation() {
			return DesignUtils.ShowMessage(SpreadsheetRibbonOwner.Spreadsheet.Site, string.Format(string.Format("Do you want to delete the existing context tabs?")),
					string.Format("Create default context tabs for '{0}'", SpreadsheetRibbonOwner.Spreadsheet.GetType().Name), MessageBoxButtons.YesNoCancel);
		}
		protected override void CreateDefaultItems(DesignEditorDescriptorItem pressedMenuItem) {
			SpreadsheetRibbonOwner.CreateDefaultItems();
		}
	}
}
