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
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Utils;
using System.ComponentModel;
using DevExpress.XtraRichEdit.Tables.Native;
using DevExpress.XtraRichEdit.Model.History;
namespace DevExpress.XtraRichEdit.Commands {
	#region InsertTableElementCommandBase
	public abstract class InsertTableElementCommandBase : RichEditCaretBasedCommand {
		protected InsertTableElementCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = DocumentModel.Selection.IsWholeSelectionInOneTable();
			ApplyCommandRestrictionOnEditableControl(state, DocumentModel.DocumentCapabilities.Tables, state.Enabled);
			ApplyDocumentProtectionToSelectedParagraphs(state);
		}
	}
	#endregion
	#region InsertTableElementMenuCommand
	public class InsertTableElementMenuCommand : InsertTableElementCommandBase {
		public InsertTableElementMenuCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("InsertTableElementMenuCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertTableElement; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("InsertTableElementMenuCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertTableElement; } }
		protected internal override void ExecuteCore() {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			state.Visible = state.Enabled;	
		}
	}
	#endregion
	#region InsertTableRowCommandBase
	public abstract class InsertTableRowCommandBase : InsertTableElementCommandBase {
		InsertTableRowCoreCommandBase insertRowCommand;
		protected InsertTableRowCommandBase(IRichEditControl control)
			: base(control) {
			this.insertRowCommand = CreateInsertTableRowCommand(Control);
		}
		protected internal virtual InsertTableRowCoreCommandBase InsertRowCommand { get { return insertRowCommand; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return InsertRowCommand.MenuCaptionStringId; } }
		public override XtraRichEditStringId DescriptionStringId { get { return InsertRowCommand.DescriptionStringId; } }
		public override string ImageName { get { return InsertRowCommand.ImageName; } }
		protected abstract InsertTableRowCoreCommandBase CreateInsertTableRowCommand(IRichEditControl control);
		protected internal override void ExecuteCore() {
			TableRow patternRow = FindPatternRow();
			if (patternRow == null)
				return;
			InsertRowCommand.Row = patternRow;
			InsertRowCommand.PerformModifyModel();
		}
		protected internal virtual TableRow FindPatternRow() {
			SelectionRangeCollection selections = DocumentModel.Selection.GetSortedSelectionCollection();
			ParagraphIndex paragraphIndex = FindPatternRowParagraphIndex(selections);
			TableCell lastCell = ActivePieceTable.Paragraphs[paragraphIndex].GetCell();
			return lastCell == null ? null : lastCell.Row;
		}
		protected internal abstract ParagraphIndex FindPatternRowParagraphIndex(SelectionRangeCollection selections);
	}
	#endregion
	#region InsertTableRowBelowCommand
	public class InsertTableRowBelowCommand : InsertTableRowCommandBase {
		public InsertTableRowBelowCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("InsertTableRowBelowCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.InsertTableRowBelow; } }
		protected override InsertTableRowCoreCommandBase CreateInsertTableRowCommand(IRichEditControl control) {
			return new InsertTableRowBelowCoreCommand(control);
		}
		protected internal override ParagraphIndex FindPatternRowParagraphIndex(SelectionRangeCollection selections) {
			ParagraphIndex endParagraphIndex = ActivePieceTable.FindParagraphIndex(selections.Last.End);
			if (selections.Last.Length > 0 && selections.Last.End == ActivePieceTable.Paragraphs[endParagraphIndex].LogPosition)
				endParagraphIndex--;
			return endParagraphIndex;
		}
	}
	#endregion
	#region InsertTableRowAboveCommand
	public class InsertTableRowAboveCommand : InsertTableRowCommandBase {
		public InsertTableRowAboveCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("InsertTableRowAboveCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.InsertTableRowAbove; } }
		protected override InsertTableRowCoreCommandBase CreateInsertTableRowCommand(IRichEditControl control) {
			return new InsertTableRowAboveCoreCommand(control);
		}
		protected internal override ParagraphIndex FindPatternRowParagraphIndex(SelectionRangeCollection selections) {
			return ActivePieceTable.FindParagraphIndex(selections.First.Start);
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Commands.Internal {
	#region InsertTableRowCoreCommandBase (abstract)
	public abstract class InsertTableRowCoreCommandBase : InsertObjectCommandBase {
		TableRow row;
		protected InsertTableRowCoreCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected internal TableRow Row { get { return row; } set { row = value; } }
		protected internal override bool ExtendSelection { get { return false; } }
		protected internal abstract TableRow InsertRow(TableRow row);
		protected internal override void ModifyModel() {
			if (Row != null)
				ChangeSelection(InsertRow(Row));
#if DEBUGTEST
			DocumentModel.ActivePieceTable.ForceCheckTablesIntegrity();
#endif
		}
		protected internal virtual void ChangeSelection(TableRow rowToSelect) {
			SelectTableRowHistoryItem item = new SelectTableRowHistoryItem(DocumentModel.ActivePieceTable);
			item.Control = Control;
			item.TableIndex = rowToSelect.Table.Index;
			int rowIndex = rowToSelect.IndexInTable;
			item.StartRowIndex = rowIndex;
			item.EndRowIndex = rowIndex;
			DocumentModel.History.Add(item);
			item.Execute();
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			ApplyCommandRestrictionOnEditableControl(state, Options.DocumentCapabilities.Tables, state.Enabled);
			ApplyDocumentProtectionToSelectedParagraphs(state);
		}
	}
	#endregion
	#region InsertTableRowBelowCoreCommand
	public class InsertTableRowBelowCoreCommand : InsertTableRowCoreCommandBase {
		public InsertTableRowBelowCoreCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertTableRowBelow; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertTableRowBelowDescription; } }
		public override string ImageName { get { return "InsertTableRowsBelow"; } }
		#endregion
		protected internal override TableRow InsertRow(TableRow row) {
			ActivePieceTable.InsertTableRowBelow(row, GetForceVisible());
			int patternRowIndex = row.Table.Rows.IndexOf(row);
			return row.Table.Rows[patternRowIndex + 1];
		}
		protected internal override DocumentLogPosition ChangePosition(DocumentModelPosition pos) {
			TableRowCollection rows = Row.Table.Rows;
			int newRowIndex = rows.IndexOf(Row) + 1;
			if (newRowIndex == rows.Count)
				return base.ChangePosition(pos);
			ParagraphIndex start = rows[newRowIndex].FirstCell.StartParagraphIndex;
			return ActivePieceTable.Paragraphs[start].LogPosition;
		}
	}
	#endregion
	#region InsertTableRowAboveCoreCommand
	public class InsertTableRowAboveCoreCommand : InsertTableRowCoreCommandBase {
		public InsertTableRowAboveCoreCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertTableRowAbove; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertTableRowAboveDescription; } }
		public override string ImageName { get { return "InsertTableRowsAbove"; } }
		#endregion
		protected internal override TableRow InsertRow(TableRow row) {
			ActivePieceTable.InsertTableRowAbove(row, GetForceVisible());
			TableRowCollection rows = row.Table.Rows;
			int patternRowIndex = rows.IndexOf(row);
			if (patternRowIndex == 0)
				return null;
			return rows[patternRowIndex - 1];
		}
		protected internal override DocumentLogPosition ChangePosition(DocumentModelPosition pos) {
			return ActivePieceTable.Paragraphs[Row.LastCell.EndParagraphIndex].LogPosition + 1;
		}
	}
	#endregion
}
