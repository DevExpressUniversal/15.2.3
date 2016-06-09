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

using DevExpress.Export;
using DevExpress.Printing.ExportHelpers;
using DevExpress.Utils;
using DevExpress.XtraExport.Csv;
using DevExpress.Export.Xl;
using DevExpress.XtraExport.Xls;
using DevExpress.XtraExport.Xlsx;
using DevExpress.XtraPrinting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using DevExpress.Office.Utils;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Printing.ExportHelpers.Helpers;
namespace DevExpress.XtraExport.Helpers {
	public class XlExportManager {
		protected IXlExport exporterCore;
		protected IXlDocument document;
		IDataAwareExportOptions optionsCore = null;
		public XlExportManager(IDataAwareExportOptions options){
			this.optionsCore = options;
		}
		public IDataAwareExportOptions Options { get { return optionsCore; } }
		protected void CreateExporter(){
			switch(Options.ExportTarget){
				case ExportTarget.Xls:
					exporterCore = new XlsDataAwareExporter();
					break;
				case ExportTarget.Csv:
					CsvDataAwareExporter temp = new CsvDataAwareExporter();
					temp.Options.Encoding = Options.CSVEncoding;
					temp.Options.WritePreamble = Options.WritePreamble;
					if(!string.IsNullOrEmpty(Options.CSVSeparator))
						temp.Options.ValueSeparatorString = Options.CSVSeparator;
					exporterCore = temp;
					break;
				case ExportTarget.Xlsx:
					exporterCore = new XlsxDataAwareExporter();
					break;
				default:
#if DXPORTABLE
					throw new Exception("only Xls, Xlsx and Csv allowed");
#else
					throw new NotFiniteNumberException("only Xls, Xlsx and Csv allowed");
#endif
			}
		}
		public IXlExport Exporter{
			get{
				if(exporterCore == null) CreateExporter();
				return exporterCore;
			}
		}
		protected virtual void ExportCore(Stream stream){
		}
		public virtual void Export(Stream stream){
			Diagnostics.Trace();
			Exporter.DocumentOptions.Culture = CultureInfo.CurrentCulture;
			document = Exporter.BeginExport(stream);
			ExportCore(stream);
			Exporter.EndExport();
		}
	}
	public abstract class BaseViewExcelExporter<T> : XlExportManager where T : class {
		T viewToExportCore;
		IXlSheet sheet;
		IDataAwareExportOptions optionsCore;
		public BaseViewExcelExporter(T viewToExport, IDataAwareExportOptions options):base(options){
			viewToExportCore = viewToExport;
			optionsCore = options;
		}
		internal T View { get { return viewToExportCore; } }
		internal IXlSheet Sheet { get { return sheet; } }
		protected abstract void ExportOverride();
		public void Export(string outputpath){
			using(FileStream stream = new FileStream(outputpath, FileMode.Create, FileAccess.Write, FileShare.Read)){
				Export(stream);
			}
		}
		protected override void ExportCore(Stream stream){
			sheet = Exporter.BeginSheet();
			if(!string.IsNullOrEmpty(Options.SheetName)) sheet.Name = Options.SheetName;
			sheet.ViewOptions.RightToLeft = Options.RightToLeftDocument == DefaultBoolean.True;
			ExportOverride();
		}
	}
	public class GridViewExcelExporter<TCol, TRow> : BaseViewExcelExporter<IGridView<TCol, TRow>> where TRow : class, IRowBase where TCol : class, IColumn {
		DataAwareExportContext<TCol, TRow> context;
		Lazy<ConditionalFormattingExporter<TCol, TRow>> condFmtExporter;
		Lazy<ExportCellMerger<TCol, TRow>> exportMergeHelper;
		Lazy<SummaryExportHelper<TCol, TRow>> summaryExportHelper;
		Lazy<LookUpValuesExporter<TCol, TRow>> lookUpValuesExporter;
		ExporterInfo<TCol, TRow> exportInfo;
		public GridViewExcelExporter(IGridView<TCol, TRow> viewToExport, IDataAwareExportOptions options) : base(viewToExport, options){
			CreateInstaces();
		}
		public GridViewExcelExporter(IGridView<TCol, TRow> viewToExport) : base(viewToExport, DataAwareExportOptionsFactory.Create(ExportTarget.Xlsx)){
			CreateInstaces();
		}
		protected virtual void CreateInstaces(){
			condFmtExporter = new Lazy<ConditionalFormattingExporter<TCol, TRow>>(CreateInstance());
			exportMergeHelper = new Lazy<ExportCellMerger<TCol, TRow>>(CreateMergeHelperInstance());
			summaryExportHelper = new Lazy<SummaryExportHelper<TCol, TRow>>(CreateSummaryExporterInstance());
			lookUpValuesExporter = new Lazy<LookUpValuesExporter<TCol, TRow>>(CreateLookUpExporterInstance());
		}
		LookUpValuesExporter<TCol, TRow> LookUpExporter { get { return lookUpValuesExporter.Value; } }
		Func<LookUpValuesExporter<TCol, TRow>> CreateLookUpExporterInstance(){
			return () => new LookUpValuesExporter<TCol, TRow>(ExportInfo);
		}
		internal SummaryExportHelper<TCol, TRow> SummaryExporter { get { return summaryExportHelper.Value; } }
		Func<SummaryExportHelper<TCol, TRow>> CreateSummaryExporterInstance(){
			return () => new SummaryExportHelper<TCol, TRow>(ExportInfo);
		}
		internal ExportCellMerger<TCol, TRow> CellMerger{
			get { return exportMergeHelper.Value; }
		}
		Func<ExportCellMerger<TCol, TRow>> CreateMergeHelperInstance(){
			return () => new ExportCellMerger<TCol, TRow>(ExportInfo);
		}
		ConditionalFormattingExporter<TCol, TRow> CFHelper{
			get { return condFmtExporter.Value; }
		}
		Func<ConditionalFormattingExporter<TCol, TRow>> CreateInstance(){
			return () => new ConditionalFormattingExporter<TCol, TRow>(ExportInfo);
		}
		protected internal ExporterInfo<TCol, TRow> ExportInfo {
			get {
				if(exportInfo == null)
					exportInfo = CreateExportInfo();
				return exportInfo;
			}
		}
		internal virtual ExporterInfo<TCol, TRow> CreateExportInfo() {
			return new ExporterInfo<TCol, TRow>(this);
		}
		protected DataAwareExportContext<TCol, TRow> Context {
			get {
				if(context == null) context = CreateContext(ExportInfo);
				return context;
			}
		}
		internal virtual DataAwareExportContext<TCol, TRow> CreateContext(ExporterInfo<TCol, TRow> exportInfo) {
			return new DataAwareExportContext<TCol, TRow>(exportInfo);
		}
		protected virtual bool CanExportRow(IRowBase row) {
			return !row.IsGroupRow && ExportInfo.СomplyWithFormatLimits();
		}
		protected void ForAllColumns(IEnumerable<TCol> gridcolumns, Action<TCol> action) {
			foreach(TCol col in gridcolumns) action(col);
		}
		internal virtual void ForAllRows(IGridView<TCol, TRow> view, Action<TRow> action) {
			ForAllRowsCore(null, view, action);
		}
		void ForAllRowsCore(IGroupRow<TRow> parent, IGridView<TCol, TRow> view, Action<TRow> action) {
			IEnumerable<TRow> iterator;
			if(parent == null) iterator = GetRowsCore(view);
			else iterator = parent.GetAllRows();
			foreach(TRow row in iterator) {
				action(row);
				if(view.IsCancelPending) break;
				IGroupRow<TRow> igr = row as IGroupRow<TRow>;
				if(igr != null) ForAllRowsCore(igr, view, action);
			}
		}
		public void ForAllColumns(IGridView<TCol, TRow> view, Action<TCol> action){
			ForAllColumns(null, view, action);
		}
		public void ForAllColumns(TCol parent, IGridView<TCol, TRow> view, Action<TCol> action){
			IEnumerable<IColumn> iterator;
			if(parent != null && parent.IsGroupColumn) iterator = parent.GetAllColumns();
			else iterator = GetColumnsCore(view);
			foreach(TCol column in iterator){
				action(column);
				if(column.IsGroupColumn) ForAllColumns(column, view, action);
			}
		}
		protected virtual IEnumerable<TCol> GetColumnsCore(IGridView<TCol, TRow> view){
			return view.GetAllColumns();
		}
		protected virtual IEnumerable<TRow> GetRowsCore(IGridView<TCol, TRow> view) {
			return view.GetAllRows();
		}
		protected override void ExportOverride() {
			ExportColumns();
			ExportDocumentHeader();
			ExportData();
		}
		void ExportColumns() {
			ExportInfo.Sheet.OutlineProperties.SummaryRight = false;
			ForAllColumns(View, ExportColumn());
			while(ExportInfo.GroupsStack.Count > 0) {
				ExportInfo.GroupsStack.Pop();
				Exporter.EndGroup();
			}
			ExportInfo.GroupsStack.Clear();
		}
		protected virtual Action<TCol> ExportColumn(){
			return gridColumn => { Context.CreateColumn(gridColumn); };
		}
		void ExportDocumentHeader() {
			Context.SetExportSheetSettings();
			if(Options.ShowPageTitle == DefaultBoolean.True) Context.PrintTitles();
			if(ExportInfo.OptionsAllowSetFixedHeader) Context.CustomizeSheetSettings();
			if(ExportInfo.ExportColumnHeaders) Context.CreateHeader();
		}
		void ExportData() {
			TRow lastExportedRow = null;
			bool wasDataRow = false;
			int startExcelGroupIndex = ExportInfo.ExportRowIndex;
			int endExcelGroupIndex = 0;
			int percentage = 0;
			int groupId = 1;
			ForAllRows(View, gridRow => {
				startExcelGroupIndex = ExportDataCore(ref lastExportedRow, ref wasDataRow, ref endExcelGroupIndex, ref percentage, ref groupId, gridRow, startExcelGroupIndex);
			});
			AddGroupToList(startExcelGroupIndex, ExportInfo.ExportRowIndex);
			if(ExportInfo.OptionsAllowAddAutoFilter) Context.AddAutoFilter();
			CompleteMerging(lastExportedRow);
			if(!ExportInfo.RawDataMode) CFHelper.ExportFormatRules();
			CloseAllOpenGroups();
			Context.CreateFooter();
			if(Options.AllowSparklines == DefaultBoolean.True){
				SparklineExportHelper<TCol, TRow> spHelper = new SparklineExportHelper<TCol, TRow>(ExportInfo);
				spHelper.Execute(percentage);
			}
			if(Options.AllowLookupValues == DefaultBoolean.True){
				LookUpExporter.ProcessDataValidation("Sheet2");
				LookUpExporter.CompleteDataValidation(percentage, ExportInfo.ExportRowIndex);
			} else ExportInfo.Exporter.EndSheet();
			CompleteReportProgress(percentage);
		}
		protected virtual int ExportDataCore(ref TRow lastExportedRow, ref bool wasDataRow, ref int endExcelGroupIndex, ref int percentage, ref int groupId, TRow gridRow, int startExcelGroupIndex) {
			lastExportedRow = gridRow;
			int currentRowLevel = gridRow.GetRowLevel();
			endExcelGroupIndex = CompleteGrouping(lastExportedRow, endExcelGroupIndex, currentRowLevel);
			ExportStartGroup(ref wasDataRow, startExcelGroupIndex, endExcelGroupIndex, ref groupId, gridRow, currentRowLevel);
			startExcelGroupIndex = ExportDataRow(ref wasDataRow, gridRow, startExcelGroupIndex);
			ExportInfo.ReportProgress(ExportInfo.ExportRowIndex, ref percentage);
			return startExcelGroupIndex;
		}
		protected virtual int CompleteGrouping(TRow lastExportedRow, int endExcelGroupIndex, int currentRowLevel) {
			while(ExportInfo.GroupsStack.Count > 0 && ExportInfo.GroupsStack.Peek().Group.OutlineLevel >= currentRowLevel + 1) {
				endExcelGroupIndex = Exporter.CurrentRowIndex;
				if(Options.AllowCellMerge == DefaultBoolean.True)
					CellMerger.CompleteMergingInGroup(lastExportedRow, endExcelGroupIndex);
				Context.SetSummary(ExportInfo.GroupsStack.Peek().StartGroup, endExcelGroupIndex, ExportInfo.GroupsStack.Peek().GroupId);
				ExportInfo.GroupsStack.Pop();
				Exporter.EndGroup();
			}
			return endExcelGroupIndex;
		}
		protected virtual int ExportDataRow(ref bool wasDataRow, TRow gridRow, int startExcelGroupIndex) {
			if(CanExportRow(gridRow)) {
				if(!wasDataRow) startExcelGroupIndex = ExportInfo.ExportRowIndex;
				Context.AddRow(gridColumn => !gridColumn.IsGroupColumn ? View.GetRowCellValue(gridRow, gridColumn) : null, gridRow);
				wasDataRow = true;
			}
			return startExcelGroupIndex;
		}
		protected virtual void ExportStartGroup(ref bool wasDataRow, int startExcelGroupIndex, int endExcelGroupIndex, ref int groupId, TRow gridRow, int currentRowLevel) {
			if(gridRow.IsGroupRow && Options.AllowGrouping == DefaultBoolean.True) {
				if(wasDataRow) AddGroupToList(startExcelGroupIndex, endExcelGroupIndex - 1);
				IGroupRow<TRow> groupRow = gridRow as IGroupRow<TRow>;
				if(ExportInfo.СomplyWithFormatLimits()) {
					Context.PrintGroupRowHeader(gridRow);
				}
				bool isRowCollapsed = groupRow != null && Options.GroupState == GroupState.Default ? groupRow.IsCollapsed : (Options.GroupState == GroupState.CollapseAll);
				Context.CreateExportDataGroup(currentRowLevel, groupId++, isRowCollapsed);
				wasDataRow = false;
			}
		}
		void CompleteReportProgress(int sPercentage) {
			for(int i = sPercentage; i < 100; i++)
				ExportInfo.ReportProgress(i);
		}
		protected virtual void AddGroupToList(int startIndex, int endIndex) {
			ExportInfo.GroupsList.Add(new Group { Start = startIndex, End = endIndex });
		}
		void CompleteMerging(TRow lastRow) {
			if(Options.AllowCellMerge == DefaultBoolean.True)
				CellMerger.MergedLastCells(lastRow, ExportInfo.ExportRowIndex);
		}
		protected virtual void CloseAllOpenGroups() {
			while(ExportInfo.GroupsStack.Count > 0) {
				Context.SetSummary(ExportInfo.GroupsStack.Peek().StartGroup, ExportInfo.ExportRowIndex, ExportInfo.GroupsStack.Peek().GroupId);
				ExportInfo.GroupsStack.Pop();
				Exporter.EndGroup();
			}
		}
	}
}
namespace DevExpress.Export {
	public enum ExportType { Default, DataAware, WYSIWYG }
	public class ExportSettings {
		static ExportType defaultExportTypeCore = ExportType.DataAware;
		public static ExportType DefaultExportType { get { return defaultExportTypeCore == ExportType.Default ? ExportType.DataAware : defaultExportTypeCore; } set { defaultExportTypeCore = value; } }
	}
}
