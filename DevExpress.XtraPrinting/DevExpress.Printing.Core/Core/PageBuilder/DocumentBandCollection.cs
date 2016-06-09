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
using System.Collections.Generic;
using System.Collections;
using System.Drawing;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Native.Enumerators;
using System.Collections.ObjectModel;
#if SL
using DevExpress.Xpf.Collections;
#else
#endif
namespace DevExpress.XtraPrinting.Native {
#if DEBUGTEST
	[System.Diagnostics.DebuggerDisplay(@"\{{GetType().FullName,nq}, Count = {Items.Count}}")]
	[System.Diagnostics.DebuggerTypeProxy(typeof(DebuggerHelpers.DocumentBandCollectionDebuggerTypeProxy))]
#endif
	public class DocumentBandCollection : Collection<DocumentBand>, IListWrapper<DocumentBand> {
		#region inner classes
		class DetailBreakEnumerator : IIndexedEnumerator {
			IIndexedEnumerator en;
			public object Current { get { return en.Current; } }
			public int RealIndex { get { return en.RealIndex; } }
			public DetailBreakEnumerator(IIndexedEnumerator en) {
				this.en = en;
			}
			bool IEnumerator.MoveNext() {
				return en.MoveNext() && !((DocumentBand)Current).IsKindOf(DocumentBandKind.Detail);
			}
			void IEnumerator.Reset() {
				en.Reset();
			}
		}
		class IndexedEnumerator : IIndexedEnumerator {
			IEnumerator en;
			int realIndex;
			public object Current { get { return en.Current; } }
			public int RealIndex { get { return realIndex; } }
			public IndexedEnumerator(IEnumerator en) {
				this.en = en;
				ResetIndex();
			}
			bool IEnumerator.MoveNext() {
				realIndex++;
				return en.MoveNext();
			}
			void IEnumerator.Reset() {
				ResetIndex();
				en.Reset();
			}
			void ResetIndex() {
				realIndex = -1;
			}
		}
		#endregion
		DocumentBand owner;
		public DocumentBandCollection(DocumentBand owner) : base(new List<DocumentBand>()) {
			this.owner = owner;
		}
		public DocumentBand Last { get { return Count > 0 ? this[Count - 1] : null; } }
		public DocumentBand First { get { return Count > 0 ? this[0] : null; } }
		public DocumentBand this[DocumentBandKind kind] {
			get {
				return FindBand(kind, band => band.IsKindOf(kind));
			}
		}
		public virtual float TotalHeight {
			get {
				float totalHeight = 0;
				foreach(DocumentBand band in this)
					if(!band.IsMarginBand)
						totalHeight += band.TotalHeight;
				return totalHeight;
			}
		}
		public DocumentBand FindBand(DocumentBandKind kind, Predicate<DocumentBand> predicate) {
			int bandIndex;
			return FindBandCore(out bandIndex, kind, predicate);
		}
		IIndexedEnumerator CreateEnumerator(DocumentBandKind kind) {
			if(kind.IsFooter())
				return new DetailBreakEnumerator(new ReversedEnumerator(this));
			else if(kind.IsHeader())
				return new DetailBreakEnumerator(GetIndexedEnumerator());
			else
				return GetIndexedEnumerator();
		}
		IndexedEnumerator GetIndexedEnumerator() {
			return new IndexedEnumerator(GetEnumerator());
		}
		DocumentBand FindBandCore(out int bandIndex, DocumentBandKind kind, Predicate<DocumentBand> predicate) {
			bandIndex = -1;
			IIndexedEnumerator en = CreateEnumerator(kind);
			while(en.MoveNext()) {
				DocumentBand band = (DocumentBand)en.Current;
				if(predicate(band)) {
					bandIndex = en.RealIndex;
					return band;
				}
			}
			return null;
		}
		public void Sort(IComparer<DocumentBand> comparer) {
			((List<DocumentBand>)Items).Sort(comparer);
		}
		public int FastIndexOf(DocumentBand band) {
			if(band == null)
				return -1;
			int bandIndex;
			FindBandCore(
				out bandIndex,
				band.Kind,
				item => object.ReferenceEquals(item, band)
			);
			return bandIndex;
		}
		protected override void InsertItem(int index, DocumentBand value) {
			if(value != null) {
				base.InsertItem(index, value);
				if(value.IsDetailBand)
					owner.HasDetailBands = true;
				if(owner.ShouldAssignParent)
					value.Parent = owner;
			}
		}
		#region IListWrapper<DocumentBand> Members
		int IListWrapper<DocumentBand>.IndexOf(DocumentBand item) {
			return FastIndexOf(item);
		}
		void IListWrapper<DocumentBand>.Insert(DocumentBand item, int index) {
			Insert(index, item);
		}
		#endregion
	}
}
