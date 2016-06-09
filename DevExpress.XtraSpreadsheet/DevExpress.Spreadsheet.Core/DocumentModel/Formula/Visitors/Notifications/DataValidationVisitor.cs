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
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region DataValidationNotificationInfo
	public class DataValidationNotificationInfo {
		#region Fields
		CellRangeBase range;
		ParsedExpression expression1;
		ParsedExpression expression2;
		#endregion
		public DataValidationNotificationInfo(DataValidation validation) {
			Guard.ArgumentNotNull(validation, "validation");
			Initialize(validation.CellRange, validation.Expression1, validation.Expression2);
		}
		public DataValidationNotificationInfo(CellRangeBase range, ParsedExpression expression1, ParsedExpression expression2) {
			Guard.ArgumentNotNull(range, "range");
			Initialize(range, expression1, expression2);
		}
		#region Properties
		public CellRangeBase Range { get { return range; } set { SetRange(value); } }
		public Worksheet Sheet { get { return range.GetFirstInnerCellRange().Worksheet as Worksheet; } }
		public WorkbookDataContext Context { get { return Sheet.Workbook.DataContext; } }
		public ParsedExpression Expression1 { get { return expression1; } set { expression1 = value; } }
		public ParsedExpression Expression2 { get { return expression2; } set { expression2 = value; } }
		public string Formula1 { get { return GetFormula(expression1); } }
		public string Formula2 { get { return GetFormula(expression2); } }
		#endregion
		void Initialize(CellRangeBase range, ParsedExpression expression1, ParsedExpression expression2) {
			SetRange(range.Clone());
			this.expression1 = expression1.Clone();
			this.expression2 = expression2.Clone();
		}
		string GetFormula(ParsedExpression expression) {
			Context.PushRelativeToCurrentCell(true);
			Context.PushCurrentCell(Range.TopLeft);
			try {
				return "=" + expression.BuildExpressionString(Context);
			}
			finally {
				Context.PopCurrentCell();
				Context.PopRelativeToCurrentCell();
			}
		}
		public bool Equals(DataValidationNotificationInfo other) {
			return expression1.Equals(other.expression1) && expression2.Equals(other.expression2);
		}
		public void UpdateExpressionsFromCurrentPosition(ReplaceThingsPRNVisitor walker) {
			UpdateExpressionsFromCurrentPosition(walker, Context);
		}
		public void UpdateExpressionsFromCurrentPosition(ReplaceThingsPRNVisitor walker, WorkbookDataContext targetContext) {
			targetContext.PushSharedFormulaProcessing(true);
			targetContext.PushRelativeToCurrentCell(true);
			targetContext.PushCurrentCell(Range.GetFirstInnerCellRange().TopLeft);
			try {
				UpdateExpressions(walker.Process);
			}
			finally {
				targetContext.PopCurrentCell();
				targetContext.PopRelativeToCurrentCell();
				targetContext.PopSharedFormulaProcessing();
			}
		}
		public void UpdateExpressions(Func<ParsedExpression, ParsedExpression> process) {
			if (expression1 != null && expression1.Count > 0)
				expression1 = process(expression1.Clone());
			if (expression2 != null && expression2.Count > 0)
				expression2 = process(expression2.Clone());
		}
		void SetRange(CellRangeBase range) {
			if (range == null)
				return;
			if (range.RangeType == CellRangeType.UnionRange) {
				CellRangeBase merged = range.MergeWithRange(range.GetFirstInnerCellRange().Clone());
				if (merged.RangeType != CellRangeType.UnionRange)
					range = merged;
			}
			this.range = range;
		}
		#region GetRelativeReferences
		public List<CellRange> GetRelativeReferenceRanges(CellPosition topLeft) {
			return GetRelativeReferenceRanges(topLeft, Sheet);
		}
		public List<CellRange> GetRelativeReferenceRanges(CellPosition topLeft, IWorksheet targetSheet) {
			return GetReferenceRanges(topLeft, targetSheet, ProcessReferenceType.Relative);
		}
		public List<CellRange> GetReferenceRanges(CellPosition topLeft, IWorksheet targetSheet) {
			return GetReferenceRanges(topLeft, targetSheet, ProcessReferenceType.Both);
		}
		List<CellRange> GetReferenceRanges(CellPosition topLeft, IWorksheet targetSheet, ProcessReferenceType referenceType) {
			List<CellRange> result = new List<CellRange>();
			Context.PushSharedFormulaProcessing(true);
			Context.PushCurrentWorksheet(targetSheet);
			Context.PushCurrentCell(topLeft);
			try {
				result.AddRange(GetReferenceRangesCore(Expression1, targetSheet, referenceType));
				result.AddRange(GetReferenceRangesCore(Expression2, targetSheet, referenceType));
				return result;
			}
			finally {
				Context.PopCurrentCell();
				Context.PopCurrentWorksheet();
				Context.PopSharedFormulaProcessing();
			}
		}
		List<CellRange> GetReferenceRangesCore(ParsedExpression expression, IWorksheet targetSheet, ProcessReferenceType referenceType) {
			List<CellRange> result = new List<CellRange>();
			CellRangeBase ranges = new GetFormulaRangesRPNVisitor(Context, null).Perform(expression, targetSheet);
			if (ranges == null)
				return result;
			foreach (CellRange range in ranges.GetAreasEnumerable())
				if (ShouldGetRange(range, referenceType))
					result.Add(range);
			return result;
		}
		bool ShouldGetRange(CellRange range, ProcessReferenceType referenceType) {
			if (referenceType == ProcessReferenceType.Absolute)
				return range.IsAbsolute();
			if (referenceType == ProcessReferenceType.Relative)
				return range.IsRelative();
			return true;
		}
		#endregion
	}
	#endregion
	#region DataValidationSplitterBase (abstract)
	public abstract class DataValidationSplitterBase {
		#region Fields
		DataValidationNotificationInfo info;
		List<DataValidationNotificationInfo> resultList;
		#endregion
		protected DataValidationSplitterBase(DataValidationNotificationInfo info) {
			Guard.ArgumentNotNull(info, "info");
			this.info = info;
			this.resultList = new List<DataValidationNotificationInfo>();
		}
		#region Properties
		protected Worksheet Sheet { get { return info.Sheet; } }
		protected DataValidationNotificationInfo Info { get { return info; } }
		protected CellRangeBase Range { get { return info.Range; } set { info.Range = value; } }
		protected ParsedExpression Expression1 { get { return info.Expression1; } set { info.Expression1 = value; } }
		protected ParsedExpression Expression2 { get { return info.Expression2; } set { info.Expression2 = value; } }
		protected List<DataValidationNotificationInfo> ResultList { get { return resultList; } }
		protected abstract CellRange ActionRange { get; }
		protected abstract ReplaceThingsPRNVisitor Walker { get; }
		#endregion
		public abstract List<DataValidationNotificationInfo> Process();
		protected void AddToResultList(CellRangeBase range) {
			resultList.Add(new DataValidationNotificationInfo(range, Expression1, Expression2));
		}
		#region Process Methods
		protected void ProcessAllCells(CellRangeBase range) {
			ProcessAllCells(range, false);
		}
		protected void ProcessAllCells(CellRangeBase range, bool shifted) {
			foreach (CellKey key in range.GetAllCellKeysEnumerable())
				ProcessCell(key, shifted);
		}
		protected void ProcessCell(CellKey key) {
			ProcessCell(key, false);
		}
		protected void ProcessCell(CellKey key, bool shifted) {
			CellPosition position = key.GetPosition();
			ProcessCellRange(new CellRange(Sheet, position, position), shifted);
		}
		protected void ProcessCellRange(CellRangeBase range) {
			ProcessCellRange(range, false);
		}
		protected void ProcessCellRange(CellRangeBase range, bool shifted) {
			if (range == null)
				return;
			DataValidationNotificationInfo newInfo = new DataValidationNotificationInfo(range, Expression1, Expression2);
			if (shifted)
				UpdateExpressionsShifted(newInfo);
			newInfo.UpdateExpressionsFromCurrentPosition(Walker);
			AddNewInfo(newInfo);
		}
		protected virtual void UpdateExpressionsShifted(DataValidationNotificationInfo newInfo) {
		}
		protected void AddNewInfo(DataValidationNotificationInfo newInfo) {
			foreach (DataValidationNotificationInfo info in resultList) {
				if (newInfo.Equals(info)) {
					info.Range = info.Range.MergeWithRange(newInfo.Range);
					return;
				}
			}
			resultList.Add(newInfo);
		}
		#endregion
		#region EnlargeRange
		protected CellRange EnlargeReferencedRange(CellRange range, int width, int height) {
			CellRange result = EnlargeReferencedRangeCore(range, width, height);
			if (result == null)
				return range;
			return result;
		}
		CellRange EnlargeReferencedRangeCore(CellRange range, int width, int height) {
			CellPosition topLeft = range.TopLeft;
			CellPosition bottomRight = range.BottomRight;
			if (bottomRight.ColumnType == PositionType.Absolute &&
				topLeft.ColumnType == PositionType.Absolute &&
				bottomRight.RowType == PositionType.Absolute &&
				topLeft.RowType == PositionType.Absolute)
				return null;
			int column = bottomRight.Column;
			int row = bottomRight.Row;
			if (bottomRight.ColumnType == PositionType.Relative)
				column += width - 1;
			if (topLeft.ColumnType == PositionType.Relative)
				column = Math.Max(column, topLeft.Column + width - 1);
			if (bottomRight.RowType == PositionType.Relative)
				row += height - 1;
			if (topLeft.RowType == PositionType.Relative)
				row = Math.Max(row, topLeft.Row + height - 1);
			bottomRight = new CellPosition(column, row, bottomRight.ColumnType, bottomRight.RowType);
			return new CellRange(range.Worksheet, topLeft, bottomRight);
		}
		#endregion
	}
	#endregion
	#region DataValidationNotificationsProcessorBase (abstract)
	public abstract class DataValidationNotificationsProcessorBase : DataValidationSplitterBase {
		#region Static Members
		public static DataValidationNotificationsProcessorBase GetProcessor(DataValidationNotificationInfo info, InsertRangeNotificationContext notificationContext) {
			if (notificationContext.Mode == InsertCellMode.ShiftCellsRight)
				return new DataValidationNotificationsShiftRightProcessor(info, notificationContext);
			return new DataValidationNotificationsShiftDownProcessor(info, notificationContext);
		}
		public static DataValidationNotificationsProcessorBase GetProcessor(DataValidationNotificationInfo info, RemoveRangeNotificationContext notificationContext) {
			RemoveCellMode mode = notificationContext.Mode;
			if (mode == RemoveCellMode.ShiftCellsLeft)
				return new DataValidationNotificationsShiftLeftProcessor(info, notificationContext);
			else if (mode == RemoveCellMode.ShiftCellsUp)
				return new DataValidationNotificationsShiftUpProcessor(info, notificationContext);
			return new DataValidationNotificationsNoShiftProcessor(info, notificationContext);
		}
		#endregion
		#region Fields
		InsertRemoveRangeNotificationContextBase notificationContext;
		RemovedShiftLeftDataValidationRPNVisitor walker;
		ConditionalFormattingRPNVisitor offsetWalker;
		#endregion
		protected DataValidationNotificationsProcessorBase(DataValidationNotificationInfo info, InsertRemoveRangeNotificationContextBase notificationContext)
			: base(info) {
			this.notificationContext = notificationContext;
			this.walker = GetWalker(notificationContext);
		}
		#region Properties
		protected override CellRange ActionRange { get { return notificationContext.Range; } }
		protected WorkbookDataContext Context { get { return ActionRange.Worksheet.Workbook.DataContext; } }
		protected override ReplaceThingsPRNVisitor Walker { get { return walker; } }
		protected bool RemovingBehaviourChanged { get; set; }
		#endregion
		protected abstract RemovedShiftLeftDataValidationRPNVisitor GetWalker(InsertRemoveRangeNotificationContextBase notificationContext);
		protected abstract ConditionalFormattingRPNVisitor GetOffsetWalker();
		protected abstract CellRangeBase GetAddedRange(CellRange singleRange);
		protected abstract List<int> GetGrid(CellRange singleRange);
		protected abstract CellRangeBase GetResultRange(CellRange modifiedSingleRange, List<int> grid);
		protected abstract void ProcessRangeInfo(RangeNotificationInfo info, CellRange singleRange);
		protected virtual bool ShouldExitNoSplitCase(CellRange range, CellRange relativeRange, CellRange changingRange, bool changingRangeContainsRange, bool changingRangeContainsFormulaRange) {
			return false;
		}
		protected virtual bool RemovingDontAffectAnythingCore(CellRange range) {
			return false;
		}
		protected virtual void ProcessRowOrColumn(CellRange range, bool shifted) {
		}
		#region Process
		public override List<DataValidationNotificationInfo> Process() {
			ICellTable targetSheet = ActionRange.Worksheet;
			if (Object.ReferenceEquals(Sheet, targetSheet))
				ProcessCore();
			else {
				Info.UpdateExpressionsFromCurrentPosition(Walker, targetSheet.Workbook.DataContext);
				AddToResultList(Range);
			}
			return ResultList;
		}
		void ProcessCore() {
			UpdateAbsoluteReferences();
			if (!walker.HasRelativeReferences) {
				CellRangeBase newRange = ProcessRange(Range.Clone());
				if (newRange != null)
					AddToResultList(newRange);
			}
			else
				UpdateRelativeReferences();
		}
		void UpdateAbsoluteReferences() {
			walker.ReferenceType = ProcessReferenceType.Absolute;
			Info.UpdateExpressions(ProcessExpression);
		}
		void UpdateRelativeReferences() {
			RangeNotificationInfo rangeInfo = GetRangeNotificationInfo();
			if (rangeInfo.ShiftedRange == null && rangeInfo.NonShiftedRange == null)
				return;
			walker.ReferenceType = ProcessReferenceType.Relative;
			if (walker.Logic.RemoveBehavior)
				UpdateRelativeReferencesForRemove(rangeInfo);
			else
				UpdateRelativeReferencesForInsert(rangeInfo);
		}
		protected virtual ParsedExpression ProcessExpression(ParsedExpression expression) {
			return walker.Process(expression);
		}
		protected override void UpdateExpressionsShifted(DataValidationNotificationInfo newInfo) {
			newInfo.UpdateExpressions(offsetWalker.Process);
		}
		#region ProcessRange
		CellRangeBase ProcessRange(CellRangeBase modifiedRange) {
			if (modifiedRange.RangeType == CellRangeType.UnionRange) {
				CellUnion resultRange = new CellUnion(new CellRangeList());
				foreach (CellRange currentRange in (modifiedRange as CellUnion).InnerCellRanges) {
					CellRangeBase range = ProcessSingleRange(currentRange);
					if (range == null)
						continue;
					if (range.RangeType == CellRangeType.UnionRange)
						resultRange.InnerCellRanges.AddRange((range as CellUnion).InnerCellRanges);
					else resultRange.InnerCellRanges.Add(range);
				}
				return resultRange;
			}
			return ProcessSingleRange(modifiedRange as CellRange);
		}
		CellRangeBase ProcessSingleRange(CellRange modifiedSingleRange) {
			CellRangeBase addedRange = GetAddedRange(modifiedSingleRange);
			if (addedRange != null)
				return addedRange;
			if (!walker.IsResizingRange(modifiedSingleRange))
				return walker.ProcessCellRange(modifiedSingleRange) as CellRange;
			List<int> grid = GetGrid(modifiedSingleRange);
			if (grid.Count == 2)
				return walker.ProcessCellRange(modifiedSingleRange);
			return GetResultRange(modifiedSingleRange, grid);
		}
		#region Data Validation removing range behaviour
		protected bool FormulaAffectsRemoving(ParsedExpression expression) {
			List<CellRangeBase> ranges = expression.GetInvolvedCellRanges(Sheet.Workbook.DataContext);
			if (ranges == null)
				return false;
			foreach (CellRangeBase formulaRange in ranges)
				if (ActionRange.Intersects(formulaRange) || FormulaIsRightOrBelowActionRange(formulaRange))
					return true;
			return false;
		}
		protected virtual bool FormulaIsRightOrBelowActionRange(CellRangeBase formulaRange) {
			return false;
		}
		#endregion
		#endregion
		#endregion
		#region UpdateRelativeReferencesForInsert
		void UpdateRelativeReferencesForInsert(RangeNotificationInfo rangeInfo) {
			CellRangeBase shiftedRange = rangeInfo.ShiftedRange;
			CellRangeBase nonShiftedRange = rangeInfo.NonShiftedRange;
			if (NoSplitCase()) {
				ProcessNoSplitCase(rangeInfo);
				return;
			}
			if (shiftedRange != null) {
				InitializeOffsetWalker(rangeInfo.Offset);
				ProcessAllCells(shiftedRange, true);
				if (nonShiftedRange == null)
					return;
				Range = nonShiftedRange;
			}
			foreach (CellKey key in Range.GetAllCellKeysEnumerable())
				if (!ActionRange.ContainsCell(key))
					ProcessCell(key);
			foreach (DataValidationNotificationInfo dvInfo in ResultList)
				dvInfo.Range = GetAddedRange(dvInfo.Range);
		}
		bool NoSplitCase() {
			bool containsAllFormulas = true;
			bool containsFormula = false;
			CellRange changingRange = walker.Logic.GetChangingRange();
			foreach (CellRange range in Range.GetAreasEnumerable()) {
				bool changingRangeContainsRange = changingRange.ContainsRange(range);
				bool changingRangeIntersectsRange = range.Intersects(changingRange);
				List<CellRange> relativeRefRanges = Info.GetRelativeReferenceRanges(range.TopLeft);
				foreach (CellRange relativeRange in relativeRefRanges) {
					bool changingRangeContainsFormulaRange = changingRange.ContainsRange(relativeRange);
					if (changingRangeIntersectsRange && !changingRangeContainsRange)
						return false;
					if (changingRangeContainsFormulaRange)
						containsFormula = true;
					else
						containsAllFormulas = false;
					if (ShouldExitNoSplitCase(range, relativeRange, changingRange, changingRangeContainsRange, changingRangeContainsFormulaRange))
						return false;
				}
			}
			if (containsFormula && !containsAllFormulas)
				return false;
			return true;
		}
		void ProcessNoSplitCase(RangeNotificationInfo rangeInfo) {
			CellRangeBase shiftedRange = rangeInfo.ShiftedRange;
			CellRangeBase nonShiftedRange = rangeInfo.NonShiftedRange;
			if (nonShiftedRange == null) {
				InitializeOffsetWalker(rangeInfo.Offset);
				ProcessCellRange(shiftedRange, true);
			}
			else if (shiftedRange == null)
				ProcessCellRange(nonShiftedRange);
			else
				AddToResultList(shiftedRange.MergeWithRange(nonShiftedRange));
		}
		CellRangeBase GetAddedRange(CellRangeBase range) {
			CellUnion result = new CellUnion(range.Worksheet, new List<CellRangeBase>());
			foreach (CellRange singleRange in range.GetAreasEnumerable()) {
				CellRangeBase addedRange = GetAddedRange(singleRange);
				if (addedRange != null)
					result.InnerCellRanges.AddRange(addedRange.GetAreasEnumerable());
			}
			result.InnerCellRanges.Add(range);
			return result;
		}
		#endregion
		#region UpdateRelativeReferencesForRemove
		void UpdateRelativeReferencesForRemove(RangeNotificationInfo rangeInfo) {
			if (RemovingDontAffectAnything()) {
				AddToResultList(Range);
				return;
			}
			CellRangeBase shiftedRange = rangeInfo.ShiftedRange;
			CellRangeBase nonShiftedRange = rangeInfo.NonShiftedRange;
			if (shiftedRange != null) {
				InitializeOffsetWalker(rangeInfo.Offset);
				UpdateRelativeReferencesForRemove(shiftedRange, true);
			}
			if (nonShiftedRange == null)
				return;
			UpdateRelativeReferencesForRemove(nonShiftedRange, false);
		}
		void InitializeOffsetWalker(int offset) {
			offsetWalker = GetOffsetWalker();
			offsetWalker.DiffBetweenNewAndOldRanges = offset;
		}
		void UpdateRelativeReferencesForRemove(CellRangeBase range, bool shifted) {
			foreach (CellRange singleRange in range.GetAreasEnumerable())
				UpdateRelativeReferencesForRemoveCore(singleRange, shifted);
		}
		protected virtual void UpdateRelativeReferencesForRemoveCore(CellRange range, bool shifted) {
			List<CellRange> relativeRanges = Info.GetRelativeReferenceRanges(range.TopLeft);
			if (relativeRanges.Count == 1) {
				if (relativeRanges[0].TopLeft.Column < range.TopLeft.Column)
					ProcessCellRange(range, shifted);
				else
					ProcessRowOrColumn(range, shifted);
			}
			else {
				ProcessAllCells(range, shifted);
			}
		}
		bool RemovingDontAffectAnything() {
			CellRange changingRange = walker.Logic.GetChangingRange();
			int minColumn = 0;
			int minRow = 0;
			int maxColumn = 0;
			int maxRow = 0;
			foreach (CellRange range in Range.GetAreasEnumerable()) {
				if (changingRange.Intersects(range) && !RemovingDontAffectAnythingCore(range))
					return false;
				List<CellRange> relativeRanges = Info.GetRelativeReferenceRanges(range.TopLeft);
				foreach (CellRange relativeRange in relativeRanges) {
					CellRange enlarged = EnlargeReferencedRange(relativeRange, range.Width, range.Height);
					minColumn = minColumn == 0 ? enlarged.TopLeft.Column : Math.Min(minColumn, enlarged.TopLeft.Column);
					minRow = minRow == 0 ? enlarged.TopLeft.Row : Math.Min(minRow, enlarged.TopLeft.Row);
					maxColumn = Math.Max(maxColumn, enlarged.BottomRight.Column);
					maxRow = Math.Max(maxRow, enlarged.BottomRight.Row);
				}
			}
			CellRange combinedRelativeRanges = new CellRange(Sheet, new CellPosition(minColumn, minRow), new CellPosition(maxColumn, maxRow));
			return !changingRange.Intersects(combinedRelativeRanges);
		}
		protected bool ActionRangeContainsRowOrColumn(CellRange range, CellRange actionRange) {
			CellRange intersection = range.Intersection(actionRange);
			if (intersection == null)
				return false;
			return intersection.Width == range.Width || intersection.Height == range.Height;
		}
		#endregion
		#region GetRangeNotificationInfo
		RangeNotificationInfo GetRangeNotificationInfo() {
			RangeNotificationInfo rangeInfo = new RangeNotificationInfo();
			if (ActionRange == null || Range == null)
				return rangeInfo;
			if (!object.ReferenceEquals(ActionRange.Worksheet, Sheet)) {
				rangeInfo.NonShiftedRange = Range;
				return rangeInfo;
			}
			foreach (CellRange range in Range.GetAreasEnumerable())
				ProcessRangeInfo(rangeInfo, range);
			return rangeInfo;
		}
		#endregion
	}
	#endregion
	#region DataValidationNotificationsShiftRightProcessor
	public class DataValidationNotificationsShiftRightProcessor : DataValidationNotificationsProcessorBase {
		public DataValidationNotificationsShiftRightProcessor(DataValidationNotificationInfo info, InsertRemoveRangeNotificationContextBase notificationContext)
			: base(info, notificationContext) {
		}
		protected new RemovedShiftLeftDataValidationRPNVisitor Walker { get { return (RemovedShiftLeftDataValidationRPNVisitor)base.Walker; } }
		protected override RemovedShiftLeftDataValidationRPNVisitor GetWalker(InsertRemoveRangeNotificationContextBase notificationContext) {
			return new InsertedShiftRightDataValidationRPNVisitor(notificationContext, Context);
		}
		protected override ConditionalFormattingRPNVisitor GetOffsetWalker() {
			return new ShiftRightConditionalFormattingRPNVisitor(Context);
		}
		protected override bool ShouldExitNoSplitCase(CellRange range, CellRange relativeRange, CellRange changingRange, bool changingRangeContainsRange, bool changingRangeContainsFormula) {
			if (range.Intersects(relativeRange)) {
				return range.TopLeft.Row > relativeRange.TopLeft.Row || range.BottomRight.Row < relativeRange.BottomRight.Row ||
					(changingRangeContainsRange && changingRangeContainsFormula && range.TopLeft.Column < relativeRange.TopLeft.Column) ||
					range.ContainsRange(relativeRange);
			}
			CellRange enlarged = EnlargeReferencedRange(relativeRange, range.Width, range.Height);
			return changingRangeContainsFormula && !changingRange.ContainsRange(enlarged);
		}
		protected override CellRangeBase GetResultRange(CellRange modifiedSingleRange, List<int> grid) {
			CellUnion resultRange = new CellUnion(new List<CellRangeBase>());
			PositionType columnType = modifiedSingleRange.TopLeft.ColumnType;
			PositionType rowType = modifiedSingleRange.TopLeft.RowType;
			int left = modifiedSingleRange.TopLeft.Column;
			int right = modifiedSingleRange.BottomRight.Column;
			for (int i = 0; i < grid.Count - 1; i++) {
				int top = grid[i];
				int bottom = grid[i + 1] - ((i + 1 != grid.Count - 1) ? 1 : 0);
				CellRange range = new CellRange(Sheet, new CellPosition(left, top, columnType, rowType), new CellPosition(right, bottom, columnType, rowType));
				if (top >= ActionRange.TopLeft.Row && bottom <= ActionRange.BottomRight.Row) {
					range = Walker.ProcessCellRange(range) as CellRange;
					if (range != null)
						resultRange.InnerCellRanges.Insert(0, range);
				}
				else resultRange.InnerCellRanges.Add(range);
			}
			return resultRange.InnerCellRanges.Count == 1 ? resultRange.InnerCellRanges[0] : resultRange;
		}
		protected override List<int> GetGrid(CellRange modifiedSingleRange) {
			List<int> grid = new List<int>();
			int minGrid = modifiedSingleRange.TopLeft.Row;
			int maxGrid = modifiedSingleRange.BottomRight.Row;
			grid.Add(minGrid);
			int topRow = ActionRange.TopLeft.Row;
			int bottomRow = ActionRange.BottomRight.Row + 1;
			if (topRow <= maxGrid && topRow >= minGrid && !grid.Contains(topRow))
				grid.Add(topRow);
			if (bottomRow <= maxGrid && bottomRow >= minGrid && !grid.Contains(bottomRow))
				grid.Add(bottomRow);
			grid.Add(maxGrid);
			return grid;
		}
		protected override CellRangeBase GetAddedRange(CellRange modifiedSingleRange) {
			if (ActionRange.TopLeft.Column == modifiedSingleRange.BottomRight.Column + 1)
				if (ActionRange.RangeType == CellRangeType.IntervalRange) {
					modifiedSingleRange.BottomRight = new CellPosition(ActionRange.BottomRight.Column, modifiedSingleRange.BottomRight.Row, modifiedSingleRange.BottomRight.ColumnType, modifiedSingleRange.BottomRight.RowType);
					return modifiedSingleRange;
				}
				else if (ActionRange.TopLeft.Row <= modifiedSingleRange.BottomRight.Row && ActionRange.BottomRight.Row >= modifiedSingleRange.TopLeft.Row) {
					int topRow = Math.Max(modifiedSingleRange.TopLeft.Row, ActionRange.TopLeft.Row);
					int bottomRow = Math.Min(modifiedSingleRange.BottomRight.Row, ActionRange.BottomRight.Row);
					CellRange range = new CellRange(Sheet,
						new CellPosition(ActionRange.TopLeft.Column, topRow, modifiedSingleRange.TopLeft.ColumnType, modifiedSingleRange.TopLeft.RowType),
						new CellPosition(ActionRange.BottomRight.Column, bottomRow, modifiedSingleRange.BottomRight.ColumnType, modifiedSingleRange.BottomRight.RowType)
						);
					CellUnion unionRange = new CellUnion(new List<CellRangeBase>());
					unionRange.InnerCellRanges.Add(modifiedSingleRange);
					unionRange.InnerCellRanges.Add(range);
					return unionRange;
				}
			return null;
		}
		#region ProcessRangeInfo
		protected override void ProcessRangeInfo(RangeNotificationInfo info, CellRange singleRange) {
			if (ActionRange.TopLeft.Row > singleRange.BottomRight.Row ||
				ActionRange.BottomRight.Row < singleRange.TopLeft.Row ||
				ActionRange.TopLeft.Column > singleRange.BottomRight.Column + 1) {
				info.AddNonShiftedRange(singleRange);
				return;
			}
			int firstModifiedRow = 0;
			if (singleRange.TopLeft.Row < ActionRange.TopLeft.Row) {
				firstModifiedRow = ActionRange.TopLeft.Row - singleRange.TopLeft.Row;
				CellRange rowsBeforeModifiedRows = singleRange.GetSubRowRange(0, firstModifiedRow - 1);
				info.AddNonShiftedRange(rowsBeforeModifiedRows);
			}
			int lastModifiedRow = singleRange.Height - 1;
			if (singleRange.BottomRight.Row > ActionRange.BottomRight.Row) {
				lastModifiedRow = ActionRange.BottomRight.Row - singleRange.TopLeft.Row;
				CellRange rowsAterModifiedRows = singleRange.GetSubRowRange(lastModifiedRow + 1, singleRange.Height - 1);
				info.AddNonShiftedRange(rowsAterModifiedRows);
			}
			CellRange modifiedRows = singleRange.GetSubRowRange(firstModifiedRow, lastModifiedRow);
			ProcessRangeInfoCore(info, modifiedRows);
		}
		void ProcessRangeInfoCore(RangeNotificationInfo info, CellRange modifiedRows) {
			int width = ActionRange.Width;
			if (modifiedRows.TopLeft.Column >= ActionRange.TopLeft.Column) {
				modifiedRows = modifiedRows.GetResized(width, 0, width, 0);
				info.AddShiftedRange(modifiedRows, width);
			}
			else {
				modifiedRows = modifiedRows.GetResized(0, 0, width, 0);
				int firstShiftedColumn = ActionRange.BottomRight.Column - modifiedRows.TopLeft.Column + 1;
				int lastNonShiftedColumn = firstShiftedColumn - 1;
				CellRange nonShiftedColumns = modifiedRows.GetSubColumnRange(0, lastNonShiftedColumn);
				info.AddNonShiftedRange(nonShiftedColumns);
				if (firstShiftedColumn <= modifiedRows.Width - 1) {
					CellRange shiftedColumns = modifiedRows.GetSubColumnRange(firstShiftedColumn, modifiedRows.Width - 1);
					info.AddShiftedRange(shiftedColumns, width);
				}
			}
		}
		#endregion
	}
	#endregion
	#region DataValidationNotificationsShiftDownProcessor
	public class DataValidationNotificationsShiftDownProcessor : DataValidationNotificationsProcessorBase {
		public DataValidationNotificationsShiftDownProcessor(DataValidationNotificationInfo info, InsertRemoveRangeNotificationContextBase notificationContext)
			: base(info, notificationContext) {
		}
		protected new RemovedShiftLeftDataValidationRPNVisitor Walker { get { return (RemovedShiftLeftDataValidationRPNVisitor)base.Walker; } }
		protected override RemovedShiftLeftDataValidationRPNVisitor GetWalker(InsertRemoveRangeNotificationContextBase notificationContext) {
			return new InsertedShiftDownDataValidationRPNVisitor(notificationContext, Context);
		}
		protected override ConditionalFormattingRPNVisitor GetOffsetWalker() {
			return new ShiftDownConditionalFormattingRPNVisitor(Context);
		}
		protected override bool ShouldExitNoSplitCase(CellRange range, CellRange relativeRange, CellRange changingRange, bool changingRangeContainsRange, bool changingRangeContainsFormula) {
			return range.Intersects(relativeRange) && relativeRange.TopLeft.Column >= range.TopLeft.Column;
		}
		protected override List<int> GetGrid(CellRange modifiedSingleRange) {
			List<int> grid = new List<int>();
			int minGrid = modifiedSingleRange.TopLeft.Column;
			int maxGrid = modifiedSingleRange.BottomRight.Column;
			grid.Add(minGrid);
			int leftColumn = ActionRange.TopLeft.Column;
			int rightColumn = ActionRange.BottomRight.Column + 1;
			if (leftColumn <= maxGrid && leftColumn >= minGrid && !grid.Contains(leftColumn))
				grid.Add(leftColumn);
			if (rightColumn <= maxGrid && rightColumn >= minGrid && !grid.Contains(rightColumn))
				grid.Add(rightColumn);
			grid.Add(maxGrid);
			return grid;
		}
		protected override CellRangeBase GetAddedRange(CellRange modifiedSingleRange) {
			if (ActionRange.TopLeft.Row == modifiedSingleRange.BottomRight.Row + 1)
				if (ActionRange.RangeType == CellRangeType.IntervalRange) {
					modifiedSingleRange.BottomRight = new CellPosition(modifiedSingleRange.BottomRight.Column, ActionRange.BottomRight.Row, modifiedSingleRange.BottomRight.ColumnType, modifiedSingleRange.BottomRight.RowType);
					return modifiedSingleRange;
				}
				else if (ActionRange.TopLeft.Column <= modifiedSingleRange.BottomRight.Column && ActionRange.BottomRight.Column >= modifiedSingleRange.TopLeft.Column) {
					int leftColumn = Math.Max(modifiedSingleRange.TopLeft.Column, ActionRange.TopLeft.Column);
					int rightColumn = Math.Min(modifiedSingleRange.BottomRight.Column, ActionRange.BottomRight.Column);
					CellRange range = new CellRange(Sheet,
						new CellPosition(leftColumn, ActionRange.TopLeft.Row, modifiedSingleRange.TopLeft.ColumnType, modifiedSingleRange.TopLeft.RowType),
						new CellPosition(rightColumn, ActionRange.BottomRight.Row, modifiedSingleRange.BottomRight.ColumnType, modifiedSingleRange.BottomRight.RowType)
						);
					CellUnion unionRange = new CellUnion(new List<CellRangeBase>());
					unionRange.InnerCellRanges.Add(modifiedSingleRange);
					unionRange.InnerCellRanges.Add(range);
					return unionRange;
				}
			return null;
		}
		protected override CellRangeBase GetResultRange(CellRange modifiedSingleRange, List<int> grid) {
			CellUnion resultRange = new CellUnion(new List<CellRangeBase>());
			PositionType columnType = modifiedSingleRange.TopLeft.ColumnType;
			PositionType rowType = modifiedSingleRange.TopLeft.RowType;
			int top = modifiedSingleRange.TopLeft.Row;
			int bottom = modifiedSingleRange.BottomRight.Row;
			for (int i = 0; i < grid.Count - 1; i++) {
				int left = grid[i];
				int right = grid[i + 1] - ((i + 1 != grid.Count - 1) ? 1 : 0);
				CellRange range = new CellRange(Sheet, new CellPosition(left, top, columnType, rowType), new CellPosition(right, bottom, columnType, rowType));
				if (left >= ActionRange.TopLeft.Column && right <= ActionRange.BottomRight.Column) {
					CellRangeBase processedRange = Walker.ProcessCellRange(range);
					range = processedRange as CellRange;
					if (range != null)
						resultRange.InnerCellRanges.Insert(0, range);
					else {
						CellUnion resultUnion = processedRange as CellUnion;
						if (resultUnion != null)
							resultRange.InnerCellRanges.InsertRange(0, resultUnion.InnerCellRanges);
					}
				}
				else resultRange.InnerCellRanges.Add(range);
			}
			return resultRange.InnerCellRanges.Count == 1 ? resultRange.InnerCellRanges[0] : resultRange; ;
		}
		#region ProcessRangeInfo
		protected override void ProcessRangeInfo(RangeNotificationInfo info, CellRange singleRange) {
			if (ActionRange.TopLeft.Column > singleRange.BottomRight.Column ||
				ActionRange.BottomRight.Column < singleRange.TopLeft.Column ||
				ActionRange.TopLeft.Row > singleRange.BottomRight.Row + 1) {
				info.AddNonShiftedRange(singleRange);
				return;
			}
			int firstModifiedColumn = 0;
			if (singleRange.TopLeft.Column < ActionRange.TopLeft.Column) {
				firstModifiedColumn = ActionRange.TopLeft.Column - singleRange.TopLeft.Column;
				CellRange columnsBeforeModifiedColumns = singleRange.GetSubColumnRange(0, firstModifiedColumn - 1);
				info.AddNonShiftedRange(columnsBeforeModifiedColumns);
			}
			int lastModifiedColumn = singleRange.Width - 1;
			if (singleRange.BottomRight.Column > ActionRange.BottomRight.Column) {
				lastModifiedColumn = ActionRange.BottomRight.Column - singleRange.TopLeft.Column;
				CellRange columnsAfterModyfiedColumns = singleRange.GetSubColumnRange(lastModifiedColumn + 1, singleRange.Width - 1);
				info.AddNonShiftedRange(columnsAfterModyfiedColumns);
			}
			CellRange modifiedColumns = singleRange.GetSubColumnRange(firstModifiedColumn, lastModifiedColumn);
			CellIntervalRange columnsInterval = modifiedColumns as CellIntervalRange;
			if (columnsInterval != null && columnsInterval.IsColumnInterval) {
				info.AddNonShiftedRange(singleRange);
				return;
			}
			ProcessRangeInfoCore(info, modifiedColumns);
		}
		void ProcessRangeInfoCore(RangeNotificationInfo info, CellRange modifiedColumns) {
			int height = ActionRange.Height;
			if (modifiedColumns.TopLeft.Row >= ActionRange.TopLeft.Row) {
				modifiedColumns = modifiedColumns.GetResizedLimited(0, height, 0, height);
				info.AddShiftedRange(modifiedColumns, height);
			}
			else {
				modifiedColumns = modifiedColumns.GetResizedLimited(0, 0, 0, height);
				int firstShiftedRow = ActionRange.BottomRight.Row - modifiedColumns.TopLeft.Row + 1;
				int lastNonShiftedRow = firstShiftedRow - 1;
				CellRange nonShiftedRows = modifiedColumns.GetSubRowRange(0, lastNonShiftedRow);
				info.AddNonShiftedRange(nonShiftedRows);
				if (firstShiftedRow <= modifiedColumns.Height - 1) {
					CellRange shiftedRows = modifiedColumns.GetSubRowRange(firstShiftedRow, modifiedColumns.Height - 1);
					info.AddShiftedRange(shiftedRows, height);
				}
			}
		}
		#endregion
	}
	#endregion
	#region DataValidationNotificationsShiftLeftProcessor
	public class DataValidationNotificationsShiftLeftProcessor : DataValidationNotificationsShiftRightProcessor {
		public DataValidationNotificationsShiftLeftProcessor(DataValidationNotificationInfo info, InsertRemoveRangeNotificationContextBase notificationContext)
			: base(info, notificationContext) {
		}
		protected override RemovedShiftLeftDataValidationRPNVisitor GetWalker(InsertRemoveRangeNotificationContextBase notificationContext) {
			return new RemovedShiftLeftDataValidationRPNVisitor(notificationContext, Context);
		}
		protected override ConditionalFormattingRPNVisitor GetOffsetWalker() {
			return new ShiftLeftConditionalFormattingRPNVisitor(Context);
		}
		protected override CellRangeBase GetAddedRange(CellRange modifiedSingleRange) {
			return null;
		}
		protected override ParsedExpression ProcessExpression(ParsedExpression expression) {
			ParsedExpression result = base.ProcessExpression(expression);
			if (Walker.FormulaChanged || FormulaAffectsRemoving(expression))
				RemovingBehaviourChanged = true;
			return result;
		}
		protected override CellRangeBase GetResultRange(CellRange modifiedSingleRange, List<int> grid) {
			if (!ShouldProcessRange(modifiedSingleRange))
				return modifiedSingleRange;
			return base.GetResultRange(modifiedSingleRange, grid);
		}
		bool ShouldProcessRange(CellRange modifiedSingleRange) {
			if (RemovingBehaviourChanged)
				return true;
			int lastColumn = modifiedSingleRange.BottomRight.Column;
			if (ActionRange.TopLeft.Column <= lastColumn && ActionRange.BottomRight.Column >= lastColumn) {
				bool afterFistRow = ActionRange.TopLeft.Row > modifiedSingleRange.TopLeft.Row && ActionRange.TopLeft.Row <= modifiedSingleRange.BottomRight.Row;
				bool beforeLastRow = ActionRange.BottomRight.Row < modifiedSingleRange.BottomRight.Row && ActionRange.BottomRight.Row >= modifiedSingleRange.TopLeft.Row;
				if (afterFistRow && beforeLastRow)
					return false;
				if (afterFistRow || beforeLastRow)
					return ActionRange.TopLeft.Column <= modifiedSingleRange.TopLeft.Column;
			}
			return true;
		}
		protected override bool FormulaIsRightOrBelowActionRange(CellRangeBase formulaRange) {
			CellPosition topLeft = formulaRange.TopLeft;
			CellPosition bottomRight = formulaRange.BottomRight;
			return topLeft.Column > ActionRange.BottomRight.Column &&
				(ActionRange.TopLeft.Row >= topLeft.Row || ActionRange.BottomRight.Row <= bottomRight.Row);
		}
		protected override bool RemovingDontAffectAnythingCore(CellRange range) {
			return ActionRange.TopLeft.Column > range.TopLeft.Column && ActionRange.BottomRight.Column >= range.BottomRight.Column &&
				  (ActionRange.TopLeft.Row > range.TopLeft.Row || ActionRange.BottomRight.Row < range.BottomRight.Row);
		}
		#region ProcessRowOrColumn
		protected override void ProcessRowOrColumn(CellRange range, bool shifted) {
			int rowsCount = range.BottomRight.Row - range.TopLeft.Row + 1;
			for (int i = 0; i < rowsCount; i++)
				ProcessRow(range.GetSubRowRange(i, i), shifted);
		}
		void ProcessRow(CellRange row, bool shifted) {
			int specialCaseFirstColumn = -1;
			int specialCaseLastColumn = -1;
			int firstColumn = row.TopLeft.Column;
			bool shouldAddNextCell = false;
			CellRange actionRange = shifted ? new CellRange(Sheet, new CellPosition(0, ActionRange.TopLeft.Row), ActionRange.BottomRight) : ActionRange;
			foreach (CellKey key in row.GetAllCellKeysEnumerable()) {
				CellRange relativeRange = Info.GetRelativeReferenceRanges(key.GetPosition())[0];
				if (ActionRangeContainsRowOrColumn(relativeRange, actionRange))
					shouldAddNextCell = true;
				if (ActionRange.Intersects(relativeRange) && !ActionRange.ContainsRange(relativeRange) && shouldAddNextCell) {
					if (specialCaseFirstColumn == -1)
						specialCaseFirstColumn = key.ColumnIndex;
					else
						specialCaseLastColumn = key.ColumnIndex;
				}
				else
					ProcessCell(key, shifted);
			}
			if (specialCaseFirstColumn >= 0) {
				if (specialCaseLastColumn < 0)
					specialCaseLastColumn = specialCaseFirstColumn;
				ProcessCellRange(row.GetSubColumnRange(specialCaseFirstColumn - firstColumn, specialCaseLastColumn - firstColumn), shifted);
			}
		}
		#endregion
		#region ProcessRangeInfo
		protected override void ProcessRangeInfo(RangeNotificationInfo info, CellRange singleRange) {
			if (ActionRange.Includes(singleRange))
				return;
			if (singleRange.BottomRight.Column < ActionRange.TopLeft.Column ||
				singleRange.BottomRight.Row < ActionRange.TopLeft.Row ||
				singleRange.TopLeft.Row > ActionRange.BottomRight.Row) {
				info.AddNonShiftedRange(singleRange);
				return;
			}
			int firstShiftedRow = 0;
			if (singleRange.TopLeft.Row < ActionRange.TopLeft.Row) {
				firstShiftedRow = ActionRange.TopLeft.Row - singleRange.TopLeft.Row;
				CellRange rowsBeforeModifiedRows = singleRange.GetSubRowRange(0, firstShiftedRow - 1);
				info.AddNonShiftedRange(rowsBeforeModifiedRows);
			}
			int lastShiftedRow = singleRange.Height - 1;
			if (singleRange.BottomRight.Row > ActionRange.BottomRight.Row) {
				lastShiftedRow = ActionRange.BottomRight.Row - singleRange.TopLeft.Row;
				CellRange rowsAfterModifiedRows = singleRange.GetSubRowRange(lastShiftedRow + 1, singleRange.Height - 1);
				info.AddNonShiftedRange(rowsAfterModifiedRows);
			}
			singleRange = singleRange.GetSubRowRange(firstShiftedRow, lastShiftedRow);
			ProcessRangeInfoCore(info, singleRange);
		}
		void ProcessRangeInfoCore(RangeNotificationInfo info, CellRange singleRange) {
			if (ActionRange.ContainsRange(singleRange))
				return;
			int leftShift = 0;
			int rightShift = 0;
			int firstColumnShifted = 0;
			if (singleRange.TopLeft.Column > ActionRange.TopLeft.Column)
				if (singleRange.TopLeft.Column > ActionRange.BottomRight.Column)
					leftShift = -ActionRange.Width;
				else
					leftShift = ActionRange.TopLeft.Column - singleRange.TopLeft.Column;
			else
				firstColumnShifted = ActionRange.TopLeft.Column - singleRange.TopLeft.Column;
			if (singleRange.BottomRight.Column >= ActionRange.BottomRight.Column) {
				rightShift = -ActionRange.Width;
				singleRange = singleRange.GetResized(leftShift, 0, rightShift, 0);
				if (firstColumnShifted > 0) {
					CellRange columnsBeforeShifted = singleRange.GetSubColumnRange(0, firstColumnShifted - 1);
					info.AddNonShiftedRange(columnsBeforeShifted);
				}
				if (firstColumnShifted <= singleRange.Width - 1) {
					CellRange shiftedColumns = singleRange.GetSubColumnRange(firstColumnShifted, singleRange.Width - 1);
					info.AddShiftedRange(shiftedColumns, ActionRange.Width);
				}
			}
			else {
				rightShift = ActionRange.TopLeft.Column - singleRange.BottomRight.Column - 1;
				singleRange = singleRange.GetResized(leftShift, 0, rightShift, 0);
				info.AddNonShiftedRange(singleRange);
			}
		}
		#endregion
	}
	#endregion
	#region DataValidationNotificationsShiftUpProcessor
	public class DataValidationNotificationsShiftUpProcessor : DataValidationNotificationsShiftDownProcessor {
		public DataValidationNotificationsShiftUpProcessor(DataValidationNotificationInfo info, InsertRemoveRangeNotificationContextBase notificationContext)
			: base(info, notificationContext) {
		}
		protected override RemovedShiftLeftDataValidationRPNVisitor GetWalker(InsertRemoveRangeNotificationContextBase notificationContext) {
			return new RemovedShiftUpDataValidationRPNVisitor(notificationContext, Context);
		}
		protected override ConditionalFormattingRPNVisitor GetOffsetWalker() {
			return new ShiftUpConditionalFormattingRPNVisitor(Context);
		}
		protected override CellRangeBase GetAddedRange(CellRange modifiedSingleRange) {
			return null;
		}
		protected override ParsedExpression ProcessExpression(ParsedExpression expression) {
			ParsedExpression result = base.ProcessExpression(expression);
			if (Walker.FormulaChanged || FormulaAffectsRemoving(expression))
				RemovingBehaviourChanged = true;
			return result;
		}
		protected override CellRangeBase GetResultRange(CellRange modifiedSingleRange, List<int> grid) {
			if (!ShouldProcessRange(modifiedSingleRange))
				return modifiedSingleRange;
			return base.GetResultRange(modifiedSingleRange, grid);
		}
		bool ShouldProcessRange(CellRange modifiedSingleRange) {
			if (RemovingBehaviourChanged)
				return true;
			int lastRow = modifiedSingleRange.BottomRight.Row;
			if (ActionRange.TopLeft.Row <= lastRow && ActionRange.BottomRight.Row >= lastRow) {
				bool afterFistColumn = ActionRange.TopLeft.Column > modifiedSingleRange.TopLeft.Column && ActionRange.TopLeft.Column <= modifiedSingleRange.BottomRight.Column;
				bool beforeLastColumn = ActionRange.BottomRight.Column < modifiedSingleRange.BottomRight.Column && ActionRange.BottomRight.Column >= modifiedSingleRange.TopLeft.Column;
				if (afterFistColumn && beforeLastColumn)
					return false;
				if (afterFistColumn || beforeLastColumn)
					return ActionRange.TopLeft.Row <= modifiedSingleRange.TopLeft.Row;
			}
			return true;
		}
		protected override bool FormulaIsRightOrBelowActionRange(CellRangeBase formulaRange) {
			CellPosition topLeft = formulaRange.TopLeft;
			CellPosition bottomRight = formulaRange.BottomRight;
			return topLeft.Row > ActionRange.BottomRight.Row &&
				(ActionRange.TopLeft.Column >= topLeft.Column || ActionRange.BottomRight.Column <= bottomRight.Column);
		}
		protected override bool RemovingDontAffectAnythingCore(CellRange range) {
			return ActionRange.TopLeft.Row > range.TopLeft.Row && ActionRange.BottomRight.Row >= range.BottomRight.Row &&
				  (ActionRange.TopLeft.Column > range.TopLeft.Column || ActionRange.BottomRight.Column < range.BottomRight.Column);
		}
		#region ProcessRowOrColumn
		protected override void ProcessRowOrColumn(CellRange range, bool shifted) {
			int columnsCount = range.BottomRight.Column - range.TopLeft.Column + 1;
			for (int i = 0; i < columnsCount; i++)
				ProcessColumn(range.GetSubColumnRange(i, i), shifted);
		}
		void ProcessColumn(CellRange column, bool shifted) {
			int specialCaseFirstRow = -1;
			int specialCaseLastRow = -1;
			int firstRow = column.TopLeft.Row;
			bool shouldAddNextCell = false;
			CellRange actionRange = shifted ? new CellRange(Sheet, new CellPosition(ActionRange.TopLeft.Column, 0), ActionRange.BottomRight) : ActionRange;
			foreach (CellKey key in column.GetAllCellKeysEnumerable()) {
				CellRange relativeRange = Info.GetRelativeReferenceRanges(key.GetPosition())[0];
				if (ActionRangeContainsRowOrColumn(relativeRange, actionRange))
					shouldAddNextCell = true;
				if (actionRange.Intersects(relativeRange) && !actionRange.ContainsRange(relativeRange) && shouldAddNextCell) {
					if (specialCaseFirstRow == -1)
						specialCaseFirstRow = key.RowIndex;
					else
						specialCaseLastRow = key.RowIndex;
				}
				else
					ProcessCell(key, shifted);
			}
			if (specialCaseFirstRow >= 0) {
				if (specialCaseLastRow < 0)
					specialCaseLastRow = specialCaseFirstRow;
				ProcessCellRange(column.GetSubRowRange(specialCaseFirstRow - firstRow, specialCaseLastRow - firstRow), shifted);
			}
		}
		#endregion
		#region ProcessRangeInfo
		protected override void ProcessRangeInfo(RangeNotificationInfo info, CellRange singleRange) {
			if (ActionRange.Includes(singleRange))
				return;
			if (singleRange.BottomRight.Row < ActionRange.TopLeft.Row ||
				singleRange.BottomRight.Column < ActionRange.TopLeft.Column ||
				singleRange.TopLeft.Column > ActionRange.BottomRight.Column) {
				info.AddNonShiftedRange(singleRange);
				return;
			}
			int firstShiftedColumn = 0;
			if (singleRange.TopLeft.Column < ActionRange.TopLeft.Column) {
				firstShiftedColumn = ActionRange.TopLeft.Column - singleRange.TopLeft.Column;
				CellRange rangeBeforeActialProcessableRange = singleRange.GetSubColumnRange(0, firstShiftedColumn - 1);
				info.AddNonShiftedRange(rangeBeforeActialProcessableRange);
			}
			int lastShiftedColumn = singleRange.Width - 1;
			if (singleRange.BottomRight.Column > ActionRange.BottomRight.Column) {
				lastShiftedColumn = ActionRange.BottomRight.Column - singleRange.TopLeft.Column;
				CellRange rangeAfterActualProcessableRange = singleRange.GetSubColumnRange(lastShiftedColumn + 1, singleRange.Width - 1);
				info.AddNonShiftedRange(rangeAfterActualProcessableRange);
			}
			singleRange = singleRange.GetSubColumnRange(firstShiftedColumn, lastShiftedColumn);
			ProcessRangeInfoCore(info, singleRange);
		}
		void ProcessRangeInfoCore(RangeNotificationInfo info, CellRange singleRange) {
			if (ActionRange.ContainsRange(singleRange))
				return;
			int topShift = 0;
			int bottomShift = 0;
			int firstRowShifted = 0;
			if (singleRange.TopLeft.Row > ActionRange.TopLeft.Row)
				if (singleRange.TopLeft.Row >= ActionRange.BottomRight.Row)
					topShift = -ActionRange.Height;
				else
					topShift = ActionRange.TopLeft.Row - singleRange.TopLeft.Row;
			else
				firstRowShifted = ActionRange.TopLeft.Row - singleRange.TopLeft.Row;
			if (singleRange.BottomRight.Row >= ActionRange.BottomRight.Row) {
				bottomShift = -ActionRange.Height;
				singleRange = singleRange.GetResized(0, topShift, 0, bottomShift);
				if (firstRowShifted > 0) {
					CellRange rowsBeforeShifted = singleRange.GetSubRowRange(0, firstRowShifted - 1);
					info.AddNonShiftedRange(rowsBeforeShifted);
				}
				if (firstRowShifted <= singleRange.Height - 1) {
					CellRange shiftedRows = singleRange.GetSubRowRange(firstRowShifted, singleRange.Height - 1);
					info.AddShiftedRange(shiftedRows, ActionRange.Height);
				}
			}
			else {
				bottomShift = ActionRange.TopLeft.Row - singleRange.BottomRight.Row - 1;
				singleRange = singleRange.GetResized(0, topShift, 0, bottomShift);
				info.AddNonShiftedRange(singleRange);
			}
		}
		#endregion
	}
	#endregion
	#region DataValidationNotificationsNoShiftProcessor
	public class DataValidationNotificationsNoShiftProcessor : DataValidationNotificationsShiftDownProcessor {
		public DataValidationNotificationsNoShiftProcessor(DataValidationNotificationInfo info, RemoveRangeNotificationContext notificationContext)
			: base(info, notificationContext) {
		}
		protected override RemovedShiftLeftDataValidationRPNVisitor GetWalker(InsertRemoveRangeNotificationContextBase notificationContext) {
			return new RemovedNoShiftDataValidationRPNVisitor(notificationContext, Context);
		}
		protected override ConditionalFormattingRPNVisitor GetOffsetWalker() {
			return null;
		}
		protected override CellRangeBase GetAddedRange(CellRange modifiedSingleRange) {
			return null;
		}
		protected override void ProcessRangeInfo(RangeNotificationInfo info, CellRange singleRange) {
			CellRangeBase result = singleRange.ExcludeRange(ActionRange);
			if (result != null)
				info.AddNonShiftedRange(result);
		}
		protected override void UpdateRelativeReferencesForRemoveCore(CellRange range, bool shifted) {
			List<CellRange> referencedRanges = Info.GetRelativeReferenceRanges(range.TopLeft);
			bool singleRelativeRefRange = referencedRanges.Count == 1;
			if (singleRelativeRefRange && referencedRanges[0].TopLeft.Column < range.TopLeft.Column) {
				ProcessCellRange(range);
				return;
			}
			CellRangeBase allAffectedRange = range.Intersection(ActionRange);
			if (allAffectedRange == null || !allAffectedRange.IsEqualSurfacesWith(range)) {
				foreach (CellRange referencedRange in referencedRanges) {
					CellRange enlargedReferencedRange = EnlargeReferencedRange(referencedRange, range.Width, range.Height);
					if (enlargedReferencedRange.Intersects(ActionRange)) {
						RangeAffectedByReferenceCalculator calculator = new RangeAffectedByReferenceCalculator(range);
						CellRange formulaAffectedRangeByReferencedRange = calculator.GetResult(referencedRange, ActionRange, Walker, enlargedReferencedRange);
						if (formulaAffectedRangeByReferencedRange != null)
							allAffectedRange = allAffectedRange == null ? formulaAffectedRangeByReferencedRange : allAffectedRange.MergeWithRange(formulaAffectedRangeByReferencedRange);
					}
				}
			}
			if (allAffectedRange != null) {
				CellRangeBase unnaffected = range.ExcludeRange(allAffectedRange);
				if (unnaffected != null)
					AddToResultList(unnaffected);
				if (singleRelativeRefRange)
					ProcessAffectedRange(allAffectedRange, singleRelativeRefRange);
				else
					ProcessAllCells(allAffectedRange, false);
			}
			else
				AddToResultList(range);
		}
		void ProcessAffectedRange(CellRangeBase affectedRange, bool singleRelativeRefRange) {
			CellRangeBase column = null;
			CellRangeBase row = null;
			CellRangeBase other = null;
			foreach (CellKey key in affectedRange.GetAllCellKeysEnumerable()) {
				CellPosition position = key.GetPosition();
				CellRange relativeRange = Info.GetRelativeReferenceRanges(position)[0];
				CellRange intersection = relativeRange.Intersection(ActionRange);
				System.Diagnostics.Debug.Assert(intersection != null);
				if (intersection.ContainsRange(relativeRange))
					ProcessCell(key);
				else if (intersection.Height == relativeRange.Height)
					row = AddPositionToRange(row, position);
				else if (intersection.Width == relativeRange.Width)
					column = AddPositionToRange(column, position);
				else
					other = AddPositionToRange(other, position);
			}
			ProcessCellRange(column);
			ProcessCellRange(row);
			ProcessCellRange(other);
		}
		CellRangeBase AddPositionToRange(CellRangeBase range, CellPosition position) {
			CellRange positionRange = new CellRange(Sheet, position, position);
			if (range == null)
				return positionRange;
			return range.MergeWithRange(positionRange);
		}
	}
	#endregion
	#region ProcessReferenceType (enum)
	public enum ProcessReferenceType {
		Both = 0,
		Absolute = 1,
		Relative = 2
	}
	#endregion
	#region RemovedShiftLeftDataValidationRPNVisitor
	public class RemovedShiftLeftDataValidationRPNVisitor : RemovedShiftLeftRPNVisitor {
		bool hasRelativeReferences;
		ProcessReferenceType referenceType;
		public RemovedShiftLeftDataValidationRPNVisitor(InsertRemoveRangeNotificationContextBase notificationContext, WorkbookDataContext dataContext)
			: base(notificationContext, dataContext) {
			referenceType = ProcessReferenceType.Both;
		}
		#region Properties
		public bool HasRelativeReferences { get { return hasRelativeReferences; } }
		public ProcessReferenceType ReferenceType { get { return referenceType; } set { referenceType = value; } }
		#endregion
		bool IsRelativeRef(CellOffset offset) {
			if (offset.ColumnType == CellOffsetType.Offset || offset.RowType == CellOffsetType.Offset) {
				hasRelativeReferences = true;
				return true;
			}
			return false;
		}
		bool ShouldProccesParsedThing(CellOffset offset) {
			if (referenceType == ProcessReferenceType.Both)
				return true;
			else if (referenceType == ProcessReferenceType.Absolute)
				return !IsRelativeRef(offset);
			return IsRelativeRef(offset);
		}
		public override void Visit(ParsedThingRef3dRel thing) {
			if (!ShouldProccesParsedThing(thing.Location) || !NotificationContext.CheckEqualSheetStartDefinition(DataContext, thing.SheetDefinitionIndex))
				return;
			base.Visit(thing);
		}
		public override void Visit(ParsedThingArea3dRel thing) {
			if ((!ShouldProccesParsedThing(thing.First) && !ShouldProccesParsedThing(thing.Last)) || 
				!NotificationContext.CheckEqualSheetStartDefinition(DataContext, thing.SheetDefinitionIndex))
				return;
			base.Visit(thing);
		}
		protected internal override ParsedThingBase ProcessRefRel(ParsedThingRefRel thing) {
			if (!ShouldProccesParsedThing(thing.Location) || !NotificationContext.CheckEqualSheetId(DataContext))
				return thing;
			CellPosition position = thing.Location.ToCellPosition(DataContext);
			CellRange range = new CellRange(DataContext.CurrentWorksheet, position, position);
			if (Logic.RemoveBehavior && NotificationContext.Range.Includes(range))
				return thing.GetRefErrorEquivalent();
			Logic.SetRange(range);
			if (Logic.ProcessRange()) {
				thing.Location = Logic.Range.TopLeft.ToCellOffset(DataContext);
				FormulaChanged = true;
			}
			return thing;
		}
		protected internal override ParsedThingBase ProcessAreaRel(ParsedThingAreaN thing) {
			if ((!ShouldProccesParsedThing(thing.First) && !ShouldProccesParsedThing(thing.Last)) || 
				!NotificationContext.CheckEqualSheetId(DataContext))
				return thing;
			CellPosition topLeft = thing.First.ToCellPosition(DataContext);
			CellPosition bottomRight = thing.Last.ToCellPosition(DataContext);
			CellRange range = new CellRange(DataContext.CurrentWorksheet, topLeft, bottomRight);
			if (Logic.RemoveBehavior && NotificationContext.Range.Includes(range))
				return thing.GetRefErrorEquivalent();
			Logic.SetRange(range);
			if (Logic.ProcessRange()) {
				thing.First = range.TopLeft.ToCellOffset(DataContext);
				thing.Last = range.BottomRight.ToCellOffset(DataContext);
				FormulaChanged = true;
			}
			return thing;
		}
	}
	#endregion
	#region RemovedShiftUpDataValidationRPNVisitor
	public class RemovedShiftUpDataValidationRPNVisitor : RemovedShiftLeftDataValidationRPNVisitor {
		public RemovedShiftUpDataValidationRPNVisitor(InsertRemoveRangeNotificationContextBase notificationContext, WorkbookDataContext dataContext)
			: base(notificationContext, dataContext) {
		}
		protected override RemoveShiftLeftRPNLogic GetLogic(CellRange cellRange) {
			return new RemoveShiftUpRPNLogic(cellRange);
		}
	}
	#endregion
	#region RemovedNoShiftDataValidationRPNVisitor
	public class RemovedNoShiftDataValidationRPNVisitor : RemovedShiftLeftDataValidationRPNVisitor {
		public RemovedNoShiftDataValidationRPNVisitor(InsertRemoveRangeNotificationContextBase notificationContext, WorkbookDataContext dataContext)
			: base(notificationContext, dataContext) {
		}
		public override CellRangeBase ProcessCellRange(CellRange range) {
			CellRange rangeValue = range.IntersectionWith(NotificationContext.Range).CellRangeValue as CellRange;
			if (rangeValue != null && range.Includes(rangeValue)) {
				CellRange firstRange = new CellRange(range.Worksheet, range.TopLeft, new CellPosition(rangeValue.BottomRight.Column, rangeValue.TopLeft.Row - 1));
				CellRange lastRange = new CellRange(range.Worksheet, new CellPosition(rangeValue.TopLeft.Column, rangeValue.BottomRight.Row + 1), range.BottomRight);
				List<CellRangeBase> resultRanges = new List<CellRangeBase>();
				if (!rangeValue.TopLeft.EqualsPosition(range.TopLeft))
					resultRanges.Add(firstRange);
				if (!rangeValue.BottomRight.EqualsPosition(range.BottomRight))
					resultRanges.Add(lastRange);
				if (resultRanges.Count == 0)
					return null;
				return new CellUnion(resultRanges);
			}
			return range;
		}
		protected override RemoveShiftLeftRPNLogic GetLogic(CellRange cellRange) {
			return new RemoveNoShiftRPNLogic(cellRange);
		}
	}
	#endregion
	#region InsertedShiftRightDataValidationRPNVisitor
	public class InsertedShiftRightDataValidationRPNVisitor : RemovedShiftLeftDataValidationRPNVisitor {
		public InsertedShiftRightDataValidationRPNVisitor(InsertRemoveRangeNotificationContextBase notificationContext, WorkbookDataContext dataContext)
			: base(notificationContext, dataContext) {
		}
		protected override RemoveShiftLeftRPNLogic GetLogic(CellRange cellRange) {
			return new InsertShiftRightDefinedNameRPNLogic(cellRange);
		}
	}
	#endregion
	#region InsertedShiftDownDataValidationRPNVisitor
	public class InsertedShiftDownDataValidationRPNVisitor : RemovedShiftLeftDataValidationRPNVisitor {
		public InsertedShiftDownDataValidationRPNVisitor(InsertRemoveRangeNotificationContextBase notificationContext, WorkbookDataContext dataContext)
			: base(notificationContext, dataContext) {
		}
		protected override RemoveShiftLeftRPNLogic GetLogic(CellRange cellRange) {
			return new InsertShiftDownRPNLogic(cellRange);
		}
	}
	#endregion
}
