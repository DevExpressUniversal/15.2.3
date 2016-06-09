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

using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionBase
	public class FunctionBase : WorksheetFunctionBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "BASE"; } }
		public override int Code { get { return 0x4018; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue number1 = arguments[0].ToNumeric(context);
			VariantValue number2 = arguments[1].ToNumeric(context);
			if (number1.IsError)
				return number1;
			if (number2.IsError)
				return number2;
			long value = (long)number1.NumericValue;
			int baseTo = (int)number2.NumericValue;
			int minLength = 1;
			if (arguments.Count > 2) {
				VariantValue number3 = arguments[2].ToNumeric(context);
				if (number3.IsError)
					return number3;
				minLength = (int)number3.NumericValue;
			}
			if (value < 0 || value >= ((long)1 << 53) || baseTo < 2 || baseTo > 36 || minLength < 0 || minLength > 255)
				return VariantValue.ErrorNumber;
			return GetResult(value, baseTo, minLength);
		}
		string GetResult(long number, int baseTo, int minLength) {
			string result = string.Empty;
			int currentDigit;
			while (number != 0) {
				currentDigit = (int)(number % baseTo);
				result = (currentDigit > 9 ? (char)(currentDigit + 0x37) : (char)(currentDigit + 0x30)) + result; 
				number /= baseTo;
			}
			if (minLength <= result.Length)
				return result;
			return new string('0', minLength - result.Length) + result;
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
