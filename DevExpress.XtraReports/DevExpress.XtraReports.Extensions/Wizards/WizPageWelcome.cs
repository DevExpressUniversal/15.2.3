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
using DevExpress.XtraEditors;
using DevExpress.XtraReports.UserDesigner;
namespace DevExpress.XtraReports.Design
{
	[ToolboxItem(false)]
	public class WizPageWelcome : DevExpress.Utils.ExteriorWizardPage
	{
		private System.Windows.Forms.Label lblDescription;
		private System.Windows.Forms.Label lblNext;
		private GroupControl grpReportType;
		private System.ComponentModel.IContainer components = null;
		XRWizardRunnerBase runner;
		private DevExpress.XtraEditors.Internal.RadioGroupLocalizable rgrpReportType;
		XtraReportWizardBase standardWizard;
		public WizPageWelcome(XRWizardRunnerBase runner) 
			: this() {
			this.runner = runner;
			this.standardWizard = runner.Wizard;
		}
		WizPageWelcome() {
			InitializeComponent();
			this.watermarkPicture.Image = ResourceImageHelper.CreateBitmapFromResources("Images.WizStart.gif", typeof(LocalResFinder));
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		#region Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WizPageWelcome));
			this.lblDescription = new System.Windows.Forms.Label();
			this.lblNext = new System.Windows.Forms.Label();
			this.grpReportType = new DevExpress.XtraEditors.GroupControl();
			this.rgrpReportType = new DevExpress.XtraEditors.Internal.RadioGroupLocalizable();
			((System.ComponentModel.ISupportInitialize)(this.watermarkPicture)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grpReportType)).BeginInit();
			this.grpReportType.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.rgrpReportType.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.titleLabel, "titleLabel");
			resources.ApplyResources(this.lblDescription, "lblDescription");
			this.lblDescription.Name = "lblDescription";
			resources.ApplyResources(this.lblNext, "lblNext");
			this.lblNext.Name = "lblNext";
			resources.ApplyResources(this.grpReportType, "grpReportType");
			this.grpReportType.Controls.Add(this.rgrpReportType);
			this.grpReportType.Name = "grpReportType";
			resources.ApplyResources(this.rgrpReportType, "rgrpReportType");
			this.rgrpReportType.Name = "rgrpReportType";
			this.rgrpReportType.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
			this.rgrpReportType.Properties.Appearance.Options.UseBackColor = true;
			this.rgrpReportType.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.rgrpReportType.Properties.Columns = 1;
			this.rgrpReportType.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
			new DevExpress.XtraEditors.Controls.RadioGroupItem(0, "&Standard Report"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(1, "&Label Report")});
			this.Controls.Add(this.grpReportType);
			this.Controls.Add(this.lblNext);
			this.Controls.Add(this.lblDescription);
			this.Name = "WizPageWelcome";
			this.Controls.SetChildIndex(this.lblDescription, 0);
			this.Controls.SetChildIndex(this.lblNext, 0);
			this.Controls.SetChildIndex(this.grpReportType, 0);
			this.Controls.SetChildIndex(this.titleLabel, 0);
			this.Controls.SetChildIndex(this.watermarkPicture, 0);
			((System.ComponentModel.ISupportInitialize)(this.watermarkPicture)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpReportType)).EndInit();
			this.grpReportType.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.rgrpReportType.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		protected override void UpdateWizardButtons() {
			Wizard.WizardButtons = WizardButton.Next | WizardButton.DisabledFinish;
		}
		protected override string OnWizardNext() {
			if (rgrpReportType.SelectedIndex > 0) {
				runner.Wizard = new LabelReportWizard(runner.Report);
				return "WizPageLabelType";
			}
			else {
				runner.Wizard = this.standardWizard;
				return WizardForm.NextPage;
			}
		}
	}
}
