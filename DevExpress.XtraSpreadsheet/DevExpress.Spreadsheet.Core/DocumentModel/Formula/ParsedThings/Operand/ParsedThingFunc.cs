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

using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Utils;
using System;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region ParsedThingFunc
	public class ParsedThingFunc : ParsedThingWithDataType {
		#region Fields
		int funcCode;
		#endregion
		public ParsedThingFunc() {
		}
		public ParsedThingFunc(int funcCode) {
			this.funcCode = funcCode;
		}
		public ParsedThingFunc(int funcCode, OperandDataType dataType)
			: base(dataType) {
			this.funcCode = funcCode;
		}
		#region Properties
		public virtual int FuncCode {
			get { return this.funcCode; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "FuncCode");
				this.funcCode = value;
			}
		}
		public virtual ISpreadsheetFunction Function { get { return FormulaCalculator.GetFunctionByCode(funcCode); } }
		public virtual int ParamCount { get { return Function.Parameters.Count; } set { throw new InvalidOperationException(); } }
		#endregion
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		#region BuildExpressionString
		public override void BuildExpressionString(Stack<int> stack, StringBuilder builder, StringBuilder spacesBuilder, WorkbookDataContext context) {
			ISpreadsheetFunction function = Function;
			string functionName = PrepareFunctionName(function, context);
			BuildExpressionStringCore(stack, ParamCount, functionName, builder, spacesBuilder.ToString(), context);
			spacesBuilder.Remove(0, spacesBuilder.Length);
		}
		protected string PrepareFunctionName(ISpreadsheetFunction function, WorkbookDataContext context) {
			string name = function.Name;
			if (context.ImportExportMode && PredefinedFunctions.IsExcel2010FutureFunction(name))
				return WorksheetFunctionBase.FUTURE_FUNCTION_PREFIX + name;
			return FormulaCalculator.GetFunctionName(name, context);
		}
		protected void BuildExpressionStringCore(Stack<int> stack, int parametersCount, string functionName, StringBuilder builder, string spacesString, WorkbookDataContext context) {
			System.Diagnostics.Debug.Assert(stack.Count >= parametersCount);
			if (parametersCount > 0) {
				string separator = context.GetListSeparator().ToString();
				for (int i = 0; i < parametersCount - 1; i++)
					builder.Insert(stack.Pop(), separator);
				int startPos = stack.Peek();
				builder.Insert(startPos, functionName + "(");
				builder.Insert(startPos, spacesString);
				builder.Append(")");
			}
			else {
				stack.Push(builder.Length);
				builder.Append(functionName);
				builder.Append("()");
			}
		}
		#endregion
		public override void Evaluate(Stack<VariantValue> stack, WorkbookDataContext context) {
			bool arrayFormulaEvaluationMode = (DataType == OperandDataType.Array || DataType == OperandDataType.Value) && Function.ReturnDataType == OperandDataType.Value;
			VariantValue result = PreEvaluate(stack, ParamCount, context, arrayFormulaEvaluationMode);
			result = ConvertDataType(arrayFormulaEvaluationMode, result, context);
			stack.Push(result);
		}
		protected internal VariantValue ConvertDataType(bool arrayFormulaEvaluationMode, VariantValue result, WorkbookDataContext context) {
			arrayFormulaEvaluationMode &= result.IsArray;
			if (DataType == OperandDataType.Value) {
				if (!context.ArrayFormulaProcessing && !arrayFormulaEvaluationMode)
					result = context.DereferenceValue(result, false);
			}
			else if (DataType == OperandDataType.Array) {
				if (result.IsCellRange) {
					if (result.CellRangeValue.RangeType == CellRangeType.UnionRange)
						result = VariantValue.ErrorInvalidValueInFunction;
					else {
						RangeVariantArray array = new RangeVariantArray((CellRange)result.CellRangeValue);
						result = VariantValue.FromArray(array);
					}
				}
			}
			return result;
		}
		protected VariantValue PreEvaluate(Stack<VariantValue> stack, int parametersCount, WorkbookDataContext context, bool arrayFormulaEvaluationMode) {
			System.Diagnostics.Debug.Assert(stack.Count >= parametersCount);
			List<VariantValue> list = new List<VariantValue>();
			ISpreadsheetFunction function = Function;
			for (int i = parametersCount - 1; i >= 0; i--) {
				VariantValue parameterValue = stack.Pop();
				FunctionParameter parameterDescription = function.GetParameterByExpressionIndex(i);
				if (parameterValue.IsEmpty) {
					if (parameterDescription.DereferenceEmptyValueAsZero)
						parameterValue = 0;
				}
				else if (parameterValue.IsMissing)
					parameterValue = VariantValue.Empty;
				else if (parameterDescription.DataType == OperandDataType.Value && !arrayFormulaEvaluationMode)
					parameterValue = context.DereferenceValue(parameterValue, false);
				if (parameterDescription.DataType == OperandDataType.Reference) {
					if (!parameterValue.IsCellRange && !parameterValue.IsError)
						parameterValue = VariantValue.ErrorInvalidValueInFunction;
				}
				list.Insert(0, parameterValue);
			}
			return Function.Evaluate(list, context, arrayFormulaEvaluationMode);
		}
		protected void GetInvolvedCellRangesCore(Stack<CellRangeList> stack, int parametersCount, WorkbookDataContext context) {
			System.Diagnostics.Debug.Assert(stack.Count >= parametersCount);
			CellRangeList resultList = new CellRangeList();
			for (int i = parametersCount - 1; i >= 0; i--) {
				List<CellRangeBase> value = stack.Pop();
				resultList.AddRange(value);
			}
			stack.Push(resultList);
		}
		public override void GetInvolvedCellRanges(Stack<CellRangeList> stack, WorkbookDataContext context) {
			GetInvolvedCellRangesCore(stack, Function.Parameters.Count, context);
		}
		public override IParsedThing Clone() {
			ParsedThingFunc clone = new ParsedThingFunc();
			clone.DataType = DataType;
			clone.FuncCode = funcCode;
			return clone;
		}
		public override bool IsEqual(IParsedThing obj) {
			if (!base.IsEqual(obj))
				return false;
			ParsedThingFunc anotherPtg = (ParsedThingFunc)obj;
			return funcCode == anotherPtg.funcCode;
		}
	}
	#endregion
	#region ParsedThingFuncVar
	public class ParsedThingFuncVar : ParsedThingFunc {
		#region Fields
		int paramCount;
		#endregion
		public ParsedThingFuncVar()
			: base() {
		}
		public ParsedThingFuncVar(int funcCode, int paramCount)
			: base(funcCode) {
			this.paramCount = paramCount;
		}
		public ParsedThingFuncVar(int funcCode, int paramCount, OperandDataType dataType)
			: this(funcCode, paramCount) {
			this.DataType = dataType;
		}
		#region Properties
		public override int ParamCount {
			get { return this.paramCount; }
			set {
				ValueChecker.CheckValue(value, 0, byte.MaxValue, "ParamCount");
				this.paramCount = value;
			}
		}
		#endregion
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		public override void GetInvolvedCellRanges(Stack<CellRangeList> stack, WorkbookDataContext context) {
			GetInvolvedCellRangesCore(stack, paramCount, context);
		}
		public override IParsedThing Clone() {
			ParsedThingFuncVar clone = new ParsedThingFuncVar();
			clone.DataType = DataType;
			clone.FuncCode = FuncCode;
			clone.paramCount = paramCount;
			return clone;
		}
		public override bool IsEqual(IParsedThing obj) {
			if (!base.IsEqual(obj))
				return false;
			ParsedThingFuncVar anotherPtg = (ParsedThingFuncVar)obj;
			return paramCount == anotherPtg.paramCount;
		}
	}
	#endregion
	#region ParsedThingUnknownFunc
	public class ParsedThingUnknownFunc : ParsedThingFuncVar {
		#region Fields
		string name;
		#endregion
		public ParsedThingUnknownFunc() {
			FuncCode = 0xFF;
		}
		public ParsedThingUnknownFunc(string name, int paramCount, OperandDataType dataType)
			: base(0xFF, paramCount, dataType) {
			this.name = name;
		}
		#region Properties
		public string Name { get { return name; } set { name = value; } }
		#endregion
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		public override void BuildExpressionString(Stack<int> stack, StringBuilder builder, StringBuilder spacesBuilder, WorkbookDataContext context) {
			BuildExpressionStringCore(stack, ParamCount, name, builder, spacesBuilder.ToString(), context);
			spacesBuilder.Remove(0, spacesBuilder.Length);
		}
		public override void Evaluate(Stack<VariantValue> stack, WorkbookDataContext context) {
			System.Diagnostics.Debug.Assert(stack.Count >= ParamCount);
			for (int i = 0; i < ParamCount; i++)
				stack.Pop();
			stack.Push(VariantValue.ErrorName);
		}
		public override IParsedThing Clone() {
			ParsedThingUnknownFunc clone = new ParsedThingUnknownFunc();
			clone.DataType = DataType;
			clone.FuncCode = FuncCode;
			clone.ParamCount = ParamCount;
			clone.name = name;
			return clone;
		}
		public override bool IsEqual(IParsedThing obj) {
			if (!base.IsEqual(obj))
				return false;
			ParsedThingUnknownFunc anotherPtg = (ParsedThingUnknownFunc)obj;
			return StringExtensions.CompareInvariantCultureIgnoreCase(name, anotherPtg.name) == 0;
		}
	}
	#endregion
	#region ParsedThingUnknownFuncExt
	public class ParsedThingUnknownFuncExt : ParsedThingUnknownFunc, IParsedThingWithSheetDefinition {
		#region Fields
		int sheetDefinitionIndex;
		#endregion
		public ParsedThingUnknownFuncExt()
			: base() {
		}
		public ParsedThingUnknownFuncExt(string name, int paramCount, int sheetDefinitionIndex, OperandDataType dataType)
			: base(name, paramCount, dataType) {
			this.sheetDefinitionIndex = sheetDefinitionIndex;
		}
		#region Propertries
		public int SheetDefinitionIndex { get { return sheetDefinitionIndex; } set { sheetDefinitionIndex = value; } }
		#endregion
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		public override void BuildExpressionString(Stack<int> stack, StringBuilder builder, StringBuilder spacesBuilder, WorkbookDataContext context) {
			BuildExpressionStringCore(stack, ParamCount, Name, builder, spacesBuilder.ToString(), context);
			spacesBuilder.Remove(0, spacesBuilder.Length);
			SheetDefinition sheetDefinition = context.GetSheetDefinition(sheetDefinitionIndex);
			StringBuilder sheetDefinitionBuilder = new StringBuilder();
			sheetDefinition.BuildExpressionString(sheetDefinitionBuilder, context);
			builder.Insert(stack.Peek(), sheetDefinitionBuilder.ToString());
		}
		public override IParsedThing Clone() {
			ParsedThingUnknownFuncExt clone = new ParsedThingUnknownFuncExt();
			clone.DataType = DataType;
			clone.FuncCode = FuncCode;
			clone.ParamCount = ParamCount;
			clone.Name = Name;
			clone.sheetDefinitionIndex = sheetDefinitionIndex;
			return clone;
		}
		public override bool IsEqual(IParsedThing obj) {
			if (!base.IsEqual(obj))
				return false;
			ParsedThingUnknownFuncExt anotherPtg = (ParsedThingUnknownFuncExt)obj;
			return this.sheetDefinitionIndex == anotherPtg.sheetDefinitionIndex;
		}
	}
	#endregion
	#region ParsedThingAddinFunc
	public class ParsedThingAddinFunc : ParsedThingUnknownFunc {
		public static readonly string ADDIN_PREFIX = "_xll.";
		public ParsedThingAddinFunc()
			: base() {
		}
		public ParsedThingAddinFunc(string name, int paramCount, OperandDataType dataType)
			: base(name, paramCount, dataType) {
		}
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		public override void BuildExpressionString(Stack<int> stack, StringBuilder builder, StringBuilder spacesBuilder, WorkbookDataContext context) {
			BuildExpressionStringCore(stack, ParamCount, ADDIN_PREFIX + Name, builder, spacesBuilder.ToString(), context);
			spacesBuilder.Remove(0, spacesBuilder.Length);
		}
		public override void Evaluate(Stack<VariantValue> stack, WorkbookDataContext context) {
			System.Diagnostics.Debug.Assert(stack.Count >= ParamCount);
			for (int i = 0; i < ParamCount; i++) {
				stack.Pop();
			}
			stack.Push(VariantValue.ErrorName);
		}
		public override IParsedThing Clone() {
			ParsedThingAddinFunc clone = new ParsedThingAddinFunc();
			clone.DataType = DataType;
			clone.FuncCode = FuncCode;
			clone.ParamCount = ParamCount;
			clone.Name = Name;
			return clone;
		}
	}
	#endregion
	#region ParsedThingCustomFunc
	public class ParsedThingCustomFunc : ParsedThingFuncVar {
		#region Fields
		ISpreadsheetFunction function;
		#endregion
		public ParsedThingCustomFunc() {
			FuncCode = 0xFF;
		}
		public ParsedThingCustomFunc(ISpreadsheetFunction function, int paramCount, OperandDataType dataType)
			: base(0xFF, paramCount, dataType) {
			this.function = function;
		}
		public override ISpreadsheetFunction Function { get { return function; } }
		#region Properties
		public virtual string Name { get { return function.Name; } }
		#endregion
		public void SetFunction(ISpreadsheetFunction value) {
			this.function = value;
		}
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		public override void BuildExpressionString(Stack<int> stack, StringBuilder builder, StringBuilder spacesBuilder, WorkbookDataContext context) {
			string invariantName = function.Name;
			string localizedName = FormulaCalculator.GetFunctionName(invariantName, context);
			BuildExpressionStringCore(stack, ParamCount, localizedName, builder, spacesBuilder.ToString(), context);
			spacesBuilder.Remove(0, spacesBuilder.Length);
		}
		public override IParsedThing Clone() {
			ParsedThingCustomFunc clone = new ParsedThingCustomFunc(function, ParamCount, DataType);
			clone.FuncCode = FuncCode;
			return clone;
		}
		public override bool IsEqual(IParsedThing obj) {
			if (!base.IsEqual(obj))
				return false;
			ParsedThingCustomFunc anotherPtg = (ParsedThingCustomFunc)obj;
			return ReferenceEquals(function, anotherPtg.function);
		}
	}
	#endregion
}
