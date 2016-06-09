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
	partial class AppointmentRecurrenceForm {
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (CurrentRecurrenceControl != null) {
					UnsubscribeRecurrencePatternControlEvents();
					CurrentRecurrenceControl = null;
				}
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		#region Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AppointmentRecurrenceForm));
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnRemoveRecurrence = new DevExpress.XtraEditors.SimpleButton();
			this.grpAptTime = new DevExpress.XtraEditors.GroupControl();
			this.cbDuration = new DevExpress.XtraScheduler.UI.DurationEdit();
			this.edtEndTime = new DevExpress.XtraScheduler.UI.SchedulerTimeEdit();
			this.lblEnd = new DevExpress.XtraEditors.LabelControl();
			this.edtStartTime = new DevExpress.XtraScheduler.UI.SchedulerTimeEdit();
			this.lblStart = new DevExpress.XtraEditors.LabelControl();
			this.lblDuration = new DevExpress.XtraEditors.LabelControl();
			this.grpRecurrencePattern = new DevExpress.XtraEditors.GroupControl();
			this.yearlyRecurrenceControl1 = new DevExpress.XtraScheduler.UI.YearlyRecurrenceControl();
			this.monthlyRecurrenceControl1 = new DevExpress.XtraScheduler.UI.MonthlyRecurrenceControl();
			this.weeklyRecurrenceControl1 = new DevExpress.XtraScheduler.UI.WeeklyRecurrenceControl();
			this.dailyRecurrenceControl1 = new DevExpress.XtraScheduler.UI.DailyRecurrenceControl();
			this.chkWeekly = new DevExpress.XtraEditors.CheckEdit();
			this.chkMonthly = new DevExpress.XtraEditors.CheckEdit();
			this.chkDaily = new DevExpress.XtraEditors.CheckEdit();
			this.chkYearly = new DevExpress.XtraEditors.CheckEdit();
			this.grpRecurrenceRange = new DevExpress.XtraEditors.GroupControl();
			this.spinRangeOccurrencesCount = new DevExpress.XtraEditors.SpinEdit();
			this.edtRangeEnd = new DevExpress.XtraEditors.DateEdit();
			this.edtRangeStart = new DevExpress.XtraEditors.DateEdit();
			this.lblRangeOccurrencesCount = new DevExpress.XtraEditors.LabelControl();
			this.lblRangeStart = new DevExpress.XtraEditors.LabelControl();
			this.chkNoEndDate = new DevExpress.XtraEditors.CheckEdit();
			this.chkEndAfterNumberOfOccurrences = new DevExpress.XtraEditors.CheckEdit();
			this.chkEndByDate = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)(this.grpAptTime)).BeginInit();
			this.grpAptTime.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbDuration.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtEndTime.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtStartTime.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grpRecurrencePattern)).BeginInit();
			this.grpRecurrencePattern.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chkWeekly.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkMonthly.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkDaily.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkYearly.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grpRecurrenceRange)).BeginInit();
			this.grpRecurrenceRange.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.spinRangeOccurrencesCount.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtRangeEnd.Properties.VistaTimeProperties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtRangeEnd.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtRangeStart.Properties.VistaTimeProperties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtRangeStart.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkNoEndDate.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkEndAfterNumberOfOccurrences.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkEndByDate.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			resources.ApplyResources(this.btnRemoveRecurrence, "btnRemoveRecurrence");
			this.btnRemoveRecurrence.DialogResult = System.Windows.Forms.DialogResult.Abort;
			this.btnRemoveRecurrence.Name = "btnRemoveRecurrence";
			resources.ApplyResources(this.grpAptTime, "grpAptTime");
			this.grpAptTime.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
			this.grpAptTime.Controls.Add(this.cbDuration);
			this.grpAptTime.Controls.Add(this.edtEndTime);
			this.grpAptTime.Controls.Add(this.lblEnd);
			this.grpAptTime.Controls.Add(this.edtStartTime);
			this.grpAptTime.Controls.Add(this.lblStart);
			this.grpAptTime.Controls.Add(this.lblDuration);
			this.grpAptTime.Name = "grpAptTime";
			resources.ApplyResources(this.cbDuration, "cbDuration");
			this.cbDuration.Name = "cbDuration";
			this.cbDuration.Properties.AccessibleName = resources.GetString("cbDuration.Properties.AccessibleName");
			this.cbDuration.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbDuration.Properties.Buttons"))))});
			resources.ApplyResources(this.edtEndTime, "edtEndTime");
			this.edtEndTime.Name = "edtEndTime";
			this.edtEndTime.Properties.AccessibleName = resources.GetString("edtEndTime.Properties.AccessibleName");
			this.edtEndTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			resources.ApplyResources(this.lblEnd, "lblEnd");
			this.lblEnd.Name = "lblEnd";
			resources.ApplyResources(this.edtStartTime, "edtStartTime");
			this.edtStartTime.Name = "edtStartTime";
			this.edtStartTime.Properties.AccessibleName = resources.GetString("edtStartTime.Properties.AccessibleName");
			this.edtStartTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			resources.ApplyResources(this.lblStart, "lblStart");
			this.lblStart.Name = "lblStart";
			resources.ApplyResources(this.lblDuration, "lblDuration");
			this.lblDuration.Name = "lblDuration";
			resources.ApplyResources(this.grpRecurrencePattern, "grpRecurrencePattern");
			this.grpRecurrencePattern.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
			this.grpRecurrencePattern.Controls.Add(this.yearlyRecurrenceControl1);
			this.grpRecurrencePattern.Controls.Add(this.monthlyRecurrenceControl1);
			this.grpRecurrencePattern.Controls.Add(this.weeklyRecurrenceControl1);
			this.grpRecurrencePattern.Controls.Add(this.dailyRecurrenceControl1);
			this.grpRecurrencePattern.Controls.Add(this.chkWeekly);
			this.grpRecurrencePattern.Controls.Add(this.chkMonthly);
			this.grpRecurrencePattern.Controls.Add(this.chkDaily);
			this.grpRecurrencePattern.Controls.Add(this.chkYearly);
			this.grpRecurrencePattern.Name = "grpRecurrencePattern";
			this.yearlyRecurrenceControl1.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
			this.yearlyRecurrenceControl1.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("yearlyRecurrenceControl1.Appearance.BackColor")));
			this.yearlyRecurrenceControl1.Appearance.Options.UseBackColor = true;
			resources.ApplyResources(this.yearlyRecurrenceControl1, "yearlyRecurrenceControl1");
			this.yearlyRecurrenceControl1.Name = "yearlyRecurrenceControl1";
			this.monthlyRecurrenceControl1.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
			this.monthlyRecurrenceControl1.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("monthlyRecurrenceControl1.Appearance.BackColor")));
			this.monthlyRecurrenceControl1.Appearance.Options.UseBackColor = true;
			resources.ApplyResources(this.monthlyRecurrenceControl1, "monthlyRecurrenceControl1");
			this.monthlyRecurrenceControl1.Name = "monthlyRecurrenceControl1";
			this.weeklyRecurrenceControl1.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
			this.weeklyRecurrenceControl1.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("weeklyRecurrenceControl1.Appearance.BackColor")));
			this.weeklyRecurrenceControl1.Appearance.Options.UseBackColor = true;
			resources.ApplyResources(this.weeklyRecurrenceControl1, "weeklyRecurrenceControl1");
			this.weeklyRecurrenceControl1.Name = "weeklyRecurrenceControl1";
			this.dailyRecurrenceControl1.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
			this.dailyRecurrenceControl1.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("dailyRecurrenceControl1.Appearance.BackColor")));
			this.dailyRecurrenceControl1.Appearance.Options.UseBackColor = true;
			resources.ApplyResources(this.dailyRecurrenceControl1, "dailyRecurrenceControl1");
			this.dailyRecurrenceControl1.Name = "dailyRecurrenceControl1";
			resources.ApplyResources(this.chkWeekly, "chkWeekly");
			this.chkWeekly.Name = "chkWeekly";
			this.chkWeekly.Properties.AccessibleName = resources.GetString("chkWeekly.Properties.AccessibleName");
			this.chkWeekly.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.RadioButton;
			this.chkWeekly.Properties.AutoWidth = true;
			this.chkWeekly.Properties.Caption = resources.GetString("chkWeekly.Properties.Caption");
			this.chkWeekly.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.chkWeekly.Properties.RadioGroupIndex = 1;
			this.chkWeekly.TabStop = false;
			this.chkWeekly.Tag = DevExpress.XtraScheduler.RecurrenceType.Weekly;
			resources.ApplyResources(this.chkMonthly, "chkMonthly");
			this.chkMonthly.Name = "chkMonthly";
			this.chkMonthly.Properties.AccessibleName = resources.GetString("chkMonthly.Properties.AccessibleName");
			this.chkMonthly.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.RadioButton;
			this.chkMonthly.Properties.AutoWidth = true;
			this.chkMonthly.Properties.Caption = resources.GetString("chkMonthly.Properties.Caption");
			this.chkMonthly.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.chkMonthly.Properties.RadioGroupIndex = 1;
			this.chkMonthly.TabStop = false;
			this.chkMonthly.Tag = DevExpress.XtraScheduler.RecurrenceType.Monthly;
			resources.ApplyResources(this.chkDaily, "chkDaily");
			this.chkDaily.Name = "chkDaily";
			this.chkDaily.Properties.AccessibleName = resources.GetString("chkDaily.Properties.AccessibleName");
			this.chkDaily.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.RadioButton;
			this.chkDaily.Properties.AutoWidth = true;
			this.chkDaily.Properties.Caption = resources.GetString("chkDaily.Properties.Caption");
			this.chkDaily.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.chkDaily.Properties.RadioGroupIndex = 1;
			this.chkDaily.TabStop = false;
			this.chkDaily.Tag = DevExpress.XtraScheduler.RecurrenceType.Daily;
			resources.ApplyResources(this.chkYearly, "chkYearly");
			this.chkYearly.Name = "chkYearly";
			this.chkYearly.Properties.AccessibleName = resources.GetString("chkYearly.Properties.AccessibleName");
			this.chkYearly.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.RadioButton;
			this.chkYearly.Properties.AutoWidth = true;
			this.chkYearly.Properties.Caption = resources.GetString("chkYearly.Properties.Caption");
			this.chkYearly.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.chkYearly.Properties.RadioGroupIndex = 1;
			this.chkYearly.TabStop = false;
			this.chkYearly.Tag = DevExpress.XtraScheduler.RecurrenceType.Yearly;
			resources.ApplyResources(this.grpRecurrenceRange, "grpRecurrenceRange");
			this.grpRecurrenceRange.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
			this.grpRecurrenceRange.Controls.Add(this.spinRangeOccurrencesCount);
			this.grpRecurrenceRange.Controls.Add(this.edtRangeEnd);
			this.grpRecurrenceRange.Controls.Add(this.edtRangeStart);
			this.grpRecurrenceRange.Controls.Add(this.lblRangeOccurrencesCount);
			this.grpRecurrenceRange.Controls.Add(this.lblRangeStart);
			this.grpRecurrenceRange.Controls.Add(this.chkNoEndDate);
			this.grpRecurrenceRange.Controls.Add(this.chkEndAfterNumberOfOccurrences);
			this.grpRecurrenceRange.Controls.Add(this.chkEndByDate);
			this.grpRecurrenceRange.Name = "grpRecurrenceRange";
			resources.ApplyResources(this.spinRangeOccurrencesCount, "spinRangeOccurrencesCount");
			this.spinRangeOccurrencesCount.Name = "spinRangeOccurrencesCount";
			this.spinRangeOccurrencesCount.Properties.AccessibleName = resources.GetString("spinRangeOccurrencesCount.Properties.AccessibleName");
			this.spinRangeOccurrencesCount.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
			this.spinRangeOccurrencesCount.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spinRangeOccurrencesCount.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
			this.spinRangeOccurrencesCount.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
			this.spinRangeOccurrencesCount.Properties.IsFloatValue = false;
			this.spinRangeOccurrencesCount.Properties.Mask.EditMask = resources.GetString("spinRangeOccurrencesCount.Properties.Mask.EditMask");
			this.spinRangeOccurrencesCount.Properties.MaxValue = new decimal(new int[] {
			100000000,
			0,
			0,
			0});
			this.spinRangeOccurrencesCount.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			0});
			resources.ApplyResources(this.edtRangeEnd, "edtRangeEnd");
			this.edtRangeEnd.Name = "edtRangeEnd";
			this.edtRangeEnd.Properties.AccessibleName = resources.GetString("edtRangeEnd.Properties.AccessibleName");
			this.edtRangeEnd.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtRangeEnd.Properties.Buttons"))))});
			this.edtRangeEnd.Properties.MaxValue = new System.DateTime(4000, 1, 1, 0, 0, 0, 0);
			this.edtRangeEnd.Properties.VistaTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			resources.ApplyResources(this.edtRangeStart, "edtRangeStart");
			this.edtRangeStart.Name = "edtRangeStart";
			this.edtRangeStart.Properties.AccessibleName = resources.GetString("edtRangeStart.Properties.AccessibleName");
			this.edtRangeStart.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtRangeStart.Properties.Buttons"))))});
			this.edtRangeStart.Properties.MaxValue = new System.DateTime(4000, 1, 1, 0, 0, 0, 0);
			this.edtRangeStart.Properties.VistaTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			resources.ApplyResources(this.lblRangeOccurrencesCount, "lblRangeOccurrencesCount");
			this.lblRangeOccurrencesCount.Name = "lblRangeOccurrencesCount";
			resources.ApplyResources(this.lblRangeStart, "lblRangeStart");
			this.lblRangeStart.Name = "lblRangeStart";
			resources.ApplyResources(this.chkNoEndDate, "chkNoEndDate");
			this.chkNoEndDate.Name = "chkNoEndDate";
			this.chkNoEndDate.Properties.AccessibleName = resources.GetString("chkNoEndDate.Properties.AccessibleName");
			this.chkNoEndDate.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.RadioButton;
			this.chkNoEndDate.Properties.AutoWidth = true;
			this.chkNoEndDate.Properties.Caption = resources.GetString("chkNoEndDate.Properties.Caption");
			this.chkNoEndDate.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.chkNoEndDate.Properties.RadioGroupIndex = 2;
			this.chkNoEndDate.TabStop = false;
			this.chkNoEndDate.Tag = DevExpress.XtraScheduler.RecurrenceRange.NoEndDate;
			resources.ApplyResources(this.chkEndAfterNumberOfOccurrences, "chkEndAfterNumberOfOccurrences");
			this.chkEndAfterNumberOfOccurrences.Name = "chkEndAfterNumberOfOccurrences";
			this.chkEndAfterNumberOfOccurrences.Properties.AccessibleName = resources.GetString("chkEndAfterNumberOfOccurrences.Properties.AccessibleName");
			this.chkEndAfterNumberOfOccurrences.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.RadioButton;
			this.chkEndAfterNumberOfOccurrences.Properties.AutoWidth = true;
			this.chkEndAfterNumberOfOccurrences.Properties.Caption = resources.GetString("chkEndAfterNumberOfOccurrences.Properties.Caption");
			this.chkEndAfterNumberOfOccurrences.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.chkEndAfterNumberOfOccurrences.Properties.RadioGroupIndex = 2;
			this.chkEndAfterNumberOfOccurrences.TabStop = false;
			this.chkEndAfterNumberOfOccurrences.Tag = DevExpress.XtraScheduler.RecurrenceRange.OccurrenceCount;
			resources.ApplyResources(this.chkEndByDate, "chkEndByDate");
			this.chkEndByDate.Name = "chkEndByDate";
			this.chkEndByDate.Properties.AccessibleName = resources.GetString("chkEndByDate.Properties.AccessibleName");
			this.chkEndByDate.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.RadioButton;
			this.chkEndByDate.Properties.AutoWidth = true;
			this.chkEndByDate.Properties.Caption = resources.GetString("chkEndByDate.Properties.Caption");
			this.chkEndByDate.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.chkEndByDate.Properties.RadioGroupIndex = 2;
			this.chkEndByDate.TabStop = false;
			this.chkEndByDate.Tag = DevExpress.XtraScheduler.RecurrenceRange.EndByDate;
			this.AcceptButton = this.btnOk;
			this.AccessibleRole = System.Windows.Forms.AccessibleRole.Window;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.btnRemoveRecurrence);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.grpRecurrenceRange);
			this.Controls.Add(this.grpRecurrencePattern);
			this.Controls.Add(this.grpAptTime);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AppointmentRecurrenceForm";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			((System.ComponentModel.ISupportInitialize)(this.grpAptTime)).EndInit();
			this.grpAptTime.ResumeLayout(false);
			this.grpAptTime.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbDuration.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtEndTime.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtStartTime.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpRecurrencePattern)).EndInit();
			this.grpRecurrencePattern.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.chkWeekly.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkMonthly.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkDaily.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkYearly.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpRecurrenceRange)).EndInit();
			this.grpRecurrenceRange.ResumeLayout(false);
			this.grpRecurrenceRange.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.spinRangeOccurrencesCount.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtRangeEnd.Properties.VistaTimeProperties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtRangeEnd.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtRangeStart.Properties.VistaTimeProperties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtRangeStart.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkNoEndDate.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkEndAfterNumberOfOccurrences.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkEndByDate.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private System.ComponentModel.Container components = null;
		protected DevExpress.XtraEditors.SimpleButton btnOk;
		protected DevExpress.XtraEditors.SimpleButton btnCancel;
		protected DevExpress.XtraEditors.SimpleButton btnRemoveRecurrence;
		protected DevExpress.XtraEditors.GroupControl grpAptTime;
		protected DevExpress.XtraEditors.GroupControl grpRecurrencePattern;
		protected DevExpress.XtraEditors.GroupControl grpRecurrenceRange;
		protected DevExpress.XtraEditors.LabelControl lblRangeStart;
		protected DevExpress.XtraEditors.DateEdit edtRangeStart;
		protected DevExpress.XtraEditors.LabelControl lblRangeOccurrencesCount;
		protected DevExpress.XtraEditors.DateEdit edtRangeEnd;
		protected DevExpress.XtraScheduler.UI.SchedulerTimeEdit edtStartTime;
		protected DevExpress.XtraEditors.LabelControl lblStart;
		protected DevExpress.XtraEditors.LabelControl lblEnd;
		protected DevExpress.XtraScheduler.UI.SchedulerTimeEdit edtEndTime;
		protected DevExpress.XtraEditors.SpinEdit spinRangeOccurrencesCount;
		protected DevExpress.XtraScheduler.UI.DailyRecurrenceControl dailyRecurrenceControl1;
		protected DevExpress.XtraScheduler.UI.WeeklyRecurrenceControl weeklyRecurrenceControl1;
		protected DevExpress.XtraScheduler.UI.MonthlyRecurrenceControl monthlyRecurrenceControl1;
		protected DevExpress.XtraScheduler.UI.YearlyRecurrenceControl yearlyRecurrenceControl1;
		protected DevExpress.XtraEditors.LabelControl lblDuration;
		protected DurationEdit cbDuration;
		protected DevExpress.XtraEditors.CheckEdit chkNoEndDate;
		protected DevExpress.XtraEditors.CheckEdit chkEndByDate;
		protected DevExpress.XtraEditors.CheckEdit chkDaily;
		protected DevExpress.XtraEditors.CheckEdit chkWeekly;
		protected DevExpress.XtraEditors.CheckEdit chkMonthly;
		protected DevExpress.XtraEditors.CheckEdit chkYearly;
		protected DevExpress.XtraEditors.CheckEdit chkEndAfterNumberOfOccurrences;
	}
}
