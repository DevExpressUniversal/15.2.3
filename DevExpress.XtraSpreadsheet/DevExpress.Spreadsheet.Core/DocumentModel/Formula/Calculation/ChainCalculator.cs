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

using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Services;
using System;
using System.Collections.Generic;
using System.Text;
namespace DevExpress.XtraSpreadsheet.Model {
	public interface IChainCalculator {
		bool Calculating { get; set; }
		void Calculate(ModelCalculationMode calculationMode);
		bool IsCalculated(ICell cell);
		bool TryGetCalculatedValue(ICell cell, out VariantValue calculatedValue);
	}
	#region ChainCalculator
	public class ChainCalculator : IChainCalculator {
		#region Fields
		ModelCalculationMode calculationMode;
		readonly DocumentModel documentModel;
		readonly CellsChain cellsChain;
		PendingCellsCache pendingCells;
		volatile bool calculating;
		ICell currentCell;
		IList<ICell> notCalculatedCells;
		#endregion
		public ChainCalculator(DocumentModel documentModel, CellsChain cellsChain) {
			this.documentModel = documentModel;
			this.cellsChain = cellsChain;
		}
		public CellsChain CellsChain { get { return cellsChain; } }
		public ICell CurrentCell { get { return currentCell; } set { currentCell = value; } }
		public DocumentModel DocumentModel { get { return documentModel; } }
		public IList<ICell> NotCalculatedCells { get { return notCalculatedCells; } }
		public PendingCellsCache PendingCells { get { return pendingCells; } set { pendingCells = value; } }
		public WorkbookDataContext DataContext { get { return DocumentModel.DataContext; } }
		public bool Calculating { get { return calculating; } set { calculating = value; } }
		public virtual void Calculate(ModelCalculationMode calculationMode) {
			this.calculationMode = calculationMode;
			pendingCells = new PendingCellsCache();
			notCalculatedCells = new List<ICell>();
			Calculating = true;
			MarkupCalculateAlwaysCells();
			DoCalculate();
		}
		protected virtual void MarkupCalculateAlwaysCells() {
			currentCell = cellsChain.Header;
			while (currentCell != null) {
				if (FormulaFactory.GetFormulaCalculateAlways(currentCell)) {
					currentCell.MarkUpForRecalculation();
					FormulaFactory.SetFormulaCalculateAlways(currentCell, currentCell.Formula.IsVolatile());
				}
				currentCell = currentCell.FormulaInfo.NextCell;
			}
		}
		protected virtual void DoCalculate() {
			WorkbookDataContext threadContext = DocumentModel.DataContext;
			try {
				currentCell = cellsChain.Header;
#if DEBUGTEST
				StringBuilder trace = new StringBuilder();
				int calculatedCellsCounter = 0;
				int nonMarkedCellsCounter = 0;
#endif
				while (currentCell != null) {
					if (currentCell.MarkedForRecalculation) {
						NotCalculatedCells.Clear();
						VariantValue calculationResult = CalculateCellCore(currentCell, threadContext);
						if (NotCalculatedCells.Count > 0) {
							PushNotCalculatedCells(NotCalculatedCells, currentCell);
							currentCell.MarkUpForRecalculation();
						}
						else {
							pendingCells.PopPendingCells(currentCell, cellsChain);
							currentCell.Worksheet.WebRanges.AddChangedCellPosition(currentCell);
#if DEBUGTEST
							calculatedCellsCounter++;
#endif
						}
#if DEBUGTEST
#endif
					}
#if DEBUGTEST
					else {
						nonMarkedCellsCounter++;
					}
#endif
#if DEBUGTEST
#endif
					currentCell = currentCell.FormulaInfo.NextCell;
				}
				bool hasCirculars = pendingCells.Count > 0;
				DocumentModel.HasCircularReferences = hasCirculars;
				if (hasCirculars)
					CalculateCircularReferences(threadContext);
			}
			finally {
				pendingCells = null;
				Calculating = false;
			}
		}
		protected void PushNotCalculatedCells(IList<ICell> cells, ICell currentCell) {
			if (cells.Count <= 0)
				return;
			if (cells.Count == 1)
				pendingCells.RegisterPendingWaiter(cells[0], currentCell);
			else {
				CalculationFewCellsWaiter waiter = new CalculationFewCellsWaiter(currentCell);
				int counter = 0;
				foreach (ICell cell in cells) {
					pendingCells.RegisterPendingWaiter(cell, waiter);
					counter++;
				}
				waiter.Counter = counter;
			}
			cellsChain.UnRegisterCell(currentCell);
		}
		internal virtual VariantValue CalculateCellCore(ICell cell, WorkbookDataContext context) {
			FormulaBase formula = cell.GetFormula();
			context.PushArrayFormulaProcessing(false);
			context.PushDefinedNameProcessing(null);
			context.PushArrayFormulaOffcet(CellPositionOffset.None);
			context.PushRelativeToCurrentCell(false);
			DocumentModel.RealTimeDataManager.OnStartCellCalculation(cell);
			try {
				VariantValue formulaResult = formula.Calculate(cell, context);
				if (formulaResult.IsMissing) {
					cell.ContentVersion = DocumentModel.ContentVersion;
					return cell.ExtractValue();
				}
				else {
					if (formulaResult != VariantValue.ErrorGettingData)
						cell.AssignValue(formulaResult);
					return formulaResult;
				}
			}
			finally {
				DocumentModel.RealTimeDataManager.OnEndCellCalculation(cell);
				context.PopArrayFormulaProcessing();
				context.PopDefinedNameProcessing();
				context.PopArrayFromulaOffcet();
				context.PopRelativeToCurrentCell();
			}
		}
		protected internal void PrepareCalculation(ICell currentCell) {
			pendingCells = new PendingCellsCache();
			Calculating = true;
			this.currentCell = currentCell;
		}
		#region CalculateCirculars
		void CalculateCircularReferences(WorkbookDataContext context) {
			CalculationOptions calculationOptions = DocumentModel.Properties.CalculationOptions;
			ICell lastNormallyCalculatedCell = cellsChain.Footer;
			pendingCells.PushPengingCellsToTheEndOfChain(cellsChain);
			if (lastNormallyCalculatedCell != null)
				lastNormallyCalculatedCell = lastNormallyCalculatedCell.FormulaInfo.NextCell;
			if (calculationOptions.IterationsEnabled)
				CalculateChainIterative(lastNormallyCalculatedCell, calculationOptions.MaximumIterations, calculationOptions.IterativeCalculationDelta, context);
		}
		protected virtual List<ICell> CalculateChainIterative(ICell startCell, int maxIterations, double delta, WorkbookDataContext context) {
			DocumentModel.Iterative = true;
			try {
				List<ICell> cellsWithCircular = new List<ICell>();
				if (startCell == null)
					startCell = cellsChain.Header;
				while (startCell != null) {
					cellsWithCircular.Add(startCell);
					startCell = startCell.FormulaInfo.NextCell;
				}
				cellsWithCircular.Sort(CompareCells);
				int cellCount = cellsWithCircular.Count;
				for (int i = 0; i < maxIterations; i++) {
					int nonChangedCellsCounter = 0;
					foreach (ICell cell in cellsWithCircular) {
						VariantValue startValue = cell.GetValue();
						VariantValue endValue = CalculateCellCore(cell, context);
						if (GetDelta(startValue, endValue) < delta)
							nonChangedCellsCounter++;
					}
					if (nonChangedCellsCounter >= cellCount)
						break;
				}
				return cellsWithCircular;
			}
			finally {
				DocumentModel.Iterative = false;
			}
		}
		double GetDelta(VariantValue startValue, VariantValue endValue) {
			if (!startValue.IsNumeric || !endValue.IsNumeric)
				return 0;
			return Math.Abs(endValue.NumericValue - startValue.NumericValue);
		}
		int CompareCells(ICell x, ICell y) {
			if (x.Sheet.SheetId != y.Sheet.SheetId)
				return x.Sheet.Name.CompareTo(y.Sheet.Name);
			return x.Key.CompareTo(y.Key);
		}
		#endregion
		public virtual bool IsCalculated(ICell cell) {
			if (!cell.HasFormula || !Calculating)
				return true;
			if (DocumentModel.Iterative || calculationMode == ModelCalculationMode.Manual)
				return true;
			if (FormulaFactory.GetFormulaCalculateAlways(cell))
				FormulaFactory.SetFormulaCalculateAlways(currentCell, true);
			return !cell.MarkedForRecalculation;
		}
		public bool TryGetCalculatedValue(ICell cell, out VariantValue calculatedValue) {
			if (IsCalculated(cell)) {
				calculatedValue = cell.GetValue();
				return true;
			}
			NotCalculatedCells.Add(cell);
			calculatedValue = VariantValue.ErrorGettingData;
			return false;
		}
		internal void Reset() {
			pendingCells = new PendingCellsCache();
			Calculating = false;
		}
	}
	#endregion
	#region ChainRangeCalculator
	public class ChainRangeCalculator : ChainCalculator {
		#region Fields
		readonly CellRangeBase range;
		#endregion
		public ChainRangeCalculator(DocumentModel documentModel, CellsChain cellsChain, CellRangeBase range)
			: base(documentModel, cellsChain) {
			this.range = range;
		}
		#region Properties
		public CellRangeBase Range { get { return range; } }
		#endregion
		internal static ChainRangeCalculator GetInstance(DocumentModel documentModel, CellsChain chain, CellRangeBase range) {
			if (documentModel.Properties.CalculationOptions.IterationsEnabled)
				return new ChainRangeIterativeCalculator(documentModel, chain, range);
			return new ChainRangeCalculator(documentModel, chain, range);
		}
		protected override void MarkupCalculateAlwaysCells() {
		}
		int CalculateFormulaCellCount() {
			int cellsWithFormulaCounter = 0;
			foreach (ICellBase cellBase in Range.GetExistingCellsEnumerable()) {
				ICell cell = cellBase as ICell;
				if (cell == null && !cell.HasFormula)
					continue;
				if (FormulaFactory.GetFormulaCalculateAlways(cell))
					FormulaFactory.SetFormulaCalculateAlways(cell, cell.Formula.IsVolatile());
				cellsWithFormulaCounter++;
			}
			return cellsWithFormulaCounter;
		}
		protected override void DoCalculate() {
			int calculatedCellsCount = 0;
			int cellsWithFormulaCounter = CalculateFormulaCellCount();
			WorkbookDataContext threadContext = DocumentModel.DataContext;
			try {
				ICell previousCellFromRange = null;
				CurrentCell = CellsChain.Header;
				while (CurrentCell != null && calculatedCellsCount < cellsWithFormulaCounter) {
					ICell nextCell = CurrentCell.FormulaInfo.NextCell;
					if (range.ContainsCell(CurrentCell)) {
						NotCalculatedCells.Clear();
						VariantValue calculationResult = CalculateCellCore(CurrentCell, threadContext);
						if (NotCalculatedCells.Count > 0) {
							PushNotCalculatedCells(NotCalculatedCells, CurrentCell);
							CurrentCell.MarkUpForRecalculation();
						}
						else {
							PendingCells.PopPendingCells(CurrentCell, CellsChain);
							nextCell = CurrentCell.FormulaInfo.NextCell;
							MoveCurrentCellCloserToChainHeader(previousCellFromRange);
							CurrentCell.Worksheet.WebRanges.AddChangedCellPosition(CurrentCell);
							calculatedCellsCount++;
						}
						previousCellFromRange = CurrentCell;
					}
					CurrentCell = nextCell;
				}
			}
			finally {
				PendingCells = null;
				Calculating = false;
			}
		}
		void MoveCurrentCellCloserToChainHeader(ICell previousCellFromRange) {
			if (!object.ReferenceEquals(previousCellFromRange, CurrentCell.FormulaInfo.PreviousCell)) {
				CellsChain.UnRegisterCell(CurrentCell);
				if (previousCellFromRange == null)
					CellsChain.RegisterCell(CurrentCell);
				else
					CellsChain.InsertAfter(previousCellFromRange, CurrentCell);
			}
		}
		public override bool IsCalculated(ICell cell) {
			if (!cell.HasFormula || !Calculating)
				return true;
			if (DocumentModel.Iterative)
				return true;
			if (!Range.ContainsCell(cell))
				return true;
			if (FormulaFactory.GetFormulaCalculateAlways(cell))
				FormulaFactory.SetFormulaCalculateAlways(CurrentCell, true);
			return !cell.MarkedForRecalculation;
		}
	}
	#endregion
	#region ChainCustomCalculator
	public class ChainCustomCalculator : ChainCalculator {
		#region Fields
		readonly ICustomCalculationService service;
		#endregion
		public ChainCustomCalculator(DocumentModel documentModel, CellsChain cellsChain, ICustomCalculationService service)
			: base(documentModel, cellsChain) {
			this.service = service;
		}
		#region Properties
		public ICustomCalculationService Service { get { return service; } }
		#endregion
		public override void Calculate(ModelCalculationMode calculationMode) {
			if (!service.OnBeginCalculation())
				return;
			try {
				base.Calculate(calculationMode);
			}
			finally {
				service.OnEndCalculation();
			}
		}
		protected override void MarkupCalculateAlwaysCells() {
			bool shouldMarkUp = service.ShouldMarkupCalculateAlwaysCells();
			ICell currentCell = CellsChain.Header;
			while (currentCell != null) {
				if (FormulaFactory.GetFormulaCalculateAlways(currentCell)) {
					if (shouldMarkUp)
						currentCell.MarkUpForRecalculation();
					FormulaFactory.SetFormulaCalculateAlways(currentCell, currentCell.Formula.IsVolatile());
				}
				currentCell = currentCell.FormulaInfo.NextCell;
			}
		}
		internal override VariantValue CalculateCellCore(ICell cell, WorkbookDataContext context) {
			DevExpress.Spreadsheet.CellValue startNativeValue = new Spreadsheet.CellValue(cell.ExtractValue(), context);
			int sheetIndex = cell.Worksheet.Workbook.Sheets.GetIndexById(cell.SheetId);
			DevExpress.Spreadsheet.CellKey nativeCellKey = new Spreadsheet.CellKey(sheetIndex, cell.ColumnIndex, cell.RowIndex);
			DevExpress.Spreadsheet.Formulas.CellCalculationArgs args = new DevExpress.Spreadsheet.Formulas.CellCalculationArgs(nativeCellKey, startNativeValue);
			service.OnBeginCellCalculation(args);
			if (args.Handled) {
				VariantValue newValue = args.Value.ModelVariantValue;
				cell.AssignValue(newValue);
				return newValue;
			}
			VariantValue result = base.CalculateCellCore(cell, context);
			if (NotCalculatedCells.Count <= 0) {
				DevExpress.Spreadsheet.CellValue endNativeValue = new DevExpress.Spreadsheet.CellValue(result, context);
				service.OnEndCellCalculation(nativeCellKey, startNativeValue, endNativeValue);
			}
			return result;
		}
		protected override List<ICell> CalculateChainIterative(ICell startCell, int maxIterations, double delta, WorkbookDataContext context) {
			if (!service.OnBeginCircularReferencesCalculation())
				return new List<ICell>();
			List<ICell> result = base.CalculateChainIterative(startCell, maxIterations, delta, context);
			List<Spreadsheet.CellKey> nativeCellKeys = new List<Spreadsheet.CellKey>();
			foreach (ICell cell in result) {
				nativeCellKeys.Add(new Spreadsheet.CellKey(cell.Key));
			}
			service.OnEndCircularReferencesCalculation(nativeCellKeys);
			return result;
		}
	}
	#endregion
	#region ChainRangeIterativeCalculator
	public class ChainRangeIterativeCalculator : ChainRangeCalculator {
		public ChainRangeIterativeCalculator(DocumentModel documentModel, CellsChain chain, CellRangeBase range)
			: base(documentModel, chain, range) {
		}
		protected override void DoCalculate() {
			WorkbookDataContext threadContext = DataContext;
			try {
				ICell previousCell = null;
				foreach (ICellBase cellBase in Range.GetExistingCellsEnumerable()) {
					CurrentCell = cellBase as ICell;
					if (CurrentCell == null || !CurrentCell.HasFormula)
						continue;
					CellsChain.UnRegisterCell(CurrentCell);
					if (previousCell == null)
						CellsChain.RegisterCell(CurrentCell);
					else
						CellsChain.InsertAfter(previousCell, CurrentCell);
					CalculateCellCore(CurrentCell, threadContext);
					CurrentCell.Worksheet.WebRanges.AddChangedCellPosition(CurrentCell);
					previousCell = CurrentCell;
				}
			}
			finally {
				Calculating = false;
			}
		}
		public override bool IsCalculated(ICell cell) {
			if (FormulaFactory.GetFormulaCalculateAlways(cell))
				FormulaFactory.SetFormulaCalculateAlways(CurrentCell, true);
			return true;
		}
	}
	#endregion
	#region PendingCellsCache
	public class PendingCellsCache {
		Dictionary<ICell, ICalculationWaiter> innerList;
		public PendingCellsCache() {
			innerList = new Dictionary<ICell, ICalculationWaiter>();
		}
		public int Count { get { return innerList.Count; } }
		public void RegisterPendingWaiter(ICell cell, ICalculationWaiter waiter) {
			ICalculationWaiter existingPrecedents = null;
			if (innerList.TryGetValue(cell, out existingPrecedents)) {
				if (existingPrecedents.AllowsMerging)
					existingPrecedents.MergeWith(waiter);
				else
					innerList[cell] = new CalculationWaitersList(existingPrecedents, waiter);
			}
			else
				innerList.Add(cell, waiter);
		}
		public void PopPendingCells(ICell calculatedCell, CellsChain chain) {
			ICalculationWaiter pendingPrecedent;
			if (innerList.TryGetValue(calculatedCell, out pendingPrecedent)) {
				pendingPrecedent.TryInsertInto(chain, calculatedCell);
				innerList.Remove(calculatedCell);
			}
		}
		public void PushPengingCellsToTheEndOfChain(CellsChain cellsChain) {
			foreach (ICalculationWaiter precedent in innerList.Values)
				precedent.AddToTheEndAndMarkUp(cellsChain);
		}
	}
	#endregion
	#region ICalculationWaiter
	public interface ICalculationWaiter {
		bool TryInsertInto(CellsChain where, ICell position);
		void AddToTheEndAndMarkUp(CellsChain where);
		bool AllowsMerging { get; }
		void MergeWith(ICalculationWaiter waiter);
	}
	#endregion
	#region CalculationWaitersList
	public class CalculationWaitersList : ICalculationWaiter {
		List<ICalculationWaiter> innerList;
		public CalculationWaitersList() {
			innerList = new List<ICalculationWaiter>();
		}
		public CalculationWaitersList(ICalculationWaiter item1, ICalculationWaiter item2)
			: this() {
			innerList.Add(item1);
			innerList.Add(item2);
		}
		internal List<ICalculationWaiter> InnerList { get { return innerList; } }
		#region ICalculationWaiter Members
		public bool AllowsMerging { get { return true; } }
		public void MergeWith(ICalculationWaiter item) {
			innerList.Add(item);
		}
		public bool TryInsertInto(CellsChain where, ICell position) {
			bool result = true;
			int i = innerList.Count - 1;
			while (i >= 0) {
				if (innerList[i].TryInsertInto(where, position))
					innerList.RemoveAt(i);
				else
					result = false;
				i--;
			}
			return result;
		}
		public void AddToTheEndAndMarkUp(CellsChain where) {
			for (int i = 0; i < innerList.Count; i++)
				innerList[i].AddToTheEndAndMarkUp(where);
		}
		#endregion
	}
	#endregion
	#region CalculationFewCellsWaiter
	public class CalculationFewCellsWaiter : ICalculationWaiter {
		readonly ICalculationWaiter innerWaiter;
		int cellsCounter = 0;
		public CalculationFewCellsWaiter(ICalculationWaiter innerWaiter) {
			Guard.ArgumentNotNull(innerWaiter, "innerWaiter");
			this.innerWaiter = innerWaiter;
		}
		public int Counter { get { return cellsCounter; } set { cellsCounter = value; } }
		#region ICalculationWaiter Members
		public bool TryInsertInto(CellsChain where, ICell position) {
			cellsCounter--;
			if (cellsCounter == 0)
				return innerWaiter.TryInsertInto(where, position);
			return false;
		}
		public bool AllowsMerging { get { return false; } }
		public void MergeWith(ICalculationWaiter waiter) {
			throw new InvalidOperationException("Merging is not supported for this class");
		}
		public void AddToTheEndAndMarkUp(CellsChain where) {
			cellsCounter--;
			if (cellsCounter == 0)
				innerWaiter.AddToTheEndAndMarkUp(where);
		}
		#endregion
	}
	#endregion
}
