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
using System.Numerics;
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionImPower
	public class FunctionImPower : WorksheetFunctionBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "IMPOWER"; } }
		public override int Code { get { return 0x018E; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override VariantValue EvaluateCore(System.Collections.Generic.IList<VariantValue> arguments, WorkbookDataContext context) {
			if (arguments[0].IsEmpty && arguments[1].IsEmpty)
				return VariantValue.ErrorValueNotAvailable;
			VariantValue complexValue = context.GetTextComplexValue(arguments[0]);
			if (complexValue.IsError)
				return complexValue;
			SpreadsheetComplex complex;
			if (!SpreadsheetComplex.TryParse(complexValue.InlineTextValue, context, out complex))
				return VariantValue.ErrorNumber;
			VariantValue powerValue = GetNumericValue(arguments[1], context);
			if (powerValue.IsError)
				return powerValue;
			return ImPowerCore(complex, powerValue.NumericValue, context);
		}
		protected override VariantValue GetNumericValue(VariantValue value, WorkbookDataContext context) {
			if (value.IsBoolean)
				return VariantValue.ErrorInvalidValueInFunction;
			return base.GetNumericValue(value, context);
		}
		VariantValue ImPowerCore(SpreadsheetComplex complex, double power, WorkbookDataContext context) {
			if (complex.Value == Complex.Zero && power == 0)
				return VariantValue.ErrorNumber;
			SpreadsheetComplex result = new SpreadsheetComplex();
			result.Value = Complex.Pow(complex.Value, power);
			result.Suffix = complex.Suffix;
			return result.ToVariantValue(context);
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference));
			return collection;
		}
	}
	#endregion
}
