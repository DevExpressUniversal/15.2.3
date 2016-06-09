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
	#region PivotFieldReferenceCollection
	public interface PivotFieldReferenceCollection : ISimpleCollection<PivotFieldReference> {
		PivotFieldReference this[string name] { get; }
		PivotFieldReference Add(PivotField field);
		PivotFieldReference AddValuesReference();
		PivotFieldReference Insert(int index, PivotField field);
		PivotFieldReference InsertValuesReference(int index);
		void Remove(PivotFieldReference fieldReference);
		void RemoveAt(int index);
		void Clear();
		bool Contains(PivotFieldReference field);
		int IndexOf(PivotFieldReference field);
	}
	#endregion
	#region PivotPageFieldCollection
	public interface PivotPageFieldCollection : ISimpleCollection<PivotPageField> {
		PivotPageField this[string name] { get; }
		PivotPageField Add(PivotField field);
		PivotPageField Insert(int index, PivotField field);
		void Remove(PivotPageField pageField);
		void RemoveAt(int index);
		void Clear();
		bool Contains(PivotPageField field);
		int IndexOf(PivotPageField field);
	}
	#endregion
	#region PivotDataFieldCollection
	public interface PivotDataFieldCollection : ISimpleCollection<PivotDataField> {
		PivotDataField this[string name] { get; }
		PivotDataField Add(PivotField field);
		PivotDataField Add(PivotField field, string name); 
		PivotDataField Insert(int index, PivotField field);
		PivotDataField Insert(int index, PivotField field, string name);
		void Remove(PivotDataField dataField);
		void RemoveAt(int index);
		void Clear();
		bool Contains(PivotDataField field);
		int IndexOf(PivotDataField field);
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Spreadsheet;
	using DevExpress.Utils;
	using System.Collections;
	using DevExpress.Office.Utils;
	using DevExpress.Office.Model;
	#region NativePivotFieldReferenceCollectionBase
	abstract partial class NativePivotFieldReferenceCollectionBase<T, N, M> : NativeObjectBase, ISimpleCollection<T>
		where T : PivotFieldReferenceBase
		where N : NativePivotFieldReferenceBase<T, N, M>, T
		where M : Model.IPivotFieldReference {
		#region Fields
		readonly List<N> innerList;
		readonly NativePivotTable parentPivotTable;
		readonly Model.PivotFieldReferenceCollection<M> modelCollection;
		#endregion
		protected NativePivotFieldReferenceCollectionBase(NativePivotTable parentPivotTable, Model.PivotFieldReferenceCollection<M> modelCollection) {
			Guard.ArgumentNotNull(parentPivotTable, "parentPivotTable");
			Guard.ArgumentNotNull(modelCollection, "modelCollection");
			this.innerList = new List<N>();
			this.parentPivotTable = parentPivotTable;
			this.modelCollection = modelCollection;
			SubscribeEvents();
		}
		#region Properties
		public int Count {
			get {
				CheckValid();
				return innerList.Count;
			}
		}
		public T this[int index] {
			get {
				CheckValid();
				return innerList[index];
			}
		}
		protected internal abstract PivotAxisType AxisType { get; }
		protected internal List<N> InnerList { get { return innerList; } }
		protected virtual int LastItemIndex { get { return innerList.Count - 1; } }
		protected internal NativePivotTable Parent { get { return parentPivotTable; } }
		protected internal Model.PivotFieldReferenceCollection<M> ModelCollection { get { return modelCollection; } }
		#endregion
		#region Internal
		protected internal void Populate() {
			modelCollection.ForEach(RegisterItem);
		}
		void RegisterItem(M item) {
			innerList.Add(CreateNativeObject(item, Count));
		}
		protected abstract N CreateNativeObject(M modelItem);
		N CreateNativeObject(M modelItem, int index) {
			N nativeItem = CreateNativeObject(modelItem);
			nativeItem.ReferenceIndex = index;
			return nativeItem;
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
		#region TryGetItemByName
		public T this[string name] {
			get {
				CheckValid();
				return TryGetItemByName(name);
			}
		}
		protected N TryGetItemByName(string name) {
			for (int i = 0; i < Count; i++) {
				N nativeItem = innerList[i];
				string currentName = GetName(nativeItem);
				if (string.IsNullOrEmpty(currentName))
					continue;
				if (StringExtensions.ComparerInvariantCultureIgnoreCase.Equals(name, currentName))
					return nativeItem;
			}
			return null;
		}
		protected abstract string GetName(N nativeItem);
		#endregion
		public T Add(PivotField field) {
			return Insert(innerList.Count, field);
		}
		#region Insert
		public virtual T Insert(int index, PivotField field) {
			CheckArguments(index, field);
			N existingItem = TryGetItemByName(field.Name);
			if (existingItem != null) {
				int fieldIndex = ((NativePivotField)field).FieldIndex;
				ExecuteInsert(fieldIndex, index);
				return existingItem;
			}
			return InsertCore(index, field);
		}
		protected void CheckArguments(int index, PivotField field) {
			CheckValid();
			ApiValueChecker.CheckIndex(index, innerList.Count);
			if (field == null || !Object.ReferenceEquals(parentPivotTable, ((NativePivotField)field).ParentTable))
				ApiErrorHandler.Instance.HandleError(Model.ModelErrorType.UsingInvalidObject);
		}
		protected N InsertCore(int index, PivotField field) {
			int fieldIndex = ((NativePivotField)field).FieldIndex;
			if (!ExecuteInsert(fieldIndex, index))
				return null;
			return innerList[index];
		}
		protected virtual bool ExecuteInsert(int fieldIndex, int index) {
			return parentPivotTable.InsertFieldToKeyFields(fieldIndex, AxisType, index);
		}
		#endregion
		#region Remove
		public void Remove(T item) {
			CheckValid();
			if (item == null)
				ApiErrorHandler.Instance.HandleError(Model.ModelErrorType.UsingInvalidObject);
			int index = IndexOf(item);
			if (index != -1)
				parentPivotTable.RemoveKeyField(index, AxisType);
		}
		public void RemoveAt(int index) {
			CheckValid();
			ApiValueChecker.CheckIndex(index, innerList.Count - 1);
			parentPivotTable.RemoveKeyField(index, AxisType);
		}
		#endregion
		public void Clear() {
			CheckValid();
			parentPivotTable.ClearKeyFields(AxisType);
		}
		public int IndexOf(T item) {
			CheckValid();
			N nativeItem = (N)item;
			if (nativeItem != null)
				return InnerList.IndexOf(nativeItem);
			return -1;
		}
		public bool Contains(T item) {
			return IndexOf(item) != -1;
		}
		#endregion
		#region SubscribeEvents
		protected internal void SubscribeEvents() {
			modelCollection.OnAdd += OnAdd;
			modelCollection.OnRemoveAt += OnRemoveAt;
			modelCollection.OnInsert += OnInsert;
			modelCollection.OnClear += OnClear;
			modelCollection.OnAddRange += OnAddRange;
			modelCollection.OnMove += OnMove;
		}
		protected internal void UnsubscribeEvents() {
			modelCollection.OnAdd -= OnAdd;
			modelCollection.OnRemoveAt -= OnRemoveAt;
			modelCollection.OnInsert -= OnInsert;
			modelCollection.OnClear -= OnClear;
			modelCollection.OnAddRange -= OnAddRange;
			modelCollection.OnMove -= OnMove;
		}
		void OnAdd(object sender, EventArgs e) {
		   UndoableCollectionAddEventArgs<M> modelArgs = e as UndoableCollectionAddEventArgs<M>;
			if (modelArgs != null)
				RegisterItem(modelArgs.Item);
		}
		void OnRemoveAt(object sender, UndoableCollectionRemoveAtEventArgs e) {
			int index = e.Index;
			if (index < innerList.Count) {
				innerList[index].IsValid = false;
				CorrectIndexes(index, DecrementIndex);
				innerList.RemoveAt(index);
			}
		}
		void OnInsert(object sender, EventArgs e) {
			UndoableCollectionInsertEventArgs<M> modelArgs = e as UndoableCollectionInsertEventArgs<M>;
			if (modelArgs != null) {
				int index = modelArgs.Index;
				innerList.Insert(index, CreateNativeObject(modelArgs.Item, index));
				CorrectIndexes(index, IncrementIndex);
			}
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
		void OnMove(object sender, EventArgs e) {
			UndoableCollectionMoveEventArgs modelArgs = e as UndoableCollectionMoveEventArgs;
			if (modelArgs == null)
				return;
			int sourceIndex = modelArgs.SourceIndex;
			int targetIndex = modelArgs.TargetIndex;
			N item = innerList[sourceIndex];
			CorrectIndexes(sourceIndex, DecrementIndex);
			innerList.Remove(item);
			item.ReferenceIndex = targetIndex;
			innerList.Insert(targetIndex, item);
			CorrectIndexes(targetIndex, IncrementIndex);
		}
		void CorrectIndexes(int startIndex, Action<N> action) {
			for (int i = startIndex + 1; i < Count; i++)
				action(innerList[i]);
		}
		void IncrementIndex(N nativeItem) {
			nativeItem.ReferenceIndex++;
		}
		void DecrementIndex(N nativeItem) {
			nativeItem.ReferenceIndex--;
		}
		#endregion
		#region ISimpleCollection<T> Members
		public IEnumerator<T> GetEnumerator() {
			CheckValid();
			return new EnumeratorAdapter<T, N>(innerList.GetEnumerator());
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
	}
	#endregion
	#region NativePivotFieldReferenceCollection
	partial class NativePivotFieldReferenceCollection : NativePivotFieldReferenceCollectionBase<PivotFieldReference, NativePivotFieldReference, Model.PivotFieldReference>, PivotFieldReferenceCollection {
		readonly PivotAxisType axisType;
		public NativePivotFieldReferenceCollection(NativePivotTable parentPivotTable, Model.PivotTableColumnRowFieldIndices modelCollection, bool isRowAxis) 
			: base(parentPivotTable, modelCollection) {
			this.axisType = isRowAxis ? PivotAxisType.Row : PivotAxisType.Column;
			Populate();
		}
		#region Properties
		protected internal override PivotAxisType AxisType { get { return axisType; } }
		protected override int LastItemIndex { get { return ModelCollection.LastKeyFieldIndex; } }
		protected new Model.PivotTableColumnRowFieldIndices ModelCollection { get { return (Model.PivotTableColumnRowFieldIndices)base.ModelCollection; } }
		protected internal bool HasValuesField { get { return ModelCollection.HasValuesField; } }
		int ValuesFieldIndex { get { return ModelCollection.ValuesFieldIndex; } }
		#endregion
		protected override NativePivotFieldReference CreateNativeObject(Model.PivotFieldReference modelItem) {
			return new NativePivotFieldReference(this, modelItem);
		}
		protected override string GetName(NativePivotFieldReference nativeItem) {
			return nativeItem.IsValuesReference ? String.Empty : nativeItem.Field.Name;
		}
		public PivotFieldReference AddValuesReference() {
			CheckValid();
			if (!ModelCollection.HasValuesField)
				if (!ExecuteInsert(Model.PivotTable.ValuesFieldFakeIndex, InnerList.Count))
					return null;
			return InnerList[ValuesFieldIndex];
		}
		public PivotFieldReference InsertValuesReference(int index) {
			CheckValid();
			ApiValueChecker.CheckIndex(index, InnerList.Count);
			if (!ModelCollection.HasValuesField || (index != ValuesFieldIndex && index != InnerList.Count))
				if (!ExecuteInsert(Model.PivotTable.ValuesFieldFakeIndex, index))
					return null;
			return InnerList[ValuesFieldIndex];
		}
	}
	#endregion
	#region NativePivotPageFieldCollection
	partial class NativePivotPageFieldCollection : NativePivotFieldReferenceCollectionBase<PivotPageField, NativePivotPageField, Model.PivotPageField>, PivotPageFieldCollection {
		public NativePivotPageFieldCollection(NativePivotTable parentPivotTable, Model.PivotPageFieldCollection modelCollection)
			: base(parentPivotTable, modelCollection) {
			Populate();
		}
		protected internal override PivotAxisType AxisType { get { return PivotAxisType.Page; } }
		protected override NativePivotPageField CreateNativeObject(Model.PivotPageField modelItem) {
			return new NativePivotPageField(this, modelItem);
		}
		protected override string GetName(NativePivotPageField nativeItem) {
			return nativeItem.Field.Name;
		}
	}
	#endregion
	#region NativePivotDataFieldCollection
	partial class NativePivotDataFieldCollection : NativePivotFieldReferenceCollectionBase<PivotDataField, NativePivotDataField, Model.PivotDataField>, PivotDataFieldCollection {
		public NativePivotDataFieldCollection(NativePivotTable parentPivotTable, Model.PivotDataFieldCollection modelCollection)
			: base(parentPivotTable, modelCollection) {
			Populate();
		}
		protected internal override PivotAxisType AxisType { get { return PivotAxisType.Value; } }
		protected override NativePivotDataField CreateNativeObject(Model.PivotDataField modelItem) {
			return new NativePivotDataField(this, modelItem);
		}
		protected override string GetName(NativePivotDataField nativeItem) {
			return nativeItem.Name;
		}
		public PivotDataField Add(PivotField field, string name) {
			return Insert(InnerList.Count, field, name);
		}
		#region Insert
		public override PivotDataField Insert(int index, PivotField field) {
			CheckArguments(index, field);
			return InsertCore(index, field);
		}
		public PivotDataField Insert(int index, PivotField field, string name) {
			CheckArguments(index, field);
			if (String.IsNullOrEmpty(name))
				ApiErrorHandler.Instance.HandleError(Model.ModelErrorType.PivotFieldNameIsInvalid);
			NativePivotDataField existingItem = TryGetItemByName(name);
			if (existingItem != null)
				ApiErrorHandler.Instance.HandleError(Model.ModelErrorType.PivotFieldNameAlreadyExists);
			Model.DocumentModel documentModel = Parent.ModelItem.Worksheet.Workbook;
			documentModel.BeginUpdate();
			try {
				PivotDataField dataField = InsertCore(index, field);
				if (dataField != null)
					dataField.Name = name;
				return dataField;
			}
			finally {
				documentModel.EndUpdate();
			}
		}
		protected override bool ExecuteInsert(int fieldIndex, int index) {
			return Parent.InsertDataField(fieldIndex, index);
		}
		#endregion
	}
	#endregion
}
