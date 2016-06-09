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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Data.WizardFramework;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.DataAccess.UI.Native;
using DevExpress.DataAccess.UI.Wizard;
using DevExpress.DataAccess.UI.Wizard.Clients;
using DevExpress.DataAccess.UI.Wizard.Services;
using DevExpress.DataAccess.Wizard;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using DevExpress.XtraReports.ReportGeneration.Wizard.Presenters;
using DevExpress.XtraReports.Wizards3.Localization;
namespace DevExpress.XtraReports.ReportGeneration.Wizard {
	public class ReportGeneratorWizardRunner<TModel> : DataSourceWizardRunnerBase<TModel, IDataSourceWizardClientUI>
		where TModel : ReportGridDataModel,new () {
		public ReportGeneratorWizardRunner(UserLookAndFeel lookAndFeel, IWin32Window owner) : base(lookAndFeel, owner){
		}
		public ReportGeneratorWizardRunner(IWizardRunnerContext context) : base(context){
		}
		protected override WizardPageFactoryBase<TModel, IDataSourceWizardClientUI> CreatePageFactory(IDataSourceWizardClientUI client){
			return new ReportGeneratorWizardPageFactory<TModel, IDataSourceWizardClientUI>(client);
		}
		protected override Type StartPage { get { return typeof(WizardStartPage<TModel>); } }
		protected override string WizardTitle{
			get { return ReportDesignerLocalizer.GetString(ReportBoxDesignerStringId.Wizard_WindowTitleReport); }
		}
		protected override Size WizardSize { get { return new Size((int)(Skins.DpiProvider.Default.DpiScaleFactor * 756), (int)(Skins.DpiProvider.Default.DpiScaleFactor * 507)); } }  
	}
	public class ReportGeneratorWizardRunnerContext : IWizardRunnerContext {
		readonly UserLookAndFeel lookAndFeel;
		readonly IWin32Window owner;
		ReportGeneratorWizardView view; 
		IWaitFormActivator waitFormActivator;
		public ReportGeneratorWizardRunnerContext(UserLookAndFeel lookAndFeel, IWin32Window owner)  {
			this.lookAndFeel = lookAndFeel;
			this.owner = owner;
		}
		public bool Run<TModel>(Wizard<TModel> wizard) where TModel : IDataComponentModel{
			DevExpress.Skins.SkinManager.EnableFormSkins();
			DevExpress.LookAndFeel.LookAndFeelHelper.ForceDefaultLookAndFeelChanged();
			view.LookAndFeel.ParentLookAndFeel = this.lookAndFeel;
			wizard.Completed += (sender, e) => {
				if(view != null){
					view.DialogResult = DialogResult.OK;
					view.Close();
				}
			};
			return view.ShowDialog(this.owner) == DialogResult.OK;
		}
		public IExceptionHandler CreateExceptionHandler(ExceptionHandlerKind kind){
			return CreateExceptionHandler(kind, view == null ? null : view.Text);
		}
		public IExceptionHandler CreateExceptionHandler(ExceptionHandlerKind kind, string caption){
			if(this.owner == null)
				return new EmptyExceptionHandler();
			switch(kind){
				case ExceptionHandlerKind.Connection:
					return new LoaderExceptionHandler(view, lookAndFeel){ErrorsMessage = DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardCannotConnectMessage), CancelledMessage = "Connection has been aborted."};
				case ExceptionHandlerKind.Loading:
					return new LoaderExceptionHandler(view, lookAndFeel);
				default:
					return new ExceptionHandler(this.lookAndFeel, view, caption);
			}
		}
		public IWizardView CreateWizardView(string wizardTitle, Size wizardSize){
			view = new ReportGeneratorWizardView(){Text = wizardTitle};
			if(wizardSize != Size.Empty)
				view.Size = wizardSize;
			return view;
		}
		public void ShowMessage(string message) { ShowMessage(message, view.Text); }
		public void ShowMessage(string message, string caption){
			XtraMessageBox.Show(this.lookAndFeel, view, message, caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}
		public bool Confirm(string message) {
			return XtraMessageBox.Show(lookAndFeel, view, message, view.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK;
		}
		public IWaitFormActivator WaitFormActivator { get { return waitFormActivator ?? (waitFormActivator = CreateWaitFormActivator()); } }
		IWaitFormActivator CreateWaitFormActivator(){
			if(owner == null)
				return EmptyWaitFormActivator.Instance;
			return new WaitFormActivator(view, typeof(WaitFormWithCancel), true);
		}
	}
}
