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
using System.Windows.Controls;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using DevExpress.Xpf.Scheduler.Native;
using DevExpress.Xpf.Scheduler.Drawing;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Scheduler.ThemeKeys;
namespace DevExpress.Xpf.Scheduler {
	#region WeekViewBase
	public abstract class WeekViewBase : SchedulerViewBase {
		#region Properties
		protected internal new InnerWeekView InnerView {
			get {
				return (InnerWeekView)base.InnerView;
			}
		}
		#region MoreButtonStyle
		public static readonly DependencyProperty MoreButtonStyleProperty = CreateMoreButtonStyleProperty();
		static DependencyProperty CreateMoreButtonStyleProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<WeekViewBase, Style>("MoreButtonStyle", null);
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("WeekViewBaseMoreButtonStyle")]
#endif
		public Style MoreButtonStyle {
			get {
				return (Style)GetValue(MoreButtonStyleProperty);
			}
			set {
				SetValue(MoreButtonStyleProperty, value);
			}
		}
		#endregion
		#region DayOfWeekHeaderStyle
		public static readonly DependencyProperty DayOfWeekHeaderStyleProperty = CreateDayOfWeekHeaderStyleProperty();
		static DependencyProperty CreateDayOfWeekHeaderStyleProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<WeekViewBase, Style>("DayOfWeekHeaderStyle", null);
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("WeekViewBaseDayOfWeekHeaderStyle")]
#endif
		public Style DayOfWeekHeaderStyle {
			get {
				return (Style)GetValue(DayOfWeekHeaderStyleProperty);
			}
			set {
				SetValue(DayOfWeekHeaderStyleProperty, value);
			}
		}
		#endregion
		#region HorizontalWeekDateHeaderStyle
		public static readonly DependencyProperty HorizontalWeekDateHeaderStyleProperty = CreateHorizontalWeekDateHeaderStyleProperty();
		static DependencyProperty CreateHorizontalWeekDateHeaderStyleProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<WeekViewBase, Style>("HorizontalWeekDateHeaderStyle", null);
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("WeekViewBaseHorizontalWeekDateHeaderStyle")]
#endif
		public Style HorizontalWeekDateHeaderStyle {
			get {
				return (Style)GetValue(HorizontalWeekDateHeaderStyleProperty);
			}
			set {
				SetValue(HorizontalWeekDateHeaderStyleProperty, value);
			}
		}
		#endregion
		#region HorizontalWeekCellStyle
		public static readonly DependencyProperty HorizontalWeekCellStyleProperty = CreateHorizontalWeekCellStyleProperty();
		static DependencyProperty CreateHorizontalWeekCellStyleProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<WeekViewBase, Style>("HorizontalWeekCellStyle", null);
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("WeekViewBaseHorizontalWeekCellStyle")]
#endif
		public Style HorizontalWeekCellStyle {
			get {
				return (Style)GetValue(HorizontalWeekCellStyleProperty);
			}
			set {
				SetValue(HorizontalWeekCellStyleProperty, value);
			}
		}
		#endregion
		#endregion
	}
	#endregion
	#region WeekView
	public class WeekView : WeekViewBase, IWeekViewProperties {
		static WeekView() {
		}
		#region Properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override SchedulerViewType Type {
			get {
				return SchedulerViewType.Week;
			}
		}
		#region VerticalWeekDateHeaderStyle
		public static readonly DependencyProperty VerticalWeekDateHeaderStyleProperty = CreateVerticalWeekDateHeaderStyleProperty();
		static DependencyProperty CreateVerticalWeekDateHeaderStyleProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<WeekView, Style>("VerticalWeekDateHeaderStyle", null);
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("WeekViewVerticalWeekDateHeaderStyle")]
#endif
		public Style VerticalWeekDateHeaderStyle {
			get {
				return (Style)GetValue(VerticalWeekDateHeaderStyleProperty);
			}
			set {
				SetValue(VerticalWeekDateHeaderStyleProperty, value);
			}
		}
		#endregion
		#region VerticalWeekCellStyle
		public static readonly DependencyProperty VerticalWeekCellStyleProperty = CreateVerticalWeekCellStyleProperty();
		static DependencyProperty CreateVerticalWeekCellStyleProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<WeekView, Style>("VerticalWeekCellStyle", null);
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("WeekViewVerticalWeekCellStyle")]
#endif
		public Style VerticalWeekCellStyle {
			get {
				return (Style)GetValue(VerticalWeekCellStyleProperty);
			}
			set {
				SetValue(VerticalWeekCellStyleProperty, value);
			}
		}
		#endregion
		#region AppointmetDisplayOptions
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("WeekViewAppointmentDisplayOptions")]
#endif
		public SchedulerWeekViewAppointmentDisplayOptions AppointmentDisplayOptions {
			get {
				return (SchedulerWeekViewAppointmentDisplayOptions)GetValue(AppointmentDisplayOptionsProperty);
			}
			set {
				SetValue(AppointmentDisplayOptionsProperty, value);
			}
		}
		public static readonly DependencyProperty AppointmentDisplayOptionsProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<WeekView, SchedulerWeekViewAppointmentDisplayOptions>("AppointmentDisplayOptions", null, (d, e) => d.UpdateInnerObjectPropertyValue(AppointmentDisplayOptionsProperty, e.OldValue, e.NewValue), null);
		#endregion
		#region DeferredScrolling
		ISchedulerDeferredScrollingOption ISchedulerViewPropertiesBase.DeferredScrolling { get { return DeferredScrolling; } set { DeferredScrolling = (SchedulerDeferredScrollingOption)value; } }
		public SchedulerDeferredScrollingOption DeferredScrolling {
			get { return (SchedulerDeferredScrollingOption)GetValue(DeferredScrollingProperty); }
			set { SetValue(DeferredScrollingProperty, value); }
		}
		public static readonly DependencyProperty DeferredScrollingProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<WeekView, SchedulerDeferredScrollingOption>("DeferredScrolling", null, (d, e) => d.OnDeferredScrollingChanged(e.OldValue, e.NewValue), null);
		void OnDeferredScrollingChanged(SchedulerDeferredScrollingOption oldValue, SchedulerDeferredScrollingOption newValue) {
		}
		#endregion
		#endregion
		protected override void InitializePropertiesWithDefaultValues() {
			if (AppointmentDisplayOptions == null)
				AppointmentDisplayOptions = new SchedulerWeekViewAppointmentDisplayOptions();
			if (DeferredScrolling == null)
				DeferredScrolling = new SchedulerDeferredScrollingOption();
		}
		protected override DependencyPropertySyncManager CreatePropertySyncManager() {
			return new WeekViewPropertySyncManager(this);
		}
		protected internal override InnerSchedulerViewBase CreateInnerView() {
			return new InnerWeekView(this,  this);
		}
		protected internal override AppointmentDisplayOptions CreateAppointmentDisplayOptionsCore() {
			return new WeekViewAppointmentDisplayOptions();
		}
		protected internal override ViewFactoryHelper CreateFactoryHelper() {
			return new WeekViewFactoryHelper();
		}
		protected internal override ViewDateTimeScrollController CreateDateTimeScrollController() {
			return new WeekViewDateTimeScrollController(this);
		}
		protected override bool ValidateAppointmentDisplayOptions(SchedulerAppointmentDisplayOptions appointmentDisplayOptions) {
			return appointmentDisplayOptions is SchedulerWeekViewAppointmentDisplayOptions;
		}
		protected override SchedulerAppointmentDisplayOptions GetAppointmentDisplayOptions() {
			return this.AppointmentDisplayOptions;
		}
	}
	#endregion
}
namespace DevExpress.Xpf.Scheduler.Drawing {
	#region WeekViewContentStyleSelector
	public class WeekViewContentStyleSelector : ViewContentStyleSelector {
		protected internal override Type GroupByNoneType { get { return typeof(VisualWeekViewGroupByNone); } }
		protected internal override Type GroupByDateType { get { return typeof(VisualWeekViewGroupByDate); } }
		protected internal override Type GroupByResourceType { get { return typeof(VisualWeekViewGroupByResource); } }
	}
	#endregion
}
