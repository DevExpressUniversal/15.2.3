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
	#region FunctionIfError
	public class FunctionIfError : FunctionIfBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "IFERROR"; } }
		public override int Code { get { return 0x01E0; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value | OperandDataType.Array; } }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue value = arguments[0];
			if (value.IsArray)
				return EvaluateArrayValue(value.ArrayValue, arguments[1], context);
			return EvaluateSingleValue(value, arguments[1], context);
		}
		VariantValue EvaluateArrayValue(IVariantArray array, VariantValue valueIfError, WorkbookDataContext context) {
			int width = array.Width;
			int height = array.Height;
			width = Math.Max(width, GetArrayWidth(valueIfError));
			height = Math.Max(height, GetArrayHeight(valueIfError));
			VariantArray result = VariantArray.Create(width, height);
			for (int i = 0; i < height; i++)
				for (int j = 0; j < width; j++) {
					VariantValue conditionValue = array.GetValue(i, j);
					if (!conditionValue.IsError)
						result.SetValue(i, j, conditionValue);
					else
						result.SetValue(i, j, PrepareArrayElement(valueIfError, i, j));
				}
			return VariantValue.FromArray(result);
		}
		protected VariantValue EvaluateSingleValue(VariantValue conditionValue, VariantValue valueIfError, WorkbookDataContext context) {
			if (conditionValue == VariantValue.ErrorGettingData)
				return conditionValue;
			if (conditionValue.IsError)
				return valueIfError;
			return conditionValue;
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Array));
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference | OperandDataType.Array));
			return collection;
		}
	}
	#endregion
}
