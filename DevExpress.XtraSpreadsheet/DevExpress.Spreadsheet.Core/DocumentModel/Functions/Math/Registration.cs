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
		#region AddMathAndTrigonometryFunctions
		public static void AddMathAndTrigonometryFunctions(Dictionary<string, ISpreadsheetFunction> result) {
			AddFunction(result, new FunctionAbs());
			AddFunction(result, new FunctionACos());
			AddFunction(result, new FunctionACosH());
			AddFunction(result, new FunctionACot());
			AddFunction(result, new FunctionAcoth());
			AddFunction(result, new FunctionArabic());
			AddFunction(result, new FunctionAsin());
			AddFunction(result, new FunctionAsinh());
			AddFunction(result, new FunctionAtan());
			AddFunction(result, new FunctionAtan2());
			AddFunction(result, new FunctionAtanh()); 
			AddFunction(result, new FunctionBase());
			AddFunction(result, new FunctionCeiling());
			AddFunction(result, new FunctionCeilingMath());
			AddFunction(result, new FunctionCeilingPrecise());
			AddFunction(result, new FunctionCombin());
			AddFunction(result, new FunctionCombinA());
			AddFunction(result, new FunctionCos());
			AddFunction(result, new FunctionCosH());
			AddFunction(result, new FunctionCot());
			AddFunction(result, new FunctionCoth());
			AddFunction(result, new FunctionCsc());
			AddFunction(result, new FunctionCsch());
			AddFunction(result, new FunctionDecimal());
			AddFunction(result, new FunctionDegrees());
			AddFunction(result, new FunctionEven());
			AddFunction(result, new FunctionExp());
			AddFunction(result, new FunctionFact());
			AddFunction(result, new FunctionFactDouble());
			AddFunction(result, new FunctionFloor());
			AddFunction(result, new FunctionFloorMath());
			AddFunction(result, new FunctionFloorPrecise());
			AddFunction(result, new FunctionGcd());
			AddFunction(result, new FunctionInt());
			AddFunction(result, new FunctionIsoCeiling());
			AddFunction(result, new FunctionLcm());
			AddFunction(result, new FunctionLn());
			AddFunction(result, new FunctionLog());
			AddFunction(result, new FunctionLog10());
			AddFunction(result, new FunctionMDeterm());
			AddFunction(result, new FunctionMInverse());
			AddFunction(result, new FunctionMMult());
			AddFunction(result, new FunctionMod());
			AddFunction(result, new FunctionMRound());
			AddFunction(result, new FunctionMUnit());
			AddFunction(result, new FunctionMultinomial());
			AddFunction(result, new FunctionOdd());
			AddFunction(result, new FunctionPi());
			AddFunction(result, new FunctionPower());
			AddFunction(result, new FunctionProduct());
			AddFunction(result, new FunctionQuotient());
			AddFunction(result, new FunctionRadians());
			AddFunction(result, new FunctionRand());
			AddFunction(result, new FunctionRandBetween());
			AddFunction(result, new FunctionRoman());
			AddFunction(result, new FunctionRound());
			AddFunction(result, new FunctionRoundDown());
			AddFunction(result, new FunctionRoundUp());
			AddFunction(result, new FunctionSeriesSum());
			AddFunction(result, new FunctionSec());
			AddFunction(result, new FunctionSech());
			AddFunction(result, new FunctionSign());
			AddFunction(result, new FunctionSin());
			AddFunction(result, new FunctionSinH());
			AddFunction(result, new FunctionSqrt());
			AddFunction(result, new FunctionSqrtPi());
			AddFunction(result, new FunctionSubtotal());
			AddFunction(result, new FunctionSum());
			AddFunction(result, new FunctionSumIf());
			AddFunction(result, new FunctionSumIfs());
			AddFunction(result, new FunctionSumProduct());
			AddFunction(result, new FunctionSumSq());
			AddFunction(result, new FunctionSumx2my2());
			AddFunction(result, new FunctionSumX2PY2());
			AddFunction(result, new FunctionSumXMY2());
			AddFunction(result, new FunctionTan());
			AddFunction(result, new FunctionTanH());
			AddFunction(result, new FunctionTrunc());
		}
		#endregion
	}
}
