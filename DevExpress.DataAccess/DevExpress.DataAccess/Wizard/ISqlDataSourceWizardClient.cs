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
using System.ComponentModel.Design;
using System.Linq;
using DevExpress.Data;
using DevExpress.Data.Entity;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Native.Sql.ConnectionStrategies;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Wizard.Services;
namespace DevExpress.DataAccess.Wizard {
	public interface ISqlDataSourceWizardClient : IDataSourceWizardClientBase {
		IServiceProvider PropertyGridServices { get; }
		IDBSchemaProvider DBSchemaProvider { get; }
		IEnumerable<SqlDataConnection> DataConnections { get; }
		SqlWizardOptions Options { get; }
		IDisplayNameProvider DisplayNameProvider { get; }
		List<ProviderLookupItem> DataProviders { get; }
		ICustomQueryValidator CustomQueryValidator { get; }
	}
	[Flags]
	public enum SqlWizardOptions {
		None = 0,
		AlwaysSaveCredentials = 1,
		DisableNewConnections = 2,
		EnableCustomSql = 4,
		QueryBuilderLight = 8
	}
	public class SqlDataSourceWizardClient : DataSourceWizardClientBase, ISqlDataSourceWizardClient {
		IEnumerable<SqlDataConnection> dataConnections;
		public SqlDataSourceWizardClient(IConnectionStorageService connectionProvider, IParameterService parameterService)
			: this(connectionProvider, parameterService, null) { }
		public SqlDataSourceWizardClient(IConnectionStorageService connectionProvider, IParameterService parameterService, IDBSchemaProvider dbSchemaProvider)
			: this(connectionProvider, parameterService, null, dbSchemaProvider) { }
		public SqlDataSourceWizardClient(IConnectionStorageService connectionProvider, IParameterService parameterService, IServiceProvider propertyGridServices, IDBSchemaProvider dbSchemaProvider)
			: this(connectionProvider, parameterService, null, propertyGridServices, dbSchemaProvider, SqlWizardOptions.None, null) { }
		public SqlDataSourceWizardClient(IConnectionStorageService connectionProvider, IParameterService parameterService, IConnectionStringsProvider connectionStringsProvider, IServiceProvider propertyGridServices, IDBSchemaProvider dbSchemaProvider)
			: this(connectionProvider, parameterService, connectionStringsProvider, propertyGridServices, dbSchemaProvider, SqlWizardOptions.None, null) { }
		public SqlDataSourceWizardClient(IConnectionStorageService connectionProvider, IParameterService parameterService, IConnectionStringsProvider connectionStringsProvider, IServiceProvider propertyGridServices, IDBSchemaProvider dbSchemaProvider, SqlWizardOptions options, IDisplayNameProvider displayNameProvider)
			: this(connectionProvider, parameterService, connectionStringsProvider, propertyGridServices, dbSchemaProvider, options, displayNameProvider, null, null) { }
		public SqlDataSourceWizardClient(IConnectionStorageService connectionProvider, IParameterService parameterService, IConnectionStringsProvider connectionStringsProvider, IServiceProvider propertyGridServices, IDBSchemaProvider dbSchemaProvider, SqlWizardOptions options, IDisplayNameProvider displayNameProvider, IEnumerable<ProviderLookupItem> dataProviders, ICustomQueryValidator customQueryValidator)
			: base(parameterService, connectionProvider, connectionStringsProvider) {
			PropertyGridServices = propertyGridServices ?? new ServiceContainer();
			DBSchemaProvider = dbSchemaProvider ?? new DBSchemaProvider();
			Options = options;
			DisplayNameProvider = displayNameProvider;
			DataProviders = (dataProviders ?? ProviderLookupItem.GetPredefinedItems()).ToList();
			CustomQueryValidator = customQueryValidator;
		}
		public IEnumerable<SqlDataConnection> DataConnections {
			get {
				if(dataConnections != null)
					return dataConnections;
				dataConnections = ConnectionStorageService != null ? ConnectionStorageService.GetConnections() : new SqlDataConnection[0];
				return dataConnections;
			}
		}
		public IServiceProvider PropertyGridServices { get; private set; }
		public IDBSchemaProvider DBSchemaProvider { get; set; }
		public SqlWizardOptions Options { get; set; }
		public IDisplayNameProvider DisplayNameProvider { get; private set; }
		public List<ProviderLookupItem> DataProviders { get; private set; }
		public ICustomQueryValidator CustomQueryValidator { get; set; }
	}
}
