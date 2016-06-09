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
	partial class PivotTableValueFilterForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PivotTableValueFilterForm));
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.lblShowItems = new DevExpress.XtraEditors.LabelControl();
			this.lblColumnName = new DevExpress.XtraEditors.LabelControl();
			this.edtFilterValue = new DevExpress.XtraEditors.TextEdit();
			this.edtFilterValue1 = new DevExpress.XtraEditors.TextEdit();
			this.edtFilterValue2 = new DevExpress.XtraEditors.TextEdit();
			this.lblAnd = new DevExpress.XtraEditors.LabelControl();
			this.edtDataFields = new DevExpress.XtraEditors.LookUpEdit();
			this.edtFilterOperator = new DevExpress.XtraEditors.LookUpEdit();
			((System.ComponentModel.ISupportInitialize)(this.edtFilterValue.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtFilterValue1.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtFilterValue2.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtDataFields.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtFilterOperator.Properties)).BeginInit();
			this.SuspendLayout();
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.Name = "btnCancel";
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			resources.ApplyResources(this.lblShowItems, "lblShowItems");
			this.lblShowItems.Name = "lblShowItems";
			resources.ApplyResources(this.lblColumnName, "lblColumnName");
			this.lblColumnName.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblColumnName.LineVisible = true;
			this.lblColumnName.Name = "lblColumnName";
			resources.ApplyResources(this.edtFilterValue, "edtFilterValue");
			this.edtFilterValue.Name = "edtFilterValue";
			this.edtFilterValue.Properties.MaxLength = 255;
			resources.ApplyResources(this.edtFilterValue1, "edtFilterValue1");
			this.edtFilterValue1.Name = "edtFilterValue1";
			this.edtFilterValue1.Properties.MaxLength = 255;
			resources.ApplyResources(this.edtFilterValue2, "edtFilterValue2");
			this.edtFilterValue2.Name = "edtFilterValue2";
			this.edtFilterValue2.Properties.MaxLength = 255;
			resources.ApplyResources(this.lblAnd, "lblAnd");
			this.lblAnd.Name = "lblAnd";
			resources.ApplyResources(this.edtDataFields, "edtDataFields");
			this.edtDataFields.Name = "edtDataFields";
			this.edtDataFields.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtDataFields.Properties.Buttons"))))});
			this.edtDataFields.Properties.DropDownRows = 6;
			this.edtDataFields.Properties.NullText = resources.GetString("edtDataFields.Properties.NullText");
			this.edtDataFields.Properties.ShowFooter = false;
			this.edtDataFields.Properties.ShowHeader = false;
			resources.ApplyResources(this.edtFilterOperator, "edtFilterOperator");
			this.edtFilterOperator.Name = "edtFilterOperator";
			this.edtFilterOperator.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtFilterOperator.Properties.Buttons"))))});
			this.edtFilterOperator.Properties.DropDownRows = 6;
			this.edtFilterOperator.Properties.NullText = resources.GetString("edtFilterOperator.Properties.NullText");
			this.edtFilterOperator.Properties.ShowFooter = false;
			this.edtFilterOperator.Properties.ShowHeader = false;
			this.AcceptButton = this.btnOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.edtFilterOperator);
			this.Controls.Add(this.edtDataFields);
			this.Controls.Add(this.lblAnd);
			this.Controls.Add(this.edtFilterValue2);
			this.Controls.Add(this.edtFilterValue1);
			this.Controls.Add(this.edtFilterValue);
			this.Controls.Add(this.lblColumnName);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.lblShowItems);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PivotTableValueFilterForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			((System.ComponentModel.ISupportInitialize)(this.edtFilterValue.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtFilterValue1.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtFilterValue2.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtDataFields.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtFilterOperator.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private XtraEditors.SimpleButton btnCancel;
		private XtraEditors.SimpleButton btnOk;
		private XtraEditors.LabelControl lblShowItems;
		private XtraEditors.LabelControl lblColumnName;
		private XtraEditors.TextEdit edtFilterValue;
		private XtraEditors.TextEdit edtFilterValue1;
		private XtraEditors.TextEdit edtFilterValue2;
		private XtraEditors.LabelControl lblAnd;
		private XtraEditors.LookUpEdit edtDataFields;
		private XtraEditors.LookUpEdit edtFilterOperator;
	}
}
