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
	public interface PivotTableCollection : ISimpleCollection<PivotTable> {
		PivotTable First { get; }
		PivotTable Last { get; }
		PivotTable this[string name] { get; }
		PivotTable Add(Range sourceRange, Range location); 
		PivotTable Add(Range sourceRange, Range location, string name); 
		PivotTable Add(PivotCache cache, Range location); 
		PivotTable Add(PivotCache cache, Range location, string name); 
		void Remove(PivotTable table);
		void RemoveAt(int index);
		void Clear();
		bool Contains(PivotTable table);
		int IndexOf(PivotTable table);
	}
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Spreadsheet;
	using DevExpress.Utils;
	using System.Collections;
	using DevExpress.Office.Utils;
	using DevExpress.Office.Model;
	#region NativePivotTableCollection
	partial class NativePivotTableCollection : NativeObjectBase, PivotTableCollection {
		#region Fields
		readonly List<NativePivotTable> innerList;
		readonly NativeWorksheet nativeWorksheet;
		#endregion
		public NativePivotTableCollection(NativeWorksheet nativeWorksheet) {
			Guard.ArgumentNotNull(nativeWorksheet, "nativeWorksheet");
			this.innerList = new List<NativePivotTable>();
			this.nativeWorksheet = nativeWorksheet;
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
		public PivotTable this[int index] {
			get {
				CheckValid();
				return innerList[index];
			}
		}
		protected internal NativeWorksheet NativeWorksheet { get { return nativeWorksheet; } }
		protected internal Model.Worksheet ModelWorksheet { get { return nativeWorksheet.ModelWorksheet; } }
		protected internal Model.PivotTableCollection ModelCollection { get { return ModelWorksheet.PivotTables; } }
		Model.DocumentModel ModelWorkbook { get { return ModelWorksheet.Workbook; } }
		ApiErrorHandler ErrorHandler { get { return ApiErrorHandler.Instance; } }
		#endregion
		#region Internal
		void Populate() {
			ModelCollection.ForEach(RegisterItem);
		}
		void RegisterItem(Model.PivotTable item) {
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
		NativePivotTable CreateNativeObject(Model.PivotTable item) {
			return new NativePivotTable(item, nativeWorksheet, GetCache(item.Cache));
		}
		NativePivotCache GetCache(Model.PivotCache modelCache) {
			NativePivotCache nativeCache = ((NativePivotCacheCollection)nativeWorksheet.Workbook.PivotCaches).Find(modelCache);
			Guard.ArgumentNotNull(nativeCache, "nativeCache");
			return nativeCache;
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
			UndoableCollectionAddEventArgs<Model.PivotTable> modelArgs = e as UndoableCollectionAddEventArgs<Model.PivotTable>;
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
			UndoableCollectionInsertEventArgs<Model.PivotTable> modelArgs = e as UndoableCollectionInsertEventArgs<Model.PivotTable>;
			if (modelArgs != null)
				innerList.Insert(modelArgs.Index, CreateNativeObject(modelArgs.Item));
		}
		void OnClear(object sender) {
			InvalidateItems();
			innerList.Clear();
		}
		void OnAddRange(object sender, EventArgs e) {
			UndoableCollectionAddRangeEventArgs<Model.PivotTable> modelArgs = e as UndoableCollectionAddRangeEventArgs<Model.PivotTable>;
			if (modelArgs == null)
				return;
			IEnumerable<Model.PivotTable> collection = modelArgs.Collection;
			foreach (Model.PivotTable modelItem in collection)
				RegisterItem(modelItem);
		}
		#endregion
		#region ISimpleCollection<PivotTable> Members
		public IEnumerator<PivotTable> GetEnumerator() {
			CheckValid();
			return new EnumeratorAdapter<PivotTable, NativePivotTable>(innerList.GetEnumerator());
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
		#region PivotTableCollection Members
		public PivotTable First {
			get {
				CheckValid();
				if (innerList.Count > 0)
					return innerList[0];
				return null;
			}
		}
		public PivotTable Last {
			get {
				CheckValid();
				if (innerList.Count > 0)
					return innerList[innerList.Count - 1];
				return null;
			}
		}
		public PivotTable this[string name] {
			get {
				CheckValid();
				for (int i = 0; i < Count; i++) {
					NativePivotTable item = innerList[i];
					if (StringExtensions.ComparerInvariantCultureIgnoreCase.Equals(name, item.Name))
						return item;
				}
				return null;
			}
		}
		#region Add
		public PivotTable Add(Range sourceRange, Range location) {
			CheckValid();
			return AddCore(sourceRange, location, String.Empty);
		}
		public PivotTable Add(PivotCache cache, Range location) {
			CheckValid();
			return AddCore(cache.SourceRange, location, String.Empty);
		}
		public PivotTable Add(PivotCache cache, Range location, string name) {
			CheckValid();
			if (String.IsNullOrEmpty(name))
				HandleError(Model.ModelErrorType.PivotTableNameIsInvalid);
			return AddCore(cache.SourceRange, location, name);
		}
		public PivotTable Add(Range sourceRange, Range location, string name) {
			CheckValid();
			if (String.IsNullOrEmpty(name))
				HandleError(Model.ModelErrorType.PivotTableNameIsInvalid);
			return AddCore(sourceRange, location, name);
		}
		PivotTable AddCore(Range sourceRange, Range location, string name) {
			Model.CellRange modelSourceRange;
			Model.CellRange modelLocation;
			CheckArguments(sourceRange, location, name, out modelSourceRange, out modelLocation);
			ModelWorkbook.BeginUpdate();
			try {
				Model.PivotTable newModelItem = Model.PivotTable.Create(modelLocation, modelSourceRange, name, ErrorHandler);
				if (newModelItem == null)
					return null;
			}
			finally {
				ModelWorkbook.EndUpdate();
			}
			return innerList[Count - 1];
		}
		void CheckArguments(Range sourceRange, Range location, string name, out Model.CellRange modelSourceRange, out Model.CellRange modelLocation) {
			if (sourceRange == null || location == null || !Object.ReferenceEquals(location.Worksheet, nativeWorksheet))
				HandleError(Model.ModelErrorType.ErrorInvalidRange);
			if (ModelCollection.Contains(name))
				HandleError(Model.ModelErrorType.PivotTableNameAlreadyExists);
			modelSourceRange = ((NativeWorksheet)sourceRange.Worksheet).GetModelRange(sourceRange) as Model.CellRange;
			modelLocation = nativeWorksheet.GetModelRange(location) as Model.CellRange;
			if (modelSourceRange == null || modelLocation == null)
				HandleError(Model.ModelErrorType.UnionRangeNotAllowed);
		}
		void HandleError(Model.ModelErrorType errorType) {
			ErrorHandler.HandleError(errorType);
		}
		#endregion
		public void Remove(PivotTable table) {
			if (table == null)
				ApiErrorHandler.Instance.HandleError(Model.ModelErrorType.UsingInvalidObject);
			int index = IndexOf(table);
			if (index != -1)
				RemoveAt(index);
		}
		public void RemoveAt(int index) {
			CheckValid();
			ApiValueChecker.CheckIndex(index, innerList.Count - 1);
			ModelCollection[index].Remove(ErrorHandler);
		}
		public void Clear() {
			CheckValid();
			ModelWorkbook.BeginUpdate();
			try {
				for (int i = Count - 1; i >= 0; i--)
					ModelCollection[i].Remove(ErrorHandler);
			}
			finally {
				ModelWorkbook.EndUpdate();
			}
		}
		public bool Contains(PivotTable table) {
			return IndexOf(table) != -1;
		}
		public int IndexOf(PivotTable table) {
			CheckValid();
			NativePivotTable nativeItem = (NativePivotTable)table;
			if (nativeItem != null)
				return innerList.IndexOf(nativeItem);
			return -1;
		}
		#endregion
	}
	#endregion
}
