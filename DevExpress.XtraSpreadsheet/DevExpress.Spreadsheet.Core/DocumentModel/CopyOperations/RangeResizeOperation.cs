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
using System.Globalization;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Localization;
namespace DevExpress.XtraSpreadsheet.Model.CopyOperation {
	public class RangeResizeOperation : ErrorHandledWorksheetCommand {
		CellRange sourceRange;
		CellRange targetRange;
		CellRange targetRangeExceptSource;
		IList<ConditionalFormatting> conditionalFormattings;
		int sharedFormulaIndex;
		SharedFormula sharedFormula;
		Sequence sourceCellSequence;
		PresetData sourceCellPreset;
		List<Sequence> numericHelperList;
		Dictionary<CellPosition, PresetData> presetsCollection;
		Dictionary<CellKey, CellContentSnapshot> affectedCells = new Dictionary<CellKey, CellContentSnapshot>();
		List<CellRange> filteredRanges = null;
		public Dictionary<CellKey, CellContentSnapshot> AffectedCells { get { return affectedCells; } }
		public RangeResizeOperation(Worksheet worksheet, CellRange source, CellRange target, IErrorHandler errorHandler)
			: base(worksheet, errorHandler) {
			Guard.ArgumentNotNull(source, "sourceRange");
			Guard.ArgumentNotNull(target, "targetRange");
			if (!source.TopLeft.EqualsPosition(target.TopLeft) && !source.BottomRight.EqualsPosition(target.BottomRight))
				throw new ArgumentOutOfRangeException("target", "You can move only one corner of range.");
			sourceRange = source;
			targetRange = target;
		}
		bool HiddenByFilter(int rowIndex) {
			return Worksheet.IsRowFiltered(rowIndex, this.filteredRanges) && !Worksheet.IsRowVisible(rowIndex);
		}
		protected internal override void ExecuteCore() {
			GetFilteredRanges();
			affectedCells.Clear();
			if (this.targetRangeExceptSource != null) {
				foreach (ICellBase cell in targetRangeExceptSource.GetExistingCellsEnumerable()) {
					ICell targetCell = cell as ICell;
					if (targetCell != null && !targetCell.Value.IsEmpty && !HiddenByFilter(targetCell.RowIndex))
						affectedCells[targetCell.Key] = new CellContentSnapshot(targetCell);
				}
				Worksheet.ClearAll(targetRangeExceptSource, ModelPasteSpecialFlags.All, ErrorHandler, false, true, !HasFilteredRanges());
			}
			int conditionalFormattingsCount = Worksheet.ConditionalFormattings.Count;
			presetsCollection = GatherPresetDataFromExistingCells();
			if (sourceRange.BottomRight.Row == targetRange.BottomRight.Row && sourceRange.TopLeft.Row == targetRange.TopLeft.Row) {
				numericHelperList = CreateNumericHelpersFromHorizontalRange();
				FillHorizontalRange();
			}
			else {
				numericHelperList = CreateNumericHelpersFromVerticalRange();
				FillVerticalRange();
			}
			while (Worksheet.ConditionalFormattings.Count > conditionalFormattingsCount)
				Worksheet.ConditionalFormattings.RemoveAt(0);
		}
		void GetFilteredRanges() {
			CellRange range = this.sourceRange.Expand(this.targetRange);
			this.filteredRanges = Worksheet.GetFilteredRanges(range);
		}
		bool HasFilteredRanges() {
			return this.filteredRanges != null && this.filteredRanges.Count > 0;
		}
		protected internal override bool Validate() {
			targetRangeExceptSource = targetRange.ExcludeRange(sourceRange) as CellRange;
			if (targetRangeExceptSource == null)
				return true;
			IModelErrorInfo error;
			error = CheckMergedCells();
			if (error != null)
				return HandleError(error);
			error = CheckIntersectsArrayFormula();
			if (error != null)
				return HandleError(error);
			error = CheckTableIntersection();
			return HandleError(error);
		}
		IModelErrorInfo CheckIntersectsArrayFormula() {
			List<CellRange> intersectedArrayFomulaRanges = Worksheet.ArrayFormulaRanges.GetArrayFormulaRangesIntersectsRange(targetRangeExceptSource);
			if (intersectedArrayFomulaRanges.Count != 0)
				return new ModelErrorInfo(ModelErrorType.ErrorChangingPartOfAnArray);
			return null;
		}
		IModelErrorInfo CheckMergedCells() {
			List<CellRange> intersectedMergedCellRanges = Worksheet.MergedCells.GetMergedCellRangesIntersectsRange(targetRangeExceptSource);
			int maxMergedCellWidth = 1;
			int maxMergedCellHeight = 1;
			List<CellRange> sourseMergedCellRanges = Worksheet.MergedCells.GetMergedCellRangesIntersectsRange(sourceRange);
			foreach (CellRange mergedCell in sourseMergedCellRanges) {
				if (maxMergedCellWidth < mergedCell.Width)
					maxMergedCellWidth = mergedCell.Width;
				if (maxMergedCellHeight < mergedCell.Height)
					maxMergedCellHeight = mergedCell.Height;
			}
			foreach (CellRange mergedCell in intersectedMergedCellRanges) {
				if (mergedCell.Height != maxMergedCellHeight || mergedCell.Width != maxMergedCellWidth)
					return new ModelErrorInfo(ModelErrorType.MergedCellMustBeIdenticallySized);
			}
			List<CellRange> exceptIncludedRanges = new List<CellRange>();
			foreach (CellRange mergedCells in intersectedMergedCellRanges) {
				if (!targetRangeExceptSource.Includes(mergedCells))
					exceptIncludedRanges.Add(mergedCells);
			}
			if (exceptIncludedRanges.Count != 0)
				return new ModelErrorInfo(ModelErrorType.MergedCellCanNotBeChanged);
			return null;
		}
		IModelErrorInfo CheckTableIntersection() {
			CellRange expandedCellRange = sourceRange.Expand(targetRange);
			List<Table> tables = Worksheet.Tables.GetItems(expandedCellRange, true);
			for (int i = 0; i < tables.Count; ++i) {
				Table table = tables[i];
				CellRange headerRowRange = table.TryGetHeadersRowRange();
				if (!expandedCellRange.Intersects(headerRowRange)) {
					CellRange tableRange = table.Range;
					CellRange intersection = tableRange.Intersection(expandedCellRange);
					if (intersection == null || intersection.CellCount == expandedCellRange.CellCount) {
						tables.RemoveAt(i); 
						continue;
					}
				}
				return new ModelErrorInfo(ModelErrorType.ErrorRangeContainsTable);
			}
			return null;
		}
		bool IsReverseSelection() {
			return sourceRange.BottomRight.Column == targetRange.BottomRight.Column && sourceRange.BottomRight.Row == targetRange.BottomRight.Row;
		}
		void FillHorizontalRange() {
			int sourceRangeWidth = sourceRange.Width;
			int targetRangeWidth = targetRange.Width;
			int sourceCellRepeatsCount = (int)Math.Ceiling((double)targetRangeWidth / sourceRangeWidth) - 1;
			if (sourceCellRepeatsCount <= 0) {
				ClearHorizontalRange();
				return;
			}
			foreach (Sequence sequence in numericHelperList)
				sequence.GenerateValues(sourceCellRepeatsCount);
			if (IsReverseSelection())
				FillHorizontalRangeCoreReverse(sourceRangeWidth, targetRangeWidth);
			else
				FillHorizontalRangeCore(sourceRangeWidth, targetRangeWidth);
		}
		void ClearHorizontalRange() {
			for (int currentRow = sourceRange.TopLeft.Row; currentRow <= sourceRange.BottomRight.Row; ++currentRow)
				for (int currentColumn = sourceRange.BottomRight.Column; currentColumn > targetRange.BottomRight.Column; --currentColumn)
					CleanTheCell(currentColumn, currentRow);
		}
		void FillHorizontalRangeCore(int sourceRangeWidth, int targetRangeWidth) {
			for (int currentRow = sourceRange.TopLeft.Row; currentRow <= sourceRange.BottomRight.Row; currentRow++) {
				if (HiddenByFilter(currentRow))
					continue;
				for (int sourceCellColumn = sourceRange.TopLeft.Column; sourceCellColumn <= sourceRange.BottomRight.Column; ++sourceCellColumn) {
					ICell sourceCell = Worksheet.TryGetCell(sourceCellColumn, currentRow);
					GenerateSourceCellInformation(sourceCell, sourceRangeWidth, targetRangeWidth, sourceCellColumn - sourceRange.TopLeft.Column, currentRow, GenerateSharedFormulaRangeHorizontal);
					int sourceCellIndex = sourceCellColumn - sourceRange.TopLeft.Column;
					int sourceCellRepeatIndex = 0;
					for (int currentColumn = sourceCellColumn + sourceRangeWidth; currentColumn <= targetRange.BottomRight.Column; currentColumn += sourceRangeWidth) {
						CellPosition targetCellPosition = new CellPosition(currentColumn, currentRow);
						FillCell(sourceCell, targetCellPosition, sourceCellRepeatIndex, sourceCellIndex);
						++sourceCellRepeatIndex;
					}
				}
			}
		}
		CellRange GenerateSharedFormulaRangeHorizontal(int from, int to, int currentRow) {
			return new CellRange(Worksheet, new CellPosition(sourceRange.TopLeft.Column + from, currentRow), new CellPosition(sourceRange.TopLeft.Column + to, currentRow));
		}
		void FillHorizontalRangeCoreReverse(int sourceRangeWidth, int targetRangeWidth) {
			for (int currentRow = sourceRange.TopLeft.Row; currentRow <= sourceRange.BottomRight.Row; currentRow++) {
				if (HiddenByFilter(currentRow))
					continue;
				for (int sourceCellColumn = sourceRange.BottomRight.Column; sourceCellColumn >= sourceRange.TopLeft.Column; --sourceCellColumn) {
					ICell sourceCell = Worksheet.TryGetCell(sourceCellColumn, currentRow);
					GenerateSourceCellInformation(sourceCell, sourceRangeWidth, targetRangeWidth, sourceRange.BottomRight.Column - sourceCellColumn, currentRow, GenerateSharedFormulaRangeHorizontalReverse);
					int sourceCellIndex = sourceRange.BottomRight.Column - sourceCellColumn;
					int sourceCellRepeatIndex = 0;
					for (int currentColumn = sourceCellColumn - sourceRangeWidth; currentColumn >= targetRange.TopLeft.Column; currentColumn -= sourceRangeWidth) {
						CellPosition targetCellPosition = new CellPosition(currentColumn, currentRow);
						FillCell(sourceCell, targetCellPosition, sourceCellRepeatIndex, sourceCellIndex);
						++sourceCellRepeatIndex;
					}
				}
			}
		}
		CellRange GenerateSharedFormulaRangeHorizontalReverse(int from, int to, int currentRow) {
			return new CellRange(Worksheet, new CellPosition(targetRange.TopLeft.Column + to, currentRow), new CellPosition(targetRange.TopLeft.Column + from, currentRow));
		}
		void FillVerticalRange() {
			int sourceRangeHeight = sourceRange.Height;
			int targetRangeHeight = targetRange.Height;
			int sourceCellRepeatsCount = (int)Math.Ceiling((double)targetRangeHeight / sourceRangeHeight) - 1;
			if (sourceCellRepeatsCount <= 0) {
				ClearVerticalRange();
				return;
			}
			if (HasFilteredRanges()) {
				if (IsReverseSelection())
					FillFilteredVerticalRangeCoreReverse(sourceRangeHeight, targetRangeHeight);
				else
					FillFilteredVerticalRangeCore(sourceRangeHeight, targetRangeHeight);
			}
			else {
				foreach (Sequence sequence in numericHelperList)
					sequence.GenerateValues(sourceCellRepeatsCount);
				if (IsReverseSelection())
					FillVerticalRangeCoreReverse(sourceRangeHeight, targetRangeHeight);
				else
					FillVerticalRangeCore(sourceRangeHeight, targetRangeHeight);
			}
		}
		void ClearVerticalRange() {
			for (int currentColumn = sourceRange.TopLeft.Column; currentColumn <= sourceRange.BottomRight.Column; ++currentColumn)
				for (int currentRow = sourceRange.BottomRight.Row; currentRow > targetRange.BottomRight.Row; --currentRow)
					CleanTheCell(currentColumn, currentRow);
		}
		void FillVerticalRangeCore(int sourceRangeHeight, int targetRangeHeight) {
			for (int currentColumn = sourceRange.TopLeft.Column; currentColumn <= sourceRange.BottomRight.Column; ++currentColumn)
				for (int sourceCellRow = sourceRange.TopLeft.Row; sourceCellRow <= sourceRange.BottomRight.Row; ++sourceCellRow) {
					ICell sourceCell = Worksheet.TryGetCell(currentColumn, sourceCellRow);
					GenerateSourceCellInformation(sourceCell, sourceRangeHeight, targetRangeHeight, sourceCellRow - sourceRange.TopLeft.Row, currentColumn, GenerateSharedFormulaRangeVertical);
					int sourceCellIndex = sourceCellRow - sourceRange.TopLeft.Row;
					int sourceCellRepeatIndex = 0;
					for (int currentRow = sourceCellRow + sourceRangeHeight; currentRow <= targetRange.BottomRight.Row; currentRow += sourceRangeHeight) {
						CellPosition targetCellPosition = new CellPosition(currentColumn, currentRow);
						FillCell(sourceCell, targetCellPosition, sourceCellRepeatIndex, sourceCellIndex);
						++sourceCellRepeatIndex;
					}
				}
		}
		void FillVerticalRangeCoreReverse(int sourceRangeHeight, int targetRangeHeight) {
			for (int currentColumn = sourceRange.TopLeft.Column; currentColumn <= sourceRange.BottomRight.Column; ++currentColumn)
				for (int sourceCellRow = sourceRange.BottomRight.Row; sourceCellRow >= sourceRange.TopLeft.Row; --sourceCellRow) {
					ICell sourceCell = Worksheet.TryGetCell(currentColumn, sourceCellRow);
					GenerateSourceCellInformation(sourceCell, sourceRangeHeight, targetRangeHeight, sourceRange.BottomRight.Row - sourceCellRow, currentColumn, GenerateSharedFormulaRangeVerticalReverse);
					int sourceCellIndex = sourceRange.BottomRight.Row - sourceCellRow;
					int sourceCellRepeatIndex = 0;
					for (int currentRow = sourceCellRow - sourceRangeHeight; currentRow >= targetRange.TopLeft.Row; currentRow -= sourceRangeHeight) {
						CellPosition targetCellPosition = new CellPosition(currentColumn, currentRow);
						FillCell(sourceCell, targetCellPosition, sourceCellRepeatIndex, sourceCellIndex);
						++sourceCellRepeatIndex;
					}
				}
		}
		void FillFilteredVerticalRangeCore(int sourceRangeHeight, int targetRangeHeight) {
			for (int currentColumn = sourceRange.TopLeft.Column; currentColumn <= sourceRange.BottomRight.Column; ++currentColumn) {
				int sourceCellRow = sourceRange.TopLeft.Row;
				ICell sourceCell = Worksheet.TryGetCell(currentColumn, sourceCellRow);
				GenerateSourceCellInformation(sourceCell, 1, targetRangeHeight, 0, currentColumn, GenerateSharedFormulaRangeVertical);
				int sourceCellIndex = 0;
				int sourceCellRepeatIndex = 0;
				for (int currentRow = sourceCellRow + 1; currentRow <= targetRange.BottomRight.Row; currentRow += 1) {
					if (HiddenByFilter(currentRow))
						continue;
					CellPosition targetCellPosition = new CellPosition(currentColumn, currentRow);
					FillCell(sourceCell, targetCellPosition, sourceCellRepeatIndex, sourceCellIndex);
					++sourceCellRepeatIndex;
				}
			}
		}
		void FillFilteredVerticalRangeCoreReverse(int sourceRangeHeight, int targetRangeHeight) {
			for (int currentColumn = sourceRange.TopLeft.Column; currentColumn <= sourceRange.BottomRight.Column; ++currentColumn) {
				int sourceCellRow = sourceRange.BottomRight.Row;
				ICell sourceCell = Worksheet.TryGetCell(currentColumn, sourceCellRow);
				GenerateSourceCellInformation(sourceCell, sourceRangeHeight, targetRangeHeight, sourceRange.BottomRight.Row - sourceCellRow, currentColumn, GenerateSharedFormulaRangeVerticalReverse);
				int sourceCellIndex = 0;
				int sourceCellRepeatIndex = 0;
				for (int currentRow = sourceCellRow - 1; currentRow >= targetRange.TopLeft.Row; currentRow -= 1) {
					if (HiddenByFilter(currentRow))
						continue;
					CellPosition targetCellPosition = new CellPosition(currentColumn, currentRow);
					FillCell(sourceCell, targetCellPosition, sourceCellRepeatIndex, sourceCellIndex);
					++sourceCellRepeatIndex;
				}
			}
		}
		CellRange GenerateSharedFormulaRangeVertical(int from, int to, int currentColumn) {
			return new CellRange(Worksheet, new CellPosition(currentColumn, sourceRange.TopLeft.Row + from), new CellPosition(currentColumn, sourceRange.TopLeft.Row + to));
		}
		CellRange GenerateSharedFormulaRangeVerticalReverse(int from, int to, int currentColumn) {
			return new CellRange(Worksheet, new CellPosition(currentColumn, targetRange.TopLeft.Row + to), new CellPosition(currentColumn, targetRange.TopLeft.Row + from));
		}
		void GenerateSourceCellInformation(ICell cell, int sourceRangeLength, int targetRangeLength, int indexInSourceRange, int currentIndex, Func<int, int, int, CellRange> generateSharedFormulaRange) {
			if (cell == null) {
				conditionalFormattings = null;
				sharedFormulaIndex = -1;
				sharedFormula = null;
				sourceCellSequence = null;
				sourceCellPreset = null;
				return;
			}
			conditionalFormattings = Worksheet.ConditionalFormattings.GetConditionalFormattings(cell.Key);
			GenerateSharedFormula(cell, sourceRangeLength, targetRangeLength, indexInSourceRange, currentIndex, generateSharedFormulaRange);
			sourceCellSequence = GetSequenceInformation(cell, sourceRangeLength);
			sourceCellPreset = GetPresetInformation(cell);
		}
		void GenerateSharedFormula(ICell cell, int sourceRangeLength, int targetRangeLength, int indexInSourceRange, int currentIndex, Func<int, int, int, CellRange> generateSharedFormulaRange) {
			if (cell.HasFormula) {
				CellRange sharedFormulaRange = GenerateSharedFormulaRange(targetRangeLength, sourceRangeLength, indexInSourceRange, currentIndex, generateSharedFormulaRange);
				FormulaBase sourceFormula = cell.Formula;
				if (sourceFormula.Type == FormulaType.Shared) {
					SharedFormulaRef sourceRef = sourceFormula as SharedFormulaRef;
					sharedFormula = sourceRef.HostSharedFormula;
					sharedFormulaIndex = sourceRef.HostFormulaIndex;
					sharedFormulaRange = sharedFormula.Range.Expand(sharedFormulaRange);
					sharedFormula.SetRange(sharedFormulaRange);
					return;
				}
				if (sourceFormula.Type != FormulaType.Array && sourceFormula.Type != FormulaType.ArrayPart) {
					sharedFormula = new SharedFormula(cell, cell.FormulaBody, sharedFormulaRange);
					SharedFormulaValidateRPNVisitor validator = new SharedFormulaValidateRPNVisitor(cell.GetRange());
					if (validator.Validate(sharedFormula.Expression)) {
						sharedFormulaIndex = Worksheet.SharedFormulas.Add(sharedFormula);
						Worksheet.CreateSharedFormulaRefTransacted(cell, sharedFormulaIndex, sharedFormula);
						return;
					}
				}
			}
			sharedFormula = null;
			sharedFormulaIndex = -1;
		}
		CellRange GenerateSharedFormulaRange(int targetRangeLength, int sourceRangeLength, int indexInSourceRange, int currentIndex, Func<int, int, int, CellRange> generateSharedFormulaRange) {
			int sourceCellRepeatsCount = (int)Math.Ceiling((double)(targetRangeLength - sourceRangeLength - indexInSourceRange) / sourceRangeLength);
			int to = sourceCellRepeatsCount * sourceRangeLength + indexInSourceRange;
			int from = indexInSourceRange;
			return generateSharedFormulaRange(from, to, currentIndex);
		}
		Sequence GetSequenceInformation(ICell cell, int sourceRangeLength) {
			if (!cell.HasFormula && cell.Value.IsNumeric && (sourceRangeLength != 1 || cell.ActualFormat.IsDateTime))
				foreach (Sequence helper in numericHelperList)
					if (helper.ContainsCell(cell))
						return helper;
			return null;
		}
		PresetData GetPresetInformation(ICell cell) {
			PresetData preset;
			presetsCollection.TryGetValue(cell.Position, out preset);
			return preset;
		}
		void CleanTheCell(int column, int row) {
			if(Worksheet.IsRowFiltered(row, this.filteredRanges) && !Worksheet.IsRowVisible(row))
				return;
			 DocumentModel.BeginUpdate();
			 try {
				 ICell targetCell = Worksheet.TryGetCell(column, row);
				 CleanTheCell(targetCell);
			 }
			 finally {
				 DocumentModel.EndUpdate();
			 }
		}
		void CleanTheCell(ICell targetCell) {
			if (targetCell == null)
				return;
			if (targetCell.HasNoValue)
				targetCell.TransactedSetValue(VariantValue.Empty);
			else {
				CellContentSnapshot snapshot = new CellContentSnapshot(targetCell);
				targetCell.TransactedSetValue(VariantValue.Empty);
				if (!affectedCells.ContainsKey(targetCell.Key))
					affectedCells.Add(targetCell.Key, snapshot);
			}
		}
		void FillCell(ICell sourceCell, CellPosition targetCellPosition, int sourceCellRepeatIndex, int sourceCellIndex) {
			ICell targetCell;
			if (sourceCell == null) {
				targetCell = Worksheet.TryGetCell(targetCellPosition.Column, targetCellPosition.Row);
				CleanTheCell(targetCell);
				return;
			}
			CellRange mergedSource = sourceCell.GetMergedCell();
			targetCell = Worksheet[targetCellPosition.Column, targetCellPosition.Row];
			MergeTargetCells(mergedSource, targetCell);
			CellContentSnapshot snapshot = new CellContentSnapshot(targetCell);
			if (sharedFormula != null) {
				CopyCell(sourceCell, targetCell, ModelPasteSpecialFlags.FormatAndStyle);
				SetSharedFormula(sourceCell, targetCell);
			}
			else {
				CopyCell(sourceCell, targetCell, ModelPasteSpecialFlags.All & ~ModelPasteSpecialFlags.Comments);
				if (sourceCellPreset != null)
					ChangeCellValue(targetCell, sourceCellPreset.NextValue);
				else if (sourceCellSequence != null) {
					double value = sourceCellSequence.GetValue(sourceCellRepeatIndex, sourceCellIndex);
					ChangeCellValue(targetCell, value);
				}
			}
			if (!affectedCells.ContainsKey(targetCell.Key))
				affectedCells.Add(targetCell.Key, snapshot);
			if (conditionalFormattings != null)
				foreach (ConditionalFormatting cf in conditionalFormattings) {
					CellRangeBase newCfRange = cf.CellRange.MergeWithRange(targetCell.GetRange());
					cf.SetCellRangeOrNull(newCfRange);
				}
		}
		void MergeTargetCells(CellRange mergedSource, ICell targetCell) {
			CellRange mergedTarget = targetCell.GetMergedCell();
			if (mergedSource != null && (mergedTarget == null || mergedTarget.Height != mergedSource.Height || mergedTarget.Width != mergedSource.Width)) {
				CellPosition mergedTargetTo = new CellPosition(targetCell.ColumnIndex + mergedSource.Width - 1, targetCell.RowIndex + mergedSource.Height - 1);
				CellRange rangeToMarge = new CellRange(Worksheet, targetCell.Position, mergedTargetTo);
				Worksheet.MergeCells(rangeToMarge, ErrorHandler);
			}
		}
		void SetSharedFormula(ICell sourceCell, ICell targetCell) {
			if (sharedFormulaIndex >= 0) {
				SharedFormulaValidateRPNVisitor validator = new SharedFormulaValidateRPNVisitor(targetCell.GetRange());
				if (validator.Validate(sharedFormula.Expression)) {
					Worksheet.CreateSharedFormulaRefTransacted(targetCell, sharedFormulaIndex, sharedFormula);
					return;
				}
			}
			SharedFormulaToNormalRPNVisitor converter = new SharedFormulaToNormalRPNVisitor(Worksheet.DataContext);
			DataContext.PushCurrentCell(targetCell);
			try {
				ParsedExpression convertedExpression = converter.Process(sharedFormula.Expression.Clone());
				targetCell.TransactedSetFormula(new Formula(targetCell, convertedExpression));
			}
			catch {
				DataContext.PopCurrentCell();
			}
			finally {
				sharedFormulaIndex = -1;
			}
		}
		void ChangeCellValue(ICell cell, VariantValue value) {
			cell.SetValue(value);
		}
		void CopyCell(ICell source, ICell target, ModelPasteSpecialFlags flags) {
			CopyCellOperation operation = new CopyCellOperation(flags, source, target);
			operation.SuppressChecks = true;
			operation.SuppressCellValueChanged = true;
			operation.Execute();
		}
		Dictionary<CellPosition, PresetData> GatherPresetDataFromExistingCells() {
			Dictionary<CellPosition, PresetData> result = new Dictionary<CellPosition, PresetData>();
			foreach (ICellBase existingCell in sourceRange.GetExistingCellsEnumerable(false)) {
				ICell sourceCell = existingCell as ICell;
				if (sourceCell == null)
					return result;
				PresetData preset = PresentDataCultureTable.Create(sourceCell.Text, DocumentModel.Culture);
				if (preset == null)
					return result;
				result.Add(sourceCell.Position, preset);
			}
			return result;
		}
		List<Sequence> CreateNumericHelpersFromHorizontalRange() {
			List<Sequence> result = new List<Sequence>();
			CellPosition firstCellInRange = CellPosition.InvalidValue;
			CellPosition lastCellInRange = CellPosition.InvalidValue;
			bool reverse = IsReverseSelection();
			for (int row = sourceRange.TopLeft.Row; row <= sourceRange.BottomRight.Row; ++row) {
				for (int column = sourceRange.TopLeft.Column; column <= sourceRange.BottomRight.Column; ++column) {
					ICell sourceCell = Worksheet.TryGetCell(column, row);
					if (sourceCell == null || sourceCell.HasFormula || !sourceCell.Value.IsNumeric) {
						AddNumericHelperFromHorizontalRange(firstCellInRange, lastCellInRange, reverse, result);
						firstCellInRange = CellPosition.InvalidValue;
					}
					else {
						lastCellInRange = new CellPosition(column, row);
						if (!firstCellInRange.IsValid)
							firstCellInRange = lastCellInRange;
					}
				}
				AddNumericHelperFromHorizontalRange(firstCellInRange, lastCellInRange, reverse, result);
				firstCellInRange = CellPosition.InvalidValue;
			}
			return result;
		}
		void AddNumericHelperFromHorizontalRange(CellPosition firstCell, CellPosition lastCell, bool reverse, List<Sequence> result) {
			if (firstCell.IsValid) {
				int sourceRangeLength = lastCell.Column - firstCell.Column + 1;
				if (sourceRangeLength == 1 && sourceRange.Height != 1)
					return;
				int sequenceStartIndex = reverse ? sourceRange.BottomRight.Column - lastCell.Column : firstCell.Column - sourceRange.TopLeft.Column;
				CellRange numericRange = new CellRange(Worksheet, firstCell, lastCell);
				Sequence sequence = new Sequence(numericRange, sourceRangeLength, sequenceStartIndex, reverse);
				result.Add(sequence);
			}
		}
		List<Sequence> CreateNumericHelpersFromVerticalRange() {
			List<Sequence> result = new List<Sequence>();
			CellPosition firstCellInRange = CellPosition.InvalidValue;
			CellPosition lastCellInRange = CellPosition.InvalidValue;
			bool reverse = IsReverseSelection();
			for (int column = sourceRange.TopLeft.Column; column <= sourceRange.BottomRight.Column; ++column) {
				for (int row = sourceRange.TopLeft.Row; row <= sourceRange.BottomRight.Row; ++row) {
					ICell sourceCell = Worksheet.TryGetCell(column, row);
					if (sourceCell == null || sourceCell.HasFormula || !sourceCell.Value.IsNumeric) {
						AddNumericHelperFromVerticalRange(firstCellInRange, lastCellInRange, reverse, result);
						firstCellInRange = CellPosition.InvalidValue;
					}
					else {
						lastCellInRange = new CellPosition(column, row);
						if (!firstCellInRange.IsValid)
							firstCellInRange = lastCellInRange;
					}
				}
				AddNumericHelperFromVerticalRange(firstCellInRange, lastCellInRange, reverse, result);
				firstCellInRange = CellPosition.InvalidValue;
			}
			return result;
		}
		void AddNumericHelperFromVerticalRange(CellPosition firstCell, CellPosition lastCell, bool reverse, List<Sequence> result) {
			if (firstCell.IsValid) {
				int sourceRangeLength = lastCell.Row - firstCell.Row + 1;
				if (sourceRangeLength == 1 && sourceRange.Width != 1)
					return;
				int sequenceStartIndex = reverse ? sourceRange.BottomRight.Row - lastCell.Row : firstCell.Row - sourceRange.TopLeft.Row;
				CellRange numericRange = new CellRange(Worksheet, firstCell, lastCell);
				Sequence sequence = new Sequence(numericRange, sourceRangeLength, sequenceStartIndex, reverse);
				result.Add(sequence);
			}
		}
	}
}
