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
using DevExpress.XtraRichEdit;
using System.ComponentModel;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Tables.Native;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Layout.TableLayout;
namespace DevExpress.XtraRichEdit.Commands {
	#region DeleteTableColumnsCommand
	public class DeleteTableColumnsCommand : RichEditSelectionCommand {
		#region Fields
		DocumentLogPosition newLogPosition;
		bool isNotEmptySelectedCells;
		#endregion
		public DeleteTableColumnsCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("DeleteTableColumnsCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.DeleteTableColumns; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("DeleteTableColumnsCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_DeleteTableColumns; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("DeleteTableColumnsCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_DeleteTableColumnsDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("DeleteTableColumnsCommandImageName")]
#endif
		public override string ImageName { get { return "DeleteTableColumns"; } }
		protected internal override bool TryToKeepCaretX { get { return false; } }
		protected internal override bool ExtendSelection { get { return false; } }
		protected internal override DocumentLayoutDetailsLevel UpdateCaretPositionBeforeChangeSelectionDetailsLevel { get { return DocumentLayoutDetailsLevel.None; } }
		protected internal override bool TreatStartPositionAsCurrent { get { return false; } }
		#endregion
		protected override void UpdateUIStateCore(DevExpress.Utils.Commands.ICommandUIState state) {
			base.UpdateUIStateCore(state);
			state.Enabled = CanDeleteTableColumns(); 
			ApplyCommandRestrictionOnEditableControl(state, Options.DocumentCapabilities.Tables, state.Enabled);
			ApplyDocumentProtectionToSelectedParagraphs(state);
		}
		protected internal override void PerformModifyModel() {
			ISelectedTableStructureBase selectedCells = DocumentModel.Selection.SelectedCells;
			isNotEmptySelectedCells = CanDeleteTableColumns();
			if (isNotEmptySelectedCells)
				PerformModifyModelCore(selectedCells as SelectedCellsCollection);
#if DEBUGTEST
			DocumentModel.ActivePieceTable.ForceCheckTablesIntegrity();
#endif
		}
		protected internal virtual bool CanDeleteTableColumns() {
			SelectedCellsCollection selectedCellsCollection = DocumentModel.Selection.SelectedCells as SelectedCellsCollection;
			return selectedCellsCollection != null && selectedCellsCollection.IsNotEmpty;
		}
		protected internal virtual void PerformModifyModelCore(SelectedCellsCollection selectedCellsCollection) {
			newLogPosition = CalculateNewLogPosition(selectedCellsCollection);
			ActivePieceTable.DeleteTableColumns(selectedCellsCollection, Control.InnerDocumentServer.Owner);
		}
		protected internal virtual DocumentLogPosition CalculateNewLogPosition(SelectedCellsCollection selectedCells) {
			int minColumnIndex = Int32.MaxValue;
			int selectedRowsCount = selectedCells.RowsCount;
			for (int i = 0; i < selectedRowsCount; i++) {
				minColumnIndex = Math.Min(minColumnIndex, selectedCells[i].StartCell.GetStartColumnIndexConsiderRowGrid());
			}
			TableCell cell = selectedCells.FirstSelectedCell.Table.GetCell(0, minColumnIndex);
			if (cell == null)
				cell = selectedCells.FirstSelectedCell.Table.FirstRow.FirstCell;
			return ActivePieceTable.Paragraphs[cell.StartParagraphIndex].LogPosition;
		}
		protected internal override bool CanChangePosition(DocumentModelPosition pos) {
			return true;
		}
		protected internal override DocumentLogPosition ChangePosition(DocumentModelPosition pos) {
			if (isNotEmptySelectedCells)
				return newLogPosition;
			return pos.LogPosition;
		}
	}
	#endregion
	#region DeleteTableColumnsMenuCommand
	public class DeleteTableColumnsMenuCommand : DeleteTableColumnsCommand {
		public DeleteTableColumnsMenuCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("DeleteTableColumnsMenuCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.DeleteTableColumnsMenuItem; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("DeleteTableColumnsMenuCommandImageName")]
#endif
		public override string ImageName { get { return String.Empty; } }
		#endregion
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			if (state.Enabled == false) {
				state.Visible = false;
				return;
			}
			SelectedCellsCollection cellsCollection = (SelectedCellsCollection)DocumentModel.Selection.SelectedCells;
			state.Enabled = state.Enabled && cellsCollection.IsSelectedEntireTableColumns();
			state.Visible = state.Enabled;
		}
	}
	#endregion
}
