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
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office;
using System.Collections.Generic;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
namespace DevExpress.XtraRichEdit.Model {
	#region DocumentModelPosition
	public class DocumentModelPosition : ICloneable<DocumentModelPosition>, ISupportsCopyFrom<DocumentModelPosition>, IComparable<DocumentModelPosition> {
		#region Fields
		static readonly DocumentModelPosition maxValue = new MaxDocumentPosition2();
		readonly PieceTable pieceTable;
		DocumentLogPosition logPosition;
		ParagraphIndex paragraphIndex;
		RunIndex runIndex;
		DocumentLogPosition runStartLogPosition;
		#endregion
		protected DocumentModelPosition() {
		}
		public DocumentModelPosition(PieceTable pieceTable) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
		}
		#region Properties
		public PieceTable PieceTable { get { return pieceTable; } }
		public DocumentLogPosition LogPosition { get { return logPosition; } set { logPosition = value; } }
		public ParagraphIndex ParagraphIndex { get { return paragraphIndex; } set { paragraphIndex = value; } }
		public RunIndex RunIndex { get { return runIndex; } set { runIndex = value; } }
		public DocumentLogPosition RunStartLogPosition { get { return runStartLogPosition; } set { runStartLogPosition = value; } }
		public virtual DocumentLogPosition RunEndLogPosition { get { return RunStartLogPosition + PieceTable.Runs[RunIndex].Length - 1; } }
		public int RunOffset { get { return LogPosition - RunStartLogPosition; } }
		public static DocumentModelPosition MaxValue { get { return maxValue; } }
		#endregion
		public override bool Equals(object obj) {
			DocumentModelPosition pos = obj as DocumentModelPosition;
			if (!Object.ReferenceEquals(pos, null))
				return this.LogPosition == pos.LogPosition;
			else
				return base.Equals(obj);
		}
		public override int GetHashCode() {
			return LogPosition.GetHashCode();
		}
		public virtual void Update() {
			ParagraphIndex = PieceTable.FindParagraphIndex(LogPosition);
			RunIndex runIndex;
			RunStartLogPosition = PieceTable.FindRunStartLogPosition(PieceTable.Paragraphs[ParagraphIndex], LogPosition, out runIndex);
			RunIndex = runIndex;
		}
		public static bool operator <(DocumentModelPosition pos1, DocumentModelPosition pos2) {
			return pos1.LogPosition < pos2.LogPosition;
		}
		public static bool operator <=(DocumentModelPosition pos1, DocumentModelPosition pos2) {
			return pos1.LogPosition <= pos2.LogPosition;
		}
		public static bool operator ==(DocumentModelPosition pos1, DocumentModelPosition pos2) {
			if (Object.ReferenceEquals(pos1, pos2))
				return true;
			if (Object.ReferenceEquals(pos1, null))
				return false;
			return pos1.Equals(pos2);
		}
		public static bool operator >(DocumentModelPosition pos1, DocumentModelPosition pos2) {
			return !(pos1 <= pos2);
		}
		public static bool operator >=(DocumentModelPosition pos1, DocumentModelPosition pos2) {
			return !(pos1 < pos2);
		}
		public static bool operator !=(DocumentModelPosition pos1, DocumentModelPosition pos2) {
			return !(pos1 == pos2);
		}
		public static DocumentModelPosition FromParagraphStart(PieceTable pieceTable, ParagraphIndex paragraphIndex) {
			DocumentModelPosition currentPosition = new DocumentModelPosition(pieceTable);
			SetParagraphStart(currentPosition, paragraphIndex);
			return currentPosition;
		}
		public static DocumentModelPosition FromRunStart(PieceTable pieceTable, RunIndex runIndex) {
			DocumentModelPosition currentPosition = new DocumentModelPosition(pieceTable);
			SetRunStart(currentPosition, runIndex);
			return currentPosition;
		}
		public static DocumentModelPosition FromRunEnd(PieceTable pieceTable, RunIndex runIndex) {
			DocumentModelPosition currentPosition = new DocumentModelPosition(pieceTable);
			SetRunEnd(currentPosition, runIndex);
			return currentPosition;
		}
		public static DocumentModelPosition FromDocumentEnd(PieceTable pieceTable) {
			DocumentModelPosition pos = new DocumentModelPosition(pieceTable);
			pos.LogPosition = pieceTable.DocumentEndLogPosition;
			pos.ParagraphIndex = new ParagraphIndex(pieceTable.Paragraphs.Count - 1);
			pos.RunIndex = new RunIndex(pieceTable.Runs.Count - 1);
			pos.RunStartLogPosition = pieceTable.DocumentEndLogPosition;
			return pos;
		}
		public static void SetParagraphStart(DocumentModelPosition pos, ParagraphIndex paragraphIndex) {
			PieceTable pieceTable = pos.PieceTable;
			Paragraph paragraph = pieceTable.Paragraphs[paragraphIndex];
			pos.LogPosition = paragraph.LogPosition;
			pos.ParagraphIndex = paragraphIndex;
			pos.RunIndex = paragraph.FirstRunIndex;
			pos.RunStartLogPosition = pos.LogPosition;
		}
		public static DocumentModelPosition FromParagraphEnd(PieceTable pieceTable, ParagraphIndex paragraphIndex) {
			DocumentModelPosition currentPosition = new DocumentModelPosition(pieceTable);
			SetParagraphEnd(currentPosition, paragraphIndex);
			return currentPosition;
		}
		public static void SetParagraphEnd(DocumentModelPosition pos, ParagraphIndex paragraphIndex) {
			PieceTable pieceTable = pos.PieceTable;
			Paragraph paragraph = pieceTable.Paragraphs[paragraphIndex];
			pos.LogPosition = paragraph.LogPosition + paragraph.Length;
			pos.ParagraphIndex = paragraphIndex;
			pos.RunIndex = paragraph.LastRunIndex;
			pos.RunStartLogPosition = pos.LogPosition - pieceTable.Runs[paragraph.LastRunIndex].Length;
		}
		public static void SetRunStart(DocumentModelPosition pos, RunIndex runIndex) {
			PieceTable pieceTable = pos.PieceTable;
			Paragraph paragraph = pieceTable.Runs[runIndex].Paragraph;
			int offset = 0;
			for (RunIndex i = paragraph.FirstRunIndex; i < runIndex; i++)
				offset += pieceTable.Runs[i].Length;
			pos.LogPosition = paragraph.LogPosition + offset;
			pos.ParagraphIndex = paragraph.Index;
			pos.RunIndex = runIndex;
			pos.RunStartLogPosition = pos.LogPosition;
		}
		public static void SetRunEnd(DocumentModelPosition pos, RunIndex runIndex) {
			PieceTable pieceTable = pos.PieceTable;
			Paragraph paragraph = pieceTable.Runs[runIndex].Paragraph;
			int offset = 0;
			for (RunIndex i = paragraph.FirstRunIndex; i <= runIndex; i++)
				offset += pieceTable.Runs[i].Length;
			pos.LogPosition = paragraph.LogPosition + offset;
			pos.ParagraphIndex = paragraph.Index;
			if (paragraph.LastRunIndex == runIndex)
				pos.ParagraphIndex++;
			pos.RunIndex = runIndex + 1;
			pos.RunStartLogPosition = pos.LogPosition;
		}
		public static DocumentModelPosition MoveBackward(DocumentModelPosition pos) {
			DocumentModelPosition result = pos.Clone();
			MoveBackwardCore(result);
			return result;
		}
		public static void MoveBackwardCore(DocumentModelPosition pos) {
			pos.LogPosition--;
			if (pos.LogPosition < pos.RunStartLogPosition) {
				pos.RunIndex--;
				TextRunBase run = pos.PieceTable.Runs[pos.RunIndex];
				pos.ParagraphIndex = run.Paragraph.Index;
				pos.RunStartLogPosition = pos.LogPosition - run.Length + 1;
			}
		}
		public static DocumentModelPosition MoveForward(DocumentModelPosition pos) {
			DocumentModelPosition result = pos.Clone();
			MoveForwardCore(result);
			return result;
		}
		public static void MoveForwardCore(DocumentModelPosition result) {
			result.LogPosition++;
			if (result.LogPosition > result.RunEndLogPosition) {
				result.RunIndex++;
				result.ParagraphIndex = result.PieceTable.Runs[result.RunIndex].Paragraph.Index;
				result.RunStartLogPosition = result.LogPosition;
			}
		}
		#region ICloneable<DocumentPosition2> Members
		public DocumentModelPosition Clone() {
			DocumentModelPosition clone = CreateEmptyClone();
			clone.CopyFrom(this);
			return clone;
		}
		protected virtual DocumentModelPosition CreateEmptyClone() {
			return new DocumentModelPosition(PieceTable);
		}
		#endregion
		public void CopyFrom(DocumentModelPosition pos) {
			this.LogPosition = pos.LogPosition;
			this.ParagraphIndex = pos.ParagraphIndex;
			this.RunIndex = pos.RunIndex;
			this.RunStartLogPosition = pos.RunStartLogPosition;
		}
		#region IComparable<DocumentModelPosition> Members
		int IComparable<DocumentModelPosition>.CompareTo(DocumentModelPosition other) {
			if (LogPosition < other.LogPosition)
				return -1;
			if (LogPosition > other.LogPosition)
				return 1;
			else
				return 0;
		}
		#endregion
#if DEBUG
		public override string ToString() {
			return String.Format("parIdx={0}, runIdx={1}, logPos={2}", ParagraphIndex, RunIndex, LogPosition);
		}
#endif
	}
	#endregion
	#region MaxDocumentPosition2
	public class MaxDocumentPosition2 : DocumentModelPosition {
		public MaxDocumentPosition2() {
			LogPosition = DocumentLogPosition.MaxValue;
		}
		public override DocumentLogPosition RunEndLogPosition { get { return DocumentLogPosition.MaxValue; } }
	}
	#endregion
	#region DocumentModelPositionAnchor
	public class DocumentModelPositionAnchor : IDocumentModelStructureChangedListener {
		bool positionChanged;
		readonly DocumentModelPosition pos;
		List<int> notChangeNotificatiodIds;
		protected internal DocumentModelPositionAnchor(DocumentModelPosition pos, List<int> notChangeNotificatiodIds) {
			this.notChangeNotificatiodIds = notChangeNotificatiodIds;
			this.pos = pos;
		}
		public DocumentModelPositionAnchor(DocumentModelPosition pos) : this(pos, null) {			
		}
		protected internal List<int> NotChangeNotificatiodIds { get { return notChangeNotificatiodIds; } }
		public bool PositionChanged { get { return positionChanged; } }
		public DocumentModelPosition Position { get { return pos; } }
		#region IDocumentModelStructureChangedListener Members
		public virtual void OnParagraphInserted(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, TableCell cell, bool isParagraphMerged, ParagraphIndex actualParagraphIndex, int historyNotificationId) {
			if (!Object.ReferenceEquals(pos.PieceTable, pieceTable))
				return;
			this.positionChanged = OnParagraphInsertedCore(paragraphIndex, runIndex, historyNotificationId);
		}
		public virtual void OnParagraphRemoved(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
			if (!Object.ReferenceEquals(pos.PieceTable, pieceTable))
				return;
			this.positionChanged = OnParagraphRemovedCore(paragraphIndex, runIndex, historyNotificationId);
		}
		public virtual void OnParagraphMerged(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
			if (!Object.ReferenceEquals(pos.PieceTable, pieceTable))
				return;
			this.positionChanged = OnParagraphMergedCore(paragraphIndex, runIndex, historyNotificationId);
		}
		public virtual void OnRunInserted(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length, int historyNotificationId) {
			if (!Object.ReferenceEquals(pos.PieceTable, pieceTable))
				return;
			this.positionChanged = OnRunInsertedCore(newRunIndex, length, historyNotificationId);
		}
		public virtual void OnRunRemoved(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int length, int historyNotificationId) {
			if (!Object.ReferenceEquals(pos.PieceTable, pieceTable))
				return;
			this.positionChanged = OnRunRemovedCore(runIndex, length, historyNotificationId);
		}
		public void OnBeginMultipleRunSplit(PieceTable pieceTable) {
		}
		public void OnEndMultipleRunSplit(PieceTable pieceTable) {
		}
		public virtual void OnRunSplit(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int splitOffset) {
			if (!Object.ReferenceEquals(pos.PieceTable, pieceTable))
				return;
			this.positionChanged = OnRunSplitCore(runIndex, splitOffset);
		}
		public void OnRunJoined(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex joinedRunIndex, int splitOffset, int tailRunLength) {
			if (!Object.ReferenceEquals(pos.PieceTable, pieceTable))
				return;
			this.positionChanged = OnRunJoinedCore(joinedRunIndex, splitOffset, tailRunLength);
		}
		public void OnRunMerged(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
			if (!Object.ReferenceEquals(pos.PieceTable, pieceTable))
				return;
			this.positionChanged = OnRunMergedCore(runIndex, deltaRunLength);
		}
		public void OnRunUnmerged(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
			if (!Object.ReferenceEquals(pos.PieceTable, pieceTable))
				return;
			this.positionChanged = OnRunUnmergedCore(runIndex, deltaRunLength);
		}
		public void OnFieldInserted(PieceTable pieceTable, int fieldIndex) {
			if (!Object.ReferenceEquals(pos.PieceTable, pieceTable))
				return;
			OnFieldInsertedCore(fieldIndex);
		}
		public void OnFieldRemoved(PieceTable pieceTable, int fieldIndex) {
			if (!Object.ReferenceEquals(pos.PieceTable, pieceTable))
				return;
			OnFieldRemovedCore(fieldIndex);
		}
		#endregion
		protected bool ShouldChange(int historyNotificationId) {
			if (notChangeNotificatiodIds == null || historyNotificationId == NotificationIdGenerator.EmptyId)
				return true;
			else
				return notChangeNotificatiodIds.BinarySearch(historyNotificationId) < 0;
		}
		protected void AddNotChangeNotificationIds(int historyNotificationId) {
			if (historyNotificationId == NotificationIdGenerator.EmptyId)
				return;
			if (notChangeNotificatiodIds == null)
				notChangeNotificatiodIds = new List<int>();
			int index = notChangeNotificatiodIds.BinarySearch(historyNotificationId);
			if (index >= 0)
				return;
			notChangeNotificatiodIds.Insert(~index, historyNotificationId);
		}
		protected internal virtual bool OnParagraphInsertedCore(ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
			if (paragraphIndex >= pos.ParagraphIndex) {
				if (runIndex < pos.RunIndex || (runIndex == pos.RunIndex && ShouldChange(historyNotificationId))) {					
					pos.ParagraphIndex++;
					return true;
				}
				else
					return false;
			}
			else if (paragraphIndex < pos.ParagraphIndex) {
				pos.ParagraphIndex++;
				return true;
			}
			else
				return false;
		}
		protected internal virtual bool OnParagraphRemovedCore(ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
			if (paragraphIndex < pos.ParagraphIndex) {
				if (pos.ParagraphIndex > ParagraphIndex.Zero) {
					pos.ParagraphIndex--;
					return true;
				}
			}
			else if (paragraphIndex == pos.ParagraphIndex)
				AddNotChangeNotificationIds(historyNotificationId);
			return false;
		}
		protected internal virtual bool OnParagraphMergedCore(ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
			if (paragraphIndex == pos.ParagraphIndex) {
				if (runIndex <= pos.RunIndex) {
					if (pos.ParagraphIndex > ParagraphIndex.Zero) {
						pos.ParagraphIndex--;
						return true;
					}
				}
				return false;
			}
			else if (paragraphIndex < pos.ParagraphIndex) {
				if (pos.ParagraphIndex > ParagraphIndex.Zero) {
					pos.ParagraphIndex--;
					return true;
				}
			}
			else if (paragraphIndex == pos.ParagraphIndex + 1) {
				if (runIndex == pos.RunIndex)
					AddNotChangeNotificationIds(historyNotificationId);
			}
			return false;
		}
		protected internal virtual bool OnRunInsertedCore(RunIndex newRunIndex, int length, int historyNotificationId) {
			if (newRunIndex < pos.RunIndex || (newRunIndex == pos.RunIndex && ShouldChange(historyNotificationId))) {
				pos.RunIndex++;
				pos.RunStartLogPosition += length;
				pos.LogPosition += length;
				return true;
			}
			else
				return false;
		}
		protected internal virtual bool OnRunRemovedCore(RunIndex runIndex, int length, int historyNotificationId) {
			if (runIndex == pos.RunIndex) {
				if (pos.RunOffset == 0)
					AddNotChangeNotificationIds(historyNotificationId);
				else
					pos.LogPosition -= pos.RunOffset;
				return true;
			}
			else if (runIndex < pos.RunIndex) {
				pos.RunIndex--;
				pos.RunStartLogPosition -= length;
				pos.LogPosition -= length;
				return true;
			}
			else
				return false;
		}
		protected internal virtual bool OnRunSplitCore(RunIndex runIndex, int splitOffset) {
			if (runIndex < pos.RunIndex) {
				pos.RunIndex++;
				return true;
			}
			else if (runIndex == pos.RunIndex) {
				if (splitOffset <= pos.RunOffset) {
					pos.RunStartLogPosition += pos.PieceTable.Runs[runIndex].Length;
					pos.RunIndex++;
					return true;
				}
			}
			return false;
		}
		protected internal virtual bool OnRunJoinedCore(RunIndex joinedRunIndex, int splitOffset, int tailRunLength) {
			if (joinedRunIndex + 1 == pos.RunIndex) {
				pos.RunIndex--;
				pos.RunStartLogPosition -= (pos.PieceTable.Runs[joinedRunIndex].Length - tailRunLength);
				return true;
			}
			else if (joinedRunIndex < pos.RunIndex) {
				pos.RunIndex--;
				return true;
			}
			else
				return false;
		}
		protected internal virtual bool OnRunMergedCore(RunIndex runIndex, int deltaRunLength) {
			if (runIndex < pos.RunIndex) {
				pos.LogPosition += deltaRunLength;
				pos.RunStartLogPosition += deltaRunLength;
				return true;
			}
			else
				return false;
		}
		protected internal virtual bool OnRunUnmergedCore(RunIndex runIndex, int deltaRunLength) {
			if (runIndex < pos.RunIndex) {
				pos.LogPosition += deltaRunLength;
				pos.RunStartLogPosition += deltaRunLength;
				return true;
			}
			if (runIndex == pos.RunIndex) {
				int newRunLength = pos.PieceTable.Runs[runIndex].Length;
				int prevRunOffset = pos.LogPosition - pos.RunStartLogPosition;
				if (prevRunOffset >= newRunLength) {
					pos.RunIndex++;
					pos.RunStartLogPosition += pos.PieceTable.Runs[runIndex].Length;
					pos.LogPosition -= prevRunOffset - newRunLength;
				}
				return true;
			}
			else
				return false;
		}
		protected internal virtual void OnFieldInsertedCore(int fieldIndex) {
		}
		protected internal virtual void OnFieldRemovedCore(int fieldIndex) {
		}
	}
	#endregion
	public abstract class ReferencedDocumentModelPositionBase : DocumentModelPosition, IComparable<ReferencedDocumentModelPositionBase> {
		int referenceCount;
		List<int> changeNotificationIds;
		protected ReferencedDocumentModelPositionBase(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public bool HasReference { get { return referenceCount > 0; } }
		protected List<int> ChangeNotificationIds { get { return changeNotificationIds; } }
		public abstract bool OnRunInserted(RunIndex newRunIndex, int length, int historyNotificationId);
		public abstract bool OnRunRemoved(RunIndex runIndex, int length, int historyNotificationId);
		public void AddChangeNotificationId(int historyNotificationId) {
			if (historyNotificationId == NotificationIdGenerator.EmptyId)
				return;
			if (changeNotificationIds == null)
				changeNotificationIds = new List<int>();
			int index = ChangeNotificationIds.BinarySearch(historyNotificationId);
			if (index >= 0)
				return;
			ChangeNotificationIds.Insert(~index, historyNotificationId);
		}
		public void IncReferences() {
			referenceCount++;
		}
		public void DecReferences() {
			referenceCount--;
		}
		int IComparable<ReferencedDocumentModelPositionBase>.CompareTo(ReferencedDocumentModelPositionBase other) {
			return ((IComparable<DocumentModelPosition>)this).CompareTo(other);
		}
	}
	public class ReferencedBookmarkDocumentModelPosition : ReferencedDocumentModelPositionBase {
		public ReferencedBookmarkDocumentModelPosition(PieceTable pieceTable)
			: base(pieceTable) {			
		}
		public override bool OnRunInserted(RunIndex newRunIndex, int length, int historyNotificationId) {
			if (newRunIndex < RunIndex || (newRunIndex == RunIndex && (RunOffset > 0 || ShouldChange(historyNotificationId)))) {
				RunIndex++;
				RunStartLogPosition += length;
				LogPosition += length;
				return true;
			}
			return false;
		}
		public override bool OnRunRemoved(RunIndex runIndex, int length, int historyNotificationId) {
			if (runIndex == RunIndex) {
				if (RunOffset > 0)
					LogPosition -= RunOffset;
				return true;
			}
			else if (runIndex < RunIndex) {
				if (runIndex == (RunIndex - 1) && RunOffset == 0)
					AddChangeNotificationId(historyNotificationId);
				RunIndex--;
				RunStartLogPosition -= length;
				LogPosition -= length;
				return true;
			}
			return false;
		}
		bool ShouldChange(int historyNotificationId) {
			if (ChangeNotificationIds == null || historyNotificationId == NotificationIdGenerator.EmptyId)
				return false;
			else
				return ChangeNotificationIds.BinarySearch(historyNotificationId) >= 0;
		}
		protected override DocumentModelPosition CreateEmptyClone() {
			return new ReferencedBookmarkDocumentModelPosition(PieceTable);
		}
	}
	public class ReferencedIntervalDocumentModelPosition : ReferencedDocumentModelPositionBase {
		public ReferencedIntervalDocumentModelPosition(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public override bool OnRunInserted(RunIndex newRunIndex, int length, int historyNotificationId) {			
			if (newRunIndex < RunIndex || (newRunIndex == RunIndex && ShouldChange(historyNotificationId))) {
				RunIndex++;
				RunStartLogPosition += length;
				LogPosition += length;
				return true;
			}
			else
				return false;
		}
		public override bool OnRunRemoved(RunIndex runIndex, int length, int historyNotificationId) {
			if (runIndex == RunIndex) {
				if (RunOffset == 0)
					AddChangeNotificationId(historyNotificationId);
				else
					LogPosition -= RunOffset;
				return true;
			}
			else if (runIndex < RunIndex) {
				RunIndex--;
				RunStartLogPosition -= length;
				LogPosition -= length;
				return true;
			}
			else
				return false;
		}
		bool ShouldChange(int historyNotificationId) {
			if (ChangeNotificationIds == null || historyNotificationId == NotificationIdGenerator.EmptyId)
				return true;
			else
				return ChangeNotificationIds.BinarySearch(historyNotificationId) < 0;
		}
		protected override DocumentModelPosition CreateEmptyClone() {
			return new ReferencedIntervalDocumentModelPosition(PieceTable);
		}
	}
	public class DocumentModelPositionManager : IDocumentModelStructureChangedListener {
		List<ReferencedDocumentModelPositionBase> positions;
		PieceTable pieceTable;
		public DocumentModelPositionManager(PieceTable pieceTable) {
			this.positions = new List<ReferencedDocumentModelPositionBase>();			
			this.pieceTable = pieceTable;
		}
		int Count { get { return positions.Count; } }
		PieceTable PieceTable { get { return pieceTable; } }
		public virtual void Clear() {
			positions.Clear();
		}
		public virtual ReferencedDocumentModelPositionBase RegisterNewPosition(ReferencedDocumentModelPositionBase position) {
			Guard.ArgumentNotNull(position, "position");
			int index = positions.BinarySearch(position);
			if (index >= 0)
				position = positions[index];
			else
				positions.Insert(~index, position);
			position.IncReferences();
			return position;
		}
		public virtual void DetachPosition(ReferencedDocumentModelPositionBase position) {
			if(!position.HasReference)
				Exceptions.ThrowArgumentException("position.HasReference", position.HasReference);
			position.DecReferences();
			if (position.HasReference)
				return;
			int index = positions.BinarySearch(position);
			if (index < 0)
				Exceptions.ThrowArgumentException("position", position);
			positions.RemoveAt(index);
		}
		public virtual void AttachPosition(ReferencedDocumentModelPositionBase position) {
			if (!position.HasReference) {
				int index = positions.BinarySearch(position);
				if (index >= 0)
					Exceptions.ThrowInternalException();
				positions.Insert(~index, position);
			}
			position.IncReferences();
		}
		protected delegate bool ProcessDocumentModelPositionAction(ReferencedDocumentModelPositionBase position);
		protected void ForEach(PieceTable pieceTable, ProcessDocumentModelPositionAction action) {
			if (!Object.ReferenceEquals(this.PieceTable, pieceTable))
				return;
			int count = positions.Count;
			for (int i = count - 1; i >= 0; i--)
				if (!action(positions[i]))
					break;
		}
		protected void ForEach(PieceTable pieceTable, Action<DocumentModelPositionAnchor> action) {
			if (!Object.ReferenceEquals(this.PieceTable, pieceTable))
				return;
			int count = positions.Count;
			for (int i = 0; i < count; i++) {
				DocumentModelPositionAnchor anchor = new DocumentModelPositionAnchor(positions[i], null);
				action(anchor);
			}
		}
		public void OnParagraphInserted(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, TableCell cell, bool isParagraphMerged, ParagraphIndex actualParagraphIndex, int historyNotificationId) {
			ForEach(pieceTable, anchor => anchor.OnParagraphInserted(pieceTable, sectionIndex, paragraphIndex, runIndex, cell, isParagraphMerged, actualParagraphIndex, historyNotificationId));
		}
		public void OnParagraphRemoved(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
			ForEach(pieceTable, anchor => anchor.OnParagraphRemoved(pieceTable, sectionIndex, paragraphIndex, runIndex, historyNotificationId));
		}
		public void OnParagraphMerged(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
			ForEach(pieceTable, anchor => anchor.OnParagraphMerged(pieceTable, sectionIndex, paragraphIndex, runIndex, historyNotificationId));
		}
		public void OnRunInserted(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length, int historyNotificationId) {
			ForEach(pieceTable, position => position.OnRunInserted(newRunIndex, length, historyNotificationId));
		}
		public void OnRunRemoved(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int length, int historyNotificationId) {
			ForEach(pieceTable, position => position.OnRunRemoved(runIndex, length, historyNotificationId));
		}
		public void OnRunSplit(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int splitOffset) {
			ForEach(pieceTable, anchor => anchor.OnRunSplit(pieceTable, paragraphIndex, runIndex, splitOffset));
		}
		public void OnRunJoined(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex joinedRunIndex, int splitOffset, int tailRunLength) {
			ForEach(pieceTable, anchor => anchor.OnRunJoined(pieceTable, paragraphIndex, joinedRunIndex, splitOffset, tailRunLength));
		}
		public void OnRunMerged(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
			ForEach(pieceTable, anchor => anchor.OnRunMerged(pieceTable, paragraphIndex, runIndex, deltaRunLength));
		}
		public void OnRunUnmerged(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
			ForEach(pieceTable, anchor => anchor.OnRunUnmerged(pieceTable, paragraphIndex, runIndex, deltaRunLength));
		}
		public void OnFieldRemoved(PieceTable pieceTable, int fieldIndex) {
			ForEach(pieceTable, anchor => anchor.OnFieldRemoved(pieceTable, fieldIndex));
		}
		public void OnFieldInserted(PieceTable pieceTable, int fieldIndex) {
			ForEach(pieceTable, anchor => anchor.OnFieldInserted(pieceTable, fieldIndex));
		}
		public void OnBeginMultipleRunSplit(PieceTable pieceTable) {
		}
		public void OnEndMultipleRunSplit(PieceTable pieceTable) {
		}
	}
}
