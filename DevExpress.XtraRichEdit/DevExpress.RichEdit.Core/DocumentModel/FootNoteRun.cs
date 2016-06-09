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
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Layout;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Model {
	#region FootNoteRunBase<T> (abstract class)
	public abstract class FootNoteRunBase<T> : LayoutDependentTextRun where T : FootNoteBase<T> {
		int noteIndex;
		protected FootNoteRunBase(Paragraph paragraph)
			: base(paragraph) {
		}
		protected FootNoteRunBase(Paragraph paragraph, int startIndex, int length)
			: base(paragraph, startIndex, length) {
		}
		#region Properties
		public int NoteIndex { get { return noteIndex; } set { noteIndex = value; } }
		public T Note { get { return NoteCollection[NoteIndex]; } }
		public IList<T> NoteCollection { get { return GetNoteCollection(DocumentModel); } }
		protected internal abstract string CounterId { get; }
		protected internal abstract T CreateNote(DocumentModel documentModel);
		protected internal abstract IList<T> GetNoteCollection(DocumentModel documentModel);
		#endregion
		protected internal int CreateNoteCopy(DocumentModelCopyManager copyManager) {
			T source = Note;
			T target = CreateNote(copyManager.TargetModel);
			PieceTable sourcePieceTable = source.PieceTable;
			PieceTable targetPieceTable = target.PieceTable;
			DocumentModelCopyManager noteCopyManager = new DocumentModelCopyManager(sourcePieceTable, targetPieceTable, ParagraphNumerationCopyOptions.CopyAlways);
			noteCopyManager.TargetModel.UnsafeEditor.InsertFirstParagraph(targetPieceTable);
			CopySectionOperation operation = sourcePieceTable.DocumentModel.CreateCopySectionOperation(noteCopyManager);
			operation.FixLastParagraph = true;
			operation.Execute(sourcePieceTable.DocumentStartLogPosition, sourcePieceTable.DocumentEndLogPosition - sourcePieceTable.DocumentStartLogPosition + 1, false);
			IList<T> targetNoteCollection = GetNoteCollection(copyManager.TargetModel);
			int targetNoteIndex = targetNoteCollection.Count;
			targetNoteCollection.Add(target);
			return targetNoteIndex;
		}
		protected override void CopyContentCore(DocumentModelCopyManager copyManager) {
			DocumentLogPosition logPosition = copyManager.TargetPosition.LogPosition;
			ParagraphIndex paragraphIndex = copyManager.TargetPosition.ParagraphIndex;
			if (copyManager.TargetPieceTable.IsNote) {
				InsertFootNoteEndNoteRun(copyManager, paragraphIndex, logPosition, -1);
			} else {
				int targetNoteIndex = CreateNoteCopy(copyManager);
				InsertFootNoteEndNoteRun(copyManager, paragraphIndex, logPosition, targetNoteIndex);
			}
		}
		protected internal abstract void InsertFootNoteEndNoteRun(DocumentModelCopyManager copyManager, ParagraphIndex paragraphIndex, DocumentLogPosition logPosition, int noteIndex);
	}
	#endregion
	#region FootNoteRun
	public class FootNoteRun : FootNoteRunBase<FootNote> {
		public FootNoteRun(Paragraph paragraph)
			: base(paragraph) {
		}
		public FootNoteRun(Paragraph paragraph, int startIndex, int length)
			: base(paragraph, startIndex, length) {
		}
		protected internal override string CounterId { get { return FootNote.FootNoteCounterId; } }
		protected internal override RowProcessingFlags RowProcessingFlags {
			get {
				return base.RowProcessingFlags | RowProcessingFlags.ContainsFootNotes;
			}
			set {
				base.RowProcessingFlags = value;
			}
		}
		protected internal override IList<FootNote> GetNoteCollection(DocumentModel documentModel) {
			return documentModel.FootNotes;
		}
		protected internal override FootNote CreateNote(DocumentModel documentModel) {
			return new FootNote(documentModel);
		}
		public override void Export(IDocumentModelExporter exporter) {
			exporter.Export(this);
		}
		protected internal override void InsertFootNoteEndNoteRun(DocumentModelCopyManager copyManager, ParagraphIndex paragraphIndex, DocumentLogPosition logPosition, int noteIndex) {
			copyManager.TargetPieceTable.InsertFootNoteRun(paragraphIndex, logPosition, noteIndex);
		}
	}
	#endregion
	#region EndNoteRun
	public class EndNoteRun : FootNoteRunBase<EndNote> {
		public EndNoteRun(Paragraph paragraph)
			: base(paragraph) {
		}
		public EndNoteRun(Paragraph paragraph, int startIndex, int length)
			: base(paragraph, startIndex, length) {
		}
		protected internal override string CounterId { get { return EndNote.EndNoteCounterId; } }
		protected internal override RowProcessingFlags RowProcessingFlags {
			get {
				return base.RowProcessingFlags | RowProcessingFlags.ContainsEndNotes;
			}
			set {
				base.RowProcessingFlags = value;
			}
		}
		protected internal override IList<EndNote> GetNoteCollection(DocumentModel documentModel) {
			return documentModel.EndNotes;
		}
		protected internal override EndNote CreateNote(DocumentModel documentModel) {
			return new EndNote(documentModel);
		}
		public override void Export(IDocumentModelExporter exporter) {
			exporter.Export(this);
		}
		protected internal override void InsertFootNoteEndNoteRun(DocumentModelCopyManager copyManager, ParagraphIndex paragraphIndex, DocumentLogPosition logPosition, int noteIndex) {
			copyManager.TargetPieceTable.InsertEndNoteRun(paragraphIndex, logPosition, noteIndex);
		}
	}
	#endregion
	#region FootNoteNumberResultFormattingBase<T> (abstract class)
	public abstract class FootNoteNumberResultFormattingBase<T> : FieldResultFormatting where T : FootNoteBase<T> {
		static readonly string[] emptyGenericFormatting = new string[] { };
		protected FootNoteNumberResultFormattingBase()
			: base(null, emptyGenericFormatting) {
		}
		public override bool RecalculateOnSecondaryFormatting { get { return false; } }
		protected override string[] GeneralFormatting {
			get {
				string[] result = base.GeneralFormatting;
				return result == null ? emptyGenericFormatting : result;
			}
		}
		protected internal abstract string CounterId { get; }
		protected override int GetValueCore(ParagraphBoxFormatter formatter, DocumentModel documentModel) {
			Counter counter = formatter.RowsController.ColumnController.PageAreaController.PageController.DocumentLayout.Counters.GetCounter(CounterId);
			DocumentLogPosition logPosition = GetLogPosition(formatter);
			logPosition += formatter.Iterator.Offset;
			return counter.Increment(logPosition);
		}
		protected internal DocumentLogPosition GetLogPosition(ParagraphBoxFormatter formatter) {
			PieceTable pieceTable = formatter.PieceTable;
			if (pieceTable.IsNote) {
				FootNoteBase<T> note = (FootNoteBase<T>)pieceTable.ContentType;				
				FootNoteRunBase<T> run = note.ReferenceRun;
				Debug.Assert(run != null);
				pieceTable = run.Paragraph.PieceTable;
				return pieceTable.GetRunLogPosition(run);
			}
			else
				return pieceTable.GetRunLogPosition(formatter.Iterator.RunIndex);
		}
		protected internal abstract SectionFootNote GetFootNoteProperties(Section section);
		protected internal virtual SectionFootNote GetFootNoteProperties(ParagraphBoxFormatter formatter) {
			return GetFootNoteProperties(formatter.RowsController.ColumnController.PageAreaController.PageController.CurrentSection);
		}
		protected override string ApplyImplicitFormatting(ParagraphBoxFormatter formatter, string value, int intValue) {
			return GetFootNoteProperties(formatter).FormatCounterValue(intValue);
		}
	}
	#endregion
	#region FootNoteNumberResultFormatting
	public class FootNoteNumberResultFormatting : FootNoteNumberResultFormattingBase<FootNote> {
		static readonly FootNoteNumberResultFormatting instance = new FootNoteNumberResultFormatting();
		public static FootNoteNumberResultFormatting Instance { get { return instance; } }
		protected internal override string CounterId { get { return FootNote.FootNoteCounterId; } }
		protected internal override SectionFootNote GetFootNoteProperties(Section section) {
			return section.FootNote;
		}
	}
	#endregion
	#region EndNoteNumberResultFormatting
	public class EndNoteNumberResultFormatting : FootNoteNumberResultFormattingBase<EndNote> {
		static readonly EndNoteNumberResultFormatting instance = new EndNoteNumberResultFormatting();
		public static EndNoteNumberResultFormatting Instance { get { return instance; } }
		protected internal override string CounterId { get { return EndNote.EndNoteCounterId; } }
		protected internal override SectionFootNote GetFootNoteProperties(Section section) {
			return section.EndNote;
		}
	}
	#endregion
}
