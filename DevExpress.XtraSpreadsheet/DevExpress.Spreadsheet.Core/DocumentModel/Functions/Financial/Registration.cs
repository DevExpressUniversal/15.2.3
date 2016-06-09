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
		#region AddFinancialFunctions
		public static void AddFinancialFunctions(Dictionary<string, ISpreadsheetFunction> result) {
			AddFunction(result, new FunctionAccrintm());
			AddFunction(result, new FunctionCoupDayBs());
			AddFunction(result, new FunctionCoupNcd());
			AddFunction(result, new FunctionCoupNum());
			AddFunction(result, new FunctionCoupPcd());
			AddFunction(result, new FunctionCumIpmt());
			AddFunction(result, new FunctionCumPrinc());
			AddFunction(result, new FunctionDb());
			AddFunction(result, new FunctionDdb());
			AddFunction(result, new FunctionDisc());
			AddFunction(result, new FunctionDollarDe());
			AddFunction(result, new FunctionDollarFr());
			AddFunction(result, new FunctionEffect());
			AddFunction(result, new FunctionFv());
			AddFunction(result, new FunctionFvSchedule());
			AddFunction(result, new FunctionIntrate());
			AddFunction(result, new FunctionIpmt());
			AddFunction(result, new FunctionIrr());
			AddFunction(result, new FunctionIsPmt());
			AddFunction(result, new FunctionMirr());
			AddFunction(result, new FunctionNominal());
			AddFunction(result, new FunctionNPer());
			AddFunction(result, new FunctionNpv());
			AddFunction(result, new FunctionPDuration());
			AddFunction(result, new FunctionPmt());
			AddFunction(result, new FunctionPpmt());
			AddFunction(result, new FunctionPriceDisc());
			AddFunction(result, new FunctionPv());
			AddFunction(result, new FunctionRate());
			AddFunction(result, new FunctionReceived());
			AddFunction(result, new FunctionSln());
			AddFunction(result, new FunctionSyd());
			AddFunction(result, new FunctionTBillEq());
			AddFunction(result, new FunctionTBillPrice());
			AddFunction(result, new FunctionTBillYield());
			AddFunction(result, new FunctionVdb());
			AddFunction(result, new FunctionXIrr());
			AddFunction(result, new FunctionRRI());
			AddFunction(result, new FunctionXnpv());
			AddFunction(result, new FunctionYieldDisc());
		}
		#endregion
	}
}
