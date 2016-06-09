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
	partial class DefineNameForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DefineNameForm));
			this.edtReference = new DevExpress.XtraSpreadsheet.ReferenceEditControl();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.lblName = new DevExpress.XtraEditors.LabelControl();
			this.lblScope = new DevExpress.XtraEditors.LabelControl();
			this.lblComment = new DevExpress.XtraEditors.LabelControl();
			this.lblReference = new DevExpress.XtraEditors.LabelControl();
			this.edtName = new DevExpress.XtraEditors.TextEdit();
			this.edtScope = new DevExpress.XtraEditors.LookUpEdit();
			this.edtComment = new DevExpress.XtraEditors.MemoEdit();
			((System.ComponentModel.ISupportInitialize)(this.edtReference.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtName.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtScope.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtComment.Properties)).BeginInit();
			this.SuspendLayout();
			this.edtReference.Activated = false;
			resources.ApplyResources(this.edtReference, "edtReference");
			this.edtReference.EditValuePrefix = null;
			this.edtReference.Name = "edtReference";
			this.edtReference.PositionType = DevExpress.XtraSpreadsheet.Model.PositionType.Relative;
			this.edtReference.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.edtReference.SpreadsheetControl = null;
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.CausesValidation = false;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			resources.ApplyResources(this.lblName, "lblName");
			this.lblName.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblName.Name = "lblName";
			resources.ApplyResources(this.lblScope, "lblScope");
			this.lblScope.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblScope.Name = "lblScope";
			resources.ApplyResources(this.lblComment, "lblComment");
			this.lblComment.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblComment.Name = "lblComment";
			resources.ApplyResources(this.lblReference, "lblReference");
			this.lblReference.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblReference.Name = "lblReference";
			resources.ApplyResources(this.edtName, "edtName");
			this.edtName.Name = "edtName";
			this.edtName.Properties.MaxLength = 31;
			resources.ApplyResources(this.edtScope, "edtScope");
			this.edtScope.Name = "edtScope";
			this.edtScope.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtScope.Properties.Buttons"))))});
			this.edtScope.Properties.ShowFooter = false;
			this.edtScope.Properties.ShowHeader = false;
			resources.ApplyResources(this.edtComment, "edtComment");
			this.edtComment.Name = "edtComment";
			this.edtComment.UseOptimizedRendering = true;
			this.AcceptButton = this.btnOk;
			this.AccessibleRole = System.Windows.Forms.AccessibleRole.Window;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.edtComment);
			this.Controls.Add(this.edtScope);
			this.Controls.Add(this.edtName);
			this.Controls.Add(this.lblReference);
			this.Controls.Add(this.lblComment);
			this.Controls.Add(this.lblScope);
			this.Controls.Add(this.edtReference);
			this.Controls.Add(this.lblName);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.btnCancel);
			this.Name = "DefineNameForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			((System.ComponentModel.ISupportInitialize)(this.edtReference.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtName.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtScope.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtComment.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private XtraEditors.SimpleButton btnCancel;
		private XtraEditors.SimpleButton btnOk;
		private XtraEditors.LabelControl lblName;
		private ReferenceEditControl edtReference;
		private XtraEditors.LabelControl lblScope;
		private XtraEditors.LabelControl lblComment;
		private XtraEditors.LabelControl lblReference;
		private XtraEditors.TextEdit edtName;
		private XtraEditors.LookUpEdit edtScope;
		private XtraEditors.MemoEdit edtComment;
	}
}
