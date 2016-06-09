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

using System.Threading;
using System.Threading.Tasks;
using DevExpress.DataAccess.Native.Sql.ConnectionProviders;
using DevExpress.DataAccess.Sql;
using DevExpress.Utils;
using DevExpress.Xpo.DB;
namespace DevExpress.DataAccess.Native.Sql {
	public class DBSchemaProviderAsyncHelper {
		readonly IDataConnectionParametersService dataConnectionParameterService;
		public DBSchemaProviderAsyncHelper(IDataConnectionParametersService dataConnectionParameterService) {
			this.dataConnectionParameterService = dataConnectionParameterService;
		}
		#region Implementation of IDBSchemaProviderAsync
		public Task<DBSchema> GetSchemaAsync(SqlDataConnection connection, CancellationToken cancellationToken) {
			return OpenConnection(connection, cancellationToken).ContinueWith(task => {
				if(task.IsFaulted && task.Exception != null)
					throw task.Exception;
				return connection.GetDBSchema();
			}, cancellationToken, TaskContinuationOptions.None, TaskScheduler.Default);
		}
		public Task<DBSchema> GetSchemaAsync(SqlDataConnection connection, string[] tableList, CancellationToken cancellationToken) {
			return OpenConnection(connection, cancellationToken).ContinueWith(task => {
				if(task.IsFaulted && task.Exception != null)
					throw task.Exception;
				return connection.GetDBSchema(tableList);
			}, cancellationToken, TaskContinuationOptions.None, TaskScheduler.Default);
		}
		public Task<DBSchema> GetStoredProceduresAsync(SqlDataConnection connection, string[] proceduresList, CancellationToken cancellationToken) {
			return OpenConnection(connection, cancellationToken).ContinueWith(task => {
				if(task.IsFaulted && task.Exception != null)
					throw task.Exception;
				ISupportStoredProc storedProcProvider = connection.SchemaExplorer as ISupportStoredProc;
				if(storedProcProvider != null)
					return new DBSchema(new DBTable[0], new DBTable[0], storedProcProvider.GetStoredProcedures(proceduresList));
				return new DBSchema();
			}, cancellationToken, TaskContinuationOptions.None, TaskScheduler.Default);
		}
		#endregion
		Task OpenConnection(SqlDataConnection connection, CancellationToken cancellationToken) {
			return Task.Factory.StartNew(() => {
				Guard.ArgumentNotNull(connection, "connection");
				if(!connection.IsConnected)
					connection.CreateDataStore(this.dataConnectionParameterService, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}
	}
}
