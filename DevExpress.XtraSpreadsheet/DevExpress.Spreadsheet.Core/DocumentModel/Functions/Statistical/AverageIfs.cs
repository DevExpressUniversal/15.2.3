﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
	#region FunctionAverageIfs
	public class FunctionAverageIfs : FunctionCalculationIfsBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "AVERAGEIFS"; } }
		public override int Code { get { return 0x01E4; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override int CellRangeValueStartIndex { get { return 1; } }
		protected override FunctionResult CreateInitialFunctionResult(WorkbookDataContext context) {
			FunctionAverageIfsResult result = new FunctionAverageIfsResult(context);
			result.ProcessErrorValues = true;
			return result;
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			for (int i = 0; i < 127; i++) {
				collection.Add(new FunctionParameter(OperandDataType.Reference, FunctionParameterOption.NonRequired | FunctionParameterOption.NonRequiredUnlimited));
				collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.NonRequired | FunctionParameterOption.NonRequiredUnlimited));
			}
			return collection;
		}
	}
	#endregion
	#region FunctionAverageIfsResult
	public class FunctionAverageIfsResult : FunctionAverageResult {
		public FunctionAverageIfsResult(WorkbookDataContext context)
			: base(context) {
		}
		public override bool ShouldProcessValueCore(VariantValue value) {
			if (ArrayOrRangeProcessing)
				return value.IsNumeric || value.IsError;
			return !value.IsEmpty;
		}
	}
	#endregion
}
