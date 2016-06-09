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
using System.Diagnostics;
using DevExpress.Office;
using System.IO;
using System.Xml;
using DevExpress.XtraSpreadsheet.Model;
using System.Collections.Generic;
using DevExpress.Utils.Zip;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		#region Fields
		int commentCounter;
		Dictionary<int, int> commentWorksheetAuthorsIdConvertedTable = new Dictionary<int, int>();
		#endregion
		protected internal virtual void AddCommentsContent() {
			if (ActiveSheet.Comments.Count == 0)
				return;
			string path = PopulateCommentRelation();
			AddPackageContent(path, ExportCommentContent());
		}
		protected internal virtual string PopulateCommentRelation() {
			OpenXmlRelationCollection relations = SheetRelationsTable[ActiveSheet.Name];
			string id = GenerateIdByCollection(relations);
			this.commentCounter++;
			string fileName = String.Format("comments{0}.xml", this.commentCounter);
			string target = "../" + fileName;
			relations.Add(new OpenXmlRelation(id, target, RelsCommentsNamepace));
			Builder.OverriddenContentTypes.Add("/xl/" + fileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.comments+xml");
			return @"xl\" + fileName;
		}
		protected internal virtual CompressedStream ExportCommentContent() {
			return CreateXmlContent(GenerateCommentXmlContent);
		}
		protected internal virtual void GenerateCommentXmlContent(XmlWriter writer) {
			DocumentContentWriter = writer;
			GenerateCommentContent();
		}
		protected internal virtual void GenerateCommentContent() {
			WriteShStartElement("comments");
			try {
				CommentAuthorCollection authors = GetWorksheetAuthors();
				GenerateCommentAuthorsContent(authors);
				GenerateCommentListContent(ActiveSheet.Comments);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual CommentAuthorCollection GetWorksheetAuthors() {
			commentWorksheetAuthorsIdConvertedTable.Clear();
			CommentAuthorCollection result = new CommentAuthorCollection();
			foreach (Comment comment in ActiveSheet.Comments) {
				int globalAuthorId = comment.AuthorId;
				string author = Workbook.CommentAuthors[globalAuthorId];
				if (!commentWorksheetAuthorsIdConvertedTable.ContainsKey(globalAuthorId)) {
					int authorIndexInSheetCollection = result.AddIfNotPresent(author);
					commentWorksheetAuthorsIdConvertedTable.Add(globalAuthorId, authorIndexInSheetCollection);
				}
			}
			return result;
		}
		protected internal virtual void GenerateCommentAuthorsContent(CommentAuthorCollection authors) {
			Debug.Assert(authors.Count > 0);
			WriteShStartElement("authors");
			try {
				authors.ForEach(GenerateCommentAuthorContent);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual void GenerateCommentAuthorContent(string author) {
			WriteShString("author", author, true);
		}
		protected internal virtual void GenerateCommentListContent(CommentCollection comments) {
			WriteShStartElement("commentList");
			try {
				comments.ForEach(GenerateCommentContent);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual void GenerateCommentContent(Comment comment) {
			WriteShStartElement("comment");
			try {
				GenerateCommentAttributesContent(comment);
				GenerateCommentTextContent(comment);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual void GenerateCommentAttributesContent(Comment comment) {
			WriteStringValue("ref", comment.Reference.ToString());
			int authorId = commentWorksheetAuthorsIdConvertedTable[comment.AuthorId];
			WriteShIntValue("authorId", authorId);
		}
		protected internal virtual void GenerateCommentTextContent(Comment comment) {
			WriteShStartElement("text");
			try {
				int count = comment.Runs.Count;
				for(int i = 0; i < count; i++)
					GenerateCommentRunContent(comment.Runs[i], i);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal void GenerateCommentRunContent(CommentRun run, int index) {
			WriteShStartElement("r");
			try {
				GenerateRunFontPropertiesContent(run.Info, index);
				Debug.Assert(!String.IsNullOrEmpty(run.Text));
				WriteShString("t", run.Text, true);
			}
			finally {
				WriteShEndElement();
			}
		}
	}
}
