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
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Export.OpenDocument;
namespace DevExpress.XtraRichEdit.Import.OpenDocument {
	#region NoteDestination
	public class NoteDestination : ElementDestination {
		enum NoteType {
			Unknown = 0,
			FootNote,
			EndNote
		}
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("note-body", OnNoteBody);
			return result;
		}
		NoteType noteType;
		public NoteDestination(OpenDocumentTextImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		NoteType Type { get { return noteType; } }
		static NoteDestination GetThis(OpenDocumentTextImporter importer) {
			return (NoteDestination)importer.PeekDestination();
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string noteClass = ImportHelper.GetTextStringAttribute(reader, "note-class");
			if (noteClass == "footnote")
				this.noteType = NoteType.FootNote;
			else if (noteClass == "endnote")
				this.noteType = NoteType.EndNote;
			else
				this.noteType = NoteType.Unknown;
		}
		static Destination OnNoteBody(OpenDocumentTextImporter importer, XmlReader reader) {
			return GetThis(importer).OnNoteBody();
		}
		protected internal virtual Destination OnNoteBody() {
			switch (Type) {
				case NoteType.FootNote:
					return CreateFootNoteDestination();
				case NoteType.EndNote:
					return CreateEndNoteDestination();
				default:
					return null;
			}
		}
		protected internal virtual Destination CreateFootNoteDestination() {
			if (!DocumentModel.DocumentCapabilities.FootNotesAllowed)
				return null;
			FootNote note = new FootNote(DocumentModel);
			DocumentModel.UnsafeEditor.InsertFirstParagraph(note.PieceTable);
			return new FootNoteDestination(Importer, note);
		}
		protected internal virtual Destination CreateEndNoteDestination() {
			if (!DocumentModel.DocumentCapabilities.EndNotesAllowed)
				return null;
			EndNote note = new EndNote(DocumentModel);
			DocumentModel.UnsafeEditor.InsertFirstParagraph(note.PieceTable);
			return new EndNoteDestination(Importer, note);
		}
	}
	#endregion
	#region FootNoteDestinationBase<T> (abstract class)
	public abstract class FootNoteDestinationBase<T> : TextDestination where T : FootNoteBase<T> {
		readonly FootNoteRunBase<T> selfReferencedRun;
		protected FootNoteDestinationBase(OpenDocumentTextImporter importer, FootNoteBase<T> note)
			: base(importer) {
			Guard.ArgumentNotNull(note, "note");
			importer.PushCurrentPieceTable(note.PieceTable);
			this.selfReferencedRun = InsertReferenceRun(note.PieceTable, -1);
		}
		protected T Note { get { return (T)Importer.PieceTable.ContentType; } }
		public override void ProcessElementClose(XmlReader reader) {
			Importer.PieceTable.CheckIntegrity();
			Importer.PieceTable.FixLastParagraph();
			Importer.InsertBookmarks();
			Importer.InsertRangePermissions();
			List<T> notes = GetNoteCollection();
			int index = notes.Count;
			notes.Add(Note); 
			Importer.PopCurrentPieceTable();
			FootNoteRunBase<T> run = InsertReferenceRun(Importer.PieceTable, index);
			selfReferencedRun.NoteIndex = index;
			int styleIndex = DocumentModel.GetFootNoteReferenceCharacterStyle();
			run.CharacterStyleIndex = styleIndex;
			selfReferencedRun.CharacterStyleIndex = styleIndex;
		}
		protected internal abstract FootNoteRunBase<T> InsertReferenceRun(PieceTable pieceTable, int index);
		protected internal abstract List<T> GetNoteCollection();
	}
	#endregion
	#region FootNoteDestination
	public class FootNoteDestination : FootNoteDestinationBase<FootNote> {
		public FootNoteDestination(OpenDocumentTextImporter importer, FootNote note)
			: base(importer, note) {
		}
		protected internal override FootNoteRunBase<FootNote> InsertReferenceRun(PieceTable pieceTable, int index) {
			return pieceTable.InsertFootNoteRun(Importer.InputPosition, index);
		}
		protected internal override List<FootNote> GetNoteCollection() {
			return DocumentModel.FootNotes;
		}
	}
	#endregion
	#region EndNoteDestination
	public class EndNoteDestination : FootNoteDestinationBase<EndNote> {
		public EndNoteDestination(OpenDocumentTextImporter importer, EndNote note)
			: base(importer, note) {
		}
		protected internal override FootNoteRunBase<EndNote> InsertReferenceRun(PieceTable pieceTable, int index) {
			return pieceTable.InsertEndNoteRun(Importer.InputPosition, index);
		}
		protected internal override List<EndNote> GetNoteCollection() {
			return DocumentModel.EndNotes;
		}
	}
	#endregion
}
