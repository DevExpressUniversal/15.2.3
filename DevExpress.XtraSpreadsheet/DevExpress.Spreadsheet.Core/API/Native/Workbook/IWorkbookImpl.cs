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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Xml.Linq;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.ComponentModel.Design;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Spreadsheet;
using DevExpress.Spreadsheet.Formulas;
using DevExpress.Spreadsheet.Functions;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Export;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
using DefinedNameCollection = DevExpress.Spreadsheet.DefinedNameCollection;
using ModelCellRangeType = DevExpress.XtraSpreadsheet.Model.CellRangeType;
using ParsedExpression = DevExpress.XtraSpreadsheet.Model.ParsedExpression;
using TableStyleCollection = DevExpress.Spreadsheet.TableStyleCollection;
using Worksheet = DevExpress.Spreadsheet.Worksheet;
using WorksheetCollection = DevExpress.Spreadsheet.WorksheetCollection;
using PivotCacheCollection = DevExpress.Spreadsheet.PivotCacheCollection;
using DevExpress.DataAccess;
using DevExpress.DataAccess.Native;
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	partial class NativeWorkbook : IWorkbook, IRangeProvider {
		public WorksheetCollection Worksheets {
			get {
				CheckValid();
				return this.NativeWorksheets;
			}
		}
		public DefinedNameCollection DefinedNames {
			get {
				CheckValid();
				return this.NativeDefinedNames;
			}
		}
		public StyleCollection Styles {
			get {
				CheckValid();
				return NativeCellStyles;
			}
		}
		public TableStyleCollection TableStyles {
			get {
				CheckValid();
				return NativeTableStyles;
			}
		}
		public ExternalWorkbookCollection ExternalWorkbooks {
			get {
				CheckValid();
				return NativeExternalWorkbooks;
			}
		}
		public DocumentOptions Options {
			get {
				CheckValid();
				return server != null ? server.Options : null;
			}
		}
		public DocumentSettings DocumentSettings {
			get {
				CheckValid();
				return NativeDocumentSettings;
			}
		}
		public SpreadsheetHistory History {
			get {
				CheckValid();
				return NativeHistory;
			}
		}
		public PivotCacheCollection PivotCaches {
			get {
				CheckValid();
				return this.NativePivotCaches;
			}
		}
		public DocumentUnit Unit {
			get {
				CheckValid();
				return server.Unit;
			}
			set {
				CheckValid();
				server.Unit = value;
			}
		}
		public IRangeProvider Range { get { return this; } }
		public bool IsDisposed { get { return isDisposed; } }
		[Browsable(false)]
		public DocumentModelAccessor Model { get { return server.Model; } }
		public string CurrentAuthor {
			get {
				CheckValid();
				return this.ModelWorkbook.CurrentAuthor;
			}
			set {
				CheckValid();
				ModelWorkbook.CurrentAuthor = value;
			}
		}
		public CustomFunctionCollection GlobalCustomFunctions { get { return Functions.GlobalCustomFunctions; } }
		public CustomFunctionCollection CustomFunctions { get { return Functions.CustomFunctions; } }
		public WorkbookFunctions Functions { get { return NativeFunctions; } }
		public FormulaEngine FormulaEngine { get { return NativeFormulas; } }
		public bool Modified {
			get {
				CheckValid();
				if (Server != null)
					return Server.Modified;
				return false;
			}
			set {
				CheckValid();
				if (Server != null)
					Server.Modified = value;
			}
		}
		public bool HasMacros {
			get {
				CheckValid();
				return ModelWorkbook.HasMacros;
			}
		}
		public IClipboardManager Clipboard {
			get {
				CheckValid();
				return Server.Clipboard;
			}
		}
		public void CreateNewDocument() {
			CheckValid();
			if (server != null) {
				server.CreateNewDocument();
			}
		}
		public bool LoadDocument(string fileName, DocumentFormat format) {
			CheckValid();
			return LoadDocumentCore(fileName, format);
		}
		public bool LoadDocument(string fileName) {
			CheckValid();
			return LoadDocumentCore(fileName);
		}
		public bool LoadDocument(byte[] buffer, DocumentFormat format) {
			CheckValid();
			using (MemoryStream stream = new MemoryStream(buffer)) {
				return LoadDocumentCore(stream, format);
			}
		}
		public bool LoadDocument(Stream stream, DocumentFormat format) {
			CheckValid();
			return LoadDocumentCore(stream, format);
		}
		public void SaveDocument(string fileName) {
			CheckValid();
			SaveDocumentCore(fileName);
		}
		public void SaveDocument(string fileName, DocumentFormat format) {
			CheckValid();
			SaveDocumentCore(fileName, format);
		}
		public byte[] SaveDocument(DocumentFormat format) {
			CheckValid();
			using (MemoryStream stream = new MemoryStream()) {
				SaveDocumentCore(stream, format);
				return stream.ToArray();
			}
		}
		public void SaveDocument(Stream stream, DocumentFormat format) {
			CheckValid();
			SaveDocumentCore(stream, format);
		}
		public void Calculate() {
			CheckValid();
			ModelWorkbook.CalculationChain.CalculateWorkbook();
			ModelWorkbook.InnerApplyChanges(DocumentModelChangeActions.Redraw);
		}
		public void Calculate(Range range) {
			CheckValid();
			ModelWorkbook.CalculationChain.CalculateRange(((NativeWorksheet)range.Worksheet).GetModelRange(range));
			ModelWorkbook.InnerApplyChanges(DocumentModelChangeActions.Redraw);
		}
		public void Calculate(Worksheet sheet) {
			CheckValid();
			ModelWorkbook.CalculationChain.CalculateWorksheet(((NativeWorksheet)sheet).ModelWorksheet);
			ModelWorkbook.InnerApplyChanges(DocumentModelChangeActions.Redraw);
		}
		public void CalculateFull() {
			CheckValid();
			ModelWorkbook.CalculationChain.ForceFullCalculation();
			ModelWorkbook.InnerApplyChanges(DocumentModelChangeActions.Redraw);
		}
		public void CalculateFullRebuild() {
			CheckValid();
			ModelWorkbook.CalculationChain.ForceFullCalculationRebuild();
			ModelWorkbook.InnerApplyChanges(DocumentModelChangeActions.Redraw);
		}
		public ParameterValue Evaluate(string formula) {
			Worksheet sheet = worksheets.ActiveWorksheet;
			Range selectedCell = sheet.SelectedCell;
			FormulaEvaluationContext context = new FormulaEvaluationContext(selectedCell.LeftColumnIndex, selectedCell.TopRowIndex, sheet);
			return Evaluate(formula, context);
		}
		public ParameterValue Evaluate(string formula, FormulaEvaluationContext context) {
			WorkbookDataContext modelContext = ModelWorkbook.DataContext;
			Worksheet sheet = context.Sheet;
			if (sheet != null) {
				if (!ReferenceEquals(sheet.Workbook, this) || !this.worksheets.Contains(sheet))
					Exceptions.ThrowInvalidOperationException(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_InvalidSheetSpecified));
			}
			else
				sheet = worksheets.ActiveWorksheet;
			modelContext.PushCurrentWorksheet(((NativeWorksheet)sheet).ModelWorksheet);
			modelContext.PushCurrentCell(context.Column, context.Row);
			modelContext.PushArrayFormulaProcessing(context.IsArrayFormula);
			if (context.Culture != null)
				modelContext.PushCulture(context.Culture);
			else
				modelContext.PushCulture(modelContext.Culture);
			try {
				ParsedExpression expression = modelContext.ParseExpression(formula, OperandDataType.Value, false);
				if (expression == null) {
					CultureInfo culture = modelContext.Culture;
					string cultureName = culture == CultureInfo.InvariantCulture ? "Invariant" : culture.Name;
					string message = string.Format(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorFormula), formula, cultureName);
					throw new ArgumentException(message);
				}
				VariantValue value = expression.Evaluate(modelContext);
				return new ParameterValue(value, modelContext);
			}
			finally {
				modelContext.PopCulture();
				modelContext.PopArrayFormulaProcessing();
				modelContext.PopCurrentCell();
				modelContext.PopCurrentWorksheet();
			}
		}
		#region Events
		#region ActiveSheetChanging
		public event ActiveSheetChangingEventHandler ActiveSheetChanging {
			add { if (Server != null) Server.ActiveSheetChanging += value; }
			remove { if (Server != null) Server.ActiveSheetChanging -= value; }
		}
		#endregion
		#region ActiveSheetChanged
		public event ActiveSheetChangedEventHandler ActiveSheetChanged {
			add { if (Server != null) Server.ActiveSheetChanged += value; }
			remove { if (Server != null) Server.ActiveSheetChanged -= value; }
		}
		#endregion
		#region SheetRenaming
		public event SheetRenamingEventHandler SheetRenaming {
			add { if (Server != null) Server.SheetRenaming += value; }
			remove { if (Server != null) Server.SheetRenaming -= value; }
		}
		#endregion
		#region SheetRenamed
		public event SheetRenamedEventHandler SheetRenamed {
			add { if (Server != null) Server.SheetRenamed += value; }
			remove { if (Server != null) Server.SheetRenamed -= value; }
		}
		#endregion
		#region SheetInserted
		public event SheetInsertedEventHandler SheetInserted {
			add { if (Server != null) Server.SheetInserted += value; }
			remove { if (Server != null) Server.SheetInserted -= value; }
		}
		#endregion
		#region SheetRemoved
		public event SheetRemovedEventHandler SheetRemoved {
			add { if (Server != null) Server.SheetRemoved += value; }
			remove { if (Server != null) Server.SheetRemoved -= value; }
		}
		#endregion
		#region RowsInserted
		public event RowsInsertedEventHandler RowsInserted {
			add { if (Server != null) Server.RowsInserted += value; }
			remove { if (Server != null) Server.RowsInserted -= value; }
		}
		#endregion
		#region RowsRemoved
		public event RowsRemovedEventHandler RowsRemoved {
			add { if (Server != null) Server.RowsRemoved += value; }
			remove { if (Server != null) Server.RowsRemoved -= value; }
		}
		#endregion
		#region ColumnsInserted
		public event ColumnsInsertedEventHandler ColumnsInserted {
			add { if (Server != null) Server.ColumnsInserted += value; }
			remove { if (Server != null) Server.ColumnsInserted -= value; }
		}
		#endregion
		#region ColumnsRemoved
		public event ColumnsRemovedEventHandler ColumnsRemoved {
			add { if (Server != null) Server.ColumnsRemoved += value; }
			remove { if (Server != null) Server.ColumnsRemoved -= value; }
		}
		#endregion
		#region SelectionChanged
		public event EventHandler SelectionChanged {
			add { if (Server != null) Server.SelectionChanged += value; }
			remove { if (Server != null) Server.SelectionChanged -= value; }
		}
		#endregion
		#region DocumentLoaded
		public event EventHandler DocumentLoaded {
			add { if (Server != null) Server.DocumentLoaded += value; }
			remove { if (Server != null) Server.DocumentLoaded -= value; }
		}
		#endregion
		#region ModifiedChanged
		public event EventHandler ModifiedChanged {
			add { if (Server != null) Server.ModifiedChanged += value; }
			remove { if (Server != null) Server.ModifiedChanged -= value; }
		}
		#endregion
		#region EmptyDocumentCreated
		public event EventHandler EmptyDocumentCreated {
			add { if (Server != null) Server.EmptyDocumentCreated += value; }
			remove { if (Server != null) Server.EmptyDocumentCreated -= value; }
		}
		#endregion
		#region BeforeImport
		public event BeforeImportEventHandler BeforeImport {
			add { if (Server != null) Server.BeforeImport += value; }
			remove { if (Server != null) Server.BeforeImport -= value; }
		}
		#endregion
		#region BeforeExport
		public event BeforeExportEventHandler BeforeExport {
			add { if (Server != null) Server.BeforeExport += value; }
			remove { if (Server != null) Server.BeforeExport -= value; }
		}
		#endregion
		#region InitializeDocument
		public event EventHandler InitializeDocument {
			add { if (Server != null) Server.InitializeDocument += value; }
			remove { if (Server != null) Server.InitializeDocument -= value; }
		}
		#endregion
		#region UnitChanging
		public event EventHandler UnitChanging {
			add { if (Server != null) Server.UnitChanging += value; }
			remove { if (Server != null) Server.UnitChanging -= value; }
		}
		#endregion
		#region UnitChanged
		public event EventHandler UnitChanged {
			add { if (Server != null) Server.UnitChanged += value; }
			remove { if (Server != null) Server.UnitChanged -= value; }
		}
		#endregion
		#region ContentChanged
		public event EventHandler ContentChanged {
			add { if (Server != null) Server.ContentChanged += value; }
			remove { if (Server != null) Server.ContentChanged -= value; }
		}
		#endregion
		#region SchemaChanged
		public event EventHandler SchemaChanged {
			add { if (Server != null) Server.SchemaChanged += value; }
			remove { if (Server != null) Server.SchemaChanged -= value; }
		}
		#endregion
		#region PanesFrozen
		public event PanesFrozenEventHandler PanesFrozen {
			add { if (Server != null) Server.PanesFrozen += value; }
			remove { if (Server != null) Server.PanesFrozen -= value; }
		}
		#endregion
		#region PanesUnfrozen
		public event PanesUnfrozenEventHandler PanesUnfrozen {
			add { if (Server != null) Server.PanesUnfrozen += value; }
			remove { if (Server != null) Server.PanesUnfrozen -= value; }
		}
		#endregion
		#region BeforePrintSheet
		public event BeforePrintSheetEventHandler BeforePrintSheet {
			add { if (Server != null) Server.BeforePrintSheet += value; }
			remove { if (Server != null) Server.BeforePrintSheet -= value; }
		}
		#endregion
		public event RangeCopyingEventHandler RangeCopying {
			add { if (Server != null) Server.RangeCopying += value; }
			remove { if (Server != null) Server.RangeCopying -= value; }
		}
		public event RangeCopiedEventHandler RangeCopied {
			add { if (Server != null) Server.RangeCopied += value; }
			remove { if (Server != null) Server.RangeCopied -= value; }
		}
		public event ShapesCopyingEventHandler ShapesCopying {
			add { if (Server != null) Server.ShapesCopying += value; }
			remove { if (Server != null) Server.ShapesCopying -= value; }
		}
		public event CopiedRangePastingEventHandler CopiedRangePasting {
			add { if (Server != null) Server.CopiedRangePasting += value; }
			remove { if (Server != null) Server.CopiedRangePasting -= value; }
		}
		public event CopiedRangePastedEventHandler CopiedRangePasted {
			add { if (Server != null) Server.CopiedRangePasted += value; }
			remove { if (Server != null) Server.CopiedRangePasted -= value; }
		}
		public event ClipboardDataPastingEventHandler ClipboardDataPasting {
			add { if (Server != null) Server.ClipboardDataPasting += value; }
			remove { if (Server != null) Server.ClipboardDataPasting -= value; }
		}
		public event ClipboardDataObtainedEventHandler ClipboardDataObtained {
			add { if (Server != null) Server.ClipboardDataObtained += value; }
			remove { if (Server != null) Server.ClipboardDataObtained -= value; }
		}
		public event EventHandler ClipboardDataPasted {
			add { if (Server != null) Server.ClipboardDataPasted += value; }
			remove { if (Server != null) Server.ClipboardDataPasted -= value; }
		}
		#region ScrollPositionChanged
		public event ScrollPositionChangedEventHandler ScrollPositionChanged {
			add { if (Server != null) Server.ScrollPositionChanged += value; }
			remove { if (Server != null) Server.ScrollPositionChanged -= value; }
		}
		#endregion
		#endregion
		#region IBatchUpdateable Members
		public BatchUpdateHelper BatchUpdateHelper { get { return DocumentModel.BatchUpdateHelper; } }
		public bool IsUpdateLocked { get { return DocumentModel.IsUpdateLocked; } }
		public void BeginUpdate() {
			DocumentModel.BeginUpdate();
		}
		public void EndUpdate() {
			DocumentModel.EndUpdate();
		}
		public void CancelUpdate() {
			DocumentModel.CancelUpdate();
		}
		#endregion
		#region ExternalWorkbook Members
		IEnumerable<ExternalWorksheet> ExternalWorkbook.Worksheets {
			get { return Worksheets; }
		}
		public string Path {
			get { return Options.Save.CurrentFileName; }
		}
		IEnumerable<ExternalDefinedName> ExternalWorkbook.DefinedNames {
			get { return DefinedNames; }
		}
		#endregion
		#region IRangeProvider Members
		public Range this[string reference] { get { return Parse(reference, ReferenceStyle.A1); } }
		public Range FromLTRB(int leftColumnIndex, int topRowIndex, int rightColumnIndex, int bottomRowIndex) {
			if (Worksheets.ActiveWorksheet == null)
				return null;
			return Worksheets.ActiveWorksheet.Range.FromLTRB(leftColumnIndex, topRowIndex, rightColumnIndex, bottomRowIndex);
		}
		public Range Parse(string reference) {
			return Parse(reference, ReferenceStyle.A1);
		}
		public Range Parse(string reference, ReferenceStyle style) {
			NativeRangeReferenceParseHelper helper = new NativeRangeReferenceParseHelper();
			WorkbookDataContext context = ModelWorkbook.DataContext;
			NativeWorksheet activeWorksheet = (NativeWorksheet)Worksheets.ActiveWorksheet;
			context.PushCurrentWorksheet(activeWorksheet.ModelWorksheet);
			try {
				return helper.Process(context, reference, activeWorksheet, style);
			}
			finally {
				context.PopCurrentWorksheet();
			}
		}
		public Range Union(params Range[] ranges) {
			return NativeRangeReferenceParseHelper.CreateUnion(ranges);
		}
		public Range Union(IEnumerable<Range> enumerable) {
			return NativeRangeReferenceParseHelper.CreateUnion(enumerable);
		}
		#endregion
		#region MailMerge
		public object MailMergeDataSource {
			get {
				return ModelWorkbook.MailMergeDataSource;
			}
			set {
				ModelWorkbook.MailMergeDataSource = value;
			}
		}
		public string MailMergeDataMember {
			get {
				return ModelWorkbook.MailMergeDataMember;
			}
			set {
				ModelWorkbook.MailMergeDataMember = value;
			}
		}
		public IList<IWorkbook> GenerateMailMergeDocuments() {
			IList<IWorkbook> result = new List<IWorkbook>();
			object realDataSource = MailMergeDataSource;
			IDataComponent dataComponent = MailMergeDataSource as IDataComponent;
			if(dataComponent != null) {
				XElement xmlBody = new DataComponentHelper().SaveToXml(dataComponent);
				IDataComponent newDataComponent = new DataComponentHelper().LoadFromXml(xmlBody);
				ModelWorkbook.DataComponentInfos.TryToFillDataSource(newDataComponent);
				realDataSource = newDataComponent;
			}
			MailMergeProcessor processor = MailMergeProcessor.CreateInstance(ModelWorkbook);
			processor.DataSource = realDataSource;
			processor.DataMember = MailMergeDataMember;
			processor.Parameters.AddRange(ModelWorkbook.MailMergeParameters.InnerList);
			foreach (DocumentModel model in processor.Process())
				result.Add(new NativeWorkbook(model));
			return result;
		}
		public ParametersCollection MailMergeParameters { get { return NativeParameters; } }
		#endregion
		public void ExportToHtml(Stream stream, int sheetIndex) {
			DocumentModel.InternalAPI.SaveDocumentHtmlContent(stream, sheetIndex, null);
		}
		public void ExportToHtml(Stream stream, Worksheet sheet) {
			DocumentModel.InternalAPI.SaveDocumentHtmlContent(stream, Worksheets.IndexOf(sheet), null);
		}
		public void ExportToHtml(Stream stream, Range range) {
			if (range != null) {
				NativeWorksheet sheet = (NativeWorksheet)range.Worksheet;
				CellRange modelRange = sheet.GetModelSingleRange(range);
				DocumentModel.InternalAPI.SaveDocumentHtmlContent(stream, Worksheets.IndexOf(sheet), modelRange);
			}
		}
		public void ExportToHtml(Stream stream, HtmlDocumentExporterOptions options) {
			DocumentModel.InternalAPI.SaveDocumentHtmlContent(stream, options);
		}
		public void ExportToHtml(string fileName, int sheetIndex) {
			DocumentModel.InternalAPI.SaveDocumentHtmlContent(fileName, sheetIndex, null);
		}
		public void ExportToHtml(string fileName, Worksheet sheet) {
			DocumentModel.InternalAPI.SaveDocumentHtmlContent(fileName, Worksheets.IndexOf(sheet), null);
		}
		public void ExportToHtml(string fileName, Range range) {
			if (range != null) {
				NativeWorksheet sheet = (NativeWorksheet)range.Worksheet;
				CellRange modelRange = sheet.GetModelSingleRange(range);
				DocumentModel.InternalAPI.SaveDocumentHtmlContent(fileName, Worksheets.IndexOf(sheet), modelRange);
			}
		}
		public void ExportToHtml(string fileName, HtmlDocumentExporterOptions options) {
			DocumentModel.InternalAPI.SaveDocumentHtmlContent(fileName, options);
		}
		public bool IsProtected { get { return DocumentModel.IsProtected; } }
		public void Protect(string password, bool lockStructure, bool lockWindows) {
			DocumentModel.Protect(password, lockStructure, lockWindows);
		}
		public bool Unprotect(string password) {
			return DocumentModel.UnProtect(password) == null;
		}
	}
}
