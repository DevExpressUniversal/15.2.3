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
using System.Text.RegularExpressions;
using DevExpress.Data;
using DevExpress.Data.Filtering;
namespace DevExpress.DataAccess.Native {
	public class ParametersRenamingHelper : IClientCriteriaVisitor {
		readonly IDictionary<string, string> renamingMap;
		public void Process(IEnumerable<IParameter> parameters) {
			foreach(IParameter parameter in parameters) {
				if(parameter.Type != typeof(Expression))
					continue;
				Expression expression = parameter.Value as Expression;
				if(expression == null)
					continue;
				CriteriaOperator criteriaOperator;
				try {
					criteriaOperator = CriteriaOperator.Parse(expression.ExpressionString);
				} catch {
					continue;
				}
				Process(criteriaOperator);
				expression.ExpressionString = criteriaOperator.ToString();
			}
		}
		public void Process(CriteriaOperator op) {
			if(!ReferenceEquals(op, null))
				op.Accept(this);
		}
		public ParametersRenamingHelper(IDictionary<string, string> renamingMap) {
			this.renamingMap = renamingMap;
		}
		public void Visit(FunctionOperator theOperator) {
			foreach(CriteriaOperator operand in theOperator.Operands)
				Process(operand);
		}
		public void Visit(BinaryOperator theOperator) {
			Process(theOperator.LeftOperand);
			Process(theOperator.RightOperand);
		}
		public void Visit(UnaryOperator theOperator) {
			Process(theOperator.Operand);
		}
		public void Visit(BetweenOperator theOperator) {
			Process(theOperator.TestExpression);
			Process(theOperator.BeginExpression);
			Process(theOperator.EndExpression);
		}
		public void Visit(InOperator theOperator) {
			Process(theOperator.LeftOperand);
			foreach(CriteriaOperator operand in theOperator.Operands)
				Process(operand);
		}
		public void Visit(GroupOperator theOperator) {
			foreach(CriteriaOperator operand in theOperator.Operands)
				Process(operand);
		}
		public void Visit(AggregateOperand theOperand) {
		}
		public void Visit(JoinOperand theOperand) {
		}
		public void Visit(OperandValue theOperand) {
		}
		public void Visit(OperandProperty theOperand) {
			string name = theOperand.PropertyName;
			Regex newReg = new Regex(@"\AParameters\.(?<name>.+)\z", RegexOptions.None);
			Match match = newReg.Match(name);
			if(!match.Success)
				return;
			string oldName = match.Groups["name"].Value;
			string newName;
			if(!this.renamingMap.TryGetValue(oldName, out newName))
				return;
			theOperand.PropertyName = string.Format("Parameters.{0}", newName);
		}
	}
}
