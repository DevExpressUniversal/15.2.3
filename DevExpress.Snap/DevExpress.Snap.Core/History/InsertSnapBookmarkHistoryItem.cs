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
using DevExpress.XtraRichEdit.Model.History;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.Snap.Core.Native.Data;
using DevExpress.Snap.Core.Native;
namespace DevExpress.Snap.Core.History {
	public class InsertSnapBookmarkHistoryItem : InsertBookmarkBaseHistoryItem<SnapBookmark> {
		public InsertSnapBookmarkHistoryItem(SnapPieceTable pieceTable)
			: base(pieceTable) {
		}
		public IFieldContext DataContext { get; set; }
		public DocumentLogInterval TemplateLogInterval { get; set; }
		public SnapTemplateInfo TemplateInfo { get; set; }
		public SnapPieceTable TemplatePieceTable { get; set; }
		protected new SnapPieceTable PieceTable { get { return (SnapPieceTable)base.PieceTable; } }		
		protected internal override SnapBookmark CreateBookmark(DocumentLogPosition start, DocumentLogPosition end) {
			SnapTemplateInterval templateInterval = new SnapTemplateInterval(TemplatePieceTable, TemplateLogInterval.Start, TemplateLogInterval.Start + TemplateLogInterval.Length, TemplateInfo);
			SnapBookmark bookmark = CreateSnapBookmarkCore(start, end, templateInterval);
			return bookmark;
		}
		protected virtual SnapBookmark CreateSnapBookmarkCore(DocumentLogPosition start, DocumentLogPosition end, SnapTemplateInterval templateInterval) {
			return new SnapBookmark(PieceTable, DataContext, start, end, templateInterval);
		}
		bool executing;
		public override void Execute() {
			executing = true;
			try {
				base.Execute();
			}
			finally {
				executing = false;
			}
			Bookmark.RegisterInterval(PieceTable.DocumentPositionManager);
		}
		protected override void RedoCore() {
			SnapBookmark parent = null;
			SnapBookmarkController controller = new SnapBookmarkController(PieceTable);
			IndexToInsert = controller.FindInnermostTemplateBookmarkIndexByStartPosition(Bookmark.NormalizedStart);
			SnapBookmarkCollection bookmarks = PieceTable.SnapBookmarks;
			if (IndexToInsert < 0) {
				IndexToInsert = ~IndexToInsert;
				if (IndexToInsert > 0) {
					parent = bookmarks[IndexToInsert - 1];
					while (parent != null) {
						if (parent.NormalizedStart <= Bookmark.NormalizedStart && parent.NormalizedEnd >= Bookmark.NormalizedEnd)
							break;
						parent = parent.Parent;
					}
				}
			}
			else {				
				int count = bookmarks.Count;
				for (; IndexToInsert < count;IndexToInsert++) {
					SnapBookmark nextBookmark = bookmarks[IndexToInsert];
					if (nextBookmark.NormalizedStart == Bookmark.NormalizedStart && nextBookmark.NormalizedEnd > Bookmark.NormalizedEnd) {
						parent = nextBookmark;
						break;
					}
					if (nextBookmark.NormalizedStart > Bookmark.NormalizedStart) {
						IndexToInsert--;
						parent = bookmarks[IndexToInsert].Parent;
						break;
					}						
				}
			}
			int[] childBookmarkIndices = PieceTable.GetChildSnapBookmarkIndexes(Bookmark);
			int childrenCount = childBookmarkIndices.Length;
			if (childrenCount > 0) {
				for (int i = 0; i < childrenCount; i++) {
					PieceTable.SnapBookmarks[childBookmarkIndices[i]].Parent = Bookmark;
				}
			}
			Bookmark.Parent = parent;
			Bookmark.Deleted = false;
			GetBookmarkCollection().Insert(IndexToInsert, Bookmark);
			if (!executing) {
				Bookmark.AttachInterval(PieceTable.DocumentPositionManager);
				PieceTable.CheckSnapBookmarksIntegrity();
			}
		}
		protected override void UndoCore() {
			int[] childBookmarkIndices = PieceTable.GetChildSnapBookmarkIndexes(Bookmark);
			int childrenCount = childBookmarkIndices.Length;
			SnapBookmark newParent = Bookmark.Parent;
			for (int i = 0; i < childrenCount; i++)
				PieceTable.SnapBookmarks[childBookmarkIndices[i]].Parent = newParent;
			Bookmark.Deleted = true;
			base.UndoCore();
			Bookmark.DetachInterval(PieceTable.DocumentPositionManager);
			PieceTable.CheckSnapBookmarksIntegrity();
		}
		protected internal override BookmarkBaseCollection<SnapBookmark> GetBookmarkCollection() {
			return PieceTable.SnapBookmarks;
		}		
	}
}
