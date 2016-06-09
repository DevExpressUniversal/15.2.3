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
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.Utils;
using System.Security.Cryptography;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System;
#if !SL
using System.Drawing.Printing;
#if !DXPORTABLE
using System.Runtime.ConstrainedExecution;
#endif
using System.Drawing;
using DevExpress.Office;
#else
using DevExpress.Xpf.Core.Native;
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Utils {
	#region IndexesChecker
	public static class IndicesChecker {
		public static int MaxColumnCount = 16384; 
		public static int MaxRowCount = 1048576; 
		public static int MaxColumnIndex = 16383;
		public static int MaxRowIndex = 1048575;
		public static bool CheckIsColumnIndexValid(int index) {
			return CheckIsColumnIndexValid(index, 0);
		}
		public static bool CheckIsRowIndexValid(int index) {
			return CheckIsRowIndexValid(index, 0);
		}
		public static bool CheckIsColumnIndexValid(int index, int maxColumnCount) {
			return index >= 0 && index < (maxColumnCount > 0 ? maxColumnCount : MaxColumnCount);
		}
		public static bool CheckIsRowIndexValid(int index, int maxRowCount) {
			return index >= 0 && index < (maxRowCount > 0 ? maxRowCount : MaxRowCount);
		}
	}
	#endregion
	#region WildcardComparer
	public static class WildcardComparer {
		static char[] wildcardChars = new char[] { '?', '*' };
		public static bool IsWildcard(string value) {
			return value.IndexOfAny(wildcardChars) >= 0;
		}
		public static bool TryGetMatch(string wildcardString, string compare, out Match match) {
			Regex regex = CreateWildcardRegex(wildcardString);
			match = regex.Match(compare);
			return match.Success;
		}
		public static bool Match(string wildcardString, string compare) {
			return Match(CreateWildcardRegex(wildcardString), compare);
		}
		public static bool Match(Regex regex, string compare) {
			Match match = regex.Match(compare);
			if (match != null) {
				int matchLength = match.Length;
				if (matchLength == compare.Length)
					return match.Success;
				if (matchLength < compare.Length) {
					if (compare[matchLength] == '\r' || compare[matchLength] == '\n')
						return match.Success;
					if (match.Index > 0 && (compare[match.Index - 1] == '\r' || compare[match.Index - 1] == '\n'))
						return match.Success;
				}
				return false;
			}
			return false;
		}
		const int maxRegexCacheCapacity = 20;
		[ThreadStatic]
		static Dictionary<string, Regex> cachedRegexTable;
		static Dictionary<string, Regex> CachedRegexTable {
			get {
				if (cachedRegexTable == null)
					cachedRegexTable = new Dictionary<string, Regex>();
				return cachedRegexTable;
			}
		}
		public static Regex CreateWildcardRegex(string wildcardString) {
			Regex result;
			Dictionary<string, Regex> cache = CachedRegexTable;
			if (cache.TryGetValue(wildcardString, out result))
				return result;
			string pattern = CreatePattern(wildcardString);
			result = new Regex(pattern, RegexOptions.IgnoreCase);
			if (cache.Count >= maxRegexCacheCapacity)
				cache.Clear();
			cache.Add(wildcardString, result);
			return result;
		}
		static Dictionary<char, string> replacementTable = CreateReplacementTable();
		static Dictionary<char, string> CreateReplacementTable() {
			Dictionary<char, string> result = new Dictionary<char, string>();
			result.Add('*', ".*");
			result.Add('?', ".");
			result.Add('\\', @"\\");
			result.Add('[', @"\[");
			result.Add(']', @"\]");
			result.Add('(', @"\(");
			result.Add(')', @"\)");
			result.Add('{', @"\{");
			result.Add('}', @"\}");
			result.Add('.', @"\.");
			result.Add('+', @"\+");
			result.Add('|', @"\|");
			result.Add('$', @"\$");
			result.Add('^', @"\^");
			return result;
		}
		static string CreatePattern(string wildcardString) {
			StringBuilder stringBuilder = new StringBuilder();
			int count = wildcardString.Length;
			for (int i = 0; i < count; i++) {
				char ch = wildcardString[i];
				string replacement;
				if (replacementTable.TryGetValue(ch, out replacement)) {
					if (i > 0 && wildcardString[i - 1] == '~' && (ch == '*' || ch == '?')) {
						stringBuilder[stringBuilder.Length - 1] = '\\';
						stringBuilder.Append(ch);
					}
					else
						stringBuilder.Append(replacement);
				}
				else
					stringBuilder.Append(ch);
			}
			return stringBuilder.ToString();
		}
	}
	#endregion
	#region SimpleUniqueItemCollection (abstract class)
	public abstract class SimpleUniqueItemCollection<T> : SimpleCollection<T> where T : class {
		public override int Add(T item) {
			Guard.ArgumentNotNull(item, "Item");
			if (this.InnerList.Contains(item))
				return IndexOf(item);
			return base.Add(item);
		}
	}
	#endregion
	public abstract class UniqueItemCollection<T> : SimpleCollection<T> {
		#region Fields
		static object locker = new object();
		Dictionary<T, int> innerDictionary;
		#endregion
		protected UniqueItemCollection() {
			innerDictionary = new Dictionary<T, int>();
		}
		protected UniqueItemCollection(IEqualityComparer<T> comparer) {
			innerDictionary = new Dictionary<T, int>(comparer);
		}
		#region Properties
		#endregion
		protected override void OnItemInserted(int index, T item) {
			base.OnItemInserted(index, item);
			innerDictionary.Add(item, index);
		}
		protected override void OnItemRemoved(int index, T item) {
			base.OnItemRemoved(index, item);
			innerDictionary.Remove(item);
		}
		public override void Clear() {
			base.Clear();
			innerDictionary.Clear();
		}
		public virtual int AddIfNotAlreadyAdded(T item) {
			int existingIndex = -1;
			if (innerDictionary.TryGetValue(item, out existingIndex))
				return existingIndex;
			return Add(item);
		}
		internal int AddIfNotAlreadyAddedThreadSafe(T item) {
			int existingIndex = -1;
			if (innerDictionary.TryGetValue(item, out existingIndex))
				return existingIndex;
			lock (locker) {
				if (!innerDictionary.TryGetValue(item, out existingIndex))
					existingIndex = Add(item);
			}
			return existingIndex;
		}
		public override void Sort(int index, int count, IComparer<T> comparer) {
			throw new ArgumentException("Sort operation for the unique item collection is not implemented yet");
		}
		public override int IndexOf(T item) {
			int index = -1;
			if (innerDictionary.TryGetValue(item, out index))
				return index;
			return -1;
		}
	}
	#region SpreadsheetExceptions
	public static class SpreadsheetExceptions {
		public static void ThrowInvalidOperationException(XtraSpreadsheetStringId id) {
			Exceptions.ThrowInvalidOperationException(XtraSpreadsheetLocalizer.GetString(id));
		}
		public static void ThrowInvalidOperationException(XtraSpreadsheetStringId id, string message) {
			Exceptions.ThrowInvalidOperationException(XtraSpreadsheetLocalizer.GetString(id) + ": " + message);
		}
		public static void ThrowArgumentOutOfRangeException(XtraSpreadsheetStringId id, string parameterName) {
			Exceptions.ThrowArgumentOutOfRangeException(parameterName, XtraSpreadsheetLocalizer.GetString(id));
		}
		public static void ThrowArgumentException(XtraSpreadsheetStringId id) {
			throw new ArgumentException(XtraSpreadsheetLocalizer.GetString(id));
		}
		public static void ThrowArgumentException(XtraSpreadsheetStringId id, string message) {
			throw new ArgumentException(XtraSpreadsheetLocalizer.GetString(id) + ": " + message);
		}
	}
	#endregion
	public static class IntegrityChecks {
		public static void Fail(string message) {
			System.Diagnostics.Debug.WriteLine(message);
			System.Diagnostics.Debug.Assert(false, message, String.Empty);
			throw new Exception("INTEGRITY CHECKS FAILED\r\n" + message);
		}
	}
	public class ChunkedList<T> : IList<T>{
		const int DefaultMaxBufferSize = 4096;
		readonly List<List<T>> lists = new List<List<T>>();
		int totalItemCount;
		public ChunkedList() {
			Initialize();
		}
		public bool IsReadOnly { get { return false; } }
		void Initialize() {
			lists.Add(new List<T>());
			totalItemCount = 0;
		}
		public T this[int index] {
			get {
				if (index < 0 && index >= totalItemCount)
					throw new IndexOutOfRangeException();
				int itemsCount = 0;
				int count = lists.Count;
				for (int i = 0; i < count; i++) {
					int newItemsCount = itemsCount + lists[i].Count;
					if (newItemsCount > index)
						return lists[i][index - itemsCount];
					else
						itemsCount = newItemsCount;
				}
				throw new IndexOutOfRangeException();
			}
			set {
				if (index < 0 && index >= totalItemCount)
					throw new IndexOutOfRangeException();
				int itemsCount = 0;
				int count = lists.Count;
				for (int i = 0; i < count; i++) {
					int newItemsCount = itemsCount + lists[i].Count;
					if (newItemsCount > index) {
						lists[i][index - itemsCount] = value;
						return;
					}
					else
						itemsCount = newItemsCount;
				}
				throw new IndexOutOfRangeException();
			}
		}
		public int Count { get { return totalItemCount; } }
		public void Add(T value) {
			List<T> lastList = lists[lists.Count - 1];
			if (lastList.Count >= DefaultMaxBufferSize) {
				lastList = new List<T>();
				lists.Add(lastList);
			}
			lastList.Add(value);
			totalItemCount++;
		}
		public void RemoveAt(int index) {
			int itemsCount = 0;
			int count = lists.Count;
			for (int i = 0; i < count; i++) {
				int newItemsCount = itemsCount + lists[i].Count;
				if (newItemsCount > index) {
					lists[i].RemoveAt(index - itemsCount);
					totalItemCount--;
					break;
				}
				else
					itemsCount = newItemsCount;
			}
		}
		public void Sort() {
			int count = lists.Count;
			for (int i = 0; i < count; i++)
				lists[i].Sort();
		}
		internal void AddRange(IEnumerable<T> items) {
			foreach (T item in items) {
				Add(item);
			}
		}
		#region IEnumerable<T> Members
		public IEnumerator<T> GetEnumerator() {
			foreach (List<T> innerList in lists) {
				foreach (T item in innerList)
					yield return item;
			}
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return ((IEnumerable<T>)this).GetEnumerator();
		}
		#endregion
		public int IndexOf(T item) {
			foreach (List<T> innerList in lists) {
				int index = innerList.IndexOf(item);
				if (index >= 0)
					return index;
			}
			return - 1;
		}
		#region IList<T> Members
		public void Insert(int index, T item) {
			throw new NotSupportedException();
		}
		#endregion
		#region ICollection<T> Members
		public void Clear() {
			lists.Clear();
			Initialize();
		}
		public bool Contains(T item) {
			foreach (List<T> innerList in lists) {
				if (innerList.Contains(item))
					return true;
			}
			return false;
		}
		public void CopyTo(T[] array, int arrayIndex) {
			throw new ArgumentException("Not implemented");
		}
		public bool Remove(T item) {
			int index = IndexOf(item);
			if (index < 0)
				return false;
			RemoveAt(index);
			return true;
		}
		#endregion
	}
	#region ChunkedDictionary
	[System.Diagnostics.DebuggerDisplay("Count = {Count}")]
	[Serializable]
	[System.Runtime.InteropServices.ComVisible(false)]
	public class ChunkedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary
	{
		struct Entry {
			public int hashCode;	
			public int next;		
			public TKey key;		   
			public TValue value;		 
		}
		int count;
		int version;
		int freeList;
		int freeCount;
		int currentCapacity;
		IEqualityComparer<TKey> comparer;
		Object _syncRoot;
		List<int[]> bucketsList;
		List<Entry[]> entriesList;
		int chunkSize;
		public ChunkedDictionary() : this(2801) { }
		public ChunkedDictionary(int chunkSize) : this(chunkSize, 0) { }
		public ChunkedDictionary(int chunkSize, int capacity) : this(chunkSize, capacity, null) { }
		public ChunkedDictionary(IEqualityComparer<TKey> comparer) : this(2801, 0, comparer) { }
		public ChunkedDictionary(int chunkSize, int capacity, IEqualityComparer<TKey> comparer) {
			if (capacity < 0)
				throw new ArgumentOutOfRangeException("capacity");
			this.chunkSize = chunkSize;
			if (capacity > 0)
				Initialize(capacity);
			this.comparer = comparer ?? EqualityComparer<TKey>.Default;
		}
		#region not implemented Constructors
		#endregion
		public IEqualityComparer<TKey> Comparer { get { return comparer; } }
		public int Count { get { return count - freeCount; } }
		public TValue this[TKey key] {
			get {
				int i = FindEntry(key);
				if (i >= 0)
					return GetEntryByIndex(i).value;
				throw new KeyNotFoundException();
			}
			set {
				Insert(key, value, false);
			}
		}
		object this[object key] {
			get {
				if (IsCompatibleKey(key)) {
					int i = FindEntry((TKey)key);
					if (i >= 0) {
						return GetEntryByIndex(i).value;
					}
				}
				return null;
			}
		}
		object IDictionary.this[object key] {
			get {
				throw new ArgumentException("not implemented");
			}
			set {
				if (key == null)
					throw new ArgumentNullException("key");
				if (value == null)
					throw new ArgumentNullException("value");
				try {
					TKey tempKey = (TKey)key;
					try {
						this[tempKey] = (TValue)value;
					}
					catch (InvalidCastException) {
						throw new ArgumentException("Wrong value type");
					}
				}
				catch (InvalidCastException) {
					throw new ArgumentException("Wrong key type");
				}
			}
		}
		void Initialize(int capacity) {
			currentCapacity = HashHelpers.GetPrime(capacity);
			int chunksCount = CalculateChunkCount(currentCapacity);
			int lastChunkSize = currentCapacity % chunkSize;
			if (lastChunkSize == 0)
				lastChunkSize = chunkSize;
			bucketsList = new List<int[]>(chunksCount);
			entriesList = new List<Entry[]>(chunksCount);
			for (int i = 0; i < chunksCount; i++) {
				int currentChunkSize = i == chunksCount - 1 ? lastChunkSize : chunkSize;
				int[] buckets = new int[currentChunkSize];
				for (int j = 0; j < currentChunkSize; j++)
					buckets[j] = -1;
				bucketsList.Add(buckets);
				Entry[] entries = new Entry[currentChunkSize];
				entriesList.Add(entries);
			}
			count = 0;
			freeList = -1;
		}
		public void Clear() {
			if (count > 0) {
				for (int i = 0; i < bucketsList.Count; i++)
					ClearInnerItem(i);
				freeList = -1;
				count = 0;
				freeCount = 0;
				version++;
			}
		}
		void ClearInnerItem(int index) {
			int[] buckets = bucketsList[index];
			Entry[] entries = entriesList[index];
			for (int i = 0; i < buckets.Length; i++)
				buckets[i] = -1;
			Array.Clear(entries, 0, entries.Length);
		}
		Entry GetEntryByIndex(int index) {
			int listIndex = index / chunkSize;
			Entry[] entries = entriesList[listIndex];
			return entries[index - listIndex * chunkSize];
		}
		void SetEntryByIndex(int index, Entry entry) {
			int listIndex = index / chunkSize;
			Entry[] entries = entriesList[listIndex];
			entries[index - listIndex * chunkSize] = entry;
		}
		int GetBucketByIndex(int index) {
			int listIndex = index / chunkSize;
			int[] bucket = bucketsList[listIndex];
			return bucket[index - listIndex * chunkSize];
		}
		void SetBucketByIndex(int index, int value) {
			int listIndex = index / chunkSize;
			int[] bucket = bucketsList[listIndex];
			bucket[index - listIndex * chunkSize] = value;
		}
		public bool ContainsKey(TKey key) {
			return FindEntry(key) >= 0;
		}
		public bool ContainsValue(TValue value) {
			if (value == null) {
				for (int i = 0; i < count; i++) {
					Entry entry = GetEntryByIndex(i);
					if (entry.hashCode >= 0 && entry.value == null)
						return true;
				}
			}
			else {
				EqualityComparer<TValue> c = EqualityComparer<TValue>.Default;
				for (int i = 0; i < count; i++) {
					Entry entry = GetEntryByIndex(i);
					if (entry.hashCode >= 0 && c.Equals(entry.value, value))
						return true;
				}
			}
			return false;
		}
		public int FindEntry(TKey key) {
			if (key == null) {
				throw new ArgumentNullException("key");
			}
			if (bucketsList != null) {
				int hashCode = comparer.GetHashCode(key) & 0x7FFFFFFF;
				int startIndex = GetBucketByIndex(hashCode % currentCapacity);
				Entry currentEntry;
				for (int i = startIndex; i >= 0; i = currentEntry.next) {
					currentEntry = GetEntryByIndex(i);
					if (currentEntry.hashCode == hashCode && comparer.Equals(currentEntry.key, key))
						return i;
				}
			}
			return -1;
		}
		public void Insert(TKey key, TValue value, bool add) {
			if (key == null)
				throw new ArgumentNullException("key");
			if (bucketsList == null)
				Initialize(0);
			int hashCode = comparer.GetHashCode(key) & 0x7FFFFFFF;
			int targetBucket = hashCode % currentCapacity;
			Entry currentEntry;
			for (int i = GetBucketByIndex(targetBucket); i >= 0; i = currentEntry.next) {
				currentEntry = GetEntryByIndex(i);
				if (currentEntry.hashCode == hashCode && comparer.Equals(currentEntry.key, key)) {
					if (add)
						throw new ArgumentException("duplicate");
					currentEntry.value = value;
					version++;
					return;
				}
			}
			InsertCore(key, value, hashCode, targetBucket);
		}
		public void InsertCore(TKey key, TValue value, int hashCode, int targetBucket) {
			int index;
			if (freeCount > 0) {
				index = freeList;
				freeList = GetEntryByIndex(index).next;
				freeCount--;
			}
			else {
				if (count >= currentCapacity) {
					Resize();
					targetBucket = hashCode % currentCapacity;
				}
				index = count;
				count++;
			}
			Entry entry;
			entry.hashCode = hashCode;
			entry.next = GetBucketByIndex(targetBucket);
			entry.key = key;
			entry.value = value;
			SetEntryByIndex(index, entry);
			SetBucketByIndex(targetBucket, index);
			version++;
		}
		int CalculateChunkCount(int capacity) {
			int result = capacity / chunkSize;
			if (capacity % chunkSize > 0)
				result++;
			return result;
		}
		private void Resize() {
			Resize(HashHelpers.ExpandPrime(count), false);
		}
		void RecalculateHashCodes() {
			for (int j = 0; j < entriesList.Count; j++) {
				Entry[] entries = entriesList[j];
				for (int i = 0; i < entries.Length; i++) {
					if (entries[i].hashCode != -1)
						entries[i].hashCode = (comparer.GetHashCode(entries[i].key) & 0x7FFFFFFF);
				}
			}
		}
		void Resize(int newSize, bool forceNewHashCodes) {
			System.Diagnostics.Debug.Assert(newSize >= currentCapacity);
			if (forceNewHashCodes)
				throw new ArgumentException("Not implemented");
			int newChunksCount = CalculateChunkCount(newSize);
			int startChunksCount = entriesList.Count;
			int lastChunkStartingSize = count % chunkSize;
			if (lastChunkStartingSize == 0)
				lastChunkStartingSize = chunkSize;
			int lastChunkNewSize = newSize % chunkSize;
			if (lastChunkNewSize == 0)
				lastChunkNewSize = chunkSize;
			if (newChunksCount <= startChunksCount) {
				bucketsList[startChunksCount - 1] = new int[lastChunkNewSize];
				Entry[] newEntries = new Entry[lastChunkNewSize];
				Array.Copy(entriesList[entriesList.Count - 1], 0, newEntries, 0, lastChunkStartingSize);
				entriesList[entriesList.Count - 1] = newEntries;
			}
			else {
				Entry[] existingEntries = entriesList[startChunksCount - 1];
				if (existingEntries.Length < chunkSize) {
					Entry[] newEntries = new Entry[chunkSize];
					Array.Copy(existingEntries, 0, newEntries, 0, existingEntries.Length);
					entriesList[startChunksCount - 1] = newEntries;
					bucketsList[startChunksCount - 1] = new int[chunkSize];
				}
				int itemsToAdd = newChunksCount - startChunksCount;
				for (int i = 0; i < itemsToAdd; i++) {
					int currentBucketSize = i == itemsToAdd - 1 ? lastChunkNewSize : chunkSize;
					bucketsList.Add(new int[currentBucketSize]);
					entriesList.Add(new Entry[currentBucketSize]);
				}
			}
			for (int i = 0; i < bucketsList.Count; i++) {
				int[] currentBuckets = bucketsList[i];
				int currentBucketSize = currentBuckets.Length;
				for (int j = 0; j < currentBuckets.Length; j++)
					currentBuckets[j] = -1;
			}
			if (forceNewHashCodes)
				RecalculateHashCodes();
			for (int j = 0; j < startChunksCount; j++) {
				Entry[] currentEntries = entriesList[j];
				int currentChunkSize = j == startChunksCount - 1 ? lastChunkStartingSize : chunkSize;
				for (int i = 0; i < currentChunkSize; i++) {
					int bucket = currentEntries[i].hashCode % newSize;
					currentEntries[i].next = GetBucketByIndex(bucket);
					SetBucketByIndex(bucket, i + j * chunkSize);
				}
			}
			currentCapacity = newSize;
		}
		public bool TryGetValue(TKey key, out TValue value) {
			int i = FindEntry(key);
			if (i >= 0) {
				value = GetEntryByIndex(i).value;
				return true;
			}
			value = default(TValue);
			return false;
		}
		public bool TryGetOrAdd(TKey key, TValue addValue, out TValue value) {
			int i = FindEntry(key);
			if (i >= 0) {
				value = GetEntryByIndex(i).value;
				return true;
			}
			if (key == null) {
				throw new ArgumentNullException("key");
			}
			if (bucketsList == null)
				Initialize(0);
			int hashCode = comparer.GetHashCode(key) & 0x7FFFFFFF;
			int targetBucket = hashCode % currentCapacity;
			InsertCore(key, addValue, hashCode, targetBucket);
			value = addValue;
			return false;
		}
		public void Add(TKey key, TValue value) {
			Insert(key, value, true);
		}
		public bool Remove(TKey key) {
			throw new ArgumentException("not implemented");
		}
		static bool IsCompatibleKey(object key) {
			if (key == null)
				throw new ArgumentNullException("key");
			return (key is TKey);
		}
		#region ICollection members
		object ICollection.SyncRoot {
			get {
				if (_syncRoot == null) {
					System.Threading.Interlocked.CompareExchange<Object>(ref _syncRoot, new Object(), null);
				}
				return _syncRoot;
			}
		}
		bool ICollection.IsSynchronized { get { return false; } }
		bool IDictionary.IsFixedSize { get { return false; } }
		bool IDictionary.IsReadOnly { get { return false; } }
		void ICollection.CopyTo(Array array, int index) {
			throw new ArgumentException("not implemented");
		}
		#endregion
		#region ICollection<KeyValuePair<TKey, TValue>> members
		bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly {
			get { return false; }
		}
		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int index) {
			CopyTo(array, index);
		}
		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> keyValuePair) {
			Add(keyValuePair.Key, keyValuePair.Value);
		}
		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> keyValuePair) {
			throw new ArgumentException("not implemented");
		}
		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> keyValuePair) {
			throw new ArgumentException("not implemented");
		}
		#endregion
		#region IDictionary members
		void IDictionary.Add(object key, object value) {
			if (key == null) {
				throw new ArgumentNullException("key");
			}
			if (value == null)
				throw new ArgumentNullException("value");
			try {
				TKey tempKey = (TKey)key;
				try {
					Add(tempKey, (TValue)value);
				}
				catch (InvalidCastException) {
					throw new ArgumentException("Wrong value type");
				}
			}
			catch (InvalidCastException) {
				throw new ArgumentException("Wrong key type");
			}
		}
		bool IDictionary.Contains(object key) {
			if (IsCompatibleKey(key)) {
				return ContainsKey((TKey)key);
			}
			return false;
		}
		void IDictionary.Remove(object key) {
			if (IsCompatibleKey(key)) {
				Remove((TKey)key);
			}
		}
		#endregion
		#region Not implemented
		IEnumerator IEnumerable.GetEnumerator() {
			throw new ArgumentException("not implemented");
		}
		IDictionaryEnumerator IDictionary.GetEnumerator() {
			throw new ArgumentException("not implemented");
		}
		ICollection IDictionary.Keys {
			get {
				throw new ArgumentException("not implemented");
			}
		}
		ICollection IDictionary.Values {
			get {
				throw new ArgumentException("not implemented");
			}
		}
		ICollection<TKey> IDictionary<TKey, TValue>.Keys {
			get {
				throw new ArgumentException("not implemented");
			}
		}
		ICollection<TValue> IDictionary<TKey, TValue>.Values {
			get {
				throw new ArgumentException("not implemented");
			}
		}
		private void CopyTo(KeyValuePair<TKey, TValue>[] array, int index) {
			throw new ArgumentException("not implemented");
		}
		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() {
			throw new ArgumentException("not implemented");
		}
		#endregion
	}
	#region HashHelpers
	internal static class HashHelpers {
		public static readonly int[] primes = {
			3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919,
			1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591,
			17519, 21023, 25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437,
			187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403, 968897, 1162687, 1395263,
			1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369};
#if !DXPORTABLE
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
#endif
		public static bool IsPrime(int candidate) {
			if ((candidate & 1) != 0) {
				int limit = (int)Math.Sqrt(candidate);
				for (int divisor = 3; divisor <= limit; divisor += 2) {
					if ((candidate % divisor) == 0)
						return false;
				}
				return true;
			}
			return (candidate == 2);
		}
#if !DXPORTABLE
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
#endif
		public static int GetPrime(int min) {
			if (min < 0)
				throw new ArgumentException("Arg_HTCapacityOverflow");
			for (int i = 0; i < primes.Length; i++) {
				int prime = primes[i];
				if (prime >= min)
					return prime;
			}
			for (int i = (min | 1); i < Int32.MaxValue; i += 2) {
				if (IsPrime(i) && ((i - 1) % 101 != 0))
					return i;
			}
			return min;
		}
		public static int GetMinPrime() {
			return primes[0];
		}
		public static int ExpandPrime(int oldSize) {
			int newSize = 2 * oldSize;
			if ((uint)newSize > MaxPrimeArrayLength && MaxPrimeArrayLength > oldSize) {
				return MaxPrimeArrayLength;
			}
			return GetPrime(newSize);
		}
		public const int MaxPrimeArrayLength = 0x7FEFFFFD;
	}
	#endregion
	#endregion
	#region Chunked dictionary old realization
	#endregion
	#region ProtectionUtils
	public struct PasswordVerifierCalculator {
		[CLSCompliant(false)]
		public UInt16 Calculate(byte[] passwordBytes) {
			ushort wPasswordHash;
			wPasswordHash = 0;
			int length = passwordBytes.Length;
			if (length > 0) {
				for (int i = length - 1; i >= 0; i--) {
					wPasswordHash = (ushort)(((wPasswordHash >> 14) & 0x01) | ((wPasswordHash << 1) & 0x7fff));
					wPasswordHash ^= passwordBytes[i];
				}
				wPasswordHash = (ushort)(((wPasswordHash >> 14) & 0x01) | ((wPasswordHash << 1) & 0x7fff));
				wPasswordHash ^= (ushort)length;
				wPasswordHash ^= (0x8000 | ('N' << 8) | 'K');
			}
			return (ushort)wPasswordHash;
		}
		[CLSCompliant(false)]
		public UInt16 Calculate(string password) {
			Encoding encodingForCurrentWindowsCodePage = DXEncoding.Default.Clone() as Encoding;
#if !SL && !DXPORTABLE
			encodingForCurrentWindowsCodePage.EncoderFallback = EncoderFallback.ReplacementFallback;
			encodingForCurrentWindowsCodePage.DecoderFallback = DecoderFallback.ReplacementFallback;
#endif
			byte[] passwordBytes = encodingForCurrentWindowsCodePage.GetBytes(password);
			return Calculate(passwordBytes);
		}
	}
	#endregion
	#region SpreadsheetPasswordHashCodeCalculator
	public class SpreadsheetPasswordHashCalculator {
		public byte[] CalculatePasswordHashSpreadsheet(string password, byte[] prefix, int hashCount, HashAlgorithmType hashAlgorithmType) {
			HashAlgorithm hashAlgorithm = CreateHashAlgorithm(hashAlgorithmType);
			if (hashAlgorithm == null) {
				throw new NotSupportedException(string.Format("{0} hash algorithm is not supported", hashAlgorithm.ToString()));
			}
			using (hashAlgorithm) {
				hashAlgorithm.Initialize();
				return CalculatePasswordHashSpreadsheet(password, prefix, hashCount, hashAlgorithm);
			}
		}
		public byte[] CalculatePasswordHashSpreadsheet(string password, byte[] prefixSalt, int iterations, HashAlgorithm hashAlgorithm) {
			byte[] passwordBytes = Encoding.Unicode.GetBytes(password);
			return CalculatePasswordHashCore(passwordBytes, prefixSalt, iterations, hashAlgorithm);
		}
		protected internal byte[] CalculatePasswordHashCore(byte[] passwordBytes, byte[] prefix, int hashCount, HashAlgorithm hashAlgorithm) {
			byte[] bytes = Concatenate(prefix, passwordBytes);
			int i = 0;
			for (; ; ) {
				bytes = hashAlgorithm.ComputeHash(bytes);
				if (i < hashCount) {
					bytes = Concatenate(bytes, i);
					i++;
				}
				else
					return bytes;
			}
		}
		public byte[] GenerateSalt(int length) {
			byte[] result = new byte[length];
			RandomNumberGenerator provider = RandomNumberGenerator.Create();
#if !SL && !DXPORTABLE
			provider.GetNonZeroBytes(result);
#else
			provider.GetBytes(result);
#endif
			return result;
		}
#if DXPORTABLE
		protected internal virtual HashAlgorithm CreateHashAlgorithm(HashAlgorithmType hashAlgorithmType) {
			switch (hashAlgorithmType) {
				case HashAlgorithmType.None:
				case HashAlgorithmType.Mac:
				case HashAlgorithmType.HMac:
				case HashAlgorithmType.Ripemd:
				case HashAlgorithmType.Md2:
				case HashAlgorithmType.Md4:
				default:
					return null;
				case HashAlgorithmType.Sha1:
					return SHA1.Create();
				case HashAlgorithmType.Sha256:
					return SHA256.Create();
				case HashAlgorithmType.Sha384:
					return SHA384.Create();
				case HashAlgorithmType.Sha512:
					return SHA512.Create();
				case HashAlgorithmType.Md5:
					return MD5.Create();
			}
		}
#else
		protected internal virtual HashAlgorithm CreateHashAlgorithm(HashAlgorithmType hashAlgorithmType) {
			switch (hashAlgorithmType) {
				case HashAlgorithmType.None:
				case HashAlgorithmType.Mac:
				case HashAlgorithmType.HMac:
				case HashAlgorithmType.Ripemd:
				case HashAlgorithmType.Md2:
				case HashAlgorithmType.Md4:
				default:
					return null;
				case HashAlgorithmType.Sha1:
					return new SHA1Managed();
				case HashAlgorithmType.Sha256:
					return new SHA256Managed();
#if !SL
				case HashAlgorithmType.Sha384:
					return new SHA384Managed();
				case HashAlgorithmType.Sha512:
					return new SHA512Managed();
				case HashAlgorithmType.Md5:
					return new MD5CryptoServiceProvider();
				case HashAlgorithmType.Ripemd160:
					return new RIPEMD160Managed();
#endif
			}
		}
#endif
		public byte[] Concatenate(byte[] b1, byte[] b2) { 
			if (b1 == null)
				return b2;
			if (b2 == null)
				return b1;
			byte[] result = new byte[b1.Length + b2.Length];
			Array.Copy(b1, 0, result, 0, b1.Length);
			Array.Copy(b2, 0, result, b1.Length, b2.Length);
			return result;
		}
		public byte[] Concatenate(byte[] bytes, int num) { 
			byte[] result = new byte[bytes.Length + 4];
			byte[] countBytes = new byte[4];
			countBytes[3] = (byte)((num & 0xFF000000) >> 24);
			countBytes[2] = (byte)((num & 0x00FF0000) >> 16);
			countBytes[1] = (byte)((num & 0x0000FF00) >> 8);
			countBytes[0] = (byte)((num & 0x000000FF));
			Array.Copy(bytes, 0, result, 0, bytes.Length);
			Array.Copy(countBytes, 0, result, bytes.Length, countBytes.Length);
			return result;
		}
		public static bool CompareByteArrays(byte[] b1, byte[] b2) {
			if (Object.ReferenceEquals(b1, b2))
				return true;
			if (b1 == null || b2 == null)
				return false;
			if (b1.Length != b2.Length)
				return false;
			int count = b1.Length;
			for (int i = 0; i < count; i++)
				if (b1[i] != b2[i])
					return false;
			return true;
		}
	}
	#endregion
	#region HashAlgorithmType
	public enum HashAlgorithmType {
		None = 0,
		Md2 = 1,
		Md4 = 2,
		Md5 = 3,
		Sha1 = 4,
		Mac = 5,
		Ripemd = 6,
		Ripemd160 = 7,
		HMac = 9, 
		Sha256 = 12,
		Sha384 = 13,
		Sha512 = 14,
		Whirlpool = 15, 
	}
	#endregion
	public static class DoubleComparer {
		const long FRAC_MASK = 0x000FFFFFFFFFFFFFL;
		public static int Compare(double a, double b) {
			if (AreEqualUlpsAndAbsAndRel(a, b, 1, 1e-15, 1e-15))
				return 0;
			return Comparer<double>.Default.Compare(a, b);
		}
		public static bool AreEqual(double a, double b) {
			return AreEqualUlpsAndAbsAndRel(a, b, 1, 1e-15, 1e-15);
		}
		public static bool AreEqualUlpsAndRel(double a, double b, int maxUlpsDiff, double maxRelDiff) {
			if (double.IsNaN(a) || double.IsInfinity(a) || double.IsNaN(b) || double.IsInfinity(b))
				return Comparer<double>.Default.Compare(a, b) == 0;
			if (AreEqualUlpsCore(a, b, maxUlpsDiff))
				return true;
			return AreEqualRelCore(a, b, maxRelDiff);
		}
		static bool AreEqualAbsCore(double a, double b, double maxAbsDiff) {
			double absDiff = Math.Abs(a - b);
			return absDiff <= maxAbsDiff;
		}
		static bool AreEqualRelCore(double a, double b, double maxRelDiff) {
			double absDiff = Math.Abs(a - b);
			double largest = Math.Max(Math.Abs(a), Math.Abs(b));
			return absDiff < largest * maxRelDiff;
		}
		static bool AreEqualUlpsCore(double a, double b, int maxUlpsDiff) {
			long bitsA = BitConverter.DoubleToInt64Bits(a);
			bool negativeA = (bitsA < 0);
			long bitsB = BitConverter.DoubleToInt64Bits(b);
			bool negativeB = (bitsB < 0);
			if (negativeA != negativeB)
				return Comparer<double>.Default.Compare(a, b) == 0;
			long ulpsDiff = Math.Abs(bitsA - bitsB);
			return ulpsDiff <= maxUlpsDiff;
		}
		static bool AreEqualUlpsAndAbsAndRel(double a, double b, int maxUlpsDiff, double maxAbsDiff, double maxRelDiff) {
			if (double.IsNaN(a) || double.IsInfinity(a) || double.IsNaN(b) || double.IsInfinity(b))
				return Comparer<double>.Default.Compare(a, b) == 0;
			if (AreEqualAbsCore(a, b, maxAbsDiff))
				return true;
			if (AreEqualUlpsCore(a, b, maxUlpsDiff))
				return true;
			return AreEqualRelCore(a, b, maxRelDiff);
		}
	}
	public static class DoubleConverter {
		public static bool TryParse15(string s, NumberStyles style, IFormatProvider provider, out double result) {
			if (double.TryParse(s, style, provider, out result)) {
				result = Math.Round(result, 15);
				return true;
			}
			else
				return false;
		}
		public static string ToExactString(double d) {
			if (double.IsPositiveInfinity(d))
				return "+Infinity";
			if (double.IsNegativeInfinity(d))
				return "-Infinity";
			if (double.IsNaN(d))
				return "NaN";
			long bits = BitConverter.DoubleToInt64Bits(d);
			bool negative = (bits < 0);
			int exponent = (int)((bits >> 52) & 0x7ffL);
			long mantissa = bits & 0xfffffffffffffL;
			if (exponent == 0) {
				exponent++;
			}
			else {
				mantissa = mantissa | (1L << 52);
			}
			exponent -= 1075;
			if (mantissa == 0) {
				return "0";
			}
			while ((mantissa & 1) == 0) {	
				mantissa >>= 1;
				exponent++;
			}
			ArbitraryDecimal ad = new ArbitraryDecimal(mantissa);
			if (exponent < 0) {
				for (int i = 0; i < -exponent; i++)
					ad.MultiplyBy(5);
				ad.Shift(-exponent);
			}
			else {
				for (int i = 0; i < exponent; i++)
					ad.MultiplyBy(2);
			}
			if (negative)
				return "-" + ad.ToString();
			else
				return ad.ToString();
		}
		class ArbitraryDecimal {
			byte[] digits;
			int decimalPoint = 0;
			internal ArbitraryDecimal(long x) {
				string tmp = x.ToString(System.Globalization.CultureInfo.InvariantCulture);
				digits = new byte[tmp.Length];
				for (int i = 0; i < tmp.Length; i++)
					digits[i] = (byte)(tmp[i] - '0');
				Normalize();
			}
			internal void MultiplyBy(int amount) {
				byte[] result = new byte[digits.Length + 1];
				for (int i = digits.Length - 1; i >= 0; i--) {
					int resultDigit = digits[i] * amount + result[i + 1];
					result[i] = (byte)(resultDigit / 10);
					result[i + 1] = (byte)(resultDigit % 10);
				}
				if (result[0] != 0) {
					digits = result;
				}
				else {
					Array.Copy(result, 1, digits, 0, digits.Length);
				}
				Normalize();
			}
			internal void Shift(int amount) {
				decimalPoint += amount;
			}
			internal void Normalize() {
				int first;
				for (first = 0; first < digits.Length; first++)
					if (digits[first] != 0)
						break;
				int last;
				for (last = digits.Length - 1; last >= 0; last--)
					if (digits[last] != 0)
						break;
				if (first == 0 && last == digits.Length - 1)
					return;
				byte[] tmp = new byte[last - first + 1];
				for (int i = 0; i < tmp.Length; i++)
					tmp[i] = digits[i + first];
				decimalPoint -= digits.Length - (last + 1);
				digits = tmp;
			}
			public override String ToString() {
				char[] digitString = new char[digits.Length];
				for (int i = 0; i < digits.Length; i++)
					digitString[i] = (char)(digits[i] + '0');
				if (decimalPoint == 0) {
					return new string(digitString);
				}
				if (decimalPoint < 0) {
					return new string(digitString) +
						   new string('0', -decimalPoint);
				}
				if (decimalPoint >= digitString.Length) {
					return "0." +
						new string('0', (decimalPoint - digitString.Length)) +
						new string(digitString);
				}
				return new string(digitString, 0,
								   digitString.Length - decimalPoint) +
					"." +
					new string(digitString,
								digitString.Length - decimalPoint,
								decimalPoint);
			}
		}
	}
	public struct DateTimeInfo {
		static readonly DateTimeInfo empty = new DateTimeInfo();
		public static DateTimeInfo Empty { get { return empty; } }
		public DateTime Value { get; set; }
		public string Format { get; set; }
	}
	public static class DateTimeUtils {
		public static bool TryParseByParts(string textValue, CultureInfo culture, out DateTime dateTime) {
			dateTime = DateTime.MinValue;
			string[] parts = textValue.Split(' ');
			int count = parts.Length - 1;
			if (count <= 0)
				return false;
			for (int i = 0; i < count; i++) {
				string firstPart = CreatePart(parts, 0, i);
				string secondPart = CreatePart(parts, i + 1, count);
				DateTime date;
				DateTime time;
				if (DateTimeParanoicParser.DateTimeTryParse(firstPart, culture, DevExpress.XtraSpreadsheet.Model.WorkbookDataContext.defaultFlags, out date) &&
					DateTimeParanoicParser.DateTimeTryParse(secondPart, culture, DevExpress.XtraSpreadsheet.Model.WorkbookDataContext.defaultFlags, out time)) {
					if (date == date.Date && time.Date == DateTime.MinValue && ValidateDesignator(secondPart, time, culture))
						dateTime = new DateTime(Math.Max(dateTime.Ticks, date.Ticks + time.Ticks));
				}
			}
			return dateTime != DateTime.MinValue;
		}
		static string CreatePart(string[] parts, int from, int to) {
			StringBuilder result = new StringBuilder();
			for (int i = from; i <= to; i++) {
				if (result.Length > 0)
					result.Append(' ');
				result.Append(parts[i]);
			}
			return result.ToString();
		}
		public static bool ValidateDesignator(string textValue, DateTime dateTime, CultureInfo culture) {
			if (dateTime.TimeOfDay < TimeSpan.FromHours(13))
				return true;
			DateTimeFormatInfo format = culture.DateTimeFormat;
			return ValidateAMDesignatorCore(textValue, format.AMDesignator, dateTime) &&
				ValidatePMDesignatorCore(textValue, format.PMDesignator, dateTime, culture);
		}
		static bool ValidateAMDesignatorCore(string textValue, string designator, DateTime dateTime) {
			if (!String.IsNullOrEmpty(designator))
				if (textValue.Trim().EndsWith(designator))
					return false;
			return true;
		}
		static bool ValidatePMDesignatorCore(string textValue, string designator, DateTime dateTime, CultureInfo culture) {
			if (String.IsNullOrEmpty(designator) || !textValue.Trim().EndsWith(designator))
				return true;
			string textWithoutDesignator = textValue.Substring(0, textValue.Length - designator.Length).Trim();
			DateTime dateTimeWithoutDesignator;
			if (!DateTime.TryParse(textWithoutDesignator, culture, DevExpress.XtraSpreadsheet.Model.WorkbookDataContext.defaultFlags, out dateTimeWithoutDesignator)) {
				char timeSeparator;
				if (Char.TryParse(GetTimeSeparator(culture), out timeSeparator)) {
					if (!textValue.Contains(timeSeparator.ToString())) {
						int hour;
						string hourValues = textWithoutDesignator.Substring(textWithoutDesignator.LastIndexOf(" ") + 1);
						if (Int32.TryParse(hourValues, out hour))
							return hour < 13;
					}
				}
				return false;
			}
			return dateTimeWithoutDesignator != dateTime;
		}
		static string GetTimeSeparator(CultureInfo culture) {
#if!SL
			return culture.GetTimeSeparator();
#else
			return ":";
#endif
		}
	}
	internal class DateTimeParanoicParser {
		readonly static Dictionary<int, string> timePartTable = CreateTimePartTable();
		static Dictionary<int, string> CreateTimePartTable() {
			Dictionary<int, string> result = new Dictionary<int, string>();
			result.Add(0, "hh:mm:ss tt");
			result.Add(1, "h:mm:ss tt");
			result.Add(2, "hh:mm:ss");
			result.Add(3, "h:mm:ss");
			result.Add(4, "hh:mm tt");
			result.Add(5, "h:mm tt");
			result.Add(6, "hh:mm");
			result.Add(7, "h:mm");
			result.Add(8, "hh tt");
			result.Add(9, "h tt");
			return result;
		}
		readonly Dictionary<int, Regex[]> regexCache = new Dictionary<int, Regex[]>();
		internal bool TryParse(string s, DateTimeStyles styles, Model.WorkbookDataContext context, out DateTimeInfo result) {
			return TryParseDateTimeParanoid(s, styles, context, out result);
		}
		public string TryDetectFormat(DateTime value, string formattedValue, Model.WorkbookDataContext context) {
			string result = TryDetectFormat(value, formattedValue, context.Culture);
			if (String.IsNullOrEmpty(result))
				result = TryDetectFormat(value, formattedValue, CultureInfo.InvariantCulture);
			if (String.IsNullOrEmpty(result))
				result = TryDetectFormat(value, formattedValue, new CultureInfo("en-US"));
			return result;
		}
		string TryDetectFormat(DateTime value, string formattedValue, CultureInfo culture) {
			formattedValue = CleanupString(formattedValue);
			if (String.IsNullOrEmpty(formattedValue))
				return String.Empty;
			string[] patterns = GetPatterns(culture);
			int count = patterns.Length;
			for (int i = 0; i < count; i++) {
				string text = CleanupString(value.ToString(patterns[i], culture));
				if (String.Compare(text, formattedValue, StringComparison.OrdinalIgnoreCase) == 0)
					return patterns[i];
			}
			return String.Empty;
		}
		string CleanupString(string value) {
			return value.Replace(" ", "");
		}
		bool TryParseDateTimeParanoid(string value, DateTimeStyles styles, Model.WorkbookDataContext context, out DateTimeInfo result) {
			CultureInfo culture = context.Culture;
			result = DateTimeInfo.Empty;
			string[] patterns = GetPatterns(culture);
			int count = patterns.Length;
			double maxNumberResult = -1;
			DateTimeInfo maxResult = DateTimeInfo.Empty;
			for (int i = 0; i < count; i++) {
				if (TryParseDateTimeParanoid(value, context.Culture, styles, patterns[i], out result)) {
					double numberResult = ConvertToNumber(result.Value, context);
					if (maxNumberResult < numberResult)
						maxNumberResult = numberResult;
					if (maxResult.Value < result.Value)
						maxResult = result;
				}
			}
			result = maxResult;
			return maxNumberResult > 0;
		}
		double ConvertToNumber(DateTime dateTime, Model.WorkbookDataContext context) {
			DateTime baseDate = context.DateSystem == Model.DateSystem.Date1900 ? Model.VariantValue.BaseDate : Model.VariantValue.BaseDate1904;
			if (dateTime.Date < baseDate.Date) {
				TimeSpan span = TimeSpan.FromTicks(dateTime.Ticks);
				dateTime = baseDate + span;
				if (context.DateSystem == Model.DateSystem.Date1900 && dateTime.Date < baseDate.Date.AddDays(2))
					dateTime = dateTime.AddDays(2);
			}
			Model.VariantValue value = new Model.VariantValue();
			value.SetDateTime(dateTime, context);
			return value.NumericValue;
		}
		public static string[] GetAllDateTimePatterns(CultureInfo culture) {
			return GetPatterns(culture);
		}
#if !SL && !DXRESTRICTED
		[ThreadStatic]
		static Dictionary<DateTimeFormatInfo, string[]> allDateTimePatternsTable;
		static string[] GetPatterns(CultureInfo culture) {
			DateTimeFormatInfo formatInfo = DateTimeFormatInfo.GetInstance(culture);
			string[] result;
			if (allDateTimePatternsTable == null)
				allDateTimePatternsTable = new Dictionary<DateTimeFormatInfo, string[]>();
			if (!allDateTimePatternsTable.TryGetValue(formatInfo, out result)) {
				result = formatInfo.GetAllDateTimePatterns();
				allDateTimePatternsTable.Add(formatInfo, result);
			}
			return result;
		}
#else
		static string[] GetPatterns(CultureInfo culture) {
			return culture.GetAllDateTimePatterns();
		}
#endif
		bool TryParseDateTimeParanoid(string value, CultureInfo culture, DateTimeStyles styles, string pattern, out DateTimeInfo result) {
			DateTime dateTime = DateTime.MinValue;
			result = DateTimeInfo.Empty;
			if (DateTime.TryParseExact(value, pattern, culture, styles, out dateTime)) {
				result.Value = dateTime;
				result.Format = pattern;
				return true;
			}
			int timePartStart = -1;
			int timePartNumber = -1;
			foreach (int keys in timePartTable.Keys) {
				timePartStart = pattern.IndexOfInvariantCultureIgnoreCase(timePartTable[keys]);
				if (timePartStart > 0) {
					timePartNumber = keys;
					break;
				}
			}
			if (timePartStart < 0)
				return false;
			string[] parts = value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			if (parts == null)
				return false;
			if (parts.Length == 1) {
				TimeSpan timeOnly;
				if (!TryParseTimeParanoid(parts[0], culture, false, out timeOnly, timePartNumber))
					return false;
				dateTime += timeOnly;
				result.Value = dateTime;
				result.Format = ReplaceTimeDesignator(timePartTable[timePartNumber]); 
				return true;
			}
			bool forcePmDesignator = false;
			if (parts.Length == 3 && StringExtensions.CompareInvariantCultureIgnoreCase(parts[2], culture.DateTimeFormat.PMDesignator) == 0)
				forcePmDesignator = true;
			else if (parts.Length != 2)
				return false;
			if (!DateTimeTryParse(parts[0], culture, styles, out dateTime) || dateTime.TimeOfDay != TimeSpan.Zero)
				return false;
			TimeSpan time;
			if (!TryParseTimeParanoid(parts[1], culture, forcePmDesignator, out time, timePartNumber))
				return false;
			dateTime += time;
			result.Value = dateTime;
			result.Format = ReplaceTimeDesignator(timePartTable[timePartNumber]);
			return true;
		}
		internal static bool DateTimeTryParse(string text, CultureInfo culture, DateTimeStyles styles, out DateTime dateTime) {
#if !DXPORTABLE
			return DateTime.TryParse(text, culture, styles, out dateTime);
#else
			if (DateTime.TryParse(text, culture, styles, out dateTime))
				return true;
			string correctedText = TryReplaceAbbreviatedMonthNames(text, culture);
			if (text != correctedText)
				return DateTime.TryParse(correctedText, culture, styles, out dateTime);
			return false;
#endif
		}
#if DXPORTABLE
		static string TryReplaceAbbreviatedMonthNames(string text, CultureInfo culture) {
			if (culture == null)
				return text;
			DateTimeFormatInfo dtfi = culture.DateTimeFormat;
			text = TryReplaceMonthNames(text, culture, dtfi.AbbreviatedMonthGenitiveNames, dtfi.MonthGenitiveNames);
			text = TryReplaceMonthNames(text, culture, dtfi.AbbreviatedMonthNames, dtfi.MonthNames);
			return text;
		}
		static string TryReplaceMonthNames(string text, CultureInfo culture, string[] what, string[] with) {
			if (what.Length != with.Length)
				return text;
			int count = what.Length;
			for (int i = 0; i < count; i++)
				text = TryReplaceMonthNames(text, culture, what[i], with[i]);
			return text;
		}
		static string TryReplaceMonthNames(string text, CultureInfo culture, string what, string with) {
			if (String.IsNullOrEmpty(what))
				return text;
			if (what.Length > 3)
				what = what.Substring(0, 3);
			int from = 0;
			for (;;) {
				int index = culture.CompareInfo.IndexOf(text, what, from, CompareOptions.IgnoreCase);
				if (index < 0)
					return text;
				if (IsSeparateWord(text, index, what.Length)) {
					text = text.Remove(index, what.Length);
					text = text.Insert(index, with);
					from = index + with.Length;
				}
				else
					from = index + what.Length;
				if (from >= text.Length)
					return text;
			}
		}
		static bool IsSeparateWord(string text, int from, int length) {
			return !IsLetter(text, from - 1) && !IsLetter(text, from + length);
		}
		static bool IsLetter(string text, int index) {
			char ch = GetCharAtSafe(text, index);
			if (ch == ' ' || ch == '.' || ch == '-' || ch == '/' || ch == ':')
				return false;
			UnicodeCategory category = ch.GetUnicodeCategory();
			if (category == UnicodeCategory.LowercaseLetter || category == UnicodeCategory.OtherLetter || category == UnicodeCategory.TitlecaseLetter || category == UnicodeCategory.UppercaseLetter)
				return true;
			return false;
		}
		static char GetCharAtSafe(string text, int index) {
			if (index >= 0 && index < text.Length)
				return text[index];
			return ' ';
		}
#endif
		string ReplaceTimeDesignator(string value) {
			return value.Replace(" tt", " AM/PM");
		}
		bool TryParseTimeParanoid(string value, CultureInfo culture, bool forcePmDesignator, out TimeSpan result, int timePartNumber) {
			result = TimeSpan.Zero;
#if !SL
			string timeSeparator = culture.GetTimeSeparator();
#else
			string timeSeparator = ":";
#endif
			if (timeSeparator == ".")
				timeSeparator = @"\.";
			if (IsInvalidTimeFormat(value, timeSeparator))
				return false;
			Regex regex = GetRegex(timeSeparator, timePartNumber);
			Match match = regex.Match(value);
			if (match == null || !match.Success)
				return false;
			TimeSpan time = TimeSpan.Zero;
			Group group = match.Groups["hours"];
			if (group != null && group.Success) {
				int hours;
				if (!Int32.TryParse(value.Substring(group.Index, group.Length), out hours))
					return false;
				if (hours < 0)
					return false;
				time += TimeSpan.FromHours(hours);
			}
			group = match.Groups["minutes"];
			if (group != null && group.Success) {
				int minutes;
				if (!Int32.TryParse(value.Substring(group.Index, group.Length), out minutes))
					return false;
				if (minutes < 0)
					return false;
				time += TimeSpan.FromMinutes(minutes);
			}
			group = match.Groups["seconds"];
			if (group != null && group.Success) {
				int seconds;
				if (!Int32.TryParse(value.Substring(group.Index, group.Length), out seconds))
					return false;
				if (seconds < 0)
					return false;
				time += TimeSpan.FromSeconds(seconds);
			}
			if (forcePmDesignator && time < TimeSpan.FromHours(13)) {
				if (time > TimeSpan.FromHours(12))
					time -= TimeSpan.FromHours(12);
				else
					time += TimeSpan.FromHours(12);
			}
			result = time;
			return true;
		}
		bool IsInvalidTimeFormat(string value, string timeSeparator) {
			char timeSeparatorAsChar;
			if (Char.TryParse(timeSeparator, out timeSeparatorAsChar)) {
				string[] timePart = value.Split(timeSeparatorAsChar);
				if (timePart.Length > 3 && timePart[3].Length != 0)
					return true;
			}
			return false;
		}
		Regex GetRegex(string timeSeparator, int timePartNumber) {
			int patternIndex = GetRegexPatternIndex(timeSeparator, timePartNumber);
			int hashCode = timeSeparator.GetHashCode() ^ patternIndex;
			Regex[] regexArray;
			if (!regexCache.TryGetValue(hashCode, out regexArray)) {
				regexArray = new Regex[3];
				regexCache.Add(hashCode, regexArray);
			}
			if (regexArray[patternIndex] == null)
				regexArray[patternIndex] = new Regex(GetRegexPattern(timeSeparator, timePartNumber));
			return regexArray[patternIndex];
		}
		int GetRegexPatternIndex(string timeSeparator, int timePartNumber) {
			if (timePartNumber >= 0 && timePartNumber <= 3)
				return 0;
			if (timePartNumber >= 4 && timePartNumber <= 7)
				return 1;
			return 2;
		}
		string GetRegexPattern(string timeSeparator, int timePartNumber) {
			if (timePartNumber >= 0 && timePartNumber <= 3)
				return @"^(?<hours>\d+)" + timeSeparator + @"(?<minutes>\d+)" + timeSeparator + @"(?<seconds>\d+)";
			if (timePartNumber >= 4 && timePartNumber <= 7)
				return @"^(?<hours>\d+)" + timeSeparator + @"(?<minutes>\d+)?";
			return @"^(?<hours>\d+)";
		}
	}
	public interface IShiftableEnumerator {
		void OnObjectInserted(int insertedItemValueOrder);
		void OnObjectDeleted(int deletedItemValueOrder);
	}
	#region IOrderedEnumerator<T>
	public interface IOrderedEnumerator<T> : IEnumerator<T>, IShiftableEnumerator {
		int CurrentValueOrder { get; }
		bool IsReverseOrder { get; }
	}
	#endregion
	#region IOrderedEnumerator<T>
	public interface IOrderedItemsRangeEnumerator<T> : IOrderedEnumerator<T> {
		IList<T> Items { get; }
		int NearItemIndex { get; }
		int FarItemIndex { get; }
		int ActualFirstIndex { get; }
		int ActualLastIndex { get; }
		int DebugInnerIndex { get; }
		bool ShouldCalculateActualIndices();
		bool CalculateActualIndices();
	}
	#endregion
	#region JoinedOrderedEnumerator
	public class JoinedOrderedEnumerator<T> : IOrderedEnumerator<T> {
		readonly IOrderedEnumerator<T> primaryEnumerator;
		readonly IOrderedEnumerator<T> secondaryEnumerator;
		IOrderedEnumerator<T> activeEnumerator;
		bool skipMovePrimary;
		bool skipMoveSecondary;
		bool lastMovePrimaryResult;
		bool lastMoveSecondaryResult;
		public JoinedOrderedEnumerator(IOrderedEnumerator<T> primaryEnumerator, IOrderedEnumerator<T> secondaryEnumerator) {
			Guard.ArgumentNotNull(primaryEnumerator, "primaryEnumerator");
			Guard.ArgumentNotNull(secondaryEnumerator, "secondaryEnumerator");
			this.primaryEnumerator = primaryEnumerator;
			this.secondaryEnumerator = secondaryEnumerator;
			System.Diagnostics.Debug.Assert(primaryEnumerator.IsReverseOrder == secondaryEnumerator.IsReverseOrder);
		}
		#region IEnumerator<T> Members
		public T Current { get { return GetCurrent(); } }
		#endregion
		#region IDisposable Members
		public void Dispose() {
			IDisposable disposable = primaryEnumerator as IDisposable;
			if (disposable != null)
				disposable.Dispose();
			disposable = secondaryEnumerator as IDisposable;
			if (disposable != null)
				disposable.Dispose();
		}
		#endregion
		#region IEnumerator Members
		object IEnumerator.Current { get { return GetCurrent(); } }
		public bool MoveNext() {
			lastMovePrimaryResult = skipMovePrimary ? lastMovePrimaryResult : primaryEnumerator.MoveNext();
			lastMoveSecondaryResult = skipMoveSecondary ? lastMoveSecondaryResult : secondaryEnumerator.MoveNext();
			if (lastMovePrimaryResult == false && lastMoveSecondaryResult == false)
				return false;
			this.skipMovePrimary = false;
			this.skipMoveSecondary = false;
			if (!lastMovePrimaryResult) {
				this.activeEnumerator = secondaryEnumerator;
				this.skipMovePrimary = true;
				return lastMoveSecondaryResult;
			}
			if (!lastMoveSecondaryResult) {
				this.activeEnumerator = primaryEnumerator;
				this.skipMoveSecondary = true;
				return lastMovePrimaryResult;
			}
			int sign = primaryEnumerator.CurrentValueOrder - secondaryEnumerator.CurrentValueOrder;
			if (IsReverseOrder)
				sign *= -1;
			if (sign == 0)
				this.activeEnumerator = primaryEnumerator;
			else if (sign < 0) {
				this.activeEnumerator = primaryEnumerator;
				this.skipMoveSecondary = true;
			}
			else {
				this.activeEnumerator = secondaryEnumerator;
				this.skipMovePrimary = true;
			}
			return true;
		}
		public void Reset() {
			primaryEnumerator.Reset();
			secondaryEnumerator.Reset();
			this.skipMovePrimary = false;
			this.skipMoveSecondary = false;
		}
		T GetCurrent() {
			return activeEnumerator.Current;
		}
		#endregion
		#region IOrderedEnumerator<T> Members
		int IOrderedEnumerator<T>.CurrentValueOrder { get { return activeEnumerator.CurrentValueOrder; } }
		public bool IsReverseOrder { get { return primaryEnumerator.IsReverseOrder; } }
		#endregion
		void IShiftableEnumerator.OnObjectInserted(int insertedItemValueOrder) {
			primaryEnumerator.OnObjectInserted(insertedItemValueOrder);
			secondaryEnumerator.OnObjectInserted(insertedItemValueOrder);
		}
		void IShiftableEnumerator.OnObjectDeleted(int itemValueOrder) {
			primaryEnumerator.OnObjectDeleted(itemValueOrder);
			secondaryEnumerator.OnObjectDeleted(itemValueOrder);
		}
	}
	#endregion
	#region Enumerable<T>
	public class Enumerable<T> : IEnumerable<T> {
		bool used = false;
		readonly IEnumerator<T> enumerator;
		[System.Diagnostics.DebuggerStepThrough]
		public Enumerable(IEnumerator<T> enumerator) {
			this.enumerator = enumerator;
		}
		[System.Diagnostics.DebuggerStepThrough]
		public IEnumerator<T> GetEnumerator() {
			return GetEnumeratorCore();
		}
		[System.Diagnostics.DebuggerStepThrough]
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumeratorCore();
		}
		[System.Diagnostics.DebuggerStepThrough]
		protected virtual IEnumerator<T> GetEnumeratorCore() {
			if (used)
				enumerator.Reset();
			used = true;
			return enumerator;
		}
	}
	#endregion
	#region OrderedItemsRangeEnumerator<T> (abstract class)
	public abstract class OrderedItemsRangeEnumerator<T> : IOrderedItemsRangeEnumerator<T> {
		readonly IList<T> items;
		int nearItemIndex;
		int farItemIndex;
		readonly Predicate<T> filter;
		int actualFirstIndex = Int32.MaxValue;
		int actualLastIndex = Int32.MinValue;
		int index;
		int step;
		int borderIndex;
		T currentItem;
		protected OrderedItemsRangeEnumerator(IList<T> items, int nearItemIndex, int farItemIndex, bool reverseOrder, Predicate<T> filter) {
			Guard.ArgumentNonNegative(farItemIndex - nearItemIndex, "nearItemIndex > farItemIndex");
			this.items = items;
			this.nearItemIndex = nearItemIndex;
			this.farItemIndex = farItemIndex;
			this.step = (reverseOrder ? -1 : 1);
			this.filter = filter;
			Reset();
		}
		public IList<T> Items { get { return items; } }
		public int NearItemIndex { get { return nearItemIndex; } }
		public int FarItemIndex { get { return farItemIndex; } }
		public int ActualFirstIndex { get { return actualFirstIndex; } }
		public int ActualLastIndex { get { return actualLastIndex; } }
		public bool IsReverseOrder { get { return this.step == -1; } }
		public int DebugInnerIndex { get { return index; } }
		#region IEnumerator<T> Members
		T IEnumerator<T>.Current { get { return GetItem(index); } }
		#endregion
		#region IDisposable Members
		void IDisposable.Dispose() {
		}
		#endregion
		#region IEnumerator Members
		object IEnumerator.Current { get { return GetItem(index); } }
		bool IEnumerator.MoveNext() {
			this.currentItem = default(T);
			if (ShouldCalculateActualIndices()) {
				if (!CalculateActualIndices())
					return false;
			}
			for (; ; ) {
				index += step;
				if (index == borderIndex)
					return false;
				if (IsValidItem(index) && (filter == null || filter(GetItemCore(index))))
					break;
			}
			return true;
		}
		void IEnumerator.Reset() {
			this.Reset();
		}
		#endregion
		protected virtual void Reset() {
			ResetIndex();
			this.currentItem = default(T);
		}
		protected virtual bool IsValidItem(int index) {
			return true;
		}
		void ResetIndex() {
			if (step > 0)
				index = actualFirstIndex - 1;
			else
				index = actualLastIndex + 1;
		}
		public bool ShouldCalculateActualIndices() {
			return actualFirstIndex > actualLastIndex;
		}
		public bool CalculateActualIndices() {
			actualFirstIndex = CalculateActualFirstIndex();
			if (actualFirstIndex < 0) {
				actualFirstIndex = ~actualFirstIndex;
				if (actualFirstIndex >= items.Count || actualFirstIndex > farItemIndex)
					return false;
			}
			actualLastIndex = CalculateActualLastIndex();
			if (actualLastIndex < 0) {
				actualLastIndex = ~actualLastIndex;
				if (actualLastIndex >= items.Count)
					actualLastIndex = items.Count - 1;
				else
					actualLastIndex--;
				if (actualLastIndex >= 0) {
					actualLastIndex = ValidateActualLastIndex(farItemIndex, actualLastIndex);
				}
			}
			if (step > 0)
				borderIndex = actualLastIndex + 1;
			else
				borderIndex = actualFirstIndex - 1;
			ResetIndex();
			return true;
		}
		protected virtual int CalculateActualFirstIndex() {
			return NearItemIndex;
		}
		protected virtual int CalculateActualLastIndex() {
			return FarItemIndex;
		}
		protected virtual int ValidateActualLastIndex(int farItemIndex, int actualLastIndex) {
			return actualLastIndex;
		}
		protected T GetItem(int index) {
			if (Object.Equals(currentItem, default(T)))
				currentItem = GetItemCore(index);
			return currentItem;
		}
		protected virtual T GetItemCore(int index) {
			return items[index];
		}
		protected internal abstract int GetCurrentValueOrder(int itemIndex);
		#region IOrderedEnumerator<T> Members
		public int CurrentValueOrder { get { return GetCurrentValueOrder(index); } }
		#endregion
		public virtual void OnObjectInserted(int insertedItemValueOrder) {
			bool valid = !ShouldCalculateActualIndices();
			currentItem = default(T);
			if (!valid) {
				if (!IsReverseOrder)
					nearItemIndex = insertedItemValueOrder + 1; 
				else
					farItemIndex = insertedItemValueOrder - 1;
				return;
			}
			if (!IsReverseOrder) {
				index += step;
				borderIndex += step;  
			}
		}
		public virtual void OnObjectDeleted(int deletedItemValueOrder) {
			if (IsReverseOrder && index >= deletedItemValueOrder) {
				currentItem = default(T);
				index += step; 
				borderIndex += step;  
			}
			else if (!IsReverseOrder) {
				currentItem = default(T); 
				index -= step; 
				borderIndex -= step;  
			}
		}
	}
	#endregion
	#region SparseOrderedItemsRangeEnumerator<T> (abstract class)
	public abstract class SparseOrderedItemsRangeEnumerator<T> : OrderedItemsRangeEnumerator<T> {
		protected SparseOrderedItemsRangeEnumerator(IList<T> items, int nearItemIndex, int farItemIndex, bool reverseOrder, Predicate<T> filter)
			: base(items, nearItemIndex, farItemIndex, reverseOrder, filter) {
		}
		protected override int CalculateActualFirstIndex() {
			return Algorithms.BinarySearch(Items, CreateComparable(NearItemIndex));
		}
		protected override int CalculateActualLastIndex() {
			return Algorithms.BinarySearch(Items, CreateComparable(FarItemIndex), ActualFirstIndex + 1, Items.Count - 1);
		}
		protected override int ValidateActualLastIndex(int farItemIndex, int actualLastIndex) {
			if (CreateComparable(farItemIndex).CompareTo(Items[actualLastIndex]) > 0)
				return actualLastIndex - 1;
			else
				return actualLastIndex;
		}
		protected internal abstract IComparable<T> CreateComparable(int itemIndex);
	}
	#endregion
	#region EnumeratorConverter<TSourceType, TTargetType>
	public delegate TTargetType ConvertMethod<TSourceType, TTargetType>(TSourceType value);
	public class EnumeratorConverter<TSourceType, TTargetType> : IEnumerator<TTargetType> {
		readonly IEnumerator<TSourceType> enumerator;
		readonly ConvertMethod<TSourceType, TTargetType> convert;
		TTargetType current;
		public EnumeratorConverter(IEnumerator<TSourceType> enumerator, ConvertMethod<TSourceType, TTargetType> convert) {
			Guard.ArgumentNotNull(enumerator, "enumerator");
			Guard.ArgumentNotNull(convert, "convert");
			this.enumerator = enumerator;
			this.convert = convert;
		}
		TTargetType IEnumerator<TTargetType>.Current { get { return current; } }
		void IDisposable.Dispose() {
			enumerator.Dispose();
		}
		object IEnumerator.Current { get { return current; } }
		bool IEnumerator.MoveNext() {
			bool result = enumerator.MoveNext();
			if (result)
				current = convert(enumerator.Current);
			return result;
		}
		void IEnumerator.Reset() {
			enumerator.Reset();
			current = default(TTargetType);
		}
	}
	public class OrderedEnumeratorConverter<TSourceType, TTargetType> : IOrderedEnumerator<TTargetType> {
		readonly IOrderedEnumerator<TSourceType> enumerator;
		readonly ConvertMethod<TSourceType, TTargetType> convert;
		TTargetType current;
		public OrderedEnumeratorConverter(IOrderedEnumerator<TSourceType> enumerator, ConvertMethod<TSourceType, TTargetType> convert) {
			Guard.ArgumentNotNull(enumerator, "enumerator");
			Guard.ArgumentNotNull(convert, "convert");
			this.enumerator = enumerator;
			this.convert = convert;
		}
		protected IOrderedEnumerator<TSourceType> Enumerator { get { return enumerator; } }
		protected ConvertMethod<TSourceType, TTargetType> Convert { get { return convert; } }
		TTargetType IEnumerator<TTargetType>.Current { get { return current; } }
		void IDisposable.Dispose() {
			enumerator.Dispose();
		}
		object IEnumerator.Current { get { return current; } }
		bool IEnumerator.MoveNext() {
			bool result = enumerator.MoveNext();
			if (result)
				current = convert(enumerator.Current);
			return result;
		}
		void IEnumerator.Reset() {
			enumerator.Reset();
			current = default(TTargetType);
		}
		public int CurrentValueOrder { get { return enumerator.CurrentValueOrder; } }
		public bool IsReverseOrder { get { return enumerator.IsReverseOrder; } }
		public void OnObjectInserted(int insertedItemValueOrder) {
			enumerator.OnObjectInserted(insertedItemValueOrder);
		}
		public void OnObjectDeleted(int index) {
			enumerator.OnObjectDeleted(index);
		}
	}
	public class OrderedItemsRangeEnumeratorConverter<TSourceType, TTargetType> : OrderedEnumeratorConverter<TSourceType, TTargetType>, IOrderedItemsRangeEnumerator<TTargetType> {
		public OrderedItemsRangeEnumeratorConverter(IOrderedItemsRangeEnumerator<TSourceType> enumerator, ConvertMethod<TSourceType, TTargetType> convert)
			: base(enumerator, convert) {
		}
		protected new IOrderedItemsRangeEnumerator<TSourceType> Enumerator { get { return (IOrderedItemsRangeEnumerator<TSourceType>)base.Enumerator; } }
		public IList<TTargetType> Items {
			get { return new DynamicListWrapper<TSourceType, TTargetType>(Enumerator.Items, Convert); }
		}
		public int NearItemIndex { get { return Enumerator.NearItemIndex; } }
		public int FarItemIndex { get { return Enumerator.FarItemIndex; } }
		public int ActualFirstIndex { get { return Enumerator.ActualFirstIndex; } }
		public int ActualLastIndex { get { return Enumerator.ActualLastIndex; } }
		public int DebugInnerIndex { get { return Enumerator.DebugInnerIndex; } }
		public bool ShouldCalculateActualIndices() {
			return Enumerator.ShouldCalculateActualIndices();
		}
		public bool CalculateActualIndices() {
			return Enumerator.CalculateActualIndices();
		}
	}
	#endregion
	#region DXMath
	public static class DXMath {
		public static decimal Round(decimal value) {
#if !SL
			return Math.Round(value, MidpointRounding.AwayFromZero);
#else
			return Math.Round(value);
#endif
		}
		public static double Round(double value) {
#if !SL
			return Math.Round(value, MidpointRounding.AwayFromZero);
#else
			return Math.Round(value);
#endif
		}
	}
	#endregion
	#region DxStack
	public class DxStack<T> : Stack<T> {
		public DxStack() {
		}
		public DxStack(IEnumerable<T> collection)
			: base(collection) {
		}
		public DxStack<T> Clone() {
			int count = Count;
			T[] array = new T[count];
			this.CopyTo(array, 0);
			DxStack<T> clone = new DxStack<T>();
			for (int i = count - 1; i >= 0; i--)
				clone.Push(array[i]);
			return clone;
		}
	}
	#endregion
	#region PackedValues helper
	static class PackedValues {
		public static bool GetBoolBitValue(uint packedValues, uint mask) {
			return (packedValues & mask) != 0;
		}
		public static void SetBoolBitValue(ref uint packedValues, uint mask, bool value) {
			if (value)
				packedValues |= mask;
			else
				packedValues &= ~mask;
		}
		public static void SetBoolBitValue(ref byte packedValues, byte mask, bool value) {
			if (value)
				packedValues |= mask;
			else
				packedValues &= (byte)~mask;
		}
		public static int GetIntBitValue(uint packedValues, uint mask) {
			return (int)(packedValues & mask);
		}
		public static int GetIntBitValue(uint packedValues, uint mask, int offset) {
			return (int)((packedValues & mask) >> offset);
		}
		public static void SetIntBitValue(ref uint packedValues, uint mask, int value) {
			packedValues &= ~mask;
			packedValues |= (uint)value & mask;
		}
		public static void SetIntBitValue(ref uint packedValues, uint mask, int offset, int value) {
			packedValues &= ~mask;
			packedValues |= ((uint)value << offset) & mask;
		}
		public static void SetIntBitValue(ref byte packedValues, byte mask, byte offset, byte value) {
			packedValues &= (byte)~mask;
			packedValues |= (byte)((value << offset) & mask);
		}
	}
	#endregion
	#region RTree
	public static class LinkedListExtensions {
		public static void addAll<T>(this LinkedList<T> where, LinkedList<T> what) {
			foreach (T value in what)
				where.AddLast(value);
		}
		public static void addAll<T>(this HashSet<T> where, LinkedList<T> what) {
			foreach (T value in what)
				where.Add(value);
		}
	}
	public static class HashSetExtensions {
		public static void AddRange<T>(this HashSet<T> where, List<T> what) {
			foreach (T value in what)
				where.Add(value);
		}
	}
	#endregion
	public static class NextObjectNameGenerator {
		public static string GetUniqueName(string existingObjectName, IList<string> otherNames, bool separateBySpace) {
			string result = existingObjectName;
			int maxNum = FindMaxNum(existingObjectName, otherNames, 0);
			if (maxNum > 0) {
				if (separateBySpace)
					result += " ";
				result += maxNum + 1;
			}
			return result;
		}
		public static string GetNameForDuplicate(string existingObjectName, IList<string> otherNames) {
			string result = String.Empty;
			int maxNum = FindMaxNum(existingObjectName, otherNames, 1);
			result = String.Concat(existingObjectName, " ", maxNum + 1);
			return result;
		}
		private static int FindMaxNum(string prefix, IList<string> names, int startValue) {
			int maxNum = startValue;
			int count = names.Count;
			for (int i = 0; i < count; i++) {
				string item = names[i];
				if (item.StartsWith(prefix)) {
					if ((item.Length != prefix.Length)) {
						string end = item.Substring(prefix.Length, item.Length - prefix.Length);
						int parsedNum;
						if (Int32.TryParse(end, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedNum))
							maxNum = Math.Max(maxNum, parsedNum);
					}
					else if (maxNum == 0)
						maxNum = 1;
				}
			}
			return maxNum;
		}
		public static string GetNameForNextObject(string prefix, IList<string> otherNames) {
			string result = String.Empty;
			int maxNum = FindMaxNum(prefix, otherNames, 0);
			result = String.Concat(prefix, maxNum + 1);
			return result;
		}
	}
	public static class IListExtensions {
		public static void RemoveRange<T>(this IList<T> list, int startIndex, int count) {
			for (int i = startIndex + count - 1; i >= startIndex; i--)
				list.RemoveAt(i);
		}
		public static void InsertRange<T>(this IList<T> list, int index, IList<T> items) {
			int count = items.Count;
			for (int i = 0; i < count; i++)
				list.Insert(index + i, items[i]);
		}
		public static void ForEach<T>(this IList<T> list, Action<T> action) {
			foreach (T item in list)
				action(item);
		}
	}
	#region DynamicListWrapper
	public class DynamicListWrapper<TSourceType, TTargetType> : IList<TTargetType> {
		readonly IList<TSourceType> innerList;
		readonly ConvertMethod<TSourceType, TTargetType> convert;
		public DynamicListWrapper(IList<TSourceType> innerList, ConvertMethod<TSourceType, TTargetType> convert) {
			this.innerList = innerList;
			this.convert = convert;
		}
		#region IList<TTargetType> Members
		public int IndexOf(TTargetType item) {
			throw new InvalidOperationException();
		}
		public void Insert(int index, TTargetType item) {
			throw new InvalidOperationException();
		}
		public void RemoveAt(int index) {
			throw new InvalidOperationException();
		}
		public TTargetType this[int index] {
			get {
				return convert(innerList[index]);
			}
			set {
				throw new InvalidOperationException();
			}
		}
		#endregion
		#region ICollection<T> Members
		public int Count { get { return innerList.Count; } }
		public bool IsReadOnly { get { return true; } }
		public void Add(TTargetType item) {
			throw new InvalidOperationException();
		}
		public void Clear() {
			throw new InvalidOperationException();
		}
		public bool Contains(TTargetType item) {
			throw new InvalidOperationException();
		}
		public void CopyTo(TTargetType[] array, int arrayIndex) {
			throw new InvalidOperationException();
		}
		public bool Remove(TTargetType item) {
			throw new InvalidOperationException();
		}
		#endregion
		#region IEnumerable<TTargetType> Members
		public IEnumerator<TTargetType> GetEnumerator() {
			foreach (TSourceType item in innerList)
				yield return convert(item);
		}
		#endregion
		#region IEnumerable Members
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return ((IEnumerable<TTargetType>)this).GetEnumerator();
		}
		#endregion
	}
	#endregion
	public static class ColorExtensions {
		public static Color RemoveTransparency(this Color value) {
			if (DXColor.IsTransparentOrEmpty(value))
				return DXColor.Empty;
			if (value.A != 255)
				return DXColor.FromArgb(255, value.R, value.G, value.B);
			return value;
		}
	}
}
