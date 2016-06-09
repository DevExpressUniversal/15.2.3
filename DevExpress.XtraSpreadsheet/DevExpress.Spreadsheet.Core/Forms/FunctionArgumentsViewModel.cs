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
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Localization;
using System.Collections.ObjectModel;
using System.Diagnostics;
namespace DevExpress.XtraSpreadsheet.Forms {
	#region XtraSpreadsheetFunctionArgumentId
	enum XtraSpreadsheetFunctionArgumentId {
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
	#region FormulaParserHelper
	internal class FunctionPresentation {
		#region Fields
		readonly string formula;
		readonly string[] args;
		readonly string functionName;
		#endregion
		#region Properties
		public string Formula { get { return formula; } }
		public string[] Args { get { return args; } }
		public string FunctionName { get { return functionName; } }
		public FunctionCallItemParsedData CallInfo { get; private set; }
		public int ArgumentsLeft { get; private set; }
		public int ArgumentsRight { get; private set; }
		public int ArgumentsLength { get; private set; }
		public int CurrentArgument { get; private set; }
		#endregion
		public FunctionCallItemParsedData GetTopCallInfo(List<FunctionCallItemParsedData> callInfos) {
			int index = callInfos.Count - 1;
			for(int i = 0; i < callInfos.Count; i++) {
				if(callInfos[i].Position < callInfos[index].Position)
					index = i;
			}
			return callInfos[index];
		}
		static string Substring(int left, int right, string formulaBody) {
			int length = Math.Min(formulaBody.Length - left, right - left + 1);
			return formulaBody.Substring(left, length);
		}
		public FunctionPresentation(Function function, DocumentModel documentModel, string formulaBody) {
			Debug.Assert(function != null, "function != null");
			this.functionName = Substring(function.Name.Left, function.Name.Right, formulaBody);
			this.args = new string[function.Arguments.Count];
			for(int i = 0; i < function.Arguments.Count; i++) {
				Argument argument = function.Arguments[i];
				args[i] = Substring(function.ArgumentsLeft + argument.Left, function.ArgumentsLeft + argument.Right, formulaBody);
			}
			char separator = documentModel.DataContext.GetListSeparator();
			StringBuilder sb = new StringBuilder(functionName);
			sb.Append('(');
			string arguments = GenerateArguments(args, separator, args.Length);
			sb.Append(arguments);
			sb.Append(')');
			formula = sb.ToString();
			ArgumentsLeft = function.ArgumentsLeft;
			ArgumentsLength = arguments.Length;
			ArgumentsRight = ArgumentsLeft + ArgumentsLength - 1;
			WorkbookDataContext context = documentModel.DataContext;
			context.PushCurrentCell(documentModel.ActiveSheet.Selection.ActiveCell);
			try {
				IncompleteExpressionParserContext parserContext = context.ExpressionParser.ParseIncomplete("=" + this.formula, OperandDataType.Default);
				CallInfo = parserContext == null || parserContext.FunctionCalls.Count == 0 ? null : GetTopCallInfo(parserContext.FunctionCalls);
			}
			finally {
				context.PopCurrentCell();
			}
			CurrentArgument = function.CurrentArgumentIndex;
		}
		static List<string> GetRealArguments(string argumentText, char separator) {
			List<string> result = new List<string>();
			int pos = 0;
			while(pos < argumentText.Length) {
				Argument argument = new Argument();
				int length = argument.Parse(argumentText.Substring(pos), separator);
				if(length == -1)
					length = argumentText.Length - pos;
				result.Add(argumentText.Substring(pos, length));
				pos += length + 1;
			}
			if(result.Count == 0)
				result.Add(String.Empty);
			return result;
		}
		public static string GenerateArguments(IEnumerable<string> arguments, char separator, int count) {
			StringBuilder sb = new StringBuilder();
			int i = 0;
			int j = 0;
			foreach(string argument in arguments) {
				if(j == count)
					break;
				List<string> realArguments = GetRealArguments(argument, separator);
				foreach(string realArgument in realArguments) {
					if(i != 0)
						sb.Append(separator);
					sb.Append(realArgument);
					i++;
				}
				j++;
			}
			return sb.ToString();
		}
		public static string GenerateArguments(IEnumerable<FunctionArgumentViewModel> arguments, char separator, int count) {
			List<string> argumentsAsStrings = new List<string>();
			foreach(FunctionArgumentViewModel argument in arguments) {
				argumentsAsStrings.Add(argument.Value);
			}
			string result = GenerateArguments(argumentsAsStrings, separator, count);
			return result;
		}
	}
	internal class Param {
		#region Properties
		public int Left { get; set; }
		public int Right { get; set; }
		public int Length { get { return Right - Left + 1; } }
		#endregion
		public static bool MayBeFunctionChar(char c) {
			return char.IsLetterOrDigit(c) || c == '.' || c == '_';
		}
		public bool ContainsSegment(int left, int right) {
			return right >= left && left >= Left && right <= Right;
		}
		#region Equality members
		public override bool Equals(object obj) {
			if(ReferenceEquals(null, obj))
				return false;
			if(ReferenceEquals(this, obj))
				return true;
			if(obj.GetType() != this.GetType())
				return false;
			return Equals((Param)obj);
		}
		protected bool Equals(Param other) {
			return Left == other.Left && Right == other.Right;
		}
		public override int GetHashCode() {
			return (Left * 397) + Right;
		}
		#endregion
	}
	internal class Argument : Param {
		#region Properties
		public List<Function> Functions { get; private set; }
		#endregion
		public Argument() {
			Functions = new List<Function>();
		}
		public int Parse(string text, char separator) {
			if(String.IsNullOrEmpty(text))
				return 0;
			int round = 0;
			int square = 0;
			int curved = 0;
			int pos = 0;
			int length = text.Length;
			Param function = new Param {Left = -1};
			bool quotes = false;
			while(pos < length) {
				char c = text[pos];
				if(c == '(') {
					if(function.Left != -1) {
						Function newFunction = new Function {
							Left = function.Left,
							Name = {Left = function.Left, Right = function.Right}
						};
						int result = newFunction.Parse(text.Substring(pos + 1), separator);
						if(result == -1)
							return -1;
						pos += result;
						newFunction.Right = pos + 1;
						Functions.Add(newFunction);
						function.Left = -1;
						if(pos >= length)
							break;
					}
					else
						round++;
				}
				else if(c == '"') {
					quotes = !quotes;
				}
				else if(c == ')') {
					round--;
					function.Left = -1;
					if(round == -1) {
						break;
					}
				}
				else if(c == '}') {
					curved--;
					function.Left = -1;
				}
				else if(c == '{') {
					curved++;
					function.Left = -1;
				}
				else if(c == '[') {
					square++;
					function.Left = -1;
				}
				else if(c == ']') {
					square--;
					function.Left = -1;
				}
				else if(MayBeFunctionChar(c)) {
					if(function.Left == -1)
						function.Left = pos;
					function.Right = pos;
				}
				else if(c == separator && !quotes) {
					if(round == 0)
						break;
				}
				else {
					function.Left = -1;
				}
				pos++;
			}
			if(curved != 0)
				return -1;
			if(square > 0)
				return -1;
			return pos;
		}
		public Function Get(int left, int right) {
			int l = left - this.Left;
			int r = right - this.Left;
			foreach(Function function in Functions) {
				if(function.ContainsSegment(l, r)) {
					Function getResult = function.Get(l, r);
					if(getResult != null) {
						Function result = new Function {
							Left = this.Left + getResult.Left,
							Right = this.Left + getResult.Right,
							Name = {Left = this.Left + getResult.Name.Left, Right = this.Left + getResult.Name.Right}
						};
						result.Arguments.AddRange(getResult.Arguments);
						return result;
					}
					return null;
				}
			}
			return null;
		}
	}
	internal class Function : Param {
		#region Properties
		public List<Argument> Arguments { get; private set; }
		public Param Name { get; private set; }
		public int ArgumentsLeft { get { return Name.Right + 2; } }
		public int CurrentArgumentIndex { get; set; }
		#endregion
		public Function() {
			Name = new Param();
			Arguments = new List<Argument>();
		}
		public int Parse(string text, char separator) {
			int pos = -1;
			int length = text.Length;
			while(pos < length) {
				pos++;
				Argument argument = new Argument {Left = pos};
				int result = argument.Parse(text.Substring(pos), separator);
				if(result == -1)
					return -1;
				pos += result;
				argument.Right = pos >= length ? pos : pos - 1;
				Arguments.Add(argument);
				if(pos < length && text[pos] == ')') {
					pos++;
					break;
				}
			}
			return pos;
		}
		public Function Get(int left, int right) {
			int l = left - this.Name.Right - 2;
			int r = right - this.Name.Right - 2;
			Function result = null;
			foreach(Argument argument in Arguments) {
				if(argument.ContainsSegment(l, r)) {
					Function getResult = argument.Get(l, r);
					if(getResult != null) {
						result = new Function {
							Left = ArgumentsLeft + getResult.Left,
							Right = ArgumentsLeft + getResult.Right,
							Name = {
								Left = ArgumentsLeft + getResult.Name.Left,
								Right = ArgumentsLeft + getResult.Name.Right
							}
						};
						result.Arguments.AddRange(getResult.Arguments);
					}
					break;
				}
			}
			return result ?? this;
		}
	}
	internal class FormulaParser {
		#region Field
		readonly DocumentModel documentModel;
		int parseResult;
		#endregion
		#region Properties
		public List<Argument> Body { get; set; }
		public string FormulaBody { get; private set; }
		#endregion
		public FormulaParser(DocumentModel documentModel) {
			this.documentModel = documentModel;
		}
		public Function Get(int selectionStart, int selectionLength) {
			if(parseResult == -1)
				return null;
			int left = selectionStart;
			int right = selectionLength == 0 ? left : left + selectionLength - 1;
			foreach(Argument argument in Body) {
				if(argument.Left == -1)
					continue;
				if(argument.ContainsSegment(left, right)) {
					Function function = argument.Get(left, right);
					if(function == null)
						return null;
					int argumentPos = selectionStart - function.ArgumentsLeft;
					function.CurrentArgumentIndex = 0;
					for(int i = 0; i < function.Arguments.Count; i++) {
						if(argumentPos < function.Arguments[i].Left)
							break;
						function.CurrentArgumentIndex = i;
					}
					return function;
				}
			}
			return null;
		}
		public void Update(string formulaBody) {
			FormulaBody = formulaBody;
			if(String.IsNullOrEmpty(formulaBody)) {
				parseResult = -1;
				return;
			}
			parseResult = 0;
			Body = new List<Argument>();
			char listSeparator = documentModel.DataContext.GetListSeparator();
			Body.Add(new Argument {Left = FormulaBody[0] == '=' ? 1 : 0});
			for(int i = 0; i < Body.Count; i++) {
				Argument argument = Body[i];
				int length = argument.Parse(FormulaBody.Substring(argument.Left), listSeparator);
				if(length == -1) {
					parseResult = -1;
					break;
				}
				length += argument.Left;
				argument.Right = length >= formulaBody.Length ? length : length - 1;
				length++;
				if(length < formulaBody.Length) {
					Body.Add(new Argument {Left = length});
				}
			}
		}
	}
	#endregion
	public class FunctionArgumentsViewModel : ViewModelBase, IReferenceEditViewModel {
		#region Fields
		int lockUpdate;
		readonly ObservableCollection<FunctionArgumentViewModel> arguments;
		int currentArgumentIndex = int.MinValue;
		string formulaBody;
		readonly string originalFormulaBody;
		int selectionStart;
		int selectionLength;
		readonly ISpreadsheetControl control;
		readonly FormulaParser formulaParser;
		FunctionPresentation currentFunctionPresentation;
		Function currentFunction;
		#endregion
		public FunctionArgumentsViewModel(ISpreadsheetControl control, string formulaBody, int selectionStart, int selectionLength) {
			this.control = control;
			this.originalFormulaBody = formulaBody;
			this.selectionStart = selectionStart;
			this.selectionLength = selectionLength;
			this.formulaBody = formulaBody;
			this.arguments = new ObservableCollection<FunctionArgumentViewModel>();
			this.formulaParser = new FormulaParser(control.InnerControl.DocumentModel);
			this.KeepChanges = false;
			ResultFormula = originalFormulaBody;
			formulaParser.Update(formulaBody);
			UpdateViewModel();
		}
		#region Properties
		DocumentModel DocumentModel { get { return SpreadsheetControl.InnerControl.DocumentModel; } }
		public string FunctionName { get { return currentFunctionPresentation != null ? currentFunctionPresentation.FunctionName : String.Empty; } }
		public string FunctionDescription {
			get {
				if (currentFunctionPresentation==null || currentFunctionPresentation.CallInfo == null)
					return "";
				int functionCode = currentFunctionPresentation.CallInfo.FuncThing.FuncCode;
				if(functionCode == 255) {
					CustomFunctionDescription customFunctionDescription;
					if(!DocumentModel.CustomFunctionsDescriptions.TryGetValue(currentFunctionPresentation.CallInfo.FuncThing.Function.Name, out customFunctionDescription)) {
						return "Undefined";						
					}
					return customFunctionDescription.Description;
				}
				string description = XtraSpreadsheetFunctionDescriptionResLocalizer.GetString((XtraSpreadsheetFunctionDescriptionStringId) functionCode, this.DocumentModel.Culture);
				return description;
			}
		}
		public ObservableCollection<FunctionArgumentViewModel> Arguments { get { return arguments; } }
		public ISpreadsheetControl SpreadsheetControl { get { return control; } }
		ISpreadsheetControl IReferenceEditViewModel.Control { get { return SpreadsheetControl; } }
		public string ResultFormula { get; private set; }
		public bool KeepChanges { get; private set; }
		#region CurrentArgumentIndex
		public int CurrentArgumentIndex {
			get { return currentArgumentIndex; }
			set {
				if(CurrentArgumentIndex == value)
					return;
				currentArgumentIndex = value;
				OnPropertyChanged("CurrentArgumentIndex");
				OnPropertyChanged("CurrentArgumentName");
				OnPropertyChanged("CurrentArgumentDescription");
				if(currentArgumentIndex + 1 < Arguments.Count && !Arguments[currentArgumentIndex + 1].IsVisible) {
					Arguments[currentArgumentIndex + 1].IsVisible = true;
				}
			}
		}
		#endregion
		public FunctionArgumentViewModel CurrentArgument {
			get {
				if(CurrentArgumentIndex < 0 || CurrentArgumentIndex >= Arguments.Count)
					return null;
				return Arguments[CurrentArgumentIndex];
			}
		}
		public string CurrentArgumentName {
			get {
				if(CurrentArgumentIndex < 0 || CurrentArgumentIndex >= Arguments.Count)
					return String.Empty;
				string result = Arguments[CurrentArgumentIndex].Name;
				if(!String.IsNullOrEmpty(result))
					result += ":";
				return result;
			}
		}
		public string CurrentArgumentDescription {
			get {
				if(CurrentArgumentIndex < 0 || CurrentArgumentIndex >= Arguments.Count)
					return String.Empty;
				return Arguments[CurrentArgumentIndex].Description;
			}
		}
		public string CallBody { get { return currentFunctionPresentation == null ? string.Empty : currentFunctionPresentation.Formula; } }
		public string FormulaBody { get { return currentFunctionPresentation == null ? string.Empty : currentFunctionPresentation.Formula; } }
		public string CallResult {
			get {
				ExpresionCalculationResult result = FunctionArgumentViewModel.CalculateExpressionResult(CallBody, DocumentModel);
				if(result.IsInvalid)
					return CallBody;
				return result.Result;
			}
		}
		#endregion
		#region FunctionArgumentsFakeParameters
		static Dictionary<XtraSpreadsheetFunctionArgumentId, FakeParameterType> fakeArguments = PrepareFakeArguments();
		static Dictionary<XtraSpreadsheetFunctionArgumentId, FakeParameterType> PrepareFakeArguments() {
			Dictionary<XtraSpreadsheetFunctionArgumentId, FakeParameterType> result = new Dictionary<XtraSpreadsheetFunctionArgumentId, FakeParameterType> {
				{XtraSpreadsheetFunctionArgumentId.AbsNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.AccrintIssue, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.AccrintFirstinterest, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.AccrintSettlement, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.AccrintRate, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.AccrintPar, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.AccrintmIssue, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.AccrintmSettlement, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.AccrintmRate, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.AccrintmPar, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.AccrintmBasis, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.ACosNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ACosHNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ACotNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ACotHNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.AddressRownum, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.AddressColumnnum, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.AddressAbsnum, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.AddressA1, FakeParameterType.Logical},
				{XtraSpreadsheetFunctionArgumentId.AddressSheettext, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.AggregateFunctionnum, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.AggregateOptions, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.AggregateArray, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.AggregateK, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.AmordegrcCost, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.AmordegrcDatepurchased, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.AmordegrcFirstperiod, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.AmordegrcSalvage, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.AmordegrcPeriod, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.AmorlincCost, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.AmorlincDatepurchased, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.AmorlincFirstperiod, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.AmorlincSalvage, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.AmorlincPeriod, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.AndLogical1, FakeParameterType.Logical},
				{XtraSpreadsheetFunctionArgumentId.AndLogical2, FakeParameterType.Logical},
				{XtraSpreadsheetFunctionArgumentId.ArabicText, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.AreasReference, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.AscText, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.ASinNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ASinHNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ATanNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ATan2Xnum, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ATan2Ynum, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ATanHNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.AvedevNumber1, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.AvedevNumber2, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.AverageNumber1, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.AverageNumber2, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.AverageAValue1, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.AverageAValue2, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.AverageIfRange, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.AverageIfCriteria, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.AverageIfAveragerange, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.AverageIfsAveragerange, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.AverageIfsCriteriarange, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.BahtTextNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.BaseNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.BaseRadix, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.BaseMinlength, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.BesselIX, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.BesselIN, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.BesselJX, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.BesselJN, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.BesselKX, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.BesselKN, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.BesselYX, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.BesselYN, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.BetaDistX, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.BetaDistAlpha, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.BetaDistBeta, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.BetaDistCumulative, FakeParameterType.Logical},
				{XtraSpreadsheetFunctionArgumentId.BetaDistA, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.BetaDotInvProbability, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.BetaDotInvAlpha, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.BetaDotInvBeta, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.BetaDotInvA, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.BetaDotInvB, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.BetaDistCompatibilityX, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.BetaDistCompatibilityAlpha, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.BetaDistCompatibilityBeta, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.BetaDistCompatibilityA, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.BetaDistCompatibilityB, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.BetaInvProbability, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.BetaInvAlpha, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.BetaInvBeta, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.BetaInvA, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.BetaInvB, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.Bin2DecNumber, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.Bin2HexNumber, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.Bin2HexPlaces, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.Bin2OctNumber, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.Bin2OctPlaces, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.BinomDotDistNumbers, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.BinomDotDistTrials, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.BinomDotDistProbabilitys, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.BinomDotDistCumulative, FakeParameterType.Logical},
				{XtraSpreadsheetFunctionArgumentId.BinomDistRangeTrials, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.BinomDistRangeProbabilitys, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.BinomDistRangeNumbers, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.BinomDistRangeNumbers2, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.BinomDotInvTrials, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.BinomDotInvProbabilitys, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.BinomDotInvAlpha, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.BinomDistNumbers, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.BinomDistTrials, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.BinomDistProbabilitys, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.BinomDistCumulative, FakeParameterType.Logical},
				{XtraSpreadsheetFunctionArgumentId.BitAndNumber1, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.BitAndNumber2, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.BitLShiftNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.BitLShiftShiftamount, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.BitOrNumber1, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.BitOrNumber2, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.BitRShiftNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.BitRShiftShiftamount, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.BitXorNumber1, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.BitXorNumber2, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.CeilingNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.CeilingSignificance, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.CeilingMathNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.CeilingMathSignificance, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.CeilingMathMode, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.CeilingDotPreciseNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.CeilingDotPreciseSignificance, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.CellInfotype, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.CellReference, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.CharNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ChiDistX, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ChiDistDegfreedom, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ChiInvProbability, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ChiInvDegfreedom, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ChisqDotDistX, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ChisqDotDistDegfreedom, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ChisqDotDistCumulative, FakeParameterType.Logical},
				{XtraSpreadsheetFunctionArgumentId.ChisqDotDistDotRtX, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ChisqDotDistDotRtDegfreedom, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ChisqDotInvProbability, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ChisqDotInvDegfreedom, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ChisqDotInvDotRtProbability, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ChisqDotInvDotRtDegfreedom, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ChisqDotTestActualrange, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.ChisqDotTestExpectedrange, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.ChiTestActualrange, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.ChiTestExpectedrange, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.ChooseIndexnum, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ChooseValue1, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.ChooseValue2, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.CleanText, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.CodeText, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.ColumnReference, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.ColumnsArray, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.CombinNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.CombinNumberchosen, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.CombinANumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.CombinANumberchosen, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ComplexRealnum, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.ComplexInum, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.ComplexSuffix, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.ConcatenateText1, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.ConcatenateText2, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.ConfidenceAlpha, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ConfidenceStandarddev, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ConfidenceSize, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ConfidenceNormAlpha, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ConfidenceNormStandarddev, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ConfidenceNormSize, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ConfidenceDotTAlpha, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ConfidenceDotTStandarddev, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ConfidenceDotTSize, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ConvertNumber, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.ConvertFromunit, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.ConvertTounit, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.CorrelArray1, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.CorrelArray2, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.CosNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.CosHNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.CotNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.CotHNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.CountValue1, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.CountValue2, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.CountAValue1, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.CountAValue2, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.CountBlankRange, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.CountIfRange, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.CountIfCriteria, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.CountIfsCriteriarange, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.CountIfsCriteria, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.CoupDaybsSettlement, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.CoupDaybsMaturity, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.CoupDaybsFrequency, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.CoupDaybsBasis, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.CoupDaysSettlement, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.CoupDaysMaturity, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.CoupDaysFrequency, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.CoupDaysBasis, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.CoupDaysncSettlement, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.CoupDaysncMaturity, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.CoupDaysncFrequency, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.CoupDaysncBasis, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.CoupncdSettlement, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.CoupncdMaturity, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.CoupncdFrequency, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.CoupncdBasis, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.CoupNumSettlement, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.CoupNumMaturity, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.CoupNumFrequency, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.CoupNumBasis, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.CouppcdSettlement, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.CouppcdMaturity, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.CouppcdFrequency, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.CouppcdBasis, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.CovarArray1, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.CovarArray2, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.CovariancePArray1, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.CovariancePArray2, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.CovarianceSArray1, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.CovarianceSArray2, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.CritBinomTrials, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.CritBinomProbabilitys, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.CritBinomAlpha, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.CscNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.CscHNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.CubeKpiMemberConnection, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.CubeKpiMemberKpiname, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.CubeKpiMemberKpiproperty, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.CubeKpiMemberCaption, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.CubeMemberConnection, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.CubeMemberMemberexpression, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.CubeMemberCaption, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.CubeMemberPropertyConnection, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.CubeMemberPropertyMemberexpression, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.CubeMemberPropertyProperty, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.CubeRankedMemberConnection, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.CubeRankedMemberSetexpression, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.CubeRankedMemberRank, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.CubeRankedMemberCaption, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.CubeSetConnection, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.CubeSetSetexpression, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.CubeSetCaption, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.CubeSetSortorder, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.CubeSetSortby, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.CubeSetCountSet, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.CubeValueConnection, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.CubeValueMemberexpression1, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.CumIpmtRate, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.CumIpmtNper, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.CumIpmtPv, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.CumIpmtStartperiod, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.CumIpmtEndperiod, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.CumPrincRate, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.CumPrincNper, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.CumPrincPv, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.CumPrincStartperiod, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.CumPrincEndperiod, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.DateYear, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.DateMonth, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.DateDay, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.DateValueDatetext, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.DAverageDatabase, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.DAverageField, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.DAverageCriteria, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.DaySerialnumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.DaysEnddate, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.DaysStartdate, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.Days360Startdate, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.Days360Enddate, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.Days360Method, FakeParameterType.Logical},
				{XtraSpreadsheetFunctionArgumentId.DbCost, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.DbSalvage, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.DbLife, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.DbPeriod, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.DbMonth, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.DCountDatabase, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.DCountField, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.DCountCriteria, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.DCountADatabase, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.DCountAField, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.DCountACriteria, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.DdbCost, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.DdbSalvage, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.DdbLife, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.DdbPeriod, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.DdbFactor, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.Dec2BinNumber, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.Dec2BinPlaces, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.Dec2HexNumber, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.Dec2HexPlaces, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.Dec2OctNumber, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.Dec2OctPlaces, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.DecimalNumber, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.DecimalRadix, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.DegreesAngle, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.DeltaNumber1, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.DeltaNumber2, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.DevSqNumber1, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.DevSqNumber2, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.DGetDatabase, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.DGetField, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.DGetCriteria, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.DiscSettlement, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.DiscMaturity, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.DiscPr, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.DiscRedemption, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.DiscBasis, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.DMaxDatabase, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.DMaxField, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.DMaxCriteria, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.DMinDatabase, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.DMinField, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.DMinCriteria, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.DollarNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.DollarDecimals, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.DollarDeFractionaldollar, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.DollarDeFraction, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.DollarFrDecimaldollar, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.DollarFrFraction, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.DProductDatabase, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.DProductField, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.DProductCriteria, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.DStDevDatabase, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.DStDevField, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.DStDevCriteria, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.DStDevPDatabase, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.DStDevPField, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.DStDevPCriteria, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.DSumDatabase, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.DSumField, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.DSumCriteria, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.DurationSettlement, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.DurationMaturity, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.DurationCoupon, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.DurationYld, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.DurationFrequency, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.DVarDatabase, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.DVarField, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.DVarCriteria, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.DVarPDatabase, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.DVarPField, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.DVarPCriteria, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.EDateStartdate, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.EDateMonths, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.EffectNominalrate, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.EffectNpery, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.EncodeURLText, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.EOMonthStartdate, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.EOMonthMonths, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.ErfLowerlimit, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.ErfUpperlimit, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.ErfPreciseX, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.ErfcX, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.ErfcPreciseX, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.ErrorTypeErrorval, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.EvenNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ExactText1, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.ExactText2, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.ExpNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ExponDotDistX, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ExponDotDistLambda, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ExponDotDistCumulative, FakeParameterType.Logical},
				{XtraSpreadsheetFunctionArgumentId.ExponDistX, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ExponDistLambda, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ExponDistCumulative, FakeParameterType.Logical},
				{XtraSpreadsheetFunctionArgumentId.FDotDistX, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.FDotDistDegfreedom1, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.FDotDistDegfreedom2, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.FDotDistCumulative, FakeParameterType.Logical},
				{XtraSpreadsheetFunctionArgumentId.FDotDistDotRtX, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.FDotDistDotRtDegfreedom1, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.FDotDistDotRtDegfreedom2, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.FDotInvProbability, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.FDotInvDegfreedom1, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.FDotInvDegfreedom2, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.FDotinvDotRtProbability, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.FDotinvDotRtDegfreedom1, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.FDotinvDotRtDegfreedom2, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.FDotTestArray1, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.FDotTestArray2, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.FactNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.FactDoubleNumber, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.FDistX, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.FDistDegfreedom1, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.FDistDegfreedom2, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.FunctionFilterXMLXml, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.FunctionFilterXMLXpath, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.FindFindtext, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.FindWithintext, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.FindStartnum, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.FindBFindtext, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.FindBWithintext, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.FindBStartnum, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.FInvProbability, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.FInvDegfreedom1, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.FInvDegfreedom2, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.FisherX, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.FisherInvY, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.FixedNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.FixedDecimals, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.FixedNocommas, FakeParameterType.Logical},
				{XtraSpreadsheetFunctionArgumentId.FloorNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.FloorSignificance, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.FloorMathNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.FloorMathSignificance, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.FloorMathMode, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.FloorDotPreciseNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.FloorDotPreciseSignificance, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ForecastX, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ForecastKnownys, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.ForecastKnownxs, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.FormulaTextReference, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.FrequencyDataarray, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.FrequencyBinsarray, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.FTestArray1, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.FTestArray2, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.FvRate, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.FvNper, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.FvPmt, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.FvPv, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.FvType, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.FvSchedulePrincipal, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.FvScheduleSchedule, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.GammaX, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.GammaDotDistX, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.GammaDotDistAlpha, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.GammaDotDistBeta, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.GammaDotDistCumulative, FakeParameterType.Logical},
				{XtraSpreadsheetFunctionArgumentId.GammaDotInvProbability, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.GammaDotInvAlpha, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.GammaDotInvBeta, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.GammaDistX, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.GammaDistAlpha, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.GammaDistBeta, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.GammaDistCumulative, FakeParameterType.Logical},
				{XtraSpreadsheetFunctionArgumentId.GammaInvProbability, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.GammaInvAlpha, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.GammaInvBeta, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.GammaLnX, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.GammaLnPreciseX, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.GcdNumber1, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.GcdNumber2, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.GeomeanNumber1, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.GeomeanNumber2, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.GestepNumber, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.GestepStep, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.GetPivotDataDatafield, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.GetPivotDataPivottable, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.GrowthKnownys, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.GrowthKnownxs, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.GrowthNewxs, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.GrowthConst, FakeParameterType.Logical},
				{XtraSpreadsheetFunctionArgumentId.HarMeanNumber1, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.HarMeanNumber2, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.Hex2BinNumber, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.Hex2BinPlaces, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.Hex2DecNumber, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.Hex2OctNumber, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.Hex2OctPlaces, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.HLookupLookupvalue, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.HLookupTablearray, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.HLookupRowindexnum, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.HLookupRangelookup, FakeParameterType.Logical},
				{XtraSpreadsheetFunctionArgumentId.HourSerialnumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.HyperlinkLinklocation, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.HyperlinkFriendlyname, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.HypgeomDotDistSamples, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.HypgeomDotDistNumbersample, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.HypgeomDotDistPopulations, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.HypgeomDotDistNumberpop, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.HypgeomDotDistCumulative, FakeParameterType.Logical},
				{XtraSpreadsheetFunctionArgumentId.HypGeomDistSamples, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.HypGeomDistNumbersample, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.HypGeomDistPopulations, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.HypGeomDistNumberpop, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.IfLogicaltest, FakeParameterType.Logical},
				{XtraSpreadsheetFunctionArgumentId.IfValueiftrue, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.IfValueiffalse, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.IfErrorValue, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.IfErrorValueiferror, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.IfNAValue, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.IfNAValueifna, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.ImAbsInumber, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.ImaginaryInumber, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.ImArgumentInumber, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.ImConjugateInumber, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.ImCosInumber, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.ImCosHInumber, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.ImCotInumber, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.ImCscInumber, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.ImCscHInumber, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.ImDivInumber1, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.ImDivInumber2, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.ImExpInumber, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.ImLnInumber, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.ImLog2Inumber, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.ImLog10Inumber, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.ImPowerInumber, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.ImPowerNumber, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.ImProductInumber1, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.ImProductInumber2, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.ImRealInumber, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.ImSecInumber, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.ImSecHInumber, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.ImSinInumber, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.ImSinHInumber, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.ImSqrtInumber, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.ImSubInumber1, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.ImSubInumber2, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.ImSumInumber1, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.ImSumInumber2, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.ImTanInumber, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.IndexArray, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.IndexRownum, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.IndexColumnnum, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.IndirectReftext, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.IndirectA1, FakeParameterType.Logical},
				{XtraSpreadsheetFunctionArgumentId.InfoTypetext, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.IntNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.InterceptKnownys, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.InterceptKnownxs, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.IntrateSettlement, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.IntrateMaturity, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.IntrateInvestment, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.IntrateRedemption, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.IntrateBasis, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.IpmtRate, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.IpmtPer, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.IpmtNper, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.IpmtPv, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.IpmtFv, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.IrrValues, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.IrrGuess, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.IsBlankValue, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.IsErrValue, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.IsErrorValue, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.IsEvenNumber, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.IsFormulaReference, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.IsLogicalValue, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.IsNAValue, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.IsNonTextValue, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.IsNumberValue, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.IsoCeilingNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.IsoCeilingSignificance, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.IsOddNumber, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.ISOWeekNumDate, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.IsPmtRate, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.IsPmtPer, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.IsPmtNper, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.IsPmtPv, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.IsRefValue, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.IsTextValue, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.KurtNumber1, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.KurtNumber2, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.LargeArray, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.LargeK, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.LcmNumber1, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.LcmNumber2, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.LeftText, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.LeftNumchars, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.LeftBText, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.LeftBNumbytes, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.LenText, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.LenBText, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.LinestKnownys, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.LinestKnownxs, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.LinestConst, FakeParameterType.Logical},
				{XtraSpreadsheetFunctionArgumentId.LinestStats, FakeParameterType.Logical},
				{XtraSpreadsheetFunctionArgumentId.LnNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.LogNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.LogBase, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.Log10Number, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.LogestKnownys, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.LogestKnownxs, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.LogestConst, FakeParameterType.Logical},
				{XtraSpreadsheetFunctionArgumentId.LogestStats, FakeParameterType.Logical},
				{XtraSpreadsheetFunctionArgumentId.LogInvCompatibilityProbability, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.LogInvCompatibilityMean, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.LogInvCompatibilityStandarddev, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.LogNormDistX, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.LogNormDistMean, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.LogNormDistStandarddev, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.LogNormDistCumulative, FakeParameterType.Logical},
				{XtraSpreadsheetFunctionArgumentId.LogNormInvProbability, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.LogNormInvMean, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.LogNormInvStandarddev, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.LogNormDistCompatibilityX, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.LogNormDistCompatibilityMean, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.LogNormDistCompatibilityStandarddev, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.LookupLookupvalue, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.LookupLookupvector, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.LookupResultvector, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.LowerText, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.MatchLookupvalue, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.MatchLookuparray, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.MatchMatchtype, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.MaxNumber1, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.MaxNumber2, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.MaxAValue1, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.MaxAValue2, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.MDetermArray, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.MDurationSettlement, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.MDurationMaturity, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.MDurationCoupon, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.MDurationYld, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.MDurationFrequency, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.MedianNumber1, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.MedianNumber2, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.MidText, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.MidStartnum, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.MidNumchars, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.MidBText, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.MidBStartnum, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.MidBNumbytes, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.MinNumber1, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.MinNumber2, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.MinAValue1, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.MinAValue2, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.MinuteSerialnumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.MInverseArray, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.MirrValues, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.MirrFinancerate, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.MirrReinvestrate, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.MMultArray1, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.MMultArray2, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.ModNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ModDivisor, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ModeNumber1, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.ModeNumber2, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.ModeMultNumber1, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.ModeMultNumber2, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.ModeSnglNumber1, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.ModeSnglNumber2, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.MonthSerialnumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.MRoundNumber, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.MRoundMultiple, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.MultinomialNumber1, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.MultinomialNumber2, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.MUnitDimension, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.NValue, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.NegbinomDotDistNumberf, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.NegbinomDotDistNumbers, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.NegbinomDotDistProbabilitys, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.NegbinomDotDistCumulative, FakeParameterType.Logical},
				{XtraSpreadsheetFunctionArgumentId.NegBinomDistNumberf, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.NegBinomDistNumbers, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.NegBinomDistProbabilitys, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.NetworkDaysStartdate, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.NetworkDaysEnddate, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.NetworkDaysHolidays, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.NetworkDaysIntlStartdate, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.NetworkDaysIntlEnddate, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.NetworkDaysIntlWeekend, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.NetworkDaysIntlHolidays, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.NominalEffectrate, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.NominalNpery, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.NormDistX, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.NormDistMean, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.NormDistStandarddev, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.NormDistCumulative, FakeParameterType.Logical},
				{XtraSpreadsheetFunctionArgumentId.NormInvProbability, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.NormInvMean, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.NormInvStandarddev, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.NormSDistZ, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.NormSDistCumulative, FakeParameterType.Logical},
				{XtraSpreadsheetFunctionArgumentId.NormSInvProbability, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.NormDistCompatibilityX, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.NormDistCompatibilityMean, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.NormDistCompatibilityStandarddev, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.NormDistCompatibilityCumulative, FakeParameterType.Logical},
				{XtraSpreadsheetFunctionArgumentId.NormInvCompatibilityProbability, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.NormInvCompatibilityMean, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.NormInvCompatibilityStandarddev, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.NormSDistCompatibilityZ, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.NormSInvCompatibilityProbability, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.NotLogical, FakeParameterType.Logical},
				{XtraSpreadsheetFunctionArgumentId.NPerRate, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.NPerPmt, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.NPerPv, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.NPerFv, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.NPerType, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.NpvRate, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.NpvValue1, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.NpvValue2, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.NumberValueText, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.NumberValueDecimalseparator, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.NumberValueGroupseparator, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.Oct2BinNumber, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.Oct2BinPlaces, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.Oct2DecNumber, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.Oct2HexNumber, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.Oct2HexPlaces, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.OddNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.OddFPriceSettlement, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.OddFPriceMaturity, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.OddFPriceIssue, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.OddFPriceFirstcoupon, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.OddFPriceRate, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.OddFYieldSettlement, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.OddFYieldMaturity, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.OddFYieldIssue, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.OddFYieldFirstcoupon, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.OddFYieldRate, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.OddLPriceSettlement, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.OddLPriceMaturity, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.OddLPriceLastinterest, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.OddLPriceRate, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.OddLPriceYld, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.OddLYieldSettlement, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.OddLYieldMaturity, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.OddLYieldLastinterest, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.OddLYieldRate, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.OddLYieldPr, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.OffsetReference, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.OffsetRows, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.OffsetCols, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.OffsetHeight, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.OffsetWidth, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.OrLogical1, FakeParameterType.Logical},
				{XtraSpreadsheetFunctionArgumentId.OrLogical2, FakeParameterType.Logical},
				{XtraSpreadsheetFunctionArgumentId.PDurationRate, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.PDurationPv, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.PDurationFv, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.PearsonArray1, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.PearsonArray2, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.PercentileArray, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.PercentileK, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.PercentileExcArray, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.PercentileExcK, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.PercentileIncArray, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.PercentileIncK, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.PercentRankArray, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.PercentRankX, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.PercentRankSignificance, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.PercentRankExcArray, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.PercentRankExcX, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.PercentRankExcSignificance, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.PercentRankIncArray, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.PercentRankIncX, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.PercentRankIncSignificance, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.PermutNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.PermutNumberchosen, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.PermutationaNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.PermutationaNumberchosen, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.PhiX, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.PhoneticReference, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.PmtRate, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.PmtNper, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.PmtPv, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.PmtFv, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.PmtType, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.PoissonX, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.PoissonMean, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.PoissonCumulative, FakeParameterType.Logical},
				{XtraSpreadsheetFunctionArgumentId.PoissonDotDistX, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.PoissonDotDistMean, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.PoissonDotDistCumulative, FakeParameterType.Logical},
				{XtraSpreadsheetFunctionArgumentId.PowerNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.PowerPower, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.PpmtRate, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.PpmtPer, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.PpmtNper, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.PpmtPv, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.PpmtFv, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.PriceSettlement, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.PriceMaturity, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.PriceRate, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.PriceYld, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.PriceRedemption, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.PriceDiscSettlement, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.PriceDiscMaturity, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.PriceDiscDiscount, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.PriceDiscRedemption, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.PriceDiscBasis, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.PriceMatSettlement, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.PriceMatMaturity, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.PriceMatIssue, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.PriceMatRate, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.PriceMatYld, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.ProbXrange, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.ProbProbrange, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.ProbLowerlimit, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ProbUpperlimit, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ProductNumber1, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ProductNumber2, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ProperText, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.PvRate, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.PvNper, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.PvPmt, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.PvFv, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.PvType, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.QuartileArray, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.QuartileQuart, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.QuartileExcArray, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.QuartileExcQuart, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.QuartileIncArray, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.QuartileIncQuart, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.QuotientNumerator, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.QuotientDenominator, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.RadiansAngle, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.RandBetweenBottom, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.RandBetweenTop, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.RankNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.RankRef, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.RankOrder, FakeParameterType.Logical},
				{XtraSpreadsheetFunctionArgumentId.RankDotAvgNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.RankDotAvgRef, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.RankDotAvgOrder, FakeParameterType.Logical},
				{XtraSpreadsheetFunctionArgumentId.RankDotEqNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.RankDotEqRef, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.RankDotEqOrder, FakeParameterType.Logical},
				{XtraSpreadsheetFunctionArgumentId.RateNper, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.RatePmt, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.RatePv, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.RateFv, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.RateType, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ReceivedSettlement, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.ReceivedMaturity, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.ReceivedInvestment, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.ReceivedDiscount, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.ReceivedBasis, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.ReplaceOldtext, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.ReplaceStartnum, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ReplaceNumchars, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ReplaceNewtext, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.ReplaceBOldtext, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.ReplaceBStartnum, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ReplaceBNumbytes, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ReplaceBNewtext, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.ReptText, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.ReptNumbertimes, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.RightText, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.RightNumchars, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.RightBText, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.RightBNumbytes, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.RomanNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.RomanForm, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.RoundNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.RoundNumdigits, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.RoundDownNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.RoundDownNumdigits, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.RoundUpNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.RoundUpNumdigits, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.RowReference, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.RowsArray, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.RRINper, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.RRIPv, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.RRIFv, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.RsqKnownys, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.RsqKnownxs, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.RtdProgID, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.RtdServer, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.RtdTopic1, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.RtdTopic2, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.SearchFindtext, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.SearchWithintext, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.SearchStartnum, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.SearchBFindtext, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.SearchBWithintext, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.SearchBStartnum, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.SecNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.SecHNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.SecondSerialnumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.SeriessumX, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.SeriessumN, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.SeriessumM, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.SeriessumCoefficients, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.SheetValue, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.SheetsReference, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.SignNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.SinNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.SinHNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.SkewNumber1, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.SkewNumber2, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.SkewPNumber1, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.SkewPNumber2, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.SlnCost, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.SlnSalvage, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.SlnLife, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.SlopeKnownys, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.SlopeKnownxs, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.SmallArray, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.SmallK, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.SqrtNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.SqrtPiNumber, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.StandardizeX, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.StandardizeMean, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.StandardizeStandarddev, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.StDevNumber1, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.StDevNumber2, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.StDevDotPNumber1, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.StDevDotPNumber2, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.StDevDotSNumber1, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.StDevDotSNumber2, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.StDevAValue1, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.StDevAValue2, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.StDevPNumber1, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.StDevPNumber2, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.StDevPAValue1, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.StDevPAValue2, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.StEYXKnownys, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.StEYXKnownxs, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.SubstituteText, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.SubstituteOldtext, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.SubstituteNewtext, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.SubstituteInstancenum, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.SubtotalFunctionnum, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.SubtotalRef1, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.SumNumber1, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.SumNumber2, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.SumIfRange, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.SumIfCriteria, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.SumIfSumrange, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.SumIfsSumrange, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.SumIfsCriteriarange, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.SumIfsCriteria, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.SumProductArray1, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.SumProductArray2, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.SumProductArray3, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.SumSqNumber1, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.SumSqNumber2, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.SumX2MY2Arrayx, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.SumX2MY2Arrayy, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.SumX2PY2Arrayx, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.SumX2PY2Arrayy, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.SumXMY2Arrayx, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.SumXMY2Arrayy, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.SydCost, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.SydSalvage, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.SydLife, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.SydPer, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.TValue, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.TDotDistX, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.TDotDistDegfreedom, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.TDotDistCumulative, FakeParameterType.Logical},
				{XtraSpreadsheetFunctionArgumentId.TDotDistDot2tX, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.TDotDistDot2tDegfreedom, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.TDotDistDotTtX, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.TDotDistDotTtDegfreedom, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.TDotInvProbability, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.TDotInvDegfreedom, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.TDotInvDot2tProbability, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.TDotInvDot2tDegfreedom, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.TDotTestArray1, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.TDotTestArray2, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.TDotTestTails, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.TDotTestType, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.TanNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.TanHNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.TbillEqSettlement, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.TbillEqMaturity, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.TbillEqDiscount, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.TbillPriceSettlement, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.TbillPriceMaturity, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.TbillPriceDiscount, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.TbillYieldSettlement, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.TbillYieldMaturity, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.TbillYieldPr, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.TDistX, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.TDistDegfreedom, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.TDistTails, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.TextValue, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.TextFormattext, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.TimeHour, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.TimeMinute, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.TimeSecond, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.TimeValueTimetext, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.TinvProbability, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.TinvDegfreedom, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.TransposeArray, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.TrendKnownys, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.TrendKnownxs, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.TrendNewxs, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.TrendConst, FakeParameterType.Logical},
				{XtraSpreadsheetFunctionArgumentId.TrimText, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.TrimmeanArray, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.TrimmeanPercent, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.TruncNumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.TruncNumdigits, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.TTestArray1, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.TTestArray2, FakeParameterType.Array},
				{XtraSpreadsheetFunctionArgumentId.TTestTails, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.TTestType, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.TypeValue, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.UnicodeText, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.UpperText, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.ValueText, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.VarNumber1, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.VarNumber2, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.VarDotPNumber1, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.VarDotPNumber2, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.VarDotSNumber1, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.VarDotSNumber2, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.VarAValue1, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.VarAValue2, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.VarPNumber1, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.VarPNumber2, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.VarPAValue1, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.VarPAValue2, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.VdbCost, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.VdbSalvage, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.VdbLife, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.VdbStartperiod, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.VdbEndperiod, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.VLookupLookupvalue, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.VLookupTablearray, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.VLookupColindexnum, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.VLookupRangelookup, FakeParameterType.Logical},
				{XtraSpreadsheetFunctionArgumentId.FunctionWebServiceUrl, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.WeekDaySerialnumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.WeekDayReturntype, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.WeekNumSerialnumber, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.WeekNumReturntype, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.WeibullX, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.WeibullAlpha, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.WeibullBeta, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.WeibullCumulative, FakeParameterType.Logical},
				{XtraSpreadsheetFunctionArgumentId.WeibullDotDistX, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.WeibullDotDistAlpha, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.WeibullDotDistBeta, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.WeibullDotDistCumulative, FakeParameterType.Logical},
				{XtraSpreadsheetFunctionArgumentId.WorkDayStartdate, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.WorkDayDays, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.WorkDayHolidays, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.WorkDayIntlStartdate, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.WorkDayIntlDays, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.WorkDayIntlWeekend, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.WorkDayIntlHolidays, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.XirrValues, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.XirrDates, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.XirrGuess, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.XnpvRate, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.XnpvValues, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.XnpvDates, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.XOrLogical1, FakeParameterType.Logical},
				{XtraSpreadsheetFunctionArgumentId.XOrLogical2, FakeParameterType.Logical},
				{XtraSpreadsheetFunctionArgumentId.YearSerialnumber, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.YearFracStartdate, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.YearFracEnddate, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.YearFracBasis, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.YieldSettlement, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.YieldMaturity, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.YieldRate, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.YieldPr, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.YieldRedemption, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.YieldDiscSettlement, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.YieldDiscMaturity, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.YieldDiscPr, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.YieldDiscRedemption, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.YieldDiscBasis, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.YieldMatSettlement, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.YieldMatMaturity, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.YieldMatIssue, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.YieldMatRate, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.YieldMatPr, FakeParameterType.Any},
				{XtraSpreadsheetFunctionArgumentId.ZDotTestArray, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ZDotTestX, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ZDotTestSigma, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ZTestArray, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ZTestX, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ZTestSigma, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.FieldDataFieldName, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.RangeCellRefenrece, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.FieldPictureDataFieldName, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.FieldPicturePicturePlacement, FakeParameterType.Text},
				{XtraSpreadsheetFunctionArgumentId.FieldPictureTargetRange, FakeParameterType.Reference},
				{XtraSpreadsheetFunctionArgumentId.FieldPictureLockAspectRatio, FakeParameterType.Logical},
				{XtraSpreadsheetFunctionArgumentId.FieldPictureOffsetX, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.FieldPictureOffsetY, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.FieldPictureWidth, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.FieldPictureHeight, FakeParameterType.Number},
				{XtraSpreadsheetFunctionArgumentId.ParameterParameterName, FakeParameterType.Text},
			};
			return result;
		}
		#endregion
		public bool Update(string newFormulaBody, int newSelectionStart, int newSelectionLength) {
			if(lockUpdate != 0)
				return false;
			if(this.selectionStart == newSelectionStart && this.selectionLength == newSelectionLength && this.formulaBody == newFormulaBody) {
				return false;
			}
			this.selectionStart = newSelectionStart;
			this.selectionLength = newSelectionLength;
			if(this.formulaBody != newFormulaBody) {
				this.formulaBody = newFormulaBody;
				formulaParser.Update(formulaBody);
			}
			UpdateViewModel();
			return true;
		}
		void UpdateViewModel() {
			Function newFunction = formulaParser.Get(this.selectionStart, this.selectionLength);
			if(Equals(newFunction, currentFunction))
				return;
			currentFunction = newFunction;
			if(currentFunction != null) {
				FunctionPresentation newFunctionPresentation = new FunctionPresentation(currentFunction, DocumentModel, formulaBody);
				bool editSameFunction = currentFunctionPresentation != null && newFunctionPresentation.FunctionName == currentFunctionPresentation.FunctionName && newFunction.Left == currentFunction.Left;
				currentFunctionPresentation = newFunctionPresentation;
				if(editSameFunction) {
					UpdateArguments(currentFunctionPresentation);
				}
				else {
					PopulateArguments(currentFunctionPresentation);
				}
				CurrentArgumentIndex = currentFunctionPresentation.CurrentArgument;
			}
			else {
				arguments.Clear();
				currentFunctionPresentation = null;
			}
			OnPropertyChanged("CallBody");
			OnPropertyChanged("CallResult");
			OnPropertyChanged("FormulaBody");
			OnPropertyChanged("FormulaResult");
			OnPropertyChanged("Arguments");
		}
		void AddArgument(FunctionArgumentViewModel arg) {
			arg.PropertyChanged += OnArgumentChanged;
			arguments.Add(arg);
		}
		void OnArgumentChanged(object sender, PropertyChangedEventArgs e) {
			if(e.PropertyName != "Value")
				return;
			FunctionArgumentViewModel arg = (sender as FunctionArgumentViewModel);
			if(arg == null || !arg.IsVisible)
				return;
			int index = arguments.IndexOf(arg);
			if(index == -1)
				return;
			if(index < currentFunction.Arguments.Count && (currentFunctionPresentation.Args[index] == arg.Value))
				return;
			int count = arguments.Count;
			while(count > 1 && String.IsNullOrEmpty(arguments[count - 1].Value)) {
				count--;
			}
			string newArgs = FunctionPresentation.GenerateArguments(arguments, DocumentModel.DataContext.GetListSeparator(), count);
			string beforeText = formulaBody.Substring(0, currentFunctionPresentation.ArgumentsLeft);
			string afterText = currentFunctionPresentation.ArgumentsRight + 1 < formulaBody.Length ? formulaBody.Substring(currentFunctionPresentation.ArgumentsRight + 1) : String.Empty;
			string newFormulaBody = beforeText + newArgs + afterText;
			lockUpdate++;
			formulaBody = newFormulaBody;
			formulaParser.Update(formulaBody);
			lockUpdate--;
			currentFunction = formulaParser.Get(currentFunction.Left, 0);
			currentFunctionPresentation = new FunctionPresentation(currentFunction, DocumentModel, formulaBody);
			EditFunctionArgumentsCommand command = new EditFunctionArgumentsCommand(SpreadsheetControl);
			command.UpdateModel(newFormulaBody);
			OnPropertyChanged("CallBody");
			OnPropertyChanged("CallResult");
			OnPropertyChanged("FormulaBody");
			OnPropertyChanged("FormulaResult");
			OnPropertyChanged("Arguments");
		}
		void UpdateArguments(FunctionPresentation functionPresentation) {
			if(functionPresentation == null)
				return;
			if(functionPresentation.CallInfo == null) {
				for(int i = 0; i < functionPresentation.Args.Length; i++)
					if(arguments.Count <= i)
						AppendArgument(null, new FunctionParameter(OperandDataType.Default), functionPresentation.Args[i], true, -1);
					else
						UpdateArgument(null, arguments[i], new FunctionParameter(OperandDataType.Default), functionPresentation.Args[i], true, -1);
				for(int i = functionPresentation.Args.Length; i < arguments.Count; i++)
					arguments[i].IsVisible = false;
			}
			else {
				ISpreadsheetFunction function = functionPresentation.CallInfo.FuncThing.Function;
				int count = Math.Max(functionPresentation.Args.Length, function.Parameters.Count);
				if (function.Parameters.Count == 0) 
					count = 0;
				bool lastParameterUnlimited = function.Parameters.Count > 0 && function.Parameters[function.Parameters.Count - 1].Unlimited;
				FunctionParameter lastParameter = lastParameterUnlimited ? function.Parameters[function.Parameters.Count - 1] : new FunctionParameter(OperandDataType.Default);
				for(int i = 0; i < count; i++) {
					FunctionParameter parameter = i < function.Parameters.Count ? function.Parameters[i] : lastParameter;
					string value = i < functionPresentation.Args.Length ? functionPresentation.Args[i] : String.Empty;
					int index = function.Code * 1000 + i;
					if(arguments.Count <= i)
						AppendArgument(function, parameter, value, true, index);
					else
						UpdateArgument(function, arguments[i], parameter, value, true, index);
				}
				for(int i = count; i < arguments.Count; i++) {
					arguments[i].IsVisible = false;
					arguments[i].Value = "";
				}
			}
		}
		void PopulateArguments(FunctionPresentation functionPresentation) {
			arguments.Clear();
			if(functionPresentation == null)
				return;
			if(functionPresentation.CallInfo == null) {
				foreach(string arg in functionPresentation.Args)
					AppendArgument(null, new FunctionParameter(OperandDataType.Default), arg, true, -1);
			}
			else {
				ISpreadsheetFunction function = functionPresentation.CallInfo.FuncThing.Function;
				int count = Math.Max(functionPresentation.Args.Length, function.Parameters.Count);
				if(function.Parameters.Count == 0) 
					count = 0;
				bool lastParameterUnlimited = function.Parameters.Count > 0 && function.Parameters[function.Parameters.Count - 1].Unlimited;
				FunctionParameter lastParameter = lastParameterUnlimited ? function.Parameters[function.Parameters.Count - 1] : new FunctionParameter(OperandDataType.Default);
				for(int i = 0; i < count; i++) {
					int index = function.Code * 1000 + i;
					FunctionParameter parameter = i < function.Parameters.Count ? function.Parameters[i] : lastParameter;
					AppendArgument(function, parameter, i < functionPresentation.Args.Length ? functionPresentation.Args[i] : String.Empty, true, index);
				}
				if(count > 0) {
					if(lastParameterUnlimited && !(function is NotExistingFunction)) {
						for(int i = count; i < 256; i++) {
							int index = function.Code * 1000 + i;
							AppendArgument(function, lastParameter, String.Empty, false, index);
						}
					}
				}
			}
		}
		static Dictionary<FakeParameterType, string> parametersTypeDictionary = GetParametersTypeString();
		static Dictionary<FakeParameterType, string> GetParametersTypeString() {
			Dictionary<FakeParameterType, string> result = new Dictionary<FakeParameterType, string>();
			result.Add(FakeParameterType.Any, "any");
			result.Add(FakeParameterType.Array, "array");
			result.Add(FakeParameterType.Logical, "logical");
			result.Add(FakeParameterType.Number, "number");
			result.Add(FakeParameterType.Reference, "reference");
			result.Add(FakeParameterType.Text, "text");
			return result;
		}
		void CalculateParameterInfo(ISpreadsheetFunction function, int index, out string parameterName, out string parameterDescription, out string parameterResultType) {
			if(function is NotExistingFunction) {
				parameterName = String.Empty;
				parameterDescription = String.Empty;
				parameterResultType = String.Empty;
				return;
			}
			CalculateParameterInfoCore(function, index, out parameterName, out parameterDescription, out parameterResultType);
			if(parameterName.Length!=0 && !Char.IsUpper(parameterName[0]))
				parameterName = Char.ToUpperInvariant(parameterName[0]) + (parameterName.Length > 1 ? parameterName.Substring(1) : String.Empty);
		}
		void CalculateParameterInfoCore(ISpreadsheetFunction function, int index, out string parameterName, out string parameterDescription, out string parameterResultType) {
			bool isCustom = function is CustomFunction;
			int parametersCount = function.Parameters.Count;
			if(parametersCount == 0) {
				parameterName = String.Empty;
				parameterDescription = String.Empty;
				parameterResultType = parametersTypeDictionary[FakeParameterType.Any];
				return;
			}
			string realParameterName;
			if(isCustom ?
				TryGetParameterNameCustom(function, index, out realParameterName, out parameterDescription, out parameterResultType) :
				TryGetParameterNameBuiltIn(index, out realParameterName, out parameterDescription, out parameterResultType)) {
				parameterName = realParameterName;
				return;
			}
			int parameterDelta;
			int realIndex = CalculateParameterRealIndex(function, parametersCount, index, out parameterDelta);
			if(isCustom ? TryGetRealParameterNameCustom(function, realIndex, out realParameterName) : TryGetRealParameterNameBuiltIn(function, realIndex, out realParameterName)) {
				parameterName = realParameterName;
				parameterDescription = String.Empty;
				parameterResultType = String.Empty;
				return;
			}
			parameterName = CalculateUnlimitedParameterName(realParameterName, parameterDelta);
			parameterDescription = isCustom ? GetParameterDescriptionCustom(function, realIndex) : GetParameterDescriptionBuiltIn(function, realIndex);
			parameterResultType = isCustom ? GetParameterResultTypeCustom(function, realIndex) : GetParameterResultTypeBuiltIn(function, realIndex);
		}
		#region GetParameterDescription
		string GetParameterDescriptionBuiltIn(ISpreadsheetFunction function, int index) {
			index += function.Code * 1000;
			CultureInfo culture = DocumentModel.Culture;
			string parameterDescription = XtraSpreadsheetFunctionArgumentsDescriptionsResLocalizer.GetString((XtraSpreadsheetFunctionArgumentDescriptionStringId) (index), culture);
			return parameterDescription;
		}
		string GetParameterDescriptionCustom(ISpreadsheetFunction function, int index) {
			CustomFunctionDescription customFunctionDescription;
			if(!DocumentModel.CustomFunctionsDescriptions.TryGetValue(function.Name, out customFunctionDescription)) {
				return String.Empty;
			}
			if(index > customFunctionDescription.ParametersDescription.Count)
				return String.Empty;
			return customFunctionDescription.ParametersDescription[index];
		}
		#endregion
		#region GetParameterResultType
		string GetParameterResultTypeBuiltIn(ISpreadsheetFunction function, int index) {
			index += function.Code * 1000;
			string parameterResultType = parametersTypeDictionary[fakeArguments[(XtraSpreadsheetFunctionArgumentId) index]];
			return parameterResultType;
		}
		string GetParameterResultTypeCustom(ISpreadsheetFunction function, int index) {
			CustomFunctionDescription customFunctionDescription;
			if(!DocumentModel.CustomFunctionsDescriptions.TryGetValue(function.Name, out customFunctionDescription)) {
				return String.Empty;
			}
			if(index > customFunctionDescription.ReturnTypes.Count)
				return String.Empty;
			return customFunctionDescription.ReturnTypes[index];
		}
		#endregion
		#region TryGetParameterName
		bool TryGetParameterNameCustom(ISpreadsheetFunction function, int index, out string parameterName, out string parameterDescription, out string parameterResultType) {
			parameterName = String.Empty;
			parameterDescription = String.Empty;
			parameterResultType = String.Empty;
			CustomFunctionDescription customFunctionDescription;
			if(!DocumentModel.CustomFunctionsDescriptions.TryGetValue(function.Name, out customFunctionDescription)) {
				return true;
			}
			index %= 1000;
			if(index < customFunctionDescription.ParametersName.Count) {
				parameterName = customFunctionDescription.ParametersName[index];
				parameterDescription = GetParameterDescriptionCustom(function, index);
				parameterResultType = GetParameterResultTypeCustom(function, index);
				return true;
			}
			return false;
		}
		bool TryGetParameterNameBuiltIn(int index, out string parameterName, out string parameterDescription, out string parameterResultType) {
			CultureInfo culture = DocumentModel.Culture;
			string realParameterName = XtraSpreadsheetFunctionArgumentsNamesResLocalizer.GetString((XtraSpreadsheetFunctionArgumentNameStringId) (index), culture);
			if(!String.IsNullOrEmpty(realParameterName)) {
				parameterName = realParameterName;
				parameterDescription = XtraSpreadsheetFunctionArgumentsDescriptionsResLocalizer.GetString((XtraSpreadsheetFunctionArgumentDescriptionStringId) (index), culture);
				parameterResultType = parametersTypeDictionary[fakeArguments[(XtraSpreadsheetFunctionArgumentId) index]];
				return true;
			}
			parameterName = String.Empty;
			parameterDescription = String.Empty;
			parameterResultType = String.Empty;
			return false;
		}
		#endregion
		#region TryGetRealParameterName
		bool TryGetRealParameterNameCustom(ISpreadsheetFunction function, int realIndex, out string realParameterName) {
			CustomFunctionDescription customFunctionDescription;
			if(!DocumentModel.CustomFunctionsDescriptions.TryGetValue(function.Name, out customFunctionDescription)) {
				realParameterName = String.Empty;
				return true;
			}
			if(realIndex >= customFunctionDescription.ParametersName.Count) {
				realParameterName = String.Empty;
				return true;
			}
			realParameterName = customFunctionDescription.ParametersName[realIndex];
			return false;
		}
		bool TryGetRealParameterNameBuiltIn(ISpreadsheetFunction function, int realIndex, out string realParameterName) {
			if(realIndex >= function.Parameters.Count) {
				realParameterName = String.Empty;
				return true;
			}
			realIndex += function.Code * 1000;
			realParameterName = XtraSpreadsheetFunctionArgumentsNamesResLocalizer.GetString((XtraSpreadsheetFunctionArgumentNameStringId)(realIndex), DocumentModel.Culture);
			if(String.IsNullOrEmpty(realParameterName)) {
				realParameterName = String.Empty;
				return true;
			}
			return false;
		}
		#endregion
		static int CalculateParameterRealIndex(ISpreadsheetFunction function, int parametersCount, int index, out int parameterDelta) {
			int period = 0;
			while(period < parametersCount && function.Parameters[parametersCount - 1 - period].Unlimited) {
				period++;
			}
			if(period > 2) {
				parametersCount = parametersCount - period;
				period = 2;
			}
			if(period == 0)
				period = 1;
			int start = parametersCount - period;
			index %= 1000;
			int realIndex = start + (index - start) % period;
			parameterDelta = (index - start) / period;
			return realIndex;
		}
		static string CalculateUnlimitedParameterName(string realParameterName, int parameterDelta) {
			int numberLength = 0;
			while(numberLength < realParameterName.Length && Char.IsDigit(realParameterName[realParameterName.Length - 1 - numberLength])) {
				numberLength++;
			}
			int parameterNumber;
			int numberStart = realParameterName.Length - numberLength;
			if(numberLength == 0 || numberStart == 0) {
				parameterNumber = 1;
			}
			else if(numberStart == 0) {
				parameterNumber = String.IsNullOrEmpty(realParameterName) ? 1 : int.Parse(realParameterName);
				realParameterName = String.Empty;
			}
			else {
				parameterNumber = int.Parse(realParameterName.Substring(numberStart));
				realParameterName = realParameterName.Substring(0, numberStart);
			}
			realParameterName += (parameterNumber + parameterDelta).ToString();
			return realParameterName;
		}
		void UpdateArgument(ISpreadsheetFunction function, FunctionArgumentViewModel arg, FunctionParameter parameter, string value, bool visible, int index) {
			string parameterName;
			string parameterDescription;
			string parameterType;
			CalculateParameterInfo(function, index, out parameterName, out parameterDescription, out parameterType);
			arg.Name = parameterName;
			arg.ParameterType = parameterType;
			if(value != null)
				arg.Value = value;
			arg.Description = parameterDescription;
			arg.IsRequired = parameter.Required;
			arg.IsVisible = visible;
		}
		void AppendArgument(ISpreadsheetFunction function, FunctionParameter parameter, string value, bool visible, int index) {
			DocumentModel documentModel = SpreadsheetControl.InnerControl.DocumentModel;
			FunctionArgumentViewModel arg = new FunctionArgumentViewModel(documentModel);
			UpdateArgument(function, arg, parameter, value, visible, index);
			AddArgument(arg);
		}
		public void PrepareChanges() {
			ResultFormula = formulaBody;
			KeepChanges = true;
		}
		public void ApplyChanges() {
			EditFunctionArgumentsCommand command = new EditFunctionArgumentsCommand(SpreadsheetControl);
			command.UnSubscribeCommand();
			command.ApplyChanges(this);
		}
		public bool MayExecute() {
			return currentFunction != null;
		}
	}
	public class FunctionArgumentViewModel : ViewModelBase {
		#region Fields
		readonly DocumentModel documentModel;
		string name;
		string description;
		string value;
		string type;
		bool isRequired;
		bool isVisible;
		bool isInvalidOrError;
		#endregion
		public FunctionArgumentViewModel(DocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
		}
		#region Properties
		public string Name {
			get { return name; }
			set {
				if(Name == value)
					return;
				name = value;
				OnPropertyChanged("Name");
			}
		}
		public string Description {
			get { return description; }
			set {
				if(Description == value)
					return;
				description = value;
				OnPropertyChanged("Description");
			}
		}
		public string Value {
			get { return value; }
			set {
				if(Value == value)
					return;
				this.value = value;
				OnPropertyChanged("Value");
				OnPropertyChanged("ValueOrType");
			}
		}
		public string ParameterType {
			get { return type; }
			set {
				if(ParameterType == value)
					return;
				type = value;
				OnPropertyChanged("ParameterType");
				OnPropertyChanged("ValueOrType");
			}
		}
		public string ValueOrType {
			get {
				if(String.IsNullOrEmpty(Value))
					return ParameterType;
				ExpresionCalculationResult result = CalculateExpressionResult(Value, documentModel);
				this.IsInvalidOrError = result.IsInvalidOrError;
				return result.Result;
			}
		}
		public bool IsRequired {
			get { return isRequired; }
			set {
				if(IsRequired == value)
					return;
				isRequired = value;
				OnPropertyChanged("IsRequired");
			}
		}
		public bool IsVisible {
			get { return isVisible; }
			set {
				if(IsVisible == value)
					return;
				isVisible = value;
				OnPropertyChanged("IsVisible");
			}
		}
		public bool IsInvalidOrError {
			get { return isInvalidOrError; }
			set {
				if(IsInvalidOrError == value)
					return;
				isInvalidOrError = value;
				OnPropertyChanged("IsInvalidOrError");
			}
		}
		#endregion
		public static ExpresionCalculationResult CalculateExpressionResult(string expressionText, DocumentModel documentModel) {
			documentModel.DataContext.PushCurrentCell(documentModel.ActiveSheet.Selection.ActiveCell);
			ExpresionCalculationResult calculationResult = new ExpresionCalculationResult();
			try {
				ParsedExpression expression = documentModel.DataContext.ParseExpression(expressionText, OperandDataType.Default, false);
				if(expression == null) {
					calculationResult.IsInvalid = true;
					calculationResult.Result = GetInvalidExpressionText();
					return calculationResult;
				}
				VariantValue result = expression.Evaluate(documentModel.DataContext);
				if(result.IsCellRange && result.CellRangeValue.CellCount > 1)
					result = ArrayToText(new RangeVariantArray((CellRange) result.CellRangeValue), documentModel);
				else if(result.IsArray)
					result = ArrayToText(result.ArrayValue, documentModel);
				else
					result = result.ToText(documentModel.DataContext);
				calculationResult.IsError = result.IsError;
				calculationResult.Result = result.IsError ? result.ErrorValue.Name : result.GetTextValue(documentModel.SharedStringTable);
				return calculationResult;
			}
			catch {
				calculationResult.IsInvalid = true;
				calculationResult.Result = GetInvalidExpressionText();
				return calculationResult;
			}
			finally {
				documentModel.DataContext.PopCurrentCell();
			}
		}
		static string GetInvalidExpressionText() {
			return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_InvalidExpression);
		}
		static string ArrayToText(IVariantArray array, DocumentModel documentModel) {
			if(array == null)
				return String.Empty;
			ParsedThingArray element = new ParsedThingArray();
			element.ArrayValue = array;
			StringBuilder result = new StringBuilder();
			element.BuildExpressionString(new Stack<int>(), result, new StringBuilder(), documentModel.DataContext);
			return result.ToString();
		}
	}
	public class ExpresionCalculationResult {
		public string Result { get; set; }
		public bool IsInvalid { get; set; }
		public bool IsError { get; set; }
		public bool IsInvalidOrError { get { return IsInvalid || IsError; } }
	}
	public class FunctionCallItemParsedDataComparer : IComparer<FunctionCallItemParsedData> {
		public int Compare(FunctionCallItemParsedData x, FunctionCallItemParsedData y) {
			return Comparer<int>.Default.Compare(x.Length, y.Length);
		}
	}
	public class FunctionCallItemParsedDataPositionComparable : IComparable<FunctionCallItemParsedData> {
		readonly int position;
		public FunctionCallItemParsedDataPositionComparable(int position) {
			this.position = position;
		}
		public int CompareTo(FunctionCallItemParsedData other) {
			return -position.CompareTo(other.Position + other.Length);
		}
	}
}
