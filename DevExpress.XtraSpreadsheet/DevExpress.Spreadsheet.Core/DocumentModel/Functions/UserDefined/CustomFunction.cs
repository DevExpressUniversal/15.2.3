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
using APIFunctions = DevExpress.Spreadsheet.Functions;
namespace DevExpress.XtraSpreadsheet.Model {
#if DXPORTABLE
	public
#else
	internal
#endif
		class CustomFunction : WorksheetFunctionBase {
		#region Fields
		readonly APIFunctions.ICustomFunction nativeCustomFunction;
		readonly FunctionParameterCollection parameters;
		readonly string name;
		readonly bool isVolatile;
		readonly OperandDataType returnDataType;
		#endregion
		public CustomFunction(APIFunctions.ICustomFunction nativeCustomFunction, FunctionParameterCollection parameters) {
			this.name = nativeCustomFunction.Name;
			this.isVolatile = nativeCustomFunction.Volatile;
			this.returnDataType = (OperandDataType)nativeCustomFunction.ReturnType;
			this.parameters = parameters;
			this.nativeCustomFunction = nativeCustomFunction;
		}
		#region Properties
		public APIFunctions.ICustomFunction NativeCustomFunction { get { return nativeCustomFunction; } }
		public override string Name { get { return name; } }
		public override int Code { get { return 0x00FF; } }
		public override OperandDataType ReturnDataType { get { return returnDataType; ; } }
		public override FunctionParameterCollection Parameters { get { return parameters; } }
		public override bool IsVolatile { get { return isVolatile; } }
		#endregion
		protected internal static bool ValidateName(string name) {
			return WorkbookDataContext.IsValidIndentifier(name);
		}
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue result = context.Workbook.InternalAPI.RaiseCustomFunctionEvaluation(name, arguments);
			if (result.IsEmpty)
				result = 0;
			return result;
		}
	}
}
