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
using System.Threading;
namespace DevExpress.Utils {
	public class IndexedDictionary<T> : IList<T> {
		List<T> itemsArray = new List<T>();
		Dictionary<T, int> indices;
		Dictionary<T, int> Indices {
			get {
				if (indices == null) {
					indices = new Dictionary<T, int>();
					for (int i = 0; i < itemsArray.Count; i++)
						indices.Add(itemsArray[i], i);
				}
				return indices;
			}
		}
		public int Count { get { return itemsArray.Count; } }
		public T this[int index] {
			get { return itemsArray[index]; }
			set {
				Indices.Remove(itemsArray[index]);
				itemsArray[index] = value;
				Indices.Add(itemsArray[index], index);
			}
		}
		public int Add(T item) {
			itemsArray.Add(item);
			Indices[item] = itemsArray.Count - 1;
			return itemsArray.Count - 1;
		}
		public bool Remove(T item) {
			if (itemsArray.Remove(item)) {
				ClearIndices();
				return true;
			}
			return false;
		}
		void ClearIndices() {
			if (indices != null) {
				indices.Clear();
				indices = null;
			}
		}
		public void RemoveAt(int index) {
			if (index < itemsArray.Count) {
				itemsArray.RemoveAt(index);
				ClearIndices();
			}
		}
		public int IndexOf(T item) {
			int index;
			if (Indices.TryGetValue(item, out index))
				return index;
			return -1;
		}
		public void Clear() {
			itemsArray.Clear();
			ClearIndices();
		}
		#region IList<T> Members
		void IList<T>.Insert(int index, T item) {
			itemsArray.Insert(index, (T)item);
			ClearIndices();
		}
		void ICollection<T>.Add(T item) {
			Add(item);
		}
		bool ICollection<T>.Contains(T item) {
			return IndexOf(item) >= 0;
		}
		void ICollection<T>.CopyTo(T[] array, int arrayIndex) {
			for (int i = 0; i < itemsArray.Count; i++)
				array.SetValue(itemsArray[i], i + arrayIndex);
		}
		bool ICollection<T>.IsReadOnly {
			get { return false; }
		}
		IEnumerator<T> IEnumerable<T>.GetEnumerator() {
			return itemsArray.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return itemsArray.GetEnumerator();
		}
		#endregion
	}
}
