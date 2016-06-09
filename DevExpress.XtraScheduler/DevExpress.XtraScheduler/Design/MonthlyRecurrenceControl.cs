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

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Localization;
using DevExpress.Utils;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.MonthlyRecurrenceControl.lblMonthlyOfEvery")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.MonthlyRecurrenceControl.lblMonthlyMonthCount")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.MonthlyRecurrenceControl.lblMonthlyMonthCount1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.MonthlyRecurrenceControl.cbMonthlyWeekDays")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.MonthlyRecurrenceControl.spinMonthlyDay")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.MonthlyRecurrenceControl.spinMonthlyMonthCount1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.MonthlyRecurrenceControl.spinMonthlyMonthCount")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.MonthlyRecurrenceControl.cbMonthlyWeekOfMonth")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.MonthlyRecurrenceControl.lblOfEveryWeekOfMonth")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.MonthlyRecurrenceControl.chkDay")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.MonthlyRecurrenceControl.chkWeekOfMonth")]
#endregion
namespace DevExpress.XtraScheduler.UI {
	#region MonthlyRecurrenceControl
	[
	DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabScheduling),
	ToolboxBitmap(typeof(SchedulerControl), DevExpress.Utils.ControlConstants.BitmapPath + "monthlyRecurrenceControl.bmp"),
	System.Runtime.InteropServices.ComVisible(false),
	Description("A control used to set the recurrence options for monthly recurrent appointments.")
	]
	public class MonthlyRecurrenceControl : RecurrenceControlBase {
		protected DevExpress.XtraEditors.LabelControl lblMonthlyOfEvery;
		protected DevExpress.XtraEditors.LabelControl lblMonthlyMonthCount;
		protected DevExpress.XtraEditors.LabelControl lblMonthlyMonthCount1;
		protected DevExpress.XtraScheduler.UI.WeekDaysEdit cbMonthlyWeekDays;
		protected DevExpress.XtraEditors.SpinEdit spinMonthlyDay;
		protected DevExpress.XtraEditors.SpinEdit spinMonthlyMonthCount1;
		protected DevExpress.XtraEditors.SpinEdit spinMonthlyMonthCount;
		protected DevExpress.XtraScheduler.UI.WeekOfMonthEdit cbMonthlyWeekOfMonth;
		protected DevExpress.XtraEditors.LabelControl lblOfEveryWeekOfMonth;
		protected CheckEdit chkDay;
		protected CheckEdit chkWeekOfMonth;
		public MonthlyRecurrenceControl() {
			InitializeComponent();
			UpdateControlsCore();
			SubscribeControlsEvents();
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MonthlyRecurrenceControl));
			this.lblMonthlyOfEvery = new DevExpress.XtraEditors.LabelControl();
			this.lblMonthlyMonthCount = new DevExpress.XtraEditors.LabelControl();
			this.cbMonthlyWeekOfMonth = new DevExpress.XtraScheduler.UI.WeekOfMonthEdit();
			this.lblOfEveryWeekOfMonth = new DevExpress.XtraEditors.LabelControl();
			this.lblMonthlyMonthCount1 = new DevExpress.XtraEditors.LabelControl();
			this.cbMonthlyWeekDays = new DevExpress.XtraScheduler.UI.WeekDaysEdit();
			this.spinMonthlyDay = new DevExpress.XtraEditors.SpinEdit();
			this.spinMonthlyMonthCount1 = new DevExpress.XtraEditors.SpinEdit();
			this.spinMonthlyMonthCount = new DevExpress.XtraEditors.SpinEdit();
			this.chkDay = new DevExpress.XtraEditors.CheckEdit();
			this.chkWeekOfMonth = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)(this.cbMonthlyWeekOfMonth.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbMonthlyWeekDays.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.spinMonthlyDay.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.spinMonthlyMonthCount1.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.spinMonthlyMonthCount.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkDay.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkWeekOfMonth.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.lblMonthlyOfEvery, "lblMonthlyOfEvery");
			this.lblMonthlyOfEvery.Name = "lblMonthlyOfEvery";
			resources.ApplyResources(this.lblMonthlyMonthCount, "lblMonthlyMonthCount");
			this.lblMonthlyMonthCount.Name = "lblMonthlyMonthCount";
			resources.ApplyResources(this.cbMonthlyWeekOfMonth, "cbMonthlyWeekOfMonth");
			this.cbMonthlyWeekOfMonth.Name = "cbMonthlyWeekOfMonth";
			this.cbMonthlyWeekOfMonth.Properties.AccessibleName = resources.GetString("cbMonthlyWeekOfMonth.Properties.AccessibleName");
			this.cbMonthlyWeekOfMonth.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbMonthlyWeekOfMonth.Properties.Buttons"))))});
			resources.ApplyResources(this.lblOfEveryWeekOfMonth, "lblOfEveryWeekOfMonth");
			this.lblOfEveryWeekOfMonth.Name = "lblOfEveryWeekOfMonth";
			resources.ApplyResources(this.lblMonthlyMonthCount1, "lblMonthlyMonthCount1");
			this.lblMonthlyMonthCount1.Name = "lblMonthlyMonthCount1";
			resources.ApplyResources(this.cbMonthlyWeekDays, "cbMonthlyWeekDays");
			this.cbMonthlyWeekDays.Name = "cbMonthlyWeekDays";
			this.cbMonthlyWeekDays.Properties.AccessibleName = resources.GetString("cbMonthlyWeekDays.Properties.AccessibleName");
			this.cbMonthlyWeekDays.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbMonthlyWeekDays.Properties.Buttons"))))});
			resources.ApplyResources(this.spinMonthlyDay, "spinMonthlyDay");
			this.spinMonthlyDay.Name = "spinMonthlyDay";
			this.spinMonthlyDay.Properties.AccessibleName = resources.GetString("spinMonthlyDay.Properties.AccessibleName");
			this.spinMonthlyDay.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
			this.spinMonthlyDay.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spinMonthlyDay.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
			this.spinMonthlyDay.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
			this.spinMonthlyDay.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
			this.spinMonthlyDay.Properties.IsFloatValue = false;
			this.spinMonthlyDay.Properties.Mask.EditMask = resources.GetString("spinMonthlyDay.Properties.Mask.EditMask");
			this.spinMonthlyDay.Properties.MaxValue = new decimal(new int[] {
			31,
			0,
			0,
			0});
			this.spinMonthlyDay.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			0});
			resources.ApplyResources(this.spinMonthlyMonthCount1, "spinMonthlyMonthCount1");
			this.spinMonthlyMonthCount1.Name = "spinMonthlyMonthCount1";
			this.spinMonthlyMonthCount1.Properties.AccessibleName = resources.GetString("spinMonthlyMonthCount1.Properties.AccessibleName");
			this.spinMonthlyMonthCount1.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
			this.spinMonthlyMonthCount1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spinMonthlyMonthCount1.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
			this.spinMonthlyMonthCount1.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
			this.spinMonthlyMonthCount1.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
			this.spinMonthlyMonthCount1.Properties.IsFloatValue = false;
			this.spinMonthlyMonthCount1.Properties.Mask.EditMask = resources.GetString("spinMonthlyMonthCount1.Properties.Mask.EditMask");
			this.spinMonthlyMonthCount1.Properties.MaxValue = new decimal(new int[] {
			2147483647,
			0,
			0,
			0});
			this.spinMonthlyMonthCount1.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			0});
			resources.ApplyResources(this.spinMonthlyMonthCount, "spinMonthlyMonthCount");
			this.spinMonthlyMonthCount.Name = "spinMonthlyMonthCount";
			this.spinMonthlyMonthCount.Properties.AccessibleName = resources.GetString("spinMonthlyMonthCount.Properties.AccessibleName");
			this.spinMonthlyMonthCount.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
			this.spinMonthlyMonthCount.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spinMonthlyMonthCount.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
			this.spinMonthlyMonthCount.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
			this.spinMonthlyMonthCount.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
			this.spinMonthlyMonthCount.Properties.IsFloatValue = false;
			this.spinMonthlyMonthCount.Properties.Mask.EditMask = resources.GetString("spinMonthlyMonthCount.Properties.Mask.EditMask");
			this.spinMonthlyMonthCount.Properties.MaxValue = new decimal(new int[] {
			2147483647,
			0,
			0,
			0});
			this.spinMonthlyMonthCount.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			0});
			resources.ApplyResources(this.chkDay, "chkDay");
			this.chkDay.Name = "chkDay";
			this.chkDay.Properties.AccessibleName = resources.GetString("chkDay.Properties.AccessibleName");
			this.chkDay.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.RadioButton;
			this.chkDay.Properties.Caption = resources.GetString("chkDay.Properties.Caption");
			this.chkDay.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.chkDay.Properties.RadioGroupIndex = 1;
			this.chkDay.TabStop = false;
			resources.ApplyResources(this.chkWeekOfMonth, "chkWeekOfMonth");
			this.chkWeekOfMonth.Name = "chkWeekOfMonth";
			this.chkWeekOfMonth.Properties.AccessibleName = resources.GetString("chkWeekOfMonth.Properties.AccessibleName");
			this.chkWeekOfMonth.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.RadioButton;
			this.chkWeekOfMonth.Properties.Caption = resources.GetString("chkWeekOfMonth.Properties.Caption");
			this.chkWeekOfMonth.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.chkWeekOfMonth.Properties.RadioGroupIndex = 1;
			this.chkWeekOfMonth.TabStop = false;
			this.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("MonthlyRecurrenceControl.Appearance.BackColor")));
			this.Appearance.Options.UseBackColor = true;
			this.Controls.Add(this.chkDay);
			this.Controls.Add(this.lblMonthlyOfEvery);
			this.Controls.Add(this.lblMonthlyMonthCount);
			this.Controls.Add(this.cbMonthlyWeekOfMonth);
			this.Controls.Add(this.lblOfEveryWeekOfMonth);
			this.Controls.Add(this.lblMonthlyMonthCount1);
			this.Controls.Add(this.cbMonthlyWeekDays);
			this.Controls.Add(this.spinMonthlyDay);
			this.Controls.Add(this.spinMonthlyMonthCount1);
			this.Controls.Add(this.spinMonthlyMonthCount);
			this.Controls.Add(this.chkWeekOfMonth);
			this.Name = "MonthlyRecurrenceControl";
			((System.ComponentModel.ISupportInitialize)(this.cbMonthlyWeekOfMonth.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbMonthlyWeekDays.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.spinMonthlyDay.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.spinMonthlyMonthCount1.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.spinMonthlyMonthCount.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkDay.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkWeekOfMonth.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected internal override void InitRecurrenceInfo() {
			RecurrenceInfo.Type = RecurrenceType.Monthly;
		}
		protected internal override void SubscribeControlsEvents() {
			cbMonthlyWeekOfMonth.EditValueChanged += new EventHandler(cbMonthlyWeekOfMonth_EditValueChanged);
			cbMonthlyWeekDays.EditValueChanged += new EventHandler(cbMonthlyWeekDays_EditValueChanged);
			spinMonthlyDay.EditValueChanged += new EventHandler(spinMonthlyDay_EditValueChanged);
			spinMonthlyDay.Validating += new CancelEventHandler(spinMonthlyDay_Validating);
			spinMonthlyDay.Validated += new EventHandler(spinMonthlyDay_Validated);
			spinMonthlyDay.InvalidValue += new InvalidValueExceptionEventHandler(spinMonthlyDay_InvalidValue);
			spinMonthlyMonthCount1.EditValueChanged += new EventHandler(spinMonthlyMonthCount1_EditValueChanged);
			spinMonthlyMonthCount1.Validating += new CancelEventHandler(spinMonthlyMonthCount1_Validating);
			spinMonthlyMonthCount1.Validated += new EventHandler(spinMonthlyMonthCount1_Validated);
			spinMonthlyMonthCount1.InvalidValue += new InvalidValueExceptionEventHandler(spinMonthlyMonthCount1_InvalidValue);
			spinMonthlyMonthCount.EditValueChanged += new EventHandler(spinMonthlyMonthCount_EditValueChanged);
			spinMonthlyMonthCount.Validating += new CancelEventHandler(spinMonthlyMonthCount_Validating);
			spinMonthlyMonthCount.Validated += new EventHandler(spinMonthlyMonthCount_Validated);
			spinMonthlyMonthCount.InvalidValue += new InvalidValueExceptionEventHandler(spinMonthlyMonthCount_InvalidValue);
			chkDay.EditValueChanged += new EventHandler(OnRecurrenceSubtypeChanged);
			chkWeekOfMonth.EditValueChanged += new EventHandler(OnRecurrenceSubtypeChanged);
		}
		protected internal override void UnsubscribeControlsEvents() {
			cbMonthlyWeekOfMonth.EditValueChanged -= new EventHandler(cbMonthlyWeekOfMonth_EditValueChanged);
			cbMonthlyWeekDays.EditValueChanged -= new EventHandler(cbMonthlyWeekDays_EditValueChanged);
			spinMonthlyDay.EditValueChanged -= new EventHandler(spinMonthlyDay_EditValueChanged);
			spinMonthlyDay.Validating -= new CancelEventHandler(spinMonthlyDay_Validating);
			spinMonthlyDay.Validated -= new EventHandler(spinMonthlyDay_Validated);
			spinMonthlyDay.InvalidValue -= new InvalidValueExceptionEventHandler(spinMonthlyDay_InvalidValue);
			spinMonthlyMonthCount1.EditValueChanged -= new EventHandler(spinMonthlyMonthCount1_EditValueChanged);
			spinMonthlyMonthCount1.Validating -= new CancelEventHandler(spinMonthlyMonthCount1_Validating);
			spinMonthlyMonthCount1.Validated -= new EventHandler(spinMonthlyMonthCount1_Validated);
			spinMonthlyMonthCount1.InvalidValue -= new InvalidValueExceptionEventHandler(spinMonthlyMonthCount1_InvalidValue);
			spinMonthlyMonthCount.EditValueChanged -= new EventHandler(spinMonthlyMonthCount_EditValueChanged);
			spinMonthlyMonthCount.Validating -= new CancelEventHandler(spinMonthlyMonthCount_Validating);
			spinMonthlyMonthCount.Validated -= new EventHandler(spinMonthlyMonthCount_Validated);
			spinMonthlyMonthCount.InvalidValue -= new InvalidValueExceptionEventHandler(spinMonthlyMonthCount_InvalidValue);
			chkDay.EditValueChanged -= new EventHandler(OnRecurrenceSubtypeChanged);
			chkWeekOfMonth.EditValueChanged -= new EventHandler(OnRecurrenceSubtypeChanged);
		}
		protected internal virtual void SwitchToMonthlyEveryWeekOfMonth() {
			BeginUpdate();
			try {
				chkWeekOfMonth.Checked = true;
				RecurrenceInfo.WeekOfMonth = cbMonthlyWeekOfMonth.WeekOfMonth;
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual void SwitchToMonthlyEveryNDay() {
			BeginUpdate();
			try {
				chkDay.Checked = true;
				RecurrenceInfo.WeekOfMonth = WeekOfMonth.None;
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual void spinMonthlyDay_EditValueChanged(object sender, EventArgs e) {
			SwitchToMonthlyEveryNDay();
			Validate();
		}
		protected internal virtual void spinMonthlyDay_InvalidValue(object sender, InvalidValueExceptionEventArgs e) {
			e.ErrorText = Validator.GetInvalidDayNumberErrorMessage(spinMonthlyDay.EditValue, 31);
		}
		protected internal virtual void spinMonthlyDay_Validated(object sender, System.EventArgs e) {
			BeginUpdate();
			try {
				RecurrenceInfo.DayNumber = Validator.GetIntegerValue(spinMonthlyDay.EditValue);
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual void spinMonthlyDay_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
			e.Cancel = !Validator.CheckDayNumber(spinMonthlyDay.EditValue, 31);
		}
		protected internal virtual void spinMonthlyMonthCount_EditValueChanged(object sender, System.EventArgs e) {
			SwitchToMonthlyEveryNDay();
			Validate();
		}
		protected internal virtual void spinMonthlyMonthCount_InvalidValue(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e) {
			e.ErrorText = Validator.GetInvalidMonthCountErrorMessage(spinMonthlyMonthCount.EditValue);
		}
		protected internal virtual void spinMonthlyMonthCount_Validated(object sender, System.EventArgs e) {
			if (RecurrenceInfo.WeekOfMonth != WeekOfMonth.None)
				return;
			BeginUpdate();
			try {
				spinMonthlyMonthCount1.EditValue = spinMonthlyMonthCount.EditValue;
				RecurrenceInfo.Periodicity = Validator.GetIntegerValue(spinMonthlyMonthCount.EditValue); 
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual void spinMonthlyMonthCount_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
			e.Cancel = !Validator.CheckPositiveValue(spinMonthlyMonthCount.EditValue);
		}
		protected internal virtual void spinMonthlyMonthCount1_EditValueChanged(object sender, System.EventArgs e) {
			SwitchToMonthlyEveryWeekOfMonth();
			Validate();
		}
		protected internal virtual void spinMonthlyMonthCount1_InvalidValue(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e) {
			e.ErrorText = Validator.GetInvalidMonthCountErrorMessage(spinMonthlyMonthCount1.EditValue);
		}
		protected internal virtual void spinMonthlyMonthCount1_Validated(object sender, System.EventArgs e) {
			if (RecurrenceInfo.WeekOfMonth == WeekOfMonth.None)
				return;
			BeginUpdate();
			try {
				spinMonthlyMonthCount.EditValue = spinMonthlyMonthCount1.EditValue;
				RecurrenceInfo.Periodicity = Validator.GetIntegerValue(spinMonthlyMonthCount1.EditValue);
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual void spinMonthlyMonthCount1_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
			e.Cancel = !Validator.CheckPositiveValue(spinMonthlyMonthCount1.EditValue);
		}
		protected internal virtual void cbMonthlyWeekOfMonth_EditValueChanged(object sender, System.EventArgs e) {
			BeginUpdate();
			try {
				SwitchToMonthlyEveryWeekOfMonth();
				RecurrenceInfo.WeekOfMonth = cbMonthlyWeekOfMonth.WeekOfMonth;
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual void cbMonthlyWeekDays_EditValueChanged(object sender, System.EventArgs e) {
			BeginUpdate();
			try {
				SwitchToMonthlyEveryWeekOfMonth();
				RecurrenceInfo.WeekDays = cbMonthlyWeekDays.DayOfWeek;
			}
			finally {
				EndUpdate();
			}
		}
		protected internal override void UpdateControlsCore() {
			spinMonthlyMonthCount.EditValue = RecurrenceInfo.Periodicity;
			spinMonthlyMonthCount1.EditValue = RecurrenceInfo.Periodicity;
			cbMonthlyWeekDays.DayOfWeek = RecurrenceInfo.WeekDays;
			if (RecurrenceInfo.WeekOfMonth == WeekOfMonth.None) {
				chkDay.Checked = true;
				cbMonthlyWeekOfMonth.WeekOfMonth = CalcWeekOfMonth();
				spinMonthlyDay.EditValue = RecurrenceInfo.DayNumber;
			}
			else {
				chkWeekOfMonth.Checked = true;
				cbMonthlyWeekOfMonth.WeekOfMonth = RecurrenceInfo.WeekOfMonth;
				spinMonthlyDay.EditValue = RecurrenceInfo.Start.Day;
			}
		}
		public override void ValidateValues(ValidationArgs args) {
			if (chkDay.Checked) {
				ValidateMonthCount(args, spinMonthlyMonthCount);
				if (!args.Valid)
					return;
				Validator.ValidateDayNumber(args, spinMonthlyDay, spinMonthlyDay.EditValue, 31);
			}
			else
				ValidateMonthCount(args, spinMonthlyMonthCount1);
		}
		protected internal virtual void ValidateMonthCount(ValidationArgs args, TextEdit edit) {
			Validator.ValidateMonthCount(args, edit, edit.EditValue);
		}
		public override void CheckForWarnings(ValidationArgs args) {
			if (Validator.NeedToCheckLargeDayNumberWarning(RecurrenceInfo.WeekOfMonth))
				ValidateLargeDayNumber(args, spinMonthlyDay);
		}
		protected internal virtual void OnRecurrenceSubtypeChanged(object sender, System.EventArgs e) {
			BeginUpdate();
			try {
				if (chkWeekOfMonth.Checked) {
					spinMonthlyDay.EditValue = RecurrenceInfo.DayNumber;
					RecurrenceInfo.WeekOfMonth = CalcWeekOfMonth();
				}
				else {
					cbMonthlyWeekOfMonth.WeekOfMonth = CalcWeekOfMonth();
					cbMonthlyWeekDays.DayOfWeek = RecurrenceInfo.WeekDays;
					RecurrenceInfo.WeekOfMonth = WeekOfMonth.None;
				}
			}
			finally {
				EndUpdate();
			}
		}
	}
	#endregion
}
