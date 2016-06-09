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
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.Export.Xl;
namespace DevExpress.XtraExport.Csv {
	using DevExpress.XtraExport.Implementation;
	public partial class CsvDataAwareExporter : IXlExport, IXlFormulaEngine, IXlExporter {
		#region Fields
		const int contentCapacity = 16384;
		int sheetCount;
		int columnIndex;
		int currentRowIndex;
		int currentColumnIndex;
		Stream outputStream = null;
		XlDocument currentDocument = null;
		XlSheet currentSheet = null;
		XlRow currentRow = null;
		XlColumn currentColumn = null;
		XlCell currentCell = null;
		readonly Dictionary<int, IXlColumn> columns = new Dictionary<int, IXlColumn>();
		readonly CsvDataAwareExporterOptions options = new CsvDataAwareExporterOptions();
		StringBuilder contentBuilder = new StringBuilder(contentCapacity);
		string newline;
		char[] escape;
		string textQualifier;
		string escapedTextQualifier;
		XlDocumentProperties documentProperties = null;
		bool escapeInitialized = false;
		bool rowContentStarted = false;
		#endregion
		public IXlDocumentOptions DocumentOptions { get { return options; } }
		public CsvDataAwareExporterOptions Options { get { return options; } }
		public XlDocumentProperties DocumentProperties { get { return documentProperties; } }
		CultureInfo CurrentCulture {
			get { return Options.Culture; }
		}
		#region IXlExporter implementation
		public IXlDocument CreateDocument(Stream stream) {
			return BeginExport(stream);
		}
		#endregion
		#region IXlExport Members
		public int CurrentRowIndex { get { return currentRow == null ? currentRowIndex : currentRow.RowIndex; } }
		public int CurrentColumnIndex { get { return currentColumnIndex; } }
		public int CurrentOutlineLevel { get { return 0; } }
		public IXlDocument BeginExport(Stream outputStream) {
			Guard.ArgumentNotNull(outputStream, "outputStream");
			this.outputStream = outputStream;
			InitializeExport();
			documentProperties = new XlDocumentProperties();
			documentProperties.Created = DateTime.Now;
			this.currentDocument = new XlDocument(this);
			return currentDocument;
		}
		public void EndExport() {
			currentDocument = null;
			documentProperties = null;
			if(outputStream == null)
				throw new InvalidOperationException("BeginExport/EndExport calls consistency.");
			this.outputStream = null;
		}
		public IXlSheet BeginSheet() {
			if(sheetCount > 0)
				throw new InvalidOperationException("Only one worksheet can be exported to CSV.");
			this.rowContentStarted = false;
			escapeInitialized = false;
			currentRowIndex = 0;
			currentColumnIndex = 0;
			sheetCount++;
			columnIndex = 0;
			columns.Clear();
			contentBuilder.Clear();
			if(Options.WritePreamble)
				WritePreamble();
			currentSheet = new XlSheet(this);
			currentSheet.Name = string.Format("Sheet{0}", sheetCount);
			return currentSheet;
		}
		public void EndSheet() {
			if(currentSheet == null)
				throw new InvalidOperationException("BeginSheet/EndSheet calls consistency.");
			if(Options.NewlineAfterLastRow && currentRowIndex > 0)
				WriteContent(newline);
			currentSheet = null;
		}
		public IXlGroup BeginGroup() {
			return new XlGroup();
		}
		public void EndGroup() {
		}
		public IXlColumn BeginColumn() {
			if(rowContentStarted)
				throw new InvalidOperationException("Columns have to be created before rows and cells.");
			currentColumn = new XlColumn(currentSheet);
			currentColumn.ColumnIndex = columnIndex;
			return currentColumn;
		}
		public void EndColumn() {
			if(currentColumn == null)
				throw new InvalidOperationException("BeginColumn/EndColumn calls consistency.");
			if(rowContentStarted)
				throw new InvalidOperationException("Columns have to be created before rows and cells.");
			if(currentColumn.ColumnIndex < 0 || currentColumn.ColumnIndex >= options.MaxColumnCount)
				throw new ArgumentOutOfRangeException(string.Format("Column index out of range 0...{0}", options.MaxColumnCount - 1));
			currentSheet.RegisterColumnIndex(currentColumn);
			columns[currentColumn.ColumnIndex] = currentColumn;
			columnIndex = currentColumn.ColumnIndex + 1;
			currentColumn = null;
		}
		public void SkipColumns(int count) {
			Guard.ArgumentPositive(count, "count");
			if(currentColumn != null)
				throw new InvalidOperationException("Operation cannot be executed inside BeginColumn/EndColumn scope.");
			if((columnIndex + count) >= options.MaxColumnCount)
				throw new ArgumentOutOfRangeException(string.Format("Column index goes beyond range 0..{0}.", options.MaxColumnCount - 1));
			columnIndex += count;
		}
		public IXlRow BeginRow() {
			if(!escapeInitialized) {
				escape = new char[] { Options.TextQualifier, '\r', '\n', '\v', '\f' };
				textQualifier = new string(Options.TextQualifier, 1);
				escapedTextQualifier = new string(Options.TextQualifier, 2);
				escapeInitialized = true;
			}
			this.rowContentStarted = true;
			currentColumnIndex = 0;
			currentRow = new XlRow(this);
			currentRow.RowIndex = currentRowIndex;
			return currentRow;
		}
		public void EndRow() {
			if(currentRow == null)
				throw new InvalidOperationException("BeginRow/EndRow calls consistency.");
			if(currentRow.RowIndex < 0 || currentRow.RowIndex >= options.MaxRowCount)
				throw new ArgumentOutOfRangeException(string.Format("Row index out of range 0..{0}.", options.MaxRowCount - 1));
			if(currentRow.RowIndex < currentRowIndex)
				throw new InvalidOperationException("RowIndex consistency.");
			AppendNewlines(currentRow.RowIndex);
			if(currentColumnIndex > 0) {
				WriteContent(contentBuilder.ToString());
				contentBuilder.Clear();
			}
			currentRowIndex = currentRow.RowIndex + 1;
			currentRow = null;
		}
		public void SkipRows(int count) {
			Guard.ArgumentPositive(count, "count");
			if(currentRow != null)
				throw new InvalidOperationException("Operation cannot be executed inside BeginRow/EndRow scope.");
			int newRowIndex = currentRowIndex + count;
			if(newRowIndex >= options.MaxRowCount)
				throw new ArgumentOutOfRangeException(string.Format("Row index goes beyond range 0..{0}.", options.MaxRowCount - 1));
			currentColumnIndex = 0;
			AppendNewlines(newRowIndex - 1);
			currentRowIndex = newRowIndex;
		}
		public IXlCell BeginCell() {
			AppendNewlines(currentRow.RowIndex);
			currentCell = new XlCell();
			currentCell.RowIndex = CurrentRowIndex;
			currentCell.ColumnIndex = currentColumnIndex;
			currentCell.Formatting = XlFormatting.CopyObject(currentRow.Formatting);
			IXlColumn column;
			if(columns.TryGetValue(currentColumnIndex, out column))
				currentCell.Formatting = XlFormatting.CopyObject(column.Formatting);
			return currentCell;
		}
		public void EndCell() {
			if(currentCell == null)
				throw new InvalidOperationException("BeginCell/EndCell calls consistency.");
			if(currentCell.ColumnIndex < 0 || currentCell.ColumnIndex >= options.MaxColumnCount)
				throw new ArgumentOutOfRangeException(string.Format("Cell column index out of range 0..{0}.", options.MaxColumnCount - 1));
			if(currentCell.ColumnIndex < currentColumnIndex)
				throw new InvalidOperationException("Cell column index consistency.");
			if(currentCell.Value.IsNumeric && DevExpress.XtraExport.Xls.XNumChecker.IsNegativeZero(currentCell.Value.NumericValue))
				currentCell.Value = 0.0;
			currentSheet.RegisterCellPosition(currentCell);
			if(currentCell.ColumnIndex > 0) {
				int pendingSeparators = currentCell.ColumnIndex - currentColumnIndex;
				if(currentColumnIndex > 0)
					pendingSeparators++;
				for(int i = 0; i < pendingSeparators; i++)
					contentBuilder.Append(Options.ValueSeparatorString);
			}
			GenerateCellValue(currentCell);
			currentColumnIndex = currentCell.ColumnIndex + 1;
			currentCell = null;
		}
		public void SkipCells(int count) {
			Guard.ArgumentPositive(count, "count");
			if(currentCell != null)
				throw new InvalidOperationException("Operation cannot be executed inside BeginCell/EndCell scope.");
			if((currentColumnIndex + count) >= options.MaxColumnCount)
				throw new ArgumentOutOfRangeException(string.Format("Cell column index goes beyond range 0..{0}.", options.MaxColumnCount - 1));
			AppendNewlines(currentRow.RowIndex);
			int pendingSeparators = count - 1;
			if(currentColumnIndex > 0)
				pendingSeparators++;
			for(int i = 0; i < pendingSeparators; i++)
				contentBuilder.Append(Options.ValueSeparatorString);
			currentColumnIndex += count;
		}
		public IXlFormulaEngine FormulaEngine { get { return this; } }
		XlPicture currentPicture = null;
		public IXlPicture BeginPicture() {
			currentPicture = new XlPicture(this);
			return currentPicture;
		}
		public void EndPicture() {
			currentPicture = null;
		}
		#endregion
		#region IXlFormulaEngine Members
		IXlFormulaParameter IXlFormulaEngine.Param(XlVariantValue value) {
			return null;
		}
		IXlFormulaParameter IXlFormulaEngine.Subtotal(XlCellRange range, XlSummary summary, bool ignoreHidden) {
			return null;
		}
		IXlFormulaParameter IXlFormulaEngine.Subtotal(IList<XlCellRange> ranges, XlSummary summary, bool ignoreHidden) {
			return null;
		}
		IXlFormulaParameter IXlFormulaEngine.VLookup(XlVariantValue lookupValue, XlCellRange table, int columnIndex, bool rangeLookup) {
			return null;
		}
		IXlFormulaParameter IXlFormulaEngine.VLookup(IXlFormulaParameter lookupValue, XlCellRange table, int columnIndex, bool rangeLookup) {
			return null;
		}
		IXlFormulaParameter IXlFormulaEngine.Text(XlVariantValue value, string netFormatString, bool isDateTimeFormatString) {
			return null;
		}
		IXlFormulaParameter IXlFormulaEngine.Text(IXlFormulaParameter value, string netFormatString, bool isDateTimeFormatString) {
			return null;
		}
		IXlFormulaParameter IXlFormulaEngine.Concatenate(params IXlFormulaParameter[] parameters) {
			return null;
		}
		#endregion
		void WritePreamble() {
			byte[] preamble = Options.Encoding.GetPreamble();
			if(preamble.Length > 0)
				outputStream.Write(preamble, 0, preamble.Length);
		}
		void InitializeExport() {
			sheetCount = 0;
			currentRowIndex = 0;
			currentColumnIndex = 0;
			newline = GetNewline(Options.NewlineType);
			escapeInitialized = false;
		}
		string GetNewline(CsvNewlineType newlineType) {
			switch(newlineType) {
				case CsvNewlineType.LfCr:
					return "\n\r";
				case CsvNewlineType.Cr:
					return "\r";
				case CsvNewlineType.Lf:
					return "\n";
				case CsvNewlineType.VerticalTab:
					return "\v";
				case CsvNewlineType.FormFeed:
					return "\f";
			}
			return "\r\n";
		}
		void WriteContent(string value) {
			byte[] buf = Options.Encoding.GetBytes(value);
			outputStream.Write(buf, 0, buf.Length);
		}
		void GenerateCellValue(IXlCell cell) {
			if(!Options.UseCellNumberFormat)
				GenerateNonFormattedCellValue(cell);
			else
				GenerateFormattedCellValue(cell);
		}
		void GenerateNonFormattedCellValue(IXlCell cell) {
			XlVariantValue value = cell.Value;
			if(value.IsEmpty)
				return;
			if(value.IsNumeric) {
				bool isDateTimeFormat = cell.Formatting != null ? cell.Formatting.IsDateTimeFormatString : false;
				if(isDateTimeFormat)
					GenerateTextValue(value.DateTimeValue.ToString("d", CurrentCulture));
				else
					GenerateTextValue(value.NumericValue.ToString("G", CurrentCulture));
			}
			else if(value.IsText)
				GenerateTextValue(value.TextValue);
			else if(value.IsBoolean)
				contentBuilder.Append(value.BooleanValue ? "TRUE" : "FALSE");
			else if (value.IsError)
				contentBuilder.Append(value.ErrorValue.Name);
		}
		void GenerateFormattedCellValue(IXlCell cell) {
			XlVariantValue value = cell.Value;
			if(value.IsEmpty)
				return;
			if (value.IsError) {
				contentBuilder.Append(value.ErrorValue.Name);
				return;
			}
			string formatCode = string.Empty;
			bool isDateTimeFormat = false;
			XlCellFormatting formatting = cell.Formatting;
			if(formatting != null) {
				if(!string.IsNullOrEmpty(formatting.NetFormatString)) {
					formatCode = formatting.NetFormatString;
					isDateTimeFormat = formatting.IsDateTimeFormatString;
				}
				else if(formatting.NumberFormat != null) {
					GenerateTextValue(FormatValue(formatting.NumberFormat, cell.Value));
					return;
				}
			}
			if(string.IsNullOrEmpty(formatCode)) {
				GenerateNonFormattedCellValue(cell);
				return;
			}
			XlExportNetFormatParser parser = new XlExportNetFormatParser(formatCode);
			if(value.IsNumeric) {
				if(isDateTimeFormat)
					GenerateDateTimeFormattedValue(value, parser);
				else
					GenerateNumericFormattedValue(value, parser);
			}
			else if(value.IsText) {
				GenerateTextValue(string.Format(XlExportNetFormatComposer.CreateFormat(parser.Prefix, string.Empty, parser.Postfix), value.TextValue));
			}
			else if(value.IsBoolean)
				GenerateTextValue(string.Format(XlExportNetFormatComposer.CreateFormat(parser.Prefix, string.Empty, parser.Postfix), value.BooleanValue ? "TRUE" : "FALSE"));
		}
		void GenerateNumericFormattedValue(XlVariantValue value, XlExportNetFormatParser parser) {
			string formatString = parser.FormatString;
			if(string.IsNullOrEmpty(formatString))
				formatString = "G";
			string formattedValue;
			int integralValue;
			if(TryGetIntegralValue(value.NumericValue, out integralValue) && integralValue == value.NumericValue) {
				try {
					formattedValue = string.Format(CurrentCulture, XlExportNetFormatComposer.CreateFormat(parser.Prefix, formatString, parser.Postfix), integralValue);
				}
				catch(FormatException) {
					formattedValue = string.Format(CurrentCulture, XlExportNetFormatComposer.CreateFormat(parser.Prefix, "G", parser.Postfix), integralValue);
				}
			}
			else {
				string prefix = parser.Prefix;
				char formatChar = formatString[0];
				if(formatChar == 'd' || formatChar == 'D')
					formatString = "G";
				else if(formatChar == 'x' || formatChar == 'X') {
					formatString = "G";
					if(prefix == "0x")
						prefix = string.Empty;
				}
				try {
					formattedValue = string.Format(CurrentCulture, XlExportNetFormatComposer.CreateFormat(prefix, formatString, parser.Postfix), value.NumericValue);
				}
				catch(FormatException) {
					formattedValue = string.Format(CurrentCulture, XlExportNetFormatComposer.CreateFormat(prefix, "G", parser.Postfix), value.NumericValue);
				}
			}
			GenerateTextValue(formattedValue);
		}
		void GenerateDateTimeFormattedValue(XlVariantValue value, XlExportNetFormatParser parser) {
			string formatString = parser.FormatString;
			if(string.IsNullOrEmpty(formatString))
				formatString = "d";
			string formattedValue;
			try {
				formattedValue = string.Format(CurrentCulture, XlExportNetFormatComposer.CreateFormat(parser.Prefix, formatString, parser.Postfix), value.DateTimeValue);
			}
			catch(FormatException) {
				formattedValue = string.Format(CurrentCulture, XlExportNetFormatComposer.CreateFormat(parser.Prefix, "d", parser.Postfix), value.DateTimeValue);
			}
			GenerateTextValue(formattedValue);
		}
		bool TryGetIntegralValue(double value, out int integralValue) {
			try {
				integralValue = Convert.ToInt32(value);
				return true;
			}
			catch(OverflowException) {
				integralValue = 0;
				return false;
			}
		}
		void GenerateTextValue(string text) {
			bool escaped = text.IndexOfAny(escape) >= 0 || text.IndexOf(Options.ValueSeparatorString) >= 0;
			if(escaped) {
				contentBuilder.Append(Options.TextQualifier);
				contentBuilder.Append(text.Replace(textQualifier, escapedTextQualifier));
				contentBuilder.Append(Options.TextQualifier);
			}
			else
				contentBuilder.Append(text);
		}
		void AppendNewlines(int rowIndex) {
			if(currentColumnIndex == 0) {
				int pendingNewlines = rowIndex - currentRowIndex;
				if(currentRowIndex > 0)
					pendingNewlines++;
				for(int i = 0; i < pendingNewlines; i++)
					contentBuilder.Append(newline);
			}
		}
	}
}
