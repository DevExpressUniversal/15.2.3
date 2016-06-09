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

using System.Diagnostics;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model.External;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Services;
using DevExpress.XtraSpreadsheet.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
namespace DevExpress.XtraSpreadsheet.Model {
	#region CalculationChain
	public class CalculationChain : IDisposable {
		readonly DocumentModel workbook;
		CalculationHash calculationHash;
		bool enabled;
		CellsChain cellsChain;
		ICalculationLogic calculationLogic;
		public CalculationChain(DocumentModel workbook) {
			Guard.ArgumentNotNull(workbook, "workbook");
			this.workbook = workbook;
			this.enabled = true;
			this.cellsChain = new CellsChain();
			this.calculationHash = new CalculationHash(workbook);
			InitCalculationLogic(enabled);
		}
		void InitCalculationLogic(bool enabled) {
			if (calculationLogic != null)
				calculationLogic.Dispose();
			if (enabled)
				calculationLogic = new ChainBaisedCalculationLogic(this);
			else
				calculationLogic = new RecursiveCalculationLogic(this);
		}
		public ICalculationLogic CalculationLogic { get { return calculationLogic; } }
		public CellsChain CellsChain { get { return cellsChain; } }
		internal DocumentModel Workbook { get { return workbook; } }
		public bool Enabled { get { return enabled; } set { SetEnabled(value); } }
		public bool Calculating { get { return calculationLogic.Calculating; } set { calculationLogic.Calculating = value; } }
		public CalculationHash CalculationHash { get { return calculationHash; } }
		#region Enable/Disable
		void SetEnabled(bool value) {
			if (enabled == value)
				return;
			enabled = value;
			InitCalculationLogic(enabled);
			if (value)
				Enable();
			else
				Disable();
		}
		void Enable() {
			Regenerate(true);
		}
		void Disable() {
			this.cellsChain.Reset();
		}
		#endregion
		public void Reset() {
			this.calculationLogic.Reset();
			this.cellsChain.Reset();
		}
		protected internal void Regenerate(bool rebuildChain) {
			calculationLogic.Regenerate(rebuildChain);
		}
		#region ICell notifications
		public void OnBeforeCellValueReplacedByFormula(ICell cell) {
		}
		public void OnAfterCellValueReplacedByFormula(ICell cell) {
			calculationLogic.AddCellFormula(cell);
			MarkupDependentsForRecalculation(cell);
		}
		public void OnBeforeCellFormulaChanged(ICell cell) {
			RemoveCellFormula(cell);
		}
		public void OnAfterCellFormulaChanged(ICell cell) {
			calculationLogic.AddCellFormula(cell);
			MarkupDependentsForRecalculation(cell);
		}
		public void OnBeforeCellFormulaReplacedByValue(ICell cell) {
			RemoveCellFormula(cell);
		}
		public void OnAfterCellFormulaReplacedByValue(ICell cell) {
			MarkupDependentsForRecalculation(cell);
		}
		public void OnCellValueChanged(ICell cell) {
		}
		public void OnColumnInserted(int columnIndex) {
		}
		public void OnColumnRemove(int columnIndex) {
		}
		public void OnBeforeCellRemove(ICell cell) {
			cell.OnBeforeRemove();
			cell.Worksheet.OnCellRemoved(cell);
			if (enabled)
				cellsChain.UnRegisterCell(cell);
		}
		public void OnAfterCellInsert(int innerIndex, ICell cell) {
			cell.OnAfterInsert();
			cell.Worksheet.SetCachedCell(cell, innerIndex);
			if (enabled && cell.HasFormula)
				cellsChain.RegisterCell(cell);
		}
		public void OnBeforeRowRangeRemoving(List<Row> rows) {
			if (!enabled)
				return;
			foreach (Row row in rows) {
				foreach (ICell cell in row.Cells)
					if (cell.HasFormula)
						cellsChain.UnRegisterCell(cell);
			}
		}
		public void OnAfterRowRangeInserting(List<Row> rows) {
			if (!enabled)
				return;
			foreach (Row row in rows) {
				foreach (ICell cell in row.Cells)
					if (cell.HasFormula)
						cellsChain.RegisterCell(cell);
			}
		}
		public void OnAfterCellValueChanged(ICell cell) {
			MarkupDependentsForRecalculation(cell);
		}
		public void OnBeforeSharedFormulaRemoved(SharedFormula sharedFormula) {
			calculationLogic.RemoveSharedFormula(sharedFormula);
		}
		public void OnAfterSharedFormulaAdded(SharedFormula sharedFormula) {
			calculationLogic.AddSharedFormula(sharedFormula);
		}
		public void OnBeforeSharedFormulaChanged(SharedFormula sharedFormula) {
			calculationLogic.RemoveSharedFormula(sharedFormula);
		}
		public void OnAfterSharedFormulaChanged(SharedFormula sharedFormula) {
			calculationLogic.AddSharedFormula(sharedFormula);
		}
		public void OnAfterRangeInserting(int sheetId, CellRangeBase cellRange, InsertCellMode mode) {
			MarkUpWorksheetForRecalculation(sheetId);
			workbook.DefinedNamesRefencesCacheManager.Invalidate();
		}
		public void OnAfterRangeRemoving(int sheetId, CellRangeBase cellRange, RemoveCellMode mode) {
			if (mode == RemoveCellMode.Default || mode == RemoveCellMode.NoShiftOrRangeToPasteCutRange)
				MarkupDependentsForRecalculation(cellRange);
			else
				MarkUpWorksheetForRecalculation(sheetId);
			if (mode != RemoveCellMode.Default)
				workbook.DefinedNamesRefencesCacheManager.Invalidate();
		}
		public void OnBeforeSheetRemoving(Worksheet sheet) {
			MarkupDependentsForRecalculation(CellIntervalRange.CreateColumnInterval(sheet, 0, PositionType.Absolute, IndicesChecker.MaxColumnCount - 1, PositionType.Absolute));
			foreach (ICellBase cellBase in sheet.GetExistingCells()) {
				ICell cell = cellBase as ICell;
				if (cell != null && cell.HasFormula)
					UnRegisterCell(cell);
			}
		}
		public void OnAfterSheetRemoving(Worksheet sheet) {
			workbook.DefinedNamesRefencesCacheManager.Invalidate();
		}
		public void OnBeforeSheetInserting(Worksheet sheet) {
		}
		public void OnAfterSheetInserting(Worksheet sheet) {
			workbook.DefinedNamesRefencesCacheManager.Invalidate();
		}
		public void OnAfterCollapsedExpandedOutlineGroup(CellRangeBase range) {
			calculationLogic.MarkupDependentsForRecalculation(range);
		}
		#endregion
		#region MarkupForRecalculation
		void MarkUpWorksheetForRecalculation(int sheetId) {
			Worksheet sheet = workbook.Sheets.GetById(sheetId);
			CellRange sheetRange = new CellRange(sheet, new CellPosition(0, 0), new CellPosition(sheet.MaxColumnCount - 1, sheet.MaxRowCount - 1));
			MarkupDependentsForRecalculation(sheetRange);
		}
		public void MarkupDependentsForRecalculation(ICell cell) {
			calculationLogic.MarkupDependentsForRecalculation(cell);
		}
		public void MarkupDependentsForRecalculationForWholeCellFormula(ICell cell) {
			CellRange formulaRange = null;
			if (cell.HasFormula) {
				if (cell.FormulaType == FormulaType.Array)
					formulaRange = ((ArrayFormula)cell.Formula).Range;
				else if (cell.FormulaType == FormulaType.ArrayPart) {
					ArrayFormula arrayFormula = ((ArrayFormulaPart)cell.Formula).TopLeftCell.Formula as ArrayFormula;
					if (arrayFormula != null)
						formulaRange = arrayFormula.Range;
				}
				else if (cell.FormulaType == FormulaType.Shared)
					formulaRange = ((SharedFormulaRef)cell.Formula).HostSharedFormula.Range;
			}
			if (formulaRange != null)
				workbook.CalculationChain.MarkupDependentsForRecalculation(formulaRange);
			else
				workbook.CalculationChain.MarkupDependentsForRecalculation(cell);
		}
		public void MarkupDependentsForRecalculation(CellRangeBase range) {
			calculationLogic.MarkupDependentsForRecalculation(range);
		}
		public void MarkupDependentsForRecalculationWithHistory(CellRangeBase range) {
			MarkupDependentsForRecalculationHistoryItem historyItem = new MarkupDependentsForRecalculationHistoryItem(Workbook, range);
			workbook.History.Add(historyItem);
			historyItem.Execute();
		}
		#endregion
		#region Check Integrity
		public void CheckIntegrity() {
			if (this != Workbook.CalculationChain)
				IntegrityChecks.Fail("CalculationChain: this != Workbook.CalculationChain");
			this.cellsChain.CheckIntegrity();
		}
		public void CheckCellKey(CellKey key) {
			IWorksheet workSheet = CheckCellKeySheetId(key);
			if (workSheet == null)
				return;
			Row row = CheckCellKeyRowIndex(workSheet, key);
			if (row == null)
				return;
			CheckCellKeyColumnIndex(row, key);
		}
		public IWorksheet CheckCellKeySheetId(CellKey key) {
			int sheetId = key.SheetId;
			IWorksheet workSheet = workbook.GetSheetById(sheetId);
			if (workSheet == null)
				IntegrityChecks.Fail(String.Format("CellKey refers to non-existing sheet. SheetId={0}", sheetId));
			return workSheet;
		}
		public Row CheckCellKeyRowIndex(IWorksheet workSheet, CellKey key) {
			int rowIndex = key.RowIndex;
			List<Row> rows = new List<Row>();
			foreach (Row row in workSheet.Rows.GetExistingRows(rowIndex, rowIndex, false))
				rows.Add(row);
			if (rows.Count != 1) {
				IntegrityChecks.Fail(String.Format("CellKey refers to non-existing row. rowIndex={0}, rows.Count={1}, key.RowIndex={2}, key.ColumnIndex={3}", rowIndex, rows.Count, key.RowIndex, key.ColumnIndex));
				return null;
			}
			else
				return rows[0];
		}
		public ICell CheckCellKeyColumnIndex(Row row, CellKey key) {
			int columnIndex = key.ColumnIndex;
			List<ICell> cells = new List<ICell>();
			foreach (ICell cell in row.Cells.GetExistingCells(columnIndex, columnIndex, false))
				cells.Add(cell);
			if (cells.Count != 1) {
				IntegrityChecks.Fail(String.Format("CellKey refers to non-existing cell. rowIndex={0}, columnIndex={1}", row.Index, columnIndex));
				return null;
			}
			else
				return cells[0];
		}
		#endregion
		#region GetDependents
		public CellRangeBase GetDependents(CellKey key, bool includeMergedRanges) {
			return calculationLogic.GetDependents(key, includeMergedRanges);
		}
		public CellRangeBase GetDependents(CellRangeBase range, bool includeMergedRanges) {
			return calculationLogic.GetDependents(range, includeMergedRanges);
		}
		public CellRangeBase GetDirectDependents(CellKey cellKey, bool includeMergedRanges) {
			return calculationLogic.GetDirectDependents(cellKey, includeMergedRanges);
		}
		public CellRangeBase GetDirectDependents(CellRangeBase range, bool includeMergedRanges) {
			return calculationLogic.GetDirectDependents(range, includeMergedRanges);
		}
		#endregion
		#region GetPrecedents
		public CellRangeBase GetPrecedents(ICell cell, bool includeMergedRanges) {
			return calculationLogic.GetPrecedents(cell, includeMergedRanges);
		}
		public CellRangeBase GetPrecedents(CellRangeBase range, bool includeMergedRanges) {
			return calculationLogic.GetPrecedents(range, includeMergedRanges);
		}
		public CellRangeBase GetDirectPrecedents(ICell cell, bool includeMergedRanges) {
			return calculationLogic.GetDirectPrecedents(cell, includeMergedRanges);
		}
		public CellRangeBase GetDirectPrecedents(CellRangeBase range, bool includeMergedRanges) {
			return calculationLogic.GetDirectPrecedents(range, includeMergedRanges);
		}
		#endregion
		#region RemoveCell
		public void RemoveCellFormula(ICell cell) {
			if (!cell.HasFormula)
				return;
			UnregisterRtdFunctions(cell);
			calculationLogic.RemoveCellFormula(cell);
		}
		void UnregisterRtdFunctions(ICell cell) {
			workbook.RealTimeDataManager.UnregisterCell(cell);
		}
		public void UnRegisterCell(ICell cell) {
			cellsChain.UnRegisterCell(cell);
		}
		#endregion
		#region AddCellFormula
		public void RegisterCell(ICell cell) {
			cellsChain.RegisterCell(cell);
		}
		#endregion
		public void ForceCalculate() {
			calculationLogic.ForceCalculate();
		}
		public void CalculateWorkbookIfHasMarkedCells() {
			calculationLogic.CalculateWorkbookIfHasMarkedCells();
		}
		public void CalculateWorkbook() {
			calculationLogic.CalculateWorkbook();
		}
		public void CalculateWorksheet(Worksheet sheet) {
			calculationLogic.CalculateWorksheet(sheet);
		}
		public void CalculateRange(CellRangeBase range) {
			calculationLogic.CalculateRange(range);
		}
		public void CalculateRangeIfHasMarkedCells(CellRangeBase range) {
			calculationLogic.CalculateRangeIfHasMarkedCells(range);
		}
		public void ForceFullCalculation() {
			calculationLogic.ForceFullCalculation();
		}
		public void ForceFullCalculationRebuild() {
			calculationLogic.ForceFullCalculationRebuild();
		}
		public VariantValue CalculateCell(ICell cell) {
			System.Diagnostics.Debug.Assert(cell.HasFormula);
			return calculationLogic.CalculateCell(cell);
		}
		public bool TryGetCalculatedValue(ICellBase cell, out VariantValue calculatedValue) {
			return calculationLogic.TryGetCalculatedValue(cell, out calculatedValue);
		}
		#region IDisposable Members
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		void Dispose(bool disposing) {
			if (disposing) {
				calculationLogic.Dispose();
				CalculationHash.Dispose();
			}
		}
		#endregion
#if DEBUGTEST
		protected internal void PrepareCalculation(ICell currentCell) {
			calculationLogic.PrepareCalculation(currentCell);
		}
#endif
		public void SetUncalculatedState() {
			calculationLogic.SetUncalculatedState();
		}
	}
	#endregion
	#region ICalculationLogic
	public interface ICalculationLogic : IDisposable {
		bool Calculating { get; set; }
		void ForceCalculate();
		void Reset();
		void Regenerate(bool rebuildChain);
		void CalculateWorkbook();
		void CalculateWorkbookIfHasMarkedCells();
		void CalculateWorksheet(Worksheet sheet);
		void CalculateRange(CellRangeBase range);
		void CalculateRangeIfHasMarkedCells(CellRangeBase range);
		void ForceFullCalculation();
		void ForceFullCalculationRebuild();
		VariantValue CalculateCell(ICell cell);
		bool TryGetCalculatedValue(ICellBase cell, out VariantValue calculatedValue);
		void MarkupDependentsForRecalculation(ICell cell);
		void MarkupDependentsForRecalculation(CellRangeBase range);
		#region Dependents
		CellRangeBase GetDependents(CellKey cell, bool includeMergedRanges);
		CellRangeBase GetDependents(CellRangeBase range, bool includeMergedRanges);
		CellRangeBase GetDirectDependents(CellKey cellKey, bool includeMergedRanges);
		CellRangeBase GetDirectDependents(CellRangeBase range, bool includeMergedRanges);
		#endregion
		#region Precedents
		CellRangeBase GetPrecedents(ICell cell, bool includeMergedRanges);
		CellRangeBase GetPrecedents(CellRangeBase range, bool includeMergedRanges);
		CellRangeBase GetDirectPrecedents(ICell cell, bool includeMergedRanges);
		CellRangeBase GetDirectPrecedents(CellRangeBase range, bool includeMergedRanges);
		#endregion
		void RemoveCellFormula(ICell cell);
		void RemoveSharedFormula(SharedFormula sharedFormula);
		void AddCellFormula(ICell cell);
		void AddSharedFormula(SharedFormula sharedFormula);
		void SetUncalculatedState();
#if DEBUGTEST
		void PrepareCalculation(ICell currentCell);
#endif
	}
	#endregion
	#region ChainBaisedCalculationLogic
	public class ChainBaisedCalculationLogic : ICalculationLogic {
		readonly CalculationChain chain;
		ChainCalculator wholeWorkbookCalculator;
		IChainCalculator calculator;
		CellDependentsTreeBuilder dependeciesCalculator;
		Dictionary<int, CellPrecedentsRTree> precedentsTrees;
		bool hasMarkedCells;
		public ChainBaisedCalculationLogic(CalculationChain chain) {
			this.chain = chain;
			this.dependeciesCalculator = new CellDependentsTreeBuilder(DocumentModel, this);
			this.wholeWorkbookCalculator = new ChainCalculator(DocumentModel, chain.CellsChain);
			this.calculator = wholeWorkbookCalculator;
			this.precedentsTrees = new Dictionary<int, CellPrecedentsRTree>();
			hasMarkedCells = false;
			LinkHandlers();
		}
		void LinkHandlers() {
			DocumentModel.ContentVersionChanged += workbook_ContentVersionChanged;
		}
		void UnLinkHandlers() {
			DocumentModel.ContentVersionChanged -= workbook_ContentVersionChanged;
		}
		#region Properties
		public Dictionary<int, CellPrecedentsRTree> PrecedentsTrees { get { return precedentsTrees; } }
		public CalculationChain Chain { get { return chain; } }
		public DocumentModel DocumentModel { get { return chain.Workbook; } }
		#endregion
		#region ICalculationLogic members
		public bool Calculating { get { return calculator.Calculating; } set { calculator.Calculating = value; } }
		public void SetUncalculatedState() {
			this.hasMarkedCells = true;
		}
		public void ForceCalculate() {
			MarkupWorkbookForRecalculation();
			DocumentModel.IncrementContentVersion();
		}
		public void MarkupWorkbookForRecalculation() {
			ForceRecalculationCellsWalker walker = new ForceRecalculationCellsWalker();
			walker.Walk(DocumentModel);
			hasMarkedCells = true;
		}
		public void Reset() {
			this.wholeWorkbookCalculator.Reset();
			this.calculator = wholeWorkbookCalculator;
			this.precedentsTrees = new Dictionary<int, CellPrecedentsRTree>();
		}
		public void Regenerate(bool rebuildChain) {
			Reset();
			if (rebuildChain)
				chain.CellsChain.Reset();
			dependeciesCalculator.CalculateChain(rebuildChain);
		}
		public void CalculateWorkbookIfHasMarkedCells() {
			if (hasMarkedCells)
				CalculateWorkbookCore(DocumentModel.Properties.CalculationOptions.CalculationMode);
		}
		public void CalculateWorkbook() {
			DocumentModel.IncrementContentVersionCore();
			CalculateWorkbookCore(ModelCalculationMode.Automatic);
			ForceChartsRecalculation();
		}
		void ForceChartsRecalculation() {
			ChartsUpdateDataWalker walker = new ChartsUpdateDataWalker();
			foreach (Worksheet sheet in DocumentModel.Sheets)
				sheet.DrawingObjects.ForEach(delegate(IDrawingObject drawingObject) { drawingObject.Visit(walker); });
		}
		public void CalculateWorkbookCore(ModelCalculationMode calculationMode) {
			ICustomCalculationService service = DocumentModel.GetService<ICustomCalculationService>();
			if (service != null)
				PerformCustomizedCalculation(service, calculationMode);
			else
				CalculateCore(calculationMode);
			DocumentModel.IncrementTransactionVersion();
			hasMarkedCells = false;
		}
		void PerformCustomizedCalculation(ICustomCalculationService service, ModelCalculationMode calculationMode) {
			ChainCustomCalculator currentCalculator = new ChainCustomCalculator(DocumentModel, chain.CellsChain, service);
			calculator = currentCalculator;
			try {
				CalculateCore(calculationMode);
			}
			finally {
				calculator = wholeWorkbookCalculator;
			}
		}
		void CalculateCore(ModelCalculationMode calculationMode) {
			calculator.Calculate(calculationMode);
		}
		public void CalculateWorksheet(Worksheet sheet) {
			CalculateRange(CellIntervalRange.CreateAllWorksheetRange(sheet));
		}
		public void CalculateRange(CellRangeBase range) {
			DocumentModel.IncrementContentVersionCore();
			ChainRangeCalculator currentCalculator = ChainRangeCalculator.GetInstance(DocumentModel, chain.CellsChain, range);
			calculator = currentCalculator;
			try {
				calculator.Calculate(DocumentModel.Properties.CalculationOptions.CalculationMode);
			}
			finally {
				calculator = wholeWorkbookCalculator;
			}
			DocumentModel.IncrementTransactionVersion();
			ForceChartsRecalculation();
		}
		public void CalculateRangeIfHasMarkedCells(CellRangeBase range) {
			if (hasMarkedCells)
				CalculateRange(range);
		}
		public void ForceFullCalculation() {
			MarkupWorkbookForRecalculation();
			CalculateWorkbook();
		}
		public void ForceFullCalculationRebuild() {
			Regenerate(true);
			ForceFullCalculation();
		}
		public VariantValue CalculateCell(ICell cell) {
			return wholeWorkbookCalculator.CalculateCellCore(cell, cell.Context);
		}
		public bool TryGetCalculatedValue(ICellBase cell, out VariantValue calculatedValue) {
			ICell icell = cell as ICell;
			if (icell == null) {
				calculatedValue = cell.Value;
				return true;
			}
			return calculator.TryGetCalculatedValue(icell, out calculatedValue);
		}
		#region MarkupDependentsForRecalculation
		public void MarkupDependentsForRecalculation(ICell cell) {
			MarkupDependentsForRecalculationCore(cell);
			DocumentModel.IncrementContentVersion();
		}
		public void MarkupDependentsForRecalculation(CellRangeBase range) {
			MarkupDependentsForRecalculationCore(range);
			DocumentModel.IncrementContentVersion();
		}
		void MarkupDependentsForRecalculationCore(ICell cell) {
			Queue<CellPrecedentsHolder> precedentHolders = new Queue<CellPrecedentsHolder>();
			cell.MarkUpForRecalculation();
			AddDirectChainDependentsToQueue(cell, precedentHolders);
			MarkupQueue(precedentHolders);
		}
		void MarkupDependentsForRecalculationCore(CellRangeBase range) {
			Queue<CellPrecedentsHolder> precedentHolders = new Queue<CellPrecedentsHolder>();
			foreach (ICellBase cellBase in range.GetExistingCellsEnumerable()) {
				ICell cell = cellBase as ICell;
				if (cell != null)
					cell.MarkUpForRecalculation();
			}
			AddDirectChainDependentsToQueue(range, precedentHolders);
			MarkupQueue(precedentHolders);
		}
		void MarkupQueue(Queue<CellPrecedentsHolder> precedentHolders) {
			while (precedentHolders.Count > 0) {
				CellPrecedentsHolder currentCellPrecedentsHolder = precedentHolders.Dequeue();
				IList<IChainPrecedent> dependents = currentCellPrecedentsHolder.Precedents;
				ISheetPosition currentAffectedRange = currentCellPrecedentsHolder.AffectedRange;
				for (int currentChainDependent = 0; currentChainDependent < dependents.Count; currentChainDependent++) {
					List<ICell> dependentAffectedCells = new List<ICell>();
					dependents[currentChainDependent].AddItemsTo(dependentAffectedCells, currentAffectedRange);
					for (int i = 0; i < dependentAffectedCells.Count; i++) {
						ICell processingCell = dependentAffectedCells[i];
						if (!processingCell.MarkedForRecalculation) {
							AddDirectChainDependentsToQueue(processingCell, precedentHolders);
							processingCell.MarkUpForRecalculation();
						}
					}
				}
			}
			hasMarkedCells = true;
		}
		void AddDirectChainDependentsToQueue(ICell cell, Queue<CellPrecedentsHolder> where) {
			IList<IChainPrecedent> list = GetCellDependentRanges(cell.Key);
			if (list != null && list.Count > 0)
				where.Enqueue(new CellPrecedentsHolder(cell, list));
		}
		void AddDirectChainDependentsToQueue(CellRangeBase range, Queue<CellPrecedentsHolder> where) {
			foreach (CellRange innerRange in range.GetAreasEnumerable()) {
				IList<IChainPrecedent> list = GetCellDependentRanges(innerRange);
				if (list != null && list.Count > 0)
					where.Enqueue(new CellPrecedentsHolder(innerRange, list));
			}
		}
		#endregion
		#region Dependents
		#region GetDirectDependents
		public CellRangeBase GetDirectDependents(CellKey cellKey, bool includeMergedRanges) {
			IList<IChainPrecedent> precedents = GetCellDependentRanges(cellKey);
			return GetDirectDependentsCore(precedents, cellKey, includeMergedRanges);
		}
		public CellRangeBase GetDirectDependents(CellRangeBase range, bool includeMergedRanges) {
			CellRangeBase result = null;
			foreach (CellRange innerRange in range.GetAreasEnumerable()) {
				CellPrecedentsRTree sheetTree = null;
				if (!PrecedentsTrees.TryGetValue(innerRange.SheetId, out sheetTree))
					continue;
				IList<IChainPrecedent> currentList = sheetTree.Search(innerRange);
				CellRangeBase currentRange = ConvertPrecedentsToRange(currentList, innerRange);
				result = CellRange.MergeRanges(result, currentRange);
			}
			if (includeMergedRanges && result != null)
				result.EnlargeByMergedRanges();
			return result;
		}
		CellRangeBase GetDirectDependentsCore(IList<IChainPrecedent> precedents, ISheetPosition affectedRange, bool includeMergedRanges) {
			CellRangeBase result = ConvertPrecedentsToRange(precedents, affectedRange);
			if (result != null && includeMergedRanges)
				result.EnlargeByMergedRanges();
			return result;
		}
		CellRangeBase ConvertPrecedentsToRange(IList<IChainPrecedent> precedents, ISheetPosition affectedRange) {
			CellRangeBase result = null;
			if (precedents != null)
				for (int i = 0; i < precedents.Count; i++) {
					CellRangeBase rangePrecedentRange = precedents[i].ToRange(affectedRange);
					result = CellRangeBase.MergeRanges(result, rangePrecedentRange);
				}
			return result;
		}
		#endregion
		public CellRangeBase GetDependents(CellKey key, bool includeMergedRanges) {
			CellRangeBase directPrecedents = GetDirectDependents(key, false);
			return GetDependentsCore(directPrecedents);
		}
		public CellRangeBase GetDependents(CellRangeBase range, bool includeMergedRanges) {
			CellRangeBase directDependents = GetDirectDependents(range, false);
			if (directDependents == null)
				return null;
			return GetDependentsCore(directDependents);
		}
		CellRangeBase GetDependentsCore(CellRangeBase directPrecedents) {
			if (directPrecedents == null)
				return null;
			CellRangeBase result = directPrecedents;
			List<CellRangeBase> newDependents = new List<CellRangeBase>();
			newDependents.Add(directPrecedents);
			int i = 0;
			while (i < newDependents.Count) {
				CellRangeBase currentDependent = newDependents[i];
				foreach (CellRange innerRange in currentDependent.GetAreasEnumerable()) {
					CellRangeBase processingPrecedents = GetDirectDependents(innerRange, false);
					if (processingPrecedents != null) {
						CellRangeBase newPrecedentRange = processingPrecedents.ExcludeRange(result);
						if (newPrecedentRange != null) {
							newDependents.Add(newPrecedentRange);
							result = result.MergeWithRange(processingPrecedents);
						}
					}
				}
				i++;
			}
			return result;
		}
		#endregion
		#region Precedents
		public CellRangeBase GetPrecedents(ICell cell, bool includeMergedRanges) {
			CellRangeBase result = GetDirectPrecedents(cell, includeMergedRanges);
			return GetPrecedentsCore(result, includeMergedRanges);
		}
		public CellRangeBase GetPrecedents(CellRangeBase range, bool includeMergedRanges) {
			CellRangeBase result = GetDirectPrecedents(range, includeMergedRanges);
			return GetPrecedentsCore(result, includeMergedRanges);
		}
		public CellRangeBase GetDirectPrecedents(CellRangeBase range, bool includeMergedRanges) {
			CellRangeBase result = null;
			foreach (ICellBase cellBase in range.GetExistingCellsEnumerable()) {
				ICell cell = cellBase as ICell;
				CellRangeBase precedents = GetDirectPrecedents(cell, includeMergedRanges);
				if (precedents != null)
					result = result != null ? result.MergeWithRange(precedents) : precedents;
			}
			return result;
		}
		public CellRangeBase GetDirectPrecedents(ICell cell, bool includeMergedRanges) {
			if (!cell.HasFormula)
				return null;
			List<CellRangeBase> ranges = cell.Formula.GetPrecedents(cell);
			if (ranges == null || ranges.Count <= 0)
				return null;
			if (includeMergedRanges)
				for (int i = 0; i < ranges.Count; i++)
					ranges[i].EnlargeByMergedRanges();
			CellRangeBase result = ranges[0];
			for (int i = 1; i < ranges.Count; i++)
				result = result.MergeWithRange(ranges[i]);
			return result;
		}
		CellRangeBase GetPrecedentsCore(CellRangeBase range, bool includeMergedRanges) {
			if (range == null)
				return null;
			CellRangeBase result = range;
			for (; ; ) {
				CellRangeBase currentDependecies = GetDirectPrecedents(range, includeMergedRanges);
				if (currentDependecies == null || result.Includes(currentDependecies))
					return result;
				range = currentDependecies;
				result = result.MergeWithRange(currentDependecies);
			}
		}
		#endregion
		#region Add/Remove formula
		public void RemoveCellFormula(ICell cell) {
			dependeciesCalculator.RemoveCellFormula(cell);
		}
		public void RemoveSharedFormula(SharedFormula sharedFormula) {
			dependeciesCalculator.RemoveSharedFormula(sharedFormula);
		}
		public void AddCellFormula(ICell cell) {
			Debug.Assert(cell.HasFormula);
			dependeciesCalculator.ProcessCell(cell);
		}
		public void AddSharedFormula(SharedFormula sharedFormula) {
			dependeciesCalculator.ProcessSharedFormula(sharedFormula);
		}
		#endregion
		#endregion
		void workbook_ContentVersionChanged(object sender, EventArgs e) {
			if (DocumentModel.SuppressAutoStartCalculation)
				return;
			if (DocumentModel.Properties.CalculationOptions.CalculationMode != ModelCalculationMode.Manual)
				CalculateWorkbookIfHasMarkedCells();
		}
		#region GetCellDependentRanges
		public IList<IChainPrecedent> GetCellDependentRanges(CellKey key) {
			CellPrecedentsRTree sheetTree = null;
			if (!PrecedentsTrees.TryGetValue(key.SheetId, out sheetTree))
				return null;
			return sheetTree.Search(key);
		}
		internal IList<IChainPrecedent> GetCellDependentRanges(CellRangeBase range) {
			List<IChainPrecedent> result = new List<IChainPrecedent>();
			foreach (CellRange innerRange in range.GetAreasEnumerable()) {
				CellPrecedentsRTree sheetTree = null;
				if (!PrecedentsTrees.TryGetValue(innerRange.SheetId, out sheetTree))
					return null;
				result.AddRange(sheetTree.Search(innerRange));
			}
			return result;
		}
		#endregion
		#region Dispose
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				UnLinkHandlers();
				dependeciesCalculator = null;
				calculator = null;
				wholeWorkbookCalculator = null;
				precedentsTrees = null;
			}
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion
#if DEBUGTEST
		public void PrepareCalculation(ICell currentCell) {
			wholeWorkbookCalculator.PrepareCalculation(currentCell);
		}
#endif
	}
	#endregion
	#region RecursiveCalculationLogic
	public class RecursiveCalculationLogic : ICalculationLogic {
		readonly CalculationChain chain;
		public RecursiveCalculationLogic(CalculationChain chain) {
			this.chain = chain;
		}
		#region ICalculationLogic members
		public bool Calculating { get { return false; } set { } }
		public void ForceCalculate() {
			chain.Workbook.IncrementContentVersion();
		}
		public void Reset() {
		}
		public void Regenerate(bool rebuildChain) {
		}
		public void CalculateWorkbookIfHasMarkedCells() {
			CalculateWorkbook();
		}
		public void CalculateWorkbook() {
			foreach (Worksheet sheet in chain.Workbook.Sheets)
				CalculateWorksheetCore(sheet);
		}
		public void CalculateWorksheet(Worksheet sheet) {
			CalculateWorksheetCore(sheet);
		}
		public void CalculateRange(CellRangeBase range) {
			foreach (ICellBase cellBase in range.GetExistingCellsEnumerable()) {
				ICell cell = cellBase as ICell;
				if (cell != null)
					CalculateCellWithMarkupingVolatiles(cell);
			}
		}
		public void CalculateRangeIfHasMarkedCells(CellRangeBase range) {
			CalculateRange(range);
		}
		public void ForceFullCalculation() {
			chain.Workbook.IncrementContentVersion();
			CalculateWorkbook();
		}
		public void ForceFullCalculationRebuild() {
			ForceFullCalculation();
		}
		public VariantValue CalculateCell(ICell cell) {
			return cell.CalculateFormulaValue();
		}
		public bool TryGetCalculatedValue(ICellBase cell, out VariantValue calculatedValue) {
			calculatedValue = cell.Value;
			return true;
		}
		public void MarkupDependentsForRecalculation(ICell cell) {
			chain.Workbook.IncrementContentVersion();
		}
		public void MarkupDependentsForRecalculation(CellRangeBase range) {
			chain.Workbook.IncrementContentVersion();
		}
		#region Dependents
		public CellRangeBase GetDirectDependents(CellKey cellKey, bool includeMergedRanges) {
			return null;
		}
		public CellRangeBase GetDirectDependents(CellRangeBase range, bool includeMergedRanges) {
			return null;
		}
		public CellRangeBase GetDependents(CellKey key, bool includeMergedRanges) {
			return null;
		}
		public CellRangeBase GetDependents(CellRangeBase range, bool includeMergedRanges) {
			return null;
		}
		#endregion
		#region Precedents
		public CellRangeBase GetPrecedents(ICell cell, bool includeMergedRanges) {
			return null;
		}
		public CellRangeBase GetPrecedents(CellRangeBase range, bool includeMergedRanges) {
			return null;
		}
		public CellRangeBase GetDirectPrecedents(ICell cell, bool includeMergedRanges) {
			return null;
		}
		public CellRangeBase GetDirectPrecedents(CellRangeBase range, bool includeMergedRanges) {
			return null;
		}
		#endregion
		public void RemoveCellFormula(ICell cell) {
		}
		public void RemoveSharedFormula(SharedFormula sharedFormula) {
		}
		public void AddCellFormula(ICell cell) {
		}
		public void AddSharedFormula(SharedFormula sharedFormula) {
		}
		#endregion
		void CalculateWorksheetCore(Worksheet sheet) {
			foreach (ICellBase cellBase in sheet.GetExistingCells()) {
				ICell cell = cellBase as ICell;
				if (cell != null)
					CalculateCellWithMarkupingVolatiles(cell);
			}
		}
		void CalculateCellWithMarkupingVolatiles(ICell cell) {
			if (cell.HasFormula) {
				if (FormulaFactory.GetFormulaCalculateAlways(cell) || cell.Formula.IsVolatile())
					cell.MarkUpForRecalculation();
				cell.CalculateFormulaValue();
			}
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
			}
		}
		public void Dispose() {
			Dispose(true);
		}
