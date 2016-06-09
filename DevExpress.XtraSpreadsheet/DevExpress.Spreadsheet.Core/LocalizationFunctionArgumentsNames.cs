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
	#region XtraSpreadsheetFunctionArgumentNameStringId
	public enum XtraSpreadsheetFunctionArgumentNameStringId {
		CountValue1 = 0,
		CountValue2 = 1,
		IfLogicaltest = 1000,
		IfValueiftrue = 1001,
		IfValueiffalse = 1002,
		IsNAValue = 2000,
		IsErrorValue = 3000,
		SumNumber1 = 4000,
		SumNumber2 = 4001,
		AverageNumber1 = 5000,
		AverageNumber2 = 5001,
		MinNumber1 = 6000,
		MinNumber2 = 6001,
		MaxNumber1 = 7000,
		MaxNumber2 = 7001,
		RowReference = 8000,
		ColumnReference = 9000,
		NpvRate = 11000,
		NpvValue1 = 11001,
		NpvValue2 = 11002,
		StDevNumber1 = 12000,
		StDevNumber2 = 12001,
		DollarNumber = 13000,
		DollarDecimals = 13001,
		FixedNumber = 14000,
		FixedDecimals = 14001,
		FixedNocommas = 14002,
		SinNumber = 15000,
		CosNumber = 16000,
		TanNumber = 17000,
		ATanNumber = 18000,
		SqrtNumber = 20000,
		ExpNumber = 21000,
		LnNumber = 22000,
		Log10Number = 23000,
		AbsNumber = 24000,
		IntNumber = 25000,
		SignNumber = 26000,
		RoundNumber = 27000,
		RoundNumdigits = 27001,
		LookupLookupvalue = 28000,
		LookupLookupvector = 28001,
		LookupResultvector = 28002,
		IndexArray = 29000,
		IndexRownum = 29001,
		IndexColumnnum = 29002,
		ReptText = 30000,
		ReptNumbertimes = 30001,
		MidText = 31000,
		MidStartnum = 31001,
		MidNumchars = 31002,
		LenText = 32000,
		ValueText = 33000,
		AndLogical1 = 36000,
		AndLogical2 = 36001,
		OrLogical1 = 37000,
		OrLogical2 = 37001,
		NotLogical = 38000,
		ModNumber = 39000,
		ModDivisor = 39001,
		DCountDatabase = 40000,
		DCountField = 40001,
		DCountCriteria = 40002,
		DSumDatabase = 41000,
		DSumField = 41001,
		DSumCriteria = 41002,
		DAverageDatabase = 42000,
		DAverageField = 42001,
		DAverageCriteria = 42002,
		DMinDatabase = 43000,
		DMinField = 43001,
		DMinCriteria = 43002,
		DMaxDatabase = 44000,
		DMaxField = 44001,
		DMaxCriteria = 44002,
		DStDevDatabase = 45000,
		DStDevField = 45001,
		DStDevCriteria = 45002,
		VarNumber1 = 46000,
		VarNumber2 = 46001,
		DVarDatabase = 47000,
		DVarField = 47001,
		DVarCriteria = 47002,
		TextValue = 48000,
		TextFormattext = 48001,
		LinestKnownys = 49000,
		LinestKnownxs = 49001,
		LinestConst = 49002,
		LinestStats = 49003,
		TrendKnownys = 50000,
		TrendKnownxs = 50001,
		TrendNewxs = 50002,
		TrendConst = 50003,
		LogestKnownys = 51000,
		LogestKnownxs = 51001,
		LogestConst = 51002,
		LogestStats = 51003,
		GrowthKnownys = 52000,
		GrowthKnownxs = 52001,
		GrowthNewxs = 52002,
		GrowthConst = 52003,
		PvRate = 56000,
		PvNper = 56001,
		PvPmt = 56002,
		PvFv = 56003,
		PvType = 56004,
		FvRate = 57000,
		FvNper = 57001,
		FvPmt = 57002,
		FvPv = 57003,
		FvType = 57004,
		NPerRate = 58000,
		NPerPmt = 58001,
		NPerPv = 58002,
		NPerFv = 58003,
		NPerType = 58004,
		PmtRate = 59000,
		PmtNper = 59001,
		PmtPv = 59002,
		PmtFv = 59003,
		PmtType = 59004,
		RateNper = 60000,
		RatePmt = 60001,
		RatePv = 60002,
		RateFv = 60003,
		RateType = 60004,
		RateGuess = 60005,
		MirrValues = 61000,
		MirrFinancerate = 61001,
		MirrReinvestrate = 61002,
		IrrValues = 62000,
		IrrGuess = 62001,
		MatchLookupvalue = 64000,
		MatchLookuparray = 64001,
		MatchMatchtype = 64002,
		DateYear = 65000,
		DateMonth = 65001,
		DateDay = 65002,
		TimeHour = 66000,
		TimeMinute = 66001,
		TimeSecond = 66002,
		DaySerialnumber = 67000,
		MonthSerialnumber = 68000,
		YearSerialnumber = 69000,
		WeekDaySerialnumber = 70000,
		WeekDayReturntype = 70001,
		HourSerialnumber = 71000,
		MinuteSerialnumber = 72000,
		SecondSerialnumber = 73000,
		AreasReference = 75000,
		RowsArray = 76000,
		ColumnsArray = 77000,
		OffsetReference = 78000,
		OffsetRows = 78001,
		OffsetCols = 78002,
		OffsetHeight = 78003,
		OffsetWidth = 78004,
		SearchFindtext = 82000,
		SearchWithintext = 82001,
		SearchStartnum = 82002,
		TransposeArray = 83000,
		TypeValue = 86000,
		ATan2Xnum = 97000,
		ATan2Ynum = 97001,
		ASinNumber = 98000,
		ACosNumber = 99000,
		ChooseIndexnum = 100000,
		ChooseValue1 = 100001,
		ChooseValue2 = 100002,
		HLookupLookupvalue = 101000,
		HLookupTablearray = 101001,
		HLookupRowindexnum = 101002,
		HLookupRangelookup = 101003,
		VLookupLookupvalue = 102000,
		VLookupTablearray = 102001,
		VLookupColindexnum = 102002,
		VLookupRangelookup = 102003,
		IsRefValue = 105000,
		LogNumber = 109000,
		LogBase = 109001,
		CharNumber = 111000,
		LowerText = 112000,
		UpperText = 113000,
		ProperText = 114000,
		LeftText = 115000,
		LeftNumchars = 115001,
		RightText = 116000,
		RightNumchars = 116001,
		ExactText1 = 117000,
		ExactText2 = 117001,
		TrimText = 118000,
		ReplaceOldtext = 119000,
		ReplaceStartnum = 119001,
		ReplaceNumchars = 119002,
		ReplaceNewtext = 119003,
		SubstituteText = 120000,
		SubstituteOldtext = 120001,
		SubstituteNewtext = 120002,
		SubstituteInstancenum = 120003,
		CodeText = 121000,
		FindFindtext = 124000,
		FindWithintext = 124001,
		FindStartnum = 124002,
		CellInfotype = 125000,
		CellReference = 125001,
		IsErrValue = 126000,
		IsTextValue = 127000,
		IsNumberValue = 128000,
		IsBlankValue = 129000,
		TValue = 130000,
		NValue = 131000,
		DateValueDatetext = 140000,
		TimeValueTimetext = 141000,
		SlnCost = 142000,
		SlnSalvage = 142001,
		SlnLife = 142002,
		SydCost = 143000,
		SydSalvage = 143001,
		SydLife = 143002,
		SydPer = 143003,
		DdbCost = 144000,
		DdbSalvage = 144001,
		DdbLife = 144002,
		DdbPeriod = 144003,
		DdbFactor = 144004,
		IndirectReftext = 148000,
		IndirectA1 = 148001,
		CallRegisterid = 150000,
		CallArgument1 = 150001,
		CleanText = 162000,
		MDetermArray = 163000,
		MInverseArray = 164000,
		MMultArray1 = 165000,
		MMultArray2 = 165001,
		IpmtRate = 167000,
		IpmtPer = 167001,
		IpmtNper = 167002,
		IpmtPv = 167003,
		IpmtFv = 167004,
		IpmtType = 167005,
		PpmtRate = 168000,
		PpmtPer = 168001,
		PpmtNper = 168002,
		PpmtPv = 168003,
		PpmtFv = 168004,
		PpmtType = 168005,
		CountAValue1 = 169000,
		CountAValue2 = 169001,
		ProductNumber1 = 183000,
		ProductNumber2 = 183001,
		FactNumber = 184000,
		DProductDatabase = 189000,
		DProductField = 189001,
		DProductCriteria = 189002,
		IsNonTextValue = 190000,
		StDevPNumber1 = 193000,
		StDevPNumber2 = 193001,
		VarPNumber1 = 194000,
		VarPNumber2 = 194001,
		DStDevPDatabase = 195000,
		DStDevPField = 195001,
		DStDevPCriteria = 195002,
		DVarPDatabase = 196000,
		DVarPField = 196001,
		DVarPCriteria = 196002,
		TruncNumber = 197000,
		TruncNumdigits = 197001,
		IsLogicalValue = 198000,
		DCountADatabase = 199000,
		DCountAField = 199001,
		DCountACriteria = 199002,
		FindBFindtext = 205000,
		FindBWithintext = 205001,
		FindBStartnum = 205002,
		SearchBFindtext = 206000,
		SearchBWithintext = 206001,
		SearchBStartnum = 206002,
		ReplaceBOldtext = 207000,
		ReplaceBStartnum = 207001,
		ReplaceBNumbytes = 207002,
		ReplaceBNewtext = 207003,
		LeftBText = 208000,
		LeftBNumbytes = 208001,
		RightBText = 209000,
		RightBNumbytes = 209001,
		MidBText = 210000,
		MidBStartnum = 210001,
		MidBNumbytes = 210002,
		LenBText = 211000,
		RoundUpNumber = 212000,
		RoundUpNumdigits = 212001,
		RoundDownNumber = 213000,
		RoundDownNumdigits = 213001,
		AscText = 214000,
		RankNumber = 216000,
		RankRef = 216001,
		RankOrder = 216002,
		AddressRownum = 219000,
		AddressColumnnum = 219001,
		AddressAbsnum = 219002,
		AddressA1 = 219003,
		AddressSheettext = 219004,
		Days360Startdate = 220000,
		Days360Enddate = 220001,
		Days360Method = 220002,
		VdbCost = 222000,
		VdbSalvage = 222001,
		VdbLife = 222002,
		VdbStartperiod = 222003,
		VdbEndperiod = 222004,
		VdbFactor = 222005,
		VdbNoswitch = 222006,
		MedianNumber1 = 227000,
		MedianNumber2 = 227001,
		SumProductArray1 = 228000,
		SumProductArray2 = 228001,
		SumProductArray3 = 228002,
		SinHNumber = 229000,
		CosHNumber = 230000,
		TanHNumber = 231000,
		ASinHNumber = 232000,
		ACosHNumber = 233000,
		ATanHNumber = 234000,
		DGetDatabase = 235000,
		DGetField = 235001,
		DGetCriteria = 235002,
		InfoTypetext = 244000,
		DbCost = 247000,
		DbSalvage = 247001,
		DbLife = 247002,
		DbPeriod = 247003,
		DbMonth = 247004,
		FrequencyDataarray = 252000,
		FrequencyBinsarray = 252001,
		ErrorTypeErrorval = 261000,
		RegisterIdModuletext = 267000,
		RegisterIdProcedure = 267001,
		RegisterIdTypetext = 267002,
		AvedevNumber1 = 269000,
		AvedevNumber2 = 269001,
		BetaDistCompatibilityX = 270000,
		BetaDistCompatibilityAlpha = 270001,
		BetaDistCompatibilityBeta = 270002,
		BetaDistCompatibilityA = 270003,
		BetaDistCompatibilityB = 270004,
		GammaLnX = 271000,
		BetaInvProbability = 272000,
		BetaInvAlpha = 272001,
		BetaInvBeta = 272002,
		BetaInvA = 272003,
		BetaInvB = 272004,
		BinomDistNumbers = 273000,
		BinomDistTrials = 273001,
		BinomDistProbabilitys = 273002,
		BinomDistCumulative = 273003,
		ChiDistX = 274000,
		ChiDistDegfreedom = 274001,
		ChiInvProbability = 275000,
		ChiInvDegfreedom = 275001,
		CombinNumber = 276000,
		CombinNumberchosen = 276001,
		ConfidenceAlpha = 277000,
		ConfidenceStandarddev = 277001,
		ConfidenceSize = 277002,
		CritBinomTrials = 278000,
		CritBinomProbabilitys = 278001,
		CritBinomAlpha = 278002,
		EvenNumber = 279000,
		ExponDistX = 280000,
		ExponDistLambda = 280001,
		ExponDistCumulative = 280002,
		FDistX = 281000,
		FDistDegfreedom1 = 281001,
		FDistDegfreedom2 = 281002,
		FInvProbability = 282000,
		FInvDegfreedom1 = 282001,
		FInvDegfreedom2 = 282002,
		FisherX = 283000,
		FisherInvY = 284000,
		FloorNumber = 285000,
		FloorSignificance = 285001,
		GammaDistX = 286000,
		GammaDistAlpha = 286001,
		GammaDistBeta = 286002,
		GammaDistCumulative = 286003,
		GammaInvProbability = 287000,
		GammaInvAlpha = 287001,
		GammaInvBeta = 287002,
		CeilingNumber = 288000,
		CeilingSignificance = 288001,
		HypGeomDistSamples = 289000,
		HypGeomDistNumbersample = 289001,
		HypGeomDistPopulations = 289002,
		HypGeomDistNumberpop = 289003,
		LogNormDistCompatibilityX = 290000,
		LogNormDistCompatibilityMean = 290001,
		LogNormDistCompatibilityStandarddev = 290002,
		LogInvCompatibilityProbability = 291000,
		LogInvCompatibilityMean = 291001,
		LogInvCompatibilityStandarddev = 291002,
		NegBinomDistNumberf = 292000,
		NegBinomDistNumbers = 292001,
		NegBinomDistProbabilitys = 292002,
		NormDistCompatibilityX = 293000,
		NormDistCompatibilityMean = 293001,
		NormDistCompatibilityStandarddev = 293002,
		NormDistCompatibilityCumulative = 293003,
		NormSDistCompatibilityZ = 294000,
		NormInvCompatibilityProbability = 295000,
		NormInvCompatibilityMean = 295001,
		NormInvCompatibilityStandarddev = 295002,
		NormSInvCompatibilityProbability = 296000,
		StandardizeX = 297000,
		StandardizeMean = 297001,
		StandardizeStandarddev = 297002,
		OddNumber = 298000,
		PermutNumber = 299000,
		PermutNumberchosen = 299001,
		PoissonX = 300000,
		PoissonMean = 300001,
		PoissonCumulative = 300002,
		TDistX = 301000,
		TDistDegfreedom = 301001,
		TDistTails = 301002,
		WeibullX = 302000,
		WeibullAlpha = 302001,
		WeibullBeta = 302002,
		WeibullCumulative = 302003,
		SumXMY2Arrayx = 303000,
		SumXMY2Arrayy = 303001,
		SumX2MY2Arrayx = 304000,
		SumX2MY2Arrayy = 304001,
		SumX2PY2Arrayx = 305000,
		SumX2PY2Arrayy = 305001,
		ChiTestActualrange = 306000,
		ChiTestExpectedrange = 306001,
		CorrelArray1 = 307000,
		CorrelArray2 = 307001,
		CovarArray1 = 308000,
		CovarArray2 = 308001,
		ForecastX = 309000,
		ForecastKnownys = 309001,
		ForecastKnownxs = 309002,
		FTestArray1 = 310000,
		FTestArray2 = 310001,
		InterceptKnownys = 311000,
		InterceptKnownxs = 311001,
		PearsonArray1 = 312000,
		PearsonArray2 = 312001,
		RsqKnownys = 313000,
		RsqKnownxs = 313001,
		StEYXKnownys = 314000,
		StEYXKnownxs = 314001,
		SlopeKnownys = 315000,
		SlopeKnownxs = 315001,
		TTestArray1 = 316000,
		TTestArray2 = 316001,
		TTestTails = 316002,
		TTestType = 316003,
		ProbXrange = 317000,
		ProbProbrange = 317001,
		ProbLowerlimit = 317002,
		ProbUpperlimit = 317003,
		DevSqNumber1 = 318000,
		DevSqNumber2 = 318001,
		GeomeanNumber1 = 319000,
		GeomeanNumber2 = 319001,
		HarMeanNumber1 = 320000,
		HarMeanNumber2 = 320001,
		SumSqNumber1 = 321000,
		SumSqNumber2 = 321001,
		KurtNumber1 = 322000,
		KurtNumber2 = 322001,
		SkewNumber1 = 323000,
		SkewNumber2 = 323001,
		ZTestArray = 324000,
		ZTestX = 324001,
		ZTestSigma = 324002,
		LargeArray = 325000,
		LargeK = 325001,
		SmallArray = 326000,
		SmallK = 326001,
		QuartileArray = 327000,
		QuartileQuart = 327001,
		PercentileArray = 328000,
		PercentileK = 328001,
		PercentRankArray = 329000,
		PercentRankX = 329001,
		PercentRankSignificance = 329002,
		ModeNumber1 = 330000,
		ModeNumber2 = 330001,
		TrimmeanArray = 331000,
		TrimmeanPercent = 331001,
		TinvProbability = 332000,
		TinvDegfreedom = 332001,
		ConcatenateText1 = 336000,
		ConcatenateText2 = 336001,
		PowerNumber = 337000,
		PowerPower = 337001,
		RadiansAngle = 342000,
		DegreesAngle = 343000,
		SubtotalFunctionnum = 344000,
		SubtotalRef1 = 344001,
		SumIfRange = 345000,
		SumIfCriteria = 345001,
		SumIfSumrange = 345002,
		CountIfRange = 346000,
		CountIfCriteria = 346001,
		CountBlankRange = 347000,
		IsPmtRate = 350000,
		IsPmtPer = 350001,
		IsPmtNper = 350002,
		IsPmtPv = 350003,
		RomanNumber = 354000,
		RomanForm = 354001,
		GetPivotDataDatafield = 358000,
		GetPivotDataPivottable = 358001,
		GetPivotDataField = 358002,
		GetPivotDataItem = 358003,
		HyperlinkLinklocation = 359000,
		HyperlinkFriendlyname = 359001,
		PhoneticReference = 360000,
		AverageAValue1 = 361000,
		AverageAValue2 = 361001,
		MaxAValue1 = 362000,
		MaxAValue2 = 362001,
		MinAValue1 = 363000,
		MinAValue2 = 363001,
		StDevPAValue1 = 364000,
		StDevPAValue2 = 364001,
		VarPAValue1 = 365000,
		VarPAValue2 = 365001,
		StDevAValue1 = 366000,
		StDevAValue2 = 366001,
		VarAValue1 = 367000,
		VarAValue2 = 367001,
		BahtTextNumber = 368000,
		RtdProgID = 379000,
		RtdServer = 379001,
		RtdTopic1 = 379002,
		RtdTopic2 = 379003,
		CubeValueConnection = 380000,
		CubeValueMemberexpression1 = 380001,
		CubeMemberConnection = 381000,
		CubeMemberMemberexpression = 381001,
		CubeMemberCaption = 381002,
		CubeMemberPropertyConnection = 382000,
		CubeMemberPropertyMemberexpression = 382001,
		CubeMemberPropertyProperty = 382002,
		CubeRankedMemberConnection = 383000,
		CubeRankedMemberSetexpression = 383001,
		CubeRankedMemberRank = 383002,
		CubeRankedMemberCaption = 383003,
		Hex2BinNumber = 384000,
		Hex2BinPlaces = 384001,
		Hex2DecNumber = 385000,
		Hex2OctNumber = 386000,
		Hex2OctPlaces = 386001,
		Dec2BinNumber = 387000,
		Dec2BinPlaces = 387001,
		Dec2HexNumber = 388000,
		Dec2HexPlaces = 388001,
		Dec2OctNumber = 389000,
		Dec2OctPlaces = 389001,
		Oct2BinNumber = 390000,
		Oct2BinPlaces = 390001,
		Oct2HexNumber = 391000,
		Oct2HexPlaces = 391001,
		Oct2DecNumber = 392000,
		Bin2DecNumber = 393000,
		Bin2OctNumber = 394000,
		Bin2OctPlaces = 394001,
		Bin2HexNumber = 395000,
		Bin2HexPlaces = 395001,
		ImSubInumber1 = 396000,
		ImSubInumber2 = 396001,
		ImDivInumber1 = 397000,
		ImDivInumber2 = 397001,
		ImPowerInumber = 398000,
		ImPowerNumber = 398001,
		ImAbsInumber = 399000,
		ImSqrtInumber = 400000,
		ImLnInumber = 401000,
		ImLog2Inumber = 402000,
		ImLog10Inumber = 403000,
		ImSinInumber = 404000,
		ImCosInumber = 405000,
		ImExpInumber = 406000,
		ImArgumentInumber = 407000,
		ImConjugateInumber = 408000,
		ImaginaryInumber = 409000,
		ImRealInumber = 410000,
		ComplexRealnum = 411000,
		ComplexInum = 411001,
		ComplexSuffix = 411002,
		ImSumInumber1 = 412000,
		ImSumInumber2 = 412001,
		ImProductInumber1 = 413000,
		ImProductInumber2 = 413001,
		SeriessumX = 414000,
		SeriessumN = 414001,
		SeriessumM = 414002,
		SeriessumCoefficients = 414003,
		FactDoubleNumber = 415000,
		SqrtPiNumber = 416000,
		QuotientNumerator = 417000,
		QuotientDenominator = 417001,
		DeltaNumber1 = 418000,
		DeltaNumber2 = 418001,
		GestepNumber = 419000,
		GestepStep = 419001,
		IsEvenNumber = 420000,
		IsOddNumber = 421000,
		MRoundNumber = 422000,
		MRoundMultiple = 422001,
		ErfLowerlimit = 423000,
		ErfUpperlimit = 423001,
		ErfcX = 424000,
		BesselJX = 425000,
		BesselJN = 425001,
		BesselKX = 426000,
		BesselKN = 426001,
		BesselYX = 427000,
		BesselYN = 427001,
		BesselIX = 428000,
		BesselIN = 428001,
		XirrValues = 429000,
		XirrDates = 429001,
		XirrGuess = 429002,
		XnpvRate = 430000,
		XnpvValues = 430001,
		XnpvDates = 430002,
		PriceMatSettlement = 431000,
		PriceMatMaturity = 431001,
		PriceMatIssue = 431002,
		PriceMatRate = 431003,
		PriceMatYld = 431004,
		PriceMatBasis = 431005,
		YieldMatSettlement = 432000,
		YieldMatMaturity = 432001,
		YieldMatIssue = 432002,
		YieldMatRate = 432003,
		YieldMatPr = 432004,
		YieldMatBasis = 432005,
		IntrateSettlement = 433000,
		IntrateMaturity = 433001,
		IntrateInvestment = 433002,
		IntrateRedemption = 433003,
		IntrateBasis = 433004,
		ReceivedSettlement = 434000,
		ReceivedMaturity = 434001,
		ReceivedInvestment = 434002,
		ReceivedDiscount = 434003,
		ReceivedBasis = 434004,
		DiscSettlement = 435000,
		DiscMaturity = 435001,
		DiscPr = 435002,
		DiscRedemption = 435003,
		DiscBasis = 435004,
		PriceDiscSettlement = 436000,
		PriceDiscMaturity = 436001,
		PriceDiscDiscount = 436002,
		PriceDiscRedemption = 436003,
		PriceDiscBasis = 436004,
		YieldDiscSettlement = 437000,
		YieldDiscMaturity = 437001,
		YieldDiscPr = 437002,
		YieldDiscRedemption = 437003,
		YieldDiscBasis = 437004,
		TbillEqSettlement = 438000,
		TbillEqMaturity = 438001,
		TbillEqDiscount = 438002,
		TbillPriceSettlement = 439000,
		TbillPriceMaturity = 439001,
		TbillPriceDiscount = 439002,
		TbillYieldSettlement = 440000,
		TbillYieldMaturity = 440001,
		TbillYieldPr = 440002,
		PriceSettlement = 441000,
		PriceMaturity = 441001,
		PriceRate = 441002,
		PriceYld = 441003,
		PriceRedemption = 441004,
		PriceFrequency = 441005,
		PriceBasis = 441006,
		YieldSettlement = 442000,
		YieldMaturity = 442001,
		YieldRate = 442002,
		YieldPr = 442003,
		YieldRedemption = 442004,
		YieldFrequency = 442005,
		YieldBasis = 442006,
		DollarDeFractionaldollar = 443000,
		DollarDeFraction = 443001,
		DollarFrDecimaldollar = 444000,
		DollarFrFraction = 444001,
		NominalEffectrate = 445000,
		NominalNpery = 445001,
		EffectNominalrate = 446000,
		EffectNpery = 446001,
		CumPrincRate = 447000,
		CumPrincNper = 447001,
		CumPrincPv = 447002,
		CumPrincStartperiod = 447003,
		CumPrincEndperiod = 447004,
		CumPrincType = 447005,
		CumIpmtRate = 448000,
		CumIpmtNper = 448001,
		CumIpmtPv = 448002,
		CumIpmtStartperiod = 448003,
		CumIpmtEndperiod = 448004,
		CumIpmtType = 448005,
		EDateStartdate = 449000,
		EDateMonths = 449001,
		EOMonthStartdate = 450000,
		EOMonthMonths = 450001,
		YearFracStartdate = 451000,
		YearFracEnddate = 451001,
		YearFracBasis = 451002,
		CoupDaybsSettlement = 452000,
		CoupDaybsMaturity = 452001,
		CoupDaybsFrequency = 452002,
		CoupDaybsBasis = 452003,
		CoupDaysSettlement = 453000,
		CoupDaysMaturity = 453001,
		CoupDaysFrequency = 453002,
		CoupDaysBasis = 453003,
		CoupDaysncSettlement = 454000,
		CoupDaysncMaturity = 454001,
		CoupDaysncFrequency = 454002,
		CoupDaysncBasis = 454003,
		CoupncdSettlement = 455000,
		CoupncdMaturity = 455001,
		CoupncdFrequency = 455002,
		CoupncdBasis = 455003,
		CoupNumSettlement = 456000,
		CoupNumMaturity = 456001,
		CoupNumFrequency = 456002,
		CoupNumBasis = 456003,
		CouppcdSettlement = 457000,
		CouppcdMaturity = 457001,
		CouppcdFrequency = 457002,
		CouppcdBasis = 457003,
		DurationSettlement = 458000,
		DurationMaturity = 458001,
		DurationCoupon = 458002,
		DurationYld = 458003,
		DurationFrequency = 458004,
		DurationBasis = 458005,
		MDurationSettlement = 459000,
		MDurationMaturity = 459001,
		MDurationCoupon = 459002,
		MDurationYld = 459003,
		MDurationFrequency = 459004,
		MDurationBasis = 459005,
		OddLPriceSettlement = 460000,
		OddLPriceMaturity = 460001,
		OddLPriceLastinterest = 460002,
		OddLPriceRate = 460003,
		OddLPriceYld = 460004,
		OddLPriceRedemption = 460005,
		OddLPriceFrequency = 460006,
		OddLPriceBasis = 460007,
		OddLYieldSettlement = 461000,
		OddLYieldMaturity = 461001,
		OddLYieldLastinterest = 461002,
		OddLYieldRate = 461003,
		OddLYieldPr = 461004,
		OddLYieldRedemption = 461005,
		OddLYieldFrequency = 461006,
		OddLYieldBasis = 461007,
		OddFPriceSettlement = 462000,
		OddFPriceMaturity = 462001,
		OddFPriceIssue = 462002,
		OddFPriceFirstcoupon = 462003,
		OddFPriceRate = 462004,
		OddFPriceYld = 462005,
		OddFPriceRedemption = 462006,
		OddFPriceFrequency = 462007,
		OddFPriceBasis = 462008,
		OddFYieldSettlement = 463000,
		OddFYieldMaturity = 463001,
		OddFYieldIssue = 463002,
		OddFYieldFirstcoupon = 463003,
		OddFYieldRate = 463004,
		OddFYieldPr = 463005,
		OddFYieldRedemption = 463006,
		OddFYieldFrequency = 463007,
		OddFYieldBasis = 463008,
		RandBetweenBottom = 464000,
		RandBetweenTop = 464001,
		WeekNumSerialnumber = 465000,
		WeekNumReturntype = 465001,
		AmordegrcCost = 466000,
		AmordegrcDatepurchased = 466001,
		AmordegrcFirstperiod = 466002,
		AmordegrcSalvage = 466003,
		AmordegrcPeriod = 466004,
		AmordegrcRate = 466005,
		AmordegrcBasis = 466006,
		AmorlincCost = 467000,
		AmorlincDatepurchased = 467001,
		AmorlincFirstperiod = 467002,
		AmorlincSalvage = 467003,
		AmorlincPeriod = 467004,
		AmorlincRate = 467005,
		AmorlincBasis = 467006,
		ConvertNumber = 468000,
		ConvertFromunit = 468001,
		ConvertTounit = 468002,
		AccrintIssue = 469000,
		AccrintFirstinterest = 469001,
		AccrintSettlement = 469002,
		AccrintRate = 469003,
		AccrintPar = 469004,
		AccrintFrequency = 469005,
		AccrintBasis = 469006,
		AccrintCalcmethod = 469007,
		AccrintmIssue = 470000,
		AccrintmSettlement = 470001,
		AccrintmRate = 470002,
		AccrintmPar = 470003,
		AccrintmBasis = 470004,
		WorkDayStartdate = 471000,
		WorkDayDays = 471001,
		WorkDayHolidays = 471002,
		NetworkDaysStartdate = 472000,
		NetworkDaysEnddate = 472001,
		NetworkDaysHolidays = 472002,
		GcdNumber1 = 473000,
		GcdNumber2 = 473001,
		MultinomialNumber1 = 474000,
		MultinomialNumber2 = 474001,
		LcmNumber1 = 475000,
		LcmNumber2 = 475001,
		FvSchedulePrincipal = 476000,
		FvScheduleSchedule = 476001,
		CubeKpiMemberConnection = 477000,
		CubeKpiMemberKpiname = 477001,
		CubeKpiMemberKpiproperty = 477002,
		CubeKpiMemberCaption = 477003,
		CubeSetConnection = 478000,
		CubeSetSetexpression = 478001,
		CubeSetCaption = 478002,
		CubeSetSortorder = 478003,
		CubeSetSortby = 478004,
		CubeSetCountSet = 479000,
		IfErrorValue = 480000,
		IfErrorValueiferror = 480001,
		CountIfsCriteriarange = 481000,
		CountIfsCriteria = 481001,
		SumIfsSumrange = 482000,
		SumIfsCriteriarange = 482001,
		SumIfsCriteria = 482002,
		AverageIfRange = 483000,
		AverageIfCriteria = 483001,
		AverageIfAveragerange = 483002,
		AverageIfsAveragerange = 484000,
		AverageIfsCriteriarange = 484001,
		AverageIfsCriteria = 484002,
		AggregateFunctionnum = 16494000,
		AggregateOptions = 16494001,
		AggregateArray = 16494002,
		AggregateK = 16494003,
		BinomDotDistNumbers = 16495000,
		BinomDotDistTrials = 16495001,
		BinomDotDistProbabilitys = 16495002,
		BinomDotDistCumulative = 16495003,
		BinomDotInvTrials = 16496000,
		BinomDotInvProbabilitys = 16496001,
		BinomDotInvAlpha = 16496002,
		ConfidenceNormAlpha = 16400000,
		ConfidenceNormStandarddev = 16400001,
		ConfidenceNormSize = 16400002,
		ConfidenceDotTAlpha = 16498000,
		ConfidenceDotTStandarddev = 16498001,
		ConfidenceDotTSize = 16498002,
		ChisqDotTestActualrange = 16489000,
		ChisqDotTestExpectedrange = 16489001,
		FDotTestArray1 = 16470000,
		FDotTestArray2 = 16470001,
		CovariancePArray1 = 16461000,
		CovariancePArray2 = 16461001,
		CovarianceSArray1 = 16462000,
		CovarianceSArray2 = 16462001,
		ExponDotDistX = 16500000,
		ExponDotDistLambda = 16500001,
		ExponDotDistCumulative = 16500002,
		GammaDotDistX = 16472000,
		GammaDotDistAlpha = 16472001,
		GammaDotDistBeta = 16472002,
		GammaDotDistCumulative = 16472003,
		GammaDotInvProbability = 16484000,
		GammaDotInvAlpha = 16484001,
		GammaDotInvBeta = 16484002,
		ModeMultNumber1 = 16385000,
		ModeMultNumber2 = 16385001,
		ModeSnglNumber1 = 16386000,
		ModeSnglNumber2 = 16386001,
		NormDistX = 16397000,
		NormDistMean = 16397001,
		NormDistStandarddev = 16397002,
		NormDistCumulative = 16397003,
		NormInvProbability = 16396000,
		NormInvMean = 16396001,
		NormInvStandarddev = 16396002,
		PercentileExcArray = 16457000,
		PercentileExcK = 16457001,
		PercentileIncArray = 16458000,
		PercentileIncK = 16458001,
		PercentRankExcArray = 16455000,
		PercentRankExcX = 16455001,
		PercentRankExcSignificance = 16455002,
		PercentRankIncArray = 16456000,
		PercentRankIncX = 16456001,
		PercentRankIncSignificance = 16456002,
		PoissonDotDistX = 16501000,
		PoissonDotDistMean = 16501001,
		PoissonDotDistCumulative = 16501002,
		QuartileExcArray = 16459000,
		QuartileExcQuart = 16459001,
		QuartileIncArray = 16460000,
		QuartileIncQuart = 16460001,
		RankDotAvgNumber = 16463000,
		RankDotAvgRef = 16463001,
		RankDotAvgOrder = 16463002,
		RankDotEqNumber = 16464000,
		RankDotEqRef = 16464001,
		RankDotEqOrder = 16464002,
		StDevDotSNumber1 = 16389000,
		StDevDotSNumber2 = 16389001,
		StDevDotPNumber1 = 16390000,
		StDevDotPNumber2 = 16390001,
		TDotDistX = 16469000,
		TDotDistDegfreedom = 16469001,
		TDotDistCumulative = 16469002,
		TDotDistDot2tX = 16481000,
		TDotDistDot2tDegfreedom = 16481001,
		TDotDistDotTtX = 16480000,
		TDotDistDotTtDegfreedom = 16480001,
		TDotInvProbability = 16466000,
		TDotInvDegfreedom = 16466001,
		TDotInvDot2tProbability = 16482000,
		TDotInvDot2tDegfreedom = 16482001,
		VarDotSNumber1 = 16388000,
		VarDotSNumber2 = 16388001,
		VarDotPNumber1 = 16387000,
		VarDotPNumber2 = 16387001,
		WeibullDotDistX = 16491000,
		WeibullDotDistAlpha = 16491001,
		WeibullDotDistBeta = 16491002,
		WeibullDotDistCumulative = 16491003,
		NetworkDaysIntlStartdate = 16399000,
		NetworkDaysIntlEnddate = 16399001,
		NetworkDaysIntlWeekend = 16399002,
		NetworkDaysIntlHolidays = 16399003,
		WorkDayIntlStartdate = 16398000,
		WorkDayIntlDays = 16398001,
		WorkDayIntlWeekend = 16398002,
		WorkDayIntlHolidays = 16398003,
		IsoCeilingNumber = 16417000,
		IsoCeilingSignificance = 16417001,
		BetaDistX = 16404000,
		BetaDistAlpha = 16404001,
		BetaDistBeta = 16404002,
		BetaDistCumulative = 16404003,
		BetaDistA = 16404004,
		BetaDistB = 16404005,
		BetaDotInvProbability = 16465000,
		BetaDotInvAlpha = 16465001,
		BetaDotInvBeta = 16465002,
		BetaDotInvA = 16465003,
		BetaDotInvB = 16465004,
		ChisqDotDistX = 16485000,
		ChisqDotDistDegfreedom = 16485001,
		ChisqDotDistCumulative = 16485002,
		ChisqDotDistDotRtX = 16486000,
		ChisqDotDistDotRtDegfreedom = 16486001,
		ChisqDotInvProbability = 16487000,
		ChisqDotInvDegfreedom = 16487001,
		ChisqDotInvDotRtProbability = 16488000,
		ChisqDotInvDotRtDegfreedom = 16488001,
		FDotDistX = 16468000,
		FDotDistDegfreedom1 = 16468001,
		FDotDistDegfreedom2 = 16468002,
		FDotDistCumulative = 16468003,
		FDotDistDotRtX = 16471000,
		FDotDistDotRtDegfreedom1 = 16471001,
		FDotDistDotRtDegfreedom2 = 16471002,
		FDotInvProbability = 16467000,
		FDotInvDegfreedom1 = 16467001,
		FDotInvDegfreedom2 = 16467002,
		FDotinvDotRtProbability = 16483000,
		FDotinvDotRtDegfreedom1 = 16483001,
		FDotinvDotRtDegfreedom2 = 16483002,
		HypgeomDotDistSamples = 16493000,
		HypgeomDotDistNumbersample = 16493001,
		HypgeomDotDistPopulations = 16493002,
		HypgeomDotDistNumberpop = 16493003,
		HypgeomDotDistCumulative = 16493004,
		LogNormDistX = 16393000,
		LogNormDistMean = 16393001,
		LogNormDistStandarddev = 16393002,
		LogNormDistCumulative = 16393003,
		LogNormInvProbability = 16395000,
		LogNormInvMean = 16395001,
		LogNormInvStandarddev = 16395002,
		NegbinomDotDistNumberf = 16492000,
		NegbinomDotDistNumbers = 16492001,
		NegbinomDotDistProbabilitys = 16492002,
		NegbinomDotDistCumulative = 16492003,
		NormSDistZ = 16392000,
		NormSDistCumulative = 16392001,
		NormSInvProbability = 16394000,
		TDotTestArray1 = 16473000,
		TDotTestArray2 = 16473001,
		TDotTestTails = 16473002,
		TDotTestType = 16473003,
		ZDotTestArray = 16490000,
		ZDotTestX = 16490001,
		ZDotTestSigma = 16490002,
		ErfPreciseX = 16401000,
		ErfcPreciseX = 16402000,
		GammaLnPreciseX = 16403000,
		CeilingDotPreciseNumber = 16497000,
		CeilingDotPreciseSignificance = 16497001,
		FloorDotPreciseNumber = 16499000,
		FloorDotPreciseSignificance = 16499001,
		ACotNumber = 16405000,
		ACotHNumber = 16406000,
		CotNumber = 16411000,
		CotHNumber = 16412000,
		CscNumber = 16413000,
		CscHNumber = 16414000,
		SecNumber = 16419000,
		SecHNumber = 16420000,
		ImTanInumber = 16438000,
		ImCotInumber = 16432000,
		ImCscInumber = 16433000,
		ImCscHInumber = 16434000,
		ImSecInumber = 16435000,
		ImSecHInumber = 16436000,
		BitAndNumber1 = 16426000,
		BitAndNumber2 = 16426001,
		BitOrNumber1 = 16428000,
		BitOrNumber2 = 16428001,
		BitXorNumber1 = 16430000,
		BitXorNumber2 = 16430001,
		BitLShiftNumber = 16427000,
		BitLShiftShiftamount = 16427001,
		BitRShiftNumber = 16429000,
		BitRShiftShiftamount = 16429001,
		PermutationaNumber = 16423000,
		PermutationaNumberchosen = 16423001,
		CombinANumber = 16410000,
		CombinANumberchosen = 16410001,
		XOrLogical1 = 16445000,
		XOrLogical2 = 16445001,
		PDurationRate = 16442000,
		PDurationPv = 16442001,
		PDurationFv = 16442002,
		BaseNumber = 16408000,
		BaseRadix = 16408001,
		BaseMinlength = 16408002,
		DecimalNumber = 16415000,
		DecimalRadix = 16415001,
		DaysEnddate = 16450000,
		DaysStartdate = 16450001,
		BinomDistRangeTrials = 16421000,
		BinomDistRangeProbabilitys = 16421001,
		BinomDistRangeNumbers = 16421002,
		BinomDistRangeNumbers2 = 16421003,
		GammaX = 16422000,
		SkewPNumber1 = 16425000,
		SkewPNumber2 = 16425001,
		PhiX = 16424000,
		RRINper = 16443000,
		RRIPv = 16443001,
		RRIFv = 16443002,
		UnicodeText = 16441000,
		MUnitDimension = 16418000,
		ArabicText = 16407000,
		ISOWeekNumDate = 16451000,
		NumberValueText = 16439000,
		NumberValueDecimalseparator = 16439001,
		NumberValueGroupseparator = 16439002,
		SheetValue = 16448000,
		SheetsReference = 16449000,
		FormulaTextReference = 16446000,
		IsFormulaReference = 16447000,
		IfNAValue = 16444000,
		IfNAValueifna = 16444001,
		CeilingMathNumber = 16409000,
		CeilingMathSignificance = 16409001,
		CeilingMathMode = 16409002,
		FloorMathNumber = 16416000,
		FloorMathSignificance = 16416001,
		FloorMathMode = 16416002,
		ImSinHInumber = 16437000,
		ImCosHInumber = 16431000,
		FunctionFilterXMLXml = 16453000,
		FunctionFilterXMLXpath = 16453001,
		FunctionWebServiceUrl = 16454000,
		EncodeURLText = 16452000,
		FieldDataFieldName = 0x4100*1000+0,
		RangeCellRefenrece = 0x4101*1000+0,
		FieldPictureDataFieldName = 0x4102*1000+0,
		FieldPicturePicturePlacement = 0x4102 * 1000 + 1,
		FieldPictureTargetRange = 0x4102 * 1000 + 2,
		FieldPictureLockAspectRatio = 0x4102 * 1000 + 3,
		FieldPictureOffsetX = 0x4102 * 1000 + 4,
		FieldPictureOffsetY = 0x4102 * 1000 + 5,
		FieldPictureWidth = 0x4102 * 1000 + 6,
		FieldPictureHeight = 0x4102 * 1000 + 7,
		ParameterParameterName = 0x4103*1000+0,
	}
	#endregion
	#region XtraSpreadsheetFunctionArgumentsNamesLocalizer
	public class XtraSpreadsheetFunctionArgumentsNamesLocalizer : XtraLocalizer<XtraSpreadsheetFunctionArgumentNameStringId> {
		static XtraSpreadsheetFunctionArgumentsNamesLocalizer() { SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<XtraSpreadsheetFunctionArgumentNameStringId>(CreateDefaultLocalizer())); }
		#region PopulateStringTable
		protected override void PopulateStringTable() {
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CountValue1, "value1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CountValue2, "value2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.IfLogicaltest, "logical_test");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.IfValueiftrue, "value_if_true");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.IfValueiffalse, "value_if_false");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.IsNAValue, "value");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.IsErrorValue, "value");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SumNumber1, "number1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SumNumber2, "number2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AverageNumber1, "number1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AverageNumber2, "number2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.MinNumber1, "number1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.MinNumber2, "number2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.MaxNumber1, "number1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.MaxNumber2, "number2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.RowReference, "reference");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ColumnReference, "reference");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.NpvRate, "rate");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.NpvValue1, "value1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.NpvValue2, "value2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.StDevNumber1, "number1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.StDevNumber2, "number2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DollarNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DollarDecimals, "decimals");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FixedNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FixedDecimals, "decimals");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FixedNocommas, "no_commas");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SinNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CosNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TanNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ATanNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SqrtNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ExpNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.LnNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.Log10Number, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AbsNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.IntNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SignNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.RoundNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.RoundNumdigits, "num_digits");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.LookupLookupvalue, "lookup_value");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.LookupLookupvector, "lookup_vector");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.LookupResultvector, "result_vector");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.IndexArray, "array");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.IndexRownum, "row_num");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.IndexColumnnum, "column_num");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ReptText, "text");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ReptNumbertimes, "number_times");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.MidText, "text");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.MidStartnum, "start_num");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.MidNumchars, "num_chars");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.LenText, "text");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ValueText, "text");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AndLogical1, "logical1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AndLogical2, "logical2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.OrLogical1, "logical1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.OrLogical2, "logical2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.NotLogical, "logical");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ModNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ModDivisor, "divisor");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DCountDatabase, "database");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DCountField, "field");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DCountCriteria, "criteria");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DSumDatabase, "database");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DSumField, "field");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DSumCriteria, "criteria");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DAverageDatabase, "database");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DAverageField, "field");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DAverageCriteria, "criteria");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DMinDatabase, "database");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DMinField, "field");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DMinCriteria, "criteria");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DMaxDatabase, "database");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DMaxField, "field");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DMaxCriteria, "criteria");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DStDevDatabase, "database");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DStDevField, "field");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DStDevCriteria, "criteria");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.VarNumber1, "number1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.VarNumber2, "number2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DVarDatabase, "database");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DVarField, "field");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DVarCriteria, "criteria");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TextValue, "value");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TextFormattext, "format_text");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.LinestKnownys, "known_y's");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.LinestKnownxs, "known_x's");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.LinestConst, "const");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.LinestStats, "stats");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TrendKnownys, "known_y's");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TrendKnownxs, "known_x's");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TrendNewxs, "new_x's");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TrendConst, "const");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.LogestKnownys, "known_y's");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.LogestKnownxs, "known_x's");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.LogestConst, "const");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.LogestStats, "stats");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.GrowthKnownys, "known_y's");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.GrowthKnownxs, "known_x's");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.GrowthNewxs, "new_x's");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.GrowthConst, "const");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PvRate, "rate");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PvNper, "nper");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PvPmt, "pmt");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PvFv, "fv");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PvType, "type");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FvRate, "rate");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FvNper, "nper");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FvPmt, "pmt");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FvPv, "pv");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FvType, "type");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.NPerRate, "rate");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.NPerPmt, "pmt");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.NPerPv, "pv");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.NPerFv, "fv");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.NPerType, "type");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PmtRate, "rate");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PmtNper, "nper");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PmtPv, "pv");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PmtFv, "fv");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PmtType, "type");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.RateNper, "nper");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.RatePmt, "pmt");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.RatePv, "pv");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.RateFv, "fv");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.RateType, "type");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.RateGuess, "guess");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.MirrValues, "values");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.MirrFinancerate, "finance_rate");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.MirrReinvestrate, "reinvest_rate");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.IrrValues, "values");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.IrrGuess, "guess");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.MatchLookupvalue, "lookup_value");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.MatchLookuparray, "lookup_array");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.MatchMatchtype, "match_type");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DateYear, "year");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DateMonth, "month");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DateDay, "day");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TimeHour, "hour");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TimeMinute, "minute");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TimeSecond, "second");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DaySerialnumber, "serial_number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.MonthSerialnumber, "serial_number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.YearSerialnumber, "serial_number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.WeekDaySerialnumber, "serial_number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.WeekDayReturntype, "return_type");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.HourSerialnumber, "serial_number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.MinuteSerialnumber, "serial_number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SecondSerialnumber, "serial_number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AreasReference, "reference");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.RowsArray, "array");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ColumnsArray, "array");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.OffsetReference, "reference");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.OffsetRows, "rows");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.OffsetCols, "cols");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.OffsetHeight, "height");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.OffsetWidth, "width");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SearchFindtext, "find_text");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SearchWithintext, "within_text");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SearchStartnum, "start_num");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TransposeArray, "array");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TypeValue, "value");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ATan2Xnum, "x_num");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ATan2Ynum, "y_num");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ASinNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ACosNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ChooseIndexnum, "index_num");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ChooseValue1, "value1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ChooseValue2, "value2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.HLookupLookupvalue, "lookup_value");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.HLookupTablearray, "table_array");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.HLookupRowindexnum, "row_index_num");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.HLookupRangelookup, "range_lookup");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.VLookupLookupvalue, "lookup_value");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.VLookupTablearray, "table_array");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.VLookupColindexnum, "col_index_num");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.VLookupRangelookup, "range_lookup");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.IsRefValue, "value");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.LogNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.LogBase, "base");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CharNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.LowerText, "text");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.UpperText, "text");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ProperText, "text");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.LeftText, "text");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.LeftNumchars, "num_chars");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.RightText, "text");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.RightNumchars, "num_chars");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ExactText1, "text1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ExactText2, "text2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TrimText, "text");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ReplaceOldtext, "old_text");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ReplaceStartnum, "start_num");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ReplaceNumchars, "num_chars");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ReplaceNewtext, "new_text");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SubstituteText, "text");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SubstituteOldtext, "old_text");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SubstituteNewtext, "new_text");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SubstituteInstancenum, "instance_num");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CodeText, "text");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FindFindtext, "find_text");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FindWithintext, "within_text");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FindStartnum, "start_num");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CellInfotype, "info_type");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CellReference, "reference");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.IsErrValue, "value");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.IsTextValue, "value");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.IsNumberValue, "value");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.IsBlankValue, "value");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TValue, "value");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.NValue, "value");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DateValueDatetext, "date_text");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TimeValueTimetext, "time_text");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SlnCost, "cost");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SlnSalvage, "salvage");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SlnLife, "life");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SydCost, "cost");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SydSalvage, "salvage");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SydLife, "life");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SydPer, "per");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DdbCost, "cost");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DdbSalvage, "salvage");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DdbLife, "life");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DdbPeriod, "period");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DdbFactor, "factor");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.IndirectReftext, "ref_text");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.IndirectA1, "a1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CallRegisterid, "register_id");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CallArgument1, "argument1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CleanText, "text");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.MDetermArray, "array");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.MInverseArray, "array");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.MMultArray1, "array1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.MMultArray2, "array2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.IpmtRate, "rate");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.IpmtPer, "per");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.IpmtNper, "nper");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.IpmtPv, "pv");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.IpmtFv, "fv");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.IpmtType, "type");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PpmtRate, "rate");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PpmtPer, "per");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PpmtNper, "nper");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PpmtPv, "pv");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PpmtFv, "fv");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PpmtType, "type");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CountAValue1, "value1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CountAValue2, "value2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ProductNumber1, "number1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ProductNumber2, "number2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FactNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DProductDatabase, "database");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DProductField, "field");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DProductCriteria, "criteria");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.IsNonTextValue, "value");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.StDevPNumber1, "number1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.StDevPNumber2, "number2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.VarPNumber1, "number1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.VarPNumber2, "number2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DStDevPDatabase, "database");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DStDevPField, "field");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DStDevPCriteria, "criteria");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DVarPDatabase, "database");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DVarPField, "field");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DVarPCriteria, "criteria");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TruncNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TruncNumdigits, "num_digits");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.IsLogicalValue, "value");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DCountADatabase, "database");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DCountAField, "field");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DCountACriteria, "criteria");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FindBFindtext, "find_text");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FindBWithintext, "within_text");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FindBStartnum, "start_num");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SearchBFindtext, "find_text");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SearchBWithintext, "within_text");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SearchBStartnum, "start_num");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ReplaceBOldtext, "old_text");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ReplaceBStartnum, "start_num");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ReplaceBNumbytes, "num_bytes");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ReplaceBNewtext, "new_text");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.LeftBText, "text");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.LeftBNumbytes, "num_bytes");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.RightBText, "text");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.RightBNumbytes, "num_bytes");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.MidBText, "text");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.MidBStartnum, "start_num");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.MidBNumbytes, "num_bytes");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.LenBText, "text");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.RoundUpNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.RoundUpNumdigits, "num_digits");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.RoundDownNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.RoundDownNumdigits, "num_digits");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AscText, "text");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.RankNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.RankRef, "ref");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.RankOrder, "order");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AddressRownum, "row_num");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AddressColumnnum, "column_num");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AddressAbsnum, "abs_num");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AddressA1, "a1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AddressSheettext, "sheet_text");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.Days360Startdate, "start_date");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.Days360Enddate, "end_date");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.Days360Method, "method");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.VdbCost, "cost");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.VdbSalvage, "salvage");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.VdbLife, "life");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.VdbStartperiod, "start_period");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.VdbEndperiod, "end_period");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.VdbFactor, "factor");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.VdbNoswitch, "no_switch");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.MedianNumber1, "number1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.MedianNumber2, "number2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SumProductArray1, "array1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SumProductArray2, "array2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SumProductArray3, "array3");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SinHNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CosHNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TanHNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ASinHNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ACosHNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ATanHNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DGetDatabase, "database");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DGetField, "field");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DGetCriteria, "criteria");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.InfoTypetext, "type_text");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DbCost, "cost");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DbSalvage, "salvage");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DbLife, "life");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DbPeriod, "period");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DbMonth, "month");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FrequencyDataarray, "data_array");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FrequencyBinsarray, "bins_array");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ErrorTypeErrorval, "error_val");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.RegisterIdModuletext, "module_text");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.RegisterIdProcedure, "procedure");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.RegisterIdTypetext, "type_text");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AvedevNumber1, "number1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AvedevNumber2, "number2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BetaDistCompatibilityX, "x");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BetaDistCompatibilityAlpha, "alpha");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BetaDistCompatibilityBeta, "beta");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BetaDistCompatibilityA, "A");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BetaDistCompatibilityB, "B");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.GammaLnX, "x");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BetaInvProbability, "probability");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BetaInvAlpha, "alpha");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BetaInvBeta, "beta");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BetaInvA, "A");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BetaInvB, "B");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BinomDistNumbers, "number_s");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BinomDistTrials, "trials");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BinomDistProbabilitys, "probability_s");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BinomDistCumulative, "cumulative");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ChiDistX, "x");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ChiDistDegfreedom, "deg_freedom");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ChiInvProbability, "probability");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ChiInvDegfreedom, "deg_freedom");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CombinNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CombinNumberchosen, "number_chosen");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ConfidenceAlpha, "alpha");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ConfidenceStandarddev, "standard_dev");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ConfidenceSize, "size");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CritBinomTrials, "trials");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CritBinomProbabilitys, "probability_s");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CritBinomAlpha, "alpha");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.EvenNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ExponDistX, "x");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ExponDistLambda, "lambda");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ExponDistCumulative, "cumulative");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FDistX, "x");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FDistDegfreedom1, "deg_freedom1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FDistDegfreedom2, "deg_freedom2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FInvProbability, "probability");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FInvDegfreedom1, "deg_freedom1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FInvDegfreedom2, "deg_freedom2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FisherX, "x");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FisherInvY, "y");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FloorNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FloorSignificance, "significance");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.GammaDistX, "x");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.GammaDistAlpha, "alpha");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.GammaDistBeta, "beta");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.GammaDistCumulative, "cumulative");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.GammaInvProbability, "probability");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.GammaInvAlpha, "alpha");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.GammaInvBeta, "beta");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CeilingNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CeilingSignificance, "significance");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.HypGeomDistSamples, "sample_s");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.HypGeomDistNumbersample, "number_sample");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.HypGeomDistPopulations, "population_s");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.HypGeomDistNumberpop, "number_pop");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.LogNormDistCompatibilityX, "x");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.LogNormDistCompatibilityMean, "mean");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.LogNormDistCompatibilityStandarddev, "standard_dev");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.LogInvCompatibilityProbability, "probability");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.LogInvCompatibilityMean, "mean");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.LogInvCompatibilityStandarddev, "standard_dev");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.NegBinomDistNumberf, "number_f");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.NegBinomDistNumbers, "number_s");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.NegBinomDistProbabilitys, "probability_s");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.NormDistCompatibilityX, "x");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.NormDistCompatibilityMean, "mean");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.NormDistCompatibilityStandarddev, "standard_dev");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.NormDistCompatibilityCumulative, "cumulative");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.NormSDistCompatibilityZ, "z");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.NormInvCompatibilityProbability, "probability");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.NormInvCompatibilityMean, "mean");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.NormInvCompatibilityStandarddev, "standard_dev");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.NormSInvCompatibilityProbability, "probability");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.StandardizeX, "x");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.StandardizeMean, "mean");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.StandardizeStandarddev, "standard_dev");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.OddNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PermutNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PermutNumberchosen, "number_chosen");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PoissonX, "x");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PoissonMean, "mean");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PoissonCumulative, "cumulative");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TDistX, "x");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TDistDegfreedom, "deg_freedom");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TDistTails, "tails");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.WeibullX, "x");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.WeibullAlpha, "alpha");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.WeibullBeta, "beta");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.WeibullCumulative, "cumulative");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SumXMY2Arrayx, "array_x");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SumXMY2Arrayy, "array_y");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SumX2MY2Arrayx, "array_x");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SumX2MY2Arrayy, "array_y");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SumX2PY2Arrayx, "array_x");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SumX2PY2Arrayy, "array_y");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ChiTestActualrange, "actual_range");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ChiTestExpectedrange, "expected_range");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CorrelArray1, "array1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CorrelArray2, "array2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CovarArray1, "array1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CovarArray2, "array2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ForecastX, "x");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ForecastKnownys, "known_y's");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ForecastKnownxs, "known_x's");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FTestArray1, "array1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FTestArray2, "array2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.InterceptKnownys, "known_y's");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.InterceptKnownxs, "known_x's");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PearsonArray1, "array1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PearsonArray2, "array2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.RsqKnownys, "known_y's");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.RsqKnownxs, "known_x's");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.StEYXKnownys, "known_y's");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.StEYXKnownxs, "known_x's");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SlopeKnownys, "known_y's");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SlopeKnownxs, "known_x's");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TTestArray1, "array1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TTestArray2, "array2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TTestTails, "tails");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TTestType, "type");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ProbXrange, "x_range");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ProbProbrange, "prob_range");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ProbLowerlimit, "lower_limit");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ProbUpperlimit, "upper_limit");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DevSqNumber1, "number1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DevSqNumber2, "number2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.GeomeanNumber1, "number1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.GeomeanNumber2, "number2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.HarMeanNumber1, "number1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.HarMeanNumber2, "number2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SumSqNumber1, "number1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SumSqNumber2, "number2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.KurtNumber1, "number1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.KurtNumber2, "number2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SkewNumber1, "number1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SkewNumber2, "number2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ZTestArray, "array");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ZTestX, "x");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ZTestSigma, "sigma");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.LargeArray, "array");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.LargeK, "k");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SmallArray, "array");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SmallK, "k");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.QuartileArray, "array");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.QuartileQuart, "quart");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PercentileArray, "array");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PercentileK, "k");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PercentRankArray, "array");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PercentRankX, "x");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PercentRankSignificance, "significance");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ModeNumber1, "number1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ModeNumber2, "number2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TrimmeanArray, "array");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TrimmeanPercent, "percent");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TinvProbability, "probability");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TinvDegfreedom, "deg_freedom");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ConcatenateText1, "text1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ConcatenateText2, "text2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PowerNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PowerPower, "power");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.RadiansAngle, "angle");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DegreesAngle, "angle");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SubtotalFunctionnum, "function_num");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SubtotalRef1, "ref1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SumIfRange, "range");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SumIfCriteria, "criteria");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SumIfSumrange, "sum_range");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CountIfRange, "range");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CountIfCriteria, "criteria");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CountBlankRange, "range");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.IsPmtRate, "rate");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.IsPmtPer, "per");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.IsPmtNper, "nper");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.IsPmtPv, "pv");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.RomanNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.RomanForm, "form");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.GetPivotDataDatafield, "data_field");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.GetPivotDataPivottable, "pivot_table");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.GetPivotDataField, "field");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.GetPivotDataItem, "item");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.HyperlinkLinklocation, "link_location");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.HyperlinkFriendlyname, "friendly_name");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PhoneticReference, "reference");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AverageAValue1, "value1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AverageAValue2, "value2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.MaxAValue1, "value1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.MaxAValue2, "value2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.MinAValue1, "value1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.MinAValue2, "value2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.StDevPAValue1, "value1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.StDevPAValue2, "value2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.VarPAValue1, "value1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.VarPAValue2, "value2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.StDevAValue1, "value1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.StDevAValue2, "value2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.VarAValue1, "value1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.VarAValue2, "value2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BahtTextNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.RtdProgID, "progID");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.RtdServer, "server");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.RtdTopic1, "topic1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.RtdTopic2, "topic2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CubeValueConnection, "connection");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CubeValueMemberexpression1, "member_expression1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CubeMemberConnection, "connection");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CubeMemberMemberexpression, "member_expression");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CubeMemberCaption, "caption");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CubeMemberPropertyConnection, "connection");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CubeMemberPropertyMemberexpression, "member_expression");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CubeMemberPropertyProperty, "property");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CubeRankedMemberConnection, "connection");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CubeRankedMemberSetexpression, "set_expression");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CubeRankedMemberRank, "rank");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CubeRankedMemberCaption, "caption");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.Hex2BinNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.Hex2BinPlaces, "places");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.Hex2DecNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.Hex2OctNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.Hex2OctPlaces, "places");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.Dec2BinNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.Dec2BinPlaces, "places");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.Dec2HexNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.Dec2HexPlaces, "places");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.Dec2OctNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.Dec2OctPlaces, "places");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.Oct2BinNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.Oct2BinPlaces, "places");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.Oct2HexNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.Oct2HexPlaces, "places");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.Oct2DecNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.Bin2DecNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.Bin2OctNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.Bin2OctPlaces, "places");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.Bin2HexNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.Bin2HexPlaces, "places");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ImSubInumber1, "inumber1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ImSubInumber2, "inumber2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ImDivInumber1, "inumber1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ImDivInumber2, "inumber2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ImPowerInumber, "inumber");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ImPowerNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ImAbsInumber, "inumber");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ImSqrtInumber, "inumber");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ImLnInumber, "inumber");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ImLog2Inumber, "inumber");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ImLog10Inumber, "inumber");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ImSinInumber, "inumber");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ImCosInumber, "inumber");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ImExpInumber, "inumber");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ImArgumentInumber, "inumber");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ImConjugateInumber, "inumber");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ImaginaryInumber, "inumber");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ImRealInumber, "inumber");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ComplexRealnum, "real_num");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ComplexInum, "i_num");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ComplexSuffix, "suffix");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ImSumInumber1, "inumber1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ImSumInumber2, "inumber2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ImProductInumber1, "inumber1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ImProductInumber2, "inumber2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SeriessumX, "x");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SeriessumN, "n");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SeriessumM, "m");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SeriessumCoefficients, "coefficients");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FactDoubleNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SqrtPiNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.QuotientNumerator, "numerator");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.QuotientDenominator, "denominator");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DeltaNumber1, "number1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DeltaNumber2, "number2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.GestepNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.GestepStep, "step");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.IsEvenNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.IsOddNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.MRoundNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.MRoundMultiple, "multiple");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ErfLowerlimit, "lower_limit");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ErfUpperlimit, "upper_limit");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ErfcX, "x");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BesselJX, "x");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BesselJN, "n");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BesselKX, "x");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BesselKN, "n");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BesselYX, "x");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BesselYN, "n");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BesselIX, "x");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BesselIN, "n");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.XirrValues, "values");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.XirrDates, "dates");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.XirrGuess, "guess");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.XnpvRate, "rate");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.XnpvValues, "values");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.XnpvDates, "dates");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PriceMatSettlement, "settlement");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PriceMatMaturity, "maturity");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PriceMatIssue, "issue");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PriceMatRate, "rate");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PriceMatYld, "yld");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PriceMatBasis, "basis");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.YieldMatSettlement, "settlement");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.YieldMatMaturity, "maturity");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.YieldMatIssue, "issue");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.YieldMatRate, "rate");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.YieldMatPr, "pr");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.YieldMatBasis, "basis");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.IntrateSettlement, "settlement");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.IntrateMaturity, "maturity");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.IntrateInvestment, "investment");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.IntrateRedemption, "redemption");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.IntrateBasis, "basis");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ReceivedSettlement, "settlement");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ReceivedMaturity, "maturity");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ReceivedInvestment, "investment");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ReceivedDiscount, "discount");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ReceivedBasis, "basis");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DiscSettlement, "settlement");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DiscMaturity, "maturity");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DiscPr, "pr");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DiscRedemption, "redemption");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DiscBasis, "basis");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PriceDiscSettlement, "settlement");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PriceDiscMaturity, "maturity");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PriceDiscDiscount, "discount");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PriceDiscRedemption, "redemption");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PriceDiscBasis, "basis");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.YieldDiscSettlement, "settlement");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.YieldDiscMaturity, "maturity");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.YieldDiscPr, "pr");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.YieldDiscRedemption, "redemption");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.YieldDiscBasis, "basis");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TbillEqSettlement, "settlement");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TbillEqMaturity, "maturity");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TbillEqDiscount, "discount");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TbillPriceSettlement, "settlement");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TbillPriceMaturity, "maturity");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TbillPriceDiscount, "discount");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TbillYieldSettlement, "settlement");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TbillYieldMaturity, "maturity");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TbillYieldPr, "pr");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PriceSettlement, "settlement");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PriceMaturity, "maturity");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PriceRate, "rate");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PriceYld, "yld");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PriceRedemption, "redemption");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PriceFrequency, "frequency");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PriceBasis, "basis");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.YieldSettlement, "settlement");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.YieldMaturity, "maturity");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.YieldRate, "rate");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.YieldPr, "pr");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.YieldRedemption, "redemption");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.YieldFrequency, "frequency");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.YieldBasis, "basis");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DollarDeFractionaldollar, "fractional_dollar");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DollarDeFraction, "fraction");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DollarFrDecimaldollar, "decimal_dollar");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DollarFrFraction, "fraction");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.NominalEffectrate, "effect_rate");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.NominalNpery, "npery");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.EffectNominalrate, "nominal_rate");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.EffectNpery, "npery");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CumPrincRate, "rate");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CumPrincNper, "nper");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CumPrincPv, "pv");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CumPrincStartperiod, "start_period");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CumPrincEndperiod, "end_period");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CumPrincType, "type");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CumIpmtRate, "rate");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CumIpmtNper, "nper");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CumIpmtPv, "pv");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CumIpmtStartperiod, "start_period");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CumIpmtEndperiod, "end_period");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CumIpmtType, "type");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.EDateStartdate, "start_date");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.EDateMonths, "months");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.EOMonthStartdate, "start_date");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.EOMonthMonths, "months");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.YearFracStartdate, "start_date");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.YearFracEnddate, "end_date");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.YearFracBasis, "basis");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CoupDaybsSettlement, "settlement");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CoupDaybsMaturity, "maturity");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CoupDaybsFrequency, "frequency");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CoupDaybsBasis, "basis");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CoupDaysSettlement, "settlement");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CoupDaysMaturity, "maturity");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CoupDaysFrequency, "frequency");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CoupDaysBasis, "basis");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CoupDaysncSettlement, "settlement");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CoupDaysncMaturity, "maturity");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CoupDaysncFrequency, "frequency");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CoupDaysncBasis, "basis");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CoupncdSettlement, "settlement");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CoupncdMaturity, "maturity");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CoupncdFrequency, "frequency");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CoupncdBasis, "basis");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CoupNumSettlement, "settlement");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CoupNumMaturity, "maturity");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CoupNumFrequency, "frequency");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CoupNumBasis, "basis");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CouppcdSettlement, "settlement");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CouppcdMaturity, "maturity");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CouppcdFrequency, "frequency");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CouppcdBasis, "basis");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DurationSettlement, "settlement");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DurationMaturity, "maturity");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DurationCoupon, "coupon");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DurationYld, "yld");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DurationFrequency, "frequency");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DurationBasis, "basis");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.MDurationSettlement, "settlement");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.MDurationMaturity, "maturity");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.MDurationCoupon, "coupon");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.MDurationYld, "yld");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.MDurationFrequency, "frequency");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.MDurationBasis, "basis");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.OddLPriceSettlement, "settlement");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.OddLPriceMaturity, "maturity");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.OddLPriceLastinterest, "last_interest");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.OddLPriceRate, "rate");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.OddLPriceYld, "yld");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.OddLPriceRedemption, "redemption");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.OddLPriceFrequency, "frequency");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.OddLPriceBasis, "basis");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.OddLYieldSettlement, "settlement");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.OddLYieldMaturity, "maturity");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.OddLYieldLastinterest, "last_interest");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.OddLYieldRate, "rate");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.OddLYieldPr, "pr");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.OddLYieldRedemption, "redemption");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.OddLYieldFrequency, "frequency");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.OddLYieldBasis, "basis");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.OddFPriceSettlement, "settlement");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.OddFPriceMaturity, "maturity");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.OddFPriceIssue, "issue");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.OddFPriceFirstcoupon, "first_coupon");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.OddFPriceRate, "rate");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.OddFPriceYld, "yld");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.OddFPriceRedemption, "redemption");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.OddFPriceFrequency, "frequency");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.OddFPriceBasis, "basis");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.OddFYieldSettlement, "settlement");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.OddFYieldMaturity, "maturity");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.OddFYieldIssue, "issue");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.OddFYieldFirstcoupon, "first_coupon");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.OddFYieldRate, "rate");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.OddFYieldPr, "pr");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.OddFYieldRedemption, "redemption");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.OddFYieldFrequency, "frequency");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.OddFYieldBasis, "basis");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.RandBetweenBottom, "bottom");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.RandBetweenTop, "top");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.WeekNumSerialnumber, "serial_number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.WeekNumReturntype, "return_type");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AmordegrcCost, "cost");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AmordegrcDatepurchased, "date_purchased");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AmordegrcFirstperiod, "first_period");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AmordegrcSalvage, "salvage");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AmordegrcPeriod, "period");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AmordegrcRate, "rate");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AmordegrcBasis, "basis");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AmorlincCost, "cost");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AmorlincDatepurchased, "date_purchased");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AmorlincFirstperiod, "first_period");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AmorlincSalvage, "salvage");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AmorlincPeriod, "period");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AmorlincRate, "rate");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AmorlincBasis, "basis");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ConvertNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ConvertFromunit, "from_unit");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ConvertTounit, "to_unit");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AccrintIssue, "issue");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AccrintFirstinterest, "first_interest");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AccrintSettlement, "settlement");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AccrintRate, "rate");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AccrintPar, "par");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AccrintFrequency, "frequency");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AccrintBasis, "basis");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AccrintCalcmethod, "calc_method");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AccrintmIssue, "issue");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AccrintmSettlement, "settlement");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AccrintmRate, "rate");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AccrintmPar, "par");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AccrintmBasis, "basis");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.WorkDayStartdate, "start_date");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.WorkDayDays, "days");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.WorkDayHolidays, "holidays");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.NetworkDaysStartdate, "start_date");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.NetworkDaysEnddate, "end_date");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.NetworkDaysHolidays, "holidays");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.GcdNumber1, "number1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.GcdNumber2, "number2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.MultinomialNumber1, "number1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.MultinomialNumber2, "number2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.LcmNumber1, "number1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.LcmNumber2, "number2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FvSchedulePrincipal, "principal");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FvScheduleSchedule, "schedule");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CubeKpiMemberConnection, "connection");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CubeKpiMemberKpiname, "kpi_name");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CubeKpiMemberKpiproperty, "kpi_property");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CubeKpiMemberCaption, "caption");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CubeSetConnection, "connection");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CubeSetSetexpression, "set_expression");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CubeSetCaption, "caption");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CubeSetSortorder, "sort_order");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CubeSetSortby, "sort_by");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CubeSetCountSet, "set");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.IfErrorValue, "value");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.IfErrorValueiferror, "value_if_error");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CountIfsCriteriarange, "criteria_range");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CountIfsCriteria, "criteria");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SumIfsSumrange, "sum_range");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SumIfsCriteriarange, "criteria_range");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SumIfsCriteria, "criteria");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AverageIfRange, "range");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AverageIfCriteria, "criteria");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AverageIfAveragerange, "average_range");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AverageIfsAveragerange, "average_range");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AverageIfsCriteriarange, "criteria_range");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AverageIfsCriteria, "criteria");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AggregateFunctionnum, "function_num");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AggregateOptions, "options");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AggregateArray, "array");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.AggregateK, "k");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BinomDotDistNumbers, "number_s");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BinomDotDistTrials, "trials");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BinomDotDistProbabilitys, "probability_s");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BinomDotDistCumulative, "cumulative");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BinomDotInvTrials, "trials");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BinomDotInvProbabilitys, "probability_s");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BinomDotInvAlpha, "alpha");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ConfidenceNormAlpha, "alpha");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ConfidenceNormStandarddev, "standard_dev");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ConfidenceNormSize, "size");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ConfidenceDotTAlpha, "alpha");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ConfidenceDotTStandarddev, "standard_dev");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ConfidenceDotTSize, "size");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ChisqDotTestActualrange, "actual_range");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ChisqDotTestExpectedrange, "expected_range");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FDotTestArray1, "array1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FDotTestArray2, "array2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CovariancePArray1, "array1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CovariancePArray2, "array2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CovarianceSArray1, "array1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CovarianceSArray2, "array2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ExponDotDistX, "x");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ExponDotDistLambda, "lambda");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ExponDotDistCumulative, "cumulative");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.GammaDotDistX, "x");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.GammaDotDistAlpha, "alpha");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.GammaDotDistBeta, "beta");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.GammaDotDistCumulative, "cumulative");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.GammaDotInvProbability, "probability");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.GammaDotInvAlpha, "alpha");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.GammaDotInvBeta, "beta");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ModeMultNumber1, "number1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ModeMultNumber2, "number2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ModeSnglNumber1, "number1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ModeSnglNumber2, "number2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.NormDistX, "x");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.NormDistMean, "mean");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.NormDistStandarddev, "standard_dev");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.NormDistCumulative, "cumulative");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.NormInvProbability, "probability");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.NormInvMean, "mean");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.NormInvStandarddev, "standard_dev");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PercentileExcArray, "array");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PercentileExcK, "k");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PercentileIncArray, "array");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PercentileIncK, "k");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PercentRankExcArray, "array");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PercentRankExcX, "x");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PercentRankExcSignificance, "significance");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PercentRankIncArray, "array");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PercentRankIncX, "x");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PercentRankIncSignificance, "significance");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PoissonDotDistX, "x");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PoissonDotDistMean, "mean");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PoissonDotDistCumulative, "cumulative");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.QuartileExcArray, "array");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.QuartileExcQuart, "quart");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.QuartileIncArray, "array");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.QuartileIncQuart, "quart");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.RankDotAvgNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.RankDotAvgRef, "ref");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.RankDotAvgOrder, "order");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.RankDotEqNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.RankDotEqRef, "ref");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.RankDotEqOrder, "order");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.StDevDotSNumber1, "number1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.StDevDotSNumber2, "number2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.StDevDotPNumber1, "number1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.StDevDotPNumber2, "number2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TDotDistX, "x");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TDotDistDegfreedom, "deg_freedom");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TDotDistCumulative, "cumulative");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TDotDistDot2tX, "x");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TDotDistDot2tDegfreedom, "deg_freedom");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TDotDistDotTtX, "x");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TDotDistDotTtDegfreedom, "deg_freedom");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TDotInvProbability, "probability");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TDotInvDegfreedom, "deg_freedom");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TDotInvDot2tProbability, "probability");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TDotInvDot2tDegfreedom, "deg_freedom");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.VarDotSNumber1, "number1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.VarDotSNumber2, "number2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.VarDotPNumber1, "number1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.VarDotPNumber2, "number2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.WeibullDotDistX, "x");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.WeibullDotDistAlpha, "alpha");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.WeibullDotDistBeta, "beta");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.WeibullDotDistCumulative, "cumulative");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.NetworkDaysIntlStartdate, "start_date");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.NetworkDaysIntlEnddate, "end_date");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.NetworkDaysIntlWeekend, "weekend");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.NetworkDaysIntlHolidays, "holidays");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.WorkDayIntlStartdate, "start_date");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.WorkDayIntlDays, "days");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.WorkDayIntlWeekend, "weekend");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.WorkDayIntlHolidays, "holidays");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.IsoCeilingNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.IsoCeilingSignificance, "significance");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BetaDistX, "x");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BetaDistAlpha, "alpha");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BetaDistBeta, "beta");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BetaDistCumulative, "cumulative");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BetaDistA, "A");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BetaDistB, "B");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BetaDotInvProbability, "probability");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BetaDotInvAlpha, "alpha");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BetaDotInvBeta, "beta");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BetaDotInvA, "A");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BetaDotInvB, "B");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ChisqDotDistX, "x");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ChisqDotDistDegfreedom, "deg_freedom");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ChisqDotDistCumulative, "cumulative");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ChisqDotDistDotRtX, "x");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ChisqDotDistDotRtDegfreedom, "deg_freedom");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ChisqDotInvProbability, "probability");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ChisqDotInvDegfreedom, "deg_freedom");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ChisqDotInvDotRtProbability, "probability");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ChisqDotInvDotRtDegfreedom, "deg_freedom");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FDotDistX, "x");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FDotDistDegfreedom1, "deg_freedom1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FDotDistDegfreedom2, "deg_freedom2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FDotDistCumulative, "cumulative");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FDotDistDotRtX, "x");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FDotDistDotRtDegfreedom1, "deg_freedom1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FDotDistDotRtDegfreedom2, "deg_freedom2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FDotInvProbability, "probability");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FDotInvDegfreedom1, "deg_freedom1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FDotInvDegfreedom2, "deg_freedom2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FDotinvDotRtProbability, "probability");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FDotinvDotRtDegfreedom1, "deg_freedom1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FDotinvDotRtDegfreedom2, "deg_freedom2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.HypgeomDotDistSamples, "sample_s");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.HypgeomDotDistNumbersample, "number_sample");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.HypgeomDotDistPopulations, "population_s");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.HypgeomDotDistNumberpop, "number_pop");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.HypgeomDotDistCumulative, "cumulative");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.LogNormDistX, "x");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.LogNormDistMean, "mean");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.LogNormDistStandarddev, "standard_dev");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.LogNormDistCumulative, "cumulative");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.LogNormInvProbability, "probability");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.LogNormInvMean, "mean");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.LogNormInvStandarddev, "standard_dev");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.NegbinomDotDistNumberf, "number_f");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.NegbinomDotDistNumbers, "number_s");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.NegbinomDotDistProbabilitys, "probability_s");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.NegbinomDotDistCumulative, "cumulative");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.NormSDistZ, "z");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.NormSDistCumulative, "cumulative");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.NormSInvProbability, "probability");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TDotTestArray1, "array1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TDotTestArray2, "array2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TDotTestTails, "tails");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.TDotTestType, "type");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ZDotTestArray, "array");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ZDotTestX, "x");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ZDotTestSigma, "sigma");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ErfPreciseX, "X");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ErfcPreciseX, "X");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.GammaLnPreciseX, "x");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CeilingDotPreciseNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CeilingDotPreciseSignificance, "significance");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FloorDotPreciseNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FloorDotPreciseSignificance, "significance");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ACotNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ACotHNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CotNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CotHNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CscNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CscHNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SecNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SecHNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ImTanInumber, "inumber");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ImCotInumber, "inumber");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ImCscInumber, "inumber");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ImCscHInumber, "inumber");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ImSecInumber, "inumber");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ImSecHInumber, "inumber");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BitAndNumber1, "number1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BitAndNumber2, "number2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BitOrNumber1, "number1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BitOrNumber2, "number2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BitXorNumber1, "number1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BitXorNumber2, "number2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BitLShiftNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BitLShiftShiftamount, "shift_amount");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BitRShiftNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BitRShiftShiftamount, "shift_amount");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PermutationaNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PermutationaNumberchosen, "number_chosen");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CombinANumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CombinANumberchosen, "number_chosen");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.XOrLogical1, "logical1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.XOrLogical2, "logical2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PDurationRate, "rate");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PDurationPv, "pv");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PDurationFv, "fv");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BaseNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BaseRadix, "radix");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BaseMinlength, "min_length");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DecimalNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DecimalRadix, "radix");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DaysEnddate, "end_date");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.DaysStartdate, "start_date");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BinomDistRangeTrials, "trials");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BinomDistRangeProbabilitys, "probability_s");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BinomDistRangeNumbers, "number_s");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.BinomDistRangeNumbers2, "number_s2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.GammaX, "x");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SkewPNumber1, "number1");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SkewPNumber2, "number2");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.PhiX, "x");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.RRINper, "nper");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.RRIPv, "pv");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.RRIFv, "fv");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.UnicodeText, "text");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.MUnitDimension, "dimension");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ArabicText, "text");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ISOWeekNumDate, "date");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.NumberValueText, "text");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.NumberValueDecimalseparator, "decimal_separator");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.NumberValueGroupseparator, "group_separator");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SheetValue, "value");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.SheetsReference, "reference");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FormulaTextReference, "reference");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.IsFormulaReference, "reference");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.IfNAValue, "value");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.IfNAValueifna, "value_if_na");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CeilingMathNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CeilingMathSignificance, "significance");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.CeilingMathMode, "mode");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FloorMathNumber, "number");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FloorMathSignificance, "significance");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FloorMathMode, "mode");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ImSinHInumber, "inumber");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ImCosHInumber, "inumber");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FunctionFilterXMLXml, "xml");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FunctionFilterXMLXpath, "xpath");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FunctionWebServiceUrl, "url");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.EncodeURLText, "text");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FieldDataFieldName, "data_field_name");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.RangeCellRefenrece, "abs_cell_reference");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FieldPictureDataFieldName, "data_field_name");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FieldPicturePicturePlacement, "picture_placement");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FieldPictureTargetRange, "target_range");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FieldPictureLockAspectRatio, "lock_aspect_ratio");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FieldPictureOffsetX, "offsetX");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FieldPictureOffsetY, "offsetY");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FieldPictureWidth, "width");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.FieldPictureHeight, "height");
			AddString(XtraSpreadsheetFunctionArgumentNameStringId.ParameterParameterName, "parameter_name");
		}
		#endregion
		public static XtraLocalizer<XtraSpreadsheetFunctionArgumentNameStringId> CreateDefaultLocalizer() {
			return new XtraSpreadsheetFunctionArgumentsNamesResLocalizer();
		}
		public static string GetString(XtraSpreadsheetFunctionArgumentNameStringId id) {
			return Active.GetLocalizedString(id);
		}
		public override XtraLocalizer<XtraSpreadsheetFunctionArgumentNameStringId> CreateResXLocalizer() {
			return new XtraSpreadsheetFunctionArgumentsNamesResLocalizer();
		}
		protected override void AddString(XtraSpreadsheetFunctionArgumentNameStringId id, string str) {
			Dictionary<XtraSpreadsheetFunctionArgumentNameStringId, string> table = XtraLocalizierHelper<XtraSpreadsheetFunctionArgumentNameStringId>.GetStringTable(this);
			table[id] = str;
		}
	}
	#endregion
	#region XtraSpreadsheetFunctionArgumentsNamesResLocalizer
	public class XtraSpreadsheetFunctionArgumentsNamesResLocalizer : XtraResXLocalizer<XtraSpreadsheetFunctionArgumentNameStringId> {
		static XtraSpreadsheetFunctionArgumentsNamesResLocalizer() { SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<XtraSpreadsheetFunctionArgumentNameStringId>(CreateDefaultLocalizer())); }
		public XtraSpreadsheetFunctionArgumentsNamesResLocalizer()
			: base(new XtraSpreadsheetFunctionArgumentsNamesLocalizer()) { }
		public static XtraLocalizer<XtraSpreadsheetFunctionArgumentNameStringId> CreateDefaultLocalizer() {
			return new XtraSpreadsheetFunctionArgumentsNamesResLocalizer();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new ResourceManager Manager {
			get { return base.Manager; }
		}
		protected override ResourceManager CreateResourceManagerCore() {
#if DXPORTABLE
			return new ResourceManager("DevExpress.Spreadsheet.Core.LocalizationFunctionArgumentsNamesRes", typeof(XtraSpreadsheetFunctionArgumentsNamesResLocalizer).GetAssembly());
#else
			return new ResourceManager("DevExpress.XtraSpreadsheet.LocalizationFunctionArgumentsNamesRes", typeof(XtraSpreadsheetFunctionArgumentsNamesResLocalizer).GetAssembly());
#endif
		}
#if DXPORTABLE
		public static string GetString(XtraSpreadsheetFunctionArgumentNameStringId id, CultureInfo culture) {
			return XtraResXLocalizer<XtraSpreadsheetFunctionArgumentNameStringId>.GetLocalizedStringFromResources<XtraSpreadsheetFunctionArgumentNameStringId, XtraSpreadsheetFunctionArgumentsNamesResLocalizer>(
				id,
				culture,
				() => Active as XtraSpreadsheetFunctionArgumentsNamesResLocalizer,
				(stringId) => XtraSpreadsheetFunctionArgumentsNamesLocalizer.GetString(stringId)
			);
		}
#else
		[ThreadStatic]
		static CultureInfo lastCulture;
		[ThreadStatic]
		static ResourceSet lastSet;
		public static string GetString(XtraSpreadsheetFunctionArgumentNameStringId id, CultureInfo culture) {
			if (culture == lastCulture)
				return GetString(id, lastSet);
			XtraSpreadsheetFunctionArgumentsNamesLocalizer.GetString(id); 
			lastCulture = culture;
			lastSet = null;
			XtraSpreadsheetFunctionArgumentsNamesResLocalizer localizer = Active as XtraSpreadsheetFunctionArgumentsNamesResLocalizer;
			if (localizer == null)
				return XtraSpreadsheetFunctionArgumentsNamesLocalizer.GetString(id);
			lastSet = localizer.Manager.GetResourceSet(culture, true, true);
			return GetString(id, lastSet);
		}
		static string GetString(XtraSpreadsheetFunctionArgumentNameStringId id, ResourceSet set) {
			if (set == null)
				return XtraSpreadsheetFunctionArgumentsNamesLocalizer.GetString(id);
			string resStr = String.Format("{0}.{1}", typeof(XtraSpreadsheetFunctionArgumentNameStringId).Name, id);
			string result = set.GetString(resStr);
			if (!String.IsNullOrEmpty(result))
				return result;
			return XtraSpreadsheetFunctionArgumentsNamesLocalizer.GetString(id);
		}
#endif
	}
	#endregion
	#region XtraSpreadsheetFunctionArgumentDescriptionStringId
	public enum XtraSpreadsheetFunctionArgumentDescriptionStringId {
		CountValue1 = 0,
		CountValue2 = 1,
		IfLogicaltest = 1000,
		IfValueiftrue = 1001,
		IfValueiffalse = 1002,
		IsNAValue = 2000,
		IsErrorValue = 3000,
		SumNumber1 = 4000,
		SumNumber2 = 4001,
		AverageNumber1 = 5000,
		AverageNumber2 = 5001,
		MinNumber1 = 6000,
		MinNumber2 = 6001,
		MaxNumber1 = 7000,
		MaxNumber2 = 7001,
		RowReference = 8000,
		ColumnReference = 9000,
		NpvRate = 11000,
		NpvValue1 = 11001,
		NpvValue2 = 11002,
		StDevNumber1 = 12000,
		StDevNumber2 = 12001,
		DollarNumber = 13000,
		DollarDecimals = 13001,
		FixedNumber = 14000,
		FixedDecimals = 14001,
		FixedNocommas = 14002,
		SinNumber = 15000,
		CosNumber = 16000,
		TanNumber = 17000,
		ATanNumber = 18000,
		SqrtNumber = 20000,
		ExpNumber = 21000,
		LnNumber = 22000,
		Log10Number = 23000,
		AbsNumber = 24000,
		IntNumber = 25000,
		SignNumber = 26000,
		RoundNumber = 27000,
		RoundNumdigits = 27001,
		LookupLookupvalue = 28000,
		LookupLookupvector = 28001,
		LookupResultvector = 28002,
		IndexArray = 29000,
		IndexRownum = 29001,
		IndexColumnnum = 29002,
		ReptText = 30000,
		ReptNumbertimes = 30001,
		MidText = 31000,
		MidStartnum = 31001,
		MidNumchars = 31002,
		LenText = 32000,
		ValueText = 33000,
		AndLogical1 = 36000,
		AndLogical2 = 36001,
		OrLogical1 = 37000,
		OrLogical2 = 37001,
		NotLogical = 38000,
		ModNumber = 39000,
		ModDivisor = 39001,
		DCountDatabase = 40000,
		DCountField = 40001,
		DCountCriteria = 40002,
		DSumDatabase = 41000,
		DSumField = 41001,
		DSumCriteria = 41002,
		DAverageDatabase = 42000,
		DAverageField = 42001,
		DAverageCriteria = 42002,
		DMinDatabase = 43000,
		DMinField = 43001,
		DMinCriteria = 43002,
		DMaxDatabase = 44000,
		DMaxField = 44001,
		DMaxCriteria = 44002,
		DStDevDatabase = 45000,
		DStDevField = 45001,
		DStDevCriteria = 45002,
		VarNumber1 = 46000,
		VarNumber2 = 46001,
		DVarDatabase = 47000,
		DVarField = 47001,
		DVarCriteria = 47002,
		TextValue = 48000,
		TextFormattext = 48001,
		LinestKnownys = 49000,
		LinestKnownxs = 49001,
		LinestConst = 49002,
		LinestStats = 49003,
		TrendKnownys = 50000,
		TrendKnownxs = 50001,
		TrendNewxs = 50002,
		TrendConst = 50003,
		LogestKnownys = 51000,
		LogestKnownxs = 51001,
		LogestConst = 51002,
		LogestStats = 51003,
		GrowthKnownys = 52000,
		GrowthKnownxs = 52001,
		GrowthNewxs = 52002,
		GrowthConst = 52003,
		PvRate = 56000,
		PvNper = 56001,
		PvPmt = 56002,
		PvFv = 56003,
		PvType = 56004,
		FvRate = 57000,
		FvNper = 57001,
		FvPmt = 57002,
		FvPv = 57003,
		FvType = 57004,
		NPerRate = 58000,
		NPerPmt = 58001,
		NPerPv = 58002,
		NPerFv = 58003,
		NPerType = 58004,
		PmtRate = 59000,
		PmtNper = 59001,
		PmtPv = 59002,
		PmtFv = 59003,
		PmtType = 59004,
		RateNper = 60000,
		RatePmt = 60001,
		RatePv = 60002,
		RateFv = 60003,
		RateType = 60004,
		RateGuess = 60005,
		MirrValues = 61000,
		MirrFinancerate = 61001,
		MirrReinvestrate = 61002,
		IrrValues = 62000,
		IrrGuess = 62001,
		MatchLookupvalue = 64000,
		MatchLookuparray = 64001,
		MatchMatchtype = 64002,
		DateYear = 65000,
		DateMonth = 65001,
		DateDay = 65002,
		TimeHour = 66000,
		TimeMinute = 66001,
		TimeSecond = 66002,
		DaySerialnumber = 67000,
		MonthSerialnumber = 68000,
		YearSerialnumber = 69000,
		WeekDaySerialnumber = 70000,
		WeekDayReturntype = 70001,
		HourSerialnumber = 71000,
		MinuteSerialnumber = 72000,
		SecondSerialnumber = 73000,
		AreasReference = 75000,
		RowsArray = 76000,
		ColumnsArray = 77000,
		OffsetReference = 78000,
		OffsetRows = 78001,
		OffsetCols = 78002,
		OffsetHeight = 78003,
		OffsetWidth = 78004,
		SearchFindtext = 82000,
		SearchWithintext = 82001,
		SearchStartnum = 82002,
		TransposeArray = 83000,
		TypeValue = 86000,
		ATan2Xnum = 97000,
		ATan2Ynum = 97001,
		ASinNumber = 98000,
		ACosNumber = 99000,
		ChooseIndexnum = 100000,
		ChooseValue1 = 100001,
		ChooseValue2 = 100002,
		HLookupLookupvalue = 101000,
		HLookupTablearray = 101001,
		HLookupRowindexnum = 101002,
		HLookupRangelookup = 101003,
		VLookupLookupvalue = 102000,
		VLookupTablearray = 102001,
		VLookupColindexnum = 102002,
		VLookupRangelookup = 102003,
		IsRefValue = 105000,
		LogNumber = 109000,
		LogBase = 109001,
		CharNumber = 111000,
		LowerText = 112000,
		UpperText = 113000,
		ProperText = 114000,
		LeftText = 115000,
		LeftNumchars = 115001,
		RightText = 116000,
		RightNumchars = 116001,
		ExactText1 = 117000,
		ExactText2 = 117001,
		TrimText = 118000,
		ReplaceOldtext = 119000,
		ReplaceStartnum = 119001,
		ReplaceNumchars = 119002,
		ReplaceNewtext = 119003,
		SubstituteText = 120000,
		SubstituteOldtext = 120001,
		SubstituteNewtext = 120002,
		SubstituteInstancenum = 120003,
		CodeText = 121000,
		FindFindtext = 124000,
		FindWithintext = 124001,
		FindStartnum = 124002,
		CellInfotype = 125000,
		CellReference = 125001,
		IsErrValue = 126000,
		IsTextValue = 127000,
		IsNumberValue = 128000,
		IsBlankValue = 129000,
		TValue = 130000,
		NValue = 131000,
		DateValueDatetext = 140000,
		TimeValueTimetext = 141000,
		SlnCost = 142000,
		SlnSalvage = 142001,
		SlnLife = 142002,
		SydCost = 143000,
		SydSalvage = 143001,
		SydLife = 143002,
		SydPer = 143003,
		DdbCost = 144000,
		DdbSalvage = 144001,
		DdbLife = 144002,
		DdbPeriod = 144003,
		DdbFactor = 144004,
		IndirectReftext = 148000,
		IndirectA1 = 148001,
		CallRegisterid = 150000,
		CallArgument1 = 150001,
		CleanText = 162000,
		MDetermArray = 163000,
		MInverseArray = 164000,
		MMultArray1 = 165000,
		MMultArray2 = 165001,
		IpmtRate = 167000,
		IpmtPer = 167001,
		IpmtNper = 167002,
		IpmtPv = 167003,
		IpmtFv = 167004,
		IpmtType = 167005,
		PpmtRate = 168000,
		PpmtPer = 168001,
		PpmtNper = 168002,
		PpmtPv = 168003,
		PpmtFv = 168004,
		PpmtType = 168005,
		CountAValue1 = 169000,
		CountAValue2 = 169001,
		ProductNumber1 = 183000,
		ProductNumber2 = 183001,
		FactNumber = 184000,
		DProductDatabase = 189000,
		DProductField = 189001,
		DProductCriteria = 189002,
		IsNonTextValue = 190000,
		StDevPNumber1 = 193000,
		StDevPNumber2 = 193001,
		VarPNumber1 = 194000,
		VarPNumber2 = 194001,
		DStDevPDatabase = 195000,
		DStDevPField = 195001,
		DStDevPCriteria = 195002,
		DVarPDatabase = 196000,
		DVarPField = 196001,
		DVarPCriteria = 196002,
		TruncNumber = 197000,
		TruncNumdigits = 197001,
		IsLogicalValue = 198000,
		DCountADatabase = 199000,
		DCountAField = 199001,
		DCountACriteria = 199002,
		FindBFindtext = 205000,
		FindBWithintext = 205001,
		FindBStartnum = 205002,
		SearchBFindtext = 206000,
		SearchBWithintext = 206001,
		SearchBStartnum = 206002,
		ReplaceBOldtext = 207000,
		ReplaceBStartnum = 207001,
		ReplaceBNumbytes = 207002,
		ReplaceBNewtext = 207003,
		LeftBText = 208000,
		LeftBNumbytes = 208001,
		RightBText = 209000,
		RightBNumbytes = 209001,
		MidBText = 210000,
		MidBStartnum = 210001,
		MidBNumbytes = 210002,
		LenBText = 211000,
		RoundUpNumber = 212000,
		RoundUpNumdigits = 212001,
		RoundDownNumber = 213000,
		RoundDownNumdigits = 213001,
		AscText = 214000,
		RankNumber = 216000,
		RankRef = 216001,
		RankOrder = 216002,
		AddressRownum = 219000,
		AddressColumnnum = 219001,
		AddressAbsnum = 219002,
		AddressA1 = 219003,
		AddressSheettext = 219004,
		Days360Startdate = 220000,
		Days360Enddate = 220001,
		Days360Method = 220002,
		VdbCost = 222000,
		VdbSalvage = 222001,
		VdbLife = 222002,
		VdbStartperiod = 222003,
		VdbEndperiod = 222004,
		VdbFactor = 222005,
		VdbNoswitch = 222006,
		MedianNumber1 = 227000,
		MedianNumber2 = 227001,
		SumProductArray1 = 228000,
		SumProductArray2 = 228001,
		SumProductArray3 = 228002,
		SinHNumber = 229000,
		CosHNumber = 230000,
		TanHNumber = 231000,
		ASinHNumber = 232000,
		ACosHNumber = 233000,
		ATanHNumber = 234000,
		DGetDatabase = 235000,
		DGetField = 235001,
		DGetCriteria = 235002,
		InfoTypetext = 244000,
		DbCost = 247000,
		DbSalvage = 247001,
		DbLife = 247002,
		DbPeriod = 247003,
		DbMonth = 247004,
		FrequencyDataarray = 252000,
		FrequencyBinsarray = 252001,
		ErrorTypeErrorval = 261000,
		RegisterIdModuletext = 267000,
		RegisterIdProcedure = 267001,
		RegisterIdTypetext = 267002,
		AvedevNumber1 = 269000,
		AvedevNumber2 = 269001,
		BetaDistCompatibilityX = 270000,
		BetaDistCompatibilityAlpha = 270001,
		BetaDistCompatibilityBeta = 270002,
		BetaDistCompatibilityA = 270003,
		BetaDistCompatibilityB = 270004,
		GammaLnX = 271000,
		BetaInvProbability = 272000,
		BetaInvAlpha = 272001,
		BetaInvBeta = 272002,
		BetaInvA = 272003,
		BetaInvB = 272004,
		BinomDistNumbers = 273000,
		BinomDistTrials = 273001,
		BinomDistProbabilitys = 273002,
		BinomDistCumulative = 273003,
		ChiDistX = 274000,
		ChiDistDegfreedom = 274001,
		ChiInvProbability = 275000,
		ChiInvDegfreedom = 275001,
		CombinNumber = 276000,
		CombinNumberchosen = 276001,
		ConfidenceAlpha = 277000,
		ConfidenceStandarddev = 277001,
		ConfidenceSize = 277002,
		CritBinomTrials = 278000,
		CritBinomProbabilitys = 278001,
		CritBinomAlpha = 278002,
		EvenNumber = 279000,
		ExponDistX = 280000,
		ExponDistLambda = 280001,
		ExponDistCumulative = 280002,
		FDistX = 281000,
		FDistDegfreedom1 = 281001,
		FDistDegfreedom2 = 281002,
		FInvProbability = 282000,
		FInvDegfreedom1 = 282001,
		FInvDegfreedom2 = 282002,
		FisherX = 283000,
		FisherInvY = 284000,
		FloorNumber = 285000,
		FloorSignificance = 285001,
		GammaDistX = 286000,
		GammaDistAlpha = 286001,
		GammaDistBeta = 286002,
		GammaDistCumulative = 286003,
		GammaInvProbability = 287000,
		GammaInvAlpha = 287001,
		GammaInvBeta = 287002,
		CeilingNumber = 288000,
		CeilingSignificance = 288001,
		HypGeomDistSamples = 289000,
		HypGeomDistNumbersample = 289001,
		HypGeomDistPopulations = 289002,
		HypGeomDistNumberpop = 289003,
		LogNormDistCompatibilityX = 290000,
		LogNormDistCompatibilityMean = 290001,
		LogNormDistCompatibilityStandarddev = 290002,
		LogInvCompatibilityProbability = 291000,
		LogInvCompatibilityMean = 291001,
		LogInvCompatibilityStandarddev = 291002,
		NegBinomDistNumberf = 292000,
		NegBinomDistNumbers = 292001,
		NegBinomDistProbabilitys = 292002,
		NormDistCompatibilityX = 293000,
		NormDistCompatibilityMean = 293001,
		NormDistCompatibilityStandarddev = 293002,
		NormDistCompatibilityCumulative = 293003,
		NormSDistCompatibilityZ = 294000,
		NormInvCompatibilityProbability = 295000,
		NormInvCompatibilityMean = 295001,
		NormInvCompatibilityStandarddev = 295002,
		NormSInvCompatibilityProbability = 296000,
		StandardizeX = 297000,
		StandardizeMean = 297001,
		StandardizeStandarddev = 297002,
		OddNumber = 298000,
		PermutNumber = 299000,
		PermutNumberchosen = 299001,
		PoissonX = 300000,
		PoissonMean = 300001,
		PoissonCumulative = 300002,
		TDistX = 301000,
		TDistDegfreedom = 301001,
		TDistTails = 301002,
		WeibullX = 302000,
		WeibullAlpha = 302001,
		WeibullBeta = 302002,
		WeibullCumulative = 302003,
		SumXMY2Arrayx = 303000,
		SumXMY2Arrayy = 303001,
		SumX2MY2Arrayx = 304000,
		SumX2MY2Arrayy = 304001,
		SumX2PY2Arrayx = 305000,
		SumX2PY2Arrayy = 305001,
		ChiTestActualrange = 306000,
		ChiTestExpectedrange = 306001,
		CorrelArray1 = 307000,
		CorrelArray2 = 307001,
		CovarArray1 = 308000,
		CovarArray2 = 308001,
		ForecastX = 309000,
		ForecastKnownys = 309001,
		ForecastKnownxs = 309002,
		FTestArray1 = 310000,
		FTestArray2 = 310001,
		InterceptKnownys = 311000,
		InterceptKnownxs = 311001,
		PearsonArray1 = 312000,
		PearsonArray2 = 312001,
		RsqKnownys = 313000,
		RsqKnownxs = 313001,
		StEYXKnownys = 314000,
		StEYXKnownxs = 314001,
		SlopeKnownys = 315000,
		SlopeKnownxs = 315001,
		TTestArray1 = 316000,
		TTestArray2 = 316001,
		TTestTails = 316002,
		TTestType = 316003,
		ProbXrange = 317000,
		ProbProbrange = 317001,
		ProbLowerlimit = 317002,
		ProbUpperlimit = 317003,
		DevSqNumber1 = 318000,
		DevSqNumber2 = 318001,
		GeomeanNumber1 = 319000,
		GeomeanNumber2 = 319001,
		HarMeanNumber1 = 320000,
		HarMeanNumber2 = 320001,
		SumSqNumber1 = 321000,
		SumSqNumber2 = 321001,
		KurtNumber1 = 322000,
		KurtNumber2 = 322001,
		SkewNumber1 = 323000,
		SkewNumber2 = 323001,
		ZTestArray = 324000,
		ZTestX = 324001,
		ZTestSigma = 324002,
		LargeArray = 325000,
		LargeK = 325001,
		SmallArray = 326000,
		SmallK = 326001,
		QuartileArray = 327000,
		QuartileQuart = 327001,
		PercentileArray = 328000,
		PercentileK = 328001,
		PercentRankArray = 329000,
		PercentRankX = 329001,
		PercentRankSignificance = 329002,
		ModeNumber1 = 330000,
		ModeNumber2 = 330001,
		TrimmeanArray = 331000,
		TrimmeanPercent = 331001,
		TinvProbability = 332000,
		TinvDegfreedom = 332001,
		ConcatenateText1 = 336000,
		ConcatenateText2 = 336001,
		PowerNumber = 337000,
		PowerPower = 337001,
		RadiansAngle = 342000,
		DegreesAngle = 343000,
		SubtotalFunctionnum = 344000,
		SubtotalRef1 = 344001,
		SumIfRange = 345000,
		SumIfCriteria = 345001,
		SumIfSumrange = 345002,
		CountIfRange = 346000,
		CountIfCriteria = 346001,
		CountBlankRange = 347000,
		IsPmtRate = 350000,
		IsPmtPer = 350001,
		IsPmtNper = 350002,
		IsPmtPv = 350003,
		RomanNumber = 354000,
		RomanForm = 354001,
		GetPivotDataDatafield = 358000,
		GetPivotDataPivottable = 358001,
		GetPivotDataField = 358002,
		GetPivotDataItem = 358003,
		HyperlinkLinklocation = 359000,
		HyperlinkFriendlyname = 359001,
		PhoneticReference = 360000,
		AverageAValue1 = 361000,
		AverageAValue2 = 361001,
		MaxAValue1 = 362000,
		MaxAValue2 = 362001,
		MinAValue1 = 363000,
		MinAValue2 = 363001,
		StDevPAValue1 = 364000,
		StDevPAValue2 = 364001,
		VarPAValue1 = 365000,
		VarPAValue2 = 365001,
		StDevAValue1 = 366000,
		StDevAValue2 = 366001,
		VarAValue1 = 367000,
		VarAValue2 = 367001,
		BahtTextNumber = 368000,
		RtdProgID = 379000,
		RtdServer = 379001,
		RtdTopic1 = 379002,
		RtdTopic2 = 379003,
		CubeValueConnection = 380000,
		CubeValueMemberexpression1 = 380001,
		CubeMemberConnection = 381000,
		CubeMemberMemberexpression = 381001,
		CubeMemberCaption = 381002,
		CubeMemberPropertyConnection = 382000,
		CubeMemberPropertyMemberexpression = 382001,
		CubeMemberPropertyProperty = 382002,
		CubeRankedMemberConnection = 383000,
		CubeRankedMemberSetexpression = 383001,
		CubeRankedMemberRank = 383002,
		CubeRankedMemberCaption = 383003,
		Hex2BinNumber = 384000,
		Hex2BinPlaces = 384001,
		Hex2DecNumber = 385000,
		Hex2OctNumber = 386000,
		Hex2OctPlaces = 386001,
		Dec2BinNumber = 387000,
		Dec2BinPlaces = 387001,
		Dec2HexNumber = 388000,
		Dec2HexPlaces = 388001,
		Dec2OctNumber = 389000,
		Dec2OctPlaces = 389001,
		Oct2BinNumber = 390000,
		Oct2BinPlaces = 390001,
		Oct2HexNumber = 391000,
		Oct2HexPlaces = 391001,
		Oct2DecNumber = 392000,
		Bin2DecNumber = 393000,
		Bin2OctNumber = 394000,
		Bin2OctPlaces = 394001,
		Bin2HexNumber = 395000,
		Bin2HexPlaces = 395001,
		ImSubInumber1 = 396000,
		ImSubInumber2 = 396001,
		ImDivInumber1 = 397000,
		ImDivInumber2 = 397001,
		ImPowerInumber = 398000,
		ImPowerNumber = 398001,
		ImAbsInumber = 399000,
		ImSqrtInumber = 400000,
		ImLnInumber = 401000,
		ImLog2Inumber = 402000,
		ImLog10Inumber = 403000,
		ImSinInumber = 404000,
		ImCosInumber = 405000,
		ImExpInumber = 406000,
		ImArgumentInumber = 407000,
		ImConjugateInumber = 408000,
		ImaginaryInumber = 409000,
		ImRealInumber = 410000,
		ComplexRealnum = 411000,
		ComplexInum = 411001,
		ComplexSuffix = 411002,
		ImSumInumber1 = 412000,
		ImSumInumber2 = 412001,
		ImProductInumber1 = 413000,
		ImProductInumber2 = 413001,
		SeriessumX = 414000,
		SeriessumN = 414001,
		SeriessumM = 414002,
		SeriessumCoefficients = 414003,
		FactDoubleNumber = 415000,
		SqrtPiNumber = 416000,
		QuotientNumerator = 417000,
		QuotientDenominator = 417001,
		DeltaNumber1 = 418000,
		DeltaNumber2 = 418001,
		GestepNumber = 419000,
		GestepStep = 419001,
		IsEvenNumber = 420000,
		IsOddNumber = 421000,
		MRoundNumber = 422000,
		MRoundMultiple = 422001,
		ErfLowerlimit = 423000,
		ErfUpperlimit = 423001,
		ErfcX = 424000,
		BesselJX = 425000,
		BesselJN = 425001,
		BesselKX = 426000,
		BesselKN = 426001,
		BesselYX = 427000,
		BesselYN = 427001,
		BesselIX = 428000,
		BesselIN = 428001,
		XirrValues = 429000,
		XirrDates = 429001,
		XirrGuess = 429002,
		XnpvRate = 430000,
		XnpvValues = 430001,
		XnpvDates = 430002,
		PriceMatSettlement = 431000,
		PriceMatMaturity = 431001,
		PriceMatIssue = 431002,
		PriceMatRate = 431003,
		PriceMatYld = 431004,
		PriceMatBasis = 431005,
		YieldMatSettlement = 432000,
		YieldMatMaturity = 432001,
		YieldMatIssue = 432002,
		YieldMatRate = 432003,
		YieldMatPr = 432004,
		YieldMatBasis = 432005,
		IntrateSettlement = 433000,
		IntrateMaturity = 433001,
		IntrateInvestment = 433002,
		IntrateRedemption = 433003,
		IntrateBasis = 433004,
		ReceivedSettlement = 434000,
		ReceivedMaturity = 434001,
		ReceivedInvestment = 434002,
		ReceivedDiscount = 434003,
		ReceivedBasis = 434004,
		DiscSettlement = 435000,
		DiscMaturity = 435001,
		DiscPr = 435002,
		DiscRedemption = 435003,
		DiscBasis = 435004,
		PriceDiscSettlement = 436000,
		PriceDiscMaturity = 436001,
		PriceDiscDiscount = 436002,
		PriceDiscRedemption = 436003,
		PriceDiscBasis = 436004,
		YieldDiscSettlement = 437000,
		YieldDiscMaturity = 437001,
		YieldDiscPr = 437002,
		YieldDiscRedemption = 437003,
		YieldDiscBasis = 437004,
		TbillEqSettlement = 438000,
		TbillEqMaturity = 438001,
		TbillEqDiscount = 438002,
		TbillPriceSettlement = 439000,
		TbillPriceMaturity = 439001,
		TbillPriceDiscount = 439002,
		TbillYieldSettlement = 440000,
		TbillYieldMaturity = 440001,
		TbillYieldPr = 440002,
		PriceSettlement = 441000,
		PriceMaturity = 441001,
		PriceRate = 441002,
		PriceYld = 441003,
		PriceRedemption = 441004,
		PriceFrequency = 441005,
		PriceBasis = 441006,
		YieldSettlement = 442000,
		YieldMaturity = 442001,
		YieldRate = 442002,
		YieldPr = 442003,
		YieldRedemption = 442004,
		YieldFrequency = 442005,
		YieldBasis = 442006,
		DollarDeFractionaldollar = 443000,
		DollarDeFraction = 443001,
		DollarFrDecimaldollar = 444000,
		DollarFrFraction = 444001,
		NominalEffectrate = 445000,
		NominalNpery = 445001,
		EffectNominalrate = 446000,
		EffectNpery = 446001,
		CumPrincRate = 447000,
		CumPrincNper = 447001,
		CumPrincPv = 447002,
		CumPrincStartperiod = 447003,
		CumPrincEndperiod = 447004,
		CumPrincType = 447005,
		CumIpmtRate = 448000,
		CumIpmtNper = 448001,
		CumIpmtPv = 448002,
		CumIpmtStartperiod = 448003,
		CumIpmtEndperiod = 448004,
		CumIpmtType = 448005,
		EDateStartdate = 449000,
		EDateMonths = 449001,
		EOMonthStartdate = 450000,
		EOMonthMonths = 450001,
		YearFracStartdate = 451000,
		YearFracEnddate = 451001,
		YearFracBasis = 451002,
		CoupDaybsSettlement = 452000,
		CoupDaybsMaturity = 452001,
		CoupDaybsFrequency = 452002,
		CoupDaybsBasis = 452003,
		CoupDaysSettlement = 453000,
		CoupDaysMaturity = 453001,
		CoupDaysFrequency = 453002,
		CoupDaysBasis = 453003,
		CoupDaysncSettlement = 454000,
		CoupDaysncMaturity = 454001,
		CoupDaysncFrequency = 454002,
		CoupDaysncBasis = 454003,
		CoupncdSettlement = 455000,
		CoupncdMaturity = 455001,
		CoupncdFrequency = 455002,
		CoupncdBasis = 455003,
		CoupNumSettlement = 456000,
		CoupNumMaturity = 456001,
		CoupNumFrequency = 456002,
		CoupNumBasis = 456003,
		CouppcdSettlement = 457000,
		CouppcdMaturity = 457001,
		CouppcdFrequency = 457002,
		CouppcdBasis = 457003,
		DurationSettlement = 458000,
		DurationMaturity = 458001,
		DurationCoupon = 458002,
		DurationYld = 458003,
		DurationFrequency = 458004,
		DurationBasis = 458005,
		MDurationSettlement = 459000,
		MDurationMaturity = 459001,
		MDurationCoupon = 459002,
		MDurationYld = 459003,
		MDurationFrequency = 459004,
		MDurationBasis = 459005,
		OddLPriceSettlement = 460000,
		OddLPriceMaturity = 460001,
		OddLPriceLastinterest = 460002,
		OddLPriceRate = 460003,
		OddLPriceYld = 460004,
		OddLPriceRedemption = 460005,
		OddLPriceFrequency = 460006,
		OddLPriceBasis = 460007,
		OddLYieldSettlement = 461000,
		OddLYieldMaturity = 461001,
		OddLYieldLastinterest = 461002,
		OddLYieldRate = 461003,
		OddLYieldPr = 461004,
		OddLYieldRedemption = 461005,
		OddLYieldFrequency = 461006,
		OddLYieldBasis = 461007,
		OddFPriceSettlement = 462000,
		OddFPriceMaturity = 462001,
		OddFPriceIssue = 462002,
		OddFPriceFirstcoupon = 462003,
		OddFPriceRate = 462004,
		OddFPriceYld = 462005,
		OddFPriceRedemption = 462006,
		OddFPriceFrequency = 462007,
		OddFPriceBasis = 462008,
		OddFYieldSettlement = 463000,
		OddFYieldMaturity = 463001,
		OddFYieldIssue = 463002,
		OddFYieldFirstcoupon = 463003,
		OddFYieldRate = 463004,
		OddFYieldPr = 463005,
		OddFYieldRedemption = 463006,
		OddFYieldFrequency = 463007,
		OddFYieldBasis = 463008,
		RandBetweenBottom = 464000,
		RandBetweenTop = 464001,
		WeekNumSerialnumber = 465000,
		WeekNumReturntype = 465001,
		AmordegrcCost = 466000,
		AmordegrcDatepurchased = 466001,
		AmordegrcFirstperiod = 466002,
		AmordegrcSalvage = 466003,
		AmordegrcPeriod = 466004,
		AmordegrcRate = 466005,
		AmordegrcBasis = 466006,
		AmorlincCost = 467000,
		AmorlincDatepurchased = 467001,
		AmorlincFirstperiod = 467002,
		AmorlincSalvage = 467003,
		AmorlincPeriod = 467004,
		AmorlincRate = 467005,
		AmorlincBasis = 467006,
		ConvertNumber = 468000,
		ConvertFromunit = 468001,
		ConvertTounit = 468002,
		AccrintIssue = 469000,
		AccrintFirstinterest = 469001,
		AccrintSettlement = 469002,
		AccrintRate = 469003,
		AccrintPar = 469004,
		AccrintFrequency = 469005,
		AccrintBasis = 469006,
		AccrintCalcmethod = 469007,
		AccrintmIssue = 470000,
		AccrintmSettlement = 470001,
		AccrintmRate = 470002,
		AccrintmPar = 470003,
		AccrintmBasis = 470004,
		WorkDayStartdate = 471000,
		WorkDayDays = 471001,
		WorkDayHolidays = 471002,
		NetworkDaysStartdate = 472000,
		NetworkDaysEnddate = 472001,
		NetworkDaysHolidays = 472002,
		GcdNumber1 = 473000,
		GcdNumber2 = 473001,
		MultinomialNumber1 = 474000,
		MultinomialNumber2 = 474001,
		LcmNumber1 = 475000,
		LcmNumber2 = 475001,
		FvSchedulePrincipal = 476000,
		FvScheduleSchedule = 476001,
		CubeKpiMemberConnection = 477000,
		CubeKpiMemberKpiname = 477001,
		CubeKpiMemberKpiproperty = 477002,
		CubeKpiMemberCaption = 477003,
		CubeSetConnection = 478000,
		CubeSetSetexpression = 478001,
		CubeSetCaption = 478002,
		CubeSetSortorder = 478003,
		CubeSetSortby = 478004,
		CubeSetCountSet = 479000,
		IfErrorValue = 480000,
		IfErrorValueiferror = 480001,
		CountIfsCriteriarange = 481000,
		CountIfsCriteria = 481001,
		SumIfsSumrange = 482000,
		SumIfsCriteriarange = 482001,
		SumIfsCriteria = 482002,
		AverageIfRange = 483000,
		AverageIfCriteria = 483001,
		AverageIfAveragerange = 483002,
		AverageIfsAveragerange = 484000,
		AverageIfsCriteriarange = 484001,
		AverageIfsCriteria = 484002,
		AggregateFunctionnum = 16494000,
		AggregateOptions = 16494001,
		AggregateArray = 16494002,
		AggregateK = 16494003,
		BinomDotDistNumbers = 16495000,
		BinomDotDistTrials = 16495001,
		BinomDotDistProbabilitys = 16495002,
		BinomDotDistCumulative = 16495003,
		BinomDotInvTrials = 16496000,
		BinomDotInvProbabilitys = 16496001,
		BinomDotInvAlpha = 16496002,
		ConfidenceNormAlpha = 16400000,
		ConfidenceNormStandarddev = 16400001,
		ConfidenceNormSize = 16400002,
		ConfidenceDotTAlpha = 16498000,
		ConfidenceDotTStandarddev = 16498001,
		ConfidenceDotTSize = 16498002,
		ChisqDotTestActualrange = 16489000,
		ChisqDotTestExpectedrange = 16489001,
		FDotTestArray1 = 16470000,
		FDotTestArray2 = 16470001,
		CovariancePArray1 = 16461000,
		CovariancePArray2 = 16461001,
		CovarianceSArray1 = 16462000,
		CovarianceSArray2 = 16462001,
		ExponDotDistX = 16500000,
		ExponDotDistLambda = 16500001,
		ExponDotDistCumulative = 16500002,
		GammaDotDistX = 16472000,
		GammaDotDistAlpha = 16472001,
		GammaDotDistBeta = 16472002,
		GammaDotDistCumulative = 16472003,
		GammaDotInvProbability = 16484000,
		GammaDotInvAlpha = 16484001,
		GammaDotInvBeta = 16484002,
		ModeMultNumber1 = 16385000,
		ModeMultNumber2 = 16385001,
		ModeSnglNumber1 = 16386000,
		ModeSnglNumber2 = 16386001,
		NormDistX = 16397000,
		NormDistMean = 16397001,
		NormDistStandarddev = 16397002,
		NormDistCumulative = 16397003,
		NormInvProbability = 16396000,
		NormInvMean = 16396001,
		NormInvStandarddev = 16396002,
		PercentileExcArray = 16457000,
		PercentileExcK = 16457001,
		PercentileIncArray = 16458000,
		PercentileIncK = 16458001,
		PercentRankExcArray = 16455000,
		PercentRankExcX = 16455001,
		PercentRankExcSignificance = 16455002,
		PercentRankIncArray = 16456000,
		PercentRankIncX = 16456001,
		PercentRankIncSignificance = 16456002,
		PoissonDotDistX = 16501000,
		PoissonDotDistMean = 16501001,
		PoissonDotDistCumulative = 16501002,
		QuartileExcArray = 16459000,
		QuartileExcQuart = 16459001,
		QuartileIncArray = 16460000,
		QuartileIncQuart = 16460001,
		RankDotAvgNumber = 16463000,
		RankDotAvgRef = 16463001,
		RankDotAvgOrder = 16463002,
		RankDotEqNumber = 16464000,
		RankDotEqRef = 16464001,
		RankDotEqOrder = 16464002,
		StDevDotSNumber1 = 16389000,
		StDevDotSNumber2 = 16389001,
		StDevDotPNumber1 = 16390000,
		StDevDotPNumber2 = 16390001,
		TDotDistX = 16469000,
		TDotDistDegfreedom = 16469001,
		TDotDistCumulative = 16469002,
		TDotDistDot2tX = 16481000,
		TDotDistDot2tDegfreedom = 16481001,
		TDotDistDotTtX = 16480000,
		TDotDistDotTtDegfreedom = 16480001,
		TDotInvProbability = 16466000,
		TDotInvDegfreedom = 16466001,
		TDotInvDot2tProbability = 16482000,
		TDotInvDot2tDegfreedom = 16482001,
		VarDotSNumber1 = 16388000,
		VarDotSNumber2 = 16388001,
		VarDotPNumber1 = 16387000,
		VarDotPNumber2 = 16387001,
		WeibullDotDistX = 16491000,
		WeibullDotDistAlpha = 16491001,
		WeibullDotDistBeta = 16491002,
		WeibullDotDistCumulative = 16491003,
		NetworkDaysIntlStartdate = 16399000,
		NetworkDaysIntlEnddate = 16399001,
		NetworkDaysIntlWeekend = 16399002,
		NetworkDaysIntlHolidays = 16399003,
		WorkDayIntlStartdate = 16398000,
		WorkDayIntlDays = 16398001,
		WorkDayIntlWeekend = 16398002,
		WorkDayIntlHolidays = 16398003,
		IsoCeilingNumber = 16417000,
		IsoCeilingSignificance = 16417001,
		BetaDistX = 16404000,
		BetaDistAlpha = 16404001,
		BetaDistBeta = 16404002,
		BetaDistCumulative = 16404003,
		BetaDistA = 16404004,
		BetaDistB = 16404005,
		BetaDotInvProbability = 16465000,
		BetaDotInvAlpha = 16465001,
		BetaDotInvBeta = 16465002,
		BetaDotInvA = 16465003,
		BetaDotInvB = 16465004,
		ChisqDotDistX = 16485000,
		ChisqDotDistDegfreedom = 16485001,
		ChisqDotDistCumulative = 16485002,
		ChisqDotDistDotRtX = 16486000,
		ChisqDotDistDotRtDegfreedom = 16486001,
		ChisqDotInvProbability = 16487000,
		ChisqDotInvDegfreedom = 16487001,
		ChisqDotInvDotRtProbability = 16488000,
		ChisqDotInvDotRtDegfreedom = 16488001,
		FDotDistX = 16468000,
		FDotDistDegfreedom1 = 16468001,
		FDotDistDegfreedom2 = 16468002,
		FDotDistCumulative = 16468003,
		FDotDistDotRtX = 16471000,
		FDotDistDotRtDegfreedom1 = 16471001,
		FDotDistDotRtDegfreedom2 = 16471002,
		FDotInvProbability = 16467000,
		FDotInvDegfreedom1 = 16467001,
		FDotInvDegfreedom2 = 16467002,
		FDotinvDotRtProbability = 16483000,
		FDotinvDotRtDegfreedom1 = 16483001,
		FDotinvDotRtDegfreedom2 = 16483002,
		HypgeomDotDistSamples = 16493000,
		HypgeomDotDistNumbersample = 16493001,
		HypgeomDotDistPopulations = 16493002,
		HypgeomDotDistNumberpop = 16493003,
		HypgeomDotDistCumulative = 16493004,
		LogNormDistX = 16393000,
		LogNormDistMean = 16393001,
		LogNormDistStandarddev = 16393002,
		LogNormDistCumulative = 16393003,
		LogNormInvProbability = 16395000,
		LogNormInvMean = 16395001,
		LogNormInvStandarddev = 16395002,
		NegbinomDotDistNumberf = 16492000,
		NegbinomDotDistNumbers = 16492001,
		NegbinomDotDistProbabilitys = 16492002,
		NegbinomDotDistCumulative = 16492003,
		NormSDistZ = 16392000,
		NormSDistCumulative = 16392001,
		NormSInvProbability = 16394000,
		TDotTestArray1 = 16473000,
		TDotTestArray2 = 16473001,
		TDotTestTails = 16473002,
		TDotTestType = 16473003,
		ZDotTestArray = 16490000,
		ZDotTestX = 16490001,
		ZDotTestSigma = 16490002,
		ErfPreciseX = 16401000,
		ErfcPreciseX = 16402000,
		GammaLnPreciseX = 16403000,
		CeilingDotPreciseNumber = 16497000,
		CeilingDotPreciseSignificance = 16497001,
		FloorDotPreciseNumber = 16499000,
		FloorDotPreciseSignificance = 16499001,
		ACotNumber = 16405000,
		ACotHNumber = 16406000,
		CotNumber = 16411000,
		CotHNumber = 16412000,
		CscNumber = 16413000,
		CscHNumber = 16414000,
		SecNumber = 16419000,
		SecHNumber = 16420000,
		ImTanInumber = 16438000,
		ImCotInumber = 16432000,
		ImCscInumber = 16433000,
		ImCscHInumber = 16434000,
		ImSecInumber = 16435000,
		ImSecHInumber = 16436000,
		BitAndNumber1 = 16426000,
		BitAndNumber2 = 16426001,
		BitOrNumber1 = 16428000,
		BitOrNumber2 = 16428001,
		BitXorNumber1 = 16430000,
		BitXorNumber2 = 16430001,
		BitLShiftNumber = 16427000,
		BitLShiftShiftamount = 16427001,
		BitRShiftNumber = 16429000,
		BitRShiftShiftamount = 16429001,
		PermutationaNumber = 16423000,
		PermutationaNumberchosen = 16423001,
		CombinANumber = 16410000,
		CombinANumberchosen = 16410001,
		XOrLogical1 = 16445000,
		XOrLogical2 = 16445001,
		PDurationRate = 16442000,
		PDurationPv = 16442001,
		PDurationFv = 16442002,
		BaseNumber = 16408000,
		BaseRadix = 16408001,
		BaseMinlength = 16408002,
		DecimalNumber = 16415000,
		DecimalRadix = 16415001,
		DaysEnddate = 16450000,
		DaysStartdate = 16450001,
		BinomDistRangeTrials = 16421000,
		BinomDistRangeProbabilitys = 16421001,
		BinomDistRangeNumbers = 16421002,
		BinomDistRangeNumbers2 = 16421003,
		GammaX = 16422000,
		SkewPNumber1 = 16425000,
		SkewPNumber2 = 16425001,
		PhiX = 16424000,
		RRINper = 16443000,
		RRIPv = 16443001,
		RRIFv = 16443002,
		UnicodeText = 16441000,
		MUnitDimension = 16418000,
		ArabicText = 16407000,
		ISOWeekNumDate = 16451000,
		NumberValueText = 16439000,
		NumberValueDecimalseparator = 16439001,
		NumberValueGroupseparator = 16439002,
		SheetValue = 16448000,
		SheetsReference = 16449000,
		FormulaTextReference = 16446000,
		IsFormulaReference = 16447000,
		IfNAValue = 16444000,
		IfNAValueifna = 16444001,
		CeilingMathNumber = 16409000,
		CeilingMathSignificance = 16409001,
		CeilingMathMode = 16409002,
		FloorMathNumber = 16416000,
		FloorMathSignificance = 16416001,
		FloorMathMode = 16416002,
		ImSinHInumber = 16437000,
		ImCosHInumber = 16431000,
		FunctionFilterXMLXml = 16453000,
		FunctionFilterXMLXpath = 16453001,
		FunctionWebServiceUrl = 16454000,
		EncodeURLText = 16452000,
		FieldDataFieldName = 0x4100 * 1000 + 0,
		RangeCellRefenrece = 0x4101 * 1000 + 0,
		FieldPictureDataFieldName = 0x4102 * 1000 + 0,
		FieldPicturePicturePlacement = 0x4102 * 1000 + 1,
		FieldPictureTargetRange = 0x4102 * 1000 + 2,
		FieldPictureLockAspectRatio = 0x4102 * 1000 + 3,
		FieldPictureOffsetX = 0x4102 * 1000 + 4,
		FieldPictureOffsetY = 0x4102 * 1000 + 5,
		FieldPictureWidth = 0x4102 * 1000 + 6,
		FieldPictureHeight = 0x4102 * 1000 + 7,
		ParameterParameterName = 0x4103 * 1000 + 0,
	}
	#endregion
	#region XtraSpreadsheetFunctionArgumentsDescriptionsLocalizer
	public class XtraSpreadsheetFunctionArgumentsDescriptionsLocalizer : XtraLocalizer<XtraSpreadsheetFunctionArgumentDescriptionStringId> {
		static XtraSpreadsheetFunctionArgumentsDescriptionsLocalizer() {
			SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<XtraSpreadsheetFunctionArgumentDescriptionStringId>(CreateDefaultLocalizer()));
		}
		#region PopulateStringTable
		protected override void PopulateStringTable() {
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CountValue1, "are 1 to 255 arguments that can contain or refer to a variety of different types of data, but only numbers are counted");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CountValue2, "are 1 to 255 arguments that can contain or refer to a variety of different types of data, but only numbers are counted");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.IfLogicaltest, "is any value or expression that can be evaluated to TRUE or FALSE");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.IfValueiftrue, "is the value that is returned if Logical_test is TRUE. If omitted, TRUE is returned. You can nest up to seven IF functions");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.IfValueiffalse, "is the value that is returned if Logical_test is FALSE. If omitted, FALSE is returned");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.IsNAValue, "is the value you want to test. Value can refer to a cell, a formula, or a name that refers to a cell, formula, or value");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.IsErrorValue, "is the value you want to test. Value can refer to a cell, a formula, or a name that refers to a cell, formula, or value");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SumNumber1, "number1,number2,... are 1 to 255 numbers to sum. Logical values and text are ignored in cells, included if typed as arguments");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SumNumber2, "number1,number2,... are 1 to 255 numbers to sum. Logical values and text are ignored in cells, included if typed as arguments");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AverageNumber1, "are 1 to 255 numeric arguments for which you want the average");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AverageNumber2, "are 1 to 255 numeric arguments for which you want the average");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.MinNumber1, "are 1 to 255 numbers, empty cells, logical values, or text numbers for which you want the minimum");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.MinNumber2, "are 1 to 255 numbers, empty cells, logical values, or text numbers for which you want the minimum");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.MaxNumber1, "are 1 to 255 numbers, empty cells, logical values, or text numbers for which you want the maximum");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.MaxNumber2, "are 1 to 255 numbers, empty cells, logical values, or text numbers for which you want the maximum");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.RowReference, "is the cell or a single range of cells for which you want the row number; if omitted, returns the cell containing the ROW function");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ColumnReference, "is the cell or range of contiguous cells for which you want the column number. If omitted, the cell containing the COLUMN function is used");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.NpvRate, "is the rate of discount over the length of one period");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.NpvValue1, "are 1 to 254 payments and income, equally spaced in time and occurring at the end of each period");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.NpvValue2, "are 1 to 254 payments and income, equally spaced in time and occurring at the end of each period");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.StDevNumber1, "are 1 to 255 numbers corresponding to a sample of a population and can be numbers or references that contain numbers");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.StDevNumber2, "are 1 to 255 numbers corresponding to a sample of a population and can be numbers or references that contain numbers");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DollarNumber, "is a number, a reference to a cell containing a number, or a formula that evaluates to a number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DollarDecimals, "is the number of digits to the right of the decimal point. The number is rounded as necessary; if omitted, Decimals = 2");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FixedNumber, "is the number you want to round and convert to text");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FixedDecimals, "is the number of digits to the right of the decimal point. If omitted, Decimals = 2");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FixedNocommas, "is a logical value: do not display commas in the returned text = TRUE; do display commas in the returned text = FALSE or omitted");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SinNumber, "is the angle in radians for which you want the sine. Degrees * PI()/180 = radians");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CosNumber, "is the angle in radians for which you want the cosine");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TanNumber, "is the angle in radians for which you want the tangent. Degrees * PI()/180 = radians");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ATanNumber, "is the tangent of the angle you want");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SqrtNumber, "is the number for which you want the square root");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ExpNumber, "is the exponent applied to the base e. The constant e equals 2.71828182845904, the base of the natural logarithm");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.LnNumber, "is the positive real number for which you want the natural logarithm");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.Log10Number, "is the positive real number for which you want the base-10 logarithm");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AbsNumber, "is the real number for which you want the absolute value");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.IntNumber, "is the real number you want to round down to an integer");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SignNumber, "is any real number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.RoundNumber, "is the number you want to round");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.RoundNumdigits, "is the number of digits to which you want to round. Negative rounds to the left of the decimal point; zero to the nearest integer");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.LookupLookupvalue, "is a value that LOOKUP searches for in Lookup_vector and can be a number, text, a logical value, or a name or reference to a value");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.LookupLookupvector, "is a range that contains only one row or one column of text, numbers, or logical values, placed in ascending order");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.LookupResultvector, "is a range that contains only one row or column, the same size as Lookup_vector");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.IndexArray, "is a range of cells or an array constant.");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.IndexRownum, "selects the row in Array or Reference from which to return a value. If omitted, Column_num is required");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.IndexColumnnum, "selects the column in Array or Reference from which to return a value. If omitted, Row_num is required");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ReptText, "is the text you want to repeat");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ReptNumbertimes, "is a positive number specifying the number of times to repeat text");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.MidText, "is the text string from which you want to extract the characters");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.MidStartnum, "is the position of the first character you want to extract. The first character in Text is 1");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.MidNumchars, "specifies how many characters to return from Text");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.LenText, "is the text whose length you want to find. Spaces count as characters");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ValueText, "is the text enclosed in quotation marks or a reference to a cell containing the text you want to convert");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AndLogical1, "are 1 to 255 conditions you want to test that can be either TRUE or FALSE and can be logical values, arrays, or references");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AndLogical2, "are 1 to 255 conditions you want to test that can be either TRUE or FALSE and can be logical values, arrays, or references");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.OrLogical1, "are 1 to 255 conditions that you want to test that can be either TRUE or FALSE");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.OrLogical2, "are 1 to 255 conditions that you want to test that can be either TRUE or FALSE");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.NotLogical, "is a value or expression that can be evaluated to TRUE or FALSE");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ModNumber, "is the number for which you want to find the remainder after the division is performed");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ModDivisor, "is the number by which you want to divide Number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DCountDatabase, "is the range of cells that makes up the list or database. A database is a list of related data");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DCountField, "is either the label of the column in double quotation marks or a number that represents the column's position in the list");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DCountCriteria, "is the range of cells that contains the conditions you specify. The range includes a column label and one cell below the label for a condition");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DSumDatabase, "is the range of cells that makes up the list or database. A database is a list of related data");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DSumField, "is either the label of the column in double quotation marks or a number that represents the column's position in the list");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DSumCriteria, "is the range of cells that contains the conditions you specify. The range includes a column label and one cell below the label for a condition");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DAverageDatabase, "is the range of cells that makes up the list or database. A database is a list of related data");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DAverageField, "is either the label of the column in double quotation marks or a number that represents the column's position in the list");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DAverageCriteria, "is the range of cells that contains the conditions you specify. The range includes a column label and one cell below the label for a condition");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DMinDatabase, "is the range of cells that makes up the list or database. A database is a list of related data");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DMinField, "is either the label of the column in double quotation marks or a number that represents the column's position in the list");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DMinCriteria, "is the range of cells that contains the conditions you specify. The range includes a column label and one cell below the label for a condition");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DMaxDatabase, "is the range of cells that makes up the list or database. A database is a list of related data");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DMaxField, "is either the label of the column in double quotation marks or a number that represents the column's position in the list");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DMaxCriteria, "is the range of cells that contains the conditions you specify. The range includes a column label and one cell below the label for a condition");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DStDevDatabase, "is the range of cells that makes up the list or database. A database is a list of related data");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DStDevField, "is either the label of the column in double quotation marks or a number that represents the column's position in the list");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DStDevCriteria, "is the range of cells that contains the conditions you specify. The range includes a column label and one cell below the label for a condition");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.VarNumber1, "are 1 to 255 numeric arguments corresponding to a sample of a population");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.VarNumber2, "are 1 to 255 numeric arguments corresponding to a sample of a population");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DVarDatabase, "is the range of cells that makes up the list or database. A database is a list of related data");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DVarField, "is either the label of the column in double quotation marks or a number that represents the column's position in the list");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DVarCriteria, "is the range of cells that contains the conditions you specify. The range includes a column label and one cell below the label for a condition");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TextValue, "is a number, a formula that evaluates to a numeric value, or a reference to a cell containing a numeric value");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TextFormattext, "is a number format in text form from the Category box on the Number tab in the Format Cells dialog box (not General)");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.LinestKnownys, "is the set of y-values you already know in the relationship y = mx + b");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.LinestKnownxs, "is an optional set of x-values that you may already know in the relationship y = mx + b");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.LinestConst, "is a logical value: the constant b is calculated normally if Const = TRUE or omitted; b is set equal to 0 if Const = FALSE");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.LinestStats, "is a logical value: return additional regression statistics = TRUE; return m-coefficients and the constant b = FALSE or omitted");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TrendKnownys, "is a range or array of y-values you already know in the relationship y = mx + b");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TrendKnownxs, "is an optional range or array of x-values that you know in the relationship y = mx + b, an array the same size as Known_y's");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TrendNewxs, "is a range or array of new x-values for which you want TREND to return corresponding y-values");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TrendConst, "is a logical value: the constant b is calculated normally if Const = TRUE or omitted; b is set equal to 0 if Const = FALSE");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.LogestKnownys, "is the set of y-values you already know in the relationship y = b*m^x");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.LogestKnownxs, "is an optional set of x-values that you may already know in the relationship y = b*m^x");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.LogestConst, "is a logical value: the constant b is calculated normally if Const = TRUE or omitted; b is set equal to 1 if Const = FALSE");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.LogestStats, "is a logical value: return additional regression statistics = TRUE; return m-coefficients and the constant b = FALSE or omitted");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.GrowthKnownys, "is the set of y-values you already know in the relationship y = b*m^x, an array or range of positive numbers");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.GrowthKnownxs, "is an optional set of x-values that you may already know in the relationship y = b*m^x, an array or range the same size as Known_y's");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.GrowthNewxs, "are new x-values for which you want GROWTH to return corresponding y-values");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.GrowthConst, "is a logical value: the constant b is calculated normally if Const = TRUE; b is set equal to 1 if Const = FALSE or omitted");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PvRate, "is the interest rate per period. For example, use 6%/4 for quarterly payments at 6% APR");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PvNper, "is the total number of payment periods in an investment");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PvPmt, "is the payment made each period and cannot change over the life of the investment");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PvFv, "is the future value, or a cash balance you want to attain after the last payment is made");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PvType, "is a logical value: payment at the beginning of the period = 1; payment at the end of the period = 0 or omitted");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FvRate, "is the interest rate per period. For example, use 6%/4 for quarterly payments at 6% APR");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FvNper, "is the total number of payment periods in the investment");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FvPmt, "is the payment made each period; it cannot change over the life of the investment");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FvPv, "is the present value, or the lump-sum amount that a series of future payments is worth now. If omitted, Pv = 0");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FvType, "is a value representing the timing of payment: payment at the beginning of the period = 1; payment at the end of the period = 0 or omitted");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.NPerRate, "is the interest rate per period. For example, use 6%/4 for quarterly payments at 6% APR");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.NPerPmt, "is the payment made each period; it cannot change over the life of the investment");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.NPerPv, "is the present value, or the lump-sum amount that a series of future payments is worth now");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.NPerFv, "is the future value, or a cash balance you want to attain after the last payment is made. If omitted, zero is used");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.NPerType, "is a logical value: payment at the beginning of the period = 1; payment at the end of the period = 0 or omitted");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PmtRate, "is the interest rate per period for the loan. For example, use 6%/4 for quarterly payments at 6% APR");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PmtNper, "is the total number of payments for the loan");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PmtPv, "is the present value: the total amount that a series of future payments is worth now");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PmtFv, "is the future value, or a cash balance you want to attain after the last payment is made, 0 (zero) if omitted");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PmtType, "is a logical value: payment at the beginning of the period = 1; payment at the end of the period = 0 or omitted");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.RateNper, "is the total number of payment periods for the loan or investment");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.RatePmt, "is the payment made each period and cannot change over the life of the loan or investment");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.RatePv, "is the present value: the total amount that a series of future payments is worth now");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.RateFv, "is the future value, or a cash balance you want to attain after the last payment is made. If omitted, uses Fv = 0");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.RateType, "is a logical value: payment at the beginning of the period = 1; payment at the end of the period = 0 or omitted");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.RateGuess, "is your guess for what the rate will be; if omitted, Guess = 0.1 (10 percent)");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.MirrValues, "is an array or a reference to cells that contain numbers that represent a series of payments (negative) and income (positive) at regular periods");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.MirrFinancerate, "is the interest rate you pay on the money used in the cash flows");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.MirrReinvestrate, "is the interest rate you receive on the cash flows as you reinvest them");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.IrrValues, "is an array or a reference to cells that contain numbers for which you want to calculate the internal rate of return");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.IrrGuess, "is a number that you guess is close to the result of IRR; 0.1 (10 percent) if omitted");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.MatchLookupvalue, "is the value you use to find the value you want in the array, a number, text, or logical value, or a reference to one of these");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.MatchLookuparray, "is a contiguous range of cells containing possible lookup values, an array of values, or a reference to an array");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.MatchMatchtype, "is a number 1, 0, or -1 indicating which value to return.");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DateYear, "is a number from 1900 to 9999 in Spreadsheet for Windows or from 1904 to 9999 in Spreadsheet for the Macintosh");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DateMonth, "is a number from 1 to 12 representing the month of the year");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DateDay, "is a number from 1 to 31 representing the day of the month");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TimeHour, "is a number from 0 to 23 representing the hour");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TimeMinute, "is a number from 0 to 59 representing the minute");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TimeSecond, "is a number from 0 to 59 representing the second");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DaySerialnumber, "is a number in the date-time code used by Spreadsheet");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.MonthSerialnumber, "is a number in the date-time code used by Spreadsheet");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.YearSerialnumber, "is a number in the date-time code used by Spreadsheet");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.WeekDaySerialnumber, "is a number that represents a date");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.WeekDayReturntype, "is a number: for Sunday=1 through Saturday=7, use 1; for Monday=1 through Sunday=7, use 2; for Monday=0 through Sunday=6, use 3");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.HourSerialnumber, "is a number in the date-time code used by Spreadsheet, or text in time format, such as 16:48:00 or 4:48:00 PM");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.MinuteSerialnumber, "is a number in the date-time code used by Spreadsheet or text in time format, such as 16:48:00 or 4:48:00 PM");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SecondSerialnumber, "is a number in the date-time code used by Spreadsheet or text in time format, such as 16:48:23 or 4:48:47 PM");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AreasReference, "is a reference to a cell or range of cells and can refer to multiple areas");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.RowsArray, "is an array, an array formula, or a reference to a range of cells for which you want the number of rows");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ColumnsArray, "is an array or array formula, or a reference to a range of cells for which you want the number of columns");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.OffsetReference, "is the reference from which you want to base the offset, a reference to a cell or range of adjacent cells");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.OffsetRows, "is the number of rows, up or down, that you want the upper-left cell of the result to refer to");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.OffsetCols, "is the number of columns, to the left or right, that you want the upper-left cell of the result to refer to");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.OffsetHeight, "is the height, in number of rows, that you want the result to be, the same height as Reference if omitted");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.OffsetWidth, "is the width, in number of columns, that you want the result to be, the same width as Reference if omitted");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SearchFindtext, "is the text you want to find. You can use the ? and * wildcard characters; use ~? and ~* to find the ? and * characters");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SearchWithintext, "is the text in which you want to search for Find_text");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SearchStartnum, "is the character number in Within_text, counting from the left, at which you want to start searching. If omitted, 1 is used");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TransposeArray, "is a range of cells on a worksheet or an array of values that you want to transpose");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TypeValue, "can be any value");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ATan2Xnum, "is the x-coordinate of the point");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ATan2Ynum, "is the y-coordinate of the point");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ASinNumber, "is the sine of the angle you want and must be from -1 to 1");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ACosNumber, "is the cosine of the angle you want and must be from -1 to 1");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ChooseIndexnum, "specifies which value argument is selected. Index_num must be between 1 and 254, or a formula or a reference to a number between 1 and 254");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ChooseValue1, "are 1 to 254 numbers, cell references, defined names, formulas, functions, or text arguments from which CHOOSE selects");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ChooseValue2, "are 1 to 254 numbers, cell references, defined names, formulas, functions, or text arguments from which CHOOSE selects");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.HLookupLookupvalue, "is the value to be found in the first row of the table and can be a value, a reference, or a text string");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.HLookupTablearray, "is a table of text, numbers, or logical values in which data is looked up. Table_array can be a reference to a range or a range name");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.HLookupRowindexnum, "is the row number in table_array from which the matching value should be returned. The first row of values in the table is row 1");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.HLookupRangelookup, "is a logical value: to find the closest match in the top row (sorted in ascending order) = TRUE or omitted; find an exact match = FALSE");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.VLookupLookupvalue, "is the value to be found in the first column of the table, and can be a value, a reference, or a text string");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.VLookupTablearray, "is a table of text, numbers, or logical values, in which data is retrieved. Table_array can be a reference to a range or a range name");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.VLookupColindexnum, "is the column number in table_array from which the matching value should be returned. The first column of values in the table is column 1");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.VLookupRangelookup, "is a logical value: to find the closest match in the first column (sorted in ascending order) = TRUE or omitted; find an exact match = FALSE");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.IsRefValue, "is the value you want to test. Value can refer to a cell, a formula, or a name that refers to a cell, formula, or value");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.LogNumber, "is the positive real number for which you want the logarithm");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.LogBase, "is the base of the logarithm; 10 if omitted");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CharNumber, "is a number between 1 and 255 specifying which character you want");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.LowerText, "is the text you want to convert to lowercase. Characters in Text that are not letters are not changed");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.UpperText, "is the text you want converted to uppercase, a reference or a text string");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ProperText, "is text enclosed in quotation marks, a formula that returns text, or a reference to a cell containing text to partially capitalize");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.LeftText, "is the text string containing the characters you want to extract");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.LeftNumchars, "specifies how many characters you want LEFT to extract; 1 if omitted");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.RightText, "is the text string that contains the characters you want to extract");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.RightNumchars, "specifies how many characters you want to extract, 1 if omitted");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ExactText1, "is the first text string");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ExactText2, "is the second text string");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TrimText, "is the text from which you want spaces removed");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ReplaceOldtext, "is text in which you want to replace some characters");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ReplaceStartnum, "is the position of the character in Old_text that you want to replace with New_text");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ReplaceNumchars, "is the number of characters in Old_text that you want to replace");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ReplaceNewtext, "is the text that will replace characters in Old_text");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SubstituteText, "is the text or the reference to a cell containing text in which you want to substitute characters");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SubstituteOldtext, "is the existing text you want to replace. If the case of Old_text does not match the case of text, SUBSTITUTE will not replace the text");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SubstituteNewtext, "is the text you want to replace Old_text with");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SubstituteInstancenum, "specifies which occurrence of Old_text you want to replace. If omitted, every instance of Old_text is replaced");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CodeText, "is the text for which you want the code of the first character");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FindFindtext, "is the text you want to find. Use double quotes (empty text) to match the first character in Within_text; wildcard characters not allowed");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FindWithintext, "is the text containing the text you want to find");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FindStartnum, "specifies the character at which to start the search. The first character in Within_text is character number 1. If omitted, Start_num = 1");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CellInfotype, "is a text value that specifies what type of cell information you want.");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CellReference, "is the cell that you want information about");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.IsErrValue, "is the value you want to test. Value can refer to a cell, a formula, or a name that refers to a cell, formula, or value");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.IsTextValue, "is the value you want to test. Value can refer to a cell, a formula, or a name that refers to a cell, formula, or value");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.IsNumberValue, "is the value you want to test. Value can refer to a cell, a formula, or a name that refers to a cell, formula, or value");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.IsBlankValue, "is the cell or a name that refers to the cell you want to test");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TValue, "is the value to test");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.NValue, "is the value you want converted");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DateValueDatetext, "is text that represents a date in a Spreadsheet date format, between 1/1/1900 (Windows) or 1/1/1904 (Macintosh) and 12/31/9999");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TimeValueTimetext, "is a text string that gives a time in any one of the Spreadsheet time formats (date information in the string is ignored)");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SlnCost, "is the initial cost of the asset");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SlnSalvage, "is the salvage value at the end of the life of the asset");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SlnLife, "is the number of periods over which the asset is being depreciated (sometimes called the useful life of the asset)");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SydCost, "is the initial cost of the asset");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SydSalvage, "is the salvage value at the end of the life of the asset");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SydLife, "is the number of periods over which the asset is being depreciated (sometimes called the useful life of the asset)");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SydPer, "is the period and must use the same units as Life");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DdbCost, "is the initial cost of the asset");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DdbSalvage, "is the salvage value at the end of the life of the asset");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DdbLife, "is the number of periods over which the asset is being depreciated (sometimes called the useful life of the asset)");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DdbPeriod, "is the period for which you want to calculate the depreciation. Period must use the same units as Life");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DdbFactor, "is the rate at which the balance declines. If Factor is omitted, it is assumed to be 2 (the double-declining balance method)");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.IndirectReftext, "is a reference to a cell that contains an A1- or R1C1-style reference, a name defined as a reference, or a reference to a cell as a text string");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.IndirectA1, "is a logical value that specifies the type of reference in Ref_text: R1C1-style = FALSE; A1-style = TRUE or omitted");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CallRegisterid, "is the value returned by a previously executed REGISTER or REGISTER.ID function");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CallArgument1, "are the arguments to be passed to the procedure");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CleanText, "is any worksheet information from which you want to remove nonprintable characters");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.MDetermArray, "is a numeric array with an equal number of rows and columns, either a cell range or an array constant");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.MInverseArray, "is a numeric array with an equal number of rows and columns, either a cell range or an array constant");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.MMultArray1, "is the first array of numbers to multiply and must have the same number of columns as Array2 has rows");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.MMultArray2, "is the first array of numbers to multiply and must have the same number of columns as Array2 has rows");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.IpmtRate, "is the interest rate per period. For example, use 6%/4 for quarterly payments at 6% APR");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.IpmtPer, "is the period for which you want to find the interest and must be in the range 1 to Nper");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.IpmtNper, "is the total number of payment periods in an investment");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.IpmtPv, "is the present value, or the lump-sum amount that a series of future payments is worth now");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.IpmtFv, "is the future value, or a cash balance you want to attain after the last payment is made. If omitted, Fv = 0");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.IpmtType, "is a logical value representing the timing of payment: at the end of the period = 0 or omitted, at the beginning of the period = 1");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PpmtRate, "is the interest rate per period. For example, use 6%/4 for quarterly payments at 6% APR");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PpmtPer, "specifies the period and must be in the range 1 to nper");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PpmtNper, "is the total number of payment periods in an investment");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PpmtPv, "is the present value: the total amount that a series of future payments is worth now");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PpmtFv, "is the future value, or cash balance you want to attain after the last payment is made");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PpmtType, "is a logical value: payment at the beginning of the period = 1; payment at the end of the period = 0 or omitted");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CountAValue1, "are 1 to 255 arguments representing the values and cells you want to count. Values can be any type of information");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CountAValue2, "are 1 to 255 arguments representing the values and cells you want to count. Values can be any type of information");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ProductNumber1, "are 1 to 255 numbers, logical values, or text representations of numbers that you want to multiply");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ProductNumber2, "are 1 to 255 numbers, logical values, or text representations of numbers that you want to multiply");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FactNumber, "is the nonnegative number you want the factorial of");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DProductDatabase, "is the range of cells that makes up the list or database. A database is a list of related data");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DProductField, "is either the label of the column in double quotation marks or a number that represents the column's position in the list");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DProductCriteria, "is the range of cells that contains the conditions you specify. The range includes a column label and one cell below the label for a condition");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.IsNonTextValue, "is the value you want tested: a cell; a formula; or a name referring to a cell, formula, or value");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.StDevPNumber1, "are 1 to 255 numbers corresponding to a population and can be numbers or references that contain numbers");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.StDevPNumber2, "are 1 to 255 numbers corresponding to a population and can be numbers or references that contain numbers");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.VarPNumber1, "are 1 to 255 numeric arguments corresponding to a population");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.VarPNumber2, "are 1 to 255 numeric arguments corresponding to a population");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DStDevPDatabase, "is the range of cells that makes up the list or database. A database is a list of related data");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DStDevPField, "is either the label of the column in double quotation marks or a number that represents the column's position in the list");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DStDevPCriteria, "is the range of cells that contains the conditions you specify. The range includes a column label and one cell below the label for a condition");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DVarPDatabase, "is the range of cells that makes up the list or database. A database is a list of related data");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DVarPField, "is either the label of the column in double quotation marks or a number that represents the column's position in the list");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DVarPCriteria, "is the range of cells that contains the conditions you specify. The range includes a column label and one cell below the label for a condition");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TruncNumber, "is the number you want to truncate");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TruncNumdigits, "is a number specifying the precision of the truncation, 0 (zero) if omitted");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.IsLogicalValue, "is the value you want to test. Value can refer to a cell, a formula, or a name that refers to a cell, formula, or value");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DCountADatabase, "is the range of cells that makes up the list or database. A database is a list of related data");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DCountAField, "is either the label of the column in double quotation marks or a number that represents the column's position in the list");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DCountACriteria, "is the range of cells that contains the conditions you specify. The range includes a column label and one cell below the label for a condition");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FindBFindtext, "is the text you want to find");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FindBWithintext, "is the text containing the text you want to find");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FindBStartnum, "specifies the character at which to start the search");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SearchBFindtext, "is the text you want to find");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SearchBWithintext, "is the text in which you want to search for find_text");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SearchBStartnum, "is the character number in within_text, counting from the left, at which you want to start searching");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ReplaceBOldtext, "is text in which you want to replace some characters");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ReplaceBStartnum, "is the position of the character in old_text that you want to replace with new_text");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ReplaceBNumbytes, "is the number of characters in old_text that you want to replace with new_text");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ReplaceBNewtext, "is the text that will replace characters in old_text");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.LeftBText, "is the text string containing the characters you want to extract");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.LeftBNumbytes, "specifies how many characters you want LEFT to return");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.RightBText, "is the text string containing the characters you want to extract");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.RightBNumbytes, "specifies how many characters you want to extract");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.MidBText, "is the text string containing the characters you want to extract");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.MidBStartnum, "is the position of the first character you want to extract in text");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.MidBNumbytes, "specifies how many characters to return from text");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.LenBText, "is the text whose length you want to find");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.RoundUpNumber, "is any real number that you want rounded up");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.RoundUpNumdigits, "is the number of digits to which you want to round. Negative rounds to the left of the decimal point; zero or omitted, to the nearest integer");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.RoundDownNumber, "is any real number that you want rounded down");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.RoundDownNumdigits, "is the number of digits to which you want to round. Negative rounds to the left of the decimal point; zero or omitted, to the nearest integer");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AscText, "is a text, or a reference to a cell containing a text");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.RankNumber, "is the number for which you want to find the rank");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.RankRef, "is an array of, or a reference to, a list of numbers. Nonnumeric values are ignored");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.RankOrder, "is a number: rank in the list sorted descending = 0 or omitted; rank in the list sorted ascending = any nonzero value");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AddressRownum, "is the row number to use in the cell reference: Row_number = 1 for row 1");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AddressColumnnum, "is the column number to use in the cell reference. For example, Column_number = 4 for column D");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AddressAbsnum, "specifies the reference type: absolute = 1; absolute row/relative column = 2; relative row/absolute column = 3; relative = 4");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AddressA1, "is a logical value that specifies the reference style: A1 style = 1 or TRUE; R1C1 style = 0 or FALSE");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AddressSheettext, "is text specifying the name of the worksheet to be used as the external reference");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.Days360Startdate, "start_date and end_date are the two dates between which you want to know the number of days");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.Days360Enddate, "start_date and end_date are the two dates between which you want to know the number of days");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.Days360Method, "is a logical value specifying the calculation method: U.S. (NASD) = FALSE or omitted; European = TRUE.");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.VdbCost, "is the initial cost of the asset");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.VdbSalvage, "is the salvage value at the end of the life of the asset");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.VdbLife, "is the number of periods over which the asset is being depreciated (sometimes called the useful life of the asset)");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.VdbStartperiod, "is the starting period for which you want to calculate the depreciation, in the same units as Life");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.VdbEndperiod, "is the ending period for which you want to calculate the depreciation, in the same units as Life");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.VdbFactor, "is the rate at which the balance declines, 2 (double-declining balance) if omitted");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.VdbNoswitch, "switch to straight-line depreciation when depreciation is greater than the declining balance = FALSE or omitted; do not switch = TRUE");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.MedianNumber1, "are 1 to 255 numbers or names, arrays, or references that contain numbers for which you want the median");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.MedianNumber2, "are 1 to 255 numbers or names, arrays, or references that contain numbers for which you want the median");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SumProductArray1, "are 2 to 255 arrays for which you want to multiply and then add components. All arrays must have the same dimensions");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SumProductArray2, "are 2 to 255 arrays for which you want to multiply and then add components. All arrays must have the same dimensions");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SumProductArray3, "are 2 to 255 arrays for which you want to multiply and then add components. All arrays must have the same dimensions");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SinHNumber, "is any real number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CosHNumber, "is any real number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TanHNumber, "is any real number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ASinHNumber, "is any real number equal to or greater than 1");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ACosHNumber, "is any real number equal to or greater than 1");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ATanHNumber, "is any real number between -1 and 1 excluding -1 and 1");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DGetDatabase, "is the range of cells that makes up the list or database. A database is a list of related data");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DGetField, "is either the label of the column in double quotation marks or a number that represents the column's position in the list");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DGetCriteria, "is the range of cells that contains the conditions you specify. The range includes a column label and one cell below the label for a condition");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.InfoTypetext, "is text specifying what type of information you want returned.");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DbCost, "is the initial cost of the asset");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DbSalvage, "is the salvage value at the end of the life of the asset");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DbLife, "is the number of periods over which the asset is being depreciated (sometimes called the useful life of the asset)");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DbPeriod, "is the period for which you want to calculate the depreciation. Period must use the same units as Life");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DbMonth, "is the number of months in the first year. If month is omitted, it is assumed to be 12");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FrequencyDataarray, "is an array of or reference to a set of values for which you want to count frequencies (blanks and text are ignored)");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FrequencyBinsarray, "is an array of or reference to intervals into which you want to group the values in data_array");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ErrorTypeErrorval, "is the error value for which you want the identifying number, and can be an actual error value or a reference to a cell containing an error value");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.RegisterIdModuletext, "is text specifying the name of the DLL that contains the function in Spreadsheet for Windows");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.RegisterIdProcedure, "is text specifying the name of the function in the DLL in Spreadsheet for Windows");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.RegisterIdTypetext, "is text encoding input and return data types and can be omitted if the function is already registered.");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AvedevNumber1, "are 1 to 255 arguments for which you want the average of the absolute deviations");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AvedevNumber2, "are 1 to 255 arguments for which you want the average of the absolute deviations");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BetaDistCompatibilityX, "is the value between A and B at which to evaluate the function");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BetaDistCompatibilityAlpha, "is a parameter to the distribution and must be greater than 0");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BetaDistCompatibilityBeta, "is a parameter to the distribution and must be greater than 0");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BetaDistCompatibilityA, "is an optional lower bound to the interval of x. If omitted, A = 0");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BetaDistCompatibilityB, "is an optional upper bound to the interval of x. If omitted, B = 1");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.GammaLnX, "is the value for which you want to calculate GAMMALN, a positive number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BetaInvProbability, "is a probability associated with the beta distribution");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BetaInvAlpha, "is a parameter to the distribution and must be greater than 0");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BetaInvBeta, "is a parameter to the distribution and must be greater than 0");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BetaInvA, "is an optional lower bound to the interval of x. If omitted, A = 0");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BetaInvB, "is an optional upper bound to the interval of x. If omitted, B = 1");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BinomDistNumbers, "is the number of successes in trials");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BinomDistTrials, "is the number of independent trials");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BinomDistProbabilitys, "is the probability of success on each trial");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BinomDistCumulative, "is a logical value: for the cumulative distribution function, use TRUE; for the probability mass function, use FALSE");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ChiDistX, "is the value at which you want to evaluate the distribution, a nonnegative number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ChiDistDegfreedom, "is the number of degrees of freedom, a number between 1 and 10^10, excluding 10^10");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ChiInvProbability, "is a probability associated with the chi-squared distribution, a value between 0 and 1 inclusive");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ChiInvDegfreedom, "is the number of degrees of freedom, a number between 1 and 10^10, excluding 10^10");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CombinNumber, "is the total number of items");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CombinNumberchosen, "is the number of items in each combination");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ConfidenceAlpha, "is the significance level used to compute the confidence level, a number greater than 0 and less than 1");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ConfidenceStandarddev, "is the population standard deviation for the data range and is assumed to be known. Standard_dev must be greater than 0");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ConfidenceSize, "is the sample size");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CritBinomTrials, "is the number of Bernoulli trials");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CritBinomProbabilitys, "is the probability of success on each trial, a number between 0 and 1 inclusive");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CritBinomAlpha, "is the criterion value, a number between 0 and 1 inclusive");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.EvenNumber, "is the value to round");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ExponDistX, "is the value of the function, a nonnegative number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ExponDistLambda, "is the parameter value, a positive number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ExponDistCumulative, "is a logical value for the function to return: the cumulative distribution function = TRUE; the probability density function = FALSE");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FDistX, "is the value at which to evaluate the function, a nonnegative number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FDistDegfreedom1, "is the numerator degrees of freedom, a number between 1 and 10^10, excluding 10^10");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FDistDegfreedom2, "is the denominator degrees of freedom, a number between 1 and 10^10, excluding 10^10");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FInvProbability, "is a probability associated with the F cumulative distribution, a number between 0 and 1 inclusive");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FInvDegfreedom1, "is the numerator degrees of freedom, a number between 1 and 10^10, excluding 10^10");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FInvDegfreedom2, "is the denominator degrees of freedom, a number between 1 and 10^10, excluding 10^10");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FisherX, "is the value for which you want the transformation, a number between -1 and 1, excluding -1 and 1");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FisherInvY, "is the value for which you want to perform the inverse of the transformation");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FloorNumber, "is the numeric value you want to round");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FloorSignificance, "is the multiple to which you want to round. Number and Significance must either both be positive or both be negative");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.GammaDistX, "is the value at which you want to evaluate the distribution, a nonnegative number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.GammaDistAlpha, "is a parameter to the distribution, a positive number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.GammaDistBeta, "is a parameter to the distribution, a positive number. If beta = 1, GAMMADIST returns the standard gamma distribution");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.GammaDistCumulative, "is a logical value: return the cumulative distribution function = TRUE; return the probability mass function = FALSE or omitted");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.GammaInvProbability, "is the probability associated with the gamma distribution, a number between 0 and 1, inclusive");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.GammaInvAlpha, "is a parameter to the distribution, a positive number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.GammaInvBeta, "is a parameter to the distribution, a positive number. If beta = 1, GAMMAINV returns the inverse of the standard gamma distribution");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CeilingNumber, "is the value you want to round");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CeilingSignificance, "is the multiple to which you want to round");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.HypGeomDistSamples, "is the number of successes in the sample");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.HypGeomDistNumbersample, "is the size of the sample");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.HypGeomDistPopulations, "is the number of successes in the population");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.HypGeomDistNumberpop, "is the population size");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.LogNormDistCompatibilityX, "is the value at which to evaluate the function, a positive number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.LogNormDistCompatibilityMean, "is the mean of ln(x)");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.LogNormDistCompatibilityStandarddev, "is the standard deviation of ln(x), a positive number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.LogInvCompatibilityProbability, "is a probability associated with the lognormal distribution, a number between 0 and 1, inclusive");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.LogInvCompatibilityMean, "is the mean of ln(x)");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.LogInvCompatibilityStandarddev, "is the standard deviation of ln(x), a positive number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.NegBinomDistNumberf, "is the number of failures");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.NegBinomDistNumbers, "is the threshold number of successes");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.NegBinomDistProbabilitys, "is the probability of a success; a number between 0 and 1");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.NormDistCompatibilityX, "is the value for which you want the distribution");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.NormDistCompatibilityMean, "is the arithmetic mean of the distribution");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.NormDistCompatibilityStandarddev, "is the standard deviation of the distribution, a positive number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.NormDistCompatibilityCumulative, "is a logical value: for the cumulative distribution function, use TRUE; for the probability density function, use FALSE");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.NormSDistCompatibilityZ, "is the value for which you want the distribution");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.NormInvCompatibilityProbability, "is a probability corresponding to the normal distribution, a number between 0 and 1 inclusive");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.NormInvCompatibilityMean, "is the arithmetic mean of the distribution");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.NormInvCompatibilityStandarddev, "is the standard deviation of the distribution, a positive number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.NormSInvCompatibilityProbability, "is a probability corresponding to the normal distribution, a number between 0 and 1 inclusive");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.StandardizeX, "is the value you want to normalize");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.StandardizeMean, "is the arithmetic mean of the distribution");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.StandardizeStandarddev, "is the standard deviation of the distribution, a positive number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.OddNumber, "is the value to round");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PermutNumber, "is the total number of objects");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PermutNumberchosen, "is the number of objects in each permutation");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PoissonX, "is the number of events");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PoissonMean, "is the expected numeric value, a positive number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PoissonCumulative, "is a logical value: for the cumulative Poisson probability, use TRUE; for the Poisson probability mass function, use FALSE");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TDistX, "is the numeric value at which to evaluate the distribution");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TDistDegfreedom, "is an integer indicating the number of degrees of freedom that characterize the distribution");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TDistTails, "specifies the number of distribution tails to return: one-tailed distribution = 1; two-tailed distribution = 2");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.WeibullX, "is the value at which to evaluate the function, a nonnegative number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.WeibullAlpha, "is a parameter to the distribution, a positive number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.WeibullBeta, "is a parameter to the distribution, a positive number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.WeibullCumulative, "is a logical value: for the cumulative distribution function, use TRUE; for the probability mass function, use FALSE");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SumXMY2Arrayx, "is the first range or array of values and can be a number or name, array, or reference that contains numbers");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SumXMY2Arrayy, "is the second range or array of values and can be a number or name, array, or reference that contains numbers");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SumX2MY2Arrayx, "is the first range or array of numbers and can be a number or name, array, or reference that contains numbers");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SumX2MY2Arrayy, "is the second range or array of numbers and can be a number or name, array, or reference that contains numbers");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SumX2PY2Arrayx, "is the first range or array of numbers and can be a number or name, array, or reference that contains numbers");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SumX2PY2Arrayy, "is the second range or array of numbers and can be a number or name, array, or reference that contains numbers");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ChiTestActualrange, "is the range of data that contains observations to test against expected values");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ChiTestExpectedrange, "is the range of data that contains the ratio of the product of row totals and column totals to the grand total");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CorrelArray1, "is a cell range of values. The values should be numbers, names, arrays, or references that contain numbers");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CorrelArray2, "is a second cell range of values. The values should be numbers, names, arrays, or references that contain numbers");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CovarArray1, "is the first cell range of integers and must be numbers, arrays, or references that contain numbers");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CovarArray2, "is the second cell range of integers and must be numbers, arrays, or references that contain numbers");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ForecastX, "is the data point for which you want to predict a value and must be a numeric value");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ForecastKnownys, "is the dependent array or range of numeric data");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ForecastKnownxs, "is the independent array or range of numeric data. The variance of Known_x's must not be zero");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FTestArray1, "is the first array or range of data and can be numbers or names, arrays, or references that contain numbers (blanks are ignored)");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FTestArray2, "is the second array or range of data and can be numbers or names, arrays, or references that contain numbers (blanks are ignored)");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.InterceptKnownys, "is the dependent set of observations or data and can be numbers or names, arrays, or references that contain numbers");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.InterceptKnownxs, "is the independent set of observations or data and can be numbers or names, arrays, or references that contain numbers");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PearsonArray1, "is a set of independent values");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PearsonArray2, "is a set of dependent values");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.RsqKnownys, "is an array or range of data points and can be numbers or names, arrays, or references that contain numbers");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.RsqKnownxs, "is an array or range of data points and can be numbers or names, arrays, or references that contain numbers");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.StEYXKnownys, "is an array or range of dependent data points and can be numbers or names, arrays, or references that contain numbers");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.StEYXKnownxs, "is an array or range of independent data points and can be numbers or names, arrays, or references that contain numbers");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SlopeKnownys, "is an array or cell range of numeric dependent data points and can be numbers or names, arrays, or references that contain numbers");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SlopeKnownxs, "is the set of independent data points and can be numbers or names, arrays, or references that contain numbers");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TTestArray1, "is the first data set");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TTestArray2, "is the second data set");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TTestTails, "specifies the number of distribution tails to return: one-tailed distribution = 1; two-tailed distribution = 2");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TTestType, "is the kind of t-test: paired = 1, two-sample equal variance (homoscedastic) = 2, two-sample unequal variance = 3");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ProbXrange, "is the range of numeric values of x with which there are associated probabilities");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ProbProbrange, "is the set of probabilities associated with values in X_range, values between 0 and 1 and excluding 0");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ProbLowerlimit, "is the lower bound on the value for which you want a probability");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ProbUpperlimit, "is the optional upper bound on the value. If omitted, PROB returns the probability that X_range values are equal to Lower_limit");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DevSqNumber1, "are 1 to 255 arguments, or an array or array reference, on which you want DEVSQ to calculate");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DevSqNumber2, "are 1 to 255 arguments, or an array or array reference, on which you want DEVSQ to calculate");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.GeomeanNumber1, "are 1 to 255 numbers or names, arrays, or references that contain numbers for which you want the mean");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.GeomeanNumber2, "are 1 to 255 numbers or names, arrays, or references that contain numbers for which you want the mean");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.HarMeanNumber1, "are 1 to 255 numbers or names, arrays, or references that contain numbers for which you want the harmonic mean");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.HarMeanNumber2, "are 1 to 255 numbers or names, arrays, or references that contain numbers for which you want the harmonic mean");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SumSqNumber1, "are 1 to 255 numbers, arrays, names, or references to arrays for which you want the sum of the squares");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SumSqNumber2, "are 1 to 255 numbers, arrays, names, or references to arrays for which you want the sum of the squares");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.KurtNumber1, "are 1 to 255 numbers or names, arrays, or references that contain numbers for which you want the kurtosis");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.KurtNumber2, "are 1 to 255 numbers or names, arrays, or references that contain numbers for which you want the kurtosis");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SkewNumber1, "are 1 to 255 numbers or names, arrays, or references that contain numbers for which you want the skewness");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SkewNumber2, "are 1 to 255 numbers or names, arrays, or references that contain numbers for which you want the skewness");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ZTestArray, "is the array or range of data against which to test X");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ZTestX, "is the value to test");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ZTestSigma, "is the population (known) standard deviation. If omitted, the sample standard deviation is used");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.LargeArray, "is the array or range of data for which you want to determine the k-th largest value");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.LargeK, "is the position (from the largest) in the array or cell range of the value to return");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SmallArray, "is an array or range of numerical data for which you want to determine the k-th smallest value");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SmallK, "is the position (from the smallest) in the array or range of the value to return");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.QuartileArray, "is the array or cell range of numeric values for which you want the quartile value");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.QuartileQuart, "is a number: minimum value = 0; 1st quartile = 1; median value = 2; 3rd quartile = 3; maximum value = 4");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PercentileArray, "is the array or range of data that defines relative standing");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PercentileK, "is the percentile value that is between 0 through 1, inclusive");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PercentRankArray, "is the array or range of data with numeric values that defines relative standing");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PercentRankX, "is the value for which you want to know the rank");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PercentRankSignificance, "is an optional value that identifies the number of significant digits for the returned percentage, three digits if omitted (0.xxx%)");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ModeNumber1, "are 1 to 255 numbers, or names, arrays, or references that contain numbers for which you want the mode");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ModeNumber2, "are 1 to 255 numbers, or names, arrays, or references that contain numbers for which you want the mode");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TrimmeanArray, "is the range or array of values to trim and average");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TrimmeanPercent, "is the fractional number of data points to exclude from the top and bottom of the data set");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TinvProbability, "is the probability associated with the two-tailed Student's t-distribution, a number between 0 and 1 inclusive");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TinvDegfreedom, "is a positive integer indicating the number of degrees of freedom to characterize the distribution");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ConcatenateText1, "are 1 to 255 text strings to be joined into a single text string and can be text strings, numbers, or single-cell references");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ConcatenateText2, "are 1 to 255 text strings to be joined into a single text string and can be text strings, numbers, or single-cell references");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PowerNumber, "is the base number, any real number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PowerPower, "is the exponent, to which the base number is raised");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.RadiansAngle, "is an angle in degrees that you want to convert");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DegreesAngle, "is the angle in radians that you want to convert");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SubtotalFunctionnum, "is the number 1 to 11 that specifies the summary function for the subtotal.");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SubtotalRef1, "are 1 to 254 ranges or references for which you want the subtotal");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SumIfRange, "is the range of cells you want evaluated");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SumIfCriteria, "is the condition or criteria in the form of a number, expression, or text that defines which cells will be added");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SumIfSumrange, "are the actual cells to sum. If omitted, the cells in range are used");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CountIfRange, "is the range of cells from which you want to count nonblank cells");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CountIfCriteria, "is the condition in the form of a number, expression, or text that defines which cells will be counted");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CountBlankRange, "is the range from which you want to count the empty cells");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.IsPmtRate, "interest rate per period. For example, use 6%/4 for quarterly payments at 6% APR");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.IsPmtPer, "period for which you want to find the interest");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.IsPmtNper, "number of payment periods in an investment");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.IsPmtPv, "lump sum amount that a series of future payments is right now");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.RomanNumber, "is the Arabic numeral you want to convert");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.RomanForm, "is the number specifying the type of Roman numeral you want.");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.GetPivotDataDatafield, "is the name of the data field to extract data from");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.GetPivotDataPivottable, "is a reference to a cell or range of cells in the PivotTable that contains the data you want to retrieve");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.GetPivotDataField, "field to refer to");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.GetPivotDataItem, "field item to refer to");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.HyperlinkLinklocation, "is the text giving the path and file name to the document to be opened, a hard drive location, UNC address, or URL path");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.HyperlinkFriendlyname, "is text or a number that is displayed in the cell. If omitted, the cell displays the Link_location text");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PhoneticReference, "is a reference to a cell containing a phonetic string");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AverageAValue1, "are 1 to 255 arguments for which you want the average");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AverageAValue2, "are 1 to 255 arguments for which you want the average");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.MaxAValue1, "are 1 to 255 numbers, empty cells, logical values, or text numbers for which you want the maximum");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.MaxAValue2, "are 1 to 255 numbers, empty cells, logical values, or text numbers for which you want the maximum");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.MinAValue1, "are 1 to 255 numbers, empty cells, logical values, or text numbers for which you want the minimum");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.MinAValue2, "are 1 to 255 numbers, empty cells, logical values, or text numbers for which you want the minimum");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.StDevPAValue1, "are 1 to 255 values corresponding to a population and can be values, names, arrays, or references that contain values");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.StDevPAValue2, "are 1 to 255 values corresponding to a population and can be values, names, arrays, or references that contain values");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.VarPAValue1, "are 1 to 255 value arguments corresponding to a population");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.VarPAValue2, "are 1 to 255 value arguments corresponding to a population");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.StDevAValue1, "are 1 to 255 values corresponding to a sample of a population and can be values or names or references to values");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.StDevAValue2, "are 1 to 255 values corresponding to a sample of a population and can be values or names or references to values");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.VarAValue1, "are 1 to 255 value arguments corresponding to a sample of a population");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.VarAValue2, "are 1 to 255 value arguments corresponding to a sample of a population");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BahtTextNumber, "is a number that you want to convert");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.RtdProgID, "is the name of the ProgID of a registered COM automation add-in. Enclose the name in quotation marks");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.RtdServer, "is the name of the server where the add-in should be run. Enclose the name in quotation marks. If the add-in is run locally, use an empty string");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.RtdTopic1, "are 1 to 38 parameters that specify a piece of data");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.RtdTopic2, "are 1 to 38 parameters that specify a piece of data");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CubeValueConnection, "is the name of a connection to an OLAP cube");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CubeValueMemberexpression1, "is a slicer that determines the portion of the OLAP cube for which the aggregated value is to be retrieved");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CubeMemberConnection, "is the name of a connection to an OLAP cube");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CubeMemberMemberexpression, "is the expression representing the name of a member or tuple in the OLAP cube");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CubeMemberCaption, "is the caption to be displayed in the cell");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CubeMemberPropertyConnection, "is the name of a connection to an OLAP cube");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CubeMemberPropertyMemberexpression, "is the expression representing the name of a member in the OLAP cube");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CubeMemberPropertyProperty, "is the property name");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CubeRankedMemberConnection, "is the name of a connection to an OLAP cube");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CubeRankedMemberSetexpression, "is the set from which the element is to be retrieved");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CubeRankedMemberRank, "is the rank of the element to be retrieved");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CubeRankedMemberCaption, "is the caption to be displayed in the cell");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.Hex2BinNumber, "is the hexadecimal number you want to convert");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.Hex2BinPlaces, "is the number of characters to use");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.Hex2DecNumber, "is the hexadecimal number you want to convert");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.Hex2OctNumber, "is the hexadecimal number you want to convert");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.Hex2OctPlaces, "is the number of characters to use");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.Dec2BinNumber, "is the decimal integer you want to convert");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.Dec2BinPlaces, "is the number of characters to use");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.Dec2HexNumber, "is the decimal integer you want to convert");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.Dec2HexPlaces, "is the number of characters to use");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.Dec2OctNumber, "is the decimal integer you want to convert");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.Dec2OctPlaces, "is the number of characters to use");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.Oct2BinNumber, "is the octal number you want to convert");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.Oct2BinPlaces, "is the number of characters to use");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.Oct2HexNumber, "is the octal number you want to convert");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.Oct2HexPlaces, "is the number of characters to use");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.Oct2DecNumber, "is the octal number you want to convert");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.Bin2DecNumber, "is the binary number you want to convert");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.Bin2OctNumber, "is the binary number you want to convert");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.Bin2OctPlaces, "is the number of characters to use");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.Bin2HexNumber, "is the binary number you want to convert");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.Bin2HexPlaces, "is the number of characters to use");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ImSubInumber1, "is the complex number from which to subtract inumber2");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ImSubInumber2, "is the complex number to subtract from inumber1");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ImDivInumber1, "is the complex numerator or dividend");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ImDivInumber2, "is the complex denominator or divisor");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ImPowerInumber, "is a complex number you want to raise to a power");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ImPowerNumber, "is the power to which you want to raise the complex number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ImAbsInumber, "is a complex number for which you want the absolute value");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ImSqrtInumber, "is a complex number for which you want the square root");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ImLnInumber, "is a complex number for which you want the natural logarithm");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ImLog2Inumber, "is a complex number for which you want the base-2 logarithm");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ImLog10Inumber, "is a complex number for which you want the common logarithm");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ImSinInumber, "is a complex number for which you want the sine");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ImCosInumber, "is a complex number for which you want the cosine");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ImExpInumber, "is a complex number for which you want the exponential");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ImArgumentInumber, "is a complex number for which you want the argument");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ImConjugateInumber, "is a complex number for which you want the conjugate");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ImaginaryInumber, "is a complex number for which you want the imaginary coefficient");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ImRealInumber, "is a complex number for which you want the real coefficient");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ComplexRealnum, "is the real coefficient of the complex number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ComplexInum, "is the imaginary coefficient of the complex number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ComplexSuffix, "is the suffix for the imaginary component of the complex number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ImSumInumber1, "are from 1 to 255 complex numbers to add");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ImSumInumber2, "are from 1 to 255 complex numbers to add");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ImProductInumber1, "Inumber1, Inumber2,... are from 1 to 255 complex numbers to multiply.");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ImProductInumber2, "Inumber1, Inumber2,... are from 1 to 255 complex numbers to multiply.");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SeriessumX, "is the input value to the power series");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SeriessumN, "is the initial power to which you want to raise x");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SeriessumM, "is the step by which to increase n for each term in the series");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SeriessumCoefficients, "is a set of coefficients by which each successive power of x is multiplied");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FactDoubleNumber, "is the value for which to return the double factorial");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SqrtPiNumber, "is the number by which p is multiplied");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.QuotientNumerator, "is the dividend");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.QuotientDenominator, "is the divisor");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DeltaNumber1, "is the first number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DeltaNumber2, "is the second number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.GestepNumber, "is the value to test against step");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.GestepStep, "is the threshold value");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.IsEvenNumber, "is the value to test");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.IsOddNumber, "is the value to test");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.MRoundNumber, "is the value to round");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.MRoundMultiple, "is the multiple to which you want to round number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ErfLowerlimit, "is the lower bound for integrating ERF");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ErfUpperlimit, "is the upper bound for integrating ERF");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ErfcX, "is the lower bound for integrating ERF");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BesselJX, "is the value at which to evaluate the function");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BesselJN, "is the order of the Bessel function");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BesselKX, "is the value at which to evaluate the function");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BesselKN, "is the order of the function");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BesselYX, "is the value at which to evaluate the function");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BesselYN, "is the order of the function");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BesselIX, "is the value at which to evaluate the function");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BesselIN, "is the order of the Bessel function");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.XirrValues, "is a series of cash flows that correspond to a schedule of payments in dates");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.XirrDates, "is a schedule of payment dates that corresponds to the cash flow payments");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.XirrGuess, "is a number that you guess is close to the result of XIRR");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.XnpvRate, "is the discount rate to apply to the cash flows");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.XnpvValues, "is a series of cash flows that correspond to a schedule of payments in dates");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.XnpvDates, "is a schedule of payment dates that corresponds to the cash flow payments");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PriceMatSettlement, "is the security's settlement date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PriceMatMaturity, "is the security's maturity date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PriceMatIssue, "is the security's issue date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PriceMatRate, "is the security's interest rate at date of issue");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PriceMatYld, "is the security's annual yield");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PriceMatBasis, "is the type of day count basis to use");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.YieldMatSettlement, "is the security's settlement date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.YieldMatMaturity, "is the security's maturity date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.YieldMatIssue, "is the security's issue date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.YieldMatRate, "is the security's interest rate at date of issue");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.YieldMatPr, "is the security's price per $100 face value");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.YieldMatBasis, "is the type of day count basis to use");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.IntrateSettlement, "is the security's settlement date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.IntrateMaturity, "is the security's maturity date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.IntrateInvestment, "is the amount invested in the security");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.IntrateRedemption, "is the amount to be received at maturity");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.IntrateBasis, "is the type of day count basis to use");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ReceivedSettlement, "is the security's settlement date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ReceivedMaturity, "is the security's maturity date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ReceivedInvestment, "is the amount invested in the security");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ReceivedDiscount, "is the security's discount rate");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ReceivedBasis, "is the type of day count basis to use");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DiscSettlement, "is the security's settlement date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DiscMaturity, "is the security's maturity date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DiscPr, "is the security's price per $100 face value");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DiscRedemption, "is the security's redemption value per $100 face value");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DiscBasis, "is the type of day count basis to use");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PriceDiscSettlement, "is the security's settlement date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PriceDiscMaturity, "is the security's maturity date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PriceDiscDiscount, "is the security's discount rate");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PriceDiscRedemption, "is the security's redemption value per $100 face value");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PriceDiscBasis, "is the type of day count basis to use");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.YieldDiscSettlement, "is the security's settlement date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.YieldDiscMaturity, "is the security's maturity date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.YieldDiscPr, "is the security's price per $100 face value");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.YieldDiscRedemption, "is the security's redemption value per $100 face value");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.YieldDiscBasis, "is the type of day count basis to use");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TbillEqSettlement, "is the Treasury bill's settlement date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TbillEqMaturity, "is the Treasury bill's maturity date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TbillEqDiscount, "is the Treasury bill's discount rate");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TbillPriceSettlement, "is the Treasury bill's settlement date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TbillPriceMaturity, "is the Treasury bill's maturity date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TbillPriceDiscount, "is the Treasury bill's discount rate");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TbillYieldSettlement, "is the Treasury bill's settlement date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TbillYieldMaturity, "is the Treasury bill's maturity date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TbillYieldPr, "is the Treasury Bill's price per $100 face value");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PriceSettlement, "is the security's settlement date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PriceMaturity, "is the security's maturity date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PriceRate, "is the security's annual coupon rate");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PriceYld, "is the security's annual yield");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PriceRedemption, "is the security's redemption value per $100 face value");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PriceFrequency, "is the number of coupon payments per year");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PriceBasis, "is the type of day count basis to use");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.YieldSettlement, "is the security's settlement date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.YieldMaturity, "is the security's maturity date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.YieldRate, "is the security's annual coupon rate");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.YieldPr, "is the security's price per $100 face value");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.YieldRedemption, "is the security's redemption value per $100 face value");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.YieldFrequency, "is the number of coupon payments per year");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.YieldBasis, "is the type of day count basis to use");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DollarDeFractionaldollar, "is a number expressed as a fraction");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DollarDeFraction, "is the integer to use in the denominator of the fraction");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DollarFrDecimaldollar, "is a decimal number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DollarFrFraction, "is the integer to use in the denominator of a fraction");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.NominalEffectrate, "is the effective interest rate");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.NominalNpery, "is the number of compounding periods per year");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.EffectNominalrate, "is the nominal interest rate");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.EffectNpery, "is the number of compounding periods per year");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CumPrincRate, "is the interest rate");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CumPrincNper, "is the total number of payment periods");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CumPrincPv, "is the present value");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CumPrincStartperiod, "is the first period in the calculation");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CumPrincEndperiod, "is the last period in the calculation");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CumPrincType, "is the timing of the payment");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CumIpmtRate, "is the interest rate");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CumIpmtNper, "is the total number of payment periods");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CumIpmtPv, "is the present value");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CumIpmtStartperiod, "is the first period in the calculation");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CumIpmtEndperiod, "is the last period in the calculation");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CumIpmtType, "is the timing of the payment");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.EDateStartdate, "is a serial date number that represents the start date");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.EDateMonths, "is the number of months before or after start_date");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.EOMonthStartdate, "is a serial date number that represents the start date");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.EOMonthMonths, "is the number of months before or after the start_date");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.YearFracStartdate, "is a serial date number that represents the start date");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.YearFracEnddate, "is a serial date number that represents the end date");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.YearFracBasis, "is the type of day count basis to use");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CoupDaybsSettlement, "is the security's settlement date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CoupDaybsMaturity, "is the security's maturity date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CoupDaybsFrequency, "is the number of coupon payments per year");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CoupDaybsBasis, "is the type of day count basis to use");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CoupDaysSettlement, "is the security's settlement date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CoupDaysMaturity, "is the security's maturity date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CoupDaysFrequency, "is the number of coupon payments per year");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CoupDaysBasis, "is the type of day count basis to use");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CoupDaysncSettlement, "is the security's settlement date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CoupDaysncMaturity, "is the security's maturity date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CoupDaysncFrequency, "is the number of coupon payments per year");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CoupDaysncBasis, "is the type of day count basis to use");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CoupncdSettlement, "is the security's settlement date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CoupncdMaturity, "is the security's maturity date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CoupncdFrequency, "is the number of coupon payments per year");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CoupncdBasis, "is the type of day count basis to use");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CoupNumSettlement, "is the security's settlement date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CoupNumMaturity, "is the security's maturity date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CoupNumFrequency, "is the number of coupon payments per year");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CoupNumBasis, "is the type of day count basis to use");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CouppcdSettlement, "is the security's settlement date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CouppcdMaturity, "is the security's maturity date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CouppcdFrequency, "is the number of coupon payments per year");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CouppcdBasis, "is the type of day count basis to use");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DurationSettlement, "is the security's settlement date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DurationMaturity, "is the security's maturity date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DurationCoupon, "is the security's annual coupon rate");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DurationYld, "is the security's annual yield");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DurationFrequency, "is the number of coupon payments per year");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DurationBasis, "is the type of day count basis to use");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.MDurationSettlement, "is the security's settlement date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.MDurationMaturity, "is the security's maturity date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.MDurationCoupon, "is the security's annual coupon rate");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.MDurationYld, "is the security's annual yield");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.MDurationFrequency, "is the number of coupon payments per year");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.MDurationBasis, "is the type of day count basis to use");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.OddLPriceSettlement, "is the security's settlement date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.OddLPriceMaturity, "is the security's maturity date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.OddLPriceLastinterest, "is the security's last coupon date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.OddLPriceRate, "is the security's interest rate");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.OddLPriceYld, "is the security's annual yield");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.OddLPriceRedemption, "is the security's redemption value per $100 face value");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.OddLPriceFrequency, "is the number of coupon payments per year");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.OddLPriceBasis, "is the type of day count basis to use");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.OddLYieldSettlement, "is the security's settlement date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.OddLYieldMaturity, "is the security's maturity date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.OddLYieldLastinterest, "is the security's last coupon date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.OddLYieldRate, "is the security's interest rate");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.OddLYieldPr, "is the security's price");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.OddLYieldRedemption, "is the security's redemption value per $100 face value");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.OddLYieldFrequency, "is the number of coupon payments per year");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.OddLYieldBasis, "is the type of day count basis to use");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.OddFPriceSettlement, "is the security's settlement date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.OddFPriceMaturity, "is the security's maturity date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.OddFPriceIssue, "is the security's issue date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.OddFPriceFirstcoupon, "is the security's first coupon date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.OddFPriceRate, "is the security's interest rate");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.OddFPriceYld, "is the security's annual yield");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.OddFPriceRedemption, "is the security's redemption value per $100 face value");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.OddFPriceFrequency, "is the number of coupon payments per year");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.OddFPriceBasis, "is the type of day count basis to use");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.OddFYieldSettlement, "is the security's settlement date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.OddFYieldMaturity, "is the security's maturity date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.OddFYieldIssue, "is the security's issue date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.OddFYieldFirstcoupon, "is the security's first coupon date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.OddFYieldRate, "is the security's interest rate");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.OddFYieldPr, "is the security's price");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.OddFYieldRedemption, "is the security's redemption value per $100 face value");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.OddFYieldFrequency, "is the number of coupon payments per year");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.OddFYieldBasis, "is the type of day count basis to use");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.RandBetweenBottom, "is the smallest integer RANDBETWEEN will return");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.RandBetweenTop, "is the largest integer RANDBETWEEN will return");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.WeekNumSerialnumber, "is the date-time code used by Spreadsheet for date and time calculation");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.WeekNumReturntype, "is a number (1 or 2) that determines the type of the return value");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AmordegrcCost, "is the cost of the asset");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AmordegrcDatepurchased, "is the date the asset is purchased");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AmordegrcFirstperiod, "is the date of the end of the first period");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AmordegrcSalvage, "is the salvage value at the end of life of the asset.");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AmordegrcPeriod, "is the period");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AmordegrcRate, "is the rate of depreciation");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AmordegrcBasis, "year_basis : 0 for year of 360 days, 1 for actual, 3 for year of 365 days.");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AmorlincCost, "is the cost of the asset");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AmorlincDatepurchased, "is the date the asset is purchased");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AmorlincFirstperiod, "is the date of the end of the first period");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AmorlincSalvage, "is the salvage value at the end of life of the asset.");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AmorlincPeriod, "is the period");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AmorlincRate, "is the rate of depreciation");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AmorlincBasis, "year_basis : 0 for year of 360 days, 1 for actual, 3 for year of 365 days.");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ConvertNumber, "is the value in from_units to convert");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ConvertFromunit, "is the units for number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ConvertTounit, "is the units for the result");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AccrintIssue, "is the security's issue date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AccrintFirstinterest, "is the security's first interest date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AccrintSettlement, "is the security's settlement date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AccrintRate, "is the security's annual coupon rate");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AccrintPar, "is the security's par value");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AccrintFrequency, "is the number of coupon payments per year");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AccrintBasis, "is the type of day count basis to use");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AccrintCalcmethod, "is a logical value: to accrued interest from issue date = TRUE or omitted; to calculate from last coupon payment date = FALSE");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AccrintmIssue, "is the security's issue date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AccrintmSettlement, "is the security's maturity date, expressed as a serial date number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AccrintmRate, "is the security's annual coupon rate");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AccrintmPar, "is the security's par value");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AccrintmBasis, "is the type of day count basis to use");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.WorkDayStartdate, "is a serial date number that represents the start date");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.WorkDayDays, "is the number of nonweekend and non-holiday days before or after start_date");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.WorkDayHolidays, "is an optional array of one or more serial date numbers to exclude from the working calendar, such as state and federal holidays and floating holidays");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.NetworkDaysStartdate, "is a serial date number that represents the start date");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.NetworkDaysEnddate, "is a serial date number that represents the end date");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.NetworkDaysHolidays, "is an optional set of one or more serial date numbers to exclude from the working calendar, such as state and federal holidays and floating holidays");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.GcdNumber1, "are 1 to 255 values");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.GcdNumber2, "are 1 to 255 values");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.MultinomialNumber1, "are 1 to 255 values for which you want the multinomial");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.MultinomialNumber2, "are 1 to 255 values for which you want the multinomial");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.LcmNumber1, "are 1 to 255 values for which you want the least common multiple");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.LcmNumber2, "are 1 to 255 values for which you want the least common multiple");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FvSchedulePrincipal, "is the present value");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FvScheduleSchedule, "is an array of interest rates to apply");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CubeKpiMemberConnection, "is the name of a connection to an OLAP cube");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CubeKpiMemberKpiname, "is the KPI name");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CubeKpiMemberKpiproperty, "is the KPI property");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CubeKpiMemberCaption, "is the caption to be displayed in the cell");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CubeSetConnection, "is the name of a connection to an OLAP cube");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CubeSetSetexpression, "is the expression for the set");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CubeSetCaption, "is the caption to be displayed in the cell");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CubeSetSortorder, "is the sort order");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CubeSetSortby, "is the sort by");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CubeSetCountSet, "is the set whose elements are to be counted");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.IfErrorValue, "is any value or expression or reference");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.IfErrorValueiferror, "is any value or expression or reference");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CountIfsCriteriarange, "is the range of cells you want evaluated for the particular condition");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CountIfsCriteria, "is the condition in the form of a number, expression, or text that defines which cells will be counted");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SumIfsSumrange, "are the actual cells to sum.");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SumIfsCriteriarange, "is the range of cells you want evaluated for the particular condition");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SumIfsCriteria, "is the condition or criteria in the form of a number, expression, or text that defines which cells will be added");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AverageIfRange, "is the range of cells you want evaluated");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AverageIfCriteria, "is the condition or criteria in the form of a number, expression, or text that defines which cells will be used to find the average");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AverageIfAveragerange, "are the actual cells to be used to find the average. If omitted, the cells in range are used ");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AverageIfsAveragerange, "are the actual cells to be used to find the average.");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AverageIfsCriteriarange, "is the range of cells you want evaluated for the particular condition");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AverageIfsCriteria, "is the condition or criteria in the form of a number, expression, or text that defines which cells will be used to find the average");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AggregateFunctionnum, "is the number 1 to 19 that specifies the summary function for the aggregate.");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AggregateOptions, "is the number 0 to 7 that specifies the values to ignore for the aggregate");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AggregateArray, "is the array or range of numerical data on which to calculate the aggregate");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.AggregateK, "indicates the position in the array; it is k-th largest, k-th smallest, k-th percentile, or k-th quartile.");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BinomDotDistNumbers, "is the number of successes in trials");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BinomDotDistTrials, "is the number of independent trials");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BinomDotDistProbabilitys, "is the probability of success on each trial");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BinomDotDistCumulative, "is a logical value: for the cumulative distribution function, use TRUE; for the probability mass function, use FALSE");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BinomDotInvTrials, "is the number of Bernoulli trials");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BinomDotInvProbabilitys, "is the probability of success on each trial, a number between 0 and 1 inclusive");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BinomDotInvAlpha, "is the criterion value, a number between 0 and 1 inclusive");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ConfidenceNormAlpha, "is the significance level used to compute the confidence level, a number greater than 0 and less than 1");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ConfidenceNormStandarddev, "is the population standard deviation for the data range and is assumed to be known. Standard_dev must be greater than 0");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ConfidenceNormSize, "is the sample size");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ConfidenceDotTAlpha, "is the significance level used to compute the confidence level, a number greater than 0 and less than 1");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ConfidenceDotTStandarddev, "is the population standard deviation for the data range and is assumed to be known. Standard_dev must be greater than 0");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ConfidenceDotTSize, "is the sample size");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ChisqDotTestActualrange, "is the range of data that contains observations to test against expected values");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ChisqDotTestExpectedrange, "is the range of data that contains the ratio of the product of row totals and column totals to the grand total");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FDotTestArray1, "is the first array or range of data and can be numbers or names, arrays, or references that contain numbers (blanks are ignored)");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FDotTestArray2, "is the second array or range of data and can be numbers or names, arrays, or references that contain numbers (blanks are ignored)");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CovariancePArray1, "is the first cell range of integers and must be numbers, arrays, or references that contain numbers");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CovariancePArray2, "is the second cell range of integers and must be numbers, arrays, or references that contain numbers");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CovarianceSArray1, "is the first cell range of integers and must be numbers, arrays, or references that contain numbers");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CovarianceSArray2, "is the second cell range of integers and must be numbers, arrays, or references that contain numbers");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ExponDotDistX, "is the value of the function, a nonnegative number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ExponDotDistLambda, "is the parameter value, a positive number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ExponDotDistCumulative, "is a logical value for the function to return: the cumulative distribution function = TRUE; the probability density function = FALSE");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.GammaDotDistX, "is the value at which you want to evaluate the distribution, a nonnegative number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.GammaDotDistAlpha, "is a parameter to the distribution, a positive number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.GammaDotDistBeta, "is a parameter to the distribution, a positive number. If beta = 1, GAMMA.DIST returns the standard gamma distribution");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.GammaDotDistCumulative, "is a logical value: return the cumulative distribution function = TRUE; return the probability mass function = FALSE or omitted");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.GammaDotInvProbability, "is the probability associated with the gamma distribution, a number between 0 and 1, inclusive");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.GammaDotInvAlpha, "is a parameter to the distribution, a positive number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.GammaDotInvBeta, "is a parameter to the distribution, a positive number. If beta = 1, GAMMA.INV returns the inverse of the standard gamma distribution");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ModeMultNumber1, "are 1 to 255 numbers, or names, arrays, or references that contain numbers for which you want the mode");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ModeMultNumber2, "are 1 to 255 numbers, or names, arrays, or references that contain numbers for which you want the mode");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ModeSnglNumber1, "are 1 to 255 numbers, or names, arrays, or references that contain numbers for which you want the mode");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ModeSnglNumber2, "are 1 to 255 numbers, or names, arrays, or references that contain numbers for which you want the mode");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.NormDistX, "is the value for which you want the distribution");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.NormDistMean, "is the arithmetic mean of the distribution");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.NormDistStandarddev, "is the standard deviation of the distribution, a positive number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.NormDistCumulative, "is a logical value: for the cumulative distribution function, use TRUE; for the probability density function, use FALSE");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.NormInvProbability, "is a probability corresponding to the normal distribution, a number between 0 and 1 inclusive");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.NormInvMean, "is the arithmetic mean of the distribution");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.NormInvStandarddev, "is the standard deviation of the distribution, a positive number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PercentileExcArray, "is the array or range of data that defines relative standing");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PercentileExcK, "is the percentile value that is between 0 through 1, inclusive");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PercentileIncArray, "is the array or range of data that defines relative standing");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PercentileIncK, "is the percentile value that is between 0 through 1, inclusive");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PercentRankExcArray, "is the array or range of data with numeric values that defines relative standing");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PercentRankExcX, "is the value for which you want to know the rank");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PercentRankExcSignificance, "is an optional value that identifies the number of significant digits for the returned percentage, three digits if omitted (0.xxx%)");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PercentRankIncArray, "is the array or range of data with numeric values that defines relative standing");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PercentRankIncX, "is the value for which you want to know the rank");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PercentRankIncSignificance, "is an optional value that identifies the number of significant digits for the returned percentage, three digits if omitted (0.xxx%)");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PoissonDotDistX, "is the number of events");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PoissonDotDistMean, "is the expected numeric value, a positive number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PoissonDotDistCumulative, "is a logical value: for the cumulative Poisson probability, use TRUE; for the Poisson probability mass function, use FALSE");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.QuartileExcArray, "is the array or cell range of numeric values for which you want the quartile value");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.QuartileExcQuart, "is a number: minimum value = 0; 1st quartile = 1; median value = 2; 3rd quartile = 3; maximum value = 4");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.QuartileIncArray, "is the array or cell range of numeric values for which you want the quartile value");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.QuartileIncQuart, "is a number: minimum value = 0; 1st quartile = 1; median value = 2; 3rd quartile = 3; maximum value = 4");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.RankDotAvgNumber, "is the number for which you want to find the rank");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.RankDotAvgRef, "is an array of, or a reference to, a list of numbers. Nonnumeric values are ignored");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.RankDotAvgOrder, "is a number: rank in the list sorted descending = 0 or omitted; rank in the list sorted ascending = any nonzero value");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.RankDotEqNumber, "is the number for which you want to find the rank");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.RankDotEqRef, "is an array of, or a reference to, a list of numbers. Nonnumeric values are ignored");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.RankDotEqOrder, "is a number: rank in the list sorted descending = 0 or omitted; rank in the list sorted ascending = any nonzero value");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.StDevDotSNumber1, "are 1 to 255 numbers corresponding to a sample of a population and can be numbers or references that contain numbers");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.StDevDotSNumber2, "are 1 to 255 numbers corresponding to a sample of a population and can be numbers or references that contain numbers");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.StDevDotPNumber1, "are 1 to 255 numbers corresponding to a population and can be numbers or references that contain numbers");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.StDevDotPNumber2, "are 1 to 255 numbers corresponding to a population and can be numbers or references that contain numbers");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TDotDistX, "is the numeric value at which to evaluate the distribution");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TDotDistDegfreedom, "is an integer indicating the number of degrees of freedom that characterize the distribution");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TDotDistCumulative, "is a logical value: for the cumulative distribution function, use TRUE; for the probability density function, use FALSE");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TDotDistDot2tX, "is the numeric value at which to evaluate the distribution");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TDotDistDot2tDegfreedom, "is an integer indicating the number of degrees of freedom that characterize the distribution");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TDotDistDotTtX, "is the numeric value at which to evaluate the distribution");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TDotDistDotTtDegfreedom, "is an integer indicating the number of degrees of freedom that characterize the distribution");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TDotInvProbability, "is the probability associated with the two-tailed Student's t-distribution, a number between 0 and 1 inclusive");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TDotInvDegfreedom, "is a positive integer indicating the number of degrees of freedom to characterize the distribution");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TDotInvDot2tProbability, "is the probability associated with the two-tailed Student's t-distribution, a number between 0 and 1 inclusive");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TDotInvDot2tDegfreedom, "is a positive integer indicating the number of degrees of freedom to characterize the distribution");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.VarDotSNumber1, "are 1 to 255 numeric arguments corresponding to a sample of a population");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.VarDotSNumber2, "are 1 to 255 numeric arguments corresponding to a sample of a population");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.VarDotPNumber1, "are 1 to 255 numeric arguments corresponding to a population");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.VarDotPNumber2, "are 1 to 255 numeric arguments corresponding to a population");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.WeibullDotDistX, "is the value at which to evaluate the function, a nonnegative number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.WeibullDotDistAlpha, "is a parameter to the distribution, a positive number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.WeibullDotDistBeta, "is a parameter to the distribution, a positive number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.WeibullDotDistCumulative, "is a logical value: for the cumulative distribution function, use TRUE; for the probability mass function, use FALSE");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.NetworkDaysIntlStartdate, "is a serial date number that represents the start date");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.NetworkDaysIntlEnddate, "is a serial date number that represents the end date");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.NetworkDaysIntlWeekend, "is a number or string specifying when weekends occur");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.NetworkDaysIntlHolidays, "is an optional set of one or more serial date numbers to exclude from the working calendar, such as state and federal holidays and floating holidays");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.WorkDayIntlStartdate, "is a serial date number that represents the start date");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.WorkDayIntlDays, "is the number of nonweekend and non-holiday days before or after start_date");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.WorkDayIntlWeekend, "is a number or string specifying when weekends occur");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.WorkDayIntlHolidays, "is an optional array of one or more serial date numbers to exclude from the working calendar, such as state and federal holidays and floating holidays");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.IsoCeilingNumber, "is the value you want to round");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.IsoCeilingSignificance, "is the optional multiple to which you want to round");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BetaDistX, "is the value between A and B at which to evaluate the function");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BetaDistAlpha, "is a parameter to the distribution and must be greater than 0");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BetaDistBeta, "is a parameter to the distribution and must be greater than 0");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BetaDistCumulative, "is a logical value: for the cumulative distribution function, use TRUE; for the probability density function, use FALSE");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BetaDistA, "is an optional lower bound to the interval of x. If omitted, A = 0");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BetaDistB, "is an optional upper bound to the interval of x. If omitted, B = 1");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BetaDotInvProbability, "is a probability associated with the beta distribution");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BetaDotInvAlpha, "is a parameter to the distribution and must be greater than 0");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BetaDotInvBeta, "is a parameter to the distribution and must be greater than 0");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BetaDotInvA, "is an optional lower bound to the interval of x. If omitted, A = 0");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BetaDotInvB, "is an optional upper bound to the interval of x. If omitted, B = 1");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ChisqDotDistX, "is the value at which you want to evaluate the distribution, a nonnegative number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ChisqDotDistDegfreedom, "is the number of degrees of freedom, a number between 1 and 10^10, excluding 10^10");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ChisqDotDistCumulative, "is a logical value for the function to return: the cumulative distribution function = TRUE; the probability density function = FALSE");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ChisqDotDistDotRtX, "is the value at which you want to evaluate the distribution, a nonnegative number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ChisqDotDistDotRtDegfreedom, "is the number of degrees of freedom, a number between 1 and 10^10, excluding 10^10");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ChisqDotInvProbability, "is a probability associated with the chi-squared distribution, a value between 0 and 1 inclusive");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ChisqDotInvDegfreedom, "is the number of degrees of freedom, a number between 1 and 10^10, excluding 10^10");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ChisqDotInvDotRtProbability, "is a probability associated with the chi-squared distribution, a value between 0 and 1 inclusive");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ChisqDotInvDotRtDegfreedom, "is the number of degrees of freedom, a number between 1 and 10^10, excluding 10^10");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FDotDistX, "is the value at which to evaluate the function, a nonnegative number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FDotDistDegfreedom1, "is the numerator degrees of freedom, a number between 1 and 10^10, excluding 10^10");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FDotDistDegfreedom2, "is the denominator degrees of freedom, a number between 1 and 10^10, excluding 10^10");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FDotDistCumulative, "is a logical value for the function to return: the cumulative distribution function = TRUE; the probability density function = FALSE");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FDotDistDotRtX, "is the value at which to evaluate the function, a nonnegative number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FDotDistDotRtDegfreedom1, "is the numerator degrees of freedom, a number between 1 and 10^10, excluding 10^10");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FDotDistDotRtDegfreedom2, "is the denominator degrees of freedom, a number between 1 and 10^10, excluding 10^10");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FDotInvProbability, "is a probability associated with the F cumulative distribution, a number between 0 and 1 inclusive");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FDotInvDegfreedom1, "is the numerator degrees of freedom, a number between 1 and 10^10, excluding 10^10");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FDotInvDegfreedom2, "is the denominator degrees of freedom, a number between 1 and 10^10, excluding 10^10");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FDotinvDotRtProbability, "is a probability associated with the F cumulative distribution, a number between 0 and 1 inclusive");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FDotinvDotRtDegfreedom1, "is the numerator degrees of freedom, a number between 1 and 10^10, excluding 10^10");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FDotinvDotRtDegfreedom2, "is the denominator degrees of freedom, a number between 1 and 10^10, excluding 10^10");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.HypgeomDotDistSamples, "is the number of successes in the sample");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.HypgeomDotDistNumbersample, "is the size of the sample");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.HypgeomDotDistPopulations, "is the number of successes in the population");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.HypgeomDotDistNumberpop, "is the population size");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.HypgeomDotDistCumulative, "is a logical value: for the cumulative distribution function, use TRUE; for the probability density function, use FALSE");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.LogNormDistX, "is the value at which to evaluate the function, a positive number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.LogNormDistMean, "is the mean of ln(x)");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.LogNormDistStandarddev, "is the standard deviation of ln(x), a positive number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.LogNormDistCumulative, "is a logical value: for the cumulative distribution function, use TRUE; for the probability density function, use FALSE");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.LogNormInvProbability, "is a probability associated with the lognormal distribution, a number between 0 and 1, inclusive");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.LogNormInvMean, "is the mean of ln(x)");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.LogNormInvStandarddev, "is the standard deviation of ln(x), a positive number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.NegbinomDotDistNumberf, "is the number of failures");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.NegbinomDotDistNumbers, "is the threshold number of successes");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.NegbinomDotDistProbabilitys, "is the probability of a success; a number between 0 and 1");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.NegbinomDotDistCumulative, "is a logical value: for the cumulative distribution function, use TRUE; for the probability mass function, use FALSE");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.NormSDistZ, "is the value for which you want the distribution");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.NormSDistCumulative, "is a logical value for the function to return: the cumulative distribution function = TRUE; the probability density function = FALSE");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.NormSInvProbability, "is a probability corresponding to the normal distribution, a number between 0 and 1 inclusive");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TDotTestArray1, "is the first data set");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TDotTestArray2, "is the second data set");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TDotTestTails, "specifies the number of distribution tails to return: one-tailed distribution = 1; two-tailed distribution = 2");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.TDotTestType, "is the kind of t-test: paired = 1, two-sample equal variance (homoscedastic) = 2, two-sample unequal variance = 3");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ZDotTestArray, "is the array or range of data against which to test X");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ZDotTestX, "is the value to test");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ZDotTestSigma, "is the population (known) standard deviation. If omitted, the sample standard deviation is used");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ErfPreciseX, "is the lower bound for integrating ERF.PRECISE");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ErfcPreciseX, "is the lower bound for integrating ERFC.PRECISE");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.GammaLnPreciseX, "is the value for which you want to calculate GAMMALN.PRECISE, a positive number");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CeilingDotPreciseNumber, "is the value you want to round");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CeilingDotPreciseSignificance, "is the multiple to which you want to round");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FloorDotPreciseNumber, "is the numeric value you want to round");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FloorDotPreciseSignificance, "is the multiple to which you want to round. ");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ACotNumber, "is the cotangent of the angle you want");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ACotHNumber, "is the hyperbolic cotangent of the angle that you want");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CotNumber, "is the angle in radians for which you want the cotangent");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CotHNumber, "is the angle in radians for which you want the hyperbolic cotangent");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CscNumber, "is the angle in radians for which you want the cosecant");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CscHNumber, "is the angle in radians for which you want the hyperbolic cosecant");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SecNumber, "is the angle in radians for which you want the secant");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SecHNumber, "is the angle in radians for which you want the hyperbolic secant");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ImTanInumber, "is a complex number for which you want the tangent");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ImCotInumber, "is a complex number for which you want the cotangent");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ImCscInumber, "is a complex number for which you want the cosecant");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ImCscHInumber, "is a complex number for which you want the hyperbolic cosecant");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ImSecInumber, "is a complex number for which you want the secant");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ImSecHInumber, "is a complex number for which you want the hyperbolic secant");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BitAndNumber1, "is the decimal representation of the binary number you want to evaluate");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BitAndNumber2, "is the decimal representation of the binary number you want to evaluate");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BitOrNumber1, "is the decimal representation of the binary number you want to evaluate");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BitOrNumber2, "is the decimal representation of the binary number you want to evaluate");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BitXorNumber1, "is the decimal representation of the binary number you want to evaluate");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BitXorNumber2, "is the decimal representation of the binary number you want to evaluate");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BitLShiftNumber, "is the decimal representation of the binary number you want to evaluate");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BitLShiftShiftamount, "is the number of bits that you want to shift Number left by");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BitRShiftNumber, "is the decimal representation of the binary number you want to evaluate");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BitRShiftShiftamount, "is the number of bits that you want to shift Number right by");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PermutationaNumber, "is the total number of objects");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PermutationaNumberchosen, "is the number of objects in each permutation");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CombinANumber, "is the total number of items");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CombinANumberchosen, "is the number of items in each combination");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.XOrLogical1, "are 1 to 254 conditions you want to test that can be either TRUE or FALSE and can be logical values, arrays, or references");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.XOrLogical2, "are 1 to 254 conditions you want to test that can be either TRUE or FALSE and can be logical values, arrays, or references");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PDurationRate, "is the interest rate per period.");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PDurationPv, "is the present value of the investment");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PDurationFv, "is the desired future value of the investment");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BaseNumber, "is the number that you want to convert");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BaseRadix, "is the base Radix that you want to convert the number into");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BaseMinlength, "is the minimum length of the returned string.  If omitted leading zeros are not added");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DecimalNumber, "is the number that you want to convert");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DecimalRadix, "is the base Radix of the number you are converting");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DaysEnddate, "start_date and end_date are the two dates between which you want to know the number of days");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.DaysStartdate, "start_date and end_date are the two dates between which you want to know the number of days");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BinomDistRangeTrials, "is the number of independent trials");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BinomDistRangeProbabilitys, "is the probability of success on each trial");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BinomDistRangeNumbers, "is the number of successes in trials");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.BinomDistRangeNumbers2, "if provided this function returns the probability that the number of successful trials shall lie between number_s and number_s2");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.GammaX, "is the value for which you want to calculate Gamma");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SkewPNumber1, "are 1 to 254 numbers or names, arrays, or references that contain numbers for which you want the population skewness");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SkewPNumber2, "are 1 to 254 numbers or names, arrays, or references that contain numbers for which you want the population skewness");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.PhiX, "is the number for which you want the density of the standard normal distribution");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.RRINper, "is the number of periods for the investment");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.RRIPv, "is the present value of the investment");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.RRIFv, "is the future value of the investment");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.UnicodeText, "is the character that you want the Unicode value of");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.MUnitDimension, "is an integer specifying the dimension of the unit matrix that you want to return");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ArabicText, "is the Roman numeral you want to convert");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ISOWeekNumDate, "is the date-time code used by Spreadsheet for date and time calculation");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.NumberValueText, "is the string representing the number you want to convert");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.NumberValueDecimalseparator, "is the character used as the decimal separator in the string");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.NumberValueGroupseparator, "is the character used as the group separator in the string");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SheetValue, "is the name of a sheet or a reference that you want the sheet number of.  If omitted the number of the sheet containing the function is returned");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.SheetsReference, "is a reference for which you want to know the number of sheets it contains.  If omitted the number of sheets in the workbook containing the function is returned");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FormulaTextReference, "is a reference to a formula");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.IsFormulaReference, "is a reference to the cell you want to test.  Reference can be a cell reference, a formula, or name that refers to a cell");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.IfNAValue, "is any value or expression or reference");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.IfNAValueifna, "is any value or expression or reference");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CeilingMathNumber, "is the value you want to round");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CeilingMathSignificance, "is the multiple to which you want to round");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.CeilingMathMode, "when given and nonzero this function will round away from zero");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FloorMathNumber, "is the value you want to round");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FloorMathSignificance, "is the multiple to which you want to round");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FloorMathMode, "when given and nonzero this function will round towards zero");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ImSinHInumber, "is a complex number for which you want the hyperbolic sine");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ImCosHInumber, "is a complex number for which you want the hyperbolic cosine");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FunctionFilterXMLXml, "is a string in valid XML format");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FunctionFilterXMLXpath, "is a string in standard XPath format");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FunctionWebServiceUrl, "is the URL of the web service");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.EncodeURLText, "is a string to be URL encoded");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FieldDataFieldName, "is the name of the data source field from which the value should be retrieved");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.RangeCellRefenrece, "is an absolute reference to a cell in a template that will be copied for each data source record on the resulting worksheet");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FieldPictureDataFieldName, "is the name of the data source field from which the picture should be retrieved");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FieldPicturePicturePlacement, "is a string that specifies where a picture should be inserted in the resulting document.");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FieldPictureTargetRange, "is a reference to the cell range into which a picture should be inserted");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FieldPictureLockAspectRatio, "is a parameter that indicates whether the picture's original aspect ratio should be preserved");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FieldPictureOffsetX, "is the distance in pixels from the left edge of the target range to the top-left corner of the picture");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FieldPictureOffsetY, "is the distance in pixels from the top edge of the target range to the top-left corner of the picture");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.FieldPictureWidth, "is the value that specifies the desired width of the picture in pixels");
			AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId.ParameterParameterName, "is the value that specifies the desired height of the picture in pixels");
		}
		#endregion
		public static XtraLocalizer<XtraSpreadsheetFunctionArgumentDescriptionStringId> CreateDefaultLocalizer() {
			return new XtraSpreadsheetFunctionArgumentsDescriptionsResLocalizer();
		}
		public static string GetString(XtraSpreadsheetFunctionArgumentDescriptionStringId id) {
			return Active.GetLocalizedString(id);
		}
		public override XtraLocalizer<XtraSpreadsheetFunctionArgumentDescriptionStringId> CreateResXLocalizer() {
			return new XtraSpreadsheetFunctionArgumentsDescriptionsResLocalizer();
		}
		protected override void AddString(XtraSpreadsheetFunctionArgumentDescriptionStringId id, string str) {
			Dictionary<XtraSpreadsheetFunctionArgumentDescriptionStringId, string> table = XtraLocalizierHelper<XtraSpreadsheetFunctionArgumentDescriptionStringId>.GetStringTable(this);
			table[id] = str;
		}
	}
	#endregion
	#region XtraSpreadsheetFunctionArgumentsDescriptionsResLocalizer
	public class XtraSpreadsheetFunctionArgumentsDescriptionsResLocalizer : XtraResXLocalizer<XtraSpreadsheetFunctionArgumentDescriptionStringId> {
		static XtraSpreadsheetFunctionArgumentsDescriptionsResLocalizer() {
			SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<XtraSpreadsheetFunctionArgumentDescriptionStringId>(CreateDefaultLocalizer()));
		}
		public XtraSpreadsheetFunctionArgumentsDescriptionsResLocalizer()
			: base(new XtraSpreadsheetFunctionArgumentsDescriptionsLocalizer()) { }
		public static XtraLocalizer<XtraSpreadsheetFunctionArgumentDescriptionStringId> CreateDefaultLocalizer() {
			return new XtraSpreadsheetFunctionArgumentsDescriptionsResLocalizer();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new ResourceManager Manager {
			get { return base.Manager; }
		}
		protected override ResourceManager CreateResourceManagerCore() {
#if DXPORTABLE
			return new ResourceManager("DevExpress.Spreadsheet.Core.LocalizationFunctionArgumentsDescriptionsRes", typeof(XtraSpreadsheetFunctionArgumentsDescriptionsResLocalizer).GetAssembly());
#else
			return new ResourceManager("DevExpress.XtraSpreadsheet.LocalizationFunctionArgumentsDescriptionsRes", typeof(XtraSpreadsheetFunctionArgumentsDescriptionsResLocalizer).GetAssembly());
#endif
		}
#if DXPORTABLE
		public static string GetString(XtraSpreadsheetFunctionArgumentDescriptionStringId id, CultureInfo culture) {
			return XtraResXLocalizer<XtraSpreadsheetFunctionArgumentDescriptionStringId>.GetLocalizedStringFromResources<XtraSpreadsheetFunctionArgumentDescriptionStringId, XtraSpreadsheetFunctionArgumentsDescriptionsResLocalizer>(
				id,
				culture,
				() => Active as XtraSpreadsheetFunctionArgumentsDescriptionsResLocalizer,
				(stringId) => XtraSpreadsheetFunctionArgumentsDescriptionsLocalizer.GetString(stringId)
			);
		}
#else
		[ThreadStatic]
		static CultureInfo lastCulture;
		[ThreadStatic]
		static ResourceSet lastSet;
		public static string GetString(XtraSpreadsheetFunctionArgumentDescriptionStringId id, CultureInfo culture) {
			if (culture == lastCulture)
				return GetString(id, lastSet);
			XtraSpreadsheetFunctionArgumentsDescriptionsLocalizer.GetString(id); 
			lastCulture = culture;
			lastSet = null;
			XtraSpreadsheetFunctionArgumentsDescriptionsResLocalizer localizer = Active as XtraSpreadsheetFunctionArgumentsDescriptionsResLocalizer;
			if (localizer == null)
				return XtraSpreadsheetFunctionArgumentsDescriptionsLocalizer.GetString(id);
			lastSet = localizer.Manager.GetResourceSet(culture, true, true);
			return GetString(id, lastSet);
		}
		static string GetString(XtraSpreadsheetFunctionArgumentDescriptionStringId id, ResourceSet set) {
			if (set == null)
				return XtraSpreadsheetFunctionArgumentsDescriptionsLocalizer.GetString(id);
			string resStr = String.Format("{0}.{1}", typeof(XtraSpreadsheetFunctionArgumentDescriptionStringId).Name, id);
			string result = set.GetString(resStr);
			if (!String.IsNullOrEmpty(result))
				return result;
			return XtraSpreadsheetFunctionArgumentsDescriptionsLocalizer.GetString(id);
		}
#endif
	}
	#endregion
}
