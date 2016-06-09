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

using System.Windows;
using System.Linq;
using System.Windows.Controls;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using System.Windows.Input;
using System;
using System.Collections.Generic;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.DocumentViewer {
	public class NavigationStrategy {
		readonly Locker pageIndexSyncLocker = new Locker();
		readonly Locker backPageIndexSyncLocker = new Locker();
		readonly Locker scrollLocker = new Locker();
		readonly Locker changedPositionLocker = new Locker();
		readonly DocumentPresenterControl presenter;
		protected DocumentViewerPanel ItemsPanel { get { return presenter.ItemsPanel; } }
		protected ScrollViewer ScrollViewer { get { return presenter.ScrollViewer; } }
		protected BehaviorProvider BehaviorProvider { get { return presenter.BehaviorProvider; } }
		protected DocumentPresenterControl Presenter { get { return presenter; } }
		protected PositionCalculator PositionCalculator { get; private set; }
		int PageCount { get { return presenter.Pages.Return(x => x.Count(), () => 0); } }
		int PageIndex { get { return BehaviorProvider.PageIndex; } }
		IEnumerable<PageWrapper> Pages { get { return presenter.Pages; } }
		public NavigationStrategy(DocumentPresenterControl documentPresenter) {
			presenter = documentPresenter;
			PositionCalculator = CreatePositionCalculator();
		}
		public virtual void ProcessScroll(ScrollCommand command) {
			switch (command) {
				case ScrollCommand.LineUp:
					ScrollViewer.Do(x => x.LineUp());
					break;
				case ScrollCommand.LineDown:
					ScrollViewer.Do(x => x.LineDown());
					break;
				case ScrollCommand.LineLeft:
					ScrollViewer.Do(x => x.LineLeft());
					break;
				case ScrollCommand.LineRight:
					ScrollViewer.Do(x => x.LineRight());
					break;
				case ScrollCommand.PageUp:
					ScrollViewer.Do(x => x.PageUp());
					break;
				case ScrollCommand.PageDown:
					ScrollViewer.Do(x => x.PageDown());
					break;
				case ScrollCommand.Home:
					ScrollViewer.Do(x => x.ScrollToHome());
					break;
				case ScrollCommand.End:
					if (PageIndex == PageCount - 1)
						ScrollViewer.ScrollToEnd();
					else
						ScrollIntoView(PageCount - 1, Rect.Empty, ScrollIntoViewMode.TopLeft);
					break;
			}
		}
		public virtual void ProcessScrollViewerScrollChanged(System.Windows.Controls.ScrollChangedEventArgs e) {
			UpdatePageIndex();
			if (scrollLocker.IsLocked)
				return;
			RegisterUndoAction(GenerateCurrentState(BehaviorProvider.RotateAngle, BehaviorProvider.ZoomFactor), UndoActionType.DeferredScroll);
		}
		public virtual void ProcessPageIndexChanged(int pageIndex) {
			pageIndexSyncLocker.DoIfNotLocked(() => {
				backPageIndexSyncLocker.LockOnce();
				var pageWrapperIndex = PositionCalculator.GetPageWrapperIndex(pageIndex);
				ScrollIntoView(pageWrapperIndex, Rect.Empty, ScrollIntoViewMode.TopLeft);
			});
		}
		public virtual void ProcessRotateAngleChanged(RotateAngleChangedEventArgs e) {
			if (!changedPositionLocker.IsLocked && ScrollViewer != null) {
				double zoomFactor = BehaviorProvider.ZoomFactor;
				NavigationState oldState = GenerateCurrentState(e.OldValue, zoomFactor);
				NavigationState newState = oldState.Clone();
				newState.Angle = e.NewValue;
				UpdatePagesRotateAngle(e.NewValue);
				ChangePositionAndRegisterUndoAction(oldState, newState, UndoActionType.Rotate);
			}
			else
				UpdatePagesRotateAngle(e.NewValue);
		}
		public virtual void ProcessZoomChanged(ZoomChangedEventArgs e) {
			if (!changedPositionLocker.IsLocked && ScrollViewer != null) {
				NavigationState oldState = GenerateCurrentState(BehaviorProvider.RotateAngle, e.OldZoomFactor);
				NavigationState newState = oldState.Clone();
				newState.ZoomFactor = e.ZoomFactor;
				UpdatePagesZoomFactor(e.ZoomFactor);
				ChangePositionAndRegisterUndoAction(oldState, newState, UndoActionType.Zoom);
			}
			else
				UpdatePagesZoomFactor(e.ZoomFactor);
		}
		public virtual void ProcessScrollTo(double offset, bool isVertical) {
			if (isVertical)
				ScrollViewer.ScrollToVerticalOffset(offset);
			else
				ScrollViewer.ScrollToHorizontalOffset(offset);
		}
		public virtual void ProcessMarqueeZoom(Rect rect, double x, double y) {
			double marqueeZoomFactor = CalcFitRectangleZoomFactor(BehaviorProvider.Viewport, rect);
			ScrollToAnchorPoint(marqueeZoomFactor, CalcHorizontalAnchorPointForFitRectangle(rect, x), CalcVerticalAnchorPointForFitRectangle(rect, y), UndoActionType.Scroll);
		}
		public virtual void ZoomToAnchorPoint(bool isZoomIn, Point visibleAnchorPoint) {
			double zoomFactor = CalcMouseWheelZoomFactor(isZoomIn);
			double delta = BehaviorProvider.ZoomFactor / zoomFactor;
			double horizontalOffset = visibleAnchorPoint.X + ScrollViewer.HorizontalOffset;
			double horizontalAnchor = Math.Max(horizontalOffset - delta * visibleAnchorPoint.X, 0d);
			double verticalOffset = 0d;
			if (Presenter.ShowSingleItem) {
				double relativeOffset = ItemsPanel.GetPageRelativeVerticalOffset(true);
				verticalOffset = visibleAnchorPoint.Y + PositionCalculator.GetPageVerticalOffset(PageIndex, relativeOffset);
				double pageHeight = ItemsPanel.IndexCalculator.GetRealItemSize(PageIndex).Height;
				if (pageHeight.LessThanOrClose(ItemsPanel.ViewportHeight))
					verticalOffset -= (ItemsPanel.ViewportHeight - pageHeight) / 2d;
			}
			else 
				verticalOffset = visibleAnchorPoint.Y + ScrollViewer.VerticalOffset;
			double verticalAnchor = Math.Max(verticalOffset - delta * visibleAnchorPoint.Y, 0d);
			ScrollToAnchorPoint(zoomFactor, horizontalAnchor, verticalAnchor, UndoActionType.Zoom);
		}
		public void ChangePosition(NavigationState state) {
			ChangePositionInternal(state);
		}
		public void ScrollIntoView(int pageWrapperIndex, Rect rect, ScrollIntoViewMode mode) {
			ItemsPanel.Do(x => x.ScrollIntoView(pageWrapperIndex, rect, mode));
		}
		public void GenerateStartUpState() {
			presenter.UndoRedoManager.Do(x => x.Flush());
		}
		public void ScrollToStartUp() {
			ChangePositionWithDeferredScrollLocking(new NavigationState {
				Angle = BehaviorProvider.Return(x => x.RotateAngle, () => 0d),
				OffsetX = 0,
				OffsetY = 0,
				PageIndex = BehaviorProvider.Return(x => x.PageIndex, () => 1),
				ZoomFactor = BehaviorProvider.Return(x => x.ZoomFactor.IsNumber(), () => false) ? BehaviorProvider.ZoomFactor : 1d
			});
		}
		protected virtual PositionCalculator CreatePositionCalculator() {
			return new PositionCalculator(() => Pages);
		}
		double CalcMouseWheelZoomFactor(bool isZoomIn) {
			return BehaviorProvider.GetNextZoomFactor(isZoomIn);
		}
		void ChangePositionAndRegisterUndoAction(NavigationState oldState, NavigationState newState, UndoActionType state) {
			RegisterUndoAction(newState, state);
			ChangePosition(newState);
		}
		void RegisterUndoAction(NavigationState newState, UndoActionType state) {
			scrollLocker.LockOnce();
			if (presenter.UndoRedoManager == null)
				return;
			presenter.UndoRedoManager.RegisterAction(new UndoState { State = newState, Perform = ChangePositionWithDeferredScrollLocking, ActionType = state });
			presenter.ImmediateActionsManager.EnqueueAction(new DelegateAction(() => scrollLocker.Unlock()));
		}
		void ChangePositionInternal(NavigationState state) {
			changedPositionLocker.DoLockedActionIfNotLocked(() => {
				BehaviorProvider.RotateAngle = state.Angle;
				BehaviorProvider.ZoomFactor = state.ZoomFactor;
				PositionCalculator calc = PositionCalculator;
				double horizontalOffset = calc.GetPageHorizontalOffset(state.OffsetX);
				double verticalOffset = calc.GetPageVerticalOffset(state.PageIndex, state.OffsetY) - calc.GetPageVerticalOffset(state.PageIndex, 0);
				Presenter.With(x => x.ImmediateActionsManager).Do(x => x.EnqueueAction(new DelegateAction(() => ScrollIntoView(calc.GetPageWrapperIndex(state.PageIndex), new Rect(horizontalOffset, verticalOffset, 1, 1), ScrollIntoViewMode.TopLeft))));
			});
		}
		void ChangePositionWithDeferredScrollLocking(NavigationState state) {
			scrollLocker.LockOnce();
			ChangePosition(state);
			presenter.ImmediateActionsManager.EnqueueAction(new DelegateAction(() => scrollLocker.Unlock()));
		}
		protected NavigationState GenerateCurrentState(double rotateAngle, double zoomFactor) {
			return GenerateCurrentState(rotateAngle, zoomFactor, () => ScrollViewer.Return(x => x.HorizontalOffset, () => 0d), () => ScrollViewer.Return(x => x.VerticalOffset, () => 0d));
		}
		NavigationState GenerateCurrentState(double rotateAngle, double zoomFactor, Func<double> getHorizontalOffset, Func<double> getVerticalOffset) {
			int pageIndex = Presenter.ShowSingleItem ? ItemsPanel.IndexCalculator.VerticalOffsetToIndex(ItemsPanel.GetVirtualVerticalOffset()) : PositionCalculator.GetPageIndex(getVerticalOffset());
			double relativeOffsetX = PositionCalculator.GetRelativeOffsetX(getHorizontalOffset(), ScrollViewer.ExtentWidth);
			double relativeOffsetY = Presenter.ShowSingleItem ? ItemsPanel.GetPageRelativeVerticalOffset(true) : PositionCalculator.GetRelativeOffsetY(getVerticalOffset());
			return new NavigationState() {
				PageIndex = pageIndex,
				Angle = rotateAngle,
				ZoomFactor = zoomFactor,
				OffsetX = relativeOffsetX,
				OffsetY = relativeOffsetY,
			};
		}
		public void ScrollToAnchorPoint(double zoomFactor, double horizontalAnchor, double verticalAnchor, UndoActionType actionType) {
			var oldState = GenerateCurrentState(BehaviorProvider.RotateAngle, BehaviorProvider.ZoomFactor);
			var newState = oldState.Clone();
			newState.ZoomFactor = zoomFactor;
			double maxPageWidth = PositionCalculator.GetMaxPageWidth();
			double relativeHorizontalOffset = PositionCalculator.GetRelativeOffsetX(horizontalAnchor, maxPageWidth);
			newState.OffsetX = relativeHorizontalOffset;
			double relativeVerticalOffset = PositionCalculator.GetRelativeOffsetY(verticalAnchor);
			newState.OffsetY = relativeVerticalOffset;
			newState.PageIndex = PositionCalculator.GetPageIndex(verticalAnchor);
			ChangePositionAndRegisterUndoAction(oldState, newState, actionType);
		}
		protected double CalcHorizontalAnchorPointForFitRectangle(Rect rect, double x) {
			double offset = CalcMinHorizontalOffsetFromPanel();
			double marqueeX = offset.GreaterThan(0d) ? x - offset : x;
			double horizontalOffset = ScrollViewer.HorizontalOffset;
			bool shouldUseWidth = !ShouldUseHeight(rect);
			if (shouldUseWidth)
				return marqueeX + horizontalOffset;
			double viewportProp = BehaviorProvider.Viewport.Width / BehaviorProvider.Viewport.Height;
			double width = rect.Height * viewportProp;
			double center = marqueeX + rect.Width / 2d + horizontalOffset;
			double anchor = center - width / 2d;
			return Math.Max(0d, anchor);
		}
		protected double CalcVerticalAnchorPointForFitRectangle(Rect rect, double y) {
			double relativeOffset = ItemsPanel.GetPageRelativeVerticalOffset(Presenter.ShowSingleItem);
			double verticalOffset = Presenter.ShowSingleItem ? PositionCalculator.GetPageVerticalOffset(PageIndex, relativeOffset) : ScrollViewer.VerticalOffset;
			double pageHeight = ItemsPanel.IndexCalculator.GetRealItemSize(PageIndex).Height;
			if (Presenter.ShowSingleItem && pageHeight.LessThanOrClose(ItemsPanel.ViewportHeight))
				verticalOffset -= (ItemsPanel.ViewportHeight - pageHeight) / 2d;
			bool shouldUseHeight = ShouldUseHeight(rect);
			if (shouldUseHeight)
				return y + verticalOffset;
			double viewportProp = BehaviorProvider.Viewport.Width / BehaviorProvider.Viewport.Height;
			double height = rect.Width / viewportProp;
			double center = y + rect.Height / 2d + verticalOffset;
			double anchor = center - height / 2d;
			return Math.Max(0d, anchor);
		}
		double CalcMinHorizontalOffsetFromPanel() {
			double maxPageWidth = PositionCalculator.GetMaxPageWidth();
			return ItemsPanel.CalcPageHorizontalOffset(maxPageWidth);
		}
		protected double CalcFitRectangleZoomFactor(Size viewport, Rect rect) {
			bool shouldUseWidth = !ShouldUseHeight(rect);
			double diff = shouldUseWidth ? viewport.Width / rect.Width : viewport.Height / rect.Height;
			return Math.Min(5d, Math.Max(0.1d, BehaviorProvider.ZoomFactor * diff));
		}
		bool ShouldUseHeight(Rect rect) {
			double viewportProp = BehaviorProvider.Viewport.Width / BehaviorProvider.Viewport.Height;
			return rect.Width.LessThan(rect.Height * viewportProp);
		}
		void UpdatePagesZoomFactor(double zoomFactor) {
			if (!Presenter.HasPages)
				return;
			foreach (PageWrapper page in Pages)
				page.ZoomFactor = zoomFactor;
			ItemsPanel.Do(x => x.InvalidatePanel());
		}
		void UpdatePagesRotateAngle(double rotateAngle) {
			UpdatePagesRotateAngleInternal(rotateAngle);
			if (!Presenter.HasPages)
				return;
			foreach (PageWrapper page in Pages)
				page.RotateAngle = rotateAngle;
			ItemsPanel.Do(x => x.InvalidatePanel());
		}
		protected virtual void UpdatePagesRotateAngleInternal(double rotateAngle) {
		}
		void UpdatePageIndex() {
			if (backPageIndexSyncLocker.IsLocked) {
				backPageIndexSyncLocker.Unlock();
				return;
			}
			var indexCalculator = ItemsPanel.IndexCalculator;
			int pageWrapperIndex = indexCalculator.VerticalOffsetToIndex(ItemsPanel.GetVirtualVerticalOffset());
			pageIndexSyncLocker.DoLockedAction(() => BehaviorProvider.PageIndex = PositionCalculator.GetPageIndexFromWrapper(pageWrapperIndex));
		}
	}
	public class PositionCalculator {
		readonly Func<IEnumerable<PageWrapper>> getPagesHandler;
		public PositionCalculator(Func<IEnumerable<PageWrapper>> getPagesHandler) {
			this.getPagesHandler = getPagesHandler;
		}
		public double GetRelativeOffsetX(double horizontalOffset, double extentWidth) {
			if (extentWidth.AreClose(0d))
				return 0d;
			return horizontalOffset / extentWidth;
		}
		public double GetPageVerticalOffset(int pageIndex, double relativeOffset) {
			double offset = 0;
			int wrapperIndex = GetPageWrapperIndex(pageIndex);
			for (int i = 0; i < wrapperIndex; i++) {
				Size visibleSize = GetWrapperRenderSize(i);
				offset += visibleSize.Height;
			}
			Size cpVisibleSize = GetPageRenderSize(pageIndex);
			if (cpVisibleSize.IsEmpty)
				return 0d;
			return offset + cpVisibleSize.Height * relativeOffset;
		}
		public double GetPageHorizontalOffset(double relativeOffset) {
			return GetMaxPageWidth() * relativeOffset;
		}
		public double GetRelativeOffsetY(double verticalOffset) {
			int pageIndex = GetPageIndex(verticalOffset);
			double offsetY = GetPageVerticalOffset(pageIndex, 0d);
			double delta = verticalOffset - offsetY;
			var pageSize = GetPageRenderSize(pageIndex);
			if (pageSize.IsEmpty)
				return 0;
			return delta / pageSize.Height;
		}
		public Size GetWrapperRenderSize(int wrapperIndex) {
			var pages = getPagesHandler();
			if (pages == null)
				return Size.Empty;
			PageWrapper page = pages.ElementAtOrDefault(wrapperIndex);
			return page.Return(x => x.RenderSize, () => Size.Empty);
		}
		public Size GetPageRenderSize(int pageIndex) {
			int wrapperIndex = GetPageWrapperIndex(pageIndex);
			return GetWrapperRenderSize(wrapperIndex);
		}
		public double GetMaxPageWidth() {
			double maxWidth = 0d;
			var pages = getPagesHandler();
			if (pages == null)
				return 0d;
			foreach (var page in pages)
				maxWidth = Math.Max(maxWidth, page.RenderSize.Width);
			return maxWidth;
		}
		public int GetPageIndex(double verticalOffset) {
			var pages = getPagesHandler();
			if (pages == null)
				return 0;
			double offset = 0d;
			for (int i = 0; i < pages.Count(); ++i) {
				offset += pages.ElementAt(i).RenderSize.Height;
				if ((offset - verticalOffset).GreaterThan(0d))
					return pages.ElementAt(i).Pages.First().PageIndex;
			}
			return 0;
		}
		public int GetPageIndex(double verticalOffset, double horizontalOffset, Func<double, double> getPageHorizontalOffsetHandler) {
			var pages = getPagesHandler();
			if (pages == null)
				return 0;
			var pageIndex = GetPageIndex(verticalOffset);
			var pageWrapperIndex = GetPageWrapperIndex(pageIndex);
			var pageWrapper = pages.ElementAt(pageWrapperIndex);
			var pageWrapperOffset = GetPageWrapperVerticalOffset(pageWrapperIndex);
			foreach (var page in pageWrapper.Pages) {
				var pageRect = pageWrapper.GetPageRect(page);
				if (pageRect.IsInside(new Point(horizontalOffset - getPageHorizontalOffsetHandler(pageWrapper.RenderSize.Width), verticalOffset - pageWrapperOffset)))
					return page.PageIndex;
			}
			return 0;
		}
		public int GetPageIndexFromWrapper(int wrapperIndex) {
			var pages = getPagesHandler();
			if (pages == null)
				return 0;
			if (wrapperIndex < 0 || wrapperIndex >= pages.Count())
				return -1;
			return pages.ElementAt(wrapperIndex).Pages.First().PageIndex;
		}
		public double GetPageWrapperOffset(int pageIndex) {
			int pageWrapperIndex = GetPageWrapperIndex(pageIndex);
			var pages = getPagesHandler();
			if (pages == null)
				return 0;
			var pageWrapper = pages.ElementAt(pageWrapperIndex);
			var page = pageWrapper.Pages.Single(x => x.PageIndex == pageIndex);
			return pageWrapper.GetPageRect(page).Left;
		}
		public double GetPageWrapperVerticalOffset(int pageWrapperIndex) {
			var pages = getPagesHandler();
			if (pages == null)
				return 0;
			double verticalOffset = 0d;
			for (int i = 0; i < pageWrapperIndex; ++i)
				verticalOffset += pages.ElementAt(i).RenderSize.Height;
			return verticalOffset;
		}
		public int GetPageWrapperIndex(int pageIndex) {
			var pages = getPagesHandler();
			if (pages == null)
				return 0;
			for (int i = 0; i < pages.Count(); ++i) {
				if (pages.ElementAt(i).Pages.Any(page => page.PageIndex == pageIndex))
					return i;
			}
			return 0;
		}
	}
}
