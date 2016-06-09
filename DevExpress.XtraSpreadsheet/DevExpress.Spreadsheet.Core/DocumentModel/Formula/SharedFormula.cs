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

using DevExpress.Office.History;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace DevExpress.XtraSpreadsheet.Model {
	#region SharedFormula
	public class SharedFormula : IFormula, IChainPrecedent {
		#region Fields
		ParsedExpression expression;
		bool hasCompliant = false;
		bool isCompliant;
		CellRange range;
		#endregion
		protected SharedFormula(CellRange range) {
			Guard.ArgumentNotNull(range, "range");
			this.range = range;
		}
		public SharedFormula(ICell hostCell, string formulaBody, CellRange range)
			: this(range) {
			Guard.ArgumentIsNotNullOrEmpty(formulaBody, "formulaBody");
			ParseExpression(formulaBody, hostCell);
		}
		public SharedFormula(ParsedExpression expression, CellRange range)
			: this(range) {
			Guard.ArgumentNotNull(range, "range");
			this.expression = expression;
		}
		#region Properties
		public Worksheet Worksheet { get { return (Worksheet)range.Worksheet; } }
		protected internal bool IsFormulaCompliant {
			get {
				if (!hasCompliant) {
					hasCompliant = true;
					isCompliant = Expression.IsXlsSharedFormulaCompliant();
				}
				return isCompliant;
			}
		}
		public ParsedExpression Expression { get { return expression; } set { SetExpression(value); } }
		public FormulaType Type { get { return FormulaType.Shared; } }
		public CellRange Range { get { return range; } }
		#endregion
		#region GetBody
		public string GetBody(ICell hostCell) {
			WorkbookDataContext context = hostCell.Context;
			context.PushCurrentCell(hostCell);
			try {
				return '=' + expression.BuildExpressionString(context);
			}
			finally {
				context.PopCurrentCell();
			}
		}
		#endregion
		#region Expression
		void SetExpression(ParsedExpression value) {
			DocumentModel documentModel = Worksheet.Workbook;
			SharedFormulaExpressionChangedHistoryItem historyItem = new SharedFormulaExpressionChangedHistoryItem(documentModel, this, value);
			documentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetExpressionCore(ParsedExpression value) {
			this.expression = value;
		}
		protected void ParseExpression(string formulaBody, ICell hostCell) {
			expression = null;
			WorkbookDataContext dataContext = hostCell.Context;
			try {
				dataContext.PushCurrentCell(hostCell);
				dataContext.PushSharedFormulaProcessing(true);
				ParsedExpression ptgs = dataContext.ParseExpression(formulaBody, OperandDataType.Value, true);
				if (ptgs != null && ptgs.Count > 0)
					SetExpressionCore(ptgs);
				else
					throw new ArgumentException(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorFormula));
			}
			finally {
				dataContext.PopSharedFormulaProcessing();
				dataContext.PopCurrentCell();
			}
		}
		#endregion
		#region Calculate
		public VariantValue Calculate(ICell cell, WorkbookDataContext context) {
			Debug.Assert(Expression != null);
			return FormulaCalculator.Calculate(Expression, cell, context);
		}
		#endregion
		#region Notification
		public void OnRangeInserting(InsertRangeNotificationContext notificationContext) {
			WorkbookDataContext dataContext = Worksheet.Workbook.DataContext;
			RemovedShiftLeftSharedFormulaRPNVisitor walker;
			if (notificationContext.Mode == InsertCellMode.ShiftCellsDown)
				walker = new InsertedShiftDownSharedFormulaRPNVisitor(notificationContext, dataContext);
			else
				walker = new InsertedShiftRightSharedFormulaRPNVisitor(notificationContext, dataContext);
			ProcessFormula(walker);
		}
		public void OnRangeRemoving(RemoveRangeNotificationContext notificationContext) {
			RemoveCellMode mode = notificationContext.Mode;
			if (mode == RemoveCellMode.NoShiftOrRangeToPasteCutRange) {
				RemovedShiftLeftSharedFormulaRPNVisitor walker = RemovedShiftLeftSharedFormulaRPNVisitor.GetWalkerShared(notificationContext, Worksheet.Workbook.DataContext);
				OnRangeRemovingNoShiftOrRangeToPasteCutRange(notificationContext, walker);
			}
			else if (mode == RemoveCellMode.Default) 
				OnRangeRemovingDefault(notificationContext);
			else {
				RemovedShiftLeftSharedFormulaRPNVisitor walker = RemovedShiftLeftSharedFormulaRPNVisitor.GetWalkerShared(notificationContext, Worksheet.Workbook.DataContext);
				ProcessFormula(walker);
			}
		}
		void OnRangeRemovingNoShiftOrRangeToPasteCutRange(RemoveRangeNotificationContext notificationContext, RemovedShiftLeftSharedFormulaRPNVisitor walker) {
			CellRange formulaRange = range;
			CellRange changingRange = notificationContext.Range;
			CellRangeBase allAffectedRange = formulaRange.Intersection(changingRange);
			if (allAffectedRange == null || !allAffectedRange.IsEqualSurfacesWith(formulaRange)) {
				List<CellRange> referencedRanges = GetReferencedRanges();
				foreach (CellRange referencedRange in referencedRanges) {
					CellRange enlargedReferencedRange = EnlargeReferencedRange(referencedRange, formulaRange.Width, formulaRange.Height);
					if (enlargedReferencedRange.Intersects(changingRange)) {
						CellRange formulaAffectedRangeByReferencedRange = GetFormulaNotificationAffectedRangeByReferencedRange(referencedRange, changingRange, walker, enlargedReferencedRange);
						if (formulaAffectedRangeByReferencedRange != null) {
							allAffectedRange = allAffectedRange == null ? formulaAffectedRangeByReferencedRange : allAffectedRange.MergeWithRange(formulaAffectedRangeByReferencedRange);
						}
					}
				}
			}
			if (allAffectedRange == null)
				return;
			RemoveCellsFromAffectedRange(allAffectedRange);
			CellRangeBase newFormulaRange = formulaRange.ExcludeRange(allAffectedRange);
			ChangeRangeAndConvertCellReferences(newFormulaRange);
		}
		void OnRangeRemovingDefault(RemoveRangeNotificationContext notificationContext) {
			CellRange formulaRange = range;
			CellRange changingRange = notificationContext.Range;
			CellRange formulaAffectedRange = formulaRange.Intersection(changingRange);
			if (formulaAffectedRange == null)
				return;
			CellRangeBase newFormulaRange = formulaRange.ExcludeRange(formulaAffectedRange);
			if (newFormulaRange != null) {
				SetRange(newFormulaRange.GetCoveredRange());
			}
			else
				SetRange(null);
		}
		void ProcessFormula(RemovedShiftLeftSharedFormulaRPNVisitor walker) {
			CellRange formulaRange = range.Clone();
			CellRange changingRange = walker.Logic.GetChangingRange();
			CellRange changedFormulaAndReferencedRange = null;
			CellRange formulaAffectedRange = formulaRange.Intersection(changingRange);
			CellRangeBase allAffectedRange = null;
			CellRangeBase allDeletedRange = null;
			if (formulaAffectedRange != null) {
				changedFormulaAndReferencedRange = formulaAffectedRange.Clone();
				allAffectedRange = formulaAffectedRange.Clone();
			}
			if (walker.RemoveBehavour)
				allDeletedRange = formulaRange.Intersection(walker.Logic.ModifyingRange);
			List<CellRange> referencedRanges = GetReferencedRanges();
			foreach (CellRange referencedRange in referencedRanges) {
				CellRange enlargedReferencedRange = EnlargeReferencedRange(referencedRange, formulaRange.Width, formulaRange.Height);
				if (enlargedReferencedRange.Intersects(changingRange)) {
					CellRange formulaAffectedRangeByReferencedRange = GetFormulaNotificationAffectedRangeByReferencedRange(referencedRange, changingRange, walker, enlargedReferencedRange);
					if (formulaAffectedRangeByReferencedRange != null) {
						allAffectedRange = allAffectedRange == null ? formulaAffectedRangeByReferencedRange : allAffectedRange.MergeWithRange(formulaAffectedRangeByReferencedRange);
						if (changedFormulaAndReferencedRange != null) {
							bool processingByColumns = walker.Logic.IsProcessingByColumns;
							if ((processingByColumns && referencedRange.TopLeft.ColumnType == PositionType.Absolute) ||
							   (!processingByColumns && referencedRange.TopLeft.RowType == PositionType.Absolute))
								formulaAffectedRangeByReferencedRange = ModifyReferencedRangeForAbsoluteReference(formulaAffectedRange, formulaAffectedRangeByReferencedRange);
						}
					}
					if (formulaAffectedRangeByReferencedRange != null && changedFormulaAndReferencedRange != null)
						changedFormulaAndReferencedRange = changedFormulaAndReferencedRange.Intersection(formulaAffectedRangeByReferencedRange);
					else
						changedFormulaAndReferencedRange = null;
					if (walker.RemoveBehavour && enlargedReferencedRange.Intersects(walker.Logic.ModifyingRange)) {
						CellRange formulaDeletedRangeByReferencedRange = GetFormulaNotificationAffectedRangeByReferencedRange(referencedRange, walker.Logic.ModifyingRange, walker, enlargedReferencedRange);
						if (formulaDeletedRangeByReferencedRange != null)
							allDeletedRange = allDeletedRange == null ? formulaDeletedRangeByReferencedRange : allDeletedRange.MergeWithRange(formulaDeletedRangeByReferencedRange);
					}
				}
				else
					changedFormulaAndReferencedRange = null;
			}
			if (allAffectedRange == null && allDeletedRange == null)
				return;
			if (changedFormulaAndReferencedRange != null)
				allAffectedRange = allAffectedRange.ExcludeRange(changedFormulaAndReferencedRange);
			CellRangeBase resultAffectedRange = CalculateResultAffectedRange(allDeletedRange, allAffectedRange);
			CellRangeBase newFormulaRange = null;
			CellRangeBase formulaAffectedProcessedRange = null;
			if (resultAffectedRange != null) {
				if (resultAffectedRange.CellCount != formulaRange.CellCount) {
					RemoveCellsFromAffectedRange(resultAffectedRange);
					if (formulaAffectedRange != null) {
						CellRangeBase formulaAffectedRangeExcludeRange = formulaAffectedRange.ExcludeRange(resultAffectedRange);
						if (formulaAffectedRangeExcludeRange != null) {
							formulaAffectedRange = formulaAffectedRangeExcludeRange.GetCoveredRange();
							formulaAffectedProcessedRange = walker.ProcessCellRange(formulaAffectedRange);
						}
					}
					newFormulaRange = formulaRange.ExcludeRange(resultAffectedRange);
				}
				else {
					if (allDeletedRange == null)
						newFormulaRange = formulaRange;
					else {
						RemoveCellsFromAffectedRange(allDeletedRange);
						newFormulaRange = formulaRange.ExcludeRange(allDeletedRange);
					}
					if (formulaAffectedRange != null)
						formulaAffectedProcessedRange = walker.ProcessCellRange(formulaAffectedRange);
				}
			}
			else {
				formulaAffectedProcessedRange = walker.ProcessCellRange(formulaAffectedRange);
				newFormulaRange = formulaRange.ExcludeRange(formulaAffectedRange);
			}
			if (newFormulaRange != null) {
				CellPosition startPosition = newFormulaRange.GetFirstInnerCellRange().TopLeft;
				CellPosition endPosition = walker.ProcessCellRange(new CellRange(Worksheet, startPosition, startPosition)).TopLeft;
				newFormulaRange = walker.ProcessCellRange(newFormulaRange.GetCoveredRange());
				if (formulaAffectedProcessedRange != null)
					newFormulaRange = newFormulaRange.UnionWith(formulaAffectedProcessedRange).CellRangeValue;
				SetRange(newFormulaRange.GetCoveredRange());
				CorrectSharedFormula(startPosition, endPosition, walker);
			}
			else
				SetRange(null);
		}
		CellRange ModifyReferencedRangeForAbsoluteReference(CellRange formulaAffectedRange, CellRange formulaAffectedRangeByReferencedRange) {
			CellRangeBase tempRange = formulaAffectedRange.ExcludeRange(formulaAffectedRangeByReferencedRange);
			if (tempRange != null)
				return new CellRange(Worksheet, tempRange.TopLeft, tempRange.BottomRight);
			return null;
		}
		CellRangeBase CalculateResultAffectedRange(CellRangeBase allDeletedRange, CellRangeBase allAffectedRange) {
			CellRangeBase resultAffectedRange = allAffectedRange;
			if (allDeletedRange != null) {
				resultAffectedRange = allDeletedRange.Clone();
				if (allAffectedRange != null)
					resultAffectedRange = resultAffectedRange.MergeWithRange(allAffectedRange);
			}
			return resultAffectedRange;
		}
		protected internal void RemoveCellsFromAffectedRange(CellRangeBase allAffectedRange) {
			if (allAffectedRange.RangeType == CellRangeType.UnionRange)
				foreach (CellRangeBase innerCellRange in ((CellUnion)allAffectedRange).InnerCellRanges) {
					RemoveCellsFromAffectedRange(innerCellRange);
				}
			else {
				CellRange affectedRange = (CellRange)allAffectedRange;
				foreach (ICell cell in affectedRange.GetExistingCellsEnumerable()) {
					if (cell.FormulaType == FormulaType.Shared) {
						SharedFormulaRef sharedFormulaRef = (SharedFormulaRef)cell.Formula;
						if (object.ReferenceEquals(sharedFormulaRef.HostSharedFormula, this))
							RemoveGuestCell(cell);
					}
				}
			}
		}
		void RemoveGuestCell(ICell cell) {
			((SharedFormulaRef)cell.GetFormula()).ConvertToNormalFormula();
		}
		public CellRange GetFormulaAffectedRangeByReferencedRange(CellRange referencedRange, ISheetPosition affectedRange) {
			RangeAffectedByReferenceCalculator calculator = new RangeAffectedByReferenceCalculator(range);
			return calculator.GetResult(referencedRange, affectedRange);
		}
		public CellRange GetFormulaNotificationAffectedRangeByReferencedRange(CellRange referencedRange, ISheetPosition affectedRange, RemovedShiftLeftSharedFormulaRPNVisitor walker, CellRange referencedEnlargedRange) {
			RangeAffectedByReferenceCalculator calculator = new RangeAffectedByReferenceCalculator(range);
			return calculator.GetResult(referencedRange, affectedRange, walker, referencedEnlargedRange);
		}
		void CorrectSharedFormula(CellPosition startPosition, CellPosition endPosition, RemovedShiftLeftSharedFormulaRPNVisitor walker) {
			int dColumn = endPosition.Column - startPosition.Column;
			int dRow = endPosition.Row - startPosition.Row;
			walker.SetHostCellProperties(dColumn, dRow, startPosition, Worksheet);
			WorkbookDataContext context = this.Worksheet.DataContext;
			context.PushCurrentWorksheet(Worksheet);
			context.PushCurrentCell(startPosition);
			try {
				ParsedExpression updatedExpression = walker.Process(Expression.Clone());
				if (walker.FormulaChanged)
					SetExpression(updatedExpression);
			}
			finally {
				context.PopCurrentCell();
				context.PopCurrentWorksheet();
			}
		}
		#endregion
		void ChangeRangeAndConvertCellReferences(CellRangeBase newFormulaRange) {
			if (newFormulaRange != null) {
				CellRange resultRange = newFormulaRange.GetCoveredRange();
				if (resultRange.CellCount == 1) {
					RemoveCellsFromAffectedRange(resultRange);
					SetRange(null);
				}
				else
					SetRange(resultRange);
			}
			else
				SetRange(null);
		}
		#region Range
		protected internal void SetRange(CellRange newRange) {
			if (newRange == null && range == null)
				return;
			if (newRange != null && range != null && newRange.EqualsPosition(range))
				return;
			DocumentModel documentModel = Worksheet.Workbook;
			SharedFormulaRangeChangedHistoryItem historyItem = new SharedFormulaRangeChangedHistoryItem(documentModel, this, newRange);
			documentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetRangeCore(CellRange newRange) {
			this.range = newRange;
		}
		#endregion
		public void UnregisterGuestCell(ICell cell) {
			SetRange(CalculateActualRangeExceptCell(cell));
		}
		CellRange CalculateActualRangeExceptCell(ICell cell) {
			int leftColumn = this.Worksheet.MaxColumnCount;
			int topRow = this.Worksheet.MaxRowCount;
			int rightColumn = -1;
			int bottomRow = -1;
			foreach (ICell guestCell in GetGuestCellsEnumerable()) {
				if (object.ReferenceEquals(cell, guestCell))
					continue;
				leftColumn = Math.Min(leftColumn, guestCell.ColumnIndex);
				topRow = Math.Min(topRow, guestCell.RowIndex);
				rightColumn = Math.Max(rightColumn, guestCell.ColumnIndex);
				bottomRow = Math.Max(bottomRow, guestCell.RowIndex);
			}
			CellPosition topLeft = new CellPosition(leftColumn, topRow);
			CellPosition bottomRight = new CellPosition(rightColumn, bottomRow);
			if (!topLeft.IsValid || !bottomRight.IsValid)
				return null;
			return new CellRange(this.Worksheet, topLeft, bottomRight);
		}
		public IEnumerable<ICellBase> GetGuestCellsEnumerable() {
			return new Enumerable<ICellBase>(new SharedFormulaGuestCellsEnumerator(this));
		}
		#region CheckIntegrity
		public void CheckIntegrity() {
			if (Range == null)
				IntegrityChecks.Fail("SharedFormula: range is null");
		}
		#endregion
		#region GetInvolvedCellRanges
		public List<CellRange> GetReferencedRanges() {
			List<CellRange> result = new List<CellRange>();
			CellPosition topLeftCell = range.TopLeft;
			WorkbookDataContext context = Worksheet.Workbook.DataContext;
			context.PushCurrentWorksheet(Worksheet);
			context.PushCurrentCell(topLeftCell);
			context.PushSharedFormulaProcessing(true);
			try {
				GetFormulaRangesRPNVisitor visitor = new GetFormulaRangesRPNVisitor(context, null);
				CellRangeBase resultRange = visitor.Perform(Expression, Worksheet);
				if (resultRange != null)
					resultRange.AddRanges(result);
				return result;
			}
			finally {
				context.PopSharedFormulaProcessing();
				context.PopCurrentCell();
				context.PopCurrentWorksheet();
			}
		}
		public List<PrecedentPair> GetInvolvedCellRanges() {
			List<PrecedentPair> result = new List<PrecedentPair>();
			if (range == null)
				return result;
			CellPosition topLeftCell = range.TopLeft;
			WorkbookDataContext context = Worksheet.DataContext;
			context.PushCurrentWorksheet(Worksheet);
			context.PushCurrentCell(topLeftCell);
			context.PushSharedFormulaProcessing(true);
			try {
				List<CellRangeBase> involvedRanges = Expression.GetInvolvedCellRanges(context);
				PrepareChainPrecedents(involvedRanges, result, range.Width, range.Height);
				return result;
			}
			finally {
				context.PopSharedFormulaProcessing();
				context.PopCurrentCell();
				context.PopCurrentWorksheet();
			}
		}
		void PrepareChainPrecedents(List<CellRangeBase> ranges, IList<PrecedentPair> result, int formulaWidth, int formulaHeight) {
			foreach (CellRangeBase innerRange in ranges) {
				if (innerRange.RangeType == CellRangeType.UnionRange) {
					PrepareChainPrecedents(((CellUnion)innerRange).InnerCellRanges, result, formulaWidth, formulaHeight);
				}
				else {
					PrecedentPair precedentPair = PrepareChainPrecedentPair((CellRange)innerRange, formulaWidth, formulaHeight);
					result.Add(precedentPair);
				}
			}
		}
		PrecedentPair PrepareChainPrecedentPair(CellRange sourceRange, int formulaWidth, int formulaHeight) {
			CellRange enlargedRange = EnlargeReferencedRangeCore(sourceRange, formulaWidth, formulaHeight);
			if (enlargedRange == null)
				return new PrecedentPair(sourceRange, this);
			return new PrecedentPair(enlargedRange, new SharedFormulaEnlargedRangeChainPrecedent(this, sourceRange));
		}
		CellRange EnlargeReferencedRange(CellRange sourceRange, int formulaWidth, int formulaHeight) {
			CellRange result = EnlargeReferencedRangeCore(sourceRange, formulaWidth, formulaHeight);
			if (result == null)
				return sourceRange;
			return result;
		}
		CellRange EnlargeReferencedRangeCore(CellRange sourceRange, int formulaWidth, int formulaHeight) {
			CellPosition topLeft = sourceRange.TopLeft;
			CellPosition bottomRight = sourceRange.BottomRight;
			if (bottomRight.ColumnType == PositionType.Absolute &&
				topLeft.ColumnType == PositionType.Absolute &&
				bottomRight.RowType == PositionType.Absolute &&
				topLeft.RowType == PositionType.Absolute)
				return null;
			int column = bottomRight.Column;
			int row = bottomRight.Row;
			if (bottomRight.ColumnType == PositionType.Relative)
				column += formulaWidth - 1;
			if (topLeft.ColumnType == PositionType.Relative)
				column = Math.Max(column, topLeft.Column + formulaWidth - 1);
			if (bottomRight.RowType == PositionType.Relative)
				row += formulaHeight - 1;
			if (topLeft.RowType == PositionType.Relative)
				row = Math.Max(row, topLeft.Row + formulaHeight - 1);
			bottomRight = new CellPosition(column, row, bottomRight.ColumnType, bottomRight.RowType);
			return new CellRange(sourceRange.Worksheet, topLeft, bottomRight);
		}
		#endregion
		#region IChainPrecedent Members
		public bool AllowsMerging { get { return false; } }
		public void MarkUpForRecalculation() {
			foreach (ICell cell in GetGuestCellsEnumerable())
				cell.MarkUpForRecalculation();
		}
		public void MergeWith(IChainPrecedent item) {
			throw new InvalidOperationException();
		}
		public void AddItemsTo(IList<ICell> where, ISheetPosition affectedRange) {
			foreach (ICell cell in GetGuestCellsEnumerable())
				where.Add(cell);
		}
		public CellRangeBase ToRange(ISheetPosition affectedRange) {
			CellRangeBase result = null;
			foreach (ICell cell in GetGuestCellsEnumerable()) {
				result = CellRangeBase.MergeRanges(result, cell.GetRange());
			}
			return result;
		}
		public bool Remove(IChainPrecedent cell) {
			return true;
		}
		#endregion
		public bool IsVolatile() {
			if (Expression == null)
				return false;
			return Expression.IsVolatile;
		}
		#region IFormula Members
		public FormulaDataState DataState { get { return FormulaDataState.ExpressionReady; } }
		public FormulaProperties GetProperties() {
			if (Expression == null)
				return FormulaProperties.None;
			return Expression.GetProperties();
		}
		#endregion
#if DEBUGTEST
		internal int GetGuestCellsCount() {
			IEnumerator<ICellBase> enumerator = GetGuestCellsEnumerable().GetEnumerator();
			int count = 0;
			while (enumerator.MoveNext()) {
				count++;
			}
			return count;
		}
		internal ICell GetTopLeftCell() {
			IEnumerator<ICellBase> enumerator = GetGuestCellsEnumerable().GetEnumerator();
			if (enumerator.MoveNext())
				return (ICell)enumerator.Current;
			return null;
		}
#endif
	}
	#endregion
	public class SharedFormulaGuestCellsEnumerator : IEnumerator<ICellBase> {
		readonly SharedFormula formula;
		IEnumerator<ICellBase> innerEnumerator;
		public SharedFormulaGuestCellsEnumerator(SharedFormula formula) {
			this.formula = formula;
			Guard.ArgumentNotNull(formula, "sharedFormula");
			Guard.ArgumentNotNull(formula.Range, "shared formula range");
		}
		public ICellBase Current { get { return innerEnumerator.Current; } }
		public void Dispose() {
			innerEnumerator.Dispose();
		}
		object System.Collections.IEnumerator.Current { get { return ((IEnumerator<ICellBase>)this).Current; } }
		public bool MoveNext() {
			if (innerEnumerator == null)
				innerEnumerator = formula.Range.GetExistingCellsEnumerator(false);
			if (!innerEnumerator.MoveNext())
				return false;
			while (!IsValidCell(innerEnumerator.Current)) {
				if (!innerEnumerator.MoveNext())
					return false;
			}
			return true;
		}
		bool IsValidCell(ICellBase cellBase) {
			ICell cell = cellBase as ICell;
			if (cell == null || cell.FormulaType != FormulaType.Shared)
				return false;
			SharedFormulaRef sharedFormulaRef = (SharedFormulaRef)cell.Formula;
			return object.ReferenceEquals(sharedFormulaRef.HostSharedFormula, formula);
		}
		public void Reset() {
			innerEnumerator = null;
		}
	}
	#region SharedFormulaCollection
	public class SharedFormulaCollection : IEnumerable<SharedFormula> { 
		#region Fields
		readonly Worksheet worksheet;
		readonly Dictionary<int, SharedFormula> innerCollection;
		int counter = 0;
		#endregion
		public SharedFormulaCollection(Worksheet worksheet) {
			Guard.ArgumentNotNull(worksheet, "worksheet");
			this.worksheet = worksheet;
			this.innerCollection = new Dictionary<int, SharedFormula>();
		}
		#region Properties
		public Dictionary<int, SharedFormula> InnerCollection { get { return innerCollection; } }
		public int Count { get { return innerCollection.Count; } }
		public SharedFormula this[int key] { get { return innerCollection[key]; } }
		List<int> markedForDeletionItems = new List<int>();
		#endregion
		public void OnRangeRemoving(RemoveRangeNotificationContext context) {
			foreach (KeyValuePair<int, SharedFormula> pair in innerCollection) {
				pair.Value.OnRangeRemoving(context);
				if (pair.Value.Range == null)
					markedForDeletionItems.Add(pair.Key);
			}
		}
		public void OnRangeInserting(InsertRangeNotificationContext notificationContext) {
			foreach (KeyValuePair<int, SharedFormula> pair in innerCollection)
				pair.Value.OnRangeInserting(notificationContext);
		}
		public int GetKeyBySharedFormula(SharedFormula formula) {
			foreach (KeyValuePair<int, SharedFormula> pair in innerCollection) {
				if (Object.ReferenceEquals(pair.Value, formula))
					return pair.Key;
			}
			return -1;
		}
		public void Remove(int sharedFormulaIndex) {
			DocumentHistory history = worksheet.Workbook.History;
			SharedFormulaRemoveHistoryItem historyItem = new SharedFormulaRemoveHistoryItem(worksheet, sharedFormulaIndex);
			history.Add(historyItem);
			historyItem.Execute();
		}
		public void RemoveCore(int sharedFormulaIndex) {
			SharedFormula formula = this[sharedFormulaIndex];
			if (formula.Range != null)
				worksheet.Workbook.CalculationChain.OnBeforeSharedFormulaRemoved(formula);
			this.innerCollection.Remove(sharedFormulaIndex);
		}
		internal void CheckIntegrity(CheckIntegrityFlags flags) {
			foreach (KeyValuePair<int, SharedFormula> sharedFormulaPair in innerCollection)
				sharedFormulaPair.Value.CheckIntegrity();
		}
		internal void Clear() {
			innerCollection.Clear();
			counter = 0;
		}
		public int Add(SharedFormula sharedFormula) {
			DocumentHistory history = worksheet.Workbook.History;
			int position = counter;
			SharedFormulaAddHistoryItem historyItem = new SharedFormulaAddHistoryItem(worksheet, sharedFormula, position);
			history.Add(historyItem);
			historyItem.Execute();
			return position;
		}
		public int AddWithoutHistory(SharedFormula sharedFormula) {
			int position = counter;
			AddCore(position, sharedFormula);
			return position;
		}
		public void AddCore(int index, SharedFormula sharedFormula) {
			innerCollection.Add(index, sharedFormula);
			counter++;
			if (sharedFormula.Range != null)
				worksheet.Workbook.CalculationChain.OnAfterSharedFormulaAdded(sharedFormula);
		}
		#region IEnumerable<SharedFormula> Members
		public IEnumerator<SharedFormula> GetEnumerator() {
			return innerCollection.Values.GetEnumerator();
		}
		#endregion
		#region IEnumerable Members
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return innerCollection.Values.GetEnumerator();
		}
		#endregion
		public bool Contains(SharedFormula formula) {
			return innerCollection.ContainsValue(formula);
		}
		public void MarkUpForDeletion(int sharedFormulaIndex) {
			markedForDeletionItems.Add(sharedFormulaIndex);
		}
		public void RemoveMarkedItems() {
			foreach (int index in markedForDeletionItems)
				this.Remove(index);
			markedForDeletionItems.Clear();
		}
	}
	#endregion
	#region SharedFormulaRef
	public class SharedFormulaRef : FormulaBase {
		const int formulaOffset = 5;
		#region Fields
		int hostFormulaIndex;
		SharedFormula hostSharedFormula;
		readonly ICell ownerCell;
		#endregion
		public SharedFormulaRef(ICell cell) {
			this.ownerCell = cell;
		}
		public SharedFormulaRef(ICell cell, int index, SharedFormula hostSharedFormula) {
			this.ownerCell = cell;
			Guard.ArgumentNotNull(hostSharedFormula, "hostSharedFormula");
			this.hostSharedFormula = hostSharedFormula;
			this.hostFormulaIndex = index;
		}
		#region Properties
		public SharedFormula HostSharedFormula { get { return hostSharedFormula; } }
		public int HostFormulaIndex { get { return hostFormulaIndex; } }
		public override ParsedExpression Expression {
			get { return hostSharedFormula.Expression; }
			protected set { throw new ArgumentException(); }
		}
		public ICell OwnerCell { get { return ownerCell; } }
		public override FormulaType Type { get { return FormulaType.Shared; } }
		#endregion
		public override FormulaDataState GetDataState() {
			return FormulaDataState.ExpressionReady;
		}
		public override ParsedExpression GetExpression(WorkbookDataContext context) {
			return Expression;
		}
		public override FormulaProperties GetProperties() {
			return HostSharedFormula.GetProperties();
		}
		public override VariantValue Calculate(ICell cell, WorkbookDataContext context) {
			return hostSharedFormula.Calculate(cell, context);
		}
		#region Binary <-> ParsedExpression conversion
		public override void InitializeFromBinary(byte[] binaryFormula, Worksheet sheet) {
			ReadAdditionalDataFromBinary(sheet, binaryFormula);
			hostFormulaIndex = BitConverter.ToInt32(binaryFormula, 1);
			hostSharedFormula = ownerCell.Worksheet.SharedFormulas[hostFormulaIndex];
		}
		public override byte[] GetBinary(WorkbookDataContext context) {
			byte[] result = new byte[5];
			result[0] = 3;
			BitConverter.GetBytes(hostFormulaIndex).CopyTo(result, 1);
			return result;
		}
		#endregion
		public void DetachFromSharedFormula() {
			hostSharedFormula.UnregisterGuestCell(ownerCell);
		}
		internal void ConvertToNormalFormula() {
			Formula newFormula = new Formula(OwnerCell, GetNormalCellFormula(OwnerCell));
			OwnerCell.TransactedSetFormula(newFormula);
		}
		public override string GetBody(ICell hostCell) {
			return HostSharedFormula.GetBody(hostCell);
		}
		#region CheckIntegrity
		public override void CheckIntegrity() {
		}
		public override void CheckIntegrity(ICell cell) {
			WorkbookDataContext context = cell.Context;
			context.PushCurrentCell(cell);
			try {
				if (cell != ownerCell)
					IntegrityChecks.Fail("SharedFormulaRef: inconsistend cell: cell != HostCell");
				if (!this.IsEqual(cell.GetFormula() as SharedFormulaRef))
					IntegrityChecks.Fail("SharedFormulaRef: cell.Content.Formula != this");
				SharedFormula parentFormula = hostSharedFormula;
				if (parentFormula == null)
					IntegrityChecks.Fail(String.Format("SharedFormulaRef: shared formula not found: GetFormula(SharedFormulaIndex) == null. ICell={0}", cell.ToString()));
				if (!Object.ReferenceEquals(parentFormula.Worksheet, cell.Sheet))
					IntegrityChecks.Fail(String.Format("SharedFormulaRef: inconsistent shared formula. parentFormula.Worksheet: [{0}], cell.Sheet: [{1}]", parentFormula.Worksheet.ToString(), cell.Sheet.ToString()));
				CellRange parentFormulaRange = parentFormula.Range;
				if (!parentFormulaRange.ContainsCell(cell))
					IntegrityChecks.Fail(String.Format("SharedFormulaRef: parentFormulaRange.ContainsCell(cell) == false: parentFormulaRange='{0}', cell={1}", parentFormulaRange.ToString(), cell.ToString()));
				if (!parentFormula.Range.ContainsCell(cell))
					IntegrityChecks.Fail(String.Format("SharedFormulaRef: cell is not registered in shared formula cell={0}", cell.ToString()));
			}
			finally {
				context.PopCurrentCell();
			}
		}
		#endregion
		public override List<CellRangeBase> GetPrecedents(ICell cell) {
			WorkbookDataContext context = cell.Worksheet.DataContext;
			context.PushCurrentCell(cell);
			context.PushSharedFormulaProcessing(true);
			try {
				return Expression.GetInvolvedCellRanges(context);
			}
			finally {
				context.PopSharedFormulaProcessing();
				context.PopCurrentCell();
			}
		}
		public override List<PrecedentPair> GetInvolvedCellRanges(ICell cell) {
			return new List<PrecedentPair>();
		}
		public ParsedExpression GetNormalCellFormula(ICell cell) {
			WorkbookDataContext context = cell.Context;
			context.PushCurrentCell(cell);
			try {
				SharedFormulaToNormalRPNVisitor visitor = new SharedFormulaToNormalRPNVisitor(context);
				return visitor.Process(Expression.Clone());
			}
			finally {
				context.PopCurrentCell();
			}
		}
		public override void PushSettingsToContext(WorkbookDataContext context, ICell cell) {
			context.PushSharedFormulaProcessing(true);
		}
		public override void PopSettingsFromContext(WorkbookDataContext context) {
			context.PopSharedFormulaProcessing();
		}
		public bool IsEqual(SharedFormulaRef other) {
			return this.hostFormulaIndex == other.hostFormulaIndex && ownerCell == other.ownerCell;
		}
	}
	#endregion
	#region RangeAffectedByReferenceCalculator
	public class RangeAffectedByReferenceCalculator {
		CellRange formulaRange;
		public RangeAffectedByReferenceCalculator(CellRange formulaRange) {
			this.formulaRange = formulaRange;
		}
		#region SharedFormulaCellOffsetRange (inner struct)
		struct SharedFormulaCellOffsetRange {
			int start, end;
			int startInclusion, endInclusion;
			public SharedFormulaCellOffsetRange(int start, int end, int startInclusion, int endInclusion) {
				this.start = start;
				this.end = end;
				this.startInclusion = startInclusion;
				this.endInclusion = endInclusion;
			}
			public int Start { get { return start; } }
			public int End { get { return end; } }
			public int StartInclusion { get { return startInclusion; } }
			public int EndInclusion { get { return endInclusion; } }
		}
		#endregion
		public CellRange GetResult(CellRange referencedRange, ISheetPosition affectedRange) {
			Tuple<SharedFormulaCellOffsetRange, SharedFormulaCellOffsetRange> intervals = GetFormulaAffectedIntervals(referencedRange, affectedRange);
			SharedFormulaCellOffsetRange rowIndices = intervals.Item1;
			SharedFormulaCellOffsetRange columnIndices = intervals.Item2;
			int startRow = formulaRange.TopLeft.Row;
			int startColumn = formulaRange.TopLeft.Column;
			int bottomRightRow = Math.Min(IndicesChecker.MaxRowCount, Math.Min(rowIndices.End, formulaRange.Height - 1) + startRow);
			int bottomRightColumn = Math.Min(IndicesChecker.MaxColumnCount, Math.Min(columnIndices.End, formulaRange.Width - 1) + startColumn);
			CellPosition topLeftAffected = new CellPosition(columnIndices.Start + startColumn, rowIndices.Start + startRow);
			CellPosition bottomRightAffected = new CellPosition(bottomRightColumn, bottomRightRow);
			return new CellRange(formulaRange.Worksheet, topLeftAffected, bottomRightAffected);
		}
		public CellRange GetResult(CellRange referencedRange, ISheetPosition affectedRange, RemovedShiftLeftRPNVisitor walker, CellRange referencedEnlargedRange) {
			Tuple<SharedFormulaCellOffsetRange, SharedFormulaCellOffsetRange> intervals = GetFormulaAffectedIntervals(referencedRange, affectedRange);
			SharedFormulaCellOffsetRange rowIndices = intervals.Item1;
			SharedFormulaCellOffsetRange columnIndices = intervals.Item2;
			int rowIndicesStart = rowIndices.StartInclusion;
			int rowIndicesEnd = rowIndices.EndInclusion;
			int columnIndicesStart = columnIndices.Start;
			int columnIndicesEnd = columnIndices.End;
			CellRange range1 = CalculateFormulaChangedRange(walker, referencedEnlargedRange, rowIndicesStart, rowIndicesEnd, columnIndicesStart, columnIndicesEnd);
			rowIndicesStart = rowIndices.Start;
			rowIndicesEnd = rowIndices.End;
			columnIndicesStart = columnIndices.StartInclusion;
			columnIndicesEnd = columnIndices.EndInclusion;
			CellRange range2 = CalculateFormulaChangedRange(walker, referencedEnlargedRange, rowIndicesStart, rowIndicesEnd, columnIndicesStart, columnIndicesEnd);
			if (range1 != null) {
				if (range2 != null)
					return range1.UnionWith(range2).CellRangeValue.GetFirstInnerCellRange();
				return range1;
			}
			return range2;
		}
		CellRange CalculateFormulaChangedRange(RemovedShiftLeftRPNVisitor walker, CellRange referencedEnlargedRange, int rowIndicesStart, int rowIndicesEnd, int columnIndicesStart, int columnIndicesEnd) {
			CellRange result = null;
			bool startIndexIswithinRange = columnIndicesStart < formulaRange.Width && rowIndicesStart < formulaRange.Height;
			bool intervalIsValid = rowIndicesEnd >= rowIndicesStart && columnIndicesEnd >= columnIndicesStart;
			if (startIndexIswithinRange && intervalIsValid) {
				int startRow = formulaRange.TopLeft.Row;
				int startColumn = formulaRange.TopLeft.Column;
				int bottomRightRow = Math.Min(IndicesChecker.MaxRowCount, Math.Min(rowIndicesEnd, referencedEnlargedRange.Height - 1) + referencedEnlargedRange.TopLeft.Row);
				int bottomRightColumn = Math.Min(IndicesChecker.MaxColumnCount, Math.Min(columnIndicesEnd, referencedEnlargedRange.Width - 1) + referencedEnlargedRange.TopLeft.Column);
				CellPosition topLeftAffected = new CellPosition(columnIndicesStart + referencedEnlargedRange.TopLeft.Column, rowIndicesStart + referencedEnlargedRange.TopLeft.Row);
				CellPosition bottomRightAffected = new CellPosition(bottomRightColumn, bottomRightRow);
				CellRange referencedAffectedRange = new CellRange(formulaRange.Worksheet, topLeftAffected, bottomRightAffected);
				if (walker.IsResizingRange(referencedAffectedRange) || walker.IsMovingRange(referencedAffectedRange)) {
					bottomRightRow = Math.Min(IndicesChecker.MaxRowCount, Math.Min(rowIndicesEnd, formulaRange.Height - 1) + startRow);
					bottomRightColumn = Math.Min(IndicesChecker.MaxColumnCount, Math.Min(columnIndicesEnd, formulaRange.Width - 1) + startColumn);
					topLeftAffected = new CellPosition(columnIndicesStart + startColumn, rowIndicesStart + startRow);
					bottomRightAffected = new CellPosition(bottomRightColumn, bottomRightRow);
					result = new CellRange(formulaRange.Worksheet, topLeftAffected, bottomRightAffected);
				}
			}
			return result;
		}
		Tuple<SharedFormulaCellOffsetRange, SharedFormulaCellOffsetRange> GetFormulaAffectedIntervals(CellRange referencedRange, ISheetPosition affectedRange) {
			CellPosition sourceRangeTopLeft = referencedRange.TopLeft;
			CellPosition sourceRangeBottomRight = referencedRange.BottomRight;
			int affectedCellTopRowIndex = affectedRange.TopRowIndex;
			int dTopLeft = affectedCellTopRowIndex - sourceRangeTopLeft.Row;
			int dBottomRight = affectedCellTopRowIndex - sourceRangeBottomRight.Row;
			int size = affectedRange.BottomRowIndex - affectedRange.TopRowIndex;
			bool canMoveTop = sourceRangeTopLeft.RowType == PositionType.Relative;
			bool canMoveBottom = sourceRangeBottomRight.RowType == PositionType.Relative;
			SharedFormulaCellOffsetRange rowIndices = GetCellAffectedInterval(dTopLeft, dBottomRight, size, canMoveTop, canMoveBottom);
			int affectedCellColumnIndex = affectedRange.LeftColumnIndex;
			dTopLeft = affectedCellColumnIndex - sourceRangeTopLeft.Column;
			dBottomRight = affectedCellColumnIndex - sourceRangeBottomRight.Column;
			size = affectedRange.RightColumnIndex - affectedRange.LeftColumnIndex;
			canMoveTop = sourceRangeTopLeft.ColumnType == PositionType.Relative;
			canMoveBottom = sourceRangeBottomRight.ColumnType == PositionType.Relative;
			SharedFormulaCellOffsetRange columnIndices = GetCellAffectedInterval(dTopLeft, dBottomRight, size, canMoveTop, canMoveBottom);
			return new Tuple<SharedFormulaCellOffsetRange, SharedFormulaCellOffsetRange>(rowIndices, columnIndices);
		}
		SharedFormulaCellOffsetRange GetCellAffectedInterval(int dTopLeft, int dBottomRight, int size, bool canMoveTop, bool canMoveBottom) {
			if (!canMoveTop && !canMoveBottom) {
				if (dTopLeft <= 0 && -dBottomRight <= size)
					return new SharedFormulaCellOffsetRange(0, Int32.MaxValue, 0, Int32.MaxValue);
				return new SharedFormulaCellOffsetRange(0, Int32.MaxValue, Int32.MaxValue, Int32.MinValue);
			}
			if (dTopLeft < 0 && dBottomRight < 0) {
				int distance = Math.Abs(Math.Max(dTopLeft, dBottomRight));
				if (distance > size)
					DevExpress.Office.Utils.Exceptions.ThrowInternalException();
				dTopLeft += distance;
				dBottomRight += distance;
				size -= distance;
			}
			int startIndex = 0;
			int endIndex = Int32.MaxValue;
			int startIndexInclusion = Int32.MaxValue;
			int endIndexInclusion = Int32.MinValue;
			if (dTopLeft * dBottomRight > 0) {
				startIndex = canMoveBottom ? dBottomRight : dTopLeft;
				if (canMoveTop && canMoveBottom) {
					endIndex = Math.Abs(dTopLeft - dBottomRight) + startIndex + size;
					startIndexInclusion = dTopLeft;
					endIndexInclusion = dBottomRight + size;
				}
			}
			else {
				if (canMoveTop && ((dBottomRight < 0 && dBottomRight < -size) || canMoveBottom))
					endIndex = dTopLeft + size;
				if (-dBottomRight <= size && (canMoveTop || dTopLeft == 0)) {
					startIndexInclusion = dTopLeft;
					endIndexInclusion = canMoveBottom ? size + dBottomRight : size + dTopLeft;
				}
			}
			if (endIndexInclusion < startIndexInclusion) {
				startIndexInclusion = Int32.MaxValue;
				endIndexInclusion = Int32.MinValue;
			}
			return new SharedFormulaCellOffsetRange(startIndex, endIndex, startIndexInclusion, endIndexInclusion);
		}
	}
	#endregion
}
