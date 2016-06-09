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
	#region FunctionCount
	public class FunctionCountBlank : WorksheetGenericFunctionBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "COUNTBLANK"; } }
		public override int Code { get { return 0x015B; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override FunctionResult CreateInitialFunctionResult(WorkbookDataContext context) {
			FunctionCountBlankResult result = new FunctionCountBlankResult(context);
			result.ProcessErrorValues = true;
			return result;
		}
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue value = arguments[0];
			if(value.IsError)
				return value;
			return base.EvaluateCore(arguments, context);
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Reference));
			return collection;
		}
	}
	#endregion
	#region FunctionCountResult
	public class FunctionCountBlankResult : FunctionResult {
		double count;
		public FunctionCountBlankResult(WorkbookDataContext context)
			: base(context) {
		}
		protected double Count { get { return count; } set { count = value; } }
		public override bool ShouldProcessValueCore(VariantValue value) {
			if (value.IsCellRange) {
				count += value.CellRangeValue.CellCount;
				return true;
			}
			return !value.IsEmpty && (!value.IsText || !string.IsNullOrEmpty(value.GetTextValue(Context.StringTable)));
		}
		public override VariantValue ConvertValue(VariantValue value) {
			if (value.IsError)
				return 0;
			return value;
		}
		public override bool ProcessConvertedValue(VariantValue value) {
			count--;
			return true;
		}
		public override VariantValue GetFinalValue() {
			return count;
		}
	}
	#endregion
}
