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
using System.Text;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Export;
using DevExpress.XtraSpreadsheet.Model.CopyOperation;
using System.Collections.Generic;
#if !DXPORTABLE
using PlatformIndependentDataObject = System.Windows.Forms.DataObject;
using System.Windows.Forms;
#else
using PlatformIndependentDataObject = DevExpress.Compatibility.System.Windows.Forms.DataObject;
using DevExpress.Compatibility.System.Windows.Forms;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	public interface ICopiedRangeDataForClipboard {
		string[] DataFormatsForCopy { get; }
		string[] DataFormatsAfterCloseDocument { get; }
		object GetData(string format, CopiedRangeProvider copiedRangeProvider);
	}
	public class CopiedRangeDataForClipboard : ICopiedRangeDataForClipboard {
		DocumentModel documentModel;
		Dictionary<string, Func<string,CellRange,Worksheet, object>> table = null;
		public CopiedRangeDataForClipboard(DocumentModel documentModel) {
			this.documentModel = documentModel;
			this.table = new Dictionary<string, Func<string, CellRange, Worksheet, object>>();
			if(!this.table.ContainsKey(SpreadsheetDataFormats.Text))
				this.table.Add(SpreadsheetDataFormats.Text, GetTextUtf8);
			if (!this.table.ContainsKey(OfficeDataFormats.UnicodeText))
				this.table.Add(OfficeDataFormats.UnicodeText, GetTextUnicode);
			this.table.Add(SpreadsheetDataFormats.CommaSeparatedValue, GetCsvStream);
			this.table.Add(SpreadsheetDataFormats.Biff8, GetXlsStream);
			this.table.Add(SpreadsheetDataFormats.Link, GetLinkStream);
			this.table.Add(OfficeDataFormats.Html, GetHtmlStream);
		}
		public DocumentModel DocumentModel { get { return documentModel; } }
		public object GetTextUtf8(string rangeReference, CellRange range, Worksheet sheet) {
			return GetText(rangeReference, range, sheet, Encoding.UTF8);
		}
		public object GetTextUnicode(string rangeReference, CellRange range, Worksheet sheet) {
			return GetText(rangeReference, range, sheet, Encoding.Unicode);
		}
		public object GetText(string rangeReference, CellRange range, Worksheet sheet, Encoding encoding) {
			TxtDocumentExporterOptions options = new TxtDocumentExporterOptions();
			options.ValueSeparator = '\t';
			options.FormulaExportMode = sheet.ActiveView.ShowFormulas ?
				FormulaExportMode.Formula : FormulaExportMode.CalculatedValue;
			options.Range = rangeReference;
			options.Worksheet = sheet.Name;
			options.Culture = DocumentModel.Culture;
			options.Encoding = encoding;
			options.NewlineAfterLastRow = true;
			options.IsNullTerminated = true;
			return DocumentModel.InternalAPI.GetDocumentCsvContent(options);
		}
		public object GetCsvStream(string rangeReference,CellRange range, Worksheet sheet) {
			CsvDocumentExporterOptions options = new CsvDocumentExporterOptions();
			options.FormulaExportMode = sheet.ActiveView.ShowFormulas ?
				FormulaExportMode.Formula : FormulaExportMode.CalculatedValue;
			options.Range = rangeReference;
			options.Worksheet = sheet.Name;
			options.Culture = DocumentModel.Culture;
			string csvText = DocumentModel.InternalAPI.GetDocumentCsvContent(options);
			if (String.IsNullOrEmpty(csvText))
				return null;
			byte[] utf8Bytes = Encoding.UTF8.GetBytes(csvText);
			MemoryStream csvStream = new MemoryStream(utf8Bytes);
			return csvStream;
		}
		public object GetHtmlStream(string rangeReference, CellRange range, Worksheet sheet) {
			HtmlDocumentExporterOptions options = new HtmlDocumentExporterOptions();
			options.Range = rangeReference;
			options.ExportImages = false;
			int sheetIndex = DocumentModel.Sheets.GetIndexById(sheet.SheetId);
			options.SheetIndex = sheetIndex;
			string htmlText = DocumentModel.InternalAPI.GetDocumentCF_HTMLContent(options);
			if (String.IsNullOrEmpty(htmlText))
				return null;
			byte[] htmlBytes = Encoding.UTF8.GetBytes(htmlText);
			MemoryStream stream = new MemoryStream(htmlBytes);
			return stream;
		}
		protected internal DocumentModel PrepareTemporaryModelWithCopiedContent(DocumentModel existingModel, CellRange rangeToCopy, string sheetName) {
			DocumentModel newDocumentModel = existingModel.CreateEmptyCopy();
			newDocumentModel.History.DisableHistory();
			WorkbookSaveOptions documentSaveOptions = existingModel.DocumentSaveOptions;
			string filePath = documentSaveOptions.CurrentFileName;
			if (documentSaveOptions.CanSaveToCurrentFileName && !String.IsNullOrEmpty(filePath) && documentSaveOptions.CurrentFormat != DevExpress.Spreadsheet.DocumentFormat.Undefined) {
				WorkbookSaveOptions newModelSaveOptions = newDocumentModel.DocumentSaveOptions;
				newModelSaveOptions.CurrentFileName = filePath;
				newModelSaveOptions.CurrentFormat = documentSaveOptions.CurrentFormat;
			}
			Worksheet newSheet = newDocumentModel.CreateWorksheet(sheetName);
			newDocumentModel.Sheets.Add(newSheet);
			Model.CellRangeBase rangeInTemporaryModel = rangeToCopy.Clone(newSheet);
			ModelPasteSpecialFlags flags = (ModelPasteSpecialFlags.All | ModelPasteSpecialFlags.ColumnWidths) & ~ModelPasteSpecialFlags.Formulas;
			var ranges = new SourceTargetRangesForCopy(rangeToCopy, rangeInTemporaryModel);
			RangeCopyOperation op = new RangeCopyOperation(ranges, flags);
			op.SheetDefinitionToOuterAreasReplaceMode = SheetDefToOuterAreasReplaceMode.EntireFormulaToValue;
			op.DisabledHistory = true;
			op.SuppressChecks = true;
			op.Execute();
			return newDocumentModel;
		}
		public object GetLinkStream(string reference, CellRange range, Worksheet sheet) {
			string currentFileName = DocumentModel.DocumentSaveOptions.CurrentFileName;
			ClipboardLinkBuilder builder = new ClipboardLinkBuilder(DocumentModel.Culture, currentFileName);
			string link = builder.Build(range);
			byte[] bytes = DevExpress.Utils.DXEncoding.Default.GetBytes(link);
			MemoryStream linkStream = new MemoryStream(bytes);
			return linkStream;
		}
		public object GetXlsStream(string rangeReference, CellRange range, Worksheet sheet) {
			DocumentModel newDocumentModel = PrepareTemporaryModelWithCopiedContent(this.DocumentModel, range, sheet.Name);
			XlsDocumentExporterOptions options = new XlsDocumentExporterOptions();
			ChunkedMemoryStream ms = new ChunkedMemoryStream();
			CellRange rangeAbs = range.GetWithModifiedPositionType(PositionType.Absolute) as CellRange;
			CellRangeInfo rangeToExport = new CellRangeInfo(rangeAbs.TopLeft, rangeAbs.BottomRight);
			newDocumentModel.InternalAPI.SaveBiff8ContentForClipboard(ms, options, rangeToExport);
			return ms;
		}
		public string[] DataFormatsForCopy {
			get {
				string[] result = new string[] {
					OfficeDataFormats.UnicodeText,
					SpreadsheetDataFormats.Text,
					SpreadsheetDataFormats.CommaSeparatedValue,
					OfficeDataFormats.Html,
					SpreadsheetDataFormats.Biff8,
					SpreadsheetDataFormats.Link
				};
				return result;
			}
		}
		public string[] DataFormatsAfterCloseDocument {
			get {
				string[] result = new string[] {
					OfficeDataFormats.UnicodeText,
					SpreadsheetDataFormats.Text
				};
				return result;
			}
		}
		public object GetData(string format, CopiedRangeProvider copiedRange) {
			CellRange range = copiedRange.Range;
			if (range == null)
				throw new InvalidOperationException("DocumentModel.IsCopyCutMode is false");
			Worksheet sheet = range.Worksheet as Worksheet;
			string rangeReferenceCroppedByPrintRange = copiedRange.RangeReferenceCroppedByPrintRange;
			Func<string, CellRange, Worksheet, object> func = null;
			if(!table.TryGetValue(format, out func))
				throw new InvalidOperationException(String.Concat(format, " as clipboard data format is not suppored"));
			return func(rangeReferenceCroppedByPrintRange, range, sheet);
		}
	}
}
