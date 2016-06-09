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
	#region FunctionArabic
	public class FunctionArabic : WorksheetFunctionSingleArgumentBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		static readonly Dictionary<char, int> romanToArabic = CreateRomanToArabic();
		static Dictionary<char, int> CreateRomanToArabic(){
			Dictionary<char, int> result = new Dictionary<char, int>();
			result.Add('I', 1);
			result.Add('V', 5);
			result.Add('X', 10);
			result.Add('L', 50);
			result.Add('C', 100);
			result.Add('D', 500);
			result.Add('M', 1000);
			return result;
		}
		static Dictionary<char, int> RomanToArabic { get { return romanToArabic; } }
		public override string Name { get { return "ARABIC"; } }
		public override int Code { get { return 0x4017; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override VariantValue EvaluateArgument(VariantValue argument, WorkbookDataContext context) {
			VariantValue value = argument.ToText(context);
			if (value.IsError)
				return value;
			return GetNumericResult(value.InlineTextValue);
		}
		VariantValue GetNumericResult(string value) {
			value = value.Trim(' ');
			if (string.IsNullOrEmpty(value))
				return 0;
			int sign = 1;
			if (value[0] == '-') {
				sign *= -1;
				value = value.Substring(1);
			}
			if (value.Length > 255)
				return VariantValue.ErrorInvalidValueInFunction;
			value = value.ToUpper();
			double result = 0, lastMaxNum = 0;
			for (int i = value.Length - 1; i >= 0; --i) {
				int currentNum;
				if (!RomanToArabic.TryGetValue(value[i], out currentNum))
					return VariantValue.ErrorInvalidValueInFunction;
				if (currentNum < lastMaxNum)
					result -= currentNum;
				else {
					result += currentNum;
					lastMaxNum = currentNum;
				}
			}
			return sign * result;
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value));
			return collection;
		}
	}
	#endregion
}
