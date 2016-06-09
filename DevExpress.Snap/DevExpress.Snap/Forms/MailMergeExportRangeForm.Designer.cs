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

namespace DevExpress.Snap.Forms {
	partial class MailMergeExportRangeForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MailMergeExportRangeForm));
			this.radioGroupMode = new DevExpress.XtraEditors.RadioGroup();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.spinEditCount = new DevExpress.XtraEditors.SpinEdit();
			this.spinEditFrom = new DevExpress.XtraEditors.SpinEdit();
			this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
			this.simpleButton2 = new DevExpress.XtraEditors.SimpleButton();
			this.icbSeparator = new DevExpress.XtraEditors.ImageComboBoxEdit();
			this.lblSeparator = new DevExpress.XtraEditors.LabelControl();
			((System.ComponentModel.ISupportInitialize)(this.radioGroupMode.Properties)).BeginInit();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.spinEditCount.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.spinEditFrom.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.icbSeparator.Properties)).BeginInit();
			this.SuspendLayout();
			this.radioGroupMode.AllowDrop = true;
			resources.ApplyResources(this.radioGroupMode, "radioGroupMode");
			this.radioGroupMode.AutoSizeInLayoutControl = true;
			this.radioGroupMode.Name = "radioGroupMode";
			this.radioGroupMode.Properties.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("radioGroupMode.Properties.Appearance.BackColor")));
			this.radioGroupMode.Properties.Appearance.Options.UseBackColor = true;
			this.radioGroupMode.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.radioGroupMode.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
			new DevExpress.XtraEditors.Controls.RadioGroupItem(((object)(resources.GetObject("radioGroupMode.Properties.Items"))), resources.GetString("radioGroupMode.Properties.Items1")),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(((object)(resources.GetObject("radioGroupMode.Properties.Items2"))), resources.GetString("radioGroupMode.Properties.Items3")),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(((object)(resources.GetObject("radioGroupMode.Properties.Items4"))), resources.GetString("radioGroupMode.Properties.Items5"))});
			this.radioGroupMode.SelectedIndexChanged += new System.EventHandler(this.radioGroup1_SelectedIndexChanged);
			this.groupBox1.Controls.Add(this.labelControl2);
			this.groupBox1.Controls.Add(this.labelControl1);
			this.groupBox1.Controls.Add(this.spinEditCount);
			this.groupBox1.Controls.Add(this.spinEditFrom);
			resources.ApplyResources(this.groupBox1, "groupBox1");
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.TabStop = false;
			resources.ApplyResources(this.labelControl2, "labelControl2");
			this.labelControl2.Name = "labelControl2";
			resources.ApplyResources(this.labelControl1, "labelControl1");
			this.labelControl1.Name = "labelControl1";
			resources.ApplyResources(this.spinEditCount, "spinEditCount");
			this.spinEditCount.Name = "spinEditCount";
			this.spinEditCount.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("spinEditCount.Properties.Buttons"))))});
			this.spinEditCount.Properties.Mask.EditMask = resources.GetString("spinEditCount.Properties.Mask.EditMask");
			this.spinEditCount.Properties.MaxValue = new decimal(new int[] {
			2147483647,
			0,
			0,
			0});
			this.spinEditCount.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.spinEditCount.EditValueChanged += new System.EventHandler(this.spinEdit2_EditValueChanged);
			resources.ApplyResources(this.spinEditFrom, "spinEditFrom");
			this.spinEditFrom.Name = "spinEditFrom";
			this.spinEditFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("spinEditFrom.Properties.Buttons"))))});
			this.spinEditFrom.Properties.Mask.EditMask = resources.GetString("spinEditFrom.Properties.Mask.EditMask");
			this.spinEditFrom.Properties.MaxValue = new decimal(new int[] {
			2147483647,
			0,
			0,
			0});
			this.spinEditFrom.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.spinEditFrom.EditValueChanged += new System.EventHandler(this.spinEdit1_EditValueChanged);
			resources.ApplyResources(this.simpleButton1, "simpleButton1");
			this.simpleButton1.Name = "simpleButton1";
			this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
			this.simpleButton2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.simpleButton2, "simpleButton2");
			this.simpleButton2.Name = "simpleButton2";
			this.simpleButton2.Click += new System.EventHandler(this.simpleButton2_Click);
			resources.ApplyResources(this.icbSeparator, "icbSeparator");
			this.icbSeparator.Name = "icbSeparator";
			this.icbSeparator.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("icbSeparator.Properties.Buttons"))))});
			this.icbSeparator.SelectedIndexChanged += new System.EventHandler(this.icbSeparator_SelectedIndexChanged);
			resources.ApplyResources(this.lblSeparator, "lblSeparator");
			this.lblSeparator.Name = "lblSeparator";
			this.AcceptButton = this.simpleButton1;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.simpleButton2;
			this.Controls.Add(this.lblSeparator);
			this.Controls.Add(this.icbSeparator);
			this.Controls.Add(this.radioGroupMode);
			this.Controls.Add(this.simpleButton2);
			this.Controls.Add(this.simpleButton1);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MailMergeExportForm";
			this.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)(this.radioGroupMode.Properties)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.spinEditCount.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.spinEditFrom.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.icbSeparator.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected System.Windows.Forms.GroupBox groupBox1;
		protected XtraEditors.LabelControl labelControl2;
		protected XtraEditors.LabelControl labelControl1;
		protected XtraEditors.SimpleButton simpleButton1;
		protected XtraEditors.SimpleButton simpleButton2;
		protected XtraEditors.RadioGroup radioGroupMode;
		protected XtraEditors.SpinEdit spinEditCount;
		protected XtraEditors.SpinEdit spinEditFrom;
		protected XtraEditors.ImageComboBoxEdit icbSeparator;
		protected XtraEditors.LabelControl lblSeparator;
	}
}
