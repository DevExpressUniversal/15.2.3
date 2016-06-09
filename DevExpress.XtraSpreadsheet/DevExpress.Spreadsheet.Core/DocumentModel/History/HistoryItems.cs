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
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model.External;
using System.Drawing;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraSpreadsheet.Model.History {
	#region CellValueChangedHistoryItem
	public class CellValueChangedHistoryItem : SpreadsheetHistoryItem {
		public CellValueChangedHistoryItem(Worksheet worksheet, ICell cell, VariantValue oldValue, VariantValue newValue)
			: base(worksheet) {
			this.ICell = cell;
			if (oldValue.IsInlineText)
				this.OldValue = oldValue.InlineTextValue;
			else
				this.OldValue = oldValue;
			this.NewValue = newValue;
		}
		public ICell ICell { get; private set; }
		public VariantValue OldValue { get; private set; }
		public VariantValue NewValue { get; private set; }
		protected override void UndoCore() {
			if (Workbook.AffectedCellsRepository != null)
				Workbook.AffectedCellsRepository.Add(new CellContentSnapshot(ICell));
			ICell.ReplaceValueByValueCore(OldValue);
		}
		protected override void RedoCore() {
			if (Workbook.AffectedCellsRepository != null)
				Workbook.AffectedCellsRepository.Add(new CellContentSnapshot(ICell));
			ICell.ReplaceValueByValueCore(NewValue);
		}
	}
	#endregion
	#region CellFormulaChangedHistoryItem
	public class CellFormulaChangedHistoryItem : SpreadsheetHistoryItem {
		public CellFormulaChangedHistoryItem(Worksheet worksheet, ICell cell, byte[] oldFormula, byte[] newFormula)
			: base(worksheet) {
			this.ICell = cell;
			this.OldFormula = oldFormula;
			this.NewFormula = newFormula;
		}
		public ICell ICell { get; private set; }
		public byte[] OldFormula { get; private set; }
		public byte[] NewFormula { get; private set; }
		protected override void UndoCore() {
			if (Workbook.AffectedCellsRepository != null)
				Workbook.AffectedCellsRepository.Add(new CellContentSnapshot(ICell));
			Workbook.CalculationChain.OnBeforeCellFormulaChanged(ICell);
			ICell.SetBinaryFormula(OldFormula);
			Workbook.CalculationChain.OnAfterCellFormulaChanged(ICell);
		}
		protected override void RedoCore() {
			if (Workbook.AffectedCellsRepository != null)
				Workbook.AffectedCellsRepository.Add(new CellContentSnapshot(ICell));
			Workbook.CalculationChain.OnBeforeCellFormulaChanged(ICell);
			ICell.SetBinaryFormula(NewFormula);
			Workbook.CalculationChain.OnAfterCellFormulaChanged(ICell);
		}
	}
	#endregion
	#region CellFormulaModifiedHistoryItem
	public class CellFormulaModifiedHistoryItem : SpreadsheetHistoryItem {
		public CellFormulaModifiedHistoryItem(Worksheet worksheet, ICell cell, byte[] oldFormula, byte[] newFormula)
			: base(worksheet) {
			this.ICell = cell;
			this.OldFormula = oldFormula;
			this.NewFormula = newFormula;
		}
		public ICell ICell { get; private set; }
		public byte[] OldFormula { get; private set; }
		public byte[] NewFormula { get; private set; }
		protected override void UndoCore() {
			if (Workbook.AffectedCellsRepository != null)
				Workbook.AffectedCellsRepository.Add(new CellContentSnapshot(ICell));
			Workbook.CalculationChain.OnBeforeCellFormulaChanged(ICell);
			ICell.FormulaInfo.BinaryFormula = OldFormula;
			Workbook.CalculationChain.OnAfterCellFormulaChanged(ICell);
		}
		protected override void RedoCore() {
			if (Workbook.AffectedCellsRepository != null)
				Workbook.AffectedCellsRepository.Add(new CellContentSnapshot(ICell));
			Workbook.CalculationChain.OnBeforeCellFormulaChanged(ICell);
			ICell.FormulaInfo.BinaryFormula = NewFormula;
			Workbook.CalculationChain.OnAfterCellFormulaChanged(ICell);
		}
	}
	#endregion
	#region CellValueReplacedWithFormulaHistoryItem
	public class CellValueReplacedWithFormulaHistoryItem : SpreadsheetHistoryItem {
		public CellValueReplacedWithFormulaHistoryItem(Worksheet worksheet, ICell cell, VariantValue oldValue, byte[] newFormula)
			: base(worksheet) {
			this.ICell = cell;
			if (oldValue.IsInlineText)
				this.OldValue = oldValue.InlineTextValue;
			else
				this.OldValue = oldValue;
			this.NewFormula = newFormula;
		}
		public ICell ICell { get; private set; }
		public VariantValue OldValue { get; private set; }
		public byte[] NewFormula { get; private set; }
		protected override void UndoCore() {
			if (Workbook.AffectedCellsRepository != null)
				Workbook.AffectedCellsRepository.Add(new CellContentSnapshot(ICell));
			Workbook.CalculationChain.OnBeforeCellFormulaReplacedByValue(ICell);
			ICell.SetValueCore(OldValue);
			Workbook.CalculationChain.OnAfterCellFormulaReplacedByValue(ICell);
		}
		protected override void RedoCore() {
			if (Workbook.AffectedCellsRepository != null)
				Workbook.AffectedCellsRepository.Add(new CellContentSnapshot(ICell));
			Workbook.CalculationChain.OnBeforeCellValueReplacedByFormula(ICell);
			ICell.SetBinaryFormula(NewFormula);
			Workbook.CalculationChain.OnAfterCellValueReplacedByFormula(ICell);
		}
	}
	#endregion
	#region CellFormulaReplacedWithValueHistoryItem
	public class CellFormulaReplacedWithValueHistoryItem : SpreadsheetHistoryItem {
		public CellFormulaReplacedWithValueHistoryItem(Worksheet worksheet, ICell cell, byte[] oldFormula, VariantValue newValue)
			: base(worksheet) {
			this.ICell = cell;
			this.OldFormula = oldFormula;
			this.NewValue = newValue;
		}
		public ICell ICell { get; private set; }
		public byte[] OldFormula { get; private set; }
		public VariantValue NewValue { get; private set; }
		protected override void UndoCore() {
			if (Workbook.AffectedCellsRepository != null)
				Workbook.AffectedCellsRepository.Add(new CellContentSnapshot(ICell));
			Workbook.CalculationChain.OnBeforeCellValueReplacedByFormula(ICell);
			ICell.SetBinaryFormula(OldFormula);
			this.Worksheet.WebRanges.AddChangedCellPosition(this.ICell);
			Workbook.CalculationChain.OnAfterCellValueReplacedByFormula(ICell);
		}
		protected override void RedoCore() {
			if (Workbook.AffectedCellsRepository != null)
				Workbook.AffectedCellsRepository.Add(new CellContentSnapshot(ICell));
			Workbook.CalculationChain.OnBeforeCellFormulaReplacedByValue(ICell);
			ICell.SetValueCore(NewValue);
			this.Worksheet.WebRanges.AddChangedCellPosition(this.ICell);
			Workbook.CalculationChain.OnAfterCellFormulaReplacedByValue(ICell);
		}
	}
	#endregion
	#region MergedCells
	public class CellMergeHistoryItem : SpreadsheetHistoryItem {
		#region fields
		readonly CellRange mergedCell;
		#endregion
		public CellMergeHistoryItem(Worksheet worksheet, CellRange mergedCell)
			: base(worksheet) {
			Guard.ArgumentNotNull(mergedCell, "mergedCell");
			this.mergedCell = mergedCell;
		}
		protected CellRange MergedCell { get { return mergedCell; } }
		protected override void RedoCore() {
			Worksheet.MergedCells.AddCore(mergedCell);
			Worksheet.WebRanges.ChangeRange(mergedCell);
		}
		protected override void UndoCore() {
			Worksheet.MergedCells.RemoveCore(Worksheet.MergedCells.Count - 1, mergedCell);
			Worksheet.WebRanges.ChangeRange(mergedCell);
		}
	}
	public class CellUnMergeHistoryItem : SpreadsheetHistoryItem {
		readonly CellRange mergedRange;
		int index;
		public CellUnMergeHistoryItem(Worksheet worksheet, CellRange mergedRange)
			: base(worksheet) {
			this.mergedRange = mergedRange;
			this.index = Worksheet.MergedCells.IndexOf(mergedRange);
		}
		public CellUnMergeHistoryItem(Worksheet worksheet, CellRange mergedRange, int index)
			: base(worksheet) {
			this.mergedRange = mergedRange;
			this.index = index;
		}
		protected override void RedoCore() {
			Worksheet.MergedCells.RemoveCore(index, mergedRange);
			Worksheet.WebRanges.ChangeRange(mergedRange);
		}
		protected override void UndoCore() {
			Worksheet.MergedCells.Insert(index, mergedRange);
			Worksheet.WebRanges.ChangeRange(mergedRange);
		}
	}
	public class MergedCellRangeChangedHistoryItem : SpreadsheetHistoryItem {
		#region fields
		readonly int index;
		readonly CellRange oldRange;
		readonly CellRange newRange;
		#endregion
		public MergedCellRangeChangedHistoryItem(Worksheet worksheet, int mergedCellIndex, CellRange newRange, CellRange oldRange)
			: base(worksheet) {
			this.index = mergedCellIndex;
			this.newRange = newRange;
			this.oldRange = oldRange;
		}
		protected override void RedoCore() {
			Worksheet.MergedCells.ChangeItemRangeCore(index, newRange);
			this.Worksheet.WebRanges.ChangeRange(newRange);
		}
		protected override void UndoCore() {
			Worksheet.MergedCells.ChangeItemRangeCore(index, oldRange);
			this.Worksheet.WebRanges.ChangeRange(oldRange);
		}
	}
	#endregion
	#region FormatBaseHistoryItems
	public abstract class FormatBaseHistoryItem : IndexChangedHistoryItemCore<DocumentModelChangeActions> {
		readonly FormatBase obj;
		static IDocumentModelPart GetModelPart(FormatBase obj) {
			Guard.ArgumentNotNull(obj, "obj");
			return obj.DocumentModel;
		}
		protected FormatBaseHistoryItem(FormatBase obj)
			: base(GetModelPart(obj)) {
			this.obj = obj;
		}
		protected FormatBase Object { get { return obj; } }
		public override IIndexBasedObject<DocumentModelChangeActions> GetObject() {
			return null;
		}
	}
	public class FormatBaseFontIndexChangeHistoryItem : FormatBaseHistoryItem {
		public FormatBaseFontIndexChangeHistoryItem(FormatBase obj)
			: base(obj) {
		}
		protected override void UndoCore() {
			Object.SetIndexCore(FormatBase.FontIndexAccessor, OldIndex, ChangeActions);
		}
		protected override void RedoCore() {
			Object.SetIndexCore(FormatBase.FontIndexAccessor, NewIndex, ChangeActions);
		}
	}
	public class FormatBaseAlignmentIndexChangeHistoryItem : FormatBaseHistoryItem {
		public FormatBaseAlignmentIndexChangeHistoryItem(FormatBase obj)
			: base(obj) {
		}
		protected override void UndoCore() {
			Object.SetIndexCore(FormatBase.AlignmentIndexAccessor, OldIndex, ChangeActions);
		}
		protected override void RedoCore() {
			Object.SetIndexCore(FormatBase.AlignmentIndexAccessor, NewIndex, ChangeActions);
		}
	}
	public class FormatBaseBorderIndexChangeHistoryItem : FormatBaseHistoryItem {
		public FormatBaseBorderIndexChangeHistoryItem(FormatBase obj)
			: base(obj) {
		}
		protected override void UndoCore() {
			Object.SetIndexCore(FormatBase.BorderIndexAccessor, OldIndex, ChangeActions);
		}
		protected override void RedoCore() {
			Object.SetIndexCore(FormatBase.BorderIndexAccessor, NewIndex, ChangeActions);
		}
	}
	public class FormatBaseFillIndexChangeHistoryItem : FormatBaseHistoryItem {
		public FormatBaseFillIndexChangeHistoryItem(FormatBase obj)
			: base(obj) {
		}
		protected override void UndoCore() {
			Object.SetIndexCore(FormatBase.FillIndexAccessor, OldIndex, ChangeActions);
		}
		protected override void RedoCore() {
			Object.SetIndexCore(FormatBase.FillIndexAccessor, NewIndex, ChangeActions);
		}
	}
	public class FormatBaseGradientFillIndexChangeHistoryItem : FormatBaseHistoryItem {
		public FormatBaseGradientFillIndexChangeHistoryItem(FormatBase obj)
			: base(obj) {
		}
		protected override void UndoCore() {
			Object.SetIndexCore(FormatBase.GradientFillIndexAccessor, OldIndex, ChangeActions);
		}
		protected override void RedoCore() {
			Object.SetIndexCore(FormatBase.GradientFillIndexAccessor, NewIndex, ChangeActions);
		}
	}
	public class FormatBaseCellFormatFlagsIndexChangeHistoryItem : FormatBaseHistoryItem {
		public FormatBaseCellFormatFlagsIndexChangeHistoryItem(FormatBase obj)
			: base(obj) {
		}
		protected override void UndoCore() {
			Object.SetIndexCore(FormatBase.CellFormatFlagsIndexAccessor, OldIndex, ChangeActions);
		}
		protected override void RedoCore() {
			Object.SetIndexCore(FormatBase.CellFormatFlagsIndexAccessor, NewIndex, ChangeActions);
		}
	}
	public class FormatBaseNumberFormatIndexChangeHistoryItem : FormatBaseHistoryItem {
		public FormatBaseNumberFormatIndexChangeHistoryItem(FormatBase obj)
			: base(obj) {
		}
		protected override void UndoCore() {
			Object.SetIndexCore(FormatBase.NumberFormatIndexAccessor, OldIndex, ChangeActions);
		}
		protected override void RedoCore() {
			Object.SetIndexCore(FormatBase.NumberFormatIndexAccessor, NewIndex, ChangeActions);
		}
	}
	#endregion
	#region CellFormatHistoryItems
	public abstract class CellFormatHistoryItem : IndexChangedHistoryItemCore<DocumentModelChangeActions> {
		readonly CellFormat obj;
		static IDocumentModelPart GetModelPart(CellFormat obj) {
			Guard.ArgumentNotNull(obj, "obj");
			return obj.DocumentModel;
		}
		protected CellFormatHistoryItem(CellFormat obj)
			: base(GetModelPart(obj)) {
			this.obj = obj;
		}
		protected CellFormat Object { get { return obj; } }
		public override IIndexBasedObject<DocumentModelChangeActions> GetObject() {
			return null;
		}
	}
	public class CellFormatStyleInfoIndexChangeHistoryItem : CellFormatHistoryItem {
		public CellFormatStyleInfoIndexChangeHistoryItem(CellFormat obj)
			: base(obj) {
		}
		protected override void UndoCore() {
			Object.SetIndexCore(CellFormat.StyleInfoIndexAccessor, OldIndex, ChangeActions);
		}
		protected override void RedoCore() {
			Object.SetIndexCore(CellFormat.StyleInfoIndexAccessor, NewIndex, ChangeActions);
		}
	}
	#endregion
	#region RowHistoryItems
	public abstract class RowHistoryItem : IndexChangedHistoryItemCore<DocumentModelChangeActions> {
		readonly Row obj;
		static IDocumentModelPart GetModelPart(Row obj) {
			Guard.ArgumentNotNull(obj, "obj");
			return obj.DocumentModel;
		}
		protected RowHistoryItem(Row obj)
			: base(GetModelPart(obj)) {
			this.obj = obj;
		}
		protected Row Object { get { return obj; } }
		public override IIndexBasedObject<DocumentModelChangeActions> GetObject() {
			return null;
		}
	}
	public class RowCellFormatIndexChangeHistoryItem : RowHistoryItem {
		public RowCellFormatIndexChangeHistoryItem(Row obj)
			: base(obj) {
		}
		protected override void UndoCore() {
			Object.SetIndexCore(Row.CellFormatIndexAccessor, OldIndex, ChangeActions);
		}
		protected override void RedoCore() {
			Object.SetIndexCore(Row.CellFormatIndexAccessor, NewIndex, ChangeActions);
		}
	}
	public class RowHeightIndexChangeHistoryItem : RowHistoryItem {
		public RowHeightIndexChangeHistoryItem(Row obj)
			: base(obj) {
		}
		protected override void UndoCore() {
			Object.SetIndexCore(Row.HeightIndexAccessor, OldIndex, ChangeActions);
		}
		protected override void RedoCore() {
			Object.SetIndexCore(Row.HeightIndexAccessor, NewIndex, ChangeActions);
		}
	}
	public class RowInfoIndexChangeHistoryItem : RowHistoryItem {
		public RowInfoIndexChangeHistoryItem(Row obj)
			: base(obj) {
		}
		protected override void UndoCore() {
			Object.SetIndexCore(Row.InfoIndexAccessor, OldIndex, ChangeActions);
		}
		protected override void RedoCore() {
			Object.SetIndexCore(Row.InfoIndexAccessor, NewIndex, ChangeActions);
		}
	}
	#endregion
	#region ColumnHistoryItems
	public abstract class ColumnHistoryItem : IndexChangedHistoryItemCore<DocumentModelChangeActions> {
		readonly Column obj;
		static IDocumentModelPart GetModelPart(Column obj) {
			Guard.ArgumentNotNull(obj, "obj");
			return obj.DocumentModel;
		}
		protected ColumnHistoryItem(Column obj)
			: base(GetModelPart(obj)) {
			this.obj = obj;
		}
		protected Column Object { get { return obj; } }
		public override IIndexBasedObject<DocumentModelChangeActions> GetObject() {
			return null;
		}
	}
	public class ColumnCellFormatIndexChangeHistoryItem : ColumnHistoryItem {
		public ColumnCellFormatIndexChangeHistoryItem(Column obj)
			: base(obj) {
		}
		protected override void UndoCore() {
			Object.SetIndexCore(Column.CellFormatIndexAccessor, OldIndex, ChangeActions);
		}
		protected override void RedoCore() {
			Object.SetIndexCore(Column.CellFormatIndexAccessor, NewIndex, ChangeActions);
		}
	}
	public class ColumnWidthIndexChangeHistoryItem : ColumnHistoryItem {
		public ColumnWidthIndexChangeHistoryItem(Column obj)
			: base(obj) {
		}
		protected override void UndoCore() {
			Object.SetIndexCore(Column.WidthIndexAccessor, OldIndex, ChangeActions);
		}
		protected override void RedoCore() {
			Object.SetIndexCore(Column.WidthIndexAccessor, NewIndex, ChangeActions);
		}
	}
	public class ColumnInfoIndexChangeHistoryItem : ColumnHistoryItem {
		public ColumnInfoIndexChangeHistoryItem(Column obj)
			: base(obj) {
		}
		protected override void UndoCore() {
			Object.SetIndexCore(Column.InfoIndexAccessor, OldIndex, ChangeActions);
		}
		protected override void RedoCore() {
			Object.SetIndexCore(Column.InfoIndexAccessor, NewIndex, ChangeActions);
		}
	}
	#endregion
	#region CellStyleHistoryItems
	public abstract class CellStyleHistoryItem : IndexChangedHistoryItemCore<DocumentModelChangeActions> {
		readonly CellStyleBase obj;
		static IDocumentModelPart GetModelPart(CellStyleBase obj) {
			Guard.ArgumentNotNull(obj, "obj");
			return obj.DocumentModel;
		}
		protected CellStyleHistoryItem(CellStyleBase obj)
			: base(GetModelPart(obj)) {
			this.obj = obj;
		}
		protected CellStyleBase Object { get { return obj; } }
		public override IIndexBasedObject<DocumentModelChangeActions> GetObject() {
			return null;
		}
	}
	public class CellStyleFormatIndexChangeHistoryItem : CellStyleHistoryItem {
		public CellStyleFormatIndexChangeHistoryItem(CellStyleBase obj)
			: base(obj) {
		}
		protected override void UndoCore() {
			Object.SetIndexCore(CellStyleBase.CellStyleFormatIndexAccessor, OldIndex, ChangeActions);
		}
		protected override void RedoCore() {
			Object.SetIndexCore(CellStyleBase.CellStyleFormatIndexAccessor, NewIndex, ChangeActions);
		}
	}
	#endregion
	#region ChangeDataValidationRangeHistoryItem
	public class ChangeDataValidationRangeHistoryItem : SpreadsheetSimpleTypeHistoryItem<CellRangeBase> {
		#region Fields
		readonly DataValidation validation;
		#endregion
		public ChangeDataValidationRangeHistoryItem(DataValidation validation, CellRangeBase oldRange, CellRangeBase newRange)
			: base(validation.DocumentModel.ActiveSheet, oldRange, newRange) {
			this.validation = validation;
		}
		protected override void UndoCore() {
			validation.SetRangeCore(OldValue);
		}
		protected override void RedoCore() {
			validation.SetRangeCore(NewValue);
		}
	}
	#endregion
	#region ChangeDataValidationExpressionHistoryItem
	public class ChangeDataValidationExpressionHistoryItem : SpreadsheetSimpleTypeHistoryItem<ParsedExpression> {
		#region Fields
		readonly DataValidation validation;
		readonly bool firstExpression;
		#endregion
		public ChangeDataValidationExpressionHistoryItem(DataValidation validation, ParsedExpression oldExpression, ParsedExpression newExpression, bool firstExpression)
			: base(validation.DocumentModel.ActiveSheet, oldExpression, newExpression) {
			this.validation = validation;
			this.firstExpression = firstExpression;
		}
		protected override void UndoCore() {
			if (firstExpression)
				validation.SetExpression1Core(OldValue);
			else
				validation.SetExpression2Core(OldValue);
		}
		protected override void RedoCore() {
			if (firstExpression)
				validation.SetExpression1Core(NewValue);
			else
				validation.SetExpression2Core(NewValue);
		}
	}
	#endregion
	#region DifferentialFormatHistoryItem
	public abstract class DifferentialFormatHistoryItem : IndexChangedHistoryItemCore<DocumentModelChangeActions> {
		readonly DifferentialFormat obj;
		static IDocumentModelPart GetModelPart(DifferentialFormat obj) {
			Guard.ArgumentNotNull(obj, "obj");
			return obj.DocumentModel;
		}
		protected DifferentialFormatHistoryItem(DifferentialFormat obj)
			: base(GetModelPart(obj)) {
			this.obj = obj;
		}
		protected DifferentialFormat Object { get { return obj; } }
		public override IIndexBasedObject<DocumentModelChangeActions> GetObject() {
			return null;
		}
	}
	#endregion
	#region DifferentialFormatMultiOptionsIndexChangeHistoryItem
	public class DifferentialFormatMultiOptionsIndexChangeHistoryItem : DifferentialFormatHistoryItem {
		public DifferentialFormatMultiOptionsIndexChangeHistoryItem(DifferentialFormat obj)
			: base(obj) {
		}
		protected override void UndoCore() {
			Object.SetIndexCore(DifferentialFormat.MultiOptionsIndexAccessor, OldIndex, ChangeActions);
		}
		protected override void RedoCore() {
			Object.SetIndexCore(DifferentialFormat.MultiOptionsIndexAccessor, NewIndex, ChangeActions);
		}
	}
	#endregion
	#region DifferentialFormatOptionsIndexChangeHistoryItem
	public class DifferentialFormatBorderOptionsIndexChangeHistoryItem : DifferentialFormatHistoryItem {
		public DifferentialFormatBorderOptionsIndexChangeHistoryItem(DifferentialFormat obj)
			: base(obj) {
		}
		protected override void UndoCore() {
			Object.SetIndexCore(DifferentialFormat.BorderOptionsIndexAccessor, OldIndex, ChangeActions);
		}
		protected override void RedoCore() {
			Object.SetIndexCore(DifferentialFormat.BorderOptionsIndexAccessor, NewIndex, ChangeActions);
		}
	}
	#endregion
	#region DifferentialFormatNumberFormatIdInfoIndexChangeHistoryItem
	public class DifferentialFormatNumberFormatIdInfoIndexChangeHistoryItem : DifferentialFormatHistoryItem {
		public DifferentialFormatNumberFormatIdInfoIndexChangeHistoryItem(DifferentialFormat obj)
			: base(obj) {
		}
		protected override void UndoCore() {
			Object.SetIndexCore(DifferentialFormat.NumberFormatIdInfoIndexAccessor, OldIndex, ChangeActions);
		}
		protected override void RedoCore() {
			Object.SetIndexCore(DifferentialFormat.NumberFormatIdInfoIndexAccessor, NewIndex, ChangeActions);
		}
	}
	#endregion
	#region RangeInsertedHistoryItem
	public class RangeInsertedHistoryItem : SpreadsheetHistoryItem {
		readonly int sheetId;
		readonly CellRangeBase range;
		readonly InsertCellMode mode;
		public RangeInsertedHistoryItem(DocumentModel documentModel, int sheetId, CellRangeBase range, InsertCellMode mode)
			: base(documentModel) {
			this.sheetId = sheetId;
			this.range = range;
			this.mode = mode;
		}
		protected override void UndoCore() {
			Workbook.CalculationChain.OnAfterRangeRemoving(sheetId, range, CalculateRemoveRangeMode());
		}
		protected override void RedoCore() {
			Workbook.CalculationChain.OnAfterRangeInserting(sheetId, range, mode);
		}
		RemoveCellMode CalculateRemoveRangeMode() {
			switch (mode) {
				case InsertCellMode.ShiftCellsDown:
					return RemoveCellMode.ShiftCellsUp;
				case InsertCellMode.ShiftCellsRight:
					return RemoveCellMode.ShiftCellsLeft;
				default:
					return RemoveCellMode.Default;
			}
		}
	}
	#endregion
	#region RangeRemovedHistoryItem
	public class RangeRemovedHistoryItem : SpreadsheetHistoryItem {
		readonly int sheetId;
		readonly CellRangeBase range;
		readonly RemoveCellMode mode;
		public RangeRemovedHistoryItem(DocumentModel documentModel, int sheetId, CellRangeBase range, RemoveCellMode mode)
			: base(documentModel) {
			this.sheetId = sheetId;
			this.range = range;
			this.mode = mode;
		}
		protected override void UndoCore() {
			if (mode != RemoveCellMode.Default)
				Workbook.CalculationChain.OnAfterRangeInserting(sheetId, range, CalculateInsertRangeMode());
		}
		protected override void RedoCore() {
			Workbook.CalculationChain.OnAfterRangeRemoving(sheetId, range, mode);
		}
		InsertCellMode CalculateInsertRangeMode() {
			switch (mode) {
				default:
				case RemoveCellMode.ShiftCellsLeft:
					return InsertCellMode.ShiftCellsRight;
				case RemoveCellMode.ShiftCellsUp:
					return InsertCellMode.ShiftCellsDown;
			}
		}
	}
	#endregion
	#region AutoFilterHistoryItems
	#region AutoFilterColumnHistoryItem
	public abstract class AutoFilterColumnHistoryItem : IndexChangedHistoryItemCore<DocumentModelChangeActions> {
		readonly AutoFilterColumn obj;
		static IDocumentModelPart GetModelPart(AutoFilterColumn obj) {
			Guard.ArgumentNotNull(obj, "obj");
			return obj.DocumentModel;
		}
		protected AutoFilterColumnHistoryItem(AutoFilterColumn obj)
			: base(GetModelPart(obj)) {
			this.obj = obj;
		}
		protected AutoFilterColumn Object { get { return obj; } }
		public override IIndexBasedObject<DocumentModelChangeActions> GetObject() {
			return null;
		}
	}
	#endregion
	#region  AutoFilterColumnInfoIndexChangeHistoryItem
	public class AutoFilterColumnInfoIndexChangeHistoryItem : AutoFilterColumnHistoryItem {
		public AutoFilterColumnInfoIndexChangeHistoryItem(AutoFilterColumn obj)
			: base(obj) {
		}
		protected override void UndoCore() {
			Object.SetIndexCore(AutoFilterColumn.AutoFilterColumnInfoIndexAccessor, OldIndex, ChangeActions);
		}
		protected override void RedoCore() {
			Object.SetIndexCore(AutoFilterColumn.AutoFilterColumnInfoIndexAccessor, NewIndex, ChangeActions);
		}
	}
	#endregion
	#region  AutoFilterColumnDifferentialFormatIndexChangeHistoryItem
	public class AutoFilterColumnDifferentialFormatIndexChangeHistoryItem : AutoFilterColumnHistoryItem {
		public AutoFilterColumnDifferentialFormatIndexChangeHistoryItem(AutoFilterColumn obj)
			: base(obj) {
		}
		protected override void UndoCore() {
			Object.SetIndexCore(AutoFilterColumn.DifferentialFormatIndexAccessor, OldIndex, ChangeActions);
		}
		protected override void RedoCore() {
			Object.SetIndexCore(AutoFilterColumn.DifferentialFormatIndexAccessor, NewIndex, ChangeActions);
		}
	}
	#endregion
	#endregion
	#region FormulaDataExpressionPropertyChangedHistoryItem
	public class FormulaDataExpressionPropertyChangedHistoryItem : SpreadsheetHistoryItem {
		readonly ParsedExpression oldValue;
		readonly ParsedExpression newValue;
		readonly FormulaData formulaData;
		public FormulaDataExpressionPropertyChangedHistoryItem(DocumentModel documentModel, FormulaData formulaData, ParsedExpression oldValue, ParsedExpression newValue)
			: base(documentModel) {
			this.formulaData = formulaData;
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		protected override void UndoCore() {
			formulaData.SetExpressionCore(oldValue);
		}
		protected override void RedoCore() {
			formulaData.SetExpressionCore(newValue);
		}
	}
	#endregion
	#region ExternalLinkAddHistoryItem
	public class ExternalLinkInsertHistoryItem : SpreadsheetHistoryItem {
		#region Fields
		readonly ExternalLink link;
		readonly int index;
		readonly bool modifyReferences;
		#endregion
		public ExternalLinkInsertHistoryItem(DocumentModel documentModel, ExternalLink link, int index, bool modifyReferences)
			: base(documentModel) {
			this.link = link;
			this.index = index;
			this.modifyReferences = modifyReferences;
		}
		protected override void UndoCore() {
			List<ExternalLink> externalLinksList = Workbook.ExternalLinks.InnerList;
			externalLinksList.RemoveAt(index);
			if (modifyReferences)
				Workbook.SheetDefinitions.ShiftDownExternalLinks(index + 1);
			Workbook.CalculationChain.ForceCalculate();
		}
		protected override void RedoCore() {
			List<ExternalLink> externalLinksList = Workbook.ExternalLinks.InnerList;
			if (modifyReferences)
				Workbook.SheetDefinitions.ShiftUpExternalLinks(index + 1);
			externalLinksList.Insert(index, link);
			Workbook.CalculationChain.ForceCalculate();
		}
	}
	#endregion
	#region ExternalLinkRemovedHistoryItem
	public class ExternalLinkRemovedHistoryItem : SpreadsheetHistoryItem {
		#region Fields
		readonly ExternalLink link;
		readonly int index;
		readonly bool moveReferences;
		#endregion
		public ExternalLinkRemovedHistoryItem(DocumentModel documentModel, int index, bool moveReferences)
			: base(documentModel) {
			this.index = index;
			this.link = documentModel.ExternalLinks[index];
			this.moveReferences = moveReferences;
		}
		protected override void UndoCore() {
			if (moveReferences)
				Workbook.SheetDefinitions.ShiftUpExternalLinks(index + 1);
			Workbook.ExternalLinks.InnerList.Insert(index, link);
			Workbook.CalculationChain.ForceCalculate();
		}
		protected override void RedoCore() {
			Workbook.ExternalLinks.InnerList.RemoveAt(index);
			if (moveReferences)
				Workbook.SheetDefinitions.ShiftDownExternalLinks(index + 1);
			Workbook.CalculationChain.ForceCalculate();
		}
	}
	#endregion
	#region SharedFormulaRangeChangedHistoryItem
	public class SharedFormulaRangeChangedHistoryItem : SpreadsheetHistoryItem {
		readonly SharedFormula formula;
		readonly CellRange newRange;
		CellRange oldRange;
		public SharedFormulaRangeChangedHistoryItem(DocumentModel documentModel, SharedFormula formula, CellRange newRange)
			: base(documentModel) {
			this.formula = formula;
			this.newRange = newRange;
		}
		protected override void UndoCore() {
			if (newRange != null)
				Workbook.CalculationChain.OnBeforeSharedFormulaChanged(formula);
			formula.SetRangeCore(oldRange);
			if (oldRange != null)
				Workbook.CalculationChain.OnAfterSharedFormulaChanged(formula);
		}
		protected override void RedoCore() {
			oldRange = formula.Range;
			if (oldRange != null)
				Workbook.CalculationChain.OnBeforeSharedFormulaChanged(formula);
			formula.SetRangeCore(newRange);
			if (newRange != null)
				Workbook.CalculationChain.OnAfterSharedFormulaChanged(formula);
		}
	}
	#endregion
	#region SharedFormulaExpressionChangedHistoryItem
	public class SharedFormulaExpressionChangedHistoryItem : SpreadsheetHistoryItem {
		readonly SharedFormula formula;
		readonly ParsedExpression newExpression;
		ParsedExpression oldExpression;
		public SharedFormulaExpressionChangedHistoryItem(DocumentModel documentModel, SharedFormula formula, ParsedExpression newExpression)
			: base(documentModel) {
			this.formula = formula;
			this.newExpression = newExpression;
		}
		protected override void UndoCore() {
			Workbook.CalculationChain.OnBeforeSharedFormulaChanged(formula);
			formula.SetExpressionCore(oldExpression);
			Workbook.CalculationChain.OnAfterSharedFormulaChanged(formula);
		}
		protected override void RedoCore() {
			oldExpression = formula.Expression;
			Workbook.CalculationChain.OnBeforeSharedFormulaChanged(formula);
			formula.SetExpressionCore(newExpression);
			Workbook.CalculationChain.OnAfterSharedFormulaChanged(formula);
		}
	}
	#endregion
	#region SparklineHistoryItems
	#region SparklineGroupExpressionHistoryItem
	public class SparklineGroupExpressionHistoryItem : SpreadsheetSimpleTypeHistoryItem<ParsedExpression> {
		readonly SparklineGroup sparklineGroup;
		public SparklineGroupExpressionHistoryItem(SparklineGroup sparklineGroup, ParsedExpression oldExpression, ParsedExpression newExpression)
			: base(sparklineGroup.Sheet, oldExpression, newExpression) {
			this.sparklineGroup = sparklineGroup;
		}
		protected override void UndoCore() {
			sparklineGroup.SetExpressionCore(OldValue);
		}
		protected override void RedoCore() {
			sparklineGroup.SetExpressionCore(NewValue);
		}
	}
	#endregion
	#region SparklineGroupColorsHistoryItem
	public class SparklineGroupColorsHistoryItem : SpreadsheetSimpleTypeHistoryItem<Color> {
		readonly SparklineGroup sparklineGroup;
		readonly SparklineColorType colorType;
		public SparklineGroupColorsHistoryItem(SparklineGroup sparklineGroup, SparklineColorType colorType, Color oldValue, Color newValue)
			: base(sparklineGroup.Sheet, oldValue, newValue) {
			this.sparklineGroup = sparklineGroup;
			this.colorType = colorType;
		}
		protected override void UndoCore() {
			sparklineGroup.SetColorIndexCore(OldValue, colorType);
		}
		protected override void RedoCore() {
			sparklineGroup.SetColorIndexCore(NewValue, colorType);
		}
	}
	#endregion
	#region SparklineExpressionHistoryItem
	public class SparklineExpressionHistoryItem : SpreadsheetSimpleTypeHistoryItem<ParsedExpression> {
		readonly Sparkline sparkline;
		public SparklineExpressionHistoryItem(Sparkline sparkline, ParsedExpression oldExpression, ParsedExpression newExpression)
			: base(sparkline.Sheet, oldExpression, newExpression) {
			this.sparkline = sparkline;
		}
		protected override void UndoCore() {
			sparkline.SetExpressionCore(OldValue);
		}
		protected override void RedoCore() {
			sparkline.SetExpressionCore(NewValue);
		}
	}
	#endregion
	#region SparklinePositionHistoryItem
	public class SparklinePositionHistoryItem : SpreadsheetCellPositionHistoryItem {
		readonly Sparkline sparkline;
		public SparklinePositionHistoryItem(Sparkline sparkline, CellPosition oldValue, CellPosition newValue)
			: base(sparkline.Sheet, oldValue, newValue) {
			this.sparkline = sparkline;
		}
		protected override void UndoCore() {
			sparkline.SetPositionCore(OldValue);
		}
		protected override void RedoCore() {
			sparkline.SetPositionCore(NewValue);
		}
	}
	#endregion
	#endregion
	#region IgnoredErrorRangeChangedHistoryItem
	public class IgnoredErrorRangeChangedHistoryItem : SpreadsheetSimpleTypeHistoryItem<CellRangeBase> {
		readonly IgnoredError ignoredError;
		public IgnoredErrorRangeChangedHistoryItem(IgnoredError ignoredError, CellRangeBase oldRange, CellRangeBase newRange)
			: base(ignoredError.Sheet, oldRange, newRange) {
			this.ignoredError = ignoredError;
		}
		protected override void UndoCore() {
			ignoredError.SetRangeCore(OldValue);
		}
		protected override void RedoCore() {
			ignoredError.SetRangeCore(NewValue);
		}
	}
	#endregion
}
