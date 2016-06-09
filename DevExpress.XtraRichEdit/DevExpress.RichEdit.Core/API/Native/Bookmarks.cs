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
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Localization;
namespace DevExpress.XtraRichEdit.API.Native {
	#region Bookmark
	[ComVisible(true)]
	public interface Bookmark {
		string Name { get; }
		DocumentRange Range { get; }
	}
	#endregion
	#region ReadOnlyBookmarkCollection
	[ComVisible(true)]
	public interface ReadOnlyBookmarkCollection : ISimpleCollection<Bookmark> {
		ReadOnlyBookmarkCollection Get(DocumentRange range);
	}
	#endregion
	#region BookmarkCollection
	[ComVisible(true)]
	public interface BookmarkCollection : ReadOnlyBookmarkCollection {
		Bookmark this[string name] { get; }
		Bookmark Create(DocumentRange range, string name);
		Bookmark Create(DocumentPosition start, int length, string name);
		void Select(Bookmark bookmark);
		void Remove(Bookmark bookmark);
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.API.Native.Implementation {
	using ModelBookmark = DevExpress.XtraRichEdit.Model.Bookmark;
	using ModelDocumentInterval = DevExpress.XtraRichEdit.Model.DocumentInterval;
	using ModelBookmarkCollection = DevExpress.XtraRichEdit.Model.BookmarkCollection;
	using DocumentModelPosition = DevExpress.XtraRichEdit.Model.DocumentModelPosition;
	using ModelDocumentIntervalEventHandler = DevExpress.XtraRichEdit.Model.DocumentIntervalEventHandler;
	using ModelDocumentIntervalEventArgs = DevExpress.XtraRichEdit.Model.DocumentIntervalEventArgs;
	using ModelPieceTable = DevExpress.XtraRichEdit.Model.PieceTable;
	using DevExpress.Office.Utils;
	#region NativeDocumentInterval<T> (abstract class)
	public abstract class NativeDocumentInterval<T> where T : ModelDocumentInterval {
		readonly NativeSubDocument document;
		readonly T documentInterval;
		bool isValid = true;
		protected NativeDocumentInterval(NativeSubDocument document, T documentInterval) {
			Guard.ArgumentNotNull(document, "document");
			this.document = document;
			Guard.ArgumentNotNull(documentInterval, "documentInterval");
			this.documentInterval = documentInterval;
		}
		public bool IsValid { get { return isValid; } set { isValid = value; } }
		protected internal NativeSubDocument Document { get { return document; } }
		protected T DocumentInterval { get { return documentInterval; } }
		protected DocumentRange GetRange() {
			DocumentModelPosition start = documentInterval.Interval.Start.Clone();
			DocumentModelPosition end = documentInterval.Interval.End.Clone();
			return new NativeDocumentRange(document, start, end);
		}
		protected abstract void CheckValid();
	}
	#endregion
	#region NativeBookmark
	public class NativeBookmark : NativeDocumentInterval<ModelBookmark>, Bookmark {
		public NativeBookmark(NativeSubDocument document, ModelBookmark bookmark)
			: base(document, bookmark) {
		}
		#region Bookmark Members
		public string Name {
			get {
				CheckValid();
				return DocumentInterval.Name;
			}
		}
		public DocumentRange Range {
			get {
				CheckValid();
				return GetRange();
			}
		}
		#endregion
		protected override void CheckValid() {
			if (!IsValid)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseDeletedBookmarkError);
		}
	}
	#endregion
	#region NativeBookmarkCollection
	public class NativeBookmarkCollection : List<NativeBookmark>, BookmarkCollection {
		readonly NativeSubDocument document;
		int lastInsertedBookmarkIndex = -1;
		public NativeBookmarkCollection(NativeSubDocument document) {
			Guard.ArgumentNotNull(document, "document");
			this.document = document;
			SubscribeBoormarksCollectionEvents();
		}
		ModelPieceTable PieceTable { get { return document.PieceTable; } }
		public NativeBookmark LastInsertedBookmark {
			get {
				if (lastInsertedBookmarkIndex < 0)
					return null;
				return this[lastInsertedBookmarkIndex];
			}
		}
		protected internal virtual void Invalidate() {
			UnsubscribeBoormarksCollectionEvents();
			Clear();
		}
		void SubscribeBoormarksCollectionEvents() {
			ModelBookmarkCollection bookmarks = PieceTable.Bookmarks;
			bookmarks.Inserted += new ModelDocumentIntervalEventHandler(OnBookmarkInserted);
			bookmarks.Removed += new ModelDocumentIntervalEventHandler(OnBookmarkRemoved);
		}
		void UnsubscribeBoormarksCollectionEvents() {
			ModelBookmarkCollection bookmarks = PieceTable.Bookmarks;
			bookmarks.Inserted -= new ModelDocumentIntervalEventHandler(OnBookmarkInserted);
			bookmarks.Removed -= new ModelDocumentIntervalEventHandler(OnBookmarkRemoved);
		}
		void OnBookmarkInserted(object sender, ModelDocumentIntervalEventArgs e) {
			ModelBookmark bookmark = PieceTable.Bookmarks[e.Index];
			this.Insert(e.Index, new NativeBookmark(document, bookmark));
			this.lastInsertedBookmarkIndex = e.Index;
		}
		void OnBookmarkRemoved(object sender, ModelDocumentIntervalEventArgs e) {
			this[e.Index].IsValid = false;
			this.RemoveAt(e.Index);
		}
		NativeBookmark FindByName(string name) {
			for (int i = 0; i < Count; i++) {
				NativeBookmark bookmark = this[i];
				if (StringExtensions.CompareInvariantCulture(bookmark.Name, name) == 0)
					return bookmark;
			}
			return null;
		}
		#region ISimpleCollection<Bookmark> Members
		Bookmark BookmarkCollection.this[string name] {
			get { return FindByName(name); }
		}
		Bookmark ISimpleCollection<Bookmark>.this[int index] {
			get { return this[index]; }
		}
		#endregion
		#region IEnumerable<Bookmark> Members
		IEnumerator<Bookmark> IEnumerable<Bookmark>.GetEnumerator() {
			return new EnumeratorAdapter<Bookmark, NativeBookmark>(this.GetEnumerator());
		}
		#endregion
		public Bookmark Create(DocumentRange range, string name) {
			return Create(range.Start, range.Length, name);
		}
		public Bookmark Create(DocumentPosition start, int length, string name) {
			document.CheckValid();
			Guard.ArgumentNotNull(start, "start");
			Guard.ArgumentNonNegative(length, "length");
			Guard.ArgumentNotNull(name, "name");
			if (document.DocumentModel.ActivePieceTable.Bookmarks.FindByName(name) != null)
				throw new InvalidOperationException(XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_DuplicateBookmark));
			document.CheckDocumentPosition(start);
			PieceTable.CreateBookmark(document.NormalizeLogPosition(start.LogPosition), length, name.Trim());
			return LastInsertedBookmark;
		}
		public void Select(Bookmark bookmark) {
			if (!Object.ReferenceEquals(PieceTable, document.DocumentModel.ActivePieceTable))
				throw new InvalidOperationException(XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_SelectBookmarkError));
			document.CheckValid();
			Guard.ArgumentNotNull(bookmark, "bookmark");
			document.CheckDocumentRange(bookmark.Range);
			NativeDocumentRange range = (NativeDocumentRange)bookmark.Range;
			document.SetSelectionCore(range.Start.LogPosition, range.End.LogPosition, false);
		}
		public void Remove(Bookmark bookmark) {
			document.CheckValid();
			Guard.ArgumentNotNull(bookmark, "bookmark");
			int index = 0;
			for (; index < Count; index++) {
				if (String.Equals(this[index].Name, bookmark.Name, StringComparison.Ordinal))
					break;
			}
			if (index == Count)
				return;
			PieceTable.DeleteBookmark(index);
		}
		public ReadOnlyBookmarkCollection Get(DocumentRange range) {
			document.CheckValid();
			document.CheckDocumentRange(range);
			NativeDocumentRange nativeRange = (NativeDocumentRange)range;
			DocumentModelPosition startRange = nativeRange.NormalizedStart.Position;
			DocumentModelPosition endRange = nativeRange.NormalizedEnd.Position;
			NativeBookmarkCollection result = new NativeBookmarkCollection(document);
			int count = PieceTable.Bookmarks.Count;
			for (int i = 0; i < count; i++) {
				DocumentModelPosition startBookmark = PieceTable.Bookmarks[i].Interval.Start;
				DocumentModelPosition endBookmark = PieceTable.Bookmarks[i].Interval.End;
				if ((startBookmark >= startRange) && (endBookmark <= endRange)) {
					NativeBookmark bookmark = new NativeBookmark(document, PieceTable.Bookmarks[i]);
					result.Add(bookmark);
				}
			}
			return result;
		}
	}
	#endregion
}
