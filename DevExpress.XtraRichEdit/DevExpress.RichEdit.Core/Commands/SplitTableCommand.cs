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
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Tables.Native;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Commands {
	#region SplitTableCommand
	public class SplitTableCommand : RichEditSelectionCommand {
		#region Fields
		DocumentLogPosition firstCellLogPosition;
		#endregion
		public SplitTableCommand(IRichEditControl control)
			: base(control) {
			this.firstCellLogPosition = new DocumentLogPosition(-1);
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SplitTableCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.SplitTable; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SplitTableCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_SplitTable; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SplitTableCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_SplitTableDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SplitTableCommandImageName")]
#endif
		public override string ImageName { get { return "SplitTable"; } }
		protected internal override bool TryToKeepCaretX { get { return false; } }
		protected internal override bool ExtendSelection { get { return false; } }
		protected internal override DocumentLayoutDetailsLevel UpdateCaretPositionBeforeChangeSelectionDetailsLevel { get { return DocumentLayoutDetailsLevel.None; } }
		protected internal override bool TreatStartPositionAsCurrent { get { return false; } }
		#endregion
		protected internal override void PerformModifyModel() {
			PerformTableSplitBySelectionStart();
		}
		protected internal virtual void PerformTableSplitBySelectionStart() {
			TableCell startCell = GetStartCell(DocumentModel.Selection);
			ParagraphIndex firstCellStartParagraphIndex = startCell.Row.FirstCell.StartParagraphIndex;
			this.firstCellLogPosition = ActivePieceTable.Paragraphs[firstCellStartParagraphIndex].LogPosition;
			ActivePieceTable.SplitTable(startCell.Table.Index, startCell.RowIndex, GetForceVisible());
#if DEBUGTEST
			DocumentModel.ActivePieceTable.ForceCheckTablesIntegrity();
#endif
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = CheckSelection();
			ApplyCommandRestrictionOnEditableControl(state, Options.DocumentCapabilities.Tables, state.Enabled);
			ApplyDocumentProtectionToSelectedParagraphs(state);
		}
		protected internal virtual bool CheckSelection() {
			Selection selection = DocumentModel.Selection;
			if (!selection.IsWholeSelectionInOneTable())
				return false;
			TableCell startCell = GetStartCell(selection);
			TableCell endCell = GetEndCell(selection);
			return !(startCell == null || endCell == null || startCell.Table != endCell.Table);
		}
		protected internal virtual TableCell GetStartCell(Selection selection) {
			return ((SelectedCellsCollection)selection.SelectedCells).NormalizedFirst.NormalizedStartCell;
		}
		protected internal virtual TableCell GetEndCell(Selection selection) {
			return ((SelectedCellsCollection)selection.SelectedCells).NormalizedLast.NormalizedEndCell;
		}
		protected internal override bool CanChangePosition(DocumentModelPosition pos) {
			return true;
		}
		protected internal override DocumentLogPosition ChangePosition(DocumentModelPosition pos) {
			Debug.Assert(firstCellLogPosition != new DocumentLogPosition(-1));
			return firstCellLogPosition;
		}
	}
	#endregion
}
