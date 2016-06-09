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
using System.Windows;
using System.Windows.Controls;
using System.Collections;
using DevExpress.XtraScheduler;
using System.ComponentModel;
using System.Windows.Data;
using DevExpress.Xpf.Scheduler.Drawing;
using DevExpress.Xpf.Core;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Native;
using System.Windows.Markup;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Internal;
namespace DevExpress.Xpf.Scheduler.UI {
	[DXToolboxBrowsable, ToolboxTabName(AssemblyInfo.DXTabWpfScheduling)]
	public partial class RecurrenceRangeControl : UserControl, INotifyPropertyChanged {
		public RecurrenceRangeControl() {
			InitializeComponent();
			MainElement.DataContext = this;
		}
		#region PatternRecurrenceInfo
		public IRecurrenceInfo RecurrenceInfo {
			get { return (IRecurrenceInfo)GetValue(RecurrenceInfoProperty); }
			set { SetValue(RecurrenceInfoProperty, value); }
		}
		public static readonly DependencyProperty RecurrenceInfoProperty = CreatePatternRecurrenceInfoProperty();
		static DependencyProperty CreatePatternRecurrenceInfoProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<RecurrenceRangeControl, IRecurrenceInfo>("RecurrenceInfo", null, (d, e) => d.OnPatternRecurrenceInfoChanged(e.OldValue, e.NewValue), null);
		}
		void OnPatternRecurrenceInfoChanged(IRecurrenceInfo oldValue, IRecurrenceInfo newValue) {
			UnsubscribeRecurrenceInfoEvents(oldValue);
			if (newValue != null) {
				SubscribeRecurrenceInfoEvents(newValue);
				if (oldValue != null)
					CopyValuesFromOldRecurrenceInfo(oldValue, newValue);
				SetRecurrenceRange(newValue.Range);
			}
			UpdateRangeControl();
		}
		void CopyValuesFromOldRecurrenceInfo(IRecurrenceInfo oldValue, IRecurrenceInfo newValue) {
			((IInternalRecurrenceInfo)newValue).UpdateRange(oldValue.Start, oldValue.End, oldValue.Range, oldValue.OccurrenceCount, Pattern);
		}
		#endregion
		#region TimeZoneHelper
		public TimeZoneHelper TimeZoneHelper {
			get { return (TimeZoneHelper)GetValue(TimeZoneHelperProperty); }
			set { SetValue(TimeZoneHelperProperty, value); }
		}
		public static readonly DependencyProperty TimeZoneHelperProperty = CreateTimeZoneHelperProperty();
		static DependencyProperty CreateTimeZoneHelperProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<RecurrenceRangeControl, TimeZoneHelper>("TimeZoneHelper", null, (d, e) => d.OnTimeZoneHelperChanged(e.OldValue, e.NewValue), null);
		}
		void OnTimeZoneHelperChanged(TimeZoneHelper oldTimeZoneHelper, TimeZoneHelper newTimeZoneHelper) {
			UpdateRangeControl();
		}
		#endregion
		#region Pattern
		public Appointment Pattern {
			get { return (Appointment)GetValue(PatternProperty); }
			set { SetValue(PatternProperty, value); }
		}
		public static readonly DependencyProperty PatternProperty = CreatePatternProperty();
		static DependencyProperty CreatePatternProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<RecurrenceRangeControl, Appointment>("Pattern", null);
		}
		#endregion
		#region IsReadOnly
		public bool IsReadOnly {
			get { return (bool)GetValue(IsReadOnlyProperty); }
			set { SetValue(IsReadOnlyProperty, value); }
		}
		public static readonly DependencyProperty IsReadOnlyProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<RecurrenceRangeControl, bool>("IsReadOnly", false);
		#endregion
		public DateTime LocalStart {
			get { return RecurrenceInfo != null ? ToClientTime(RecurrenceInfo.Start) : DateTime.MinValue; }
		}
		public DateTime LocalEnd {
			get { return RecurrenceInfo != null ? ToClientTime(RecurrenceInfo.End) : DateTime.MinValue; }
			set {
				if (RecurrenceInfo != null)
					RecurrenceInfo.End = FromClientTime(value);
			}
		}
		public IList RecurrenceRanges { get { return GetRecurrenceRanges(); } }
		void OnPatternRecurrenceInfoPropertyChanged(object sender, PropertyChangedEventArgs e) {
			UnsubscribeRecurrenceInfoEvents(RecurrenceInfo);
			try {
				((IInternalRecurrenceInfo)RecurrenceInfo).UpdateRange(RecurrenceInfo.Start, RecurrenceInfo.End, RecurrenceInfo.Range, RecurrenceInfo.OccurrenceCount, Pattern);
			} finally {
				SubscribeRecurrenceInfoEvents(RecurrenceInfo);
			}
			UpdateRangeControl();
		}
		protected internal virtual void SetRecurrenceRange(RecurrenceRange type) {
			switch (type) {
				default:
				case RecurrenceRange.EndByDate:
					EndByDateRadioButton.IsChecked = true;
					break;
				case RecurrenceRange.NoEndDate:
					NoEndDateRadioButton.IsChecked = true;
					break;
				case RecurrenceRange.OccurrenceCount:
					OccurenceCountRadioButton.IsChecked = true;
					break;
			}
		}
		void SubscribeRecurrenceInfoEvents(IRecurrenceInfo info) {
			if (info == null)
				return;
			info.PropertyChanged += OnPatternRecurrenceInfoPropertyChanged;
		}
		void UnsubscribeRecurrenceInfoEvents(IRecurrenceInfo info) {
			if (info == null)
				return;
			info.PropertyChanged -= OnPatternRecurrenceInfoPropertyChanged;
		}
		protected internal virtual DateTime ToClientTime(DateTime date) {
			if (TimeZoneHelper != null)
				return TimeZoneHelper.ToClientTime(date, RecurrenceInfo.TimeZoneId);
			else
				return date;
		}
		protected internal virtual DateTime FromClientTime(DateTime date) {
			if (TimeZoneHelper != null)
				return TimeZoneHelper.FromClientTime(date, RecurrenceInfo.TimeZoneId);
			else
				return date;
		}
		protected virtual IList GetRecurrenceRanges() {
			NamedElementList result = new NamedElementList();
			result.Add(new NamedElement(RecurrenceRange.NoEndDate, SchedulerControlLocalizer.GetString(SchedulerControlStringId.Form_NoEndDate), this));
			result.Add(new NamedElement(RecurrenceRange.OccurrenceCount, SchedulerControlLocalizer.GetString(SchedulerControlStringId.Form_EndAfter), this));
			result.Add(new NamedElement(RecurrenceRange.EndByDate, SchedulerControlLocalizer.GetString(SchedulerControlStringId.Form_EndBy), this));
			return result;
		}
		#region INotifyPropertyChanged Members
		PropertyChangedEventHandler onPropertyChanged;
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged {
			add { onPropertyChanged += value; }
			remove { onPropertyChanged -= value; }
		}
		protected virtual void RaiseOnPropertyChanged(string propertyName) {
			if (onPropertyChanged != null)
				onPropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
		protected virtual void UpdateRangeControl() {
			RaiseOnPropertyChanged("LocalStart");
			RaiseOnPropertyChanged("LocalEnd");
		}
		void OnOccurrenceCountSpinEditGotFocus(object sender, RoutedEventArgs e) {
			if (RecurrenceInfo == null)
				return;
			RecurrenceInfo.Range = RecurrenceRange.OccurrenceCount;
			if (!IsReadOnly)
				OccurenceCountRadioButton.IsChecked = true;
		}
		void OnEndByDateEditGotFocus(object sender, RoutedEventArgs e) {
			if (RecurrenceInfo == null)
				return;
			RecurrenceInfo.Range = RecurrenceRange.EndByDate;
			if (!IsReadOnly)
				EndByDateRadioButton.IsChecked = true;
		}
		private void OnEdtEndDateValidate(object sender, Editors.ValidationEventArgs e) {
			if (e.Value == null)
				return;
			e.IsValid = (DateTime)e.Value > LocalStart;
			e.ErrorContent = SchedulerLocalizer.GetString(SchedulerStringId.Msg_InvalidEndDate);
		}
		public class RecurrenceRangeTemplateSelector : DataTemplateSelector {
			public DataTemplate NoEndDateTemplate { get; set; }
			public DataTemplate OccurenceCountTemplate { get; set; }
			public DataTemplate EndByDateTemplate { get; set; }
			public override DataTemplate SelectTemplate(object item, DependencyObject container) {
				NamedElement el = item as NamedElement;
				if (el == null)
					return null;
				RecurrenceRange range = (RecurrenceRange)el.Id;
				switch (range) {
					case RecurrenceRange.NoEndDate:
						return NoEndDateTemplate;
					case RecurrenceRange.OccurrenceCount:
						return OccurenceCountTemplate;
					case RecurrenceRange.EndByDate:
						return EndByDateTemplate;
				}
				return null;
			}
		}
	}
	public class RecurrenceRangeTypeConverter : MarkupExtension, IValueConverter {
		static RecurrenceRangeTypeConverter instance;
		static RecurrenceRangeTypeConverter Instance {
			get {
				if (instance == null)
					instance = new RecurrenceRangeTypeConverter();
				return instance;
			}
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return Instance;
		}
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			RecurrenceRange rangeType = (RecurrenceRange)parameter;
			return rangeType == (RecurrenceRange)value;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			RecurrenceRange rangeType = (RecurrenceRange)parameter;
			return rangeType;
		}
	}
}
