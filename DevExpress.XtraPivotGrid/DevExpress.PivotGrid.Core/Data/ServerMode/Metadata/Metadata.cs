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
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.Helpers;
using DevExpress.Data.IO;
using DevExpress.PivotGrid.QueryMode;
namespace DevExpress.PivotGrid.ServerMode {
	class ServerModeMetadata : QueryMetadata<ServerModeColumn> {
		IPivotQueryExecutor exec;
		public IPivotQueryExecutor Exec { get { return exec; } }
		protected new IServerModeQueryExecutor QueryExecutor {
			get { return (IServerModeQueryExecutor)base.QueryExecutor; }
			set { base.QueryExecutor = value; }
		}
		public ServerModeMetadata(IPivotQueryExecutor exec) {
			this.exec = exec;
			QueryExecutor = new QueryExecutor(exec, this);
		}
		internal bool PopulateColumns(ServerModeDataSource owner) {
			return PopulateColumnsCore(owner);
		}
		public override bool Connected {
			get { return exec != null && exec.Connected; }
		}
		protected internal override bool Disconnect() {
			return true;
		}
		protected internal override bool PopulateColumnsCore(IDataSourceHelpersOwner<ServerModeColumn> owner) {
			Columns.Clear();
			IPivotQueryExecutor exec = ((IServerModeHelpersOwner)owner).Executor;
			if(!Connected)
				return false;
			foreach(ServerModeColumnModel column in exec.GetColumns()) {
				string caption = string.IsNullOrEmpty(column.Caption) ? column.Name : column.Caption;
				Columns.Add(new MetadataColumn(column.Name, column.Alias, column.TableName, column.TableAlias, column.DataType, column.DisplayFolder, SplitStringHelper.SplitPascalCaseString(caption)));
			}
			return true;
		}
		protected override QueryMember LoadMemberCore(IQueryMetadataColumn column, TypedBinaryReader reader) {
			object value = LoadMemberValue(column, reader, false);
			return new ServerModeMember(column, value);
		}
		protected override QueryMetadataColumns CreateMetadataColumns() {
			return new ServerModeMetadataColumns(this);
		}
		internal List<object> QueryValues(IServerModeHelpersOwner owner, QueryColumn column, Dictionary<QueryColumn, object> values) {
			return QueryExecutor.QueryValues(owner, column, values);
		}
		internal bool ValidateExpression(IServerModeHelpersOwner owner, string expression, UnboundColumnType columnType) {
			try {
				CriteriaOperator criteria = CriteriaOperator.Parse(expression);
				if(object.ReferenceEquals(null, criteria))
					return false;
				if(!Connected)
					return false;
				return QueryExecutor.ValidateExpression(owner, criteria, columnType);
			} catch {
				return false;
			}
		}
		internal Type GetUnboundExpressionType(IServerModeHelpersOwner owner, string expression, bool makeQuery) {
			try {
				CriteriaOperator criteria = CriteriaOperator.Parse(expression);
				if(object.ReferenceEquals(null, criteria))
					return null;
				return QueryExecutor.GetUnboundExpressionType(owner, criteria, makeQuery);
			} catch {
				return null;
			}
		}
	}
}
