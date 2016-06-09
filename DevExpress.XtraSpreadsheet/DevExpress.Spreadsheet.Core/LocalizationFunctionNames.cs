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
	#region XtraSpreadsheetFunctionNameStringId
	public enum XtraSpreadsheetFunctionNameStringId {
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
		Gauss = 0x4076,
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
		AddressInfo = 0x2000,
		ColumnInfo = 0x2001,
		ColorInfo = 0x2002,
		ContentsInfo = 0x2003,
		FilenameInfo = 0x2004,
		FormatInfo = 0x2005,
		ParenthesesInfo = 0x2006,
		PrefixInfo = 0x2007,
		ProtectInfo = 0x2008,
		RowInfo = 0x2009,
		TypeInfo = 0x200A,
		WidthInfo = 0x200B,
		DirectoryInfo = 0x200C,
		NumFileInfo = 0x200D,
		OriginInfo = 0x200E,
		OsVersionInfo = 0x200F,
		RecalcInfo = 0x2010,
		ReleaseInfo = 0x2011,
		SystemInfo = 0x2012,
		MemAvailInfo = 0x2013,
		TotMemInfo = 0x2014,
		MemUsedInfo = 0x2015,
		AutoCalcMode = 0x2016,
		ManualCalcMode = 0x2017,
	}
	#endregion
	#region XtraSpreadsheetFunctionNameLocalizer
	public class XtraSpreadsheetFunctionNameLocalizer : XtraLocalizer<XtraSpreadsheetFunctionNameStringId> {
		static XtraSpreadsheetFunctionNameLocalizer() {
			SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<XtraSpreadsheetFunctionNameStringId>(CreateDefaultLocalizer()));
		}
		#region PopulateStringTable
		protected override void PopulateStringTable() {
			AddString(XtraSpreadsheetFunctionNameStringId.BetaDistCompatibility, "BETADIST");
			AddString(XtraSpreadsheetFunctionNameStringId.Abs, "ABS");
			AddString(XtraSpreadsheetFunctionNameStringId.Accrint, "ACCRINT");
			AddString(XtraSpreadsheetFunctionNameStringId.Accrintm, "ACCRINTM");
			AddString(XtraSpreadsheetFunctionNameStringId.ACos, "ACOS");
			AddString(XtraSpreadsheetFunctionNameStringId.ACosH, "ACOSH");
			AddString(XtraSpreadsheetFunctionNameStringId.ACot, "ACOT");
			AddString(XtraSpreadsheetFunctionNameStringId.ACotH, "ACOTH");
			AddString(XtraSpreadsheetFunctionNameStringId.Address, "ADDRESS");
			AddString(XtraSpreadsheetFunctionNameStringId.Aggregate, "AGGREGATE");
			AddString(XtraSpreadsheetFunctionNameStringId.Amordegrc, "AMORDEGRC");
			AddString(XtraSpreadsheetFunctionNameStringId.Amorlinc, "AMORLINC");
			AddString(XtraSpreadsheetFunctionNameStringId.And, "AND");
			AddString(XtraSpreadsheetFunctionNameStringId.Arabic, "ARABIC");
			AddString(XtraSpreadsheetFunctionNameStringId.Areas, "AREAS");
			AddString(XtraSpreadsheetFunctionNameStringId.Asc, "ASC");
			AddString(XtraSpreadsheetFunctionNameStringId.ASin, "ASIN");
			AddString(XtraSpreadsheetFunctionNameStringId.ASinH, "ASINH");
			AddString(XtraSpreadsheetFunctionNameStringId.ATan, "ATAN");
			AddString(XtraSpreadsheetFunctionNameStringId.ATan2, "ATAN2");
			AddString(XtraSpreadsheetFunctionNameStringId.ATanH, "ATANH");
			AddString(XtraSpreadsheetFunctionNameStringId.Avedev, "AVEDEV");
			AddString(XtraSpreadsheetFunctionNameStringId.Average, "AVERAGE");
			AddString(XtraSpreadsheetFunctionNameStringId.AverageA, "AVERAGEA");
			AddString(XtraSpreadsheetFunctionNameStringId.AverageIf, "AVERAGEIF");
			AddString(XtraSpreadsheetFunctionNameStringId.AverageIfs, "AVERAGEIFS");
			AddString(XtraSpreadsheetFunctionNameStringId.BahtText, "BAHTTEXT");
			AddString(XtraSpreadsheetFunctionNameStringId.Base, "BASE");
			AddString(XtraSpreadsheetFunctionNameStringId.BesselI, "BESSELI");
			AddString(XtraSpreadsheetFunctionNameStringId.BesselJ, "BESSELJ");
			AddString(XtraSpreadsheetFunctionNameStringId.BesselK, "BESSELK");
			AddString(XtraSpreadsheetFunctionNameStringId.BesselY, "BESSELY");
			AddString(XtraSpreadsheetFunctionNameStringId.BetaDist, "BETA.DIST");
			AddString(XtraSpreadsheetFunctionNameStringId.BetaDotInv, "BETA.INV");
			AddString(XtraSpreadsheetFunctionNameStringId.BetaInv, "BETAINV");
			AddString(XtraSpreadsheetFunctionNameStringId.Bin2Dec, "BIN2DEC");
			AddString(XtraSpreadsheetFunctionNameStringId.Bin2Hex, "BIN2HEX");
			AddString(XtraSpreadsheetFunctionNameStringId.Bin2Oct, "BIN2OCT");
			AddString(XtraSpreadsheetFunctionNameStringId.BinomDist, "BINOMDIST");
			AddString(XtraSpreadsheetFunctionNameStringId.BinomDistRange, "BINOM.DIST.RANGE");
			AddString(XtraSpreadsheetFunctionNameStringId.BinomDotDist, "BINOM.DIST");
			AddString(XtraSpreadsheetFunctionNameStringId.BinomDotInv, "BINOM.INV");
			AddString(XtraSpreadsheetFunctionNameStringId.BitAnd, "BITAND");
			AddString(XtraSpreadsheetFunctionNameStringId.BitLShift, "BITLSHIFT");
			AddString(XtraSpreadsheetFunctionNameStringId.BitOr, "BITOR");
			AddString(XtraSpreadsheetFunctionNameStringId.BitRShift, "BITRSHIFT");
			AddString(XtraSpreadsheetFunctionNameStringId.BitXor, "BITXOR");
			AddString(XtraSpreadsheetFunctionNameStringId.Ceiling, "CEILING");
			AddString(XtraSpreadsheetFunctionNameStringId.CeilingDotPrecise, "CEILING.PRECISE");
			AddString(XtraSpreadsheetFunctionNameStringId.CeilingMath, "CEILING.MATH");
			AddString(XtraSpreadsheetFunctionNameStringId.Cell, "CELL");
			AddString(XtraSpreadsheetFunctionNameStringId.Char, "CHAR");
			AddString(XtraSpreadsheetFunctionNameStringId.ChiDist, "CHIDIST");
			AddString(XtraSpreadsheetFunctionNameStringId.ChiInv, "CHIINV");
			AddString(XtraSpreadsheetFunctionNameStringId.ChisqDotDist, "CHISQ.DIST");
			AddString(XtraSpreadsheetFunctionNameStringId.ChisqDotDistDotRt, "CHISQ.DIST.RT");
			AddString(XtraSpreadsheetFunctionNameStringId.ChisqDotInv, "CHISQ.INV");
			AddString(XtraSpreadsheetFunctionNameStringId.ChisqDotInvDotRt, "CHISQ.INV.RT");
			AddString(XtraSpreadsheetFunctionNameStringId.ChisqDotTest, "CHISQ.TEST");
			AddString(XtraSpreadsheetFunctionNameStringId.ChiTest, "CHITEST");
			AddString(XtraSpreadsheetFunctionNameStringId.Choose, "CHOOSE");
			AddString(XtraSpreadsheetFunctionNameStringId.Clean, "CLEAN");
			AddString(XtraSpreadsheetFunctionNameStringId.Code, "CODE");
			AddString(XtraSpreadsheetFunctionNameStringId.Column, "COLUMN");
			AddString(XtraSpreadsheetFunctionNameStringId.Columns, "COLUMNS");
			AddString(XtraSpreadsheetFunctionNameStringId.Combin, "COMBIN");
			AddString(XtraSpreadsheetFunctionNameStringId.CombinA, "COMBINA");
			AddString(XtraSpreadsheetFunctionNameStringId.Complex, "COMPLEX");
			AddString(XtraSpreadsheetFunctionNameStringId.Concatenate, "CONCATENATE");
			AddString(XtraSpreadsheetFunctionNameStringId.Confidence, "CONFIDENCE");
			AddString(XtraSpreadsheetFunctionNameStringId.ConfidenceDotT, "CONFIDENCE.T");
			AddString(XtraSpreadsheetFunctionNameStringId.ConfidenceNorm, "CONFIDENCE.NORM");
			AddString(XtraSpreadsheetFunctionNameStringId.Convert, "CONVERT");
			AddString(XtraSpreadsheetFunctionNameStringId.Correl, "CORREL");
			AddString(XtraSpreadsheetFunctionNameStringId.Cos, "COS");
			AddString(XtraSpreadsheetFunctionNameStringId.CosH, "COSH");
			AddString(XtraSpreadsheetFunctionNameStringId.Cot, "COT");
			AddString(XtraSpreadsheetFunctionNameStringId.CotH, "COTH");
			AddString(XtraSpreadsheetFunctionNameStringId.Count, "COUNT");
			AddString(XtraSpreadsheetFunctionNameStringId.CountA, "COUNTA");
			AddString(XtraSpreadsheetFunctionNameStringId.CountBlank, "COUNTBLANK");
			AddString(XtraSpreadsheetFunctionNameStringId.CountIf, "COUNTIF");
			AddString(XtraSpreadsheetFunctionNameStringId.CountIfs, "COUNTIFS");
			AddString(XtraSpreadsheetFunctionNameStringId.CoupDaybs, "COUPDAYBS");
			AddString(XtraSpreadsheetFunctionNameStringId.CoupDays, "COUPDAYS");
			AddString(XtraSpreadsheetFunctionNameStringId.CoupDaysnc, "COUPDAYSNC");
			AddString(XtraSpreadsheetFunctionNameStringId.Coupncd, "COUPNCD");
			AddString(XtraSpreadsheetFunctionNameStringId.CoupNum, "COUPNUM");
			AddString(XtraSpreadsheetFunctionNameStringId.Couppcd, "COUPPCD");
			AddString(XtraSpreadsheetFunctionNameStringId.Covar, "COVAR");
			AddString(XtraSpreadsheetFunctionNameStringId.CovarianceP, "COVARIANCE.P");
			AddString(XtraSpreadsheetFunctionNameStringId.CovarianceS, "COVARIANCE.S");
			AddString(XtraSpreadsheetFunctionNameStringId.CritBinom, "CRITBINOM");
			AddString(XtraSpreadsheetFunctionNameStringId.Csc, "CSC");
			AddString(XtraSpreadsheetFunctionNameStringId.CscH, "CSCH");
			AddString(XtraSpreadsheetFunctionNameStringId.CubeKpiMember, "CUBEKPIMEMBER");
			AddString(XtraSpreadsheetFunctionNameStringId.CubeMember, "CUBEMEMBER");
			AddString(XtraSpreadsheetFunctionNameStringId.CubeMemberProperty, "CUBEMEMBERPROPERTY");
			AddString(XtraSpreadsheetFunctionNameStringId.CubeRankedMember, "CUBERANKEDMEMBER");
			AddString(XtraSpreadsheetFunctionNameStringId.CubeSet, "CUBESET");
			AddString(XtraSpreadsheetFunctionNameStringId.CubeSetCount, "CUBESETCOUNT");
			AddString(XtraSpreadsheetFunctionNameStringId.CubeValue, "CUBEVALUE");
			AddString(XtraSpreadsheetFunctionNameStringId.CumIpmt, "CUMIPMT");
			AddString(XtraSpreadsheetFunctionNameStringId.CumPrinc, "CUMPRINC");
			AddString(XtraSpreadsheetFunctionNameStringId.Date, "DATE");
			AddString(XtraSpreadsheetFunctionNameStringId.DateDif, "DATEDIF");
			AddString(XtraSpreadsheetFunctionNameStringId.DateValue, "DATEVALUE");
			AddString(XtraSpreadsheetFunctionNameStringId.DAverage, "DAVERAGE");
			AddString(XtraSpreadsheetFunctionNameStringId.Day, "DAY");
			AddString(XtraSpreadsheetFunctionNameStringId.Days, "DAYS");
			AddString(XtraSpreadsheetFunctionNameStringId.Days360, "DAYS360");
			AddString(XtraSpreadsheetFunctionNameStringId.Db, "DB");
			AddString(XtraSpreadsheetFunctionNameStringId.DCount, "DCOUNT");
			AddString(XtraSpreadsheetFunctionNameStringId.DCountA, "DCOUNTA");
			AddString(XtraSpreadsheetFunctionNameStringId.Ddb, "DDB");
			AddString(XtraSpreadsheetFunctionNameStringId.Dec2Bin, "DEC2BIN");
			AddString(XtraSpreadsheetFunctionNameStringId.Dec2Hex, "DEC2HEX");
			AddString(XtraSpreadsheetFunctionNameStringId.Dec2Oct, "DEC2OCT");
			AddString(XtraSpreadsheetFunctionNameStringId.Decimal, "DECIMAL");
			AddString(XtraSpreadsheetFunctionNameStringId.Degrees, "DEGREES");
			AddString(XtraSpreadsheetFunctionNameStringId.Delta, "DELTA");
			AddString(XtraSpreadsheetFunctionNameStringId.DevSq, "DEVSQ");
			AddString(XtraSpreadsheetFunctionNameStringId.DGet, "DGET");
			AddString(XtraSpreadsheetFunctionNameStringId.Disc, "DISC");
			AddString(XtraSpreadsheetFunctionNameStringId.DMax, "DMAX");
			AddString(XtraSpreadsheetFunctionNameStringId.DMin, "DMIN");
			AddString(XtraSpreadsheetFunctionNameStringId.Dollar, "DOLLAR");
			AddString(XtraSpreadsheetFunctionNameStringId.DollarDe, "DOLLARDE");
			AddString(XtraSpreadsheetFunctionNameStringId.DollarFr, "DOLLARFR");
			AddString(XtraSpreadsheetFunctionNameStringId.DProduct, "DPRODUCT");
			AddString(XtraSpreadsheetFunctionNameStringId.DStDev, "DSTDEV");
			AddString(XtraSpreadsheetFunctionNameStringId.DStDevP, "DSTDEVP");
			AddString(XtraSpreadsheetFunctionNameStringId.DSum, "DSUM");
			AddString(XtraSpreadsheetFunctionNameStringId.Duration, "DURATION");
			AddString(XtraSpreadsheetFunctionNameStringId.DVar, "DVAR");
			AddString(XtraSpreadsheetFunctionNameStringId.DVarP, "DVARP");
			AddString(XtraSpreadsheetFunctionNameStringId.EDate, "EDATE");
			AddString(XtraSpreadsheetFunctionNameStringId.Effect, "EFFECT");
			AddString(XtraSpreadsheetFunctionNameStringId.EncodeURL, "ENCODEURL");
			AddString(XtraSpreadsheetFunctionNameStringId.EOMonth, "EOMONTH");
			AddString(XtraSpreadsheetFunctionNameStringId.Erf, "ERF");
			AddString(XtraSpreadsheetFunctionNameStringId.Erfc, "ERFC");
			AddString(XtraSpreadsheetFunctionNameStringId.ErfcPrecise, "ERFC.PRECISE");
			AddString(XtraSpreadsheetFunctionNameStringId.ErfPrecise, "ERF.PRECISE");
			AddString(XtraSpreadsheetFunctionNameStringId.ErrorType, "ERROR.TYPE");
			AddString(XtraSpreadsheetFunctionNameStringId.Even, "EVEN");
			AddString(XtraSpreadsheetFunctionNameStringId.Exact, "EXACT");
			AddString(XtraSpreadsheetFunctionNameStringId.Exp, "EXP");
			AddString(XtraSpreadsheetFunctionNameStringId.ExponDist, "EXPONDIST");
			AddString(XtraSpreadsheetFunctionNameStringId.ExponDotDist, "EXPON.DIST");
			AddString(XtraSpreadsheetFunctionNameStringId.Fact, "FACT");
			AddString(XtraSpreadsheetFunctionNameStringId.FactDouble, "FACTDOUBLE");
			AddString(XtraSpreadsheetFunctionNameStringId.False, "FALSE");
			AddString(XtraSpreadsheetFunctionNameStringId.FDist, "FDIST");
			AddString(XtraSpreadsheetFunctionNameStringId.FDotDist, "F.DIST");
			AddString(XtraSpreadsheetFunctionNameStringId.FDotDistDotRt, "F.DIST.RT");
			AddString(XtraSpreadsheetFunctionNameStringId.FDotInv, "F.INV");
			AddString(XtraSpreadsheetFunctionNameStringId.FDotinvDotRt, "F.INV.RT");
			AddString(XtraSpreadsheetFunctionNameStringId.FDotTest, "F.TEST");
			AddString(XtraSpreadsheetFunctionNameStringId.Find, "FIND");
			AddString(XtraSpreadsheetFunctionNameStringId.FindB, "FINDB");
			AddString(XtraSpreadsheetFunctionNameStringId.FInv, "FINV");
			AddString(XtraSpreadsheetFunctionNameStringId.Fisher, "FISHER");
			AddString(XtraSpreadsheetFunctionNameStringId.FisherInv, "FISHERINV");
			AddString(XtraSpreadsheetFunctionNameStringId.Fixed, "FIXED");
			AddString(XtraSpreadsheetFunctionNameStringId.Floor, "FLOOR");
			AddString(XtraSpreadsheetFunctionNameStringId.FloorDotPrecise, "FLOOR.PRECISE");
			AddString(XtraSpreadsheetFunctionNameStringId.FloorMath, "FLOOR.MATH");
			AddString(XtraSpreadsheetFunctionNameStringId.Forecast, "FORECAST");
			AddString(XtraSpreadsheetFunctionNameStringId.FormulaText, "FORMULATEXT");
			AddString(XtraSpreadsheetFunctionNameStringId.Frequency, "FREQUENCY");
			AddString(XtraSpreadsheetFunctionNameStringId.FTest, "FTEST");
			AddString(XtraSpreadsheetFunctionNameStringId.FunctionFilterXML, "FILTERXML");
			AddString(XtraSpreadsheetFunctionNameStringId.FunctionWebService, "WEBSERVICE");
			AddString(XtraSpreadsheetFunctionNameStringId.Fv, "FV");
			AddString(XtraSpreadsheetFunctionNameStringId.FvSchedule, "FVSCHEDULE");
			AddString(XtraSpreadsheetFunctionNameStringId.Gamma, "GAMMA");
			AddString(XtraSpreadsheetFunctionNameStringId.GammaDist, "GAMMADIST");
			AddString(XtraSpreadsheetFunctionNameStringId.GammaDotDist, "GAMMA.DIST");
			AddString(XtraSpreadsheetFunctionNameStringId.GammaDotInv, "GAMMA.INV");
			AddString(XtraSpreadsheetFunctionNameStringId.GammaInv, "GAMMAINV");
			AddString(XtraSpreadsheetFunctionNameStringId.GammaLn, "GAMMALN");
			AddString(XtraSpreadsheetFunctionNameStringId.GammaLnPrecise, "GAMMALN.PRECISE");
			AddString(XtraSpreadsheetFunctionNameStringId.Gauss, "GAUSS");
			AddString(XtraSpreadsheetFunctionNameStringId.Gcd, "GCD");
			AddString(XtraSpreadsheetFunctionNameStringId.Geomean, "GEOMEAN");
			AddString(XtraSpreadsheetFunctionNameStringId.Gestep, "GESTEP");
			AddString(XtraSpreadsheetFunctionNameStringId.GetPivotData, "GETPIVOTDATA");
			AddString(XtraSpreadsheetFunctionNameStringId.Growth, "GROWTH");
			AddString(XtraSpreadsheetFunctionNameStringId.HarMean, "HARMEAN");
			AddString(XtraSpreadsheetFunctionNameStringId.Hex2Bin, "HEX2BIN");
			AddString(XtraSpreadsheetFunctionNameStringId.Hex2Dec, "HEX2DEC");
			AddString(XtraSpreadsheetFunctionNameStringId.Hex2Oct, "HEX2OCT");
			AddString(XtraSpreadsheetFunctionNameStringId.HLookup, "HLOOKUP");
			AddString(XtraSpreadsheetFunctionNameStringId.Hour, "HOUR");
			AddString(XtraSpreadsheetFunctionNameStringId.Hyperlink, "HYPERLINK");
			AddString(XtraSpreadsheetFunctionNameStringId.HypGeomDist, "HYPGEOMDIST");
			AddString(XtraSpreadsheetFunctionNameStringId.HypgeomDotDist, "HYPGEOM.DIST");
			AddString(XtraSpreadsheetFunctionNameStringId.If, "IF");
			AddString(XtraSpreadsheetFunctionNameStringId.IfError, "IFERROR");
			AddString(XtraSpreadsheetFunctionNameStringId.IfNA, "IFNA");
			AddString(XtraSpreadsheetFunctionNameStringId.ImAbs, "IMABS");
			AddString(XtraSpreadsheetFunctionNameStringId.Imaginary, "IMAGINARY");
			AddString(XtraSpreadsheetFunctionNameStringId.ImArgument, "IMARGUMENT");
			AddString(XtraSpreadsheetFunctionNameStringId.ImConjugate, "IMCONJUGATE");
			AddString(XtraSpreadsheetFunctionNameStringId.ImCos, "IMCOS");
			AddString(XtraSpreadsheetFunctionNameStringId.ImCosH, "IMCOSH");
			AddString(XtraSpreadsheetFunctionNameStringId.ImCot, "IMCOT");
			AddString(XtraSpreadsheetFunctionNameStringId.ImCsc, "IMCSC");
			AddString(XtraSpreadsheetFunctionNameStringId.ImCscH, "IMCSCH");
			AddString(XtraSpreadsheetFunctionNameStringId.ImDiv, "IMDIV");
			AddString(XtraSpreadsheetFunctionNameStringId.ImExp, "IMEXP");
			AddString(XtraSpreadsheetFunctionNameStringId.ImLn, "IMLN");
			AddString(XtraSpreadsheetFunctionNameStringId.ImLog10, "IMLOG10");
			AddString(XtraSpreadsheetFunctionNameStringId.ImLog2, "IMLOG2");
			AddString(XtraSpreadsheetFunctionNameStringId.ImPower, "IMPOWER");
			AddString(XtraSpreadsheetFunctionNameStringId.ImProduct, "IMPRODUCT");
			AddString(XtraSpreadsheetFunctionNameStringId.ImReal, "IMREAL");
			AddString(XtraSpreadsheetFunctionNameStringId.ImSec, "IMSEC");
			AddString(XtraSpreadsheetFunctionNameStringId.ImSecH, "IMSECH");
			AddString(XtraSpreadsheetFunctionNameStringId.ImSin, "IMSIN");
			AddString(XtraSpreadsheetFunctionNameStringId.ImSinH, "IMSINH");
			AddString(XtraSpreadsheetFunctionNameStringId.ImSqrt, "IMSQRT");
			AddString(XtraSpreadsheetFunctionNameStringId.ImSub, "IMSUB");
			AddString(XtraSpreadsheetFunctionNameStringId.ImSum, "IMSUM");
			AddString(XtraSpreadsheetFunctionNameStringId.ImTan, "IMTAN");
			AddString(XtraSpreadsheetFunctionNameStringId.Index, "INDEX");
			AddString(XtraSpreadsheetFunctionNameStringId.Indirect, "INDIRECT");
			AddString(XtraSpreadsheetFunctionNameStringId.Info, "INFO");
			AddString(XtraSpreadsheetFunctionNameStringId.Int, "INT");
			AddString(XtraSpreadsheetFunctionNameStringId.Intercept, "INTERCEPT");
			AddString(XtraSpreadsheetFunctionNameStringId.Intrate, "INTRATE");
			AddString(XtraSpreadsheetFunctionNameStringId.Ipmt, "IPMT");
			AddString(XtraSpreadsheetFunctionNameStringId.Irr, "IRR");
			AddString(XtraSpreadsheetFunctionNameStringId.IsBlank, "ISBLANK");
			AddString(XtraSpreadsheetFunctionNameStringId.IsErr, "ISERR");
			AddString(XtraSpreadsheetFunctionNameStringId.IsError, "ISERROR");
			AddString(XtraSpreadsheetFunctionNameStringId.IsEven, "ISEVEN");
			AddString(XtraSpreadsheetFunctionNameStringId.IsFormula, "ISFORMULA");
			AddString(XtraSpreadsheetFunctionNameStringId.IsLogical, "ISLOGICAL");
			AddString(XtraSpreadsheetFunctionNameStringId.IsNA, "ISNA");
			AddString(XtraSpreadsheetFunctionNameStringId.IsNonText, "ISNONTEXT");
			AddString(XtraSpreadsheetFunctionNameStringId.IsNumber, "ISNUMBER");
			AddString(XtraSpreadsheetFunctionNameStringId.IsoCeiling, "ISO.CEILING");
			AddString(XtraSpreadsheetFunctionNameStringId.IsOdd, "ISODD");
			AddString(XtraSpreadsheetFunctionNameStringId.ISOWeekNum, "ISOWEEKNUM");
			AddString(XtraSpreadsheetFunctionNameStringId.IsPmt, "ISPMT");
			AddString(XtraSpreadsheetFunctionNameStringId.IsRef, "ISREF");
			AddString(XtraSpreadsheetFunctionNameStringId.IsText, "ISTEXT");
			AddString(XtraSpreadsheetFunctionNameStringId.Kurt, "KURT");
			AddString(XtraSpreadsheetFunctionNameStringId.Large, "LARGE");
			AddString(XtraSpreadsheetFunctionNameStringId.Lcm, "LCM");
			AddString(XtraSpreadsheetFunctionNameStringId.Left, "LEFT");
			AddString(XtraSpreadsheetFunctionNameStringId.LeftB, "LEFTB");
			AddString(XtraSpreadsheetFunctionNameStringId.Len, "LEN");
			AddString(XtraSpreadsheetFunctionNameStringId.LenB, "LENB");
			AddString(XtraSpreadsheetFunctionNameStringId.Linest, "LINEST");
			AddString(XtraSpreadsheetFunctionNameStringId.Ln, "LN");
			AddString(XtraSpreadsheetFunctionNameStringId.Log, "LOG");
			AddString(XtraSpreadsheetFunctionNameStringId.Log10, "LOG10");
			AddString(XtraSpreadsheetFunctionNameStringId.Logest, "LOGEST");
			AddString(XtraSpreadsheetFunctionNameStringId.LogInvCompatibility, "LOGINV");
			AddString(XtraSpreadsheetFunctionNameStringId.LogNormDist, "LOGNORM.DIST");
			AddString(XtraSpreadsheetFunctionNameStringId.LogNormDistCompatibility, "LOGNORMDIST");
			AddString(XtraSpreadsheetFunctionNameStringId.LogNormInv, "LOGNORM.INV");
			AddString(XtraSpreadsheetFunctionNameStringId.Lookup, "LOOKUP");
			AddString(XtraSpreadsheetFunctionNameStringId.Lower, "LOWER");
			AddString(XtraSpreadsheetFunctionNameStringId.Match, "MATCH");
			AddString(XtraSpreadsheetFunctionNameStringId.Max, "MAX");
			AddString(XtraSpreadsheetFunctionNameStringId.MaxA, "MAXA");
			AddString(XtraSpreadsheetFunctionNameStringId.MDeterm, "MDETERM");
			AddString(XtraSpreadsheetFunctionNameStringId.MDuration, "MDURATION");
			AddString(XtraSpreadsheetFunctionNameStringId.Median, "MEDIAN");
			AddString(XtraSpreadsheetFunctionNameStringId.Mid, "MID");
			AddString(XtraSpreadsheetFunctionNameStringId.MidB, "MIDB");
			AddString(XtraSpreadsheetFunctionNameStringId.Min, "MIN");
			AddString(XtraSpreadsheetFunctionNameStringId.MinA, "MINA");
			AddString(XtraSpreadsheetFunctionNameStringId.Minute, "MINUTE");
			AddString(XtraSpreadsheetFunctionNameStringId.MInverse, "MINVERSE");
			AddString(XtraSpreadsheetFunctionNameStringId.Mirr, "MIRR");
			AddString(XtraSpreadsheetFunctionNameStringId.MMult, "MMULT");
			AddString(XtraSpreadsheetFunctionNameStringId.Mod, "MOD");
			AddString(XtraSpreadsheetFunctionNameStringId.Mode, "MODE");
			AddString(XtraSpreadsheetFunctionNameStringId.ModeMult, "MODE.MULT");
			AddString(XtraSpreadsheetFunctionNameStringId.ModeSngl, "MODE.SNGL");
			AddString(XtraSpreadsheetFunctionNameStringId.Month, "MONTH");
			AddString(XtraSpreadsheetFunctionNameStringId.MRound, "MROUND");
			AddString(XtraSpreadsheetFunctionNameStringId.Multinomial, "MULTINOMIAL");
			AddString(XtraSpreadsheetFunctionNameStringId.MUnit, "MUNIT");
			AddString(XtraSpreadsheetFunctionNameStringId.N, "N");
			AddString(XtraSpreadsheetFunctionNameStringId.NA, "NA");
			AddString(XtraSpreadsheetFunctionNameStringId.NegBinomDist, "NEGBINOMDIST");
			AddString(XtraSpreadsheetFunctionNameStringId.NegbinomDotDist, "NEGBINOM.DIST");
			AddString(XtraSpreadsheetFunctionNameStringId.NetworkDays, "NETWORKDAYS");
			AddString(XtraSpreadsheetFunctionNameStringId.NetworkDaysIntl, "NETWORKDAYS.INTL");
			AddString(XtraSpreadsheetFunctionNameStringId.Nominal, "NOMINAL");
			AddString(XtraSpreadsheetFunctionNameStringId.NormDist, "NORM.DIST");
			AddString(XtraSpreadsheetFunctionNameStringId.NormDistCompatibility, "NORMDIST");
			AddString(XtraSpreadsheetFunctionNameStringId.NormInv, "NORM.INV");
			AddString(XtraSpreadsheetFunctionNameStringId.NormInvCompatibility, "NORMINV");
			AddString(XtraSpreadsheetFunctionNameStringId.NormSDist, "NORM.S.DIST");
			AddString(XtraSpreadsheetFunctionNameStringId.NormSDistCompatibility, "NORMSDIST");
			AddString(XtraSpreadsheetFunctionNameStringId.NormSInv, "NORM.S.INV");
			AddString(XtraSpreadsheetFunctionNameStringId.NormSInvCompatibility, "NORMSINV");
			AddString(XtraSpreadsheetFunctionNameStringId.Not, "NOT");
			AddString(XtraSpreadsheetFunctionNameStringId.Now, "NOW");
			AddString(XtraSpreadsheetFunctionNameStringId.NPer, "NPER");
			AddString(XtraSpreadsheetFunctionNameStringId.Npv, "NPV");
			AddString(XtraSpreadsheetFunctionNameStringId.NumberValue, "NUMBERVALUE");
			AddString(XtraSpreadsheetFunctionNameStringId.Oct2Bin, "OCT2BIN");
			AddString(XtraSpreadsheetFunctionNameStringId.Oct2Dec, "OCT2DEC");
			AddString(XtraSpreadsheetFunctionNameStringId.Oct2Hex, "OCT2HEX");
			AddString(XtraSpreadsheetFunctionNameStringId.Odd, "ODD");
			AddString(XtraSpreadsheetFunctionNameStringId.OddFPrice, "ODDFPRICE");
			AddString(XtraSpreadsheetFunctionNameStringId.OddFYield, "ODDFYIELD");
			AddString(XtraSpreadsheetFunctionNameStringId.OddLPrice, "ODDLPRICE");
			AddString(XtraSpreadsheetFunctionNameStringId.OddLYield, "ODDLYIELD");
			AddString(XtraSpreadsheetFunctionNameStringId.Offset, "OFFSET");
			AddString(XtraSpreadsheetFunctionNameStringId.Or, "OR");
			AddString(XtraSpreadsheetFunctionNameStringId.PDuration, "PDURATION");
			AddString(XtraSpreadsheetFunctionNameStringId.Pearson, "PEARSON");
			AddString(XtraSpreadsheetFunctionNameStringId.Percentile, "PERCENTILE");
			AddString(XtraSpreadsheetFunctionNameStringId.PercentileExc, "PERCENTILE.EXC");
			AddString(XtraSpreadsheetFunctionNameStringId.PercentileInc, "PERCENTILE.INC");
			AddString(XtraSpreadsheetFunctionNameStringId.PercentRank, "PERCENTRANK");
			AddString(XtraSpreadsheetFunctionNameStringId.PercentRankExc, "PERCENTRANK.EXC");
			AddString(XtraSpreadsheetFunctionNameStringId.PercentRankInc, "PERCENTRANK.INC");
			AddString(XtraSpreadsheetFunctionNameStringId.Permut, "PERMUT");
			AddString(XtraSpreadsheetFunctionNameStringId.Permutationa, "PERMUTATIONA");
			AddString(XtraSpreadsheetFunctionNameStringId.Phi, "PHI");
			AddString(XtraSpreadsheetFunctionNameStringId.Phonetic, "PHONETIC");
			AddString(XtraSpreadsheetFunctionNameStringId.Pi, "PI");
			AddString(XtraSpreadsheetFunctionNameStringId.Pmt, "PMT");
			AddString(XtraSpreadsheetFunctionNameStringId.Poisson, "POISSON");
			AddString(XtraSpreadsheetFunctionNameStringId.PoissonDotDist, "POISSON.DIST");
			AddString(XtraSpreadsheetFunctionNameStringId.Power, "POWER");
			AddString(XtraSpreadsheetFunctionNameStringId.Ppmt, "PPMT");
			AddString(XtraSpreadsheetFunctionNameStringId.Price, "PRICE");
			AddString(XtraSpreadsheetFunctionNameStringId.PriceDisc, "PRICEDISC");
			AddString(XtraSpreadsheetFunctionNameStringId.PriceMat, "PRICEMAT");
			AddString(XtraSpreadsheetFunctionNameStringId.Prob, "PROB");
			AddString(XtraSpreadsheetFunctionNameStringId.Product, "PRODUCT");
			AddString(XtraSpreadsheetFunctionNameStringId.Proper, "PROPER");
			AddString(XtraSpreadsheetFunctionNameStringId.Pv, "PV");
			AddString(XtraSpreadsheetFunctionNameStringId.Quartile, "QUARTILE");
			AddString(XtraSpreadsheetFunctionNameStringId.QuartileExc, "QUARTILE.EXC");
			AddString(XtraSpreadsheetFunctionNameStringId.QuartileInc, "QUARTILE.INC");
			AddString(XtraSpreadsheetFunctionNameStringId.Quotient, "QUOTIENT");
			AddString(XtraSpreadsheetFunctionNameStringId.Radians, "RADIANS");
			AddString(XtraSpreadsheetFunctionNameStringId.Rand, "RAND");
			AddString(XtraSpreadsheetFunctionNameStringId.RandBetween, "RANDBETWEEN");
			AddString(XtraSpreadsheetFunctionNameStringId.Rank, "RANK");
			AddString(XtraSpreadsheetFunctionNameStringId.RankDotAvg, "RANK.AVG");
			AddString(XtraSpreadsheetFunctionNameStringId.RankDotEq, "RANK.EQ");
			AddString(XtraSpreadsheetFunctionNameStringId.Rate, "RATE");
			AddString(XtraSpreadsheetFunctionNameStringId.Received, "RECEIVED");
			AddString(XtraSpreadsheetFunctionNameStringId.Replace, "REPLACE");
			AddString(XtraSpreadsheetFunctionNameStringId.ReplaceB, "REPLACEB");
			AddString(XtraSpreadsheetFunctionNameStringId.Rept, "REPT");
			AddString(XtraSpreadsheetFunctionNameStringId.Right, "RIGHT");
			AddString(XtraSpreadsheetFunctionNameStringId.RightB, "RIGHTB");
			AddString(XtraSpreadsheetFunctionNameStringId.Roman, "ROMAN");
			AddString(XtraSpreadsheetFunctionNameStringId.Round, "ROUND");
			AddString(XtraSpreadsheetFunctionNameStringId.RoundDown, "ROUNDDOWN");
			AddString(XtraSpreadsheetFunctionNameStringId.RoundUp, "ROUNDUP");
			AddString(XtraSpreadsheetFunctionNameStringId.Row, "ROW");
			AddString(XtraSpreadsheetFunctionNameStringId.Rows, "ROWS");
			AddString(XtraSpreadsheetFunctionNameStringId.RRI, "RRI");
			AddString(XtraSpreadsheetFunctionNameStringId.Rsq, "RSQ");
			AddString(XtraSpreadsheetFunctionNameStringId.Rtd, "RTD");
			AddString(XtraSpreadsheetFunctionNameStringId.Search, "SEARCH");
			AddString(XtraSpreadsheetFunctionNameStringId.SearchB, "SEARCHB");
			AddString(XtraSpreadsheetFunctionNameStringId.Sec, "SEC");
			AddString(XtraSpreadsheetFunctionNameStringId.SecH, "SECH");
			AddString(XtraSpreadsheetFunctionNameStringId.Second, "SECOND");
			AddString(XtraSpreadsheetFunctionNameStringId.Seriessum, "SERIESSUM");
			AddString(XtraSpreadsheetFunctionNameStringId.Sheet, "SHEET");
			AddString(XtraSpreadsheetFunctionNameStringId.Sheets, "SHEETS");
			AddString(XtraSpreadsheetFunctionNameStringId.Sign, "SIGN");
			AddString(XtraSpreadsheetFunctionNameStringId.Sin, "SIN");
			AddString(XtraSpreadsheetFunctionNameStringId.SinH, "SINH");
			AddString(XtraSpreadsheetFunctionNameStringId.Skew, "SKEW");
			AddString(XtraSpreadsheetFunctionNameStringId.SkewP, "SKEW.P");
			AddString(XtraSpreadsheetFunctionNameStringId.Sln, "SLN");
			AddString(XtraSpreadsheetFunctionNameStringId.Slope, "SLOPE");
			AddString(XtraSpreadsheetFunctionNameStringId.Small, "SMALL");
			AddString(XtraSpreadsheetFunctionNameStringId.Sqrt, "SQRT");
			AddString(XtraSpreadsheetFunctionNameStringId.SqrtPi, "SQRTPI");
			AddString(XtraSpreadsheetFunctionNameStringId.Standardize, "STANDARDIZE");
			AddString(XtraSpreadsheetFunctionNameStringId.StDev, "STDEV");
			AddString(XtraSpreadsheetFunctionNameStringId.StDevA, "STDEVA");
			AddString(XtraSpreadsheetFunctionNameStringId.StDevDotP, "STDEV.P");
			AddString(XtraSpreadsheetFunctionNameStringId.StDevDotS, "STDEV.S");
			AddString(XtraSpreadsheetFunctionNameStringId.StDevP, "STDEVP");
			AddString(XtraSpreadsheetFunctionNameStringId.StDevPA, "STDEVPA");
			AddString(XtraSpreadsheetFunctionNameStringId.StEYX, "STEYX");
			AddString(XtraSpreadsheetFunctionNameStringId.Substitute, "SUBSTITUTE");
			AddString(XtraSpreadsheetFunctionNameStringId.Subtotal, "SUBTOTAL");
			AddString(XtraSpreadsheetFunctionNameStringId.Sum, "SUM");
			AddString(XtraSpreadsheetFunctionNameStringId.SumIf, "SUMIF");
			AddString(XtraSpreadsheetFunctionNameStringId.SumIfs, "SUMIFS");
			AddString(XtraSpreadsheetFunctionNameStringId.SumProduct, "SUMPRODUCT");
			AddString(XtraSpreadsheetFunctionNameStringId.SumSq, "SUMSQ");
			AddString(XtraSpreadsheetFunctionNameStringId.SumX2MY2, "SUMX2MY2");
			AddString(XtraSpreadsheetFunctionNameStringId.SumX2PY2, "SUMX2PY2");
			AddString(XtraSpreadsheetFunctionNameStringId.SumXMY2, "SUMXMY2");
			AddString(XtraSpreadsheetFunctionNameStringId.Syd, "SYD");
			AddString(XtraSpreadsheetFunctionNameStringId.T, "T");
			AddString(XtraSpreadsheetFunctionNameStringId.Tan, "TAN");
			AddString(XtraSpreadsheetFunctionNameStringId.TanH, "TANH");
			AddString(XtraSpreadsheetFunctionNameStringId.TbillEq, "TBILLEQ");
			AddString(XtraSpreadsheetFunctionNameStringId.TbillPrice, "TBILLPRICE");
			AddString(XtraSpreadsheetFunctionNameStringId.TbillYield, "TBILLYIELD");
			AddString(XtraSpreadsheetFunctionNameStringId.TDist, "TDIST");
			AddString(XtraSpreadsheetFunctionNameStringId.TDotDist, "T.DIST");
			AddString(XtraSpreadsheetFunctionNameStringId.TDotDistDot2t, "T.DIST.2T");
			AddString(XtraSpreadsheetFunctionNameStringId.TDotDistDotTt, "T.DIST.RT");
			AddString(XtraSpreadsheetFunctionNameStringId.TDotInv, "T.INV");
			AddString(XtraSpreadsheetFunctionNameStringId.TDotInvDot2t, "T.INV.2T");
			AddString(XtraSpreadsheetFunctionNameStringId.TDotTest, "T.TEST");
			AddString(XtraSpreadsheetFunctionNameStringId.Text, "TEXT");
			AddString(XtraSpreadsheetFunctionNameStringId.Time, "TIME");
			AddString(XtraSpreadsheetFunctionNameStringId.TimeValue, "TIMEVALUE");
			AddString(XtraSpreadsheetFunctionNameStringId.Tinv, "TINV");
			AddString(XtraSpreadsheetFunctionNameStringId.Today, "TODAY");
			AddString(XtraSpreadsheetFunctionNameStringId.Transpose, "TRANSPOSE");
			AddString(XtraSpreadsheetFunctionNameStringId.Trend, "TREND");
			AddString(XtraSpreadsheetFunctionNameStringId.Trim, "TRIM");
			AddString(XtraSpreadsheetFunctionNameStringId.Trimmean, "TRIMMEAN");
			AddString(XtraSpreadsheetFunctionNameStringId.True, "TRUE");
			AddString(XtraSpreadsheetFunctionNameStringId.Trunc, "TRUNC");
			AddString(XtraSpreadsheetFunctionNameStringId.TTest, "TTEST");
			AddString(XtraSpreadsheetFunctionNameStringId.Type, "TYPE");
			AddString(XtraSpreadsheetFunctionNameStringId.Unicode, "UNICODE");
			AddString(XtraSpreadsheetFunctionNameStringId.Upper, "UPPER");
			AddString(XtraSpreadsheetFunctionNameStringId.Value, "VALUE");
			AddString(XtraSpreadsheetFunctionNameStringId.Var, "VAR");
			AddString(XtraSpreadsheetFunctionNameStringId.VarA, "VARA");
			AddString(XtraSpreadsheetFunctionNameStringId.VarDotP, "VAR.P");
			AddString(XtraSpreadsheetFunctionNameStringId.VarDotS, "VAR.S");
			AddString(XtraSpreadsheetFunctionNameStringId.VarP, "VARP");
			AddString(XtraSpreadsheetFunctionNameStringId.VarPA, "VARPA");
			AddString(XtraSpreadsheetFunctionNameStringId.Vdb, "VDB");
			AddString(XtraSpreadsheetFunctionNameStringId.VLookup, "VLOOKUP");
			AddString(XtraSpreadsheetFunctionNameStringId.WeekDay, "WEEKDAY");
			AddString(XtraSpreadsheetFunctionNameStringId.WeekNum, "WEEKNUM");
			AddString(XtraSpreadsheetFunctionNameStringId.Weibull, "WEIBULL");
			AddString(XtraSpreadsheetFunctionNameStringId.WeibullDotDist, "WEIBULL.DIST");
			AddString(XtraSpreadsheetFunctionNameStringId.WorkDay, "WORKDAY");
			AddString(XtraSpreadsheetFunctionNameStringId.WorkDayIntl, "WORKDAY.INTL");
			AddString(XtraSpreadsheetFunctionNameStringId.Xirr, "XIRR");
			AddString(XtraSpreadsheetFunctionNameStringId.Xnpv, "XNPV");
			AddString(XtraSpreadsheetFunctionNameStringId.XOr, "XOR");
			AddString(XtraSpreadsheetFunctionNameStringId.Year, "YEAR");
			AddString(XtraSpreadsheetFunctionNameStringId.YearFrac, "YEARFRAC");
			AddString(XtraSpreadsheetFunctionNameStringId.Yield, "YIELD");
			AddString(XtraSpreadsheetFunctionNameStringId.YieldDisc, "YIELDDISC");
			AddString(XtraSpreadsheetFunctionNameStringId.YieldMat, "YIELDMAT");
			AddString(XtraSpreadsheetFunctionNameStringId.ZDotTest, "Z.TEST");
			AddString(XtraSpreadsheetFunctionNameStringId.ZTest, "ZTEST");
			AddString(XtraSpreadsheetFunctionNameStringId.Call, "CALL");
			AddString(XtraSpreadsheetFunctionNameStringId.RegisterId, "REGISTER.ID");
			AddString(XtraSpreadsheetFunctionNameStringId.Field, "FIELD");
			AddString(XtraSpreadsheetFunctionNameStringId.Range, "RANGE");
			AddString(XtraSpreadsheetFunctionNameStringId.FieldPicture, "FIELDPICTURE");
			AddString(XtraSpreadsheetFunctionNameStringId.Parameter, "PARAMETER");
			AddString(XtraSpreadsheetFunctionNameStringId.AddressInfo, "address");
			AddString(XtraSpreadsheetFunctionNameStringId.ColumnInfo, "col");
			AddString(XtraSpreadsheetFunctionNameStringId.ColorInfo, "color");
			AddString(XtraSpreadsheetFunctionNameStringId.ContentsInfo, "contents");
			AddString(XtraSpreadsheetFunctionNameStringId.FilenameInfo, "filename");
			AddString(XtraSpreadsheetFunctionNameStringId.FormatInfo, "format");
			AddString(XtraSpreadsheetFunctionNameStringId.ParenthesesInfo, "parentheses");
			AddString(XtraSpreadsheetFunctionNameStringId.PrefixInfo, "prefix");
			AddString(XtraSpreadsheetFunctionNameStringId.ProtectInfo, "protect");
			AddString(XtraSpreadsheetFunctionNameStringId.RowInfo, "row");
			AddString(XtraSpreadsheetFunctionNameStringId.TypeInfo, "type");
			AddString(XtraSpreadsheetFunctionNameStringId.WidthInfo, "width");
			AddString(XtraSpreadsheetFunctionNameStringId.DirectoryInfo, "directory");
			AddString(XtraSpreadsheetFunctionNameStringId.NumFileInfo, "numfile");
			AddString(XtraSpreadsheetFunctionNameStringId.OriginInfo, "origin");
			AddString(XtraSpreadsheetFunctionNameStringId.OsVersionInfo, "osversion");
			AddString(XtraSpreadsheetFunctionNameStringId.RecalcInfo, "recalc");
			AddString(XtraSpreadsheetFunctionNameStringId.ReleaseInfo, "release");
			AddString(XtraSpreadsheetFunctionNameStringId.SystemInfo, "system");
			AddString(XtraSpreadsheetFunctionNameStringId.MemAvailInfo, "memavail");
			AddString(XtraSpreadsheetFunctionNameStringId.TotMemInfo, "totmem");
			AddString(XtraSpreadsheetFunctionNameStringId.MemUsedInfo, "memused");
			AddString(XtraSpreadsheetFunctionNameStringId.AutoCalcMode, "Automatic");
			AddString(XtraSpreadsheetFunctionNameStringId.ManualCalcMode, "Manual");
		}
		#endregion
		public static XtraLocalizer<XtraSpreadsheetFunctionNameStringId> CreateDefaultLocalizer() {
			return new XtraSpreadsheetFunctionNameResLocalizer();
		}
		public static string GetString(XtraSpreadsheetFunctionNameStringId id) {
			return Active.GetLocalizedString(id);
		}
		public override XtraLocalizer<XtraSpreadsheetFunctionNameStringId> CreateResXLocalizer() {
			return new XtraSpreadsheetFunctionNameResLocalizer();
		}
		protected override void AddString(XtraSpreadsheetFunctionNameStringId id, string str) {
			Dictionary<XtraSpreadsheetFunctionNameStringId, string> table = XtraLocalizierHelper<XtraSpreadsheetFunctionNameStringId>.GetStringTable(this);
			table[id] = str;
		}
	}
	#endregion
	#region XtraSpreadsheetFunctionNameResLocalizer
	public class XtraSpreadsheetFunctionNameResLocalizer : XtraResXLocalizer<XtraSpreadsheetFunctionNameStringId> {
		static XtraSpreadsheetFunctionNameResLocalizer() {
			SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<XtraSpreadsheetFunctionNameStringId>(CreateDefaultLocalizer()));
		}
		public XtraSpreadsheetFunctionNameResLocalizer()
			: base(new XtraSpreadsheetFunctionNameLocalizer()) {
		}
		public static XtraLocalizer<XtraSpreadsheetFunctionNameStringId> CreateDefaultLocalizer() {
			return new XtraSpreadsheetFunctionNameResLocalizer();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new ResourceManager Manager {
			get { return base.Manager; }
		}
		protected override ResourceManager CreateResourceManagerCore() {
#if DXPORTABLE
			return new ResourceManager("DevExpress.Spreadsheet.Core.LocalizationFunctionNamesRes", typeof(XtraSpreadsheetFunctionNameResLocalizer).GetAssembly());
#else
			return new ResourceManager("DevExpress.XtraSpreadsheet.LocalizationFunctionNamesRes", typeof(XtraSpreadsheetFunctionNameResLocalizer).GetAssembly());
#endif
		}
#if DXPORTABLE
		public static string GetString(XtraSpreadsheetFunctionNameStringId id, CultureInfo culture) {
			return XtraResXLocalizer<XtraSpreadsheetFunctionNameStringId>.GetLocalizedStringFromResources<XtraSpreadsheetFunctionNameStringId, XtraSpreadsheetFunctionNameResLocalizer>(
				id,
				culture,
				() => Active as XtraSpreadsheetFunctionNameResLocalizer,
				(stringId) => XtraSpreadsheetFunctionNameLocalizer.GetString(stringId)
			);
		}
#else
		[ThreadStatic] static CultureInfo lastCulture;
		[ThreadStatic] static ResourceSet lastSet;
		public static string GetString(XtraSpreadsheetFunctionNameStringId id, CultureInfo culture) {
			if (culture == lastCulture)
				return GetString(id, lastSet);
			XtraSpreadsheetFunctionNameLocalizer.GetString(id); 
			lastCulture = culture;
			lastSet = null;
			XtraSpreadsheetFunctionNameResLocalizer localizer = XtraSpreadsheetFunctionNameLocalizer.Active as XtraSpreadsheetFunctionNameResLocalizer;
			if (localizer == null)
				return XtraSpreadsheetFunctionNameLocalizer.GetString(id);
			lastSet = localizer.Manager.GetResourceSet(culture, true, true);
			return GetString(id, lastSet);
		}
		static string GetString(XtraSpreadsheetFunctionNameStringId id, ResourceSet set) {
			if (set == null)
				return XtraSpreadsheetFunctionNameLocalizer.GetString(id);
			string resStr = String.Format("{0}.{1}", typeof(XtraSpreadsheetFunctionNameStringId).Name, id.ToString());
			string result = set.GetString(resStr);
			if (!String.IsNullOrEmpty(result))
				return result;
			return XtraSpreadsheetFunctionNameLocalizer.GetString(id);
		}
#endif
	}
#endregion
}
