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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.Internal;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Editors.Helpers {
	public class ChunkList2<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IList, ICollection, INotifyCollectionChanged {
		public string Tag { get; private set; }
		internal List<Chunk2<T>> ChunksList = new List<Chunk2<T>>();
		public int SplitSize { get; private set; }
		public int MergeSize { get; private set; }
		int? count;
		readonly Func<object, object> getChunkHandler;
		readonly Action<object, object> setChunkHandler;
		readonly bool optimizedMode;
		readonly bool subscribeToPropertyChanged;
		public ChunkList2(IEnumerable<T> enumerable = null, string tag = null, bool subscribeToPropertyChanged = false, int splitSize = 16000, int mergeSize = 16000 / 4) {
			SplitSize = splitSize;
			MergeSize = mergeSize;
			this.subscribeToPropertyChanged = subscribeToPropertyChanged && typeof(INotifyPropertyChanged).IsAssignableFrom(typeof(T));
			if (!string.IsNullOrEmpty(tag)) {
				this.getChunkHandler = ReflectionHelper.CreateInstanceMethodHandler<Func<object, object>>(null, "get_" + tag, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, typeof(T));
				this.setChunkHandler = ReflectionHelper.CreateInstanceMethodHandler<Action<object, object>>(null, "set_" + tag, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, typeof(T));
				optimizedMode = true;
			}
			if (enumerable == null) {
				this.ChunksList.Add(new Chunk2<T>(PropertyInItemChanged, splitSize));
			}
			else {
				SplitEnumerable(enumerable, SplitSize / 2);
				UpdateChunks(0);
			}
		}
		void SplitEnumerable(IEnumerable<T> source, int chunkSize) {
			var chunk = new Chunk2<T>(PropertyInItemChanged, chunkSize);
			foreach (var x in source) {
				chunk.Add(x);
				if (chunk.Count <= chunkSize) {
					continue;
				}
				ChunksList.Add(chunk);
				UpdateItemsInChunk(null, chunk, chunk);
				chunk = new Chunk2<T>(PropertyInItemChanged, chunkSize);
			}
			if (chunk.Any()) {
				ChunksList.Add(chunk);
				UpdateItemsInChunk(null, chunk, chunk);
			}
			else
				ChunksList.Add(new Chunk2<T>(PropertyInItemChanged, SplitSize / 2));
		}
		public int BinarySearch(T item, int startIndex, int count, IComparer<object> comparer) {
			int lo = startIndex;
			int length = count;
			int hi = startIndex + length - 1;
			while (lo <= hi) {
				int i = lo + ((hi - lo) >> 1);
				T target = this[i];
				int order = comparer.Compare(target, item);
				if (order == 0)
					return i;
				if (order < 0)
					lo = i + 1;
				else
					hi = i - 1;
			}
			return ~lo;
		}
		void UpdateItemsInChunk(Chunk2<T> oldChunk, Chunk2<T> chunk, IEnumerable<T> items) {
			foreach (var item in items) {
				if (oldChunk != null)
					UpdateItem(oldChunk, item, true);
				if (chunk != null)
					UpdateItem(chunk, item, false);
			}
		}
		public IEnumerator<T> GetEnumerator() {
			return new MergedEnumerator<T>(ChunksList.Select(chunk => (chunk as IEnumerable<T>).With(x => x.GetEnumerator())).ToArray());
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		public void Add(T item) {
			int chunkIndex = this.ChunksList.Count - 1;
			var chunk = GetChunk(chunkIndex);
			if (!CanInsertToChunk(chunk))
				chunk = AddChunk();
			chunk.Add(item);
			UpdateItem(chunk, item, false);
			UpdateChunks(chunkIndex);
			RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, chunkIndex));
		}
		void UpdateItem(Chunk2<T> chunk, T item, bool reset) {
			if (subscribeToPropertyChanged) {
				var changed = (INotifyPropertyChanged)item;
				if (reset) {
					changed.PropertyChanged -= chunk.ItemPropertyChanged;
				}
				else
					changed.PropertyChanged += chunk.ItemPropertyChanged;
			}
			if (!optimizedMode)
				return;
			this.setChunkHandler(item, reset ? null : chunk);
		}
		void PropertyInItemChanged(T item) {
			RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, item));
		}
		void InsertCore(int index, T item) {
			int chunkIndex = FindChunkIndex(index);
			var chunk = GetChunk(chunkIndex);
			if (!CanInsertToChunk(chunk))
				chunk = SplitChunk(chunk, chunkIndex, index);
			int indexInChunk = index - chunk.Index;
			chunk.Insert(indexInChunk, item);
			UpdateItem(chunk, item, false);
			UpdateChunks(chunkIndex);
			RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, chunkIndex));
		}
		bool CanInsertToChunk(Chunk2<T> chunk) {
			return chunk.Count < SplitSize;
		}
		Chunk2<T> GetChunk(int chunkIndex) {
			return this.ChunksList[chunkIndex];
		}
		void UpdateChunks(int chunkIndex) {
			var chunk = this.ChunksList[chunkIndex];
			int computedIndex = chunk.Index;
			bool hasMergeCandidates = false;
			for (int i = chunkIndex + 1; i < this.ChunksList.Count; i++) {
				var next = this.ChunksList[i];
				computedIndex += chunk.Count;
				next.Index = computedIndex;
				chunk = next;
				hasMergeCandidates |= chunk.Count < MergeSize;
			}
			if (hasMergeCandidates) {
				var next = this.ChunksList.LastOrDefault();
				for (int i = this.ChunksList.Count - 2; i >= 0; i--) {
					var current = this.ChunksList[i];
					if (current.Count + next.Count < MergeSize)
						MergeChunk(chunk, next, i);
					next = current;
				}
			}
			count = null;
		}
		Chunk2<T> SplitChunk(Chunk2<T> chunk, int chunkIndex, int index) {
			int indexInChunk = index - chunk.Index;
			if (indexInChunk < MergeSize || indexInChunk > chunk.Count - MergeSize - 1)
				return chunk;
			var newChunk = new Chunk2<T>(PropertyInItemChanged, chunk.Take(indexInChunk));
			UpdateItemsInChunk(chunk, newChunk, newChunk);
			newChunk.Index = chunk.Index;
			this.ChunksList.Insert(chunkIndex, newChunk);
			chunk.RemoveRange(0, indexInChunk);
			return newChunk;
		}
		void MergeChunk(Chunk2<T> chunk, Chunk2<T> next, int chunkIndex) {
			this.ChunksList.RemoveAt(chunkIndex + 1);
			chunk.AddRange(next);
			UpdateItemsInChunk(next, chunk, chunk);
		}
		Chunk2<T> AddChunk() {
			var chunk = new Chunk2<T>(PropertyInItemChanged, SplitSize);
			var prevChunk = GetChunk(this.ChunksList.Count - 1);
			chunk.Index = prevChunk.Index + prevChunk.Count;
			this.ChunksList.Add(chunk);
			return chunk;
		}
		int FindChunkIndex(int index) {
			int candidate = FindChunkIndexInternal(index);
			if (candidate >= 0)
				return candidate;
			return ChunksList.Count - 1;
		}
		public int FindChunkIndexInternal(int index) {
			int lo = 0;
			int length = this.ChunksList.Count;
			int hi = 0 + length - 1;
			while (lo <= hi) {
				int i = lo + ((hi - lo) >> 1);
				Chunk2<T> chunk = this.ChunksList[i];
				if (index >= chunk.Index && index < chunk.Index + chunk.Count)
					return i;
				if (index < chunk.Index)
					hi = i - 1;
				else
					lo = i + 1;
			}
			return ~lo;
		}
		public int Add(object value) {
			return Add(value);
		}
		public bool Contains(object value) {
			throw new NotImplementedException();
		}
		void IList.Clear() {
			throw new NotImplementedException();
		}
		public int IndexOf(object value) {
			throw new NotImplementedException();
		}
		public void Insert(int index, object value) {
			InsertCore(index, (T)value);
		}
		public void Remove(object value) {
			Remove((T)value);
		}
		public void RemoveAt(int index) {
			int chunkIndex = FindChunkIndex(index);
			var chunk = GetChunk(chunkIndex);
			if (!CanInsertToChunk(chunk))
				chunk = SplitChunk(chunk, chunkIndex, index + 1);
			int indexInChunk = index - chunk.Index;
			var item = chunk[indexInChunk];
			chunk.RemoveAt(indexInChunk);
			UpdateItem(chunk, item, true);
			UpdateChunks(chunkIndex);
			RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, chunkIndex));
		}
		object IList.this[int index] {
			get { return this[index]; }
			set { this[index] = (T)value; }
		}
		bool IList.IsReadOnly {
			get { return false; }
		}
		public bool IsFixedSize { get; private set; }
		void ICollection<T>.Clear() {
			throw new NotImplementedException();
		}
		public bool Contains(T item) {
			throw new NotImplementedException();
		}
		public void CopyTo(T[] array, int arrayIndex) {
			int index = arrayIndex;
			foreach (var item in this.Skip(arrayIndex))
				array[index++] = item;
		}
		public bool Remove(T item) {
			int index = IndexOf(item);
			if (index < 0)
				return false;
			RemoveAt(index);
			return true;
		}
		public void CopyTo(Array array, int index) {
			throw new NotImplementedException();
		}
		public int Count { get { return (count ?? (count = CalcCount())).Value; } }
		int CalcCount() {
			int resultCount = 0;
			for (int i = 0; i < this.ChunksList.Count; i++)
				resultCount += this.ChunksList[i].Count;
			return resultCount;
		}
		public object SyncRoot { get { return this; } }
		public bool IsSynchronized { get { return true; } }
		int ICollection<T>.Count { get { return Count; } }
		bool ICollection<T>.IsReadOnly { get { return false; } }
		public int IndexOf(T item) {
			if (this.optimizedMode) {
				return IndexOfOptimized(item);
			}
			return IndexOfUnoptimized(item);
		}
		int IndexOfOptimized(T item) {
			var chunk = (Chunk2<T>)this.getChunkHandler(item);
			if (chunk == null)
				return -1;
			int index = IndexOfInChunk(chunk, item);
			if (index < 0)
				return -1;
			return index + chunk.Index;
		}
		int IndexOfInChunk(Chunk2<T> chunk, T item) {
			for(int i = 0; i < chunk.Count; i++)
				if (object.ReferenceEquals(chunk[i], item))
					return i;
			return -1;
		}
		int IndexOfUnoptimized(T item) {
			int i = 0;
			foreach (var current in this) {
				if (ReferenceEquals(item, current))
					return i;
				i++;
			}
			return -1;
		}
		public void Insert(int index, T item) {
			InsertCore(index, item);
		}
		void IList<T>.RemoveAt(int index) {
			RemoveAt(index);
		}
		public T this[int index] {
			get {
				int chunkIndex = FindChunkIndex(index);
				var chunk = this.ChunksList[chunkIndex];
				return chunk[index - chunk.Index];
			}
			set {
				int chunkIndex = FindChunkIndex(index);
				var chunk = this.ChunksList[chunkIndex];
				int indexInChunk = index - chunk.Index;
				var oldValue = chunk[chunkIndex];
				chunk[indexInChunk] = value;
				RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, oldValue, value));
			}
		}
		void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e) {
			var collectionChanged = CollectionChanged;
			if (collectionChanged != null)
				collectionChanged(this, e);
		}
		public event NotifyCollectionChangedEventHandler CollectionChanged;
	}
	class Chunk2<T> : List<T> {
		readonly Action<T> notifyPropertyChanged;
		public Chunk2(Action<T> notifyPropertyChanged, int capacity) : base(capacity) {
			this.notifyPropertyChanged = notifyPropertyChanged;
		}
		public Chunk2(Action<T> notifyPropertyChanged, IEnumerable<T> source) : base(source) {
			this.notifyPropertyChanged = notifyPropertyChanged;
		}
		public int Index { get; set; }
		public void ItemPropertyChanged(object sender, PropertyChangedEventArgs e) {
			notifyPropertyChanged((T)sender);
		}
	}
}
