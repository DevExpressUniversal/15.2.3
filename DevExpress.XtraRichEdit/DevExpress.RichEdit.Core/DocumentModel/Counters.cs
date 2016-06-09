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
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Utils;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Model {
	#region Counters
	public class Counters {
		readonly Dictionary<string, Counter> items = new Dictionary<string, Counter>();
		protected internal Dictionary<string, Counter> InnerItems { get { return items; } }
		public void Clear() {
			items.Clear();
		}
		public void Reset() {
			foreach (string id in items.Keys)
				items[id].Reset();
		}
		public void ResetFrom(DocumentLogPosition logPosition) {
			foreach (string id in items.Keys)
				items[id].ResetFrom(logPosition);
		}
		public Counter RegisterCounter(string id) {
			Counter result = new Counter();
			items.Add(id, result);
			return result;
		}
		public void UnregisterCounter(string id) {
			if (items.ContainsKey(id))
				items.Remove(id);
		}
		public Counter GetCounter(string id) {
			return items[id];
		}
	}
	#endregion
	#region Counter
	public class Counter {
		readonly List<CounterItem> items = new List<CounterItem>(); 
		int lastValue;
		public void Reset() {
			items.Clear();
			lastValue = 1; 
		}
		public void ResetFrom(DocumentLogPosition logPosition) {
			int index = Algorithms.BinarySearch(items, new CounterItemAndLogPositionComparable(logPosition));
			if (index < 0)
				index = ~index;
			if (index >= items.Count)
				return;
			items.RemoveRange(index, items.Count - index);
			if (HasItems)
				lastValue = LastItem.Value;
			else
				lastValue = 1;
		}
		protected internal List<CounterItem> InnerItems { get { return items; } }
		protected internal bool HasItems { get { return items.Count > 0; } }
		public CounterItem LastItem { get { return HasItems ? items[items.Count - 1] : null; } }
		public int LastValue { get { return lastValue; } set { lastValue = value; } }
		public int Increment(DocumentLogPosition logPosition) {
			int index = Algorithms.BinarySearch(items, new CounterItemAndLogPositionComparable(logPosition));
			if (index >= 0)
				return items[index].Value;
			index = ~index;
			Debug.Assert(index >= items.Count);
			CounterItem item = new CounterItem(logPosition, lastValue);
			items.Add(item);
			lastValue++;
			return item.Value;
		}
	}
	#endregion
	#region CounterItem
	public class CounterItem {
		readonly DocumentLogPosition logPosition;
		readonly int value;
		public CounterItem(DocumentLogPosition logPosition, int value) {
			this.logPosition = logPosition;
			this.value = value;
		}
		public DocumentLogPosition LogPosition { get { return logPosition; } }
		public int Value { get { return value; } }
	}
	#endregion
	#region CounterItemAndLogPositionComparable
	public class CounterItemAndLogPositionComparable : IComparable<CounterItem> {
		readonly DocumentLogPosition logPosition;
		public CounterItemAndLogPositionComparable(DocumentLogPosition logPosition) {
			this.logPosition = logPosition;
		}
		public DocumentLogPosition LogPosition { get { return logPosition; } }
		#region IComparable<CounterItem> Members
		public int CompareTo(CounterItem item) {
			DocumentLogPosition itemLogPosition = item.LogPosition;
			if (logPosition < itemLogPosition)
				return 1;
			else if (logPosition > itemLogPosition)
				return -1;
			else
				return 0;
		}
		#endregion
	}
	#endregion
}
