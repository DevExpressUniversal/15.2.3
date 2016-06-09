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
using System.Collections.ObjectModel;
using System.Diagnostics;
using DevExpress.Utils;
namespace DevExpress.Office.Utils {
	#region List<T, U>
	public class List<T, U> : IList<T>, ICollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable where U : struct, IConvertToInt<U> {
		static readonly U indexConverter = default(U);
		readonly List<T> innerList;
		public List() {
			this.innerList = new List<T>();
		}
		public List(IEnumerable<T> collection) {
			this.innerList = new List<T>(collection);
		}
		public List(int capacity) {
			this.innerList = new List<T>(capacity);
		}
		#region Properties
		public List<T> InnerList { get { return innerList; } }
		public int Capacity { get { return InnerList.Capacity; } set { InnerList.Capacity = value; } }
		public int Count {
			get {
				return InnerList.Count;
			}
		}
		public T this[U index] {
			get { return InnerList[index.ToInt()]; }
			set { InnerList[index.ToInt()] = value; }
		}
		bool ICollection<T>.IsReadOnly { get { return ((ICollection<T>)InnerList).IsReadOnly; } }
		bool ICollection.IsSynchronized { get { return ((ICollection)InnerList).IsSynchronized; } }
		object ICollection.SyncRoot { get { return ((ICollection)InnerList).SyncRoot; } }
		bool IList.IsFixedSize { get { return ((IList)InnerList).IsFixedSize; } }
		bool IList.IsReadOnly { get { return ((IList)InnerList).IsReadOnly; } }
		object IList.this[int index] {
			get { return InnerList[index]; }
			set { InnerList[index] = (T)value; }
		}
		T IList<T>.this[int index] {
			get { return InnerList[index]; }
			set { InnerList[index] = value; }
		}
		#region First
		public T First {
			[DebuggerStepThrough]
			get {
				if (Count <= 0)
					return default(T);
				else
					return InnerList[0];
			}
		}
		#endregion
		#region Last
		public T Last {
			[DebuggerStepThrough]
			get {
				if (Count <= 0)
					return default(T);
				else
					return InnerList[Count - 1];
			}
		}
		#endregion
		#endregion
		public void Add(T item) {
			InnerList.Add(item);
		}
		public void AddRange(IEnumerable<T> collection) {
			InnerList.AddRange(collection);
		}
		public ReadOnlyCollection<T> AsReadOnly() {
			return InnerList.AsReadOnly();
		}
		public U BinarySearch(T item) {
			return indexConverter.FromInt(InnerList.BinarySearch(item));
		}
		public U BinarySearch(T item, IComparer<T> comparer) {
			return indexConverter.FromInt(InnerList.BinarySearch(item, comparer));
		}
		public U BinarySearch(U index, int count, T item, IComparer<T> comparer) {
			return indexConverter.FromInt(InnerList.BinarySearch(index.ToInt(), count, item, comparer));
		}
		public virtual void Clear() {
			InnerList.Clear();
		}
		public bool Contains(T item) {
			return InnerList.Contains(item);
		}
		public void CopyTo(T[] array) {
			InnerList.CopyTo(array);
		}
		public void CopyTo(T[] array, int arrayIndex) {
			InnerList.CopyTo(array, arrayIndex);
		}
		public void CopyTo(U index, T[] array, int arrayIndex, int count) {
			InnerList.CopyTo(index.ToInt(), array, arrayIndex, count);
		}
		public void ForEach(Action<T> action) {
			InnerList.ForEach(action);
		}
		public List<T>.Enumerator GetEnumerator() {
			return InnerList.GetEnumerator();
		}
		public List<T> GetRange(U index, int count) {
			return InnerList.GetRange(index.ToInt(), count);
		}
		public U IndexOf(T item) {
			return indexConverter.FromInt(InnerList.IndexOf(item));
		}
		int IList<T>.IndexOf(T item) {
			return InnerList.IndexOf(item);
		}
		public U IndexOf(T item, U index) {
			return indexConverter.FromInt(InnerList.IndexOf(item, index.ToInt()));
		}
		public U IndexOf(T item, U index, int count) {
			return indexConverter.FromInt(InnerList.IndexOf(item, index.ToInt(), count));
		}
		public void Insert(U index, T item) {
			InnerList.Insert(index.ToInt(), item);
		}
		void IList<T>.Insert(int index, T item) {
			InnerList.Insert(index, item);
		}
		public void InsertRange(U index, IEnumerable<T> collection) {
			InnerList.InsertRange(index.ToInt(), collection);
		}
		public U LastIndexOf(T item) {
			return indexConverter.FromInt(InnerList.LastIndexOf(item));
		}
		public U LastIndexOf(T item, U index) {
			return indexConverter.FromInt(InnerList.LastIndexOf(item, index.ToInt()));
		}
		public U LastIndexOf(T item, U index, int count) {
			return indexConverter.FromInt(InnerList.LastIndexOf(item, index.ToInt(), count));
		}
		public bool Remove(T item) {
			return InnerList.Remove(item);
		}
		public void RemoveAt(U index) {
			InnerList.RemoveAt(index.ToInt());
		}
		void IList<T>.RemoveAt(int index) {
			InnerList.RemoveAt(index);
		}
		void IList.RemoveAt(int index) {
			InnerList.RemoveAt(index);
		}
		public void RemoveRange(U index, int count) {
			InnerList.RemoveRange(index.ToInt(), count);
		}
		public void Reverse() {
			InnerList.Reverse();
		}
		public void Reverse(U index, int count) {
			InnerList.Reverse(index.ToInt(), count);
		}
		public void Sort() {
			InnerList.Sort();
		}
		public void Sort(IComparer<T> comparer) {
			InnerList.Sort(comparer);
		}
		public void Sort(Comparison<T> comparison) {
			InnerList.Sort(comparison);
		}
		public void Sort(U index, int count, IComparer<T> comparer) {
			InnerList.Sort(index.ToInt(), count, comparer);
		}
		IEnumerator<T> IEnumerable<T>.GetEnumerator() {
			return ((IEnumerable<T>)InnerList).GetEnumerator();
		}
		void ICollection.CopyTo(Array array, int arrayIndex) {
			((ICollection)InnerList).CopyTo(array, arrayIndex);
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return ((IEnumerable)InnerList).GetEnumerator();
		}
		int IList.Add(object item) {
			return ((IList)InnerList).Add(item);
		}
		bool IList.Contains(object item) {
			return ((IList)InnerList).Contains(item);
		}
		int IList.IndexOf(object item) {
			return ((IList)InnerList).IndexOf(item);
		}
		void IList.Insert(int index, object item) {
			((IList)InnerList).Insert(index, item);
		}
		void IList.Remove(object item) {
			((IList)InnerList).Remove(item);
		}
		public T[] ToArray() {
			return InnerList.ToArray();
		}
		public void TrimExcess() {
			InnerList.TrimExcess();
		}
#if !SL
		public bool TrueForAll(Predicate<T> match) {
			return InnerList.TrueForAll(match);
		}
		public int RemoveAll(Predicate<T> match) {
			return InnerList.RemoveAll(match);
		}
#if !DXRESTRICTED
		public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter) {
			return InnerList.ConvertAll(converter);
		}
#endif
		public bool Exists(Predicate<T> match) {
			return InnerList.Exists(match);
		}
		public T Find(Predicate<T> match) {
			return InnerList.Find(match);
		}
		public List<T> FindAll(Predicate<T> match) {
			return InnerList.FindAll(match);
		}
		public U FindIndex(Predicate<T> match) {
			return indexConverter.FromInt(InnerList.FindIndex(match));
		}
		public U FindIndex(U startIndex, Predicate<T> match) {
			return indexConverter.FromInt(InnerList.FindIndex(startIndex.ToInt(), match));
		}
		public U FindIndex(U startIndex, int count, Predicate<T> match) {
			return indexConverter.FromInt(InnerList.FindIndex(startIndex.ToInt(), count, match));
		}
		public T FindLast(Predicate<T> match) {
			return InnerList.FindLast(match);
		}
		public U FindLastIndex(Predicate<T> match) {
			return indexConverter.FromInt(InnerList.FindLastIndex(match));
		}
		public U FindLastIndex(U startIndex, Predicate<T> match) {
			return indexConverter.FromInt(InnerList.FindLastIndex(startIndex.ToInt(), match));
		}
		public U FindLastIndex(U startIndex, int count, Predicate<T> match) {
			return indexConverter.FromInt(InnerList.FindLastIndex(startIndex.ToInt(), count, match));
		}
#endif
	}
	#endregion
	#region OfficeAlgorithms
	public static class OfficeAlgorithms {
		public static U BinarySearch<T, U>(List<T, U> list, IComparable<T> predicate) where U : struct, IConvertToInt<U> {
			U indexConverter = default(U);
			int low = 0;
			int hi = list.Count - 1;
			while (low <= hi) {
				int median = (low + ((hi - low) >> 1));
				int compareResult = predicate.CompareTo(list.InnerList[median]);
				if (compareResult == 0)
					return indexConverter.FromInt(median);
				if (compareResult < 0)
					low = median + 1;
				else
					hi = median - 1;
			}
			return indexConverter.FromInt(~low);
		}
	}
	#endregion
}