#if DEBUGTEST
		public void PrepareCalculation(ICell currentCell) {
		}
#endif
		#region ICalculationLogic Members
		public void SetUncalculatedState() {
		}
		#endregion
	}
	#endregion
	#region CalculationHash
	public class CalculationHash : IDisposable {
		readonly DocumentModel documentModel;
		readonly List<LookupCalculationInfo> calculatedHashes;
		readonly Dictionary<string, VariantValue> textToNumericConversions;
		readonly Dictionary<string, VariantValue> textToVariantValueConversions;
		CultureInfo textConversionsCulture;
		CultureInfo textToVariantValueCulture;
		public List<LookupCalculationInfo> CalculatedHashes { get { return calculatedHashes; } }
		public CalculationHash(DocumentModel documentModel) {
			this.documentModel = documentModel;
			calculatedHashes = new List<LookupCalculationInfo>();
			textToNumericConversions = new Dictionary<string, VariantValue>();
			textToVariantValueConversions = new Dictionary<string, VariantValue>();
			LinkHandlers();
			textConversionsCulture = documentModel.Culture;
		}
		void LinkHandlers() {
			documentModel.ContentVersionChanged += workbook_ContentVersionChanged;
		}
		void UnLinkHandlers() {
			documentModel.ContentVersionChanged -= workbook_ContentVersionChanged;
		}
		void workbook_ContentVersionChanged(object sender, EventArgs e) {
			ClearHashes();
		}
		public void ClearHashes() {
			CalculatedHashes.Clear();
			textToNumericConversions.Clear();
			textToVariantValueConversions.Clear();
		}
		#region LookupCalculation
		public LookupCalculationInfo GetLookupCalculationInfo(IVector<VariantValue> values) {
			foreach (LookupCalculationInfo info in CalculatedHashes) {
				if (info.Values.Equals(values)) {
					return info;
				}
			}
			LookupCalculationInfo newLookupCalculationInfo = new LookupCalculationInfo(values, documentModel.DataContext);
			CalculatedHashes.Add(newLookupCalculationInfo);
			return newLookupCalculationInfo;
		}
		#endregion
		#region TextToNumericConversion
		public VariantValue ConvertTextToNumeric(string text, CultureInfo culture) {
			if (textConversionsCulture != culture) {
				textToNumericConversions.Clear();
				textConversionsCulture = culture;
			}
			VariantValue result;
			if (!textToNumericConversions.TryGetValue(text, out result)) {
				result = VariantValue.ConvertStringToDouble(text, documentModel.DataContext);
				textToNumericConversions.Add(text, result);
			}
			return result;
		}
		#endregion
		#region ConvertTextToVariantValue
		public VariantValue ConvertTextToVariantValue(string text, CultureInfo culture) {
			if (textToVariantValueCulture != culture) {
				textToVariantValueConversions.Clear();
				textToVariantValueCulture = culture;
			}
			VariantValue result;
			if (!textToVariantValueConversions.TryGetValue(text, out result)) {
				result = CellValueFormatter.GetValue(VariantValue.Empty, text, documentModel.DataContext, false).Value;
				textToVariantValueConversions.Add(text, result);
			}
			return result;
		}
		#endregion
		#region Dispose
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				UnLinkHandlers();
			}
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion
	}
	#endregion
}
