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
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Export;
using DevExpress.XtraRichEdit.Export.Rtf;
namespace DevExpress.XtraRichEdit.Commands {
	#region DragMoveContentCommand
	public class DragMoveContentCommand : DragMoveContentCommandBase {
		public DragMoveContentCommand(IRichEditControl control, DocumentModelPosition targetPosition)
			: base(control, targetPosition) {
		}
		protected internal override bool CopyContent { get { return false; } }
	}
	#endregion
	#region DragMoveContentCommandBase (abstract class)
	public abstract class DragMoveContentCommandBase : TransactedMultiCommand {
		CopyAndSaveContentCommand copyCommand;
		SimpleSetSelectionCommand setSelectionCommand;
		Command pasteContentCommand;
		readonly DocumentModelPositionAnchor anchor;
		protected DragMoveContentCommandBase(IRichEditControl control, DocumentModelPosition targetPosition)
			: base(control) {
			setSelectionCommand.Position = targetPosition;
			this.anchor = new DocumentModelPositionAnchor(targetPosition);
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		protected internal override MultiCommandExecutionMode ExecutionMode { get { return MultiCommandExecutionMode.ExecuteAllAvailable; } }
		protected internal override MultiCommandUpdateUIStateMode UpdateUIStateMode { get { return MultiCommandUpdateUIStateMode.EnableIfAnyAvailable; } }
		protected internal abstract bool CopyContent { get; }
		protected Command PasteContentCommand { get { return pasteContentCommand; } }
		protected DocumentModelPositionAnchor Anchor { get { return anchor; } }
		#endregion
		protected internal override void CreateCommands() {
			this.copyCommand = CreateCopyContentCommand();
			if (copyCommand != null)
				Commands.Add(copyCommand);
			if (!CopyContent)
				Commands.Add(new DeleteNonEmptySelectionCommand(Control));
			this.setSelectionCommand = new SimpleSetSelectionCommand(Control);
			Commands.Add(setSelectionCommand);
			this.pasteContentCommand = CreatePasteContentCommand();
			Commands.Add(pasteContentCommand);
		}
		protected internal virtual CopyAndSaveContentCommand CreateCopyContentCommand() {
			return DocumentModel.CommandsCreationStrategy.CreateCopyContentCommand(Control);
		}
		protected internal virtual Command CreatePasteContentCommand() {
			return DocumentModel.CommandsCreationStrategy.CreatePasteContentCommand(Control, copyCommand);
		}
		protected internal override void ForceExecuteCore(ICommandUIState state) {
			DocumentModel.InternalAPI.RegisterAnchor(anchor);
			try {
				base.ForceExecuteCore(state);
			}
			finally {
				DocumentModel.InternalAPI.UnregisterAnchor(anchor);
			}
		}
		public override void UpdateUIState(ICommandUIState state) {
			UpdateUIStateCore(state, false);
		}
		protected void UpdateUIStateCore(ICommandUIState state, bool canDropOnSelection) {
			if (!canDropOnSelection) {
				Selection selection = DocumentModel.Selection;
				int offset = anchor.Position.LogPosition - selection.NormalizedStart;
				bool canExecute = (offset < 0 || offset > selection.Length || selection.Length <= 0);
				if (!canExecute) {
					state.Enabled = false;
					state.Visible = true;
					return;
				}
			}
			base.UpdateUIState(state);
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Commands.Internal {
	#region CopyAndSaveContentCommand
	public class CopyAndSaveContentCommand : RichEditCaretBasedCommand {
		string rtfText;
		string suppressStoreImageSizeCollection;
		public CopyAndSaveContentCommand(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		public string RtfText { get { return rtfText; } }
		public string SuppressStoreImageSizeCollection { get { return suppressStoreImageSizeCollection; } }
		protected internal override void ExecuteCore() {
			CopySelectionManager manager = Control.InnerDocumentServer.CreateCopySelectionManager();
			CopySelectedContent(manager);
		}
		protected internal virtual void CopySelectedContent(CopySelectionManager manager) {
			Selection selection = DocumentModel.Selection;
			RtfDocumentExporterOptions options = new RtfDocumentExporterOptions();
			options.ExportFinalParagraphMark = ExportFinalParagraphMark.Never;
			rtfText = manager.GetRtfText(selection.PieceTable, selection.GetSortedSelectionCollection(), options, true, true);
			suppressStoreImageSizeCollection = manager.GetSuppressStoreImageSizeCollection(selection.PieceTable, selection.GetSortedSelectionCollection());
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Checked = false;
			state.Enabled = true;
			state.Visible = true;
		}
	}
	#endregion
	#region PasteSavedContentCommand
	public class PasteSavedContentCommand : PasteRtfTextCommand {
		readonly CopyAndSaveContentCommand copyCommand;
		public PasteSavedContentCommand(IRichEditControl control, CopyAndSaveContentCommand copyCommand)
			: base(control) {
			Guard.ArgumentNotNull(copyCommand, "copyCommand");
			this.copyCommand = copyCommand;
			CreateInnerCommand();
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		protected internal override PieceTablePasteContentConvertedToDocumentModelCommandBase CreateInnerCommandCore() {
			if (copyCommand == null)
				return null;
			return new PieceTablePasteSavedContentCommand(ActivePieceTable, copyCommand);
		}
	}
	#endregion
	public class PieceTablePasteSavedContentCommand : PieceTablePasteRtfTextCommand {
		readonly CopyAndSaveContentCommand copyCommand;
		public PieceTablePasteSavedContentCommand(PieceTable pieceTable, CopyAndSaveContentCommand copyCommand)
			: base(pieceTable) {
			Guard.ArgumentNotNull(copyCommand, "copyCommand");
			this.copyCommand = copyCommand;
		}
		protected internal override ClipboardStringContent GetContent() {
			return new ClipboardStringContent(copyCommand.RtfText);
		}
		protected internal override string GetAdditionalContentString() {
			return copyCommand.SuppressStoreImageSizeCollection;
		}
		protected internal override bool IsDataAvailable() {
			return !String.IsNullOrEmpty(copyCommand.RtfText);
		}
	}
	#region SimpleSetSelectionCommand
	public class SimpleSetSelectionCommand : RichEditMenuItemSimpleCommand {
		DocumentModelPosition pos;
		public SimpleSetSelectionCommand(IRichEditControl control)
			: base(control) {
		}
		public DocumentModelPosition Position { get { return pos; } set { pos = value; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		protected internal override void ExecuteCore() {
			DocumentModel documentModel = DocumentModel;
			Selection selection = documentModel.Selection;
			documentModel.BeginUpdate();
			try {
				selection.Start = pos.LogPosition;
				selection.End = pos.LogPosition;
			}
			finally {
				documentModel.EndUpdate();
			}
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Checked = false;
			state.Enabled = true;
			state.Visible = true;
		}
	}
	#endregion
}
