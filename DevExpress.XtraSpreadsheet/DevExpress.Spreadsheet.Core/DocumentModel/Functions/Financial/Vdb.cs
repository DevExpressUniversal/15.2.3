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
	#region FunctionVdb
	public class FunctionVdb : WorksheetFunctionBase {
		#region Static Members
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.NonRequired));
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.NonRequired));
			return collection;
		}
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		#endregion
		#region Properties
		public override string Name { get { return "VDB"; } }
		public override int Code { get { return 0xDE; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		#endregion
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue value = arguments[0].ToNumeric(context);
			if (value.IsError)
				return value;
			double cost = value.NumericValue;
			value = arguments[1].ToNumeric(context);
			if (value.IsError)
				return value;
			double salvage = value.NumericValue;
			value = arguments[2].ToNumeric(context);
			if (value.IsError)
				return value;
			double life = value.NumericValue;
			value = arguments[3].ToNumeric(context);
			if (value.IsError)
				return value;
			double startPeriod = value.NumericValue;
			value = arguments[4].ToNumeric(context);
			if (value.IsError)
				return value;
			double endPeriod = value.NumericValue;
			double factor = 2.0;
			if (arguments.Count > 5) {
				value = arguments[5].ToNumeric(context);
				if (value.IsError)
					return value;
				factor = value.NumericValue;
			}
			bool slnSwitch = false;
			if (arguments.Count > 6) {
				value = arguments[6].ToBoolean(context);
				if (value.IsError)
					return value;
				slnSwitch = value.BooleanValue;
			}
			if (CheckErrorNumber(cost, salvage, life, startPeriod, endPeriod, factor))
				return VariantValue.ErrorNumber;
			if (CheckDivisionByZeroError(cost, salvage, life, startPeriod, endPeriod, factor, slnSwitch))
				return VariantValue.ErrorDivisionByZero;
			return DepreciationCalculator.CalculateVDB(cost, salvage, life, startPeriod, endPeriod, factor, slnSwitch);
		}
		bool CheckErrorNumber(double cost, double salvage, double life, double startPeriod, double endPeriod, double factor) {
			return cost < 0 || salvage < 0 || life < 0 || startPeriod < 0 || startPeriod > life || startPeriod > endPeriod || endPeriod > life || factor < 0;
		}
		bool CheckDivisionByZeroError(double cost, double salvage, double life, double startPeriod, double endPeriod, double factor, bool slnSwitch) {
			if (life == 0)
				return true;
			bool startEqualsLife = startPeriod == life;
			bool zeroSalvage = salvage == 0;
			bool case1 = zeroSalvage && salvage == cost;
			bool case2 = factor == 0 && salvage >= cost;
			bool case3 = zeroSalvage && factor >= life;
			return !slnSwitch && startEqualsLife && (case1 || case2 || case3);
		}
	}
	#endregion
	#region DepreciationCalculator
	struct DepreciationCalculator {
		#region Static Members
		public static double CalculateSLN(double cost, double salvage, double life) {
			DepreciationCalculator calculator = new DepreciationCalculator();
			calculator.InitializeCore(cost, salvage, life, 0);
			return calculator.CalculateSLN();
		}
		public static double CalculateSYD(double cost, double salvage, double life, double period) {
			DepreciationCalculator calculator = new DepreciationCalculator();
			calculator.InitializeCore(cost, salvage, life, 0);
			return calculator.CalculateSYD(period);
		}
		public static double CalculateDDB(double cost, double salvage, double life, double period, double factor) {
			DepreciationCalculator calculator = new DepreciationCalculator();
			calculator.InitializeCore(cost, salvage, life, factor);
			return calculator.CalculateDDB(period);
		}
		public static double CalculateVDB(double cost, double salvage, double life, double startPeriod, double endPeriod, double factor, bool slnSwitch) {
			DepreciationCalculator calculator = new DepreciationCalculator();
			calculator.Initialize(cost, salvage, life, startPeriod, endPeriod, factor);
			return calculator.CalculateVDB(slnSwitch);
		}
		#endregion
		#region Fields
		double cost;
		double salvage;
		double life;
		double startPeriod;
		double endPeriod;
		double factor;
		double rate;
		#endregion
		double CostSalvageDifference { get { return cost - salvage; } }
		void Initialize(double cost, double salvage, double life, double startPeriod, double endPeriod, double factor) {
			InitializeCore(cost, salvage, life, factor);
			this.startPeriod = startPeriod;
			this.endPeriod = endPeriod;
		}
		void InitializeCore(double cost, double salvage, double life, double factor) {
			this.cost = cost;
			this.salvage = salvage;
			this.life = life;
			this.factor = factor;
			this.rate = factor >= life ? 1.0 : factor / life;
		}
		#region Calculation Methods
		double CalculateSLN() {
			return CostSalvageDifference / life;
		}
		double CalculateSYD(double period) {
			return 2.0 * CostSalvageDifference * (life - period + 1.0) / (life * (life + 1.0));
		}
		double CalculateDDB(double period) {
			return CalculateDDB(period - 1, period);
		}
		double CalculateVDB(bool slnSwitch) {
			return slnSwitch ? CalculateDDBFractionalSum() : CalculateVDBFractionalSum();
		}
		double CalculateVDBFractionalSum() {
			if (CostSalvageDifference < 0)
				return CalculateNegativeVDB();
			double result = 0;
			double currentDepr = 0;
			double startFrac = startPeriod - Math.Floor(startPeriod);
			if (startFrac != 0) {
				currentDepr = CalculateVDB(startFrac);
				if (startFrac > startPeriod)
					result += currentDepr;
			}
			while (startFrac <= endPeriod - 1) {
				currentDepr = CalculateVDB(1);
				if (currentDepr == 0)
					return result;
				if (startFrac >= startPeriod)
					result += currentDepr;
				startFrac++;
			}
			double endFrac = endPeriod - startFrac;
			if (endFrac != 0)
				result += CalculateVDBCore(endFrac);
			return result;
		}
		double CalculateVDB(double fracValue) {
			double deprication = CalculateVDBCore(fracValue);
			cost -= deprication;
			life -= fracValue;
			return deprication;
		}
		double CalculateVDBCore(double fracValue) {
			double costLeft = CostSalvageDifference * fracValue;
			double deprDB = cost * rate * fracValue;
			double deprSL = CalculateSLN() * fracValue;
			if (deprDB > deprSL)
				return deprDB > costLeft ? costLeft : deprDB;
			return deprSL;
		}
		double CalculateNegativeVDB() {
			if (startPeriod >= 1)
				return 0;
			double totalPeriod = endPeriod - startPeriod;
			return totalPeriod >= 1 ? CostSalvageDifference : CostSalvageDifference * totalPeriod;
		}
		double CalculateDDBFractionalSum() {
			double result = 0;
			double intStartPeriod = Math.Floor(startPeriod);
			double intEndPeriod = Math.Floor(endPeriod);
			if (intStartPeriod == intEndPeriod)
				return CalculateDDB(intStartPeriod + 1) * (endPeriod - startPeriod);
			if (startPeriod != intStartPeriod) {
				result += CalculateDDB(intStartPeriod + 1) * (1 - startPeriod + intStartPeriod);
				intStartPeriod++;
			}
			if (intStartPeriod != intEndPeriod)
				result += CalculateDDB(intStartPeriod, intEndPeriod);
			if (endPeriod != intEndPeriod)
				result += CalculateDDB(intEndPeriod + 1) * (endPeriod - intEndPeriod);
			return result;
		}
		double CalculateDDB(double startPeriod, double endPeriod) {
			double depreciation;
			if (endPeriod > 1) {
				double oldValue = GetDDBSumCore(startPeriod);
				double newValue = GetDDBSumCore(endPeriod);
				depreciation = newValue < salvage ? oldValue - salvage : oldValue - newValue;
			}
			else
				depreciation = Math.Min(cost * rate, CostSalvageDifference);
			return depreciation < 0 ? 0 : depreciation;
		}
		double GetDDBSumCore(double period) {
			return cost * Math.Pow(1.0 - rate, period);
		}
		#endregion
	}
	#endregion
}
