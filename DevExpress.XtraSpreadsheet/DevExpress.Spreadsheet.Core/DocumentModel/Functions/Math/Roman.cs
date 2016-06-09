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
using DevExpress.Office.NumberConverters;
using System.Collections;
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionRoman
	public class FunctionRoman : WorksheetFunctionBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "ROMAN"; } }
		public override int Code { get { return 0x0162; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override VariantValue EvaluateCore(System.Collections.Generic.IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue numberValue = arguments[0].ToNumeric(context);
			if (numberValue.IsError)
				return numberValue;
			long number = (long)numberValue.NumericValue; int form = 0;
			if (arguments.Count > 1 && !arguments[1].IsEmpty) {
				VariantValue formValue = arguments[1].ToNumeric(context);
				if (formValue.IsError)
					return formValue;
				form = (int)formValue.NumericValue;
			}
			if (number < 0 || number >= 4000 || form < 0 || form >= 5)
				return VariantValue.ErrorInvalidValueInFunction;
			return GetResult(number, form);
		}
		string GetResult(long number, int form) {
			UpperRomanNumberConverterClassic converterClassic = new UpperRomanNumberConverterClassic();
			UpperRomanNumberConverterAlternative_x45x99 converter_x45x99 = new UpperRomanNumberConverterAlternative_x45x99();
			UpperRomanNumberConverterAlternative_x90x99 converter_x90x99 = new UpperRomanNumberConverterAlternative_x90x99();
			UpperRomanNumberConverterAlternative_x95x99 converter_x95x99 = new UpperRomanNumberConverterAlternative_x95x99();
			UpperRomanNumberConverterAlternative_x99 converter_x99 = new UpperRomanNumberConverterAlternative_x99();
			string result = "";
			if ((number / 100 % 10 == 4 || number / 100 % 10 == 9) && number % 100 >= 45) {
				result += (number > 1000) ? converterClassic.ConvertNumberCore(number / 1000 * 1000) : "";
				switch (form) {
					case 0:
						result += converterClassic.ConvertNumber(number % 1000);
						break;
					case 1:
						result += converter_x45x99.ConvertNumber(number % 1000);
						break;
					case 2: 
						if (number % 100 < 90) result += converter_x45x99.ConvertNumber(number % 1000);
						if (number % 100 >= 90) result += converter_x90x99.ConvertNumber(number % 1000);
						break;
					case 3: 
						if (number % 100 < 90) result += converter_x45x99.ConvertNumber(number % 1000);
						if (number % 100 >= 90 && number % 100 < 95) result += converter_x90x99.ConvertNumber(number % 1000);
						if (number % 100 >= 95 && number % 100 < 100) result += converter_x95x99.ConvertNumber(number % 1000);
						break;
					case 4:
						if (number % 100 < 90) result += converter_x45x99.ConvertNumber(number % 1000);
						if (number % 100 >= 90 && number % 100 < 95) result += converter_x90x99.ConvertNumber(number % 1000);
						if (number % 100 >= 95 && number % 100 < 99) result += converter_x95x99.ConvertNumber(number % 1000);
						if (number % 100 == 99) result += converter_x99.ConvertNumber(number % 1000);
						break;
				}
			}
			else if ((number % 100 >= 45 && number % 100 < 50) || (number % 100 >= 95 && number % 100 < 100)) {
				result += converterClassic.ConvertNumber(number / 100 * 100);
				if (form == 0)  result += converterClassic.ConvertNumber(number % 100);
				else  result += (form != 1 && (number % 100 == 99 || number % 100 == 49)) ? converter_x99.ConvertNumber(number % 100) : converter_x45x99.ConvertNumber(number % 100);
			}
			else
				return converterClassic.ConvertNumber(number);
			return result;
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.NonRequired));
			return collection;
		}
	}
	#endregion
}
