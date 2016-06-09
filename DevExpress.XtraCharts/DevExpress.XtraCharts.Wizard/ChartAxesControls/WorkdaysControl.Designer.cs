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

namespace DevExpress.XtraCharts.Wizard.ChartAxesControls
{
	partial class WorkdaysControl
	{
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WorkdaysControl));
			this.panelWorkdaysPadding = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.labelLine = new DevExpress.XtraEditors.LabelControl();
			this.ceWorkdaysOnly = new DevExpress.XtraEditors.CheckEdit();
			this.grWorkdays = new DevExpress.XtraEditors.GroupControl();
			this.ceSaturday = new DevExpress.XtraEditors.CheckEdit();
			this.ceFriday = new DevExpress.XtraEditors.CheckEdit();
			this.ceThursday = new DevExpress.XtraEditors.CheckEdit();
			this.ceWednesday = new DevExpress.XtraEditors.CheckEdit();
			this.ceTuesday = new DevExpress.XtraEditors.CheckEdit();
			this.ceMonday = new DevExpress.XtraEditors.CheckEdit();
			this.ceSunday = new DevExpress.XtraEditors.CheckEdit();
			this.panelKnownDaysPadding = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.grKnownDays = new DevExpress.XtraEditors.GroupControl();
			this.panelExactWorkdays = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.btnExactWorkdays = new DevExpress.XtraEditors.ButtonEdit();
			this.labelExactWorkdays = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.panelHolidaysSplitter = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.panelHolidays = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.btnHolidays = new DevExpress.XtraEditors.ButtonEdit();
			this.labelHolidays = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			((System.ComponentModel.ISupportInitialize)(this.panelWorkdaysPadding)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ceWorkdaysOnly.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grWorkdays)).BeginInit();
			this.grWorkdays.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ceSaturday.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ceFriday.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ceThursday.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ceWednesday.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ceTuesday.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ceMonday.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ceSunday.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelKnownDaysPadding)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grKnownDays)).BeginInit();
			this.grKnownDays.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelExactWorkdays)).BeginInit();
			this.panelExactWorkdays.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.btnExactWorkdays.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelHolidaysSplitter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelHolidays)).BeginInit();
			this.panelHolidays.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.btnHolidays.Properties)).BeginInit();
			this.SuspendLayout();
			this.panelWorkdaysPadding.BackColor = System.Drawing.Color.Transparent;
			this.panelWorkdaysPadding.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelWorkdaysPadding, "panelWorkdaysPadding");
			this.panelWorkdaysPadding.Name = "panelWorkdaysPadding";
			resources.ApplyResources(this.labelLine, "labelLine");
			this.labelLine.LineVisible = true;
			this.labelLine.Name = "labelLine";
			resources.ApplyResources(this.ceWorkdaysOnly, "ceWorkdaysOnly");
			this.ceWorkdaysOnly.Name = "ceWorkdaysOnly";
			this.ceWorkdaysOnly.Properties.Caption = resources.GetString("ceWorkdaysOnly.Properties.Caption");
			this.ceWorkdaysOnly.CheckedChanged += new System.EventHandler(this.ceWorkdaysOnly_CheckedChanged);
			resources.ApplyResources(this.grWorkdays, "grWorkdays");
			this.grWorkdays.Controls.Add(this.ceSaturday);
			this.grWorkdays.Controls.Add(this.ceFriday);
			this.grWorkdays.Controls.Add(this.ceThursday);
			this.grWorkdays.Controls.Add(this.ceWednesday);
			this.grWorkdays.Controls.Add(this.ceTuesday);
			this.grWorkdays.Controls.Add(this.ceMonday);
			this.grWorkdays.Controls.Add(this.ceSunday);
			this.grWorkdays.Name = "grWorkdays";
			resources.ApplyResources(this.ceSaturday, "ceSaturday");
			this.ceSaturday.Name = "ceSaturday";
			this.ceSaturday.Properties.Caption = resources.GetString("ceSaturday.Properties.Caption");
			this.ceSaturday.CheckedChanged += new System.EventHandler(this.ceSaturday_CheckedChanged);
			resources.ApplyResources(this.ceFriday, "ceFriday");
			this.ceFriday.Name = "ceFriday";
			this.ceFriday.Properties.Caption = resources.GetString("ceFriday.Properties.Caption");
			this.ceFriday.CheckedChanged += new System.EventHandler(this.ceFriday_CheckedChanged);
			resources.ApplyResources(this.ceThursday, "ceThursday");
			this.ceThursday.Name = "ceThursday";
			this.ceThursday.Properties.Caption = resources.GetString("ceThursday.Properties.Caption");
			this.ceThursday.CheckedChanged += new System.EventHandler(this.ceThursday_CheckedChanged);
			resources.ApplyResources(this.ceWednesday, "ceWednesday");
			this.ceWednesday.Name = "ceWednesday";
			this.ceWednesday.Properties.Caption = resources.GetString("ceWednesday.Properties.Caption");
			this.ceWednesday.CheckedChanged += new System.EventHandler(this.ceWednesday_CheckedChanged);
			resources.ApplyResources(this.ceTuesday, "ceTuesday");
			this.ceTuesday.Name = "ceTuesday";
			this.ceTuesday.Properties.Caption = resources.GetString("ceTuesday.Properties.Caption");
			this.ceTuesday.CheckedChanged += new System.EventHandler(this.ceTuesday_CheckedChanged);
			resources.ApplyResources(this.ceMonday, "ceMonday");
			this.ceMonday.Name = "ceMonday";
			this.ceMonday.Properties.Caption = resources.GetString("ceMonday.Properties.Caption");
			this.ceMonday.CheckedChanged += new System.EventHandler(this.ceMonday_CheckedChanged);
			resources.ApplyResources(this.ceSunday, "ceSunday");
			this.ceSunday.Name = "ceSunday";
			this.ceSunday.Properties.Caption = resources.GetString("ceSunday.Properties.Caption");
			this.ceSunday.CheckedChanged += new System.EventHandler(this.ceSunday_CheckedChanged);
			this.panelKnownDaysPadding.BackColor = System.Drawing.Color.Transparent;
			this.panelKnownDaysPadding.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelKnownDaysPadding, "panelKnownDaysPadding");
			this.panelKnownDaysPadding.Name = "panelKnownDaysPadding";
			resources.ApplyResources(this.grKnownDays, "grKnownDays");
			this.grKnownDays.Controls.Add(this.panelExactWorkdays);
			this.grKnownDays.Controls.Add(this.panelHolidaysSplitter);
			this.grKnownDays.Controls.Add(this.panelHolidays);
			this.grKnownDays.Name = "grKnownDays";
			resources.ApplyResources(this.panelExactWorkdays, "panelExactWorkdays");
			this.panelExactWorkdays.BackColor = System.Drawing.Color.Transparent;
			this.panelExactWorkdays.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelExactWorkdays.Controls.Add(this.btnExactWorkdays);
			this.panelExactWorkdays.Controls.Add(this.labelExactWorkdays);
			this.panelExactWorkdays.Name = "panelExactWorkdays";
			resources.ApplyResources(this.btnExactWorkdays, "btnExactWorkdays");
			this.btnExactWorkdays.Name = "btnExactWorkdays";
			this.btnExactWorkdays.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.btnExactWorkdays.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.btnExactWorkdays.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btnExactWorkdays_ButtonClick);
			resources.ApplyResources(this.labelExactWorkdays, "labelExactWorkdays");
			this.labelExactWorkdays.Name = "labelExactWorkdays";
			this.panelHolidaysSplitter.BackColor = System.Drawing.Color.Transparent;
			this.panelHolidaysSplitter.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelHolidaysSplitter, "panelHolidaysSplitter");
			this.panelHolidaysSplitter.Name = "panelHolidaysSplitter";
			resources.ApplyResources(this.panelHolidays, "panelHolidays");
			this.panelHolidays.BackColor = System.Drawing.Color.Transparent;
			this.panelHolidays.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelHolidays.Controls.Add(this.btnHolidays);
			this.panelHolidays.Controls.Add(this.labelHolidays);
			this.panelHolidays.Name = "panelHolidays";
			resources.ApplyResources(this.btnHolidays, "btnHolidays");
			this.btnHolidays.Name = "btnHolidays";
			this.btnHolidays.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.btnHolidays.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.btnHolidays.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btnHolidays_ButtonClick);
			resources.ApplyResources(this.labelHolidays, "labelHolidays");
			this.labelHolidays.Name = "labelHolidays";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.grKnownDays);
			this.Controls.Add(this.panelKnownDaysPadding);
			this.Controls.Add(this.grWorkdays);
			this.Controls.Add(this.panelWorkdaysPadding);
			this.Controls.Add(this.labelLine);
			this.Controls.Add(this.ceWorkdaysOnly);
			this.Name = "WorkdaysControl";
			((System.ComponentModel.ISupportInitialize)(this.panelWorkdaysPadding)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ceWorkdaysOnly.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grWorkdays)).EndInit();
			this.grWorkdays.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.ceSaturday.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ceFriday.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ceThursday.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ceWednesday.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ceTuesday.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ceMonday.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ceSunday.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelKnownDaysPadding)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grKnownDays)).EndInit();
			this.grKnownDays.ResumeLayout(false);
			this.grKnownDays.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelExactWorkdays)).EndInit();
			this.panelExactWorkdays.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.btnExactWorkdays.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelHolidaysSplitter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelHolidays)).EndInit();
			this.panelHolidays.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.btnHolidays.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraCharts.Wizard.ChartPanelControl panelWorkdaysPadding;
		private DevExpress.XtraEditors.LabelControl labelLine;
		private DevExpress.XtraEditors.CheckEdit ceWorkdaysOnly;
		private DevExpress.XtraEditors.GroupControl grWorkdays;
		private DevExpress.XtraEditors.CheckEdit ceSaturday;
		private DevExpress.XtraEditors.CheckEdit ceFriday;
		private DevExpress.XtraEditors.CheckEdit ceThursday;
		private DevExpress.XtraEditors.CheckEdit ceWednesday;
		private DevExpress.XtraEditors.CheckEdit ceTuesday;
		private DevExpress.XtraEditors.CheckEdit ceMonday;
		private DevExpress.XtraEditors.CheckEdit ceSunday;
		private ChartPanelControl panelKnownDaysPadding;
		private DevExpress.XtraEditors.GroupControl grKnownDays;
		private ChartPanelControl panelExactWorkdays;
		private DevExpress.XtraEditors.ButtonEdit btnExactWorkdays;
		private ChartLabelControl labelExactWorkdays;
		private ChartPanelControl panelHolidaysSplitter;
		private ChartPanelControl panelHolidays;
		private DevExpress.XtraEditors.ButtonEdit btnHolidays;
		private ChartLabelControl labelHolidays;
	}
}
