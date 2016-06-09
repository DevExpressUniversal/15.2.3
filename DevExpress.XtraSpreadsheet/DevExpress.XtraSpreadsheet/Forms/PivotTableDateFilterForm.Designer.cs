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
	partial class PivotTableDateFilterForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PivotTableDateFilterForm));
			DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
			DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
			DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.lblShowItems = new DevExpress.XtraEditors.LabelControl();
			this.lblColumnName = new DevExpress.XtraEditors.LabelControl();
			this.lblAnd = new DevExpress.XtraEditors.LabelControl();
			this.edtFilterOperator = new DevExpress.XtraEditors.LookUpEdit();
			this.dateEdit2 = new DevExpress.XtraEditors.DateEdit();
			this.dateEdit1 = new DevExpress.XtraEditors.DateEdit();
			this.dateEdit = new DevExpress.XtraEditors.DateEdit();
			((System.ComponentModel.ISupportInitialize)(this.edtFilterOperator.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dateEdit2.Properties.CalendarTimeProperties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dateEdit2.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dateEdit1.Properties.CalendarTimeProperties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dateEdit1.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dateEdit.Properties.CalendarTimeProperties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dateEdit.Properties)).BeginInit();
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
			resources.ApplyResources(this.lblAnd, "lblAnd");
			this.lblAnd.Name = "lblAnd";
			resources.ApplyResources(this.edtFilterOperator, "edtFilterOperator");
			this.edtFilterOperator.Name = "edtFilterOperator";
			this.edtFilterOperator.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtFilterOperator.Properties.Buttons"))))});
			this.edtFilterOperator.Properties.DropDownRows = 6;
			this.edtFilterOperator.Properties.NullText = resources.GetString("edtFilterOperator.Properties.NullText");
			this.edtFilterOperator.Properties.ShowFooter = false;
			this.edtFilterOperator.Properties.ShowHeader = false;
			this.dateEdit2.CausesValidation = false;
			resources.ApplyResources(this.dateEdit2, "dateEdit2");
			this.dateEdit2.Name = "dateEdit2";
			this.dateEdit2.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("dateEdit2.Properties.Buttons"))), resources.GetString("dateEdit2.Properties.Buttons1"), ((int)(resources.GetObject("dateEdit2.Properties.Buttons2"))), ((bool)(resources.GetObject("dateEdit2.Properties.Buttons3"))), ((bool)(resources.GetObject("dateEdit2.Properties.Buttons4"))), ((bool)(resources.GetObject("dateEdit2.Properties.Buttons5"))), ((DevExpress.XtraEditors.ImageLocation)(resources.GetObject("dateEdit2.Properties.Buttons6"))), ((System.Drawing.Image)(resources.GetObject("dateEdit2.Properties.Buttons7"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, resources.GetString("dateEdit2.Properties.Buttons8"), ((object)(resources.GetObject("dateEdit2.Properties.Buttons9"))), ((DevExpress.Utils.SuperToolTip)(resources.GetObject("dateEdit2.Properties.Buttons10"))), ((bool)(resources.GetObject("dateEdit2.Properties.Buttons11"))))});
			this.dateEdit2.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("dateEdit2.Properties.CalendarTimeProperties.Buttons"))))});
			this.dateEdit2.Properties.Mask.MaskType = ((DevExpress.XtraEditors.Mask.MaskType)(resources.GetObject("dateEdit2.Properties.Mask.MaskType")));
			this.dateEdit2.EditValueChanged += new System.EventHandler(this.dateEdit2_EditValueChanged);
			this.dateEdit1.CausesValidation = false;
			resources.ApplyResources(this.dateEdit1, "dateEdit1");
			this.dateEdit1.Name = "dateEdit1";
			this.dateEdit1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("dateEdit1.Properties.Buttons"))), resources.GetString("dateEdit1.Properties.Buttons1"), ((int)(resources.GetObject("dateEdit1.Properties.Buttons2"))), ((bool)(resources.GetObject("dateEdit1.Properties.Buttons3"))), ((bool)(resources.GetObject("dateEdit1.Properties.Buttons4"))), ((bool)(resources.GetObject("dateEdit1.Properties.Buttons5"))), ((DevExpress.XtraEditors.ImageLocation)(resources.GetObject("dateEdit1.Properties.Buttons6"))), ((System.Drawing.Image)(resources.GetObject("dateEdit1.Properties.Buttons7"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject2, resources.GetString("dateEdit1.Properties.Buttons8"), ((object)(resources.GetObject("dateEdit1.Properties.Buttons9"))), ((DevExpress.Utils.SuperToolTip)(resources.GetObject("dateEdit1.Properties.Buttons10"))), ((bool)(resources.GetObject("dateEdit1.Properties.Buttons11"))))});
			this.dateEdit1.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("dateEdit1.Properties.CalendarTimeProperties.Buttons"))))});
			this.dateEdit1.Properties.Mask.MaskType = ((DevExpress.XtraEditors.Mask.MaskType)(resources.GetObject("dateEdit1.Properties.Mask.MaskType")));
			this.dateEdit1.EditValueChanged += new System.EventHandler(this.dateEdit1_EditValueChanged);
			this.dateEdit.CausesValidation = false;
			resources.ApplyResources(this.dateEdit, "dateEdit");
			this.dateEdit.Name = "dateEdit";
			this.dateEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("dateEdit.Properties.Buttons"))), resources.GetString("dateEdit.Properties.Buttons1"), ((int)(resources.GetObject("dateEdit.Properties.Buttons2"))), ((bool)(resources.GetObject("dateEdit.Properties.Buttons3"))), ((bool)(resources.GetObject("dateEdit.Properties.Buttons4"))), ((bool)(resources.GetObject("dateEdit.Properties.Buttons5"))), ((DevExpress.XtraEditors.ImageLocation)(resources.GetObject("dateEdit.Properties.Buttons6"))), ((System.Drawing.Image)(resources.GetObject("dateEdit.Properties.Buttons7"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject3, resources.GetString("dateEdit.Properties.Buttons8"), ((object)(resources.GetObject("dateEdit.Properties.Buttons9"))), ((DevExpress.Utils.SuperToolTip)(resources.GetObject("dateEdit.Properties.Buttons10"))), ((bool)(resources.GetObject("dateEdit.Properties.Buttons11"))))});
			this.dateEdit.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("dateEdit.Properties.CalendarTimeProperties.Buttons"))))});
			this.dateEdit.Properties.Mask.MaskType = ((DevExpress.XtraEditors.Mask.MaskType)(resources.GetObject("dateEdit.Properties.Mask.MaskType")));
			this.dateEdit.EditValueChanged += new System.EventHandler(this.dateEdit_EditValueChanged);
			this.AcceptButton = this.btnOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.dateEdit);
			this.Controls.Add(this.dateEdit1);
			this.Controls.Add(this.dateEdit2);
			this.Controls.Add(this.edtFilterOperator);
			this.Controls.Add(this.lblAnd);
			this.Controls.Add(this.lblColumnName);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.lblShowItems);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PivotTableDateFilterForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			((System.ComponentModel.ISupportInitialize)(this.edtFilterOperator.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dateEdit2.Properties.CalendarTimeProperties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dateEdit2.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dateEdit1.Properties.CalendarTimeProperties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dateEdit1.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dateEdit.Properties.CalendarTimeProperties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dateEdit.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private XtraEditors.SimpleButton btnCancel;
		private XtraEditors.SimpleButton btnOk;
		private XtraEditors.LabelControl lblShowItems;
		private XtraEditors.LabelControl lblColumnName;
		private XtraEditors.LabelControl lblAnd;
		private XtraEditors.LookUpEdit edtFilterOperator;
		private XtraEditors.DateEdit dateEdit2;
		private XtraEditors.DateEdit dateEdit1;
		private XtraEditors.DateEdit dateEdit;
	}
}
