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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid.Native;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Grid.Hierarchy;
using DevExpress.Mvvm.Native;
#if SILVERLIGHT
using DevExpress.Xpf.Core.WPFCompatibility;
using Visual = System.Windows.UIElement;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
#endif
namespace DevExpress.Xpf.Grid {
	public abstract class DataPresenterBase : ContentControl, IScrollInfoOwner, IScrollInfo, INotifyCurrentViewChanged {
		public const double DefaultTouchScrollThreshold = 3;
		public const double WheelScrollLinesPerPage = -1;
		internal static int GenerateItemsCount { get; set; }
		static DataPresenterBase() {
			GenerateItemsCount = 5;
		}
		static void AutoSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
		}
		ScrollInfo scrollInfoCore;
		Size lastConstraint;
		internal Size LastConstraint {
			get { return lastConstraint; }
			private set {
				if(lastConstraint == value) return;
				lastConstraint = value;
				View.UpdateCellMergingPanels();
				if(Panel != null)
					Panel.CalcViewport(lastConstraint);
			}
		}
		DataViewBase viewCore;
		IContinousAction currentContinousAction;
		Queue<IContinousAction> continousActionsQueue = new Queue<IContinousAction>();
		internal HierarchyPanel Panel { get; set; }
#if DEBUGTEST
		internal long LayoutUpdatedFireCount { get; set; }
		internal static event EventHandler LayoutUpdatedWhenEnoughItems;
#endif
		bool HasQueuedContinuousActions { get { return continousActionsQueue.Count > 0; } }
		public virtual double ScrollItemOffset { get { return 0; } }
		internal int ItemCount { get { return DataControl == null ? 0 : DataControl.VisibleRowCount + View.CalcGroupSummaryVisibleRowCount() + View.DataControl.MasterDetailProvider.CalcVisibleDetailRowsCount(); } }
		protected DataPresenterBase() {
			Unloaded += new RoutedEventHandler(DataPresenter_Unloaded);
			Loaded += new RoutedEventHandler(DataPresenter_Loaded);
			Content = CreateContent();
			HorizontalContentAlignment = HorizontalAlignment.Stretch;
			FocusVisualStyle = null;
		}
		internal FrameworkElement ContentElement { get { return Content as FrameworkElement; } }
		protected abstract FrameworkElement CreateContent();
		void DataPresenter_Loaded(object sender, RoutedEventArgs e) {
			LayoutUpdated += new EventHandler(OnLayoutUpdated);
			UpdateView();
			InvalidateMeasure();
		}
		void DataPresenter_Unloaded(object sender, RoutedEventArgs e) {
			LayoutUpdated -= new EventHandler(OnLayoutUpdated);
		}
		public ScrollInfo ScrollInfoCore {
			get {
				if(scrollInfoCore == null)
					scrollInfoCore = CreateScrollInfo();
				return scrollInfoCore;
			}
		}
		protected virtual void UpdateView() {
			if(View == null)
				return;
			View.ScrollInfoOwner = this;
			AddUpdateRowsStateAction();
			UpdateViewCore();
		}
		protected abstract void UpdateViewCore();
		protected internal virtual void UpdateAutoSize() {
			ScrollInfoCore.HorizontalScrollInfo.SetOffsetForce(0);
		}
		internal DataViewBase View { get { return viewCore; } }
		internal DataControlBase DataControl { get { return View == null ? null : View.DataControl; } }
		internal SizeHelperBase SizeHelper { get { return SizeHelperBase.GetDefineSizeHelper(View.OrientationCore); } }
		internal double CollapseBufferSize { get; set; }
		protected virtual VirtualDataStackPanelScrollInfo CreateScrollInfo() {
			return new VirtualDataStackPanelScrollInfo(this);
		}
		protected virtual bool CheckIsTreeValid() {
			return UIElementHelper.IsVisibleInTree(this, true);
		}
		internal bool IsInAction { get { return currentContinousAction != null; } }
#if !SL
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			base.OnPropertyChanged(e);
			if(e.Property == UIElement.IsVisibleProperty && IsVisible)
				InvalidateMeasure();
		}
