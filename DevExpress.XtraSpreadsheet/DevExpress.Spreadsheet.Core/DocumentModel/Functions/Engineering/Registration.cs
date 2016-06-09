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
		#region AddEngineeringFunctions
		public static void AddEngineeringFunctions(Dictionary<string, ISpreadsheetFunction> result) {
			AddFunction(result, new FunctionBesselI());
			AddFunction(result, new FunctionBesselJ());
			AddFunction(result, new FunctionBesselK());
			AddFunction(result, new FunctionBesselY());
			AddFunction(result, new FunctionBin2Dec());
			AddFunction(result, new FunctionBin2Hex());
			AddFunction(result, new FunctionBin2Oct());
			AddFunction(result, new FunctionBitAnd());
			AddFunction(result, new FunctionBitLShift());
			AddFunction(result, new FunctionBitOr());
			AddFunction(result, new FunctionBitRShift());
			AddFunction(result, new FunctionBitXor());
			AddFunction(result, new FunctionComplex());
			AddFunction(result, new FunctionConvert());
			AddFunction(result, new FunctionDec2Bin());
			AddFunction(result, new FunctionDec2Hex());
			AddFunction(result, new FunctionDec2Oct());
			AddFunction(result, new FunctionDelta());
			AddFunction(result, new FunctionErf());
			AddFunction(result, new FunctionErfPrecise());
			AddFunction(result, new FunctionErfc());
			AddFunction(result, new FunctionErfcPrecise());
			AddFunction(result, new FunctionGestep());
			AddFunction(result, new FunctionHex2Bin());
			AddFunction(result, new FunctionHex2Dec());
			AddFunction(result, new FunctionHex2Oct());
			AddFunction(result, new FunctionImAbs());
			AddFunction(result, new FunctionImaginary());
			AddFunction(result, new FunctionImArgument());
			AddFunction(result, new FunctionImConjugate());
			AddFunction(result, new FunctionImCos());
			AddFunction(result, new FunctionImCosH());
			AddFunction(result, new FunctionImCot());
			AddFunction(result, new FunctionImCsc());
			AddFunction(result, new FunctionImCsch());
			AddFunction(result, new FunctionImDiv());
			AddFunction(result, new FunctionImExp());
			AddFunction(result, new FunctionImLn());
			AddFunction(result, new FunctionImLog10());
			AddFunction(result, new FunctionImLog2());
			AddFunction(result, new FunctionImPower());
			AddFunction(result, new FunctionImProduct());
			AddFunction(result, new FunctionImReal());
			AddFunction(result, new FunctionImSec());
			AddFunction(result, new FunctionImSecH());
			AddFunction(result, new FunctionImSin());
			AddFunction(result, new FunctionImSinH());
			AddFunction(result, new FunctionImSqrt());
			AddFunction(result, new FunctionImSub());
			AddFunction(result, new FunctionImSum());
			AddFunction(result, new FunctionImTan());
			AddFunction(result, new FunctionOct2Bin());
			AddFunction(result, new FunctionOct2Dec());
			AddFunction(result, new FunctionOct2Hex());
		}
		#endregion
	}
}
