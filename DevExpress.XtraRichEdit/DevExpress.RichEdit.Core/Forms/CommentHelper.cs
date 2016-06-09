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
using System.Drawing;
using System.Linq;
using System.Text;
using DevExpress.Office;
using DevExpress.Office.Layout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.XtraRichEdit.Localization;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Forms {
	public abstract class CommentsCreator {
		#region Fields
		const int thinLine = 2;
		const int thikLine = 10;
		const int offset = 10;
		int marginRight;
		int marginLeft;
		CommentIdProvider provider;
		DocumentModel targetModel;
		#endregion
		protected CommentsCreator(CommentIdProvider provider, DocumentModel targetModel) {
			this.provider = provider;
			this.targetModel = targetModel;
		}
		#region Properties
		protected int MarginRight { get { return marginRight; } set { marginRight = value; } }
		protected int MarginLeft { get { return marginLeft; } set { marginLeft = value; } }
		protected CommentIdProvider Provider { get { return provider; } }
		protected DocumentModel TargetModel { get { return targetModel; } }
		protected PieceTable TargetPieceTable { get { return targetModel.MainPieceTable; } }
		protected internal abstract Dictionary<string, Comment> CommentsBookmarkName { get; }
		#endregion
		protected internal DocumentLogPosition CopyComment(Comment sourceComment, int commentIndex, DocumentLogPosition logPosition, int pageOrdinal, int id) {
			Color fillColor = TargetModel.CommentColorer.GetColor(sourceComment);
			Color borderColor = CommentOptions.SetBorderColor(fillColor);
			string headComment = ShapeHeadComment(sourceComment, commentIndex, pageOrdinal, sourceComment.Index + 1);
			TargetPieceTable.InsertTable(logPosition, 1, 1);
			int countTables = TargetPieceTable.Tables.Count;
			Table table = TargetPieceTable.Tables[countTables - 1];
			DrawTable(table, fillColor, borderColor);
			ParagraphIndex index = CalculateParagraphIndex(logPosition);
			TargetPieceTable.Paragraphs[index].ParagraphStyleIndex = TargetModel.ParagraphStyles.GetStyleIndexByName("CommentHeading");
			TargetPieceTable.InsertText(logPosition, headComment);
			string firstBookmarkName = ShapeNameBookmark(sourceComment, id);
			DocumentLogInterval firstBookmarkInterval = new DocumentLogInterval(logPosition, headComment.Length + 2);			
			logPosition += headComment.Length + 1;
			TargetPieceTable.InsertParagraph(logPosition);
			index = CalculateParagraphIndex(logPosition);
			RunIndex runIndex = TargetPieceTable.Paragraphs[index].LastRunIndex;
			TargetPieceTable.Runs[runIndex].CharacterProperties.Hidden = true;
			logPosition += 1;
			int lengthComment = CalculateCommentLength(sourceComment);
			index = CalculateParagraphIndex(logPosition);
			TargetPieceTable.Paragraphs[index].ParagraphStyleIndex = TargetModel.ParagraphStyles.DefaultItemIndex;
			CopyFrom(sourceComment, logPosition);
			string nameBookmark = firstBookmarkName + "comment";
			TargetPieceTable.CreateBookmark(firstBookmarkInterval.Start, firstBookmarkInterval.Length, firstBookmarkName);
			TargetPieceTable.CreateBookmark(logPosition, lengthComment , nameBookmark);
			if (!CommentsBookmarkName.ContainsKey(nameBookmark))
				CommentsBookmarkName.Add(nameBookmark, sourceComment);
			CreateRangePermission(logPosition, lengthComment - 1, string.Empty, "Everyone");
			TargetPieceTable.DocumentModel.Selection.ClearMultiSelection();
			TargetPieceTable.DocumentModel.Selection.SetInterval(logPosition, logPosition);
			TargetPieceTable.DocumentModel.Selection.SetStartCell(logPosition);
			logPosition += lengthComment;
			return logPosition;
		}
		public virtual string ShapeHeadComment(Comment sourceComment, int commentIndex, int pageOrdinal, int sourceCommentIndex) {
			return " ";
		}
		protected string ShapeNameBookmark(Comment sourceComment, int id) {
			return String.Format(XtraRichEditLocalizer.GetString(XtraRichEditStringId.Comment) + " \t{2}", sourceComment.Name, id, sourceComment.Author);
		}
		protected ParagraphIndex CalculateParagraphIndex(DocumentLogPosition logPosition) {
			DocumentModelPosition position = PositionConverter.ToDocumentModelPosition(TargetPieceTable, logPosition);
			return position.ParagraphIndex;
		}
		protected internal DocumentLogPosition CalculateStartLogPositionCore(Comment sourceComment, bool scrollToCaret) {
			BookmarkCollection bookmarks = TargetPieceTable.Bookmarks;
			string nameBookmark = ShapeNameBookmark(sourceComment, provider.GetCommentId(sourceComment));
			if (!scrollToCaret)
				nameBookmark += "comment";
			Bookmark bookmark = bookmarks.FindByName(nameBookmark);
			if (bookmark != null)
				return bookmark.Start;
			return TargetPieceTable.DocumentStartLogPosition;
		}
		public int CalculateCommentLength(Comment sourceComment) {
			PieceTable sourcePieceTable = sourceComment.Content.PieceTable;
			DocumentLogPosition startLogPosition = sourcePieceTable.DocumentStartLogPosition;
			DocumentLogPosition endLogPosition = sourcePieceTable.DocumentEndLogPosition;
			return endLogPosition - startLogPosition + 1;
		}
		public void CreateCommentHeadingParagraphStyle() {
			ParagraphStyle commentHeadingStyle = new ParagraphStyle(TargetModel);
			commentHeadingStyle.StyleName = "CommentHeading";
			TargetModel.ParagraphStyles.Add(commentHeadingStyle);
			commentHeadingStyle.CharacterProperties.FontBold = true;
		}
		public void ChangeCommentHeadingParagraphStyle(int position) {
			ParagraphStyle commentHeadingStyle = TargetModel.ParagraphStyles.GetStyleByName("CommentHeading");
			if (commentHeadingStyle != null) {
				DocumentModelUnitConverter unitConverter = TargetModel.UnitConverter;
				position = ConvertedPosition(unitConverter, position, thikLine, thinLine, offset);
				TabInfo commentTab = new TabInfo(position, TabAlignmentType.Right);
				TabFormattingInfo commentHeadingTabs = new TabFormattingInfo();
				commentHeadingTabs.Add(commentTab);
				commentHeadingStyle.Tabs.SetTabs(commentHeadingTabs);
			}
		}
		protected abstract int ConvertedPosition(DocumentModelUnitConverter unitConverter, int position, int thikLine, int thinLine, int offset);
		void CreateRangePermission(DocumentLogPosition start, int length, string userName, string group) {
			RangePermissionInfo info = new RangePermissionInfo();
			info.UserName = userName;
			info.Group = group;
			TargetPieceTable.ApplyDocumentPermission(start, start + length, info);
		}
		void CopyFrom(Comment sourceComment, DocumentLogPosition targetPosition) {
			PieceTable sourcePieceTable = sourceComment.Content.PieceTable;
			DocumentModel intermediateModel = sourcePieceTable.DocumentModel.CreateNew(false, false);
			DocumentLogPosition startLogPosition = sourcePieceTable.DocumentStartLogPosition;
			DocumentLogPosition endLogPosition = sourcePieceTable.DocumentEndLogPosition;
			DocumentModelCopyOptions options = new DocumentModelCopyOptions(startLogPosition, endLogPosition - startLogPosition + 1);
			DocumentModelCopyCommand copyCommand = sourceComment.DocumentModel.CreateDocumentModelCopyCommand(sourcePieceTable, intermediateModel, options);
			copyCommand.SuppressFieldsUpdate = true;
			copyCommand.Execute();
			PieceTableInsertContentConvertedToDocumentModelCommand command = new PieceTableInsertContentConvertedToDocumentModelCommand(TargetPieceTable, intermediateModel, targetPosition, false);
			command.Execute();
		}
		int CalculateLengthComment(Comment sourceComment) {
			PieceTable sourcePieceTable = sourceComment.Content.PieceTable;
			DocumentLogPosition startLogPosition = sourcePieceTable.DocumentStartLogPosition;
			DocumentLogPosition endLogPosition = sourcePieceTable.DocumentEndLogPosition;
			return endLogPosition - startLogPosition + 1;
		}  
		void DrawTable(Table table, Color fillColor, Color borderColor) {
			TableProperties properties = table.TableProperties;
			SetPreferredWidth(properties.PreferredWidth);
			SetBackgroundColor(table, fillColor);
			DocumentModelUnitConverter unitConverter = TargetPieceTable.DocumentModel.UnitConverter;
			int thinLineWidth = unitConverter.DocumentsToModelUnits(thinLine);
			int thikLineWidth = unitConverter.DocumentsToModelUnits(thikLine);
			SetTableBorder(properties.Borders.TopBorder, borderColor, thinLineWidth);
			SetTableBorder(properties.Borders.RightBorder, borderColor, thikLineWidth);
			SetTableBorder(properties.Borders.BottomBorder, borderColor, thikLineWidth);
			SetTableBorder(properties.Borders.LeftBorder, borderColor, thinLineWidth);
			MarginRight = properties.CellMargins.Right.Value;
			MarginLeft = properties.CellMargins.Left.Value;
		}
		void SetPreferredWidth(PreferredWidth preferredWidth) {
			preferredWidth.Type = WidthUnitType.FiftiethsOfPercent;
			preferredWidth.Value = 100 * 50;
		}
		void SetBackgroundColor(Table table, Color color) {
			table.TableProperties.BackgroundColor = color;
			table.Rows[0].Cells[0].BackgroundColor = color;
		}
		void SetTableBorder(BorderBase border, Color color, int width) {
			border.Color = color;
			border.Style = BorderLineStyle.Single;
			border.Width = width;
		}
	}
	public class DocumentPrinterCommentCreator : CommentsCreator {
		Dictionary<string, Comment> commentsBookmarkName = new Dictionary<string, Comment>();
		public DocumentPrinterCommentCreator(CommentIdProvider provider, DocumentModel targetModel) : base(provider, targetModel) { }
		protected internal override Dictionary<string, Comment> CommentsBookmarkName { get { return commentsBookmarkName; } }
		public override string ShapeHeadComment(Comment sourceComment, int commentIndex, int pageOrdinal, int sourceCommentIndex) {
			if (sourceComment.Date > Comment.MinCommentDate)
				return String.Format(XtraRichEditLocalizer.GetString(XtraRichEditStringId.Page) + XtraRichEditLocalizer.GetString(XtraRichEditStringId.Comment) + "                  {4}  \t{5}", sourceComment.Name, sourceCommentIndex, pageOrdinal, commentIndex + 1, sourceComment.Author, sourceComment.Date );
			return String.Format(XtraRichEditLocalizer.GetString(XtraRichEditStringId.Page) + XtraRichEditLocalizer.GetString(XtraRichEditStringId.Comment) + "                   {4}", sourceComment.Name, sourceCommentIndex, pageOrdinal, commentIndex + 1, sourceComment.Author);
		}
		protected override int ConvertedPosition(DocumentModelUnitConverter unitConverter, int position, int thikLine, int thinLine, int offset) {
			return unitConverter.HundredthsOfInchToModelUnits(position);
		}
	}
	public class ReviewingPaneCommentCreator : CommentsCreator {
		ReviewingPaneFormController controller;
		public ReviewingPaneCommentCreator(CommentIdProvider provider, DocumentModel targetModel, ReviewingPaneFormController controller) : base(provider, targetModel) {
			this.controller = controller;
		}
		protected internal override Dictionary<string, Comment> CommentsBookmarkName { get { return controller.CommentsBookmarkName; } }
		List<Comment> Comments { get { return controller.Comments; } }
		public override string ShapeHeadComment(Comment sourceComment, int commentIndex, int pageOrdinal, int sourceCommentIndex) {
			return String.Format(XtraRichEditLocalizer.GetString(XtraRichEditStringId.Comment) + " \t{2}", sourceComment.Name, sourceCommentIndex, sourceComment.Author);
		}
		protected override int ConvertedPosition(DocumentModelUnitConverter unitConverter, int position, int thikLine, int thinLine, int offset) {
			return unitConverter.PixelsToModelUnits(position - thikLine - MarginRight - thinLine - MarginLeft - offset, 96);
		}
		protected internal void DeleteComment(CommentCollection comments, Comment sourceComment, int commentIndex) {
			Bookmark bookmark = FindBookmark(sourceComment);
			if (bookmark == null)
				return;
			DocumentLogPosition start = bookmark.Start;
			int length = bookmark.Length + CalculateCommentLength(sourceComment);
			TargetPieceTable.DeleteContent(start, length, start + length >= TargetPieceTable.DocumentEndLogPosition);
			int count = comments.Count;
			if (commentIndex <= count - 1) {
				for (int i = commentIndex; i < count; i++) {
					Comment originalComment = comments[i];
					RenameCommentHeadingAfterDeleteComment(originalComment, originalComment.Index);
				}
			}
		}
		Bookmark FindBookmark(Comment sourceComment) {
			string nameBookmark = ShapeNameBookmark(sourceComment, Provider.GetCommentId(sourceComment));
			BookmarkCollection bookmarks = TargetPieceTable.Bookmarks;
			return bookmarks.FindByName(nameBookmark);
		}
		void RenameCommentHeadingAfterDeleteComment(Comment sourceComment, int commentIndex) {
			int oldIndex = commentIndex + 2;
			int newIndex = commentIndex + 1;
			RenameCommentCore(sourceComment, oldIndex, newIndex);
		}
		void RenameCommentCore(Comment sourceComment, int oldIndex, int newIndex) {
			Bookmark bookmark = FindBookmark(sourceComment);
			if (bookmark == null)
				return;
			DocumentLogPosition startBookmark = bookmark.Start;
			int length = bookmark.Length;
			DocumentLogPosition start = startBookmark;
			string headComment = ShapeHeadComment(sourceComment, 0, 0, newIndex);
			TargetPieceTable.InsertText(start, headComment);
			start += headComment.Length;
			TargetPieceTable.DeleteContent(start, length - 2, start + length - 2>= TargetPieceTable.DocumentEndLogPosition);
		}
		protected internal void InsertComment(Comment newComment, int newCommentIndex, int newId) {
			TargetModel.BeginUpdate();
			try {
				Comments.Insert(newCommentIndex, newComment);
				int count = Comments.Count;
				if (newCommentIndex < count - 1)
					InsertNonLastComment(newComment, newCommentIndex, newId);
				else
					InsertLastComment(newComment, newCommentIndex, newId);
			}
			finally {
				TargetModel.EndUpdate();
			}
		}
		void InsertNonLastComment(Comment newComment, int newCommentIndex, int newId) {
			Bookmark nextBookmark = FindBookmark(Comments[newCommentIndex + 1]);
			if (nextBookmark == null)
				return;
			DocumentLogInterval nextBookmarkInterval = new DocumentLogInterval(nextBookmark.Start, nextBookmark.Length);
			DeleteBookmark(nextBookmark.Name);
			DocumentLogPosition start = TargetPieceTable.DocumentStartLogPosition;
			if (newCommentIndex > 0)
				start = CalculateStartLogPosition(Comments[newCommentIndex - 1], newCommentIndex);
			InsertParagraphBeforeTable(start);
			DocumentLogPosition end = CopyComment(newComment, newCommentIndex, start, 0, newId); ;
			TargetPieceTable.DeleteContent(end, 1, end + 1 >= TargetPieceTable.DocumentEndLogPosition);
			int shift = end - start;
			Comment comment = Comments[newCommentIndex + 1];
			ChangeBookmarkAndHeadComment(comment, shift, nextBookmarkInterval, nextBookmark.Name);
			int count = Comments.Count;
			for (int i = newCommentIndex + 2; i < count; i++) {
				Comment originalComment = Comments[i];
				RenameCommentHeadingAfterInsertComment(originalComment, originalComment.Index);
			}
		}
		void InsertLastComment(Comment sourceComment, int sourceCommentIndex, int newId) {
			DocumentLogPosition logPosition = TargetPieceTable.DocumentEndLogPosition;
			logPosition = CopyComment(sourceComment, sourceCommentIndex, logPosition, 0, newId);
		}
		DocumentLogPosition CalculateStartLogPosition(Comment sourceComment, int commentIndex) {
			if (commentIndex > 0) 
				return CalculateStartLogPositionCore(sourceComment, false) + CalculateCommentLength(sourceComment);
			return TargetPieceTable.DocumentStartLogPosition;
		}
		void InsertParagraphBeforeTable(DocumentLogPosition logPosition) {
			ParagraphIndex index = CalculateParagraphIndex(logPosition);
			Paragraph paragraph = TargetPieceTable.Paragraphs[index];
			TableCell paragraphCell = paragraph.GetCell();
			Paragraph newParagraph = TargetPieceTable.InsertParagraph(paragraph.LogPosition);
			TargetPieceTable.ChangeCellStartParagraphIndex(paragraphCell, newParagraph.Index + 1);
		}
		void ChangeBookmarkAndHeadComment(Comment sourceComment, int shift, DocumentLogInterval oldBookmarkInterval, string bookmarkName) {
			DocumentLogPosition newStart = oldBookmarkInterval.Start + shift;
			string headComment = ShapeHeadComment(sourceComment, 0, 0, sourceComment.Index + 1);
			TargetPieceTable.InsertText(newStart, headComment);
			DocumentLogPosition oldTextStart = newStart + headComment.Length;
			int oldLength = ShapeHeadComment(sourceComment, 0, 0, sourceComment.Index).Length;
			TargetPieceTable.DeleteContent(oldTextStart, oldLength, oldTextStart + oldLength >= TargetPieceTable.DocumentEndLogPosition);
			TargetPieceTable.CreateBookmark(newStart, headComment.Length + 2, bookmarkName);
		}
		void DeleteBookmark(string nameBookmark) {
			int indexBookmark = CalculateIndexBookmark(nameBookmark);
			TargetPieceTable.DeleteBookmark(indexBookmark);
		}
		int CalculateIndexBookmark(string name) {
			BookmarkCollection bookmarks = TargetPieceTable.Bookmarks;
			return bookmarks.FindIndexByName(name);
		}
		void RenameCommentHeadingAfterInsertComment(Comment sourceComment, int commentIndex) {
			int oldIndex = commentIndex;
			int newIndex = commentIndex + 1;
			RenameCommentCore(sourceComment, oldIndex, newIndex);
		}
		protected internal void CopyCommentContentFromReviewingPane() {
			Bookmark bookmark = GetBookmarkFromSelection();
			if (bookmark != null) {
				Comment comment = controller.GetCommentFromBookmarkName(bookmark.Name);
				if (comment != null) {
					CopyFromReviewingPane(bookmark, comment);
				}
			}
		}
		protected internal Bookmark GetBookmarkFromSelection() {
			Selection selection = TargetModel.Selection;
			BookmarkCollection bookmarks = TargetModel.ActivePieceTable.Bookmarks;
			int count = bookmarks.Count;
			for (int i = 0; i < count; i++)
				if ((selection.End <= bookmarks[i].End) && (selection.Start >= bookmarks[i].Start))
					return bookmarks[i];
			return null;
		}
		protected internal void CopyFromReviewingPane(Bookmark bookmark, Comment comment) {
			PieceTable sourcePieceTable = TargetModel.MainPieceTable;
			DocumentModel intermediateModel = sourcePieceTable.DocumentModel.CreateNew(false, false);
			DocumentLogPosition startLogPosition = bookmark.Start;
			DocumentLogPosition endLogPosition = bookmark.End - 1;
			DocumentModelCopyOptions options = new DocumentModelCopyOptions(startLogPosition, endLogPosition - startLogPosition + 1);
			DocumentModelCopyCommand copyCommand = TargetModel.CreateDocumentModelCopyCommand(sourcePieceTable, intermediateModel, options);
			copyCommand.SuppressFieldsUpdate = true;
			copyCommand.Execute();
			intermediateModel.ActivePieceTable.RangePermissions.Clear();
			PieceTable commentPieceTable = comment.Content.PieceTable;
			DocumentLogPosition start = commentPieceTable.DocumentStartLogPosition;
			int length = commentPieceTable.DocumentEndLogPosition - start;
			commentPieceTable.DeleteContent(start, length, start + length >= commentPieceTable.DocumentEndLogPosition);
			PieceTableInsertContentConvertedToDocumentModelCommand command = new PieceTableInsertContentConvertedToDocumentModelCommand(commentPieceTable, intermediateModel, start, false);
			command.Execute();
		}
	}
}
