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
	partial class GotoDateForm {
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		#region Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GotoDateForm));
			this.btnOK = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.lblDate = new DevExpress.XtraEditors.LabelControl();
			this.lblShowIn = new DevExpress.XtraEditors.LabelControl();
			this.grpGroup = new DevExpress.XtraEditors.GroupControl();
			this.cbShowIn = new DevExpress.XtraEditors.ImageComboBoxEdit();
			this.edtDate = new DevExpress.XtraEditors.DateEdit();
			((System.ComponentModel.ISupportInitialize)(this.grpGroup)).BeginInit();
			this.grpGroup.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbShowIn.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtDate.Properties.VistaTimeProperties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtDate.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.btnOK, "btnOK");
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Name = "btnOK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			resources.ApplyResources(this.lblDate, "lblDate");
			this.lblDate.Name = "lblDate";
			resources.ApplyResources(this.lblShowIn, "lblShowIn");
			this.lblShowIn.Name = "lblShowIn";
			this.grpGroup.AccessibleRole = System.Windows.Forms.AccessibleRole.Window;
			resources.ApplyResources(this.grpGroup, "grpGroup");
			this.grpGroup.Controls.Add(this.cbShowIn);
			this.grpGroup.Controls.Add(this.edtDate);
			this.grpGroup.Controls.Add(this.lblDate);
			this.grpGroup.Controls.Add(this.lblShowIn);
			this.grpGroup.Name = "grpGroup";
			this.grpGroup.ShowCaption = false;
			resources.ApplyResources(this.cbShowIn, "cbShowIn");
			this.cbShowIn.Name = "cbShowIn";
			this.cbShowIn.Properties.AccessibleName = resources.GetString("cbShowIn.Properties.AccessibleName");
			this.cbShowIn.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.ComboBox;
			this.cbShowIn.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbShowIn.Properties.Buttons"))))});
			resources.ApplyResources(this.edtDate, "edtDate");
			this.edtDate.Name = "edtDate";
			this.edtDate.Properties.AccessibleName = resources.GetString("edtDate.Properties.AccessibleName");
			this.edtDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtDate.Properties.Buttons"))))});
			this.edtDate.Properties.MaxValue = new System.DateTime(4000, 1, 1, 0, 0, 0, 0);
			this.edtDate.Properties.VistaTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.AcceptButton = this.btnOK;
			this.AccessibleRole = System.Windows.Forms.AccessibleRole.Window;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.grpGroup);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "GotoDateForm";
			this.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)(this.grpGroup)).EndInit();
			this.grpGroup.ResumeLayout(false);
			this.grpGroup.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbShowIn.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtDate.Properties.VistaTimeProperties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtDate.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		protected DevExpress.XtraEditors.SimpleButton btnOK;
		protected DevExpress.XtraEditors.SimpleButton btnCancel;
		protected DevExpress.XtraEditors.LabelControl lblDate;
		protected DevExpress.XtraEditors.LabelControl lblShowIn;
		protected DevExpress.XtraEditors.GroupControl grpGroup;
		protected DevExpress.XtraEditors.DateEdit edtDate;
		protected DevExpress.XtraEditors.ImageComboBoxEdit cbShowIn;
		private System.ComponentModel.Container components = null;
	}
}
