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
using System.Reflection;
using System.Resources;
using DevExpress.Utils;
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
using System.Globalization;
using System;
using System.ComponentModel;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraSpreadsheet.Localization {
	#region XtraSpreadsheetFunctionDescriptionStringId
	public enum XtraSpreadsheetFunctionDescriptionStringId {
		BetaDistCompatibility = 0x10E,
		BetaInv = 0x110,
		BinomDist = 0x111,
		ChiDist = 0x112,
		ChiInv = 0x113,
		ChiTest = 0x132,
		Confidence = 0x115,
		Covar = 0x134,
		CritBinom = 0x116,
		ExponDist = 0x118,
		FDist = 0x119,
		FInv = 0x11A,
		FTest = 0x136,
		GammaDist = 0x11E,
		GammaInv = 0x11F,
		HypGeomDist = 0x121,
		LogInvCompatibility = 0x123,
		LogNormDistCompatibility = 0x122,
		Mode = 0x14A,
		NegBinomDist = 0x124,
		NormDistCompatibility = 0x125,
		NormInvCompatibility = 0x127,
		NormSDistCompatibility = 0x126,
		NormSInvCompatibility = 0x128,
		Percentile = 0x148,
		PercentRank = 0x149,
		Poisson = 0x12C,
		Quartile = 0x147,
		Rank = 0xD8,
		StDev = 0x0C,
		StDevP = 0xC1,
		TDist = 0x12D,
		Tinv = 0x14C,
		TTest = 0x13C,
		Var = 0x2E,
		VarP = 0xC2,
		Weibull = 0x12E,
		ZTest = 0x144,
		DAverage = 0x2A,
		DCount = 0x28,
		DCountA = 0xC7,
		DGet = 0xEB,
		DMax = 0x2C,
		DMin = 0x2B,
		DProduct = 0xBD,
		DStDev = 0x2D,
		DStDevP = 0xC3,
		DSum = 0x29,
		DVar = 0x2F,
		DVarP = 0xC4,
		Date = 0x41,
		DateValue = 0x8C,
		Day = 0x43,
		Days = 0x4042,
		Days360 = 0xDC,
		EDate = 0x1C1,
		EOMonth = 0x1C2,
		Hour = 0x47,
		ISOWeekNum = 0x4043,
		Minute = 0x48,
		Month = 0x44,
		NetworkDays = 0x1D8,
		NetworkDaysIntl = 0x400F,
		Now = 0x4A,
		Second = 0x49,
		Time = 0x42,
		TimeValue = 0x8D,
		Today = 0xDD,
		WeekDay = 0x46,
		WeekNum = 0x1D1,
		WorkDay = 0x1D7,
		WorkDayIntl = 0x400E,
		Year = 0x45,
		YearFrac = 0x1C3,
		DateDif = 0x15F,
		BesselI = 0x01AC,
		BesselJ = 0x01A9,
		BesselK = 0x01AA,
		BesselY = 0x01AB,
		Bin2Dec = 0x0189,
		Bin2Oct = 0x018A,
		Bin2Hex = 0x018B,
		BitAnd = 0x402A,
		BitLShift = 0x402B,
		BitOr = 0x402C,
		BitRShift = 0x402D,
		BitXor = 0x402E,
		Complex = 0x019B,
		Convert = 0x1D4,
		Dec2Bin = 0x0183,
		Dec2Hex = 0x0184,
		Dec2Oct = 0x0185,
		Delta = 0x01A2,
		Erf = 0x01A7,
		Erfc = 0x01A8,
		ErfPrecise = 0x4011,
		ErfcPrecise = 0x4012,
		Gestep = 0x01A3,
		Hex2Bin = 0x0180,
		Hex2Dec = 0x0181,
		Hex2Oct = 0x0182,
		ImAbs = 0x18F,
		Imaginary = 0x199,
		ImArgument = 0x197,
		ImConjugate = 0x198,
		ImCos = 0x195,
		ImCosH = 0x402F,
		ImCot = 0x4030,
		ImCsc = 0x4031,
		ImCscH = 0x4032,
		ImDiv = 0x18D,
		ImExp = 0x196,
		ImLn = 0x191,
		ImLog10 = 0x193,
		ImLog2 = 0x192,
		ImPower = 0x18E,
		ImProduct = 0x19D,
		ImReal = 0x19A,
		ImSec = 0x4033,
		ImSecH = 0x4034,
		ImSin = 0x194,
		ImSinH = 0x4035,
		ImSqrt = 0x190,
		ImSub = 0x18C,
		ImSum = 0x19C,
		ImTan = 0x4036,
		Oct2Bin = 0x0186,
		Oct2Dec = 0x0188,
		Oct2Hex = 0x0187,
		CumIpmt = 0x1C0,
		CumPrinc = 0x1BF,
		DollarDe = 0x01BB,
		DollarFr = 0x01BC,
		Effect = 0x01BE,
		Db = 0xF7,
		Ddb = 0x90,
		Fv = 0x39,
		Ipmt = 0xA7,
		Irr = 0x3E,
		IsPmt = 0x15E,
		Mirr = 0x3D,
		Nominal = 0x01BD,
		NPer = 0x3A,
		Npv = 0x0B,
		PDuration = 0x403A,
		Pmt = 0x3B,
		Ppmt = 0xA8,
		Pv = 0x38,
		Rate = 0x3C,
		RRI = 0x403B,
		Sln = 0x8E,
		Syd = 0x8F,
		Vdb = 0xDE,
		Xnpv = 0x01AE,
		Cell = 0x7D,
		ErrorType = 0x105,
		Info = 0xF4,
		IsBlank = 0x81,
		IsErr = 0x7E,
		IsError = 0x03,
		IsEven = 0x1A4,
		IsFormula = 0x403F,
		IsLogical = 0xC6,
		IsNA = 0x02,
		IsNonText = 0xBE,
		IsNumber = 0x80,
		IsOdd = 0x1A5,
		IsRef = 0x69,
		IsText = 0x7F,
		N = 0x83,
		NA = 0x0A,
		Sheets = 0x4041,
		Sheet = 0x4040,
		Type = 0x56,
		And = 0x24,
		False = 0x23,
		If = 0x01,
		IfError = 0x1E0,
		IfNA = 0x403C,
		Not = 0x26,
		Or = 0x25,
		True = 0x22,
		XOr = 0x403D,
		Address = 0xDB,
		Areas = 0x4B,
		Choose = 0x64,
		Column = 0x09,
		Columns = 0x4D,
		FormulaText = 0x403E,
		GetPivotData = 0x166,
		HLookup = 0x65,
		Hyperlink = 0x167,
		Index = 0x1D,
		Indirect = 0x94,
		Lookup = 0x1C,
		Match = 0x40,
		Offset = 0x4E,
		Row = 0x08,
		Rows = 0x4C,
		Rtd = 0x17B,
		Transpose = 0x53,
		VLookup = 0x66,
		Abs = 0x18,
		ACos = 0x63,
		ACosH = 0xE9,
		ACot = 0x4015,
		ACotH = 0x4016,
		Arabic = 0x4017,
		ASin = 0x62,
		ASinH = 0xE8,
		ATan = 0x12,
		ATan2 = 0x61,
		ATanH = 0xEA,
		Base = 0x4018,
		Ceiling = 0x120,
		CeilingMath = 0x4019,
		Combin = 0x114,
		CombinA = 0x401A,
		Cos = 0x10,
		CosH = 0xE6,
		Cot = 0x401B,
		CotH = 0x401C,
		Csc = 0x401D,
		CscH = 0x401E,
		Decimal = 0x401F,
		Degrees = 0x157,
		Even = 0x117,
		Exp = 0x15,
		Fact = 0xB8,
		Floor = 0x11D,
		FloorMath = 0x4020,
		Int = 0x19,
		IsoCeiling = 0x4021,
		Ln = 0x16,
		Log = 0x6D,
		Log10 = 0x17,
		MDeterm = 0xA3,
		MInverse = 0xA4,
		MMult = 0xA5,
		Mod = 0x27,
		MRound = 0x1A6,
		MUnit = 0x4022,
		Odd = 0x12A,
		Pi = 0x13,
		Power = 0x151,
		Product = 0xB7,
		Quotient = 0x1A1,
		Radians = 0x156,
		Rand = 0x3F,
		RandBetween = 0x1D0,
		Roman = 0x162,
		Round = 0x1B,
		RoundDown = 0xD5,
		RoundUp = 0xD4,
		Sec = 0x4023,
		SecH = 0x4024,
		Sign = 0x1A,
		Sin = 0x0F,
		SinH = 0xE5,
		Sqrt = 0x14,
		SqrtPi = 0x1A0,
		Subtotal = 0x158,
		Sum = 0x04,
		SumIf = 0x159,
		SumIfs = 0x1E2,
		SumProduct = 0xE4,
		SumSq = 0x141,
		SumX2MY2 = 0x130,
		SumX2PY2 = 0x131,
		SumXMY2 = 0x12F,
		Tan = 0x11,
		TanH = 0xE7,
		Trunc = 0xC5,
		Avedev = 0x10D,
		Average = 0x05,
		AverageA = 0x169,
		AverageIf = 0x1E3,
		AverageIfs = 0x1E4,
		BetaDist = 0x4014,
		BetaDotInv = 0x4051,
		BinomDistRange = 0x4025,
		ChisqDotDist = 0x4065,
		ChisqDotDistDotRt = 0x4066,
		ChisqDotInv = 0x4067,
		ChisqDotInvDotRt = 0x4068,
		ChisqDotTest = 0x4069,
		ConfidenceNorm = 0x4010,
		Correl = 0x133,
		Count = 0x00,
		CountA = 0xA9,
		CountBlank = 0x15B,
		CountIf = 0x15A,
		CountIfs = 0x1E1,
		CovarianceP = 0x404D,
		CovarianceS = 0x404E,
		DevSq = 0x13E,
		FDotDist = 0x4054,
		FDotDistDotRt = 0x4057,
		FDotInv = 0x4053,
		FDotinvDotRt = 0x4063, 
		FDotTest = 0x4056,
		Fisher = 0x11B,
		FisherInv = 0x11C,
		Forecast = 0x135,
		Frequency = 0xFC,
		Gamma = 0x4026,
		GammaDotDist = 0x4058,
		GammaDotInv = 0x4064,
		GammaLn = 0x10F,
		GammaLnPrecise = 0x4013,
		Geomean = 0x13F,
		Growth = 0x34,
		HarMean = 0x140,
		Intercept = 0x137,
		Kurt = 0x142,
		Large = 0x145,
		Linest = 0x31,
		Logest = 0x33,
		LogNormDist = 0x4009,
		LogNormInv = 0x400B,
		Max = 0x07,
		MaxA = 0x16A,
		Median = 0xE3,
		Min = 0x06,
		MinA = 0x16B,
		ModeMult = 0x4001,
		ModeSngl = 0x4002,
		NormDist = 0x400D,
		NormInv = 0x400C,
		NormSDist = 0x4008,
		NormSInv = 0x400A,
		Pearson = 0x138,
		PercentileExc = 0x4049,
		PercentileInc = 0x404A,
		PercentRankExc = 0x4047,
		PercentRankInc = 0x4048,
		Permutationa = 0x4027,
		Phi = 0x4028,
		Permut = 0x12B,
		Prob = 0x13D,
		Rsq = 0x139,
		QuartileExc = 0x404B,
		QuartileInc = 0x404C,
		RankDotAvg = 0x404F,
		RankDotEq = 0x4050,
		Skew = 0x143,
		SkewP = 0x4029,
		Slope = 0x13B,
		Small = 0x146,
		Standardize = 0x129,
		StDevDotP = 0x4006,
		StDevDotS = 0x4005,
		StDevA = 0x16E,
		StDevPA = 0x16C,
		TDotDist = 0x4055,
		TDotDistDot2t = 0x4061,
		TDotDistDotTt = 0x4060, 
		TDotInvDot2t = 0x4062,
		TDotInv = 0x4052,
		TDotTest = 0x4059,
		StEYX = 0x13A,
		Trend = 0x32,
		VarDotP = 0x4003,
		VarDotS = 0x4004,
		VarA = 0x16F,
		VarPA = 0x16D,
		Asc = 0xD6,
		BahtText = 0x170,
		Char = 0x6F,
		Clean = 0xA2,
		Code = 0x79,
		Concatenate = 0x150,
		Dollar = 0x0D,
		Exact = 0x75,
		Find = 0x7C,
		FindB = 0xCD,
		Fixed = 0x0E,
		Left = 0x73,
		LeftB = 0xD0,
		Len = 0x20,
		LenB = 0xD3,
		Lower = 0x70,
		Mid = 0x1F,
		MidB = 0xD2,
		NumberValue = 0x4037,
		Phonetic = 0x168,
		Proper = 0x72,
		Replace = 0x77,
		ReplaceB = 0xCF,
		Rept = 0x1E,
		Right = 0x74,
		RightB = 0xD1,
		Search = 0x52,
		SearchB = 0xCE,
		Substitute = 0x78,
		T = 0x82,
		Text = 0x30,
		Trim = 0x76,
		Unicode = 0x4039,
		Upper = 0x71,
		Value = 0x21,
		EncodeURL = 0x4044,
		Call = 0x96,
		RegisterId = 0x10B,
		Yield = 0x01BA,
		OddLYield = 0x01CD,
		OddFPrice = 0x01CE,
		OddLPrice = 0x01CC,
		YieldDisc = 0x01B5,
		YieldMat = 0x01B0,
		Xirr = 0x01AD,
		Trimmean = 0x014B,
		TbillEq = 0x01B6,
		TbillPrice = 0x01B7,
		TbillYield = 0x01B8,
		Seriessum = 0x019E,
		Price = 0x01B9,
		PriceDisc = 0x01B4,
		PriceMat = 0x01AF,
		ZDotTest = 0x406A,
		WeibullDotDist = 0x406B,
		NegbinomDotDist = 0x406C,
		HypgeomDotDist = 0x406D,
		Aggregate = 0x406E,
		BinomDotDist = 0x406F,
		BinomDotInv = 0x4070,
		CeilingDotPrecise = 0x4071,
		ConfidenceDotT = 0x4072,
		FloorDotPrecise = 0x4073,
		ExponDotDist = 0x4074,
		PoissonDotDist = 0x4075,
		FunctionFilterXML = 0x4045,
		FunctionWebService = 0x4046,
		Received = 0x01B2,
		OddFYield = 0x01CF,
		MDuration = 0x01CB,
		Multinomial = 0x01DA,
		Intrate = 0x01B1,
		Lcm = 0x01DB,
		FvSchedule = 0x01DC,
		Gcd = 0x01D9,
		Accrint = 0x01D5,
		Accrintm = 0x01D6,
		Amordegrc = 0x01D2,
		Amorlinc = 0x01D3,
		CoupDaybs = 0x01C4,
		CoupDays = 0x01C5,
		CoupDaysnc = 0x01C6,
		Coupncd = 0x01C7,
		CoupNum = 0x01C8,
		Couppcd = 0x01C9,
		CubeKpiMember = 0x01DD,
		CubeMember = 0x017D,
		CubeMemberProperty = 0x017E,
		CubeRankedMember = 0x017F,
		CubeSet = 0x01DE,
		CubeSetCount = 0x01DF,
		CubeValue = 0x017C,
		Disc = 0x01B3,
		Duration = 0x01CA,
		FactDouble = 0x019F,
		Field = 0x4100,
		Range = 0x4101,
		FieldPicture = 0x4102,
		Parameter = 0x4103,
	}
	#endregion
	#region XtraSpreadsheetFunctionDescriptionLocalizer
	public class XtraSpreadsheetFunctionDescriptionLocalizer : XtraLocalizer<XtraSpreadsheetFunctionDescriptionStringId> {
		static XtraSpreadsheetFunctionDescriptionLocalizer() {
			SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<XtraSpreadsheetFunctionDescriptionStringId>(CreateDefaultLocalizer()));
		}
		#region PopulateStringTable
		protected override void PopulateStringTable() {
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Count, "Counts the number of cells in a range that contain numbers.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.If, "Checks whether a condition is met, and returns one value if TRUE, and another value if FALSE.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.IsNA, "Checks whether a value is #N/A, and returns TRUE or FALSE.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.IsError, "Checks whether a value is an error (#N/A, #VALUE!, #REF!, #DIV/0!, #NUM!, #Description?, or #NULL!), and returns TRUE or FALSE.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Sum, "Adds all the numbers in a range of cells.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Average, "Returns the average (arithmetic mean) of its arguments, which can be numbers or Descriptions, arrays, or references that contain numbers.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Min, "Returns the smallest number in a set of values. Ignores logical values and text.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Max, "Returns the largest value in a set of values. Ignores logical values and text.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Row, "Returns the row number of a reference.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Column, "Returns the column number of a reference.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.NA, "Returns the error value #N/A (value not available).");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Npv, "Returns the net present value of an investment based on a discount rate and a series of future payments (negative values) and income (positive values).");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.StDev, "Estimates standard deviation based on a sample (ignores logical values and text in the sample).");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Dollar, "Converts a number to text, using currency format.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Fixed, "Rounds a number to the specified number of decimals and returns the result as text with or without commas.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Sin, "Returns the sine of an angle.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Cos, "Returns the cosine of an angle.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Tan, "Returns the tangent of an angle.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ATan, "Returns the arctangent of a number in radians, in the range -Pi/2 to Pi/2.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Pi, "Returns the value of Pi, 3.14159265358979, accurate to 15 digits.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Sqrt, "Returns the square root of a number.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Exp, "Returns e raised to the power of a given number.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Ln, "Returns the natural logarithm of a number.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Log10, "Returns the base-10 logarithm of a number.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Abs, "Returns the absolute value of a number, a number without its sign.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Int, "Rounds a number down to the nearest integer.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Sign, "Returns the sign of a number: 1 if the number is positive, zero if the number is zero, or -1 if the number is negative.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Round, "Rounds a number to a specified number of digits.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Lookup, "Looks up a value either from a one-row or one-column range or from an array. Provided for backward compatibility.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Index, "Returns a value or reference of the cell at the intersection of a particular row and column, in a given range.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Rept, "Repeats text a given number of times. Use REPT to fill a cell with a number of instances of a text string.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Mid, "Returns the characters from the middle of a text string, given a starting position and length.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Len, "Returns the number of characters in a text string.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Value, "Converts a text string that represents a number to a number.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.True, "Returns the logical value TRUE.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.False, "Returns the logical value FALSE.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.And, "Checks whether all arguments are TRUE, and returns TRUE if all arguments are TRUE.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Or, "Checks whether any of the arguments are TRUE, and returns TRUE or FALSE. Returns FALSE only if all arguments are FALSE.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Not, "Changes FALSE to TRUE, or TRUE to FALSE.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Mod, "Returns the remainder after a number is divided by a divisor.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.DCount, "Counts the cells containing numbers in the field (column) of records in the database that match the conditions you specify.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.DSum, "Adds the numbers in the field (column) of records in the database that match the conditions you specify.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.DAverage, "Averages the values in a column in a list or database that match conditions you specify.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.DMin, "Returns the smallest number in the field (column) of records in the database that match the conditions you specify.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.DMax, "Returns the largest number in the field (column) of records in the database that match the conditions you specify.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.DStDev, "Estimates the standard deviation based on a sample from selected database entries.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Var, "Estimates variance based on a sample (ignores logical values and text in the sample).");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.DVar, "Estimates variance based on a sample from selected database entries.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Text, "Converts a value to text in a specific number format.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Linest, "Returns statistics that describe a linear trend matching known data points, by fitting a straight line using the least squares method.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Trend, "Returns numbers in a linear trend matching known data points, using the least squares method.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Logest, "Returns statistics that describe an exponential curve matching known data points.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Growth, "Returns numbers in an exponential growth trend matching known data points.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Pv, "Returns the present value of an investment: the total amount that a series of future payments is worth now.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Fv, "Returns the future value of an investment based on periodic, constant payments and a constant interest rate.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.NPer, "Returns the number of periods for an investment based on periodic, constant payments and a constant interest rate.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Pmt, "Calculates the payment for a loan based on constant payments and a constant interest rate.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Rate, "Returns the interest rate per period of a loan or an investment. For example, use 6%/4 for quarterly payments at 6% APR.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Mirr, "Returns the internal rate of return for a series of periodic cash flows, considering both cost of investment and interest on reinvestment of cash.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Irr, "Returns the internal rate of return for a series of cash flows.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Rand, "Returns a random number greater than or equal to 0 and less than 1, evenly distributed (changes on recalculation).");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Match, "Returns the relative position of an item in an array that matches a specified value in a specified order.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Date, "Returns the number that represents the date in Spreadsheet date-time code.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Time, "Converts hours, minutes, and seconds given as numbers to an Spreadsheet serial number, formatted with a time format.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Day, "Returns the day of the month, a number from 1 to 31.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Month, "Returns the month, a number from 1 (January) to 12 (December).");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Year, "Returns the year of a date, an integer in the range 1900 - 9999.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.WeekDay, "Returns a number from 1 to 7 identifying the day of the week of a date.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Hour, "Returns the hour as a number from 0 (12:00 A.M.) to 23 (11:00 P.M.).");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Minute, "Returns the minute, a number from 0 to 59.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Second, "Returns the second, a number from 0 to 59.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Now, "Returns the current date and time formatted as a date and time.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Areas, "Returns the number of areas in a reference. An area is a range of contiguous cells or a single cell.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Rows, "Returns the number of rows in a reference or array.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Columns, "Returns the number of columns in an array or reference.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Offset, "Returns a reference to a range that is a given number of rows and columns from a given reference.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Search, "Returns the number of the character at which a specific character or text string is first found, reading left to right (not case-sensitive).");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Transpose, "Converts a vertical range of cells to a horizontal range, or vice versa.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Type, "Returns an integer representing the data type of a value: number = 1; text = 2; logical value = 4; error value = 16; array = 64.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Call, "");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ATan2, "Returns the arctangent of the specified x- and y- coordinates, in radians between -Pi and Pi, excluding -Pi.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ASin, "Returns the arcsine of a number in radians, in the range -Pi/2 to Pi/2.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ACos, "Returns the arccosine of a number, in radians in the range 0 to Pi. The arccosine is the angle whose cosine is Number.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Choose, "Chooses a value or action to perform from a list of values, based on an index number.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.HLookup, "Looks for a value in the top row of a table or array of values and returns the value in the same column from a row you specify.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.VLookup, "Looks for a value in the leftmost column of a table, and then returns a value in the same row from a column you specify. By default, the table must be sorted in an ascending order.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.IsRef, "Checks whether a value is a reference, and returns TRUE or FALSE.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Log, "Returns the logarithm of a number to the base you specify.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Char, "Returns the character specified by the code number from the character set for your computer.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Lower, "Converts all letters in a text string to lowercase.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Upper, "Converts a text string to all uppercase letters.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Proper, "Converts a text string to proper case; the first letter in each word in uppercase, and all other letters to lowercase.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Left, "Returns the specified number of characters from the start of a text string.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Right, "Returns the specified number of characters from the end of a text string.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Exact, "Checks whether two text strings are exactly the same, and returns TRUE or FALSE. EXACT is case-sensitive.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Trim, "Removes all spaces from a text string except for single spaces between words.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Replace, "Replaces part of a text string with a different text string.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Substitute, "Replaces existing text with new text in a text string.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Code, "Returns a numeric code for the first character in a text string, in the character set used by your computer.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Find, "Returns the starting position of one text string within another text string. FIND is case-sensitive.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Cell, "Returns information about the formatting, location, or contents of the first cell, according to the sheet's reading order, in a reference.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.IsErr, "Checks whether a value is an error (#VALUE!, #REF!, #DIV/0!, #NUM!, #Description?, or #NULL!) excluding #N/A, and returns TRUE or FALSE.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.IsText, "Checks whether a value is text, and returns TRUE or FALSE.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.IsNumber, "Checks whether a value is a number, and returns TRUE or FALSE.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.IsBlank, "Checks whether a reference is to an empty cell, and returns TRUE or FALSE.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.T, "Checks whether a value is text, and returns the text if it is, or returns double quotes (empty text) if it is not.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.N, "Converts non-number value to a number, dates to serial numbers, TRUE to 1, anything else to 0 (zero).");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.DateValue, "Converts a date in the form of text to a number that represents the date in Spreadsheet date-time code.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.TimeValue, "Converts a text time to an Spreadsheet serial number for a time, a number from 0 (12:00:00 AM) to 0.999988426 (11:59:59 PM). Format the number with a time format after entering the formula.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Sln, "Returns the straight-line depreciation of an asset for one period.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Syd, "Returns the sum-of-years' digits depreciation of an asset for a specified period.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Ddb, "Returns the depreciation of an asset for a specified period using the double-declining balance method or some other method you specify.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Indirect, "Returns the reference specified by a text string.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Call, "Calls a procedure in a dynamic link library or code resource.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Clean, "Removes all nonprintable characters from text.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.MDeterm, "Returns the matrix determinant of an array.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.MInverse, "Returns the inverse matrix for the matrix stored in an array.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.MMult, "Returns the matrix product of two arrays, an array with the same number of rows as array1 and columns as array2.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Ipmt, "Returns the interest payment for a given period for an investment, based on periodic, constant payments and a constant interest rate.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Ppmt, "Returns the payment on the principal for a given investment based on periodic, constant payments and a constant interest rate.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.CountA, "Counts the number of cells in a range that are not empty.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Product, "Multiplies all the numbers given as arguments.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Fact, "Returns the factorial of a number, equal to 1*2*3*...* Number.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.DProduct, "Multiplies the values in the field (column) of records in the database that match the conditions you specify.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.IsNonText, "Checks whether a value is not text (blank cells are not text), and returns TRUE or FALSE.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.StDevP, "Calculates standard deviation based on the entire population given as arguments (ignores logical values and text).");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.VarP, "Calculates variance based on the entire population (ignores logical values and text in the population).");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.DStDevP, "Calculates the standard deviation based on the entire population of selected database entries.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.DVarP, "Calculates variance based on the entire population of selected database entries.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Trunc, "Truncates a number to an integer by removing the decimal, or fractional, part of the number.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.IsLogical, "Checks whether a value is a logical value (TRUE or FALSE), and returns TRUE or FALSE.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.DCountA, "Counts nonblank cells in the field (column) of records in the database that match the conditions you specify.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.FindB, "Finds the starting position of one text string within another text string. FINDB is case-sensitive. Use with double-byte character sets (DBCS).");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.SearchB, " Returns the number of the character at which a specific character or text string is first found, reading left to right (not case-sensitive). Use with double-byte character sets (DBCS).");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ReplaceB, "Replaces part of a text string with a different text string. Use with double-byte character sets (DBCS).");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.LeftB, "Returns the specified number of characters from the start of a text string. Use with double-byte character sets (DBCS).");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.RightB, "Returns the specified number of characters from the end of a text string. Use with double-byte character sets (DBCS).");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.MidB, "Returns characters from the middle of a text string, given a starting position and length. Use with double-byte character sets (DBCS).");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.LenB, "Returns the number of characters in a text string. Use with double-byte character sets (DBCS).");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.RoundUp, "Rounds a number up, away from zero.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.RoundDown, "Rounds a number down, toward zero.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Asc, "Changes full-width (double-byte) characters to half-width (single-byte) characters. Use with double-byte character sets (DBCS).");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Rank, "Returns the rank of a number in a list of numbers: its size relative to other values in the list.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Address, "Creates a cell reference as text, given specified row and column numbers.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Days360, "Returns the number of days between two dates based on a 360-day year (twelve 30-day months).");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Today, "Returns the current date formatted as a date.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Vdb, "Returns the depreciation of an asset for any period you specify, including partial periods, using the double-declining balance method or some other method you specify.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Median, "Returns the median, or the number in the middle of the set of given numbers.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.SumProduct, "Returns the sum of the products of corresponding ranges or arrays.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.SinH, "Returns the hyperbolic sine of a number.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.CosH, "Returns the hyperbolic cosine of a number.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.TanH, "Returns the hyperbolic tangent of a number.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ASinH, "Returns the inverse hyperbolic sine of a number.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ACosH, "Returns the inverse hyperbolic cosine of a number.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ATanH, "Returns the inverse hyperbolic tangent of a number.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.DGet, "Extracts from a database a single record that matches the conditions you specify.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Info, "Returns information about the current operating environment.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Db, "Returns the depreciation of an asset for a specified period using the fixed-declining balance method.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Frequency, "Calculates how often values occur within a range of values and then returns a vertical array of numbers having one more element than Bins_array.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ErrorType, "Returns a number matching an error value.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.RegisterId, "Returns the register ID of the specified dynamic link library (DLL) or code resource that has been previously registered.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Avedev, "Returns the average of the absolute deviations of data points from their mean. Arguments can be numbers or Descriptions, arrays, or references that contain numbers.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.BetaDistCompatibility, "Returns the cumulative beta probability density function.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.GammaLn, "Returns the natural logarithm of the gamma function.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.BetaInv, "Returns the inverse of the cumulative beta probability density function (BETADIST).");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.BinomDist, "Returns the individual term binomial distribution probability.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ChiDist, "Returns the right-tailed probability of the chi-squared distribution.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ChiInv, "Returns the inverse of the right-tailed probability of the chi-squared distribution.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Combin, "Returns the number of combinations for a given number of items.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Confidence, "Returns the confidence interval for a population mean, using a normal distribution.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.CritBinom, "Returns the smallest value for which the cumulative binomial distribution is greater than or equal to a criterion value.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Even, "Rounds a positive number up and negative number down to the nearest even integer.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ExponDist, "Returns the exponential distribution.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.FDist, "Returns the (right-tailed) F probability distribution (degree of diversity) for two data sets.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.FInv, "Returns the inverse of the (right-tailed) F probability distribution: if p = FDIST(x,...), then FINV(p,...) = x.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Fisher, "Returns the Fisher transformation.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.FisherInv, "Returns the inverse of the Fisher transformation: if y = FISHER(x), then FISHERINV(y) = x.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Floor, "Rounds a number down to the nearest multiple of significance.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.GammaDist, "Returns the gamma distribution.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.GammaInv, "Returns the inverse of the gamma cumulative distribution: if p = GAMMADIST(x,...), then GAMMAINV(p,...) = x.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Ceiling, "Rounds a number up, to the nearest multiple of significance.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.HypGeomDist, "Returns the hypergeometric distribution.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.LogNormDistCompatibility, "Returns the cumulative lognormal distribution of x, where ln(x) is normally distributed with parameters Mean and Standard_dev.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.LogInvCompatibility, "Returns the inverse of the lognormal cumulative distribution function of x, where ln(x) is normally distributed with parameters Mean and Standard_dev.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.NegBinomDist, "Returns the negative binomial distribution, the probability that there will be Number_f failures before the Number_s-th success, with Probability_s probability of a success.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.NormDistCompatibility, "Returns the normal cumulative distribution for the specified mean and standard deviation.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.NormSDistCompatibility, "Returns the standard normal cumulative distribution (has a mean of zero and a standard deviation of one).");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.NormInvCompatibility, "Returns the inverse of the normal cumulative distribution for the specified mean and standard deviation.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.NormSInvCompatibility, "Returns the inverse of the standard normal cumulative distribution (has a mean of zero and a standard deviation of one).");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Standardize, "Returns a normalized value from a distribution characterized by a mean and standard deviation.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Odd, "Rounds a positive number up and negative number down to the nearest odd integer.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Permut, "Returns the number of permutations for a given number of objects that can be selected from the total objects.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Poisson, "Returns the Poisson distribution.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.TDist, "Returns the Student's t-distribution.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Weibull, "Returns the Weibull distribution.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.SumXMY2, "Sums the squares of the differences in two corresponding ranges or arrays.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.SumX2MY2, "Sums the differences between the squares of two corresponding ranges or arrays.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.SumX2PY2, "Returns the sum total of the sums of squares of numbers in two corresponding ranges or arrays.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ChiTest, "Returns the test for independence: the value from the chi-squared distribution for the statistic and the appropriate degrees of freedom.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Correl, "Returns the correlation coefficient between two data sets.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Covar, "Returns covariance, the average of the products of deviations for each data point pair in two data sets.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Forecast, "Calculates, or predicts, a future value along a linear trend by using existing values.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.FTest, "Returns the result of an F-test, the two-tailed probability that the variances in Array1 and Array2 are not significantly different.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Intercept, "Calculates the point at which a line will intersect the y-axis by using a best-fit regression line plotted through the known x-values and y-values.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Pearson, "Returns the Pearson product moment correlation coefficient, r.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Rsq, "Returns the square of the Pearson product moment correlation coefficient through the given data points.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.StEYX, "Returns the standard error of the predicted y-value for each x in a regression.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Slope, "Returns the slope of the linear regression line through the given data points.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.TTest, "Returns the probability associated with a Student's t-Test.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Prob, "Returns the probability that values in a range are between two limits or equal to a lower limit.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.DevSq, "Returns the sum of squares of deviations of data points from their sample mean.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Geomean, "Returns the geometric mean of an array or range of positive numeric data.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.HarMean, "Returns the harmonic mean of a data set of positive numbers: the reciprocal of the arithmetic mean of reciprocals.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.SumSq, "Returns the sum of the squares of the arguments. The arguments can be numbers, arrays, Descriptions, or references to cells that contain numbers.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Kurt, "Returns the kurtosis of a data set.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Skew, "Returns the skewness of a distribution: a characterization of the degree of asymmetry of a distribution around its mean.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ZTest, "Returns the one-tailed P-value of a z-test.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Large, "Returns the k-th largest value in a data set. For example, the fifth largest number.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Small, "Returns the k-th smallest value in a data set. For example, the fifth smallest number.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Quartile, "Returns the quartile of a data set.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Percentile, "Returns the k-th percentile of values in a range.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.PercentRank, "Returns the rank of a value in a data set as a percentage of the data set.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Mode, "Returns the most frequently occurring, or repetitive, value in an array or range of data.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Trimmean, "Returns the mean of the interior portion of a set of data values.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Tinv, "Returns the two-tailed inverse of the Student's t-distribution.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Concatenate, "Joins several text strings into one text string.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Power, "Returns the result of a number raised to a power.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Radians, "Converts degrees to radians.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Degrees, "Converts radians to degrees.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Subtotal, "Returns a subtotal in a list or database.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.SumIf, "Adds the cells specified by a given condition or criteria.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.CountIf, "Counts the number of cells within a range that meet the given condition.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.CountBlank, "Counts the number of empty cells in a specified range of cells.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.IsPmt, "Returns the interest paid during a specific period of an investment.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.DateDif, "");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Roman, "Converts an Arabic numeral to Roman, as text.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.GetPivotData, "Extracts data stored in a PivotTable.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Hyperlink, "Creates a shortcut or jump that opens a document stored on your hard drive, a network server, or on the Internet.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Phonetic, "Get phonetic string.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.AverageA, "Returns the average (arithmetic mean) of its arguments, evaluating text and FALSE in arguments as 0; TRUE evaluates as 1. Arguments can be numbers, Descriptions, arrays, or references.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.MaxA, "Returns the largest value in a set of values. Does not ignore logical values and text.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.MinA, "Returns the smallest value in a set of values. Does not ignore logical values and text.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.StDevPA, "Calculates standard deviation based on an entire population, including logical values and text. Text and the logical value FALSE have the value 0; the logical value TRUE has the value 1.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.VarPA, "Calculates variance based on the entire population, including logical values and text. Text and the logical value FALSE have the value 0; the logical value TRUE has the value 1.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.StDevA, "Estimates standard deviation based on a sample, including logical values and text. Text and the logical value FALSE have the value 0; the logical value TRUE has the value 1.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.VarA, "Estimates variance based on a sample, including logical values and text. Text and the logical value FALSE have the value 0; the logical value TRUE has the value 1.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.BahtText, "Converts a number to text (baht).");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Rtd, "Retrieves real-time data from a program that supports COM automation.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.CubeValue, "Returns an aggregated value from the cube.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.CubeMember, "Returns a member or tuple from the cube.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.CubeMemberProperty, "Returns the value of a member property from the cube.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.CubeRankedMember, "Returns the nth, or ranked, member in a set.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Hex2Bin, "Converts a Hexadecimal number to binary.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Hex2Dec, "Converts a hexadecimal number to decimal.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Hex2Oct, "Converts a hexadecimal number to octal.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Dec2Bin, "Converts a decimal number to binary.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Dec2Hex, "Converts a decimal number to hexadecimal.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Dec2Oct, "Converts a decimal number to octal.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Oct2Bin, "Converts an octal number to binary.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Oct2Hex, "Converts an octal number to hexadecimal.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Oct2Dec, "Converts an octal number to decimal.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Bin2Dec, "Converts a binary number to decimal.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Bin2Oct, "Converts a binary number to octal.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Bin2Hex, "Converts a binary number to hexadecimal.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ImSub, "Returns the difference of two complex numbers.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ImDiv, "Returns the quotient of two complex numbers.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ImPower, "Returns a complex number raised to an integer power.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ImAbs, "Returns the absolute value (modulus) of a complex number.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ImSqrt, "Returns the square root of a complex number.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ImLn, "Returns the natural logarithm of a complex number.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ImLog2, "Returns the base-2 logarithm of a complex number.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ImLog10, "Returns the base-10 logarithm of a complex number.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ImSin, "Returns the sine of a complex number.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ImCos, "Returns the cosine of a complex number.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ImExp, "Returns the exponential of a complex number.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ImArgument, "Returns the argument q, an angle expressed in radians.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ImConjugate, "Returns the complex conjugate of a complex number.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Imaginary, "Returns the imaginary coefficient of a complex number.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ImReal, "Returns the real coefficient of a complex number.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Complex, "Converts real and imaginary coefficients into a complex number.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ImSum, "Returns the sum of complex numbers.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ImProduct, "Returns the product of 1 to 255 complex numbers.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Seriessum, "Returns the sum of a power series based on the formula.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.FactDouble, "Returns the double factorial of a number.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.SqrtPi, "Returns the square root of (number * Pi).");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Quotient, "Returns the integer portion of a division.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Delta, "Tests whether two numbers are equal.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Gestep, "Tests whether a number is greater than a threshold value.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.IsEven, "Returns TRUE if the number is even.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.IsOdd, "Returns TRUE if the number is odd.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.MRound, "Returns a number rounded to the desired multiple.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Erf, "Returns the error function.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Erfc, "Returns the complementary error function.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.BesselJ, "Returns the Bessel function Jn(x).");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.BesselK, "Returns the modified Bessel function Kn(x).");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.BesselY, "Returns the Bessel function Yn(x).");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.BesselI, "Returns the modified Bessel function In(x).");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Xirr, "Returns the internal rate of return for a schedule of cash flows.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Xnpv, "Returns the net present value for a schedule of cash flows.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.PriceMat, "Returns the price per $100 face value of a security that pays interest at maturity.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.YieldMat, "Returns the annual yield of a security that pays interest at maturity.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Intrate, "Returns the interest rate for a fully invested security.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Received, "Returns the amount received at maturity for a fully invested security.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Disc, "Returns the discount rate for a security.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.PriceDisc, "Returns the price per $100 face value of a discounted security.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.YieldDisc, "Returns the annual yield for a discounted security. For example, a treasury bill.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.TbillEq, "Returns the bond-equivalent yield for a treasury bill.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.TbillPrice, "Returns the price per $100 face value for a treasury bill.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.TbillYield, "Returns the yield for a treasury bill.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Price, "Returns the price per $100 face value of a security that pays periodic interest.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Yield, "Returns the yield on a security that pays periodic interest.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.DollarDe, "Converts a dollar price, expressed as a fraction, into a dollar price, expressed as a decimal number.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.DollarFr, "Converts a dollar price, expressed as a decimal number, into a dollar price, expressed as a fraction.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Nominal, "Returns the annual nominal interest rate.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Effect, "Returns the effective annual interest rate.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.CumPrinc, "Returns the cumulative principal paid on a loan between two periods.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.CumIpmt, "Returns the cumulative interest paid between two periods.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.EDate, "Returns the serial number of the date that is the indicated number of months before or after the start date.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.EOMonth, "Returns the serial number of the last day of the month before or after a specified number of months.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.YearFrac, "Returns the year fraction representing the number of whole days between start_date and end_date.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.CoupDaybs, "Returns the number of days from the beginning of the coupon period to the settlement date.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.CoupDays, "Returns the number of days in the coupon period that contains the settlement date.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.CoupDaysnc, "Returns the number of days from the settlement date to the next coupon date.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Coupncd, "Returns the next coupon date after the settlement date.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.CoupNum, "Returns the number of coupons payable between the settlement date and maturity date.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Couppcd, "Returns the previous coupon date before the settlement date.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Duration, "Returns the annual duration of a security with periodic interest payments.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.MDuration, "Returns the Macauley modified duration for a security with an assumed par value of $100.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.OddLPrice, "Returns the price per $100 face value of a security with an odd last period.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.OddLYield, "Returns the yield of a security with an odd last period.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.OddFPrice, "Returns the price per $100 face value of a security with an odd first period.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.OddFYield, "Returns the yield of a security with an odd first period.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.RandBetween, "Returns a random number between the numbers you specify.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.WeekNum, "Returns the week number in the year.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Amordegrc, "Returns the prorated linear depreciation of an asset for each accounting period.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Amorlinc, "Returns the prorated linear depreciation of an asset for each accounting period.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Convert, "Converts a number from one measurement system to another.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Accrint, "Returns the accrued interest for a security that pays periodic interest.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Accrintm, "Returns the accrued interest for a security that pays interest at maturity.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.WorkDay, "Returns the serial number of the date before or after a specified number of workdays.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.NetworkDays, "Returns the number of whole workdays between two dates.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Gcd, "Returns the greatest common divisor.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Multinomial, "Returns the multinomial of a set of numbers.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Lcm, "Returns the least common multiple.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.FvSchedule, "Returns the future value of an initial principal after applying a series of compound interest rates.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.CubeKpiMember, "Returns a key performance indicator (KPI) property and displays the KPI Description in the cell.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.CubeSet, "Defines a calculated set of members or tuples by sending a set expression to the cube on the server, which creates the set, and then returns that set to Spreadsheet.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.CubeSetCount, "Returns the number of items in a set.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.IfError, "Returns value_if_error if expression is an error and the value of the expression itself otherwise.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.CountIfs, "Counts the number of cells specified by a given set of conditions or criteria.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.SumIfs, "Adds the cells specified by a given set of conditions or criteria.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.AverageIf, "Finds average(arithmetic mean) for the cells specified by a given condition or criteria.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.AverageIfs, "Finds average(arithmetic mean) for the cells specified by a given set of conditions or criteria.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Aggregate, "Returns an aggregate in a list or database.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.BinomDotDist, "Returns the individual term binomial distribution probability.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.BinomDotInv, "Returns the smallest value for which the cumulative binomial distribution is greater than or equal to a criterion value.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ConfidenceNorm, "Returns the confidence interval for a population mean, using a normal distribution.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ConfidenceDotT, "Returns the confidence interval for a population mean, using a Student's T distribution.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ChisqDotTest, "Returns the test for independence: the value from the chi-squared distribution for the statistic and the appropriate degrees of freedom.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.FDotTest, "Returns the result of an F-test, the two-tailed probability that the variances in Array1 and Array2 are not significantly different.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.CovarianceP, "Returns population covariance, the average of the products of deviations for each data point pair in two data sets.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.CovarianceS, "Returns sample covariance, the average of the products of deviations for each data point pair in two data sets.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ExponDotDist, "Returns the exponential distribution.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.GammaDotDist, "Returns the gamma distribution.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.GammaDotInv, "Returns the inverse of the gamma cumulative distribution: if p = GAMMA.DIST(x,...), then GAMMA.INV(p,...) = x.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ModeMult, "Returns a vertical array of the most frequently occurring, or repetitive, values in an array or range of data.  For a horizontal array, use =TRANSPOSE(MODE.MULT(number1,number2,...)).");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ModeSngl, "Returns the most frequently occurring, or repetitive, value in an array or range of data.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.NormDist, "Returns the normal distribution for the specified mean and standard deviation.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.NormInv, "Returns the inverse of the normal cumulative distribution for the specified mean and standard deviation.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.PercentileExc, "Returns the k-th percentile of values in a range, where k is in the range 0..1, exclusive.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.PercentileInc, "Returns the k-th percentile of values in a range, where k is in the range 0..1, inclusive.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.PercentRankExc, "Returns the rank of a value in a data set as a percentage of the data set as a percentage (0..1, exclusive) of the data set.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.PercentRankInc, "Returns the rank of a value in a data set as a percentage of the data set as a percentage (0..1, inclusive) of the data set.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.PoissonDotDist, "Returns the Poisson distribution.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.QuartileExc, "Returns the quartile of a data set, based on percentile values from 0..1, exclusive.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.QuartileInc, "Returns the quartile of a data set, based on percentile values from 0..1, inclusive.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.RankDotAvg, "Returns the rank of a number in a list of numbers: its size relative to other values in the list; if more than one value has the same rank, the average rank is returned.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.RankDotEq, "Returns the rank of a number in a list of numbers: its size relative to other values in the list; if more than one value has the same rank, the top rank of that set of values is returned.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.StDevDotS, "Estimates standard deviation based on a sample (ignores logical values and text in the sample).");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.StDevDotP, "Calculates standard deviation based on the entire population given as arguments (ignores logical values and text).");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.TDotDist, "Returns the left-tailed Student's t-distribution.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.TDotDistDot2t, "Returns the two-tailed Student's t-distribution.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.TDotDistDotTt, "Returns the right-tailed Student's t-distribution.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.TDotInv, "Returns the left-tailed inverse of the Student's t-distribution.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.TDotInvDot2t, "Returns the two-tailed inverse of the Student's t-distribution.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.VarDotS, "Estimates variance based on a sample (ignores logical values and text in the sample).");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.VarDotP, "Calculates variance based on the entire population (ignores logical values and text in the population).");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.WeibullDotDist, "Returns the Weibull distribution.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.NetworkDaysIntl, "Returns the number of whole workdays between two dates with custom weekend parameters.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.WorkDayIntl, "Returns the serial number of the date before or after a specified number of workdays with custom weekend parameters.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.IsoCeiling, "Rounds a number up, to the nearest integer or to the nearest multiple of significance.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.BetaDist, "Returns the beta probability distribution function.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.BetaDotInv, "Returns the inverse of the cumulative beta probability density function (BETA.DIST).");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ChisqDotDist, "Returns the left-tailed probability of the chi-squared distribution.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ChisqDotDistDotRt, "Returns the right-tailed probability of the chi-squared distribution.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ChisqDotInv, "Returns the inverse of the left-tailed probability of the chi-squared distribution.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ChisqDotInvDotRt, "Returns the inverse of the right-tailed probability of the chi-squared distribution.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.FDotDist, "Returns the (left-tailed) F probability distribution (degree of diversity) for two data sets.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.FDotDistDotRt, "Returns the (right-tailed) F probability distribution (degree of diversity) for two data sets.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.FDotInv, "Returns the inverse of the (left-tailed) F probability distribution: if p = F.DIST(x,...), then F.INV(p,...) = x.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.FDotinvDotRt, "Returns the inverse of the (right-tailed) F probability distribution: if p = F.DIST.RT(x,...), then F.INV.RT(p,...) = x.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.HypgeomDotDist, "Returns the hypergeometric distribution.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.LogNormDist, "Returns the lognormal distribution of x, where ln(x) is normally distributed with parameters Mean and Standard_dev.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.LogNormInv, "Returns the inverse of the lognormal cumulative distribution function of x, where ln(x) is normally distributed with parameters Mean and Standard_dev.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.NegbinomDotDist, "Returns the negative binomial distribution, the probability that there will be Number_f failures before the Number_s-th success, with Probability_s probability of a success.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.NormSDist, "Returns the standard normal distribution (has a mean of zero and a standard deviation of one).");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.NormSInv, "Returns the inverse of the standard normal cumulative distribution (has a mean of zero and a standard deviation of one).");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.TDotTest, "Returns the probability associated with a Student's t-Test.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ZDotTest, "Returns the one-tailed P-value of a z-test.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ErfPrecise, "Returns the error function.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ErfcPrecise, "Returns the complementary error function.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.GammaLnPrecise, "Returns the natural logarithm of the gamma function.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.CeilingDotPrecise, "Rounds a number up, to the nearest integer or to the nearest multiple of significance.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.FloorDotPrecise, "Rounds a number down, to the nearest integer or to the nearest multiple of significance.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ACot, "Returns the arccotangent of a number, in radians in the range 0 to Pi.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ACotH, "Returns the inverse hyperbolic cotangent of a number.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Cot, "Returns the cotangent of an angle.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.CotH, "Returns the hyperbolic cotangent of a number.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Csc, "Returns the cosecant of an angle.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.CscH, "Returns the hyperbolic cosecant of an angle.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Sec, "Returns the secant of an angle.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.SecH, "Returns the hyperbolic secant of an angle.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ImTan, "Returns the tangent of a complex number.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ImCot, "Returns the cotangent of a complex number.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ImCsc, "Returns the cosecant of a complex number.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ImCscH, "Returns the hyperbolic cosecant of a complex number.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ImSec, "Returns the secant of a complex number.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ImSecH, "Returns the hyperbolic secant of a complex number.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.BitAnd, "Returns a bitwise 'And' of two numbers.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.BitOr, "Returns a bitwise 'Or' of two numbers.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.BitXor, "Returns a bitwise 'Exclusive Or' of two numbers.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.BitLShift, "Returns a number shifted left by shift_amount bits.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.BitRShift, "Returns a number shifted right by shift_amount bits.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Permutationa, "Returns the number of permutations for a given number of objects (with repetitions) that can be selected from the total objects.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.CombinA, "Returns the number of combinations with repetitions for a given number of items.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.XOr, "Returns a logical 'Exclusive Or' of all arguments.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.PDuration, "Returns the number of periods required by an investment to reach a specified value.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Base, "Converts a number into a text representation with the given radix (base).");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Decimal, "Converts a text representation of a number in a given base into a decimal number.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Days, "Returns the number of days between the two dates.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.BinomDistRange, "Returns the probability of a trial result using a binomial distribution.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Gamma, "Returns the Gamma function value.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.SkewP, "Returns the skewness of a distribution based on a population: a characterization of the degree of asymmetry of a distribution around its mean.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Phi, "Returns the value of the density function for a standard normal distribution.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.RRI, "Returns an equivalent interest rate for the growth of an investment.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Unicode, "Returns the number (code point) corresponding to the first character of the text.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.MUnit, "Returns the unit matrix for the specified dimension.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Arabic, "Converts a Roman numeral to Arabic.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ISOWeekNum, "Returns number of the ISO week number of the year for a given date.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.NumberValue, "Converts text to number in a locale-independent manner.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Sheet, "Returns the sheet number of the referenced sheet.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Sheets, "Returns the number of sheets in a reference.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.FormulaText, "Returns a formula as a string.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.IsFormula, "Checks whether a reference is to a cell containing a formula, and returns TRUE or FALSE.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.IfNA, "Returns the value you specify if the expression resolves to #N/A, otherwise returns the result of the expression.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.CeilingMath, "Rounds a number up, to the nearest integer or to the nearest multiple of significance.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.FloorMath, "Rounds a number down, to the nearest integer or to the nearest multiple of significance.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ImSinH, "Returns the hyperbolic sine of a complex number.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.ImCosH, "Returns the hyperbolic cosine of a complex number.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.FunctionFilterXML, "Returns specific data from the XML content by using the specified XPath.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.FunctionWebService, "Returns data from a web service.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.EncodeURL, "Returns a URL-encoded string.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Field, "Retrieves a data value from the corresponding field in the data source.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Range, "Returns a cell range into which the specified template cell will be expanded in the resulting document after a mail merge is performed.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.FieldPicture, "Retrieves a picture from the specified data field and inserts it into a merged document.");
			AddString(XtraSpreadsheetFunctionDescriptionStringId.Parameter, "Retrieves the value of the mail merge parameter.");
		}
		#endregion
		public static XtraLocalizer<XtraSpreadsheetFunctionDescriptionStringId> CreateDefaultLocalizer() {
			return new XtraSpreadsheetFunctionDescriptionResLocalizer();
		}
		public static string GetString(XtraSpreadsheetFunctionDescriptionStringId id) {
			return Active.GetLocalizedString(id);
		}
		public override XtraLocalizer<XtraSpreadsheetFunctionDescriptionStringId> CreateResXLocalizer() {
			return new XtraSpreadsheetFunctionDescriptionResLocalizer();
		}
		protected override void AddString(XtraSpreadsheetFunctionDescriptionStringId id, string str) {
			Dictionary<XtraSpreadsheetFunctionDescriptionStringId, string> table = XtraLocalizierHelper<XtraSpreadsheetFunctionDescriptionStringId>.GetStringTable(this);
			table[id] = str;
		}
	}
	#endregion
	#region XtraSpreadsheetFunctionDescriptionResLocalizer
	public class XtraSpreadsheetFunctionDescriptionResLocalizer : XtraResXLocalizer<XtraSpreadsheetFunctionDescriptionStringId> {
		static XtraSpreadsheetFunctionDescriptionResLocalizer() {
			SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<XtraSpreadsheetFunctionDescriptionStringId>(CreateDefaultLocalizer()));
		}
		public XtraSpreadsheetFunctionDescriptionResLocalizer()
			: base(new XtraSpreadsheetFunctionDescriptionLocalizer()) { }
		public static XtraLocalizer<XtraSpreadsheetFunctionDescriptionStringId> CreateDefaultLocalizer() {
			return new XtraSpreadsheetFunctionDescriptionResLocalizer();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new ResourceManager Manager {
			get { return base.Manager; }
		}
		protected override ResourceManager CreateResourceManagerCore() {
#if DXPORTABLE
			return new ResourceManager("DevExpress.Spreadsheet.Core.LocalizationFunctionDescriptionsRes", typeof(XtraSpreadsheetFunctionDescriptionResLocalizer).GetAssembly());
#else
			return new ResourceManager("DevExpress.XtraSpreadsheet.LocalizationFunctionDescriptionsRes", typeof(XtraSpreadsheetFunctionDescriptionResLocalizer).GetAssembly());
#endif
		}
#if DXPORTABLE
		public static string GetString(XtraSpreadsheetFunctionDescriptionStringId id, CultureInfo culture) {
			return XtraResXLocalizer<XtraSpreadsheetFunctionDescriptionStringId>.GetLocalizedStringFromResources<XtraSpreadsheetFunctionDescriptionStringId, XtraSpreadsheetFunctionDescriptionResLocalizer>(
				id,
				culture,
				() => Active as XtraSpreadsheetFunctionDescriptionResLocalizer,
				(stringId) => XtraSpreadsheetFunctionDescriptionLocalizer.GetString(stringId)
			);
		}
#else
		[ThreadStatic] static CultureInfo lastCulture;
		[ThreadStatic] static ResourceSet lastSet;
		public static string GetString(XtraSpreadsheetFunctionDescriptionStringId id, CultureInfo culture) {
			if (culture == lastCulture)
				return GetString(id, lastSet);
			XtraSpreadsheetFunctionDescriptionLocalizer.GetString(id); 
			lastCulture = culture;
			lastSet = null;
			XtraSpreadsheetFunctionDescriptionResLocalizer localizer = Active as XtraSpreadsheetFunctionDescriptionResLocalizer;
			if (localizer == null)
				return XtraSpreadsheetFunctionDescriptionLocalizer.GetString(id);
			lastSet = localizer.Manager.GetResourceSet(culture, true, true);
			return GetString(id, lastSet);
		}
		static string GetString(XtraSpreadsheetFunctionDescriptionStringId id, ResourceSet set) {
			if (set == null)
				return XtraSpreadsheetFunctionDescriptionLocalizer.GetString(id);
			string resStr = String.Format("{0}.{1}", typeof(XtraSpreadsheetFunctionDescriptionStringId).Name, id);
			string result = set.GetString(resStr);
			if (!String.IsNullOrEmpty(result))
				return result;
			return XtraSpreadsheetFunctionDescriptionLocalizer.GetString(id);
		}
#endif
	}
#endregion
}
