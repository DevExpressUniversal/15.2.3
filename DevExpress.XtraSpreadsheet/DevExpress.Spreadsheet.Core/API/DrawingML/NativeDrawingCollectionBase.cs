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
using DevExpress.Office;
using DevExpress.Office.Model;
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	#region NativeDrawingCollectionBase (abstract class)
	abstract partial class NativeDrawingCollectionBase<T, N, M> : NativeObjectBase
		where N : T
		where M : class {
		#region Fields
		readonly UndoableCollection<M> modelCollection;
		readonly List<N> innerList;
		#endregion
		protected NativeDrawingCollectionBase(UndoableCollection<M> modelCollection) {
			this.modelCollection = modelCollection;
			this.innerList = new List<N>();
			SubscribeEvents();
		}
		#region Properties
		public int Count {
			get {
				CheckValid();
				return InnerList.Count;
			}
		}
		public T this[int index] {
			get {
				CheckValid();
				return InnerList[index];
			}
		}
		protected UndoableCollection<M> ModelCollection { get { return modelCollection; } }
		protected List<N> InnerList { 
			get {
				if (innerList.Count == 0 && modelCollection.Count > 0)
					Populate();
				return innerList; 
			} 
		}
		#endregion
		protected override void SetIsValid(bool value) {
			base.SetIsValid(value);
			if (!value) {
				InvalidateItems();
				UnsubscribeEvents();
			}
		}
		protected void AddModelItem(M item) {
			modelCollection.Add(item);
		}
		public virtual void RemoveAt(int index) {
			CheckValid();
			modelCollection.RemoveAt(index);
		}
		public virtual void Clear() {
			CheckValid();
			modelCollection.Clear();
		}
		#region Internal
		void Populate() {
			int count = modelCollection.Count;
			for (int i = 0; i < count; i++)
				RegisterItem(modelCollection[i]);
		}
		void RegisterItem(M modelItem) {
			N nativeObject = CreateNativeObject(modelItem);
			if (nativeObject != null)
				innerList.Add(nativeObject);
		}
		void InvalidateItems() {
			int count = innerList.Count;
			for (int i = 0; i < count; i++)
				InvalidateItem(innerList[i]);
		}
		void InvalidateItem(N nativeItem) {
			NativeObjectBase nativeBase = nativeItem as NativeObjectBase;
			if (nativeBase != null)
				nativeBase.IsValid = false;
		}
		#region SubscribeEvents
		protected internal void SubscribeEvents() {
			modelCollection.OnAdd += OnAdd;
			modelCollection.OnRemoveAt += OnRemoveAt;
			modelCollection.OnInsert += OnInsert;
			modelCollection.OnClear += OnClear;
			modelCollection.OnAddRange += OnAddRange;
		}
		protected internal void UnsubscribeEvents() {
			modelCollection.OnAdd -= OnAdd;
			modelCollection.OnRemoveAt -= OnRemoveAt;
			modelCollection.OnInsert -= OnInsert;
			modelCollection.OnClear -= OnClear;
			modelCollection.OnAddRange -= OnAddRange;
		}
		void OnAdd(object sender, EventArgs e) {
			UndoableCollectionAddEventArgs<M> modelArgs = e as UndoableCollectionAddEventArgs<M>;
			if (modelArgs != null)
				RegisterItem(modelArgs.Item);
		}
		void OnRemoveAt(object sender, UndoableCollectionRemoveAtEventArgs e) {
			int index = e.Index;
			if (index < innerList.Count) {
				InvalidateItem(innerList[index]);
				innerList.RemoveAt(index);
			}
		}
		void OnInsert(object sender, EventArgs e) {
			UndoableCollectionInsertEventArgs<M> modelArgs = e as UndoableCollectionInsertEventArgs<M>;
			if (modelArgs != null)
				innerList.Insert(modelArgs.Index, CreateNativeObject(modelArgs.Item));
		}
		void OnClear(object sender) {
			InvalidateItems();
			innerList.Clear();
		}
		void OnAddRange(object sender, EventArgs e) {
		   UndoableCollectionAddRangeEventArgs<M> modelArgs = e as UndoableCollectionAddRangeEventArgs<M>;
			if (modelArgs == null)
				return;
			IEnumerable<M> collection = modelArgs.Collection;
			foreach (M modelItem in collection)
				RegisterItem(modelItem);
		}
		#endregion
		#endregion
		protected abstract N CreateNativeObject(M modelItem);
	}
	#endregion 
}
