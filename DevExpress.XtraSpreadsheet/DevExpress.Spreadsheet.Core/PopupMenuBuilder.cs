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
using System.Drawing;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.Utils.Menu;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpellChecker;
using DevExpress.XtraSpreadsheet.Layout.Engine;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.Utils.Commands;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Commands.Internal;
namespace DevExpress.XtraSpreadsheet.Menu {
	#region SpreadsheetMenuBuilder (abstract class)
	public abstract class SpreadsheetMenuBuilder : CommandBasedPopupMenuBuilder<SpreadsheetCommand, SpreadsheetCommandId> {
		readonly ISpreadsheetControl control;
		protected SpreadsheetMenuBuilder(ISpreadsheetControl control, IMenuBuilderUIFactory<SpreadsheetCommand, SpreadsheetCommandId> uiFactory)
			: base(uiFactory) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
		public ISpreadsheetControl Control { get { return control; } }
		protected IDXPopupMenu<SpreadsheetCommandId> AddSubmenuCore(IDXPopupMenu<SpreadsheetCommandId> menu, SpreadsheetSubmenuBuilder builder) {
			IDXPopupMenu<SpreadsheetCommandId> subMenu = builder.CreateSubMenu();
			Command command = builder.CreateCommandGroup();
			subMenu.Caption = command.MenuCaption;
			AppendSubmenu(menu, subMenu, false);
			return subMenu;
		}
	}
	#endregion
	#region SpreadsheetContentMenuBuilder (abstract class)
	public abstract class SpreadsheetContentMenuBuilder : SpreadsheetMenuBuilder {
		protected SpreadsheetContentMenuBuilder(ISpreadsheetControl control, IMenuBuilderUIFactory<SpreadsheetCommand, SpreadsheetCommandId> uiFactory)
			: base(control, uiFactory) {
		}
		public override void PopulatePopupMenu(IDXPopupMenu<SpreadsheetCommandId> menu) {
			InnerSpreadsheetControl innerControl = Control.InnerControl;
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.CutSelection));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.CopySelection));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PasteSelection));
			AddChartChangeTypeMenuItem(menu, innerControl);
			AddChartSelectDataMenuItem(menu, innerControl);
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.ChartChangeTitleContextMenuItem));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.ChartChangeHorizontalAxisTitleContextMenuItem));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.ChartChangeVerticalAxisTitleContextMenuItem));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.InsertSheetRowsContextMenuItem)).BeginGroup = true;
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.InsertSheetColumnsContextMenuItem)).BeginGroup = true;
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.RemoveSheetRowsContextMenuItem));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.RemoveSheetColumnsContextMenuItem));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.FormatClearContentsContextMenuItem)).BeginGroup = true;
			AddBringForwardSubmenu(menu).BeginGroup = true;
			AddSendBackwardSubmenu(menu);
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.ReviewInsertCommentContextMenuItem)).BeginGroup = true;
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.ReviewEditCommentContextMenuItem)).BeginGroup = true;
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.ReviewDeleteCommentContextMenuItem));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.ReviewShowHideCommentContextMenuItem));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.FormatCellsContextMenuItem)).BeginGroup = true;
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.FormatRowHeightContextMenuItem));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.FormatColumnWidthContextMenuItem));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.InsertHyperlinkContextMenuItem)); 
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.EditHyperlinkContextMenuItem));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.OpenHyperlinkContextMenuItem));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.OpenPictureHyperlinkContextMenuItem));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.RemoveHyperlinkContextMenuItem));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.RemoveHyperlinksContextMenuItem));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.HideRowsContextMenuItem)).BeginGroup = true;
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.HideColumnsContextMenuItem)).BeginGroup = true;
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.UnhideRowsContextMenuItem));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.UnhideColumnsContextMenuItem));
		}
		protected internal virtual void AddChartChangeTypeMenuItem(IDXPopupMenu<SpreadsheetCommandId> menu, InnerSpreadsheetControl innerControl) {
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.ChartChangeTypeContextMenuItem)).BeginGroup = true;
		}
		protected internal virtual void AddChartSelectDataMenuItem(IDXPopupMenu<SpreadsheetCommandId> menu, InnerSpreadsheetControl innerControl) {
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.ChartSelectDataContextMenuItem));
		}
		protected internal IDXPopupMenu<SpreadsheetCommandId> AddBringForwardSubmenu(IDXPopupMenu<SpreadsheetCommandId> menu) {
			SpreadsheetBringForwardSubmenuBuilder builder = new SpreadsheetBringForwardSubmenuBuilder(Control, UiFactory);
			return AddSubmenuCore(menu, builder);
		}
		protected internal IDXPopupMenu<SpreadsheetCommandId> AddSendBackwardSubmenu(IDXPopupMenu<SpreadsheetCommandId> menu) {
			SpreadsheetSendBackwardSubmenuBuilder builder = new SpreadsheetSendBackwardSubmenuBuilder(Control, UiFactory);
			return AddSubmenuCore(menu, builder);
		}
	}
	#endregion
	#region SpreadsheetSubmenuBuilder (abstract class)
	public abstract class SpreadsheetSubmenuBuilder : SpreadsheetMenuBuilder {
		protected SpreadsheetSubmenuBuilder(ISpreadsheetControl control, IMenuBuilderUIFactory<SpreadsheetCommand, SpreadsheetCommandId> uiFactory)
			: base(control, uiFactory) {
		}
		#region Properties
		protected internal abstract SpreadsheetCommandId CommandGroupId { get; }
		#endregion
		public override IDXPopupMenu<SpreadsheetCommandId> CreatePopupMenu() {
			IDXPopupMenu<SpreadsheetCommandId> menu = base.CreatePopupMenu() as IDXPopupMenu<SpreadsheetCommandId>;
			if (menu != null)
				UpdateMenuState(menu);
			return menu;
		}
		public override IDXPopupMenu<SpreadsheetCommandId> CreateSubMenu() {
			IDXPopupMenu<SpreadsheetCommandId> menu = base.CreateSubMenu() as IDXPopupMenu<SpreadsheetCommandId>;
			if (menu != null)
				UpdateMenuState(menu);
			return menu;
		}
		protected internal virtual void UpdateMenuState(IDXPopupMenu<SpreadsheetCommandId> menu) {
			Command command = CreateCommandGroup();
			ICommandUIState state = command.CreateDefaultCommandUIState();
			command.UpdateUIState(state);
			menu.Caption = command.MenuCaption;
			menu.Visible = state.Visible;
			SpreadsheetCommand spreadsheetCommand = command as SpreadsheetCommand;
			if (spreadsheetCommand != null)
				menu.Id = spreadsheetCommand.Id;
		}
		protected internal Command CreateCommandGroup() {
			return Control.CreateCommand(CommandGroupId);
		}
	}
	#endregion
	#region SpreadsheetBringForwardSubmenuBuilder
	public class SpreadsheetBringForwardSubmenuBuilder : SpreadsheetSubmenuBuilder {
		public SpreadsheetBringForwardSubmenuBuilder(ISpreadsheetControl control, IMenuBuilderUIFactory<SpreadsheetCommand, SpreadsheetCommandId> uiFactory)
			: base(control, uiFactory) {
		}
		#region Properties
		protected internal override SpreadsheetCommandId CommandGroupId { get { return SpreadsheetCommandId.ArrangeBringForwardCommandGroupContextMenuItem; } }
		#endregion
		public override void PopulatePopupMenu(IDXPopupMenu<SpreadsheetCommandId> menu) {
			InnerSpreadsheetControl innerControl = Control.InnerControl;
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.ArrangeBringForward));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.ArrangeBringToFront));
		}
	}
	#endregion
	#region SpreadsheetSendBackwardSubmenuBuilder
	public class SpreadsheetSendBackwardSubmenuBuilder : SpreadsheetSubmenuBuilder {
		public SpreadsheetSendBackwardSubmenuBuilder(ISpreadsheetControl control, IMenuBuilderUIFactory<SpreadsheetCommand, SpreadsheetCommandId> uiFactory)
			: base(control, uiFactory) {
		}
		#region Properties
		protected internal override SpreadsheetCommandId CommandGroupId { get { return SpreadsheetCommandId.ArrangeSendBackwardCommandGroupContextMenuItem; } }
		#endregion
		public override void PopulatePopupMenu(IDXPopupMenu<SpreadsheetCommandId> menu) {
			InnerSpreadsheetControl innerControl = Control.InnerControl;
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.ArrangeSendBackward));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.ArrangeSendToBack));
		}
	}
	#endregion
	#region SpreadsheetTabSelectorMenuBuilder (abstract class)
	public abstract class SpreadsheetTabSelectorMenuBuilder : SpreadsheetMenuBuilder {
		protected SpreadsheetTabSelectorMenuBuilder(ISpreadsheetControl control, IMenuBuilderUIFactory<SpreadsheetCommand, SpreadsheetCommandId> uiFactory)
			: base(control, uiFactory) {
		}
		public override void PopulatePopupMenu(IDXPopupMenu<SpreadsheetCommandId> menu) {
			InnerSpreadsheetControl innerControl = Control.InnerControl;
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.InsertSheetContextMenuItem));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.RemoveSheetContextMenuItem));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.RenameSheetContextMenuItem));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.MoveOrCopySheetContextMenuItem));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.ReviewProtectSheetContextMenuItem));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.ReviewUnprotectSheetContextMenuItem));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.HideSheetContextMenuItem));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.UnhideSheetContextMenuItem));
		}
	}
	#endregion
	#region SpreadsheetPivotTableMenuBuilder (abstract class)
	public abstract class SpreadsheetPivotTableMenuBuilder : SpreadsheetMenuBuilder {
		protected SpreadsheetPivotTableMenuBuilder(ISpreadsheetControl control, IMenuBuilderUIFactory<SpreadsheetCommand, SpreadsheetCommandId> uiFactory)
			: base(control, uiFactory) {
		}
		public override void PopulatePopupMenu(IDXPopupMenu<SpreadsheetCommandId> menu) {
			InnerSpreadsheetControl innerControl = Control.InnerControl;
			CreatePivotInfo();
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.CopySelection));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.FormatCellsContextMenuItem));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.RefreshPivotTable)).BeginGroup = true;
			AddSortFieldsSubmenu(menu).BeginGroup = true;
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.SubtotalPivotField)).BeginGroup = true;
			IDXPopupMenu<SpreadsheetCommandId> moveSubMenu = AddMoveSubmenu(menu);
			moveSubMenu.BeginGroup = true;
			IDXMenuItem<SpreadsheetCommandId> removeMenuItem = AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.RemovePivotFieldCommand));
			removeMenuItem.BeginGroup = !moveSubMenu.Visible;
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.RemoveGrandTotalPivotTable)).BeginGroup = true;
			AddSummarizeValuesBySubmenu(menu).BeginGroup = true;
			AddShowValuesAsSubmenu(menu);
			AddExpandCollapseFieldSubmenu(menu).BeginGroup = true;
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.FieldSettingsPivotTableContextMenuItem)).BeginGroup = true;
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFieldSettingsPivotTableContextMenuItem)).BeginGroup = true;
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.OptionsPivotTableContextMenuItem));
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.FieldListPanelPivotTableContextMenuItem));
		}
		void CreatePivotInfo() {
			Worksheet activeSheet = Control.InnerControl.DocumentModel.ActiveSheet;
			CellPosition activeCell = activeSheet.Selection.ActiveCell;
			PivotTable pivotTable = activeSheet.TryGetPivotTable(activeCell);
			int tableIndex = activeSheet.PivotTables.IndexOf(pivotTable);
			PivotFieldZoneInfo zoneInfo = pivotTable.CalculationInfo.GetActiveFieldInfo(activeCell);
			PivotTableStaticInfo pivotInfo = new PivotTableStaticInfo(tableIndex, zoneInfo.Axis);
			pivotInfo.FieldIndex = zoneInfo.FieldIndex;
			pivotInfo.FieldReferenceIndex = zoneInfo.FieldReferenceIndex;
			activeSheet.PivotTableStaticInfo = pivotInfo;
		}
		protected internal IDXPopupMenu<SpreadsheetCommandId> AddSortFieldsSubmenu(IDXPopupMenu<SpreadsheetCommandId> menu) {
			SpreadsheetPivotTableSortFieldsSubmenuBuilder builder = new SpreadsheetPivotTableSortFieldsSubmenuBuilder(Control, UiFactory);
			return AddSubmenuCore(menu, builder);
		}
		protected internal IDXPopupMenu<SpreadsheetCommandId> AddMoveSubmenu(IDXPopupMenu<SpreadsheetCommandId> menu) {
			SpreadsheetPivotTableMoveFieldReferenceSubmenuBuilder builder = new SpreadsheetPivotTableMoveFieldReferenceSubmenuBuilder(Control, UiFactory);
			return AddSubmenuCore(menu, builder);
		}
		protected internal IDXPopupMenu<SpreadsheetCommandId> AddSummarizeValuesBySubmenu(IDXPopupMenu<SpreadsheetCommandId> menu) {
			SpreadsheetPivotTableSummarizeValuesBySubmenuBuilder builder = new SpreadsheetPivotTableSummarizeValuesBySubmenuBuilder(Control, UiFactory);
			return AddSubmenuCore(menu, builder);
		}
		protected internal IDXPopupMenu<SpreadsheetCommandId> AddShowValuesAsSubmenu(IDXPopupMenu<SpreadsheetCommandId> menu) {
			SpreadsheetPivotTableShowValuesAsSubmenuBuilder builder = new SpreadsheetPivotTableShowValuesAsSubmenuBuilder(Control, UiFactory);
			return AddSubmenuCore(menu, builder);
		}
		protected internal IDXPopupMenu<SpreadsheetCommandId> AddExpandCollapseFieldSubmenu(IDXPopupMenu<SpreadsheetCommandId> menu) {
			SpreadsheetPivotTableExpandCollapseFieldSubmenuBuilder builder = new SpreadsheetPivotTableExpandCollapseFieldSubmenuBuilder(Control, UiFactory);
			return AddSubmenuCore(menu, builder);
		}
	}
	#endregion
	#region SpreadsheetPivotTableSortFieldsSubmenuBuilder
	public class SpreadsheetPivotTableSortFieldsSubmenuBuilder : SpreadsheetSubmenuBuilder {
		public SpreadsheetPivotTableSortFieldsSubmenuBuilder(ISpreadsheetControl control, IMenuBuilderUIFactory<SpreadsheetCommand, SpreadsheetCommandId> uiFactory)
			: base(control, uiFactory) {
		}
		#region Properties
		protected internal override SpreadsheetCommandId CommandGroupId { get { return SpreadsheetCommandId.PivotTableFieldSortCommandGroup; } }
		#endregion
		public override void PopulatePopupMenu(IDXPopupMenu<SpreadsheetCommandId> menu) {
			InnerSpreadsheetControl innerControl = Control.InnerControl;
			SpreadsheetCommand sortAscendingCommand = innerControl.CreateCommand(SpreadsheetCommandId.PivotTableFieldSortAscending);
			(sortAscendingCommand as PivotTableFieldSortAscendingCommand).IsPivotTableContextMenu = true;
			SpreadsheetCommand sortDescendingCommand = innerControl.CreateCommand(SpreadsheetCommandId.PivotTableFieldSortDescending);
			(sortDescendingCommand as PivotTableFieldSortDescendingCommand).IsPivotTableContextMenu = true;
			AddMenuItem(menu, sortAscendingCommand);
			AddMenuItem(menu, sortDescendingCommand);
		}
	}
	#endregion
	#region SpreadsheetPivotTableMoveFieldReferenceSubmenuBuilder
	public class SpreadsheetPivotTableMoveFieldReferenceSubmenuBuilder : SpreadsheetSubmenuBuilder {
		public SpreadsheetPivotTableMoveFieldReferenceSubmenuBuilder(ISpreadsheetControl control, IMenuBuilderUIFactory<SpreadsheetCommand, SpreadsheetCommandId> uiFactory)
			: base(control, uiFactory) {
		}
		#region Properties
		protected internal override SpreadsheetCommandId CommandGroupId { get { return SpreadsheetCommandId.MovePivotFieldReferenceCommandGroup; } }
		#endregion
		public override void PopulatePopupMenu(IDXPopupMenu<SpreadsheetCommandId> menu) {
			InnerSpreadsheetControl innerControl = Control.InnerControl;
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.MovePivotFieldItemToBeginning));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.MovePivotFieldItemUp));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.MovePivotFieldItemDown));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.MovePivotFieldItemToEnd));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.MovePivotFieldReferenceToBeginning));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.MovePivotFieldReferenceUp));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.MovePivotFieldReferenceDown));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.MovePivotFieldReferenceToEnd));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.MovePivotFieldReferenceToDifferentAxis)).BeginGroup = true;
		}
	}
	#endregion
	#region SpreadsheetPivotTableSummarizeValuesBySubmenuBuilder
	public class SpreadsheetPivotTableSummarizeValuesBySubmenuBuilder : SpreadsheetSubmenuBuilder {
		public SpreadsheetPivotTableSummarizeValuesBySubmenuBuilder(ISpreadsheetControl control, IMenuBuilderUIFactory<SpreadsheetCommand, SpreadsheetCommandId> uiFactory)
			: base(control, uiFactory) {
		}
		#region Properties
		protected internal override SpreadsheetCommandId CommandGroupId { get { return SpreadsheetCommandId.PivotTableSummarizeValuesByCommandGroup; } }
		#endregion
		public override void PopulatePopupMenu(IDXPopupMenu<SpreadsheetCommandId> menu) {
			InnerSpreadsheetControl innerControl = Control.InnerControl;
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableSummarizeValuesBySum));
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableSummarizeValuesByCount));
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableSummarizeValuesByAverage));
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableSummarizeValuesByMax));
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableSummarizeValuesByMin));
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableSummarizeValuesByProduct));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableSummarizeValuesByMoreOptions)).BeginGroup = true;
		}
	}
	#endregion
	#region SpreadsheetPivotTableShowValuesAsSubmenuBuilder
	public class SpreadsheetPivotTableShowValuesAsSubmenuBuilder : SpreadsheetSubmenuBuilder {
		public SpreadsheetPivotTableShowValuesAsSubmenuBuilder(ISpreadsheetControl control, IMenuBuilderUIFactory<SpreadsheetCommand, SpreadsheetCommandId> uiFactory)
			: base(control, uiFactory) {
		}
		#region Properties
		protected internal override SpreadsheetCommandId CommandGroupId { get { return SpreadsheetCommandId.PivotTableShowValuesAsCommandGroup; } }
		#endregion
		public override void PopulatePopupMenu(IDXPopupMenu<SpreadsheetCommandId> menu) {
			InnerSpreadsheetControl innerControl = Control.InnerControl;
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableShowValuesAsNormal));
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableShowValuesAsPercentOfTotal));
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableShowValuesAsPercentOfColumn));
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableShowValuesAsPercentOfRow));
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableShowValuesAsPercent));
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableShowValuesAsPercentOfParentRow));
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableShowValuesAsPercentOfParentColumn));
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableShowValuesAsPercentOfParent));
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableShowValuesAsDifference));
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableShowValuesAsPercentDifference));
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableShowValuesAsRunningTotal));
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableShowValuesAsPercentOfRunningTotal));
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableShowValuesAsRankAscending));
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableShowValuesAsRankDescending));
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableShowValuesAsIndex));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableShowValuesAsMoreOptions)).BeginGroup = true;
		}
	}
	#endregion
	#region SpreadsheetPivotTableExpandCollapseFieldSubmenuBuilder
	public class SpreadsheetPivotTableExpandCollapseFieldSubmenuBuilder : SpreadsheetSubmenuBuilder {
		public SpreadsheetPivotTableExpandCollapseFieldSubmenuBuilder(ISpreadsheetControl control, IMenuBuilderUIFactory<SpreadsheetCommand, SpreadsheetCommandId> uiFactory)
			: base(control, uiFactory) {
		}
		#region Properties
		protected internal override SpreadsheetCommandId CommandGroupId { get { return SpreadsheetCommandId.PivotTableExpandCollapseFieldCommandGroup; } }
		#endregion
		public override void PopulatePopupMenu(IDXPopupMenu<SpreadsheetCommandId> menu) {
			InnerSpreadsheetControl innerControl = Control.InnerControl;
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableExpandFieldContextMenuItem));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableCollapseFieldContextMenuItem));
		}
	}
	#endregion
	#region SpreadsheetPivotTableAutoFilterMenuBuilder
	public class SpreadsheetPivotTableAutoFilterMenuBuilder : SpreadsheetMenuBuilder {
		public SpreadsheetPivotTableAutoFilterMenuBuilder(ISpreadsheetControl control, IMenuBuilderUIFactory<SpreadsheetCommand, SpreadsheetCommandId> uiFactory)
			: base(control, uiFactory) {
		}
		public override void PopulatePopupMenu(IDXPopupMenu<SpreadsheetCommandId> menu) {
			InnerSpreadsheetControl innerControl = Control.InnerControl;
			AddPivotFieldsSubmenu(menu);
			SpreadsheetCommand sortAscendingCommand = innerControl.CreateCommand(SpreadsheetCommandId.PivotTableFieldSortAscending);
			(sortAscendingCommand as PivotTableFieldSortAscendingCommand).IsPivotTableContextMenu = false;
			AddMenuCheckItem(menu, sortAscendingCommand).BeginGroup = true;
			SpreadsheetCommand sortDescendingCommand = innerControl.CreateCommand(SpreadsheetCommandId.PivotTableFieldSortDescending);
			(sortDescendingCommand as PivotTableFieldSortDescendingCommand).IsPivotTableContextMenu = false;
			AddMenuCheckItem(menu, sortDescendingCommand);
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableClearFieldFilters)).BeginGroup = true;
			AddLabelFiltersSubmenu(menu);
			AddDateFiltersSubmenu(menu);
			AddValueFiltersSubmenu(menu);
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableRowFieldsFilterItems)).BeginGroup = true;
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableColumnFieldsFilterItems)).BeginGroup = true; 
		}
		protected internal IDXPopupMenu<SpreadsheetCommandId> AddPivotFieldsSubmenu(IDXPopupMenu<SpreadsheetCommandId> menu) {
			SpreadsheetPivotTableAutoFilterFieldsSubmenuBuilder builder = new SpreadsheetPivotTableAutoFilterFieldsSubmenuBuilder(Control, UiFactory);
			return AddSubmenuCore(menu, builder);
		}
		protected internal IDXPopupMenu<SpreadsheetCommandId> AddLabelFiltersSubmenu(IDXPopupMenu<SpreadsheetCommandId> menu) {
			SpreadsheetPivotTableAutoFilterLabelFiltersSubmenuBuilder builder = new SpreadsheetPivotTableAutoFilterLabelFiltersSubmenuBuilder(Control, UiFactory);
			return AddSubmenuCore(menu, builder);
		}
		protected internal IDXPopupMenu<SpreadsheetCommandId> AddDateFiltersSubmenu(IDXPopupMenu<SpreadsheetCommandId> menu) {
			SpreadsheetPivotTableAutoFilterDateFiltersSubmenuBuilder builder = new SpreadsheetPivotTableAutoFilterDateFiltersSubmenuBuilder(Control, UiFactory);
			return AddSubmenuCore(menu, builder);
		}
		protected internal IDXPopupMenu<SpreadsheetCommandId> AddValueFiltersSubmenu(IDXPopupMenu<SpreadsheetCommandId> menu) {
			SpreadsheetPivotTableAutoFilterValueFiltersSubmenuBuilder builder = new SpreadsheetPivotTableAutoFilterValueFiltersSubmenuBuilder(Control, UiFactory);
			return AddSubmenuCore(menu, builder);
		}
	}
	#endregion
	#region SpreadsheetPivotTableAutoFilterFieldsSubmenuBuilder
	public class SpreadsheetPivotTableAutoFilterFieldsSubmenuBuilder : SpreadsheetSubmenuBuilder {
		public SpreadsheetPivotTableAutoFilterFieldsSubmenuBuilder(ISpreadsheetControl control, IMenuBuilderUIFactory<SpreadsheetCommand, SpreadsheetCommandId> uiFactory)
			: base(control, uiFactory) {
		}
		#region Properties
		protected internal override SpreadsheetCommandId CommandGroupId { get { return SpreadsheetCommandId.PivotTableFieldsFiltersCommandGroup; } }
		#endregion
		public override void PopulatePopupMenu(IDXPopupMenu<SpreadsheetCommandId> menu) {
			InnerSpreadsheetControl innerControl = Control.InnerControl;
			PivotTableStaticInfo info = Control.Document.Model.DocumentModel.ActiveSheet.PivotTableStaticInfo;
			info.FieldIndexList = FindAllPivotFieldIndeces(info);
			for (int i = 0; i < info.FieldIndexList.Count; i++)
				AddMenuItem(menu, new PivotTableSelectFieldFiltersCommand(Control, info.FieldIndexList[i]));
		}
		List<int> FindAllPivotFieldIndeces(PivotTableStaticInfo info) {
			PivotTable table = Control.Document.Model.DocumentModel.ActiveSheet.PivotTables[info.TableIndex];
			List<int> fieldIndexList = new List<int>();
			fieldIndexList = (!info.IsPageFilter && info.IsRowFieldsFilter) ? PopulateFieldIndexList(table.RowFields) : PopulateFieldIndexList(table.ColumnFields);			 
			return fieldIndexList;
		}
		List<int> PopulateFieldIndexList(PivotTableColumnRowFieldIndices fields) {
			List<int> result = new List<int>();
			foreach (PivotFieldReference fieldReference in fields) {
				int fieldIndex = fieldReference.FieldIndex;
				if (fieldIndex >= 0)
					result.Add(fieldIndex);
			}
			return result;
		}
	}
	#endregion
	#region SpreadsheetPivotTableAutoFilterLabelFiltersSubmenuBuilder
	public class SpreadsheetPivotTableAutoFilterLabelFiltersSubmenuBuilder : SpreadsheetSubmenuBuilder {
		public SpreadsheetPivotTableAutoFilterLabelFiltersSubmenuBuilder(ISpreadsheetControl control, IMenuBuilderUIFactory<SpreadsheetCommand, SpreadsheetCommandId> uiFactory)
			: base(control, uiFactory) {
		}
		#region Properties
		protected internal override SpreadsheetCommandId CommandGroupId { get { return SpreadsheetCommandId.PivotTableLabelFiltersCommandGroup; } }
		#endregion
		public override void PopulatePopupMenu(IDXPopupMenu<SpreadsheetCommandId> menu) {
			InnerSpreadsheetControl innerControl = Control.InnerControl;
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableClearFieldLabelFilter));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableLabelFilterEquals)).BeginGroup = true;
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableLabelFilterDoesNotEqual));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableLabelFilterBeginsWith)).BeginGroup = true;
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableLabelFilterDoesNotBeginWith));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableLabelFilterEndsWith));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableLabelFilterDoesNotEndWith));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableLabelFilterContains)).BeginGroup = true;
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableLabelFilterDoesNotContain));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableLabelFilterGreaterThan)).BeginGroup = true;
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableLabelFilterGreaterThanOrEqual));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableLabelFilterLessThan));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableLabelFilterLessThanOrEqual));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableLabelFilterBetween)).BeginGroup = true;
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableLabelFilterNotBetween));
		}
	}
	#endregion
	#region SpreadsheetPivotTableAutoFilterDateFiltersSubmenuBuilder
	public class SpreadsheetPivotTableAutoFilterDateFiltersSubmenuBuilder : SpreadsheetSubmenuBuilder {
		public SpreadsheetPivotTableAutoFilterDateFiltersSubmenuBuilder(ISpreadsheetControl control, IMenuBuilderUIFactory<SpreadsheetCommand, SpreadsheetCommandId> uiFactory)
			: base(control, uiFactory) {
		}
		#region Properties
		protected internal override SpreadsheetCommandId CommandGroupId { get { return SpreadsheetCommandId.PivotTableDateFiltersCommandGroup; } }
		#endregion
		public override void PopulatePopupMenu(IDXPopupMenu<SpreadsheetCommandId> menu) {
			InnerSpreadsheetControl innerControl = Control.InnerControl;
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableClearFieldLabelFilter));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableDateFilterEquals)).BeginGroup = true;
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableDateFilterBefore)).BeginGroup = true;
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableDateFilterAfter));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableDateFilterBetween));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableDateFilterTomorrow)).BeginGroup = true;
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableDateFilterToday));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableDateFilterYesterday));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableDateFilterNextWeek)).BeginGroup = true;
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableDateFilterThisWeek));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableDateFilterLastWeek));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableDateFilterNextMonth)).BeginGroup = true;
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableDateFilterThisMonth));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableDateFilterLastMonth));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableDateFilterNextQuarter)).BeginGroup = true;
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableDateFilterThisQuarter));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableDateFilterLastQuarter));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableDateFilterNextYear)).BeginGroup = true;
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableDateFilterThisYear));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableDateFilterLastYear));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableDateFilterYearToDate)).BeginGroup = true;
			AddFilterAllDatesInPeriodSubmenu(menu).BeginGroup = true;
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableDateFilterCustom)).BeginGroup = true;
		}
		protected internal IDXPopupMenu<SpreadsheetCommandId> AddFilterAllDatesInPeriodSubmenu(IDXPopupMenu<SpreadsheetCommandId> menu) {
			SpreadsheetPivotTableFilterAllDatesInPeriodSubmenuBuilder builder = new SpreadsheetPivotTableFilterAllDatesInPeriodSubmenuBuilder(Control, UiFactory);
			return AddSubmenuCore(menu, builder);
		}
	}
	#endregion
	#region SpreadsheetPivotTableFilterAllDatesInPeriodSubmenuBuilder
	public class SpreadsheetPivotTableFilterAllDatesInPeriodSubmenuBuilder : SpreadsheetSubmenuBuilder {
		public SpreadsheetPivotTableFilterAllDatesInPeriodSubmenuBuilder(ISpreadsheetControl control, IMenuBuilderUIFactory<SpreadsheetCommand, SpreadsheetCommandId> uiFactory)
			: base(control, uiFactory) {
		}
		#region Properties
		protected internal override SpreadsheetCommandId CommandGroupId { get { return SpreadsheetCommandId.PivotTableFilterAllDatesInPeriodCommandGroup; } }
		#endregion
		public override void PopulatePopupMenu(IDXPopupMenu<SpreadsheetCommandId> menu) {
			InnerSpreadsheetControl innerControl = Control.InnerControl;
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableDateFilterFirstQuarter));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableDateFilterSecondQuarter));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableDateFilterThirdQuarter));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableDateFilterFourthQuarter));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableDateFilterJanuary)).BeginGroup = true;
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableDateFilterFebruary));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableDateFilterMarch));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableDateFilterApril));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableDateFilterMay));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableDateFilterJune));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableDateFilterJuly));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableDateFilterAugust));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableDateFilterSeptember));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableDateFilterOctober));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableDateFilterNovember));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableDateFilterDecember));
		}
	}
	#endregion
	#region SpreadsheetPivotTableAutoFilterValueFiltersSubmenuBuilder
	public class SpreadsheetPivotTableAutoFilterValueFiltersSubmenuBuilder : SpreadsheetSubmenuBuilder {
		public SpreadsheetPivotTableAutoFilterValueFiltersSubmenuBuilder(ISpreadsheetControl control, IMenuBuilderUIFactory<SpreadsheetCommand, SpreadsheetCommandId> uiFactory)
			: base(control, uiFactory) {
		}
		#region Properties
		protected internal override SpreadsheetCommandId CommandGroupId { get { return SpreadsheetCommandId.PivotTableValueFiltersCommandGroup; } }
		#endregion
		public override void PopulatePopupMenu(IDXPopupMenu<SpreadsheetCommandId> menu) {
			InnerSpreadsheetControl innerControl = Control.InnerControl;
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableClearFieldValueFilter));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableValueFilterEquals)).BeginGroup = true;
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableValueFilterDoesNotEqual));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableValueFilterGreaterThan)).BeginGroup = true;
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableValueFilterGreaterThanOrEqual));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableValueFilterLessThan));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableValueFilterLessThanOrEqual));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableValueFilterBetween)).BeginGroup = true;
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableValueFilterNotBetween));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.PivotTableValueFilterTop10)).BeginGroup = true;
		}
	}
	#endregion
	#region SpreadsheetAutoFilterMenuBuilder
	public class SpreadsheetAutoFilterMenuBuilder : SpreadsheetMenuBuilder {
		public SpreadsheetAutoFilterMenuBuilder(ISpreadsheetControl control, IMenuBuilderUIFactory<SpreadsheetCommand, SpreadsheetCommandId> uiFactory)
			: base(control, uiFactory) {
		}
		public override void PopulatePopupMenu(IDXPopupMenu<SpreadsheetCommandId> menu) {
			InnerSpreadsheetControl innerControl = Control.InnerControl;
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataSortAscending));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataSortDescending));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterColumnClear)).BeginGroup = true;
			AddDateFiltersSubmenu(menu);
			AddTextFiltersSubmenu(menu);
			AddNumberFiltersSubmenu(menu);
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterSimple)).BeginGroup = true;
		}
		protected internal IDXPopupMenu<SpreadsheetCommandId> AddDateFiltersSubmenu(IDXPopupMenu<SpreadsheetCommandId> menu) {
			SpreadsheetAutoFilterDateFiltersSubmenuBuilder builder = new SpreadsheetAutoFilterDateFiltersSubmenuBuilder(Control, UiFactory);
			return AddSubmenuCore(menu, builder);
		}
		protected internal IDXPopupMenu<SpreadsheetCommandId> AddTextFiltersSubmenu(IDXPopupMenu<SpreadsheetCommandId> menu) {
			SpreadsheetAutoFilterTextFiltersSubmenuBuilder builder = new SpreadsheetAutoFilterTextFiltersSubmenuBuilder(Control, UiFactory);
			return AddSubmenuCore(menu, builder);
		}
		protected internal IDXPopupMenu<SpreadsheetCommandId> AddNumberFiltersSubmenu(IDXPopupMenu<SpreadsheetCommandId> menu) {
			SpreadsheetAutoFilterNumberFiltersSubmenuBuilder builder = new SpreadsheetAutoFilterNumberFiltersSubmenuBuilder(Control, UiFactory);
			return AddSubmenuCore(menu, builder);
		}
	}
	#endregion
	#region SpreadsheetAutoFilterDateFiltersSubmenuBuilder
	public class SpreadsheetAutoFilterDateFiltersSubmenuBuilder : SpreadsheetSubmenuBuilder {
		public SpreadsheetAutoFilterDateFiltersSubmenuBuilder(ISpreadsheetControl control, IMenuBuilderUIFactory<SpreadsheetCommand, SpreadsheetCommandId> uiFactory)
			: base(control, uiFactory) {
		}
		#region Properties
		protected internal override SpreadsheetCommandId CommandGroupId { get { return SpreadsheetCommandId.DataFilterDateFiltersCommandGroup; } }
		#endregion
		public override void PopulatePopupMenu(IDXPopupMenu<SpreadsheetCommandId> menu) {
			InnerSpreadsheetControl innerControl = Control.InnerControl;
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterDateEquals));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterDateBefore)).BeginGroup = true;
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterDateAfter));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterDateBetween));
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterTomorrow)).BeginGroup = true;
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterToday));
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterYesterday));
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterNextWeek)).BeginGroup = true;
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterThisWeek));
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterLastWeek));
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterNextMonth)).BeginGroup = true;
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterThisMonth));
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterLastMonth));
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterNextQuarter)).BeginGroup = true;
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterThisQuarter));
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterLastQuarter));
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterNextYear)).BeginGroup = true;
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterThisYear));
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterLastYear));
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterYearToDate)).BeginGroup = true;
			AddAllDatesInPeriodSubmenu(menu).BeginGroup = true;
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterDateCustom)).BeginGroup = true;
		}
		protected internal IDXPopupMenu<SpreadsheetCommandId> AddAllDatesInPeriodSubmenu(IDXPopupMenu<SpreadsheetCommandId> menu) {
			SpreadsheetAutoFilterAllDatesInPeriodFiltersSubmenuBuilder builder = new SpreadsheetAutoFilterAllDatesInPeriodFiltersSubmenuBuilder(Control, UiFactory);
			return AddSubmenuCore(menu, builder);
		}
	}
	#endregion
	#region SpreadsheetAutoFilterAllDatesInPeriodFiltersSubmenuBuilder
	public class SpreadsheetAutoFilterAllDatesInPeriodFiltersSubmenuBuilder : SpreadsheetSubmenuBuilder {
		public SpreadsheetAutoFilterAllDatesInPeriodFiltersSubmenuBuilder(ISpreadsheetControl control, IMenuBuilderUIFactory<SpreadsheetCommand, SpreadsheetCommandId> uiFactory)
			: base(control, uiFactory) {
		}
		#region Properties
		protected internal override SpreadsheetCommandId CommandGroupId { get { return SpreadsheetCommandId.DataFilterAllDatesInPeriodCommandGroup; } }
		#endregion
		public override void PopulatePopupMenu(IDXPopupMenu<SpreadsheetCommandId> menu) {
			InnerSpreadsheetControl innerControl = Control.InnerControl;
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterQuarter1));
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterQuarter2));
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterQuarter3));
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterQuarter4));
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterMonthJanuary)).BeginGroup = true;
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterMonthFebruary));
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterMonthMarch));
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterMonthApril));
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterMonthMay));
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterMonthJune));
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterMonthJuly));
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterMonthAugust));
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterMonthSeptember));
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterMonthOctober));
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterMonthNovember));
			AddMenuCheckItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterMonthDecember));
		}
	}
	#endregion
	#region SpreadsheetAutoFilterTextFiltersSubmenuBuilder
	public class SpreadsheetAutoFilterTextFiltersSubmenuBuilder : SpreadsheetSubmenuBuilder {
		public SpreadsheetAutoFilterTextFiltersSubmenuBuilder(ISpreadsheetControl control, IMenuBuilderUIFactory<SpreadsheetCommand, SpreadsheetCommandId> uiFactory)
			: base(control, uiFactory) {
		}
		#region Properties
		protected internal override SpreadsheetCommandId CommandGroupId { get { return SpreadsheetCommandId.DataFilterTextFiltersCommandGroup; } }
		#endregion
		public override void PopulatePopupMenu(IDXPopupMenu<SpreadsheetCommandId> menu) {
			InnerSpreadsheetControl innerControl = Control.InnerControl;
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterEquals));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterDoesNotEqual));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterBeginsWith)).BeginGroup = true;
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterEndsWith));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterContains)).BeginGroup = true;
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterDoesNotContain));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterCustom)).BeginGroup = true;
		}
	}
	#endregion
	#region SpreadsheetAutoFilterNumberFiltersSubmenuBuilder
	public class SpreadsheetAutoFilterNumberFiltersSubmenuBuilder : SpreadsheetSubmenuBuilder {
		public SpreadsheetAutoFilterNumberFiltersSubmenuBuilder(ISpreadsheetControl control, IMenuBuilderUIFactory<SpreadsheetCommand, SpreadsheetCommandId> uiFactory)
			: base(control, uiFactory) {
		}
		#region Properties
		protected internal override SpreadsheetCommandId CommandGroupId { get { return SpreadsheetCommandId.DataFilterNumberFiltersCommandGroup; } }
		#endregion
		public override void PopulatePopupMenu(IDXPopupMenu<SpreadsheetCommandId> menu) {
			InnerSpreadsheetControl innerControl = Control.InnerControl;
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterEquals));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterDoesNotEqual));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterGreaterThan)).BeginGroup = true;
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterGreaterThanOrEqualTo));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterLessThan));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterLessThanOrEqualTo));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterBetween));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterTop10)).BeginGroup = true;
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterAboveAverage));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterBelowAverage));
			AddMenuItem(menu, innerControl.CreateCommand(SpreadsheetCommandId.DataFilterCustom)).BeginGroup = true;
		}
	}
	#endregion
}
