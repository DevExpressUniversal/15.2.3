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
using DevExpress.DataAccess.Excel;
using DevExpress.DataAccess.Localization;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Wizard.Services;
namespace DevExpress.DataAccess.Wizard.Native {
	public static class ExcelSchemaLoaderAsync {
		public static FieldInfo[] GetSchema(Func<CancellationToken, FieldInfo[]> getSchema, IWaitFormActivator waitFormActivator, IExceptionHandler exceptionHandler) {
			var cts = new CancellationTokenSource();
			var hook = new CancellationTokenHook(cts);
			var token = cts.Token;
			var waitFormProvider = new WaitFormProvider(
				TaskScheduler.FromCurrentSynchronizationContext(),
				waitFormActivator,
				hook,
				DataAccessLocalizer.GetString(DataAccessStringId.ExcelDataSource_SchemaLoadingText));
			FieldInfo[] schema = new FieldInfo[0];
			Exception taskException = null;
			waitFormProvider.ShowWaitForm();
			try {
				var task = Task.Factory.StartNew(() => getSchema(token), token, TaskCreationOptions.None, TaskScheduler.Default);
				task.Wait(token);
				schema = task.Result;
			}
			catch(Exception e) {
				AggregateException aex = e as AggregateException;
				taskException = aex == null ? e : ExceptionHelper.Unwrap(aex);
			}
			finally {
				waitFormProvider.CloseWaitForm(true);
			}
			if(taskException != null) {
				exceptionHandler.HandleException(taskException);
				return null;
			}
			return schema;
		}
	}
}
