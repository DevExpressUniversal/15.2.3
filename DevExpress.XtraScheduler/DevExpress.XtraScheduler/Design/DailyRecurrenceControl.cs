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
using System.ComponentModel;
using System.Drawing;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraScheduler.Localization;
using DevExpress.Utils;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.DailyRecurrenceControl.spinDailyDaysCount")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.DailyRecurrenceControl.lblDailyDaysCount")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.DailyRecurrenceControl.chkDay")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.DailyRecurrenceControl.chkEveryWeekDay")]
#endregion
namespace DevExpress.XtraScheduler.UI {
	#region DailyRecurrenceControl
	[
	DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabScheduling),
	ToolboxBitmap(typeof(SchedulerControl), DevExpress.Utils.ControlConstants.BitmapPath + "dailyRecurrenceControl.bmp"),
	System.Runtime.InteropServices.ComVisible(false),
	Description("A control used to set recurrence options for daily recurrent appointments in appointment editing dialogs.")
	]
	public class DailyRecurrenceControl : RecurrenceControlBase {
		protected DevExpress.XtraEditors.SpinEdit spinDailyDaysCount;
		protected DevExpress.XtraEditors.LabelControl lblDailyDaysCount;
		protected DevExpress.XtraEditors.CheckEdit chkDay;
		protected DevExpress.XtraEditors.CheckEdit chkEveryWeekDay;
		public DailyRecurrenceControl() {
			InitializeComponent();
			UpdateControlsCore();
			SubscribeControlsEvents();
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DailyRecurrenceControl));
			this.spinDailyDaysCount = new DevExpress.XtraEditors.SpinEdit();
			this.lblDailyDaysCount = new DevExpress.XtraEditors.LabelControl();
			this.chkDay = new DevExpress.XtraEditors.CheckEdit();
			this.chkEveryWeekDay = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)(this.spinDailyDaysCount.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkDay.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkEveryWeekDay.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.spinDailyDaysCount, "spinDailyDaysCount");
			this.spinDailyDaysCount.Name = "spinDailyDaysCount";
			this.spinDailyDaysCount.Properties.AccessibleName = resources.GetString("spinDailyDaysCount.Properties.AccessibleName");
			this.spinDailyDaysCount.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
			this.spinDailyDaysCount.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spinDailyDaysCount.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
			this.spinDailyDaysCount.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
			this.spinDailyDaysCount.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
			this.spinDailyDaysCount.Properties.IsFloatValue = false;
			this.spinDailyDaysCount.Properties.Mask.EditMask = resources.GetString("spinDailyDaysCount.Properties.Mask.EditMask");
			this.spinDailyDaysCount.Properties.MaxValue = new decimal(new int[] {
			100000000,
			0,
			0,
			0});
			this.spinDailyDaysCount.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			0});
			resources.ApplyResources(this.lblDailyDaysCount, "lblDailyDaysCount");
			this.lblDailyDaysCount.Name = "lblDailyDaysCount";
			resources.ApplyResources(this.chkDay, "chkDay");
			this.chkDay.Name = "chkDay";
			this.chkDay.Properties.AccessibleName = resources.GetString("chkDay.Properties.AccessibleName");
			this.chkDay.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.RadioButton;
			this.chkDay.Properties.Caption = resources.GetString("chkDay.Properties.Caption");
			this.chkDay.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.chkDay.Properties.RadioGroupIndex = 1;
			this.chkDay.TabStop = false;
			resources.ApplyResources(this.chkEveryWeekDay, "chkEveryWeekDay");
			this.chkEveryWeekDay.Name = "chkEveryWeekDay";
			this.chkEveryWeekDay.Properties.AccessibleName = resources.GetString("chkEveryWeekDay.Properties.AccessibleName");
			this.chkEveryWeekDay.Properties.Caption = resources.GetString("chkEveryWeekDay.Properties.Caption");
			this.chkEveryWeekDay.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.chkEveryWeekDay.Properties.RadioGroupIndex = 1;
			this.chkEveryWeekDay.TabStop = false;
			this.Appearance.BackColor = System.Drawing.Color.Transparent;
			this.Appearance.Options.UseBackColor = true;
			this.Controls.Add(this.chkDay);
			this.Controls.Add(this.chkEveryWeekDay);
			this.Controls.Add(this.spinDailyDaysCount);
			this.Controls.Add(this.lblDailyDaysCount);
			this.Name = "DailyRecurrenceControl";
			((System.ComponentModel.ISupportInitialize)(this.spinDailyDaysCount.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkDay.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkEveryWeekDay.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected internal override void InitRecurrenceInfo() {
			RecurrenceInfo.Type = RecurrenceType.Daily;
		}
		protected internal override void SubscribeControlsEvents() {
			spinDailyDaysCount.EditValueChanged += new EventHandler(spinDailyDaysCount_EditValueChanged);
			spinDailyDaysCount.Validating += new CancelEventHandler(spinDailyDaysCount_Validating);
			spinDailyDaysCount.Validated += new EventHandler(spinDailyDaysCount_Validated);
			spinDailyDaysCount.InvalidValue += new InvalidValueExceptionEventHandler(spinDailyDaysCount_InvalidValue);
			chkDay.EditValueChanged += new EventHandler(OnRecurrenceSubtypeChanged);
			chkEveryWeekDay.EditValueChanged += new EventHandler(OnRecurrenceSubtypeChanged);
		}
		protected internal override void UnsubscribeControlsEvents() {
			spinDailyDaysCount.EditValueChanged -= new EventHandler(spinDailyDaysCount_EditValueChanged);
			spinDailyDaysCount.Validating -= new CancelEventHandler(spinDailyDaysCount_Validating);
			spinDailyDaysCount.Validated -= new EventHandler(spinDailyDaysCount_Validated);
			spinDailyDaysCount.InvalidValue -= new InvalidValueExceptionEventHandler(spinDailyDaysCount_InvalidValue);
			chkDay.EditValueChanged -= new EventHandler(OnRecurrenceSubtypeChanged);
			chkEveryWeekDay.EditValueChanged -= new EventHandler(OnRecurrenceSubtypeChanged);
		}
		protected internal virtual void spinDailyDaysCount_EditValueChanged(object sender, EventArgs e) {
			BeginUpdate();
			try {
				chkDay.Checked = true;
				RecurrenceInfo.WeekDays = WeekDays.EveryDay;
			}
			finally {
				EndUpdate();
			}
			Validate();
		}
		protected internal virtual void spinDailyDaysCount_InvalidValue(object sender, InvalidValueExceptionEventArgs e) {
			e.ErrorText = Validator.GetInvalidDayCountErrorMessage(spinDailyDaysCount.EditValue);
		}
		protected internal virtual void spinDailyDaysCount_Validated(object sender, EventArgs e) {
			BeginUpdate();
			try {
				RecurrenceInfo.Periodicity = Validator.GetIntegerValue(spinDailyDaysCount.EditValue); 
				spinDailyDaysCount.EditValue = RecurrenceInfo.Periodicity;
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual void spinDailyDaysCount_Validating(object sender, CancelEventArgs e) {
			e.Cancel = !Validator.CheckPositiveValue(spinDailyDaysCount.EditValue);
		}
		protected internal override void UpdateControlsCore() {
			if (RecurrenceInfo.WeekDays == WeekDays.EveryDay)
				chkDay.Checked = true;
			else
				chkEveryWeekDay.Checked = true;
			spinDailyDaysCount.EditValue = RecurrenceInfo.Periodicity;
		}
		public override void ValidateValues(ValidationArgs args) {
			if (chkEveryWeekDay.Checked)
				return;
			Validator.ValidateDayCount(args, spinDailyDaysCount, spinDailyDaysCount.EditValue);
		}
		protected internal virtual void OnRecurrenceSubtypeChanged(object sender, System.EventArgs e) {
			BeginUpdate();
			try {
				if (chkEveryWeekDay.Checked) {
					spinDailyDaysCount.EditValue = 1;  ;
					RecurrenceInfo.WeekDays = WeekDays.WorkDays;
				}
				else
					RecurrenceInfo.WeekDays = WeekDays.EveryDay;
			}
			finally {
				EndUpdate();
			}
		}
	}
	#endregion
}
