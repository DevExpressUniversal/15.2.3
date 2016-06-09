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

using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Server;
using System;
using System.Collections;
namespace DevExpress.DashboardCommon.Service {
	public class DashboardService : IDashboardService, IDisposable, IDashboardServiceAdminHandlers {
		readonly IDashboardServer server;
		readonly bool disposeServer;
		protected IDashboardServer Server { get { return server; } }
		public DashboardService()
			: this(new DashboardServer(), true) {
		}
		public DashboardService(IDashboardServer server, bool disposeServer) {
			this.server = server;
			this.disposeServer = disposeServer;
		}
		public void Dispose() {
			if (disposeServer) {
				IDisposable serverDisposable = server as IDisposable;
				if(serverDisposable != null)
					serverDisposable.Dispose();
			}
		}
		protected virtual void OnSingleFilterDefaultValue(SingleFilterDefaultValueEventArgs e) {
		}
		protected virtual void OnFilterElementDefaultValues(FilterElementDefaultValuesEventArgs e) {
		}
		protected virtual void OnRangeFilterDefaultValue(RangeFilterDefaultValueEventArgs e) {
		}
		protected virtual void ConfigureDataConnection(ConfigureDataConnectionServerEventArgs e) {
		}
		protected virtual void CustomFilterExpression(CustomFilterExpressionServerEventArgs e) {
		}
		protected virtual void CustomParameters(CustomParametersServerEventArgs e) {
		}
		protected virtual void DataLoading(DataLoadingServerEventArgs e) {
		}
		protected virtual void DashboardLoading(DashboardLoadingServerEventArgs e) {
		}
		protected virtual void DashboardLoaded(DashboardLoadedServerEventArgs e) {
		}
		protected virtual void ConnectionError(ConnectionErrorServerEventArgs e) {
		}
		protected virtual void AllowLoadUnusedDataSources(AllowLoadUnusedDataSourcesServerEventArgs e) {
		}
		protected virtual void OnDashboardUnloading(DashboardUnloadingEventArgs e) {
			e.Dashboard.Dispose();
		}
		protected virtual void RequestCustomizationServices(RequestCustomizationServicesEventArgs e) {
		}
		protected virtual void RequestWaitFormActivator(RequestWaitFormActivatorEventArgs e) {
		}
		protected virtual void RequestErrorHandler(RequestErrorHandlerEventArgs e) {
		}
		protected virtual void RequestUnderlyingDataFormat(RequestUnderlyingDataFormatEventArgs e) {
		}
		protected virtual void OnRequestDataLoader(RequestDataLoaderEventArgs e) {
		}
		protected virtual void OnRequestAppConfigPatcherService(RequestAppConfigPatcherServiceEventArgs e) {
		}
		protected virtual void OnValidateCustomSqlQuery(ValidateDashboardCustomSqlQueryEventArgs e) {
		}
		#region IDashboardServiceAdminHandlers members
		void IDashboardServiceAdminHandlers.OnSingleFilterDefaultValue(object sender, SingleFilterDefaultValueEventArgs e) {
			OnSingleFilterDefaultValue(e);
		}
		void IDashboardServiceAdminHandlers.OnFilterElementDefaultValues(object sender, FilterElementDefaultValuesEventArgs e) {
			OnFilterElementDefaultValues(e);
		}
		void IDashboardServiceAdminHandlers.OnRangeFilterDefaultValue(object sender, RangeFilterDefaultValueEventArgs e) {
			OnRangeFilterDefaultValue(e);
		}
		void IDashboardServiceAdminHandlers.OnConfigureDataConnection(object sender, ConfigureDataConnectionServerEventArgs e) {
			ConfigureDataConnection(e);
		}
		void IDashboardServiceAdminHandlers.OnCustomFilterExpression(object sender, CustomFilterExpressionServerEventArgs e) {
			CustomFilterExpression(e);
		}
		void IDashboardServiceAdminHandlers.OnCustomParameters(object sender, CustomParametersServerEventArgs e) {
			CustomParameters(e);
		}
		void IDashboardServiceAdminHandlers.OnDataLoading(object sender, DataLoadingServerEventArgs e) {
			DataLoading(e);
		}
		void IDashboardServiceAdminHandlers.OnDashboardLoading(object sender, DashboardLoadingServerEventArgs e) {
			DashboardLoading(e);
		}
		void IDashboardServiceAdminHandlers.OnDashboardLoaded(object sender, DashboardLoadedServerEventArgs e) {
			DashboardLoaded(e);
		}
		void IDashboardServiceAdminHandlers.OnConnectionError(object sender, ConnectionErrorServerEventArgs e) {
			ConnectionError(e);
		}
		void IDashboardServiceAdminHandlers.OnAllowLoadUnusedDataSources(object sender, AllowLoadUnusedDataSourcesServerEventArgs e) {
			AllowLoadUnusedDataSources(e);
		}
		void IDashboardServiceAdminHandlers.OnDashboardUnloading(object sender, DashboardUnloadingEventArgs e) {
			OnDashboardUnloading(e);
		}
		void IDashboardServiceAdminHandlers.OnRequestCustomizationServices(object sender, RequestCustomizationServicesEventArgs e) {
			RequestCustomizationServices(e);
		}
		void IDashboardServiceAdminHandlers.OnRequestWaitFormActivator(object sender, RequestWaitFormActivatorEventArgs e) {
			RequestWaitFormActivator(e);
		}
		void IDashboardServiceAdminHandlers.OnRequestErrorHandler(object sender, RequestErrorHandlerEventArgs e) {
			RequestErrorHandler(e);
		}
		void IDashboardServiceAdminHandlers.OnRequestUnderlyingDataFormat(object sender, RequestUnderlyingDataFormatEventArgs e) {
			RequestUnderlyingDataFormat(e);
		}
		void IDashboardServiceAdminHandlers.OnRequestDataLoader(object sender, RequestDataLoaderEventArgs e) {
			OnRequestDataLoader(e);
		}
		void IDashboardServiceAdminHandlers.OnRequestAppConfigPatcherService(object sender, RequestAppConfigPatcherServiceEventArgs e) {
			OnRequestAppConfigPatcherService(e);
		}
		void IDashboardServiceAdminHandlers.OnValidateCustomSqlQuery(object sender, ValidateDashboardCustomSqlQueryEventArgs e) {
			OnValidateCustomSqlQuery(e);
		}
		#endregion
		#region IDashboardService members
		InitializeResult IDashboardService.Initialize(InitializeSessionArgs initializeArgs) {
			initializeArgs.Settings = initializeArgs.Settings ?? SessionSettings.Default;
			InitializeResult result = new InitializeResult();
			InitializeOperation operation = new InitializeOperation(server, this, initializeArgs);
			operation.Execute(result);
			return result;
		}
		DashboardServiceResult IDashboardService.PerformAction(PerformActionArgs performActionArgs) {
			DashboardServiceResult result = new DashboardServiceResult();
			PerformActionOperation operation = new PerformActionOperation(server, this, performActionArgs);
			operation.Execute(result);
			return result;
		}
		DashboardServiceResult IDashboardService.ReloadData(ReloadDataArgs  reloadDataArgs) {
			DashboardServiceResult result = new DashboardServiceResult();
			ReloadDataOperation operation = new ReloadDataOperation(server, this, reloadDataArgs);
			operation.Execute(result);
			return result;
		}
		DashboardServiceResult IDashboardService.Export(ExportArgs exportArgs) {
			DashboardServiceResult result = new DashboardServiceResult();
			ExportOperation operation = new ExportOperation(server, this, exportArgs);
			operation.Execute(result);
			return result;
		}
		RefreshResult IDashboardService.Refresh(RefreshArgs args) {
			RefreshResult result = new RefreshResult();
			RefreshOperation operation = new RefreshOperation(server, this, args);
			operation.Execute(result);
			return result;
		}
		GetExportReportResult IDashboardService.GetExportReport(ExportArgs exportArgs) {
			GetExportReportResult result = new GetExportReportResult();
			GetExportReportOperation operation = new GetExportReportOperation(server, this, exportArgs);
			operation.Execute(result);
			return result;
		}
		#endregion
	}
}
