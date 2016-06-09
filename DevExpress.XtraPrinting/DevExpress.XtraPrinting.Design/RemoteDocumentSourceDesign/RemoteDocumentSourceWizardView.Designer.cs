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

namespace DevExpress.XtraPrinting.Design.RemoteDocumentSourceDesign {
	partial class RemoteDocumentSourceWizardView {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RemoteDocumentSourceWizardView));
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.nextButton = new DevExpress.XtraEditors.SimpleButton();
			this.previousButton = new DevExpress.XtraEditors.SimpleButton();
			this.finishButton = new DevExpress.XtraEditors.SimpleButton();
			this.contentPanel = new System.Windows.Forms.Panel();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.label4 = new System.Windows.Forms.Label();
			this.pictureEdit2 = new DevExpress.XtraEditors.PictureEdit();
			this.label3 = new System.Windows.Forms.Label();
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this.descriptionLbl = new System.Windows.Forms.Label();
			this.tableLayoutPanel1.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureEdit2.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
			this.tableLayoutPanel1.Controls.Add(this.nextButton, 3, 2);
			this.tableLayoutPanel1.Controls.Add(this.previousButton, 2, 2);
			this.tableLayoutPanel1.Controls.Add(this.finishButton, 4, 2);
			this.tableLayoutPanel1.Controls.Add(this.contentPanel, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.descriptionLbl, 0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			resources.ApplyResources(this.nextButton, "nextButton");
			this.nextButton.Name = "nextButton";
			this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
			resources.ApplyResources(this.previousButton, "previousButton");
			this.previousButton.Name = "previousButton";
			this.previousButton.Click += new System.EventHandler(this.previousButton_Click);
			resources.ApplyResources(this.finishButton, "finishButton");
			this.finishButton.Name = "finishButton";
			this.finishButton.Click += new System.EventHandler(this.finishButton_Click);
			this.tableLayoutPanel1.SetColumnSpan(this.contentPanel, 5);
			resources.ApplyResources(this.contentPanel, "contentPanel");
			this.contentPanel.Name = "contentPanel";
			resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
			this.tableLayoutPanel1.SetColumnSpan(this.tableLayoutPanel2, 2);
			this.tableLayoutPanel2.Controls.Add(this.label4, 0, 2);
			this.tableLayoutPanel2.Controls.Add(this.pictureEdit2, 0, 0);
			this.tableLayoutPanel2.Controls.Add(this.label3, 0, 1);
			this.tableLayoutPanel2.Controls.Add(this.linkLabel1, 1, 2);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			resources.ApplyResources(this.label4, "label4");
			this.label4.Name = "label4";
			this.tableLayoutPanel2.SetColumnSpan(this.pictureEdit2, 2);
			resources.ApplyResources(this.pictureEdit2, "pictureEdit2");
			this.pictureEdit2.Name = "pictureEdit2";
			this.pictureEdit2.Properties.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("pictureEdit2.Properties.Appearance.BackColor")));
			this.pictureEdit2.Properties.Appearance.Options.UseBackColor = true;
			this.pictureEdit2.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pictureEdit2.Properties.PictureAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.pictureEdit2.Properties.ShowMenu = false;
			resources.ApplyResources(this.label3, "label3");
			this.tableLayoutPanel2.SetColumnSpan(this.label3, 2);
			this.label3.Name = "label3";
			resources.ApplyResources(this.linkLabel1, "linkLabel1");
			this.linkLabel1.LinkColor = System.Drawing.Color.Orange;
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.TabStop = true;
			this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
			resources.ApplyResources(this.descriptionLbl, "descriptionLbl");
			this.tableLayoutPanel1.SetColumnSpan(this.descriptionLbl, 5);
			this.descriptionLbl.Name = "descriptionLbl";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.tableLayoutPanel1);
			this.LookAndFeel.UseDefaultLookAndFeel = false;
			this.MinimizeBox = false;
			this.Name = "RemoteDocumentSourceWizardView";
			this.ShowAssemblyVersion = false;
			this.ShowInTaskbar = false;
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tableLayoutPanel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureEdit2.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private XtraEditors.SimpleButton nextButton;
		private XtraEditors.SimpleButton previousButton;
		private XtraEditors.SimpleButton finishButton;
		private System.Windows.Forms.Panel contentPanel;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.Label label4;
		private XtraEditors.PictureEdit pictureEdit2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.LinkLabel linkLabel1;
		private System.Windows.Forms.Label descriptionLbl;
	}
}
