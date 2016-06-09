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
using System.Web.UI;
using System.ComponentModel;
using DevExpress.Utils.Serializing;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using DevExpress.Web.ASPxScheduler.Drawing;
using DevExpress.Web.ASPxScheduler.Internal;
using DevExpress.Web.ASPxScheduler.Rendering;
using DevExpress.Utils.Serializing.Helpers;
namespace DevExpress.Web.ASPxScheduler {
	#region DayView
	public class DayView : SchedulerViewBase, IXtraSupportDeserializeCollectionItem {
		TimeSpan topRowTime;
		TimeIndicatorDisplayOptions timeIndicatorDisplayOptions;
		public DayView(ASPxScheduler control)
			: base(control) {
			ShouldSerializeHelper.RegisterXtraShouldSerializeMethod("VisibleTime", XtraShouldSerializeVisibleTime);
			ShouldSerializeHelper.RegisterXtraShouldSerializeMethod("WorkTime", XtraShouldSerializeWorkTime);
			ShouldSerializeHelper.RegisterXtraShouldSerializeMethod("TimeSlots", XtraShouldSerializeTimeSlots);
			ShouldSerializeHelper.RegisterXtraShouldSerializeMethod("TimeScale", XtraShouldSerializeTimeScale);
			this.timeIndicatorDisplayOptions = CreateTimeIndicatorDisplayOptions();
		}
		#region Properties
		#region TopRowTime
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TimeSpan TopRowTime { get { return topRowTime; } set { topRowTime = value; } }
		#endregion
		#region ShowWorkTimeOnly
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("DayViewShowWorkTimeOnly"),
#endif
DefaultValue(InnerDayView.DefaultShowWorkTimeOnly), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatEnable()]
		public bool ShowWorkTimeOnly {
			get {
				if(InnerView != null) {
					InnerDayView innerView = (InnerDayView)InnerView;
					return innerView.ShowWorkTimeOnly;
				}
				else
					return false;
			}
			set {
				if(InnerView != null) {
					InnerDayView innerView = (InnerDayView)InnerView;
					innerView.ShowWorkTimeOnly = value;
				}
			}
		}
		#endregion
		#region ShowDayHeaders
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("DayViewShowDayHeaders"),
#endif
DefaultValue(InnerDayView.DefaultShowDayHeaders), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatEnable]
		public bool ShowDayHeaders {
			get {
				if(InnerView != null) {
					InnerDayView innerView = (InnerDayView)InnerView;
					return innerView.ShowDayHeaders;
				}
				else
					return false;
			}
			set {
				if(InnerView != null) {
					InnerDayView innerView = (InnerDayView)InnerView;
					innerView.ShowDayHeaders = value;
				}
			}
		}
		#endregion
		#region ShowMoreButtonsOnEachColumn
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("DayViewShowMoreButtonsOnEachColumn"),
#endif
DefaultValue(InnerDayView.DefaultShowMoreButtonsOnEachColumn), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatEnable]
		public bool ShowMoreButtonsOnEachColumn {
			get {
				if(InnerView != null) {
					InnerDayView innerView = (InnerDayView)InnerView;
					return innerView.ShowMoreButtonsOnEachColumn;
				}
				else
					return false;
			}
			set {
				if(InnerView != null) {
					InnerDayView innerView = (InnerDayView)InnerView;
					innerView.ShowMoreButtonsOnEachColumn = value;
				}
			}
		}
		#endregion
		#region ShowAllDayArea
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("DayViewShowAllDayArea"),
#endif
DefaultValue(InnerDayView.DefaultShowAllDayArea), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatEnable]
		public bool ShowAllDayArea {
			get {
				if(InnerView != null) {
					InnerDayView innerView = (InnerDayView)InnerView;
					return innerView.ShowAllDayArea;
				}
				else
					return false;
			}
			set {
				if(InnerView != null) {
					InnerDayView innerView = (InnerDayView)InnerView;
					innerView.ShowAllDayArea = value;
				}
			}
		}
		#endregion
		#region ShowAllDayAppointmentsAtTimeCell
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("DayViewShowAllAppointmentsAtTimeCells"),
#endif
DefaultValue(InnerDayView.DefaultShowAllAppointmentsAtTimeCells), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatEnable]
		public bool ShowAllAppointmentsAtTimeCells {
			get {
				if(InnerView != null) {
					InnerDayView innerView = (InnerDayView)InnerView;
					return innerView.ShowAllAppointmentsAtTimeCells;
				}
				else
					return false;
			}
			set {
				if(InnerView != null) {
					InnerDayView innerView = (InnerDayView)InnerView;
					innerView.ShowAllAppointmentsAtTimeCells = value;
				}
			}
		}
		#endregion
		#region ActualShowAllAppointmentsAtTimeCells
		protected internal bool ActualShowAllAppointmentsAtTimeCells {
			get { return ((InnerDayView)InnerView).ActualShowAllAppointmentsAtTimeCells; }
		}
		#endregion
		#region VisibleTime
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("DayViewVisibleTime"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public TimeOfDayInterval VisibleTime {
			get {
				if(InnerView != null) {
					InnerDayView innerView = (InnerDayView)InnerView;
					return innerView.VisibleTime;
				}
				else
					return null;
			}
			set {
				if(InnerView != null) {
					InnerDayView innerView = (InnerDayView)InnerView;
					innerView.VisibleTime = value;
				}
			}
		}
		internal bool ShouldSerializeVisibleTime() {
			return !VisibleTime.IsEqual(InnerDayView.defaultVisibleTime);
		}
		internal void ResetVisibleTime() {
			VisibleTime = InnerDayView.defaultVisibleTime;
		}
		internal bool XtraShouldSerializeVisibleTime() {
			return ShouldSerializeVisibleTime();
		}
		#endregion
		#region WorkTime
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("DayViewWorkTime"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public TimeOfDayInterval WorkTime {
			get {
				if(InnerView != null) {
					InnerDayView innerView = (InnerDayView)InnerView;
					return innerView.WorkTime;
				}
				else
					return null;
			}
			set {
				if(InnerView != null) {
					InnerDayView innerView = (InnerDayView)InnerView;
					innerView.WorkTime = value;
				}
			}
		}
		internal bool ShouldSerializeWorkTime() {
			return !WorkTime.IsEqual(InnerDayView.defaultWorkTime);
		}
		internal void ResetWorkTime() {
			WorkTime = InnerDayView.defaultWorkTime;
		}
		internal bool XtraShouldSerializeWorkTime() {
			return ShouldSerializeWorkTime();
		}
		#endregion
		#region TimeScale
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("DayViewTimeScale"),
#endif
XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatEnable]
		public TimeSpan TimeScale {
			get {
				if(InnerView != null) {
					InnerDayView innerView = (InnerDayView)InnerView;
					return innerView.TimeScale;
				}
				else
					return TimeSpan.Zero;
			}
			set {
				if(InnerView != null) {
					InnerDayView innerView = (InnerDayView)InnerView;
					innerView.TimeScale = value;
				}
			}
		}
		internal bool ShouldSerializeTimeScale() {
			return TimeScale != InnerDayView.defaultTimeScale;
		}
		internal bool XtraShouldSerializeTimeScale() {
			return ShouldSerializeTimeScale();
		}
		internal void ResetTimeScale() {
			TimeScale = InnerDayView.defaultTimeScale;
		}
		#endregion
		#region TimeSlots
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("DayViewTimeSlots"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true), AutoFormatDisable()]
		public TimeSlotCollection TimeSlots {
			get {
				if(InnerView != null) {
					InnerDayView innerView = (InnerDayView)InnerView;
					return innerView.TimeSlots;
				}
				else
					return null;
			}
		}
		internal bool ShouldSerializeTimeSlots() {
			return !TimeSlots.HasDefaultContent();
		}
		internal void ResetTimeSlots() {
			TimeSlots.LoadDefaults();
		}
		internal bool XtraShouldSerializeTimeSlots() {
			return ShouldSerializeTimeSlots();
		}
		internal object XtraCreateTimeSlotsItem(XtraItemEventArgs e) {
			InnerDayView innerView = (InnerDayView)InnerView;
			if (innerView != null)
				return innerView.XtraCreateTimeSlotsItem(e);
			else
				return null;
		}
		internal void XtraSetIndexTimeSlotsItem(XtraSetItemIndexEventArgs e) {
			InnerDayView innerView = (InnerDayView)InnerView;
			if (innerView != null)
				innerView.XtraSetIndexTimeSlotsItem(e);
		}
		#endregion
		#region TimeRulers
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("DayViewTimeRulers"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true), AutoFormatEnable()]
		public TimeRulerCollection TimeRulers {
			get {
				if(InnerView != null) {
					InnerDayView innerView = (InnerDayView)InnerView;
					return innerView.TimeRulers;
				}
				else
					return null;
			}
		}
		internal object XtraCreateTimeRulersItem(XtraItemEventArgs e) {
			InnerDayView innerView = (InnerDayView)InnerView;
			if (innerView != null)
				return innerView.XtraCreateTimeRulersItem(e);
			else
				return null;
		}
		internal void XtraSetIndexTimeRulersItem(XtraSetItemIndexEventArgs e) {
			InnerDayView innerView = (InnerDayView)InnerView;
			if (innerView != null)
				innerView.XtraSetIndexTimeRulersItem(e);
		}
		#endregion
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override SchedulerViewType Type { get { return SchedulerViewType.Day; } }
		protected internal new InnerDayView InnerView { get { return (InnerDayView)base.InnerView; } }
		protected internal TimeOfDayInterval ActualVisibleTime {
			get { return ShowWorkTimeOnly ? (TimeOfDayInterval)WorkTime.Clone() : VisibleTime; }
		}
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("DayViewDayCount"),
#endif
		DefaultValue(InnerDayView.DefaultDayCount), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatEnable()]
		public virtual int DayCount {
			get {
				if(InnerView != null) {
					InnerDayView innerView = (InnerDayView)InnerView;
					return innerView.DayCount;
				}
				else
					return 0;
			}
			set {
				if(InnerView != null) {
					InnerDayView innerView = (InnerDayView)InnerView;
					innerView.DayCount = value;
				}
			}
		}
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("DayViewAppointmentDisplayOptions"),
#endif
Category(SRCategoryNames.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]
		public DayViewAppointmentDisplayOptions AppointmentDisplayOptions { get { return (DayViewAppointmentDisplayOptions)base.AppointmentDisplayOptionsInternal; } }
		protected internal virtual bool ShowExtendedCells { get { return false; } }
		protected internal virtual int MinimumExtendedCellsInColumn { get { return 3; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), AutoFormatDisable()]
		public override string MoreButtonHTML { get { return base.MoreButtonHTML; } set { base.MoreButtonHTML = value; } }
		#region Styles
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("DayViewStyles"),
#endif
Category("Styles"), NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DayViewStyles Styles { get { return (DayViewStyles)InnerStyles; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual DayViewStyles DayViewStyles { get { return this.Styles; } }
		#endregion
		#region Templates
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public DayViewTemplates Templates { get { return (DayViewTemplates)InnerTemplates; } }
		#endregion
		#region TimeIndicatorDisplayOptions
		[
		Category(SRCategoryNames.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]
		public TimeIndicatorDisplayOptions TimeIndicatorDisplayOptions {
			get {
				return timeIndicatorDisplayOptions;
			}
		}
		#endregion
		#region TimeMarkerVisibility
		[
		DefaultValue(InnerDayView.DefaultTimeMarkerVisibility), XtraSerializableProperty()]
		public TimeMarkerVisibility TimeMarkerVisibility {
			get {
				InnerDayView innerView = (InnerDayView)InnerView;
				if (innerView != null)
					return innerView.TimeMarkerVisibility;
				else
					return InnerDayView.DefaultTimeMarkerVisibility;
			}
			set {
				InnerDayView innerView = (InnerDayView)InnerView;
				if (innerView != null)
					innerView.TimeMarkerVisibility = value;
			}
		}
		#endregion
		#endregion
		#region Events
		#region TopRowTimeChanged
		ChangeEventHandler onTopRowTimeChanged;
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("DayViewTopRowTimeChanged")]
#endif
		public event ChangeEventHandler TopRowTimeChanged {
			add { onTopRowTimeChanged += value; }
			remove { onTopRowTimeChanged -= value; }
		}
		protected internal virtual void RaiseTopRowTimeChanged(ChangeEventArgs e) {
			if (onTopRowTimeChanged != null)
				onTopRowTimeChanged(this, e);
		}
		#endregion
		#region VisibleRowCountChanged
		ChangeEventHandler onVisibleRowCountChanged;
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("DayViewVisibleRowCountChanged")]
#endif
		public event ChangeEventHandler VisibleRowCountChanged {
			add { onVisibleRowCountChanged += value; }
			remove { onVisibleRowCountChanged -= value; }
		}
		protected internal virtual void RaiseVisibleRowCountChanged(ChangeEventArgs e) {
			if (onVisibleRowCountChanged != null)
				onVisibleRowCountChanged(this, e);
		}
		#endregion
		#endregion
		protected virtual TimeIndicatorDisplayOptions CreateTimeIndicatorDisplayOptions() {
			return new TimeIndicatorDisplayOptions();
		}
		protected internal override InnerSchedulerViewBase CreateInnerView() {
			return new InnerDayView(this, new DayViewProperties());
		}
		protected internal override AppointmentDisplayOptions CreateAppointmentDisplayOptionsCore() {
			return new DayViewAppointmentDisplayOptions();
		}
		protected internal override ViewFactoryHelper CreateFactoryHelper() {
			return new DayViewFactoryHelper();
		}
		protected internal virtual DateTime CalculateActualDate(DateTime dateTime) {
			InnerDayView innerView = (InnerDayView)InnerView;
			return innerView.CalculateActualDate(dateTime);
		}
		protected internal override ASPxSchedulerStylesBase CreateStyles() {
			return new DayViewStyles(Control);
		}
		protected internal override SchedulerTemplates CreateTemplates() {
			return new DayViewTemplates();
		}
		public override void Assign(SchedulerViewBase source) {
			base.Assign(source);
			DayView dayView = source as DayView;
			if (dayView != null) {
				Styles.CopyFrom(dayView.Styles);
				DayViewStyles.CopyFrom(dayView.DayViewStyles);
				TimeSlots.Assign(dayView.TimeSlots);
				VisibleTime.Assign(dayView.VisibleTime);
				AppointmentDisplayOptions.Assign(dayView.AppointmentDisplayOptions);
				WorkTime.Assign(dayView.WorkTime);
				DayCount = dayView.DayCount;
				ShowAllAppointmentsAtTimeCells = dayView.ShowAllAppointmentsAtTimeCells;
				ShowAllDayArea = dayView.ShowAllDayArea;
				ShowDayHeaders = dayView.ShowDayHeaders;
				ShowMoreButtonsOnEachColumn = dayView.ShowMoreButtonsOnEachColumn;
				ShowWorkTimeOnly = dayView.ShowWorkTimeOnly;
				TimeScale = dayView.TimeScale;
				TopRowTime = dayView.TopRowTime;
				NavigationButtonAppointmentSearchInterval = dayView.NavigationButtonAppointmentSearchInterval;
				TimeRulers.Assign(dayView.TimeRulers);
				TimeMarkerVisibility = dayView.TimeMarkerVisibility;
				TimeIndicatorDisplayOptions.Assign(dayView.TimeIndicatorDisplayOptions);
			}
		}
		#region IXtraSupportDeserializeCollectionItem Members
		object IXtraSupportDeserializeCollectionItem.CreateCollectionItem(string propertyName, XtraItemEventArgs e) {
			switch (propertyName) {
				case "TimeSlots":
					return XtraCreateTimeSlotsItem(e);
				case "TimeRulers":
					return XtraCreateTimeRulersItem(e);
				default:
					return null;
			}
		}
		void IXtraSupportDeserializeCollectionItem.SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
			switch (propertyName) {
				case "TimeSlots":
					XtraSetIndexTimeSlotsItem(e);
					break;
				case "TimeRulers":
					XtraSetIndexTimeRulersItem(e);
					break;
				default:
					break;
			}
		}
		#endregion
	}
	#endregion
}
namespace DevExpress.Web.ASPxScheduler.Internal {
	#region DayViewFactoryHelper
	public class DayViewFactoryHelper : ViewFactoryHelper {
		public override ViewGroupTypeStrategy CreateGroupByNoneStrategy() {
			return new DayViewGroupByNoneStrategy();
		}
		public override ViewGroupTypeStrategy CreateGroupByDateStrategy() {
			return new DayViewGroupByDateStrategy();
		}
		public override ViewGroupTypeStrategy CreateGroupByResourceStrategy() {
			return new DayViewGroupByResourceStrategy();
		}
		public override AppointmentBaseLayoutCalculator CreateAppointmentLayoutCalculator(ISchedulerWebViewInfoBase viewInfo, AppointmentContentLayoutCalculator contentCalculator, bool alternate) {
			if (alternate)
				return new DayViewLongAppointmentLayoutCalculator(viewInfo, contentCalculator);
			else
				return new DayViewShortAppointmentLayoutCalculator(viewInfo, contentCalculator);
		}
		public override AppointmentContentLayoutCalculator CreateAppointmentContentLayoutCalculator(ISchedulerWebViewInfoBase viewInfo, bool alternate) {
			if (alternate)
				return new DayViewLongAppointmentContentLayoutCalculator(viewInfo);
			else
				return new DayViewShortAppointmentContentLayoutCalculator(viewInfo);
		}
		public override WebViewRenderer CreateWebViewRenderer(ISchedulerWebViewInfoBase viewInfo) {
			return new DayViewWebRenderer((DayView)viewInfo.View, viewInfo);
		}
		public override AppointmentsBlock CreateAppointmentsBlock(ASPxScheduler control) {
			return new DayViewAppointmentsBlockBuilder(control);
		}
	}
	#endregion
	#region DayViewGroupByNoneStrategy
	public class DayViewGroupByNoneStrategy : ViewGroupTypeStrategy {
		public override IContinuousCellsInfosCalculator CreateVisuallyContinuousCellsInfosCalculator(bool alternate) {
			if (alternate)
				return new DayViewGroupByNoneAllDayAreaVisuallyContinuousCellsInfosCalculator();
			else
				return new DayViewGroupByNoneVisuallyContinuousCellsInfosCalculator();
		}
		public override ISchedulerWebViewInfoBase CreateWebViewInfo(SchedulerViewBase view) {
			return new WebDayViewGroupByNone((DayView)view);
		}
	}
	#endregion
	#region DayViewGroupByDateStrategy
	public class DayViewGroupByDateStrategy : ViewGroupTypeStrategy {
		public override IContinuousCellsInfosCalculator CreateVisuallyContinuousCellsInfosCalculator(bool alternate) {
			if (alternate)
				return new DayViewGroupByDateAllDayAreaVisuallyContinuousCellsInfosCalculator();
			else
				return new DayViewGroupByDateVisuallyContinuousCellsInfosCalculator();
		}
		public override ISchedulerWebViewInfoBase CreateWebViewInfo(SchedulerViewBase view) {
			return new WebDayViewGroupByDate((DayView)view);
		}
	}
	#endregion
	#region DayViewGroupByResourceStrategy
	public class DayViewGroupByResourceStrategy : ViewGroupTypeStrategy {
		public override IContinuousCellsInfosCalculator CreateVisuallyContinuousCellsInfosCalculator(bool alternate) {
			if (alternate)
				return new DayViewGroupByResourceAllDayAreaVisuallyContinuousCellsInfosCalculator();
			else
				return new DayViewGroupByResourceVisuallyContinuousCellsInfosCalculator();
		}
		public override ISchedulerWebViewInfoBase CreateWebViewInfo(SchedulerViewBase view) {
			return new WebDayViewGroupByResource((DayView)view);
		}
	}
	#endregion
}
