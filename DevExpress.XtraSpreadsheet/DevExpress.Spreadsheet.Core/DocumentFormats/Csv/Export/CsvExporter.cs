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

using System.IO;
using System.Text;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Import;
using System;
using System.Drawing;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraSpreadsheet.Export.Csv {
	public class CsvExporter : DocumentModelExporter {
		#region Fields
		StringBuilder valueAccumulator;
		char valueSeparator;
		char quoteChar;
		byte[] newline;
		byte[] nullByte;
		byte[] quotation;
		byte[] separator;
		Stream outputStream;
		TextDocumentExporterOptionsBase options;
		Rectangle sourceArea;
		#endregion
		public CsvExporter(DocumentModel workbook, TextDocumentExporterOptionsBase options)
			: base(workbook) {
			this.options = options;
		}
		#region Properties
		public TextDocumentExporterOptionsBase Options { get { return options; } set { options = value; } }
		protected internal StringBuilder ValueAccumulator { get { return valueAccumulator; } set { valueAccumulator = value; } }
		protected Rectangle SourceArea { get { return sourceArea; } set { sourceArea = value; } }
		protected Encoding Encoding { get { return options.Encoding; } }
		#endregion
		protected bool GetUseR1C1() {
			if (options.CellReferenceStyle == CellReferenceStyle.WorkbookDefined)
				return Workbook.Properties.UseR1C1ReferenceStyle;
			return (options.CellReferenceStyle == CellReferenceStyle.R1C1);
		}
		public string ExportAsString() {
			using (MemoryStream ms = new MemoryStream()) {
				Export(ms);
				return ReadFromStream(ms, false);
			}
		}
		public virtual void Export(Stream outputStream) {
			this.outputStream = outputStream;
			try {
				Workbook.DataContext.SetImportExportSettings(options.Culture, GetUseR1C1());
				Export();
			}
			finally {
				Workbook.DataContext.SetWorkbookDefinedSettings();
			}
		}
		public override void Export() {
			Guard.ArgumentNotNull(this.outputStream, "outputStream");
			if (!string.IsNullOrEmpty(Options.Worksheet)) {
				int sheetId = Workbook.Sheets.GetSheetIndexByName(Options.Worksheet);
				if (sheetId >= 0) {
					ExportSheet(Workbook.Sheets[sheetId]);
				}
				else
					Exceptions.ThrowArgumentException("Options.Worksheet", Options.Worksheet);
			}
			else
				ExportSheet(Workbook.ActiveSheet);
		}
		protected string GetNewlineSequence(NewlineType newlineType) {
			switch (newlineType) {
				case NewlineType.LfCr:
					return "\n\r";
				case NewlineType.Cr:
					return "\r";
				case NewlineType.Lf:
					return "\n";
				case NewlineType.VerticalTab:
					return "\v";
				case NewlineType.FormFeed:
					return "\f";
			}
			return "\r\n";
		}
		protected void WritePreamble() {
			byte[] preamble = Encoding.GetPreamble();
			if (preamble.Length > 0)
				outputStream.Write(preamble, 0, preamble.Length);
		}
		protected Rectangle DetermineSourceArea(Worksheet sheet, bool anchoredLeftTop) {
			IRowCollection sheetRows = sheet.Rows;
			if (sheetRows.Count < 1)
				return new Rectangle();
			int initialRow = 0;
			int initialColumn = 0;
			int lastRow = sheetRows.Last.Index;
			int lastColumn = 0;
			if (!String.IsNullOrEmpty(Options.Range)) {
				if (!String.IsNullOrEmpty(Options.Worksheet))
					System.Diagnostics.Debug.Assert(options.Worksheet == sheet.Name);
				var parser = new OptionsRangeParser();
				CellRange range = parser.CalculateOptionsRange(sheet, Options.Range);
				if (range != null) {
					initialRow = range.TopLeft.Row;
					initialColumn = range.TopLeft.Column;
					lastRow = range.BottomRight.Row;
					lastColumn = range.BottomRight.Column;
					return Rectangle.FromLTRB(initialColumn, initialRow, lastColumn + 1, lastRow + 1);
				}
			}
			int rowOffset = anchoredLeftTop ? initialRow : sheetRows.First.Index;
			int columnOffset = anchoredLeftTop ? initialColumn : Int32.MaxValue;
			foreach (Row currentRow in sheetRows.GetExistingRows(rowOffset, lastRow, false)) {
				int firstColumnIndex = currentRow.FirstColumnIndex;
				if (firstColumnIndex >= 0) { 
					int lastColumnIndex = currentRow.LastColumnIndex;
					if (firstColumnIndex < columnOffset)
						columnOffset = firstColumnIndex;
					if (lastColumnIndex > lastColumn)
						lastColumn = lastColumnIndex;
				}
			}
			return Rectangle.FromLTRB(columnOffset, rowOffset, lastColumn + 1, lastRow + 1);
		}
		protected void ProcessOptions() {
			quoteChar = (Options.TextQualifier != '\0') ? Options.TextQualifier : TextDocumentExporterOptionsBase.DefaultTextQualifier;
			valueSeparator = (Options.ValueSeparator != '\0') ? Options.ValueSeparator : TextDocumentExporterOptionsBase.DefaultValueSeparator;
			Encoding encoding = Encoding;
			separator = encoding.GetBytes(new string(this.valueSeparator, 1));
			quotation = encoding.GetBytes(new string(this.quoteChar, 1));
			newline = encoding.GetBytes(GetNewlineSequence(Options.NewlineType));
			nullByte = encoding.GetBytes("\0");
		}
		private bool PrepareValue(StringBuilder target, string source) {
			bool quoted = false;
			foreach (char ch in source) {
				target.Append(ch);
				if ((ch == valueSeparator) || (ch == '\r') || (ch == '\n'))
					quoted = true;
				else {
					if (ch == quoteChar) {
						target.Append(ch);
						quoted = true;
					}
				}
			}
			return quoted;
		}
		protected void ProcessValue(string value) {
			if (!string.IsNullOrEmpty(value)) {
				bool quoted = PrepareValue(ValueAccumulator, value);
				if (quoted) {
					ValueAccumulator.Append(quoteChar);
					outputStream.Write(quotation, 0, quotation.Length);
				}
				outputStream.Write(ValueAccumulator, Encoding);
				ValueAccumulator.Length = 0;
			}
		}
		protected void WriteSeparator(int columnIndex) {
			if (columnIndex > 0)
				outputStream.Write(separator, 0, separator.Length);
		}
		protected void ProcessRow(Worksheet sheet, int rowIndex, Rectangle area) {
			int columnCount = (Options.DiscardTrailingEmptyCells) ? sheet.Rows[rowIndex].LastColumnIndex - area.Left + 1 : area.Width;
			for (int columnIndex = 0; columnIndex < columnCount; ++columnIndex) {
				ICell currentCell = sheet.TryGetCell(columnIndex + area.Left, rowIndex + area.Top);
				if (currentCell != null) {
					string cellValue;
					if (currentCell.HasFormula && (Options.FormulaExportMode == FormulaExportMode.Formula))
						cellValue = GetFormulaBody(currentCell);
					else
						cellValue = GetCellTextValue(currentCell);
					WriteSeparator(columnIndex);
					ProcessValue(cellValue);
				}
				else
					WriteSeparator(columnIndex);
			}
		}
		string GetFormulaBody(ICell cell) {
			FormulaBase formula = cell.Formula;
			if (Workbook.DocumentExportOptions.CustomFunctionExportMode != CustomFunctionExportMode.CalculatedValue)
				return formula.GetBody(cell);
			FormulaBase correctedFormula = cell.GetFormulaWithoutCustomFunctions(true);
			if (correctedFormula == null)
				return GetCellTextValue(cell);
			return correctedFormula.GetBody(cell);
		}
		string GetCellTextValue(ICell cell) {
			if (Options.UseCellNumberFormat)
				return cell.Text;
			return cell.Value.ToText(Workbook.DataContext).InlineTextValue;
		}
		protected void IterateRows(Worksheet sheet, Rectangle area) {
			int rowCount = area.Height;
			for (int i = 0; i < rowCount; ++i) {
				ProcessRow(sheet, i, area);
				outputStream.Write(newline, 0, newline.Length);
			}
		}
		protected internal override void ExportSheet(Worksheet sheet) {
			if (sheet == null)
				return;
			SourceArea = DetermineSourceArea(sheet, Options.ShiftTopLeft == false );
			if (SourceArea.Height < 1)
				return;
			if (Options.WritePreamble)
				WritePreamble();
			if ((Options.DiscardTrailingEmptyCells == false ) && (SourceArea.Width < 1))
				return;
			ValueAccumulator = new StringBuilder(4096); 
			ProcessOptions();
			if (Options.NewlineAfterLastRow)
				IterateRows(sheet, SourceArea);
			else {
				int rowCount = SourceArea.Height - 1;
				IterateRows(sheet, new Rectangle(SourceArea.X, SourceArea.Y, SourceArea.Width, rowCount));
				ProcessRow(sheet, rowCount, SourceArea);
			}
			if (Options.IsNullTerminated) 
				outputStream.Write(nullByte, 0, nullByte.Length);
		}
		protected internal override void ExportSheet(Chartsheet sheet) { }
		protected internal string ReadFromStream(Stream stream, bool contain_preamble) {
			if (stream.Length < 1)
				return "";
			byte[] bytes = new byte[stream.Length]; 
			if (stream.CanSeek)
				stream.Position = 0;
			stream.Read(bytes, 0, (int)stream.Length);
			if (contain_preamble) {
				byte[] preamble = Encoding.GetPreamble();
				if (!ArrayContainBytes(preamble, bytes, 0))
					return "";
				int offset = preamble.Length;
				return Encoding.GetString(bytes, offset, bytes.Length - offset);
			}
			return Encoding.GetString(bytes, 0, bytes.Length);
		}
		protected internal bool ArrayContainBytes(byte[] what, byte[] where, int from_pos) {
			int count = what.Length;
			if (count < 1)
				return true; 
			if ((where.Length - from_pos) < count)
				return false;
			for (int i = 0; i < count; ++i) {
				if (where[i + from_pos] != what[i])
					return false;
			}
			return true;
		}
	}
}
