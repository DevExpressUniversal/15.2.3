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
	#region FunctionWeekDay
	public class FunctionWeekDay : FunctionSerialNumberBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "WEEKDAY"; } }
		public override int Code { get { return 0x0046; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			int argumentsCount = arguments.Count;
			VariantValue serialNumber = ToSerialNumber(arguments[0], context);
			if (serialNumber.IsError)
				return serialNumber;
			VariantValue numberDayOfWeek = (int)context.FromDateTimeSerialForDayOfWeek(serialNumber.NumericValue).DayOfWeek + 1;
			if (numberDayOfWeek.IsError)
				return numberDayOfWeek;
			VariantValue returnType = 1;
			if (argumentsCount == 2) {
				returnType = ToSerialNumberCore(arguments[1], context);
				if (returnType.IsError)
					return returnType;
				if (!IsValidReturnType((int)returnType.NumericValue))
					return VariantValue.ErrorNumber; 
			}
			return GetNumericResult((int)numberDayOfWeek.NumericValue, (int)returnType.NumericValue);
		}
		protected override VariantValue ToSerialNumberCore(VariantValue value, WorkbookDataContext context) {
			return value.ToNumeric(context);
		}
		bool IsValidReturnType(int returnType) {
			if (returnType >= 1 || returnType <= 3 || returnType >= 11 || returnType <= 17)
				return true;
			return false;
		}
		int GetNumberAtRightCircularShift(int numberShift, int position) {
			numberShift %= 7; 
			int currentNumber = 7 - numberShift;
			for (int i = 0; i < numberShift; i++) 
				if (position == i)
					return currentNumber;
				else
					currentNumber++;
			currentNumber = 0;
			position -= numberShift;
			for (int i = 0; i < position; i++)
				currentNumber++;
			return currentNumber % 7;
		}
		VariantValue GetNumericResult(int numberDayOfWeek, int returnType) {
			if (returnType == 1 || returnType == 2)
				return GetNumberAtRightCircularShift(returnType, numberDayOfWeek) + 1;
			if (returnType == 3)
				return GetNumberAtRightCircularShift(2, numberDayOfWeek);
			for (int i = 11; i <= 17; i++) {
				if (returnType == i)
					return GetNumberAtRightCircularShift(i - 9, numberDayOfWeek) + 1;
			}
			return VariantValue.ErrorNumber;
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.NonRequired));
			return collection;
		}
	}
	#endregion
}
