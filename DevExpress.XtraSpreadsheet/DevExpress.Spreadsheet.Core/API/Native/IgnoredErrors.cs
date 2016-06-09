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
using System.Collections;
using System.Collections.Generic;
using DevExpress.Office;
namespace DevExpress.Spreadsheet {
	#region IgnoredErrorType
	[Flags]
	public enum IgnoredErrorType {
		None = DevExpress.XtraSpreadsheet.Model.IgnoredErrorType.None,
		InconsistentColumnFormula = DevExpress.XtraSpreadsheet.Model.IgnoredErrorType.InconsistentColumnFormula,
		InconsistentFormula = DevExpress.XtraSpreadsheet.Model.IgnoredErrorType.InconsistentFormula,
		FormulaRange = DevExpress.XtraSpreadsheet.Model.IgnoredErrorType.FormulaRange,
		TextDate = DevExpress.XtraSpreadsheet.Model.IgnoredErrorType.TextDate,
		EmptyCellReferences = DevExpress.XtraSpreadsheet.Model.IgnoredErrorType.EmptyCellReferences,
		ListDataValidation = DevExpress.XtraSpreadsheet.Model.IgnoredErrorType.ListDataValidation,
		EvaluateToError = DevExpress.XtraSpreadsheet.Model.IgnoredErrorType.EvaluateToError,
		NumberAsText = DevExpress.XtraSpreadsheet.Model.IgnoredErrorType.NumberAsText,
		UnlockedFormula = DevExpress.XtraSpreadsheet.Model.IgnoredErrorType.UnlockedFormula
	}
	#endregion
	public interface IgnoredErrorCollection : ISimpleCollection<IgnoredError> {
		IgnoredError Add(Range range, IgnoredErrorType errorType);
		bool Remove(IgnoredError item);
		void RemoveAt(int index);
		void Clear();
		void Clear(Range range);
		bool Contains(IgnoredError item);
		int IndexOf(IgnoredError item);
	}
	public interface IgnoredError {
		Range Range { get; }
		IgnoredErrorType Type { get; }
	}
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Spreadsheet;
	using DevExpress.Utils;
	using DevExpress.Office.Utils;
	using DevExpress.Office.Model;
	#region NativeIgnoredErrorCollection
	partial class NativeIgnoredErrorCollection : NativeObjectBase, IgnoredErrorCollection {
		#region Fields
		readonly NativeWorksheet nativeWorksheet;
		readonly List<NativeIgnoredError> innerList;
		#endregion
		public NativeIgnoredErrorCollection(NativeWorksheet nativeWorksheet) {
			this.nativeWorksheet = nativeWorksheet;
			this.innerList = new List<NativeIgnoredError>();
			Initialize();
		}
		#region Properties
		public int Count {
			get {
				CheckValid();
				return InnerList.Count;
			}
		}
		public IgnoredError this[int index] {
			get {
				CheckValid();
				return InnerList[index];
			}
		}
		protected internal Model.Worksheet ModelWorksheet { get { return nativeWorksheet.ModelWorksheet; } }
		protected internal List<NativeIgnoredError> InnerList { get { return innerList; } }
		Model.IgnoredErrorCollection ModelCollection { get { return ModelWorksheet.IgnoredErrors; } }
		#endregion
		#region Internal
		protected internal void Initialize() {
			ModelCollection.ForEach(RegisterItem);
			SubscribeEvents();
		}
		void RegisterItem(Model.IgnoredError item) {
			innerList.Add(new NativeIgnoredError(item, nativeWorksheet));
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
		#endregion
		#region SubscribeEvents
		protected internal void SubscribeEvents() {
			ModelCollection.OnAdd += OnAdd;
			ModelCollection.OnRemoveAt += OnRemoveAt;
			ModelCollection.OnInsert += OnInsert;
			ModelCollection.OnClear += OnClear;
			ModelCollection.OnAddRange += OnAddRange;
		}
		protected internal void UnsubscribeEvents() {
			ModelCollection.OnAdd -= OnAdd;
			ModelCollection.OnRemoveAt -= OnRemoveAt;
			ModelCollection.OnInsert -= OnInsert;
			ModelCollection.OnClear -= OnClear;
			ModelCollection.OnAddRange -= OnAddRange;
		}
		void OnAdd(object sender, EventArgs e) {
			UndoableCollectionAddEventArgs<Model.IgnoredError> modelArgs = e as UndoableCollectionAddEventArgs<Model.IgnoredError>;
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
			UndoableCollectionInsertEventArgs<Model.IgnoredError> modelArgs = e as UndoableCollectionInsertEventArgs<Model.IgnoredError>;
			if (modelArgs != null)
				innerList.Insert(modelArgs.Index, new NativeIgnoredError(modelArgs.Item, nativeWorksheet));
		}
		void OnClear(object sender) {
			InvalidateItems();
			innerList.Clear();
		}
		void OnAddRange(object sender, EventArgs e) {
			UndoableCollectionAddRangeEventArgs<Model.IgnoredError> modelArgs = e as UndoableCollectionAddRangeEventArgs<Model.IgnoredError>;
			if (modelArgs == null)
				return;
			IEnumerable<Model.IgnoredError> collection = modelArgs.Collection;
			foreach (Model.IgnoredError modelItem in collection)
				RegisterItem(modelItem);
		}
		#endregion
		#region ISimpleCollection<IgnoredError> Members
		public IEnumerator<IgnoredError> GetEnumerator() {
			CheckValid();
			return new EnumeratorAdapter<IgnoredError, NativeIgnoredError>(innerList.GetEnumerator());
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
		#region IgnoredErrorCollection Members
		public IgnoredError Add(Range range, IgnoredErrorType errorType) {
			if (errorType == IgnoredErrorType.None) {
				Clear(range);
				return null;
			}
			CheckValid();
			Model.CellRangeBase modelRange = nativeWorksheet.GetModelRange(range);
			Model.IgnoredErrorAddCommand command = new Model.IgnoredErrorAddCommand(ModelWorksheet, ApiErrorHandler.Instance, modelRange, (Model.IgnoredErrorType)errorType);
			if (!command.Execute())
				return null;
			return InnerList[Count - 1];
		}
		public bool Remove(IgnoredError item) {
			int index = IndexOf(item);
			if (index != -1)
				RemoveAt(index);
			return index != -1;
		}
		public void RemoveAt(int index) {
			CheckValid();
			ModelCollection.RemoveAt(index);
		}
		public void Clear() {
			CheckValid();
			ModelCollection.Clear();
		}
		public void Clear(Range range) {
			CheckValid();
			Model.CellRangeBase modelRange = nativeWorksheet.GetModelRange(range);
			Model.IgnoredErrorClearCommand command = new Model.IgnoredErrorClearCommand(ModelWorksheet, ApiErrorHandler.Instance, modelRange);
			command.Execute();
		}
		public bool Contains(IgnoredError item) {
			return IndexOf(item) != -1;
		}
		public int IndexOf(IgnoredError item) {
			CheckValid();
			NativeIgnoredError nativeItem = item as NativeIgnoredError;
			if (nativeItem != null)
				return InnerList.IndexOf(nativeItem);
			return -1;
		}
		#endregion
	}
	#endregion
	#region NativeIgnoredError
	partial class NativeIgnoredError : NativeObjectBase, IgnoredError {
		readonly Model.IgnoredError modelIgnoredError;
		readonly NativeWorksheet nativeWorksheet;
		public NativeIgnoredError(Model.IgnoredError modelIgnoredError, NativeWorksheet nativeWorksheet) {
			Guard.ArgumentNotNull(nativeWorksheet, "nativeWorksheet");
			Guard.ArgumentNotNull(modelIgnoredError, "modelIgnoredError");
			this.nativeWorksheet = nativeWorksheet;
			this.modelIgnoredError = modelIgnoredError;
		}
		#region Properties
		protected internal NativeWorksheet NativeWorksheet { get { return nativeWorksheet; } }
		protected internal Model.IgnoredError ModelItem { get { return modelIgnoredError; } }
		public Range Range {
			get {
				CheckValid();
				return new NativeRange(modelIgnoredError.Range, nativeWorksheet);
			}
		}
		public IgnoredErrorType Type {
			get {
				CheckValid();
				return (IgnoredErrorType)modelIgnoredError.GetErrorType();
			}
		}
		#endregion
	}
	#endregion
}
