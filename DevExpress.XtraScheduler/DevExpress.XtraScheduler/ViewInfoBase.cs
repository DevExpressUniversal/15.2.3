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
using System.Linq;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Printing.Native;
using DevExpress.XtraScheduler.Services;
using DevExpress.XtraScheduler.Internal.Implementations;
using DevExpress.XtraScheduler.Internal;
namespace DevExpress.XtraScheduler.Drawing {
	#region ISchedulerObjectAnchor
	public interface ISchedulerObjectAnchor {
		Resource Resource { get; set; }
		TimeInterval Interval { get; set; }
		Rectangle Bounds { get; set; }
	}
	public interface ISchedulerObjectAnchorCollection {
		ISchedulerObjectAnchor this[int index] { get; }
		int Count { get; }
	}
	#endregion
	#region SchedulerViewInfoBase (abstract class)
	public abstract class SchedulerViewInfoBase : IDisposable, IPrintableObjectViewInfo, ISupportAppointments, IViewAsyncAccessor {
		#region Fields
		SchedulerViewBase view;
		Rectangle bounds;
		BaseViewAppearance paintAppearance;
		SchedulerHeaderCollection resourceHeaders;
		SchedulerHeaderCollection groupSeparators;
		ViewPainterBase painter;
		SchedulerViewCellContainerCollection cellContainers;
		MoreButtonCollection moreButtons;
		NavigationButtonCollection navigationButtons;
		bool overriddenAppointmentForeColor;
		bool isDisposed;
		IAppointmentComparerProvider appointmentComparerProvider;
		ViewInfoBasePreliminaryLayoutResult preliminaryLayoutResult;
		SchedulerCancellationTokenSource tokenSource;
		#endregion
		protected SchedulerViewInfoBase(SchedulerViewBase view) {
			if (view == null)
				Exceptions.ThrowArgumentException("view", view);
			this.view = view;
			this.paintAppearance = view.CreateAppearance();
			this.resourceHeaders = new SchedulerHeaderCollection();
			this.groupSeparators = new SchedulerHeaderCollection();
			this.moreButtons = new MoreButtonCollection();
			this.navigationButtons = new NavigationButtonCollection();
			this.cellContainers = CreateCellContainers();
			this.preliminaryLayoutResult = CreatePreliminaryLayoutResult();
			this.tokenSource = new SchedulerCancellationTokenSource();
		}
		#region Properties
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("SchedulerViewInfoBaseView")]
#endif
		public SchedulerViewBase View { get { return view; } }
		internal bool IsDisposed { get { return isDisposed; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ViewPainterBase Painter {
			get { return painter; }
			set { painter = value; }
		}
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("SchedulerViewInfoBasePaintAppearance")]
#endif
		public BaseViewAppearance PaintAppearance { get { return paintAppearance; } }
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("SchedulerViewInfoBaseResourceHeaders")]
#endif
		public SchedulerHeaderCollection ResourceHeaders { get { return resourceHeaders; } }
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("SchedulerViewInfoBaseGroupSeparators")]
#endif
		public SchedulerHeaderCollection GroupSeparators { get { return groupSeparators; } }
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("SchedulerViewInfoBaseBounds")]
#endif
		public Rectangle Bounds { get { return bounds; } set { bounds = value; } }
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("SchedulerViewInfoBaseVisibleResources")]
#endif
		public ResourceBaseCollection VisibleResources { get { return View.VisibleResources; } }
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("SchedulerViewInfoBaseVisibleIntervals")]
#endif
		public TimeIntervalCollection VisibleIntervals { get { return View.InnerVisibleIntervals; } }
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("SchedulerViewInfoBaseCellContainers")]
#endif
		public SchedulerViewCellContainerCollection CellContainers { get { return cellContainers; } }
#if DEBUGTEST
		public AppointmentViewInfoCollection AppointmentViewInfos {
			get { return CopyAllAppointmentViewInfos(); }
		}
