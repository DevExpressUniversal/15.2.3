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
	#region FunctionTBillBase
	public abstract class FunctionTBillBase : FunctionSerialNumberBase {
		#region Static Members
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference));
			return collection;
		}
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		#endregion
		#region Properties
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected const int DaysInYear = 365;
		protected const int DaysInLeapYear = 366;
		protected const int YearBasis = 360;
		#endregion
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue settlementDate = DateToSerialNumber(arguments[0], context);
			if (settlementDate.IsError)
				return settlementDate;
			VariantValue maturityDate = DateToSerialNumber(arguments[1], context);
			if (maturityDate.IsError)
				return maturityDate;
			VariantValue argument = context.ToNumericWithoutCrossing(arguments[2]);
			if (argument.IsError)
				return argument;
			return ProcessErrors((int)settlementDate.NumericValue, (int)maturityDate.NumericValue, argument.NumericValue, context);
		}
		VariantValue ProcessErrors(int settlementDateValue, int maturityDateValue, double argumentValue, WorkbookDataContext context) {
			double daysPassed = maturityDateValue - settlementDateValue;
			bool leapYearError = false;
			if (daysPassed == DaysInLeapYear && settlementDateValue > 60) {
				DateTime startDate = context.FromDateTimeSerial(settlementDateValue);
				DateTime endDate = context.FromDateTimeSerial(maturityDateValue);
				leapYearError = !WithFeb29(startDate, endDate);
			}
			if (daysPassed < 1 || daysPassed > DaysInLeapYear || leapYearError || argumentValue <= 0)
				return VariantValue.ErrorNumber;
			return GetResult(daysPassed, argumentValue);
		}
		bool WithFeb29(DateTime startDate, DateTime endDate) {
			if (DateTime.IsLeapYear(startDate.Year))
				return startDate.Month <= 2;
			if (DateTime.IsLeapYear(endDate.Year))
				return endDate.Month > 2;
			return false;
		}
		protected abstract VariantValue GetResult(double daysPassed, double argument);
	}
	#endregion
	#region FunctionTBillEq
	public class FunctionTBillEq : FunctionTBillBase {
		#region Properties
		public override string Name { get { return "TBILLEQ"; } }
		public override int Code { get { return 0x01B6; } }
		#endregion
		protected override VariantValue GetResult(double daysPassed, double discountRate) {
			double discountInPeriod = daysPassed * discountRate;
			bool firstHalfOfYear = daysPassed <= 182;
			bool discountEqualsBasis = discountInPeriod == YearBasis;
			if (discountInPeriod > YearBasis || (discountEqualsBasis && !firstHalfOfYear))
				return VariantValue.ErrorNumber;
			double fracPart = discountEqualsBasis ? 0 : discountRate / (YearBasis - discountInPeriod);
			if (firstHalfOfYear)
				return fracPart * DaysInYear;
			double yearPart = daysPassed == DaysInLeapYear ? 1 : daysPassed / DaysInYear;
			return (-2 * yearPart + 2 * Math.Sqrt(yearPart * yearPart + (2 * yearPart - 1) * daysPassed * fracPart)) / (2 * yearPart - 1);
		}
	}
	#endregion
}
