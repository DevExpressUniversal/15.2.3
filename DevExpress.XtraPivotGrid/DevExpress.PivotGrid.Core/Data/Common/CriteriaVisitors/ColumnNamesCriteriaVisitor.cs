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
	public class ColumnNamesCriteriaVisitor : IClientCriteriaVisitor {
		readonly bool patchOperands;
		List<string> columnNames;
		public List<string> ColumnNames {
			get { return columnNames; }
		}
		public ColumnNamesCriteriaVisitor(bool patchOperands) {
			columnNames = new List<string>();
			this.patchOperands = patchOperands;
		}
		#region IClientCriteriaVisitor Members
		public void Visit(DevExpress.Data.Filtering.OperandProperty theOperand) {
			if(!columnNames.Contains(theOperand.PropertyName))
				columnNames.Add(theOperand.PropertyName);
		}
		public void Visit(DevExpress.Data.Filtering.JoinOperand theOperand) {
			Process(theOperand.Condition);
		}
		public void Visit(DevExpress.Data.Filtering.AggregateOperand theOperand) {
			Process(theOperand.CollectionProperty);
			Process(theOperand.Condition);
			Process(theOperand.AggregatedExpression);
		}
		#endregion
		#region ICriteriaVisitor Members
		public void Visit(DevExpress.Data.Filtering.OperandValue theOperand) {
			if(!patchOperands)
				return;
			theOperand.Value = GetInnerValue(theOperand.Value);
		}
		object GetInnerValue(object value) {
			var item = value as FilterItem;
			return (item == null) ? value : GetInnerValue(item.Value);
		}
		public void Visit(DevExpress.Data.Filtering.FunctionOperator theOperator) {
			foreach(CriteriaOperator operand in theOperator.Operands)
				Process(operand);
		}
		public void Visit(DevExpress.Data.Filtering.GroupOperator theOperator) {
			foreach(CriteriaOperator operand in theOperator.Operands)
				Process(operand);
		}
		public void Visit(DevExpress.Data.Filtering.InOperator theOperator) {
			Process(theOperator.LeftOperand);
			foreach(CriteriaOperator operand in theOperator.Operands)
				Process(operand);
		}
		public void Visit(DevExpress.Data.Filtering.UnaryOperator theOperator) {
			Process(theOperator.Operand);
		}
		public void Visit(DevExpress.Data.Filtering.BinaryOperator theOperator) {
			Process(theOperator.LeftOperand);
			Process(theOperator.RightOperand);
		}
		public void Visit(BetweenOperator theOperator) {
			Process(theOperator.TestExpression);
			Process(theOperator.BeginExpression);
			Process(theOperator.EndExpression);
		}
		#endregion
		void Process(CriteriaOperator operand) {
			if(ReferenceEquals(operand, null))
				return;
			operand.Accept(this);
		}
	}
}
