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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.XtraReports.UserDesigner;
namespace DevExpress.XtraReports.Design
{
	[ToolboxItem(false)]
	public class WizPageReportTitle : DevExpress.Utils.ExteriorWizardPage
	{
		private DevExpress.XtraEditors.TextEdit tbReportTitle;
		private System.Windows.Forms.Label lblFinish;
		private System.ComponentModel.IContainer components = null;
		StandardReportWizard wizard;
		public WizPageReportTitle(XRWizardRunnerBase runner) 
			: this() {
			this.wizard = (StandardReportWizard)runner.Wizard;
		}
		WizPageReportTitle() {
			InitializeComponent();
			this.watermarkPicture.Image = ResourceImageHelper.CreateBitmapFromResources("Images.WizFinish.gif", typeof(LocalResFinder));
		}
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		#region Designer generated code
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WizPageReportTitle));
			this.tbReportTitle = new DevExpress.XtraEditors.TextEdit();
			this.lblFinish = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.watermarkPicture)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tbReportTitle.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.titleLabel, "titleLabel");
			resources.ApplyResources(this.tbReportTitle, "tbReportTitle");
			this.tbReportTitle.Name = "tbReportTitle";
			resources.ApplyResources(this.lblFinish, "lblFinish");
			this.lblFinish.Name = "lblFinish";
			this.Controls.Add(this.lblFinish);
			this.Controls.Add(this.tbReportTitle);
			this.Name = "WizPageReportTitle";
			this.Controls.SetChildIndex(this.tbReportTitle, 0);
			this.Controls.SetChildIndex(this.lblFinish, 0);
			this.Controls.SetChildIndex(this.titleLabel, 0);
			this.Controls.SetChildIndex(this.watermarkPicture, 0);
			((System.ComponentModel.ISupportInitialize)(this.watermarkPicture)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tbReportTitle.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		void ApplyChanges() {
			wizard.ReportTitle = tbReportTitle.Text;
		}
		void RollbackChanges() {
			wizard.ReportTitle = wizard.ReportName;
		}
		protected override bool OnSetActive() {
			tbReportTitle.Text = wizard.ReportName;
			tbReportTitle.SelectAll();
			return true;
		}
		protected override void UpdateWizardButtons() {
			Wizard.WizardButtons = WizardButton.Back | WizardButton.Finish;
		}
		protected override string OnWizardBack() {
			RollbackChanges();
			return WizardForm.NextPage;
		}
		protected override string OnWizardNext() {
			ApplyChanges();
			return WizardForm.NextPage;
		}
		protected override bool OnWizardFinish() {
			ApplyChanges();
			return true;
		}
	}
}
