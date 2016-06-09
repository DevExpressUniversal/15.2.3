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
	#region FunctionPercentile
	public class FunctionPercentile : WorksheetGenericFunctionBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "PERCENTILE"; } }
		public override int Code { get { return 0x0148; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue array = arguments[0];
			if (array.IsError)
				return array;
			VariantValue numberValue = arguments[1].ToNumeric(context);
			if (numberValue.IsError)
				return numberValue;
			FunctionPercentileResult functionResult = (FunctionPercentileResult)CreateInitialFunctionResult(context);
			VariantValue error = functionResult.SetPercentile(numberValue.NumericValue);
			if (error.IsError)
				return error;
			return EvaluateValue(array, functionResult);
		}
		protected override FunctionResult CreateInitialFunctionResult(WorkbookDataContext context) {
			return new FunctionPercentileResult(context);
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Array | OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			return collection;
		}
	}
	#endregion
	#region FunctionPercentileResult
	public class FunctionPercentileResult : FunctionSumResultBase {
		readonly List<double> listNumbers;
		double percentile;
		public FunctionPercentileResult(WorkbookDataContext context)
			: base(context) {
			this.listNumbers = new List<double>();
		}
		protected internal double Percentile { get { return percentile; } set { percentile = value; } }
		protected List<double> ListNumbers { get { return listNumbers; } }
		protected internal virtual VariantValue SetPercentile(double value) {
			if (value < 0 || value > 1)
				return VariantValue.ErrorNumber;
			percentile = value;
			return VariantValue.Empty;
		}
		public override bool ProcessConvertedValue(VariantValue value) {
			listNumbers.Add(value.NumericValue);
			return true;
		}
		public override VariantValue GetFinalValue() {
			if (listNumbers.Count == 0)
				return VariantValue.ErrorNumber;
			listNumbers.Sort();
			int lastIndex = listNumbers.Count - 1;
			if (percentile == 0 || lastIndex == 0)
				return listNumbers[0];
			if (percentile == 1)
				return listNumbers[lastIndex];
			return GetFinalValueCore(lastIndex);
		}
		protected VariantValue GetFinalValueCore(int lastIndex) {
			double aspect = percentile * lastIndex;
			int index = (int)Math.Floor(aspect);
			aspect -= index;
			return (listNumbers[index] * (1 - aspect) + listNumbers[index + 1] * aspect);
		}
	}
	#endregion
}
