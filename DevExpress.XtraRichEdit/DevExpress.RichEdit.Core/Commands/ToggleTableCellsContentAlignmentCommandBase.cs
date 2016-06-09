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
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Tables.Native;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Localization;
namespace DevExpress.XtraRichEdit.Commands {
	#region ToggleTableCellsContentAlignmentCommandBase (abstract class)
	public abstract class ToggleTableCellsContentAlignmentCommandBase : ChangeTableCellsCommandBase {
		List<SelectionItem> cellsContentIntervals;
		protected ToggleTableCellsContentAlignmentCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected internal List<SelectionItem> CellsContentIntervals { get { return cellsContentIntervals; } }
		protected internal abstract ParagraphAlignment ParagraphAlignment { get; }
		protected internal abstract VerticalAlignment CellVerticalAlignment { get; }
		protected internal override void CollectCellsCore(SelectedCellsCollection cells, List<TableCell> target) {
			this.cellsContentIntervals = new List<SelectionItem>();
			base.CollectCellsCore(cells, target);
		}
		protected internal override void CollectCell(TableCell cell, SelectedTableCellPositionInfo cellPositionInfo, List<TableCell> target) {
			base.CollectCell(cell, cellPositionInfo, target);
			SelectionItem item = new SelectionItem(ActivePieceTable);
			item.BeginUpdate();
			try {
				Paragraph endParagraph = ActivePieceTable.Paragraphs[cell.EndParagraphIndex];
				item.Start = ActivePieceTable.Paragraphs[cell.StartParagraphIndex].LogPosition;
				item.End = endParagraph.LogPosition + endParagraph.Length;
			}
			finally {
				item.EndUpdate();
			}
			cellsContentIntervals.Add(item);
		}
		protected internal override void PerformModifyModelCore(ICommandUIState state) {
			base.PerformModifyModelCore(state);
			ToggleTableCellsParagraphAlignmentCommand command = new ToggleTableCellsParagraphAlignmentCommand(Control, this);
			ICommandUIState paragraphAlignmentState = command.CreateDefaultCommandUIState();
			command.ModifyDocumentModelCore(paragraphAlignmentState);
		}
		protected internal override void ModifyCell(TableCell tableCell) {
			tableCell.VerticalAlignment = CellVerticalAlignment;
		}
		protected internal override void UpdateUIStateCore(ICommandUIState state, ISelectedTableStructureBase selectedCellsCollection) {
			base.UpdateUIStateCore(state, selectedCellsCollection);
			if (!state.Enabled)
				return;
			List<TableCell> cells = new List<TableCell>();
			SelectedCellsCollection selectedCells = selectedCellsCollection as SelectedCellsCollection;
			if (selectedCells != null)
				CollectCellsCore(selectedCells, cells);
			int count = cells.Count;
			if (count <= 0) {
				state.Enabled = false;
				state.Checked = false;
				return;
			}
			VerticalAlignment verticalAlignment = cells[0].VerticalAlignment;
			for (int i = 1; i < count; i++) {
				if (cells[i].VerticalAlignment != verticalAlignment) {
					state.Checked = false;
					return;
				}
			}
			ToggleTableCellsParagraphAlignmentCommand command = new ToggleTableCellsParagraphAlignmentCommand(Control, this);
			ICommandUIState horizontalAlignmentState = command.CreateDefaultCommandUIState();
			command.UpdateUIState(horizontalAlignmentState);
			if (!horizontalAlignmentState.Checked) {
				state.Checked = false;
				return;
			}
			state.Checked = verticalAlignment == CellVerticalAlignment;
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Commands.Internal {
	#region ToggleTableCellsParagraphAlignmentCommand
	public class ToggleTableCellsParagraphAlignmentCommand : ToggleChangeParagraphFormattingCommandBase<ParagraphAlignment> {
		readonly ToggleTableCellsContentAlignmentCommandBase ownerCommand;
		public ToggleTableCellsParagraphAlignmentCommand(IRichEditControl control, ToggleTableCellsContentAlignmentCommandBase ownerCommand)
			: base(control) {
			Guard.ArgumentNotNull(ownerCommand, "ownerCommand");
			this.ownerCommand = ownerCommand;
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		protected internal override List<SelectionItem> GetSelectionItems() {
			return ownerCommand.CellsContentIntervals;
		}
		protected internal override ParagraphPropertyModifier<ParagraphAlignment> CreateModifier(ICommandUIState state) {
			ParagraphAlignment alignment;
			if (state.Checked)
				alignment = ParagraphAlignment.Left;
			else
				alignment = ownerCommand.ParagraphAlignment;
			return new ParagraphAlignmentModifier(alignment);
		}
		protected internal override bool IsCheckedValue(ParagraphAlignment value) {
			return value == ownerCommand.ParagraphAlignment;
		}
	}
	#endregion
	#region ChangeTableCellsContentAlignmentPlaceholderCommand
	public class ChangeTableCellsContentAlignmentPlaceholderCommand : ChangeTableCellsCommandBase, IPlaceholderCommand {
		public ChangeTableCellsContentAlignmentPlaceholderCommand(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ChangeTableCellsContentAlignment; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ChangeTableCellsContentAlignmentDescription; } }
		public override RichEditCommandId Id { get { return RichEditCommandId.ChangeTableCellsContentAlignmentPlaceholder; } }
		public override void ForceExecute(ICommandUIState state) {
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			return new DefaultCommandUIState();
		}
		protected internal override void ModifyCell(TableCell tableCell) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			state.Visible = state.Enabled;	
		}
	}
	#endregion
}
