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
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.Xpf.PivotGrid.Internal {
	public abstract class ScrollableItemsList {
		readonly List<int> rowIndexes;
		HybridArray<ScrollableAreaItemBase> items;
		protected ScrollableItemsList()
			: base() {
			this.rowIndexes = new List<int>();
		}
		protected internal HybridArray<ScrollableAreaItemBase> InnerItems {
			get {
				if(items == null)
					items = new HybridArray<ScrollableAreaItemBase>(Count);
				return items;
			}
		}
		public abstract int RowCount { get; }
		public abstract int ColumnCount { get; }
		public int MaxLevel { get { return Math.Max(RowCount - 1, 0); } }
		public int MaxIndex { get { return Math.Max(ColumnCount - 1, 0); } }
		public abstract int Count { get; }
		public virtual ScrollableAreaItemBase this[int index] {
			get {
				if(InnerItems[index] == null)
					InnerItems[index] = CreateItem(index);
				return InnerItems[index];
			}
		}
		public virtual object GetRawValue(int index) {
			return InnerItems[index];
		}
		protected abstract ScrollableAreaItemBase CreateItem(int index);
		public virtual int GetRowIndex(int index) {
			return GetRowIndexCore(index);
		}
		protected int GetRowIndexCore(int index) {
			if(rowIndexes.Count == 0)
				UpdateRowIndexes();
			return index < rowIndexes.Count ? rowIndexes[index] : Count;
		}
		void UpdateRowIndexes() {
			rowIndexes.Clear();
			int prevIndex = -1;
			for(int i = 0; i < Count; i++) {
				int minLevel = GetMinLevelAtIndex(i);
				if(minLevel > prevIndex) {
					rowIndexes.Add(i);
					prevIndex = minLevel;
				}
				if(minLevel < prevIndex)
					throw new Exception("Unsorted list");
			}
		}
		protected virtual int GetMinLevelAtIndex(int i) {
			return this[i].MinLevel;
		}
	}
	public class PivotFieldValueItemItemsComparer : IComparer<PivotFieldValueItem>, IComparer<ScrollableAreaItemBase>, IComparer<FieldValueItem> {
		static PivotFieldValueItemItemsComparer _default;
		public static PivotFieldValueItemItemsComparer Default {
			get {
				if(_default == null)
					_default = new PivotFieldValueItemItemsComparer();
				return _default;
			}
		}
		IComparer<int> intComparer;
		public PivotFieldValueItemItemsComparer() {
			this.intComparer = Comparer<int>.Default;
		}
		#region IComparer<PivotFieldValueItem> Members
		public int Compare(PivotFieldValueItem x, PivotFieldValueItem y) {
			int res = CompareItems(x, y, x.IsColumn);
			if(res != 0) return res;
			return CompareItems(x, y, !x.IsColumn);
		}
		int CompareItems(PivotFieldValueItem x, PivotFieldValueItem y, bool isColumn) {
			if(isColumn)
				return intComparer.Compare(x.StartLevel, y.StartLevel);
			else
				return intComparer.Compare(x.MinLastLevelIndex, y.MinLastLevelIndex);
		}
		#endregion
		#region IComparer<ScrollableAreaItemBase> Members
		public int Compare(ScrollableAreaItemBase x, ScrollableAreaItemBase y) {
			int res = intComparer.Compare(x.MinLevel, y.MinLevel);
			if(res != 0) return res;
			return intComparer.Compare(x.MinIndex, y.MinIndex);
		}
		#endregion
		#region IComparer<FieldValueItem> Members
		int IComparer<FieldValueItem>.Compare(FieldValueItem x, FieldValueItem y) {
			return Compare(x, y);
		}
		#endregion
	}
	public class HybridArray<T> {
		const int MaxArraySize = 5000;
		readonly int count;
		readonly Dictionary<int, T> dic;
		readonly T[] array;
		public HybridArray(int count) {
			this.count = count;
			if(count < MaxArraySize)
				array = new T[count];
			else
				dic = new Dictionary<int, T>();
		}
		public int Count { get { return count; } }
		public T this[int index] {
			get {
				if(array != null)
					return array[index];
				T res;
				return dic.TryGetValue(index, out res) ? res : default(T);
			}
			set {
				if(array != null)
					array[index] = value;
				else
					dic[index] = value;
			}
		}
	}
}
