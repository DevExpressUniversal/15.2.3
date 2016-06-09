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
using System.ComponentModel;
using System.Drawing;
using DevExpress.XtraPrinting.Native;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.DocumentView;
#if SL
using DevExpress.Xpf.Collections;
#endif
namespace DevExpress.XtraPrinting {
	public class PageList : IList<Page>, IEnumerable {
		IList<Page> list;
		readonly Document document;
		int raiseDocumentChangedLockCount;
		bool containsExternalPages;
		protected IList<Page> InnerList { 
			get { return this.list; }
		}
		internal Document Document { 
			get { return document; } 
		}
		internal bool ContainsExternalPages { 
			get { return containsExternalPages; } 
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PageListFirst")
#else
	Description("")
#endif
		]
		public Page First {
			get { return Count > 0 ? this[0] : null; }
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("PageListLast")]
#endif
		public Page Last {
			get { return Count > 0 ? this[Count - 1] : null; }
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("PageListItem")]
#endif
		public virtual Page this[int index] {
			get { 
				Page page = InnerList[index];
				if(page.Owner == null)
					page.Owner = this;
				return page;
			}
			set {
				throw new NotSupportedException();
			}
		}
		public virtual int Count {
			get { return InnerList.Count; }
		}
		public PageList(Document document, IList<Page> list) {
			this.document = document;
			this.list = list;
		}
		public PageList(Document document)
			: this(document, new IndexedDictionary<Page>()) {
		}
		public bool TryGetPageByIndex(int index, out Page page) {
			if(index >= 0 && index < Count) {
				page = this[index];
				return true;
			}
			page = null;
			return false;
		}
		public bool TryGetPageByID(Int32 id, out Page page) {
			foreach(Page item in list) {
				if(id == item.ID) {
					page = item;
					return true;
				}
			}
			page = null;
			return false;
		}
		public int IndexOf(Page page) {
			return InnerList.IndexOf(page);
		}
		internal void AddPageInternal(Page page) {
			try {
				SuppressRaiseDocumentChanged();
				Add(page);
			} finally {
				AllowRaiseDocumentChanged();
			}
		}
		public void AddRange(IEnumerable pages) {
			SuppressRaiseDocumentChanged();
			foreach(Page page in pages)
				Add(page);
			AllowRaiseDocumentChanged();
			RaiseDocumentChanged();
		}
		public void Add(Page page) {
			if(document.IsCreated)
				InsertPageBookmarks(GetPageIds(Count), page);
			ValidatePage(page);
			int index = InnerList.Count;
			InnerList.Add(page);
			OnInsertComplete(index, page);
		}
		void InsertPageBookmarks(int[] prevPages, Page page) {
			if(page.Document == null)
				return;
			IList<BookmarkNode> nodes = page.Document.BookmarkNodes.GetPageNodes(page);
			if(nodes.Count > 0)
				document.BookmarkNodes.InsertNodes(prevPages, nodes);
		}
		int[] GetPageIds(int length) {
			int[] ids = new int[length];
			for(int i = 0; i < length; i++)
				ids[i] = this[i].ID;
			return ids;
		}
		public void Insert(int index, Page page) {
			if(document.IsCreated)
				InsertPageBookmarks(GetPageIds(index), page);
			ValidatePage(page);
			InnerList.Insert(index, page);
			OnInsertComplete(index, page);
		}
		public bool Remove(Page page) {
			int index = InnerList.IndexOf(page);
			if(index < 0)
				throw new ArgumentException("page");
			RemoveCore(page, index);
			return true;
		}
		public void RemoveAt(int index) {
			RemoveCore(this[index], index);
		}
		void RemoveCore(Page page, int index) {
			OnRemove(index, page);
			InnerList.RemoveAt(index);
			OnRemoveComplete(index, page);
		}
		protected virtual void OnClear() {
			if(document != null)
				document.OnClearPages();
		}
		protected virtual void OnRemove(int index, object value) {
			this[index].Owner = null;
		}
		protected virtual void OnInsertComplete(int index, object value) {
			InvalidateIndices(index);
			document.PrintingSystem.OnPageInsertComplete(new PageInsertCompleteEventArgs(index));
			RaiseDocumentChanged();
		}
		protected virtual void OnRemoveComplete(int index, object value) {
			InvalidateIndices(index);
			RaiseDocumentChanged();
		}
		void InvalidateIndices(int fromIndex) {
			for(int i = fromIndex; i < Count; i++) {
				this[i].IvalidateIndex();
			}
		}
		protected void RaiseDocumentChanged() {
			if(raiseDocumentChangedLockCount != 0)
				return;
			document.SetModified(true);
			document.CanChangePageSettings = false;
			document.OnContentChanged();
		}
		void ValidatePage(Page page) {
			if(page == null)
				throw new ArgumentNullException("page");
			if(InnerList.Contains(page)) {
				SuppressRaiseDocumentChanged();
				Remove(page);
				AllowRaiseDocumentChanged();
			}
			if(page.Owner != null) {
				containsExternalPages = true;
			}
			page.Owner = this;
		}
		void AllowRaiseDocumentChanged() {
			raiseDocumentChangedLockCount--;
		}
		void SuppressRaiseDocumentChanged() {
			raiseDocumentChangedLockCount++;
		}
		public Page[] ToArray() {
			Page[] pages = new Page[InnerList.Count];
			InnerList.CopyTo(pages, 0);
			return pages;
		}
		public void Clear() {
			this.OnClear();
			this.InnerList.Clear();
		}		
		public IEnumerator GetEnumerator() {
			return InnerList.GetEnumerator();
		}
		public bool Contains(Page item) {
			return InnerList.Contains(item);
		}
		public void CopyTo(Page[] array, int arrayIndex) {
			InnerList.CopyTo(array, arrayIndex);
		}
		#region IList<Page> Members
		bool ICollection<Page>.IsReadOnly {
			get { return InnerList.IsReadOnly; }
		}
		IEnumerator<Page> IEnumerable<Page>.GetEnumerator() {
			return InnerList.GetEnumerator();
		}
		#endregion
	}
}
