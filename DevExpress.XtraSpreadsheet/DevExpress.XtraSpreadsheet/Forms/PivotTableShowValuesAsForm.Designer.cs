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
	partial class PivotTableShowValuesAsForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PivotTableShowValuesAsForm));
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.lblCalculation = new DevExpress.XtraEditors.LabelControl();
			this.edtBaseField = new DevExpress.XtraEditors.LookUpEdit();
			this.edtBaseItem = new DevExpress.XtraEditors.LookUpEdit();
			this.lblBaseField = new DevExpress.XtraEditors.LabelControl();
			this.lblBaseItem = new DevExpress.XtraEditors.LabelControl();
			((System.ComponentModel.ISupportInitialize)(this.edtBaseField.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtBaseItem.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			resources.ApplyResources(this.lblCalculation, "lblCalculation");
			this.lblCalculation.Name = "lblCalculation";
			resources.ApplyResources(this.edtBaseField, "edtBaseField");
			this.edtBaseField.Name = "edtBaseField";
			this.edtBaseField.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtBaseField.Properties.Buttons"))))});
			this.edtBaseField.Properties.DropDownRows = 6;
			this.edtBaseField.Properties.NullText = resources.GetString("edtBaseField.Properties.NullText");
			this.edtBaseField.Properties.ShowFooter = false;
			this.edtBaseField.Properties.ShowHeader = false;
			resources.ApplyResources(this.edtBaseItem, "edtBaseItem");
			this.edtBaseItem.Name = "edtBaseItem";
			this.edtBaseItem.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtBaseItem.Properties.Buttons"))))});
			this.edtBaseItem.Properties.DropDownRows = 6;
			this.edtBaseItem.Properties.NullText = resources.GetString("edtBaseItem.Properties.NullText");
			this.edtBaseItem.Properties.ShowFooter = false;
			this.edtBaseItem.Properties.ShowHeader = false;
			resources.ApplyResources(this.lblBaseField, "lblBaseField");
			this.lblBaseField.Name = "lblBaseField";
			resources.ApplyResources(this.lblBaseItem, "lblBaseItem");
			this.lblBaseItem.Name = "lblBaseItem";
			this.AcceptButton = this.btnOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.lblBaseItem);
			this.Controls.Add(this.lblBaseField);
			this.Controls.Add(this.edtBaseItem);
			this.Controls.Add(this.edtBaseField);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.lblCalculation);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PivotTableShowValuesAsForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			((System.ComponentModel.ISupportInitialize)(this.edtBaseField.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtBaseItem.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private XtraEditors.SimpleButton btnCancel;
		private XtraEditors.SimpleButton btnOk;
		private XtraEditors.LabelControl lblCalculation;
		private XtraEditors.LookUpEdit edtBaseField;
		private XtraEditors.LookUpEdit edtBaseItem;
		private XtraEditors.LabelControl lblBaseField;
		private XtraEditors.LabelControl lblBaseItem;
	}
}
