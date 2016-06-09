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
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Printing;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PrintStyleControls.WeeklyPrintStyleOptionsControl.chkDontPrintWeekends")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PrintStyleControls.WeeklyPrintStyleOptionsControl.edtPrintInterval")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PrintStyleControls.WeeklyPrintStyleOptionsControl.lblLayout")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PrintStyleControls.WeeklyPrintStyleOptionsControl.lblArrangeDays")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PrintStyleControls.WeeklyPrintStyleOptionsControl.printRange")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PrintStyleControls.WeeklyPrintStyleOptionsControl.edtLayout")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PrintStyleControls.WeeklyPrintStyleOptionsControl.chkTopToBottom")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PrintStyleControls.WeeklyPrintStyleOptionsControl.chkLeftToRight")]
#endregion
namespace DevExpress.XtraScheduler.Design.PrintStyleControls {
	[DXToolboxItem(false), System.Runtime.InteropServices.ComVisible(false)]
	public class WeeklyPrintStyleOptionsControl : PrintStyleOptionsControlBase {
		protected CheckEdit chkDontPrintWeekends;
		protected TimeOfDayIntervalEditControl edtPrintInterval;
		protected DevExpress.XtraEditors.LabelControl lblLayout;
		protected DevExpress.XtraEditors.LabelControl lblArrangeDays;
		protected PrintRangeControl printRange;
		protected ImageComboBoxEdit edtLayout;
		protected CheckEdit chkTopToBottom;
		protected CheckEdit chkLeftToRight;
		IContainer components = null;
		public WeeklyPrintStyleOptionsControl() {
			InitializeComponent();
			FillLayoutList();
		}
		protected internal new WeeklyPrintStyle PrintStyle { get { return (WeeklyPrintStyle)base.PrintStyle; } }
		protected internal virtual void FillLayoutList() {
			string oneWeekPerPage = SchedulerLocalizer.GetString(SchedulerStringId.PrintWeeklyOptControlOneWeekPerPage);
			string twoWeekPerPage = SchedulerLocalizer.GetString(SchedulerStringId.PrintWeeklyOptControlTwoWeekPerPage);
			edtLayout.Properties.Items.AddRange(new ImageComboBoxItem[] {
																			new ImageComboBoxItem(oneWeekPerPage, PageLayout.OnePage),
																			new ImageComboBoxItem(twoWeekPerPage, PageLayout.TwoPage)
																		});
		}
		#region Dispose
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null) {
					components.Dispose();
					components = null;
				}
			}
			base.Dispose(disposing);
		}
		#endregion
		#region Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WeeklyPrintStyleOptionsControl));
			DevExpress.XtraScheduler.TimeOfDayInterval timeOfDayInterval1 = new DevExpress.XtraScheduler.TimeOfDayInterval();
			this.chkDontPrintWeekends = new DevExpress.XtraEditors.CheckEdit();
			this.lblLayout = new DevExpress.XtraEditors.LabelControl();
			this.lblArrangeDays = new DevExpress.XtraEditors.LabelControl();
			this.edtPrintInterval = new DevExpress.XtraScheduler.Design.PrintStyleControls.TimeOfDayIntervalEditControl();
			this.printRange = new DevExpress.XtraScheduler.Design.PrintRangeControl();
			this.edtLayout = new DevExpress.XtraEditors.ImageComboBoxEdit();
			this.chkTopToBottom = new DevExpress.XtraEditors.CheckEdit();
			this.chkLeftToRight = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)(this.chkDontPrintWeekends.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtLayout.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkTopToBottom.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkLeftToRight.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.chkDontPrintWeekends, "chkDontPrintWeekends");
			this.chkDontPrintWeekends.Name = "chkDontPrintWeekends";
			this.chkDontPrintWeekends.Properties.AutoWidth = true;
			this.chkDontPrintWeekends.Properties.Caption = resources.GetString("chkDontPrintWeekends.Properties.Caption");
			this.chkDontPrintWeekends.AutoSizeInLayoutControl = true;
			resources.ApplyResources(this.lblLayout, "lblLayout");
			this.lblLayout.Name = "lblLayout";
			resources.ApplyResources(this.lblArrangeDays, "lblArrangeDays");
			this.lblArrangeDays.Name = "lblArrangeDays";
			this.edtPrintInterval.Appearance.BackColor = System.Drawing.Color.Transparent;
			this.edtPrintInterval.Appearance.Options.UseBackColor = true;
			this.edtPrintInterval.Interval = timeOfDayInterval1;
			resources.ApplyResources(this.edtPrintInterval, "edtPrintInterval");
			this.edtPrintInterval.Name = "edtPrintInterval";
			this.printRange.Appearance.BackColor = System.Drawing.Color.Transparent;
			this.printRange.Appearance.Options.UseBackColor = true;
			resources.ApplyResources(this.printRange, "printRange");
			this.printRange.Name = "printRange";
			resources.ApplyResources(this.edtLayout, "edtLayout");
			this.edtLayout.Name = "edtLayout";
			this.edtLayout.Properties.AccessibleName = resources.GetString("edtLayout.Properties.AccessibleName");
			this.edtLayout.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.ComboBox;
			this.edtLayout.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtLayout.Properties.Buttons"))))});
			resources.ApplyResources(this.chkTopToBottom, "chkTopToBottom");
			this.chkTopToBottom.Name = "chkTopToBottom";
			this.chkTopToBottom.Properties.AccessibleName = resources.GetString("chkTopToBottom.Properties.AccessibleName");
			this.chkTopToBottom.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.RadioButton;
			this.chkTopToBottom.Properties.AutoWidth = true;
			this.chkTopToBottom.Properties.Caption = resources.GetString("chkTopToBottom.Properties.Caption");
			this.chkTopToBottom.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.chkTopToBottom.Properties.RadioGroupIndex = 1;
			this.chkTopToBottom.TabStop = false;
			resources.ApplyResources(this.chkLeftToRight, "chkLeftToRight");
			this.chkLeftToRight.Name = "chkLeftToRight";
			this.chkLeftToRight.Properties.AccessibleName = resources.GetString("chkLeftToRight.Properties.AccessibleName");
			this.chkLeftToRight.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.RadioButton;
			this.chkLeftToRight.Properties.AutoWidth = true;
			this.chkLeftToRight.Properties.Caption = resources.GetString("chkLeftToRight.Properties.Caption");
			this.chkLeftToRight.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.chkLeftToRight.Properties.RadioGroupIndex = 1;
			this.chkLeftToRight.TabStop = false;
			this.Appearance.BackColor = System.Drawing.Color.Transparent;
			this.Appearance.Options.UseBackColor = true;
			this.Controls.Add(this.chkLeftToRight);
			this.Controls.Add(this.chkTopToBottom);
			this.Controls.Add(this.edtLayout);
			this.Controls.Add(this.printRange);
			this.Controls.Add(this.edtPrintInterval);
			this.Controls.Add(this.lblArrangeDays);
			this.Controls.Add(this.chkDontPrintWeekends);
			this.Controls.Add(this.lblLayout);
			this.Name = "WeeklyPrintStyleOptionsControl";
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			resources.ApplyResources(this, "$this");
			((System.ComponentModel.ISupportInitialize)(this.chkDontPrintWeekends.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtLayout.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkTopToBottom.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkLeftToRight.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected internal override SchedulerPrintStyle CreateDefaultPrintStyle() {
			return new WeeklyPrintStyle();
		}
		protected internal override bool IsValidPrintStyle(SchedulerPrintStyle style) {
			return style is WeeklyPrintStyle;
		}
		protected internal override void SubscribeEvents() {
			base.SubscribeEvents();
			printRange.DateRangeChanged += new EventHandler(DateRangeChanged);
			edtLayout.EditValueChanged += new EventHandler(LayoutChanged);
			chkDontPrintWeekends.CheckedChanged += new EventHandler(DontPrintWeekendsCheckChanged);
			edtPrintInterval.IntervalChanged += new EventHandler(IntervalChanged);
			chkTopToBottom.CheckedChanged += new EventHandler(ArrangeDaysChanged);
			chkLeftToRight.CheckedChanged += new EventHandler(ArrangeDaysChanged);
		}
		protected internal override void UnsubscribeEvents() {
			base.UnsubscribeEvents();
			printRange.DateRangeChanged -= new EventHandler(DateRangeChanged);
			edtLayout.EditValueChanged -= new EventHandler(LayoutChanged);
			chkDontPrintWeekends.CheckedChanged -= new EventHandler(DontPrintWeekendsCheckChanged);
			edtPrintInterval.IntervalChanged -= new EventHandler(IntervalChanged);
			chkTopToBottom.CheckedChanged -= new EventHandler(ArrangeDaysChanged);
			chkLeftToRight.CheckedChanged -= new EventHandler(ArrangeDaysChanged);
		}
		protected internal override void RefreshDataCore(SchedulerPrintStyle printStyle) {
			base.RefreshDataCore(printStyle);
			WeeklyPrintStyle weeklyPrintStyle = (WeeklyPrintStyle)printStyle;
			if (weeklyPrintStyle.ArrangeDays == ArrangeDaysKind.TopToBottom)
				chkTopToBottom.Checked = true;
			else
				chkLeftToRight.Checked = true;
			edtLayout.EditValue = weeklyPrintStyle.Layout;
			edtPrintInterval.Interval = weeklyPrintStyle.PrintTime;
			chkDontPrintWeekends.Checked = !weeklyPrintStyle.PrintWeekends;
			printRange.SetRange(weeklyPrintStyle.StartRangeDate, weeklyPrintStyle.EndRangeDate);
			if (weeklyPrintStyle.ArrangeDays == ArrangeDaysKind.LeftToRight)
				EnableLeftRightElement();
			else
				DisableLeftRightElement();
		}
		protected internal override void OnBeginUpdateCore() {
			base.OnBeginUpdateCore();
			edtPrintInterval.BeginUpdate();
			printRange.BeginUpdate();
		}
		protected internal override void OnEndUpdateCore() {
			base.OnEndUpdateCore();
			edtPrintInterval.EndUpdate();
			printRange.EndUpdate();
		}
		protected internal virtual void EnableLeftRightElement() {
			edtPrintInterval.Enabled = true;
			chkDontPrintWeekends.Enabled = true;
		}
		protected internal virtual void DisableLeftRightElement() {
			edtPrintInterval.Enabled = false;
			chkDontPrintWeekends.Enabled = false;
		}
		protected internal virtual void DateRangeChanged(object sender, System.EventArgs e) {
			PrintStyle.StartRangeDate = printRange.StartDate;
			PrintStyle.EndRangeDate = printRange.EndDate;
			OnPrintStyleChanged();
		}
		protected internal virtual void LayoutChanged(object sender, EventArgs e) {
			PrintStyle.Layout = (PageLayout)edtLayout.EditValue;
			OnPrintStyleChanged();
		}
		protected internal virtual void ArrangeDaysChanged(object sender, System.EventArgs e) {
			if (chkTopToBottom.Checked)
				PrintStyle.ArrangeDays = ArrangeDaysKind.TopToBottom;
			else
				PrintStyle.ArrangeDays = ArrangeDaysKind.LeftToRight;
			OnPrintStyleChanged();
		}
		protected internal virtual void IntervalChanged(object sender, System.EventArgs e) {
			PrintStyle.PrintTime = edtPrintInterval.Interval;
			OnPrintStyleChanged();
		}
		protected internal virtual void DontPrintWeekendsCheckChanged(object sender, System.EventArgs e) {
			PrintStyle.PrintWeekends = !chkDontPrintWeekends.Checked;
			OnPrintStyleChanged();
		}
	}
}
