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
	partial class LineNumberingForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LineNumberingForm));
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.rgrpNumberingRestart = new DevExpress.XtraEditors.RadioGroup();
			this.lblStartAt = new DevExpress.XtraEditors.LabelControl();
			this.chkAddLineNumbering = new DevExpress.XtraEditors.CheckEdit();
			this.edtStartAt = new DevExpress.XtraEditors.SpinEdit();
			this.lblFromText = new DevExpress.XtraEditors.LabelControl();
			this.edtCountBy = new DevExpress.XtraEditors.SpinEdit();
			this.lblCountBy = new DevExpress.XtraEditors.LabelControl();
			this.lblNumbering = new DevExpress.XtraEditors.LabelControl();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.edtFromText = new DevExpress.XtraRichEdit.Design.RichTextIndentEdit();
			((System.ComponentModel.ISupportInitialize)(this.rgrpNumberingRestart.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkAddLineNumbering.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtStartAt.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtCountBy.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtFromText.Properties)).BeginInit();
			this.SuspendLayout();
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.Name = "btnCancel";
			resources.ApplyResources(this.rgrpNumberingRestart, "rgrpNumberingRestart");
			this.rgrpNumberingRestart.Name = "rgrpNumberingRestart";
			this.rgrpNumberingRestart.Properties.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("rgrpNumberingRestart.Properties.Appearance.BackColor")));
			this.rgrpNumberingRestart.Properties.Appearance.Options.UseBackColor = true;
			this.rgrpNumberingRestart.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.rgrpNumberingRestart.Properties.Columns = 1;
			this.rgrpNumberingRestart.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, resources.GetString("rgrpNumberingRestart.Properties.Items")),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, resources.GetString("rgrpNumberingRestart.Properties.Items1")),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, resources.GetString("rgrpNumberingRestart.Properties.Items2"))});
			this.lblStartAt.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			resources.ApplyResources(this.lblStartAt, "lblStartAt");
			this.lblStartAt.Name = "lblStartAt";
			resources.ApplyResources(this.chkAddLineNumbering, "chkAddLineNumbering");
			this.chkAddLineNumbering.Name = "chkAddLineNumbering";
			this.chkAddLineNumbering.Properties.AccessibleName = resources.GetString("chkAddLineNumbering.Properties.AccessibleName");
			this.chkAddLineNumbering.Properties.AutoWidth = true;
			this.chkAddLineNumbering.Properties.Caption = resources.GetString("chkAddLineNumbering.Properties.Caption");
			resources.ApplyResources(this.edtStartAt, "edtStartAt");
			this.edtStartAt.Name = "edtStartAt";
			this.edtStartAt.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
			this.edtStartAt.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.edtStartAt.Properties.IsFloatValue = false;
			this.edtStartAt.Properties.Mask.EditMask = resources.GetString("edtStartAt.Properties.Mask.EditMask");
			this.edtStartAt.Properties.MaxValue = new decimal(new int[] {
			1000,
			0,
			0,
			0});
			this.edtStartAt.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.lblFromText.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			resources.ApplyResources(this.lblFromText, "lblFromText");
			this.lblFromText.Name = "lblFromText";
			resources.ApplyResources(this.edtCountBy, "edtCountBy");
			this.edtCountBy.Name = "edtCountBy";
			this.edtCountBy.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
			this.edtCountBy.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.edtCountBy.Properties.IsFloatValue = false;
			this.edtCountBy.Properties.Mask.EditMask = resources.GetString("edtCountBy.Properties.Mask.EditMask");
			this.edtCountBy.Properties.MaxValue = new decimal(new int[] {
			1000,
			0,
			0,
			0});
			this.edtCountBy.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.lblCountBy.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			resources.ApplyResources(this.lblCountBy, "lblCountBy");
			this.lblCountBy.Name = "lblCountBy";
			this.lblNumbering.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			resources.ApplyResources(this.lblNumbering, "lblNumbering");
			this.lblNumbering.Name = "lblNumbering";
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.OnBtnOkClick);
			resources.ApplyResources(this.edtFromText, "edtFromText");
			this.edtFromText.Name = "edtFromText";
			this.edtFromText.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
			this.edtFromText.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.edtFromText.Properties.IsValueInPercent = false;
			this.AcceptButton = this.btnOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.lblNumbering);
			this.Controls.Add(this.lblCountBy);
			this.Controls.Add(this.edtCountBy);
			this.Controls.Add(this.edtFromText);
			this.Controls.Add(this.lblFromText);
			this.Controls.Add(this.edtStartAt);
			this.Controls.Add(this.chkAddLineNumbering);
			this.Controls.Add(this.lblStartAt);
			this.Controls.Add(this.rgrpNumberingRestart);
			this.Controls.Add(this.btnCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "LineNumberingForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)(this.rgrpNumberingRestart.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkAddLineNumbering.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtStartAt.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtCountBy.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtFromText.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected DevExpress.XtraEditors.SimpleButton btnCancel;
		protected DevExpress.XtraEditors.RadioGroup rgrpNumberingRestart;
		protected DevExpress.XtraEditors.LabelControl lblStartAt;
		protected DevExpress.XtraEditors.CheckEdit chkAddLineNumbering;
		protected DevExpress.XtraEditors.SpinEdit edtStartAt;
		protected DevExpress.XtraEditors.LabelControl lblFromText;
		protected DevExpress.XtraRichEdit.Design.RichTextIndentEdit edtFromText;
		protected DevExpress.XtraEditors.SpinEdit edtCountBy;
		protected DevExpress.XtraEditors.LabelControl lblCountBy;
		protected DevExpress.XtraEditors.LabelControl lblNumbering;
		protected DevExpress.XtraEditors.SimpleButton btnOk;
	}
}
