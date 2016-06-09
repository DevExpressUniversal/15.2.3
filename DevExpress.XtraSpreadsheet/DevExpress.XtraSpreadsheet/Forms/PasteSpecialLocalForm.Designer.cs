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

namespace DevExpress.XtraSpreadsheet.Forms {
	partial class PasteSpecialLocalForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PasteSpecialLocalForm));
			this.rg_paste = new DevExpress.XtraEditors.RadioGroup();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.lblPaste = new DevExpress.XtraEditors.LabelControl();
			this.cbSkipBlanks = new DevExpress.XtraEditors.CheckEdit();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			((System.ComponentModel.ISupportInitialize)(this.rg_paste.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbSkipBlanks.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.rg_paste, "rg_paste");
			this.rg_paste.Name = "rg_paste";
			this.rg_paste.Properties.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("rg_paste.Properties.Appearance.BackColor")));
			this.rg_paste.Properties.Appearance.Options.UseBackColor = true;
			this.rg_paste.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.rg_paste.Properties.Columns = 2;
			this.rg_paste.Properties.GlyphAlignment = DevExpress.Utils.HorzAlignment.Default;
			this.rg_paste.Properties.DoubleClick += new System.EventHandler(this.rg_paste_Properties_DoubleClick);
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.Name = "btnCancel";
			resources.ApplyResources(this.lblPaste, "lblPaste");
			this.lblPaste.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblPaste.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("lblLeader.Appearance.BackColor")));
			this.lblPaste.LineVisible = true;
			this.lblPaste.Name = "lblPaste";
			resources.ApplyResources(this.cbSkipBlanks, "cbSkipBlanks");
			this.cbSkipBlanks.Name = "cbSkipBlanks";
			this.cbSkipBlanks.Properties.AutoWidth = true;
			this.cbSkipBlanks.Properties.Caption = resources.GetString("cbSkipBlanks.Properties.Caption");
			resources.ApplyResources(this.labelControl1, "labelControl1");
			this.labelControl1.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("labelControl1.Appearance.BackColor")));
			this.labelControl1.LineVisible = true;
			this.labelControl1.Name = "labelControl1";
			this.AcceptButton = this.btnOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.labelControl1);
			this.Controls.Add(this.cbSkipBlanks);
			this.Controls.Add(this.lblPaste);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.rg_paste);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PasteSpecialLocalForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)(this.rg_paste.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbSkipBlanks.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private XtraEditors.RadioGroup rg_paste;
		private XtraEditors.SimpleButton btnOk;
		private XtraEditors.SimpleButton btnCancel;
		private XtraEditors.LabelControl lblPaste;
		private XtraEditors.CheckEdit cbSkipBlanks;
		private XtraEditors.LabelControl labelControl1;
	}
}
