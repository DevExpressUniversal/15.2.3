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
using DevExpress.Office.History;
namespace DevExpress.XtraRichEdit.Model {
	public interface IDocumentIntervalVisitor {
		void Visit(Bookmark bookmark);
		void Visit(RangePermission rangePermission);
		void Visit(Comment comment);
	}
	#region BookmarkBase (abstract class)
	public abstract class BookmarkBase : VisitableDocumentInterval {
		delegate List<int> GetDelegate();
		delegate void SetDelegate(List<int> value);
		class HistoryNotificationsAccessor {
			readonly GetDelegate getter;
			readonly SetDelegate setter;
			public HistoryNotificationsAccessor(GetDelegate getter, SetDelegate setter) {
				this.getter = getter;
				this.setter = setter;
			}
			public List<int> ChangeNotificatiodIds { get { return getter(); } set { setter(value); } }
			public bool ShouldChange(int historyNotificationId) {
				if (ChangeNotificatiodIds == null || historyNotificationId == NotificationIdGenerator.EmptyId)
					return false;
				else
					return ChangeNotificatiodIds.BinarySearch(historyNotificationId) >= 0;
			}
			public void AddChangeNotificationId(int historyNotificationId) {
				if (historyNotificationId == NotificationIdGenerator.EmptyId)
					return;
				if (ChangeNotificatiodIds == null)
					ChangeNotificatiodIds = new List<int>();
				int index = ChangeNotificatiodIds.BinarySearch(historyNotificationId);
				if (index >= 0)
					return;
				ChangeNotificatiodIds.Insert(~index, historyNotificationId);
			}
		}
		bool canExpand = true;
		bool forceUpdateInterval;
		List<int> startChangeNotificationIds;
		List<int> endChangeNotificationIds;
		protected BookmarkBase(PieceTable pieceTable, DocumentLogPosition start, DocumentLogPosition end) : this(pieceTable, start, end, false) { }
		protected BookmarkBase(PieceTable pieceTable, DocumentLogPosition start, DocumentLogPosition end, bool forceUpdateInterval)
			: base(pieceTable) {
			this.forceUpdateInterval = forceUpdateInterval;
			SetStartCore(start);
			SetEndCore(end);
		}
		public bool CanExpand { get { return canExpand; } set { canExpand = value; } }
		protected internal override void UpdateStartPosition() {
			if (!this.forceUpdateInterval)
				if (DocumentModel.IsUpdateLocked && DocumentModel.DeferredChanges.IsSetContentMode)
					return;
			base.UpdateStartPosition();
		}
		protected internal override void UpdateEndPosition() {
			if (!this.forceUpdateInterval)
				if (DocumentModel.IsUpdateLocked && DocumentModel.DeferredChanges.IsSetContentMode)
					return;
			base.UpdateEndPosition();
		}
		protected internal virtual void UpdateInterval() {
			base.UpdateStartPosition();
			base.UpdateEndPosition();
		}
		protected internal override void OnChangedCore() {
		}
		protected internal abstract void Delete(int index);
		protected internal virtual List<int> GetStartChangeNotificationIds() {
			return startChangeNotificationIds;
		}
		protected internal virtual void SetStartChangeNotificationIds(List<int> value) {
			startChangeNotificationIds = value;
		}
		protected internal virtual List<int> GetEndChangeNotificationIds() {
			return endChangeNotificationIds;
		}
		protected internal virtual void SetEndChangeNotificationIds(List<int> value) {
			endChangeNotificationIds = value;
		}
		protected internal override void OnRunInserted(ParagraphIndex paragraphIndex, RunIndex newRunIndex, int length, int historyNotificationId) {
			HistoryNotificationsAccessor startAccessor = new HistoryNotificationsAccessor(GetStartChangeNotificationIds, SetStartChangeNotificationIds);
			HistoryNotificationsAccessor endAccessor = new HistoryNotificationsAccessor(GetEndChangeNotificationIds, SetEndChangeNotificationIds);
			bool startChanged = OnRunInserted(Interval.Start, newRunIndex, length, startAccessor, historyNotificationId);
			bool endChanged = OnRunInserted(Interval.End, newRunIndex, length, endAccessor, historyNotificationId);
			OnChanged(startChanged, endChanged);
		}
		bool OnRunInserted(DocumentModelPosition position, RunIndex newRunIndex, int length, HistoryNotificationsAccessor accessor, int historyNotificationId) {
			if (newRunIndex < position.RunIndex || (newRunIndex == position.RunIndex && (position.RunOffset > 0 || accessor.ShouldChange(historyNotificationId)))) {
				position.RunIndex++;
				position.RunStartLogPosition += length;
				position.LogPosition += length;
				return true;
			}
			else
				return false;
		}
		protected internal override void OnRunRemoved(ParagraphIndex paragraphIndex, RunIndex runIndex, int length, int historyNotificationId) {
			HistoryNotificationsAccessor startAccessor = new HistoryNotificationsAccessor(GetStartChangeNotificationIds, SetStartChangeNotificationIds);
			HistoryNotificationsAccessor endAccessor = new HistoryNotificationsAccessor(GetEndChangeNotificationIds, SetEndChangeNotificationIds);
			bool startChanged = OnRunRemoved(Interval.Start, runIndex, length, startAccessor, historyNotificationId);
			bool endChanged = OnRunRemoved(Interval.End, runIndex, length, endAccessor, historyNotificationId);
			OnChanged(startChanged, endChanged);
		}
		bool OnRunRemoved(DocumentModelPosition pos, RunIndex runIndex, int length, HistoryNotificationsAccessor accessor, int historyNotificationId) {
			if (runIndex == pos.RunIndex) {
				if (pos.RunOffset > 0)
					pos.LogPosition -= pos.RunOffset;
				return true;
			}
			else if (runIndex < pos.RunIndex) {
				if (runIndex == (pos.RunIndex - 1) && pos.RunOffset == 0)
					accessor.AddChangeNotificationId(historyNotificationId);
				pos.RunIndex--;
				pos.RunStartLogPosition -= length;
				pos.LogPosition -= length;
				return true;
			}
			else
				return false;
		}
	}
	#endregion
	#region Bookmark
	public class Bookmark : BookmarkBase {
		#region static methods
		public static bool IsNameValid(string name) {
			return IsNameValidCore(name.Trim());
		}
		static bool IsNameValidCore(string name) {
			int count = name.Length;
			if (count > 0 && !Char.IsLetter(name[0]))
				return false;
			for (int i = 0; i < count; i++) {
				if (!Char.IsLetterOrDigit(name[i]) && name[i] != '_')
					return false;
			}
			return true;
		}
		#endregion
		#region Fields
		string name;
		#endregion
		public Bookmark(PieceTable pieceTable, DocumentLogPosition start, DocumentLogPosition end) : this(pieceTable, start, end, false) { }
		public Bookmark(PieceTable pieceTable, DocumentLogPosition start, DocumentLogPosition end, bool forceUpdateInterval) : base(pieceTable, start, end, forceUpdateInterval) { }
		public string Name { get { return name; } set { name = value; } }
		public bool IsHidden { get { return String.IsNullOrEmpty(name) ? true : name.StartsWith("_"); } }
		public override string ToString() {
			return name;
		}
		public override void Visit(IDocumentIntervalVisitor visitor) {
			visitor.Visit(this);
		}
		protected internal override void Delete(int index) {
			PieceTable.DeleteBookmarkCore(index);
		}
	}
	#endregion
	#region BookmarkBaseCollection<T> (abstract class)
	public abstract class BookmarkBaseCollection<T> : DocumentIntervalCollection<T> where T : BookmarkBase, IDocumentModelStructureChangedListener {
		protected BookmarkBaseCollection(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public void Clear() {
			InnerList.Clear();
		}
		public virtual void UpdateIntervals() {
			for (int i = 0; i < Count; i++)
				this[i].UpdateInterval();
		}
	}
	#endregion
	#region BookmarkCollection
	public class BookmarkCollection : BookmarkBaseCollection<Bookmark> {
		public BookmarkCollection(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public virtual Bookmark FindByName(string name) {
			for (int i = 0; i < Count; i++) {
				if (this[i].Name == name)
					return this[i];
			}
			return null;
		}
		public virtual int FindIndexByName(string name) {
			for (int i = 0; i < Count; i++) {
				if (this[i].Name == name)
					return i;
			}
			return 0;
		}
		public virtual Bookmark GetBookmarkFromSelection(Selection selection) {
			for (int i = 0; i < Count; i++)
				if ((selection.End < this[i].End) && (selection.Start >= this[i].Start))
					return this[i];
			return null;
		}
	}
	#endregion
	#region BookmarkNameComparer
	public class BookmarkNameComparer : IComparer<Bookmark> {
		#region IComparer<Bookmark> Members
		public int Compare(Bookmark x, Bookmark y) {
			return Comparer<string>.Default.Compare(x.Name, y.Name);
		}
		#endregion
	}
	#endregion
}
