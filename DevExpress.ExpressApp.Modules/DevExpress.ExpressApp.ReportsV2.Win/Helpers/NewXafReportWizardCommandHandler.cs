#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Windows.Forms;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;
namespace DevExpress.ExpressApp.ReportsV2.Win {
	internal class NewXafReportWizardCommandHandler : ICommandHandler {
		private XafApplication application;
		private EventHandler<NewXafReportWizardShowingEventArgs> newXafReportWizardShowingHandler;
		private IDesignForm designForm;
		internal static bool RunXafReportWizard(XafApplication application, XtraReport newReport, Type reportDataType, EventHandler<NewXafReportWizardShowingEventArgs> newXafReportWizardShowingHandler) {
			QueryRootReportComponentNameEventArgs raiseQueryRootReportComponentNameArgs = new QueryRootReportComponentNameEventArgs(newReport);
			WinReportServiceController.RaiseQueryRootReportComponentName(raiseQueryRootReportComponentNameArgs);
			if(!raiseQueryRootReportComponentNameArgs.Handled) {
				raiseQueryRootReportComponentNameArgs.Name = raiseQueryRootReportComponentNameArgs.GetDefaultName();
			}
			newReport.Name = raiseQueryRootReportComponentNameArgs.Name;
			NewXafReportWizardShowingEventArgs args = new NewXafReportWizardShowingEventArgs(reportDataType, new NewReportWizardParameters(newReport, reportDataType));
			if(newXafReportWizardShowingHandler != null) {
				newXafReportWizardShowingHandler(null, args);
			}
			newReport.Tag = args.WizardParameters;
			if(!args.Handled) {
				using(XRDesignFormEx form = new XRDesignFormEx()) {
					form.OpenReport(args.WizardParameters.Report);
					XtraReportWizardRunner wizard = new XtraReportWizardRunner(newReport);
					using(IObjectSpace designerObjectSpace = application.CreateObjectSpace(reportDataType)) {
						WizPageXafWelcome wizPageXafWelcome = new WizPageXafWelcome(wizard, application, args.WizardParameters, designerObjectSpace);
						wizard.BeforeRun += delegate(object sender, XRWizardRunnerBeforeRunEventArgs e) {
							for(int i = 0; i < e.WizardPages.Count; i++) {
								if(e.WizardPages[i] is WizPageWelcome) {
									e.WizardPages[i] = wizPageXafWelcome;
									break;
								}
							}
						};
						args.WizardResult = wizard.Run();
						wizPageXafWelcome.Dispose();
					}
				}
			}
			return args.WizardResult == DialogResult.OK;
		}
		public NewXafReportWizardCommandHandler(XafApplication application, IDesignForm designForm, EventHandler<NewXafReportWizardShowingEventArgs> newXafReportWizardShowingHandler) {
			this.application = application;
			this.designForm = designForm;
			this.newXafReportWizardShowingHandler = newXafReportWizardShowingHandler;
		}
		public bool CanHandleCommand(ReportCommand command, ref bool enableNextHandler) {
			if(command == ReportCommand.NewReportWizard) {
				enableNextHandler = false;
				return true;
			}
			return false;
		}
		public void HandleCommand(ReportCommand command, object[] args) {
			ReportsModuleV2 reportsModule = ReportsModuleV2.FindReportsModule(application.Modules); 
			if(reportsModule == null) {
				throw new InvalidOperationException("The ReportsModule is not found");
			}
			;Type reportDataType = reportsModule.ReportDataType;
			using(IObjectSpace newReportObjectSpace = application.CreateObjectSpace(reportDataType)) {
				IReportDataV2 reportData = (IReportDataV2)newReportObjectSpace.CreateObject(reportDataType);
				XtraReport report = ReportDataProvider.ReportsStorage.LoadReport(reportData);
				if(RunXafReportWizard(application, report, reportDataType, newXafReportWizardShowingHandler)) {
					designForm.OpenReport(report);
					string newReportHandle = ReportDataProvider.ReportsStorage.CreateNewReportHandle(reportDataType);
					designForm.DesignMdiController.ActiveDesignPanel.FileName = newReportHandle;
				}
			}
		}
	}
}
