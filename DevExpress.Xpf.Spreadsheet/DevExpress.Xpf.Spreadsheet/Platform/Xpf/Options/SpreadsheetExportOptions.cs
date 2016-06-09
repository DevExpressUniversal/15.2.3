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

using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Export;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Import;
using System;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;
namespace DevExpress.Xpf.Spreadsheet {
	#region SpreadsheetExportOptions
	public class SpreadsheetExportOptions : DependencyObject {
		#region Fields
		public static readonly DependencyProperty CsvProperty;
		public static readonly DependencyProperty TxtProperty;
		public static readonly DependencyProperty CustomFunctionExportModeProperty;
		WorkbookExportOptions source;
		#endregion
		static SpreadsheetExportOptions() {
			Type ownerType = typeof(SpreadsheetExportOptions);
			CsvProperty = DependencyProperty.Register("Csv", typeof(SpreadsheetCsvExportOptions), ownerType,
				new FrameworkPropertyMetadata((d, e) => ((SpreadsheetExportOptions)d).OnCsvChanged((SpreadsheetCsvExportOptions)e.OldValue, (SpreadsheetCsvExportOptions)e.NewValue)));
			TxtProperty = DependencyProperty.Register("Txt", typeof(SpreadsheetTxtExportOptions), ownerType,
				new FrameworkPropertyMetadata((d, e) => ((SpreadsheetExportOptions)d).OnTxtChanged((SpreadsheetTxtExportOptions)e.OldValue, (SpreadsheetTxtExportOptions)e.NewValue)));
			CustomFunctionExportModeProperty = DependencyProperty.Register("CustomFunctionExportMode", typeof(CustomFunctionExportMode), ownerType,
				new FrameworkPropertyMetadata(CustomFunctionExportMode.Function));
		}
		public SpreadsheetExportOptions() {
			Csv = new SpreadsheetCsvExportOptions();
			Txt = new SpreadsheetTxtExportOptions();
		}
		#region Properties
		public SpreadsheetCsvExportOptions Csv {
			get { return (SpreadsheetCsvExportOptions)GetValue(CsvProperty); }
			set { SetValue(CsvProperty, value); }
		}
		public SpreadsheetTxtExportOptions Txt {
			get { return (SpreadsheetTxtExportOptions)GetValue(TxtProperty); }
			set { SetValue(TxtProperty, value); }
		}
		public CustomFunctionExportMode CustomFunctionExportMode {
			get { return (CustomFunctionExportMode)GetValue(CustomFunctionExportModeProperty); }
			set { SetValue(CustomFunctionExportModeProperty, value); }
		}
		#endregion
		void OnCsvChanged(SpreadsheetCsvExportOptions oldValue, SpreadsheetCsvExportOptions newValue) {
			if (oldValue != null)
				oldValue.Reset();
			if (newValue != null && source != null)
				newValue.SetSource(source.Csv);
		}
		void OnTxtChanged(SpreadsheetTxtExportOptions oldValue, SpreadsheetTxtExportOptions newValue) {
			if (oldValue != null)
				oldValue.Reset();
			if (newValue != null && source != null)
				newValue.SetSource(source.Txt);
		}
		internal void SetSource(WorkbookExportOptions source) {
			Guard.ArgumentNotNull(source, "source");
			this.source = source;
			UpdateSourceProperties();
			BindProperties();
			Csv.SetSource(source.Csv);
			Txt.SetSource(source.Txt);
		}
		void UpdateSourceProperties() {
			if (CustomFunctionExportMode != (CustomFunctionExportMode)GetDefaultValue(CustomFunctionExportModeProperty))
				source.CustomFunctionExportMode = CustomFunctionExportMode;
		}
		object GetDefaultValue(DependencyProperty property) {
			return property.DefaultMetadata.DefaultValue;
		}
		void BindProperties() {
			BindCustomFunctionExportModeProperty();
		}
		void BindCustomFunctionExportModeProperty() {
			Binding bind = new Binding("CustomFunctionExportMode") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, CustomFunctionExportModeProperty, bind);
		}
		public void Reset() {
			if (source != null)
				source.Reset();
		}
	}
	#endregion
	#region SpreadsheetCsvExportOptions
	public class SpreadsheetCsvExportOptions : DependencyObject {
		#region Fields
		public static readonly DependencyProperty CellReferenceStyleProperty;
		public static readonly DependencyProperty CultureProperty;
		public static readonly DependencyProperty DiscardTrailingEmptyCellsProperty;
		public static readonly DependencyProperty FormulaExportModeProperty;
		public static readonly DependencyProperty NewlineAfterLastRowProperty;
		public static readonly DependencyProperty NewlineTypeProperty;
		public static readonly DependencyProperty RangeProperty;
		public static readonly DependencyProperty ShiftTopLeftProperty;
		public static readonly DependencyProperty TextQualifierProperty;
		public static readonly DependencyProperty UseCellNumberFormatProperty;
		public static readonly DependencyProperty ValueSeparatorProperty;
		public static readonly DependencyProperty WorksheetNameProperty;
		public static readonly DependencyProperty WritePreambleProperty;
		public static readonly DependencyProperty EncodingProperty;
		CsvDocumentExporterOptions source;
		#endregion
		static SpreadsheetCsvExportOptions() {
			Type ownerType = typeof(SpreadsheetCsvExportOptions);
			CellReferenceStyleProperty = DependencyProperty.Register("CellReferenceStyle", typeof(CellReferenceStyle), ownerType,
				new FrameworkPropertyMetadata(CsvDocumentExporterOptions.DefaultReferenceStyle));
			CultureProperty = DependencyProperty.Register("Culture", typeof(CultureInfo), ownerType,
				new FrameworkPropertyMetadata(CultureInfo.InvariantCulture));
			DiscardTrailingEmptyCellsProperty = DependencyProperty.Register("DiscardTrailingEmptyCells", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(CsvDocumentExporterOptions.DefaultDiscardTrailingEmptyCells));
			FormulaExportModeProperty = DependencyProperty.Register("FormulaExportMode", typeof(FormulaExportMode), ownerType,
				new FrameworkPropertyMetadata(CsvDocumentExporterOptions.DefaultFormulaExportMode));
			NewlineAfterLastRowProperty = DependencyProperty.Register("NewlineAfterLastRow", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(CsvDocumentExporterOptions.DefaultNewlineAfterLastRow));
			NewlineTypeProperty = DependencyProperty.Register("NewlineType", typeof(NewlineType), ownerType,
				new FrameworkPropertyMetadata(CsvDocumentExporterOptions.DefaultNewlineType));
			RangeProperty = DependencyProperty.Register("Range", typeof(string), ownerType,
				new FrameworkPropertyMetadata(String.Empty));
			ShiftTopLeftProperty = DependencyProperty.Register("ShiftTopLeft", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(CsvDocumentExporterOptions.DefaultShiftTopLeft));
			TextQualifierProperty = DependencyProperty.Register("TextQualifier", typeof(char), ownerType,
				new FrameworkPropertyMetadata(CsvDocumentExporterOptions.DefaultTextQualifier));
			UseCellNumberFormatProperty = DependencyProperty.Register("UseCellNumberFormat", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(CsvDocumentExporterOptions.DefaultUseCellNumberFormat));
			ValueSeparatorProperty = DependencyProperty.Register("ValueSeparator", typeof(char), ownerType,
				new FrameworkPropertyMetadata(TextDocumentExporterOptionsBase.DefaultValueSeparator));
			WorksheetNameProperty = DependencyProperty.Register("WorksheetName", typeof(string), ownerType,
				new FrameworkPropertyMetadata(String.Empty));
			WritePreambleProperty = DependencyProperty.Register("WritePreamble", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(CsvDocumentExporterOptions.DefaultWritePreamble));
			EncodingProperty = DependencyProperty.Register("Encoding", typeof(Encoding), ownerType,
				new FrameworkPropertyMetadata(Encoding.UTF8));
		}
		#region Properties
		public CellReferenceStyle CellReferenceStyle {
			get { return (CellReferenceStyle)GetValue(CellReferenceStyleProperty); }
			set { SetValue(CellReferenceStyleProperty, value); }
		}
		public CultureInfo Culture {
			get { return (CultureInfo)GetValue(CultureProperty); }
			set { SetValue(CultureProperty, value); }
		}
		public bool DiscardTrailingEmptyCells {
			get { return (bool)GetValue(DiscardTrailingEmptyCellsProperty); }
			set { SetValue(DiscardTrailingEmptyCellsProperty, value); }
		}
		public FormulaExportMode FormulaExportMode {
			get { return (FormulaExportMode)GetValue(FormulaExportModeProperty); }
			set { SetValue(FormulaExportModeProperty, value); }
		}
		public bool NewlineAfterLastRow {
			get { return (bool)GetValue(NewlineAfterLastRowProperty); }
			set { SetValue(NewlineAfterLastRowProperty, value); }
		}
		public NewlineType NewlineType {
			get { return (NewlineType)GetValue(NewlineTypeProperty); }
			set { SetValue(NewlineTypeProperty, value); }
		}
		public string Range {
			get { return (string)GetValue(RangeProperty); }
			set { SetValue(RangeProperty, value); }
		}
		public bool ShiftTopLeft {
			get { return (bool)GetValue(ShiftTopLeftProperty); }
			set { SetValue(ShiftTopLeftProperty, value); }
		}
		public char TextQualifier {
			get { return (char)GetValue(TextQualifierProperty); }
			set { SetValue(TextQualifierProperty, value); }
		}
		public bool UseCellNumberFormat {
			get { return (bool)GetValue(UseCellNumberFormatProperty); }
			set { SetValue(UseCellNumberFormatProperty, value); }
		}
		public char ValueSeparator {
			get { return (char)GetValue(ValueSeparatorProperty); }
			set { SetValue(ValueSeparatorProperty, value); }
		}
		public string WorksheetName {
			get { return (string)GetValue(WorksheetNameProperty); }
			set { SetValue(WorksheetNameProperty, value); }
		}
		public bool WritePreamble {
			get { return (bool)GetValue(WritePreambleProperty); }
			set { SetValue(WritePreambleProperty, value); }
		}
		public Encoding Encoding {
			get { return (Encoding)GetValue(EncodingProperty); }
			set { SetValue(EncodingProperty, value); }
		}
		#endregion
		internal void SetSource(CsvDocumentExporterOptions source) {
			Guard.ArgumentNotNull(source, "source");
			this.source = source;
			UpdateSourceProperties();
			BindProperties();
		}
		void UpdateSourceProperties() {
			if (CellReferenceStyle != (CellReferenceStyle)GetDefaultValue(CellReferenceStyleProperty))
				source.CellReferenceStyle = CellReferenceStyle;
			if (Culture != (CultureInfo)GetDefaultValue(CultureProperty))
				source.Culture = Culture;
			if (DiscardTrailingEmptyCells != (bool)GetDefaultValue(DiscardTrailingEmptyCellsProperty))
				source.DiscardTrailingEmptyCells = DiscardTrailingEmptyCells;
			if (FormulaExportMode != (FormulaExportMode)GetDefaultValue(FormulaExportModeProperty))
				source.FormulaExportMode = FormulaExportMode;
			if (NewlineAfterLastRow != (bool)GetDefaultValue(NewlineAfterLastRowProperty))
				source.NewlineAfterLastRow = NewlineAfterLastRow;
			if (NewlineType != (NewlineType)GetDefaultValue(NewlineTypeProperty))
				source.NewlineType = NewlineType;
			if (Range != (string)GetDefaultValue(RangeProperty))
				source.Range = Range;
			if (ShiftTopLeft != (bool)GetDefaultValue(ShiftTopLeftProperty))
				source.ShiftTopLeft = ShiftTopLeft;
			if (TextQualifier != (char)GetDefaultValue(TextQualifierProperty))
				source.TextQualifier = TextQualifier;
			if (UseCellNumberFormat != (bool)GetDefaultValue(UseCellNumberFormatProperty))
				source.UseCellNumberFormat = UseCellNumberFormat;
			if (ValueSeparator != (char)GetDefaultValue(ValueSeparatorProperty))
				source.ValueSeparator = ValueSeparator;
			if (WorksheetName != (string)GetDefaultValue(WorksheetNameProperty))
				source.Worksheet = WorksheetName;
			if (WritePreamble != (bool)GetDefaultValue(WritePreambleProperty))
				source.WritePreamble = WritePreamble;
			if (Encoding != (Encoding)GetDefaultValue(EncodingProperty))
				source.Encoding = Encoding;
		}
		object GetDefaultValue(DependencyProperty property) {
			return property.DefaultMetadata.DefaultValue;
		}
		void BindProperties() {
			BindCellReferenceStyleProperty();
			BindCultureProperty();
			BindDiscardTrailingEmptyCellsProperty();
			BindFormulaExportModeProperty();
			BindNewlineAfterLastRowProperty();
			BindNewlineTypeProperty();
			BindRangeProperty();
			BindShiftTopLeftProperty();
			BindTextQualifierProperty();
			BindUseCellNumberFormatProperty();
			BindValueSeparatorProperty();
			BindWorksheetNameProperty();
			BindWritePreambleProperty();
			BindEncodingProperty();
		}
		void BindCellReferenceStyleProperty() {
			Binding bind = new Binding("CellReferenceStyle") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, CellReferenceStyleProperty, bind);
		}
		void BindCultureProperty() {
			Binding bind = new Binding("Culture") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, CultureProperty, bind);
		}
		void BindDiscardTrailingEmptyCellsProperty() {
			Binding bind = new Binding("DiscardTrailingEmptyCells") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, DiscardTrailingEmptyCellsProperty, bind);
		}
		void BindFormulaExportModeProperty() {
			Binding bind = new Binding("FormulaExportMode") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, FormulaExportModeProperty, bind);
		}
		void BindNewlineAfterLastRowProperty() {
			Binding bind = new Binding("NewlineAfterLastRow") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, NewlineAfterLastRowProperty, bind);
		}
		void BindNewlineTypeProperty() {
			Binding bind = new Binding("NewlineType") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, NewlineTypeProperty, bind);
		}
		void BindRangeProperty() {
			Binding bind = new Binding("Range") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, RangeProperty, bind);
		}
		void BindShiftTopLeftProperty() {
			Binding bind = new Binding("ShiftTopLeft") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, ShiftTopLeftProperty, bind);
		}
		void BindTextQualifierProperty() {
			Binding bind = new Binding("TextQualifier") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, TextQualifierProperty, bind);
		}
		void BindUseCellNumberFormatProperty() {
			Binding bind = new Binding("UseCellNumberFormat") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, UseCellNumberFormatProperty, bind);
		}
		void BindValueSeparatorProperty() {
			Binding bind = new Binding("ValueSeparator") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, ValueSeparatorProperty, bind);
		}
		void BindWorksheetNameProperty() {
			Binding bind = new Binding("Worksheet") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, WorksheetNameProperty, bind);
		}
		void BindWritePreambleProperty() {
			Binding bind = new Binding("WritePreamble") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, WritePreambleProperty, bind);
		}
		void BindEncodingProperty() {
			Binding bind = new Binding("Encoding") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, EncodingProperty, bind);
		}
		public void Reset() {
			if (source != null)
				source.Reset();
		}
	}
	#endregion
	#region SpreadsheetTxtExportOptions
	public class SpreadsheetTxtExportOptions : DependencyObject {
		#region Fields
		public static readonly DependencyProperty CellReferenceStyleProperty;
		public static readonly DependencyProperty CultureProperty;
		public static readonly DependencyProperty DiscardTrailingEmptyCellsProperty;
		public static readonly DependencyProperty FormulaExportModeProperty;
		public static readonly DependencyProperty NewlineAfterLastRowProperty;
		public static readonly DependencyProperty NewlineTypeProperty;
		public static readonly DependencyProperty RangeProperty;
		public static readonly DependencyProperty ShiftTopLeftProperty;
		public static readonly DependencyProperty TextQualifierProperty;
		public static readonly DependencyProperty UseCellNumberFormatProperty;
		public static readonly DependencyProperty ValueSeparatorProperty;
		public static readonly DependencyProperty WorksheetNameProperty;
		public static readonly DependencyProperty WritePreambleProperty;
		public static readonly DependencyProperty EncodingProperty;
		TxtDocumentExporterOptions source;
		#endregion
		static SpreadsheetTxtExportOptions() {
			Type ownerType = typeof(SpreadsheetTxtExportOptions);
			CellReferenceStyleProperty = DependencyProperty.Register("CellReferenceStyle", typeof(CellReferenceStyle), ownerType,
				new FrameworkPropertyMetadata(TxtDocumentExporterOptions.DefaultReferenceStyle));
			CultureProperty = DependencyProperty.Register("Culture", typeof(CultureInfo), ownerType,
				new FrameworkPropertyMetadata(CultureInfo.InvariantCulture));
			DiscardTrailingEmptyCellsProperty = DependencyProperty.Register("DiscardTrailingEmptyCells", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(TxtDocumentExporterOptions.DefaultDiscardTrailingEmptyCells));
			FormulaExportModeProperty = DependencyProperty.Register("FormulaExportMode", typeof(FormulaExportMode), ownerType,
				new FrameworkPropertyMetadata(TxtDocumentExporterOptions.DefaultFormulaExportMode));
			NewlineAfterLastRowProperty = DependencyProperty.Register("NewlineAfterLastRow", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(TxtDocumentExporterOptions.DefaultNewlineAfterLastRow));
			NewlineTypeProperty = DependencyProperty.Register("NewlineType", typeof(NewlineType), ownerType,
				new FrameworkPropertyMetadata(TxtDocumentExporterOptions.DefaultNewlineType));
			RangeProperty = DependencyProperty.Register("Range", typeof(string), ownerType,
				new FrameworkPropertyMetadata(String.Empty));
			ShiftTopLeftProperty = DependencyProperty.Register("ShiftTopLeft", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(TxtDocumentExporterOptions.DefaultShiftTopLeft));
			TextQualifierProperty = DependencyProperty.Register("TextQualifier", typeof(char), ownerType,
				new FrameworkPropertyMetadata(TxtDocumentExporterOptions.DefaultTextQualifier));
			UseCellNumberFormatProperty = DependencyProperty.Register("UseCellNumberFormat", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(TxtDocumentExporterOptions.DefaultUseCellNumberFormat));
			ValueSeparatorProperty = DependencyProperty.Register("ValueSeparator", typeof(char), ownerType,
				new FrameworkPropertyMetadata(TxtDocumentExporterOptions.TextDefaultValueSeparator));
			WorksheetNameProperty = DependencyProperty.Register("WorksheetName", typeof(string), ownerType,
				new FrameworkPropertyMetadata(String.Empty));
			WritePreambleProperty = DependencyProperty.Register("WritePreamble", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(TxtDocumentExporterOptions.DefaultWritePreamble));
			EncodingProperty = DependencyProperty.Register("Encoding", typeof(Encoding), ownerType,
				new FrameworkPropertyMetadata(Encoding.UTF8));
		}
		#region Properties
		public CellReferenceStyle CellReferenceStyle {
			get { return (CellReferenceStyle)GetValue(CellReferenceStyleProperty); }
			set { SetValue(CellReferenceStyleProperty, value); }
		}
		public CultureInfo Culture {
			get { return (CultureInfo)GetValue(CultureProperty); }
			set { SetValue(CultureProperty, value); }
		}
		public bool DiscardTrailingEmptyCells {
			get { return (bool)GetValue(DiscardTrailingEmptyCellsProperty); }
			set { SetValue(DiscardTrailingEmptyCellsProperty, value); }
		}
		public FormulaExportMode FormulaExportMode {
			get { return (FormulaExportMode)GetValue(FormulaExportModeProperty); }
			set { SetValue(FormulaExportModeProperty, value); }
		}
		public bool NewlineAfterLastRow {
			get { return (bool)GetValue(NewlineAfterLastRowProperty); }
			set { SetValue(NewlineAfterLastRowProperty, value); }
		}
		public NewlineType NewlineType {
			get { return (NewlineType)GetValue(NewlineTypeProperty); }
			set { SetValue(NewlineTypeProperty, value); }
		}
		public string Range {
			get { return (string)GetValue(RangeProperty); }
			set { SetValue(RangeProperty, value); }
		}
		public bool ShiftTopLeft {
			get { return (bool)GetValue(ShiftTopLeftProperty); }
			set { SetValue(ShiftTopLeftProperty, value); }
		}
		public char TextQualifier {
			get { return (char)GetValue(TextQualifierProperty); }
			set { SetValue(TextQualifierProperty, value); }
		}
		public bool UseCellNumberFormat {
			get { return (bool)GetValue(UseCellNumberFormatProperty); }
			set { SetValue(UseCellNumberFormatProperty, value); }
		}
		public char ValueSeparator {
			get { return (char)GetValue(ValueSeparatorProperty); }
			set { SetValue(ValueSeparatorProperty, value); }
		}
		public string WorksheetName {
			get { return (string)GetValue(WorksheetNameProperty); }
			set { SetValue(WorksheetNameProperty, value); }
		}
		public bool WritePreamble {
			get { return (bool)GetValue(WritePreambleProperty); }
			set { SetValue(WritePreambleProperty, value); }
		}
		public Encoding Encoding {
			get { return (Encoding)GetValue(EncodingProperty); }
			set { SetValue(EncodingProperty, value); }
		}
		#endregion
		internal void SetSource(TxtDocumentExporterOptions source) {
			Guard.ArgumentNotNull(source, "source");
			this.source = source;
			UpdateSourceProperties();
			BindProperties();
		}
		void UpdateSourceProperties() {
			if (CellReferenceStyle != (CellReferenceStyle)GetDefaultValue(CellReferenceStyleProperty))
				source.CellReferenceStyle = CellReferenceStyle;
			if (Culture != (CultureInfo)GetDefaultValue(CultureProperty))
				source.Culture = Culture;
			if (DiscardTrailingEmptyCells != (bool)GetDefaultValue(DiscardTrailingEmptyCellsProperty))
				source.DiscardTrailingEmptyCells = DiscardTrailingEmptyCells;
			if (FormulaExportMode != (FormulaExportMode)GetDefaultValue(FormulaExportModeProperty))
				source.FormulaExportMode = FormulaExportMode;
			if (NewlineAfterLastRow != (bool)GetDefaultValue(NewlineAfterLastRowProperty))
				source.NewlineAfterLastRow = NewlineAfterLastRow;
			if (NewlineType != (NewlineType)GetDefaultValue(NewlineTypeProperty))
				source.NewlineType = NewlineType;
			if (Range != (string)GetDefaultValue(RangeProperty))
				source.Range = Range;
			if (ShiftTopLeft != (bool)GetDefaultValue(ShiftTopLeftProperty))
				source.ShiftTopLeft = ShiftTopLeft;
			if (TextQualifier != (char)GetDefaultValue(TextQualifierProperty))
				source.TextQualifier = TextQualifier;
			if (UseCellNumberFormat != (bool)GetDefaultValue(UseCellNumberFormatProperty))
				source.UseCellNumberFormat = UseCellNumberFormat;
			if (ValueSeparator != (char)GetDefaultValue(ValueSeparatorProperty))
				source.ValueSeparator = ValueSeparator;
			if (WorksheetName != (string)GetDefaultValue(WorksheetNameProperty))
				source.Worksheet = WorksheetName;
			if (WritePreamble != (bool)GetDefaultValue(WritePreambleProperty))
				source.WritePreamble = WritePreamble;
			if (Encoding != (Encoding)GetDefaultValue(EncodingProperty))
				source.Encoding = Encoding;
		}
		object GetDefaultValue(DependencyProperty property) {
			return property.DefaultMetadata.DefaultValue;
		}
		void BindProperties() {
			BindCellReferenceStyleProperty();
			BindCultureProperty();
			BindDiscardTrailingEmptyCellsProperty();
			BindFormulaExportModeProperty();
			BindNewlineAfterLastRowProperty();
			BindNewlineTypeProperty();
			BindRangeProperty();
			BindShiftTopLeftProperty();
			BindTextQualifierProperty();
			BindUseCellNumberFormatProperty();
			BindValueSeparatorProperty();
			BindWorksheetNameProperty();
			BindWritePreambleProperty();
			BindEncodingProperty();
		}
		void BindCellReferenceStyleProperty() {
			Binding bind = new Binding("CellReferenceStyle") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, CellReferenceStyleProperty, bind);
		}
		void BindCultureProperty() {
			Binding bind = new Binding("Culture") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, CultureProperty, bind);
		}
		void BindDiscardTrailingEmptyCellsProperty() {
			Binding bind = new Binding("DiscardTrailingEmptyCells") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, DiscardTrailingEmptyCellsProperty, bind);
		}
		void BindFormulaExportModeProperty() {
			Binding bind = new Binding("FormulaExportMode") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, FormulaExportModeProperty, bind);
		}
		void BindNewlineAfterLastRowProperty() {
			Binding bind = new Binding("NewlineAfterLastRow") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, NewlineAfterLastRowProperty, bind);
		}
		void BindNewlineTypeProperty() {
			Binding bind = new Binding("NewlineType") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, NewlineTypeProperty, bind);
		}
		void BindRangeProperty() {
			Binding bind = new Binding("Range") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, RangeProperty, bind);
		}
		void BindShiftTopLeftProperty() {
			Binding bind = new Binding("ShiftTopLeft") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, ShiftTopLeftProperty, bind);
		}
		void BindTextQualifierProperty() {
			Binding bind = new Binding("TextQualifier") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, TextQualifierProperty, bind);
		}
		void BindUseCellNumberFormatProperty() {
			Binding bind = new Binding("UseCellNumberFormat") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, UseCellNumberFormatProperty, bind);
		}
		void BindValueSeparatorProperty() {
			Binding bind = new Binding("ValueSeparator") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, ValueSeparatorProperty, bind);
		}
		void BindWorksheetNameProperty() {
			Binding bind = new Binding("Worksheet") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, WorksheetNameProperty, bind);
		}
		void BindWritePreambleProperty() {
			Binding bind = new Binding("WritePreamble") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, WritePreambleProperty, bind);
		}
		void BindEncodingProperty() {
			Binding bind = new Binding("Encoding") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, EncodingProperty, bind);
		}
		public void Reset() {
			if (source != null)
				source.Reset();
		}
	}
	#endregion
}
