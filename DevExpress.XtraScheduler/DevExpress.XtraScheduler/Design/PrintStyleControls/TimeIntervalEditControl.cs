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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.UI;
using DevExpress.XtraScheduler.Internal.Diagnostics;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PrintStyleControls.TimeOfDayIntervalEditControl.lblDuration")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PrintStyleControls.TimeOfDayIntervalEditControl.lblPrintTo")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PrintStyleControls.TimeOfDayIntervalEditControl.lblPrintFrom")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PrintStyleControls.TimeOfDayIntervalEditControl.cbDuration")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PrintStyleControls.TimeOfDayIntervalEditControl.edtEnd")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Design.PrintStyleControls.TimeOfDayIntervalEditControl.edtStart")]
#endregion
namespace DevExpress.XtraScheduler.Design.PrintStyleControls {
	#region TimeOfDayIntervalEditControl
	[DXToolboxItem(false), System.Runtime.InteropServices.ComVisible(false)]
	public class TimeOfDayIntervalEditControl : DevExpress.XtraEditors.XtraUserControl, IBatchUpdateable, IBatchUpdateHandler {
		#region Fields
		protected DevExpress.XtraEditors.LabelControl lblDuration;
		protected DevExpress.XtraEditors.LabelControl lblPrintTo;
		protected DevExpress.XtraEditors.LabelControl lblPrintFrom;
		protected DurationEdit cbDuration;
		IContainer components = null;
		protected SchedulerTimeEdit edtEnd;
		protected SchedulerTimeEdit edtStart;
		BatchUpdateHelper batchUpdateHelper;
		bool deferredRaiseChanged;
		#endregion
		public TimeOfDayIntervalEditControl() {
			InitializeComponent();
			batchUpdateHelper = new BatchUpdateHelper(this);
			cbDuration.LoadDefaults(DateTimeHelper.DaySpan);
			cbDuration.SelectedIndex = 0;
			Interval = new TimeOfDayInterval(TimeSpan.Zero, DateTimeHelper.DaySpan);
		}
		#region Properties
		public TimeOfDayInterval Interval {
			get {
				TimeSpan span = cbDuration.Duration;
				return new TimeOfDayInterval(edtStart.Time.TimeOfDay, edtStart.Time.TimeOfDay + span);
			}
			set {
				if (value.Duration > DateTimeHelper.DaySpan || value.Duration.Ticks <= 0)
					Exceptions.ThrowArgumentException("value", value);
				if (value.Start >= DateTimeHelper.DaySpan)
					Exceptions.ThrowArgumentException("value", value);
				if (Interval.IsEqual(value))
					return;
				UnsubscribeEvents();
				edtStart.Time = DateTimeHelper.ToDateTime(value.Start);
				edtEnd.Time = new DateTime((value.Start + value.Duration).Ticks % DateTimeHelper.DaySpan.Ticks);
				cbDuration.Duration = value.Duration;
				SubscribeEvents();
				OnIntervalChanged();
			}
		}
		#endregion
		protected internal virtual void SubscribeEvents() {
			cbDuration.InvalidValue += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(DurationInvalidValue);
			cbDuration.Validating += new System.ComponentModel.CancelEventHandler(DurationValidating);
			cbDuration.Validated += new System.EventHandler(DurationValidated);
			edtEnd.EditValueChanged += new System.EventHandler(EndEditValueChanged);
			edtStart.EditValueChanged += new System.EventHandler(StartEditValueChanged);
		}
		protected internal virtual void UnsubscribeEvents() {
			cbDuration.InvalidValue -= new InvalidValueExceptionEventHandler(DurationInvalidValue);
			cbDuration.Validating -= new System.ComponentModel.CancelEventHandler(DurationValidating);
			cbDuration.Validated -= new System.EventHandler(DurationValidated);
			edtEnd.EditValueChanged -= new System.EventHandler(EndEditValueChanged);
			edtStart.EditValueChanged -= new System.EventHandler(StartEditValueChanged);
		}
		#region IntervalChanged
		internal static readonly object onIntervalChanged = new object();
		public event EventHandler IntervalChanged {
			add { Events.AddHandler(onIntervalChanged, value); }
			remove { Events.RemoveHandler(onIntervalChanged, value); }
		}
		protected internal virtual void RaiseIntervalChangedEvent() {
			EventHandler handler = (EventHandler)Events[onIntervalChanged];
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
		#endregion
		protected internal virtual void OnIntervalChanged() {
			if (IsUpdateLocked)
				deferredRaiseChanged = true;
			else
				RaiseIntervalChangedEvent();
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TimeOfDayIntervalEditControl));
			this.lblDuration = new DevExpress.XtraEditors.LabelControl();
			this.edtEnd = new DevExpress.XtraScheduler.UI.SchedulerTimeEdit();
			this.edtStart = new DevExpress.XtraScheduler.UI.SchedulerTimeEdit();
			this.lblPrintTo = new DevExpress.XtraEditors.LabelControl();
			this.lblPrintFrom = new DevExpress.XtraEditors.LabelControl();
			this.cbDuration = new DevExpress.XtraScheduler.UI.DurationEdit();
			((System.ComponentModel.ISupportInitialize)(this.edtEnd.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtStart.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbDuration.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.lblDuration, "lblDuration");
			this.lblDuration.Name = "lblDuration";
			resources.ApplyResources(this.edtEnd, "edtEnd");
			this.edtEnd.Name = "edtEnd";
			this.edtEnd.Properties.EditValueChangedFiringMode = EditValueChangedFiringMode.Default;
			this.edtEnd.Properties.AccessibleName = resources.GetString("edtEnd.Properties.AccessibleName");
			this.edtEnd.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.edtEnd.EditValueChanged += new System.EventHandler(this.EndEditValueChanged);
			resources.ApplyResources(this.edtStart, "edtStart");
			this.edtStart.Name = "edtStart";
			this.edtStart.Properties.EditValueChangedFiringMode = EditValueChangedFiringMode.Default;
			this.edtStart.Properties.AccessibleName = resources.GetString("edtStart.Properties.AccessibleName");
			this.edtStart.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.edtStart.EditValueChanged += new System.EventHandler(this.StartEditValueChanged);
			resources.ApplyResources(this.lblPrintTo, "lblPrintTo");
			this.lblPrintTo.Name = "lblPrintTo";
			resources.ApplyResources(this.lblPrintFrom, "lblPrintFrom");
			this.lblPrintFrom.Name = "lblPrintFrom";
			resources.ApplyResources(this.cbDuration, "cbDuration");
			this.cbDuration.Name = "cbDuration";
			this.cbDuration.Properties.AccessibleName = resources.GetString("cbDuration.Properties.AccessibleName");
			this.cbDuration.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbDuration.Properties.Buttons"))))});
			this.cbDuration.InvalidValue += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.DurationInvalidValue);
			this.cbDuration.Validating += new System.ComponentModel.CancelEventHandler(this.DurationValidating);
			this.cbDuration.Validated += new System.EventHandler(this.DurationValidated);
			this.Appearance.BackColor = System.Drawing.Color.Transparent;
			this.Appearance.Options.UseBackColor = true;
			this.Controls.Add(this.cbDuration);
			this.Controls.Add(this.lblDuration);
			this.Controls.Add(this.edtEnd);
			this.Controls.Add(this.edtStart);
			this.Controls.Add(this.lblPrintTo);
			this.Controls.Add(this.lblPrintFrom);
			this.Name = "TimeOfDayIntervalEditControl";
			resources.ApplyResources(this, "$this");
			((System.ComponentModel.ISupportInitialize)(this.edtEnd.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtStart.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbDuration.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected internal virtual void DurationValidated(object sender, System.EventArgs e) {
			TimeSpan duration = cbDuration.Duration;
			XtraSchedulerDebug.Assert(duration <= DateTimeHelper.DaySpan);
			UnsubscribeEvents();
			DateTime newEnd = edtStart.Time + duration;
			edtEnd.Time = DateTimeHelper.ToDateTime(newEnd.TimeOfDay);
			SubscribeEvents();
			OnIntervalChanged();
		}
		protected internal virtual void DurationValidating(object sender, CancelEventArgs e) {
			TimeSpan span = cbDuration.Duration;
			e.Cancel = span.Ticks <= 0 || span > DateTimeHelper.DaySpan;
		}
		protected internal virtual void DurationInvalidValue(object sender, InvalidValueExceptionEventArgs e) {
			e.ErrorText = SchedulerLocalizer.GetString(SchedulerStringId.PrintTimeIntervalControlInvalidDuration);
		}
		#region IBatchUpdateable implementation
		public void BeginUpdate() {
			batchUpdateHelper.BeginUpdate();
		}
		public void EndUpdate() {
			batchUpdateHelper.EndUpdate();
		}
		public void CancelUpdate() {
			batchUpdateHelper.CancelUpdate();
		}
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper { get { return batchUpdateHelper; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsUpdateLocked { get { return batchUpdateHelper.IsUpdateLocked; } }
		#endregion
		#region IBatchUpdateHandler implementation
		void IBatchUpdateHandler.OnFirstBeginUpdate() {
			deferredRaiseChanged = false;
		}
		void IBatchUpdateHandler.OnBeginUpdate() {
		}
		void IBatchUpdateHandler.OnEndUpdate() {
		}
		void IBatchUpdateHandler.OnLastEndUpdate() {
			if (deferredRaiseChanged)
				RaiseIntervalChangedEvent();
		}
		void IBatchUpdateHandler.OnCancelUpdate() {
		}
		void IBatchUpdateHandler.OnLastCancelUpdate() {
		}
		#endregion
		protected internal virtual void StartEditValueChanged(object sender, EventArgs e) {
			AdjustDuration();
		}
		protected internal virtual void EndEditValueChanged(object sender, EventArgs e) {
			AdjustDuration();
		}
		protected internal virtual void AdjustDuration() {
			UnsubscribeEvents();
			TimeSpan duration = edtEnd.Time.TimeOfDay - edtStart.Time.TimeOfDay;
			if (duration.Ticks <= 0)
				duration += DateTimeHelper.DaySpan;
			cbDuration.Duration = duration;
			SubscribeEvents();
			OnIntervalChanged();
		}
	}
	#endregion
}
