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
	partial class SplitTableCellsForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplitTableCellsForm));
			this.lblNumberOfColumns = new DevExpress.XtraEditors.LabelControl();
			this.lblNumberOfRows = new DevExpress.XtraEditors.LabelControl();
			this.spnNumberOfColumns = new DevExpress.XtraEditors.SpinEdit();
			this.spnNumberOfRows = new DevExpress.XtraEditors.SpinEdit();
			this.chkMergeBeforeSplit = new DevExpress.XtraEditors.CheckEdit();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.spnNumberOfColumns.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.spnNumberOfRows.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkMergeBeforeSplit.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.lblNumberOfColumns, "lblNumberOfColumns");
			this.lblNumberOfColumns.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblNumberOfColumns.Name = "lblNumberOfColumns";
			resources.ApplyResources(this.lblNumberOfRows, "lblNumberOfRows");
			this.lblNumberOfRows.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblNumberOfRows.Name = "lblNumberOfRows";
			this.spnNumberOfColumns.CausesValidation = false;
			resources.ApplyResources(this.spnNumberOfColumns, "spnNumberOfColumns");
			this.spnNumberOfColumns.Name = "spnNumberOfColumns";
			this.spnNumberOfColumns.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
			this.spnNumberOfColumns.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnNumberOfColumns.Properties.IsFloatValue = false;
			this.spnNumberOfColumns.Properties.Mask.EditMask = resources.GetString("spnNumberOfColumns.Properties.Mask.EditMask");
			this.spnNumberOfColumns.Properties.MaxValue = new decimal(new int[] {
			1000,
			0,
			0,
			0});
			this.spnNumberOfColumns.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.spnNumberOfColumns.TextChanged += new System.EventHandler(this.OnSpnNumberOfColumnsTextChanged);
			this.spnNumberOfRows.CausesValidation = false;
			resources.ApplyResources(this.spnNumberOfRows, "spnNumberOfRows");
			this.spnNumberOfRows.Name = "spnNumberOfRows";
			this.spnNumberOfRows.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
			this.spnNumberOfRows.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnNumberOfRows.Properties.IsFloatValue = false;
			this.spnNumberOfRows.Properties.Mask.EditMask = resources.GetString("spnNumberOfRows.Properties.Mask.EditMask");
			this.spnNumberOfRows.Properties.MaxValue = new decimal(new int[] {
			1000,
			0,
			0,
			0});
			this.spnNumberOfRows.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.spnNumberOfRows.Spin += new DevExpress.XtraEditors.Controls.SpinEventHandler(this.OnSpnNumberOfRowsSpin);
			this.spnNumberOfRows.TextChanged += new System.EventHandler(this.OnSpnNumberOfRowsTextChanged);
			resources.ApplyResources(this.chkMergeBeforeSplit, "chkMergeBeforeSplit");
			this.chkMergeBeforeSplit.Name = "chkMergeBeforeSplit";
			this.chkMergeBeforeSplit.Properties.AutoWidth = true;
			this.chkMergeBeforeSplit.Properties.Caption = resources.GetString("chkMergeBeforeSplit.Properties.Caption");
			this.chkMergeBeforeSplit.CheckedChanged += new System.EventHandler(this.OnChkMergeBeforeSplitCheckedChanged);
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.OnBtnOkClick);
			this.AcceptButton = this.btnOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.chkMergeBeforeSplit);
			this.Controls.Add(this.spnNumberOfRows);
			this.Controls.Add(this.spnNumberOfColumns);
			this.Controls.Add(this.lblNumberOfRows);
			this.Controls.Add(this.lblNumberOfColumns);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SplitTableCellsForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)(this.spnNumberOfColumns.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.spnNumberOfRows.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkMergeBeforeSplit.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected DevExpress.XtraEditors.LabelControl lblNumberOfColumns;
		protected DevExpress.XtraEditors.LabelControl lblNumberOfRows;
		protected DevExpress.XtraEditors.SpinEdit spnNumberOfColumns;
		protected DevExpress.XtraEditors.SpinEdit spnNumberOfRows;
		protected DevExpress.XtraEditors.CheckEdit chkMergeBeforeSplit;
		protected DevExpress.XtraEditors.SimpleButton btnCancel;
		protected DevExpress.XtraEditors.SimpleButton btnOk;
	}
}
