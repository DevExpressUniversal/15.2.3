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
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Fields {
	public class ArgumentList : List<FormulaNodeBase> {
	}
	public class ExpressionFieldBase {
		ExpressionTree expressionTree;
		public ExpressionFieldBase() {
			this.expressionTree = new ExpressionTree();
		}
		public ExpressionTree ExpressionTree { get { return expressionTree; } set { expressionTree = value; } }
	}
	public class SimpleRichEditField : ExpressionFieldBase {
		string token;
		public string Token { get { return token; } set { token = value; } }
	}
	public class ExpressionTree {
		FormulaNodeBase root;
		public FormulaNodeBase Root { get { return root; } set { root = value; } }
	}
	public abstract class FormulaNodeBase {
		public static FormulaNodeBase GetFormulaNode(FormulaNodeBase left, FormulaNodeBase right, string op) {
			if (left == null)
				return right;
			if (right == null)
				return left;
			return new FormulaNode(left, right, op);
		}
		public static FormulaNodeBase GetFunctionCallNode(string functionName, ArgumentList arguments) {
			return new FunctionCallNode(functionName, arguments.ToArray());
		}
		public abstract bool BeginDataIteration(PieceTable pieceTable, Field field);
		public abstract bool MoveNext();
		public abstract void EndDataIteration();
		public abstract double GetValue();		
	}
	public class UnaryMinus : FormulaNodeBase {
		readonly FormulaNodeBase subNode;
		public UnaryMinus(FormulaNodeBase subNode) {
			this.subNode = subNode;
		}
		public override string ToString() {
			return String.Format("(-{0})", subNode);
		}
		public override double GetValue() {
			return -subNode.GetValue();
		}
		public override bool BeginDataIteration(PieceTable pieceTable, Field field) {
			return subNode.BeginDataIteration(pieceTable, field);
		}
		public override bool MoveNext() {
			return subNode.MoveNext();
		}
		public override void EndDataIteration() {
			subNode.EndDataIteration();
		}
	}
	public class FunctionCallNode : FormulaNodeBase {
		FormulaNodeBase[] arguments;
		string functionName;
		IFunctionCalculator calculator;
		public FunctionCallNode(string functionName, FormulaNodeBase[] arguments) {
			this.functionName = functionName;
			this.arguments = arguments;
		}
		public override double GetValue() {
			if (calculator != null)
				return calculator.CalculateValue(arguments);
			else
				return 0;
		}
		public override bool BeginDataIteration(PieceTable pieceTable, Field field) {
			IFieldFunctionCalculatorFactory service = pieceTable.DocumentModel.GetService<IFieldFunctionCalculatorFactory>();
			if (service != null)
				calculator = service.GetCalculator(functionName, arguments);
			else
				return true;
			return calculator.BeginFunctionResultIteration(pieceTable, field, arguments);
		}
		public override bool MoveNext() {
			if (calculator != null)
				return calculator.MoveNextFunctionResult();
			else
				return false;
		}
		public override void EndDataIteration() {
			if(calculator != null)
				calculator.EndFunctionResultIteration();
			calculator = null;
		}
	}
	public class Percent : FormulaNodeBase {
		readonly FormulaNodeBase subNode;
		public Percent(FormulaNodeBase subNode) {
			this.subNode = subNode;
		}
		public override string ToString() {
			return String.Format("({0}%)", subNode);
		}
		public override double GetValue() {
			return subNode.GetValue() / 100f;
		}
		public override bool BeginDataIteration(PieceTable pieceTable, Field field) {
			return subNode.BeginDataIteration(pieceTable, field);
		}
		public override bool MoveNext() {
			return subNode.MoveNext();
		}
		public override void EndDataIteration() {
			subNode.EndDataIteration();
		}
	}
	public class FormulaNode : FormulaNodeBase {
		readonly FormulaNodeBase left;
		readonly FormulaNodeBase right;
		readonly string op;
		public FormulaNode(FormulaNodeBase left, FormulaNodeBase right, string op) {
			this.left = left;
			this.right = right;
			this.op = op;
		}
		public override string ToString() {
			return String.Format("({0}{1}{2})", left, op, right);
		}
		public override double GetValue() {
			double leftValue = left.GetValue();
			double rightValue = right.GetValue();
			switch (op) {
				case "+":
					return leftValue + rightValue;
				case "-":
					return leftValue - rightValue;
				case "*":
					return leftValue * rightValue;
				case "/":
					return leftValue / rightValue;
				case "^":
					return Math.Pow(leftValue, rightValue);
				case "=":
					return leftValue == rightValue ? 1 : 0;
				case "<":
					return leftValue < rightValue ? 1 : 0;
				case "<=":
					return leftValue <= rightValue ? 1 : 0;
				case ">":
					return leftValue > rightValue ? 1 : 0;
				case ">=":
					return leftValue >= rightValue ? 1 : 0;
				case "<>":
					return leftValue != rightValue ? 1 : 0;
				default:
					Debug.Assert(false);
					return 0;
			}
		}
		public override bool BeginDataIteration(PieceTable pieceTable, Field field) {
			return left.BeginDataIteration(pieceTable, field) & right.BeginDataIteration(pieceTable, field);
		}
		public override bool MoveNext() {
			return left.MoveNext() | right.MoveNext();
		}
		public override void EndDataIteration() {
			left.EndDataIteration();
			right.EndDataIteration();
		}
	}
	public class PrimitiveFormulaNode : FormulaNodeBase {
		string number;
		public PrimitiveFormulaNode(string number) {
			this.number = number;
		}
		public override string ToString() {
			return number;
		}
		public override double GetValue() {
			return Double.Parse(number);
		}
		public override bool BeginDataIteration(PieceTable pieceTable, Field field) {
			return true;
		}
		public override bool MoveNext() {
			return false;
		}
		public override void EndDataIteration() {			
		}
	}
	public class ReferenceFormulaNode : FormulaNodeBase {
		IEnumerator<double> valuesEnumerator;		
		string reference;
		public ReferenceFormulaNode(string reference) {
			this.reference = reference;
		}
		public override string ToString() {
			return reference;
		}
		public override double GetValue() {
			if (valuesEnumerator == null)
				return 0;
			return valuesEnumerator.Current;
		}
		public override bool BeginDataIteration(PieceTable pieceTable, Field field) {
			IFieldDataService service = pieceTable.DocumentModel.GetService<IFieldDataService>();
			if (service == null)
				return false;
			valuesEnumerator = service.GetReferenceValuesEnumerator(pieceTable, field, reference);
			if (valuesEnumerator == null)
				return false;
			return valuesEnumerator.MoveNext();
		}
		public override bool MoveNext() {
			return valuesEnumerator.MoveNext();
		}
		public override void EndDataIteration() {
			valuesEnumerator.Dispose();				
			valuesEnumerator = null;
		}
	}
}
