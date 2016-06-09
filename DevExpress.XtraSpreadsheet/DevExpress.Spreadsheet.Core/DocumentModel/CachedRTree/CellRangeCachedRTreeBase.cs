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
using DevExpress.XtraSpreadsheet.Utils.Trees;
namespace DevExpress.XtraSpreadsheet.Model {
	public abstract class CellRangeCachedRTreeBase<TItem>
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
		protected internal void Insert(TItem item, CellRange range) {
			InsertCore(item, range);
		}
		void InsertCore(TItem item, CellRange range) {
			tree.Insert(range.TopLeft.Column, range.TopLeft.Row, range.Width, range.Height, item);
		}
		#endregion
		#region Remove
		public void Remove(TItem item) {
			RemoveByRange(item, GetItemRange(item));
		}
		public virtual void Remove(TItem item, CellRange range) {
			RemoveByRange(item, range);
		}
		protected internal void RemoveByRange(TItem item, CellRange range) {
			tree.Delete(new NodeBase(range.TopLeft.Column, range.TopLeft.Row, range.Width, range.Height), item, false);
			cachedResult = null;
		}
		#endregion
		public void Clear() {
			tree.Clear();
			cachedResult = null;
		}
		#region Search
		public List<TItem> Search(CellRange range) {
			return Search(range, true);
		}
		public List<TItem> Search(CellRange range, bool allowIntersection) {
			if (Count <= 0)
				return new List<TItem>();
			return tree.Search(new NodeBase(range.TopLeft.Column, range.TopLeft.Row, range.Width, range.Height), allowIntersection);
		}
		public TItem SearchItemOrNull(CellRange range, bool allowIntersection) {
			if (Count <= 0)
				return null;
			return tree.SearchItemOrNull(new NodeBase(range.TopLeft.Column, range.TopLeft.Row, range.Width, range.Height), allowIntersection);
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
		protected abstract CellRange GetItemRange(TItem item);
	}
}
