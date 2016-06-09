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
	partial class Top10FilterForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Top10FilterForm));
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.cbFilterOrder = new DevExpress.XtraEditors.LookUpEdit();
			this.lblShow = new DevExpress.XtraEditors.LabelControl();
			this.cbFilterType = new DevExpress.XtraEditors.LookUpEdit();
			this.edtValue = new DevExpress.XtraEditors.SpinEdit();
			((System.ComponentModel.ISupportInitialize)(this.cbFilterOrder.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbFilterType.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtValue.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			resources.ApplyResources(this.cbFilterOrder, "cbFilterOrder");
			this.cbFilterOrder.Name = "cbFilterOrder";
			this.cbFilterOrder.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbFilterOrder.Properties.Buttons"))))});
			this.cbFilterOrder.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("cbFilterOrder.Properties.Columns"), resources.GetString("cbFilterOrder.Properties.Columns1"))});
			this.cbFilterOrder.Properties.ShowFooter = false;
			this.cbFilterOrder.Properties.ShowHeader = false;
			resources.ApplyResources(this.lblShow, "lblShow");
			this.lblShow.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblShow.LineVisible = true;
			this.lblShow.Name = "lblShow";
			resources.ApplyResources(this.cbFilterType, "cbFilterType");
			this.cbFilterType.Name = "cbFilterType";
			this.cbFilterType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbFilterType.Properties.Buttons"))))});
			this.cbFilterType.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("cbFilterType.Properties.Columns"), resources.GetString("cbFilterType.Properties.Columns1"))});
			this.cbFilterType.Properties.ShowFooter = false;
			this.cbFilterType.Properties.ShowHeader = false;
			resources.ApplyResources(this.edtValue, "edtValue");
			this.edtValue.Name = "edtValue";
			this.edtValue.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.edtValue.Properties.Mask.EditMask = resources.GetString("edtValue.Properties.Mask.EditMask");
			this.edtValue.Properties.MaxValue = new decimal(new int[] {
			500,
			0,
			0,
			0});
			this.edtValue.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.AcceptButton = this.btnOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.edtValue);
			this.Controls.Add(this.cbFilterType);
			this.Controls.Add(this.lblShow);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.cbFilterOrder);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Top10FilterForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)(this.cbFilterOrder.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbFilterType.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtValue.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private XtraEditors.SimpleButton btnCancel;
		private XtraEditors.SimpleButton btnOk;
		private XtraEditors.LookUpEdit cbFilterOrder;
		private XtraEditors.LabelControl lblShow;
		private XtraEditors.LookUpEdit cbFilterType;
		private XtraEditors.SpinEdit edtValue;
	}
}
