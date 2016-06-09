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
	public class PdfPageList : IList<PdfPage> {
		class PdfDeferredPageList : PdfDeferredList<PdfPage> {
			public PdfDeferredPageList(PdfPageTreeNode source) : base(((IEnumerable<PdfPage>)source).GetEnumerator(), source.Count) { 
			}
			public PdfDeferredPageList() {
			}
			protected override PdfPage ParseObject(object value) {
				return (PdfPage)value;
			}
		}
		readonly PdfDeferredPageList pages;
		readonly PdfDocumentCatalog documentCatalog;
		int nodeObjectNumber = PdfObject.DirectObjectNumber;
		public PdfPage this[int index] {
			get {
				return pages[index];
			}
			set {
				if (value != null) {
					PdfPage page = null;
					if (index < Count)
						page = pages[index];
					pages[index] = ClonePage(value);
					if (page != null)
						DeletePage(page);
				}
			}
		}
		public int Count {
			get { return pages.Count; }
		}
		bool ICollection<PdfPage>.IsReadOnly {
			get { return false; }
		}
		public PdfPageList(PdfDocumentCatalog documentCatalog) {
			this.documentCatalog = documentCatalog;
			pages = new PdfDeferredPageList();
		}
		public PdfPageList(PdfPageTreeNode source, PdfDocumentCatalog documentCatalog) {
			this.documentCatalog = documentCatalog;
			pages = new PdfDeferredPageList(source);
		}
		public bool Contains(PdfPage item) {
			return pages.Contains(item);
		}
		public PdfPage Add(PdfPage item) {
			PdfPage page = null;
			if (item != null) {
				page = ClonePage(item);
				pages.Add(page);
			}
			return page;
		}
		public PdfPage Insert(int index, PdfPage item) {
			PdfPage page = null;
			if (item != null) {
				page = ClonePage(item);
				pages.Insert(index, page);
			}
			return page;
		}
		public void AppendDocument(PdfDocumentCatalog catalog) {
			pages.AddRange(documentCatalog.Objects.ClonePages(catalog.Pages, true));
		}
		public PdfPage AddNewPage(PdfPage item) {
			if (item != null)
				pages.Add(item);
			return item;
		}
		public PdfPage InsertNewPage(int index, PdfPage item) {
			if (item != null)
				pages.Insert(index, item);
			return item;
		}
		public void DeletePage(int pageNumber) {
			int pageCount = pages.Count;
			if (pageNumber < 1 || pageNumber > pageCount)
				throw new ArgumentOutOfRangeException("pageNumber", String.Format(PdfCoreLocalizer.GetString(PdfCoreStringId.MsgIncorrectPageNumber), pageCount));
			DeletePage(pages[pageNumber - 1]);
		}
		public PdfPageTreeNode GetPageNode(PdfObjectCollection objects, bool withPages) {
			return new PdfPageTreeNode(documentCatalog, null, null, 0, withPages ? pages : (IEnumerable<PdfPage>)new PdfPage[0]) { ObjectNumber = GetNodeNumber(objects) };
		}
		public PdfPage FindPage(int objectNumber) {
			foreach (PdfPage page in pages)
				if (page.ObjectNumber == objectNumber)
					return page;
			return null;
		}
		int GetNodeNumber(PdfObjectCollection objects) {
			if (nodeObjectNumber == PdfObject.DirectObjectNumber)
				nodeObjectNumber = ++objects.LastObjectNumber;
			return nodeObjectNumber;
		}
		PdfPage ClonePage(PdfPage page) {
			return documentCatalog.Objects.ClonePages(new PdfPage[] { page }, false)[0];
		}
		bool DeletePage(PdfPage page) {
			foreach (PdfPage p in pages)
				p.EnsureAnnotations();
			PdfNames names = documentCatalog.Names;
			if (names != null) {
				DoWithItems(names.PageNames, p => p == page);
				DoWithItems(names.PageDestinations, p => p != null && p.Page == page);
				DoWithItems(names.WebCaptureContentSetsIds, w => {
					DoWithItems((IList<PdfPage>)w.PageSet, p => p == page);
					return false;
				});
				DoWithItems(names.WebCaptureContentSetsUrls, w => {
					DoWithItems((IList<PdfPage>)w.PageSet, p => p == page);
					return false;
				});
			}
			DoWithItems(documentCatalog.Threads, item => CheckItem(item, page));
			DoWithItems(documentCatalog.Destinations, d => d != null && d.Page == page);
			PdfLogicalStructure logicalStructure = documentCatalog.LogicalStructure;
			if (logicalStructure != null) {
				logicalStructure.Resolve();
				DoWithItems(logicalStructure.Kids, item => CheckItem(item, page));
				DoWithItems(logicalStructure.Parents, item => CheckItem(item, page));
				DoWithItems(logicalStructure.Elements, item => CheckItem(item, page));
			}
			DeleteFromOutlines(documentCatalog.Outlines, page);
			DoWithItems(pages, p => {
				DoWithItems(p.Annotations, a => {
					PdfLinkAnnotation la = a as PdfLinkAnnotation;
					if (la != null) {
						PdfGoToAction gta = la.Action as PdfGoToAction;
						PdfDestination destination = la.Destination;
						if ((gta != null && gta.Destination != null && gta.Destination.Page == page) ||
							(destination != null && destination.Page == page))
							return true;
					}
					return a.Page == page;
				});
				return false;
			});
			PdfInteractiveForm acroForm = documentCatalog.AcroForm;
			if (acroForm != null) {
				DoWithItems(acroForm.Fields, f => CheckItem(f, page) || (f.Widget != null && f.Widget.Page == page));
				DoWithItems(acroForm.CalculationOrder, co => co != null && co.Widget != null && co.Widget.Page == page);
			}
			if (documentCatalog.OpenDestination != null && documentCatalog.OpenDestination.Page == page)
				documentCatalog.OpenDestination = null;
			PdfGoToAction gotoAction = documentCatalog.OpenAction as PdfGoToAction;
			if (gotoAction != null && gotoAction.Destination != null && gotoAction.Destination.Page == page)
				documentCatalog.OpenAction = null;
			if (page.Parent != null)
				page.Parent.RemovePage(page);
			return pages.Remove(page);
		}
		void DoWithItems<T>(IList<T> list, Func<T, bool> action) {
			if (list != null)
				for (int i = list.Count - 1; i >= 0; i--)
					if (action(list[i]))
						list.RemoveAt(i);
		}
		void DoWithItems<Tkey, Tvalue>(IDictionary<Tkey, Tvalue> dictionary, Func<Tvalue, bool> action) {
			if (dictionary != null)
				foreach (Tkey key in new List<Tkey>(dictionary.Keys))
					if (action(dictionary[key]))
						dictionary.Remove(key);
		}
		void DeleteFromOutlines(PdfOutlineItem item, PdfPage page) {
			if (item != null) {
				for (PdfOutline outline = item.First; outline != null; outline = outline.Next) {
					PdfDestination destination = outline.Destination;
					if (destination == null) {
						PdfGoToAction action = outline.Action as PdfGoToAction;
						if (action != null)
							destination = action.Destination;
					}
					if (destination != null && destination.Page == page) {
						PdfOutline previous = outline.Prev;
						PdfOutline next = outline.Next;
						if (previous != null)
							previous.Next = next;
						if (next != null)
							next.Prev = previous;
						if (item.First == outline)
							item.First = next;
						if (item.Last == outline)
							item.Last = previous;
					}
					DeleteFromOutlines(outline, page);
				}
				item.UpdateCount();
			}
		}
		bool CheckItem(PdfArticleThread articleThread, PdfPage page) {
			PdfBead firstBead = articleThread.FirstBead;
			if (firstBead.Page == page) {
				PdfBead actualFirstBead = null;
				for (PdfBead bead = firstBead.Next; bead != firstBead; bead = bead.Next)
					if (bead.Page != page) {
						actualFirstBead = bead;
						break;
					}
				if (actualFirstBead == null)
					return true;
				firstBead = actualFirstBead;
				articleThread.FirstBead = firstBead;
			}
			PdfBead previousBead = firstBead;
			for (PdfBead bead = previousBead.Next; bead != firstBead; bead = bead.Next) {
				if (bead.Page != page) {
					previousBead.Next = bead;
					bead.Previous = previousBead;
					previousBead = bead;
				}
			}
			firstBead.Previous = previousBead;
			previousBead.Next = firstBead;
			return false;
		}
		bool CheckItem(PdfInteractiveFormField element, PdfPage page) {
			DoWithItems(element.Kids, k => CheckItem(k, page));
			return (element.Widget != null && element.Widget.Page == page) || (element.Kids != null && element.Kids.Count == 0);
		}
		bool CheckItem(PdfLogicalStructureItem item, PdfPage page) {
			if (item.ContainingPage == page)
				return true;
			PdfLogicalStructureElement element = item as PdfLogicalStructureElement;
			if (element != null) {
				DoWithItems(element.Kids, kid => CheckItem(kid, page));
				for (PdfLogicalStructureElement parent = element.Parent as PdfLogicalStructureElement; parent != null; parent = parent.Parent as PdfLogicalStructureElement)
					if (parent.Page == page)
						return true;
			}
			return false;
		}
		bool CheckItem(PdfLogicalStructureElementList elements, PdfPage page) {
			DoWithItems(elements, item => CheckItem(item, page));
			return elements.Count == 0;
		}
		void IList<PdfPage>.Insert(int index, PdfPage item) {
			Insert(index, item);
		}
		int IList<PdfPage>.IndexOf(PdfPage item) {
			return pages.IndexOf(item);
		}
		void IList<PdfPage>.RemoveAt(int index) {
			DeletePage(index + 1);
		}
		void ICollection<PdfPage>.Add(PdfPage item) {
			Add(item);
		}
		void ICollection<PdfPage>.Clear() {
			while (pages.Count > 0)
				DeletePage(pages.Count);
		}
		void ICollection<PdfPage>.CopyTo(PdfPage[] array, int arrayIndex) {
			pages.CopyTo(array, arrayIndex);
		}
		bool ICollection<PdfPage>.Remove(PdfPage item) {
			return DeletePage(item);
		}
		IEnumerator<PdfPage> IEnumerable<PdfPage>.GetEnumerator() {
			return pages.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return pages.GetEnumerator();
		}
	}
}
