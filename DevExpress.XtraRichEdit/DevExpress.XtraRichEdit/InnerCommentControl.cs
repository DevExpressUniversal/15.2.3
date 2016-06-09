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
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using DevExpress.XtraRichEdit.Forms;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Services;
using DevExpress.XtraRichEdit.Services.Implementation;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.Office.Services.Implementation;
#if SL || WPF
#else
using DevExpress.XtraBars.Docking;
using DevExpress.XtraRichEdit.Utils;
#endif
#if SL || WPF
namespace DevExpress.Xpf.RichEdit {
#else
namespace DevExpress.XtraRichEdit {
#endif
	#region InnerCommentControl
	[DXToolboxItem(false)]
	public partial class InnerCommentControl : RichEditControl {
		#region Properties
		public ReviewingPaneFormController Controller {get; set;}
		public CommentIdProvider IdProvider { get; set; }
		[DefaultValue(false)]
		public bool DockPanelVisible { get; set; }
		#endregion
		internal void GenerateMainDocumentComments(bool keepHistory) {
			IdProvider = new CommentIdProvider();
			IdProvider.Initialize(RichEditControl.DocumentModel.MainPieceTable.Comments);
			ReviewingPaneFormController controller = CreateReviewingPaneFormController();
			int widthCommentArea = CalculateWidthCommentArea();
			CommentColorer colorer = RichEditControl.DocumentModel.CommentColorer;
			if (DocumentModel != null) {
				controller.FillRichEditControl(DocumentModel, widthCommentArea, IdProvider, colorer, keepHistory);
				DocumentModelCopyCommand.CopyStyles(DocumentModel, RichEditControl.DocumentModel, true);
			}
			Controller = controller;
			RecreateVisibleTextFilter();
		}
		void RecreateVisibleTextFilter() {
			DocumentModel.MainPieceTable.CustomCreateVisibleTextFilter -= CustomCreateVisibleTextFilter;
			DocumentModel.MainPieceTable.CustomCreateVisibleTextFilter += CustomCreateVisibleTextFilter;
			DocumentModel.RecreateVisibleTextFilter();
		}
		void CustomCreateVisibleTextFilter(object sender, CustomCreateVisibleTextFilterEventArgs e) {
			if(RichEditControl != null)
				e.TextFilter = new CommentTextFilter(RichEditControl.Options.Comments, Controller, DocumentModel.MainPieceTable, e.TextFilter);
		}
		void SubscribeRichEditCommentControlEvents(RichEditControl richEditControl) {
			SubscribeSelectionEvents();
			richEditControl.DocumentLoaded += RichEditControl_DocumentLoaded;
			richEditControl.EmptyDocumentCreated += RichEditControl_EmptyDocumentCreated;
			richEditControl.ShowReviewingPane += RichEditControl_ShowReviewingPane;
			richEditControl.ObtainReviewingPaneVisible += RichEditControl_ObtainReviewingPaneVisible; 
			richEditControl.CloseReviewingPane += RichEditControl_CloseReviewingPane;		  
			SubscribeCommentEvents();
			richEditControl.DocumentModel.CommentOptions.Changed += CommentOptions_Changed;
			SubscribeToStyleCollectionChanged(richEditControl);
		}
		void SubscribeToStyleCollectionChanged(RichEditControl richEditControl) {
			richEditControl.DocumentModel.ParagraphStyles.CollectionChanged += ParagraphStyles_CollectionChanged;
			richEditControl.DocumentModel.CharacterStyles.CollectionChanged += CharacterStyles_CollectionChanged;
			richEditControl.DocumentModel.TableStyles.CollectionChanged += TableStyles_CollectionChanged;
			richEditControl.DocumentModel.TableCellStyles.CollectionChanged += TableCellStyles_CollectionChanged;
		}
		void UnsubscribeFromStyleCollectionChanged(RichEditControl richEditControl) {
			richEditControl.DocumentModel.ParagraphStyles.CollectionChanged -= ParagraphStyles_CollectionChanged;
			richEditControl.DocumentModel.CharacterStyles.CollectionChanged -= CharacterStyles_CollectionChanged;
			richEditControl.DocumentModel.TableStyles.CollectionChanged -= TableStyles_CollectionChanged;
			richEditControl.DocumentModel.TableCellStyles.CollectionChanged -= TableCellStyles_CollectionChanged;
		}
		void TableCellStyles_CollectionChanged(object sender, EventArgs e) {
			DocumentModelCopyCommand.CopyStylesCore<TableCellStyle>(DocumentModel.TableCellStyles, RichEditControl.DocumentModel.TableCellStyles, true);
		}
		void TableStyles_CollectionChanged(object sender, EventArgs e) {
			DocumentModelCopyCommand.CopyStylesCore<TableStyle>(DocumentModel.TableStyles, RichEditControl.DocumentModel.TableStyles, true);
		}
		void CharacterStyles_CollectionChanged(object sender, EventArgs e) {
			DocumentModelCopyCommand.CopyStylesCore<CharacterStyle>(DocumentModel.CharacterStyles, RichEditControl.DocumentModel.CharacterStyles, true);
		}
		void ParagraphStyles_CollectionChanged(object sender, EventArgs e) {
			DocumentModelCopyCommand.CopyStylesCore<ParagraphStyle>(DocumentModel.ParagraphStyles, RichEditControl.DocumentModel.ParagraphStyles, true);
		}
		void UnsubscribeRichEditCommentControlEvents(RichEditControl richEditControl) {
			UnsubscribeSelectionEvents();
			richEditControl.DocumentLoaded -= RichEditControl_DocumentLoaded;
			richEditControl.EmptyDocumentCreated -= RichEditControl_EmptyDocumentCreated;
			richEditControl.ShowReviewingPane -= RichEditControl_ShowReviewingPane;
			richEditControl.ObtainReviewingPaneVisible -= RichEditControl_ObtainReviewingPaneVisible;
			richEditControl.CloseReviewingPane -= RichEditControl_CloseReviewingPane;
			UnsubscribeCommentEvents();
			richEditControl.DocumentModel.CommentOptions.Changed -= CommentOptions_Changed;
			UnsubscribeFromStyleCollectionChanged(richEditControl);
		}
		protected internal void SubscribeShowReviewingPaneEvent() {
			RichEditControl.ShowReviewingPane += RichEditControl_ShowReviewingPane;
		}
		protected internal void UnsubscribeShowReviewingPaneEvent() {
			RichEditControl.ShowReviewingPane -= RichEditControl_ShowReviewingPane;
		}
		void RichEditControl_ObtainReviewingPaneVisible(object sender, ObtainReviewingPaneVisibleEventArg e) {
			e.ReviewingPaneVisible = DockPanelVisible;
		}
		internal void SetCursor(CommentViewInfo commentViewInfo, bool selectParagraph, DocumentLogPosition start, DocumentLogPosition end) {
			if (lockSelectionChangedCount > 0)
				return;
			if (commentViewInfo != null) {
				Controller.SetSelect(DocumentModel, commentViewInfo.Comment, true, IdProvider, selectParagraph, start, end);
				this.ScrollToCaret(0);
				Controller.SetSelect(DocumentModel, commentViewInfo.Comment, false, IdProvider, selectParagraph, start, end);
			}
			else
				Controller.SetCursor(DocumentModel, IdProvider);
		}
		void RichEditControl_EmptyDocumentCreated(object sender, EventArgs e) {
			GenerateMainDocumentComments(false);
		}
		void RichEditControl_DocumentLoaded(object sender, EventArgs e) {
			GenerateMainDocumentComments(false);
			UnsubscribeFromStyleCollectionChanged(RichEditControl);
			SubscribeToStyleCollectionChanged(RichEditControl);
		}
		void RichEditControlCommentDeleted(object sender, CommentEventArgs e) {
			if (!e.OnExecute) {
				Controller.Comments.Remove(e.Comment);
				return;
			}
			ReviewingPaneCommentCreator creator = new ReviewingPaneCommentCreator(IdProvider, DocumentModel, Controller);
			DocumentModel documentModel = RichEditControl.DocumentModel;
			CommentCollection comments = documentModel.MainPieceTable.Comments;
			UnsubscribeSelectionEvents();
			DocumentModel.BeginUpdate();
			try {
				creator.DeleteComment(comments, e.Comment, e.CommentIndex);
				Controller.Comments.Remove(e.Comment);
			}
			finally {
				DocumentModel.EndUpdate();
			}
			SubscribeSelectionEvents();
		}	  
		void RichEditControlCommentInserted(object sender, CommentEventArgs e) {
			if (!e.OnExecute) {
				Controller.Comments.Insert(e.CommentIndex, e.Comment);
				return;
			}
			ReviewingPaneCommentCreator creator = new ReviewingPaneCommentCreator(IdProvider, DocumentModel, Controller);
			Comment newComment = e.Comment;
			int newCommentIndex = e.CommentIndex;
			UnsubscribeSelectionEvents();
			DocumentModel.BeginUpdate();
			try {
				creator.InsertComment(newComment, newCommentIndex, IdProvider.GetCommentId(newComment));
			}
			finally {
				DocumentModel.EndUpdate();
			}
			ClearSelectionInCommentControl();
			SubscribeSelectionEvents();
		}
		internal void ClearSelectionInCommentControl() {
			DocumentModel.BeginUpdate();
			DocumentModel.Selection.Start = DocumentLogPosition.Zero;
			DocumentModel.Selection.End = DocumentLogPosition.Zero;
			DocumentModel.EndUpdate();
		}
		private void CommentOptions_Changed(object sender, DevExpress.Utils.Controls.BaseOptionChangedEventArgs e) {
			if (e.Name == "CommentColor") {
				GenerateMainDocumentComments(false);
				UnsubscribeFromStyleCollectionChanged(RichEditControl);
				SubscribeToStyleCollectionChanged(RichEditControl);
			}
			DocumentModel.BeginUpdate();
			try {
				DocumentModel.MainPieceTable.ApplyChangesCore(DocumentModelChangeActions.ResetAllPrimaryLayout, RunIndex.Zero, RunIndex.MaxValue);
				DocumentModel.MainPieceTable.ApplyChanges(DocumentModelChangeType.CommentOptionsVisibilityChanged, RunIndex.Zero, RunIndex.MaxValue);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal override void ResizeView(bool ensureCaretVisibleonResize) {
			base.ResizeView(ensureCaretVisibleonResize);
			int widthCommentArea = CalculateWidthCommentArea();
			ResizeCore(widthCommentArea);
		}
		protected internal void ResizeCore(int widthCommentArea) {
			ReviewingPaneCommentCreator creator = new ReviewingPaneCommentCreator(IdProvider, DocumentModel, Controller);
			DocumentModel.History.DisableHistory();
			try {
				DocumentModel.BeginUpdate();
				creator.ChangeCommentHeadingParagraphStyle(widthCommentArea);
				DocumentModel.ApplyChanges(DocumentModel.MainPieceTable, DocumentModelChangeType.CommentOptionsVisibilityChanged, RunIndex.MinValue, RunIndex.MaxValue);
				DocumentModel.EndUpdate();
			}
			finally {
				DocumentModel.History.EnableHistory();
			}
		}
		internal Comment FindCommentFromSelection(Bookmark bookmark) {
			if (bookmark != null)
				return Controller.GetCommentFromBookmarkName(bookmark.Name);
			return null;
		}
		internal Bookmark GetBookmarkFromSelection() {
			Selection selection = DocumentModel.Selection;
			BookmarkCollection bookmarks = DocumentModel.ActivePieceTable.Bookmarks;
			return bookmarks.GetBookmarkFromSelection(selection);
		}
		void ActivateMainPieceTable() {
			if (MainDocumentModel.ActivePieceTable.IsComment) {
				CommentCaretPosition position = MainActiveView.CaretPosition as CommentCaretPosition;
				if (position != null) {
					CommentViewInfo comment = RichEditControl.InnerControl.GetCommentViewInfoFromCaretPosition(position);
					if (comment!= null && RichEditControl.InnerControl.CommentViewInfoNoContainsCursorCore(position, comment))
						ActivateMainPieceTableCore(comment);
				}
			}
		}
		void ActivateMainPieceTableCore(CommentViewInfo comment) {
			IThreadSyncService service = GetService<IThreadSyncService>();
			if (service != null) {
				service.EnqueueInvokeInUIThread(new Action(delegate() { RichEditControl.InnerControl.ActivateMainPieceTable(MainActiveView.Control, comment.Comment.Interval.End.LogPosition); }));
			}
		}
		protected internal void SubscribeCommentEvents() {
			RichEditControl.DocumentModel.CommentInserted += RichEditControlCommentInserted;
			RichEditControl.DocumentModel.CommentDeleted += RichEditControlCommentDeleted;
			RichEditControl.DocumentModel.CommentChangedViaAPI += RichEditControlCommentChangedViaAPI;
		}
		void RichEditControlCommentChangedViaAPI(object sender, CommentEventArgs e) {
			if (Controller.Comments.Contains(e.Comment)) {
				UnsubscribeSelectionEvents();
				GenerateMainDocumentComments(e.OnExecute);
				ClearSelectionInCommentControl();
				SubscribeSelectionEvents();
			}
		}
		protected internal void UnsubscribeCommentEvents() {
			RichEditControl.DocumentModel.CommentInserted -= RichEditControlCommentInserted;
			RichEditControl.DocumentModel.CommentDeleted -= RichEditControlCommentDeleted;
			RichEditControl.DocumentModel.CommentChangedViaAPI -= RichEditControlCommentChangedViaAPI;
		}
		protected virtual void SubscribeSelectionEvents() {
			DocumentModel.EndDocumentUpdate += DocumentModel_EndDocumentUpdate;
			RichEditControl.DocumentModel.BeginDocumentUpdate += RichEditDocumentModel_BeginDocumentUpdate;
			RichEditControl.DocumentModel.EndDocumentUpdate += RichEditDocumentModel_EndDocumentUpdate;
		}
		protected virtual void UnsubscribeSelectionEvents() {
			this.DocumentModel.EndDocumentUpdate -= DocumentModel_EndDocumentUpdate;
			RichEditControl.DocumentModel.BeginDocumentUpdate -= RichEditDocumentModel_BeginDocumentUpdate;
			RichEditControl.DocumentModel.EndDocumentUpdate -= RichEditDocumentModel_EndDocumentUpdate;
		}
		void RichEditControl_SelectionChanged(object sender, EventArgs e) {
			SelectionChangeCore();
		}
		internal void SelectionChangeCore() {
			if (lockSelectionChangedCount > 0)
				return;
			this.DocumentModel.EndDocumentUpdate -= DocumentModel_EndDocumentUpdate;
			RichEditCommentCommandHelper helper = new RichEditCommentCommandHelper(this, RichEditControl);
			helper.SetSelectionInCommentControl();
			this.DocumentModel.EndDocumentUpdate += DocumentModel_EndDocumentUpdate;
		}
		void DocumentModel_EndDocumentUpdate(object sender, DocumentUpdateCompleteEventArgs e) {
			if ((e.DeferredChanges.ChangeActions & DocumentModelChangeActions.RaiseSelectionChanged) != 0)
				RichEditCommentControl_SelectionChanged(sender, EventArgs.Empty);
		}
		void RichEditDocumentModel_BeginDocumentUpdate(object sender, EventArgs e) {
			this.DocumentModel.BeginUpdate();
		}
		void RichEditDocumentModel_EndDocumentUpdate(object sender, DocumentUpdateCompleteEventArgs e) {
			if ((e.DeferredChanges.ChangeActions & DocumentModelChangeActions.RaiseSelectionChanged) != 0)
				RichEditControl_SelectionChanged(sender, EventArgs.Empty);
			this.DocumentModel.EndDocumentUpdate -= DocumentModel_EndDocumentUpdate;
			this.DocumentModel.EndUpdate();
			this.DocumentModel.EndDocumentUpdate += DocumentModel_EndDocumentUpdate;
		}
		int lockSelectionChangedCount;
		protected internal void LockSelectionChangedEvent() {
			lockSelectionChangedCount++;
		}
		protected internal void UnlockSelectionChangedEvent() {
			lockSelectionChangedCount--;
			if (lockSelectionChangedCount < 0)
				Exceptions.ThrowInternalException();
		}
		void RichEditCommentControl_SelectionChanged(object sender, EventArgs e) {
			if (lockSelectionChangedCount > 0)
				return;
			LockSelectionChangedEvent();
			RichEditControl.DocumentModel.EndDocumentUpdate -= RichEditDocumentModel_EndDocumentUpdate;
			RichEditCommentCommandHelper helper = new RichEditCommentCommandHelper(this, RichEditControl);
			helper.SetSelection();
			RichEditControl.DocumentModel.EndDocumentUpdate += RichEditDocumentModel_EndDocumentUpdate;
			UnlockSelectionChangedEvent();
		}
	}
	#endregion
}
