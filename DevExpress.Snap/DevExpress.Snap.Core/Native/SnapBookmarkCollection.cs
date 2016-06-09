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
using DevExpress.XtraRichEdit.Model;
using System.Collections;
namespace DevExpress.Snap.Core.Native {
	public class SnapBookmarkCollection : BookmarkBaseCollection<SnapBookmark>, IList<SnapBookmark> {
		public SnapBookmarkCollection(SnapPieceTable pieceTable)
			: base(pieceTable) {
		}
		public SnapBookmark First { get { return Count > 0 ? InnerList[0] : null; } }
		public SnapBookmark Last { get { return Count > 0 ? InnerList[Count - 1] : null; } }
		SnapBookmark IList<SnapBookmark>.this[int index] {
			get {
				return InnerList[index];
			}
			set {
				InnerList[index] = value;
			}
		}
		void ICollection<SnapBookmark>.Add(SnapBookmark item) {
			Add(item);
		}
		bool ICollection<SnapBookmark>.Contains(SnapBookmark item) {
			return InnerList.Contains(item);
		}
		void ICollection<SnapBookmark>.CopyTo(SnapBookmark[] array, int arrayIndex) {
			InnerList.CopyTo(array, arrayIndex);
		}
		bool ICollection<SnapBookmark>.Remove(SnapBookmark item) {
			return InnerList.Remove(item);
		}
		bool ICollection<SnapBookmark>.IsReadOnly {
			get { return false; }
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return InnerList.GetEnumerator();
		}
		IEnumerator<SnapBookmark> IEnumerable<SnapBookmark>.GetEnumerator() {
			return InnerList.GetEnumerator();
		}
	}
}
