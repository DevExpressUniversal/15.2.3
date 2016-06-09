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
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Effects;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using System.Windows.Threading;
using DevExpress.Xpf.Core.Internal;
using DevExpress.Xpf.Editors.RangeControl.Internal;
using System.Diagnostics;
using DevExpress.Utils;
using System.ComponentModel;
using DevExpress.Data.Filtering;
namespace DevExpress.Xpf.Editors.RangeControl {
	public enum ShadingModes {
		Shading,
		Grayscale,
		Both
	}
	[DXToolboxBrowsable]
	[ToolboxTabName(AssemblyInfo.DXTabWpfCommon)]
	[ContentProperty("Client")]
	public class RangeControl : Control {
		const double MinVisibleThumbPart = 5;
		const int DefaultPostDelay = 500;
		const double AutoScrollVelocityFactor = 0.05;
		const double MinNavigationOffset = 15;
		#region Static
		static readonly DependencyPropertyKey IsSelectionMovingPropertyKey;
		protected static readonly DependencyPropertyKey OwnerRangeControlPropertyKey;
		public static readonly DependencyProperty OwnerRangeControlProperty;
		public static readonly DependencyProperty ClientProperty;
		public static readonly DependencyProperty SelectionRangeStartProperty;
		public static readonly DependencyProperty SelectionRangeEndProperty;
		public static readonly DependencyProperty VisibleRangeStartProperty;
		public static readonly DependencyProperty VisibleRangeEndProperty;
		public static readonly DependencyProperty RangeStartProperty;
		public static readonly DependencyProperty RangeEndProperty;
		public static readonly DependencyProperty AllowSnapToIntervalProperty;
		public static readonly DependencyProperty ShowRangeBarProperty;
		public static readonly DependencyProperty ShowRangeThumbsProperty;
		public static readonly DependencyProperty AllowImmediateRangeUpdateProperty;
		public static readonly DependencyProperty AllowScrollProperty;
		public static readonly DependencyProperty AllowZoomProperty;
		public static readonly DependencyProperty ShowSelectionRectangleProperty;
		public static readonly DependencyProperty EnableAnimationProperty;
		public static readonly DependencyProperty IsSelectionMovingProperty;
		public static readonly DependencyProperty ShowLabelsProperty;
		public static readonly DependencyProperty ShowNavigationButtonsProperty;
		public static readonly DependencyProperty LabelTemplateProperty;
		public static readonly DependencyProperty UpdateDelayProperty;
		public static readonly DependencyProperty ShadingModeProperty;
		protected static readonly DependencyPropertyKey SelectionRangePropertyKey;
		public static readonly DependencyProperty SelectionRangeProperty;
		public static readonly DependencyProperty ResetRangesOnClientItemsSourceChangedProperty;
		static RangeControl() {
			Type ownerType = typeof(RangeControl);
			OwnerRangeControlPropertyKey =
			DependencyProperty.RegisterAttachedReadOnly("OwnerRangeControl", ownerType, ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
			OwnerRangeControlProperty = OwnerRangeControlPropertyKey.DependencyProperty;
			ClientProperty =
			DependencyProperty.Register("Client", typeof(IRangeControlClient), ownerType,
			new FrameworkPropertyMetadata((d, e) => ((RangeControl)d).OnClientChanged(e.OldValue as IRangeControlClient, e.NewValue as IRangeControlClient)));
			SelectionRangeStartProperty =
			DependencyProperty.Register("SelectionRangeStart", typeof(object), ownerType,
			new FrameworkPropertyMetadata((d, e) => ((RangeControl)d).OnSelectionRangeStartChanged()));
			SelectionRangeEndProperty =
			DependencyProperty.Register("SelectionRangeEnd", typeof(object), ownerType,
			new FrameworkPropertyMetadata((d, e) => ((RangeControl)d).OnSelectionRangeEndChanged()));
			VisibleRangeStartProperty =
			DependencyProperty.Register("VisibleRangeStart", typeof(object), ownerType,
			new FrameworkPropertyMetadata((d, e) => ((RangeControl)d).OnVisibleRangeStartChanged(e.OldValue)));
			VisibleRangeEndProperty =
			DependencyProperty.Register("VisibleRangeEnd", typeof(object), ownerType,
			new FrameworkPropertyMetadata((d, e) => ((RangeControl)d).OnVisibleRangeEndChanged(e.OldValue)));
			RangeStartProperty =
			DependencyProperty.Register("RangeStart", typeof(object), ownerType,
			new FrameworkPropertyMetadata((d, e) => ((RangeControl)d).OnRangeStartChanged()));
			RangeEndProperty =
			DependencyProperty.Register("RangeEnd", typeof(object), ownerType,
			new FrameworkPropertyMetadata((d, e) => ((RangeControl)d).OnRangeEndChanged()));
			AllowSnapToIntervalProperty =
			DependencyProperty.Register("AllowSnapToInterval", typeof(bool), ownerType,
			new FrameworkPropertyMetadata(true, (d, e) => ((RangeControl)d).OnAllowSnapToIntervalChanged()));
			ShowRangeBarProperty =
			DependencyProperty.Register("ShowRangeBar", typeof(bool), ownerType,
			new FrameworkPropertyMetadata(true, (d, e) => ((RangeControl)d).OnShowRangeBarChanged()));
			ShowRangeThumbsProperty =
			DependencyProperty.Register("ShowRangeThumbs", typeof(bool), ownerType,
			new FrameworkPropertyMetadata(true, (d, e) => ((RangeControl)d).OnShowRangeThumbsChanged()));
			AllowImmediateRangeUpdateProperty =
			DependencyProperty.Register("AllowImmediateRangeUpdate", typeof(bool), ownerType,
			new FrameworkPropertyMetadata(false, (d, e) => ((RangeControl)d).OnAllowImmediateUpdateChanged()));
			AllowScrollProperty =
			DependencyProperty.Register("AllowScroll", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			AllowZoomProperty =
			DependencyProperty.Register("AllowZoom", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			ShowSelectionRectangleProperty =
			DependencyProperty.Register("ShowSelectionRectangle", typeof(bool?), ownerType, new FrameworkPropertyMetadata(null));
			EnableAnimationProperty =
			DependencyProperty.Register("EnableAnimation", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			IsSelectionMovingPropertyKey = DependencyProperty.RegisterReadOnly("IsSelectionMoving", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			IsSelectionMovingProperty = IsSelectionMovingPropertyKey.DependencyProperty;
			ShowLabelsProperty =
			DependencyProperty.Register("ShowLabels", typeof(bool), ownerType,
			new FrameworkPropertyMetadata(true, (d, e) => ((RangeControl)d).OnShowLabelsChanged()));
			ShowNavigationButtonsProperty =
			DependencyProperty.Register("ShowNavigationButtons", typeof(bool), ownerType,
			new FrameworkPropertyMetadata(true, (d, e) => ((RangeControl)d).OnShowNavigationButtonsChanged()));
			LabelTemplateProperty =
			DependencyProperty.Register("LabelTemplate", typeof(DataTemplate), ownerType,
			new FrameworkPropertyMetadata(null, (d, e) => ((RangeControl)d).OnShowLabelTemplateChanged()));
			UpdateDelayProperty =
			DependencyProperty.Register("UpdateDelay", typeof(int), ownerType, new FrameworkPropertyMetadata(
				DefaultPostDelay,
			   (d, e) => ((RangeControl)d).OnUpdateDelayChangedChanged(),
			   (d, o) => ((RangeControl)d).CoerceUpdateDelay((int)o)));
			ShadingModeProperty =
			DependencyProperty.Register("ShadingMode", typeof(ShadingModes), ownerType, new FrameworkPropertyMetadata(
				ShadingModes.Both,
			   (d, e) => ((RangeControl)d).OnShadingModeChanged()));
			SelectionRangePropertyKey = DependencyProperty.RegisterReadOnly("SelectionRange", typeof(EditRange), ownerType, new FrameworkPropertyMetadata(null));
			SelectionRangeProperty = SelectionRangePropertyKey.DependencyProperty;
			ResetRangesOnClientItemsSourceChangedProperty =
			DependencyProperty.Register("ResetRangesOnClientItemsSourceChanged", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
		}
		private void OnShadingModeChanged() {
			Invalidate();
		}
		private object CoerceUpdateDelay(int baseValue) {
			return Math.Sign(baseValue) != -1 ? baseValue : 0;
		}
		private void OnUpdateDelayChangedChanged() { }
		public static RangeControl GetOwnerRangeControl(DependencyObject element) {
			if (element == null) return null;
			return (RangeControl)element.GetValue(OwnerRangeControlProperty);
		}
		internal static void SetOwnerRangeControl(DependencyObject element, RangeControl value) {
			if (element == null) return;
			element.SetValue(OwnerRangeControlPropertyKey, value);
		}
		#endregion
		readonly List<object> logicalChildren = new List<object>();
		public RangeControl() {
			SetOwnerRangeControl(this, this);
			DefaultStyleKey = typeof(RangeControl);
			this.Loaded += RangeControlLoaded;
			AllowUpdateNormalizedRange = true;
			RangeUpdater = new StartEndUpdateHelper(this, RangeControl.RangeStartProperty, RangeControl.RangeEndProperty);
			VisibleRangeUpdater = new StartEndUpdateHelper(this, RangeControl.VisibleRangeStartProperty, RangeControl.VisibleRangeEndProperty);
			SelectionRangeUpdater = new StartEndUpdateHelper(this, RangeControl.SelectionRangeStartProperty, RangeControl.SelectionRangeEndProperty);
			startEndPropertyChangedAction = new PostponedAction(() => shouldLockUpdate);
			UpdateSelectionLocker = new Locker();
		}
		PostponedAction startEndPropertyChangedAction;
		public IRangeControlClient Client {
			get { return (IRangeControlClient)GetValue(ClientProperty); }
			set { SetValue(ClientProperty, value); }
		}
		public bool ResetRangesOnClientItemsSourceChanged {
			get { return (bool)GetValue(ResetRangesOnClientItemsSourceChangedProperty); }
			set { SetValue(ResetRangesOnClientItemsSourceChangedProperty, value); }
		}
		public EditRange SelectionRange {
			get { return (EditRange)GetValue(SelectionRangeProperty); }
			private set { SetValue(SelectionRangePropertyKey, value); }
		}
		public object SelectionRangeStart {
			get { return (object)GetValue(SelectionRangeStartProperty); }
			set { SetValue(SelectionRangeStartProperty, value); }
		}
		public object SelectionRangeEnd {
			get { return (object)GetValue(SelectionRangeEndProperty); }
			set { SetValue(SelectionRangeEndProperty, value); }
		}
		public object VisibleRangeStart {
			get { return (object)GetValue(VisibleRangeStartProperty); }
			set { SetValue(VisibleRangeStartProperty, value); }
		}
		public object VisibleRangeEnd {
			get { return (object)GetValue(VisibleRangeEndProperty); }
			set { SetValue(VisibleRangeEndProperty, value); }
		}
		public object RangeStart {
			get { return (object)GetValue(RangeStartProperty); }
			set { SetValue(RangeStartProperty, value); }
		}
		public object RangeEnd {
			get { return (object)GetValue(RangeEndProperty); }
			set { SetValue(RangeEndProperty, value); }
		}
		public bool AllowSnapToInterval {
			get { return (bool)GetValue(AllowSnapToIntervalProperty); }
			set { SetValue(AllowSnapToIntervalProperty, value); }
		}
		public bool ShowRangeBar {
			get { return (bool)GetValue(ShowRangeBarProperty); }
			set { SetValue(ShowRangeBarProperty, value); }
		}
		public bool ShowRangeThumbs {
			get { return (bool)GetValue(ShowRangeThumbsProperty); }
			set { SetValue(ShowRangeThumbsProperty, value); }
		}
		public bool AllowImmediateRangeUpdate {
			get { return (bool)GetValue(AllowImmediateRangeUpdateProperty); }
			set { SetValue(AllowImmediateRangeUpdateProperty, value); }
		}
		public bool AllowScroll {
			get { return (bool)GetValue(AllowScrollProperty); }
			set { SetValue(AllowScrollProperty, value); }
		}
		public bool AllowZoom {
			get { return (bool)GetValue(AllowZoomProperty); }
			set { SetValue(AllowZoomProperty, value); }
		}
		public bool EnableAnimation {
			get { return (bool)GetValue(EnableAnimationProperty); }
			set { SetValue(EnableAnimationProperty, value); }
		}
		public bool? ShowSelectionRectangle {
			get { return (bool?)GetValue(ShowSelectionRectangleProperty); }
			set { SetValue(ShowSelectionRectangleProperty, value); }
		}
		public bool IsSelectionMoving {
			get { return (bool)GetValue(IsSelectionMovingProperty); }
			private set { SetValue(IsSelectionMovingPropertyKey, value); }
		}
		public bool ShowLabels {
			get { return (bool)GetValue(ShowLabelsProperty); }
			set { SetValue(ShowLabelsProperty, value); }
		}
		public bool ShowNavigationButtons {
			get { return (bool)GetValue(ShowNavigationButtonsProperty); }
			set { SetValue(ShowNavigationButtonsProperty, value); }
		}
		public DataTemplate LabelTemplate {
			get { return (DataTemplate)GetValue(LabelTemplateProperty); }
			set { SetValue(LabelTemplateProperty, value); }
		}
		public int UpdateDelay {
			get { return (int)GetValue(UpdateDelayProperty); }
			set { SetValue(UpdateDelayProperty, value); }
		}
		public ShadingModes ShadingMode {
			get { return (ShadingModes)GetValue(ShadingModeProperty); }
			set { SetValue(ShadingModeProperty, value); }
		}
		bool HasVisibleRangeWidthChange {
			get { return !DoubleExtensions.AreClose(GetActualVisibleRange(), LastVisibleRangeWidth); }
		}
		double LastVisibleRangeWidth { get; set; }
		RangeControlAnimator animator;
		RangeControlAnimator Animator {
			get {
				if (animator == null) {
					animator = new RangeControlAnimator();
					animator.AnimationCompleted += OnAnimationCompleted;
				}
				return animator;
			}
		}
		bool AllowUpdateNormalizedRange { get; set; }
		SelectionChangesType SelectionType { get; set; }
		RangeControlController Controller { get; set; }
		bool IsStartManualBehavior { get; set; }
		bool IsEndManualBehavior { get; set; }
		bool IsVisibleRangeCorrected { get; set; }
		bool IsAutoScrollInProcess { get; set; }
		Locker UpdateSelectionLocker { get; set; }
		double ScaleFactor {
			get {
				double visibleRange = GetActualVisibleRange();
				return visibleRange > 0 ? 1 / visibleRange : 1;
			}
		}
		object actualSelectionStart, actualSelectionEnd;
		internal object ActualSelectionStart {
			get { return actualSelectionStart; }
			set {
				if (actualSelectionStart != value) {
					actualSelectionStart = value;
					if (Client == null) return;
					OnActualSelectionStartChanged();
				}
			}
		}
		internal object ActualSelectionEnd {
			get { return actualSelectionEnd; }
			set {
				if (actualSelectionEnd != value) {
					actualSelectionEnd = value;
					if (Client == null) return;
					OnActualSelectionEndChanged();
				}
			}
		}
		object actualVisibleStart;
		internal object ActualVisibleStart {
			get { return actualVisibleStart; }
			set {
				if (actualVisibleStart != value) {
					actualVisibleStart = value;
					if (value == null) {
						Invalidate();
						return;
					}
					if (Client == null) return;
					OnActualVisibleStartChanged();
				}
			}
		}
		object actualVisibleEnd;
		internal object ActualVisibleEnd {
			get { return actualVisibleEnd; }
			set {
				if (actualVisibleEnd != value) {
					actualVisibleEnd = value;
					if (value == null) {
						Invalidate();
						return;
					}
					if (Client == null) return;
					OnActualVisibleEndChanged();
				}
			}
		}
		double normalizedSelectionStart = double.NaN;
		internal double NormalizedSelectionStart {
			get { return normalizedSelectionStart; }
			set {
				if (normalizedSelectionStart != value) {
					normalizedSelectionStart = value;
					OnNormalizedRangeChanged();
				}
			}
		}
		double normalizedSelectionEnd = double.NaN;
		internal double NormalizedSelectionEnd {
			get { return normalizedSelectionEnd; }
			set {
				if (normalizedSelectionEnd != value) {
					normalizedSelectionEnd = value;
					OnNormalizedRangeChanged();
				}
			}
		}
		bool IsRangeControlInitialized { get; set; }
		RangeControlClientHitTestResult CurrentHitTest { get; set; }
		bool StopPosting { get; set; }
		bool IsInteraction { get; set; }
		bool IsSelectionRangeLessVisibleRange { get; set; }
		bool IsNormalizedRangeSnappedToCenter { get; set; }
		double AutoScrollDelta { get { return AutoScrollVelocityFactor * GetActualVisibleRange(); } }
		double ClientWidth { get { return clientPanel.ActualWidth; } }
		protected virtual void OnRangeEndChanged() {
			IsEndManualBehavior = true;
			if (Client == null) return;
			if (updateRangeLocker)
				return;
			RangeUpdater.EndValue = new IComparableObjectWrapper(Client.GetComparableValue(RangeEnd), RangeEnd, false);
			using (updateRangeLocker.Lock()) {
				startEndPropertyChangedAction.PerformPostpone(() => RangeUpdater.Update<double>(StartEndUpdateSource.EndChanged));
			}
			if (shouldLockUpdate)
				return;
			StartEndChanged();
		}
		protected virtual void OnAllowSnapToIntervalChanged() {
			if (CanSnapToInterval()) {
				object start = GetCenterSnappedValue(ActualSelectionStart, false);
				object end = GetCenterSnappedValue(ActualSelectionEnd, true);
				SetActualSelectionRangeInternal(start, end);
				Invalidate();
			}
		}
		protected virtual void OnShowRangeThumbsChanged() {
			UpdateThumbVisibility();
		}
		private void UpdateThumbVisibility() {
			if (leftSelectionThumb != null && rightSelectionThumb != null) {
				leftSelectionThumb.Opacity = rightSelectionThumb.Opacity = IsThumbsVisible() ? 1 : 0;
			}
		}
		private bool IsThumbsVisible() {
			return ShowRangeThumbs && (Client != null ? Client.AllowThumbs : true);
		}
		protected virtual void OnShowRangeBarChanged() {
			InvalidateArrange();
		}
		Locker updateRangeLocker = new Locker();
		bool shouldLockUpdate = true;
		StartEndUpdateHelper RangeUpdater;
		StartEndUpdateHelper VisibleRangeUpdater;
		StartEndUpdateHelper SelectionRangeUpdater;
		protected virtual void OnRangeStartChanged() {
			IsStartManualBehavior = true;
			if (Client == null) return;
			if (updateRangeLocker)
				return;
			RangeUpdater.StartValue = new IComparableObjectWrapper(Client.GetComparableValue(RangeStart), RangeStart, false);
			using (updateRangeLocker.Lock()) {
				startEndPropertyChangedAction.PerformPostpone(() => RangeUpdater.Update<double>(StartEndUpdateSource.StartChanged));
			}
			if (shouldLockUpdate)
				return;
			StartEndChanged();
		}
		protected virtual void OnVisibleRangeStartChanged(object oldValue) {
			if (updateRangeLocker)
				return;
			if (Client != null) {
				VisibleRangeUpdater.StartValue = new IComparableObjectWrapper(Client.GetComparableValue(VisibleRangeStart), VisibleRangeStart, false);
				using (updateRangeLocker.Lock()) {
					startEndPropertyChangedAction.PerformPostpone(() => VisibleRangeUpdater.Update<double>(StartEndUpdateSource.StartChanged));
				}
				if (shouldLockUpdate)
					return;
			}
			ActualVisibleStart = VisibleRangeStart;
		}
		protected virtual void OnActualVisibleRangeChanged() {
			if (!renderLocker.IsLocked) {
				SetClientPanelWidth();
				SyncronizeHorizontalOffset();
				Invalidate();
			}
		}
		protected override void OnInitialized(EventArgs e) {
			shouldLockUpdate = false;
			using (updateRangeLocker.Lock()) {
				startEndPropertyChangedAction.PerformForce(() => RangeUpdater.Update<double>(StartEndUpdateSource.ISupportInitialize));
				startEndPropertyChangedAction.PerformForce(() => VisibleRangeUpdater.Update<double>(StartEndUpdateSource.ISupportInitialize));
				startEndPropertyChangedAction.PerformForce(() => SelectionRangeUpdater.Update<double>(StartEndUpdateSource.ISupportInitialize));
			}
			base.OnInitialized(e);
		}
		protected virtual void OnVisibleRangeEndChanged(object oldValue) {
			if (updateRangeLocker)
				return;
			if (Client != null) {
				VisibleRangeUpdater.EndValue = new IComparableObjectWrapper(Client.GetComparableValue(VisibleRangeEnd), VisibleRangeEnd, false);
				using (updateRangeLocker.Lock()) {
					startEndPropertyChangedAction.PerformPostpone(() => VisibleRangeUpdater.Update<double>(StartEndUpdateSource.EndChanged));
				}
				if (shouldLockUpdate)
					return;
			}
			ActualVisibleEnd = VisibleRangeEnd;
		}
		protected virtual void OnSelectionRangeEndChanged() {
			if (updateRangeLocker)
				return;
			if (Client != null) {
				SelectionRangeUpdater.EndValue = new IComparableObjectWrapper(Client.GetComparableValue(SelectionRangeEnd), SelectionRangeEnd, false);
				using (updateRangeLocker.Lock()) {
					startEndPropertyChangedAction.PerformPostpone(() => SelectionRangeUpdater.Update<double>(StartEndUpdateSource.EndChanged));
				}
				if (shouldLockUpdate)
					return;
			}
			SnapActualSelectionEnd();
			if (!updateSelectedRangeLocker.IsLocked) UpdateSelectedRange();
		}
		private void SnapActualSelectionEnd() {
			if (!UpdateSelectionLocker.IsLocked && CanSnapToInterval())
				ActualSelectionEnd = GetCenterSnappedValue(SelectionRangeEnd, true);
			else
				ActualSelectionEnd = SelectionRangeEnd;
		}
		private object GetCenterSnappedValue(object value, bool isRight) {
			return Client != null && Client.ConvergeThumbsOnZoomingOut ? SnapToCenter(value, isRight) : value;
		}
		protected virtual void OnSelectionRangeStartChanged() {
			if (updateRangeLocker)
				return;
			if (Client != null) {
				SelectionRangeUpdater.StartValue = new IComparableObjectWrapper(Client.GetComparableValue(SelectionRangeStart), SelectionRangeStart, false);
				using (updateRangeLocker.Lock()) {
					startEndPropertyChangedAction.PerformPostpone(() => SelectionRangeUpdater.Update<double>(StartEndUpdateSource.StartChanged));
				}
				if (shouldLockUpdate)
					return;
			}
			SnapActualSelectionStart();
			if (!updateSelectedRangeLocker.IsLocked) UpdateSelectedRange();
		}
		private void SnapActualSelectionStart() {
			if (!UpdateSelectionLocker.IsLocked && CanSnapToInterval())
				ActualSelectionStart = GetCenterSnappedValue(SelectionRangeStart, false);
			else
				ActualSelectionStart = SelectionRangeStart;
		}
		private bool CanSnapToInterval() {
			return AllowSnapToInterval || (Client != null && Client.SnapSelectionToGrid);
		}
		protected virtual void OnClientChanged(IRangeControlClient oldClient, IRangeControlClient newClient) {
			if (oldClient != null)
				RemoveLogicalChild(oldClient);
			if (newClient != null)
				AddLogicalChild(newClient);
			IsRangeControlInitialized = false;
			if (newClient != null) {
				UpdatePropertiesOnClientChanged();
				if (IsViewPortValid())
					Initialize();
			}
		}
		protected override System.Collections.IEnumerator LogicalChildren { get { return logicalChildren.GetEnumerator(); } }
		protected internal new void AddLogicalChild(object child) {
			if (!logicalChildren.Contains(child)) {
				logicalChildren.Add(child);
				base.AddLogicalChild(child);
			}
		}
		protected internal new void RemoveLogicalChild(object child) {
			if (logicalChildren.Contains(child)) {
				logicalChildren.Remove(child);
				base.RemoveLogicalChild(child);
			}
		}
		private void UpdatePropertiesOnClientChanged() {
			Client.LayoutChanged += OnClientLayoutChanged;
			UpdateVisibility();
		}
		private void UpdateVisibility() {
			UpdateThumbVisibility();
			UpdateOutOfRangeBorderVisibility();
		}
		private void UpdateOutOfRangeBorderVisibility() {
			if (leftSide != null && rightSide != null) {
				if (Client != null) rightSide.Visibility = leftSide.Visibility = Client.AllowThumbs ? Visibility.Visible : Visibility.Collapsed;
				else rightSide.Visibility = leftSide.Visibility = Visibility.Collapsed;
			}
		}
		protected virtual void OnAllowImmediateUpdateChanged() { }
		private void OnShowLabelTemplateChanged() {
			Invalidate();
		}
		private void OnShowNavigationButtonsChanged() {
			UpdateNavigationButtonsVisibility();
			Invalidate();
		}
		private void OnShowLabelsChanged() {
			UpdateLabelsVisibility();
			Invalidate();
		}
		private void UpdateLabelsVisibility() {
			if (leftLabel != null && rightLabel != null)
				leftLabel.Visibility = rightLabel.Visibility = IsLabelsVisible() ? Visibility.Visible : Visibility.Collapsed;
		}
		private bool IsLabelsVisible() {
			return ShowLabels && (Client != null ? Client.AllowThumbs : true);
		}
		private void UpdateNavigationButtonsVisibility() {
			if (leftNavigationButton != null)
				leftNavigationButton.Visibility = IsLeftNavigationButtonsVisible() ? Visibility.Visible : Visibility.Collapsed;
			if (rightNavigationButton != null)
				rightNavigationButton.Visibility = IsRightNavigationButtonsVisible() ? Visibility.Visible : Visibility.Collapsed;
		}
		private bool IsRightNavigationButtonsVisible() {
			return ShowNavigationButtons && IsSelectionEndGreaterVisibleEnd(!IsAutoScrollInProcess);
		}
		private bool IsLeftNavigationButtonsVisible() {
			return ShowNavigationButtons && IsSelectionStartLessVisibleStart(!IsAutoScrollInProcess);
		}
		double precision = 0.000000000001;
		private bool IsSelectionStartLessVisibleStart(bool isStrong) {
			return isStrong ? DoubleExtensions.LessThan(NormalizedSelectionStart, GetNormalizedVisibleStart()) : DoubleExtensions.LessThanOrClose(NormalizedSelectionStart - precision, GetNormalizedVisibleStart());
		}
		private bool IsSelectionEndGreaterVisibleEnd(bool isStrong) {
			return isStrong ? DoubleExtensions.GreaterThan(NormalizedSelectionEnd, GetNormalizedVisibleEnd()) : DoubleExtensions.GreaterThanOrClose(NormalizedSelectionEnd + precision, GetNormalizedVisibleEnd());
		}
		Canvas clientPanel;
		ScrollViewer scrollViewer;
		RangeBar rangeBar;
		Thumb leftSelectionThumb;
		Thumb rightSelectionThumb;
		Border leftSide;
		Border rightSide;
		Border selectionRectangle;
		ContentPresenter content;
		Canvas interactionArea;
		Canvas layoutPanel;
		Thumb draggedThumb, fixedThumb;
		Grid rootContainer;
		Button leftNavigationButton, rightNavigationButton;
		ContentPresenter leftLabel, rightLabel;
		Grid navigationButtonsContainer;
		Canvas selectionRactangleContainer;
		ContentControl contentControl;
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			FindElements();
			InvalidateMeasure();
			rangeBar.Owner = this;
			Controller = new RangeControlController(clientPanel, this, LayoutHelper.FindElementByName(rootContainer, "PART_ClientPanelArea") as Grid);
			SetUpShaderEffect();
			Invalidate();
		}
		protected override Size MeasureOverride(Size constraint) {
			var size = new Size();
			if (double.IsInfinity(constraint.Width) || double.IsInfinity(constraint.Height)) {
				if (rightNavigationButton != null) {
					var visibility = rightNavigationButton.Visibility;
					rightNavigationButton.Visibility = System.Windows.Visibility.Visible;
					rightNavigationButton.Measure(constraint);
					size = new Size(rightNavigationButton.DesiredSize.Width * 2, rightNavigationButton.DesiredSize.Height);
					rightNavigationButton.Visibility = visibility;
				}
				if (ShowRangeBar && rangeBar != null) {
					rangeBar.Measure(constraint);
					var height = rangeBar.DesiredSize.Height + size.Height;
					size = new Size(size.Width, height);
				}
			}
			double wholeWidth = double.IsInfinity(constraint.Width) ? size.Width : constraint.Width;
			double wholeHeight = double.IsInfinity(constraint.Height) ? size.Height : constraint.Height;
			var resultSize = new Size(wholeWidth, wholeHeight);
			if (contentControl != null)
				contentControl.Measure(resultSize);
			return resultSize;
		}
		private void FindElements() {
			UnSubscribeEvents();
			if (scrollViewer != null) isSyncronizeHorizontalOffset = true;
			contentControl = GetContentBorder();
			rootContainer = LayoutHelper.FindElementByName((contentControl.Content as FrameworkElement), "PART_RootContainer") as Grid;
			scrollViewer = LayoutHelper.FindElementByName(rootContainer, "PART_ScrollViewer") as ScrollViewer;
			rangeBar = LayoutHelper.FindElementByName(rootContainer, "PART_RangeBar") as RangeBar;
			selectionRectangle = LayoutHelper.FindElementByName(rootContainer, "PART_SelectionRectangle") as Border;
			selectionRactangleContainer = LayoutHelper.GetParent(selectionRectangle) as Canvas;
			layoutPanel = LayoutHelper.FindElementByName(rootContainer, "PART_LayoutPanel") as Canvas;
			clientPanel = LayoutHelper.FindElementByName((FrameworkElement)scrollViewer.Content, "PART_ClientPanel") as Canvas;
			interactionArea = LayoutHelper.FindElementByName((FrameworkElement)scrollViewer.Content, "PART_InteractionArea") as Canvas;
			InitializeElements();
			SubscribeEvents();
		}
		private ContentControl GetContentBorder() {
			return LayoutHelper.FindElementByName(this, "PART_Border") as ContentControl;
		}
		private void SubscribeEvents() {
			rootContainer.SizeChanged += RootContainerSizeChanged;
			scrollViewer.ScrollChanged += OnScrollViewerScrollChanged;
			leftLabel.SizeChanged += OnLabelSizeChanged;
			rightLabel.SizeChanged += OnLabelSizeChanged;
		}
		private void OnLabelSizeChanged(object sender, SizeChangedEventArgs e) {
			if (!CanRender()) RenderDefault();
			else RenderLabels(false);
		}
		private void UnSubscribeEvents() {
			if (rootContainer != null) rootContainer.SizeChanged -= RootContainerSizeChanged;
			if (scrollViewer != null) scrollViewer.ScrollChanged -= OnScrollViewerScrollChanged;
			if (leftLabel != null) leftLabel.SizeChanged -= OnLabelSizeChanged;
			if (rightLabel != null) rightLabel.SizeChanged -= OnLabelSizeChanged;
		}
		private void OnActualSelectionStartChanged() {
			if (Client.GetComparableValue(Client.SelectionStart) != Client.GetComparableValue(ActualSelectionStart))
				ConstrainSelectionRange(true);
			if (AllowUpdateNormalizedRange)
				NormalizedSelectionStart = RealToNormalized(ActualSelectionStart);
		}
		private bool ConstrainSelectionRange(bool isStart) {
			bool isCorrected = Client.SetSelectionRange(ActualSelectionStart, ActualSelectionEnd, GetViewportSize(), AllowSnapToInterval);
			if (isCorrected) {
				if (isStart)
					ActualSelectionStart = Client.SelectionStart;
				else
					ActualSelectionEnd = Client.SelectionEnd;
			}
			return isCorrected;
		}
		private void OnAnimationCompleted(object sender, AnimationEventArgs e) {
			animateLabelsLocker.Unlock();
			if (e.AnimationType == AnimationTypes.Zoom) {
				PostWithDelay(SetSelectionRangeByActual);
				PostWithDelay(SetVisibleRangeByActual);
				SyncronizeHorizontalOffset();
			}
			else if (e.AnimationType == AnimationTypes.Label) {
				ContentPresenter label = e.Target as ContentPresenter;
				double left = label == leftLabel ? LeftLabelLeft : RightLabelLeft;
				label.SetValue(Canvas.LeftProperty, left);
			}
		}
		private void OnActualVisibleStartChanged() {
			if (Client.GetComparableValue(Client.VisibleStart) != Client.GetComparableValue(ActualVisibleStart))
				ConstrainVisibleRange(true);
			OnActualVisibleRangeChanged();
		}
		private void OnActualSelectionEndChanged() {
			if (Client.GetComparableValue(Client.SelectionEnd) != Client.GetComparableValue(ActualSelectionEnd))
				ConstrainSelectionRange(false);
			if (AllowUpdateNormalizedRange)
				NormalizedSelectionEnd = RealToNormalized(ActualSelectionEnd);
		}
		private void OnActualVisibleEndChanged() {
			if (Client.GetComparableValue(Client.VisibleEnd) != Client.GetComparableValue(ActualVisibleEnd))
				ConstrainVisibleRange(false);
			OnActualVisibleRangeChanged();
		}
		private double GetNormalizedVisibleStart() {
			return RealToNormalized(ActualVisibleStart);
		}
		private double GetNormalizedVisibleEnd() {
			return RealToNormalized(ActualVisibleEnd);
		}
		private void OnNormalizedRangeChanged() {
			IsNormalizedRangeSnappedToCenter = false;
			if (!renderLocker.IsLocked)
				InvalidateRender(CanStartAnimation());
		}
		private object SnapToCenter(object value, bool isRight) {
			object snappedValue = GetSnappedValue(value);
			return snappedValue != null && !snappedValue.Equals(value) ? Client.GetSnappedValue(value, isRight) : snappedValue;
		}
		private void StartEndChanged() {
			if (Client != null) {
				if (!renderLocker.IsLocked) {
					SetClientRange(RangeStart, RangeEnd);
					object visibleStart = ActualVisibleStart != null ? ActualVisibleStart : Client.Start;
					object visibleEnd = ActualVisibleEnd != null ? ActualVisibleEnd : Client.End;
					SetVisibleRange(visibleStart, visibleEnd);
					SetNormalizedRangeInternal(RealToNormalized(ActualSelectionStart), RealToNormalized(ActualSelectionEnd));
				}
				Invalidate();
			}
		}
		Size nativeSize = new Size();
		private void RootContainerSizeChanged(object sender, SizeChangedEventArgs e) {
			UpdateClientPanelWidth(e.NewSize);
			bool zeroWidth = e.NewSize.Width == 0;
			if (zeroWidth)
				shouldLockUpdate = zeroWidth;
			else if (shouldLockUpdate) {
				shouldLockUpdate = false;
				if (!IsRangeControlInitialized)
					Initialize();
				else {
					SyncronizeWithClient(RangeStart, RangeEnd);
					Invalidate();
				}
			}
		}
		private void UpdateClientPanelWidth(Size newSize) {
			nativeSize = newSize;
			if (clientPanel != null) {
				SetClientPanelWidth();
				Invalidate();
			}
		}
		private void RangeControlLoaded(object sender, RoutedEventArgs e) {
			UpdateVisibility();
			Invalidate();
		}
		private bool ConstrainVisibleRange(bool isStart) {
			Size viewport = GetViewportSize();
			if (viewport.Width == 0)
				return false;
			bool result = Client.SetVisibleRange(ActualVisibleStart, ActualVisibleEnd, viewport);
			if (isStart)
				ActualVisibleStart = Client.VisibleStart;
			else
				ActualVisibleEnd = Client.VisibleEnd;
			return result;
		}
		internal double RealToNormalized(object value) {
			if (value == null) return 0;
			return ConstrainNormalizedValue((Client.GetComparableValue(value) - Client.GetComparableValue(RangeStart)) / GetComparableLength());
		}
		private void PerformPostpone() {
			while (postponeQueue.Count != 0) {
				Action action = postponeQueue.Dequeue();
				action.Invoke();
			}
		}
		bool isSyncronizeHorizontalOffset = false;
		private void SetClientPanelWidth() {
			if (clientPanel != null) {
				double oldWidth = clientPanel.Width;
				double newWidth = nativeSize.Width * ScaleFactor;
				clientPanel.Width = newWidth;
				if (!double.IsNaN(oldWidth) && !oldWidth.Round().AreClose(clientPanel.Width.Round()))
					isSyncronizeHorizontalOffset = true;
				if (!IsRangeControlInitialized) return;
				content.Width = newWidth;
				content.Height = clientPanel.ActualHeight;
				interactionArea.Width = newWidth;
				interactionArea.Height = clientPanel.ActualHeight;
			}
		}
		private double ConstrainNormalizedValue(double value) {
			if (double.IsNaN(value)) return 0;
			return Math.Max(Math.Min(value, 1), 0);
		}
		private void OnClientLayoutChanged(object sender, LayoutChangedEventArgs e) {
			if (e == null || !IsViewPortValid()) return;
			if (!IsRangeControlInitialized)
				Initialize(e.Start, e.End);
			else {
				PerformPostpone();
				if (e.ChangeType == LayoutChangedType.Data)
					SyncronizeWithClient(e.Start, e.End, true);
				else
					SyncronizeVisibleRange();
				InvalidateClient();
			}
		}
		private void InvalidateClient() {
			Client.Invalidate(GetViewportSize());
		}
		private void SyncronizeVisibleRange() {
			if (!IsInteraction)
				SetVisibleRange(Client.VisibleStart, Client.VisibleEnd);
			Invalidate();
		}
		private void SyncronizeWithClient(object newStart, object newEnd, bool resetSelection = false) {
			object start = IsStartManualBehavior ? RangeStart : newStart;
			object end = IsEndManualBehavior ? RangeEnd : newEnd;
			SetStartEndInternal(start, end, IsStartManualBehavior, IsEndManualBehavior);
			SetClientRange(RangeStart, RangeEnd);
			UpdateRanges();
			SyncronizeHorizontalOffset();
			if (resetSelection)
				SetSelectionRangeByActual();
			Invalidate();
		}
		private void UpdateRanges() {
			if (ResetRangesOnClientItemsSourceChanged) {
				SetActualSelectionRangeInternal(Client.Start, Client.End, true);
				SetNormalizedRangeInternal(RealToNormalized(Client.Start), RealToNormalized(Client.End));
				SetVisibleRange(Client.Start, Client.End);
			}
			else {
				SetActualSelectionRangeInternal(Client.SelectionStart, Client.SelectionEnd, true);
				SetNormalizedRangeInternal(RealToNormalized(Client.SelectionStart), RealToNormalized(Client.SelectionEnd));
				SetVisibleRange(Client.VisibleStart, Client.VisibleEnd);
			}
		}
		private void SetClientRange(object start, object end) {
			if (Client.SetRange(start, end, GetViewportSize()))
				SetStartEndInternal(Client.Start, Client.End, IsStartManualBehavior, IsEndManualBehavior);
		}
		private void SetStartEndInternal(object start, object end, bool isStartManual, bool isEndManual) {
			renderLocker.DoLockedAction(() => {
				SetRangeStart(start);
				SetRangeEnd(end);
				IsStartManualBehavior = isStartManual;
				IsEndManualBehavior = isEndManual;
			});
		}
		private void SetRangeEnd(object end) {
			SetCurrentValue(RangeEndProperty, end);
		}
		private void SetRangeStart(object start) {
			SetCurrentValue(RangeStartProperty, start);
		}
		GrayScaleEffect ShaderEffect { get; set; }
		private void InitializeElements() {
			if (clientPanel != null && clientPanel.Children.Count > 0 && layoutPanel != null) {
				leftSelectionThumb = LayoutHelper.FindElementByName(layoutPanel, "PART_SelectionLeftThumb") as Thumb;
				rightSelectionThumb = LayoutHelper.FindElementByName(layoutPanel, "PART_SelectionRightThumb") as Thumb;
				draggedThumb = leftSelectionThumb;
				fixedThumb = rightSelectionThumb;
				leftSide = LayoutHelper.FindElementByName(layoutPanel, "PART_LeftSideBorder") as Border;
				rightSide = LayoutHelper.FindElementByName(layoutPanel, "PART_RightSideBorder") as Border;
				content = LayoutHelper.FindElementByName(clientPanel, "PART_Content") as ContentPresenter;
				InitLabels();
				InitNavigationButtons();
			}
		}
		private void InitNavigationButtons() {
			navigationButtonsContainer = LayoutHelper.FindElementByName(rootContainer, "PART_NavigationButtonsContainer") as Grid;
			leftNavigationButton = LayoutHelper.FindElementByName(layoutPanel, "PART_LeftNavigationButton") as Button;
			rightNavigationButton = LayoutHelper.FindElementByName(layoutPanel, "PART_RightNavigationButton") as Button;
			leftNavigationButton.Click += OnLeftNavigationButtonClick;
			rightNavigationButton.Click += OnRightNavigationButtonClick;
			UpdateNavigationButtonsVisibility();
		}
		void OnRightNavigationButtonClick(object sender, RoutedEventArgs e) {
			Controller.StopInetria();
			double offset =
				IsSelectionRangeGreaterVisibleRange() ? CalcSelectionEndPosition() - GetViewportSize().Width + MinNavigationOffset : CalcSelectionCenterPosition() - GetViewportSize().Width / 2;
			HideNavigationButton(rightNavigationButton);
			AnimateScroll(offset);
		}
		private double CalcSelectionEndPosition() {
			return NormalizedSelectionEnd * Client.ClientBounds.Width;
		}
		void OnLeftNavigationButtonClick(object sender, RoutedEventArgs e) {
			Controller.StopInetria();
			double offset = IsSelectionRangeGreaterVisibleRange() ? CalcSelectionStartPosition() - MinNavigationOffset : CalcSelectionCenterPosition() - GetViewportSize().Width / 2;
			HideNavigationButton(leftNavigationButton);
			AnimateScroll(offset);
		}
		private void HideNavigationButton(Button button) {
			button.Visibility = System.Windows.Visibility.Collapsed;
		}
		private void AnimateScroll(double offset) {
			if (EnableAnimation)
				Animator.AnimateScroll(scrollViewer.HorizontalOffset, offset, (o) => {
					scrollViewer.ScrollToHorizontalOffset(o);
					Controller.Reset();
				});
			else {
				scrollViewer.ScrollToHorizontalOffset(offset);
				Controller.Reset();
			}
		}
		private double CalcSelectionCenterPosition() {
			return GetCenterSelection() * Client.ClientBounds.Width;
		}
		private double CalcSelectionStartPosition() {
			return NormalizedSelectionStart * Client.ClientBounds.Width;
		}
		private double GetCenterSelection() {
			return NormalizedSelectionStart + GetSelectionRange() / 2;
		}
		private bool IsSelectionRangeGreaterVisibleRange() {
			return GetSelectionRange() > GetActualVisibleRange();
		}
		private double GetSelectionRange() {
			return NormalizedSelectionEnd - NormalizedSelectionStart;
		}
		private void InitLabels() {
			leftLabel = LayoutHelper.FindElementByName(rootContainer, "PART_LeftLabel") as ContentPresenter;
			rightLabel = LayoutHelper.FindElementByName(rootContainer, "PART_RightLabel") as ContentPresenter;
			UpdateLabelsVisibility();
		}
		private void SetUpShaderEffect() {
			if (Client != null && scrollViewer != null) {
				if (ShaderEffect == null)
					ShaderEffect = CreateShader();
				scrollViewer.Effect = ShaderEffect;
				ShaderEffect.IsEnable = CanGrayscale();
			}
		}
		private bool CanGrayscale() {
			return Client.GrayOutNonSelectedRange && ShadingMode != ShadingModes.Shading;
		}
		private GrayScaleEffect CreateShader() {
			return new GrayScaleEffect() { RFactor = 0.299, GFactor = 0.587, BFactor = 0.114 };
		}
		private void SyncronizeHorizontalOffset() {
			if (scrollViewer == null)
				return;
			double offset = GetNormalizedVisibleStart() * scrollViewer.ExtentWidth;
			scrollViewer.ScrollToHorizontalOffset(offset);
		}
		private void InvalidateRender(bool isAnimate) {
			if (!CanRender()) RenderDefault();
			else Render(isAnimate);
		}
		private void Render(bool isAnimate) {
			if (!renderLocker.IsLocked) {
				RenderLeftSelectionSide(Client.ClientBounds, isAnimate);
				RenderRightSelectionSide(Client.ClientBounds, isAnimate);
				layoutPanel.ClipToBounds();
				RenderSelectionRectangle(Client.ClientBounds);
				RenderLabels(isAnimate);
				RenderNavigationButtons();
				RenderShader();
				RenderRangeBar();
			}
		}
		private void RenderNavigationButtons() {
			if (!IsAutoScrollInProcess) UpdateNavigationButtonsVisibility();
			navigationButtonsContainer.SetValue(Canvas.TopProperty, Client.ClientBounds.Top);
			navigationButtonsContainer.Height = Client.ClientBounds.Height;
			navigationButtonsContainer.Width = layoutPanel.ActualWidth;
		}
		bool IsLastLeftThumbOutOfBounds { get; set; }
		bool IsLastRightThumbOutOfBounds { get; set; }
		double LeftLabelLeft { get; set; }
		double RightLabelLeft { get; set; }
		const double MinLabelsOffset = 1;
		private void RenderLabels(bool isAnimate) {
			if (IsLabelsVisible()) {
				SetLabelsText();
				Thickness leftMargin = leftLabel.Margin;
				Thickness rightMargin = rightLabel.Margin;
				bool isLeftThumbOutOfBounds;
				bool isRightThumbOutOfBounds;
				RenderLabelsCore(isAnimate, out isLeftThumbOutOfBounds, out isRightThumbOutOfBounds);
				IsLastLeftThumbOutOfBounds = isLeftThumbOutOfBounds;
				IsLastRightThumbOutOfBounds = isRightThumbOutOfBounds;
			}
		}
		private void RenderLabelsCore(bool isAnimate, out bool isLeftThumbOutOfBounds, out bool isRightThumbOutOfBounds) {
			double thumbWidth = leftSelectionThumb.ActualWidth;
			Rect bounds = Client.ClientBounds;
			double calcLeftLabelLeft = CalcLeftThumbPosition(bounds, thumbWidth) + thumbWidth / 2;
			double calcRightLabelLeft = CalcRightThumbPosition(bounds, thumbWidth) + thumbWidth / 2 - rightLabel.DesiredSize.Width;
			LeftLabelLeft = calcLeftLabelLeft - leftLabel.DesiredSize.Width;
			RightLabelLeft = calcRightLabelLeft + rightLabel.DesiredSize.Width;
			isLeftThumbOutOfBounds = Client.GetComparableValue(VisibleRangeStart) == Client.GetComparableValue(RangeStart) && LeftLabelLeft < 0;
			isRightThumbOutOfBounds = Client.GetComparableValue(VisibleRangeEnd) == Client.GetComparableValue(RangeEnd) && (RightLabelLeft + rightLabel.ActualWidth > this.ActualWidth);
			leftLabel.Height = rightLabel.Height = Client.ClientBounds.Height;
			leftLabel.SetValue(Canvas.TopProperty, Client.ClientBounds.Top);
			rightLabel.SetValue(Canvas.TopProperty, Client.ClientBounds.Top);
			if (isLeftThumbOutOfBounds && !animateLabelsLocker.IsLocked) LeftLabelLeft = LeftLabelLeft + leftLabel.DesiredSize.Width;
			if (isRightThumbOutOfBounds && !animateLabelsLocker.IsLocked) RightLabelLeft = RightLabelLeft - rightLabel.DesiredSize.Width;
			if (CanLabelIntersects(LeftLabelLeft, RightLabelLeft)) {
				if (IsDoubleEquals(LeftLabelLeft, calcLeftLabelLeft)) {
					isLeftThumbOutOfBounds = false;
					LeftLabelLeft = LeftLabelLeft - leftLabel.DesiredSize.Width;
				}
				if (IsDoubleEquals(RightLabelLeft, calcRightLabelLeft)) {
					isRightThumbOutOfBounds = false;
					RightLabelLeft = RightLabelLeft + rightLabel.DesiredSize.Width;
				}
			}
			if (isAnimate && !animateLabelsLocker.IsLocked) {
				Animator.AnimateLabel(leftLabel, ConstrainByNaN(LeftLabelLeft));
				Animator.AnimateLabel(rightLabel, ConstrainByNaN(RightLabelLeft));
			}
			else {
				bool needAnimate = (isLeftThumbOutOfBounds != IsLastLeftThumbOutOfBounds) && !animateLabelsLocker.IsLocked;
				if (needAnimate) Animator.AnimateLabel(leftLabel, ConstrainByNaN(LeftLabelLeft));
				else leftLabel.SetValue(Canvas.LeftProperty, LeftLabelLeft);
				needAnimate = (isRightThumbOutOfBounds != IsLastRightThumbOutOfBounds) && !animateLabelsLocker.IsLocked;
				if (needAnimate) Animator.AnimateLabel(rightLabel, ConstrainByNaN(RightLabelLeft));
				else rightLabel.SetValue(Canvas.LeftProperty, RightLabelLeft);
			}
		}
		private bool IsDoubleEquals(double value1, double value2) {
			int digits = 8;
			return Math.Round(value1, digits) == Math.Round(value2, digits);
		}
		private void SetLabelsText() {
			leftLabel.Content = FormatText(ActualSelectionStart);
			rightLabel.Content = FormatText(ActualSelectionEnd);
		}
		private object FormatText(object value) {
			return Client != null ? Client.FormatText(value) : string.Empty;
		}
		private bool CanLabelIntersects(double leftLabelLeft, double rightLabelLeft) {
			Thickness margin = leftLabel.Margin;
			return leftLabelLeft + TransformHelper.GetElementWidth(leftLabel) + margin.Left + margin.Right + MinLabelsOffset > rightLabelLeft;
		}
		private void RenderShader() {
			if (ShaderEffect != null)
				ShaderEffect.Invalidate(CalcShaderBounds());
		}
		private double[] CalcShaderBounds() {
			Rect bounds = Client.ClientBounds;
			double left = (NormalizedSelectionStart - GetNormalizedVisibleStart()) * bounds.Width / rootContainer.ActualWidth;
			double right = (NormalizedSelectionEnd - GetNormalizedVisibleStart()) * bounds.Width / rootContainer.ActualWidth;
			double top = bounds.Top / clientPanel.ActualHeight;
			double bottom = bounds.Bottom / clientPanel.ActualHeight;
			return new double[] { ConstrainNormalizedValue(left), ConstrainNormalizedValue(top), ConstrainNormalizedValue(right), ConstrainNormalizedValue(bottom) };
		}
		private void RenderSelectionRectangle(Rect bounds) {
			if (IsSelectionRectangleVisible()) {
				UpdateSelectionRectangleVisibility();
				SetSelectionRectangleContainerClipping();
				Rect selectionBounds = CalcSelectionRectangleBounds(bounds);
				selectionRectangle.SetValue(Canvas.LeftProperty, selectionBounds.Left);
				selectionRectangle.SetValue(Canvas.TopProperty, selectionBounds.Top);
				selectionRectangle.Height = selectionBounds.Height;
				selectionRectangle.Width = selectionBounds.Width;
			}
		}
		private bool IsSelectionRectangleVisible() {
			return IsSelectionMoving && ((ShowSelectionRectangle == null && !Client.AllowThumbs) || (ShowSelectionRectangle != null && ShowSelectionRectangle.Value));
		}
		private void UpdateSelectionRectangleVisibility() {
			selectionRectangle.Visibility = IsSelectionRectangleVisible() ? Visibility.Visible : Visibility.Collapsed;
		}
		private Rect CalcSelectionRectangleBounds(Rect bounds) {
			double left = ((NormalizedSelectionStart - GetNormalizedVisibleStart()) * bounds.Width);
			double right = ((NormalizedSelectionEnd - GetNormalizedVisibleStart()) * bounds.Width);
			return new Rect(left, bounds.Top, right - left, bounds.Height);
		}
		private void SetSelectionRectangleContainerClipping() {
			if (selectionRactangleContainer != null) {
				Rect clipping =
					new Rect(new Point(), new Size(selectionRactangleContainer.ActualWidth, selectionRactangleContainer.ActualHeight));
				selectionRactangleContainer.Clip = new RectangleGeometry() { Rect = clipping };
			}
		}
		private bool CanRender() {
			return IsRangeControlInitialized && rangeBar != null && CanRenderLeftSide() && CanRenderRightSide() && IsClientBoundsCorrect();
		}
		private bool IsClientBoundsCorrect() {
			return Client != null && Client.ClientBounds.Width > 0;
		}
		private void Invalidate() {
			InvalidateRender(false);
		}
		private void RenderDefault() {
			if (leftSide != null && leftSelectionThumb != null && rightSelectionThumb != null)
				RenderSelectionDefault();
		}
		private void RenderSelectionDefault() {
			leftSide.Width = nativeSize.Width;
			leftSide.Height = clientPanel.ActualHeight;
			leftSide.SetValue(Canvas.LeftProperty, 0d);
			leftLabel.SetValue(Canvas.LeftProperty, 0d);
			rightLabel.SetValue(Canvas.LeftProperty, 0d);
			leftLabel.Height = rightLabel.Height = rightSelectionThumb.Height = leftSelectionThumb.Height = 0;
		}
		private void RenderRangeBar() {
			if (rangeBar != null) {
				rangeBar.ViewPortStart = GetNormalizedVisibleStart();
				rangeBar.ViewPortEnd = GetNormalizedVisibleEnd();
				rangeBar.Minimum = IsNormalizedRangeSnappedToCenter ? RealToNormalized(ActualSelectionStart) : NormalizedSelectionStart;
				rangeBar.Maximum = IsNormalizedRangeSnappedToCenter ? RealToNormalized(ActualSelectionEnd) : NormalizedSelectionEnd;
				if (ShowRangeBar)
					rangeBar.Invalidate();
			}
		}
		private void OnSelectionResizingStarted() {
			AllowUpdateNormalizedRange = false;
			prevPosition = Double.NaN;
			PatchActualSelectionRange();
		}
		private void PatchActualSelectionRange() {
			object start = NormalizedSelectionStart.AreClose(RealToNormalized(ActualSelectionStart)) ? ActualSelectionStart : NormalizedToReal(NormalizedSelectionStart);
			object end = NormalizedSelectionEnd.AreClose(RealToNormalized(ActualSelectionEnd)) ? ActualSelectionEnd : NormalizedToReal(NormalizedSelectionEnd);
			SetActualSelectionRangeInternal(start, end);
		}
		private double ConstrainPosition(double currentPosition) {
			if (Math.Sign(currentPosition) == -1) {
				currentPosition = 0;
			}
			else if (currentPosition.Round() > ClientWidth) {
				currentPosition = ClientWidth;
			}
			return currentPosition;
		}
		internal double NormalizeByLength(double value) {
			double width = TransformHelper.GetElementWidth(clientPanel);
			if (width == 0 || GetActualVisibleRange() <= 0) return 0;
			return value / width;
		}
		DispatcherTimer autoScrollTimer;
		private void ChangeActualSelection(double delta, bool isSelectionTypeChanged) {
			if (SelectionType == SelectionChangesType.Start) {
				double newStart = NormalizedSelectionStart + delta;
				SetNormalizedStartWithLock(isSelectionTypeChanged, ConstrainNormalizedValue(newStart));
				Render(false);
			}
			else {
				double newEnd = NormalizedSelectionEnd + delta;
				SetNormalizedEndWithLock(isSelectionTypeChanged, ConstrainNormalizedValue(newEnd));
				Render(false);
			}
		}
		private void StartAutoScroll(Action action) {
			if (autoScrollTimer != null) return;
			autoScrollTimer = new DispatcherTimer(DispatcherPriority.Render) { Interval = TimeSpan.FromMilliseconds(50) };
			autoScrollTimer.Tick += (t, e) => {
				action.Invoke();
			};
			autoScrollTimer.Start();
		}
		private void ResetAutoScroll() {
			if (autoScrollTimer != null) {
				IsAutoScrollInProcess = false;
				UpdateNavigationButtonsVisibility();
				autoScrollTimer.Stop();
				autoScrollTimer = null;
			}
		}
		private void ScrollToActualVisibleStart() {
			double scrollDelta = -AutoScrollDelta;
			double normalizedValue = Math.Max(RealToNormalized(ActualVisibleStart) + scrollDelta, 0);
			ProccesAutoScroll(scrollDelta, normalizedValue);
		}
		private void ProccesAutoScroll(double scrollDelta, double normalizedValue) {
			bool isSelectionTypeChanged;
			ProcessScroll(scrollDelta, out isSelectionTypeChanged);
			if (SelectionType == SelectionChangesType.Start) SetNormalizedStartWithLock(isSelectionTypeChanged, normalizedValue);
			else SetNormalizedEndWithLock(isSelectionTypeChanged, normalizedValue);
		}
		private void ProcessScroll(double scrollDelta, out bool isSelectionTypeChanged) {
			isSelectionTypeChanged = false;
			double offset = scrollViewer.HorizontalOffset + (scrollDelta * scrollViewer.ExtentWidth);
			if (offset < 0 || offset > scrollViewer.ExtentWidth) {
				autoScrollTimer.Stop();
				autoScrollTimer = null;
			}
			else {
				isSelectionTypeChanged = IsSelectionTypeChanged(Mouse.GetPosition(clientPanel).X);
				scrollViewer.ScrollToHorizontalOffset(offset);
			}
		}
		private void ProcessScroll(double scrollDelta) {
			bool isCorrectValue;
			ProcessScroll(scrollDelta, out isCorrectValue);
		}
		private void ScrollToActualVisibleEnd() {
			double scrollDelta = AutoScrollDelta;
			double normalizedValue = Math.Max(RealToNormalized(ActualVisibleEnd) + scrollDelta, 0);
			ProccesAutoScroll(scrollDelta, normalizedValue);
		}
		private void SetNormalizedStartWithLock(bool isSelectionTypeChanged, double newStart) {
			renderLocker.DoLockedAction(() => {
				if (isSelectionTypeChanged) {
					NormalizedSelectionEnd = RealToNormalized(ActualSelectionStart);
					ActualSelectionEnd = ActualSelectionStart;
					SetSelectionEndByActual();
					ReassignThumbs();
				}
				NormalizedSelectionStart = newStart <= NormalizedSelectionEnd ? newStart : NormalizedSelectionEnd;
				SetActualSelectionStart();
			});
		}
		private void ReassignThumbs() {
			Thumb temp = draggedThumb;
			draggedThumb = fixedThumb;
			fixedThumb = temp;
		}
		private void SetNormalizedEndWithLock(bool isSelectionTypeChanged, double newEnd) {
			renderLocker.DoLockedAction(() => {
				if (isSelectionTypeChanged) {
					NormalizedSelectionStart = RealToNormalized(ActualSelectionEnd);
					ActualSelectionStart = ActualSelectionEnd;
					SetSelectionStartByActual();
					ReassignThumbs();
				}
				NormalizedSelectionEnd = newEnd >= NormalizedSelectionStart ? newEnd : NormalizedSelectionStart;
				SetActualSelectionEnd();
			});
		}
		private void SetActualSelectionEnd() {
			ActualSelectionEnd = CanSnapToInterval() ? SnapEnd() : NormalizedToReal(NormalizedSelectionEnd);
			SetSelectionEndByActual();
		}
		private void SetActualSelectionStart() {
			ActualSelectionStart = CanSnapToInterval() ? SnapStart() : NormalizedToReal(NormalizedSelectionStart);
			SetSelectionStartByActual();
		}
		private void SetSelectionEndByActual() {
			if (AllowImmediateRangeUpdate)
				SetSelectionEndCore();
		}
		void UpdateSelectedRange() {
		   UpdateSelectedRange(SelectionRangeStart, SelectionRangeEnd);
		}
		private void UpdateSelectedRange(object start, object end) {
			SelectionRange = new EditRange(start, end);
		}
		private void SetSelectionEndCore() {
			UpdateSelectionLocker.DoLockedAction(() => {
				SetCurrentValue(SelectionRangeEndProperty, ActualSelectionEnd);
			});
			if (!updateSelectedRangeLocker.IsLocked) UpdateSelectedRange();
		}
		private void SetSelectionStartByActual() {
			if (AllowImmediateRangeUpdate)
				SetSelectionStartCore();
		}
		internal object NormalizedToReal(double normalizedValue) {
			if (Client == null)
				return null;
			double comparableValue = ConstrainNormalizedValue(normalizedValue) * GetComparableLength() + Client.GetComparableValue(RangeStart);
			return Client.GetRealValue(comparableValue);
		}
		private void OnSelectionResizingCompleted() {
			AllowUpdateNormalizedRange = true;
			if (CanSnapToInterval()) Animator.CanAnimate = true;
			SetNormalizedRangeInternal(RealToNormalized(ActualSelectionStart), RealToNormalized(ActualSelectionEnd));
			InvalidateRender(CanStartAnimation());
			PostWithDelay(SetSelectionRangeByActual);
			Animator.CanAnimate = false;
		}
		private object SnapStart() {
			return GetSnappedValue(NormalizedToReal(NormalizedSelectionStart));
		}
		private object SnapEnd() {
			return GetSnappedValue(NormalizedToReal(NormalizedSelectionEnd));
		}
		private void SetSelectionStartCore() {
			UpdateSelectionLocker.DoLockedAction(() => {
				SetCurrentValue(SelectionRangeStartProperty, ActualSelectionStart);
			});
			if (!updateSelectedRangeLocker.IsLocked) UpdateSelectedRange();
		}
		internal object GetSnappedValue(object value) {
			object leftValue = Client.GetSnappedValue(value, true);
			object rightValue = Client.GetSnappedValue(value, false);
			double position = (Client.GetComparableValue(value) - Client.GetComparableValue(leftValue)) / (Client.GetComparableValue(rightValue) - Client.GetComparableValue(leftValue));
			return position < 0.5 ? leftValue : rightValue;
		}
		private void RenderRightSelectionSide(Rect bounds, bool isAnimate) {
			double rightThumbPosition;
			double rightSidePosition;
			CalcRightSelection(bounds, TransformHelper.GetElementWidth(rightSelectionThumb), out rightThumbPosition, out rightSidePosition);
			rightSelectionThumb.SetValue(Canvas.TopProperty, bounds.Top);
			rightSide.SetValue(Canvas.TopProperty, bounds.Top);
			rightSide.Width = bounds.Width;
			rightSide.Opacity = CanShading();
			rightSide.Height = rightSelectionThumb.Height = SelectionRangeStart != null ? bounds.Height : 0;
			if (!isAnimate) {
				rightSelectionThumb.SetValue(Canvas.LeftProperty, rightThumbPosition);
				rightSide.SetValue(Canvas.LeftProperty, rightSidePosition);
			}
			else
				Animator.AnimateSelection(rightSelectionThumb, rightSide, rightSidePosition, rightThumbPosition, this);
		}
		private void CalcRightSelection(Rect bounds, double thumbWidth, out double rightThumbPosition, out double rightSidePosition) {
			rightThumbPosition = CalcRightThumbPosition(bounds, thumbWidth);
			rightSidePosition = CalcSelectionRight(bounds);
		}
		private double CalcRightThumbPosition(Rect bounds, double thumbWidth) {
			double maxPosition = (bounds.Width - thumbWidth / 2) - Math.Floor(MinVisibleThumbPart / 2) - (GetPixelVisibleStart(bounds));
			double positionCandidate = CalcSelectionRight(bounds) - thumbWidth / 2;
			return ConstrainByNaN(Math.Min(positionCandidate, maxPosition));
		}
		private double GetPixelVisibleStart(Rect bounds) {
			return GetNormalizedVisibleStart() * bounds.Width;
		}
		private double CalcSelectionRight(Rect bounds) {
			return (NormalizedSelectionEnd - GetNormalizedVisibleStart()) * bounds.Width;
		}
		private double ConstrainByNaN(double value) {
			return double.IsNaN(value) ? 0 : value;
		}
		private void RenderLeftSelectionSide(Rect bounds, bool isAnimate) {
			double leftThumbPosition;
			double leftSidePosition;
			CalcLeftSelection(bounds, TransformHelper.GetElementWidth(leftSelectionThumb), out leftThumbPosition, out leftSidePosition);
			leftSelectionThumb.SetValue(Canvas.TopProperty, bounds.Top);
			leftSide.SetValue(Canvas.TopProperty, bounds.Top);
			leftSide.Width = bounds.Width;
			leftSide.Opacity = CanShading();
			leftSelectionThumb.Height = leftSide.Height = SelectionRangeStart != null ? bounds.Height : 0;
			if (!isAnimate) {
				leftSelectionThumb.SetValue(Canvas.LeftProperty, leftThumbPosition);
				leftSide.SetValue(Canvas.LeftProperty, leftSidePosition);
			}
			else {
				Animator.AnimateSelection(leftSelectionThumb, leftSide, leftSidePosition, leftThumbPosition, this);
			}
		}
		private double CanShading() {
			return Client.GrayOutNonSelectedRange && (ShadingMode != ShadingModes.Grayscale) ? 1 : 0;
		}
		private void CalcLeftSelection(Rect bounds, double thumbWidth, out double leftThumbPosition, out double leftSidePosition) {
			leftThumbPosition = CalcLeftThumbPosition(bounds, thumbWidth);
			leftSidePosition = CalcSelectionLeft(bounds) - bounds.Width;
		}
		private double CalcLeftThumbPosition(Rect bounds, double thumbWidth) {
			double minPosition = -(thumbWidth - MinVisibleThumbPart) / 2 - GetPixelVisibleStart(bounds);
			double positionCandidate = CalcSelectionLeft(bounds) - thumbWidth / 2;
			return ConstrainByNaN(Math.Max(positionCandidate, minPosition));
		}
		private double CalcSelectionLeft(Rect bounds) {
			return (NormalizedSelectionStart - GetNormalizedVisibleStart()) * bounds.Width;
		}
		private bool CanRenderRightSide() {
			return clientPanel != null && rightSelectionThumb != null && rightSide != null;
		}
		private bool CanRenderLeftSide() {
			return clientPanel != null && leftSelectionThumb != null && leftSide != null;
		}
		Locker renderLocker = new Locker();
		private void OnScrollViewerScrollChanged(object sender, ScrollChangedEventArgs e) {
			if (scrollViewer.ExtentWidth != 0 && IsRangeControlInitialized) {
				if (isSyncronizeHorizontalOffset) {
					isSyncronizeHorizontalOffset = false;
					SyncronizeHorizontalOffset();
					InvalidateClient();
					Invalidate();
					return;
				}
				if (IsAutoScrollInProcess) UpdateNavigationButtonsVisibility();
				object newVisibleStart = NormalizedToReal(scrollViewer.HorizontalOffset / scrollViewer.ExtentWidth);
				object newVisibleEnd = NormalizedToReal((scrollViewer.HorizontalOffset + scrollViewer.ViewportWidth) / scrollViewer.ExtentWidth);
				if (!newVisibleStart.Equals(ActualVisibleStart)) {
					UpdateVisibleRange(newVisibleStart, newVisibleEnd, false);
					InvalidateClient();
					Invalidate();
				}
			}
		}
		private void SetVisibleRange(object newVisibleStart, object newVisibleEnd) {
			SetActualVisibleRange(newVisibleStart, newVisibleEnd);
			if (!Animator.IsProcessAnimation) {
				SetVisibleRangeByActual();
			}
		}
		private void SetActualVisibleRange(object newVisibleStart, object newVisibleEnd, bool checkBeforeSet = false) {
			if (Client != null && IsViewPortValid() && newVisibleStart != null && newVisibleEnd != null) {
				renderLocker.DoLockedAction(() => {
					IsVisibleRangeCorrected = Client.SetVisibleRange(newVisibleStart, newVisibleEnd, GetViewportSize());
					if (IsVisibleRangeCorrected && checkBeforeSet && !DetectVisibleRangeWidthChange()) {
						StopAnimation();
						return;
					}
					SetActualVisibleRangeAfterCorrection();
				});
			}
		}
		private void StopAnimation() {
			Animator.StopAnimation();
		}
		private void CenterSelectionInsideVisibleRange() {
			double normalizedStart = RealToNormalized(Client.VisibleStart);
			double normalizedEnd = RealToNormalized(Client.VisibleEnd);
			double centerVisible = normalizedStart + (normalizedEnd - normalizedStart) / 2;
			double centerSelection = RealToNormalized(ActualSelectionStart) + (RealToNormalized(ActualSelectionEnd) - RealToNormalized(ActualSelectionStart)) / 2;
			double centerDelta = centerVisible - centerSelection;
			Client.SetVisibleRange(NormalizedToReal(normalizedStart - centerDelta), NormalizedToReal(normalizedEnd - centerDelta), GetViewportSize());
		}
		private bool DetectVisibleRangeWidthChange() {
			double actualVisibleWidth = GetComparableRange(ActualVisibleStart, ActualVisibleEnd);
			double newVisibleWidth = GetComparableRange(Client.VisibleStart, Client.VisibleEnd);
			return !DoubleExtensions.AreClose(actualVisibleWidth, newVisibleWidth);
		}
		private double GetComparableRange(object start, object end) {
			return Client.GetComparableValue(end) - Client.GetComparableValue(start);
		}
		private void SetActualVisibleRangeAfterCorrection() {
			ActualVisibleStart = Client.VisibleStart;
			ActualVisibleEnd = Client.VisibleEnd;
			SetClientPanelWidth();
		}
		private bool IsViewPortValid() {
			Size viewPort = GetViewportSize();
			return viewPort.Width != 0 && viewPort.Height != 0;
		}
		private void SetVisibleRangeByActual() {
			SetVisibleStartByActual();
			SetVisibleEndByActual();
		}
		private void SetVisibleEndByActual() {
			SetCurrentValue(VisibleRangeEndProperty, ActualVisibleEnd);
		}
		private void SetVisibleStartByActual() {
			SetCurrentValue(VisibleRangeStartProperty, ActualVisibleStart);
		}
		private double GetActualVisibleRange() {
			return Math.Min(Math.Max(GetNormalizedVisibleEnd() - GetNormalizedVisibleStart(), 0), 1);
		}
		private double GetComparableLength() {
			return Client.GetComparableValue(RangeEnd) - Client.GetComparableValue(RangeStart);
		}
		private Predicate<FrameworkElement> GetElementByType(Type type) {
			return new Predicate<FrameworkElement>((e) => e.GetType() == type);
		}
		private void Initialize(object start, object end) {
			IsRangeControlInitialized = InitializeStartEnd(start, end);
			if (!IsRangeControlInitialized) return;
			SetUpShaderEffect();
			InitializeVisibleRange();
			InitializeSelection();
			Invalidate();
		}
		private void Initialize() {
			Initialize(null, null);
		}
		private void InitializeSelection() {
			object start = SelectionRangeStart ?? Client.SelectionStart;
			object end = SelectionRangeEnd ?? Client.SelectionEnd;
			start = start ?? RangeStart;
			end = end ?? RangeEnd;
			SetActualSelectionRangeInternal(start, end, true);
			SnapToRealSelection();
		}
		private void SnapToRealSelection() {
			EnqueueAction(new Action(() => {
				if (CanSnapToInterval()) {
					ActualSelectionStart = GetCenterSnappedValue(ActualSelectionStart, false);
					ActualSelectionEnd = GetCenterSnappedValue(ActualSelectionEnd, true);
				}
				SetNormalizedRangeInternal(RealToNormalized(ActualSelectionStart), RealToNormalized(ActualSelectionEnd));
			}));
		}
		private void InitializeVisibleRange() {
			object start = ActualVisibleStart == null ? Client.Start : ActualVisibleStart;
			object end = ActualVisibleEnd == null ? Client.End : ActualVisibleEnd;
			SetActualVisibleRange(start, end);
			if (VisibleRangeStart == null) SetVisibleStartByActual();
			if (VisibleRangeEnd == null) SetVisibleEndByActual();
		}
		private bool InitializeStartEnd(object newStart, object newEnd) {
			object start = RangeStart, end = RangeEnd;
			if (RangeStart == null) {
				start = newStart == null ? Client.Start : newStart;
				IsStartManualBehavior = false;
			}
			if (RangeEnd == null) {
				end = newEnd == null ? Client.End : newEnd;
				IsEndManualBehavior = false;
			}
			SetClientRange(start, end);
			return RangeStart != null && RangeEnd != null;
		}
		Queue<Action> postponeQueue = new Queue<Action>();
		private void EnqueueAction(Action action) {
			postponeQueue.Enqueue(action);
		}
		private Size GetViewportSize() {
			if (nativeSize.Width == 0)
				return new Size();
			return new Size(scrollViewer.ActualWidth, scrollViewer.ActualHeight);
		}
		private void SetActualSelectionRangeInternal(object start, object end, bool syncWithActual = false) {
			renderLocker.DoLockedAction(() => {
				if (Client != null) {
					Client.SetSelectionRange(start, end, GetViewportSize(), AllowSnapToInterval);
					if (Client.SelectionStart == null || Client.SelectionEnd == null) return;
					ActualSelectionStart = Client.SelectionStart;
					ActualSelectionEnd = Client.SelectionEnd;
					if (syncWithActual)
						SetSelectionRangeByActual();
				}
			});
		}
		private bool IsInsideThumbsBounds(Point position) {
			return GetLeftThumbBounds().Contains(position) || GetRightThumbBounds().Contains(position);
		}
		private Rect GetRightThumbBounds() {
			return TransformHelper.GetElementBounds(rightSelectionThumb, clientPanel);
		}
		private Rect GetLeftThumbBounds() {
			return TransformHelper.GetElementBounds(leftSelectionThumb, clientPanel);
		}
		private void UpdateVisibleRange(object newStart, object newEnd, bool isCheckBeforeSet = true) {
			SetActualVisibleRange(newStart, newEnd, isCheckBeforeSet);
			if (AllowImmediateRangeUpdate)
				PostWithDelay(SetVisibleRangeByActual);
		}
		private void SetNormalizedRange(double newStart, double newEnd) {
			SetNormalizedRangeInternal(newStart, newEnd);
			SetActualSelectionStart();
			SetActualSelectionEnd();
			Render(false);
		}
		private void SetNormalizedRangeInternal(double newStart, double newEnd) {
			renderLocker.DoLockedAction(() => {
				NormalizedSelectionStart = newStart;
				NormalizedSelectionEnd = newEnd;
			});
		}
		private void UpdateByControllerState(RangeControlStateType state) {
			switch (state) {
				case RangeControlStateType.MoveSelection:
					OnSelectionRangeChangedByController(true);
					break;
				case RangeControlStateType.Selection:
					OnSelectionResizingCompleted();
					break;
				case RangeControlStateType.Click:
					OnSelectionRangeChangedByController(true);
					break;
				case RangeControlStateType.Zoom:
					UpdateVisibleOnControllerReset();
					break;
				case RangeControlStateType.Scrolling:
					UpdateVisibleOnControllerReset();
					break;
				case RangeControlStateType.ThumbDragging:
					OnSelectionResizingCompleted();
					break;
			}
		}
		private void UpdateVisibleOnControllerReset() {
			IsInteraction = true;
			PostWithDelay(new Action(() => {
				IsInteraction = false;
				SetVisibleRangeByActual();
			}));
		}
		private void OnSelectionRangeChangedByController(bool isAnimate) {
			AllowUpdateNormalizedRange = true;
			Animator.CanAnimate = isAnimate;
			if (IsSelectionRectangleVisible()) UpdateThumbVisibility();
			IsSelectionMoving = false;
			UpdateSelectionRectangleVisibility();
			SetNormalizedRangeInternal(RealToNormalized(ActualSelectionStart), RealToNormalized(ActualSelectionEnd));
			InvalidateRender(CanStartAnimation());
			PostWithDelay(SetSelectionRangeByActual);
			Animator.CanAnimate = false;
		}
		private void PostWithDelay(Action action) {
			DispatcherTimer postTimer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(GetDelay()) };
			postTimer.Tick += (s, e) => {
				((DispatcherTimer)s).Stop();
				if (StopPosting) {
					StopPosting = false;
					return;
				}
				action.Invoke();
			};
			postTimer.Start();
		}
		private double GetDelay() {
			return AllowImmediateRangeUpdate ? 0 : UpdateDelay;
		}
		private void SetRangeThumbsVisible() {
			leftSelectionThumb.Opacity = 1;
			rightSelectionThumb.Opacity = 1;
		}
		private bool CanStartAnimation() {
			return EnableAnimation && Animator.CanAnimate;
		}
		Locker updateSelectedRangeLocker = new Locker();
		private void SetSelectionRangeByActual() {
			updateSelectedRangeLocker.DoLockedAction(() => {
				SetSelectionStartCore();
				SetSelectionEndCore();
			});
			UpdateSelectedRange();
		}
		private void GoToMoveSelectionState() {
			IsSelectionMoving = true;
			if (IsSelectionRectangleVisible()) CollapseRangeThumbs();
		}
		private void CollapseRangeThumbs() {
			leftSelectionThumb.Opacity = 0;
			rightSelectionThumb.Opacity = 0;
		}
		private void ConstrainVisibleRange(ref double visibleStart, ref double visibleEnd) {
			if (visibleEnd > 1d) {
				visibleStart = Math.Max(visibleStart - Math.Abs(1d - visibleEnd), 0d);
				visibleEnd = 1d;
			}
			else if (visibleStart < 0d) {
				visibleEnd = Math.Min(visibleEnd + Math.Abs(visibleStart), 1d);
				visibleStart = 0d;
			}
		}
		private void SnapNormalizedRangeToCenter() {
			if (CanSnapToInterval()) {
				double start = RealToNormalized(GetCenterSnappedValue(ActualSelectionStart, false));
				double end = RealToNormalized(GetCenterSnappedValue(ActualSelectionEnd, true));
				if (start > end) start = end;
				SetNormalizedRangeInternal(start, end);
				IsNormalizedRangeSnappedToCenter = true;
			}
		}
		double lastScaleFactorSign = 0;
		private bool ProcessZoom(double position, double factor) {
			if (!AllowZoom || (!HasVisibleRangeWidthChange && Math.Sign(factor - 1) == lastScaleFactorSign)) return false;
			lastScaleFactorSign = Math.Sign(factor - 1);
			double center = NormalizeByLength(position);
			double newVisibleRange = GetActualVisibleRange() * factor;
			double newStart = center - (((center - GetNormalizedVisibleStart()) / GetActualVisibleRange()) * newVisibleRange);
			double newEnd = center + (((GetNormalizedVisibleEnd() - center) / GetActualVisibleRange()) * newVisibleRange);
			ConstrainVisibleRange(ref newStart, ref newEnd);
			LastVisibleRangeWidth = GetActualVisibleRange();
			if (DoubleExtensions.AreClose((newEnd - newStart), LastVisibleRangeWidth)) return false;
			UpdateVisibleRange(NormalizedToReal(newStart), NormalizedToReal(newEnd));
			return true;
		}
		internal bool IsOutsideClientBounds(Point point) {
			if (Client == null)
				return false;
			return !Client.ClientBounds.Contains(point);
		}
		internal bool IsInsideSelectionArea(Point position) {
			double startSelectionPosition = NormalizedSelectionStart * ClientWidth;
			double endSelectionPosition = NormalizedSelectionEnd * ClientWidth;
			return Client.ClientBounds.Contains(position) && position.X >= startSelectionPosition && position.X <= endSelectionPosition;
		}
		internal RangeControlHitTestType HitTest(Point position) {
			if (Client == null) return RangeControlHitTestType.None;
			CurrentHitTest = Client.HitTest(position);
			if (CurrentHitTest.RegionType == RangeControlClientRegionType.ItemInterval) return HitTestInsideClientBounds(position);
			else return RangeControlHitTestType.LabelArea;
		}
		private RangeControlHitTestType HitTestInsideClientBounds(Point position) {
			if (IsInsideThumbsBounds(position)) return RangeControlHitTestType.ThumbsArea;
			if (IsInsideSelectionArea(position)) return RangeControlHitTestType.SelectionArea;
			else return RangeControlHitTestType.ScrollableArea;
		}
		internal void OnRangeBarSliderMoved(double delta) {
			if (scrollViewer != null && AllowScroll) {
				scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset + delta * scrollViewer.ExtentWidth);
			}
		}
		internal void OnRangeBarViewPortResizing(double newStart, double newEnd, bool changeOffset) {
			if (scrollViewer != null && AllowZoom) {
				UpdateVisibleRange(NormalizedToReal(newStart), NormalizedToReal(newEnd));
				PostponeSnapNormalizedRangeToCenter();
			}
		}
		internal void SelectByHitTest() {
			if (Client == null) return;
			Animator.CanAnimate = Client.AllowThumbs;
			SetActualSelectionRangeInternal(CurrentHitTest.RegionStart, CurrentHitTest.RegionEnd);
			SetNormalizedRangeInternal(RealToNormalized(ActualSelectionStart), RealToNormalized(ActualSelectionEnd));
		}
		internal void MoveSelection(double pixelDelta) {
			if (Client == null) return;
			double newStart;
			double newEnd;
			CalcSelectionRangeOnMove(pixelDelta, out newStart, out newEnd);
			SetNormalizedRange(newStart, newEnd);
		}
		private void CalcSelectionRangeOnMove(double pixelDelta, out double newStart, out double newEnd) {
			double normalizedDelta = NormalizeByLength(pixelDelta.Round());
			newStart = NormalizedSelectionStart + normalizedDelta;
			newEnd = NormalizedSelectionEnd + normalizedDelta;
			ConstrainSelectionRange(ref newStart, ref newEnd, normalizedDelta);
			if (ConstrainSelectionRange(ref newStart, ref newEnd, normalizedDelta))
				Controller.StopInetria();
		}
		private bool ConstrainSelectionRange(ref double newStart, ref double newEnd, double normalizedDelta) {
			if (newStart < 0) {
				newEnd = NormalizedSelectionEnd + Math.Sign(normalizedDelta) * NormalizedSelectionStart;
				newStart = 0;
				return true;
			}
			else if (newEnd > 1) {
				newStart = NormalizedSelectionStart + Math.Sign(normalizedDelta) * (1 - NormalizedSelectionEnd);
				newEnd = 1;
				return true;
			}
			return false;
		}
		private void ProccesMoveCore(double scrollDelta, double start, double end) {
			ProcessScroll(scrollDelta);
			SetNormalizedRangeInternal(start, end);
			SetActualSelectionStart();
			SetActualSelectionEnd();
		}
		private void MoveToActualVisibleEnd() {
			double scrollDelta = AutoScrollDelta;
			ProcessMove(scrollDelta);
		}
		private void MoveToActualVisibleStart() {
			double scrollDelta = -AutoScrollDelta;
			ProcessMove(scrollDelta);
		}
		private void ProcessMove(double scrollDelta) {
			if (NormalizedSelectionStart == 0 || NormalizedSelectionEnd == 1) return;
			double start = NormalizedSelectionStart + scrollDelta;
			double end = NormalizedSelectionEnd + scrollDelta;
			ConstrainSelectionRange(ref start, ref end, scrollDelta);
			ProccesMoveCore(scrollDelta, start, end);
		}
		internal void ResizeSelection(double currentPosition, bool isTouch = false) {
			if (isTouch)
				currentPosition = RealToPixel(ActualVisibleStart) + currentPosition;
			if (Client == null) return;
			StopAnimation();
			Invalidate();
			if (!isResizingDetected) {
				FindDraggedAndFixedThumbs(currentPosition);
				SelectionType = draggedThumb.Equals(leftSelectionThumb) ? SelectionChangesType.Start : SelectionChangesType.End;
			}
			else {
				ProcessSelectionResizing(currentPosition);
			}
		}
		private double RealToPixel(object realValue) {
			double normalValue = RealToNormalized(realValue);
			return normalValue * ClientWidth;
		}
		private void FindDraggedAndFixedThumbs(double currentPosition) {
			double normalPosition = RealToNormalized(this.PixelToReal(currentPosition));
			if (normalPosition <= NormalizedSelectionStart) {
				draggedThumb = leftSelectionThumb;
				fixedThumb = rightSelectionThumb;
				isResizingDetected = true;
			}
			else if (currentPosition >= NormalizedSelectionEnd) {
				draggedThumb = rightSelectionThumb;
				fixedThumb = leftSelectionThumb;
				isResizingDetected = true;
			}
		}
		private object PixelToReal(double currentPosition) {
			double normalized = currentPosition / Client.ClientBounds.Width;
			return NormalizedToReal(normalized);
		}
		bool isResizingDetected;
		internal void PrepareResizeSelection() {
			InvalidateRender(CanStartAnimation());
			Animator.CanAnimate = false;
			AllowUpdateNormalizedRange = false;
			isResizingDetected = false;
			OnSelectionResizingStarted();
		}
		internal void OnControllerReset(RangeControlStateType state) {
			UpdateByControllerState(state);
		}
		internal void ScrollByDelta(double pixelDelta) {
			if (scrollViewer != null && AllowScroll)
				scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset + pixelDelta);
		}
		internal void PrepareMoveSelection() {
			GoToMoveSelectionState();
			double selectionWidth = NormalizedSelectionEnd - NormalizedSelectionStart;
			double visibleWidth = RealToNormalized(ActualVisibleEnd) - RealToNormalized(ActualVisibleStart);
			IsSelectionRangeLessVisibleRange = DoubleExtensions.GreaterThanOrClose(visibleWidth, selectionWidth);
			AllowUpdateNormalizedRange = false;
			Invalidate();
		}
		private bool CanAutoScrollOnSelectionMoving(object start, object end) {
			return RealToNormalized(ActualSelectionStart) >= RealToNormalized(ActualVisibleStart) &&
				RealToNormalized(ActualSelectionEnd) <= RealToNormalized(ActualVisibleEnd);
		}
		internal void OnLabelTapped(Point point) {
			if (Client != null)
				Client.HitTest(point);
		}
		internal void ZoomByWheel(int wheelDelta, double position) {
			if (!AllowZoom) return;
			double factor = Math.Sign(wheelDelta) == -1 ? 2 : 0.5;
			if (ProcessZoom(position, factor))
				PostponeSnapNormalizedRangeToCenter();
			UpdateVisibleOnControllerReset();
		}
		private void PostponeSnapNormalizedRangeToCenter() {
			EnqueueAction(new Action(SnapNormalizedRangeToCenter));
		}
		internal void ZoomByPinch(double scale, double position) {
			position = RealToPixel(ActualVisibleStart) + position;
			double factor = 2 / scale;
			if (ProcessZoom(position, factor))
				EnqueueAction(new Action(SnapNormalizedRangeToCenter));
		}
		internal void ZoomByDoubleTap(Point position) {
			if (!AllowZoom) return;
			if (CanChangeVisibleRange(position)) {
				StopPost();
				AnimateDoubleTapZoom(Client.VisibleStart, Client.VisibleEnd);
			}
		}
		private bool CanChangeVisibleRange(Point position) {
			RangeControlClientHitTestResult hitTest = Client.HitTest(position);
			double oldVisibleStart = RealToNormalized(Client.VisibleStart);
			double oldVisibleEnd = RealToNormalized(Client.VisibleEnd);
			bool isCorrect = Client.SetVisibleRange(hitTest.RegionStart, hitTest.RegionEnd, GetViewportSize());
			if (!DetectVisibleRangeWidthChange() || IsLastVisibleRangeEqualsCurrent(oldVisibleStart, oldVisibleEnd)) return false;
			if (isCorrect) CenterSelectionInsideVisibleRange();
			return true;
		}
		private bool IsLastVisibleRangeEqualsCurrent(double oldVisibleStart, double oldVisibleEnd) {
			return oldVisibleStart.AreClose(RealToNormalized(Client.VisibleStart)) && oldVisibleEnd.AreClose(RealToNormalized(Client.VisibleEnd));
		}
		Locker animateLabelsLocker = new Locker();
		private void AnimateDoubleTapZoom(object newVisibleStart, object newVisibleEnd) {
			Animator.CanAnimate = true;
			if (CanStartAnimation()) {
				if (!animateLabelsLocker.IsLocked) animateLabelsLocker.Lock();
				animator.AnimateDoubleTapZoom(GetNormalizedVisibleStart(), GetNormalizedVisibleEnd(), RealToNormalized(newVisibleStart), RealToNormalized(newVisibleEnd),
					(s, e) => { SetActualVisibleRange(NormalizedToReal(s), NormalizedToReal(e), true); });
			}
			else {
				SetActualVisibleRange(newVisibleStart, newVisibleEnd, true);
				InvalidateRender(CanStartAnimation());
				PostWithDelay(SetSelectionRangeByActual);
				PostWithDelay(SetVisibleRangeByActual);
				SyncronizeHorizontalOffset();
			}
		}
		private void StopPost() {
			StopPosting = true;
		}
		internal void OnRangeBarViewPortChanged() {
			if (AllowZoom)
				PostWithDelay(SetVisibleRangeByActual);
		}
		internal void PrepareZoom() {
			LastVisibleRangeWidth = 0;
		}
		internal void ThumbDragStarted(Point position, double dragDelta) {
			bool isLeftThumbDragged = IsLeftThumbDragged(position, dragDelta);
			draggedThumb = isLeftThumbDragged ? leftSelectionThumb : rightSelectionThumb;
			SelectionType = isLeftThumbDragged ? SelectionChangesType.Start : SelectionChangesType.End;
			OnSelectionResizingStarted();
		}
		private bool IsLeftThumbDragged(Point position, double dragDelta) {
			Rect leftThumbBounds = GetLeftThumbBounds();
			Rect rightThumbBounds = GetRightThumbBounds();
			double leftCenterDelta = Math.Abs(TransformHelper.GetElementCenter(leftSelectionThumb, clientPanel) - position.X);
			double rightCenterDelta = Math.Abs(TransformHelper.GetElementCenter(rightSelectionThumb, clientPanel) - position.X);
			return leftThumbBounds.Contains(position) &&
				   (!leftThumbBounds.IntersectsWith(rightThumbBounds) || leftCenterDelta < rightCenterDelta || (leftThumbBounds.IntersectsWith(rightThumbBounds) && Math.Sign(dragDelta) == -1));
		}
		internal void ProcessSelectionResizing(double currentPosition) {
			bool isSelectionTypeChanged;
			double rangeDelta;
			double positionDelta;
			CalcSelectionRangeDelta(currentPosition, out isSelectionTypeChanged, out rangeDelta, out positionDelta);
			if (Math.Sign(positionDelta) == Math.Sign(rangeDelta))
				ChangeActualSelection(NormalizeByLength(rangeDelta.Round()), isSelectionTypeChanged);
		}
		double prevPosition = double.NaN;
		private void CalcSelectionRangeDelta(double currentPosition, out bool isSelectionTypeChanged, out double rangeDelta, out double positionDelta) {
			currentPosition = ConstrainPosition(currentPosition);
			fixedThumb = draggedThumb.Equals(leftSelectionThumb) ? rightSelectionThumb : leftSelectionThumb;
			double value = draggedThumb.Equals(leftSelectionThumb) ? NormalizedSelectionStart : NormalizedSelectionEnd;
			rangeDelta = currentPosition - RealToPixel(NormalizedToReal(value));
			if (double.IsNaN(prevPosition)) {
				prevPosition = currentPosition;
				positionDelta = currentPosition;
			}
			else
				positionDelta = currentPosition - prevPosition;
			prevPosition = currentPosition;
			isSelectionTypeChanged = IsSelectionTypeChanged(currentPosition, positionDelta);
		}
		private bool IsSelectionTypeChanged(double currentPosition) {
			double fixedCenter = TransformHelper.GetElementCenter(fixedThumb, clientPanel);
			SelectionChangesType changesType = (currentPosition >= fixedCenter) ? SelectionChangesType.End : SelectionChangesType.Start;
			bool isChanged = SelectionType != changesType;
			SelectionType = changesType;
			return isChanged;
		}
		private bool IsSelectionTypeChanged(double currentPosition, double delta) {
			double fixedCenter = TransformHelper.GetElementCenter(fixedThumb, clientPanel);
			SelectionChangesType changesType = SelectionType;
			if (draggedThumb == rightSelectionThumb && (Math.Sign(delta) == -1) && currentPosition < fixedCenter) changesType = SelectionChangesType.Start;
			if (draggedThumb == leftSelectionThumb && (Math.Sign(delta) == 1) && currentPosition >= fixedCenter) changesType = SelectionChangesType.End;
			bool isChanged = SelectionType != changesType;
			SelectionType = changesType;
			return isChanged;
		}
		internal void SelectGroupInterval() {
			SetActualSelectionRangeInternal(CurrentHitTest.RegionStart, CurrentHitTest.RegionEnd);
			SetNormalizedRangeInternal(RealToNormalized(ActualSelectionStart), RealToNormalized(ActualSelectionEnd));
			Invalidate();
		}
		internal FrameworkElement GetRootContainer() {
			return rootContainer;
		}
		internal bool IsPositionOutOfBounds(double position) {
			return position < 0 || position > layoutPanel.ActualWidth;
		}
		internal void StopAutoScroll() {
			ResetAutoScroll();
		}
		internal void StartAutoScroll(double position, bool isResize) {
			Action action = null;
			IsAutoScrollInProcess = true;
			if (isResize) action = position < 0 ? new Action(ScrollToActualVisibleStart) : new Action(ScrollToActualVisibleEnd);
			else action = position < 0 ? new Action(MoveToActualVisibleStart) : new Action(MoveToActualVisibleEnd);
			StartAutoScroll(action);
		}
		internal void ScrollRangeBar(double normalOffset) {
			if (!AllowScroll) return;
			double offset = normalOffset * ClientWidth;
			AnimateScroll(offset);
		}
	}
	public class EditRange {
		object start;
		object end;
		public object Start {
			get { return start; }
			private set { start = value; }
		}
		public object End {
			get { return end; }
			private set { end = value; }
		}
		public EditRange() { }
		public EditRange(object start, object end) {
			Start = start;
			End = end;
		}
		public override bool Equals(object obj) {
			EditRange range = obj as EditRange;
			if (range == null)
				return false;
			return range.start == Start && range.End == End;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
}
