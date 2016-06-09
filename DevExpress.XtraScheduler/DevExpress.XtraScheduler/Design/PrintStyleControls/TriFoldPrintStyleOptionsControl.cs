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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraScheduler.Localization;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PrintStyleControls.TriFoldPrintStyleOptionsControl.lblLeftSection")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PrintStyleControls.TriFoldPrintStyleOptionsControl.lblMiddleSection")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PrintStyleControls.TriFoldPrintStyleOptionsControl.lblRightSection")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PrintStyleControls.TriFoldPrintStyleOptionsControl.printRange")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PrintStyleControls.TriFoldPrintStyleOptionsControl.cbLeftSection")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PrintStyleControls.TriFoldPrintStyleOptionsControl.cbMiddleSection")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PrintStyleControls.TriFoldPrintStyleOptionsControl.cbRightSection")]
#endregion
namespace DevExpress.XtraScheduler.Design.PrintStyleControls {
	[DXToolboxItem(false), System.Runtime.InteropServices.ComVisible(false)]
	public class TriFoldPrintStyleOptionsControl : PrintStyleOptionsControlBase {
		protected DevExpress.XtraEditors.LabelControl lblLeftSection;
		protected DevExpress.XtraEditors.LabelControl lblMiddleSection;
		protected DevExpress.XtraEditors.LabelControl lblRightSection;
		protected PrintRangeControl printRange;
		protected ImageComboBoxEdit cbLeftSection;
		protected ImageComboBoxEdit cbMiddleSection;
		protected ImageComboBoxEdit cbRightSection;
		IContainer components = null;
		public TriFoldPrintStyleOptionsControl() {
			InitializeComponent();
			FillSectionList(cbLeftSection);
			FillSectionList(cbRightSection);
			FillSectionList(cbMiddleSection);
		}
		protected internal new TriFoldPrintStyle PrintStyle { get { return (TriFoldPrintStyle)base.PrintStyle; } }
		protected internal virtual void FillSectionList(ImageComboBoxEdit cbSection) {
			string dailyCalendar = SchedulerLocalizer.GetString(SchedulerStringId.PrintTriFoldOptControlDailyCalendar);
			string weeklyCalendar = SchedulerLocalizer.GetString(SchedulerStringId.PrintTriFoldOptControlWeeklyCalendar);
			string monthlyCalendar = SchedulerLocalizer.GetString(SchedulerStringId.PrintTriFoldOptControlMonthlyCalendar);
			cbSection.Properties.Items.AddRange(new ImageComboBoxItem[] {
																			new ImageComboBoxItem(dailyCalendar, PrintStyleSectionKind.DailyCalendar),
																			new ImageComboBoxItem(weeklyCalendar, PrintStyleSectionKind.WeeklyCalendar),
																			new ImageComboBoxItem(monthlyCalendar, PrintStyleSectionKind.MonthlyCalendar)
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TriFoldPrintStyleOptionsControl));
			this.lblLeftSection = new DevExpress.XtraEditors.LabelControl();
			this.lblMiddleSection = new DevExpress.XtraEditors.LabelControl();
			this.lblRightSection = new DevExpress.XtraEditors.LabelControl();
			this.printRange = new DevExpress.XtraScheduler.Design.PrintRangeControl();
			this.cbLeftSection = new DevExpress.XtraEditors.ImageComboBoxEdit();
			this.cbMiddleSection = new DevExpress.XtraEditors.ImageComboBoxEdit();
			this.cbRightSection = new DevExpress.XtraEditors.ImageComboBoxEdit();
			((System.ComponentModel.ISupportInitialize)(this.cbLeftSection.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbMiddleSection.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbRightSection.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.lblLeftSection, "lblLeftSection");
			this.lblLeftSection.Name = "lblLeftSection";
			resources.ApplyResources(this.lblMiddleSection, "lblMiddleSection");
			this.lblMiddleSection.Name = "lblMiddleSection";
			resources.ApplyResources(this.lblRightSection, "lblRightSection");
			this.lblRightSection.Name = "lblRightSection";
			this.printRange.Appearance.BackColor = System.Drawing.Color.Transparent;
			this.printRange.Appearance.Options.UseBackColor = true;
			resources.ApplyResources(this.printRange, "printRange");
			this.printRange.Name = "printRange";
			this.printRange.DateRangeChanged += new System.EventHandler(this.DateRangeChanged);
			resources.ApplyResources(this.cbLeftSection, "cbLeftSection");
			this.cbLeftSection.Name = "cbLeftSection";
			this.cbLeftSection.Properties.AccessibleName = resources.GetString("cbLeftSection.Properties.AccessibleName");
			this.cbLeftSection.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.ComboBox;
			this.cbLeftSection.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbLeftSection.Properties.Buttons"))))});
			this.cbLeftSection.EditValueChanged += new System.EventHandler(this.SectionChanged);
			resources.ApplyResources(this.cbMiddleSection, "cbMiddleSection");
			this.cbMiddleSection.Name = "cbMiddleSection";
			this.cbMiddleSection.Properties.AccessibleName = resources.GetString("cbMiddleSection.Properties.AccessibleName");
			this.cbMiddleSection.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.ComboBox;
			this.cbMiddleSection.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbMiddleSection.Properties.Buttons"))))});
			this.cbMiddleSection.EditValueChanged += new System.EventHandler(this.SectionChanged);
			resources.ApplyResources(this.cbRightSection, "cbRightSection");
			this.cbRightSection.Name = "cbRightSection";
			this.cbRightSection.Properties.AccessibleName = resources.GetString("cbRightSection.Properties.AccessibleName");
			this.cbRightSection.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.ComboBox;
			this.cbRightSection.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbRightSection.Properties.Buttons"))))});
			this.cbRightSection.EditValueChanged += new System.EventHandler(this.SectionChanged);
			this.Appearance.BackColor = System.Drawing.Color.Transparent;
			this.Appearance.Options.UseBackColor = true;
			this.Controls.Add(this.cbRightSection);
			this.Controls.Add(this.cbMiddleSection);
			this.Controls.Add(this.cbLeftSection);
			this.Controls.Add(this.printRange);
			this.Controls.Add(this.lblRightSection);
			this.Controls.Add(this.lblMiddleSection);
			this.Controls.Add(this.lblLeftSection);
			this.Name = "TriFoldPrintStyleOptionsControl";
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			((System.ComponentModel.ISupportInitialize)(this.cbLeftSection.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbMiddleSection.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbRightSection.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected internal override bool IsValidPrintStyle(SchedulerPrintStyle style) {
			return style is TriFoldPrintStyle;
		}
		protected internal override void SubscribeEvents() {
			base.SubscribeEvents();
			printRange.DateRangeChanged += new EventHandler(DateRangeChanged);
			cbLeftSection.EditValueChanged += new EventHandler(SectionChanged);
			cbRightSection.EditValueChanged += new EventHandler(SectionChanged);
			cbMiddleSection.EditValueChanged += new EventHandler(SectionChanged);
		}
		protected internal override void UnsubscribeEvents() {
			base.UnsubscribeEvents();
			printRange.DateRangeChanged -= new EventHandler(DateRangeChanged);
			cbLeftSection.EditValueChanged -= new EventHandler(SectionChanged);
			cbRightSection.EditValueChanged -= new EventHandler(SectionChanged);
			cbMiddleSection.EditValueChanged -= new EventHandler(SectionChanged);
		}
		protected internal override void RefreshDataCore(SchedulerPrintStyle printStyle) {
			base.RefreshDataCore(printStyle);
			TriFoldPrintStyle triFldPrintStyle = (TriFoldPrintStyle)printStyle;
			cbLeftSection.EditValue = triFldPrintStyle.LeftSection;
			cbMiddleSection.EditValue = triFldPrintStyle.MiddleSection;
			cbRightSection.EditValue = triFldPrintStyle.RightSection;
			printRange.SetRange(triFldPrintStyle.StartRangeDate, triFldPrintStyle.EndRangeDate);
		}
		protected internal virtual void DateRangeChanged(object sender, System.EventArgs e) {
			PrintStyle.StartRangeDate = printRange.StartDate;
			PrintStyle.EndRangeDate = printRange.EndDate;
			OnPrintStyleChanged();
		}
		protected internal virtual void SectionChanged(object sender, System.EventArgs e) {
			PrintStyle.LeftSection = (PrintStyleSectionKind)cbLeftSection.EditValue;
			PrintStyle.RightSection = (PrintStyleSectionKind)cbRightSection.EditValue;
			PrintStyle.MiddleSection = (PrintStyleSectionKind)cbMiddleSection.EditValue;
			OnPrintStyleChanged();
		}
		protected internal override SchedulerPrintStyle CreateDefaultPrintStyle() {
			return new TriFoldPrintStyle();
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
