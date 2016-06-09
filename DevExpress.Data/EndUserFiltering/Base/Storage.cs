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

namespace DevExpress.Utils.Filtering.Internal {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	public interface IStorage<T> : IEnumerable<T> {
		T this[string path, Func<T, string> getPath] { get; }
		IEnumerable<KeyValuePair<string, TValue>> GetPairs<TValue>(Func<T, string> getPath, Func<T, TValue> accessor);
	}
	interface IOrderedStorage<T> : IStorage<T> {
		void ResetOrder(Func<T, int> getOrder);
	}
	sealed class Storage<T> : IOrderedStorage<T> {
		Lazy<T[]> orderedElements;
		SortHelper helper;
		internal Storage(IEnumerable<T> elements, Func<T, int> getOrder) {
			helper = new SortHelper(elements ?? new T[0]);
			InitializeOrderedElements(getOrder);
		}
		void InitializeOrderedElements(Func<T, int> getOrder) {
			if(orderedElements == null || orderedElements.IsValueCreated)
				orderedElements = new Lazy<T[]>(() => helper.GetOrderedElements(getOrder));
		}
		T IStorage<T>.this[string path, Func<T, string> getPath] {
			get {
				T result = default(T);
				if(!TryGetValue(path, out result))
					result = Find(path, getPath);
				return result;
			}
		}
		IEnumerable<KeyValuePair<string, TValue>> IStorage<T>.GetPairs<TValue>(Func<T, string> getPath, Func<T, TValue> accessor) {
			foreach(T element in orderedElements.Value)
				yield return new KeyValuePair<string, TValue>(getPath(element), accessor(element));
		}
		void IOrderedStorage<T>.ResetOrder(Func<T, int> getOrder) {
			InitializeOrderedElements(getOrder);
		}
		bool TryGetValue(string path, out T result) {
			return GetPathMappings().TryGetValue(path, out result);
		}
		IDictionary<string, T> pathMappings;
		IDictionary<string, T> GetPathMappings() {
			if(pathMappings == null) 
				pathMappings = new Dictionary<string, T>();
			return pathMappings;
		}
		IEnumerator<T> pathEnumerator;
		T Find(string path, Func<T, string> getPath) {
			T result = default(T);
			if(pathEnumerator == null)
				pathEnumerator = GetEnumeratorCore();
			while(pathEnumerator.MoveNext()) {
				var current = pathEnumerator.Current;
				string currentPath = getPath(current);
				pathMappings.Add(currentPath, current);
				if(currentPath == path) {
					result = current;
					break;
				}
			}
			return result;
		}
		#region IEnumerable
		IEnumerator<T> IEnumerable<T>.GetEnumerator() {
			return GetEnumeratorCore();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return orderedElements.Value.GetEnumerator();
		}
		IEnumerator<T> GetEnumeratorCore() {
			return ((IEnumerable<T>)orderedElements.Value).GetEnumerator();
		}
		#endregion IEnumerable
		#region Sorting
		sealed class SortHelper : IComparer<KeyValuePair<int, int>> {
			Lazy<T[]> originalElements;
			internal SortHelper(IEnumerable<T> elements) {
				originalElements = new Lazy<T[]>(() => elements.ToArray());
			}
			internal T[] GetOrderedElements(Func<T, int> getOrder) {
				var elementsArray = originalElements.Value;
				T[] result = new T[elementsArray.Length];
				KeyValuePair<int, int>[] itemsAndindices = new KeyValuePair<int, int>[elementsArray.Length];
				for(int i = 0; i < itemsAndindices.Length; i++) {
					result[i] = elementsArray[i];
					itemsAndindices[i] = new KeyValuePair<int, int>(getOrder(elementsArray[i]), i);
				}
				Array.Sort(itemsAndindices, result, this);
				return result;
			}
			int IComparer<KeyValuePair<int, int>>.Compare(KeyValuePair<int, int> p1, KeyValuePair<int, int> p2) {
				return (p1.Key == p2.Key) ? p1.Value.CompareTo(p2.Value) : p1.Key.CompareTo(p2.Key);
			}
		}
		#endregion Sorting
	}
}
