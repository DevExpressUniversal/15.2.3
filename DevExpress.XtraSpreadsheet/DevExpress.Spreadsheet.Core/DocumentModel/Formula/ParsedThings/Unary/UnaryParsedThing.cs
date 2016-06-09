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
using System.Diagnostics;
namespace DevExpress.XtraSpreadsheet.Model {
	public abstract class UnaryParsedThing : ParsedThingBase {
		public override void BuildExpressionString(Stack<int> stack, System.Text.StringBuilder builder, System.Text.StringBuilder spacesBuilder, WorkbookDataContext context) {
			Debug.Assert(stack.Count >= 1);
			builder.Insert(stack.Peek(), spacesBuilder.Append(GetOperatorText(context)).ToString());
			spacesBuilder.Remove(0, spacesBuilder.Length);
		}
		public abstract string GetOperatorText(WorkbookDataContext context);
		protected abstract double GetNumericResult(double operand);
		#region Evaluate
		public override void Evaluate(Stack<VariantValue> stack, WorkbookDataContext context) {
			Debug.Assert(stack.Count >= 1);
			VariantValue value = stack.Pop();
			if(value.IsCellRange)
				value = context.GetArrayFromCellRange(value.CellRangeValue);
			if (value.IsArray)
				stack.Push(EvaluateArrayCore(context, value.ArrayValue));
			else
				stack.Push(EvaluateCore(context, value));
		}
		VariantValue EvaluateArrayCore(WorkbookDataContext context, IVariantArray array) {
			VariantArray result = VariantArray.Create(array.Width, array.Height);
			for (int y = 0; y < array.Height; y++)
				for (int x = 0; x < array.Width; x++)
					result.SetValue(y, x, EvaluateCore(context, array.GetValue(y, x)));
			return VariantValue.FromArray(result);
		}
		protected virtual VariantValue EvaluateCore(WorkbookDataContext context, VariantValue value) {
			VariantValue operand = value;
			if (operand.IsEmpty)
				operand = 0;
			if (operand.IsError)
				return operand;
			VariantValue numericValue = operand.ToNumeric(context);
			if (numericValue.IsError)
				return numericValue;
			return GetNumericResult(numericValue.NumericValue);
		}
		#endregion
		public override IParsedThing Clone() {
			return this;
		}
	}
}
