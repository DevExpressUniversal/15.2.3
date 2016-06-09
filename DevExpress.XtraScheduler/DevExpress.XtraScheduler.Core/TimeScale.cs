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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.XtraScheduler.Localization;
using System.Collections.Specialized;
using DevExpress.XtraScheduler.Native;
#if !SL
using DevExpress.Utils.Design;
using DevExpress.Utils;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraScheduler {
	#region TimeScale classes
	#region TimeScale (abstract class)
#if !SL
	[TypeConverter(typeof(DevExpress.Utils.Design.UniversalTypeConverterEx))]
#endif
	public abstract class TimeScale : UserInterfaceObject, INotifyPropertyChanged {
		#region Fields
		const int defaultWidth = 50;
		const bool defaultEnabled = true;
		const bool defaultVisible = true;
		bool enabled = defaultEnabled;
		bool visible = defaultVisible;
		int width = defaultWidth;
		string displayFormat = String.Empty;
		ITimeScaleDateTimeFormatterFactory formatterFactory;
		#endregion
		protected TimeScale()
			: this(String.Empty) {
			ResetDisplayName();
			ResetMenuCaption();
		}
		protected TimeScale(string displayName)
			: this(displayName, displayName) {
		}
		protected TimeScale(string displayName, string menuCaption)
			: base(displayName, menuCaption) {
			ShouldSerializeHelper.RegisterXtraShouldSerializeMethod("Enabled", XtraShouldSerializeEnabled);
			ShouldSerializeHelper.RegisterXtraShouldSerializeMethod("Visible", XtraShouldSerializeVisible);
		}
		#region Properties
		#region Width
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("TimeScaleWidth"),
#endif
DefaultValue(defaultWidth), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), NotifyParentProperty(true), AutoFormatEnable()]
		public int Width {
			get { return width; }
			set {
				int val = Math.Max(1, value);
				if (width == val)
					return;
				int oldValue = width;
				width = val;
				RaisePropertyChanged("Width");
				OnChanged("Width", oldValue, val);
			}
		}
		#endregion
		#region Enabled
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("TimeScaleEnabled"),
#endif
XtraSerializableProperty(XtraSerializationFlags.DefaultValue), NotifyParentProperty(true), AutoFormatDisable()]
		public bool Enabled {
			get { return enabled; }
			set {
				if (enabled == value)
					return;
				enabled = value;
				RaisePropertyChanged("Enabled");
				OnChanged("Enabled", !value, value);
			}
		}
		protected internal virtual bool ShouldSerializeEnabled() {
			return Enabled != defaultEnabled;
		}
		protected internal virtual bool XtraShouldSerializeEnabled() {
			return ShouldSerializeEnabled();
		}
		#endregion
		#region Visible
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("TimeScaleVisible"),
#endif
XtraSerializableProperty(XtraSerializationFlags.DefaultValue), NotifyParentProperty(true), AutoFormatDisable()]
		public bool Visible {
			get { return visible; }
			set {
				if (visible == value)
					return;
				visible = value;
				RaisePropertyChanged("Visible");
				OnChanged("Visible", !value, value);
			}
		}
		protected internal virtual bool ShouldSerializeVisible() {
			return Visible != defaultVisible;
		}
		protected internal virtual bool XtraShouldSerializeVisible() {
			return ShouldSerializeVisible();
		}
		#endregion
		#region DisplayFormat
#if !SL
		[TypeConverter(typeof(DateTimeFormatConverter))]
