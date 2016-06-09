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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.XtraScheduler.UI {
	[
	DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabScheduling),
	ToolboxBitmap(typeof(SchedulerControl), DevExpress.Utils.ControlConstants.BitmapPath + "weekDaysCheckEdit.bmp"),
	System.Runtime.InteropServices.ComVisible(false),
	Designer("DevExpress.XtraScheduler.Design.XtraSchedulerSuiteControlDesigner," + AssemblyInfo.SRAssemblySchedulerDesign),
	Description("A control that allows selecting days of the week by checking the corresponding boxes.")
	]
	[Docking(DockingBehavior.Ask)]
	public class WeekDaysCheckEdit : DevExpress.XtraEditors.XtraUserControl, ISupportInitialize, IBatchUpdateable, IBatchUpdateHandler {
		FirstDayOfWeek firstDayOfWeek = FirstDayOfWeek.System;
		WeekDays weekDays = WeekDays.EveryDay;
		WeekDays visibleWeekDays = WeekDays.EveryDay;
		BatchUpdateHelper batchUpdateHelper;
		bool useAbbreviatedDayNames;
		SizeF factor = new SizeF(1, 1);
		DayOfWeek[] checkWeekDays;
		WeekDaysCheckEditActions deferredChanges;
		public WeekDaysCheckEdit() {
			this.batchUpdateHelper = new BatchUpdateHelper(this);
			InitializeComponent();
			PerformActions(WeekDaysCheckEditActions.UpdateCheckWeekDays | WeekDaysCheckEditActions.CreateCheckControls | WeekDaysCheckEditActions.UpdateCheckControls | WeekDaysCheckEditActions.UpdateCheckControlsState | WeekDaysCheckEditActions.RecalcLayout);
		}
		#region Properties
		#region FirstDayOfWeek
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("WeekDaysCheckEditFirstDayOfWeek"),
#endif
DefaultValue(FirstDayOfWeek.System), Category(SRCategoryNames.Appearance)]
		public FirstDayOfWeek FirstDayOfWeek {
			get { return firstDayOfWeek; }
			set {
				if (firstDayOfWeek == value)
					return;
				firstDayOfWeek = value;
				PerformActions(WeekDaysCheckEditActions.UpdateCheckWeekDays | WeekDaysCheckEditActions.UpdateCheckControls | WeekDaysCheckEditActions.UpdateCheckControlsState | WeekDaysCheckEditActions.RecalcLayout);
			}
		}
		#endregion
		#region WeekDays
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("WeekDaysCheckEditWeekDays"),
#endif
DefaultValue(WeekDays.EveryDay), Category(SRCategoryNames.Appearance)]
		[Editor(typeof(DevExpress.Utils.Editors.AttributesEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public WeekDays WeekDays {
			get { return weekDays; }
			set {
				if (weekDays == value)
					return;
				weekDays = value;
				PerformActions(WeekDaysCheckEditActions.UpdateCheckControlsState | WeekDaysCheckEditActions.RaiseWeekDaysChanged);
			}
		}
		#endregion
		#region VisibleWeekDays
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("WeekDaysCheckEditVisibleWeekDays"),
#endif
DefaultValue(WeekDays.EveryDay), Category(SRCategoryNames.Appearance)]
		[Editor(typeof(DevExpress.Utils.Editors.AttributesEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public WeekDays VisibleWeekDays {
			get { return visibleWeekDays; }
			set {
				if (visibleWeekDays == value)
					return;
				visibleWeekDays = value;
				PerformActions(WeekDaysCheckEditActions.UpdateCheckWeekDays | WeekDaysCheckEditActions.ClearCheckControls | WeekDaysCheckEditActions.CreateCheckControls | WeekDaysCheckEditActions.UpdateCheckControls | WeekDaysCheckEditActions.UpdateCheckControlsState | WeekDaysCheckEditActions.RecalcLayout);
			}
		}
		#endregion
		#region UseAbbreviatedDayNames
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("WeekDaysCheckEditUseAbbreviatedDayNames"),
#endif
DefaultValue(false), Category(SRCategoryNames.Appearance)]
		public bool UseAbbreviatedDayNames {
			get { return useAbbreviatedDayNames; }
			set {
				if (useAbbreviatedDayNames == value)
					return;
				useAbbreviatedDayNames = value;
				PerformActions(WeekDaysCheckEditActions.UpdateCheckControls | WeekDaysCheckEditActions.RecalcLayout);
			}
		}
		#endregion
		protected internal WeekDaysCheckEditActions DeferredChanges { get { return deferredChanges; } set { deferredChanges = value; } }
		protected internal DayOfWeek[] CheckWeekDays { get { return checkWeekDays; } }
		#endregion
		#region Events
		#region WeekDaysChanged
		static readonly object weekDaysChanged = new object();
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("WeekDaysCheckEditWeekDaysChanged")]
#endif
		public event EventHandler WeekDaysChanged {
			add { Events.AddHandler(weekDaysChanged, value); }
			remove { Events.RemoveHandler(weekDaysChanged, value); }
		}
		protected internal virtual void RaiseWeekDaysChanged() {
			EventHandler handler = (EventHandler)this.Events[weekDaysChanged];
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		#region Component Designer generated code
		private void InitializeComponent() {
			this.Appearance.BackColor = System.Drawing.Color.Transparent;
			this.Appearance.Options.UseBackColor = true;
			this.Name = "WeekDaysCheckEdit";
			this.Size = new System.Drawing.Size(424, 112);
		}
		#endregion
		#region ISupportInitialize implementation
		public void BeginInit() {
			BeginUpdate();
		}
		public void EndInit() {
			CancelUpdate();
		}
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
			this.deferredChanges = WeekDaysCheckEditActions.None;
		}
		void IBatchUpdateHandler.OnBeginUpdate() {
		}
		void IBatchUpdateHandler.OnEndUpdate() {
		}
		void IBatchUpdateHandler.OnLastEndUpdate() {
			if (deferredChanges != WeekDaysCheckEditActions.None)
				PerformActions(deferredChanges);
			this.deferredChanges = WeekDaysCheckEditActions.None;
		}
		void IBatchUpdateHandler.OnCancelUpdate() {
		}
		void IBatchUpdateHandler.OnLastCancelUpdate() {
			deferredChanges &= ~WeekDaysCheckEditActions.RaiseWeekDaysChanged;
			if (deferredChanges != WeekDaysCheckEditActions.None)
				PerformActions(deferredChanges);
			this.deferredChanges = WeekDaysCheckEditActions.None;
		}
		#endregion
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			PerformActions(WeekDaysCheckEditActions.RecalcLayout);
		}
		protected override void ScaleControl(SizeF factor, BoundsSpecified specified) {
			base.ScaleControl(factor, specified);
			this.factor = factor;
			PerformActions(WeekDaysCheckEditActions.RecalcLayout);
		}
		protected internal virtual bool IsActionQueued(WeekDaysCheckEditActions actions, WeekDaysCheckEditActions action) {
			return (actions & action) != 0;
		}
		protected internal virtual void PerformActions(WeekDaysCheckEditActions actions) {
			if (IsUpdateLocked) {
				deferredChanges |= actions;
				return;
			}
			SuspendLayout();
			try {
				PerformActionsCore(actions);
			}
			finally {
				ResumeLayout();
			}
		}
		protected internal virtual void PerformActionsCore(WeekDaysCheckEditActions actions) {
			if (IsActionQueued(actions, WeekDaysCheckEditActions.ClearCheckControls))
				ClearCheckControls();
			if (IsActionQueued(actions, WeekDaysCheckEditActions.UpdateCheckWeekDays))
				UpdateCheckWeekDays();
			if (IsActionQueued(actions, WeekDaysCheckEditActions.CreateCheckControls))
				CreateCheckControls();
			if (IsActionQueued(actions, WeekDaysCheckEditActions.UpdateCheckControls))
				UpdateCheckControls();
			if (IsActionQueued(actions, WeekDaysCheckEditActions.UpdateCheckControlsState))
				UpdateCheckControlsState();
			if (IsActionQueued(actions, WeekDaysCheckEditActions.RecalcLayout))
				RecalcLayout();
			if (IsActionQueued(actions, WeekDaysCheckEditActions.RaiseWeekDaysChanged))
				RaiseWeekDaysChanged();
		}
		protected internal virtual void UnsubscribeCheckEvent(CheckEdit check) {
			check.CheckedChanged -= new EventHandler(OnCheckedChanged);
		}
		protected internal virtual void SubscribeCheckEvent(CheckEdit check) {
			check.CheckedChanged += new EventHandler(OnCheckedChanged);
		}
		protected internal virtual void UpdateCheckWeekDays() {
			this.checkWeekDays = FilterWeekDays(DateTimeHelper.GetWeekDays(DateTimeHelper.ConvertFirstDayOfWeek(FirstDayOfWeek)));
		}
		protected internal virtual DayOfWeek[] FilterWeekDays(DayOfWeek[] days) {
			if (visibleWeekDays == WeekDays.EveryDay)
				return days;
			int count = days.Length;
			List<DayOfWeek> result = new List<DayOfWeek>(count);
			for (int i = 0; i < count; i++) {
				WeekDays dayOfWeek = DateTimeHelper.ToWeekDays(days[i]);
				if ((dayOfWeek & visibleWeekDays) != 0)
					result.Add(days[i]);
			}
			return result.ToArray();
		}
		protected internal virtual void ClearCheckControls() {
			while (Controls.Count != 0) {
				CheckEdit check = (CheckEdit)Controls[0];
				UnsubscribeCheckEvent(check);
				check.Dispose();
			}
		}
		protected internal virtual void CreateCheckControls() {
			int count = checkWeekDays.Length;
			for (int i = 0; i < count; i++) {
				CheckEdit check = new CheckEdit();
				SubscribeCheckEvent(check);
				Controls.Add(check);
			}
		}
		protected internal virtual void UpdateCheckControls() {
			int count = Controls.Count;
			XtraSchedulerDebug.Assert(count == checkWeekDays.Length);
			DateTimeFormatInfo dtfi = DateTimeFormatHelper.CurrentUIDateTimeFormat;
			for (int i = 0; i < count; i++) {
				CheckEdit check = (CheckEdit)Controls[i];
				DayOfWeek dayOfWeek = checkWeekDays[i];
				check.Tag = DateTimeHelper.ToWeekDays(dayOfWeek);
				check.Text = useAbbreviatedDayNames ? dtfi.GetAbbreviatedDayName(dayOfWeek) : dtfi.GetDayName(dayOfWeek);
				check.Size = check.CalcBestSize();
				check.TabIndex = i;
			}
		}
		protected internal virtual void UpdateCheckControlsState() {
			int count = Controls.Count;
			for (int i = 0; i < count; i++)
				UpdateCheckState((CheckEdit)Controls[i]);
		}
		protected internal virtual void UpdateCheckState(CheckEdit check) {
			WeekDays day = (WeekDays)check.Tag;
			UnsubscribeCheckEvent(check);
			try {
				check.Checked = (this.WeekDays & day) != 0;
			}
			finally {
				SubscribeCheckEvent(check);
			}
		}
		protected internal virtual void RecalcLayout() {
			int count = Controls.Count;
			if (count <= 0)
				return;
			CheckRows rows = new CheckRows();
			CheckRow row = new CheckRow();
			rows.Add(row);
			for (int i = 0; i < count; i++)
				row.Add((CheckEdit)Controls[i]);
			LayoutCheckTable(rows);
		}
		protected internal virtual void OnCheckedChanged(object sender, EventArgs e) {
			CheckEdit check = (CheckEdit)sender;
			WeekDays day = (WeekDays)check.Tag;
			if (check.Checked)
				weekDays |= day;
			else
				weekDays &= (~day);
			RaiseWeekDaysChanged();
		}
		#region Checkboxes layout
		class CheckRow : List<CheckEdit> {
		}
		class CheckRows : List<CheckRow> {
		}
		void DistributeCells(CheckRows rows) {
			while (!IsRowsValid(rows)) {
				int rowCount = rows.Count;
				for (int i = 0; i < rowCount; i++) {
					CheckRow row = rows[i];
					if (!IsRowValid(row, rows)) {
						int index = row.Count - 1;
						CheckEdit check = row[index];
						row.RemoveAt(index);
						if (i + 1 == rowCount)
							rows.Add(new CheckRow());
						rows[i + 1].Insert(0, check);
						break;
					}
				}
			}
		}
		void LayoutCheckTable(CheckRows rows) {
			DistributeCells(rows);
			int rowCount = rows.Count;
			int y = 0;
			int height = ((CheckEdit)Controls[0]).Height;
			for (int rowIndex = 0; rowIndex < rowCount; rowIndex++) {
				CheckRow row = rows[rowIndex];
				int x = RightToLeft == System.Windows.Forms.RightToLeft.No ? 0 : this.Width;
				int colCount = row.Count;
				for (int colIndex = 0; colIndex < colCount; colIndex++) {
					CheckEdit check = (CheckEdit)row[colIndex];
					if (RightToLeft == System.Windows.Forms.RightToLeft.No) {
						check.Location = new Point(x, y);
						x += CalcColumnWidth(rows, colIndex) + padding;
					} else {
						int width = CalcColumnWidth(rows, colIndex) + padding;
						x -= width;
						int rtlOffset = 0;
						if (colIndex < row.Count){
							int checkBoxWidth = CalculateCheckBoxScaledWidth(row[colIndex]);
							if (width != checkBoxWidth)
								rtlOffset = width - checkBoxWidth;
						}
						check.Location = new Point(x + rtlOffset, y);
					}
				}
				y += height + padding;
			}
		}
		const int padding = 8;
		int CalcRowWidth(CheckRow row, CheckRows rows) {
			int columnCount = row.Count;
			if (columnCount <= 0)
				return 0;
			int width = 0;
			for (int columnIndex = 0; columnIndex < columnCount; columnIndex++)
				width += CalcColumnWidth(rows, columnIndex);
			return width + (columnCount - 1) * padding;
		}
		bool IsRowsValid(CheckRows rows) {
			int count = rows.Count;
			for (int i = 0; i < count; i++)
				if (!IsRowValid(rows[i], rows))
					return false;
			return true;
		}
		bool IsRowValid(CheckRow row, CheckRows rows) {
			return row.Count == 1 || CalcRowWidth(row, rows) <= Width;
		}
		int CalcColumnWidth(CheckRows rows, int columnIndex) {
			int width = 0;
			int rowsCount = rows.Count;
			for (int i = 0; i < rowsCount; i++) {
				CheckRow row = rows[i];
				if (columnIndex < row.Count)
					width = Math.Max(width, CalculateCheckBoxScaledWidth(row[columnIndex]));
			}
			return width;
		}
		int CalculateCheckBoxScaledWidth(CheckEdit chkEdit) {
			return (int)(chkEdit.Width * this.factor.Width);
		}
		#endregion
	}
}
namespace DevExpress.XtraScheduler.Native {
	#region WeekDaysCheckEditActions
	[Flags]
	public enum WeekDaysCheckEditActions {
		None = 0x00000000,
		ClearCheckControls = 0x00000001,
		CreateCheckControls = 0x00000002,
		UpdateCheckControls = 0x00000004,
		UpdateCheckControlsState = 0x00000008,
		RecalcLayout = 0x00000010,
		RaiseWeekDaysChanged = 0x00000020,
		UpdateCheckWeekDays = 0x00000040
	}
	#endregion
}
