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

using DevExpress.XtraEditors;
namespace DevExpress.XtraSpreadsheet.Forms {
	partial class OutlineSettingsForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OutlineSettingsForm));
			this.btnCreate = new DevExpress.XtraEditors.SimpleButton();
			this.btnApply = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.lcDirection = new DevExpress.XtraEditors.LabelControl();
			this.ceRowsBelow = new DevExpress.XtraEditors.CheckEdit();
			this.ceColumnsRight = new DevExpress.XtraEditors.CheckEdit();
			this.ceAutomaticStyles = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)(this.ceRowsBelow.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ceColumnsRight.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ceAutomaticStyles.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.btnCreate, "btnCreate");
			this.btnCreate.CausesValidation = false;
			this.btnCreate.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnCreate.Name = "btnCreate";
			this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
			resources.ApplyResources(this.btnApply, "btnApply");
			this.btnApply.Name = "btnApply";
			this.btnApply.Click += new System.EventHandler(this.btn_Click);
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.CausesValidation = false;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.btn_Click);
			resources.ApplyResources(this.lcDirection, "lcDirection");
			this.lcDirection.LineLocation = DevExpress.XtraEditors.LineLocation.Center;
			this.lcDirection.LineOrientation = DevExpress.XtraEditors.LabelLineOrientation.Horizontal;
			this.lcDirection.LineVisible = true;
			this.lcDirection.Name = "lcDirection";
			resources.ApplyResources(this.ceRowsBelow, "ceRowsBelow");
			this.ceRowsBelow.Name = "ceRowsBelow";
			this.ceRowsBelow.Properties.AutoWidth = true;
			this.ceRowsBelow.Properties.Caption = resources.GetString("ceRowsBelow.Properties.Caption");
			this.ceRowsBelow.CheckedChanged += new System.EventHandler(this.ceRowsBelow_CheckedChanged);
			resources.ApplyResources(this.ceColumnsRight, "ceColumnsRight");
			this.ceColumnsRight.Name = "ceColumnsRight";
			this.ceColumnsRight.Properties.AutoWidth = true;
			this.ceColumnsRight.Properties.Caption = resources.GetString("ceColumnsRight.Properties.Caption");
			this.ceColumnsRight.CheckedChanged += new System.EventHandler(this.ceColumnsRight_CheckedChanged);
			resources.ApplyResources(this.ceAutomaticStyles, "ceAutomaticStyles");
			this.ceAutomaticStyles.Name = "ceAutomaticStyles";
			this.ceAutomaticStyles.Properties.AutoWidth = true;
			this.ceAutomaticStyles.Properties.Caption = resources.GetString("ceAutomaticStyles.Properties.Caption");
			this.ceAutomaticStyles.CheckedChanged += new System.EventHandler(this.ceAutomaticStyles_CheckedChanged);
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.ceAutomaticStyles);
			this.Controls.Add(this.ceColumnsRight);
			this.Controls.Add(this.ceRowsBelow);
			this.Controls.Add(this.lcDirection);
			this.Controls.Add(this.btnCreate);
			this.Controls.Add(this.btnApply);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "OutlineSettingsForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)(this.ceRowsBelow.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ceColumnsRight.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ceAutomaticStyles.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		SimpleButton btnCreate;
		SimpleButton btnApply;
		SimpleButton btnCancel;
		SimpleButton btnOk;
		#endregion
		private LabelControl lcDirection;
		private CheckEdit ceRowsBelow;
		private CheckEdit ceColumnsRight;
		private CheckEdit ceAutomaticStyles;
	}
}
