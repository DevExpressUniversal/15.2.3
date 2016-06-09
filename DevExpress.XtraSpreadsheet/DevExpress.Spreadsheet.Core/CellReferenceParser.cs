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
using System.Text;
using System.Text.RegularExpressions;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region CellReferencePartRC
	class CellReferencePartRC {
		PositionType type;
		int value;
		int startChar;
		bool wasError;
		public CellReferencePartRC(char startChar) {
			this.startChar = startChar;
			wasError = false;
			value = 0;
			type = PositionType.Absolute;
		}
		public PositionType Type { get { return type; } }
		public int Value { get { return value; } }
		public bool WasError { get { return wasError; } }
		public int Parse(string reference, int from) {
			int index = from;
			if (index < reference.Length && reference[index] == startChar) {
				index++;
				if (index >= reference.Length) {
					type = PositionType.Relative;
					return index;
				}
				if (reference[index] == '[') {
					index++;
					type = PositionType.Relative;
				}
				int numberStart = index;
				while (index < reference.Length && (char.IsDigit(reference[index]) || reference[index] == '-' || reference[index] == '+'))
					index++;
				if (index - numberStart > 0 && !int.TryParse(reference.Substring(numberStart, index - numberStart), out value))
					wasError = true;
				else if (type == PositionType.Relative && (index >= reference.Length || reference[index++] != ']'))
					wasError = true;
				if (index - numberStart <= 0)
					type = PositionType.Relative;
			}
			return index;
		}
	}
	#endregion
	public class CellReferenceParser {
		static CellReferenceParser instance = new CellReferenceParser();
		static Dictionary<char, int> letters = CellReferenceParserProvider.Letters;
		static Dictionary<char, int> digits = CellReferenceParserProvider.Digits;
		public static CellPosition Parse(string reference) {
			CellPosition result = instance.ParseCore(reference);
			if (!result.IsValidValue)
				throw new ArgumentException(reference, reference);
			return result;
		}
		public static CellPosition TryParse(string reference) {
			return instance.ParseCore(reference);
		}
		public static CellPosition TryParsePart(string reference) {
			return instance.ParsePartCore(reference);
		}
		public static CellPosition TryParsePartRC(string reference, int currentColumnIndex, int currentRowIndex, char r, char c) {
			return instance.ParsePartCoreRC(reference, currentColumnIndex, currentRowIndex, false, r, c);
		}
		public static bool CompareStringWithRCRef(string reference) {
			return instance.CompareStringWithRCRefCore(reference);
		}
		public static CellRange ParseIntervalRange(ICellTable sheet, string reference) {
			return instance.ParseIntervalRangeCore(sheet, reference);
		}
		public static CellPosition AssumeToCurrent(CellPosition position, int columnIndex, int rowIndex) {
			return instance.AssumeToCurrentCore(position, columnIndex, rowIndex);
		}
		public static CellPosition GetRelativeToCurrent(CellPosition position, int columnIndex, int rowIndex) {
			return instance.GetRelativeToCurrentCore(position, columnIndex, rowIndex);
		}
		public static int ParseColumnPartA1Style(string reference) {
			CellReferencePart lettersPart = new CellReferencePart(letters, 26);
			int from = lettersPart.Parse(reference, 0);
			if (from <= 0)
				return Int32.MinValue;
			lettersPart.Value--;
			if (!IndicesChecker.CheckIsColumnIndexValid(lettersPart.Value))
				return Int32.MinValue;
			return lettersPart.Value;
		}
		public static int ParseRowPartA1Style(string reference) {
			CellReferencePart digitsPart = new CellReferencePart(digits, 10);
			int from = digitsPart.Parse(reference, 0);
			if (from < reference.Length)
				return Int32.MinValue;
			digitsPart.Value--;
			if (!IndicesChecker.CheckIsRowIndexValid(digitsPart.Value))
				return Int32.MinValue;
			return digitsPart.Value;
		}
		CellPosition ParseCore(string reference) {
			CellReferencePart lettersPart = new CellReferencePart(letters, 26);
			int from = lettersPart.Parse(reference, 0);
			if (from <= 0)
				return CellPosition.InvalidValue;
			lettersPart.Value--;
			if (!IndicesChecker.CheckIsColumnIndexValid(lettersPart.Value))
				return CellPosition.InvalidValue;
			CellReferencePart digitsPart = new CellReferencePart(digits, 10);
			from = digitsPart.Parse(reference, from);
			if (from < reference.Length)
				return CellPosition.InvalidValue;
			digitsPart.Value--;
			if (!IndicesChecker.CheckIsRowIndexValid(digitsPart.Value))
				return CellPosition.InvalidValue;
			return new CellPosition(lettersPart.Value, digitsPart.Value, lettersPart.Type, digitsPart.Type);
		}
		CellPosition ParsePartCore(string reference) {
			CellReferencePart lettersPart = new CellReferencePart(letters, 26);
			bool wasLetters = true;
			int lettersEnd = lettersPart.Parse(reference, 0);
			lettersPart.Value--;
			if (lettersEnd <= 0)
				wasLetters = false;
			else
				if (!IndicesChecker.CheckIsColumnIndexValid(lettersPart.Value))
					return CellPosition.InvalidValue;
			CellReferencePart digitsPart = new CellReferencePart(digits, 10);
			int digitsEnd = digitsPart.Parse(reference, lettersEnd);
			digitsPart.Value--;
			if (digitsEnd < reference.Length || digitsEnd == lettersEnd) {
				if (!wasLetters)
					return CellPosition.InvalidValue;
			}
			else
				if (!IndicesChecker.CheckIsRowIndexValid(digitsPart.Value))
					return CellPosition.InvalidValue;
			return new CellPosition(lettersPart.Value, digitsPart.Value, lettersPart.Type, digitsPart.Type);
		}
		CellPosition ParsePartCoreRC(string reference, int currentColumnIndex, int currentRowIndex, bool ignoreLineEndings, char r, char c) {
			bool wasRow = false;
			bool wasCol = false;
			reference = reference.ToLower();
			CellReferencePartRC rowPart = new CellReferencePartRC(r);
			int rowEnd = rowPart.Parse(reference, 0);
			if (rowPart.WasError)
				return CellPosition.InvalidValue;
			wasRow = rowEnd > 0;
			CellReferencePartRC columnPart = new CellReferencePartRC(c);
			int colEnd = columnPart.Parse(reference, rowEnd);
			if (columnPart.WasError)
				return CellPosition.InvalidValue;
			wasCol = colEnd > rowEnd;
			if ((!wasRow && !wasCol) || (colEnd < reference.Length && !ignoreLineEndings))
				return CellPosition.InvalidValue;
			int column = -1;
			if (wasCol) {
				column = PrepareRelativeValue(columnPart.Value - 1, columnPart.Type, currentColumnIndex, IndicesChecker.MaxColumnCount);
				if (!IndicesChecker.CheckIsColumnIndexValid(column))
					return CellPosition.InvalidValue;
			}
			int row = -1;
			if (wasRow) {
				row = PrepareRelativeValue(rowPart.Value - 1, rowPart.Type, currentRowIndex, IndicesChecker.MaxRowCount);
				if (!IndicesChecker.CheckIsRowIndexValid(row))
					return CellPosition.InvalidValue;
			}
			return new CellPosition(column, row, columnPart.Type, rowPart.Type);
		}
		bool CompareStringWithRCRefCore(string reference) {
			bool wasRow = false;
			bool wasCol = false;
			reference = reference.ToLower();
			CellReferencePartRC rowPart = new CellReferencePartRC('r');
			int rowEnd = rowPart.Parse(reference, 0);
			if (rowPart.WasError)
				return false;
			wasRow = rowEnd > 0;
			CellReferencePartRC columnPart = new CellReferencePartRC('c');
			int colEnd = columnPart.Parse(reference, rowEnd);
			if (columnPart.WasError)
				return false;
			wasCol = colEnd > rowEnd;
			if (!wasRow && !wasCol)
				return false;
			if (colEnd < reference.Length) {
				if (reference[colEnd] == '.') 
					return false;
				if (columnPart.Value == 0 && rowPart.Value == 0)
					return false;
			}
			int column = -1;
			if (wasCol) {
				column = PrepareRelativeValue(columnPart.Value - 1, columnPart.Type, 0, IndicesChecker.MaxColumnCount);
				if (!IndicesChecker.CheckIsColumnIndexValid(column))
					return false;
			}
			int row = -1;
			if (wasRow) {
				row = PrepareRelativeValue(rowPart.Value - 1, rowPart.Type, 0, IndicesChecker.MaxRowCount);
				if (!IndicesChecker.CheckIsRowIndexValid(row))
					return false;
			}
			CellPosition result = new CellPosition(column, row, columnPart.Type, rowPart.Type);
			if (result.IsValid)
				return true;
			if (!result.IsColumnOrRow)
				return false;
			if (colEnd >= reference.Length)
				return true;
			if (result.IsRow && (result.Row != 0 || result.RowType == PositionType.Absolute))
				return true;
			if (result.IsColumn && (result.Column != 0 || result.ColumnType == PositionType.Absolute))
				return true;
			return false;
		}
		int PrepareRelativeValue(int value, PositionType type, int currentValue, int maxValue) {
			if (type == PositionType.Absolute)
				return value;
			value = value + currentValue + 1;
			if (value < 0)
				value = maxValue + value;
			else if (value >= maxValue && value - maxValue < currentValue)
				value = value - maxValue;
			return value;
		}
		#region AssumeToCurrentCore
		CellPosition AssumeToCurrentCore(CellPosition position, int currentColumnIndex, int currentRowIndex) {
			if (!position.IsValid && !position.IsColumnOrRow)
				return position;
			int column = position.Column;
			int row = position.Row;
			if (!position.IsRow)
				column = AssumeToCurrentPart(column, position.ColumnType, currentColumnIndex, IndicesChecker.MaxColumnCount);
			else
				column = -1;
			if (!position.IsColumn)
				row = AssumeToCurrentPart(row, position.RowType, currentRowIndex, IndicesChecker.MaxRowCount);
			else
				row = -1;
			return new CellPosition(column, row, position.ColumnType, position.RowType);
		}
		int AssumeToCurrentPart(int value, PositionType type, int currentValue, int maxValue) {
			if (type == PositionType.Absolute)
				return value;
			value = value - currentValue;
			if (value < 0)
				value = maxValue + value;
			return value;
		}
		#endregion
		CellPosition GetRelativeToCurrentCore(CellPosition position, int currentColumnIndex, int currentRowIndex) {
			if (!position.IsValid && !position.IsColumnOrRow)
				return position;
			int column = position.Column;
			int row = position.Row;
			if (!position.IsRow)
				column = GetRelativeToCurrentPart(column, position.ColumnType, currentColumnIndex, IndicesChecker.MaxColumnCount);
			else
				column = -1;
			if (!position.IsColumn)
				row = GetRelativeToCurrentPart(row, position.RowType, currentRowIndex, IndicesChecker.MaxRowCount);
			else
				row = -1;
			return new CellPosition(column, row, position.ColumnType, position.RowType);
		}
		int GetRelativeToCurrentPart(int value, PositionType type, int currentValue, int maxValue) {
			if (type == PositionType.Absolute)
				return value;
			else
				value = value + currentValue;
			if (value >= maxValue)
				value -= maxValue;
			return value;
		}
		CellRange ParseIntervalRangeCore(ICellTable sheet, string reference) {
			CellReferencePart lettersLeftPart = new CellReferencePart(letters, 26);
			CellReferencePart lettersRightPart = new CellReferencePart(letters, 26);
			CellReferencePart digitsLeftPart = new CellReferencePart(digits, 10);
			CellReferencePart digitsRightPart = new CellReferencePart(digits, 10);
			int from = lettersLeftPart.Parse(reference, 0);
			if (from <= 0) {
				from = digitsLeftPart.Parse(reference, 0);
				if (from <= 0) 
					return null;
				digitsLeftPart.Value--;
				if (reference[from] != ':')
					return null;
				from = digitsRightPart.Parse(reference, from + 1);
				if (from < reference.Length)
					return null;
				digitsRightPart.Value--;
				return CellIntervalRange.CreateRowInterval(sheet, digitsLeftPart.Value, digitsLeftPart.Type, digitsRightPart.Value, digitsRightPart.Type);
			}
			lettersLeftPart.Value--;
			if (reference[from] != ':')
				return null;
			from = lettersRightPart.Parse(reference, from + 1);
			if (from < reference.Length)
				return null;
			lettersRightPart.Value--;
			return CellIntervalRange.CreateColumnInterval(sheet, lettersLeftPart.Value, lettersLeftPart.Type, lettersRightPart.Value, lettersRightPart.Type);
		}
		public static string ToString(CellPosition position) {
			var builder = new StringBuilder();
			ToString(position, builder);
			return builder.ToString();
		}
		public static void ToString(CellPosition position, StringBuilder builder) {
			GetA1StringRepresentation(position, builder);
		}
		public static string ToString(CellPosition position, WorkbookDataContext context) {
			return ToString(position, context.UseR1C1ReferenceStyle, context.CurrentColumnIndex, context.CurrentRowIndex);
		}
		public static void ToString(CellPosition position, WorkbookDataContext context, StringBuilder builder) {
			ToString(position, context.UseR1C1ReferenceStyle, context.CurrentColumnIndex, context.CurrentRowIndex, builder);
		}
		public static string ToString(CellPosition position, bool useR1C1, int currentColumn, int currentRow) {
			var sb = new StringBuilder();
			ToString(position, useR1C1, currentColumn, currentRow, sb);
			return sb.ToString();
		}
		public static void ToString(CellPosition position, bool useR1C1, int currentColumn, int currentRow, StringBuilder sb) {
			if (!useR1C1)
				GetA1StringRepresentation(position, sb);
			else
				GetR1C1StringRepresentation(position, currentColumn, currentRow, sb);
		}
		internal static void GetA1StringRepresentation(CellPosition position, StringBuilder stringBuilder) {
			if (!position.IsRow) {
				int columnIndex = position.Column;
				if (!IndicesChecker.CheckIsColumnIndexValid(columnIndex))
					Exceptions.ThrowArgumentException("columnIndex", columnIndex.ToString());
				if (position.ColumnType == PositionType.Absolute)
					stringBuilder.Append('$');
				ColumnIndexToString(columnIndex, stringBuilder);
			}
			if (!position.IsColumn) {
				int rowIndex = position.Row;
				if (!IndicesChecker.CheckIsRowIndexValid(rowIndex))
					Exceptions.ThrowArgumentException("rowIndex", rowIndex.ToString());
				rowIndex++;
				if (position.RowType == PositionType.Absolute)
					stringBuilder.Append('$');
				stringBuilder.Append(rowIndex);
			}
		}
		public static void GetR1C1StringRepresentation(CellPosition position, int currentColumnIndex, int currentRowIndex, char rowLetter, char columnLetter, StringBuilder stringBuilder) {
			if (!position.IsColumn) {
				stringBuilder.Append(rowLetter);
				BuildRCStylePart(stringBuilder, position.Row, position.RowType, currentRowIndex, IndicesChecker.MaxRowCount);
			}
			if (!position.IsRow) {
				stringBuilder.Append(columnLetter);
				BuildRCStylePart(stringBuilder, position.Column, position.ColumnType, currentColumnIndex, IndicesChecker.MaxColumnCount);
			}
		}
		static void GetR1C1StringRepresentation(CellPosition position, int currentColumnIndex, int currentRowIndex, StringBuilder stringBuilder) {
			 GetR1C1StringRepresentation(position, currentColumnIndex, currentRowIndex, 'R', 'C', stringBuilder);
		}
		internal static void BuildRCStylePart(StringBuilder result, int value, PositionType type, int currentValue, int maxValue) {
			if (type == PositionType.Absolute)
				result.Append(value + 1);
			else {
				value = value - currentValue;
				if (value != 0)
					result.AppendFormat("[{0}]", value.ToString()); 
			}
		}
		public static string ColumnIndexToString(int zeroBasedColumnIndex) {
			StringBuilder stringBuilder = new StringBuilder();
			ColumnIndexToString(zeroBasedColumnIndex, stringBuilder);
			return stringBuilder.ToString();
		}
		public static void ColumnIndexToString(int columnIndex, StringBuilder stringBuilder) {
			columnIndex++;
			int lastPart = columnIndex % 26;
			if (lastPart == 0)
				lastPart = 26;
			if (columnIndex > 702) {
				int middlePart = (columnIndex - lastPart) % 676;
				if (middlePart == 0)
					middlePart = 676;
				stringBuilder.Append((char)('@' + (columnIndex - middlePart - lastPart) / 676));
				stringBuilder.Append((char)('@' + middlePart / 26));
			}
			else if (columnIndex > 26)
				stringBuilder.Append((char)('@' + (columnIndex - lastPart) / 26));
			stringBuilder.Append((char)('@' + lastPart));
		}
	}
}
