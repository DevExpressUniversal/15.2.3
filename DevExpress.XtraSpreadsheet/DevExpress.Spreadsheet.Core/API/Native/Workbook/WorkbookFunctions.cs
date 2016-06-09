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
using System.ComponentModel;
using System.Globalization;
using System.Text;
namespace DevExpress.Spreadsheet.Functions {
	using System.Threading.Tasks;
	using DevExpress.XtraSpreadsheet.Localization;
	using DevExpress.XtraSpreadsheet.Utils;
	#region WorkbookFunctions
	public interface WorkbookFunctions {
		void OverrideFunction(string name, ICustomFunction function);
		void OverrideFunction(string name, ICustomFunction function, bool skipIfExists);
		IBuiltInFunction this[string name] { get; }
		CustomFunctionCollection GlobalCustomFunctions { get; }
		CustomFunctionCollection CustomFunctions { get; }
		ICompatibilityFunctions Compatibility { get; }
		IDatabaseFunctions Database { get; }
		IDateAndTimeFunctions DateAndTime { get; }
		IEngineeringFunctions Engineering { get; }
		IFinancialFunctions Financial { get; }
		IInformationFunctions Informational { get; }
		ILogicalFunctions Logical { get; }
		ILookupAndReferenceFunctions LookupAndReference { get; }
		IMathAndTrigonometryFunctions Math { get; }
		IStatisticalFunctions Statistical { get; }
		ITextFunctions Text { get; }
		IWebFunctions Web { get; }
	}
	#endregion
	#region ParameterType(enum)
	[Flags]
	public enum ParameterType {
		Reference = 1,
		Value = 2,
		Array = 4,
		Any = 7,
	}
	#endregion
	#region ParameterAttributes(enum)
	public enum ParameterAttributes {
		Required = 0,
		Optional = 1,
		OptionalUnlimited = 3,
	}
	#endregion
	#region EvaluationContext
	public interface EvaluationContext {
		CultureInfo Culture { get; }
		int Column { get; }
		int Row { get; }
		Worksheet Sheet { get; }
	}
	#endregion
	#region ParameterInfo
	public class ParameterInfo {
		public ParameterInfo(ParameterType type) {
			this.Type = type;
			this.ConvertEmptyValueToZero = true;
		}
		public ParameterInfo(ParameterType type, ParameterAttributes Attributes)
			: this(type) {
			this.Attributes = Attributes;
		}
		#region Properties
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("ParameterInfoType")]
#endif
		public ParameterType Type { get; set; }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("ParameterInfoAttributes")]
#endif
		public ParameterAttributes Attributes { get; set; }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("ParameterInfoConvertEmptyValueToZero")]
#endif
		public bool ConvertEmptyValueToZero { get; set; }
		#endregion
	}
	#endregion
	#region EvaluationContext
	public class FormulaEvaluationContext : EvaluationContext {
		#region Fields
		CultureInfo culture;
		int column = 0;
		int row = 0;
		Worksheet sheet;
		bool isArrayFormula = false;
		#endregion
		public FormulaEvaluationContext() {
		}
		public FormulaEvaluationContext(int column, int row, Worksheet sheet)
			: this(column, row, sheet, null, false) {
		}
		public FormulaEvaluationContext(int column, int row, Worksheet sheet, CultureInfo culture, bool isArrayFormula) {
			CheckColumnIndex(column);
			CheckRowIndex(row);
			this.column = column;
			this.row = row;
			this.sheet = sheet;
			this.culture = culture;
			this.isArrayFormula = isArrayFormula;
		}
		#region Properties
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("FormulaEvaluationContextCulture")]
#endif
		public CultureInfo Culture { get { return culture; } set { culture = value; } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("FormulaEvaluationContextColumn")]
#endif
		public int Column {
			get { return column; }
			set {
				CheckColumnIndex(column);
				column = value;
			}
		}
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("FormulaEvaluationContextRow")]
#endif
		public int Row {
			get { return row; }
			set {
				CheckRowIndex(row);
				row = value;
			}
		}
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("FormulaEvaluationContextSheet")]
#endif
		public Worksheet Sheet { get { return sheet; } set { sheet = value; } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("FormulaEvaluationContextIsArrayFormula")]
