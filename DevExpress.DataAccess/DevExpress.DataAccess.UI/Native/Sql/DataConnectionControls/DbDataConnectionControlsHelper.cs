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
using DevExpress.DataAccess.Localization;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.DataAccess.Wizard.Native;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Utils;
using DevExpress.XtraEditors;
namespace DevExpress.DataAccess.UI.Native.Sql.DataConnectionControls {
	public delegate string[] GetDatabaseListDelegate();
	public static class DbDataConnectionControlsHelper {
		public static void FillDataBasesList(XtraForm parentForm, ComboBoxEdit databaseList, GetDatabaseListDelegate getDatabases) {
			Guard.ArgumentNotNull(getDatabases, "getDatabase");
			databaseList.Properties.Items.Clear();
			CancellationTokenSource cts = new CancellationTokenSource();
			CancellationTokenHook hook = new CancellationTokenHook(cts);
			CancellationToken token = cts.Token;
			IWaitFormActivator waitFormActivator = new WaitFormActivator(parentForm, typeof(WaitFormWithCancel), true);
#if DEBUGTEST
			TaskScheduler taskScheduler = waitFormActivator.GetType() == typeof(DevExpress.DataAccess.Tests.MockWaitFormActivator) ? TaskScheduler.Default : TaskScheduler.FromCurrentSynchronizationContext();
#else
			TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
#endif
			WaitFormProvider waitFormProvider = new WaitFormProvider(taskScheduler, waitFormActivator, hook, DataAccessLocalizer.GetString(DataAccessStringId.LoadingDataPanelText));
			IExceptionHandler handler = new LoaderExceptionHandler(parentForm, parentForm.LookAndFeel);
			Exception taskException = null;
			waitFormProvider.ShowWaitForm();
			string[] result = null;
			try {
				Task<string[]> getResult = Task.Factory.StartNew(() => getDatabases(), token, TaskCreationOptions.None, TaskScheduler.Default);
				getResult.Wait(token);
				result = getResult.Result;
			} catch(Exception e) {
				AggregateException aex = e as AggregateException;
				taskException = aex == null ? e : ExceptionHelper.Unwrap(aex);
			} finally {
				waitFormProvider.CloseWaitForm(false);
			}
			if(taskException != null)
				handler.HandleException(taskException);
			else if(result == null || result.Length == 0)
				XtraMessageBox.Show(parentForm.LookAndFeel, parentForm,
					DataAccessUILocalizer.GetString(DataAccessUIStringId.MessageCannotLoadDatabasesList),
					DataAccessUILocalizer.GetString(DataAccessUIStringId.MessageBoxErrorTitle), MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			else
				foreach(string databaseName in result)
					databaseList.Properties.Items.Add(databaseName);
		}
	}
}
