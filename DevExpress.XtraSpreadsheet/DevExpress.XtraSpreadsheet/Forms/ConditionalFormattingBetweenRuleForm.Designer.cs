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
	partial class ConditionalFormattingBetweenRuleForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConditionalFormattingBetweenRuleForm));
			this.edtLowValue = new DevExpress.XtraSpreadsheet.ReferenceEditControl();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.cbFormat = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblAnd = new DevExpress.XtraEditors.LabelControl();
			this.lblHeader = new DevExpress.XtraEditors.LabelControl();
			this.edtHighValue = new DevExpress.XtraSpreadsheet.ReferenceEditControl();
			this.lblWith = new DevExpress.XtraEditors.LabelControl();
			((System.ComponentModel.ISupportInitialize)(this.edtLowValue.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbFormat.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtHighValue.Properties)).BeginInit();
			this.SuspendLayout();
			this.edtLowValue.Activated = false;
			this.edtLowValue.EditValuePrefix = null;
			this.edtLowValue.IncludeSheetName = false;
			resources.ApplyResources(this.edtLowValue, "edtLowValue");
			this.edtLowValue.Name = "edtLowValue";
			this.edtLowValue.PositionType = DevExpress.XtraSpreadsheet.Model.PositionType.Relative;
			this.edtLowValue.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtLowValue.Properties.Buttons"))))});
			this.edtLowValue.SpreadsheetControl = null;
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			resources.ApplyResources(this.cbFormat, "cbFormat");
			this.cbFormat.Name = "cbFormat";
			this.cbFormat.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbFormat.Properties.Buttons"))))});
			this.cbFormat.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			resources.ApplyResources(this.lblAnd, "lblAnd");
			this.lblAnd.Name = "lblAnd";
			this.lblHeader.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("lblHeader.Appearance.Font")));
			resources.ApplyResources(this.lblHeader, "lblHeader");
			this.lblHeader.Name = "lblHeader";
			this.edtHighValue.Activated = false;
			this.edtHighValue.EditValuePrefix = null;
			this.edtHighValue.IncludeSheetName = false;
			resources.ApplyResources(this.edtHighValue, "edtHighValue");
			this.edtHighValue.Name = "edtHighValue";
			this.edtHighValue.PositionType = DevExpress.XtraSpreadsheet.Model.PositionType.Relative;
			this.edtHighValue.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtHighValue.Properties.Buttons"))))});
			this.edtHighValue.SpreadsheetControl = null;
			resources.ApplyResources(this.lblWith, "lblWith");
			this.lblWith.Name = "lblWith";
			this.AcceptButton = this.btnOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.lblWith);
			this.Controls.Add(this.edtHighValue);
			this.Controls.Add(this.edtLowValue);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.cbFormat);
			this.Controls.Add(this.lblAnd);
			this.Controls.Add(this.lblHeader);
			this.Name = "ConditionalFormattingBetweenRuleForm";
			((System.ComponentModel.ISupportInitialize)(this.edtLowValue.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbFormat.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtHighValue.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private ReferenceEditControl edtLowValue;
		private XtraEditors.SimpleButton btnCancel;
		private XtraEditors.SimpleButton btnOk;
		private XtraEditors.ComboBoxEdit cbFormat;
		private XtraEditors.LabelControl lblAnd;
		private XtraEditors.LabelControl lblHeader;
		private ReferenceEditControl edtHighValue;
		private XtraEditors.LabelControl lblWith;
	}
}
