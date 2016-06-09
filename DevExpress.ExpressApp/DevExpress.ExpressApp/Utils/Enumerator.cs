#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Collections;
namespace DevExpress.ExpressApp.Utils {
	public delegate void EnumerateDelegate<OfType>(OfType item);
	public static class Enumerator {
		public static int Count<T>(IEnumerable<T> list) {
			int i = 0;
			foreach (T item in list) {
				i++;
			}
			return i;
		}
		public static int Count(IEnumerable list) {
			int i = 0;
			foreach(object item in list) {
				i++;
			}
			return i;
		}
		public static IEnumerable<T> Combine<T>(params IEnumerable<T>[] enumerables) {
			foreach (IEnumerable<T> enumerable in enumerables) {
				foreach (T item in enumerable) {
					yield return item;
				}
			}
		}
		public static T GetFirst<T>(IEnumerable<T> source) {
			foreach (T item in source) {
				return item;
			}
			return default(T);
		}
		public static bool Exists<T>(IEnumerable<T> enumerable, T toFind) {
			return null != Find<T>(enumerable, 
				delegate(T item) { 
					return Object.ReferenceEquals(item, toFind); 
				});
		}
		public static bool Exists(IEnumerable enumerable, object toFind) {
			return null != Find(enumerable,
				delegate(object item) {
					return Object.ReferenceEquals(item, toFind);
				});
		}
		public static IEnumerable<T> Filter<T>(IEnumerable<T> enumerable, Predicate<T> predicate) {
			foreach (T item in enumerable) {
				if (predicate(item)) {
					yield return item;
				}
			}
		}
		public static IEnumerable Filter(IEnumerable enumerable, Predicate<object> predicate) {
			foreach(object item in enumerable) {
				if(predicate(item)) {
					yield return item;
				}
			}
		}
		public static T Find<T>(IEnumerable<T> enumerable, Predicate<T> predicate) {
			foreach (T item in enumerable) {
				if (predicate(item)) {
					return item;
				}
			}
			return default(T);
		}
		public static object Find(IEnumerable enumerable, Predicate<object> predicate) {
			foreach(object item in enumerable) {
				if(predicate(item)) {
					return item;
				}
			}
			return null;
		}
		public static IEnumerable<TOutput> Convert<TOutput, TInput>(IEnumerable<TInput> source) where TInput : TOutput {
			foreach(TInput item in source) {
				yield return (TOutput)item;
			}
		}
		public static IEnumerable<TOutput> Convert<TInput, TOutput>(IEnumerable<TInput> source, Converter<TInput, TOutput> castDelegate) {
			foreach (TInput item in source) {
				yield return castDelegate(item);
			}
		}
		public static T[] ToArray<T>(IEnumerable<T> source) {
			return new List<T>(source).ToArray();
		}
		public static IEnumerable<T> Convert<T>(IEnumerable source) {
			foreach(object item in source) {
				yield return (T)item;
			}
		}
		public static void SafeEnumerate<OfType>(IEnumerable<OfType> source, EnumerateDelegate<OfType> handler) {
			int index = 0;
			List<OfType> list = new List<OfType>(source);
			while (index < list.Count) {
				handler(list[index]); 
				list = new List<OfType>(source);
				index++;
			}
		}
	}
}
