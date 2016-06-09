#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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
namespace DevExpress.Mvvm.Internal {
	public class FuzzyKeyValueVair<TKey, TValue> {
		public FuzzyKeyValueVair(TKey key, TValue value, bool useIncludeCondition) {
			Key = key;
			Value = value;
			UseIncludeCondition = useIncludeCondition;
		}
		public TKey Key { get; private set; }
		public TValue Value { get; private set; }
		public bool UseIncludeCondition { get; private set; }
	}
	public class FuzzyDictionary<TKey, TValue> : IEnumerable<FuzzyKeyValueVair<TKey, TValue>> {
		Func<TKey, TKey, bool> includeCondition;
		Dictionary<TKey, TValue> exactlyPairs = new Dictionary<TKey, TValue>();
		Dictionary<TKey, TValue> otherPairs = new Dictionary<TKey, TValue>();
		public FuzzyDictionary(Func<TKey, TKey, bool> includeCondition) {
			this.includeCondition = includeCondition;
		}
		public void Add(TKey key, TValue value, bool useIncludeCondition = false) {
			if(useIncludeCondition && includeCondition != null)
				otherPairs.Add(key, value);
			else
				exactlyPairs.Add(key, value);
		}
		public void Remove(TKey key, bool useIncludeCondition) {
			if(useIncludeCondition && includeCondition != null)
				otherPairs.Remove(key);
			else
				exactlyPairs.Remove(key);
		}
		public bool TryGetValue(TKey key, bool useIncludeCondition, out TValue value) {
			if(useIncludeCondition && includeCondition != null)
				return otherPairs.TryGetValue(key, out value);
			else
				return exactlyPairs.TryGetValue(key, out value);
		}
		public IEnumerable<TValue> GetValues(TKey key) {
			return key == null ? Values : GetValuesCore(key);
		}
		public IEnumerable<TValue> Values {
			get {
				foreach(var pair in exactlyPairs)
					yield return pair.Value;
				foreach(var pair in otherPairs)
					yield return pair.Value;
			}
		}
		IEnumerable<TValue> GetValuesCore(TKey key) {
			TValue value;
			if(exactlyPairs.TryGetValue(key, out value)) yield return value;
			foreach(var pair in otherPairs) {
				if(includeCondition(pair.Key, key)) yield return pair.Value;
			}
		}
		IEnumerator<FuzzyKeyValueVair<TKey, TValue>> IEnumerable<FuzzyKeyValueVair<TKey, TValue>>.GetEnumerator() {
			foreach(var pair in exactlyPairs)
				yield return new FuzzyKeyValueVair<TKey, TValue>(pair.Key, pair.Value, false);
			foreach(var pair in otherPairs)
				yield return new FuzzyKeyValueVair<TKey, TValue>(pair.Key, pair.Value, true);
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			foreach(var pair in exactlyPairs)
				yield return new FuzzyKeyValueVair<TKey, TValue>(pair.Key, pair.Value, false);
			foreach(var pair in otherPairs)
				yield return new FuzzyKeyValueVair<TKey, TValue>(pair.Key, pair.Value, true);
		}
	}
}
