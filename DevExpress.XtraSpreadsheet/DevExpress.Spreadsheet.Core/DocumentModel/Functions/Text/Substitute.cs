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
	#region FunctionSubstitute
	public class FunctionSubstitute : FunctionLeft {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "SUBSTITUTE"; } }
		public override int Code { get { return 0x0078; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue text = arguments[0].ToText(context);
			if (text.IsError)
				return text;
			VariantValue oldText = arguments[1].ToText(context);
			if (oldText.IsError)
				return oldText;
			VariantValue newText = arguments[2].ToText(context);
			if (newText.IsError)
				return newText;
			int instanceNum = -1;
			if (arguments.Count == 4) {
				VariantValue value = GetNumericArgument(arguments[3], context);
				if (value.IsError)
					return value;
				if (value.NumericValue < 1)
					return VariantValue.ErrorInvalidValueInFunction;
				instanceNum = (int)value.NumericValue;
			}
			return GetStringResult(text.GetTextValue(context.StringTable), oldText.GetTextValue(context.StringTable), newText.GetTextValue(context.StringTable), instanceNum);
		}
		VariantValue GetStringResult(string text, string oldText, string newText, int instanceNum) {
			string result = text;
			if(String.IsNullOrEmpty(text) || String.IsNullOrEmpty(oldText))
				return result;
			if (instanceNum == -1)
				return result.Replace(oldText, newText);
			int index = GetIndexReplacedString(text, oldText, instanceNum);
			if (index == -1)
				return result;
			result = result.Remove(index, oldText.Length);
			result = result.Insert(index, newText);
			return result;
		}
		int GetIndexReplacedString(string text, string oldText, int instanceNum) {
			string targetText = text;
			int result = 0;
			for (int i = 1; i <= instanceNum; i++) {
				int index = targetText.IndexOf(oldText);
				if (index == -1) 
					return index;
				index += oldText.Length;
				targetText = targetText.Remove(0, index);
				result += index; 
			}
			return result - oldText.Length;
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.DoNotDereferenceEmptyValueAsZero));
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.DoNotDereferenceEmptyValueAsZero));
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.DoNotDereferenceEmptyValueAsZero));
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.NonRequired));
			return collection;
		}
	}
	#endregion   
}
