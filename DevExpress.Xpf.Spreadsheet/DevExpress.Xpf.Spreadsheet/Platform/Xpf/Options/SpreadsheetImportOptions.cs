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

using DevExpress.Spreadsheet;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Import;
using DevExpress.XtraSpreadsheet.Internal;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
namespace DevExpress.Xpf.Spreadsheet {
	#region SpreadsheetImportOptions
	public class SpreadsheetImportOptions : DependencyObject {
		#region Fields
		public static readonly DependencyProperty CsvProperty;
		public static readonly DependencyProperty OpenXmlProperty;
		public static readonly DependencyProperty TxtProperty;
		public static readonly DependencyProperty XlsProperty;
		public static readonly DependencyProperty XlsmProperty;
		public static readonly DependencyProperty FallbackFormatProperty;
		public static readonly DependencyProperty ThrowExceptionOnInvalidDocumentProperty;
		public static readonly DependencyProperty WorksheetNameValidationTypeProperty;
		WorkbookImportOptions source;
		#endregion
		static SpreadsheetImportOptions() {
			Type ownerType = typeof(SpreadsheetImportOptions);
			CsvProperty = DependencyProperty.Register("Csv", typeof(SpreadsheetCsvImportOptions), ownerType,
				new FrameworkPropertyMetadata((d, e) => ((SpreadsheetImportOptions)d).OnCsvChanged((SpreadsheetCsvImportOptions)e.OldValue, (SpreadsheetCsvImportOptions)e.NewValue)));
			OpenXmlProperty = DependencyProperty.Register("OpenXml", typeof(SpreadsheetOpenXmlImportOptions), ownerType,
				new FrameworkPropertyMetadata((d, e) => ((SpreadsheetImportOptions)d).OnOpenXmlChanged((SpreadsheetOpenXmlImportOptions)e.OldValue, (SpreadsheetOpenXmlImportOptions)e.NewValue)));
			TxtProperty = DependencyProperty.Register("Txt", typeof(SpreadsheetTxtImportOptions), ownerType,
				new FrameworkPropertyMetadata((d, e) => ((SpreadsheetImportOptions)d).OnTxtChanged((SpreadsheetTxtImportOptions)e.OldValue, (SpreadsheetTxtImportOptions)e.NewValue)));
			XlsProperty = DependencyProperty.Register("Xls", typeof(SpreadsheetXlsImportOptions), ownerType,
				new FrameworkPropertyMetadata((d, e) => ((SpreadsheetImportOptions)d).OnXlsChanged((SpreadsheetXlsImportOptions)e.OldValue, (SpreadsheetXlsImportOptions)e.NewValue)));
			XlsmProperty = DependencyProperty.Register("Xlsm", typeof(SpreadsheetXlsmImportOptions), ownerType,
				new FrameworkPropertyMetadata((d, e) => ((SpreadsheetImportOptions)d).OnXlsmChanged((SpreadsheetXlsmImportOptions)e.OldValue, (SpreadsheetXlsmImportOptions)e.NewValue)));
			FallbackFormatProperty = DependencyProperty.Register("FallbackFormat", typeof(DocumentFormat), ownerType,
				new FrameworkPropertyMetadata(DocumentFormat.OpenXml));
			ThrowExceptionOnInvalidDocumentProperty = DependencyProperty.Register("ThrowExceptionOnInvalidDocument", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false));
			WorksheetNameValidationTypeProperty = DependencyProperty.Register("WorksheetNameValidationType", typeof(WorksheetNameValidationType), ownerType,
				new FrameworkPropertyMetadata(WorksheetNameValidationType.Check));
		}
		public SpreadsheetImportOptions() {
			Csv = new SpreadsheetCsvImportOptions();
			OpenXml = new SpreadsheetOpenXmlImportOptions();
			Txt = new SpreadsheetTxtImportOptions();
			Xls = new SpreadsheetXlsImportOptions();
			Xlsm = new SpreadsheetXlsmImportOptions();
		}
		#region Properties
		public SpreadsheetCsvImportOptions Csv {
			get { return (SpreadsheetCsvImportOptions)GetValue(CsvProperty); }
			set { SetValue(CsvProperty, value); }
		}
		public SpreadsheetOpenXmlImportOptions OpenXml {
			get { return (SpreadsheetOpenXmlImportOptions)GetValue(OpenXmlProperty); }
			set { SetValue(OpenXmlProperty, value); }
		}
		public SpreadsheetTxtImportOptions Txt {
			get { return (SpreadsheetTxtImportOptions)GetValue(TxtProperty); }
			set { SetValue(TxtProperty, value); }
		}
		public SpreadsheetXlsImportOptions Xls {
			get { return (SpreadsheetXlsImportOptions)GetValue(XlsProperty); }
			set { SetValue(XlsProperty, value); }
		}
		public SpreadsheetXlsmImportOptions Xlsm {
			get { return (SpreadsheetXlsmImportOptions)GetValue(XlsmProperty); }
			set { SetValue(XlsmProperty, value); }
		}
		public DocumentFormat FallbackFormat {
			get { return (DocumentFormat)GetValue(FallbackFormatProperty); }
			set { SetValue(FallbackFormatProperty, value); }
		}
		public bool ThrowExceptionOnInvalidDocument {
			get { return (bool)GetValue(ThrowExceptionOnInvalidDocumentProperty); }
			set { SetValue(ThrowExceptionOnInvalidDocumentProperty, value); }
		}
		public WorksheetNameValidationType WorksheetNameValidationType {
			get { return (WorksheetNameValidationType)GetValue(WorksheetNameValidationTypeProperty); }
			set { SetValue(WorksheetNameValidationTypeProperty, value); }
		}
		#endregion
		void OnCsvChanged(SpreadsheetCsvImportOptions oldValue, SpreadsheetCsvImportOptions newValue) {
			if (oldValue != null)
				oldValue.Reset();
			if (newValue != null && source != null)
				newValue.SetSource(source.Csv);
		}
		void OnOpenXmlChanged(SpreadsheetOpenXmlImportOptions oldValue, SpreadsheetOpenXmlImportOptions newValue) {
			if (oldValue != null)
				oldValue.Reset();
			if (newValue != null && source != null)
				newValue.SetSource(source.OpenXml);
		}
		void OnTxtChanged(SpreadsheetTxtImportOptions oldValue, SpreadsheetTxtImportOptions newValue) {
			if (oldValue != null)
				oldValue.Reset();
			if (newValue != null && source != null)
				newValue.SetSource(source.Txt);
		}
		void OnXlsChanged(SpreadsheetXlsImportOptions oldValue, SpreadsheetXlsImportOptions newValue) {
			if (oldValue != null)
				oldValue.Reset();
			if (newValue != null && source != null)
				newValue.SetSource(source.Xls);
		}
		void OnXlsmChanged(SpreadsheetXlsmImportOptions oldValue, SpreadsheetXlsmImportOptions newValue) {
			if (oldValue != null)
				oldValue.Reset();
			if (newValue != null && source != null)
				newValue.SetSource(source.Xlsm);
		}
		internal void SetSource(WorkbookImportOptions source) {
			Guard.ArgumentNotNull(source, "source");
			this.source = source;
			UpdateSourceProperties();
			BindProperties();
			Csv.SetSource(source.Csv);
			OpenXml.SetSource(source.OpenXml);
			Txt.SetSource(source.Txt);
			Xls.SetSource(source.Xls);
			Xlsm.SetSource(source.Xlsm);
		}
		void UpdateSourceProperties() {
			if (FallbackFormat != (DocumentFormat)GetDefaultValue(FallbackFormatProperty))
				source.FallbackFormat = FallbackFormat;
			if (ThrowExceptionOnInvalidDocument != (bool)GetDefaultValue(ThrowExceptionOnInvalidDocumentProperty))
				source.ThrowExceptionOnInvalidDocument = ThrowExceptionOnInvalidDocument;
			if (WorksheetNameValidationType != (WorksheetNameValidationType)GetDefaultValue(WorksheetNameValidationTypeProperty))
				source.WorksheetNameValidationType = WorksheetNameValidationType;
		}
		object GetDefaultValue(DependencyProperty property) {
			return property.DefaultMetadata.DefaultValue;
		}
		void BindProperties() {
			BindFallbackFormatProperty();
			BindThrowExceptionOnInvalidDocumentProperty();
			BindWorksheetNameValidationTypeProperty();
		}
		void BindFallbackFormatProperty() {
			Binding bind = new Binding("FallbackFormat") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, FallbackFormatProperty, bind);
		}
		void BindThrowExceptionOnInvalidDocumentProperty() {
			Binding bind = new Binding("ThrowExceptionOnInvalidDocument") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, ThrowExceptionOnInvalidDocumentProperty, bind);
		}
		void BindWorksheetNameValidationTypeProperty() {
			Binding bind = new Binding("WorksheetNameValidationType") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, WorksheetNameValidationTypeProperty, bind);
		}
		public void Reset() {
			if (source != null)
				source.Reset();
		}
	}
	#endregion
	#region SpreadsheetCsvImportOptions
	public class SpreadsheetCsvImportOptions : DependencyObject {
		#region Fields
		public static readonly DependencyProperty AutoDetectEncodingProperty;
		public static readonly DependencyProperty CellIndexOutOfRangeStrategyProperty;
		public static readonly DependencyProperty CellReferenceStyleProperty;
		public static readonly DependencyProperty ClearWorksheetBeforeImportProperty;
		public static readonly DependencyProperty CultureProperty;
		public static readonly DependencyProperty DelimiterProperty;
		public static readonly DependencyProperty MaxRowCountToImportProperty;
		public static readonly DependencyProperty NewlineTypeProperty;
		public static readonly DependencyProperty StartCellToInsertProperty;
		public static readonly DependencyProperty StartRowToImportProperty;
		public static readonly DependencyProperty TextQualifierProperty;
		public static readonly DependencyProperty TrimBlanksProperty;
		public static readonly DependencyProperty ValueTypeDetectModeProperty;
		public static readonly DependencyProperty WorksheetNameProperty;
		CsvDocumentImporterOptions source;
		#endregion
		static SpreadsheetCsvImportOptions() {
			Type ownerType = typeof(SpreadsheetCsvImportOptions);
			AutoDetectEncodingProperty = DependencyProperty.Register("AutoDetectEncoding", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(true));
			CellIndexOutOfRangeStrategyProperty = DependencyProperty.Register("CellIndexOutOfRangeStrategy", typeof(CsvImportCellIndexStrategy), ownerType,
				new FrameworkPropertyMetadata(TextDocumentImporterOptionsBase.DefaultCellIndexOutOfRangeStrategy));
			CellReferenceStyleProperty = DependencyProperty.Register("CellReferenceStyle", typeof(CellReferenceStyle), ownerType,
				new FrameworkPropertyMetadata(TextDocumentImporterOptionsBase.DefaultCellReferenceStyle));
			ClearWorksheetBeforeImportProperty = DependencyProperty.Register("ClearWorksheetBeforeImport", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(TextDocumentImporterOptionsBase.DefaultClearSheetBeforeImport));
			CultureProperty = DependencyProperty.Register("Culture", typeof(CultureInfo), ownerType,
				new FrameworkPropertyMetadata(CultureInfo.InvariantCulture));
			DelimiterProperty = DependencyProperty.Register("Delimiter", typeof(char), ownerType,
				new FrameworkPropertyMetadata('\0'));
			MaxRowCountToImportProperty = DependencyProperty.Register("MaxRowCountToImport", typeof(int), ownerType,
				new FrameworkPropertyMetadata(TextDocumentImporterOptionsBase.DefaultMaxRows));
			NewlineTypeProperty = DependencyProperty.Register("NewlineType", typeof(NewlineType), ownerType,
				new FrameworkPropertyMetadata(TextDocumentImporterOptionsBase.DefaultNewlineType));
			StartCellToInsertProperty = DependencyProperty.Register("StartCellToInsert", typeof(string), ownerType,
				new FrameworkPropertyMetadata(TextDocumentImporterOptionsBase.DefaultStartCellToInsert));
			StartRowToImportProperty = DependencyProperty.Register("StartRowToImport", typeof(int), ownerType,
				new FrameworkPropertyMetadata(TextDocumentImporterOptionsBase.DefaultSkipHeaderLines));
			TextQualifierProperty = DependencyProperty.Register("TextQualifier", typeof(char), ownerType,
				new FrameworkPropertyMetadata(TextDocumentImporterOptionsBase.DefaultTextQualifier));
			TrimBlanksProperty = DependencyProperty.Register("TrimBlanks", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(TextDocumentImporterOptionsBase.DefaultTrimBlanks));
			ValueTypeDetectModeProperty = DependencyProperty.Register("ValueTypeDetectMode", typeof(CsvValueTypeDetectMode), ownerType,
				new FrameworkPropertyMetadata(TextDocumentImporterOptionsBase.DefaultValueTypeDetectMode));
			WorksheetNameProperty = DependencyProperty.Register("WorksheetName", typeof(string), ownerType,
				new FrameworkPropertyMetadata(String.Empty));
		}
		#region Properties
		public bool AutoDetectEncoding {
			get { return (bool)GetValue(AutoDetectEncodingProperty); }
			set { SetValue(AutoDetectEncodingProperty, value); }
		}
		public CsvImportCellIndexStrategy CellIndexOutOfRangeStrategy {
			get { return (CsvImportCellIndexStrategy)GetValue(CellIndexOutOfRangeStrategyProperty); }
			set { SetValue(CellIndexOutOfRangeStrategyProperty, value); }
		}
		public CellReferenceStyle CellReferenceStyle {
			get { return (CellReferenceStyle)GetValue(CellReferenceStyleProperty); }
			set { SetValue(CellReferenceStyleProperty, value); }
		}
		public bool ClearWorksheetBeforeImport {
			get { return (bool)GetValue(ClearWorksheetBeforeImportProperty); }
			set { SetValue(ClearWorksheetBeforeImportProperty, value); }
		}
		public CultureInfo Culture {
			get { return (CultureInfo)GetValue(CultureProperty); }
			set { SetValue(CultureProperty, value); }
		}
		public char Delimiter {
			get { return (char)GetValue(DelimiterProperty); }
			set { SetValue(DelimiterProperty, value); }
		}
		public int MaxRowCountToImport {
			get { return (int)GetValue(MaxRowCountToImportProperty); }
			set { SetValue(MaxRowCountToImportProperty, value); }
		}
		public NewlineType NewlineType {
			get { return (NewlineType)GetValue(NewlineTypeProperty); }
			set { SetValue(NewlineTypeProperty, value); }
		}
		public string StartCellToInsert {
			get { return (string)GetValue(StartCellToInsertProperty); }
			set { SetValue(StartCellToInsertProperty, value); }
		}
		public int StartRowToImport {
			get { return (int)GetValue(StartRowToImportProperty); }
			set { SetValue(StartRowToImportProperty, value); }
		}
		public char TextQualifier {
			get { return (char)GetValue(TextQualifierProperty); }
			set { SetValue(TextQualifierProperty, value); }
		}
		public bool TrimBlanks {
			get { return (bool)GetValue(TrimBlanksProperty); }
			set { SetValue(TrimBlanksProperty, value); }
		}
		public CsvValueTypeDetectMode ValueTypeDetectMode {
			get { return (CsvValueTypeDetectMode)GetValue(ValueTypeDetectModeProperty); }
			set { SetValue(ValueTypeDetectModeProperty, value); }
		}
		public string WorksheetName {
			get { return (string)GetValue(WorksheetNameProperty); }
			set { SetValue(WorksheetNameProperty, value); }
		}
		#endregion
		internal void SetSource(CsvDocumentImporterOptions source) {
			Guard.ArgumentNotNull(source, "source");
			this.source = source;
			UpdateSourceProperties();
			BindProperties();
		}
		void UpdateSourceProperties() {
			if (AutoDetectEncoding != (bool)GetDefaultValue(AutoDetectEncodingProperty))
				source.AutoDetectEncoding = AutoDetectEncoding;
			if (CellIndexOutOfRangeStrategy != (CsvImportCellIndexStrategy)GetDefaultValue(CellIndexOutOfRangeStrategyProperty))
				source.CellIndexOutOfRangeStrategy = CellIndexOutOfRangeStrategy;
			if (CellReferenceStyle != (CellReferenceStyle)GetDefaultValue(CellReferenceStyleProperty))
				source.CellReferenceStyle = CellReferenceStyle;
			if (ClearWorksheetBeforeImport != (bool)GetDefaultValue(ClearWorksheetBeforeImportProperty))
				source.ClearWorksheetBeforeImport = ClearWorksheetBeforeImport;
			if (Culture != (CultureInfo)GetDefaultValue(CultureProperty))
				source.Culture = Culture;
			if (Delimiter != (char)GetDefaultValue(DelimiterProperty))
				source.Delimiter = Delimiter;
			if (MaxRowCountToImport != (int)GetDefaultValue(MaxRowCountToImportProperty))
				source.MaxRowCountToImport = MaxRowCountToImport;
			if (NewlineType != (NewlineType)GetDefaultValue(NewlineTypeProperty))
				source.NewlineType = NewlineType;
			if (StartCellToInsert != (string)GetDefaultValue(StartCellToInsertProperty))
				source.StartCellToInsert = StartCellToInsert;
			if (StartRowToImport != (int)GetDefaultValue(StartRowToImportProperty))
				source.StartRowToImport = StartRowToImport;
			if (TextQualifier != (char)GetDefaultValue(TextQualifierProperty))
				source.TextQualifier = TextQualifier;
			if (TrimBlanks != (bool)GetDefaultValue(TrimBlanksProperty))
				source.TrimBlanks = TrimBlanks;
			if (ValueTypeDetectMode != (CsvValueTypeDetectMode)GetDefaultValue(ValueTypeDetectModeProperty))
				source.ValueTypeDetectMode = ValueTypeDetectMode;
			if (WorksheetName != (string)GetDefaultValue(WorksheetNameProperty))
				source.Worksheet = WorksheetName;
		}
		object GetDefaultValue(DependencyProperty property) {
			return property.DefaultMetadata.DefaultValue;
		}
		void BindProperties() {
			BindAutoDetectEncodingProperty();
			BindCellIndexOutOfRangeStrategyProperty();
			BindCellReferenceStyleProperty();
			BindClearWorksheetBeforeImportProperty();
			BindCultureProperty();
			BindDelimiterProperty();
			BindMaxRowCountToImportProperty();
			BindNewlineTypeProperty();
			BindStartCellToInsertProperty();
			BindStartRowToImportProperty();
			BindTextQualifierProperty();
			BindTrimBlanksProperty();
			BindValueTypeDetectModeProperty();
			BindWorksheetNameProperty();
		}
		void BindAutoDetectEncodingProperty() {
			Binding bind = new Binding("AutoDetectEncoding") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, AutoDetectEncodingProperty, bind);
		}
		void BindCellIndexOutOfRangeStrategyProperty() {
			Binding bind = new Binding("CellIndexOutOfRangeStrategy") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, CellIndexOutOfRangeStrategyProperty, bind);
		}
		void BindCellReferenceStyleProperty() {
			Binding bind = new Binding("CellReferenceStyle") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, CellReferenceStyleProperty, bind);
		}
		void BindClearWorksheetBeforeImportProperty() {
			Binding bind = new Binding("ClearWorksheetBeforeImport") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, ClearWorksheetBeforeImportProperty, bind);
		}
		void BindCultureProperty() {
			Binding bind = new Binding("Culture") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, CultureProperty, bind);
		}
		void BindDelimiterProperty() {
			Binding bind = new Binding("Delimiter") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, DelimiterProperty, bind);
		}
		void BindMaxRowCountToImportProperty() {
			Binding bind = new Binding("MaxRowCountToImport") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, MaxRowCountToImportProperty, bind);
		}
		void BindNewlineTypeProperty() {
			Binding bind = new Binding("NewlineType") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, NewlineTypeProperty, bind);
		}
		void BindStartCellToInsertProperty() {
			Binding bind = new Binding("StartCellToInsert") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, StartCellToInsertProperty, bind);
		}
		void BindStartRowToImportProperty() {
			Binding bind = new Binding("StartRowToImport") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, StartRowToImportProperty, bind);
		}
		void BindTextQualifierProperty() {
			Binding bind = new Binding("TextQualifier") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, TextQualifierProperty, bind);
		}
		void BindTrimBlanksProperty() {
			Binding bind = new Binding("TrimBlanks") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, TrimBlanksProperty, bind);
		}
		void BindValueTypeDetectModeProperty() {
			Binding bind = new Binding("ValueTypeDetectMode") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, ValueTypeDetectModeProperty, bind);
		}
		void BindWorksheetNameProperty() {
			Binding bind = new Binding("Worksheet") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, WorksheetNameProperty, bind);
		}
		public void Reset() {
			if (source != null)
				source.Reset();
		}
	}
	#endregion
	#region SpreadsheetOpenXmlImportOptions
	public class SpreadsheetOpenXmlImportOptions : DependencyObject {
		#region Fields
		public static readonly DependencyProperty EncryptionPasswordProperty;
		public static readonly DependencyProperty OverrideCalculationModeProperty;
		OpenXmlDocumentImporterOptions source;
		#endregion
		static SpreadsheetOpenXmlImportOptions() {
			Type ownerType = typeof(SpreadsheetOpenXmlImportOptions);
			EncryptionPasswordProperty = DependencyProperty.Register("EncryptionPassword", typeof(string), ownerType,
				new FrameworkPropertyMetadata(String.Empty));
			OverrideCalculationModeProperty = DependencyProperty.Register("OverrideCalculationMode", typeof(CalculationModeOverride), ownerType,
				new FrameworkPropertyMetadata(CalculationModeOverride.None));
		}
		#region Properties
		public string EncryptionPassword {
			get { return (string)GetValue(EncryptionPasswordProperty); }
			set { SetValue(EncryptionPasswordProperty, value); }
		}
		public CalculationModeOverride OverrideCalculationMode {
			get { return (CalculationModeOverride)GetValue(OverrideCalculationModeProperty); }
			set { SetValue(OverrideCalculationModeProperty, value); }
		}
		#endregion
		internal void SetSource(OpenXmlDocumentImporterOptions source) {
			Guard.ArgumentNotNull(source, "source");
			this.source = source;
			UpdateSourceProperties();
			BindProperties();
		}
		void UpdateSourceProperties() {
			if (EncryptionPassword != (string)GetDefaultValue(EncryptionPasswordProperty))
				source.EncryptionPassword = EncryptionPassword;
			if (OverrideCalculationMode != (CalculationModeOverride)GetDefaultValue(OverrideCalculationModeProperty))
				source.OverrideCalculationMode = OverrideCalculationMode;
		}
		object GetDefaultValue(DependencyProperty property) {
			return property.DefaultMetadata.DefaultValue;
		}
		void BindProperties() {
			BindEncryptionPasswordProperty();
			BindOverrideCalculationModeProperty();
		}
		void BindEncryptionPasswordProperty() {
			Binding bind = new Binding("EncryptionPassword") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, EncryptionPasswordProperty, bind);
		}
		void BindOverrideCalculationModeProperty() {
			Binding bind = new Binding("OverrideCalculationMode") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, OverrideCalculationModeProperty, bind);
		}
		public void Reset() {
			if (source != null)
				source.Reset();
		}
	}
	#endregion
	#region SpreadsheetTxtImportOptions
	public class SpreadsheetTxtImportOptions : DependencyObject {
		#region Fields
		public static readonly DependencyProperty AutoDetectEncodingProperty;
		public static readonly DependencyProperty CellIndexOutOfRangeStrategyProperty;
		public static readonly DependencyProperty CellReferenceStyleProperty;
		public static readonly DependencyProperty ClearWorksheetBeforeImportProperty;
		public static readonly DependencyProperty CultureProperty;
		public static readonly DependencyProperty DelimiterProperty;
		public static readonly DependencyProperty MaxRowCountToImportProperty;
		public static readonly DependencyProperty NewlineTypeProperty;
		public static readonly DependencyProperty StartCellToInsertProperty;
		public static readonly DependencyProperty StartRowToImportProperty;
		public static readonly DependencyProperty TextQualifierProperty;
		public static readonly DependencyProperty TrimBlanksProperty;
		public static readonly DependencyProperty ValueTypeDetectModeProperty;
		public static readonly DependencyProperty WorksheetNameProperty;
		TxtDocumentImporterOptions source;
		#endregion
		static SpreadsheetTxtImportOptions() {
			Type ownerType = typeof(SpreadsheetTxtImportOptions);
			AutoDetectEncodingProperty = DependencyProperty.Register("AutoDetectEncoding", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(true));
			CellIndexOutOfRangeStrategyProperty = DependencyProperty.Register("CellIndexOutOfRangeStrategy", typeof(CsvImportCellIndexStrategy), ownerType,
				new FrameworkPropertyMetadata(TextDocumentImporterOptionsBase.DefaultCellIndexOutOfRangeStrategy));
			CellReferenceStyleProperty = DependencyProperty.Register("CellReferenceStyle", typeof(CellReferenceStyle), ownerType,
				new FrameworkPropertyMetadata(TextDocumentImporterOptionsBase.DefaultCellReferenceStyle));
			ClearWorksheetBeforeImportProperty = DependencyProperty.Register("ClearWorksheetBeforeImport", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(TextDocumentImporterOptionsBase.DefaultClearSheetBeforeImport));
			CultureProperty = DependencyProperty.Register("Culture", typeof(CultureInfo), ownerType,
				new FrameworkPropertyMetadata(CultureInfo.InvariantCulture));
			DelimiterProperty = DependencyProperty.Register("Delimiter", typeof(char), ownerType,
				new FrameworkPropertyMetadata('\0'));
			MaxRowCountToImportProperty = DependencyProperty.Register("MaxRowCountToImport", typeof(int), ownerType,
				new FrameworkPropertyMetadata(TextDocumentImporterOptionsBase.DefaultMaxRows));
			NewlineTypeProperty = DependencyProperty.Register("NewlineType", typeof(NewlineType), ownerType,
				new FrameworkPropertyMetadata(TextDocumentImporterOptionsBase.DefaultNewlineType));
			StartCellToInsertProperty = DependencyProperty.Register("StartCellToInsert", typeof(string), ownerType,
				new FrameworkPropertyMetadata(TextDocumentImporterOptionsBase.DefaultStartCellToInsert));
			StartRowToImportProperty = DependencyProperty.Register("StartRowToImport", typeof(int), ownerType,
				new FrameworkPropertyMetadata(TextDocumentImporterOptionsBase.DefaultSkipHeaderLines));
			TextQualifierProperty = DependencyProperty.Register("TextQualifier", typeof(char), ownerType,
				new FrameworkPropertyMetadata(TextDocumentImporterOptionsBase.DefaultTextQualifier));
			TrimBlanksProperty = DependencyProperty.Register("TrimBlanks", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(TextDocumentImporterOptionsBase.DefaultTrimBlanks));
			ValueTypeDetectModeProperty = DependencyProperty.Register("ValueTypeDetectMode", typeof(CsvValueTypeDetectMode), ownerType,
				new FrameworkPropertyMetadata(TextDocumentImporterOptionsBase.DefaultValueTypeDetectMode));
			WorksheetNameProperty = DependencyProperty.Register("WorksheetName", typeof(string), ownerType,
				new FrameworkPropertyMetadata(String.Empty));
		}
		#region Properties
		public bool AutoDetectEncoding {
			get { return (bool)GetValue(AutoDetectEncodingProperty); }
			set { SetValue(AutoDetectEncodingProperty, value); }
		}
		public CsvImportCellIndexStrategy CellIndexOutOfRangeStrategy {
			get { return (CsvImportCellIndexStrategy)GetValue(CellIndexOutOfRangeStrategyProperty); }
			set { SetValue(CellIndexOutOfRangeStrategyProperty, value); }
		}
		public CellReferenceStyle CellReferenceStyle {
			get { return (CellReferenceStyle)GetValue(CellReferenceStyleProperty); }
			set { SetValue(CellReferenceStyleProperty, value); }
		}
		public bool ClearWorksheetBeforeImport {
			get { return (bool)GetValue(ClearWorksheetBeforeImportProperty); }
			set { SetValue(ClearWorksheetBeforeImportProperty, value); }
		}
		public CultureInfo Culture {
			get { return (CultureInfo)GetValue(CultureProperty); }
			set { SetValue(CultureProperty, value); }
		}
		public char Delimiter {
			get { return (char)GetValue(DelimiterProperty); }
			set { SetValue(DelimiterProperty, value); }
		}
		public int MaxRowCountToImport {
			get { return (int)GetValue(MaxRowCountToImportProperty); }
			set { SetValue(MaxRowCountToImportProperty, value); }
		}
		public NewlineType NewlineType {
			get { return (NewlineType)GetValue(NewlineTypeProperty); }
			set { SetValue(NewlineTypeProperty, value); }
		}
		public string StartCellToInsert {
			get { return (string)GetValue(StartCellToInsertProperty); }
			set { SetValue(StartCellToInsertProperty, value); }
		}
		public int StartRowToImport {
			get { return (int)GetValue(StartRowToImportProperty); }
			set { SetValue(StartRowToImportProperty, value); }
		}
		public char TextQualifier {
			get { return (char)GetValue(TextQualifierProperty); }
			set { SetValue(TextQualifierProperty, value); }
		}
		public bool TrimBlanks {
			get { return (bool)GetValue(TrimBlanksProperty); }
			set { SetValue(TrimBlanksProperty, value); }
		}
		public CsvValueTypeDetectMode ValueTypeDetectMode {
			get { return (CsvValueTypeDetectMode)GetValue(ValueTypeDetectModeProperty); }
			set { SetValue(ValueTypeDetectModeProperty, value); }
		}
		public string WorksheetName {
			get { return (string)GetValue(WorksheetNameProperty); }
			set { SetValue(WorksheetNameProperty, value); }
		}
		#endregion
		internal void SetSource(TxtDocumentImporterOptions source) {
			Guard.ArgumentNotNull(source, "source");
			this.source = source;
			UpdateSourceProperties();
			BindProperties();
		}
		void UpdateSourceProperties() {
			if (AutoDetectEncoding != (bool)GetDefaultValue(AutoDetectEncodingProperty))
				source.AutoDetectEncoding = AutoDetectEncoding;
			if (CellIndexOutOfRangeStrategy != (CsvImportCellIndexStrategy)GetDefaultValue(CellIndexOutOfRangeStrategyProperty))
				source.CellIndexOutOfRangeStrategy = CellIndexOutOfRangeStrategy;
			if (CellReferenceStyle != (CellReferenceStyle)GetDefaultValue(CellReferenceStyleProperty))
				source.CellReferenceStyle = CellReferenceStyle;
			if (ClearWorksheetBeforeImport != (bool)GetDefaultValue(ClearWorksheetBeforeImportProperty))
				source.ClearWorksheetBeforeImport = ClearWorksheetBeforeImport;
			if (Culture != (CultureInfo)GetDefaultValue(CultureProperty))
				source.Culture = Culture;
			if (Delimiter != (char)GetDefaultValue(DelimiterProperty))
				source.Delimiter = Delimiter;
			if (MaxRowCountToImport != (int)GetDefaultValue(MaxRowCountToImportProperty))
				source.MaxRowCountToImport = MaxRowCountToImport;
			if (NewlineType != (NewlineType)GetDefaultValue(NewlineTypeProperty))
				source.NewlineType = NewlineType;
			if (StartCellToInsert != (string)GetDefaultValue(StartCellToInsertProperty))
				source.StartCellToInsert = StartCellToInsert;
			if (StartRowToImport != (int)GetDefaultValue(StartRowToImportProperty))
				source.StartRowToImport = StartRowToImport;
			if (TextQualifier != (char)GetDefaultValue(TextQualifierProperty))
				source.TextQualifier = TextQualifier;
			if (TrimBlanks != (bool)GetDefaultValue(TrimBlanksProperty))
				source.TrimBlanks = TrimBlanks;
			if (ValueTypeDetectMode != (CsvValueTypeDetectMode)GetDefaultValue(ValueTypeDetectModeProperty))
				source.ValueTypeDetectMode = ValueTypeDetectMode;
			if (WorksheetName != (string)GetDefaultValue(WorksheetNameProperty))
				source.Worksheet = WorksheetName;
		}
		object GetDefaultValue(DependencyProperty property) {
			return property.DefaultMetadata.DefaultValue;
		}
		void BindProperties() {
			BindAutoDetectEncodingProperty();
			BindCellIndexOutOfRangeStrategyProperty();
			BindCellReferenceStyleProperty();
			BindClearWorksheetBeforeImportProperty();
			BindCultureProperty();
			BindDelimiterProperty();
			BindMaxRowCountToImportProperty();
			BindNewlineTypeProperty();
			BindStartCellToInsertProperty();
			BindStartRowToImportProperty();
			BindTextQualifierProperty();
			BindTrimBlanksProperty();
			BindValueTypeDetectModeProperty();
			BindWorksheetNameProperty();
		}
		void BindAutoDetectEncodingProperty() {
			Binding bind = new Binding("AutoDetectEncoding") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, AutoDetectEncodingProperty, bind);
		}
		void BindCellIndexOutOfRangeStrategyProperty() {
			Binding bind = new Binding("CellIndexOutOfRangeStrategy") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, CellIndexOutOfRangeStrategyProperty, bind);
		}
		void BindCellReferenceStyleProperty() {
			Binding bind = new Binding("CellReferenceStyle") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, CellReferenceStyleProperty, bind);
		}
		void BindClearWorksheetBeforeImportProperty() {
			Binding bind = new Binding("ClearWorksheetBeforeImport") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, ClearWorksheetBeforeImportProperty, bind);
		}
		void BindCultureProperty() {
			Binding bind = new Binding("Culture") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, CultureProperty, bind);
		}
		void BindDelimiterProperty() {
			Binding bind = new Binding("Delimiter") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, DelimiterProperty, bind);
		}
		void BindMaxRowCountToImportProperty() {
			Binding bind = new Binding("MaxRowCountToImport") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, MaxRowCountToImportProperty, bind);
		}
		void BindNewlineTypeProperty() {
			Binding bind = new Binding("NewlineType") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, NewlineTypeProperty, bind);
		}
		void BindStartCellToInsertProperty() {
			Binding bind = new Binding("StartCellToInsert") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, StartCellToInsertProperty, bind);
		}
		void BindStartRowToImportProperty() {
			Binding bind = new Binding("StartRowToImport") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, StartRowToImportProperty, bind);
		}
		void BindTextQualifierProperty() {
			Binding bind = new Binding("TextQualifier") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, TextQualifierProperty, bind);
		}
		void BindTrimBlanksProperty() {
			Binding bind = new Binding("TrimBlanks") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, TrimBlanksProperty, bind);
		}
		void BindValueTypeDetectModeProperty() {
			Binding bind = new Binding("ValueTypeDetectMode") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, ValueTypeDetectModeProperty, bind);
		}
		void BindWorksheetNameProperty() {
			Binding bind = new Binding("Worksheet") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, WorksheetNameProperty, bind);
		}
		public void Reset() {
			if (source != null)
				source.Reset();
		}
	}
	#endregion
	#region SpreadsheetXlsImportOptions
	public class SpreadsheetXlsImportOptions : DependencyObject {
		#region Fields
		public static readonly DependencyProperty PasswordProperty;
		public static readonly DependencyProperty ValidateFormulaProperty;
		public static readonly DependencyProperty OverrideCalculationModeProperty;
		XlsDocumentImporterOptions source;
		#endregion
		static SpreadsheetXlsImportOptions() {
			Type ownerType = typeof(SpreadsheetXlsImportOptions);
			PasswordProperty = DependencyProperty.Register("Password", typeof(string), ownerType,
				new FrameworkPropertyMetadata(String.Empty));
			ValidateFormulaProperty = DependencyProperty.Register("ValidateFormula", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false));
			OverrideCalculationModeProperty = DependencyProperty.Register("OverrideCalculationMode", typeof(CalculationModeOverride), ownerType,
				new FrameworkPropertyMetadata(CalculationModeOverride.None));
		}
		#region Properties
		public string Password {
			get { return (string)GetValue(PasswordProperty); }
			set { SetValue(PasswordProperty, value); }
		}
		public bool ValidateFormula {
			get { return (bool)GetValue(ValidateFormulaProperty); }
			set { SetValue(ValidateFormulaProperty, value); }
		}
		public CalculationModeOverride OverrideCalculationMode {
			get { return (CalculationModeOverride)GetValue(OverrideCalculationModeProperty); }
			set { SetValue(OverrideCalculationModeProperty, value); }
		}
		#endregion
		internal void SetSource(XlsDocumentImporterOptions source) {
			Guard.ArgumentNotNull(source, "source");
			this.source = source;
			UpdateSourceProperties();
			BindProperties();
		}
		void UpdateSourceProperties() {
			if (Password != (string)GetDefaultValue(PasswordProperty))
				source.Password = Password;
			if (ValidateFormula != (bool)GetDefaultValue(ValidateFormulaProperty))
				source.ValidateFormula = ValidateFormula;
			if (OverrideCalculationMode != (CalculationModeOverride)GetDefaultValue(OverrideCalculationModeProperty))
				source.OverrideCalculationMode = OverrideCalculationMode;
		}
		object GetDefaultValue(DependencyProperty property) {
			return property.DefaultMetadata.DefaultValue;
		}
		void BindProperties() {
			BindPasswordProperty();
			BindValidateFormulaProperty();
			BindOverrideCalculationModeProperty();
		}
		void BindPasswordProperty() {
			Binding bind = new Binding("Password") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, PasswordProperty, bind);
		}
		void BindValidateFormulaProperty() {
			Binding bind = new Binding("ValidateFormula") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, ValidateFormulaProperty, bind);
		}
		void BindOverrideCalculationModeProperty() {
			Binding bind = new Binding("OverrideCalculationMode") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, OverrideCalculationModeProperty, bind);
		}
		public void Reset() {
			if (source != null)
				source.Reset();
		}
	}
	#endregion
	#region SpreadsheetXlsmImportOptions
	public class SpreadsheetXlsmImportOptions : DependencyObject {
		#region Fields
		public static readonly DependencyProperty EncryptionPasswordProperty;
		public static readonly DependencyProperty OverrideCalculationModeProperty;
		XlsmDocumentImporterOptions source;
		#endregion
		static SpreadsheetXlsmImportOptions() {
			Type ownerType = typeof(SpreadsheetXlsmImportOptions);
			EncryptionPasswordProperty = DependencyProperty.Register("EncryptionPassword", typeof(string), ownerType,
				new FrameworkPropertyMetadata(String.Empty));
			OverrideCalculationModeProperty = DependencyProperty.Register("OverrideCalculationMode", typeof(CalculationModeOverride), ownerType,
				new FrameworkPropertyMetadata(CalculationModeOverride.None));
		}
		#region Properties
		public string EncryptionPassword {
			get { return (string)GetValue(EncryptionPasswordProperty); }
			set { SetValue(EncryptionPasswordProperty, value); }
		}
		public CalculationModeOverride OverrideCalculationMode {
			get { return (CalculationModeOverride)GetValue(OverrideCalculationModeProperty); }
			set { SetValue(OverrideCalculationModeProperty, value); }
		}
		#endregion
		internal void SetSource(XlsmDocumentImporterOptions source) {
			Guard.ArgumentNotNull(source, "source");
			this.source = source;
			UpdateSourceProperties();
			BindProperties();
		}
		void UpdateSourceProperties() {
			if (EncryptionPassword != (string)GetDefaultValue(EncryptionPasswordProperty))
				source.EncryptionPassword = EncryptionPassword;
			if (OverrideCalculationMode != (CalculationModeOverride)GetDefaultValue(OverrideCalculationModeProperty))
				source.OverrideCalculationMode = OverrideCalculationMode;
		}
		object GetDefaultValue(DependencyProperty property) {
			return property.DefaultMetadata.DefaultValue;
		}
		void BindProperties() {
			BindEncryptionPasswordProperty();
			BindOverrideCalculationModeProperty();
		}
		void BindEncryptionPasswordProperty() {
			Binding bind = new Binding("EncryptionPassword") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, EncryptionPasswordProperty, bind);
		}
		void BindOverrideCalculationModeProperty() {
			Binding bind = new Binding("OverrideCalculationMode") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, OverrideCalculationModeProperty, bind);
		}
		public void Reset() {
			if (source != null)
				source.Reset();
		}
	}
	#endregion
}
