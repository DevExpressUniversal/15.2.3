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
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.YearlyRecurrenceControl.cbYearlyWeekDays")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.YearlyRecurrenceControl.lblYearlyOf")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.YearlyRecurrenceControl.cbYearlyMonth1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.YearlyRecurrenceControl.cbYearlyMonth")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.YearlyRecurrenceControl.spinYearlyDayNumber")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.YearlyRecurrenceControl.cbYearlyWeekOfMonth")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.YearlyRecurrenceControl.chkDay")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.YearlyRecurrenceControl.chkWeekOfMonth")]
#endregion
namespace DevExpress.XtraScheduler.UI {
	#region YearlyRecurrenceControl
	[
	DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabScheduling),
	ToolboxBitmap(typeof(SchedulerControl), DevExpress.Utils.ControlConstants.BitmapPath + "yearlyRecurrenceControl.bmp"),
	System.Runtime.InteropServices.ComVisible(false)
	]
	public class YearlyRecurrenceControl : RecurrenceControlBase {
		protected DevExpress.XtraScheduler.UI.WeekDaysEdit cbYearlyWeekDays;
		protected DevExpress.XtraEditors.LabelControl lblYearlyOf;
		protected DevExpress.XtraScheduler.UI.MonthEdit cbYearlyMonth1;
		protected DevExpress.XtraScheduler.UI.MonthEdit cbYearlyMonth;
		protected DevExpress.XtraEditors.SpinEdit spinYearlyDayNumber;
		protected DevExpress.XtraScheduler.UI.WeekOfMonthEdit cbYearlyWeekOfMonth;
		protected CheckEdit chkDay;
		private Label lblDayOfMonth;
		protected CheckEdit chkWeekOfMonth;
		public YearlyRecurrenceControl() {
			InitializeComponent();
			UpdateControlsCore();
			SubscribeControlsEvents();
		}
		protected internal override void InitRecurrenceInfo() {
			RecurrenceInfo.Type = RecurrenceType.Yearly;
		}
		protected internal virtual void SwitchToYearlyEveryNDay() {
			BeginUpdate();
			try {
				chkDay.Checked = true;
				RecurrenceInfo.WeekOfMonth = WeekOfMonth.None;
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual void SwitchToYearlyEveryWeekOfMonth() {
			BeginUpdate();
			try {
				chkWeekOfMonth.Checked = true;
				RecurrenceInfo.WeekOfMonth = cbYearlyWeekOfMonth.WeekOfMonth;
			}
			finally {
				EndUpdate();
			}
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(YearlyRecurrenceControl));
			this.cbYearlyWeekDays = new DevExpress.XtraScheduler.UI.WeekDaysEdit();
			this.cbYearlyWeekOfMonth = new DevExpress.XtraScheduler.UI.WeekOfMonthEdit();
			this.lblYearlyOf = new DevExpress.XtraEditors.LabelControl();
			this.cbYearlyMonth1 = new DevExpress.XtraScheduler.UI.MonthEdit();
			this.cbYearlyMonth = new DevExpress.XtraScheduler.UI.MonthEdit();
			this.spinYearlyDayNumber = new DevExpress.XtraEditors.SpinEdit();
			this.chkDay = new DevExpress.XtraEditors.CheckEdit();
			this.chkWeekOfMonth = new DevExpress.XtraEditors.CheckEdit();
			this.lblDayOfMonth = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.cbYearlyWeekDays.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbYearlyWeekOfMonth.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbYearlyMonth1.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbYearlyMonth.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.spinYearlyDayNumber.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkDay.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkWeekOfMonth.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.cbYearlyWeekDays, "cbYearlyWeekDays");
			this.cbYearlyWeekDays.Name = "cbYearlyWeekDays";
			this.cbYearlyWeekDays.Properties.AccessibleName = resources.GetString("cbYearlyWeekDays.Properties.AccessibleName");
			this.cbYearlyWeekDays.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbYearlyWeekDays.Properties.Buttons"))))});
			resources.ApplyResources(this.cbYearlyWeekOfMonth, "cbYearlyWeekOfMonth");
			this.cbYearlyWeekOfMonth.Name = "cbYearlyWeekOfMonth";
			this.cbYearlyWeekOfMonth.Properties.AccessibleName = resources.GetString("cbYearlyWeekOfMonth.Properties.AccessibleName");
			this.cbYearlyWeekOfMonth.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbYearlyWeekOfMonth.Properties.Buttons"))))});
			resources.ApplyResources(this.lblYearlyOf, "lblYearlyOf");
			this.lblYearlyOf.Name = "lblYearlyOf";
			resources.ApplyResources(this.cbYearlyMonth1, "cbYearlyMonth1");
			this.cbYearlyMonth1.Name = "cbYearlyMonth1";
			this.cbYearlyMonth1.Properties.AccessibleName = resources.GetString("cbYearlyMonth1.Properties.AccessibleName");
			this.cbYearlyMonth1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbYearlyMonth1.Properties.Buttons"))))});
			resources.ApplyResources(this.cbYearlyMonth, "cbYearlyMonth");
			this.cbYearlyMonth.Name = "cbYearlyMonth";
			this.cbYearlyMonth.Properties.AccessibleName = resources.GetString("cbYearlyMonth.Properties.AccessibleName");
			this.cbYearlyMonth.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbYearlyMonth.Properties.Buttons"))))});
			resources.ApplyResources(this.spinYearlyDayNumber, "spinYearlyDayNumber");
			this.spinYearlyDayNumber.Name = "spinYearlyDayNumber";
			this.spinYearlyDayNumber.Properties.AccessibleName = resources.GetString("spinYearlyDayNumber.Properties.AccessibleName");
			this.spinYearlyDayNumber.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
			this.spinYearlyDayNumber.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spinYearlyDayNumber.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
			this.spinYearlyDayNumber.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
			this.spinYearlyDayNumber.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
			this.spinYearlyDayNumber.Properties.IsFloatValue = false;
			this.spinYearlyDayNumber.Properties.Mask.EditMask = resources.GetString("spinYearlyDayNumber.Properties.Mask.EditMask");
			this.spinYearlyDayNumber.Properties.MaxValue = new decimal(new int[] {
			31,
			0,
			0,
			0});
			this.spinYearlyDayNumber.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			0});
			resources.ApplyResources(this.chkDay, "chkDay");
			this.chkDay.Name = "chkDay";
			this.chkDay.Properties.AccessibleName = resources.GetString("chkDay.Properties.AccessibleName");
			this.chkDay.Properties.Caption = resources.GetString("chkDay.Properties.Caption");
			this.chkDay.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.chkDay.Properties.RadioGroupIndex = 1;
			this.chkDay.TabStop = false;
			resources.ApplyResources(this.chkWeekOfMonth, "chkWeekOfMonth");
			this.chkWeekOfMonth.Name = "chkWeekOfMonth";
			this.chkWeekOfMonth.Properties.AccessibleName = resources.GetString("chkWeekOfMonth.Properties.AccessibleName");
			this.chkWeekOfMonth.Properties.Caption = resources.GetString("chkWeekOfMonth.Properties.Caption");
			this.chkWeekOfMonth.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.chkWeekOfMonth.Properties.RadioGroupIndex = 1;
			this.chkWeekOfMonth.TabStop = false;
			this.lblDayOfMonth.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
			resources.ApplyResources(this.lblDayOfMonth, "lblDayOfMonth");
			this.lblDayOfMonth.Name = "lblDayOfMonth";
			this.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("YearlyRecurrenceControl.Appearance.BackColor")));
			this.Appearance.Options.UseBackColor = true;
			this.Controls.Add(this.chkDay);
			this.Controls.Add(this.chkWeekOfMonth);
			this.Controls.Add(this.cbYearlyWeekDays);
			this.Controls.Add(this.cbYearlyWeekOfMonth);
			this.Controls.Add(this.lblYearlyOf);
			this.Controls.Add(this.cbYearlyMonth1);
			this.Controls.Add(this.cbYearlyMonth);
			this.Controls.Add(this.spinYearlyDayNumber);
			this.Controls.Add(this.lblDayOfMonth);
			this.Name = "YearlyRecurrenceControl";
			((System.ComponentModel.ISupportInitialize)(this.cbYearlyWeekDays.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbYearlyWeekOfMonth.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbYearlyMonth1.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbYearlyMonth.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.spinYearlyDayNumber.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkDay.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkWeekOfMonth.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected internal override void SubscribeControlsEvents() {
			cbYearlyWeekDays.EditValueChanged += new EventHandler(cbYearlyWeekDays_EditValueChanged);
			cbYearlyWeekOfMonth.EditValueChanged += new EventHandler(cbYearlyWeekOfMonth_EditValueChanged);
			cbYearlyMonth1.EditValueChanged += new EventHandler(cbYearlyMonth1_EditValueChanged);
			cbYearlyMonth.EditValueChanged += new EventHandler(cbYearlyMonth_EditValueChanged);
			spinYearlyDayNumber.EditValueChanged += new EventHandler(spinYearlyDayNumber_EditValueChanged);
			spinYearlyDayNumber.Validating += new CancelEventHandler(spinYearlyDayNumber_Validating);
			spinYearlyDayNumber.Validated += new EventHandler(spinYearlyDayNumber_Validated);
			spinYearlyDayNumber.InvalidValue += new InvalidValueExceptionEventHandler(spinYearlyDayNumber_InvalidValue);
			chkDay.EditValueChanged += new EventHandler(OnRecurrenceSubtypeChanged);
			chkWeekOfMonth.EditValueChanged += new EventHandler(OnRecurrenceSubtypeChanged);
		}
		protected internal override void UnsubscribeControlsEvents() {
			cbYearlyWeekDays.EditValueChanged -= new EventHandler(cbYearlyWeekDays_EditValueChanged);
			cbYearlyWeekOfMonth.EditValueChanged -= new EventHandler(cbYearlyWeekOfMonth_EditValueChanged);
			cbYearlyMonth1.EditValueChanged -= new EventHandler(cbYearlyMonth1_EditValueChanged);
			cbYearlyMonth.EditValueChanged -= new EventHandler(cbYearlyMonth_EditValueChanged);
			spinYearlyDayNumber.EditValueChanged -= new EventHandler(spinYearlyDayNumber_EditValueChanged);
			spinYearlyDayNumber.Validating -= new CancelEventHandler(spinYearlyDayNumber_Validating);
			spinYearlyDayNumber.Validated -= new EventHandler(spinYearlyDayNumber_Validated);
			spinYearlyDayNumber.InvalidValue -= new InvalidValueExceptionEventHandler(spinYearlyDayNumber_InvalidValue);
			chkDay.EditValueChanged -= new EventHandler(OnRecurrenceSubtypeChanged);
			chkWeekOfMonth.EditValueChanged -= new EventHandler(OnRecurrenceSubtypeChanged);
		}
		protected internal virtual void spinYearlyDayNumber_EditValueChanged(object sender, System.EventArgs e) {
			SwitchToYearlyEveryNDay();
			Validate();
		}
		protected internal virtual void spinYearlyDayNumber_InvalidValue(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e) {
			e.ErrorText = Validator.GetInvalidMonthDayNumberErrorMessage(spinYearlyDayNumber.EditValue, cbYearlyMonth.Month);
		}
		protected internal virtual void spinYearlyDayNumber_Validated(object sender, System.EventArgs e) {
			BeginUpdate();
			try {
				RecurrenceInfo.DayNumber = Validator.GetIntegerValue(spinYearlyDayNumber.EditValue); 
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual void spinYearlyDayNumber_Validating(object sender, CancelEventArgs e) {
			e.Cancel = !Validator.CheckMonthDayNumber(spinYearlyDayNumber.EditValue, cbYearlyMonth.Month);
		}
		protected internal virtual void cbYearlyWeekOfMonth_EditValueChanged(object sender, System.EventArgs e) {
			BeginUpdate();
			try {
				SwitchToYearlyEveryWeekOfMonth();
				RecurrenceInfo.WeekOfMonth = cbYearlyWeekOfMonth.WeekOfMonth;
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual void cbYearlyWeekDays_EditValueChanged(object sender, System.EventArgs e) {
			BeginUpdate();
			try {
				SwitchToYearlyEveryWeekOfMonth();
				RecurrenceInfo.WeekDays = cbYearlyWeekDays.DayOfWeek;
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual void cbYearlyMonth_EditValueChanged(object sender, System.EventArgs e) {
			BeginUpdate();
			try {
				SwitchToYearlyEveryNDay();
				RecurrenceInfo.Month = cbYearlyMonth.Month;
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual void cbYearlyMonth1_EditValueChanged(object sender, System.EventArgs e) {
			BeginUpdate();
			try {
				SwitchToYearlyEveryWeekOfMonth();
				RecurrenceInfo.Month = cbYearlyMonth1.Month;
			}
			finally {
				EndUpdate();
			}
		}
		private void OnRecurrenceSubtypeChanged(object sender, System.EventArgs e) {
			BeginUpdate();
			try {
				if (chkDay.Checked) {
					cbYearlyMonth1.Month = RecurrenceInfo.Month;
					cbYearlyWeekOfMonth.WeekOfMonth = CalcWeekOfMonth();
					cbYearlyWeekDays.DayOfWeek = RecurrenceInfo.WeekDays;
					RecurrenceInfo.WeekOfMonth = WeekOfMonth.None;
					RecurrenceInfo.Month = cbYearlyMonth.Month;
					RecurrenceInfo.DayNumber = Validator.GetIntegerValue(spinYearlyDayNumber.EditValue); 
				}
				else {
					cbYearlyMonth.Month = RecurrenceInfo.Month;
					spinYearlyDayNumber.EditValue = RecurrenceInfo.DayNumber;
					RecurrenceInfo.WeekOfMonth = cbYearlyWeekOfMonth.WeekOfMonth;
					RecurrenceInfo.Month = cbYearlyMonth1.Month;
					RecurrenceInfo.WeekDays = cbYearlyWeekDays.DayOfWeek;
				}
			}
			finally {
				EndUpdate();
			}
		}
		protected internal override void UpdateControlsCore() {
			cbYearlyMonth.Month = RecurrenceInfo.Month;
			cbYearlyMonth1.Month = RecurrenceInfo.Month;
			cbYearlyWeekDays.DayOfWeek = RecurrenceInfo.WeekDays;
			if (RecurrenceInfo.WeekOfMonth == WeekOfMonth.None) {
				chkDay.Checked = true;
				spinYearlyDayNumber.EditValue = RecurrenceInfo.DayNumber; 
				cbYearlyWeekOfMonth.WeekOfMonth = CalcWeekOfMonth();
			}
			else {
				chkWeekOfMonth.Checked = true;
				spinYearlyDayNumber.EditValue = RecurrenceInfo.DayNumber;
				cbYearlyWeekOfMonth.WeekOfMonth = RecurrenceInfo.WeekOfMonth;
			}
		}
		public override void ValidateValues(ValidationArgs args) {
			if (chkWeekOfMonth.Checked)
				return;
			Validator.ValidateMonthDayNumber(args, spinYearlyDayNumber, spinYearlyDayNumber.EditValue, cbYearlyMonth.Month);
		}
	}
	#endregion
}
