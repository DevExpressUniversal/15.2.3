#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Linq;
using System.Text;
namespace DevExpress.DashboardCommon.Native {
	static class ExtensionHelper {
		class PredicateComparer<T> : IEqualityComparer<T> {
			Func<T, T, bool> equals;
			Func<T, int> hashCode;
			public PredicateComparer(Func<T, T, bool> equals, Func<T, int> hashCode) {
				this.equals = equals;
				this.hashCode = hashCode;
			}
			public bool Equals(T x, T y) {
				return equals(x, y);
			}
			public int GetHashCode(T obj) {
				if(hashCode != null)
					return hashCode(obj);
				else
					return 0;
			}
		}
		public static void AddRange<T>(this IList<T> list, IEnumerable<T> itemsToAdd) {
			foreach(T item in itemsToAdd)
				list.Add(item);
		}
		public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action) {
			foreach(T item in enumerable)
				action(item);
		}
		public static IEnumerable<T> NotNull<T>(this IEnumerable<T> enumerable) {
			return enumerable.Where<T>(item => item != null);
		}
		public static bool SequenceEqual<T>(this IEnumerable<T> first, IEnumerable<T> second, Func<T, T, bool> equalsFunc, Func<T, int> hashCodeFunc) {
			return first.SequenceEqual(second, new PredicateComparer<T>(equalsFunc, hashCodeFunc));
		}
		public static bool ContainsAny<T>(this IEnumerable<T> collection, params T[] values) {
			return collection.ContainsAny((IEnumerable<T>)values);
		}
		public static bool ContainsAny<T>(this IEnumerable<T> collection, IEnumerable<T> values) {
			IEqualityComparer<T> comarer = EqualityComparer<T>.Default;
			return collection.Any(item => {
				bool contais = false;
				foreach(T testValue in values) {
					contais = comarer.Equals(item, testValue);
					if(contais)
						return true;
				}
				return false;
			});
		}
	}
}
