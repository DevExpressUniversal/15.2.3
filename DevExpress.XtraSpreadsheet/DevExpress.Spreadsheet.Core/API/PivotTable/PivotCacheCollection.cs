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
using System.ComponentModel;
using DevExpress.Office;
namespace DevExpress.Spreadsheet {
	public interface PivotCacheCollection : ISimpleCollection<PivotCache> {
		void RefreshAll();
	}
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Spreadsheet;
	using DevExpress.Utils;
	using System.Collections;
	using DevExpress.Office.Utils;
	using DevExpress.Office.Model;
	#region NativePivotCacheCollection
	partial class NativePivotCacheCollection : NativeObjectBase, PivotCacheCollection {
		#region Fields
		readonly List<NativePivotCache> innerList;
		readonly NativeWorkbook nativeWorkbook;
		#endregion
		public NativePivotCacheCollection(NativeWorkbook nativeWorkbook) {
			Guard.ArgumentNotNull(nativeWorkbook, "nativeWorkbook");
			this.innerList = new List<NativePivotCache>();
			this.nativeWorkbook = nativeWorkbook;
			Populate();
			SubscribeEvents();
		}
		#region Properties
		public int Count {
			get {
				CheckValid();
				return innerList.Count;
			}
		}
		public PivotCache this[int index] {
			get {
				CheckValid();
				return innerList[index];
			}
		}
		protected internal NativeWorkbook NativeWorkbook { get { return nativeWorkbook; } }
		protected internal Model.DocumentModel ModelWorkbook { get { return nativeWorkbook.Model; } }
		protected internal Model.PivotCacheCollection ModelCollection { get { return ModelWorkbook.PivotCaches; } }
		#endregion
		#region Internal
		void Populate() {
			ModelCollection.ForEach(RegisterItem);
		}
		void RegisterItem(Model.PivotCache item) {
			innerList.Add(CreateNativeObject(item));
		}
		protected override void SetIsValid(bool value) {
			base.SetIsValid(value);
			if (!value) {
				InvalidateItems();
				UnsubscribeEvents();
			}
		}
		void InvalidateItems() {
			int count = innerList.Count;
			for (int i = 0; i < count; i++)
				innerList[i].IsValid = false;
		}
		NativePivotCache CreateNativeObject(Model.PivotCache item) {
			return new NativePivotCache(item, nativeWorkbook);
		}
		#endregion
		#region SubscribeEvents
		void SubscribeEvents() {
			ModelCollection.OnAdd += OnAdd;
			ModelCollection.OnRemoveAt += OnRemoveAt;
			ModelCollection.OnInsert += OnInsert;
			ModelCollection.OnClear += OnClear;
			ModelCollection.OnAddRange += OnAddRange;
		}
		void UnsubscribeEvents() {
			ModelCollection.OnAdd -= OnAdd;
			ModelCollection.OnRemoveAt -= OnRemoveAt;
			ModelCollection.OnInsert -= OnInsert;
			ModelCollection.OnClear -= OnClear;
			ModelCollection.OnAddRange -= OnAddRange;
		}
		void OnAdd(object sender, EventArgs e) {
			UndoableCollectionAddEventArgs<Model.PivotCache> modelArgs = e as UndoableCollectionAddEventArgs<Model.PivotCache>;
			if (modelArgs != null)
				RegisterItem(modelArgs.Item);
		}
		void OnRemoveAt(object sender, UndoableCollectionRemoveAtEventArgs e) {
			int index = e.Index;
			if (index < innerList.Count) {
				innerList[index].IsValid = false;
				innerList.RemoveAt(index);
			}
		}
		void OnInsert(object sender, EventArgs e) {
			UndoableCollectionInsertEventArgs<Model.PivotCache> modelArgs = e as UndoableCollectionInsertEventArgs<Model.PivotCache>;
			if (modelArgs != null)
				innerList.Insert(modelArgs.Index, CreateNativeObject(modelArgs.Item));
		}
		void OnClear(object sender) {
			InvalidateItems();
			innerList.Clear();
		}
		void OnAddRange(object sender, EventArgs e) {
			UndoableCollectionAddRangeEventArgs<Model.PivotCache> modelArgs = e as UndoableCollectionAddRangeEventArgs<Model.PivotCache>;
			if (modelArgs == null)
				return;
			IEnumerable<Model.PivotCache> collection = modelArgs.Collection;
			foreach (Model.PivotCache modelItem in collection)
				RegisterItem(modelItem);
		}
		#endregion
		#region ISimpleCollection<PivotCache> Members
		public IEnumerator<PivotCache> GetEnumerator() {
			CheckValid();
			return new EnumeratorAdapter<PivotCache, NativePivotCache>(innerList.GetEnumerator());
		}
		IEnumerator IEnumerable.GetEnumerator() {
			CheckValid();
			return innerList.GetEnumerator();
		}
		void ICollection.CopyTo(Array array, int index) {
			CheckValid();
			Array.Copy(innerList.ToArray(), 0, array, index, innerList.Count);
		}
		bool ICollection.IsSynchronized {
			get {
				CheckValid();
				return ((IList)this.innerList).IsSynchronized;
			}
		}
		object ICollection.SyncRoot {
			get {
				CheckValid();
				return ((IList)this.innerList).SyncRoot;
			}
		}
		#endregion
		public void RefreshAll() {
			CheckValid();
			ModelCollection.RefreshAll(ApiErrorHandler.Instance);
		}
		protected internal NativePivotCache Find(Model.PivotCache modelItem) {
			foreach (NativePivotCache item in this)
				if (Object.ReferenceEquals(item.ModelCache, modelItem))
					return item;
			return null;
		}
	}
	#endregion
}
