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
	#region FunctionFind
	public class FunctionFind : WorksheetFunctionBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "FIND"; } }
		public override int Code { get { return 0x007C; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue findValue = arguments[0].ToText(context);
			if (findValue.IsError)
				return findValue;
			VariantValue withinValue = arguments[1].ToText(context);
			if (withinValue.IsError)
				return withinValue;
			return GetIndexOf(arguments, context, arguments.Count, findValue, withinValue);
		}
		VariantValue GetIndexOf(IList<VariantValue> arguments, WorkbookDataContext context, int count, VariantValue findValue, VariantValue withinValue) {
			string withinText = withinValue.GetTextValue(context.StringTable);
			string findText = findValue.GetTextValue(context.StringTable);
			if (!FindTextValid(findText))
				return VariantValue.ErrorInvalidValueInFunction;
			if (count == 2) {
				int indexOf = withinText.IndexOf(findText);
				return (indexOf != -1) ? indexOf + 1 : VariantValue.ErrorInvalidValueInFunction;
			} else {
				VariantValue startValue = arguments[2].ToNumeric(context);
				if (startValue.IsError)
					return startValue;
				int from = (int)startValue.NumericValue - 1;
				if ((from < 0) || (from >= withinText.Length))
					return VariantValue.ErrorInvalidValueInFunction;
				int indexOf = withinText.IndexOf(findText, from);
				if (indexOf == -1)
					return VariantValue.ErrorInvalidValueInFunction;
				return indexOf + 1;
			}
		}
		bool FindTextValid(string findText) {
			return (findText.Length >= 1) || (String.IsNullOrEmpty(findText) && !Object.ReferenceEquals(findText, null));
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.DoNotDereferenceEmptyValueAsZero));
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.DoNotDereferenceEmptyValueAsZero));
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.NonRequired));
			return collection;
		}
	}
	#endregion
}
