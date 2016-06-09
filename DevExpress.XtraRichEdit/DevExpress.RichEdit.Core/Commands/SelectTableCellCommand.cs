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
using DevExpress.XtraRichEdit.Tables.Native;
using System.ComponentModel;
namespace DevExpress.XtraRichEdit.Commands {
	#region SelectTableCellCommand
	public class SelectTableCellCommand : RichEditSelectionCommand {
		#region Fields
		TableCell cell;
		bool shouldUpdateCaretY;
		#endregion
		public SelectTableCellCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override RichEditCommandId Id { get { return RichEditCommandId.SelectTableCell; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_SelectTableCell; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_SelectTableCellDescription; } }
		public override string ImageName { get { return "SelectTableCell"; } }
		protected internal override bool TryToKeepCaretX { get { return false; } }
		protected internal override bool TreatStartPositionAsCurrent { get { return false; } }
		protected internal override bool ExtendSelection { get { return false; } }
		protected internal override DocumentLayoutDetailsLevel UpdateCaretPositionBeforeChangeSelectionDetailsLevel { get { return DocumentLayoutDetailsLevel.None; } }
		public bool ShouldUpdateCaretTableAnchorVerticalPositionY { get { return shouldUpdateCaretY; } set { shouldUpdateCaretY = value; } }
		protected internal override bool ShouldUpdateCaretY { get { return ShouldUpdateCaretTableAnchorVerticalPositionY; } }
		public TableCell Cell { get { return cell; } set { cell = value; } }
		#endregion
		protected internal override bool PerformChangeSelection() {
			if (Cell == null)
				CalculateSelectedCell();
			return base.PerformChangeSelection();
		}
		protected internal virtual void CalculateSelectedCell() {
			if (!DocumentModel.Selection.IsWholeSelectionInOneTable())
				return;
			SelectedCellsCollection cells = (SelectedCellsCollection)DocumentModel.Selection.SelectedCells;
			bool isEmpty = !cells.IsNotEmpty;
			bool severalRowsSelected = cells.RowsCount > 1;
			if (isEmpty || severalRowsSelected)
				return;
			bool isSquare = cells.IsSquare();
			bool isPositiveLength = cells.NormalizedFirst.NormalizedLength > 0;
			if (isSquare && isPositiveLength)
				return;
			Cell = cells.TopLeftCell;
		}
		protected internal override void ChangeSelection(Selection selection) {
			if (Cell == null)
				return;
			Paragraph startParagraph = ActivePieceTable.Paragraphs[Cell.StartParagraphIndex];
			Paragraph endParagraph = ActivePieceTable.Paragraphs[Cell.EndParagraphIndex];
			selection.ClearSelectionInTable();
			selection.ActiveSelection.Start = startParagraph.LogPosition;
			selection.ActiveSelection.End = endParagraph.EndLogPosition + 1;
			selection.TryMergeByActiveSelection();
			selection.SetStartCell(selection.Start);
			ValidateSelection(selection, true);
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
