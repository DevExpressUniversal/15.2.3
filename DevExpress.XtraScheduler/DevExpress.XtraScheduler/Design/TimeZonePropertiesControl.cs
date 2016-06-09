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
using System.Globalization;
using System.Windows.Forms;
using DevExpress.XtraScheduler.Native;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.TimeZonePropertiesControl.lblTimeZoneLabel")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.TimeZonePropertiesControl.lblTimeZone")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.TimeZonePropertiesControl.cbTimeZones")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.TimeZonePropertiesControl.tbLabel")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.TimeZonePropertiesControl.chkAdjustForDaylightChanges")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.TimeZonePropertiesControl.lblCurrentTime")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.TimeZonePropertiesControl.tbCurrentTime")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.TimeZonePropertiesControl.timer")]
#endregion
namespace DevExpress.XtraScheduler.UI {
	[
	DXToolboxItem(false),
	System.Runtime.InteropServices.ComVisible(false)
	]
	public class TimeZonePropertiesControl : DevExpress.XtraEditors.XtraUserControl {
		protected DevExpress.XtraEditors.LabelControl lblTimeZoneLabel;
		protected DevExpress.XtraEditors.LabelControl lblTimeZone;
		protected DevExpress.XtraScheduler.UI.TimeZoneEdit cbTimeZones;
		protected DevExpress.XtraEditors.TextEdit tbLabel;
		protected DevExpress.XtraEditors.CheckEdit chkAdjustForDaylightChanges;
		protected DevExpress.XtraEditors.LabelControl lblCurrentTime;
		protected DevExpress.XtraEditors.TextEdit tbCurrentTime;
		protected System.Windows.Forms.Timer timer;
		TimeRuler timeRuler;
		private System.ComponentModel.IContainer components;
		public TimeZonePropertiesControl() {
			InitializeComponent();
		}
		#region Properties
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public TimeRuler TimeRuler {
			get { return timeRuler; }
			set {
				timeRuler = value;
				Initialize();
			}
		}
		[Browsable(false)]
		public string Caption { get { return tbLabel.Text; } }
		[Browsable(false)]
		public string TimeZoneId { get { return cbTimeZones.TimeZoneId; } }
		[Browsable(false)]
		public bool AdjustForDaylightSavingTime { get { return chkAdjustForDaylightChanges.Checked; } }
		#endregion
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TimeZonePropertiesControl));
			this.lblTimeZoneLabel = new DevExpress.XtraEditors.LabelControl();
			this.tbLabel = new DevExpress.XtraEditors.TextEdit();
			this.lblTimeZone = new DevExpress.XtraEditors.LabelControl();
			this.chkAdjustForDaylightChanges = new DevExpress.XtraEditors.CheckEdit();
			this.lblCurrentTime = new DevExpress.XtraEditors.LabelControl();
			this.tbCurrentTime = new DevExpress.XtraEditors.TextEdit();
			this.timer = new System.Windows.Forms.Timer(this.components);
			this.cbTimeZones = new DevExpress.XtraScheduler.UI.TimeZoneEdit();
			((System.ComponentModel.ISupportInitialize)(this.tbLabel.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkAdjustForDaylightChanges.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tbCurrentTime.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbTimeZones.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.lblTimeZoneLabel, "lblTimeZoneLabel");
			this.lblTimeZoneLabel.Name = "lblTimeZoneLabel";
			resources.ApplyResources(this.tbLabel, "tbLabel");
			this.tbLabel.Name = "tbLabel";
			this.tbLabel.Properties.AccessibleName = resources.GetString("tbLabel.Properties.AccessibleName");
			resources.ApplyResources(this.lblTimeZone, "lblTimeZone");
			this.lblTimeZone.Name = "lblTimeZone";
			resources.ApplyResources(this.chkAdjustForDaylightChanges, "chkAdjustForDaylightChanges");
			this.chkAdjustForDaylightChanges.Name = "chkAdjustForDaylightChanges";
			this.chkAdjustForDaylightChanges.Properties.AccessibleName = resources.GetString("chkAdjustForDaylightChanges.Properties.AccessibleName");
			this.chkAdjustForDaylightChanges.Properties.AutoWidth = true;
			this.chkAdjustForDaylightChanges.Properties.Caption = resources.GetString("chkAdjustForDaylightChanges.Properties.Caption");
			this.chkAdjustForDaylightChanges.CheckedChanged += new System.EventHandler(this.chkAdjustForDaylightChanges_CheckedChanged);
			resources.ApplyResources(this.lblCurrentTime, "lblCurrentTime");
			this.lblCurrentTime.Name = "lblCurrentTime";
			resources.ApplyResources(this.tbCurrentTime, "tbCurrentTime");
			this.tbCurrentTime.Name = "tbCurrentTime";
			this.tbCurrentTime.Properties.AccessibleName = resources.GetString("tbCurrentTime.Properties.AccessibleName");
			this.tbCurrentTime.Properties.ReadOnly = true;
			this.timer.Interval = 60000;
			this.timer.Tick += new System.EventHandler(this.timer_Tick);
			resources.ApplyResources(this.cbTimeZones, "cbTimeZones");
			this.cbTimeZones.Name = "cbTimeZones";
			this.cbTimeZones.Properties.AccessibleName = resources.GetString("cbTimeZones.Properties.AccessibleName");
			this.cbTimeZones.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.ComboBox;
			this.cbTimeZones.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbTimeZones.Properties.Buttons"))))});
			this.cbTimeZones.EditValueChanged += new System.EventHandler(this.OnCbTimeZonesEditValueChanged);
			this.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("TimeZonePropertiesControl.Appearance.BackColor")));
			this.Appearance.Options.UseBackColor = true;
			this.Controls.Add(this.chkAdjustForDaylightChanges);
			this.Controls.Add(this.cbTimeZones);
			this.Controls.Add(this.tbLabel);
			this.Controls.Add(this.lblTimeZoneLabel);
			this.Controls.Add(this.lblTimeZone);
			this.Controls.Add(this.lblCurrentTime);
			this.Controls.Add(this.tbCurrentTime);
			this.Name = "TimeZonePropertiesControl";
			resources.ApplyResources(this, "$this");
			this.Load += new System.EventHandler(this.TimeZonePropertiesControl_Load);
			((System.ComponentModel.ISupportInitialize)(this.tbLabel.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkAdjustForDaylightChanges.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tbCurrentTime.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbTimeZones.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		void OnCbTimeZonesEditValueChanged(object sender, System.EventArgs e) {
			UpdateControls();
		}
		protected virtual void UpdateControls() {
			if (TimeRuler == null)
				return;
			TimeZoneInfo tz = GetSelectedTimeZone();
			if (tz == null)
				return;
			chkAdjustForDaylightChanges.Enabled = tz.SupportsDaylightSavingTime;
			chkAdjustForDaylightChanges.Checked = TimeRuler.AdjustForDaylightSavingTime;
			UpdateCurrentTime();
		}
		protected TimeZoneInfo GetSelectedTimeZone() {
			string tzId = cbTimeZones.TimeZoneId;
			TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(tzId);
			if (tz == null)
				tz = TimeZoneInfo.FindSystemTimeZoneById(TimeRuler.TimeZoneId);
			return tz;
		}
		private void TimeZonePropertiesControl_Load(object sender, System.EventArgs e) {
			Initialize();
			timer.Enabled = true;
		}
		protected virtual void Initialize() {
			if (TimeRuler == null)
				return;
			tbLabel.Text = timeRuler.Caption;
			cbTimeZones.TimeZoneId = timeRuler.TimeZoneId;
			chkAdjustForDaylightChanges.Checked = timeRuler.AdjustForDaylightSavingTime;
			UpdateControls();
		}
		private void chkAdjustForDaylightChanges_CheckedChanged(object sender, System.EventArgs e) {
			UpdateCurrentTime();
		}
		protected void UpdateCurrentTime() {
			if (TimeRuler == null)
				return;
			TimeZoneInfo tz = GetSelectedTimeZone();
			if (tz == null)
				return;
			DateTime now = DateTime.Now;
			DateTime result = TimeZoneInfo.ConvertTime(now, TimeZoneEngine.Local, tz);
			tbCurrentTime.Text = String.Format("{0} {1}", result.ToShortDateString(), DateTimeFormatHelper.DateToShortTimeString(result));
		}
		private void timer_Tick(object sender, System.EventArgs e) {
			UpdateCurrentTime();
		}
	}
}
