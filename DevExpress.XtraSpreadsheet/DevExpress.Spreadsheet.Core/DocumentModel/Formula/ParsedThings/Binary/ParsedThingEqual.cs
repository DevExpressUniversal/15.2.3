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
using System.Diagnostics;
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	public class ParsedThingEqual : BinaryBooleanParsedThing {
		#region Fields
		static ParsedThingEqual instance = new ParsedThingEqual();
		#endregion
		protected ParsedThingEqual() {
		}
		#region Properties
		public static ParsedThingEqual Instance {
			get {
				return instance;
			}
		}
		public override ConditionPriority Priority { get { return ConditionPriority.Medium; } }
		#endregion
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		public override string GetOperatorText(WorkbookDataContext context) {
			return "=";
		}
		protected override VariantValue CalculateBooleanResult(WorkbookDataContext context, VariantValue leftOperand, VariantValue rightOperand) {
			return context.AreEqual(leftOperand, rightOperand);
		}
	}
	#region ParsedThingEqualWithStringWildcards
	public class ParsedThingEqualWithStringWildcards : ParsedThingEqual, IHeplerParsedThing {
		bool cacheRightOperandValue;
		VariantValue numericRightOperandValue;
		public ParsedThingEqualWithStringWildcards(bool cacheRightOperandValue)
			: base() {
			this.cacheRightOperandValue = cacheRightOperandValue;
		}
		public override void Evaluate(System.Collections.Generic.Stack<VariantValue> stack, WorkbookDataContext context) {
			System.Diagnostics.Debug.Assert(stack.Count >= 2);
			VariantValue rightOperand = stack.Pop();
			VariantValue leftOperand = stack.Pop();
			stack.Push(EvaluateCore(context, leftOperand, rightOperand));
		}
		protected internal override VariantValue EvaluateSimpleOperands(WorkbookDataContext context, VariantValue leftOperand, VariantValue rightOperand) {
			if (leftOperand.IsError) {
				if (leftOperand == rightOperand)
					return true;
				return leftOperand;
			}
			if (leftOperand.IsText && rightOperand.IsText) {
				string right = rightOperand.GetTextValue(context.StringTable);
				if (WildcardComparer.IsWildcard(right)) {
					string text = leftOperand.GetTextValue(context.StringTable);
					return WildcardComparer.Match(right, text);
				}
			}
			else {
				if (cacheRightOperandValue) {
					if (numericRightOperandValue.IsEmpty) {
						if (rightOperand.IsText && !String.IsNullOrEmpty(rightOperand.GetTextValue(context.StringTable)))
							rightOperand = rightOperand.ToNumeric(context);
						numericRightOperandValue = rightOperand; 
					}
					else
						rightOperand = numericRightOperandValue; 
				}
				else {
					if (rightOperand.IsText && !String.IsNullOrEmpty(rightOperand.GetTextValue(context.StringTable)))
						rightOperand = rightOperand.ToNumeric(context);
				}
				if (rightOperand.IsError)
					return rightOperand;
				if (leftOperand.IsText && !String.IsNullOrEmpty(leftOperand.GetTextValue(context.StringTable)))
					leftOperand = leftOperand.ToNumeric(context);
				if (leftOperand.IsError)
					return leftOperand;
			}
			if (leftOperand.IsEmpty && !rightOperand.IsText)
				return false;
			return base.EvaluateSimpleOperands(context, leftOperand, rightOperand);
		}
	}
	#endregion
	#region ImplicitParsedThingEqual
	public class ImplicitParsedThingEqual : BinaryBooleanParsedThing, IHeplerParsedThing {
		public ImplicitParsedThingEqual()
			: base() {
		}
		public override ConditionPriority Priority { get { return ConditionPriority.Medium; } }
		public override void Evaluate(System.Collections.Generic.Stack<VariantValue> stack, WorkbookDataContext context) {
			Debug.Assert(stack.Count >= 2);
			VariantValue rightOperand = stack.Pop();
			VariantValue leftOperand = stack.Pop();
			stack.Push(EvaluateCore(context, leftOperand, rightOperand));
		}
		protected internal override VariantValue EvaluateSimpleOperands(WorkbookDataContext context, VariantValue leftOperand, VariantValue rightOperand) {
			return leftOperand.IsEqual(rightOperand, StringComparison.CurrentCultureIgnoreCase, context.StringTable);
		}
		protected override VariantValue CalculateBooleanResult(WorkbookDataContext context, VariantValue leftOperand, VariantValue rightOperand) {
			return leftOperand.IsEqual(rightOperand, StringComparison.CurrentCultureIgnoreCase, context.StringTable);
		}
		public override string GetOperatorText(WorkbookDataContext context) {
			return "=";
		}
		public override void Visit(IParsedThingVisitor visitor) {
		}
	}
	#endregion
}
