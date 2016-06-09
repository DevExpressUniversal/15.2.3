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
	#region FunctionTime
	public class FunctionTime : FunctionDateBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "TIME"; } }
		public override int Code { get { return 0x0042; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			double maxValue = 32767;
			double secondsCountInDays = 24 * 60 * 60;
			VariantValue hourValue = arguments[0].ToNumeric(context);
			if (hourValue.IsError)
				return hourValue;
			VariantValue minuteValue = arguments[1].ToNumeric(context);
			if (minuteValue.IsError)
				return minuteValue;
			VariantValue secondValue = arguments[2].ToNumeric(context);
			if (secondValue.IsError)
				return secondValue;
			if ((hourValue.NumericValue > maxValue) || (minuteValue.NumericValue > maxValue) || (secondValue.NumericValue > maxValue))
				return VariantValue.ErrorNumber;
			double seconds = Truncate(secondValue.NumericValue);
			double minutes = Truncate(minuteValue.NumericValue);
			double hours = Truncate(hourValue.NumericValue);
			if (seconds > 59) {
				minutes += Truncate(seconds / 60);
				seconds %= 60;				
			}
			if (minutes > 59) {
				hours += Truncate(minutes / 60);
				minutes %= 60;				
			}
			if (hours > 23)
				hours %= 24;
			double result = ((hours * 60 * 60 + minutes * 60 + seconds) / secondsCountInDays);
			return (result >= 0) ? result : VariantValue.ErrorNumber;
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			return collection;
		}
	}
	#endregion
}
