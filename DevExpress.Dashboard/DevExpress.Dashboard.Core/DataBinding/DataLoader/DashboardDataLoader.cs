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
using System.ComponentModel.Design;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.Utils;
using DevExpress.DataAccess;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Localization;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Sql;
#if !DXPORTABLE
using DevExpress.DataAccess.Wizard.Native;
using DevExpress.DataAccess.Excel;
using DevExpress.DataAccess.Native.Excel;
#endif
using DevExpress.Xpo.DB.Helpers;
using DevExpress.Compatibility.System.ComponentModel.Design;
namespace DevExpress.DashboardCommon.Native {
	public class DataSourceLoadingResult {
		public static DataSourceLoadingResult Success {
			get {
				return new DataSourceLoadingResult(null) { ResultType = DataSourceLoadingResultType.Success };
			}
		}
		readonly List<DataLoaderError> errors;
		public DataSourceLoadingResultType ResultType { get; set; }
		public List<DataLoaderError> Errors {
			get { return errors; }
		}
		public DataSourceLoadingResult(List<DataLoaderError> errors) {
			this.errors = errors;
		}
	}
	public class DataSourceDataLoadedEventArgs : EventArgs {
		readonly string dataSourceName;
		public string DataSourceName {
			get { return dataSourceName; }
		}
		public DataSourceDataLoadedEventArgs(string dataSourceName) {
			this.dataSourceName = dataSourceName;
		}
	}
	public interface ISchemaLoader : ISupportCancel {
		void LoadSchema();
	}
	public delegate void DataSourceDataLoadedEventHandler(object sender, DataSourceDataLoadedEventArgs args);
	public interface IDashboardCustomSqlQueryValidator {
		void Validate(ValidateDashboardCustomSqlQueryEventArgs args);
	}
	public interface IDataConnectionParametersProvider  {
		object RaiseDataLoading(string dataSourceComponentName, string dataSourceName, object data);
		IDBSchemaProvider GetDbSchemaProvider(SqlDataConnection dataConnection);
		CriteriaOperator RaiseCustomFilterExpression(CustomFilterExpressionEventArgs eventArgs);
		DataConnectionParametersBase RaiseConfigureDataConnection(string connectionName, string dataSourceName, DataConnectionParametersBase parameters);
		DataConnectionParametersBase RaiseHandleConnectionError(string dataSourceName, ConnectionErrorEventArgs eventArgs);
	}
	public interface IActualParametersProvider {
		IEnumerable<IParameter> GetParameters();
		IEnumerable<IParameter> GetActualParameters();
	}
	public class EmptyParametersProvider : IActualParametersProvider {
		IEnumerable<IParameter> IActualParametersProvider.GetParameters() {
			yield break;
		}
		IEnumerable<IParameter> IActualParametersProvider.GetActualParameters() {
			yield break;
		}
	}
	public interface IPlatformDependenciesService {
		void DoEvents();
	}
	public interface IDashboardSqlCustomizationService : IDataConnectionParametersService {
		IWaitFormActivator CurrentWaitFormActivator { get; set; }
		void SetNativeProvider(IDataConnectionParametersService nativeProvider);
	}
#if !DXPORTABLE
	public interface IDashboardExcelCustomizationService : IExcelOptionsCustomizationService {
		IWaitFormActivator CurrentWaitFormActivator { get; set; }
	}
#endif
	public class DashboardDataLoader : IDashboardDataLoader {
		readonly IWaitFormActivator fWaitFormActivator;
		readonly IDataConnectionParametersProvider dataConnectionParametersProvider;
		readonly IDashboardSqlCustomizationService sqlCustomizationService;
#if !DXPORTABLE
		readonly IDashboardExcelCustomizationService excelCustomizationService;
#endif
		readonly IConnectionStringsService appConfigPatcherService;
		readonly IDashboardCustomSqlQueryValidator customSqlQueryValidator;
		readonly IPlatformDependenciesService platformDependenciesService;
		public IWaitFormActivator WaitFormActivator {
			get { return fWaitFormActivator ?? EmptyWaitFormActivator.Instance; }
		}
		protected IDataConnectionParametersProvider DataConnectionParametersProvider { get { return dataConnectionParametersProvider; } }
		protected IDashboardSqlCustomizationService SqlCustomizationService { get { return sqlCustomizationService; } }
#if !DXPORTABLE
		protected IDashboardExcelCustomizationService ExcelCustomizationService { get { return excelCustomizationService; } }
#endif
		protected IPlatformDependenciesService PlatformDependenciesService { get { return platformDependenciesService;} }
		protected IConnectionStringsService AppConfigPatcherService { get { return appConfigPatcherService; }  }
		protected IDashboardCustomSqlQueryValidator CustomSqlQueryValidator { get { return customSqlQueryValidator; } }
		public event DataSourceDataLoadedEventHandler DataLoaded;
		public DashboardDataLoader(
			IDataConnectionParametersProvider dataConnectionParametersProvider,
			IDashboardSqlCustomizationService sqlCustomizationService,
#if !DXPORTABLE
			IDashboardExcelCustomizationService excelCustomizationService,
#endif
			IWaitFormActivator waitFormActivator,
			IErrorHandler errorHandler,
			IConnectionStringsService appConfigPatcherService,
			IDashboardCustomSqlQueryValidator customSqlQueryValidator,
			IPlatformDependenciesService platformDependenciesService
		) {
			this.dataConnectionParametersProvider = dataConnectionParametersProvider;
			this.sqlCustomizationService = sqlCustomizationService;
#if !DXPORTABLE
			this.excelCustomizationService = excelCustomizationService;
#endif
			this.fWaitFormActivator = waitFormActivator;
			this.errorHandler = errorHandler;
			this.appConfigPatcherService = appConfigPatcherService;
			this.customSqlQueryValidator = customSqlQueryValidator;
			this.platformDependenciesService = platformDependenciesService;
		}
		public DashboardDataLoader(
			IDataConnectionParametersProvider dataConnectionParametersProvider,
			IDashboardSqlCustomizationService sqlCustomizationService,
#if !DXPORTABLE
			IDashboardExcelCustomizationService excelCustomizationService,
#endif
			IWaitFormActivator waitFormActivator,
			IErrorHandler errorHandler
		)
			: this(
				dataConnectionParametersProvider, sqlCustomizationService,
#if !DXPORTABLE
				excelCustomizationService,
#endif
				waitFormActivator, errorHandler, null, null, null
			) {
		}
		public DashboardDataLoader(
			IDataConnectionParametersProvider dataConnectionParametersProvider,
			IDashboardSqlCustomizationService sqlCustomizationService,
#if !DXPORTABLE
			IDashboardExcelCustomizationService excelCustomizationService,
#endif
			IWaitFormActivator waitFormActivator
		)
			: this(
				dataConnectionParametersProvider, sqlCustomizationService,
#if !DXPORTABLE
				excelCustomizationService,
#endif
				waitFormActivator, null, null, null, null
			) {
		}
		public DashboardDataLoader(
			IDataConnectionParametersProvider dataConnectionParametersProvider, IDashboardSqlCustomizationService sqlCustomizationService
#if !DXPORTABLE
			,IDashboardExcelCustomizationService excelCustomizationService
#endif
		)
			: this(
				dataConnectionParametersProvider, sqlCustomizationService,
#if !DXPORTABLE
				excelCustomizationService,
#endif
				null, null, null, null, null
			) {
		}
		internal DashboardDataLoader(DataLoaderParameters parameters)
			: this(
			parameters.DataConnectionParametersProvider, parameters.SqlCustomizationService,
#if !DXPORTABLE
			parameters.ExcelCustomizationService,
#endif
			parameters.WaitFormActivator, parameters.ErrorHandler, parameters.ConfigPatcherService, parameters.CustomSqlQueryValidator, parameters.PlatformDependenciesService) {
		}
		protected void DataSourceDataLoaded(object sender, EventArgs e) {
			IDataSource dataSource = sender as IDataSource;
			if(dataSource != null) {
				RaiseDataLoaded(dataSource);
			}
			WaitFormActivator.EnableCancelButton(false);
		}
		protected void RaiseDataLoaded(IDataSource dataSource) {
			if(DataLoaded != null)
				DataLoaded(this, new DataSourceDataLoadedEventArgs(dataSource.Name));
		}	
#if DEBUGTEST
		public static bool AbortDataLoading = false;
		public static bool ThrowExceptionOnError = false;
#endif
		readonly IErrorHandler errorHandler;
		public IErrorHandler ErrorHandler { get { return errorHandler ?? EmptyErrorHandler.Instance; } }
		void FillDataSource(IEnumerable<IParameter> parameters, IDataComponent dataComponent, CancellationToken token) {
			DashboardSqlDataSource sqlDataSource = dataComponent as DashboardSqlDataSource;
#if !DXPORTABLE
			DashboardExcelDataSource excelDataSource = dataComponent as DashboardExcelDataSource;
#endif
			if(sqlDataSource != null)
				sqlDataSource.FillSync(parameters, PlatformDependenciesService, token);
#if !DXPORTABLE
			else if(excelDataSource != null)
				excelDataSource.FillSync(parameters, PlatformDependenciesService, token);
#endif
			else
				dataComponent.Fill(parameters);
		}
		bool LoadDataCore(IEnumerable<IDashboardDataSource> dataSources, IEnumerable<IParameter> parameters, List<DataLoaderError> errors) {
			try {
				WaitFormActivator.ShowWaitForm(true, false, true);
				CancellationTokenSource cts = new CancellationTokenSource();
				CancellationToken token = cts.Token;
#if !DXPORTABLE
				CancellationTokenHook hook = new CancellationTokenHook(cts);
				WaitFormActivator.SetWaitFormObject(hook);
#endif
				WaitFormActivator.EnableCancelButton(true);
				WaitFormActivator.SetWaitFormDescription(DataAccessLocalizer.GetString(DataAccessStringId.ConnectingToDatabaseMessage));
				foreach(IDashboardDataSource dataSource in dataSources) {
					try {
						if(SqlCustomizationService != null)
							SqlCustomizationService.CurrentWaitFormActivator = WaitFormActivator;
#if !DXPORTABLE
						if(ExcelCustomizationService != null)
							ExcelCustomizationService.CurrentWaitFormActivator = WaitFormActivator;
#endif
						SqlDataSource sqlDatasource = dataSource as SqlDataSource;
						if(sqlDatasource != null)
							SubscribeSqlDataSourceEvents(sqlDatasource);
#if !DXPORTABLE
						DashboardOlapDataSource olapDataSource = dataSource as DashboardOlapDataSource;
						if(olapDataSource != null)
							SubscribeOlapDataSourceEvents(olapDataSource);
						DashboardObjectDataSource objectDataSource = dataSource as DashboardObjectDataSource;
						if(objectDataSource != null) {
							object data = objectDataSource.RequestData();
							if(DataConnectionParametersProvider != null) {
								data = DataConnectionParametersProvider.RaiseDataLoading(objectDataSource.ComponentName, objectDataSource.Name, data);
							}
							if(data != null)
								objectDataSource.SetData(data);
						}
						DashboardExcelDataSource excelDataSource = dataSource as DashboardExcelDataSource;
						if(excelDataSource != null)
							SubscribeExcelDataSourceEvents(excelDataSource);
#endif
						IDataComponent dataComponent = dataSource as IDataComponent;
						if(dataComponent != null)
							FillDataSource(parameters, dataComponent, token);
						if(token.IsCancellationRequested)
							break;
					} catch(TaskCanceledException) {
						return false;
					} catch(Exception ex) {
						Exception innerException = ex.InnerException;
						errors.Add(new DataLoaderError(DataLoaderErrorType.Connection, dataSource.Name, innerException == null ? ex.Message : innerException.Message));
					} finally {
						SqlDataSource sqlDatasource = dataSource as SqlDataSource;
						if(sqlDatasource != null)
							UnsubscribeSqlDataSourceEvents(sqlDatasource);
#if !DXPORTABLE
						DashboardOlapDataSource olapDataSource = dataSource as DashboardOlapDataSource;
						if(olapDataSource != null)
							UnsubscribeOlapDataSourceEvents(olapDataSource);
						DashboardExcelDataSource excelDataSource = dataSource as DashboardExcelDataSource;
						if(excelDataSource != null)
							UnsubscribeExcelDataSourceEvents(excelDataSource);
						if(ExcelCustomizationService != null)
							ExcelCustomizationService.CurrentWaitFormActivator = null;
#endif
						if(SqlCustomizationService != null)
							SqlCustomizationService.CurrentWaitFormActivator = null;
					}
				}
			} finally {
				WaitFormActivator.CloseWaitForm(false, 0, true);
			}
			return true;
		}	  
		public virtual DataSourceLoadingResult LoadData(IEnumerable<IDashboardDataSource> dataSources, IEnumerable<IParameter> parameters) {
#if DEBUGTEST
			if(AbortDataLoading)
				return new DataSourceLoadingResult(new List<DataLoaderError>()) { ResultType = DataSourceLoadingResultType.Success };
#endif
			bool loadDataResult = true;
			List<DataLoaderError> errors = new List<DataLoaderError>();
#if !DXPORTABLE
			bool containsObjectDataSource = dataSources.OfType<DashboardObjectDataSource>().Any();
#endif
			try {
#if !DXPORTABLE
				if(containsObjectDataSource)
					PatchVSAppConfig();
#endif
				if (dataSources.Any()) 
					loadDataResult = LoadDataCore(dataSources, parameters, errors);
			} finally {
#if !DXPORTABLE
				if(containsObjectDataSource)
					RestoreVSAppConfig();
#endif
			}
			DataSourceLoadingResult result = new DataSourceLoadingResult(errors);
			if(!loadDataResult) {
				result.ResultType = DataSourceLoadingResultType.Aborted;
			}
			else if(result.Errors.Count > 0) {
				result.ResultType = DataSourceLoadingResultType.Errors;
#if DEBUGTEST
				if(ThrowExceptionOnError) {
					StringBuilder sb  = new StringBuilder();
					foreach(DataLoaderError error in result.Errors){
						sb.Append(string.Format("Error in {0}:{1}", error.DataSourceName,error.Message));
					}
					throw new Exception(sb.ToString());
				}
#endif
				ErrorHandler.ShowDataSourceLoadingErrors(result.Errors);
			} 
			else
				result.ResultType = DataSourceLoadingResultType.Success;
			return result;
		}
		void PatchVSAppConfig() {			
			if(AppConfigPatcherService!= null)
				AppConfigPatcherService.PatchConnection();
		}
		void RestoreVSAppConfig() {
			if(AppConfigPatcherService != null)
				AppConfigPatcherService.RestoreConnection();
		}			
		void UnsubscribeSqlDataSourceEvents(SqlDataSource sqlDataSource) {
			sqlDataSource.ConfigureDataConnection -= sqlDatasource_ConfigureDataConnection;
			sqlDataSource.ConnectionError -= sqlDataSource_ConnectionError;
			sqlDataSource.ValidateCustomSqlQuery -= sqlDataSource_ValidateCustomSqlQuery;
			if(handler != null) {
				sqlDataSource.CustomizeFilterExpression -= handler;
				handler = null;
			}
			SqlDataSourceAwareDataConnectionParametersService service = ((IServiceContainer)sqlDataSource).GetService(typeof(IDataConnectionParametersService)) as SqlDataSourceAwareDataConnectionParametersService;
			if(service != null) {
				service.Unregister(sqlDataSource);
			}			
		}
		CustomizeFilterExpressionEventHandler handler;
		void SubscribeSqlDataSourceEvents(SqlDataSource sqlDataSource) {
			sqlDataSource.ConfigureDataConnection += sqlDatasource_ConfigureDataConnection;
			sqlDataSource.ConnectionError += sqlDataSource_ConnectionError;
			sqlDataSource.ValidateCustomSqlQuery += sqlDataSource_ValidateCustomSqlQuery;
			handler = (sender, e) => 
				sqlDataSource_CustomFilterExpression(sqlDataSource, e);
			sqlDataSource.CustomizeFilterExpression += handler;
			if (SqlCustomizationService != null)
				new SqlDataSourceAwareDataConnectionParametersService().Register(SqlCustomizationService, sqlDataSource);
		}
#if !DXPORTABLE
		void SubscribeExcelDataSourceEvents(DashboardExcelDataSource excelDataSource) {
			excelDataSource.BeforeFill += excelDataSource_BeforeFill;
			if(ExcelCustomizationService != null)
				new ExcelDataSourceAwareExcelOptionsCustomizationService().Register(ExcelCustomizationService, excelDataSource);
		}
		void UnsubscribeExcelDataSourceEvents(DashboardExcelDataSource excelDataSource) {
			excelDataSource.BeforeFill -= excelDataSource_BeforeFill;
			ExcelDataSourceAwareExcelOptionsCustomizationService service = ((IServiceContainer)excelDataSource).GetService(typeof(IExcelOptionsCustomizationService)) as ExcelDataSourceAwareExcelOptionsCustomizationService;
			if(service != null) {
				service.Unregister(excelDataSource);
			}			
		}
		void excelDataSource_BeforeFill(object sender, DataAccess.Excel.BeforeFillEventArgs e) {
			ExcelSourceOptions excelSourceOptions = e.SourceOptions as ExcelSourceOptions;
			ExcelDataSourceConnectionParameters parameters = new ExcelDataSourceConnectionParameters(e.FileName, excelSourceOptions != null ? excelSourceOptions.Password : null);
			;
			if(DataConnectionParametersProvider != null)
				DataConnectionParametersProvider.RaiseConfigureDataConnection(null, ((DashboardExcelDataSource)sender).Name, parameters);
			e.FileName = parameters.FileName;
		}
		void UnsubscribeOlapDataSourceEvents(DashboardOlapDataSource olapDataSource) {
			olapDataSource.ConfigureOlapConnection -= olapDataSource_ConfigureOlapConnection;
			olapDataSource.ConnectionError -= olapDataSource_ConnectionError;			
		}
		void SubscribeOlapDataSourceEvents(DashboardOlapDataSource olapDataSource) {
			olapDataSource.ConfigureOlapConnection += olapDataSource_ConfigureOlapConnection;
			olapDataSource.ConnectionError += olapDataSource_ConnectionError;			
		}		
		void olapDataSource_ConfigureOlapConnection(object sender, ConfigureOlapConnectionEventArgs eventArgs) {
			OlapConnectionParameters parameters = new OlapConnectionParameters(eventArgs.ConnectionString);
			if(DataConnectionParametersProvider != null)
				DataConnectionParametersProvider.RaiseConfigureDataConnection(eventArgs.ConnectionName, ((DashboardOlapDataSource)sender).Name, parameters);
			eventArgs.ConnectionString = parameters.ConnectionString;
		}
		void olapDataSource_ConnectionError(object sender, ConnectionErrorEventArgs e) {
			if (DataConnectionParametersProvider != null)
				DataConnectionParametersProvider.RaiseHandleConnectionError(((DashboardOlapDataSource)sender).Name, e);
		}
#endif
		void sqlDataSource_CustomFilterExpression(object sender, CustomizeFilterExpressionEventArgs e) {
			if(DataConnectionParametersProvider != null) {
				DashboardSqlDataSource dashboardSqlDataSource = sender as DashboardSqlDataSource;
				if(dashboardSqlDataSource != null)
					e.FilterExpression = DataConnectionParametersProvider.RaiseCustomFilterExpression(new CustomFilterExpressionEventArgs(dashboardSqlDataSource.ComponentName, dashboardSqlDataSource.Name, e.QueryName) { FilterExpression = e.FilterExpression });
			}
		}   
		void sqlDataSource_ConnectionError(object sender, ConnectionErrorEventArgs e) {
			if(DataConnectionParametersProvider != null)
				DataConnectionParametersProvider.RaiseHandleConnectionError(((DashboardSqlDataSource)sender).Name, e);
		}
		void sqlDatasource_ConfigureDataConnection(object sender, ConfigureDataConnectionEventArgs e) {
			if(DataConnectionParametersProvider != null)
				e.ConnectionParameters = DataConnectionParametersProvider.RaiseConfigureDataConnection(e.ConnectionName, ((DashboardSqlDataSource)sender).Name, e.ConnectionParameters);
		}
		void sqlDataSource_ValidateCustomSqlQuery(object sender, ValidateCustomSqlQueryEventArgs e) {
			if (CustomSqlQueryValidator != null)
				CustomSqlQueryValidator.Validate(new ValidateDashboardCustomSqlQueryEventArgs((DashboardSqlDataSource)sender, e));
		}
	}
	public class SqlDataSourceAwareDataConnectionParametersService  {
		object oldService;
		public void Register(IDashboardSqlCustomizationService service, SqlDataSource sqlDataSource) {
			oldService = sqlDataSource.GetService(typeof(IDataConnectionParametersService));
			service.SetNativeProvider(sqlDataSource);
			((IServiceContainer)sqlDataSource).RemoveService(typeof(IDataConnectionParametersService));
			((IServiceContainer)sqlDataSource).AddService(typeof(IDataConnectionParametersService), service);
		}			 
		public  void Unregister(SqlDataSource sqlDataSource) {
			((IServiceContainer)sqlDataSource).RemoveService(typeof(IDataConnectionParametersService));
			((IServiceContainer)sqlDataSource).AddService(typeof(IDataConnectionParametersService), oldService);		
		}
	}
#if !DXPORTABLE
	public class ExcelDataSourceAwareExcelOptionsCustomizationService {
		object oldService;
		public void Register(IExcelOptionsCustomizationService service, ExcelDataSource excelDataSource) {
			oldService = excelDataSource.GetService(typeof(IExcelOptionsCustomizationService));
			((IServiceContainer)excelDataSource).RemoveService(typeof(IExcelOptionsCustomizationService));
			((IServiceContainer)excelDataSource).AddService(typeof(IExcelOptionsCustomizationService), service);
		}
		public void Unregister(ExcelDataSource excelDataSource) {
			((IServiceContainer)excelDataSource).RemoveService(typeof(IExcelOptionsCustomizationService));
			((IServiceContainer)excelDataSource).AddService(typeof(IExcelOptionsCustomizationService), oldService);
		}
	}
#endif
}
