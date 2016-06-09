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
	#region FunctionDays360
	public class FunctionDays360 : FunctionSerialNumberBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "DAYS360"; } }
		public override int Code { get { return 0x00DC; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue startDate = ToSerialNumber(arguments[0], context);
			if (startDate.IsError)
				return startDate;
			VariantValue endDate = ToSerialNumber(arguments[1], context);
			if (endDate.IsError)
				return endDate;
			bool method = false;
			if (arguments.Count > 2) {
				VariantValue value = arguments[2].ToBoolean(context);
				if (value.IsError)
					return value;
				method = value.BooleanValue;
			}
			return GetNumericResult(startDate, endDate, method, context);
		}
		protected override VariantValue ToSerialNumberCore(VariantValue value, WorkbookDataContext context) {
			return value.ToNumeric(context);
		}
		int GetNumericResult(VariantValue startDate, VariantValue endDate, bool method, WorkbookDataContext context) {
			return 360 * (GetYear(endDate, context) - GetYear(startDate, context)) + 
					30 * (GetMonth(endDate, context) - GetMonth(startDate, context)) +
					GetDifferenceBetweenEndStartDay(startDate, endDate, method, context);
		}
		int GetDay(VariantValue date, WorkbookDataContext context) {
			if (context.DateSystem == DateSystem.Date1900 && date.NumericValue == 0)
				return 0;
			if (context.DateSystem == DateSystem.Date1900 && date.NumericValue == 60)
				return 29;
			return date.ToDateTime(context).Day;
		}
		int GetMonth(VariantValue date, WorkbookDataContext context) {
			if (context.DateSystem == DateSystem.Date1900 && date.NumericValue == 0)
				return 1;
			if (context.DateSystem == DateSystem.Date1900 && date.NumericValue == 60)
				return 2;
			return date.ToDateTime(context).Month;
		}
		int GetYear(VariantValue date, WorkbookDataContext context) {
			if (context.DateSystem == DateSystem.Date1900 && date.NumericValue == 0)
				return 1900;
			return date.ToDateTime(context).Year;
		}
		int GetDifferenceBetweenEndStartDay(VariantValue startDate, VariantValue endDate, bool method, WorkbookDataContext context) {
			int startDay = GetDay(startDate, context);
			int endDay = GetDay(endDate, context);
			if (method) {
				if (startDay == 31)
					startDay = 30;
				if (endDay == 31)
					endDay = 30;
			}
			else {
				int startMonth = GetMonth(startDate, context);
				if (startDay == 31 || (startMonth == 2 && startDay == DateTime.DaysInMonth(GetYear(startDate, context), startMonth)))
					startDay = 30;
				if (startDay == 30 && endDay == 31)
					endDay = 30;
				endDay += GetAspect(startDate, endDate, context);
			}
			return endDay - startDay;
		}
		int GetAspect(VariantValue startDate, VariantValue endDate, WorkbookDataContext context) {
			if (endDate.NumericValue == 0 || endDate.ToDateTime(context).Day != 31) {
				if (context.DateSystem == DateSystem.Date1900 && startDate.NumericValue == NumberDay29Feb1900)
					return -1;
				if (context.DateSystem == DateSystem.Date1900 && startDate.NumericValue == NumberDay29Feb1900 - 1)
					return 2;
			}
			if (endDate.ToDateTime(context).Day == 31) {
				if (context.DateSystem == DateSystem.Date1900 && startDate.NumericValue == NumberDay29Feb1900)
					return -2;
				if (context.DateSystem == DateSystem.Date1900 && startDate.NumericValue == NumberDay29Feb1900 - 1)
					return 3;
			}
			return 0;
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.NonRequired));
			return collection;
		}
	}
	#endregion
}
