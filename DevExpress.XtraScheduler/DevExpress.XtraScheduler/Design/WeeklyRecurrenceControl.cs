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
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Localization;
using DevExpress.Utils;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.WeeklyRecurrenceControl.lblWeeklyRecur")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.WeeklyRecurrenceControl.lblWeeklyWeeksOn")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.WeeklyRecurrenceControl.spinWeeklyWeeksCount")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.WeeklyRecurrenceControl.weekDaysCheckEdit")]
#endregion
namespace DevExpress.XtraScheduler.UI {
	[
	DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabScheduling),
	ToolboxBitmap(typeof(SchedulerControl), DevExpress.Utils.ControlConstants.BitmapPath + "weeklyRecurrenceControl.bmp"),
	System.Runtime.InteropServices.ComVisible(false),
	Description("A control used to set recurrence options for weekly recurrent appointments.")
	]
	public class WeeklyRecurrenceControl : RecurrenceControlBase {
		protected DevExpress.XtraEditors.LabelControl lblWeeklyRecur;
		protected DevExpress.XtraEditors.LabelControl lblWeeklyWeeksOn;
		protected DevExpress.XtraEditors.SpinEdit spinWeeklyWeeksCount;
		protected DevExpress.XtraScheduler.UI.WeekDaysCheckEdit weekDaysCheckEdit;
		FirstDayOfWeek firstDayOfWeek = FirstDayOfWeek.System;
		public WeeklyRecurrenceControl() {
			InitializeComponent();
			weekDaysCheckEdit.FirstDayOfWeek = firstDayOfWeek;
			UpdateControlsCore();
			SubscribeControlsEvents();
		}
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("WeeklyRecurrenceControlFirstDayOfWeek"),
#endif
DefaultValue(FirstDayOfWeek.System)]
		public FirstDayOfWeek FirstDayOfWeek {
			get { return firstDayOfWeek; }
			set {
				if (firstDayOfWeek == value)
					return;
				firstDayOfWeek = value;
				if (weekDaysCheckEdit != null) {
					BeginUpdate();
					try {
						WeekDays weekDays = weekDaysCheckEdit.WeekDays;
						weekDaysCheckEdit.FirstDayOfWeek = firstDayOfWeek;
						weekDaysCheckEdit.WeekDays = weekDays;
					}
					finally {
						CancelUpdate();
					}
				}
			}
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WeeklyRecurrenceControl));
			this.lblWeeklyRecur = new DevExpress.XtraEditors.LabelControl();
			this.lblWeeklyWeeksOn = new DevExpress.XtraEditors.LabelControl();
			this.spinWeeklyWeeksCount = new DevExpress.XtraEditors.SpinEdit();
			this.weekDaysCheckEdit = new DevExpress.XtraScheduler.UI.WeekDaysCheckEdit();
			((System.ComponentModel.ISupportInitialize)(this.spinWeeklyWeeksCount.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.weekDaysCheckEdit)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.lblWeeklyRecur, "lblWeeklyRecur");
			this.lblWeeklyRecur.Name = "lblWeeklyRecur";
			resources.ApplyResources(this.lblWeeklyWeeksOn, "lblWeeklyWeeksOn");
			this.lblWeeklyWeeksOn.Name = "lblWeeklyWeeksOn";
			resources.ApplyResources(this.spinWeeklyWeeksCount, "spinWeeklyWeeksCount");
			this.spinWeeklyWeeksCount.Name = "spinWeeklyWeeksCount";
			this.spinWeeklyWeeksCount.Properties.AccessibleName = resources.GetString("spinWeeklyWeeksCount.Properties.AccessibleName");
			this.spinWeeklyWeeksCount.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
			this.spinWeeklyWeeksCount.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spinWeeklyWeeksCount.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
			this.spinWeeklyWeeksCount.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
			this.spinWeeklyWeeksCount.Properties.IsFloatValue = false;
			this.spinWeeklyWeeksCount.Properties.Mask.EditMask = resources.GetString("spinWeeklyWeeksCount.Properties.Mask.EditMask");
			this.spinWeeklyWeeksCount.Properties.MaxValue = new decimal(new int[] {
			100000000,
			0,
			0,
			0});
			this.spinWeeklyWeeksCount.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.weekDaysCheckEdit.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
			resources.ApplyResources(this.weekDaysCheckEdit, "weekDaysCheckEdit");
			this.weekDaysCheckEdit.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("weekDaysCheckEdit.Appearance.BackColor")));
			this.weekDaysCheckEdit.Appearance.Options.UseBackColor = true;
			this.weekDaysCheckEdit.Name = "weekDaysCheckEdit";
			this.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("WeeklyRecurrenceControl.Appearance.BackColor")));
			this.Appearance.Options.UseBackColor = true;
			this.Controls.Add(this.weekDaysCheckEdit);
			this.Controls.Add(this.lblWeeklyRecur);
			this.Controls.Add(this.lblWeeklyWeeksOn);
			this.Controls.Add(this.spinWeeklyWeeksCount);
			this.Name = "WeeklyRecurrenceControl";
			((System.ComponentModel.ISupportInitialize)(this.spinWeeklyWeeksCount.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.weekDaysCheckEdit)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected internal override void InitRecurrenceInfo() {
			RecurrenceInfo.Type = RecurrenceType.Weekly;
		}
		protected internal override void SubscribeControlsEvents() {
			this.spinWeeklyWeeksCount.EditValueChanged += OnSpinWeeklyWeeksCountEditValueChanged;
			this.spinWeeklyWeeksCount.Validating += this.spinWeeklyWeeksCount_Validating;
			this.spinWeeklyWeeksCount.Validated += this.spinWeeklyWeeksCount_Validated;
			this.weekDaysCheckEdit.WeekDaysChanged += this.weekDaysCheckEdit_WeekDaysChanged;
		}	   
		protected internal override void UnsubscribeControlsEvents() {
			this.spinWeeklyWeeksCount.EditValueChanged -= OnSpinWeeklyWeeksCountEditValueChanged;
			this.spinWeeklyWeeksCount.Validating -= this.spinWeeklyWeeksCount_Validating;
			this.spinWeeklyWeeksCount.Validated -= this.spinWeeklyWeeksCount_Validated;
			this.weekDaysCheckEdit.WeekDaysChanged -= this.weekDaysCheckEdit_WeekDaysChanged;
		}
		void OnSpinWeeklyWeeksCountEditValueChanged(object sender, EventArgs e) {
			Validate();
		}
		protected internal virtual void spinWeeklyWeeksCount_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
			e.Cancel = !Validator.CheckPositiveValue(spinWeeklyWeeksCount.EditValue);
		}
		protected internal virtual void spinWeeklyWeeksCount_Validated(object sender, System.EventArgs e) {
			BeginUpdate();
			try {
				RecurrenceInfo.Periodicity = Validator.GetIntegerValue(spinWeeklyWeeksCount.EditValue);
			}
			finally {
				EndUpdate();
			}
		}
		protected internal override void UpdateControlsCore() {
			spinWeeklyWeeksCount.EditValue = RecurrenceInfo.Periodicity;
			weekDaysCheckEdit.WeekDays = RecurrenceInfo.WeekDays;
		}
		public override void ValidateValues(ValidationArgs args) {
			Validator.ValidateWeekCount(args, spinWeeklyWeeksCount, spinWeeklyWeeksCount.EditValue);
			Validator.ValidateDayOfWeek(args, weekDaysCheckEdit, weekDaysCheckEdit.WeekDays);
		}
		protected internal virtual void weekDaysCheckEdit_WeekDaysChanged(object sender, System.EventArgs e) {
			BeginUpdate();
			try {
				RecurrenceInfo.WeekDays = weekDaysCheckEdit.WeekDays;
			}
			finally {
				EndUpdate();
			}
		}
	}
}
