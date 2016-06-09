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
using System.Xml;
using DevExpress.Office;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Import.OpenXml;
using DevExpress.XtraRichEdit.Export.WordML;
namespace DevExpress.XtraRichEdit.Import.WordML {
	#region AnnotationElementDestination
	public class AnnotationElementDestination : LeafElementDestination {
		XmlReader reader;
		string id;
		public AnnotationElementDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			this.reader = reader;
			id = reader.GetAttribute("id", WordMLExporter.AMLNamespaceConst);
			string type = reader.GetAttribute("type", Importer.WordProcessingNamespaceConst);
			if (String.IsNullOrEmpty(id) || String.IsNullOrEmpty(type))
				return;
			id.Trim();
			if (type == "Word.Bookmark.Start") 
				AssignBookmarkStart();
			if (type == "Word.Bookmark.End") 
				AssignBookmarkEnd();
			if (type == "Word.Comment.Start")
				AssignCommentStart(); 
			if (type == "Word.Comment.End") 
				AssignCommentEnd(); 
		}
		ImportBookmarkInfo GetBookmark() {
			ImportBookmarkInfo bookmark;
			if (!Importer.Bookmarks.TryGetValue(id, out bookmark)) {
				bookmark = new ImportBookmarkInfo();
				string name = reader.GetAttribute("name", Importer.WordProcessingNamespaceConst);
				if (!String.IsNullOrEmpty(name))
					bookmark.Name = name.Trim();
				Importer.Bookmarks.Add(id, bookmark);
			}
			return bookmark;
		}
		void AssignBookmarkStart() {
			ImportBookmarkInfo bookmark = GetBookmark();
			bookmark.Start = Importer.Position.LogPosition;
		}
		void AssignBookmarkEnd() {
			ImportBookmarkInfo bookmark = GetBookmark();
			bookmark.End = Importer.Position.LogPosition;
		}
		ImportCommentInfo GetComment() {
			ImportCommentInfo comment;
			if (!Importer.Comments.TryGetValue(id, out comment)) {
				comment = new ImportCommentInfo(DocumentModel.MainPieceTable);
				Importer.Comments.Add(id, comment);
			}
			return comment;
		}
		void AssignCommentStart() {
			ImportCommentInfo comment = GetComment();
			comment.Start = Importer.Position.LogPosition;
		}
		void AssignCommentEnd() {
			ImportCommentInfo comment = GetComment();
			comment.End = Importer.Position.LogPosition;
		}
	}
	#endregion
	#region AnnotationCommentDestination
	public class AnnotationCommentDestination : ElementDestination {
		#region CreateElementHandlerTable
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("content", OnComment);
			return result;
		}
		#endregion
		#region Element Handler
		static Destination OnComment(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new AnnotationContentDestination(importer, GetThis(importer).ActiveComment);
		}
		#endregion
		static AnnotationCommentDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (AnnotationCommentDestination)importer.PeekDestination();
		}
		ImportCommentInfo activeComment;
		public AnnotationCommentDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
			this.activeComment = new ImportCommentInfo(DocumentModel.MainPieceTable);
		}
		#region Properties
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		protected ImportCommentInfo ActiveComment { get { return activeComment; } set { activeComment = value; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			string id = reader.GetAttribute("id", WordMLExporter.AMLNamespaceConst);
			string type = reader.GetAttribute("type", Importer.WordProcessingNamespaceConst);
			if (String.IsNullOrEmpty(id) || type != "Word.Comment")
				return;
			id = id.Trim();
			ImportCommentInfo comment;
			if (!Importer.Comments.TryGetValue(id, out comment)) {
				comment = new ImportCommentInfo(DocumentModel.MainPieceTable);
				DocumentLogPosition position = Importer.Position.LogPosition;
				comment.Start = position;
				comment.End = position;
				Importer.Comments.Add(id, comment);
			}
			string author = reader.GetAttribute("author", WordMLExporter.AMLNamespaceConst);
			if (!String.IsNullOrEmpty(author))
				comment.Author = author;
			string date = reader.GetAttribute("createdate", WordMLExporter.AMLNamespaceConst);
			if (date == null)
				comment.Date = Comment.MinCommentDate;
			else
				comment.Date = DateTimeUtils.FromDateTimeISO8601(date);
			string initials = reader.GetAttribute("initials", Importer.WordProcessingNamespaceConst);
			if (!String.IsNullOrEmpty(initials))
				comment.Name = initials;
			ActiveComment = comment;
		}
	}
	#endregion
	#region AnnotationContentDestination
	public class AnnotationContentDestination : BodyDestinationBase {
		#region CreateElementHandlerTable
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("p", OnParagraph);
			result.Add("tbl", OnTable);
			result.Add("sectPr", OnSection);
			result.Add("bookmarkStart", OnBookmarkStart);
			result.Add("bookmarkEnd", OnBookmarkEnd);
			result.Add("permStart", OnRangePermissionStart);
			result.Add("permEnd", OnRangePermissionEnd);
			result.Add("sdt", OnStructuredDocument);
			result.Add("altChunk", OnAltChunk);
			result.Add("customXml", OnCustomXml);
			return result;
		}
		#endregion
		#region Element Handler
		static Destination OnParagraph(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return importer.CreateParagraphDestination();
		}
		static Destination OnTable(WordProcessingMLBaseImporter importer, XmlReader reader) {
			if (importer.DocumentModel.DocumentCapabilities.TablesAllowed)
				return new TableDestination(importer);
			else
				return new TableDisabledDestination(importer);
		}
		static Destination OnSection(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new LastSectionDestination(importer);
		}
		static Destination OnAltChunk(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new AltChunkDestination(importer);
		}
		#endregion
		#region Fields
		CommentContentType commentContentType;
		ImportCommentInfo activeComment;
		#endregion
		public AnnotationContentDestination(WordProcessingMLBaseImporter importer, ImportCommentInfo activeComment)
			: base(importer) {
			this.activeComment = activeComment;
		}
		#region Properties
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		protected ImportCommentInfo ActiveComment { get { return activeComment; } set { activeComment = value; } }
		protected CommentContentType CommentContentType { get { return commentContentType; } set { commentContentType = value; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			CommentContentType = new CommentContentType(DocumentModel);
			Importer.PushCurrentPieceTable(CommentContentType.PieceTable);
		}
		public override void ProcessElementClose(XmlReader reader) {
			PieceTable.FixLastParagraph();
			Importer.InsertBookmarks();
			Importer.InsertRangePermissions();
			PieceTable.FixTables();
			Importer.PopCurrentPieceTable();
			ActiveComment.Content = CommentContentType;
			ActiveComment.HasReference = true;
		}
	}
	#endregion
}
