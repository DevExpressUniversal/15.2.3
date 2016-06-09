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
using System.Globalization;
using System.Xml;
using System.Collections.Generic;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Utils;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Import.OpenXml {
	#region CommentElementDestination (abstract class)
	public abstract class CommentElementDestination : LeafElementDestination {
		protected CommentElementDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string id = reader.GetAttribute("id", Importer.WordProcessingNamespaceConst);
			if (String.IsNullOrEmpty(id))
				return;
			ImportCommentInfo comment;
			if (Importer.Comments.TryGetValue(id, out comment))
				AssignCommentPosition(comment);
		}
		protected internal abstract void AssignCommentPosition(ImportCommentInfo comment);
	}
	#endregion
	#region CommentStartElementDestination
	public class CommentStartElementDestination : CommentElementDestination {
		public CommentStartElementDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override void AssignCommentPosition(ImportCommentInfo comment) {
			if (comment.Start == new DocumentLogPosition(-1)) {
				comment.Start = Importer.Position.LogPosition;
			}
		}
	}
	#endregion
	#region CommentEndElementDestination
	public class CommentEndElementDestination : CommentElementDestination {
		public CommentEndElementDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override void AssignCommentPosition(ImportCommentInfo comment) {
			DocumentLogPosition position = Importer.Position.LogPosition;
			if (comment.Start != position && comment.End == new DocumentLogPosition(-1)) {
				comment.End = position;
			}
		}
	}
	#endregion
	#region CommentReferenceElementDestination
	public class CommentReferenceElementDestination : CommentElementDestination {
		public CommentReferenceElementDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override void AssignCommentPosition(ImportCommentInfo comment) {
			if (comment.Reference == new DocumentLogPosition(-1)) 
				comment.Reference = Importer.Position.LogPosition;
			comment.HasReference = true;
		}
	}
	#endregion
	#region CommentsDestination
	public class CommentsDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("comment", OnComment);
			return result;
		}
		protected static Destination OnComment(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new CommentDestination(importer);
		}
		public CommentsDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
	}
	#endregion
	#region CommentDestination
	public class CommentDestination : BodyDestinationBase {
		#region CreateElementHandlerTable
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("p", OnParagraph);
			result.Add("tbl", OnTable);
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
		#region Element handlers
		static Destination OnTable(WordProcessingMLBaseImporter importer, XmlReader reader) {
			if (importer.DocumentModel.DocumentCapabilities.TablesAllowed)
				return new TableDestination(importer);
			else
				return new TableDisabledDestination(importer);
		}
		static Destination OnParagraph(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return importer.CreateParagraphDestination();
		}
		static Destination OnAltChunk(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new AltChunkDestination(importer);
		}
		#endregion
		#region Fields
		string id;
		string author;
		string initials;
		DateTime date;
		CommentContentType newComment;
		#endregion
		public CommentDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			newComment = new CommentContentType(DocumentModel);
			Debug.Assert(newComment.PieceTable.Paragraphs.Count > 0);
			Importer.PushCurrentPieceTable(newComment.PieceTable);
			id = reader.GetAttribute("id", Importer.WordProcessingNamespaceConst);
			if (String.IsNullOrEmpty(id))
				id = String.Empty;
			string stringDate = reader.GetAttribute("date", Importer.WordProcessingNamespaceConst);
			if (stringDate == null)
				date = Comment.MinCommentDate;
			else
				date = DateTimeUtils.FromDateTimeISO8601(stringDate);
			author = reader.GetAttribute("author", Importer.WordProcessingNamespaceConst);
			if (String.IsNullOrEmpty(author))
				author = String.Empty;
			initials = reader.GetAttribute("initials", Importer.WordProcessingNamespaceConst);
			if (String.IsNullOrEmpty(initials))
				initials = String.Empty;
		}
		public override void ProcessElementClose(XmlReader reader) {
			PieceTable.FixLastParagraph();
			Importer.InsertBookmarks();
			Importer.InsertRangePermissions();
			PieceTable.FixTables();
			Importer.PopCurrentPieceTable();
			if (!String.IsNullOrEmpty(id) && !Importer.Comments.ContainsKey(id)) {
				AddToCommentListInfos();
				ImportCommentInfo comment = new ImportCommentInfo(DocumentModel.MainPieceTable);
				comment.Author = author;
				comment.Name = initials;
				comment.Date = date;
				comment.Content = newComment;
				comment.ParaId = Importer.ParaId;
				Importer.Comments.Add(id, comment);
			}
		}
		void AddToCommentListInfos() {
			CommentListInfo listInfo = new CommentListInfo();
			listInfo.Id = id;
			listInfo.ParaId = Importer.ParaId;
			if (!Importer.CommentListInfos.MapId.ContainsKey(id) && Importer.ParaId > 0)
				Importer.CommentListInfos.Add(listInfo);
		}
	}
	#endregion
	#region CommentListInfo
	public class CommentListInfo {
		string id;
		Int32 paraId;
		public string Id { get { return id; } set { id = value; } }
		public Int32 ParaId { get { return paraId; } set { paraId = value; } }
	}
	#endregion
	#region CommentListInfoCollection
	public class CommentListInfoCollection  {
		Dictionary<int, CommentListInfo> mapParaId = new Dictionary<int, CommentListInfo>();
		Dictionary<string, CommentListInfo> mapId = new Dictionary<string, CommentListInfo>();
		internal Dictionary<string, CommentListInfo> MapId { get { return mapId; } set { mapId = value; } }
		public void Add(CommentListInfo info) {
			mapParaId.Add(info.ParaId, info);
			mapId.Add(info.Id, info);
		}
		public CommentListInfo FindByParaId(int paraId) {
			CommentListInfo listInfo;
			if (mapParaId.TryGetValue(paraId, out listInfo))
				return listInfo;
			else
				return null;
		}
	}
	#endregion
}
