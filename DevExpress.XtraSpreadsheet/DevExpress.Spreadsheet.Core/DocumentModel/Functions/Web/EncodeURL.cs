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
using System.Text;
#if SL
using System.Net;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionEncodeURL
	public class FunctionEncodeURL : WorksheetFunctionSingleArgumentBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "ENCODEURL"; } }
		public override int Code { get { return 0x4044; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override VariantValue EvaluateArgument(VariantValue argument, WorkbookDataContext context) {
			VariantValue text = argument.ToText(context);
			if (argument.IsError)
				return argument;
			return EncodeURL(text.InlineTextValue);
		}
		VariantValue EncodeURL(string text) {
			string result = string.Empty;
			Encoding encoding = DXEncoding.UTF8NoByteOrderMarks;
			for (int i = 0; i < text.Length; ++i) {
				char currentSymbol = text[i];
				if (currentSymbol >= 'A' && currentSymbol <= 'Z' || currentSymbol >= '0' && currentSymbol <= '9' ||
					currentSymbol >= 'a' && currentSymbol <= 'z' || currentSymbol == '-' || currentSymbol == '_' || currentSymbol == '.') {
					result += currentSymbol;
					continue;
				}
				byte[] bytes;
				if (char.IsSurrogate(currentSymbol)) {
					if (text.Length == i + 1 || !char.IsSurrogatePair(currentSymbol, text[i + 1]))
						return VariantValue.ErrorInvalidValueInFunction;
					bytes = encoding.GetBytes(new char[] { currentSymbol, text[i + 1] });
					++i;
				}
				else
					bytes = encoding.GetBytes(new char[] { currentSymbol });
				for (int j = 0; j < bytes.Length; ++j)
					result += '%' + bytes[j].ToString("X");
			}
			return result;
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value));
			return collection;
		}
	}
	#endregion
}
