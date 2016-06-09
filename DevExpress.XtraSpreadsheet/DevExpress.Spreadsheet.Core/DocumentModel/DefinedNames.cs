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
using DevExpress.Utils;
using System.Text;
using System.Diagnostics;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.Office.History;
using DevExpress.Office;
namespace DevExpress.XtraSpreadsheet.Model {
	#region DefinedNameBase (abstract class)
	public abstract class DefinedNameBase : ISpreadsheetNamedObject{
		#region Fields
		readonly IModelWorkbook workbook;
		string name = String.Empty;
		ParsedExpression expression;
		int scopedSheetId;
		#endregion
		#region AbsoluteExpressionTypeTable
		static HashSet<Type> absoluteExpressionTypeTable = CreateAbsoluteExpressionTypeTable();
		static HashSet<Type> CreateAbsoluteExpressionTypeTable() {
			HashSet<Type> result = new HashSet<Type>();
			result.Add(typeof(Model.ParsedThingRefRel));
			result.Add(typeof(Model.ParsedThingRef3dRel));
			result.Add(typeof(Model.ParsedThingAreaN));
			result.Add(typeof(Model.ParsedThingArea3dRel));
			result.Add(typeof(Model.ParsedThingRange));
			result.Add(typeof(Model.ParsedThingUnion));
			result.Add(typeof(Model.ParsedThingIntersect));
			result.Add(typeof(Model.ParsedThingAttrSpace));
			result.Add(typeof(Model.ParsedThingAttrSpaceSemi));
			result.Add(typeof(Model.ParsedThingTable));
			result.Add(typeof(Model.ParsedThingTableExt));
			return result;
		}
		#endregion
		DefinedNameBase(string name, IModelWorkbook workbook, int scopedSheetId) {
			Guard.ArgumentNotNull(workbook, "workbook");
			Guard.ArgumentIsNotNullOrEmpty(name, "name");
			this.workbook = workbook;
			this.scopedSheetId = scopedSheetId;
			this.name = name;
		}
		protected DefinedNameBase(string name, IModelWorkbook workbook, string reference, int scopedSheetId)
			: this(name, workbook, scopedSheetId) {
			this.expression = CreateExpression(reference);
		}
		protected DefinedNameBase(string name, IModelWorkbook workbook, ParsedExpression expression, int scopedSheetId)
			: this(name, workbook, scopedSheetId) {
			this.expression = expression;
		}
		#region Properties
		#region Expression
		public ParsedExpression Expression {
			get {
				return expression;
			}
			set {
				if (expression == value)
					return;
				SetExpression(value);
			}
		}
		void SetExpression(ParsedExpression value) {
			DocumentHistory history = Workbook.DataContext.Workbook.History;
			DefinedNameExpressionChangedHistoryItem historyItem = new DefinedNameExpressionChangedHistoryItem(this, Expression, value);
			history.Add(historyItem);
			historyItem.Execute();
		}
		protected internal virtual void SetExpressionCore(ParsedExpression value) {
			this.expression = value;
			RaiseExpressionChanged();
		}
		protected internal virtual void SetExpressionWithoutNotifications(ParsedExpression value) {
			this.expression = value;
		}
		#endregion
		#region Name
		public string Name {
			get { return name; }
			set {
				if (name == value)
					return;
				SetName(value);
			}
		}
		protected internal virtual void SetName(string newName) {
			SetNameCore(newName);
		}
		protected internal void SetNameCore(string name) {
			string oldName = this.name;
			this.name = name;
			RaiseNameChanged(name, oldName);
		}
		#region NameChanged
		NameChangedEventHandler onNameChanged;
		public event NameChangedEventHandler NameChanged { add { onNameChanged += value; } remove { onNameChanged -= value; } }
		protected internal virtual void RaiseNameChanged(string name, string oldName) {
			if (onNameChanged != null) {
				NameChangedEventArgs args = new NameChangedEventArgs(name, oldName);
				onNameChanged(this, args);
			}
		}
		#endregion
		#endregion
		protected internal int ScopedSheetId { get { return scopedSheetId; } }
		protected internal IModelWorkbook Workbook { get { return workbook; } }
		protected internal WorkbookDataContext DataContext { get { return workbook.DataContext; } }
		#endregion
		public void SetReference(string value) {
			Expression = CreateExpression(value);
		}
		internal void SetReferenceCore(string value) {
			SetExpressionWithoutNotifications(CreateExpression(value));
		}
		ParsedExpression CreateExpression(string expressionText) {
			if (String.IsNullOrEmpty(expressionText))
				return null;
			DataContext.PushDefinedNameProcessing(this);
			DataContext.PushCurrentWorkbook(workbook);
			try {
				return DataContext.ParseExpression(expressionText, OperandDataType.Default, true);
			}
			finally {
				DataContext.PopDefinedNameProcessing();
				DataContext.PopCurrentWorkbook();
			}
		}
		public string GetReference(ICell cell) {
			return GetReference(cell.ColumnIndex, cell.RowIndex);
		}
		public string GetReference(int currentColumnIndex, int currentRowIndex) {
			if (expression == null)
				return String.Empty;
			WorkbookDataContext context = DataContext;
			context.PushDefinedNameProcessing(this);
			context.PushCurrentWorkbook(workbook);
			context.PushCurrentCell(currentColumnIndex, currentRowIndex);
			try {
				return expression.BuildExpressionString(context);
			}
			finally {
				context.PopCurrentCell();
				context.PopCurrentWorkbook();
				context.PopDefinedNameProcessing();
			}
		}
		public virtual void OnRangeRemoving(RemoveRangeNotificationContext context) {
		}
		public virtual void OnRangeInserting(InsertRangeNotificationContext context) {
		}
		protected internal abstract DefinedNameBase Clone();
		#region ExpressionChanged event
		EventHandler onExpressionChanged;
		internal event EventHandler ExpressionChanged { add { onExpressionChanged += value; } remove { onExpressionChanged -= value; } }
		protected internal virtual void RaiseExpressionChanged() {
			if (onExpressionChanged != null) {
				onExpressionChanged(this, new EventArgs());
			}
		}
		#endregion
		protected internal virtual DefinedNameCollectionBase GetOwnerCollection() {
			if (scopedSheetId < 0)
				return workbook.DefinedNames;
			IWorksheet sheet = workbook.GetSheetById(scopedSheetId);
			System.Diagnostics.Debug.Assert(sheet != null);
			return sheet.DefinedNames;
		}
		protected internal IWorksheet GetScopedSheet() {
			if (scopedSheetId < 0)
				return null;
			return workbook.GetSheetById(scopedSheetId);
		}
		protected internal VariantValue Evaluate(WorkbookDataContext context) {
			if (Expression == null)
				return VariantValue.ErrorName;
			context.Workbook.RealTimeDataManager.OnStartDefinedNameEvaluation();
			context.PushDefinedNameProcessing(this);
			try {
				return Expression.Evaluate(context);
			}
			finally {
				context.PopDefinedNameProcessing();
				context.Workbook.RealTimeDataManager.OnEndDefinedNameEvaluation();
			}
		}
		protected internal virtual CellRangeBase GetReferencedRange() {
			WorkbookDataContext context = DataContext;
			context.PushCurrentCell(0, 0);
			try {
				return GetReferencedRangeCore();
			}
			finally {
				context.PopCurrentCell();
			}
		}
		protected internal virtual CellRangeBase GetReferencedRange(ICell currentCell) {
			return GetReferencedRange(currentCell.Position, currentCell.Worksheet);
		}
		protected internal virtual CellRangeBase GetReferencedRange(CellPosition currentCellPosition, Worksheet currentSheet) {
			WorkbookDataContext context = DataContext;
			context.PushCurrentWorksheet(currentSheet);
			context.PushCurrentCell(currentCellPosition);
			try {
				return GetReferencedRangeCore();
			}
			finally {
				context.PopCurrentCell();
				context.PopCurrentWorksheet();
			}
		}
		protected internal virtual CellRangeBase GetReferencedRangeCore() {
			ParsedExpression expression = Expression;
			if (expression == null || expression.Count <= 0)
				return null;
			for (int i = 0; i < expression.Count; i++) {
				IParsedThing thing = expression[i];
				if (!(thing is Model.ParsedThingMemBase) && !(thing is Model.ParsedThingAttrBase) && !absoluteExpressionTypeTable.Contains(thing.GetType()))
					return null;
			}
			WorkbookDataContext context = DataContext;
			context.PushDefinedNameProcessing(this);
			try {
				VariantValue value = expression.Evaluate(context);
				if (value.IsCellRange)
					return value.CellRangeValue;
			}
			finally {
				context.PopDefinedNameProcessing();
			}
			return null;
		}
		protected internal VariantValue EvaluateToSimpleValue(WorkbookDataContext context) {
			VariantValue value = Evaluate(context);
			return context.DereferenceValue(value, true);
		}
		protected internal virtual ReferencesCache GetReferencesCache(WorkbookDataContext context) {
			return new ReferencesCache();
		}
		internal virtual void InvalidateReferencesCache() {
			InvalidateReferencesCacheCore();
		}
		internal virtual void InvalidateReferencesCacheCore() {
		}
	}
	#endregion
	#region DefinedName
	public class DefinedName : DefinedNameBase {
		#region Fields
		bool isHidden;
		string comment;
		bool isXlmMacro;
		bool isVbaMacro;
		bool isMacro;
		int functionGroupId;
		ReferencesCache referencesCache;
		#endregion
		public DefinedName(DocumentModel workbook, string name, string reference, int scopedsheetId)
			: base(name, workbook, reference, scopedsheetId) {
			this.isHidden = false;
			this.comment = String.Empty;
		}
		public DefinedName(DocumentModel workbook, string name, ParsedExpression expression, int scopedsheetId)
			: base(name, workbook, expression, scopedsheetId) {
			this.isHidden = false;
			this.comment = String.Empty;
		}
		#region Properties
		protected internal new DocumentModel Workbook { get { return (DocumentModel)base.Workbook; } }
		#region IsHidden
		public bool IsHidden {
			get { return isHidden; }
			set {
				if (isHidden == value)
					return;
				SetIsHidden(value);
			}
		}
		void SetIsHidden(bool value) {
			DocumentHistory history = Workbook.History;
			DefinedIsHiddenChangeIsHiddenHistoryItem historyItem = new DefinedIsHiddenChangeIsHiddenHistoryItem(this, isHidden, value);
			history.Add(historyItem);
			historyItem.Execute();
			Workbook.RaiseSchemaChanged();
		}
		internal void SetIsHiddenCore(bool isHidden) {
			this.isHidden = isHidden;
		}
		#endregion
		#region Comment
		public string Comment {
			get { return comment; }
			set {
				if (comment == value)
					return;
				SetComment(value);
			}
		}
		void SetComment(string value) {
			DocumentHistory history = Workbook.History;
			DefinedCommentChangeCommentHistoryItem historyItem = new DefinedCommentChangeCommentHistoryItem(this, comment, value);
			history.Add(historyItem);
			historyItem.Execute();
		}
		internal void SetCommentCore(string comment) {
			this.comment = comment;
		}
		#endregion
		protected internal bool IsXlmMacro { get { return isXlmMacro; } set { isXlmMacro = value; } }
		protected internal bool IsVbaMacro { get { return isVbaMacro; } set { isVbaMacro = value; } }
		protected internal bool IsMacro { get { return isMacro; } set { isMacro = value; } }
		protected internal int FunctionGroupId { get { return functionGroupId; } set { functionGroupId = value; } }
		#endregion
		protected internal override void SetName(string newName) {
			Workbook.CheckDefinedName(newName, ScopedSheetId);
			DefinedNameRenamedCommand command = new DefinedNameRenamedCommand(this, newName);
			command.Execute();
		}
		public override void OnRangeRemoving(RemoveRangeNotificationContext notificationContext) {
			RemoveCellMode mode = notificationContext.Mode;
			if (mode == RemoveCellMode.ShiftCellsLeft)
				UpdateExpression(new RemovedShiftLeftDefinedNameRPNVisitor(notificationContext, DataContext));
			else if (mode == RemoveCellMode.ShiftCellsUp)
				UpdateExpression(new RemovedShiftUpDefinedNameRPNVisitor(notificationContext, DataContext));
			else if (mode == RemoveCellMode.NoShiftOrRangeToPasteCutRange)
				UpdateExpression(new RemovedNoShiftRPNVisitor(notificationContext, DataContext));
		}
		public override void OnRangeInserting(InsertRangeNotificationContext notificationContext) {
			if (notificationContext.Mode == InsertCellMode.ShiftCellsRight)
				UpdateExpression(new InsertedShiftRightDefinedNameRPNVisitor(notificationContext, DataContext));
			else
				UpdateExpression(new InsertedShiftDownDefinedNameRPNVisitor(notificationContext, DataContext));
		}
		protected internal virtual void UpdateExpression(ReplaceThingsPRNVisitor walker) {
			if (Expression == null)
				return;
			ParsedExpression updatedExpression = walker.Process(Expression.Clone());
			if (walker.FormulaChanged) {
				this.Expression = updatedExpression;
			}
		}
		protected internal override DefinedNameBase Clone() {
			DefinedName result = new DefinedName(Workbook, Name, Expression.Clone(), ScopedSheetId);
			result.SetIsHiddenCore(IsHidden);
			result.SetCommentCore(Comment);
			result.IsXlmMacro = IsXlmMacro;
			result.IsVbaMacro = IsVbaMacro;
			result.IsMacro = IsMacro;
			result.FunctionGroupId = FunctionGroupId;
			return result;
		}
		protected internal override void SetExpressionCore(ParsedExpression value) {
			base.SetExpressionCore(value);
			InvalidateReferencesCache();
			Workbook.CalculationChain.ForceCalculate();
		}
		protected internal override ReferencesCache GetReferencesCache(WorkbookDataContext context) {
			if (referencesCache == null) {
				DefinedNameReferencedRangesRPNVisitor visitor = new DefinedNameReferencedRangesRPNVisitor(context);
				referencesCache = visitor.Perform(Expression);
			}
			return referencesCache;
		}
		internal override void InvalidateReferencesCache() {
			InvalidateReferencesCacheCore();
			Workbook.DefinedNamesRefencesCacheManager.OnDefinedNameReferenceCacheInvalidated(this);
		}
		internal override void InvalidateReferencesCacheCore() {
			referencesCache = null;
		}
	}
	#endregion
	public class ReferencesCache {
		public List<CellRange> RangeReferences { get; set; }
		public List<DefinedNameReference> DefinedNameReferences { get; set; }
		internal CellRangeBase Calculate(WorkbookDataContext context) {
			List<ReferencesCache> relatedReferencesCaches = CollectRelatedReferences(context);
			return ProcessReferencesList(relatedReferencesCaches, context);
		}
		List<ReferencesCache> CollectRelatedReferences(WorkbookDataContext context) {
			List<ReferencesCache> resultList = new List<ReferencesCache>();
			resultList.Add(this);
			AddRelatedDefinedNamesTo(resultList, context);
			return resultList;
		}
		protected internal void AddRelatedDefinedNamesTo(List<ReferencesCache> resultList, WorkbookDataContext context) {
			if (DefinedNameReferences == null)
				return;
			foreach (DefinedNameReference reference in DefinedNameReferences) {
				ReferencesCache definedNameReference = LookupDefinedNameReferences(reference, context);
				if (definedNameReference != null && !resultList.Contains(definedNameReference)) {
					resultList.Add(definedNameReference);
					definedNameReference.AddRelatedDefinedNamesTo(resultList, context);
				}
			}
		}
		ReferencesCache LookupDefinedNameReferences(DefinedNameReference referenceToDefinedName, WorkbookDataContext context) {
			DefinedNameBase definedName;
			int sheetId = referenceToDefinedName.SheetId;
			string name = referenceToDefinedName.Name;
			if (sheetId < 0) {
				definedName = context.GetDefinedName(name);
			}
			else {
				IWorksheet sheet = context.Workbook.GetSheetById(sheetId);
				if (sheet == null)
					return null;
				definedName = context.GetDefinedNameForWorksheet(name, sheet);
			}
			if (definedName == null)
				return null;
			return definedName.GetReferencesCache(context);
		}
		CellRangeBase ProcessReferencesList(List<ReferencesCache> relatedReferencesCaches, WorkbookDataContext context) {
			List<CellRangeBase> result = new List<CellRangeBase>();
			foreach (ReferencesCache cache in relatedReferencesCaches)
				cache.AddReferencesTo(result, context);
			if (result.Count == 0)
				return null;
			if (result.Count == 1)
				return result[0];
			return new CellUnion(result);
		}
		void AddReferencesTo(List<CellRangeBase> where, WorkbookDataContext context) {
			if (RangeReferences != null) {
				foreach (CellRange range in RangeReferences) {
					CellRange processingRange = range;
					if (processingRange.Worksheet == null)
						processingRange = (CellRange)range.Clone(context.CurrentWorksheet);
					else
						if (!object.ReferenceEquals(processingRange.Worksheet, context.CurrentWorksheet))
							continue;
					processingRange = processingRange.GetRelativeToCurrent(context.CurrentColumnIndex, context.CurrentRowIndex);
					where.Add(processingRange);
				}
			}
		}
	}
	public class DefinedNameReference : IEquatable<DefinedNameReference> {
		public DefinedNameReference(DefinedNameBase definedName) {
			this.Name = definedName.Name;
			this.SheetId = definedName.ScopedSheetId;
		}
		public DefinedNameReference(string name, int sheetId) {
			this.Name = name;
			this.SheetId = sheetId;
		}
		public string Name { get; set; }
		public int SheetId { get; set; } 
		public bool Equals(DefinedNameReference other) {
			if (other == null)
				return false;
			return StringExtensions.CompareInvariantCultureIgnoreCase(Name, other.Name) == 0 &&
				SheetId == other.SheetId;
		}
		public override bool Equals(object obj) {
			return ((IEquatable<DefinedNameReference>)this).Equals(obj as DefinedNameReference);
		}
		public override int GetHashCode() {
			return HashCodeCalculator.CalcHashCode32(this.Name.GetHashCode(), this.SheetId.GetHashCode());
		}
	}
	#region DefinedNameCollectionBase (abstract class)
	public abstract class DefinedNameCollectionBase : UndoableNamedItemCollection<DefinedNameBase> {
		protected DefinedNameCollectionBase(IDocumentModelPart documentModelPart)
			: base(documentModelPart, StringExtensions.ComparerInvariantCultureIgnoreCase) {
		}
		#region Properties
		protected internal DocumentModel Workbook { get { return (DocumentModel)base.DocumentModel; } }
		#endregion
		protected override void OnModified() {
			Workbook.IncrementContentVersion();
		}
		public virtual void OnRangeRemoving(RemoveRangeNotificationContext context) {
			if (context.Mode == RemoveCellMode.Default)
				return;
			foreach (DefinedNameBase item in this)
				item.OnRangeRemoving(context);
		}
		public virtual void OnRangeInserting(InsertRangeNotificationContext context) {
			foreach (DefinedNameBase item in this)
				item.OnRangeInserting(context);
		}
	}
	#endregion
	#region DefinedNameCollection
	public class DefinedNameCollection : DefinedNameCollectionBase {
		public DefinedNameCollection(IDocumentModelPart documentModelPart)
			: base(documentModelPart) {
		}
		public Worksheet Worksheet { get { return DocumentModelPart as Worksheet; } }
		public override DefinedNameBase GetCloneItem(DefinedNameBase item, IDocumentModelPart documentModelPart) {
			return item.Clone();
		}
		public override UndoableClonableCollection<DefinedNameBase> GetNewCollection(IDocumentModelPart documentModelPart) {
			return new DefinedNameCollection(documentModelPart);
		}
		public override int AddCore(DefinedNameBase item) {
			int result = base.AddCore(item);
			DefinedName definedName = item as DefinedName;
			if (definedName != null) {
				if (Worksheet == null)
					Workbook.InternalAPI.OnDefinedNameWorkbookAdd(Workbook, definedName);
				else
					Workbook.InternalAPI.OnDefinedNameWorksheetAdd(Worksheet, definedName);
				Workbook.RaiseSchemaChanged();
			}
			return result;
		}
		public override void AddRangeCore(IEnumerable<DefinedNameBase> collection) {
			base.AddRangeCore(collection);
			Workbook.RaiseSchemaChanged();
		}
		protected  override void InsertCore(int index, DefinedNameBase item) {
			base.InsertCore(index, item);
			Workbook.RaiseSchemaChanged();
		}
		public override void RemoveAtCore(int index) {
			DefinedName definedName = this[index] as DefinedName;
			base.RemoveAtCore(index);
			if (definedName != null) {
				if (Worksheet == null)
					Workbook.InternalAPI.OnDefinedNameWorkbookRemove(Workbook, definedName);
				else
					Workbook.InternalAPI.OnDefinedNameWorksheetRemove(Worksheet, definedName);
				Workbook.RaiseSchemaChanged();
			}
		}
		public override void ClearCore() {
			if (Worksheet == null)
				Workbook.InternalAPI.OnDefinedNameWorkbookCollectionClear(Workbook);
			else
				Workbook.InternalAPI.OnDefinedNameWorksheetCollectionClear(Worksheet);
			base.ClearCore();
			Workbook.RaiseSchemaChanged();
		}
		protected internal void ClearNoSchemaChanged() {
			base.ClearCore();
		}
	}
	#endregion
	public class DefinedNamesRefencesCacheManager {
		readonly DocumentModel documentModel;
		public DefinedNamesRefencesCacheManager(DocumentModel documentModel) {
			this.documentModel = documentModel;
		}
		public virtual void Invalidate() {
			InvalidateDefinedNameCollection(documentModel.DefinedNames);
			foreach (Worksheet sheet in documentModel.Sheets) {
				InvalidateDefinedNameCollection(sheet.DefinedNames);
			}
		}
		void InvalidateDefinedNameCollection(DefinedNameCollection collection) {
			foreach (DefinedName definedName in collection)
				definedName.InvalidateReferencesCacheCore();
		}
		public void OnDefinedNameReferenceCacheInvalidated(DefinedName definedName) {
		}
	}
}
