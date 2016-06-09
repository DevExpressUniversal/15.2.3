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
using System.Diagnostics;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.Office.History;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model.CopyOperation {
	public class CopyCellOperation : CopyEverythingBase {
		ICell sourceCell;
		ICell targetCell;
		public CopyCellOperation(ModelPasteSpecialFlags pasteSpecialFlags, ICell sourceCell, ICell targetCell)
			: base(new SourceTargetRangesForCopy(sourceCell, targetCell), pasteSpecialFlags) {
			Guard.ArgumentNotNull(sourceCell, "sourceCell");
			Guard.ArgumentNotNull(targetCell, "targetCell");
			this.sourceCell = sourceCell;
			this.targetCell = targetCell;
		}
		public ICell TargetCell { [System.Diagnostics.DebuggerStepThrough] get { return this.targetCell; } set { this.targetCell = value; } }
		public ICell SourceCell { [System.Diagnostics.DebuggerStepThrough] get { return this.sourceCell; } set { this.sourceCell = value; } }
		protected internal override bool Validate() {
			return base.Validate();
		}
		protected internal override void ExecuteCore() {
			RangeCopyInfo pairOfCells = this.RangesInfo.First;
			CellRange targetRange = pairOfCells.TargetBigRange;
			if (pairOfCells.PasteSingleCellToMergedRange)
				this.PasteSpecialOptions.InnerFlags = ModelPasteSpecialFlags.Values;
			CopyCellContent(SourceCell, TargetCell, targetRange);
			CopyContent(pairOfCells);
			LocateArrayFormulaToCells();
		}
		public override CellPositionOffset GetTargetRangeOffset(CellRange currentTargetRange) {
			CellPositionOffset offset = SourceCell.GetRange().GetCellPositionOffset(TargetCell);
			return offset;
		}
		protected override IEnumerable<string> GetUsedDefinedNames() {
			if (SourceCell == null || SourceCell is FakeCell || !SourceCell.HasFormula)
				return new List<String>();
			return base.GetUsedDefinedNames();
		}
		protected override void CopyArrayFormulaList(RangeCopyInfo rangeInfo) {
		}
		protected override IEnumerable<CellRange> GetInnerSourceArrayFormulaRanges() {
			throw new InvalidOperationException();
		}
		protected override void CopySharedFormulaList(RangeCopyInfo rangeInfo) {
		}
		protected override void ProcessSourceSharedFormula(RangeCopyInfo rangeInfo, SharedFormula sourceSharedFormula, int existingSharedFormulaIndex, CellRange sourceSharedFormulaRange) {
			throw new InvalidOperationException();
		}
		protected internal override void ProcessSourceArrayFormula(RangeCopyInfo rangeInfo, ArrayFormula sourceArrayFormula, CellRange sourceArrayFormulaRange) {
			throw new InvalidOperationException();
		}
		protected override IDictionary<int, SharedFormula> GetInnerSharedFormulas(CellRange range, Predicate<CellRange> match) {
			Dictionary<int, SharedFormula> result = new Dictionary<int, SharedFormula>();
			Debug.Assert(range.CellCount == 1);
			ICell cell = range.GetFirstCellUnsafe() as ICell;
			if (cell == null || !cell.HasFormula)
				return result;
			SharedFormulaRef sourceSharedFormula = cell.GetFormula() as SharedFormulaRef;
			if (sourceSharedFormula == null)
				return result;
			SharedFormula hostSF = sourceSharedFormula.HostSharedFormula;
			if(match(hostSF.Range))
				result.Add(sourceSharedFormula.HostFormulaIndex, sourceSharedFormula.HostSharedFormula);
			return result;
		}
		protected override bool CopyCellSharedFormulaCore(ICell source, FormulaBase formula, ICell target) {
			return CreateTargetCellSharedFormula(source, formula, target, target.GetRange());
		}
		bool CreateTargetCellSharedFormula(ICell source, FormulaBase formula, ICell target, CellRange _currentTargetRange) {
			SharedFormulaRef sharedFormulaRef = formula as SharedFormulaRef;
			SharedFormula sourceSharedFormula = sharedFormulaRef.HostSharedFormula;
			CellRange sourceSharedFormulaRange = sourceSharedFormula.Range;
			CellRange targetObjectRange = GetTargetObjectRange(source.GetRange(), _currentTargetRange) as CellRange; 
			bool sourceIntersectsCurrentTargetRange = IsSourceObjectRangeInterserctsWithCurrentTargetRange(sourceSharedFormulaRange, _currentTargetRange);
			bool targetRangeHasSameWorksheet = Object.ReferenceEquals(sourceSharedFormulaRange.Worksheet, TargetWorksheet);
			if (targetRangeHasSameWorksheet && sourceIntersectsCurrentTargetRange) {
				SharedFormula targetSharedFormula = sourceSharedFormula;
				TargetWorksheet.CreateSharedFormulaRefTransacted(target, sharedFormulaRef.HostFormulaIndex, targetSharedFormula);
				return true;
			}
			ParsedExpression modifiedExpression = ConvertParsedExpressionWithShiftReferencesOrNull(formula.Expression, new CellPositionOffset(0,0));
			if (modifiedExpression == null)
				return false;
			SharedFormula sharedFormula = new SharedFormula(modifiedExpression, targetObjectRange);
			int sharedFormulaIndex = TargetWorksheet.SharedFormulas.Add(sharedFormula);
			foreach (ICell cell in targetObjectRange.GetAllCellsEnumerable())
				TargetWorksheet.CreateSharedFormulaRefTransacted(cell, sharedFormulaIndex, sharedFormula);
			return true;
		}
		protected override void CopyCellArrayFormulaCore(CellRange sourceCellRange, FormulaBase formula, ICell target, CellRange currentTargetRange) {
			ArrayFormula arrayFormula = formula as ArrayFormula;
			ArrayFormulaPart arrayFormulaPart = formula as ArrayFormulaPart;
			CellRange topLeftCellArrayFormulaRange = null;
			if (arrayFormula != null)
				topLeftCellArrayFormulaRange = sourceCellRange;
			else if (arrayFormulaPart != null)
				topLeftCellArrayFormulaRange = arrayFormulaPart.TopLeftCell.GetRange();
			if (topLeftCellArrayFormulaRange == null)
				return; 
			CellPositionOffset finalArrayFormulaOffset = GetArrayFormulaExpressionReferencesOffset(topLeftCellArrayFormulaRange, currentTargetRange);
			ParsedExpression modifiedExpression = ConvertParsedExpressionWithShiftReferencesOrNull(formula.Expression, finalArrayFormulaOffset); 
			if (modifiedExpression == null)
				DevExpress.Office.Utils.Exceptions.ThrowInternalException();
			CellRange targetObjectRange = GetTargetObjectRange(sourceCellRange, currentTargetRange) as CellRange;
			ArrayFormula targetArrayFormula = new ArrayFormula(targetObjectRange, modifiedExpression);
			InsertWorksheetArrayFormulaCommand.ExecuteWithoutChecks(targetArrayFormula, targetObjectRange);
		}
		protected internal override void CopyRows(RangeCopyInfo rangeCopyInfo) {
		}
		protected override IEnumerable<Column> GetSourceColumns() {
			if (!PasteSpecialOptions.ShouldCopyColumnWidths)
				return new List<Column>();
			return new List<Column>() { TargetCell.Worksheet.Columns.GetColumnRangeForReading(SourceCell.ColumnIndex) };
		}
		protected override Column CreateTargetColumn(Column sourceColumn) {
			if (PasteSpecialOptions.ShouldCopyColumnWidths && sourceColumn.IsCustomWidth)
				return TargetCell.Worksheet.Columns.GetIsolatedColumn(TargetCell.ColumnIndex);
			else
				return null;
		}
		protected override IEnumerable<CellRange> GetMergedCellRanges() {
			return new List<CellRange>();
		}
		protected override void ProcessSourceMergedRange(RangeCopyInfo rangeInfo, CellRange sourceMergedRange) {
		}
		#region Hyperlinks
		protected override List<ModelHyperlink> GetSourceHyperlinks() {
			List<ModelHyperlink> result = new List<ModelHyperlink>();
			ModelHyperlinkCollection sourceHyperlinkCollection = SourceWorksheet.Hyperlinks;
			int sourceHyperlinkIndex = sourceHyperlinkCollection.GetHyperlink(SourceCell);
			if (sourceHyperlinkIndex < 0)
				return result;
			result.Add(sourceHyperlinkCollection[sourceHyperlinkIndex]);
			return result;
		}
		public override CellRangeBase GetTargetObjectRange(CellRangeBase sourceObjectRange, CellRange currentTargetRange) {
			Debug.Assert(sourceObjectRange.ContainsCell(SourceCell));
			return TargetCell.GetRange();
		}
		protected override bool IsSourceObjectRangeInterserctsWithCurrentTargetRange(CellRangeBase sourceObjectRange, CellRange currentTargetRange) {
			if (!Object.ReferenceEquals(TargetCell.Worksheet, sourceObjectRange.Worksheet))
				return false;
			return sourceObjectRange.Intersects(TargetCell.GetRange());
		}
		#endregion
		protected internal override IEnumerable<Table> GetSourceTables(bool includeTablesIntersectedSourceRange) {
			return new List<Table>();
		}
		protected internal override List<Table> GetSourceTablesFromIntersectionSourceTargetRange(CellRange intersection){
			return new List<Table>();
		}
		protected override void ProcessSourceTable(RangeCopyInfo rangeCopyInfo, Table sourceTable, List<string> existingTableNames) {
		}
		protected internal override IEnumerable<PivotTable> GetSourcePivotTables(CellRange sourceRange, bool includeTablesIntersectedSourceRange) {
			return new List<PivotTable>();
		}
		protected override void ProcessSourcePivotTable(RangeCopyInfo rangeCopyInfo, PivotTable sourcePivotTable, IList<string> existingNames) {
		}
		protected override IEnumerable<IDrawingObject> GetDrawingsForCopy() {
			return SourceWorksheet.DrawingObjectsByZOrderCollections.GetDrawingObjects(SourceCell.GetRange());
		}
		protected override IEnumerable<Comment> GetCommentsForCopy() {
			List<Comment> result = new List<Comment>();
			foreach (var comment in SourceWorksheet.Comments.InnerList) {
				if (comment.Reference.EqualsPosition(SourceCell.Position)) {
					result.Add(comment);
					return result;
				}
			}
			return result;
		}
		protected override IEnumerable<ConditionalFormatting> GetConditionalFormattingsForCopy(Worksheet worksheet) {
			return worksheet.ConditionalFormattings.GetConditionalFormattings(SourceCell.Key);
		}
		protected override IEnumerable<ModelProtectedRange> GetSourceProtectedRanges() {
			return SourceWorksheet.ProtectedRanges.LookupProtectedRanges(this.SourceCell.Position);
		}
		protected override void ProcessSourceSparklineGroup(RangeCopyInfo rangeInfo, SparklineGroup sourceSparklineGroup) {
		}
		protected override IEnumerable<SparklineGroup> GetSourceSparklineGroups() {
			return new List<SparklineGroup>(); 
		}
		protected override List<DataValidation> GetSourceDataValidations() {
			return SourceWorksheet.DataValidations.LookupDataValidations(this.SourceCell.Position);
		}
		protected override void BeforeBeginTargetDocumentModelUpdate() {
		}
		protected override void AfterBeginTargetDocumentModelUpdate() {
		}
		protected override void BeforeEndTargetDocumentModelUpdate() {
			bool changeValuesExpected = PasteSpecialOptions.ShouldCopyValues || PasteSpecialOptions.ShouldCopyFormulas;
			if (!changeValuesExpected)
				return;
			TableColumnNamesChangeOperation tableColumnNamesCorrectedOperation = new TableColumnNamesChangeOperation();
			tableColumnNamesCorrectedOperation.ShouldCorrectColumnFormulas = true;
			tableColumnNamesCorrectedOperation.ShouldUpdateColumnCells = true;
			Table targetTable = TargetWorksheet.Tables.TryGetItem(targetCell.Position);
			if (tableColumnNamesCorrectedOperation.Init(targetTable, targetCell, new Dictionary<string, string>()))
				tableColumnNamesCorrectedOperation.Execute();
		}
		protected override void AfterEndTargetDocumentModelUpdate() {
		}
	}
}
