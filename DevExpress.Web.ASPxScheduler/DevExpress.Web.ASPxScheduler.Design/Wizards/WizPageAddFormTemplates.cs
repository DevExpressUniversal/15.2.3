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
using DevExpress.XtraScheduler.Native;
using DevExpress.Utils;
namespace DevExpress.Web.ASPxScheduler.Design.Wizards {
	public partial class WizPageAddFormTemplates : DevExpress.Utils.InteriorWizardPage {
		private DevExpress.XtraEditors.ListBoxControl lbFiles;
		private DevExpress.XtraEditors.LabelControl lblFiles;
		private DevExpress.XtraEditors.LabelControl lblFolderName;
		string[] newAddedFiles = new string[] { "" };
		private System.Windows.Forms.Label lblDescription;
		private System.Windows.Forms.Label lblWarning;
		string targetFolder = String.Empty;
		public WizPageAddFormTemplates() {
			InitializeComponent();
		}
		public WizPageAddFormTemplates(AddFormTemplatesWizard wizard, string targetFolder, string[] newAddedFiles) {
			Guard.ArgumentNotNull(wizard, "wizard");
			this.newAddedFiles = newAddedFiles;
			this.targetFolder = targetFolder;
			InitializeComponent();
		}
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WizPageAddFormTemplates));
			this.lbFiles = new DevExpress.XtraEditors.ListBoxControl();
			this.lblFiles = new DevExpress.XtraEditors.LabelControl();
			this.lblFolderName = new DevExpress.XtraEditors.LabelControl();
			this.lblDescription = new System.Windows.Forms.Label();
			this.lblWarning = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.headerPicture)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lbFiles)).BeginInit();
			this.SuspendLayout();
			this.titleLabel.Text = "Template update notification";
			this.subtitleLabel.Text = "New version templates are added to your project.";
			this.lbFiles.Location = new System.Drawing.Point(3, 104);
			this.lbFiles.Name = "lbFiles";
			this.lbFiles.Size = new System.Drawing.Size(241, 206);
			this.lbFiles.TabIndex = 5;
			this.lblFiles.Location = new System.Drawing.Point(3, 66);
			this.lblFiles.Name = "lblFiles";
			this.lblFiles.Size = new System.Drawing.Size(278, 13);
			this.lblFiles.TabIndex = 6;
			this.lblFiles.Text = "The following files have been added to your project folder";
			this.lblFolderName.Location = new System.Drawing.Point(3, 85);
			this.lblFolderName.Name = "lblFolderName";
			this.lblFolderName.Size = new System.Drawing.Size(0, 13);
			this.lblFolderName.TabIndex = 6;
			this.lblDescription.Location = new System.Drawing.Point(250, 104);
			this.lblDescription.Name = "lblDescription";
			this.lblDescription.Size = new System.Drawing.Size(242, 159);
			this.lblDescription.TabIndex = 8;
			this.lblDescription.Text = resources.GetString("lblDescription.Text");
			this.lblWarning.ForeColor = System.Drawing.Color.Red;
			this.lblWarning.Location = new System.Drawing.Point(250, 263);
			this.lblWarning.Name = "lblWarning";
			this.lblWarning.Size = new System.Drawing.Size(242, 47);
			this.lblWarning.TabIndex = 8;
			this.lblWarning.Text = "Note that these files are overwritten in template version updates. Any user modif" +
				"ications are lost.";
			this.Controls.Add(this.lblFiles);
			this.Controls.Add(this.lbFiles);
			this.Controls.Add(this.lblDescription);
			this.Controls.Add(this.lblFolderName);
			this.Controls.Add(this.lblWarning);
			this.Name = "WizPageAddFormTemplates";
			this.Load += new System.EventHandler(this.WizPageAddFormTemplates_Load);
			this.Controls.SetChildIndex(this.lblWarning, 0);
			this.Controls.SetChildIndex(this.lblFolderName, 0);
			this.Controls.SetChildIndex(this.lblDescription, 0);
			this.Controls.SetChildIndex(this.lbFiles, 0);
			this.Controls.SetChildIndex(this.lblFiles, 0);
			this.Controls.SetChildIndex(this.headerPanel, 0);
			this.Controls.SetChildIndex(this.headerSeparator, 0);
			this.Controls.SetChildIndex(this.titleLabel, 0);
			this.Controls.SetChildIndex(this.subtitleLabel, 0);
			this.Controls.SetChildIndex(this.headerPicture, 0);
			((System.ComponentModel.ISupportInitialize)(this.headerPicture)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lbFiles)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private void WizPageAddFormTemplates_Load(object sender, EventArgs e) {
			lblFolderName.Text = targetFolder;
			int prefixLength = targetFolder.Length;
			int count = newAddedFiles.Length;
			for (int i = 0; i < count; i++) {
				string item = newAddedFiles[i];
				if (item.StartsWith(targetFolder))
					item = item.Substring(prefixLength);
				lbFiles.Items.Add(item);
			}
		}
		protected override bool OnSetActive() {
			Wizard.WizardButtons = WizardButton.Finish;
			return true;
		}
	}
}
