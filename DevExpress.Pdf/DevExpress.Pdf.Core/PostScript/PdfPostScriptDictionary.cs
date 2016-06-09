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
using System.Collections;
using System.Collections.Generic;
namespace DevExpress.Pdf.Native {
	public class PdfPostScriptDictionary : IDictionary {
		readonly Dictionary<string, object> dictionary;
		readonly List<string> duplicatedKeys = new List<string>();
		public object this[string key] { 
			get { return dictionary[key]; } 
			set { 
				if (dictionary.ContainsKey(key))
					duplicatedKeys.Add(key);
				dictionary[key] = value; 
			}
		}
		public int Count { get { return dictionary.Count; } }
		public IList<string> DuplicatedKeys { get { return duplicatedKeys; } }
		public PdfPostScriptDictionary(int capacity) {
			dictionary = new Dictionary<string, object>(capacity);
		}
		public bool ContainsKey(string key) {
			return dictionary.ContainsKey(key);
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return dictionary.GetEnumerator();
		}
		#region ICollection implementation
		bool ICollection.IsSynchronized { get { return false; } }
		object ICollection.SyncRoot { get { return null; } }
		void ICollection.CopyTo(Array array, int index) {
		}
		#endregion
		#region IDictionary implementation
		bool IDictionary.IsFixedSize { get { return false; } }
		bool IDictionary.IsReadOnly { get { return false; } }
		ICollection IDictionary.Keys { get { return dictionary.Keys; } }
		ICollection IDictionary.Values { get { return dictionary.Values; } }
		object IDictionary.this[object key] { 
			get { return this[(string)key]; } 
			set { this[(string)key] = value; }
		}
		bool IDictionary.Contains(object key) {
			return dictionary.ContainsKey((string)key);
		}
		void IDictionary.Add(object key, object value) {
			dictionary.Add((string)key, value);
		}
		void IDictionary.Remove(object key) {
			dictionary.Remove((string)key);
		}
		void IDictionary.Clear() {
			dictionary.Clear();
		}
		IDictionaryEnumerator IDictionary.GetEnumerator() {
			return dictionary.GetEnumerator();
		}
		#endregion
	}
}
