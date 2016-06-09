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
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.Office;
namespace DevExpress.XtraSpreadsheet.Model {
	#region ArrayFormula
	public class ArrayFormula : Formula {
		#region Fields
		CellRange range;
		#endregion
		public ArrayFormula(ICell cell)
			: base(cell) {
		}
		public ArrayFormula(CellRange range)
			: base(range.GetFirstCellUnsafe() as ICell) {
			this.range = range;
		}
		public ArrayFormula(CellRange range, string body)
			: base(range.GetFirstCellUnsafe() as ICell) {
			if (String.IsNullOrEmpty(body) || body[0] != '=')
				ThrowErrorFormula(body, Context.Culture);
			this.range = range;
			ICell hostCell = (ICell)range.GetCellRelative(0, 0);
			ParseExpression(body, hostCell, true);
		}
		public ArrayFormula(CellRange range, ParsedExpression formulaExpression)
			: base(range.GetFirstCellUnsafe() as ICell, formulaExpression) {
			this.range = range;
		}
		#region Properties
		public CellRange Range { get { return this.range; } }
		protected override int BinaryFormulaOffset { get { return 13; } }
		public override FormulaType Type { get { return FormulaType.Array; } }
		#endregion
		#region Calculate
		protected override VariantValue CalculateCore(byte[] binaryFormula, ICell cell, WorkbookDataContext context) {
			try {
				context.PushArrayFormulaProcessing(true);
				VariantValue result = base.CalculateCore(binaryFormula, cell, context);
				if (!context.Workbook.CalculationChain.Enabled)
					result = PopulateRangeWithValues(result, context);
				return result;
			}
			finally {
				context.PopArrayFormulaProcessing();
			}
		}
		protected override VariantValue CalculateCore(ParsedExpression expression, ICell cell, WorkbookDataContext context) {
			try {
				context.PushArrayFormulaProcessing(true);
				VariantValue result = base.CalculateCore(expression, cell, context);
				if (!context.Workbook.CalculationChain.Enabled)
					result = PopulateRangeWithValues(result, context);
				return result;
			}
			finally {
				context.PopArrayFormulaProcessing();
			}
		}
		VariantValue PopulateRangeWithValues(VariantValue value, WorkbookDataContext context) {
			bool hasGettingData = false;
			if (value.IsArray) {
				IVariantArray array = value.ArrayValue;
				for (int i = 0; i < range.Width; i++)
					for (int j = 0; j < range.Height; j++) {
						ICellBase cell = range.GetCellRelative(i, j);
						if (cell == null)
							break;
						VariantValue cellValue = array.GetValue(j, i);
						if (cellValue.IsEmpty)
							cellValue = 0;
						if (cellValue == VariantValue.ErrorGettingData)
							hasGettingData = true;
						else
							((ICell)cell).AssignValue(cellValue);
					}
			}
			else if (value.IsCellRange) {
				CellRangeBase resultRange = value.CellRangeValue;
				for (int i = 0; i < range.Width; i++)
					for (int j = 0; j < range.Height; j++) {
						ICellBase cell = range.GetCellRelative(i, j);
						if (cell == null)
							break;
						VariantValue cellValue = resultRange.GetCellValueRelative(i, j);
						if (cellValue.IsEmpty)
							cellValue = 0;
						else if (cellValue == VariantValue.ErrorGettingData)
							hasGettingData = true;
						else
							((ICell)cell).AssignValue(cellValue);
					}
			}
			if (hasGettingData)
				return VariantValue.ErrorGettingData;
			return context.DereferenceValue(value, true);
		}
		#endregion
		#region ParseExpressionCore
		protected override ParsedExpression ParseExpressionCore(string body, WorkbookDataContext context) {
			try {
				context.PushArrayFormulaProcessing(true);
				return context.ParseExpression(body, OperandDataType.Value, true);
			}
			finally {
				context.PopArrayFormulaProcessing();
			}
		}
		#endregion
		#region GetInvolvedCellRangesCore
		public override List<CellRangeBase> GetPrecedents(ICell cell) {
			ParsedExpression expression = GetExpression(Context);
			WorkbookDataContext context = cell.Context;
			context.PushCurrentCell(cell);
			context.PushArrayFormulaProcessing(true);
			try {
				return Expression.GetInvolvedCellRanges(context);
			}
			finally {
				context.PopArrayFormulaProcessing();
				context.PopCurrentCell();
			}
		}
		protected override List<PrecedentPair> GetInvolvedCellRangesCore(ICell cell) {
			WorkbookDataContext context = cell.Context;
			try {
				context.PushArrayFormulaProcessing(true);
				IChainPrecedent precedent = new ArrayFormulaRangeChainPrecedent(Range);
				return CalculateChainPrecedents(context, precedent);
			}
			finally {
				context.PopArrayFormulaProcessing();
			}
		}
		#endregion
		#region CheckIntegrity
		public override void CheckIntegrity() {
			base.CheckIntegrity();
			if (range == null)
				IntegrityChecks.Fail("ArrayFormula: range should not be null");
		}
		#endregion
		#region Notifications
		public void OnRangeRemoving(RemoveRangeNotificationContext notificationContext, int formulaIndex) {
			ProcessFormula(formulaIndex, notificationContext.Visitor);
		}
		public void OnRangeInserting(InsertRangeNotificationContext notificationContext, int formulaIndex) {
			ProcessFormula(formulaIndex, notificationContext.Visitor);
		}
		void ProcessFormula(int formulaIndex, ReferenceThingsRPNVisitor walker) {
			CellRange newRange = walker.ProcessCellRange((CellRange)range.Clone()) as CellRange;
			if (newRange == null) {
				range = null;
				return;
			}
			bool rangeChanged = !range.Equals(newRange);
			bool expressionModified = ProcessExpression(walker);
			if (expressionModified || rangeChanged) {
				ICell oldHostCell = Cell;
				ChangeRange(formulaIndex, newRange);
				BinaryFormula = null;
				range = newRange.GetCoveredRange();
				if (!rangeChanged) 
					ApplyDataToCellTransacted(oldHostCell);
			}
		}
		void ChangeRange(int formulaIndex, CellRange newRange) {
			if (newRange == null)
				return;
			Worksheet worksheet = range.Worksheet as Worksheet;
			ArrayFormulaRangeChangedHistoryItem historyItem = new ArrayFormulaRangeChangedHistoryItem(worksheet, formulaIndex, this, newRange);
			worksheet.Workbook.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void ChangeRangeCore(int formulaIndex, CellRange newRange, CellRange realCellLocation) {
			UpdateTopCellPositionInGuestCells(newRange, realCellLocation);
			ICell oldHostCell = Cell;
			this.range = newRange;
			GetExpression(oldHostCell.Worksheet.DataContext);
			BinaryFormula = null;
			ApplyDataToCell(oldHostCell);
			Worksheet worksheet = range.Worksheet as Worksheet;
			if (worksheet != null)
				worksheet.ArrayFormulaRanges.ChangeRange(formulaIndex, newRange);
		}
		protected void UpdateTopCellPositionInGuestCells(CellRangeBase newRange, CellRange realCellLocation) {
			CellPosition topLeft = newRange.TopLeft;
			CellPositionOffset offset = new CellPositionOffset(topLeft.Column - range.TopLeft.Column, topLeft.Row - range.TopLeft.Row);
			foreach (ICell cell in realCellLocation.GetAllCellsEnumerable()) {
				if (cell.Position.EqualsPosition(realCellLocation.TopLeft))
					continue;
				ArrayFormulaPart formulaPart = ((ArrayFormulaPart)cell.GetFormula());
				formulaPart.TopLeftCellPosition = topLeft;
				formulaPart.ApplyDataToCell(cell); 
			}
		}
		bool ProcessExpression(ReferenceThingsRPNVisitor walker) {
			ParsedExpression preparedExpression = GetExpression(Context);
			if (Expression == null)
				return false;
			Expression = walker.Process(preparedExpression);
			return walker.FormulaChanged;
		}
		protected internal override void UpdateFormula(ReplaceThingsPRNVisitor walker, ICell ownerCell) {
		}
		public bool CanRangeRemove(CellRange cellRange, RemoveCellMode mode) {
			if (cellRange.Includes(range))
				return true;
			if (cellRange.Intersects(this.range))
				return false;
			if (mode == RemoveCellMode.Default || mode == RemoveCellMode.NoShiftOrRangeToPasteCutRange)
				return true;
			CellRange prohibitedRange = new CellRange(Range.Worksheet,
				mode == RemoveCellMode.ShiftCellsLeft ? new CellPosition(0, Range.TopLeft.Row) : new CellPosition(Range.TopLeft.Column, 0),
				mode == RemoveCellMode.ShiftCellsLeft ? new CellPosition(Range.TopLeft.Column - 1, Range.BottomRight.Row) : new CellPosition(Range.BottomRight.Column, Range.TopLeft.Row - 1));
			if (prohibitedRange.Intersects(cellRange)) {
				if (mode == RemoveCellMode.ShiftCellsLeft)
					return cellRange.IntersectionWith(prohibitedRange).CellRangeValue.Height == prohibitedRange.Height;
				else
					return cellRange.IntersectionWith(prohibitedRange).CellRangeValue.Width == prohibitedRange.Width;
			}
			return true;
		}
		public bool CanRangeInsert(CellRange cellRange, InsertCellMode mode) {
			if (cellRange.Includes(range))
				return true;
			if (cellRange.Intersects(this.range)) {
				CellRangeBase intersectionRange = cellRange.IntersectionWith(this.range).CellRangeValue;
				if (mode == InsertCellMode.ShiftCellsRight)
					if (intersectionRange.Height != this.range.Height || cellRange.BottomRight.Column > this.range.TopLeft.Column)
						return false;
				if (mode == InsertCellMode.ShiftCellsDown)
					if (intersectionRange.Width != this.range.Width || cellRange.BottomRight.Row > this.range.TopLeft.Row)
						return false;
			}
			CellRange prohibitedRange = new CellRange(Range.Worksheet,
				mode == InsertCellMode.ShiftCellsRight ? new CellPosition(0, Range.TopLeft.Row) : new CellPosition(Range.TopLeft.Column, 0),
				mode == InsertCellMode.ShiftCellsRight ? new CellPosition(Range.TopLeft.Column, Range.BottomRight.Row) : new CellPosition(Range.BottomRight.Column, Range.TopLeft.Row));
			if (prohibitedRange.Intersects(cellRange)) {
				if (mode == InsertCellMode.ShiftCellsRight)
					return cellRange.IntersectionWith(prohibitedRange).CellRangeValue.Height == prohibitedRange.Height;
				else
					return cellRange.IntersectionWith(prohibitedRange).CellRangeValue.Width == prohibitedRange.Width;
			}
			return true;
		}
		#endregion
		#region Binary <-> ParsedExpression conversion
		protected override void WriteAdditionalDataToBinary(byte[] data, FormulaDataState dataState) {
			uint flagByte = (uint)Type;
			PackedValues.SetIntBitValue(ref flagByte, FormulaBase.DataStateMask, FormulaBase.DataStateOffset, (int)dataState);
			PackedValues.SetBoolBitValue(ref flagByte, FormulaBase.CalculateAlwaysMask, CalculateAlways);
			data[0] = (byte)flagByte;
			BinaryRPNWriterBase.WriteCellRange(range, data, 1);
		}
		protected override void ReadAdditionalDataFromBinary(Worksheet sheet, byte[] data) {
			byte flagByte = data[0];
			System.Diagnostics.Debug.Assert((flagByte & FormulaTypeMask) == 1);
			CalculateAlways = PackedValues.GetBoolBitValue(flagByte, FormulaBase.CalculateAlwaysMask);
			range = BinaryRPNReaderBase.ReadCellRange(data, 1, sheet);
		}
		protected override ParsedExpression ReadParsedExpression(byte[] formulaPtgs, int startIndex, WorkbookDataContext context) {
			context.PushArrayFormulaProcessing(true);
			try {
				return base.ReadParsedExpression(formulaPtgs, startIndex, context);
			}
			finally {
				context.PopArrayFormulaProcessing();
			}
		}
		protected override byte[] GetBinaryCore(WorkbookDataContext context) {
			context.PushArrayFormulaProcessing(true);
			try {
				return base.GetBinaryCore(context);
			}
			finally {
				context.PopArrayFormulaProcessing();
			}
		}
		#endregion
		public override void PushSettingsToContext(WorkbookDataContext context, ICell cell) {
			context.PushArrayFormulaProcessing(true);
		}
		public override void PopSettingsFromContext(WorkbookDataContext context) {
			context.PopArrayFormulaProcessing();
		}
		public bool IsEqual(ArrayFormula other) {
			return range.Equals(other.Range); 
		}
	}
	#endregion
	#region ArrayFormulaPart
	public class ArrayFormulaPart : FormulaBase {
		#region Fields
		CellPosition topLeftCellPosition;
		ICell ownerCell;
		#endregion
		public ArrayFormulaPart(ICell ownerCell)
			: base() {
			Guard.ArgumentNotNull(ownerCell, "ownerCell");
			this.ownerCell = ownerCell;
		}
		public ArrayFormulaPart(ICell ownerCell, ICell topLeftCell)
			: this(ownerCell) {
			Guard.ArgumentNotNull(topLeftCell, "topLeftCell");
			this.topLeftCellPosition = topLeftCell.Position;
		}
		#region Properties
		public ICell TopLeftCell { get { return ownerCell.Worksheet[topLeftCellPosition]; } }
		public CellPosition TopLeftCellPosition { get { return topLeftCellPosition; } set { topLeftCellPosition = value; } }
		public CellPositionOffset CellOffset { get { return new CellPositionOffset(topLeftCellPosition, ownerCell.Position); } }
		protected override int BinaryFormulaOffset { get { return 7; } }
		public override ParsedExpression Expression {
			get {
				return TopLeftCell.GetFormula().Expression;
			}
			protected set {
				throw new ArgumentException();
			}
		}
		public override FormulaType Type { get { return FormulaType.ArrayPart; } }
		#endregion
		public override FormulaDataState GetDataState() {
			return FormulaDataState.ExpressionReady;
		}
		public override ParsedExpression GetExpression(WorkbookDataContext context) {
			return TopLeftCell.GetFormula().GetExpression(context);
		}
		public override FormulaProperties GetProperties() {
			return TopLeftCell.GetFormula().GetProperties();
		}
		#region GetBody
		public override string GetBody(ICell hostCell) {
			return TopLeftCell.GetFormula().GetBody(hostCell);
		}
		#endregion
		#region Calculate
		public override VariantValue Calculate(ICell cell, WorkbookDataContext context) {
			context.PushArrayFormulaOffcet(new CellPositionOffset(topLeftCellPosition, cell.Position));
			try {
				return TopLeftCell.GetFormula().Calculate(cell, context);
			}
			finally {
				context.PopArrayFromulaOffcet();
			}
		}
		#endregion
		#region GetInvolvedCellRanges
		public override List<PrecedentPair> GetInvolvedCellRanges(ICell cell) {
			return new List<PrecedentPair>();
		}
		public override List<CellRangeBase> GetPrecedents(ICell cell) {
			ParsedExpression expression = GetExpression(cell.Worksheet.DataContext);
			WorkbookDataContext context = cell.Context;
			context.PushCurrentCell(cell);
			context.PushArrayFormulaProcessing(true);
			try {
				return Expression.GetInvolvedCellRanges(context);
			}
			finally {
				context.PopArrayFormulaProcessing();
				context.PopCurrentCell();
			}
		}
		#endregion
		#region OnBeforeSheetRemoved
		public override void OnBeforeSheetRemoved(RemoveRangeNotificationContext notificationContext, WorkbookDataContext dataContext) {
		}
		#endregion
		#region CheckIntegrity
		public override void CheckIntegrity(ICell cell) {
			CheckIntegrity();
		}
		public override void CheckIntegrity() {
			if (TopLeftCell == null)
				IntegrityChecks.Fail("ArrayFormulaPart: topLeftCell should not be null");
		}
		#endregion
		#region Binary <-> ParsedExpression conversion
		public override void InitializeFromBinary(byte[] binaryFormula, Worksheet sheet) {
			Guard.ArgumentNotNull(binaryFormula, "binaryFormula");
			Guard.ArgumentNotNull(sheet, "sheet");
			byte flagByte = binaryFormula[0];
			System.Diagnostics.Debug.Assert((flagByte & FormulaBase.DataStateMask) == ((int)FormulaDataState.Binary << DataStateOffset));
			System.Diagnostics.Debug.Assert((flagByte & FormulaBase.FormulaTypeMask) == (int)FormulaType.ArrayPart);
			CalculateAlways = PackedValues.GetBoolBitValue(flagByte, FormulaBase.CalculateAlwaysMask);
			topLeftCellPosition = BinaryRPNReaderBase.ReadCellPosition(binaryFormula, 1);
		}
		public override byte[] GetBinary(WorkbookDataContext context) {
			byte[] result = new byte[BinaryFormulaOffset];
			uint flagByte = (uint)Type;
			PackedValues.SetIntBitValue(ref flagByte, FormulaBase.FormulaTypeMask, (int)Type);
			PackedValues.SetIntBitValue(ref flagByte, FormulaBase.DataStateMask, FormulaBase.DataStateOffset, (int)FormulaDataState.Binary);
			PackedValues.SetBoolBitValue(ref flagByte, FormulaBase.CalculateAlwaysMask, CalculateAlways);
			result[0] = (byte)flagByte;
			BinaryRPNWriterBase.WriteCellPostion(topLeftCellPosition, result, 1);
			return result;
		}
		#endregion
		public override void PushSettingsToContext(WorkbookDataContext context, ICell cell) {
			context.PushArrayFormulaProcessing(true);
			context.PushArrayFormulaOffcet(new CellPositionOffset(topLeftCellPosition, cell.Position));
		}
		public override void PopSettingsFromContext(WorkbookDataContext context) {
			context.PopArrayFromulaOffcet();
			context.PopArrayFormulaProcessing();
		}
	}
	#endregion
	#region ArrayFormulaCollection
	public class ArrayFormulaRangesCollection : UndoableCollection<CellRange> {
		public ArrayFormulaRangesCollection(Worksheet worksheet)
			: base(worksheet) {
		}
		ArrayFormula GetArrayFormulaByRange(CellRange range) {
			ICell cell = range.Worksheet.GetCell(range.TopLeft.Column, range.TopLeft.Row) as ICell;
			return cell.GetFormula() as ArrayFormula;
		}
		public ArrayFormula GetArrayFormulaByRangeIndex(int index) {
			if (index < 0 || index >= Count)
				throw new ArgumentException("Index");
			return GetArrayFormulaByRange(this[index]);
		}
		#region OnRangeRemoving
		public void OnRangeRemoving(RemoveRangeNotificationContext context) {
			for (int i = Count - 1; i >= 0; i--) {
				ArrayFormula formula = GetArrayFormulaByRange(this[i]);
				formula.OnRangeRemoving(context, i);
				if (formula.Range == null)
					RemoveAt(i);
			}
		}
		#endregion
		#region OnRangeInserting
		public void OnRangeInserting(InsertRangeNotificationContext context) {
			for (int i = Count - 1; i >= 0; i--) {
				ArrayFormula formula = GetArrayFormulaByRange(this[i]);
				formula.OnRangeInserting(context, i);
			}
		}
		#endregion
		#region CanRangeRemove
		public bool CanRangeRemove(CellRangeBase cellRange, RemoveCellMode mode) {
			if (cellRange.RangeType == CellRangeType.UnionRange) {
				foreach (CellRange currentRange in (cellRange as CellUnion).InnerCellRanges)
					if (!CanSingleRangeRemove(currentRange, mode))
						return false;
				return true;
			}
			return CanSingleRangeRemove(cellRange as CellRange, mode);
		}
		#endregion
		#region CanSingleRangeRemove
		bool CanSingleRangeRemove(CellRange cellRange, RemoveCellMode mode) {
			for (int i = Count - 1; i >= 0; i--) {
				ArrayFormula formula = GetArrayFormulaByRange(this[i]);
				if (!formula.CanRangeRemove(cellRange, mode))
					return false;
			}
			return true;
		}
		#endregion
		#region CanRangeInsert
		public bool CanRangeInsert(CellRangeBase cellRange, InsertCellMode mode) {
			if (cellRange.RangeType == CellRangeType.UnionRange) {
				foreach (CellRange currentRange in (cellRange as CellUnion).InnerCellRanges)
					if (!CanSingleRangeInsert(currentRange, mode))
						return false;
				return true;
			}
			return CanSingleRangeInsert(cellRange as CellRange, mode);
		}
		#endregion
		#region CanSingleRangeInsert
		bool CanSingleRangeInsert(CellRange cellRange, InsertCellMode mode) {
			for (int i = Count - 1; i >= 0; i--) {
				ArrayFormula formula = GetArrayFormulaByRange(this[i]);
				if (!formula.CanRangeInsert(cellRange, mode))
					return false;
			}
			return true;
		}
		#endregion
		#region CheckChangePartOfArray
		public bool CheckChangePartOfArray(ISheetPosition range) {
			if (Count <= 0)
				return false;
			foreach (CellRange formulaRange in this) {
				if (range.Intersects(formulaRange) && !range.Includes(formulaRange))
					return true;
			}
			return false;
		}
		public bool CheckChangePartOfArray(CellRangeBase range) {
			if (Count <= 0)
				return false;
			foreach (CellRange formulaRange in this) {
				if (!range.Intersects(formulaRange))
					continue;
				if (range.RangeType == CellRangeType.UnionRange)
					return true;
				if (!((CellRange)range).Includes(formulaRange))
					return true;
			}
			return false;
		}
		#endregion
		#region CheckMultiCellArrayFormulasInRange
		public bool CheckMultiCellArrayFormulasInRange(CellRangeBase range) {
			foreach (CellRange formulaRange in this) {
				if (!range.Intersects(formulaRange))
					continue;
				if (formulaRange.CellCount > 1)
					return true;
			}
			return false;
		}
		#endregion
		#region FindArrayFormulaIndex
		public int FindArrayFormulaIndex(ICell cell) {
			for (int i = Count - 1; i >= 0; i--) {
				CellRange item = this[i];
				if (item.ContainsCell(cell))
					return i;
			}
			return -1;
		}
		#endregion
		#region FindArrayFormulaIndex
		public int FindArrayFormulaIndex(int columnIndex, int rowIndex) {
			for (int i = Count - 1; i >= 0; i--) {
				CellRange item = this[i];
				if (item.ContainsCell(columnIndex, rowIndex))
					return i;
			}
			return -1;
		}
		#endregion
		#region FindArrayFormulaIndexExactRange
		public int FindArrayFormulaIndexExactRange(CellRange cellRange) {
			for (int i = Count - 1; i >= 0; i--) {
				CellRange item = this[i];
				if (item.Includes(cellRange))
					return i;
			}
			return -1;
		}
		#endregion
		#region FindArrayFormulaIndexInsideRange
		public int FindArrayFormulaIndexInsideRange(CellRange cellRange, int initialIndexFromTheEnd) {
			for (int i = initialIndexFromTheEnd; i >= 0; i--) {
				CellRange item = this[i];
				if (cellRange.Includes(item))
					return i;
			}
			return -1;
		}
		#endregion
		#region GetArrayFormulaRangesIntersectsRange
		public List<CellRange> GetArrayFormulaRangesIntersectsRange(CellRange range) {
			List<CellRange> result = new List<CellRange>();
			int count = Count;
			for (int i = 0; i < count; i++) {
				CellRange arrayRange = this[i];
				if (arrayRange.Intersects(range) && !range.Includes(arrayRange))
					result.Add(arrayRange);
			}
			return result;
		}
		#endregion
		public bool HasIntersectedSubtotalArrays(CellRange range) {
			foreach (CellRange arrayRange in this) {
				if (arrayRange.Intersects(range))
					return true;
				if (arrayRange.TopRowIndex >= range.TopRowIndex && arrayRange.TopRowIndex <= range.BottomRowIndex)
					return true;
				if (arrayRange.BottomRowIndex >= range.TopRowIndex && arrayRange.BottomRowIndex <= range.BottomRowIndex)
					return true;
			}
			return false;
		}
		internal void ChangeRange(int formulaIndex, CellRange newRange) {
			CellRange currentRange = this[formulaIndex];
			currentRange.TopLeft = newRange.TopLeft;
			currentRange.BottomRight = newRange.BottomRight;
		}
		public bool Exists(Predicate<CellRange> match) {
			return InnerList.Exists(match);
		}
		public CellRange Find(Predicate<CellRange> match) {
			return InnerList.Find(match);
		}
		public List<CellRange> FindAll(Predicate<CellRange> match) {
			return InnerList.FindAll(match);
		}
	}
	#endregion
	public class ArrayFormulaRangeChangedHistoryItem : SpreadsheetHistoryItem {
		readonly int formulaIndex;
		readonly ArrayFormula formula;
		readonly CellRange newRange;
		CellRange oldRange;
		public ArrayFormulaRangeChangedHistoryItem(Worksheet worksheet, int formulaIndex, ArrayFormula formula, CellRange newRange)
			: base(worksheet) {
			this.formulaIndex = formulaIndex;
			this.formula = formula;
			this.newRange = newRange;
		}
		protected override void RedoCore() {
			oldRange = formula.Range;
			formula.ChangeRangeCore(formulaIndex, newRange, oldRange);
		}
		protected override void UndoCore() {
			formula.ChangeRangeCore(formulaIndex, oldRange, oldRange);
		}
	}
}
