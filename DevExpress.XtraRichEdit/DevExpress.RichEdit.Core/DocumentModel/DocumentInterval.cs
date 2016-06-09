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
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Utils;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Model {
	#region DocumentInterval (abstract class)
	public abstract class DocumentInterval : IDocumentModelStructureChangedListener {
		#region Fields
		readonly PieceTable pieceTable;
		List<int> notChangedStartIds;
		List<int> notChangedEndIds;
		RunInfo interval;
		#endregion
		protected DocumentInterval(PieceTable pieceTable) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
			this.interval = CreateRunInfo(pieceTable);
		}
		#region Properties
		public DocumentModel DocumentModel { get { return pieceTable.DocumentModel; } }
		public PieceTable PieceTable { get { return pieceTable; } }
		#region Start
		public virtual DocumentLogPosition Start {
			get { return interval.Start.LogPosition; }
		}
		protected virtual void SetStartCore(DocumentLogPosition value) {
			if (Start == value)
				return;
			interval.Start.LogPosition = value;
			UpdateStartPosition();
			OnChanged(true, false);
		}
		#endregion
		#region End
		public virtual DocumentLogPosition End {
			get { return interval.End.LogPosition; }
		}
		protected virtual void SetEndCore(DocumentLogPosition value) {
			if (End == value)
					return;
				interval.End.LogPosition = value;
				UpdateEndPosition();
				OnChanged(false, true);
		}
		#endregion
		public DocumentLogPosition NormalizedStart { get { return Interval.NormalizedStart.LogPosition; } }
		public DocumentLogPosition NormalizedEnd { get { return Interval.NormalizedEnd.LogPosition; } }
		public int Length { get { return Math.Abs(End - Start); } }
		public RunInfo Interval { get { return interval; } }
		#endregion
		protected virtual RunInfo CreateRunInfo(PieceTable pieceTable) {
			return new RunInfo(pieceTable);
		}
		protected internal virtual bool SetPositionCore(DocumentModelPosition pos, DocumentModelPosition value) {
			if (pos.Equals(value))
				return false;
			pos.CopyFrom(value);
			return true;
		}
		protected internal virtual void SetStartCore(DocumentModelPosition pos) {
			if (SetPositionCore(Interval.Start, pos))
				OnChanged(true, false);
		}
		protected internal virtual void SetEndCore(DocumentModelPosition pos) {
			if (SetPositionCore(Interval.End, pos))
				OnChanged(false, true);
		}
		protected internal virtual void OnChanged(bool startChanged, bool endChanged) {
			if (startChanged || endChanged)
				OnChangedCore();
		}
		protected internal virtual void UpdateStartPosition() {
			PieceTable.CalculateRunInfoStart(Start, interval);
		}
		protected internal virtual void UpdateEndPosition() {
			PieceTable.CalculateRunInfoEnd(End, interval);
		}
		public bool Contains(DocumentLogPosition start, DocumentLogPosition end) {
			return start >= NormalizedStart && end <= NormalizedEnd;
		}
		public bool Contains(DocumentInterval interval) {
			if (interval == null)
				return false;
			return interval.NormalizedStart >= NormalizedStart && interval.NormalizedEnd <= NormalizedEnd;
		}
		public bool IntersectsWith(DocumentLogPosition start, DocumentLogPosition end) {
			return end >= NormalizedStart && start <= NormalizedEnd;
		}
		public bool IntersectsWith(DocumentInterval interval) {
			if (interval == null)
				return false;
			return interval.NormalizedEnd >= NormalizedStart && interval.NormalizedStart <= NormalizedEnd;
		}
		public bool IntersectsWithExcludingBounds(DocumentInterval interval) {
			if (interval == null)
				return false;
			if (this.Length == 0 && interval.Length == 0 && interval.NormalizedStart == NormalizedStart)
				return true;
			return interval.NormalizedEnd > NormalizedStart && interval.NormalizedStart < NormalizedEnd;
		}
		protected internal abstract void OnChangedCore();
		#region IDocumentModelStructureChangedListener Members
		void IDocumentModelStructureChangedListener.OnParagraphInserted(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, TableCell cell, bool isParagraphMerged, ParagraphIndex actualParagraphIndex, int historyNotificationId) {
			if (!Object.ReferenceEquals(this.PieceTable, pieceTable))
				return;
			OnParagraphInserted(sectionIndex, paragraphIndex, runIndex, cell, isParagraphMerged, actualParagraphIndex, historyNotificationId);
		}
		void IDocumentModelStructureChangedListener.OnParagraphRemoved(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
			if (!Object.ReferenceEquals(this.PieceTable, pieceTable))
				return;
			OnParagraphRemoved(sectionIndex, paragraphIndex, runIndex, historyNotificationId);
		}
		void IDocumentModelStructureChangedListener.OnParagraphMerged(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
			if (!Object.ReferenceEquals(this.PieceTable, pieceTable))
				return;
			OnParagraphMerged(sectionIndex, paragraphIndex, runIndex, historyNotificationId);
		}
		void IDocumentModelStructureChangedListener.OnRunInserted(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length, int historyNotificationId) {
			if (!Object.ReferenceEquals(this.PieceTable, pieceTable))
				return;
			OnRunInserted(paragraphIndex, newRunIndex, length, historyNotificationId);
		}
		void IDocumentModelStructureChangedListener.OnRunRemoved(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int length, int historyNotificationId) {
			if (!Object.ReferenceEquals(this.PieceTable, pieceTable))
				return;
			OnRunRemoved(paragraphIndex, runIndex, length, historyNotificationId);
		}
		void IDocumentModelStructureChangedListener.OnBeginMultipleRunSplit(PieceTable pieceTable) {
		}
		void IDocumentModelStructureChangedListener.OnEndMultipleRunSplit(PieceTable pieceTable) {
		}
		void IDocumentModelStructureChangedListener.OnRunSplit(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int splitOffset) {
			if (!Object.ReferenceEquals(this.PieceTable, pieceTable))
				return;
			OnRunSplit(paragraphIndex, runIndex, splitOffset);
		}
		void IDocumentModelStructureChangedListener.OnRunJoined(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex joinedRunIndex, int splitOffset, int tailRunLength) {
			if (!Object.ReferenceEquals(this.PieceTable, pieceTable))
				return;
			OnRunJoined(paragraphIndex, joinedRunIndex, splitOffset, tailRunLength);
		}
		void IDocumentModelStructureChangedListener.OnRunMerged(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
			if (!Object.ReferenceEquals(this.PieceTable, pieceTable))
				return;
			OnRunMerged(paragraphIndex, runIndex, deltaRunLength);
		}
		void IDocumentModelStructureChangedListener.OnRunUnmerged(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
			if (!Object.ReferenceEquals(this.PieceTable, pieceTable))
				return;
			OnRunUnmerged(paragraphIndex, runIndex, deltaRunLength);
		}
		void IDocumentModelStructureChangedListener.OnFieldInserted(PieceTable pieceTable, int fieldIndex) {
			if (!Object.ReferenceEquals(this.PieceTable, pieceTable))
				return;
			OnFieldInserted(fieldIndex);
		}
		void IDocumentModelStructureChangedListener.OnFieldRemoved(PieceTable pieceTable, int fieldIndex) {
			if (!Object.ReferenceEquals(this.PieceTable, pieceTable))
				return;
			OnFieldRemoved(fieldIndex);
		}
		#endregion
		#region OnParagraphInserted
		protected internal virtual void OnParagraphInserted(SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, TableCell cell, bool isParagraphMerged, ParagraphIndex actualParagraphIndex, int historyNotificationId) {
			DocumentModelPositionAnchor startAnchor = new DocumentModelPositionAnchor(Interval.Start, notChangedStartIds);
			DocumentModelPositionAnchor endAnchor = new DocumentModelPositionAnchor(Interval.End, notChangedEndIds);
			startAnchor.OnParagraphInserted(PieceTable, sectionIndex, paragraphIndex, runIndex, cell, isParagraphMerged, actualParagraphIndex, historyNotificationId);
			endAnchor.OnParagraphInserted(PieceTable, sectionIndex, paragraphIndex, runIndex, cell, isParagraphMerged, actualParagraphIndex, historyNotificationId);
			notChangedStartIds = startAnchor.NotChangeNotificatiodIds;
			notChangedEndIds = endAnchor.NotChangeNotificatiodIds;
			OnChanged(startAnchor.PositionChanged, endAnchor.PositionChanged);
		}
		#endregion
		#region OnParagraphRemoved
		protected internal virtual void OnParagraphRemoved(SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
			DocumentModelPositionAnchor startAnchor = new DocumentModelPositionAnchor(Interval.Start, notChangedStartIds);
			DocumentModelPositionAnchor endAnchor = new DocumentModelPositionAnchor(Interval.End, notChangedEndIds);
			startAnchor.OnParagraphRemoved(PieceTable, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
			endAnchor.OnParagraphRemoved(PieceTable, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
			notChangedStartIds = startAnchor.NotChangeNotificatiodIds;
			notChangedEndIds = endAnchor.NotChangeNotificatiodIds;
			OnChanged(startAnchor.PositionChanged, endAnchor.PositionChanged);
		}
		#endregion
		#region OnParagraphMerged
		protected internal virtual void OnParagraphMerged(SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
			DocumentModelPositionAnchor startAnchor = new DocumentModelPositionAnchor(Interval.Start, notChangedStartIds);
			DocumentModelPositionAnchor endAnchor = new DocumentModelPositionAnchor(Interval.End, notChangedEndIds);
			startAnchor.OnParagraphMerged(PieceTable, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
			endAnchor.OnParagraphMerged(PieceTable, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
			notChangedStartIds = startAnchor.NotChangeNotificatiodIds;
			notChangedEndIds = endAnchor.NotChangeNotificatiodIds;
			OnChanged(startAnchor.PositionChanged, endAnchor.PositionChanged);
		}
		#endregion
		#region OnRunInserted
		protected internal virtual void OnRunInserted(ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length, int historyNotificationId) {
			DocumentModelPositionAnchor startAnchor = new DocumentModelPositionAnchor(Interval.Start, notChangedStartIds);
			DocumentModelPositionAnchor endAnchor = new DocumentModelPositionAnchor(Interval.End, notChangedEndIds);
			startAnchor.OnRunInserted(PieceTable, paragraphIndex, newRunIndex, length, historyNotificationId);
			endAnchor.OnRunInserted(PieceTable, paragraphIndex, newRunIndex, length, historyNotificationId);
			notChangedStartIds = startAnchor.NotChangeNotificatiodIds;
			notChangedEndIds = endAnchor.NotChangeNotificatiodIds;
			OnChanged(startAnchor.PositionChanged, endAnchor.PositionChanged);
		}
		#endregion
		#region OnRunRemoved
		protected internal virtual void OnRunRemoved(ParagraphIndex paragraphIndex, RunIndex runIndex, int length, int historyNotificationId) {
			DocumentModelPositionAnchor startAnchor = new DocumentModelPositionAnchor(Interval.Start, notChangedStartIds);
			DocumentModelPositionAnchor endAnchor = new DocumentModelPositionAnchor(Interval.End, notChangedEndIds);
			startAnchor.OnRunRemoved(PieceTable, paragraphIndex, runIndex, length, historyNotificationId);
			endAnchor.OnRunRemoved(PieceTable, paragraphIndex, runIndex, length, historyNotificationId);
			notChangedStartIds = startAnchor.NotChangeNotificatiodIds;
			notChangedEndIds = endAnchor.NotChangeNotificatiodIds;
			OnChanged(startAnchor.PositionChanged, endAnchor.PositionChanged);
		}
		#endregion
		#region OnRunSplit
		protected internal virtual void OnRunSplit(ParagraphIndex paragraphIndex, RunIndex runIndex, int splitOffset) {
			DocumentModelPositionAnchor startAnchor = new DocumentModelPositionAnchor(Interval.Start);
			DocumentModelPositionAnchor endAnchor = new DocumentModelPositionAnchor(Interval.End);
			startAnchor.OnRunSplit(PieceTable, paragraphIndex, runIndex, splitOffset);
			endAnchor.OnRunSplit(PieceTable, paragraphIndex, runIndex, splitOffset);
			OnChanged(startAnchor.PositionChanged, endAnchor.PositionChanged);
		}
		#endregion
		#region OnRunJoined
		protected internal virtual void OnRunJoined(ParagraphIndex paragraphIndex, RunIndex joinedRunIndex, int splitOffset, int tailRunLength) {
			DocumentModelPositionAnchor startAnchor = new DocumentModelPositionAnchor(Interval.Start);
			DocumentModelPositionAnchor endAnchor = new DocumentModelPositionAnchor(Interval.End);
			startAnchor.OnRunJoined(PieceTable, paragraphIndex, joinedRunIndex, splitOffset, tailRunLength);
			endAnchor.OnRunJoined(PieceTable, paragraphIndex, joinedRunIndex, splitOffset, tailRunLength);
			OnChanged(startAnchor.PositionChanged, endAnchor.PositionChanged);
		}
		#endregion
		#region OnRunMerged
		protected internal virtual void OnRunMerged(ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
			DocumentModelPositionAnchor startAnchor = new DocumentModelPositionAnchor(Interval.Start);
			DocumentModelPositionAnchor endAnchor = new DocumentModelPositionAnchor(Interval.End);
			startAnchor.OnRunMerged(PieceTable, paragraphIndex, runIndex, deltaRunLength);
			endAnchor.OnRunMerged(PieceTable, paragraphIndex, runIndex, deltaRunLength);
			OnChanged(startAnchor.PositionChanged, endAnchor.PositionChanged);
		}
		#endregion
		#region OnRunUnmerged
		protected internal virtual void OnRunUnmerged(ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
			DocumentModelPositionAnchor startAnchor = new DocumentModelPositionAnchor(Interval.Start);
			DocumentModelPositionAnchor endAnchor = new DocumentModelPositionAnchor(Interval.End);
			startAnchor.OnRunUnmerged(PieceTable, paragraphIndex, runIndex, deltaRunLength);
			endAnchor.OnRunUnmerged(PieceTable, paragraphIndex, runIndex, deltaRunLength);
			OnChanged(startAnchor.PositionChanged, endAnchor.PositionChanged);
		}
		#endregion
		#region OnFieldInserted
		protected internal virtual void OnFieldInserted(int fieldIndex) {
		}
		#endregion
		#region OnFieldRemoved
		protected internal virtual void OnFieldRemoved(int fieldIndex) {
		}
		#endregion
#if DEBUG
		public override string ToString() {
			return Interval.ToString();
		}
#endif
	}
	#endregion
	public abstract class ChangableDocumentInterval : DocumentInterval {
		protected ChangableDocumentInterval(PieceTable pieceTable) : base(pieceTable) {
		}
		public virtual new DocumentLogPosition Start {
			get {
				return base.Start;
			}
			set {
				SetStartCore(value);
			}
		}
		public virtual new DocumentLogPosition End {
			get {
				return base.End;
			}
			set {
				SetEndCore(value);
			}
		}
	}
	public abstract class VisitableDocumentInterval : DocumentInterval {
		protected VisitableDocumentInterval(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public abstract void Visit(IDocumentIntervalVisitor visitor);
	}
	#region DocumentIntervalCollection<T>
	public class DocumentIntervalCollection<T> : IDocumentModelStructureChangedListener where T : DocumentInterval, IDocumentModelStructureChangedListener {
		readonly PieceTable pieceTable;
		readonly List<T> innerList;
		protected DocumentIntervalCollection(PieceTable pieceTable) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
			this.innerList = new List<T>();
		}
		#region Properties
		public PieceTable PieceTable { get { return pieceTable; } }
		public T this[int index] { get { return innerList[index]; } }
		public int Count { get { return innerList.Count; } }
		protected internal List<T> InnerList { get { return innerList; } }
		#endregion
		#region Events
		#region Inserted
		DocumentIntervalEventHandler onInserted;
		internal event DocumentIntervalEventHandler Inserted { add { onInserted += value; } remove { onInserted -= value; } }
		protected void RaiseDocumentIntervalInserted(int index) {
			if (onInserted != null)
				onInserted(this, new DocumentIntervalEventArgs(index));
		}
		#endregion
		#region Removed
		DocumentIntervalEventHandler onRemoved;
		internal event DocumentIntervalEventHandler Removed { add { onRemoved += value; } remove { onRemoved -= value; } }
		protected void RaiseDocumentIntervalRemoved(int index) {
			if (onRemoved != null)
				onRemoved(this, new DocumentIntervalEventArgs(index));
		}
		#endregion
		#endregion
		public virtual int Add(T value) {
			return AddCore(value);
		}
		protected internal virtual int AddCore(T value) {
			Debug.Assert(Object.ReferenceEquals(value.PieceTable, this.PieceTable));
			int index = innerList.BinarySearch(value, CreateDocumentIntervalComparer());
			if (index < 0)
				index = ~index;
			else
				index++;
			Insert(index, value);
			return index;
		}
		protected virtual IComparer<T> CreateDocumentIntervalComparer() {
			return new DocumentIntervalComparer<T>();
		}
		public void Insert(int index, T value) {
			InsertCore(index, value);
			OnDocumentIntervalInserted(index);
		}
		protected internal virtual void InsertCore(int index, T value) {
			Debug.Assert(Object.ReferenceEquals(value.PieceTable, this.PieceTable));
			innerList.Insert(index, value);
		}
		public void RemoveAt(int index) {
			RemoveAtCore(index);
			OnDocumentIntervalRemoved(index);
		}
		protected virtual void OnDocumentIntervalInserted(int index) {
			RaiseDocumentIntervalInserted(index);
		}
		protected virtual void OnDocumentIntervalRemoved(int index) {
			RaiseDocumentIntervalRemoved(index);
		}
		protected internal virtual void RemoveAtCore(int index) {
			innerList.RemoveAt(index);
		}
		public void ForEach(Action<T> action) {
			ForEachCore(action);
		}
		protected internal virtual void ForEachCore(Action<T> action) {
			innerList.ForEach(action);
		}
		public int IndexOf(T item) {
			return IndexOfCore(item);
		}
		protected internal virtual int IndexOfCore(T item) {
			return innerList.IndexOf(item);
		}
		public int BinarySearch(IComparable<T> predicate) {
			return BinarySearchCore(predicate);
		}
		protected internal virtual int BinarySearchCore(IComparable<T> predicate) {
			return Algorithms.BinarySearch(innerList, predicate);
		}
		#region IDocumentModelStructureChangedListener Members
		void IDocumentModelStructureChangedListener.OnParagraphInserted(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, TableCell cell, bool isParagraphMerged, ParagraphIndex actualParagraphIndex, int historyNotificationId) {
			if (!Object.ReferenceEquals(this.PieceTable, pieceTable))
				return;
			for (int i = 0; i < Count; i++)
				this[i].OnParagraphInserted(pieceTable, sectionIndex, paragraphIndex, runIndex, cell, isParagraphMerged, actualParagraphIndex, historyNotificationId);
		}
		void IDocumentModelStructureChangedListener.OnParagraphRemoved(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
			if (!Object.ReferenceEquals(this.PieceTable, pieceTable))
				return;
			for (int i = 0; i < Count; i++)
				this[i].OnParagraphRemoved(pieceTable, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
		}
		void IDocumentModelStructureChangedListener.OnParagraphMerged(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex, RunIndex runIndex, int historyNotificationId) {
			if (!Object.ReferenceEquals(this.PieceTable, pieceTable))
				return;
			for (int i = 0; i < Count; i++)
				this[i].OnParagraphMerged(pieceTable, sectionIndex, paragraphIndex, runIndex, historyNotificationId);
		}
		void IDocumentModelStructureChangedListener.OnRunInserted(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length, int historyNotificationId) {
			if (!Object.ReferenceEquals(this.PieceTable, pieceTable))
				return;
			OnRunInserted(pieceTable, paragraphIndex, newRunIndex, length, historyNotificationId);
		}
		protected virtual void OnRunInserted(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length, int historyNotificationId) {
			for (int i = 0; i < Count; i++)
				this[i].OnRunInserted(pieceTable, paragraphIndex, newRunIndex, length, historyNotificationId);
		}
		void IDocumentModelStructureChangedListener.OnRunRemoved(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int length, int historyNotificationId) {
			if (!Object.ReferenceEquals(this.PieceTable, pieceTable))
				return;
			for (int i = 0; i < Count; i++)
				this[i].OnRunRemoved(pieceTable, paragraphIndex, runIndex, length, historyNotificationId);
		}
		void IDocumentModelStructureChangedListener.OnBeginMultipleRunSplit(PieceTable pieceTable) {
		}
		void IDocumentModelStructureChangedListener.OnEndMultipleRunSplit(PieceTable pieceTable) {
		}
		void IDocumentModelStructureChangedListener.OnRunSplit(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int splitOffset) {
			if (!Object.ReferenceEquals(this.PieceTable, pieceTable))
				return;
			for (int i = 0; i < Count; i++)
				this[i].OnRunSplit(pieceTable, paragraphIndex, runIndex, splitOffset);
		}
		void IDocumentModelStructureChangedListener.OnRunJoined(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex joinedRunIndex, int splitOffset, int tailRunLength) {
			if (!Object.ReferenceEquals(this.PieceTable, pieceTable))
				return;
			for (int i = 0; i < Count; i++)
				this[i].OnRunJoined(pieceTable, paragraphIndex, joinedRunIndex, splitOffset, tailRunLength);
		}
		void IDocumentModelStructureChangedListener.OnRunMerged(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
			if (!Object.ReferenceEquals(this.PieceTable, pieceTable))
				return;
			OnRunMerged(pieceTable, paragraphIndex, runIndex, deltaRunLength);
		}
		protected virtual void OnRunMerged(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
			for (int i = 0; i < Count; i++)
				this[i].OnRunMerged(pieceTable, paragraphIndex, runIndex, deltaRunLength);
		}
		void IDocumentModelStructureChangedListener.OnRunUnmerged(PieceTable pieceTable, ParagraphIndex paragraphIndex, RunIndex runIndex, int deltaRunLength) {
			if (!Object.ReferenceEquals(this.PieceTable, pieceTable))
				return;
			for (int i = 0; i < Count; i++)
				this[i].OnRunUnmerged(pieceTable, paragraphIndex, runIndex, deltaRunLength);
		}
		void IDocumentModelStructureChangedListener.OnFieldRemoved(PieceTable pieceTable, int fieldIndex) {
		}
		void IDocumentModelStructureChangedListener.OnFieldInserted(PieceTable pieceTable, int fieldIndex) {
		}
		#endregion
	}
	#endregion
	public delegate void DocumentIntervalEventHandler(object sender, DocumentIntervalEventArgs e);
	public class DocumentIntervalEventArgs : EventArgs {
		int index;
		public DocumentIntervalEventArgs(int index) {
			this.index = index;
		}
		public int Index { get { return index; } }
	}
	#region DocumentIntervalComparer<T>
	public class DocumentIntervalComparer<T> : IComparer<T> where T : DocumentInterval {
		#region IComparer<Bookmark> Members
		public int Compare(T x, T y) {
			if (x.Start == y.Start) {
				if (x.End == y.End)
					return 0;
				if (x.End < y.End)
					return -1;
				else
					return 1;
			}
			if (x.Start < y.Start)
				return -1;
			else
				return 1;
		}
		#endregion
	}
	#endregion
	#region CommentIntervalComparer<T>
	public class CommentIntervalComparer<T> : IComparer<T> where T :  Comment {
		#region IComparer<Bookmark> Members
		public int Compare(T x, T y) {
			if (x.End == y.End) {
				if (x.Start == y.Start) {
					if (y.ParentComment != null) {
						if (x.Index <= y.ParentComment.Index)
							return -1;
						else {
							if ((x.ParentComment == null) ||(x.ParentComment.Index > y.ParentComment.Index))
								return 1;
							if (x.ParentComment.Index == y.ParentComment.Index)
								return CompareCommentsDate(x.Date, y.Date);
							return -1;
						}
					}
					else {
						if ((x.ParentComment != null))
							if (x.ParentComment == y)
								return 1;
							else
								return -1;
						else {
							return CompareCommentsDate(x.Date, y.Date);
						}
					}
				}
				return (x.Start - y.Start);
			}
			return (x.End - y.End);
		}
		#endregion
		int CompareCommentsDate(DateTime xDate, DateTime yDate) {
			if (xDate <= yDate)
				return -1;
			else
				return 1;
		}
	}
	#endregion
	#region CommentRunIndexComparable
	public class CommentDocumentLogPositionComparable : IComparable<DocumentInterval> {
		readonly DocumentLogPosition logPosition;
		public CommentDocumentLogPositionComparable(DocumentLogPosition logPosition) {
			this.logPosition = logPosition;
		}
		#region IComparable<DocumentInterval> Members
		public int CompareTo(DocumentInterval interval) {
			if (interval.End < logPosition)
				return -1;
			else
				if (interval.End > logPosition)
					return 1;
				else
					return 0;
		}
		#endregion
	}
	#endregion
}
