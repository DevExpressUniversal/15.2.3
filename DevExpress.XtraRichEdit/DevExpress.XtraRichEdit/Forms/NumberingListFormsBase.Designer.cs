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

using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using System.ComponentModel;
namespace DevExpress.XtraRichEdit.Forms {
	partial class NumberingListFormsBase {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnFont = new DevExpress.XtraEditors.SimpleButton();
			this.lblAlignedAt = new DevExpress.XtraEditors.LabelControl();
			this.lblIndentAt = new DevExpress.XtraEditors.LabelControl();
			this.edtAligned = new DevExpress.XtraRichEdit.Design.RichTextIndentEdit();
			this.edtIndent = new DevExpress.XtraRichEdit.Design.RichTextIndentEdit();
			((System.ComponentModel.ISupportInitialize)(this.edtIndent.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtAligned.Properties)).BeginInit();
			this.SuspendLayout();
			this.btnFont.Name = "btnFont";
			this.btnFont.Click += new System.EventHandler(this.OnFontClick);
			this.lblAlignedAt.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblAlignedAt.Name = "lblAlignedAt";
			this.lblIndentAt.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblIndentAt.Name = "lblIndentAt";
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.OnOkClick);
			this.btnCancel.CausesValidation = false;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			this.edtIndent.Name = "edtIndent";
			this.edtIndent.Properties.AllowNullInput = DefaultBoolean.False;
			this.edtIndent.Properties.Buttons.AddRange(new EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.edtIndent.Properties.MaxValue = 6599;
			this.edtIndent.Properties.MinValue = -6599;
			this.edtAligned.Name = "edtAligned";
			this.edtAligned.Properties.AllowNullInput = DefaultBoolean.False;
			this.edtAligned.Properties.Buttons.AddRange(new EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.edtAligned.Properties.MaxValue = 6600;
			this.edtAligned.Properties.MinValue = -6600;
			this.AcceptButton = this.btnOk;
			this.AccessibleRole = System.Windows.Forms.AccessibleRole.Window;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.edtIndent);
			this.Controls.Add(this.lblIndentAt);
			this.Controls.Add(this.edtAligned);
			this.Controls.Add(this.lblAlignedAt);
			this.Controls.Add(this.btnFont);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "NumberingListFormBase";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			((ISupportInitialize)(this.edtIndent.Properties)).EndInit();
			((ISupportInitialize)(this.edtAligned.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		protected DevExpress.XtraEditors.SimpleButton btnOk;
		protected DevExpress.XtraEditors.SimpleButton btnCancel;
		protected DevExpress.XtraEditors.SimpleButton btnFont;
		protected DevExpress.XtraRichEdit.Design.RichTextIndentEdit edtAligned;
		protected DevExpress.XtraRichEdit.Design.RichTextIndentEdit edtIndent;
		protected DevExpress.XtraEditors.LabelControl lblAlignedAt;
		protected DevExpress.XtraEditors.LabelControl lblIndentAt;
		#endregion
	}
}
