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
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Utils;
namespace DevExpress.XtraRichEdit.Commands {
	#region CreateBookmarkCommand
	public class CreateBookmarkCommand : RichEditMenuItemSimpleCommand {
		string bookmarkName = String.Empty;
		public CreateBookmarkCommand(IRichEditControl control, string bookmarkName)
			: base(control) {
			Guard.ArgumentNotNull(bookmarkName, "bookmarkName");
			this.bookmarkName = bookmarkName;
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("CreateBookmarkCommandBookmarkName")]
#endif
		public string BookmarkName { get { return bookmarkName; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("CreateBookmarkCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_CreateBookmark; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("CreateBookmarkCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_CreateBookmarkDescription; } }
		protected internal override void ExecuteCore() {
			List<SelectionItem> selectionItems = DocumentModel.Selection.Items;
			DocumentLogPosition start = selectionItems[0].NormalizedStart;
			DocumentLogPosition end = selectionItems[selectionItems.Count - 1].NormalizedEnd;
			int length = end - start;
			CreateBookmarkCoreCommand command = new CreateBookmarkCoreCommand(Control, start, length, bookmarkName);
			command.Execute();
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			CheckExecutedAtUIThread();
			ApplyCommandRestrictionOnEditableControl(state, Options.DocumentCapabilities.Bookmarks);
			ApplyDocumentProtectionToSelectedCharacters(state);
		}
	}
	#endregion
	#region SelectBookmarkCommand
	public class SelectBookmarkCommand : RichEditSelectionCommand {
		Bookmark bookmark;
		public SelectBookmarkCommand(IRichEditControl control, Bookmark bookmark)
			: base(control) {
			Guard.ArgumentNotNull(bookmark, "bookmark");
			this.bookmark = bookmark;
		}
		#region Properties
public Bookmark Bookmark { get { return bookmark; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_SelectBookmark; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_SelectBookmarkDescription; } }
		protected internal override bool TryToKeepCaretX { get { return false; } }
		protected internal override bool TreatStartPositionAsCurrent { get { return false; } }
		protected internal override bool ExtendSelection { get { return true; } }
		protected internal override DocumentLayoutDetailsLevel UpdateCaretPositionBeforeChangeSelectionDetailsLevel { get { return DocumentLayoutDetailsLevel.Box; } }
		#endregion
		protected override void UpdateUIStateCore(ICommandUIState state) {
			CheckExecutedAtUIThread();
			ApplyCommandsRestriction(state, Options.DocumentCapabilities.Bookmarks);
			state.Visible = true;
		}
		protected internal override bool CanChangePosition(DocumentModelPosition pos) {
			return true;
		}
		protected internal override DocumentLogPosition ChangePosition(DocumentModelPosition pos) {
			return pos.LogPosition;
		}
		protected internal override void ChangeSelection(Selection selection) {
			selection.ClearMultiSelection(); 
			selection.Start = Bookmark.Start;
			selection.End = bookmark.End;
		}
		protected internal override RichEditCommand CreateEnsureCaretVisibleVerticallyCommand() {
			RichEditCommand result = base.CreateEnsureCaretVisibleVerticallyCommand();
			EnsureCaretVisibleVerticallyCommand command = result as EnsureCaretVisibleVerticallyCommand;
			if (command != null)
				command.RelativeCaretPosition = 0.3f;
			return result;
		}
	}
	#endregion
	#region DeleteBookmarkCommand
	public class DeleteBookmarkCommand : RichEditMenuItemSimpleCommand {
		Bookmark bookmark;
		public DeleteBookmarkCommand(IRichEditControl control, Bookmark bookmark)
			: base(control) {
			Guard.ArgumentNotNull(bookmark, "bookmark");
			this.bookmark = bookmark;
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("DeleteBookmarkCommandBookmark")]
#endif
		public Bookmark Bookmark { get { return bookmark; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("DeleteBookmarkCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_DeleteBookmark; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("DeleteBookmarkCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_DeleteBookmarkDescription; } }
		protected internal override void ExecuteCore() {
			PieceTable pieceTable = bookmark.PieceTable;
			int index = pieceTable.Bookmarks.IndexOf(bookmark);
			if (index < 0)
				Exceptions.ThrowArgumentException("bookmark", bookmark);
			pieceTable.DeleteBookmark(index);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			CheckExecutedAtUIThread();
			ApplyCommandRestrictionOnEditableControl(state, Options.DocumentCapabilities.Bookmarks);
			ApplyDocumentProtectionToSelectedCharacters(state);
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Commands {
	#region CreateBookmarkCoreCommand
	public class CreateBookmarkCoreCommand : RichEditMenuItemSimpleCommand {
		readonly DocumentLogPosition position;
		readonly string bookmarkName;
		readonly int length;
		public CreateBookmarkCoreCommand(IRichEditControl control, DocumentLogPosition position, int length, string bookmarkName)
			: base(control) {
			Guard.ArgumentNotNull(bookmarkName, "bookmarkName");
			this.bookmarkName = bookmarkName;
			this.position = position;
			this.length = length;
		}
		public string BookmarkName { get { return bookmarkName; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		protected internal override void ExecuteCore() {
			ActivePieceTable.CreateBookmark(position, length, bookmarkName);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			CheckExecutedAtUIThread();
			ApplyCommandRestrictionOnEditableControl(state, Options.DocumentCapabilities.Bookmarks);
			ApplyDocumentProtectionToSelectedCharacters(state);
		}
	}
	#endregion
}
