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
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
namespace DevExpress.Web.ASPxRichEdit.Export {
	#region IRichEditServerGenerator
	public interface IRichEditServerGenerator {
		RichEditServerResponse GetResponse(WebFontInfoCache fontInfoCache, DocumentLogPosition start, int length);
		RichEditServerResponse GetResponse(WebFontInfoCache fontInfoCache, DocumentLogPosition start, int length, int chunkBufferSize);
		RichEditServerResponse GetResponse(WebFontInfoCache fontInfoCache, DocumentLogPosition start, Func<bool> predicate);
		RichEditServerResponse GetResponse(WebFontInfoCache fontInfoCache, DocumentLogPosition start, Func<bool> predicate, int chunkBufferSize);
	}
	#endregion
	#region RichEditServerResponseGenerator
	public class RichEditServerResponseGenerator : IRichEditServerGenerator {
		readonly PieceTable pieceTable;
		public RichEditServerResponseGenerator(PieceTable pieceTable) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
		}
		protected PieceTable PieceTable { get { return pieceTable; } }
		protected DocumentModel DocumentModel { get { return pieceTable.DocumentModel; } }
		public RichEditServerResponse GetResponse(WebFontInfoCache fontInfoCache, DocumentLogPosition start, int length) {
			return GetResponseCore(new FixedLengthIterator(new RichEditServerResponse(PieceTable, fontInfoCache), PieceTable, start, length));
		}
		public RichEditServerResponse GetResponse(WebFontInfoCache fontInfoCache, DocumentLogPosition start, int length, int chunkBufferSize) {
			return GetResponseCore(new FixedLengthIterator(new RichEditServerResponse(PieceTable, fontInfoCache), PieceTable, start, length, chunkBufferSize));
		}
		public RichEditServerResponse GetResponse(WebFontInfoCache fontInfoCache, DocumentLogPosition start, Func<bool> predicate) {
			return GetResponseCore(new PredicateIterator(new RichEditServerResponse(PieceTable, fontInfoCache), PieceTable, start, predicate));
		}
		public RichEditServerResponse GetResponse(WebFontInfoCache fontInfoCache, DocumentLogPosition start, Func<bool> predicate, int chunkBufferSize) {
			return GetResponseCore(new PredicateIterator(new RichEditServerResponse(PieceTable, fontInfoCache), PieceTable, start, predicate, chunkBufferSize));
		}
		RichEditServerResponse GetResponseCore(WebRichEditRunIteratorBase iterator) {
			do {
				iterator.MoveNext();
			} while (!iterator.IsFinished());
			return iterator.ServerResponse;
		}
	}
	#endregion
	#region WebRichEditRunIteratorBase
	public abstract class WebRichEditRunIteratorBase {
		readonly PieceTable pieceTable;
		readonly RichEditServerResponse serverResponse;
		DocumentLogPosition currentPos;
		RunIndex runIndex;
		int offset;
		ParagraphIndex paragraphIndex;
		WebParagraph paragraph;
		int defaultMaxBufferSize;
		protected WebRichEditRunIteratorBase(RichEditServerResponse serverResponse, PieceTable pieceTable, DocumentLogPosition start, int chunkBufferSize) {
			this.pieceTable = pieceTable;
			this.defaultMaxBufferSize = chunkBufferSize;
			this.serverResponse = serverResponse;
			Initialize(start);
		}
		public int DefaultMaxBufferSize { get { return defaultMaxBufferSize; } set { defaultMaxBufferSize = value; } }
		public RichEditServerResponse ServerResponse { get { return serverResponse; } }
		protected PieceTable PieceTable { get { return pieceTable; } }
		protected TextRunBase CurrentRun { get { return pieceTable.Runs[runIndex]; } }
		protected Paragraph CurrentParagraph { get { return pieceTable.Paragraphs[paragraphIndex]; } }
		protected DocumentLogPosition CurrentPosition { get { return currentPos; } }
		protected bool PieceTableFinished { get { return currentPos > pieceTable.DocumentEndLogPosition; } }
		void Initialize(DocumentLogPosition start) {
			this.currentPos = start;
			if (PieceTableFinished)
				return;
			DocumentModelPosition modelPos = PositionConverter.ToDocumentModelPosition(PieceTable, start);
			this.runIndex = modelPos.RunIndex;
			this.offset = start - modelPos.RunStartLogPosition;
			this.paragraphIndex = modelPos.ParagraphIndex;
			StartNewParagraph();
		}
		Chunk StartNewChunk() {
			if (IsFinished())
				return null;
			Chunk result = new Chunk();
			result.Start = currentPos;
			result.MaxBufferSize = CalcChunkBufferSize();
			return result;
		}
		void StartNewParagraph() {
			this.paragraph = new WebParagraph(ServerResponse);
			Paragraph sourceParagraph = CurrentParagraph;
			this.paragraph.LogPosition = sourceParagraph.LogPosition;
			this.paragraph.EndLogPosition = sourceParagraph.EndLogPosition;
			this.paragraph.ParagraphPropertiesIndex = sourceParagraph.ParagraphProperties.Index;
			this.paragraph.MergedParagraphFormattingCacheIndex = sourceParagraph.MergedParagraphFormattingCacheIndex;
			this.paragraph.ParagraphStyleIndex = sourceParagraph.ParagraphStyleIndex;
			this.paragraph.Tabs = new WebTabsCollection(sourceParagraph.Tabs);
			this.paragraph.NumberingListIndex = ((IConvertToInt<NumberingListIndex>)sourceParagraph.NumberingListIndex).ToInt();
			this.paragraph.ListLevelIndex = sourceParagraph.GetListLevelIndex();
		}
		int CalcChunkBufferSize() {
			return CanSplitRun(CurrentRun) ? DefaultMaxBufferSize : Math.Max(DefaultMaxBufferSize, CurrentRun.Length - offset);
		}
		public void MoveNext() {
			Chunk chunk = GetNextChunk();
			if (chunk != null)
				this.serverResponse.Chunks.Add(chunk);
		}
		Chunk GetNextChunk() {
			Chunk result = StartNewChunk();
			bool finished = Object.ReferenceEquals(result, null);
			while (!finished)
				finished = !MoveNextCore(result);
			if (result != null && PieceTableFinished)
				result.IsLast = true;
			return result;
		}
		bool MoveNextCore(Chunk chunk) {
			if (chunk.Length == chunk.MaxBufferSize || IsFinished())
				return false;
			if (currentPos > this.paragraph.EndLogPosition) {
				paragraphIndex++;
				StartNewParagraph();
			}
			int length = CalcNextRunLength();
			if (length <= chunk.MaxBufferSize - chunk.Length) {
				Advance(chunk, length);
				return true;
			}
			if (CanSplitRun(CurrentRun)) {
				length = chunk.MaxBufferSize - chunk.Length;
				Advance(chunk, length);
				return true;
			}
			return false;
		}
		protected virtual int CalcNextRunLength() {
			return CurrentRun.Length - this.offset;
		}
		void Advance(Chunk chunk, int length) {
			AppendRun(chunk, length);
			this.currentPos += length;
			if (this.offset == CurrentRun.Length) {
				this.runIndex++;
				this.offset = 0;
			}
		}
		void AppendRun(Chunk chunk, int length) {
			WebTextRunBase run = CreateRun(chunk.Length, length);
			chunk.AppendRun(run, CurrentRun.GetText(PieceTable.TextBuffer, offset, offset + length - 1));
			this.offset += length;
		}
		WebTextRunBase CreateRun(int startIndex, int length) {
			WebTextRunBase result = WebTextRunFactory.CreateRun(CurrentRun.GetType(), this.paragraph, startIndex, length);
			result.ApplyFormatting(CurrentRun);
			return result;
		}
		bool CanSplitRun(TextRunBase run) {
			return run is TextRun;
		}
		public bool IsFinished() {
			if (PieceTableFinished)
				return true;
			return IsFinishedCore();
		}
		protected abstract bool IsFinishedCore();
	}
	#endregion
	#region FixedLengthIterator
	public class FixedLengthIterator : WebRichEditRunIteratorBase {
		DocumentLogPosition end;
		public FixedLengthIterator(RichEditServerResponse serverResponse, PieceTable pieceTable, DocumentLogPosition start, int length)
			: this(serverResponse, pieceTable, start, length, Chunk.DefaultMaxBufferSize) {
		}
		public FixedLengthIterator(RichEditServerResponse serverResponse, PieceTable pieceTable, DocumentLogPosition start, int length, int chunkBufferSize)
			: base(serverResponse, pieceTable, start, chunkBufferSize) {
			end = start + length - 1;
		}
		protected override int CalcNextRunLength() {
			return Math.Min(base.CalcNextRunLength(), this.end - CurrentPosition + 1);
		}
		protected override bool IsFinishedCore() {
			return CurrentPosition > this.end;
		}
	}
	#endregion
	#region PredicateIterator
	public class PredicateIterator : WebRichEditRunIteratorBase {
		Func<bool> predicate;
		public PredicateIterator(RichEditServerResponse serverResponse, PieceTable pieceTable, DocumentLogPosition start, Func<bool> predicate)
			: this(serverResponse, pieceTable, start, predicate, Chunk.DefaultMaxBufferSize) {
		}
		public PredicateIterator(RichEditServerResponse serverResponse, PieceTable pieceTable, DocumentLogPosition start, Func<bool> predicate, int chunkBufferSize)
			: base(serverResponse, pieceTable, start, chunkBufferSize) {
			this.predicate = predicate;
		}
		protected override bool IsFinishedCore() {
			if (predicate == null)
				return false;
			return predicate();
		}
	}
	#endregion
}
