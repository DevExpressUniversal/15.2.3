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
using DevExpress.Office;
using System.Collections.Generic;
using DevExpress.Spreadsheet.Formulas;
namespace DevExpress.Spreadsheet {
	public interface DefinedName : ExternalDefinedName {
		new string RefersTo { get; set; }
		Range Range { get; set; }
		Worksheet Scope { get; }
		bool IsGlobal { get; }
		new string Name { get; set; }
		string Comment { get; set; }
		ParsedExpression ParsedExpression { get; set; }
		bool Hidden { get; set; }
	}
	public interface DefinedNameCollection : ISimpleCollection<DefinedName> {
		void Remove(DefinedName item);
		void Remove(string name);
		void RemoveAt(int index);
		int IndexOf(DefinedName item);
		void Clear();
		DefinedName GetDefinedName(string name);
		bool Contains(string name);
		bool Contains(DefinedName item);
		DefinedName Add(string name, string refersTo);
	}
}
#region NativeDefinedName Implementation
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using ModelDefinedName = DevExpress.XtraSpreadsheet.Model.DefinedName;
	using ModelDefinedNameCollection = DevExpress.XtraSpreadsheet.Model.DefinedNameCollection;
	using ModelWorkbookDataContext = DevExpress.XtraSpreadsheet.Model.WorkbookDataContext;
	using DevExpress.Utils;
	using DevExpress.XtraSpreadsheet.Utils;
	using DevExpress.XtraSpreadsheet.Localization;
	using DevExpress.Office.Utils;
	using DevExpress.Spreadsheet;
	using System.Collections;
	using DevExpress.Spreadsheet.Functions;
	using DevExpress.Office.Model;
	#region NativeDefinedName
	partial class NativeDefinedName : NativeObjectBase, DefinedName {
		#region Fields
		readonly ModelDefinedName innerDefinedName;
		readonly NativeDefinedNameCollection collection;
		#endregion
		public NativeDefinedName(ModelDefinedName innerDefinedName, NativeDefinedNameCollection collection) {
			Guard.ArgumentNotNull(innerDefinedName, "innerDefinedName");
			this.innerDefinedName = innerDefinedName;
			this.collection = collection;
			innerDefinedName.ExpressionChanged += OnModelDefinedNameExpressionChanged;
		}
		#region DefinedName Members
		public string Name {
			get {
				CheckValid();
				return InnerDefinedName.Name;
			}
			set {
				CheckValid();
				InnerDefinedName.Name = value;
			}
		}
		public string RefersTo {
			get {
				CheckValid();
				string reference = InnerDefinedName.GetReference(0, 0);
				return String.IsNullOrEmpty(reference) ? reference : String.Concat("=", reference);
			}
			set {
				CheckValid();
				InnerDefinedName.SetReference(value);
			}
		}
		public Range Range {
			get {
				CheckValid();
				Model.CellRangeBase modelRangeBase = InnerDefinedName.GetReferencedRange();
				if (modelRangeBase == null || !(modelRangeBase.Worksheet is Model.Worksheet))
					return null;
				int sheetIndex = this.InnerDefinedName.Workbook.Sheets.GetIndexById(modelRangeBase.Worksheet.SheetId);
				return new NativeRange(modelRangeBase, collection.Worksheets[sheetIndex] as NativeWorksheet);
			}
			set {
				CheckValid();
				Range newValue = value;
				NativeWorksheet nativeWorksheet = collection.Worksheets[0] as NativeWorksheet;
				IWorkbook workbook = nativeWorksheet.Workbook;
				if (!Object.ReferenceEquals(workbook, newValue.Worksheet.Workbook))
					SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorUseRangeFromAnotherWorkbook);
				string newReference = String.Empty;
				if (workbook.DocumentSettings.R1C1ReferenceStyle)
					newReference = newValue.GetReferenceR1C1(ReferenceElement.ColumnAbsolute | ReferenceElement.RowAbsolute | ReferenceElement.IncludeSheetName, null);
				else
					newReference = newValue.GetReferenceA1(ReferenceElement.ColumnAbsolute | ReferenceElement.RowAbsolute | ReferenceElement.IncludeSheetName);
				RefersTo = newReference;
			}
		}
		public Worksheet Scope {
			get {
				CheckValid();
				Model.Worksheet modelWorksheet = collection.ModelDefinedNames.Worksheet;
				return modelWorksheet !=null ?  NativeWorkbook.NativeWorksheets[modelWorksheet.Name] : null;
			}
		}
		public bool IsGlobal {
			get { CheckValid(); return collection.ModelDefinedNames.Worksheet == null; }
		}
		public string Comment {
			get {
				CheckValid();
				return String.IsNullOrEmpty(InnerDefinedName.Comment) ? String.Empty : InnerDefinedName.Comment;
			}
			set {
				CheckValid();
				InnerDefinedName.Comment = value;
			}
		}
		public bool Hidden {
			get {
				CheckValid();
				return InnerDefinedName.IsHidden;
			}
			set {
				CheckValid();
				InnerDefinedName.IsHidden = value;
			}
		}
		protected internal ModelDefinedName InnerDefinedName { get { return innerDefinedName; } }
		#endregion
		public void OnModelDefinedNameExpressionChanged(object sender, EventArgs e) {
			Model.CellRangeBase newReference = InnerDefinedName.GetReferencedRange();
			this.collection.AfterDefinedNameRefersToChanged(this, newReference);
		}
		protected override void SetIsValid(bool value) {
			if(!value)
				innerDefinedName.ExpressionChanged -= OnModelDefinedNameExpressionChanged;
			base.SetIsValid(value);
		}
		#region ExternalDefinedName Members
		public ParameterValue Calculate(Spreadsheet.Functions.EvaluationContext context) {
			DevExpress.XtraSpreadsheet.Model.WorkbookDataContext modelContext = innerDefinedName.DataContext;
			modelContext.PushDefinedNameProcessing(innerDefinedName);
			NativeWorksheet nativeSheet = context.Sheet as NativeWorksheet;
			if (nativeSheet != null)
				modelContext.PushCurrentWorksheet(nativeSheet.ModelWorksheet);
			else
				modelContext.PushCurrentWorksheet(modelContext.Workbook.ActiveSheet);
			modelContext.PushCurrentCell(context.Column, context.Row);
			try {
				DevExpress.XtraSpreadsheet.Model.VariantValue modelValue = innerDefinedName.Expression.Evaluate(modelContext);
				if (modelValue.IsCellRange) {
					if (nativeSheet == null)
						return new ParameterValue(modelContext.DereferenceValue(modelValue, false), modelContext);
					if (modelValue.CellRangeValue.Worksheet == null)
						modelValue.CellRangeValue.Worksheet = nativeSheet.ModelWorksheet;
					return new ParameterValue(modelValue, nativeSheet.Workbook);
				}
				else
					return new ParameterValue(modelValue, modelContext);
			}
			finally {
				modelContext.PopCurrentCell();
				modelContext.PopDefinedNameProcessing();
				modelContext.PopDefinedNameProcessing();
			}
		}
		#endregion
		public override string ToString() {
			return String.Format("DefinedName \"{0}\"", Name);
		}
		NativeWorkbook NativeWorkbook {
			get {
				NativeWorksheet nativeWorksheet = collection.Worksheets[0] as NativeWorksheet;
				return nativeWorksheet.NativeWorkbook;
			}
		}
		public ParsedExpression ParsedExpression {
			get {
				CheckValid();
				NativeWorkbook nativeWorkbook = NativeWorkbook;
				Model.WorkbookDataContext dataContext = nativeWorkbook.DocumentModel.DataContext;
				dataContext.PushCurrentWorkbook(dataContext.Workbook, true);
				dataContext.PushCurrentCell(0, 0);
				try {
					return ParsedExpression.FromModelExporession(innerDefinedName.Expression, nativeWorkbook);
				}
				finally {
					dataContext.PopCurrentCell();
					dataContext.PopCurrentWorksheet();
				}
			}
			set {
				CheckValid();
				string formula = string.Empty;
				if (value != null) {
					Model.WorkbookDataContext dataContext = NativeWorkbook.DocumentModel.DataContext;
					dataContext.PushCurrentWorkbook(dataContext.Workbook, true);
					dataContext.PushCurrentCell(0, 0);
					try {
						RefersTo = value.ToString();
					}
					finally {
						dataContext.PopCurrentCell();
						dataContext.PopCurrentWorkbook();
					}
				}
			}
		}
	}
	#endregion
	#region NativeDefinedNameCollection
	partial class NativeDefinedNameCollection : NativeObjectBase, DefinedNameCollection {
		#region Fields
		readonly ModelWorkbookDataContext dataContext;
		int scopedSheetId;
		readonly List<NativeDefinedName> innerList;
		readonly ModelDefinedNameCollection modelDefinedNames;
		readonly Dictionary<Model.CellRangeBase, NativeDefinedName> cache;
		readonly WorksheetCollection worksheets;
		#endregion
		public NativeDefinedNameCollection(ModelDefinedNameCollection innerDefinedNames, ModelWorkbookDataContext dataContext, int scopedSheetId, WorksheetCollection worksheets) {
			Guard.ArgumentNotNull(innerDefinedNames, "innerDefinedNames");
			Guard.ArgumentNotNull(dataContext, "dataContext");
			this.dataContext = dataContext;
			this.scopedSheetId = scopedSheetId;
			this.modelDefinedNames = innerDefinedNames;
			this.innerList = new List<NativeDefinedName>();
			this.cache = new Dictionary<Model.CellRangeBase, NativeDefinedName>(); 
			this.worksheets = worksheets;
			Initialize();
		}
		#region Properties
		protected internal ModelWorkbookDataContext DataContext { get { return dataContext; } }
		public DefinedName this[int index] { get { return InnerList[index]; } }
		public int Count { get { return ModelDefinedNames.Count; } }
		protected internal ModelDefinedNameCollection ModelDefinedNames { get { return modelDefinedNames; } }
		protected internal List<NativeDefinedName> InnerList { get { return innerList; } }
		protected internal Dictionary<Model.CellRangeBase, NativeDefinedName> Cache { get { return cache; } }
		protected internal WorksheetCollection Worksheets { get { return worksheets; } }
		#endregion
		#region Internal
		protected internal void Populate() {
			InvalidateItems();
			UnsubscribeEvents();
			innerList.Clear();
			cache.Clear();
			Initialize();
		}
		protected internal void Initialize() {
			foreach(Model.DefinedNameBase item in ModelDefinedNames)
				RegisterItem(item as ModelDefinedName);
			SubscribeEvents();
		}
		void RegisterItem(ModelDefinedName item) {
			if (item == null)
				return;
			NativeDefinedName nativeDefinedName = new NativeDefinedName(item, this);
			innerList.Add(nativeDefinedName);
			item.RaiseExpressionChanged();
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
			ModelDefinedNames.OnAdd += OnAdd;
			ModelDefinedNames.OnRemoveAt += OnRemoveAt;
			ModelDefinedNames.OnInsert += OnInsert;
			ModelDefinedNames.OnClear += OnClear;
			ModelDefinedNames.OnAddRange += OnAddRange;
		}
		protected internal void UnsubscribeEvents() {
			ModelDefinedNames.OnAdd -= OnAdd;
			ModelDefinedNames.OnRemoveAt -= OnRemoveAt;
			ModelDefinedNames.OnInsert -= OnInsert;
			ModelDefinedNames.OnClear -= OnClear;
			ModelDefinedNames.OnAddRange -= OnAddRange;
		}
		void OnAdd(object sender, EventArgs e) {
			UndoableCollectionAddEventArgs<Model.DefinedNameBase> modelArgs = e as UndoableCollectionAddEventArgs<Model.DefinedNameBase>;
			if (modelArgs != null)
				RegisterItem(modelArgs.Item as ModelDefinedName);
		}
		void OnRemoveAt(object sender, UndoableCollectionRemoveAtEventArgs e) {
			int index = e.Index;
			if (index < innerList.Count) {
				NativeDefinedName nativeDefinedName = innerList[index];
				RemoveDefinedNameFromCache(nativeDefinedName);
				nativeDefinedName.IsValid = false;
				innerList.RemoveAt(index);
			}
		}
		void OnInsert(object sender, EventArgs e) {
			UndoableCollectionInsertEventArgs<Model.DefinedNameBase> modelArgs = e as UndoableCollectionInsertEventArgs<Model.DefinedNameBase>;
			if (modelArgs != null) {
				ModelDefinedName modelDefinedName = modelArgs.Item as ModelDefinedName;
				if (modelDefinedName != null) {
					NativeDefinedName nativeDefinedName = new NativeDefinedName(modelDefinedName, this);
					innerList.Insert(modelArgs.Index, new NativeDefinedName(modelDefinedName, this));
					modelDefinedName.RaiseExpressionChanged();
				}
			}
		}
		void OnClear(object sender) {
			InvalidateItems();
			innerList.Clear();
			cache.Clear();
		}
		void OnAddRange(object sender, EventArgs e) {
			UndoableCollectionAddRangeEventArgs<Model.DefinedNameBase> modelArgs = e as UndoableCollectionAddRangeEventArgs<Model.DefinedNameBase>;
			if (modelArgs == null)
				return;
			IEnumerable<Model.DefinedNameBase> collection = modelArgs.Collection;
			foreach (Model.DefinedNameBase modelItem in collection)
				RegisterItem(modelItem as ModelDefinedName);
		}
		#endregion
		#region Add
		public DefinedName Add(string name, string refersTo) {
			if (scopedSheetId == -1)
				ModelDefinedNames.Workbook.CreateDefinedName(name, refersTo);
			else
				ModelDefinedNames.Worksheet.CreateDefinedName(name, refersTo);
			return this[Count - 1];
		}
		#endregion
		#region Remove
		public void Remove(DefinedName item) {
			string name = item.Name;
			if (scopedSheetId == -1)
				ModelDefinedNames.Workbook.RemoveDefinedName(name);
			else
				ModelDefinedNames.Worksheet.RemoveDefinedName(name);
		}
		#endregion
		#region RemoveAt
		public void RemoveAt(int index) {
			string name = InnerList[index].Name;
			if (scopedSheetId == -1)
				ModelDefinedNames.Workbook.RemoveDefinedName(name);
			else
				ModelDefinedNames.Worksheet.RemoveDefinedName(name);
		}
		#endregion
		#region Remove
		public void Remove(string name) {
			if (scopedSheetId == -1)
				ModelDefinedNames.Workbook.RemoveDefinedName(name);
			else
				ModelDefinedNames.Worksheet.RemoveDefinedName(name);
		}
		#endregion
		#region Contains
		public bool Contains(string name) {
			return ModelDefinedNames.Contains(name);
		}
		#endregion
		#region Contains
		public bool Contains(DefinedName item) {
			return ModelDefinedNames.Contains(((NativeDefinedName)item).InnerDefinedName.Name);
		}
		#endregion
		#region GetDefinedName
		public DefinedName GetDefinedName(string name) {
			int innerListCount = InnerList.Count;
			for (int i = innerListCount - 1; i >= 0; i--) {
				NativeDefinedName currentDefinedName = InnerList[i];
				if (currentDefinedName.Name == name)
					return currentDefinedName;
			}
			SpreadsheetExceptions.ThrowArgumentOutOfRangeException(XtraSpreadsheetStringId.Msg_ErrorDefinedNameNotFounded, "name");
			return null;
		}
		#endregion
		#region GetDefinedNameByReference
		protected internal NativeDefinedName GetDefinedNameByReference(Model.CellRangeBase cellRange) {
			NativeDefinedName result = null;
			cache.TryGetValue(cellRange, out result);
			return result;
		}
		#endregion
		#region AfterDefinedNameRefersToChanged
		public void AfterDefinedNameRefersToChanged(NativeDefinedName definedName, Model.CellRangeBase newRefersTo) {
			Guard.ArgumentNotNull(definedName, "definedName");
			RemoveDefinedNameFromCache(definedName);
			if (newRefersTo == null)
				return;
			if (!cache.ContainsKey(newRefersTo))
				cache.Add(newRefersTo, definedName);
			else
				cache[newRefersTo] = definedName;
		}
		protected internal void RemoveDefinedNameFromCache(NativeDefinedName definedName) {
			foreach (KeyValuePair<Model.CellRangeBase, NativeDefinedName> item in cache) {
				if (object.ReferenceEquals(item.Value, definedName)) {
					cache.Remove(item.Key);
					break;
				}
			}
		}
		#endregion
		#region IndexOf
		public int IndexOf(DefinedName item) {
			return InnerList.IndexOf((NativeDefinedName)item);
		}
		#endregion
		#region Clear
		public void Clear() {
			if (scopedSheetId == -1)
				ModelDefinedNames.Workbook.ClearDefinedNames();
			else
				ModelDefinedNames.Worksheet.ClearDefinedNames();
		}
		#endregion
		#region GetEnumerator
		public IEnumerator<DefinedName> GetEnumerator() {
			return new EnumeratorAdapter<DefinedName, NativeDefinedName>(innerList.GetEnumerator());
		}
		#endregion
		#region IEnumerable.GetEnumerator
		IEnumerator IEnumerable.GetEnumerator() {
			return innerList.GetEnumerator();
		}
		#endregion
		#region ICollection members
		object ICollection.SyncRoot {
			get {
				ICollection collection = innerList;
				return collection.SyncRoot;
			}
		}
		bool ICollection.IsSynchronized {
			get {
				ICollection collection = innerList;
				return collection.IsSynchronized;
			}
		}
		void ICollection.CopyTo(Array array, int index) {
			Array.Copy(innerList.ToArray(), 0, array, index, innerList.Count);
		}
		#endregion
	}
	#endregion
}
#endregion
