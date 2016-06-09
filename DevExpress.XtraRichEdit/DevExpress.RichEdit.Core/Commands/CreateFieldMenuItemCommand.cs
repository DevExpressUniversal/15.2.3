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
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Internal;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Commands {
	#region CreateFieldCommand
	public class CreateFieldCommand : RichEditMenuItemSimpleCommand {
		readonly string NewEmptyFieldText = "  ";
		public CreateFieldCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("CreateFieldCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.CreateField; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("CreateFieldCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_CreateField; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("CreateFieldCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_CreateFieldDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("CreateFieldCommandImageName")]
#endif
		public override string ImageName { get { return "InsertDataField"; } }
		#endregion
		protected internal override void ExecuteCore() {
			if (DocumentModel.DocumentCapabilities.Fields == DocumentCapability.Disabled)
				return;
			DocumentModel.BeginUpdate();
			try {
				bool forceVisible = GetForceVisible();
				Selection selection = DocumentModel.Selection;
				DocumentLogPosition selectionStart = selection.NormalizedStart;
				bool newEmptyField = false;
				if (DocumentModel.Selection.Length == 0) {
					Debug.Assert(selectionStart == selection.End);
					ActivePieceTable.InsertText(selectionStart, NewEmptyFieldText, forceVisible);
					selection.Start = selectionStart;
					selection.End = selectionStart + NewEmptyFieldText.Length;
					newEmptyField = true;
				}
				CreateField(forceVisible, selection, selectionStart);
				selection.Start = newEmptyField ? selectionStart + 2 : selectionStart + 1;
				selection.End = selection.Start;
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected virtual Field CreateField(bool forceVisible, Selection selection, DocumentLogPosition selectionStart) {
			return ActivePieceTable.CreateField(selectionStart, selection.Length, forceVisible);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			CheckExecutedAtUIThread();
			state.Enabled = IsContentEditable;
			state.Visible = true;
			ApplyDocumentProtectionToSelectedCharacters(state);
		}
	}
	#endregion
	#region FieldBasedRichEditMenuItemSimpleCommand (abstract class)
	public abstract class FieldBasedRichEditMenuItemSimpleCommand : RichEditMenuItemSimpleCommand {
		Field field;
		protected FieldBasedRichEditMenuItemSimpleCommand(IRichEditControl control, Field field)
			: base(control) {
			this.field = field;
		}
		protected internal Field Field { get { return field; } }
		public override void ForceExecute(ICommandUIState state) {
			if (ValidateField())
				base.ForceExecute(state);
		}
		protected internal virtual bool ValidateField() {
			if (field != null)
				return true;
			DocumentLayoutPosition caretPos = ActiveView.CaretPosition.LayoutPosition;
			PieceTable pieceTable = ActivePieceTable;
			ParagraphIndex paragraphIndex = pieceTable.FindParagraphIndex(caretPos.LogPosition);
			Paragraph paragraph = pieceTable.Paragraphs[paragraphIndex];
			RunIndex runIndex;
			pieceTable.FindRunStartLogPosition(paragraph, caretPos.LogPosition, out runIndex);
			this.field = pieceTable.FindFieldByRunIndex(runIndex);
			return field != null;
		}
	}
	#endregion
	#region UpdateFieldCommand
	public class UpdateFieldCommand : FieldBasedRichEditMenuItemSimpleCommand {
		public UpdateFieldCommand(IRichEditControl control)
			: this(control, null) {
		}
		public UpdateFieldCommand(IRichEditControl control, Field field)
			: base(control, field) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("UpdateFieldCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.UpdateField; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("UpdateFieldCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_UpdateField; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("UpdateFieldCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_UpdateFieldDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("UpdateFieldCommandImageName")]
#endif
		public override string ImageName { get { return "UpdateField"; } }
		protected internal override void ExecuteCore() {
			ActivePieceTable.FieldUpdater.UpdateFieldAndNestedFields(Field);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			CheckExecutedAtUIThread();
			state.Enabled = IsContentEditable;
			if (state.Enabled && DocumentModel.FieldOptions.UpdateLockedFields != UpdateLockedFields.Always) {
				ValidateField();
				if (Field != null)
					state.Enabled &= !Field.Locked;
				else
					state.Enabled = false;
			}
			state.Visible = true;
		}
	}
	#endregion
	#region ToggleFieldCodesCommand
	public class ToggleFieldCodesCommand : FieldBasedRichEditMenuItemSimpleCommand {
		public ToggleFieldCodesCommand(IRichEditControl control)
			: this(control, null) {
		}
		public ToggleFieldCodesCommand(IRichEditControl control, Field field)
			: base(control, field) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleFieldCodesCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.ToggleFieldCodes; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleFieldCodesCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleFieldCodes; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleFieldCodesCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleFieldCodesDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleFieldCodesCommandImageName")]
#endif
		public override string ImageName { get { return "ToggleFieldCodes"; } }
		protected internal override void ExecuteCore() {
			ActivePieceTable.ToggleFieldCodesFromCommandOrApi(Field);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			CheckExecutedAtUIThread();
			state.Enabled = IsContentEditable;
			state.Visible = true;
		}
	}
	#endregion
	#region ToggleFieldCodesCommand
	public class ToggleFieldLockedCommand : FieldBasedRichEditMenuItemSimpleCommand {
		public ToggleFieldLockedCommand(IRichEditControl control)
			: this(control, null) {
		}
		public ToggleFieldLockedCommand(IRichEditControl control, Field field)
			: base(control, field) {
		}
		public override RichEditCommandId Id { get { return RichEditCommandId.ToggleFieldLockedCommand; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleFieldLocked; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleFieldLockedDescription; } }
		public override string ImageName { get { return String.Empty; } }
		protected internal override void ExecuteCore() {
			ActivePieceTable.ToggleFieldLocked(Field);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			CheckExecutedAtUIThread();
			state.Enabled = IsContentEditable;
			state.Visible = true;
		}
	}
	#endregion
	#region LockFieldCommand
	public class LockFieldCommand : FieldBasedRichEditMenuItemSimpleCommand {
		public LockFieldCommand(IRichEditControl control)
			: this(control, null) {
		}
		public LockFieldCommand(IRichEditControl control, Field field)
			: base(control, field) {
		}
		public override RichEditCommandId Id { get { return RichEditCommandId.LockFieldCommand; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_LockField; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_LockFieldDescription; } }
		public override string ImageName { get { return String.Empty; } }
		protected internal override void ExecuteCore() {
			if(!Field.Locked)
				ActivePieceTable.ToggleFieldLocked(Field);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			CheckExecutedAtUIThread();
			state.Enabled = IsContentEditable;
			if (state.Enabled && DocumentModel.FieldOptions.UpdateLockedFields != UpdateLockedFields.Always) {
				ValidateField();
				if (Field != null)
					state.Enabled &= !Field.Locked;
				else
					state.Enabled = false;
			}
			state.Visible = true;
		}
	}
	#endregion
	#region UnlockFieldCommand
	public class UnlockFieldCommand : FieldBasedRichEditMenuItemSimpleCommand {
		public UnlockFieldCommand(IRichEditControl control)
			: this(control, null) {
		}
		public UnlockFieldCommand(IRichEditControl control, Field field)
			: base(control, field) {
		}
		public override RichEditCommandId Id { get { return RichEditCommandId.UnlockFieldCommand; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_UnlockField; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_UnlockFieldDescription; } }
		public override string ImageName { get { return String.Empty; } }
		protected internal override void ExecuteCore() {
			if (Field.Locked)
				ActivePieceTable.ToggleFieldLocked(Field);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			CheckExecutedAtUIThread();
			state.Enabled = IsContentEditable;
			if (state.Enabled) {
				ValidateField();
				if (Field != null)
					state.Enabled &= Field.Locked;
				else
					state.Enabled = false;
			}
			state.Visible = true;
		}
	}
	#endregion
}