#endif
		public bool IsArrayFormula { get { return isArrayFormula; } set { isArrayFormula = value; } }
		#endregion
		void CheckColumnIndex(int value) {
			if (!IndicesChecker.CheckIsColumnIndexValid(value))
				SpreadsheetExceptions.ThrowArgumentOutOfRangeException(XtraSpreadsheetStringId.Msg_ErrorIncorrectColumnIndex, "column");
		}
		void CheckRowIndex(int value) {
			if (!IndicesChecker.CheckIsRowIndexValid(value))
				SpreadsheetExceptions.ThrowArgumentOutOfRangeException(XtraSpreadsheetStringId.Msg_ErrorIncorrectRowIndex, "row");
		}
	}
	#endregion
	#region BuiltInFunctionCategories
	public interface ICompatibilityFunctions {
		IBuiltInFunction BetaDist { get; }
		IBuiltInFunction BetaInv { get; }
		IBuiltInFunction BinomDist { get; }
		IBuiltInFunction ChiDist { get; }
		IBuiltInFunction ChiInv { get; }
		IBuiltInFunction ChiTest { get; }
		IBuiltInFunction Confidence { get; }
		IBuiltInFunction Covar { get; }
		IBuiltInFunction CritBinom { get; }
		IBuiltInFunction ExponDist { get; }
		IBuiltInFunction FDist { get; }
		IBuiltInFunction FInv { get; }
		IBuiltInFunction FTest { get; }
		IBuiltInFunction GammaDist { get; }
		IBuiltInFunction GammaInv { get; }
		IBuiltInFunction HypGeomDist { get; }
		IBuiltInFunction LogInv { get; }
		IBuiltInFunction LogNormDist { get; }
		IBuiltInFunction Mode { get; }
		IBuiltInFunction NegBinomDist { get; }
		IBuiltInFunction NormDist { get; }
		IBuiltInFunction NormInv { get; }
		IBuiltInFunction NormSDist { get; }
		IBuiltInFunction NormSInv { get; }
		IBuiltInFunction Percentile { get; }
		IBuiltInFunction PercentRank { get; }
		IBuiltInFunction Poisson { get; }
		IBuiltInFunction Quartile { get; }
		IBuiltInFunction Rank { get; }
		IBuiltInFunction StDev { get; }
		IBuiltInFunction StDevP { get; }
		IBuiltInFunction TDist { get; }
		IBuiltInFunction TInv { get; }
		IBuiltInFunction TTest { get; }
		IBuiltInFunction Var { get; }
		IBuiltInFunction VarP { get; }
		IBuiltInFunction Weibull { get; }
		IBuiltInFunction ZTest { get; }
	}
	public interface ICubeFunctions {
	}
	public interface IDatabaseFunctions {
		IBuiltInFunction DAverage { get; }
		IBuiltInFunction DCount { get; }
		IBuiltInFunction DCountA { get; }
		IBuiltInFunction DGet { get; }
		IBuiltInFunction DMax { get; }
		IBuiltInFunction DMin { get; }
		IBuiltInFunction DProduct { get; }
		IBuiltInFunction DStDev { get; }
		IBuiltInFunction DStDevP { get; }
		IBuiltInFunction DSum { get; }
		IBuiltInFunction DVar { get; }
		IBuiltInFunction DVarP { get; }
	}
	public interface IDateAndTimeFunctions {
		IBuiltInFunction Date { get; }
		IBuiltInFunction DateDif { get; }
		IBuiltInFunction DateValue { get; }
		IBuiltInFunction Day { get; }
		IBuiltInFunction Days { get; }
		IBuiltInFunction Days360 { get; }
		IBuiltInFunction EDate { get; }
		IBuiltInFunction EOMonth { get; }
		IBuiltInFunction Hour { get; }
		IBuiltInFunction ISOWeekNum { get; }
		IBuiltInFunction Minute { get; }
		IBuiltInFunction Month { get; }
		IBuiltInFunction NetworkDays { get; }
		IBuiltInFunction NetworkDaysIntl { get; }
		IBuiltInFunction Now { get; }
		IBuiltInFunction Second { get; }
		IBuiltInFunction Time { get; }
		IBuiltInFunction TimeValue { get; }
		IBuiltInFunction Today { get; }
		IBuiltInFunction WeekDay { get; }
		IBuiltInFunction WeekNum { get; }
		IBuiltInFunction WorkDay { get; }
		IBuiltInFunction WorkDayIntl { get; }
		IBuiltInFunction Year { get; }
		IBuiltInFunction YearFrac { get; }
	}
	public interface IEngineeringFunctions {
		IBuiltInFunction BesselI { get; }
		IBuiltInFunction BesselJ { get; }
		IBuiltInFunction BesselK { get; }
		IBuiltInFunction BesselY { get; }
		IBuiltInFunction Bin2Dec { get; }
		IBuiltInFunction Bin2Hex { get; }
		IBuiltInFunction Bin2Oct { get; }
		IBuiltInFunction BitAnd { get; }
		IBuiltInFunction BitLShift { get; }
		IBuiltInFunction BitOr { get; }
		IBuiltInFunction BitRShift { get; }
		IBuiltInFunction BitXor { get; }
		IBuiltInFunction Complex { get; }
		IBuiltInFunction Convert { get; }
		IBuiltInFunction Dec2Bin { get; }
		IBuiltInFunction Dec2Hex { get; }
		IBuiltInFunction Dec2Oct { get; }
		IBuiltInFunction Delta { get; }
		IBuiltInFunction Erf { get; }
		IBuiltInFunction ErfPrecise { get; }
		IBuiltInFunction Erfc { get; }
		IBuiltInFunction ErfcPrecise { get; }
		IBuiltInFunction Gestep { get; }
		IBuiltInFunction Hex2Bin { get; }
		IBuiltInFunction Hex2Dec { get; }
		IBuiltInFunction Hex2Oct { get; }
		IBuiltInFunction ImAbs { get; }
		IBuiltInFunction Imaginary { get; }
		IBuiltInFunction ImArgument { get; }
		IBuiltInFunction ImConjugate { get; }
		IBuiltInFunction ImCos { get; }
		IBuiltInFunction ImCosh { get; }
		IBuiltInFunction ImCot { get; }
		IBuiltInFunction ImCsc { get; }
		IBuiltInFunction ImCsch { get; }
		IBuiltInFunction ImDiv { get; }
		IBuiltInFunction ImExp { get; }
		IBuiltInFunction ImLn { get; }
		IBuiltInFunction ImLog10 { get; }
		IBuiltInFunction ImLog2 { get; }
		IBuiltInFunction ImPower { get; }
		IBuiltInFunction ImProduct { get; }
		IBuiltInFunction ImReal { get; }
		IBuiltInFunction ImSec { get; }
		IBuiltInFunction ImSech { get; }
		IBuiltInFunction ImSin { get; }
		IBuiltInFunction ImSinh { get; }
		IBuiltInFunction ImSqrt { get; }
		IBuiltInFunction ImSub { get; }
		IBuiltInFunction ImSum { get; }
		IBuiltInFunction ImTan { get; }
		IBuiltInFunction Oct2Bin { get; }
		IBuiltInFunction Oct2Dec { get; }
		IBuiltInFunction Oct2Hex { get; }
	}
	public interface IFinancialFunctions {
		IBuiltInFunction AccrintM { get; }
		IBuiltInFunction CoupDayBs { get; }
		IBuiltInFunction CoupNcd { get; }
		IBuiltInFunction CoupNum { get; }
		IBuiltInFunction CoupPcd { get; }
		IBuiltInFunction CumIpmt { get; }
		IBuiltInFunction CumPrinc { get; }
		IBuiltInFunction Db { get; }
		IBuiltInFunction Ddb { get; }
		IBuiltInFunction Disc { get; }
		IBuiltInFunction DollarDe { get; }
		IBuiltInFunction DollarFr { get; }
		IBuiltInFunction Effect { get; }
		IBuiltInFunction Fv { get; }
		IBuiltInFunction FvSchedule { get; }
		IBuiltInFunction Intrate { get; }
		IBuiltInFunction Ipmt { get; }
		IBuiltInFunction Irr { get; }
		IBuiltInFunction Ispmt { get; }
		IBuiltInFunction Mirr { get; }
		IBuiltInFunction Nominal { get; }
		IBuiltInFunction NPer { get; }
		IBuiltInFunction Npv { get; }
		IBuiltInFunction PDuration { get; }
		IBuiltInFunction Pmt { get; }
		IBuiltInFunction Ppmt { get; }
		IBuiltInFunction PriceDisc { get; }
		IBuiltInFunction Pv { get; }
		IBuiltInFunction Rate { get; }
		IBuiltInFunction Received { get; }
		IBuiltInFunction Rri { get; }
		IBuiltInFunction Sln { get; }
		IBuiltInFunction Syd { get; }
		IBuiltInFunction TBillEq { get; }
		IBuiltInFunction TBillPrice { get; }
		IBuiltInFunction TBillYield { get; }
		IBuiltInFunction Vdb { get; }
		IBuiltInFunction Xirr { get; }
		IBuiltInFunction Xnpv { get; }
		IBuiltInFunction YieldDisc { get; }
	}
	public interface IInformationFunctions {
		IBuiltInFunction Cell { get; }
		IBuiltInFunction ErrorType { get; }
		IBuiltInFunction Info { get; }
		IBuiltInFunction IsBlank { get; }
		IBuiltInFunction IsErr { get; }
		IBuiltInFunction IsError { get; }
		IBuiltInFunction IsEven { get; }
		IBuiltInFunction IsFormula { get; }
		IBuiltInFunction IsLogical { get; }
		IBuiltInFunction IsNA { get; }
		IBuiltInFunction IsNonText { get; }
		IBuiltInFunction IsNumber { get; }
		IBuiltInFunction IsOdd { get; }
		IBuiltInFunction IsRef { get; }
		IBuiltInFunction IsText { get; }
		IBuiltInFunction N { get; }
		IBuiltInFunction NA { get; }
		IBuiltInFunction Sheet { get; }
		IBuiltInFunction Sheets { get; }
		IBuiltInFunction Type { get; }
	}
	public interface ILogicalFunctions {
		IBuiltInFunction And { get; }
		IBuiltInFunction False { get; }
		IBuiltInFunction If { get; }
		IBuiltInFunction IfError { get; }
		IBuiltInFunction IfNA { get; }
		IBuiltInFunction Not { get; }
		IBuiltInFunction Or { get; }
		IBuiltInFunction True { get; }
		IBuiltInFunction Xor { get; }
	}
	public interface ILookupAndReferenceFunctions {
		IBuiltInFunction Address { get; }
		IBuiltInFunction Areas { get; }
		IBuiltInFunction Choose { get; }
		IBuiltInFunction Column { get; }
		IBuiltInFunction Columns { get; }
		IBuiltInFunction FormulaText { get; }
		IBuiltInFunction GetPivotData { get; }
		IBuiltInFunction HLookup { get; }
		IBuiltInFunction Hyperlink { get; }
		IBuiltInFunction Index { get; }
		IBuiltInFunction Indirect { get; }
		IBuiltInFunction Lookup { get; }
		IBuiltInFunction Match { get; }
		IBuiltInFunction Offset { get; }
		IBuiltInFunction Row { get; }
		IBuiltInFunction Rows { get; }
		IBuiltInFunction Rtd { get; }
		IBuiltInFunction Transpose { get; }
		IBuiltInFunction VLookup { get; }
	}
	public interface IMathAndTrigonometryFunctions {
		IBuiltInFunction Abs { get; }
		IBuiltInFunction Acos { get; }
		IBuiltInFunction Acosh { get; }
		IBuiltInFunction Acot { get; }
		IBuiltInFunction Acoth { get; }
		IBuiltInFunction Arabic { get; }
		IBuiltInFunction Asin { get; }
		IBuiltInFunction Asinh { get; }
		IBuiltInFunction Atan { get; }
		IBuiltInFunction Atan2 { get; }
		IBuiltInFunction Atanh { get; }
		IBuiltInFunction Base { get; }
		IBuiltInFunction Ceiling { get; }
		IBuiltInFunction CeilingMath { get; }
		IBuiltInFunction CeilingPrecise { get; }
		IBuiltInFunction Combin { get; }
		IBuiltInFunction CombinA { get; }
		IBuiltInFunction Cos { get; }
		IBuiltInFunction Cosh { get; }
		IBuiltInFunction Cot { get; }
		IBuiltInFunction Coth { get; }
		IBuiltInFunction Csc { get; }
		IBuiltInFunction Csch { get; }
		IBuiltInFunction Decimal { get; }
		IBuiltInFunction Degrees { get; }
		IBuiltInFunction Even { get; }
		IBuiltInFunction Exp { get; }
		IBuiltInFunction Fact { get; }
		IBuiltInFunction FactDouble { get; }
		IBuiltInFunction Floor { get; }
		IBuiltInFunction FloorMath { get; }
		IBuiltInFunction FloorPrecise { get; }
		IBuiltInFunction Gcd { get; }
		IBuiltInFunction Int { get; }
		IBuiltInFunction IsoCeiling { get; }
		IBuiltInFunction Lcm { get; }
		IBuiltInFunction Ln { get; }
		IBuiltInFunction Log { get; }
		IBuiltInFunction Log10 { get; }
		IBuiltInFunction MDeterm { get; }
		IBuiltInFunction MInverse { get; }
		IBuiltInFunction MMult { get; }
		IBuiltInFunction Mod { get; }
		IBuiltInFunction MRound { get; }
		IBuiltInFunction Multinomial { get; }
		IBuiltInFunction MUnit { get; }
		IBuiltInFunction Odd { get; }
		IBuiltInFunction Pi { get; }
		IBuiltInFunction Power { get; }
		IBuiltInFunction Product { get; }
		IBuiltInFunction Quotient { get; }
		IBuiltInFunction Radians { get; }
		IBuiltInFunction Rand { get; }
		IBuiltInFunction RandBetween { get; }
		IBuiltInFunction Roman { get; }
		IBuiltInFunction Round { get; }
		IBuiltInFunction RoundDown { get; }
		IBuiltInFunction RoundUp { get; }
		IBuiltInFunction Sec { get; }
		IBuiltInFunction Sech { get; }
		IBuiltInFunction SeriesSum { get; }
		IBuiltInFunction Sign { get; }
		IBuiltInFunction Sin { get; }
		IBuiltInFunction Sinh { get; }
		IBuiltInFunction Sqrt { get; }
		IBuiltInFunction SqrtPi { get; }
		IBuiltInFunction Subtotal { get; }
		IBuiltInFunction Sum { get; }
		IBuiltInFunction SumIf { get; }
		IBuiltInFunction SumIfS { get; }
		IBuiltInFunction SumProduct { get; }
		IBuiltInFunction SumSq { get; }
		IBuiltInFunction SumX2mY2 { get; }
		IBuiltInFunction SumX2pY2 { get; }
		IBuiltInFunction SumXmY2 { get; }
		IBuiltInFunction Tan { get; }
		IBuiltInFunction Tanh { get; }
		IBuiltInFunction Trunc { get; }
	}
	public interface IStatisticalFunctions {
		IBuiltInFunction Avedev { get; }
		IBuiltInFunction Average { get; }
		IBuiltInFunction AverageA { get; }
		IBuiltInFunction AverageIf { get; }
		IBuiltInFunction AverageIfS { get; }
		IBuiltInFunction BetaDist { get; }
		IBuiltInFunction BetaInv { get; }
		IBuiltInFunction BinomDist { get; }
		IBuiltInFunction BinomDistRange { get; }
		IBuiltInFunction BinomInv { get; }
		IBuiltInFunction ChiSqDist { get; }
		IBuiltInFunction ChiSqDistRt { get; }
		IBuiltInFunction ChiSqInv { get; }
		IBuiltInFunction ChiSqInvRt { get; }
		IBuiltInFunction ChiSqTest { get; }
		IBuiltInFunction ConfidenceNorm { get; }
		IBuiltInFunction ConfidenceT { get; }
		IBuiltInFunction Correl { get; }
		IBuiltInFunction Count { get; }
		IBuiltInFunction CountA { get; }
		IBuiltInFunction CountBlank { get; }
		IBuiltInFunction CountIf { get; }
		IBuiltInFunction CountIfS { get; }
		IBuiltInFunction CovarianceP { get; }
		IBuiltInFunction CovarianceS { get; }
		IBuiltInFunction Devsq { get; }
		IBuiltInFunction ExponDist { get; }
		IBuiltInFunction FDist { get; }
		IBuiltInFunction FDistRt { get; }
		IBuiltInFunction FInv { get; }
		IBuiltInFunction FInvRt { get; }
		IBuiltInFunction FTest { get; }
		IBuiltInFunction Fisher { get; }
		IBuiltInFunction FisherInv { get; }
		IBuiltInFunction Forecast { get; }
		IBuiltInFunction Frequency { get; }
		IBuiltInFunction Gamma { get; }
		IBuiltInFunction GammaDist { get; }
		IBuiltInFunction GammaInv { get; }
		IBuiltInFunction GammaLn { get; }
		IBuiltInFunction GammaLnPrecise { get; }
		IBuiltInFunction Gauss { get; }
		IBuiltInFunction Geomean { get; }
		IBuiltInFunction Growth { get; }
		IBuiltInFunction HarMean { get; }
		IBuiltInFunction HypGeomDist { get; }
		IBuiltInFunction Intercept { get; }
		IBuiltInFunction Kurt { get; }
		IBuiltInFunction Large { get; }
		IBuiltInFunction Linest { get; }
		IBuiltInFunction Logest { get; }
		IBuiltInFunction LogNormDist { get; }
		IBuiltInFunction LogNormInv { get; }
		IBuiltInFunction Max { get; }
		IBuiltInFunction MaxA { get; }
		IBuiltInFunction Median { get; }
		IBuiltInFunction Min { get; }
		IBuiltInFunction MinA { get; }
		IBuiltInFunction ModeMult { get; }
		IBuiltInFunction ModeSngl { get; }
		IBuiltInFunction NegBinomDist { get; }
		IBuiltInFunction NormDist { get; }
		IBuiltInFunction NormInv { get; }
		IBuiltInFunction NormSDist { get; }
		IBuiltInFunction NormSInv { get; }
		IBuiltInFunction Pearson { get; }
		IBuiltInFunction PercentileExc { get; }
		IBuiltInFunction PercentileInc { get; }
		IBuiltInFunction PercentRankExc { get; }
		IBuiltInFunction PercentRankInc { get; }
		IBuiltInFunction Permut { get; }
		IBuiltInFunction Permutationa { get; }
		IBuiltInFunction Phi { get; }
		IBuiltInFunction PoissonDist { get; }
		IBuiltInFunction Prob { get; }
		IBuiltInFunction QuartileExc { get; }
		IBuiltInFunction QuartileInc { get; }
		IBuiltInFunction RankAvg { get; }
		IBuiltInFunction RankEq { get; }
		IBuiltInFunction Rsq { get; }
		IBuiltInFunction Skew { get; }
		IBuiltInFunction SkewP { get; }
		IBuiltInFunction Slope { get; }
		IBuiltInFunction Small { get; }
		IBuiltInFunction Standardize { get; }
		IBuiltInFunction StdevP { get; }
		IBuiltInFunction StdevS { get; }
		IBuiltInFunction Stdeva { get; }
		IBuiltInFunction Stdevpa { get; }
		IBuiltInFunction Steyx { get; }
		IBuiltInFunction TDist { get; }
		IBuiltInFunction TDist2t { get; }
		IBuiltInFunction TDistRt { get; }
		IBuiltInFunction TInv { get; }
		IBuiltInFunction TInv2t { get; }
		IBuiltInFunction TTest { get; }
		IBuiltInFunction Trend { get; }
		IBuiltInFunction TrimMean { get; }
		IBuiltInFunction VarP { get; }
		IBuiltInFunction VarS { get; }
		IBuiltInFunction Vara { get; }
		IBuiltInFunction Varpa { get; }
		IBuiltInFunction WeibullDist { get; }
		IBuiltInFunction ZTest { get; }
	}
	public interface ITextFunctions {
		IBuiltInFunction BahtText { get; }
		IBuiltInFunction Char { get; }
		IBuiltInFunction Clean { get; }
		IBuiltInFunction Code { get; }
		IBuiltInFunction Concatenate { get; }
		IBuiltInFunction Dollar { get; }
		IBuiltInFunction Exact { get; }
		IBuiltInFunction Find { get; }
		IBuiltInFunction Fixed { get; }
		IBuiltInFunction Left { get; }
		IBuiltInFunction Len { get; }
		IBuiltInFunction Lower { get; }
		IBuiltInFunction Mid { get; }
		IBuiltInFunction NumberValue { get; }
		IBuiltInFunction Proper { get; }
		IBuiltInFunction Replace { get; }
		IBuiltInFunction Rept { get; }
		IBuiltInFunction Right { get; }
		IBuiltInFunction Search { get; }
		IBuiltInFunction Substitute { get; }
		IBuiltInFunction T { get; }
		IBuiltInFunction Text { get; }
		IBuiltInFunction Trim { get; }
		IBuiltInFunction Unicode { get; }
		IBuiltInFunction Upper { get; }
		IBuiltInFunction Value { get; }
	}
	public interface IWebFunctions {
		IBuiltInFunction EncodeUrl { get; }
	}
	#endregion
	public abstract class FunctionWrapper : ICustomFunction {
		IFunction innerFunction;
		protected FunctionWrapper(IFunction function) {
			this.innerFunction = function;
		}
		#region ICustomFunction Members
		public string Name {
			get { return innerFunction.Name; }
		}
		public ParameterType ReturnType {
			get { return innerFunction.ReturnType; }
		}
		public ParameterInfo[] Parameters {
			get { return innerFunction.Parameters; }
		}
		public virtual bool Volatile { get { return innerFunction.Volatile; } }
		public virtual ParameterValue Evaluate(IList<ParameterValue> parameters, EvaluationContext context) {
			return innerFunction.Evaluate(parameters, context);
		}
		public virtual string GetName(CultureInfo culture) {
			return innerFunction.GetName(culture);
		}
		#endregion
	}
