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
using System.Linq;
using System.Collections.Generic;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.DataAccess;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.Utils;
using DevExpress.DataAccess.Native;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Service;
using DevExpress.Data.Utils;
using DevExpress.DataAccess.Native.Excel;
namespace DevExpress.DashboardCommon.Server {
	public abstract class DashboardServerEventArgs : EventArgs {
		public string DashboardId { get; private set; }
		protected DashboardServerEventArgs(string dashboardId) {
			DashboardId = dashboardId;
		}
	}
	public class ConfigureDataConnectionServerEventArgs : DashboardServerEventArgs {
		public string ConnectionName { get; private set; }
		public string DataSourceName { get; private set; }
		public DataConnectionParametersBase ConnectionParameters { get; set; }
		public ConfigureDataConnectionServerEventArgs(string dashboardId, string connectionName, string dataSourceName)
			: base(dashboardId) {
			ConnectionName = connectionName;
			DataSourceName = dataSourceName;
		}
	}
	public class CustomFilterExpressionServerEventArgs : DashboardServerEventArgs {
		public string DataSourceComponentName { get; private set; }
		public string DataSourceName { get; private set; }
		public string TableName { get; private set; }
		public CriteriaOperator FilterExpression { get; set; }
		public CustomFilterExpressionServerEventArgs(string dashboardId, string dataSourceComponentName, string dataSourceName, string tableName)
			: base(dashboardId) {
			DataSourceComponentName = dataSourceComponentName;
			DataSourceName = dataSourceName;
			TableName = tableName;
		}
	}
	public class CustomParametersServerEventArgs : DashboardServerEventArgs {
		public List<IParameter> Parameters { get; internal set; }
		public CustomParametersServerEventArgs(string dashboardId, IEnumerable<IParameter> parameters)
			: base(dashboardId) {
			Parameters = parameters.ToList<IParameter>();
		}
	}
	public class DataLoadingServerEventArgs : DashboardServerEventArgs {
		public string DataSourceComponentName { get; private set; }
		public string DataSourceName { get; private set; }
		public object Data { get; set; }
		public DataLoadingServerEventArgs(string dashboardId, string dataSourceComponentName, string dataSourceName)
			: base(dashboardId) {
			DataSourceComponentName = dataSourceComponentName;
			DataSourceName = dataSourceName;
		}
	}
	public class DashboardLoadingServerEventArgs : DashboardServerEventArgs {
		public string DashboardXml { get; set; }
		public Type DashboardType { get; set; }
		public Dashboard Dashboard { get; set; }
		public DashboardLoadingServerEventArgs(string dashboardId)
			: base(dashboardId) {
		}
		internal IDashboardActivator GetActivator() {
			if(Dashboard != null)
				return new DashboardInstanceActivator(Dashboard);
			if(!string.IsNullOrEmpty(DashboardXml))
				return new DashboardXmlActivator(DashboardXml);
			if(DashboardType != null)
				return new DashboardTypeActivator(DashboardType);
			return null;
		}
	}
	public class DashboardLoadedServerEventArgs : DashboardServerEventArgs {
		readonly Dashboard dashboard;
		public Dashboard Dashboard { get { return dashboard; } }
		public DashboardLoadedServerEventArgs(string dashboardId, Dashboard dashboard)
			: base(dashboardId) {
				this.dashboard = dashboard;
		}
	}
	public class ConnectionErrorServerEventArgs : DashboardServerEventArgs {
		public string ConnectionName { get; private set; }
		public string DataSourceName { get;private set; }
		public Exception Exception { get; private set; }
		public bool Handled { get; set; }
		public bool Cancel { get; set; }
		public DataConnectionParametersBase ConnectionParameters { get; set; }
		public ConnectionErrorServerEventArgs(string dashboardId, string connectionName, string dataSourceName,Exception exception)
			: base(dashboardId) {
			ConnectionName = connectionName;
			DataSourceName = dataSourceName;
			Exception = exception;
		}
	}
	public class AllowLoadUnusedDataSourcesServerEventArgs : DashboardServerEventArgs {
		public bool Allow { get; set; }
		public AllowLoadUnusedDataSourcesServerEventArgs(string dashboardId)
			: base(dashboardId) {
		}
	}
	public class RequestAppConfigPatcherServiceEventArgs: EventArgs {
		public IConnectionStringsService AppConfigPatcherService { get; set; }
		public RequestAppConfigPatcherServiceEventArgs(){
		}
	}
	public class DashboardUnloadingEventArgs : EventArgs {
		public Dashboard Dashboard { get; private set; }
		public DashboardUnloadingEventArgs(Dashboard dashboard) {
			Guard.ArgumentNotNull(dashboard, "dashboard");
			Dashboard = dashboard;
		}
	}
	public class RequestCustomizationServicesEventArgs : EventArgs {
		public IDashboardSqlCustomizationService SqlCustomizationService { get; set; }
#if !DXPORTABLE
		public IDashboardExcelCustomizationService ExcelCustomizationService { get; set; }
#endif
		public IPlatformDependenciesService PlatformDependenciesService { get; set; }
		public IDataConnectionParametersProvider DataConnectionParametersProvider { get; set; }
		public IDataConnectionParametersProvider DefaultDataConnectionParametersProvider { get; private set; }
		public RequestCustomizationServicesEventArgs(IDataConnectionParametersProvider defaultDataConnectionParametersProvider) {
			Guard.ArgumentNotNull(defaultDataConnectionParametersProvider, "defaultProvider");
			this.DefaultDataConnectionParametersProvider = defaultDataConnectionParametersProvider;
		}
	}
	public class RequestWaitFormActivatorEventArgs : EventArgs {
		public IWaitFormActivator WaitFormActivator { get; set; }
	}
	public class RequestErrorHandlerEventArgs : EventArgs {
		public IErrorHandler ErrorHandler { get; set; }
	}
	public enum UnderlyingDataFormat {
		DataSet,
		ListAndSchema
	}
	public class RequestUnderlyingDataFormatEventArgs : EventArgs {
		public UnderlyingDataFormat? SupportedFormat { get; set; }
	}
	public class RequestDataLoaderEventArgs : EventArgs {
		public DataLoaderParameters Parameters { get; private set; }
		public IDashboardDataLoader DataLoader { get; set; }
		public RequestDataLoaderEventArgs(DataLoaderParameters parameters) {
			Parameters = parameters;
		}
	}
}
