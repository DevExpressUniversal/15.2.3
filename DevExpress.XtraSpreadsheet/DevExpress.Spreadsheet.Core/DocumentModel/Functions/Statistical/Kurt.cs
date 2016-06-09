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

using System.Collections.Generic;
using System;
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionKurt
	public class FunctionKurt : WorksheetGenericFunctionBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "KURT"; } }
		public override int Code { get { return 0x0142; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override FunctionResult CreateInitialFunctionResult(WorkbookDataContext context) {
			return new FunctionKurtResult(context);
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Array | OperandDataType.Reference | OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Array | OperandDataType.Reference | OperandDataType.Value, FunctionParameterOption.NonRequiredUnlimited));
			return collection;
		}
	}
	#endregion
	#region FunctionKurtResult
	public class FunctionKurtResult : FunctionStdevResult {
		List<double> collection = new List<double>();
		public FunctionKurtResult(WorkbookDataContext context)
			: base(context) {
		}
		public override bool ProcessConvertedValue(VariantValue value) {
			collection.Add(value.NumericValue);
			return base.ProcessConvertedValue(value);
		}
		public override VariantValue GetFinalValue() {
			double count = collection.Count;
			double stdev = Math.Sqrt(Variance / (count - 1));
			if (count < 4 || stdev == 0)
				return VariantValue.ErrorDivisionByZero;
			double sum = 0;
			for (int i = 0; i < count; i++)
				sum += Math.Pow((collection[i] - Mean) / stdev, 4);
			double frac1 = count / (count - 1.0) * (count + 1.0) / (count - 2.0) * sum / (count - 3.0);
			double frac2 = 3.0 * (count - 1.0) / (count - 2.0) * (count - 1.0) / (count - 3.0);
			return frac1 - frac2;
		}
	}
	#endregion
}
