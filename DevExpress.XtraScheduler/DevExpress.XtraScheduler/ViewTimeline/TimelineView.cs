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
using System.Drawing.Design;
using System.Globalization;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraEditors;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Services;
using System.Windows.Forms;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.Utils.Controls;
namespace DevExpress.XtraScheduler {
	public class TimelineView : SchedulerViewBase, IXtraSupportDeserializeCollection {
		#region Fields
		TimeIndicatorDisplayOptions timeIndicatorDisplayOptions;
		#endregion
		public TimelineView(SchedulerControl control)
			: base(control) {
			this.timeIndicatorDisplayOptions = CreateTimeIndicatorDisplayOptions();
		}
		#region Properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new TimelineViewInfo ViewInfo { get { return (TimelineViewInfo)base.ViewInfo; } }
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("TimelineViewCellsAutoHeightOptions"),
#endif
Category(SRCategoryNames.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public CellsAutoHeightOptions CellsAutoHeightOptions {
			get {
				if (InnerView != null)
					return InnerView.CellsAutoHeightOptions;
				else
					return null;
			}
		}
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("TimelineViewAppearance"),
#endif
Category(SRCategoryNames.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new TimelineViewAppearance Appearance { get { return (TimelineViewAppearance)base.Appearance; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override SchedulerViewType Type { get { return SchedulerViewType.Timeline; } }
		#region WorkTime
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("TimelineViewWorkTime"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public TimeOfDayInterval WorkTime {
			get {
				if (InnerView != null)
					return InnerView.WorkTime;
				else
					return null;
			}
			set {
				if (InnerView != null)
					InnerView.WorkTime = value;
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
	DevExpressXtraSchedulerLocalizedDescription("TimelineViewScales"),
#endif
Category(SRCategoryNames.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.XtraScheduler.Design.TimeScaleCollectionEditor," + AssemblyInfo.SRAssemblySchedulerDesign, typeof(UITypeEditor)),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true)
		]
		public TimeScaleCollection Scales {
			get {
				if (InnerView != null)
					return InnerView.Scales;
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
		#region SelectionBar
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("TimelineViewSelectionBar"),
#endif
Category(SRCategoryNames.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public SelectionBarOptions SelectionBar {
			get {
				if (InnerView != null)
					return InnerView.SelectionBar;
				else
					return null;
			}
		}
		#endregion
		#region CellsAutoHeight
		[Obsolete("You should use the 'CellsAutoHeightOptions.Enabled' instead", false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool CellsAutoHeight { get { return CellsAutoHeightOptions.Enabled; } set { CellsAutoHeightOptions.Enabled = value; } }
		#endregion
		#region ShowResourceHeaders
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("TimelineViewShowResourceHeaders"),
#endif
DefaultValue(InnerTimelineView.defaultShowResourceHeaders)]
		public bool ShowResourceHeaders {
			get {
				if (InnerView != null)
					return InnerView.ShowResourceHeaders;
				else
					return false;
			}
			set {
				if (InnerView != null)
					InnerView.ShowResourceHeaders = value;
			}
		}
		#endregion
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override SchedulerMenuItemId MenuItemId { get { return SchedulerMenuItemId.SwitchToTimelineView; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal new TimelineViewFactoryHelper FactoryHelper { get { return (TimelineViewFactoryHelper)base.FactoryHelper; } }
		internal int CollapsedResourceHeight { get { return 20; } }
		internal TimeScaleCollection ActualScales {
			get {
				if (InnerView != null)
					return InnerView.ActualScales;
				else
					return null;
			}
		}
		protected internal new InnerTimelineView InnerView { get { return (InnerTimelineView)base.InnerView; } }
		#region OptionsSelectionBehavior
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("TimelineViewOptionsSelectionBehavior"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue),
		NotifyParentProperty(true)]
		public OptionsSelectionBehavior OptionsSelectionBehavior {
			get {
				if (InnerView != null) {
					InnerTimelineView innerView = InnerView;
					return innerView.OptionsSelectionBehavior;
				} else
					return null;
			}
		}
		#endregion
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("TimelineViewTimelineScrollBarVisible"),
#endif
		DefaultValue(SchedulerViewBase.defaultContainerScrollbarVisible)]
		public bool TimelineScrollBarVisible {
			get { return ContainerScrollBarVisibility == SchedulerScrollBarVisibility.Always; }
			set { ContainerScrollBarVisibility = value ? SchedulerScrollBarVisibility.Always : SchedulerScrollBarVisibility.Never; }
		}
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("TimelineViewAppointmentDisplayOptions"),
#endif
		Category(SRCategoryNames.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public new TimelineViewAppointmentDisplayOptions AppointmentDisplayOptions { get { return (TimelineViewAppointmentDisplayOptions)base.AppointmentDisplayOptions; } }
		internal override bool ShowResourceHeadersInternal { get { return ShowResourceHeaders; } }
		#region DeferredScrolling
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("TimelineViewDeferredScrolling"),
#endif
		XtraSerializableProperty()]
		public SchedulerDeferredScrollingOption DeferredScrolling { get { return (SchedulerDeferredScrollingOption)InnerView.DeferredScrolling; } }
		#endregion
		#region EnableInfinitScrolling
		bool enableInfinitScrolling = true;
		[DefaultValue(true)]
		public bool EnableInfiniteScrolling {
			get {
				return enableInfinitScrolling;
			}
			set {
				if (enableInfinitScrolling == value)
					return;
				enableInfinitScrolling = value;
				RaiseChanged(SchedulerControlChangeType.DateTimeScrollbarVisibilityChanged);
				Control.RecreateDateTimeScrollController();
				Control.ApplyChanges(SchedulerControlChangeType.DateTimeScrollbarVisibilityChanged);
			}
		}
		#endregion
		#region TimeIndicatorDisplayOptions
		[
Category(SRCategoryNames.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public TimeIndicatorDisplayOptions TimeIndicatorDisplayOptions {
			get {
				return timeIndicatorDisplayOptions;
			}
		}
		#endregion
		#endregion
		public TimeScale GetBaseTimeScale() {
			if (InnerView != null)
				return InnerView.GetBaseTimeScale();
			return null;
		}
		protected internal virtual TimeScaleCollection CreateTimeScaleCollection() {
			return new TimeScaleCollection();
		}
		protected internal override InnerSchedulerViewBase CreateInnerView() {
			return new InnerTimelineView(this, new TimelineViewProperties());
		}
		protected internal override void Initialize() {
			base.Initialize();
			SubscribeTimeIndicatorDisplayOptionsEvents();
		}
		#region IDisposable implementation
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if (timeIndicatorDisplayOptions != null) {
				UnsubscribeTimeIndicatorDisplayOptionsEvents();
				timeIndicatorDisplayOptions = null;
			}
		}
		#endregion
		#region SubscribeTimeIndicatorDisplayOptionsEvents
		protected internal virtual void SubscribeTimeIndicatorDisplayOptionsEvents() {
			timeIndicatorDisplayOptions.Changed += new BaseOptionChangedEventHandler(OnTimeIndicatorDisplayOptionsChanged);
		}
		#endregion
		#region UnsubscribeTimeIndicatorDisplayOptionsEvents
		protected internal virtual void UnsubscribeTimeIndicatorDisplayOptionsEvents() {
			timeIndicatorDisplayOptions.Changed -= new BaseOptionChangedEventHandler(OnTimeIndicatorDisplayOptionsChanged);
		}
		#endregion
		protected internal virtual void OnTimeIndicatorDisplayOptionsChanged(object sender, BaseOptionChangedEventArgs e) {
			RaiseChanged(SchedulerControlChangeType.TimeIndicatorDisplayOptionsChanged);
		}
		protected internal virtual void UpdateFilteredAppointments() {
			InnerView.QueryAppointments();
		}
		protected internal override ViewDateTimeScrollController CreateDateTimeScrollController() {
			if (EnableInfiniteScrolling)
				return new TimelineViewDateTimeScrollController(this);
			return new TimelineFiniteDateTimeScrollController(this);
		}
		protected internal override BaseViewAppearance CreateAppearance() {
			return new TimelineViewAppearance();
		}
		protected internal override ViewPainterBase CreatePainter(SchedulerPaintStyle paintStyle) {
			if (paintStyle == null)
				Exceptions.ThrowArgumentException("paintStyle", paintStyle);
			return paintStyle.CreateTimelineViewPainter();
		}
		protected internal override ViewFactoryHelper CreateFactoryHelper() {
			return new TimelineViewFactoryHelper();
		}
		protected internal override SchedulerViewInfoBase CreateViewInfoCore() {
			return new TimelineViewInfo(this);
		}
		protected internal override AppointmentDisplayOptions CreateAppointmentDisplayOptionsCore() {
			return new TimelineViewAppointmentDisplayOptions();
		}
		protected internal override void InitializeViewInfo(SchedulerViewInfoBase viewInfo) {
		}
		protected internal override bool ChangeResourceScrollBarOrientationIfNeeded(ResourceNavigator navigator) {
			bool oldVertical = navigator.Vertical;
			bool newVertical = CanShowResources() && GroupType != SchedulerGroupType.None;
			navigator.Vertical = newVertical;
			return oldVertical != newVertical;
		}
		protected internal override bool ChangeDateTimeScrollBarOrientationIfNeeded(DateTimeScrollBar scrollBar) {
			ScrollBarType oldScrollBarType = scrollBar.ScrollBarType;
			scrollBar.ScrollBarType = ScrollBarType.Horizontal;
			return oldScrollBarType != ScrollBarType.Horizontal;
		}
		protected internal override void Reset() {
			base.Reset();
			SelectionBar.Reset();
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
		protected override void MakeAppointmentVisibleInScrollContainers(Appointment apt) {
			ViewInfo.MakeAppointmentVisibleInScrollContainers(apt);
		}
	}
}
