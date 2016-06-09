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
using DevExpress.Data.Filtering;
namespace DevExpress.PivotGrid.CriteriaVisitors {
	public abstract class CriteriaPatcherBase : IClientCriteriaVisitor<CriteriaOperator> {
		protected CriteriaPatcherBase() { }
		public CriteriaOperator Process(CriteriaOperator op) {
			return ReferenceEquals(op, null) ? null : op.Accept(this);
		}
		protected IEnumerable<CriteriaOperator> ProcessAny(IEnumerable<CriteriaOperator> operands) {
			List<CriteriaOperator> ops = new List<CriteriaOperator>();
			foreach(CriteriaOperator operand in operands) {
				CriteriaOperator op = Process(operand);
				if(!ReferenceEquals(op, null))
					ops.Add(op);
			}
			return ops.Count > 0 ? ops : null;
		}
		protected IEnumerable<CriteriaOperator> ProcessEvery(IEnumerable<CriteriaOperator> operands) {
			List<CriteriaOperator> ops = new List<CriteriaOperator>();
			bool haveItems = false;
			foreach(CriteriaOperator operand in operands) {
				haveItems = true;
				CriteriaOperator op = Process(operand);
				if(ReferenceEquals(op, null))
					return null;
				ops.Add(op);
			}
			return ops.Count > 0 || !haveItems ? ops : null;
		}
		CriteriaOperator ICriteriaVisitor<CriteriaOperator>.Visit(BetweenOperator theOperator) {
			CriteriaOperator test = Process(theOperator.TestExpression);
			if(ReferenceEquals(test, null))
				return null;
			CriteriaOperator begin = Process(theOperator.BeginExpression);
			if(ReferenceEquals(begin, null))
				return null;
			CriteriaOperator end = Process(theOperator.EndExpression);
			if(ReferenceEquals(end, null))
				return null;
			return new BetweenOperator(test, begin, end);
		}
		public virtual CriteriaOperator Visit(BinaryOperator theOperator) {
			CriteriaOperator left = Process(theOperator.LeftOperand);
			if(ReferenceEquals(left, null))
				return null;
			CriteriaOperator right = Process(theOperator.RightOperand);
			if(ReferenceEquals(right, null))
				return null;
			return new BinaryOperator(left, right, theOperator.OperatorType);
		}
		public virtual CriteriaOperator Visit(FunctionOperator theOperator) {
			IEnumerable<CriteriaOperator> operands = ProcessEvery(theOperator.Operands);
			return operands != null ? new FunctionOperator(theOperator.OperatorType, operands) : null;
		}
		CriteriaOperator ICriteriaVisitor<CriteriaOperator>.Visit(GroupOperator theOperator) {
			IEnumerable<CriteriaOperator> operands = ProcessAny(theOperator.Operands);
			return operands != null ? new GroupOperator(theOperator.OperatorType, operands) : null;
		}
		public virtual CriteriaOperator Visit(InOperator theOperator) {
			CriteriaOperator left = Process(theOperator.LeftOperand);
			if(ReferenceEquals(left, null))
				return null;
			IEnumerable<CriteriaOperator> operands = ProcessAny(theOperator.Operands);
			if(operands == null)
				return null;
			return new InOperator(left, operands);
		}
		public virtual CriteriaOperator Visit(OperandValue theOperand) {
			return theOperand;
		}
		CriteriaOperator ICriteriaVisitor<CriteriaOperator>.Visit(UnaryOperator theOperator) {
			CriteriaOperator operand = Process(theOperator.Operand);
			if(ReferenceEquals(operand, null))
				return null;
			return new UnaryOperator(theOperator.OperatorType, operand);
		}
		public virtual CriteriaOperator Visit(AggregateOperand theOperand) {
			return new AggregateOperand(theOperand.CollectionProperty, Process(theOperand.AggregatedExpression), theOperand.AggregateType, Process(theOperand.Condition));
		}
		CriteriaOperator IClientCriteriaVisitor<CriteriaOperator>.Visit(JoinOperand theOperand) {
			return theOperand;
		}
		public virtual CriteriaOperator Visit(OperandProperty theOperand) {
			return theOperand;
		}
	}
}
