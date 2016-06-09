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
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.Snap.Core.Fields;
using System.Diagnostics;
namespace DevExpress.Snap.Core.Native {
	public class SnapBookmarkController {
		readonly SnapPieceTable pieceTable;
		public SnapBookmarkController(SnapPieceTable pieceTable) {
			this.pieceTable = pieceTable;
		}
		public int FindInnermostTemplateBookmarkIndexByStartPosition(DocumentLogPosition position) {
			SnapBookmarkCollection bookmarks = pieceTable.SnapBookmarks;
			int index = bookmarks.BinarySearch(new BookmarkStartAndLogPositionComparable(position));
			if (index < 0)
				return index;
			int count = bookmarks.Count;
			for (int i = index + 1; i < count; i++) {
				SnapBookmark bookmark = bookmarks[i];
				if (bookmark.NormalizedStart > position)
					return i - 1;
			}
			return count - 1;
		}
		public SnapBookmark FindInnermostTemplateBookmarkByPosition(DocumentLogPosition position) {
			SnapBookmarkCollection bookmarks = pieceTable.SnapBookmarks;
			int count = bookmarks.Count;
			if (count == 0)
				return null;
			int index = bookmarks.BinarySearch(new BookmarkAndLogPositionComparable(position));
			Debug.Assert(index < 0);
			index = ~index;
			if (index == 0)
				return null;
			SnapBookmark bookmark = bookmarks[index - 1];
			while (bookmark != null) {
				if (bookmark.NormalizedStart <= position && position <= bookmark.NormalizedEnd)
					break;
				bookmark = bookmark.Parent;
			}
			return bookmark;
		}
		public SnapBookmark FindInnermostTemplateBookmarkByPosition(RunIndex runIndex) {
			SnapBookmarkCollection bookmarks = pieceTable.SnapBookmarks;
			int count = bookmarks.Count;
			if (count == 0)
				return null;
			int index = bookmarks.BinarySearch(new BookmarkAndRunIndexComparable(runIndex));
			Debug.Assert(index < 0);
			index = ~index;
			if (index == 0)
				return null;
			SnapBookmark bookmark = bookmarks[index - 1];
			while (bookmark != null) {
				RunIndex start = PositionConverter.ToDocumentModelPosition(pieceTable, bookmark.NormalizedStart).RunIndex;
				if (start <= runIndex) {
					RunIndex end = PositionConverter.ToDocumentModelPosition(pieceTable, bookmark.NormalizedEnd).RunIndex;
					if (runIndex <= end)
						break;
				}
				bookmark = bookmark.Parent;
			}
			return bookmark;
		}
		public SnapBookmark FindInnermostTemplateBookmarkByTableCell(TableCell cell) {
			RunInfo info = pieceTable.GetRunInfoByTableCell(cell);
			return FindInnermostTemplateBookmarkByPosition(info.Start.RunIndex);
		}
		public bool IsTableCellLastInTemplateBookmark(TableCell cell) {
			TableCell next = cell.Next;
			if (next == null)
				return true;
			return !Object.ReferenceEquals(FindInnermostTemplateBookmarkByTableCell(cell), FindInnermostTemplateBookmarkByTableCell(next));
		}
		internal SnapBookmark GetHeaderBookmark(SnapBookmark bookmark) {
			SnapFieldCalculatorService parser = new SnapFieldCalculatorService();
			SnapPieceTable pieceTable = bookmark.PieceTable;
			DocumentModelPosition start = PositionConverter.ToDocumentModelPosition(pieceTable, bookmark.TemplateInterval.Start);
			Field field = bookmark.PieceTable.FindNonTemplateFieldByRunIndex(start.RunIndex);
			SNListField displayListField = parser.ParseField(bookmark.PieceTable, field) as SNListField;
			if (displayListField == null || displayListField.ListHeaderTemplateInterval == null)
				return null;
			SnapBookmark result = FindBookmarkByTemplateInterval(pieceTable.SnapBookmarks, displayListField.ListHeaderTemplateInterval);
			if (!Object.ReferenceEquals(result, bookmark))
				return result;
			return null;
		}
		public SnapBookmark FindBookmarkByTemplateInterval(SnapBookmarkCollection bookmarks, DocumentLogInterval interval) {
			foreach (SnapBookmark bookmark in bookmarks)
				if (bookmark.TemplateInterval.Start == interval.Start && bookmark.TemplateInterval.Length == interval.Length)
					return bookmark;
			return null;
		}
		internal SnapBookmark GetFooterBookmark(SnapBookmark bookmark) {
			SnapFieldCalculatorService parser = new SnapFieldCalculatorService();
			SnapPieceTable pieceTable = bookmark.PieceTable;
			DocumentModelPosition start = PositionConverter.ToDocumentModelPosition(pieceTable, bookmark.TemplateInterval.Start);
			Field field = bookmark.PieceTable.FindNonTemplateFieldByRunIndex(start.RunIndex);
			SNListField displayListField = parser.ParseField(bookmark.PieceTable, field) as SNListField;
			if (displayListField == null || displayListField.ListFooterTemplateInterval == null)
				return null;
			SnapBookmark result = FindBookmarkByTemplateInterval(pieceTable.SnapBookmarks, displayListField.ListFooterTemplateInterval);
			if (!Object.ReferenceEquals(result, bookmark))
				return result;
			return null;
		}
	}
	public class BookmarkAndRunIndexComparable : IComparable<SnapBookmark> {
		readonly RunIndex runIndex;
		public BookmarkAndRunIndexComparable(RunIndex runIndex) {
			this.runIndex = runIndex;
		}
		public int CompareTo(SnapBookmark bookmark) {
			DocumentModelPosition position = PositionConverter.ToDocumentModelPosition(bookmark.PieceTable, bookmark.NormalizedStart);
			if (position.RunIndex > runIndex)
				return 1;
			return -1;
		}
	}
	public class BookmarkStartAndLogPositionComparable : IComparable<SnapBookmark> {
		readonly DocumentLogPosition position;
		public BookmarkStartAndLogPositionComparable(DocumentLogPosition position) {
			this.position = position;
		}
		public int CompareTo(SnapBookmark bookmark) {
			return bookmark.NormalizedStart - position;
		}
	}
	public class BookmarkAndLogPositionComparable : IComparable<SnapBookmark> {
		readonly DocumentLogPosition position;
		public BookmarkAndLogPositionComparable(DocumentLogPosition position) {
			this.position = position;
		}
		public int CompareTo(SnapBookmark bookmark) {
			if (bookmark.NormalizedStart > position)
				return 1;
			return -1;
		}
	}
}
