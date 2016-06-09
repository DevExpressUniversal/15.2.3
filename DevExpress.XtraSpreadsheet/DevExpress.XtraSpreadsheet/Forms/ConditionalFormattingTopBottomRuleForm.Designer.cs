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
	partial class ConditionalFormattingTopBottomRuleForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConditionalFormattingTopBottomRuleForm));
			this.spnRank = new DevExpress.XtraEditors.SpinEdit();
			this.lblHeader = new DevExpress.XtraEditors.LabelControl();
			this.lblWith = new DevExpress.XtraEditors.LabelControl();
			this.cbFormat = new DevExpress.XtraEditors.ComboBoxEdit();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.spnRank.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbFormat.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.spnRank, "spnRank");
			this.spnRank.Name = "spnRank";
			this.spnRank.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("spnRank.Properties.Buttons"))))});
			this.spnRank.Properties.IsFloatValue = false;
			this.spnRank.Properties.Mask.EditMask = resources.GetString("spnRank.Properties.Mask.EditMask");
			this.spnRank.Properties.MaxLength = 4;
			this.spnRank.Properties.MaxValue = new decimal(new int[] {
			9999,
			0,
			0,
			0});
			this.spnRank.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.lblHeader.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("lblHeader.Appearance.Font")));
			resources.ApplyResources(this.lblHeader, "lblHeader");
			this.lblHeader.Name = "lblHeader";
			this.lblWith.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			resources.ApplyResources(this.lblWith, "lblWith");
			this.lblWith.Name = "lblWith";
			resources.ApplyResources(this.cbFormat, "cbFormat");
			this.cbFormat.Name = "cbFormat";
			this.cbFormat.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbFormat.Properties.Buttons"))))});
			this.cbFormat.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			this.AcceptButton = this.btnOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.cbFormat);
			this.Controls.Add(this.lblWith);
			this.Controls.Add(this.lblHeader);
			this.Controls.Add(this.spnRank);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ConditionalFormattingTopBottomRuleForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)(this.spnRank.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbFormat.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private XtraEditors.SpinEdit spnRank;
		private XtraEditors.LabelControl lblHeader;
		private XtraEditors.LabelControl lblWith;
		private XtraEditors.ComboBoxEdit cbFormat;
		private XtraEditors.SimpleButton btnOk;
		private XtraEditors.SimpleButton btnCancel;
	}
}