#endif
		void OnLayoutUpdated(object sender, EventArgs e) {
			rendered = true;
			if(LayoutUpdatedHelper.GlobalLocker.IsLocked || (View != null && View.layoutUpdatedLocker.IsLocked))
				return;
#if DEBUGTEST
			LayoutUpdatedFireCount++;
#endif
			if(!CheckIsTreeValid()) {
				ForceCompleteContinuousActions();
				return;
			}
			if(View == null || DataControl == null)
				return;
			View.RaiseCommandsCanExecuteChanged();
			DataControl.UpdateAllDetailAndOriginationDataControls(dataControl => dataControl.DataView.ViewBehavior.EnsureSurroundingsActualSize(LastConstraint));
			if(currentContinousAction != null && currentContinousAction.IsDone)
				currentContinousAction = null;
			if(currentContinousAction == null && HasQueuedContinuousActions) {
				currentContinousAction = continousActionsQueue.Dequeue();
				currentContinousAction.Prepare();
				InvalidateMeasure();
				return;
			}
			if(!OnLayoutUpdatedCore()) return;
#if DEBUGTEST
			if(LayoutUpdatedWhenEnoughItems != null)
				LayoutUpdatedWhenEnoughItems(this, EventArgs.Empty);
#endif
			if(currentContinousAction == null) {
				UpdateScrollInfo();
				if(!View.FocusedView.IsRootView)
					View.FocusedView.ClearCurrentCellIfNeeded();
				View.DataControl.UpdateAllDetailDataControls(dataControl => {
					dataControl.DataView.UpdateRowsState();
				});
				ExecuteImmediateActions();
			}
			if(currentContinousAction != null)
				currentContinousAction.Execute();
		}
		protected virtual bool OnLayoutUpdatedCore() {
			if(IsEnoughItems()) {
				View.RootNodeContainer.IsScrolling = false;
				if(View.CanAdjustScrollbar()) {
					if(!prohibitAdjustment && currentContinousAction == null) {
						double childHeight = GetChildHeight();
						if(adjustmentInProgress && childHeight > DesiredSize.Height) {
							prohibitAdjustment = true;
							if(View.ViewBehavior.AllowPerPixelScrolling) {
								double firstElementHeight = GetFirstVisibleRow().DesiredSize.Height;
								SetVerticalOffsetForce(CurrentOffset + (firstElementHeight * (1 - ScrollItemOffset) - lastEmptyHeight) / firstElementHeight);
							} else {
								SetVerticalOffsetForce(CurrentOffset + 1);
							}
							return false;
						}
						if(childHeight < DesiredSize.Height && CurrentOffset > 0) {
							lastEmptyHeight = DesiredSize.Height - childHeight;
							adjustmentInProgress = true;
							View.LockEditorClose = true;
							double delta = 1;
							if(View.ViewBehavior.AllowPerPixelScrolling && ScrollItemOffset != 0 && GetFirstVisibleRow() != null) {
								double firstElementHeight = GetFirstVisibleRow().DesiredSize.Height;
								if(firstElementHeight * ScrollItemOffset > DesiredSize.Height - childHeight)
									delta = (DesiredSize.Height - childHeight) / firstElementHeight;
								else
									delta = ScrollItemOffset;
							}
							adjustmentDelta = delta;
							SetVerticalOffsetForce(CurrentOffset - delta);
							return false;
						}
					}
					View.LockEditorClose = false;
					if(adjustmentInProgress) {
						adjustmentInProgress = false;
						UpdatePostponedData();
					}
				}
			} else {
				GenerateItems(GenerateItemsCount, LastConstraint.Height);
				return false;
			}
			return true;
		}
		protected double GetChildHeight(){
			UIElement element = Content as UIElement;
			if(element == null)
				return 0;
			return element.DesiredSize.Height;
		}
		protected internal FrameworkElement GetFirstVisibleRow() {
			RowDataBase rowData = GetRowDataToScroll();
			return rowData != null ? rowData.WholeRowElement : null;
		}
		protected internal RowDataBase GetRowDataToScroll() {
			if(View.DataControl == null)
				return null;
			RowNode node = View.RootNodeContainer.GetNodeToScroll();
			return node != null ? node.GetRowData() : null;
		}
		void UpdatePostponedData() {
			View.MasterRootRowsContainer.UpdatePostponedData(false, false);
		}
		internal void ForceCompleteContinuousActions() {
			ForceCompleteCurrentAction();
			continousActionsQueue.Clear();
		}
		internal void ForceCompleteCurrentAction() {
			if(currentContinousAction != null) {
				currentContinousAction.ForceComplete();
				currentContinousAction = null;
			}
		}
		internal void UpdateScrollInfo() {
			UpdateDefineSizeScrollInfo();
			UpdateSecondarySizeScrollInfo();
		}
		void UpdateDefineSizeScrollInfo() {
			double oldViewport = ScrollInfo.ViewportHeight;
			ScrollInfoCore.DefineSizeScrollInfo.UpdateScrollInfo(View.ScrollingMode == ScrollingMode.Normal ? 1 : Math.Max(0, PanelViewport), Extent);
			if(oldViewport != ScrollInfo.ViewportHeight) {
				UpdatePostponedData();
				View.UpdateCellMergingPanels();
			}
		}
		internal void UpdateSecondarySizeScrollInfo(bool allowUpdateViewportVisibleColumns = true) {
			if(View == null || (View.DataProviderBase != null && View.DataProviderBase.IsUpdateLocked))
				return;
			ScrollBarCombineHelper helper = new ScrollBarCombineHelper();
			DataControl.UpdateAllDetailDataControls(dataControl => helper.ProcessScrollInfo(dataControl.DataView.FixedViewport, dataControl.DataView.FixedExtent));
			ScrollInfoCore.SecondarySizeScrollInfo.UpdateScrollInfo(helper.FinalViewport, helper.FinalExtent);
			DataControl.UpdateAllDetailDataControls(dataControl => dataControl.DataView.ViewBehavior.UpdateSecondaryScrollInfoCore(-helper.ConverToRealOffset(ScrollInfoCore.SecondarySizeScrollInfo.Offset, dataControl.DataView.FixedViewport, dataControl.DataView.FixedExtent), allowUpdateViewportVisibleColumns));
		}
		internal void ClearScrollInfoAndUpdate() {
			scrollInfoCore.ClearScrollInfo();
			UpdateScrollInfo();
			SetVerticalOffsetForce(0);
		}
		protected virtual void GenerateItems(int count, double availableHeight) {
			View.RootNodeContainer.GenerateItems(count);
		}
		protected virtual void ExecuteImmediateActions() {
			View.ImmediateActionsManager.ExecuteActions();
		}
		bool prohibitAdjustment = false;
		bool adjustmentInProgress = false;
		internal bool AdjustmentInProgress { get { return adjustmentInProgress; } }
		double childDesiredHeight = 0d;
		double lastEmptyHeight = 0d;
		protected bool rendered = false;
		double adjustmentDelta = 0;
		protected override Size MeasureOverride(Size constraint) {
			if(DataControl == null)
				return default(Size);
			if(DesiredSize.Height != constraint.Height && !adjustmentInProgress) prohibitAdjustment = false;
			if(View == null)
				return new Size(0, 0);
			bool needUpdateScrollInfo = currentContinousAction == null && LastConstraint != constraint && !adjustmentInProgress;
			LastConstraint = ColumnsLayoutParametersValidator.GetLastDataPresenterConstraint(constraint);
#if !SL     //please check runtime if you ever decide to include this in SL
			if(!View.IsDesignTime) {
				InfiniteGridSizeException.ValidateDefineSize(SizeHelper.GetDefineSize(LastConstraint), View.OrientationCore, DataControl.GetType().Name);
			}
#endif
#if !SL     // remove automation events (dirty hack)
			ClearAutomationEventsHelper.ClearAutomationEvents();
#endif
			PregenerateItems(constraint);
			int oldItemsCount = View.RootNodeContainer.ItemCount;
			if(AdjustmentInProgress)
				oldItemsCount += (int)Math.Ceiling(adjustmentDelta);
			CancelAllGetRows();
			View.RootNodeContainer.ReGenerateItems(GenerateItemsOffset, oldItemsCount);
			EnsureAllRowsLoaded(ScrollOffset, oldItemsCount);
			Size desiredSize = base.MeasureOverride(GetMeasureSize(constraint));
#if !SL
			if(DataControl.AutomationPeer != null) DataControl.AutomationPeer.ResetDataPanelChildrenForce();
#endif
			if(childDesiredHeight != desiredSize.Height && !adjustmentInProgress) prohibitAdjustment = false;
			childDesiredHeight = desiredSize.Height;
			if(needUpdateScrollInfo) UpdateScrollInfo();
			return View.ViewBehavior.CorrectMeasureResult(ActualScrollOffset, constraint, desiredSize);
		}
		protected virtual double GetOffset() {
			return 0;
		}
		protected virtual void CancelAllGetRows() { }
		protected virtual void EnsureAllRowsLoaded(int firstRowIndex, int rowsCount) { }
		protected virtual void PregenerateItems(Size constraint) {
		}
		protected Size BaseMeasureOverride(Size constraint) {
			return base.MeasureOverride(GetMeasureSize(constraint));
		}
		protected virtual Size GetMeasureSize(Size constraint) {
			return new Size(constraint.Width, double.PositiveInfinity);
		}
		protected override Size ArrangeOverride(Size arrangeBounds) {
			if(View == null)
				return arrangeBounds;
			return ArrangeOverrideCore(arrangeBounds);
		}
		protected virtual Size ArrangeOverrideCore(Size arrangeBounds) {
			return base.ArrangeOverride(arrangeBounds);
		}
		internal void EnqueueContinousAction(IContinousAction action) {
			continousActionsQueue.Enqueue(action);
		}
		internal void AddUpdateRowsStateAction() {
			View.EnqueueImmediateAction(View.ForceUpdateRowsState);
		}
		internal void SetDefineScrollOffset(double scrollOffset) {
			ScrollInfoCore.DefineSizeScrollInfo.SetOffsetForce(scrollOffset);
		}
		internal void SetVerticalOffsetForce(double value) {
			ScrollInfoCore.SetVerticalOffsetForce(value);
		}
		internal void SetHorizontalOffsetForce(double value) {
			ScrollInfoCore.SetHorizontalOffsetForce(value);
		}
		#region IScrollInfoOwner Members
		bool IScrollInfoOwner.OnBeforeChangeItemScrollOffset() {
			return View.RequestUIUpdate();
		}
		bool IScrollInfoOwner.OnBeforeChangePixelScrollOffset() {
			return View.OnBeforeChangePixelScrollOffset();
		}
		FrameworkElement IScrollInfoOwner.ScrollContentPresenter { get { return View.ScrollContentPresenter; } }
		int IScrollInfoOwner.ScrollStep { get { return View.ScrollStep; } }
		bool IScrollInfoOwner.IsDeferredScrolling { get { return View.IsDeferredScrolling; } }
		int IScrollInfoOwner.ItemCount { get { return ItemCount; } }
		int IScrollInfoOwner.Offset { get { return ScrollOffset; } }
		DataControlScrollMode IScrollInfoOwner.VerticalScrollMode { get { return VerticalScrollModeCore; } }
		protected virtual DataControlScrollMode VerticalScrollModeCore { get { return View.ViewBehavior.AllowPerPixelScrolling ? DataControlScrollMode.ItemPixel : DataControlScrollMode.Item; } }
		DataControlScrollMode IScrollInfoOwner.HorizontalScrollMode { get { return HorizontalScrollModeCore; } }
		protected virtual DataControlScrollMode HorizontalScrollModeCore { get { return DataControlScrollMode.Pixel; } }
		void IScrollInfoOwner.OnSecondaryScrollInfoChanged() {
			OnSecondaryScrollInfoChangedCore();
		}
		void IScrollInfoOwner.OnDefineScrollInfoChanged() {
			OnDefineScrollInfoChangedCore();
		}
		bool IScrollInfoOwner.IsHorizontalScrollBarVisible {
			get { return View.IsHorizontalScrollBarVisible; }
			set { View.IsHorizontalScrollBarVisible = value; }
		}
		bool IScrollInfoOwner.IsTouchScrollBarsMode {
			get { return View.IsTouchScrollBarsMode; }
			set { View.IsTouchScrollBarsMode = value; }
		}
		protected virtual void OnDefineScrollInfoChangedCore() {
			ScrollInfoCore.OnScrollInfoChanged();
			if(View != null) {
				View.ViewBehavior.UpdateTopRowIndex();
			}
		}
		void OnSecondaryScrollInfoChangedCore() {
			ScrollInfoCore.OnScrollInfoChanged();
			if(View != null) {
				UpdateSecondarySizeScrollInfo(false);
				View.DataControl.UpdateAllDetailDataControls(dc => dc.DataView.ViewBehavior.UpdateViewportVisibleColumns());
				View.EnqueueImmediateAction(View.ViewBehavior.ResetHeadersChildrenCache);
			}
		}
		void IScrollInfoOwner.InvalidateMeasure() {
			InvalidateMeasure();
		}
		void IScrollInfoOwner.InvalidateHorizontalScrolling() {
			if(View != null)
				View.OnInvalidateHorizontalScrolling();
		}
		int IScrollInfoOwner.ItemsOnPage { get { return ViewPort; } }
		internal void ClearInvisibleItems() {
			if(View == null)
				return;
			View.RootNodeContainer.ReGenerateItemsCore(ScrollOffset, ViewPort + GenerateItemsCount);
		}
		internal int FullyVisibleItemsCount { get { return Panel != null ? Panel.FullyVisibleItemsCount : 0; } }
		double PanelViewport { get { return Panel != null ? Panel.Viewport : 0; } }
		protected virtual double Extent { get { return ItemCount; } }
		protected internal int ViewPort { get { return (int)Math.Ceiling(PanelViewport); } }
		protected internal int ScrollOffset { get { return (int)ScrollInfoCore.DefineSizeScrollInfo.Offset; } }
		protected internal virtual int GenerateItemsOffset { get { return ScrollOffset; } }
		protected internal double ActualScrollOffset { get { return ScrollInfoCore.DefineSizeScrollInfo.Offset; } }
		protected internal double ActualViewPort { get { return ScrollInfoCore.DefineSizeScrollInfo.Viewport; } }
		protected internal virtual bool CanScrollWithAnimation { get { return false; } }
		protected internal virtual bool IsAnimationInProgress { get { return false; } }
		protected internal virtual double CurrentOffset { get { return ActualScrollOffset; } }
		internal bool IsElementPartiallyVisible(Size size, Rect elementRect) {
			return SizeHelper.GetDefinePoint(elementRect.BottomRight()) > SizeHelper.GetDefineSize(size);
		}
		protected virtual bool IsElementVisible(RowNode node) {
			if(!node.IsRowVisible)
				return true;
			FrameworkElement rowElement = node.GetRowElement();
			if(rowElement == null) return false;
			Rect rowRect = LayoutHelper.GetRelativeElementRect(rowElement, this);
			return SizeHelper.GetDefinePoint(rowRect.Location()) < SizeHelper.GetDefineSize(LastConstraint) + CollapseBufferSize;
		}
		protected bool AreAllElementsVisible(NodeContainer containerItem) {
			if(containerItem.Items.Count == 0) return true;
			for(int i = containerItem.Items.Count - 1; i >= 0; i--) {
				if(!IsElementVisible(containerItem.Items[i]))
					return false;
			}
			return true;
		}
		bool IsEnoughItems() {
			if(!IsEnoughExpandingItems())
				return false;
			if(View.RootNodeContainer.IsFinished) {
				if(View.DataControl == null || View.DataControl.BottomRowBelowOldVisibleRowCount || View.RootNodeContainer.IsEnumeratorValid) return true;
				View.RootNodeContainer.OnDataChangedCore();
				return false;
			}
			return !AreAllElementsVisible(View.RootNodeContainer);
		}
		internal virtual bool IsEnoughExpandingItems() {
			return true;
		}
		public double WheelScrollLines {
			get {
				if(viewCore == null)
					return System.Windows.SystemParameters.WheelScrollLines;
				return viewCore.WheelScrollLines;
			}
		}
		#endregion
		#region IScrollInfo Members
		protected IScrollInfo ScrollInfo { get { return View != null && DataControl != null ? ScrollInfoCore as IScrollInfo : new FakeScrollInfo(); } }
		bool IScrollInfo.CanHorizontallyScroll {
			get { return ScrollInfo.CanHorizontallyScroll; }
			set { ScrollInfo.CanHorizontallyScroll = value; }
		}
		bool IScrollInfo.CanVerticallyScroll {
			get { return ScrollInfo.CanVerticallyScroll; }
			set { ScrollInfo.CanVerticallyScroll = value; }
		}
		double IScrollInfo.ExtentHeight { get { return ScrollInfo.ExtentHeight; } }
		double IScrollInfo.ExtentWidth { get { return ScrollInfo.ExtentWidth; } }
		double IScrollInfo.HorizontalOffset { get { return ScrollInfo.HorizontalOffset; } }
		void IScrollInfo.LineDown() { ScrollInfo.LineDown(); }
		void IScrollInfo.LineLeft() { ScrollInfo.LineLeft(); }
		void IScrollInfo.LineRight() { ScrollInfo.LineRight(); }
		void IScrollInfo.LineUp() { ScrollInfo.LineUp(); }
		Rect IScrollInfo.MakeVisible(Visual visual, Rect rectangle) {
			return ScrollInfo.MakeVisible(visual, rectangle);
		}
		void IScrollInfo.MouseWheelDown() { OnMouseWheelDown(); }
		protected virtual void OnMouseWheelDown() {
			ScrollInfo.MouseWheelDown();
		}
		void IScrollInfo.MouseWheelLeft() { ScrollInfo.MouseWheelLeft(); }
		void IScrollInfo.MouseWheelRight() { ScrollInfo.MouseWheelRight(); }
		void IScrollInfo.MouseWheelUp() { OnMouseWheelUp(); }
		protected virtual void OnMouseWheelUp() {
			ScrollInfo.MouseWheelUp();
		}
		void IScrollInfo.PageDown() { ScrollInfo.PageDown(); }
		void IScrollInfo.PageLeft() { ScrollInfo.PageLeft(); }
		void IScrollInfo.PageRight() { ScrollInfo.PageRight(); }
		void IScrollInfo.PageUp() { ScrollInfo.PageUp(); }
		ScrollViewer IScrollInfo.ScrollOwner {
			get { return ScrollInfo.ScrollOwner; }
			set { ScrollInfo.ScrollOwner = value; }
		}
		void IScrollInfo.SetHorizontalOffset(double offset) { ScrollInfo.SetHorizontalOffset(offset); }
		void IScrollInfo.SetVerticalOffset(double offset) { ScrollInfo.SetVerticalOffset(offset); }
		double IScrollInfo.VerticalOffset { get { return ScrollInfo.VerticalOffset; } }
		double IScrollInfo.ViewportHeight { get { return ScrollInfo.ViewportHeight; } }
		double IScrollInfo.ViewportWidth { get { return ScrollInfo.ViewportWidth; } }
		#endregion
