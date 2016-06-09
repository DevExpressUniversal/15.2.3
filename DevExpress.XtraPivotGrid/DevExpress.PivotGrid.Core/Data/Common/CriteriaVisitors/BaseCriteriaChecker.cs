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
using DevExpress.XtraPivotGrid;
namespace DevExpress.PivotGrid.CriteriaVisitors {
	public abstract class BaseCriteriaChecker : IClientCriteriaVisitor<bool> {
		public static CriteriaOperator GetCheckingForNull(CriteriaOperator criteriaOperator) {
			UnaryOperator op0 = criteriaOperator as UnaryOperator;
			if(ReferenceEquals(null, op0) || op0.OperatorType != UnaryOperatorType.Not)
				return null;
			FunctionOperator op1 = op0.Operand as FunctionOperator;
			if(!ReferenceEquals(null, op1)) {
				return op1.OperatorType == FunctionOperatorType.IsNull ? GetOperandValue(op1.Operands) : null;
			}
			UnaryOperator op2 = op0.Operand as UnaryOperator;
			if(ReferenceEquals(null, op2) || op2.OperatorType != UnaryOperatorType.IsNull)
				return null;
			return GetOperandValue(op2.Operand);
		}
		static CriteriaOperator GetOperandValue(CriteriaOperator criteriaOperator) {
			return criteriaOperator as OperandProperty;
		}
		static CriteriaOperator GetOperandValue(CriteriaOperatorCollection criteriaOperatorCollection) {
			if(criteriaOperatorCollection.Count != 1)
				return null;
			return GetOperandValue(criteriaOperatorCollection[0]);
		}
		public static bool Check<T>(CriteriaOperator criteria) where T : BaseCriteriaChecker, new() {
			BaseCriteriaChecker instance = new T();
			if(!ReferenceEquals(null, criteria))
				return criteria.Accept(instance);
			return false;
		}
		protected BaseCriteriaChecker() { }
		public bool Process(CriteriaOperator op) {
			return ReferenceEquals(op, null) ? false : op.Accept(this);
		}
		protected bool ProcessAny(IEnumerable<CriteriaOperator> operands) {
			foreach(CriteriaOperator operand in operands)
				if(Process(operand))
					return true;
			return false;
		}
		bool ICriteriaVisitor<bool>.Visit(BetweenOperator theOperator) {
			return Process(theOperator.TestExpression) || Process(theOperator.BeginExpression) || Process(theOperator.EndExpression);
		}
		public virtual bool Visit(BinaryOperator theOperator) {
			return Process(theOperator.LeftOperand) || Process(theOperator.RightOperand);
		}
		public virtual bool Visit(FunctionOperator theOperator) {
			return ProcessAny(theOperator.Operands);
		}
		bool ICriteriaVisitor<bool>.Visit(GroupOperator theOperator) {
			return ProcessAny(theOperator.Operands);
		}
		public virtual bool Visit(InOperator theOperator) {
			return Process(theOperator.LeftOperand) || ProcessAny(theOperator.Operands);
		}
		public virtual bool Visit(OperandValue theOperand) {
			return false;
		}
		bool ICriteriaVisitor<bool>.Visit(UnaryOperator theOperator) {
			return Process(theOperator.Operand);
		}
		public virtual bool Visit(AggregateOperand theOperand) {
			return Process(theOperand.AggregatedExpression) || Process(theOperand.CollectionProperty) || Process(theOperand.Condition);
		}
		bool IClientCriteriaVisitor<bool>.Visit(JoinOperand theOperand) {
			return Process(theOperand.AggregatedExpression) || Process(theOperand.Condition);
		}
		public virtual bool Visit(OperandProperty theOperand) {
			return false;
		}
	}
}
