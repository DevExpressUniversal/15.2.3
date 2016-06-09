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

namespace DevExpress.DataAccess.UI.Native {
	partial class OkCancelForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.layoutControlMain = new DevExpress.XtraLayout.LayoutControl();
			this.panelControlAdditionalButtons = new DevExpress.XtraEditors.PanelControl();
			this.panelContent = new DevExpress.XtraEditors.PanelControl();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOK = new DevExpress.XtraEditors.SimpleButton();
			this.layoutControlGroupMain = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlGroupContent = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutItemContentPanel = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlGroupButtons = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlGroupOkCancel = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutItemButtonOk = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutItemButtonCancel = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItemAdditionalButtons = new DevExpress.XtraLayout.LayoutControlItem();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlMain)).BeginInit();
			this.layoutControlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControlAdditionalButtons)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelContent)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupMain)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupContent)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemContentPanel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupButtons)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupOkCancel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonOk)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonCancel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItemAdditionalButtons)).BeginInit();
			this.SuspendLayout();
			this.layoutControlMain.BackColor = System.Drawing.Color.Transparent;
			this.layoutControlMain.Controls.Add(this.panelControlAdditionalButtons);
			this.layoutControlMain.Controls.Add(this.panelContent);
			this.layoutControlMain.Controls.Add(this.btnCancel);
			this.layoutControlMain.Controls.Add(this.btnOK);
			this.layoutControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControlMain.Location = new System.Drawing.Point(0, 0);
			this.layoutControlMain.Name = "layoutControlMain";
			this.layoutControlMain.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(649, 190, 839, 575);
			this.layoutControlMain.Root = this.layoutControlGroupMain;
			this.layoutControlMain.Size = new System.Drawing.Size(284, 261);
			this.layoutControlMain.TabIndex = 6;
			this.panelControlAdditionalButtons.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControlAdditionalButtons.Location = new System.Drawing.Point(2, 228);
			this.panelControlAdditionalButtons.Name = "panelControlAdditionalButtons";
			this.panelControlAdditionalButtons.Size = new System.Drawing.Size(85, 31);
			this.panelControlAdditionalButtons.TabIndex = 7;
			this.panelContent.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelContent.Location = new System.Drawing.Point(2, 2);
			this.panelContent.Name = "panelContent";
			this.panelContent.Size = new System.Drawing.Size(280, 222);
			this.panelContent.TabIndex = 4;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(190, 228);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(83, 22);
			this.btnCancel.StyleController = this.layoutControlMain;
			this.btnCancel.TabIndex = 6;
			this.btnCancel.Text = "&Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(101, 228);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(83, 22);
			this.btnOK.StyleController = this.layoutControlMain;
			this.btnOK.TabIndex = 5;
			this.btnOK.Text = "&OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			this.layoutControlGroupMain.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
			this.layoutControlGroupMain.GroupBordersVisible = false;
			this.layoutControlGroupMain.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlGroupContent,
			this.layoutControlGroupButtons});
			this.layoutControlGroupMain.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroupMain.Name = "Root";
			this.layoutControlGroupMain.Size = new System.Drawing.Size(284, 261);
			this.layoutControlGroupMain.TextVisible = false;
			this.layoutControlGroupContent.GroupBordersVisible = false;
			this.layoutControlGroupContent.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutItemContentPanel});
			this.layoutControlGroupContent.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroupContent.Name = "layoutControlGroupContent";
			this.layoutControlGroupContent.Size = new System.Drawing.Size(284, 226);
			this.layoutControlGroupContent.TextVisible = false;
			this.layoutItemContentPanel.Control = this.panelContent;
			this.layoutItemContentPanel.Location = new System.Drawing.Point(0, 0);
			this.layoutItemContentPanel.Name = "layoutItemContentPanel";
			this.layoutItemContentPanel.Size = new System.Drawing.Size(284, 226);
			this.layoutItemContentPanel.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemContentPanel.TextVisible = false;
			this.layoutControlGroupButtons.GroupBordersVisible = false;
			this.layoutControlGroupButtons.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlGroupOkCancel,
			this.layoutControlItemAdditionalButtons});
			this.layoutControlGroupButtons.Location = new System.Drawing.Point(0, 226);
			this.layoutControlGroupButtons.Name = "layoutControlGroupButtons";
			this.layoutControlGroupButtons.Size = new System.Drawing.Size(284, 35);
			this.layoutControlGroupButtons.TextVisible = false;
			this.layoutControlGroupOkCancel.GroupBordersVisible = false;
			this.layoutControlGroupOkCancel.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutItemButtonOk,
			this.layoutItemButtonCancel});
			this.layoutControlGroupOkCancel.Location = new System.Drawing.Point(89, 0);
			this.layoutControlGroupOkCancel.Name = "layoutControlGroupOkCancel";
			this.layoutControlGroupOkCancel.Size = new System.Drawing.Size(195, 35);
			this.layoutControlGroupOkCancel.TextVisible = false;
			this.layoutItemButtonOk.Control = this.btnOK;
			this.layoutItemButtonOk.Location = new System.Drawing.Point(0, 0);
			this.layoutItemButtonOk.Name = "layoutItemButtonOk";
			this.layoutItemButtonOk.Padding = new DevExpress.XtraLayout.Utils.Padding(12, 3, 2, 11);
			this.layoutItemButtonOk.Size = new System.Drawing.Size(98, 35);
			this.layoutItemButtonOk.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemButtonOk.TextVisible = false;
			this.layoutItemButtonCancel.Control = this.btnCancel;
			this.layoutItemButtonCancel.Location = new System.Drawing.Point(98, 0);
			this.layoutItemButtonCancel.Name = "layoutItemButtonCancel";
			this.layoutItemButtonCancel.Padding = new DevExpress.XtraLayout.Utils.Padding(3, 11, 2, 11);
			this.layoutItemButtonCancel.Size = new System.Drawing.Size(97, 35);
			this.layoutItemButtonCancel.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemButtonCancel.TextVisible = false;
			this.layoutControlItemAdditionalButtons.Control = this.panelControlAdditionalButtons;
			this.layoutControlItemAdditionalButtons.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItemAdditionalButtons.Name = "layoutControlItemAdditionalButtons";
			this.layoutControlItemAdditionalButtons.Size = new System.Drawing.Size(89, 35);
			this.layoutControlItemAdditionalButtons.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItemAdditionalButtons.TextVisible = false;
			this.AcceptButton = this.btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(284, 261);
			this.Controls.Add(this.layoutControlMain);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "OkCancelForm";
			this.ShowIcon = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			((System.ComponentModel.ISupportInitialize)(this.layoutControlMain)).EndInit();
			this.layoutControlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.panelControlAdditionalButtons)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelContent)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupMain)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupContent)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemContentPanel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupButtons)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupOkCancel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonOk)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonCancel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItemAdditionalButtons)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		protected XtraEditors.SimpleButton btnCancel;
		protected XtraEditors.SimpleButton btnOK;
		protected XtraEditors.PanelControl panelContent;
		protected XtraLayout.LayoutControl layoutControlMain;
		protected XtraLayout.LayoutControlGroup layoutControlGroupMain;
		protected XtraLayout.LayoutControlItem layoutItemContentPanel;
		protected XtraLayout.LayoutControlItem layoutItemButtonOk;
		protected XtraLayout.LayoutControlItem layoutItemButtonCancel;
		private XtraLayout.LayoutControlGroup layoutControlGroupContent;
		private XtraLayout.LayoutControlGroup layoutControlGroupButtons;
		protected XtraLayout.LayoutControlGroup layoutControlGroupOkCancel;
		protected XtraEditors.PanelControl panelControlAdditionalButtons;
		protected XtraLayout.LayoutControlItem layoutControlItemAdditionalButtons;
	}
}
