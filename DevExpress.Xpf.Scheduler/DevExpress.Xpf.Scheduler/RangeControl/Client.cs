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
using System.Globalization;
using System.Windows.Controls;
using DevExpress.Xpf.Editors.RangeControl;
using System.Windows;
using DevExpress.Xpf.Editors.RangeControl.Internal;
using DevExpress.XtraScheduler;
using System.Windows.Media;
using DevExpress.XtraScheduler.Native;
using DevExpress.Xpf.Scheduler.Native;
using DevExpress.Xpf.Core;
using System.Collections.Generic;
using DevExpress.Xpf.Scheduler.Drawing;
using System.Windows.Threading;
using DevExpress.Xpf.Core.Native;
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.Xpf.Scheduler {
	public partial class SchedulerRangeControlClient : IRangeControlClient {
		const int DeferredSelectedRangeUpdateInterval = 500;
		Rect clientBounds = Rect.Empty;
		Locker clientLock = new Locker();
		Locker schedulerInitiateOperationLock = new Locker();
		DispatcherTimer timerDeferredSelectedRangeUpdate;
		#region Properties
#if DEBUGTEST
		bool allowDeferredChanges = true;
		internal bool AllowDeferredChanges { get { return allowDeferredChanges; } set { allowDeferredChanges = value; } }
#endif
		Locker ClientLock { get { return clientLock; } }
		int MinStepsNumber { get { return 3; } }
		IRangeControlClient Instance { get { return this; } }
		bool IRangeControlClient.ConvergeThumbsOnZoomingOut { get { return true; } }
		Rect IRangeControlClient.ClientBounds { get { return clientBounds; } }
		bool IRangeControlClient.GrayOutNonSelectedRange { get { return true; } }
		bool IRangeControlClient.SnapSelectionToGrid { get { return true; } }
		bool IRangeControlClient.AllowThumbs { get { return true; } }
		object IRangeControlClient.Start { get { return Controller.Minimum; } }
		object IRangeControlClient.End { get { return Controller.Maximum; } }
		object IRangeControlClient.SelectionStart { get { return (Controller.RulerCount > 0) ? Controller.BaseScale.Floor(DataProvider.SelectedRangeStart) : DataProvider.SelectedRangeStart; } }
		object IRangeControlClient.SelectionEnd { get { return RoundToRight(DataProvider.SelectedRangeEnd); } }
		#region VisibleStart
		object IRangeControlClient.VisibleStart { get { return VisibleStart; } }
		internal DateTime VisibleStart { get; set; }
		#endregion
		#region VisibleEnd
		object IRangeControlClient.VisibleEnd { get { return VisibleEnd; } }
		internal DateTime VisibleEnd { get; set; }
		#endregion
		protected DateTime ActualVisibleStart { get { return VisibleStart == DateTime.MinValue ? Controller.Minimum : VisibleStart; } }
		protected DateTime ActualVisibleEnd { get { return VisibleEnd == DateTime.MinValue ? Controller.Maximum : VisibleEnd; } }
		#endregion
		#region Events
		#region LayoutChanged
		event EventHandler<LayoutChangedEventArgs> LayoutChanged;
		event EventHandler<LayoutChangedEventArgs> IRangeControlClient.LayoutChanged {
			add { LayoutChanged += value; }
			remove { LayoutChanged -= value; }
		}
		protected void RaiseLayoutChanged(LayoutChangedType layoutType, object start, object end) {
			if (LayoutChanged == null)
				return;
			SchedulerLogger.Trace(XpfLoggerTraceLevel.RangeControl, "->RangeControlClient.RaiseLayoutChanged: start={0}, end={1}", start, end);
			LayoutChanged(this, new LayoutChangedEventArgs(layoutType, start, end));
		}
		protected void RaiseLayoutChanged() {
			if (LayoutChanged == null)
				return;
			LayoutChanged(this, new LayoutChangedEventArgs(LayoutChangedType.Layout, null, null));
		}
		#endregion
		#endregion
		DateTime RoundToRight(DateTime value) {
			if (Controller.RulerCount <= 0)
				return value;
			DateTime leftValue = Controller.BaseScale.Floor(DataProvider.SelectedRangeEnd);
			if (leftValue == value)
				return value;
			return Controller.BaseScale.GetNextDate(leftValue);
		}
		string IRangeControlClient.FormatText(object value) {
			return string.Format(CultureInfo.CurrentCulture, "{0:%d} {0:MMMM} {0:yyyy}", value);
		}
		double IRangeControlClient.GetComparableValue(object realValue) {
			return Controller.GetComparableValue(Convert.ToDateTime(realValue));
		}
		object IRangeControlClient.GetRealValue(double comparable) {
			return Controller.GetRealValue(comparable);
		}
		object IRangeControlClient.GetSnappedValue(object value, bool isLeft) {
			DateTime dateTime = (DateTime)value;
			if (SchedulerControl == null)
				return dateTime;
			TimeScale baseScale = GetBaseTimeScale();
			if (baseScale == null)
				return dateTime;
			DateTime leftDate = baseScale.Floor(dateTime);
			return isLeft ? leftDate : baseScale.GetNextDate(leftDate);
		}
		RangeControlClientHitTestResult IRangeControlClient.HitTest(System.Windows.Point point) {
			if (SchedulerControl == null)
				return RangeControlClientHitTestResult.Nothing;
			double value = RenderToComparable(point.X, ActualWidth);
			DateTime hitDate = Controller.GetRealValue(value);
			TimeScale hitScale = GetScaleViaPoint(point);
			if (hitScale == null)
				return new RangeControlClientHitTestResult(RangeControlClientRegionType.Nothing, DateTime.MinValue, DateTime.MinValue.AddDays(1));
			DateTime start = hitScale.Floor(hitDate);
			DateTime end = hitScale.GetNextDate(start);
			return new RangeControlClientHitTestResult(RangeControlClientRegionType.ItemInterval, start, end);
		}
		protected double RenderToComparable(double renderValue, double totalWidth) {
			double normalValue = renderValue / totalWidth;
			return Controller.GetComparableFromNormalValue(normalValue);
		}
		void IRangeControlClient.Invalidate(Size viewPort) {
			SaveViewPort(viewPort);
			InvalidateArrange();
		}
		bool IRangeControlClient.SetRange(object start, object end, Size viewportSize) {
			SaveViewPort(viewportSize);
			if (ClientLock)
				return true;
			bool canChange = SetRangeInternal(start, end, viewportSize);
			return canChange;
		}
		bool IRangeControlClient.SetSelectionRange(object selectedMinimum, object selectedMaximum, Size viewportSize, bool isSnapped) {
			SaveViewPort(viewportSize);
			if (ClientLock)
				return true;
			if (SchedulerControl == null)
				return false;
			using (Locker locker = ClientLock.Lock()) {
				if (!this.schedulerInitiateOperationLock)
					BeginDeferredChanges();
				DateTime selectionStart = (DateTime)selectedMinimum;
				DateTime selectionEnd = (DateTime)selectedMaximum;
				if (selectionStart > selectionEnd)
					return false;
				if (Controller != null)
					Controller.OnRangeChanged(selectionStart, selectionEnd);
				OnSetSelectionRange();
			}
			return true;
		}
		void BeginDeferredChanges() {
#if DEBUGTEST
			if (!AllowDeferredChanges)
				return;
#endif
			if (!this.isClientInitialized)
				return;
			SchedulerControlClientDataProvider clientDataProvider = DataProvider as SchedulerControlClientDataProvider;
			if (clientDataProvider == null)
				return;
			if (this.timerDeferredSelectedRangeUpdate == null) {
				this.timerDeferredSelectedRangeUpdate = new DispatcherTimer();
				this.timerDeferredSelectedRangeUpdate.Tick += OnTimerDeferredSelectedRangeUpdateTick;
				this.timerDeferredSelectedRangeUpdate.Interval = TimeSpan.FromMilliseconds(DeferredSelectedRangeUpdateInterval);
			}
			this.timerDeferredSelectedRangeUpdate.Stop();
			this.timerDeferredSelectedRangeUpdate.Start();
			clientDataProvider.BeginDeferredChanges();
		}
		void OnTimerDeferredSelectedRangeUpdateTick(object sender, EventArgs e) {
			SchedulerControlClientDataProvider clientDataProvider = DataProvider as SchedulerControlClientDataProvider;
			if (clientDataProvider == null)
				return;
			using (Locker locker = ClientLock.Lock()) {
				clientDataProvider.EndDeferredChanges();
				this.timerDeferredSelectedRangeUpdate.Stop();
			}
		}
		bool IRangeControlClient.SetVisibleRange(object visibleStart, object visibleEnd, Size viewportSize) {
			if (SchedulerControl == null)
				return false;
			SaveViewPort(viewportSize);
			return SetVisibleRangeCore(visibleStart, visibleEnd);
		}
		bool SetVisibleRangeCore(object visibleStart, object visibleEnd) {
			bool isCorrected = false;
			isCorrected = AdjustScaleFactor();
			if (!isCorrected) {
				DateTime newVisibleStart = Convert.ToDateTime(visibleStart);
				DateTime newVisibleEnd = Convert.ToDateTime(visibleEnd);
				isCorrected = ConstrainVisibleRange(newVisibleStart, newVisibleEnd);
			}
			return isCorrected;
		}
		bool ConstrainVisibleRange(DateTime newVisibleStart, DateTime newVisibleEnd) {
			bool isCorrected = false;
			if (newVisibleStart < Controller.Minimum) {
				newVisibleStart = Controller.Minimum;
				isCorrected = true;
			}
			if (Controller.Maximum < newVisibleEnd) {
				newVisibleEnd = Controller.Maximum;
				isCorrected = true;
			}
			double scaleFactor = Controller.CalculateScaleFactorViaIntervals(newVisibleStart, newVisibleEnd);
			double maxScaleFactor = 1 / Controller.CalculateMinScaleFactor();
			double minScaleFactor = 1 / Controller.CalculateMaxScaleFactor();
			if (minScaleFactor <= scaleFactor && scaleFactor <= maxScaleFactor) {
				VisibleStart = newVisibleStart;
				VisibleEnd = newVisibleEnd;
				return isCorrected;
			}
			double startDelta = Controller.GetComparableValue(VisibleStart) - Controller.GetComparableValue(newVisibleStart);
			double endDelta = Controller.GetComparableValue(VisibleEnd) - Controller.GetComparableValue(newVisibleEnd);
			bool isStartChanged = !startDelta.AreClose(0);
			bool isEndChanged = !endDelta.AreClose(0);
			double actualScaleFactor = (scaleFactor < minScaleFactor) ? minScaleFactor : maxScaleFactor;
			double visibleIntervalComparableDuration = (Controller.GetComparableValue(Controller.Maximum) - Controller.GetComparableValue(Controller.Minimum)) * actualScaleFactor;
			if (!isStartChanged) {
				double visibleEndComparable = Controller.GetComparableValue(VisibleStart) + visibleIntervalComparableDuration;
				VisibleEnd = Controller.GetRealValue(visibleEndComparable);
				isCorrected = true;
			} else if (!isEndChanged) {
				double visibleStartComparable = Controller.GetComparableValue(VisibleEnd) - visibleIntervalComparableDuration;
				VisibleStart = Controller.GetRealValue(visibleStartComparable);
				isCorrected = true;
			} else {
				VisibleStart = newVisibleStart;
				double visibleEndComparable = Controller.GetComparableValue(VisibleStart) + visibleIntervalComparableDuration;
				VisibleEnd = Controller.GetRealValue(visibleEndComparable);
				isCorrected = true;
			}
			EnsureVisibleIntervalInRangeInterval();
			return isCorrected;
		}
		void EnsureVisibleIntervalInRangeInterval() {
			if (VisibleEnd > Controller.Maximum) {
				TimeSpan delta = VisibleEnd - Controller.Maximum;
				VisibleStart -= delta;
				VisibleEnd -= delta;
			} else if (VisibleStart < Controller.Minimum) {
				TimeSpan delta = Controller.Minimum - VisibleStart;
				VisibleStart += delta;
				VisibleEnd += delta;
			}
			if (VisibleStart < Controller.Minimum)
				VisibleStart = Controller.Minimum;
			if (VisibleEnd > Controller.Maximum)
				VisibleEnd = Controller.Maximum;
		}
		bool SetRangeInternal(object start, object end, Size viewportSize) {
			DateTime actualStart = (DateTime)start;
			DateTime actualEnd = (DateTime)end;
			Controller.UpdateTotalRange(actualStart, actualEnd);
			UpdateVisibleRangeOnSetRange(Convert.ToDateTime(actualStart), Convert.ToDateTime(actualEnd), viewportSize);
			InvalidateRender();
			return true;
		}
		void UpdateVisibleRangeOnSetRange(DateTime start, DateTime end, Size viewportSize) {
			double comparableStart = Controller.GetComparableValue(start);
			double comparableEnd = Controller.GetComparableValue(end);
			double comparableVisibleStart = Controller.GetComparableValue(ActualVisibleStart);
			double comparableVisibleEnd = Controller.GetComparableValue(ActualVisibleEnd);
			if (!comparableVisibleStart.InRange(comparableStart, comparableEnd) || !comparableVisibleEnd.InRange(comparableStart, comparableEnd)) {
				comparableVisibleStart = comparableVisibleStart.ToRange(comparableStart, comparableEnd);
				comparableVisibleEnd = comparableVisibleEnd.ToRange(comparableVisibleStart, comparableEnd);
				Instance.SetVisibleRange(Controller.GetRealValue(comparableVisibleStart), Controller.GetRealValue(comparableVisibleEnd), viewportSize);
			}
		}
		protected void OnSetSelectionRange() {
			InvalidateRender();
		}
	}
	public partial class SchedulerRangeControlClient : IRangeControlClientSyncSupport {
		IRangeControlClientSyncSupport ClientSyncSupportInstance { get { return this; } }
		bool IRangeControlClientSyncSupport.CanAdjustRangeControl {
			get { return true; }
		}
		#region Options
		IScaleBasedRangeControlClientOptions IRangeControlClientSyncSupport.Options {
			get {
				return options;
			}
		}
		#endregion
		TimeInterval IRangeControlClientSyncSupport.TotalRange { get { return new TimeInterval(Controller.Minimum, Controller.Maximum); } }
		double IRangeControlClientSyncSupport.ViewportWidth { get { return ViewPort.Width; } }
		void SaveViewPort(Size viewport) {
			ViewPort = viewport;
		}
		void IRangeControlClientSyncSupport.AdjustRangeControlRange(RangeControlAdjustEventArgs e) {
			if (ClientLock)
				return;
			using (Locker locker = ClientLock.Lock()) {
			AdjustRangeControlScalesCore(e);
		}
		}
		void IRangeControlClientSyncSupport.AdjustRangeControlScales(RangeControlAdjustEventArgs e) {
			using(Locker locker = this.schedulerInitiateOperationLock.Lock())
			AdjustRangeControlScalesCore(e);
		}
		protected virtual void AdjustRangeControlScalesCore(RangeControlAdjustEventArgs e) {
			NotifyRangeControlAutoAdjusting(e);
			AdjustTotalRange(e.RangeMinimum, e.RangeMaximum);
			AdjustVisibleScales(e.Scales);
			ResetScaleFactorAdjusted();
			RaiseLayoutChanged(LayoutChangedType.Data, Controller.Minimum, Controller.Maximum);
		}
		void NotifyRangeControlAutoAdjusting(RangeControlAdjustEventArgs e) {
			if (SchedulerControl == null)
				return;
			SchedulerControl.NotifyRangeControlAutoAdjusting(e);
			XpfSchedulerControlRangeHelper.UpdateScalesFirstDayOfWeek(SchedulerControl.InnerControl, e.Scales);
		}
		bool scaleFactorAdjusted;
		void ResetScaleFactorAdjusted() {
			this.scaleFactorAdjusted = false;
		}
		bool AdjustScaleFactor() {
			if (this.scaleFactorAdjusted || Controller.RulerCount == 0)
				return false;
			double desiredScaleFactor = Controller.CalculateDesiredScaleFactor();
			if (desiredScaleFactor == 0)
				return false;
			AdjustVisibleRange(desiredScaleFactor);
			this.scaleFactorAdjusted = true;
			return true;
		}
		void AdjustVisibleRange(double factor) {
			DateTime selectionRangeCenter = CalculateSelectedRangeCenter();
			double selectionRangeCenterComparable = Controller.GetComparableValue(selectionRangeCenter);
			double rangeComparable = Controller.GetComparableValue(Controller.Maximum) - Controller.GetComparableValue(Controller.Minimum);
			double deltaComparable = rangeComparable * factor / 2;
			VisibleStart = Controller.GetRealValue(selectionRangeCenterComparable - deltaComparable);
			VisibleEnd = Controller.GetRealValue(selectionRangeCenterComparable + deltaComparable);
		}
		DateTime CalculateSelectedRangeCenter() {
			DateTime start = DataProvider.SelectedRangeStart;
			TimeSpan duration = DataProvider.SelectedRangeEnd - start;
			return start + TimeSpan.FromTicks(duration.Ticks / 2);
		}
		protected virtual void AdjustTotalRange(DateTime minimum, DateTime maximum) {
			Controller.UpdateTotalRange(minimum, maximum);
			AdjustScaleFactor();
		}
		protected virtual void AdjustVisibleScales(TimeScaleCollection visibleScales) {
			Controller.UpdateVisibleRulers(visibleScales);
		}
		void IRangeControlClientSyncSupport.ReInitialize() {
			InitControllerProperties();
			ResetScaleFactorAdjusted();
		}
		void IRangeControlClientSyncSupport.RefreshRangeControl(bool forceRequestData) {
			if (!this.isClientInitialized)
				return;
			RefreshRangeControlCore(forceRequestData);
		}
		void IRangeControlClientSyncSupport.SyncRangeControlRange() {
			if (!this.isClientInitialized)
				return;
			RefreshRangeControlCore(true);
		}
		protected virtual void InitControllerProperties() {
			UpdateControllerRulers();
			UpdateControllerTotalRange();
			UpdateControllerMaxSelectedIntervalCount();
		}
		protected virtual void UpdateControllerMaxSelectedIntervalCount() {
			if (ClientSyncSupportInstance.Options == null)
				return;
			Controller.UpdateMaxSelectedIntervalCount(ClientSyncSupportInstance.Options.MaxSelectedIntervalCount);
		}
		protected virtual void UpdateControllerTotalRange() {
			if (ClientSyncSupportInstance.Options == null)
				return;
			Controller.UpdateTotalRange(ClientSyncSupportInstance.Options.RangeMinimum, ClientSyncSupportInstance.Options.RangeMaximum);
			Controller.ValidateTotalRange(DataProvider.SelectedRangeStart, DataProvider.SelectedRangeEnd);
		}
		protected virtual void UpdateControllerRulers() {
			if (SchedulerControl != null) {
				SchedulerControlRangeHelper.UpdateScalesFirstDayOfWeek(SchedulerControl.InnerControl, ClientSyncSupportInstance.Options.Scales);
				if (SchedulerControl.OptionsRangeControl != null && SchedulerControl.OptionsRangeControl.AutoAdjustMode) {
					SchedulerControlClientDataProvider provider = DataProvider as SchedulerControlClientDataProvider;
					provider.AdjustScales();
					return;
				}
			}
			if (ClientSyncSupportInstance.Options != null)
				Controller.UpdateVisibleRulers(ClientSyncSupportInstance.Options.Scales);
		}
	}
	public partial class SchedulerRangeControlClient : Panel {
		#region Default values
		internal static DateTime DefaultRangeMinimum {
			get {
				DateTime now = DateTime.Now;
				DateTime startOfMonth = new DateTime(now.Year, now.Month, 1);
				return startOfMonth.AddMonths(-1);
			}
		}
		internal static DateTime DefaultRangeMaximum {
			get {
				DateTime now = DateTime.Now;
				DateTime startOfMonth = new DateTime(now.Year, now.Month, 1);
				return startOfMonth.AddMonths(2);
			}
		}
		#endregion
		IRangeControlClientDataProvider dataProvider;
		IScaleBasedRangeControlClientOptions options;
		ScaleBasedRangeControlController controller;
		Size viewPort;
		ItemGenerator<RulerHeaderControl> headerItemGenerator;
		ItemGenerator<RulerCellControl> contentItemGenerator;
		ItemGenerator<ThumbnailControl> thumbnailItemGenerator;
		ItemGenerator<ThumbnailGroupControl> thumbnailGroupItemGenerator;
		bool isClientInitialized;
		List<Rect> HeaderLevelsBounds { get; set; }
		public SchedulerRangeControlClient() {
			this.isClientInitialized = false;
			this.controller = CreateController();
			Loaded += OnLoaded;
			SizeChanged += OnClientSizeChanged;
			this.headerItemGenerator = new ItemGenerator<RulerHeaderControl>(this);
			this.contentItemGenerator = new ItemGenerator<RulerCellControl>(this);
			this.thumbnailItemGenerator = new ItemGenerator<ThumbnailControl>(this);
			this.thumbnailGroupItemGenerator = new ItemGenerator<ThumbnailGroupControl>(this);
		}
		#region Properties
		protected Rect RenderBounds { get; private set; }
		internal ScaleBasedRangeControlController Controller { get { return controller; } }
		#region SchedulerControl
		public SchedulerControl SchedulerControl {
			get { return (SchedulerControl)GetValue(SchedulerControlProperty); }
			set { SetValue(SchedulerControlProperty, value); }
		}
		public static readonly DependencyProperty SchedulerControlProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerRangeControlClient, SchedulerControl>("SchedulerControl", null, (d, e) => d.OnSchedulerControlChanged(e.OldValue, e.NewValue), null);
		void OnSchedulerControlChanged(SchedulerControl oldValue, SchedulerControl newValue) {
			SchedulerLogger.Trace(XpfLoggerTraceLevel.RangeControl, "->RangeControlClient.setSchedulerControl");
			DataProvider = new SchedulerControlClientDataProvider(newValue);
		}
		#endregion
		#region DataProvider
		protected IRangeControlClientDataProvider DataProvider {
			get {
				if (dataProvider == null)
					dataProvider = CreateDefaultDataProvider();
				return dataProvider;
			}
			set {
				if (DataProvider == value)
					return;
				SchedulerLogger.Trace(XpfLoggerTraceLevel.RangeControl, "->RangeControlClient.setDataProvider, IsLoaded={0}", IsLoaded);
				DetachDataProvider(DataProvider);
				IRangeControlClientDataProvider newValue = GetActualDataProvider(value);
				AttachDataProvider(newValue);
				OnDataProviderChanged();
				if (IsLoaded)
					RefreshRangeControlCore(true);
			}
		}
		#endregion
		#region ViewPort
		internal Size ViewPort {
			get {
				return viewPort;
			}
			set {
				if (viewPort == value)
					return;
				viewPort = value;
			}
		}
		#endregion
		internal DevExpress.Xpf.Editors.RangeControl.Internal.LayoutInfo LayoutInfo { get; private set; }
		#endregion
		protected virtual ScaleBasedRangeControlController CreateController() {
			return new ScaleBasedRangeControlController();
		}
		TimeScale GetBaseTimeScale() {
			return Controller.BaseScale;
		}
		protected internal virtual void OnDataProviderChanged() {
			Controller.UpdateDataProvider(DataProvider);
			Controller.UpdateTotalRange(ClientSyncSupportInstance.Options.RangeMinimum, ClientSyncSupportInstance.Options.RangeMaximum);
			Controller.UpdateMaxSelectedIntervalCount(ClientSyncSupportInstance.Options.MaxSelectedIntervalCount);
			Controller.ValidateTotalRange(DataProvider.SelectedRangeStart, DataProvider.SelectedRangeEnd);
		}
		IRangeControlClientDataProvider GetActualDataProvider(IRangeControlClientDataProvider value) {
			return (value != null) ? value : CreateDefaultDataProvider();
		}
		protected virtual IRangeControlClientDataProvider CreateDefaultDataProvider() {
			return new EmptyTimeClientDataProvider();
		}
		protected void AttachDataProvider(IRangeControlClientDataProvider dataProvider) {
			this.dataProvider = dataProvider;
			DataProvider.SyncSupport = this;
			this.options = DataProvider.GetOptions();
			SubscribeOptionsEvents();
		}
		protected void DetachDataProvider(IRangeControlClientDataProvider dataProvider) {
			UnsubscribeOptionsEvents();
			if (this.timerDeferredSelectedRangeUpdate != null) {
				this.timerDeferredSelectedRangeUpdate.Stop();
				this.timerDeferredSelectedRangeUpdate = null;
			}
			this.options = null;
			dataProvider.SyncSupport = null;
			dataProvider.Dispose();
			dataProvider = null;
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			SchedulerLogger.Trace(XpfLoggerTraceLevel.RangeControl, "->RangeControlClient.Loaded: isClientInitialized={0}, OptionsIsNull={1}", this.isClientInitialized, this.options == null);
			if (WasLoadedBefore()) {
				using(Locker locker = ClientLock.Lock())
					RaiseLayoutChanged(LayoutChangedType.Data, Instance.Start, Instance.End);
				return;
			}
			if (this.options == null || this.isClientInitialized)
				return;
			using (Locker locker = ClientLock.Lock()) {
				ClientSyncSupportInstance.ReInitialize();
				this.isClientInitialized = true;
				RaiseLayoutChanged(LayoutChangedType.Data, Instance.Start, Instance.End);
			}
		}
		bool WasLoadedBefore() {
			return this.isClientInitialized && this.options != null;
		}
		void SubscribeOptionsEvents() {
			if (this.options == null)
				return;
			this.options.Changed += OnOptions;
		}
		void UnsubscribeOptionsEvents() {
			if (this.options == null)
				return;
			this.options.Changed -= OnOptions;
		}
		void OnOptions(object sender, DevExpress.Utils.Controls.BaseOptionChangedEventArgs e) {
			if (!this.isClientInitialized)
				return;
			if (e.Name == "Scales") {
				OnOptionsScalesChanged();
			}
			DataProvider.OnOptionsChanged(e.Name, e.OldValue, e.NewValue);
			RaiseLayoutChanged(LayoutChangedType.Data, ClientSyncSupportInstance.Options.RangeMinimum, ClientSyncSupportInstance.Options.RangeMaximum);
		}
		void OnOptionsScalesChanged() {
			ResetScaleFactorAdjusted();
			UpdateControllerRulers();
			RefreshRangeControlCore(true);
		}
		void OnClientSizeChanged(object sender, SizeChangedEventArgs e) {
			if (SchedulerControl == null)
				return;
			UpdateClientBounds();
			RaiseLayoutChanged();
		}
		private void UpdateClientBounds() {
			Rect bounds = new Rect(new Point(0, 0), new Size(ActualWidth, ActualHeight));
			this.clientBounds = bounds;
		}
		protected DateTime GetDefaultValue() {
			return DateTime.Today;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			if (double.IsInfinity(finalSize.Height) || double.IsInfinity(finalSize.Width))
				return base.ArrangeOverride(finalSize);
			if (Controller.RulerCount <= 0)
				return base.ArrangeOverride(finalSize);
			if (finalSize.Width <= 0 || finalSize.Height <= 0)
				return base.ArrangeOverride(finalSize);
			PrepareToRender(CalcUpdateBounds(finalSize));
			NormalPositionCalculator calculator = new NormalPositionCalculator(Controller);
			RulerGroupInfo viewInfo = calculator.Calculate(ActualVisibleStart, ActualVisibleEnd);
			RulerHeaderGroupLayout headerGroup = new RulerHeaderGroupLayout(this.headerItemGenerator);
			Rect headersBounds = headerGroup.Layout(new Rect(0, 0, ActualWidth, RenderBounds.Size.Height), viewInfo.Rulers);
			UpdateHitTestLayout(headerGroup.LevelBoundsList);
			double headerHeight = headersBounds.Height;
			double cellAreaHeight = RenderBounds.Size.Height - headerHeight;
			if (cellAreaHeight <= 0)
				return finalSize;
			CellContentLayout contentLayout = new CellContentLayout(this.contentItemGenerator);
			contentLayout.Layout(new Rect(0, headerHeight, ActualWidth, cellAreaHeight), viewInfo.BaseRuler);
			TimeIntervalCollection intervals = viewInfo.BaseRuler.GetIntervals();
			List<DataItemThumbnailList> thumbnails = Controller.CreateThumbnailData(intervals);
			ThumbnailListLayout thumbnailLayout = new ThumbnailListLayout(this.thumbnailItemGenerator, this.thumbnailGroupItemGenerator, contentLayout, this.SchedulerControl.Storage, ClientSyncSupportInstance.Options.DataDisplayType);
			thumbnailLayout.Layout(new Rect(0, headerHeight, ActualWidth, cellAreaHeight), thumbnails);
			return finalSize;
		}
		Rect CalcUpdateBounds(Size size) {
			return ViewPort.IsZero() ? new Rect(new Point(0, 0), size) : CalcRenderRect(size);
		}
		Rect CalcRenderRect(Size size) {
			double left = Controller.GetNormalizedValue(VisibleStart);
			return new Rect(new Point(size.Width * left, 0), ViewPort);
		}
		protected internal void PrepareToRender(Rect renderBounds) {
			RenderBounds = renderBounds;
		}
		protected void InvalidateRender() {
			InvalidateArrange();
		}
		protected virtual void RefreshRangeControlCore(bool forceRequestData) {
			if (!ClientLock) {
				using (Locker locker = ClientLock.Lock()) {
				CenteredSelection();
				RaiseLayoutChanged(LayoutChangedType.Data, Controller.Minimum, Controller.Maximum);
				InvalidateRender();
			}
		}
		}
		void CenteredSelection() {
			TimeSpan visibleDuration = VisibleEnd - VisibleStart;
			DateTime selectionCenterTime = CalculateSelectedRangeCenter();
			long halfVisibleDurationTicks = visibleDuration.Ticks / 2;
			DateTime newVisibleStart = selectionCenterTime.AddTicks(-halfVisibleDurationTicks);
			DateTime newVisibleEnd = selectionCenterTime.AddTicks(halfVisibleDurationTicks);
			if (newVisibleStart < Controller.Minimum) {
				newVisibleStart = Controller.Minimum;
				newVisibleEnd = newVisibleStart + visibleDuration;
			} else if (newVisibleEnd > Controller.Maximum) {
				newVisibleEnd = Controller.Maximum;
				newVisibleStart = newVisibleEnd - visibleDuration;
			}
			VisibleStart = newVisibleStart;
			VisibleEnd = newVisibleEnd;
		}
		void UpdateHitTestLayout(List<Rect> list) {
			if (HeaderLevelsBounds == null)
				HeaderLevelsBounds = new List<Rect>();
			HeaderLevelsBounds.Clear();
			HeaderLevelsBounds.AddRange(list);
		}
		TimeScale GetScaleViaPoint(Point pnt) {
			if (Controller.RulerCount <= 0)
				return null;
			int count = HeaderLevelsBounds.Count;
			if (count != Controller.RulerCount)
				return Controller.BaseScale;
			for (int i = 0; i < count; i++) {
				Rect headerLevelBounds = HeaderLevelsBounds[i];
				if (pnt.Y >= headerLevelBounds.Top && pnt.Y <= headerLevelBounds.Bottom)
					return Controller.ActualScales[i];
			}
			return Controller.BaseScale;
		}
	}
}
