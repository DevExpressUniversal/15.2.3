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

using DevExpress.Xpf.ChunkList.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
namespace DevExpress.Xpf.ChunkList.Native {
	class Chunk<T> {
		readonly ChunkList<T> chunkList;
		public Chunk(ChunkList<T> chunkList, int chunkSize) {
			Items = new List<T>(chunkSize);
			this.chunkList = chunkList;
		}
		public int PrevCount { get; set; }
		public int IndexOf(T item) {
			return PrevCount + GetIndexFast(item);
		}
		int GetIndexFast(T item) {
			List<T> items = Items;
			for(int i = 0; i < items.Count; i++) {
				if(object.ReferenceEquals(items[i], item))
					return i;
			}
			return -1;
		}
		public List<T> Items { get; set; }
		public void AddItem(T item) {
			Items.Add(item);
			OnItemAdded(item);
		}
		public void OnItemAdded(T item) {
			IChunkListObject chunkListObject = item as IChunkListObject;
			if(chunkListObject != null)
				chunkListObject.ChunkObject = this;
			else if(chunkList.UseChunksCache)
				chunkList.ChunksCache[item] = this;
			if(!chunkList.SupportPropertyChanged) return;
			INotifyPropertyChanging propertyChanging = item as INotifyPropertyChanging;
			if(propertyChanging != null)
				propertyChanging.PropertyChanging += propertyChanging_PropertyChanging;
			INotifyPropertyChanged propertyChanged = item as INotifyPropertyChanged;
			if(propertyChanged != null)
				propertyChanged.PropertyChanged += propertyChanged_PropertyChanged;
		}
		public void RemoveItem(T item) {
			Items.Remove(item);
			OnItemRemoved(item);
		}
		public void OnItemRemoved(T item) {
			IChunkListObject chunkListObject = item as IChunkListObject;
			if(chunkListObject != null)
				chunkListObject.ChunkObject = null;
			else if(chunkList.UseChunksCache)
				chunkList.ChunksCache.Remove(item);
			if(!chunkList.SupportPropertyChanged) return;
			INotifyPropertyChanging propertyChanging = item as INotifyPropertyChanging;
			if(propertyChanging != null)
				propertyChanging.PropertyChanging -= propertyChanging_PropertyChanging;
			INotifyPropertyChanged propertyChanged = item as INotifyPropertyChanged;
			if(propertyChanged != null)
				propertyChanged.PropertyChanged -= propertyChanged_PropertyChanged;
		}
		void propertyChanged_PropertyChanged(object sender, PropertyChangedEventArgs e) {
			chunkList.OnItemPropertyChanged(this, (T)sender, e.PropertyName);
		}
		void propertyChanging_PropertyChanging(object sender, PropertyChangingEventArgs e) {
			chunkList.OnItemPropertyChanging(this, (T)sender, e.PropertyName);
		}
	}
}
namespace DevExpress.Xpf.ChunkList {
	public class ChunkListChangedEventArgs : ListChangedEventArgs {
		public object OldItem { get; private set; }
		public ChunkListChangedEventArgs(ListChangedType listChangedType, int index, object oldItem) : base(listChangedType, index) {
			OldItem = oldItem;
		}
	}
	public delegate void ListChangingEventHandler(object sender, ListChangingEventArgs e);
	public class ListChangingEventArgs : EventArgs {
		public int Index { get; private set; }
		public PropertyDescriptor PropertyDescriptor { get; private set; }
		public ListChangingEventArgs(int index, PropertyDescriptor propertyDescriptor) {
			Index = index;
			PropertyDescriptor = propertyDescriptor;
		}
	}
	public interface IChunkListObject {
		object ChunkObject { get; set; }
	}
	public interface IListChanging {
		event ListChangingEventHandler ListChanging;
	}
	public class ChunkList<T> : IList<T>, IBindingList, IListChanging {
		readonly int chunkSize;
		int count = 0;
		List<Chunk<T>> chunks = new List<Chunk<T>>();
		public ChunkList(int capacity, bool useChunksCache = false) : this((int)Math.Ceiling(Math.Sqrt(capacity)), true, useChunksCache) { }
		public ChunkList(int chunkSize, bool supportPropertyChanged, bool useChunksCache = false) {
			this.chunkSize = chunkSize;
			SupportPropertyChanged = supportPropertyChanged;
			UseChunksCache = useChunksCache;
		}
		public event ListChangingEventHandler ListChanging;
		public event ListChangedEventHandler ListChanged;
		public bool SupportPropertyChanged { get; private set; }
		public bool UseChunksCache { get; private set; }
		Dictionary<T, Chunk<T>> chunksCache;
		internal Dictionary<T, Chunk<T>> ChunksCache {
			get {
				if(chunksCache == null)
					chunksCache = new Dictionary<T, Chunk<T>>();
				return chunksCache;
			}
		}
#if DEBUGTEST
		internal List<Chunk<T>> GetChunksDebugTest() {
			return chunks;
		}
		public static int IndexOfCallCount { get; set; }
#endif
		internal const string SlowIndexOfException = "Attempting to perform a slow 'index of' operation. Either implement the IChunkListObject interface for your data objects or enable the UseChunksCache option.";
		public int IndexOf(T item) {
			IChunkListObject chunkObject = item as IChunkListObject;
			Chunk<T> chunk;
			if(chunkObject == null) {
				if(!UseChunksCache)
					throw new InvalidOperationException(SlowIndexOfException);
				chunk = ChunksCache[item];
			} else {
				chunk = (Chunk<T>)chunkObject.ChunkObject;
			}
			return chunk != null ? chunk.IndexOf(item) : -1;
		}
		int lastItemIndex = -1;
		int IndexOf(Chunk<T> chunk, T item) {
			if(lastItemIndex < 0 || lastItemIndex >= count || !this[lastItemIndex].Equals(item)) {
#if DEBUGTEST
				IndexOfCallCount++;
#endif
				lastItemIndex = chunk.IndexOf(item);
			}
			return lastItemIndex;
		}
		public void Insert(int index, T item) {
			int chunkIndex = FindChunk(index);
			Chunk<T> chunk = chunks[chunkIndex];
			chunk.Items.Insert(index - chunk.PrevCount, item);
			chunk.OnItemAdded(item);
			count++;
			if(chunk.Items.Count > chunkSize * 1.5) {
				Chunk<T> newChunk = new Chunk<T>(this, chunkSize);
				chunks.Insert(chunks.IndexOf(chunk) + 1, newChunk);
				for(int i = chunk.Items.Count / 2; i < chunk.Items.Count; ) {
					T current = chunk.Items[i];
					chunk.RemoveItem(current);
					newChunk.AddItem(current);
				}
			}
			UpdatePrevCount(chunkIndex);
			RaiseListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, index));
		}
		void UpdatePrevCount(int startIndex) {
			List<Chunk<T>> localChunks = chunks;
			Chunk<T> chunk = localChunks[startIndex];
			int prevCount = chunk.PrevCount + chunk.Items.Count;
			for(int i = startIndex + 1; i < localChunks.Count; i++) {
				Chunk<T> current = localChunks[i];
				current.PrevCount = prevCount;
				prevCount += current.Items.Count;
			}
		}
		public void RemoveAt(int index) {
			int chunkIndex = FindChunk(index);
			Chunk<T> chunk = chunks[chunkIndex];
			int itemIndex = index - chunk.PrevCount;
			T item = chunk.Items[itemIndex];
			chunk.OnItemRemoved(item);
			chunk.Items.RemoveAt(itemIndex);
			count--;
			if(chunk.Items.Count < chunkSize) {
				if(chunkIndex > 0 && chunks[chunkIndex - 1].Items.Count + chunk.Items.Count <= chunkSize) {
					CombineChunks(chunkIndex - 1);
				}
				if(chunkIndex < chunks.Count - 1 && chunks[chunkIndex + 1].Items.Count + chunk.Items.Count <= chunkSize) {
					CombineChunks(chunkIndex);
				}
			}
			UpdatePrevCount(chunkIndex > 0 ? chunkIndex - 1 : 0);
			RaiseListChanged(new ChunkListChangedEventArgs(ListChangedType.ItemDeleted, index, item));
		}
		void CombineChunks(int chunkIndex) {
			Chunk<T> chunk = chunks[chunkIndex];
			Chunk<T> nextChunk = chunks[chunkIndex + 1];
			for(int i = 0; i < nextChunk.Items.Count; ) {
				T item = nextChunk.Items[0];
				nextChunk.RemoveItem(item);
				chunk.AddItem(item);
			}
			chunks.RemoveAt(chunkIndex + 1);
		}
		int FindChunk(int index) {
			int first = 0, last = chunks.Count - 1;
			while(first < last) {
				int middle = first + (last - first) / 2;
				if(index < chunks[middle + 1].PrevCount)
					last = middle;
				else
					first = middle + 1;
			}
			return last;
		}
		public T this[int index] {
			get {
				Chunk<T> chunk = chunks[FindChunk(index)];
				return chunk.Items[index - chunk.PrevCount];
			}
			set {
				Chunk<T> chunk = chunks[FindChunk(index)];
				int innerIndex = index - chunk.PrevCount;
				T oldItem = chunk.Items[innerIndex];
				chunk.OnItemRemoved(oldItem);
				chunk.Items[innerIndex] = value;
				chunk.OnItemAdded(value);
				RaiseListChanged(new ChunkListChangedEventArgs(ListChangedType.ItemChanged, index, oldItem));
			}
		}
		public void Add(T item) {
			if(chunks.Count == 0 || chunks[chunks.Count - 1].Items.Count == chunkSize)
				chunks.Add(new Chunk<T>(this, chunkSize) { PrevCount = count });
			chunks[chunks.Count - 1].AddItem(item);
			count++;
			RaiseListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, count - 1));
		}
		internal void OnItemPropertyChanged(Chunk<T> chunk, T item, string propertyName) {
			RaiseListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, IndexOf(chunk, item), GetPropertyDescriptor(item, propertyName)));
		}
		PropertyDescriptorCollection itemProperties;
		PropertyDescriptor GetPropertyDescriptor(object item, string propertyName) {
			if(itemProperties == null)
				itemProperties = TypeDescriptor.GetProperties(item);
			return itemProperties.Find(propertyName, true);
		}
		internal void OnItemPropertyChanging(Chunk<T> chunk, T item, string propertyName) {
			RaiseListChanging(new ListChangingEventArgs(IndexOf(chunk, item), GetPropertyDescriptor(item, propertyName)));
		}
		void RaiseListChanging(ListChangingEventArgs args) {
			if(ListChanging != null)
				ListChanging(this, args);
		}
		void RaiseListChanged(ListChangedEventArgs args) {
			if(ListChanged != null)
				ListChanged(this, args);
		}
		public void Clear() {
			foreach(Chunk<T> chunk in chunks) {
				foreach(T item in chunk.Items)
					chunk.OnItemRemoved(item);
				chunk.Items.Clear();
			}
			chunks.Clear();
			RaiseListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
			count = 0;
		}
		public bool Contains(T item) {
			return IndexOf(item) != -1;
		}
		public void CopyTo(T[] array, int arrayIndex) {
			((ICollection)this).CopyTo((Array)array, arrayIndex);
		}
		public int Count {
			get { return count; }
		}
		bool IList.IsReadOnly {
			get { return false; }
		}
		bool ICollection<T>.IsReadOnly {
			get { return false; }
		}
		public bool Remove(T item) {
			RemoveAt(IndexOf(item));
			return true;
		}
		IEnumerable<T> GetEnumerable() {
			foreach(Chunk<T> chunk in chunks)
				foreach(T item in chunk.Items)
					yield return item;
		}
		public IEnumerator<T> GetEnumerator() {
			return GetEnumerable().GetEnumerator();
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerable().GetEnumerator();
		}
		int IList.Add(object value) {
			Add((T)value);
			return count - 1;
		}
		bool IList.Contains(object value) {
			return Contains((T)value);
		}
		int IList.IndexOf(object value) {
			return IndexOf((T)value);
		}
		void IList.Insert(int index, object value) {
			Insert(index, (T)value);
		}
		void IList.Remove(object value) {
			Remove((T)value);
		}
		object System.Collections.IList.this[int index] {
			get {
				return this[index];
			}
			set {
				this[index] = (T)value;
			}
		}
		void ICollection.CopyTo(Array array, int index) {
			for(int i = 0; i < count; i++) {
				array.SetValue(this[i], i + index);
			}
		}
		void IBindingList.AddIndex(PropertyDescriptor property) {
			throw new NotSupportedException();
		}
		object IBindingList.AddNew() {
			throw new NotSupportedException();
		}
		bool IBindingList.AllowEdit {
			get { return true; }
		}
		bool IBindingList.AllowNew {
			get { return false; }
		}
		bool IBindingList.AllowRemove {
			get { return true; }
		}
		void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction) {
			throw new NotSupportedException();
		}
		int IBindingList.Find(PropertyDescriptor property, object key) {
			throw new NotSupportedException();
		}
		bool IBindingList.IsSorted {
			get { return false; }
		}
		void IBindingList.RemoveIndex(PropertyDescriptor property) {
			throw new NotSupportedException();
		}
		void IBindingList.RemoveSort() {
			throw new NotSupportedException();
		}
		ListSortDirection IBindingList.SortDirection {
			get { throw new NotSupportedException(); }
		}
		PropertyDescriptor IBindingList.SortProperty {
			get { throw new NotSupportedException(); }
		}
		bool IBindingList.SupportsChangeNotification {
			get { return true; }
		}
		bool IBindingList.SupportsSearching {
			get { return false; }
		}
		bool IBindingList.SupportsSorting {
			get { return false; }
		}
		bool IList.IsFixedSize {
			get { return false; }
		}
		bool ICollection.IsSynchronized {
			get { return false; }
		}
		object syncRoot = new object();
		object ICollection.SyncRoot {
			get { return syncRoot; }
		}
	}
}
