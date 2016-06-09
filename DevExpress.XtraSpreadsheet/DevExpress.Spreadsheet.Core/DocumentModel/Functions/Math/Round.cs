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
	#region FunctionRoundBase (abstract class)
	public abstract class FunctionRoundBase : WorksheetFunctionBase {
		static readonly double[] orderTable = CreateOrderTable();
		static double[] CreateOrderTable() {
			double[] result = new double[2];
			double order = 1;
			int count = result.Length;
			for (int i = 0; i < count; i++) {
				result[i] = order;
				order *= 10;
			}
			return result;
		}
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue value = arguments[0].ToNumeric(context);
			if (value.IsError)
				return value;
			VariantValue decimalsValue = arguments[1].ToNumeric(context);
			if (decimalsValue.IsError)
				return decimalsValue;
			int decimals = (int)Truncate(decimalsValue.NumericValue);
			if (decimalsValue != decimalsValue.NumericValue)
				return VariantValue.ErrorInvalidValueInFunction;
			return Round(value.NumericValue, decimals);
		}
		protected virtual double RoundCore(double value) {
#if !SL
			return Math.Round(value, MidpointRounding.AwayFromZero);
#else
			if (value < 0)
				return Truncate(value - 0.5);
			else
				return Truncate(value + 0.5);
#endif
		}
		protected virtual double Round(double value, int digitsCount) {
			if (digitsCount == 0)
				return RoundCore(value);
			if (digitsCount < 0)
				return RoundToOrder(value, -digitsCount);
			else
				return RoundToDecimal(value, digitsCount);
		}
		protected virtual double RoundToOrder(double value, int digitsCount) {
			double order = GetOrder(digitsCount);
			return order * RoundCore(value / order);
		}
		protected virtual double RoundToDecimal(double value, int digitsCount) {
			double order = GetOrder(digitsCount);
			return RoundCore(value * order) / order;
		}
		double GetOrder(int digitsCount) {
			if (digitsCount < orderTable.Length)
				return orderTable[digitsCount];
			int count = orderTable.Length - 1;
			double order = orderTable[count];
			for (int i = count; i < digitsCount; i++)
				order *= 10;
			return order;
		}
	}
	#endregion
	#region FunctionRound
	public class FunctionRound : FunctionRoundBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "ROUND"; } }
		public override int Code { get { return 0x001B; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			return collection;
		}
	}
	#endregion
}
