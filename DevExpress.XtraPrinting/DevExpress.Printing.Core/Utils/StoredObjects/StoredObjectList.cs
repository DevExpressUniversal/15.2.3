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
using System.Linq;
using System.Text;
using DevExpress.XtraPrinting;
namespace DevExpress.Utils.StoredObjects {
	class StoredObjectList<T> : IList<T> where T : IStoredObject {
		public static StoredObjectList<T> CreateInstance(IRepositoryProvider provider, IEnumerable<T> source) {
			IObjectRepository<T> repository;
			provider.TryGetRepository<T>(out repository);
			StoredObjectList<T> list = new StoredObjectList<T>(repository, provider);
			foreach(T item in source) {
				list.Add(item);
			}
			return list;
		}
		readonly RepositoryObjectChangedWeakEventHandler<StoredObjectList<T>> onRepositoryObjectChanged;
		IObjectRepository<T> repository;
		IRepositoryProvider provider;
		List<long> list = new List<long>();
		T item;
		public StoredObjectList(IObjectRepository<T> repository, IRepositoryProvider provider)
			: this(repository, provider, new long[0]) {
		}
		public StoredObjectList(IObjectRepository<T> repository, IRepositoryProvider provider, long[] ids) {
			this.repository = repository;
			this.provider = provider;
			list = new List<long>(ids);
			onRepositoryObjectChanged = new RepositoryObjectChangedWeakEventHandler<StoredObjectList<T>>(this, (me, sender, e) => me.item = default(T));
			repository.ObjectChanged += onRepositoryObjectChanged.Handler;
		}
		#region IList<T> Members
		int IList<T>.IndexOf(T item) {
			return list.IndexOf(item.Id);
		}
		void IList<T>.Insert(int index, T item) {
			EnsureObjectIsStored(item);
			list.Insert(index, item.Id);
		}
		void EnsureObjectIsStored(T item) {
			if(!item.HasId())
				repository.StoreObject(provider, item);
		}
		void IList<T>.RemoveAt(int index) {
			list.RemoveAt(index);
		}
		T IList<T>.this[int index] {
			get {
				if(index < 0 || index > list.Count - 1)
					throw new IndexOutOfRangeException();
				long id = list[index];
				if(item == null || item.Id != id) {
					if(!repository.TryRestoreObject<T>(provider, id, out item))
						throw new InvalidRestoreException();
				}
				return item;
			}
			set {
				if(value == null) throw new ArgumentNullException();
				EnsureObjectIsStored(value);
				list[index] = value.Id;
			}
		}
		#endregion
		#region ICollection<T> Members
		public virtual void Add(T item) {
			EnsureObjectIsStored(item);
			list.Add(item.Id);
		}
		void ICollection<T>.Clear() {
			list.Clear();
		}
		bool ICollection<T>.Contains(T item) {
			return list.Contains(item.Id);
		}
		void ICollection<T>.CopyTo(T[] array, int arrayIndex) {
			throw new NotSupportedException();
		}
		int ICollection<T>.Count {
			get { return list.Count; }
		}
		bool ICollection<T>.IsReadOnly {
			get { return false; }
		}
		bool ICollection<T>.Remove(T item) {
			if(item != null && item.HasId())
				return list.Remove(item.Id);
			return false;
		}
		#endregion
		#region IEnumerable<T> Members
		IEnumerator<T> IEnumerable<T>.GetEnumerator() {
			return new StoredObjectEnumerator<T>(repository, provider, list.GetEnumerator());
		}
		#endregion
		#region IEnumerable Members
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return new StoredObjectEnumerator<T>(repository, provider, list.GetEnumerator());
		}
		#endregion
	}
	class StoredObjectEnumerator<T> : IEnumerator<T> where T : IStoredObject {
		readonly RepositoryObjectChangedWeakEventHandler<StoredObjectEnumerator<T>> onRepositoryObjectChanged;
		IObjectRepository<T> repository;
		IRepositoryProvider provider;
		IEnumerator<long> ids;
		T item;
		public StoredObjectEnumerator(IObjectRepository<T> repository,  IRepositoryProvider provider, IEnumerator<long> ids) {
			onRepositoryObjectChanged = new RepositoryObjectChangedWeakEventHandler<StoredObjectEnumerator<T>>(this, (me, sender, e) => me.item = default(T));
			repository.ObjectChanged += onRepositoryObjectChanged.Handler;
			this.repository = repository;
			this.provider = provider;
			this.ids = ids;
		}
		T GetItem(long id) {
			if(item == null || item.Id != id) {
				if(!repository.TryRestoreObject<T>(provider, id, out item))
					throw new InvalidRestoreException();
			}
			return item;
		}
		#region IEnumerator<T> Members
		T IEnumerator<T>.Current {
			get { return GetItem(ids.Current); }
		}
		#endregion
		#region IDisposable Members
		void IDisposable.Dispose() {
			ids.Dispose();
		}
		#endregion
		#region IEnumerator Members
		object System.Collections.IEnumerator.Current {
			get {
				return GetItem(ids.Current);
			}
		}
		bool System.Collections.IEnumerator.MoveNext() {
			return ids.MoveNext();
		}
		void System.Collections.IEnumerator.Reset() {
			ids.Reset();
		}
		#endregion
	}
}
