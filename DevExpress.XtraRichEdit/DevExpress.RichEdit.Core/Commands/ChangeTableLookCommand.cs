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
using System.Linq;
using System.Text;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Localization;
using System.ComponentModel;
namespace DevExpress.XtraRichEdit.Commands {
	#region ToggleFirstRowCommand
	public class ToggleFirstRowCommand : ChangeTableLookCommand {
		public ToggleFirstRowCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleFirstRowCommandId")]
#endif
		public override RichEditCommandId Id {get { return RichEditCommandId.ToggleFirstRow; } }
		protected override TableLookTypes TableLook { get { return TableLookTypes.ApplyFirstRow; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleFirstRowCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleFirstRow; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleFirstRowCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleFirstRowDescription; } }
	}
	#endregion
	#region ToggleLastRowCommand
	public class ToggleLastRowCommand : ChangeTableLookCommand {
		public ToggleLastRowCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleLastRowCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.ToggleLastRow; } }
		protected override TableLookTypes TableLook { get { return TableLookTypes.ApplyLastRow; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleLastRowCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleLastRow; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleLastRowCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleLastRowDescription; } }
	}
	#endregion
	#region ToggleFirstColumnCommand
	public class ToggleFirstColumnCommand : ChangeTableLookCommand {
		public ToggleFirstColumnCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleFirstColumnCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.ToggleFirstColumn; } }
		protected override TableLookTypes TableLook { get { return TableLookTypes.ApplyFirstColumn; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleFirstColumnCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleFirstColumn; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleFirstColumnCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleFirstColumnDescription; } }
	}
	#endregion
	#region ToggleLastColumnCommand
	public class ToggleLastColumnCommand : ChangeTableLookCommand {
		public ToggleLastColumnCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleLastColumnCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.ToggleLastColumn; } }
		protected override TableLookTypes TableLook { get { return TableLookTypes.ApplyLastColumn; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleLastColumnCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleLastColumn; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleLastColumnCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleLastColumnDescription; } }
	}
	#endregion
	#region ToggleBandedRowsCommand
	public class ToggleBandedRowsCommand : ChangeBandedTableLookCommand {
		public ToggleBandedRowsCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleBandedRowsCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.ToggleBandedRows; } }
		protected override TableLookTypes TableLook { get { return TableLookTypes.DoNotApplyRowBanding; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleBandedRowsCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleBandedRows; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleBandedRowsCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleBandedRowsDescription; } }
	}
	#endregion
	#region ToggleBandedColumnCommand
	public class ToggleBandedColumnCommand : ToggleBandedColumnsCommand {
		public ToggleBandedColumnCommand(IRichEditControl control)
			: base(control) {
		}
	}
	#endregion
	#region ToggleBandedColumnsCommand
	public class ToggleBandedColumnsCommand : ChangeBandedTableLookCommand {
		public ToggleBandedColumnsCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleBandedColumnsCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.ToggleBandedColumns; } }
		protected override TableLookTypes TableLook { get { return TableLookTypes.DoNotApplyColumnBanding; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleBandedColumnsCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleBandedColumn; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleBandedColumnsCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleBandedColumnDescription; } }
	}
	#endregion
	#region ChangeBandedTableLookCommand
	public abstract class ChangeBandedTableLookCommand : ChangeTableLookCommand {
		protected ChangeBandedTableLookCommand(IRichEditControl control)
			: base(control) {
		}
		protected override void ApplyTableLook(ICommandUIState state, TableCell startCell, TableCell endCell) {
			if (startCell != null && endCell != null && startCell.Table == endCell.Table) {
				if (!state.Checked)
					startCell.Table.TableLook |= TableLook;
				else
					startCell.Table.TableLook &= ~TableLook;
			}
		}
		protected override void ChangeState(ICommandUIState state, TableCell startCell, TableCell endCell) {
			if (startCell != null && endCell != null && startCell.Table == endCell.Table) {
				if (!startCell.Table.TableLook.HasFlag(TableLook))
					state.Checked = true;
				else
					state.Checked = false;
			}
		}
	}
	#endregion
	#region ChangeTableLookCommand
	public abstract class ChangeTableLookCommand : SelectionBasedPropertyChangeCommandBase {
		private TableLookTypes tableLook = TableLookTypes.None;
		protected ChangeTableLookCommand(IRichEditControl control)
			: base(control) {
		}
		protected virtual TableLookTypes TableLook { get { return tableLook; } set { tableLook = value; } }
		protected virtual void ApplyTableLook(ICommandUIState state, TableCell startCell, TableCell endCell) {
			if (startCell != null && endCell != null && startCell.Table == endCell.Table) {
				if (state.Checked)
					startCell.Table.TableLook |= TableLook;
				else
					startCell.Table.TableLook &= ~TableLook;
			}
		}
		protected internal override DocumentModelChangeActions ChangeProperty(DocumentModelPosition start, DocumentModelPosition end, ICommandUIState state) {
			if (state != null) {
				state.Checked = !state.Checked;
				ParagraphCollection paragraphs = this.Control.InnerDocumentServer.DocumentModel.ActivePieceTable.Paragraphs;
				TableCell startCell = paragraphs[start.ParagraphIndex].GetCell();
				TableCell endCell = paragraphs[end.ParagraphIndex].GetCell();
				ApplyTableLook(state, startCell, endCell);
			}
			return DocumentModelChangeActions.None;
		}
		protected virtual void ChangeState(ICommandUIState state, TableCell startCell, TableCell endCell) {
			if (startCell != null && endCell != null && startCell.Table == endCell.Table) {
				if (startCell.Table.TableLook.HasFlag(TableLook))
					state.Checked = true;
				else
					state.Checked = false;
			}
		}
		public override void UpdateUIState(ICommandUIState state) {
			if (DocumentModel.Selection.IsWholeSelectionInOneTable()) {
				state.Enabled = true;
				UpdateUIStateViaService(state);
				UpdateUIStateCore(state);
			}
			else {
				UpdateUIStateViaService(state);			
				state.Enabled = false;
			}
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			if (state != null) {
				DocumentModel model = this.Control.InnerDocumentServer.DocumentModel;
				RunInfo interval = model.Selection.Interval;
				DocumentModelPosition start = interval.Start;
				DocumentModelPosition end = interval.End;
				ParagraphCollection paragraphs = model.ActivePieceTable.Paragraphs;
				TableCell startCell = paragraphs[start.ParagraphIndex].GetCell();
				TableCell endCell = paragraphs[end.ParagraphIndex].GetCell();
				ChangeState(state, startCell, endCell);
			}
		}
	}
	#endregion
}
