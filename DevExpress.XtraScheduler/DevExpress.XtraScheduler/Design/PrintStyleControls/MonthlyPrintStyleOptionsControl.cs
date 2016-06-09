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
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PrintStyleControls.MonthlyPrintStyleOptionsControl.chkDontPrintWeekends")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PrintStyleControls.MonthlyPrintStyleOptionsControl.chkPrintExactlyOneMonth")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PrintStyleControls.MonthlyPrintStyleOptionsControl.lblLayout")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PrintStyleControls.MonthlyPrintStyleOptionsControl.printRange")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PrintStyleControls.MonthlyPrintStyleOptionsControl.cbLayout")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PrintStyleControls.MonthlyPrintStyleOptionsControl.chkCompressWeekend")]
#endregion
namespace DevExpress.XtraScheduler.Design.PrintStyleControls {
	[DXToolboxItem(false), System.Runtime.InteropServices.ComVisible(false)]
	public class MonthlyPrintStyleOptionsControl : PrintStyleOptionsControlBase {
		protected CheckEdit chkDontPrintWeekends;
		protected CheckEdit chkPrintExactlyOneMonth;
		protected DevExpress.XtraEditors.LabelControl lblLayout;
		protected PrintRangeControl printRange;
		protected ImageComboBoxEdit cbLayout;
		protected CheckEdit chkCompressWeekend;
		IContainer components = null;
		public MonthlyPrintStyleOptionsControl() {
			InitializeComponent();
			FillLayoutList();
		}
		protected internal virtual void FillLayoutList() {
			string onePagePerMonth = SchedulerLocalizer.GetString(SchedulerStringId.PrintMonthlyOptControlOnePagePerMonth);
			string twoPagesPerMonth = SchedulerLocalizer.GetString(SchedulerStringId.PrintMonthlyOptControlTwoPagesPerMonth);
			cbLayout.Properties.Items.AddRange(new ImageComboBoxItem[] {
																			new ImageComboBoxItem(onePagePerMonth, PageLayout.OnePage),
																			new ImageComboBoxItem(twoPagesPerMonth, PageLayout.TwoPage)
																		});
		}
		protected internal new MonthlyPrintStyle PrintStyle { get { return (MonthlyPrintStyle)base.PrintStyle; } }
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MonthlyPrintStyleOptionsControl));
			this.lblLayout = new DevExpress.XtraEditors.LabelControl();
			this.chkDontPrintWeekends = new DevExpress.XtraEditors.CheckEdit();
			this.chkPrintExactlyOneMonth = new DevExpress.XtraEditors.CheckEdit();
			this.printRange = new DevExpress.XtraScheduler.Design.PrintRangeControl();
			this.cbLayout = new DevExpress.XtraEditors.ImageComboBoxEdit();
			this.chkCompressWeekend = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)(this.chkDontPrintWeekends.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkPrintExactlyOneMonth.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbLayout.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkCompressWeekend.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.lblLayout, "lblLayout");
			this.lblLayout.Name = "lblLayout";
			resources.ApplyResources(this.chkDontPrintWeekends, "chkDontPrintWeekends");
			this.chkDontPrintWeekends.Name = "chkDontPrintWeekends";
			this.chkDontPrintWeekends.Properties.AutoWidth = true;
			this.chkDontPrintWeekends.Properties.Caption = resources.GetString("chkDontPrintWeekends.Properties.Caption");
			this.chkDontPrintWeekends.CheckedChanged += new System.EventHandler(this.DontPrintWeekendsCheckedChanged);
			this.chkDontPrintWeekends.AutoSizeInLayoutControl = true;
			resources.ApplyResources(this.chkPrintExactlyOneMonth, "chkPrintExactlyOneMonth");
			this.chkPrintExactlyOneMonth.Name = "chkPrintExactlyOneMonth";
			this.chkPrintExactlyOneMonth.Properties.AutoWidth = true;
			this.chkPrintExactlyOneMonth.Properties.Caption = resources.GetString("chkPrintExactlyOneMonth.Properties.Caption");
			this.chkPrintExactlyOneMonth.CheckedChanged += new System.EventHandler(this.PrintExactlyOneMonthCheckedChanged);
			this.chkPrintExactlyOneMonth.AutoSizeInLayoutControl = true;
			this.printRange.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
			this.printRange.Appearance.BackColor = System.Drawing.Color.Transparent;
			this.printRange.Appearance.Options.UseBackColor = true;
			resources.ApplyResources(this.printRange, "printRange");
			this.printRange.Name = "printRange";
			this.printRange.DateRangeChanged += new System.EventHandler(this.DateRangeChanged);
			resources.ApplyResources(this.cbLayout, "cbLayout");
			this.cbLayout.Name = "cbLayout";
			this.cbLayout.Properties.AccessibleName = resources.GetString("cbLayout.Properties.AccessibleName");
			this.cbLayout.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.ComboBox;
			this.cbLayout.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbLayout.Properties.Buttons"))))});
			this.cbLayout.SelectedIndexChanged += new System.EventHandler(this.LayoutChanged);
			resources.ApplyResources(this.chkCompressWeekend, "chkCompressWeekend");
			this.chkCompressWeekend.Name = "chkCompressWeekend";
			this.chkCompressWeekend.Properties.AutoWidth = true;
			this.chkCompressWeekend.Properties.Caption = resources.GetString("chkCompressWeekend.Properties.Caption");
			this.chkCompressWeekend.CheckedChanged += new System.EventHandler(this.CompressWeekendCheckedChanged);
			this.chkCompressWeekend.AutoSizeInLayoutControl = true;
			this.Appearance.BackColor = System.Drawing.Color.Transparent;
			this.Appearance.Options.UseBackColor = true;
			this.Controls.Add(this.chkCompressWeekend);
			this.Controls.Add(this.cbLayout);
			this.Controls.Add(this.printRange);
			this.Controls.Add(this.chkPrintExactlyOneMonth);
			this.Controls.Add(this.chkDontPrintWeekends);
			this.Controls.Add(this.lblLayout);
			this.Name = "MonthlyPrintStyleOptionsControl";
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			((System.ComponentModel.ISupportInitialize)(this.chkDontPrintWeekends.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkPrintExactlyOneMonth.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbLayout.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkCompressWeekend.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected internal override bool IsValidPrintStyle(SchedulerPrintStyle style) {
			return style is MonthlyPrintStyle;
		}
		protected internal override void SubscribeEvents() {
			base.SubscribeEvents();
			printRange.DateRangeChanged += new EventHandler(DateRangeChanged);
			chkCompressWeekend.CheckedChanged += new EventHandler(CompressWeekendCheckedChanged);
			chkDontPrintWeekends.CheckedChanged += new EventHandler(DontPrintWeekendsCheckedChanged);
			chkPrintExactlyOneMonth.CheckedChanged += new EventHandler(PrintExactlyOneMonthCheckedChanged);
			cbLayout.SelectedIndexChanged += new EventHandler(LayoutChanged);
		}
		protected internal override void UnsubscribeEvents() {
			base.UnsubscribeEvents();
			printRange.DateRangeChanged -= new EventHandler(DateRangeChanged);
			chkCompressWeekend.CheckedChanged -= new EventHandler(CompressWeekendCheckedChanged);
			chkDontPrintWeekends.CheckedChanged -= new EventHandler(DontPrintWeekendsCheckedChanged);
			chkPrintExactlyOneMonth.CheckedChanged -= new EventHandler(PrintExactlyOneMonthCheckedChanged);
			cbLayout.SelectedIndexChanged -= new EventHandler(LayoutChanged);
		}
		protected internal override void RefreshDataCore(SchedulerPrintStyle printStyle) {
			base.RefreshDataCore(printStyle);
			MonthlyPrintStyle monthlyPrintStyle = (MonthlyPrintStyle)printStyle;
			cbLayout.EditValue = monthlyPrintStyle.Layout;
			chkPrintExactlyOneMonth.Checked = monthlyPrintStyle.OneMonthPerPage;
			chkDontPrintWeekends.Checked = !monthlyPrintStyle.PrintWeekends;
			chkCompressWeekend.Checked = monthlyPrintStyle.CompressWeekend;
			printRange.SetRange(monthlyPrintStyle.StartRangeDate, monthlyPrintStyle.EndRangeDate);
		}
		protected internal virtual void DateRangeChanged(object sender, System.EventArgs e) {
			PrintStyle.StartRangeDate = printRange.StartDate;
			PrintStyle.EndRangeDate = printRange.EndDate;
			OnPrintStyleChanged();
		}
		protected internal virtual void DontPrintWeekendsCheckedChanged(object sender, System.EventArgs e) {
			PrintStyle.PrintWeekends = !chkDontPrintWeekends.Checked;
			OnPrintStyleChanged();
		}
		protected internal virtual void PrintExactlyOneMonthCheckedChanged(object sender, System.EventArgs e) {
			PrintStyle.OneMonthPerPage = chkPrintExactlyOneMonth.Checked;
			OnPrintStyleChanged();
		}
		protected internal virtual void LayoutChanged(object sender, System.EventArgs e) {
			PrintStyle.Layout = (PageLayout)cbLayout.EditValue;
			OnPrintStyleChanged();
		}
		protected internal virtual void CompressWeekendCheckedChanged(object sender, System.EventArgs e) {
			PrintStyle.CompressWeekend = chkCompressWeekend.Checked;
			OnPrintStyleChanged();
		}
		protected internal override SchedulerPrintStyle CreateDefaultPrintStyle() {
			return new MonthlyPrintStyle();
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
