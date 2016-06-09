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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.XtraSpreadsheet.Model {
	public class ClipboardLinkParser {
		Model.DocumentModel model;
		string filePath;
		#region localizedRCParts
		static Dictionary<string, string> localizedRCParts = CreateLocalizedRCPartsForParser();
		public static string GetLocalizedRCLetters(System.Globalization.CultureInfo culture) {
			string localizedRC;
			if (!localizedRCParts.TryGetValue(culture.Name.ToLowerInvariant(), out localizedRC)) {
				if (!localizedRCParts.TryGetValue(culture.TwoLetterISOLanguageName.ToLowerInvariant(), out localizedRC))
					localizedRC = "rc";
			}
			return localizedRC;
		}
		static Dictionary<string, string> CreateLocalizedRCPartsForParser() {
			Dictionary<string, string> localizedRCParts = new Dictionary<string, string>();
			localizedRCParts.Add("de", "zs()");
			localizedRCParts.Add("es-es", "fc[]");
			localizedRCParts.Add("es-mx", "fc[]");
			localizedRCParts.Add("es", "lc[]");
			localizedRCParts.Add("fr", "lc[]"); 
			localizedRCParts.Add("ca", "lc[]"); 
			localizedRCParts.Add("pt", "lc[]");
			localizedRCParts.Add("nl", "rk[]");
			localizedRCParts.Add("hu", "so[]");
			localizedRCParts.Add("pl", "wk[]");
			return localizedRCParts;
		}
		public static Dictionary<string, string> LocalizedRCParts { get { return localizedRCParts; } }
		#endregion
		public ClipboardLinkParser(Model.DocumentModel model) {
			Guard.ArgumentNotNull(model, "model");
			this.model = model;
			this.filePath = String.Empty;
		}
		public string FilePath { get { return filePath; } }
		public Model.CellRange GetCellRange(string linkData) {
			string[] parts = linkData.Split(new String[] { "\0" }, StringSplitOptions.RemoveEmptyEntries);
			if (parts.Length != 3)
				return null;
			if (parts[0] != "Excel")
				return null;
			string sheetName = GetSheetName(parts[1]);
			if (String.IsNullOrEmpty(sheetName))
				return null;
			string r1c1Reference = parts[2];
			if (String.IsNullOrEmpty(r1c1Reference))
				return null;
			this.filePath = GetFilePathFromPartSecond(parts[1]);
			return GetCellRangeFromR1C1Reference(sheetName, r1c1Reference);
		}
		string GetSheetName(string text) {
			int index = text.LastIndexOf(']');
			if (index < 0 || index == text.Length - 1)
				return text;
			return text.Substring(index + 1).Trim();
		}
		string GetFilePathFromPartSecond(string text) {
			int indexBrLast = text.LastIndexOf(']');
			if (indexBrLast < 0 || indexBrLast == text.Length - 1)
				return String.Empty;
			int indexBrFist = text.IndexOf('[');
			string start = text.Substring(0, indexBrFist);
			int length = indexBrLast - indexBrFist - 1;
			if (length <= 0)
				return string.Empty;
			string middle = text.Substring(indexBrFist + 1, length);
			return start + middle;
		}
		public Model.CellRange GetCellRangeFromR1C1Reference(string sheetName, string r1c1Reference) {
			model.DataContext.PushUseR1C1(true);
			try {
				Model.Worksheet sheet = model.Sheets[sheetName];
				if (sheet == null)
					return null;
				string localizedRC = GetLocalizedRCLetters(model.DataContext.Culture);
				Model.CellRange range = ParseCellIntervalRangeR1C1_PositionTypeAbsolute(model.DataContext, sheet, r1c1Reference, localizedRC);
				if (range == null) {
					localizedRC = "rc"; 
					range = ParseCellIntervalRangeR1C1_PositionTypeAbsolute(model.DataContext, sheet, r1c1Reference, localizedRC);
				}
				return range;
			}
			finally {
				model.DataContext.PopUseR1C1();
			}
		}
		public Model.CellRange ParseCellIntervalRangeR1C1_PositionTypeAbsolute(Model.WorkbookDataContext context, Model.Worksheet sheet, string r1c1Reference, string localizedRCLetters) {
			string[] cellPositions = r1c1Reference.ToLowerInvariant().Split(':');
			char rowLetter = localizedRCLetters[0];
			char columnLetter = localizedRCLetters[1];
			Model.PositionType positionType = Model.PositionType.Absolute;
			if (cellPositions.Length == 1) {
				string single = cellPositions[0];
				Model.CellPosition position = Model.CellReferenceParser.TryParsePartRC(single, context.CurrentColumnIndex, context.CurrentRowIndex, rowLetter, columnLetter);
				if (!position.IsValid)
					return TryParseSingleRowColumnR1C1(sheet, rowLetter, columnLetter, positionType, single);
				return Model.CellRangeFactory.Default.Create(sheet, position, position);
			}
			if (cellPositions.Length != 2)
				return null;
			string start = cellPositions[0];
			string end = cellPositions[1];
			Model.CellPosition position1 = Model.CellReferenceParser.TryParsePartRC(start, context.CurrentColumnIndex, context.CurrentRowIndex, rowLetter, columnLetter);
			Model.CellPosition position2 = Model.CellReferenceParser.TryParsePartRC(end, context.CurrentColumnIndex, context.CurrentRowIndex, rowLetter, columnLetter);
			if (position1.IsValid && position2.IsValid)
				return Model.CellRangeFactory.Default.Create(sheet, position1, position2);
			return TryParseRowColumnIntervalR1C1(sheet, start, end, rowLetter, columnLetter, positionType);
		}
		Model.CellRange TryParseRowColumnIntervalR1C1(Model.Worksheet cellRangeSheet, string start, string end, char rowLetter, char columnLetter, Model.PositionType type) {
			int index;
			Model.CellRange result = null;
			if (TryParseSingle(start, rowLetter, out index)) {
				int rowStartIndex = index;
				if (TryParseSingle(end, rowLetter, out index)) {
					int rowEndIndex = index;
					result = Model.CellIntervalRange.CreateRowInterval(cellRangeSheet, rowStartIndex, type, rowEndIndex, type);
				}
				else
					return null;
			}
			else if (TryParseSingle(start, columnLetter, out index)) {
				int columnStart = index;
				if (TryParseSingle(end, columnLetter, out index)) {
					int columnEnd = index;
					result = Model.CellIntervalRange.CreateColumnInterval(cellRangeSheet, columnStart, type, columnEnd, type);
				}
				else
					return null;
			}
			if (result != null)
				result = result.TryConvertToCellInterval();
			return TryConvertToColumnInternalRange(result);
		}
		Model.CellRange TryParseSingleRowColumnR1C1(Model.Worksheet cellRangeSheet, char rowLetter, char columnLetter, Model.PositionType positionType, string reference) {
			int index;
			if (TryParseSingle(reference, rowLetter, out index))
				return Model.CellIntervalRange.CreateRowInterval(cellRangeSheet, index, positionType, index, positionType);
			if (TryParseSingle(reference, columnLetter, out index))
				return Model.CellIntervalRange.CreateColumnInterval(cellRangeSheet, index, positionType, index, positionType);
			return null;
		}
		Model.CellRange TryConvertToColumnInternalRange(Model.CellRange range) {
			if (range == null)
				return null;
			Model.CellIntervalRange intervalRange = range as Model.CellIntervalRange;
			bool isWholeWorksheetRowIntervalRange = intervalRange != null
				&& intervalRange.IsRowRangeInterval()
				&& Model.CellRange.CheckIsColumnRangeInterval(intervalRange.TopLeft, intervalRange.BottomRight, range.Worksheet as Model.Worksheet);
			Model.CellRange result = range;
			if (isWholeWorksheetRowIntervalRange)
				result = Model.CellIntervalRange.CreateAllWorksheetRange(range.Worksheet as Model.Worksheet);
			return result;
		}
		bool TryParseSingle(string reference, char letter, out int index) {
			index = 0;
			Model.CellReferencePartRC part = new Model.CellReferencePartRC(letter);
			int rowEnd = part.Parse(reference, 0);
			if (!part.WasError && rowEnd > 0) {
				index = part.Value - 1;
				return true;
			}
			return false;
		}
		public CellRange GetCellRangefromFullRCReference(string rcReference) {
			if (String.IsNullOrEmpty(rcReference))
				return null;
			int sheetNameEndIndex = rcReference.LastIndexOf("!");
			if (sheetNameEndIndex < 0)
				return null;
			string sheetName = rcReference.Substring(0, sheetNameEndIndex);
			int rcReferenceWithoutSheetNameLenght = rcReference.Length - sheetNameEndIndex - 1;
			string rcReferenceWithoutSheetName = rcReference.Substring(sheetNameEndIndex + 1, rcReferenceWithoutSheetNameLenght);
			return GetCellRangeFromR1C1Reference(sheetName, rcReferenceWithoutSheetName);
		}
	}
}