#endif
		public MoreButtonCollection MoreButtons { get { return moreButtons; } }
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("SchedulerViewInfoBaseNavigationButtons")]
#endif
		public NavigationButtonCollection NavigationButtons { get { return navigationButtons; } }
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("SchedulerViewInfoBaseAppointmentHeight")]
#endif
		public int AppointmentHeight { get { return AppointmentDisplayOptions.AppointmentHeight; } set { AppointmentDisplayOptions.AppointmentHeight = value; } }
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("SchedulerViewInfoBaseAppointmentAutoHeight")]
#endif
		public bool AppointmentAutoHeight { get { return AppointmentDisplayOptions.AppointmentAutoHeight; } set { AppointmentDisplayOptions.AppointmentAutoHeight = value; } }
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("SchedulerViewInfoBaseAppointmentDisplayOptions")]
#endif
		public AppointmentDisplayOptions AppointmentDisplayOptions { get { return View.AppointmentDisplayOptions; } }
		internal ViewInfoBasePreliminaryLayoutResult PreliminaryLayoutResult {
			get { return preliminaryLayoutResult; }
			set { this.preliminaryLayoutResult = value; }
		}
		IAppointmentComparerProvider ISupportAppointmentsBase.AppointmentComparerProvider {
			get {
				if (appointmentComparerProvider == null) {
					appointmentComparerProvider = View.Control.GetService<IAppointmentComparerProvider>();
				}
				if (appointmentComparerProvider == null)
					appointmentComparerProvider = new AppointmentComparerProvider(View.Control);
				return appointmentComparerProvider;
			}
		}
		protected virtual bool UseAsyncMode {
			get {
				IViewAsyncSupport viewAsyncSupport = view as IViewAsyncSupport;
				if (viewAsyncSupport == null)
					return false;
				return viewAsyncSupport.UseAsyncMode;
			}
		}
		IViewAsyncSupport IViewAsyncAccessor.View {
			get { return View; }
		}
		#endregion
		protected internal TimeZoneHelper TimeZoneHelper { get { return View.Control.InnerControl.TimeZoneHelper; } }
		protected internal bool OverriddenAppointmentForeColor { get { return overriddenAppointmentForeColor; } }
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (paintAppearance != null) {
					paintAppearance.Dispose();
					paintAppearance = null;
				}
				ReleaseScrollContainers();
				if (moreButtons != null) {
					moreButtons.Clear();
					moreButtons = null;
				}
				if (appointmentComparerProvider != null)
					appointmentComparerProvider = null;
				if (this.tokenSource != null) {
					this.tokenSource.Dispose();
					this.tokenSource = null;
				}
			}
			this.isDisposed = true;
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~SchedulerViewInfoBase() {
			Dispose(false);
		}
		#endregion
		protected internal virtual void PrepareAppointmentLayout() {
		}
		protected internal virtual SchedulerViewCellContainerCollection CreateCellContainers() {
			return new SchedulerViewCellContainerCollection();
		}
		protected internal void SubsribeScrollContainerEvents(SchedulerViewCellContainer container) {
			if (container != null)
				container.ScrollBarValueChanged += new EventHandler(OnContainerScrollBarValueChanged);
		}
		protected internal virtual void ReleaseScrollContainers() {
		}
		protected internal void UnsubsribeScrollContainerEvents(SchedulerViewCellContainer container) {
			if (container != null)
				container.ScrollBarValueChanged -= new EventHandler(OnContainerScrollBarValueChanged);
		}
		void OnContainerScrollBarValueChanged(object sender, EventArgs e) {
			View.Control.ApplyChanges(SchedulerControlChangeType.CellContentScroll);
		}
		protected internal virtual void DisposeCellContainers(SchedulerViewCellContainerCollection cellContainers) {
			for (int i = 0; i < cellContainers.Count; i++) {
				cellContainers[i].Dispose();
			}
			cellContainers.Clear();
		}
		public IEnumerable<AppointmentViewInfo> GetAllAppointmentViewInfos() {
			SchedulerViewCellContainerCollection cellContainers = GetCellContainers();
			return cellContainers.SelectMany(c => c.AppointmentViewInfos);
		}
		public AppointmentViewInfoCollection CopyAllAppointmentViewInfos() {
			SchedulerViewCellContainerCollection cellContainers = GetCellContainers();
			int count = cellContainers.Count;
			AppointmentViewInfoCollection[] containerViewInfos = new AppointmentViewInfoCollection[count];
			for (int i = 0; i < count; i++) {
				lock (cellContainers[i].AppointmentViewInfos)
					containerViewInfos[i] = new AppointmentViewInfoCollection(cellContainers[i].AppointmentViewInfos);
			}
			AppointmentViewInfoCollection result = new AppointmentViewInfoCollection(containerViewInfos.Sum(c => c.Count));
			foreach (AppointmentViewInfoCollection viewInfos in containerViewInfos)
				result.AddRange(viewInfos);
			return result;
		}
		public virtual SchedulerViewCellContainerCollection GetCellContainers() {
			return CellContainers;
		}
		public virtual void CalcPreliminaryLayout() {
			GraphicsInfo gInfo = new GraphicsInfo();
			gInfo.AddGraphics(null);
			try {
				CalcPreliminaryLayoutCore(gInfo.Cache);
			} finally {
				gInfo.ReleaseGraphics();
			}
		}
		public void Initialize() {
			CalculatePaintAppearance(Painter);
		}
		public virtual void CalcScrollBarVisibility() {
			GraphicsInfo gInfo = new GraphicsInfo();
			gInfo.AddGraphics(null);
			try {
				CalcScrollBarVisibilityCore(gInfo.Cache);
			} finally {
				gInfo.ReleaseGraphics();
			}
		}
		protected virtual void CalcScrollBarVisibilityCore(GraphicsCache cache) {
			if (cache == null)
				Exceptions.ThrowArgumentException("cache", cache);
		}
		public virtual void CalcFinalLayout() {
			GraphicsInfo gInfo = new GraphicsInfo();
			gInfo.AddGraphics(null);
			try {
				CalcFinalLayoutCore(gInfo.Cache);
				UpdateSelection(View.Control.Selection);
			} finally {
				gInfo.ReleaseGraphics();
			}
		}
		public virtual bool MakeAppointmentVisibleInScrollContainers(Appointment appointment) {
			bool result = false;
			SchedulerViewCellContainerCollection containers = GetScrollContainers();
			int count = containers.Count;
			for (int i = 0; i < count; i++) {
				SchedulerViewCellContainer container = containers[i];
				AppointmentViewInfoCollection viewInfos = GetScrollContainerAppointmentViewInfos(container);
				AppointmentViewInfo appointmentViewInfo = SchedulerWinUtils.FindAppointmentViewInfoByAppointment(viewInfos, appointment);
				if (appointmentViewInfo != null)
					result |= container.MakeAppointmentViewInfoVisible(appointmentViewInfo, viewInfos);
			}
			return result;
		}
		public virtual SchedulerHitInfo CalcHitInfo(Point pt, bool layoutOnly) {
			if (layoutOnly) {
				SchedulerHitInfo layoutHitInfo = CalculateLayoutHitInfo(pt);
				SchedulerHitInfo moreButtonHitInfo = CalculateMoreButtonHitInfo(pt, layoutHitInfo);
				SchedulerHitInfo navigationButtonHitInfo = CalculateNavigationButtonHitInfo(pt, moreButtonHitInfo);
				return navigationButtonHitInfo;
			} else
				return CalculateHitInfo(pt);
		}
		protected internal ChangeActions PrepareChangeActions(Rectangle bounds) {
			ChangeActions result;
			GraphicsInfo gInfo = new GraphicsInfo();
			gInfo.AddGraphics(null);
			try {
				result = PrepateChangeActionsCore(gInfo.Cache, bounds);
			} finally {
				gInfo.ReleaseGraphics();
			}
			return result;
		}
		protected virtual ChangeActions PrepateChangeActionsCore(GraphicsCache cache, Rectangle bounds) {
			return PreliminaryLayoutResult.Calculated ? ChangeActions.None : ChangeActions.RecalcPreliminaryLayout;
		}
		protected internal virtual void CalcFinalLayoutCore(GraphicsCache cache) {
			if (cache == null)
				Exceptions.ThrowArgumentException("cache", cache);
			ExecuteHeadersLayoutCalculator(cache, Bounds);
			ExecuteCellsLayoutCalculator(cache, Bounds);
			ClearCellContainerAppointments();
			ExecuteAppointmentLayoutCalculator(cache);
			ExecuteNavigationButtonsLayoutCalculator(cache);
		}
		protected virtual void ClearCellContainerAppointments() {
			PreliminaryLayoutResult.CellContainers.ForEach(c => c.AppointmentViewInfos.Clear());
		}
		protected internal virtual bool ShouldShowContainerScrollBar() {
			SchedulerScrollBarVisibility visibility = View.ContainerScrollBarVisibility;
			return visibility == SchedulerScrollBarVisibility.Always;
		}
		protected internal virtual void CreateContainerScrollBars(SchedulerViewCellContainerCollection containers) {
			int count = containers.Count;
			for (int i = 0; i < count; i++) {
				if (containers[i].ScrollController.ScrollBar == null)
					containers[i].CreateScrollBar(View.Control);
			}
		}
		protected internal virtual void UpdateContainerScrollBars(SchedulerViewCellContainerCollection containers) {
			int count = containers.Count;
			for (int i = 0; i < count; i++) {
				SchedulerViewCellContainer container = containers[i];
				AppointmentViewInfoCollection appointments = GetScrollContainerAppointmentViewInfos(container);
				Rectangle bounds = CalculateScrollContainerBounds(container);
				container.UpdateScrollBar(appointments, bounds, Painter.AppointmentPainter.VerticalInterspacing);
			}
		}
		protected internal virtual Rectangle CalculateScrollContainerBounds(SchedulerViewCellContainer container) {
			return container.Bounds;
		}
		protected internal virtual void ApplySelection(SchedulerViewCellContainer cellContainer) {
			UpdateAppointmentsSelection(cellContainer);
			UpdateAppointmentsDisableDropState(cellContainer);
		}
		protected internal virtual void ExecuteAppointmentLayoutCalculator(GraphicsCache cache) {
			PrepareAppointmentLayout();
			ExecuteAppointmentLayoutCalculatorCore(cache);
		}
		protected internal virtual void ExecuteHeadersLayoutCalculator(GraphicsCache cache, Rectangle bounds) {
			SchedulerViewHeadersLayoutCalculator headersLayoutCalculator = View.FactoryHelper.CreateHeadersLayoutCalculator(this, cache, painter.HorizontalHeaderPainter);
			headersLayoutCalculator.CalcLayout(bounds);
		}
		protected internal virtual void ExecuteCellsLayoutCalculator(GraphicsCache cache, Rectangle bounds) {
			SchedulerViewCellsLayoutCalculator cellsLayoutCalculator = View.FactoryHelper.CreateCellsLayoutCalculator(this, cache, painter.SelectCellsLayoutPainter());
			cellsLayoutCalculator.CalcLayout(bounds);
		}
		protected internal virtual void ExecuteNavigationButtonsLayoutCalculator(GraphicsCache cache) {
			NavigationButtons.Clear();
			WinNavigationButtonCalculator calc = new WinNavigationButtonCalculator(View.Control, this);
			PrevNextAppointmentIntervalPairCollection data = calc.Calculate();
			if (data == null)
				return;
			XtraSchedulerDebug.Assert(data.Count > 0);
			NavigationButtonsLayoutCalculator layoutCalc = View.FactoryHelper.CreateNavigationButtonsLayoutCalculator(data, cache, this, painter.NavigationButtonPainter);
			layoutCalc.CalcLayout(Bounds);
		}
		protected internal virtual void AddNavigationButtons(NavigationButtonCollection buttons) {
			NavigationButtons.AddRange(buttons);
		}
		protected internal virtual void CalcPreliminaryLayoutCore(GraphicsCache cache) {
			if (cache == null)
				Exceptions.ThrowArgumentException("cache", cache);
			RemoveInvalidPreliminaryResults();
			CalculateHeaderPreliminaryLayout(cache);
			CalculateCellsPreliminaryLayout(cache);
			CalculateAppointmentsPreliminaryLayout(cache);
		}
		protected virtual void CalculateHeaderPreliminaryLayout(GraphicsCache cache) {
			SchedulerViewHeadersLayoutCalculator headersLayoutCalculator = (SchedulerViewHeadersLayoutCalculator)View.FactoryHelper.CreateHeadersLayoutCalculator(this, cache, Painter.HorizontalHeaderPainter);
			headersLayoutCalculator.CalculatePreliminaryLayout();
		}
		protected virtual void CalculateCellsPreliminaryLayout(GraphicsCache cache) {
			SchedulerViewCellsLayoutCalculator cellsLayoutCalculator = (SchedulerViewCellsLayoutCalculator)View.FactoryHelper.CreateCellsLayoutCalculator(this, cache, Painter.SelectCellsLayoutPainter());
			cellsLayoutCalculator.CalculatePreliminaryLayout();
		}
		protected virtual void CalculateAppointmentsPreliminaryLayout(GraphicsCache cache) {
			AppointmentsBaseLayoutStrategy layoutStrategy = (AppointmentsBaseLayoutStrategy)View.FactoryHelper.CreateAppointmentsLayoutStrategy(View);
			PreliminaryLayoutResult.PreliminaryAppointmentResult.AddRange(layoutStrategy.CalculateAppointmentsPreliminaryLayout(cache));
		}
		protected internal virtual void CalculatePaintAppearance(ViewPainterBase painter) {
			SchedulerAppearance controlAppearanceCollection = View.Control.Appearance;
			AppearanceDefaultInfo[] defaultAppearances = painter.GetDefaultAppearances();
			BaseViewAppearance viewAppearanceCollection = View.Appearance;
			int count = defaultAppearances.Length;
			for (int i = 0; i < count; i++) {
				AppearanceDefaultInfo defaultAppearance = defaultAppearances[i];
				string appearanceName = defaultAppearance.Name;
				AppearanceObject paintAppearance = PaintAppearance.GetAppearance(appearanceName);
				AppearanceObject controlAppearance = controlAppearanceCollection.GetAppearance(appearanceName);
				AppearanceObject viewAppearance = viewAppearanceCollection.GetAppearance(appearanceName);
				UpdateIsAppearancePropertiesEmpty(appearanceName, viewAppearance, controlAppearance);
				AppearanceHelper.Combine(paintAppearance, new AppearanceObject[] { viewAppearance, controlAppearance }, defaultAppearance.DefaultAppearance);
			}
		}
		protected virtual void UpdateIsAppearancePropertiesEmpty(string appearanceName, AppearanceObject viewAppearance, AppearanceObject controlAppearance) {
			if (appearanceName == SchedulerAppearanceNames.Appointment) {
				this.overriddenAppointmentForeColor = !IsAppearanceForeColorEmpty(viewAppearance) || !IsAppearanceForeColorEmpty(controlAppearance);
			}
		}
		protected bool IsAppearanceForeColorEmpty(AppearanceObject appearance) {
			return appearance.ForeColor == Color.Empty && appearance.Options.UseForeColor == false;
		}
		protected internal BaseHeaderAppearance GetHorizontalResourceHeaderAppearance() {
			BaseHeaderAppearance appearance = new BaseHeaderAppearance();
			appearance.AlternateHeaderCaption.Assign(paintAppearance.AlternateHeaderCaption);
			appearance.AlternateHeaderCaptionLine.Assign(paintAppearance.AlternateHeaderCaptionLine);
			appearance.Selection.Assign(paintAppearance.Selection);
			appearance.HeaderCaption.Assign(paintAppearance.ResourceHeaderCaption);
			appearance.HeaderCaptionLine.Assign(paintAppearance.ResourceHeaderCaptionLine);
			return appearance;
		}
		protected internal BaseHeaderAppearance GetVerticalResourceHeaderAppearance() {
			return GetHorizontalResourceHeaderAppearance();
		}
		protected internal virtual void DisposeAppointmentViewInfos() {
		}
		protected internal virtual void UpdateSelection(SchedulerViewSelection selection) {
			int count = CellContainers.Count;
			for (int i = 0; i < count; i++)
				CellContainers[i].UpdateSelection(selection);
		}
		protected internal virtual void UpdateScrollMoreButtonsVisibility() {
		}
		protected internal virtual ToolTipControlInfo CalculateToolTipInfo(Point pt, ToolTipVisibility visibility) {
			if (visibility == ToolTipVisibility.Never)
				return null;
			SchedulerHitInfo hitInfo = CalculateHitInfo(pt);
			if (hitInfo.HitTest == SchedulerHitTest.None)
				return null;
			SelectableIntervalViewInfo viewInfo = hitInfo.ViewInfo;
			if (CanCreateToolTipControlInfo(viewInfo, visibility)) {
				return new ToolTipControlInfo(viewInfo, viewInfo.ToolTipText); 
			}
			return null;
		}
		protected internal virtual bool CanCreateToolTipControlInfo(SelectableIntervalViewInfo viewInfo, ToolTipVisibility visibility) {
			if (visibility == ToolTipVisibility.Always || viewInfo.ShouldShowToolTip) {
				return !String.IsNullOrEmpty(viewInfo.ToolTipText);
			}
			return false;
		}
		protected internal virtual SchedulerHitInfo CalculateHitInfo(Point pt) {
			SchedulerHitInfo layoutHitInfo = CalculateLayoutHitInfo(pt);
			SchedulerHitInfo dependencyHitInfo = CalculateDependencyHitInfo(pt, layoutHitInfo);
			SchedulerHitInfo appointmentHitInfo = CalculateAppointmentsHitInfo(pt, dependencyHitInfo);
			SchedulerHitInfo moreButtonHitInfo = CalculateMoreButtonHitInfo(pt, appointmentHitInfo);
			SchedulerHitInfo navigationButtonHitInfo = CalculateNavigationButtonHitInfo(pt, moreButtonHitInfo);
			return navigationButtonHitInfo;
		}
		protected internal virtual SchedulerHitInfo CalculateDependencyHitInfo(Point pt, SchedulerHitInfo hitInfo) {
			return hitInfo;
		}
		protected internal virtual SchedulerHitInfo CalculateLayoutHitInfo(Point pt) {
			SchedulerHitInfo layoutHitInfo = CalculateHitInfoCellContainers(pt);
			if (layoutHitInfo.HitTest != SchedulerHitTest.None)
				return layoutHitInfo;
			return CalculateHeaderCollectionHitInfo(pt, ResourceHeaders);
		}
		protected internal virtual AppointmentViewInfoCollection GetScrollContainerAppointmentViewInfos(SchedulerViewCellContainer container) {
			return container.AppointmentViewInfos;
		}
		protected internal virtual SchedulerHitInfo CalculateAppointmentsHitInfo(Point pt, SchedulerHitInfo layoutHitInfo) {
			SchedulerViewCellContainer scrollContainer = ObtainScrollContainer(layoutHitInfo);
			if (scrollContainer == null)
				return layoutHitInfo;
			AppointmentViewInfoCollection appointments = GetScrollContainerAppointmentViewInfos(scrollContainer);
			return CalculateAppointmentsHitInfoCore(PointToScrollContainer(scrollContainer, pt), layoutHitInfo, appointments);
		}
		protected internal virtual SchedulerViewCellContainer ObtainScrollContainer(SchedulerHitInfo layoutHitInfo) {
			SchedulerViewCellBase cell = layoutHitInfo.ViewInfo as SchedulerViewCellBase;
			return cell != null ? layoutHitInfo.NextHitInfo.ViewInfo as SchedulerViewCellContainer : null;
		}
		protected virtual Point PointToScrollContainer(SchedulerViewCellContainer container, Point pt) {
			pt.Y += container.CalculateScrollOffset();
			return pt;
		}
		protected internal virtual SchedulerHitInfo CalculateAppointmentsHitInfoCore(Point pt, SchedulerHitInfo layoutHitInfo, AppointmentViewInfoCollection viewInfos) {
			SchedulerHitInfo suitableHitInfo = layoutHitInfo;
			int count = viewInfos.Count;
			for (int i = 0; i < count; i++) {
				SchedulerHitInfo hitInfo = viewInfos[i].CalculateHitInfo(pt, layoutHitInfo);
				if (hitInfo != layoutHitInfo) {
					if (hitInfo.ViewInfo.Selected)
						return hitInfo;
					if (suitableHitInfo == layoutHitInfo)
						suitableHitInfo = hitInfo;
				}
			}
			return suitableHitInfo;
		}
		protected internal virtual SchedulerHitInfo CalculateNavigationButtonHitInfo(Point pt, SchedulerHitInfo nextHitInfo) {
			int count = NavigationButtons.Count;
			for (int i = 0; i < count; i++) {
				SchedulerHitInfo hitInfo = NavigationButtons[i].CalculateHitInfo(pt, nextHitInfo);
				if (hitInfo != nextHitInfo)
					return hitInfo;
			}
			return nextHitInfo;
		}
		protected internal virtual SchedulerHitInfo CalculateMoreButtonHitInfo(Point pt, SchedulerHitInfo nextHitInfo) {
			return CalculateMoreButtonHitInfoCore(pt, nextHitInfo, MoreButtons);
		}
		protected internal virtual SchedulerHitInfo CalculateMoreButtonHitInfoCore(Point pt, SchedulerHitInfo nextHitInfo, MoreButtonCollection moreButtons) {
			int count = moreButtons.Count;
			for (int i = 0; i < count; i++) {
				SchedulerHitInfo hitInfo = moreButtons[i].CalculateHitInfo(pt, nextHitInfo);
				if (hitInfo != nextHitInfo)
					return hitInfo;
			}
			return nextHitInfo;
		}
		protected internal virtual SchedulerHitInfo CalculateHitInfoCellContainers(Point pt) {
			int count = cellContainers.Count;
			for (int i = 0; i < count; i++) {
				SchedulerHitInfo hitInfo = cellContainers[i].CalculateHitInfo(pt, SchedulerHitInfo.None);
				if (hitInfo.HitTest != SchedulerHitTest.None) {
					return hitInfo;
				}
			}
			return SchedulerHitInfo.None;
		}
		protected internal virtual SchedulerHitInfo CalculateHeaderCollectionHitInfo(Point pt, SchedulerHeaderCollection collection) {
			int count = collection.Count;
			for (int i = 0; i < count; i++) {
				SchedulerHitInfo hitInfo = collection[i].CalculateHitInfo(pt, SchedulerHitInfo.None);
				if (hitInfo.HitTest != SchedulerHitTest.None)
					return hitInfo;
			}
			return SchedulerHitInfo.None;
		}
		protected internal virtual void UpdateAppointmentsSelection(SchedulerViewCellContainer cellContainer) {
			InnerSchedulerControl innerControl = View.Control.InnerControl;
			IAppointmentVisualStateCalculator visualStateCalculator = innerControl.AppointmentVisualStateCalculator;
			lock (cellContainer.AppointmentViewInfos) {
				foreach (AppointmentViewInfo vi in cellContainer.AppointmentViewInfos)
					vi.Selected = visualStateCalculator.IsSelected(vi.Appointment);
			}
		}
		protected internal virtual void UpdateAppointmentsDisableDropState(SchedulerViewCellContainer cellContainer) {
			InnerSchedulerControl innerControl = View.Control.InnerControl;
			IAppointmentVisualStateCalculator visualStateCalculator = innerControl.AppointmentVisualStateCalculator;
			lock (cellContainer.AppointmentViewInfos) {
				foreach (AppointmentViewInfo vi in cellContainer.AppointmentViewInfos)
					vi.DisableDrop = visualStateCalculator.IsDisabled(vi.Appointment);
			}
		}
		protected internal virtual MoreButton CreateMoreButton() {
			return new MoreButton();
		}
		protected internal virtual Size CalculateMoreButtonMinSize() {
			return Painter.MoreButtonPainter.CalculateObjectMinSize();
		}
		protected internal virtual Rectangle GetInplaceEditorBounds(AppointmentViewInfo vi) {
			return Rectangle.Intersect(Bounds, vi.InnerBounds);
		}
		protected internal virtual SchedulerViewCellContainerCollection GetScrollContainers() {
			return CellContainers;
		}
		protected internal virtual int CalculateVerticalDateTimeScrollBarTop() {
			return Bounds.Top;
		}
		protected internal abstract void ExecuteAppointmentLayoutCalculatorCore(GraphicsCache cache);
		protected internal abstract ViewInfoBasePreliminaryLayoutResult CreatePreliminaryLayoutResult();
		protected void RemoveInvalidPreliminaryResults() {
			PreliminaryLayoutResult.PreliminaryAppointmentResult.RemoveAll(par => (!VisibleResources.Contains(par.Resource) && par.Resource != ResourceBase.Empty) || !VisibleIntervals.Contains(par.Interval));
			List<SchedulerViewCellContainer> cellContainersToDelete = PreliminaryLayoutResult.CellContainers.Where(cc => (!VisibleResources.Contains(cc.Resource) && cc.Resource != ResourceBase.Empty) || !VisibleIntervals.Contains(cc.Interval)).ToList();
			foreach (SchedulerViewCellContainer cellContainer in cellContainersToDelete) {
				PreliminaryLayoutResult.CellContainers.Remove(cellContainer);
				cellContainer.Dispose();
			}
		}
		#region IPrintableObjectViewInfo implementation
		void IPrintableObjectViewInfo.Print(GraphicsInfoArgs graphicsInfoArgs) {
			ViewPainterBase painter = View.CreatePainter(new SchedulerFlatPaintStyle());
			painter.Draw(graphicsInfoArgs, this);
		}
		#endregion
		#region IAppointmentsSupport Members
		object ISupportAppointments.AppointmentImages {
			get { return View.Control.AppointmentImages; }
		}
		AppearanceObject ISupportAppointments.AppointmentAppearance {
			get { return PaintAppearance.Appointment; }
		}
		MoreButton ISupportAppointments.CreateMoreButton() {
			return CreateMoreButton();
		}
		bool ISupportAppointments.ShowMoreButtons {
			get { return View.ShowMoreButtons; }
		}
		TimeZoneHelper ISupportAppointments.TimeZoneHelper {
			get { return TimeZoneHelper; }
		}
		ColoredSkinElementCache ISupportAppointments.ColoredSkinElementCache {
			get { return View.Control.ColoredSkinElementCache; }
		}
		Size ISupportAppointments.CalculateMoreButtonMinSize() {
			return CalculateMoreButtonMinSize();
		}
		IAppointmentStatus ISupportAppointments.GetStatus(object statusId) {
			return View.Control.GetStatus(statusId);
		}
		IAppointmentFormatStringService ISupportAppointments.GetFormatStringProvider() {
			return (IAppointmentFormatStringService)View.Control.GetService(typeof(IAppointmentFormatStringService));
		}
		Color ISupportAppointments.GetLabelColor(object labelId) {
			return View.Control.GetLabelColor(labelId);
		}
		TimeInterval ISupportAppointmentsBase.GetVisibleInterval() {
			return VisibleIntervals.Interval;
		}
		bool ISupportAppointments.DrawMoreButtonsOverAppointments { get { return View.DrawMoreButtonsOverAppointments; } }
		bool ISupportAppointments.OverriddenAppointmentForeColor { get { return OverriddenAppointmentForeColor; } }
		bool ISupportAppointments.ShouldShowContainerScrollBar() {
			return ShouldShowContainerScrollBar();
		}
		bool ISupportAppointmentsBase.UseAsyncMode { get { return UseAsyncMode; } }
		SchedulerCancellationTokenSource ISupportAppointmentsBase.CancellationToken {
			get { return tokenSource ?? new NullSchedulerCancellationTokenSource(); }
		}
		protected internal SchedulerCancellationTokenSource CancellationToken {
			get { return ((ISupportAppointmentsBase)this).CancellationToken; }
		}
		#endregion
	}
	#endregion
	#region SchedulerViewLayoutCalculatorBase (abstract class)
	public abstract class SchedulerViewLayoutCalculatorBase {
		#region Fields
		GraphicsCache cache;
		SchedulerViewInfoBase viewInfo;
		ViewInfoPainterBase painter;
		#endregion
		protected SchedulerViewLayoutCalculatorBase(GraphicsCache cache, SchedulerViewInfoBase viewInfo, ViewInfoPainterBase painter) {
			if (viewInfo == null)
				Exceptions.ThrowArgumentException("viewInfo", viewInfo);
			if (cache == null)
				Exceptions.ThrowArgumentException("cache", cache);
			if (painter == null)
				Exceptions.ThrowArgumentException("painter", painter);
			this.cache = cache;
			this.painter = painter;
			this.viewInfo = viewInfo;
		}
		protected internal SchedulerViewInfoBase ViewInfo { get { return viewInfo; } }
		protected internal SchedulerViewBase View { get { return ViewInfo.View; } }
		protected internal GraphicsCache Cache { get { return cache; } }
		protected internal ViewInfoPainterBase Painter { get { return painter; } }
		public abstract void CalcLayout(Rectangle bounds);
		protected internal virtual int CalculateResourceColorIndex(int visibleIndex) {
			return View.ActualFirstVisibleResourceIndex + visibleIndex;
		}
		protected internal virtual void CalculatePreliminaryLayout() {
		}
	}
	#endregion
	#region CalcHeaderResourceIndexDelegate
	public delegate int CalcHeaderResourceIndexDelegate(int headerIndex);
	#endregion
	#region SchedulerViewHeadersLayoutCalculator
	public abstract class SchedulerViewHeadersLayoutCalculator : SchedulerViewLayoutCalculatorBase {
		protected SchedulerViewHeadersLayoutCalculator(GraphicsCache cache, SchedulerViewInfoBase viewInfo, SchedulerHeaderPainter painter)
			: base(cache, viewInfo, painter) {
		}
		protected internal new SchedulerHeaderPainter Painter { get { return (SchedulerHeaderPainter)base.Painter; } }
		protected abstract SchedulerHeaderCollection TopLevelHeaders { get; }
		protected virtual SchedulerHeaderCollection SubLevelHeaders { get { return new SchedulerHeaderCollection(); } }
		protected internal virtual SchedulerHeaderCollection CreateTopLevelHeaders() {
			return new SchedulerHeaderCollection();
		}
		protected internal virtual SchedulerHeaderCollection CreateSubLevelHeaders() {
			return new SchedulerHeaderCollection();
		}
		protected internal virtual void AssignHeadersHeight(SchedulerHeaderCollection headers, int height) {
			int count = headers.Count;
			for (int i = 0; i < count; i++) {
				SchedulerHeader header = headers[i];
				Rectangle bounds = header.Bounds;
				bounds.Height = height;
				header.Bounds = bounds;
				bounds = header.AnchorBounds;
				bounds.Height = height;
				header.AnchorBounds = bounds;
			}
		}
		protected internal virtual void SetLeftAndRightBorders(SchedulerHeaderCollection headers) {
			int count = headers.Count;
			if (count > 0) {
				headers[0].HasLeftBorder = true;
				headers[count - 1].HasRightBorder = true;
			}
		}
		protected internal virtual void ApplyHeaderSeparators(SchedulerHeaderCollection headers) {
			int count = headers.Count;
			if (count <= 0)
				return;
			headers[0].HasSeparator = false;
			for (int i = 1; i < count; i++)
				headers[i].HasSeparator = true;
		}
		protected internal virtual SchedulerHeaderCollection CreateVerticalGroupSeparators(int count) {
			SchedulerHeaderCollection result = new SchedulerHeaderCollection();
			for (int i = 0; i < count; i++) {
				GroupSeparatorVertical header = new GroupSeparatorVertical(ViewInfo.PaintAppearance);
				result.Add(header);
			}
			return result;
		}
		protected internal virtual SchedulerHeaderCollection CreateHorizontalGroupSeparators(int count) {
			SchedulerHeaderCollection result = new SchedulerHeaderCollection();
			for (int i = 0; i < count; i++) {
				GroupSeparatorHorizontal header = new GroupSeparatorHorizontal(ViewInfo.PaintAppearance);
				result.Add(header);
			}
			return result;
		}
		protected internal virtual SchedulerHeaderPreliminaryLayoutResultCollection CalculateHeadersPreliminaryLayout(SchedulerHeaderCollection headers) {
			SchedulerHeaderPreliminaryLayoutResultCollection result = new SchedulerHeaderPreliminaryLayoutResultCollection();
			int count = headers.Count;
			for (int i = 0; i < count; i++) {
				SchedulerHeader header = headers[i];
				header.AssignAppearance(ViewInfo.PaintAppearance);
				SchedulerHeaderPreliminaryLayoutResult preliminaryResult = (SchedulerHeaderPreliminaryLayoutResult)header.CalculateHeaderPreliminaryLayout(Cache, Painter, ViewInfo.Bounds.Size);
				result.Add(preliminaryResult);
			}
			return result;
		}
		protected internal virtual void CalcFinalLayout(SchedulerHeaderCollection headers, SchedulerHeaderPreliminaryLayoutResultCollection preliminaryResults) {
			int count = headers.Count;
			for (int i = 0; i < count; i++) {
				headers[i].CalcLayout(Cache, preliminaryResults[i]);
			}
		}
		protected internal virtual int CalculateHeadersHeight(SchedulerHeaderPreliminaryLayoutResultCollection preliminaryResults) {
			int result = 0;
			int count = preliminaryResults.Count;
			for (int i = 0; i < count; i++)
				result = Math.Max(result, preliminaryResults[i].Size.Height);
			return result;
		}
		protected internal virtual int CalculateVerticalGroupSeparatorWidth() {
			int width = ViewInfo.View.GroupSeparatorWidth;
			if (width <= 0)
				width = Painter.GetVerticalGroupSeparatorWidth(Cache);
			return width;
		}
		protected internal virtual void CalculateGroupSeparatorsLayout(SchedulerHeaderCollection headers) {
			SchedulerHeaderPreliminaryLayoutResultCollection preliminaryResults = CalculateHeadersPreliminaryLayout(headers);
			CalcFinalLayout(headers, preliminaryResults);
		}
		protected internal virtual void CalculateInitialHeadersBounds(SchedulerHeaderCollection headers, Rectangle bounds) {
			CalculateInitialHeadersBounds(headers, bounds, new SchedulerHeaderCollection(), 0);
		}
		protected internal virtual void CalculateInitialHeadersBounds(SchedulerHeaderCollection headers, Rectangle bounds, SchedulerHeaderCollection groupSeparators, int groupSeparatorWidth) {
			CalculateInitialHeadersBounds(headers, bounds, groupSeparators, groupSeparatorWidth, new int[0]);
		}
		protected internal virtual void CalculateInitialHeadersBounds(SchedulerHeaderCollection headers, Rectangle bounds, SchedulerHeaderCollection groupSeparators, int groupSeparatorWidth, int[] headerAnchorsWidth) {
			int count = headers.Count;
			if (count <= 0)
				return;
			Rectangle[] headersBounds = CalculateInitialHeadersRectangles(bounds, headers, groupSeparatorWidth, headerAnchorsWidth);
#if DEBUG
			XtraSchedulerDebug.Assert(headers.Count == headersBounds.Length);
			if (groupSeparatorWidth > 0)
				XtraSchedulerDebug.Assert(headers.Count == groupSeparators.Count + 1);
#endif
			SetInitialHeadersBounds(headers, headersBounds);
			AdjustHeadersWidth(headers, groupSeparatorWidth);
			CalculateInitialGroupSeparatorsBounds(headers, groupSeparators, groupSeparatorWidth);
		}
		protected internal virtual Rectangle[] CalculateInitialHeadersRectangles(Rectangle bounds, SchedulerHeaderCollection headers, int groupSeparatorWidth, int[] headerAnchorsWidth) {
			if (headerAnchorsWidth.Length == 0)
				return CalculateInitialHeadersEqualRectangles(bounds, headers, groupSeparatorWidth);
			else {
				XtraSchedulerDebug.Assert(headers.Count == headerAnchorsWidth.Length);
				AdjustHeadersWidth(headerAnchorsWidth, true, false);
				return CalculateInitialHeadersRectanglesCore(bounds, groupSeparatorWidth, headerAnchorsWidth);
			}
		}
		protected internal virtual void AdjustHeadersWidth(int[] headerAnchorsWidth, bool groupSeparatorBeforeHeaders, bool groupSeparatorAfterHeaders) {
			int count = headerAnchorsWidth.Length;
			int horizontalOverlap = Painter.HorizontalOverlap;
			for (int i = 0; i < count - 1; i++)
				headerAnchorsWidth[i] += horizontalOverlap;
			if (groupSeparatorBeforeHeaders)
				headerAnchorsWidth[0] += horizontalOverlap;
			if (groupSeparatorAfterHeaders)
				headerAnchorsWidth[count - 1] += horizontalOverlap;
		}
		protected internal virtual Rectangle[] CalculateInitialHeadersEqualRectangles(Rectangle bounds, SchedulerHeaderCollection headers, int groupSeparatorWidth) {
			Rectangle extendedBounds = bounds;
			extendedBounds.Width += groupSeparatorWidth;
			return RectUtils.SplitHorizontally(extendedBounds, headers.Count);
		}
		protected internal virtual Rectangle[] CalculateInitialHeadersRectanglesCore(Rectangle bounds, int groupSeparatorWidth, int[] headersWidth) {
			int count = headersWidth.Length;
			Rectangle[] result = new Rectangle[count];
			int currentX = bounds.X;
			for (int i = 0; i < count; i++) {
				int width = headersWidth[i] + groupSeparatorWidth;
				result[i] = Rectangle.FromLTRB(currentX, bounds.Top, currentX + width, bounds.Bottom);
				currentX += width;
			}
			return result;
		}
		protected internal virtual void SetInitialHeadersBounds(SchedulerHeaderCollection headers, Rectangle[] headersBounds) {
			int count = headers.Count;
			int horizontalOverlap = Painter.HorizontalOverlap;
			for (int i = 0; i < count; i++) {
				Rectangle headerBounds = headersBounds[i];
				if (i > 0) {
					headerBounds.X -= horizontalOverlap;
					headerBounds.Width += horizontalOverlap;
				}
				headers[i].Bounds = headerBounds;
			}
		}
		protected internal virtual void AdjustHeadersWidth(SchedulerHeaderCollection headers, int groupSeparatorWidth) {
			int count = headers.Count;
			for (int i = 0; i < count; i++) {
				SchedulerHeader header = headers[i];
				Rectangle r = header.Bounds;
				r.Width -= groupSeparatorWidth;
				headers[i].Bounds = r;
			}
		}
		protected internal virtual void CalculateInitialGroupSeparatorsBounds(SchedulerHeaderCollection headers, SchedulerHeaderCollection groupSeparators, int groupSeparatorWidth) {
			int count = groupSeparators.Count;
			int horizontalOverlap = Painter.HorizontalOverlap;
			for (int i = 0; i < count; i++) {
				int x = headers[i].Bounds.Right - horizontalOverlap;
				int width = groupSeparatorWidth + horizontalOverlap;
				groupSeparators[i].Bounds = new Rectangle(x, headers[i].Bounds.Top, width, headers[i].Bounds.Height);
			}
		}
		protected internal virtual void CalculateInitialHeadersAnchorBounds(SchedulerHeaderCollection headers, bool groupSeparatorBeforeHeaders, bool groupSeparatorAfterHeaders) {
			int count = headers.Count;
			int horizontalOverlap = Painter.HorizontalOverlap;
			for (int i = 0; i < count; i++) {
				SchedulerHeader header = headers[i];
				Rectangle bounds = header.Bounds;
				int leftAnchor = bounds.Left;
				int rightAnchor = bounds.Right;
				if (i == 0 && groupSeparatorBeforeHeaders)
					leftAnchor += horizontalOverlap;
				if (groupSeparatorAfterHeaders)
					rightAnchor -= horizontalOverlap;
				else
					if (i < count - 1)
					rightAnchor -= horizontalOverlap;
				header.AnchorBounds = Rectangle.FromLTRB(leftAnchor, bounds.Top, rightAnchor, bounds.Bottom);
			}
		}
		protected internal virtual SchedulerHeaderCollection CreateHorizontalResourceHeaders(TimeInterval interval) {
			SchedulerHeaderCollection result = new SchedulerHeaderCollection();
			ResourceBaseCollection resources = ViewInfo.VisibleResources;
			int count = resources.Count;
			SchedulerResourceHeaderOptions options = View.Control.OptionsView.ResourceHeaders;
			for (int i = 0; i < count; i++) {
				Resource resource = resources[i];
				SchedulerHeader header = new HorizontalResourceHeader(ViewInfo.GetHorizontalResourceHeaderAppearance(), options);
				InitializeResourceHeader(header, resource, interval);
				result.Add(header);
			}
			return result;
		}
		protected internal virtual void InitializeResourceHeader(SchedulerHeader header, Resource resource, TimeInterval interval) {
			header.Interval = interval;
			header.Caption = resource.Caption;
			header.ToolTipText = resource.Caption;
			header.Resource = resource;
			header.HasTopBorder = true;
		}
		protected internal virtual void CalculateResourceHeadersLayout(SchedulerHeaderCollection headers) {
			SchedulerHeaderPreliminaryLayoutResultCollection preliminaryResults = CalculateHeadersPreliminaryLayout(headers);
			int headerHeight = View.ShowResourceHeadersInternal ? CalculateHeadersHeight(preliminaryResults) : 0;
			AssignHeadersHeight(headers, headerHeight);
			CalcFinalLayout(headers, preliminaryResults);
		}
		protected internal virtual void CacheResourceHeadersSkinElementInfos(SchedulerHeaderCollection headers) {
			CacheHeadersSkinElementInfos(headers, CalcVisibleResourceIndexByResourceHeaderIndex);
		}
		protected internal virtual int CalcVisibleResourceIndexByResourceHeaderIndex(int headerIndex) {
			return headerIndex;
		}
		protected internal virtual void CacheHeadersSkinElementInfos(SchedulerHeaderCollection headers, CalcHeaderResourceIndexDelegate calcHeaderResourceIndex) {
			if (!Painter.ShouldCacheSkinElementInfo)
				return;
			int count = headers.Count;
			for (int i = 0; i < count; i++)
				CacheHeaderSkinElementInfo(headers[i], i, calcHeaderResourceIndex);
		}
		protected internal virtual void CacheHeaderSkinElementInfo(SchedulerHeader header, int headerIndex, CalcHeaderResourceIndexDelegate calcHeaderResourceIndex) {
			if (header.Alternate)
				return;
			Color color = CalculateHeaderColor(header, headerIndex, calcHeaderResourceIndex);
			header.CachedSkinElementInfo = Painter.PrepareCachedSkinElementInfo(header, color);
		}
		protected internal virtual Color CalculateHeaderColor(SchedulerHeader header, int headerIndex, CalcHeaderResourceIndexDelegate calcHeaderResourceIndex) {
			int visibleResourceIndex = calcHeaderResourceIndex(headerIndex);
			int resourceColorIndex = CalculateResourceColorIndex(visibleResourceIndex + GetCoutOfInnerChildResourcesInCollapsedVisibleResources(visibleResourceIndex));
			SchedulerColorSchema schema = View.GetResourceColorSchema(header.Resource, resourceColorIndex);
			return schema.Cell;
		}
		int GetCoutOfInnerChildResourcesInCollapsedVisibleResources(int visibleResourceIndex) {
			if (this.View.VisibleResources.Count == 0)
				return 0;
			XtraSchedulerDebug.Assert(this.View.VisibleResources.Count > visibleResourceIndex);
			int result = 0;
			for (int i = 0; i < visibleResourceIndex; i++) {
				IInternalResource res = (IInternalResource)this.View.VisibleResources[i];
				if (!res.IsExpanded)
					result += res.AllChildResourcesCount;
			}
			return result;
		}
	}
	#endregion
	#region VerticalHeadersSupportedLayoutCalculator (abstract class)
	public abstract class VerticalHeadersSupportedLayoutCalculator : SchedulerViewHeadersLayoutCalculator {
		protected VerticalHeadersSupportedLayoutCalculator(GraphicsCache cache, SchedulerViewInfoBase viewInfo, SchedulerHeaderPainter painter)
			: base(cache, viewInfo, painter) {
		}
		protected virtual SchedulerHeaderCollection CreateGroupSeparators() {
			return new SchedulerHeaderCollection();
		}
		protected virtual SchedulerHeaderCollection CreateCorners() {
			return new SchedulerHeaderCollection();
		}
		protected internal override void CalculatePreliminaryLayout() {
			SchedulerHeaderCollection topHeaders = CreateTopLevelHeaders();
			TopLevelHeaders.AddRange(topHeaders);
			SchedulerHeaderCollection subHeaders = CreateSubLevelHeaders();
			SubLevelHeaders.AddRange(subHeaders);
			SchedulerHeaderCollection groupSeparators = CreateGroupSeparators();
			ViewInfo.PreliminaryLayoutResult.GroupSeparators.AddRange(groupSeparators);
			SchedulerHeaderCollection corners = CreateCorners();
			ViewInfo.PreliminaryLayoutResult.Corners.AddRange(corners);
		}
		protected internal virtual void Transform(SchedulerHeaderCollection headers, TransformMatrix transform) {
			int count = headers.Count;
			for (int i = 0; i < count; i++)
				Transform(headers[i], transform);
		}
		protected internal virtual void Transform(SchedulerHeader header, TransformMatrix transform) {
			header.Bounds = transform.Apply(header.Bounds);
			header.AnchorBounds = transform.Apply(header.AnchorBounds);
			header.ContentBounds = transform.Apply(header.ContentBounds);
			header.TextBounds = transform.Apply(header.TextBounds);
			header.ImageBounds = transform.Apply(header.ImageBounds);
			header.FullSeparatorBounds = transform.Apply(header.FullSeparatorBounds);
			header.SeparatorBounds = transform.Apply(header.SeparatorBounds);
			header.UnderlineBounds = transform.Apply(header.UnderlineBounds);
			header.LeftBorderBounds = transform.Apply(header.LeftBorderBounds);
			header.RightBorderBounds = transform.Apply(header.RightBorderBounds);
			header.TopBorderBounds = transform.Apply(header.TopBorderBounds);
			header.BottomBorderBounds = transform.Apply(header.BottomBorderBounds);
		}
		protected internal virtual SchedulerHeaderCollection CreateVerticalResourceHeaders() {
			SchedulerHeaderCollection result = new SchedulerHeaderCollection();
			ResourceBaseCollection resources = ViewInfo.VisibleResources;
			int count = resources.Count;
			SchedulerResourceHeaderOptions options = View.Control.OptionsView.ResourceHeaders;
			TimeInterval interval = new TimeInterval(ViewInfo.VisibleIntervals.Start, ViewInfo.VisibleIntervals.End);
			for (int i = 0; i < count; i++) {
				Resource resource = resources[i];
				VerticalResourceHeader header = new VerticalResourceHeader(ViewInfo.GetVerticalResourceHeaderAppearance(), options);
				InitializeResourceHeader(header, resource, interval);
				header.HasBottomBorder = true;
				header.HasRightBorder = false;
				result.Add(header);
			}
			if (count > 0)
				result[count - 1].HasRightBorder = true;
			return result;
		}
		protected void PerformVerticalResourceHeadersAndSeparatorsLayout(Rectangle bounds, int[] headersHeight) {
			SchedulerHeaderCollection groupSeparators = ViewInfo.PreliminaryLayoutResult.GroupSeparators;
			int topHeadersHeight = CalculateHeadersHeight(TopLevelHeaders);
			Rectangle resourceHeadersBounds = new Rectangle(0, 0, bounds.Height - topHeadersHeight + Painter.VerticalOverlap, bounds.Width);
			CalculateResourceHeadersBounds(resourceHeadersBounds, true, headersHeight);
			CalculateGroupSeparatorsLayout(groupSeparators);
			TransformMatrix transform = PrepareTransformMatrix(bounds.X, bounds.Y + topHeadersHeight - Painter.VerticalOverlap);
			Transform(SubLevelHeaders, transform);
			Transform(groupSeparators, transform);
			CacheResourceHeadersSkinElementInfos(SubLevelHeaders);
			ViewInfo.ResourceHeaders.AddRange(SubLevelHeaders);
			ViewInfo.GroupSeparators.AddRange(groupSeparators);
		}
		protected internal virtual TransformMatrix PrepareTransformMatrix(int x, int y) {
			TransformMatrix transform = new TransformMatrix();
			transform = transform.RotateCW90();
			transform = transform.Scale(-1, 1);
			transform = transform.Translate(x, y);
			return transform;
		}
		protected void PerformUpperCornerLayout(SchedulerHeaderCollection corners, Point location, Rectangle topResourceHeaderBounds) {
			int verticalOverlap = Painter.VerticalOverlap;
			foreach (SchedulerHeader corner in corners)
				corner.Bounds = Rectangle.FromLTRB(location.X, location.Y, topResourceHeaderBounds.Right, topResourceHeaderBounds.Top + verticalOverlap);
			CalculateGroupSeparatorsLayout(corners);
		}
		protected void CalculateResourceHeadersBounds(Rectangle bounds, bool groupSeparatorBeforeHeaders, int[] headerAnchorsWidth) {
			SchedulerHeaderCollection resourceHeaders = ViewInfo.PreliminaryLayoutResult.ResourceHeaders;
			CalculateInitialHeadersBounds(resourceHeaders, bounds, ViewInfo.PreliminaryLayoutResult.GroupSeparators, CalculateVerticalGroupSeparatorWidth(), headerAnchorsWidth);
			CalculateInitialHeadersAnchorBounds(resourceHeaders, groupSeparatorBeforeHeaders, false);
			CalculateResourceHeadersLayout(resourceHeaders);
		}
		protected void CalculateResourceHeadersBounds(Rectangle bounds, SchedulerHeaderCollection resourceHeaders, SchedulerHeaderCollection groupSeparators, bool groupSeparatorBeforeHeaders, int[] headerAnchorsWidth) {
			CalculateInitialHeadersBounds(resourceHeaders, bounds, groupSeparators, CalculateVerticalGroupSeparatorWidth(), headerAnchorsWidth);
			CalculateInitialHeadersAnchorBounds(resourceHeaders, groupSeparatorBeforeHeaders, false);
			CalculateResourceHeadersLayout(resourceHeaders);
		}
		protected internal virtual int CalculateHeadersHeight(SchedulerHeaderCollection headers) {
			SchedulerHeaderPreliminaryLayoutResultCollection preliminaryResults = CalculateHeadersPreliminaryLayout(headers);
			return CalculateHeadersHeight(preliminaryResults);
		}
		protected UpperLeftCorner CreateUppreLeftCorner() {
			return new UpperLeftCorner(ViewInfo.PaintAppearance);
		}
	}
	#endregion
	#region ViewInfoBasePreliminaryLayoutResult
	public class ViewInfoBasePreliminaryLayoutResult : IDisposable {
		SchedulerHeaderCollection resourceHeaders;
		SchedulerHeaderCollection groupSeparators;
		SchedulerViewCellContainerCollection cellContainers;
		SchedulerHeaderCollection corners;
		List<AppointmentIntermediateViewInfoCollection> preliminaryAppointmentsResult;
		bool calculated;
		public ViewInfoBasePreliminaryLayoutResult() {
			this.resourceHeaders = new SchedulerHeaderCollection();
			this.groupSeparators = new SchedulerHeaderCollection();
			this.cellContainers = new SchedulerViewCellContainerCollection();
			this.corners = new SchedulerHeaderCollection();
			this.preliminaryAppointmentsResult = new List<AppointmentIntermediateViewInfoCollection>();
			this.calculated = false;
		}
		public SchedulerHeaderCollection ResourceHeaders { get { return this.resourceHeaders; } }
		public SchedulerHeaderCollection GroupSeparators { get { return this.groupSeparators; } }
		public SchedulerViewCellContainerCollection CellContainers { get { return this.cellContainers; } }
		public SchedulerHeaderCollection Corners { get { return this.corners; } }
		public List<AppointmentIntermediateViewInfoCollection> PreliminaryAppointmentResult {
			get { return this.preliminaryAppointmentsResult; }
			set { this.preliminaryAppointmentsResult = value; }
		}
		public bool Calculated {
			get { return this.calculated; }
			set { this.calculated = value; }
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (this.resourceHeaders != null) {
					for (int i = 0; i < this.resourceHeaders.Count; i++)
						this.resourceHeaders[i].Dispose();
					this.resourceHeaders.Clear();
					this.resourceHeaders = null;
				}
				if (this.groupSeparators != null) {
					for (int i = 0; i < this.groupSeparators.Count; i++)
						this.groupSeparators[i].Dispose();
					this.groupSeparators.Clear();
					this.groupSeparators = null;
				}
			}
		}
	}
	#endregion
	#region SelectableIntervalViewInfo (abstract class)
	public abstract class SelectableIntervalViewInfo : ObjectInfoArgs, ISelectableIntervalViewInfo {
		internal sealed class EmptySelectableIntervalViewInfo : SelectableIntervalViewInfo {
			public override SchedulerHitTest HitTestType { get { return SchedulerHitTest.None; } }
		}
		static readonly SelectableIntervalViewInfo empty = new EmptySelectableIntervalViewInfo();
		public static SelectableIntervalViewInfo Empty { get { return empty; } }
		TimeInterval interval = TimeInterval.Empty;
		bool selected;
		bool hotTrackedInternal;
		string toolTipText = String.Empty;
		bool shouldShowToolTip = false;
		Resource resource = ResourceBase.Empty;
		protected SelectableIntervalViewInfo() {
		}
		#region Properties
		internal bool HotTrackedInternal { get { return hotTrackedInternal; } set { hotTrackedInternal = value; } }
		public virtual TimeInterval Interval { get { return interval; } set { interval = value; } }
		public virtual bool Selected { get { return selected; } set { selected = value; } }
		protected internal virtual bool AllowHotTrack { get { return false; } }
		public string ToolTipText { get { return toolTipText; } set { toolTipText = value; } }
		public abstract SchedulerHitTest HitTestType { get; }
		SchedulerHitTest ISelectableIntervalViewInfo.HitTestType { get { return this.HitTestType; } }
		public virtual Resource Resource {
			get { return resource; }
			set {
				if (value == null)
					Exceptions.ThrowArgumentException("Resource", resource);
				resource = value;
			}
		}
		public bool ShouldShowToolTip { get { return shouldShowToolTip; } set { shouldShowToolTip = value; } }
		#endregion
		public virtual SchedulerHitInfo CalculateHitInfo(Point pt, SchedulerHitInfo nextHitInfo) {
			if (Bounds.Contains(pt))
				return new SchedulerHitInfo(this, HitTestType, nextHitInfo);
			else
				return nextHitInfo;
		}
		[Obsolete("You should use the 'CalculateHitInfo' instead", false), EditorBrowsable(EditorBrowsableState.Never)]
		public void CalcHitInfo(SchedulerHitInfo info, Point pt) {
			SchedulerHitInfo result = CalculateHitInfo(pt, SchedulerHitInfo.None);
			info.SetProperties(result.ViewInfo, result.HitTest, result.NextHitInfo);
		}
	}
	#endregion
	#region BorderObjectViewInfo (abstract class)
	public abstract class BorderObjectViewInfo : SelectableIntervalViewInfo {
		#region Fields
		Rectangle leftBorderBounds;
		Rectangle rightBorderBounds;
		Rectangle topBorderBounds;
		Rectangle bottomBorderBounds;
		bool hasTopBorder;
		bool hasBottomBorder;
		bool hasLeftBorder;
		bool hasRightBorder;
		#endregion
		#region Properties
		public Rectangle LeftBorderBounds { get { return leftBorderBounds; } set { leftBorderBounds = value; } }
		public Rectangle RightBorderBounds { get { return rightBorderBounds; } set { rightBorderBounds = value; } }
		public Rectangle TopBorderBounds { get { return topBorderBounds; } set { topBorderBounds = value; } }
		public Rectangle BottomBorderBounds { get { return bottomBorderBounds; } set { bottomBorderBounds = value; } }
		public bool HasTopBorder { get { return hasTopBorder; } set { hasTopBorder = value; } }
		public bool HasBottomBorder { get { return hasBottomBorder; } set { hasBottomBorder = value; } }
		public bool HasLeftBorder { get { return hasLeftBorder; } set { hasLeftBorder = value; } }
		public bool HasRightBorder { get { return hasRightBorder; } set { hasRightBorder = value; } }
		#endregion
		protected internal virtual void CalcBorderBounds(BorderObjectPainter painter) {
			this.leftBorderBounds = CalcLeftBorderBounds(painter);
			this.rightBorderBounds = CalcRightBorderBounds(painter);
			this.topBorderBounds = CalcTopBorderBounds(painter);
			this.bottomBorderBounds = CalcBottomBorderBounds(painter);
		}
		protected internal virtual Rectangle CalcLeftBorderBounds(BorderObjectPainter painter) {
			return HasLeftBorder ? RectUtils.GetLeftSideRect(this.Bounds, painter.GetLeftBorderWidth(this)) :
				RectUtils.GetLeftSideRect(this.Bounds, painter.GetNoLeftBorderWidth(this));
		}
		protected internal virtual Rectangle CalcRightBorderBounds(BorderObjectPainter painter) {
			return HasRightBorder ? RectUtils.GetRightSideRect(this.Bounds, painter.GetRightBorderWidth(this)) :
				RectUtils.GetRightSideRect(this.Bounds, painter.GetNoRightBorderWidth(this));
		}
		protected internal virtual Rectangle CalcTopBorderBounds(BorderObjectPainter painter) {
			return HasTopBorder ? RectUtils.GetTopSideRect(this.Bounds, painter.GetTopBorderWidth(this)) :
				RectUtils.GetTopSideRect(this.Bounds, painter.GetNoTopBorderWidth(this));
		}
		protected internal virtual Rectangle CalcBottomBorderBounds(BorderObjectPainter painter) {
			return HasBottomBorder ? RectUtils.GetBottomSideRect(this.Bounds, painter.GetBottomBorderWidth(this)) :
				RectUtils.GetBottomSideRect(this.Bounds, painter.GetNoBottomBorderWidth(this));
		}
	}
	#endregion
	#region SchedulerViewCellsLayoutCalculator (abstract class)
	public abstract class SchedulerViewCellsLayoutCalculator : SchedulerViewLayoutCalculatorBase {
		protected SchedulerViewCellsLayoutCalculator(GraphicsCache cache, SchedulerViewInfoBase viewInfo, ViewInfoPainterBase painter)
			: base(cache, viewInfo, painter) {
		}
		protected internal virtual SchedulerColorSchema GetColorSchema(Resource resource, int visibleResourceIndex) {
			return View.GetResourceColorSchema(resource, CalculateResourceColorIndex(visibleResourceIndex));
		}
		protected int GetVerticalOvertlap() {
			return ((SchedulerHeaderPainter)Painter).VerticalOverlap;
		}
		protected internal Rectangle AdjustWeekVerticallyUp(int left, int top, int right, int bottom) {
			int overlap = GetVerticalOvertlap();
			return Rectangle.FromLTRB(left, top - overlap, right, bottom);
		}
		protected internal Rectangle AdjustWeekVerticallyDown(int left, int top, int right, int bottom) {
			int overlap = GetVerticalOvertlap();
			return Rectangle.FromLTRB(left, top + overlap, right, bottom);
		}
		protected internal virtual void ResetContainerBorders(SchedulerViewCellContainer container) {
			container.HasLeftBorder = false;
			container.HasTopBorder = false;
			container.HasRightBorder = false;
			container.HasBottomBorder = false;
		}
		protected SchedulerViewCellContainer GetCellContainerByResourceAndInterval(Resource resource, TimeInterval interval) {
			return ViewInfo.PreliminaryLayoutResult.CellContainers.Find(cc => (cc.Resource == resource || resource == ResourceBase.Empty) && cc.Interval.Equals(interval));
		}
	}
	#endregion
	#region SchedulerHitInfo
	public class SchedulerHitInfo : ISchedulerHitInfo {
		static readonly SchedulerHitInfo none = new SchedulerHitInfo();
		public static SchedulerHitInfo None { get { return none; } }
		SelectableIntervalViewInfo viewInfo;
		SchedulerHitTest hitTest;
		SchedulerHitInfo nextHitInfo;
		SchedulerHitInfo() {
			this.viewInfo = SelectableIntervalViewInfo.Empty;
			this.hitTest = SchedulerHitTest.None;
			this.nextHitInfo = this;
		}
		public SchedulerHitInfo(SelectableIntervalViewInfo viewInfo, SchedulerHitTest hitTest)
			: this(viewInfo, hitTest, SchedulerHitInfo.None) {
		}
		public SchedulerHitInfo(SelectableIntervalViewInfo viewInfo, SchedulerHitTest hitTest, SchedulerHitInfo nextHitInfo) {
			if (viewInfo == null)
				Exceptions.ThrowArgumentException("viewInfo", viewInfo);
			if (nextHitInfo == null)
				Exceptions.ThrowArgumentException("nextHitInfo", nextHitInfo);
			SetProperties(viewInfo, hitTest, nextHitInfo);
		}
		Point ISchedulerHitInfo.HitPoint { get { return Point.Empty; } }
		public SchedulerHitTest HitTest { get { return hitTest; } }
		public SelectableIntervalViewInfo ViewInfo { get { return viewInfo; } }
		ISelectableIntervalViewInfo ISchedulerHitInfo.ViewInfo { get { return this.ViewInfo; } }
		public SchedulerHitInfo NextHitInfo { get { return nextHitInfo; } }
		ISchedulerHitInfo ISchedulerHitInfo.NextHitInfo { get { return this.NextHitInfo; } }
		protected internal virtual void SetProperties(SelectableIntervalViewInfo viewInfo, SchedulerHitTest hitTest, SchedulerHitInfo nextHitInfo) {
			this.viewInfo = viewInfo;
			this.hitTest = hitTest;
			this.nextHitInfo = nextHitInfo;
		}
		public virtual SchedulerHitInfo FindHitInfo(SchedulerHitTest types) {
			return FindHitInfo(types, SchedulerHitTest.None);
		}
		ISchedulerHitInfo ISchedulerHitInfo.FindHitInfo(SchedulerHitTest types) {
			return this.FindHitInfo(types);
		}
		public SchedulerHitInfo FindHitInfo(SchedulerHitTest types, SchedulerHitTest stopTypes) {
			if ((types & stopTypes) != 0)
				Exceptions.ThrowArgumentException("stopTypes", stopTypes);
			SchedulerHitInfo current = this;
			while (current != SchedulerHitInfo.None) {
				if ((current.HitTest & types) != 0)
					return current;
				if ((current.HitTest & stopTypes) != 0)
					return SchedulerHitInfo.None;
				current = current.NextHitInfo;
			}
			return SchedulerHitInfo.None;
		}
		ISchedulerHitInfo ISchedulerHitInfo.FindHitInfo(SchedulerHitTest types, SchedulerHitTest stopTypes) {
			return this.FindHitInfo(types, stopTypes);
		}
		public virtual SchedulerHitInfo FindFirstLayoutHitInfo() {
			return FindHitInfo(SchedulerHitTest.Cell | SchedulerHitTest.DayHeader | SchedulerHitTest.AllDayArea | SchedulerHitTest.DayOfWeekHeader | SchedulerHitTest.TimeScaleHeader | SchedulerHitTest.ResourceHeader | SchedulerHitTest.SelectionBarCell);
		}
		ISchedulerHitInfo ISchedulerHitInfo.FindFirstLayoutHitInfo() {
			return this.FindFirstLayoutHitInfo();
		}
		public virtual bool Contains(SchedulerHitTest types) {
			SchedulerHitInfo current = this;
			while (current != SchedulerHitInfo.None) {
				if ((current.HitTest & types) != 0)
					return true;
				current = current.NextHitInfo;
			}
			return false;
		}
	}
	#endregion
	#region ISupportAppointments
	public interface ISupportAppointments : ISupportAppointmentsBase {
		object AppointmentImages { get; }   
		Size CalculateMoreButtonMinSize(); 
		AppearanceObject AppointmentAppearance { get; } 
		MoreButton CreateMoreButton();
		bool ShowMoreButtons { get; }
		TimeZoneHelper TimeZoneHelper { get; } 
		ColoredSkinElementCache ColoredSkinElementCache { get; } 
		IAppointmentFormatStringService GetFormatStringProvider(); 
		IAppointmentStatus GetStatus(object statusId);		
		Color GetLabelColor(object labelId); 
		bool DrawMoreButtonsOverAppointments { get; }
		bool ShouldShowContainerScrollBar();
		bool OverriddenAppointmentForeColor { get; }
	}
	#endregion
}
