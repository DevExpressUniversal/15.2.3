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

using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Model;
using System;
using System.Collections.Generic;
namespace DevExpress.Snap.Core.Native.Operations {
	class SnapBookmarkCopyOperation : BookmarkCopyOperationBase<SnapBookmark> {
		Dictionary<SnapBookmark, SnapBookmark> mapSourceToTargetBookmarks;
		List<SnapBookmark> copiedBookmarks;
		public SnapBookmarkCopyOperation() {
			mapSourceToTargetBookmarks = new Dictionary<SnapBookmark, SnapBookmark>();
			copiedBookmarks = new List<SnapBookmark>();
		}
		public override List<SnapBookmark> GetEntireBookmarks(PieceTable sourcePieceTable, DocumentLogPosition start, int length) {
			List<SnapBookmark> result = ((SnapPieceTable)sourcePieceTable).GetEntireSnapBookmarks(start, length);
			for (int i = result.Count - 1; i >= 0; i--) {
				if (!ShouldCopyBookmark(sourcePieceTable, result[i], start, length))
					result.RemoveAt(i);
			}
			return result;
		}
		protected virtual bool ShouldCopyBookmark(PieceTable sourcePieceTable, SnapBookmark snapBookmark, DocumentLogPosition start, int length) {			
			SnapTemplateInterval templateInterval = snapBookmark.TemplateInterval;
			if (templateInterval.PieceTable != sourcePieceTable)
				return true;
			if (templateInterval.NormalizedStart < start)
				return false;
			if (templateInterval.NormalizedEnd >= start + length)
				return false;
			return true;
		}
		public override void InsertBookmark(PieceTable targetPieceTable, DocumentLogPosition start, int length, SnapBookmark bookmark, int positionOffset) {
			SnapTemplateInterval templateInterval = bookmark.TemplateInterval;
			DocumentLogInterval templateLogInterval = GetNewTemplateInterval(bookmark.PieceTable, targetPieceTable, templateInterval, positionOffset);			
			SnapPieceTable targetSnapPieceTable = (SnapPieceTable)targetPieceTable;
			SnapBookmark copiedBookmark = targetSnapPieceTable.CreateSnapBookmarkCore(start, length, bookmark.FieldContext, templateLogInterval, targetSnapPieceTable, templateInterval.TemplateInfo);
			mapSourceToTargetBookmarks.Add(bookmark, copiedBookmark);
			copiedBookmarks.Add(copiedBookmark);
		}
		DocumentLogInterval GetNewTemplateInterval(PieceTable sourcePieceTable, PieceTable targetPieceTable, SnapTemplateInterval templateInterval, int positionOffset) {
			if(Object.ReferenceEquals(sourcePieceTable, targetPieceTable) || !Object.ReferenceEquals(targetPieceTable, templateInterval.PieceTable))
				return new DocumentLogInterval(templateInterval.Start + positionOffset, templateInterval.Length);
			else
				return new DocumentLogInterval(templateInterval.Start, templateInterval.Length);
		}
		public void FixBookmarkReferences() {
			foreach (SnapBookmark copiedBookmark in copiedBookmarks) {
				SnapTemplateInfo info = copiedBookmark.TemplateInterval.TemplateInfo;
				if (info.FirstGroupBookmark != null)
					info.FirstGroupBookmark = GetCopiedBookmarkSafe(info.FirstGroupBookmark);				
				if (info.LastGroupBookmark != null)
					info.LastGroupBookmark = GetCopiedBookmarkSafe(info.LastGroupBookmark);
				if (info.FirstListBookmark != null)
					info.FirstListBookmark = GetCopiedBookmarkSafe(info.FirstListBookmark);
				if (info.LastListBookmark != null)
					info.LastListBookmark = GetCopiedBookmarkSafe(info.LastListBookmark);
			}
		}
		SnapBookmark GetCopiedBookmarkSafe(SnapBookmark sourceBookmark) {
			if(IndicesAreCleared(sourceBookmark))
				return sourceBookmark;
			SnapBookmark result;
			if (mapSourceToTargetBookmarks.TryGetValue(sourceBookmark, out result))
				return result;
			else
				return null;
		}
		bool IndicesAreCleared(SnapBookmark sourceBookmark) {
			return IndicesAreCleared(sourceBookmark.Interval.Start) && IndicesAreCleared(sourceBookmark.Interval.End);
		}
		bool IndicesAreCleared(DocumentModelPosition pos) {
			return pos.RunIndex == RunIndex.Zero && pos.ParagraphIndex == ParagraphIndex.Zero;
		}
	}
}
