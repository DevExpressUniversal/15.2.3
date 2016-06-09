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
using System.Windows.Forms;
using DevExpress.XtraScheduler.Printing;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraEditors;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PrintStyleControls.CalendarDetailsPrintStyleOptionsControl.chkStartNewPagePeriod")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PrintStyleControls.CalendarDetailsPrintStyleOptionsControl.printRange")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PrintStyleControls.CalendarDetailsPrintStyleOptionsControl.cbPeriod")]
#endregion
namespace DevExpress.XtraScheduler.Design.PrintStyleControls {
	[DXToolboxItem(false), System.Runtime.InteropServices.ComVisible(false)]
	public class CalendarDetailsPrintStyleOptionsControl : PrintStyleOptionsControlBase {
		protected CheckEdit chkStartNewPagePeriod;
		protected PrintRangeControl printRange;
		protected ImageComboBoxEdit cbPeriod;
		IContainer components = null;
		public CalendarDetailsPrintStyleOptionsControl() {
			InitializeComponent();
			FillPeriodList();
		}
		protected internal new CalendarDetailsPrintStyle PrintStyle { get { return (CalendarDetailsPrintStyle)base.PrintStyle; } }
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CalendarDetailsPrintStyleOptionsControl));
			this.chkStartNewPagePeriod = new DevExpress.XtraEditors.CheckEdit();
			this.printRange = new DevExpress.XtraScheduler.Design.PrintRangeControl();
			this.cbPeriod = new DevExpress.XtraEditors.ImageComboBoxEdit();
			((System.ComponentModel.ISupportInitialize)(this.chkStartNewPagePeriod.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbPeriod.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.chkStartNewPagePeriod, "chkStartNewPagePeriod");
			this.chkStartNewPagePeriod.Name = "chkStartNewPagePeriod";
			this.chkStartNewPagePeriod.Properties.AccessibleName = resources.GetString("chkStartNewPagePeriod.Properties.AccessibleName");
			this.chkStartNewPagePeriod.Properties.AutoWidth = true;
			this.chkStartNewPagePeriod.Properties.Caption = resources.GetString("chkStartNewPagePeriod.Properties.Caption");
			this.chkStartNewPagePeriod.CheckedChanged += new System.EventHandler(this.StartNewPagePeriodCheckedChanged);
			this.printRange.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
			this.printRange.Appearance.BackColor = System.Drawing.Color.Transparent;
			this.printRange.Appearance.Options.UseBackColor = true;
			resources.ApplyResources(this.printRange, "printRange");
			this.printRange.Name = "printRange";
			this.printRange.DateRangeChanged += new System.EventHandler(this.DateRangeChanged);
			resources.ApplyResources(this.cbPeriod, "cbPeriod");
			this.cbPeriod.Name = "cbPeriod";
			this.cbPeriod.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.ComboBox;
			this.cbPeriod.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbPeriod.Properties.Buttons"))))});
			this.cbPeriod.SelectedValueChanged += new System.EventHandler(this.PeriodChanged);
			this.Appearance.BackColor = System.Drawing.Color.Transparent;
			this.Appearance.Options.UseBackColor = true;
			this.Controls.Add(this.cbPeriod);
			this.Controls.Add(this.printRange);
			this.Controls.Add(this.chkStartNewPagePeriod);
			this.Name = "CalendarDetailsPrintStyleOptionsControl";
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			((System.ComponentModel.ISupportInitialize)(this.chkStartNewPagePeriod.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbPeriod.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		protected internal virtual void FillPeriodList() {
			string dayPeriod = SchedulerLocalizer.GetString(SchedulerStringId.PrintCalendarDetailsControlDayPeriod);
			string weekPeriod = SchedulerLocalizer.GetString(SchedulerStringId.PrintCalendarDetailsControlWeekPeriod);
			string monthPeriod = SchedulerLocalizer.GetString(SchedulerStringId.PrintCalendarDetailsControlMonthPeriod);
			cbPeriod.Properties.Items.AddRange(new ImageComboBoxItem[] {
																		   new ImageComboBoxItem(dayPeriod, PeriodKind.Day),
																		   new ImageComboBoxItem(weekPeriod, PeriodKind.Week),
																		   new ImageComboBoxItem(monthPeriod, PeriodKind.Month)
																	   });
		}
		protected internal override bool IsValidPrintStyle(SchedulerPrintStyle style) {
			return style is CalendarDetailsPrintStyle;
		}
		protected internal override void SubscribeEvents() {
			base.SubscribeEvents();
			printRange.DateRangeChanged += new EventHandler(DateRangeChanged);
			chkStartNewPagePeriod.CheckedChanged += new EventHandler(StartNewPagePeriodCheckedChanged);
			cbPeriod.SelectedValueChanged += new EventHandler(PeriodChanged);
		}
		protected internal override void UnsubscribeEvents() {
			base.UnsubscribeEvents();
			printRange.DateRangeChanged -= new EventHandler(DateRangeChanged);
			chkStartNewPagePeriod.CheckedChanged -= new EventHandler(StartNewPagePeriodCheckedChanged);
			cbPeriod.SelectedValueChanged -= new EventHandler(PeriodChanged);
		}
		protected internal override void RefreshDataCore(SchedulerPrintStyle printStyle) {
			base.RefreshDataCore(printStyle);
			CalendarDetailsPrintStyle calendarDetailsPrintStyle = (CalendarDetailsPrintStyle)printStyle;
			chkStartNewPagePeriod.Checked = calendarDetailsPrintStyle.UseNewPagePeriod;
			cbPeriod.EditValue = calendarDetailsPrintStyle.StartNewPagePeriod;
			cbPeriod.Enabled = chkStartNewPagePeriod.Checked;
			printRange.SetRange(calendarDetailsPrintStyle.StartRangeDate, calendarDetailsPrintStyle.EndRangeDate);
		}
		protected internal virtual void DateRangeChanged(object sender, System.EventArgs e) {
			PrintStyle.StartRangeDate = printRange.StartDate;
			PrintStyle.EndRangeDate = printRange.EndDate;
			OnPrintStyleChanged();
		}
		protected internal virtual void StartNewPagePeriodCheckedChanged(object sender, System.EventArgs e) {
			PrintStyle.UseNewPagePeriod = chkStartNewPagePeriod.Checked;
			OnPrintStyleChanged();
		}
		protected internal virtual void PeriodChanged(object sender, System.EventArgs e) {
			PrintStyle.StartNewPagePeriod = (PeriodKind)cbPeriod.EditValue;
			OnPrintStyleChanged();
		}
		protected internal override SchedulerPrintStyle CreateDefaultPrintStyle() {
			return new CalendarDetailsPrintStyle();
		}
		protected internal override void OnBeginUpdateCore() {
			base.OnBeginUpdateCore();
			printRange.BeginUpdate();
		}
		protected internal override void OnEndUpdateCore() {
			base.OnEndUpdateCore();
			printRange.EndUpdate();
		}
	}
}
