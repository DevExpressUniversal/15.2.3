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

namespace DevExpress.XtraRichEdit.Forms {
	partial class InsertMergeFieldForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InsertMergeFieldForm));
			this.lblInsert = new DevExpress.XtraEditors.LabelControl();
			this.btnInsert = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.rgFieldsSource = new DevExpress.XtraEditors.RadioGroup();
			this.lblFields = new DevExpress.XtraEditors.LabelControl();
			this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
			this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
			this.lbMergeFields = new DevExpress.XtraEditors.ListBoxControl();
			((System.ComponentModel.ISupportInitialize)(this.rgFieldsSource.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
			this.panelControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
			this.panelControl2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.lbMergeFields)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.lblInsert, "lblInsert");
			this.lblInsert.Name = "lblInsert";
			resources.ApplyResources(this.btnInsert, "btnInsert");
			this.btnInsert.Name = "btnInsert";
			this.btnInsert.Click += new System.EventHandler(this.btnInsert_Click);
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			resources.ApplyResources(this.rgFieldsSource, "rgFieldsSource");
			this.rgFieldsSource.Name = "rgFieldsSource";
			this.rgFieldsSource.Properties.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("rgFieldsSource.Properties.Appearance.BackColor")));
			this.rgFieldsSource.Properties.Appearance.Options.UseBackColor = true;
			this.rgFieldsSource.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.rgFieldsSource.Properties.Columns = 1;
			this.rgFieldsSource.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, resources.GetString("rgFieldsSource.Properties.Items")),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, resources.GetString("rgFieldsSource.Properties.Items1"), ((bool)(resources.GetObject("rgFieldsSource.Properties.Items2"))))});
			resources.ApplyResources(this.lblFields, "lblFields");
			this.lblFields.Name = "lblFields";
			this.panelControl1.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("panelControl1.Appearance.BackColor")));
			this.panelControl1.Appearance.Options.UseBackColor = true;
			this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl1.Controls.Add(this.lblInsert);
			this.panelControl1.Controls.Add(this.lblFields);
			this.panelControl1.Controls.Add(this.rgFieldsSource);
			resources.ApplyResources(this.panelControl1, "panelControl1");
			this.panelControl1.Name = "panelControl1";
			this.panelControl2.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("panelControl2.Appearance.BackColor")));
			this.panelControl2.Appearance.Options.UseBackColor = true;
			this.panelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl2.Controls.Add(this.btnInsert);
			this.panelControl2.Controls.Add(this.btnCancel);
			resources.ApplyResources(this.panelControl2, "panelControl2");
			this.panelControl2.Name = "panelControl2";
			resources.ApplyResources(this.lbMergeFields, "lbMergeFields");
			this.lbMergeFields.Name = "lbMergeFields";
			this.lbMergeFields.SortOrder = System.Windows.Forms.SortOrder.Ascending;
			this.lbMergeFields.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbMergeFields_MouseDoubleClick);
			this.AcceptButton = this.btnInsert;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.lbMergeFields);
			this.Controls.Add(this.panelControl2);
			this.Controls.Add(this.panelControl1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "InsertMergeFieldForm";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.InsertMergeFieldForm_FormClosed);
			((System.ComponentModel.ISupportInitialize)(this.rgFieldsSource.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
			this.panelControl1.ResumeLayout(false);
			this.panelControl1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
			this.panelControl2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.lbMergeFields)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		protected DevExpress.XtraEditors.LabelControl lblInsert;
		protected DevExpress.XtraEditors.SimpleButton btnInsert;
		protected DevExpress.XtraEditors.SimpleButton btnCancel;
		protected DevExpress.XtraEditors.RadioGroup rgFieldsSource;
		protected DevExpress.XtraEditors.LabelControl lblFields;
		protected DevExpress.XtraEditors.PanelControl panelControl1;
		protected DevExpress.XtraEditors.PanelControl panelControl2;
		protected DevExpress.XtraEditors.ListBoxControl lbMergeFields;
	}
}
