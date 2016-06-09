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
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PrintStyleControls.DailyPrintStyleOptionsControl.edtPrintTime")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PrintStyleControls.DailyPrintStyleOptionsControl.printRange")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PrintStyleControls.DailyPrintStyleOptionsControl.chkPrintAllAppointments")]
#endregion
namespace DevExpress.XtraScheduler.Design.PrintStyleControls {
	[DXToolboxItem(false), System.Runtime.InteropServices.ComVisible(false)]
	public class DailyPrintStyleOptionsControl : PrintStyleOptionsControlBase {
		protected TimeOfDayIntervalEditControl edtPrintTime;
		protected PrintRangeControl printRange;
		System.ComponentModel.IContainer components = null;
		protected DevExpress.XtraEditors.CheckEdit chkPrintAllAppointments;
		public DailyPrintStyleOptionsControl() {
			InitializeComponent();
		}
		protected internal new DailyPrintStyle PrintStyle {
			get { return (DailyPrintStyle)base.PrintStyle; }
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
			DevExpress.XtraScheduler.TimeOfDayInterval timeOfDayInterval1 = new DevExpress.XtraScheduler.TimeOfDayInterval();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DailyPrintStyleOptionsControl));
			this.edtPrintTime = new DevExpress.XtraScheduler.Design.PrintStyleControls.TimeOfDayIntervalEditControl();
			this.printRange = new DevExpress.XtraScheduler.Design.PrintRangeControl();
			this.chkPrintAllAppointments = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)(this.chkPrintAllAppointments.Properties)).BeginInit();
			this.SuspendLayout();
			this.edtPrintTime.Appearance.BackColor = System.Drawing.Color.Transparent;
			this.edtPrintTime.Appearance.Options.UseBackColor = true;
			this.edtPrintTime.Interval = timeOfDayInterval1;
			resources.ApplyResources(this.edtPrintTime, "edtPrintTime");
			this.edtPrintTime.Name = "edtPrintTime";
			this.edtPrintTime.IntervalChanged += new System.EventHandler(this.IntervalChanged);
			this.printRange.Appearance.BackColor = System.Drawing.Color.Transparent;
			this.printRange.Appearance.Options.UseBackColor = true;
			resources.ApplyResources(this.printRange, "printRange");
			this.printRange.Name = "printRange";
			this.printRange.DateRangeChanged += new System.EventHandler(this.DateRangeChanged);
			resources.ApplyResources(this.chkPrintAllAppointments, "chkPrintAllAppointments");
			this.chkPrintAllAppointments.AutoSizeInLayoutControl = true;
			this.chkPrintAllAppointments.Name = "chkPrintAllAppointments";
			this.chkPrintAllAppointments.Properties.AccessibleName = resources.GetString("chkPrintAllAppointments.Properties.AccessibleName");
			this.chkPrintAllAppointments.Properties.AutoWidth = true;
			this.chkPrintAllAppointments.Properties.Caption = resources.GetString("chkPrintAllAppointments.Properties.Caption");
			this.chkPrintAllAppointments.CheckedChanged += new System.EventHandler(this.PrintAllAppointmentsChanged);
			this.Appearance.BackColor = System.Drawing.Color.Transparent;
			this.Appearance.Options.UseBackColor = true;
			this.Controls.Add(this.chkPrintAllAppointments);
			this.Controls.Add(this.printRange);
			this.Controls.Add(this.edtPrintTime);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Name = "DailyPrintStyleOptionsControl";
			((System.ComponentModel.ISupportInitialize)(this.chkPrintAllAppointments.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		protected internal override SchedulerPrintStyle CreateDefaultPrintStyle() {
			return new DailyPrintStyle();
		}
		protected internal override bool IsValidPrintStyle(SchedulerPrintStyle style) {
			return style is DailyPrintStyle;
		}
		protected internal override void SubscribeEvents() {
			base.SubscribeEvents();
			edtPrintTime.IntervalChanged += new EventHandler(IntervalChanged);
			printRange.DateRangeChanged += new EventHandler(DateRangeChanged);
			chkPrintAllAppointments.CheckedChanged += new EventHandler(PrintAllAppointmentsChanged);
		}
		protected internal override void UnsubscribeEvents() {
			base.UnsubscribeEvents();
			edtPrintTime.IntervalChanged -= new EventHandler(IntervalChanged);
			printRange.DateRangeChanged -= new EventHandler(DateRangeChanged);
			chkPrintAllAppointments.CheckedChanged -= new EventHandler(PrintAllAppointmentsChanged);
		}
		protected internal override void RefreshDataCore(SchedulerPrintStyle printStyle) {
			base.RefreshDataCore(printStyle);
			DailyPrintStyle dailyPrintStyle = (DailyPrintStyle)printStyle;
			edtPrintTime.Interval = dailyPrintStyle.PrintTime;
			printRange.SetRange(dailyPrintStyle.StartRangeDate, dailyPrintStyle.EndRangeDate);
			chkPrintAllAppointments.Checked = dailyPrintStyle.PrintAllAppointments;
		}
		protected internal virtual void DateRangeChanged(object sender, System.EventArgs e) {
			PrintStyle.StartRangeDate = printRange.StartDate;
			PrintStyle.EndRangeDate = printRange.EndDate;
			OnPrintStyleChanged();
		}
		protected internal virtual void IntervalChanged(object sender, System.EventArgs e) {
			PrintStyle.PrintTime = edtPrintTime.Interval;
			OnPrintStyleChanged();
		}
		protected internal virtual void PrintAllAppointmentsChanged(object sender, EventArgs e) {
			PrintStyle.PrintAllAppointments = chkPrintAllAppointments.Checked;
			OnPrintStyleChanged();
		}
		protected internal override void OnBeginUpdateCore() {
			base.OnBeginUpdateCore();
			edtPrintTime.BeginUpdate();
			printRange.BeginUpdate();
		}
		protected internal override void OnEndUpdateCore() {
			base.OnEndUpdateCore();
			edtPrintTime.EndUpdate();
			printRange.EndUpdate();
		}
	}
}
