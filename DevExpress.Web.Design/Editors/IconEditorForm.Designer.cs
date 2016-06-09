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

namespace DevExpress.Web.Design {
	partial class IconEditorForm {
		private System.ComponentModel.IContainer components = null;
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.pnlBottom = new DevExpress.XtraEditors.PanelControl();
			this.btnReset = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
			this.rgResourceTypeSelector = new DevExpress.XtraEditors.RadioGroup();
			this.pnlContent = new DevExpress.XtraEditors.PanelControl();
			this.dxImageGalleryControl = new DevExpress.Web.Design.IconEditorControl();
			((System.ComponentModel.ISupportInitialize)(this.pnlBottom)).BeginInit();
			this.pnlBottom.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
			this.panelControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.rgResourceTypeSelector.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlContent)).BeginInit();
			this.pnlContent.SuspendLayout();
			this.SuspendLayout();
			this.pnlBottom.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlBottom.Controls.Add(this.btnReset);
			this.pnlBottom.Controls.Add(this.btnCancel);
			this.pnlBottom.Controls.Add(this.btnOk);
			this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlBottom.Location = new System.Drawing.Point(0, 462);
			this.pnlBottom.Name = "pnlBottom";
			this.pnlBottom.Size = new System.Drawing.Size(662, 42);
			this.pnlBottom.TabIndex = 1;
			this.btnReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnReset.DialogResult = System.Windows.Forms.DialogResult.Abort;
			this.btnReset.Location = new System.Drawing.Point(575, 7);
			this.btnReset.Name = "btnReset";
			this.btnReset.Size = new System.Drawing.Size(75, 23);
			this.btnReset.TabIndex = 2;
			this.btnReset.Text = "Reset";
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(494, 7);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "Cancel";
			this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Location = new System.Drawing.Point(413, 7);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 23);
			this.btnOk.TabIndex = 0;
			this.btnOk.Text = "OK";
			this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl1.Controls.Add(this.rgResourceTypeSelector);
			this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panelControl1.Location = new System.Drawing.Point(12, 372);
			this.panelControl1.Name = "panelControl1";
			this.panelControl1.Size = new System.Drawing.Size(612, 38);
			this.panelControl1.TabIndex = 1;
			this.rgResourceTypeSelector.Location = new System.Drawing.Point(-1, 13);
			this.rgResourceTypeSelector.Name = "rgResourceTypeSelector";
			this.rgResourceTypeSelector.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
			this.rgResourceTypeSelector.Properties.Appearance.Options.UseBackColor = true;
			this.rgResourceTypeSelector.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.rgResourceTypeSelector.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
			new DevExpress.XtraEditors.Controls.RadioGroupItem(0, "Add to project resources"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(1, "Add to form resources")});
			this.rgResourceTypeSelector.Size = new System.Drawing.Size(342, 26);
			this.rgResourceTypeSelector.TabIndex = 2;
			this.pnlContent.Appearance.BackColor = System.Drawing.Color.White;
			this.pnlContent.Appearance.Options.UseBackColor = true;
			this.pnlContent.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlContent.Controls.Add(this.dxImageGalleryControl);
			this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlContent.Location = new System.Drawing.Point(0, 0);
			this.pnlContent.Name = "pnlContent";
			this.pnlContent.Padding = new System.Windows.Forms.Padding(8, 4, 8, 10);
			this.pnlContent.Size = new System.Drawing.Size(662, 462);
			this.pnlContent.TabIndex = 3;
			this.dxImageGalleryControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dxImageGalleryControl.Location = new System.Drawing.Point(8, 4);
			this.dxImageGalleryControl.Margin = new System.Windows.Forms.Padding(0);
			this.dxImageGalleryControl.Name = "dxImageGalleryControl";
			this.dxImageGalleryControl.Size = new System.Drawing.Size(646, 448);
			this.dxImageGalleryControl.TabIndex = 0;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(662, 504);
			this.Controls.Add(this.pnlContent);
			this.Controls.Add(this.pnlBottom);
			this.LookAndFeel.UseDefaultLookAndFeel = false;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(540, 470);
			this.Name = "IconEditorForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Icon Picker";
			((System.ComponentModel.ISupportInitialize)(this.pnlBottom)).EndInit();
			this.pnlBottom.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
			this.panelControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.rgResourceTypeSelector.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlContent)).EndInit();
			this.pnlContent.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		private XtraEditors.PanelControl pnlBottom;
		private XtraEditors.SimpleButton btnCancel;
		private XtraEditors.SimpleButton btnOk;
		private IconEditorControl dxImageGalleryControl;
		private XtraEditors.PanelControl pnlContent;
		private XtraEditors.PanelControl panelControl1;
		private XtraEditors.RadioGroup rgResourceTypeSelector;
		private XtraEditors.SimpleButton btnReset;
	}
}
