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

using DevExpress.Export.Xl;
using DevExpress.SpreadsheetSource.Implementation;
using DevExpress.SpreadsheetSource.Xlsx.Import;
using DevExpress.Utils;
using System;
using System.Collections.Generic;
namespace DevExpress.SpreadsheetSource.Xlsx {
	#region XlsxSourceDataReader
	public class XlsxSourceDataReader : SpreadsheetDataReaderBase {
		#region Fields
		const int defaultCellFormatIndex = 0;
		XlsxSpreadsheetSourceImporter importer;
		int fieldOffset = 0;
		int rowIndexCounter = 0;
		XlsxRowAttributes row;
		#endregion
		public XlsxSourceDataReader(XlsxSpreadsheetSourceImporter importer) {
			Guard.ArgumentNotNull(importer, "importer");
			this.importer = importer;
		}
		#region Properties
		bool SkipEmptyRows { get { return importer.Source.Options.SkipEmptyRows; } }
		bool SkipHiddenColumns { get { return importer.Source.Options.SkipHiddenColumns; } }
		bool SkipHiddenRows { get { return importer.Source.Options.SkipHiddenRows; } }
		protected override List<int> NumberFormatIds { get { return importer.Source.NumberFormatIds; } }
		protected override Dictionary<int, string> NumberFormatCodes { get { return importer.Source.NumberFormatCodes; } }
		protected override bool UseDate1904 { get { return importer.Source.UseDate1904; } }
		protected override int DefaultCellFormatIndex { get { return defaultCellFormatIndex; } }
		#endregion
		protected override bool ReadCore() {
			if (row == null) {
				if (Range == null || SkipEmptyRows)
					return false;
				return Range.LastRow >= rowIndexCounter++;
			}
			ResetFieldOffset();
			do {
				if (!SkipEmptyRows && row.Index > rowIndexCounter) {
					rowIndexCounter++;
					return true;
				}
				if (Range != null && Range.LastRow < row.Index)
					return false;
				if (Range != null && Range.FirstRow > row.Index)
					continue;
				rowIndexCounter++;
				if (SkipHiddenRows && row.IsHidden)
					continue;
				ReadCells();
				if (SkipEmptyRows && ExistingCells.Count == 0)
					continue;
				ReadNextRow();
				return true;
			} while (ReadNextRow());
			return false;
		}
		bool ReadNextRow() {
			if (!ReadToNextRow()) {
				row = null;
				CurrentRowIndex = -1;
				return false;
			}
			row = ReadCurrentRow();
			return true;
		}
		void ResetFieldOffset() {
			fieldOffset = Range == null ? 0 : Range.FirstColumn;
		}
		void ReadFirstRow() {
			if (ReadToNextRow()) {
				row = ReadCurrentRow();
				rowIndexCounter = Range == null ? 0 : Range.FirstRow;
			}
		}
		bool ReadToNextRow() {
			return importer.ReadToNextRow();
		}
		XlsxRowAttributes ReadCurrentRow() {
			XlsxRowAttributes result = importer.ReadRowAttributes();
			CurrentRowIndex = result != null ? result.Index : -1;
			return result;
		}
		void ReadCells() {
			while (ReadToNextCell()) {
				int index = ReadCurrentCellIndex();
				if (IsCellNearerDataRage(index))
					continue;
				if (IsCellFartherDataRage(index)) {
					SkipToNextRow();
					break;
				}
				ReadCurrentCell();
			}
		}
		bool ReadToNextCell() {
			return importer.ReadToNextCell();
		}
		int ReadCurrentCellIndex() {
			return importer.ReadCellIndex();
		}
		bool IsCellNearerDataRage(int index) {
			return Range != null && index < Range.FirstColumn;
		}
		bool IsCellFartherDataRage(int index) {
			return Range != null && index > Range.LastColumn;
		}
		void SkipToNextRow() {
			importer.SkipToNextRow();
		}
		void ReadCurrentCell() {
			importer.ReadCell();
		}
		public override void Open(IWorksheet worksheet, XlCellRange range) {
			base.Open(worksheet, range);
			XlsxWorksheet sheet = worksheet as XlsxWorksheet;
			if (sheet == null)
				throw new InvalidOperationException("Can't find worksheet relation");
			importer.PrepareBeforeReadSheet(sheet.RelationId);
			importer.ReadWorksheetColumns();
			ReadFirstRow();
		}
		public override void Close() {
			this.importer.CloseWorksheetReader();
			this.importer = null;
			base.Close();
		}
		internal bool CanAddColumn(int firstIndex, int lastIndex) {
			return IsColumnsFitToDataRange(firstIndex, lastIndex);
		}
		bool IsColumnsFitToDataRange(int firsColumntIndex, int lastColumnIndex) {
			if (Range == null)
				return true;
			if (firsColumntIndex >= Range.FirstColumn && firsColumntIndex <= Range.LastColumn)
				return true;
			if (lastColumnIndex >= Range.FirstColumn && lastColumnIndex <= Range.LastColumn)
				return true;
			return false;
		}
		internal bool CanAddCell(int columnIndex) {
			if (!IsColumnsFitToDataRange(columnIndex, columnIndex))
				return false;
			if (SkipHiddenColumns) {
				ColumnInfo column = Columns.FindColumn(columnIndex);
				if (column != null && column.IsHidden) {
					fieldOffset++;
					return false;
				}
			}
			return true;
		}
		internal void AddCell(int columnIndex, XlVariantValue value, int formatIndex) {
			int fieldIndex = columnIndex - fieldOffset;
			Cell cell = new Cell(fieldIndex, value, columnIndex, formatIndex);
			AddCell(cell);
		}
	}
	#endregion
	#region XlsxRowAttributes
	internal class XlsxRowAttributes {
		internal int Index { get; set; }
		internal bool IsHidden { get; set; }
		internal int StyleIndex { get; set; }
	}
	#endregion
}
