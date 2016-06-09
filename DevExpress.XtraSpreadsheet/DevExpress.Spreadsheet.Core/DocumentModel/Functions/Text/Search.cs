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
using DevExpress.XtraSpreadsheet.Utils;
using System.Text.RegularExpressions;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionSearch
	public class FunctionSearch : FunctionLeft {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "SEARCH"; } }
		public override int Code { get { return 0x0052; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue textFindArgument = arguments[0].ToText(context);
			if (textFindArgument.IsError)
				return textFindArgument;
			VariantValue withinTextArgument = arguments[1].ToText(context);
			if (withinTextArgument.IsError)
				return withinTextArgument;
			VariantValue startIndexArgument = 1;
			if (arguments.Count == 3) {
				startIndexArgument = GetNumericArgument(arguments[2], context);
				if (startIndexArgument.IsError)
					return startIndexArgument;
			}
			return GetNumericResult(textFindArgument.GetTextValue(context.StringTable), withinTextArgument.GetTextValue(context.StringTable), (int)startIndexArgument.NumericValue);
		}
		VariantValue GetNumericResult(string textFind, string withinText, int startIndex) {
			startIndex--;
			if (startIndex < 0 || String.IsNullOrEmpty(withinText) || textFind.Length + startIndex > withinText.Length)
				return VariantValue.ErrorInvalidValueInFunction;
			int result = -1;
			if (WildcardComparer.IsWildcard(textFind)) {
				withinText = withinText.Remove(0, startIndex);
				Match match;
				if (WildcardComparer.TryGetMatch(textFind, withinText, out match))
					result = match.Index + startIndex;
			}
			else
				result = withinText.IndexOfInvariantCultureIgnoreCase(textFind, startIndex);
			if (result == -1)
				return VariantValue.ErrorInvalidValueInFunction;
			return result + 1;
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
