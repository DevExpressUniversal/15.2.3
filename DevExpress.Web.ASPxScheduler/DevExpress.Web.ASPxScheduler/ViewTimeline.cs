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
using System.Drawing.Design;
using System.Web.UI;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Web.ASPxScheduler;
using DevExpress.Web.ASPxScheduler.Drawing;
using DevExpress.Web.ASPxScheduler.Internal;
using DevExpress.Web.ASPxScheduler.Rendering;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
namespace DevExpress.Web.ASPxScheduler {
	#region TimelineView
	public class TimelineView : SchedulerViewBase, IXtraSupportDeserializeCollection, IXtraSupportDeserializeCollectionItem {
		#region Fields
		const int defaultIntervalCount = 10;
		int intervalCount = defaultIntervalCount;
		ASPxSchedulerOptionsCellAutoHeight cellAutoHeight;
		TimeIndicatorDisplayOptions timeIndicatorDisplayOptions;
		#endregion
		public TimelineView(ASPxScheduler control)
			: base(control) {
			ShouldSerializeHelper.RegisterXtraShouldSerializeMethod("WorkTime", XtraShouldSerializeWorkTime);
			ShouldSerializeHelper.RegisterXtraShouldSerializeMethod("Scales", XtraShouldSerializeScales);
			this.cellAutoHeight = new ASPxSchedulerOptionsCellAutoHeight();
			this.timeIndicatorDisplayOptions = new TimeIndicatorDisplayOptions();
		}
		#region Properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override SchedulerViewType Type { get { return SchedulerViewType.Timeline; } }
		#region WorkTime
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("TimelineViewWorkTime"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable()]
		public TimeOfDayInterval WorkTime {
			get {
				if(InnerView != null) {
					InnerTimelineView innerView = (InnerTimelineView)InnerView;
					return innerView.WorkTime;
				}
				else
					return null;
			}
			set {
				if(InnerView != null) {
					InnerTimelineView innerView = (InnerTimelineView)InnerView;
					innerView.WorkTime = value;
				}
			}
		}
		internal bool ShouldSerializeWorkTime() {
			return !WorkTime.IsEqual(InnerTimelineView.defaultWorkTime);
		}
		internal void ResetWorkTime() {
			WorkTime = InnerTimelineView.defaultWorkTime;
		}
		internal bool XtraShouldSerializeWorkTime() {
			return ShouldSerializeWorkTime();
		}
		#endregion
		#region Scales
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("TimelineViewScales"),
#endif
Category(SRCategoryNames.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.XtraScheduler.Design.TimeScaleCollectionEditor," + AssemblyInfo.SRAssemblySchedulerDesign, typeof(UITypeEditor)),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true),
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable()
		]
		public TimeScaleCollection Scales {
			get {
				if(InnerView != null) {
					InnerTimelineView innerView = (InnerTimelineView)InnerView;
					return innerView.Scales;
				}
				else
					return null;
			}
		}
		internal bool ShouldSerializeScales() {
			return !Scales.HasDefaultContent();
		}
		internal bool XtraShouldSerializeScales() {
			return ShouldSerializeScales();
		}
		internal void ResetScales() {
			Scales.LoadDefaults();
		}
		internal virtual void XtraClearScales(XtraItemEventArgs e) {
			Scales.Clear();
		}
		internal virtual object XtraCreateScalesItem(XtraItemEventArgs e) {
			if (e.Item.ChildProperties == null)
				return null;
			DevExpress.Utils.Serializing.Helpers.XtraPropertyInfo propertyInfo;
			propertyInfo = e.Item.ChildProperties["SerializationTypeName"];
			if (propertyInfo == null || propertyInfo.Value == null)
				return null;
			string typeName = propertyInfo.Value.ToString();
			if (String.IsNullOrEmpty(typeName))
				return null;
			Type type = TypeSerializationHelper.CreateTypeFromSerializationTypeName(typeName);
			if (type == null)
				return null;
			TimeScale scale = Activator.CreateInstance(type) as TimeScale;
			return scale;
		}
		internal void XtraSetIndexScalesItem(XtraSetItemIndexEventArgs e) {
			TimeScale scale = e.Item.Value as TimeScale;
			if (scale == null)
				return;
			Scales.Add(scale);
		}
		#endregion
		#region Styles
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("TimelineViewStyles"),
#endif
Category("Styles"), NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TimelineViewStyles Styles { get { return (TimelineViewStyles)InnerStyles; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual TimelineViewStyles TimelineViewStyles { get { return this.Styles; } }
		#endregion
		#region Templates
		[
		Browsable(false),
		PersistenceMode(PersistenceMode.InnerProperty),
		NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public TimelineViewTemplates Templates { get { return (TimelineViewTemplates)InnerTemplates; } }
		#endregion
		#region AppointmentDisplayOptions
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("TimelineViewAppointmentDisplayOptions"),
#endif
		Category(SRCategoryNames.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]
		public TimelineViewAppointmentDisplayOptions AppointmentDisplayOptions { get { return (TimelineViewAppointmentDisplayOptions)base.AppointmentDisplayOptionsInternal; } }
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
		protected internal new InnerTimelineView InnerView { get { return (InnerTimelineView)base.InnerView; } }
		#region ShowSelectionBar
		bool showSelectionBar = false;
		[Category(SRCategoryNames.Appearance), XtraSerializableProperty(XtraSerializationVisibility.Visible, XtraSerializationFlags.DefaultValue), NotifyParentProperty(true), AutoFormatDisable()]
		internal bool ShowSelectionBar { get { return showSelectionBar; } set { showSelectionBar = value; } }
		#endregion
		#region IntervalCount
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("TimelineViewIntervalCount"),
#endif
 XtraSerializableProperty(XtraSerializationVisibility.Visible, XtraSerializationFlags.DefaultValue),
NotifyParentProperty(true), DefaultValue(defaultIntervalCount), AutoFormatEnable()]
		public int IntervalCount {
			get { return intervalCount; }
			set {
				if(value < 1)
					Exceptions.ThrowArgumentException("IntervalCount", value);
				if(IntervalCount == value) 
					return;
				intervalCount = value;
				InnerView.RaiseChanged(SchedulerControlChangeType.TimelineIntervalCountChanged);
			}
		}
		#endregion
		#region OptionsSelectionBehavior
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("TimelineViewOptionsSelectionBehavior"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatDisable()]
		public OptionsSelectionBehavior OptionsSelectionBehavior {
			get {
				if(InnerView != null) {
					InnerTimelineView innerView = (InnerTimelineView)InnerView;
					return innerView.OptionsSelectionBehavior;
				}
				else
					return null;
			}
		}
		#endregion
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("TimelineViewCellAutoHeightOptions"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]
		public ASPxSchedulerOptionsCellAutoHeight CellAutoHeightOptions { get { return cellAutoHeight; } }
		#endregion
		public TimeScale GetBaseTimeScale() {
			if (InnerView != null) {
				InnerTimelineView innerView = (InnerTimelineView)InnerView;
				return innerView.GetBaseTimeScale();
			}
			return null;
		}
		protected internal virtual TimeScaleCollection CreateTimeScaleCollection() {
			return new TimeScaleCollection();
		}
		protected internal override InnerSchedulerViewBase CreateInnerView() {
			return new ASPxInnerTimelineView(this, new TimelineViewProperties());
		}
		protected internal override ViewFactoryHelper CreateFactoryHelper() {
			return new TimelineViewFactoryHelper();
		}
		protected internal virtual void UpdateFilteredAppointments() {
			FilteredAppointments.BeginUpdate();
			try {
				FilteredAppointments.Clear();
				FilteredAppointments.AddRange(Control.GetFilteredAppointments(InnerVisibleIntervals.Interval, FilteredResources));
			}
			finally {
				FilteredAppointments.EndUpdate();
			}
		}
		protected internal override ASPxSchedulerStylesBase CreateStyles() {
			return new TimelineViewStyles(Control);
		}
		protected internal override SchedulerTemplates CreateTemplates() {
			return new TimelineViewTemplates();
		}
		protected internal override AppointmentDisplayOptions CreateAppointmentDisplayOptionsCore() {
			return new TimelineViewAppointmentDisplayOptions();
		}
		public override void Assign(SchedulerViewBase source) {
			base.Assign(source);
			TimelineView timelineView = source as TimelineView;
			if (timelineView != null) {
				WorkTime.Assign(timelineView.WorkTime);
				CellAutoHeightOptions.Assign(timelineView.CellAutoHeightOptions);
				OptionsSelectionBehavior.Assign(timelineView.OptionsSelectionBehavior);
				AppointmentDisplayOptions.Assign(timelineView.AppointmentDisplayOptions);
				TimelineViewStyles.CopyFrom(timelineView.TimelineViewStyles);
				IntervalCount = timelineView.IntervalCount;
				TimeIndicatorDisplayOptions.Assign(timelineView.TimeIndicatorDisplayOptions);
				Scales.Assign(timelineView.Scales);
			}
		}
		#region IXtraSupportDeserializeCollection Members
		void IXtraSupportDeserializeCollection.BeforeDeserializeCollection(string propertyName, XtraItemEventArgs e) {
			if (InnerView != null)
				InnerView.BeforeDeserializeCollection(propertyName, e);
		}
		bool IXtraSupportDeserializeCollection.ClearCollection(string propertyName, XtraItemEventArgs e) {
			if (InnerView != null)
				InnerView.ClearCollection(propertyName, e);
			return true;
		}
		void IXtraSupportDeserializeCollection.AfterDeserializeCollection(string propertyName, XtraItemEventArgs e) {
			if (InnerView != null)
				InnerView.AfterDeserializeCollection(propertyName, e);
		}
		#endregion
		#region IXtraSupportDeserializeCollectionItem Members
		object IXtraSupportDeserializeCollectionItem.CreateCollectionItem(string propertyName, XtraItemEventArgs e) {
			switch (propertyName) {
				case "Scales":
					return XtraCreateScalesItem(e);
				default:
					return null;
			}
		}
		void IXtraSupportDeserializeCollectionItem.SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
			switch (propertyName) {
				case "Scales":
					XtraSetIndexScalesItem(e);
					break;
				default:
					break;
			}
		}
		#endregion
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Native {
	public class ASPxInnerTimelineView : InnerTimelineView {
		public ASPxInnerTimelineView(IInnerSchedulerViewOwner owner, ITimelineViewProperties properties)
			: base(owner, properties) {
		}
		protected internal override TimeIntervalCollection CreateValidIntervals(DayIntervalCollection days) {
			if (days.Count <= 0)
				return days;					   
			TimeScaleIntervalCollection result = (TimeScaleIntervalCollection)InnerVisibleIntervals.CreateEmptyClone();
			PopulateVisibleIntervalsCore(days[0].Start, result);			
			return result;
		}
		protected internal virtual void PopulateVisibleIntervalsCore(DateTime date, TimeScaleIntervalCollection collection) {
			TimelineView view = (TimelineView)Owner;
			TimeScale scale = collection.Scale;
			DateTime currentDate = scale.Floor(date);
			int count = view.IntervalCount;
			for (int i = 0; i < count; i++) {
				collection.Add(new TimeInterval(currentDate, TimeSpan.Zero));
				currentDate = scale.GetNextDate(currentDate);
			}
		}
		protected internal override void PopulateVisibleIntervalsCore(DateTime date) {			
			TimeScaleIntervalCollection collection = (TimeScaleIntervalCollection)InnerVisibleIntervals;
			PopulateVisibleIntervalsCore(date, collection);
		}
	}
}
namespace DevExpress.Web.ASPxScheduler.Internal {
	#region TimelineViewFactoryHelper
	public class TimelineViewFactoryHelper : ViewFactoryHelper {
		public override ViewGroupTypeStrategy CreateGroupByNoneStrategy() {
			return new TimelineViewGroupByNoneStrategy();
		}
		public override ViewGroupTypeStrategy CreateGroupByDateStrategy() {
			return new TimelineViewGroupByDateStrategy();
		}
		public override ViewGroupTypeStrategy CreateGroupByResourceStrategy() {
			return new TimelineViewGroupByResourceStrategy();
		}
		public override AppointmentBaseLayoutCalculator CreateAppointmentLayoutCalculator(ISchedulerWebViewInfoBase viewInfo, AppointmentContentLayoutCalculator contentCalculator, bool alternate) {
			return new TimeLineAppointmentLayoutCalculator(viewInfo, contentCalculator);
		}
		public override AppointmentContentLayoutCalculator CreateAppointmentContentLayoutCalculator(ISchedulerWebViewInfoBase viewInfo, bool alternate) {
			return new TimelineViewAppointmentContentLayoutCalculator(viewInfo);
		}
		public override WebViewRenderer CreateWebViewRenderer(ISchedulerWebViewInfoBase viewInfo) {
			return new TimelineViewWebRenderer(viewInfo.View, viewInfo);
		}
	}
	#endregion
	#region TimelineViewGroupByNoneStrategy
	public class TimelineViewGroupByNoneStrategy : ViewGroupTypeStrategy {
		public override IContinuousCellsInfosCalculator CreateVisuallyContinuousCellsInfosCalculator(bool alternate) {
			return new TimelineViewGroupByNoneVisuallyContinuousCellsInfosCalculator();
		}
		public override ISchedulerWebViewInfoBase CreateWebViewInfo(SchedulerViewBase view) {
			return new WebTimelineViewGroupByNone((TimelineView)view);
		}
	}
	#endregion
	#region TimelineViewGroupByDateStrategy
	public class TimelineViewGroupByDateStrategy : ViewGroupTypeStrategy {
		public override IContinuousCellsInfosCalculator CreateVisuallyContinuousCellsInfosCalculator(bool alternate) {
			return new TimelineViewGroupByDateVisuallyContinuousCellsInfosCalculator();
		}
		public override ISchedulerWebViewInfoBase CreateWebViewInfo(SchedulerViewBase view) {
			return new WebTimelineViewGroupByDate((TimelineView)view);
		}
	}
	#endregion
	#region TimelineViewGroupByResourceStrategy
	public class TimelineViewGroupByResourceStrategy : ViewGroupTypeStrategy {
		public override IContinuousCellsInfosCalculator CreateVisuallyContinuousCellsInfosCalculator(bool alternate) {
			return new TimelineViewGroupByDateVisuallyContinuousCellsInfosCalculator();
		}
		public override ISchedulerWebViewInfoBase CreateWebViewInfo(SchedulerViewBase view) {
			return new WebTimelineViewGroupByResource((TimelineView)view);
		}
	}
	#endregion
}
