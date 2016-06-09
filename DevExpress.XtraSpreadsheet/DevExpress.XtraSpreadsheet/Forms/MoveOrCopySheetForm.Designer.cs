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
	partial class MoveOrCopySheetForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MoveOrCopySheetForm));
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.lblMoveSheetsBeforeSheet = new DevExpress.XtraEditors.LabelControl();
			this.lbBeforeSheet = new DevExpress.XtraEditors.ListBoxControl();
			this.chkCreateCopy = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)(this.lbBeforeSheet)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkCreateCopy.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Name = "btnOk";
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.CausesValidation = false;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			resources.ApplyResources(this.lblMoveSheetsBeforeSheet, "lblMoveSheetsBeforeSheet");
			this.lblMoveSheetsBeforeSheet.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblMoveSheetsBeforeSheet.Name = "lblMoveSheetsBeforeSheet";
			resources.ApplyResources(this.lbBeforeSheet, "lbBeforeSheet");
			this.lbBeforeSheet.AccessibleRole = System.Windows.Forms.AccessibleRole.List;
			this.lbBeforeSheet.Name = "lbBeforeSheet";
			this.lbBeforeSheet.DoubleClick += new System.EventHandler(this.lbBeforeSheet_DoubleClick);
			resources.ApplyResources(this.chkCreateCopy, "chkCreateCopy");
			this.chkCreateCopy.Name = "chkCreateCopy";
			this.chkCreateCopy.Properties.AutoWidth = true;
			this.chkCreateCopy.Properties.Caption = resources.GetString("chkCreateCopy.Properties.Caption");
			this.AcceptButton = this.btnOk;
			this.AccessibleRole = System.Windows.Forms.AccessibleRole.Window;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.chkCreateCopy);
			this.Controls.Add(this.lbBeforeSheet);
			this.Controls.Add(this.lblMoveSheetsBeforeSheet);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.btnCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MoveOrCopySheetForm";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			((System.ComponentModel.ISupportInitialize)(this.lbBeforeSheet)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkCreateCopy.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private XtraEditors.SimpleButton btnOk;
		private XtraEditors.SimpleButton btnCancel;
		private XtraEditors.LabelControl lblMoveSheetsBeforeSheet;
		private XtraEditors.ListBoxControl lbBeforeSheet;
		private XtraEditors.CheckEdit chkCreateCopy;
	}
}
