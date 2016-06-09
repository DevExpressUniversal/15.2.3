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
	partial class GenericFilterForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GenericFilterForm));
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.cbFilterOperator = new DevExpress.XtraEditors.LookUpEdit();
			this.lblShowRows = new DevExpress.XtraEditors.LabelControl();
			this.lblColumnName = new DevExpress.XtraEditors.LabelControl();
			this.cbFilterValue = new DevExpress.XtraEditors.ComboBoxEdit();
			this.rgrpAndOr = new DevExpress.XtraEditors.RadioGroup();
			this.cbSecondaryFilterValue = new DevExpress.XtraEditors.ComboBoxEdit();
			this.cbSecondaryFilterOperator = new DevExpress.XtraEditors.LookUpEdit();
			this.lblQuestionSignDescription = new DevExpress.XtraEditors.LabelControl();
			this.lblStarSignDescription = new DevExpress.XtraEditors.LabelControl();
			this.edtValueDatePicker = new DevExpress.XtraEditors.DateEdit();
			this.edtSecondaryValueDatePicker = new DevExpress.XtraEditors.DateEdit();
			((System.ComponentModel.ISupportInitialize)(this.cbFilterOperator.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbFilterValue.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.rgrpAndOr.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbSecondaryFilterValue.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbSecondaryFilterOperator.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtValueDatePicker.Properties.CalendarTimeProperties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtValueDatePicker.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtSecondaryValueDatePicker.Properties.CalendarTimeProperties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtSecondaryValueDatePicker.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			resources.ApplyResources(this.cbFilterOperator, "cbFilterOperator");
			this.cbFilterOperator.Name = "cbFilterOperator";
			this.cbFilterOperator.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbFilterOperator.Properties.Buttons"))))});
			this.cbFilterOperator.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("cbFilterOperator.Properties.Columns"), resources.GetString("cbFilterOperator.Properties.Columns1"))});
			this.cbFilterOperator.Properties.ShowFooter = false;
			this.cbFilterOperator.Properties.ShowHeader = false;
			resources.ApplyResources(this.lblShowRows, "lblShowRows");
			this.lblShowRows.Name = "lblShowRows";
			resources.ApplyResources(this.lblColumnName, "lblColumnName");
			this.lblColumnName.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblColumnName.LineVisible = true;
			this.lblColumnName.Name = "lblColumnName";
			resources.ApplyResources(this.cbFilterValue, "cbFilterValue");
			this.cbFilterValue.Name = "cbFilterValue";
			this.cbFilterValue.Properties.AutoComplete = false;
			this.cbFilterValue.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbFilterValue.Properties.Buttons"))))});
			resources.ApplyResources(this.rgrpAndOr, "rgrpAndOr");
			this.rgrpAndOr.Name = "rgrpAndOr";
			this.rgrpAndOr.Properties.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("rgrpAndOr.Properties.Appearance.BackColor")));
			this.rgrpAndOr.Properties.Appearance.Options.UseBackColor = true;
			this.rgrpAndOr.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.rgrpAndOr.Properties.Columns = 2;
			this.rgrpAndOr.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
			new DevExpress.XtraEditors.Controls.RadioGroupItem(((object)(resources.GetObject("rgrpAndOr.Properties.Items"))), resources.GetString("rgrpAndOr.Properties.Items1")),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(((object)(resources.GetObject("rgrpAndOr.Properties.Items2"))), resources.GetString("rgrpAndOr.Properties.Items3"))});
			resources.ApplyResources(this.cbSecondaryFilterValue, "cbSecondaryFilterValue");
			this.cbSecondaryFilterValue.Name = "cbSecondaryFilterValue";
			this.cbSecondaryFilterValue.Properties.AutoComplete = false;
			this.cbSecondaryFilterValue.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbSecondaryFilterValue.Properties.Buttons"))))});
			resources.ApplyResources(this.cbSecondaryFilterOperator, "cbSecondaryFilterOperator");
			this.cbSecondaryFilterOperator.Name = "cbSecondaryFilterOperator";
			this.cbSecondaryFilterOperator.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbSecondaryFilterOperator.Properties.Buttons"))))});
			this.cbSecondaryFilterOperator.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("cbSecondaryFilterOperator.Properties.Columns"), resources.GetString("cbSecondaryFilterOperator.Properties.Columns1"))});
			this.cbSecondaryFilterOperator.Properties.ShowFooter = false;
			this.cbSecondaryFilterOperator.Properties.ShowHeader = false;
			resources.ApplyResources(this.lblQuestionSignDescription, "lblQuestionSignDescription");
			this.lblQuestionSignDescription.Name = "lblQuestionSignDescription";
			resources.ApplyResources(this.lblStarSignDescription, "lblStarSignDescription");
			this.lblStarSignDescription.Name = "lblStarSignDescription";
			resources.ApplyResources(this.edtValueDatePicker, "edtValueDatePicker");
			this.edtValueDatePicker.Name = "edtValueDatePicker";
			this.edtValueDatePicker.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtValueDatePicker.Properties.Buttons"))))});
			this.edtValueDatePicker.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtValueDatePicker.Properties.CalendarTimeProperties.Buttons"))))});
			this.edtValueDatePicker.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
			this.edtValueDatePicker.EditValueChanged += new System.EventHandler(this.edtValueDatePicker_EditValueChanged);
			resources.ApplyResources(this.edtSecondaryValueDatePicker, "edtSecondaryValueDatePicker");
			this.edtSecondaryValueDatePicker.Name = "edtSecondaryValueDatePicker";
			this.edtSecondaryValueDatePicker.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtSecondaryValueDatePicker.Properties.Buttons"))))});
			this.edtSecondaryValueDatePicker.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtSecondaryValueDatePicker.Properties.CalendarTimeProperties.Buttons"))))});
			this.edtSecondaryValueDatePicker.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
			this.edtSecondaryValueDatePicker.EditValueChanged += new System.EventHandler(this.edtSecondaryValueDatePicker_EditValueChanged);
			this.AcceptButton = this.btnOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.edtSecondaryValueDatePicker);
			this.Controls.Add(this.edtValueDatePicker);
			this.Controls.Add(this.lblStarSignDescription);
			this.Controls.Add(this.lblQuestionSignDescription);
			this.Controls.Add(this.cbSecondaryFilterValue);
			this.Controls.Add(this.cbSecondaryFilterOperator);
			this.Controls.Add(this.rgrpAndOr);
			this.Controls.Add(this.cbFilterValue);
			this.Controls.Add(this.lblColumnName);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.cbFilterOperator);
			this.Controls.Add(this.lblShowRows);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "GenericFilterForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)(this.cbFilterOperator.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbFilterValue.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.rgrpAndOr.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbSecondaryFilterValue.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbSecondaryFilterOperator.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtValueDatePicker.Properties.CalendarTimeProperties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtValueDatePicker.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtSecondaryValueDatePicker.Properties.CalendarTimeProperties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtSecondaryValueDatePicker.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private XtraEditors.SimpleButton btnCancel;
		private XtraEditors.SimpleButton btnOk;
		private XtraEditors.LookUpEdit cbFilterOperator;
		private XtraEditors.LabelControl lblShowRows;
		private XtraEditors.LabelControl lblColumnName;
		private XtraEditors.ComboBoxEdit cbFilterValue;
		private XtraEditors.RadioGroup rgrpAndOr;
		private XtraEditors.ComboBoxEdit cbSecondaryFilterValue;
		private XtraEditors.LookUpEdit cbSecondaryFilterOperator;
		private XtraEditors.LabelControl lblQuestionSignDescription;
		private XtraEditors.LabelControl lblStarSignDescription;
		private XtraEditors.DateEdit edtValueDatePicker;
		private XtraEditors.DateEdit edtSecondaryValueDatePicker;
	}
}
