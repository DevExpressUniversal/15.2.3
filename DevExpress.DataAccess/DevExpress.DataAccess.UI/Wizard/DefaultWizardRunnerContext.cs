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

using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Data.WizardFramework;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.DataAccess.UI.Native;
using DevExpress.DataAccess.UI.Wizard.Services;
using DevExpress.DataAccess.Wizard;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
namespace DevExpress.DataAccess.UI.Wizard {
	public class DefaultWizardRunnerContext : IWizardRunnerContext {
		readonly UserLookAndFeel lookAndFeel;
		readonly IWin32Window owner;
		WizardView view;
		IWaitFormActivator waitFormActivator;
		public DefaultWizardRunnerContext(UserLookAndFeel lookAndFeel, IWin32Window owner) {
			this.lookAndFeel = lookAndFeel;
			this.owner = owner;
		}
		public IWaitFormActivator WaitFormActivator { get { return waitFormActivator ?? (waitFormActivator = CreateWaitFormActivator()); } }
		public bool Run<TModel>(Wizard<TModel> wizard) where TModel : IDataComponentModel {
			view.LookAndFeel.ParentLookAndFeel = this.lookAndFeel;
			wizard.Completed += (sender, e) => {
				if(view != null) {
					view.DialogResult = DialogResult.OK;
					view.Close();
				}
			};
			return view.ShowDialog(this.owner) == DialogResult.OK;
		}
		public IExceptionHandler CreateExceptionHandler(ExceptionHandlerKind kind) {
			return CreateExceptionHandler(kind, view == null ? null : view.Text);
		}
		public IExceptionHandler CreateExceptionHandler(ExceptionHandlerKind kind, string caption) {
			switch(kind) {
				case ExceptionHandlerKind.Connection:
					return new LoaderExceptionHandler(view, lookAndFeel) {
						ErrorsMessage = DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardCannotConnectMessage),
						CancelledMessage = "Connection has been aborted."
					};
				case ExceptionHandlerKind.Loading:
					return new LoaderExceptionHandler(view, lookAndFeel);
				default:
					return new ExceptionHandler(this.lookAndFeel, view, caption);
			}
		}
		public IWizardView CreateWizardView(string wizardTitle, Size wizardSize) {
			view = new WizardView() {Text = wizardTitle};
			if(wizardSize != Size.Empty)
				view.Size = wizardSize;
			return view;
		}
		public void ShowMessage(string message) { ShowMessage(message, view.Text); }
		public void ShowMessage(string message, string caption) {
			XtraMessageBox.Show(this.lookAndFeel, view, message, caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}
		public bool Confirm(string message) {
			return XtraMessageBox.Show(lookAndFeel, view, message, view.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK;
		}
		IWaitFormActivator CreateWaitFormActivator() {
			return new WaitFormActivator(view, typeof(WaitFormWithCancel), true);
		}
	}
}
