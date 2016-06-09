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
	#region FunctionCeilingMath
	public class FunctionCeilingMath : WorksheetFunctionBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "CEILING.MATH"; } }
		public override int Code { get { return 0x4019; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue number = arguments[0].ToNumeric(context);
			if (number.IsError)
				return number;
			double value = number.NumericValue, doubleSignificance = 1, doubleMode = 0;
			if (arguments.Count > 1 && !arguments[1].IsEmpty) {
				VariantValue significance = arguments[1].ToNumeric(context);
				if (significance.IsError)
					return significance;
				doubleSignificance = significance.NumericValue;
			}
			if (arguments.Count > 2) {
				VariantValue mode = arguments[2].ToNumeric(context);
				if (mode.IsError)
					return mode;
				doubleMode = mode.NumericValue;
			}
			return GetNumericResult(value, doubleSignificance, doubleMode);
		}
		protected double GetNumericResult(double number, double significance, double mode) {
			if (significance == 0 || number == 0)
				return 0;
			significance = Math.Abs(significance);
			double result = Math.Abs(number) / significance;
			if (result - Math.Floor(result) == 0)
				result = number;
			else {
				result = Math.Floor(result);
				if (number < 0) {
					result *= -significance;
					if (mode != 0)
						result -= significance;
				}
				else
					result = (result + 1) * significance;
			}
			if (result >= double.MaxValue)
				return double.MaxValue;
			if (result <= double.MinValue)
				return double.MinValue;
			return result;
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.NonRequired));
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.NonRequired));
			return collection;
		}
	}
	#endregion
}
