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
using DevExpress.XtraRichEdit.Localization;
using DevExpress.Utils.Commands;
using System.ComponentModel;
using DevExpress.Office.History;
namespace DevExpress.XtraRichEdit.Commands {
	#region ShowInsertDeleteTableCellsFormCommandBase (abstract class)
	public abstract class ShowInsertDeleteTableCellsFormCommandBase : RichEditCommand {
		#region Fields
		InsertDeleteTableCellsDispatcherCommandBase insertDeleteTableCellscommand;
		#endregion
		protected ShowInsertDeleteTableCellsFormCommandBase(IRichEditControl control)
			: base(control) {
			this.insertDeleteTableCellscommand = CreateTableCellsCommand();
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return InsertDeleteTableCellscommand.MenuCaptionStringId; } }
		public override XtraRichEditStringId DescriptionStringId { get { return InsertDeleteTableCellscommand.DescriptionStringId; } }
		public override string ImageName { get { return InsertDeleteTableCellscommand.ImageName; } }
		public override bool ShowsModalDialog { get { return true; } }
		protected internal InsertDeleteTableCellsDispatcherCommandBase InsertDeleteTableCellscommand { get { return insertDeleteTableCellscommand; } }
		#endregion
		protected internal abstract InsertDeleteTableCellsDispatcherCommandBase CreateTableCellsCommand();
		protected internal abstract TableCellOperation GetTableCellsOperation();
		protected internal abstract void ShowInsertDeleteTableCellsForm(TableCellsParameters parameters, ShowInsertDeleteTableCellsFormCallback callback, object callbackData);
		public override ICommandUIState CreateDefaultCommandUIState() {
			return new DefaultValueBasedCommandUIState<TableCellsParameters>();
		}
		public override void ForceExecute(ICommandUIState state) {
			CheckExecutedAtUIThread();
			NotifyBeginCommandExecution(state);
			try {
				IValueBasedCommandUIState<TableCellsParameters> valueBasedState = state as IValueBasedCommandUIState<TableCellsParameters>;
				valueBasedState.Value = new TableCellsParameters(GetTableCellsOperation());
				ShowInsertDeleteTableCellsForm(valueBasedState.Value, ShowInsertDeleteTableCellsFormCallback, state);
			}
			finally {
				NotifyEndCommandExecution(state);
			}
		}
		protected internal virtual void ShowInsertDeleteTableCellsFormCallback(TableCellsParameters parameters, object callbackData) {
			Control.BeginUpdate();
			try {
				using (HistoryTransaction transaction = new HistoryTransaction(DocumentModel.History)) {
					if (InsertDeleteTableCellscommand.CanExecute()) {
						DefaultValueBasedCommandUIState<TableCellsParameters> state = new DefaultValueBasedCommandUIState<TableCellsParameters>();
						state.Value = parameters;
						InsertDeleteTableCellscommand.ForceExecute(state);
					}
				}
			}
			finally {
				Control.EndUpdate();
			}
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = DocumentModel.Selection.IsWholeSelectionInOneTable();
			ApplyCommandRestrictionOnEditableControl(state, DocumentModel.DocumentCapabilities.Tables, state.Enabled);
			ApplyDocumentProtectionToSelectedParagraphs(state);
		}
	}
	#endregion
	#region ShowInsertTableCellsFormCommand
	public class ShowInsertTableCellsFormCommand : ShowInsertDeleteTableCellsFormCommandBase {
		public ShowInsertTableCellsFormCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowInsertTableCellsFormCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.ShowInsertTableCellsForm; } }
		#endregion
		protected internal override InsertDeleteTableCellsDispatcherCommandBase CreateTableCellsCommand() {
			return new InsertTableCellsDispatcherCommand(Control);
		}
		protected internal override TableCellOperation GetTableCellsOperation() {
			return TableCellOperation.ShiftToTheVertically;
		}
		protected internal override void ShowInsertDeleteTableCellsForm(TableCellsParameters parameters, ShowInsertDeleteTableCellsFormCallback callback, object callbackData) {
			Control.ShowInsertTableCellsForm(parameters, callback, callbackData);
		}
	}
	#endregion
	#region TableCellOperation
	public enum TableCellOperation {
		ShiftToTheHorizontally,
		ShiftToTheVertically,
		RowOperation,
		ColumnOperation
	}
	#endregion
	#region TableCellsParameters
	public class TableCellsParameters {
		TableCellOperation cellOperation;
		public TableCellsParameters(TableCellOperation cellOperation) {
			this.cellOperation = cellOperation;
		}
		public TableCellOperation CellOperation { get { return cellOperation; } set { cellOperation = value; } }
	}
	#endregion
}