#if !SL
		internal bool CanApplyScroll() {
			FrameworkElement firstVisibleRowElement = null;
			return CanApplyScroll(out firstVisibleRowElement);
		}
		internal bool CanApplyScroll(out FrameworkElement firstVisibleRowElement) {
			firstVisibleRowElement = null;
			if(View == null)
				return false;
			firstVisibleRowElement = View.DataPresenter.GetFirstVisibleRow();
			return firstVisibleRowElement != null;
		}
#if DEBUGTEST
		public
#else
		internal
#endif
		ScrollAccumulator accumulatorHorizontal = new ScrollAccumulator();
#if DEBUGTEST
		public
#else
		internal
#endif
		ScrollAccumulator accumulatorVertical = new ScrollAccumulator();
#endif
		#region INotifyCurrentViewChanged Members
		void INotifyCurrentViewChanged.OnCurrentViewChanged(DependencyObject d) {
			viewCore = DataControlBase.GetCurrentView(this);
			UpdateView();
		}
		#endregion
		public bool UseSmartMouseScrolling() {
			return true;
		}
		protected internal virtual void SetManipulation(bool isColumnFilterOpened) { }
	}
	public class DataPresenterScrollBehavior : ScrollBehaviorBase {
		protected DataPresenterBase GetDataPresenter(DependencyObject source) { return DataControlBase.GetCurrentView(source).DataPresenter; }
		public override bool CanScrollDown(DependencyObject source) {
			DataPresenterBase dataPresenter = GetDataPresenter(source);
			if(!dataPresenter.CanApplyScroll())
				return false;
			return dataPresenter.ScrollInfoCore.VerticalScrollInfo.Offset + dataPresenter.ScrollInfoCore.VerticalScrollInfo.Viewport < dataPresenter.ScrollInfoCore.VerticalScrollInfo.Extent;
		}
		public override bool CanScrollLeft(DependencyObject source) {
			DataPresenterBase dataPresenter = GetDataPresenter(source);
			if(!dataPresenter.CanApplyScroll())
				return false;
			return dataPresenter.ScrollInfoCore.HorizontalScrollInfo.Offset > 0;
		}
		public override bool CanScrollRight(DependencyObject source) {
			DataPresenterBase dataPresenter = GetDataPresenter(source);
			if(!dataPresenter.CanApplyScroll())
				return false;
			return dataPresenter.ScrollInfoCore.HorizontalScrollInfo.Offset + dataPresenter.ScrollInfoCore.HorizontalScrollInfo.Viewport < dataPresenter.ScrollInfoCore.HorizontalScrollInfo.Extent;
		}
		public override bool CanScrollUp(DependencyObject source) {
			DataPresenterBase dataPresenter = GetDataPresenter(source);
			if(!dataPresenter.CanApplyScroll())
				return false;
			return dataPresenter.ScrollInfoCore.VerticalScrollInfo.Offset > 0;
		}
		public override bool CheckHandlesMouseWheelScrolling(DependencyObject source) {
			return ((FrameworkElement)source).IsVisible;
		}
		public override void MouseWheelDown(DependencyObject source) {
			DataPresenterBase dataPresenter = GetDataPresenter(source);
			((IScrollInfo)dataPresenter).MouseWheelDown();
		}
		public override void MouseWheelLeft(DependencyObject source) {
			DataPresenterBase dataPresenter = GetDataPresenter(source);
			((IScrollInfo)dataPresenter).MouseWheelLeft();
		}
		public override void MouseWheelRight(DependencyObject source) {
			DataPresenterBase dataPresenter = GetDataPresenter(source);
			((IScrollInfo)dataPresenter).MouseWheelRight();
		}
		public override void MouseWheelUp(DependencyObject source) {
			DataPresenterBase dataPresenter = GetDataPresenter(source);
			((IScrollInfo)dataPresenter).MouseWheelUp();
		}
		public override bool PreventMouseScrolling(DependencyObject source) {
			return false;
		}
		public override void ScrollToHorizontalOffset(DependencyObject source, double offset) {
			ScrollToHorizontalOffsetCore(source, offset, false);
		}
		protected void ScrollToHorizontalOffsetCore(DependencyObject source, double offset, bool useAccumulator) {
			DataPresenterBase dataPresenter = GetDataPresenter(source);
			FrameworkElement firstVisibleRowElement = null;
			if(!dataPresenter.CanApplyScroll(out firstVisibleRowElement))
				return;
			double correctedOffset = dataPresenter.accumulatorHorizontal.GetCorrectedDelta(useAccumulator, firstVisibleRowElement.ActualWidth, -offset);
			dataPresenter.View.ViewBehavior.ChangeHorizontalOffsetBy(correctedOffset);
		}
		public override void ScrollToVerticalOffset(DependencyObject source, double offset) {
			ScrollToVerticalOffsetCore(source, offset, true);
		}
		protected void ScrollToVerticalOffsetCore(DependencyObject source, double offset, bool useAccumulator) {
			DataPresenterBase dataPresenter = GetDataPresenter(source);
			FrameworkElement firstVisibleRowElement = null;
			if(!dataPresenter.CanApplyScroll(out firstVisibleRowElement))
				return;
			double correctedOffset = useAccumulator ? dataPresenter.accumulatorVertical.GetCorrectedDelta(!dataPresenter.View.ViewBehavior.AllowPerPixelScrolling, firstVisibleRowElement.ActualHeight, offset) : offset;
			dataPresenter.View.ViewBehavior.ChangeVerticalOffsetBy(correctedOffset);
		}
	}
#if !SL
#if DEBUGTEST
	public
#else
	internal 
#endif
	class ScrollAccumulator {
		public const double ScrollLineSize = 120.0;
#if DEBUGTEST
		public
#endif
		double accumulator = 0;
#if DEBUGTEST
		public
#endif
		int direction = 0;
		void ChangeDirection(double delta) {
			if(Math.Sign(delta) != direction) {
				direction = Math.Sign(delta);
				accumulator = 0;
			}
		}
		public double GetCorrectedDelta(bool useAccumulator, double elementSize, double delta) {
			ChangeDirection(delta);
			double scaleFactorY = elementSize;
			double correctedDelta = -delta / scaleFactorY;
			if(!useAccumulator)
				return correctedDelta;
			double d = correctedDelta - (int)correctedDelta;
			correctedDelta = Math.Round(correctedDelta);
			accumulator += Math.Abs(d);
			if(Math.Abs(accumulator) >= 1) {
				accumulator--;
				correctedDelta -= direction;
			}
			return correctedDelta;
		}
	}
#endif
}
