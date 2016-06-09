﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
using DevExpress.Utils;
using System.Text;
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionCode
	public class FunctionCode : WorksheetFunctionTextArgumentBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "CODE"; } }
		public override int Code { get { return 0x0079; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue value = arguments[0];
			if (value.IsError)
				return value;
			VariantValue text = GetText(context, value);
			if (text.IsError)
				return text;
			string stringValue = text.GetTextValue(context.StringTable);
			if (String.IsNullOrEmpty(stringValue))
				return VariantValue.ErrorInvalidValueInFunction;
			string firstChar = new string(stringValue[0], 1);
			Encoding encodingForCurrentWindowsCodePage = DXEncoding.Default.Clone() as Encoding;
#if !SL && !DXPORTABLE
			encodingForCurrentWindowsCodePage.EncoderFallback = EncoderFallback.ReplacementFallback;
			encodingForCurrentWindowsCodePage.DecoderFallback = DecoderFallback.ReplacementFallback;
#endif
			byte[] bytes = encodingForCurrentWindowsCodePage.GetBytes(firstChar);
			if (bytes.Length <= 0)
				return VariantValue.ErrorInvalidValueInFunction;
			return bytes[0];
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.DoNotDereferenceEmptyValueAsZero));
			return collection;
		}
	}
	#endregion
}
