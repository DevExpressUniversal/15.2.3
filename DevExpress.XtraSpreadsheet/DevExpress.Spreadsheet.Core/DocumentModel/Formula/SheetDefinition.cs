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
using System.IO;
using System.Text;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Model.External;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region SheetDefinition
	public class SheetDefinition : ICloneable<SheetDefinition>, IEquatable<SheetDefinition> {
		#region Fields
		string sheetNameStart = string.Empty;
		string sheetNameEnd = string.Empty;
		int externalReferenceIndex = -1;
		bool validReference = true;
		#endregion
		public SheetDefinition() {
		}
		public SheetDefinition(string sheetNameStart) {
			this.sheetNameStart = sheetNameStart;
		}
		public SheetDefinition(string sheetNameStart, string sheetNameEnd) : this(sheetNameStart){
			this.sheetNameEnd = sheetNameEnd;
		}
		#region Properties
		public string SheetNameStart { get { return sheetNameStart; } set { sheetNameStart = UnQuoteSheetName(value); } }
		public string SheetNameEnd { get { return sheetNameEnd; } set { sheetNameEnd = UnQuoteSheetName(value); } }
		public int ExternalReferenceIndex { get { return externalReferenceIndex; } set { externalReferenceIndex = value; } }
		public bool Is3DReference { get { return !string.IsNullOrEmpty(sheetNameEnd) && string.Compare(sheetNameStart, sheetNameEnd) != 0; } }
		public bool IsExternalReference { get { return externalReferenceIndex > 0; } }
		public bool IsCurrentWorkbookReference { get { return !SheetDefinied && externalReferenceIndex == 0; } }
		public bool IsCurrentSheetReference { get { return !SheetDefinied && externalReferenceIndex < 0; } }
		public bool SheetDefinied { get { return !string.IsNullOrEmpty(sheetNameStart); } }
		public bool ValidReference { get { return validReference; } set { validReference = value; } }
		#endregion
		IWorksheet GetSheet(string sheetName, WorkbookDataContext context) {
			IModelWorkbook referencedWorkbook = GetWorkbook(context);
			if (referencedWorkbook == null || referencedWorkbook.SheetCount <= 0)
				return null;
			if (string.IsNullOrEmpty(sheetName))
				return referencedWorkbook.GetSheetByIndex(0);
			return referencedWorkbook.GetSheetByName(sheetName);
		}
		public IWorksheet GetSheetStart(WorkbookDataContext context) {
			return GetSheet(SheetNameStart, context);
		}
		public ExternalWorkbook GetExternalBook(WorkbookDataContext context) {
			if (externalReferenceIndex <= 0)
				return null;
			return context.Workbook.GetExternalWorkbookByIndex(externalReferenceIndex);
		}
		public IModelWorkbook GetWorkbook(WorkbookDataContext context) {
			if (externalReferenceIndex <= 0)
				return context.CurrentWorkbook;
			return GetExternalBook(context);
		}
		public List<IWorksheet> GetSheetsFor3DReference(WorkbookDataContext context) {
			if (!Is3DReference)
				return null;
			IModelWorkbook workbook = GetWorkbook(context);
			return workbook.GetSheets(sheetNameStart, sheetNameEnd);
		}
		internal List<IWorksheet> GetReferencedSheets(WorkbookDataContext context) {
			List<IWorksheet> result = null;
			if (Is3DReference)
				result = GetSheetsFor3DReference(context);
			else {
				IWorksheet sheet = null;
				if (IsCurrentSheetReference)
					sheet = context.CurrentWorksheet;
				else
					sheet = GetSheetStart(context);
				if (sheet != null) {
					result = new List<IWorksheet>();
					result.Add(sheet);
				}
			}
			return result;
		}
#if !DXPORTABLE
		string TryGetFileName(string fullPath) {
			try {
				return Path.GetFileName(fullPath);
			}
			catch (ArgumentException) {
			}
			return string.Empty;
		}
#else
		string TryGetFileName(string fullPath) {
			try {
				int index = fullPath.LastIndexOf('/');
				if(index == -1)
					index = fullPath.LastIndexOf('\\');
				if (index != -1)
					return fullPath.Substring(index + 1);
				return fullPath;
			}
			catch {
			}
			return string.Empty;
		}
#endif
		protected internal void BuildExpressionString(StringBuilder result, WorkbookDataContext context) {
			if (!validReference) {
				result.Append(CellErrorFactory.GetErrorName(ReferenceError.Instance, context));
				return;
			}
			bool is3dRerence = Is3DReference;
			int startPos = result.Length;
			bool addQuotes = false;
			bool containsSheetStart = !string.IsNullOrEmpty(sheetNameStart);
			if (IsExternalReference || externalReferenceIndex == 0) {
				if (context.ImportExportMode)
					result.AppendFormat("[{0}]", ExternalReferenceIndex);
				else {
					string externalReferenceName = string.Empty;
					if (externalReferenceIndex == 0) {
						externalReferenceName = context.Workbook.DocumentSaveOptions.CurrentFileName;
						addQuotes |= !WorkbookDataContext.IsIdent(externalReferenceName);
					}
					else {
						ExternalWorkbook externalBook = GetExternalBook(context);
						if (externalBook != null) {
							DdeExternalWorkbook ddeBook = externalBook as External.DdeExternalWorkbook;
							if (ddeBook != null)
								externalReferenceName = PrepareIdent(ddeBook.DdeServiceName, context) + "|" + PrepareIdent(ddeBook.DdeServerTopic, context);
							else {
								string fullPath = externalBook.FilePath;
								bool pathIsIdenticalToSheetName = containsSheetStart &&
																	!is3dRerence &&
																	StringExtensions.CompareInvariantCultureIgnoreCase(sheetNameStart, fullPath) == 0 &&
																	externalBook.Sheets[sheetNameStart] != null;
								if (containsSheetStart && !pathIsIdenticalToSheetName) {
									string fileName = TryGetFileName(fullPath);
									string filePath = fullPath.Substring(0, fullPath.Length - fileName.Length);
									if (!string.IsNullOrEmpty(filePath)) {
										addQuotes = true;
										externalReferenceName = QuoteSheetName(filePath);
									}
									else
										addQuotes |= !WorkbookDataContext.IsWideIdent(fileName);
									externalReferenceName += string.Format("[{0}]", QuoteSheetName(fileName));
								}
								else {
									externalReferenceName = QuoteSheetName(fullPath);
									addQuotes |= !WorkbookDataContext.IsWideIdent(fullPath);
									if (pathIsIdenticalToSheetName)
										containsSheetStart = false;
								}
							}
						}
					}
					if (!string.IsNullOrEmpty(externalReferenceName))
						result.Append(externalReferenceName);
					else
						result.AppendFormat("[{0}]", ExternalReferenceIndex);
				}
			}
			if (containsSheetStart)
				addQuotes |= WriteSheetName(result, sheetNameStart);
			if (is3dRerence) {
				result.Append(":");
				addQuotes |= WriteSheetName(result, sheetNameEnd);
			}
			if (addQuotes) {
				result.Insert(startPos, "'");
				result.Append('\'');
			}
			result.Append("!");
		}
		bool WriteSheetName(StringBuilder stringBuilder, string sheetName) {
			if (!WorkbookDataContext.IsIdent(sheetName)) {
				stringBuilder.Append(QuoteSheetName(sheetName));
				return true;
			}
			stringBuilder.Append(sheetName);
			return false;
		}
		string UnQuoteSheetName(string s) {
			return s.Replace("''", "'");
		}
		public static string QuoteSheetName(string s) {
			return s.Replace("'", "''");
		}
		public string PrepareIdent(string value, WorkbookDataContext context) {
			if (!WorkbookDataContext.IsIdent(value))
				return string.Format("'{0}'", QuoteSheetName(value));
			return value;
		}
		public static bool ShouldAddQuotes(bool is3dRerence, string nameStart, string nameEnd) {
			if (string.IsNullOrEmpty(nameStart))
				return false;
			return !WorkbookDataContext.IsIdent(nameStart) || (is3dRerence && !WorkbookDataContext.IsIdent(nameEnd));
		}
		public bool Equals(SheetDefinition anotherSheetDefinition) {
			if (externalReferenceIndex != anotherSheetDefinition.ExternalReferenceIndex)
				return false;
			if (validReference != anotherSheetDefinition.validReference)
				return false;
			if (StringExtensions.CompareInvariantCultureIgnoreCase(sheetNameStart, anotherSheetDefinition.sheetNameStart) != 0)
				return false;
			if (StringExtensions.CompareInvariantCultureIgnoreCase(sheetNameEnd, anotherSheetDefinition.sheetNameEnd) != 0) {
				if (string.IsNullOrEmpty(sheetNameEnd) && StringExtensions.CompareInvariantCultureIgnoreCase(anotherSheetDefinition.sheetNameStart, anotherSheetDefinition.sheetNameEnd) == 0)
					return true;
				if (string.IsNullOrEmpty(anotherSheetDefinition.sheetNameEnd) && StringExtensions.CompareInvariantCultureIgnoreCase(sheetNameStart, sheetNameEnd) == 0)
					return true;
				return false;
			}
			return true;
		}
		public override bool Equals(object obj) {
			return this.Equals(obj);
		}
		public override int GetHashCode() {
			return HashCodeCalculator.CalcHashCode32(sheetNameStart.GetHashCode(), sheetNameEnd.GetHashCode(), externalReferenceIndex, validReference.GetHashCode());
		}
#if DEBUGTEST
		public void ToXmlTree(System.Xml.XmlWriter writer, WorkbookDataContext context) {
			writer.WriteStartElement(GetType().Name);
			try {
				writer.WriteAttributeString("sheetNameStart", sheetNameStart);
				writer.WriteAttributeString("sheetNameEnd", sheetNameEnd);
				writer.WriteAttributeString("externalReferenceIndex", externalReferenceIndex.ToString());
			}
			finally { writer.WriteEndElement(); }
		}
#endif
		#region ICloneable<SheetDefinition> Members
		public SheetDefinition Clone() {
			SheetDefinition clone = new SheetDefinition();
			clone.sheetNameStart = sheetNameStart;
			clone.sheetNameEnd = sheetNameEnd;
			clone.externalReferenceIndex = externalReferenceIndex;
			clone.validReference = validReference;
			return clone;
		}
		#endregion
		public VariantValue AssignSheetDefinition(VariantValue value, WorkbookDataContext context) {
			if (value.IsError)
				return value;
			if (!value.IsCellRange)
				return VariantValue.ErrorReference;
			return AssignSheetDefinition(value.CellRangeValue, context);
		}
		public VariantValue AssignSheetDefinition(CellRangeBase cellRange, WorkbookDataContext context) {
			if (!ValidReference)
				return VariantValue.ErrorReference;
			VariantValue result = new VariantValue();
			result.CellRangeValue = cellRange;
			if (string.IsNullOrEmpty(sheetNameStart)) {
				if (!IsExternalReference)
					cellRange.Worksheet = context.CurrentWorksheet;
				else
					cellRange.Worksheet = null;
				return result;
			}
			List<IWorksheet> sheets = GetReferencedSheets(context);
			if (sheets == null || sheets.Count <= 0) {
				if (Is3DReference)
					return VariantValue.ErrorName;
				cellRange.Worksheet = null; 
				return result;
			}
			if (sheets.Count == 1) {
				cellRange.Worksheet = sheets[0];
				return result;
			}
			List<CellRangeBase> cellRanges = new List<CellRangeBase>();
			foreach (IWorksheet sheet in sheets) {
				CellRangeBase range = cellRange.Clone();
				range.Worksheet = sheet;
				cellRanges.Add(range);
			}
			result.CellRangeValue = new CellUnion(cellRanges);
			return result;
		}
		internal void PushSettings(WorkbookDataContext context) {
			if (!SheetDefinied) {
				if (externalReferenceIndex > 0)
					context.PushCurrentWorkbook(GetWorkbook(context), false);
				else
					context.PushCurrentWorksheet(context.CurrentWorksheet);
			}
			else
				context.PushCurrentWorksheet(GetSheetStart(context));
		}
		public string ToString(WorkbookDataContext context) {
			StringBuilder builder = new StringBuilder();
			BuildExpressionString(builder, context);
			return builder.ToString();
		}
	}
	#endregion
	public class SheetDefinitionCollection {
		#region Fields
		Dictionary<int, SheetDefinition> innerCollection;
		#endregion
		public SheetDefinitionCollection() {
			innerCollection = new Dictionary<int, SheetDefinition>();
		}
		#region Properties
		public SheetDefinition this[int index] { get { return index < 0 ? null : innerCollection[index]; } }
		public int Count { get { return innerCollection.Count; } }
		#endregion
		public int GetIndex(SheetDefinition sheetDefinition) {
			if (sheetDefinition == null)
				return -1;
			foreach (KeyValuePair<int, SheetDefinition> pair in innerCollection)
				if (pair.Value.Equals(sheetDefinition))
					return pair.Key;
			int index = innerCollection.Count;
			innerCollection.Add(index, sheetDefinition);
			return index;
		}
		public void RenameSheet(string oldName, string newName) {
			foreach (SheetDefinition sheetDefinition in this.innerCollection.Values) {
				if (sheetDefinition.IsExternalReference)
					continue;
				if (StringExtensions.CompareInvariantCultureIgnoreCase(oldName, sheetDefinition.SheetNameStart) == 0)
					sheetDefinition.SheetNameStart = newName;
				if (StringExtensions.CompareInvariantCultureIgnoreCase(oldName, sheetDefinition.SheetNameEnd) == 0)
					sheetDefinition.SheetNameEnd = newName;
			}
		}
		internal void Clear() {
			innerCollection.Clear();
		}
		public void CopyFrom(SheetDefinitionCollection sheetDefinitions) {
			innerCollection.Clear();
			foreach (KeyValuePair<int, SheetDefinition> sourceSheetDefinition in sheetDefinitions.innerCollection) {
				SheetDefinition targetSheetDefinition = sourceSheetDefinition.Value.Clone();
				innerCollection.Add(sourceSheetDefinition.Key, targetSheetDefinition);
			}
		}
		public void ShiftUpExternalLinks(int startIndex) {
			foreach (SheetDefinition sheetDefinition in innerCollection.Values) {
				int externalIndex = sheetDefinition.ExternalReferenceIndex;
				if (externalIndex < -1)
					sheetDefinition.ExternalReferenceIndex = ~sheetDefinition.ExternalReferenceIndex;
				else
					if (externalIndex >= startIndex)
						sheetDefinition.ExternalReferenceIndex += 1;
			}
		}
		public void ShiftDownExternalLinks(int startIndex) {
			foreach (SheetDefinition sheetDefinition in innerCollection.Values) {
				int externalIndex = sheetDefinition.ExternalReferenceIndex;
				if (externalIndex == startIndex)
					sheetDefinition.ExternalReferenceIndex = ~startIndex;
				else
					if (externalIndex > startIndex)
						sheetDefinition.ExternalReferenceIndex -= 1;
			}
		}
	}
}
