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
using DevExpress.DashboardCommon;
using DevExpress.Data;
using DevExpress.DataAccess;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Sql;
using DevExpress.Utils;
namespace DevExpress.DashboardWeb {
	public class DashboardLoadingEventArgs : EventArgs {
		public string DashboardId { get; private set; }
		public string DashboardXml { get; set; }
		public Type DashboardType { get; set; }
		internal DashboardLoadingEventArgs(string dashboardId) {
			DashboardId = dashboardId;
		}
	}
	public delegate void DashboardLoadingEventHandler(object sender, DashboardLoadingEventArgs e);
	public class DataLoadingWebEventArgs : DataLoadingEventArgs {
		public string DashboardId { get; private set; }
		internal DataLoadingWebEventArgs(string dashboardId, string dataSourceComponentName, string dataSourceName)
			: base(dataSourceComponentName, dataSourceName) {
			DashboardId = dashboardId;
		}
	}
	public delegate void DataLoadingWebEventHandler(object sender, DataLoadingWebEventArgs e);   
	public class ConfigureDataConnectionWebEventArgs : DashboardConfigureDataConnectionEventArgs {
		public string DashboardId { get; private set; }
		internal ConfigureDataConnectionWebEventArgs(string dashboardId, string connectionName, string dataSourceName, DataConnectionParametersBase connectionParameters)
			: base(connectionName, dataSourceName, connectionParameters) {
			DashboardId = dashboardId;
		}
	}
	public delegate void ConfigureDataConnectionWebEventHandler(object sender, ConfigureDataConnectionWebEventArgs e);
	public class CustomFilterExpressionWebEventArgs : CustomFilterExpressionEventArgs {
		public string DashboardId { get; private set; }
		internal CustomFilterExpressionWebEventArgs(string dashboardId, string dataSourceComponentName, string dataSourceName, string tableName)
			: base(dataSourceComponentName, dataSourceName, tableName) {
			DashboardId = dashboardId;
		}
	}
	public delegate void CustomFilterExpressionWebEventHandler(object sender, CustomFilterExpressionWebEventArgs e);
	public class CustomParametersWebEventArgs : CustomParametersEventArgs {
		public string DashboardId { get; private set; }
		internal CustomParametersWebEventArgs(string dashboardId, IEnumerable<IParameter> parameters)
			: base(parameters) {
			DashboardId = dashboardId;
		}
	}
	public delegate void CustomParametersWebEventHandler(object sender, CustomParametersWebEventArgs e);
	public class DashboardLoadedWebEventArgs : EventArgs {
		readonly Dashboard dashboard;
		readonly string dashboardId;
		public Dashboard Dashboard { get { return dashboard; } }
		public string DashboardId { get { return dashboardId; } }
		public DashboardLoadedWebEventArgs(string dashboardId, Dashboard dashboard) {
			this.dashboardId = dashboardId;
			this.dashboard = dashboard;
		}
	}
	public delegate void DashboardLoadedWebEventHandler(object sender, DashboardLoadedWebEventArgs e);
	public class ValidateDashboardCustomSqlQueryWebEventArgs : EventArgs {
		readonly ValidateDashboardCustomSqlQueryEventArgs innerArgs;
		public string DashboardId { get { return innerArgs.DashboardId; } }
		public string DataSourceComponentName { get { return innerArgs.DataSourceComponentName; } }
		public string DataSourceName { get { return innerArgs.DataSourceName; } }
		public CustomSqlQuery CustomSqlQuery { get { return innerArgs.CustomSqlQuery; } }
		public string ExceptionMessage { get { return innerArgs.ExceptionMessage; } set { innerArgs.ExceptionMessage = value; } }
		public bool Valid { get { return innerArgs.Valid; } set { innerArgs.Valid = value; } }
		public ValidateDashboardCustomSqlQueryWebEventArgs(ValidateDashboardCustomSqlQueryEventArgs innerArgs) {
			Guard.ArgumentNotNull(innerArgs, "innerArgs");
			this.innerArgs = innerArgs;
		}
	}
	public delegate void ValidateDashboardCustomSqlQueryWebEventHandler(object sender, ValidateDashboardCustomSqlQueryWebEventArgs e);
}
