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
using System.Diagnostics;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region BinaryParsedThing
	public abstract class BinaryParsedThing : ParsedThingBase {
		public virtual bool DereferenceEmptyValueAsZero { get { return true; } }
		public abstract string GetOperatorText(WorkbookDataContext context);
		public override void BuildExpressionString(Stack<int> stack, System.Text.StringBuilder builder, System.Text.StringBuilder spacesBuilder, WorkbookDataContext context) {
			Debug.Assert(stack.Count >= 2);
			builder.Insert(stack.Pop(), spacesBuilder.Append(GetOperatorText(context)).ToString());
			spacesBuilder.Remove(0, spacesBuilder.Length);
		}
		#region Evaluate
		public override void Evaluate(Stack<VariantValue> stack, WorkbookDataContext context) {
			Debug.Assert(stack.Count >= 2);
			VariantValue rightOperand = stack.Pop();
			VariantValue leftOperand = stack.Pop();
			VariantValue result;
			if (leftOperand.IsError)
				result = leftOperand;
			else if (rightOperand.IsError)
				result = rightOperand;
			else
				result = EvaluateCore(context, leftOperand, rightOperand);
			stack.Push(result);
		}
		protected VariantValue EvaluateCore(WorkbookDataContext context, VariantValue leftOperand, VariantValue rightOperand) {
			if (leftOperand.IsCellRange){
				leftOperand = context.GetArrayFromCellRange(leftOperand.CellRangeValue);
				if(leftOperand.IsError)
					return leftOperand;
			}
			else
				if (leftOperand.IsArray && leftOperand.ArrayValue.Count == 1)
					leftOperand = leftOperand.ArrayValue.GetValue(0, 0);
			if (rightOperand.IsCellRange){
				rightOperand = context.GetArrayFromCellRange(rightOperand.CellRangeValue);
				if (rightOperand.IsError)
					return rightOperand;
			}
			else
				if (rightOperand.IsArray && rightOperand.ArrayValue.Count == 1)
				rightOperand = rightOperand.ArrayValue.GetValue(0, 0);
			if (leftOperand.IsArray) {
				if (rightOperand.IsArray)
					return EvaluateArrayAndArray(context, leftOperand.ArrayValue, rightOperand.ArrayValue);
				else
					return EvaluateArrayAndValue(context, leftOperand.ArrayValue, rightOperand);
			}
			if (rightOperand.IsArray)
				return EvaluateValueAndArray(context, leftOperand, rightOperand.ArrayValue);
			return EvaluateSimpleOperands(context, leftOperand, rightOperand);
		}
		VariantValue EvaluateArrayAndValue(WorkbookDataContext context, IVariantArray array, VariantValue rightOperand) {
			BinaryParsedThingFixedRightOperandVariantArrayItemCalculator calculator;
			calculator = new BinaryParsedThingFixedRightOperandVariantArrayItemCalculator(this, context, rightOperand);
			return VariantValue.FromArray(new CalculatedVariantArray(array, calculator));
		}
		VariantValue EvaluateValueAndArray(WorkbookDataContext context, VariantValue leftOperand, IVariantArray array) {
			BinaryParsedThingFixedLeftOperandVariantArrayItemCalculator calculator;
			calculator = new BinaryParsedThingFixedLeftOperandVariantArrayItemCalculator(this, context, leftOperand);
			return VariantValue.FromArray(new CalculatedVariantArray(array, calculator));
		}
		VariantValue EvaluateArrayAndArray(WorkbookDataContext context, IVariantArray leftArray, IVariantArray rightArray) {
			BinaryParsedThingCombinedVariantArrayItemCalculator calculator;
			calculator = new BinaryParsedThingCombinedVariantArrayItemCalculator(this, context);
			return VariantValue.FromArray(new CombinedVariantArray(leftArray, rightArray, calculator));
		}
		protected internal abstract VariantValue EvaluateSimpleOperands(WorkbookDataContext context, VariantValue leftOperand, VariantValue rightOperand);
		#endregion
		#region GetInvolvedCellRanges
		public override void GetInvolvedCellRanges(Stack<CellRangeList> stack, WorkbookDataContext context) {
			Debug.Assert(stack.Count >= 2);
			CellRangeList rightList = stack.Pop();
			CellRangeList leftList = stack.Pop();
			CellRangeList result = new CellRangeList();
			result.AddRange(leftList);
			result.AddRange(rightList);
			stack.Push(result);
		}
		#endregion
		public override IParsedThing Clone() {
			return this;
		}
	}
	#endregion
	#region BinaryBooleanParsedThing (abstract class)
	public abstract class BinaryBooleanParsedThing : BinaryParsedThing {
		public override bool DereferenceEmptyValueAsZero { get { return false; } }
		public abstract ConditionPriority Priority { get; }
		protected internal override VariantValue EvaluateSimpleOperands(WorkbookDataContext context, VariantValue leftOperand, VariantValue rightOperand) {
			if (leftOperand.IsError)
				return leftOperand;
			if (rightOperand.IsError)
				return rightOperand;
			return CalculateBooleanResult(context, leftOperand, rightOperand);
		}
		protected abstract VariantValue CalculateBooleanResult(WorkbookDataContext context, VariantValue leftOperand, VariantValue rightOperand);
	}
	#endregion
	#region BinaryArithmeticParsedThing (abstract class)
	public abstract class BinaryArithmeticParsedThing : BinaryParsedThing {
		public override void Evaluate(Stack<VariantValue> stack, WorkbookDataContext context) {
			Debug.Assert(stack.Count >= 2);
			VariantValue rightOperand = stack.Pop();
			VariantValue leftOperand = stack.Pop();
			if (leftOperand.IsError) {
				stack.Push(leftOperand);
				return;
			}
			if (rightOperand.IsError) {
				if (leftOperand.IsText && String.IsNullOrEmpty(leftOperand.GetTextValue(context.StringTable))) {
					stack.Push(VariantValue.ErrorInvalidValueInFunction);
					return;
				}
			}
			stack.Push(EvaluateCore(context, leftOperand, rightOperand));
		}
		protected internal override VariantValue EvaluateSimpleOperands(WorkbookDataContext context, VariantValue leftOperand, VariantValue rightOperand) {
			if (leftOperand.IsEmpty)
				leftOperand = 0;
			if (rightOperand.IsEmpty)
				rightOperand = 0;
			VariantValue leftValue = leftOperand.ToNumeric(context);
			if (leftValue.IsError)
				return leftValue;
			if (rightOperand.IsError)
				return rightOperand;
			VariantValue rightValue = rightOperand.ToNumeric(context);
			if (rightValue.IsError)
				return rightValue;
			return GetNumericResult(leftValue.NumericValue, rightValue.NumericValue);
		}
		protected abstract VariantValue GetDefaultResult();
		protected abstract VariantValue GetNumericResult(double left, double right);
	}
	#endregion
	#region BinaryReferenceParsedThing(abstract class)
	public abstract class BinaryReferenceParsedThing : BinaryParsedThing {
		protected internal abstract VariantValue EvaluateCellRanges(WorkbookDataContext Context, CellRangeBase leftCellRange, CellRangeBase rightCellRange);
		public override void Evaluate(Stack<VariantValue> stack, WorkbookDataContext context) {
			Debug.Assert(stack.Count >= 2);
			VariantValue rightOperand = stack.Pop();
			VariantValue leftOperand = stack.Pop();
			if (leftOperand.IsError) {
				stack.Push(leftOperand);
				return;
			}
			if (!leftOperand.IsCellRange) {
				stack.Push(VariantValue.ErrorInvalidValueInFunction);
				return;
			}
			if (rightOperand.IsError) {
				stack.Push(rightOperand);
				return;
			}
			if (!rightOperand.IsCellRange) {
				stack.Push(VariantValue.ErrorInvalidValueInFunction);
				return;
			}
			CellRangeBase leftCellRange = leftOperand.CellRangeValue;
			CellRangeBase rightCellRange = rightOperand.CellRangeValue;
			if ((leftCellRange.RangeType != CellRangeType.UnionRange && leftCellRange.Worksheet == null) ||
				(rightCellRange.RangeType != CellRangeType.UnionRange && rightCellRange.Worksheet == null))
				stack.Push(VariantValue.ErrorInvalidValueInFunction);
			else
				stack.Push(EvaluateCellRanges(context, leftCellRange, rightCellRange));
		}
		protected internal override VariantValue EvaluateSimpleOperands(WorkbookDataContext context, VariantValue leftOperand, VariantValue rightOperand) {
			throw new System.ArgumentException();
		}
		#region GetInvolvedCellRanges
		public override void GetInvolvedCellRanges(Stack<CellRangeList> stack, WorkbookDataContext context) {
			Debug.Assert(stack.Count >= 2);
			CellRangeList list2 = stack.Pop();
			CellRangeList list1 = stack.Pop();
			CellRangeList result = new CellRangeList();
			if (list1.Count == 1 && list2.Count == 1) {
				VariantValue evaluationResult = EvaluateCellRanges(context, list1[0], list2[0]);
				if (evaluationResult.IsCellRange)
					result.Add(evaluationResult.CellRangeValue);
			}
			else {
				result.AddRange(list1);
				result.AddRange(list2);
			}
			stack.Push(result);
		}
		#endregion
	}
	#endregion
	#region BinaryParsedThingVariantArrayItemCalculator (abstract class)
	public abstract class BinaryParsedThingVariantArrayItemCalculator : IVariantArrayItemCalculator {
		readonly BinaryParsedThing expression;
		readonly WorkbookDataContext context;
		protected BinaryParsedThingVariantArrayItemCalculator(BinaryParsedThing expression, WorkbookDataContext context) {
			Guard.ArgumentNotNull(expression, "expression");
			Guard.ArgumentNotNull(context, "context");
			this.expression = expression;
			this.context = CreateContextSnapshot(context);
		}
		public BinaryParsedThing Expression { get { return expression; } }
		public WorkbookDataContext Context { get { return context; } }
		WorkbookDataContext CreateContextSnapshot(WorkbookDataContext context) {
			return context.CreateSnapshot();
		}
		public abstract VariantValue Calculate(VariantValue value);
	}
	#endregion
	#region BinaryParsedThingFixedRightOperandVariantArrayItemCalculator
	public class BinaryParsedThingFixedRightOperandVariantArrayItemCalculator : BinaryParsedThingVariantArrayItemCalculator {
		readonly VariantValue rightOperand;
		public BinaryParsedThingFixedRightOperandVariantArrayItemCalculator(BinaryParsedThing expression, WorkbookDataContext context, VariantValue rightOperand)
			: base(expression, context) {
			this.rightOperand = rightOperand;
		}
		public override VariantValue Calculate(VariantValue value) {
			return Expression.EvaluateSimpleOperands(Context, value, rightOperand);
		}
	}
	#endregion
	#region BinaryParsedThingFixedLeftOperandVariantArrayItemCalculator
	public class BinaryParsedThingFixedLeftOperandVariantArrayItemCalculator : BinaryParsedThingVariantArrayItemCalculator {
		readonly VariantValue leftOperand;
		public BinaryParsedThingFixedLeftOperandVariantArrayItemCalculator(BinaryParsedThing expression, WorkbookDataContext context, VariantValue leftOperand)
			: base(expression, context) {
			this.leftOperand = leftOperand;
		}
		public override VariantValue Calculate(VariantValue value) {
			return Expression.EvaluateSimpleOperands(Context, leftOperand, value);
		}
	}
	#endregion
	#region BinaryParsedThingCombinedVariantArrayItemCalculator
	public class BinaryParsedThingCombinedVariantArrayItemCalculator : ICombinedVariantArrayItemCalculator {
		readonly BinaryParsedThing expression;
		readonly WorkbookDataContext context;
		public BinaryParsedThingCombinedVariantArrayItemCalculator(BinaryParsedThing expression, WorkbookDataContext context) {
			Guard.ArgumentNotNull(expression, "expression");
			Guard.ArgumentNotNull(context, "context");
			this.expression = expression;
			this.context = CreateContextSnapshot(context);
		}
		public BinaryParsedThing Expression { get { return expression; } }
		public WorkbookDataContext Context { get { return context; } }
		WorkbookDataContext CreateContextSnapshot(WorkbookDataContext context) {
			return context.CreateSnapshot();
		}
		public VariantValue Calculate(VariantValue firstValue, VariantValue secondValue) {
			return Expression.EvaluateSimpleOperands(Context, firstValue, secondValue);
		}
	}
	#endregion
}
