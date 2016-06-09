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
using System.Linq;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Model {
	public sealed class LazyCalculatedModelNodeList<T> : IList<T>, IModelList<T>, IModelList where T : IModelNode {
		private IEnumerable<T> enumeratorMethod;
		private IModelList<T> collection;
		public LazyCalculatedModelNodeList(IEnumerable<T> enumeratorMethod, IModelList<T> collection) {
			this.enumeratorMethod = enumeratorMethod;
			this.collection = collection;
		}
		#region IModelList<T> Members
		public T this[string id] {
			get { return collection[id]; }
		}
		IEnumerable<P> IModelList<T>.GetNodes<P>() {
			List<P> result = new List<P>();
			foreach(T item in this.enumeratorMethod) {
				if(item is P) {
					result.Add((P)item);
				}
			}
			return result;
		}
		public void ClearNodes() {
			throw new NotImplementedException();
		}
		#endregion
		#region IModelList Members
		object IModelList.this[string id] {
			get {
				return this[id];
			}
		}
		#endregion
		#region IList<T> Members
		public int IndexOf(T item) {
			throw new NotImplementedException();
		}
		public void Insert(int index, T item) {
			throw new NotImplementedException();
		}
		public void RemoveAt(int index) {
			throw new NotImplementedException();
		}
		public T this[int index] {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		#endregion
		#region ICollection<T> Members
		public void Add(T item) {
			throw new NotImplementedException();
		}
		public void Clear() {
			throw new NotImplementedException();
		}
		public bool Contains(T item) {
			throw new NotImplementedException();
		}
		public void CopyTo(T[] array, int arrayIndex) {
			throw new NotImplementedException();
		}
		public int Count {
			get { return enumeratorMethod.Count(); }
		}
		public bool IsReadOnly {
			get { return true; }
		}
		public bool Remove(T item) {
			throw new NotImplementedException();
		}
		#endregion
		#region IEnumerable<T> Members
		public IEnumerator<T> GetEnumerator() {
			return enumeratorMethod.GetEnumerator();
		}
		#endregion
		#region IEnumerable Members
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return enumeratorMethod.GetEnumerator();
		}
		#endregion
	}
	public sealed class CalculatedModelNodeList<T> : List<T>, IModelList<T>, IModelList where T : IModelNode {
		private string keyFieldName;
		public CalculatedModelNodeList(string keyFieldName, IEnumerable<T> collection)
			: base(collection) {
			Guard.ArgumentNotNullOrEmpty(keyFieldName, "keyFieldName");
			this.keyFieldName = keyFieldName;
		}
		public CalculatedModelNodeList(string keyFieldName) {
			Guard.ArgumentNotNullOrEmpty(keyFieldName, "keyFieldName");
			this.keyFieldName = keyFieldName;
		}
		public CalculatedModelNodeList() : this(ModelValueNames.Id) { }
		public CalculatedModelNodeList(IEnumerable<T> collection) : this(ModelValueNames.Id, collection) { }
		object IModelList.this[string id] {
			get {
				foreach(IModelNode item in this) {
					if(item.GetValue<string>(keyFieldName) == id) return item;
				}
				return null;
			}
		}
		T IModelList<T>.this[string id] {
			get {
				foreach(T item in this) {
					if(((IModelNode)item).GetValue<string>(keyFieldName) == id) return item;
				}
				return default(T);
			}
		}
		IEnumerable<P> IModelList<T>.GetNodes<P>() {
			List<P> result = new List<P>();
			foreach(T item in this) {
				if(item is P) {
					result.Add((P)item);
				}
			}
			return result;
		}
		void IModelList<T>.ClearNodes() {
			Clear();
		}
	}
}
