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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.DataAccess.EntityFramework;
using DevExpress.DataAccess.Localization;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Utils;
#if DEBUGTEST
using DevExpress.DataAccess.Tests;
#endif
namespace DevExpress.DataAccess.Wizard.Native {
	public static class ConnectionHelper {
		public static bool OpenConnection(DataConnectionBase dataConnection, IExceptionHandler exceptionHandler, IWaitFormActivator waitFormActivator, IDataConnectionParametersService dataConnectionParametersService) {
			Guard.ArgumentNotNull(dataConnection, "dataConnection");
			Guard.ArgumentNotNull(waitFormActivator, "waitFormActivator");
			Guard.ArgumentNotNull(exceptionHandler, "exceptionHandler");
			var cts = new CancellationTokenSource();
			var hook = new CancellationTokenHook(cts);
			var token = cts.Token;
			TaskScheduler taskScheduler;
#if DEBUGTEST
			taskScheduler = waitFormActivator.GetType() == typeof(MockWaitFormActivator) ? TaskScheduler.Default : TaskScheduler.FromCurrentSynchronizationContext();
#else
			taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
#endif
			Exception taskException;
			var waitFormProvider = new WaitFormProvider(taskScheduler, waitFormActivator, hook,
				DataAccessLocalizer.GetString(DataAccessStringId.ConnectingToDatabaseMessage));
			try {
				Task task = Task.Factory.StartNew(() => dataConnection.CreateDataStore(dataConnectionParametersService, waitFormProvider, token), token, TaskCreationOptions.None, TaskScheduler.Default);
				while(!(task.IsCompleted || token.IsCancellationRequested))
					Application.DoEvents();
				task.Wait(token);
				return dataConnection.IsConnected;
			}
			catch(Exception e) {
				AggregateException aex = e as AggregateException;
				taskException = aex == null ? e : ExceptionHelper.Unwrap(aex);
			}
			finally {
				waitFormProvider.CloseWaitForm(true);
			}
			if(taskException != null)
				exceptionHandler.HandleException(taskException);
			return false;
		}
		public static bool OpenConnection(EFDataConnection dataConnection, IExceptionHandler exceptionHandler, IWaitFormActivator waitFormActivator) {
			Exception connectionException;
			waitFormActivator.ShowWaitForm(true, false, true);
			try {
				dataConnection.Open();
				return dataConnection.IsConnected;
			} catch(Exception ex) {
				connectionException = ex;
			} finally {
				waitFormActivator.CloseWaitForm();
			}
			exceptionHandler.HandleException(connectionException);
			return false;
		}
	}
	public class WaitFormProvider : IWaitFormProvider {
		readonly IWaitFormActivator waitFormActivator;
		CancellationTokenHook hook;
		string caption;
		bool closedCompletely;
		readonly TaskScheduler ui;
		public WaitFormProvider(TaskScheduler ui, IWaitFormActivator waitFormActivator, CancellationTokenHook hook, string caption) {
			this.ui = ui;
			this.waitFormActivator = waitFormActivator;
			this.hook = hook;
			this.caption = caption;
		}
		public void CloseWaitForm(bool closeCompletely) {
			if(this.closedCompletely)
				return;
			if(closeCompletely)
				this.closedCompletely = true;
			Task.Factory.StartNew(() => {
				waitFormActivator.CloseWaitForm();
			}, CancellationToken.None, TaskCreationOptions.None, ui).Wait();
		}
		public void ShowWaitForm() {
			if(closedCompletely)
				return;
			Task.Factory.StartNew(() => {
				waitFormActivator.ShowWaitForm(true, false, true);
				waitFormActivator.SetWaitFormObject(hook);
				waitFormActivator.EnableCancelButton(true);
				waitFormActivator.SetWaitFormCaption(caption);
			}, CancellationToken.None, TaskCreationOptions.None, ui).Wait();
		}
	}
}
