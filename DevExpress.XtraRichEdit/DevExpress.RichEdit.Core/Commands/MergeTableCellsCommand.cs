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
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Commands.Internal;
using System.Collections.Generic;
using DevExpress.XtraRichEdit.Tables.Native;
using DevExpress.XtraRichEdit.Model.History;
using DevExpress.XtraRichEdit.Layout;
namespace DevExpress.XtraRichEdit.Commands {
	#region MergeTableElementCommandBase (abstract class)
	public abstract class MergeTableElementCommandBase : RichEditSelectionCommand {
		protected MergeTableElementCommandBase(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_MergeTableCells; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_MergeTableCellsDescription; } }
		public override string ImageName { get { return "MergeTableCells"; } }
		protected internal override bool TryToKeepCaretX { get { return false; } }
		protected internal override bool TreatStartPositionAsCurrent { get { return false; } }
		protected internal override bool ExtendSelection { get { return false; } }
		protected internal override DocumentLayoutDetailsLevel UpdateCaretPositionBeforeChangeSelectionDetailsLevel { get { return DocumentLayoutDetailsLevel.None; } }
		#endregion
		protected override void UpdateUIStateCore(ICommandUIState state) {
			UpdateUIStateEnabled(state);
			ApplyCommandRestrictionOnEditableControl(state, Options.DocumentCapabilities.Tables, state.Enabled);
			ApplyDocumentProtectionToSelectedParagraphs(state);
		}
		protected virtual void UpdateUIStateEnabled(ICommandUIState state) {
			if (!DocumentModel.Selection.IsWholeSelectionInOneTable())
				state.Enabled = false;
			else {
				SelectedCellsCollection cells = (SelectedCellsCollection)DocumentModel.Selection.SelectedCells;
				state.Enabled = cells.IsSquare()
					&& (cells.NormalizedFirst.NormalizedLength > 0 || cells.RowsCount > 1);
			}
		}
		SelectedCellsCollection selectedCellsCollection;
		protected internal override void PerformModifyModel() {
			if (!DocumentModel.Selection.SelectedCells.IsNotEmpty)
				return;
			this.selectedCellsCollection = (SelectedCellsCollection)DocumentModel.Selection.SelectedCells;
			ActivePieceTable.MergeCells(selectedCellsCollection);
#if DEBUGTEST
			DocumentModel.ActivePieceTable.ForceCheckTablesIntegrity();
#endif
		}
		protected internal override bool CanChangePosition(DocumentModelPosition pos) {
			return true;
		}
		protected internal override bool PerformChangeSelection() {
			if (selectedCellsCollection.NormalizedFirst != null)
				ChangeSelection(selectedCellsCollection.NormalizedFirst.NormalizedStartCell);
			else {
				DocumentModel.Selection.ClearSelectionInTable();
			}
			return true;
		}
		protected internal virtual void ChangeSelection(TableCell cellToSelect) {
			SelectTableCellCommand command = new SelectTableCellCommand(Control);
			command.Cell = cellToSelect;
			command.ChangeSelection(DocumentModel.Selection);
		}
		protected internal override DocumentLogPosition ChangePosition(DocumentModelPosition pos) {
			return pos.LogPosition;
		}
	}
	#endregion
	#region MergeTableElementMenuCommand
	public class MergeTableElementMenuCommand : MergeTableElementCommandBase {
		public MergeTableElementMenuCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override RichEditCommandId Id { get { return RichEditCommandId.MergeTableElement; } }
		#endregion
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			state.Visible = state.Enabled;
		}
	}
	#endregion
	#region MergeTableCellsCommand
	public class MergeTableCellsCommand : MergeTableElementCommandBase {
		public MergeTableCellsCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("MergeTableCellsCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.MergeTableCells; } }
		#endregion
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			ApplyCommandsRestriction(state, Options.DocumentCapabilities.Tables, state.Enabled);
		}
	}
	#endregion
}
