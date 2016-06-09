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
using System.Diagnostics;
using System.Text;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Model.History;
using DevExpress.XtraRichEdit.Commands.Internal;
using System.ComponentModel;
#if !SL
using System.Windows.Forms;
#else
using DevExpress.Data;
#endif
namespace DevExpress.XtraRichEdit.Commands {
	#region CreateHyperlinkCommand
	public class CreateHyperlinkCommand : RichEditMenuItemSimpleCommand {
		#region Fields
		DocumentLogPosition logPosition;
		int length;
		HyperlinkInfo hyperlinkInfo;
		#endregion
		public CreateHyperlinkCommand(IRichEditControl control, HyperlinkInfo hyperlinkInfo, DocumentLogPosition logPosition, int length)
			: base(control) {
			Guard.ArgumentNotNull(hyperlinkInfo, "hyperlinkInfo");
			this.hyperlinkInfo = hyperlinkInfo;
			this.logPosition = logPosition;
			this.length = length;
		}
		public CreateHyperlinkCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public HyperlinkInfo HyperlinkInfo { get { return hyperlinkInfo; } set { hyperlinkInfo = value; } }
		public DocumentLogPosition LogPosition { get { return logPosition; } set { logPosition = value; } }
		public int Length { get { return length; } set { length = value; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_CreateHyperlink; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_CreateHyperlinkDescription; } }
		#endregion
		protected internal override void ExecuteCore() {
			DocumentModel.BeginUpdate();
			try {
				bool isSplit = SplitTable(LogPosition, LogPosition + Length);
				if (isSplit)
					Length++;
				ActivePieceTable.CreateHyperlink(LogPosition, Length, HyperlinkInfo, GetForceVisible());
				DocumentModel.Selection.Start = LogPosition;
				DocumentModel.Selection.End = LogPosition;
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal virtual bool SplitTable(DocumentLogPosition start, DocumentLogPosition end) {
			TableCell startCell = ActivePieceTable.FindParagraph(start).GetCell();
			TableCell endCell = ActivePieceTable.FindParagraph(end).GetCell();
			if (startCell == null || startCell == endCell)
				return false;
			ActivePieceTable.SplitTable(startCell.Table.Index, startCell.Row.IndexInTable, GetForceVisible());
			return true;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			CheckExecutedAtUIThread();
			ApplyCommandRestrictionOnEditableControl(state, Options.DocumentCapabilities.Hyperlinks);
			ApplyDocumentProtectionToSelectedCharacters(state);
		}
	}
	#endregion
	#region InsertHyperlinkCommand
	public class InsertHyperlinkCommand : TransactedMultiCommand {
		public InsertHyperlinkCommand(IRichEditControl control)
			: base(control) {
		}
		public string Text { get { return InsertTextCommand.Text; } set { InsertTextCommand.Text = value; } }
		public HyperlinkInfo HyperlinkInfo { get { return CreateHyperlinkCommand.HyperlinkInfo; } set { CreateHyperlinkCommand.HyperlinkInfo = value; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertHyperlink; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertHyperlinkDescription; } }
		protected internal override MultiCommandExecutionMode ExecutionMode { get { return MultiCommandExecutionMode.ExecuteAllAvailable; } }
		protected internal override MultiCommandUpdateUIStateMode UpdateUIStateMode { get { return MultiCommandUpdateUIStateMode.EnableIfAllAvailable; } }
		protected InsertTextCommand InsertTextCommand { get { return (InsertTextCommand)Commands[0]; } }
		protected CreateHyperlinkCommand CreateHyperlinkCommand { get { return (CreateHyperlinkCommand)Commands[1]; } }
		protected internal override void ForceExecuteCore(ICommandUIState state) {
			CreateHyperlinkCommand.LogPosition = DocumentModel.Selection.NormalizedStart;
			CreateHyperlinkCommand.Length = Text.Length;
			base.ForceExecuteCore(state);
		}
		protected internal override void CreateCommands() {
			Commands.Add(new InsertTextCommand(Control));
			Commands.Add(new CreateHyperlinkCommand(Control));
		}
	}
	#endregion
	#region HyperlinkFieldCommandBase (abstract class)
	public abstract class HyperlinkFieldCommandBase : FieldBasedRichEditMenuItemSimpleCommand {
		protected HyperlinkFieldCommandBase(IRichEditControl control)
			: base(control, null) {
		}
		protected HyperlinkFieldCommandBase(IRichEditControl control, Field field)
			: base(control, field) {
			EnsureFieldIsHyperlink(Field);
		}
		protected internal virtual void EnsureFieldIsHyperlink(Field field) {
			if (!ActivePieceTable.HyperlinkInfos.IsHyperlink(field.Index))
				Exceptions.ThrowArgumentException("field", field);
		}
		protected internal override bool ValidateField() {
			if (!base.ValidateField())
				return false;
			return ActivePieceTable.HyperlinkInfos.IsHyperlink(Field.Index);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			CheckExecutedAtUIThread();
			ApplyCommandsRestriction(state, Options.DocumentCapabilities.Hyperlinks);
		}
	}
	#endregion
	#region FollowHyperlinkCommand
	public class FollowHyperlinkCommand : HyperlinkFieldCommandBase {
		public FollowHyperlinkCommand(IRichEditControl control)
			: base(control) {
		}
		public FollowHyperlinkCommand(IRichEditControl control, Field field)
			: base(control, field) {
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_OpenHyperlink; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_OpenHyperlinkDescription; } }
		protected internal override void ExecuteCore() {
			HyperlinkInfo hyperlinkInfo = ActivePieceTable.HyperlinkInfos[Field.Index];
			if (!String.IsNullOrEmpty(hyperlinkInfo.NavigateUri))
				NavigateToExternalUri(hyperlinkInfo);
			else if (!String.IsNullOrEmpty(hyperlinkInfo.Anchor))
				NavigateToBookmark(hyperlinkInfo.Anchor);
			else
				return;
			hyperlinkInfo.Visited = true;
		}
		[System.Security.SecuritySafeCritical]
		protected internal virtual void NavigateToExternalUri(HyperlinkInfo hyperlinkInfo) {
			try {
#if !SL
				string uri = hyperlinkInfo.CreateUrl();
				if (Office.HyperlinkUriHelper.IsRelativePath(uri))
					uri = Office.HyperlinkUriHelper.ConvertRelativePathToAbsolute(hyperlinkInfo.NavigateUri, Control.InnerControl.Options.DocumentSaveOptions.CurrentFileName);
				using (Process process = new Process()) {
					process.StartInfo.FileName = uri;
					process.Start();
				}
#else
				new DevExpress.Xpf.Core.HyperlinkNavigator().Navigate(hyperlinkInfo.CreateUrl(), hyperlinkInfo.Target);
#endif
			}
			catch {
				string message = XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_InvalidNavigateUri);
				Control.ShowWarningMessage(message);
			}
		}
		protected internal virtual void NavigateToBookmark(string anchor) {
			Bookmark bookmark = FindBookmarkByName(anchor);
			NavigateToBookmark(bookmark);
		}
		protected internal void NavigateToBookmark(Bookmark bookmark) {
			if (bookmark != null) {
				if (!Object.ReferenceEquals(bookmark.PieceTable, DocumentModel.ActivePieceTable))
					ChangeActivePieceTable(bookmark);
				SelectBookmark(bookmark);
			}
			else
				MoveToPieceTableStart();
		}
		protected internal virtual void ChangeActivePieceTable(Bookmark bookmark) {
			PieceTable pieceTable = bookmark.PieceTable;
			if (pieceTable.IsMain) {
				DocumentLayoutPosition pos = ActiveView.CaretPosition.LayoutPosition;
				if (pos.IsValid(DocumentLayoutDetailsLevel.Page)) {
					ChangeActivePieceTableCommand command = new ChangeActivePieceTableCommand(Control, pieceTable, null, pos.Page.PageIndex);
					command.Execute();
				}
			}
			else
				MakeHeaderFooterActive(pieceTable);
		}
		protected internal virtual void SelectBookmark(Bookmark bookmark) {
			if (!Object.ReferenceEquals(bookmark.PieceTable, DocumentModel.ActivePieceTable))
				return;
			SelectBookmarkCommand command = new SelectBookmarkCommand(Control, bookmark);
			command.Execute();
		}
		protected internal virtual void MakeHeaderFooterActive(PieceTable pieceTable) {
			SectionHeader sectionHeader = pieceTable.ContentType as SectionHeader;
			if (sectionHeader != null) {
				MakeNearestHeaderActiveCommand command = new MakeNearestHeaderActiveCommand(Control, sectionHeader);
				command.Execute();
				return;
			}
			SectionFooter sectionFooter = pieceTable.ContentType as SectionFooter;
			if (sectionFooter != null) {
				MakeNearestFooterActiveCommand command = new MakeNearestFooterActiveCommand(Control, sectionFooter);
				command.Execute();
				return;
			}
		}
		protected internal virtual void MoveToPieceTableStart() {
			StartOfDocumentCommand command = new StartOfDocumentCommand(Control);
			command.Execute();
		}
		protected internal virtual Bookmark FindBookmarkByName(string name) {
			List<Bookmark> bookmarks = DocumentModel.GetBookmarks();
			int count = bookmarks.Count;
			for (int i = 0; i < count; i++) {
				Bookmark bookmark = bookmarks[i];
				if (StringExtensions.CompareInvariantCultureIgnoreCase(bookmark.Name, name) == 0)
					return bookmark;
			}
			return null;
		}
	}
	#endregion
	#region OpenHyperlinkCommand
	public class OpenHyperlinkCommand : HyperlinkFieldCommandBase {
		public OpenHyperlinkCommand(IRichEditControl control)
			: base(control) {
		}
		public OpenHyperlinkCommand(IRichEditControl control, Field field)
			: base(control, field) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("OpenHyperlinkCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_OpenHyperlink; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("OpenHyperlinkCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_OpenHyperlinkDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("OpenHyperlinkCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.OpenHyperlink; } }
		protected internal override void ExecuteCore() {
			Control.InnerControl.OnHyperlinkClick(Field, false);
		}
	}
	#endregion
	#region ModifyHyperlinkCommandBase (abstract class)
	public abstract class ModifyHyperlinkCommandBase : HyperlinkFieldCommandBase {
		protected ModifyHyperlinkCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected ModifyHyperlinkCommandBase(IRichEditControl control, Field field)
			: base(control, field) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			CheckExecutedAtUIThread();
			ApplyCommandRestrictionOnEditableControl(state, Options.DocumentCapabilities.Hyperlinks);
			ApplyDocumentProtectionToSelectedCharacters(state);
		}
	}
	#endregion
	#region RemoveHyperlinkFieldCommand
	public class RemoveHyperlinkFieldCommand : ModifyHyperlinkCommandBase {
		public RemoveHyperlinkFieldCommand(IRichEditControl control)
			: base(control) {
		}
		public RemoveHyperlinkFieldCommand(IRichEditControl control, Field field)
			: base(control, field) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RemoveHyperlinkFieldCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_RemoveHyperlink; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RemoveHyperlinkFieldCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_RemoveHyperlinkDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RemoveHyperlinkFieldCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.RemoveHyperlink; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RemoveHyperlinkFieldCommandImageName")]
#endif
		public override string ImageName { get { return "Delete_Hyperlink"; } }
		protected internal override void ExecuteCore() {
			ActivePieceTable.DeleteHyperlink(Field);
		}
	}
	#endregion
}
