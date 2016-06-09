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
using System.Text;
using DevExpress.XtraEditors;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Native;
using System.Drawing;
using DevExpress.Utils.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Controls;
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.XtraScheduler {
	public class ScaleBasedRangeControlClient : IRangeControlClient, IDisposable, IRangeControlClientSyncSupport, ISupportInitialize {
		public static DateTime DefaultRangeMinimum {
			get {
				DateTime now = DateTime.Now;
				DateTime startOfMonth = new DateTime(now.Year, now.Month, 1);
				return startOfMonth.AddMonths(-1);
			}
		}
		public static DateTime DefaultRangeMaximum {
			get {
				DateTime now = DateTime.Now;
				DateTime startOfMonth = new DateTime(now.Year, now.Month, 1);
				return startOfMonth.AddMonths(2);
			}
		}
		ScaleBasedRangeControlController controller;
		SchedulerRangeControlPainter painter;
		ScaleBasedRangeControlViewInfo viewInfo;
		IRangeControlClientDataProvider dataProvider;
		IScaleBasedRangeControlClientOptions options;
		NotificationCollectionChangedListener<TimeScale> timeScaleCollectionChangedListener;
		bool isDisposed;
		public ScaleBasedRangeControlClient(IRangeControlClientDataProvider dataProvider) {
			Initialize(dataProvider);
		}
		public ScaleBasedRangeControlClient() {
			Initialize(null);
		}
		public ScaleBasedRangeControlClient(RangeControl rangeControl) {
			Guard.ArgumentNotNull(rangeControl, "rangeControl");
			Initialize(null);
			RangeControl = rangeControl;
			UpdateControllerRulers();
		}
		#region Properties
		[Category(SRCategoryNames.Options),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)
		]
		public IScaleBasedRangeControlClientOptions Options {
			get {
				if (options == null)
					options = new ScaleBasedRangeControlClientOptions();
				return options;
			}
		}
		public RangeControl RangeControl { get; private set; }
		public IRangeControlClientDataProvider DataProvider {
			get {
				if (dataProvider == null)
					dataProvider = CreateDefaultDataProvider();
				return dataProvider;
			}
			set {
				if (DataProvider == value)
					return;
				DetachDataProvider(DataProvider);
				IRangeControlClientDataProvider newValue = GetActualDataProvider(value);
				AttachDataProvider(newValue);
				OnDataProviderChanged();
			}
		}
		private IRangeControlClientDataProvider GetActualDataProvider(IRangeControlClientDataProvider value) {
			return (value != null) ? value : CreateDefaultDataProvider();
		}
		protected virtual IRangeControlClientDataProvider CreateDefaultDataProvider() {
			return new DetachedRangeControlClientDataProvider(Options);
		}
		protected internal SchedulerRangeControlPainter Painter {
			get {
				if (painter == null)
					painter = CreatePainter();
				return painter;
			}
		}
		protected internal ScaleBasedRangeControlViewInfo ViewInfo {
			get {
				if (viewInfo == null)
					viewInfo = CreateViewInfo(RangeControl);
				return viewInfo;
			}
		}
		public virtual AppearanceObject CellAppearance { get; private set; }
		protected internal ScaleBasedRangeControlController Controller { get { return controller; } }
		protected internal bool ScaleFactorAdjusted { get; internal set; }
		protected internal bool EndInitialization { get; set; }
		internal bool IsDisposed { get { return isDisposed; } }
		#endregion
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (DataProvider != null) {
						DetachDataProvider(dataProvider);
					}
					if (viewInfo != null) {
						viewInfo.Dispose();
						viewInfo = null;
					}
					if (CellAppearance != null) {
						CellAppearance.Dispose();
						CellAppearance = null;
					}
					if (timeScaleCollectionChangedListener != null) {
						UnsubscribeScalesEvents();
						timeScaleCollectionChangedListener.Dispose();
						timeScaleCollectionChangedListener = null;
					}
					if (Options != null) {
						UnsubscribeOptionsEvents();
						options = null;
					}
					if (Controller != null) {
						Controller.UpdateDataProvider(null);
						controller = null;
					}
				}
			} finally {
				isDisposed = true;
			}
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~ScaleBasedRangeControlClient() {
			Dispose(false);
		}
		#endregion
		#region IRangeControlClient Members
		ClientRangeChangedEventHandler onRangeChanged;
		event ClientRangeChangedEventHandler IRangeControlClient.RangeChanged {
			add { onRangeChanged += value; }
			remove { onRangeChanged -= value; }
		}
		internal void RaiseRangeChanged(RangeControlClientRangeEventArgs e) {
			if (onRangeChanged != null)
				onRangeChanged(this, e);
		}
		bool IRangeControlClient.IsValidType(Type type) { return true; }
		void IRangeControlClient.UpdateHotInfo(RangeControlHitInfo hitInfo) {
			UpdateHotInfoCore(hitInfo);
		}
		void IRangeControlClient.UpdatePressedInfo(RangeControlHitInfo hitInfo) {
			UpdatePressedInfoCore(hitInfo);
		}
		void IRangeControlClient.OnClick(RangeControlHitInfo hitInfo) {
			OnClickCore(hitInfo);
		}
		string IRangeControlClient.InvalidText { get { return String.Empty; } }
		bool IRangeControlClient.IsValid { get { return true; } }
		object IRangeControlClient.GetOptions() {
			return Options;
		}
		bool IRangeControlClient.DrawRuler(RangeControlPaintEventArgs e) {
			return DrawRulerCore(e);
		}
		void IRangeControlClient.DrawContent(RangeControlPaintEventArgs e) {
			DrawContentCore(e);
		}
		bool IRangeControlClient.SupportOrientation(Orientation orientation) {
			return orientation == Orientation.Horizontal;
		}
		double IRangeControlClient.GetNormalizedValue(object value) {
			return GetNormalizedValueCore(value);
		}
		object IRangeControlClient.GetValue(double normalizedValue) {
			return GetValueCore(normalizedValue);
		}
		int IRangeControlClient.RangeBoxBottomIndent { get { return 0; } }
		int IRangeControlClient.RangeBoxTopIndent { get { return GetRangeBoxTopIndent(); } }
		string IRangeControlClient.RulerToString(int ruleIndex) { return string.Empty; }
		bool IRangeControlClient.IsCustomRuler { get { return true; } }
		List<object> IRangeControlClient.GetRuler(RulerInfoArgs e) { return null; }
		object IRangeControlClient.RulerDelta { get { return -1; } }
		double IRangeControlClient.NormalizedRulerDelta { get { return 0.0; } }
		void IRangeControlClient.OnRangeChanged(object rangeMinimum, object rangeMaximum) {
			AdjustRangeControlScaleFactor();
			OnRangeChangedCore(rangeMinimum, rangeMaximum);
		}
		void IRangeControlClient.OnRangeControlChanged(IRangeControl rangeControl) {
			OnRangeControlChangedCore((RangeControl)rangeControl);
		}
		void IRangeControlClient.ValidateRange(NormalizedRangeInfo info) {
			ValidateRangeCore(info);
		}
		double IRangeControlClient.ValidateScale(double newScale) {
			return ValidateScaleCore(newScale);
		}
		void IRangeControlClient.OnResize() {
			OnResizeCore();
		}
		void IRangeControlClient.Calculate(Rectangle contentRect) {
			CalculateCore(contentRect);
		}
		string IRangeControlClient.ValueToString(double normalizedValue) {
			return string.Empty;
		}
		#endregion
		#region ISupportInitialize implementation
		public void BeginInit() {
			EndInitialization = false;
		}
		public void EndInit() {
			EndInitialization = true;
		}
		#endregion
		protected virtual void Initialize(IRangeControlClientDataProvider dataProvider) {
			this.controller = CreateController();
			DataProvider = dataProvider != null ? dataProvider : CreateDefaultDataProvider();
			Controller.UpdateVisibleRulers(Options.Scales);
		}
		#region Options
		protected virtual void SubscribeOptionsEvents() {
			if (Options == null)
				return;
			Options.Changed += OnOptionsChanged;
		}
		protected virtual void UnsubscribeOptionsEvents() {
			if (Options == null)
				return;
			Options.Changed -= OnOptionsChanged;
		}
		protected void OnOptionsChanged(object sender, BaseOptionChangedEventArgs e) {
			if (e.Name == "Scales") {
				OnOptionsScalesChanged();
			}
			if (e.Name == "AutoFormatScaleCaptions")
				OnOptionsAutoFormatScaleCaptionsChanged();
			if (e.Name == "DataDisplayType")
				OnOptionsDataDisplayTypeChanged();
			if (e.Name == "MaxSelectedIntervalCount")
				OnOptionsMaxSelectedIntervalCountChanged();
			if (e.Name == "DataRowHeight")
				OnOptionsDataRowHeightChanged();
			if (e.Name == "RangeMinimum")
				OnOptionsRangeMinimumChanged();
			if (e.Name == "RangeMaximum")
				OnOptionsRangeMaximumChanged();
			if (e.Name == "MinIntervalWidth")
				OnOptionsMinIntervalWidthChanged();
			if (e.Name == "MaxIntervalWidth")
				OnOptionsMaxIntervalWidthChanged();
			if (string.IsNullOrEmpty(e.Name)) {
				OnOptionsBatchUpdated();
			}
			DataProvider.OnOptionsChanged(e.Name, e.OldValue, e.NewValue);
			RefreshLayout();
		}
		protected internal virtual void OnOptionsScalesChanged() {
			ResetScaleFactorAdjusted();
			UpdateControllerRulers();
			RefreshRangeControlCore(true);
		}
		protected virtual void OnOptionsRangeMinimumChanged() {
			UpdateControllerTotalRange();
			ResetScaleFactorAdjusted();
		}
		protected virtual void OnOptionsRangeMaximumChanged() {
			UpdateControllerTotalRange();
			ResetScaleFactorAdjusted();
		}
		protected virtual void OnOptionsMaxSelectedIntervalCountChanged() {
			UpdateControllerMaxSelectedIntervalCount();
		}
		protected virtual void OnOptionsAutoAdjustModeChanged() {
			InitControllerProperties();
			ResetScaleFactorAdjusted();
		}
		protected virtual void OnOptionsDataRowHeightChanged() {
		}
		protected virtual void OnOptionsAutoFormatScaleCaptionsChanged() {
		}
		protected virtual void OnOptionsDataDisplayTypeChanged() {
		}
		protected virtual void OnOptionsMinIntervalWidthChanged() {
			ResetScaleFactorAdjusted();
		}
		protected virtual void OnOptionsMaxIntervalWidthChanged() {
			ResetScaleFactorAdjusted();
		}
		protected virtual void OnOptionsBatchUpdated() {
			InitControllerProperties();
			ResetScaleFactorAdjusted();
		}
		protected void ResetScaleFactorAdjusted() {
			ScaleFactorAdjusted = false;
		}
		#endregion
		protected void DetachDataProvider(IRangeControlClientDataProvider dataProvider) {
			UnsubscribeOptionsEvents();
			UnsubscribeScalesEvents();
			UnsubscribeDataProviderEvents();
			timeScaleCollectionChangedListener = null;
			this.options = null;
			dataProvider.SyncSupport = null;
			dataProvider.Dispose();
			dataProvider = null;
		}
		protected void AttachDataProvider(IRangeControlClientDataProvider dataProvider) {
			this.dataProvider = dataProvider;
			DataProvider.SyncSupport = this;
			this.options = DataProvider.GetOptions();
			this.timeScaleCollectionChangedListener = CreateTimeScaleCollectionListener();
			SubscribeScalesEvents();
			SubscribeOptionsEvents();
			SubscribeDataProviderEvents();
		}
		protected internal virtual void OnDataProviderChanged() {
			Controller.UpdateDataProvider(DataProvider);
			Controller.UpdateTotalRange(Options.RangeMinimum, Options.RangeMaximum);
			Controller.UpdateMaxSelectedIntervalCount(Options.MaxSelectedIntervalCount);
		}
		protected internal virtual NotificationCollectionChangedListener<TimeScale> CreateTimeScaleCollectionListener() {
			return new NotificationCollectionChangedListener<TimeScale>(Options.Scales);
		}
		protected internal virtual void SubscribeScalesEvents() {
			if (timeScaleCollectionChangedListener != null)
				timeScaleCollectionChangedListener.Changed += new EventHandler(OnScalesChanged);
		}
		protected internal virtual void UnsubscribeScalesEvents() {
			if (timeScaleCollectionChangedListener != null)
				timeScaleCollectionChangedListener.Changed -= new EventHandler(OnScalesChanged);
		}
		protected internal virtual void OnScalesChanged(object sender, EventArgs e) {
			OnOptionsChanged(Options.Scales, new BaseOptionChangedEventArgs("Scales", null, null));
		}
		protected internal virtual void SubscribeDataProviderEvents() {
			ISupportObjectChanged supportChanged = DataProvider as ISupportObjectChanged;
			if (supportChanged != null)
				supportChanged.Changed += new EventHandler(OnDataProviderObjectChanged);
		}
		protected internal virtual void UnsubscribeDataProviderEvents() {
			ISupportObjectChanged supportChanged = DataProvider as ISupportObjectChanged;
			if (supportChanged != null)
				supportChanged.Changed -= new EventHandler(OnDataProviderObjectChanged);
		}
		protected virtual void OnDataProviderObjectChanged(object sender, EventArgs e) {
		}
		protected virtual TimeInterval GetTotalRangeCore() {
			return new TimeInterval(Controller.Minimum, Controller.Maximum);
		}
		protected virtual void InitControllerProperties() {
			UpdateControllerRulers();
			UpdateControllerTotalRange();
			UpdateControllerMaxSelectedIntervalCount();
		}
		protected virtual void UpdateControllerMaxSelectedIntervalCount() {
			Controller.UpdateMaxSelectedIntervalCount(Options.MaxSelectedIntervalCount);
		}
		protected virtual void UpdateControllerTotalRange() {
			Controller.UpdateTotalRange(Options.RangeMinimum, Options.RangeMaximum);
		}
		protected virtual void UpdateControllerRulers() {
			Controller.UpdateVisibleRulers(Options.Scales);
		}
		protected virtual void AdjustRangeControlScaleFactor() {
			if (ScaleFactorAdjusted)
				return;
			if (!IsRangeControlAssigned())
				return;
			RangeControl.VisibleRangeMaximumScaleFactor = double.PositiveInfinity;
			double factor = Controller.CalculateDesiredScaleFactor();
			double selectedRangeFactor = GetSelectedRangeFactor();
			double currentScaleFactor = 1 / RangeControl.VisibleRangeScaleFactor;
			if (Math.Max(Math.Max(selectedRangeFactor, factor), currentScaleFactor) == 0)
				return;
			RangeControl.VisibleRangeScaleFactor = 1 / Math.Max(Math.Max(selectedRangeFactor, factor), currentScaleFactor);
			ScaleFactorAdjusted = true;
		}
		double GetSelectedRangeFactor() {
			if (RangeControl == null || RangeControl.SelectedRange == null || RangeControl.SelectedRange.Maximum == null || RangeControl.SelectedRange.Minimum == null)
				return 0;
			double range = Controller.GetComparableValue(Controller.Maximum) - Controller.GetComparableValue(Controller.Minimum);
			double scale = Controller.GetComparableValue(((DateTime)RangeControl.SelectedRange.Maximum).AddDays(1)) - Controller.GetComparableValue((DateTime)RangeControl.SelectedRange.Minimum);
			return scale / range;
		}
		protected virtual ScaleBasedRangeControlController CreateController() {
			return new ScaleBasedRangeControlController();
		}
		protected virtual bool IsRangeControlAssigned() {
			return RangeControl != null && !RangeControl.IsDisposed;
		}
		private int GetRangeBoxTopIndent() {
			return ViewInfo.GetRangeBoxTopIndent();
		}
		protected virtual void UpdateHotInfoCore(RangeControlHitInfo hitInfo) {
			if (!IsRangeControlAssigned())
				return;
			ViewInfo.UpdateHotInfo(hitInfo);
		}
		protected virtual void UpdatePressedInfoCore(RangeControlHitInfo hitInfo) {
			if (!IsRangeControlAssigned())
				return;
			ViewInfo.UpdatePressedInfo(hitInfo);
		}
		protected virtual void OnClickCore(RangeControlHitInfo hitInfo) {
			if (!IsRangeControlAssigned())
				return;
			TimeInterval interval = ViewInfo.CalculateHitInterval(hitInfo);
			if (interval != null)
				ApplyHitInterval(interval);
		}
		protected virtual void ApplyHitInterval(TimeInterval interval) {
			Controller.SetHitInterval(interval);
		}
		protected virtual bool DrawRulerCore(RangeControlPaintEventArgs e) {
			Painter.DrawRuler(e, ViewInfo);
			return true;
		}
		protected virtual void DrawContentCore(RangeControlPaintEventArgs e) {
			if (!IsRangeControlAssigned())
				return;
			Painter.DrawContent(e, ViewInfo);
		}
		protected virtual void PrepareViewInfo(Rectangle contentRect) {
			if (ShouldCalculateViewInfo()) {
				UpdateCellAppearance();
				InitializeViewInfo();
				ViewInfo.Recalculate(contentRect);
			}
		}
		protected virtual SchedulerRangeControlPainter CreatePainter() {
			return new SchedulerRangeControlPainter();
		}
		protected virtual void InitializeViewInfo() {
			ViewInfo.ThumbnailHeight = Options.ThumbnailHeight;
			ViewInfo.DataDisplayType = Options.DataDisplayType;
		}
		protected virtual ScaleBasedRangeControlViewInfo CreateViewInfo(RangeControl rangeControl) {
			return new ScaleBasedRangeControlViewInfo(this, rangeControl);
		}
		protected virtual void DestroyViewInfo() {
			if (ViewInfo != null) {
				this.viewInfo.Dispose();
				this.viewInfo = null;
			}
		}
		protected virtual void UpdateCellAppearance() {
			if (CellAppearance == null) {
				CellAppearance = new AppearanceObject();
				CellAppearance.TextOptions.HAlignment = HorzAlignment.Center;
				CellAppearance.TextOptions.VAlignment = VertAlignment.Center;
				CellAppearance.TextOptions.Trimming = Trimming.EllipsisCharacter;
			}
			CellAppearance.ForeColor = ViewInfo.NumberForeColor;
			CellAppearance.Font = new Font(AppearanceObject.DefaultFont.FontFamily, ViewInfo.NumberFontSize);
		}
		protected virtual bool ShouldCalculateViewInfo() {
			return true; 
		}
		protected object GetValueCore(double normalizedValue) {
			return Controller.GetValue(normalizedValue);
		}
		protected double GetNormalizedValueCore(object value) {
			return Controller.GetNormalizedValue(value);
		}
		protected virtual void ValidateRangeCore(NormalizedRangeInfo info) {
			DateTime start = Controller.GetValue(info.Range.Minimum);
			DateTime end = Controller.GetValue(info.Range.Maximum);
			NormalizedRangeInfo validatedInfo = Controller.ValidateRange(start, end, info.Type);
			info.Range.Minimum = validatedInfo.Range.Minimum;
			info.Range.Maximum = validatedInfo.Range.Maximum;
		}
		protected virtual double ValidateScaleCore(double newScale) {
			double minScaleFactor = Controller.CalculateMinScaleFactor();
			double maxScaleFactor = Controller.CalculateMaxScaleFactor();
			XtraSchedulerDebug.Assert(maxScaleFactor >= minScaleFactor);
			double validScale = newScale;
			if (validScale > maxScaleFactor)
				validScale = maxScaleFactor;
			if (validScale < minScaleFactor)
				validScale = minScaleFactor;
			return validScale;
		}
		protected virtual void OnRangeChangedCore(object rangeMinimum, object rangeMaximum) {
			Controller.OnRangeChanged(rangeMinimum, rangeMaximum);
		}
		protected virtual void OnRangeControlChangedCore(RangeControl rangeControl) {
			if (RangeControl == rangeControl)
				return;
			RangeControl = rangeControl;
			DestroyViewInfo();
			ResetScaleFactorAdjusted();
		}
		protected virtual void OnResizeCore() {
			((IRangeControl)RangeControl).CenterSelectedRange();
		}
		protected virtual void CalculateCore(Rectangle contentRect) {
			AdjustRangeControlScaleFactor();
			PrepareViewInfo(contentRect);
			CompleteInitialization();
		}
		protected virtual void CompleteInitialization() {
			if (EndInitialization) {
				EndInitialization = false;
				RefreshLayout();
			}
		}
		public virtual DateTime GetFirstVisibleDate() {
			return Controller.GetFirstVisibleDate(RangeControl.VisibleRangeStartPosition);
		}
		public virtual DateTime GetLastVisibleDate() {
			double pos = RangeControl.VisibleRangeStartPosition + RangeControl.VisibleRangeWidth;
			return Controller.GetLastVisibleDate(pos);
		}
		public virtual DateTime GetNextVisibleDate(int rulerIndex, DateTime dateTime) {
			return Controller.GetNextVisibleDate(rulerIndex, dateTime);
		}
		internal string FindOptimalDateTimeFormat(TimeScale scale, DateTime[] dates, Graphics gr, Font font, int width) {
			return Controller.FindOptimalDateTimeFormat(scale, dates, gr, font, width);
		}
		internal string FormatRulerFixedFormatCaption(int rulerIndex, DateTime start, DateTime end) {
			return Controller.FormatRulerFixedFormatCaption(rulerIndex, start, end);
		}
		#region IRangeControlClientSyncSupport
		TimeInterval IRangeControlClientSyncSupport.TotalRange { get { return GetTotalRangeCore(); } }
		IScaleBasedRangeControlClientOptions IRangeControlClientSyncSupport.Options { get { return Options; } }
		void IRangeControlClientSyncSupport.RefreshRangeControl(bool forceRequestData) {
			RefreshRangeControlCore(forceRequestData);
		}
		void IRangeControlClientSyncSupport.SyncRangeControlRange() {
			SyncRangeControlRangeCore();
		}
		void IRangeControlClientSyncSupport.ReInitialize() {
			ReInitializeCore();
		}
		bool IRangeControlClientSyncSupport.CanAdjustRangeControl {
			get { return CanAdjustRangeControlCore(); }
		}
		void IRangeControlClientSyncSupport.AdjustRangeControlRange(RangeControlAdjustEventArgs e) {
			AdjustRangeControlRangeCore(e);
		}
		void IRangeControlClientSyncSupport.AdjustRangeControlScales(RangeControlAdjustEventArgs e) {
			AdjustRangeControlScalesCore(e);
		}
		double IRangeControlClientSyncSupport.ViewportWidth { get { return RangeControl.Width; } }
		#endregion
		protected virtual void RefreshRangeControlCore(bool forceRequestData) {
			if (!IsRangeControlAssigned())
				return;
			ViewInfo.ForceRequestData = forceRequestData;
			RefreshLayout();
		}
		protected virtual void RefreshLayout() {
			RangeControlRange range = Controller.PrepareRangeForUpdate();
			RangeControlClientRangeEventArgs args = CreateRangeControlClientRangeEventArgs(range, true);
			RaiseRangeChanged(args);
		}
		protected virtual RangeControlClientRangeEventArgs CreateRangeControlClientRangeEventArgs(RangeControlRange range, bool animated) {
			return new RangeControlClientRangeEventArgs() {
				Range = range,
				InvalidateContent = true,
				MakeRangeVisible = true,
				AnimatedViewport = animated
			};
		}
		protected virtual void SyncRangeControlRangeCore() {
			ResetScaleFactorAdjusted();
			RefreshRangeControlCore(true);
		}
		protected virtual void ReInitializeCore() {
			InitControllerProperties();
			ResetScaleFactorAdjusted();
		}
		public event RangeControlAdjustEventHandler RangeControlAutoAdjusting;
		protected internal virtual void RaiseRangeControlAutoAdjusting(RangeControlAdjustEventArgs args) {
			if (RangeControlAutoAdjusting != null)
				RangeControlAutoAdjusting(this, args);
		}
		protected virtual bool CanAdjustRangeControlCore() {
			return RangeControl != null && !RangeControl.IsDesignMode;
		}
		protected virtual void AdjustRangeControlRangeCore(RangeControlAdjustEventArgs e) {
			RaiseRangeControlAutoAdjusting(e);
			AdjustTotalRange(e.RangeMinimum, e.RangeMaximum);
		}
		protected virtual void AdjustRangeControlScalesCore(RangeControlAdjustEventArgs e) {
			RaiseRangeControlAutoAdjusting(e);
			AdjustTotalRange(e.RangeMinimum, e.RangeMaximum);
			AdjustVisibleScales(e.Scales);
			ResetScaleFactorAdjusted();
			RefreshLayout();
		}
		protected virtual void AdjustTotalRange(DateTime minimum, DateTime maximum) {
			Controller.UpdateTotalRange(minimum, maximum);
		}
		protected virtual void AdjustVisibleScales(TimeScaleCollection visibleScales) {
			Controller.UpdateVisibleRulers(visibleScales);
		}
		public virtual void ZoomIn() {
			TimeScaleCollectionHelper.ZoomIn(Options.Scales, Controller.ActualScales);
		}
		public virtual void ZoomOut() {
			TimeScaleCollectionHelper.ZoomOut(Options.Scales, Controller.ActualScales);
		}
		public virtual bool CanZoomIn() {
			return TimeScaleCollectionHelper.CanZoomIn(Options.Scales, Controller.ActualScales);
		}
		public virtual bool CanZoomOut() {
			return TimeScaleCollectionHelper.CanZoomOut(Options.Scales, Controller.ActualScales);
		}
	}
}
namespace DevExpress.XtraScheduler.Native {
	public class DetachedRangeControlClientDataProvider : IRangeControlClientDataProvider {
		IScaleBasedRangeControlClientOptions options;
		IRangeControlClientSyncSupport syncSupport;
		public DetachedRangeControlClientDataProvider(IScaleBasedRangeControlClientOptions options) {
			this.options = options;
			SelectedInterval = new TimeInterval(DateTime.Today, DateTime.Today.AddMonths(1));
		}
		public virtual DateTime SelectedRangeStart { get { return GetSelectedRangeStart(); } }
		public virtual DateTime SelectedRangeEnd { get { return GetSelectedRangeEnd(); } }
		IRangeControlClientSyncSupport IRangeControlClientDataProvider.SyncSupport { get { return syncSupport; } set { syncSupport = value; } }
		protected internal TimeInterval SelectedInterval { get; private set; }
		public IScaleBasedRangeControlClientOptions GetOptions() {
			return options;
		}
		void IRangeControlClientDataProvider.OnSelectedRangeChanged(DateTime rangeMinimum, DateTime rangeMaximum) {
			SelectedInterval = new TimeInterval(rangeMinimum, rangeMaximum);
		}
		void IRangeControlClientDataProvider.OnOptionsChanged(string name, object oldValue, object newValue) {
		}
		List<DataItemThumbnailList> IRangeControlClientDataProvider.CreateThumbnailData(TimeIntervalCollection intervals) {
			return new List<DataItemThumbnailList>();
		}
		DateTime GetSelectedRangeEnd() {
			return SelectedInterval != null ? SelectedInterval.End : DateTime.MaxValue;
		}
		DateTime GetSelectedRangeStart() {
			return SelectedInterval != null ? SelectedInterval.Start : DateTime.MinValue;
		}
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
			}
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~DetachedRangeControlClientDataProvider() {
			Dispose(false);
		}
		#endregion
	}
}
