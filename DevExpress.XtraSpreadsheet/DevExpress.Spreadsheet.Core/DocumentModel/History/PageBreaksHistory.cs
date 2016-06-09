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
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Model.History {
	#region PageBreakInsertHistoryItem
	public class PageBreakInsertHistoryItem : SpreadsheetHistoryItem {
		#region Fields
		readonly PageBreakCollection collection;
		readonly int index;
		readonly int item;
		#endregion
		public PageBreakInsertHistoryItem(PageBreakCollection collection, int index, int item)
			: base(collection.Worksheet) {
			this.collection = collection;
			this.index = index;
			this.item = item;
		}
		protected override void UndoCore() {
			collection.InnerList.RemoveAt(index);
		}
		protected override void RedoCore() {
			collection.InnerList.Insert(index, item);
		}
	}
	#endregion
	#region PageBreakRemoveAtHistoryItem
	public class PageBreakRemoveAtHistoryItem : SpreadsheetHistoryItem {
		#region Fields
		readonly PageBreakCollection collection;
		readonly int index;
		int item;
		#endregion
		public PageBreakRemoveAtHistoryItem(PageBreakCollection collection, int index)
			: base(collection.Worksheet) {
			this.collection = collection;
			this.index = index;
		}
		protected override void UndoCore() {
			collection.InnerList.Insert(index, item);
		}
		protected override void RedoCore() {
			item = collection.InnerList[index];
			collection.InnerList.RemoveAt(index);
		}
	}
	#endregion
	#region PageBreakClearHistoryItem
	public class PageBreakClearHistoryItem : SpreadsheetHistoryItem {
		#region Fields
		readonly PageBreakCollection collection;
		readonly List<int> savedItems;
		#endregion
		public PageBreakClearHistoryItem(PageBreakCollection collection)
			: base(collection.Worksheet) {
			this.collection = collection;
			this.savedItems = new List<int>(collection.Count);
		}
		protected override void UndoCore() {
			collection.InnerList.Clear();
			collection.InnerList.AddRange(this.savedItems);
			this.savedItems.Clear();
		}
		protected override void RedoCore() {
			this.savedItems.Clear();
			this.savedItems.AddRange(collection.InnerList);
			collection.InnerList.Clear();
		}
	}
	#endregion
	#region PageBreakRangeRemovingHistoryItem
	public class PageBreakRangeRemovingHistoryItem : SpreadsheetHistoryItem {
		#region Fields
		readonly PageBreakCollection collection;
		readonly List<int> savedItems;
		readonly int start;
		readonly int end;
		#endregion
		public PageBreakRangeRemovingHistoryItem(PageBreakCollection collection, int start, int end)
			: base(collection.Worksheet) {
			this.collection = collection;
			this.savedItems = new List<int>(collection.Count);
			this.start = start;
			this.end = end;
		}
		protected override void UndoCore() {
			collection.InnerList.Clear();
			collection.InnerList.AddRange(this.savedItems);
			this.savedItems.Clear();
		}
		protected override void RedoCore() {
			List<int> innerList = collection.InnerList;
			this.savedItems.Clear();
			this.savedItems.AddRange(innerList);
			int deletedSize = end - start + 1;
			for (int i = innerList.Count - 1; i >= 0; i--) {
				if (innerList[i] >= start && innerList[i] <= end)
					innerList.RemoveAt(i);
				if (innerList.Count > i && innerList[i] > end)
					innerList[i] -= deletedSize;
			}
		}
	}
	#endregion
	#region PageBreakRangeInsertingHistoryItem
	public class PageBreakRangeInsertingHistoryItem : SpreadsheetHistoryItem {
		#region Fields
		readonly PageBreakCollection collection;
		readonly List<int> savedItems;
		readonly int start;
		readonly int end;
		#endregion
		public PageBreakRangeInsertingHistoryItem(PageBreakCollection collection, int start, int end)
			: base(collection.Worksheet) {
			this.collection = collection;
			this.savedItems = new List<int>(collection.Count);
			this.start = start;
			this.end = end;
		}
		protected override void UndoCore() {
			collection.InnerList.Clear();
			collection.InnerList.AddRange(this.savedItems);
			this.savedItems.Clear();
		}
		protected override void RedoCore() {
			List<int> innerList = collection.InnerList;
			this.savedItems.Clear();
			this.savedItems.AddRange(innerList);
			int insertedSize = end - start + 1;
			for (int i = innerList.Count - 1; i >= 0; i--)
				if (innerList[i] >= start)
					innerList[i] += insertedSize;
		}
	}
	#endregion
	#region PageBreakCopyHistoryItem
	public class PageBreakCopyHistoryItem : SpreadsheetHistoryItem {
		#region Fields
		readonly PageBreakCollection collection;
		readonly List<int> savedItems;
		readonly List<int> sourceItems;
		#endregion
		public PageBreakCopyHistoryItem(PageBreakCollection collection, List<int> source)
			: base(collection.Worksheet) {
			this.collection = collection;
			this.savedItems = new List<int>(collection.Count);
			this.sourceItems = new List<int>(source);
		}
		protected override void UndoCore() {
			collection.InnerList.Clear();
			collection.InnerList.AddRange(this.savedItems);
			this.savedItems.Clear();
		}
		protected override void RedoCore() {
			this.savedItems.Clear();
			this.savedItems.AddRange(collection.InnerList);
			collection.InnerList.Clear();
			collection.InnerList.AddRange(sourceItems);
		}
	}
	#endregion
}
