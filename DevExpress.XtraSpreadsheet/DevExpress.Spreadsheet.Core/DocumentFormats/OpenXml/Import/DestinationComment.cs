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
using System.Xml;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region CommentsDestination
	public class CommentsDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("authors", OnAuthors);
			result.Add("commentList", OnCommentList);
			return result;
		}
		#region Fields
		readonly Worksheet previousWorksheet;
		#endregion
		#region Properties
		internal new OpenXmlImporter Importer { get { return (OpenXmlImporter)base.Importer; } }
		#endregion
		public CommentsDestination(SpreadsheetMLBaseImporter importer, Worksheet worksheet)
			: base(importer) {
			Guard.ArgumentNotNull(worksheet, "worksheet");
			this.previousWorksheet = Importer.CurrentWorksheet;
			Importer.CurrentWorksheet = worksheet;
			Importer.CommentAuthorIds.Clear();
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementClose(XmlReader reader) {
			foreach (Comment comment in Importer.CurrentWorksheet.Comments){
				comment.AuthorId = (Importer as OpenXmlImporter).CommentAuthorIds[comment.AuthorId];
			}
			Importer.CurrentWorksheet = previousWorksheet;
		}
		static Destination OnAuthors(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new CommentAuthorsDestination(importer);
		}
		static Destination OnCommentList(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new CommentListDestination(importer);
		}
	}
	#endregion
	#region CommentAuthorsDestination
	public class CommentAuthorsDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("author", OnAuthor);
			return result;
		}
		#region Field
		int index = 0;
		#endregion
		public CommentAuthorsDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
			index = 0;
		}
		static CommentAuthorsDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (CommentAuthorsDestination)importer.PeekDestination();
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		static Destination OnAuthor(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new CommentAuthorDestination(importer, GetThis(importer).index++);
		}
	}
	#endregion
	#region CommentAuthorDestination
	public class CommentAuthorDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		int index;
		bool hasValue = false;
		public CommentAuthorDestination(SpreadsheetMLBaseImporter importer, int index)
			: base(importer) {
			this.index = index;
		}
		void AddAuthor(string name) {
			int global_id = Importer.DocumentModel.CommentAuthors.AddIfNotPresent(name);
			(Importer as OpenXmlImporter).CommentAuthorIds.Add(index, global_id);
			hasValue = true;
		}
		public override bool ProcessText(XmlReader reader) {
			AddAuthor(reader.Value);
			return base.ProcessText(reader);
		}
		public override void ProcessElementClose(XmlReader reader) {
			if(!hasValue)
				AddAuthor(reader.Value);
			base.ProcessElementClose(reader);
		}
	}
	#endregion
	#region CommentListDestination
	public class CommentListDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Fields
		Dictionary<CellPosition, int> cachedCellPositionsForComments = new Dictionary<CellPosition, int>();
		#endregion
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("comment", OnComment);
			return result;
		}
		public CommentListDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
			GenerateCachedCellPositionsForComments();
		}
		void GenerateCachedCellPositionsForComments() {
			if(Importer.CurrentWorksheet.VmlDrawing == null)
				return;
			foreach(KeyValuePair<int, VmlShape> pair in Importer.CurrentWorksheet.VmlDrawing.Shapes.InnerCollection) {
				if(pair.Value.ClientData.Column != -1 && pair.Value.ClientData.Row != -1) {
					CellPosition cellPosition = new CellPosition(pair.Value.ClientData.Column, pair.Value.ClientData.Row);
					if(!cachedCellPositionsForComments.ContainsKey(cellPosition))
						cachedCellPositionsForComments.Add(cellPosition, pair.Key);
				}
			}
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		static Destination OnComment(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new CommentDestination(importer, ((CommentListDestination) importer.PeekDestination()).cachedCellPositionsForComments);
		}
	}
	#endregion
	#region CommentDestination
	public class CommentDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Fields
		Dictionary<CellPosition, int> cachedCellPositionsForComments;
		#endregion
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("text", OnCommentText);
			return result;
		}
		#region Fields
		Worksheet sheet;
		Comment comment;
		#endregion
		public CommentDestination(SpreadsheetMLBaseImporter importer, Dictionary<CellPosition, int> cachedCellPositionsForComments)
			: base(importer) {
			this.sheet = importer.CurrentWorksheet;
			this.cachedCellPositionsForComments = cachedCellPositionsForComments;
		}
		#region Properties
		internal new OpenXmlImporter Importer { get { return (OpenXmlImporter)base.Importer; } }
		#endregion
		static CommentDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (CommentDestination)importer.PeekDestination();
		}
		public override void ProcessElementOpen(XmlReader reader) {
			int authorId = Importer.GetWpSTIntegerValue(reader, "authorId", -1);
			if (authorId < 0)
				Importer.ThrowInvalidFile("Has no author id");
			CellPosition position = Importer.ReadCellPosition(reader, "ref");
			if (!position.IsValid)
				Importer.ThrowInvalidFile("Comment position is invalid");
			int shapeId = LookupVmlShapeIdByPosition(position);
			if (shapeId < 0)
				Importer.ThrowInvalidFile("Can't lookup vml shape");
			comment = new Comment(sheet, position, authorId, shapeId);
			sheet.Comments.Add(comment);
			base.ProcessElementOpen(reader);
		}
		int LookupVmlShapeIdByPosition(CellPosition position) {
			int result;
			if(!cachedCellPositionsForComments.TryGetValue(position, out result))
				result = -1;
			return result;
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		static Destination OnCommentText(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new CommentTextDestination(importer, GetThis(importer).comment);
		}
	}
	#endregion
	#region CommentTextDestination
	public class CommentTextDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("r", OnRun);
			result.Add("t", OnSimpleText);
			return result;
		}
		#region Fields
		Comment comment;
		#endregion
		public CommentTextDestination(SpreadsheetMLBaseImporter importer, Comment comment)
			: base(importer) {
				this.comment = comment;
		}
		static CommentTextDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (CommentTextDestination)importer.PeekDestination();
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		static Destination OnRun(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			CommentTextDestination thisDestination = GetThis(importer);
			CommentRun commentRun = new CommentRun(thisDestination.comment.Worksheet);
			thisDestination.comment.Runs.AddCore(commentRun);
			return new CommentRunDestination(importer, commentRun);
		}
		static Destination OnSimpleText(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			CommentTextDestination thisDestination = GetThis(importer);
			CommentRun commentRun = new CommentRun(thisDestination.comment.Worksheet);
			thisDestination.comment.Runs.AddCore(commentRun);
			return new CommentRunTextDestination(importer, commentRun);
		}
	}
	#endregion
	#region TextRunDestination
	public class CommentRunDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		protected static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("rPr", OnRunProperties);
			result.Add("t", OnTextString);
			return result;
		}
		static CommentRunDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (CommentRunDestination)importer.PeekDestination();
		}
		readonly CommentRun commentRun;
		public CommentRunDestination(SpreadsheetMLBaseImporter importer, CommentRun commentRun)
			: base(importer) {
				this.commentRun = commentRun;
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		static Destination OnRunProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new RunPropertiesDestination(importer, GetThis(importer).commentRun.Info);
		}
		static Destination OnTextString(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new CommentRunTextDestination(importer, GetThis(importer).commentRun);
		}
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			commentRun.BeginUpdate();
		}
		public override void ProcessElementClose(XmlReader reader) {
			commentRun.EndUpdate();
		}
	}
	#endregion
	#region CommentRunTextDestination
	public class CommentRunTextDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		CommentRun commentRun;
		public CommentRunTextDestination(SpreadsheetMLBaseImporter importer, CommentRun commentRun)
			: base(importer) {
				Guard.ArgumentNotNull(commentRun, "CommentRun");
			this.commentRun = commentRun;
		}
		public override bool ProcessText(XmlReader reader) {
			string text = reader.Value;
			if (!String.IsNullOrEmpty(text))
				commentRun.Text = text;
			return true;
		}
	}
	#endregion
}
