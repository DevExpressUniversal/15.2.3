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
using DevExpress.Export.Xl;
using DevExpress.SpreadsheetSource.Implementation;
namespace DevExpress.SpreadsheetSource.Xls {
	using DevExpress.XtraExport.Xls;
	#region XlsSourceReaderStage
	public enum XlsSourceReaderStage {
		Data,
		Index
	}
	#endregion
	#region XlsSourceDataReader
	public class XlsSourceDataReader : SpreadsheetDataReaderBase {
		const int defaultCellFormatIndex = 15;
		XlsSpreadsheetSource contentBuilder;
		XlsReader workbookReader;
		IXlsSourceCommandFactory commandFactory;
		readonly List<long> dbCellPositions = new List<long>();
		readonly List<int> cellOffsets = new List<int>();
		int currentRowBlockIndex = -1;
		int firstRowInRangeIndex = -1;
		int lastRowInRangeIndex = -1;
		readonly Dictionary<int, XlsRow> rows = new Dictionary<int, XlsRow>();
		int firstColumnIndex = -1;
		int lastColumnIndex = -1;
		readonly Dictionary<int, int> numberOfHiddenColumns = new Dictionary<int, int>();
		long lastRecordPosition;
		public XlsSourceDataReader(XlsSpreadsheetSource contentBuilder)
			: base() {
			this.contentBuilder = contentBuilder;
			this.workbookReader = contentBuilder.WorkbookReader;
			this.commandFactory = contentBuilder.CommandFactory;
		}
		#region Properties
		protected internal int FirstRowIndex { get; set; }
		protected internal int LastRowIndex { get; set; }
		protected internal long DefaultColumnWidthOffset { get; set; }
		protected internal List<long> DbCellPositions { get { return dbCellPositions; } }
		protected internal long FirstRowOffset { get; set; }
		protected internal long SecondRowPosition { get; set; }
		protected internal List<int> CellOffsets { get { return cellOffsets; } }
		bool SkipEmptyRows { get { return this.contentBuilder.Options.SkipEmptyRows; } }
		bool SkipHiddenColumns { get { return this.contentBuilder.Options.SkipHiddenColumns; } }
		bool SkipHiddenRows { get { return this.contentBuilder.Options.SkipHiddenRows; } }
		protected internal XlsSourceReaderStage Stage { get; set; }
		bool HasIndexRecord { get { return DefaultColumnWidthOffset > 0; } }
		protected override List<int> NumberFormatIds { get { return contentBuilder.NumberFormatIds; } }
		protected override Dictionary<int, string> NumberFormatCodes { get { return contentBuilder.NumberFormatCodes; } }
		protected override bool UseDate1904 { get { return contentBuilder.IsDate1904; } }
		protected override int DefaultCellFormatIndex { get { return defaultCellFormatIndex; } }
		#endregion
		public override void Open(IWorksheet worksheet, XlCellRange range) {
			base.Open(worksheet, range);
			ReadBOF();
			if(ReadWorksheetIndex())
				ReadColumns();
			else
				ReadBOF();
			CalculateRowRange();
		}
		protected override bool ReadCore() {
			while(MoveToNextRow()) {
				if(ReadCells()) {
					if(!SkipEmptyRows || (ExistingCells.Count > 0))
						return true;
				}
			}
			return false;
		}
		public override void Close() {
			if(this.contentBuilder != null && this.contentBuilder.ContentType == XlsContentType.Sheet)
				this.contentBuilder.EndContent();
			this.contentBuilder = null;
			this.workbookReader = null;
			this.commandFactory = null;
			base.Close();
		}
		#region Internals
		internal void StartContent(XlsSubstreamType substreamType) {
			this.contentBuilder.StartContent(substreamType);
		}
		internal void EndContent() {
			this.contentBuilder.ClearDataCollectors();
			this.contentBuilder.EndContent();
		}
		internal void RegisterCell(int rowIndex) {
			XlsRow row;
			if(this.rows.TryGetValue(rowIndex, out row)) {
				if(row.FirstCellPosition == 0)
					row.FirstCellPosition = this.lastRecordPosition;
			}
		}
		void CalculateRowRange() {
			if(FirstRowIndex == LastRowIndex) { 
				firstRowInRangeIndex = -1;
				lastRowInRangeIndex = -1;
			}
			else if(Range == null || Range.TopLeft.IsColumn) { 
				firstRowInRangeIndex = SkipEmptyRows ? FirstRowIndex : 0;
				lastRowInRangeIndex = LastRowIndex - 1;
			}
			else { 
				firstRowInRangeIndex = SkipEmptyRows ? Math.Max(FirstRowIndex, Range.TopLeft.Row) : Range.TopLeft.Row;
				lastRowInRangeIndex = SkipEmptyRows ? Math.Min(LastRowIndex - 1, Range.BottomRight.Row) : Range.BottomRight.Row;
				if(lastRowInRangeIndex < firstRowInRangeIndex) { 
					firstRowInRangeIndex = -1;
					lastRowInRangeIndex = -1;
				}
			}
		}
		bool MoveToNextRow() {
			if(CurrentRowIndex == -1) {
				CurrentRowIndex = firstRowInRangeIndex;
				if(CurrentRowIndex == -1)
					return false;
			}
			else {
				CurrentRowIndex++;
				if(CurrentRowIndex > lastRowInRangeIndex)
					return false;
			}
			int rowBlockIndex = (CurrentRowIndex - FirstRowIndex) / 32;
			if(rowBlockIndex != currentRowBlockIndex) {
				ReadRowBlock(rowBlockIndex);
				currentRowBlockIndex = rowBlockIndex;
			}
			return true;
		}
		void ReadRowBlock(int rowBlockIndex) {
			if(!HasIndexRecord)
				return;
			FirstRowOffset = 0;
			this.rows.Clear();
			ReadDbCell(rowBlockIndex);
			if(FirstRowOffset > 0) {
				ReadRows(rowBlockIndex);
				if(this.rows.Count != cellOffsets.Count)
					DetectRowFirstCellPositions(rowBlockIndex);
			}
		}
		protected internal void AddRow(XlsRow row) {
			row.RecordIndex = this.rows.Count;
			this.rows.Add(row.Index, row);
			if(Stage == XlsSourceReaderStage.Index) {
				FirstRowIndex = Math.Min(FirstRowIndex, row.Index);
				LastRowIndex = Math.Max(LastRowIndex, row.Index + 1);
			}
		}
		bool ReadCells() {
			this.firstColumnIndex = -1;
			this.lastColumnIndex = -1;
			XlsRow row;
			if(!this.rows.TryGetValue(CurrentRowIndex, out row))
				return true;
			if(row.FirstColumnIndex == row.LastColumnIndex)
				return true;
			if(SkipHiddenRows && row.IsHidden)
				return false;
			this.firstColumnIndex = row.FirstColumnIndex;
			this.lastColumnIndex = row.LastColumnIndex - 1;
			if(Range != null && !Range.TopLeft.IsRow) {
				this.firstColumnIndex = Math.Max(this.firstColumnIndex, Range.TopLeft.Column);
				this.lastColumnIndex = Math.Min(this.lastColumnIndex, Range.BottomRight.Column);
				if(this.lastColumnIndex < this.firstColumnIndex)
					return true;
			}
			ReadCells(row);
			return true;
		}
		protected internal int TranslateColumnIndex(int columnIndex) {
			if(columnIndex < this.firstColumnIndex || columnIndex > this.lastColumnIndex)
				return -1;
			if(SkipHiddenColumns) {
				if(Range != null && !Range.TopLeft.IsRow) {
					if(columnIndex < Range.TopLeft.Column || columnIndex > Range.BottomRight.Column || Columns.IsColumnHidden(columnIndex))
						return -1;
					columnIndex -= Range.TopLeft.Column + GetHiddenColumnsCount(Range.TopLeft.Column, columnIndex);
				}
				else {
					if(Columns.IsColumnHidden(columnIndex))
						return -1;
					columnIndex -= GetHiddenColumnsCount(0, columnIndex);
				}
			}
			else if(Range != null && !Range.TopLeft.IsRow) {
				if(columnIndex < Range.TopLeft.Column || columnIndex > Range.BottomRight.Column)
					return -1;
				columnIndex -= Range.TopLeft.Column;
			}
			return columnIndex;
		}
		int GetHiddenColumnsCount(int firstColumnIndex, int lastColumnIndex) {
			if(this.numberOfHiddenColumns.ContainsKey(lastColumnIndex))
				return this.numberOfHiddenColumns[lastColumnIndex];
			int count = 0;
			for(int i = firstColumnIndex; i < lastColumnIndex; i++)
				if(Columns.IsColumnHidden(i))
					count++;
			this.numberOfHiddenColumns[lastColumnIndex] = count;
			return count;
		}
		protected internal string GetSharedString(int index) {
			return this.contentBuilder.SharedStrings[index];
		}
		#endregion
		#region Read content
		void ReadBOF() {
			XlsWorksheet sheet = Worksheet as XlsWorksheet;
			if(sheet == null)
				throw new InvalidOperationException("Can't setup worksheet substream position.");
			workbookReader.Position = sheet.StartPosition;
			XlsSourceCommandBOF command = commandFactory.CreateCommand(workbookReader) as XlsSourceCommandBOF;
			if(command == null)
				throw new InvalidFileException("Corrupted XLS file. Can't read worksheet substream BOF.");
			command.Read(workbookReader, contentBuilder);
			command.Execute(contentBuilder);
		}
		bool ReadWorksheetIndex() {
			Stage = XlsSourceReaderStage.Index;
			try {
				while(workbookReader.Position != workbookReader.StreamLength && contentBuilder.ContentType == XlsContentType.Sheet) {
					this.lastRecordPosition = workbookReader.Position;
					IXlsSourceCommand command = commandFactory.CreateCommand(workbookReader);
					command.Read(workbookReader, contentBuilder);
					command.Execute(this);
					if(HasIndexRecord)
						return true;
				}
				return false;
			}
			finally {
				Stage = XlsSourceReaderStage.Data;
			}
		}
		void ReadColumns() {
			if(!SeekToDefaultColumnWidth())
				throw new InvalidFileException("Corrupted XLS file. Can't read DefColumnWidth.");
			while(workbookReader.Position != workbookReader.StreamLength && contentBuilder.ContentType == XlsContentType.Sheet) {
				XlsSourceCommandColumnInfo command = commandFactory.CreateCommand(workbookReader) as XlsSourceCommandColumnInfo;
				if(command == null)
					return;
				command.Read(workbookReader, contentBuilder);
				command.Execute(this);
			}
		}
		bool SeekToDefaultColumnWidth() {
			XlsWorksheet sheet = Worksheet as XlsWorksheet;
			if(DefaultColumnWidthOffset < sheet.StartPosition)
				return ReadToDefaultColumnWidth(sheet.StartPosition);
			contentBuilder.WorkbookReader.Position = DefaultColumnWidthOffset;
			XlsSourceCommandDefaultColumnWidth firstCommand = commandFactory.CreateCommand(workbookReader) as XlsSourceCommandDefaultColumnWidth;
			if(firstCommand == null)
				return ReadToDefaultColumnWidth(sheet.StartPosition);
			firstCommand.Read(workbookReader, contentBuilder);
			return true;
		}
		bool ReadToDefaultColumnWidth(long startPosition) {
			contentBuilder.WorkbookReader.Position = startPosition;
			while(workbookReader.Position != workbookReader.StreamLength) {
				IXlsSourceCommand command = commandFactory.CreateCommand(workbookReader);
				command.Read(workbookReader, contentBuilder);
				if(command is XlsSourceCommandDefaultColumnWidth)
					return true;
				if(command is XlsSourceCommandEOF)
					return false;
			}
			return false;
		}
		void ReadDbCell(int rowBlockIndex) {
			if(rowBlockIndex < 0 || rowBlockIndex >= dbCellPositions.Count)
				return;
			workbookReader.Position = dbCellPositions[rowBlockIndex];
			XlsSourceCommandDbCell command = commandFactory.CreateCommand(workbookReader) as XlsSourceCommandDbCell;
			if(command == null)
				throw new InvalidFileException("Corrupted XLS file. Can't read row block index.");
			command.Read(workbookReader, contentBuilder);
			command.Execute(this);
		}
		void ReadRows(int rowBlockIndex) {
			workbookReader.Position = dbCellPositions[rowBlockIndex] - FirstRowOffset;
			while(workbookReader.Position != workbookReader.StreamLength && contentBuilder.ContentType == XlsContentType.Sheet) {
				this.lastRecordPosition = workbookReader.Position;
				XlsSourceCommandRow command = commandFactory.CreateCommand(workbookReader) as XlsSourceCommandRow;
				if(command == null) {
					workbookReader.Position = this.lastRecordPosition;
					return;
				}
				command.Read(workbookReader, contentBuilder);
				command.Execute(this);
				if(this.rows.Count == 1)
					SecondRowPosition = workbookReader.Position;
			}
		}
		void DetectRowFirstCellPositions(int rowBlockIndex) {
			Stage = XlsSourceReaderStage.Index;
			try {
				long endPosition = Math.Min(dbCellPositions[rowBlockIndex], workbookReader.StreamLength);
				while(workbookReader.Position < endPosition) {
					this.lastRecordPosition = workbookReader.Position;
					IXlsSourceCommand command = commandFactory.CreateCommand(workbookReader);
					command.Read(workbookReader, contentBuilder);
					command.Execute(this);
				}
			}
			finally {
				Stage = XlsSourceReaderStage.Data;
			}
		}
		void ReadCells(XlsRow row) {
			long endPosition = workbookReader.StreamLength;
			if(row.FirstCellPosition > 0) 
				workbookReader.Position = row.FirstCellPosition;
			else {
				if(!HasIndexRecord || this.rows.Count != cellOffsets.Count)
					return;
				workbookReader.Position = SecondRowPosition + CalculateFirstCellOffset(row);
				int rowBlockIndex = (CurrentRowIndex - FirstRowIndex) / 32;
				endPosition = Math.Min(dbCellPositions[rowBlockIndex], endPosition);
			}
			while(workbookReader.Position < endPosition && contentBuilder.ContentType == XlsContentType.Sheet) {
				IXlsSourceCommand command = commandFactory.CreateCommand(workbookReader);
				command.Read(workbookReader, contentBuilder);
				XlsSourceCommandCellBase cellCommand = command as XlsSourceCommandCellBase;
				if(cellCommand != null) {
					if(cellCommand.RowIndex != CurrentRowIndex)
						return;
					cellCommand.Execute(this);
				}
				else if(command is XlsSourceCommandString || command is XlsSourceCommandContinue)
					command.Execute(this.contentBuilder);
				else if(command.IsSubstreamBound)
					command.Execute(this.contentBuilder);
			}
		}
		int CalculateFirstCellOffset(XlsRow row) {
			int result = 0;
			for(int i = 0; i <= row.RecordIndex; i++)
				result += cellOffsets[i];
			return result;
		}
		#endregion
	}
	#endregion
	#region XlsRow
	public class XlsRow {
		public XlsRow(int rowIndex, int firstColumnIndex, int lastColumnIndex, int formatIndex, bool isHidden) {
			Index = rowIndex;
			FirstColumnIndex = firstColumnIndex;
			LastColumnIndex = lastColumnIndex;
			FormatIndex = formatIndex;
			IsHidden = isHidden;
		}
		public int Index { get; private set; }
		public int FirstColumnIndex { get; private set; }
		public int LastColumnIndex { get; private set; }
		public int FormatIndex { get; private set; }
		public bool IsHidden { get; private set; }
		internal int RecordIndex { get; set; }
		internal long FirstCellPosition { get; set; }
	}
	#endregion
}
