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
using DevExpress.XtraRichEdit.Model;
using System.Collections.Generic;
using DevExpress.Utils;
namespace DevExpress.XtraRichEdit.Import.Rtf {
	#region FootNoteDestinationBase<T> (abstract class)
	public abstract class FootNoteDestinationBase<T> : DestinationPieceTable where T : FootNoteBase<T> {
		static KeywordTranslatorTable keywordHT = CreateKeywordTable();
		static KeywordTranslatorTable CreateKeywordTable() {
			KeywordTranslatorTable table = new KeywordTranslatorTable();
			AddCommonCharacterKeywords(table);
			AddCommonParagraphKeywords(table);
			AddCommonSymbolsAndObjectsKeywords(table);
			AddCommonTabKeywords(table);
			AddCommonNumberingListsKeywords(table);
			AppendTableKeywords(table);
			table.Add("chftn", OnFootNoteSelfReference);
			table.Add("ftnalt", OnConvertToEndNote);
			return table;
		}
		readonly List<FootNoteRunBase<T>> selfReferenceRuns;
		protected FootNoteDestinationBase(RtfImporter importer, T targetPieceTable)
			: this(importer, targetPieceTable, new List<FootNoteRunBase<T>>()) {
		}
		protected FootNoteDestinationBase(RtfImporter importer, T footNoteContentType, List<FootNoteRunBase<T>> selfReferenceRuns)
			: base(importer, footNoteContentType.PieceTable) {
			Guard.ArgumentNotNull(selfReferenceRuns, "selfReferenceRuns");
			this.selfReferenceRuns = selfReferenceRuns;
		}
		protected override KeywordTranslatorTable KeywordHT { get { return keywordHT; } }
		protected internal List<FootNoteRunBase<T>> SelfReferenceRuns { get { return selfReferenceRuns; } }
		protected internal T Note { get { return (T)PieceTable.ContentType; } }
		public override void FinalizePieceTableCreation() {
			base.FinalizePieceTableCreation();
			List<T> notes = GetNoteCollection();
			int index = notes.Count;
			notes.Add(Note);
			int count = selfReferenceRuns.Count;
			for (int i = 0; i < count; i++)
				selfReferenceRuns[i].NoteIndex = index;
		}
		static void OnConvertToEndNote(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (importer.Destination is EndNoteDestination)
				return;
			EndNote note = new EndNote(importer.DocumentModel);
			importer.DocumentModel.UnsafeEditor.InsertFirstParagraph(note.PieceTable);
			importer.Destination = new EndNoteDestination(importer, note);
		}
		static void OnFootNoteSelfReference(RtfImporter importer, int parameterValue, bool hasParameter) {
			FootNoteDestinationBase<T> thisDestination = (FootNoteDestinationBase<T>)importer.Destination;
			thisDestination.OnSelfReference();
		}
		protected internal abstract void OnSelfReference();
		protected internal abstract List<T> GetNoteCollection();
	}
	#endregion
	#region FootNoteDestination
	public class FootNoteDestination : FootNoteDestinationBase<FootNote> {
		public FootNoteDestination(RtfImporter importer, FootNote targetPieceTable)
			: base(importer, targetPieceTable) {
		}
		FootNoteDestination(RtfImporter importer, FootNote targetPieceTable, List<FootNoteRunBase<FootNote>> selfReferenceRuns)
			: base(importer, targetPieceTable, selfReferenceRuns) {
		}
		protected override DestinationBase CreateClone() {
			return new FootNoteDestination(Importer, Note, SelfReferenceRuns);
		}
		protected internal override void OnSelfReference() {
			FootNoteRun run = Note.PieceTable.InsertFootNoteRun(Importer.Position, -1);
			SelfReferenceRuns.Add(run);
		}
		protected internal override List<FootNote> GetNoteCollection() {
			return DocumentModel.FootNotes;
		}
	}
	#endregion
	#region EndNoteDestination
	public class EndNoteDestination : FootNoteDestinationBase<EndNote> {
		public EndNoteDestination(RtfImporter importer, EndNote targetPieceTable)
			: base(importer, targetPieceTable) {
		}
		EndNoteDestination(RtfImporter importer, EndNote targetPieceTable, List<FootNoteRunBase<EndNote>> selfReferenceRuns)
			: base(importer, targetPieceTable, selfReferenceRuns) {
		}
		protected override DestinationBase CreateClone() {
			return new EndNoteDestination(Importer, Note, SelfReferenceRuns);
		}
		protected internal override void OnSelfReference() {
			EndNoteRun run = Note.PieceTable.InsertEndNoteRun(Importer.Position, -1);
			SelfReferenceRuns.Add(run);
		}
		protected internal override List<EndNote> GetNoteCollection() {
			return DocumentModel.EndNotes;
		}
	}
	#endregion
}
