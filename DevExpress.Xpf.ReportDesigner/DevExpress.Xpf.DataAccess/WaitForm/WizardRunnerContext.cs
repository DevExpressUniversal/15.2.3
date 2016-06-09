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
using DevExpress.Data.WizardFramework;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.DataAccess.Wizard;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Mvvm;
using DevExpress.Xpf.DataAccess.DataSourceWizard;
namespace DevExpress.Xpf.DataAccess.WaitForm {
	public class WizardRunnerContext : IWizardRunnerContext {
		public WizardRunnerContext(DataSourceWizardModelBase model) {
			this.model = model;
			this.waitFormActivator = new WaitFormActivator(model.Parameters.DoWithSplashScreenService);
		}
		readonly DataSourceWizardModelBase model;
		readonly IWaitFormActivator waitFormActivator;
		#region Implementation of IWizardRunnerContext
		IExceptionHandler IWizardRunnerContext.CreateExceptionHandler(ExceptionHandlerKind kind) {
			return new ExceptionHandler(model.Parameters.DoWithMessageBoxService);
		}
		IExceptionHandler IWizardRunnerContext.CreateExceptionHandler(ExceptionHandlerKind kind, string caption) {
			return new ExceptionHandler(model.Parameters.DoWithMessageBoxService, caption);
		}
		IWizardView IWizardRunnerContext.CreateWizardView(string wizardTitle, System.Drawing.Size wizardSize) { throw new NotSupportedException(); }
		bool IWizardRunnerContext.Run<TModel>(Wizard<TModel> wizard) { throw new NotSupportedException(); }
		void IWizardRunnerContext.ShowMessage(string message) {
			model.Parameters.DoWithMessageBoxService(dialog => dialog.ShowMessage(message, DataAccessUILocalizer.GetString(DataAccessUIStringId.MessageBoxWarningTitle), MessageButton.OK, MessageIcon.Warning));
		}
		void IWizardRunnerContext.ShowMessage(string message, string caption) {
			model.Parameters.DoWithMessageBoxService(dialog => dialog.ShowMessage(message, caption, MessageButton.OK, MessageIcon.Warning));
		}
		bool IWizardRunnerContext.Confirm(string message) {
			MessageResult result = MessageResult.None;
			model.Parameters.DoWithMessageBoxService(dialog => result = dialog.ShowMessage(message, DataAccessUILocalizer.GetString(DataAccessUIStringId.MessageBoxWarningTitle), MessageButton.OKCancel, MessageIcon.Warning));
			return result == MessageResult.OK;
		}
		IWaitFormActivator IWizardRunnerContext.WaitFormActivator { get { return waitFormActivator; } }
		#endregion
	}
}
