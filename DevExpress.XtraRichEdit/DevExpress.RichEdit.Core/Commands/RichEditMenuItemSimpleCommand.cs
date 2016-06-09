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
using DevExpress.XtraRichEdit.Tables.Native;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
namespace DevExpress.XtraRichEdit.Commands {
	#region RichEditMenuItemSimpleCommand (abstract class)
	public abstract class RichEditMenuItemSimpleCommand : RichEditCommand {
		protected RichEditMenuItemSimpleCommand(IRichEditControl control)
			: base(control) {
		}
		public override void ForceExecute(ICommandUIState state) {
			CheckExecutedAtUIThread();
			NotifyBeginCommandExecution(state);
			try {
				ExecuteCore();
			}
			finally {
				NotifyEndCommandExecution(state);
			}
		}
		protected internal abstract void ExecuteCore();
		protected internal bool GetForceVisible() {
			if (CommandSourceType == CommandSourceType.Keyboard ||
				CommandSourceType == CommandSourceType.Menu ||
				CommandSourceType == CommandSourceType.Mouse)
				return !DocumentModel.FormattingMarkVisibilityOptions.ShowHiddenText;
			else
				return false;
		}
		protected internal SelectedCellsCollection GetSelectedCellsCollection() {
			if (DocumentModel.Selection.Items.Count > 1) {
				Selection selection = DocumentModel.Selection;
				PieceTable selectionPieceTable = selection.PieceTable;
				SelectedCellsCollection cells = new SelectedCellsCollection();
				for (int i = 0; i < selection.Items.Count; i++) {
					DocumentModelPosition startPos = PositionConverter.ToDocumentModelPosition(selectionPieceTable, selection.Items[i].NormalizedStart);
					TableCell startCell = selectionPieceTable.Paragraphs[startPos.ParagraphIndex].GetCell();
					DocumentModelPosition endPos = PositionConverter.ToDocumentModelPosition(selectionPieceTable, selection.Items[i].NormalizedEnd - 1);
					TableCell endCell = selectionPieceTable.Paragraphs[endPos.ParagraphIndex].GetCell();
					if (startCell == null || endCell == null)
						return null;
					cells.AddSelectedCells(startCell.Row, startCell.IndexInRow, endCell.IndexInRow);
				}
				return cells;
			}
			else
				return DocumentModel.Selection.SelectedCells as SelectedCellsCollection;
		}
	}
	#endregion
}
