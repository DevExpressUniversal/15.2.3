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

using System.Collections.Generic;
using DevExpress.Snap.Core.Native;
using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.Snap.Core.UI.Template {
	public class IntervalCalculatorBase {
		static protected RunInfo GetFieldSelectionInterval(SnapPieceTable pieceTable, SnapBookmark contentBookmark) {
			DocumentLogPosition templateStart = contentBookmark.TemplateInterval.Start;
			DocumentModelPosition position = PositionConverter.ToDocumentModelPosition(pieceTable, templateStart);
			Field field = pieceTable.FindNonTemplateFieldByRunIndex(position.RunIndex);			
			if (field == null)
				return null;									
			return pieceTable.GetFieldRunInfo(field);
		}
		static protected RunInfo GetContentInterval(SnapPieceTable pieceTable, SnapBookmark from, SnapBookmark to) {
			Guard.ArgumentNotNull(from, "from");
			Guard.ArgumentNotNull(to, "to");
			DocumentLogPosition start = from.Interval.Start.LogPosition;
			DocumentLogPosition end = to.Interval.End.LogPosition - 1;
			if (end < start)
				return null;
			return pieceTable.GetRunInfo(start, end);
		}
	}
	public class TemplateDecoratorInfoCalculator : IntervalCalculatorBase {
		public virtual ITemplateDecoratorInfo[] GetDecorators(SnapSelectionInfo selectionInfo) {
			return GetDecorators(selectionInfo.EnteredBookmarks.ToArray());
		}
		public virtual ITemplateDecoratorInfo[] GetDecorators(EnteredBookmarkInfo[] enteredBookmarks) {
			List<EnteredBookmarkInfo> existingEnteredBookmarks = GetExistingBookmark(enteredBookmarks);
			return GetDecorators(existingEnteredBookmarks);
		}
		List<EnteredBookmarkInfo> GetExistingBookmark(EnteredBookmarkInfo[] enteredBookmarks) {
			List<EnteredBookmarkInfo> result = new List<EnteredBookmarkInfo>();
			if (enteredBookmarks == null || enteredBookmarks.Length == 0)
				return result;
			foreach (EnteredBookmarkInfo info in enteredBookmarks) {
				if (!info.Bookmark.Deleted)
					result.Add(info);
			}
			return result;
		}
		ITemplateDecoratorInfo[] GetDecorators(List<EnteredBookmarkInfo> enteredBookmarks) {
			Guard.ArgumentNotNull(enteredBookmarks, "enteredBookmarks");
			int enteredBookmarksCount = enteredBookmarks.Count;
			if (enteredBookmarksCount == 0)
				return null;
			return new ITemplateDecoratorInfo[] { CreateSingleItemDecorator(enteredBookmarks[0]) };
		}
		ITemplateDecoratorInfo CreateSingleItemDecorator(EnteredBookmarkInfo enteredBookmarkInfo) {
			SnapBookmark bookmark = enteredBookmarkInfo.Bookmark;
			SnapPieceTable pieceTable = bookmark.PieceTable;
			return CreateTemplateDecoratorCore(pieceTable, bookmark, bookmark, (TemplateDecoratorType)bookmark.TemplateInterval.TemplateInfo.TemplateType);
		}
		ITemplateDecoratorInfo CreateInnermostDecorator(List<EnteredBookmarkInfo> enteredBookmarks) {
			Debug.Assert(enteredBookmarks.Count > 0);
			EnteredBookmarkInfo bookmarkInfo = enteredBookmarks[0];
			SnapBookmark bookmark = bookmarkInfo.Bookmark;
			return CreateTemplateDecoratorCore(bookmark.PieceTable, bookmark, bookmark, (TemplateDecoratorType)bookmark.TemplateInterval.TemplateInfo.TemplateType);
		}
		ITemplateDecoratorInfo CreateTemplateDecoratorCore(SnapPieceTable pieceTable, SnapBookmark from, SnapBookmark to, TemplateDecoratorType decoratorType) {
			if (from == null || to == null)
				return null;
			RunInfo contentInterval = GetContentInterval(pieceTable, from, to);
			return TemplateDecoratorInfoSingleItem.Create(pieceTable, contentInterval, decoratorType);
		}
	}
}
