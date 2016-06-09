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
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
#if SL
using DevExpress.Xpf.Collections;
#else
#endif
namespace DevExpress.XtraPrinting.Native {
	public class PageBreakList : Collection<PageBreakInfo>, IListWrapper<PageBreakInfo> {
		internal ValueInfo LastValue { get { return Count > 0 ? this[Count - 1] : new ValueInfo(-1, false); } }
		public float MaxPageBreak {
			get {
				float maxPageBreak = 0;
				foreach (ValueInfo pageBreak in this)
					maxPageBreak = Math.Max(maxPageBreak, pageBreak.Value);
				return maxPageBreak;
			}
		}
		public PageBreakList()
			: base() {
		}
		protected override void InsertItem(int index, PageBreakInfo pageBreak) {
			if(pageBreak.Value < 0)
				return;
			for(int i = 0; i < Count; i++) {
				if(this[i].Value == pageBreak.Value)
					return;
				if(this[i].Value > pageBreak.Value) {
					base.Insert(i, pageBreak);
					return;
				}
			}
			base.InsertItem(index, pageBreak);
		}
		int IListWrapper<PageBreakInfo>.IndexOf(PageBreakInfo pageBreak) {
			throw new NotSupportedException();
		}
		void IListWrapper<PageBreakInfo>.Insert(PageBreakInfo pageBreak, int index) {
			throw new NotSupportedException();
		}
	}
	public interface IListWrapper<T>  : IEnumerable<T> {
		int Count { get; }
		T this[int index] { get; }
		void Add(T item);
		void RemoveAt(int index);
		void Insert(T item, int index);
		int IndexOf(T item);
		void Clear();
	}
	public static class IListWrapperExtentions {
		public static bool IsValidIndex<T>(this IListWrapper<T> listWrapper, int index) {
			return index >= 0 && index < listWrapper.Count;
		}
		public static T GetLast<T>(this IListWrapper<T> listWrapper) {
			return listWrapper.Count > 0 ? listWrapper[listWrapper.Count - 1] : default(T);
		}
		public static void Remove<T>(this IListWrapper<T> listWrapper, T item) {
			int index = listWrapper.IndexOf(item);
			if(index >= 0)
				listWrapper.RemoveAt(index);
		}
	}
	[Serializable, StructLayout(LayoutKind.Sequential)]
	struct Enumerator<T> : IEnumerator<T>, IDisposable, IEnumerator {
		private IEnumerator enumarator;
		public Enumerator(ICollection list) {
			enumarator = list.GetEnumerator();
		}
		public void Dispose() {
		}
		public bool MoveNext() {
			return enumarator.MoveNext();
		}
		public T Current {
			get {
				return (T)enumarator.Current;
			}
		}
		object IEnumerator.Current {
			get {
				return enumarator.Current;
			}
		}
		void IEnumerator.Reset() {
			enumarator.Reset();
		}
	}
 }
namespace System.Collections {
	using DevExpress.XtraPrinting.Native;
	public static class ArrayListExtentions {
		public static ArrayList CreateInstance<T>(IListWrapper<T> items) {
			ArrayList list = new ArrayList(items.Count);
			list.AddRange(items);
			return list;
		}
		public static void AddRange<T>(this ArrayList arrayList, IListWrapper<T> items) {
			arrayList.Capacity = Math.Max(arrayList.Capacity, arrayList.Count + items.Count);
			foreach(object item in items)
				arrayList.Add(item);
		}
	}
}
