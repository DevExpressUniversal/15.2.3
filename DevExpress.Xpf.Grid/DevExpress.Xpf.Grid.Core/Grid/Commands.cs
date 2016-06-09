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
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using System.Collections.Generic;
using DevExpress.Xpf.Printing;
using System.Windows;
using DevExpress.Xpf.Grid.Printing;
#if SL
using RoutedCommand = DevExpress.Xpf.Core.WPFCompatibility.RoutedCommand;
#endif
namespace DevExpress.Xpf.Grid {
	internal static class DataControlCommands {
#if !SL
		internal static readonly RoutedCommand changeColumnSortOrder = new RoutedCommand("ChangeColumnSortOrder", typeof(DataControlCommands));
		internal static readonly RoutedCommand clearColumnFilter = new RoutedCommand("ClearColumnFilter", typeof(DataControlCommands));
		internal static readonly RoutedCommand showFilterEditor = new RoutedCommand("ShowFilterEditor", typeof(DataControlCommands));
		internal static readonly RoutedCommand showColumnChooser = new RoutedCommand("ShowColumnChooser", typeof(DataControlCommands));
		internal static readonly RoutedCommand hideColumnChooser = new RoutedCommand("HideColumnChooser", typeof(DataControlCommands));
		internal static readonly RoutedCommand movePrevCell = new RoutedCommand("MovePrevCell", typeof(DataControlCommands));
		internal static readonly RoutedCommand moveNextCell = new RoutedCommand("MoveNextCell", typeof(DataControlCommands));
		internal static readonly RoutedCommand movePrevRow = new RoutedCommand("MovePrevRow", typeof(DataControlCommands));
		internal static readonly RoutedCommand moveNextRow = new RoutedCommand("MoveNextRow", typeof(DataControlCommands));
		internal static readonly RoutedCommand moveFirstRow = new RoutedCommand("MoveFirstRow", typeof(DataControlCommands));
		internal static readonly RoutedCommand moveLastRow = new RoutedCommand("MoveLastRow", typeof(DataControlCommands));
		internal static readonly RoutedCommand movePrevPage = new RoutedCommand("MovePrevPage", typeof(DataControlCommands));
		internal static readonly RoutedCommand moveNextPage = new RoutedCommand("MoveNextPage", typeof(DataControlCommands));
		internal static readonly RoutedCommand moveFirstCell = new RoutedCommand("MoveFirstCell", typeof(DataControlCommands));
		internal static readonly RoutedCommand moveLastCell = new RoutedCommand("MoveLastCell", typeof(DataControlCommands));
		internal static readonly RoutedCommand clearFilter = new RoutedCommand("ClearFilter", typeof(DataControlCommands));
		internal static readonly RoutedCommand deleteFocusedRow = new RoutedCommand("DeleteFocusedRow", typeof(DataControlCommands));
		internal static readonly RoutedCommand editFocusedRow = new RoutedCommand("EditFocusedRow", typeof(DataControlCommands));
		internal static readonly RoutedCommand cancelEditFocusedRow = new RoutedCommand("CancelEditFocusedRow", typeof(DataControlCommands));
		internal static readonly RoutedCommand endEditFocusedRow = new RoutedCommand("EndEditFocusedRow", typeof(DataControlCommands));
		internal static readonly RoutedCommand showUnboundExpressionEditor = new RoutedCommand("ShowUnboundExpressionEditor", typeof(DataControlCommands));
		public static RoutedCommand ChangeColumnSortOrder { get { return changeColumnSortOrder; } }
		public static RoutedCommand ClearColumnFilter { get { return clearColumnFilter; } }
		public static RoutedCommand ShowFilterEditor { get { return showFilterEditor; } }
		public static RoutedCommand ShowColumnChooser { get { return showColumnChooser; } }
		public static RoutedCommand HideColumnChooser { get { return hideColumnChooser; } }
		public static RoutedCommand MovePrevCell { get { return movePrevCell; } }
		public static RoutedCommand MoveNextCell { get { return moveNextCell; } }
		public static RoutedCommand MovePrevRow { get { return movePrevRow; } }
		public static RoutedCommand MoveNextRow { get { return moveNextRow; } }
		public static RoutedCommand MoveFirstRow { get { return moveFirstRow; } }
		public static RoutedCommand MoveLastRow { get { return moveLastRow; } }
		public static RoutedCommand MovePrevPage { get { return movePrevPage; } }
		public static RoutedCommand MoveNextPage { get { return moveNextPage; } }
		public static RoutedCommand MoveFirstCell { get { return moveFirstCell; } }
		public static RoutedCommand MoveLastCell { get { return moveLastCell; } }
		public static RoutedCommand ClearFilter { get { return clearFilter; } }
		public static RoutedCommand DeleteFocusedRow { get { return deleteFocusedRow; } }
		public static RoutedCommand EditFocusedRow { get { return editFocusedRow; } }
		public static RoutedCommand CancelEditFocusedRow { get { return cancelEditFocusedRow; } }
		public static RoutedCommand EndEditFocusedRow { get { return endEditFocusedRow; } }
		public static RoutedCommand ShowUnboundExpressionEditor { get { return showUnboundExpressionEditor; } }
#endif
	}
	public class GridColumnCommands : INotifyPropertyChanged {
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("GridColumnCommandsChangeColumnSortOrder")]
#endif
public ICommand ChangeColumnSortOrder { get; private set; }
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("GridColumnCommandsClearColumnFilter")]
#endif
		public ICommand ClearColumnFilter { get; private set; }
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Instead use the ClearColumnFilter property.")]
		public ICommand ClearFilterColumn { get { return ClearColumnFilter; } }
		public GridColumnCommands(ColumnBase column) {
			ChangeColumnSortOrder = DelegateCommandFactory.Create<object>(o => column.Owner.ChangeColumnSortOrder(column), false);
			ClearColumnFilter = DelegateCommandFactory.Create<object>(o => column.Owner.ClearColumnFilter(column), o => column.Owner.CanClearColumnFilter(column), false);
		}
		#region INotifyPropertyChanged Members
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged { add { } remove { } } 
		#endregion
	}
	public abstract class DataViewCommandsBase : INotifyPropertyChanged {
		List<IDelegateCommand> commands = new List<IDelegateCommand>();
		public ICommand ShowFilterEditor { get; private set; }
		public ICommand ShowColumnChooser { get; private set; }
		public ICommand HideColumnChooser { get; private set; }
		public ICommand MovePrevCell { get; private set; }
		public ICommand MoveNextCell { get; private set; }
		public ICommand MovePrevRow { get; private set; }
		public ICommand MoveNextRow { get; private set; }
		public ICommand MoveFirstRow { get; private set; }
		public ICommand MoveLastRow { get; private set; }
		public ICommand MovePrevPage { get; private set; }
		public ICommand MoveNextPage { get; private set; }
		public ICommand MoveFirstCell { get; private set; }
		public ICommand MoveLastCell { get; private set; }
		public ICommand ClearFilter { get; private set; }
		public ICommand ChangeColumnsSortOrder { get; private set; }
		public ICommand DeleteFocusedRow { get; private set; }
		public ICommand EditFocusedRow { get; private set; }
		public ICommand CancelEditFocusedRow { get; private set; }
		public ICommand EndEditFocusedRow { get; private set; }
		public ICommand ShowUnboundExpressionEditor { get; private set; }
		internal ICommand ChangeMasterRowExpanded { get; private set; }
		internal ICommand ExpandMasterRow { get; private set; }
		internal ICommand CollapseMasterRow { get; private set; }
		public ICommand ShowPrintPreviewDialog { get; private set; }
		public ICommand ShowPrintPreview { get; private set; }
		public ICommand ShowTotalSummaryEditor { get; private set; }
		ICommand showSearchPanel;
		public ICommand ShowSearchPanel {
			get { return showSearchPanel; }
			private set {
				showSearchPanel = value;
			}
		}
		ICommand hideSearchPanel;
		public ICommand HideSearchPanel {
			get { return hideSearchPanel; }
			private set {
				hideSearchPanel = value;
			}
		}
		readonly DataViewBase view;
		protected DataViewCommandsBase(DataViewBase view) {
			this.view = view;
			ShowFilterEditor = CreateDelegateCommand(o => view.ShowFilterEditor(o), o => view.CanShowFilterEditor(o));
			ShowColumnChooser = CreateDelegateCommand(o => view.ShowColumnChooser(), o => view.CanShowColumnChooser());
			HideColumnChooser = CreateDelegateCommand(o => view.HideColumnChooser(), o => view.CanHideColumnChooser());
			MovePrevCell = CreateDelegateCommand(o => view.MovePrevCell(), o => view.CanMovePrevCell());
			MoveNextCell = CreateDelegateCommand(o => view.MoveNextCell(), o => view.CanMoveNextCell());
			MovePrevRow = CreateDelegateCommand(o => view.MasterRootRowsContainer.FocusedView.Navigation.OnUp(false), o => view.MasterRootRowsContainer.FocusedView.CanPrevRow());
			MoveNextRow = CreateDelegateCommand(o => view.MasterRootRowsContainer.FocusedView.Navigation.OnDown(), o => view.MasterRootRowsContainer.FocusedView.CanNextRow());
			MoveFirstRow = CreateDelegateCommand(o => view.MoveFirstRow(), o => view.MasterRootRowsContainer.FocusedView.CanPrevRow());
			MoveLastRow = CreateDelegateCommand(o => view.MasterRootRowsContainer.FocusedView.MoveLastOrLastMasterRow(), o => view.MasterRootRowsContainer.FocusedView.CanNextRow());
			MovePrevPage = CreateDelegateCommand(o => view.MasterRootRowsContainer.FocusedView.Navigation.OnPageUp(), o => view.MasterRootRowsContainer.FocusedView.CanPrevRow());
			MoveNextPage = CreateDelegateCommand(o => view.MasterRootRowsContainer.FocusedView.Navigation.OnPageDown(), o => view.MasterRootRowsContainer.FocusedView.CanNextRow());
			MoveFirstCell = CreateDelegateCommand(o => view.MoveFirstCell(), o => view.CanMoveFirstCell());
			MoveLastCell = CreateDelegateCommand(o => view.MoveLastCell(), o => view.CanMoveLastCell());
			ClearFilter = CreateDelegateCommand(o => view.ClearFilter(), o => view.CanClearFilter());
			ChangeColumnsSortOrder = CreateDelegateCommand<ChangeColumnsSortOrderMode?>(o => view.ChangeColumnsSortOrder(o.GetValueOrDefault(ChangeColumnsSortOrderMode.SortedColumns)), o => true);
			DeleteFocusedRow = CreateDelegateCommand(o => view.MasterRootRowsContainer.FocusedView.DeleteFocusedRow(), o => view.MasterRootRowsContainer.FocusedView.CanDeleteFocusedRow());
			EditFocusedRow = CreateDelegateCommand(o => view.MasterRootRowsContainer.FocusedView.EditFocusedRow(), o => view.MasterRootRowsContainer.FocusedView.CanEditFocusedRow());
			CancelEditFocusedRow = CreateDelegateCommand(o => view.CancelEditFocusedRow(), o => view.CanCancelEditFocusedRow());
			EndEditFocusedRow = CreateDelegateCommand(o => view.EndEditFocusedRow(), o => view.CanEndEditFocusedRow());
			ShowUnboundExpressionEditor = CreateDelegateCommand(o => view.ShowUnboundExpressionEditor(o), o => view.CanShowUnboundExpressionEditor(o));
			ShowTotalSummaryEditor = CreateDelegateCommand(o => view.ShowTotalSummaryEditor(o), o => view.CanShowTotalSummaryEditor(o));
			ChangeMasterRowExpanded = CreateDelegateCommand(o => view.DataControl.ChangeMasterRowExpanded(o));
			ExpandMasterRow = CreateDelegateCommand(o => view.DataControl.SetMasterRowExpanded(o, true));
			CollapseMasterRow = CreateDelegateCommand(o => view.DataControl.SetMasterRowExpanded(o, false));
			ShowSearchPanel = CreateDelegateCommand(o => view.ShowSearchPanel(view.ConvertCommandParameterToBool(o)));
			HideSearchPanel = CreateDelegateCommand(o => view.HideSearchPanel());
			ShowPrintPreview = CreateDelegateCommand(o => PrintHelper.ShowPrintPreview(Window.GetWindow(view), (IPrintableControl)view));
			ShowPrintPreviewDialog = CreateDelegateCommand(o => PrintHelper.ShowPrintPreviewDialog(Window.GetWindow(view), (IPrintableControl)view));
		}
		protected DelegateCommand<object> CreateDelegateCommand(Action<object> executeMethod) {
			return CreateDelegateCommand(executeMethod, null);
		}
		protected DelegateCommand<object> CreateDelegateCommand(Action<object> executeMethod, Func<object, bool> canExecuteMethod) {
			return CreateDelegateCommand<object>(executeMethod, canExecuteMethod);
		}
		protected DelegateCommand<T> CreateDelegateCommand<T>(Action<T> executeMethod, Func<T, bool> canExecuteMethod) {
			DelegateCommand<T> command = DelegateCommandFactory.Create<T>(executeMethod, canExecuteMethod, false);
			commands.Add(command);
			return command;
		}
		internal void RaiseCanExecutedChanged() {
			commands.ForEach(command => command.RaiseCanExecuteChanged());
		}
		#region INotifyPropertyChanged Members
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged { add { } remove { } } 
		#endregion
	}
}
