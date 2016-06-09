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
using DevExpress.XtraRichEdit.Export.OpenDocument;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.XtraRichEdit.Import.OpenDocument {
	#region AnnotationDestination
	public class AnnotationDestination : ElementDestination {
		#region CreateElementHandlerTable
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("creator", OnAuthorDestination);
			result.Add("date", OnDateDestination);
			result.Add("p", OnParagraph);
			result.Add("list", OnList);
			return result;
		}
		#endregion
		#region Fields
		readonly ImportCommentInfo activeComment;
		readonly CommentContentType commentContentType;
		#endregion
		public AnnotationDestination(OpenDocumentTextImporter importer) 
			: base(importer) {
			activeComment = new ImportCommentInfo(Importer.DocumentModel.MainPieceTable);
			commentContentType = new CommentContentType(DocumentModel);
		}
		#region Properties
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		protected internal ImportCommentInfo ActiveComment { get { return activeComment; } }
		protected internal CommentContentType CommentContentType { get { return commentContentType; } }
		#endregion
		static AnnotationDestination GetThis(OpenDocumentTextImporter importer) {
			return (AnnotationDestination)importer.PeekDestination();
		}
		#region Element Handler
		static Destination OnAuthorDestination(OpenDocumentTextImporter importer, XmlReader reader) {
			return new CommentAuthorDestination(importer, GetThis(importer).ActiveComment);
		}
		static Destination OnDateDestination(OpenDocumentTextImporter importer, XmlReader reader) {
			return new CommentDateDestination(importer, GetThis(importer).ActiveComment);
		}
		static Destination OnParagraph(OpenDocumentTextImporter importer, XmlReader reader) {
			return new ParagraphDestination(importer);
		}
		public static Destination OnList(OpenDocumentTextImporter importer, XmlReader reader) {
			return new ListDestination(importer);
		}
		#endregion
		string GetCommentName() {
			return Importer.Comments.Count.ToString();
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string name = GetCommentName();
			ActiveComment.Name = name;
			DocumentLogPosition position = Importer.InputPosition.LogPosition;
			ActiveComment.Start = position;
			ActiveComment.End = position;
			Importer.Comments.Add(name, ActiveComment);
			Importer.PushCurrentPieceTable(CommentContentType.PieceTable);
		}
		public override void ProcessElementClose(XmlReader reader) {
			Importer.PieceTable.CheckIntegrity();
			Importer.PieceTable.FixLastParagraph();
			Importer.PopCurrentPieceTable();
			ActiveComment.Content = CommentContentType;
			ActiveComment.HasReference = true;
		}
	}
	#endregion
	#region CommentElementDestinationBase (abstract class)
	public abstract class CommentElementDestinationBase : ElementDestination {
		static readonly ElementHandlerTable handlerTable = new ElementHandlerTable();
		readonly ImportCommentInfo activeComment;
		protected CommentElementDestinationBase(OpenDocumentTextImporter importer, ImportCommentInfo activeComment)
			: base(importer) {
			this.activeComment = activeComment;
		}
		#region Properties
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		protected internal ImportCommentInfo ActiveComment { get { return activeComment; } }
		#endregion
		public override bool ProcessText(XmlReader reader) {
			string textContent = reader.Value;
			AssignCommentProperty(ActiveComment, textContent);
			return true;
		}
		protected internal abstract void AssignCommentProperty(ImportCommentInfo comment, string value);
	}
	#endregion
	#region CommentAuthorDestination
	public class CommentAuthorDestination : CommentElementDestinationBase {
		public CommentAuthorDestination(OpenDocumentTextImporter importer, ImportCommentInfo activeComment)
			: base(importer, activeComment) {
		}
		protected internal override void AssignCommentProperty(ImportCommentInfo comment, string value) {
			if (String.IsNullOrEmpty(value))
				return;
			comment.Author = value;
		}
	}
	#endregion
	#region CommentDateDestination
	public class CommentDateDestination : CommentElementDestinationBase {
		public CommentDateDestination(OpenDocumentTextImporter importer, ImportCommentInfo activeComment)
			: base(importer, activeComment) {
		}
		protected internal override void AssignCommentProperty(ImportCommentInfo comment, string value) {
			if (String.IsNullOrEmpty(value))
				return;
			DateTime dateTime;
			if (DateTime.TryParseExact(value, "yyyy-MM-ddTHH:mm:ss.FF", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime)) {
				if (dateTime > Comment.MinCommentDate)
					comment.Date = dateTime;
				else
					comment.Date = Comment.MinCommentDate;
			}
			else
				comment.Date = Comment.MinCommentDate;
		}
	}
	#endregion
}
