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
using System.Numerics;
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionLcm
	public class FunctionLcm : WorksheetGenericFunctionBase {
		#region Static Members
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Reference | OperandDataType.Value | OperandDataType.Array, FakeParameterType.Number));
			collection.Add(new FunctionParameter(OperandDataType.Reference | OperandDataType.Value | OperandDataType.Array, FunctionParameterOption.NonRequiredUnlimited, FakeParameterType.Number));
			return collection;
		}
		#endregion
		#region Properties
		public override string Name { get { return "LCM"; } }
		public override int Code { get { return 0x01DB; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		#endregion
		protected override FunctionResult CreateInitialFunctionResult(WorkbookDataContext context) {
			return new FunctionLcmResult(context);
		}
	}
	#endregion
	#region FunctionLcmResult
	public class FunctionLcmResult : FunctionGcdResult {
		bool skipCell;
		public FunctionLcmResult(WorkbookDataContext context)
			: base(context) {
			Result = 1;
		}
		public override bool ShouldProcessValueCore(VariantValue value) {
			if (value.IsCellRange) {
				if (value.CellRangeValue.CellCount == 1)
					skipCell = true;
				else {
					skipCell = false;
					FixEmptyCellsError();
				}
			}
			if (CellRangeProcessing && value.IsEmpty) {
				if (HasOnlyEmptyCells)
					Error = VariantValue.ErrorInvalidValueInFunction;
				if (skipCell)
					return false;
			}
			return true;
		}
		protected override double GetNextValue(double value) {
			if (Result == 0 || value == 0)
				return 0;
			if (Result == value || value == 1)
				return Result;
			if (Result == 1)
				return value;
			return GetLeastCommonMultiple(value);
		}
		double GetLeastCommonMultiple(double value) {
			double gcd = GetGreatestCommonDivisor(value);
			double max = Math.Max(Result, value);
			double min = Math.Min(Result, value);
			return (max / gcd * min);
		}
	}
	#endregion
}
