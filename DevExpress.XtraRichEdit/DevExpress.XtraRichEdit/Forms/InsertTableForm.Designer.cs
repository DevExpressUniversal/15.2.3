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
	partial class InsertTableForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InsertTableForm));
			this.lblTableSize = new DevExpress.XtraEditors.LabelControl();
			this.lblColumns = new DevExpress.XtraEditors.LabelControl();
			this.lblRows = new DevExpress.XtraEditors.LabelControl();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.spinColumns = new DevExpress.XtraEditors.SpinEdit();
			this.spinRows = new DevExpress.XtraEditors.SpinEdit();
			((System.ComponentModel.ISupportInitialize)(this.spinColumns.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.spinRows.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.lblTableSize, "lblTableSize");
			this.lblTableSize.LineVisible = true;
			this.lblTableSize.Name = "lblTableSize";
			resources.ApplyResources(this.lblColumns, "lblColumns");
			this.lblColumns.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblColumns.Name = "lblColumns";
			resources.ApplyResources(this.lblRows, "lblRows");
			this.lblRows.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblRows.Name = "lblRows";
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.CausesValidation = false;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			resources.ApplyResources(this.spinColumns, "spinColumns");
			this.spinColumns.Name = "spinColumns";
			this.spinColumns.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
			this.spinColumns.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spinColumns.Properties.IsFloatValue = false;
			this.spinColumns.Properties.Mask.EditMask = resources.GetString("spinColumns.Properties.Mask.EditMask");
			this.spinColumns.Properties.MaxValue = new decimal(new int[] {
			1000,
			0,
			0,
			0});
			this.spinColumns.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			0});
			resources.ApplyResources(this.spinRows, "spinRows");
			this.spinRows.Name = "spinRows";
			this.spinRows.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
			this.spinRows.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spinRows.Properties.IsFloatValue = false;
			this.spinRows.Properties.Mask.EditMask = resources.GetString("spinRows.Properties.Mask.EditMask");
			this.spinRows.Properties.MaxValue = new decimal(new int[] {
			1000,
			0,
			0,
			0});
			this.spinRows.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.AcceptButton = this.btnOk;
			this.AccessibleRole = System.Windows.Forms.AccessibleRole.Window;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.spinRows);
			this.Controls.Add(this.spinColumns);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.lblRows);
			this.Controls.Add(this.lblColumns);
			this.Controls.Add(this.lblTableSize);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "InsertTableForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)(this.spinColumns.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.spinRows.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected DevExpress.XtraEditors.LabelControl lblTableSize;
		protected DevExpress.XtraEditors.LabelControl lblColumns;
		protected DevExpress.XtraEditors.LabelControl lblRows;
		protected DevExpress.XtraEditors.SimpleButton btnOk;
		protected DevExpress.XtraEditors.SimpleButton btnCancel;
		protected DevExpress.XtraEditors.SpinEdit spinColumns;
		protected DevExpress.XtraEditors.SpinEdit spinRows;
	}
}
