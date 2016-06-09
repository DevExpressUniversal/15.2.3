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
	#region FunctionFixed
	public class FunctionFixed : WorksheetFunctionBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "FIXED"; } }
		public override int Code { get { return 0x000E; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			int operandCount = arguments.Count;
			VariantValue number = arguments[0].ToNumeric(context);
			if (number.IsError)
				return number;
			VariantValue decimals = 2;
			if(operandCount > 1) {
				VariantValue value = arguments[1];
				if (value.IsError)
					return value;
				if (!value.IsEmpty) {
					decimals = value.ToNumeric(context);
					if (decimals.IsError)
						return decimals;
				}
			}
			VariantValue noCommas = false;
			if(operandCount == 3) {
				noCommas = arguments[2].ToBoolean(context);
				if (noCommas.IsError)
					return noCommas;
			}
			return GetStringResult(number.NumericValue, (int)decimals.NumericValue, noCommas.BooleanValue, context);
		}
		VariantValue GetStringResult(double number, int decimals, bool noCommas, WorkbookDataContext context) {
			string specifier = noCommas ? "F" : "N";
			if (decimals >= 0)
				specifier += decimals.ToString();
			else {
				number = Math.Sign(number) * Math.Floor(Math.Abs(number) * Math.Pow(10, decimals)) * Math.Pow(10, -decimals);
				specifier += "0";
			}
			return number.ToString(specifier, context.Culture);
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
