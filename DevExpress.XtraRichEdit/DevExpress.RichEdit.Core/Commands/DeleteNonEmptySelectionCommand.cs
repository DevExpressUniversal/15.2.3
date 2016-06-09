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
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Tables.Native;
namespace DevExpress.XtraRichEdit.Commands.Internal {
	#region DeleteNonEmptySelectionCommand
	public class DeleteNonEmptySelectionCommand : DeleteCoreCommand {
		bool restoreInputPositionFormatting;
		InputPosition initialInputPosition;
		Table selectedTable;
		public DeleteNonEmptySelectionCommand(IRichEditControl control)
			: base(control) {
		}
		public override RichEditCommandId Id { get { return  RichEditCommandId.DeleteNonEmptySelection; } }
		protected internal bool RestoreInputPositionFormatting { get { return restoreInputPositionFormatting; } set { restoreInputPositionFormatting = value; } }
		bool AllowDeleteTableRows { get { return selectedTable != null; } }
		protected internal override void BeforeUpdate() {
			DocumentModel.BeginUpdate();
			base.BeforeUpdate();
			if (restoreInputPositionFormatting) {
				DocumentLogPosition position = Algorithms.Min(DocumentModel.Selection.NormalizedStart + 1, ActivePieceTable.DocumentEndLogPosition);
				this.initialInputPosition = CaretPosition.CreateInputPosition(position);
			}
			this.selectedTable = GetSelectedTable();
		}
		Table GetSelectedTable() {
			Selection selection = DocumentModel.Selection;
			SelectedCellsCollection selectedCells = selection.SelectedCells as SelectedCellsCollection;
			if (selectedCells == null || !selectedCells.IsNotEmpty)
				return null;
			TableCell firstCell = selectedCells.NormalizedFirst.StartCell;
			TableCell lastCell = selectedCells.NormalizedLast.EndCell;
			if (firstCell == null || lastCell == null || !Object.ReferenceEquals(firstCell.Table, lastCell.Table))
				return null;
			Table table = firstCell.Table;
			if (Object.ReferenceEquals(firstCell, table.FirstRow.FirstCell) && Object.ReferenceEquals(lastCell, table.LastRow.LastCell))
				return table;
			return null;
		}
		protected internal override void AfterUpdate() {
			base.AfterUpdate();
			if (this.selectedTable != null && this.selectedTable.Rows.Count == 0)
				ActivePieceTable.DeleteTableFromTableCollection(selectedTable);
			DocumentModel.EndUpdate();
			if (restoreInputPositionFormatting && initialInputPosition != null) {
				InputPosition inputPosition = CaretPosition.GetInputPosition();
				inputPosition.CopyFormattingFrom(initialInputPosition);
			}
		}
		protected internal override void DeleteContentCore(DocumentLogPosition selectionStart, int selectionLength, bool documentLastParagraphSelected) {
			DeleteContentCore(selectionStart, selectionLength, documentLastParagraphSelected, AllowDeleteTableRows);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = IsContentEditable && (DocumentModel.Selection.Length > 0);
			state.Visible = true;
			ApplyDocumentProtectionToSelectedCharacters(state);
		}
	}
	#endregion
	#region DeleteNonEmptySelectionCommandKeepSingleImage
	public class DeleteNonEmptySelectionCommandKeepSingleImage : DeleteNonEmptySelectionCommand {
		public DeleteNonEmptySelectionCommandKeepSingleImage(IRichEditControl control)
			: base(control) {
		}
		public override void UpdateUIState(ICommandUIState state) {
			base.UpdateUIState(state);
		}
		protected internal override void ExecuteCore() {
			if (IsSingleImageSelected()) {
				ShrinkSelectionToStartCommand command = new ShrinkSelectionToStartCommand(Control);
				ICommandUIState state = command.CreateDefaultCommandUIState();
				command.UpdateUIState(state);
				if(state.Enabled)
					command.ExecuteCore();
			}
			else
				base.ExecuteCore();
		}
		bool IsSingleImageSelected() {
			if (DocumentModel.Selection.Length != 1)
				return false;
			PieceTable pieceTable = DocumentModel.Selection.PieceTable;
			RunInfo result = new RunInfo(pieceTable);
			pieceTable.CalculateRunInfoStart(DocumentModel.Selection.Start, result);
			TextRunBase textRun = pieceTable.Runs[result.Start.RunIndex];
			if(textRun is InlinePictureRun)
				return true;
			FloatingObjectAnchorRun anchorRun = textRun as FloatingObjectAnchorRun;
			if (anchorRun != null && anchorRun.PictureContent != null)
				return true;
			else
				return false;
		}		
	}
	#endregion
}
