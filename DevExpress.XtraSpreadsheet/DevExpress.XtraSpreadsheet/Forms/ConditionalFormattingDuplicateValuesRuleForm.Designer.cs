﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
	partial class ConditionalFormattingDuplicateValuesRuleForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConditionalFormattingDuplicateValuesRuleForm));
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.cbFormat = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblWith = new DevExpress.XtraEditors.LabelControl();
			this.lblHeader = new DevExpress.XtraEditors.LabelControl();
			this.edtComparer = new DevExpress.XtraEditors.LookUpEdit();
			((System.ComponentModel.ISupportInitialize)(this.cbFormat.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtComparer.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			resources.ApplyResources(this.cbFormat, "cbFormat");
			this.cbFormat.Name = "cbFormat";
			this.cbFormat.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbFormat.Properties.Buttons"))))});
			this.cbFormat.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			resources.ApplyResources(this.lblWith, "lblWith");
			this.lblWith.Name = "lblWith";
			this.lblHeader.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("lblHeader.Appearance.Font")));
			resources.ApplyResources(this.lblHeader, "lblHeader");
			this.lblHeader.Name = "lblHeader";
			resources.ApplyResources(this.edtComparer, "edtComparer");
			this.edtComparer.Name = "edtComparer";
			this.edtComparer.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtComparer.Properties.Buttons"))))});
			this.edtComparer.Properties.NullText = resources.GetString("edtComparer.Properties.NullText");
			this.edtComparer.Properties.ShowFooter = false;
			this.edtComparer.Properties.ShowHeader = false;
			this.AcceptButton = this.btnOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.edtComparer);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.cbFormat);
			this.Controls.Add(this.lblWith);
			this.Controls.Add(this.lblHeader);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ConditionalFormattingDuplicateValuesRuleForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)(this.cbFormat.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtComparer.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private XtraEditors.SimpleButton btnCancel;
		private XtraEditors.SimpleButton btnOk;
		private XtraEditors.ComboBoxEdit cbFormat;
		private XtraEditors.LabelControl lblWith;
		private XtraEditors.LabelControl lblHeader;
		private XtraEditors.LookUpEdit edtComparer;
	}
}
