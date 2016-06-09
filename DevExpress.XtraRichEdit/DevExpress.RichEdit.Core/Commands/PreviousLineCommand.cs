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
using System.ComponentModel;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Layout.TableLayout;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Tables.Native;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
#if SL
using System.Windows.Controls.Primitives;
#endif
namespace DevExpress.XtraRichEdit.Commands {
	#region PreviousLineCommand
	public class PreviousLineCommand : LineUpDownCommandBase {
		public PreviousLineCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("PreviousLineCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.PreviousLine; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("PreviousLineCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_MoveLineUp; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("PreviousLineCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_MoveLineUpDescription; } }
		protected internal override bool TryToKeepCaretX { get { return true; } }
		protected internal override bool TreatStartPositionAsCurrent { get { return true; } }
		protected internal override bool ExtendSelection { get { return false; } }
		#endregion
#if SL
		protected internal override void ExecuteCore() {
			if (DocumentServer.ActualReadOnly && !Control.ShowCaretInReadOnly)
				EmulateVerticalScroll(ScrollEventType.SmallDecrement);
			else
				base.ExecuteCore();
		}
#endif
		protected internal override NextCaretPositionVerticalDirectionCalculator CreateNextCaretPositionCalculator() {
			return new NextCaretPositionLineUpCalculator(Control);
		}
	}
	#endregion
	#region ExtendPreviousLineCommand
	public class ExtendPreviousLineCommand : PreviousLineCommand {
		public ExtendPreviousLineCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ExtendPreviousLineCommandId")]
#endif
public override RichEditCommandId Id { get { return RichEditCommandId.ExtendPreviousLine; } }
		protected internal override bool ExtendSelection { get { return true; } }
		protected internal override NextCaretPositionVerticalDirectionCalculator CreateNextCaretPositionCalculator() {
			return new ExtendNextCaretPositionLineUpCalculator(Control);
		}
		protected internal override void UpdateTableSelectionAfterSelectionUpdated(DocumentLogPosition logPosition) {
			Selection selection = DocumentModel.Selection;
			selection.UpdateTableSelectionStart(logPosition);
			selection.UpdateTableSelectionEnd(logPosition);
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Commands.Internal {
	#region NextCaretPositionLineUpCalculator
	public class NextCaretPositionLineUpCalculator : NextCaretPositionUpDirectionCalculator {
		public NextCaretPositionLineUpCalculator(IRichEditControl control)
			: base(control) {
		}
		protected internal override int GetNextLayoutRowIndexInDirection(int sourceRowIndex) {
			return Math.Max(sourceRowIndex - 1, 0);
		}
		protected internal override Row GetTargetLayoutRowInCell(RowCollection rows) {
			return rows.First;
		}
		protected internal override bool ShouldObtainTargetPageViewInfo(int sourceRowIndex, int sourceRowCount) {
			Debug.Assert(sourceRowIndex >= 0);
			return sourceRowIndex == 0;
		}
		protected internal override PageViewInfoRow ObtainExistingTargetPageViewInfoRow(PageViewInfoRowCollection pageRows, int currentRowIndex) {
			return pageRows[currentRowIndex - 1];
		}
	}
	#endregion
	#region ExtendNextCaretPositionLineUpCalculator
	public class ExtendNextCaretPositionLineUpCalculator : NextCaretPositionLineUpCalculator {
		public ExtendNextCaretPositionLineUpCalculator(IRichEditControl control)
			: base(control) {
		}
		protected Selection Selection { get { return Control.InnerDocumentServer.DocumentModel.Selection; } }
		protected ISelectedTableStructureBase SelectedCells { get { return Selection.SelectedCells; } set { Selection.SelectedCells = value; } }
		protected internal override DocumentModelPosition GetDefaultPosition() {
			return DocumentModelPosition.FromParagraphStart(ActivePieceTable, ActivePieceTable.Paragraphs.First.Index);
		}
		protected internal override bool ShouldJumpToNextCellInCurrentTable(bool shouldJumpToCellInNestedTable, TableCellViewInfo nextCell, bool isLastOrFirstLayoutRowInCell) {
			bool baseCondition = base.ShouldJumpToNextCellInCurrentTable(shouldJumpToCellInNestedTable, nextCell, isLastOrFirstLayoutRowInCell);
			if (Selection.SelectedCells is StartSelectedCellInTable)
				return baseCondition;
			SelectedCellsCollection selectedCells = (SelectedCellsCollection)Selection.SelectedCells;
			bool selectingOneCell = selectedCells.SelectedOnlyOneCell;
			if (baseCondition)
				return true;
			return !selectingOneCell && (nextCell != null);
		}
	}
	#endregion
}
