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
using DevExpress.XtraRichEdit.Tables.Native;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
#if SL
using System.Windows.Controls.Primitives;
#endif
namespace DevExpress.XtraRichEdit.Commands {
	#region NextLineCommand
	public class NextLineCommand : LineUpDownCommandBase {
		public NextLineCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("NextLineCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.NextLine; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("NextLineCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_MoveLineDown; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("NextLineCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_MoveLineDownDescription; } }
		protected internal override bool TryToKeepCaretX { get { return true; } }
		protected internal override bool TreatStartPositionAsCurrent { get { return false; } }
		protected internal override bool ExtendSelection { get { return false; } }
		#endregion
#if SL
		protected internal override void ExecuteCore() {
			if (DocumentServer.ActualReadOnly && !Control.ShowCaretInReadOnly)
				EmulateVerticalScroll(ScrollEventType.SmallIncrement);
			else
				base.ExecuteCore();
		}
#endif
		protected internal override NextCaretPositionVerticalDirectionCalculator CreateNextCaretPositionCalculator() {
			return new NextCaretPositionLineDownCalculator(Control);
		}
	}
	#endregion
	#region ExtendNextLineCommand
	public class ExtendNextLineCommand : NextLineCommand {
		public ExtendNextLineCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ExtendNextLineCommandId")]
#endif
public override RichEditCommandId Id { get { return RichEditCommandId.ExtendNextLine; } }
		protected internal override bool ExtendSelection { get { return true; } }
		protected internal override NextCaretPositionVerticalDirectionCalculator CreateNextCaretPositionCalculator() {
			return new ExtendNextCaretPositionLineDownCalculator(Control);
		}
		protected internal override void UpdateTableSelectionAfterSelectionUpdated(DocumentLogPosition logPosition) {
			Selection selection = DocumentModel.Selection;
			selection.UpdateTableSelectionEnd(logPosition);
			selection.UpdateTableSelectionStart(logPosition);
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Commands.Internal {
	#region NextCaretPositionLineDownCalculator
	public class NextCaretPositionLineDownCalculator : NextCaretPositionDownDirectionCalculator {
		public NextCaretPositionLineDownCalculator(IRichEditControl control)
			: base(control) {
		}
		protected internal override int GetNextLayoutRowIndexInDirection(int sourceRowIndex) {
			return sourceRowIndex + 1;
		}
		protected internal override bool ShouldObtainTargetPageViewInfo(int sourceRowIndex, int sourceRowCount) {
			Debug.Assert(sourceRowIndex >= 0);
			Debug.Assert(sourceRowIndex + 1 <= sourceRowCount);
			return sourceRowIndex + 1 == sourceRowCount;
		}
		protected internal override PageViewInfoRow ObtainExistingTargetPageViewInfoRow(PageViewInfoRowCollection pageRows, int currentRowIndex) {
			return pageRows[currentRowIndex + 1];
		}
	}
	#endregion
	#region ExtendNextCaretPositionLineDownCalculator
	public class ExtendNextCaretPositionLineDownCalculator : NextCaretPositionLineDownCalculator {
		public ExtendNextCaretPositionLineDownCalculator(IRichEditControl control)
			: base(control) {
		}
		protected internal override DocumentModelPosition GetDefaultPosition() {
			return DocumentModelPosition.FromParagraphEnd(ActivePieceTable, ActivePieceTable.Paragraphs.Last.Index);
		}
		protected Selection Selection { get { return Control.InnerDocumentServer.DocumentModel.Selection; } }
		protected ISelectedTableStructureBase SelectedCells { get { return Selection.SelectedCells; } set { Selection.SelectedCells = value; } }
		protected internal override Row MoveFromInnerIntoOuterTable(TableCellViewInfo innerSourceCellViewInfo, Column column, TableCellViewInfo lastCellInTableRow, int caretX) {
			if (Selection.SelectedCells is StartSelectedCellInTable) {
#if (DEBUGTEST||DEBUG)
#endif
				if (Selection.SelectedCells.FirstSelectedCell != null && Object.ReferenceEquals(lastCellInTableRow.TableViewInfo.ParentTableCellViewInfo, Selection.SelectedCells.FirstSelectedCell)) 
					Selection.SelectedCells = new SelectedCellsCollection((StartSelectedCellInTable)Selection.SelectedCells);
			}
			return base.MoveFromInnerIntoOuterTable(innerSourceCellViewInfo, column, lastCellInTableRow, caretX);
		}
		protected internal override bool ShouldJumpToNextCellInCurrentTable(bool shouldJumpToCellInNestedTable,TableCellViewInfo nextCell, bool isLastOrFirstLayoutRowInCell) {
			bool baseCondition = base.ShouldJumpToNextCellInCurrentTable(shouldJumpToCellInNestedTable, nextCell, isLastOrFirstLayoutRowInCell);
			if (Selection.SelectedCells is StartSelectedCellInTable)
				return baseCondition;
			SelectedCellsCollection selectedCells = (SelectedCellsCollection)Selection.SelectedCells;
			bool selectingOneCell = selectedCells.SelectedOnlyOneCell;
			if (baseCondition)
				return true;
			return !selectingOneCell && (nextCell != null);
		}
		protected internal override void CorrectCaretXOnExtendSelection(TableCellViewInfo sourceCellViewInfo, ref int caretX) {
			if (sourceCellViewInfo == null)
				return;
			caretX = sourceCellViewInfo.Left + sourceCellViewInfo.Width - 10;
		}
	}
	#endregion
}
