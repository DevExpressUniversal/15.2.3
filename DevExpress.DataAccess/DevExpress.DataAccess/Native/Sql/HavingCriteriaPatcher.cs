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
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.DataAccess.Sql;
using DevExpress.Xpo.DB;
namespace DevExpress.DataAccess.Native.Sql {
	public class HavingCriteriaPatcher : IQueryCriteriaVisitor<CriteriaOperator> {
		struct Column {
			readonly string table;
			readonly string column;
			readonly AggregationType aggregation;
			public Column(string table, string column, AggregationType aggregation) {
				this.table = table;
				this.column = column;
				this.aggregation = aggregation;
			}
			public string TableName { get { return table; } }
			public string ColumnName { get { return column; } }
			public AggregationType Aggregation { get { return aggregation; } }
		}
		readonly Dictionary<string, Column> columnAliases;
		public HavingCriteriaPatcher(TableInfoList selected) {
			columnAliases = new Dictionary<string, Column>();
			foreach(TableInfo tableInfo in selected)
				foreach(ColumnInfo columnInfo in tableInfo.SelectedColumns)
					columnAliases.Add(columnInfo.ActualName, new Column(tableInfo.Name, columnInfo.Name, columnInfo.Aggregation));
		}
		#region Implementation of ICriteriaVisitor<CriteriaOperator>
		public CriteriaOperator Visit(BetweenOperator theOperator) {
			return new BetweenOperator(
				theOperator.TestExpression.Accept(this),
				theOperator.BeginExpression.Accept(this), 
				theOperator.EndExpression.Accept(this));
		}
		public CriteriaOperator Visit(BinaryOperator theOperator) {
			return new BinaryOperator(
				theOperator.LeftOperand.Accept(this),
				theOperator.RightOperand.Accept(this),
				theOperator.OperatorType);
		}
		public CriteriaOperator Visit(UnaryOperator theOperator) {
			return new UnaryOperator(
				theOperator.OperatorType,
				theOperator.Operand.Accept(this));
		}
		public CriteriaOperator Visit(InOperator theOperator) {
			return new InOperator(
				theOperator.LeftOperand.Accept(this),
				theOperator.Operands.Select(op => op.Accept(this)));
		}
		public CriteriaOperator Visit(GroupOperator theOperator) {
			return new GroupOperator(
				theOperator.OperatorType,
				theOperator.Operands.Select(op => op.Accept(this)));
		}
		public CriteriaOperator Visit(OperandValue theOperand) {
			return theOperand;
		}
		public CriteriaOperator Visit(FunctionOperator theOperator) {
			return new FunctionOperator(
				theOperator.OperatorType,
				theOperator.Operands.Select(op => op.Accept(this)));
		}
		#endregion
		#region Implementation of IQueryCriteriaVisitor<CriteriaOperator>
		public CriteriaOperator Visit(QueryOperand theOperand) {
			Column column;
			if(theOperand.NodeAlias != null || !columnAliases.TryGetValue(theOperand.ColumnName, out column))
				return new QueryOperand(theOperand.ColumnName, theOperand.NodeAlias, theOperand.ColumnType);
			if(column.Aggregation == AggregationType.None)
				return new QueryOperand(column.ColumnName, column.TableName, theOperand.ColumnType);
			return new QuerySubQueryContainer(null, new QueryOperand(column.ColumnName, column.TableName), (Aggregate)column.Aggregation);
		}
		public CriteriaOperator Visit(QuerySubQueryContainer theOperand) { throw new NotSupportedException(); }
		#endregion
	}
}
