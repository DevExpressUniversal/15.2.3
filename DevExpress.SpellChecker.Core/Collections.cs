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
using System.ComponentModel;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
#if !WPF
using System.Drawing;
#endif
using DevExpress.XtraSpellChecker;
using DevExpress.XtraSpellChecker.Parser;
using DevExpress.XtraSpellChecker.Strategies;
using DevExpress.XtraSpellChecker.Rules;
using System.Security.Permissions;
#if !SL && !WPF
using DevExpress.Utils.Design;
using System.Web;
#else
using DevExpress.Xpf.Core;
#endif
#if WPF
using System.Windows.Shapes;
#endif
namespace DevExpress.XtraSpellChecker {
#if !SL && !WPF
	[ListBindable(BindableSupport.No)]
	[TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter))]
	[AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
#endif
	public class DictionaryCollection : ICollection<ISpellCheckerDictionary>, ICollection, IList
#if !SL
, ICollectionEditorSupport
#endif
 {
		static readonly DictionaryCollection empty = new DictionaryCollection();
		List<ISpellCheckerDictionary> list = new List<ISpellCheckerDictionary>();
		public DictionaryCollection() {
		}
		public ISpellCheckerDictionary this[int index] { get { return list[index] as ISpellCheckerDictionary; } }
#if !SL
	[DevExpressSpellCheckerCoreLocalizedDescription("DictionaryCollectionEmpty")]
#endif
		public static DictionaryCollection Empty { get { return empty; } }
		#region Changed event
		EventHandler onChanged;
		internal event EventHandler Changed { add { onChanged += value; } remove { onChanged -= value; } }
		void RaiseChanged() {
			EventHandler handler = onChanged;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
		#endregion
		public int IndexOf(ISpellCheckerDictionary dictionary) {
			return list.IndexOf(dictionary);
		}
		public virtual void Add(ISpellCheckerDictionary dictionary) {
			if (!CanAddItem(dictionary))
				return;
			list.Add(dictionary);
			RaiseChanged();
		}
		protected virtual bool CanAddItem(object item) {
			ISpellCheckerDictionary dictionary = (ISpellCheckerDictionary)item;
			if (dictionary == null)
				return false;
			SpellCheckerDictionary spellCheckerDictionary = dictionary as SpellCheckerDictionary;
			if (spellCheckerDictionary != null)
				return String.IsNullOrEmpty(spellCheckerDictionary.DictionaryPath) || !list.Contains(dictionary);
			else
				if (item is SpellCheckerISpellDictionary)
					return !list.Contains(dictionary);
			return true;
		}
		public void AddRange(ICollection c) {
			foreach (ISpellCheckerDictionary item in c)
				Add(item);
		}
		public virtual void Remove(ISpellCheckerDictionary dictionary) {
			if (!list.Contains(dictionary))
				return;
			list.Remove(dictionary);
			RaiseChanged();
		}
#if !SL
	[DevExpressSpellCheckerCoreLocalizedDescription("DictionaryCollectionCount")]
#endif
		public int Count { get { return list.Count; } }
		public void Clear() {
			list.Clear();
			RaiseChanged();
		}
		public void RemoveAt(int index) {
			list.RemoveAt(index);
			RaiseChanged();
		}
#if !SL
		#region ICollectionEditorSupport Members
		void ICollectionEditorSupport.ReplaceItems(object[] items) {
			list.Clear();
			if (items == null)
				return;
			foreach (ISpellCheckerDictionary dictionary in items)
				Add(dictionary);
		}
		#endregion
#endif
		#region ICollection<SpellCheckerDictionaryBase> Members
		public bool Contains(ISpellCheckerDictionary item) {
			return list.Contains(item);
		}
		public void CopyTo(ISpellCheckerDictionary[] array, int arrayIndex) {
			list.CopyTo(array, arrayIndex);
		}
#if !SL
	[DevExpressSpellCheckerCoreLocalizedDescription("DictionaryCollectionIsReadOnly")]
#endif
		public bool IsReadOnly { get { return false; } }
		bool ICollection<ISpellCheckerDictionary>.Remove(ISpellCheckerDictionary item) {
			if (list.Contains(item))
				return list.Remove(item);
			else
				return false;
		}
		#endregion
		#region IEnumerable<SpellCheckerDictionaryBase> Members
		public IEnumerator<ISpellCheckerDictionary> GetEnumerator() {
			return list.GetEnumerator();
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return list.GetEnumerator();
		}
		#endregion
		#region ICollection Members
		void ICollection.CopyTo(Array array, int index) {
			Array.Copy(list.ToArray(), 0, array, index, list.Count);
		}
		bool ICollection.IsSynchronized {
			get {
				ICollection collection = list;
				return collection.IsSynchronized;
			}
		}
		object ICollection.SyncRoot {
			get {
				ICollection collection = list;
				return collection.SyncRoot;
			}
		}
		#endregion
		#region IList Members
		int IList.Add(object value) {
			IList listImplementation = this.list;
			return listImplementation.Add(value);
		}
		bool IList.Contains(object value) {
			IList listImplementation = this.list;
			return listImplementation.Contains(value);
		}
		int IList.IndexOf(object value) {
			IList listImplementation = this.list;
			return listImplementation.IndexOf(value);
		}
		void IList.Insert(int index, object value) {
			IList listImplementation = this.list;
			listImplementation.Insert(index, value);
		}
		bool IList.IsFixedSize {
			get {
				IList listImplementation = this.list;
				return listImplementation.IsFixedSize;
			}
		}
		void IList.Remove(object value) {
			IList listImplementation = this.list;
			listImplementation.Remove(value);
		}
		object IList.this[int index] {
			get {
				IList listImplementation = this.list;
				return listImplementation[index];
			}
			set {
				IList listImplementation = this.list;
				listImplementation[index] = value;
			}
		}
		#endregion
	}
	public class StringCollection : IEnumerable<string> {
		List<string> list = new List<string>();
		public string this[int index] {
			get { return list[index] as string; }
			set { list[index] = value; }
		}
		#region ICollection
		public void CopyTo(string[] array, int index) {
			list.CopyTo(array, index);
		}
		#endregion
		#region IList
		public void Add(string text) {
			list.Add(text);
		}
		public bool Contains(string text) {
			return list.Contains(text);
		}
		public int IndexOf(string text) {
			return list.IndexOf(text);
		}
		public void Insert(int index, string text) {
			list.Insert(index, text);
		}
		public void Remove(string text) {
			list.Remove(text);
		}
		#endregion
		public void Sort(IComparer<string> comparer) {
			list.Sort(comparer);
		}
		public int BinarySearch(string word, IComparer<string> comparer) {
			return list.BinarySearch(word, comparer);
		}
		public void AddRange(IEnumerable<string> collection) {
			list.AddRange(collection);
		}
		public int Count { get { return list.Count; } }
		public void Clear() { list.Clear(); }
		#region IEnumerable<string> Members
		public IEnumerator<string> GetEnumerator() {
			return list.GetEnumerator();
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return list.GetEnumerator();
		}
		#endregion
	}
	public class SuggestionCollection : IEnumerable<SuggestionBase> {
		List<SuggestionBase> list = new List<SuggestionBase>();
		public SuggestionBase this[int index] {
			get { return list[index] as SuggestionBase; }
			set { list[index] = value; }
		}
		#region ICollection
		public void CopyTo(SuggestionBase[] array, int index) {
			list.CopyTo(array, index);
		}
		#endregion
		#region IList
		public void Add(SuggestionBase suggestion) {
			list.Add(suggestion);
		}
		public bool Contains(SuggestionBase suggestion) {
			return list.Contains(suggestion);
		}
		public bool Contains(string suggestion) {
			for (int i = 0; i < list.Count; i++)
				if (this[i].Suggestion == suggestion)
					return true;
			return false;
		}
		public int IndexOf(SuggestionBase suggestion) {
			for (int i = 0; i < list.Count; i++)
				if (this[i].Suggestion == suggestion.Suggestion)
					return i;
			return -1;
		}
		public void Insert(int index, SuggestionBase suggestion) {
			list.Insert(index, suggestion);
		}
		public void Remove(SuggestionBase suggestion) {
			int index = IndexOf(suggestion);
			if (index >= 0)
				list.RemoveAt(index);
		}
		public void RemoveAt(int index) {
			list.RemoveAt(index);
		}
		public void AddRange(IEnumerable<SuggestionBase> collection) {
			list.AddRange(collection);
		}
		#endregion
		public void Sort(IComparer<SuggestionBase> comparer) {
			list.Sort(comparer);
		}
		internal void Sort(Comparison<SuggestionBase> comparison) {
			list.Sort(comparison);
		}
		internal List<SuggestionBase> FindAll(Predicate<SuggestionBase> predicate) {
			return this.list.FindAll(predicate);
		}
		internal void ForEach(Action<SuggestionBase> action) {
			this.list.ForEach(action);
		}
		public int BinarySearch(SuggestionBase item, IComparer<SuggestionBase> comparer) {
			return list.BinarySearch(item, comparer);
		}
		public void Merge(SuggestionCollection collection) {
			for (int i = 0; i < collection.Count; i++)
				if (!Contains(collection[i].Suggestion))
					Add(collection[i]);
		}
		internal SuggestionCollection GetRange(int index) {
			SuggestionCollection result = new SuggestionCollection();
			result.AddRange(this.list.GetRange(index, this.list.Count - index));
			return result;
		}
		public void Clear() {
			this.list.Clear();
		}
		public string[] ToStringArray() {
			string[] result = new string[Count];
			for (int i = 0; i < Count; i++)
				result[i] = this[i].Suggestion;
			return result;
		}
#if !SL
	[DevExpressSpellCheckerCoreLocalizedDescription("SuggestionCollectionCount")]
#endif
		public int Count { get { return list.Count; } }
		#region IEnumerable<SuggestionBase> Members
		public IEnumerator<SuggestionBase> GetEnumerator() {
			return list.GetEnumerator();
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return list.GetEnumerator();
		}
		#endregion
	}
	public struct IgnoreListItem : IIgnoreItem {
		Position start;
		Position end;
		string word;
		bool always;
		public Position Start { get { return start; } set { start = value; } }
		public Position End { get { return end; } set { end = value; } }
		public string Word { get { return word; } set { word = value; } }
		public bool Always { get { return always; } set { always = value; } }
	}
	public class IgnoreList : IIgnoreList {
		List<IgnoreListItem> list = new List<IgnoreListItem>();
		[Obsolete("This constructor has become obsolete. Use the default constructor.", true)]
		public IgnoreList(CultureInfo culture) { }
		public IgnoreList() { }
		public void Add(Position start, Position end, string word) {
			IgnoreListItem item = new IgnoreListItem();
			item.Start = start;
			item.End = end;
			item.Word = word;
			list.Add(item);
		}
		public void Add(string word) {
			IgnoreListItem item = new IgnoreListItem();
			item.Start = Position.Null;
			item.End = Position.Null;
			item.Word = word;
			item.Always = true;
			list.Add(item);
		}
		public bool Contains(Position start, Position end, string word) {
			foreach (IgnoreListItem item in list) {
				if (item.Always && CompareWords(item.Word, word))
					return true;
				if (Position.Equals(item.Start, start) && Position.Equals(item.End, end) && CompareWords(item.Word, word))
					return true;
			}
			return false;
		}
		bool CompareWords(string word1, string word2) {
#if !SL
			return String.Compare(word1, word2, StringComparison.Ordinal) == 0;
#else
			return String.Compare(word1, word2, Culture, CompareOptions.None) == 0;
#endif
		}
		public bool Contains(string word) {
			foreach (IgnoreListItem item in list)
				if (item.Always && CompareWords(item.Word, word))
					return true;
			return false;
		}
		public void Remove(string word) {
			for (int i = 0; i < list.Count; i++)
				if (list[i].Always && CompareWords(list[i].Word, word)) {
					list.RemoveAt(i);
					break;
				}
		}
		public void Remove(Position start, Position finish, string word) {
			foreach (IgnoreListItem item in list)
				if (Position.Equals(item.Start, start) && Position.Equals(item.End, finish) && CompareWords(item.Word, word)) {
					list.Remove(item);
					break;
				}
		}
		public void Clear() {
			this.list.Clear();
		}
		#region IEnumerable<IIgnoreItem> Members
		public IEnumerator<IIgnoreItem> GetEnumerator() {
			for (int i = 0; i < this.list.Count; i++)
				yield return this.list[i];
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		#endregion
	}
}
namespace DevExpress.XtraSpellChecker.Native {
	public class ErrorByWordCache {
		Dictionary<string, SpellCheckErrorBase> ht = new Dictionary<string, SpellCheckErrorBase>();
		public virtual void Add(SpellCheckErrorBase error) {
			if (!Contains(error))
				ht[error.WrongWord] = error;
		}
		public virtual bool Contains(SpellCheckErrorBase error) {
			return Contains(error.WrongWord);
		}
		public virtual bool Contains(string word) {
			SpellCheckErrorBase result;
			if (ht.TryGetValue(word, out result))
				return result != null;
			else
				return false;
		}
		public virtual void Clear() {
			ht.Clear();
		}
		public virtual SpellCheckErrorBase this[string word] {
			get {
				if (!Contains(word))
					return null;
				return ht[word];
			}
		}
	}
	public class Alphabet : IEnumerable<char> {
		List<char> list = new List<char>();
		public char this[int index] {
			get {
				return (char)list[index];
			}
			set {
				list[index] = value;
			}
		}
		public void CopyTo(char[] array, int index) {
			list.CopyTo(array, index);
		}
		public void Add(char c) {
			int index = IndexOf(c);
			if (index == -1)
				list.Add(c);
		}
		public bool Contains(char c) {
			return list.Contains(c);
		}
		public int IndexOf(char c) {
			return list.IndexOf(c);
		}
		public void Insert(int index, char c) {
			list.Insert(index, c);
		}
		public void Remove(char c) {
			int index = IndexOf(c);
			if (index >= 0)
				list.RemoveAt(index);
		}
		public void AddRange(IEnumerable<char> collection) {
			list.AddRange(collection);
		}
		public void Clear() {
			list.Clear();
		}
		public int Count { get { return list.Count; } }
		#region IEnumerable<char> Members
		public IEnumerator<char> GetEnumerator() {
			return list.GetEnumerator();
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return list.GetEnumerator();
		}
		#endregion
	}
	public class StateCache {
		Dictionary<StrategyState, SpellCheckerStateBase> states;
		public StateCache() {
			states = new Dictionary<StrategyState, SpellCheckerStateBase>();
		}
		public SpellCheckerStateBase GetState(StrategyState state) {
			SpellCheckerStateBase result = null;
			states.TryGetValue(state, out result);
			return result;
		}
		public void Add(StrategyState state, SpellCheckerStateBase scState) {
			if (states.ContainsKey(state))
				return;
			states.Add(state, scState);
		}
		public void Remove(StrategyState state) {
			if (states.ContainsKey(state))
				states.Remove(state);
		}
		public void Clear() {
			states.Clear();
		}
	}
	public class RectangleList {
		List<Rectangle> rectangles;
		public RectangleList() {
			rectangles = new List<Rectangle>();
		}
		public Rectangle this[int index] {
			get { return Rectangles[index]; }
			set { Rectangles[index] = value; }
		}
		protected List<Rectangle> Rectangles { get { return rectangles; } }
		public void Add(Rectangle rectangle) {
			if (!Contains(rectangle))
				Rectangles.Add(rectangle);
		}
		public void AddRange(RectangleList rectangles) {
			Rectangles.AddRange(rectangles.Rectangles);
		}
		public bool Contains(Rectangle rectangle) {
			for (int i = 0; i < Count; i++)
				if (rectangle.Equals(Rectangles[i]))
					return true;
			return false;
		}
		public int Count { get { return Rectangles.Count; } }
		public void Remove(Rectangle rectangle) {
			for (int i = 0; i < Count; i++) {
				if (rectangle.Equals(Rectangles[i])) {
					Rectangles.RemoveAt(i);
					break;
				}
			}
		}
		public void Clear() {
			Rectangles.Clear();
		}
	}
}
