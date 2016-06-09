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
using System.Text;
using DevExpress.Office.NumberConverters;
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionBathText
	public class FunctionBahtText : WorksheetFunctionSingleDoubleArgumentBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "BAHTTEXT"; } }
		public override int Code { get { return 0x0170; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		protected override VariantValue GetNumericResult(double argument) {
			int integerValue = Math.Abs((int)argument);
			double floatValue = Math.Round((Math.Abs(argument) - integerValue), 2);
			DescriptiveCardinalThaiNumberConverter converter = new DescriptiveCardinalThaiNumberConverter();
			string result = "";
			result += argument < 0 ? "ลบ" : "";
			if (integerValue != 0 && floatValue == 0)
				result += converter.ConvertNumberCore(integerValue) + "บาทถ้วน";
			else if (integerValue != 0 && floatValue != 0)
				result += converter.ConvertNumberCore(integerValue) + "บาท" + converter.ConvertNumberCore(Convert.ToInt64(floatValue * 100)) + "สตางค์";
			else if (integerValue == 0 && floatValue != 0)
				result += converter.ConvertNumberCore(Convert.ToInt64(floatValue * 100)) + "สตางค์";
			else
				result += "ศูนย์บาทถ้วน";
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
