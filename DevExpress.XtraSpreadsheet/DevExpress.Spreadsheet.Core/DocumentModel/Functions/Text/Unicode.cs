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

using DevExpress.Utils;
using System;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionUnicode
	public class FunctionUnicode : WorksheetFunctionBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "UNICODE"; } }
		public override int Code { get { return 0x4039; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue text = arguments[0].ToText(context);
			if (text.IsError)
				return text;
			string value = text.InlineTextValue;
			if (string.IsNullOrEmpty(value))
				return VariantValue.ErrorInvalidValueInFunction;
			char symbol = value[0];
			if (IsHighSurrogate(symbol)) {
				if (value.Length == 1)
					return VariantValue.ErrorInvalidValueInFunction;
				if (!IsLowSurrogate(value[1]))
					return 1;
			}
			int lowSurrogateCorrection = 0;
			System.Text.Encoding encoding = DXEncoding.GetEncoding(0x2ee0); 
			byte[] bytes;
			if (IsHighSurrogate(symbol))
				bytes = encoding.GetBytes(new char[] { symbol, value[1] });
			else {
				if (IsLowSurrogate(symbol)) {
					lowSurrogateCorrection = 0xD000;
					symbol = (char)((int)symbol - lowSurrogateCorrection);
				}
				bytes = encoding.GetBytes(new char[] { symbol });
			}
			return BitConverter.ToInt32(bytes, 0) + lowSurrogateCorrection;
		}
		bool IsHighSurrogate(char symbol) {
#if SL
			int code = (int)symbol;
			if (code >= 0xD800 && code <= 0xDB99)
				return true;
			return false;
#else
			return char.IsHighSurrogate(symbol);
#endif
		}
		bool IsLowSurrogate(char symbol) {
#if SL
			int code = (int)symbol;
			if (code >= 0xDC00 && code <= 0xDFFF)
				return true;
			return false;
#else
			return char.IsLowSurrogate(symbol);
#endif
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value));
			return collection;
		}
	}
	#endregion
}
