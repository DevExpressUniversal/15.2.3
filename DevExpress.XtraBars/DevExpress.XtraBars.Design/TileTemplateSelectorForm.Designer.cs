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

namespace DevExpress.XtraBars.Design {
	partial class TileTemplateSelectorForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.tileControl = new DevExpress.XtraEditors.TileControl();
			this.panelFooter = new DevExpress.XtraEditors.PanelControl();
			this.label1 = new DevExpress.XtraEditors.LabelControl();
			this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.panelFooter)).BeginInit();
			this.panelFooter.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
			this.panelControl1.SuspendLayout();
			this.SuspendLayout();
			this.tileControl.AllowDrag = false;
			this.tileControl.AppearanceGroupText.Font = new System.Drawing.Font("Segoe UI Semilight", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tileControl.AppearanceGroupText.ForeColor = System.Drawing.Color.White;
			this.tileControl.AppearanceGroupText.Options.UseFont = true;
			this.tileControl.AppearanceGroupText.Options.UseForeColor = true;
			this.tileControl.AppearanceText.Font = new System.Drawing.Font("Segoe UI Semilight", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tileControl.AppearanceText.ForeColor = System.Drawing.Color.White;
			this.tileControl.AppearanceText.Options.UseFont = true;
			this.tileControl.AppearanceText.Options.UseForeColor = true;
			this.tileControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tileControl.ItemCheckMode = DevExpress.XtraEditors.TileItemCheckMode.Single;
			this.tileControl.Location = new System.Drawing.Point(0, 0);
			this.tileControl.Name = "tileControl";
			this.tileControl.ScrollMode = DevExpress.XtraEditors.TileControlScrollMode.ScrollButtons;
			this.tileControl.ShowGroupText = true;
			this.tileControl.Size = new System.Drawing.Size(928, 555);
			this.tileControl.TabIndex = 0;
			this.tileControl.Text = "tileControl1";
			this.tileControl.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(this.tileControl1_ItemClick);
			this.tileControl.ItemCheckedChanged += new DevExpress.XtraEditors.TileItemClickEventHandler(this.tileControl1_ItemCheckedChanged);
			this.panelFooter.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelFooter.Controls.Add(this.label1);
			this.panelFooter.Controls.Add(this.panelControl1);
			this.panelFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panelFooter.Location = new System.Drawing.Point(0, 555);
			this.panelFooter.Name = "panelFooter";
			this.panelFooter.Size = new System.Drawing.Size(928, 46);
			this.panelFooter.TabIndex = 1;
			this.label1.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			this.label1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
			this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label1.Location = new System.Drawing.Point(0, 0);
			this.label1.Name = "label1";
			this.label1.Padding = new System.Windows.Forms.Padding(10, 17, 10, 10);
			this.label1.Size = new System.Drawing.Size(746, 40);
			this.label1.TabIndex = 2;
			this.label1.Text = "The Tile Template Gallery allows you to apply a predefined template to tiles. Not" +
	"e that if a template is applied, all existing tile elements will be cleared.";
			this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl1.Controls.Add(this.btnCancel);
			this.panelControl1.Controls.Add(this.btnOk);
			this.panelControl1.Dock = System.Windows.Forms.DockStyle.Right;
			this.panelControl1.Location = new System.Drawing.Point(746, 0);
			this.panelControl1.Name = "panelControl1";
			this.panelControl1.Size = new System.Drawing.Size(182, 46);
			this.panelControl1.TabIndex = 3;
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(95, 11);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "Cancel";
			this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Enabled = false;
			this.btnOk.Location = new System.Drawing.Point(14, 11);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 23);
			this.btnOk.TabIndex = 0;
			this.btnOk.Text = "Apply";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(928, 601);
			this.Controls.Add(this.tileControl);
			this.Controls.Add(this.panelFooter);
			this.LookAndFeel.SkinName = "DevExpress Design";
			this.Name = "TileTemplateSelectorForm";
			this.ShowIcon = false;
			this.Text = "TileItem Template Gallery";
			((System.ComponentModel.ISupportInitialize)(this.panelFooter)).EndInit();
			this.panelFooter.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
			this.panelControl1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		private XtraEditors.TileControl tileControl;
		private XtraEditors.PanelControl panelFooter;
		private XtraEditors.SimpleButton btnCancel;
		private XtraEditors.SimpleButton btnOk;
		private XtraEditors.LabelControl label1;
		private XtraEditors.PanelControl panelControl1;
	}
}
