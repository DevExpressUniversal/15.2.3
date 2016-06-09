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
using System.Diagnostics;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Internal;
using DevExpress.XtraScheduler.Internal.Implementations;
using DevExpress.XtraScheduler.Native;
namespace DevExpress.XtraScheduler {
	#region SchedulerViewBase
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public abstract class SchedulerViewBase : UserInterfaceObject, IDisposable, IInnerSchedulerViewOwner, ISchedulerViewRepositoryItem, IViewAsyncSupport {
		#region Fields
		const bool defaultDateTimeScrollbarVisible = true;
		internal const SchedulerScrollBarVisibility defaultContainerScrollbarVisible = SchedulerScrollBarVisibility.Never;
		const SchedulerScrollBarVisibility defaultContainerScrollBarVisibility = SchedulerScrollBarVisibility.Never;
		SchedulerControl control;
		InnerSchedulerViewBase innerView;
		SchedulerViewInfoBase viewInfo;
		BaseViewAppearance appearance;
		ViewFactoryHelper factoryHelper;
		int groupSeparatorWidth;
		bool dateTimeScrollbarVisible = defaultDateTimeScrollbarVisible;
		SchedulerScrollBarVisibility containerScrollBarVisibility = defaultContainerScrollBarVisibility;
		bool isDisposed;
		bool forceUseSyncMode = false;
		ViewInfoThreadManager threadManager;
		#endregion
		protected SchedulerViewBase(SchedulerControl control)
			: base(null, string.Empty) {
			this.threadManager = new ViewInfoThreadManager();
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
		#region Properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SchedulerControl Control { get { return control; } }
		#region IUserInterfaceObject implementation
		#region DisplayName
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerViewBaseDisplayName"),
#endif
 Localizable(true)]
		public override string DisplayName {
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
		protected internal override bool ShouldSerializeDisplayName() {
			if (innerView != null)
				return innerView.ShouldSerializeDisplayName();
			else
				return false;
		}
		protected internal override void ResetDisplayName() {
			if (innerView != null)
				innerView.ResetDisplayName();
		}
		#endregion
		#region MenuCaption
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerViewBaseMenuCaption"),
#endif
 Localizable(true)]
		public override string MenuCaption {
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
		protected internal override bool ShouldSerializeMenuCaption() {
			if (innerView != null)
				return innerView.ShouldSerializeMenuCaption();
			else
				return false;
		}
		protected internal override void ResetMenuCaption() {
			if (innerView != null)
				innerView.ResetMenuCaption();
		}
		#endregion
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override object Id {
			get { return Type; }
		}
		#endregion
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public abstract SchedulerViewType Type { get; }
		#region InnerVisibleIntervals
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal TimeIntervalCollection InnerVisibleIntervals {
			get {
				if (innerView != null)
					return innerView.InnerVisibleIntervals;
				else
					return null;
			}
		}
		#endregion
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Obsolete("You should use 'GetVisibleIntervals' and 'SetVisibleIntervals' methods instead", true)]
		public TimeIntervalCollection VisibleIntervals { get { return null; } }
		#region SelectedInterval
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TimeInterval SelectedInterval {
			get {
				SchedulerViewSelection selection = Control.Selection;
				if (selection != null)
					return selection.Interval.Clone();
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
	DevExpressXtraSchedulerLocalizedDescription("SchedulerViewBaseEnabled"),
#endif
		DefaultValue(SchedulerViewPropertiesBase.defaultEnabled)]
		public virtual bool Enabled {
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
		public SchedulerViewInfoBase ViewInfo {
			get { return viewInfo; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BaseViewAppearance Appearance { get { return appearance; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal ViewFactoryHelper FactoryHelper { get { return factoryHelper; } }
		#region FilteredResources
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal ResourceBaseCollection FilteredResources {
			get {
				if (innerView != null)
					return innerView.FilteredResources;
				else
					return null;
			}
		}
		#endregion
		#region FilteredAppointments
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal AppointmentBaseCollection FilteredAppointments {
			get {
				if (innerView != null)
					return innerView.FilteredAppointments;
				else
					return null;
			}
		}
		#endregion
		#region VisibleResources
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal ResourceBaseCollection VisibleResources {
			get {
				if (innerView != null)
					return innerView.VisibleResources;
				else
					return null;
			}
		}
		#endregion
		#region GroupSeparatorWidth
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerViewBaseGroupSeparatorWidth"),
#endif
DefaultValue(0), XtraSerializableProperty(), Category(SRCategoryNames.Appearance)]
		public int GroupSeparatorWidth {
			get { return groupSeparatorWidth; }
			set {
				if (groupSeparatorWidth == value)
					return;
				groupSeparatorWidth = value;
				RaiseChanged(SchedulerControlChangeType.ActiveViewChanged);
			}
		}
		#endregion
		#region DateTimeScrollbarVisible
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerViewBaseDateTimeScrollbarVisible"),
#endif
DefaultValue(defaultDateTimeScrollbarVisible), XtraSerializableProperty(), Category(SRCategoryNames.Appearance)]
		public bool DateTimeScrollbarVisible {
			get { return dateTimeScrollbarVisible; }
			set {
				if (dateTimeScrollbarVisible == value)
					return;
				dateTimeScrollbarVisible = value;
				RaiseChanged(SchedulerControlChangeType.DateTimeScrollbarVisibilityChanged);
			}
		}
		#endregion
		#region VisibleStart
		protected internal DateTime VisibleStart {
			get {
				if (innerView != null)
					return innerView.VisibleStart;
				else
					return DateTime.MinValue;
			}
		}
		#endregion
		#region FirstVisibleResourceIndex
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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
	DevExpressXtraSchedulerLocalizedDescription("SchedulerViewBaseResourcesPerPage"),
#endif
 DefaultValue(0), XtraSerializableProperty()]
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
		#region ActualFirstVisibleResourceIndex
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal int ActualFirstVisibleResourceIndex {
			get {
				if (innerView != null)
					return innerView.ActualFirstVisibleResourceIndex;
				else
					return 0;
			}
		}
		#endregion
		#region ActualResourcesPerPage
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal int ActualResourcesPerPage {
			get {
				if (innerView != null)
					return innerView.ActualResourcesPerPage;
				else
					return 0;
			}
		}
		#endregion
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
		#region AppointmentDisplayOptions
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerViewBaseAppointmentDisplayOptions"),
#endif
Category(SRCategoryNames.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public AppointmentDisplayOptions AppointmentDisplayOptions {
			get {
				if (innerView != null)
					return innerView.AppointmentDisplayOptions;
				else
					return null;
			}
		}
		#endregion
		#region ShowMoreButtons
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerViewBaseShowMoreButtons"),
#endif
DefaultValue(InnerSchedulerViewBase.defaultShowMoreButtons), XtraSerializableProperty()]
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
	DevExpressXtraSchedulerLocalizedDescription("SchedulerViewBaseNavigationButtonVisibility"),
#endif
DefaultValue(InnerSchedulerViewBase.defaultNavigationButtonVisibility), XtraSerializableProperty()]
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
	DevExpressXtraSchedulerLocalizedDescription("SchedulerViewBaseNavigationButtonAppointmentSearchInterval"),
#endif
XtraSerializableProperty()]
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
		protected internal virtual bool AllowVScroll { get { return true; } }
		protected internal virtual bool AllowHScroll { get { return true; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public abstract SchedulerMenuItemId MenuItemId { get; }
		#region ShortDisplayName
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerViewBaseShortDisplayName"),
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
		protected internal virtual SchedulerScrollBarVisibility ContainerScrollBarVisibility {
			get { return containerScrollBarVisibility; }
			set {
				if (containerScrollBarVisibility == value)
					return;
				containerScrollBarVisibility = value;
				OnContainerScrollBarVisibilityChanged();
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
		[Obsolete("You should use the 'AppointmentDisplayOptions.AppointmentHeight' property instead", false),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Category(SRCategoryNames.Appearance)]
		public int AppointmentHeight {
			get { return AppointmentDisplayOptions.AppointmentHeight; }
			set { AppointmentDisplayOptions.AppointmentHeight = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Obsolete("You should use the 'SchedulerControl.Bounds' or 'SchedulerViewInfoBase.Bounds' property instead", false)]
		public Rectangle Bounds { get { return Control.Bounds; } set { Control.Bounds = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Obsolete("You should use the 'SchedulerViewInfoBase.Painter' property instead", false)]
		public ViewPainterBase Painter {
			get {
				if (viewInfo != null)
					return viewInfo.Painter;
				else
					return null;
			}
		}
		protected internal virtual bool HeaderAlternateEnabled { get { return true; } }
		protected internal virtual bool DrawMoreButtonsOverAppointments { get { return true; } }
		internal InnerSchedulerViewBase InnerView { get { return innerView; } }
		internal virtual bool ShowResourceHeadersInternal { get { return true; } }
		protected internal ViewInfoThreadManager ThreadManager {
			get { return this.threadManager; }
		}
		ViewInfoThreadManager IViewAsyncSupport.ThreadManager {
			get { return this.threadManager; }
		}
		protected bool UseAsyncMode {
			get { return Control.OptionsBehavior != null && Control.OptionsBehavior.UseAsyncMode && !this.forceUseSyncMode; }
		}
		bool IViewAsyncSupport.UseAsyncMode {
			get { return UseAsyncMode; }
		}
		#endregion
		protected internal abstract InnerSchedulerViewBase CreateInnerView();
		protected internal abstract ViewDateTimeScrollController CreateDateTimeScrollController();
		protected internal abstract SchedulerViewInfoBase CreateViewInfoCore();
		protected internal abstract void InitializeViewInfo(SchedulerViewInfoBase viewInfo);
		protected internal abstract BaseViewAppearance CreateAppearance();
		protected internal abstract ViewFactoryHelper CreateFactoryHelper();
		protected internal virtual void Initialize() {
			this.innerView = CreateInnerView();
			this.innerView.Initialize(Control.Selection);
			this.appearance = CreateAppearance();
			this.factoryHelper = CreateFactoryHelper();
			SubscribeAppearanceEvents();
		}
		#region IInnerSchedulerViewOwner implementation
		WorkDaysCollection IInnerSchedulerViewOwner.WorkDays { get { return Control.InnerControl.WorkDays; } }
		DayOfWeek IInnerSchedulerViewOwner.FirstDayOfWeek { get { return Control.InnerControl.FirstDayOfWeek; } }
		string IInnerSchedulerViewOwner.ClientTimeZoneId { get { return Control.InnerControl.OptionsBehavior.ClientTimeZoneId; } }
		ResourceBaseCollection IInnerSchedulerViewOwner.GetFilteredResources() {
			return Control.InnerControl.GetFilteredResources();
		}
		ResourceBaseCollection IInnerSchedulerViewOwner.GetResourcesTree() {
			return Control.InnerControl.GetResourcesTree();
		}
		AppointmentBaseCollection IInnerSchedulerViewOwner.GetFilteredAppointments(TimeInterval interval, ResourceBaseCollection resources, out bool appointmentsAreReloaded) {
			return Control.InnerControl.GetFilteredAppointments(interval, resources, out appointmentsAreReloaded);
		}
		AppointmentBaseCollection IInnerSchedulerViewOwner.GetNonFilteredAppointments() {
			return Control.InnerControl.GetNonFilteredAppointments();
		}
		AppointmentDisplayOptions IInnerSchedulerViewOwner.CreateAppointmentDisplayOptions() {
			return CreateAppointmentDisplayOptionsCore();
		}
		void IInnerSchedulerViewOwner.UpdateSelection(SchedulerViewSelection selection) {
			ViewInfo.UpdateSelection(selection);
		}
		#endregion
		#region ISchedulerViewRepositoryItem
		InnerSchedulerViewBase ISchedulerViewRepositoryItem.InnerView { get { return this.innerView; } }
		void ISchedulerViewRepositoryItem.Initialize(InnerSchedulerControl control) {
			this.Initialize();
		}
		void ISchedulerViewRepositoryItem.Reset() {
			this.Reset();
		}
		#endregion
		public virtual SchedulerHitInfo CalcHitInfo(Point pt, bool layoutOnly) {
			if (ViewInfo == null)
				return SchedulerHitInfo.None;
			return ViewInfo.CalcHitInfo(pt, layoutOnly);
		}
		protected internal virtual AppointmentDisplayOptions CreateAppointmentDisplayOptionsCore() {
			return new AppointmentDisplayOptions();
		}
		protected virtual TimeIndicatorDisplayOptions CreateTimeIndicatorDisplayOptions() {
			return new TimeIndicatorDisplayOptions();
		}
		protected internal virtual TimeInterval RoundSelectionInterval(TimeInterval interval) {
			return innerView.RoundSelectionInterval(interval);
		}
		protected internal virtual TimeInterval CreateDefaultSelectionInterval(DateTime date) {
			return innerView.CreateDefaultSelectionInterval(date);
		}
		protected internal virtual void CreateViewInfo() {
			SchedulerViewInfoBase newViewInfo = CreateViewInfoCore();
			if (this.viewInfo != null)
				DestroyViewInfo();
			this.viewInfo = newViewInfo;
			InitializeViewInfo(ViewInfo);
			ViewInfo.Painter = CreatePainter(control.PaintStyle);
			ViewInfo.Initialize();
		}
		protected internal virtual void DestroyViewInfo() {
			EnsureCalculationsAreFinished();
			if (this.viewInfo != null) {
				this.viewInfo.Dispose();
				this.viewInfo = null;
			}
		}
		protected internal virtual void RaiseChanged(SchedulerControlChangeType changeType) {
			innerView.RaiseChanged(changeType);
		}
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (this.viewInfo != null) {
					DestroyViewInfo();
					this.viewInfo = null;
				}
				if (appearance != null) {
					UnsubscribeAppearanceEvents();
					appearance.Dispose();
					appearance = null;
				}
				if (innerView != null) {
					innerView.Dispose();
					innerView = null;
				}
				this.factoryHelper = null;
			}
			isDisposed = true;
		}
		public void Dispose() {
			Dispose(true);
		}
		internal bool IsDisposed { get { return isDisposed; } }
		#endregion
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Obsolete("You should use 'SchedulerControl.BeginUpdate' instead", false)]
		public void BeginUpdate() {
			control.BeginUpdate();
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Obsolete("You should use 'SchedulerControl.EndUpdate' instead", false)]
		public void EndUpdate() {
			control.EndUpdate();
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Obsolete("You should use 'SchedulerControl.IsUpdateLocked' instead", false)]
		public bool IsUpdateLocked { get { return control.IsUpdateLocked; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Obsolete("You should use 'SchedulerControl.BeginUpdate' instead", false)]
		public void SuspendLayout() {
			control.BeginUpdate();
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Obsolete("You should use 'SchedulerControl.EndUpdate' instead", false)]
		public void ResumeLayout() {
			control.EndUpdate();
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Obsolete("You should use 'SchedulerControl.IsUpdateLocked' instead", false)]
		public bool LayoutLocked { get { return control.IsUpdateLocked; } }
		#region SubscribeAppearanceEvents
		protected internal virtual void SubscribeAppearanceEvents() {
			appearance.Changed += new EventHandler(OnAppearanceChanged);
		}
		#endregion
		#region UnsubscribeAppearanceEvents
		protected internal virtual void UnsubscribeAppearanceEvents() {
			appearance.Changed -= new EventHandler(OnAppearanceChanged);
		}
		#endregion
		protected internal virtual void OnAppearanceChanged(object sender, EventArgs e) {
			RaiseChanged(SchedulerControlChangeType.AppearanceChanged);
		}
		public void LayoutChanged() {
			NotifyControlChanges(SchedulerControlChangeType.PerformViewLayoutChanged);
		}
		protected virtual void NotifyControlChanges(SchedulerControlChangeType change) {
			Control.ApplyChanges(change);
		}
		protected internal virtual void RecalcPreliminaryLayout() {
			RecalcPreliminaryLayout(control.ViewBounds);
		}
		public virtual void ClearPreliminaryAppointmentsAndCellContainers() {
			if (ViewInfo == null)
				return;
			ViewInfo.PreliminaryLayoutResult.PreliminaryAppointmentResult.Clear();
			ViewInfo.PreliminaryLayoutResult.CellContainers.Clear();
		}
		protected internal virtual void RecalcPreliminaryLayout(Rectangle bounds) {
			List<AppointmentIntermediateViewInfoCollection> preliminaryAppointments = ViewInfo.PreliminaryLayoutResult.PreliminaryAppointmentResult;
			SchedulerViewCellContainerCollection cellContainers = ViewInfo.PreliminaryLayoutResult.CellContainers;
			bool isResultCalculated = ViewInfo.PreliminaryLayoutResult.Calculated;
			ViewInfo.PreliminaryLayoutResult.Dispose();
			ViewInfo.PreliminaryLayoutResult = null;
			ViewInfo.PreliminaryLayoutResult = ViewInfo.CreatePreliminaryLayoutResult();
			if (isResultCalculated) {
				ViewInfo.PreliminaryLayoutResult.PreliminaryAppointmentResult.AddRange(preliminaryAppointments);
				ViewInfo.PreliminaryLayoutResult.CellContainers.AddRange(cellContainers);
			}
			ResetCellScrollBarsVisibility();
			ViewInfo.Bounds = bounds;
			ViewInfo.CalcPreliminaryLayout();
		}
		protected internal virtual void ResetCellScrollBarsVisibility() {
			Control.CellScrollBarsRegistrator.ResetScrollBarsVisibility();
		}
		public virtual void RecalcScrollBarVisibility() {
			RecalcScrollBarVisibility(control.ViewBounds);
		}
		public void RecalcScrollBarVisibility(Rectangle bounds) {
			ViewInfo.Bounds = bounds;
			ViewInfo.CalcScrollBarVisibility();
		}
		public virtual void RecreateViewInfo() {
			ViewInfoBasePreliminaryLayoutResult preliminaryResult = null;
			if (ViewInfo != null)
				preliminaryResult = ViewInfo.PreliminaryLayoutResult;
			CreateViewInfo();
			if (preliminaryResult != null)
				ViewInfo.PreliminaryLayoutResult = preliminaryResult;
		}
		protected internal virtual void RecalcFinalLayout() {
			RecalcFinalLayout(control.ViewBounds);
		}
		public virtual void RecalcFinalLayout(Rectangle bounds) {
			ViewInfo.Bounds = bounds;
			ViewInfo.CalcFinalLayout();
		}
		protected internal virtual ChangeActions PrepareChangeActions() {
			if (ViewInfo == null)
				return ChangeActions.None;
			return ViewInfo.PrepareChangeActions(control.ViewBounds);
		}
		protected internal virtual void EnsureCalculationsAreFinished() {
			if (ViewInfo == null)
				return;
			ViewInfo.CancellationToken.Cancel();
			this.threadManager.WaitForAllThreads();
		}
		protected internal abstract ViewPainterBase CreatePainter(SchedulerPaintStyle paintStyle);
		protected internal abstract bool ChangeResourceScrollBarOrientationIfNeeded(ResourceNavigator navigator);
		protected internal abstract bool ChangeDateTimeScrollBarOrientationIfNeeded(DateTimeScrollBar scrollBar);
		protected internal virtual ToolTipControlInfo CalculateToolTipInfo(Point pt, ToolTipVisibility visibility) {
			return ViewInfo.CalculateToolTipInfo(pt, visibility);
		}
		public TimeIntervalCollection GetVisibleIntervals() {
			return innerView.GetVisibleIntervals();
		}
		public virtual void SetVisibleIntervals(TimeIntervalCollection intervals) {
			if (intervals == null)
				Exceptions.ThrowArgumentException("intervals", intervals);
			innerView.SetVisibleIntervals(intervals, Control.Selection);
		}
		public void Invalidate() {
			Control.Invalidate(Control.ViewBounds);
		}
		public virtual void ChangeAppointmentSelection(Appointment apt) {
			AppointmentSelectionController controller = Control.AppointmentSelectionController;
			if (controller == null)
				return;
			if (controller.SelectSingleAppointment(apt))
				MakeAppointmentVisibleInScrollContainers(apt);
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
		[Obsolete("You should use the 'ViewPainterBase.Draw' instead", false)]
		public virtual void Draw(GraphicsInfoArgs args) {
			if (viewInfo != null && viewInfo.Painter != null)
				viewInfo.Painter.Draw(args, viewInfo);
		}
		public virtual void SetSelection(TimeInterval interval, Resource resource) {
			if (interval == null)
				Exceptions.ThrowArgumentException("interval", interval);
			if (resource == null)
				Exceptions.ThrowArgumentException("resource", resource);
			if (Control.Selection == null)
				return;
			SetSelectionCore(interval, resource);
		}
		protected internal virtual void SetSelectionCore(TimeInterval interval, Resource resource) {
			SetSelectionCommand command = CreateSetSelectionCommand(Control.InnerControl, interval, resource);
			command.Execute();
		}
		protected internal virtual SetSelectionCommand CreateSetSelectionCommand(InnerSchedulerControl control, TimeInterval interval, Resource resource) {
			return new SetSelectionCommand(control, interval, resource);
		}
		public virtual void GotoTimeInterval(TimeInterval interval) {
			if (interval == null)
				Exceptions.ThrowArgumentException("interval", interval);
			if (Control.Selection == null)
				return;
			SetSelectionCore(interval, Control.Selection.Resource);
		}
		public virtual void SelectAppointment(Appointment apt) {
			if (apt == null)
				Exceptions.ThrowArgumentException("apt", apt);
			SelectAppointmentCore(apt, apt.ResourceId);
		}
		public virtual void SelectAppointment(Appointment apt, Resource resource) {
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
			Resource resource = FilteredResources.GetResourceById(resourceId);
			if (resource == ResourceBase.Empty)
				resource = Control.Selection.Resource;
			Control.SuspendSelectionPaint();
			try {
				TimeInterval serverSelectionInterval = ((IInternalAppointment)apt).CreateInterval();
				TimeInterval clientSelectionInterval = Control.TimeZoneHelper.ToClientTime(serverSelectionInterval);
				SetSelectionCore(clientSelectionInterval, resource);
				ChangeAppointmentSelection(apt);
			} finally {
				Control.ResumeSelectionPaint();
			}
		}
		protected internal virtual void Reset() {
			innerView.Reset();
		}
		protected internal virtual SchedulerColorSchema GetResourceColorSchema(Resource resource, int resourceColorIndex) {
			QueryResourceColorSchemaEventArgs args = new QueryResourceColorSchemaEventArgs(resource, resourceColorIndex);
			control.RaiseQueryResourceColorSchema(args);
			if (args.ResourceColorSchema != null)
				return (SchedulerColorSchema)args.ResourceColorSchema;
			return GetColorSchema(resource.GetColor(), resourceColorIndex);
		}
		protected internal virtual SchedulerColorSchema GetColorSchema(Color color, int resourceColorIndex) {
			return Control.ActualResourceColorSchemas.GetSchema(color, resourceColorIndex);
		}
		protected internal virtual ProcessorBase<Appointment> CreateResourcesAppointmentsFilter(ResourceBaseCollection resources) {
			return Control.InnerControl.CreateResourcesAppointmentsFilter(resources);
		}
		public virtual ResourceBaseCollection GetFilteredResources() {
			if (innerView != null && Object.ReferenceEquals(Control.ActiveView, this)) {
				ResourceBaseCollection result = new ResourceBaseCollection();
				result.AddRange(innerView.FilteredResources);
				return result;
			} else
				return new ResourceBaseCollection();
		}
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
		protected internal virtual void OnContainerScrollBarVisibilityChanged() {
			RaiseChanged(SchedulerControlChangeType.ScrollBarVisibilityChanged);
		}
		protected virtual void MakeAppointmentVisibleInScrollContainers(Appointment apt) {
		}
		protected internal int CalculateVerticalDateTimeScrollBarTop() {
			return ViewInfo != null ? ViewInfo.CalculateVerticalDateTimeScrollBarTop() : 0;
		}
		void IViewAsyncSupport.ForceSyncMode() {
			ThreadManager.WaitForAllThreads();
			this.forceUseSyncMode = true;
		}
		void IViewAsyncSupport.ResetForceSyncMode() {
			ThreadManager.WaitForAllThreads();
			this.forceUseSyncMode = false;
		}
	}
	#endregion
	#region SchedulerViewRepository
	public class SchedulerViewRepository : SchedulerViewTypedRepositoryBase<SchedulerViewBase> {
		public SchedulerViewRepository() {
		}
		#region Properties
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerViewRepositoryDayView"),
#endif
Category(SRCategoryNames.View), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public DayView DayView { get { return (DayView)this[SchedulerViewType.Day]; } }
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerViewRepositoryWorkWeekView"),
#endif
Category(SRCategoryNames.View), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public WorkWeekView WorkWeekView { get { return (WorkWeekView)this[SchedulerViewType.WorkWeek]; } }
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerViewRepositoryWeekView"),
#endif
Category(SRCategoryNames.View), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public WeekView WeekView { get { return (WeekView)this[SchedulerViewType.Week]; } }
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerViewRepositoryMonthView"),
#endif
Category(SRCategoryNames.View), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public MonthView MonthView { get { return (MonthView)this[SchedulerViewType.Month]; } }
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerViewRepositoryTimelineView"),
#endif
Category(SRCategoryNames.View), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public TimelineView TimelineView { get { return (TimelineView)this[SchedulerViewType.Timeline]; } }
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerViewRepositoryGanttView"),
#endif
Category(SRCategoryNames.View), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public GanttView GanttView { get { return (GanttView)this[SchedulerViewType.Gantt]; } }
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("SchedulerViewRepositoryFullWeekView"),
#endif
Category(SRCategoryNames.View), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public FullWeekView FullWeekView { get { return (FullWeekView)this[SchedulerViewType.FullWeek]; } }
		#endregion
		protected internal override void CreateViews(InnerSchedulerControl control) {
			SchedulerControl winControl = (SchedulerControl)control.Owner;
			RegisterView(new DayView(winControl));
			RegisterView(new WorkWeekView(winControl));
			RegisterView(new FullWeekView(winControl));
			RegisterView(new WeekView(winControl));
			RegisterView(new MonthView(winControl));
			RegisterView(new TimelineView(winControl));
			RegisterView(new GanttView(winControl));
		}
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Drawing {
	#region ViewGroupTypeStrategy
	public abstract class ViewGroupTypeStrategy {
		public abstract SchedulerViewHeadersLayoutCalculator CreateHeadersLayoutCalculator(GraphicsCache cache, SchedulerViewInfoBase viewInfo, SchedulerHeaderPainter painter);
		public abstract SchedulerViewCellsLayoutCalculator CreateCellsLayoutCalculator(GraphicsCache cache, SchedulerViewInfoBase viewInfo, ViewInfoPainterBase painter);
		public abstract VisuallyContinuousCellsInfosCalculator CreateVisuallyContinuousCellsInfosCalculator(SchedulerViewInfoBase viewInfo, bool alternate);
		public virtual NavigationButtonsLayoutCalculator CreateNavigationButtonsLayoutCalculator(PrevNextAppointmentIntervalPairCollection data, GraphicsCache cache, SchedulerViewInfoBase viewInfo, NavigationButtonPainter painter) {
			return new NavigationButtonsLayoutCalculator(data, cache, viewInfo, painter, false);
		}
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
		public virtual SchedulerViewHeadersLayoutCalculator CreateHeadersLayoutCalculator(SchedulerViewInfoBase viewInfo, GraphicsCache cache, SchedulerHeaderPainter painter) {
			ViewGroupTypeStrategy strategy = SelectStrategy(viewInfo.View);
			return strategy.CreateHeadersLayoutCalculator(cache, viewInfo, painter);
		}
		public virtual SchedulerViewCellsLayoutCalculator CreateCellsLayoutCalculator(SchedulerViewInfoBase viewInfo, GraphicsCache cache, ViewInfoPainterBase painter) {
			ViewGroupTypeStrategy strategy = SelectStrategy(viewInfo.View);
			return strategy.CreateCellsLayoutCalculator(cache, viewInfo, painter);
		}
		public virtual VisuallyContinuousCellsInfosCalculator CreateVisuallyContinuousCellsInfosCalculator(SchedulerViewInfoBase viewInfo, bool alternate) {
			ViewGroupTypeStrategy strategy = SelectStrategy(viewInfo.View);
			return strategy.CreateVisuallyContinuousCellsInfosCalculator(viewInfo, alternate);
		}
		public virtual NavigationButtonsLayoutCalculator CreateNavigationButtonsLayoutCalculator(PrevNextAppointmentIntervalPairCollection data, GraphicsCache cache, SchedulerViewInfoBase viewInfo, NavigationButtonPainter painter) {
			ViewGroupTypeStrategy strategy = SelectStrategy(viewInfo.View);
			return strategy.CreateNavigationButtonsLayoutCalculator(data, cache, viewInfo, painter);
		}
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
		public abstract AppointmentsBaseLayoutStrategy CreateAppointmentsLayoutStrategy(SchedulerViewBase view);
	}
	#endregion
}
