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
using DevExpress.Data.Entity;
using DevExpress.DataAccess.Excel;
using DevExpress.DataAccess.Native.Excel;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Native.Sql.ConnectionStrategies;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Entity.ProjectModel;
namespace DevExpress.DataAccess.Wizard {
	public interface IDataSourceWizardClient : ISqlDataSourceWizardClient, IEFDataSourceWizardClient, IObjectDataSourceWizardClient, IExcelDataSourceWizardClient {
		IDataSourceNameCreationService DataSourceNameCreationService { get; }
		DataSourceTypes DataSourceTypes { get; }
	}
	public class DataSourceWizardClient : DataSourceWizardClientBase, IDataSourceWizardClient {
		readonly OperationMode operationMode;
		readonly EFDataSourceWizardClient efDataSourceWizardClient;
		readonly SqlDataSourceWizardClient sqlDataSourceWizardClient;
		readonly ExcelDataSourceWizardClient excelDataSourceWizardClient;
		readonly IDataSourceNameCreationService dataSourceNameCreationService;
		public DataSourceWizardClient(IConnectionStorageService connectionStorage, IParameterService parameterService, ISolutionTypesProvider solutionTypesProvider, IConnectionStringsProvider connectionStringsProvider)
			: this(connectionStorage, parameterService, solutionTypesProvider, connectionStringsProvider, new DBSchemaProvider()) { }
		public DataSourceWizardClient(IConnectionStorageService connectionStorage, IParameterService parameterService, ISolutionTypesProvider solutionTypesProvider, IConnectionStringsProvider connectionStringsProvider, IDBSchemaProvider dbSchemaProvider)
			: this(connectionStorage, parameterService, solutionTypesProvider, connectionStringsProvider, dbSchemaProvider, null, OperationMode.Both) { }
		public DataSourceWizardClient(IConnectionStorageService connectionStorage, IParameterService parameterService, ISolutionTypesProvider solutionTypesProvider, IConnectionStringsProvider connectionStringsProvider, IDBSchemaProvider dbSchemaProvider, IDataSourceNameCreationService dataSourceNameCreationService, OperationMode operationMode)
			: this(connectionStorage, parameterService, solutionTypesProvider, connectionStringsProvider, dbSchemaProvider, dataSourceNameCreationService, null, operationMode) { }
		public DataSourceWizardClient(IConnectionStorageService connectionStorage, IParameterService parameterService, ISolutionTypesProvider solutionTypesProvider, IConnectionStringsProvider connectionStringsProvider, IDBSchemaProvider dbSchemaProvider, IDataSourceNameCreationService dataSourceNameCreationService, IServiceProvider propertyGridServices, OperationMode operationMode)
			: this(connectionStorage, parameterService, solutionTypesProvider, connectionStringsProvider, dbSchemaProvider, dataSourceNameCreationService, propertyGridServices, operationMode, SqlWizardOptions.EnableCustomSql, null) { }
		public DataSourceWizardClient(IConnectionStorageService connectionStorage, IParameterService parameterService, ISolutionTypesProvider solutionTypesProvider, IConnectionStringsProvider connectionStringsProvider, IDBSchemaProvider dbSchemaProvider, IDataSourceNameCreationService dataSourceNameCreationService, IServiceProvider propertyGridServices, OperationMode operationMode, SqlWizardOptions sqlWizardOptions, IDisplayNameProvider displayNameProvider)
			: this(connectionStorage, parameterService, solutionTypesProvider, connectionStringsProvider, dbSchemaProvider, dataSourceNameCreationService, propertyGridServices, operationMode, sqlWizardOptions, displayNameProvider, new ExcelSchemaProvider(), DataSourceTypes.All) { }
		public DataSourceWizardClient(IConnectionStorageService connectionStorage, IParameterService parameterService, ISolutionTypesProvider solutionTypesProvider, IConnectionStringsProvider connectionStringsProvider, IDBSchemaProvider dbSchemaProvider, IDataSourceNameCreationService dataSourceNameCreationService, IServiceProvider propertyGridServices, OperationMode operationMode, SqlWizardOptions sqlWizardOptions, IDisplayNameProvider displayNameProvider, IExcelSchemaProvider excelSchemaProvider, DataSourceTypes dataSourceTypes)
			: this(connectionStorage, parameterService, solutionTypesProvider, connectionStringsProvider, dbSchemaProvider, dataSourceNameCreationService, propertyGridServices, operationMode, sqlWizardOptions, displayNameProvider, excelSchemaProvider, dataSourceTypes, null, new CustomQueryValidator()) { }
		public DataSourceWizardClient(IConnectionStorageService connectionStorage, IParameterService parameterService, ISolutionTypesProvider solutionTypesProvider, IConnectionStringsProvider connectionStringsProvider, IDBSchemaProvider dbSchemaProvider, IDataSourceNameCreationService dataSourceNameCreationService, IServiceProvider propertyGridServices, OperationMode operationMode, SqlWizardOptions sqlWizardOptions, IDisplayNameProvider displayNameProvider, IExcelSchemaProvider excelSchemaProvider, DataSourceTypes dataSourceTypes, IEnumerable<ProviderLookupItem> dataProviders, ICustomQueryValidator customQueryValidator)
			: base(parameterService, connectionStorage, connectionStringsProvider) {
			this.DataSourceTypes = dataSourceTypes;
			this.dataSourceNameCreationService = dataSourceNameCreationService;
			this.operationMode = operationMode;
			this.efDataSourceWizardClient = new EFDataSourceWizardClient(propertyGridServices, parameterService, solutionTypesProvider, connectionStringsProvider, connectionStorage);
			this.sqlDataSourceWizardClient = new SqlDataSourceWizardClient(connectionStorage, parameterService, connectionStringsProvider, propertyGridServices, dbSchemaProvider, sqlWizardOptions, displayNameProvider, dataProviders, customQueryValidator);
			this.excelDataSourceWizardClient = new ExcelDataSourceWizardClient(excelSchemaProvider);
			PropertyGridServices = propertyGridServices;
		}
		public DataSourceTypes DataSourceTypes { get; set; }
		public IDataSourceNameCreationService DataSourceNameCreationService {
			get { return dataSourceNameCreationService; }
		}
		public ISolutionTypesProvider SolutionTypesProvider { 
			get { return efDataSourceWizardClient.SolutionTypesProvider; }  
		}
		public OperationMode OperationMode { get { return operationMode; } }
		public IEnumerable<SqlDataConnection> DataConnections {
			get { return sqlDataSourceWizardClient.DataConnections; }
		}
		public IExcelSchemaProvider ExcelSchemaProvider {
			get { return excelDataSourceWizardClient.ExcelSchemaProvider; }
		}
		#region ISqlDataSourceWizardClient Members
		public IServiceProvider PropertyGridServices { get; set; }
		public SqlWizardOptions Options { get { return sqlDataSourceWizardClient.Options; } set { sqlDataSourceWizardClient.Options = value; } }
		IDisplayNameProvider ISqlDataSourceWizardClient.DisplayNameProvider { get { return sqlDataSourceWizardClient.DisplayNameProvider; } }
		List<ProviderLookupItem> ISqlDataSourceWizardClient.DataProviders { get { return sqlDataSourceWizardClient.DataProviders; } }
		public IDBSchemaProvider DBSchemaProvider { get { return sqlDataSourceWizardClient.DBSchemaProvider; } }
		public ICustomQueryValidator CustomQueryValidator { get { return sqlDataSourceWizardClient.CustomQueryValidator; } set { sqlDataSourceWizardClient.CustomQueryValidator = value; } }
		#endregion
	}
}
