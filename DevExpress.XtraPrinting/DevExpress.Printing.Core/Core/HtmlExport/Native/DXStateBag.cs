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
using DevExpress.Utils;
namespace DevExpress.XtraPrinting.HtmlExport.Native {
	public sealed class DXStateBag : IDictionary, ICollection, IEnumerable {
		static IDictionary CreateBag() {
			return new Dictionary<string, object>();
		}
		IDictionary bag;
		bool marked;
		public DXStateBag() {
			marked = false;
			bag = CreateBag();
		}
		public StateItem Add(string key, object value) {
			Guard.ArgumentNotNull(key, "key");
			StateItem item = bag[key] as StateItem;
			if(item == null) {
				if(value != null || marked) {
					item = new StateItem(value);
					bag.Add(key, item);
				}
			} else if(value == null && !marked)
				bag.Remove(key);
			else
				item.Value = value;
			if(item != null && marked)
				item.IsDirty = true;
			return item;
		}
		public void Clear() {
			bag.Clear();
		}
		public IDictionaryEnumerator GetEnumerator() {
			return bag.GetEnumerator();
		}
		public bool IsItemDirty(string key) {
			StateItem item = bag[key] as StateItem;
			return ((item != null) && item.IsDirty);
		}
		public void Remove(string key) {
			bag.Remove(key);
		}
		public void SetDirty(bool dirty) {
			if(bag.Count != 0)
				foreach(StateItem item in bag.Values)
					item.IsDirty = dirty;
		}
		public void SetItemDirty(string key, bool dirty) {
			StateItem item = bag[key] as StateItem;
			if(item != null)
				item.IsDirty = dirty;
		}
		void ICollection.CopyTo(Array array, int index) {
			Values.CopyTo(array, index);
		}
		void IDictionary.Add(object key, object value) {
			Add((string)key, value);
		}
		bool IDictionary.Contains(object key) {
			return bag.Contains((string)key);
		}
		void IDictionary.Remove(object key) {
			Remove((string)key);
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		internal void TrackViewState() {
			marked = true;
		}
		public int Count {
			get { return bag.Count; }
		}
		internal bool IsTrackingViewState {
			get { return marked; }
		}
		public object this[string key] {
			get {
				Guard.ArgumentNotNull(key, "key");
				StateItem item = bag[key] as StateItem;
				if(item != null)
					return item.Value;
				return null;
			}
			set { Add(key, value); }
		}
		public ICollection Keys {
			get { return bag.Keys; }
		}
		bool ICollection.IsSynchronized {
			get { return false; }
		}
		object ICollection.SyncRoot {
			get { return this; }
		}
		bool IDictionary.IsFixedSize {
			get { return false; }
		}
		bool IDictionary.IsReadOnly {
			get { return false; }
		}
		object IDictionary.this[object key] {
			get { return this[(string)key]; }
			set { this[(string)key] = value; }
		}
		public ICollection Values {
			get { return bag.Values; }
		}
	}
	public sealed class StateItem {
		bool isDirty;
		object value;
		internal StateItem(object initialValue) {
			value = initialValue;
			isDirty = false;
		}
		public bool IsDirty {
			get { return isDirty; }
			set { isDirty = value; }
		}
		public object Value {
			get { return value; }
			set { this.value = value; }
		}
	}
}
