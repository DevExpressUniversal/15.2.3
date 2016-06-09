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
using System.IO;
using System.Linq;
using System.Text;
using DevExpress.Office.History;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Model.History;
#if WPF
using DevExpress.Xpf.RichEdit;
#else
using DevExpress.XtraRichEdit;
#endif
namespace DevExpress.XtraRichEdit.Commands {
	#region CommentsCommandBase (abstract class)
	public abstract class CommentsCommandBase : RichEditMenuItemSimpleCommand {
		protected CommentsCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected PieceTable MainPieceTable { get { return DocumentModel.MainPieceTable; } }
		protected int CommentsCount {
			get {
				if (MainPieceTable.Comments != null)
					return MainPieceTable.Comments.Count;
				else
					return 0;
			}
		}
		protected int FindActiveCommentIndex() {
			for (int i = 0; i < CommentsCount; i++)
				if (MainPieceTable.Comments[i].Content.PieceTable == ActivePieceTable)
					return i;
			return 0;
		}
		protected Comment FindActiveComment(bool isDeletedComment) {
			for (int i = 0; i < CommentsCount; i++)
				if (MainPieceTable.Comments[i].Content.PieceTable == ActivePieceTable) {
					if ((MainPieceTable.Comments[i].ParentComment == null) || isDeletedComment)
						return MainPieceTable.Comments[i];
					return MainPieceTable.Comments[i].ParentComment;
				}
			return null;
		}
		protected Section CalculateCommentSection(DocumentLogPosition position) {
			SectionIndex sectionIndex = DocumentModel.FindSectionIndex(position);
			return DocumentModel.Sections[sectionIndex];
		}
		protected void ChangeActivePieceTable(PieceTable pieceTable, Section section) {
			ChangeActivePieceTableCommand command = new ChangeActivePieceTableCommand(Control, pieceTable, section, 0);
			command.Execute();
		}
		protected DocumentLogPosition CalculateCommentEnd(int index) {
			Comment comment = MainPieceTable.Comments[index];
			return comment.End;
		}
		protected Section CalculateSection(DocumentLogPosition logPosition) {
			ParagraphIndex paragraphIndex = DocumentModel.MainPieceTable.FindParagraphIndex(logPosition);
			SectionIndex sectionIndex = DocumentModel.MainPieceTable.LookupSectionIndexByParagraphIndex(paragraphIndex);
			return DocumentModel.Sections[sectionIndex];
		}
		protected void ChangeActivePieceTableViaHistory(DocumentLogPosition position) {
			ChangePieceTableHistoryItem item = new ChangePieceTableHistoryItem(ActivePieceTable, Control);
			item.NewPieceTable = MainPieceTable;
			item.Section = CalculateSection(position);
			DocumentModel.History.Add(item);
			item.Execute();
		}
		protected override void UpdateUIStateCore(DevExpress.Utils.Commands.ICommandUIState state) {
			CheckExecutedAtUIThread();
			ApplyCommandRestrictionOnEditableControl(state, Options.DocumentCapabilities.Comments);
			ApplyDocumentProtectionToSelectedCharacters(state);
		}
	}
	#endregion
	#region NewCommentCommand
	public class NewCommentCommand : CommentsCommandBase {
		public NewCommentCommand(IRichEditControl control)
			: base(control) {
		}
		#region Property
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("NewCommentCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.NewComment; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("NewCommentCommandMenuCaptionStringId")]
#endif
		public override Localization.XtraRichEditStringId MenuCaptionStringId { get { return Localization.XtraRichEditStringId.MenuCmd_NewComment; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("NewCommentCommandDescriptionStringId")]
#endif
		public override Localization.XtraRichEditStringId DescriptionStringId { get { return Localization.XtraRichEditStringId.MenuCmd_NewCommentDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("NewCommentCommandImageName")]
#endif
		public override string ImageName { get { return "NewComment"; } }
		#endregion
		protected internal override void ExecuteCore() {
			DocumentModel.BeginUpdate();
			try {
				List<SelectionItem> selectionItems = DocumentModel.Selection.Items;
				DocumentLogPosition start = selectionItems[0].NormalizedStart;
				DocumentLogPosition end = selectionItems[selectionItems.Count - 1].NormalizedEnd;
				Comment comment = CreateComment(start, end);
				ChangeActivePieceTable(comment.Content.PieceTable, CalculateCommentSection(comment.Start));
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		Comment CreateComment(DocumentLogPosition start, DocumentLogPosition end) {
			string commentAuthor = DocumentModel.CommentOptions.Author;
			DateTime commentDate = DateTime.Now;
			CommentContentType commentContent = new CommentContentType(DocumentModel);
			if (ActivePieceTable.IsComment) {
				return CreateNestedComment(commentAuthor, commentDate, commentContent); 
			}
			return DocumentModel.MainPieceTable.CreateComment(start, end - start, commentAuthor, commentDate, commentContent);
		}
		Comment CreateNestedComment(string commentAuthor, DateTime commentDate, CommentContentType commentContent) {
			Comment parentComment = FindActiveComment(false);
			DocumentLogPosition end = parentComment.End;
			DocumentLogPosition start = parentComment.Start;
			return DocumentModel.MainPieceTable.CreateComment(start, end - start, commentAuthor, commentDate, parentComment, commentContent);
		}
		protected override void UpdateUIStateCore(DevExpress.Utils.Commands.ICommandUIState state) {
			if (DocumentModel.ActivePieceTable.IsMain || DocumentModel.ActivePieceTable.IsComment)
				state.Enabled = true;
			else
				state.Enabled = false;
		}
	}
	#endregion
	#region DeleteCommentsCommandBase (abstract class)
	public abstract class DeleteCommentsCommandBase : CommentsCommandBase {
		protected DeleteCommentsCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected void SetSelection(DocumentLogPosition logPosition) {
			DocumentModel.Selection.Start = logPosition;
			DocumentModel.Selection.End = logPosition;
		}
		protected override void UpdateUIStateCore(DevExpress.Utils.Commands.ICommandUIState state) {
			if ((DocumentModel.ActivePieceTable.IsMain || DocumentModel.ActivePieceTable.IsComment) && DocumentModel.CommentOptions.Visibility == RichEditCommentVisibility.Visible)
				state.Enabled = true;
			else
				state.Enabled = false;
		}
		protected void DeleteNestedComments(Comment comment) {
			CommentsHelper helper = new CommentsHelper(MainPieceTable);
			helper.DeleteNestedComments(comment);
		}
	}
	#endregion
	#region DeleteCommentCommand
	public class DeleteCommentCommand : DeleteCommentsCommandBase {
		public DeleteCommentCommand(IRichEditControl control)
			: base(control) {
		}
		#region Property
		public override RichEditCommandId Id { get { return RichEditCommandId.DeleteCommentsPlaceholder; } }
		public override Localization.XtraRichEditStringId MenuCaptionStringId { get { return Localization.XtraRichEditStringId.MenuCmd_DeleteComment; } }
		public override Localization.XtraRichEditStringId DescriptionStringId { get { return Localization.XtraRichEditStringId.MenuCmd_DeleteCommentDescription; } }
		public override string ImageName { get { return "DeleteComment"; } }
		#endregion
		protected internal override void ExecuteCore() { }
	}
	#endregion
	#region DeleteOneCommentCommand
	public class DeleteOneCommentCommand : DeleteCommentsCommandBase {
		public DeleteOneCommentCommand(IRichEditControl control)
			: base(control) {
		}
		#region Property
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("DeleteOneCommentCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.DeleteOneComment; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("DeleteOneCommentCommandMenuCaptionStringId")]
#endif
		public override Localization.XtraRichEditStringId MenuCaptionStringId { get { return Localization.XtraRichEditStringId.MenuCmd_DeleteOneComment; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("DeleteOneCommentCommandDescriptionStringId")]
#endif
		public override Localization.XtraRichEditStringId DescriptionStringId { get { return Localization.XtraRichEditStringId.MenuCmd_DeleteOneCommentDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("DeleteOneCommentCommandImageName")]
#endif
		public override string ImageName { get { return "DeleteComment"; } }
		#endregion
		protected internal override void ExecuteCore() {
			DocumentModel.BeginUpdate();
			try {
				if (CommentsCount > 0) {
					Comment comment = FindActiveComment(true);
					DocumentLogPosition commentEnd = comment.End;
					DeleteNestedComments(comment);
					ChangeActivePieceTableViaHistory(commentEnd);
					MainPieceTable.DeleteComment(comment.Index);
					SetSelection(commentEnd);
				}
			}
			finally {
				DocumentModel.EndUpdate();
			}
			base.Execute();
		}
		protected override void UpdateUIStateCore(DevExpress.Utils.Commands.ICommandUIState state) {
			base.UpdateUIStateCore(state);
			if (ActivePieceTable.IsComment && DocumentModel.CommentOptions.Visibility == RichEditCommentVisibility.Visible)
				state.Enabled = true;
			else
				state.Enabled = false;
		}
	}
	#endregion
	#region DeleteAllCommentsShownCommand
	public class DeleteAllCommentsShownCommand : DeleteCommentsCommandBase {
		public DeleteAllCommentsShownCommand(IRichEditControl control)
			: base(control) {
		}
		#region Property
		public override RichEditCommandId Id { get { return RichEditCommandId.DeleteAllCommentsShown; } }
		public override Localization.XtraRichEditStringId MenuCaptionStringId { get { return Localization.XtraRichEditStringId.MenuCmd_DeleteAllCommentsShown; } }
		public override Localization.XtraRichEditStringId DescriptionStringId { get { return Localization.XtraRichEditStringId.MenuCmd_DeleteAllCommentsShownDescription; } }
		public override string ImageName { get { return String.Empty; } }
		public List<Comment> VisibleComments { get { return DocumentModel.CreateVisibleComments(); } } 
		#endregion
		protected internal override void ExecuteCore() {
			DocumentModel.BeginUpdate();
			try {
				DocumentLogPosition endPosition = DocumentModel.Selection.End;
				if (ActivePieceTable.IsComment) {
					int commentIndex = FindActiveCommentIndex();
					endPosition = CalculateCommentEnd(commentIndex);
					ChangeActivePieceTableViaHistory(endPosition);
				}
				for (int i = VisibleComments.Count - 1; i >= 0; i--) {
					Comment comment = VisibleComments[i];
					DeleteNestedComments(comment);
					MainPieceTable.DeleteComment(comment.Index);
				}
				SetSelection(endPosition);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected override void UpdateUIStateCore(DevExpress.Utils.Commands.ICommandUIState state) {
			base.UpdateUIStateCore(state);
			if (VisibleComments.Count < CommentsCount)
				state.Enabled = true;
			else
				state.Enabled = false;
		}
	}
	#endregion
	#region DeleteAllCommentsCommand
	public class DeleteAllCommentsCommand : DeleteCommentsCommandBase {
		public DeleteAllCommentsCommand(IRichEditControl control)
			: base(control) {
		}
		#region Property
		public override RichEditCommandId Id { get { return RichEditCommandId.DeleteAllComments; } }
		public override Localization.XtraRichEditStringId MenuCaptionStringId { get { return Localization.XtraRichEditStringId.MenuCmd_DeleteAllComments; } }
		public override Localization.XtraRichEditStringId DescriptionStringId { get { return Localization.XtraRichEditStringId.MenuCmd_DeleteAllComments; } }
		public override string ImageName { get { return String.Empty; } }
		#endregion
		protected internal override void ExecuteCore() {
			DocumentModel.BeginUpdate();
			try {
			if (CommentsCount > 0) {
					DocumentLogPosition endPosition = DocumentModel.Selection.End;
					if (ActivePieceTable.IsComment) {
						int commentIndex = FindActiveCommentIndex();
						endPosition = CalculateCommentEnd(commentIndex);
						ChangeActivePieceTableViaHistory(endPosition);
					}
					for (int i = CommentsCount - 1; i >= 0; i--) {
						MainPieceTable.DeleteComment(i);
					}
					SetSelection(endPosition);
				}
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
	}
	#endregion
	#region PreviousNextCommentCommandBase(abstract class)
	public abstract class PreviousNextCommentCommandBase : CommentsCommandBase {
		protected PreviousNextCommentCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected internal abstract bool CanNavigateFrom(int commentIndex);
		protected internal abstract int CalculateNextCommentIndex(int commentIndex);
		protected internal override void ExecuteCore() {
			if (CommentsCount > 0) {
				int commentIndex = FindActiveCommentIndex();
				if (!CanNavigateFrom(commentIndex))
					return;
				int nextIndex = CalculateNextCommentIndex(commentIndex);
				Comment nextComment = MainPieceTable.Comments[nextIndex];
				ChangeActivePieceTable(nextComment.Content.PieceTable, CalculateCommentSection(nextComment.Start));
				Control.ScrollToCaret();
			}
		}
		protected override void UpdateUIStateCore(DevExpress.Utils.Commands.ICommandUIState state)
		{
			if (DocumentModel.ActivePieceTable.IsComment && DocumentModel.CommentOptions.Visibility == RichEditCommentVisibility.Visible && CanNavigateFrom(FindActiveCommentIndex()))
				state.Enabled = true;
			else
				state.Enabled = false;
		}
	}
	#endregion
	#region PreviousCommentCommand
	public class PreviousCommentCommand : PreviousNextCommentCommandBase {
		public PreviousCommentCommand(IRichEditControl control)
			: base(control) {
		}
		#region Property
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("PreviousCommentCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.PreviousComment; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("PreviousCommentCommandMenuCaptionStringId")]
#endif
		public override Localization.XtraRichEditStringId MenuCaptionStringId { get { return Localization.XtraRichEditStringId.MenuCmd_PreviousComment; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("PreviousCommentCommandDescriptionStringId")]
#endif
		public override Localization.XtraRichEditStringId DescriptionStringId { get { return Localization.XtraRichEditStringId.MenuCmd_PreviousCommentDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("PreviousCommentCommandImageName")]
#endif
		public override string ImageName { get { return "PreviousComment"; } }
		#endregion
		protected internal override bool CanNavigateFrom(int commentIndex) {
			return commentIndex > 0;
		}
		protected internal override int CalculateNextCommentIndex(int commentIndex) {
			return commentIndex - 1;
		}
	}
	#endregion
	#region NextCommentCommand
	public class NextCommentCommand : PreviousNextCommentCommandBase {
		public NextCommentCommand(IRichEditControl control)
			: base(control) {
		}
		#region Property
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("NextCommentCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.NextComment; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("NextCommentCommandMenuCaptionStringId")]
#endif
		public override Localization.XtraRichEditStringId MenuCaptionStringId { get { return Localization.XtraRichEditStringId.MenuCmd_NextComment; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("NextCommentCommandDescriptionStringId")]
#endif
		public override Localization.XtraRichEditStringId DescriptionStringId { get { return Localization.XtraRichEditStringId.MenuCmd_NextCommentDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("NextCommentCommandImageName")]
#endif
		public override string ImageName { get { return "NextComment"; } }
		#endregion
		protected internal override bool CanNavigateFrom(int commentIndex) {
			return commentIndex < CommentsCount - 1;
		}
		protected internal override int CalculateNextCommentIndex(int commentIndex) {
			return commentIndex + 1;
		}
	}
	#endregion
}
