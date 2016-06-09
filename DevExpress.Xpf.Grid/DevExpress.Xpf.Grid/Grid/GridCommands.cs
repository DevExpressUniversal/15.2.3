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
using System.Windows.Input;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows;
using DevExpress.Xpf.Printing;
using DevExpress.Mvvm;
using DevExpress.Xpf.Core.ConditionalFormatting;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
namespace DevExpress.Xpf.Grid {
	public static class GridCommands {
		#if !SL
		static readonly RoutedCommand changeGroupExpanded = new RoutedCommand("ChangeGroupExpanded", typeof(GridCommands));
		static readonly RoutedCommand expandAllGroups = new RoutedCommand("ExpandAllGroups", typeof(GridCommands));
		static readonly RoutedCommand collapseAllGroups = new RoutedCommand("CollapseAllGroups", typeof(GridCommands));
		static readonly RoutedCommand moveParentGroupRow = new RoutedCommand("MoveParentGroupRow", typeof(GridCommands));
		static readonly RoutedCommand clearGrouping = new RoutedCommand("ClearGrouping", typeof(GridCommands));
		static readonly RoutedCommand changeCardExpanded = new RoutedCommand("ChangeCardExpanded", typeof(GridCommands));
		static readonly RoutedCommand bestFitColumn = new RoutedCommand("BestFitColumn", typeof(GridCommands));
		static readonly RoutedCommand bestFitColumns = new RoutedCommand("BestFitColumns", typeof(GridCommands));
		static readonly RoutedCommand addNewRow = new RoutedCommand("AddNewRow", typeof(GridCommands)); 
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridCommandsChangeGroupExpanded")]
#endif
		public static RoutedCommand ChangeGroupExpanded { get { return changeGroupExpanded; } }
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridCommandsExpandAllGroups")]
#endif
		public static RoutedCommand ExpandAllGroups { get { return expandAllGroups; } }
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridCommandsCollapseAllGroups")]
#endif
		public static RoutedCommand CollapseAllGroups { get { return collapseAllGroups; } }
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridCommandsChangeCardExpanded")]
#endif
		public static RoutedCommand ChangeCardExpanded { get { return changeCardExpanded; } }
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridCommandsClearGrouping")]
#endif
		public static RoutedCommand ClearGrouping { get { return clearGrouping; } }
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridCommandsBestFitColumn")]
#endif
		public static RoutedCommand BestFitColumn { get { return bestFitColumn; } }
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridCommandsBestFitColumns")]
#endif
		public static RoutedCommand BestFitColumns { get { return bestFitColumns; } }
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridCommandsMoveParentGroupRow")]
#endif
		public static RoutedCommand MoveParentGroupRow { get { return moveParentGroupRow; } }
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridCommandsAddNewRow")]
#endif
		public static RoutedCommand AddNewRow { get { return addNewRow; } }
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridCommandsChangeColumnSortOrder")]
#endif
		public static RoutedCommand ChangeColumnSortOrder { get { return DataControlCommands.ChangeColumnSortOrder; } }
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridCommandsClearColumnFilter")]
#endif
		public static RoutedCommand ClearColumnFilter { get { return DataControlCommands.ClearColumnFilter; } }
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Instead use the ClearColumnFilter property.")]
		public static RoutedCommand ClearFilterColumn { get { return DataControlCommands.ClearColumnFilter; } }
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridCommandsShowFilterEditor")]
#endif
		public static RoutedCommand ShowFilterEditor { get { return DataControlCommands.ShowFilterEditor; } }
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridCommandsShowColumnChooser")]
#endif
		public static RoutedCommand ShowColumnChooser { get { return DataControlCommands.ShowColumnChooser; } }
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridCommandsHideColumnChooser")]
#endif
		public static RoutedCommand HideColumnChooser { get { return DataControlCommands.HideColumnChooser; } }
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridCommandsMovePrevCell")]
#endif
		public static RoutedCommand MovePrevCell { get { return DataControlCommands.MovePrevCell; } }
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridCommandsMoveNextCell")]
#endif
		public static RoutedCommand MoveNextCell { get { return DataControlCommands.MoveNextCell; } }
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridCommandsMovePrevRow")]
#endif
		public static RoutedCommand MovePrevRow { get { return DataControlCommands.MovePrevRow; } }
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridCommandsMoveNextRow")]
#endif
		public static RoutedCommand MoveNextRow { get { return DataControlCommands.MoveNextRow; } }
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridCommandsMoveFirstRow")]
#endif
		public static RoutedCommand MoveFirstRow { get { return DataControlCommands.MoveFirstRow; } }
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridCommandsMoveLastRow")]
#endif
		public static RoutedCommand MoveLastRow { get { return DataControlCommands.MoveLastRow; } }
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridCommandsMovePrevPage")]
#endif
		public static RoutedCommand MovePrevPage { get { return DataControlCommands.MovePrevPage; } }
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridCommandsMoveNextPage")]
#endif
		public static RoutedCommand MoveNextPage { get { return DataControlCommands.MoveNextPage; } }
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridCommandsMoveFirstCell")]
#endif
		public static RoutedCommand MoveFirstCell { get { return DataControlCommands.MoveFirstCell; } }
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridCommandsMoveLastCell")]
#endif
		public static RoutedCommand MoveLastCell { get { return DataControlCommands.MoveLastCell; } }
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridCommandsClearFilter")]
#endif
		public static RoutedCommand ClearFilter { get { return DataControlCommands.ClearFilter; } }
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridCommandsDeleteFocusedRow")]
#endif
		public static RoutedCommand DeleteFocusedRow { get { return DataControlCommands.DeleteFocusedRow; } }
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridCommandsEditFocusedRow")]
#endif
		public static RoutedCommand EditFocusedRow { get { return DataControlCommands.EditFocusedRow; } }
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridCommandsCancelEditFocusedRow")]
#endif
		public static RoutedCommand CancelEditFocusedRow { get { return DataControlCommands.CancelEditFocusedRow; } }
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridCommandsEndEditFocusedRow")]
#endif
		public static RoutedCommand EndEditFocusedRow { get { return DataControlCommands.EndEditFocusedRow; } }
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridCommandsShowUnboundExpressionEditor")]
#endif
		public static RoutedCommand ShowUnboundExpressionEditor { get { return DataControlCommands.ShowUnboundExpressionEditor; } }
		#endif
	}
	public abstract class GridViewCommandsBase : DataViewCommandsBase {
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridViewCommandsBaseChangeGroupExpanded")]
#endif
		public ICommand ChangeGroupExpanded { get; private set; }
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridViewCommandsBaseExpandAllGroups")]
#endif
		public ICommand ExpandAllGroups { get; private set; }
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridViewCommandsBaseCollapseAllGroups")]
#endif
		public ICommand CollapseAllGroups { get; private set; }
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridViewCommandsBaseMoveParentGroupRow")]
#endif
		public ICommand MoveParentGroupRow { get; private set; }
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridViewCommandsBaseClearGrouping")]
#endif
		public ICommand ClearGrouping { get; private set; }
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridViewCommandsBaseShowGroupSummaryEditor")]
#endif
		public ICommand ShowGroupSummaryEditor { get; private set; }
		protected GridViewCommandsBase(GridViewBase view)
			: base(view) {
			ChangeGroupExpanded = CreateDelegateCommand(o => view.ChangeGroupExpanded(o));
			ExpandAllGroups = CreateDelegateCommand(o => view.ExpandAllGroups(o), o => view.CanExpandCollapseAll(o));
			CollapseAllGroups = CreateDelegateCommand(o => view.CollapseAllGroups(o), o => view.CanExpandCollapseAll(o));
			MoveParentGroupRow = CreateDelegateCommand(o => view.MoveParentGroupRow(), o => view.CanMoveGroupParentRow());
			ClearGrouping = CreateDelegateCommand(o => view.ClearGrouping(), o => view.CanClearGrouping());
			ShowGroupSummaryEditor = CreateDelegateCommand(o => view.ShowGroupSummaryEditor(), o => view.CanShowGroupSummaryEditor());
		}
	}
	public class TableViewCommands : GridViewCommandsBase, IConditionalFormattingCommands {
#if !SL
	[DevExpressXpfGridLocalizedDescription("TableViewCommandsBestFitColumn")]
#endif
		public ICommand BestFitColumn { get; private set; }
#if !SL
	[DevExpressXpfGridLocalizedDescription("TableViewCommandsBestFitColumns")]
#endif
		public ICommand BestFitColumns { get; private set; }
		public new ICommand ChangeMasterRowExpanded { get { return base.ChangeMasterRowExpanded; } }
		public new ICommand ExpandMasterRow { get { return base.ExpandMasterRow; } }
		public new ICommand CollapseMasterRow { get { return base.CollapseMasterRow; } }
		public ICommand ShowNewItemRow { get; private set; }
		public ICommand ShowLessThanFormatConditionDialog { get; private set; }
		public ICommand ShowGreaterThanFormatConditionDialog { get; private set; }
		public ICommand ShowEqualToFormatConditionDialog { get; private set; }
		public ICommand ShowBetweenFormatConditionDialog { get; private set; }
		public ICommand ShowTextThatContainsFormatConditionDialog { get; private set; }
		public ICommand ShowADateOccurringFormatConditionDialog { get; private set; }
		public ICommand ShowCustomConditionFormatConditionDialog { get; private set; }
		public ICommand ShowTop10ItemsFormatConditionDialog { get; private set; }
		public ICommand ShowBottom10ItemsFormatConditionDialog { get; private set; }
		public ICommand ShowTop10PercentFormatConditionDialog { get; private set; }
		public ICommand ShowBottom10PercentFormatConditionDialog { get; private set; }
		public ICommand ShowAboveAverageFormatConditionDialog { get; private set; }
		public ICommand ShowBelowAverageFormatConditionDialog { get; private set; }
		public ICommand ClearFormatConditionsFromAllColumns { get; private set; }
		public ICommand ClearFormatConditionsFromColumn { get; private set; }
		public ICommand ShowConditionalFormattingManager { get; private set; }
		public ICommand AddFormatCondition { get; private set; }
		public ICommand ToggleRowsSelection { get; private set; }
		public ICommand ShowEditForm { get; private set; }
		public ICommand HideEditForm { get; private set; }
		public ICommand CloseEditForm { get; private set; }
		readonly TableView tableView;
		public TableViewCommands(TableView view) : base(view) {
			this.tableView = view;
			BestFitColumn = CreateDelegateCommand(o => view.TableViewBehavior.BestFitColumn(o), o => view.CanBestFitColumn(o));
			BestFitColumns = CreateDelegateCommand(o => view.TableViewBehavior.BestFitColumns(), o => view.TableViewBehavior.CanBestFitColumns());
			ShowNewItemRow = new DelegateCommand<NewItemRowPosition?>(view.ShowNewItemRow);
			ShowLessThanFormatConditionDialog = CreateDelegateCommand(x => ShowFormatConditionDialog(x, FormatConditionDialogType.LessThan));
			ShowGreaterThanFormatConditionDialog = CreateDelegateCommand(x => ShowFormatConditionDialog(x, FormatConditionDialogType.GreaterThan));
			ShowEqualToFormatConditionDialog = CreateDelegateCommand(x => ShowFormatConditionDialog(x, FormatConditionDialogType.EqualTo));
			ShowBetweenFormatConditionDialog = CreateDelegateCommand(x => ShowFormatConditionDialog(x, FormatConditionDialogType.Between));
			ShowTextThatContainsFormatConditionDialog = CreateDelegateCommand(x => ShowFormatConditionDialog(x, FormatConditionDialogType.TextThatContains));
			ShowADateOccurringFormatConditionDialog = CreateDelegateCommand(x => ShowFormatConditionDialog(x, FormatConditionDialogType.ADateOccurring));
			ShowCustomConditionFormatConditionDialog = CreateDelegateCommand(x => ShowFormatConditionDialog(x, FormatConditionDialogType.CustomCondition));
			ShowTop10ItemsFormatConditionDialog = CreateDelegateCommand(x => ShowFormatConditionDialog(x, FormatConditionDialogType.Top10Items));
			ShowBottom10ItemsFormatConditionDialog = CreateDelegateCommand(x => ShowFormatConditionDialog(x, FormatConditionDialogType.Bottom10Items));
			ShowTop10PercentFormatConditionDialog = CreateDelegateCommand(x => ShowFormatConditionDialog(x, FormatConditionDialogType.Top10Percent));
			ShowBottom10PercentFormatConditionDialog = CreateDelegateCommand(x => ShowFormatConditionDialog(x, FormatConditionDialogType.Bottom10Percent));
			ShowAboveAverageFormatConditionDialog = CreateDelegateCommand(x => ShowFormatConditionDialog(x, FormatConditionDialogType.AboveAverage));
			ShowBelowAverageFormatConditionDialog = CreateDelegateCommand(x => ShowFormatConditionDialog(x, FormatConditionDialogType.BelowAverage));
			ClearFormatConditionsFromAllColumns = CreateDelegateCommand(x => view.ClearFormatConditionsFromAllColumns());
			ClearFormatConditionsFromColumn = CreateDelegateCommand(x => view.ClearFormatConditionsFromColumn((ColumnBase)x));
			ShowConditionalFormattingManager = CreateDelegateCommand(x => view.ShowConditionalFormattingManager((ColumnBase)x));
			AddFormatCondition = CreateDelegateCommand(x => view.AddFormatCondition((FormatConditionBase)x));
			ToggleRowsSelection = CreateDelegateCommand(o => view.ToggleRowsSelection());
			ShowEditForm = CreateDelegateCommand(o => view.ShowEditForm());
			HideEditForm = CreateDelegateCommand(o => view.HideEditForm());
			CloseEditForm = CreateDelegateCommand(o => view.CloseEditForm());
		}
		void ShowFormatConditionDialog(object column, FormatConditionDialogType dialogKind) {
			tableView.ShowFormatConditionDialog((ColumnBase)column, dialogKind);
		}
	}
	public class CardViewCommands : GridViewCommandsBase {
		public CardViewCommands(CardView view)
			: base(view) {
		}
	}
}
