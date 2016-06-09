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
using System.Text;
using System.IO;
using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Import.Doc {
	#region DocCommentsAuthorTable
	public class DocCommentsAuthorTable : DocStringTableBase {
		public static DocCommentsAuthorTable FromStream(BinaryReader reader, int offset, int size) {
			DocCommentsAuthorTable result = new DocCommentsAuthorTable();
			result.Read(reader, offset, size);
			return result;
		}
		readonly List<string> data; 
		public DocCommentsAuthorTable() {
			this.data = new List<string>();
		}
		public List<string> Data { get { return data; } }
		protected internal override void Read(BinaryReader reader, int offset, int size) {
			Guard.ArgumentNotNull(reader, "reader");
			if (size == 0) return;
			reader.BaseStream.Seek(offset, SeekOrigin.Begin);
			IsExtended = CalcIsExtended(reader);
			Encoding = GetEncoding();
			while (reader.BaseStream.Position < offset + size)
				ReadString(reader);
		}
		protected override bool CalcIsExtended(BinaryReader reader) {
			ushort typecode = reader.ReadUInt16();
			Debug.Assert(typecode != ExtendedTypeCode);
			reader.BaseStream.Seek(-2, SeekOrigin.Current);
			return false;
		}
		protected override void ReadString(BinaryReader reader) {
			int length = reader.ReadByte();
			reader.ReadByte();
			byte[] buffer = reader.ReadBytes(length * 2);
			string result = Encoding.GetString(buffer, 0, buffer.Length);
			Data.Add(StringHelper.RemoveSpecialSymbols(result));
		}
		protected internal override void Write(BinaryWriter writer) {
			Encoding = GetEncoding();
			Count = Data.Count;
			WriteCore(writer);
		}
		protected override void WriteCore(BinaryWriter writer) {
			for (int i = 0; i < Count; i++)
				WriteString(writer, i);
		}
		protected override void WriteString(BinaryWriter writer, int index) {
			string stringData = Data[index];
			string validateStringData = stringData.Substring(0, Math.Min(56, stringData.Length));
			writer.Write((short)validateStringData.Length);
			writer.Write(Encoding.GetBytes(validateStringData));
		}
	}
	#endregion
	#region DocCommentsNameTable
	public class DocCommentsNameTable : DocStringTableBase {
		public static DocCommentsNameTable FromStream(BinaryReader reader, int offset, int size) {
			DocCommentsNameTable result = new DocCommentsNameTable();
			result.Read(reader, offset, size);
			return result;
		}
		#region Fields
		const int tagOld = -1;
		const short extraDataSize = 0x000a;
		const short classAnnotationBookmark = 0x0100;
		const short maxNumberAnnotationBookmarks = 0x3ffb;
		readonly List<string> extraData;
		#endregion
		public DocCommentsNameTable() {
			this.extraData = new List<string>();
		}
		public List<string> ExtraData { get { return extraData; } }
		protected internal override int CalcRecordsCount(BinaryReader reader) {
			return Math.Min(maxNumberAnnotationBookmarks, reader.ReadInt16());
		}
		protected internal override int CalcExtraDataSize(BinaryReader reader) {
			int result = reader.ReadInt16();
			Debug.Assert(result == extraDataSize);
			return result;
		}
		protected internal override void ReadCore(BinaryReader reader) {
			for (int i = 0; i < Count; i++)
				ReadExtraData(reader);
		}
		protected override void ReadExtraData(BinaryReader reader) {
			reader.BaseStream.Seek(4, SeekOrigin.Current);
			ExtraData.Add(reader.ReadInt32().ToString());
			reader.BaseStream.Seek(4, SeekOrigin.Current);
		}
		protected override void WriteCore(BinaryWriter writer) {
			Count = Math.Min(maxNumberAnnotationBookmarks, (short)ExtraData.Count);
			for (int i = 0; i < Count; i++) {
				writer.Write((short)0);
				WriteExtraData(writer, i);
			}
		}
		protected override void WriteCount(BinaryWriter writer) {
			writer.Write(Math.Min(maxNumberAnnotationBookmarks, (short)ExtraData.Count));
		}
		protected override void WriteExtraDataSize(BinaryWriter writer) {
			writer.Write(extraDataSize);
		}
		protected override void WriteExtraData(BinaryWriter writer, int index) {
			writer.Write(classAnnotationBookmark);
			int name;
			string data = ExtraData[index];
			if (Int32.TryParse(data, out name))
				writer.Write(name);
			else
				writer.Write(data.GetHashCode());
			writer.Write(tagOld);
		}
	}
	#endregion
	#region DocCommentsFirstTable
	public class DocCommentsFirstTable {
		public static DocCommentsFirstTable FromStream(BinaryReader reader, int offset, int size) {
			DocCommentsFirstTable result = new DocCommentsFirstTable();
			result.Read(reader, offset, size);
			return result;
		}
		#region Fields
		readonly List<int> characterPositions;
		readonly List<int> indexCharacterPositions;
		#endregion
		public DocCommentsFirstTable() {
			this.characterPositions = new List<int>();
			this.indexCharacterPositions = new List<int>();
		}
		#region Properties
		public List<int> CharacterPositions { get { return characterPositions; } }
		public List<int> IndexCharacterPositions { get { return indexCharacterPositions; } }
		#endregion
		protected void Read(BinaryReader reader, int offset, int size) {
			Guard.ArgumentNotNull(reader, "reader");
			if (size == 0) return;
			reader.BaseStream.Seek(offset, SeekOrigin.Begin);
			int commentsPositionSize = DocConstants.CharacterPositionSize;
			int count = (size - commentsPositionSize) / commentsPositionSize;
			int countOriginalStartPositions = count / 2;
			for (int i = 0; i < countOriginalStartPositions; i++)
				CharacterPositions.Add(reader.ReadInt32());
			reader.ReadInt32();
			for (int i = countOriginalStartPositions; i < count; i++)
				IndexCharacterPositions.Add(reader.ReadInt32());
			SortPosition();
		}
		void SortPosition() {
			int count = IndexCharacterPositions.Count;
			for (int i = 0; i < count - 1; i++)
				for (int j = 0; j < count - 1; j++)
					if (IndexCharacterPositions[j] > IndexCharacterPositions[j + 1]) {
						int index = IndexCharacterPositions[j];
						IndexCharacterPositions[j] = IndexCharacterPositions[j + 1];
						IndexCharacterPositions[j + 1] = index;
						int position = CharacterPositions[j];
						CharacterPositions[j] = CharacterPositions[j + 1];
						CharacterPositions[j + 1] = position;
					}
		}
		public void Write(BinaryWriter writer) {
			Guard.ArgumentNotNull(writer, "writer");
			int count = CharacterPositions.Count;
			for (int i = 0; i < count; i++)
				writer.Write(CharacterPositions[i]);
			for (int i = 0; i < count - 1; i++) {
				writer.Write(IndexCharacterPositions[i]);
			}
		}
		public void Finish(int lastCharacterPosition) {
			CharacterPositions.Add(lastCharacterPosition);
		}
	}
	#endregion
	#region DocCommentsLimTable
	public class DocCommentsLimTable {
		public static DocCommentsLimTable FromStream(BinaryReader reader, int offset, int size) {
			DocCommentsLimTable result = new DocCommentsLimTable();
			result.Read(reader, offset, size);
			return result;
		}
		readonly List<int> characterPositions;
		public DocCommentsLimTable() {
			this.characterPositions = new List<int>();
		}
		public List<int> CharacterPositions { get { return characterPositions; } }
		protected void Read(BinaryReader reader, int offset, int size) {
			Guard.ArgumentNotNull(reader, "reader");
			reader.BaseStream.Seek(offset, SeekOrigin.Begin);
			int commentsPositionSize = DocConstants.CharacterPositionSize;
			int count = (size - commentsPositionSize) / commentsPositionSize;
			for (int i = 0; i < count; i++)
				CharacterPositions.Add(reader.ReadInt32());
		}
		public void Write(BinaryWriter writer) {
			Guard.ArgumentNotNull(writer, "writer");
			int count = CharacterPositions.Count;
			for (int i = 0; i < count; i++)
				writer.Write(CharacterPositions[i]);
		}
		public void Finish(int lastCharacterPosition) {
			CharacterPositions.Add(lastCharacterPosition);
		}
	}
	#endregion
	#region DocCommentsInfoTable
	public class DocCommentsInfoTable {
		public static DocCommentsInfoTable FromStream(BinaryReader reader, int offset, int size) {
			DocCommentsInfoTable result = new DocCommentsInfoTable();
			result.Read(reader, offset, size);
			return result;
		}
		#region Fields
		const int referenceLimSize = 4;
		const int structureATRDPre10Size = 30;
		const int initialsSize = 20;
		const string prefixNewCommentName = "dxcomment";
		readonly List<int> referenceLimTable;
		readonly List<string> initialsTable;
		readonly List<int> authorIndexTable;
		readonly List<string> referenceNameTable;
		readonly List<int> dateTimeDTTMTable;
		readonly List<int> parentTable;
		readonly List<int> offsetParentTable;
		#endregion
		public DocCommentsInfoTable() {
			this.referenceLimTable = new List<int>();
			this.initialsTable = new List<string>();
			this.authorIndexTable = new List<int>();
			this.referenceNameTable = new List<string>();
			this.dateTimeDTTMTable = new List<int>();
			this.parentTable = new List<int>();
			this.offsetParentTable = new List<int>();
		}
		#region Properties
		public List<int> ReferenceLimTable { get { return referenceLimTable; } }
		public List<string> InitialsTable { get { return initialsTable; } }
		public List<int> AuthorIndexTable { get { return authorIndexTable; } }
		public List<string> ReferenceNameTable { get { return referenceNameTable; } }
		public List<int> DateTimeDTTMTable { get { return dateTimeDTTMTable; } }
		public List<int> ParentTable { get { return parentTable; } }
		public List<int> OffsetParentTable { get { return offsetParentTable; } }
		#endregion
		protected void Read(BinaryReader reader, int offset, int size) {
			Guard.ArgumentNotNull(reader, "reader");
			reader.BaseStream.Seek(offset, SeekOrigin.Begin);
			int count = (size - referenceLimSize) / (referenceLimSize + structureATRDPre10Size);
			for (int i = 0; i < count; i++)
				ReferenceLimTable.Add(reader.ReadInt32());
			reader.ReadInt32();
			for (int i = 0; i < count; i++) {
				ReadInitials(reader);
				ReadAuthorNumbers(reader);
				reader.ReadInt32();
				ReadReferenceName(reader, i);
			}
			for (int i = 0; i < count; i++) {
				ReadDateTime(reader);
				reader.ReadInt16();
				ReadIdParent(reader);
				ReadOffsetParent(reader);
				reader.ReadInt32();
			}
		}
		void ReadInitials(BinaryReader reader) {
			int length = reader.ReadByte();
			reader.ReadByte();
			byte[] buffer = reader.ReadBytes(initialsSize - 2);
			string result = Encoding.Unicode.GetString(buffer, 0, buffer.Length).Substring(0, length);
			InitialsTable.Add(StringHelper.RemoveSpecialSymbols(result));
		}
		void ReadAuthorNumbers(BinaryReader reader) {
			AuthorIndexTable.Add(reader.ReadInt16());
		}
		void ReadReferenceName(BinaryReader reader, int index) {
			int referenceName = reader.ReadInt32();
			if (referenceName == -1)
				ReferenceNameTable.Add(GetNewCommentName(index));
			else
				ReferenceNameTable.Add(referenceName.ToString());
		}
		string GetNewCommentName(int index) {
			return prefixNewCommentName + index.ToString();
		}
		void ReadDateTime(BinaryReader reader) {
			DateTimeDTTMTable.Add(reader.ReadInt32());
		}
		void ReadIdParent(BinaryReader reader) {
			ParentTable.Add(reader.ReadInt32());
		}
		void ReadOffsetParent(BinaryReader reader) {
			OffsetParentTable.Add(reader.ReadInt32());
		}
		public void Write(BinaryWriter writer) {
			Guard.ArgumentNotNull(writer, "writer");
			int count = ReferenceLimTable.Count;
			for (int i = 0; i < count; i++)
				writer.Write(ReferenceLimTable[i]);
			for (int i = 0; i < count - 1; i++) {
				WriteInitials(writer, i);
				WriteAuthorNumbers(writer, i);
				writer.Write(0);
				WriteReferenceName(writer, i);
			}
			for (int i = 0; i < count - 1; i++) {
				WriteDateTime(writer, i);
				writer.Write((short)0);
				WriteIdParent(writer, i);
				WriteOffsetParent(writer, i);
				writer.Write(0);
			}
		}
		void WriteInitials(BinaryWriter writer, int index) {
			string stringInitials = InitialsTable[index];
			string validateStringInitials = stringInitials.Substring(0, Math.Min(initialsSize - 2, stringInitials.Length));
			byte[] byteInitials = Encoding.Unicode.GetBytes(validateStringInitials);
			byte[] buffer = new byte[initialsSize];
			buffer[0] = (byte)validateStringInitials.Length;
			for (int i = 0; i < byteInitials.Length; i++)
				buffer[2 + i] = byteInitials[i];
			writer.Write(buffer);
		}
		void WriteAuthorNumbers(BinaryWriter writer, int index) {
			writer.Write((short)AuthorIndexTable[index]);
		}
		void WriteReferenceName(BinaryWriter writer, int index) {
			int name;
			string reference = ReferenceNameTable[index];
			if (Int32.TryParse(reference, out name))
				writer.Write(name);
			else
				writer.Write(reference.GetHashCode());
		}
		void WriteDateTime(BinaryWriter writer, int index) {
			writer.Write(DateTimeDTTMTable[index]);
		}
		void WriteIdParent(BinaryWriter writer, int index) {
			writer.Write(ParentTable[index]);
		}
		void WriteOffsetParent(BinaryWriter writer, int index) {
			writer.Write(OffsetParentTable[index]);
		}
		public void Finish(int lastCharacterPosition) {
			ReferenceLimTable.Add(lastCharacterPosition);
		}
	}
	#endregion
	#region DocCommentsIterator
	public class DocCommentsIterator {
		#region Fields
		const string prefixNewCommentName = "dxcomment";
		readonly Dictionary<int, DocObjectCollection> commentsContent;
		List<int> commentsReferences;
		DocCommentsAuthorTable commentsAuthorTable;
		DocCommentsNameTable commentsNameTable;
		DocCommentsFirstTable commentsFirstTable;
		DocCommentsLimTable commentsLimTable;
		DocCommentsInfoTable commentsInfoTable;
		PositionConverter converter;
		Dictionary<int, Comment> commentsIndex;
		#endregion
		public DocCommentsIterator(FileInformationBlock fib, BinaryReader reader) {
			this.commentsContent = new Dictionary<int, DocObjectCollection>();
			this.converter = new PositionConverter();
			Read(fib, reader);
			this.commentsReferences = GetCommentsReferences();
			this.commentsIndex = new Dictionary<int,Comment>(); 
		}
		#region Properties
		public Dictionary<int, DocObjectCollection> CommentsContent { get { return commentsContent; } }
		protected internal List<int> CommentsReferences { get { return commentsReferences; } }
		protected internal PositionConverter Converter { get { return converter; } }
		protected internal Dictionary<int, Comment> CommentsIndex { get { return commentsIndex; } }
		#endregion
		void Read(FileInformationBlock fib, BinaryReader reader) {
			Guard.ArgumentNotNull(reader, "reader");
			commentsAuthorTable = DocCommentsAuthorTable.FromStream(reader, fib.CommentsAuthorTableOffset, fib.CommentsAuthorTableSize);
			commentsNameTable = DocCommentsNameTable.FromStream(reader, fib.CommentsNameTableOffset, fib.CommentsNameTableSize);
			commentsFirstTable = DocCommentsFirstTable.FromStream(reader, fib.CommentsFirstTableOffset, fib.CommentsFirstTableSize);
			commentsLimTable = DocCommentsLimTable.FromStream(reader, fib.CommentsLimTableOffset, fib.CommentsLimTableSize);
			commentsInfoTable = DocCommentsInfoTable.FromStream(reader, fib.CommentsReferenceOffset, fib.CommentsReferenceSize);
			UpdateTablesForZeroLenghtComments();
			InitConverter();
		}
		void UpdateTablesForZeroLenghtComments() {
			List<string> referenceNameTable = commentsInfoTable.ReferenceNameTable;
			int count = referenceNameTable.Count;
			for (int i = 0; i < count; i++)
				if (referenceNameTable[i].Contains(prefixNewCommentName)) {
					int position = commentsInfoTable.ReferenceLimTable[i];
					commentsFirstTable.CharacterPositions.Insert(i, position);
					commentsLimTable.CharacterPositions.Insert(i, position);
				}
		}
		void InitConverter() {
			Converter.BeginInit();
			Converter.AppendPositions(commentsFirstTable.CharacterPositions);
			Converter.AppendPositions(commentsLimTable.CharacterPositions);
			Converter.EndInit();
		}
		public void AdvanceNext(DocumentLogPosition logPosition, int originalPosition, int length) {
			Converter.AdvanceNext(logPosition, originalPosition, length);
		}
		public void InsertComment(PieceTable pieceTable, CommentContentType commentContent, int characterPosition) {
			int index = GetIndexCharacterPosition(characterPosition);
			if (index < 0) 
				return;
			int originalStartPosition = commentsFirstTable.CharacterPositions[index];
			if (originalStartPosition < 0) 
				return;
			int originalEndPosition = commentsLimTable.CharacterPositions[index];
			if (originalEndPosition < 0 || originalEndPosition < originalStartPosition)
				return;
			DocumentLogPosition startPosition;
			DocumentLogPosition endPosition;
			bool startPositionObtainable = Converter.TryConvert(originalStartPosition, out startPosition);
			bool endPositionObtainable = Converter.TryConvert(originalEndPosition, out endPosition);
			if (!startPositionObtainable || !endPositionObtainable || startPosition > endPosition)
				return;
			int lenght = endPosition - startPosition;
			string name = commentsInfoTable.ReferenceNameTable[index];
			string author = commentsAuthorTable.Data[commentsInfoTable.AuthorIndexTable[index]];
			string initials = commentsInfoTable.InitialsTable[index];
			DateTime dateTime = Comment.MinCommentDate;
			int commentDateTimeDTTM = commentsInfoTable.DateTimeDTTMTable[index];
			if (commentDateTimeDTTM != 0)
				dateTime = DateTimeUtils.FromDateTimeDTTM(commentDateTimeDTTM);
			Comment parentComment = FindParentComment(index);
			Comment currentComment = pieceTable.CreateCommentCore(startPosition, lenght,  initials, author, dateTime, parentComment, commentContent);
			CommentsIndex.Add(index, currentComment);
		}
		Comment FindParentComment(int index) {
			int parentId = commentsInfoTable.ParentTable[index];
			int offsetParent = commentsInfoTable.OffsetParentTable[index];
			int parentIndex = index + offsetParent;
			Comment parentComment;
			if (parentId > 0 && CommentsIndex.TryGetValue(parentIndex, out parentComment))
				return parentComment;
			return null;
		}
		int GetIndexCharacterPosition(int characterPosition) {
			for (int i = 0; i < CommentsReferences.Count; i++)
				if (CommentsReferences[i] == characterPosition)
					return i;
			return -1;
		}
		List<int> GetCommentsReferences() {
			List<int> result = commentsInfoTable.ReferenceLimTable;
			int count = result.Count;
			if (count > 0) {
				Debug.Assert(commentsFirstTable.CharacterPositions.Count == count);
				Debug.Assert(commentsLimTable.CharacterPositions.Count == count);
				Debug.Assert(commentsInfoTable.ReferenceNameTable.Count == count);
				Debug.Assert(commentsInfoTable.AuthorIndexTable.Count == count);
				Debug.Assert(commentsInfoTable.InitialsTable.Count == count);
				Debug.Assert(commentsInfoTable.DateTimeDTTMTable.Count == count);
				Debug.Assert(commentsInfoTable.ParentTable.Count == count);
			}
			return result;
		}
		public DocObjectCollection GetCommentsContent(int characterPosition) {
			DocObjectCollection result;
			if (!CommentsContent.TryGetValue(characterPosition, out result))
				result = new DocObjectCollection();
			return result;
		}
	}
	#endregion
	#region DocCommentsExporter
	public class DocCommentsExporter {
		#region Fields
		const int structureATRDPost10Size = 18; 
		const string nameZeroLenghtComment = "-1";
		readonly List<int> commentsContentPositions;
		readonly List<int> commentsFirst;
		readonly List<int> commentsLim;
		readonly List<string> commentsName;
		readonly List<string> commentsReferenceName;
		readonly DocCommentsNameTable commentsNameTable;
		readonly DocCommentsAuthorTable commentsAuthorTable;
		readonly DocCommentsFirstTable commentsFirstTable;
		readonly DocCommentsLimTable commentsLimTable;
		readonly DocCommentsInfoTable commentsInfoTable;
		#endregion
		public DocCommentsExporter() {
			commentsContentPositions = new List<int>();
			commentsFirst = new List<int>();
			commentsLim = new List<int>();
			commentsName = new List<string>();
			commentsReferenceName = new List<string>();
			commentsNameTable = new DocCommentsNameTable();
			commentsAuthorTable = new DocCommentsAuthorTable();
			commentsFirstTable = new DocCommentsFirstTable();
			commentsLimTable = new DocCommentsLimTable();
			commentsInfoTable = new DocCommentsInfoTable();
		}
		public List<int> CommentsContentPositions { get { return commentsContentPositions; } }
		public void AddCommentStart(string name, int firstPosition) {
			commentsFirst.Add(firstPosition);
			AddUniqueName(name, commentsName);
		}
		public void AddCommentEnd(Comment comment, int endPosition) {
			commentsLim.Add(endPosition);
			commentsInfoTable.ReferenceLimTable.Add(endPosition);
			AddUniqueName( Convert.ToString(comment.Index), commentsReferenceName);
			AddCommentsAuthor(comment.Author);
			commentsInfoTable.InitialsTable.Add(comment.Name);
			AddCommentDateTime(comment.Date);
			AddCommentParent(comment.ParentComment);
			AddOffsetParent(comment);
		}
		void AddUniqueName(string defaultName, List<string> names) {
			while (!names.Contains(defaultName))
				names.Add(defaultName);
		}
		void AddCommentsAuthor(string author) {
			List<string> commentsAuthor = commentsAuthorTable.Data;
			if (!commentsAuthor.Contains(author))
				commentsAuthor.Add(author);
			commentsInfoTable.AuthorIndexTable.Add(GetIndex(commentsAuthor, author));	
		}
		void AddCommentDateTime(DateTime dateTime) {
			commentsInfoTable.DateTimeDTTMTable.Add(DateTimeUtils.ToDateTimeDTTM(dateTime));
		}
		void AddCommentParent(Comment comment) {
			if(comment == null)
				commentsInfoTable.ParentTable.Add(0);
			else
				commentsInfoTable.ParentTable.Add(1);
		}
		void AddOffsetParent(Comment comment) {
			if (comment.ParentComment == null)
				commentsInfoTable.OffsetParentTable.Add(0);
			else {
				commentsInfoTable.OffsetParentTable.Add(comment.ParentComment.Index - comment.Index);
			}
		}
		int GetIndex(List<string> collection, string item) {
			for (int i = 0; i < collection.Count; i++)
				if (collection[i] == item)
					return i;
			return -1;
		}
		public void FinishCommentPositions(int commentsContentLength) {
			CommentsContentPositions.Add(commentsContentLength - 1);
			CommentsContentPositions.Add(commentsContentLength);
		}
		public void Finish(int lastCharacterPosition) {
			CalcExportTables();
			commentsFirstTable.Finish(lastCharacterPosition);
			commentsLimTable.Finish(lastCharacterPosition);
			commentsInfoTable.Finish(lastCharacterPosition);
		}
		void CalcExportTables() {
			CalcReferenceNameTable();
			CalcLimTable();
			CalcNameAndFirstTable();
		}
		void CalcReferenceNameTable() {
			List<int> indexName = GetCollectionIndexReference(commentsReferenceName, commentsName);
			for (int i = 0; i < commentsReferenceName.Count; i++)
				if (commentsFirst[indexName[i]] == commentsLim[i])
					commentsInfoTable.ReferenceNameTable.Add(nameZeroLenghtComment);
				else
					commentsInfoTable.ReferenceNameTable.Add(commentsName[indexName[i]]);
		}
		void CalcLimTable() {
			for (int i = 0; i < commentsInfoTable.ReferenceNameTable.Count; i++)
				if (commentsInfoTable.ReferenceNameTable[i] != nameZeroLenghtComment)
					commentsLimTable.CharacterPositions.Add(commentsLim[i]);
		}
		void CalcNameAndFirstTable() {
			List<int> indexReference = GetCollectionIndexReference(commentsName, commentsReferenceName);
			Dictionary<int, int> processedIndexFirst = new Dictionary<int, int>();
			int index = 0;
			for (int i = 0; i < commentsName.Count; i++) {
				string name = commentsInfoTable.ReferenceNameTable[indexReference[i]];
				if (name != nameZeroLenghtComment) {
					commentsNameTable.ExtraData.Add(name);
					commentsFirstTable.CharacterPositions.Add(commentsFirst[i]);
					processedIndexFirst.Add(index, indexReference[i]);
					index++;
				}
			}
			int count = processedIndexFirst.Count;
			Dictionary<int, int> commentsIndexFirst = new Dictionary<int, int>();
			for (int i = 0; i < count; i++) {
				index = GetKeyForMinValue(processedIndexFirst);
				commentsIndexFirst.Add(index, i);
				processedIndexFirst.Remove(index);
			}
			for (int i = 0; i < commentsIndexFirst.Count; i++)
				commentsFirstTable.IndexCharacterPositions.Add(commentsIndexFirst[i]);
		}
		int GetKeyForMinValue(Dictionary<int, int> dictionary) {
			int minValue = Int32.MaxValue;
			int result = -1;
			int value;
			foreach (int key in dictionary.Keys)
				if (dictionary.TryGetValue(key, out value) && value < minValue) {
					result = key;
					minValue = value;
				}
			return result;
		}
		List<int> GetCollectionIndexReference(List<string> collection, List<string> referenceCollection) {
			List<int> collectionIndex = new List<int>();
			for (int i = 0; i < collection.Count; i++) {
				int index = GetIndex(referenceCollection, collection[i]);
				if (index >= 0)
					collectionIndex.Add(index);
			}
			return collectionIndex;
		}
		public void ExportCommentsTables(FileInformationBlock fib, BinaryWriter writer) {
			Guard.ArgumentNotNull(writer, "writer");
			fib.CommentsAuthorTableOffset = (int)writer.BaseStream.Position;
			commentsAuthorTable.Write(writer);
			fib.CommentsAuthorTableSize = (int)(writer.BaseStream.Position - fib.CommentsAuthorTableOffset);
			fib.CommentsNameTableOffset = (int)writer.BaseStream.Position;
			commentsNameTable.Write(writer);
			fib.CommentsNameTableSize = (int)(writer.BaseStream.Position - fib.CommentsNameTableOffset);
			fib.CommentsFirstTableOffset = (int)writer.BaseStream.Position;
			commentsFirstTable.Write(writer);
			fib.CommentsFirstTableSize = (int)(writer.BaseStream.Position - fib.CommentsFirstTableOffset);
			fib.CommentsLimTableOffset = (int)writer.BaseStream.Position;
			commentsLimTable.Write(writer);
			fib.CommentsLimTableSize = (int)(writer.BaseStream.Position - fib.CommentsLimTableOffset);
			fib.CommentsReferenceOffset = (int)writer.BaseStream.Position;
			commentsInfoTable.Write(writer);
			fib.CommentsReferenceSize = (int)(writer.BaseStream.Position - fib.CommentsReferenceOffset - structureATRDPost10Size * commentsInfoTable.DateTimeDTTMTable.Count);
			fib.CommentsTextOffset = (int)writer.BaseStream.Position;
			int count = CommentsContentPositions.Count;
			for (int i = 0; i < count; i++)
				writer.Write(CommentsContentPositions[i]);
			fib.CommentsTextSize = (int)(writer.BaseStream.Position - fib.CommentsTextOffset);
		}
	}
	#endregion
}
