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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region ISpreadsheetFunction
	public interface ISpreadsheetFunction {
		string Name { get; }
		int Code { get; }
		bool IsVolatile { get; }
		FunctionParameterCollection Parameters { get; }
		bool HasFixedParametersCount { get; }
		FunctionParameter GetParameterByExpressionIndex(int index);
		OperandDataType ReturnDataType { get; }
		OperandDataType GetDefaultDataType();
		VariantValue Evaluate(IList<VariantValue> arguments, WorkbookDataContext context, bool arrayFormulaEvaluationMode);
	}
	#endregion
	#region WorksheetFunctionBase (abstract class)
	public abstract class WorksheetFunctionBase : ISpreadsheetFunction {
		public static readonly string FUTURE_FUNCTION_PREFIX = "_xlfn.";
		protected WorksheetFunctionBase() {
		}
		#region ISpreadsheetFunction Members
		public abstract string Name { get; }
		public abstract int Code { get; }
		public virtual bool IsVolatile { get { return false; } }
		public abstract OperandDataType ReturnDataType { get; }
		public abstract FunctionParameterCollection Parameters { get; }
		public bool HasFixedParametersCount {
			get {
				return Parameters.Count <= 0 || (!Parameters[Parameters.Count - 1].Unlimited && Parameters[Parameters.Count - 1].Required);
			}
		}
		public VariantValue Evaluate(IList<VariantValue> arguments, WorkbookDataContext context, bool arrayFormulaEvaluationMode) {
			Guard.ArgumentNotNull(arguments, "arguments");
			if (arrayFormulaEvaluationMode)
				return EvaluateArray(arguments, context);
			return EvaluateCore(arguments, context);
		}
		#region ArrayEvaluation
		VariantValue EvaluateArray(IList<VariantValue> arguments, WorkbookDataContext context) {
			Dictionary<int, IVariantArray> arrayResults = new Dictionary<int, IVariantArray>();
			int maxWidth = 0;
			int maxHeight = 0;
			IList<VariantValue> argumentsClone = new List<VariantValue>();
			for (int i = 0; i < arguments.Count; i++) {
				FunctionParameter parameter = GetParameterByExpressionIndex(i);
				VariantValue value = arguments[i];
				if (parameter.DataType == OperandDataType.Value) {
					if (value.IsArray && value.ArrayValue.Count > 1) {
						arrayResults.Add(i, value.ArrayValue);
						maxWidth = Math.Max(maxWidth, value.ArrayValue.Width);
						maxHeight = Math.Max(maxHeight, value.ArrayValue.Height);
					}
					else
						value = context.DereferenceValue(value, false);
				}
				argumentsClone.Add(value);
			}
			if (arrayResults.Count <= 0)
				return EvaluateCore(argumentsClone, context);
			IVariantArray resultArray = new ArrayFunctionResult(argumentsClone, arrayResults, maxWidth, maxHeight, this, context);
			return VariantValue.FromArray(resultArray);
		}
		#endregion
		public FunctionParameter GetParameterByExpressionIndex(int index) {
			Debug.Assert(Parameters.Count > 0);
			if (index >= Parameters.Count)
				return Parameters[Parameters.Count - 1];
			return Parameters[index];
		}
		public OperandDataType GetDefaultDataType() {
			OperandDataType returnType = ReturnDataType;
			if ((returnType & OperandDataType.Reference) > 0)
				return OperandDataType.Reference;
			return OperandDataType.Array;
		}
		#endregion
		protected internal VariantValue EvaluateFromArray(IList<VariantValue> arguments, WorkbookDataContext context) {
			return EvaluateCore(arguments, context);
		}
		protected abstract VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context);
		protected virtual VariantValue GetNumericValue(VariantValue value, WorkbookDataContext context) {
			return value.ToNumeric(context);
		}
		public static double Truncate(double value) {
#if !SL
			return Math.Truncate(value);
#else
			return (int)value;
#endif
		}
	}
	#endregion
	#region ArrayFunctionResult
	public class ArrayFunctionResult : IVariantArray {
		#region Fields
		readonly IList<VariantValue> argumentsClone;
		readonly Dictionary<int, IVariantArray> arrayResults;
		readonly int width;
		readonly int height;
		readonly WorkbookDataContext context;
		readonly WorksheetFunctionBase function;
		readonly VariantArray cache;
		#endregion
		public ArrayFunctionResult(IList<VariantValue> argumentsClone, Dictionary<int, IVariantArray> arrayResults, int width, int height, WorksheetFunctionBase function, WorkbookDataContext context) {
			this.argumentsClone = argumentsClone;
			this.arrayResults = arrayResults;
			this.height = height;
			this.width = width;
			this.function = function;
			this.context = new WorkbookDataContext(context);
			this.cache = VariantArray.Create(width, height);
		}
		#region IVariantArray Members
		public long Count { get { return width * height; } }
		public int Width { get { return width; } }
		public int Height { get { return height; } }
		public VariantValue this[int index] { get { return GetValue(index / Width, index % Width); } }
		public bool IsHorizontal { get { return Height == 1 && Width != 1; } }
		public bool IsVertical { get { return Width == 1 && Height != 1; } }
		public VariantValue GetValue(int y, int x) {
			if (y < 0)
				return VariantValue.ErrorValueNotAvailable;
			if (y >= Height) {
				if (Height != 1)
					return VariantValue.ErrorValueNotAvailable;
				y = 0;
			}
			if (x < 0)
				return VariantValue.ErrorValueNotAvailable;
			if (x >= Width) {
				if (Width != 1)
					return VariantValue.ErrorValueNotAvailable;
				x = 0;
			}
			VariantValue cachedValue = cache.GetValue(y, x);
			if (cachedValue != VariantValue.Empty)
				return cachedValue;
			foreach (KeyValuePair<int, IVariantArray> pair in arrayResults)
				argumentsClone[pair.Key] = pair.Value.GetValue(y, x);
			cachedValue = function.EvaluateFromArray(argumentsClone, context);
			cache.SetValue(y, x, cachedValue);
			return cachedValue;
		}
		#endregion
		#region ICloneable<IVariantArray> Members
		public IVariantArray Clone() {
			return new ArrayFunctionResult(argumentsClone, arrayResults, width, height, function, context);
		}
		#endregion
	}
	#endregion
	#region WorksheetFunctionSingleArgumentBase (abstract class)
	public abstract class WorksheetFunctionSingleArgumentBase : WorksheetFunctionBase {
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			return EvaluateArgument(arguments[0], context);
		}
		protected abstract VariantValue EvaluateArgument(VariantValue argument, WorkbookDataContext context);
	}
	#endregion
	#region WorksheetNumericFunctionSingleArgumentBase (abstract class)
	public abstract class WorksheetNumericFunctionSingleArgumentBase : WorksheetFunctionSingleArgumentBase {
		protected override VariantValue EvaluateArgument(VariantValue argument, WorkbookDataContext context) {
			argument = argument.ToNumeric(context);
			if (argument.IsError)
				return argument;
			return GetNumericResult(argument.NumericValue);
		}
		protected abstract double GetNumericResult(double argument);
	}
	#endregion
	#region WorksheetFunctionSingleDoubleArgumentBase (abstract class)
	public abstract class WorksheetFunctionSingleDoubleArgumentBase : WorksheetFunctionSingleArgumentBase {
		protected override VariantValue EvaluateArgument(VariantValue argument, WorkbookDataContext context) {
			argument = argument.ToNumeric(context);
			if (argument.IsError)
				return argument;
			return GetNumericResult(argument.NumericValue);
		}
		protected abstract VariantValue GetNumericResult(double argument);
	}
	#endregion
	#region NotExistingFunction
	public class NotExistingFunction : WorksheetFunctionBase {
		#region Fields
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		string name;
		#endregion
		public NotExistingFunction(string name) {
			this.name = name;
		}
		#region Properties
		public override string Name { get { return name; } }
		public override int Code { get { return 0x00FF; } } 
		public override OperandDataType ReturnDataType { get { return OperandDataType.Reference | OperandDataType.Value | OperandDataType.Array; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		#endregion
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			return VariantValue.ErrorName;
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Reference | OperandDataType.Array | OperandDataType.Value, FunctionParameterOption.NonRequiredUnlimited));
			return collection;
		}
	}
	#endregion
	#region FunctionParameter
	public class FunctionParameter : IEquatable<FunctionParameter> {
		#region Fields
		OperandDataType dataType;
		FunctionParameterOption options;
		FakeParameterType parameterType = FakeParameterType.Any;
		#endregion
		public FunctionParameter(OperandDataType type, FunctionParameterOption options, string name, string description) {
			this.dataType = type;
			this.options = options;
			this.Name = name;
			this.Description = description;
		}
		public FunctionParameter(OperandDataType type, FunctionParameterOption options)
			: this(type, options, String.Empty, String.Empty) {
		}
		public FunctionParameter(OperandDataType type, FunctionParameterOption options, FakeParameterType parameterType)
			: this(type, options, String.Empty, String.Empty) {
			this.parameterType = parameterType;
		}
		public FunctionParameter(OperandDataType type, string name, string description)
			: this(type, FunctionParameterOption.Default, name, description) {
		}
		public FunctionParameter(OperandDataType type)
			: this(type, FunctionParameterOption.Default) {
		}
		public FunctionParameter(OperandDataType type, FakeParameterType parameterType)
			: this(type, FunctionParameterOption.Default) {
			this.parameterType = parameterType;
		}
		#region Properties
		public OperandDataType DataType { get { return dataType; } }
		public bool Required { get { return (options & FunctionParameterOption.NonRequired) == 0; } }
		public bool Unlimited { get { return (options & FunctionParameterOption.NonRequiredUnlimited) == FunctionParameterOption.NonRequiredUnlimited; } }
		public bool DereferenceEmptyValueAsZero { get { return (options & FunctionParameterOption.DoNotDereferenceEmptyValueAsZero) == 0; } }
		public FunctionParameterOption Options { get { return options; } }
		public string Name { get; private set; }
		public string Description { get; private set; }
		public FakeParameterType ParameterType { get { return parameterType; } }
		#endregion
		#region IEquatable<FunctionParameter> Members
		public bool Equals(FunctionParameter other) {
			if (other == null)
				return false;
			return DataType == other.DataType && options == other.options;
		}
		#endregion
	}
	#endregion
	#region ParameterOption(enum)
	public enum FunctionParameterOption {
		Default = 0,
		NonRequired = 0x1,
		NonRequiredUnlimited = 0x3,
		DoNotDereferenceEmptyValueAsZero = 0x4,
	}
	#endregion
	#region FunctionParameterCollection
	public class FunctionParameterCollection : List<FunctionParameter> {
	}
	#endregion
	#region FakeParameterType
	public enum FakeParameterType {
		Number,
		Any,
		Reference,
		Array,
		Logical,
		Text
	}
	#endregion
	#region OperandDataType
	[Flags]
	public enum OperandDataType {
		None = 0,
		Reference = 1,
		Value = 2,
		Array = 4,
		Default = 8,
	}
	#endregion
	#region FunctionResult (abstract class)
	public abstract class FunctionResult {
		readonly WorkbookDataContext context;
		List<FunctionCalculationCondition> conditions;
		VariantValue error = VariantValue.Empty;
		bool arrayProcessing;
		bool cellRangeProcessing;
		bool processErrorValues;
		bool ignoredHiddenRows;
		ContainsFunctionsPredicateBase containsFunctionsPredicate;
		protected FunctionResult(WorkbookDataContext context) {
			Guard.ArgumentNotNull(context, "context");
			this.context = context;
			arrayProcessing = false;
			cellRangeProcessing = false;
		}
		public VariantValue Error { get { return error; } set { error = value; } }
		public WorkbookDataContext Context { get { return context; } }
		public bool HasConditions { get { return conditions != null && conditions.Count > 0; } }
		protected internal bool ArrayOrRangeProcessing { get { return arrayProcessing || cellRangeProcessing; } }
		protected internal bool ArrayProcessing { get { return arrayProcessing; } }
		protected internal bool CellRangeProcessing { get { return cellRangeProcessing; } }
		protected internal bool ProcessErrorValues { get { return processErrorValues; } set { processErrorValues = value; } }
		protected internal bool IgnoredHiddenRows { get { return ignoredHiddenRows; } set { ignoredHiddenRows = value; } }
		protected internal ContainsFunctionsPredicateBase ContainsFunctionsPredicate { get { return containsFunctionsPredicate; } set { containsFunctionsPredicate = value; } }
		protected List<FunctionCalculationCondition> Conditions {
			get {
				if (conditions == null)
					conditions = new List<FunctionCalculationCondition>();
				return conditions;
			}
		}
		public void AddCondition(CellRangeBase conditionRange, VariantValue condition) {
			Conditions.Add(new FunctionCalculationCondition(conditionRange, condition, Context));
		}
		public void SortConditions() {
			if (HasConditions)
				conditions.Sort(new ConditionComparer());
		}
		public void BeginArrayProcessing(long itemsCount) {
			arrayProcessing = true;
			BeginArrayProcessingCore(itemsCount);
		}
		public bool EndArrayProcessing() {
			arrayProcessing = false;
			return EndArrayProcessingCore();
		}
		public void BeginCellRangeProcessing(long itemsCount) {
			cellRangeProcessing = true;
			BeginArrayProcessingCore(itemsCount);
		}
		public bool EndCellRangeProcessing() {
			cellRangeProcessing = false;
			return EndArrayProcessingCore();
		}
		public virtual void BeginArrayProcessingCore(long itemsCount) { }
		public virtual bool EndArrayProcessingCore() { return true; }
		public virtual ConditionCalculationResult CalculateConditions(int relativeRowIndex, int relativeColumnIndex) {
			if (conditions == null)
				return ConditionCalculationResult.True;
			foreach (FunctionCalculationCondition condition in conditions) {
				ConditionCalculationResult calculationResult = condition.Calculate(context, relativeRowIndex, relativeColumnIndex);
				if (calculationResult != ConditionCalculationResult.True)
					return calculationResult;
			}
			return ConditionCalculationResult.True;
		}
		protected ConditionCalculationResult CalculateConditions(VariantValue leftValue) {
			if (conditions == null)
				return ConditionCalculationResult.True;
			foreach (FunctionCalculationCondition condition in conditions) {
				ConditionCalculationResult calculationResult = condition.Calculate(context, leftValue);
				if (calculationResult != ConditionCalculationResult.True)
					return calculationResult;
			}
			return ConditionCalculationResult.True;
		}
		public virtual bool ProcessErrorValue(VariantValue value) {
			return ProcessSingleValue(value);
		}
		public virtual bool ProcessSingleValue(VariantValue value) {
			VariantValue convertedValue = ConvertValue(value);
			if (convertedValue.IsError) {
				Error = convertedValue;
				return false;
			}
			return ProcessConvertedValue(convertedValue);
		}
		public abstract bool ShouldProcessValueCore(VariantValue value);
		public abstract VariantValue ConvertValue(VariantValue value);
		public abstract bool ProcessConvertedValue(VariantValue value);
		public abstract VariantValue GetFinalValue();
	}
	#endregion
	public class ConditionComparer : IComparer<FunctionCalculationCondition> {
		#region IComparer<FunctionCalculationCondition> Members
		public int Compare(FunctionCalculationCondition x, FunctionCalculationCondition y) {
			return Comparer<ConditionPriority>.Default.Compare(x.Priority, y.Priority);
		}
		#endregion
	}
	#region ConditionPriority
	public enum ConditionPriority {
		High = 0,
		Medium = 1,
		Low = 2,
		VeryLow = 3,
	}
	#endregion
	#region ConditionCaclculationResult
	public enum ConditionCalculationResult {
		False,
		True,
		ErrorGettingData,
	}
	#endregion
	#region FunctionCalculationCondition
	public class FunctionCalculationCondition {
		readonly CellRangeBase conditionRange;
		VariantValue constantResult = VariantValue.Empty;
		VariantValue rightValueCalculation;
		BinaryBooleanParsedThing binaryOperator;
		ConditionPriority priority;
		public ConditionPriority Priority { get { return priority; } }
		public FunctionCalculationCondition(CellRangeBase conditionRange, VariantValue condition, WorkbookDataContext context) {
			this.conditionRange = conditionRange;
			if (conditionRange != null) {
				if (!condition.IsText && !condition.IsEmpty)
					condition = condition.ToText(context);
				CreateConditionExpression(condition, context);
			}
			else
				constantResult = false;
			CalculatePriority();
		}
		void CalculatePriority() {
			if (!constantResult.IsEmpty)
				priority = ConditionPriority.High;
			else
				priority = binaryOperator.Priority;
		}
		protected internal virtual void CreateConditionExpression(VariantValue condition, WorkbookDataContext context) {
			if (condition.IsText)
				TryCreateConditionExpressionFromText(condition.GetTextValue(context.StringTable), context);
			else
				CreateEqualsConditionExpression(condition, context);
		}
		protected void CreateEqualsConditionExpression(VariantValue condition, WorkbookDataContext context) {
			binaryOperator = new ImplicitParsedThingEqual();
			rightValueCalculation = condition;
		}
		protected internal virtual void TryCreateConditionExpressionFromText(string condition, WorkbookDataContext context) {
			int logicalOperatorLength = ParseLogicalOperator(condition);
			string stringValue = condition.Substring(logicalOperatorLength);
			VariantValue rightValue;
			if (string.IsNullOrEmpty(stringValue))
				rightValue = "";
			else
				rightValue = context.ConvertTextToVariantValueWithCaching(stringValue);
			if (logicalOperatorLength <= 0) {
				CreateEqualsCondition(rightValue, stringValue, context);
				return;
			}
			binaryOperator = CreateLogicalConditionExpression(condition.Substring(0, logicalOperatorLength));
			if (binaryOperator != null)
				rightValueCalculation = rightValue;
			else
				constantResult = false;
		}
		#region condition expression
		protected internal virtual void CreateEqualsCondition(VariantValue rightValue, string condition, WorkbookDataContext context) {
			binaryOperator = new ParsedThingEqualWithStringWildcards(true);
			rightValueCalculation = rightValue;
		}
		BinaryBooleanParsedThing CreateLogicalConditionExpression(string logicalOperator) {
			switch (logicalOperator) {
				default:
					return null;
				case "=":
					return new ParsedThingEqualWithStringWildcards(true);
				case "<>":
					return new ParsedThingNotEqualWithStringWildcards();
				case ">":
					return new ParsedThingGreaterStrictTypeMatch();
				case ">=":
					return new ParsedThingGreaterEqualStrictTypeMatch();
				case "<":
					return new ParsedThingLessStrictTypeMatch();
				case "<=":
					return new ParsedThingLessEqualStrictTypeMatch();
			}
		}
		int ParseLogicalOperator(string expressionText) {
			int count = expressionText.Length;
			for (int i = 0; i < count; i++) {
				char ch = expressionText[i];
				if (ch != '=' && ch != '>' && ch != '<')
					return i;
			}
			return count;
		}
		#endregion
		public ConditionCalculationResult Calculate(WorkbookDataContext context, int relativeRowIndex, int relativeColumnIndex) {
			if (!constantResult.IsEmpty)
				return constantResult.BooleanValue ? ConditionCalculationResult.True : ConditionCalculationResult.False;
			VariantValue leftValue = conditionRange.GetCellValueRelativeCore(relativeColumnIndex, relativeRowIndex);
			return Calculate(context, leftValue);
		}
		public ConditionCalculationResult Calculate(WorkbookDataContext context, VariantValue leftValue) {
			if (!constantResult.IsEmpty)
				return constantResult.BooleanValue ? ConditionCalculationResult.True : ConditionCalculationResult.False;
			if (leftValue == VariantValue.ErrorGettingData || rightValueCalculation == VariantValue.ErrorGettingData)
				return ConditionCalculationResult.ErrorGettingData;
			VariantValue result = binaryOperator.EvaluateSimpleOperands(context, leftValue, rightValueCalculation);
			if (result.IsBoolean && result.BooleanValue)
				return ConditionCalculationResult.True;
			return ConditionCalculationResult.False;
		}
	}
	#endregion
	#region Function codes
	#endregion
	#region PredefinedFunctions
	public static class PredefinedFunctions {
		static HashSet<string> excel2003FutureFunctions = CreateExcel2003FutureFunctions();
		static HashSet<string> excel2010FutureFunctions = CreateExcel2010FutureFunctions();
		static HashSet<string> excel2010PredefinedFunctions = CreateExcel2010PredefinedFunctions();
		static HashSet<string> internalFunctions = CreateInternalFunctions();
		#region Initialization
		static HashSet<string> CreateExcel2003FutureFunctions() {
			HashSet<string> result = new HashSet<string>();
			result.Add("CUBEVALUE");
			result.Add("CUBEMEMBER");
			result.Add("CUBEMEMBERPROPERTY");
			result.Add("CUBERANKEDMEMBER");
			result.Add("CUBEKPIMEMBER");
			result.Add("CUBESET");
			result.Add("CUBESETCOUNT");
			result.Add("IFERROR");
			result.Add("COUNTIFS");
			result.Add("SUMIFS");
			result.Add("AVERAGEIF");
			result.Add("AVERAGEIFS");
			return result;
		}
		static HashSet<string> CreateExcel2010FutureFunctions() {
			HashSet<string> result = new HashSet<string>();
			result.Add("ACOT");
			result.Add("ACOTH");
			result.Add("AGGREGATE");
			result.Add("ARABIC");
			result.Add("BASE");
			result.Add("BETA.DIST");
			result.Add("BETA.INV");
			result.Add("BINOM.DIST");
			result.Add("BINOM.DIST.RANGE");
			result.Add("BINOM.INV");
			result.Add("BITAND");
			result.Add("BITLSHIFT");
			result.Add("BITOR");
			result.Add("BITRSHIFT");
			result.Add("BITXOR");
			result.Add("CEILING.MATH");
			result.Add("CEILING.PRECISE");
			result.Add("CHISQ.DIST");
			result.Add("CHISQ.DIST.RT");
			result.Add("CHISQ.INV");
			result.Add("CHISQ.INV.RT");
			result.Add("CHISQ.TEST");
			result.Add("COMBINA");
			result.Add("CONFIDENCE.NORM");
			result.Add("CONFIDENCE.T");
			result.Add("COT");
			result.Add("COTH");
			result.Add("COVARIANCE.P");
			result.Add("COVARIANCE.S");
			result.Add("CSC");
			result.Add("CSCH");
			result.Add("DAYS");
			result.Add("DECIMAL");
			result.Add("ECMA.CEILING");
			result.Add("ERF.PRECISE");
			result.Add("ERFC.PRECISE");
			result.Add("EXPON.DIST");
			result.Add("F.DIST");
			result.Add("F.DIST.RT");
			result.Add("F.INV");
			result.Add("F.INV.RT");
			result.Add("FILTERXML");
			result.Add("FLOOR.MATH");
			result.Add("FLOOR.PRECISE");
			result.Add("FORMULATEXT");
			result.Add("GAMMA");
			result.Add("GAMMA.DIST");
			result.Add("GAMMA.INV");
			result.Add("GAMMALN.PRECISE");
			result.Add("GAUSS");
			result.Add("HYPGEOM.DIST");
			result.Add("IFNA");
			result.Add("IMCOSH");
			result.Add("IMCOT");
			result.Add("IMCSC");
			result.Add("IMCSCH");
			result.Add("IMSEC");
			result.Add("IMSECH");
			result.Add("IMSINH");
			result.Add("IMTAN");
			result.Add("ISFORMULA");
			result.Add("ISO.CEILING");
			result.Add("ISOWEEKNUM");
			result.Add("LOGNORM.DIST");
			result.Add("LOGNORM.INV");
			result.Add("MODE.MULT");
			result.Add("MODE.SNGL");
			result.Add("MUNIT");
			result.Add("NEGBINOM.DIST");
			result.Add("NETWORKDAYS.INTL");
			result.Add("NORM.DIST");
			result.Add("NORM.INV");
			result.Add("NORM.S.DIST");
			result.Add("NORM.S.INV");
			result.Add("NUMBERVALUE");
			result.Add("PDURATION");
			result.Add("PERCENTILE.EXC");
			result.Add("PERCENTILE.INC");
			result.Add("PERCENTRANK.EXC");
			result.Add("PERCENTRANK.INC");
			result.Add("PERMUTATIONA");
			result.Add("PHI");
			result.Add("POISSON.DIST");
			result.Add("QUARTILE.EXC");
			result.Add("QUARTILE.INC");
			result.Add("QUERYSTRING");
			result.Add("RANK.AVG");
			result.Add("RANK.EQ");
			result.Add("RRI");
			result.Add("SEC");
			result.Add("SECH");
			result.Add("SHEET");
			result.Add("SHEETS");
			result.Add("SKEW.P");
			result.Add("STDEV.P");
			result.Add("STDEV.S");
			result.Add("T.DIST");
			result.Add("T.DIST.2T");
			result.Add("T.DIST.RT");
			result.Add("T.INV");
			result.Add("T.INV.2T");
			result.Add("T.TEST");
			result.Add("UNICHAR");
			result.Add("UNICODE");
			result.Add("VAR.P");
			result.Add("VAR.S");
			result.Add("WEBSERVICE");
			result.Add("ENCODEURL");
			result.Add("WEIBULL.DIST");
			result.Add("WORKDAY.INTL");
			result.Add("XOR");
			result.Add("Z.TEST");
			return result;
		}
		static HashSet<string> CreateExcel2010PredefinedFunctions() {
			HashSet<string> result = new HashSet<string>();
			result.Add("HEX2BIN");
			result.Add("HEX2DEC");
			result.Add("HEX2OCT");
			result.Add("DEC2BIN");
			result.Add("DEC2HEX");
			result.Add("DEC2OCT");
			result.Add("OCT2BIN");
			result.Add("OCT2HEX");
			result.Add("OCT2DEC");
			result.Add("BIN2DEC");
			result.Add("BIN2OCT");
			result.Add("BIN2HEX");
			result.Add("IMSUB");
			result.Add("IMDIV");
			result.Add("IMPOWER");
			result.Add("IMABS");
			result.Add("IMSQRT");
			result.Add("IMLN");
			result.Add("IMLOG2");
			result.Add("IMLOG10");
			result.Add("IMSIN");
			result.Add("IMCOS");
			result.Add("IMEXP");
			result.Add("IMARGUMENT");
			result.Add("IMCONJUGATE");
			result.Add("IMAGINARY");
			result.Add("IMREAL");
			result.Add("COMPLEX");
			result.Add("IMSUM");
			result.Add("IMPRODUCT");
			result.Add("SERIESSUM");
			result.Add("FACTDOUBLE");
			result.Add("SQRTPI");
			result.Add("QUOTIENT");
			result.Add("DELTA");
			result.Add("GESTEP");
			result.Add("ISEVEN");
			result.Add("ISODD");
			result.Add("MROUND");
			result.Add("ERF");
			result.Add("ERFC");
			result.Add("BESSELJ");
			result.Add("BESSELK");
			result.Add("BESSELY");
			result.Add("BESSELI");
			result.Add("XIRR");
			result.Add("XNPV");
			result.Add("PRICEMAT");
			result.Add("YIELDMAT");
			result.Add("INTRATE");
			result.Add("RECEIVED");
			result.Add("DISC");
			result.Add("PRICEDISC");
			result.Add("YIELDDISC");
			result.Add("TBILLEQ");
			result.Add("TBILLPRICE");
			result.Add("TBILLYIELD");
			result.Add("PRICE");
			result.Add("YIELD");
			result.Add("DOLLARDE");
			result.Add("DOLLARFR");
			result.Add("NOMINAL");
			result.Add("EFFECT");
			result.Add("CUMPRINC");
			result.Add("CUMIPMT");
			result.Add("EDATE");
			result.Add("EOMONTH");
			result.Add("YEARFRAC");
			result.Add("COUPDAYBS");
			result.Add("COUPDAYS");
			result.Add("COUPDAYSNC");
			result.Add("COUPNCD");
			result.Add("COUPNUM");
			result.Add("COUPPCD");
			result.Add("DURATION");
			result.Add("MDURATION");
			result.Add("ODDLPRICE");
			result.Add("ODDLYIELD");
			result.Add("ODDFPRICE");
			result.Add("ODDFYIELD");
			result.Add("RANDBETWEEN");
			result.Add("WEEKNUM");
			result.Add("AMORDEGRC");
			result.Add("AMORLINC");
			result.Add("CONVERT");
			result.Add("ACCRINT");
			result.Add("ACCRINTM");
			result.Add("WORKDAY");
			result.Add("NETWORKDAYS");
			result.Add("GCD");
			result.Add("MULTINOMIAL");
			result.Add("LCM");
			result.Add("FVSCHEDULE");
			return result;
		}
		static HashSet<string> CreateInternalFunctions() {
			HashSet<string> result = new HashSet<string>();
			result.Add("FIELD");
			result.Add("RANGE");
			result.Add("FIELDPICTURE");
			return result;
		}
		#endregion
		public static bool IsExcel2010PredefinedFunction(string name) {
			string funcName = name.ToUpperInvariant();
			return excel2010PredefinedFunctions.Contains(funcName) || excel2003FutureFunctions.Contains(funcName);
		}
		public static bool IsExcel2010PredefinedNonFutureFunction(string name) {
			return excel2010PredefinedFunctions.Contains(name.ToUpperInvariant());
		}
		public static bool IsExcel2010FutureFunction(string name) {
			return excel2010FutureFunctions.Contains(name.ToUpperInvariant());
		}
		public static bool IsExcel2003FutureFunction(string name) {
			return excel2003FutureFunctions.Contains(name.ToUpperInvariant()) || IsExcel2010FutureFunction(name);
		}
		public static bool IsInternalFunction(string name) {
			return internalFunctions.Contains(name.ToUpperInvariant());
		}
	}
	#endregion
	#region InternalFunction
	public abstract class InternalFunction : WorksheetFunctionBase {
	}
	#endregion
}
