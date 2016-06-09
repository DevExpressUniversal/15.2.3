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

namespace DevExpress.XtraPrinting.Preview {
	partial class ReportServerCredentialsForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReportServerCredentialsForm));
			this.userNameLabel = new System.Windows.Forms.Label();
			this.passwordLabel = new System.Windows.Forms.Label();
			this.userNameEdit = new DevExpress.XtraEditors.TextEdit();
			this.passwordEdit = new DevExpress.XtraEditors.TextEdit();
			this.cancelButton = new DevExpress.XtraEditors.SimpleButton();
			this.loginButton = new DevExpress.XtraEditors.SimpleButton();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			((System.ComponentModel.ISupportInitialize)(this.userNameEdit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.passwordEdit.Properties)).BeginInit();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.userNameLabel, "userNameLabel");
			this.userNameLabel.Name = "userNameLabel";
			resources.ApplyResources(this.passwordLabel, "passwordLabel");
			this.passwordLabel.Name = "passwordLabel";
			resources.ApplyResources(this.userNameEdit, "userNameEdit");
			this.userNameEdit.Name = "userNameEdit";
			this.userNameEdit.TextChanged += new System.EventHandler(this.OnCredentialsChanged);
			resources.ApplyResources(this.passwordEdit, "passwordEdit");
			this.passwordEdit.Name = "passwordEdit";
			this.passwordEdit.Properties.PasswordChar = '*';
			this.passwordEdit.TextChanged += new System.EventHandler(this.OnCredentialsChanged);
			resources.ApplyResources(this.cancelButton, "cancelButton");
			this.cancelButton.AutoWidthInLayoutControl = true;
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Name = "cancelButton";
			resources.ApplyResources(this.loginButton, "loginButton");
			this.loginButton.AutoWidthInLayoutControl = true;
			this.loginButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.loginButton.Name = "loginButton";
			resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
			this.tableLayoutPanel1.Controls.Add(this.userNameLabel, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.passwordEdit, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this.passwordLabel, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.userNameEdit, 1, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.loginButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ReportServerCredentialsForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)(this.userNameEdit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.passwordEdit.Properties)).EndInit();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);
		}
		#endregion
		private XtraEditors.SimpleButton cancelButton;
		private XtraEditors.SimpleButton loginButton;
		private System.Windows.Forms.Label userNameLabel;
		private System.Windows.Forms.Label passwordLabel;
		private XtraEditors.TextEdit userNameEdit;
		private XtraEditors.TextEdit passwordEdit;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
	}
}
