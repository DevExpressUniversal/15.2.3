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
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
namespace DevExpress.XtraRichEdit.Import.Rtf {
	#region CommentStartPositionDestination
	public class CommentStartPositionDestination : StringValueDestination {
		public CommentStartPositionDestination(RtfImporter rtfImporter) : base(rtfImporter) { }
		protected internal override StringValueDestination CreateEmptyClone() {
			return new CommentStartPositionDestination(Importer);
		}
		public override void AfterPopRtfState() {
			RTFImportCommentInfo comment = Importer.GetCommentInfo(Value);
			DocumentLogPosition position = Importer.Position.LogPosition;
			comment.Start = position;
		}
	}
	#endregion
	#region CommentEndPositionDestination
	public class CommentEndPositionDestination : StringValueDestination {
		public CommentEndPositionDestination(RtfImporter rtfImporter) : base(rtfImporter) { }
		protected internal override StringValueDestination CreateEmptyClone() {
			return new CommentEndPositionDestination(Importer);
		}
		public override void AfterPopRtfState() {
			RTFImportCommentInfo comment = Importer.GetCommentInfo(Value);
			DocumentLogPosition position = Importer.Position.LogPosition;
			comment.End = position;
		}
	}
	#endregion
	#region CommentNameDestination
	public class CommentNameDestination : StringValueDestination {
		public CommentNameDestination(RtfImporter rtfImporter) : base(rtfImporter) { }
		protected internal override StringValueDestination CreateEmptyClone() {
			return new CommentNameDestination(Importer);
		}
		public override void AfterPopRtfState() {
			Importer.ActiveCommentName = Value;
		}
	}
	#endregion
	#region CommentAuthorDestination
	public class CommentAuthorDestination : StringValueDestination {
		public CommentAuthorDestination(RtfImporter rtfImporter) : base(rtfImporter) { }
		protected internal override StringValueDestination CreateEmptyClone() {
			return new CommentAuthorDestination(Importer);
		}
		public override void AfterPopRtfState() {
			Importer.ActiveCommentAuthor = Value;
		}
	}
	#endregion
	#region CommentAnnotationDestination
	public class CommentAnnotationDestination : DestinationPieceTable {
		readonly string defaultId = "dxcommentId";
		DateTime date = Comment.MinCommentDate;
		string idParent = String.Empty;
		bool isNewId = false;
		#region CreateControlCharTable
		static ControlCharTranslatorTable controlCharHT = CreateControlCharTable();
		static ControlCharTranslatorTable CreateControlCharTable() {
			ControlCharTranslatorTable table = new ControlCharTranslatorTable();
			table.Add('\\', OnEscapedChar);
			table.Add('{', OnEscapedChar);
			table.Add('}', OnEscapedChar);
			return table;
		}
		#endregion
		#region CreateKeywordTable
		static KeywordTranslatorTable keywordHT = CreateAnnotationKeyword();
		static KeywordTranslatorTable CreateAnnotationKeyword() {
			KeywordTranslatorTable table = new KeywordTranslatorTable();
			AddCommonCharacterKeywords(table);
			AddCommonParagraphKeywords(table);
			AddCommonSymbolsAndObjectsKeywords(table);
			AddCommonTabKeywords(table);
			AddCommonNumberingListsKeywords(table);
			AppendTableKeywords(table);
			table.Add("atnref", OnCommentRefKeyword);
			table.Add("atndate", OnCommentDateKeyword);
			table.Add("atnparent", OnCommentParentKeyword); 
			return table;
		}
		#endregion
		#region Keyword handlers
		static void OnCommentDateKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Destination = new CommentDateDestination(importer);
		}
		static void OnCommentRefKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Destination = new CommentRefDestination(importer);
		}
		static void OnCommentParentKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Destination = new CommentParentDestination(importer);
		}
		#endregion
		static CommentAnnotationDestination GetThis(RtfImporter rtfImporter) {
			return (CommentAnnotationDestination)rtfImporter.Destination;
		} 
		public CommentAnnotationDestination(RtfImporter rtfImporter, CommentContentType commentContentType)
			: base(rtfImporter, commentContentType.PieceTable) {
		}
		#region Properties
		protected override KeywordTranslatorTable KeywordHT { get { return keywordHT; } }
		protected internal CommentContentType Content { get { return (CommentContentType)PieceTable.ContentType; } }
		protected internal string Id { get; set; }
		protected internal DateTime Date { get { return date; } set { date = value; } }
		protected internal string IdParent { get { return idParent; } set { idParent = value; } }
		#endregion 
		protected override DestinationBase CreateClone() {
			CommentAnnotationDestination clone = new CommentAnnotationDestination(Importer, Content);
			clone.Id = Id;
			clone.Date = Date;
			clone.IdParent = IdParent;
			return clone;
		}
		public override void NestedGroupFinished(DestinationBase nestedDestination) {
			base.NestedGroupFinished(nestedDestination);
			CommentRefDestination refDest = nestedDestination as CommentRefDestination;
			if (refDest != null)
				Id = refDest.Value;
			CommentDateDestination dateDest = nestedDestination as CommentDateDestination;
			if (dateDest != null) 
				Date = ParseDate(dateDest.Value);
			CommentParentDestination parentDest = nestedDestination as CommentParentDestination;
			if (parentDest != null)
				IdParent = parentDest.Value;
		}
		DateTime ParseDate(string value) {
			int dateTime_DTTM;
			if (Int32.TryParse(value, out dateTime_DTTM))
				return DateTimeUtils.FromDateTimeDTTM(dateTime_DTTM);
			return Comment.MinCommentDate;
		}
		public override void AfterPopRtfState() {
			if (String.IsNullOrEmpty(Id)) {
				Id = CreateUniqueId(Importer.CommentsId);
				isNewId = true;
			}
			else
				isNewId = false;
			RTFImportCommentInfo comment = Importer.GetCommentInfo(Id);
			comment.Id = Id;
			comment.Author = CalculateCommentAuthor();
			comment.Name = CalculateCommentName();
			comment.Date = Date;
			comment.ParaId = CalculateCommentParaid();
			comment.Content = Content;
			comment.HasReference = true;
			comment.Reference = Importer.Position.LogPosition;
			if ((comment.Start < DocumentLogPosition.Zero) && isNewId)
				comment.Start = Importer.Position.LogPosition;
			if ((comment.End < DocumentLogPosition.Zero) && isNewId)
				comment.End = Importer.Position.LogPosition;
		}
		string CreateUniqueId(List<string> commentsId) {
			string newId = defaultId;
			int suffix = 0;
			while (commentsId.Contains(newId)) {
				newId = defaultId + suffix;
				suffix++;
			}
			commentsId.Add(newId);
			return newId;
		}
		int CalculateCommentParaid() {
			if (idParent != String.Empty)
				return Convert.ToInt32(IdParent);
			return 0;
		}
		string CalculateCommentName() {
			if (Importer.ActiveCommentName == null)
				return String.Empty;
			else
				return Importer.ActiveCommentName;
		}
		string CalculateCommentAuthor() {
			if (Importer.ActiveCommentAuthor == null)
				return String.Empty;
			else
				return Importer.ActiveCommentAuthor;
		}
		protected override void InsertComments() {
		} 
	}
	#endregion
	#region CommentDateDestination
	public class CommentDateDestination :  StringValueDestination {
		public CommentDateDestination(RtfImporter rtfImporter) : base(rtfImporter) { }
		protected internal override StringValueDestination CreateEmptyClone() {
			return new CommentDateDestination(Importer);
		}
	}
	#endregion
	#region CommentRefDestination
	public class CommentRefDestination :  StringValueDestination {
		public CommentRefDestination(RtfImporter rtfImporter) : base(rtfImporter) { }
		protected internal override StringValueDestination CreateEmptyClone() {
			return new CommentRefDestination(Importer);			
		}
		public override void AfterPopRtfState() {
			if (!Importer.CommentsRef.ContainsKey(Value)) {
				Importer.CommentsRef.Add(Value, Importer.RefIndex);
				Importer.CommentsIndexRef.Add(Importer.RefIndex, Value);
				Importer.RefIndex++;
			}
		}
	}
	#endregion
	#region CommentParentDestination
	public class CommentParentDestination :  StringValueDestination {
		public CommentParentDestination(RtfImporter rtfImporter) : base(rtfImporter) { }
		protected internal override StringValueDestination CreateEmptyClone() {
			return new CommentParentDestination(Importer);
		}
	}
	#endregion
}
