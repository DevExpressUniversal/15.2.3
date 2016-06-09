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
using DevExpress.Office.Services;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.Utils.Zip;
using DevExpress.Export.Xl;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Layout.Engine;
using DevExpress.XtraSpreadsheet.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Export.OpenDocument {
	#region CellValueODSType
	public enum CellValueOdsType {
		None,
		Float,
		Currency,
		Percentage,
		Date,
		Time,
		Boolean,
		String,
	}
	#endregion
	#region StyleFamilyType
	public enum StyleFamilyType {
		Chart,
		DrawingPage,
		Graphic,
		None,
		Paragraph,
		Presentation,
		Ruby,
		Table,
		TableCell,
		TableColumn,
		TableRow,
		Text,
	}
	#endregion
	#region SpreadsheetMLBaseExporter (abstract class)
	public abstract class SpreadsheetMLBaseExporter : DocumentModelExporter {
		#region Fields
		DocumentExporterOptions options;
		XmlWriter documentContentWriter;
		#endregion
		protected SpreadsheetMLBaseExporter(DocumentModel workbook, DocumentExporterOptions options)
			: base(workbook) {
			Guard.ArgumentNotNull(options, "options");
			this.options = options;
		}
		#region Properties
		protected DocumentExporterOptions Options { get { return options; } }
		protected internal XmlWriter DocumentContentWriter { get { return documentContentWriter; } set { documentContentWriter = value; } }
		#endregion
		protected internal virtual XmlWriterSettings CreateXmlWriterSettings() {
			return XmlBasedExporterUtils.Instance.CreateDefaultXmlWriterSettings();
		}
		#region Write Helpers
		protected internal virtual void WriteBoolAttr(string attr, string ns, bool value) {
			WriteStringAttr(attr, ns, ConvertBoolToString(value));
		}
		protected internal virtual void WriteNumericAttr(string attr, string ns, double value) {
			WriteStringAttr(attr, ns, value.ToString(CultureInfo.InvariantCulture));
		}
		protected internal virtual void WriteStringAttr(string attr, string ns, string value) {
			documentContentWriter.WriteAttributeString(null, attr, ns, value);
		}
		protected internal virtual void WriteString(string text) {
			documentContentWriter.WriteString(text);
		}
		protected internal virtual void WriteNs(string nsName, string ns) {
			documentContentWriter.WriteAttributeString("xmlns", nsName, null, ns);
		}
		protected internal virtual void WriteElementString(string attr, string ns, string value) {
			documentContentWriter.WriteElementString(attr, ns, value);
		}
		#endregion
		#region Start\end elements
		protected internal virtual void WriteStartElement(string tag, string ns) {
			documentContentWriter.WriteStartElement(tag, ns);
		}
		protected internal virtual void WriteEndElement() {
			documentContentWriter.WriteEndElement();
		}
		#endregion
		#region Converters
		protected internal abstract string ConvertBoolToString(bool value);
		#endregion
	}
	#endregion
	#region OpenDocumentExporter
	public partial class OpenDocumentExporter : SpreadsheetMLBaseExporter {
		#region TranslationTables
		internal static readonly Dictionary<CellValueOdsType, string> CellValueOdsTypeTable = CreateCellValueOdsTypeTable();
		internal static readonly Dictionary<XlHorizontalAlignment, string> HorizontalAlignmentTable = CreateHorizontalAlignmentTable();
		internal static readonly Dictionary<XlVerticalAlignment, string> VerticalAlignmentTable = CreateVerticalAlignmentTable();
		internal static readonly Dictionary<XlReadingOrder, string> ReadingOrderTable = CreateReadingOrderTable();
		internal static readonly Dictionary<StyleFamilyType, string> StyleFamilyTypeTable = CreateStyleFamilyTypeTable();
		static Dictionary<CellValueOdsType, string> CreateCellValueOdsTypeTable() {
			Dictionary<CellValueOdsType, string> result = new Dictionary<CellValueOdsType, string>();
			result.Add(CellValueOdsType.Float, "float");
			result.Add(CellValueOdsType.Currency, "currency");
			result.Add(CellValueOdsType.Percentage, "percentage");
			result.Add(CellValueOdsType.Date, "date");
			result.Add(CellValueOdsType.Time, "time");
			result.Add(CellValueOdsType.Boolean, "boolean");
			result.Add(CellValueOdsType.String, "string");
			result.Add(CellValueOdsType.None, string.Empty);
			return result;
		}
		static Dictionary<XlHorizontalAlignment, string> CreateHorizontalAlignmentTable() {
			Dictionary<XlHorizontalAlignment, string> result = new Dictionary<XlHorizontalAlignment, string>();
			result.Add(XlHorizontalAlignment.Left, "start");
			result.Add(XlHorizontalAlignment.Right, "end");
			result.Add(XlHorizontalAlignment.Center, "center");
			result.Add(XlHorizontalAlignment.Justify, "justify");
			result.Add(XlHorizontalAlignment.Fill, "start"); 
			result.Add(XlHorizontalAlignment.CenterContinuous, "start");
			result.Add(XlHorizontalAlignment.Distributed, "start");
			result.Add(XlHorizontalAlignment.General, "default");
			return result;
		}
		static Dictionary<XlVerticalAlignment, string> CreateVerticalAlignmentTable() {
			Dictionary<XlVerticalAlignment, string> result = new Dictionary<XlVerticalAlignment, string>();
			result.Add(XlVerticalAlignment.Top, "top");
			result.Add(XlVerticalAlignment.Center, "middle");
			result.Add(XlVerticalAlignment.Bottom, "bottom");
			return result;
		}
		static Dictionary<XlReadingOrder, string> CreateReadingOrderTable() {
			Dictionary<XlReadingOrder, string> result = new Dictionary<XlReadingOrder, string>();
			result.Add(XlReadingOrder.LeftToRight, "lr");
			result.Add(XlReadingOrder.RightToLeft, "rl");
			result.Add(XlReadingOrder.Context, "page");
			return result;
		}
		static Dictionary<StyleFamilyType, string> CreateStyleFamilyTypeTable() {
			Dictionary<StyleFamilyType, string> result = new Dictionary<StyleFamilyType, string>();
			result.Add(StyleFamilyType.Chart, "chart");
			result.Add(StyleFamilyType.DrawingPage, "drawing-page");
			result.Add(StyleFamilyType.Graphic, "graphic");
			result.Add(StyleFamilyType.None, "none");
			result.Add(StyleFamilyType.Paragraph, "paragraph");
			result.Add(StyleFamilyType.Presentation, "presentation");
			result.Add(StyleFamilyType.Ruby, "ruby");
			result.Add(StyleFamilyType.Table, "table");
			result.Add(StyleFamilyType.TableCell, "table-cell");
			result.Add(StyleFamilyType.TableColumn, "table-column");
			result.Add(StyleFamilyType.TableRow, "table-row");
			result.Add(StyleFamilyType.Text, "text");
			return result;
		}
		#endregion
		#region Constants
		public const string FoNamespace = "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0";
		public const string ManifestNamespace = "urn:oasis:names:tc:opendocument:xmlns:manifest:1.0";
		public const string NumberNamespace = "urn:oasis:names:tc:opendocument:xmlns:datastyle:1.0";
		public const string OfficeNamespace = "urn:oasis:names:tc:opendocument:xmlns:office:1.0";
		public const string StyleNamespace = "urn:oasis:names:tc:opendocument:xmlns:style:1.0";
		public const string SvgNamespace = "urn:oasis:names:tc:opendocument:xmlns:svg-compatible:1.0";
		public const string TableNamespace = "urn:oasis:names:tc:opendocument:xmlns:table:1.0";
		public const string TextNamespace = "urn:oasis:names:tc:opendocument:xmlns:text:1.0";
		public const string XLinkNamespace = "http://www.w3.org/1999/xlink";
		#endregion
		#region Fields
		Stream outputStream;
		InternalZipArchive package;
		DateTime now;
		readonly ExportStyleSheet exportStyleSheet;
		#endregion
		public OpenDocumentExporter(DocumentModel workbook, OpenDocumentDocumentExporterOptions options)
			: base(workbook, options) {
			this.exportStyleSheet = new ExportStyleSheet(workbook);
		}
		#region Properties
		public new OpenDocumentDocumentExporterOptions Options { get { return (OpenDocumentDocumentExporterOptions)base.Options; } }
		protected internal Stream OutputStream { get { return outputStream; } }
		protected internal ExportStyleSheet ExportStyleSheet { get { return exportStyleSheet; } }
		#endregion
		public virtual void Export(Stream outputStream) {
			this.outputStream = outputStream;
			Export();
		}
		public override void Export() {
			if (outputStream == null)
				throw new InvalidOperationException();
			ILogService logService = Workbook.GetService<ILogService>();
			if (logService != null)
				logService.Clear();
			Workbook.DataContext.SetImportExportSettings();
			CreateInternalZipArchive(outputStream);
			Workbook.DataContext.SetWorkbookDefinedSettings();
		}
		protected void CreateInternalZipArchive(Stream outputStream) {
			using (InternalZipArchive documentPackage = new InternalZipArchive(outputStream)) {
				this.package = documentPackage;
				InitializeExport();
				AddPackageContents();
			}
		}
		protected virtual void AddPackageContents() {
			AddPackageContent(@"styles.xml", ExportStyles());
			AddPackageContent(@"content.xml", ExportContent());
			AddPackageContent(@"META-INF\manifest.xml", ExportManifest());
		}
		protected internal virtual void AddPackageContent(string fileName, CompressedStream content) {
			if (content != null)
				package.AddCompressed(fileName, now, content);
		}
		protected internal virtual void InitializeExport() {
			this.now = DateTime.Now;
			this.exportStyleSheet.RegisterStyles();
		}
		protected internal virtual CompressedStream CreateXmlContent(Action<XmlWriter> action) {
			return CreateXmlContent(action, CreateXmlWriterSettings());
		}
		protected internal virtual CompressedStream CreateXmlContent(Action<XmlWriter> action, XmlWriterSettings xmlSettings) {
			return XmlBasedExporterUtils.Instance.CreateCompressedXmlContent(action, xmlSettings);
		}
		protected internal override string ConvertBoolToString(bool value) {
			return value ? "true" : "false";
		}
		string ConvertColorToString(int index, string ifEmpty) {
			Color color = Workbook.Cache.ColorModelInfoCache[index].ToRgb(Workbook.StyleSheet.Palette, Workbook.OfficeTheme.Colors);
			if (color == DXColor.Empty)
				return ifEmpty;
			string r = color.R.ToString("x");
			if (r.Length == 1)
				r = "0" + r;
			string g = color.G.ToString("x");
			if (g.Length == 1)
				g = "0" + g;
			string b = color.B.ToString("x");
			if (b.Length == 1)
				b = "0" + b;
			return string.Concat("#", r, g, b);
		}
	}
	#endregion
	public class OdsConditionalFormattingImportCollection {
		List<OdsConditionalFormattingImport> innerList;
		public OdsConditionalFormattingImportCollection() {
			innerList = new List<OdsConditionalFormattingImport>();
		}
		public List<OdsConditionalFormattingImport> this[string styleName] {
			get {
				List<OdsConditionalFormattingImport> result = new List<OdsConditionalFormattingImport>();
				innerList.ForEach(format => {
					if (format.StyleName.Equals(styleName, StringComparison.Ordinal))
						result.Add(format);
				});
				return result;
			}
		}
		#region Import
		public void AssignCell(ICell cell, ICell newCell) {
			innerList.ForEach(format => {
				if (format.Contains(cell))
					format.AddRange(newCell.GetRange());
			});
		}
		public void AssignRange(string styleName, CellRange range) {
			this[styleName].ForEach(format => format.AddRange(range));
		}
		public void Register(CellFormat cellFormat, string styleName, string applyStyleName, string baseCellAddress, string condition, DevExpress.XtraSpreadsheet.Import.OpenDocument.OpenDocumentWorkbookImporter importer) {
			int differentialFormatIndex = importer.GetDifferentialFormatIndex(cellFormat, applyStyleName);
			if (differentialFormatIndex < 0)
				return;
			condition = condition.Replace("COM.MICROSOFT.", "");
			condition = condition.Replace("com.microsoft.", "");
			condition = importer.NormalizeOdsFormula(condition).Remove(0, 1);
			ConditionalFormattingFormulaCreatorData creator = new ConditionalFormattingFormulaCreatorData();
			if (condition.StartsWith("is-true-formula", StringComparison.OrdinalIgnoreCase)) {
				condition = condition.Substring(16, condition.Length - 17);
				creator.RuleType = ConditionalFormattingRuleType.ExpressionIsTrue;
				creator.Formulas.Add(condition);
			}
			else
				if (condition.StartsWith("cell-content()", StringComparison.OrdinalIgnoreCase)) {
					condition = condition.Substring(14);
					char first = condition[0];
					char second = condition[1];
					if (first == '>') 
						if (second == '=') { 
							condition = condition.Substring(2);
							creator.CfOperator = ConditionalFormattingOperator.GreaterThanOrEqual;
						}
						else {
							condition = condition.Substring(1);
							creator.CfOperator = ConditionalFormattingOperator.GreaterThan;
						}
					else
						if (first == '<') 
							if (second == '=') { 
								condition = condition.Substring(2);
								creator.CfOperator = ConditionalFormattingOperator.LessThanOrEqual;
							}
							else {
								condition = condition.Substring(1);
								creator.CfOperator = ConditionalFormattingOperator.LessThan;
							}
						else
							if (first == '=') { 
								condition = condition.Substring(1);
								creator.CfOperator = ConditionalFormattingOperator.Equal;
							}
							else
								if (first == '!' && second == '=') { 
									condition = condition.Substring(2);
									creator.CfOperator = ConditionalFormattingOperator.NotEqual;
								}
								else
									return;
					creator.RuleType = ConditionalFormattingRuleType.CompareWithFormulaResult;
					creator.Formulas.Add(condition);
				}
				else {
					if (condition.StartsWith("cell-content-is-between", StringComparison.OrdinalIgnoreCase)) {
						condition = condition.Substring(24, condition.Length - 25);
						creator.CfOperator = ConditionalFormattingOperator.Between;
					}
					else
						if (condition.StartsWith("cell-content-is-not-between", StringComparison.OrdinalIgnoreCase)) {
							condition = condition.Substring(28, condition.Length - 29);
							creator.CfOperator = ConditionalFormattingOperator.NotBetween;
						}
						else
							return;
					string[] conditions = condition.Split(new char[] { ',' });
					creator.RuleType = ConditionalFormattingRuleType.CompareWithFormulaResult;
					creator.Formulas.Add(conditions[0]);
					creator.Formulas.Add(conditions[1]);
				}
			OdsConditionalFormattingImport conditionalFormatting = new OdsConditionalFormattingImport(styleName, differentialFormatIndex, creator);
			innerList.Add(conditionalFormatting);
		}
		public void Apply(DevExpress.XtraSpreadsheet.Import.OpenDocument.OpenDocumentWorkbookImporter importer) {
			innerList.ForEach(format => format.Apply());
		}
		#endregion
	}
	public class OdsConditionalFormattingImport {
		CellUnion range;
		int differentialFormatIndex;
		ConditionalFormattingFormulaCreatorData creator;
		string styleName;
		public OdsConditionalFormattingImport(string styleName, int differentialFormatIndex, ConditionalFormattingFormulaCreatorData creator) {
			this.styleName = styleName;
			this.differentialFormatIndex = differentialFormatIndex;
			this.creator = creator;
			this.range = new CellUnion(new List<CellRangeBase>());
		}
		public string StyleName { get { return styleName; } }
		#region Import
		public void AddRange(CellRange range) {
			this.range.MergeWithRange(range);
		}
		public bool Contains(ICell cell) {
			return range.ContainsCell(cell);
		}
		public void Apply() {
			while (range.InnerCellRanges.Count > 0) {
				Worksheet sheet = (Worksheet)range.InnerCellRanges[0].Worksheet;
				List<CellRangeBase> cellRanges = new List<CellRangeBase>();
				for (int i = 0; i < range.InnerCellRanges.Count; ) {
					CellRangeBase cellRangeBase = range.InnerCellRanges[i];
					if (object.ReferenceEquals(sheet, cellRangeBase.Worksheet)) {
						cellRanges.Add(cellRangeBase);
						range.InnerCellRanges.RemoveAt(i);
					}
					else
						++i;
				}
				CellRangeBase conditionalFormattingRange = new CellUnion(cellRanges);
				creator.Sheet = sheet;
				creator.CellRange = conditionalFormattingRange;
				ConditionalFormatting conditionalFormatting = creator.CreateConditionalFormatting();
				conditionalFormatting.DifferentialFormatIndex = differentialFormatIndex;
				sheet.ConditionalFormattings.AddWithPrioritiesCorrection(conditionalFormatting);
			}
		}
		#endregion
	}
	public class OdsConditionalFormattingExportCollection {
		List<OdsConditionalFormattingExport> innerList;
		public OdsConditionalFormattingExportCollection() {
			innerList = new List<OdsConditionalFormattingExport>();
		}
		public OdsConditionalFormattingExport GetConditionalFormatting(CellIntervalRange range) {
			foreach (OdsConditionalFormattingExport formatItem in innerList)
				if (formatItem.Contains(range))
					return formatItem;
			return null;
		}
		public OdsConditionalFormattingExport GetConditionalFormatting(ICell cell) {
			foreach (OdsConditionalFormattingExport formatItem in innerList)
				if (formatItem.Contains(cell))
					return formatItem;
			return null;
		}
		#region Export
		public void Register(ICell cell, ConditionalFormatting conditionalFormatting) {
			FormulaConditionalFormatting formulaConditionalFormatting = conditionalFormatting as FormulaConditionalFormatting;
			if (formulaConditionalFormatting == null)
				return;
			int sameFormatIndex = -1;
			OdsConditionalFormattingExport odsConditionalFormatting;
			foreach (OdsConditionalFormattingExport format in innerList)
				if (format.CellFormatIndex == cell.FormatIndex)
					if (format.Contains(cell)) {
						format.Remove(cell);
						odsConditionalFormatting = format.CloneWithoutCells(cell, innerList.Count);
						odsConditionalFormatting.Formats.Add(formulaConditionalFormatting);
						innerList.Add(odsConditionalFormatting);
						return;
					}
					else
						if (format.Formats.Count == 1 && format.Formats[0].Equals(conditionalFormatting))
							sameFormatIndex = innerList.IndexOf(format);
			if (sameFormatIndex >= 0)
				innerList[sameFormatIndex].Add(cell);
			else {
				odsConditionalFormatting = new OdsConditionalFormattingExport(cell, cell.FormatIndex, innerList.Count);
				odsConditionalFormatting.Formats.Add(formulaConditionalFormatting);
				innerList.Add(odsConditionalFormatting);
			}
		}
		public void Generate(OpenDocumentExporter exporter) {
			OdsParsedThingVisitor visitor = new OdsParsedThingVisitor(exporter.Workbook.DataContext);
			foreach (OdsConditionalFormattingExport format in innerList)
				if (format.HasCells)
					format.Generate(visitor, exporter);
		}
		public void Clear() {
			innerList.Clear();
		}
		#endregion
	}
	public class OdsConditionalFormattingExport {
		int conditionalFormattingIndex;
		int cellFormatIndex;
		ICell baseCell;
		CellRangeBase range;
		List<FormulaConditionalFormatting> formats;
		public OdsConditionalFormattingExport(ICell baseCell, int cellFormatIndex, int nextConditionalFormattingIndex) {
			this.baseCell = baseCell;
			this.conditionalFormattingIndex = nextConditionalFormattingIndex;
			this.cellFormatIndex = cellFormatIndex;
			this.range = baseCell.GetRange().Clone();
			this.formats = new List<FormulaConditionalFormatting>();
		}
		public bool HasCells { get { return range == null ? false : range.CellCount > 0; } }
		public int CellFormatIndex { get { return cellFormatIndex; } }
		public int ConditionalFormatIndex { get { return conditionalFormattingIndex; } }
		public List<FormulaConditionalFormatting> Formats { get { return formats; } }
		public CellRangeBase Range { get { return range; } }
		public void Add(ICell cell) {
			range = range.MergeWithRange(cell.GetRange());
		}
		public void Remove(ICell cell) {
			range = range.ExcludeRange(cell.GetRange());
		}
		public bool Contains(ICell cell) {
			return range.ContainsCell(cell);
		}
		public bool Contains(CellRangeBase what) {
			VariantValue intersectionValue = what.IntersectionWith(range);
			if (intersectionValue.IsError)
				return false;
			CellRange intersection = intersectionValue.CellRangeValue as CellRange;
			if (intersection.EqualsPosition(what))
				return true;
			return false;
		}
		public OdsConditionalFormattingExport CloneWithoutCells(ICell baseCell, int nextConditionalFormattingIndex) {
			OdsConditionalFormattingExport result = new OdsConditionalFormattingExport(baseCell, this.cellFormatIndex, nextConditionalFormattingIndex);
			this.formats.ForEach(format => result.formats.Add(format));
			return result;
		}
		public void Generate(OdsParsedThingVisitor visitor, OpenDocumentExporter exporter) {
			CellFormat cellFormat = (CellFormat)exporter.Workbook.Cache.CellFormatCache[cellFormatIndex];
			exporter.GenerateCellFormat(
				exporter.ExportStyleSheet.GetConditionalFormatName(this),
				cellFormat,
				delegate { GenerateConditions(visitor, exporter);
				});
		}
		void GenerateConditions(OdsParsedThingVisitor visitor, OpenDocumentExporter exporter) {
			foreach (FormulaConditionalFormatting format in formats) {
				exporter.WriteStartElement("map", OpenDocumentExporter.StyleNamespace);
				try {
					GenerateFormulaConditionalFormatting(format, visitor, exporter);
				}
				finally {
					exporter.WriteEndElement();
				}
			}
		}
		void GenerateFormulaConditionalFormatting(FormulaConditionalFormatting format, OdsParsedThingVisitor visitor, OpenDocumentExporter exporter) {
			string condition = GetCondition(format, visitor);
			if (string.IsNullOrEmpty(condition))
				return;
			exporter.WriteStringAttr("base-cell-address", OpenDocumentExporter.StyleNamespace, baseCell.Worksheet.Name + "." + baseCell.Position);
			exporter.WriteStringAttr("condition", OpenDocumentExporter.StyleNamespace, condition);
			exporter.WriteStringAttr("apply-style-name", OpenDocumentExporter.StyleNamespace, exporter.ExportStyleSheet.GetDifferentialFormatName(format.DifferentialFormatIndex));
		}
		string GetCondition(FormulaConditionalFormatting format, OdsParsedThingVisitor visitor) {
			switch (format.RuleType) {
				case ConditionalFormattingRuleType.AboveOrBelowAverage:
					return GetCondition(format as AverageFormulaConditionalFormatting, visitor);
				case ConditionalFormattingRuleType.BeginsWithText:
				case ConditionalFormattingRuleType.ContainsText:
				case ConditionalFormattingRuleType.EndsWithText:
				case ConditionalFormattingRuleType.NotContainsText:
					return GetCondition(format as TextFormulaConditionalFormatting, visitor);
				case ConditionalFormattingRuleType.CellIsBlank:
				case ConditionalFormattingRuleType.CellIsNotBlank:
				case ConditionalFormattingRuleType.ContainsErrors:
				case ConditionalFormattingRuleType.NotContainsErrors:
				case ConditionalFormattingRuleType.DuplicateValues:
				case ConditionalFormattingRuleType.UniqueValue:
					return GetCondition(format as SpecialFormulaConditionalFormatting, visitor);
				case ConditionalFormattingRuleType.CompareWithFormulaResult:
					return GetFormulaResultCondition(format, visitor);
				case ConditionalFormattingRuleType.ExpressionIsTrue:
					return GetCondition(format as ExpressionFormulaConditionalFormatting, visitor);
				case ConditionalFormattingRuleType.InsideDatePeriod:
					return GetCondition(format as TimePeriodFormulaConditionalFormatting);
				case ConditionalFormattingRuleType.TopOrBottomValue:
					return GetCondition(format as RankFormulaConditionalFormatting, visitor);
				default:
					return null;
			}
		}
		string GetFormulaResultCondition(FormulaConditionalFormatting format, OdsParsedThingVisitor visitor) {
			switch (format.Operator) {
				case ConditionalFormattingOperator.BeginsWith:
				case ConditionalFormattingOperator.ContainsText:
				case ConditionalFormattingOperator.EndsWith:
				case ConditionalFormattingOperator.NotContains:
					return GetCondition(format as TextFormulaConditionalFormatting, visitor);
				case ConditionalFormattingOperator.Between:
				case ConditionalFormattingOperator.NotBetween:
					return GetCondition(format as RangeFormulaConditionalFormatting, visitor);
				case ConditionalFormattingOperator.Equal:
				case ConditionalFormattingOperator.GreaterThan:
				case ConditionalFormattingOperator.GreaterThanOrEqual:
				case ConditionalFormattingOperator.LessThan:
				case ConditionalFormattingOperator.LessThanOrEqual:
				case ConditionalFormattingOperator.NotEqual:
					return GetCondition(format as ExpressionFormulaConditionalFormatting, visitor);
				default:
					return null;
			}
		}
		string ConvertRangeToString(CellRangeBase range, WorkbookDataContext context, OdsParsedThingVisitor visitor) {
			switch (range.RangeType) {
				case CellRangeType.IntervalRange:
					CellIntervalRange intervalRange = (CellIntervalRange)range;
					if (intervalRange.IsColumnInterval)
						range = CellIntervalRange.CreateColumnInterval(intervalRange.Worksheet, intervalRange.LeftColumnIndex, PositionType.Absolute, intervalRange.RightColumnIndex, PositionType.Absolute);
					else
						range = CellIntervalRange.CreateRowInterval(intervalRange.Worksheet, intervalRange.TopRowIndex, PositionType.Absolute, intervalRange.BottomRowIndex, PositionType.Absolute);
					break;
				case CellRangeType.SingleRange:
					range.TopLeft = new CellPosition(range.TopLeft.Column, range.TopLeft.Row, PositionType.Absolute, PositionType.Absolute);
					range.BottomRight = new CellPosition(range.BottomRight.Column, range.BottomRight.Row, PositionType.Absolute, PositionType.Absolute);
					break;
				case CellRangeType.UnionRange:
					CellUnion cellUnion = (CellUnion)range;
					cellUnion.InnerCellRanges.ForEach(rangeItem => {
						rangeItem.TopLeft = new CellPosition(rangeItem.TopLeft.Column, rangeItem.TopLeft.Row, PositionType.Absolute, PositionType.Absolute);
						rangeItem.BottomRight = new CellPosition(rangeItem.BottomRight.Column, rangeItem.BottomRight.Row, PositionType.Absolute, PositionType.Absolute);
					});
					range = cellUnion;
					break;
			}
			return visitor.BuildExpressionString(context.ParseExpression(range.ToString(), OperandDataType.None, false), baseCell);
		}
		string GetCondition(ExpressionFormulaConditionalFormatting cf, OdsParsedThingVisitor visitor) {
			string expr = visitor.BuildExpressionString(cf.ValueExpression, baseCell);
			switch (cf.Condition) {
				case ConditionalFormattingExpressionCondition.EqualTo:
					return "cell-content()=" + expr;
				case ConditionalFormattingExpressionCondition.ExpressionIsTrue:
					return "is-true-formula(" + expr + ")";
				case ConditionalFormattingExpressionCondition.GreaterThan:
					return "cell-content()>" + expr;
				case ConditionalFormattingExpressionCondition.GreaterThanOrEqual:
					return "cell-content()>=" + expr;
				case ConditionalFormattingExpressionCondition.InequalTo:
					return "cell-content()!=" + expr;
				case ConditionalFormattingExpressionCondition.LessThan:
					return "cell-content()<" + expr;
				case ConditionalFormattingExpressionCondition.LessThanOrEqual:
					return "cell-content()<=" + expr;
				default:
					return null;
			}
		}
		string GetCondition(TextFormulaConditionalFormatting cf, OdsParsedThingVisitor visitor) {
			string expr = visitor.BuildExpressionString(cf.ValueExpression, baseCell);
			switch (cf.Condition) {
				case ConditionalFormattingTextCondition.BeginsWith:
					return string.Concat(@"is-true-formula(LEFT([.", baseCell.Position, @"];LEN(", expr, @"))=", expr, @")");
				case ConditionalFormattingTextCondition.Contains:
					return string.Concat(@"is-true-formula(NOT(ISERROR(SEARCH(", expr, @";[.", baseCell.Position, @"]))))");
				case ConditionalFormattingTextCondition.EndsWith:
					return string.Concat(@"is-true-formula(RIGHT([.", baseCell.Position, @"];LEN(", expr, @"))=", expr, @")");
				case ConditionalFormattingTextCondition.NotContains:
					return string.Concat(@"is-true-formula(ISERROR(SEARCH(", expr, @";[.", baseCell.Position, @"])))");
				default:
					return null;
			}
		}
		string GetCondition(RangeFormulaConditionalFormatting cf, OdsParsedThingVisitor visitor) {
			string lowValue = visitor.BuildExpressionString(cf.ValueExpression, baseCell);
			string highValue = visitor.BuildExpressionString(cf.Value2Expression, baseCell);
			switch (cf.Condition) {
				case ConditionalFormattingRangeCondition.Inside:
					return string.Concat("cell-content-is-between(", lowValue, ",", highValue, ")");
				case ConditionalFormattingRangeCondition.Outside:
					return string.Concat("cell-content-is-not-between(", lowValue, ",", highValue, ")");
				default:
					return null;
			}
		}
		string GetCondition(SpecialFormulaConditionalFormatting cf, OdsParsedThingVisitor visitor) {
			string range = ConvertRangeToString(cf.CellRange, cf.Sheet.DataContext, visitor);
			switch (cf.Condition) {
				case ConditionalFormattingSpecialCondition.ContainBlanks:
					return string.Concat(@"is-true-formula(LEN(TRIM([.", baseCell.Position, @"]))=0)");
				case ConditionalFormattingSpecialCondition.ContainDuplicateValue:
					return string.Concat(@"is-true-formula(AND(COUNTIF(", range, @";[.", baseCell.Position, @"])>1;NOT(ISBLANK([.", baseCell.Position, @"]))))");
				case ConditionalFormattingSpecialCondition.ContainError:
					return string.Concat(@"is-true-formula(ISERROR([.", baseCell.Position, @"]))");
				case ConditionalFormattingSpecialCondition.ContainNonBlanks:
					return string.Concat(@"is-true-formula(LEN(TRIM([.", baseCell.Position, @"]))>0)");
				case ConditionalFormattingSpecialCondition.ContainUniqueValue:
					return string.Concat(@"is-true-formula(AND(COUNTIF(", range, @";[.", baseCell.Position, @"])=1;NOT(ISBLANK([.", baseCell.Position, @"]))))");
				case ConditionalFormattingSpecialCondition.NotContainError:
					return string.Concat(@"is-true-formula(NOT(ISERROR([.", baseCell.Position, @"])))");
				default:
					return null;
			}
		}
		string GetCondition(TimePeriodFormulaConditionalFormatting cf) {
			switch (cf.TimePeriod) {
				case ConditionalFormattingTimePeriod.Last7Days:
					return string.Concat(@"is-true-formula(AND(TODAY()-FLOOR([.", baseCell.Position, @"];1)<=6;FLOOR([.", baseCell.Position, @"];1)<=TODAY()))");
				case ConditionalFormattingTimePeriod.LastMonth:
					return string.Concat(@"is-true-formula(AND(MONTH([.", baseCell.Position, @"])=MONTH(EDATE(TODAY();0-1));YEAR([.", baseCell.Position, @"])=YEAR(EDATE(TODAY();0-1))))");
				case ConditionalFormattingTimePeriod.LastWeek:
					return string.Concat(@"is-true-formula(AND(TODAY()-ROUNDDOWN([.", baseCell.Position, @"];0)>=(WEEKDAY(TODAY()));TODAY()-ROUNDDOWN([.", baseCell.Position, @"];0)<(WEEKDAY(TODAY())+7)))");
				case ConditionalFormattingTimePeriod.NextMonth:
					return string.Concat(@"is-true-formula(AND(MONTH([.", baseCell.Position, @"])=MONTH(EDATE(TODAY();0+1));YEAR([.", baseCell.Position, @"])=YEAR(EDATE(TODAY();0+1))))");
				case ConditionalFormattingTimePeriod.NextWeek:
					return string.Concat(@"is-true-formula(AND(ROUNDDOWN([.", baseCell.Position, @"];0)-TODAY()>(7-WEEKDAY(TODAY()));ROUNDDOWN([.", baseCell.Position, @"];0)-TODAY()<(15-WEEKDAY(TODAY()))))");
				case ConditionalFormattingTimePeriod.ThisMonth:
					return string.Concat(@"is-true-formula(AND(MONTH([.", baseCell.Position, @"])=MONTH(TODAY());YEAR([.", baseCell.Position, @"])=YEAR(TODAY())))");
				case ConditionalFormattingTimePeriod.ThisWeek:
					return string.Concat(@"is-true-formula(AND(TODAY()-ROUNDDOWN([.", baseCell.Position, @"];0)<=WEEKDAY(TODAY())-1;ROUNDDOWN([.", baseCell.Position, @"];0)-TODAY()<=7-WEEKDAY(TODAY())))");
				case ConditionalFormattingTimePeriod.Today:
					return string.Concat(@"is-true-formula(FLOOR([.", baseCell.Position, @"];1)=TODAY())");
				case ConditionalFormattingTimePeriod.Tomorrow:
					return string.Concat(@"is-true-formula(FLOOR([.", baseCell.Position, @"];1)=TODAY()+1)");
				case ConditionalFormattingTimePeriod.Yesterday:
					return string.Concat(@"is-true-formula(FLOOR([.", baseCell.Position, @"];1)=TODAY()-1)");
				default:
					return null;
			}
		}
		string GetCondition(RankFormulaConditionalFormatting cf, OdsParsedThingVisitor visitor) {
			string range = ConvertRangeToString(cf.CellRange, cf.Sheet.DataContext, visitor);
			switch (cf.Condition) {
				case ConditionalFormattingRankCondition.BottomByPercent:
					return string.Concat(@"is-true-formula(IF(INT(COUNT(", range, @")*", cf.Rank, @"%)>0;SMALL(", range, @";INT(COUNT(", range, @")*", cf.Rank, @"%));MIN(", range, @"))>=[.", baseCell.Position, @"])");
				case ConditionalFormattingRankCondition.BottomByRank:
					return string.Concat(@"is-true-formula(SMALL((", range, @");MIN(", cf.Rank, @";COUNT(", range, @")))>=[.", baseCell.Position, @"])");
				case ConditionalFormattingRankCondition.TopByPercent:
					return string.Concat(@"is-true-formula(IF(INT(COUNT(", range, @")*", cf.Rank, @"%)>0;LARGE(", range, @";INT(COUNT(", range, @")*", cf.Rank, @"%));MAX(", range, @"))<=[.", baseCell.Position, @"])");
				case ConditionalFormattingRankCondition.TopByRank:
					return string.Concat(@"is-true-formula(LARGE((", range, @");MIN(", cf.Rank, @";COUNT(", range, @")))<=[.", baseCell.Position, @"])");
				default:
					return null;
			}
		}
		string GetCondition(AverageFormulaConditionalFormatting cf, OdsParsedThingVisitor visitor) {
			string range = ConvertRangeToString(cf.CellRange, cf.Sheet.DataContext, visitor);
			switch (cf.Condition) {
				case ConditionalFormattingAverageCondition.Above:
					if (cf.StdDev == 0)
						return string.Concat(@"is-true-formula([.", baseCell.Position, @"]>AVERAGE(IF(ISERROR(", range, @");"""";IF(ISBLANK(", range, @");"""";", range, @"))))");
					string aboveExpr = string.Concat(@"is-true-formula(([.", baseCell.Position, @"]-AVERAGE(", range, @"))>=STDEVP(", range, @")*(");
					switch (cf.StdDev) {
						case 1:
							return aboveExpr + "1))";
						case 2:
							return aboveExpr + "2))";
						case 3:
							return aboveExpr + "3))";
						default:
							return null;
					}
				case ConditionalFormattingAverageCondition.Below:
					if (cf.StdDev == 0)
						return string.Concat(@"is-true-formula([.", baseCell.Position, @"]<AVERAGE(IF(ISERROR(", range, @");"""";IF(ISBLANK(", range, @");"""";", range, @"))))");
					string belowExpr = string.Concat(@"is-true-formula(([.", baseCell.Position, @"]-AVERAGE(", range, @"))<=STDEVP(", range, @")*(");
					switch (cf.StdDev) {
						case 1:
							return belowExpr + "-1))";
						case 2:
							return belowExpr + "-2))";
						case 3:
							return belowExpr + "-3))";
						default:
							return null;
					}
				case ConditionalFormattingAverageCondition.AboveOrEqual:
					return string.Concat(@"is-true-formula([.", baseCell.Position, @"]>=AVERAGE(IF(ISERROR(", range, @");"""";IF(ISBLANK(", range, @");"""";", range, @"))))");
				case ConditionalFormattingAverageCondition.BelowOrEqual:
					return string.Concat(@"is-true-formula([.", baseCell.Position, @"]<=AVERAGE(IF(ISERROR(", range, @");"""";IF(ISBLANK(", range, @");"""";", range, @"))))");
				default:
					return null;
			}
		}
	}
	public class OdsRowFormat  {
		#region Static
		public static OdsRowFormat GetFormat(Row row) {
			return new OdsRowFormat() { RowHeight = GetRowHeight(row), UseOptimalRowHeight = !GetIsCustomHeight(row) };
		}
		public static bool IsHidden(Row row) {
			return row.Height == 0 && row.IsCustomHeight;
		}
		static float GetRowHeight(Row row) {
			Worksheet sheet = (Worksheet)row.Sheet;
			if (row.Height == 0) {
				if (sheet.Properties.FormatProperties.IsCustomHeight)
					return sheet.Properties.FormatProperties.DefaultRowHeight;
				return 300; 
			}
			return row.Height;
		}
		static bool GetIsCustomHeight(Row row) {
			if (row.Height == 0)
				if (!row.IsCustomHeight)
					return ((Worksheet)row.Sheet).Properties.FormatProperties.IsCustomHeight;
				else
					return false;
			return row.IsCustomHeight;
		}
		#endregion
		#region Properties
		public bool UseOptimalRowHeight { get; set; }
		public bool IsNull { get { return RowHeight <= 0 || UseOptimalRowHeight; } }
		public float RowHeight { get; set; }
		#endregion
		#region Import
		public void ReadAttributes(XmlReader reader, DevExpress.XtraSpreadsheet.Import.OpenDocument.OpenDocumentWorkbookImporter importer) {
			UseOptimalRowHeight = importer.GetWpSTOnOffValue(reader, "style:use-optimal-row-height", UseOptimalRowHeight);
			RowHeight = importer.GetLength(reader, "style:row-height", RowHeight);
		}
		public void ApplyFormatOnRow(Row row) {
			if (IsNull)
				return;
			row.Height = RowHeight > 8192 ? 8192 : RowHeight;
			row.IsCustomHeight = !UseOptimalRowHeight;
		}
		#endregion
		#region Export
		public void Generate(OpenDocumentExporter exporter) {
			exporter.WriteStartElement("table-row-properties", OpenDocumentExporter.StyleNamespace);
			try {
				exporter.WriteBoolAttr("use-optimal-row-height", OpenDocumentExporter.StyleNamespace, UseOptimalRowHeight);
				float heightInches = exporter.Workbook.UnitConverter.ModelUnitsToInchesF(RowHeight);
				exporter.WriteStringAttr("row-height", OpenDocumentExporter.StyleNamespace, heightInches.ToString("0.0", CultureInfo.InvariantCulture) + "in");
			}
			finally {
				exporter.WriteEndElement();
			}
		}
		#endregion
		public override bool Equals(object obj) {
			OdsRowFormat other = obj as OdsRowFormat;
			if (object.ReferenceEquals(other, null))
				return false;
			if (this.UseOptimalRowHeight.Equals(other.UseOptimalRowHeight) && this.RowHeight.Equals(other.RowHeight))
				return true;
			return false;
		}
		public override int GetHashCode() {
			return RowHeight.GetHashCode();
		}
		public OdsRowFormat Clone() {
			return new OdsRowFormat() { RowHeight = this.RowHeight, UseOptimalRowHeight = this.UseOptimalRowHeight };
		}
	}
	public class OdsColumnFormat {
		#region Static
		public static OdsColumnFormat GetFormat(Column column) {
			return new OdsColumnFormat() { ColumnWidth = GetColumnWidth(column), UseOptimalColumnWidth = !GetIsCustomWidth(column) };
		}
		public static bool IsHidden(Column column) {
			return column.Width == 0 && column.IsCustomWidth;
		}
		static float GetColumnWidth(Column column) {
			float width = column.Width;
			if (ColumnWidthInfo.DefaultValue == width ) {
				width = column.Sheet.Properties.FormatProperties.DefaultColumnWidth;
				if (ColumnWidthInfo.DefaultValue == width)
					width = column.Sheet.Properties.FormatProperties.BaseColumnWidth;
			}
			return width;
		}
		static bool GetIsCustomWidth(Column column) {
			return true;
		}
		#endregion
		#region Properties
		public bool UseOptimalColumnWidth { get; set; }
		public float ColumnWidth { get; set; }
		#endregion
		#region Import
		public void ReadAttributes(XmlReader reader, DevExpress.XtraSpreadsheet.Import.OpenDocument.OpenDocumentWorkbookImporter importer) {
			UseOptimalColumnWidth = importer.GetWpSTOnOffValue(reader, "style:use-optimal-column-width", UseOptimalColumnWidth);
			ColumnWidth = importer.GetLength(reader, "style:column-width", ColumnWidth);
		}
		public void ApplyFormatOnColumn(Column column) {
			if (UseOptimalColumnWidth || ColumnWidth <= 0)
				return;
			float columnWidth = ConvertModelUnitsToCharacters((int)ColumnWidth, column.Sheet);
			if (columnWidth > 255)
				columnWidth = 255;
			column.Width = columnWidth;
			column.IsCustomWidth = !UseOptimalColumnWidth;
		}
		float ConvertModelUnitsToCharacters(int width, Worksheet sheet) {
			float layoutWidth = sheet.Workbook.ToDocumentLayoutUnitConverter.ToLayoutUnits(width);
			int inPixels = sheet.Workbook.UnitConverter.ModelUnitsToPixels(width, Model.DocumentModel.Dpi);
			IColumnWidthCalculationService service = sheet.Workbook.GetService<IColumnWidthCalculationService>();
			float widthInCharacters = service.ConvertLayoutsToCharacters(sheet, layoutWidth, inPixels);
			return service.RemoveGaps(sheet, widthInCharacters);
		}
		#endregion
		#region Export
		public void Generate(OpenDocumentExporter exporter) {
			exporter.WriteStartElement("table-column-properties", OpenDocumentExporter.StyleNamespace);
			try {
				float widthInInches = ConvertCharactersToInches(ColumnWidth, exporter.Workbook);
				exporter.WriteStringAttr("column-width", OpenDocumentExporter.StyleNamespace, widthInInches.ToString("0.0", CultureInfo.InvariantCulture) + "in");
			}
			finally {
				exporter.WriteEndElement();
			}
		}
		float ConvertCharactersToInches(float width, DocumentModel workbook) {
			float maxDigitWidth = workbook.MaxDigitWidth;
			float maxDigitWidthInPixels = workbook.MaxDigitWidthInPixels;
			float layoutWidth;
			Worksheet sheet = workbook.Sheets[0];
			if (UseOptimalColumnWidth)
				layoutWidth = ColumnWidthCalculationUtils.CalcDefColumnWidthInLayouts(sheet, maxDigitWidth, maxDigitWidthInPixels);
			else
				layoutWidth = ColumnWidthCalculationUtils.MeasureCellWidth(ColumnWidthCalculationUtils.AddGaps(sheet, width), maxDigitWidth);
			float modelWidth = workbook.ToDocumentLayoutUnitConverter.ToModelUnits(layoutWidth);
			return workbook.UnitConverter.ModelUnitsToInchesF(modelWidth);
		}
		#endregion
		public override bool Equals(object obj) {
			OdsColumnFormat other = obj as OdsColumnFormat;
			if (object.ReferenceEquals(other, null))
				return false;
			if (this.UseOptimalColumnWidth.Equals(other.UseOptimalColumnWidth) && this.ColumnWidth.Equals(other.ColumnWidth))
				return true;
			return false;
		}
		public override int GetHashCode() {
			return ColumnWidth.GetHashCode();
		}
		public OdsColumnFormat Clone() {
			return new OdsColumnFormat() { ColumnWidth = this.ColumnWidth, UseOptimalColumnWidth = this.UseOptimalColumnWidth };
		}
	}
	public class OdsTableFormat {
		#region Static
		public static OdsTableFormat GetFormat(Worksheet sheet) {
			return new OdsTableFormat();
		}
		#endregion
		#region Properties
		public int MarginInfoIndex { get; set; }
		public XlReadingOrder ReadingOrder { get; set; } 
		#endregion
		#region Import
		public void ReadAttributes(XmlReader reader, DevExpress.XtraSpreadsheet.Import.OpenDocument.OpenDocumentWorkbookImporter importer) {
			MarginsInfo marginsInfo = importer.DocumentModel.Cache.MarginInfoCache[MarginInfoIndex].Clone();
			marginsInfo.Bottom = ReadMarginCore(reader, importer, "fo:margin-bottom", marginsInfo.Bottom);
			marginsInfo.Left = ReadMarginCore(reader, importer, "fo:margin-left", marginsInfo.Left);
			marginsInfo.Right = ReadMarginCore(reader, importer, "fo:margin-right", marginsInfo.Right);
			marginsInfo.Top = ReadMarginCore(reader, importer, "fo:margin-top", marginsInfo.Top);
			MarginInfoIndex = importer.DocumentModel.Cache.MarginInfoCache.GetItemIndex(marginsInfo);
		}
		int ReadMarginCore(XmlReader reader, DevExpress.XtraSpreadsheet.Import.OpenDocument.OpenDocumentWorkbookImporter importer, string attributeName, int defaultValue) {
			string margin = importer.ReadAttribute(reader, attributeName);
			if (string.IsNullOrEmpty(margin) || margin.Contains("%"))
				return defaultValue;
			int marginValue = (int)importer.ParseLength(margin, defaultValue);
			return marginValue >= 0 ? marginValue : defaultValue;
		}
		public void ApplyFormatOnSheet(Worksheet sheet) {
			sheet.Properties.Margins.SetIndexInitial(MarginInfoIndex);
		}
		#endregion
		#region Export
		protected internal void Generate(OpenDocumentExporter exporter) {
			exporter.WriteStartElement("table-properties", OpenDocumentExporter.StyleNamespace);
			try {
				exporter.WriteStringAttr("writing-mode", OpenDocumentExporter.StyleNamespace, "lr-tb");
				exporter.WriteStringAttr("display", OpenDocumentExporter.TableNamespace, "true");
			}
			finally {
				exporter.WriteEndElement();
			}
		}
		#endregion
		public OdsTableFormat Clone() {
			return new OdsTableFormat() { MarginInfoIndex = this.MarginInfoIndex };
		}
	}
	#region ExportStyleSheet
	public class ExportStyleSheet {
		#region Fields
		readonly Dictionary<int, int> cellFormatTable = new Dictionary<int, int>();
		readonly Dictionary<int, int> cellStyleIndexTable = new Dictionary<int, int>();
		readonly Dictionary<int, int> cellStyleFormatTable = new Dictionary<int, int>();
		readonly Dictionary<int, int> differentialFormatTable = new Dictionary<int, int>();
		readonly Dictionary<int, int> numberFormatTable = new Dictionary<int, int>();
		readonly Dictionary<string, int> fontNameTable = new Dictionary<string, int>();
		readonly Dictionary<OdsColumnFormat, int> columnFormatTable = new Dictionary<OdsColumnFormat, int>();
		readonly Dictionary<OdsRowFormat, int> rowFormatTable = new Dictionary<OdsRowFormat, int>();
		readonly Dictionary<OdsTableFormat, int> tableFormatTable = new Dictionary<OdsTableFormat, int>();
		readonly OdsConditionalFormattingExportCollection conditionalFormattings = new OdsConditionalFormattingExportCollection();
		readonly DocumentModel workbook;
		#endregion
		#region Properties
		protected internal Dictionary<int, int> CellFormatTable { get { return cellFormatTable; } }
		protected internal Dictionary<int, int> CellStyleIndexTable { get { return cellStyleIndexTable; } }
		protected internal Dictionary<int, int> CellStyleFormatTable { get { return cellStyleFormatTable; } }
		protected internal Dictionary<int, int> DifferentialFormatTable { get { return differentialFormatTable; } }
		protected internal Dictionary<int, int> NumberFormatTable { get { return numberFormatTable; } }
		protected internal Dictionary<string, int> FontNameTable { get { return fontNameTable; } }
		protected internal Dictionary<OdsColumnFormat, int> ColumnFormatTable { get { return columnFormatTable; } }
		protected internal Dictionary<OdsRowFormat, int> RowFormatTable { get { return rowFormatTable; } }
		protected internal Dictionary<OdsTableFormat, int> TableFormatTable { get { return tableFormatTable; } }
		protected internal OdsConditionalFormattingExportCollection ConditionalFormattings { get { return conditionalFormattings; } }
		protected internal DocumentModel Workbook { get { return workbook; } }
		#endregion
		public ExportStyleSheet(DocumentModel workbook) {
			Guard.ArgumentNotNull(workbook, "workbook");
			this.workbook = workbook;
			InitializeInfoTables(workbook.StyleSheet);
		}
		void InitializeInfoTables(StyleSheet styleSheet) {
			BuiltInCellStyle normalStyle = (BuiltInCellStyle)styleSheet.CellStyles.Normal;
			CellFormatTable.Add(CellFormatCache.DefaultItemIndex, 0);
			CellStyleIndexTable.Add(0, 0);
			cellStyleFormatTable.Add(CellFormatCache.DefaultCellStyleFormatIndex, 0);
			DifferentialFormatTable.Add(CellFormatCache.DefaultDifferentialFormatIndex, 0);
			NumberFormatTable.Add(normalStyle.FormatInfo.NumberFormatIndex, 0);
			fontNameTable.Add(normalStyle.FormatInfo.Font.Name, 0);
		}
		protected internal virtual CellRange GetMaximumCellRange(IWorksheet sheet) {
			Worksheet worksheet = sheet as Worksheet;
			CellPosition topLeft = new CellPosition(0, 0);
			CellPosition bottomRight = new CellPosition(worksheet.MaxColumnCount - 1, worksheet.MaxRowCount - 1);
			return new CellRange(sheet, topLeft, bottomRight);
		}
		public virtual void RegisterStyles() {
			RegisterCellStylesAndFormats();
			RegisterConditionalFormats();
			RegisterNumberFormats();
			RegisterFonts();
			RegisterColumnFormats();
			RegisterRowFormats();
			RegisterTableFormats();
		}
		protected void RegisterItem<T>(Dictionary<T, int> collection, T key) {
			if (!collection.ContainsKey(key))
				collection.Add(key, collection.Count);
		}
		#region RegisterCellStylesAndFormats
		protected internal virtual void RegisterCellStylesAndFormats() {
			RegisterItem(CellFormatTable, workbook.StyleSheet.DefaultCellFormatIndex);
			RegisterFormats();
			RegisterReferencedCellStyle();
			RegisterNotHiddenCustomCellStyle();
			RegisterHiddenOrCustomBuiltInStyles();
			RegisterCellStyleFormats();
		}
		#region RegisterFormats
		protected internal virtual void RegisterFormats() {
			for (int i = 0; i < workbook.Sheets.Count; i++) {
				Worksheet sheet = workbook.Sheets[i];
				sheet.Columns.ForEach(RegisterColumnCellFormatIndex);
				sheet.Rows.ForEach(RegisterRowCellFormatIndex);
				RegisterCellFormatIndex(sheet);
				RegisterCellFormatIndex(sheet.Tables);
				RegisterDifferentialFormatIndex(sheet);
			}
		}
		void RegisterRowCellFormatIndex(Row row) {
			RegisterCellFormatIndex(row.FormatIndex);
		}
		void RegisterColumnCellFormatIndex(IColumnRange column) {
			RegisterCellFormatIndex(column.FormatIndex);
		}
		void RegisterCellFormatIndex(int index) {
			if (index >= workbook.StyleSheet.DefaultCellFormatIndex)
				RegisterItem(CellFormatTable, index);
		}
		#region RegisterCellFormatIndex
		void RegisterCellFormatIndex(Worksheet sheet) {
			CellRange range = GetMaximumCellRange(sheet);
			foreach (ICellBase info in range.GetExistingCellsEnumerable()) {
				ICell cell = info as ICell;
				if (cell != null)
					RegisterCellFormatIndex(cell.FormatIndex);
			}
		}
		void RegisterCellFormatIndex(TableCollection tables) {
			int count = tables.Count;
			for (int i = 0; i < count; i++) {
				Table table = tables[i];
				for (int j = 0; j < Table.CellFormatElementCount; j++)
					RegisterCellFormatIndex(table.CellFormatIndexes[j]);
				RegisterCellFormatIndex(table.Columns);
			}
		}
		void RegisterCellFormatIndex(TableColumnInfoCollection columns) {
			int count = columns.Count;
			for (int i = 0; i < count; i++) {
				TableColumn column = columns[i];
				for (int j = 0; j < TableColumn.ElementCount; j++)
					RegisterCellFormatIndex(column.CellFormatIndexes[j]);
			}
		}
		#endregion
		#region RegisterDifferentialFormatIndex
		protected internal virtual void RegisterDifferentialFormatIndex(Worksheet sheet) {
			RegisterDifferentialFormatIndex(sheet.Tables);
			RegisterDifferentialFormatIndex(sheet.AutoFilter);
			RegisterDifferentialFormatIndex(sheet.ConditionalFormattings);
		}
		protected internal void RegisterDifferentialFormatIndex(TableCollection tables) {
			for (int i = 0; i < tables.Count; i++) {
				Table table = tables[i];
				RegisterDifferentialFormatIndex(table);
				RegisterDifferentialFormatIndex(table.Columns);
				RegisterDifferentialFormatIndex(table.AutoFilter);
			}
		}
		protected internal void RegisterDifferentialFormatIndex(Table table) {
			for (int i = 0; i < Table.DifferentialFormatElementCount; i++) {
				RegisterDifferentialFormatIndex(DifferentialFormatTable, table.DifferentialFormatIndexes[i]);
				RegisterDifferentialFormatIndex(DifferentialFormatTable, table.BorderFormatIndexes[i]);
			}
		}
		protected internal void RegisterDifferentialFormatIndex(TableColumnInfoCollection columns) {
			for (int i = 0; i < columns.Count; i++)
				RegisterDifferentialFormatIndex(columns[i]);
		}
		protected internal void RegisterDifferentialFormatIndex(TableColumn column) {
			for (int i = 0; i < TableColumn.ElementCount; i++)
				RegisterDifferentialFormatIndex(DifferentialFormatTable, column.DifferentialFormatIndexes[i]);
		}
		protected internal void RegisterDifferentialFormatIndex(ConditionalFormattingCollection conditionalFormattings) {
			for (int i = 0; i < conditionalFormattings.Count; i++)
				RegisterDifferentialFormatIndex(DifferentialFormatTable, conditionalFormattings[i].DifferentialFormatIndex);
		}
		protected internal void RegisterDifferentialFormatIndex(AutoFilterBase autoFilter) {
			AutoFilterColumnCollection filterColumns = autoFilter.FilterColumns;
			int count = filterColumns.Count;
			for (int i = 0; i < count; i++)
				RegisterDifferentialFormatIndex(DifferentialFormatTable, filterColumns[i].FormatIndex);
		}
		void RegisterDifferentialFormatIndex(Dictionary<int, int> collection, int index) {
			if (CellFormatCache.DefaultDifferentialFormatIndex != index)
				RegisterItem(collection, index);
		}
		#endregion
		#endregion
		#region RegisterReferencedCellStyle
		protected internal void RegisterReferencedCellStyle() {
			foreach (int index in CellFormatTable.Keys) {
				CellFormat cellFormat = (CellFormat)workbook.Cache.CellFormatCache[index];
				RegisterItem(CellStyleIndexTable, cellFormat.Style.StyleIndex);
			}
		}
		#endregion
		#region RegisterNotHiddenCustomCellStyle
		protected internal void RegisterNotHiddenCustomCellStyle() {
			CellStyleCollection cellStyles = workbook.StyleSheet.CellStyles;
			int count = cellStyles.Count;
			for (int i = 0; i < count; i++) {
				CustomCellStyle cellStyle = cellStyles[i] as CustomCellStyle;
				if (cellStyle != null && !cellStyle.IsHidden)
					RegisterItem(CellStyleIndexTable, cellStyle.StyleIndex);
			}
		}
		#endregion
		#region RegisterHiddenOrCustomBuiltInStyles
		protected internal void RegisterHiddenOrCustomBuiltInStyles() {
			CellStyleCollection cellStyles = workbook.StyleSheet.CellStyles;
			int count = cellStyles.Count;
			for (int i = 0; i < count; i++) {
				CellStyleBase cellStyle = cellStyles[i];
				if (IsCustomOrHiddenBuiltInCellStyle(cellStyle))
					RegisterItem(CellStyleIndexTable, cellStyle.StyleIndex);
			}
		}
		bool IsCustomOrHiddenBuiltInCellStyle(CellStyleBase cellStyle) {
			BuiltInCellStyle builtInCellStyle = cellStyle as BuiltInCellStyle;
			if (builtInCellStyle != null)
				return builtInCellStyle.CustomBuiltIn || builtInCellStyle.IsHidden;
			OutlineCellStyle outlineCellStyle = cellStyle as OutlineCellStyle;
			if (outlineCellStyle != null)
				return outlineCellStyle.CustomBuiltIn || outlineCellStyle.IsHidden;
			return false;
		}
		#endregion
		#region RegisterCellStyleFormats
		protected internal virtual void RegisterCellStyleFormats() {
			foreach (int index in cellStyleIndexTable.Keys) {
				CellStyleBase style = workbook.StyleSheet.CellStyles[index];
				RegisterItem(cellStyleFormatTable, style.FormatIndex);
			}
		}
		#endregion
		#endregion
		#region RegisterConditionalFormats
		protected internal void RegisterConditionalFormats() {
			workbook.Sheets.ForEach(sheet => {
				foreach (ConditionalFormatting format in sheet.ConditionalFormattings) {
					CellRangeBase range = format.CellRangeInternalNoHistory;
					if (range == null)
						return;
					IEnumerator<ICellBase> cellEnumerator = range.GetAllCellsEnumerator(); 
					while (cellEnumerator.MoveNext())
						conditionalFormattings.Register((ICell)cellEnumerator.Current, format);
				}
			});
		}
		#endregion
		#region RegisterFonts
		protected internal virtual void RegisterFonts() {
			RegisterFont(cellFormatTable);
			RegisterFont(cellStyleFormatTable);
			RegisterFont(differentialFormatTable);
		}
		void RegisterFont(Dictionary<int, int> collection) {
			foreach (int index in collection.Keys)
				RegisterItem(fontNameTable, workbook.Cache.CellFormatCache[index].Font.Name);
		}
		#endregion
		#region RegisterNumberFormats
		protected internal virtual void RegisterNumberFormats() {
			RegisterNumberFormat(cellFormatTable);
			RegisterNumberFormat(cellStyleFormatTable);
			RegisterNumberFormat(differentialFormatTable);
		}
		void RegisterNumberFormat(Dictionary<int, int> collection) {
			foreach (int index in collection.Keys)
				RegisterItem(NumberFormatTable, workbook.Cache.CellFormatCache[index].NumberFormatIndex);
		}
		#endregion
		#region RegisterColumnFormats
		protected internal virtual void RegisterColumnFormats() {
			foreach (Worksheet sheet in Workbook.Sheets) {
				Column defaultColumn = new Column(sheet, 0, 0);
				OdsColumnFormat defaultSheetFormat = OdsColumnFormat.GetFormat(defaultColumn);
				RegisterItem(columnFormatTable, defaultSheetFormat);
				sheet.Columns.ForEach(delegate(Column column) {
					RegisterItem(columnFormatTable, OdsColumnFormat.GetFormat(column));
				});
			}
		}
		#endregion
		#region RegisterRowFormats
		protected internal virtual void RegisterRowFormats() {
			foreach (Worksheet sheet in Workbook.Sheets) {
				Row defaultRow = new Row(0, sheet);
				OdsRowFormat defaultSheetFormat = OdsRowFormat.GetFormat(defaultRow);
				RegisterItem(rowFormatTable, defaultSheetFormat);
				sheet.Rows.ForEach(delegate(Row row) {
					RegisterItem(rowFormatTable, OdsRowFormat.GetFormat(row));
				});
			}
		}
		#endregion
		#region RegisterTableFormats
		protected internal virtual void RegisterTableFormats() {
			workbook.Sheets.ForEach(sheet => RegisterItem(tableFormatTable, OdsTableFormat.GetFormat(sheet)));
		}
		#endregion
		#region GetFormatNames
		public string GetNumberFormatName(int index) {
			return string.Concat("N", NumberFormatTable[index]);
		}
		public string GetCellFormatName(int index) {
			return string.Concat("ce", CellFormatTable[index]);
		}
		public string GetCellStyleName(CellStyleBase style) {
			return style.Name.Equals("Normal") ? "Default" : style.Name;
		}
		public string GetColumnFormatName(OdsColumnFormat format) {
			return string.Concat("co", columnFormatTable[format]);
		}
		public string GetRowFormatName(OdsRowFormat format) {
			return string.Concat("ro", rowFormatTable[format]);
		}
		public string GetTableFormatName(OdsTableFormat format) {
			return string.Concat("ta", tableFormatTable[format]);
		}
		public string GetConditionalFormatName(OdsConditionalFormattingExport format) {
			return string.Concat("cecf", format.ConditionalFormatIndex);
		}
		public string GetDifferentialFormatName(int index) {
			return string.Concat("cf", differentialFormatTable[index]);
		}
		public string GetDefaultColumnFormatName(Worksheet sheet) {
			return GetColumnFormatName(OdsColumnFormat.GetFormat(new Column(sheet, 0, 0)));
		}
		public string GetDefaultRowFormatName(Worksheet sheet) {
			return GetRowFormatName(OdsRowFormat.GetFormat(new Row(0, sheet)));
		}
		public string GetColumnCellStyleName(Column column) {
			OdsConditionalFormattingExport conditionalFormatting = conditionalFormattings.GetConditionalFormatting(column.GetCellIntervalRange());
			return conditionalFormatting == null ? GetCellFormatName(column.FormatIndex) : GetConditionalFormatName(conditionalFormatting);
		}
		public string GetCellStyleName(ICell cell) {
			OdsConditionalFormattingExport conditionalFormatting = conditionalFormattings.GetConditionalFormatting(cell);
			string result = conditionalFormatting == null ? GetCellFormatName(cell.FormatIndex) : GetConditionalFormatName(conditionalFormatting);
			string columnCellStyleName = GetColumnCellStyleName(cell.Worksheet.Columns.GetColumnRangeForReading(cell.ColumnIndex));
			return columnCellStyleName.Equals(result, StringComparison.Ordinal) ? null : result;
		}
		#endregion
	}
	#endregion
}
#endif