#endif
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("TimeScaleDisplayFormat"),
#endif
		DefaultValue(""), NotifyParentProperty(true),
		XtraSerializableProperty(XtraSerializationFlags.DefaultValue), AutoFormatDisable, Localizable(false)]
		public string DisplayFormat {
			get { return displayFormat; }
			set {
				if (displayFormat == value)
					return;
				string oldValue = displayFormat;
				displayFormat = value;
				OnChanged("DisplayFormat", oldValue, value);
			}
		}
		#endregion
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		[AutoFormatDisable, Localizable(false)]
		public string SerializationTypeName { get { return TypeSerializationHelper.GetSerializationTypeName(GetType()); } }
		protected internal string ActualDisplayFormat { get { return String.IsNullOrEmpty(DisplayFormat) ? DefaultDisplayFormat : DisplayFormat; } }
		protected internal abstract string DefaultDisplayFormat { get; }
		protected internal abstract TimeSpan SortingWeight { get; }
		protected internal ITimeScaleDateTimeFormatterFactory FormatterFactory {
			get {
				if (formatterFactory == null)
					return TimeScaleDateTimeFormatterFactory.Default;
				return formatterFactory;
			}
		}
		internal void SetFormatterFactory(ITimeScaleDateTimeFormatterFactory factory) {
			this.formatterFactory = factory;
		}
		#endregion
		protected internal abstract bool HasNextDate(DateTime date);
		public abstract DateTime Floor(DateTime date);
		public abstract DateTime GetNextDate(DateTime date);
		public virtual DateTime Round(DateTime date) {
			DateTime start = Floor(date);
			DateTime end = Ceil(date);
			return (date.Ticks - start.Ticks < end.Ticks - date.Ticks) ? start : end;
		}
		public virtual string FormatCaption(DateTime start, DateTime end) {
			ITimeScaleDateTimeFormatter formatter = CreateDateTimeFormatter();
			return formatter != null ? formatter.Format(ActualDisplayFormat, start, end) : string.Empty;
		}
		protected virtual ITimeScaleDateTimeFormatter CreateDateTimeFormatter() {
			return FormatterFactory.CreateFormatter(this);
		}
		public virtual DateTime Ceil(DateTime date) {
			return GetNextDate(GetPrevDate(date));
		}
		public DateTime GetPrevDate(DateTime date) {
			if (date == DateTime.MinValue)
				return date;
			return Floor(date.AddTicks(-1));
		}
		protected internal virtual void SetFirstDayOfWeek(DayOfWeek firstDayOfWeek) { }
		protected internal override bool ShouldSerializeDisplayName() {
			return DisplayName != DefaultDisplayName;
		}
		protected internal override bool ShouldSerializeMenuCaption() {
			return MenuCaption != DefaultMenuCaption;
		}
		public override bool Equals(object obj) {
			if ( !base.Equals(obj) )
				return false;
			TimeScale scale = obj as TimeScale;
			return scale.GetType().Equals(GetType()) && scale.Enabled == Enabled && scale.Visible == Visible &&
				scale.Width == Width && scale.DisplayFormat == DisplayFormat;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		protected internal override void Assign(UserInterfaceObject source) {
			base.Assign(source);
			TimeScale timeScale = source as TimeScale;
			if (timeScale != null) {
				DisplayFormat = timeScale.DisplayFormat;
				DisplayName = timeScale.DisplayName;
				Enabled = timeScale.Enabled;
				Width = timeScale.Width;
				Visible = timeScale.Visible;
			}
		}
	}
	#endregion
	#region TimeScaleFixedInterval
	public class TimeScaleFixedInterval : TimeScale {
		#region Fields
		internal static readonly TimeSpan defaultScaleValue = TimeSpan.FromMinutes(15);
		const bool defaultEnabled = false;
		TimeSpan scaleValue;
		#endregion
		public TimeScaleFixedInterval()
			: this(defaultScaleValue) {
		}
		public TimeScaleFixedInterval(TimeSpan scaleValue) {
			this.scaleValue = scaleValue;
			ResetMenuCaption();
			ResetDisplayName();
			ShouldSerializeHelper.RegisterXtraShouldSerializeMethod("Value", XtraShouldSerializeValue);
		}
		#region Properties
		#region Value
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("TimeScaleFixedIntervalValue"),
#endif
		NotifyParentProperty(true), RefreshProperties(RefreshProperties.Repaint), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), AutoFormatDisable()]
		public TimeSpan Value {
			get { return scaleValue; }
			set {
				if (scaleValue == value)
					return;
				bool updateDisplayName = (DisplayName == DefaultDisplayName);
				bool updateMenuCaption = (MenuCaption == DefaultMenuCaption);
				TimeSpan oldValue = scaleValue;
				scaleValue = value;
				if (updateDisplayName)
					ResetDisplayName();
				if (updateMenuCaption)
					ResetMenuCaption();
				OnChanged("Value", oldValue, value);
			}
		}
		internal bool XtraShouldSerializeValue() {
			return ShouldSerializeValue();
		}
		protected internal virtual bool ShouldSerializeValue() {
			return Value != defaultScaleValue;
		}
		internal void ResetValue() {
			Value = defaultScaleValue;
		}
		#endregion
		protected internal override string DefaultDisplayName { get { return Value.ToString(); } }
		protected internal override string DefaultMenuCaption { get { return Value.ToString(); } }
		protected internal override string DefaultDisplayFormat { get { return DateTimeFormatHelper.CorrectShortTimePattern; } } 
		protected internal override TimeSpan SortingWeight { get { return Value; } }
		#endregion
		public override DateTime Floor(DateTime date) {
			return DateTimeHelper.Floor(date, scaleValue);
		}
		protected internal virtual DateTime Floor(DateTime date, DateTime baseDate) {
			return DateTimeHelper.Floor(date, scaleValue, baseDate);
		}
		public override DateTime GetNextDate(DateTime date) {
			return HasNextDate(date) ? date + scaleValue : date;
		}
		protected internal override bool HasNextDate(DateTime date) {
			return date <= DateTime.MaxValue - scaleValue;
		}
		public override bool Equals(object obj) {
			if ( !(base.Equals(obj)) )
				return false;
			TimeScaleFixedInterval scale = obj as TimeScaleFixedInterval;
			return Value.Equals(scale.Value);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		protected internal override void Assign(UserInterfaceObject source) {
			base.Assign(source);
			TimeScaleFixedInterval timeScale = source as TimeScaleFixedInterval;
			if(timeScale != null)
				Value = timeScale.Value;
		}
	}
	#endregion
	#region DayViewTimeScale
	public class DayViewTimeScale : TimeScaleFixedInterval {
		TimeSpan timeOfDayStart;
		public DayViewTimeScale(TimeSpan scaleValue, TimeSpan timeOfDayStart)
			: base(scaleValue) {
			this.timeOfDayStart = timeOfDayStart;
		}
		public TimeSpan TimeOfDayStart {
			get { return timeOfDayStart; }
			set { timeOfDayStart = value; }
		}
		public override DateTime Floor(DateTime date) {
			return DateTimeHelper.Floor(date, Value, date.Date.Add(this.timeOfDayStart));
		}
	}
	#endregion
	#region TimeScale15Minutes
	public class TimeScale15Minutes : TimeScaleFixedInterval {
		public TimeScale15Minutes()
			: base(TimeSpan.FromMinutes(15)) {
		}
		protected internal override string DefaultDisplayName { get { return SchedulerLocalizer.GetString(SchedulerStringId.Caption_15Minutes); } }
		protected internal override string DefaultMenuCaption { get { return SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_15Minutes); } }
		protected internal override string DefaultDisplayFormat { get { return DateTimeFormatHelper.CorrectShortTimePattern; } }
		#region Value
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public new TimeSpan Value { get { return base.Value; } set { } }
		protected internal override bool ShouldSerializeValue() {
			return false;
		}
		#endregion
	}
	#endregion
	#region TimeScaleHour
	public class TimeScaleHour : TimeScaleFixedInterval {
		const bool defaultEnabled = false;
		public TimeScaleHour()
			: base(TimeSpan.FromHours(1)) {
		}
		protected internal override string DefaultDisplayName { get { return SchedulerLocalizer.GetString(SchedulerStringId.TimeScaleDisplayName_Hour); } }
		protected internal override string DefaultMenuCaption { get { return SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_TimeScaleHour); } }
		protected internal override string DefaultDisplayFormat { get { return DateTimeFormatHelper.CorrectShortTimePattern; } }
		#region Value
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public new TimeSpan Value { get { return base.Value; } set { } }
		protected internal override bool ShouldSerializeValue() {
			return false;
		}
		#endregion
		public override DateTime Floor(DateTime date) {
			return new DateTime(date.Year, date.Month, date.Day, date.Hour, 0, 0);
		}
		public override DateTime GetNextDate(DateTime date) {
			return HasNextDate(date) ? date.AddHours(1) : date;
		}
		protected internal override bool HasNextDate(DateTime date) {
			return date <= DateTime.MaxValue.AddHours(-1);
		}
	}
	#endregion
	#region TimeScaleDay
	public class TimeScaleDay : TimeScaleFixedInterval {
		const bool defaultEnabled = true;
		public TimeScaleDay()
			: base(TimeSpan.FromDays(1)) {
		}
		protected internal override string DefaultDisplayName { get { return SchedulerLocalizer.GetString(SchedulerStringId.TimeScaleDisplayName_Day); } }
		protected internal override string DefaultMenuCaption { get { return SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_TimeScaleDay); } }
		protected internal override string DefaultDisplayFormat { get { return "d ddd"; } }  
		#region Value
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public new TimeSpan Value { get { return base.Value; } set { } }
		protected internal override bool ShouldSerializeValue() {
			return false;
		}
		#endregion
		public override DateTime Round(DateTime date) {
			DateTime dt = date.AddHours(12);
			return dt.Date;
		}
		public override DateTime Floor(DateTime date) {
			return date.Date;
		}
		public override DateTime GetNextDate(DateTime date) {
			return HasNextDate(date) ? date.AddDays(1) : date;
		}
		protected internal override bool HasNextDate(DateTime date) {
			return date <= DateTime.MaxValue.AddDays(-1);
		}
	}
	#endregion
	#region TimeScaleWeek
	public class TimeScaleWeek : TimeScaleFixedInterval {
		const bool defaultEnabled = true;
		DayOfWeek firstDayOfWeek = DateTimeHelper.FirstDayOfWeek;
		public TimeScaleWeek()
			: base(TimeSpan.FromDays(7)) {
		}
		#region Properties
		protected internal override string DefaultDisplayName { get { return SchedulerLocalizer.GetString(SchedulerStringId.TimeScaleDisplayName_Week); } }
		protected internal override string DefaultMenuCaption { get { return SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_TimeScaleWeek); } }
		protected internal override string DefaultDisplayFormat { get { return DateTimeFormatHelper.CurrentDateTimeFormat.LongDatePattern; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DayOfWeek FirstDayOfWeek { get { return firstDayOfWeek; } }
		#region Value
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public new TimeSpan Value { get { return base.Value; } set { } }
		protected internal override bool ShouldSerializeValue() {
			return false;
		}
		#endregion
		#endregion
		public override DateTime Floor(DateTime date) {
			return DateTimeHelper.GetStartOfWeekUI(date, FirstDayOfWeek);
		}
		public override DateTime GetNextDate(DateTime date) {
			return HasNextDate(date) ? date.AddDays(7) : date;
		}
		protected internal override bool HasNextDate(DateTime date) {
			return date <= DateTime.MaxValue.AddDays(-7);
		}
		protected internal override void SetFirstDayOfWeek(DayOfWeek firstDayOfWeek) {
			this.firstDayOfWeek = firstDayOfWeek;
		}
	}
	#endregion
	#region TimeScaleMonth
	public class TimeScaleMonth : TimeScale {
		const bool defaultEnabled = false;
		public TimeScaleMonth() {
		}
		protected internal override string DefaultDisplayName { get { return SchedulerLocalizer.GetString(SchedulerStringId.TimeScaleDisplayName_Month); } }
		protected internal override string DefaultMenuCaption { get { return SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_TimeScaleMonth); } }
		protected internal override string DefaultDisplayFormat { get { return "MMMM"; } }  
		protected internal override TimeSpan SortingWeight { get { return TimeSpan.FromDays(31); } }
		public override DateTime Floor(DateTime date) {
			return new DateTime(date.Year, date.Month, 1);
		}
		public override DateTime GetNextDate(DateTime date) {
			return HasNextDate(date) ? date.AddMonths(1) : date;
		}
		protected internal override bool HasNextDate(DateTime date) {
			return date <= DateTime.MaxValue.AddMonths(-1);
		}
	}
	#endregion
	#region TimeScaleQuarter
	public class TimeScaleQuarter : TimeScale {
		const bool defaultEnabled = false;
		public TimeScaleQuarter() {
		}
		protected internal override string DefaultDisplayName { get { return SchedulerLocalizer.GetString(SchedulerStringId.TimeScaleDisplayName_Quarter); } }
		protected internal override string DefaultMenuCaption { get { return SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_TimeScaleQuarter); } }
		protected internal override string DefaultDisplayFormat { get { return "Q{0}"; } }  
		protected internal override TimeSpan SortingWeight { get { return TimeSpan.FromDays(93); } }
		public override DateTime Floor(DateTime date) {
			return new DateTime(date.Year, DateTimeHelper.CalcFirstMonthOfQuarter(DateTimeHelper.CalcQuarterNumber(date.Month)), 1);
		}
		public override DateTime GetNextDate(DateTime date) {
			return HasNextDate(date) ? date.AddMonths(3) : date;
		}
		protected internal override bool HasNextDate(DateTime date) {
			return date <= DateTime.MaxValue.AddMonths(-3);
		}
	}
	#endregion
	#region TimeScaleYear
	public class TimeScaleYear : TimeScale {
		const bool defaultEnabled = false;
		public TimeScaleYear() {
		}
		protected internal override string DefaultDisplayName { get { return SchedulerLocalizer.GetString(SchedulerStringId.TimeScaleDisplayName_Year); } }
		protected internal override string DefaultMenuCaption { get { return SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_TimeScaleYear); } }
		protected internal override string DefaultDisplayFormat { get { return "yyyy"; } }
		protected internal override TimeSpan SortingWeight { get { return TimeSpan.FromDays(366); } }
		protected internal override bool ShouldSerializeDisplayName() {
			return DisplayName != DefaultDisplayName;
		}
		protected internal override bool ShouldSerializeMenuCaption() {
			return MenuCaption != DefaultMenuCaption;
		}
		public override DateTime Floor(DateTime date) {
			return new DateTime(date.Year, 1, 1);
		}
		public override DateTime GetNextDate(DateTime date) {
			return HasNextDate(date) ? date.AddYears(1) : date;
		}
		protected internal override bool HasNextDate(DateTime date) {
			return date <= DateTime.MaxValue.AddYears(-1);
		}
	}
	#endregion
	#endregion
	#region TimeScaleCollection
