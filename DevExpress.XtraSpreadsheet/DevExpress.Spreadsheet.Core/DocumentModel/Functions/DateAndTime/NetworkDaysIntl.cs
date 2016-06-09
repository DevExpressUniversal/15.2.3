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
	#region FunctionNetworkDays
	public class FunctionNetworkDaysIntl : FunctionWorkdayBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "NETWORKDAYS.INTL"; } }
		public override int Code { get { return 0x400F; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue startValue = ToSerialNumber(arguments[0], context);
			if(startValue.IsError)
				return startValue;
			int startSerialNumber = (int)startValue.NumericValue;
			VariantValue endValue = ToSerialNumber(arguments[1], context);
			if(endValue.IsError)
				return endValue;
			int endSerialNumber = (int)endValue.NumericValue;
			int[] weekdays = new int[7];
			VariantValue operationResult = GetWeekdays(arguments, context, weekdays, true);
			if(operationResult.IsError)
				return operationResult;
			List<double> holidays = new List<double>();
			if(arguments.Count == 4) {
				VariantValue holidaysValue = arguments[3];
				bool operandDataTypeIsValue = !holidaysValue.IsCellRange && !holidaysValue.IsArray;
				VariantValue errorHolidays = TryAddHolidays(holidaysValue, context, startSerialNumber, endSerialNumber, holidays, weekdays, operandDataTypeIsValue);
				if(errorHolidays.IsError)
					return errorHolidays;
			}
			int result = GetCountWorkdays(startSerialNumber, endSerialNumber, context, weekdays);
			return result - Math.Sign(result) * holidays.Count;
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.NonRequired));
			collection.Add(new FunctionParameter(OperandDataType.Reference | OperandDataType.Array | OperandDataType.Value, FunctionParameterOption.NonRequired));
			return collection;
		}
	}
	#endregion
}
