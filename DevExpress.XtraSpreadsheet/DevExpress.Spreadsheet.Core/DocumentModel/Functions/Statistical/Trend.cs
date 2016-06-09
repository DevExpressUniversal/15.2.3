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
using DevExpress.Office;
using DevExpress.Office.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionTrend
	public class FunctionTrend : WorksheetFunctionBase {
		#region Static Members
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Array | OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Array | OperandDataType.Reference, FunctionParameterOption.NonRequired));
			collection.Add(new FunctionParameter(OperandDataType.Array | OperandDataType.Reference, FunctionParameterOption.NonRequired));
			collection.Add(new FunctionParameter(OperandDataType.Reference | OperandDataType.Value, FunctionParameterOption.NonRequired));
			return collection;
		}
		#endregion
		#region Properties
		public override string Name { get { return "TREND"; } }
		public override int Code { get { return 0x0032; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Array; } }
		#endregion
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue knownY = arguments[0];
			if (arguments.Count == 1)
				return GetResult(knownY, VariantValue.Empty, VariantValue.Empty, true);
			VariantValue knownX = arguments[1];
			if (arguments.Count == 2)
				return GetResult(knownY, knownX, VariantValue.Empty, true);
			VariantValue newX = arguments[2];
			if (arguments.Count == 3)
				return GetResult(knownY, knownX, newX, true);
			VariantValue constant = arguments[3];
			if (constant.IsError)
				return constant;
			if (constant.IsEmpty)
				return GetResult(knownY, knownX, newX, true);
			constant = constant.ToBoolean(context);
			if (constant.IsError) {
				if (constant == VariantValue.ErrorGettingData)
					return constant;
				return VariantValue.ErrorInvalidValueInFunction;
			}
			return GetResult(knownY, knownX, newX, constant.BooleanValue);
		}
		protected virtual VariantValue GetResult(VariantValue knownY, VariantValue knownX, VariantValue newX, bool hasIntercept) {
			return RegressionMath.GetTrend(knownY, knownX, newX, hasIntercept);
		}
	}
	#endregion
}
