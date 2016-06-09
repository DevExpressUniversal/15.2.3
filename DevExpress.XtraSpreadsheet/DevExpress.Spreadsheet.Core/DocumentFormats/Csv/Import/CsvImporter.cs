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

using DevExpress.Internal;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Services;
using DevExpress.XtraSpreadsheet.Utils;
using System;
using System.IO;
using System.Text;
namespace  DevExpress.XtraSpreadsheet.Import.Csv {
	internal enum CsvImporterSetCellValueResult { Ok, MaxRowsLimit, InvalidColumn, InvalidRow };
	#region CsvImporter
	public class CsvImporter : DocumentModelImporter {
		#region Fields
		internal const int separatorDetectorCharCounterLimit = 4096;
		internal const int CharCounterLimit = 32768;
		const int valueAccumulatorStartSize = 4096;
		public const string InvalidFileMessage = "Invalid CSV file";
		TextDocumentImporterOptionsBase options;
		ByteBuilder valueAccumulator;
		int currentColumnRelative;
		int currentRowRelative;
		Worksheet currentSheet;
		IWorksheetNameCreationService worksheetNameCreationService;
		CellPosition startCellPosition = CellPosition.InvalidValue;
		#endregion
		public CsvImporter(IDocumentModel documentModel, TextDocumentImporterOptionsBase options)
			: base(documentModel) {
			this.options = options;
			this.valueAccumulator = new ByteBuilder(valueAccumulatorStartSize);
		}
		#region Properties
		IWorksheetNameCreationService WorksheetNameCreationService {
			get {
				if (worksheetNameCreationService == null)
					worksheetNameCreationService = DocumentModel.GetService<IWorksheetNameCreationService>();
				return worksheetNameCreationService;
			}
		}
		public new DocumentModel DocumentModel { get { return (DocumentModel)base.DocumentModel; } }
		public TextDocumentImporterOptionsBase Options { get { return options; } }
		protected internal ByteBuilder CellValue { get { return valueAccumulator; } }
		#endregion
		bool ArrayContainBytes(byte[] what, byte[] where, int fromPos) {
			int count = what.Length;
			if(count < 1)
				return true; 
			if((where.Length - fromPos) < count)
				return false;
			for(int i = 0; i < count; ++i) {
				if(where[i + fromPos] != what[i])
					return false;
			}
			return true;
		}
		string GetSourceFileName(Stream stream) {
			string result = Path.GetFileNameWithoutExtension(Options.SourceUri);
			if (string.IsNullOrEmpty(result)) {
				FileStream fs = stream as FileStream;
				if ((fs == null) || string.IsNullOrEmpty(fs.Name))
					return ValidateSheetName(null);
				result = Path.GetFileNameWithoutExtension(fs.Name);
			}
			return ValidateSheetName(result);
		}
		string ValidateSheetName(string name) {
			string[] existingSheetNames = DocumentModel.Sheets.GetSheetNames();
			if (WorksheetNameCreationService != null)
				return WorksheetNameCreationService.GetNormalizedName(name, existingSheetNames, false);
			return DateTime.Now.Ticks.ToString("X8");
		}
		bool GetUseR1C1() {
			if(options.CellReferenceStyle == CellReferenceStyle.WorkbookDefined)
				return DocumentModel.Properties.UseR1C1ReferenceStyle;
			return (options.CellReferenceStyle == CellReferenceStyle.R1C1);
		}
		public void Import(Stream stream) {
			int sheetIndex;
			bool intoNewWorkbook = string.IsNullOrEmpty(Options.Worksheet);
			DocumentModel.BeginSetContent(intoNewWorkbook);
			try {
				DocumentModel.DataContext.SetImportExportSettings(options.Culture, GetUseR1C1());
				if(intoNewWorkbook) {
					string sheetName = GetSourceFileName(stream);
					sheetIndex = DocumentModel.Sheets.Add(new Worksheet(DocumentModel, sheetName));
				}
				else {
					sheetIndex = DocumentModel.Sheets.GetSheetIndexByName(Options.Worksheet);
					if(sheetIndex < 0)
						Exceptions.ThrowArgumentException("Options.Worksheet", Options.Worksheet);
					if(Options.ClearWorksheetBeforeImport)
						DocumentModel.Sheets[sheetIndex].Clear();
				}
				ImportWorksheet(stream, DocumentModel.Sheets[sheetIndex]);
			}
			catch {
				if (Options.CreateEmptyDocumentOnLoadError)
					DocumentModel.SetMainDocumentEmptyContentCore();
				throw;
			}
			finally {
				DocumentModel.PrepareFormulas();
				DocumentModel.RecalculateAfterLoad = true;
				DocumentModel.DataContext.SetWorkbookDefinedSettings();
				DocumentModel.EndSetContent(DocumentModelChangeType.LoadNewDocument);
			}
		}
#if DEBUGTEST
		public void SetStartCell(int column, int row) {
			currentColumnRelative = column;
			currentRowRelative = row;
		}
#endif
		internal void MoveToNextRow() {
			currentRowRelative++;
		}
		internal void MoveToFirstColumnInRow() {
			currentColumnRelative = 0;
		}
		internal void MoveToNextPositionInRow() {
			currentColumnRelative++;
		}
		protected internal bool ApplyCellValue() {
			if (currentRowRelative < Options.StartRowToImport)
				return DispatchResult(CsvImporterSetCellValueResult.Ok);
			if (currentRowRelative >= Options.MaxRowCountToImport)
				return DispatchResult(CsvImporterSetCellValueResult.MaxRowsLimit);
			CsvImporterSetCellValueResult result = SetNewCellValue(currentSheet, valueAccumulator);
			return DispatchResult(result);
		}
		CsvImporterSetCellValueResult SetNewCellValue(Worksheet sheet, ByteBuilder value) {
			int rowIndex = currentRowRelative + startCellPosition.Row - Options.StartRowToImport;
			if (!IndicesChecker.CheckIsRowIndexValid(rowIndex))
				return CsvImporterSetCellValueResult.InvalidRow;
			int columnIndex = currentColumnRelative + startCellPosition.Column;
			if (!IndicesChecker.CheckIsColumnIndexValid(columnIndex))
				return CsvImporterSetCellValueResult.InvalidColumn;
			if (value.Length <= 0)
				return CsvImporterSetCellValueResult.Ok;
			string newValue = value.ToString(Options.ActualEncoding);
			if (Options.TrimBlanks) {
				newValue = newValue.Trim();
				if (newValue.Length < 1)
					return CsvImporterSetCellValueResult.Ok;
			}
			ICell cell = sheet[columnIndex, rowIndex];
			AssignCellValue(cell, newValue);
			return CsvImporterSetCellValueResult.Ok;
		}
		void AssignCellValue(ICell cell, string newValue) {
			DocumentModel workbook = cell.Worksheet.Workbook;
			bool suppressCellValueAssignment = workbook.SuppressCellValueAssignment;
			workbook.SuppressCellValueAssignment = false;
			switch (Options.ValueTypeDetectMode) {
				default:
				case CsvValueTypeDetectMode.Default:
				case CsvValueTypeDetectMode.Advanced:
					AssignCellValueAdvancedConversion(cell, newValue);
					break;
				case CsvValueTypeDetectMode.Simple:
					AssignCellValueSimpleConversion(cell, newValue);
					break;
				case CsvValueTypeDetectMode.None:
					AssignCellValueNoConversion(cell, newValue);
					break;
			}
			workbook.SuppressCellValueAssignment = suppressCellValueAssignment;
		}
		void AssignCellValueAdvancedConversion(ICell cell, string newValue) {
			WorkbookDataContext context = DocumentModel.DataContext;
			if (TryInterpretAsFormula(cell, newValue, context))
				return;
			FormattedVariantValue formattedValue = CellValueFormatter.GetValue(VariantValue.Empty, newValue, context, true);
			if (formattedValue.Value.IsInlineText)
				AssignSharedString(cell, formattedValue.Value.InlineTextValue);
			else
				cell.SetFormattedValue(formattedValue, newValue, cell.AssignValueCore);
		}
		void AssignCellValueSimpleConversion(ICell cell, string newValue) {
			WorkbookDataContext context = DocumentModel.DataContext;
			if (TryInterpretAsFormula(cell, newValue, context))
				return;
			VariantValue value = context.ConvertTextToVariantValueWithCaching(newValue);
			if (value.IsInlineText)
				AssignSharedString(cell, value.InlineTextValue);
			else
				cell.AssignValueCore(value);
		}
		void AssignCellValueNoConversion(ICell cell, string newValue) {
			WorkbookDataContext context = DocumentModel.DataContext;
			if (TryInterpretAsFormula(cell, newValue, context))
				return;
			AssignSharedString(cell, newValue);
		}
		void AssignSharedString(ICell cell, string newValue) {
			VariantValue value = newValue;
			value.SetSharedString(cell.Worksheet.SharedStringTable, newValue);
			cell.AssignValueCore(value);
		}
		bool TryInterpretAsFormula(ICell cell, string value, WorkbookDataContext context) {
			if ((value.Length > 1) && (value[0] == '=')) {
				try {
					Formula cellFormula = new Formula(cell, value);
					cell.ApplyFormulaCore(cellFormula);
					cell.MarkUpForRecalculation();
				}
				catch (ArgumentException) {
					return false;
				}
				return true;
			}
			return false;
		}
		void DetectEncoding(Stream stream) {
			if(options.AutoDetectEncoding) {
				InternalEncodingDetector detector = new InternalEncodingDetector();
				Encoding encoding = detector.Detect(stream);
				if(encoding != null)
					options.ActualEncoding = encoding;
			}
		}
		void SkipPreamble(Stream stream) {
			byte[] preamble = Options.ActualEncoding.GetPreamble();
			if((preamble.Length > 0) && (stream.Length >= preamble.Length)) {
				byte[] bytes = new byte[preamble.Length];
				stream.Read(bytes, 0, preamble.Length);
				if(!ArrayContainBytes(preamble, bytes, 0))
					stream.Seek(0, SeekOrigin.Begin);
			}
		}
		void ImportWorksheet(Stream stream, Worksheet sheet) {
			currentSheet = sheet;
			DetectEncoding(stream);
			SkipPreamble(stream);
			CsvImportStreamAdaptor adaptor = CsvImportStreamAdaptor.CreateAdaptor(stream, options);
			if (Options.AutoDetectDelimiter) {
				CsvImportSeparatorDetector detector = new CsvImportSeparatorDetector(adaptor);
				Options.SetDetectedDelimiter(detector.DetectSeparator(options.FallbackDelimiter));
			}
			try {
				this.startCellPosition = CellReferenceParser.TryParse(Options.StartCellToInsert);
				if (Object.ReferenceEquals(startCellPosition, CellPosition.InvalidValue))
					throw new Exception();
				CsvImportReader reader = new CsvImportReader(adaptor);
				if((stream.Length - stream.Position) > 0) {
					reader.ProcessChars(this);
					if (CellValue.Length > 0)
						ApplyCellValue();
				}
			}
			catch(Exception e) {
				if((e.GetType() == typeof(System.InvalidOperationException)) &&
				   ((e.Message == XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorIncorrectColumnIndex)) ||
					(e.Message == XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorIncorrectRowIndex))))
					throw;
				if (!DocumentModel.RaiseInvalidFormatException(e))
					throw;
			}
		}
		internal bool DispatchResult(CsvImporterSetCellValueResult value) {
			if(value == CsvImporterSetCellValueResult.Ok)
				return true;
			if(value == CsvImporterSetCellValueResult.MaxRowsLimit)
				return false;
			if (Options.CellIndexOutOfRangeStrategy == CsvImportCellIndexStrategy.Throw) {
				SpreadsheetExceptions.ThrowInvalidOperationException(value == CsvImporterSetCellValueResult.InvalidColumn ?
																						XtraSpreadsheetStringId.Msg_ErrorIncorrectColumnIndex :
																						XtraSpreadsheetStringId.Msg_ErrorIncorrectRowIndex);
			}
			return false;
		}
		public override void ThrowInvalidFile() {
			throw new Exception(InvalidFileMessage);
		}
	}
	#endregion
	#region ByteBuilder helper class
	public class ByteBuilder {
		const int defaultSize = 4096;
		const int growSize = 4096;
		byte[] data = null;
		int size = 0;
		int length = 0;
		public ByteBuilder() {
			this.data = new byte[defaultSize];
			this.size = defaultSize;
		}
		public ByteBuilder(int size) {
			this.data = new byte[size];
			this.size = size;
		}
		public int Length {
			get { return length; }
			set {
				if(value < size) {
					length = value;
				}
			}
		}
		protected void Grow() {
			int newsize = data.Length + growSize;
			byte[] newdata = new byte[newsize];
			if(data != null)
				data.CopyTo(newdata, 0);
			data = newdata;
			size = data.Length;
		}
		public void Append(byte value) {
			if(length >= size)
				Grow();
			data[length++] = value;
		}
		public void Append(Int32 value) {
			if((length + 4) >= size)
				Grow();
			data[length++] = (byte)value;
			data[length++] = (byte)(value >> 8);
			data[length++] = (byte)(value >> 16);
			data[length++] = (byte)(value >> 24);
		}
		public void AppendBigEndian(Int32 value) {
			if((length + 4) >= size)
				Grow();
			data[length++] = (byte)(value >> 24);
			data[length++] = (byte)(value >> 16);
			data[length++] = (byte)(value >> 8);
			data[length++] = (byte)value;
		}
		public void Append(char value) {
			if((length + 2) >= size)
				Grow();
			data[length++] = (byte)value;
			data[length++] = (byte)(value >> 8);
		}
		public void AppendBigEndian(char value) {
			if((length + 2) >= size)
				Grow();
			data[length++] = (byte)(value >> 8);
			data[length++] = (byte)value;
		}
		public string ToString(Encoding encoding) {
			return (data != null) ? encoding.GetString(data, 0, length) : string.Empty;
		}
	}
	#endregion
}
