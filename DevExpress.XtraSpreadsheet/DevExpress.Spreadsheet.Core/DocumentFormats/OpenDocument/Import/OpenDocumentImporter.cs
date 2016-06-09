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
using DevExpress.Office;
using DevExpress.Office.Services;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Export.OpenDocument;
using DevExpress.XtraSpreadsheet.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using DevExpress.Office.Model;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Import.OpenDocument {
	#region OpenDocumentWorkbookImporter
	public class OpenDocumentWorkbookImporter : OpenDocumentImporterBase {
		#region Fields
		OpenDocumentImporterOptions options;
		CellFormat defaultCellFormat;
		CellStyleFormat defaultCellStyle;
		OdsColumnFormat defaultColumnFormat;
		OdsRowFormat defaultRowFormat;
		OdsTableFormat defaultTableFormat;
		OdsConditionalFormattingImportCollection conditionalFormattings = new OdsConditionalFormattingImportCollection();
		Dictionary<int, string> defaultColumnCellFormatTable = new Dictionary<int, string>() { { 0, null } };
		Dictionary<string, int> cellFormatTable = new Dictionary<string, int>();
		Dictionary<string, OdsColumnFormat> columnFormatTable = new Dictionary<string, OdsColumnFormat>();
		Dictionary<string, OdsRowFormat> rowFormatTable = new Dictionary<string, OdsRowFormat>();
		Dictionary<string, OdsTableFormat> tableFormatTable = new Dictionary<string, OdsTableFormat>();
		Dictionary<string, string> formatStringTable = new Dictionary<string, string>();
		Dictionary<string, string> fontStyleTable = new Dictionary<string, string>();
		#endregion
		#region Properties
		public new DocumentModel DocumentModel { get { return (DocumentModel)base.DocumentModel; } }
		public Worksheet CurrentSheet { get; set; }
		public OpenDocumentImporterOptions Options { get { return options; } }
		public int IndexNextCell { get; set; }
		public int IndexNextColumn { get; set; }
		public int IndexNextRow { get; set; }
		public int ColumnOutlineLevel { get; set; }
		public int RowOutlineLevel { get; set; }
		public string CurrentRowDefaultCellStyleName { get; set; }
		internal CellFormat DefaultCellFormat { get { return defaultCellFormat; } }
		internal OdsColumnFormat DefaultColumnFormat { get { return defaultColumnFormat; } }
		internal OdsRowFormat DefaultRowFormat { get { return defaultRowFormat; } }
		internal OdsTableFormat DefaultTableFormat { get { return defaultTableFormat; } }
		internal OdsConditionalFormattingImportCollection ConditionalFormattings { get { return conditionalFormattings; } }
		internal Dictionary<string, int> CellFormatTable { get { return cellFormatTable; } }
		internal Dictionary<string, OdsColumnFormat> ColumnFormatTable { get { return columnFormatTable; } }
		internal Dictionary<string, OdsRowFormat> RowFormatTable { get { return rowFormatTable; } }
		internal Dictionary<string, OdsTableFormat> TableFormatTable { get { return tableFormatTable; } }
		internal Dictionary<string, string> FormatStringTable { get { return formatStringTable; } }
		internal Dictionary<string, string> FontStyleTable { get { return formatStringTable; } }
		#endregion
		public OpenDocumentWorkbookImporter(IDocumentModel documentModel, OpenDocumentImporterOptions options)
			: base(documentModel) {
			this.options = options;
			CreateDefaultFormats();
		}
		void CreateDefaultFormats() {
			defaultCellFormat = new CellFormat(DocumentModel);
			defaultCellStyle = new CellStyleFormat(DocumentModel);
			defaultColumnFormat = new OdsColumnFormat();
			defaultRowFormat = new OdsRowFormat();
			defaultTableFormat = new OdsTableFormat();
		}
		public void Import(Stream stream) {
			OpenPackage(stream);
			string contentFileName = "content.xml";
			XmlReader contentReader = GetPackageFileXmlReader(contentFileName);
			if (contentReader != null) {
				if (!ReadToRootElement(contentReader, "document-content", OfficeOpenDocumentHelper.OfficeNamespace))
					return;
				ImportMainDocument(contentReader, stream);
			}
		}
		#region Implementation
		public override string ReadAttribute(XmlReader reader, string attributeName) {
			return reader.GetAttribute(attributeName);
		}
		public override string ReadAttribute(XmlReader reader, string attributeName, string ns) {
			return reader.GetAttribute(attributeName, ns);
		}
		public override void BeginSetMainDocumentContent() {
			ILogService logService = DocumentModel.GetService<ILogService>();
			if (logService != null)
				logService.Clear();
			DocumentModel.BeginSetContent();
		}
		public override void EndSetMainDocumentContent() {
			conditionalFormattings.Apply(this);
			DocumentModel.PrepareFormulas();
			DocumentModel.EndSetContent(DocumentModelChangeType.LoadNewDocument);
		}
		public override void SetMainDocumentEmptyContent() {
			if (DocumentModel.IsUpdateLocked && DocumentModel.DeferredChanges.IsSetContentMode)
				DocumentModel.SetMainDocumentEmptyContentCore();
			else {
				DocumentModel.BeginSetContent();
				try {
					DocumentModel.SetMainDocumentEmptyContentCore();
				}
				finally {
					DocumentModel.EndSetContent(DocumentModelChangeType.LoadNewDocument);
				}
			}
		}
		protected override Destination CreateMainDocumentDestination() {
			return new DocumentContentDestination(this);
		}
		protected override Destination CreateRootStyleDestination() {
			return new DocumentStylesDestination(this);
		}
		#endregion
		public Worksheet CreateWorksheet(string name) {
			IndexNextColumn = 0;
			IndexNextRow = 0;
			defaultColumnCellFormatTable = new Dictionary<int, string>() { { 0, null } };
			Worksheet result = DocumentModel.CreateWorksheet(name);
			CurrentSheet = result;
			return result;
		}
		protected internal string GetAttribute(XmlReader reader, string attributeName, string defaultValue) {
			string value = ReadAttribute(reader, attributeName);
			return value == null ? defaultValue : value;
		}
		protected internal bool GetBoolean(XmlReader reader, string attributeName, bool defaultValue) {
			string value = ReadAttribute(reader, attributeName);
			if (string.IsNullOrEmpty(value))
				return defaultValue;
			if (value.Equals("true", StringComparison.OrdinalIgnoreCase))
				return true;
			if (value.Equals("false", StringComparison.OrdinalIgnoreCase))
				return false;
			return defaultValue;
		}
		protected internal float GetAngle(XmlReader reader, string attributeName, int defaultValue) {
			string value = reader.GetAttribute(attributeName);
			return ParseAngle(value, defaultValue);
		}
		protected internal float ParseAngle(string value, int defaultValue) {
			if (string.IsNullOrEmpty(value))
				return defaultValue;
			value = value.ToLower();
			DocumentModelUnitConverter converter = DocumentModel.UnitConverter;
			float angleInCustomUnits;
			if (float.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out angleInCustomUnits))
				return converter.DegreeToModelUnits(angleInCustomUnits);
			string customUnit = value.Substring(value.Length - 3);
			string magnitude = value.Remove(value.Length - 3, 3);
			if (float.TryParse(magnitude, NumberStyles.Number, CultureInfo.InvariantCulture, out angleInCustomUnits)) {
				if (customUnit.Equals("rad"))
					angleInCustomUnits = (float)(angleInCustomUnits * 180 / Math.PI);
				return converter.DegreeToModelUnits(angleInCustomUnits);
			}
			return defaultValue;
		}
		protected internal float GetLength(XmlReader reader, string attributeName, float defaultValue) {
			string value = reader.GetAttribute(attributeName);
			return ParseLength(value, defaultValue);
		}
		protected internal float ParseLength(string value, float defaultValue) {
			if (string.IsNullOrEmpty(value))
				return defaultValue;
			value = value.ToLower();
			DocumentModelUnitConverter converter = DocumentModel.UnitConverter;
			string customUnit = value.Substring(value.Length - 2);
			string magnitude = value.Remove(value.Length - 2, 2);
			float lengthInCustomUnits;
			if (float.TryParse(magnitude, NumberStyles.Number, CultureInfo.InvariantCulture, out lengthInCustomUnits))
				switch (customUnit) {
					case "in":
						return converter.InchesToModelUnitsF(lengthInCustomUnits);
					case "pt":
						return converter.PointsToModelUnitsF(lengthInCustomUnits);
					case "cm":
						return converter.CentimetersToModelUnitsF(lengthInCustomUnits);
					case "mm":
						return converter.MillimetersToModelUnitsF(lengthInCustomUnits);
					case "pc":
						return converter.PicasToModelUnitsF(lengthInCustomUnits);
					default:
						return defaultValue;
				}
			if (value.Equals("thin"))
				return converter.PointsToModelUnitsF(1);
			if (value.Equals("thick"))
				return converter.PointsToModelUnitsF(3);
			if (value.Equals("medium"))
				return converter.PointsToModelUnitsF(2);
			return defaultValue;
		}
		protected internal Color GetColor(XmlReader reader, string attributeName, Color defaultValue) {
			string value = reader.GetAttribute(attributeName);
			return ParseColor(value, defaultValue);
		}
		protected internal Color ParseColor(string value, Color defaultValue) { 
			if (string.IsNullOrEmpty(value))
				return defaultValue;
			value = value.Substring(1, value.Length - 1);
			uint hexColor;
			if (value.Length == 6 && UInt32.TryParse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out hexColor)) {
				hexColor += 0xFF000000;
				return DXColor.FromArgb((int)hexColor);
			}
			return defaultValue;
		}
		public CellRange GetSpannedRange(XmlReader reader, CellPosition topLeft, bool isFormulaRange) {
			int rowsSpanned;
			int columnsSpanned;
			if (isFormulaRange) {
				rowsSpanned = GetWpSTIntegerValue(reader, "table:number-matrix-rows-spanned", -1);
				columnsSpanned = GetWpSTIntegerValue(reader, "table:number-matrix-columns-spanned", -1);
			}
			else {
				rowsSpanned = GetWpSTIntegerValue(reader, "table:number-rows-spanned", 1);
				columnsSpanned = GetWpSTIntegerValue(reader, "table:number-columns-spanned", 1);
			}
			if (rowsSpanned < 0)
				return null;
			CellPosition botRight = new CellPosition(topLeft.Column + columnsSpanned - 1, topLeft.Row + rowsSpanned - 1);
			return new CellRange(CurrentSheet, topLeft, botRight);
		}
		#region Formula
		public void CopyFormula(ICell cell, byte[] binaryFormula) {
			cell.FormulaInfo = new FormulaInfo();
			cell.FormulaInfo.BinaryFormula = binaryFormula;
		}
		public void SetFormula(ICell cell, string formula, CellRange formulaRange) {
			if (string.IsNullOrEmpty(formula))
				return;
			formula = NormalizeOdsFormula(formula);
			if (formulaRange == null) {
				Formula cellFormula = new Formula(cell);
				cellFormula.SetBodyTemporarily(formula, cell);
				cell.ApplyFormulaCore(cellFormula);
				return;
			}
			cell.Worksheet.CreateArrayCore(formula, formulaRange);
		}
		protected internal string NormalizeOdsFormula(string formula) {
			return NormalizeOdsFormula(formula, string.Empty);
		}
		protected internal string NormalizeOdsFormula(string formula, string baseSheetName) {
			if (string.IsNullOrEmpty(formula))
				return formula;
			if (formula.Length > 3 && formula.Substring(0, 3) == "of:")
				formula = formula.Remove(0, 3);
			else
				if (formula.Length > 5 && formula.Substring(0, 5) == "oooc:")
					formula = formula.Remove(0, 5);
			if (formula[0] != '=') {
				formula = string.Concat("=", formula);
			}
			formula = formula.Replace(';', ','); 
			Regex regex = new Regex(@"(?<cellUnion>\[[^\]]*\]([~!]\[[^\]]*\])+)");
			formula = regex.Replace(formula, "(${cellUnion})");
			string pattern = @"(?<separator>[=(,~!])" + 
							 @"(?<spaces>\s*)" +
							 @"\[?" +
							 @"(?<sheet1>\$?((\'[^']*\')|(\w*)))" +
							 @"\." +
							 @"(?<ref1>\$?\w*\$?\d*)" +
							 @"(\:" +
							 @"(?<sheet2>\$?((\'[^']*\')|(\w*)))" +
							 @"\." +
							 @"(?<ref2>\$?\w*\$?\d*))?" +
							 @"\]?";
			regex = new Regex(pattern);
			formula = regex.Replace(formula, delegate(Match m) {
				string[] sheetNames = new string[] { m.Groups["sheet1"].Value, m.Groups["sheet2"].Value };
				string[] refs = new string[] { m.Groups["ref1"].Value, m.Groups["ref2"].Value };
				string sheetToken = string.Empty;
				string referenceToken = string.Empty;
				for (int i = 0; i < 2; ++i) {
					string sheetName = sheetNames[i];
					if (!string.IsNullOrEmpty(sheetName) && sheetName[0] == '$')
						sheetName = sheetName.Remove(0, 1);
					else
						if (!string.IsNullOrEmpty(baseSheetName) && baseSheetName.Equals(sheetName.Trim('\''))) {
							string anotherSheetName = sheetNames[(i + 1) % 2];
							if (anotherSheetName.Equals(baseSheetName) || string.IsNullOrEmpty(anotherSheetName))
								sheetName = string.Empty;
						}
					sheetToken = string.Concat(sheetToken, sheetName, ":");
					string reference = refs[i];
					if (!string.IsNullOrEmpty(reference))
						referenceToken = string.Concat(referenceToken, reference, ":");
				}
				sheetToken = sheetToken.Trim(':');
				if (!string.IsNullOrEmpty(baseSheetName))
					sheetToken = string.Concat(sheetToken, "!");
				else
					if (!string.IsNullOrEmpty(sheetToken))
						sheetToken = string.Concat(sheetToken, "!");
				if (!string.IsNullOrEmpty(referenceToken))
					referenceToken = referenceToken.Remove(referenceToken.Length - 1);
				string separator = m.Groups["separator"].Value;
				if (separator.Equals("~"))
					separator = ",";
				else if (separator.Equals("!"))
					separator = " ";
				return string.Concat(separator, m.Groups["spaces"].Value, sheetToken, referenceToken);
			});
			regex = new Regex(@"\{.*\}");
			formula = regex.Replace(formula, delegate(Match m) {
				Regex regex2 = new Regex("(?<value>[^;|]+)(?<separator>[;|]?)");
				return regex2.Replace(m.Value, delegate(Match mm) {
					string separator = mm.Groups["separator"].Value;
					if (separator == "|")
						separator = ";";
					return mm.Groups["value"].Value + separator;
				});
			});
			return formula;
		}
		public CellRange GetCellRangeByCellRangeAddress(string cellRangeAddress) {
			CellRangeBase cellRangeBase = CellRangeBase.TryParse(NormalizeOdsFormula(cellRangeAddress), DocumentModel.DataContext);
			if (cellRangeBase == null || cellRangeBase.RangeType == CellRangeType.UnionRange)
				ThrowInvalidFile();
			return cellRangeBase as CellRange;
		}
		#endregion
		#region Styles
		#region CreateFormat
		public CellFormat CreateCellFormat(string parentStyleName) {
			CellFormat result = new CellFormat(DocumentModel);
			if (!string.IsNullOrEmpty(parentStyleName)) {
				int parentCellFormatIndex;
				if (cellFormatTable.TryGetValue(parentStyleName, out parentCellFormatIndex))
					DocumentModel.Cache.CellFormatCache[parentCellFormatIndex].CloneCore(result);
				else {
					CellStyleBase parentStyle = DocumentModel.StyleSheet.CellStyles.GetCellStyleByName(parentStyleName);
					if (parentStyle != null) {
						result.ApplyFormat(parentStyle.FormatInfo);
						result.AssignStyleIndex(parentStyle.StyleIndex);
						return result;
					}
					else
						defaultCellFormat.CloneCore(result);
				}
			}
			else
				defaultCellFormat.CloneCore(result);
			return result;
		}
		public CellStyleFormat CreateCellStyleFormat(string parentStyleName) {
			CellStyleFormat result = new CellStyleFormat(DocumentModel);
			if (!string.IsNullOrEmpty(parentStyleName)) {
				CellStyleBase parentStyle = DocumentModel.StyleSheet.CellStyles.GetCellStyleByName(parentStyleName);
				if (parentStyle != null)
					parentStyle.FormatInfo.CloneCore(result);
				else
					defaultCellStyle.CloneCore(result);
			}
			else
				defaultCellStyle.CloneCore(result);
			return result;
		}
		public OdsColumnFormat CreateColumnFormat(string parentStyleName) {
			OdsColumnFormat parentFormat;
			if (!string.IsNullOrEmpty(parentStyleName) && columnFormatTable.TryGetValue(parentStyleName, out parentFormat))
				return parentFormat.Clone();
			return defaultColumnFormat.Clone();
		}
		public OdsRowFormat CreateRowFormat(string parentStyleName) {
			OdsRowFormat parentFormat;
			if (!string.IsNullOrEmpty(parentStyleName) && rowFormatTable.TryGetValue(parentStyleName, out parentFormat))
				return parentFormat.Clone();
			return defaultRowFormat.Clone();
		}
		public OdsTableFormat CreateTableFormat(string parentStyleName) {
			OdsTableFormat parentFormat;
			if (!string.IsNullOrEmpty(parentStyleName) && tableFormatTable.TryGetValue(parentStyleName, out parentFormat))
				return parentFormat.Clone();
			return defaultTableFormat.Clone();
		}
		#endregion
		#region RegisterDefaultFormat
		public void RegisterDefaultCellFormat(CellFormat format) {
			DocumentModel.StyleSheet.DefaultCellFormatIndex = DocumentModel.Cache.CellFormatCache.GetItemIndex(format);
			this.defaultCellFormat = format;
			this.defaultCellStyle = FormatToStyleFormat(format);
		}
		CellStyleFormat FormatToStyleFormat(CellFormat format) {
			CellStyleFormat result = new CellStyleFormat(DocumentModel);
			result.AssignAlignmentIndex(format.AlignmentIndex);
			result.AssignBorderIndex(format.BorderIndex);
			result.AssignCellFormatFlagsIndex(format.CellFormatFlagsIndex);
			result.AssignFillIndex(format.FillIndex);
			result.AssignFontIndex(format.FontIndex);
			result.AssignGradientFillInfoIndex(format.GradientFillInfoIndex);
			result.AssignNumberFormatIndex(format.NumberFormatIndex);
			return result;
		}
		public void RegisterDefaultColumnFormat(OdsColumnFormat format) {
			this.defaultColumnFormat = format;
		}
		public void RegisterDefaultRowFormat(OdsRowFormat format) {
			this.defaultRowFormat = format;
		}
		public void RegisterDefaultTableFormat(OdsTableFormat format) {
			this.defaultTableFormat = format;
		}
		public void RegisterDefaultColumnCellFormat(string styleName, Column column) {
			if (defaultColumnCellFormatTable.ContainsKey(column.StartIndex))
				defaultColumnCellFormatTable[column.StartIndex] = styleName;
			else
				defaultColumnCellFormatTable.Add(column.StartIndex, styleName);
			defaultColumnCellFormatTable.Add(column.EndIndex + 1, null);
		}
		#endregion
		#region GetFormat
		public bool TryGetFontName(string fontStyleName, out string fontName) {
			return TryGetFormat(fontStyleTable, fontStyleName, out fontName);
		}
		public bool TryGetFormatString(string dataStyleName, out string formatString) {
			return TryGetFormat(formatStringTable, dataStyleName, out formatString);
		}
		bool TryGetFormat<T>(Dictionary<string, T> dict, string styleName, out T style) {
			if (string.IsNullOrEmpty(styleName)) {
				style = default(T);
				return false;
			}
			return dict.TryGetValue(styleName, out style);
		}
		int TryGetCellFormatBaseIndex(string styleName) {
			int cellFormatIndex;
			if (string.IsNullOrEmpty(styleName) || !cellFormatTable.TryGetValue(styleName, out cellFormatIndex)) {
				cellFormatIndex = -1;
				CellStyleBase style = DocumentModel.StyleSheet.CellStyles.GetCellStyleByName(styleName);
				if (style != null) {
					CellFormat format = new CellFormat(DocumentModel);
					format.ApplyFormat(style.FormatInfo);
					format.AssignStyleIndex(style.StyleIndex);
					return DocumentModel.Cache.CellFormatCache.GetItemIndex(format);
				}
			}
			return cellFormatIndex;
		}
		#endregion
		#region RegisterStyles
		public int RegisterColor(Color color) {
			ColorModelInfo.Create(color);
			ColorModelInfo colorInfo = new ColorModelInfo();
			colorInfo.Rgb = color;
			return DocumentModel.Cache.ColorModelInfoCache.GetItemIndex(colorInfo);
		}
		public void RegisterCellFormat(string styleName, string dataStyleName, string parentStyleName, CellFormat format) {
			AssignNumberFormat(format, dataStyleName);
			int index = DocumentModel.Cache.CellFormatCache.GetItemIndex(format);
			RegisterStyle(cellFormatTable, styleName, index);
		}
		public void RegisterCellStyle(string styleName, string dataStyleName, CellStyleFormat format) {
			if (styleName.Equals("Default"))
				styleName = "Normal";
			AssignNumberFormat(format, dataStyleName);
			CellStyleBase style = DocumentModel.StyleSheet.CellStyles.GetCellStyleByName(styleName);
			if (style != null) {
				int index = DocumentModel.Cache.CellFormatCache.GetItemIndex(format);
				style.AssignCellStyleFormatIndex(index);
			}
			else {
				style = new CustomCellStyle(DocumentModel, styleName, format);
				DocumentModel.StyleSheet.CellStyles.Add(style);
			}
		}
		void AssignNumberFormat(CellFormatBase format, string dataStyleName) {
			string formatString;
			if (!TryGetFormatString(dataStyleName, out formatString))
				return;
			NumberFormat info = new NumberFormat(0, formatString);
			int numberFormatIndex = DocumentModel.Cache.NumberFormatCache.GetItemIndex(info);
			format.AssignNumberFormatIndex(numberFormatIndex);
			format.ApplyNumberFormat = true;
		}
		public void RegisterFormatString(string dataStyleName, string formatString) {
			RegisterStyle(formatStringTable, dataStyleName, formatString);
		}
		public void RegisterColumnFormat(string styleName, OdsColumnFormat format) {
			RegisterStyle(columnFormatTable, styleName, format);
		}
		public void RegisterRowFormat(string styleName, OdsRowFormat format) {
			RegisterStyle(rowFormatTable, styleName, format);
		}
		public void RegisterTableFormat(string styleName, OdsTableFormat format) {
			RegisterStyle(tableFormatTable, styleName, format);
		}
		public void RegisterFontName(string styleName, string fontName) {
			fontName = fontName.Trim('"', '\'');
			RegisterStyle(fontStyleTable, styleName, fontName);
		}
		public void RegisterStyle<T>(Dictionary<string, T> dict, string styleName, T format) {
			if (!dict.ContainsKey(styleName))
				dict.Add(styleName, format);
		}
		#endregion
		#region ApplyFormat
		public ICell TryCreateCellAndApplyFormat(CellValueOdsType valueType, string styleName, RowDestination parentDestination) {
			int cellFormatIndex = TryGetCellFormatBaseIndex(styleName);
			if (cellFormatIndex < 0) {
				styleName = CurrentRowDefaultCellStyleName;
				cellFormatIndex = TryGetCellFormatBaseIndex(styleName);
				if (cellFormatIndex < 0) {
					styleName = GetCurrentColumnDefaultCellStyleName();
					cellFormatIndex = TryGetCellFormatBaseIndex(styleName);
					if (cellFormatIndex < 0)
						if (valueType == CellValueOdsType.None)
							return null;
				}
			}
			ICell cell = parentDestination.CurrentRow.Cells.GetCell(IndexNextCell);
			if (cellFormatIndex >= 0)
				cell.SetCellFormatIndex(cellFormatIndex);
			conditionalFormattings.AssignRange(styleName, cell.GetRange());
			return cell;
		}
		string GetCurrentColumnDefaultCellStyleName() {
			int lastKey = 0;
			foreach (int item in defaultColumnCellFormatTable.Keys) {
				if (item > IndexNextCell)
					break;
				lastKey = item;
			}
			return defaultColumnCellFormatTable[lastKey];
		}
		public void ApplyColumnFormatOnColumn(Column column, string styleName, bool isHidden) {
			column.IsHidden = isHidden;
			column.OutlineLevel = ColumnOutlineLevel;
			OdsColumnFormat format;
			if (string.IsNullOrEmpty(styleName) || !columnFormatTable.TryGetValue(styleName, out format))
				if (defaultColumnFormat == null)
					return;
				else
					format = defaultColumnFormat;
			format.ApplyFormatOnColumn(column);
		}
		public void ApplyRowFormatOnRow(RowDestination row, string styleName, bool isHidden) {
			if (isHidden)
				row.CurrentRow.IsHidden = isHidden;
			if (RowOutlineLevel > 0)
				row.CurrentRow.OutlineLevel = RowOutlineLevel;
			OdsRowFormat format;
			if (string.IsNullOrEmpty(styleName) || !rowFormatTable.TryGetValue(styleName, out format) || format.IsNull)
				if (defaultRowFormat == null || defaultRowFormat.IsNull)
					return;
				else
					format = defaultRowFormat;
			format.ApplyFormatOnRow(row.CurrentRow);
		}
		public void ApplyTableFormatOnTable(Worksheet sheet, string styleName) {
			OdsTableFormat format;
			if (string.IsNullOrEmpty(styleName) || !tableFormatTable.TryGetValue(styleName, out format))
				if (defaultTableFormat == null)
					return;
				else
					format = defaultTableFormat;
			format.ApplyFormatOnSheet(sheet);
		}
		#endregion
		public int GetDifferentialFormatIndex(CellFormat cellFormat, string applyStyleName) {
			int applyFormatIndex = TryGetCellFormatBaseIndex(applyStyleName);
			if (applyFormatIndex < 0)
				return -1;
			CellFormatBase applyFormat = (CellFormatBase)DocumentModel.Cache.CellFormatCache[applyFormatIndex];
			DifferentialFormat differentialFormat = new DifferentialFormat(DocumentModel);
			MultiOptionsInfo options = new MultiOptionsInfo();
			differentialFormat.AssignAlignmentIndex(applyFormat.AlignmentIndex);
			if (applyFormat.ApplyAlignment) {
				CellAlignmentInfo cellAlignment = cellFormat.AlignmentInfo;
				CellAlignmentInfo applyAlignment = applyFormat.AlignmentInfo;
				options.ApplyAlignmentHorizontal = cellAlignment.HorizontalAlignment != applyAlignment.HorizontalAlignment;
				options.ApplyAlignmentIndent = cellAlignment.Indent != applyAlignment.Indent;
				options.ApplyAlignmentJustifyLastLine = cellAlignment.JustifyLastLine != applyAlignment.JustifyLastLine;
				options.ApplyAlignmentReadingOrder = cellAlignment.ReadingOrder != applyAlignment.ReadingOrder;
				options.ApplyAlignmentRelativeIndent = cellAlignment.RelativeIndent != applyAlignment.RelativeIndent;
				options.ApplyAlignmentShrinkToFit = cellAlignment.ShrinkToFit != applyAlignment.ShrinkToFit;
				options.ApplyAlignmentTextRotation = cellAlignment.TextRotation != applyAlignment.TextRotation;
				options.ApplyAlignmentVertical = cellAlignment.VerticalAlignment != applyAlignment.VerticalAlignment;
				options.ApplyAlignmentWrapText = cellAlignment.WrapText != applyAlignment.WrapText;
			}
			differentialFormat.AssignFillIndex(applyFormat.FillIndex);
			if (applyFormat.ApplyFill) {
				FillInfo cellFill = cellFormat.FillInfo;
				FillInfo applyFill = applyFormat.FillInfo;
				options.ApplyFillBackColor = cellFill.BackColorIndex != applyFill.BackColorIndex;
				options.ApplyFillForeColor = cellFill.ForeColorIndex != applyFill.ForeColorIndex;
				options.ApplyFillPatternType = cellFill.PatternType != applyFill.PatternType;
			}
			differentialFormat.AssignFontIndex(applyFormat.FontIndex);
			if (applyFormat.ApplyFont) {
				RunFontInfo cellFont = cellFormat.FontInfo;
				RunFontInfo applyFont = applyFormat.FontInfo;
				options.ApplyFontBold = cellFont.Bold != applyFont.Bold;
				options.ApplyFontCharset = cellFont.Charset != applyFont.Charset;
				options.ApplyFontColor = cellFont.ColorIndex != applyFont.ColorIndex;
				options.ApplyFontCondense = cellFont.Condense != applyFont.Condense;
				options.ApplyFontExtend = cellFont.Extend != applyFont.Extend;
				options.ApplyFontFamily = cellFont.FontFamily != applyFont.FontFamily;
				options.ApplyFontItalic = cellFont.Italic != applyFont.Italic;
				options.ApplyFontName = cellFont.Name != applyFont.Name;
				options.ApplyFontOutline = cellFont.Outline != applyFont.Outline;
				options.ApplyFontSchemeStyle = cellFont.SchemeStyle != applyFont.SchemeStyle;
				options.ApplyFontScript = cellFont.Script != applyFont.Script;
				options.ApplyFontShadow = cellFont.Shadow != applyFont.Shadow;
				options.ApplyFontSize = cellFont.Size != applyFont.Size;
				options.ApplyFontStrikeThrough = cellFont.StrikeThrough != applyFont.StrikeThrough;
				options.ApplyFontUnderline = cellFont.Underline != applyFont.Underline;
			}
			differentialFormat.AssignNumberFormatIndex(applyFormat.NumberFormatIndex);
			if (applyFormat.ApplyNumberFormat) {
				NumberFormat cellNumberFormat = cellFormat.NumberFormatInfo;
				NumberFormat applyNumberFormat = applyFormat.NumberFormatInfo;
				options.ApplyNumberFormat = cellNumberFormat.FormatCode != applyNumberFormat.FormatCode;
			}
			ICellProtectionInfo cellProtection = cellFormat.Protection;
			ICellProtectionInfo applyProtection = applyFormat.Protection;
			differentialFormat.Protection.Hidden = applyProtection.Hidden;
			differentialFormat.Protection.Locked = applyProtection.Locked;
			if (applyFormat.ApplyProtection) {
				options.ApplyProtectionHidden = cellProtection.Hidden != applyProtection.Hidden;
				options.ApplyProtectionLocked = cellProtection.Locked != applyProtection.Locked;
			}
			differentialFormat.AssignMultiOptionsIndex(options.PackedValues);
			BorderOptionsInfo borderOptions = new BorderOptionsInfo();
			differentialFormat.AssignBorderIndex(applyFormat.BorderIndex);
			if (applyFormat.ApplyBorder) {
				BorderInfo cellBorder = cellFormat.BorderInfo;
				BorderInfo applyBorder = applyFormat.BorderInfo;
				borderOptions.ApplyBottomColor = cellBorder.BottomColorIndex != applyBorder.BottomColorIndex;
				borderOptions.ApplyBottomLineStyle = cellBorder.BottomLineStyle != applyBorder.BottomLineStyle;
				borderOptions.ApplyDiagonalColor = cellBorder.DiagonalColorIndex != applyBorder.DiagonalColorIndex;
				borderOptions.ApplyDiagonalDown = cellBorder.DiagonalDown != applyBorder.DiagonalDown;
				borderOptions.ApplyDiagonalLineStyle = cellBorder.DiagonalLineStyle != applyBorder.DiagonalLineStyle;
				borderOptions.ApplyDiagonalUp = cellBorder.DiagonalUp != applyBorder.DiagonalUp;
				borderOptions.ApplyHorizontalColor = cellBorder.HorizontalColorIndex != applyBorder.HorizontalColorIndex;
				borderOptions.ApplyHorizontalLineStyle = cellBorder.HorizontalLineStyle != applyBorder.HorizontalLineStyle;
				borderOptions.ApplyLeftColor = cellBorder.LeftColorIndex != applyBorder.LeftColorIndex;
				borderOptions.ApplyLeftLineStyle = cellBorder.LeftLineStyle != applyBorder.LeftLineStyle;
				borderOptions.ApplyOutline = cellBorder.Outline != applyBorder.Outline;
				borderOptions.ApplyRightColor = cellBorder.RightColorIndex != applyBorder.RightColorIndex;
				borderOptions.ApplyRightLineStyle = cellBorder.RightLineStyle != applyBorder.RightLineStyle;
				borderOptions.ApplyTopColor = cellBorder.TopColorIndex != applyBorder.TopColorIndex;
				borderOptions.ApplyTopLineStyle = cellBorder.TopLineStyle != applyBorder.TopLineStyle;
				borderOptions.ApplyVerticalColor = cellBorder.VerticalColorIndex != applyBorder.VerticalColorIndex;
				borderOptions.ApplyVerticalLineStyle = cellBorder.VerticalLineStyle != applyBorder.VerticalLineStyle;
			}
			differentialFormat.AssignBorderOptionsIndex(borderOptions.PackedValues);
			return DocumentModel.Cache.CellFormatCache.GetItemIndex(differentialFormat);
		}
		public string NormalizeNumberFormatText(string text, bool isPercentage) {
			if (string.IsNullOrEmpty(text))
				return text;
			StringBuilder sb = new StringBuilder();
			char next = text[0];
			char current = next;
			for (int i = 0; i < text.Length; ++i) {
				char prev = current;
				current = next;
				next = i + 1 == text.Length ? current : text[i + 1];
				if (current == '"') {
					if (prev != '"')
						sb.Append('"');
					sb.Append(@"\""");
					if (next != '"')
						sb.Append('"');
				}
				else
					sb.Append(current);
			}
			current = text[0];
			if (current != '"')
				sb.Insert(0, "\"");
			if (text.Length > 0) {
				current = text[text.Length - 1];
				if (current != '"')
					sb.Append('"');
			}
			text = sb.ToString();
			if (!isPercentage)
				return text;
			for (int i = 1; i < text.Length - 1; ++i)
				if (text[i] == '%') {
					if (text[i + 1] == '"')
						if (text[i - 1] == '"')
							return text.Substring(0, i - 1) + '%' + (text.Length > i + 2 ? text.Substring(i + 2, text.Length - i - 2) : string.Empty);
						else
							return text.Substring(0, i) + '"' + '%' + (text.Length > i + 2 ? text.Substring(i + 2, text.Length - i - 2) : string.Empty);
					if (text[i - 1] == '"')
						return text.Substring(0, i - 1) + '%' + '"' + text.Substring(i + 1, text.Length - i - 1);
					return text.Substring(0, i) + @"""%""" + text.Substring(i + 1, text.Length - i - 1);
				}
			return text;
		}
		#endregion
		#region DestinationAndXmlBasedImporter Members
		public override OpenXmlRelationCollection DocumentRelations {
			get { return null; }
		}
		public override string DocumentRootFolder {get;set;}
		protected override void PrepareOfficeTheme() {
		}
		public override string RelationsNamespace { get { return String.Empty; } }
		#endregion
	}
	#endregion
}
#endif
