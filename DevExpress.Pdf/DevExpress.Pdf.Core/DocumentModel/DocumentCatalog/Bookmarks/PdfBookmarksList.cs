#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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
using DevExpress.Pdf.Localization;
namespace DevExpress.Pdf.Native {
	public class PdfBookmarkList : IList<PdfBookmark>, IPdfBookmarkParent {
		readonly List<PdfBookmark> bookmarks = new List<PdfBookmark>();
		readonly IPdfBookmarkParent parent;
		public static PdfOutlines CreateOutlines(IList<PdfBookmark> bookmarks) {
			return (bookmarks == null || bookmarks.Count == 0) ? null : new PdfOutlines(bookmarks);
		}
		public PdfDocumentCatalog DocumentCatalog { get { return parent.DocumentCatalog; } }
		public PdfBookmarkList(IPdfBookmarkParent parent) {
			this.parent = parent;
		}
		public PdfBookmarkList(IPdfBookmarkParent parent, IEnumerable<PdfBookmark> items) : this(parent) {
			if (items != null)
				foreach (PdfBookmark bookmark in items)
					bookmarks.Add(PrepareAndValidateItem(bookmark));
		}
		public PdfBookmarkList(IPdfBookmarkParent parent, PdfOutlineItem item) : this(parent) {
			if (item != null)
				for (PdfOutline outline = item.First; outline != null; outline = outline.Next)
					bookmarks.Add(new PdfBookmark(parent, outline));
		}
		public void Invalidate() {
			parent.Invalidate();
		}
		PdfBookmark PrepareAndValidateItem(PdfBookmark item) {
			if (item == null)
				throw new ArgumentNullException(PdfCoreLocalizer.GetString(PdfCoreStringId.MsgIncorrectBookmarkListValue));
			PdfDestination destination = item.Destination;
			PdfDocumentCatalog documentCatalog = DocumentCatalog;
			if (destination != null && documentCatalog != null) {
				PdfPage page = destination.Page;
				if (page != null && page.DocumentCatalog != documentCatalog)
					throw new ArgumentException(PdfCoreLocalizer.GetString(PdfCoreStringId.MsgIncorrectDestinationPage));
			}
			item.Parent = parent;
			Invalidate();
			return item;
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return bookmarks.GetEnumerator();
		}
		IEnumerator<PdfBookmark> IEnumerable<PdfBookmark>.GetEnumerator() {
			return bookmarks.GetEnumerator();
		}
		#region ICollection implementation
		bool ICollection<PdfBookmark>.IsReadOnly { get { return false; } }
		int ICollection<PdfBookmark>.Count { get { return bookmarks.Count; } }
		bool ICollection<PdfBookmark>.Contains(PdfBookmark item) {
			return bookmarks.Contains(item);
		}
		void ICollection<PdfBookmark>.Add(PdfBookmark item) {
			bookmarks.Add(PrepareAndValidateItem(item));
		}
		bool ICollection<PdfBookmark>.Remove(PdfBookmark item) {
			bool removed = bookmarks.Remove(item);
			if (removed)
				Invalidate();
			return removed;
		}
		void ICollection<PdfBookmark>.Clear() {
			if (bookmarks.Count > 0) {
				bookmarks.Clear();
				Invalidate();
			}
		}
		void ICollection<PdfBookmark>.CopyTo(PdfBookmark[] array, int arrayIndex) {
			bookmarks.CopyTo(array, arrayIndex);
		}
		#endregion
		#region IList implementation
		PdfBookmark IList<PdfBookmark>.this[int index] {
			get { return bookmarks[index]; }
			set { bookmarks[index] = PrepareAndValidateItem(value); }
		}
		int IList<PdfBookmark>.IndexOf(PdfBookmark item) {
			return bookmarks.IndexOf(item);
		}
		void IList<PdfBookmark>.Insert(int index, PdfBookmark item) {
			bookmarks.Insert(index, PrepareAndValidateItem(item));
		}
		void IList<PdfBookmark>.RemoveAt(int index) {
			bookmarks.RemoveAt(index);
		}
		#endregion
	}
}
