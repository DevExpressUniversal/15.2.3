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
		#region AddDateTimeFunctions
		public static void AddDateTimeFunctions(Dictionary<string, ISpreadsheetFunction> result) {
			AddFunction(result, new FunctionDate());
			AddFunction(result, new FunctionDateValue());
			AddFunction(result, new FunctionDay());
			AddFunction(result, new FunctionDays());
			AddFunction(result, new FunctionDays360());
			AddFunction(result, new FunctionEDate());
			AddFunction(result, new FunctionEOMonth());
			AddFunction(result, new FunctionHour());
			AddFunction(result, new FunctionISOWeekNum());
			AddFunction(result, new FunctionMinute());
			AddFunction(result, new FunctionMonth());
			AddFunction(result, new FunctionNetworkDays());
			AddFunction(result, new FunctionNetworkDaysIntl());
			AddFunction(result, new FunctionNow());
			AddFunction(result, new FunctionSecond());
			AddFunction(result, new FunctionTime());
			AddFunction(result, new FunctionTimeValue());
			AddFunction(result, new FunctionToday());
			AddFunction(result, new FunctionWeekDay());
			AddFunction(result, new FunctionWeekNum());
			AddFunction(result, new FunctionWorkDay());
			AddFunction(result, new FunctionWorkdayIntl());
			AddFunction(result, new FunctionYear());
			AddFunction(result, new FunctionYearFrac());
			AddFunction(result, new FunctionDateDif());
		}
		#endregion
	}
}
