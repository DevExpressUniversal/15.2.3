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

using System.Collections;
using System.Collections.Generic;
namespace DevExpress.Pdf.Native {
	public abstract class PdfDeferredList<T> : IList<T> where T : PdfObject {
		IEnumerator<object> source;
		readonly List<T> values = new List<T>();
		int sourceCount;
		protected PdfDeferredList() {
			this.source = new List<object>().GetEnumerator();
		}
		protected PdfDeferredList(IEnumerator<object> source, int sourceCount) {
			this.sourceCount = sourceCount;
			this.source = source;
		}
		public int IndexOf(T item) {
			ResolveAll();
			return values.IndexOf(item);
		}
		public void Insert(int index, T item) {
			if (index >= values.Count)
				ResolveAll();
			values.Insert(index, item);
		}
		public void RemoveAt(int index) {
			EnshureUptoIndex(index);
			values.RemoveAt(index);
		}
		public T this[int index] {
			get {
				EnshureUptoIndex(index);
				return values[index];
			}
			set {
				EnshureUptoIndex(index);
				values[index] = value;
			}
		}
		public void Add(T item) {
			ResolveAll();
			values.Add(item);
		}
		public void Clear() {
			values.Clear();
			if (source != null) {
				source.Dispose();
				source = null;
			}
		}
		public bool Contains(T item) {
			ResolveAll();
			return values.Contains(item);
		}
		public void CopyTo(T[] array, int arrayIndex) {
			ResolveAll();
			values.CopyTo(array, arrayIndex);
		}
		public int Count { get { return sourceCount + values.Count; } }
		public bool IsReadOnly {
			get { return false; }
		}
		public bool Remove(T item) {
			ResolveAll();
			return values.Remove(item);
		}
		public void AddRange(IEnumerable<T> items) {
			ResolveAll();
			values.AddRange(items);
		}
		public IEnumerator<T> GetEnumerator() {
			int cnt = Count;
			for (int i = 0; i < cnt; i++) {
				if (i < values.Count) {
					yield return values[i];
				}
				else
					if (source != null && source.MoveNext()) {
						object value = source.Current;
						T result = ParseObject(value);
						sourceCount--;
						values.Add(result);
						yield return result;
					}
			}
			if (source != null) {
				source.Dispose();
				source = null;
			}
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		protected abstract T ParseObject(object value);
		void EnshureUptoIndex(int index) {
			using (IEnumerator<T> enumerator = GetEnumerator())
				while (values.Count <= index && enumerator.MoveNext()) { }
		}
		void ResolveAll() {
			EnshureUptoIndex(int.MaxValue);
		}
	}
}
