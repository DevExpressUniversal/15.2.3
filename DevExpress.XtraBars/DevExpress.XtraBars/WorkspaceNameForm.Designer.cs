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

namespace DevExpress.XtraBars {
	partial class WorkspaceNameForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WorkspaceNameForm));
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.lblWorkspaceName = new DevExpress.XtraEditors.LabelControl();
			this.edtWorkspaceName = new DevExpress.XtraEditors.TextEdit();
			this.stackPanel1 = new DevExpress.XtraEditors.Internal.StackPanelControl();
			this.stackPanel2 = new DevExpress.XtraEditors.Internal.StackPanelControl();
			((System.ComponentModel.ISupportInitialize)(this.edtWorkspaceName.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.stackPanel1)).BeginInit();
			this.stackPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.stackPanel2)).BeginInit();
			this.stackPanel2.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.CausesValidation = false;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.OnOkButtonClick);
			resources.ApplyResources(this.lblWorkspaceName, "lblWorkspaceName");
			this.lblWorkspaceName.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblWorkspaceName.Name = "lblWorkspaceName";
			resources.ApplyResources(this.edtWorkspaceName, "edtWorkspaceName");
			this.edtWorkspaceName.Name = "edtWorkspaceName";
			this.edtWorkspaceName.Properties.MaxLength = 31;
			this.stackPanel1.ContentOrientation = System.Windows.Forms.Orientation.Horizontal;
			this.stackPanel1.Controls.Add(this.lblWorkspaceName);
			this.stackPanel1.Controls.Add(this.edtWorkspaceName);
			resources.ApplyResources(this.stackPanel1, "stackPanel1");
			this.stackPanel1.ItemIndent = 5;
			this.stackPanel1.Name = "stackPanel1";
			this.stackPanel2.ContentAlignment = DevExpress.Utils.HorzAlignment.Far;
			this.stackPanel2.ContentOrientation = System.Windows.Forms.Orientation.Horizontal;
			this.stackPanel2.Controls.Add(this.btnOk);
			this.stackPanel2.Controls.Add(this.btnCancel);
			resources.ApplyResources(this.stackPanel2, "stackPanel2");
			this.stackPanel2.ItemIndent = 5;
			this.stackPanel2.Name = "stackPanel2";
			this.AcceptButton = this.btnOk;
			this.AccessibleRole = System.Windows.Forms.AccessibleRole.Window;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.stackPanel2);
			this.Controls.Add(this.stackPanel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "WorkspaceNameForm";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			((System.ComponentModel.ISupportInitialize)(this.edtWorkspaceName.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.stackPanel1)).EndInit();
			this.stackPanel1.ResumeLayout(false);
			this.stackPanel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.stackPanel2)).EndInit();
			this.stackPanel2.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		private XtraEditors.SimpleButton btnCancel;
		private XtraEditors.SimpleButton btnOk;
		private XtraEditors.LabelControl lblWorkspaceName;
		private XtraEditors.TextEdit edtWorkspaceName;
		private XtraEditors.Internal.StackPanelControl stackPanel1;
		private XtraEditors.Internal.StackPanelControl stackPanel2;
	}
}
