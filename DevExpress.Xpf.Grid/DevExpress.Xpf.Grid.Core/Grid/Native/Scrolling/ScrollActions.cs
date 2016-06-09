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
using System.Windows;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid.Hierarchy;
namespace DevExpress.Xpf.Grid.Native {
	class ScrollOnePageUpAction : IAction {
		DataViewBase view;
		public ScrollOnePageUpAction(DataViewBase view) {
			this.view = view;
		}
		public DataViewBase View { get { return view; } }
		public void Execute() {
			if(View.ImmediateActionsManager.FindActionOfType(typeof(ScrollOneItemAfterPageUpPageDownAction)) != null) return;
			int scrollOffsetDelta = ScrollActionsHelper.GetOffsetDeltaForPageUp(View);
			bool needToAdjustScroll = true;
			if(scrollOffsetDelta >= 0) {
				scrollOffsetDelta = -1;
				needToAdjustScroll = false;
			}
			DataViewBase firstDataRowView = null;
			int firstDataRowVisibleIndex = 0;
			View.GetFirstScrollRowViewAndVisibleIndex(out firstDataRowView, out firstDataRowVisibleIndex);
			View.RootDataPresenter.SetDefineScrollOffset((int)Math.Ceiling(View.RootDataPresenter.ActualScrollOffset) + scrollOffsetDelta - ScrollActionsHelper.GetGroupSummaryRowCountBeforeRow(View, firstDataRowVisibleIndex, needToAdjustScroll));
			View.AddScrollOneItemAfterPageUpAction(firstDataRowView, firstDataRowVisibleIndex, needToAdjustScroll, 0, 0);
		}
	}
	class ScrollOnePageDownAction : IAction {
		DataViewBase view;
		public ScrollOnePageDownAction(DataViewBase view) {
			this.view = view;
		}
		public DataViewBase View { get { return view; } }
		public void Execute() {
			if(View.ImmediateActionsManager.FindActionOfType(typeof(ScrollOneItemAfterPageUpPageDownAction)) != null) return;
			int scrollOffsetDelta = ScrollActionsHelper.GetOffsetDeltaForPageDown(View);
			bool needToAdjustScroll = true;
			if(scrollOffsetDelta <= 0) {
				scrollOffsetDelta = 1;
				needToAdjustScroll = false;
			}
			DataViewBase lastDataRowView = null;
			int lastDataRowVisibleIndex = 0;
			View.GetLastScrollRowViewAndVisibleIndex(out lastDataRowView, out lastDataRowVisibleIndex);
			View.RootDataPresenter.SetDefineScrollOffset(View.RootDataPresenter.ActualScrollOffset + scrollOffsetDelta + ScrollActionsHelper.GetGroupSummaryRowCountAfterRow(View, lastDataRowVisibleIndex, needToAdjustScroll, true));
			View.AddScrollOneItemAfterPageDownAction(lastDataRowView, lastDataRowVisibleIndex, needToAdjustScroll, 0, 0);
		}
	}
	class ScrollOnePageUpWithAnimationAction : IAction {
		DataViewBase view;
		public ScrollOnePageUpWithAnimationAction(DataViewBase view) {
			this.view = view;
		}
		public DataViewBase View { get { return view; } }
		public void Execute() {
			int scrollOffsetDelta = ScrollActionsHelper.GetOffsetDeltaForPageUp(View);
			if(scrollOffsetDelta >= 0) {
				scrollOffsetDelta = -1;
			}
			DataViewBase targetView = null;
			int targetVisibleIndex = 0;
			View.GetFirstScrollRowViewAndVisibleIndex(out targetView, out targetVisibleIndex);
			int targetScrollIndex = targetView.DataControl.FindFirstInnerChildScrollIndex(targetVisibleIndex) + scrollOffsetDelta;
			if(targetScrollIndex <= 0) {
				targetScrollIndex = 0;
			}
			View.MoveFocusedRowToScrollIndexForPageUpPageDown(targetScrollIndex, true);
			View.OnPostponedNavigationComplete(); 
		}
	}
	class ScrollOnePageDownWithAnimationAction : IAction {
		DataViewBase view;
		public ScrollOnePageDownWithAnimationAction(DataViewBase view) {
			this.view = view;
		}
		public DataViewBase View { get { return view; } }
		public void Execute() {
			int scrollOffsetDelta = ScrollActionsHelper.GetOffsetDeltaForPerPixelPageDown(View);
			if(scrollOffsetDelta <= 0) {
				scrollOffsetDelta = 1;
			}
			List<int> fixedRowsScrollIndexes = View.DataControl.GetParentFixedRowsScrollIndexes(View.DataProviderBase.CurrentIndex);
			int targetScrollIndex = ScrollActionsHelper.CalcLastScrollRowFirstInnerChildScrollIndex(View) + scrollOffsetDelta;
			targetScrollIndex = ScrollActionsHelper.FindNearestScrollableRow(View, targetScrollIndex);
			int savedTargeScrollIndex = targetScrollIndex;
			while(fixedRowsScrollIndexes.Contains(targetScrollIndex)) {
				targetScrollIndex = SkipGroupSummaryRows(++targetScrollIndex, true);
			}
			IScrollInfoOwner scrollInfoOwner = View.RootDataPresenter;
			if(targetScrollIndex >= scrollInfoOwner.ItemCount) {
				targetScrollIndex = scrollInfoOwner.ItemCount - 1;
				targetScrollIndex = SkipGroupSummaryRows(targetScrollIndex, false);
				while(fixedRowsScrollIndexes.Contains(targetScrollIndex)) {
					targetScrollIndex = SkipGroupSummaryRows(--targetScrollIndex, false);
				}
			}
			View.MoveFocusedRowToScrollIndexForPageUpPageDown(targetScrollIndex, false);
			View.OnPostponedNavigationComplete(); 
		}
		int SkipGroupSummaryRows(int index, bool forward) {
			while(View.DataProviderBase.GetVisibleIndexByScrollIndex(index) is GroupSummaryRowKey) {
				if(forward)
					index++;
				else
					index--;
			}
			return index;
		}
	}
	abstract class ScrollOneItemAfterPageUpPageDownAction : IAction {
		int tryCount = 0;
		DataViewBase view, initialView;
		int initialVisibleIndex, previousOffsetDelta;
		bool needToAdjustScroll;
		protected DataViewBase View { get { return view; } }
		protected DataViewBase InitialView { get { return initialView; } }
		protected int InitialVisibleIndex { get { return initialVisibleIndex; } }
		protected bool NeedToAdjustScroll { get { return needToAdjustScroll; } }
		protected int PreviousOffsetDelta { get { return previousOffsetDelta; } }
		protected int TryCount { get { return tryCount; } }
		protected abstract int GetRealOffset();
		protected abstract bool IsOffsetDeltaOutOfRange(int offsetDelta);
		protected abstract void MoveFocusedRowToScrollIndex(bool isOffsetDeltaOutOfRange);
		protected abstract void AddScrollOneItemAction(int offsetDelta);
		public ScrollOneItemAfterPageUpPageDownAction(DataViewBase view, DataViewBase initialView, int initialVisibleIndex, bool needToAdjustScroll, int previousOffsetDelta, int tryCount) {
			this.view = view;
			this.initialView = initialView;
			this.initialVisibleIndex = initialVisibleIndex;
			this.needToAdjustScroll = needToAdjustScroll;
			this.previousOffsetDelta = previousOffsetDelta;
			this.tryCount = tryCount;
		}
		protected virtual int GetGroupSummaryRowOffset(int visibleIndex) {
			return 0;
		}
		public void Execute() {
			if(CheckIsBadCondition()) return;
			int desiredScrollIndex = InitialView.DataControl.FindFirstInnerChildScrollIndex(InitialVisibleIndex);
			int offsetDelta = desiredScrollIndex - GetRealOffset() + GetGroupSummaryRowOffset(InitialVisibleIndex);
			bool isOffsetDeltaOutOfRange = IsOffsetDeltaOutOfRange(offsetDelta);
			if((offsetDelta == 0) || isOffsetDeltaOutOfRange || !NeedToAdjustScroll || (PreviousOffsetDelta * offsetDelta < 0)) {
				MoveFocusedRowToScrollIndex(isOffsetDeltaOutOfRange);
				View.OnPostponedNavigationComplete();
				return;
			}
			if(offsetDelta > 0) {
				offsetDelta = 1;
			}
			if(offsetDelta < 0) {
				offsetDelta = -1;
			}
			View.RootDataPresenter.SetDefineScrollOffset(View.RootDataPresenter.ActualScrollOffset + offsetDelta);
			AddScrollOneItemAction(offsetDelta);
		}
		bool CheckIsBadCondition() {
			if(TryCount > 100) {
#if DEBUGTEST
				throw new InvalidOperationException("Cyclic ScrollOneItemAfterPageUpPageDownAction call.");
#else
				return true;
#endif
			}
			return false;
		}
	}
	class ScrollOneItemAfterPageUpAction : ScrollOneItemAfterPageUpPageDownAction {
		protected override int GetRealOffset() {
			return (int)Math.Ceiling(View.RootDataPresenter.ActualScrollOffset) + ScrollActionsHelper.GetOffsetDeltaForPageDown(View);
		}
		protected override bool IsOffsetDeltaOutOfRange(int offsetDelta) {
			IScrollInfoOwner scrollInfoOwner = View.RootDataPresenter;
			return offsetDelta < 0 && scrollInfoOwner.Offset == 0;
		}
		protected override void MoveFocusedRowToScrollIndex(bool isOffsetDeltaOutOfRange) {
			View.MoveFocusedRowToFirstScrollRow();
		}
		protected override void AddScrollOneItemAction(int offsetDelta) {
			View.AddScrollOneItemAfterPageUpAction(InitialView, InitialVisibleIndex, NeedToAdjustScroll, offsetDelta, TryCount + 1);
		}
		protected override int GetGroupSummaryRowOffset(int visibleIndex) {
			return -ScrollActionsHelper.GetGroupSummaryRowCountBeforeRow(View, visibleIndex, NeedToAdjustScroll);
		}
		public ScrollOneItemAfterPageUpAction(DataViewBase view, DataViewBase initialView, int initialVisibleIndex, bool needToAdjustScroll, int previousOffsetDelta, int tryCount)
			: base(view, initialView, initialVisibleIndex, needToAdjustScroll, previousOffsetDelta, tryCount) { }
	}
	class ScrollOneItemAfterPageDownAction : ScrollOneItemAfterPageUpPageDownAction {
		protected override int GetRealOffset() {
			return (int)Math.Ceiling(View.RootDataPresenter.ActualScrollOffset);
		}
		protected override bool IsOffsetDeltaOutOfRange(int offsetDelta) {
			IScrollInfoOwner scrollInfoOwner = View.RootDataPresenter;
			return (offsetDelta > 0) && (scrollInfoOwner.Offset >= scrollInfoOwner.ItemCount - scrollInfoOwner.ItemsOnPage);
		}
		protected override void MoveFocusedRowToScrollIndex(bool isOffsetDeltaOutOfRange) {
			View.MoveFocusedRowToLastScrollRow();
		}
		protected override void AddScrollOneItemAction(int offsetDelta) {
			View.AddScrollOneItemAfterPageDownAction(InitialView, InitialVisibleIndex, NeedToAdjustScroll, offsetDelta, TryCount + 1);
		}
		protected override int GetGroupSummaryRowOffset(int visibleIndex) {
			return ScrollActionsHelper.GetGroupSummaryRowCountAfterRow(View, visibleIndex, NeedToAdjustScroll, true);
		}
		public ScrollOneItemAfterPageDownAction(DataViewBase view, DataViewBase initialView, int initialVisibleIndex, bool needToAdjustScroll, int previousOffsetDelta, int tryCount)
			: base(view, initialView, initialVisibleIndex, needToAdjustScroll, previousOffsetDelta, tryCount) { }
	}
	class ScrollOneItemAfterPageDownPerPixelAction : ScrollOneItemAfterPageDownAction {
		public ScrollOneItemAfterPageDownPerPixelAction(DataViewBase view, DataViewBase initialView, int initialVisibleIndex, bool needToAdjustScroll, int previousOffsetDelta, int tryCount)
			: base(view, initialView, initialVisibleIndex, needToAdjustScroll, previousOffsetDelta, tryCount) { }
		protected override void MoveFocusedRowToScrollIndex(bool isOffsetDeltaOutOfRange) {
			if(!isOffsetDeltaOutOfRange) {
				AdjustScrollOffset();
				base.MoveFocusedRowToScrollIndex(isOffsetDeltaOutOfRange);
			}
			else {
				MoveFocusedRowToLastRow();
			}
		}
		void AdjustScrollOffset() {
			DataViewBase targetView = null;
			int targetVisibleIndex = 0;
			View.GetLastScrollRowViewAndVisibleIndex(out targetView, out targetVisibleIndex);
			if(targetView == null || targetVisibleIndex >= View.RootDataPresenter.ItemCount - 1)
				return;
			double offsetDelta = View.CalcOffsetForBackwardScrolling(targetVisibleIndex + 1);
			View.RootDataPresenter.SetDefineScrollOffset(View.RootDataPresenter.ActualScrollOffset - offsetDelta);
		}
		void MoveFocusedRowToLastRow() {
			DataViewBase lastInnerDetailView = View.RootView.DataControl.FindLastInnerDetailView();
			DataViewBase lastView = View;
			if(lastInnerDetailView != null && lastInnerDetailView.DataControl.VisibleRowCount > 0)
				lastView = lastInnerDetailView;
			lastView.MoveLastRow();
		}
	}
	class FocusFirstRowAfterPageDownCardView : IAction {
		DataViewBase view;
		public FocusFirstRowAfterPageDownCardView(DataViewBase view) {
			this.view = view;
		}
		public void Execute() {
			view.MoveFocusedRowToLastScrollRow();
		}
	}
	class ScrollAndFocusFirstAfterPageUpCardView : IAction {
		DataViewBase view;
		int visibleIndex;
		int tryCount;
		public ScrollAndFocusFirstAfterPageUpCardView(DataViewBase view, int visibleIndex, int tryCount) {
			this.view = view;
			this.visibleIndex = visibleIndex;
			this.tryCount = tryCount;
		}
		CardsHierarchyPanel CardsHierarchyPanel { get { return (CardsHierarchyPanel)view.DataPresenter.Panel; } }
		ScrollInfoBase ScrollInfo { get { return view.DataPresenter.ScrollInfoCore.DefineSizeScrollInfo; } }
		public void Execute() {
			int lastPageIndex = (int)(ScrollInfo.Offset + ScrollInfo.Viewport - 1);
			int visibleRowIndex = (int)CardsHierarchyPanel.CalcExtent(view.SizeHelper.GetDefineSize(view.DataPresenter.LastConstraint), visibleIndex);
			double offset = visibleRowIndex - lastPageIndex;
			if(offset == 0 || ScrollInfo.Viewport < 1 || tryCount > 100) {
				view.MoveFocusedRowToFirstScrollRow();
				return;
			}
			if(offset > 0) {
				offset = 1;
			}
			ScrollInfo.SetOffset(ScrollInfo.Offset + offset);
			view.EnqueueImmediateAction(new ScrollAndFocusFirstAfterPageUpCardView(view, visibleIndex, ++tryCount));
		}
	}
	class ScrollRowIntoViewAction : IAction {
		DataViewBase view;
		int rowHandle;
		int tryCount = 0;
		bool prohibitAnimation;
		public ScrollRowIntoViewAction(DataViewBase view, int rowHandle, int tryCount) {
			this.view = view;
			this.rowHandle = rowHandle;
			this.tryCount = tryCount;
			prohibitAnimation = View.ScrollAnimationLocker.IsLocked || View.DataControl.IsDataResetLocked;
		}
		protected DataViewBase View { get { return view; } }
		protected int RowHandle { get { return rowHandle; } }
		protected int FirstInnerScrollIndex { get; set; }
		protected int TopNewItemRowScrollIndex { get; set; }
		protected bool ProhibitAnimation { get { return prohibitAnimation; } }
		bool IsTopNewItemRow { get { return View.IsNewItemRowVisible && RowHandle == DataControlBase.NewItemRowHandle; } }
		public void Reassign(DataViewBase view, int rowHandle) {
			this.view = view;
			this.rowHandle = rowHandle;
		}
		public void Execute() {
			if(!View.DataControl.IsValidRowHandleCore(RowHandle) || !View.DataControl.IsRowVisibleCore(RowHandle))
				return;
			if(IsTopNewItemRow && View.IsRootView)
				return;
			#region Cyclic ScrollIntoView call
			if(tryCount > 8) {
#if DEBUGTEST
				throw new InvalidOperationException("Cyclic ScrollIntoView call.");
#else
				return;
#endif
			}
			#endregion
			if(View.RootDataPresenter == null) {
				EnqueueScrollIntoViewAgain();
				return;
			}
			UpdateScrollIndexes();
			View.EditFormManager.OnBeforeScroll(RowHandle);
			if(ProhibitAnimation) View.ScrollAnimationLocker.Lock();
			try {
				ScrollIntoViewCore();
			} finally {
				if(ProhibitAnimation) View.ScrollAnimationLocker.Unlock();
				View.EditFormManager.OnAfterScroll();
			}
			if(!View.RootDataPresenter.CanScrollWithAnimation) {
				if(ShouldScrollBack() || ShouldScrollForward()) {
					EnqueueScrollIntoViewAgain();
				}
			}
		}
		void UpdateScrollIndexes() {
			FirstInnerScrollIndex = FindFirstInnerChildScrollIndex();
			if(IsTopNewItemRow)
				TopNewItemRowScrollIndex = FindTopNewItemRowScrollIndex();
		}
		int FindFirstInnerChildScrollIndex() {
			if(IsTopNewItemRow)
				return View.DataControl.FindFirstInnerChildScrollIndex();
			int visibleIndex = View.DataControl.GetRowVisibleIndexByHandleCore(RowHandle);
			return View.DataControl.FindFirstInnerChildScrollIndex(visibleIndex);
		}
		int FindTopNewItemRowScrollIndex() {
			if(View.DataControl.VisibleRowCount > 0)
				return View.DataControl.FindFirstInnerChildScrollIndex(View.DataControl.VisibleRowCount - 1) + 1;
			return View.DataControl.FindFirstInnerChildScrollIndex();
		}
		void EnqueueScrollIntoViewAgain() {
			View.EnqueueImmediateAction(new ScrollRowIntoViewAction(View, RowHandle, tryCount + 1));
		}
		void ScrollIntoViewCore() {
			if(ShouldScrollBack()) {
				ScrollBack();
				return;
			}
			if(ShouldScrollForward()) {
				ScrollForward();
				return;
			}
		}
		bool ShouldScrollBack() {
			double scrollOffset = View.RootDataPresenter.ActualScrollOffset;
			return (GetBackScrollIndex() < scrollOffset) && (scrollOffset > 0);
		}
		int GetBackScrollIndex() {
			if(IsTopNewItemRow)
				return TopNewItemRowScrollIndex;
			return FirstInnerScrollIndex;
		}
		bool ShouldScrollForward() {
			IScrollInfoOwner scrollInfoOwner = View.RootDataPresenter;
			if(GetForwardScrollIndex() <= scrollInfoOwner.Offset) return false;
			if(IsRowVisibleOnScreen(View)) return false;
			int targetRowTotalLevel = View.DataControl.CalcTotalLevelByRowHandle(RowHandle);
			int lastScrollRowTotalLevel = ScrollActionsHelper.CalcLastScrollRowTotalLevel(View);
			if(targetRowTotalLevel > lastScrollRowTotalLevel) {
				return true;
			}
			return GetForwardScrollIndex() > View.CalcLastScrollRowScrollIndex();
		}
		int GetForwardScrollIndex() {
			return FirstInnerScrollIndex;
		}
		bool IsRowVisibleOnScreen(DataViewBase view) {
			int rowHandle = RowHandle;
			DataPresenterBase dataPresenter = View.RootDataPresenter;
			FrameworkElement rowElement = GetRowElement();
			if(rowElement != null && rowElement.IsVisible) {
				Rect elementRect = LayoutHelper.GetRelativeElementRect(rowElement, dataPresenter);
				if(dataPresenter.IsElementPartiallyVisible(dataPresenter.LastConstraint, elementRect)) {
					return false;
				}
				return true;
			}
			return dataPresenter.LastConstraint.Height == Double.PositiveInfinity;
		}
		FrameworkElement GetRowElement() {
			if(IsTopNewItemRow)
				return View.GetRowElementByRowHandle(DataControlBase.NewItemRowHandle);
			GridRowsEnumerator gridRowsEnumerator = view.RootView.CreateVisibleRowsEnumerator();
			while(gridRowsEnumerator.MoveNext()) {
				RowData rowData = gridRowsEnumerator.CurrentRowData as RowData;
				if((rowData != null) && (rowData.View == view) && (rowData.RowHandle.Value == rowHandle)) {
					return gridRowsEnumerator.CurrentRow;
				}
			}
			return null;
		}
		void ScrollBack() {
			View.RootDataPresenter.SetDefineScrollOffset(GetBackScrollIndex());
		}
		void ScrollForward() {
			double newOffset = 0.0;
			double viewPort = View.RootDataPresenter.ActualViewPort;
			if(View.ShouldChangeForwardIndex(RowHandle)) {
				IScrollInfoOwner scrollInfoOwner = View.RootDataPresenter;
				double newOffsetDelta = 1.0;
				if(viewPort > 1.0) {
					newOffsetDelta = View.CalcOffsetForward(RowHandle, View.ViewBehavior.AllowPerPixelScrolling);
					if(!ScrollActionsHelper.IsRowElementVisible(view, RowHandle))
						newOffsetDelta += ScrollActionsHelper.GetGroupSummaryRowCountBeforeRowByRowHandle(View, RowHandle, true);
				}
				newOffset = scrollInfoOwner.Offset + newOffsetDelta;
			}
			else {
				double itemsCount = Math.Max(viewPort, 1.0);
				newOffset = GetForwardScrollIndex() - itemsCount + View.DataControl.CalcTotalLevelByRowHandle(RowHandle) + 1;
			}
			newOffset += ScrollActionsHelper.GetGroupSummaryRowCountAfterRowByRowHandle(View, RowHandle, false);
			View.RootDataPresenter.SetDefineScrollOffset(newOffset);
		}
	}
	static class ScrollActionsHelper {
		internal static int GetOffsetDeltaForPageUp(DataViewBase view) {
			return -view.RootDataPresenter.FullyVisibleItemsCount + CalcFirstScrollRowTotalLevel(view) + 1;
		}
		internal static int GetOffsetDeltaForPageDown(DataViewBase view) {
			return view.RootDataPresenter.FullyVisibleItemsCount - CalcLastScrollRowTotalLevel(view) - 1;
		}
		internal static int GetOffsetDeltaForPerPixelPageDown(DataViewBase view) {
			int offsetDelta = view.RootDataPresenter.FullyVisibleItemsCount - 1;
			if(!view.RootDataPresenter.IsAnimationInProgress) {
				offsetDelta -= CalcLastScrollRowFirstInnerChildTotalLevel(view);
			}
			return offsetDelta;
		}
		static int CalcFirstScrollRowTotalLevel(DataViewBase view) {
			DataViewBase firstDataRowView = null;
			int firstDataRowVisibleIndex = 0;
			view.DataControl.FindViewAndVisibleIndexByScrollIndex(view.CalcFirstScrollRowScrollIndex(), true, out firstDataRowView, out firstDataRowVisibleIndex);
			if(firstDataRowView == null) {
				return 0;
			}
			int totalLevel = firstDataRowView.DataControl.CalcTotalLevel(firstDataRowVisibleIndex);
			if(firstDataRowView.DataControl.IsExpandedFixedRow(firstDataRowVisibleIndex))
				totalLevel++;
			return totalLevel;
		}
		internal static int CalcLastScrollRowTotalLevel(DataViewBase view) {
			DataViewBase lastDataRowView = null;
			int lastDataRowVisibleIndex = 0;
			view.GetLastScrollRowViewAndVisibleIndex(out lastDataRowView, out lastDataRowVisibleIndex);
			if(lastDataRowView == null) {
				return 0;
			}
			return lastDataRowView.DataControl.CalcTotalLevel(lastDataRowVisibleIndex);
		}
		static int CalcLastScrollRowFirstInnerChildTotalLevel(DataViewBase view) {
			int firstInnerChildScrollIndex = CalcLastScrollRowFirstInnerChildScrollIndex(view);
			DataViewBase firstInnerChildView = null;
			int firstInnerChildVisibleIndex = 0;
			if(view.DataControl.FindViewAndVisibleIndexByScrollIndex(firstInnerChildScrollIndex, true, out firstInnerChildView, out firstInnerChildVisibleIndex))
				return firstInnerChildView.DataControl.CalcTotalLevel(firstInnerChildVisibleIndex);
			return 0;
		}
		internal static int CalcLastScrollRowFirstInnerChildScrollIndex(DataViewBase view) {
			if(view.RootDataPresenter.IsAnimationInProgress) {
				return (int)Math.Ceiling(view.RootDataPresenter.ActualScrollOffset) + view.RootDataPresenter.FullyVisibleItemsCount - 1;
			} else {
				DataViewBase lastDataRowView = null;
				int lastDataRowVisibleIndex = 0;
				view.GetLastScrollRowViewAndVisibleIndex(out lastDataRowView, out lastDataRowVisibleIndex);
				if(lastDataRowView == null) {
					return 0;
				} else {
					return lastDataRowView.DataControl.FindFirstInnerChildScrollIndex(lastDataRowVisibleIndex);
				}
			}
		}
		internal static int FindNearestScrollableRow(DataViewBase targetView, int scrollIndex) {
			int result = -1;
			for(int i = scrollIndex; i < targetView.DataProviderBase.VisibleCount + targetView.CalcGroupSummaryVisibleRowCount(); i++) {
				object visibleIndex = targetView.DataProviderBase.GetVisibleIndexByScrollIndex(i);
				if(visibleIndex is int) {
					result = i;
					break;
				}
			}
			if(result < 0) {
				for(int i = scrollIndex; i >= 0; i--) {
					object visibleIndex = targetView.DataProviderBase.GetVisibleIndexByScrollIndex(i);
					if(visibleIndex is int) {
						result = i;
						break;
					}
				}
			}
			return result < 0 ? scrollIndex : result;
		}
		internal static int GetGroupSummaryRowCountAfterRowByRowHandle(DataViewBase targetView, int rowHandle, bool needToAdjustScroll, bool checkLastRow = false) {
			int visibleIndex = targetView.DataControl.GetRowVisibleIndexByHandleCore(rowHandle);
			return GetGroupSummaryRowCountAfterRow(targetView, visibleIndex, needToAdjustScroll, checkLastRow);
		}
		internal static int GetGroupSummaryRowCountAfterRow(DataViewBase targetView, int visibleIndex, bool needToAdjustScroll, bool checkLastRow = false) {
			int count = CalcGroupSummaryRowCountAfterRow(targetView, visibleIndex);
			if(count == 0) return 0;
			if(checkLastRow && visibleIndex == targetView.DataProviderBase.VisibleCount - 1) 
				return -1;
			return count + (needToAdjustScroll ? 1 : 0);
		}
		internal static int GetGroupSummaryRowCountBeforeRowByRowHandle(DataViewBase targetView, int rowHandle, bool needToAdjustScroll) {
			int visibleIndex = targetView.DataControl.GetRowVisibleIndexByHandleCore(rowHandle);
			return GetGroupSummaryRowCountBeforeRow(targetView, visibleIndex, needToAdjustScroll);
		}
		internal static int GetGroupSummaryRowCountBeforeRow(DataViewBase targetView, int visibleIndex, bool needToAdjustScroll) {
			int count = CalcGroupSummaryRowCountBeforeRow(targetView, visibleIndex);
			if(count == 0) return 0;
			if(visibleIndex == 0)
				return -1;
			return count + (needToAdjustScroll ? 1 : 0);
		}
		static int CalcGroupSummaryRowCountBeforeRow(DataViewBase view, int visibleIndex) {
			if(!view.ShowGroupSummaryFooter) return 0;
			int scrollIndex = view.DataProviderBase.ConvertVisibleIndexToScrollIndex(visibleIndex, false);
			int count = 0;
			for(int i = scrollIndex - 1; i >= 0; i--) {
				GroupSummaryRowKey key = view.DataProviderBase.GetVisibleIndexByScrollIndex(i) as GroupSummaryRowKey;
				if(key != null)
					count++;
				else
					break;
			}
			return count;
		}
		static int CalcGroupSummaryRowCountAfterRow(DataViewBase view, int visibleIndex) {
			if(!view.ShowGroupSummaryFooter) return 0;
			int scrollIndex = view.DataProviderBase.ConvertVisibleIndexToScrollIndex(visibleIndex, false);
			int count = 0;
			for(int i = scrollIndex + 1; i < view.DataProviderBase.VisibleCount + view.CalcGroupSummaryVisibleRowCount(); i++) {
				GroupSummaryRowKey key = view.DataProviderBase.GetVisibleIndexByScrollIndex(i) as GroupSummaryRowKey;
				if(key != null)
					count++;
				else
					break;
			}
			return count;
		}
		internal static bool IsRowElementVisible(DataViewBase view, int rowHandle) {
			return view.GetRowElementByRowHandle(rowHandle) != null;
		}
	}
}
