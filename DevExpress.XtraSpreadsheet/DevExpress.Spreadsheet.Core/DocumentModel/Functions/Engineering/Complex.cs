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

using DevExpress.XtraSpreadsheet.Utils;
using System;
using System.Collections.Generic;
using System.Numerics;
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionComplex
	public class FunctionComplex : WorksheetFunctionBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "COMPLEX"; } }
		public override int Code { get { return 0x019B; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue realNum = arguments[0];
			if (realNum.IsEmpty)
				return VariantValue.ErrorValueNotAvailable;
			realNum = context.DereferenceWithoutCrossing(realNum).ToNumeric(context);
			if (realNum.IsError)
				return realNum;
			VariantValue imNum = arguments[1];
			if (imNum.IsEmpty)
				return VariantValue.ErrorValueNotAvailable;
			imNum = context.DereferenceWithoutCrossing(imNum).ToNumeric(context);
			if (imNum.IsError)
				return imNum;
			char suffix = 'i';
			if (arguments.Count > 2) {
				VariantValue suffixValue = arguments[2];
				if (suffixValue.IsCellRange && suffixValue.CellRangeValue.CellCount > 1)
					return VariantValue.ErrorInvalidValueInFunction;
				suffixValue = context.DereferenceValue(suffixValue, false);
				if (!suffixValue.IsEmpty) {
					suffixValue = suffixValue.ToText(context);
					if (suffixValue.IsError)
						return suffixValue;
					string suffixString = suffixValue.InlineTextValue;
					if (!string.IsNullOrEmpty(suffixString)) {
						if (suffixString.Length != 1)
							return VariantValue.ErrorInvalidValueInFunction;
						suffix = suffixString[0];
						if (suffix != 'i' && suffix != 'j')
							return VariantValue.ErrorInvalidValueInFunction;
					}
				}
			}
			SpreadsheetComplex result = new SpreadsheetComplex();
			result.Value = new Complex(realNum.NumericValue, imNum.NumericValue);
			result.Suffix = suffix;
			return result.ToVariantValue(context);
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.NonRequired | FunctionParameterOption.DoNotDereferenceEmptyValueAsZero));
			return collection;
		}
	}
	#endregion
}
