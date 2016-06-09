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
using System.Collections.Generic;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Tables.Native;
namespace DevExpress.XtraRichEdit.Commands {
	#region SplitTableCellsCommand
	public class SplitTableCellsCommand : RichEditSelectionCommand {
		#region Fields
		SplitTableCellsParameters cellsParameters;
		DocumentLogPosition newPosition;
		#endregion
		public SplitTableCellsCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertTableCells; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertTableCellsDescription; } }
		protected internal override bool TryToKeepCaretX { get { return false; } }
		protected internal override bool ExtendSelection { get { return false; } }
		protected internal override DocumentLayoutDetailsLevel UpdateCaretPositionBeforeChangeSelectionDetailsLevel { get { return DocumentLayoutDetailsLevel.None; } }
		protected internal override bool TreatStartPositionAsCurrent { get { return false; } }
		public SplitTableCellsParameters CellsParameters { get { return cellsParameters; } set { cellsParameters = value; } }
		#endregion
		public override void ForceExecute(DevExpress.Utils.Commands.ICommandUIState state) {
			DefaultValueBasedCommandUIState<SplitTableCellsParameters> valueState = state as DefaultValueBasedCommandUIState<SplitTableCellsParameters>;
			if (valueState == null || valueState.Value == null)
				return;
			this.cellsParameters = valueState.Value;
			base.ForceExecute(state);
		}
		protected internal override void PerformModifyModel() {
			if (!DocumentModel.Selection.IsWholeSelectionInOneTable())
				return;
			SelectedCellsCollection selectedCells = (SelectedCellsCollection)DocumentModel.Selection.SelectedCells;
			TableCell firstCell = selectedCells.NormalizedFirst.NormalizedStartCell;
			if (CellsParameters.MergeCellsBeforeSplit) {
				MergeTableCellsCommand mergeCommand = new MergeTableCellsCommand(Control);
				mergeCommand.PerformModifyModel();
			}
			SplitTableCellsHorizontally(selectedCells);
			SplitTableCellsVertically(selectedCells);
			newPosition = CalculateNewPosition(firstCell);
#if DEBUGTEST
			DocumentModel.ActivePieceTable.ForceCheckTablesIntegrity();
#endif
		}
		protected internal virtual DocumentLogPosition CalculateNewPosition(TableCell firstCell) {
			return ActivePieceTable.Paragraphs[firstCell.EndParagraphIndex].EndLogPosition;
		}
		protected internal virtual void SplitTableCellsHorizontally(SelectedCellsCollection selectedCells) {
			IInnerRichEditDocumentServerOwner documentServerOwner = Control.InnerDocumentServer.Owner;
			if (CellsParameters.MergeCellsBeforeSplit) {
				TableCell startCell = selectedCells.NormalizedFirst.NormalizedStartCell;
				ActivePieceTable.SplitTableCellsHorizontally(startCell, CellsParameters.ColumnsCount, GetForceVisible(), documentServerOwner);
				return;
			}
			int topRowIndex = selectedCells.GetTopRowIndex();
			bool forceVisible = GetForceVisible();
			for (int i = selectedCells.GetBottomRowIndex(); i >= topRowIndex; i--) {
				SelectedCellsIntervalInRow currentSelectedRow = selectedCells[i];
				TableCellCollection cells = currentSelectedRow.Row.Cells;
				int startCellIndex = currentSelectedRow.NormalizedStartCellIndex;
				for (int j = currentSelectedRow.NormalizedEndCellIndex; j >= startCellIndex; j--) {
					TableCell currentCell = cells[j];
					if (currentCell.VerticalMerging == MergingState.Continue)
						continue;
					ActivePieceTable.SplitTableCellsHorizontally(currentCell, CellsParameters.ColumnsCount, forceVisible, documentServerOwner);
				}
			}
		}
		protected internal virtual void SplitTableCellsVertically(SelectedCellsCollection selectedCells) {
			int rowsCount = CellsParameters.RowsCount;
			if (rowsCount == 1) {
				return;
			}
			SelectedCellsIntervalInRow selectedFirstInterval = selectedCells.NormalizedFirst;
			int columnsCount = GetColumnsCountForSplitVertically(selectedFirstInterval);
			ActivePieceTable.SplitTableCellsVertically(selectedFirstInterval.NormalizedStartCell, rowsCount, columnsCount, GetForceVisible());
		}
		protected internal virtual int GetColumnsCountForSplitVertically(SelectedCellsIntervalInRow interval) {
			if (CellsParameters.MergeCellsBeforeSplit)
				return CellsParameters.ColumnsCount;
			return (interval.NormalizedLength + 1) * CellsParameters.ColumnsCount;
		}
		protected internal override bool CanChangePosition(DocumentModelPosition pos) {
			return true;
		}
		protected internal override DocumentLogPosition ChangePosition(DocumentModelPosition pos) {
			return newPosition;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = DocumentModel.Selection.IsWholeSelectionInOneTable();
			ApplyCommandRestrictionOnEditableControl(state, Options.DocumentCapabilities.Tables, state.Enabled);
			ApplyDocumentProtectionToSelectedParagraphs(state);
		}
	}
	#endregion
}
