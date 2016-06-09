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
	#region FunctionDb
	public class FunctionDb : WorksheetFunctionBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "DB"; } }
		public override int Code { get { return 0x00F7; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue cost = arguments[0].ToNumeric(context);
			if (cost.IsError)
				return cost;
			VariantValue salvage = arguments[1].ToNumeric(context);
			if (salvage.IsError)
				return salvage;
			VariantValue life = arguments[2].ToNumeric(context);
			if (life.IsError)
				return life;
			VariantValue period = arguments[3].ToNumeric(context);
			if (period.IsError)
				return period;
			int intMonth = 12;
			if (arguments.Count > 4) {
				VariantValue month = arguments[4].ToNumeric(context);
				if (month.IsError)
					return month;
				intMonth = (int)month.NumericValue;
			}
			if (CheckValues(cost.NumericValue, salvage.NumericValue, life.NumericValue, period.NumericValue, intMonth))
				return VariantValue.ErrorNumber;
			return Math.Round(GetResult(cost.NumericValue, salvage.NumericValue, life.NumericValue, (int)period.NumericValue, intMonth), 2);
		}
		double GetResult(double cost, double salvage, double life, double period, int month) {
			if (cost == 0)
				return 0;
			double rate = Math.Round((1 - Math.Pow(salvage / cost, 1.0 / life)), 3);
			double depreciation = cost * rate * month / 12;
			double depreciationSum = depreciation;
			bool greater = false;
			if (life < 1 && month == 12)
				life = 1;
			if (period > life) {
				--period;
				greater = true;
			}
			for (int i = 1; i < period; ++i) {
				depreciation = (cost - depreciationSum) * rate;
				depreciationSum += depreciation;
			}
			return greater ? ((cost - depreciationSum) * rate * (12 - month)) / 12 : depreciation;
		}
		bool CheckValues(double cost, double salvage, double life, double period, int month) {
			if (cost < 0 || salvage < 0 || life <= 0 || period <= 0 || month < 1 || month > 12)
				return true;
			++life;
			if ((int)period == (int)life && month == 12 || (int)period > (int)life)
				return true;
			return false;
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.NonRequired));
			return collection;
		}
	}
	#endregion
}
