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
	#region FunctionXnpv
	public class FunctionXnpv : FunctionSerialNumberBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "XNPV"; } }
		public override int Code { get { return 0x01AE; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue rate = arguments[0];
			if (rate.IsEmpty)
				return VariantValue.ErrorValueNotAvailable;
			rate = context.DereferenceWithoutCrossing(rate).ToNumeric(context);
			if (rate.IsError)
				return rate;
			VariantValue values = ToArray(arguments[1], context);
			if (values.IsError)
				return values;
			VariantValue dates = ToArray(arguments[2], context);
			if (dates.IsError)
				return dates;
			if (rate.NumericValue <= 0)
				return VariantValue.ErrorNumber;
			IVariantArray valuesIArray = values.ArrayValue;
			IVariantArray datesIArray = dates.ArrayValue;
			if (valuesIArray.Count != datesIArray.Count)
				return VariantValue.ErrorNumber;
			VariantValue firstDate = PrepareFirstDate(datesIArray[0], context);
			if (firstDate.IsError)
				return firstDate;
			int firstPaymentDate = (int)firstDate.NumericValue;
			VariantValue firstValue = valuesIArray[0];
			if (firstValue.IsError)
				return firstValue;
			if (firstValue.IsBoolean || firstValue.IsText || firstValue.IsEmpty)
				return VariantValue.ErrorInvalidValueInFunction;
			double result = 0;
			for (int i = 0; i < valuesIArray.Count; ++i) {
				VariantValue value = valuesIArray[i];
				if (value.IsError)
					return value;
				VariantValue date = datesIArray[i];
				if (date.IsError)
					return date;
				if (!value.IsNumeric || !date.IsNumeric)
					return VariantValue.ErrorNumber;
				date = ToSerialNumber(date, context);
				if (date.IsError)
					return date;
				int dateValue = (int)date.NumericValue;
				if (firstPaymentDate > dateValue || dateValue == 0)
					return VariantValue.ErrorNumber;
				result += value.NumericValue / Math.Pow(1 + rate.NumericValue, (double)(dateValue - firstPaymentDate) / 365);
			}
			return result;
		}
		VariantValue ToArray(VariantValue value, WorkbookDataContext context) {
			if (value.IsError || value.IsArray)
				return value;
			if (value.IsEmpty)
				return VariantValue.ErrorValueNotAvailable;
			if (value.IsNumeric || value.IsText || (value.IsCellRange && value.CellRangeValue.CellCount == 1)) {
				VariantArray array = VariantArray.Create(1, 1);
				value = value.ToNumeric(context);
				if (value == VariantValue.ErrorGettingData)
					return value;
				if (value.IsError)
					return VariantValue.ErrorInvalidValueInFunction;
				array.SetValue(0, 0, value);
				return VariantValue.FromArray(array);
			}
			if (value.IsCellRange) {
				CellRangeBase rangeBaseValue = value.CellRangeValue;
				if (rangeBaseValue.RangeType == CellRangeType.UnionRange)
					return VariantValue.ErrorInvalidValueInFunction;
				RangeVariantArray array = new RangeVariantArray(rangeBaseValue.GetFirstInnerCellRange());
				return VariantValue.FromArray(array);
			}
			return VariantValue.ErrorInvalidValueInFunction;
		}
		VariantValue PrepareFirstDate(VariantValue value, WorkbookDataContext context) {
			if (value.IsError)
				return value;
			if (!value.IsNumeric)
				return VariantValue.ErrorInvalidValueInFunction;
			return ToSerialNumber(value, context);
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference | OperandDataType.Array));
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference | OperandDataType.Array));
			return collection;
		}
	}
	#endregion
}
