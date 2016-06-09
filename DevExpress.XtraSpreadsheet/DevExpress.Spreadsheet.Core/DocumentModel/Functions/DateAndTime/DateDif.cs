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
	#region FunctionDateDif
	public class FunctionDateDif : FunctionDateBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "DATEDIF"; } }
		public override int Code { get { return 0x015F; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue startValue = arguments[0].ToNumeric(context);
			if (startValue.IsError)
				return startValue;
			VariantValue endValue = arguments[1].ToNumeric(context);
			if (endValue.IsError)
				return endValue;
			if (IncorrectDate(startValue.NumericValue, endValue.NumericValue, context.DateSystem))
				return VariantValue.ErrorNumber;
			VariantValue unitValue = arguments[2];
			if (unitValue.IsError)
				return unitValue;
			unitValue = unitValue.ToText(context);
			return GetDifference(startValue.NumericValue, endValue.NumericValue, unitValue, context);
		}
		VariantValue GetDifference(double startValue, double endValue, VariantValue unitValue, WorkbookDataContext context) {
			DateTime startDate = context.FromDateTimeSerial(startValue);
			DateTime endDate = context.FromDateTimeSerial(endValue);
			switch (unitValue.GetTextValue(context.StringTable).ToUpperInvariant()) {
				case "Y":
					return GetDifferenceYear(startDate, endDate);
				case "M":
					return GetDifferenceMonth(startDate, endDate);
				case "D":
					return endValue - startValue;
				case "MD":
					return GetDifferenceDay_MonthAndYearIgnore(startDate, endDate);
				case "YM":
					return GetDifferenceMonth_DayAndYearIgnore(startDate, endDate);
				case "YD":
					return GetDifferenceDay_YearIgnore(startValue, endValue, startDate, endDate);
				default:
					return VariantValue.ErrorNumber;
			}
		}
		VariantValue GetDifferenceYear(DateTime startDate, DateTime endDate) {
			double result = endDate.Year - startDate.Year;
			if (result >= 1)
				if ((endDate.Month < startDate.Month) || ((startDate.Month == endDate.Month) && (endDate.Day < startDate.Day)))
					result--;
			return result;
		}
		VariantValue GetDifferenceMonth(DateTime startDate, DateTime endDate) {
			return GetDifferenceYear(startDate, endDate).NumericValue * 12 + GetDifferenceMonth_DayAndYearIgnore(startDate, endDate).NumericValue;
		}
		VariantValue GetDifferenceDay_YearIgnore(double startValue, double endValue, DateTime startDate, DateTime endDate) {
			if (GetDifferenceYear(startDate, endDate).NumericValue < 1) 
				return endValue - startValue;
			else {
				if (startDate.Month == endDate.Month && startDate.Day == endDate.Day)
					return 0;
				double result = endDate.DayOfYear - startDate.DayOfYear;
				return (result >= 0) ? result : result + 365; 
			}
		}
		VariantValue GetDifferenceMonth_DayAndYearIgnore(DateTime startDate, DateTime endDate) {
			double result = endDate.Month - startDate.Month;
			if (endDate.Day < startDate.Day) 
				result--;
			if (result < 0)
				result = 12 + result;
			return result;
		}
		VariantValue GetDifferenceDay_MonthAndYearIgnore(DateTime startDate, DateTime endDate) {
			double result = endDate.Day - startDate.Day;
			if (result < 0) {
				DateTime monthAgo = endDate.AddMonths(-1);
				result += DateTime.DaysInMonth(monthAgo.Year, monthAgo.Month);
			}
			return result;
		}
		bool IncorrectDate(double startData, double endData, DateSystem dateSystem) {
			return (startData > endData) ||
				   WorkbookDataContext.IsErrorDateTimeSerial(startData, dateSystem) ||
				   WorkbookDataContext.IsErrorDateTimeSerial(endData, dateSystem);
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
