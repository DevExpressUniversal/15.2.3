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
using DevExpress.Export.Xl;
using DevExpress.XtraSpreadsheet.Model.External;
using System;
using System.Collections.Generic;
using System.Globalization;
namespace DevExpress.XtraSpreadsheet.Model {
	#region NoModelParserContext
	public class NoModelParserContext : IExpressionParserContext {
		#region Fields
		readonly ParserErrors errorList;
		readonly IXlExpressionContext context;
		ParsedExpression result;
		readonly SheetDefinitionCollection sheetDefinitions;
		#endregion
		public NoModelParserContext(IXlExpressionContext context) {
			Guard.ArgumentNotNull(context, "context");
			this.context = context;
			errorList = new ParserErrors();
			result = new ParsedExpression();
			sheetDefinitions = new SheetDefinitionCollection();
		}
		#region Properties
		public IXlExpressionContext Context { get { return context; } }
		public ParserErrors ErrorList { get { return errorList; } }
		public ParsedExpression Result { get { return result; } set { result = value; } }
		public bool ExceptionOccurred { get; set; }
		public char DecimalSymbol { get { return Culture.NumberFormat.NumberDecimalSeparator[0]; } }
		public char ListSeparator { get { return Culture.TextInfo.ListSeparator[0]; } }
		public bool AllowsToCreateExternal { get { return false; } }
		public bool UseR1C1ReferenceStyle { get { return Context.ReferenceStyle == XlCellReferenceStyle.R1C1; } }
		public bool DefinedNameProcessing { get { return Context.ExpressionStyle == XlExpressionStyle.DefinedName; } }
		public bool ArrayFormulaProcessing { get { return Context.ExpressionStyle == XlExpressionStyle.Array; } }
		public bool RelativeToCurrentCell { get { return Context.ReferenceMode == XlCellReferenceMode.Offset || Context.ExpressionStyle == XlExpressionStyle.Shared; } }
		public int CurrentColumnIndex { get { return Context.CurrentCell.Column; } }
		public int CurrentRowIndex { get { return Context.CurrentCell.Row; } }
		public bool ImportExportMode { get { return false; } }
		public CultureInfo Culture { get { return Context.Culture; } }
		#endregion
		public void PushCreatedExternalLinks() {
		}
		internal ParsedExpression GetPreparedResult() {
			if (ExceptionOccurred || result == null || errorList.Count > 0 || result.Count == 0)
				return null;
			return result;
		}
		public int RegisterExternalLink(string sheetNameStart, string sheetNameEnd, int externalIndex, string externalName) {
			throw new InvalidOperationException("External links are not supported.");
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
			return null;
		}
		public Table GetCurrentTable() {
			return null;
		}
		public bool HasDefinedName(string name, SheetDefinition sheetDefinition) {
			return false;
		}
		public bool HasSheetScopedDefinedName(string name) {
			return false;
		}
		public ICellError CreateError(string name) {
			return CellErrorFactory.CreateError(name);
		}
		public ISpreadsheetFunction GetFunctionByName(string name) {
			return FormulaCalculator.GetFunctionByInvariantName(name);
		}
		public bool TryParseBoolean(string elementToParse, out bool boolResult) {
			return WorkbookDataContext.TryParseBooleanInvariant(elementToParse, out boolResult);
		}
		public bool SheetDefinitionRefersToDdeWorkbook(int sheetDefinitionIndex) {
			return false;
		}
		public int RegisterSheetDefinition(SheetDefinition sheetDefinition) {
			return sheetDefinitions.GetIndex(sheetDefinition);
		}
		public SheetDefinition GetSheetDefinition(int sheetDefinitionIndex) {
			return sheetDefinitions[sheetDefinitionIndex];
		}
		public bool IsLocalWorkbookProcessing() {
			return true;
		}
		public string GetCurrentSheetName() {
			return Context.CurrentSheetName;
		}
		public bool IsCurrentWorkbookContainsSheet(string sheetName) {
			return true;
		}
		public ExternalLinkInfo GetExternalLinkByName(string name) {
			return null;
		}
	}
	#endregion
	#region SpreadsheetExpressionToXlExpressionConverter
	public class SpreadsheetExpressionToXlExpressionConverter : ParsedThingVisitor {
		static readonly Dictionary<ModelCellErrorType, XlCellErrorType> errorConversionTable = CreateErrorConversionTable();
		static Dictionary<ModelCellErrorType, XlCellErrorType> CreateErrorConversionTable() {
			Dictionary<ModelCellErrorType, XlCellErrorType> result = new Dictionary<ModelCellErrorType, XlCellErrorType>();
			result.Add(ModelCellErrorType.DivisionByZero, XlCellErrorType.DivisionByZero);
			result.Add(ModelCellErrorType.Name, XlCellErrorType.Name);
			result.Add(ModelCellErrorType.NotAvailable, XlCellErrorType.NotAvailable);
			result.Add(ModelCellErrorType.Null, XlCellErrorType.Null);
			result.Add(ModelCellErrorType.Number, XlCellErrorType.Number);
			result.Add(ModelCellErrorType.Reference, XlCellErrorType.Reference);
			result.Add(ModelCellErrorType.Value, XlCellErrorType.Value);
			return result;
		}
		readonly IExpressionParserContext context;
		DevExpress.Export.Xl.XlExpression result;
		public SpreadsheetExpressionToXlExpressionConverter(IExpressionParserContext context) {
			this.context = context;
		}
		internal DevExpress.Export.Xl.XlExpression Convert(ParsedExpression spreadsheetExpression) {
			result = new XlExpression();
			Process(spreadsheetExpression);
			return result;
		}
		void ThrowThingCanNotBeConverted(IParsedThing thing) {
			throw new ArgumentException("The specified parse thing can not be converted. Type: " + thing.GetType().ToString());
		}
		XlPtgDataType ConvertDataType(OperandDataType dataType) {
			switch (dataType) {
				case OperandDataType.Array:
					return XlPtgDataType.Array;
				case OperandDataType.Reference:
					return XlPtgDataType.Reference;
				case OperandDataType.Value:
					return XlPtgDataType.Value;
			}
			throw new ArgumentException("Data type \'" + dataType + "\'can not be coverted.");
		}
		XlVariantValue ConvertVariantValue(VariantValue spreadsheetValue) {
			switch (spreadsheetValue.Type) {
				case VariantValueType.Boolean:
					return spreadsheetValue.BooleanValue;
				case VariantValueType.Error:
					return ConvertErrorValue(spreadsheetValue.ErrorValue);
				case VariantValueType.InlineText:
					return spreadsheetValue.InlineTextValue;
				case VariantValueType.Numeric:
					return spreadsheetValue.NumericValue;
				case VariantValueType.None:
					return XlVariantValue.Empty;
				default:
					throw new ArgumentException("VariantValue + " + spreadsheetValue.ToString() + "can not be converted.");
			}
		}
		XlVariantValue ConvertErrorValue(ICellError cellError) {
			XlCellErrorType errorType;
			if (!errorConversionTable.TryGetValue(cellError.Type, out errorType))
				throw new ArgumentException();
			return XlCellErrorFactory.CreateError(errorType);
		}
		XlCellPosition ConvertCellPosition(CellPosition cellPosition) {
			return new XlCellPosition(cellPosition.Column, cellPosition.Row, (XlPositionType)cellPosition.ColumnType, (XlPositionType)cellPosition.RowType);
		}
		XlCellOffset ConvertCellOffset(CellOffset cellOffset) {
			return new XlCellOffset(cellOffset.Column, cellOffset.Row, (XlCellOffsetType)cellOffset.ColumnType, (XlCellOffsetType)cellOffset.RowType);
		}
		string GetSheetName(int sheetDefinitionIndex) {
			SheetDefinition sheetDefinition = context.GetSheetDefinition(sheetDefinitionIndex);
			if (sheetDefinition.Is3DReference)
				throw new ArgumentException("3d references are not supported.");
			if (sheetDefinition.IsExternalReference)
				throw new ArgumentException("External references are not supported.");
			return sheetDefinition.SheetNameStart;
		}
		public override void Visit(ParsedThingDataTable thing) {
			base.Visit(thing);
			ThrowThingCanNotBeConverted(thing);
		}
		public override void Visit(ParsedThingExp thing) {
			base.Visit(thing);
			ThrowThingCanNotBeConverted(thing);
		}
		public override void Visit(ParsedThingParentheses thing) {
			result.Add(new XlPtgParen());
			base.Visit(thing);
		}
		#region Binary
		public override void Visit(ParsedThingAdd thing) {
			base.Visit(thing);
			result.Add(new XlPtgBinaryOperator(XlPtgTypeCode.Add));
		}
		public override void Visit(ParsedThingSubtract thing) {
			base.Visit(thing);
			result.Add(new XlPtgBinaryOperator(XlPtgTypeCode.Sub));
		}
		public override void Visit(ParsedThingMultiply thing) {
			base.Visit(thing);
			result.Add(new XlPtgBinaryOperator(XlPtgTypeCode.Mul));
		}
		public override void Visit(ParsedThingDivide thing) {
			base.Visit(thing);
			result.Add(new XlPtgBinaryOperator(XlPtgTypeCode.Div));
		}
		public override void Visit(ParsedThingPower thing) {
			base.Visit(thing);
			result.Add(new XlPtgBinaryOperator(XlPtgTypeCode.Power));
		}
		public override void Visit(ParsedThingConcat thing) {
			base.Visit(thing);
			result.Add(new XlPtgBinaryOperator(XlPtgTypeCode.Concat));
		}
		public override void Visit(ParsedThingLess thing) {
			base.Visit(thing);
			result.Add(new XlPtgBinaryOperator(XlPtgTypeCode.Lt));
		}
		public override void Visit(ParsedThingLessEqual thing) {
			base.Visit(thing);
			result.Add(new XlPtgBinaryOperator(XlPtgTypeCode.Le));
		}
		public override void Visit(ParsedThingEqual thing) {
			base.Visit(thing);
			result.Add(new XlPtgBinaryOperator(XlPtgTypeCode.Eq));
		}
		public override void Visit(ParsedThingGreaterEqual thing) {
			result.Add(new XlPtgBinaryOperator(XlPtgTypeCode.Ge));
			base.Visit(thing);
		}
		public override void Visit(ParsedThingGreater thing) {
			result.Add(new XlPtgBinaryOperator(XlPtgTypeCode.Gt));
			base.Visit(thing);
		}
		public override void Visit(ParsedThingNotEqual thing) {
			result.Add(new XlPtgBinaryOperator(XlPtgTypeCode.Ne));
			base.Visit(thing);
		}
		public override void Visit(ParsedThingIntersect thing) {
			result.Add(new XlPtgBinaryOperator(XlPtgTypeCode.Isect));
			base.Visit(thing);
		}
		public override void Visit(ParsedThingUnion thing) {
			result.Add(new XlPtgBinaryOperator(XlPtgTypeCode.Union));
			base.Visit(thing);
		}
		public override void Visit(ParsedThingRange thing) {
			result.Add(new XlPtgBinaryOperator(XlPtgTypeCode.Range));
			base.Visit(thing);
		}
		#endregion
		#region Unary
		public override void Visit(ParsedThingUnaryPlus thing) {
			base.Visit(thing);
			result.Add(new XlPtgUnaryOperator(XlPtgTypeCode.Uplus));
		}
		public override void Visit(ParsedThingUnaryMinus thing) {
			result.Add(new XlPtgUnaryOperator(XlPtgTypeCode.Uminus));
			base.Visit(thing);
		}
		public override void Visit(ParsedThingPercent thing) {
			result.Add(new XlPtgUnaryOperator(XlPtgTypeCode.Percent));
			base.Visit(thing);
		}
		#endregion
		#region Elf
		public override void Visit(ParsedThingElfLel thing) {
			ThrowThingCanNotBeConverted(thing);
			base.Visit(thing);
		}
		public override void Visit(ParsedThingElfRw thing) {
			ThrowThingCanNotBeConverted(thing);
			base.Visit(thing);
		}
		public override void Visit(ParsedThingElfCol thing) {
			ThrowThingCanNotBeConverted(thing);
			base.Visit(thing);
		}
		public override void Visit(ParsedThingElfRwV thing) {
			ThrowThingCanNotBeConverted(thing);
			base.Visit(thing);
		}
		public override void Visit(ParsedThingElfColV thing) {
			ThrowThingCanNotBeConverted(thing);
			base.Visit(thing);
		}
		public override void Visit(ParsedThingElfRadical thing) {
			ThrowThingCanNotBeConverted(thing);
			base.Visit(thing);
		}
		public override void Visit(ParsedThingElfRadicalS thing) {
			ThrowThingCanNotBeConverted(thing);
			base.Visit(thing);
		}
		public override void Visit(ParsedThingElfColS thing) {
			ThrowThingCanNotBeConverted(thing);
			base.Visit(thing);
		}
		public override void Visit(ParsedThingElfColSV thing) {
			ThrowThingCanNotBeConverted(thing);
			base.Visit(thing);
		}
		public override void Visit(ParsedThingElfRadicalLel thing) {
			ThrowThingCanNotBeConverted(thing);
			base.Visit(thing);
		}
		#endregion
		public override void Visit(ParsedThingSxName thing) {
			ThrowThingCanNotBeConverted(thing);
			base.Visit(thing);
		}
		public override void Visit(ParsedThingAttrSemi thing) {
			result.Add(new XlPtgAttrSemi());
			base.Visit(thing);
		}
		public override void Visit(ParsedThingAttrIf thing) {
			base.Visit(thing);
			result.Add(new XlPtgAttrIf(thing.Offset));
		}
		public override void Visit(ParsedThingAttrChoose thing) {
			base.Visit(thing);
			result.Add(new XlPtgAttrChoose(new List<int>(thing.Offsets)));
		}
		public override void Visit(ParsedThingAttrGoto thing) {
			base.Visit(thing);
			result.Add(new XlPtgAttrGoto(thing.Offset));
		}
		public override void Visit(ParsedThingAttrSum thing) {
			ThrowThingCanNotBeConverted(thing);
		}
		public override void Visit(ParsedThingAttrBaxcel thing) {
			ThrowThingCanNotBeConverted(thing);
			base.Visit(thing);
		}
		public override void Visit(ParsedThingAttrBaxcelVolatile thing) {
			ThrowThingCanNotBeConverted(thing);
			base.Visit(thing);
		}
		public override void Visit(ParsedThingAttrSpace thing) {
			XlPtgAttrSpace xlPtg = new XlPtgAttrSpace((XlPtgAttrSpaceType)thing.SpaceType, thing.CharCount);
			result.Add(xlPtg);
			base.Visit(thing);
		}
		public override void Visit(ParsedThingAttrSpaceSemi thing) {
			ThrowThingCanNotBeConverted(thing);
		}
		public override void Visit(ParsedThingMissingArg thing) {
			result.Add(new XlPtgMissArg());
			base.Visit(thing);
		}
		public override void Visit(ParsedThingStringValue thing) {
			result.Add(new XlPtgStr(thing.Value));
			base.Visit(thing);
		}
		public override void Visit(ParsedThingError thing) {
			XlCellErrorType xlPtgErrCode;
			if (!errorConversionTable.TryGetValue(thing.Error.Type, out xlPtgErrCode))
				ThrowThingCanNotBeConverted(thing);
			result.Add(new XlPtgErr(xlPtgErrCode));
			base.Visit(thing);
		}
		public override void Visit(ParsedThingBoolean thing) {
			result.Add(new XlPtgBool(thing.Value));
			base.Visit(thing);
		}
		public override void Visit(ParsedThingInteger thing) {
			result.Add(new XlPtgInt(thing.Value));
			base.Visit(thing);
		}
		public override void Visit(ParsedThingNumeric thing) {
			result.Add(new XlPtgNum(thing.Value));
			base.Visit(thing);
		}
		public override void Visit(ParsedThingArray thing) {
			base.Visit(thing);
			List<XlVariantValue> values = new List<XlVariantValue>();
			for (int i = 0; i < thing.ArrayValue.Count; i++) {
				VariantValue spreadsheetValue = thing.ArrayValue[i];
				XlVariantValue convertedValue = ConvertVariantValue(spreadsheetValue);
				values.Add(convertedValue);
			}
			XlPtgDataType dataType = ConvertDataType(thing.DataType);
			result.Add(new XlPtgArray(thing.Width, thing.Height, values, dataType));
		}
		public override void Visit(ParsedThingName thing) {
			throw new ArgumentException("References to a defined name are not supported.");
		}
		public override void Visit(ParsedThingNameX thing) {
			throw new ArgumentException("References to a defined name are not supported.");
		}
		public override void Visit(ParsedThingRef thing) {
			base.Visit(thing);
			XlPtgDataType dataType = ConvertDataType(thing.DataType);
			XlCellPosition cellPosition = ConvertCellPosition(thing.Position);
			result.Add(new XlPtgRef(cellPosition, dataType));
		}
		public override void Visit(ParsedThingRefErr thing) {
			base.Visit(thing);
			XlPtgDataType dataType = ConvertDataType(thing.DataType);
			result.Add(new XlPtgRefErr(dataType));
		}
		public override void Visit(ParsedThingRef3d thing) {
			base.Visit(thing);
			XlPtgDataType dataType = ConvertDataType(thing.DataType);
			string sheetName = GetSheetName(thing.SheetDefinitionIndex);
			XlCellPosition cellPosition = ConvertCellPosition(thing.Position);
			result.Add(new XlPtgRef3d(cellPosition, sheetName, dataType));
		}
		public override void Visit(ParsedThingErr3d thing) {
			base.Visit(thing);
			XlPtgDataType dataType = ConvertDataType(thing.DataType);
			string sheetName = GetSheetName(thing.SheetDefinitionIndex);
			result.Add(new XlPtgRefErr3d(sheetName, dataType));
		}
		public override void Visit(ParsedThingRefRel thing) {
			base.Visit(thing);
			XlPtgDataType dataType = ConvertDataType(thing.DataType);
			XlCellOffset cellOffset = ConvertCellOffset(thing.Location);
			result.Add(new XlPtgRefN(cellOffset, dataType));
		}
		public override void Visit(ParsedThingRef3dRel thing) {
			base.Visit(thing);
			XlPtgDataType dataType = ConvertDataType(thing.DataType);
			string sheetName = GetSheetName(thing.SheetDefinitionIndex);
			XlCellOffset cellOffset = ConvertCellOffset(thing.Location);
			result.Add(new XlPtgRefN3d(cellOffset, sheetName, dataType));
		}
		public override void Visit(ParsedThingArea thing) {
			base.Visit(thing);
			XlPtgDataType dataType = ConvertDataType(thing.DataType);
			XlCellPosition topLeft = ConvertCellPosition(thing.TopLeft);
			XlCellPosition bottomRight = ConvertCellPosition(thing.BottomRight);
			result.Add(new XlPtgArea(topLeft, bottomRight, dataType));
		}
		public override void Visit(ParsedThingAreaErr thing) {
			base.Visit(thing);
			XlPtgDataType dataType = ConvertDataType(thing.DataType);
			result.Add(new XlPtgAreaErr(dataType));
		}
		public override void Visit(ParsedThingArea3d thing) {
			XlPtgDataType dataType = ConvertDataType(thing.DataType);
			XlCellPosition topLeft = ConvertCellPosition(thing.TopLeft);
			XlCellPosition bottomRight = ConvertCellPosition(thing.BottomRight);
			string sheetName = GetSheetName(thing.SheetDefinitionIndex);
			result.Add(new XlPtgArea3d(topLeft, bottomRight, sheetName, dataType));
			base.Visit(thing);
		}
		public override void Visit(ParsedThingAreaN thing) {
			XlPtgDataType dataType = ConvertDataType(thing.DataType);
			XlCellOffset topLeft = ConvertCellOffset(thing.First);
			XlCellOffset bottomRight = ConvertCellOffset(thing.Last);
			result.Add(new XlPtgAreaN(topLeft, bottomRight, dataType));
			base.Visit(thing);
		}
		public override void Visit(ParsedThingAreaErr3d thing) {
			XlPtgDataType dataType = ConvertDataType(thing.DataType);
			string sheetName = GetSheetName(thing.SheetDefinitionIndex);
			result.Add(new XlPtgAreaErr3d(sheetName, dataType));
			base.Visit(thing);
		}
		public override void Visit(ParsedThingArea3dRel thing) {
			XlPtgDataType dataType = ConvertDataType(thing.DataType);
			XlCellOffset topLeft = ConvertCellOffset(thing.First);
			XlCellOffset bottomRight = ConvertCellOffset(thing.Last);
			string sheetName = GetSheetName(thing.SheetDefinitionIndex);
			result.Add(new XlPtgAreaN3d(topLeft, bottomRight, sheetName, dataType));
			base.Visit(thing);
		}
		public override void Visit(ParsedThingTable thing) {
			throw new ArgumentException("Table references are not supported.");
		}
		public override void Visit(ParsedThingTableExt thing) {
			throw new ArgumentException("Table references are not supported.");
		}
		public override void Visit(ParsedThingVariantValue thing) {
			ThrowThingCanNotBeConverted(thing);
			base.Visit(thing);
		}
		public override void Visit(ParsedThingFunc thing) {
			XlPtgDataType dataType = ConvertDataType(thing.DataType);
			result.Add(new XlPtgFunc(thing.FuncCode, dataType));
			base.Visit(thing);
		}
		public override void Visit(ParsedThingFuncVar thing) {
			XlPtgDataType dataType = ConvertDataType(thing.DataType);
			result.Add(new XlPtgFuncVar(thing.FuncCode, thing.ParamCount, dataType));
			base.Visit(thing);
		}
		public override void Visit(ParsedThingUnknownFunc thing) {
			throw new ArgumentException("The specified function \'" + thing.Name + "\' is unknown and can not be used");
		}
		public override void Visit(ParsedThingCustomFunc thing) {
			throw new ArgumentException("Custom function \'" + thing.Name + "\' can not be used");
		}
		public override void Visit(ParsedThingUnknownFuncExt thing) {
			throw new ArgumentException("The specified function \'" + thing.Name + "\' is unknown and can not be used");
		}
		public override void Visit(ParsedThingAddinFunc thing) {
			throw new ArgumentException("The specified function \'" + thing.Name + "\' is unknown and can not be used");
		}
		public override void Visit(ParsedThingMemArea thing) {
			ThrowThingCanNotBeConverted(thing);
			base.Visit(thing);
		}
		public override void Visit(ParsedThingMemErr thing) {
			ThrowThingCanNotBeConverted(thing);
			base.Visit(thing);
		}
		public override void Visit(ParsedThingMemNoMem thing) {
			ThrowThingCanNotBeConverted(thing);
			base.Visit(thing);
		}
		public override void Visit(ParsedThingMemFunc thing) {
			base.Visit(thing);
			XlPtgDataType dataType = ConvertDataType(thing.DataType);
			result.Add(new XlPtgMemFunc(thing.InnerThingCount, dataType));
		}
	}
	#endregion
}
namespace DevExpress.Spreadsheet {
	using DevExpress.XtraSpreadsheet.Model;
	#region XlFormulaParser
	public class XlFormulaParser : DevExpress.Export.Xl.IXlFormulaParser {
		readonly ReferenceParser parser;
		public XlFormulaParser() {
			parser = new ReferenceParser();
		}
		public DevExpress.Export.Xl.XlExpression Parse(string formula, DevExpress.Export.Xl.IXlExpressionContext context) {
			formula = formula.Trim();
			if (formula.StartsWith("="))
				formula = formula.Remove(0, 1);
			NoModelParserContext spreadsheetContext = new NoModelParserContext(context);
			parser.Parse(formula, OperandDataType.Value, spreadsheetContext);
			ParsedExpression spreadsheetExpression = spreadsheetContext.Result;
			SpreadsheetExpressionToXlExpressionConverter converter = new SpreadsheetExpressionToXlExpressionConverter(spreadsheetContext);
			return converter.Convert(spreadsheetExpression);
		}
	}
	#endregion
}
