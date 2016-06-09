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
using System.Diagnostics;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Localization;
using System.Collections;
using DevExpress.Office.History;
using DevExpress.Utils;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model.CopyOperation {
	public class CopyWorksheetOperation : CopyEverythingBase {
		bool generateNewTableName;
		HashSet<string> doNotCreateExternalLinksForThisSheets;
		public CopyWorksheetOperation(SourceTargetRangesForCopy ranges)
			: base(ranges, ModelPasteSpecialFlags.All) {
			this.generateNewTableName = true;
			this.doNotCreateExternalLinksForThisSheets = new HashSet<string>();
		}
		public CopyWorksheetOperation(CellRange sourceRange, CellRange  targetRange, ModelPasteSpecialFlags flags)
			:this(new SourceTargetRangesForCopy(sourceRange.Worksheet as Worksheet, targetRange.Worksheet as Worksheet)) {
		}
		public bool GenerateNewTableName { get { return generateNewTableName; } set { generateNewTableName = value; } }
		public override bool IsCopyWorksheetOperation { get { return true; } }
		#region ChecksIFEmpty
		public override IModelErrorInfo Checks() {
			if (SuppressChecks)
				return null;
			Debug.Assert(TargetWorksheet.Rows.Count == 0);
			Debug.Assert(TargetWorksheet.Columns.Count == 0);
			Debug.Assert(TargetWorksheet.MergedCells.Count == 0);
			Debug.Assert(TargetWorksheet.Hyperlinks.Count == 0);
			Debug.Assert(TargetWorksheet.DefinedNames.Count == 0);
			Debug.Assert(TargetWorksheet.Tables.Count == 0);
			Debug.Assert(TargetWorksheet.SharedFormulas.Count == 0);
			Debug.Assert(TargetWorksheet.ArrayFormulaRanges.Count == 0);
			Debug.Assert(TargetWorksheet.DrawingObjects.Count == 0);
			Debug.Assert(TargetWorksheet.DrawingObjects.Count == 0);
			Debug.Assert(TargetWorksheet.ColumnBreaks.Count == 0);
			Debug.Assert(TargetWorksheet.RowBreaks.Count == 0);
			Debug.Assert(TargetWorksheet.Comments.Count == 0);
			Debug.Assert(TargetWorksheet.ProtectedRanges.Count == 0);
			Debug.Assert(TargetWorksheet.DataValidations.Count == 0);
			Debug.Assert(TargetWorksheet.ConditionalFormattings.Count == 0);
			return base.Checks();
		}
		#endregion
		protected override void BeforeBeginTargetDocumentModelUpdate() {
		}
		protected override void AfterBeginTargetDocumentModelUpdate() {
		}
		protected override void BeforeEndTargetDocumentModelUpdate() {
		}
		protected override void AfterEndTargetDocumentModelUpdate() {
		}
		protected internal override void ExecuteCore() {
			RangeCopyInfo rangeInfo = this.RangesInfo.First;
			CopyContent(rangeInfo);
			CopyWorksheetParts();
			LocateArrayFormulaToCells();
		}
		protected override IEnumerable<DefinedNameBase> GetDefinedNamesToCopyWithChecking() {
			foreach (var item in SourceWorksheet.DefinedNames.InnerList)
				yield return item;
			if (!object.ReferenceEquals(WorkbookSource, WorkbookTarget)) {
				foreach (var item in WorkbookSource.DefinedNames.InnerList)
					yield return item;
			}
		}
		protected override void CopyDefinedNames(CellRange targetBigRange) {
			if (!PasteSpecialOptions.ShouldCopyFormulas
				&& !IsEqualWorksheets) {
				DefinedNameBase printArea;
				if (!SourceWorksheet.DefinedNames.TryGetItemByName(PrintAreaCalculator.PrintAreaDefinedName, out printArea))
					return;
				CopyDefinedNameCore(printArea as DefinedName, targetBigRange);
				return;
			}
			base.CopyDefinedNames(targetBigRange);
		}
		protected internal override void ApplyChanges() {
		}
		protected internal virtual void CopyWorksheetParts() {
			CopyWorksheetProperties();
			CopyColumnBreaks();
			CopyRowBreaks();
			CopyWorksheetViews();
		}
		public override CellPositionOffset GetTargetRangeOffset(CellRange currentTargetRange) {
			return CellPositionOffset.None;
		}
		internal void CopyWorksheetProperties() {
			TargetWorksheet.VisibleState = SourceWorksheet.VisibleState;
			TargetWorksheet.Properties.CopyFrom(SourceWorksheet.Properties);
		}
		protected override void CopyCellArrayFormulaCore(CellRange sourceRange, FormulaBase sourceFormula, ICell target, CellRange currentTargetRange) {
		}
		protected internal override void ProcessSourceArrayFormula(RangeCopyInfo rangeInfo, ArrayFormula sourceArrayFormula, CellRange sourceArrayFormulaRange) {
			(this as IRangeCopyArrayFormulaPart).CreateTargetArrayFormula(sourceArrayFormula, rangeInfo.TargetBigRange);
		}
		protected internal override CellPositionOffset GetArrayFormulaExpressionReferencesOffset(CellRange topLeftCellArrayFromulaRange, CellRange currentTargetRange) {
			return CellPositionOffset.None;
		}
		protected override IEnumerable<CellRange> GetInnerSourceArrayFormulaRanges() {
			return SourceWorksheet.ArrayFormulaRanges.InnerList;
		}
		protected override IDictionary<int, SharedFormula> GetInnerSharedFormulas(CellRange from, Predicate<CellRange> match) {
			return SourceWorksheet.SharedFormulas.InnerCollection;
		}
		protected override void ProcessSourceSharedFormula(RangeCopyInfo rangeInfo, SharedFormula sourceSharedFormula, int existingSharedFormulaIndex, CellRange sourceSharedFormulaRange) {
			CellRange targetAllWorksheetRange = rangeInfo.TargetBigRange;
			CellRange targetSharedFormulaRange = GetTargetObjectRange(sourceSharedFormulaRange, targetAllWorksheetRange) as CellRange;
			(this as IRangeCopySharedFormulaPart).CreateTargetSharedFormula(sourceSharedFormula, targetSharedFormulaRange, targetAllWorksheetRange);
		}
		protected override bool CopyCellSharedFormulaCore(ICell source, FormulaBase formula, ICell target) {
			SharedFormula sourceSharedFormula = (source.GetFormula() as SharedFormulaRef).HostSharedFormula;
			int targetSharedFormulaIndex = Int32.MinValue;
			if (!SourceSharedFormulaToTargetSharedFormulaIndexTranslationTable.TryGetValue(sourceSharedFormula, out targetSharedFormulaIndex))
				return false;
			SharedFormula targetSharedFormula = target.Worksheet.SharedFormulas[targetSharedFormulaIndex];
			target.Worksheet.CreateSharedFormulaRefTransacted(target, targetSharedFormulaIndex, targetSharedFormula);
			return true;
		}
		#region CopyRows
		protected override internal void CopyRows(RangeCopyInfo rangeCopyInfo) {
			var currentTargetRange = rangeCopyInfo.TargetBigRange;
			bool shouldCopyRowFormat = PasteSpecialOptions.ShouldCopyFormatAndStyle || PasteSpecialOptions.ShouldCopyAllExceptBorders;
			bool shouldCopyContent = PasteSpecialOptions.ShouldCopyValues || PasteSpecialOptions.ShouldCopyFormulas;
			if (!(shouldCopyRowFormat || shouldCopyContent))
				return;
			int defaultSourceFormatIndex = SourceWorksheet.Workbook.StyleSheet.DefaultCellFormatIndex;
			foreach (Row sourceRow in SourceWorksheet.Rows) {
				Row targetNewRow = TargetWorksheet.Rows.GetRow(sourceRow.Index);
				using (HistoryTransaction transaction = new HistoryTransaction(targetNewRow.DocumentModel.History)) {
					if (shouldCopyRowFormat)
						CopyRowProperties(sourceRow, targetNewRow, defaultSourceFormatIndex);
					if (shouldCopyContent)
						CopyRowCoreContent(sourceRow, targetNewRow, currentTargetRange);
				}
			}
		}
		void CopyRowCoreContent(Row sourceRow, Row targetNewRow, CellRange currentTargetRange) {
			foreach (ICell sourceCell in sourceRow.Cells) {
				CellPosition targetCellPosition = new CellPosition(sourceCell.Key.ColumnIndex, sourceCell.Key.RowIndex);
				ICell targetCell = targetNewRow.Cells.GetCell(sourceCell.Key.ColumnIndex);
				CopyCellContent(sourceCell, targetCell, currentTargetRange);
			}
		}
		#endregion
		#region Columns
		protected override IEnumerable<Column> GetSourceColumns() {
			return SourceWorksheet.Columns.GetExistingColumns();
		}
		protected override Column CreateTargetColumn(Column sourceColumn) {
			return TargetWorksheet.Columns.CreateNewColumnRange(sourceColumn.StartIndex, sourceColumn.EndIndex);
		}
		#endregion
		protected override IEnumerable<CellRange> GetMergedCellRanges() {
			return SourceWorksheet.MergedCells.GetEVERYMergedRangeSLOWEnumerable();
		}
		protected override void ProcessSourceMergedRange(RangeCopyInfo rangeInfo, CellRange sourceMergedRange) {
			CellRange targetBigRange = rangeInfo.TargetBigRange;
			CellRange singleRange = GetTargetObjectRange(sourceMergedRange, targetBigRange) as CellRange;
			(this as IRangeCopyMergedRangePart).CreateTargetMergedRange(singleRange);
		}
		protected override List<ModelHyperlink> GetSourceHyperlinks() {
			return SourceWorksheet.Hyperlinks.InnerList;
		}
		protected internal override IEnumerable<Table> GetSourceTables(bool includeTablesIntersectedSourceRange) {
			return SourceWorksheet.Tables;
		}
		protected internal override List<Table> GetSourceTablesFromIntersectionSourceTargetRange(CellRange intersection) {
			return new List<Table>();
		}
		protected override string GetTargetTableName(string sourceTableName, List<string> existingTableNamesInWorkbook) {
			if (!GenerateNewTableName)
				return sourceTableName;
			return base.GetTargetTableName(sourceTableName, existingTableNamesInWorkbook);
		}
		protected override void ProcessSourceTable(RangeCopyInfo rangeInfo, Table sourceTable, List<string> existingTableNames) {
			CellRange targetBigRange = rangeInfo.TargetBigRange;
			CellRange targetTableRange = GetTargetObjectRange(sourceTable.Range, targetBigRange) as CellRange;
			(this as IRangeCopyTablePart).CreateTargetTable(sourceTable, targetTableRange, existingTableNames, targetBigRange);
		}
		protected override IEnumerable<IDrawingObject> GetDrawingsForCopy() {
			return SourceWorksheet.DrawingObjectsByZOrderCollections.GetDrawingObjects();
		}
		protected override internal IEnumerable<PivotTable> GetSourcePivotTables(CellRange sourceRange, bool includeTablesIntersectedSourceRange) {
			return SourceWorksheet.PivotTables;
		}
		protected override void ProcessSourcePivotTable(RangeCopyInfo rangeInfo, PivotTable sourcePivotTable, IList<string> existingTableNames) {
			CellRange targetBigRange = rangeInfo.TargetBigRange;
			CellRange targetRange = GetTargetObjectRange(sourcePivotTable.Range, targetBigRange) as CellRange;
			(this as IRangeCopyPivotTablePart).CreateTargetPivotTable(sourcePivotTable, targetRange, existingTableNames, targetBigRange);
		}
		protected override IEnumerable<Comment> GetCommentsForCopy() {
			return SourceWorksheet.Comments.InnerList;
		}
		protected void CopyColumnBreaks() {
			CopyBreakCollection(SourceWorksheet.ColumnBreaks, TargetWorksheet.ColumnBreaks);
		}
		protected void CopyRowBreaks() {
			CopyBreakCollection(SourceWorksheet.RowBreaks, TargetWorksheet.RowBreaks);
		}
		void CopyBreakCollection(PageBreakCollection sourceBreakCollection, PageBreakCollection targetBreaks) {
			targetBreaks.CopyFrom(sourceBreakCollection);
		}
		protected override IEnumerable<ModelProtectedRange> GetSourceProtectedRanges() {
			return TargetWorksheet.ProtectedRanges;
		}
		protected override void ProcessSourceProtectedRange(RangeCopyInfo rangeInfo, ModelProtectedRange sourceProtectedRange) {
			CellRangeBase targetRange = GetTargetObjectRange(sourceProtectedRange.CellRange, rangeInfo.TargetBigRange);
			CreateTargetProtectedRange(sourceProtectedRange, targetRange);
		}
		protected override void CopyDataValidations(RangeCopyInfo rangeCopyInfo) {
			if (!PasteSpecialOptions.ShouldCopyOtherFormats || IsEqualWorksheets)
				return;
			foreach (DataValidation item in SourceWorksheet.DataValidations) {
				CellRangeBase targetRange = GetTargetObjectRange(item.CellRange, rangeCopyInfo.TargetBigRange);
				ProcessTargetDataValidation(item.CloneTo(targetRange), CellPositionOffset.None);
			}
		}
		protected override IEnumerable<ConditionalFormatting> GetConditionalFormattingsForCopy(Worksheet worksheetsource) {
			return worksheetsource.ConditionalFormattings.InnerList;
		}
		protected override void CopyVmlDrawing() {
		}
		protected void CopyWorksheetViews() {
			ModelWorksheetView activeView = SourceWorksheet.ActiveView;
			bool savedTabSelected = TargetWorksheet.ActiveView.TabSelected;
			TargetWorksheet.ActiveView.CopyFrom(activeView);
			TargetWorksheet.ActiveView.TabSelected = savedTabSelected;
		}
		protected override void CopyAutoFilter(RangeCopyInfo rangeInfo) {
			TargetWorksheet.AutoFilter.CopyFrom(SourceWorksheet.AutoFilter);
		}
		protected override IEnumerable<SparklineGroup> GetSourceSparklineGroups() {
			return SourceWorksheet.SparklineGroups as IEnumerable<SparklineGroup>;
		}
		protected override void ProcessSourceSparklineGroup(RangeCopyInfo rangeInfo, SparklineGroup sourceSparklineGroup) {
			SparklineGroup targetGroup = sourceSparklineGroup.CloneTo(TargetWorksheet, true);
			TargetWorksheet.SparklineGroups.Add(targetGroup);
		}
		public override CellRangeBase GetTargetObjectRange(CellRangeBase sourceRange, CellRange currentTargetRange) {
			return sourceRange.Clone(TargetWorksheet);
		}
		protected override bool IsSourceObjectRangeInterserctsWithCurrentTargetRange(CellRangeBase sourceObjectRange, CellRange _currentTargetRange) {
			return Object.ReferenceEquals(TargetWorksheet, sourceObjectRange.Worksheet);
		}
		public void SetTargetWorksheets(IEnumerable<IWorksheet> targetWorksheets) {
			foreach (IWorksheet sheet in targetWorksheets) {
				if (!Object.ReferenceEquals(sheet.Workbook, WorkbookTarget))
					throw new ArgumentException(String.Format("sheet {0} is not in target Workbook", sheet.Name));
				doNotCreateExternalLinksForThisSheets.Add(sheet.Name);
			}
		}
		public override bool IsSheetWillBeExternalSheet(string name) {
			bool toSourceWorksheet = String.Equals(SourceWorksheet.Name, name, StringComparison.OrdinalIgnoreCase);
			return !toSourceWorksheet && !doNotCreateExternalLinksForThisSheets.Contains(name);
		}
	}
}
namespace DevExpress.XtraSpreadsheet.Model.External {
	public class CopyExternalWorksheetOperation {
		ExternalWorksheet source;
		ExternalWorksheet target;
		public CopyExternalWorksheetOperation(ExternalWorksheet source, ExternalWorksheet target) {
			this.source = source;
			this.target = target;
		}
		public ExternalWorkbook TargetWorkbook { get { return target.Workbook as ExternalWorkbook; } }
		public ExternalWorkbook SourceWorkbook { get { return source.Workbook as ExternalWorkbook; } }
		public void Execute() {
			CopyDefinedNames();
			CopyExternalRows();
		}
		void CopyDefinedNames() {
			foreach (ExternalDefinedName sourceDefinedName in source.DefinedNames) {
				ParsedExpression targetDefinedNameExpression = GetTargetDefinedNameExpression(sourceDefinedName.Name, sourceDefinedName, target);
				ExternalDefinedName newTargetDefinedName = new ExternalDefinedName(sourceDefinedName.Name, TargetWorkbook, targetDefinedNameExpression, target.SheetId);
				target.DefinedNames.Add(newTargetDefinedName);
			}
		}
		internal void CopyExternalRows() {
			foreach (ExternalRow sourceRow in source.Rows) {
				ExternalRow targetNewRow = target.Rows.GetRow(sourceRow.Index) as ExternalRow;
				foreach (ExternalCell sourceCell in sourceRow.Cells) {
					CellPosition targetCellPosition = new CellPosition(sourceCell.Key.ColumnIndex, sourceCell.Key.RowIndex);
					ExternalCell targetCell = new ExternalCell(targetCellPosition, target);
					targetNewRow.Cells.Add(targetCell);
					targetCell.SetValue(sourceCell.Value);
				}
			}
		}
		ParsedExpression GetTargetDefinedNameExpression(string Name, DefinedNameBase sourceDefinedName, IWorksheet targetWorkshet) {
			Debug.ReferenceEquals(source.Workbook, targetWorkshet.Workbook); 
			ParsedExpression sourceExpressionClone = sourceDefinedName.Expression.Clone();
			return sourceExpressionClone; 
		}
	}
}