#if !SL
	[ListBindable(BindableSupport.No), TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter))]
#endif
	public class TimeScaleCollection : UserInterfaceObjectCollection<TimeScale> {
		public TimeScaleCollection()
			: base(DXCollectionUniquenessProviderType.None) {
		}
		protected override TimeScale GetItem(int index) {
			return InnerList[index];
		}
		protected internal override UserInterfaceObjectCollection<TimeScale> CreateDefaultContent() {
			TimeScaleCollection defaultContent = new TimeScaleCollection();
			AddScaleToCollection(defaultContent, new TimeScaleYear(), false);
			AddScaleToCollection(defaultContent, new TimeScaleQuarter(), false);
			AddScaleToCollection(defaultContent, new TimeScaleMonth(), false);
			AddScaleToCollection(defaultContent, new TimeScaleWeek(), true);
			AddScaleToCollection(defaultContent, new TimeScaleDay(), true);
			AddScaleToCollection(defaultContent, new TimeScaleHour(), false);
			AddScaleToCollection(defaultContent, new TimeScale15Minutes(), false);
			return defaultContent;
		}
		void AddScaleToCollection(TimeScaleCollection scales, TimeScale scale, bool enabled) {
			scale.Enabled = enabled;
			scales.Add(scale);
		}
		protected internal override TimeScale CreateItemInstance(object id, string displayName, string menuCaption) {
			TimeScaleFixedInterval item = new TimeScaleFixedInterval();
			item.DisplayName = displayName;
			item.MenuCaption = menuCaption;
			return item;
		}
		protected internal override object ProvideDefaultId() {
			return null;
		}
		protected internal override TimeScale CloneItem(TimeScale item) {
			return item;
		}
	}
	#endregion
	#region TimeScaleComparer
	public class TimeScaleComparer : IComparer<TimeScale>, IComparer {
		int IComparer.Compare(object x, object y) {
			TimeScale valX = (TimeScale)x;
			TimeScale valY = (TimeScale)y;
			return CompareCore(valX, valY);
		}
		public int Compare(TimeScale x, TimeScale y) {
			return CompareCore(x, y);
		}
		protected internal int CompareCore(TimeScale valX, TimeScale valY) {
			return -valX.SortingWeight.CompareTo(valY.SortingWeight);
		}
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Native {
	#region TimeScaleCollectionHelper
	public static class TimeScaleCollectionHelper {
		public static TimeScaleCollection CreateSortedCollection(TimeScaleCollection sourceScales) {
			return CreateSortedScalesCore(SelectEnabledScales(sourceScales));
		}
		public static TimeScaleCollection CreateSortedAvailableScales(TimeScaleCollection sourceScales) {
			return CreateSortedScalesCore(sourceScales);
		}
		internal static TimeScaleCollection CreateSortedScalesCore(TimeScaleCollection sourceScales) {
			TimeScaleCollection result = new TimeScaleCollection();
			result.AddRange(sourceScales);
			if (result.Count == 0) {
				TimeScaleCollection defaultContent = new TimeScaleCollection();
				defaultContent.LoadDefaults();
				result.AddRange(SelectEnabledScales(defaultContent));
			}
			result.Sort(new TimeScaleComparer());
			return result;
		}
		public static TimeScaleCollection SelectEnabledScales(TimeScaleCollection sourceScales) {
			TimeScaleCollection result = new TimeScaleCollection();
			int count = sourceScales.Count;
			for (int i = 0; i < count; i++) {
				TimeScale item = sourceScales[i];
				if (item.Enabled)
					result.Add(item);
			}
			return result;
		}
		public static TimeScaleCollection SelectVisibleScales(TimeScaleCollection sourceScales) {
			TimeScaleCollection result = new TimeScaleCollection();
			TimeScaleCollection col = TimeScaleCollectionHelper.CreateSortedCollection(sourceScales);
			int count = col.Count;
			result.Add(col[count - 1]);
			for (int i = count - 2; i >= 0; i--) {
				TimeScale item = col[i];
				if (item.Visible)
					result.Insert(0, item);
			}
			return result;
		}
		public static void ZoomIn(TimeScaleCollection scales, TimeScaleCollection actualScales) {
			TimeScaleCollection availableScales = TimeScaleCollectionHelper.CreateSortedAvailableScales(scales);
			if (!CanZoomInCore(availableScales, actualScales))
				return;
			scales.BeginUpdate();
			try {
				int count = actualScales.Count;
				for (int i = count - 1; i >= 0; i--) {
					int index = availableScales.IndexOf(actualScales[i]);
					availableScales[index].Enabled = false;
					availableScales[index + 1].Visible = availableScales[index].Visible;
					availableScales[index + 1].Enabled = true;
				}
			} finally {
				scales.EndUpdate();
			}
		}
		public static bool CanZoomIn(TimeScaleCollection scales, TimeScaleCollection actualScales) {
			return CanZoomInCore(TimeScaleCollectionHelper.CreateSortedAvailableScales(scales), actualScales);
		}
		internal static bool CanZoomInCore(TimeScaleCollection availableScales, TimeScaleCollection actualScales) {
			int maxScaleIndex = availableScales.IndexOf(actualScales[actualScales.Count - 1]);
			if (maxScaleIndex < 0)
				return false;
			return maxScaleIndex < availableScales.Count - 1;
		}
		public static void ZoomOut(TimeScaleCollection scales, TimeScaleCollection actualScales) {
			TimeScaleCollection availableScales = TimeScaleCollectionHelper.CreateSortedAvailableScales(scales);
			if (!CanZoomOutCore(availableScales, actualScales))
				return;
			scales.BeginUpdate();
			try {
				int count = actualScales.Count;
				for (int i = 0; i < count; i++) {
					int index = availableScales.IndexOf(actualScales[i]);
					availableScales[index].Enabled = false;
					availableScales[index - 1].Visible = availableScales[index].Visible;
					availableScales[index - 1].Enabled = true;
				}
			} finally {
				scales.EndUpdate();
			}
		}
		public static bool CanZoomOut(TimeScaleCollection scales, TimeScaleCollection actualScales) {
			return CanZoomOutCore(TimeScaleCollectionHelper.CreateSortedAvailableScales(scales), actualScales);
		}
		internal static bool CanZoomOutCore(TimeScaleCollection availableScales, TimeScaleCollection actualScales) {
			int minScaleIndex = availableScales.IndexOf(actualScales[0]);
			return (minScaleIndex > 0);
		}
	}
	#endregion
}
