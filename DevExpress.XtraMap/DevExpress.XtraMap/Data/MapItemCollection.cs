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

using System.Collections;
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.XtraMap.Native;
namespace DevExpress.XtraMap {
	public class MapItemCollection : OwnedCollection<MapItem>, ISupportSwapItems {
		Dictionary<int, int> rowIndexToIndexHash;
		protected IMapDataAdapter DataAdapter { get { return Owner as IMapDataAdapter; } }
		protected Dictionary<int, int> RowIndexToIndexHash { get { return rowIndexToIndexHash; } }
		public MapItemCollection(IMapDataAdapter dataAdapter)
			: base(dataAdapter, DXCollectionUniquenessProviderType.None) {
				Guard.ArgumentNotNull(dataAdapter, "dataAdapter");
				rowIndexToIndexHash = new Dictionary<int, int>();
		}
		#region ISupportSwapItems members
		void ISupportSwapItems.Swap(int index1, int index2) {
			if (index1 == index2)
				return;
			MapItem swapItem = InnerList[index1];
			InnerList[index1] = InnerList[index2];
			InnerList[index2] = swapItem;
		}
		#endregion
		internal MapItem GetItemByRowIndex(int rowIndex) {
			int index = -1;
			if(RowIndexToIndexHash.TryGetValue(rowIndex, out index))
				return GetItemInternal(index);
			return null;
		}
		internal MapItem GetItemInternal(int index) {
			lock(InnerList) {
				return (index >= 0 && index < Count) ? base.GetItem(index) : null;
			}
		}
		public override int Add(MapItem value) {
			int res = -1;
			lock(DataAdapter.UpdateLocker) {
				res = base.Add(value);
			}
			return res;
		}
		protected override bool RemoveAtCore(int index) {
			bool res = false;
			lock(DataAdapter.UpdateLocker) {
				res = base.RemoveAtCore(index);
			}
			return res;
		}
		public void Swap(int index1, int index2) {
			if (index1 == index2)
				return;
			MapItem swapItem = InnerList[index1];
			InnerList[index1] = InnerList[index2];
			InnerList[index2] = swapItem;
		}
		protected override bool OnClear() {
			bool res = false;
			lock(DataAdapter.UpdateLocker) {
				res = base.OnClear();
			}
			return res;
		}
		internal void SetRangeInternal(ICollection collection) {
			BeginUpdate();
			try {
				Clear();
				if(collection == null || collection.Count <= 0)
					return;
				foreach(MapItem item in collection)
					Insert(0, item);
			} finally {
				EndUpdate();
			}
		}
		protected override void OnInsertComplete(int index, MapItem value) {
			base.OnInsertComplete(index, value);
			if(value == null || value.RowIndex < 0) return;
			if(RowIndexToIndexHash.ContainsKey(value.RowIndex))
				RowIndexToIndexHash[value.RowIndex] = index;
			else
				RowIndexToIndexHash.Add(value.RowIndex, index);
		}
		protected override void OnRemoveComplete(int index, MapItem value) {
			base.OnRemoveComplete(index, value);
			if(value == null || value.RowIndex < 0) return;
			if (RowIndexToIndexHash.ContainsKey(value.RowIndex))
				RowIndexToIndexHash.Remove(value.RowIndex);
		}
		protected override void OnClearComplete() {
			base.OnClearComplete();
			RowIndexToIndexHash.Clear();
		}
	}
}
