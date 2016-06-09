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
using System.ComponentModel;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Tables.Native;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.Snap.Core.Native;
namespace DevExpress.Snap.Core.Commands {
	#region SnapDeleteTableColumnsCommand
	public class SnapDeleteTableColumnsCommand : DeleteTableColumnsCommand {
		List<TableCell> modifiedCells;
		public SnapDeleteTableColumnsCommand(IRichEditControl control)
			: base(control) {
		}
		protected internal override bool CanDeleteTableColumns() {
			if (!base.CanDeleteTableColumns())
				return false;
			return TableCommandsHelper.CanPerformTableCellOperationWithColumn(DocumentModel);
		}
		protected internal override void PerformModifyModelCore(SelectedCellsCollection selectedCellsCollection) {
			CalculateModifiedCells(selectedCellsCollection);
			SnapDocumentModel documentModel = (SnapDocumentModel)DocumentModel;
			SnapCaretPosition position = documentModel.SelectionInfo.GetSnapCaretPositionFromSelection(documentModel.Selection.PieceTable, documentModel.Selection.NormalizedStart);
			base.PerformModifyModelCore(selectedCellsCollection);
			TableCommandsHelper.InsertSeparators(ActivePieceTable, modifiedCells);
			((SnapPieceTable)ActivePieceTable).UpdateTemplate(false);
			if (!Object.ReferenceEquals(position, null))
				documentModel.SelectionInfo.RestoreCaretPosition(position);
		}
		protected internal override void ChangeSelection(Selection selection) {
		}
		void CalculateModifiedCells(SelectedCellsCollection selectedCells) {
			TableRowCollection rows = selectedCells.NormalizedFirst.Table.Rows;
			ColumnIndexesInfo info = TableCommandsHelper.CalculateSelectedColumnIndexes(selectedCells);
			DocumentModel.BeginUpdate();
			try {
				CalculateModifiedCellsCore(rows, info.StartColumnIndex, info.EndColumnIndex);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		void CalculateModifiedCellsCore(TableRowCollection rows, int start, int end) {
			modifiedCells = new List<TableCell>();
			SnapObjectModelController objectModelController = new SnapObjectModelController((SnapPieceTable)ActivePieceTable);
			SnapBookmarkController bookmarkController = new SnapBookmarkController((SnapPieceTable)ActivePieceTable);
			int count = rows.Count;
			for (int rowIndex = 0; rowIndex < count; rowIndex++) {
				TableRow row = rows[rowIndex];
				int cellIndex = start;
				bool containsSeparator = false;
				while (cellIndex <= end) {
					TableCell cell = TableCellVerticalBorderCalculator.GetCellByColumnIndex(row, cellIndex);
					if (cell == null)
						break;
					if (objectModelController.GetFirstCellRun(cell) is SeparatorTextRun) {
						containsSeparator = true;
						break;
					}
					cellIndex += cell.ColumnSpan;
				}
				if (containsSeparator) {
					TableCell lastSelectedCell = TableCellVerticalBorderCalculator.GetCellByColumnIndex(row, end);
					if (lastSelectedCell != null && !bookmarkController.IsTableCellLastInTemplateBookmark(lastSelectedCell))
						modifiedCells.Add(lastSelectedCell.Next);
				}
			}
		}
	}
	#endregion
	#region SnapDeleteTableColumnsMenuCommand
	public class SnapDeleteTableColumnsMenuCommand : SnapDeleteTableColumnsCommand {
		public SnapDeleteTableColumnsMenuCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override RichEditCommandId Id { get { return RichEditCommandId.DeleteTableColumnsMenuItem; } }
		public override string ImageName { get { return String.Empty; } }
		#endregion
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			if (state.Enabled == false) {
				state.Visible = false;
				return;
			}
			SelectedCellsCollection cellsCollection = (SelectedCellsCollection)DocumentModel.Selection.SelectedCells;
			state.Enabled &= cellsCollection.IsSelectedEntireTableColumns();
			state.Visible = state.Enabled;
		}
	}
	#endregion
}
