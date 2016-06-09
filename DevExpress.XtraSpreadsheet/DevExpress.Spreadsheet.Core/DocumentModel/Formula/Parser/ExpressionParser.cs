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
using System.Drawing;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model.External;
using System.Collections;
using System.Globalization;
using DevExpress.Export.Xl;
using DevExpress.XtraExport;
using DevExpress.Compatibility.System.Drawing;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	#region ExpressionParser
	public class ExpressionParser : IDisposable {
		#region Fields
		readonly WorkbookDataContext context;
		readonly ReferenceParser referenceParser;
		#endregion
		public ExpressionParser(WorkbookDataContext context) {
			Guard.ArgumentNotNull(context, "context");
			this.context = context;
			referenceParser = new ReferenceParser();
		}
		#region Properties
		public WorkbookDataContext Context { get { return context; } }
		#endregion
		public ParsedExpression Parse(string formula, OperandDataType dataType, bool allowsToCreateExternal) {
			if (formula[0] != '=' || formula.Length < 2)
				Exceptions.ThrowArgumentException("formula", formula);
			ExpressionParserContext parserContext = new ExpressionParserContext(context, allowsToCreateExternal);
			ParseCore(formula, dataType, parserContext);
			return parserContext.GetPreparedResult();
		}
		public IncompleteExpressionParserContext ParseIncomplete(string formula, OperandDataType dataType) {
			if (formula.Length < 2 || formula[0] != '=')
				return null;
			IncompleteExpressionParserContext parserContext = new IncompleteExpressionParserContext(context);
			ParseCore(formula, dataType, parserContext);
			return parserContext;
		}
		void ParseCore(string formula, OperandDataType dataType, IExpressionParserContext parserContext) {
			try {
				referenceParser.Parse(formula.Remove(0, 1), dataType, parserContext);
			}
			catch {
				parserContext.ExceptionOccurred = true;
			}
		}
		public string TryRepairFormula(string formula, OperandDataType dataType, IncompleteExpressionParserContext previousContext) {
			if (previousContext == null || previousContext.Suggestions == null || previousContext.Suggestions.Count <= 0)
				return formula;
			string correctedFormula = formula.Remove(0, 1);
			for (int i = previousContext.Suggestions.Count - 1; i >= 0; i--) {
				ParserSuggestion suggestion = previousContext.Suggestions[i];
				string suggestionCorrected = suggestion.TryApplyCorrection(correctedFormula);
				if (string.IsNullOrEmpty(suggestionCorrected))
					return formula;
				correctedFormula = suggestionCorrected;
			}
			correctedFormula = "=" + correctedFormula;
			IncompleteExpressionParserContext correctedContext = ParseIncomplete(correctedFormula, dataType);
			if (correctedContext != null && correctedContext.ParsingSuccessful)
				return correctedFormula;
			return formula;
		}
		#region IDisposable Members
		public void Dispose() {
			referenceParser.Dispose();
		}
		#endregion
	}
	#endregion
	#region IExpressionParserContext
	public interface IExpressionParserContext {
		ParserErrors ErrorList { get; }
		ParsedExpression Result { get; set; }
		bool ExceptionOccurred { get; set; }
		char DecimalSymbol { get; }
		char ListSeparator { get; }
		bool AllowsToCreateExternal { get; }
		int RegisterExternalLink(string sheetNameStart, string sheetNameEnd, int externalIndex, string externalName);
		void RegisterCellRange(CellRange range, int sheetDefinitionIndex, int position, int length);
		void RegisterDefinedName(ParsedThingName definedName, int position, int length);
		void UnRegisterDefinedName(ParsedThingName definedName);
		void RegisterTableReference(ParsedThingTable ptgTable, int position, int length);
		void RegisterSuggestion(ParserSuggestion suggestion);
		void RegisterFunctionCall(ParsedThingFunc function, int position, int length, List<FunctionParameterParsedData> parameters);
		bool UseR1C1ReferenceStyle { get; }
		bool DefinedNameProcessing { get; }
		bool ArrayFormulaProcessing { get; }
		bool RelativeToCurrentCell { get; }
		bool ImportExportMode { get; }
		int CurrentColumnIndex { get; }
		int CurrentRowIndex { get; }
		CultureInfo Culture { get; }
		Table GetTable(string name);
		Table GetCurrentTable();
		bool HasDefinedName(string name, SheetDefinition sheetDefinition);
		bool HasSheetScopedDefinedName(string name);
		ICellError CreateError(string name);
		ISpreadsheetFunction GetFunctionByName(string name);
		bool SheetDefinitionRefersToDdeWorkbook(int sheetDefinitionIndex);
		bool TryParseBoolean(string elementToParse, out bool boolResult);
		int RegisterSheetDefinition(SheetDefinition sheetDefinition);
		SheetDefinition GetSheetDefinition(int sheetDefinitionIndex);
		bool IsLocalWorkbookProcessing();
		string GetCurrentSheetName();
		bool IsCurrentWorkbookContainsSheet(string sheetName);
		ExternalLinkInfo GetExternalLinkByName(string name);
	}
	#endregion
	#region ExpressionParserContext
	public class ExpressionParserContext : IExpressionParserContext {
		#region Fields
		readonly WorkbookDataContext context;
		readonly List<ExternalLink> createdExternalLinks;
		readonly ParserErrors errorList;
		ParsedExpression result;
		readonly bool allowsToCreateExternal;
		#endregion
		public ExpressionParserContext(WorkbookDataContext context, bool allowsToCreateExternal) {
			Guard.ArgumentNotNull(context, "context");
			this.context = context;
			errorList = new ParserErrors();
			createdExternalLinks = new List<ExternalLink>();
			this.allowsToCreateExternal = allowsToCreateExternal;
			result = new ParsedExpression();
		}
		#region Properties
		public ParserErrors ErrorList { get { return errorList; } }
		public ParsedExpression Result { get { return result; } set { result = value; } }
		public bool ExceptionOccurred { get; set; }
		public char DecimalSymbol { get { return context.GetDecimalSymbol(); } }
		public char ListSeparator { get { return context.GetListSeparator(); } }
		public WorkbookDataContext Context { get { return context; } }
		public bool AllowsToCreateExternal { get { return allowsToCreateExternal; } }
		public bool UseR1C1ReferenceStyle { get { return Context.UseR1C1ReferenceStyle; } }
		public bool DefinedNameProcessing { get { return Context.DefinedNameProcessing; } }
		public bool ArrayFormulaProcessing { get { return Context.ArrayFormulaProcessing; } }
		public bool RelativeToCurrentCell { get { return Context.RelativeToCurrentCell || Context.SharedFormulaProcessing; } }
		public int CurrentColumnIndex { get { return Context.CurrentColumnIndex; } }
		public int CurrentRowIndex { get { return Context.CurrentRowIndex; } }
		public bool ImportExportMode { get { return Context.ImportExportMode; } }
		public CultureInfo Culture { get { return Context.Culture; } }
		#endregion
		public void PushCreatedExternalLinks() {
			foreach (ExternalLink link in createdExternalLinks)
				context.Workbook.ExternalLinks.Add(link);
		}
		internal ParsedExpression GetPreparedResult() {
			if (ExceptionOccurred || result == null || errorList.Count > 0 || result.Count == 0)
				return null;
			PushCreatedExternalLinks();
			return result;
		}
		public int RegisterExternalLink(string sheetNameStart, string sheetNameEnd, int externalIndex, string externalName) {
			ExternalLink externalLink = null;
			if (TryGetExistingExternalLink(externalIndex, out externalLink)) {
				AddReferencedWorksheets(externalLink, sheetNameStart, sheetNameEnd);
				return externalIndex;
			}
			bool existingInWorkbookExternalNames = TryGetExistingExternalLinkByFilePath(externalName, out externalLink);
			if (existingInWorkbookExternalNames) {
				AddReferencedWorksheets(externalLink, sheetNameStart, sheetNameEnd);
				return context.Workbook.ExternalLinks.IndexOf(externalLink) + 1;
			}
			else {
				int indexInCreatedExternalLinksCollection = 0;
				if (TryFindInCreatedExternalLink(externalName, out indexInCreatedExternalLinksCollection))
					return context.Workbook.ExternalLinks.Count + indexInCreatedExternalLinksCollection + 1;
				else {
					if (context.ImportExportMode && externalName == null)
						throw new Exception("Non existing external workbook.");
					externalLink = CreateExternalLinkNotAdd(context.Workbook, externalName);
					createdExternalLinks.Add(externalLink);
					CreateExternalWorksheets(externalLink, sheetNameStart, sheetNameEnd);
					return context.Workbook.ExternalLinks.Count + createdExternalLinks.Count;
				}
			}
		}
		void AddReferencedWorksheets(ExternalLink externalLink, string sheetNameStart, string sheetNameEnd) {
			if (!AllowsToCreateExternal)
				return;
			CreateExternalWorksheets(externalLink, sheetNameStart, sheetNameEnd);
		}
		void CreateExternalWorksheets(ExternalLink externalLink, string sheetNameStart, string sheetNameEnd) {
			ExternalWorksheetCollection externalLinkWorkbookSheets = externalLink.Workbook.Sheets;
			if (!string.IsNullOrEmpty(sheetNameStart) && externalLinkWorkbookSheets.GetSheetIndexByName(sheetNameStart) < 0) {
				ExternalWorksheet sheetStart = new ExternalWorksheet(externalLink.Workbook, sheetNameStart);
				sheetStart.RefreshFailed = true;
				externalLinkWorkbookSheets.Add(sheetStart);
			}
			if (!string.IsNullOrEmpty(sheetNameEnd) && externalLinkWorkbookSheets.GetSheetIndexByName(sheetNameStart) < 0) {
				ExternalWorksheet sheetEnd = new ExternalWorksheet(externalLink.Workbook, sheetNameEnd);
				sheetEnd.RefreshFailed = true;
				externalLinkWorkbookSheets.Add(sheetEnd);
			}
		}
		ExternalLink CreateExternalLinkNotAdd(DocumentModel documentModel, string filePath) {
			ExternalLink result = new ExternalLink(documentModel);
			result.Workbook.FilePath = filePath;
			return result;
		}
		bool TryGetExistingExternalLinkByFilePath(string externalName, out ExternalLink externalLink) {
			externalLink = context.Workbook.ExternalLinks[externalName];
			return externalLink != null;
		}
		bool TryGetExistingExternalLink(int externalIndex, out ExternalLink link) {
			link = null;
			if (externalIndex <= 0)
				return false;
			int registeredExternalLinksCount = context.Workbook.ExternalLinks.Count;
			if (externalIndex <= registeredExternalLinksCount) {
				link = context.Workbook.ExternalLinks[externalIndex - 1];
				return true;
			}
			else {
				if (createdExternalLinks.Count >= externalIndex - registeredExternalLinksCount) {
					link = createdExternalLinks[externalIndex - registeredExternalLinksCount - 1];
					return true;
				}
			}
			return false;
		}
		bool TryFindInCreatedExternalLink(string externalName, out int index) {
			index = Int32.MinValue;
			for (int i = 0; i < createdExternalLinks.Count; i++)
				if (StringExtensions.CompareInvariantCultureIgnoreCase(createdExternalLinks[i].Workbook.FilePath, externalName) == 0) {
					index = i;
					return true;
				}
			return false;
		}
		public virtual void RegisterCellRange(CellRange range, int sheetDefinitionIndex, int position, int length) {
		}
		public virtual void RegisterDefinedName(ParsedThingName definedName, int position, int length) {
		}
		public virtual void UnRegisterDefinedName(ParsedThingName definedName) {
		}
		public virtual void RegisterTableReference(ParsedThingTable ptgTable, int position, int length) {
		}
		public virtual void RegisterFunctionCall(ParsedThingFunc function, int position, int length, List<FunctionParameterParsedData> parameters) {
		}
		public virtual void RegisterSuggestion(ParserSuggestion suggestion) {
		}
		public Table GetTable(string name) {
			return Context.GetDefinedTableRange(name);
		}
		public Table GetCurrentTable() {
			return context.GetCurrentTable();
		}
		public bool HasDefinedName(string name, SheetDefinition sheetDefinition) {
			return Context.GetDefinedName(name, sheetDefinition) != null;
		}
		public bool HasSheetScopedDefinedName(string name) {
			DefinedNameBase instance = Context.GetDefinedName(name, null);
			return instance != null && instance.ScopedSheetId >= 0;
		}
		public ICellError CreateError(string name) {
			return CellErrorFactory.CreateError(name, Context);
		}
		public ISpreadsheetFunction GetFunctionByName(string name) {
			return FormulaCalculator.GetFunctionByName(name, Context);
		}
		public bool SheetDefinitionRefersToDdeWorkbook(int sheetDefinitionIndex) {
			SheetDefinition sheetDefinition = Context.GetSheetDefinition(sheetDefinitionIndex);
			if (!sheetDefinition.Is3DReference) {
				External.DdeExternalWorkbook ddeExternalBook = sheetDefinition.GetWorkbook(Context) as External.DdeExternalWorkbook;
				if (ddeExternalBook != null)
					return true;
			}
			return false;
		}
		public bool TryParseBoolean(string elementToParse, out bool boolResult) {
			return Context.TryParseBoolean(elementToParse, out boolResult);
		}
		public int RegisterSheetDefinition(SheetDefinition sheetDefinition) {
			return Context.RegisterSheetDefinition(sheetDefinition);
		}
		public SheetDefinition GetSheetDefinition(int sheetDefinitionIndex) {
			return Context.GetSheetDefinition(sheetDefinitionIndex);
		}
		public bool IsLocalWorkbookProcessing() {
			return Context.CurrentWorkbook is DocumentModel;
		}
		public string GetCurrentSheetName() {
			IWorksheet currentWorksheet = Context.CurrentWorksheet;
			if (currentWorksheet == null)
				return string.Empty;
			return currentWorksheet.Name;
		}
		public bool IsCurrentWorkbookContainsSheet(string sheetName) {
			return Context.CurrentWorkbook.GetSheetByName(sheetName) != null;
		}
		public ExternalLinkInfo GetExternalLinkByName(string name) {
			return Context.Workbook.ExternalLinks.GetInfoByName(name);
		}
	}
	#endregion
	#region IncompleteExpressionParserContext
	public class IncompleteExpressionParserContext : ExpressionParserContext {
		#region Fields
		readonly List<CellRangeItemParsedData> cellRangesItems;
		readonly List<DefinedNameItemParsedData> definedNamesItems;
		readonly List<TableItemParsedData> tableItems;
		readonly List<ParserSuggestion> suggestions;
		readonly List<FunctionCallItemParsedData> functionCalls;
		#endregion
		public IncompleteExpressionParserContext(WorkbookDataContext context)
			: base(context, false) {
			cellRangesItems = new List<CellRangeItemParsedData>();
			definedNamesItems = new List<DefinedNameItemParsedData>();
			tableItems = new List<TableItemParsedData>();
			suggestions = new List<ParserSuggestion>();
			functionCalls = new List<FunctionCallItemParsedData>();
		}
		#region Properties
		public List<CellRangeItemParsedData> CellRanges { get { return cellRangesItems; } }
		public List<DefinedNameItemParsedData> DefinedNames { get { return definedNamesItems; } }
		public List<TableItemParsedData> TableItems { get { return tableItems; } }
		public List<ParserSuggestion> Suggestions { get { return suggestions; } }
		public List<FunctionCallItemParsedData> FunctionCalls { get { return functionCalls; } }
		public bool ParsingSuccessful { get { return !ExceptionOccurred && ErrorList.Count <= 0; } }
		#endregion
		public override void RegisterCellRange(CellRange range, int sheetDefinitionIndex, int position, int length) {
			if (sheetDefinitionIndex >= 0) {
				SheetDefinition sheetDefinition = Context.GetSheetDefinition(sheetDefinitionIndex);
				if (sheetDefinition == null || sheetDefinition.IsExternalReference)
					return;
				List<IWorksheet> sheets = sheetDefinition.GetReferencedSheets(Context);
				if (sheets == null || !sheets.Contains(Context.CurrentWorksheet))
					return;
			}
			CellRange registeringRange = (CellRange)range.Clone(Context.CurrentWorksheet);
			CellRangeItemParsedData itemParsedData = new CellRangeItemParsedData(registeringRange, position + 1, length);
			this.cellRangesItems.Add(itemParsedData);
		}
		public override void RegisterDefinedName(ParsedThingName definedName, int position, int length) {
			DefinedNameItemParsedData itemParsedData = new DefinedNameItemParsedData(definedName, position + 1, length);
			this.definedNamesItems.Add(itemParsedData);
		}
		public override void UnRegisterDefinedName(ParsedThingName definedName) {
			for (int i = definedNamesItems.Count - 1; i >= 0; i--) {
				if (Object.ReferenceEquals(definedNamesItems[i].DefinedName, definedName)) {
					definedNamesItems.RemoveAt(i);
					return;
				}
			}
		}
		public override void RegisterTableReference(ParsedThingTable ptgTable, int position, int length) {
			TableItemParsedData itemParsedData = new TableItemParsedData(ptgTable, position + 1, length);
			this.tableItems.Add(itemParsedData);
		}
		public override void RegisterFunctionCall(ParsedThingFunc ptgFunc, int position, int length, List<FunctionParameterParsedData> parameters) {
			functionCalls.Add(new FunctionCallItemParsedData(ptgFunc, position + 1, length, parameters));
		}
		public override void RegisterSuggestion(ParserSuggestion suggestion) {
			this.suggestions.Add(suggestion);
		}
		public FormulaReferencedRanges GetReferencedRanges() {
			FormulaReferencedRanges result = new FormulaReferencedRanges();
			foreach (CellRangeItemParsedData parsedData in cellRangesItems)
				result.Add(parsedData.GetReferencedRange(Context));
			foreach (DefinedNameItemParsedData parsedData in definedNamesItems) {
				FormulaReferencedRange range = parsedData.GetReferencedRange(Context);
				if (range != null)
					result.Add(range);
			}
			foreach (TableItemParsedData parsedData in tableItems) {
				FormulaReferencedRange range = parsedData.GetReferencedRange(Context);
				if (range != null)
					result.Add(range);
			}
			result.Sort();
			return result;
		}
	}
	#endregion
	#region IRangeParsedData
	public interface IRangeParsedData {
		CellRangeBase GetCellRange(WorkbookDataContext context);
		FormulaReferencedRange GetReferencedRange(WorkbookDataContext context);
	}
	#endregion
	#region ItemParsedData
	public abstract class ItemParsedData : IComparable<ItemParsedData>, IEquatable<ItemParsedData> {
		#region Fields
		int position;
		int length;
		#endregion
		protected ItemParsedData(int position, int length) {
			this.position = position;
			this.length = length;
		}
		#region Properties
		public int Position { get { return position; } }
		public int Length { get { return length; } }
		#endregion
		#region IComparable<ItemParsedData> Members
		public int CompareTo(ItemParsedData other) {
			return Position.CompareTo(other.Position);
		}
		#endregion
		public bool Equals(ItemParsedData other) {
			if (other == null)
				return false;
			return Position == other.Position && Length == other.Length;
		}
	}
	public class CellRangeItemParsedData : ItemParsedData, IRangeParsedData {
		#region Fields
		CellRange range;
		#endregion
		public CellRangeItemParsedData(CellRange range, int position, int length)
			: base(position, length) {
			this.range = range;
		}
		#region Properties
		public CellRange Range { get { return range; } }
		#endregion
		public CellRangeBase GetCellRange(WorkbookDataContext context) {
			return range;
		}
		public FormulaReferencedRange GetReferencedRange(WorkbookDataContext context) {
			return new FormulaReferencedRange(range, Position, Length, false);
		}
	}
	public class DefinedNameItemParsedData : ItemParsedData, IRangeParsedData {
		#region Fields
		ParsedThingName definedName;
		#endregion
		public DefinedNameItemParsedData(ParsedThingName definedName, int position, int length)
			: base(position, length) {
			this.definedName = definedName;
		}
		#region Properties
		public ParsedThingName DefinedName { get { return definedName; } set { definedName = value; } }
		#endregion
		public CellRangeBase GetCellRange(WorkbookDataContext context) {
			SheetDefinition sheetDefinition = null;
			ParsedThingNameX nameX = definedName as ParsedThingNameX;
			if (nameX != null)
				sheetDefinition = context.GetSheetDefinition(nameX.SheetDefinitionIndex);
			DefinedNameBase definedNameObject = context.GetDefinedName(definedName.DefinedName, sheetDefinition);
			if (definedNameObject == null)
				return null;
			ReferencesCache referencesCache = definedNameObject.GetReferencesCache(context);
			if (referencesCache == null)
				return null;
			return referencesCache.Calculate(context);
		}
		public FormulaReferencedRange GetReferencedRange(WorkbookDataContext context) {
			CellRangeBase range = GetCellRange(context);
			if (range == null)
				return null;
			else
				return new FormulaReferencedRange(range, Position, Length, true);
		}
	}
	public class TableItemParsedData : ItemParsedData, IRangeParsedData {
		#region Fields
		ParsedThingTable tablePtg;
		#endregion
		public TableItemParsedData(ParsedThingTable tablePtg, int position, int length)
			: base(position, length) {
			this.tablePtg = tablePtg;
		}
		#region Properties
		public ParsedThingTable TablePtg { get { return tablePtg; } }
		#endregion
		public CellRangeBase GetCellRange(WorkbookDataContext context) {
			ParsedThingTableExt externalRef = tablePtg as ParsedThingTableExt;
			if (externalRef != null) {
				SheetDefinition sheetDefinition = context.GetSheetDefinition(externalRef.SheetDefinitionIndex);
				if (sheetDefinition.GetSheetStart(context) != context.CurrentWorksheet)
					return null;
			}
			VariantValue evaluated = tablePtg.PreEvaluate(context);
			if (!evaluated.IsCellRange)
				return null;
			return evaluated.CellRangeValue;
		}
		public FormulaReferencedRange GetReferencedRange(WorkbookDataContext context) {
			CellRangeBase range = GetCellRange(context);
			if (range == null)
				return null;
			else
				return new FormulaReferencedRange(range, Position, Length, true);
		}
	}
	public class FunctionCallItemParsedData : ItemParsedData {
		#region Fields
		readonly ParsedThingFunc funcThing;
		readonly List<FunctionParameterParsedData> parameters;
		#endregion
		public FunctionCallItemParsedData(ParsedThingFunc funcThing, int position, int length, List<FunctionParameterParsedData> parameters)
			: base(position, length) {
			this.funcThing = funcThing;
			this.parameters = parameters;
		}
		#region Properties
		public ParsedThingFunc FuncThing { get { return funcThing; } }
		public List<FunctionParameterParsedData> Parameters { get { return parameters; } }
		#endregion
	}
	public class FunctionParameterParsedData : ItemParsedData {
		public FunctionParameterParsedData(int position, int length)
			: base(position, length) {
		}
	}
	#endregion
	#region FormulaReferencedRange
	public class FormulaReferencedRange : ItemParsedData, IEquatable<FormulaReferencedRange> {
		readonly CellRangeBase cellRange;
		readonly bool isReadOnly;
		public FormulaReferencedRange(CellRangeBase cellRange, int position, int length, bool isReadOnly)
			: base(position, length) {
			this.cellRange = cellRange;
			this.isReadOnly = isReadOnly;
		}
		public CellRangeBase CellRange { get { return cellRange; } }
		public bool IsReadOnly { get { return isReadOnly; } }
		public Color Color { get; set; }
		public bool Equals(FormulaReferencedRange other) {
			if (!base.Equals(other))
				return false;
			return CellRange.Equals(other.CellRange) && isReadOnly == other.isReadOnly;
		}
	}
	#endregion
	#region FormulaReferencedRanges
	public class FormulaReferencedRanges : List<FormulaReferencedRange> {
		public FormulaReferencedRanges Union() {
			if (Count <= 1)
				return this;
			if (IsVerticalDirection())
				return UnionReferencedRangesVertically();
			else
				return UnionReferencedRangesHorizontally();
		}
		bool IsVerticalDirection() {
			FormulaReferencedRange range = this[0];
			for (int i = 1; i < Count; i++) {
				if (!range.CellRange.Includes(this[i].CellRange))
					return IsVerticalDirection(range, this[i]);
			}
			return false;
		}
		bool IsVerticalDirection(FormulaReferencedRange range1, FormulaReferencedRange range2) {
			if (range1.CellRange.BottomRight.Row + 1 == range2.CellRange.TopLeft.Row ||
				range1.CellRange.TopLeft.Row == range2.CellRange.BottomRight.Row + 1)
				return true;
			else
				return false;
		}
		FormulaReferencedRanges UnionReferencedRangesVertically() {
			FormulaReferencedRange range = this[0];
			for (int i = 1; i < Count; i++) {
				range = UnionReferenceRangesVertically(range, this[i]);
				if (range == null)
					return this;
			}
			FormulaReferencedRanges result = new FormulaReferencedRanges();
			result.Add(range);
			return result;
		}
		FormulaReferencedRanges UnionReferencedRangesHorizontally() {
			FormulaReferencedRange range = this[0];
			for (int i = 1; i < Count; i++) {
				range = UnionReferenceRangesHorizontally(range, this[i]);
				if (range == null)
					return this;
			}
			FormulaReferencedRanges result = new FormulaReferencedRanges();
			result.Add(range);
			return result;
		}
		FormulaReferencedRange UnionReferenceRangesVertically(FormulaReferencedRange range1, FormulaReferencedRange range2) {
			CellRangeBase cellRange1 = range1.CellRange;
			CellRangeBase cellRange2 = range2.CellRange;
			if (cellRange1.Worksheet != cellRange2.Worksheet)
				return null;
			if (cellRange1.Includes(cellRange2))
				return range1;
			if (cellRange1.BottomRight.Row + 1 != cellRange2.TopLeft.Row)
				return null;
			if (cellRange1.TopLeft.Column != cellRange2.TopLeft.Column || cellRange1.BottomRight.Column != cellRange2.BottomRight.Column)
				return null;
			CellRange range = new CellRange(cellRange1.Worksheet, cellRange1.TopLeft, cellRange2.BottomRight);
			FormulaReferencedRange result = new FormulaReferencedRange(range, 0, 0, false);
			result.Color = range1.Color;
			return result;
		}
		FormulaReferencedRange UnionReferenceRangesHorizontally(FormulaReferencedRange range1, FormulaReferencedRange range2) {
			CellRangeBase cellRange1 = range1.CellRange;
			CellRangeBase cellRange2 = range2.CellRange;
			if (cellRange1.Worksheet != cellRange2.Worksheet)
				return null;
			if (cellRange1.Includes(cellRange2))
				return range1;
			if (cellRange1.BottomRight.Column + 1 != cellRange2.TopLeft.Column)
				return null;
			if (cellRange1.TopLeft.Row != cellRange2.TopLeft.Row || cellRange1.BottomRight.Row != cellRange2.BottomRight.Row)
				return null;
			CellRange range = new CellRange(cellRange1.Worksheet, cellRange1.TopLeft, cellRange2.BottomRight);
			FormulaReferencedRange result = new FormulaReferencedRange(range, 0, 0, false);
			result.Color = range1.Color;
			return result;
		}
		public CellRangeBase GetUnionRanges() {
			int count = Count;
			if (count == 0)
				return null;
			CellRangeBase result = this[0].CellRange;
			for (int i = 1; i < count; i++) {
				CellRangeBase range = this[i].CellRange;
				if (range == null)
					return null;
				result = result.MergeWithRange(range);
			}
			return result;
		}
		public void AssignColors(int colorIndex) {
			Color color = DevExpress.XtraSpreadsheet.Internal.ReferencedRangesCalculator.CalculateReferencedRangeColor(colorIndex);
			int count = Count;
			for (int i = 0; i < count; i++)
				this[i].Color = color;
		}
	}
	#endregion
	#region ParserSuggestions
	public abstract class ParserSuggestion {
		public abstract string TryApplyCorrection(string formula);
	}
	public class CloseBracketSuggestion : ParserSuggestion {
		#region Fields
		readonly int position;
		#endregion
		public CloseBracketSuggestion(int position) {
			this.position = position;
		}
		#region Fields
		public int Position { get { return position; } }
		#endregion
		public override string TryApplyCorrection(string formula) {
			if (formula.Length < position)
				return null;
			return formula.Insert(position, ")");
		}
	}
	public class FunctionCloseBracketSuggestion : CloseBracketSuggestion {
		public FunctionCloseBracketSuggestion(int position)
			: base(position) {
		}
	}
	#endregion
	#region CellReferenceParseHelper
	public class CellReferenceParseHelper {
		public int PrevThingIndex { get; set; }
		public int PrevSheetDefinitionIndex { get; set; }
		public int CurSheetDefinitionIndex { get; set; }
		public int PrevStartCharPosition { get; set; }
		public int CurStartCharPosition { get; set; }
		public int PrevEndCharPosition { get; set; }
		public int CurEndCharPosition { get; set; }
	}
	#endregion
	#region SheetDefinitionParserContext
	public class SheetDefinitionParserContext {
		readonly SheetDefinition sheetDefinition;
		string externalName;
		bool wasFileDefinition = false;
		public SheetDefinitionParserContext() {
			sheetDefinition = new SheetDefinition();
		}
		public string SheetNameStart { get { return sheetDefinition.SheetNameStart; } set { sheetDefinition.SheetNameStart = value; } }
		public string SheetNameEnd { get { return sheetDefinition.SheetNameEnd; } set { sheetDefinition.SheetNameEnd = value; } }
		public string ExternalName { get { return externalName; } set { externalName = value; } }
		public int ExternalReferenceIndex { get { return sheetDefinition.ExternalReferenceIndex; } set { sheetDefinition.ExternalReferenceIndex = value; } }
		public bool WasFileDefinition { get { return wasFileDefinition; } set { wasFileDefinition = false; } }
		public int RegisterSheetDefinition(IExpressionParserContext parserContext) {
			int externalIndex = sheetDefinition.ExternalReferenceIndex;
			bool containsExternalName = externalIndex > 0 || !string.IsNullOrEmpty(externalName);
			string sheetNameStart = sheetDefinition.SheetNameStart;
			if (containsExternalName)
				sheetDefinition.ExternalReferenceIndex = parserContext.RegisterExternalLink(sheetNameStart, sheetDefinition.SheetNameEnd, externalIndex, externalName);
			return parserContext.RegisterSheetDefinition(sheetDefinition);
		}
		public void Clear() {
			sheetDefinition.SheetNameStart = string.Empty;
			sheetDefinition.SheetNameEnd = string.Empty;
			sheetDefinition.ExternalReferenceIndex = -1;
			ExternalName = string.Empty;
		}
	}
	#endregion
}
