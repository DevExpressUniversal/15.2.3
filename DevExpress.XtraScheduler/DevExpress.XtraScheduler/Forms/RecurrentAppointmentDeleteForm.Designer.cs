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

namespace DevExpress.XtraScheduler.Forms {
	partial class RecurrentAppointmentDeleteForm {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RecurrentAppointmentDeleteForm));
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.lbConfirmText = new DevExpress.XtraEditors.LabelControl();
			this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
			this.chkDeleteOccurrence = new DevExpress.XtraEditors.CheckEdit();
			this.chkDeleteSeries = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkDeleteOccurrence.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkDeleteSeries.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Name = "btnOk";
			resources.ApplyResources(this.lbConfirmText, "lbConfirmText");
			this.lbConfirmText.Appearance.Options.UseTextOptions = true;
			this.lbConfirmText.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
			this.lbConfirmText.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			this.lbConfirmText.Name = "lbConfirmText";
			this.panelControl1.AccessibleRole = System.Windows.Forms.AccessibleRole.Client;
			this.panelControl1.Appearance.BackColor = System.Drawing.Color.Transparent;
			this.panelControl1.Appearance.Options.UseBackColor = true;
			this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelControl1, "panelControl1");
			this.panelControl1.Name = "panelControl1";
			resources.ApplyResources(this.chkDeleteOccurrence, "chkDeleteOccurrence");
			this.chkDeleteOccurrence.Name = "chkDeleteOccurrence";
			this.chkDeleteOccurrence.Properties.AccessibleName = resources.GetString("chkDeleteOccurrence.Properties.AccessibleName");
			this.chkDeleteOccurrence.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.RadioButton;
			this.chkDeleteOccurrence.Properties.AutoWidth = true;
			this.chkDeleteOccurrence.Properties.Caption = resources.GetString("chkDeleteOccurrence.Properties.Caption");
			this.chkDeleteOccurrence.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.chkDeleteOccurrence.Properties.RadioGroupIndex = 1;
			this.chkDeleteOccurrence.CheckedChanged += new System.EventHandler(this.chkDeleteOccurrence_CheckedChanged);
			resources.ApplyResources(this.chkDeleteSeries, "chkDeleteSeries");
			this.chkDeleteSeries.Name = "chkDeleteSeries";
			this.chkDeleteSeries.Properties.AccessibleName = resources.GetString("chkDeleteSeries.Properties.AccessibleName");
			this.chkDeleteSeries.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.RadioButton;
			this.chkDeleteSeries.Properties.AutoWidth = true;
			this.chkDeleteSeries.Properties.Caption = resources.GetString("chkDeleteSeries.Properties.Caption");
			this.chkDeleteSeries.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.chkDeleteSeries.Properties.RadioGroupIndex = 1;
			this.chkDeleteSeries.TabStop = false;
			this.chkDeleteSeries.CheckedChanged += new System.EventHandler(this.chkDeleteOccurrence_CheckedChanged);
			this.AcceptButton = this.btnOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ControlBox = false;
			this.Controls.Add(this.chkDeleteOccurrence);
			this.Controls.Add(this.panelControl1);
			this.Controls.Add(this.lbConfirmText);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.chkDeleteSeries);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "RecurrentAppointmentDeleteForm";
			this.ShowInTaskbar = false;
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.RecurrentAppointmentDeleteForm_Paint);
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkDeleteOccurrence.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkDeleteSeries.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		protected DevExpress.XtraEditors.SimpleButton btnCancel;
		protected DevExpress.XtraEditors.SimpleButton btnOk;
		protected DevExpress.XtraEditors.LabelControl lbConfirmText;
		protected DevExpress.XtraEditors.PanelControl panelControl1;
		protected DevExpress.XtraEditors.CheckEdit chkDeleteOccurrence;
		protected DevExpress.XtraEditors.CheckEdit chkDeleteSeries;
		System.ComponentModel.IContainer components = null;
	}
}
