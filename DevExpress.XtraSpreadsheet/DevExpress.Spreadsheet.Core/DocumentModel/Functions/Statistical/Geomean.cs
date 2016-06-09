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
	#region FunctionGeomean
	public class FunctionGeomean : WorksheetGenericFunctionBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "GEOMEAN"; } }
		public override int Code { get { return 0x013F; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override FunctionResult CreateInitialFunctionResult(WorkbookDataContext context) {
			return new FunctionGeomeanResult(context);
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Reference | OperandDataType.Value | OperandDataType.Array));
			collection.Add(new FunctionParameter(OperandDataType.Reference | OperandDataType.Value | OperandDataType.Array, FunctionParameterOption.NonRequiredUnlimited));
			return collection;
		}
	}
	#endregion
	#region FunctionGeomeanResult
	public class FunctionGeomeanResult : FunctionResult {
		double total;
		int count;
		protected double Total { get { return total; } set { total = value; } }
		public FunctionGeomeanResult(WorkbookDataContext context)
			: base(context) {
				total = 1;
		}
		public override bool ShouldProcessValueCore(VariantValue value) {
			if (ArrayOrRangeProcessing)
				return value.IsNumeric;
			return true;
		}
		public override VariantValue ConvertValue(VariantValue value) {
			if (value.IsError)
				return value;
			if(value.IsEmpty) 
				return VariantValue.ErrorNumber;
			VariantValue result = value.ToNumeric(Context);
			if (result.IsError)
				return VariantValue.ErrorInvalidValueInFunction;
			if (result.NumericValue <= 0)
				return VariantValue.ErrorNumber;
			return result;
		}
		public override bool ProcessConvertedValue(VariantValue value) {
			total *= value.NumericValue;
			count++;
			return true;
		}
		public override VariantValue GetFinalValue() {
			if (count == 0)
				return VariantValue.ErrorNumber;
			double power = 1 / (double)count;
			return Math.Pow(total, power);
		}
	}
	#endregion
}
