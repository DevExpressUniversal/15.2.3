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
using System.Linq;
using System.Collections.Generic;
using DevExpress.Compatibility.System;
namespace DevExpress.Utils {
	public static class ArrayHelper {
		public static T[] InsertItem<T>(T[] items, T item, int index) {
			T[] result = new T[items.Length + 1];
			result[index] = item;
			for(int i = 0; i < items.Length; i++) {
				int j = i < index ? i : i + 1;
				result[j] = items[i];
			}
			return result;
		}
		public static T[] Filter<T>(T[] items, Predicate<T> match) {
			List<T> result = new List<T>(items.Length);
			foreach(T item in items) {
				if(match(item))
					result.Add(item);
			}
			return result.ToArray();
		}
		static bool Match<T>(T[] array, T[] sample, int index) {
			int i;
			for(i = 0; i < sample.Length && index + i < array.Length; i++) {
				if(!object.Equals(sample[i], array[index + i]))
					return false;
			}
			return i == sample.Length;
		}
		static bool MatchBack<T>(T[] array, T[] sample, int count) {
			for(int i = 1; i <= count; i++)
				if(!object.Equals(array[array.Length - i], sample[sample.Length - i]))
					return false;
			return true;
		}
		public static int FindSubset<T>(T[] array, T[] sample) {
			for(int i = 0; i < array.Length; i++) {
				if(Match(array, sample, i))
					return i;
			}
			return -1;
		}
		public static bool StartsWith<T>(T[] array, T[] sample) {
			return Match(array, sample, 0);
		}
		public static bool EndsWith<T>(T[] array, T[] sample) {
			return array.Length >= sample.Length && MatchBack(array, sample, sample.Length);
		}
		public static bool EndsWith<T>(T[] array, T[] sample, int count) {
			return array.Length >= sample.Length && MatchBack(array, sample, count);
		}
		public static TOutput[] ConvertAll<TInput, TOutput>(TInput[] array, Converter<TInput, TOutput> converter) {
#if SL || DXRESTRICTED
			if (array == null) {
				throw new ArgumentNullException("array");
			}
			if(converter == null) {
				throw new ArgumentNullException("converter");
			}
			TOutput[] localArray = new TOutput[array.Length];
			for(int i = 0; i < array.Length; i++) {
				localArray[i] = converter(array[i]);
			}
			return localArray;
#else
			return Array.ConvertAll(array, converter);
#endif
		}
		public static T Find<T>(T[] array, Predicate<T> match) {
#if SL || DXRESTRICTED
			if (array == null) {
				throw new ArgumentNullException("array");
			}
			if(match == null) {
				throw new ArgumentNullException("match");
			}
			for(int i = 0; i < array.Length; i++) {
				if(match(array[i])) {
					return array[i];
				}
			}
			return default(T);
#else
			return Array.Find(array, match);
#endif
		}
		public static void ForEach<T>(IEnumerable<T> enumerable, Action<T> action) {
			foreach(T element in enumerable) {
				action(element);
			}
		}
		public static bool ArraysEqual<T>(T[] first, T[] second) {
			return ArraysEqual(first, second, 0, EqualityComparer<T>.Default);
		}
		public static bool ArraysEqual<T>(T[] first, T[] second, IEqualityComparer<T> comparer) {
			return ArraysEqual(first, second, 0, comparer);
		}
		public static bool ArraysEqual<T>(T[] first, T[] second, int startIndex, IEqualityComparer<T> comparer) {
			if(object.ReferenceEquals(first, second))
				return true;
			if(first == null ^ second == null)
				return false;
			if(first == null)
				return true;
			if(first.Length != second.Length)
				return false;
			for(int i = startIndex; i < first.Length; i++) {
				if(!comparer.Equals(first[i], second[i])) return false;
			}
			return true;
		}
#if !DXPORTABLE
		public static T[] Clone<T>(T[] source) where T : ICloneable {
			return CloneInternal(source, x => x == null ? default(T) : (T)x.Clone());
		}
#endif
#if SL || DXPORTABLE
		public static string[] Clone(string[] source) {
			return CloneInternal(source, x => x);
		}
#endif
		static T[] CloneInternal<T>(T[] source, Func<T, T> cloneItem) {
			if(source == null)
				throw new ArgumentNullException("source");
			if(cloneItem == null)
				throw new ArgumentNullException("cloneItem");
			T[] clone = new T[source.Length];
			for(int i = 0; i < source.Length; i++) {
				clone[i] = cloneItem(source[i]);
			}
			return clone;
		}
	}
}
