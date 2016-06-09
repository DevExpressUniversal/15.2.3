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

#if OPENDOCUMENT
using System.Diagnostics;
using DevExpress.Utils.Zip;
using DevExpress.XtraSpreadsheet.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
namespace DevExpress.XtraSpreadsheet.Export.OpenDocument {
	#region ExportContent
	public partial class OpenDocumentExporter {
		#region MergedCells export helpers
		class ExportMergedRange {
			#region Fields
			List<int> indexList;
			bool hasHostCell;
			#endregion
			public ExportMergedRange(CellRange range, int rowIndex) {
				Height = range.Height;
				Width = range.Width;
				indexList = new List<int>();
				for (int i = range.TopLeft.Column; i <= range.BottomRight.Column; ++i) {
					indexList.Add(i);
				}
				if (rowIndex == range.TopLeft.Row)
					hasHostCell = true;
			}
			#region Properties
			public int Height { get; private set; }
			public int Width { get; private set; }
			public int MaxIndex { get { return indexList[indexList.Count - 1]; } }
			#endregion
			public bool IsCovered(int cellColumnIndex) {
				List<int>.Enumerator e = indexList.GetEnumerator();
				if (hasHostCell)
					e.MoveNext();
				while (e.MoveNext())
					if (e.Current == cellColumnIndex)
						return true;
				return false;
			}
			public bool Contains(int cellColumnIndex) {
				return indexList.Contains(cellColumnIndex);
			}
			public bool IsHostCell(int cellColumnIndex) {
				if (indexList[0] == cellColumnIndex && hasHostCell)
					return true;
				return false;
			}
		}
		protected internal class MergedCellsOnRow {
			#region Fields
			List<ExportMergedRange> rangeList;
			#endregion
			public MergedCellsOnRow(Worksheet sheet, int rowIndex) {
				rangeList = new List<ExportMergedRange>();
				foreach (CellRange range in sheet.MergedCells.GetMergedCellRangesIntersectsRange(new Row(rowIndex, sheet).GetCellIntervalRange()))
					rangeList.Add(new ExportMergedRange(range, rowIndex));
			}
			#region Properties
			public int Count { get { return rangeList.Count; } }
			public int MaxColumnIndex {
				get {
					int maxIndex = -1;
					foreach (ExportMergedRange range in rangeList)
						if (range.MaxIndex > maxIndex)
							maxIndex = range.MaxIndex;
					return maxIndex;
				}
			}
			#endregion
			public bool IsCovered(int cellColumnIndex) {
				foreach (ExportMergedRange range in rangeList)
					if (range.IsCovered(cellColumnIndex))
						return true;
				return false;
			}
			public bool IsCoveredOrHost(int cellColumnIndex) {
				foreach (ExportMergedRange range in rangeList)
					if (range.IsCovered(cellColumnIndex) || range.IsHostCell(cellColumnIndex))
						return true;
				return false;
			}
			public bool IsCoveredBySameRange(int currentCellColumnIndex, int nextCellColumnIndex) {
				foreach (ExportMergedRange range in rangeList)
					if (range.IsCovered(nextCellColumnIndex))
						if (range.Contains(currentCellColumnIndex))
							return true;
						else
							return false;
				return false;
			}
			public CellPosition GetSpanIndexes(int cellColumnIndex) {
				foreach (ExportMergedRange range in rangeList)
					if (range.IsHostCell(cellColumnIndex)) {
						return new CellPosition(range.Width, range.Height);
					}
				return new CellPosition();
			}
		}
		#endregion
		#region CellTextCacheGenerator (helper class)
		class CellTextCacheGenerator {
			List<CellTextCacheParagraph> paragraphs;
			public CellTextCacheGenerator(string cellText) {
				this.paragraphs = ParseCellText(cellText);
			}
			List<CellTextCacheParagraph> ParseCellText(string cellText) {
				List<CellTextCacheParagraph> result = new List<CellTextCacheParagraph>();
				StringBuilder sb = new StringBuilder();
				CellTextCacheParagraph paragraph = new CellTextCacheParagraph();
				int i = 0;
				while (i < cellText.Length) {
					int spacesStart = i;
					while (i < cellText.Length && cellText[i] == ' ')
						++i;
					int spaceCount = i - spacesStart;
					if (spaceCount > 0)
						paragraph.Add(new CellTextCacheSpaces(spaceCount));
					for (; i < cellText.Length; ++i) {
						if ((cellText.Length > (i + 1)) && cellText[i] == '\r' && cellText[i + 1] == '\n' || cellText[i] == '\n') {
							if (sb.Length > 0) {
								paragraph.Add(new CellTextCacheText(sb.ToString()));
								sb.Clear();
							}
							result.Add(paragraph);
							paragraph = new CellTextCacheParagraph();
							i += 2;
							break;
						}
						sb.Append(cellText[i]);
						spacesStart = i;
						while (i < cellText.Length && cellText[i] == ' ')
							++i;
						spaceCount = i - spacesStart;
						if (spaceCount > 0) {
							--i;
							if (spaceCount > 1) {
								if (sb.Length > 0) {
									paragraph.Add(new CellTextCacheText(sb.ToString()));
									sb.Clear();
								}
								paragraph.Add(new CellTextCacheSpaces(spaceCount - 1));
							}
						}
					}
				}
				if (sb.Length > 0)
					paragraph.Add(new CellTextCacheText(sb.ToString()));
				result.Add(paragraph);
				return result;
			}
			public void GenerateTextCache(OpenDocumentExporter exporter) {
				foreach (CellTextCacheParagraph element in paragraphs)
					element.GenerateCacheWithTag(exporter);
			}
			public void GenerateHyperlinkCache(string uri, OpenDocumentExporter exporter) {
				exporter.WriteStartElement("p", TextNamespace);
				try {
					exporter.WriteStartElement("a", TextNamespace);
					try {
						exporter.WriteStringAttr("href", XLinkNamespace, uri);
						List<CellTextCacheParagraph>.Enumerator enumerator = paragraphs.GetEnumerator();
						if (enumerator.MoveNext()) {
							enumerator.Current.GenerateCacheWithOutTag(exporter);
							while (enumerator.MoveNext()) {
								GenerateNewLineTag(exporter);
								enumerator.Current.GenerateCacheWithOutTag(exporter);
							}
						}
					}
					finally {
						exporter.WriteEndElement();
					}
				}
				finally {
					exporter.WriteEndElement();
				}
			}
			void GenerateNewLineTag(OpenDocumentExporter exporter) {
				exporter.WriteStartElement("line-break", TextNamespace);
				exporter.WriteEndElement();
			}
			#region Paragraph
			class CellTextCacheParagraph {
				List<ICellTextCacheElement> content;
				public CellTextCacheParagraph() {
					content = new List<ICellTextCacheElement>();
				}
				public void Add(ICellTextCacheElement element) {
					content.Add(element);
				}
				public void GenerateCacheWithTag(OpenDocumentExporter exporter) {
					exporter.WriteStartElement("p", TextNamespace);
					try {
						GenerateCacheWithOutTag(exporter);
					}
					finally {
						exporter.WriteEndElement();
					}
				}
				public void GenerateCacheWithOutTag(OpenDocumentExporter exporter) {
					foreach (ICellTextCacheElement element in content)
						element.GenerateCache(exporter);
				}
			}
			#endregion
			#region Parsed Elements
			interface ICellTextCacheElement {
				void GenerateCache(OpenDocumentExporter exporter);
			}
			class CellTextCacheText : ICellTextCacheElement {
				string text;
				public CellTextCacheText(string text) {
					this.text = text;
				}
				public void GenerateCache(OpenDocumentExporter exporter) {
					exporter.WriteString(text);
				}
			}
			class CellTextCacheSpaces : ICellTextCacheElement {
				int count;
				public CellTextCacheSpaces(int count) {
					this.count = count;
				}
				public void GenerateCache(OpenDocumentExporter exporter) {
					exporter.WriteStartElement("s", TextNamespace);
					try {
						if (count > 1)
							exporter.WriteNumericAttr("c", TextNamespace, count);
					}
					finally {
						exporter.WriteEndElement();
					}
				}
			}
			#endregion
		}
		#endregion
		protected internal CompressedStream ExportContent() {
			return CreateXmlContent(GenerateContent);
		}
		void GenerateContent(XmlWriter writer) {
			DocumentContentWriter = writer;
			GenerateContent();
		}
		protected internal void GenerateContent() {
			GenerateDocumentContentStart();
			try {
				GenerateDocumentContentAttrs();
				GenerateFontFaceDecls();
				GenerateDocumentAutomaticStyles();
				GenerateDocumentBody();
			}
			finally {
				GenerateDocumentContentEnd();
			}
		}
		protected internal void GenerateDocumentContentStart() {
			DocumentContentWriter.WriteStartElement("office", "document-content", OfficeNamespace);
		}
		protected internal void GenerateDocumentContentAttrs() {
			WriteStringAttr("version", OfficeNamespace, "1.2");
			WriteNs("field", "urn:openoffice:names:experimental:ooo-ms-interop:xmlns:field:1.0");
			WriteNs("tableooo", "http://openoffice.org/2009/table");
			WriteNs("grddl", "http://www.w3.org/2003/g/data-view#");
			WriteNs("xhtml", "http://www.w3.org/1999/xhtml");
			WriteNs("of", "urn:oasis:names:tc:opendocument:xmlns:of:1.2");
			WriteNs("rpt", "http://openoffice.org/2005/report");
			WriteNs("xsi", "http://www.w3.org/2001/XMLSchema-instance");
			WriteNs("xsd", "http://www.w3.org/2001/XMLSchema");
			WriteNs("xforms", "http://www.w3.org/2002/xforms");
			WriteNs("dom", "http://www.w3.org/2001/xml-events");
			WriteNs("oooc", "http://openoffice.org/2004/calc");
			WriteNs("ooow", "http://openoffice.org/2004/writer");
			WriteNs("ooo", "http://openoffice.org/2004/office");
			WriteNs("script", "urn:oasis:names:tc:opendocument:xmlns:script:1.0");
			WriteNs("form", "urn:oasis:names:tc:opendocument:xmlns:form:1.0");
			WriteNs("math", "http://www.w3.org/1998/Math/MathML");
			WriteNs("dr3d", "urn:oasis:names:tc:opendocument:xmlns:dr3d:1.0");
			WriteNs("chart", "urn:oasis:names:tc:opendocument:xmlns:chart:1.0");
			WriteNs("svg", SvgNamespace);
			WriteNs("presentation", "urn:oasis:names:tc:opendocument:xmlns:presentation:1.0");
			WriteNs("number", NumberNamespace);
			WriteNs("meta", "urn:oasis:names:tc:opendocument:xmlns:meta:1.0");
			WriteNs("dc", "http://purl.org/dc/elements/1.1/");
			WriteNs("xlink", XLinkNamespace);
			WriteNs("fo", FoNamespace);
			WriteNs("draw", "urn:oasis:names:tc:opendocument:xmlns:drawing:1.0");
			WriteNs("table", TableNamespace);
			WriteNs("text", TextNamespace);
			WriteNs("style", StyleNamespace);
			WriteNs("office", OfficeNamespace);
		}
		protected internal void GenerateDocumentContentEnd() {
			WriteEndElement();
		}
		void GenerateDocumentBody() {
			WriteStartElement("body", OfficeNamespace);
			try {
				GenerateSpreadsheetContent();
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateSpreadsheetContent() {
			WriteStartElement("spreadsheet", OfficeNamespace);
			try {
				GenerateCalculationSettings();
				GenerateSheets();
				GenerateDefinedNames();
				GenerateTables();
			}
			finally {
				WriteEndElement();
			}
		}
		#region GenerateCalculationSettings
		protected internal void GenerateCalculationSettings() {
			CalculationOptions calculationOptions = Workbook.Properties.CalculationOptions;
			WriteStartElement("calculation-settings", TableNamespace);
			try {
				WriteBoolAttr("case-sensitive", TableNamespace, false);
				WriteBoolAttr("search-criteria-must-apply-to-whole-cell", TableNamespace, true);
				WriteBoolAttr("use-regular-expressions", TableNamespace, false);
				WriteBoolAttr("use-wildcards", TableNamespace, true);
				if (!calculationOptions.AcceptLabelsInFormulas)
					WriteBoolAttr("automatic-find-labels", TableNamespace, false);
				if (calculationOptions.PrecisionAsDisplayed)
					WriteBoolAttr("precision-as-shown", TableNamespace, true);
				GenerateCalcSettingsIteration();
			}
			finally {
				WriteEndElement();
			}
		}
		protected internal void GenerateCalcSettingsIteration() {
			CalculationOptions calcOptions = Workbook.Properties.CalculationOptions;
			double maxDiff = calcOptions.IterativeCalculationDelta;
			int steps = calcOptions.MaximumIterations;
			bool status = calcOptions.IterationsEnabled;
			if (maxDiff == 0.001 && steps == 100 && status == false)
				return;
			WriteStartElement("iteration", TableNamespace);
			try {
				if (maxDiff != 0.001)
					WriteNumericAttr("maximum-difference", TableNamespace, maxDiff);
				if (steps != 100)
					WriteNumericAttr("steps", TableNamespace, steps);
				if (status)
					WriteStringAttr("status", TableNamespace, "enable");
			}
			finally {
				WriteEndElement();
			}
		}
		protected internal void GenerateCalcSettingsNullDate() {
			WriteStartElement("null-date", TableNamespace);
			try {
				DateSystem systemStartDate = Workbook.Properties.CalculationOptions.DateSystem;
				string startDate = systemStartDate == DateSystem.Date1904 ? "1904-01-01" : "1900-01-01";
				WriteStringAttr("date-value", TableNamespace, startDate);
			}
			finally {
				WriteEndElement();
			}
		}
		#endregion
		#region GenerateSheets
		void GenerateSheets() {
			foreach (Worksheet sheet in Workbook.Sheets) {
				ExportSheet(sheet);
			}
		}
		protected internal override void ExportSheet(Worksheet sheet) {
			WriteStartElement("table", TableNamespace);
			try {
				WriteStringAttr("name", TableNamespace, sheet.Name);
				GenerateColumns(sheet);
				GenerateRows(sheet);
				GenerateDefinedNames(sheet);
			}
			finally {
				WriteEndElement();
			}
		}
		protected internal override void ExportSheet(Chartsheet sheet) {
		}
		#region GenerateColumns
		protected internal void GenerateColumns(Worksheet sheet) {
			if (sheet.Columns.Count < 1) {
				GenerateDefaultColumns(sheet, sheet.MaxColumnCount);
				return;
			}
			int defaultColumnsCount = 0;
			for (int i = 0; i < sheet.MaxColumnCount; ++i) {
				Column column = sheet.Columns.TryGetColumn(i);
				if (column != null) {
					GenerateDefaultColumns(sheet, defaultColumnsCount);
					GenerateColumn(column);
					defaultColumnsCount = 0;
					i += column.EndIndex - column.StartIndex;
				}
				else
					++defaultColumnsCount;
			}
			GenerateDefaultColumns(sheet, defaultColumnsCount);
		}
		void GenerateDefaultColumns(Worksheet sheet, int count) {
			if (count < 1)
				return;
			WriteStartElement("table-column", TableNamespace);
			try {
				WriteStringAttr("style-name", TableNamespace, exportStyleSheet.GetDefaultColumnFormatName(sheet));
				if (count > 1)
					WriteNumericAttr("number-columns-repeated", TableNamespace, count);
				WriteStringAttr("default-cell-style-name", TableNamespace, exportStyleSheet.GetCellFormatName(sheet.Workbook.StyleSheet.DefaultCellFormatIndex));
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateColumn(Column column) {
			WriteStartElement("table-column", TableNamespace);
			try {
				WriteStringAttr("style-name", TableNamespace, exportStyleSheet.GetColumnFormatName(OdsColumnFormat.GetFormat(column)));
				int columnsCount = column.EndIndex - column.StartIndex + 1;
				if (columnsCount > 1)
					WriteNumericAttr("number-columns-repeated", TableNamespace, columnsCount);
				if (column.IsHidden || OdsColumnFormat.IsHidden(column))
					WriteStringAttr("visibility", TableNamespace, "collapse");
				WriteStringAttr("default-cell-style-name", TableNamespace, exportStyleSheet.GetColumnCellStyleName(column));
			}
			finally {
				WriteEndElement();
			}
		}
		#endregion
		#region GenerateRows
		protected internal void GenerateRows(Worksheet sheet) {
			int maxMergedRangeIndex = 0;
			foreach (CellRange range in sheet.MergedCells.GetEVERYMergedRangeSLOWEnumerable())
				if (range.BottomRight.Row > maxMergedRangeIndex)
					maxMergedRangeIndex = range.BottomRight.Row;
			int maxRowIndex = sheet.Rows.Count > 0 ? sheet.Rows.Last.Index : 0;
			int lastRowIndex = Math.Max(maxRowIndex, maxMergedRangeIndex);
			Row row = sheet.Rows.TryGetRow(0);
			MergedCellsOnRow mergedRanges = new MergedCellsOnRow(sheet, 0);
			for (int i = 0; i <= lastRowIndex; ) {
				while (!RowIsEmpty(row, mergedRanges)) {
					if (row == null)
						row = new Row(i, sheet);
					GenerateRow(row, mergedRanges);
					++i;
					row = sheet.Rows.TryGetRow(i);
					mergedRanges = new MergedCellsOnRow(sheet, i);
				}
				if (i > lastRowIndex)
					break;
				i += GenerateEmptyRow(sheet, i, lastRowIndex);
				row = sheet.Rows.TryGetRow(i);
				mergedRanges = new MergedCellsOnRow(sheet, i);
			}
			lastRowIndex = sheet.MaxRowCount - lastRowIndex - 1;
			GenerateEmptyRows(sheet, lastRowIndex);
		}
		bool RowIsEmpty(Row row, MergedCellsOnRow mergedRanges) {
			if (row != null
				&& (row.Cells.Count > 0 || row.IsHidden || row.IsCustomHeight || row.Height != 0 || row.FormatIndex != row.Sheet.Workbook.StyleSheet.DefaultCellFormatIndex)
				|| mergedRanges.Count > 0
				)
				return false;
			return true;
		}
		int GenerateEmptyRow(Worksheet sheet, int rowIndex, int lastRowIndex) {
			int repeat = 1;
			for (int i = rowIndex + 1; i <= lastRowIndex; ++i) {
				Row row = sheet.Rows.TryGetRow(i);
				MergedCellsOnRow mergedRanges = new MergedCellsOnRow(sheet, i);
				if (!RowIsEmpty(row, mergedRanges)) {
					repeat = i - rowIndex;
					break;
				}
			}
			GenerateEmptyRows(sheet, repeat);
			return repeat;
		}
		void GenerateRow(Row row, MergedCellsOnRow mergedCells) {
			WriteStartElement("table-row", TableNamespace);
			try {
				WriteStringAttr("style-name", TableNamespace, exportStyleSheet.GetRowFormatName(OdsRowFormat.GetFormat(row)));
				if (row.IsHidden || OdsRowFormat.IsHidden(row))
					WriteStringAttr("visibility", TableNamespace, "collapse");
				GenerateCells(row, mergedCells);
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateEmptyRows(Worksheet sheet, int rowCount) {
			WriteStartElement("table-row", TableNamespace);
			try {
				WriteStringAttr("style-name", TableNamespace, exportStyleSheet.GetDefaultRowFormatName(sheet));
				if (rowCount > 1)
					WriteNumericAttr("number-rows-repeated", TableNamespace, rowCount);
				GenerateEmptyCells(sheet.MaxColumnCount, -1, false);
			}
			finally {
				WriteEndElement();
			}
		}
		#endregion
		#region GenerateCells
		protected internal void GenerateCells(Row row, MergedCellsOnRow mergedCells) {
			int maxCellIndex = row.Cells.Count > 0 ? row.Cells.Last.ColumnIndex : 0;
			int lastCellIndex = Math.Max(maxCellIndex, mergedCells.MaxColumnIndex);
			ICell cell = row.Cells.TryGetCell(0);
			for (int i = 0; i <= lastCellIndex; ) {
				while (cell != null) {
					GenerateCell(cell, mergedCells);
					++i;
					cell = row.Cells.TryGetCell(i);
				}
				if (i > lastCellIndex)
					break;
				i += GenerateEmptyCell(row, i, lastCellIndex, mergedCells);
				cell = row.Cells.TryGetCell(i);
			}
			lastCellIndex = row.Sheet.MaxColumnCount - lastCellIndex - 1;
			if (lastCellIndex > 0)
				GenerateEmptyCells(lastCellIndex, row.FormatIndex, row.FormatIndex != row.Sheet.Workbook.StyleSheet.DefaultCellFormatIndex);
		}
		void GenerateEmptyCells(int cellsCount, int parentCellFormatIndex, bool generateCellFormatName) {
			WriteStartElement("table-cell", TableNamespace);
			try {
				if (generateCellFormatName)
					WriteStringAttr("style-name", TableNamespace, exportStyleSheet.GetCellFormatName(parentCellFormatIndex));
				if (cellsCount > 1)
					WriteNumericAttr("number-columns-repeated", TableNamespace, cellsCount);
			}
			finally {
				WriteEndElement();
			}
		}
		int GenerateEmptyCell(Row row, int cellIndex, int lastCellIndex, MergedCellsOnRow mergedCells) {
			bool isCovered = mergedCells.IsCovered(cellIndex);
			if (isCovered)
				return GenerateCoveredEmptyCell(row, cellIndex, lastCellIndex, mergedCells);
			return GenerateNormalEmptyCell(row, cellIndex, lastCellIndex, mergedCells);
		}
		int GenerateNormalEmptyCell(Row row, int cellIndex, int lastCellIndex, MergedCellsOnRow mergedCells) {
			int repeated = 1;
			WriteStartElement("table-cell", TableNamespace);
			try {
				if (row.FormatIndex != row.Sheet.Workbook.StyleSheet.DefaultCellFormatIndex)
					WriteStringAttr("style-name", TableNamespace, exportStyleSheet.GetCellFormatName(row.FormatIndex));
				for (int i = cellIndex + 1; i <= lastCellIndex; ++i) {
					ICell cell = row.Cells.TryGetCell(i);
					bool nextCellIsMerged = mergedCells.IsCoveredOrHost(i);
					if (cell != null || nextCellIsMerged) {
						repeated = i - cellIndex;
						break;
					}
				}
				if (repeated > 1)
					WriteNumericAttr("number-columns-repeated", TableNamespace, repeated);
				WriteMergedCellAttr(cellIndex, mergedCells);
			}
			finally {
				WriteEndElement();
			}
			return repeated;
		}
		int GenerateCoveredEmptyCell(Row row, int cellIndex, int lastCellIndex, MergedCellsOnRow mergedCells) {
			if (row.Cells.TryGetCell(lastCellIndex) == null && mergedCells.IsCovered(lastCellIndex))
				++lastCellIndex;
			int repeated = 1;
			WriteStartElement("covered-table-cell", TableNamespace);
			try {
				if (row.FormatIndex != row.Sheet.Workbook.StyleSheet.DefaultCellFormatIndex)
					WriteStringAttr("style-name", TableNamespace, exportStyleSheet.GetCellFormatName(row.FormatIndex));
				for (int i = cellIndex + 1; i <= lastCellIndex; ++i) {
					ICell cell = row.Cells.TryGetCell(i);
					bool nextCellIsCovered = mergedCells.IsCoveredBySameRange(cellIndex, i);
					if (cell != null || !nextCellIsCovered) {
						repeated = i - cellIndex;
						break;
					}
				}
				if (repeated > 1)
					WriteNumericAttr("number-columns-repeated", TableNamespace, repeated);
			}
			finally {
				WriteEndElement();
			}
			return repeated;
		}
		void GenerateCell(ICell cell, MergedCellsOnRow mergedCells) {
			if (mergedCells.IsCovered(cell.ColumnIndex))
				WriteStartElement("covered-table-cell", TableNamespace);
			else
				WriteStartElement("table-cell", TableNamespace);
			try {
				string styleName = exportStyleSheet.GetCellStyleName(cell);
				if (!string.IsNullOrEmpty(styleName))
					WriteStringAttr("style-name", TableNamespace, styleName);
				CellValueOdsType valueType = GetValueType(cell);
				string valType = CellValueOdsTypeTable[valueType];
				if (!string.IsNullOrEmpty(valType))
					WriteStringAttr("value-type", OfficeNamespace, valType);
				WriteCellValue(cell, valueType);
				WriteMergedCellAttr(cell.ColumnIndex, mergedCells);
				WriteFormula(cell);
				if (valueType != CellValueOdsType.None)
					GenerateCellText(cell);
			}
			finally {
				WriteEndElement();
			}
		}
		void WriteMergedCellAttr(int cellColumnIndex, MergedCellsOnRow mergedCells) {
			CellPosition spanIndexes = mergedCells.GetSpanIndexes(cellColumnIndex);
			if (spanIndexes.Column > 1)
				WriteNumericAttr("number-columns-spanned", TableNamespace, spanIndexes.Column);
			if (spanIndexes.Row > 1)
				WriteNumericAttr("number-rows-spanned", TableNamespace, spanIndexes.Row);
		}
		CellValueOdsType GetValueType(ICell cell) {
			switch (cell.Value.Type) {
				case VariantValueType.Boolean:
					return CellValueOdsType.Boolean;
				case VariantValueType.InlineText:
				case VariantValueType.SharedString:
				case VariantValueType.Error:
					return CellValueOdsType.String;
				case VariantValueType.Numeric:
					return GetValueTypeCore(cell.Format.FormatCode.ToLowerInvariant());
				default:
					return CellValueOdsType.None;
			};
		}
		CellValueOdsType GetValueTypeCore(string numberFormatString) {
			if (numberFormatString.IndexOf('%') >= 0 && numberFormatString.IndexOfAny(new char[] { '0', '#', '?' }) >= 0)
				return CellValueOdsType.Percentage;
			if (numberFormatString.IndexOfAny(new char[] { 'y', 'd' }) >= 0)
				return CellValueOdsType.Date;
			if (numberFormatString.IndexOf('s') >= 0)
				return CellValueOdsType.Time;
			int mIndex = numberFormatString.IndexOf('m');
			int hIndex = numberFormatString.IndexOf('h');
			if (mIndex >= 0) {
				if (hIndex >= 0 && hIndex < mIndex || numberFormatString.Contains("am/pm"))
					return CellValueOdsType.Time;
				return CellValueOdsType.Date;
			}
			if (hIndex >= 0)
				return CellValueOdsType.Time;
			if (numberFormatString.IndexOf('$') >= 0)
				return CellValueOdsType.Currency;
			return CellValueOdsType.Float;
		}
		void WriteCellValue(ICell cell, CellValueOdsType valueType) {
			VariantValue value = cell.Value;
			switch (valueType) {
				case CellValueOdsType.Boolean:
					WriteBoolAttr("boolean-value", OfficeNamespace, value.BooleanValue);
					break;
				case CellValueOdsType.Date:
					DateTime date = cell.Value.ToDateTime(Workbook.DataContext);
					string dateString = string.Concat(date.Year.ToString("00"), "-", date.Month.ToString("00"), "-", date.Day.ToString("00"));
					if (value.NumericValue % 1 > 0) {
						dateString = string.Concat(dateString, "T", date.Hour.ToString("00"), ":", date.Minute.ToString("00"), ":", date.Second.ToString("00"));
						if (date.Millisecond > 0)
							dateString = string.Concat(dateString, ".", date.Millisecond);
					}
					WriteStringAttr("date-value", OfficeNamespace, dateString);
					break;
				case CellValueOdsType.Time:
					DateTime datee = cell.Value.ToDateTime(Workbook.DataContext);
					DateTime nullDate = Workbook.DataContext.DateSystem == DateSystem.Date1900 ? VariantValue.BaseDate : VariantValue.BaseDate1904;
					TimeSpan time = datee.AddDays(-1) - nullDate;
					string timeString = string.Concat("PT", ((int)time.TotalHours).ToString("00"), "H", time.Minutes.ToString("00"), "M", time.Seconds.ToString("00"));
					if (time.Milliseconds > 0)
						timeString = string.Concat(timeString, ".", time.Milliseconds);
					timeString += "S";
					WriteStringAttr("time-value", OfficeNamespace, timeString);
					break;
				case CellValueOdsType.Currency:
				case CellValueOdsType.Float:
				case CellValueOdsType.Percentage:
					WriteNumericAttr("value", OfficeNamespace, value.NumericValue);
					break;
			}
		}
		internal void GenerateCellText(ICell cell) {
			Worksheet sheet = (Worksheet)cell.Sheet;
			int linkIndex = sheet.Hyperlinks.GetHyperlink(cell);
			CellTextCacheGenerator generator = new CellTextCacheGenerator(cell.Text);
			if (linkIndex >= 0) {
				ModelHyperlink link = sheet.Hyperlinks[linkIndex];
				generator.GenerateHyperlinkCache(link.TargetUri, this);
			}
			else
				generator.GenerateTextCache(this);
		}
		void WriteFormula(ICell cell) {
			if (!cell.HasFormula)
				return;
			OdsParsedThingVisitor visitor = new OdsParsedThingVisitor(Workbook.DataContext);
			WriteStringAttr("formula", TableNamespace, string.Concat("of:=", visitor.BuildExpressionString(cell.GetFormula().Expression, cell)));
		}
		#endregion
		#endregion
		#region GenerateDefinedNames
		protected internal void GenerateDefinedNames() {
			if (Workbook.Sheets.Count < 1)
				return;
			GenerateDefinedNames(Workbook.DefinedNames);
		}
		protected internal void GenerateDefinedNames(IWorksheet sheet) {
			GenerateDefinedNames(sheet.DefinedNames);
		}
		void GenerateDefinedNames(DefinedNameCollectionBase definedNames) {
			if (definedNames.Count < 1)
				return;
			WriteStartElement("named-expressions", TableNamespace);
			try {
				foreach (DefinedNameBase definedName in definedNames)
					GenerateDefinedNameCore(definedName);
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateDefinedNameCore(DefinedNameBase definedName) {
			ParsedExpression expression = definedName.Expression;
			if (!TryGenerateNamedRange(definedName.Name, expression))
				GenerateNamedExpression(definedName.Name, expression);
		}
		bool TryGenerateNamedRange(string name, ParsedExpression expression) {
			OdsParsedThingNamedRangeVisitor visitor = new OdsParsedThingNamedRangeVisitor(Workbook.DataContext);
			string[] expr = visitor.TryBuildExpressionString(expression);
			if (expr == null)
				return false;
			string baseCellAddress = expr[1];
			string cellRangeAddress = expr[0];
			WriteStartElement("named-range", TableNamespace);
			try {
				WriteStringAttr("name", TableNamespace, name);
				WriteStringAttr("base-cell-address", TableNamespace, baseCellAddress);
				WriteStringAttr("cell-range-address", TableNamespace, cellRangeAddress);
			}
			finally {
				WriteEndElement();
			}
			return true;
		}
		void GenerateNamedExpression(string name, ParsedExpression expression) {
			WriteStartElement("named-expression", TableNamespace);
			try {
				WriteStringAttr("name", TableNamespace, name);
				string baseCellAddress = string.Concat("$", Workbook.Sheets[0].Name, ".$A$1");
				WriteStringAttr("base-cell-address", TableNamespace, baseCellAddress);
				OdsParsedThingVisitor visitor = new OdsParsedThingVisitor(Workbook.DataContext);
				string formula = string.Concat("of:=", visitor.BuildExpressionString(expression));
				WriteStringAttr("expression", TableNamespace, formula);
			}
			finally {
				WriteEndElement();
			}
		}
		#endregion
		#region GenerateTables
		protected internal void GenerateTables() {
			WriteStartElement("database-ranges", TableNamespace);
			try {
				foreach (Worksheet sheet in Workbook.Sheets)
					foreach (Table table in sheet.Tables)
						GenerateTable(table);
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateTable(Table table) {
			CellRange tableRange = table.Range;
			string targetRangeAddress = string.Concat(tableRange.Worksheet.Name, '.', tableRange.TopLeft, ':', tableRange.Worksheet.Name, '.', tableRange.BottomRight); 
			WriteStartElement("database-range", TableNamespace);
			try {
				WriteStringAttr("name", TableNamespace, table.Name);
				WriteStringAttr("target-range-address", TableNamespace, targetRangeAddress);
				if (!table.HasHeadersRow)
					WriteStringAttr("contains-header", TableNamespace, "false");
				if (table.ShowAutoFilterButton)
					WriteStringAttr("display-filter-buttons", TableNamespace, "true");
				if (table.AutoFilter.SortState.SortRange != null)
					GenerateSortState(tableRange.TopLeft.Column, table.AutoFilter.SortState);
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateSortState(int firstTableColumn, SortState sortState) {
			WriteStartElement("sort", TableNamespace);
			try {
				if (sortState.CaseSensitive)
					WriteStringAttr("case-sensitive", TableNamespace, "false");
				foreach (SortCondition condition in sortState.SortConditions)
					GenerateSortCondition(firstTableColumn, condition);
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateSortCondition(int firstTableColumn, SortCondition condition) {
			WriteStartElement("sort-by", TableNamespace);
			try {
				int columnIndex = condition.SortReference.TopLeft.Column - firstTableColumn;
				WriteNumericAttr("field-number", TableNamespace, columnIndex);
				if (condition.Descending)
					WriteStringAttr("order", TableNamespace, "descending");
			}
			finally {
				WriteEndElement();
			}
		}
		#endregion
	}
	#endregion
	#region OdsParsedThingVisitor
	public class OdsParsedThingVisitor : ParsedThingVisitor {
		delegate void OffsetToCellPositionAction();
		delegate void PushHostCellAction(OffsetToCellPositionAction action);
		#region Fields
		Stack<int> stack;
		StringBuilder builder;
		StringBuilder spacesBuilder;
		WorkbookDataContext context;
		PushHostCellAction pushHostCellAction;
		#endregion
		public OdsParsedThingVisitor(WorkbookDataContext context) {
			stack = new Stack<int>();
			builder = new StringBuilder();
			spacesBuilder = new StringBuilder();
			this.context = context;
		}
		public string BuildExpressionString(ParsedExpression expression) {
			PushHostCellAction action = delegate(OffsetToCellPositionAction x) { };
			return BuildExpressionString(expression, action);
		}
		public string BuildExpressionString(ParsedExpression expression, ICell hostCell) {
			PushHostCellAction action = delegate(OffsetToCellPositionAction x) {
				context.PushCurrentCell(hostCell);
				try {
					x();
				}
				finally {
					context.PopCurrentCell();
				}
			};
			return BuildExpressionString(expression, action);
		}
		string BuildExpressionString(ParsedExpression expression, PushHostCellAction pushHostCellAction) {
			this.pushHostCellAction = pushHostCellAction;
			this.Process(expression);
			string result = builder.ToString();
			stack.Clear();
			builder.Clear();
			spacesBuilder.Clear();
			return result;
		}
		string GetSheetName(SheetDefinition sheetDefinition) { 
			StringBuilder sb = new StringBuilder();
			sheetDefinition.BuildExpressionString(sb, context);
			return sb.Remove(sb.Length - 1, 1).ToString();
		}
		string[] GetSheetNames(SheetDefinition sheetDefinition) { 
			return GetSheetName(sheetDefinition).Split(':');
		}
		#region Binary
		public override void VisitBinary(ParsedThingBase thing) {
			thing.BuildExpressionString(stack, builder, spacesBuilder, context);
		}
		public override void Visit(ParsedThingIntersect thing) {
			Debug.Assert(stack.Count >= 2);
			builder.Insert(stack.Pop(), spacesBuilder.Append("!").ToString());
			spacesBuilder.Remove(0, spacesBuilder.Length);
		}
		public override void Visit(ParsedThingUnion thing) {
			Debug.Assert(stack.Count >= 2);
			builder.Insert(stack.Pop(), spacesBuilder.Append("~").ToString());
			spacesBuilder.Remove(0, spacesBuilder.Length);
		}
		public override void Visit(ParsedThingRange thing) {
			Debug.Assert(stack.Count >= 2);
			builder.Insert(stack.Pop(), spacesBuilder.Append(":").ToString());
			spacesBuilder.Remove(0, spacesBuilder.Length);
		}
		#endregion
		#region Unary
		public override void VisitUnary(ParsedThingBase thing) {
			thing.BuildExpressionString(stack, builder, spacesBuilder, context);
		}
		#endregion
		#region Attributes
		public override void Visit(ParsedThingAttrSpace thing) {
			thing.BuildExpressionString(stack, builder, spacesBuilder, context);
		}
		public override void Visit(ParsedThingAttrSpaceSemi thing) {
			thing.BuildExpressionString(stack, builder, spacesBuilder, context);
		}
		#endregion
		#region Operand
		public override void VisitOperand(ParsedThingBase thing) {
			thing.BuildExpressionString(stack, builder, spacesBuilder, context);
		}
		public override void Visit(ParsedThingArray thing) {
			stack.Push(builder.Length);
			builder.Append(spacesBuilder.ToString());
			spacesBuilder.Remove(0, spacesBuilder.Length);
			builder.Append('{');
			int index = 0;
			int width = thing.Width;
			int height = thing.Height;
			for (int h = 0; h < height; h++) {
				for (int w = 0; w < width; w++) {
					VariantValue value = thing.ArrayValue[index];
					string element = value.IsError ? CellErrorFactory.GetErrorName(value.ErrorValue, context) : value.ToText(context).GetTextValue(context.StringTable);
					if (thing.ArrayValue[index].IsText)
						element = "\"" + element.Replace("\"", "\"\"") + "\"";
					builder.Append(element);
					if (w != width - 1)
						builder.Append(';');
					index++;
				}
				if (h != height - 1)
					builder.Append('|');
			}
			builder.Append('}');
		}
		public override void Visit(ParsedThingRef thing) {
			stack.Push(builder.Length);
			builder.Append(spacesBuilder.ToString());
			spacesBuilder.Remove(0, spacesBuilder.Length);
			string reference = string.Concat("[.", thing.Position, "]");
			builder.Append(reference);
		}
		public override void Visit(ParsedThingRefErr thing) {
			stack.Push(builder.Length);
			builder.Append(spacesBuilder.ToString());
			spacesBuilder.Remove(0, spacesBuilder.Length);
			string reference = string.Concat("[.", CellErrorFactory.GetErrorName(ReferenceError.Instance, context), "]");
			builder.Append(reference);
		}
		public override void Visit(ParsedThingRefRel thing) {
			stack.Push(builder.Length);
			builder.Append(spacesBuilder.ToString());
			spacesBuilder.Remove(0, spacesBuilder.Length);
			CellPosition position = CellPosition.InvalidValue;
			pushHostCellAction(delegate {
				position = thing.Location.ToCellPosition(context);
			});
			string reference = string.Concat("[.", position, "]");
			builder.Append(reference);
		}
		public override void Visit(ParsedThingRef3d thing) {
			stack.Push(builder.Length);
			builder.Append(spacesBuilder.ToString());
			spacesBuilder.Remove(0, spacesBuilder.Length);
			SheetDefinition sheetDefinition = context.GetSheetDefinition(thing.SheetDefinitionIndex);
			string[] sheetNames = GetSheetNames(sheetDefinition);
			string reference;
			if (sheetDefinition.ValidReference)
				if (sheetNames.Length == 1)
					reference = string.Concat("[", sheetNames[0], ".", thing.Position, "]");
				else
					reference = string.Concat("[", sheetNames[0], ".", thing.Position, ":", sheetNames[1], ".", thing.Position, "]");
			else
				reference = sheetNames[0];
			builder.Append(reference);
		}
		public override void Visit(ParsedThingErr3d thing) {
			stack.Push(builder.Length);
			builder.Append(spacesBuilder.ToString());
			spacesBuilder.Remove(0, spacesBuilder.Length);
			SheetDefinition sheetDefinition = context.GetSheetDefinition(thing.SheetDefinitionIndex);
			string sheetName = GetSheetName(sheetDefinition);
			string reference;
			if (sheetDefinition.ValidReference)
				reference = string.Concat("[", sheetName, ".", CellErrorFactory.GetErrorName(ReferenceError.Instance, context), "]");
			else
				reference = sheetName;
			builder.Append(reference);
		}
		public override void Visit(ParsedThingRef3dRel thing) {
			stack.Push(builder.Length);
			builder.Append(spacesBuilder.ToString());
			spacesBuilder.Remove(0, spacesBuilder.Length);
			SheetDefinition sheetDefinition = context.GetSheetDefinition(thing.SheetDefinitionIndex);
			string[] sheetNames = GetSheetNames(sheetDefinition);
			CellPosition position = CellPosition.InvalidValue;
			pushHostCellAction(delegate {
				position = thing.Location.ToCellPosition(context);
			});
			string reference;
			if (sheetDefinition.ValidReference)
				if (sheetNames.Length == 1)
					reference = string.Concat("[", sheetNames[0], ".", position, "]");
				else
					reference = string.Concat("[", sheetNames[0], ".", position, ":", sheetNames[1], ".", position, "]");
			else
				reference = sheetNames[0];
			builder.Append(reference);
		}
		public override void Visit(ParsedThingArea thing) {
			stack.Push(builder.Length);
			builder.Append(spacesBuilder.ToString());
			spacesBuilder.Remove(0, spacesBuilder.Length);
			string reference = string.Concat("[.", thing.TopLeft, ":.", thing.BottomRight, "]");
			builder.Append(reference);
		}
		public override void Visit(ParsedThingAreaErr thing) {
			stack.Push(builder.Length);
			builder.Append(spacesBuilder.ToString());
			spacesBuilder.Remove(0, spacesBuilder.Length);
			string reference = string.Concat("[.", CellErrorFactory.GetErrorName(ReferenceError.Instance, context), "]");
			builder.Append(reference);
		}
		public override void Visit(ParsedThingAreaN thing) {
			stack.Push(builder.Length);
			builder.Append(spacesBuilder.ToString());
			spacesBuilder.Remove(0, spacesBuilder.Length);
			CellRange range = null;
			pushHostCellAction(delegate {
				CellPosition positionTopLeft = thing.First.ToCellPosition(context);
				CellPosition positionBottomRight = thing.Last.ToCellPosition(context);
				range = CellRange.PrepareCellRangeBaseValue(null, positionTopLeft, positionBottomRight);
			});
			string reference = string.Concat("[.", range.TopLeft, ":.", range.BottomRight, "]");
			builder.Append(reference);
		}
		public override void Visit(ParsedThingArea3d thing) {
			stack.Push(builder.Length);
			builder.Append(spacesBuilder.ToString());
			spacesBuilder.Remove(0, spacesBuilder.Length);
			SheetDefinition sheetDefinition = context.GetSheetDefinition(thing.SheetDefinitionIndex);
			string[] sheetNames = GetSheetNames(sheetDefinition);
			string reference;
			if (sheetDefinition.ValidReference)
				reference = string.Concat("[", sheetNames[0], ".", thing.TopLeft, ":", sheetNames.Length > 1 ? sheetNames[1] : string.Empty, ".", thing.BottomRight, "]");
			else
				reference = sheetNames[0];
			builder.Append(reference);
		}
		public override void Visit(ParsedThingAreaErr3d thing) {
			stack.Push(builder.Length);
			builder.Append(spacesBuilder.ToString());
			spacesBuilder.Remove(0, spacesBuilder.Length);
			SheetDefinition sheetDefinition = context.GetSheetDefinition(thing.SheetDefinitionIndex);
			string sheetName = GetSheetName(sheetDefinition);
			string reference;
			if (sheetDefinition.ValidReference)
				reference = string.Concat("[", sheetName, ".", CellErrorFactory.GetErrorName(ReferenceError.Instance, context), "]");
			else
				reference = sheetName;
			builder.Append(reference);
		}
		public override void Visit(ParsedThingArea3dRel thing) {
			stack.Push(builder.Length);
			builder.Append(spacesBuilder.ToString());
			spacesBuilder.Remove(0, spacesBuilder.Length);
			SheetDefinition sheetDefinition = context.GetSheetDefinition(thing.SheetDefinitionIndex);
			string[] sheetNames = GetSheetNames(sheetDefinition);
			CellRange range = null;
			pushHostCellAction(delegate {
				CellPosition positionTopLeft = thing.First.ToCellPosition(context);
				CellPosition positionBottomRight = thing.Last.ToCellPosition(context);
				range = CellRange.PrepareCellRangeBaseValue(null, positionTopLeft, positionBottomRight);
			});
			string reference;
			if (sheetDefinition.ValidReference)
				reference = string.Concat("[", sheetNames[0], ".", range.TopLeft, ":", sheetNames.Length > 1 ? sheetNames[1] : string.Empty, ".", range.BottomRight, "]");
			else
				reference = sheetNames[0];
			builder.Append(reference);
		}
		public override void Visit(ParsedThingTable thing) {
			IWorksheet sheet = thing.GetSheetDefinition(context).GetSheetStart(context);
			CellRange tableRange = sheet.Tables[thing.TableName].GetDataRange();
			stack.Push(builder.Length);
			builder.Append(spacesBuilder.ToString());
			string reference = string.Concat("[", sheet.Name, ".", tableRange.TopLeft, ":.", tableRange.BottomRight, "]");
			builder.Append(reference);
			spacesBuilder.Remove(0, spacesBuilder.Length);
		}
		#endregion
		#region Function
		public override void VisitFunction(ParsedThingFunc thing) {
			ISpreadsheetFunction function = FormulaCalculator.GetFunctionByCode(thing.FuncCode);
			string functionName = FormulaCalculator.GetFunctionName(function.Name, context);
			BuildParsedThingFuncExpressionStringCore(stack, function.Parameters.Count, functionName, builder, spacesBuilder.ToString(), context);
			spacesBuilder.Remove(0, spacesBuilder.Length);
		}
		public override void Visit(ParsedThingFuncVar thing) {
			ISpreadsheetFunction function = FormulaCalculator.GetFunctionByCode(thing.FuncCode);
			string functionName = FormulaCalculator.GetFunctionName(function.Name, context);
			BuildParsedThingFuncExpressionStringCore(stack, thing.ParamCount, functionName, builder, spacesBuilder.ToString(), context);
			spacesBuilder.Remove(0, spacesBuilder.Length);
		}
		public override void Visit(ParsedThingUnknownFunc thing) {
			BuildParsedThingFuncExpressionStringCore(stack, thing.ParamCount, thing.Name, builder, spacesBuilder.ToString(), context);
			spacesBuilder.Remove(0, spacesBuilder.Length);
		}
		public override void Visit(ParsedThingCustomFunc thing) {
			string localizedName = FormulaCalculator.GetFunctionName(thing.Function.Name, context);
			BuildParsedThingFuncExpressionStringCore(stack, thing.ParamCount, localizedName, builder, spacesBuilder.ToString(), context);
			spacesBuilder.Remove(0, spacesBuilder.Length);
		}
		public override void Visit(ParsedThingUnknownFuncExt thing) {
			BuildParsedThingFuncExpressionStringCore(stack, thing.ParamCount, thing.Name, builder, spacesBuilder.ToString(), context);
			spacesBuilder.Remove(0, spacesBuilder.Length);
			SheetDefinition sheetDefinition = context.GetSheetDefinition(thing.SheetDefinitionIndex);
			builder.Insert(stack.Peek(), GetSheetName(sheetDefinition));
		}
		public override void Visit(ParsedThingAddinFunc thing) {
			BuildParsedThingFuncExpressionStringCore(stack, thing.ParamCount, ParsedThingAddinFunc.ADDIN_PREFIX + thing.Name, builder, spacesBuilder.ToString(), context);
			spacesBuilder.Remove(0, spacesBuilder.Length);
		}
		void BuildParsedThingFuncExpressionStringCore(Stack<int> stack, int parametersCount, string functionName, StringBuilder builder, string spacesString, WorkbookDataContext context) {
			Debug.Assert(stack.Count >= parametersCount);
			if (parametersCount > 0) {
				for (int i = 0; i < parametersCount - 1; i++)
					builder.Insert(stack.Pop(), ";");
				int startPos = stack.Peek();
				builder.Insert(startPos, functionName + "(");
				builder.Insert(startPos, spacesString);
				builder.Append(")");
			}
			else {
				stack.Push(builder.Length);
				builder.Append(functionName);
				builder.Append("()");
			}
		}
		#endregion
	}
	#endregion
	#region OdsParsedThingNamedRangeVisitor
	public class OdsParsedThingNamedRangeVisitor : ParsedThingVisitor {
		#region Fields
		Stack<int> stack;
		StringBuilder builder;
		StringBuilder spacesBuilder;
		WorkbookDataContext context;
		IWorksheet baseCellSheet;
		#endregion
		public OdsParsedThingNamedRangeVisitor(WorkbookDataContext context) {
			stack = new Stack<int>();
			builder = new StringBuilder();
			spacesBuilder = new StringBuilder();
			this.context = context;
		}
		public string[] TryBuildExpressionString(ParsedExpression expression) {
			this.baseCellSheet = context.Workbook.Sheets.Last;
			try {
				this.Process(expression);
			}
			catch {
				baseCellSheet = null;
				builder.Clear();
				return null;
			}
			finally {
				stack.Clear();
				spacesBuilder.Clear();
			}
			string baseCellAddress = string.Concat("$", baseCellSheet.Name, ".$A$1");
			string cellRangeAddress = builder.ToString();
			baseCellSheet = null;
			builder.Clear();
			return new string[] { cellRangeAddress, baseCellAddress };
		}
		void CheckBaseCellSheet(SheetDefinition sheetDefinition) {
			IWorksheet sheet = sheetDefinition.GetSheetStart(context);
			if (context.Workbook.Sheets.GetIndexById(baseCellSheet.SheetId) > context.Workbook.Sheets.GetIndexById(sheet.SheetId))
				baseCellSheet = sheet;
		}
		string[] GetSheetNames(SheetDefinition sheetDefinition) { 
			StringBuilder sb = new StringBuilder();
			sheetDefinition.BuildExpressionString(sb, context);
			return sb.Remove(sb.Length - 1, 1).ToString().Split(':');
		}
		#region Binary
		public override void VisitBinary(ParsedThingBase thing) {
			throw new Exception("Non named-range operation");
		}
		public override void Visit(ParsedThingRange thing) {
			Debug.Assert(stack.Count >= 2);
			builder.Insert(stack.Pop(), spacesBuilder.Append(":").ToString());
			spacesBuilder.Remove(0, spacesBuilder.Length);
		}
		#endregion
		#region Unary
		public override void VisitUnary(ParsedThingBase thing) {
			throw new Exception("Non named-range operation");
		}
		#endregion
		#region Attributes
		public override void Visit(ParsedThingAttrSpace thing) {
			thing.BuildExpressionString(stack, builder, spacesBuilder, context);
		}
		public override void Visit(ParsedThingAttrSpaceSemi thing) {
			thing.BuildExpressionString(stack, builder, spacesBuilder, context);
		}
		#endregion
		#region Operand
		public override void VisitOperand(ParsedThingBase thing) {
			throw new Exception("Non named-range operation");
		}
		public override void Visit(ParsedThingRef thing) {
			stack.Push(builder.Length);
			builder.Append(spacesBuilder.ToString());
			spacesBuilder.Remove(0, spacesBuilder.Length);
			string reference = string.Concat(".", thing.Position);
			builder.Append(reference);
		}
		public override void Visit(ParsedThingRefRel thing) {
			stack.Push(builder.Length);
			builder.Append(spacesBuilder.ToString());
			spacesBuilder.Remove(0, spacesBuilder.Length);
			CellPosition position = thing.Location.ToCellPosition(context);
			string reference = string.Concat(".", position);
			builder.Append(reference);
		}
		public override void Visit(ParsedThingRef3d thing) {
			stack.Push(builder.Length);
			builder.Append(spacesBuilder.ToString());
			spacesBuilder.Remove(0, spacesBuilder.Length);
			SheetDefinition sheetDefinition = context.GetSheetDefinition(thing.SheetDefinitionIndex);
			string[] sheetNames = GetSheetNames(sheetDefinition);
			string reference;
			if (sheetDefinition.ValidReference)
				if (sheetNames.Length == 1)
					reference = string.Concat(sheetNames[0], ".", thing.Position);
				else
					reference = string.Concat(sheetNames[0], ".", thing.Position, ":", sheetNames[1], ".", thing.Position);
			else
				reference = sheetNames[0];
			builder.Append(reference);
			CheckBaseCellSheet(sheetDefinition);
		}
		public override void Visit(ParsedThingRef3dRel thing) {
			stack.Push(builder.Length);
			builder.Append(spacesBuilder.ToString());
			spacesBuilder.Remove(0, spacesBuilder.Length);
			SheetDefinition sheetDefinition = context.GetSheetDefinition(thing.SheetDefinitionIndex);
			string[] sheetNames = GetSheetNames(sheetDefinition);
			CellPosition position = thing.Location.ToCellPosition(context);
			string reference;
			if (sheetDefinition.ValidReference)
				if (sheetNames.Length == 1)
					reference = string.Concat(sheetNames[0], ".", position);
				else
					reference = string.Concat(sheetNames[0], ".", position, ":", sheetNames[1], ".", position);
			else
				reference = sheetNames[0];
			builder.Append(reference);
			CheckBaseCellSheet(sheetDefinition);
		}
		public override void Visit(ParsedThingArea thing) {
			stack.Push(builder.Length);
			builder.Append(spacesBuilder.ToString());
			spacesBuilder.Remove(0, spacesBuilder.Length);
			string reference = string.Concat(".", thing.TopLeft, ":.", thing.BottomRight);
			builder.Append(reference);
		}
		public override void Visit(ParsedThingAreaN thing) {
			stack.Push(builder.Length);
			builder.Append(spacesBuilder.ToString());
			spacesBuilder.Remove(0, spacesBuilder.Length);
			CellPosition positionTopLeft = thing.First.ToCellPosition(context);
			CellPosition positionBottomRight = thing.Last.ToCellPosition(context);
			CellRange range = CellRange.PrepareCellRangeBaseValue(null, positionTopLeft, positionBottomRight);
			string reference = string.Concat(".", range.TopLeft, ":.", range.BottomRight);
			builder.Append(reference);
		}
		public override void Visit(ParsedThingArea3d thing) {
			stack.Push(builder.Length);
			builder.Append(spacesBuilder.ToString());
			spacesBuilder.Remove(0, spacesBuilder.Length);
			SheetDefinition sheetDefinition = context.GetSheetDefinition(thing.SheetDefinitionIndex);
			string[] sheetNames = GetSheetNames(sheetDefinition);
			string reference;
			if (sheetDefinition.ValidReference)
				reference = string.Concat(sheetNames[0], ".", thing.TopLeft, ":", sheetNames.Length > 1 ? sheetNames[1] : string.Empty, ".", thing.BottomRight);
			else
				reference = sheetNames[0];
			builder.Append(reference);
			CheckBaseCellSheet(sheetDefinition);
		}
		public override void Visit(ParsedThingArea3dRel thing) {
			stack.Push(builder.Length);
			builder.Append(spacesBuilder.ToString());
			spacesBuilder.Remove(0, spacesBuilder.Length);
			SheetDefinition sheetDefinition = context.GetSheetDefinition(thing.SheetDefinitionIndex);
			string[] sheetNames = GetSheetNames(sheetDefinition);
			CellPosition positionTopLeft = thing.First.ToCellPosition(context);
			CellPosition positionBottomRight = thing.Last.ToCellPosition(context);
			CellRange range = CellRange.PrepareCellRangeBaseValue(null, positionTopLeft, positionBottomRight);
			string reference;
			if (sheetDefinition.ValidReference)
				reference = string.Concat(sheetNames[0], ".", range.TopLeft, ":", sheetNames.Length > 1 ? sheetNames[1] : string.Empty, ".", range.BottomRight);
			else
				reference = sheetNames[0];
			builder.Append(reference);
			CheckBaseCellSheet(sheetDefinition);
		}
		#endregion
		#region Function
		public override void VisitFunction(ParsedThingFunc thing) {
			throw new Exception("Non named-range operation");
		}
		#endregion
	}
	#endregion
}
#endif
