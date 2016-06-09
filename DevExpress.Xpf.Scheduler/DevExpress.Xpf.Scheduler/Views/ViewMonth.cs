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
using System.Windows;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using  DevExpress.Xpf.Scheduler.Native;
using System.Windows.Controls;
using DevExpress.Xpf.Scheduler.Drawing;
using DevExpress.Xpf.Core.Native;
using DependencyPropertyHelper = DevExpress.Xpf.Core.Native.DependencyPropertyHelper;
using DevExpress.Utils.Serializing;
using System.Windows.Data;
using DevExpress.Xpf.Scheduler.ThemeKeys;
#if SL
using DevExpress.Xpf.Core;
#endif
namespace DevExpress.Xpf.Scheduler {
	#region MonthView
	public class MonthView : WeekViewBase, IMonthViewProperties {
		static MonthView() {
		}
		public MonthView() {
#if SL
			InitializePropertiesWithDefaultValues();			
#endif
		}
		#region Properties
		protected internal new InnerMonthView InnerView { get { return (InnerMonthView)base.InnerView; } }
		#region Type
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override SchedulerViewType Type { get { return SchedulerViewType.Month; } }
		#endregion
		#region AppointmetDisplayOptions
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("MonthViewAppointmentDisplayOptions")]
#endif
		public SchedulerMonthViewAppointmentDisplayOptions AppointmentDisplayOptions {
			get { return (SchedulerMonthViewAppointmentDisplayOptions)GetValue(AppointmentDisplayOptionsProperty); }
			set { SetValue(AppointmentDisplayOptionsProperty, value); }
		}
		public static readonly DependencyProperty AppointmentDisplayOptionsProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<MonthView, SchedulerMonthViewAppointmentDisplayOptions>("AppointmentDisplayOptions", null, (d, e) => d.UpdateInnerObjectPropertyValue(AppointmentDisplayOptionsProperty, e.OldValue, e.NewValue), null);
		#endregion       
		#region CompressWeekend
		public static readonly DependencyProperty CompressWeekendProperty = CreateCompressWeekendProperty();
		static DependencyProperty CreateCompressWeekendProperty() {
			return DependencyPropertyHelper.RegisterProperty<MonthView, bool>("CompressWeekend", InnerMonthView.defaultCompressWeekend, (d, e) => d.OnCompressWeekendChanged(e.OldValue, e.NewValue));
		}
		void OnCompressWeekendChanged(bool oldValue, bool newValue) {
			UpdateInnerObjectPropertyValue(CompressWeekendProperty, oldValue, newValue);
		}
		[
#if !SL
	DevExpressXpfSchedulerLocalizedDescription("MonthViewCompressWeekend"),
#endif
		DefaultValue(InnerMonthView.defaultCompressWeekend), XtraSerializableProperty(), NotifyParentProperty(true)]
		public bool CompressWeekend {
			get { return (bool)GetValue(CompressWeekendProperty); }
			set { SetValue(CompressWeekendProperty, value); }
		}
		#endregion
		#region ShowWeekend
		public static readonly DependencyProperty ShowWeekendProperty = CreateShowWeekendProperty();
		static DependencyProperty CreateShowWeekendProperty() {
			return DependencyPropertyHelper.RegisterProperty<MonthView, bool>("ShowWeekend", InnerMonthView.defaultShowWeekend, (d, e) => d.OnShowWeekendChanged(e.OldValue, e.NewValue));
		}
		void OnShowWeekendChanged(bool oldValue, bool newValue) {
			UpdateInnerObjectPropertyValue(ShowWeekendProperty, oldValue, newValue);
		}		
		[
#if !SL
	DevExpressXpfSchedulerLocalizedDescription("MonthViewShowWeekend"),
#endif
		DefaultValue(InnerMonthView.defaultShowWeekend), XtraSerializableProperty(), NotifyParentProperty(true)]
		public bool ShowWeekend {
			get { return (bool)GetValue(ShowWeekendProperty); }
			set { SetValue(ShowWeekendProperty, value); }
		}
		#endregion
		#region WeekCount
		public static readonly DependencyProperty WeekCountProperty = CreateWeekCountProperty();
		static DependencyProperty CreateWeekCountProperty() {
			return DependencyPropertyHelper.RegisterProperty<MonthView, int>("WeekCount", InnerMonthView.defaultWeekCount, (d, e) => d.OnWeekCountChanged(e.OldValue, e.NewValue));
		}
		void OnWeekCountChanged(int oldValue, int newValue) {
			UpdateInnerObjectPropertyValue(WeekCountProperty, oldValue, newValue);
		}
		[
#if !SL
	DevExpressXpfSchedulerLocalizedDescription("MonthViewWeekCount"),
#endif
		DefaultValue(InnerMonthView.defaultWeekCount), XtraSerializableProperty(), NotifyParentProperty(true)]
		public int WeekCount {
			get { return (int)GetValue(WeekCountProperty); }
			set { SetValue(WeekCountProperty, value); }
		}
		#endregion
		#region DeferredScrolling
		ISchedulerDeferredScrollingOption ISchedulerViewPropertiesBase.DeferredScrolling { get { return DeferredScrolling; } set { DeferredScrolling = (SchedulerDeferredScrollingOption)value; } }
		public SchedulerDeferredScrollingOption DeferredScrolling {
			get { return (SchedulerDeferredScrollingOption)GetValue(DeferredScrollingProperty); }
			set { SetValue(DeferredScrollingProperty, value); }
		}
		public static readonly DependencyProperty DeferredScrollingProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<MonthView, SchedulerDeferredScrollingOption>("DeferredScrolling", null, (d, e) => d.OnDeferredScrollingChanged(e.OldValue, e.NewValue), null);
		void OnDeferredScrollingChanged(SchedulerDeferredScrollingOption oldValue, SchedulerDeferredScrollingOption newValue) {
		}
		#endregion
		#endregion
		protected override void InitializePropertiesWithDefaultValues() {
			if (AppointmentDisplayOptions == null)
				AppointmentDisplayOptions = new SchedulerMonthViewAppointmentDisplayOptions();
			if (DeferredScrolling == null)
				DeferredScrolling = new SchedulerDeferredScrollingOption();
		}
		protected override DependencyPropertySyncManager CreatePropertySyncManager() {
			return new MonthViewPropertySyncManager(this);
		}
		protected internal override InnerSchedulerViewBase CreateInnerView() {
			return new InnerMonthView(this,  this);
		}
		protected internal override AppointmentDisplayOptions CreateAppointmentDisplayOptionsCore() {
			return new MonthViewAppointmentDisplayOptions();
		}
		protected internal override ViewFactoryHelper CreateFactoryHelper() {
			return new MonthViewFactoryHelper();
		}
		protected internal override ViewDateTimeScrollController CreateDateTimeScrollController() {
			return new MonthViewDateTimeScrollController(this);
		}
		protected override bool ValidateAppointmentDisplayOptions(SchedulerAppointmentDisplayOptions appointmentDisplayOptions) {
			return appointmentDisplayOptions is SchedulerMonthViewAppointmentDisplayOptions;
		}
		protected override SchedulerAppointmentDisplayOptions GetAppointmentDisplayOptions() {
			return this.AppointmentDisplayOptions;
		}
	}
	#endregion
}
namespace DevExpress.Xpf.Scheduler.Drawing {
	#region MonthViewContentStyleSelector
	public class MonthViewContentStyleSelector : ViewContentStyleSelector {
		protected internal override Type GroupByNoneType { get { return typeof(VisualMonthViewGroupByNone); } }
		protected internal override Type GroupByDateType { get { return typeof(VisualMonthViewGroupByDate); } }
		protected internal override Type GroupByResourceType { get { return typeof(VisualMonthViewGroupByResource); } }
	}
	#endregion
}
