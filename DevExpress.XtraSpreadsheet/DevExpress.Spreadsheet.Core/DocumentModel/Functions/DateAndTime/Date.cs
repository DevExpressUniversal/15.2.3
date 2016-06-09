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
using DevExpress.XtraSpreadsheet.Utils;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Model {	
	#region FunctionDate
	public class FunctionDate : FunctionDateBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "DATE"; } }
		public override int Code { get { return 0x0041; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue yearValue = arguments[0].ToNumeric(context);
			if (yearValue.IsError)
				return yearValue;
			if ((yearValue.NumericValue < 0) || (yearValue.NumericValue > 9999)) 
				return VariantValue.ErrorNumber;
			VariantValue monthValue = arguments[1].ToNumeric(context);
			if (monthValue.IsError)
				return monthValue;
			VariantValue dayValue = arguments[2].ToNumeric(context);
			if (dayValue.IsError)
				return dayValue;
			DateTime date = (context.DateSystem == DateSystem.Date1900 ? SystemData : SystemData1904);
			date = GetYear(yearValue, date);
			date = GetMonth(monthValue, date);
			date = GetDay(dayValue, date);
			return context.ToDateTimeSerial(date);
		}
		DateTime GetYear(VariantValue yearValue, DateTime date) {
			double year = yearValue.NumericValue;
			if (year < 1900)
				return SystemData.AddYears((int)year);
			else
				return new DateTime((int)year, 1, 1);
		}
		DateTime GetMonth(VariantValue monthValue, DateTime date) {
			double month = monthValue.NumericValue;
			if ((month < 1) || (month > 12))
				return date.AddMonths((int)month - 1);
			else
				return new DateTime(date.Year, (int)month, 1);
		}
		DateTime GetDay(VariantValue dayValue, DateTime date) {
			double day = dayValue.NumericValue;
			if ((day < 1) || (day > DateTime.DaysInMonth(date.Year, date.Month)))
				return date.AddDays((int)day - 1);
			else
				return new DateTime(date.Year, date.Month, (int)day).Date;
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			return collection;
		}
	}
	#endregion
}
