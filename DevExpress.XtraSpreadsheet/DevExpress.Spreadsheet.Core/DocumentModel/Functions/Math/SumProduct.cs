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
	#region FunctionSumProduct
	public class FunctionSumProduct : WorksheetFunctionBase {		
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "SUMPRODUCT"; } }
		public override int Code { get { return 0x00E4; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			int width = -1;
			int height = -1;
			for (int n = 0; n < arguments.Count; n++) {
				VariantValue value = arguments[n];
				if (value == VariantValue.ErrorGettingData)
					return value;
				if (value.IsError || (value.IsCellRange && value.CellRangeValue.RangeType == CellRangeType.UnionRange))
					return VariantValue.ErrorInvalidValueInFunction;
				int valueWidth = 1;
				int valueHeight = 1;
				if (value.IsArray){
					valueWidth = value.ArrayValue.Width;
					valueHeight = value.ArrayValue.Height;
				}
				else if (value.IsCellRange){
					valueWidth = value.CellRangeValue.Width;
					valueHeight = value.CellRangeValue.Height;
				}
				if (n == 0){
					width = valueWidth;
					height = valueHeight;
				}
				else if (width != valueWidth || height != valueHeight)
					return VariantValue.ErrorInvalidValueInFunction;
			}
			return GetSumProduct(arguments, context, width, height);
		}
		VariantValue GetSumProduct(IList<VariantValue> arguments, WorkbookDataContext context, double width, double height) {
			VariantValue result = 0;
			for (int i = 0; i < height; i++) {
				for (int j = 0; j < width; j++) {
					double product = 1;
					for (int n = 0; n < arguments.Count; n++) {
						VariantValue value = arguments[n];					   
						if (value.IsError)
							return value;
						VariantValue currentValue = GetCurrentValue(i, j, value);
						if (currentValue.IsError)
							return currentValue;
						VariantValue number = currentValue.ToNumeric(context);
						if (number.IsError)
							number.NumericValue = 0;
						if (currentValue.IsText || currentValue.IsBoolean || currentValue.IsEmpty)
							if (width == 1 && height == 1) {
								return VariantValue.ErrorInvalidValueInFunction;
							} else
								number.NumericValue = 0;
						product *= number.NumericValue;
					}
					result.NumericValue += product;
				}
			}
			return result;
		}
		VariantValue GetCurrentValue(int i, int j, VariantValue value) {
			VariantValue result;
			if (value.IsArray)
				result = value.ArrayValue.GetValue(i, j);
			else if (value.IsCellRange)
				result = value.CellRangeValue.GetCellValueRelative(j, i);
			else
				result = value;
			return result;
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Array, FunctionParameterOption.DoNotDereferenceEmptyValueAsZero));
			collection.Add(new FunctionParameter(OperandDataType.Array, FunctionParameterOption.NonRequiredUnlimited | FunctionParameterOption.DoNotDereferenceEmptyValueAsZero));
			return collection;
		}
	}
	#endregion
}
