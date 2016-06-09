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
using System.Globalization;
using System.Linq;
using System.Text;
using DevExpress.Export.Xl;
namespace DevExpress.SpreadsheetSource {
	public interface ISpreadsheetDataReader {
		bool IsClosed { get; }
		int FieldsCount { get; }
		XlVariantValue this[int index] { get; }
		ICellCollection ExistingCells { get; }
		XlVariantValueType GetFieldType(int index);
		XlVariantValue GetValue(int index);
		bool GetBoolean(int index);
		double GetDouble(int index);
		DateTime GetDateTime(int index);
		string GetString(int index);
		string GetDisplayText(int index, CultureInfo culture);
		bool Read();
		void Close();
	}
}
namespace DevExpress.SpreadsheetSource.Implementation {
	using DevExpress.XtraExport.Csv;
	#region SpreadsheetDataReaderBase (abstract class)
	public abstract class SpreadsheetDataReaderBase : ISpreadsheetDataReader {
		#region Fields
		readonly CellCollection cells = new CellCollection();
		readonly Dictionary<int, ICell> cellsMap = new Dictionary<int, ICell>();
		int fieldsCount;
		readonly ColumnsCollection columns = new ColumnsCollection();
		XlFormatterFactory formatterFactory = null;
		#endregion
		protected SpreadsheetDataReaderBase() {
			CurrentRowIndex = -1;
		}
		#region Properties
		public bool IsClosed { get; private set; }
		public int FieldsCount { get { return fieldsCount; } }
		public XlVariantValue this[int index] { get { return GetValue(index); } }
		public ICellCollection ExistingCells { get { return cells; } }
		public IWorksheet Worksheet { get; private set; }
		public XlCellRange Range { get; private set; }
		protected internal ColumnsCollection Columns { get { return columns; } }
		protected int CurrentRowIndex { get; set; }
		protected internal XlFormatterFactory FormatterFactory {
			get {
				if (this.formatterFactory == null)
					this.formatterFactory = new XlFormatterFactory();
				return this.formatterFactory;
			}
		}
		protected abstract Dictionary<int, string> NumberFormatCodes { get; }
		protected abstract List<int> NumberFormatIds { get; }
		protected abstract bool UseDate1904 { get; }
		protected abstract int DefaultCellFormatIndex { get; }
		#endregion
		public XlVariantValueType GetFieldType(int index) {
			return GetValue(index).Type;
		}
		public XlVariantValue GetValue(int index) {
			if (index < 0 || index >= fieldsCount)
				throw new IndexOutOfRangeException();
			ICell cell;
			if (cellsMap.TryGetValue(index, out cell))
				return cell.Value;
			return XlVariantValue.Empty;
		}
		public bool GetBoolean(int index) {
			return GetValue(index).BooleanValue;
		}
		public double GetDouble(int index) {
			return GetValue(index).NumericValue;
		}
		public DateTime GetDateTime(int index) {
			return GetValue(index).GetDateTime();
		}
		public string GetString(int index) {
			return GetValue(index).ToText().TextValue;
		}
		#region GetDisplayText
		public string GetDisplayText(int index, CultureInfo culture) {
			if (index < 0 || index >= fieldsCount)
				throw new IndexOutOfRangeException();
			Cell cell = null;
			if (cellsMap.ContainsKey(index))
				cell = cellsMap[index] as Cell;
			if (cell == null)
				return string.Empty;
			return GetDisplayTextCore(cell, culture);
		}
		bool IsDateTimeNumberFormat(int formatIndex, int columnIndex) {
			int numberFormatId = GetNumberFormatId(formatIndex, DefaultCellFormatIndex);
			if (numberFormatId >= 14 && numberFormatId <= 22)
				return true;
			if (numberFormatId >= 45 && numberFormatId <= 47)
				return true;
			if (NumberFormatCodes.ContainsKey(numberFormatId)) {
				XlNumberFormat numberFormat = NumberFormatCodes[numberFormatId];
				return numberFormat.IsDateTime;
			}
			return false;
		}
		int GetNumberFormatId(int formatIndex, int defaultIndex) {
			if (formatIndex < 0 || formatIndex >= NumberFormatIds.Count)
				return NumberFormatIds[defaultIndex];
			return NumberFormatIds[formatIndex];
		}
		protected internal XlVariantValue GetDateTimeOrNumericValue(double value, int formatIndex, int columnIndex) {
			if (!IsDateTimeNumberFormat(formatIndex, columnIndex))
				return value;
			XlVariantValue result = new XlVariantValue();
			result.SetDateTimeSerial(value, UseDate1904);
			return result;
		}
		protected virtual string GetDisplayTextCore(Cell cell, CultureInfo culture) {
			IXlValueFormatter formatter = null;
			int numberFormatId = GetNumberFormatId(cell.FormatIndex, DefaultCellFormatIndex);
			if(NumberFormatCodes.ContainsKey(numberFormatId)) {
				XlNumberFormat numberFormat = NumberFormatCodes[numberFormatId];
				formatter = FormatterFactory.CreateFormatter(numberFormat);
			}
			else
				formatter = FormatterFactory.CreateFormatter(numberFormatId);
			if (formatter != null)
				return formatter.Format(cell.Value, culture);
			return cell.Value.ToText(culture).TextValue;
		}
		#endregion
		public bool Read() {
			if (IsClosed)
				return false;
			Clear();
			if (!ReadCore())
				return false;
			Prepare();
			return true;
		}
		public virtual void Open(IWorksheet worksheet, XlCellRange range) {
			Worksheet = worksheet;
			Range = range;
		}
		public virtual void Close() {
			IsClosed = true;
			Clear();
			this.columns.Clear();
		}
		protected abstract bool ReadCore();
		internal void AddCell(ICell cell) {
			cells.Add(cell);
		}
		void Clear() {
			cells.Clear();
			cellsMap.Clear();
			fieldsCount = 0;
		}
		void Prepare() {
			foreach (ICell cell in cells) {
				cellsMap.Add(cell.FieldIndex, cell);
				fieldsCount = Math.Max(fieldsCount, cell.FieldIndex + 1);
			}
		}
	}
	#endregion
}
