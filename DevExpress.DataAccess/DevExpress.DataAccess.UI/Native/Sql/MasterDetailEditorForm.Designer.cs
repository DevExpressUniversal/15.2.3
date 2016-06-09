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

namespace DevExpress.DataAccess.UI.Native.Sql {
	partial class MasterDetailEditorForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (this.components != null)) {
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.masterDetailEditorControl = new DevExpress.DataAccess.UI.Native.Sql.MasterDetailEditorControl();
			((System.ComponentModel.ISupportInitialize)(this.panelContent)).BeginInit();
			this.panelContent.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlMain)).BeginInit();
			this.layoutControlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupMain)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemContentPanel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonOk)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonCancel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupOkCancel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControlAdditionalButtons)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItemAdditionalButtons)).BeginInit();
			this.SuspendLayout();
			this.btnCancel.Location = new System.Drawing.Point(358, 228);
			this.btnCancel.Size = new System.Drawing.Size(115, 22);
			this.btnOK.Location = new System.Drawing.Point(237, 228);
			this.btnOK.Size = new System.Drawing.Size(115, 22);
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			this.panelContent.Controls.Add(this.masterDetailEditorControl);
			this.panelContent.Padding = new System.Windows.Forms.Padding(9, 10, 9, 10);
			this.panelContent.Size = new System.Drawing.Size(480, 222);
			this.layoutControlMain.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(649, 190, 839, 575);
			this.layoutControlMain.Size = new System.Drawing.Size(484, 261);
			this.layoutControlMain.Controls.SetChildIndex(this.btnOK, 0);
			this.layoutControlMain.Controls.SetChildIndex(this.btnCancel, 0);
			this.layoutControlMain.Controls.SetChildIndex(this.panelContent, 0);
			this.layoutControlMain.Controls.SetChildIndex(this.panelControlAdditionalButtons, 0);
			this.layoutControlGroupMain.Size = new System.Drawing.Size(484, 261);
			this.layoutItemContentPanel.Size = new System.Drawing.Size(484, 226);
			this.layoutItemButtonOk.Size = new System.Drawing.Size(130, 35);
			this.layoutItemButtonCancel.Location = new System.Drawing.Point(130, 0);
			this.layoutItemButtonCancel.Size = new System.Drawing.Size(129, 35);
			this.layoutControlGroupOkCancel.Location = new System.Drawing.Point(225, 0);
			this.layoutControlGroupOkCancel.Size = new System.Drawing.Size(259, 35);
			this.panelControlAdditionalButtons.Size = new System.Drawing.Size(221, 31);
			this.layoutControlItemAdditionalButtons.Size = new System.Drawing.Size(225, 35);
			this.masterDetailEditorControl.DataSource = null;
			this.masterDetailEditorControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.masterDetailEditorControl.Location = new System.Drawing.Point(9, 10);
			this.masterDetailEditorControl.Name = "masterDetailEditorControl";
			this.masterDetailEditorControl.Padding = new System.Windows.Forms.Padding(1);
			this.masterDetailEditorControl.Size = new System.Drawing.Size(462, 202);
			this.masterDetailEditorControl.TabIndex = 1;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.ClientSize = new System.Drawing.Size(484, 261);
			this.MinimumSize = new System.Drawing.Size(500, 250);
			this.Name = "MasterDetailEditorForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Master-Detail Relation Editor";
			((System.ComponentModel.ISupportInitialize)(this.panelContent)).EndInit();
			this.panelContent.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutControlMain)).EndInit();
			this.layoutControlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupMain)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemContentPanel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonOk)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonCancel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupOkCancel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControlAdditionalButtons)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItemAdditionalButtons)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private MasterDetailEditorControl masterDetailEditorControl;
	}
}
