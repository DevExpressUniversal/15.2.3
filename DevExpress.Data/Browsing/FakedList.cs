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
using System.Threading;
namespace DevExpress.Data.Browsing {
	public static class FakedListCreator {
		public static IList CreateFakedList(object genericList) {
			return CreateGenericListCore(genericList, argumentType => {
				Type fakedListType = typeof(FakedList<>).MakeGenericType(argumentType);
				return (IList)Activator.CreateInstance(fakedListType, genericList);
			});
		}
		public static IList CreateGenericList(object listSource) {
			return CreateGenericListCore(listSource, argumentType => {
				Type genericType = typeof(List<>).MakeGenericType(argumentType);
				return (IList)Activator.CreateInstance(genericType);
			});
		}
		static IList CreateGenericListCore(object listSource, Func<Type, IList> create) {
			Type[] genericArguments = ListTypeHelper.FindGenericArguments(listSource.GetType(), item => {
				return typeof(IList<>) == item || typeof(IEnumerable<>) == item || typeof(ICollection<>) == item;
			});
			return genericArguments != null ? create(genericArguments[0]) : null;
		}
	}
	class FakedList<T> : IList {
		IList<T> items;
		public FakedList(IList<T> items) {
			this.items = items;
		}
		public FakedList(IEnumerable<T> enumerable) {
			this.items = new List<T>(enumerable);
		}
		public T this[int index] {
			get {
				return items[index];
			}
			set {
				items[index] = (T)value;
			}
		}
		#region IList Members
		int IList.Add(object value) {
			items.Add((T)value);
			return items.Count - 1;
		}
		void IList.Clear() {
			items.Clear();
		}
		bool IList.Contains(object value) {
			return items.Contains((T)value);
		}
		int IList.IndexOf(object value) {
			return items.IndexOf((T)value);
		}
		void IList.Insert(int index, object value) {
			items.Insert(index, (T)value);
		}
		bool IList.IsFixedSize {
			get { return true; }
		}
		bool IList.IsReadOnly {
			get { return items.IsReadOnly; }
		}
		void IList.Remove(object value) {
			items.Remove((T)value);
		}
		void IList.RemoveAt(int index) {
			items.RemoveAt(index);
		}
		object IList.this[int index] {
			get {
				return items[index];
			}
			set {
				items[index] = (T)value;
			}
		}
		#endregion
		#region ICollection Members
		void ICollection.CopyTo(Array array, int index) {
			((ICollection<T>)items).CopyTo((T[])array, index);
		}
		int ICollection.Count {
			get { return items.Count; }
		}
		bool ICollection.IsSynchronized {
			get { return false; }
		}
		object syncRoot;
		object ICollection.SyncRoot {
			get {
				if(syncRoot == null)
					Interlocked.CompareExchange(ref syncRoot, new object(), null);
				return syncRoot;
			}
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return items.GetEnumerator();
		}
		#endregion
	}
}
