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
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.History;
using System.Collections.Generic;
using System.Globalization;
using DevExpress.Office;
using DevExpress.Export.Xl;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.Spreadsheet {
	#region CircularReferenceException
	public class CircularReferenceException : Exception {
		public CircularReferenceException()
			: base("Circular reference detected") {
		}
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.Model {
	#region InsertCellsFormatMode
	public enum InsertCellsFormatMode {
		FormatAsPrevious,
		FormatAsNext,
		ClearFormat
	}
	#endregion
	#region InsertCellMode
	public enum InsertCellMode {
		ShiftCellsRight = 1,
		ShiftCellsDown = 2
	}
	#endregion
	#region RemoveCellMode
	public enum RemoveCellMode {
		Default = 1, 
		ShiftCellsLeft = 2,
		ShiftCellsUp = 4,
		NoShiftOrRangeToPasteCutRange = 8 
	}
	#endregion
	#region CellCycleCheckState
	public enum CellCycleCheckState { 
		Unchecked = 0x0,
		Checking = 0x1,
		Checked = 0x2,
		CycleDetected = 0x3,
	}
	#endregion
	public delegate void SetValueDelegate(VariantValue value);
	public interface ICell : ICellBase, ICellFormat, IChainPrecedent, ICalculationWaiter, IBatchUpdateHandler, IIndexBasedObject<DocumentModelChangeActions>, IFormatBaseBatchUpdateable, IAutoFilterValue {
		void CellCopyFormatSameDocumentModels(ICell source);
		bool HasNoValue { get; }
		bool HasError { get; }
		ICellError Error { get; }
		bool ShouldUseInLayout { get; }
		int ContentVersion { get; set; }
		int TransactionVersion { get; set; }
		CellCycleCheckState 
		CycleCheckState { get; set; }
		new VariantValue Value { get; set; }
		new DocumentModel DocumentModel { get; }
		bool HasFormula { get; }
		string FormulaBody { get; set; }
		string FormulaBodyInvariant { get; }
		FormulaBase Formula { get; }
		string DisplayText { get; }
		new string Text { get; set; }
		Worksheet Worksheet {
			[DebuggerStepThrough]
			get;
		}
		WorkbookDataContext Context {
			[DebuggerStepThrough]
			get;
		}
		bool HasRichTextContent { get; }
		FormulaType FormulaType { get; }
		void SetTextSmart(string text);
		void SetFormattedValue(FormattedVariantValue value, string text);
		void SetFormattedValue(FormattedVariantValue value, string text, SetValueDelegate setValue);
		FormulaBase GetFormula();
		FormulaBase GetFormula(FormulaType formulaType);
		void SetFormula(FormulaBase formula);
		void SetFormulaCore(FormulaBase formula);
		void SetFormulaBody(string formulaBody);
		void ReplaceValueByValueCore(VariantValue value);
		void SetValueCore(VariantValue value);
		void SetValueNoChecks(VariantValue value);
		void ApplyFormula(FormulaBase formula);
		void ApplyFormulaCore(FormulaBase formula);
		void TransactedSetFormula(FormulaBase formula);
		void SetBinaryFormula(byte[] value);
		bool IsValueUpdated();
		VariantValue ExtractValue();
		VariantValue GetValue();
		void TransactedSetValue(VariantValue value);
		bool HasCircularReferences();
		void CheckCircularReferences();
		CellRange GetMergedCell();
		void ResetCachedContentVersions();
		void ResetCachedTransactionVersions();
		void OnBeforeRemove();
		void OnAfterInsert();
		void OnRangeRemovingShiftLeft(RemoveRangeNotificationContext notificationContext);
		void OnRangeRemovingShiftUp(RemoveRangeNotificationContext notificationContext);
		void OnRangeRemovingNoShift(RemoveRangeNotificationContext notificationContext);
		void OnBeforeSheetRemoved(RemoveRangeNotificationContext notificationContext);
		void OnRangeInsertingShiftRight(InsertRangeNotificationContext notificationContext);
		void OnRangeInsertingShiftDown(InsertRangeNotificationContext notificationContext);
		string GetText();
		ConditionalFormattingFormatAccumulator ConditionalFormatAccumulator { get; set; }
		NumberFormatResult GetFormatResult();
		NumberFormatResult GetFormatResult(NumberFormatParameters parameters);
		bool HasContent { get; }
		bool ClearContent();
		void ClearFormat();
		void ClearFormat(CellFormatApplyOptions options);
		string ToString();
		FormulaBase GetFormulaWithoutCustomFunctions(bool checkSimpleExpression);
		int ConditionalFormattingStoppedAtPriority { get; }
		void RemoveStyle();
		NumberFormatResult GetFormatResultCore(NumberFormat format, NumberFormatParameters parameters);
		#region ICellFormatOwner
		DocumentModelChangeActions GetBatchUpdateChangeActions();
		bool ChangeFormatIndex(int newIndex, DocumentModelChangeActions changeActions);
		void SetCellFormatIndex(int value);
		CellFormat FormatInfo { get; }
		short FormatIndex { get; }
		void ApplyFormat(CellFormatBase format);
		void ApplyFormat(CellFormat format, CellFormatApplyOptions options);
		void ApplyFormatIndex(int formatIndex);
		NumberFormat Format { get; }
		bool QuotePrefix { get; set; }
		XlPatternType ActualPatternType { get; }
		Color ActualBackgroundColor { get; }
		Color ActualForegroundColor { get; }
		bool HasVisibleFill { get; }
		NumberFormat ActualFormat { get; }
		IActualGradientFillInfo ActualGradientFillInfo { get; }
		IActualConvergenceInfo ActualConvergenceInfo { get; }
		XlHorizontalAlignment ActualHorizontalAlignment { get; }
		IActualCellAlignmentInfo InnerActualAlignment { get; }
		IActualFillInfo InnerActualFill { get; }
		IActualGradientFillInfo InnerActualGradientFillInfo { get; }
		IActualConvergenceInfo InnerActualConvergenceInfo { get; }
		IActualRunFontInfo InnerActualFont { get; }
		IActualCellProtectionInfo InnerActualProtection { get; }
		#endregion
		bool MarkedForRecalculation { get; set; }
		VariantValue CalculateFormulaValue();
		FormulaInfo FormulaInfo { get; set; }
		double DoubleValue { get; set; }
		Int64 PackedValues { get; set; }
		Int64 PackedFormat { get; set; }
	}
	#region Cell
	public partial class Cell : CellBase, ICell {
		#region Fields
		internal const uint transactionVersionMask = 0x7FFC000; 
		internal const int transactionMaskOffset = 14;
		internal const uint contentVersionMask = 0x3FFF; 
		const uint valueTypeMask = 0x38000000;
		const int valueTypeOffset = 27;
		const uint cycleCheckStateMask = 0xC0000000;
		const int cycleCheckStateOffset = 30;
		const uint formatIndexMask = 0xFFFF;
		const uint actualFontInfoIndexMask = 0xFFFF0000;
		const int actualFontInfoIndexOffset = 16;
		readonly Worksheet sheet; 
		double value; 
		uint packedValues; 
		uint packedFormat; 
		ConditionalFormattingFormatAccumulator conditionalFormatAccumulator;
		FormulaInfo formulaInfo;
		#endregion
		public Cell(CellPosition position, Worksheet sheet)
			: base(position, sheet.SheetId) {
			Guard.ArgumentNotNull(sheet, "sheet");
			this.sheet = sheet;
			this.FormatIndex = (short)sheet.Workbook.StyleSheet.DefaultCellFormatIndex;
			this.conditionalFormatAccumulator = GetCondFmtAccumulator(sheet, this.Key);
		}
		#region Properties
		public virtual FormulaInfo FormulaInfo { get { return formulaInfo; } set { formulaInfo = value; } }
		public double DoubleValue { get { return value; } set { this.value = value; } }
		public Int64 PackedValues { get { return packedValues; } set { packedValues = (uint)value; } }
		public Int64 PackedFormat { get { return packedFormat; } set { packedFormat = (uint)value; } }
		VariantValueType ValueType {
			get {
				return (VariantValueType)((packedValues & valueTypeMask) >> valueTypeOffset);
			}
			set {
				packedValues &= ~valueTypeMask;
				packedValues |= ((uint)value << valueTypeOffset) & valueTypeMask;
			}
		}
		public ICellError Error { get { return HasError ? CellErrorFactory.Errors[(int)value] : null; } }
		public bool HasError { get { return ValueType == VariantValueType.Error; } }
		public bool HasNoValue { get { return value == 0 && ValueType == VariantValueType.None; } }
		public virtual bool ShouldUseInLayout { get { return true; } }
		public int ContentVersion {
			get { return (int)(packedValues & contentVersionMask); }
			set {
				if (value == ContentVersion)
					return;
				int delta = value - (int)contentVersionMask;
				if (delta > 0)
					value = delta;
				packedValues &= ~contentVersionMask;
				packedValues |= ((uint)value & contentVersionMask);
			}
		}
		public int TransactionVersion {
			get {
				return (int)((packedValues & transactionVersionMask) >> transactionMaskOffset);
			}
			set {
				int delta = value - ((int)transactionVersionMask >> transactionMaskOffset);
				if (delta > 0)
					value = delta;
				packedValues &= ~transactionVersionMask;
				packedValues |= ((uint)value << transactionMaskOffset) & transactionVersionMask;
				ActualFontInfoIndex = -1;
			}
		}
		public CellCycleCheckState CycleCheckState { 
			get {
				return (CellCycleCheckState)((packedValues & cycleCheckStateMask) >> cycleCheckStateOffset);
			}
			set {
				CellCycleCheckState currentValue = CycleCheckState;
				if (currentValue == value)
					return;
				if (currentValue == CellCycleCheckState.CycleDetected)
					DocumentModel.DecrementCycleReferenceCellsCounter();
				if (value == CellCycleCheckState.CycleDetected)
					DocumentModel.IncrementCycleReferenceCellsCounter();
				packedValues &= ~cycleCheckStateMask;
				packedValues |= ((uint)value << cycleCheckStateOffset) & cycleCheckStateMask;
			}
		}
		public NumberFormat Format { get { return Workbook.Cache.NumberFormatCache[FormatInfo.NumberFormatIndex]; } }
		#region FormatString
		public string FormatString {
			get { return FormatInfo.FormatString; }
			set {
				SetPropertyValue(SetNumberFormat, value);
			}
		}
		DocumentModelChangeActions SetNumberFormat(FormatBase info, string value) {
			info.ForceSetFormatString(value);
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region QuotePrefix
		public bool QuotePrefix {
			get { return FormatInfo.QuotePrefix; }
			set {
				if (FormatInfo.QuotePrefix == value)
					return;
				SetPropertyValue(SetQuotePrefix, value);
			}
		}
		DocumentModelChangeActions SetQuotePrefix(FormatBase info, bool value) {
			CellFormatBase format = info as CellFormatBase;
			if (format != null)
				format.QuotePrefix = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region CellStyle members
		public CellStyleBase Style { get { return GetActualStyle(); } set { SetPropertyValue(SetStyleIndex, value); } }
		protected virtual CellStyleBase GetActualStyle() {
			return FormatInfo.Style;
		}
		DocumentModelChangeActions SetStyleIndex(FormatBase info, CellStyleBase value) {
			((CellFormat)info).ApplyStyle(value, Worksheet.IsProtected);
			return DocumentModelChangeActions.None;
		}
		#endregion
		public bool HasFormula { get { return FormulaInfo != null; } }
		public string FormulaBody {
			get {
				if (!HasFormula)
					return String.Empty;
				return GetFormula().GetBody(this);
			}
			set { SetFormulaBody(value); }
		}
		public string FormulaBodyInvariant {
			get {
				if (!HasFormula)
					return String.Empty;
				WorkbookDataContext context = this.Context;
				context.PushCulture(CultureInfo.InvariantCulture);
				try {
					return FormulaBody;
				}
				finally {
					context.PopCulture();
				}
			}
		}
		public FormulaBase Formula { get { return GetFormula(); } }
		public string DisplayText { get { return Text; } }
		public string Text { get { return GetText(); } set { SetText(value, Context); } }
		public override VariantValue Value { get { return GetValue(); } set { SetValue(value); } }
		public override IWorksheet Sheet { get { return sheet; } }
		public Worksheet Worksheet { [DebuggerStepThrough] get { return sheet; } }
		public DocumentModel Workbook { [DebuggerStepThrough] get { return Worksheet.Workbook; } }
		public DocumentModel DocumentModel { [DebuggerStepThrough] get { return Workbook; } }
		public WorkbookDataContext Context { [DebuggerStepThrough] get { return Sheet.Workbook.DataContext; } }
		public bool HasRichTextContent {
			get {
				if (HasFormula)
					return false;
				VariantValue value = Value;
				if (!value.IsSharedString)
					return false;
				ISharedStringItem item = Context.StringTable[value.SharedStringIndexValue];
				return item is FormattedStringItem;
			}
		}
		public virtual ConditionalFormattingFormatAccumulator ConditionalFormatAccumulator { get { return conditionalFormatAccumulator; } set { conditionalFormatAccumulator = value; } }
		public FormulaType FormulaType { get { return FormulaFactory.GetFormulaType(this); } }
		public bool MarkedForRecalculation {
			get {
				if (Workbook.CalculationChain.Enabled)
					return ContentVersion == 0x3FFF;
				return ContentVersion != Workbook.ContentVersion;
			}
			set {
				if (value)
					MarkUpForRecalculation();
				else
					ContentVersion = Workbook.ContentVersion;
			}
		}
		#endregion
		#region IChainPrecedent Members
		public bool AllowsMerging { get { return false; } }
		public void MarkUpForRecalculation() {
			this.ContentVersion = 0x3FFF;
		}
		public void MergeWith(IChainPrecedent item) {
			throw new InvalidOperationException();
		}
		public void AddItemsTo(IList<ICell> where, ISheetPosition affectedRange) {
			where.Add(this);
		}
		public CellRangeBase ToRange(ISheetPosition affectedRange) {
			return GetRange();
		}
		public bool Remove(IChainPrecedent cell) {
			return false;
		}
		#endregion
		#region ICalculationWaiter Members
		bool ICalculationWaiter.AllowsMerging { get { return false; } }
		bool ICalculationWaiter.TryInsertInto(CellsChain where, ICell position) {
			where.InsertAfter(position, this);
			return true;
		}
		void ICalculationWaiter.MergeWith(ICalculationWaiter waiter) {
			throw new InvalidOperationException("Merging is not supported for this class");
		}
		void ICalculationWaiter.AddToTheEndAndMarkUp(CellsChain where) {
			where.Add(this);
			FormulaFactory.SetFormulaCalculateAlways(this, true);
			MarkedForRecalculation = false;
		}
		#endregion
		#region IAutoFilterValue Members
		bool IAutoFilterValue.IsDateTime { get { return Format.IsDateTime; } }
		#endregion
		public void ApplyFormat(CellFormatBase format) {
			if (FormatInfo != format)
				ApplyFormatCore(format);
		}
		void ApplyFormatCore(CellFormatBase format) {
			DocumentModel.BeginUpdate();
			try {
				FormatBase info = GetInfoForModification();
				((CellFormatBase)info).ApplyFormat(format);
				ReplaceInfo(info, DocumentModelChangeActions.None); 
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		public void ApplyFormatIndex(int formatIndex) {
			ChangeIndexCore(formatIndex, GetBatchUpdateChangeActions());
		}
		public void ApplyFormat(CellFormat format, CellFormatApplyOptions options) {
			DocumentModel.BeginUpdate();
			try {
				FormatBase info = GetInfoForModification();
				((CellFormat)info).ApplyFormat(format, options);
				ReplaceInfo(info, DocumentModelChangeActions.None); 
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		public void RemoveStyle() {
			bool applyNumberFormat = ApplyNumberFormat;
			bool applyFont = ApplyFont;
			bool applyFill = ApplyFill;
			bool applyBorder = ApplyBorder;
			bool applyAlignment = ApplyAlignment;
			bool applyProtection = ApplyProtection;
			Style = Workbook.StyleSheet.CellStyles[0];
			ApplyNumberFormat = applyNumberFormat;
			ApplyFont = applyFont;
			ApplyFill = applyFill;
			ApplyBorder = applyBorder;
			ApplyAlignment = applyAlignment;
			ApplyProtection = applyProtection;
		}
		protected internal virtual void SetText(string text, WorkbookDataContext context) {
			if (ActualFormat.IsText)
				SetValue(text);
			else
				if (FormulaBase.IsFormula(text))
					SetFormulaBody(text);
				else
					SetValue(CellValueFormatter.GetValue(GetValue(), text, context, false).Value);
		}
		public void SetTextSmart(string text) {
			NumberFormat actualFormat = this.ActualFormat;
			if (!actualFormat.IsText && FormulaBase.IsFormula(text)) {
				BeginUpdate();
				try {
					SetFormulaBody(text);
					NumberFormat format = GetFormulaFormat(GetValue());
					if (format != null && format.IsDateTime && this.ActualFormat.IsGeneric)
						FormatString = format.FormatCode;
				}
				finally {
					EndUpdate();
				}
			}
			else {
				FormattedVariantValue value = CellValueFormatter.GetValue(GetValue(), text, Context, true, ActualFormat);
				SetFormattedValue(value, text);
			}
		}
		public void SetFormattedValue(FormattedVariantValue value, string text) {
			SetFormattedValue(value, text, this.SetValue);
		}
		public void SetFormattedValue(FormattedVariantValue value, string text, SetValueDelegate setValue) {
			NumberFormat actualFormat = this.ActualFormat;
			bool shouldSetWordWrap = this.ActualAlignment.WrapText == false && text.Contains(Environment.NewLine);
			bool shouldSetFormat = value.NumberFormatId != 0 && actualFormat.IsGeneric;
			if (!shouldSetWordWrap && !shouldSetFormat) {
				if (actualFormat.IsText)
					setValue(text);
				else
					setValue(value.Value);
				return;
			}
			BeginUpdate();
			try {
				if (actualFormat.IsText)
					setValue(text);
				else {
					if (shouldSetFormat)
						FormatString = Workbook.Cache.NumberFormatCache[value.NumberFormatId].FormatCode;
					setValue(value.Value);
				}
				if (shouldSetWordWrap)
					Alignment.WrapText = true;
			}
			finally {
				EndUpdate();
			}
		}
		NumberFormat GetFormulaFormat(VariantValue value) {
			if (!value.IsNumeric)
				return null;
			FormulaNumberFormatIdCalculator calculator = new FormulaNumberFormatIdCalculator();
			int id = calculator.Calculate(GetFormula().Expression);
			if (id <= 0)
				return null;
			return DocumentModel.StyleSheet.NumberFormats[id];
		}
		#region Formula
		public virtual FormulaBase GetFormula() {
			if (!HasFormula)
				return null;
			return FormulaFactory.CreateInstance(this);
		}
		public virtual FormulaBase GetFormula(FormulaType formulaType) {
			if (!HasFormula)
				return null;
			return FormulaFactory.CreateInstanceCore(this, formulaType);
		}
		public virtual void SetFormula(FormulaBase formula) {
			System.Diagnostics.Debug.Assert(formula != null);
			ArrayFormula arrayFormula = formula as ArrayFormula;
			ISheetPosition formulaRange;
			if (arrayFormula != null)
				formulaRange = arrayFormula.Range;
			else
				formulaRange = this;
			Worksheet.CheckingChangingPartOfArray(formulaRange);
			Workbook.BeginUpdate();
			try {
				Worksheet.RemoveArrayFormulasFromCollectionInsideRange(formulaRange);
				SetFormulaCore(formula);
				Worksheet.OnCellValueChanged(this);
			}
			finally {
				Workbook.EndUpdate();
			}
		}
		public virtual void SetFormulaCore(FormulaBase formula) {
			Workbook.BeginUpdate();
			try {
				TransactedSetFormula(formula);
			}
			finally {
				Workbook.EndUpdate();
			}
		}
		public virtual void TransactedSetFormula(FormulaBase formula) {
			Workbook.BeginUpdate();
			try {
				DetachFormulaFromShared();
				SpreadsheetHistoryItem historyItem;
				if (HasFormula) {
					historyItem = new CellFormulaChangedHistoryItem(Worksheet, this, formulaInfo.BinaryFormula, formula.GetBinary(Context));
				}
				else
					historyItem = new CellValueReplacedWithFormulaHistoryItem(Worksheet, this, Value, formula.GetBinary(Context));
				Workbook.History.Add(historyItem);
				historyItem.Execute();
				if (Context.SetValueShouldAffectSharedFormula)
					Worksheet.SharedFormulas.RemoveMarkedItems();
			}
			finally {
				Workbook.EndUpdate();
			}
		}
		public void ApplyFormula(FormulaBase formula) {
			if (formula != null)
				ApplyFormulaCore(formula);
			else {
				Workbook.CalculationChain.UnRegisterCell(this);
				formulaInfo = null;
			}
		}
		public void ApplyFormulaCore(FormulaBase formula) {
			FormulaInfo = new FormulaInfo();
			formula.ApplyDataToCell(this);
		}
		public virtual void SetFormulaBody(string formulaBody) {
			Formula formula = new Formula(this, formulaBody);
			SetFormula(formula);
		}
		public virtual void SetBinaryFormula(byte[] value) {
			FormulaInfo = new FormulaInfo();
			FormulaInfo.BinaryFormula = value;
			ApplyFormulaChanges();
		}
		void ApplyFormulaChanges() {
			if (HasFormula) {
				AssignValueCore(0);
			}
			else
				AssignValue(VariantValue.Empty);
		}
		void DetachFormulaFromShared() {
			if (Context.SetValueShouldAffectSharedFormula) {
				SharedFormulaRef sharedFormula = this.GetFormula() as SharedFormulaRef;
				if (sharedFormula != null) {
					sharedFormula.DetachFromSharedFormula();
					if (sharedFormula.HostSharedFormula.Range == null)
						Worksheet.SharedFormulas.MarkUpForDeletion(sharedFormula.HostFormulaIndex);
				}
			}
		}
		#endregion
		public bool IsValueUpdated() {
			if (!HasFormula)
				return false;
			VariantValue originalValue = ExtractValue();
			VariantValue updatedValue = GetValue();
			return !updatedValue.IsEqual(originalValue, StringComparison.Ordinal, Workbook.SharedStringTable);
		}
		#region Value calculation
		public virtual VariantValue GetValue() {
			if (Workbook.Properties.CalculationOptions.PrecisionAsDisplayed)
				return this.ActualFormat.Round(GetValueCore(), Context); 
			else
				return GetValueCore();
		}
		VariantValue GetValueCore() {
			if (!HasFormula)
				return ExtractValue();
			if (Workbook.CalculationChain.Enabled) {
				return ExtractValue();
			}
			return GetValueNormal();
		}
		#region Old formula calculation engine
		VariantValue GetValueNormal() {
			if (DocumentModel.Properties.CalculationOptions.CalculationMode == ModelCalculationMode.Manual)
				return ExtractValue();
			return CalculateFormulaValue();
		}
		public VariantValue CalculateFormulaValue() {
			if (ContentVersion == Workbook.ContentVersion)
				return ExtractValue();
			this.CycleCheckState = CellCycleCheckState.Checked;
			CalculationOptions calculationOptions = sheet.Workbook.Properties.CalculationOptions;
			if (calculationOptions.IterationsEnabled)
				return CalculateFormulaValueIterative(calculationOptions);
			else
				return CalculateFormulaValueSimple();
		}
		VariantValue CalculateFormulaValueSimple() {
			FormulaBase formula = GetFormula();
			WorkbookDataContext context = this.Context;
			CellValueCalculationInfo calculationInfo = context.CalculationsInfo.TryGetCalculationInfo(this);
			if (calculationInfo != null) { 
				context.CalculationsInfo.IsCycleDetected = true;
				this.CycleCheckState = CellCycleCheckState.CycleDetected;
				ContentVersion = Workbook.ContentVersion;
				return ExtractValue();
			}
			context.CalculationsInfo.Add(this);
			context.PushArrayFormulaProcessing(false);
			context.PushDefinedNameProcessing(null);
			context.PushArrayFormulaOffcet(CellPositionOffset.None);
			context.PushRelativeToCurrentCell(false);
			Workbook.RealTimeDataManager.OnStartCellCalculation(this);
			try {
				VariantValue formulaResult = formula.Calculate(this, context);
				if (context.CalculationsInfo.IsCycleDetected || formulaResult.IsMissing) {
					ContentVersion = Workbook.ContentVersion;
					return ExtractValue();
				}
				else {
					AssignValue(formulaResult);
					return formulaResult;
				}
			}
			finally {
				Workbook.RealTimeDataManager.OnEndCellCalculation(this);
				context.CalculationsInfo.Remove(this);
				context.PopArrayFormulaProcessing();
				context.PopDefinedNameProcessing();
				context.PopArrayFromulaOffcet();
				context.PopRelativeToCurrentCell();
			}
		}
		VariantValue CalculateFormulaValueIterative(CalculationOptions calculationOptions) {
			WorkbookDataContext context = this.Context;
			CellValueCalculationInfo calculationInfo = context.CalculationsInfo.TryGetCalculationInfo(this);
			if (calculationInfo == null) {
				calculationInfo = context.CalculationsInfo.Add(this);
				AssignValueCore(VariantValue.Empty);
			}
			if (calculationInfo.IterationCount >= calculationOptions.MaximumIterations)
				return ExtractValue();
			FormulaBase formula = GetFormula();
			if (Workbook.CalculationChain.Enabled) {
				if (ContentVersion != 0x3FFF) {
					if (!formula.IsVolatile()) {
						ContentVersion = Workbook.ContentVersion;
						return ExtractValue();
					}
				}
			}
			calculationInfo.IterationCount++;
			context.PushArrayFormulaProcessing(false);
			context.PushDefinedNameProcessing(null);
			context.PushArrayFormulaOffcet(CellPositionOffset.None);
			context.PushRelativeToCurrentCell(false);
			Workbook.RealTimeDataManager.OnStartCellCalculation(this);
			try {
				VariantValue formulaResult = formula.Calculate(this, context);
				AssignValue(formulaResult);
				return formulaResult;
			}
			finally {
				Workbook.RealTimeDataManager.OnEndCellCalculation(this);
				calculationInfo.IterationCount--;
				if (calculationInfo.IterationCount == 0)
					context.CalculationsInfo.Remove(this);
				context.PopArrayFormulaProcessing();
				context.PopDefinedNameProcessing();
				context.PopArrayFromulaOffcet();
				context.PopRelativeToCurrentCell();
			}
		}
		#endregion
		public override void AssignValue(VariantValue value) {
			AssignValueCore(value);
			ContentVersion = Workbook.ContentVersion;
		}
		public override void AssignValueCore(VariantValue value) {
			if (Workbook.SuppressCellValueAssignment)
				Exceptions.ThrowInvalidOperationException("Cell.Value assignment is prohibited at this time.");
			UnassignValueCore();
			System.Diagnostics.Debug.Assert(value.Type != VariantValueType.CellRange);
			System.Diagnostics.Debug.Assert(value.Type != VariantValueType.Array);
			VariantValueType valueType = value.Type;
			if (valueType == VariantValueType.InlineText)
				Worksheet.CellValueCache.RegisterInlineString(this, value.InlineTextValue);
			else if (valueType == VariantValueType.Error)
				this.value = (int)value.ErrorValue.Type;
			else
				this.value = value.NumericValue;
			this.ValueType = valueType;
		}
		void UnassignValueCore() {
			if (ValueType == VariantValueType.InlineText)
				Worksheet.CellValueCache.UnregisterInlineString(this);
		}
		public virtual bool HasCircularReferences() {
			if (!HasFormula)
				return false;
			if (Workbook.CalculationChain.Enabled)
				return Workbook.HasCircularReferences;
			this.GetValue(); 
			return (CycleCheckState == CellCycleCheckState.CycleDetected);
		}
		public void CheckCircularReferences() {
			if (HasCircularReferences())
				throw new DevExpress.Spreadsheet.CircularReferenceException();
		}
		#endregion
		#region Value
		public override void SetValue(VariantValue value) {
			DocumentModel documentModel = DocumentModel;
			documentModel.BeginUpdate();
			try {
				TransactedSetValue(value);
				Worksheet.OnCellValueChanged(this);
			}
			finally {
				documentModel.EndUpdate();
			}
		}
		public void SetValueNoChecks(VariantValue value) {
			System.Diagnostics.Debug.Assert(Workbook.IsUpdateLocked);
			TransactedSetValueNoChecks(value);
			sheet.WebRanges.AddChangedCellPosition(this);
			Worksheet.OnCellValueChanged(this);
		}
		public void TransactedSetValue(VariantValue value) {
			Worksheet worksheet = Worksheet;
			worksheet.CheckingChangingPartOfArray(this);
				worksheet.RemoveArrayFormulasFromCollectionInsideRange(this);
				TransactedSetValueNoChecks(value);
		}
		internal void TransactedSetValueNoChecks(VariantValue value) {
			DocumentModel documentModel = DocumentModel;
			System.Diagnostics.Debug.Assert(documentModel.IsUpdateLocked);
			VariantValue currentValue = Value;
			bool hasFormula = HasFormula;
			if (!hasFormula && CompareValues(value, currentValue))
				Workbook.History.AddEmptyOperation();
			else {
				SpreadsheetHistoryItem historyItem;
				if (hasFormula) {
					DetachFormulaFromShared();
					historyItem = new CellFormulaReplacedWithValueHistoryItem(Worksheet, this, formulaInfo.BinaryFormula, value);
				}
				else
					historyItem = new CellValueChangedHistoryItem(Worksheet, this, currentValue, value);
				Workbook.History.Add(historyItem);
				historyItem.Execute();
				if (Context.SetValueShouldAffectSharedFormula)
					Worksheet.SharedFormulas.RemoveMarkedItems();
			}
		}
		bool CompareValues(VariantValue value, VariantValue currentValue) {
			if (currentValue.IsText && QuotePrefix)
				currentValue = "\'" + currentValue;
			return currentValue.IsEqual(value, StringComparison.CurrentCulture, Context.StringTable);
		}
		public void ReplaceValueByValueCore(VariantValue value) {
			System.Diagnostics.Debug.Assert(Workbook.IsUpdateLockedOrOverlapped);
			SetValueCore(value);
			DocumentModel.CalculationChain.OnAfterCellValueChanged(this);
			Worksheet.WebRanges.AddChangedCellPosition(this);
		}
		public void SetValueCore(VariantValue value) {
			bool quotePrefixValue = false;
			if (value.IsText) {
				string text = value.GetTextValue(Worksheet.SharedStringTable);
				if (!String.IsNullOrEmpty(text) && text[0] == '\'') {
					quotePrefixValue = true;
					value = text.Substring(1);
				}
			}
			if (value.Type == VariantValueType.InlineText)
				value.SetSharedString(Worksheet.SharedStringTable, value.InlineTextValue);
			this.FormulaInfo = null;
			AssignValue(value);
			QuotePrefix = quotePrefixValue;
		}
		public VariantValue ExtractValue() {
			switch (ValueType) {
				case VariantValueType.None:
					return VariantValue.Empty;
				case VariantValueType.Error:
					return VariantValue.Errors[(int)value];
				case VariantValueType.Boolean:
					return value != 0;
				case VariantValueType.InlineText:
					return Worksheet.CellValueCache.GetInlineString(this);
				case VariantValueType.SharedString:
					return VariantValue.FromSharedString((int)value);
				case VariantValueType.Numeric:
					return value;
				case VariantValueType.Array:
				case VariantValueType.CellRange:
				default:
					Exceptions.ThrowInternalException();
					return VariantValue.Empty;
			}
		}
		#endregion
		public CellRange GetMergedCell() {
			return Worksheet.MergedCells.GetMergedCellRange(this);
		}
		public void ResetCachedContentVersions() {
			if (!Workbook.CalculationChain.Enabled || !MarkedForRecalculation)
				ContentVersion = 0;
		}
		public void ResetCachedTransactionVersions() {
			TransactionVersion = 0;
		}
		public override void OffsetColumnIndex(int offset) {
			DocumentHistory history = Workbook.History;
			CellOffsetColumnIndexHistoryItem historyItem = new CellOffsetColumnIndexHistoryItem(this, offset);
			history.Add(historyItem);
			historyItem.Execute();
		}
		public override void OffsetRowIndex(int offset, bool needChangeRow) {
			DocumentHistory history = Workbook.History;
			CellOffsetRowIndexHistoryItem historyItem = new CellOffsetRowIndexHistoryItem(this, offset, needChangeRow);
			history.Add(historyItem);
			historyItem.Execute();
		}
		#region Change notifications
		public void OnBeforeRemove() {
			UnassignValueCore();
		}
		public void OnAfterInsert() {
		}
		public void OnRangeRemovingShiftLeft(RemoveRangeNotificationContext notificationContext) {
			if (!HasFormula)
				return;
			WorkbookDataContext dataContext = Context;
			dataContext.PushCurrentCell(this);
			try {
				GetFormula().OnRangeRemovingShiftLeft(notificationContext, dataContext, this);
			}
			finally {
				dataContext.PopCurrentCell();
			}
		}
		public void OnRangeRemovingShiftUp(RemoveRangeNotificationContext notificationContext) {
			if (!HasFormula)
				return;
			WorkbookDataContext dataContext = Context;
			dataContext.PushCurrentCell(this);
			try {
				GetFormula().OnRangeRemovingShiftUp(notificationContext, dataContext, this);
			}
			finally {
				dataContext.PopCurrentCell();
			}
		}
		public void OnRangeRemovingNoShift(RemoveRangeNotificationContext notificationContext) {
			if (!HasFormula)
				return;
			WorkbookDataContext dataContext = Context;
			dataContext.PushCurrentCell(this);
			try {
				GetFormula().OnRangeRemovingNoShift(notificationContext, dataContext, this);
			}
			finally {
				dataContext.PopCurrentCell();
			}
		}
		public void OnBeforeSheetRemoved(RemoveRangeNotificationContext notificationContext) {
			WorkbookDataContext dataContext = Context;
			dataContext.PushCurrentCell(this);
			try {
				GetFormula().OnBeforeSheetRemoved(notificationContext, dataContext);
			}
			finally {
				dataContext.PopCurrentCell();
			}
		}
		public void OnRangeInsertingShiftRight(InsertRangeNotificationContext notificationContext) {
			if (!HasFormula)
				return;
			WorkbookDataContext dataContext = Context;
			dataContext.PushCurrentCell(this);
			try {
				GetFormula().OnRangeInsertingShiftRight(notificationContext, dataContext, this);
			}
			finally {
				dataContext.PopCurrentCell();
			}
		}
		public void OnRangeInsertingShiftDown(InsertRangeNotificationContext notificationContext) {
			if (!HasFormula)
				return;
			WorkbookDataContext dataContext = Context;
			dataContext.PushCurrentCell(this);
			try {
				GetFormula().OnRangeInsertingShiftDown(notificationContext, dataContext, this);
			}
			finally {
				dataContext.PopCurrentCell();
			}
		}
		#endregion
		public virtual string GetText() {
			VariantValue value = GetValue();
			if (HasError)
				return CellErrorFactory.GetErrorName(Error, Context);
			return ActualFormat.Format(value, Context).Text;
		}
		public NumberFormatResult GetFormatResult() {
			return GetFormatResult(NumberFormatParameters.Empty);
		}
		public virtual NumberFormatResult GetFormatResult(NumberFormatParameters parameters) {
			if (HasFormula && Worksheet.ActiveView.ShowFormulas) {
				NumberFormatResult result = new NumberFormatResult();
				result.Text = Worksheet.IsProtected && ActualProtection.Hidden ? String.Empty : FormulaBody;
				return result;
			}
			return GetFormatResultCore(ActualFormat, parameters);
		}
		public NumberFormatResult GetFormatResultCore(NumberFormat format, NumberFormatParameters parameters) {
			VariantValue value;
			value = GetValue();
			if (HasError) {
				string textResult = string.Empty;
				if (DocumentModel.ShowMailMergeRanges && HasFormula)
					textResult = FormulaBody;
				else
					textResult = CellErrorFactory.GetErrorName(Error, Context);
				NumberFormatResult result = new NumberFormatResult();
				result.Text = textResult;
				return result;
			}
			return format.Format(value, Context, parameters);
		}
		public void CopyFrom(ICell source) {
			this.value = source.DoubleValue;
			this.PackedValues = source.PackedValues;
			this.PackedFormat = source.PackedFormat;
		}
		public bool HasContent { get { return HasFormula || ValueType != VariantValueType.None; } }
		public bool ClearContent() {
			if (HasContent) {
				this.Value = VariantValue.Empty;
				return true;
			}
			return false;
		}
		public void ClearFormat() {
			ApplyFormatIndex(DocumentModel.StyleSheet.DefaultCellFormatIndex);
		}
		public void ClearFormat(CellFormatApplyOptions options) {
			DocumentModel.BeginUpdate();
			try {
				FormatBase info = GetInfoForModification();
				((CellFormat)info).ClearFormat(options);
				ReplaceInfo(info, DocumentModelChangeActions.None); 
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		public override string ToString() {
			string position = (new CellPosition(ColumnIndex, RowIndex)).ToString();
			FormulaBase formula = GetFormula();
			return String.Format("[{0}] v=[{1}], f=[{2}], sheet='{3}'", position, "", formula != null ? formula.GetBody(this) : String.Empty, Sheet.Name);
		}
		#region GetFormulaWithoutCustomFunctions
		public FormulaBase GetFormulaWithoutCustomFunctions(bool checkSimpleExpression) {
			if (!HasFormula)
				return null;
			FormulaBase formula = Formula;
			if ((formula.GetProperties() & FormulaProperties.HasCustomFunction) == 0)
				return formula;
			WorkbookDataContext context = Context;
			ParsedExpression updatedExpression;
			if (formula.Type == FormulaType.Shared) {
				SharedFormulaRef sharedFormulaRef = (SharedFormulaRef)formula;
				updatedExpression = sharedFormulaRef.GetNormalCellFormula(this);
				updatedExpression = ReplaceCustomFunctions(updatedExpression, checkSimpleExpression);
			}
			else {
				try {
					formula.PushSettingsToContext(context, this);
					updatedExpression = ReplaceCustomFunctions(formula.Expression, checkSimpleExpression);
				}
				finally {
					formula.PopSettingsFromContext(context);
				}
			}
			if (updatedExpression == null)
				return null;
			if (formula.Type == FormulaType.Array) {
				ArrayFormula arrayFormula = (ArrayFormula)formula;
				return new ArrayFormula(arrayFormula.Range, updatedExpression);
			}
			return new Formula(this, updatedExpression);
		}
		ParsedExpression ReplaceCustomFunctions(ParsedExpression expression, bool checkSimpleExpression) {
			WorkbookDataContext context = Context;
			CustomFunctionReplaceVisitor visitor = new CustomFunctionReplaceVisitor(context);
			context.PushCurrentCell(this);
			try {
				ParsedExpression processedExpression = visitor.Process(expression);
				if (checkSimpleExpression) {
					int expressionCount = processedExpression.Count;
					if (expressionCount == 1 && expression[0] is ValueParsedThing)
						return null;
					if (expressionCount == 2 && expression[0] is ParsedThingAttrSemi && expression[1] is ValueParsedThing)
						return null;
				}
				return processedExpression;
			}
			finally {
				context.PopCurrentCell();
			}
		}
		#endregion
		#region Check Integrity
		public override void CheckIntegrity(CheckIntegrityFlags flags) {
			base.CheckIntegrity(flags);
			if (Sheet.SheetId != Key.SheetId)
				IntegrityChecks.Fail(String.Format("Cell: Inconsistend cell: Sheet.SheetId={0}, Key.SheetId={1}", Sheet.SheetId, Key.SheetId));
			if ((flags & CheckIntegrityFlags.SkipTimeConsumingChecks) == 0) {
				Row row = Worksheet.Rows.TryGetRow(RowIndex);
				if (row == null)
					IntegrityChecks.Fail(String.Format("Cell: Inconsistend cell, unregistered row: RowIndex={0}", RowIndex));
				ICell cell = row.Cells.TryGetCell(ColumnIndex);
				if (cell == null)
					IntegrityChecks.Fail(String.Format("Cell: Inconsistend cell, unregistered cell: RowIndex={0}, ColumnIndex={1}", RowIndex, ColumnIndex));
			}
			ICell thisCell = Worksheet.GetRegisteredCell(Key.ColumnIndex, Key.RowIndex);
			if (!Object.ReferenceEquals(thisCell, this))
				IntegrityChecks.Fail("Cell: Object.ReferenceEquals(Sheet.Workbook.CalculationChain.GetRegisteredCell(Key), this) == false");
			if (HasFormula)
				GetFormula().CheckIntegrity(this);
			if (Style.IsHidden)
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorUseDeletedStyle);
		}
		#endregion
	}
	#endregion
	#region ICellPosition
	public interface ICellPosition {
		CellPosition GetCellPosition(int index);
	}
	#endregion
	#region ICellContainer
	public interface ICellContainer : ICellPosition {
		Worksheet Sheet { get; }
		ICellCollection Cells { get; }
	}
	#endregion
	public enum FormulaType {
		Normal = 0,
		Array = 1,
		ArrayPart = 2,
		Shared = 3,
		None = 5,
	}
	#region FormulaNumberFormatIdCalculator
	public class FormulaNumberFormatIdCalculator : StackMachineVisitor<int> {
		static readonly int dateCode = FormulaCalculator.GetFunctionByInvariantName("DATE").Code;
		static readonly int timeCode = FormulaCalculator.GetFunctionByInvariantName("TIME").Code;
		static readonly int nowCode = FormulaCalculator.GetFunctionByInvariantName("NOW").Code;
		static readonly int todayCode = FormulaCalculator.GetFunctionByInvariantName("TODAY").Code;
		static readonly HashSet<ParsedThingBase> nonDateTimeBinaryOperators = FillNonDateTimeBinaryOperators();
		static readonly HashSet<int> functionsDoNoAffectToDateTime = FillFunctionsDoNoAffectToDateTime();
		private static HashSet<int> FillFunctionsDoNoAffectToDateTime() {
			HashSet<int> result = new HashSet<int>();
			result.Add(FormulaCalculator.GetFunctionByInvariantName("SUM").Code);
			result.Add(FormulaCalculator.GetFunctionByInvariantName("AVERAGE").Code);
			result.Add(FormulaCalculator.GetFunctionByInvariantName("ABS").Code);
			return result;
		}
		static HashSet<ParsedThingBase> FillNonDateTimeBinaryOperators() {
			HashSet<ParsedThingBase> result = new HashSet<ParsedThingBase>();
			result.Add(ParsedThingMultiply.Instance);
			result.Add(ParsedThingConcat.Instance);
			result.Add(ParsedThingDivide.Instance);
			result.Add(ParsedThingPower.Instance);
			return result;
		}
		protected override int ProcessBinaryValue(ParsedThingBase thing, int left, int right) {
			int maxValue = Math.Max(left, right);
			if (maxValue == 0)
				return 0;
			if (left != 0 && right != 0)
				return 0; 
			if (nonDateTimeBinaryOperators.Contains(thing))
				return 0;
			return maxValue;
		}
		protected override int ProcessFunction(ParsedThingFunc thing, List<int> arguments) {
			int funcCode = thing.FuncCode;
			if (funcCode == dateCode)
				return 14;
			if (funcCode == timeCode)
				return 18;
			if (funcCode == nowCode)
				return 22;
			if (funcCode == todayCode)
				return 14;
			int nonZeroValue = 0;
			for (int i = 0; i < arguments.Count; i++) {
				nonZeroValue = arguments[i];
				if (nonZeroValue > 0)
					break;
			}
			if (nonZeroValue > 0 && functionsDoNoAffectToDateTime.Contains(funcCode))
				return nonZeroValue;
			return 0;
		}
		protected override int ProcessOperand(ParsedThingBase thing) {
			return 0;
		}
		protected override int ProcessUnaryValue(ParsedThingBase thing, int value) {
			return value;
		}
	}
	#endregion
	#region CellContentSnapshot
	public class CellContentSnapshot {
		readonly ICell cell;
		readonly VariantValue value;
		readonly string formula;
		readonly string formulaInvariant;
		public CellContentSnapshot(ICell cell) {
			this.cell = cell;
			if (cell.HasFormula) 
				this.value = cell.ExtractValue();  
			else 
				this.value = cell.Value;
			this.formula = cell.FormulaBody;
			this.formulaInvariant = cell.FormulaBodyInvariant;
		}
		public ICell Cell { get { return cell; } }
		public VariantValue Value { get { return value; } }
		public string Formula { get { return formula; } }
		public string FormulaInvariant { get { return formulaInvariant; } }
	}
	#endregion
	#region CellContentSnapshot
	public class CellContentSnapshotWithFormat : CellContentSnapshot {
		readonly NumberFormat format;
		public CellContentSnapshotWithFormat(ICell cell)
			: base(cell) {
			this.format = cell.ActualFormat;
		}
		public NumberFormat Format { get { return format; } }
	}
	#endregion
}
