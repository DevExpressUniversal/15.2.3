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
	partial class RecurrentAppointmentEditForm {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RecurrentAppointmentEditForm));
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.lbConfirmText = new DevExpress.XtraEditors.LabelControl();
			this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
			this.chkEditOccurrence = new DevExpress.XtraEditors.CheckEdit();
			this.chkEditSeries = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkEditOccurrence.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkEditSeries.Properties)).BeginInit();
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
			resources.ApplyResources(this.chkEditOccurrence, "chkEditOccurrence");
			this.chkEditOccurrence.Name = "chkEditOccurrence";
			this.chkEditOccurrence.Properties.AccessibleName = resources.GetString("chkEditOccurrence.Properties.AccessibleName");
			this.chkEditOccurrence.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.RadioButton;
			this.chkEditOccurrence.Properties.AutoWidth = true;
			this.chkEditOccurrence.Properties.Caption = resources.GetString("chkEditOccurrence.Properties.Caption");
			this.chkEditOccurrence.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.chkEditOccurrence.Properties.RadioGroupIndex = 1;
			this.chkEditOccurrence.CheckedChanged += new System.EventHandler(this.chkEditOccurrence_CheckedChanged);
			resources.ApplyResources(this.chkEditSeries, "chkEditSeries");
			this.chkEditSeries.Name = "chkEditSeries";
			this.chkEditSeries.Properties.AccessibleName = resources.GetString("chkEditSeries.Properties.AccessibleName");
			this.chkEditSeries.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.RadioButton;
			this.chkEditSeries.Properties.AutoWidth = true;
			this.chkEditSeries.Properties.Caption = resources.GetString("chkEditSeries.Properties.Caption");
			this.chkEditSeries.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.chkEditSeries.Properties.RadioGroupIndex = 1;
			this.chkEditSeries.TabStop = false;
			this.chkEditSeries.CheckedChanged += new System.EventHandler(this.chkEditOccurrence_CheckedChanged);
			this.AcceptButton = this.btnOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ControlBox = false;
			this.Controls.Add(this.chkEditOccurrence);
			this.Controls.Add(this.panelControl1);
			this.Controls.Add(this.lbConfirmText);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.chkEditSeries);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "RecurrentAppointmentEditForm";
			this.ShowInTaskbar = false;
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.RecurrentAppointmentEditForm_Paint);
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkEditOccurrence.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkEditSeries.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		protected DevExpress.XtraEditors.SimpleButton btnCancel;
		protected DevExpress.XtraEditors.SimpleButton btnOk;
		protected DevExpress.XtraEditors.LabelControl lbConfirmText;
		protected DevExpress.XtraEditors.PanelControl panelControl1;
		protected DevExpress.XtraEditors.CheckEdit chkEditOccurrence;
		protected DevExpress.XtraEditors.CheckEdit chkEditSeries;
		System.ComponentModel.IContainer components = null;
	}
}
