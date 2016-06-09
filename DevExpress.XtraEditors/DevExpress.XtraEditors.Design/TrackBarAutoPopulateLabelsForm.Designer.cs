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

namespace DevExpress.XtraEditors.Design {
	partial class TrackBarAutoPopulateLabelsForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.seStep = new DevExpress.XtraEditors.SpinEdit();
			this.lbStep = new DevExpress.XtraEditors.LabelControl();
			this.lbMinimum = new DevExpress.XtraEditors.LabelControl();
			this.seMinimum = new DevExpress.XtraEditors.SpinEdit();
			this.lbMaximum = new DevExpress.XtraEditors.LabelControl();
			this.seMaximum = new DevExpress.XtraEditors.SpinEdit();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
			((System.ComponentModel.ISupportInitialize)(this.seStep.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.seMinimum.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.seMaximum.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
			this.panelControl1.SuspendLayout();
			this.SuspendLayout();
			this.seStep.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.seStep.EditValue = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.seStep.Location = new System.Drawing.Point(123, 87);
			this.seStep.Name = "seStep";
			this.seStep.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.seStep.Properties.Mask.EditMask = "n0";
			this.seStep.Properties.MaxValue = new decimal(new int[] {
			1000000,
			0,
			0,
			0});
			this.seStep.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.seStep.Size = new System.Drawing.Size(179, 20);
			this.seStep.TabIndex = 5;
			this.lbStep.Location = new System.Drawing.Point(52, 90);
			this.lbStep.Name = "lbStep";
			this.lbStep.Size = new System.Drawing.Size(26, 13);
			this.lbStep.TabIndex = 4;
			this.lbStep.Text = "Step:";
			this.lbMinimum.Location = new System.Drawing.Point(52, 42);
			this.lbMinimum.Name = "lbMinimum";
			this.lbMinimum.Size = new System.Drawing.Size(44, 13);
			this.lbMinimum.TabIndex = 0;
			this.lbMinimum.Text = "Minimum:";
			this.seMinimum.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.seMinimum.EditValue = new decimal(new int[] {
			0,
			0,
			0,
			0});
			this.seMinimum.Location = new System.Drawing.Point(123, 39);
			this.seMinimum.Name = "seMinimum";
			this.seMinimum.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.seMinimum.Properties.Mask.EditMask = "n0";
			this.seMinimum.Size = new System.Drawing.Size(179, 20);
			this.seMinimum.TabIndex = 1;
			this.lbMaximum.Location = new System.Drawing.Point(52, 66);
			this.lbMaximum.Name = "lbMaximum";
			this.lbMaximum.Size = new System.Drawing.Size(48, 13);
			this.lbMaximum.TabIndex = 2;
			this.lbMaximum.Text = "Maximum:";
			this.seMaximum.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.seMaximum.EditValue = new decimal(new int[] {
			0,
			0,
			0,
			0});
			this.seMaximum.Location = new System.Drawing.Point(123, 63);
			this.seMaximum.Name = "seMaximum";
			this.seMaximum.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.seMaximum.Properties.Mask.EditMask = "n0";
			this.seMaximum.Size = new System.Drawing.Size(179, 20);
			this.seMaximum.TabIndex = 3;
			this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOk.Location = new System.Drawing.Point(186, 21);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 23);
			this.btnOk.TabIndex = 7;
			this.btnOk.Text = "OK";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(267, 21);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 8;
			this.btnCancel.Text = "Cancel";
			this.labelControl1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.labelControl1.Dock = System.Windows.Forms.DockStyle.Top;
			this.labelControl1.LineVisible = true;
			this.labelControl1.Location = new System.Drawing.Point(2, 2);
			this.labelControl1.Name = "labelControl1";
			this.labelControl1.Size = new System.Drawing.Size(350, 13);
			this.labelControl1.TabIndex = 9;
			this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl1.Controls.Add(this.btnOk);
			this.panelControl1.Controls.Add(this.labelControl1);
			this.panelControl1.Controls.Add(this.btnCancel);
			this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panelControl1.Location = new System.Drawing.Point(0, 142);
			this.panelControl1.Name = "panelControl1";
			this.panelControl1.Padding = new System.Windows.Forms.Padding(2);
			this.panelControl1.Size = new System.Drawing.Size(354, 56);
			this.panelControl1.TabIndex = 10;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(354, 198);
			this.Controls.Add(this.panelControl1);
			this.Controls.Add(this.lbMaximum);
			this.Controls.Add(this.seMaximum);
			this.Controls.Add(this.lbMinimum);
			this.Controls.Add(this.seMinimum);
			this.Controls.Add(this.lbStep);
			this.Controls.Add(this.seStep);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.LookAndFeel.UseDefaultLookAndFeel = false;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TrackBarAutoPopulateLabelsForm";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Populate Tick Labels Collection";
			((System.ComponentModel.ISupportInitialize)(this.seStep.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.seMinimum.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.seMaximum.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
			this.panelControl1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private SpinEdit seStep;
		private LabelControl lbStep;
		private LabelControl lbMinimum;
		private SpinEdit seMinimum;
		private LabelControl lbMaximum;
		private SpinEdit seMaximum;
		private SimpleButton btnOk;
		private SimpleButton btnCancel;
		private LabelControl labelControl1;
		private PanelControl panelControl1;
	}
}
