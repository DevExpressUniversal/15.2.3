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

namespace DevExpress.XtraScheduler.UI {
	partial class AppointmentDependencyForm {
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AppointmentDependencyForm));
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.lblFrom = new DevExpress.XtraEditors.LabelControl();
			this.lblTask1Description = new DevExpress.XtraEditors.LabelControl();
			this.lblTo = new DevExpress.XtraEditors.LabelControl();
			this.lblTask2Description = new DevExpress.XtraEditors.LabelControl();
			this.lblType = new DevExpress.XtraEditors.LabelControl();
			this.cbTypeEdit = new DevExpress.XtraScheduler.UI.AppointmentDependencyTypeEdit();
			this.btnDelete = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.cbTypeEdit.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			resources.ApplyResources(this.lblFrom, "lblFrom");
			this.lblFrom.Name = "lblFrom";
			resources.ApplyResources(this.lblTask1Description, "lblTask1Description");
			this.lblTask1Description.Name = "lblTask1Description";
			resources.ApplyResources(this.lblTo, "lblTo");
			this.lblTo.Name = "lblTo";
			resources.ApplyResources(this.lblTask2Description, "lblTask2Description");
			this.lblTask2Description.Name = "lblTask2Description";
			resources.ApplyResources(this.lblType, "lblType");
			this.lblType.Name = "lblType";
			resources.ApplyResources(this.cbTypeEdit, "cbTypeEdit");
			this.cbTypeEdit.Name = "cbTypeEdit";
			this.cbTypeEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbTypeEdit.Properties.Buttons"))))});
			resources.ApplyResources(this.btnDelete, "btnDelete");
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
			this.AcceptButton = this.btnOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.btnDelete);
			this.Controls.Add(this.cbTypeEdit);
			this.Controls.Add(this.lblType);
			this.Controls.Add(this.lblTask1Description);
			this.Controls.Add(this.lblTask2Description);
			this.Controls.Add(this.lblTo);
			this.Controls.Add(this.lblFrom);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AppointmentDependencyForm";
			this.ShowInTaskbar = false;
			this.Load += new System.EventHandler(this.AppointmentDependencyForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.cbTypeEdit.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected XtraEditors.SimpleButton btnOk;
		protected XtraEditors.SimpleButton btnCancel;
		protected XtraEditors.LabelControl lblFrom;
		protected XtraEditors.LabelControl lblTask1Description;
		protected XtraEditors.LabelControl lblTo;
		protected XtraEditors.LabelControl lblTask2Description;
		protected XtraEditors.LabelControl lblType;
		protected DevExpress.XtraScheduler.UI.AppointmentDependencyTypeEdit cbTypeEdit;
		protected DevExpress.XtraEditors.SimpleButton btnDelete;
		private System.ComponentModel.IContainer components = null;
	}
}
