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
	#region CouponFrequency
	public enum CouponFrequency {
		Annual = 1,
		Semiannual = 2,
		Quarterly = 4
	}
	#endregion
	#region FunctionCouponBase
	public abstract class FunctionCouponBase : FunctionSerialNumberBase {
		#region Static Members
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference, FunctionParameterOption.NonRequired));
			return collection;
		}
		#endregion
		#region Properties
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		#endregion
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue settlementValue = DateToSerialNumber(arguments[0], context);
			if (settlementValue.IsError)
				return settlementValue;
			int settlementSerialNumber = (int)settlementValue.NumericValue;
			VariantValue maturityValue = DateToSerialNumber(arguments[1], context);
			if (maturityValue.IsError)
				return maturityValue;
			int maturitySerialNumber = (int)maturityValue.NumericValue;
			if (maturitySerialNumber <= settlementSerialNumber || !CheckMaxValue(maturitySerialNumber, context.DateSystem))
				return VariantValue.ErrorNumber;
			VariantValue frequencyValue = GetFrequency(arguments[2], context);
			if (frequencyValue.IsError)
				return frequencyValue;
			int basisId = 0;
			if (arguments.Count > 3) {
				VariantValue basis = GetBasis(arguments[3], context);
				if (basis.IsError)
					return basis;
				basisId = (int)basis.NumericValue;
			}
			return GetResult(settlementSerialNumber, maturitySerialNumber, (CouponFrequency)frequencyValue.NumericValue, basisId, context);
		}
		VariantValue GetFrequency(VariantValue frequency, WorkbookDataContext context) {
			frequency = context.ToNumericWithoutCrossing(frequency);
			if (frequency.IsError)
				return frequency;
			int frequencyValue = (int)frequency.NumericValue;
			if (frequencyValue != 1 && frequencyValue != 2 && frequencyValue != 4)
				return VariantValue.ErrorNumber;
			return frequencyValue;
		}
		bool CheckMaxValue(int maturitySerialNumber, DateSystem dateSystem) {
			if (dateSystem == DateSystem.Date1900)
				return true;
			return maturitySerialNumber <= DayCountBasisBase.MaxDateTimeSerialNumber1904;
		}
		protected abstract VariantValue GetResult(int settlement, int maturity, CouponFrequency frequency, int basisId, WorkbookDataContext context);
	}
	#endregion
	#region CouponCalculationHelper
	struct CouponCalculationHelper {
		#region Static Members
		public static double CalculateCoupPCD(int settlement, int maturity, CouponFrequency frequency, WorkbookDataContext context) {
			CouponCalculationHelper helper = new CouponCalculationHelper();
			helper.Initialize(settlement, maturity, frequency, context);
			return helper.CalculateCoupPCD();
		}
		public static double CalculateCoupNCD(int settlement, int maturity, CouponFrequency frequency, WorkbookDataContext context) {
			CouponCalculationHelper helper = new CouponCalculationHelper();
			helper.Initialize(settlement, maturity, frequency, context);
			return helper.CalculateCoupNCD();
		}
		public static double CalculateCoupDayBS(int settlement, int maturity, CouponFrequency frequency, int basisId, WorkbookDataContext context) {
			CouponCalculationHelper helper = new CouponCalculationHelper();
			helper.Initialize(settlement, maturity, frequency, context);
			return helper.CalculateCoupDayBS(basisId);
		}
		#endregion
		#region Fields
		int settlement;
		int maturity;
		int monthsInPeriod;
		WorkbookDataContext context;
		DateTime interestDate;
		DateTime interestDateWithPeriod;
		bool shiftApplied;
		bool settlementIsBigger;
		#endregion
		#region Properties
		int FourYearsShift { get { return 1461; } }
		int Year1900Feb29Serial { get { return 60; } }
		bool IsDate1900 { get { return context.DateSystem == DateSystem.Date1900; } }
		#endregion
		void Initialize(int settlement, int maturity, CouponFrequency frequency, WorkbookDataContext context) {
			this.settlement = settlement;
			this.maturity = maturity;
			this.monthsInPeriod = 12 / (int)frequency;
			this.context = context;
		}
		#region Calcualte CoupPCD&NCD
		int CalculateCoupPCD() {
			if (IsDate1900 && maturity == Year1900Feb29Serial)
				return 0;
			InitializeDates();
			CalculateCouponInterval();
			return settlementIsBigger ? GetCouponDateSerial(interestDate) : GetCouponDateSerial(interestDateWithPeriod);
		}
		int CalculateCoupNCD() {
			if (IsDate1900 && maturity == Year1900Feb29Serial)
				return maturity;
			InitializeDates();
			CalculateCouponInterval();
			return settlementIsBigger ? GetCouponDateSerial(interestDateWithPeriod) : GetCouponDateSerial(interestDate);
		}
		void InitializeDates() {
			if (IsDate1900 && settlement == Year1900Feb29Serial)
				settlement--;
			if (!IsDate1900 && settlement <= 365)
				ShiftSettlement();
		}
		int GetCouponDateSerial(DateTime resultDate) {
			int resultSerial = DateToSerial(resultDate);
			return shiftApplied ? resultSerial - FourYearsShift : resultSerial;
		}
		#endregion
		#region Calculate CoupDays
		delegate int CoupDaysGetSerial();
		int CalculateCoupDayBS(int basisId) {
			return CalculateCoupDaysCore(basisId, CalculateCoupPCD);
		}
		int CalculateCoupDaysCore(int basisId, CoupDaysGetSerial getSerial) {
			DayCountBasisBase basis = DayCountBasisFactory.GetBasis(basisId);
			int serial = getSerial();
			UndoShift();
			return basis.GetDays(Math.Min(settlement, serial), Math.Max(settlement, serial), context);
		}
		#endregion
		#region Calculate Coupon Interval
		void CalculateCouponInterval() {
			DateTime settlementDate = context.FromDateTimeSerial(settlement);
			DateTime maturityDate = context.FromDateTimeSerial(maturity);
			DateSystem dateSystem = context.DateSystem;
			interestDate = GetInterestDate(settlementDate, maturityDate, dateSystem);
			settlementIsBigger = settlementDate >= interestDate;
			int shiftValue = settlementIsBigger ? monthsInPeriod : -monthsInPeriod;
			interestDateWithPeriod = ModifyInterestDate(interestDate.AddMonths(shiftValue), maturityDate, dateSystem);
			if (settlementIsBigger) {
				while (settlementDate >= interestDateWithPeriod)
					ShiftDateByPeriod(shiftValue, maturityDate, dateSystem);
			}
			else {
				while (settlementDate < interestDateWithPeriod)
					ShiftDateByPeriod(shiftValue, maturityDate, dateSystem);
			}
		}
		void ShiftSettlement() {
			settlement += FourYearsShift;
			shiftApplied = true;
		}
		void UndoShift() {
			if (shiftApplied) {
				settlement -= FourYearsShift;
				shiftApplied = false;
			}
		}
		void ShiftDateByPeriod(int shiftValue, DateTime maturityDate, DateSystem dateSystem) {
			interestDate = interestDateWithPeriod;
			interestDateWithPeriod = ModifyInterestDate(interestDate.AddMonths(shiftValue), maturityDate, dateSystem);
		}
		#endregion
		#region GetInterestDate
		DateTime GetInterestDate(DateTime settlementDate, DateTime maturityDate, DateSystem dateSystem) {
			int interestDateYear = settlementDate.Year == 1899 ? 1900 : settlementDate.Year;
			DateTime interestDate = new DateTime(interestDateYear, maturityDate.Month, GetInterestDateDay(settlementDate, maturityDate));
			return ModifyInterestDate(interestDate, maturityDate, dateSystem);
		}
		int GetInterestDateDay(DateTime settlementDate, DateTime maturityDate) {
			bool differentYearTypes = DateTime.IsLeapYear(settlementDate.Year) != DateTime.IsLeapYear(maturityDate.Year);
			if (IsLastDayOfFebruary(maturityDate) && differentYearTypes)
				return DateTime.DaysInMonth(settlementDate.Year, maturityDate.Month);
			return maturityDate.Day;
		}
		DateTime ModifyInterestDate(DateTime interestDate, DateTime maturityDate, DateSystem dateSystem) {
			int interestDateDay = interestDate.Day;
			if (IsLastDayOfMonth(maturityDate))
				interestDateDay = DaysInMonth(interestDate);
			if (dateSystem == DateSystem.Date1904 && maturityDate.Day == DaysInMonth(maturityDate) - 1)
				interestDateDay = DaysInMonth(interestDate) - 1;
			return new DateTime(interestDate.Year, interestDate.Month, interestDateDay);
		}
		#endregion
		#region Helper Methods
		bool IsLastDayOfMonth(DateTime date) {
			return DateTime.DaysInMonth(date.Year, date.Month) == date.Day;
		}
		bool IsLastDayOfFebruary(DateTime date) {
			return IsLastDayOfMonth(date) && date.Month == 2;
		}
		int DaysInMonth(DateTime date) {
			return DateTime.DaysInMonth(date.Year, date.Month);
		}
		int DateToSerial(DateTime date) {
			return (int)context.FromDateTime(date).NumericValue;
		}
		#endregion
	}
	#endregion
}
