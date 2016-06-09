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
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.XtraSpreadsheet.Model.History;
namespace DevExpress.XtraSpreadsheet.Model {
	#region PageBreakCollection
	public class PageBreakCollection : ICloneable<PageBreakCollection>, ISupportsCopyFrom<PageBreakCollection> {
		readonly Worksheet worksheet;
		readonly List<int> innerList = new List<int>();
		public PageBreakCollection(Worksheet worksheet) {
			this.worksheet = worksheet;
		}
		public int Count { get { return this.innerList.Count; } }
		public int this[int position] { get { return innerList[position]; } }
		protected internal List<int> InnerList { get { return innerList; } }
		protected internal Worksheet Worksheet { get { return worksheet; } }
		DocumentHistory History { get { return worksheet.Workbook.History; } }
		public void Add(int index) {
			int position = innerList.BinarySearch(index);
			if (position < 0) {
				position = ~position;
				PageBreakInsertHistoryItem historyItem = new PageBreakInsertHistoryItem(this, position, index);
				History.Add(historyItem);
				historyItem.Execute();
			}
		}
		public bool Contains(int index) {
			int position = innerList.BinarySearch(index);
			return position >= 0;
		}
		public bool TryInsert(int index) {
			int position = innerList.BinarySearch(index);
			if (position < 0) {
				position = ~position;
				PageBreakInsertHistoryItem historyItem = new PageBreakInsertHistoryItem(this, position, index);
				History.Add(historyItem);
				historyItem.Execute();
				return true;
			}
			return false;
		}
		public void Remove(int index) {
			int position = innerList.BinarySearch(index);
			if (position >= 0) {
				PageBreakRemoveAtHistoryItem historyItem = new PageBreakRemoveAtHistoryItem(this, position);
				History.Add(historyItem);
				historyItem.Execute();
			}
		}
		public void Clear() {
			if (this.innerList.Count == 0)
				return;
			PageBreakClearHistoryItem historyItem = new PageBreakClearHistoryItem(this);
			History.Add(historyItem);
			historyItem.Execute();
		}
		public int IndexOf(int index) {
			int position = innerList.BinarySearch(index);
			if (position < 0)
				return -1;
			return position;
		}
		public void ForEach(Action<int> action) {
			innerList.ForEach(action);
		}
		public void OnRangeRemoving(int startRemoveIndex, int endRemoveIndex) {
			if(innerList.Count <= 0)
				return;
			PageBreakRangeRemovingHistoryItem historyItem = new PageBreakRangeRemovingHistoryItem(this, startRemoveIndex, endRemoveIndex);
			History.Add(historyItem);
			historyItem.Execute();
		}
		public void OnRangeInserting(int startInsertIndex, int endInsertIndex) {
			if(innerList.Count <= 0)
				return;
			PageBreakRangeInsertingHistoryItem historyItem = new PageBreakRangeInsertingHistoryItem(this, startInsertIndex, endInsertIndex);
			History.Add(historyItem);
			historyItem.Execute();
		}
		#region ICloneable<PageBreakCollection> Members
		public PageBreakCollection Clone() {
			PageBreakCollection newCollection = new PageBreakCollection(worksheet);
			newCollection.CopyFrom(this);
			return newCollection;
		}
		#endregion
		#region ISupportsCopyFrom<PageBreakCollection> Members
		public void CopyFrom(PageBreakCollection value) {
			if (value.innerList.Count == 0 && this.innerList.Count == 0)
				return;
			PageBreakCopyHistoryItem historyItem = new PageBreakCopyHistoryItem(this, value.innerList);
			History.Add(historyItem);
			historyItem.Execute();
		}
		#endregion
	}
	#endregion
}
