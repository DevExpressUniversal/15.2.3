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

using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.DataAccess;
using DevExpress.DataAccess.ConnectionParameters;
using System.Windows.Forms;
using DevExpress.DataAccess.Sql;
namespace DevExpress.DashboardCommon.Native {
	public class DataSourceConverter {
#pragma warning disable 0612, 0618
		readonly DataSource dataSource;
#pragma warning restore 0612, 0618
		readonly Dashboard dashboard;
		public IDashboardDataSource NewDataSource { get; private set; }
		public string NewDataMember { get; private set; }
#pragma warning disable 0612, 0618
		public DataSourceConverter(DataSource oldDataSource, Dashboard dashboard) {
#pragma warning restore 0612, 0618
			this.dataSource = oldDataSource;
			this.dashboard = dashboard;
		}
		public void Convert() {
			if(dataSource.OlapDataProvider != null) {
				NewDataSource = new DashboardOlapDataSource(dataSource.Name, dataSource.OlapDataProvider.DataConnection.Name, new OlapConnectionParameters(dataSource.OlapDataProvider.OlapConnectionString));
			} else if(dataSource.SqlDataProvider != null) {
#pragma warning disable 0612, 0618
				DataProviderBase dataProviderBase = dataSource.SqlDataProvider;
				DashboardSqlDataSource sqlDataSource =  dataProviderBase.DataConnection != null ?
							new DashboardSqlDataSource(dataSource.Name, dataProviderBase.DataConnection.CreateDataConnectionParameters()) { ConnectionName = dataProviderBase.DataConnection.Name } :
							new DashboardSqlDataSource(dataSource.Name);
				SqlQuery query = dataProviderBase.GetQuery();
				query.Name = dataSource.Name;
				NewDataMember = dataSource.Name;
				sqlDataSource.Queries.Add(query);
				query.Parameters.AddRange(dashboard.Parameters.Select(parameter => new QueryParameter(parameter.Name, typeof(Expression), new Expression(string.Format("[Parameters.{0}]", parameter.Name), parameter.Type))));
				sqlDataSource.DataProcessingMode = dataSource.DataProcessingMode;
				if(dataSource.Filter != null) {
					TableQuery tableQuery = query as TableQuery;
					if(tableQuery != null)
						tableQuery.FilterString = dataSource.Filter;
				}
				NewDataSource = sqlDataSource;
#pragma warning restore 0612, 0618
			} else {
				DashboardObjectDataSource objectDataSource = new DashboardObjectDataSource(dataSource.Name);
				BindingSource bindingSource = dataSource.Data as BindingSource;
				if(bindingSource != null) {
					objectDataSource.DataSource = bindingSource.DataSource;
					if(!string.IsNullOrEmpty(bindingSource.DataMember))
						objectDataSource.DataMember = bindingSource.DataMember;
				} else {
					objectDataSource.DataSchema = dataSource.DataSchema;
					objectDataSource.DataSource = dataSource.Data;
					if(!string.IsNullOrEmpty(dataSource.DataMember))
						objectDataSource.DataMember = dataSource.DataMember;
				}
				objectDataSource.Filter = dataSource.Filter;
				NewDataSource = objectDataSource;
			}
			foreach(CalculatedField calcField in dataSource.CalculatedFields) {
				NewDataSource.CalculatedFields.Add(calcField);
				if(!string.IsNullOrEmpty(NewDataMember))
					calcField.DataMember = NewDataMember;
			}
			NewDataSource.ComponentName = dataSource.ComponentName;
		}
	}
}
