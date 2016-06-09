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
using System.Collections.Generic;
using DevExpress.Office;
using DevExpress.Office.Utils;
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	#region NativeChartCollectionBase (abstract class)
	abstract partial class NativeChartCollectionBase<T, N, M> : NativeDrawingCollectionBase<T, N, M>, ISimpleCollection<T>
		where N : T
		where M : class {
		protected NativeChartCollectionBase(UndoableCollection<M> modelCollection)
			: base(modelCollection) {
		}
		#region ISimpleCollection<T> Members
		public IEnumerator<T> GetEnumerator() {
			CheckValid();
			return new EnumeratorAdapter<T, N>(InnerList.GetEnumerator());
		}
		IEnumerator IEnumerable.GetEnumerator() {
			CheckValid();
			return InnerList.GetEnumerator();
		}
		void ICollection.CopyTo(Array array, int index) {
			CheckValid();
			Array.Copy(InnerList.ToArray(), 0, array, index, InnerList.Count);
		}
		bool ICollection.IsSynchronized { 
			get {
				CheckValid();
				return ((IList)this.InnerList).IsSynchronized;
			}
		}
		object ICollection.SyncRoot {
			get {
				CheckValid();
				return ((IList)this.InnerList).SyncRoot; 
			}
		}
		#endregion
	}
	#endregion
	#region ISupportIndex
	public interface ISupportIndex {
		int Index { get; }
	}
	#endregion 
	#region NativeChartIndexedCollectionBase (abstract class)
	abstract partial class NativeChartIndexedCollectionBase<T, N, M, C> : NativeChartCollectionBase<T, N, M>
		where T : class
		where N : NativeObjectBase, T, ISupportIndex
		where M : class
		where C : Model.ChartUndoableCollection<M> {
		readonly NativeWorkbook nativeWorkbook;
		public NativeChartIndexedCollectionBase(Model.ChartUndoableCollection<M> modelCollection, NativeWorkbook nativeWorkbook)
			: base(modelCollection) {
			this.nativeWorkbook = nativeWorkbook;
		}
		#region Properties
		protected C ModelChartCollection { get { return ModelCollection as C; } }
		protected NativeWorkbook NativeWorkbook { get { return nativeWorkbook; } }
		#endregion
		public T Add(int itemIndex) {
			CheckValid();
			return InnerList[FindInnerIndexByItemIndex(itemIndex)];
		}
		public T FindByIndex(int itemIndex) {
			CheckValid();
			int count = Count;
			for (int i = 0; i < count; i++) {
				N item = InnerList[i];
				if (item.Index == itemIndex)
					return item;
			}
			return null;
		}
		public bool Contains(T item) {
			CheckValid();
			N nativeItem = item as N;
			if (nativeItem == null || !nativeItem.IsValid)
				return false;
			return IndexOfCore(nativeItem) != -1;
		}
		public bool Remove(T item) {
			int index = IndexOf(item);
			if (index != -1)
				RemoveAt(index);
			return index != -1;
		}
		public int IndexOf(T item) {
			CheckValid();
			N nativeItem = item as N;
			if (nativeItem == null || !nativeItem.IsValid)
				return -1;
			return IndexOfCore(nativeItem);
		}
		#region Internal 
		int IndexOfCore(N nativeItem) {
			return InnerList.IndexOf(nativeItem);
		}
		int FindInnerIndexByItemIndex(int itemIndex) {
			int count = Count;
			for (int i = 0; i < count; i++) {
				N item = InnerList[i];
				if (item.Index == itemIndex)
					return i;
			}
			return ModelChartCollection.Add(CreateModelObject(itemIndex));
		}
		#endregion
		protected abstract M CreateModelObject(int index);
	}
	#endregion
}
