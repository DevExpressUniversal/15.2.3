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
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Snap.Core.Native;
namespace DevExpress.Snap.Core.Commands {
	public class SnapDeleteTableRowsCommand : DeleteTableRowsCommand {
		public SnapDeleteTableRowsCommand(IRichEditControl control)
			: base(control) {
		}
		protected internal override void UpdateUIStateEnabled(ICommandUIState state) {
			base.UpdateUIStateEnabled(state);
			if (!state.Enabled)
				return;
			if (!TableCommandsHelper.CanPerformTableCellsOperationWithRow(DocumentModel))
				state.Enabled = false;
		}
		protected internal override void PerformModifyModelCore(List<TableRow> selectedRows) {
			List<TableCell> modifiedCells = CalculateModifiedCells(selectedRows);
			SnapDocumentModel documentModel = (SnapDocumentModel)DocumentModel;
			SnapCaretPosition position = documentModel.SelectionInfo.GetSnapCaretPositionFromSelection(documentModel.Selection.PieceTable, documentModel.Selection.NormalizedStart);
			base.PerformModifyModelCore(selectedRows);
			TableCommandsHelper.InsertSeparators(ActivePieceTable, modifiedCells);
			((SnapPieceTable)ActivePieceTable).UpdateTemplate(false);
			if (!Object.ReferenceEquals(position, null))
				documentModel.SelectionInfo.RestoreCaretPosition(position);
		}
		List<TableCell> CalculateModifiedCells(List<TableRow> selectedRows) {
			List<TableCell> result = new List<TableCell>();
			DocumentModel.BeginUpdate();
			try {
				SnapBookmark currentBookmark = null;
				int previousRowIndex = Int32.MinValue;
				bool containsSeparator = false;
				int count = selectedRows.Count;
				for (int i = 0; i < count; i++) {
					TableRow row = selectedRows[i];
					SnapObjectModelController objectModelController = new SnapObjectModelController((SnapPieceTable)ActivePieceTable);
					SnapBookmarkController bookmarkController = new SnapBookmarkController((SnapPieceTable)ActivePieceTable);
					SnapBookmark bookmark = bookmarkController.FindInnermostTemplateBookmarkByTableCell(row.FirstCell);
					if (Object.ReferenceEquals(bookmark, null))
						continue;
					if (!Object.ReferenceEquals(bookmark, currentBookmark) || row.IndexInTable != previousRowIndex + 1) {
						currentBookmark = bookmark;
						if (containsSeparator)
							result.Add(row.Next.Cells[0]);
						containsSeparator = false;
					}
					for (int cellIndex = 0; cellIndex < row.Cells.Count; cellIndex++) {
						if (objectModelController.GetFirstCellRun(row.Cells[cellIndex]) is SeparatorTextRun) {
							containsSeparator = true;
							break;
						}
					}
					previousRowIndex = row.IndexInTable;
				}
				if (containsSeparator)
					result.Add(selectedRows[count - 1].Next.FirstCell);
			}
			finally {
				DocumentModel.EndUpdate();
			}
			return result;
		}
	}
}
