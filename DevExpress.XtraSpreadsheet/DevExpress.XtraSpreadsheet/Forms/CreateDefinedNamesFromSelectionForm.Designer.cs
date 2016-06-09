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
	partial class CreateDefinedNamesFromSelectionForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateDefinedNamesFromSelectionForm));
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.lblCreateValues = new DevExpress.XtraEditors.LabelControl();
			this.chkTopRow = new DevExpress.XtraEditors.CheckEdit();
			this.chkLeftColumn = new DevExpress.XtraEditors.CheckEdit();
			this.chkBottomRow = new DevExpress.XtraEditors.CheckEdit();
			this.chkRightColumn = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)(this.chkTopRow.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkLeftColumn.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkBottomRow.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkRightColumn.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.CausesValidation = false;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			resources.ApplyResources(this.lblCreateValues, "lblCreateValues");
			this.lblCreateValues.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblCreateValues.LineVisible = true;
			this.lblCreateValues.Name = "lblCreateValues";
			this.chkTopRow.AutoSizeInLayoutControl = true;
			resources.ApplyResources(this.chkTopRow, "chkTopRow");
			this.chkTopRow.Name = "chkTopRow";
			this.chkTopRow.Properties.AccessibleName = resources.GetString("chkTopRow.Properties.AccessibleName");
			this.chkTopRow.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.CheckButton;
			this.chkTopRow.Properties.AutoWidth = true;
			this.chkTopRow.Properties.Caption = resources.GetString("chkTopRow.Properties.Caption");
			this.chkLeftColumn.AutoSizeInLayoutControl = true;
			resources.ApplyResources(this.chkLeftColumn, "chkLeftColumn");
			this.chkLeftColumn.Name = "chkLeftColumn";
			this.chkLeftColumn.Properties.AccessibleName = resources.GetString("chkLeftColumn.Properties.AccessibleName");
			this.chkLeftColumn.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.CheckButton;
			this.chkLeftColumn.Properties.AutoWidth = true;
			this.chkLeftColumn.Properties.Caption = resources.GetString("chkLeftColumn.Properties.Caption");
			this.chkBottomRow.AutoSizeInLayoutControl = true;
			resources.ApplyResources(this.chkBottomRow, "chkBottomRow");
			this.chkBottomRow.Name = "chkBottomRow";
			this.chkBottomRow.Properties.AccessibleName = resources.GetString("chkBottomRow.Properties.AccessibleName");
			this.chkBottomRow.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.CheckButton;
			this.chkBottomRow.Properties.AutoWidth = true;
			this.chkBottomRow.Properties.Caption = resources.GetString("chkBottomRow.Properties.Caption");
			this.chkRightColumn.AutoSizeInLayoutControl = true;
			resources.ApplyResources(this.chkRightColumn, "chkRightColumn");
			this.chkRightColumn.Name = "chkRightColumn";
			this.chkRightColumn.Properties.AccessibleName = resources.GetString("chkRightColumn.Properties.AccessibleName");
			this.chkRightColumn.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.CheckButton;
			this.chkRightColumn.Properties.AutoWidth = true;
			this.chkRightColumn.Properties.Caption = resources.GetString("chkRightColumn.Properties.Caption");
			this.AcceptButton = this.btnOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.chkRightColumn);
			this.Controls.Add(this.chkBottomRow);
			this.Controls.Add(this.chkLeftColumn);
			this.Controls.Add(this.chkTopRow);
			this.Controls.Add(this.lblCreateValues);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.btnCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "CreateDefinedNamesFromSelectionForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)(this.chkTopRow.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkLeftColumn.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkBottomRow.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkRightColumn.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private XtraEditors.SimpleButton btnOk;
		private XtraEditors.SimpleButton btnCancel;
		private XtraEditors.LabelControl lblCreateValues;
		private XtraEditors.CheckEdit chkTopRow;
		private XtraEditors.CheckEdit chkLeftColumn;
		private XtraEditors.CheckEdit chkBottomRow;
		private XtraEditors.CheckEdit chkRightColumn;
	}
}
