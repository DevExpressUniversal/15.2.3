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
using System;
namespace DevExpress.PivotGrid.QueryMode {
	public abstract class QueryContextBase<TColumn> : IQueryContext<TColumn> where TColumn : QueryColumn {
		readonly IActionProvider<TColumn, CellSet<TColumn>> columnProvider, rowProvider;
		GroupInfo[] columns, rows;
		bool columnExpand, rowExpand;
		QueryAreas<TColumn> areas;
		List<TColumn> columnArea, rowArea;
		List<QueryTuple> columnTuples, rowTuples;
		IDataSourceHelpersOwner<TColumn> owner;
		protected QueryContextBase(IDataSourceHelpersOwner<TColumn> owner, GroupInfo[] columns, GroupInfo[] rows, bool columnExpand, bool rowExpand, QueryAreas<TColumn> areas) {
			this.owner = owner;
			this.columns = columns;
			this.rows = rows;
			this.columnExpand = columnExpand;
			this.rowExpand = rowExpand;
			this.areas = areas;
			this.columnProvider = GetActionProvider(columns, columnExpand);
			this.rowProvider = GetActionProvider(rows, rowExpand);
			this.columnArea = columnProvider.GetColumns(Areas.ColumnArea, columns);
			this.rowArea = rowProvider.GetColumns(Areas.RowArea, rows);
			this.columnTuples = columnProvider.GetTuples(Areas.ColumnArea, areas.ColumnValues, columns);
			this.rowTuples = rowProvider.GetTuples(Areas.RowArea, areas.RowValues, rows);
			Initialize(columns, rows, columnExpand, rowExpand, areas);
		}
		protected virtual void Initialize(GroupInfo[] columns, GroupInfo[] rows, bool columnExpand, bool rowExpand, QueryAreas<TColumn> areas) { }
		bool IQueryContext<TColumn>.HandleException(QueryHandleableException exception) {
			return ((QueryMetadata<TColumn>)owner.Metadata).HandleException(owner, exception);
		}
		#region IQueryContextBase Members
		public IDataSourceHelpersOwner<TColumn> Owner { get { return owner; } }
		public QueryAreas<TColumn> Areas { get { return areas; } }
		protected GroupInfo[] Columns { get { return columns; } }
		protected GroupInfo[] Rows { get { return rows; } }
		public bool ColumnExpand { get { return columnExpand; } }
		public bool RowExpand { get { return rowExpand; } }
		public AreaFieldValues ColumnValues { get { return areas.ColumnValues; } }
		public AreaFieldValues RowValues { get { return areas.RowValues; } }
		public List<QueryTuple> ColumnTuples { get { return columnTuples; } }
		public List<QueryTuple> RowTuples { get { return rowTuples; } }
		List<QueryTuple> IQueryContext<TColumn>.GetTuples(bool isColumn) {
			return isColumn ? columnTuples : rowTuples;
		}
		public List<TColumn> ColumnArea { get { return columnArea; } }
		public List<TColumn> RowArea { get { return rowArea; } }
		List<TColumn> IQueryContext<TColumn>.GetArea(bool isColumn) {
			return isColumn ? columnArea : rowArea;
		}
		public void PreParseResult(CellSet<TColumn> queryResult) {
			columnProvider.ParseFieldValues(queryResult, this, true);
			rowProvider.ParseFieldValues(queryResult, this, false);
		}
		protected abstract IActionProvider<TColumn, CellSet<TColumn>> GetActionProvider(GroupInfo[] columns, bool expand);
		public virtual void PerformSorting() { }
		#endregion
	}
}
