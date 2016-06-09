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
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionModeMult
	public class FunctionModeMult : WorksheetGenericFunctionBase {
		#region Static Members
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Array));
			collection.Add(new FunctionParameter(OperandDataType.Array, FunctionParameterOption.NonRequiredUnlimited));
			return collection;
		}
		#endregion
		#region Properties
		public override string Name { get { return "MODE.MULT"; } }
		public override int Code { get { return 0x4001; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Array; } }
		#endregion
		protected override FunctionResult CreateInitialFunctionResult(WorkbookDataContext context) {
			return new FunctionModeMultResult(context);
		}
	}
	#endregion
	#region FunctionModeMultResult
	public class FunctionModeMultResult : FunctionModeResult {
		public FunctionModeMultResult(WorkbookDataContext context)
			: base(context) {
		}
		protected override VariantValue ModeCore(int maxOccurenceCount, double maxOccurenceValue) {
			VariantArray resultArray = new VariantArray();
			resultArray.Values = new List<VariantValue>();
			foreach (KeyValuePair<double, int> pair in Numbers) {
				if (pair.Value == maxOccurenceCount)
					resultArray.Values.Add(pair.Key);
			}
			resultArray.Height = (int)resultArray.Count;
			resultArray.Width = 1;
			VariantValue resValue = new VariantValue();
			resValue.ArrayValue = resultArray;
			return resValue;
		}
	}
	#endregion
}
