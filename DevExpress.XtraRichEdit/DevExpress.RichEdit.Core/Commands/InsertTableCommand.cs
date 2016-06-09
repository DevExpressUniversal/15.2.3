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
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Layout;
using System.ComponentModel;
using DevExpress.Office;
using DevExpress.Office.History;
namespace DevExpress.XtraRichEdit.Commands {
	#region IInsertTableCommand
	public interface IInsertTableCommand {
		void Execute();
		bool CanExecute();
		void InsertTable(int rows, int columns);
	}
	#endregion
	#region InsertTableCommand
	public class InsertTableCommand : TransactedInsertObjectCommand, IInsertTableCommand {
		public InsertTableCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("InsertTableCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.InsertTable; } }
		protected internal override RichEditCommand CreateInsertObjectCommand() {
			return new InsertTableCoreCommand(Control);
		}
		public override void ForceExecute(ICommandUIState state) {
			CheckExecutedAtUIThread();
			NotifyBeginCommandExecution(state);
			try {
				IValueBasedCommandUIState<CreateTableParameters> valueBasedState = state as IValueBasedCommandUIState<CreateTableParameters>;
				valueBasedState.Value = new CreateTableParameters(2, 5);
				ShowInsertTableForm(valueBasedState.Value, ShowInsertTableFormCallback, state);
			}
			finally {
				NotifyEndCommandExecution(state);
			}
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			return new DefaultValueBasedCommandUIState<CreateTableParameters>();
		}
		internal virtual void ShowInsertTableForm(CreateTableParameters parameters, ShowInsertTableFormCallback callback, object callbackData) {
			Control.ShowInsertTableForm(parameters, callback, callbackData);
		}
		public virtual void InsertTable(int rows, int columns) {
			Control.BeginUpdate();
			try {
				using (HistoryTransaction transaction = new HistoryTransaction(DocumentModel.History)) {
					DefaultValueBasedCommandUIState<CreateTableParameters> state = new DefaultValueBasedCommandUIState<CreateTableParameters>();
					state.Value = new CreateTableParameters(rows, columns);
					ForceExecuteCore(state);
				}
			}
			finally {
				Control.EndUpdate();
			}
		}
		protected internal override void ForceExecuteCore(ICommandUIState state) {
			IValueBasedCommandUIState<CreateTableParameters> valueBasedState = state as IValueBasedCommandUIState<CreateTableParameters>;
			CreateTableParameters parameters = valueBasedState.Value;
			if (parameters == null)
				return;
			InsertTableCoreCommand insertCommand = (InsertTableCoreCommand)Commands[1];
			insertCommand.RowCount = parameters.RowCount;
			insertCommand.ColumnCount = parameters.ColumnCount;
			base.ForceExecuteCore(state);
		}
		protected internal virtual void ShowInsertTableFormCallback(CreateTableParameters parameters, object callbackData) {
			Control.BeginUpdate();
			try {
				using (HistoryTransaction transaction = new HistoryTransaction(DocumentModel.History)) {
					IValueBasedCommandUIState<CreateTableParameters> valueBasedState = callbackData as IValueBasedCommandUIState<CreateTableParameters>;
					valueBasedState.Value = parameters;
					base.ForceExecute(valueBasedState);
				}
			}
			finally {
				Control.EndUpdate();
			}
		}
		public override void UpdateUIState(ICommandUIState state) {
			base.UpdateUIState(state);
			ApplyCommandRestrictionOnEditableControl(state, Options.DocumentCapabilities.Tables, state.Enabled);
			ApplyDocumentProtectionToSelectedCharacters(state);
		}
	}
	#endregion
	#region CreateTableParameters
	public class CreateTableParameters {
		int rowCount;
		int columnCount;
		public CreateTableParameters(int rowCount, int columnCount) {
			this.rowCount = rowCount;
			this.columnCount = columnCount;
		}
		public int RowCount { get { return rowCount; } set { rowCount = value; } }
		public int ColumnCount { get { return columnCount; } set { columnCount = value; } }
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Commands.Internal {
	#region InsertTableCoreCommand
	public class InsertTableCoreCommand : InsertObjectCommandBase {
		int rowCount;
		int columnCount;
		public InsertTableCoreCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertTable; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertTableDescription; } }
		public int RowCount { get { return rowCount; } set { rowCount = value; } }
		public int ColumnCount { get { return columnCount; } set { columnCount = value; } }
		public override string ImageName { get { return "InsertTable"; } }
		#endregion
		protected internal override void ModifyModel() {
			if (RowCount <= 0 || ColumnCount <= 0)
				return;
			InsertTable();
		}
		void InsertTable() {
			InsertTableInnerCommand command = (InsertTableInnerCommand)Control.CreateCommand(RichEditCommandId.InsertTableInner);
			command.StartPosition = DocumentModel.Selection.End;
			command.RowCount = RowCount;
			command.ColumnCount = ColumnCount;
			command.ColumnWidth = GetColumnWidth();
			command.Execute();
		}
		protected internal int GetColumnWidth() {
			ActiveView.EnsureFormattingCompleteForSelection();
			CaretPosition caretPosition = ActiveView.CaretPosition;
			DocumentModelUnitToLayoutUnitConverter unitConverter = DocumentModel.ToDocumentLayoutUnitConverter;
			if (caretPosition.Update(DocumentLayoutDetailsLevel.TableCell)) {
				DocumentLayoutPosition layoutPosition = CaretPosition.LayoutPosition;
				int width = layoutPosition.TableCell == null ? layoutPosition.Column.Bounds.Width : layoutPosition.TableCell.TextWidth;
				if (ActiveView.MatchHorizontalTableIndentsToTextEdge)
					width = Math.Max(width - 1, 1);
				return unitConverter.ToModelUnits(width);
			}
			return 0;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			ApplyCommandsRestriction(state, Options.DocumentCapabilities.Tables, state.Enabled);
		}
	}
	#endregion
	#region InsertTableInnerCommand
	public class InsertTableInnerCommand : RichEditMenuItemSimpleCommand {
		public InsertTableInnerCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override RichEditCommandId Id { get { return RichEditCommandId.InsertTableInner; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		public virtual DocumentLogPosition StartPosition { get; set; }
		public virtual int RowCount { get; set; }
		public virtual int ColumnCount { get; set; }
		public virtual int ColumnWidth { get; set; }
		#endregion
		protected internal override void ExecuteCore() {
			ActivePieceTable.InsertTable(StartPosition, RowCount, ColumnCount, TableAutoFitBehaviorType.FixedColumnWidth, Int32.MinValue, ColumnWidth, GetForceVisible(), ActiveView.MatchHorizontalTableIndentsToTextEdge);
			Table lastTable = ActivePieceTable.Tables.Last;
			int styleIndex = DocumentModel.TableStyles.GetStyleIndexByName(TableStyleCollection.TableSimpleStyleName);
			if (styleIndex >= 0)
				lastTable.StyleIndex = styleIndex;
			JoinTables(lastTable);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = IsContentEditable;
			state.Visible = true;
		}
		protected internal void JoinTables(Table table) {
			ParagraphIndex firstParagraphInTableIndex = table.FirstRow.FirstCell.StartParagraphIndex;
			if (firstParagraphInTableIndex == ParagraphIndex.Zero)
				return;
			Paragraph previousParagraph = ActivePieceTable.Paragraphs[firstParagraphInTableIndex - 1];
			TableCell cell = previousParagraph.GetCell();
			if (cell == null || table.NestedLevel != cell.Table.NestedLevel)
				return;
			ActivePieceTable.JoinTables(cell.Table, table);
		}
	}
	#endregion
}
