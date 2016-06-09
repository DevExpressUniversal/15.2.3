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
using System.Threading;
using System.Threading.Tasks;
using DevExpress.Data;
using DevExpress.DataAccess.EntityFramework;
using DevExpress.DataAccess.Localization;
using DevExpress.DataAccess.Wizard.Native;
using DevExpress.DataAccess.Wizard.Services;
namespace DevExpress.DataAccess.Native.EntityFramework {
	public static class EFPreviewHelper {
		public static object GetPreviewData(EFDataConnection connection, EFStoredProcedureInfo storedProcedure, IEnumerable<IParameter> evaluatedParameters, IExceptionHandler exceptionHandler, IWaitFormActivator waitFormActivator) {
			if(storedProcedure == null)
				return null;
			CancellationTokenSource cts = new CancellationTokenSource();
			CancellationTokenHook hook = new CancellationTokenHook(cts);
			CancellationToken token = cts.Token;
			EFDataConnection connectionForPreview = new EFDataConnection(connection.Name, connection.ConnectionParameters) {
				SolutionTypesProvider = connection.SolutionTypesProvider,
				ConnectionStringsProvider = connection.ConnectionStringsProvider
			};
			if(!ConnectionHelper.OpenConnection(connectionForPreview, exceptionHandler, waitFormActivator))
				return null;
			waitFormActivator.ShowWaitForm(false, true, true);
			waitFormActivator.SetWaitFormObject(hook);
			waitFormActivator.SetWaitFormCaption(DataAccessLocalizer.GetString(DataAccessStringId.LoadingDataPanelText));
			waitFormActivator.SetWaitFormDescription(string.Empty);
			waitFormActivator.EnableCancelButton(true);
			Exception taskException;
			try {
				Task<object> getPreview = Task.Factory.StartNew(() => {
					EFContextWrapper wrapper = new EFContextWrapper(connectionForPreview, storedProcedure.Name,
						evaluatedParameters);
					wrapper.FillData();
					return wrapper.GetMethodValue(storedProcedure.Name);
				}, token);
				getPreview.Wait(token);
				return getPreview.Result;
			}
			catch(Exception ex) {
				AggregateException aex = ex as AggregateException;
				taskException = aex == null ? ex : ExceptionHelper.Unwrap(aex);
			}
			finally {
				waitFormActivator.CloseWaitForm();
			}
			if(taskException != null)
				exceptionHandler.HandleException(taskException);
			return null;
		}
	}
}
