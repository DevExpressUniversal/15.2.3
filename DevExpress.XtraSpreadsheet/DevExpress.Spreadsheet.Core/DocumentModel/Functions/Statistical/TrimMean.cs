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
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionTrimMean
	public class FunctionTrimMean : WorksheetFunctionBase {
		#region Static Members
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Array | OperandDataType.Reference | OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			return collection;
		}
		#endregion
		#region Properties
		public override string Name { get { return "TRIMMEAN"; } }
		public override int Code { get { return 0x014B; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		#endregion
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue array = arguments[0];
			if (array.IsError)
				return array;
			if (!array.IsCellRange && !array.IsArray && !array.IsNumeric) {
				array = arguments[0].ToNumeric(context);
				if (array.IsError)
					return array;
			}
			VariantValue value = arguments[1].ToNumeric(context);
			if (value.IsError)
				return value;
			return GetResult(array, value.NumericValue);
		}
		VariantValue GetResult(VariantValue array, double percent) {
			bool percentIsValid = percent >= 0 && percent < 1;
			if (array.IsNumeric)
				return percentIsValid ? array.NumericValue : VariantValue.ErrorNumber;
			IVector<VariantValue> valuesVector = CreateVector(array);
			List<double> numericList = new List<double>();
			int count = valuesVector.Count;
			for (int i = 0; i < count; i++) {
				VariantValue value = valuesVector[i];
				if (value.IsError)
					return value;
				if (value.IsNumeric)
					numericList.Add(value.NumericValue);
			}
			if (!percentIsValid || numericList.Count == 0)
				return VariantValue.ErrorNumber;
			return GetFinalResult(numericList, percent);
		}
		IVector<VariantValue> CreateVector(VariantValue value) {
			if (value.IsArray)
				return new ArrayZVector(value.ArrayValue);
			return new RangeZNVector(value.CellRangeValue);
		}
		VariantValue GetFinalResult(List<double> numericList, double percent) {
			numericList.Sort();
			DeleteValues(numericList, (int)(numericList.Count * percent));
			int listCount = numericList.Count;
			double average = 0;
			for (int i = 0; i < listCount;)
				average += (numericList[i] - average) / ++i;
			return average;
		}
		void DeleteValues(List<double> numericList, int valuesToDelete) {
			if (valuesToDelete % 2.0 != 0)
				valuesToDelete--;
			if (valuesToDelete >= 2) {
				valuesToDelete /= 2;
				numericList.RemoveRange(0, valuesToDelete);
				numericList.RemoveRange(numericList.Count - valuesToDelete, valuesToDelete);
			}
		}
	}
	#endregion
}
