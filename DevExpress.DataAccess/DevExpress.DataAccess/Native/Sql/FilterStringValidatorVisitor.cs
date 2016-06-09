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
	public class FilterStringValidatorVisitor : IClientCriteriaVisitor {
		readonly HashSet<string> tableActualNames;
		readonly Dictionary<string, DBTable> dbTables;
		public FilterStringValidatorVisitor(HashSet<string> tableActualNames, Dictionary<string, DBTable> dbTables) {
			this.tableActualNames = tableActualNames;
			this.dbTables = dbTables;
		}
		public void Process(CriteriaOperator theOperator) {
			if(!ReferenceEquals(theOperator, null))
				theOperator.Accept(this);
		}
		#region Implementation of ICriteriaVisitor
		public void Visit(BetweenOperator theOperator) {
			Process(theOperator.TestExpression);
			Process(theOperator.BeginExpression);
			Process(theOperator.EndExpression);
		}
		public void Visit(BinaryOperator theOperator) {
			Process(theOperator.LeftOperand);
			Process(theOperator.RightOperand);
		}
		public void Visit(UnaryOperator theOperator) { Process(theOperator.Operand); }
		public void Visit(InOperator theOperator) {
			Process(theOperator.LeftOperand);
			foreach(CriteriaOperator operand in theOperator.Operands)
				Process(operand);
		}
		public void Visit(GroupOperator theOperator) {
			foreach(CriteriaOperator operand in theOperator.Operands)
				Process(operand);
		}
		public void Visit(OperandValue theOperand) {  }
		public void Visit(FunctionOperator theOperator) {
			foreach(CriteriaOperator operand in theOperator.Operands)
				Process(operand);
		}
		#endregion
		#region Implementation of IClientCriteriaVisitor
		public void Visit(AggregateOperand theOperand) {  }
		public void Visit(OperandProperty theOperand) {
			string propertyName = theOperand.PropertyName;
			string tableName = CheckTableName(propertyName);
			if(dbTables == null)
				return;
			if(tableName != null)
				CheckColumnName(propertyName, tableName);
			else
				CheckOrphanColumn(propertyName);
		}
		public void Visit(JoinOperand theOperand) {  }
		#endregion
		string CheckTableName(string propertyName) {
			int dotIndex = propertyName.LastIndexOf('.');
			if(dotIndex == -1)
				return null;
			string name = propertyName.Substring(0, dotIndex);
			if(tableActualNames.Contains(name.ToLowerInvariant()))
				return name;
			throw new FilterByColumnOfMissingTableValidationException(propertyName);
		}
		void CheckColumnName(string propertyName, string tableName) {
			DBTable dbTable = dbTables[tableName.ToLowerInvariant()];
			string columnName = propertyName.Substring(tableName.Length + 1);
			DBColumn dbColumn = dbTable.GetColumn(columnName);
			if(dbColumn == null) {
				tableName = propertyName.Substring(0, tableName.Length);
				throw new FilterByMissingInSchemaColumnValidationExpression(tableName, columnName);
			}
		}
		void CheckOrphanColumn(string columnName) {
			string[] tables =
				dbTables.Values
					.Where(table => table.GetColumn(columnName) != null)
					.Select(table => table.Name)
					.Where(table => tableActualNames.Contains(table.ToLowerInvariant()))
					.ToArray();
			if(tables.Length == 0)
				throw new FilterByColumnOfMissingTableValidationException(columnName);
			if(tables.Length > 1)
				throw new FilterByAmbiguousColumnValidationException(columnName, tables);
		}
	}
}
