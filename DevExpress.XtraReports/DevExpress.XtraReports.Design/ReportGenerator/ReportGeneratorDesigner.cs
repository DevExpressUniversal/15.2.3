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

using DevExpress.Utils.Design;
using System.ComponentModel.Design;
using System.Windows.Forms;
using DevExpress.DataAccess.UI.Wizard;
using DevExpress.DataAccess.UI.Wizard.Clients;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.XtraBars.Docking2010.DragEngine;
using DevExpress.XtraReports.ReportGeneration.Wizard;
using DevExpress.XtraReports.Wizards3;
using System.Windows.Forms.Design;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraReports.Design.ReportGenerator{
	public class ReportGeneratorDesigner : BaseComponentDesigner{
		protected override void RegisterActionLists(DesignerActionListCollection list){
			if(AllowDesigner)
				list.Add(new ReportGeneratorDesignerActionList(this));
			base.RegisterActionLists(list);
		}
		IDesignerHost designerHost;
		public IDesignerHost DesignerHost{
			get{
				if(designerHost == null) designerHost = (IDesignerHost) GetService(typeof(IDesignerHost));
				return designerHost;
			}
		}
		ISelectionService selectionService;
		internal ISelectionService SelectionService{
			get{
				if(selectionService == null)
					selectionService = (ISelectionService) GetService(typeof(ISelectionService));
				return selectionService;
			}
		}
		internal ReportGeneration.ReportGenerator ReportGenerator{
			get { return Component as ReportGeneration.ReportGenerator; }
		}
		protected void ShowWizardForm(Form wizardForm){
			if(ReportGenerator == null) return;
			wizardForm.ShowInTaskbar = false;
			wizardForm.ShowDialog();
		}
		public void ShowGenerateReportWizard(){
			var helper = new ReportGeneratorHelper(this);
			var model = RunWizard(helper);
			if(model != null){
				if(model.AddToProject){
					helper.GenerateAndAddToProject(model);
				} else helper.GenerateAndSave(model);
			}
		}
		ReportGridDataModel RunWizard(ReportGeneratorHelper helper){
			UserLookAndFeel lookAndFeel = DesignerHost.GetService<ILookAndFeelService>().LookAndFeel;
			System.Windows.Forms.Design.IUIService uiService = DesignerHost.GetService<System.Windows.Forms.Design.IUIService>();
			IWin32Window owner = uiService != null ? uiService.GetDialogOwnerWindow() : null;
			ReportGeneratorWizardRunnerContext wizardRunnerContext = new ReportGeneratorWizardRunnerContext(lookAndFeel, owner);
			var runner = new ReportGeneratorWizardRunner<ReportGridDataModel>(wizardRunnerContext);
			var client = new DataSourceWizardClientUI(null, null, null, null, null);
			IWizardCustomizationService serv = DesignerHost.GetService(typeof(IWizardCustomizationService)) as IWizardCustomizationService;
			var model = new ReportGridDataModel();
			model.ReportFileName = helper.DefaultFileName;
			model.ViewSet = helper.GetViewSet();
			if(runner.Run(client, model, customization =>{  }))
				return runner.WizardModel;
			return null;
		}
		public class ReportGeneratorDesignerActionList : DesignerActionList{
			ReportGeneratorDesigner designer;
			public ReportGeneratorDesignerActionList(ReportGeneratorDesigner designer) : base(designer.Component){
				this.designer = designer;
			}
			public void GenerateReport(){
				if(designer != null) designer.ShowGenerateReportWizard();
			}
			public override DesignerActionItemCollection GetSortedActionItems(){
				DesignerActionItemCollection res = new DesignerActionItemCollection();
				if(designer != null && designer.Component != null){
					res.Add(new DesignerActionMethodItem(this, "GenerateReport", "Generate Report", "Actions", true));
				}
				return res;
			}
		}
	}
}
