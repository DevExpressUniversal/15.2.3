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
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Internal;
namespace DevExpress.XtraScheduler.UI {
	#region RecurrenceControlBase
	[
	DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabScheduling),
	ToolboxBitmap(typeof(SchedulerControl), DevExpress.Utils.ControlConstants.BitmapPath + "recurrenceControlBase.bmp"),
	System.Runtime.InteropServices.ComVisible(false),
	Designer("DevExpress.XtraScheduler.Design.XtraSchedulerSuiteControlDesigner," + AssemblyInfo.SRAssemblySchedulerDesign),
	Description("A base class for recurrence controls available in the XtraScheduler library.")
	]
	[Docking(DockingBehavior.Ask)]
	public class RecurrenceControlBase : DevExpress.XtraEditors.XtraUserControl, IBatchUpdateable, IBatchUpdateHandler
	{
		BatchUpdateHelper batchUpdateHelper;
		IRecurrenceInfo rinfo = new RecurrenceInfo();
		SchedulerRecurrenceValidator validator;
		public RecurrenceControlBase()
		{
			this.batchUpdateHelper = new BatchUpdateHelper(this);
			this.validator = CreateRecurrenceValidator();
			InitRecurrenceInfo();
			InitializeComponent();
			SubscribeRecurrenceInfoEvents();
		}
		protected virtual SchedulerRecurrenceValidator CreateRecurrenceValidator() {
			return new SchedulerRecurrenceValidator();
		}
		#region Properties
		protected internal SchedulerRecurrenceValidator Validator { get { return validator; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IRecurrenceInfo RecurrenceInfo {
			get { return rinfo; }
			set {
				if (rinfo == value)
					return;
				UnsubscribeRecurrenceInfoEvents();
				if (value == null) {
					rinfo = new RecurrenceInfo();
					InitRecurrenceInfo();
				}
				else
					rinfo = value;
				SubscribeRecurrenceInfoEvents();
				UpdateControls();
				RaiseRecurrenceInfoChanged();
			}
		}
		#endregion
		#region Events
		#region RecurrenceInfoChanged
		static readonly object recurrenceInfoChanged = new object();
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("RecurrenceControlBaseRecurrenceInfoChanged")]
#endif
		public event EventHandler RecurrenceInfoChanged {
			add { Events.AddHandler(recurrenceInfoChanged, value); } 
			remove { Events.RemoveHandler(recurrenceInfoChanged, value); }
		}
		protected internal virtual void RaiseRecurrenceInfoChanged() {
			EventHandler handler = (EventHandler)this.Events[recurrenceInfoChanged];
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
		#endregion
		#endregion
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
			UnsubscribeControlsEvents();
			UnsubscribeRecurrenceInfoEvents();
		}
		void IBatchUpdateHandler.OnBeginUpdate() {
		}
		void IBatchUpdateHandler.OnEndUpdate() {
		}
		void IBatchUpdateHandler.OnLastEndUpdate() {
			SubscribeRecurrenceInfoEvents();
			RaiseRecurrenceInfoChanged();
			SubscribeControlsEvents();
		}
		void IBatchUpdateHandler.OnCancelUpdate() {
		}
		void IBatchUpdateHandler.OnLastCancelUpdate() {
			SubscribeControlsEvents();
			SubscribeRecurrenceInfoEvents();
		}
		#endregion
		protected internal virtual void InitRecurrenceInfo() {
		}
		protected internal virtual void SubscribeControlsEvents() {
		}
		protected internal virtual void UnsubscribeControlsEvents() {
		}
		protected internal virtual void SubscribeRecurrenceInfoEvents() {
			((IInternalRecurrenceInfo)rinfo).Changed += new EventHandler(OnRecurrenceInfoChanged);
		}
		protected internal virtual void UnsubscribeRecurrenceInfoEvents() {
			((IInternalRecurrenceInfo)rinfo).Changed -= new EventHandler(OnRecurrenceInfoChanged);
		}
		protected internal virtual void OnRecurrenceInfoChanged(object sender, EventArgs e) {
			UpdateControls();
			RaiseRecurrenceInfoChanged();
		}
		protected internal virtual WeekOfMonth CalcWeekOfMonth() {
			int count = 0;
			DateTime date = new DateTime(RecurrenceInfo.Start.Year, RecurrenceInfo.Start.Month, 1);
			DayOfWeek dayOfWeek = DateTimeHelper.ToDayOfWeek(RecurrenceInfo.WeekDays);
			while (date <= RecurrenceInfo.Start) {
				if (date.DayOfWeek == dayOfWeek)
					count++;
				date = date.AddDays(1);
			}
			if (count >= 1 && count <= 4)
				return (WeekOfMonth)count;
			else if (count >= 5)
				return WeekOfMonth.Last;
			else
				return WeekOfMonth.First;
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (rinfo != null) {
					UnsubscribeRecurrenceInfoEvents();
					rinfo = null;
				}
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent()
		{
			this.Appearance.BackColor = System.Drawing.Color.Transparent;
			this.Appearance.Options.UseBackColor = true;
			this.Name = "RecurrenceControlBase";
			this.Size = new System.Drawing.Size(388, 96);
		}
		#endregion
		 protected internal virtual void ValidateLargeDayNumber(ValidationArgs args, TextEdit edit) {
			 Validator.CheckLargeDayNumberWarning(args, edit, edit.EditValue);
		}
		public virtual void UpdateControls() {
			BeginUpdate();
			try {
				UpdateControlsCore();
			}
			finally {
				CancelUpdate();
			}
		}
		protected internal virtual void UpdateControlsCore() {
		}
		public virtual void ValidateValues(ValidationArgs args) {
		}
		public virtual void CheckForWarnings(ValidationArgs args) {
		}
	}
	#endregion
}
