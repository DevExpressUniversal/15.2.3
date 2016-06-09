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
using System.IO;
using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraPrinting.Native;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Import.Doc {
	#region DocBookmark
	public class DocBookmark {
		#region Fields
		int originalStartPosition;
		int originalEndPosition;
		string name;
		#endregion
		public DocBookmark(int originalStartPosition, int originalEndPosition, string name) {
			this.originalStartPosition = originalStartPosition;
			this.originalEndPosition = originalEndPosition;
			this.name = name;
		}
		#region Properties
		public int OriginalStartPosition { get { return originalStartPosition; } }
		public int OriginalEndPosition { get { return originalEndPosition; } }
		public string Name { get { return name; } }
		#endregion
	}
	#endregion
	#region DocBookmarkFirstDeskriptor
	public class DocBookmarkFirstDescriptor {
		public static DocBookmarkFirstDescriptor FromStream(BinaryReader reader, int bookmarkIndexSize) {
			DocBookmarkFirstDescriptor result = new DocBookmarkFirstDescriptor();
			result.Read(reader, bookmarkIndexSize);
			return result;
		}
		#region Fields
		int bookmarkLimitDescriptorIndex;
		short firstColumnIndex;
		short limitColumnIndex;
		bool column;
		public DocBookmarkFirstDescriptor() { }
		public DocBookmarkFirstDescriptor(short bookmarkLimitDescriptorIndex) {
			this.bookmarkLimitDescriptorIndex = bookmarkLimitDescriptorIndex;
		}
		#endregion
		#region Properties
		public int BookmarkLimitDescriptorIndex { get { return bookmarkLimitDescriptorIndex; } }
		public short FirstColumnIndex { get { return firstColumnIndex; } }
		public short LimitColumnIndex { get { return limitColumnIndex; } }
		public bool Column { get { return column; } }
		#endregion
		protected void Read(BinaryReader reader, int bookmarkIndexSize) {
			this.bookmarkLimitDescriptorIndex = (bookmarkIndexSize == 2) ? reader.ReadInt16() : reader.ReadInt32();
			byte bitField = reader.ReadByte();
			this.firstColumnIndex = (short)(bitField & 0x7f);
			bitField = reader.ReadByte();
			this.limitColumnIndex = (short)(bitField & 0x7f);
			this.column = ((bitField & 0x80) == 1);
		}
		public void Write(BinaryWriter writer, int bookmarkIndexSize) {
			if (bookmarkIndexSize == 2)
				writer.Write((short)this.bookmarkLimitDescriptorIndex);
			else
				writer.Write(this.bookmarkLimitDescriptorIndex);
			byte bitField = (byte)(this.firstColumnIndex & 0x7f);
			writer.Write(bitField);
			bitField = (byte)(this.limitColumnIndex & 0x7f);
			if (this.column)
				bitField = (byte)(bitField & 0x80);
			writer.Write(bitField);
		}
	}
	#endregion
	#region DocBookmarkFirstTable
	public class DocBookmarkFirstTable {
		public static DocBookmarkFirstTable FromStream(BinaryReader reader, int offset, int size, int bookmarkIndexSize) {
			DocBookmarkFirstTable result = new DocBookmarkFirstTable();
			result.Read(reader, offset, size, bookmarkIndexSize);
			return result;
		}
		#region Fields
		const int positionSize = 4;
		const int bookmarkDescriptorSize = 2;
		const int bookmarkFirstSize = 4;
		List<int> characterPositions;
		List<DocBookmarkFirstDescriptor> bookmarkFirstDescriptors;
		#endregion
		public DocBookmarkFirstTable() {
			this.characterPositions = new List<int>();
			this.bookmarkFirstDescriptors = new List<DocBookmarkFirstDescriptor>();
		}
		#region Properties
		public List<int> CharacterPositions { get { return characterPositions; } }
		public List<DocBookmarkFirstDescriptor> BookmarkFirstDescriptors { get { return bookmarkFirstDescriptors; } }
		#endregion
		protected void Read(BinaryReader reader, int offset, int size, int bookmarkIndexSize) {
			Guard.ArgumentNotNull(reader, "reader");
			if (size <= 0 || offset < 0 || reader.BaseStream.Length - offset < size) return;
			reader.BaseStream.Seek(offset, SeekOrigin.Begin);
			int bookmarkSize = bookmarkIndexSize + bookmarkDescriptorSize;
			int count = (size - positionSize) / (positionSize + bookmarkSize);
			for (int i = 0; i < count + 1; i++)
				this.characterPositions.Add(reader.ReadInt32());
			for (int i = 0; i < count; i++)
				this.bookmarkFirstDescriptors.Add(DocBookmarkFirstDescriptor.FromStream(reader, bookmarkIndexSize));
		}
		public void Write(BinaryWriter writer, int bookmarkIndexSize) {
			Guard.ArgumentNotNull(writer, "writer");
			int count = this.characterPositions.Count;
			if (count <= 1)
				return;
			for (int i = 0; i < count; i++)
				writer.Write(this.characterPositions[i]);
			for (int i = 0; i < count - 1; i++)
				this.bookmarkFirstDescriptors[i].Write(writer, bookmarkIndexSize);
		}
		public void AddEntry(int characterPosition, DocBookmarkFirstDescriptor firstDescriptor) {
			this.characterPositions.Add(characterPosition);
			this.bookmarkFirstDescriptors.Add(firstDescriptor);
		}
		public void Finish(int lastCharacterPosition) {
			this.characterPositions.Add(lastCharacterPosition);
		}
	}
	#endregion
	#region DocBookmarkLimTable
	public class DocBookmarkLimTable {
		public static DocBookmarkLimTable FromStream(BinaryReader reader, int offset, int size) {
			DocBookmarkLimTable result = new DocBookmarkLimTable();
			result.Read(reader, offset, size);
			return result;
		}
		#region Fields
		const int positionSize = 4;
		List<int> characterPositions;
		#endregion
		public DocBookmarkLimTable() {
			this.characterPositions = new List<int>();
		}
		#region Properties
		public List<int> CharacterPositions { get { return characterPositions; } }
		#endregion
		protected void Read(BinaryReader reader, int offset, int size) {
			Guard.ArgumentNotNull(reader, "reader");
			if (size <= 0 || offset < 0 || reader.BaseStream.Length - offset < size) return;
			reader.BaseStream.Seek(offset, SeekOrigin.Begin);
			int count = size / positionSize;
			for (int i = 0; i < count; i++)
				this.characterPositions.Add(reader.ReadInt32());
		}
		public void Write(BinaryWriter writer) {
			Guard.ArgumentNotNull(writer, "writer");
			int count = this.characterPositions.Count;
			if (count <= 1)
				return;
			for (int i = 0; i < count; i++)
				writer.Write(this.characterPositions[i]);
		}
		public void Finish(int lastCharacterPosition) {
			this.characterPositions.Add(lastCharacterPosition);
		}
	}
	#endregion
	#region DocBookmarkIteratorBase (abstract class)
	public abstract class DocBookmarkIteratorBase {
		DocBookmarkFirstTable firstTable;
		DocBookmarkLimTable limTable;
		short currentIndex;
		PositionConverter converter;
		protected DocBookmarkIteratorBase() {
			firstTable = new DocBookmarkFirstTable();
			limTable = new DocBookmarkLimTable();
		}
		protected DocBookmarkIteratorBase(FileInformationBlock fib, BinaryReader reader) {
			Guard.ArgumentNotNull(fib, "fib");
			Guard.ArgumentNotNull(reader, "reader");
			Read(fib, reader);
		}
		protected PositionConverter PositionConverter { get { return converter; } }
		protected internal DocBookmarkFirstTable FirstTable { get { return firstTable; } set { firstTable = value; } }
		protected internal DocBookmarkLimTable LimTable { get { return limTable; } set { limTable = value; } }
		protected short CurrentIndex { get { return currentIndex; } set { currentIndex = value; } }
		protected virtual void InitConverter() {
			converter = new PositionConverter();
			converter.BeginInit();
			converter.AppendPositions(firstTable.CharacterPositions);
			converter.AppendPositions(limTable.CharacterPositions);
			converter.EndInit();
		}
		public void AdvanceNext(DocumentLogPosition logPosition, int originalPosition, int length) {
			converter.AdvanceNext(logPosition, originalPosition, length);
		}
		public virtual void Finish(int lastCharacterPosition) {
			firstTable.Finish(lastCharacterPosition);
			limTable.Finish(lastCharacterPosition);
		}
		protected abstract void Read(FileInformationBlock fib, BinaryReader reader);
		public abstract void Write(FileInformationBlock fib, BinaryWriter writer);
		public void BeginEmbeddedContent() {
			converter.BeginEmbeddedContent();
		}
		public void EndEmbeddedContent() {
			converter.EndEmbeddedContent();
		}
	}
	#endregion
	#region DocBookmarkIterator
	public class DocBookmarkIterator : DocBookmarkIteratorBase {
		#region Fields
		const int bookmarkIndexSize = 2;
		DocStringTable bookmarkNames;
		readonly List<DocBookmark> bookmarks = new List<DocBookmark>();
		Dictionary<string, Pair<int, int>> bookmarkDescriptions;
		#endregion
		public DocBookmarkIterator() {
			this.bookmarkDescriptions = new Dictionary<string, Pair<int, int>>();
			this.bookmarkNames = new DocStringTable();
		}
		public DocBookmarkIterator(FileInformationBlock fib, BinaryReader reader)
			: base(fib, reader) {
		}
		public void InsertBookmarks(PieceTable pieceTable) {
			int count = this.bookmarks.Count;
			List<DocBookmark> processedBookmarks = new List<DocBookmark>();
			DocumentLogPosition startPosition;
			DocumentLogPosition endPosition;
			for (int i = 0; i < count; i++) {
				bool startPositionObtainable = PositionConverter.TryConvert(this.bookmarks[i].OriginalStartPosition, out startPosition);
				bool endPositionObtainable = PositionConverter.TryConvert(this.bookmarks[i].OriginalEndPosition, out endPosition);
				if (startPositionObtainable && endPositionObtainable) {
					pieceTable.CreateBookmarkCore(startPosition, endPosition - startPosition, this.bookmarks[i].Name);
					processedBookmarks.Add(this.bookmarks[i]);
				}
			}
			RemoveProcessedBookmarks(processedBookmarks);
		}
		void RemoveProcessedBookmarks(List<DocBookmark> processedBookmarks) {
			int count = processedBookmarks.Count;
			for (int i = 0; i < count; i++) {
				this.bookmarks.Remove(processedBookmarks[i]);
			}
			PositionConverter.Clear();
		}
		protected override void Read(FileInformationBlock fib, BinaryReader reader) {
			this.bookmarkNames = DocStringTable.FromStream(reader, fib.BookmarkNamesTableOffset, fib.BookmarkNamesTableSize);
			FirstTable = DocBookmarkFirstTable.FromStream(reader, fib.BookmarkStartInfoOffset, fib.BookmarkStartInfoSize, bookmarkIndexSize);
			LimTable = DocBookmarkLimTable.FromStream(reader, fib.BookmarkEndInfoOffset, fib.BookmarkEndInfoSize);
			InitBookmarks();
			InitConverter();
		}
		public override void Write(FileInformationBlock fib, BinaryWriter writer) {
			Guard.ArgumentNotNull(writer, "writer");
			fib.BookmarkNamesTableOffset = (int)writer.BaseStream.Position;
			this.bookmarkNames.Write(writer);
			fib.BookmarkNamesTableSize = (int)(writer.BaseStream.Position - fib.BookmarkNamesTableOffset);
			fib.BookmarkStartInfoOffset = (int)writer.BaseStream.Position;
			FirstTable.Write(writer, bookmarkIndexSize);
			fib.BookmarkStartInfoSize = (int)(writer.BaseStream.Position - fib.BookmarkStartInfoOffset);
			fib.BookmarkEndInfoOffset = (int)writer.BaseStream.Position;
			LimTable.Write(writer);
			fib.BookmarkEndInfoSize = (int)(writer.BaseStream.Position - fib.BookmarkEndInfoOffset);
		}
		void CollectBookmarks() {
			List<int> endPositions = new List<int>();
			Dictionary<int, int> endPositionIndicies = new Dictionary<int, int>();
			foreach (var item in bookmarkDescriptions) {
				int end = item.Value.Second;
				if (end != -1) {
					this.bookmarks.Add(new DocBookmark(item.Value.First, end, item.Key));
					endPositions.Add(end);
				}
			}
			Comparison<DocBookmark> comparison = delegate(DocBookmark first, DocBookmark second) { return first.OriginalStartPosition - second.OriginalStartPosition; };
			this.bookmarks.Sort(comparison);
			endPositions.Sort();
			int count = this.bookmarks.Count;
			for (int i = 0; i < count; i++) {
				DocBookmark bookmark = this.bookmarks[i];
				this.bookmarkNames.Data.Add(bookmark.Name);
				int endPositionIndex = endPositions.IndexOf(bookmark.OriginalEndPosition);
#if DEBUGTEST
				Debug.Assert(endPositionIndex >= 0);
#endif
				int offset;
				if (!endPositionIndicies.TryGetValue(endPositionIndex, out offset)) {
					endPositionIndicies[endPositionIndex] = 0;
					offset = 0;
				}
				else {
					offset++;
					endPositionIndicies[endPositionIndex] = offset;
				}
				FirstTable.AddEntry(bookmark.OriginalStartPosition, new DocBookmarkFirstDescriptor((short)(endPositionIndex + offset)));
				CurrentIndex++;
			}
			LimTable.CharacterPositions.AddRange(endPositions);
		}
		void InitBookmarks() {
			int count = this.bookmarkNames.Data.Count;
#if DEBUGTEST || DEBUG
			if (count > 0) {
				Debug.Assert(FirstTable.CharacterPositions.Count == count + 1);
				Debug.Assert(LimTable.CharacterPositions.Count == count + 1);
			}
#endif
			for (int i = 0; i < count; i++) {
				if (FirstTable.BookmarkFirstDescriptors[i].Column)
					continue;
				string bookmarkName = this.bookmarkNames.Data[i];
				int bookmarkStartPosition = FirstTable.CharacterPositions[i];
				int endPositionIndex = FirstTable.BookmarkFirstDescriptors[i].BookmarkLimitDescriptorIndex;
				if (endPositionIndex >= count)
					continue;
				int bookmarkEndPosition = LimTable.CharacterPositions[endPositionIndex];
				this.bookmarks.Add(new DocBookmark(bookmarkStartPosition, bookmarkEndPosition, bookmarkName));
			}
		}
		public void AddBookmarkStart(string name, int startPosition) {
			if (!this.bookmarkDescriptions.ContainsKey(name))
				this.bookmarkDescriptions.Add(name, new Pair<int, int>(startPosition, -1));
		}
		public void AddBookmarkEnd(string name, int endPosition) {
			Pair<int, int> interval;
			if (!this.bookmarkDescriptions.TryGetValue(name, out interval))
				return;
			interval.Second = endPosition;
		}
		public override void Finish(int lastCharacterPosition) {
			CollectBookmarks();
			base.Finish(lastCharacterPosition);
		}
	}
	#endregion
}
