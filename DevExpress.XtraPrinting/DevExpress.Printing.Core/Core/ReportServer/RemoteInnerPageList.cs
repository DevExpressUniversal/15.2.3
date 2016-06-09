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
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using DevExpress.DocumentView;
using DevExpress.ReportServer.Printing.Services;
using DevExpress.XtraPrinting;
namespace DevExpress.ReportServer.Printing {
	internal class RemoteInnerPageList : IList<Page>, IDisposable {
		#region fields and properties
		readonly IPageListService pageListService;
		bool IsDisposed { get; set; }
		#endregion
		#region events
		public event EventHandler<EventArgs> PageCountChanged;
		void RaisePageCountChanged() {
			if(PageCountChanged != null)
				PageCountChanged(this, EventArgs.Empty);
		}
		#endregion
		#region ctor
		public RemoteInnerPageList(IPageListService pageListService) {
			if(pageListService == null) throw new ArgumentNullException();
			this.pageListService = pageListService;
		}
		#endregion
		#region methods
		public void SetCount(int value) {
			if(IsDisposed)
				return;
			if(pageListService.EnlargeCapacity(value))
				RaisePageCountChanged();
		}
		public void Dispose() {
			Clear();
			IsDisposed = true;
		}
		#endregion
		public int IndexOf(Page item) {
			for(int i = 0; i < pageListService.Capacity; i++)
				if(pageListService[i] == item)
					return i;
			return -1;
		}
		void IList<Page>.Insert(int index, Page item) {
			throw new NotSupportedException();
		}
		void IList<Page>.RemoveAt(int index) {
			throw new NotSupportedException();
		}
		public Page this[int index] {
			get { return pageListService[index]; }
			set { throw new NotSupportedException(); }
		}
		void ICollection<Page>.Add(Page item) {
			throw new NotSupportedException();
		}
		public void Clear() {
			pageListService.Clear();
		}
		public bool Contains(Page item) {
			foreach(var page in pageListService) {
				if(page.Equals(item))
					return true;
			}
			return false;
		}
		public void CopyTo(Page[] array, int arrayIndex) {
			for(int i = 0; i < pageListService.Capacity; i++)
				array.SetValue(pageListService[i], i + arrayIndex);
		}
		public int Count { get { return pageListService.Capacity; } }
		bool ICollection<Page>.IsReadOnly {
			get { throw new NotImplementedException(); }
		}
		bool ICollection<Page>.Remove(Page item) {
			throw new NotSupportedException();
		}
		IEnumerator<Page> IEnumerable<Page>.GetEnumerator() {
			var enumerator = GetEnumerator();
			while(enumerator.MoveNext()) {
				yield return (Page)enumerator.Current;
			}
		}
		public IEnumerator GetEnumerator() {
			return pageListService.GetEnumerator();
		}
		internal void ReplaceCachedPage(int i, XtraPrinting.Page page) {
			pageListService[i] = page;
		}
	}
}
