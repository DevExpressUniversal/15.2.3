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
using System.ComponentModel;
using DevExpress.Data;
using DevExpress.PivotGrid.QueryMode;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Compatibility.System.ComponentModel;
using System.Threading;
namespace DevExpress.PivotGrid.ServerMode {
	public class ServerModeDataSource : QueryDataSource<ServerModeColumn>, IServerModeHelpersOwner {
		bool caseSensitiveDataBinding;
		ServerModeMetadata metadata;		
		protected override QueryMetadata<ServerModeColumn> Metadata {
			get { return metadata; }
		}
		ServerModeMetadata ServerModeMetadata { get { return metadata; } }
		protected override XtraPivotGrid.Data.PivotDataSourceCaps Capabilities {
			get {
				return PivotDataSourceCaps.UnboundColumns | PivotDataSourceCaps.Prefilter;
			}
		}
		public bool CaseSensitiveDataBinding {
			get { return caseSensitiveDataBinding; }
			set { caseSensitiveDataBinding = value; }
		}
		public ServerModeDataSource(IPivotQueryExecutor exec) : this(exec, true) {
		}
		public ServerModeDataSource(IPivotQueryExecutor exec, bool caseSensitiveDataBinding) : this(new ServerModeMetadata(exec), caseSensitiveDataBinding) {
		}
		ServerModeDataSource(ServerModeMetadata metadata, bool caseSensitiveDataBinding) {
			this.metadata = metadata;
			this.caseSensitiveDataBinding = caseSensitiveDataBinding;
		}
		protected override IQueryFilterHelper CreateFilterHelper() {
			return new FilterHelper(this);
		}
		protected override UniqueValues<ServerModeColumn> CreateUniqueValues() {
			return new UniqueValues(this);
		}
		protected override QueryDataSource<ServerModeColumn> CreateInstance() {
			return new ServerModeDataSource(metadata, CaseSensitiveDataBinding);
		}
		protected override QueryAreas<ServerModeColumn> CreateAreas() {
			return new Areas(this);
		}
		protected override void PopulateColumns() {
			((ServerModeMetadata)Metadata).PopulateColumns(this);
		}
		protected override void QueryFullyExpanded() {
			QuertyFullyExpanded(AutoExpandGroups, true);
		}
		protected override IQueryContext<ServerModeColumn> CreateQueryContext(GroupInfo[] columns, GroupInfo[] rows, bool columnExpand, bool rowExpand) {
			if(!(columnExpand && rowExpand))
				return new QueryContext(this, columns, rows, columnExpand, rowExpand, Areas);
			else
				return new FullExpandQueryContext(this, Areas);
		}
		protected override QueryColumns<ServerModeColumn> CreateCubeColumns() {
			return new CubeColumns(this);
		}
		protected override ServerModeColumn CreateColumnCore(IQueryMetadataColumn column, PivotGridFieldBase field) {
			return new ServerModeColumn(column, field.Area == PivotArea.DataArea, CubeColumns.GetFieldCubeColumnsName(field));
		}
		Areas IServerModeHelpersOwner.Areas { get { return (Areas)Areas; } }
		IPivotQueryExecutor IServerModeHelpersOwner.Executor { get { return ((ServerModeMetadata)Metadata).Exec; } }
		List<object> IServerModeHelpersOwner.QueryValues(QueryColumn column, Dictionary<QueryColumn, object> values) {
			return ServerModeMetadata.QueryValues(this, column, values);
		}
		FilterHelper IServerModeHelpersOwner.FilterHelper { get { return (FilterHelper)FilterHelper; } }
		protected override bool ExpandGroups(bool isColumn, GroupInfo[] groups) {
			return ExpandCore(isColumn, groups);
		}
		protected override bool ChangeExpandedAll(bool expanded) {
			return QuertyFullyExpanded(expanded, false);
		}
		protected override bool IsUnboundExpressionValidCore(PivotGridFieldBase field) {
			if(Metadata.Columns.Count == 0)
				PopulateColumns();
			return ServerModeMetadata.ValidateExpression(this, field.UnboundExpression, field.UnboundType);
		}
		bool QuertyFullyExpanded(bool expanded, bool needFirstLevel) {
			try {
				RowValues.BeginUpdate();
				ColumnValues.BeginUpdate();
				if(needFirstLevel)
					if(expanded)
						QueryData(null, null, true, true);
					else
						QueryData(new GroupInfo[0], new GroupInfo[0], false, false);
			} finally {
				RowValues.EndUpdate();
				ColumnValues.EndUpdate();
			}
			return base.ChangeExpandedAll(expanded);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Type GetUnboundExpressionType(string expression, bool makeQuery) {
			if(Metadata.Columns.Count == 0)
				PopulateColumns();
			return ServerModeMetadata.GetUnboundExpressionType(this, expression, makeQuery);
		}
		protected override Type GetFieldTypeCore(PivotGridFieldBase field) {
			ServerModeColumn column;
			if(CubeColumns.TryGetValue(field, out column) && Areas.ContainsDataColumn(column))
				return ((ServerModeCellTable)Areas.Cells).GetActualDataType(column);
			return base.GetFieldTypeCore(field);
		}
	}
}
