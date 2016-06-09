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
using System.Collections;
using System.ComponentModel.Design;
using System.Data;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.XtraPrinting.Native;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.XtraEditors;
using System.ComponentModel.Design.Serialization;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.LookAndFeel.DesignService;
namespace DevExpress.XtraReports.Design
{
	public class WizPageDataset : DevExpress.Utils.InteriorWizardPage
	{
		protected DevExpress.XtraEditors.TextEdit tbDatasetName;
		protected System.Windows.Forms.Label lblDescription;
		private System.ComponentModel.IContainer components = null;
		protected NewStandardReportWizard fWizard;
		protected IDesignerHost designerHost;
		public WizPageDataset(XRWizardRunnerBase runner)  
			: this() {
			fWizard = (NewStandardReportWizard)runner.Wizard;
			designerHost = fWizard.DesignerHost;
		}
		WizPageDataset() {
			InitializeComponent();
			headerPicture.Image = ResourceImageHelper.CreateBitmapFromResources("Images.WizTopDataSet.gif", typeof(LocalResFinder));
		}
		#region Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WizPageDataset));
			this.tbDatasetName = new DevExpress.XtraEditors.TextEdit();
			this.lblDescription = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.headerPicture)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tbDatasetName.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.titleLabel, "titleLabel");
			resources.ApplyResources(this.subtitleLabel, "subtitleLabel");
			resources.ApplyResources(this.tbDatasetName, "tbDatasetName");
			this.tbDatasetName.Name = "tbDatasetName";
			this.tbDatasetName.TextChanged += new System.EventHandler(this.tbDatasetName_TextChanged);
			resources.ApplyResources(this.lblDescription, "lblDescription");
			this.lblDescription.Name = "lblDescription";
			this.Controls.Add(this.lblDescription);
			this.Controls.Add(this.tbDatasetName);
			this.Name = "WizPageDataset";
			this.Controls.SetChildIndex(this.tbDatasetName, 0);
			this.Controls.SetChildIndex(this.lblDescription, 0);
			this.Controls.SetChildIndex(this.headerPanel, 0);
			this.Controls.SetChildIndex(this.headerSeparator, 0);
			this.Controls.SetChildIndex(this.titleLabel, 0);
			this.Controls.SetChildIndex(this.subtitleLabel, 0);
			this.Controls.SetChildIndex(this.headerPicture, 0);
			((System.ComponentModel.ISupportInitialize)(this.headerPicture)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tbDatasetName.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		protected override void UpdateWizardButtons() {
			if (!String.IsNullOrEmpty(tbDatasetName.Text))
				Wizard.WizardButtons = WizardButton.Back | WizardButton.DisabledFinish | WizardButton.Next;
			else
				Wizard.WizardButtons = WizardButton.Back | WizardButton.DisabledFinish;
		}
		protected override bool OnSetActive() {
			tbDatasetName.Text = fWizard.DatasetName;
			BeginInvoke(new MethodInvoker(() => { tbDatasetName.Focus(); }));
			return true;
		}
		void ShowMessage(string msg) {
			string caption = "Report Wizard";
			IUIService uiSrv = (IUIService)designerHost.GetService(typeof(IUIService));
			if (uiSrv != null) {
				IWin32Window owner = uiSrv.GetDialogOwnerWindow();
				XtraMessageBox.Show(DesignLookAndFeelHelper.GetLookAndFeel(fWizard.DesignerHost), owner, msg, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			else
				XtraMessageBox.Show(DesignLookAndFeelHelper.GetLookAndFeel(fWizard.DesignerHost), msg, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		void tbDatasetName_TextChanged(object sender, System.EventArgs e) {
			UpdateWizardButtons();
		}
		protected override string OnWizardNext() {
			string message = String.Empty;
			if (CanLeavePage(ref message)) {
				fWizard.DatasetName = tbDatasetName.Text.Trim();
				return DevExpress.Utils.WizardForm.NextPage;
			}
			else {
				ShowMessage(message);
				tbDatasetName.Focus();
				tbDatasetName.SelectAll();
				return DevExpress.Utils.WizardForm.NoPageChange;
			}
		}
		protected override bool OnWizardFinish() {
			string message = String.Empty;
			if (CanLeavePage(ref message)) {
				fWizard.DatasetName = tbDatasetName.Text.Trim();
				return true;
			}
			else {
				ShowMessage(message);
				tbDatasetName.Focus();
				tbDatasetName.SelectAll();
				return false;
			}
		}
		protected virtual bool CanLeavePage(ref string msg) {
			Microsoft.CSharp.CSharpCodeProvider provider = new Microsoft.CSharp.CSharpCodeProvider();
			string name = tbDatasetName.Text.Trim();
			INameCreationService nameCreationService = (INameCreationService)designerHost.GetService(typeof(INameCreationService));
			if (provider.IsValidIdentifier(name) && nameCreationService.IsValidName(name))
				return true;
			msg  = "DataSet name is not valid or in use.";
			return false;
		}
	}
}
