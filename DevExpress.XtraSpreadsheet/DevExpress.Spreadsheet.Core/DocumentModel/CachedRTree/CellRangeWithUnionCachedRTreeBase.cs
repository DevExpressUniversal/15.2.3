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

using DevExpress.XtraSpreadsheet.Utils.Trees;
using System;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Model {
	public abstract class CellRangeWithUnionCachedRTreeBase<TItem> 
		where TItem : class {
		#region Fields
		readonly RTree2D<TItem> tree = new RTree2D<TItem>();
		TItem cachedResult;
		IList<TItem> cachedListResult;
		#endregion
		#region Properties
		public int Count { get { return tree.Count; } }
		internal RTree2D<TItem> InnerTree { get { return tree; } }
		#endregion
		#region Insert
		public void Insert(TItem item) {
			Insert(item, GetItemRange(item));
		}
		public void InsertRange(IEnumerable<TItem> items) {
			foreach (TItem item in items)
				Insert(item);
		}
		protected internal void Insert(TItem item, CellRangeBase range) {
			CellUnion unionRange = range as CellUnion;
			if (unionRange == null) 
				InsertCore(item, range as CellRange);
			else {
				List<CellRangeBase> innerRanges = unionRange.InnerCellRanges;
				int count = innerRanges.Count;
				for (int i = 0; i < count; i++)
					Insert(item, innerRanges[i]);
			}
		}
		void InsertCore(TItem item, CellRange range) {
			tree.Insert(range.TopLeft.Column, range.TopLeft.Row, range.Width, range.Height, item);
		}
		#endregion
		#region Remove
		public void Remove(TItem item) {
			Remove(item, GetItemRange(item));
		}
		public void Remove(TItem item, CellRangeBase range) {
			CellUnion unionRange = range as CellUnion;
			if (unionRange == null)
				RemoveByRange(item, range as CellRange);
			else {
				List<CellRangeBase> innerRanges = unionRange.InnerCellRanges;
				int count = innerRanges.Count;
				for (int i = 0; i < count; i++)
					Remove(item, innerRanges[i]);
			}
		}
		void RemoveByRange(TItem item, CellRange range) {
			tree.Delete(new NodeBase(range.TopLeft.Column, range.TopLeft.Row, range.Width, range.Height), item, false);
			cachedResult = null;
		}
		#endregion
		public void Clear() {
			tree.Clear();
			cachedResult = null;
		}
		#region Search
		public List<TItem> Search(CellRangeBase range) {
			return Search(range, true);
		}
		public List<TItem> Search(CellRangeBase range, bool allowIntersection) {
			List<TItem> result = new List<TItem>();
			if (Count <= 0)
				return result;
			PopulateSearchedItems(result, range, allowIntersection);
			return result;
		}
		void PopulateSearchedItems(List<TItem> items, CellRangeBase range, bool allowIntersection) {
			CellUnion unionRange = range as CellUnion;
			if (unionRange == null) {
				List<TItem> searchedItems = SearchCore(range as CellRange, allowIntersection);
				items.AddRange(searchedItems);
			} else {
				List<CellRangeBase> innerRanges = unionRange.InnerCellRanges;
				int count = innerRanges.Count;
				for (int i = 0; i < count; i++)
					PopulateSearchedItems(items, innerRanges[i], allowIntersection);
			}
		}
		List<TItem> SearchCore(CellRange range, bool allowIntersection) {
			return tree.Search(new NodeBase(range.TopLeft.Column, range.TopLeft.Row, range.Width, range.Height), allowIntersection);
		}
		public TItem SearchItemOrNull(CellRangeBase range, bool allowIntersection) {
			if (Count <= 0)
				return null;
			return SearchItemOrNullCore(range, allowIntersection);
		}
		TItem SearchItemOrNullCore(CellRangeBase range, bool allowIntersection) {
			CellUnion unionRange = range as CellUnion;
			if (unionRange == null)
				return tree.SearchItemOrNull(new NodeBase(range.TopLeft.Column, range.TopLeft.Row, range.Width, range.Height), allowIntersection);
			else {
				List<CellRangeBase> innerRanges = unionRange.InnerCellRanges;
				int count = innerRanges.Count;
				for (int i = 0; i < count; i++) {
					TItem result = SearchItemOrNullCore(innerRanges[i], allowIntersection);
					if (result != null)
						return result;
				}
				return null;
			}
		}
		public TItem Search(int columnIndex, int rowIndex) {
			if (tree.Count <= 0)
				return null;
			if (cachedResult != null && GetItemRange(cachedResult).ContainsCell(columnIndex, rowIndex))
				return cachedResult;
			IList<TItem> items = SearchAll(columnIndex, rowIndex);
			if (items == null || items.Count <= 0)
				cachedResult = null;
			else
				cachedResult = items[0];
			return cachedResult;
		}
		public IList<TItem> SearchAll(int columnIndex, int rowIndex) {
			if (tree.Count <= 0)
				return null;
			if (cachedResult != null && GetItemRange(cachedResult).ContainsCell(columnIndex, rowIndex))
				return cachedListResult;
			this.cachedListResult = tree.Search(new NodeBase(columnIndex, rowIndex, 1, 1));
			return cachedListResult;
		}
		#endregion
		protected abstract CellRangeBase GetItemRange(TItem item);
	}
}
