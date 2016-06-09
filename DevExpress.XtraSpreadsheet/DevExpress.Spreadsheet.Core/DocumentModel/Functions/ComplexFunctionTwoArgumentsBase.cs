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
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region WorksheetFunctionTwoComplexArgumentsBase
	public abstract class WorksheetFunctionTwoComplexArgumentsBase : WorksheetFunctionBase {
		#region Fields
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		protected const char defautSuffixValue = 'i';
		#endregion
		#region Helper functions
		protected char GetConsistentSuffix(SpreadsheetComplex argument1, SpreadsheetComplex argument2) {
			char result = '\0';
			if(argument1.Value.Imaginary == 0)
				result = argument2.Value.Imaginary == 0 ? defautSuffixValue : argument2.Suffix;
			else if(argument2.Value.Imaginary == 0)
				result = argument1.Value.Imaginary == 0 ? defautSuffixValue : argument1.Suffix;
			else if(argument1.Suffix == argument2.Suffix)
				result = argument1.Suffix;
			return result;
		}
		#endregion
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference));
			return collection;
		}
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			if(arguments[0].IsEmpty || arguments[1].IsEmpty)
				return VariantValue.ErrorValueNotAvailable;
			SpreadsheetComplex arg1;
			SpreadsheetComplex arg2;
			VariantValue argument = context.GetTextComplexValue(arguments[0]);
			if(argument.IsError)
				return argument;
			if(!SpreadsheetComplex.TryParse(argument.InlineTextValue, context, out arg1))
				return VariantValue.ErrorNumber;
			argument = context.GetTextComplexValue(arguments[1]);
			if(argument.IsError)
				return argument;
			if(!SpreadsheetComplex.TryParse(argument.InlineTextValue, context, out arg2))
				return VariantValue.ErrorNumber;
			return EvaluateArguments(arg1, arg2, context);
		}
		protected abstract VariantValue EvaluateArguments(SpreadsheetComplex argument1, SpreadsheetComplex argument2, WorkbookDataContext context);
	}
	#endregion
}
