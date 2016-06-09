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

namespace DevExpress.XtraScheduler.Forms {
	partial class ShadingSetupForm {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShadingSetupForm));
			this.cbedtColorScheme = new DevExpress.XtraEditors.ImageComboBoxEdit();
			this.chklbApplyTo = new DevExpress.XtraEditors.CheckedListBoxControl();
			this.lblApplyTo = new DevExpress.XtraEditors.LabelControl();
			this.lblColorScheme = new DevExpress.XtraEditors.LabelControl();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.cbedtColorScheme.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chklbApplyTo)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.cbedtColorScheme, "cbedtColorScheme");
			this.cbedtColorScheme.Name = "cbedtColorScheme";
			this.cbedtColorScheme.Properties.AccessibleName = resources.GetString("cbedtColorScheme.Properties.AccessibleName");
			this.cbedtColorScheme.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.ComboBox;
			this.cbedtColorScheme.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbedtColorScheme.Properties.Buttons"))))});
			this.cbedtColorScheme.SelectedIndexChanged += new System.EventHandler(this.cbedtColorScheme_SelectedIndexChanged);
			resources.ApplyResources(this.chklbApplyTo, "chklbApplyTo");
			this.chklbApplyTo.CheckOnClick = true;
			this.chklbApplyTo.ItemHeight = 16;
			this.chklbApplyTo.Name = "chklbApplyTo";
			resources.ApplyResources(this.lblApplyTo, "lblApplyTo");
			this.lblApplyTo.Name = "lblApplyTo";
			resources.ApplyResources(this.lblColorScheme, "lblColorScheme");
			this.lblColorScheme.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblColorScheme.Name = "lblColorScheme";
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			this.AcceptButton = this.btnOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.chklbApplyTo);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.cbedtColorScheme);
			this.Controls.Add(this.lblColorScheme);
			this.Controls.Add(this.lblApplyTo);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ShadingSetupForm";
			this.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)(this.cbedtColorScheme.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chklbApplyTo)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private System.ComponentModel.IContainer components = null;
		protected DevExpress.XtraEditors.ImageComboBoxEdit cbedtColorScheme;
		protected DevExpress.XtraEditors.SimpleButton btnOk;
		protected DevExpress.XtraEditors.SimpleButton btnCancel;
		protected DevExpress.XtraEditors.LabelControl lblColorScheme;
		protected DevExpress.XtraEditors.CheckedListBoxControl chklbApplyTo;
		protected DevExpress.XtraEditors.LabelControl lblApplyTo;
	}
}
