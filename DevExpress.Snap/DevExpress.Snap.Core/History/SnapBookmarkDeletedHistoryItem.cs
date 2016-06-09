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
using DevExpress.Snap.Core.Native;
namespace DevExpress.Snap.Core.History {
	#region SnapBookmarkDeletedHistoryItem
	public class SnapBookmarkDeletedHistoryItem : DeleteBookmarkBaseHistoryItem<SnapBookmark> {
		public SnapBookmarkDeletedHistoryItem(SnapPieceTable pieceTable)
			: base(pieceTable) {
		}
		protected new SnapPieceTable PieceTable { get { return (SnapPieceTable)base.PieceTable; } }
		protected override void RedoCore() {
			SnapBookmark deletedBookmark = PieceTable.SnapBookmarks[DeletedBookmarkIndex];
			int[] childBookmarkIndices = PieceTable.GetChildSnapBookmarkIndexes(deletedBookmark);
			int childrenCount = childBookmarkIndices.Length;
			SnapBookmark newParent = deletedBookmark.Parent;
			deletedBookmark.Deleted = true;
			for (int i = 0; i < childrenCount; i++)
				PieceTable.SnapBookmarks[childBookmarkIndices[i]].Parent = newParent;
			base.RedoCore();
		}
		protected override void UndoCore() {
			base.UndoCore();
			SnapBookmark deletedBookmark = PieceTable.SnapBookmarks[DeletedBookmarkIndex];
			deletedBookmark.Deleted = false;
			int[] childBookmarkIndices = PieceTable.GetChildSnapBookmarkIndexes(deletedBookmark);
			int childrenCount = childBookmarkIndices.Length;
			for (int i = 0; i < childrenCount; i++) {
				SnapBookmark currentBookmark = PieceTable.SnapBookmarks[childBookmarkIndices[i]];
				if (!Object.ReferenceEquals(currentBookmark, deletedBookmark))
					currentBookmark.Parent = deletedBookmark;
			}
		}
		protected internal override BookmarkBaseCollection<SnapBookmark> GetBookmarkCollection() {
			return PieceTable.SnapBookmarks;
		}
	}
	#endregion
}
