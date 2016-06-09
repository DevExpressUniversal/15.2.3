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
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionFvSchedule
	public class FunctionFvSchedule : WorksheetFunctionBase {
		#region Static Members
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference | OperandDataType.Array));
			return collection;
		}
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		#endregion
		#region Properties
		public override string Name { get { return "FVSCHEDULE"; } }
		public override int Code { get { return 0x01DC; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		#endregion
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue presentValue = arguments[0];
			if (presentValue.IsEmpty)
				return VariantValue.ErrorValueNotAvailable;
			presentValue = context.DereferenceWithoutCrossing(presentValue).ToNumeric(context);
			if (presentValue.IsError)
				return presentValue;
			VariantValue scheduleArray = arguments[1];
			if (scheduleArray.IsError)
				return scheduleArray;
			return ProcessArray(presentValue.NumericValue, scheduleArray, context);
		}
		VariantValue ProcessArray(double presentValue, VariantValue scheduleArray, WorkbookDataContext context) {
			IVector<VariantValue> vector;
			if (scheduleArray.IsEmpty)
				return VariantValue.ErrorValueNotAvailable;
			else if (scheduleArray.IsBoolean)
				return VariantValue.ErrorInvalidValueInFunction;
			else if (scheduleArray.IsArray)
				vector = new ArrayZVector(scheduleArray.ArrayValue);
			else if (scheduleArray.IsCellRange) {
				CellRangeBase cellRange = scheduleArray.CellRangeValue;
				if (cellRange.RangeType == CellRangeType.UnionRange)
					return VariantValue.ErrorInvalidValueInFunction;
				vector = new RangeZVector(cellRange.GetFirstInnerCellRange());
			}
			else
				return GetNumericCaseResult(presentValue, scheduleArray.ToNumeric(context));
			return GetArrayCaseResult(presentValue, vector);
		}
		VariantValue GetNumericCaseResult(double presentValue, VariantValue scheduleNumber) {
			if (scheduleNumber.IsError)
				return scheduleNumber;
			return presentValue + presentValue * scheduleNumber.NumericValue;
		}
		VariantValue GetArrayCaseResult(double presentValue, IVector<VariantValue> vector) {
			double result = presentValue;
			int count = vector.Count;
			for (int i = 0; i < count; i++) {
				VariantValue current = vector[i];
				if (current.IsError)
					return current;
				if (current.IsNumeric || current.IsEmpty)
					result += result * current.NumericValue;
				else 
					return VariantValue.ErrorInvalidValueInFunction;
			}
			return result;
		}
	}
	#endregion
}
