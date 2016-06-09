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
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevExpress.Data;
using DevExpress.DataAccess.Localization;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Wizard.Native;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Utils;
using DevExpress.Xpo.DB;
namespace DevExpress.DataAccess.Wizard {
	public class UIDataLoader  {		
		readonly IWaitFormActivator waitFormActivator;
		readonly IExceptionHandler exceptionHandler;
		public UIDataLoader(IWaitFormActivator waitFormActivator, IExceptionHandler exceptionHandler) {
			Guard.ArgumentNotNull(waitFormActivator, "waitFormActivator");
			Guard.ArgumentNotNull(exceptionHandler, "exceptionHandler");
			this.waitFormActivator = waitFormActivator;
			this.exceptionHandler = exceptionHandler;
		}
		IWaitFormActivator WaitFormActivator { get { return waitFormActivator; } }
		public DBSchema LoadSchema(IDBSchemaProvider schemaProvider, SqlDataConnection sqlDataConnection, string[] objectNames) {
			return LoadSchemaInternal(sqlDataConnection,
				(connection, token) => Task.Factory.StartNew(() => {
					DBSchema dbSchema = schemaProvider.GetSchema(connection);
					DBTable[] dbObjects = dbSchema.Tables.Union(dbSchema.Views).ToArray();
					schemaProvider.LoadColumns(sqlDataConnection, dbObjects.Where(o => (objectNames == null || objectNames.Contains(o.Name)) && o.Columns.Count == 0).ToArray());
					return dbSchema;
				}, token));
		}
		DBSchema LoadSchemaInternal(SqlDataConnection sqlDataConnection, Func<SqlDataConnection, CancellationToken, Task<DBSchema>> core) {
			Debug.Assert(sqlDataConnection != null && sqlDataConnection.IsConnected, "Connection must be opened.");
			var cts = new CancellationTokenSource();
			var hook = new CancellationTokenHook(cts);
			CancellationToken token = cts.Token;
			WaitFormActivator.ShowWaitForm(false, true, true);
			WaitFormActivator.SetWaitFormObject(hook);
			WaitFormActivator.SetWaitFormCaption(DataAccessLocalizer.GetString(DataAccessStringId.LoadingDataPanelText));
			WaitFormActivator.SetWaitFormDescription(string.Empty);
			WaitFormActivator.EnableCancelButton(true);
			Exception taskException = null;
			try {
				Task<DBSchema> task = core(sqlDataConnection, token);
				task.Wait(token);
				return task.Result;
			}
			catch(Exception ex) {
				AggregateException aex = ex as AggregateException;
				taskException = aex == null ? ex : ExceptionHelper.Unwrap(aex);
			}
			finally {
				WaitFormActivator.CloseWaitForm();
			}
			if(taskException != null)
				exceptionHandler.HandleException(taskException);
			return null;
		}
		public SelectedDataEx LoadData(SqlDataConnection sqlDataConnection, DBSchema dbSchema, IEnumerable<IParameter> sourceParameters, SqlQuery sqlQuery, bool topThousand) {
			CancellationTokenSource cts = new CancellationTokenSource();
			CancellationTokenHook hook = new CancellationTokenHook(cts);
			CancellationToken token = cts.Token;
			waitFormActivator.ShowWaitForm(true, true, true);
			waitFormActivator.SetWaitFormObject(hook);
			Exception exception = null;
			try {
				Task<SelectedDataEx> task = Task.Factory.StartNew(() => new QueryExecutor(sqlDataConnection, dbSchema, null, sourceParameters).Execute(sqlQuery, topThousand, token), token);
				task.Wait();
				if (task.Status == TaskStatus.RanToCompletion)
					return task.Result;
			}
			catch (Exception ex) {
				exception = ex;
			}
			finally {
				waitFormActivator.CloseWaitForm();
			}
			if(exception != null) {
				AggregateException aex = exception as AggregateException;
				if(aex != null) {
					aex.Flatten();
					if(aex.InnerExceptions.Count == 1)
						exception = aex.GetBaseException();
				}
				exceptionHandler.HandleException(exception);
			}
			return null;
		}
	}
}
