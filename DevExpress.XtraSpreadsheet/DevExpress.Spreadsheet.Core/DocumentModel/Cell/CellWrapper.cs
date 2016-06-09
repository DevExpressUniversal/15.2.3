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

using DevExpress.Office.Drawing;
using DevExpress.Utils;
using DevExpress.Export.Xl;
using System;
using System.Collections.Generic;
using System.Drawing;
using DevExpress.Compatibility.System.Drawing;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	#region CellWrapper
	public abstract class CellWrapper : ICell {
		readonly ICell innerCell;
		protected CellWrapper(ICell cell) {
			this.innerCell = cell;
		}
		public ICell InnerCell { get { return innerCell; } }
		public void SetValue(VariantValue value) {
			innerCell.SetValue(value);
		}
		public virtual VariantValue GetValue() {
			return innerCell.GetValue();
		}
		public void CellCopyFormatSameDocumentModels(ICell source) {
			innerCell.CellCopyFormatSameDocumentModels(source);
		}
		public bool HasNoValue {
			get { return innerCell.HasNoValue; }
		}
		public bool HasError {
			get { return innerCell.HasError; }
		}
		public ICellError Error {
			get { return innerCell.Error; }
		}
		public bool ShouldUseInLayout {
			get { return innerCell.ShouldUseInLayout; }
		}
		public int ContentVersion {
			get {
				return innerCell.ContentVersion;
			}
			set {
				innerCell.ContentVersion = value;
			}
		}
		public int TransactionVersion {
			get {
				return innerCell.TransactionVersion;
			}
			set {
				innerCell.TransactionVersion = value;
			}
		}
		public CellCycleCheckState CycleCheckState {
			get {
				return innerCell.CycleCheckState;
			}
			set {
				innerCell.CycleCheckState = value;
			}
		}
		public DocumentModel DocumentModel {
			get { return innerCell.DocumentModel; }
		}
		public bool HasFormula {
			get { return innerCell.HasFormula; }
		}
		public string FormulaBody {
			get {
				return innerCell.FormulaBody;
			}
			set {
				innerCell.FormulaBody = value;
			}
		}
		public string FormulaBodyInvariant {
			get { return innerCell.FormulaBodyInvariant; }
		}
		public FormulaBase Formula {
			get { return innerCell.Formula; }
		}
		public string DisplayText { get { return innerCell.DisplayText; } }
		public string Text {
			get {
				return innerCell.Text;
			}
			set {
				innerCell.Text = value;
			}
		}
		public virtual Worksheet Worksheet {
			get { return innerCell.Worksheet; }
		}
		public WorkbookDataContext Context {
			get { return innerCell.Context; }
		}
		public bool HasRichTextContent {
			get { return innerCell.HasRichTextContent; }
		}
		public FormulaType FormulaType {
			get { return innerCell.FormulaType; }
		}
		public void SetTextSmart(string text) {
			innerCell.SetTextSmart(text);
		}
		public void SetFormattedValue(FormattedVariantValue value, string text) {
			innerCell.SetFormattedValue(value, text);
		}
		public void SetFormattedValue(FormattedVariantValue value, string text, SetValueDelegate setValue) {
			innerCell.SetFormattedValue(value, text, setValue);
		}
		public FormulaBase GetFormula() {
			return innerCell.GetFormula();
		}
		public FormulaBase GetFormula(FormulaType formulaType) {
			return innerCell.GetFormula(formulaType);
		}
		public void SetFormula(FormulaBase formula) {
			innerCell.SetFormula(formula);
		}
		public void SetFormulaCore(FormulaBase formula) {
			innerCell.SetFormulaCore(formula);
		}
		public void SetFormulaBody(string formulaBody) {
			innerCell.SetFormulaBody(formulaBody);
		}
		public void ReplaceValueByValueCore(VariantValue value) {
			innerCell.ReplaceValueByValueCore(value);
		}
		public void SetValueCore(VariantValue value) {
			innerCell.SetValueCore(value);
		}
		public void SetValueNoChecks(VariantValue value) {
			innerCell.SetValueNoChecks(value);
		}
		public void ApplyFormula(FormulaBase formula) {
			innerCell.ApplyFormula(formula);
		}
		public void ApplyFormulaCore(FormulaBase formula) {
			innerCell.ApplyFormulaCore(formula);
		}
		public void TransactedSetFormula(FormulaBase formula) {
			innerCell.TransactedSetFormula(formula);
		}
		public void SetBinaryFormula(byte[] value) {
			innerCell.SetBinaryFormula(value);
		}
		public bool IsValueUpdated() {
			return innerCell.IsValueUpdated();
		}
		public VariantValue ExtractValue() {
			return innerCell.ExtractValue();
		}
		public void TransactedSetValue(VariantValue value) {
			innerCell.TransactedSetValue(value);
		}
		public bool HasCircularReferences() {
			return innerCell.HasCircularReferences();
		}
		public void CheckCircularReferences() {
			innerCell.CheckCircularReferences();
		}
		public CellRange GetMergedCell() {
			return innerCell.GetMergedCell();
		}
		public void ResetCachedContentVersions() {
			innerCell.ResetCachedContentVersions();
		}
		public void ResetCachedTransactionVersions() {
			innerCell.ResetCachedTransactionVersions();
		}
		public void OnBeforeRemove() {
			innerCell.OnBeforeRemove();
		}
		public void OnAfterInsert() {
			innerCell.OnAfterInsert();
		}
		public void OnRangeRemovingShiftLeft(RemoveRangeNotificationContext notificationContext) {
			innerCell.OnRangeRemovingShiftLeft(notificationContext);
		}
		public void OnRangeRemovingShiftUp(RemoveRangeNotificationContext notificationContext) {
			innerCell.OnRangeRemovingShiftUp(notificationContext);
		}
		public void OnRangeRemovingNoShift(RemoveRangeNotificationContext notificationContext) {
			innerCell.OnRangeRemovingNoShift(notificationContext);
		}
		public void OnBeforeSheetRemoved(RemoveRangeNotificationContext notificationContext) {
			innerCell.OnBeforeSheetRemoved(notificationContext);
		}
		public void OnRangeInsertingShiftRight(InsertRangeNotificationContext notificationContext) {
			innerCell.OnRangeInsertingShiftRight(notificationContext);
		}
		public void OnRangeInsertingShiftDown(InsertRangeNotificationContext notificationContext) {
			innerCell.OnRangeInsertingShiftDown(notificationContext);
		}
		public string GetText() {
			return innerCell.GetText();
		}
		public ConditionalFormattingFormatAccumulator ConditionalFormatAccumulator {
			get {
				return innerCell.ConditionalFormatAccumulator;
			}
			set {
				innerCell.ConditionalFormatAccumulator = value;
			}
		}
		public NumberFormatResult GetFormatResult() {
			return innerCell.GetFormatResult();
		}
		public NumberFormatResult GetFormatResult(NumberFormatParameters parameters) {
			return innerCell.GetFormatResult(parameters);
		}
		public bool HasContent {
			get { return innerCell.HasContent; }
		}
		public bool ClearContent() {
			return innerCell.ClearContent();
		}
		public void ClearFormat() {
			innerCell.ClearFormat();
		}
		public void ClearFormat(CellFormatApplyOptions options) {
			innerCell.ClearFormat(options);
		}
		public void ApplyFormatIndex(int formatIndex) {
			innerCell.ApplyFormatIndex(formatIndex);
		}
		public void ApplyFormat(CellFormat format, CellFormatApplyOptions options) {
			innerCell.ApplyFormat(format, options);
		}
		public FormulaBase GetFormulaWithoutCustomFunctions(bool checkSimpleExpression) {
			return innerCell.GetFormulaWithoutCustomFunctions(checkSimpleExpression);
		}
		public int ConditionalFormattingStoppedAtPriority {
			get { return innerCell.ConditionalFormattingStoppedAtPriority; }
		}
		public void RemoveStyle() {
			innerCell.RemoveStyle();
		}
		public NumberFormatResult GetFormatResultCore(NumberFormat format, NumberFormatParameters parameters) {
			return innerCell.GetFormatResultCore(format, parameters);
		}
		public DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return innerCell.GetBatchUpdateChangeActions();
		}
		public bool ChangeFormatIndex(int newIndex, DocumentModelChangeActions changeActions) {
			return innerCell.ChangeFormatIndex(newIndex, changeActions);
		}
		public void SetCellFormatIndex(int value) {
			innerCell.SetCellFormatIndex(value);
		}
		public CellFormat FormatInfo {
			get { return innerCell.FormatInfo; }
		}
		public short FormatIndex {
			get { return innerCell.FormatIndex; }
		}
		public void ApplyFormat(CellFormatBase format) {
			innerCell.ApplyFormat(format);
		}
		public NumberFormat Format {
			get { return innerCell.Format; }
		}
		public bool QuotePrefix {
			get {
				return innerCell.QuotePrefix;
			}
			set {
				innerCell.QuotePrefix = value;
			}
		}
		public XlPatternType ActualPatternType {
			get { return innerCell.ActualPatternType; }
		}
		public Color ActualBackgroundColor {
			get { return innerCell.ActualBackgroundColor; }
		}
		public Color ActualForegroundColor {
			get { return innerCell.ActualForegroundColor; }
		}
		public bool HasVisibleFill {
			get { return innerCell.HasVisibleFill; }
		}
		public NumberFormat ActualFormat {
			get { return innerCell.ActualFormat; }
		}
		public IActualGradientFillInfo ActualGradientFillInfo {
			get { return innerCell.ActualGradientFillInfo; }
		}
		public IActualConvergenceInfo ActualConvergenceInfo {
			get { return innerCell.ActualConvergenceInfo; }
		}
		public XlHorizontalAlignment ActualHorizontalAlignment {
			get { return innerCell.ActualHorizontalAlignment; }
		}
		public IActualCellAlignmentInfo InnerActualAlignment {
			get { return innerCell.InnerActualAlignment; }
		}
		public IActualFillInfo InnerActualFill {
			get { return innerCell.InnerActualFill; }
		}
		public IActualGradientFillInfo InnerActualGradientFillInfo {
			get { return innerCell.InnerActualGradientFillInfo; }
		}
		public IActualConvergenceInfo InnerActualConvergenceInfo {
			get { return innerCell.InnerActualConvergenceInfo; }
		}
		public IActualRunFontInfo InnerActualFont {
			get { return innerCell.InnerActualFont; }
		}
		public IActualCellProtectionInfo InnerActualProtection {
			get { return innerCell.InnerActualProtection; }
		}
		public bool MarkedForRecalculation {
			get {
				return innerCell.MarkedForRecalculation;
			}
			set {
				innerCell.MarkedForRecalculation = value;
			}
		}
		public VariantValue CalculateFormulaValue() {
			return innerCell.CalculateFormulaValue();
		}
		public FormulaInfo FormulaInfo {
			get {
				return innerCell.FormulaInfo;
			}
			set {
				innerCell.FormulaInfo = value;
			}
		}
		public CellKey Key {
			get { return innerCell.Key; }
		}
		public int ColumnIndex {
			get { return innerCell.ColumnIndex; }
		}
		public int RowIndex {
			get { return innerCell.RowIndex; }
		}
		public virtual IWorksheet Sheet {
			get { return innerCell.Sheet; }
		}
		public VariantValue Value {
			get {
				return innerCell.Value;
			}
			set {
				innerCell.Value = value;
			}
		}
		public CellPosition Position {
			get { return innerCell.Position; }
		}
		public CellRange GetRange() {
			return innerCell.GetRange();
		}
		public void AssignValue(VariantValue value) {
			innerCell.AssignValue(value);
		}
		public void AssignValueCore(VariantValue value) {
			innerCell.AssignValueCore(value);
		}
		public bool IsVisible() {
			return innerCell.IsVisible();
		}
		public void OffsetRowIndex(int offset, bool needChangeRow) {
			innerCell.OffsetRowIndex(offset, needChangeRow);
		}
		public void OffsetRowIndexCore(int offset, bool needChangeRow) {
			innerCell.OffsetRowIndexCore(offset, needChangeRow);
		}
		public void OffsetRowIndexInternal(int offset, bool needChangeRow) {
			innerCell.OffsetRowIndexInternal(offset, needChangeRow);
		}
		public void OffsetColumnIndex(int offset) {
			innerCell.OffsetColumnIndex(offset);
		}
		public void OffsetColumnIndexCore(int offset) {
			innerCell.OffsetColumnIndexCore(offset);
		}
		public void CheckIntegrity(CheckIntegrityFlags flags) {
			innerCell.CheckIntegrity(flags);
		}
		public virtual int SheetId {
			get { return innerCell.SheetId; }
		}
		public int LeftColumnIndex {
			get { return innerCell.LeftColumnIndex; }
		}
		public int RightColumnIndex {
			get { return innerCell.RightColumnIndex; }
		}
		public int TopRowIndex {
			get { return innerCell.TopRowIndex; }
		}
		public int BottomRowIndex {
			get { return innerCell.BottomRowIndex; }
		}
		public CellStyleBase Style {
			get {
				return innerCell.Style;
			}
			set {
				innerCell.Style = value;
			}
		}
		public IRunFontInfo Font {
			get { return innerCell.Font; }
		}
		public ICellAlignmentInfo Alignment {
			get { return innerCell.Alignment; }
		}
		public IBorderInfo Border {
			get { return innerCell.Border; }
		}
		public IFillInfo Fill {
			get { return innerCell.Fill; }
		}
		public ICellProtectionInfo Protection {
			get { return innerCell.Protection; }
		}
		string IRunFontInfo.Name {
			get {
				return ((IRunFontInfo)innerCell).Name;
			}
			set {
				((IRunFontInfo)innerCell).Name = value;
			}
		}
		Color IRunFontInfo.Color {
			get {
				return ((IRunFontInfo)innerCell).Color;
			}
			set {
				((IRunFontInfo)innerCell).Color = value;
			}
		}
		bool IRunFontInfo.Bold {
			get {
				return ((IRunFontInfo)innerCell).Bold;
			}
			set {
				((IRunFontInfo)innerCell).Bold = value;
			}
		}
		bool IRunFontInfo.Condense {
			get {
				return ((IRunFontInfo)innerCell).Condense;
			}
			set {
				((IRunFontInfo)innerCell).Condense = value;
			}
		}
		bool IRunFontInfo.Extend {
			get {
				return ((IRunFontInfo)innerCell).Extend;
			}
			set {
				((IRunFontInfo)innerCell).Extend = value;
			}
		}
		bool IRunFontInfo.Italic {
			get {
				return ((IRunFontInfo)innerCell).Italic;
			}
			set {
				((IRunFontInfo)innerCell).Italic = value;
			}
		}
		bool IRunFontInfo.Outline {
			get {
				return ((IRunFontInfo)innerCell).Outline;
			}
			set {
				((IRunFontInfo)innerCell).Outline = value;
			}
		}
		bool IRunFontInfo.Shadow {
			get {
				return ((IRunFontInfo)innerCell).Shadow;
			}
			set {
				((IRunFontInfo)innerCell).Shadow = value;
			}
		}
		bool IRunFontInfo.StrikeThrough {
			get {
				return ((IRunFontInfo)innerCell).StrikeThrough;
			}
			set {
				((IRunFontInfo)innerCell).StrikeThrough = value;
			}
		}
		int IRunFontInfo.Charset {
			get {
				return ((IRunFontInfo)innerCell).Charset;
			}
			set {
				((IRunFontInfo)innerCell).Charset = value;
			}
		}
		int IRunFontInfo.FontFamily {
			get {
				return ((IRunFontInfo)innerCell).FontFamily;
			}
			set {
				((IRunFontInfo)innerCell).FontFamily = value;
			}
		}
		double IRunFontInfo.Size {
			get {
				return ((IRunFontInfo)innerCell).Size;
			}
			set {
				((IRunFontInfo)innerCell).Size = value;
			}
		}
		XlFontSchemeStyles IRunFontInfo.SchemeStyle {
			get {
				return ((IRunFontInfo)innerCell).SchemeStyle;
			}
			set {
				((IRunFontInfo)innerCell).SchemeStyle = value;
			}
		}
		XlScriptType IRunFontInfo.Script {
			get {
				return ((IRunFontInfo)innerCell).Script;
			}
			set {
				((IRunFontInfo)innerCell).Script = value;
			}
		}
		XlUnderlineType IRunFontInfo.Underline {
			get {
				return ((IRunFontInfo)innerCell).Underline;
			}
			set {
				((IRunFontInfo)innerCell).Underline = value;
			}
		}
		bool ICellAlignmentInfo.WrapText {
			get {
				return ((ICellAlignmentInfo)innerCell).WrapText;
			}
			set {
				((ICellAlignmentInfo)innerCell).WrapText = value;
			}
		}
		bool ICellAlignmentInfo.JustifyLastLine {
			get {
				return ((ICellAlignmentInfo)innerCell).JustifyLastLine;
			}
			set {
				((ICellAlignmentInfo)innerCell).JustifyLastLine = value;
			}
		}
		bool ICellAlignmentInfo.ShrinkToFit {
			get {
				return ((ICellAlignmentInfo)innerCell).ShrinkToFit;
			}
			set {
				((ICellAlignmentInfo)innerCell).ShrinkToFit = value;
			}
		}
		int ICellAlignmentInfo.TextRotation {
			get {
				return ((ICellAlignmentInfo)innerCell).TextRotation;
			}
			set {
				((ICellAlignmentInfo)innerCell).TextRotation = value;
			}
		}
		byte ICellAlignmentInfo.Indent {
			get {
				return ((ICellAlignmentInfo)innerCell).Indent;
			}
			set {
				((ICellAlignmentInfo)innerCell).Indent = value;
			}
		}
		int ICellAlignmentInfo.RelativeIndent {
			get {
				return ((ICellAlignmentInfo)innerCell).RelativeIndent;
			}
			set {
				((ICellAlignmentInfo)innerCell).RelativeIndent = value;
			}
		}
		XlHorizontalAlignment ICellAlignmentInfo.Horizontal {
			get {
				return ((ICellAlignmentInfo)innerCell).Horizontal;
			}
			set {
				((ICellAlignmentInfo)innerCell).Horizontal = value;
			}
		}
		XlVerticalAlignment ICellAlignmentInfo.Vertical {
			get {
				return ((ICellAlignmentInfo)innerCell).Vertical;
			}
			set {
				((ICellAlignmentInfo)innerCell).Vertical = value;
			}
		}
		XlReadingOrder ICellAlignmentInfo.ReadingOrder {
			get {
				return ((ICellAlignmentInfo)innerCell).ReadingOrder;
			}
			set {
				((ICellAlignmentInfo)innerCell).ReadingOrder = value;
			}
		}
		XlBorderLineStyle IBorderInfo.LeftLineStyle {
			get {
				return ((IBorderInfo)innerCell).LeftLineStyle;
			}
			set {
				((IBorderInfo)innerCell).LeftLineStyle = value;
			}
		}
		XlBorderLineStyle IBorderInfo.RightLineStyle {
			get {
				return ((IBorderInfo)innerCell).RightLineStyle;
			}
			set {
				((IBorderInfo)innerCell).RightLineStyle = value;
			}
		}
		XlBorderLineStyle IBorderInfo.TopLineStyle {
			get {
				return ((IBorderInfo)innerCell).TopLineStyle;
			}
			set {
				((IBorderInfo)innerCell).TopLineStyle = value;
			}
		}
		XlBorderLineStyle IBorderInfo.BottomLineStyle {
			get {
				return ((IBorderInfo)innerCell).BottomLineStyle;
			}
			set {
				((IBorderInfo)innerCell).BottomLineStyle = value;
			}
		}
		XlBorderLineStyle IBorderInfo.DiagonalUpLineStyle {
			get {
				return ((IBorderInfo)innerCell).DiagonalUpLineStyle;
			}
			set {
				((IBorderInfo)innerCell).DiagonalUpLineStyle = value;
			}
		}
		XlBorderLineStyle IBorderInfo.DiagonalDownLineStyle {
			get {
				return ((IBorderInfo)innerCell).DiagonalDownLineStyle;
			}
			set {
				((IBorderInfo)innerCell).DiagonalDownLineStyle = value;
			}
		}
		XlBorderLineStyle IBorderInfo.HorizontalLineStyle {
			get {
				return ((IBorderInfo)innerCell).HorizontalLineStyle;
			}
			set {
				((IBorderInfo)innerCell).HorizontalLineStyle = value;
			}
		}
		XlBorderLineStyle IBorderInfo.VerticalLineStyle {
			get {
				return ((IBorderInfo)innerCell).VerticalLineStyle;
			}
			set {
				((IBorderInfo)innerCell).VerticalLineStyle = value;
			}
		}
		Color IBorderInfo.LeftColor {
			get {
				return ((IBorderInfo)innerCell).LeftColor;
			}
			set {
				((IBorderInfo)innerCell).LeftColor = value;
			}
		}
		Color IBorderInfo.RightColor {
			get {
				return ((IBorderInfo)innerCell).RightColor;
			}
			set {
				((IBorderInfo)innerCell).RightColor = value;
			}
		}
		Color IBorderInfo.TopColor {
			get {
				return ((IBorderInfo)innerCell).TopColor;
			}
			set {
				((IBorderInfo)innerCell).TopColor = value;
			}
		}
		Color IBorderInfo.BottomColor {
			get {
				return ((IBorderInfo)innerCell).BottomColor;
			}
			set {
				((IBorderInfo)innerCell).BottomColor = value;
			}
		}
		Color IBorderInfo.DiagonalColor {
			get {
				return ((IBorderInfo)innerCell).DiagonalColor;
			}
			set {
				((IBorderInfo)innerCell).DiagonalColor = value;
			}
		}
		Color IBorderInfo.HorizontalColor {
			get {
				return ((IBorderInfo)innerCell).HorizontalColor;
			}
			set {
				((IBorderInfo)innerCell).HorizontalColor = value;
			}
		}
		Color IBorderInfo.VerticalColor {
			get {
				return ((IBorderInfo)innerCell).VerticalColor;
			}
			set {
				((IBorderInfo)innerCell).VerticalColor = value;
			}
		}
		int IBorderInfo.LeftColorIndex {
			get {
				return ((IBorderInfo)innerCell).LeftColorIndex;
			}
			set {
				((IBorderInfo)innerCell).LeftColorIndex = value;
			}
		}
		int IBorderInfo.RightColorIndex {
			get {
				return ((IBorderInfo)innerCell).RightColorIndex;
			}
			set {
				((IBorderInfo)innerCell).RightColorIndex = value;
			}
		}
		int IBorderInfo.TopColorIndex {
			get {
				return ((IBorderInfo)innerCell).TopColorIndex;
			}
			set {
				((IBorderInfo)innerCell).TopColorIndex = value;
			}
		}
		int IBorderInfo.BottomColorIndex {
			get {
				return ((IBorderInfo)innerCell).BottomColorIndex;
			}
			set {
				((IBorderInfo)innerCell).BottomColorIndex = value;
			}
		}
		int IBorderInfo.DiagonalColorIndex {
			get {
				return ((IBorderInfo)innerCell).DiagonalColorIndex;
			}
			set {
				((IBorderInfo)innerCell).DiagonalColorIndex = value;
			}
		}
		int IBorderInfo.HorizontalColorIndex {
			get {
				return ((IBorderInfo)innerCell).HorizontalColorIndex;
			}
			set {
				((IBorderInfo)innerCell).HorizontalColorIndex = value;
			}
		}
		int IBorderInfo.VerticalColorIndex {
			get {
				return ((IBorderInfo)innerCell).VerticalColorIndex;
			}
			set {
				((IBorderInfo)innerCell).VerticalColorIndex = value;
			}
		}
		void IFillInfo.Clear() {
			((IFillInfo)innerCell).Clear();
		}
		XlPatternType IFillInfo.PatternType {
			get {
				return ((IFillInfo)innerCell).PatternType;
			}
			set {
				((IFillInfo)innerCell).PatternType = value;
			}
		}
		Color IFillInfo.ForeColor {
			get {
				return ((IFillInfo)innerCell).ForeColor;
			}
			set {
				((IFillInfo)innerCell).ForeColor = value;
			}
		}
		Color IFillInfo.BackColor {
			get {
				return ((IFillInfo)innerCell).BackColor;
			}
			set {
				((IFillInfo)innerCell).BackColor = value;
			}
		}
		IGradientFillInfo IFillInfo.GradientFill {
			get { return ((IFillInfo)innerCell).GradientFill; }
		}
		ModelFillType IFillInfo.FillType {
			get {
				return ((IFillInfo)innerCell).FillType;
			}
			set {
				((IFillInfo)innerCell).FillType = value;
			}
		}
		bool ICellProtectionInfo.Locked {
			get {
				return ((ICellProtectionInfo)innerCell).Locked;
			}
			set {
				((ICellProtectionInfo)innerCell).Locked = value;
			}
		}
		bool ICellProtectionInfo.Hidden {
			get {
				return ((ICellProtectionInfo)innerCell).Hidden;
			}
			set {
				((ICellProtectionInfo)innerCell).Hidden = value;
			}
		}
		public string FormatString {
			get {
				return innerCell.FormatString;
			}
			set {
				innerCell.FormatString = value;
			}
		}
		ModelGradientFillType IGradientFillInfo.Type {
			get {
				return ((IGradientFillInfo)innerCell).Type;
			}
			set {
				((IGradientFillInfo)innerCell).Type = value;
			}
		}
		IConvergenceInfo IGradientFillInfo.Convergence {
			get { return ((IGradientFillInfo)innerCell).Convergence; }
		}
		double IGradientFillInfo.Degree {
			get {
				return ((IGradientFillInfo)innerCell).Degree;
			}
			set {
				((IGradientFillInfo)innerCell).Degree = value;
			}
		}
		IGradientStopCollection IGradientFillInfo.GradientStops {
			get { return ((IGradientFillInfo)innerCell).GradientStops; }
		}
		float IConvergenceInfo.Left {
			get {
				return ((IConvergenceInfo)innerCell).Left;
			}
			set {
				((IConvergenceInfo)innerCell).Left = value;
			}
		}
		float IConvergenceInfo.Right {
			get {
				return ((IConvergenceInfo)innerCell).Right;
			}
			set {
				((IConvergenceInfo)innerCell).Right = value;
			}
		}
		float IConvergenceInfo.Top {
			get {
				return ((IConvergenceInfo)innerCell).Top;
			}
			set {
				((IConvergenceInfo)innerCell).Top = value;
			}
		}
		float IConvergenceInfo.Bottom {
			get {
				return ((IConvergenceInfo)innerCell).Bottom;
			}
			set {
				((IConvergenceInfo)innerCell).Bottom = value;
			}
		}
		public IActualRunFontInfo ActualFont {
			get { return innerCell.ActualFont; }
		}
		public IActualCellAlignmentInfo ActualAlignment {
			get { return innerCell.ActualAlignment; }
		}
		public IActualBorderInfo ActualBorder {
			get { return innerCell.ActualBorder; }
		}
		public IActualFillInfo ActualFill {
			get { return innerCell.ActualFill; }
		}
		public IActualCellProtectionInfo ActualProtection {
			get { return innerCell.ActualProtection; }
		}
		public string ActualFormatString {
			get { return innerCell.ActualFormatString; }
		}
		public int ActualFormatIndex {
			get { return innerCell.ActualFormatIndex; }
		}
		public IActualApplyInfo ActualApplyInfo {
			get { return innerCell.ActualApplyInfo; }
		}
		public int ColorIndex {
			get { return innerCell.ColorIndex; }
		}
		public FontInfo GetFontInfo() {
			return innerCell.GetFontInfo();
		}
		public int ForeColorIndex {
			get { return innerCell.ForeColorIndex; }
		}
		public int BackColorIndex {
			get { return innerCell.BackColorIndex; }
		}
		public bool ApplyPatternType {
			get { return innerCell.ApplyPatternType; }
		}
		public bool ApplyBackColor {
			get { return innerCell.ApplyBackColor; }
		}
		public bool ApplyForeColor {
			get { return innerCell.ApplyForeColor; }
		}
		public bool IsDifferential {
			get { return innerCell.IsDifferential; }
		}
		IActualGradientFillInfo IActualFillInfo.GradientFill {
			get { return ((IActualFillInfo)innerCell).GradientFill; }
		}
		IActualConvergenceInfo IActualGradientFillInfo.Convergence {
			get { return ((IActualGradientFillInfo)innerCell).Convergence; }
		}
		IActualGradientStopCollection IActualGradientFillInfo.GradientStops {
			get { return ((IActualGradientFillInfo)innerCell).GradientStops; }
		}
		bool IChainPrecedent.AllowsMerging {
			get { return ((IChainPrecedent)innerCell).AllowsMerging; }
		}
		void IChainPrecedent.MarkUpForRecalculation() {
			((IChainPrecedent)innerCell).MarkUpForRecalculation();
		}
		void IChainPrecedent.MergeWith(IChainPrecedent item) {
			((IChainPrecedent)innerCell).MergeWith(item);
		}
		void IChainPrecedent.AddItemsTo(IList<ICell> where, ISheetPosition affectedRange) {
			((IChainPrecedent)innerCell).AddItemsTo(where, affectedRange);
		}
		CellRangeBase IChainPrecedent.ToRange(ISheetPosition affectedRange) {
			return ((IChainPrecedent)innerCell).ToRange(affectedRange);
		}
		bool IChainPrecedent.Remove(IChainPrecedent cell) {
			return ((IChainPrecedent)innerCell).Remove(cell);
		}
		bool ICalculationWaiter.TryInsertInto(CellsChain where, ICell position) {
			return ((ICalculationWaiter)innerCell).TryInsertInto(where, position);
		}
		void ICalculationWaiter.AddToTheEndAndMarkUp(CellsChain where) {
			((ICalculationWaiter)innerCell).AddToTheEndAndMarkUp(where);
		}
		void ICalculationWaiter.MergeWith(ICalculationWaiter waiter) {
			((ICalculationWaiter)innerCell).MergeWith(waiter);
		}
		#region IAutoFilterValue Members
		bool IAutoFilterValue.IsDateTime { get { return Format.IsDateTime; } }
		#endregion
		public void OnBeginUpdate() {
			innerCell.OnBeginUpdate();
		}
		public void OnCancelUpdate() {
			innerCell.OnCancelUpdate();
		}
		public void OnEndUpdate() {
			innerCell.OnEndUpdate();
		}
		public void OnFirstBeginUpdate() {
			innerCell.OnFirstBeginUpdate();
		}
		public void OnLastCancelUpdate() {
			innerCell.OnLastCancelUpdate();
		}
		public void OnLastEndUpdate() {
			innerCell.OnLastEndUpdate();
		}
		public Office.IDocumentModelPart DocumentModelPart {
			get { return innerCell.DocumentModelPart; }
		}
		public int GetIndex() {
			return innerCell.GetIndex();
		}
		public void SetIndex(int index, DocumentModelChangeActions changeActions) {
			innerCell.SetIndex(index, changeActions);
		}
		public bool ApplyFont {
			get {
				return innerCell.ApplyFont;
			}
			set {
				innerCell.ApplyFont = value;
			}
		}
		public bool ApplyAlignment {
			get {
				return innerCell.ApplyAlignment;
			}
			set {
				innerCell.ApplyAlignment = value;
			}
		}
		public bool ApplyBorder {
			get {
				return innerCell.ApplyBorder;
			}
			set {
				innerCell.ApplyBorder = value;
			}
		}
		public bool ApplyFill {
			get {
				return innerCell.ApplyFill;
			}
			set {
				innerCell.ApplyFill = value;
			}
		}
		public bool ApplyProtection {
			get {
				return innerCell.ApplyProtection;
			}
			set {
				innerCell.ApplyProtection = value;
			}
		}
		public bool ApplyNumberFormat {
			get {
				return innerCell.ApplyNumberFormat;
			}
			set {
				innerCell.ApplyNumberFormat = value;
			}
		}
		public BatchUpdateHelper BatchUpdateHelper {
			get { return innerCell.BatchUpdateHelper; }
		}
		public void BeginUpdate() {
			innerCell.BeginUpdate();
		}
		public void CancelUpdate() {
			innerCell.CancelUpdate();
		}
		public void EndUpdate() {
			innerCell.EndUpdate();
		}
		public bool IsUpdateLocked {
			get { return innerCell.IsUpdateLocked; }
		}
		XlBorderLineStyle IActualBorderInfo.LeftLineStyle {
			get { return ((IActualBorderInfo)innerCell).LeftLineStyle; }
		}
		XlBorderLineStyle IActualBorderInfo.RightLineStyle {
			get { return ((IActualBorderInfo)innerCell).RightLineStyle; }
		}
		XlBorderLineStyle IActualBorderInfo.TopLineStyle {
			get { return ((IActualBorderInfo)innerCell).TopLineStyle; }
		}
		XlBorderLineStyle IActualBorderInfo.BottomLineStyle {
			get { return ((IActualBorderInfo)innerCell).BottomLineStyle; }
		}
		XlBorderLineStyle IActualBorderInfo.DiagonalUpLineStyle {
			get { return ((IActualBorderInfo)innerCell).DiagonalUpLineStyle; }
		}
		XlBorderLineStyle IActualBorderInfo.DiagonalDownLineStyle {
			get { return ((IActualBorderInfo)innerCell).DiagonalDownLineStyle; }
		}
		XlBorderLineStyle IActualBorderInfo.HorizontalLineStyle {
			get { return ((IActualBorderInfo)innerCell).HorizontalLineStyle; }
		}
		XlBorderLineStyle IActualBorderInfo.VerticalLineStyle {
			get { return ((IActualBorderInfo)innerCell).VerticalLineStyle; }
		}
		Color IActualBorderInfo.LeftColor {
			get { return ((IActualBorderInfo)innerCell).LeftColor; }
		}
		Color IActualBorderInfo.RightColor {
			get { return ((IActualBorderInfo)innerCell).RightColor; }
		}
		Color IActualBorderInfo.TopColor {
			get { return ((IActualBorderInfo)innerCell).TopColor; }
		}
		Color IActualBorderInfo.BottomColor {
			get { return ((IActualBorderInfo)innerCell).BottomColor; }
		}
		Color IActualBorderInfo.DiagonalColor {
			get { return ((IActualBorderInfo)innerCell).DiagonalColor; }
		}
		Color IActualBorderInfo.HorizontalColor {
			get { return ((IActualBorderInfo)innerCell).HorizontalColor; }
		}
		Color IActualBorderInfo.VerticalColor {
			get { return ((IActualBorderInfo)innerCell).VerticalColor; }
		}
		bool IActualCellAlignmentInfo.WrapText {
			get { return ((IActualCellAlignmentInfo)innerCell).WrapText; }
		}
		bool IActualCellAlignmentInfo.JustifyLastLine {
			get { return ((IActualCellAlignmentInfo)innerCell).JustifyLastLine; }
		}
		bool IActualCellAlignmentInfo.ShrinkToFit {
			get { return ((IActualCellAlignmentInfo)innerCell).ShrinkToFit; }
		}
		int IActualCellAlignmentInfo.TextRotation {
			get { return ((IActualCellAlignmentInfo)innerCell).TextRotation; }
		}
		byte IActualCellAlignmentInfo.Indent {
			get { return ((IActualCellAlignmentInfo)innerCell).Indent; }
		}
		int IActualCellAlignmentInfo.RelativeIndent {
			get { return ((IActualCellAlignmentInfo)innerCell).RelativeIndent; }
		}
		XlHorizontalAlignment IActualCellAlignmentInfo.Horizontal {
			get { return ((IActualCellAlignmentInfo)innerCell).Horizontal; }
		}
		XlVerticalAlignment IActualCellAlignmentInfo.Vertical {
			get { return ((IActualCellAlignmentInfo)innerCell).Vertical; }
		}
		XlReadingOrder IActualCellAlignmentInfo.ReadingOrder {
			get { return ((IActualCellAlignmentInfo)innerCell).ReadingOrder; }
		}
		int IActualBorderInfo.LeftColorIndex {
			get { return ((IActualBorderInfo)innerCell).LeftColorIndex; }
		}
		int IActualBorderInfo.RightColorIndex {
			get { return ((IActualBorderInfo)innerCell).RightColorIndex; }
		}
		int IActualBorderInfo.TopColorIndex {
			get { return ((IActualBorderInfo)innerCell).TopColorIndex; }
		}
		int IActualBorderInfo.BottomColorIndex {
			get { return ((IActualBorderInfo)innerCell).BottomColorIndex; }
		}
		int IActualBorderInfo.DiagonalColorIndex {
			get { return ((IActualBorderInfo)innerCell).DiagonalColorIndex; }
		}
		int IActualBorderInfo.HorizontalColorIndex {
			get { return ((IActualBorderInfo)innerCell).HorizontalColorIndex; }
		}
		int IActualBorderInfo.VerticalColorIndex {
			get { return ((IActualBorderInfo)innerCell).VerticalColorIndex; }
		}
		XlPatternType IActualFillInfo.PatternType {
			get { return ((IActualFillInfo)innerCell).PatternType; }
		}
		bool IActualCellProtectionInfo.Locked {
			get { return ((IActualCellProtectionInfo)innerCell).Locked; }
		}
		bool IBorderInfo.Outline {
			get {
				return ((IBorderInfo)innerCell).Outline;
			}
			set {
				((IBorderInfo)innerCell).Outline = value;
			}
		}
		string IActualRunFontInfo.Name {
			get { return ((IActualRunFontInfo)innerCell).Name; }
		}
		Color IActualRunFontInfo.Color {
			get { return ((IActualRunFontInfo)innerCell).Color; }
		}
		bool IActualRunFontInfo.Bold {
			get { return ((IActualRunFontInfo)innerCell).Bold; }
		}
		bool IActualRunFontInfo.Condense {
			get { return ((IActualRunFontInfo)innerCell).Condense; }
		}
		bool IActualRunFontInfo.Extend {
			get { return ((IActualRunFontInfo)innerCell).Extend; }
		}
		bool IActualRunFontInfo.Italic {
			get { return ((IActualRunFontInfo)innerCell).Italic; }
		}
		bool IActualRunFontInfo.Outline {
			get { return ((IActualRunFontInfo)innerCell).Outline; }
		}
		bool IActualRunFontInfo.Shadow {
			get { return ((IActualRunFontInfo)innerCell).Shadow; }
		}
		bool IActualRunFontInfo.StrikeThrough {
			get { return ((IActualRunFontInfo)innerCell).StrikeThrough; }
		}
		int IActualRunFontInfo.Charset {
			get { return ((IActualRunFontInfo)innerCell).Charset; }
		}
		int IActualRunFontInfo.FontFamily {
			get { return ((IActualRunFontInfo)innerCell).FontFamily; }
		}
		double IActualRunFontInfo.Size {
			get { return ((IActualRunFontInfo)innerCell).Size; }
		}
		XlFontSchemeStyles IActualRunFontInfo.SchemeStyle {
			get { return ((IActualRunFontInfo)innerCell).SchemeStyle; }
		}
		XlScriptType IActualRunFontInfo.Script {
			get { return ((IActualRunFontInfo)innerCell).Script; }
		}
		XlUnderlineType IActualRunFontInfo.Underline {
			get { return ((IActualRunFontInfo)innerCell).Underline; }
		}
		bool IActualBorderInfo.Outline {
			get { return ((IActualBorderInfo)innerCell).Outline; }
		}
		Color IActualFillInfo.ForeColor {
			get { return ((IActualFillInfo)innerCell).ForeColor; }
		}
		Color IActualFillInfo.BackColor {
			get { return ((IActualFillInfo)innerCell).BackColor; }
		}
		ModelFillType IActualFillInfo.FillType {
			get { return ((IActualFillInfo)innerCell).FillType; }
		}
		bool IActualCellProtectionInfo.Hidden {
			get { return ((IActualCellProtectionInfo)innerCell).Hidden; }
		}
		ModelGradientFillType IActualGradientFillInfo.Type {
			get { return ((IActualGradientFillInfo)innerCell).Type; }
		}
		double IActualGradientFillInfo.Degree {
			get { return ((IActualGradientFillInfo)innerCell).Degree; }
		}
		float IActualConvergenceInfo.Left {
			get { return ((IActualConvergenceInfo)innerCell).Left; }
		}
		float IActualConvergenceInfo.Right {
			get { return ((IActualConvergenceInfo)innerCell).Right; }
		}
		float IActualConvergenceInfo.Top {
			get { return ((IActualConvergenceInfo)innerCell).Top; }
		}
		float IActualConvergenceInfo.Bottom {
			get { return ((IActualConvergenceInfo)innerCell).Bottom; }
		}
		bool ICalculationWaiter.AllowsMerging {
			get { return ((ICalculationWaiter)innerCell).AllowsMerging; }
		}
		public double DoubleValue { get { return innerCell.DoubleValue; } set { innerCell.DoubleValue = value; } }
		public Int64 PackedValues { get { return innerCell.PackedValues; } set { innerCell.PackedValues = value; } }
		public Int64 PackedFormat { get { return innerCell.PackedFormat; } set { innerCell.PackedFormat = value; } }
#if BTREE
		#region IIndexedObject Members
		long IIndexedObject.Index { get { return innerCell.Index; } }
		#endregion
#endif
	}
	#endregion
	#region CellWrapperWithModifiedValue
	public class CellWrapperWithModifiedValue : CellWrapper {
		readonly VariantValue value;
		public CellWrapperWithModifiedValue(Cell cell, VariantValue value)
			: base(cell) {
			this.value = value;
		}
		public override VariantValue GetValue() {
			return value;
		}
	}
	#endregion
	#region CellWrapperWithOverridenSheet
	public class CellWrapperWithOverridenSheet : Cell {
		readonly ICell innerCell;
		public CellWrapperWithOverridenSheet(ICell innerCell, Worksheet sheet)
			: base(innerCell.Position, sheet) {
			this.innerCell = innerCell;
			this.CopyFrom(innerCell);
		}
		public override FormulaInfo FormulaInfo { get { return innerCell.FormulaInfo; } set { innerCell.FormulaInfo = value; } }
		public override ConditionalFormattingFormatAccumulator ConditionalFormatAccumulator { get { return innerCell.ConditionalFormatAccumulator; } set { innerCell.ConditionalFormatAccumulator = value; } }
	}
	#endregion
}
