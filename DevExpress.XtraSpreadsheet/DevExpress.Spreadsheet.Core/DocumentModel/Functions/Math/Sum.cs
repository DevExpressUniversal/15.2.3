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
	#region FunctionSum
	public class FunctionSum : WorksheetGenericFunctionBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "SUM"; } }
		public override int Code { get { return 0x0004; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override FunctionResult CreateInitialFunctionResult(WorkbookDataContext context) {
			return new FunctionSumResult(context);
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Reference | OperandDataType.Value | OperandDataType.Array, FakeParameterType.Number));
			collection.Add(new FunctionParameter(OperandDataType.Reference | OperandDataType.Value | OperandDataType.Array, FunctionParameterOption.NonRequiredUnlimited, FakeParameterType.Number));
			return collection;
		}
	}
	#endregion
	#region FunctionSumResult
	public class FunctionSumResult : FunctionSumResultBase {
		public FunctionSumResult(WorkbookDataContext context)
			: base(context) {
		}
		public override bool ShouldProcessValueCore(VariantValue value) {
			if (ArrayOrRangeProcessing) {
				if (value.IsError)
					return true;
				if (value.IsNumeric)
					return value.NumericValue != 0;
				else
					return false;
			}
			if (value.IsEmpty)
				return false;
			if (value.IsNumeric)
				return value.NumericValue != 0;
			return true;
		}
	}
	#endregion
	#region FunctionSumResult
	public abstract class FunctionSumResultBase : FunctionResult {
		double total;
		protected FunctionSumResultBase(WorkbookDataContext context)
			: base(context) {
		}
		public override bool ShouldProcessValueCore(VariantValue value) {
			if (ArrayOrRangeProcessing)
				return value.IsNumeric || value.IsError;
			return !value.IsEmpty;
		}
		public override VariantValue ConvertValue(VariantValue value) {
			return value.ToNumeric(Context);
		}
		public override bool ProcessConvertedValue(VariantValue value) {
			total += value.NumericValue;
			return true;
		}
		public override VariantValue GetFinalValue() {
			return total;
		}
	}
	#endregion
}