#if DXPORTABLE
	#region AsyncCustomFunction
	public abstract class AsyncCustomFunction : ICustomFunction {
		[ThreadStatic]
		static AsyncCustomFunctionCache cache = new AsyncCustomFunctionCache();
		static internal AsyncCustomFunctionCache Cache {
			get {
				if (cache == null)
					cache = new AsyncCustomFunctionCache();
				return cache;
			}
		}
		public abstract string Name { get; }
		public abstract ParameterType ReturnType { get; }
		public abstract ParameterInfo[] Parameters { get; }
		public abstract bool Volatile { get; }
		public abstract string GetName(CultureInfo culture);
		public virtual ParameterValue ValueUntilIsCalculated { get { return ParameterValue.ErrorValueNotAvailable; } }
		public abstract ParameterValue EvaluateAsync(IList<ParameterValue> parameters, EvaluationContext context);
		public ParameterValue Evaluate(IList<ParameterValue> parameters, EvaluationContext context) {
			DevExpress.XtraSpreadsheet.Model.DocumentModel workbook = ((DevExpress.XtraSpreadsheet.API.Native.Implementation.NativeWorksheet)context.Sheet).ModelWorkbook;
			ParameterValue result;
			if (Cache.TryGetValue(parameters, Name, Volatile, workbook.ContentVersion, ValueUntilIsCalculated, out result))
				return result;
			object[] workerArgument = new object[] { parameters, context };
			Task<object[]> task = DoWork(this, workerArgument);
			task.ContinueWith((t) => { RunWorkerCompleted(this, t.Result); });
			task.Start();
			return ValueUntilIsCalculated;
		}
		Task<object[]> DoWork(object sender, object[] workerArgument) {
			object[] parameters = workerArgument;
			IList<ParameterValue> arguments = parameters[0] as IList<ParameterValue>;
			EvaluationContext context = parameters[1] as EvaluationContext;
			Task<object[]> task = new Task<object[]>(() => {
				ParameterValue resultValue = EvaluateAsync(arguments, context);
				return new object[] { resultValue, arguments, context };
			});
			return task;
		}
		internal virtual void RunWorkerCompleted(object sender, object[] result) {
			ParameterValue resultValue = (ParameterValue)result[0];
			IList<ParameterValue> arguments = (IList<ParameterValue>)result[1];
			EvaluationContext context = (EvaluationContext)result[2];
			DevExpress.XtraSpreadsheet.Model.DocumentModel workbook = ((DevExpress.XtraSpreadsheet.API.Native.Implementation.NativeWorksheet)context.Sheet).ModelWorkbook;
			Cache.Register(arguments, Name, Volatile, workbook.ContentVersion, resultValue);
			workbook.IncrementContentVersion();
			Redraw(workbook);
		}
		void Redraw(DevExpress.XtraSpreadsheet.Model.DocumentModel documentModel) {
			if (documentModel.IsUpdateLocked)
				documentModel.ApplyChanges(DevExpress.XtraSpreadsheet.Model.DocumentModelChangeActions.ResetAllLayout);
			else
				documentModel.InnerApplyChanges(DevExpress.XtraSpreadsheet.Model.DocumentModelChangeActions.ResetAllLayout);
		}
	}
	#endregion
#else
	#region AsyncCustomFunction
	public abstract class AsyncCustomFunction : ICustomFunction {
		[ThreadStatic]
		static AsyncCustomFunctionCache cache = new AsyncCustomFunctionCache();
		static internal AsyncCustomFunctionCache Cache {
			get {
				if (cache == null)
					cache = new AsyncCustomFunctionCache();
				return cache;
			}
		}
		public abstract string Name { get; }
		public abstract ParameterType ReturnType { get; }
		public abstract ParameterInfo[] Parameters { get; }
		public abstract bool Volatile { get; }
		public abstract string GetName(CultureInfo culture);
		public virtual ParameterValue ValueUntilIsCalculated { get { return ParameterValue.ErrorValueNotAvailable; } }
		public abstract ParameterValue EvaluateAsync(IList<ParameterValue> parameters, EvaluationContext context);
		public ParameterValue Evaluate(IList<ParameterValue> parameters, EvaluationContext context) {
			DevExpress.XtraSpreadsheet.Model.DocumentModel workbook = ((DevExpress.XtraSpreadsheet.API.Native.Implementation.NativeWorksheet)context.Sheet).ModelWorkbook;
			ParameterValue result;
			if (Cache.TryGetValue(parameters, Name, Volatile, workbook.ContentVersion, ValueUntilIsCalculated, out result))
				return result;
			object[] workerArgument = new object[] { parameters, context };
			BackgroundWorker worker = new BackgroundWorker();
			worker.DoWork += new DoWorkEventHandler(DoWork);
			worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunWorkerCompleted);
			worker.RunWorkerAsync(workerArgument);
			return ValueUntilIsCalculated;
		}
		void DoWork(object sender, DoWorkEventArgs e) {
			object[] parameters = e.Argument as object[];
			IList<ParameterValue> arguments = parameters[0] as IList<ParameterValue>;
			EvaluationContext context = parameters[1] as EvaluationContext;
			ParameterValue resultValue = EvaluateAsync(arguments, context);
			e.Result = new object[] { resultValue, arguments, context };
		}
		internal virtual void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
			object[] result = e.Result as object[];
			ParameterValue resultValue = (ParameterValue)result[0];
			IList<ParameterValue> arguments = (IList<ParameterValue>)result[1];
			EvaluationContext context = (EvaluationContext)result[2];
			DevExpress.XtraSpreadsheet.Model.Worksheet worksheet = ((DevExpress.XtraSpreadsheet.API.Native.Implementation.NativeWorksheet)context.Sheet).ModelWorksheet;
			DevExpress.XtraSpreadsheet.Model.DocumentModel workbook = worksheet.Workbook;
			Cache.Register(arguments, Name, Volatile, workbook.ContentVersion, resultValue);
			DevExpress.XtraSpreadsheet.Model.ICell cell = worksheet[context.Column, context.Row];
			workbook.CalculationChain.MarkupDependentsForRecalculationForWholeCellFormula(cell);
			Redraw(workbook);
		}
		void Redraw(DevExpress.XtraSpreadsheet.Model.DocumentModel documentModel) {
			if (documentModel.IsUpdateLocked)
				documentModel.ApplyChanges(DevExpress.XtraSpreadsheet.Model.DocumentModelChangeActions.ResetAllLayout);
			else
				documentModel.InnerApplyChanges(DevExpress.XtraSpreadsheet.Model.DocumentModelChangeActions.ResetAllLayout);
		}
	}
	#endregion
