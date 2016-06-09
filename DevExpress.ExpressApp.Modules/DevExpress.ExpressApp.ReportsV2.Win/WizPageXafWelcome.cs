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

using System.Windows.Forms;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.UI;
namespace DevExpress.ExpressApp.ReportsV2.Win {
	public class WizPageXafWelcome : DevExpress.Utils.ExteriorWizardPage {
		INewReportWizardParameters wizardParameters;
		XtraReportWizardBase standardWizard;
		XRWizardRunnerBase runner;
		private DetailView paramsView;
		private IObjectSpace objectSpace;
		private bool CanLeavePage() {
			return DevExpress.Persistent.Validation.Validator.RuleSet.ValidateAll(objectSpace, new object[] { wizardParameters }, "Accept");
		}
		protected override void UpdateWizardButtons() {
			Wizard.WizardButtons = DevExpress.Utils.WizardButton.Next | DevExpress.Utils.WizardButton.Finish;
		}
		protected override bool OnWizardFinish() {
			return CanLeavePage();
		}
		protected override string OnWizardNext() {
			if(CanLeavePage()) {
				((StandardReportWizard)standardWizard).FillFields();
				if(wizardParameters.ReportType == ReportType.Label) {
					runner.Wizard = new LabelReportWizard(((XtraReport)(standardWizard.DesignerHost.RootComponent)));
					return "WizPageLabelType";
				}
				else {
					runner.Wizard = this.standardWizard;
					return DevExpress.Utils.WizardForm.NextPage;
				}
			}
			else
				return DevExpress.Utils.WizardForm.NoPageChange;
		}
		public WizPageXafWelcome(XRWizardRunnerBase runner, XafApplication application, INewReportWizardParameters wizardParameters, IObjectSpace objectSpace) {
			this.objectSpace = objectSpace;
			this.runner = runner;
			this.standardWizard = runner.Wizard;
			this.Name = "WizPageWelcome";
			this.wizardParameters = wizardParameters;
			paramsView = application.CreateDetailView(objectSpace, wizardParameters);
			paramsView.CreateControls();
			this.Controls.Clear();
			paramsView.LayoutManager.CustomizationEnabled = false;
			this.Dock = DockStyle.Fill;
			this.Controls.Add((DevExpress.ExpressApp.Win.Layout.XafLayoutControl)(paramsView.LayoutManager.Container));
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing) {
				wizardParameters = null;
				standardWizard = null;
				runner = null;
				Controls.Remove((DevExpress.ExpressApp.Win.Layout.XafLayoutControl)(paramsView.LayoutManager.Container));
				paramsView.Dispose();
				paramsView = null;
			}
		}
#if DebugTest
		public DetailView DebugTest_ParamsView {
			get { return paramsView; }
		}
#endif
	}
}
