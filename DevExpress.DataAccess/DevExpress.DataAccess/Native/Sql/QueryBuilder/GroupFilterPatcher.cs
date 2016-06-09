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
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.DataAccess.Sql;
using DevExpress.Xpo.DB;
namespace DevExpress.DataAccess.Native.Sql.QueryBuilder {
	public static class GroupFilterPatcher {
		enum Command {
			PrependTableNames,
			CutOffTableNames,
			RenameColumn
		}
		class Visitor : ClientCriteriaVisitorBase, IQueryCriteriaVisitor<CriteriaOperator> {
			readonly Command command;
			readonly TableQuery query;
			readonly string renameFrom;
			readonly string renameTo;
			public Visitor(Command command, TableQuery query) {
				this.command = command;
				this.query = query;
			}
			public Visitor(string renameFrom, string renameTo, TableQuery query) : this(Command.RenameColumn, query) {
				this.renameFrom = renameFrom;
				this.renameTo = renameTo;
			}
			public new CriteriaOperator Process(CriteriaOperator input) { return base.Process(input); }
			#region Implementation of IQueryCriteriaVisitor<CriteriaOperator>
			public CriteriaOperator Visit(QueryOperand theOperand) {
				switch(command) {
					case Command.CutOffTableNames:
						return new OperandProperty(theOperand.ColumnName);
					case Command.PrependTableNames:
						TableInfo table = query.Tables.FirstOrDefault(t => t.SelectedColumns.Any(c => c.ActualName == theOperand.ColumnName));
						string tableName = table != null ? table.ActualName : "???";
						return new OperandProperty(string.Concat(tableName, ".", theOperand.ColumnName));
					case Command.RenameColumn:
						if(theOperand.NodeAlias != null)
							break;
						return new OperandProperty(theOperand.ColumnName == renameFrom ? renameTo : theOperand.ColumnName);
				}
				return theOperand;
			}
			public CriteriaOperator Visit(QuerySubQueryContainer theOperand) { throw new NotSupportedException(); }
			#endregion
		}
		public static string CutOffTableNames(string filterString, TableQuery query) {
			return Core(new Visitor(Command.CutOffTableNames, query), filterString, query.Parameters);
		}
		public static string PrependTableNames(string filterString, TableQuery query) {
			return Core(new Visitor(Command.PrependTableNames, query), filterString, query.Parameters);
		}
		public static string RenameColumn(string filterString, TableQuery query, string oldName, string newName) {
			return Core(new Visitor(oldName, newName, query), filterString, query.Parameters);
		}
		static string Core(Visitor visitor, string filterString, List<QueryParameter> parameters) {
			if(filterString == null)
				return null;
			return CriteriaOperator.ToString(visitor.Process(FilterHelper.GetFilter(filterString, parameters)));
		}
		public static IEnumerable<DBTable> GetVirtualSchema(TableQuery query, DBSchema schema) {
			Dictionary<string, DBTable> tablesDictionary = null;
			foreach(TableInfo table in query.Tables) {
				DBTable dbTable = new DBTable(table.ActualName);
				foreach(ColumnInfo column in table.SelectedColumns) {
					DBColumnType columnType;
					switch(column.Aggregation) {
						case AggregationType.Count:
							columnType = DBColumnType.Int64;
							break;
						case AggregationType.Avg:
							columnType = DBColumnType.Double;
							break;
						default:
							if(tablesDictionary == null)
								tablesDictionary = schema.Tables.Union(schema.Views).ToDictionary(t => t.Name);
							columnType = tablesDictionary[table.Name].GetColumn(column.Name).ColumnType;
							break;
					}
					dbTable.AddColumn(new DBColumn(column.ActualName, false, null, 0, columnType));
				}
				if(dbTable.Columns.Count > 0)
					yield return dbTable;
			}
		}
	}
}
