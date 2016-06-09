#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.Data.Filtering;
using System;
using System.Linq;
using System.Collections.Generic;
namespace DevExpress.DashboardCommon.Native {
	public class CriteriaSearcher : IClientCriteriaVisitor<bool> {
		public static bool Search(CriteriaOperator criteria, string propertyName) {
			CriteriaSearcher searcher = new CriteriaSearcher(propertyName);
			return searcher.Search(criteria);
		}
		readonly string propertyName;
		CriteriaSearcher(string propertyName) {
			this.propertyName = propertyName;
		}
		bool Search(CriteriaOperator criteria) {
			return !ReferenceEquals(criteria, null) && criteria.Accept(this);
		}
		bool Search(IEnumerable<CriteriaOperator> operands) {
			foreach (CriteriaOperator operand in operands) {
				if (Search(operand))
					return true;
			}
			return false;
		}
		bool IClientCriteriaVisitor<bool>.Visit(JoinOperand theOperand) {
			return false;
		}
		bool IClientCriteriaVisitor<bool>.Visit(OperandProperty theOperand) {
			return theOperand.PropertyName == propertyName;
		}
		bool IClientCriteriaVisitor<bool>.Visit(AggregateOperand theOperand) {
			return false;
		}
		bool ICriteriaVisitor<bool>.Visit(FunctionOperator theOperator) {
			return Search(theOperator.Operands);
		}
		bool ICriteriaVisitor<bool>.Visit(OperandValue theOperand) {
			return false;
		}
		bool ICriteriaVisitor<bool>.Visit(GroupOperator theOperator) {
			return Search(theOperator.Operands);
		}
		bool ICriteriaVisitor<bool>.Visit(InOperator theOperator) {
			return Search(theOperator.LeftOperand) || Search(theOperator.Operands);
		}
		bool ICriteriaVisitor<bool>.Visit(UnaryOperator theOperator) {
			return Search(theOperator.Operand);
		}
		bool ICriteriaVisitor<bool>.Visit(BinaryOperator theOperator) {
			return Search(theOperator.LeftOperand) || Search(theOperator.RightOperand);
		}
		bool ICriteriaVisitor<bool>.Visit(BetweenOperator theOperator) {
			return Search(theOperator.TestExpression) || Search(theOperator.BeginExpression) || Search(theOperator.EndExpression);
		}
	}
	class CriteriaTreeSearcher : IClientCriteriaVisitor<bool> {
		public static bool Contains(CriteriaOperator criteriaTree, CriteriaOperator criteriaElement) {
			CriteriaTreeSearcher searcher = new CriteriaTreeSearcher(criteriaElement);
			return searcher.Search(criteriaTree);
		}
		readonly CriteriaOperator criteriaElement;
		protected CriteriaTreeSearcher(CriteriaOperator criteriaElement) {
			this.criteriaElement = criteriaElement;
		}
		bool Search(CriteriaOperator criteria) {
			if (ReferenceEquals(criteria, null))
				return false;
			else
				return criteria.Equals(criteriaElement) || criteria.Accept(this);
		}
		bool Search(IEnumerable<CriteriaOperator> operands) {
			return operands.Any(operand => Search(operand));
		}
		bool IClientCriteriaVisitor<bool>.Visit(JoinOperand theOperand) {
			return false;
		}
		bool IClientCriteriaVisitor<bool>.Visit(OperandProperty theOperand) {
			return false;
		}
		bool IClientCriteriaVisitor<bool>.Visit(AggregateOperand theOperand) {
			return false;
		}
		bool ICriteriaVisitor<bool>.Visit(FunctionOperator theOperator) {
			return Search(theOperator.Operands);
		}
		bool ICriteriaVisitor<bool>.Visit(OperandValue theOperand) {
			return false;
		}
		bool ICriteriaVisitor<bool>.Visit(GroupOperator theOperator) {
			return false;
		}
		bool ICriteriaVisitor<bool>.Visit(InOperator theOperator) {
			return false;
		}
		bool ICriteriaVisitor<bool>.Visit(UnaryOperator theOperator) {
			return false;
		}
		bool ICriteriaVisitor<bool>.Visit(BinaryOperator theOperator) {
			return Search(theOperator.LeftOperand) || Search(theOperator.RightOperand);
		}
		bool ICriteriaVisitor<bool>.Visit(BetweenOperator theOperator) {
			return false;
		}
	}
}
