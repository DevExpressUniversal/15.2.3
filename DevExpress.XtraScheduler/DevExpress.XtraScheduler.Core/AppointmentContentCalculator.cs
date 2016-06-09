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
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Localization;
using System.Globalization;
using DevExpress.XtraScheduler.Services;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.XtraScheduler.Drawing {
	#region AppointmentImageType
	public enum AppointmentImageType {
		Custom = 0,
		Reminder = 1,
		Recurrence = 2,
		RecurrenceException = 3
	};
	#endregion
	#region AppointmentImageInfoCore
	public abstract class AppointmentImageInfoCore {
		#region Fields
		bool visible = true;
		AppointmentImageType type = AppointmentImageType.Custom;
		int imageIndex = -1;
		#endregion
		#region Properties
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentImageInfoCoreImageType")]
#endif
		public AppointmentImageType ImageType { get { return type; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentImageInfoCoreImageIndex")]
#endif
		public int ImageIndex { get { return imageIndex; } set { imageIndex = value; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentImageInfoCoreVisible")]
#endif
		public bool Visible { get { return visible; } set { visible = value; } }
		#endregion
		internal void SetImageType(AppointmentImageType type) {
			this.type = type;
		}
	}
	#endregion
	#region IViewInfoTextItem
	public interface IViewInfoTextItem {
		string Text { get; set; }
	}
	#endregion
	#region AppointmentImageProvider
	public abstract class AppointmentImageProviderCore {
		static readonly Dictionary<AppointmentImageType, int> defaultImageIndexes = CreateDefaultImageIndexesTable();
		public const int DayClockImageIndex = 0;
		public const int NightClockImageIndex = 1;
		public const int RecurrenceImageIndex = 2;
		public const int RecurrenceExceptionImageIndex = 3;
		public const int ReminderImageIndex = 4;
		object defaultAppointmentImages;
		protected AppointmentImageProviderCore(object defaultAppointmentImages) {
			this.defaultAppointmentImages = defaultAppointmentImages;
		}
		public object DefaultAppointmentImages { get { return defaultAppointmentImages; } }
		protected internal static Dictionary<AppointmentImageType, int> DefaultImageIndexes { get { return defaultImageIndexes; } }
		internal static Dictionary<AppointmentImageType, int> CreateDefaultImageIndexesTable() {
			Dictionary<AppointmentImageType, int> ht = new Dictionary<AppointmentImageType, int>();
			ht.Add(AppointmentImageType.Recurrence, RecurrenceImageIndex);
			ht.Add(AppointmentImageType.RecurrenceException, RecurrenceExceptionImageIndex);
			ht.Add(AppointmentImageType.Reminder, ReminderImageIndex);
			return ht;
		}
		public virtual int GetAppointmentImageIndex(AppointmentImageType type) {
			int index;
			if (defaultImageIndexes.TryGetValue(type, out index))
				return index;
			else
				return -1;
		}
	}
	#endregion
	#region IAppointmentImageInfoCoreCollection
	public interface IAppointmentImageInfoCoreCollection {
		int AddStandardInternal(AppointmentImageType type);
	}
	#endregion
	#region AppointmentImageInfoCoreCollection
	public abstract class AppointmentImageInfoCoreCollection<TAppointmentImageInfo> : DXCollection<TAppointmentImageInfo>, IAppointmentImageInfoCoreCollection
		where TAppointmentImageInfo : AppointmentImageInfoCore, new() {
		#region Fields
		AppointmentImageProviderCore imageProvider;
		#endregion
		protected AppointmentImageInfoCoreCollection(AppointmentImageProviderCore imageProvider) {
			if (imageProvider == null)
				Exceptions.ThrowArgumentException("imageProvider", imageProvider);
			this.imageProvider = imageProvider;
		}
		#region Properties
		protected internal AppointmentImageProviderCore ImageProvider { get { return imageProvider; } }
		#endregion
		public virtual int AddStandard(AppointmentImageType type) {
			if (type == AppointmentImageType.Custom)
				return -1;
			TAppointmentImageInfo info = CreateStandardImage(type, false);
			return Add(info);
		}
		public virtual int AddStandardInternal(AppointmentImageType type) {
			if (type == AppointmentImageType.Custom)
				Exceptions.ThrowArgumentException("type", type);
			TAppointmentImageInfo info = CreateStandardImage(type, true);
			return Add(info);
		}
		public virtual TAppointmentImageInfo[] GetAppointmentImageInfos(AppointmentImageType type) {
			List<TAppointmentImageInfo> list = new List<TAppointmentImageInfo>();
			for (int i = 0; i < Count; i++) {
				TAppointmentImageInfo info = this[i];
				if (info.ImageType == type)
					list.Add(info);
			}
			return list.ToArray();
		}
		protected internal virtual TAppointmentImageInfo CreateStandardImage(AppointmentImageType type, bool useIndex) {
			TAppointmentImageInfo info = new TAppointmentImageInfo();
			info.SetImageType(type);
			if (useIndex)
				SetImageIndex(type, info);
			else
				SetImage(type, info);
			return info;
		}
		protected internal virtual void SetImageIndex(AppointmentImageType type, TAppointmentImageInfo info) {
			info.ImageIndex = ImageProvider.GetAppointmentImageIndex(type);
		}
		protected internal abstract void SetImage(AppointmentImageType type, TAppointmentImageInfo info);
	}
	#endregion
	public abstract class AppointmentContentLayoutCalculatorCore<TAppointmentViewInfoItemCollection, TAppointmentImageInfoCollection>
		where TAppointmentViewInfoItemCollection : IList, new()
		where TAppointmentImageInfoCollection : IAppointmentImageInfoCoreCollection {
		#region Fields
		ISupportAppointmentsBase viewInfo;
		AppointmentContentCalculatorHelper contentCalculatorHelper;
		TimeOfDayInterval nightTimeInterval = new TimeOfDayInterval(TimeSpan.FromHours(12), TimeSpan.FromHours(24));
		IAppointmentFormatStringService formatStringProvider;
		#endregion
		#region Events
		#region InitAppointmentDisplayText
		AppointmentDisplayTextEventHandler onInitAppointmentDisplayText;
		public event AppointmentDisplayTextEventHandler InitAppointmentDisplayText { add { onInitAppointmentDisplayText += value; } remove { onInitAppointmentDisplayText -= value; } }
		protected internal virtual void RaiseInitAppointmentDisplayText(AppointmentDisplayTextEventArgs args) {
			if (onInitAppointmentDisplayText != null)
				onInitAppointmentDisplayText(this, args);
		}
		#endregion
		protected internal abstract void RaiseInitAppointmentImages(IAppointmentViewInfo viewInfo, TAppointmentImageInfoCollection imageInfos);
		#endregion
		protected AppointmentContentLayoutCalculatorCore(ISupportAppointmentsBase viewInfo) {
			this.viewInfo = viewInfo;
			this.formatStringProvider = GetFormatStringProvider();
			contentCalculatorHelper = CreateContentLayoutCalculatorHelper();
		}
		protected internal AppointmentContentCalculatorHelper ContentCalculatorHelper { get { return contentCalculatorHelper; } }
		protected internal TimeOfDayInterval NightTimeInterval { get { return nightTimeInterval; } set { nightTimeInterval = value; } }
		protected internal ISupportAppointmentsBase ViewInfo { get { return viewInfo; } set { viewInfo = value; } }
		public IAppointmentFormatStringService FormatProvider { get { return formatStringProvider; } set { formatStringProvider = value; } }
		protected internal virtual bool IsNightTime(DateTime dateTime) {
			TimeSpan time = dateTime.TimeOfDay;
			bool f1 = (NightTimeInterval.Start <= time) && (time <= NightTimeInterval.End);
			bool f2 = ((NightTimeInterval.End.Days >= 1) && (NightTimeInterval.End >= time + TimeSpan.FromDays(1)));
			return f1 || f2;
		}
		protected internal virtual int CalculateClockImageIndex(DateTime time) {
			return IsNightTime(time) ? AppointmentImageProviderCore.NightClockImageIndex : AppointmentImageProviderCore.DayClockImageIndex;
		}
		protected internal virtual IViewInfoTextItem GetTitleTextItem(IList textItems) {
			XtraSchedulerDebug.Assert(textItems.Count == 2);
			return textItems[0] as IViewInfoTextItem;
		}
		protected internal virtual IViewInfoTextItem GetDescriptionTextItem(IList textItems) {
			XtraSchedulerDebug.Assert(textItems.Count == 2);
			return textItems[1] as IViewInfoTextItem;
		}
		protected internal virtual TAppointmentViewInfoItemCollection CreateAppointmentViewInfoTextItems(IAppointmentViewInfo aptViewInfo) {
			Appointment appointment = aptViewInfo.Appointment;
			String title = appointment.Subject;
			if (!String.IsNullOrEmpty(appointment.Location))
				title += String.Format(" ({0})", appointment.Location);
			AppointmentDisplayTextEventArgs args = new AppointmentDisplayTextEventArgs(aptViewInfo, title, appointment.Description);
			RaiseInitAppointmentDisplayText(args);
			IViewInfoTextItem titleTextItem = CreateTextItemInstance();
			titleTextItem.Text = args.Text;
			IViewInfoTextItem descriptionTextItem = CreateTextItemInstance();
			descriptionTextItem.Text = args.Description;
			TAppointmentViewInfoItemCollection items = new TAppointmentViewInfoItemCollection();
			items.Add(titleTextItem);
			items.Add(descriptionTextItem);
			return items;
		}
		protected internal virtual TAppointmentImageInfoCollection CreateAppointmentImageInfoCollection(IAppointmentViewInfo aptViewInfo) {
			TAppointmentImageInfoCollection result = CreateImageInfoCollection();
			if (aptViewInfo.Options.ShowBell)
				result.AddStandardInternal(AppointmentImageType.Reminder);
			if (aptViewInfo.Options.ShowRecurrence) {
				Appointment apt = aptViewInfo.Appointment;
				if (apt.IsException)
					result.AddStandardInternal(AppointmentImageType.RecurrenceException);
				else if (apt.IsOccurrence)
					result.AddStandardInternal(AppointmentImageType.Recurrence);
			}
			return result;
		}
		protected internal virtual TAppointmentViewInfoItemCollection CreateAppointmentViewInfoImageItems(IAppointmentViewInfo aptViewInfo) {
			TAppointmentViewInfoItemCollection imageItems = new TAppointmentViewInfoItemCollection();			
			TAppointmentImageInfoCollection imageInfos = CreateAppointmentImageInfoCollection(aptViewInfo);
			RaiseInitAppointmentImages(aptViewInfo, imageInfos);
			FillAppointmentViewInfoImageItems(imageInfos, imageItems);
			return imageItems;
		}
		protected internal virtual TAppointmentViewInfoItemCollection CreateClockTextItems(IAppointmentViewInfo aptViewInfo) {
			TAppointmentViewInfoItemCollection items = new TAppointmentViewInfoItemCollection();
			if (aptViewInfo.Options.ShowStartTime)
				items.Add(CreateStartClockTextItem(aptViewInfo));
			if (aptViewInfo.Options.ShowEndTime)
				items.Add(CreateEndClockTextItem(aptViewInfo));
			return items;
		}
		protected internal virtual IViewInfoTextItem CreateStartClockTextItem(IAppointmentViewInfo aptViewInfo) {
			IViewInfoTextItem textItem = CreateClockTextItemInstance();
			textItem.Text = ContentCalculatorHelper.GetStartTimeText(aptViewInfo);
			return textItem;
		}
		protected internal virtual IViewInfoTextItem CreateEndClockTextItem(IAppointmentViewInfo aptViewInfo) {
			IViewInfoTextItem textItem = CreateClockTextItemInstance();
			textItem.Text = ContentCalculatorHelper.GetEndTimeText(aptViewInfo);
			return textItem;
		}
		protected internal abstract IAppointmentFormatStringService GetFormatStringProvider();
		protected internal abstract AppointmentContentCalculatorHelper CreateContentLayoutCalculatorHelper();
		protected internal abstract TAppointmentImageInfoCollection CreateImageInfoCollection();
		protected internal abstract void FillAppointmentViewInfoImageItems(TAppointmentImageInfoCollection imageInfos, TAppointmentViewInfoItemCollection imageItems);
		protected internal abstract IViewInfoTextItem CreateTextItemInstance();
		protected internal abstract IViewInfoTextItem CreateClockTextItemInstance();
	}
	public abstract class AppointmentContentCalculatorHelper {
		ISupportAppointmentsBase viewInfo;
		AppointmentStatusDisplayType statusDisplayType;
		IAppointmentFormatStringService formatProvider;
		protected AppointmentContentCalculatorHelper(ISupportAppointmentsBase viewInfo, AppointmentStatusDisplayType statusDisplayType, IAppointmentFormatStringService formatProvider) {
			if (viewInfo == null)
				Exceptions.ThrowArgumentNullException("viewInfo");
			this.viewInfo = viewInfo;
			this.statusDisplayType = statusDisplayType;
			this.formatProvider = formatProvider;
		}
		protected internal ISupportAppointmentsBase ViewInfo { get { return viewInfo; } set { viewInfo = value; } }
		protected internal AppointmentDisplayOptions AppointmentDisplayOptions { get { return ViewInfo.AppointmentDisplayOptions; } }
		protected internal AppointmentStatusDisplayType StatusDisplayType { get { return statusDisplayType; } }
		protected internal AppointmentTimeDisplayType AppointmentTimeDisplayType { get { return AppointmentDisplayOptions.TimeDisplayType; } }
		protected internal AppointmentTimeVisibility AppointmentStartTimeVisibility { get { return AppointmentDisplayOptions.StartTimeVisibility; } }
		protected internal AppointmentTimeVisibility AppointmentEndTimeVisibility { get { return AppointmentDisplayOptions.EndTimeVisibility; } }
		protected internal bool ShowAppointmentRecurrence { get { return AppointmentDisplayOptions.ShowRecurrence; } }
		protected internal bool ShowAppointmentReminder { get { return AppointmentDisplayOptions.ShowReminder; } }
		protected internal IAppointmentFormatStringService FormatProvider { get { return formatProvider; } set { formatProvider = value; } }
		public virtual void CalculateViewInfoOptions(IAppointmentViewInfo aptViewInfo) {
			aptViewInfo.Options.ShowTimeAsClock = ShouldShowTimeAsClock(aptViewInfo);
			aptViewInfo.Options.ShowRecurrence = ShouldShowRecurrence(aptViewInfo);
			aptViewInfo.Options.ShowBell = ShouldShowReminder(aptViewInfo);
			aptViewInfo.Options.ShowStartTime = ShouldShowStartTime(aptViewInfo);
			aptViewInfo.Options.ShowEndTime = ShouldShowEndTime(aptViewInfo);
			aptViewInfo.Options.StatusDisplayType = StatusDisplayType;
			aptViewInfo.Options.PercentCompleteDisplayType = GetPercentCompleteDisplayType();
		}
		protected internal virtual string GetDefaultAppointmentTimeText(DateTime time) {
			return DateTimeFormatHelper.DateToShortTimeString(time);
		}
		protected internal virtual bool ShouldShowStartTime(IAppointmentViewInfo aptViewInfo) {
			switch (AppointmentStartTimeVisibility) {
				case AppointmentTimeVisibility.Never:
					return false;
				case AppointmentTimeVisibility.Always:
					return ShouldShowStartTimeAlways(aptViewInfo);
				case AppointmentTimeVisibility.Auto:
				default:
					return ShouldShowStartTimeAuto(aptViewInfo);
			}
		}
		protected internal virtual bool ShouldShowEndTime(IAppointmentViewInfo aptViewInfo) {
			switch (AppointmentEndTimeVisibility) {
				case AppointmentTimeVisibility.Never:
					return false;
				case AppointmentTimeVisibility.Always:
					return ShouldShowEndTimeAlways(aptViewInfo);
				case AppointmentTimeVisibility.Auto:
				default:
					return ShouldShowEndTimeAuto(aptViewInfo);
			}
		}
		protected internal virtual bool ShouldShowRecurrence(IAppointmentViewInfo aptViewInfo) {
			return ShowAppointmentRecurrence && aptViewInfo.Appointment.IsRecurring;
		}
		protected internal virtual bool ShouldShowReminder(IAppointmentViewInfo aptViewInfo) {
			return ShowAppointmentReminder && aptViewInfo.Appointment.HasReminder;
		}
		protected internal virtual PercentCompleteDisplayType GetPercentCompleteDisplayType() {
			GanttViewAppointmentDisplayOptions options = AppointmentDisplayOptions as GanttViewAppointmentDisplayOptions;
			if (options == null)
				return PercentCompleteDisplayType.None;
			return options.PercentCompleteDisplayType;
		}
		protected internal virtual string GetLongDatePattern(bool stripYear) {
			string pattern = DateTimeFormatHelper.CurrentDateTimeFormat.LongDatePattern;
			pattern = DateTimeFormatHelper.StripDayOfWeek(pattern);
			pattern = pattern.Replace("MMMM", "MMM");
			if (stripYear)
				return DateTimeFormatHelper.StripYear(pattern);
			else
				return pattern.Replace("yyyy", "yy");
		}
		public virtual string GetStartTimeText(IAppointmentViewInfo aptViewInfo) {
			string timeText = GetCustomStartTimeText(aptViewInfo);
			if (String.IsNullOrEmpty(timeText))
				timeText = GetDefaultStartTimeText(aptViewInfo);
			return timeText;
		}
		public virtual string GetEndTimeText(IAppointmentViewInfo aptViewInfo) {
			string timeText = GetCustomEndTimeText(aptViewInfo);
			if (String.IsNullOrEmpty(timeText))
				timeText = GetDefaultEndTimeText(aptViewInfo);
			return timeText;
		}
		protected internal virtual string GetCustomStartTimeText(IAppointmentViewInfo aptViewInfo) {
			if (FormatProvider != null) {
				string format = GetStartTimeFormatString(aptViewInfo);
				if (!String.IsNullOrEmpty(format))
					return String.Format(CultureInfo.CurrentCulture, format, aptViewInfo.AppointmentInterval.Start);
			}
			return String.Empty;
		}
		protected internal virtual string GetCustomEndTimeText(IAppointmentViewInfo aptViewInfo) {
			if (FormatProvider != null) {
				string format = GetEndTimeFormatString(aptViewInfo); 
				if (!String.IsNullOrEmpty(format))
					return String.Format(CultureInfo.CurrentCulture, format, aptViewInfo.AppointmentInterval.End);
			}
			return String.Empty;
		}
		protected internal abstract bool ShouldShowStartTimeAuto(IAppointmentViewInfo aptViewInfo);
		protected internal abstract bool ShouldShowEndTimeAuto(IAppointmentViewInfo aptViewInfo);
		protected internal abstract bool ShouldShowStartTimeAlways(IAppointmentViewInfo aptViewInfo);
		protected internal abstract bool ShouldShowEndTimeAlways(IAppointmentViewInfo aptViewInfo);
		protected internal abstract bool ShouldShowTimeAsClock(IAppointmentViewInfo aptViewInfo);
		protected internal abstract string GetStartTimeFormatString(IAppointmentViewInfo aptViewInfo);
		protected internal abstract string GetEndTimeFormatString(IAppointmentViewInfo aptViewInfo);
		protected internal abstract string GetDefaultStartTimeText(IAppointmentViewInfo aptViewInfo);
		protected internal abstract string GetDefaultEndTimeText(IAppointmentViewInfo aptViewInfo);
	}
}
namespace DevExpress.XtraScheduler.Drawing.Internal {
	public class VerticalAppointmentContentCalculatorHelper : AppointmentContentCalculatorHelper {
		public VerticalAppointmentContentCalculatorHelper(ISupportAppointmentsBase viewInfo, AppointmentStatusDisplayType statusDisplayType, IAppointmentFormatStringService formatProvider)
			: base(viewInfo, statusDisplayType, formatProvider) {
		}
		protected internal override bool ShouldShowStartTimeAuto(IAppointmentViewInfo aptViewInfo) {
			return AppointmentDisplayOptions.SnapToCellsMode != AppointmentSnapToCellsMode.Always || !TimeInterval.Equals(aptViewInfo.Interval, aptViewInfo.AppointmentInterval);
		}
		protected internal override bool ShouldShowEndTimeAuto(IAppointmentViewInfo aptViewInfo) {
			return AppointmentDisplayOptions.SnapToCellsMode != AppointmentSnapToCellsMode.Always || !TimeInterval.Equals(aptViewInfo.Interval, aptViewInfo.AppointmentInterval);
		}
		protected internal override bool ShouldShowEndTimeAlways(IAppointmentViewInfo aptViewInfo) {
			return true;
		}
		protected internal override bool ShouldShowStartTimeAlways(IAppointmentViewInfo aptViewInfo) {
			return true;
		}
		protected internal override bool ShouldShowTimeAsClock(IAppointmentViewInfo aptViewInfo) {
			return false;
		}
		protected internal override string GetDefaultStartTimeText(IAppointmentViewInfo aptViewInfo) {
			string timeText = GetVerticalAppointmentTimeText(aptViewInfo.AppointmentInterval.Start, aptViewInfo.IsLongTime());
			if (aptViewInfo.Options.ShowEndTime)
				return String.Concat(timeText, "-");
			else {
				string startContinueText = SchedulerLocalizer.GetString(SchedulerStringId.Appointment_StartContinueText);
				return aptViewInfo.Options.ShowStartTime ? String.Format(startContinueText, timeText) : timeText;
			}
		}
		protected internal override string GetDefaultEndTimeText(IAppointmentViewInfo aptViewInfo) {
			string timeText = GetVerticalAppointmentTimeText(aptViewInfo.AppointmentInterval.End, aptViewInfo.IsLongTime());
			if (aptViewInfo.Options.ShowStartTime)
				return timeText;
			else {
				string endContinueText = SchedulerLocalizer.GetString(SchedulerStringId.Appointment_EndContinueText);
				return aptViewInfo.Options.ShowEndTime ? String.Format(endContinueText, timeText) : timeText;
			}
		}
		protected internal virtual string GetVerticalAppointmentTimeText(DateTime dateTime, bool isLongTimeApt) {
			string timeText = GetDefaultAppointmentTimeText(dateTime);
			if (isLongTimeApt) {
				string dateText = GetAppointmentDateText(dateTime);
				return String.Format("{0} {1}", dateText, timeText);
			}
			else
				return timeText;
		}
		protected internal virtual string GetAppointmentDateText(DateTime dateTime) {
			string datePattern = GetLongDatePattern(true);
			return dateTime.ToString(datePattern);
		}
		protected internal override string GetStartTimeFormatString(IAppointmentViewInfo aptViewInfo) {
			return FormatProvider.GetVerticalAppointmentStartFormat(aptViewInfo);
		}
		protected internal override string GetEndTimeFormatString(IAppointmentViewInfo aptViewInfo) {
			return FormatProvider.GetVerticalAppointmentEndFormat(aptViewInfo);
		}
	}
	public class HorizontalAppointmentContentCalculatorHelper : AppointmentContentCalculatorHelper {
		public HorizontalAppointmentContentCalculatorHelper(ISupportAppointmentsBase viewInfo, AppointmentStatusDisplayType statusDisplayType, IAppointmentFormatStringService formatProvider)
			: base(viewInfo, statusDisplayType, formatProvider) {
		}
		protected internal TimeInterval VisibleInterval { get { return ViewInfo.GetVisibleInterval(); } }
		public override void CalculateViewInfoOptions(IAppointmentViewInfo aptViewInfo) {
			base.CalculateViewInfoOptions(aptViewInfo);
			aptViewInfo.Options.StartContinueItemDisplayType = CalculateStartContinueItemDisplayType(aptViewInfo);
			aptViewInfo.Options.EndContinueItemDisplayType = CalculateEndContinueItemDisplayType(aptViewInfo);
		}
		protected internal AppointmentContinueArrowDisplayType CalculateStartContinueItemDisplayType(IAppointmentViewInfo aptViewInfo) {
			return ShouldShowStartContinueItems(aptViewInfo) ? AppointmentDisplayOptions.ContinueArrowDisplayType : AppointmentContinueArrowDisplayType.Never;
		}
		protected internal AppointmentContinueArrowDisplayType CalculateEndContinueItemDisplayType(IAppointmentViewInfo aptViewInfo) {
			return ShouldShowEndContinueItems(aptViewInfo) ? AppointmentDisplayOptions.ContinueArrowDisplayType : AppointmentContinueArrowDisplayType.Never;
		}
		protected internal override bool ShouldShowStartTimeAuto(IAppointmentViewInfo aptViewInfo) {
			if (aptViewInfo.IsLongTime())
				return aptViewInfo.HasLeftBorder && aptViewInfo.Interval.Start != aptViewInfo.AppointmentInterval.Start;
			else
				return true;
		}
		protected internal override bool ShouldShowEndTimeAuto(IAppointmentViewInfo aptViewInfo) {
			if (aptViewInfo.IsLongTime())
				return aptViewInfo.HasRightBorder && aptViewInfo.Interval.End != aptViewInfo.AppointmentInterval.End;
			else
				return true;
		}
		protected internal override bool ShouldShowStartTimeAlways(IAppointmentViewInfo aptViewInfo) {
			if (aptViewInfo.IsLongTime())
				return aptViewInfo.HasLeftBorder;
			else
				return true;
		}
		protected internal override bool ShouldShowEndTimeAlways(IAppointmentViewInfo aptViewInfo) {
			if (aptViewInfo.IsLongTime())
				return aptViewInfo.HasRightBorder;
			else
				return true;
		}
		protected internal override bool ShouldShowTimeAsClock(IAppointmentViewInfo aptViewInfo) {
			switch (AppointmentTimeDisplayType) {
				case AppointmentTimeDisplayType.Clock:
					return true;
				case AppointmentTimeDisplayType.Text:
					return false;
				default:
					return aptViewInfo.IsLongTime();
			}
		}
		protected internal virtual bool ShouldShowContinueTextItems(AppointmentContinueArrowDisplayType type) {
			return (type == AppointmentContinueArrowDisplayType.ArrowWithText) || (type == AppointmentContinueArrowDisplayType.Auto);
		}
		protected internal virtual bool ShouldShowStartContinueItems(IAppointmentViewInfo aptViewInfo) {
			if (AppointmentDisplayOptions.ContinueArrowDisplayType == AppointmentContinueArrowDisplayType.Never)
				return false;
			DateTime visibleStart = VisibleInterval.Start;
			return (aptViewInfo.Interval.Start == visibleStart) && (aptViewInfo.AppointmentInterval.Start < visibleStart);
		}
		protected internal virtual bool ShouldShowEndContinueItems(IAppointmentViewInfo aptViewInfo) {
			if (AppointmentDisplayOptions.ContinueArrowDisplayType == AppointmentContinueArrowDisplayType.Never)
				return false;
			DateTime visibleEnd = VisibleInterval.End;
			return (aptViewInfo.Interval.End == visibleEnd) && (aptViewInfo.AppointmentInterval.End > visibleEnd);
		}
		protected internal virtual string GetStartContinueItemText(IAppointmentViewInfo aptViewInfo) {
			string itemText = SchedulerLocalizer.GetString(SchedulerStringId.Appointment_StartContinueText);
			string timeText = GetStartContinueItemTimeText(aptViewInfo);
			return String.Format(itemText, timeText);
		}
		protected internal virtual string GetEndContinueItemText(IAppointmentViewInfo aptViewInfo) {
			string itemText = SchedulerLocalizer.GetString(SchedulerStringId.Appointment_EndContinueText);
			string timeText = GetEndContinueItemTimeText(aptViewInfo);
			return String.Format(itemText, timeText);
		}
		protected internal virtual string GetStartContinueItemTimeText(IAppointmentViewInfo aptViewInfo) {
			string timeText = GetCustomStartContinueItemTimeText(aptViewInfo);
			if (String.IsNullOrEmpty(timeText))
				timeText = GetDefaultStartContinueItemTimeText(aptViewInfo);
			return timeText;
		}
		protected internal virtual string GetEndContinueItemTimeText(IAppointmentViewInfo aptViewInfo) {
			DateTime end = CalculateEndContinueItemTime(aptViewInfo);
			string timeText = GetCustomEndContinueItemTimeText(aptViewInfo, end);
			if (String.IsNullOrEmpty(timeText))
				timeText = GetDefaultEndContinueItemTimeText(aptViewInfo, end);
			return timeText;
		}
		protected internal virtual DateTime CalculateEndContinueItemTime(IAppointmentViewInfo aptViewInfo) {
			DateTime end = aptViewInfo.AppointmentInterval.End;
			if (aptViewInfo.Appointment.AllDay)
				return end.AddDays(-1);
			else
				return end;
		}		
		protected internal virtual string GetCustomStartContinueItemTimeText(IAppointmentViewInfo aptViewInfo) {
			if (FormatProvider != null) {
				string format = FormatProvider.GetContinueItemStartFormat(aptViewInfo);
				if (!String.IsNullOrEmpty(format))
					return String.Format(CultureInfo.CurrentCulture, format, aptViewInfo.AppointmentInterval.Start);
			}
			return String.Empty;
		}
		protected internal virtual string GetCustomEndContinueItemTimeText(IAppointmentViewInfo aptViewInfo, DateTime endTime) {
			if (FormatProvider != null) {
				string format = FormatProvider.GetContinueItemEndFormat(aptViewInfo);
				if (!String.IsNullOrEmpty(format))
					return String.Format(CultureInfo.CurrentCulture, format, endTime);
			}
			return String.Empty;
		}
		protected internal virtual string GetDefaultStartContinueItemTimeText(IAppointmentViewInfo aptViewInfo) {
			DateTime date = aptViewInfo.AppointmentInterval.Start;
			bool stripYear = date.Year == VisibleInterval.Start.Year;
			string datePattern = GetLongDatePattern(stripYear);
			return date.ToString(datePattern);
		}
		protected internal virtual string GetDefaultEndContinueItemTimeText(IAppointmentViewInfo aptViewInfo, DateTime endTime) {
			DateTime date = endTime;
			bool stripYear = date.Year == VisibleInterval.End.Year;
			string datePattern = GetLongDatePattern(stripYear);
			return date.ToString(datePattern);
		}
		protected internal override string GetDefaultEndTimeText(IAppointmentViewInfo aptViewInfo) {
			return GetDefaultAppointmentTimeText(aptViewInfo.AppointmentInterval.End);
		}
		protected internal override string GetDefaultStartTimeText(IAppointmentViewInfo aptViewInfo) {
			return GetDefaultAppointmentTimeText(aptViewInfo.AppointmentInterval.Start);
		}
		protected internal override string GetStartTimeFormatString(IAppointmentViewInfo aptViewInfo) {
			return FormatProvider.GetHorizontalAppointmentStartFormat(aptViewInfo);
		}
		protected internal override string GetEndTimeFormatString(IAppointmentViewInfo aptViewInfo) {
			return FormatProvider.GetHorizontalAppointmentEndFormat(aptViewInfo);
		}
	}
	public class TimelineAppointmentContentLayoutCalculatorHelper : HorizontalAppointmentContentCalculatorHelper {
		public TimelineAppointmentContentLayoutCalculatorHelper(ISupportAppointmentsBase viewInfo, AppointmentStatusDisplayType statusDisplayType, IAppointmentFormatStringService formatProvider)
			: base(viewInfo, statusDisplayType, formatProvider) {
		}
		protected internal override bool ShouldShowStartTimeAuto(IAppointmentViewInfo aptViewInfo) {
			if (aptViewInfo.HasLeftBorder)
				return AppointmentDisplayOptions.SnapToCellsMode != AppointmentSnapToCellsMode.Always || (aptViewInfo.Interval.Start != aptViewInfo.Appointment.Start);
			else
				return false;
		}
		protected internal override bool ShouldShowEndTimeAuto(IAppointmentViewInfo aptViewInfo) {
			if (aptViewInfo.HasRightBorder)
				return AppointmentDisplayOptions.SnapToCellsMode != AppointmentSnapToCellsMode.Always || (aptViewInfo.Interval.End != aptViewInfo.Appointment.End);
			else
				return false;
		}
	}
}
