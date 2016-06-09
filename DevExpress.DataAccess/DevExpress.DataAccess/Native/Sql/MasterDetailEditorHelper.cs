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
using System.Collections.Specialized;
using System.Linq;
using DevExpress.DataAccess.Localization;
using DevExpress.DataAccess.Sql;
using DevExpress.Xpo.DB;
namespace DevExpress.DataAccess.Native.Sql {
	public static class MasterDetailEditorHelper {
		public static List<MasterDetailInfo> GetRelationsBetweenQueries(SqlDataSource dataSource, string masterQueryName, string detailQueryName) {
			List<MasterDetailInfo> masterDetails = new List<MasterDetailInfo>();
			if(dataSource.DBSchema != null) {
				TableQuery masterQuery = dataSource.Queries.Single(q => q.Name == masterQueryName) as TableQuery;
				TableQuery detailQuery = dataSource.Queries.Single(q => q.Name == detailQueryName) as TableQuery;
				if(masterQuery != null && detailQuery != null) {
					foreach(TableInfo detailTable in detailQuery.Tables) {
						DBTable detailDBTable = dataSource.DBSchema.Tables.FirstOrDefault(t => t.Name == detailTable.Name);
						if(detailDBTable != null) {
							foreach(DBForeignKey foreignKey in detailDBTable.ForeignKeys) {
								if(!CheckTableContainsColumns(foreignKey.Columns, detailTable))
									break;
								TableInfo masterTable = masterQuery.Tables.FirstOrDefault(t => t.Name == foreignKey.PrimaryKeyTable);
								if(masterTable != null) {
									if(!CheckTableContainsColumns(foreignKey.PrimaryKeyTableKeyColumns, masterTable))
										break;
									MasterDetailInfo keyMasterDetailInfo = new MasterDetailInfo(masterQuery.Name, detailQuery.Name);
									for(int i = 0; i < foreignKey.Columns.Count; i++) {
										RelationColumnInfo keyRelationColumnInfo = new RelationColumnInfo();
										keyRelationColumnInfo.ParentKeyColumn = masterTable.SelectedColumns.Single(c => c.Name == foreignKey.PrimaryKeyTableKeyColumns[i]).ActualName;
										keyRelationColumnInfo.NestedKeyColumn = detailTable.SelectedColumns.Single(c => c.Name == foreignKey.Columns[i]).ActualName;
										keyMasterDetailInfo.KeyColumns.Add(keyRelationColumnInfo);
									}
									keyMasterDetailInfo.Name = foreignKey.Name;
									masterDetails.Add(keyMasterDetailInfo);
								}
							}
						}
					}
				}
			}
			return masterDetails;
		}
		public static void ValidateRelations(SqlDataSource dataSource, IEnumerable<MasterDetailInfo> relations) {
			foreach(MasterDetailInfo relation in relations) {
				ResultTable masterTable = dataSource.ResultSet.Tables.FirstOrDefault(t => t.TableName == relation.MasterQueryName);
				ResultTable detailTable = dataSource.ResultSet.Tables.FirstOrDefault(t => t.TableName == relation.DetailQueryName);
				if(masterTable == null)
					throw new InvalidConditionException(relation.MasterQueryName);
				if(detailTable == null)
					throw new InvalidConditionException(relation.DetailQueryName);
				foreach(RelationColumnInfo relationColumn in relation.KeyColumns) {
					ResultColumn masterColumn = masterTable.Columns.FirstOrDefault(c => c.Name == relationColumn.ParentKeyColumn);
					ResultColumn detailColumn = detailTable.Columns.FirstOrDefault(c => c.Name == relationColumn.NestedKeyColumn);
					if(masterColumn == null)
						throw new InvalidConditionException(relation.MasterQueryName, relationColumn.ParentKeyColumn);
					if(detailColumn == null)
						throw new InvalidConditionException(relation.DetailQueryName, relationColumn.NestedKeyColumn);
					if(masterColumn.PropertyType != detailColumn.PropertyType)
						throw new RelationException(relation.MasterQueryName, relationColumn.ParentKeyColumn, masterColumn.PropertyType, relation.DetailQueryName, relationColumn.NestedKeyColumn, detailColumn.PropertyType);
				}
			}
		}
		static bool CheckTableContainsColumns(StringCollection keyColumns, TableInfo table) {
			foreach(string col in keyColumns)
				if(table.SelectedColumns.All(c => c.Name != col))
					return false;
			return true;
		}
	}
	public class InvalidConditionException : Exception {
		public string ColumnName { get; private set; }
		public string QueryName { get; private set; }
		public InvalidConditionException(string queryName)
			: base(string.Format(DataAccessLocalizer.GetString(DataAccessStringId.MessageNonexistentQuery), queryName)) {
			QueryName = queryName;
		}
		public InvalidConditionException(string queryName, string columnName)
			: base(string.Format(DataAccessLocalizer.GetString(DataAccessStringId.MessageNonexistentColumn), queryName, columnName)) {
			QueryName = queryName;
			ColumnName = columnName;
		}
	}
}
