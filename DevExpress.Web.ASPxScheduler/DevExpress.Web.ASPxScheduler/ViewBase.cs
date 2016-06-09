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
using System.Web.UI;
using DevExpress.Utils.Serializing;
using DevExpress.Web.ASPxScheduler.Drawing;
using DevExpress.Web.ASPxScheduler.Internal;
using DevExpress.Web.ASPxScheduler.Rendering;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Utils.Controls;
using DevExpress.XtraScheduler.Internal.Implementations;
namespace DevExpress.Web.ASPxScheduler {
	#region SchedulerViewBase
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public abstract class SchedulerViewBase : IInnerSchedulerViewOwner, ISchedulerViewRepositoryItem, IWebAppearance, IXtraSupportShouldSerialize {
		#region Fields
		bool isDisposed;
		ASPxScheduler control;
		InnerSchedulerViewBase innerView;
		ViewFactoryHelper factoryHelper;
		ASPxSchedulerStylesBase innerStyles;
		SchedulerTemplates innerTemplates;
		XtraSupportShouldSerializeHelper shouldSerializeHelper = new XtraSupportShouldSerializeHelper();
		string moreButtonHTML;
		#endregion
		protected SchedulerViewBase(ASPxScheduler control)
			: base() {
			CheckExistenceControl(control);
			this.control = control;
			this.innerStyles = CreateStyles();
			this.innerTemplates = CreateTemplates();
			innerTemplates.Changed += new EventHandler(OnTemplatesChanged);
			this.moreButtonHTML = String.Empty;
			shouldSerializeHelper.RegisterXtraShouldSerializeMethod("DisplayName", XtraShouldSerializeDisplayName);
			shouldSerializeHelper.RegisterXtraShouldSerializeMethod("MenuCaption", XtraShouldSerializeMenuCaption);
			shouldSerializeHelper.RegisterXtraShouldSerializeMethod("ShortDisplayName", XtraShouldSerializeShortDisplayName);
			shouldSerializeHelper.RegisterXtraShouldSerializeMethod("NavigationButtonAppointmentSearchInterval", XtraShouldSerializeNavigationButtonAppointmentSearchInterval);
		}
		protected virtual void CheckExistenceControl(ASPxScheduler control) {
			if (control == null)
				Exceptions.ThrowArgumentException("control", control);
		}
		#region Properties
		protected XtraSupportShouldSerializeHelper ShouldSerializeHelper { get { return shouldSerializeHelper; } }
		internal bool IsDisposed { get { return isDisposed; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ASPxScheduler Control { get { return control; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public abstract SchedulerViewType Type { get; }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal TimeIntervalCollection InnerVisibleIntervals {
			get {
				if (innerView != null)
					return innerView.InnerVisibleIntervals;
				else
					return null;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Obsolete("You should use 'GetVisibleIntervals' and 'SetVisibleIntervals' methods instead", true)]
		public TimeIntervalCollection VisibleIntervals { get { return null; } }
		#region SelectedInterval
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TimeInterval SelectedInterval {
			get {
				SchedulerViewSelection selection = Control.Selection;
				if (selection != null)
					return Control.InnerControl.TimeZoneHelper.FromClientTime(selection.Interval);
				else
					return TimeInterval.Empty;
			}
		}
		#endregion
		#region SelectedResource
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Resource SelectedResource {
			get {
				SchedulerViewSelection selection = Control.Selection;
				if (selection != null)
					return selection.Resource;
				else
					return ResourceBase.Empty;
			}
		}
		#endregion
		#region Enabled
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("SchedulerViewBaseEnabled"),
#endif
DefaultValue(SchedulerViewPropertiesBase.defaultEnabled), NotifyParentProperty(true), AutoFormatEnable()]
		public bool Enabled {
			get {
				if (innerView != null)
					return innerView.Enabled;
				else
					return false;
			}
			set {
				if (innerView != null)
					innerView.Enabled = value;
			}
		}
		#endregion
		#region GroupType
		[DefaultValue(SchedulerViewPropertiesBase.defaultGroupType), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SchedulerGroupType GroupType {
			get {
				if (innerView != null)
					return innerView.GroupType;
				else
					return SchedulerGroupType.None;
			}
			set {
				if (innerView != null)
					innerView.GroupType = value;
			}
		}
		#endregion
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal ViewFactoryHelper FactoryHelper { get { return factoryHelper; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal ResourceBaseCollection FilteredResources {
			get {
				if (innerView != null)
					return innerView.FilteredResources;
				else
					return null;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal AppointmentBaseCollection FilteredAppointments {
			get {
				if (innerView != null)
					return innerView.FilteredAppointments;
				else
					return null;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal ResourceBaseCollection VisibleResources {
			get {
				if (innerView != null)
					return innerView.VisibleResources;
				else
					return null;
			}
		}
		protected internal DateTime VisibleStart {
			get {
				if (innerView != null)
					return innerView.VisibleStart;
				else
					return DateTime.MinValue;
			}
		}
		#region FirstVisibleResourceIndex
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), NotifyParentProperty(true), XtraSerializableProperty()]
		public int FirstVisibleResourceIndex {
			get {
				if (innerView != null)
					return innerView.FirstVisibleResourceIndex;
				else
					return 0;
			}
			set {
				if (innerView != null)
					innerView.FirstVisibleResourceIndex = value;
			}
		}
		#endregion
		#region ResourcesPerPage
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("SchedulerViewBaseResourcesPerPage"),
#endif
DefaultValue(0), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatEnable()]
		public int ResourcesPerPage {
			get {
				if (innerView != null)
					return innerView.ResourcesPerPage;
				else
					return 0;
			}
			set {
				if (innerView != null)
					innerView.ResourcesPerPage = value;
			}
		}
		#endregion
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal int ActualFirstVisibleResourceIndex {
			get {
				if (innerView != null)
					return innerView.ActualFirstVisibleResourceIndex;
				else
					return 0;
			}
		}
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("SchedulerViewBaseMoreButtonHTML"),
#endif
DefaultValue(""), NotifyParentProperty(true), AutoFormatDisable()]
		public virtual string MoreButtonHTML { get { return moreButtonHTML; } set { moreButtonHTML = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal int ActualResourcesPerPage {
			get {
				if (innerView != null)
					return innerView.ActualResourcesPerPage;
				else
					return 0;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal virtual bool CanShowResources() {
			return FilteredResources.Count > 0;
		}
		#region LimitInterval
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal TimeInterval LimitInterval {
			get {
				if (innerView != null)
					return innerView.LimitInterval;
				else
					return null;
			}
			set {
				if (innerView != null)
					innerView.LimitInterval = value;
			}
		}
		#endregion
		[Category(SRCategoryNames.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]
		protected internal AppointmentDisplayOptions AppointmentDisplayOptionsInternal {
			get {
				if (innerView != null)
					return innerView.AppointmentDisplayOptions;
				else
					return null;
			}
		}
		#region ShowMoreButtons
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("SchedulerViewBaseShowMoreButtons"),
#endif
DefaultValue(InnerSchedulerViewBase.defaultShowMoreButtons), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatEnable()]
		public bool ShowMoreButtons {
			get {
				if (innerView != null)
					return innerView.ShowMoreButtons;
				else
					return false;
			}
			set {
				if (innerView != null)
					innerView.ShowMoreButtons = value;
			}
		}
		#endregion
		#region NavigationButtonVisibility
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("SchedulerViewBaseNavigationButtonVisibility"),
#endif
DefaultValue(InnerSchedulerViewBase.defaultNavigationButtonVisibility), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatEnable()]
		public NavigationButtonVisibility NavigationButtonVisibility {
			get {
				if (innerView != null)
					return innerView.NavigationButtonVisibility;
				else
					return NavigationButtonVisibility.Never;
			}
			set {
				if (innerView != null)
					innerView.NavigationButtonVisibility = value;
			}
		}
		#endregion
		#region NavigationButtonAppointmentSearchInterval
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("SchedulerViewBaseNavigationButtonAppointmentSearchInterval"),
#endif
XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatDisable()]
		public TimeSpan NavigationButtonAppointmentSearchInterval {
			get {
				if (innerView != null)
					return innerView.NavigationButtonAppointmentSearchInterval;
				else
					return TimeSpan.Zero;
			}
			set {
				if (innerView != null)
					innerView.NavigationButtonAppointmentSearchInterval = value;
			}
		}
		protected internal virtual bool ShouldSerializeNavigationButtonAppointmentSearchInterval() {
			if (innerView != null)
				return innerView.ShouldSerializeNavigationButtonAppointmentSearchInterval();
			else
				return false;
		}
		protected internal virtual bool XtraShouldSerializeNavigationButtonAppointmentSearchInterval() {
			return ShouldSerializeNavigationButtonAppointmentSearchInterval();
		}
		protected internal virtual void ResetNavigationButtonAppointmentSearchInterval() {
			if (innerView != null)
				innerView.ResetNavigationButtonAppointmentSearchInterval();
		}
		#endregion
		#region DisplayName
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("SchedulerViewBaseDisplayName"),
#endif
NotifyParentProperty(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Localizable(true), AutoFormatDisable()]
		public virtual string DisplayName {
			get {
				if (innerView != null)
					return innerView.DisplayName;
				else
					return String.Empty;
			}
			set {
				if (innerView != null)
					innerView.DisplayName = value;
			}
		}
		protected internal virtual bool ShouldSerializeDisplayName() {
			if (innerView != null)
				return innerView.ShouldSerializeDisplayName();
			else
				return false;
		}
		protected internal virtual bool XtraShouldSerializeDisplayName() {
			return ShouldSerializeDisplayName();
		}
		protected internal virtual void ResetDisplayName() {
			if (innerView != null)
				innerView.ResetDisplayName();
		}
		#endregion
		#region MenuCaption
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("SchedulerViewBaseMenuCaption"),
#endif
NotifyParentProperty(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Localizable(true), AutoFormatDisable()]
		public virtual string MenuCaption {
			get {
				if (innerView != null)
					return innerView.MenuCaption;
				else
					return String.Empty;
			}
			set {
				if (innerView != null)
					innerView.MenuCaption = value;
			}
		}
		protected internal virtual bool ShouldSerializeMenuCaption() {
			if (innerView != null)
				return innerView.ShouldSerializeMenuCaption();
			else
				return false;
		}
		protected internal virtual bool XtraShouldSerializeMenuCaption() {
			return ShouldSerializeMenuCaption();
		}
		protected internal virtual void ResetMenuCaption() {
			if (innerView != null)
				innerView.ResetMenuCaption();
		}
		#endregion
		#region ShortDisplayName
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("SchedulerViewBaseShortDisplayName"),
#endif
NotifyParentProperty(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Localizable(true), AutoFormatDisable()]
		public string ShortDisplayName {
			get {
				if (innerView != null)
					return innerView.ShortDisplayName;
				else
					return String.Empty;
			}
			set {
				if (innerView != null)
					innerView.ShortDisplayName = value;
			}
		}
		protected internal bool ShouldSerializeShortDisplayName() {
			if (innerView != null)
				return innerView.ShouldSerializeShortDisplayName();
			else
				return false;
		}
		protected internal virtual bool XtraShouldSerializeShortDisplayName() {
			return ShouldSerializeShortDisplayName();
		}
		protected internal virtual void ResetShortDisplayName() {
			if (innerView != null)
				innerView.ResetShortDisplayName();
		}
		#endregion
		protected internal virtual bool HeaderAlternateEnabled { get { return true; } }
		protected internal InnerSchedulerViewBase InnerView { get { return innerView; } }
		protected internal ASPxSchedulerStylesBase InnerStyles { get { return innerStyles; } }
		protected internal SchedulerTemplates InnerTemplates { get { return innerTemplates; } }
		#endregion
		protected internal abstract InnerSchedulerViewBase CreateInnerView();
		protected internal abstract ViewFactoryHelper CreateFactoryHelper();
		protected internal virtual void Initialize() {
			SchedulerViewSelection selection = Control != null ? Control.Selection : null;
			this.innerView = CreateInnerView();
			this.innerView.Initialize(selection);
			SubscribeInnerViewEvents();
			this.factoryHelper = CreateFactoryHelper();
		}
		#region IInnerSchedulerViewOwner implementation
		WorkDaysCollection workDays;
		WorkDaysCollection IInnerSchedulerViewOwner.WorkDays {
			get {
				if (workDays == null)
					workDays = Control != null ? Control.WorkDays : InnerSchedulerControl.CreateWorkDays();
				return workDays;
			}
		}
		ResourceBaseCollection IInnerSchedulerViewOwner.GetResourcesTree() {
			return Control.InnerControl.GetResourcesTree();
		}
		DayOfWeek IInnerSchedulerViewOwner.FirstDayOfWeek { 
			get { return Control != null ? Control.FirstDayOfWeek : DateTimeHelper.FirstDayOfWeek; } 
		}
		string IInnerSchedulerViewOwner.ClientTimeZoneId { 
			get { return Control != null ? Control.InnerControl.OptionsBehavior.ClientTimeZoneId : TimeZoneId.Custom; } 
		}
		ResourceBaseCollection IInnerSchedulerViewOwner.GetFilteredResources() {
			return Control.GetFilteredResources();
		}
		AppointmentBaseCollection IInnerSchedulerViewOwner.GetFilteredAppointments(TimeInterval interval, ResourceBaseCollection resources, out bool appointmentsAreReloaded) {
			appointmentsAreReloaded = false;
			return Control.GetFilteredAppointments(interval, resources);
		}
		AppointmentBaseCollection IInnerSchedulerViewOwner.GetNonFilteredAppointments() {
			return Control.GetNonFilteredAppointments();
		}
		void IInnerSchedulerViewOwner.UpdateSelection(SchedulerViewSelection selection) {
		}
		#endregion
		AppointmentDisplayOptions IInnerSchedulerViewOwner.CreateAppointmentDisplayOptions() {
			return CreateAppointmentDisplayOptionsCore();
		}
		protected internal virtual AppointmentDisplayOptions CreateAppointmentDisplayOptionsCore() {
			return new AppointmentDisplayOptions();
		}
		protected internal virtual TimeInterval RoundSelectionInterval(TimeInterval interval) {
			return innerView.RoundSelectionInterval(interval);
		}
		protected internal virtual TimeInterval CreateDefaultSelectionInterval(DateTime date) {
			return innerView.CreateDefaultSelectionInterval(date);
		}
		#region ISchedulerViewRepositoryItem
		InnerSchedulerViewBase ISchedulerViewRepositoryItem.InnerView { get { return this.innerView; } }
		void ISchedulerViewRepositoryItem.Initialize(InnerSchedulerControl control) {
			this.Initialize();
		}
		void ISchedulerViewRepositoryItem.Reset() {
			this.Reset();
		}
		#endregion
		#region Events
		#region EnabledChanging
		CancelEventHandler onEnabledChanging;
		internal event CancelEventHandler EnabledChanging { add { onEnabledChanging += value; } remove { onEnabledChanging -= value; } }
		protected internal virtual bool RaiseEnabledChanging() {
			if (onEnabledChanging != null) {
				CancelEventArgs args = new CancelEventArgs();
				onEnabledChanging(this, args);
				return !args.Cancel;
			}
			return true;
		}
		#endregion
		#region Changed
		SchedulerControlStateChangedEventHandler onChanged;
		internal event SchedulerControlStateChangedEventHandler Changed { add { onChanged += value; } remove { onChanged -= value; } }
		protected internal virtual void RaiseChanged(SchedulerControlChangeType changeType) {
			if (onChanged != null) {
				SchedulerControlStateChangedEventArgs args = new SchedulerControlStateChangedEventArgs(changeType);
				onChanged(this, args);
			}
		}
		#endregion
		#region TemplatesChanged
		EventHandler onTemplatesChanged;
		internal event EventHandler TemplatesChanged { add { onTemplatesChanged += value; } remove { onTemplatesChanged -= value; } }
		protected internal virtual void RaiseTemplatesChanged() {
			if (onTemplatesChanged != null)
				onTemplatesChanged(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (innerView != null) {
					UnsubscribeInnerViewEvents();
					innerView.Dispose();
					innerView = null;
				}
				this.factoryHelper = null;
			}
			isDisposed = true;
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~SchedulerViewBase() {
			Dispose(false);
		}
		#endregion
		#region SubscribeInnerViewEvents
		protected internal virtual void SubscribeInnerViewEvents() {
			innerView.Changed += new SchedulerControlStateChangedEventHandler(OnInnerViewChanged);
			innerView.EnabledChanging += new CancelEventHandler(OnEnabledChanging);
		}
		#endregion
		#region UnsubscribeInnerViewEvents
		protected internal virtual void UnsubscribeInnerViewEvents() {
			innerView.Changed -= new SchedulerControlStateChangedEventHandler(OnInnerViewChanged);
			innerView.EnabledChanging -= new CancelEventHandler(OnEnabledChanging);
		}
		#endregion
		protected internal virtual void OnEnabledChanging(object sender, CancelEventArgs e) {
			e.Cancel = !RaiseEnabledChanging();
		}
		protected internal virtual void OnInnerViewChanged(object sender, SchedulerControlStateChangedEventArgs e) {
			RaiseChanged(e.Change);
		}
		protected virtual void NotifyControlChanges(SchedulerControlChangeType change) {
			Control.InnerControl.ApplyChanges(change);
		}
		public TimeIntervalCollection GetVisibleIntervals() {
			return innerView.GetVisibleIntervals();
		}
		public virtual void SetVisibleIntervals(TimeIntervalCollection intervals) {
			if (intervals == null)
				Exceptions.ThrowArgumentException("intervals", intervals);
			ChangeActions actions = innerView.SetVisibleIntervals(intervals, Control.Selection);
			NotifyControlChangesCore(actions);
		}
		protected virtual void NotifyControlChangesCore(ChangeActions actions) {
			List<SchedulerControlChangeType> changeTypes = new List<SchedulerControlChangeType>();
			changeTypes.Add(SchedulerControlChangeType.VisibleIntervalsChanged);
			Control.InnerControl.ApplyChangesCore(changeTypes, actions);
		}
		protected internal virtual void SetVisibleDays(DayIntervalCollection days) {
			ChangeActions actions = innerView.SetVisibleDays(days, Control.Selection);
			NotifyControlChangesCore(actions);
		}
		public virtual void ChangeAppointmentSelection(Appointment apt) {
			AppointmentSelectionController controller = Control.AppointmentSelectionController;
			if (controller == null)
				return;
			controller.SelectSingleAppointment(apt);
		}
		public void AddAppointmentSelection(Appointment apt) {
			AppointmentSelectionController controller = Control.AppointmentSelectionController;
			if (controller == null)
				return;
			controller.AddToSelection(apt);
		}
		public void ReverseAppointmentSelection(Appointment apt) {
			AppointmentSelectionController controller = Control.AppointmentSelectionController;
			if (controller == null)
				return;
			controller.ChangeSelection(apt);
		}
		public virtual void SetSelection(TimeInterval interval, XtraScheduler.Resource resource) {
			if (interval == null)
				Exceptions.ThrowArgumentException("interval", interval);
			if (resource == null)
				Exceptions.ThrowArgumentException("resource", resource);
			if (Control.Selection == null)
				return;
			SetSelectionCore(interval, resource);
		}
		protected internal virtual void SetSelectionCore(TimeInterval interval, XtraScheduler.Resource resource) {
			SetSelectionCommand command = new SetSelectionCommand(Control.InnerControl, interval, resource);
			command.Execute();
		}
		public AppointmentDisplayOptions GetAppointmentDisplayOptions() {
			return AppointmentDisplayOptionsInternal;
		}
		public virtual void SelectAppointment(Appointment apt) {
			if (apt == null)
				Exceptions.ThrowArgumentException("apt", apt);
			SelectAppointmentCore(apt, apt.ResourceId);
		}
		public virtual void SelectAppointment(Appointment apt, XtraScheduler.Resource resource) {
			if (apt == null)
				Exceptions.ThrowArgumentException("apt", apt);
			if (resource == null)
				Exceptions.ThrowArgumentException("resource", resource);
			SelectAppointmentCore(apt, resource.Id);
		}
		protected internal virtual void SelectAppointmentCore(Appointment apt, object resourceId) {
			if (Control.Selection == null)
				return;
			if (!ResourceBase.InternalMatchIdToResourceIdCollection(apt.ResourceIds, resourceId))
				resourceId = Control.Selection.Resource.Id;
			XtraScheduler.Resource resource = this.FilteredResources.GetResourceById(resourceId);
			if (resource == ResourceBase.Empty)
				resource = Control.Selection.Resource;
			try {
				SetSelectionCore(((IInternalAppointment)apt).CreateInterval(), resource);
				ChangeAppointmentSelection(apt);
			}
			finally {
			}
		}
		protected internal virtual void Reset() {
			innerView.Reset();
		}
		protected internal virtual SchedulerColorSchema GetColorSchema(Color color, int resourceColorIndex) {
			if (color != Color.Empty)
				return new SchedulerColorSchema(color);
			SchedulerColorSchema schema = Control.ResourceColorSchemas.GetSchema(resourceColorIndex);
			return (SchedulerColorSchema)schema.Clone();
		}
		protected internal abstract ASPxSchedulerStylesBase CreateStyles();
		protected internal abstract SchedulerTemplates CreateTemplates();
		protected internal virtual void OnTemplatesChanged(object sender, EventArgs e) {
			RaiseTemplatesChanged();
		}
		#region IWebAppearance Members
		ASPxSchedulerStylesBase IWebAppearance.InnerStyles { get { return InnerStyles; } }
		SchedulerTemplates IWebAppearance.InnerTemplates { get { return InnerTemplates; } }
		#endregion
		public virtual ResourceBaseCollection GetResources() {
			if (innerView != null && Object.ReferenceEquals(Control.ActiveView, this))
				return innerView.GetResources();
			else
				return new ResourceBaseCollection();
		}
		public virtual AppointmentBaseCollection GetAppointments() {
			if (innerView != null && Object.ReferenceEquals(Control.ActiveView, this))
				return innerView.GetAppointments();
			else
				return new AppointmentBaseCollection();
		}
		public virtual void ZoomIn() {
			InnerView.ZoomIn();
		}
		public virtual void ZoomOut() {
			InnerView.ZoomOut();
		}
		public virtual void Assign(SchedulerViewBase source) {
			if (source == null)
				return;
			DisplayName = source.DisplayName;
			Enabled = source.Enabled;
			FirstVisibleResourceIndex = source.FirstVisibleResourceIndex;
			GroupType = source.GroupType;
			MenuCaption = source.MenuCaption;
			MoreButtonHTML = source.MoreButtonHTML;
			NavigationButtonAppointmentSearchInterval = source.NavigationButtonAppointmentSearchInterval;
			NavigationButtonVisibility = source.NavigationButtonVisibility;
			ResourcesPerPage = source.ResourcesPerPage;
			ShortDisplayName = source.ShortDisplayName;
			ShowMoreButtons = source.ShowMoreButtons;
		}
		#region IXtraSupportShouldSerialize Members
		bool IXtraSupportShouldSerialize.ShouldSerialize(string propertyName) {
			return shouldSerializeHelper.ShouldSerialize(propertyName);
		}
		#endregion
	}
	#endregion
	#region SchedulerViewRepository
	public class SchedulerViewRepository : SchedulerViewTypedRepositoryBase<SchedulerViewBase> {
		public SchedulerViewRepository() {
		}
		#region Properties
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("SchedulerViewRepositoryDayView"),
#endif
Category(SRCategoryNames.View), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]
		public DayView DayView { get { return (DayView)this[SchedulerViewType.Day]; } }
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("SchedulerViewRepositoryWorkWeekView"),
#endif
Category(SRCategoryNames.View), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]
		public WorkWeekView WorkWeekView { get { return (WorkWeekView)this[SchedulerViewType.WorkWeek]; } }
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("SchedulerViewRepositoryWeekView"),
#endif
Category(SRCategoryNames.View), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]
		public WeekView WeekView { get { return (WeekView)this[SchedulerViewType.Week]; } }
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("SchedulerViewRepositoryMonthView"),
#endif
Category(SRCategoryNames.View), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]
		public MonthView MonthView { get { return (MonthView)this[SchedulerViewType.Month]; } }
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("SchedulerViewRepositoryTimelineView"),
#endif
Category(SRCategoryNames.View), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]
		public TimelineView TimelineView { get { return (TimelineView)this[SchedulerViewType.Timeline]; } }
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("SchedulerViewRepositoryTimelineView"),
#endif
Category(SRCategoryNames.View), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]
		public FullWeekView FullWeekView { get { return (FullWeekView)this[SchedulerViewType.FullWeek]; } }
		#endregion
		protected internal override void CreateViews(InnerSchedulerControl control) {
			ASPxScheduler aspxControl = control != null ? (ASPxScheduler)control.Owner : null;
			RegisterView(CreateDayViewByControl(aspxControl));
			RegisterView(CreateWorkWeekViewByControl(aspxControl));
			RegisterView(CreateFullWeekViewByControl(aspxControl));
			RegisterView(CreateWeekViewByControl(aspxControl));
			RegisterView(CreateMonthViewByControl(aspxControl));
			RegisterView(CreateTimelineViewByControl(aspxControl));			
		}
		protected virtual DayView CreateDayViewByControl(ASPxScheduler control) {
			return new DayView(control);
		}
		protected virtual WorkWeekView CreateWorkWeekViewByControl(ASPxScheduler control) {
			return new WorkWeekView(control);
		}
		protected virtual WeekView CreateWeekViewByControl(ASPxScheduler control) {
			return new WeekView(control);
		}
		protected virtual MonthView CreateMonthViewByControl(ASPxScheduler control) {
			return new MonthView(control);
		}
		protected virtual TimelineView CreateTimelineViewByControl(ASPxScheduler control) {
			return new TimelineView(control);
		}
		protected virtual FullWeekView CreateFullWeekViewByControl(ASPxScheduler control) {
			return new FullWeekView(control);
		}
		public virtual void Assign(SchedulerViewRepository source) {
			if (source == null)
				return;
			DayView.Assign(source.DayView);
			WorkWeekView.Assign(source.WorkWeekView);
			WeekView.Assign(source.WeekView);
			MonthView.Assign(source.MonthView);
			TimelineView.Assign(source.TimelineView);
			FullWeekView.Assign(source.FullWeekView);
		}
	}
	#endregion
}
namespace DevExpress.Web.ASPxScheduler.Internal {
	#region ViewGroupTypeStrategy
	public abstract class ViewGroupTypeStrategy {
		public abstract IContinuousCellsInfosCalculator CreateVisuallyContinuousCellsInfosCalculator(bool alternate);
		public abstract ISchedulerWebViewInfoBase CreateWebViewInfo(SchedulerViewBase view);
	}
	#endregion
	#region ViewFactoryHelper
	public abstract class ViewFactoryHelper {
		#region Fields
		ViewGroupTypeStrategy groupByNoneStrategy;
		ViewGroupTypeStrategy groupByDateStrategy;
		ViewGroupTypeStrategy groupByResourceStrategy;
		#endregion
		protected ViewFactoryHelper() {
			this.groupByNoneStrategy = CreateGroupByNoneStrategy();
			this.groupByDateStrategy = CreateGroupByDateStrategy();
			this.groupByResourceStrategy = CreateGroupByResourceStrategy();
		}
		#region Properties
		protected internal ViewGroupTypeStrategy GroupByNoneStrategy { get { return groupByNoneStrategy; } }
		protected internal ViewGroupTypeStrategy GroupByDateStrategy { get { return groupByDateStrategy; } }
		protected internal ViewGroupTypeStrategy GroupByResourceStrategy { get { return groupByResourceStrategy; } }
		protected internal virtual bool CanShowResources(SchedulerViewBase view) {
			return view.CanShowResources();
		}
		#endregion
		public virtual IContinuousCellsInfosCalculator CreateVisuallyContinuousCellsInfosCalculator(SchedulerViewBase view, bool alternate) {
			ViewGroupTypeStrategy strategy = SelectStrategy(view);
			return strategy.CreateVisuallyContinuousCellsInfosCalculator(alternate);
		}
		public virtual ISchedulerWebViewInfoBase CreateWebViewInfo(SchedulerViewBase view) {
			ViewGroupTypeStrategy strategy = SelectStrategy(view);
			ISchedulerWebViewInfoBase viewInfo = strategy.CreateWebViewInfo(view);
			viewInfo.Create();
			return viewInfo;
		}
		public virtual WebViewRenderer CreateWebViewRenderer(ISchedulerWebViewInfoBase viewInfo) {
			return new WebViewRenderer(viewInfo.View, viewInfo);
		}
		public virtual AppointmentsBlock CreateAppointmentsBlock(ASPxScheduler control) {
			return new AppointmentsBlock(control);
		}
		public abstract AppointmentBaseLayoutCalculator CreateAppointmentLayoutCalculator(ISchedulerWebViewInfoBase viewInfo, AppointmentContentLayoutCalculator contentCalculator, bool alternate);
		public abstract AppointmentContentLayoutCalculator CreateAppointmentContentLayoutCalculator(ISchedulerWebViewInfoBase viewInfo, bool alternate);
		protected internal virtual ViewGroupTypeStrategy SelectStrategy(SchedulerViewBase view) {
			SchedulerGroupType groupType = CalcActualGroupType(view);
			switch (groupType) {
				case SchedulerGroupType.None:
					return groupByNoneStrategy;
				case SchedulerGroupType.Date:
					return groupByDateStrategy;
				case SchedulerGroupType.Resource:
					return groupByResourceStrategy;
				default:
					Exceptions.ThrowInternalException();
					return null;
			}
		}
		protected internal virtual SchedulerGroupType CalcActualGroupType(SchedulerViewBase view) {
			return CanShowResources(view) ? view.GroupType : SchedulerGroupType.None;
		}
		public abstract ViewGroupTypeStrategy CreateGroupByNoneStrategy();
		public abstract ViewGroupTypeStrategy CreateGroupByDateStrategy();
		public abstract ViewGroupTypeStrategy CreateGroupByResourceStrategy();
	}
	#endregion
}
