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
using System.Diagnostics;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.Office.Utils;
using System.Collections;
using DevExpress.Office.Model;
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	abstract partial class NativeCollectionBase<T, N, M> : ISimpleCollection<T>
		where N : T
		where M : class {
		readonly List<N> innerList;
		readonly NativeWorksheet worksheet;
		protected NativeCollectionBase(NativeWorksheet worksheet) {
			Guard.ArgumentNotNull(worksheet, "worksheet");
			this.worksheet = worksheet;
			this.innerList = new List<N>();
		}
		protected internal List<N> InnerList { get { return innerList; } }
		protected internal NativeWorksheet Worksheet { get { return worksheet; } }
		protected internal Model.Worksheet ModelWorksheet { get { return worksheet.ModelWorksheet; } }
		public abstract IEnumerable<M> GetModelItemEnumerable();
		public abstract int ModelCollectionCount { get; }
		protected abstract void RemoveModelObjectAt(int index);
		protected abstract void InvalidateItem(N item);
		public int Count { get { return InnerList.Count; } }
		public T this[int index] { get { return InnerList[index]; } }
		#region PopulateObjects
		protected internal virtual void Initialize() {
			if (ModelCollectionCount != 0)
				InitializeCore();
		}
		void InitializeCore() {
			foreach (M item in GetModelItemEnumerable())
				RegisterObject(item);
		}
		#endregion
		public void Clear() {
			ClearModelObjects();
		}
		protected abstract void ClearModelObjects();
		#region OnAdd
		protected internal void OnAdd(M modelObject) {
			InnerList.Add(CreateNativeObject(modelObject));
		}
		#endregion
		#region OnInsert
		protected internal void OnInsert(int index, M modelObject) {
			InnerList.Insert(index, CreateNativeObject(modelObject));
		}
		#endregion
		#region OnRemoveAt
		protected internal void OnRemoveAt(int index) {
			InvalidateItem(InnerList[index]);
			InnerList.RemoveAt(index);
		}
		#endregion
		#region OnClear
		protected internal void OnClear() {
			foreach (N hyperlink in InnerList)
				InvalidateItem(hyperlink);
			InnerList.Clear();
		}
		#endregion
		#region ClearCore
		protected internal void ClearCore() {
			foreach (N item in InnerList)
				InvalidateItem(item);
			InnerList.Clear();
		}
		#endregion
		#region RegisterObject
		void RegisterObject(M modelObject) {
			N newItem = CreateNativeObject(modelObject);
			innerList.Add(newItem);
		}
		#endregion
		protected abstract N CreateNativeObject(M modelObject);
		#region RemoveAt
		public void RemoveAt(int index) {
			RemoveModelObjectAt(index);
		}
		#endregion
		#region IndexOf
		public int IndexOf(T item) {
			return InnerList.IndexOf((N)item);
		}
		#endregion
		#region Remove
		public void Remove(T item) {
			int index = InnerList.IndexOf((N)item);
			RemoveModelObjectAt(index);
		}
		#endregion
		#region Contains
		public bool Contains(T item) {
			return InnerList.Contains((N)item);
		}
		#endregion
		#region GetEnumerator
		public IEnumerator<T> GetEnumerator() {
			return new EnumeratorAdapter<T, N>(InnerList.GetEnumerator());
		}
		#endregion
		#region IEnumerable.GetEnumerator
		IEnumerator IEnumerable.GetEnumerator() {
			return InnerList.GetEnumerator();
		}
		#endregion
		#region ICollection members
		bool ICollection.IsSynchronized { get { return ((IList)this.InnerList).IsSynchronized; } }
		object ICollection.SyncRoot { get { return ((IList)this.InnerList).SyncRoot; } }
		void ICollection.CopyTo(Array array, int index) {
			Array.Copy(InnerList.ToArray(), 0, array, index, InnerList.Count);
		}
		#endregion
		protected internal virtual void Invalidate() {
			ClearCore();
		}
	}
	abstract partial class NativeCollectionForUndoableCollectionBase<T, N, M> : NativeCollectionBase<T, N, M>
		where N : T
		where M : class {
		protected NativeCollectionForUndoableCollectionBase(NativeWorksheet worksheet)
			: base(worksheet) {
		}
		protected internal override void Initialize() {
			base.Initialize();
			SubscribeEvents();
		}
		protected abstract UndoableCollection<M> GetModelCollection();
		#region SubscribeEvents
		protected internal void SubscribeEvents() {
			UndoableCollection<M> modelCollection = GetModelCollection();
			System.Diagnostics.Debug.Assert(modelCollection != null);
			modelCollection.OnAdd += OnAdd;
			modelCollection.OnRemoveAt += OnRemoveAt;
			modelCollection.OnInsert += OnInsert;
			modelCollection.OnClear += OnClear;
			modelCollection.OnAddRange += OnAddRange;
		}
		protected internal void UnsubscribeEvents() {
			UndoableCollection<M> modelCollection = GetModelCollection();
			System.Diagnostics.Debug.Assert(modelCollection != null);
			modelCollection.OnAdd -= OnAdd;
			modelCollection.OnRemoveAt -= OnRemoveAt;
			modelCollection.OnInsert -= OnInsert;
			modelCollection.OnClear -= OnClear;
			modelCollection.OnAddRange -= OnAddRange;
		}
		void OnAdd(object sender, EventArgs e) {
			UndoableCollectionAddEventArgs<M> modelArgs = e as UndoableCollectionAddEventArgs<M>;
			if (modelArgs != null)
				OnAdd(modelArgs.Item);
		}
		void OnRemoveAt(object sender, UndoableCollectionRemoveAtEventArgs e) {
			int index = e.Index;
			if (index < InnerList.Count)
				OnRemoveAt(index);
		}
		void OnInsert(object sender, EventArgs e) {
			UndoableCollectionInsertEventArgs<M> modelArgs = e as UndoableCollectionInsertEventArgs<M>;
			if (modelArgs != null)
				OnInsert(modelArgs.Index, modelArgs.Item);
		}
		void OnClear(object sender) {
			OnClear();
		}
		void OnAddRange(object sender, EventArgs e) {
			UndoableCollectionAddRangeEventArgs<M> modelArgs = e as UndoableCollectionAddRangeEventArgs<M>;
			if (modelArgs == null)
				return;
			IEnumerable<M> collection = modelArgs.Collection;
			foreach (M modelItem in collection)
				OnAdd(modelItem);
		}
		#endregion
		protected internal override void Invalidate() {
			UnsubscribeEvents();
			base.Invalidate();
		}
	}
}
