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
namespace DevExpress.XtraSpreadsheet.Model {
	public partial class FormulaCalculator {
		#region AddStatisticalFunctions
		public static void AddStatisticalFunctions(Dictionary<string, ISpreadsheetFunction> result) {
			AddFunction(result, new FunctionAveDev());
			AddFunction(result, new FunctionAverage());
			AddFunction(result, new FunctionAverageA());
			AddFunction(result, new FunctionAverageIf());
			AddFunction(result, new FunctionAverageIfs());
			AddFunction(result, new FunctionBetaDist());
			AddFunction(result, new FunctionBetaDotInv());
			AddFunction(result, new FunctionBinomDotDist());
			AddFunction(result, new FunctionBinomDistRange());
			AddFunction(result, new FunctionBinomDotInv());
			AddFunction(result, new FunctionChisqDotDist());
			AddFunction(result, new FunctionChisqDotDistDotRt());
			AddFunction(result, new FunctionChisqDotInv());
			AddFunction(result, new FunctionChisqDotInvDotRt());
			AddFunction(result, new FunctionChisqDotTest());
			AddFunction(result, new FunctionConfidenceNorm());
			AddFunction(result, new FunctionConfidenceDotT());
			AddFunction(result, new FunctionCorrel());
			AddFunction(result, new FunctionCount());
			AddFunction(result, new FunctionCountA());
			AddFunction(result, new FunctionCountBlank());
			AddFunction(result, new FunctionCountIf());
			AddFunction(result, new FunctionCountIfs());
			AddFunction(result, new FunctionCovarianceP());
			AddFunction(result, new FunctionCovarianceS());
			AddFunction(result, new FunctionDevsq());
			AddFunction(result, new FunctionExponDotDist());
			AddFunction(result, new FunctionFDotDist());
			AddFunction(result, new FunctionFDotDistDotRt());
			AddFunction(result, new FunctionFDotInv());
			AddFunction(result, new FunctionFDotInvDotRt());
			AddFunction(result, new FunctionFDotTest());
			AddFunction(result, new FunctionFisher());
			AddFunction(result, new FunctionFisherInv());
			AddFunction(result, new FunctionForecast());
			AddFunction(result, new FunctionFrequency());
			AddFunction(result, new FunctionGammaDotDist());
			AddFunction(result, new FunctionGammaDotInv());
			AddFunction(result, new FunctionGamma());
			AddFunction(result, new FunctionGammaLn());
			AddFunction(result, new FunctionGammaLnPrecise());
			AddFunction(result, new FunctionGauss());
			AddFunction(result, new FunctionGeomean());
			AddFunction(result, new FunctionGrowth());
			AddFunction(result, new FunctionHarMean());
			AddFunction(result, new FunctionHypGeomDotDist());
			AddFunction(result, new FunctionIntercept());
			AddFunction(result, new FunctionKurt());
			AddFunction(result, new FunctionLarge());
			AddFunction(result, new FunctionLinest());
			AddFunction(result, new FunctionLogest());
			AddFunction(result, new FunctionLogNormDist());
			AddFunction(result, new FunctionLogNormInv());
			AddFunction(result, new FunctionMax());
			AddFunction(result, new FunctionMaxA());
			AddFunction(result, new FunctionMedian());
			AddFunction(result, new FunctionMin());
			AddFunction(result, new FunctionMinA());
			AddFunction(result, new FunctionModeMult());
			AddFunction(result, new FunctionModeSngl());
			AddFunction(result, new FunctionNegBinomDotDist());
			AddFunction(result, new FunctionNormDist());
			AddFunction(result, new FunctionNormInv());
			AddFunction(result, new FunctionNormSDist());
			AddFunction(result, new FunctionNormSInv());
			AddFunction(result, new FunctionPearson());
			AddFunction(result, new FunctionPermutationa());
			AddFunction(result, new FunctionPhi());
			AddFunction(result, new FunctionPercentileExc());
			AddFunction(result, new FunctionPercentileInc());
			AddFunction(result, new FunctionPercentRankExc());
			AddFunction(result, new FunctionPercentRankInc());
			AddFunction(result, new FunctionPermut());
			AddFunction(result, new FunctionPoissonDotDist());
			AddFunction(result, new FunctionProb());
			AddFunction(result, new FunctionQuartileExc());
			AddFunction(result, new FunctionQuartileInc());
			AddFunction(result, new FunctionRankDotAvg());
			AddFunction(result, new FunctionRankDotEq());
			AddFunction(result, new FunctionRsq());
			AddFunction(result, new FunctionSkew());
			AddFunction(result, new FunctionSkewP());
			AddFunction(result, new FunctionSlope());
			AddFunction(result, new FunctionSmall());
			AddFunction(result, new FunctionStandardize());
			AddFunction(result, new FunctionStdevDotP());
			AddFunction(result, new FunctionStdevDotS());
			AddFunction(result, new FunctionStdeva());
			AddFunction(result, new FunctionStdevpa());
			AddFunction(result, new FunctionSteyx());
			AddFunction(result, new FunctionTDotDist());
			AddFunction(result, new FunctionTDotDistDot2t());
			AddFunction(result, new FunctionTDotDistDotRt());
			AddFunction(result, new FunctionTDotInv());
			AddFunction(result, new FunctionTDotInvDot2t());
			AddFunction(result, new FunctionTrend());
			AddFunction(result, new FunctionTrimMean());
			AddFunction(result, new FunctionTDotTest());
			AddFunction(result, new FunctionVarDotP());
			AddFunction(result, new FunctionVarDotS());
			AddFunction(result, new FunctionVara());
			AddFunction(result, new FunctionVarpa());
			AddFunction(result, new FunctionWeibullDotDist());
			AddFunction(result, new FunctionZDotTest());
		}
		#endregion
	}
}
