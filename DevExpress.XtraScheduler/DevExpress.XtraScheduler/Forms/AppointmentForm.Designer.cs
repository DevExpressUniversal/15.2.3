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
	partial class AppointmentForm {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AppointmentForm));
			this.lblSubject = new DevExpress.XtraEditors.LabelControl();
			this.lblLocation = new DevExpress.XtraEditors.LabelControl();
			this.tbSubject = new DevExpress.XtraEditors.TextEdit();
			this.lblLabel = new DevExpress.XtraEditors.LabelControl();
			this.lblStartTime = new DevExpress.XtraEditors.LabelControl();
			this.lblEndTime = new DevExpress.XtraEditors.LabelControl();
			this.chkAllDay = new DevExpress.XtraEditors.CheckEdit();
			this.lblShowTimeAs = new DevExpress.XtraEditors.LabelControl();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnDelete = new DevExpress.XtraEditors.SimpleButton();
			this.btnRecurrence = new DevExpress.XtraEditors.SimpleButton();
			this.edtStartDate = new DevExpress.XtraEditors.DateEdit();
			this.edtEndDate = new DevExpress.XtraEditors.DateEdit();
			this.chkReminder = new DevExpress.XtraEditors.CheckEdit();
			this.tbDescription = new DevExpress.XtraEditors.MemoEdit();
			this.lblResource = new DevExpress.XtraEditors.LabelControl();
			this.tbLocation = new DevExpress.XtraEditors.TextEdit();
			this.panel1 = new DevExpress.XtraEditors.PanelControl();
			this.progressPanel = new System.Windows.Forms.Panel();
			this.tbProgress = new DevExpress.XtraEditors.TrackBarControl();
			this.lblPercentCompleteValue = new DevExpress.XtraEditors.LabelControl();
			this.lblPercentComplete = new DevExpress.XtraEditors.LabelControl();
			this.edtResource = new DevExpress.XtraScheduler.UI.AppointmentResourceEdit();
			this.edtResources = new DevExpress.XtraScheduler.UI.AppointmentResourcesEdit();
			this.cbReminder = new DevExpress.XtraScheduler.UI.DurationEdit();
			this.edtLabel = new DevExpress.XtraScheduler.UI.AppointmentLabelEdit();
			this.edtStartTime = new DevExpress.XtraScheduler.UI.SchedulerTimeEdit();
			this.edtEndTime = new DevExpress.XtraScheduler.UI.SchedulerTimeEdit();
			this.edtShowTimeAs = new DevExpress.XtraScheduler.UI.AppointmentStatusEdit();
			((System.ComponentModel.ISupportInitialize)(this.tbSubject.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkAllDay.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtStartDate.Properties.VistaTimeProperties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtStartDate.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtEndDate.Properties.VistaTimeProperties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtEndDate.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkReminder.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tbDescription.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tbLocation.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panel1)).BeginInit();
			this.panel1.SuspendLayout();
			this.progressPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.tbProgress)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tbProgress.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtResource.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtResources.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbReminder.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtLabel.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtStartTime.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtEndTime.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtShowTimeAs.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.lblSubject, "lblSubject");
			this.lblSubject.Name = "lblSubject";
			resources.ApplyResources(this.lblLocation, "lblLocation");
			this.lblLocation.Name = "lblLocation";
			resources.ApplyResources(this.tbSubject, "tbSubject");
			this.tbSubject.Name = "tbSubject";
			this.tbSubject.Properties.AccessibleName = resources.GetString("tbSubject.Properties.AccessibleName");
			resources.ApplyResources(this.lblLabel, "lblLabel");
			this.lblLabel.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("lblLabel.Appearance.BackColor")));
			this.lblLabel.Name = "lblLabel";
			resources.ApplyResources(this.lblStartTime, "lblStartTime");
			this.lblStartTime.Name = "lblStartTime";
			resources.ApplyResources(this.lblEndTime, "lblEndTime");
			this.lblEndTime.Name = "lblEndTime";
			resources.ApplyResources(this.chkAllDay, "chkAllDay");
			this.chkAllDay.Name = "chkAllDay";
			this.chkAllDay.Properties.AccessibleName = resources.GetString("chkAllDay.Properties.AccessibleName");
			this.chkAllDay.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.CheckButton;
			this.chkAllDay.Properties.AutoWidth = true;
			this.chkAllDay.Properties.Caption = resources.GetString("chkAllDay.Properties.Caption");
			resources.ApplyResources(this.lblShowTimeAs, "lblShowTimeAs");
			this.lblShowTimeAs.Name = "lblShowTimeAs";
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.OnBtnOkClick);
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.CausesValidation = false;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			resources.ApplyResources(this.btnDelete, "btnDelete");
			this.btnDelete.CausesValidation = false;
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.Click += new System.EventHandler(this.OnBtnDeleteClick);
			resources.ApplyResources(this.btnRecurrence, "btnRecurrence");
			this.btnRecurrence.Name = "btnRecurrence";
			this.btnRecurrence.Click += new System.EventHandler(this.OnBtnRecurrenceClick);
			resources.ApplyResources(this.edtStartDate, "edtStartDate");
			this.edtStartDate.Name = "edtStartDate";
			this.edtStartDate.Properties.AccessibleName = resources.GetString("edtStartDate.Properties.AccessibleName");
			this.edtStartDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtStartDate.Properties.Buttons"))))});
			this.edtStartDate.Properties.MaxValue = new System.DateTime(4000, 1, 1, 0, 0, 0, 0);
			this.edtStartDate.Properties.VistaTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			resources.ApplyResources(this.edtEndDate, "edtEndDate");
			this.edtEndDate.Name = "edtEndDate";
			this.edtEndDate.Properties.AccessibleName = resources.GetString("edtEndDate.Properties.AccessibleName");
			this.edtEndDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtEndDate.Properties.Buttons"))))});
			this.edtEndDate.Properties.MaxValue = new System.DateTime(4000, 1, 1, 0, 0, 0, 0);
			this.edtEndDate.Properties.VistaTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			resources.ApplyResources(this.chkReminder, "chkReminder");
			this.chkReminder.Name = "chkReminder";
			this.chkReminder.Properties.AutoWidth = true;
			this.chkReminder.Properties.Caption = resources.GetString("chkReminder.Properties.Caption");
			resources.ApplyResources(this.tbDescription, "tbDescription");
			this.tbDescription.Name = "tbDescription";
			this.tbDescription.Properties.AccessibleName = resources.GetString("tbDescription.Properties.AccessibleName");
			this.tbDescription.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.Client;
			resources.ApplyResources(this.lblResource, "lblResource");
			this.lblResource.Name = "lblResource";
			resources.ApplyResources(this.tbLocation, "tbLocation");
			this.tbLocation.Name = "tbLocation";
			this.tbLocation.Properties.AccessibleName = resources.GetString("tbLocation.Properties.AccessibleName");
			resources.ApplyResources(this.panel1, "panel1");
			this.panel1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panel1.Controls.Add(this.edtResource);
			this.panel1.Controls.Add(this.lblLabel);
			this.panel1.Controls.Add(this.lblResource);
			this.panel1.Controls.Add(this.edtResources);
			this.panel1.Controls.Add(this.cbReminder);
			this.panel1.Controls.Add(this.chkAllDay);
			this.panel1.Controls.Add(this.edtLabel);
			this.panel1.Controls.Add(this.chkReminder);
			this.panel1.Name = "panel1";
			resources.ApplyResources(this.progressPanel, "progressPanel");
			this.progressPanel.Controls.Add(this.tbProgress);
			this.progressPanel.Controls.Add(this.lblPercentCompleteValue);
			this.progressPanel.Controls.Add(this.lblPercentComplete);
			this.progressPanel.Name = "progressPanel";
			this.progressPanel.TabStop = true;
			resources.ApplyResources(this.tbProgress, "tbProgress");
			this.tbProgress.Name = "tbProgress";
			this.tbProgress.Properties.AutoSize = false;
			this.tbProgress.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.tbProgress.Properties.Maximum = 100;
			this.tbProgress.Properties.ShowValueToolTip = true;
			this.tbProgress.Properties.TickFrequency = 10;
			resources.ApplyResources(this.lblPercentCompleteValue, "lblPercentCompleteValue");
			this.lblPercentCompleteValue.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("lblPercentCompleteValue.Appearance.BackColor")));
			this.lblPercentCompleteValue.Name = "lblPercentCompleteValue";
			resources.ApplyResources(this.lblPercentComplete, "lblPercentComplete");
			this.lblPercentComplete.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("lblPercentComplete.Appearance.BackColor")));
			this.lblPercentComplete.Name = "lblPercentComplete";
			resources.ApplyResources(this.edtResource, "edtResource");
			this.edtResource.Name = "edtResource";
			this.edtResource.Properties.AccessibleName = resources.GetString("edtResource.Properties.AccessibleName");
			this.edtResource.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.ComboBox;
			this.edtResource.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtResource.Properties.Buttons"))))});
			resources.ApplyResources(this.edtResources, "edtResources");
			this.edtResources.Name = "edtResources";
			this.edtResources.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtResources.Properties.Buttons"))))});
			resources.ApplyResources(this.cbReminder, "cbReminder");
			this.cbReminder.Name = "cbReminder";
			this.cbReminder.Properties.AccessibleName = resources.GetString("cbReminder.Properties.AccessibleName");
			this.cbReminder.Properties.DisabledStateText = System.String.Empty;
			this.cbReminder.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbReminder.Properties.Buttons"))))});
			resources.ApplyResources(this.edtLabel, "edtLabel");
			this.edtLabel.Name = "edtLabel";
			this.edtLabel.Properties.AccessibleName = resources.GetString("edtLabel.Properties.AccessibleName");
			this.edtLabel.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.ComboBox;
			this.edtLabel.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtLabel.Properties.Buttons"))))});
			resources.ApplyResources(this.edtStartTime, "edtStartTime");
			this.edtStartTime.Name = "edtStartTime";
			this.edtStartTime.Properties.AccessibleName = resources.GetString("edtStartTime.Properties.AccessibleName");
			this.edtStartTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			resources.ApplyResources(this.edtEndTime, "edtEndTime");
			this.edtEndTime.Name = "edtEndTime";
			this.edtEndTime.Properties.AccessibleName = resources.GetString("edtEndTime.Properties.AccessibleName");
			this.edtEndTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			resources.ApplyResources(this.edtShowTimeAs, "edtShowTimeAs");
			this.edtShowTimeAs.Name = "edtShowTimeAs";
			this.edtShowTimeAs.Properties.AccessibleName = resources.GetString("edtShowTimeAs.Properties.AccessibleName");
			this.edtShowTimeAs.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.ComboBox;
			this.edtShowTimeAs.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtShowTimeAs.Properties.Buttons"))))});
			this.AcceptButton = this.btnOk;
			this.AccessibleRole = System.Windows.Forms.AccessibleRole.Window;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.tbDescription);
			this.Controls.Add(this.progressPanel);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.edtStartTime);
			this.Controls.Add(this.edtStartDate);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.lblStartTime);
			this.Controls.Add(this.tbSubject);
			this.Controls.Add(this.lblLocation);
			this.Controls.Add(this.lblSubject);
			this.Controls.Add(this.tbLocation);
			this.Controls.Add(this.lblEndTime);
			this.Controls.Add(this.lblShowTimeAs);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnDelete);
			this.Controls.Add(this.btnRecurrence);
			this.Controls.Add(this.edtEndDate);
			this.Controls.Add(this.edtEndTime);
			this.Controls.Add(this.edtShowTimeAs);
			this.Name = "AppointmentForm";
			this.ShowInTaskbar = false;
			this.Activated += new System.EventHandler(this.OnAppointmentFormActivated);
			((System.ComponentModel.ISupportInitialize)(this.tbSubject.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkAllDay.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtStartDate.Properties.VistaTimeProperties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtStartDate.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtEndDate.Properties.VistaTimeProperties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtEndDate.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkReminder.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tbDescription.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tbLocation.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panel1)).EndInit();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.progressPanel.ResumeLayout(false);
			this.progressPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.tbProgress.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tbProgress)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtResource.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtResources.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbReminder.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtLabel.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtStartTime.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtEndTime.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtShowTimeAs.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected DevExpress.XtraEditors.LabelControl lblSubject;
		protected DevExpress.XtraEditors.LabelControl lblLocation;
		protected DevExpress.XtraEditors.LabelControl lblLabel;
		protected DevExpress.XtraEditors.LabelControl lblStartTime;
		protected DevExpress.XtraEditors.LabelControl lblEndTime;
		protected DevExpress.XtraEditors.LabelControl lblShowTimeAs;
		protected DevExpress.XtraEditors.CheckEdit chkAllDay;
		protected DevExpress.XtraEditors.SimpleButton btnOk;
		protected DevExpress.XtraEditors.SimpleButton btnCancel;
		protected DevExpress.XtraEditors.SimpleButton btnDelete;
		protected DevExpress.XtraEditors.SimpleButton btnRecurrence;
		protected DevExpress.XtraEditors.DateEdit edtStartDate;
		protected DevExpress.XtraEditors.DateEdit edtEndDate;
		protected DevExpress.XtraScheduler.UI.SchedulerTimeEdit edtStartTime;
		protected DevExpress.XtraScheduler.UI.SchedulerTimeEdit edtEndTime;
		protected DevExpress.XtraScheduler.UI.AppointmentLabelEdit edtLabel;
		protected DevExpress.XtraScheduler.UI.AppointmentStatusEdit edtShowTimeAs;
		protected DevExpress.XtraEditors.TextEdit tbSubject;
		protected DevExpress.XtraScheduler.UI.AppointmentResourceEdit edtResource;
		protected DevExpress.XtraEditors.LabelControl lblResource;
		protected DevExpress.XtraScheduler.UI.AppointmentResourcesEdit edtResources;
		protected DevExpress.XtraEditors.CheckEdit chkReminder;
		protected DevExpress.XtraEditors.MemoEdit tbDescription;
		protected DevExpress.XtraScheduler.UI.DurationEdit cbReminder;
		private System.ComponentModel.IContainer components = null;
		protected DevExpress.XtraEditors.TextEdit tbLocation;
		protected DevExpress.XtraEditors.PanelControl panel1;
		protected System.Windows.Forms.Panel progressPanel;
		protected DevExpress.XtraEditors.TrackBarControl tbProgress;
		protected DevExpress.XtraEditors.LabelControl lblPercentComplete;
		protected DevExpress.XtraEditors.LabelControl lblPercentCompleteValue;
	}
}
