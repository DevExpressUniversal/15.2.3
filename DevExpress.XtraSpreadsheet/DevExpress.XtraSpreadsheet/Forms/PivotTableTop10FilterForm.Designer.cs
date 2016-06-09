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
	partial class PivotTableTop10FilterForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PivotTableTop10FilterForm));
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.lblShow = new DevExpress.XtraEditors.LabelControl();
			this.lblColumnName = new DevExpress.XtraEditors.LabelControl();
			this.edtTopBottom = new DevExpress.XtraEditors.LookUpEdit();
			this.edtFilterOperator = new DevExpress.XtraEditors.LookUpEdit();
			this.lblBy = new DevExpress.XtraEditors.LabelControl();
			this.edtDataFields = new DevExpress.XtraEditors.LookUpEdit();
			this.edtValue = new DevExpress.XtraEditors.SpinEdit();
			((System.ComponentModel.ISupportInitialize)(this.edtTopBottom.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtFilterOperator.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtDataFields.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtValue.Properties)).BeginInit();
			this.SuspendLayout();
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.Name = "btnCancel";
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			resources.ApplyResources(this.lblShow, "lblShow");
			this.lblShow.Name = "lblShow";
			resources.ApplyResources(this.lblColumnName, "lblColumnName");
			this.lblColumnName.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblColumnName.LineVisible = true;
			this.lblColumnName.Name = "lblColumnName";
			resources.ApplyResources(this.edtTopBottom, "edtTopBottom");
			this.edtTopBottom.Name = "edtTopBottom";
			this.edtTopBottom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtTopBottom.Properties.Buttons"))))});
			this.edtTopBottom.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("edtTopBottom.Properties.Columns"), resources.GetString("edtTopBottom.Properties.Columns1"))});
			this.edtTopBottom.Properties.DropDownRows = 6;
			this.edtTopBottom.Properties.NullText = resources.GetString("edtTopBottom.Properties.NullText");
			this.edtTopBottom.Properties.ShowFooter = false;
			this.edtTopBottom.Properties.ShowHeader = false;
			resources.ApplyResources(this.edtFilterOperator, "edtFilterOperator");
			this.edtFilterOperator.Name = "edtFilterOperator";
			this.edtFilterOperator.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtFilterOperator.Properties.Buttons"))))});
			this.edtFilterOperator.Properties.DropDownRows = 6;
			this.edtFilterOperator.Properties.NullText = resources.GetString("edtFilterOperator.Properties.NullText");
			this.edtFilterOperator.Properties.ShowFooter = false;
			this.edtFilterOperator.Properties.ShowHeader = false;
			resources.ApplyResources(this.lblBy, "lblBy");
			this.lblBy.Name = "lblBy";
			resources.ApplyResources(this.edtDataFields, "edtDataFields");
			this.edtDataFields.Name = "edtDataFields";
			this.edtDataFields.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtDataFields.Properties.Buttons"))))});
			this.edtDataFields.Properties.DropDownRows = 6;
			this.edtDataFields.Properties.NullText = resources.GetString("edtDataFields.Properties.NullText");
			this.edtDataFields.Properties.ShowFooter = false;
			this.edtDataFields.Properties.ShowHeader = false;
			resources.ApplyResources(this.edtValue, "edtValue");
			this.edtValue.Name = "edtValue";
			this.edtValue.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.edtValue.Properties.Mask.EditMask = resources.GetString("edtValue.Properties.Mask.EditMask");
			this.edtValue.Properties.MaxLength = 27;
			this.AcceptButton = this.btnOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.edtValue);
			this.Controls.Add(this.edtDataFields);
			this.Controls.Add(this.edtFilterOperator);
			this.Controls.Add(this.edtTopBottom);
			this.Controls.Add(this.lblBy);
			this.Controls.Add(this.lblColumnName);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.lblShow);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PivotTableTop10FilterForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			((System.ComponentModel.ISupportInitialize)(this.edtTopBottom.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtFilterOperator.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtDataFields.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtValue.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private XtraEditors.SimpleButton btnCancel;
		private XtraEditors.SimpleButton btnOk;
		private XtraEditors.LabelControl lblShow;
		private XtraEditors.LabelControl lblColumnName;
		private XtraEditors.LookUpEdit edtTopBottom;
		private XtraEditors.LookUpEdit edtFilterOperator;
		private XtraEditors.LabelControl lblBy;
		private XtraEditors.LookUpEdit edtDataFields;
		private XtraEditors.SpinEdit edtValue;
	}
}
