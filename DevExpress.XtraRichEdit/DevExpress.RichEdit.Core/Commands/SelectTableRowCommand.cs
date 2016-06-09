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
using DevExpress.XtraRichEdit.Tables.Native;
using System.ComponentModel;
namespace DevExpress.XtraRichEdit.Commands {
	#region SelectTableRowCommand
	public class SelectTableRowCommand : RichEditSelectionCommand {
		#region Fields
		TableRowCollection rows;
		int startRowIndex;
		int endRowIndex;
		bool canCalculateExecutionParameters;
		#endregion
		public SelectTableRowCommand(IRichEditControl control)
			: base(control) {
			this.canCalculateExecutionParameters = true;
		}
		#region Properties
		public override RichEditCommandId Id { get { return RichEditCommandId.SelectTableRow; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_SelectTableRow; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_SelectTableRowDescription; } }
		public override string ImageName { get { return "SelectTableRow"; } }
		protected internal override bool TryToKeepCaretX { get { return false; } }
		protected internal override bool TreatStartPositionAsCurrent { get { return false; } }
		protected internal override bool ExtendSelection { get { return false; } }
		protected internal override DocumentLayoutDetailsLevel UpdateCaretPositionBeforeChangeSelectionDetailsLevel { get { return DocumentLayoutDetailsLevel.None; } }
		public TableRowCollection Rows { get { return rows; } set { rows = value; } }
		public int StartRowIndex { get { return startRowIndex; } set { startRowIndex = value; } }
		public int EndRowIndex { get { return endRowIndex; } set { endRowIndex = value; } }
		public bool CanCalculateExecutionParameters { get { return canCalculateExecutionParameters; } set { canCalculateExecutionParameters = value; } }
		#endregion
		protected internal override bool PerformChangeSelection() {
			if (CanCalculateExecutionParameters)
				CalculateExecutionParameters();
			return base.PerformChangeSelection();
		}
		protected internal virtual void CalculateExecutionParameters() {
			if (!DocumentModel.Selection.IsWholeSelectionInOneTable())
				return;
			SelectedCellsCollection selectedCellsCollection = (SelectedCellsCollection)DocumentModel.Selection.SelectedCells;
			TableRow row = selectedCellsCollection.NormalizedFirst.Row;
			Rows = row.Table.Rows;
			StartRowIndex = row.IndexInTable;
			EndRowIndex = selectedCellsCollection.NormalizedLast.Row.IndexInTable;
		}
		protected internal override void ChangeSelection(Selection selection) {
			selection.ClearSelectionInTable();
			TableRow startRow = Rows[Algorithms.Min(StartRowIndex, EndRowIndex)];
			TableRow endRow = Rows[Algorithms.Max(StartRowIndex, EndRowIndex)];
			Paragraph startParagraph = ActivePieceTable.Paragraphs[startRow.FirstCell.StartParagraphIndex];
			Paragraph endParagraph = ActivePieceTable.Paragraphs[endRow.LastCell.EndParagraphIndex];
			DocumentLogPosition startSelection = startParagraph.LogPosition;
			DocumentLogPosition endSelection = endParagraph.LogPosition + endParagraph.Length;
			if (StartRowIndex <= EndRowIndex) {
				selection.Start = startSelection;
				selection.End = endSelection;
			}
			else {
				selection.Start = endSelection;
				selection.End = startSelection;
			}
			selection.SelectedCells = GetSelectedCells();
			selection.RemoveIntersectedSelectionItems(1); 
			ValidateSelection(selection, true);
		}
		protected internal virtual SelectedCellsCollection GetSelectedCells() {
			SelectedCellsCollection result = new SelectedCellsCollection();
			if (StartRowIndex <= EndRowIndex) {
				for (int i = StartRowIndex; i <= EndRowIndex; i++) {
					GetSelectedCellsCore(result, i);
				}
			}
			else {
				for (int i = StartRowIndex; i >= EndRowIndex; i--) {
					GetSelectedCellsCore(result, i);
				}
			}
			return result;
		}
		protected internal virtual void GetSelectedCellsCore(SelectedCellsCollection selectedCells, int index) {
			TableRow currentRow = Rows[index];
			selectedCells.AddSelectedCells(currentRow, 0, currentRow.Cells.Count - 1);
		}
		protected internal override DocumentLogPosition ChangePosition(DocumentModelPosition pos) {
			return pos.LogPosition;
		}
		protected internal override bool CanChangePosition(DocumentModelPosition pos) {
			return true;
		}
	}
	#endregion
}
