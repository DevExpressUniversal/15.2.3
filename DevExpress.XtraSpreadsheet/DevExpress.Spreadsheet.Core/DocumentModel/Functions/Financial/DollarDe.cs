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

using DevExpress.XtraSpreadsheet.Utils;
using System;
using System.Collections.Generic;
using System.Numerics;
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionDollarDe
	public class FunctionDollarDe : WorksheetFunctionBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "DOLLARDE"; } }
		public override int Code { get { return 0x01BB; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue value = GetNumberValue(arguments[0], context);
			if (value.IsError)
				return value;
			double amount = value.NumericValue;
			value = GetNumberValue(arguments[1], context);
			if (value.IsError)
				return value;
			int fraction = (int)value.NumericValue;
			if (fraction < 0)
				return VariantValue.ErrorNumber;
			if (fraction == 0)
				return VariantValue.ErrorDivisionByZero;
			int shift = 1;
			do 
				shift *= 10; 
			while (shift < fraction);
			return GetResult(amount, fraction, shift);
		}
		protected virtual VariantValue GetResult(double amount, int fraction, int shift) {
			return (long)amount + (amount - (long)amount) * shift / fraction;
		}
		protected VariantValue GetNumberValue(VariantValue argument, WorkbookDataContext context) {
			if (argument.IsEmpty)
				return VariantValue.ErrorValueNotAvailable;
			if (argument.IsBoolean)
				return VariantValue.ErrorInvalidValueInFunction;
			if (argument.IsCellRange) {
				VariantValue cell = argument.CellRangeValue.GetFirstCellValue();
				if (argument.CellRangeValue.CellCount > 1 || cell.IsBoolean)
					return VariantValue.ErrorInvalidValueInFunction;
			}
			return argument.ToNumeric(context);
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			return collection;
		}
	}
	#endregion
}
