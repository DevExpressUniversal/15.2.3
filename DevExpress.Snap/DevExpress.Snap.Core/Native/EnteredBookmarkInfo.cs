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
using DevExpress.Snap.Core.History;
using DevExpress.Utils;
using DevExpress.Office;
namespace DevExpress.Snap.Core.Native {
	public class EnteredBookmarkInfo : IDisposable, ICloneable<EnteredBookmarkInfo>, ISupportsCopyFrom<EnteredBookmarkInfo> {
		SnapBookmark bookmark;
		ModificationTracker modificationTracker;
		EnteredBookmarkInfo() { }
		public EnteredBookmarkInfo(SnapBookmark bookmark) {
			Guard.ArgumentNotNull(bookmark, "bookmark");
			this.bookmark = bookmark;
			this.modificationTracker = ((SnapDocumentHistory)bookmark.DocumentModel.History).CreateModificationTracker();
		}
		public SnapBookmark Bookmark { get { return bookmark; } }
		public bool Modified { get { return modificationTracker.Modified; } }
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (modificationTracker != null) {
					modificationTracker.Dispose();
					modificationTracker = null;
				}
				bookmark = null;
			}
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~EnteredBookmarkInfo() {
			Dispose(false);
		}
		public EnteredBookmarkInfo Clone() {
			EnteredBookmarkInfo clone = new EnteredBookmarkInfo();
			clone.CopyFrom(this);
			return clone;
		}
		public void CopyFrom(EnteredBookmarkInfo value) {
			this.bookmark = value.bookmark;
			this.modificationTracker = value.modificationTracker.Clone();
		}
	}
}