#endif
	#region AsyncCustomFunctionCache
	class AsyncCustomFunctionCache {
		readonly Dictionary<string, ParameterValue> constCache = new Dictionary<string, ParameterValue>();
		readonly List<string> volatileFunctionsInCalculation = new List<string>();
		readonly Dictionary<string, ParameterValue> volatileCache = new Dictionary<string, ParameterValue>();
		int volatileCacheIsActualForContentVersion;
		public Dictionary<string, ParameterValue> ConstCache { get { return constCache; } }
		public List<string> VolatileFunctionsInCalculation { get { return volatileFunctionsInCalculation; } }
		public Dictionary<string, ParameterValue> VolatileCache { get { return volatileCache; } }
		public int VolatileCacheIsActualForContentVersion { get { return volatileCacheIsActualForContentVersion; } }
		public bool TryGetValue(IList<ParameterValue> parameters, string functionName, bool isVolatile, int wbContentVersion, ParameterValue ifCalculating, out ParameterValue result) {
			string key = GenerateKey(parameters, functionName);
			if (isVolatile) {
				if (volatileFunctionsInCalculation.Count < 1 && wbContentVersion != volatileCacheIsActualForContentVersion)
					volatileCache.Clear();
				if (volatileCache.TryGetValue(key, out result))
					return true;
				result = ifCalculating;
				if (volatileFunctionsInCalculation.Contains(key)) 
					return true;
				volatileFunctionsInCalculation.Add(key);
				return false;
			}
			if (constCache.TryGetValue(key, out result))
				return true;
			constCache.Add(key, ifCalculating); 
			return false;
		}
		public void Register(IList<ParameterValue> parameters, string functionName, bool isVolatile, int wbContentVersion, ParameterValue value) {
			string key = GenerateKey(parameters, functionName);
			if (isVolatile) {
				volatileFunctionsInCalculation.Remove(key);
				volatileCache.Add(key, value);
				volatileCacheIsActualForContentVersion = wbContentVersion + 1;
			}
			else
				constCache[key] = value;
		}
		string GenerateKey(IList<ParameterValue> parameters, string functionName) {
			StringBuilder sb = new StringBuilder();
			sb.Append(functionName);
			foreach (ParameterValue parameter in parameters) {
				sb.Append('/');
				sb.Append(parameter);
			}
			return sb.ToString();
		}
	}
	#endregion
}
#region WorkbookFunctions implementation
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Office.Utils;
	using DevExpress.Spreadsheet;
	using DevExpress.Spreadsheet.Formulas;
	using DevExpress.Spreadsheet.Functions;
	using DevExpress.XtraSpreadsheet.Localization;
	using DevExpress.XtraSpreadsheet.Utils;
	using ModelCustomFunction = DevExpress.XtraSpreadsheet.Model.CustomFunction;
	using ModelFormulaCalculator = DevExpress.XtraSpreadsheet.Model.FormulaCalculator;
	using ModelWorkbookDataContext = DevExpress.XtraSpreadsheet.Model.WorkbookDataContext;
	using DevExpress.Export.Xl;
	#region NativeEvaluationContext
	partial class NativeEvaluationContext : IExpressionContext {
		readonly NativeWorkbook workbook;
		readonly ModelWorkbookDataContext modelContext;
		public NativeEvaluationContext(NativeWorkbook workbook) {
			this.workbook = workbook;
			this.modelContext = workbook.Model.DocumentModel.DataContext;
		}
		#region IExpressionContext Members
		public CultureInfo Culture { get { return modelContext.Culture; } }
		public int Column { get { return modelContext.CurrentColumnIndex; } }
		public int Row { get { return modelContext.CurrentRowIndex; } }
		public Worksheet Sheet { get { return modelContext.CurrentWorksheet == null ? null : workbook.Worksheets[modelContext.CurrentWorksheet.Name]; } }
		public ReferenceStyle ReferenceStyle { get { return Spreadsheet.ReferenceStyle.UseDocumentSettings; } }
		public bool IsArrayFormula { get { return false; } }
		public ExpressionStyle ExpressionStyle { get { return ExpressionStyle.Normal; } }
		#endregion
	}
	#endregion
	#region NativeFunctionsBase
	abstract partial class NativeFunctionsBase {
		#region Fields
		readonly NativeWorkbook workbook;
		readonly Model.ICustomFunctionProvider functionProvider;
		Dictionary<string, ICustomFunction> innerList;
		#endregion
		protected NativeFunctionsBase(NativeWorkbook workbook, Model.ICustomFunctionProvider functionProvider) {
			this.functionProvider = functionProvider;
			this.workbook = workbook;
		}
		#region Properties
		protected Dictionary<string, ICustomFunction> InnerList { get { return innerList; } set { innerList = value; } }
		protected NativeWorkbook Workbook { get { return workbook; } }
		protected Model.ICustomFunctionProvider FunctionProvider { get { return functionProvider; } }
		protected Model.WorkbookDataContext Context { get { return workbook.ModelWorkbook.DataContext; } }
		#endregion
		protected internal bool Contains(string name) {
			return innerList.ContainsKey(name);
		}
		protected internal void InternalAPI_CustomFunctionEvaluation(Model.CustomFunctionEvaluateEventArgs args) {
			args.Result = EvaluateFunction(args.Name, args.Arguments);
		}
		protected internal Model.VariantValue EvaluateFunction(string name, IList<Model.VariantValue> arguments) {
			ICustomFunction function = innerList[name];
			if (function == null)
				return Model.VariantValue.ErrorName;
			Model.WorkbookDataContext context = Context;
			List<ParameterValue> nativeArguments = new List<ParameterValue>();
			foreach (Model.VariantValue value in arguments) {
				if (value.IsCellRange) {
					Model.CellRangeBase modelCellRangeBase = value.CellRangeValue;
					if (modelCellRangeBase.Worksheet == null)
						modelCellRangeBase.Worksheet = context.CurrentWorksheet;
					nativeArguments.Add(new ParameterValue(value, workbook));
				}
				else
					nativeArguments.Add(new ParameterValue(value, context));
			}
			NativeEvaluationContext nativeContext = new NativeEvaluationContext(workbook);
			ParameterValue nativeResult = function.Evaluate(nativeArguments, nativeContext);
			if (nativeResult == null)
				return 0;
			return nativeResult.ModelVariantValue;
		}
		protected Model.FunctionParameterCollection PrepareParameters(ParameterInfo[] parameters) {
			if (parameters == null || parameters.Length == 0)
				return Model.WorksheetParameterlessFunctionBase.EmptyParametersList;
			Model.FunctionParameterCollection result = new Model.FunctionParameterCollection();
			foreach (ParameterInfo nativeParameter in parameters) {
				Model.OperandDataType dataType = (Model.OperandDataType)nativeParameter.Type;
				Model.FunctionParameterOption options = (Model.FunctionParameterOption)nativeParameter.Attributes;
				if (!nativeParameter.ConvertEmptyValueToZero)
					options |= Model.FunctionParameterOption.DoNotDereferenceEmptyValueAsZero;
				result.Add(new Model.FunctionParameter(dataType, options));
			}
			return result;
		}
	}
	#endregion
	#region NativeWorkbookFunctions
	partial class NativeWorkbookFunctions : NativeFunctionsBase, WorkbookFunctions {
		#region Fields
		readonly NativeCompatibilityFunctions CompatibilityFunctions;
		readonly NativeDatabaseFunctions DatabaseFunctions;
		readonly NativeDateAndTimeFunctions DateAndTimeFunctions;
		readonly NativeEngineeringFunctions EngineeringFunctions;
		readonly NativeFinancialFunctions FinancialFunctions;
		readonly NativeInformationFunctions InformationalFunctions;
		readonly NativeLogicalFunctions LogicalFunctions;
		readonly NativeLookupAndReferenceFunctions LookupAndReferenceFunctions;
		readonly NativeMathAndTrigonometryFunctions MathFunctions;
		readonly NativeStatisticalFunctions StatisticalFunctions;
		readonly NativeTextFunctions TextFunctions;
		readonly NativeWebFunctions WebFunctions;
		NativeCustomFunctions globalCustomFunctions;
		NativeCustomFunctions customFunctions;
		#endregion
		public NativeWorkbookFunctions(NativeWorkbook workbook, Model.ICustomFunctionProvider functionProvider)
			: base(workbook, functionProvider) {
			this.InnerList = new Dictionary<string, ICustomFunction>();
			CompatibilityFunctions = new NativeCompatibilityFunctions(this);
			DatabaseFunctions = new NativeDatabaseFunctions(this);
			DateAndTimeFunctions = new NativeDateAndTimeFunctions(this);
			EngineeringFunctions = new NativeEngineeringFunctions(this);
			FinancialFunctions = new NativeFinancialFunctions(this);
			InformationalFunctions = new NativeInformationFunctions(this);
			LogicalFunctions = new NativeLogicalFunctions(this);
			LookupAndReferenceFunctions = new NativeLookupAndReferenceFunctions(this);
			MathFunctions = new NativeMathAndTrigonometryFunctions(this);
			StatisticalFunctions = new NativeStatisticalFunctions(this);
			TextFunctions = new NativeTextFunctions(this);
			WebFunctions = new NativeWebFunctions(this);
		}
		#region WorkbookFunctions Members
		public CustomFunctionCollection GlobalCustomFunctions { get { return NativeGlobalCustomFunctions; } }
		public CustomFunctionCollection CustomFunctions { get { return NativeCustomFunctions; } }
		public IBuiltInFunction this[string name] {
			get { return GetBuiltInByName(name); }
		}
		public ICompatibilityFunctions Compatibility { get { return CompatibilityFunctions; } }
		public IDatabaseFunctions Database { get { return DatabaseFunctions; } }
		public IDateAndTimeFunctions DateAndTime { get { return DateAndTimeFunctions; } }
		public IEngineeringFunctions Engineering { get { return EngineeringFunctions; } }
		public IFinancialFunctions Financial { get { return FinancialFunctions; } }
		public IInformationFunctions Informational { get { return InformationalFunctions; } }
		public ILogicalFunctions Logical { get { return LogicalFunctions; } }
		public ILookupAndReferenceFunctions LookupAndReference { get { return LookupAndReferenceFunctions; } }
		public IMathAndTrigonometryFunctions Math { get { return MathFunctions; } }
		public IStatisticalFunctions Statistical { get { return StatisticalFunctions; } }
		public ITextFunctions Text { get { return TextFunctions; } }
		public IWebFunctions Web { get { return WebFunctions; } }
		public void OverrideFunction(string name, ICustomFunction function) {
			OverrideFunction(name, function, false);
		}
		public void OverrideFunction(string name, ICustomFunction function, bool skipIfExists) {
			if (function == null)
				throw new ArgumentNullException("function");
			Model.ISpreadsheetFunction modelBuildInfunction = GetModelBuiltInByName(name);
			if (modelBuildInfunction == null)
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_BuiltInFunctionNotFound);
			if (skipIfExists && !(modelBuildInfunction is Model.NotSupportedFunction))
				return;
			Model.FunctionParameterCollection parameters = PrepareParameters(function.Parameters);
			ModelCustomFunction modelCustomFunction = new ModelCustomFunction(function, parameters);
			CompareParameters(modelBuildInfunction, modelCustomFunction);
			FunctionProvider.RegisterFunction(name, modelCustomFunction, Context.Culture);
			InnerList.Add(name, function);
		}
		#endregion
		protected internal NativeCustomFunctions NativeGlobalCustomFunctions {
			get {
				if (globalCustomFunctions == null)
					globalCustomFunctions = new NativeCustomFunctions(Workbook, Model.FormulaCalculator.CustomFunctionProvider, true);
				return globalCustomFunctions;
			}
		}
		protected internal NativeCustomFunctions NativeCustomFunctions {
			get {
				if (customFunctions == null)
					customFunctions = new NativeCustomFunctions(Workbook, Workbook.ModelWorkbook.CustomFunctionProvider);
				return customFunctions;
			}
		}
		protected internal virtual void OnCustomFunctionEvaluation(object sender, Model.CustomFunctionEvaluateEventArgs args) {
			if (Contains(args.Name))
				InternalAPI_CustomFunctionEvaluation(args);
			else if (NativeCustomFunctions.Contains(args.Name))
				NativeCustomFunctions.InternalAPI_CustomFunctionEvaluation(args);
			else if (NativeGlobalCustomFunctions.Contains(args.Name))
				NativeGlobalCustomFunctions.InternalAPI_CustomFunctionEvaluation(args);
		}
		void CompareParameters(Model.ISpreadsheetFunction function, Model.ISpreadsheetFunction customFunction) {
			if (function.IsVolatile != customFunction.IsVolatile)
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_BuiltInReplaceInvalidVolatile);
			if (function.ReturnDataType != customFunction.ReturnDataType)
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_BuiltInReplaceInvalidReturnType);
			if (function.Parameters.Count != customFunction.Parameters.Count)
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_BuiltInReplaceInvalidParametersCount);
			for (int i = 0; i < function.Parameters.Count; i++)
				if (!function.Parameters[i].Equals(customFunction.Parameters[i]))
					Exceptions.ThrowInvalidOperationException(string.Format(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_BuiltInReplaceInvalidParameter), i));
		}
		IBuiltInFunction GetBuiltInByName(string name) {
			Model.ISpreadsheetFunction modelBuildInfunction = GetModelBuiltInByName(name);
			if (modelBuildInfunction == null)
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_BuiltInFunctionNotFound);
			return new NativeBuiltInFunction(modelBuildInfunction, Workbook);
		}
		Model.ISpreadsheetFunction GetModelBuiltInByName(string name) {
			Model.ISpreadsheetFunction modelBuildInfunction = ModelFormulaCalculator.FunctionProvider.GetFunctionByInvariantName(name);
			if (modelBuildInfunction == null )
				return null;
			return modelBuildInfunction;
		}
	}
	#endregion
	#region BuiltInFunction
	partial class NativeBuiltInFunction : IBuiltInFunction {
		#region Fields
		readonly Model.ISpreadsheetFunction function;
		readonly NativeWorkbook workbook;
		ParameterInfo[] parameters;
		#endregion
		public NativeBuiltInFunction(Model.ISpreadsheetFunction function, NativeWorkbook workbook) {
			this.function = function;
			this.workbook = workbook;
		}
		#region IBuiltInFunction Members
		public string Name { get { return function.Name; } }
		public ParameterType ReturnType { get { return (ParameterType)function.ReturnDataType; } }
		public bool Volatile { get { return function.IsVolatile; } }
		public Model.WorkbookDataContext ModelContext { get { return workbook.ModelWorkbook.DataContext; } }
		public Model.ISpreadsheetFunction ModelFunction { get { return function; } }
		public ParameterInfo[] Parameters {
			get {
				if (parameters == null)
					PrepareParameters();
				return parameters;
			}
		}
		public string GetName(CultureInfo culture) {
			return Model.FormulaCalculator.FunctionProvider.GetFunctionNameForCulture(function, culture);
		}
		public ParameterValue Evaluate(params ParameterValue[] parameters) {
			List<ParameterValue> ListParameters = new List<ParameterValue>();
			if (parameters != null)
				ListParameters.AddRange(parameters);
			return Evaluate(ListParameters, new ActiveSettingsBasedExpressionContext(workbook));
		}
		public ParameterValue Evaluate(IList<ParameterValue> parameters, EvaluationContext context) {
			Model.WorkbookDataContext modelContext = ModelContext;
			int columnIndex = 0;
			int rowIndex = 0;
			Model.IWorksheet currentSheet = modelContext.CurrentWorksheet;
			CultureInfo culture = modelContext.Culture;
			if (context != null) {
				columnIndex = context.Column;
				rowIndex = context.Row;
				if (context.Culture != null)
					culture = context.Culture;
				if (context.Sheet != null)
					currentSheet = ((NativeWorksheet)context.Sheet).ModelWorksheet;
			}
			CheckParameterCount(parameters);
			modelContext.PushCurrentWorksheet(currentSheet);
			modelContext.PushCurrentCell(columnIndex, rowIndex);
			modelContext.PushCulture(culture);
			try {
				List<Model.VariantValue> arguments = PrepareEvaluationArguments(parameters);
				Model.VariantValue result = function.Evaluate(arguments, modelContext, false);
				return PrepareEvaluationResult(result);
			}
			finally {
				modelContext.PopCulture();
				modelContext.PopCurrentCell();
				modelContext.PopCurrentWorksheet();
			}
		}
		#endregion
		void PrepareParameters() {
			if (function.Parameters == null || function.Parameters.Count == 0) {
				parameters = new ParameterInfo[0];
				return;
			}
			int parameterCount = function.Parameters.Count;
			parameters = new ParameterInfo[parameterCount];
			for (int i = 0; i < parameterCount; i++) {
				Model.FunctionParameter modelParameter = function.Parameters[i];
				ParameterType dataType = (ParameterType)modelParameter.DataType;
				ParameterAttributes attributes = (ParameterAttributes)modelParameter.Options;
				ParameterInfo parameter = new ParameterInfo(dataType, attributes);
				if (modelParameter.DereferenceEmptyValueAsZero)
					parameter.ConvertEmptyValueToZero = true;
				parameters[i] = parameter;
			}
		}
		#region Evaluation
		void CheckParameterCount(IList<ParameterValue> parameters) {
			Model.FunctionParameter lastParameter = null;
			int functionParametersCount = function.Parameters.Count;
			int parametersCount = parameters == null ? 0 : parameters.Count;
			if (functionParametersCount > 0)
				lastParameter = function.Parameters[functionParametersCount - 1];
			if (parametersCount == 0 && functionParametersCount > 0 && function.Parameters[0].Required)
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_BuiltInFunctionNotFound); 
			if (parametersCount < functionParametersCount) {
				if (function.Parameters[parametersCount].Required)
					SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_BuiltInFunctionNotFound); 
			}
			if (parametersCount > functionParametersCount && (lastParameter == null || !lastParameter.Unlimited))
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_BuiltInFunctionNotFound); 
		}
		List<Model.VariantValue> PrepareEvaluationArguments(IList<ParameterValue> parameters) {
			Model.WorkbookDataContext modelContext = ModelContext;
			int parametersCount = parameters == null ? 0 : parameters.Count;
			List<Model.VariantValue> arguments = new List<Model.VariantValue>();
			if (parametersCount <= 0)
				return arguments;
			for (int i = 0; i < parametersCount; i++) {
				Model.VariantValue argument = parameters[i].ModelVariantValue;
				Model.FunctionParameter functionParameter = function.GetParameterByExpressionIndex(i);
				if (functionParameter.DataType == Model.OperandDataType.Reference && !argument.IsCellRange)
					SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_BuiltInFunctionNotFound); 
				if (argument.IsEmpty) {
					if (functionParameter.DereferenceEmptyValueAsZero)
						argument = 0;
				}
				else if (argument.IsMissing)
					argument = Model.VariantValue.Empty;
				else if (argument.IsCellRange) {
					if ((functionParameter.DataType & Model.OperandDataType.Reference) <= 0) {
						if ((functionParameter.DataType & Model.OperandDataType.Array) > 0) {
							if (argument.CellRangeValue.RangeType == Model.CellRangeType.UnionRange)
								argument = Model.VariantValue.ErrorInvalidValueInFunction;
							else {
								Model.RangeVariantArray arrayValue = new Model.RangeVariantArray((Model.CellRange)argument.CellRangeValue);
								argument = Model.VariantValue.FromArray(arrayValue);
							}
						}
						else
							argument = modelContext.DereferenceValue(argument, false);
					}
				}
				else if (argument.IsArray) {
					if ((functionParameter.DataType & Model.OperandDataType.Array) <= 0)
						argument = modelContext.DereferenceValue(argument, false);
				}
				arguments.Add(argument);
			}
			return arguments;
		}
		ParameterValue PrepareEvaluationResult(Model.VariantValue value) {
			if (value.IsCellRange) {
				if (value.CellRangeValue.Worksheet == null)
					value.CellRangeValue.Worksheet = ModelContext.CurrentWorksheet;
				return new ParameterValue(value, workbook);
			}
			else
				return new ParameterValue(value, ModelContext);
		}
		#endregion
	}
	#endregion
	#region BuiltInFunctionCategories
	abstract partial class NativeFunctionCategoryBase {
		NativeWorkbookFunctions functions;
		protected NativeFunctionCategoryBase(NativeWorkbookFunctions functions) {
			this.functions = functions;
		}
		protected NativeWorkbookFunctions Functions { get { return functions; } }
	}
	partial class NativeCompatibilityFunctions : NativeFunctionCategoryBase, ICompatibilityFunctions {
		public NativeCompatibilityFunctions(NativeWorkbookFunctions functions)
			: base(functions) {
		}
		public IBuiltInFunction BetaDist { get { return Functions["BETADIST"]; } }
		public IBuiltInFunction BetaInv { get { return Functions["BETAINV"]; } }
		public IBuiltInFunction BinomDist { get { return Functions["BINOMDIST"]; } }
		public IBuiltInFunction ChiDist { get { return Functions["CHIDIST"]; } }
		public IBuiltInFunction ChiInv { get { return Functions["CHIINV"]; } }
		public IBuiltInFunction ChiTest { get { return Functions["CHITEST"]; } }
		public IBuiltInFunction Confidence { get { return Functions["CONFIDENCE"]; } }
		public IBuiltInFunction Covar { get { return Functions["COVAR"]; } }
		public IBuiltInFunction CritBinom { get { return Functions["CRITBINOM"]; } }
		public IBuiltInFunction ExponDist { get { return Functions["EXPONDIST"]; } }
		public IBuiltInFunction FDist { get { return Functions["FDIST"]; } }
		public IBuiltInFunction FInv { get { return Functions["FINV"]; } }
		public IBuiltInFunction FTest { get { return Functions["FTEST"]; } }
		public IBuiltInFunction GammaDist { get { return Functions["GAMMADIST"]; } }
		public IBuiltInFunction GammaInv { get { return Functions["GAMMAINV"]; } }
		public IBuiltInFunction HypGeomDist { get { return Functions["HYPGEOMDIST"]; } }
		public IBuiltInFunction LogInv { get { return Functions["LOGINV"]; } }
		public IBuiltInFunction LogNormDist { get { return Functions["LOGNORMDIST"]; } }
		public IBuiltInFunction Mode { get { return Functions["MODE"]; } }
		public IBuiltInFunction NegBinomDist { get { return Functions["NEGBINOMDIST"]; } }
		public IBuiltInFunction NormDist { get { return Functions["NORMDIST"]; } }
		public IBuiltInFunction NormInv { get { return Functions["NORMINV"]; } }
		public IBuiltInFunction NormSDist { get { return Functions["NORMSDIST"]; } }
		public IBuiltInFunction NormSInv { get { return Functions["NORMSINV"]; } }
		public IBuiltInFunction Percentile { get { return Functions["PERCENTILE"]; } }
		public IBuiltInFunction PercentRank { get { return Functions["PERCENTRANK"]; } }
		public IBuiltInFunction Poisson { get { return Functions["POISSON"]; } }
		public IBuiltInFunction Quartile { get { return Functions["QUARTILE"]; } }
		public IBuiltInFunction Rank { get { return Functions["RANK"]; } }
		public IBuiltInFunction StDev { get { return Functions["STDEV"]; } }
		public IBuiltInFunction StDevP { get { return Functions["STDEVP"]; } }
		public IBuiltInFunction TDist { get { return Functions["TDIST"]; } }
		public IBuiltInFunction TInv { get { return Functions["TINV"]; } }
		public IBuiltInFunction TTest { get { return Functions["TTEST"]; } }
		public IBuiltInFunction Var { get { return Functions["VAR"]; } }
		public IBuiltInFunction VarP { get { return Functions["VARP"]; } }
		public IBuiltInFunction Weibull { get { return Functions["WEIBULL"]; } }
		public IBuiltInFunction ZTest { get { return Functions["ZTEST"]; } }
	}
	partial class NativeCubeFunctions : NativeFunctionCategoryBase, ICubeFunctions {
		public NativeCubeFunctions(NativeWorkbookFunctions functions)
			: base(functions) {
		}
	}
	partial class NativeDatabaseFunctions : NativeFunctionCategoryBase, IDatabaseFunctions {
		public NativeDatabaseFunctions(NativeWorkbookFunctions functions)
			: base(functions) {
		}
		public IBuiltInFunction DAverage { get { return Functions["DAVERAGE"]; } }
		public IBuiltInFunction DCount { get { return Functions["DCOUNT"]; } }
		public IBuiltInFunction DCountA { get { return Functions["DCOUNTA"]; } }
		public IBuiltInFunction DGet { get { return Functions["DGET"]; } }
		public IBuiltInFunction DMax { get { return Functions["DMAX"]; } }
		public IBuiltInFunction DMin { get { return Functions["DMIN"]; } }
		public IBuiltInFunction DProduct { get { return Functions["DPRODUCT"]; } }
		public IBuiltInFunction DStDev { get { return Functions["DSTDEV"]; } }
		public IBuiltInFunction DStDevP { get { return Functions["DSTDEVP"]; } }
		public IBuiltInFunction DSum { get { return Functions["DSUM"]; } }
		public IBuiltInFunction DVar { get { return Functions["DVAR"]; } }
		public IBuiltInFunction DVarP { get { return Functions["DVARP"]; } }
	}
	partial class NativeDateAndTimeFunctions : NativeFunctionCategoryBase, IDateAndTimeFunctions {
		public NativeDateAndTimeFunctions(NativeWorkbookFunctions functions)
			: base(functions) {
		}
		public IBuiltInFunction Date { get { return Functions["DATE"]; } }
		public IBuiltInFunction DateDif { get { return Functions["DATEDIF"]; } }
		public IBuiltInFunction DateValue { get { return Functions["DATEVALUE"]; } }
		public IBuiltInFunction Day { get { return Functions["DAY"]; } }
		public IBuiltInFunction Days { get { return Functions["DAYS"]; } }
		public IBuiltInFunction Days360 { get { return Functions["DAYS360"]; } }
		public IBuiltInFunction EDate { get { return Functions["EDATE"]; } }
		public IBuiltInFunction EOMonth { get { return Functions["EOMONTH"]; } }
		public IBuiltInFunction Hour { get { return Functions["HOUR"]; } }
		public IBuiltInFunction ISOWeekNum { get { return Functions["ISOWEEKNUM"]; } }
		public IBuiltInFunction Minute { get { return Functions["MINUTE"]; } }
		public IBuiltInFunction Month { get { return Functions["MONTH"]; } }
		public IBuiltInFunction NetworkDays { get { return Functions["NETWORKDAYS"]; } }
		public IBuiltInFunction NetworkDaysIntl { get { return Functions["NETWORKDAYS.INTL"]; } }
		public IBuiltInFunction Now { get { return Functions["NOW"]; } }
		public IBuiltInFunction Second { get { return Functions["SECOND"]; } }
		public IBuiltInFunction Time { get { return Functions["TIME"]; } }
		public IBuiltInFunction TimeValue { get { return Functions["TIMEVALUE"]; } }
		public IBuiltInFunction Today { get { return Functions["TODAY"]; } }
		public IBuiltInFunction WeekDay { get { return Functions["WEEKDAY"]; } }
		public IBuiltInFunction WeekNum { get { return Functions["WEEKNUM"]; } }
		public IBuiltInFunction WorkDay { get { return Functions["WORKDAY"]; } }
		public IBuiltInFunction WorkDayIntl { get { return Functions["WorkDay.INTL"]; } }
		public IBuiltInFunction Year { get { return Functions["YEAR"]; } }
		public IBuiltInFunction YearFrac { get { return Functions["YEARFRAC"]; } }
	}
	partial class NativeEngineeringFunctions : NativeFunctionCategoryBase, IEngineeringFunctions {
		public NativeEngineeringFunctions(NativeWorkbookFunctions functions)
			: base(functions) {
		}
		public IBuiltInFunction BesselI { get { return Functions["BESSELI"]; } }
		public IBuiltInFunction BesselJ { get { return Functions["BESSELJ"]; } }
		public IBuiltInFunction BesselK { get { return Functions["BESSELK"]; } }
		public IBuiltInFunction BesselY { get { return Functions["BESSELY"]; } }
		public IBuiltInFunction Bin2Dec { get { return Functions["BIN2DEC"]; } }
		public IBuiltInFunction Bin2Hex { get { return Functions["BIN2HEX"]; } }
		public IBuiltInFunction Bin2Oct { get { return Functions["BIN2OCT"]; } }
		public IBuiltInFunction BitAnd { get { return Functions["BITAND"]; } }
		public IBuiltInFunction BitLShift { get { return Functions["BITLSHIFT"]; } }
		public IBuiltInFunction BitOr { get { return Functions["BITOR"]; } }
		public IBuiltInFunction BitRShift { get { return Functions["BITRSHIFT"]; } }
		public IBuiltInFunction BitXor { get { return Functions["BITXOR"]; } }
		public IBuiltInFunction Complex { get { return Functions["COMPLEX"]; } }
		public IBuiltInFunction Convert { get { return Functions["CONVERT"]; } }
		public IBuiltInFunction Dec2Bin { get { return Functions["DEC2BIN"]; } }
		public IBuiltInFunction Dec2Hex { get { return Functions["DEC2HEX"]; } }
		public IBuiltInFunction Dec2Oct { get { return Functions["DEC2OCT"]; } }
		public IBuiltInFunction Delta { get { return Functions["DELTA"]; } }
		public IBuiltInFunction Erf { get { return Functions["ERF"]; } }
		public IBuiltInFunction ErfPrecise { get { return Functions["Erf.PRECISE"]; } }
		public IBuiltInFunction Erfc { get { return Functions["ERFC"]; } }
		public IBuiltInFunction ErfcPrecise { get { return Functions["Erfc.PRECISE"]; } }
		public IBuiltInFunction Gestep { get { return Functions["GESTEP"]; } }
		public IBuiltInFunction Hex2Bin { get { return Functions["HEX2BIN"]; } }
		public IBuiltInFunction Hex2Dec { get { return Functions["HEX2DEC"]; } }
		public IBuiltInFunction Hex2Oct { get { return Functions["HEX2OCT"]; } }
		public IBuiltInFunction ImAbs { get { return Functions["IMABS"]; } }
		public IBuiltInFunction Imaginary { get { return Functions["IMAGINARY"]; } }
		public IBuiltInFunction ImArgument { get { return Functions["IMARGUMENT"]; } }
		public IBuiltInFunction ImConjugate { get { return Functions["IMCONJUGATE"]; } }
		public IBuiltInFunction ImCos { get { return Functions["IMCOS"]; } }
		public IBuiltInFunction ImCosh { get { return Functions["IMCOSH"]; } }
		public IBuiltInFunction ImCot { get { return Functions["IMCOT"]; } }
		public IBuiltInFunction ImCsc { get { return Functions["IMCSC"]; } }
		public IBuiltInFunction ImCsch { get { return Functions["IMCSCH"]; } }
		public IBuiltInFunction ImDiv { get { return Functions["IMDIV"]; } }
		public IBuiltInFunction ImExp { get { return Functions["IMEXP"]; } }
		public IBuiltInFunction ImLn { get { return Functions["IMLN"]; } }
		public IBuiltInFunction ImLog10 { get { return Functions["IMLOG10"]; } }
		public IBuiltInFunction ImLog2 { get { return Functions["IMLOG2"]; } }
		public IBuiltInFunction ImPower { get { return Functions["IMPOWER"]; } }
		public IBuiltInFunction ImProduct { get { return Functions["IMPRODUCT"]; } }
		public IBuiltInFunction ImReal { get { return Functions["IMREAL"]; } }
		public IBuiltInFunction ImSec { get { return Functions["IMSEC"]; } }
		public IBuiltInFunction ImSech { get { return Functions["IMSECH"]; } }
		public IBuiltInFunction ImSin { get { return Functions["IMSIN"]; } }
		public IBuiltInFunction ImSinh { get { return Functions["IMSINH"]; } }
		public IBuiltInFunction ImSqrt { get { return Functions["IMSQRT"]; } }
		public IBuiltInFunction ImSub { get { return Functions["IMSUB"]; } }
		public IBuiltInFunction ImSum { get { return Functions["IMSUM"]; } }
		public IBuiltInFunction ImTan { get { return Functions["IMTAN"]; } }
		public IBuiltInFunction Oct2Bin { get { return Functions["OCT2BIN"]; } }
		public IBuiltInFunction Oct2Dec { get { return Functions["OCT2DEC"]; } }
		public IBuiltInFunction Oct2Hex { get { return Functions["OCT2HEX"]; } }
	}
	partial class NativeFinancialFunctions : NativeFunctionCategoryBase, IFinancialFunctions {
		public NativeFinancialFunctions(NativeWorkbookFunctions functions)
			: base(functions) {
		}
		public IBuiltInFunction AccrintM { get { return Functions["ACCRINTM"]; } }
		public IBuiltInFunction CoupDayBs { get { return Functions["COUPDAYBS"]; } }
		public IBuiltInFunction CoupNcd { get { return Functions["COUPNCD"]; } }
		public IBuiltInFunction CoupNum { get { return Functions["COUPNUM"]; } }
		public IBuiltInFunction CoupPcd { get { return Functions["COUPPCD"]; } }
		public IBuiltInFunction CumIpmt { get { return Functions["CUMIPMT"]; } }
		public IBuiltInFunction CumPrinc { get { return Functions["CUMPRINC"]; } }
		public IBuiltInFunction Db { get { return Functions["DB"]; } }
		public IBuiltInFunction Ddb { get { return Functions["DDB"]; } }
		public IBuiltInFunction Disc { get { return Functions["DISC"]; } }
		public IBuiltInFunction DollarDe { get { return Functions["DOLLARDE"]; } }
		public IBuiltInFunction DollarFr { get { return Functions["DOLLARFR"]; } }
		public IBuiltInFunction Effect { get { return Functions["EFFECT"]; } }
		public IBuiltInFunction Fv { get { return Functions["FV"]; } }
		public IBuiltInFunction FvSchedule { get { return Functions["FVSCHEDULE"]; } }
		public IBuiltInFunction Intrate { get { return Functions["INTRATE"]; } }
		public IBuiltInFunction Ipmt { get { return Functions["IPMT"]; } }
		public IBuiltInFunction Irr { get { return Functions["IRR"]; } }
		public IBuiltInFunction Ispmt { get { return Functions["ISPMT"]; } }
		public IBuiltInFunction Mirr { get { return Functions["MIRR"]; } }
		public IBuiltInFunction Nominal { get { return Functions["NOMINAL"]; } }
		public IBuiltInFunction NPer { get { return Functions["NPER"]; } }
		public IBuiltInFunction Npv { get { return Functions["NPV"]; } }
		public IBuiltInFunction PDuration { get { return Functions["PDURATION"]; } }
		public IBuiltInFunction Pmt { get { return Functions["PMT"]; } }
		public IBuiltInFunction Ppmt { get { return Functions["PPMT"]; } }
		public IBuiltInFunction PriceDisc { get { return Functions["PRICEDISC"]; } }
		public IBuiltInFunction Pv { get { return Functions["PV"]; } }
		public IBuiltInFunction Rate { get { return Functions["RATE"]; } }
		public IBuiltInFunction Received { get { return Functions["RECEIVED"]; } }
		public IBuiltInFunction Rri { get { return Functions["RRI"]; } }
		public IBuiltInFunction Sln { get { return Functions["SLN"]; } }
		public IBuiltInFunction Syd { get { return Functions["SYD"]; } }
		public IBuiltInFunction TBillEq { get { return Functions["TBILLEQ"]; } }
		public IBuiltInFunction TBillPrice { get { return Functions["TBILLPRICE"]; } }
		public IBuiltInFunction TBillYield { get { return Functions["TBILLYIELD"]; } }
		public IBuiltInFunction Vdb { get { return Functions["VDB"]; } }
		public IBuiltInFunction Xirr { get { return Functions["XIRR"]; } }
		public IBuiltInFunction Xnpv { get { return Functions["XNPV"]; } }
		public IBuiltInFunction YieldDisc { get { return Functions["YIELDDISC"]; } }
	}
	partial class NativeInformationFunctions : NativeFunctionCategoryBase, IInformationFunctions {
		public NativeInformationFunctions(NativeWorkbookFunctions functions)
			: base(functions) {
		}
		public IBuiltInFunction Cell { get { return Functions["CELL"]; } }
		public IBuiltInFunction ErrorType { get { return Functions["ERROR.TYPE"]; } }
		public IBuiltInFunction Info { get { return Functions["INFO"]; } }
		public IBuiltInFunction IsBlank { get { return Functions["ISBLANK"]; } }
		public IBuiltInFunction IsErr { get { return Functions["ISERR"]; } }
		public IBuiltInFunction IsError { get { return Functions["ISERROR"]; } }
		public IBuiltInFunction IsEven { get { return Functions["ISEVEN"]; } }
		public IBuiltInFunction IsFormula { get { return Functions["ISFORMULA"]; } }
		public IBuiltInFunction IsLogical { get { return Functions["ISLOGICAL"]; } }
		public IBuiltInFunction IsNA { get { return Functions["ISNA"]; } }
		public IBuiltInFunction IsNonText { get { return Functions["ISNONTEXT"]; } }
		public IBuiltInFunction IsNumber { get { return Functions["ISNUMBER"]; } }
		public IBuiltInFunction IsOdd { get { return Functions["ISODD"]; } }
		public IBuiltInFunction IsRef { get { return Functions["ISREF"]; } }
		public IBuiltInFunction IsText { get { return Functions["ISTEXT"]; } }
		public IBuiltInFunction N { get { return Functions["N"]; } }
		public IBuiltInFunction NA { get { return Functions["NA"]; } }
		public IBuiltInFunction Sheet { get { return Functions["SHEET"]; } }
		public IBuiltInFunction Sheets { get { return Functions["SHEETS"]; } }
		public IBuiltInFunction Type { get { return Functions["TYPE"]; } }
	}
	partial class NativeLogicalFunctions : NativeFunctionCategoryBase, ILogicalFunctions {
		public NativeLogicalFunctions(NativeWorkbookFunctions functions)
			: base(functions) {
		}
		public IBuiltInFunction And { get { return Functions["AND"]; } }
		public IBuiltInFunction False { get { return Functions["FALSE"]; } }
		public IBuiltInFunction If { get { return Functions["IF"]; } }
		public IBuiltInFunction IfError { get { return Functions["IFERROR"]; } }
		public IBuiltInFunction IfNA { get { return Functions["IFNA"]; } }
		public IBuiltInFunction Not { get { return Functions["NOT"]; } }
		public IBuiltInFunction Or { get { return Functions["OR"]; } }
		public IBuiltInFunction True { get { return Functions["TRUE"]; } }
		public IBuiltInFunction Xor { get { return Functions["XOR"]; } }
	}
	partial class NativeLookupAndReferenceFunctions : NativeFunctionCategoryBase, ILookupAndReferenceFunctions {
		public NativeLookupAndReferenceFunctions(NativeWorkbookFunctions functions)
			: base(functions) {
		}
		public IBuiltInFunction Address { get { return Functions["ADDRESS"]; } }
		public IBuiltInFunction Areas { get { return Functions["AREAS"]; } }
		public IBuiltInFunction Choose { get { return Functions["CHOOSE"]; } }
		public IBuiltInFunction Column { get { return Functions["COLUMN"]; } }
		public IBuiltInFunction Columns { get { return Functions["COLUMNS"]; } }
		public IBuiltInFunction FormulaText { get { return Functions["FORMULATEXT"]; } }
		public IBuiltInFunction GetPivotData { get { return Functions["GETPIVOTDATA"]; } }
		public IBuiltInFunction HLookup { get { return Functions["HLOOKUP"]; } }
		public IBuiltInFunction Hyperlink { get { return Functions["HYPERLINK"]; } }
		public IBuiltInFunction Index { get { return Functions["INDEX"]; } }
		public IBuiltInFunction Indirect { get { return Functions["INDIRECT"]; } }
		public IBuiltInFunction Lookup { get { return Functions["LOOKUP"]; } }
		public IBuiltInFunction Match { get { return Functions["MATCH"]; } }
		public IBuiltInFunction Offset { get { return Functions["OFFSET"]; } }
		public IBuiltInFunction Row { get { return Functions["ROW"]; } }
		public IBuiltInFunction Rows { get { return Functions["ROWS"]; } }
		public IBuiltInFunction Rtd { get { return Functions["RTD"]; } }
		public IBuiltInFunction Transpose { get { return Functions["TRANSPOSE"]; } }
		public IBuiltInFunction VLookup { get { return Functions["VLOOKUP"]; } }
	}
	partial class NativeMathAndTrigonometryFunctions : NativeFunctionCategoryBase, IMathAndTrigonometryFunctions {
		public NativeMathAndTrigonometryFunctions(NativeWorkbookFunctions functions)
			: base(functions) {
		}
		public IBuiltInFunction Abs { get { return Functions["ABS"]; } }
		public IBuiltInFunction Acos { get { return Functions["ACOS"]; } }
		public IBuiltInFunction Acosh { get { return Functions["ACOSH"]; } }
		public IBuiltInFunction Acot { get { return Functions["ACOT"]; } }
		public IBuiltInFunction Acoth { get { return Functions["ACOTH"]; } }
		public IBuiltInFunction Arabic { get { return Functions["ARABIC"]; } }
		public IBuiltInFunction Asin { get { return Functions["ASIN"]; } }
		public IBuiltInFunction Asinh { get { return Functions["ASINH"]; } }
		public IBuiltInFunction Atan { get { return Functions["ATAN"]; } }
		public IBuiltInFunction Atan2 { get { return Functions["ATAN2"]; } }
		public IBuiltInFunction Atanh { get { return Functions["ATANH"]; } }
		public IBuiltInFunction Base { get { return Functions["BASE"]; } }
		public IBuiltInFunction Ceiling { get { return Functions["CEILING"]; } }
		public IBuiltInFunction CeilingMath { get { return Functions["Ceiling.MATH"]; } }
		public IBuiltInFunction CeilingPrecise { get { return Functions["CEILING.PRECISE"]; } }
		public IBuiltInFunction Combin { get { return Functions["COMBIN"]; } }
		public IBuiltInFunction CombinA { get { return Functions["COMBINA"]; } }
		public IBuiltInFunction Cos { get { return Functions["COS"]; } }
		public IBuiltInFunction Cosh { get { return Functions["COSH"]; } }
		public IBuiltInFunction Cot { get { return Functions["COT"]; } }
		public IBuiltInFunction Coth { get { return Functions["COTH"]; } }
		public IBuiltInFunction Csc { get { return Functions["CSC"]; } }
		public IBuiltInFunction Csch { get { return Functions["CSCH"]; } }
		public IBuiltInFunction Decimal { get { return Functions["DECIMAL"]; } }
		public IBuiltInFunction Degrees { get { return Functions["DEGREES"]; } }
		public IBuiltInFunction Even { get { return Functions["EVEN"]; } }
		public IBuiltInFunction Exp { get { return Functions["EXP"]; } }
		public IBuiltInFunction Fact { get { return Functions["FACT"]; } }
		public IBuiltInFunction FactDouble { get { return Functions["FACTDOUBLE"]; } }
		public IBuiltInFunction Floor { get { return Functions["FLOOR"]; } }
		public IBuiltInFunction FloorMath { get { return Functions["Floor.MATH"]; } }
		public IBuiltInFunction FloorPrecise { get { return Functions["FLOOR.PRECISE"]; } }
		public IBuiltInFunction Gcd { get { return Functions["GCD"]; } }
		public IBuiltInFunction Int { get { return Functions["INT"]; } }
		public IBuiltInFunction IsoCeiling { get { return Functions["ISO.CEILING"]; } }
		public IBuiltInFunction Lcm { get { return Functions["LCM"]; } }
		public IBuiltInFunction Ln { get { return Functions["LN"]; } }
		public IBuiltInFunction Log { get { return Functions["LOG"]; } }
		public IBuiltInFunction Log10 { get { return Functions["LOG10"]; } }
		public IBuiltInFunction MDeterm { get { return Functions["MDETERM"]; } }
		public IBuiltInFunction MInverse { get { return Functions["MINVERSE"]; } }
		public IBuiltInFunction MMult { get { return Functions["MMULT"]; } }
		public IBuiltInFunction Mod { get { return Functions["MOD"]; } }
		public IBuiltInFunction MRound { get { return Functions["MROUND"]; } }
		public IBuiltInFunction Multinomial { get { return Functions["MULTINOMIAL"]; } }
		public IBuiltInFunction MUnit { get { return Functions["MUNIT"]; } }
		public IBuiltInFunction Odd { get { return Functions["ODD"]; } }
		public IBuiltInFunction Pi { get { return Functions["PI"]; } }
		public IBuiltInFunction Power { get { return Functions["POWER"]; } }
		public IBuiltInFunction Product { get { return Functions["PRODUCT"]; } }
		public IBuiltInFunction Quotient { get { return Functions["QUOTIENT"]; } }
		public IBuiltInFunction Radians { get { return Functions["RADIANS"]; } }
		public IBuiltInFunction Rand { get { return Functions["RAND"]; } }
		public IBuiltInFunction RandBetween { get { return Functions["RANDBETWEEN"]; } }
		public IBuiltInFunction Roman { get { return Functions["ROMAN"]; } }
		public IBuiltInFunction Round { get { return Functions["ROUND"]; } }
		public IBuiltInFunction RoundDown { get { return Functions["ROUNDDOWN"]; } }
		public IBuiltInFunction RoundUp { get { return Functions["ROUNDUP"]; } }
		public IBuiltInFunction Sec { get { return Functions["SEC"]; } }
		public IBuiltInFunction Sech { get { return Functions["SECH"]; } }
		public IBuiltInFunction SeriesSum { get { return Functions["SERIESSUM"]; } }
		public IBuiltInFunction Sign { get { return Functions["SIGN"]; } }
		public IBuiltInFunction Sin { get { return Functions["SIN"]; } }
		public IBuiltInFunction Sinh { get { return Functions["SINH"]; } }
		public IBuiltInFunction Sqrt { get { return Functions["SQRT"]; } }
		public IBuiltInFunction SqrtPi { get { return Functions["SQRTPI"]; } }
		public IBuiltInFunction Subtotal { get { return Functions["SUBTOTAL"]; } }
		public IBuiltInFunction Sum { get { return Functions["SUM"]; } }
		public IBuiltInFunction SumIf { get { return Functions["SUMIF"]; } }
		public IBuiltInFunction SumIfS { get { return Functions["SUMIFS"]; } }
		public IBuiltInFunction SumProduct { get { return Functions["SUMPRODUCT"]; } }
		public IBuiltInFunction SumSq { get { return Functions["SUMSQ"]; } }
		public IBuiltInFunction SumX2mY2 { get { return Functions["SUMX2MY2"]; } }
		public IBuiltInFunction SumX2pY2 { get { return Functions["SUMX2PY2"]; } }
		public IBuiltInFunction SumXmY2 { get { return Functions["SUMXMY2"]; } }
		public IBuiltInFunction Tan { get { return Functions["TAN"]; } }
		public IBuiltInFunction Tanh { get { return Functions["TANH"]; } }
		public IBuiltInFunction Trunc { get { return Functions["TRUNC"]; } }
	}
	partial class NativeStatisticalFunctions : NativeFunctionCategoryBase, IStatisticalFunctions {
		public NativeStatisticalFunctions(NativeWorkbookFunctions functions)
			: base(functions) {
		}
		public IBuiltInFunction Avedev { get { return Functions["AVEDEV"]; } }
		public IBuiltInFunction Average { get { return Functions["AVERAGE"]; } }
		public IBuiltInFunction AverageA { get { return Functions["AVERAGEA"]; } }
		public IBuiltInFunction AverageIf { get { return Functions["AVERAGEIF"]; } }
		public IBuiltInFunction AverageIfS { get { return Functions["AVERAGEIFS"]; } }
		public IBuiltInFunction BetaDist { get { return Functions["BETA.DIST"]; } }
		public IBuiltInFunction BetaInv { get { return Functions["BETA.INV"]; } }
		public IBuiltInFunction BinomDist { get { return Functions["BINOM.DIST"]; } }
		public IBuiltInFunction BinomDistRange { get { return Functions["BINOM.DIST.RANGE"]; } }
		public IBuiltInFunction BinomInv { get { return Functions["BINOM.INV"]; } }
		public IBuiltInFunction ChiSqDist { get { return Functions["CHISQ.DIST"]; } }
		public IBuiltInFunction ChiSqDistRt { get { return Functions["CHISQ.DIST.RT"]; } }
		public IBuiltInFunction ChiSqInv { get { return Functions["CHISQ.INV"]; } }
		public IBuiltInFunction ChiSqInvRt { get { return Functions["CHISQ.INV.RT"]; } }
		public IBuiltInFunction ChiSqTest { get { return Functions["CHISQ.TEST"]; } }
		public IBuiltInFunction ConfidenceNorm { get { return Functions["CONFIDENCE.NORM"]; } }
		public IBuiltInFunction ConfidenceT { get { return Functions["CONFIDENCE.T"]; } }
		public IBuiltInFunction Correl { get { return Functions["CORREL"]; } }
		public IBuiltInFunction Count { get { return Functions["COUNT"]; } }
		public IBuiltInFunction CountA { get { return Functions["COUNTA"]; } }
		public IBuiltInFunction CountBlank { get { return Functions["COUNTBLANK"]; } }
		public IBuiltInFunction CountIf { get { return Functions["COUNTIF"]; } }
		public IBuiltInFunction CountIfS { get { return Functions["COUNTIFS"]; } }
		public IBuiltInFunction CovarianceP { get { return Functions["COVARIANCE.P"]; } }
		public IBuiltInFunction CovarianceS { get { return Functions["COVARIANCE.S"]; } }
		public IBuiltInFunction Devsq { get { return Functions["DEVSQ"]; } }
		public IBuiltInFunction ExponDist { get { return Functions["EXPON.DIST"]; } }
		public IBuiltInFunction FDist { get { return Functions["F.DIST"]; } }
		public IBuiltInFunction FDistRt { get { return Functions["F.DIST.RT"]; } }
		public IBuiltInFunction FInv { get { return Functions["F.INV"]; } }
		public IBuiltInFunction FInvRt { get { return Functions["F.INV.RT"]; } }
		public IBuiltInFunction FTest { get { return Functions["F.TEST"]; } }
		public IBuiltInFunction Fisher { get { return Functions["FISHER"]; } }
		public IBuiltInFunction FisherInv { get { return Functions["FISHERINV"]; } }
		public IBuiltInFunction Forecast { get { return Functions["FORECAST"]; } }
		public IBuiltInFunction Frequency { get { return Functions["FREQUENCY"]; } }
		public IBuiltInFunction Gamma { get { return Functions["GAMMA"]; } }
		public IBuiltInFunction GammaDist { get { return Functions["GAMMA.DIST"]; } }
		public IBuiltInFunction GammaInv { get { return Functions["GAMMA.INV"]; } }
		public IBuiltInFunction GammaLn { get { return Functions["GAMMALN"]; } }
		public IBuiltInFunction GammaLnPrecise { get { return Functions["GAMMALN.PRECISE"]; } }
		public IBuiltInFunction Gauss { get { return Functions["GAUSS"]; } }
		public IBuiltInFunction Geomean { get { return Functions["GEOMEAN"]; } }
		public IBuiltInFunction Growth { get { return Functions["GROWTH"]; } }
		public IBuiltInFunction HarMean { get { return Functions["HARMEAN"]; } }
		public IBuiltInFunction HypGeomDist { get { return Functions["HYPGEOM.DIST"]; } }
		public IBuiltInFunction Intercept { get { return Functions["INTERCEPT"]; } }
		public IBuiltInFunction Kurt { get { return Functions["KURT"]; } }
		public IBuiltInFunction Large { get { return Functions["LARGE"]; } }
		public IBuiltInFunction Linest { get { return Functions["LINEST"]; } }
		public IBuiltInFunction Logest { get { return Functions["LOGEST"]; } }
		public IBuiltInFunction LogNormDist { get { return Functions["LOGNORM.DIST"]; } }
		public IBuiltInFunction LogNormInv { get { return Functions["LOGNORM.INV"]; } }
		public IBuiltInFunction Max { get { return Functions["MAX"]; } }
		public IBuiltInFunction MaxA { get { return Functions["MAXA"]; } }
		public IBuiltInFunction Median { get { return Functions["MEDIAN"]; } }
		public IBuiltInFunction Min { get { return Functions["MIN"]; } }
		public IBuiltInFunction MinA { get { return Functions["MINA"]; } }
		public IBuiltInFunction ModeMult { get { return Functions["MODE.MULT"]; } }
		public IBuiltInFunction ModeSngl { get { return Functions["MODE.SNGL"]; } }
		public IBuiltInFunction NegBinomDist { get { return Functions["NEGBINOM.DIST"]; } }
		public IBuiltInFunction NormDist { get { return Functions["NORM.DIST"]; } }
		public IBuiltInFunction NormInv { get { return Functions["NORM.INV"]; } }
		public IBuiltInFunction NormSDist { get { return Functions["NORM.S.DIST"]; } }
		public IBuiltInFunction NormSInv { get { return Functions["NORM.S.INV"]; } }
		public IBuiltInFunction Pearson { get { return Functions["PEARSON"]; } }
		public IBuiltInFunction PercentileExc { get { return Functions["PERCENTILE.EXC"]; } }
		public IBuiltInFunction PercentileInc { get { return Functions["PERCENTILE.INC"]; } }
		public IBuiltInFunction PercentRankExc { get { return Functions["PERCENTRANK.EXC"]; } }
		public IBuiltInFunction PercentRankInc { get { return Functions["PERCENTRANK.INC"]; } }
		public IBuiltInFunction Permut { get { return Functions["PERMUT"]; } }
		public IBuiltInFunction Permutationa { get { return Functions["PERMUTATIONA"]; } }
		public IBuiltInFunction Phi { get { return Functions["PHI"]; } }
		public IBuiltInFunction PoissonDist { get { return Functions["POISSON.DIST"]; } }
		public IBuiltInFunction Prob { get { return Functions["PROB"]; } }
		public IBuiltInFunction QuartileExc { get { return Functions["QUARTILE.EXC"]; } }
		public IBuiltInFunction QuartileInc { get { return Functions["QUARTILE.INC"]; } }
		public IBuiltInFunction RankAvg { get { return Functions["RANK.AVG"]; } }
		public IBuiltInFunction RankEq { get { return Functions["RANK.EQ"]; } }
		public IBuiltInFunction Rsq { get { return Functions["RSQ"]; } }
		public IBuiltInFunction Skew { get { return Functions["SKEW"]; } }
		public IBuiltInFunction SkewP { get { return Functions["SKEW.P"]; } }
		public IBuiltInFunction Slope { get { return Functions["SLOPE"]; } }
		public IBuiltInFunction Small { get { return Functions["SMALL"]; } }
		public IBuiltInFunction Standardize { get { return Functions["STANDARDIZE"]; } }
		public IBuiltInFunction StdevP { get { return Functions["STDEV.P"]; } }
		public IBuiltInFunction StdevS { get { return Functions["STDEV.S"]; } }
		public IBuiltInFunction Stdeva { get { return Functions["STDEVA"]; } }
		public IBuiltInFunction Stdevpa { get { return Functions["STDEVPA"]; } }
		public IBuiltInFunction Steyx { get { return Functions["STEYX"]; } }
		public IBuiltInFunction TDist { get { return Functions["T.DIST"]; } }
		public IBuiltInFunction TDist2t { get { return Functions["T.DIST.2T"]; } }
		public IBuiltInFunction TDistRt { get { return Functions["T.DIST.RT"]; } }
		public IBuiltInFunction TInv { get { return Functions["T.INV"]; } }
		public IBuiltInFunction TInv2t { get { return Functions["T.INV.2T"]; } }
		public IBuiltInFunction TTest { get { return Functions["T.TEST"]; } }
		public IBuiltInFunction Trend { get { return Functions["TREND"]; } }
		public IBuiltInFunction TrimMean { get { return Functions["TRIMMEAN"]; } }
		public IBuiltInFunction VarP { get { return Functions["VAR.P"]; } }
		public IBuiltInFunction VarS { get { return Functions["VAR.S"]; } }
		public IBuiltInFunction Vara { get { return Functions["VARA"]; } }
		public IBuiltInFunction Varpa { get { return Functions["VARPA"]; } }
		public IBuiltInFunction WeibullDist { get { return Functions["WEIBULL.DIST"]; } }
		public IBuiltInFunction ZTest { get { return Functions["Z.TEST"]; } }
	}
	partial class NativeTextFunctions : NativeFunctionCategoryBase, ITextFunctions {
		public NativeTextFunctions(NativeWorkbookFunctions functions)
			: base(functions) {
		}
		public IBuiltInFunction BahtText { get { return Functions["BAHTTEXT"]; } }
		public IBuiltInFunction Char { get { return Functions["CHAR"]; } }
		public IBuiltInFunction Clean { get { return Functions["CLEAN"]; } }
		public IBuiltInFunction Code { get { return Functions["CODE"]; } }
		public IBuiltInFunction Concatenate { get { return Functions["CONCATENATE"]; } }
		public IBuiltInFunction Dollar { get { return Functions["DOLLAR"]; } }
		public IBuiltInFunction Exact { get { return Functions["EXACT"]; } }
		public IBuiltInFunction Find { get { return Functions["FIND"]; } }
		public IBuiltInFunction Fixed { get { return Functions["FIXED"]; } }
		public IBuiltInFunction Left { get { return Functions["LEFT"]; } }
		public IBuiltInFunction Len { get { return Functions["LEN"]; } }
		public IBuiltInFunction Lower { get { return Functions["LOWER"]; } }
		public IBuiltInFunction Mid { get { return Functions["MID"]; } }
		public IBuiltInFunction NumberValue { get { return Functions["NUMBERVALUE"]; } }
		public IBuiltInFunction Proper { get { return Functions["PROPER"]; } }
		public IBuiltInFunction Replace { get { return Functions["REPLACE"]; } }
		public IBuiltInFunction Rept { get { return Functions["REPT"]; } }
		public IBuiltInFunction Right { get { return Functions["RIGHT"]; } }
		public IBuiltInFunction Search { get { return Functions["SEARCH"]; } }
		public IBuiltInFunction Substitute { get { return Functions["SUBSTITUTE"]; } }
		public IBuiltInFunction T { get { return Functions["T"]; } }
		public IBuiltInFunction Text { get { return Functions["TEXT"]; } }
		public IBuiltInFunction Trim { get { return Functions["TRIM"]; } }
		public IBuiltInFunction Unicode { get { return Functions["UNICODE"]; } }
		public IBuiltInFunction Upper { get { return Functions["UPPER"]; } }
		public IBuiltInFunction Value { get { return Functions["VALUE"]; } }
	}
	partial class NativeWebFunctions : NativeFunctionCategoryBase, IWebFunctions {
		public NativeWebFunctions(NativeWorkbookFunctions functions)
			: base(functions) {
		}
		public IBuiltInFunction EncodeUrl { get { return Functions["ENCODEURL"]; } }
	}
	#endregion
}
#endregion
