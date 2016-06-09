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
using System.ComponentModel;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Data;
using DevExpress.XtraExport;
using DevExpress.Utils;
using System.Text;
using DevExpress.XtraPrinting;
using DevExpress.Export;
using DevExpress.Export.Xl;
using System.Drawing;
using DevExpress.Printing.ExportHelpers;
using DevExpress.XtraExport.Helpers;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraPrinting {
	public abstract class XlsExportOptionsBase : PageByPageExportOptionsBase {
		#region static
		static readonly string[] unvalideSymbols = { ":", @"\", "/", "?", "*", "[", "]" };
		static readonly byte validLength = 31;
		static string ValidSheetName(string sheetName) {
			string validSheetName = sheetName;
			foreach(string unvalidSymbol in unvalideSymbols)
				if(validSheetName.Contains(unvalidSymbol))
					validSheetName = validSheetName.Replace(unvalidSymbol, string.Empty);
			if(validSheetName.Length > validLength)
				validSheetName = validSheetName.Substring(0, validLength);
			if(string.IsNullOrEmpty(validSheetName))
				return DefaultSheetName;
			return validSheetName;
		}
		const string DefaultSheetName = "Sheet";
		const TextExportMode DefaultTextExportMode = TextExportMode.Value;
		#endregion
		DefaultBoolean rightToLeftDocument = DefaultBoolean.Default;
		bool fitToPrintedPageWidth = false;
		bool exportHyperlinks = true;
		bool showGridLines;
		string sheetName = DefaultSheetName;
		TextExportMode textExportMode = DefaultTextExportMode;
		bool rawDataMode;
		XlDocumentOptions documentOptions = new XlDocumentOptions();
		protected XlsExportOptionsBase(XlsExportOptionsBase source)
			: base(source) {
		}
		public XlsExportOptionsBase() : this(TextExportMode.Value) { }
		public XlsExportOptionsBase(TextExportMode textExportMode) {
			this.textExportMode = textExportMode;
		}
		public XlsExportOptionsBase(TextExportMode textExportMode, bool showGridLines)
			: this(textExportMode) {
			this.showGridLines = showGridLines;
		}
		public XlsExportOptionsBase(TextExportMode textExportMode, bool showGridLines, bool exportHyperlinks)
			: this(textExportMode, showGridLines) {
			this.exportHyperlinks = exportHyperlinks;
		}
		public XlsExportOptionsBase(TextExportMode textExportMode, bool showGridLines, bool exportHyperlinks, bool fitToPrintedPageWidth)
			: this(textExportMode, showGridLines, exportHyperlinks) {
			this.fitToPrintedPageWidth = fitToPrintedPageWidth;
		}
		public XlsExportOptionsBase(TextExportMode textExportMode, bool showGridLines, bool exportHyperlinks, bool fitToPrintedPageWidth, DefaultBoolean rightToLeftDocument)
			: this(textExportMode, showGridLines, exportHyperlinks, fitToPrintedPageWidth) {
				this.rightToLeftDocument = rightToLeftDocument;
		}
		#region properties
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.XlsExportOptions.RawDataMode"),
		DefaultValue(false),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool RawDataMode { get { return rawDataMode; } set { rawDataMode = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("XlsExportOptionsBaseTextExportMode"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.XlsExportOptionsBase.TextExportMode"),
		DefaultValue(DefaultTextExportMode),
		XtraSerializableProperty]
		public TextExportMode TextExportMode {
			get { return textExportMode; }
			set { textExportMode = value; }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("XlsExportOptionsBaseExportHyperlinks"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.XlsExportOptions.ExportHyperlinks"),
		DefaultValue(true),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty]
		public bool ExportHyperlinks { get { return exportHyperlinks; } set { exportHyperlinks = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("XlsExportOptionsBaseShowGridLines"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.XlsExportOptions.ShowGridLines"),
		DefaultValue(false),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty]
		public bool ShowGridLines { get { return showGridLines; } set { showGridLines = value; } }
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.XlsExportOptionsBase.FitToPrintedPageWidth"),
		DefaultValue(false),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty]
		public bool FitToPrintedPageWidth { get { return fitToPrintedPageWidth; } set { fitToPrintedPageWidth = value; } }
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.XlsExportOptionsBase.RightToLeftDocument"),
		DefaultValue(DefaultBoolean.Default),
		TypeConverter(typeof(DefaultBooleanConverter)),
		XtraSerializableProperty]
		public DefaultBoolean RightToLeftDocument { get { return rightToLeftDocument; } set { rightToLeftDocument = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("XlsExportOptionsBaseSheetName"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.XlsExportOptions.SheetName"),
		DefaultValue(DefaultSheetName),
		Localizable(true),
		XtraSerializableProperty]
		public string SheetName { get { return sheetName; } set { sheetName = ValidSheetName(value); } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("XlsExportOptionsBaseDocumentOptions"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.XlsExportOptions.DocumentOptions"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public XlDocumentOptions DocumentOptions { get { return documentOptions; } }
		#endregion
		public override void Assign(ExportOptionsBase source) {
			XlsExportOptionsBase xlsSource = (XlsExportOptionsBase)source;
			textExportMode = xlsSource.TextExportMode;
			showGridLines = xlsSource.ShowGridLines;
			exportHyperlinks = xlsSource.ExportHyperlinks;
			rawDataMode = xlsSource.RawDataMode;
			sheetName = ValidSheetName(xlsSource.SheetName);
			fitToPrintedPageWidth = xlsSource.FitToPrintedPageWidth;
			rightToLeftDocument = xlsSource.RightToLeftDocument;
			documentOptions = xlsSource.DocumentOptions;
		}
		bool ShouldSerializeDocumentOptions() {
			return DocumentOptions.ShouldSerialize();
		}
		protected internal override bool ShouldSerialize() {
			return textExportMode != DefaultTextExportMode || exportHyperlinks != true || showGridLines != false || rawDataMode != false ||
				fitToPrintedPageWidth != false || rightToLeftDocument != DefaultBoolean.Default || sheetName != DefaultSheetName ||
				ShouldSerializeDocumentOptions() || base.ShouldSerialize();
		}
	}
	[DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.XlsExportOptions")]
	public class XlsExportOptions : XlsExportOptionsBase {
		const XlsExportMode DefaultExportMode = XlsExportMode.SingleFile;
		const WorkbookColorPaletteCompliance DefaultWorkbookColorPaletteCompliance = WorkbookColorPaletteCompliance.ReducePaletteForExactColors;
		bool suppress256ColumnsWarning = false;
		bool suppress65536RowsWarning = false;
		XlsExportMode exportMode = DefaultExportMode;
		WorkbookColorPaletteCompliance workbookColorPaletteCompliance = DefaultWorkbookColorPaletteCompliance;
		protected internal override bool IsMultiplePaged {
			get { return ExportMode == XlsExportMode.DifferentFiles || SinglePageForDifferentFiles; }
		}
		internal bool SinglePageForDifferentFiles { get; set; }
		XlsExportOptions(XlsExportOptions source)
			: base(source) {
		}
		public XlsExportOptions() : this(TextExportMode.Value) { }
		public XlsExportOptions(TextExportMode textExportMode)
			: base(textExportMode) {
		}
		public XlsExportOptions(TextExportMode textExportMode, bool showGridLines)
			: base(textExportMode, showGridLines) {
		}
		public XlsExportOptions(TextExportMode textExportMode, bool showGridLines, bool exportHyperlinks)
			: base(textExportMode, showGridLines, exportHyperlinks) {
		}
		public XlsExportOptions(TextExportMode textExportMode, bool showGridLines, bool exportHyperlinks, bool fitToPrintedPageWidth)
			: base(textExportMode, showGridLines, exportHyperlinks, fitToPrintedPageWidth) {
		}
		public XlsExportOptions(TextExportMode textExportMode, bool showGridLines, bool exportHyperlinks, bool suppress256ColumnsWarning, bool suppress65536RowsWarning, WorkbookColorPaletteCompliance workbookColorPaletteCompliance)
			: base(textExportMode, showGridLines, exportHyperlinks) {
			this.suppress256ColumnsWarning = suppress256ColumnsWarning;
			this.suppress65536RowsWarning = suppress65536RowsWarning;
			this.workbookColorPaletteCompliance = workbookColorPaletteCompliance;
		}
		#region properties
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("XlsExportOptionsWorkbookColorPaletteCompliance"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.XlsExportOptions.WorkbookColorPaletteCompliance"),
		DefaultValue(DefaultWorkbookColorPaletteCompliance),
		XtraSerializableProperty,]
		public WorkbookColorPaletteCompliance WorkbookColorPaletteCompliance { get { return workbookColorPaletteCompliance; } set { workbookColorPaletteCompliance = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("XlsExportOptionsSuppress256ColumnsWarning"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.XlsExportOptions.Suppress256ColumnsWarning"),
		DefaultValue(false),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty,]
		public bool Suppress256ColumnsWarning { get { return suppress256ColumnsWarning; } set { suppress256ColumnsWarning = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("XlsExportOptionsSuppress65536RowsWarning"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.XlsExportOptions.Suppress65536RowsWarning"),
		DefaultValue(false),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty,]
		public bool Suppress65536RowsWarning { get { return suppress65536RowsWarning; } set { suppress65536RowsWarning = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("XlsExportOptionsExportMode"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.XlsExportOptions.ExportMode"),
		DefaultValue(DefaultExportMode),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty,
		]
		public XlsExportMode ExportMode { get { return exportMode; } set { exportMode = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("XlsExportOptionsPageRange"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.XlsExportOptions.PageRange"),
#if !DXPORTABLE
		TypeConverter(typeof(XlsPageRangeConverter)),
#endif
		XtraSerializableProperty,
		]
		public override string PageRange { get { return base.PageRange; } set { base.PageRange = value; } }
		#endregion
		protected internal override ExportOptionsBase CloneOptions() {
			return new XlsExportOptions(this);
		}
		public override void Assign(ExportOptionsBase source) {
			base.Assign(source);
			XlsExportOptions xlsSource = (XlsExportOptions)source;
			suppress256ColumnsWarning = xlsSource.Suppress256ColumnsWarning;
			suppress65536RowsWarning = xlsSource.Suppress65536RowsWarning;
			workbookColorPaletteCompliance = xlsSource.workbookColorPaletteCompliance;
			ExportMode = xlsSource.ExportMode;
			PageRange = xlsSource.PageRange;
			SinglePageForDifferentFiles = xlsSource.SinglePageForDifferentFiles;
		}
		protected internal override bool ShouldSerialize() {
			return suppress256ColumnsWarning != false || suppress65536RowsWarning != false || exportMode != DefaultExportMode || workbookColorPaletteCompliance != DefaultWorkbookColorPaletteCompliance || base.ShouldSerialize();
		}
	}
	[DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.XlsxExportOptions")]
	public class XlsxExportOptions : XlsExportOptionsBase {
		const XlsxExportMode DefaultExportMode = XlsxExportMode.SingleFile;
		XlsxExportMode exportMode = DefaultExportMode;
		XlsxExportOptions(XlsxExportOptions source)
			: base(source) {
		}
		public XlsxExportOptions() : this(TextExportMode.Value) { }
		public XlsxExportOptions(TextExportMode textExportMode)
			: base(textExportMode) {
		}
		public XlsxExportOptions(TextExportMode textExportMode, bool showGridLines)
			: base(textExportMode, showGridLines) {
		}
		public XlsxExportOptions(TextExportMode textExportMode, bool showGridLines, bool exportHyperlinks)
			: base(textExportMode, showGridLines, exportHyperlinks) {
		}
		public XlsxExportOptions(TextExportMode textExportMode, bool showGridLines, bool exportHyperlinks, bool fitToPrintedPageWidth)
			: base(textExportMode, showGridLines, exportHyperlinks, fitToPrintedPageWidth) {
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("XlsxExportOptionsExportMode"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.XlsxExportOptions.ExportMode"),
		DefaultValue(DefaultExportMode),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty,
		]
		public XlsxExportMode ExportMode { get { return exportMode; } set { exportMode = value; } }
		protected internal override bool IsMultiplePaged {
			get { return ExportMode == XlsxExportMode.DifferentFiles || ExportMode == XlsxExportMode.SingleFilePageByPage; }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("XlsxExportOptionsPageRange"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.XlsxExportOptions.PageRange"),
#if !DXPORTABLE
		TypeConverter(typeof(XlsxPageRangeConverter)),
#endif
		XtraSerializableProperty,
		]
		public override string PageRange { get { return base.PageRange; } set { base.PageRange = value; } }
		protected internal override ExportOptionsBase CloneOptions() {
			return new XlsxExportOptions(this);
		}
		public override void Assign(ExportOptionsBase source) {
			base.Assign(source);
			XlsxExportOptions xlsxSource = (XlsxExportOptions)source;
			exportMode = xlsxSource.ExportMode;
			PageRange = xlsxSource.PageRange;
		}
		protected internal override bool ShouldSerialize() {
			return exportMode != DefaultExportMode || base.ShouldSerialize();
		}
	}
	public class CsvExportOptionsEx : CsvExportOptions, IDataAwareExportOptions {
		public CsvExportOptionsEx() {
			Init();
		}
		public CsvExportOptionsEx(string separator, Encoding encoding, TextExportMode textExportMode, bool skipEmptyRows, bool skipEmptyColumns)
			: base(separator, encoding, textExportMode, skipEmptyRows, skipEmptyColumns) {
			Init();
		}
		DefaultBoolean IDataAwareExportOptions.AllowSortingAndFiltering { get { return DefaultBoolean.False; } set { } }
		DefaultBoolean IDataAwareExportOptions.ShowTotalSummaries { get { return DefaultBoolean.False; } set { } }
		DefaultBoolean IDataAwareExportOptions.AllowCellMerge { get { return DefaultBoolean.False; } set { } }
		DefaultBoolean IDataAwareExportOptions.AllowLookupValues { get { return DefaultBoolean.False; } set { } }
		DefaultBoolean IDataAwareExportOptions.AllowGrouping { get { return DefaultBoolean.False; } set { } }
		DefaultBoolean IDataAwareExportOptions.AllowSparklines { get { return DefaultBoolean.False; } set { } }
		GroupState IDataAwareExportOptions.GroupState { get { return GroupState.Default; } set { } }
		DefaultBoolean IDataAwareExportOptions.AllowFixedColumnHeaderPanel { get { return DefaultBoolean.False; } set { } }
		DefaultBoolean IDataAwareExportOptions.ShowGroupSummaries { get { return DefaultBoolean.False; } set { } }
		DefaultBoolean IDataAwareExportOptions.AllowFixedColumns { get { return DefaultBoolean.False; } set { } }
		DefaultBoolean IDataAwareExportOptions.ShowPageTitle { get { return DefaultBoolean.False; } set { } }
		DefaultBoolean IDataAwareExportOptions.ShowColumnHeaders { get; set; }
		DefaultBoolean IDataAwareExportOptions.AllowHorzLines { get { return DefaultBoolean.False; } set { } }
		DefaultBoolean IDataAwareExportOptions.AllowVertLines { get { return DefaultBoolean.False; } set { } }
		DefaultBoolean IDataAwareExportOptions.AllowHyperLinks { get { return DefaultBoolean.False; } set { } }
		DefaultBoolean IDataAwareExportOptions.RightToLeftDocument { get { return DefaultBoolean.False; } set { } }
		string IDataAwareExportOptions.CSVSeparator { get { return Separator; } set { Separator = value; } }
		Encoding IDataAwareExportOptions.CSVEncoding { get { return Encoding; } set { Encoding = value; } }
		public bool WritePreamble { get; set; }
		ExportTarget IDataAwareExportOptions.ExportTarget { get { return ExportTarget.Csv; } set { } }
		string IDataAwareExportOptions.SheetName { get { return null; } set { } }
		public event ExportProgressCallback ExportProgress;
		public event CustomizeCellEventHandler CustomizeCell;
		event CustomizeSheetSettingsEventHandler IDataAwareExportOptions.CustomizeSheetSettings{
			add { throw new NotImplementedException(); } remove { throw new NotImplementedException(); } }
		event CustomizeSheetHeaderEventHandler IDataAwareExportOptions.CustomizeSheetHeader{
			add { throw new NotImplementedException(); } remove { throw new NotImplementedException(); } }
		event CustomizeSheetFooterEventHandler IDataAwareExportOptions.CustomizeSheetFooter{
			add { throw new NotImplementedException(); } remove { throw new NotImplementedException(); } }
		event AfterAddRowEventHandler IDataAwareExportOptions.AfterAddRow{
			add { throw new NotImplementedException(); } remove { throw new NotImplementedException(); } }
		bool IDataAwareExportOptions.CanRaiseAfterAddRow {
			get { return false; }
		}
		bool IDataAwareExportOptions.CanRaiseCustomizeCellEvent {
			get { return CustomizeCell != null; }
		}
		bool IDataAwareExportOptions.CanRaiseCustomizeSheetSettingsEvent{
			get { return false; }
		}
		bool IDataAwareExportOptions.CanRaiseCustomizeHeaderEvent {
			get { return false; }
		}
		bool IDataAwareExportOptions.CanRaiseCustomizeFooterEvent {
			get { return false; }
		}
		void IDataAwareExportOptions.RaiseAfterAddRowEvent(AfterAddRowEventArgs e) {
		}
		void IDataAwareExportOptions.RaiseCustomizeCellEvent(CustomizeCellEventArgs e) {
			if(CustomizeCell != null) CustomizeCell(e);
		}
		void IDataAwareExportOptions.RaiseCustomizeSheetSettingsEvent(CustomizeSheetEventArgs e) {
		}
		void IDataAwareExportOptions.RaiseCustomizeSheetHeaderEvent(ContextEventArgs e) {
		}
		void IDataAwareExportOptions.RaiseCustomizeSheetFooterEvent(ContextEventArgs e) {
		}
		void IDataAwareExportOptions.ReportProgress(ProgressChangedEventArgs e) {
			if(ExportProgress != null) ExportProgress(e);
		}
		void Init() {
			((IDataAwareExportOptions)this).ShowColumnHeaders = DefaultBoolean.Default;
		}
		void IDataAwareExportOptions.InitDefaults() {
			((IDataAwareExportOptions)this).ShowColumnHeaders = DataAwareExportOptionsFactory.UpdateDefaultBoolean(((IDataAwareExportOptions)this).ShowColumnHeaders, true);
		}
		ExportType exportTypeCore = ExportType.Default;
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("CsvExportOptionsExExportType"),
#endif
 DefaultValue(ExportType.DataAware), XtraSerializableProperty]
		public ExportType ExportType {
			get { return exportTypeCore == DevExpress.Export.ExportType.Default ? DevExpress.Export.ExportSettings.DefaultExportType : exportTypeCore; }
			set { exportTypeCore = value; }
		}
	}
	public class XlsxExportOptionsEx : XlsxExportOptions, IDataAwareExportOptions {
		public XlsxExportOptionsEx() {
			Init();
		}
		public XlsxExportOptionsEx(TextExportMode textExportMode)
			: base(textExportMode) {
			Init();
		}
		DefaultBoolean allowSortingAndFiltering;
		DefaultBoolean allowCellMerge;
		DefaultBoolean allowLookupValues;
		DefaultBoolean allowFixedColumnHeaderPanel;
		DefaultBoolean allowFixedColumns;
		DefaultBoolean allowHyperlinks;
		DefaultBoolean allowSparklines;
		bool suppressMaxColumnsWarning = false;
		bool suppressMaxRowsWarning = false;
		[XtraSerializableProperty]
		[DefaultValue(false)]
		public bool SuppressMaxColumnsWarning { get { return suppressMaxColumnsWarning; } set { suppressMaxColumnsWarning = value; } }
		[XtraSerializableProperty]
		[DefaultValue(false)]
		public bool SuppressMaxRowsWarning { get { return suppressMaxRowsWarning; } set { suppressMaxRowsWarning = value; } }
		[XtraSerializableProperty]
		[DefaultValue(DefaultBoolean.Default)]
		[TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean AllowGrouping { get; set; }
		[XtraSerializableProperty]
		[DefaultValue(DefaultBoolean.Default)]
		[TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean AllowSparklines{
			get { return DataAwareExportOptionsFactory.GetActualOptionValue(allowSparklines, RawDataMode); }
			set { allowSparklines = value; }
		}
		[XtraSerializableProperty]
		[DefaultValue(GroupState.Default)]
		[TypeConverter(typeof(EnumConverter))]
		public GroupState GroupState { get; set; }
		[XtraSerializableProperty]
		[DefaultValue(DefaultBoolean.Default)]
		[TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean ShowTotalSummaries { get; set; }
		[XtraSerializableProperty]
		[DefaultValue(DefaultBoolean.Default)]
		[TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean ShowGroupSummaries { get; set; }
		[XtraSerializableProperty]
		[DefaultValue(DefaultBoolean.Default)]
		[TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean ShowPageTitle { get; set; }
		[XtraSerializableProperty]
		[DefaultValue(DefaultBoolean.Default)]
		[TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean ShowColumnHeaders { get; set; }
		[XtraSerializableProperty]
		[DefaultValue(DefaultBoolean.Default)]
		[TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean AllowSortingAndFiltering {
			get { return DataAwareExportOptionsFactory.GetActualOptionValue(allowSortingAndFiltering, RawDataMode); }
			set { allowSortingAndFiltering = value; }
		}
		[XtraSerializableProperty]
		[DefaultValue(DefaultBoolean.Default)]
		[TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean AllowCellMerge {
			get { return DataAwareExportOptionsFactory.GetActualOptionValue(allowCellMerge, RawDataMode); }
			set { allowCellMerge = value; }
		}
		[XtraSerializableProperty]
		[DefaultValue(DefaultBoolean.Default)]
		[TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean AllowLookupValues {
			get { return DataAwareExportOptionsFactory.GetActualOptionValue(allowLookupValues, RawDataMode); }
			set { allowLookupValues = value; }
		}
		[XtraSerializableProperty]
		[DefaultValue(DefaultBoolean.Default)]
		[TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean AllowFixedColumnHeaderPanel {
			get { return DataAwareExportOptionsFactory.GetActualOptionValue(allowFixedColumnHeaderPanel, RawDataMode); }
			set { allowFixedColumnHeaderPanel = value; }
		}
		[XtraSerializableProperty]
		[DefaultValue(DefaultBoolean.Default)]
		[TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean AllowFixedColumns {
			get { return DataAwareExportOptionsFactory.GetActualOptionValue(allowFixedColumns, RawDataMode); }
			set { allowFixedColumns = value; }
		}
		[XtraSerializableProperty]
		[DefaultValue(DefaultBoolean.Default)]
		[TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean AllowHyperLinks {
			get { return DataAwareExportOptionsFactory.GetActualOptionValue(allowHyperlinks, RawDataMode); }
			set { allowHyperlinks = value; }
		}
		DefaultBoolean allowHorzLines;
		DefaultBoolean allowVertLines;
		DefaultBoolean IDataAwareExportOptions.AllowHorzLines {
			get { return DataAwareExportOptionsFactory.GetActualOptionValue(allowHorzLines, RawDataMode); }
			set { allowHorzLines = value; }
		}
		DefaultBoolean IDataAwareExportOptions.AllowVertLines {
			get { return DataAwareExportOptionsFactory.GetActualOptionValue(allowVertLines, RawDataMode); }
			set { allowVertLines = value; }
		}
		ExportTarget IDataAwareExportOptions.ExportTarget { get { return ExportTarget.Xlsx; } set { } }
		string IDataAwareExportOptions.CSVSeparator { get { return null; } set { } }
		Encoding IDataAwareExportOptions.CSVEncoding { get { return null; } set { } }
		bool IDataAwareExportOptions.WritePreamble { get { return false; } set { } }
		public event ExportProgressCallback ExportProgress;
		void IDataAwareExportOptions.ReportProgress(ProgressChangedEventArgs e) {
			if(ExportProgress != null) ExportProgress(e);
		}
		public event CustomizeCellEventHandler CustomizeCell;
		public event CustomizeSheetSettingsEventHandler CustomizeSheetSettings;
		public event CustomizeSheetHeaderEventHandler CustomizeSheetHeader;
		public event CustomizeSheetFooterEventHandler CustomizeSheetFooter;
		public event AfterAddRowEventHandler AfterAddRow;
		bool IDataAwareExportOptions.CanRaiseAfterAddRow {
			get { return AfterAddRow != null; }
		}
		bool IDataAwareExportOptions.CanRaiseCustomizeCellEvent {
			get { return CustomizeCell != null; }
		}
		bool IDataAwareExportOptions.CanRaiseCustomizeSheetSettingsEvent {
			get { return CustomizeSheetSettings != null; }
		}
		bool IDataAwareExportOptions.CanRaiseCustomizeHeaderEvent {
			get { return CustomizeSheetHeader != null; }
		}
		bool IDataAwareExportOptions.CanRaiseCustomizeFooterEvent {
			get { return CustomizeSheetFooter != null; }
		}
		void IDataAwareExportOptions.RaiseAfterAddRowEvent(AfterAddRowEventArgs e) {
			if(AfterAddRow != null) AfterAddRow(e);
		}
		void IDataAwareExportOptions.RaiseCustomizeCellEvent(CustomizeCellEventArgs e) {
			if(CustomizeCell != null) CustomizeCell(e);
		}
		void IDataAwareExportOptions.RaiseCustomizeSheetSettingsEvent(CustomizeSheetEventArgs e) {
			if(CustomizeSheetSettings != null) CustomizeSheetSettings(e);
		}
		void IDataAwareExportOptions.RaiseCustomizeSheetHeaderEvent(ContextEventArgs e) {
			if(CustomizeSheetHeader != null) CustomizeSheetHeader(e);
		}
		void IDataAwareExportOptions.RaiseCustomizeSheetFooterEvent(ContextEventArgs e) {
			if(CustomizeSheetFooter != null) CustomizeSheetFooter(e);
		}
		void IDataAwareExportOptions.InitDefaults() {
			AllowSortingAndFiltering = DataAwareExportOptionsFactory.UpdateDefaultBoolean(AllowSortingAndFiltering, true);
			AllowLookupValues = DataAwareExportOptionsFactory.UpdateDefaultBoolean(AllowLookupValues, true);
			AllowFixedColumns = DataAwareExportOptionsFactory.UpdateDefaultBoolean(AllowFixedColumns, true);
			AllowFixedColumnHeaderPanel = DataAwareExportOptionsFactory.UpdateDefaultBoolean(AllowFixedColumnHeaderPanel, true);
			AllowGrouping = DataAwareExportOptionsFactory.UpdateDefaultBoolean(AllowGrouping, true);
			ShowGroupSummaries = DataAwareExportOptionsFactory.UpdateDefaultBoolean(ShowGroupSummaries, true);
			ShowTotalSummaries = DataAwareExportOptionsFactory.UpdateDefaultBoolean(ShowTotalSummaries, true);
			AllowCellMerge = DataAwareExportOptionsFactory.UpdateDefaultBoolean(AllowCellMerge, false);
			ShowColumnHeaders = DataAwareExportOptionsFactory.UpdateDefaultBoolean(ShowColumnHeaders, true);
			((IDataAwareExportOptions)this).AllowHorzLines = DataAwareExportOptionsFactory.UpdateDefaultBoolean(((IDataAwareExportOptions)this).AllowHorzLines, false);
			((IDataAwareExportOptions)this).AllowVertLines = DataAwareExportOptionsFactory.UpdateDefaultBoolean(((IDataAwareExportOptions)this).AllowVertLines, false);
			AllowHyperLinks = DataAwareExportOptionsFactory.UpdateDefaultBoolean(AllowHyperLinks, false);
		}
		void Init() {
			AllowSortingAndFiltering = DefaultBoolean.Default;
			AllowLookupValues = DefaultBoolean.Default;
			ShowPageTitle = DefaultBoolean.Default;
			AllowFixedColumns = DefaultBoolean.Default;
			AllowFixedColumnHeaderPanel = DefaultBoolean.Default;
			ShowTotalSummaries = DefaultBoolean.Default;
			AllowGrouping = DefaultBoolean.Default;
			ShowGroupSummaries = DefaultBoolean.Default;
			ShowColumnHeaders = DefaultBoolean.Default;
			AllowHyperLinks = DefaultBoolean.Default;
			AllowCellMerge = DefaultBoolean.Default;
			((IDataAwareExportOptions)this).AllowVertLines = DefaultBoolean.Default;
			((IDataAwareExportOptions)this).AllowHorzLines = DefaultBoolean.Default;
			ShowGridLines = true;
		}
		ExportType exportTypeCore = ExportType.Default;
		[DefaultValue(ExportType.DataAware),TypeConverter(typeof(EnumTypeConverter)),XtraSerializableProperty]
		public ExportType ExportType {
			get { return exportTypeCore == DevExpress.Export.ExportType.Default ? DevExpress.Export.ExportSettings.DefaultExportType : exportTypeCore; }
			set { exportTypeCore = value; }
		}
	}
	public class XlsExportOptionsEx : XlsExportOptions, IDataAwareExportOptions {
		public XlsExportOptionsEx() {
			Init();
		}
		public XlsExportOptionsEx(TextExportMode textExportMode)
			: base(textExportMode) {
			Init();
		}
		DefaultBoolean allowSortingAndFiltering;
		DefaultBoolean allowCellMerge;
		DefaultBoolean allowLookupValues;
		DefaultBoolean allowFixedColumnHeaderPanel;
		DefaultBoolean allowFixedColumns;
		DefaultBoolean allowHyperlinks;
		[XtraSerializableProperty]
		[DefaultValue(DefaultBoolean.Default)]
		[TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean AllowGrouping { get; set; }
		[XtraSerializableProperty]
		[DefaultValue(DefaultBoolean.Default)]
		[TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean AllowSparklines { get; set; }
		[XtraSerializableProperty]
		[DefaultValue(GroupState.Default)]
		[TypeConverter(typeof(EnumConverter))]
		public GroupState GroupState { get; set; }
		[XtraSerializableProperty]
		[DefaultValue(DefaultBoolean.Default)]
		[TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean ShowTotalSummaries { get; set; }
		[XtraSerializableProperty]
		[DefaultValue(DefaultBoolean.Default)]
		[TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean ShowGroupSummaries { get; set; }
		[XtraSerializableProperty]
		[DefaultValue(DefaultBoolean.Default)]
		[TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean ShowPageTitle { get; set; }
		[XtraSerializableProperty]
		[DefaultValue(DefaultBoolean.Default)]
		[TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean ShowColumnHeaders { get; set; }
		[XtraSerializableProperty]
		[DefaultValue(DefaultBoolean.Default)]
		[TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean AllowSortingAndFiltering {
			get { return DataAwareExportOptionsFactory.GetActualOptionValue(allowSortingAndFiltering, RawDataMode); }
			set { allowSortingAndFiltering = value; }
		}
		[XtraSerializableProperty]
		[DefaultValue(DefaultBoolean.Default)]
		[TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean AllowCellMerge {
			get { return DataAwareExportOptionsFactory.GetActualOptionValue(allowCellMerge, RawDataMode); }
			set { allowCellMerge = value; }
		}
		[XtraSerializableProperty]
		[DefaultValue(DefaultBoolean.Default)]
		[TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean AllowLookupValues {
			get { return DataAwareExportOptionsFactory.GetActualOptionValue(allowLookupValues, RawDataMode); }
			set { allowLookupValues = value; }
		}
		[XtraSerializableProperty]
		[DefaultValue(DefaultBoolean.Default)]
		[TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean AllowFixedColumnHeaderPanel {
			get { return DataAwareExportOptionsFactory.GetActualOptionValue(allowFixedColumnHeaderPanel, RawDataMode); }
			set { allowFixedColumnHeaderPanel = value; }
		}
		[XtraSerializableProperty]
		[DefaultValue(DefaultBoolean.Default)]
		[TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean AllowFixedColumns {
			get { return DataAwareExportOptionsFactory.GetActualOptionValue(allowFixedColumns, RawDataMode); }
			set { allowFixedColumns = value; }
		}
		[XtraSerializableProperty]
		[DefaultValue(DefaultBoolean.Default)]
		[TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean AllowHyperLinks {
			get { return DataAwareExportOptionsFactory.GetActualOptionValue(allowHyperlinks, RawDataMode); }
			set { allowHyperlinks = value; }
		}
		DefaultBoolean allowHorzLines;
		DefaultBoolean allowVertLines;
		DefaultBoolean IDataAwareExportOptions.AllowHorzLines {
			get { return DataAwareExportOptionsFactory.GetActualOptionValue(allowHorzLines, RawDataMode); }
			set { allowHorzLines = value; }
		}
		DefaultBoolean IDataAwareExportOptions.AllowVertLines {
			get { return DataAwareExportOptionsFactory.GetActualOptionValue(allowVertLines, RawDataMode); }
			set { allowVertLines = value; }
		}
		ExportTarget IDataAwareExportOptions.ExportTarget { get { return ExportTarget.Xls; } set { } }
		string IDataAwareExportOptions.CSVSeparator { get { return null; } set { } }
		Encoding IDataAwareExportOptions.CSVEncoding { get { return null; } set { } }
		bool IDataAwareExportOptions.WritePreamble { get { return false; } set { } }
		public event ExportProgressCallback ExportProgress;
		void IDataAwareExportOptions.ReportProgress(ProgressChangedEventArgs e) {
			if(ExportProgress != null) ExportProgress(e);
		}
		public event CustomizeCellEventHandler CustomizeCell;
		public event CustomizeSheetSettingsEventHandler CustomizeSheetSettings;
		public event CustomizeSheetHeaderEventHandler CustomizeSheetHeader;
		public event CustomizeSheetFooterEventHandler CustomizeSheetFooter;
		public event AfterAddRowEventHandler AfterAddRow;
		bool IDataAwareExportOptions.CanRaiseAfterAddRow {
			get { return AfterAddRow != null; }
		}
		bool IDataAwareExportOptions.CanRaiseCustomizeCellEvent {
			get { return CustomizeCell != null; }
		}
		bool IDataAwareExportOptions.CanRaiseCustomizeSheetSettingsEvent{
			get { return CustomizeSheetSettings != null; }
		}
		bool IDataAwareExportOptions.CanRaiseCustomizeHeaderEvent {
			get { return CustomizeSheetHeader != null; }
		}
		bool IDataAwareExportOptions.CanRaiseCustomizeFooterEvent { 
			get { return CustomizeSheetFooter != null; }
		}
		void IDataAwareExportOptions.RaiseAfterAddRowEvent(AfterAddRowEventArgs e) {
			if(AfterAddRow != null) AfterAddRow(e);
		}
		void IDataAwareExportOptions.RaiseCustomizeCellEvent(CustomizeCellEventArgs e) {
			if(CustomizeCell!=null) CustomizeCell(e);
		}
		void IDataAwareExportOptions.RaiseCustomizeSheetSettingsEvent(CustomizeSheetEventArgs e) {
			if(CustomizeSheetSettings != null) CustomizeSheetSettings(e);
		}
		void IDataAwareExportOptions.RaiseCustomizeSheetHeaderEvent(ContextEventArgs e) {
			if(CustomizeSheetHeader != null) CustomizeSheetHeader(e);
		}
		void IDataAwareExportOptions.RaiseCustomizeSheetFooterEvent(ContextEventArgs e) {
			if(CustomizeSheetFooter != null) CustomizeSheetFooter(e);
		}
		void IDataAwareExportOptions.InitDefaults() {
			AllowSortingAndFiltering = DataAwareExportOptionsFactory.UpdateDefaultBoolean(AllowSortingAndFiltering, true);
			AllowLookupValues = DataAwareExportOptionsFactory.UpdateDefaultBoolean(AllowLookupValues, true);
			AllowFixedColumns = DataAwareExportOptionsFactory.UpdateDefaultBoolean(AllowFixedColumns, true);
			AllowFixedColumnHeaderPanel = DataAwareExportOptionsFactory.UpdateDefaultBoolean(AllowFixedColumnHeaderPanel, true);
			AllowGrouping = DataAwareExportOptionsFactory.UpdateDefaultBoolean(AllowGrouping, true);
			ShowGroupSummaries = DataAwareExportOptionsFactory.UpdateDefaultBoolean(ShowGroupSummaries, true);
			ShowTotalSummaries = DataAwareExportOptionsFactory.UpdateDefaultBoolean(ShowTotalSummaries, true);
			AllowCellMerge = DataAwareExportOptionsFactory.UpdateDefaultBoolean(AllowCellMerge, false);
			ShowColumnHeaders = DataAwareExportOptionsFactory.UpdateDefaultBoolean(ShowColumnHeaders, true);
			((IDataAwareExportOptions)this).AllowVertLines = DataAwareExportOptionsFactory.UpdateDefaultBoolean(((IDataAwareExportOptions)this).AllowVertLines, false);
			((IDataAwareExportOptions)this).AllowHorzLines = DataAwareExportOptionsFactory.UpdateDefaultBoolean(((IDataAwareExportOptions)this).AllowHorzLines, false);
			AllowHyperLinks = DataAwareExportOptionsFactory.UpdateDefaultBoolean(AllowHyperLinks, false);
		}
		void Init() {
			AllowSortingAndFiltering = DefaultBoolean.Default;
			AllowLookupValues = DefaultBoolean.Default;
			ShowPageTitle = DefaultBoolean.Default;
			AllowFixedColumns = DefaultBoolean.Default;
			AllowFixedColumnHeaderPanel = DefaultBoolean.Default;
			ShowTotalSummaries = DefaultBoolean.Default;
			AllowGrouping = DefaultBoolean.Default;
			ShowGroupSummaries = DefaultBoolean.Default;
			ShowColumnHeaders = DefaultBoolean.Default;
			AllowHyperLinks = DefaultBoolean.Default;
			AllowCellMerge = DefaultBoolean.Default;
			((IDataAwareExportOptions)this).AllowHorzLines = DefaultBoolean.Default;
			((IDataAwareExportOptions)this).AllowVertLines = DefaultBoolean.Default;
			ShowGridLines = true;
		}
		ExportType exportTypeCore = ExportType.Default;
		[DefaultValue(ExportType.DataAware),TypeConverter(typeof(EnumTypeConverter)),XtraSerializableProperty]
		public ExportType ExportType {
			get { return exportTypeCore == DevExpress.Export.ExportType.Default ? DevExpress.Export.ExportSettings.DefaultExportType : exportTypeCore; }
			set { exportTypeCore = value; }
		}
	}
}
namespace DevExpress.Export {
	public enum GroupState { Default = 0, ExpandAll, CollapseAll }
	public interface IDataAwareExportOptions {
		ExportType ExportType { get; set; }
		DefaultBoolean AllowSortingAndFiltering { get; set; }
		DefaultBoolean ShowTotalSummaries { get; set; }
		DefaultBoolean AllowCellMerge { get; set; }
		DefaultBoolean AllowLookupValues { get; set; }
		DefaultBoolean AllowGrouping { get; set; }
		DefaultBoolean AllowSparklines { get; set; }
		GroupState GroupState { get; set; }
		DefaultBoolean AllowFixedColumnHeaderPanel { get; set; }
		DefaultBoolean ShowGroupSummaries { get; set; }
		DefaultBoolean AllowFixedColumns { get; set; }
		DefaultBoolean ShowPageTitle { get; set; }
		DefaultBoolean ShowColumnHeaders { get; set; }
		DefaultBoolean AllowHyperLinks { get; set; }
		DefaultBoolean RightToLeftDocument { get; set; }
		DefaultBoolean AllowHorzLines { get; set; }
		DefaultBoolean AllowVertLines { get; set; }
		ExportTarget ExportTarget { get; set; }
		string SheetName { get; set; }
		Encoding CSVEncoding { get; set; }
		bool WritePreamble { get; set; }
		string CSVSeparator { get; set; }
		event CustomizeCellEventHandler CustomizeCell;
		event CustomizeSheetSettingsEventHandler CustomizeSheetSettings;
		event CustomizeSheetHeaderEventHandler CustomizeSheetHeader;
		event CustomizeSheetFooterEventHandler CustomizeSheetFooter;
		event AfterAddRowEventHandler AfterAddRow;
		event ExportProgressCallback ExportProgress;
		void RaiseAfterAddRowEvent(AfterAddRowEventArgs e);
		void RaiseCustomizeCellEvent(CustomizeCellEventArgs e);
		void RaiseCustomizeSheetSettingsEvent(CustomizeSheetEventArgs e);
		void RaiseCustomizeSheetHeaderEvent(ContextEventArgs e);
		void RaiseCustomizeSheetFooterEvent(ContextEventArgs e);
		void ReportProgress(ProgressChangedEventArgs e);
		void InitDefaults();
		bool CanRaiseAfterAddRow { get; }
		bool CanRaiseCustomizeCellEvent { get; }
		bool CanRaiseCustomizeSheetSettingsEvent { get; }
		bool CanRaiseCustomizeHeaderEvent { get; }
		bool CanRaiseCustomizeFooterEvent { get; }
	}
	public delegate void CustomizeSheetSettingsEventHandler(CustomizeSheetEventArgs e);
	public delegate void AfterAddRowEventHandler(AfterAddRowEventArgs e);
	public delegate void CustomizeCellEventHandler(CustomizeCellEventArgs e);
	public delegate void CustomizeSheetHeaderEventHandler(ContextEventArgs e);
	public delegate void CustomizeSheetFooterEventHandler(ContextEventArgs e);
	public delegate void ExportProgressCallback(ProgressChangedEventArgs e);
	public class IDataAwareEventArgsBase {
		public IDataAwareEventArgsBase() { Handled = false; } 
		public int DataSourceRowIndex { get; set; }
		public int DocumentRow { get; set; }
		public int RowHandle { get; set; }
		public object DataSourceOwner { get; set; }
		public bool Handled { get; set; }
	}
	public class CustomizeCellEventArgsBase : IDataAwareEventArgsBase {
		public string ColumnFieldName { get; set; }
		public XlFormattingObject Formatting { get; set; }
		public SheetAreaType AreaType { get; set; }
	}
	public class CustomizeCellEventArgs : CustomizeCellEventArgsBase{
		public object Value { get; set; }
		public string Hyperlink { get; set; }
		public ExportSummaryItem SummaryItem { get; set; }
	}
	public class CustomizeFooterCellEventArgs : CustomizeCellEventArgsBase {
		public ExportSummaryItem SummaryItem { get; set; }
	}
	public class CustomizeCellEventArgsExtended : CustomizeCellEventArgs {
		public IColumn Column { get; set; }
		public IRowBase Row { get; set; }
	}
	public class AfterAddRowEventArgs: IDataAwareEventArgsBase{
		public IExportContext ExportContext { get; set; }
	}
	public class ContextEventArgs{
		public ISheetHeaderFooterExportContext ExportContext { get; set; }
	}
	public class CustomizeSheetEventArgs{
		public ISheetCustomizationContext ExportContext { get; set; }
	}
	public static class DataAwareExportOptionsFactory {
		static IDataAwareExportOptions CreateCore<T>(object options, ExportTarget target) {
			if(options is T) return options as IDataAwareExportOptions;
			else {
				IDataAwareExportOptions result = Create(target);
				CsvExportOptions csvOptions = options as CsvExportOptions;
				XlsExportOptionsBase xlsOptions = options as XlsExportOptionsBase;
				if(xlsOptions != null) {
					XlsExportOptionsBase resXlsEO = result as XlsExportOptionsBase;
					if(resXlsEO != null) {
						resXlsEO.SheetName = xlsOptions.SheetName;
						resXlsEO.ShowGridLines = xlsOptions.ShowGridLines;
					}
				}
				if(csvOptions != null) {
					result.CSVEncoding = csvOptions.Encoding;
					result.CSVSeparator = csvOptions.Separator;
				}
				return result;
			}
		}
		public static IDataAwareExportOptions Create(ExportTarget target, object options) {
			if(options == null) return Create(target);
			else {
				switch(target) {
					case ExportTarget.Csv: return CreateCore<CsvExportOptionsEx>(options, target);
					case ExportTarget.Xls: return CreateCore<XlsExportOptionsEx>(options, target);
					case ExportTarget.Xlsx: return CreateCore<XlsxExportOptionsEx>(options, target);
					default: throw new NotImplementedException();
				}
			}
		}
		public static IDataAwareExportOptions Create(ExportTarget target) {
			switch(target) {
				case ExportTarget.Csv: return new CsvExportOptionsEx();
				case ExportTarget.Xls: return new XlsExportOptionsEx();
				case ExportTarget.Xlsx: return new XlsxExportOptionsEx();
				default: throw new NotImplementedException();
			}
		}
		public static DefaultBoolean UpdateDefaultBoolean(DefaultBoolean oldValue, bool suggestedValue) {
			if(oldValue != DefaultBoolean.Default) return oldValue;
			return suggestedValue ? DefaultBoolean.True : DefaultBoolean.False;
		}
		public static DefaultBoolean GetActualOptionValue(DefaultBoolean currentValue, bool condition){
			return condition ? DefaultBoolean.False : currentValue;
		}
	}
}
