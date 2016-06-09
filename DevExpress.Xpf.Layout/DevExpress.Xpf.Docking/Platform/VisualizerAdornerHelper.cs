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
using System.Windows.Documents;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Layout.Core;
namespace DevExpress.Xpf.Docking.Platform {
	public class VisualizerAdornerHelper : IDisposable {
		bool isDisposing;
		bool fUseAdornerWindow;
		AdornerWindowHelper helper;
		DockingHintAdorner dockingHintAdornerCore;
		TabHeadersAdorner tabHeadersAdornerCore;
		ShadowResizeAdorner shadowResizeAdornerCore;
		System.Windows.Threading.DispatcherTimer HideTimer;
		public LayoutView View { get; private set; }
		public DockLayoutManager Container { get; private set; }
		public VisualizerAdornerHelper(LayoutView view) {
			View = view;
			Container = view.Container;
			fUseAdornerWindow = !WindowHelper.IsXBAP;
			TryCreateAdornerWindowHelper(view);
			InitTimer();
		}
		public void Dispose() {
			if(!isDisposing) {
				isDisposing = true;
				DestroyTimer();
				Ref.Dispose(ref shadowResizeAdornerCore);
				Ref.Dispose(ref tabHeadersAdornerCore);
				Ref.Dispose(ref dockingHintAdornerCore);
				Ref.Dispose(ref helper);
				Container = null;
				View = null;
			}
			GC.SuppressFinalize(this);
		}
		void InitTimer() {
			delayedActions = new List<Action>();
			HideTimer = InvokeHelper.GetBackgroundTimer(180);
			HideTimer.Tick += HideTimer_Tick;
		}
		void DestroyTimer() {
			HideTimer.Tick -= HideTimer_Tick;
			delayedActions.Clear();
		}
		protected void TryCreateAdornerWindowHelper(LayoutView view) {
			if(fUseAdornerWindow)
				helper = CreateAdornerWindowHelper(view);
		}
		UIElement TryGetAdornerWindowRoot(UIElement adornerWindowRoot) {
			return (helper.AdornerWindow != null) ? helper.AdornerWindow.RootElement : null;
		}
		AdornerWindowHelper CreateAdornerWindowHelper(LayoutView view) {
			return new AdornerWindowHelper(view, Container);
		}
		protected UIElement GetAdornedElement() {
			UIElement adornerWindowRoot = null;
			if(fUseAdornerWindow) {
				adornerWindowRoot = TryGetAdornerWindowRoot(adornerWindowRoot);
			}
			return (adornerWindowRoot != null) ? adornerWindowRoot : View.RootUIElement;
		}
		public RenameController GetRenameController() {
			if(dockingHintAdornerCore == null) return null;
			return dockingHintAdornerCore.RenameController;
		}
		public DockingHintAdorner GetDockingHintAdorner() {
			if(dockingHintAdornerCore == null || dockingHintAdornerCore.IsDisposing) {
				UIElement adornedElement = GetAdornedElement();
				dockingHintAdornerCore = FindAdorner<DockingHintAdorner>(adornedElement);
				if(dockingHintAdornerCore == null) {
					dockingHintAdornerCore = View.CreateDockingHintAdorner(adornedElement);
					EnsureAdornerActivated(Container.DragAdorner);
					EnsureAdornerActivated(dockingHintAdornerCore);
				}
			}
			return dockingHintAdornerCore;
		}
		public TabHeadersAdorner GetTabHeadersAdorner() {
			if(tabHeadersAdornerCore == null) {
				UIElement adornedElement = GetAdornedElement();
				tabHeadersAdornerCore = FindAdorner<TabHeadersAdorner>(adornedElement);
				if(tabHeadersAdornerCore == null) {
					tabHeadersAdornerCore = View.CreateTabHeadersAdorner(adornedElement);
					EnsureAdornerActivated(Container.DragAdorner);
					EnsureAdornerActivated(tabHeadersAdornerCore);
				}
			}
			return tabHeadersAdornerCore;
		}
		public ShadowResizeAdorner GetShadowResizeAdorner() {
			if(shadowResizeAdornerCore == null) {
				UIElement adornedElement = GetAdornedElement();
				shadowResizeAdornerCore = FindAdorner<ShadowResizeAdorner>(adornedElement);
				if(shadowResizeAdornerCore == null) {
					shadowResizeAdornerCore = View.CreateShadowResizeAdorner(adornedElement);
					EnsureAdornerActivated(Container.DragAdorner);
					EnsureAdornerActivated(shadowResizeAdornerCore);
				}
			}
			return shadowResizeAdornerCore;
		}
		bool fCancelHiding;
		public void TryShowAdornerWindow(bool forceUpdateAdornerBounds = false) {
			fCancelHiding = true;
			if(fUseAdornerWindow) {
				if(Container.IsTransparencyDisabled && View is FloatingView)
					Container.Win32AdornerWindowProvider.CancelHideAdornerWindow();
				helper.ShowAdornerWindow(forceUpdateAdornerBounds);
			}
		}
		public void TryHideAdornerWindow() {
				HideTimer.Stop();
				DoHidingActions();
			if(fUseAdornerWindow)
				helper.HideAdornerWindow();
		}
		List<Action> delayedActions;
		public void BeginHideAdornerWindowAndResetDockingHints() {
			fCancelHiding = false;
			delayedActions.Add(ResetDockingHints);
			if(Container.IsTransparencyDisabled && View is FloatingView) {
				DoHidingActions();
				if(fUseAdornerWindow) Container.Win32AdornerWindowProvider.EnqueueHideAdornerWindow();
			}
			else
				HideTimer.Start();
		}
		public void BeginHideAdornerWindowAndResetTabHeadersHints() {
			fCancelHiding = false;
			delayedActions.Add(ResetTabHeadersHints);
			HideTimer.Start();
		}
		void HideTimer_Tick(object sender, EventArgs e) {
			HideTimer.Stop();
			DoHidingActions();
			if(fUseAdornerWindow && !fCancelHiding)
				helper.HideAdornerWindow();
		}
		public void ResetDragVisualization() {
			Container.CustomizationController.HideDragCursor();
			Container.CustomizationController.UpdateDragInfo(null);
		}
		int lockHidingCounter = 0;
		void DoHidingActions() {
			if(lockHidingCounter > 0) return;
			lockHidingCounter++;
			Action[] actions = delayedActions.ToArray();
			delayedActions.Clear();
			for(int i = 0; i < actions.Length; i++) {
				if(actions[i] != null)
					actions[i]();
			}
			lockHidingCounter--;
		}
		public void ShowSelection() {
			if(!Container.IsCustomization || Container.IsInDesignTime) return;
			TryShowAdornerWindow(true);
			DockingHintAdorner adorner = GetDockingHintAdorner();
			if(adorner != null) {
				adorner.ShowSelectionHints = true;
				adorner.UpdateSelection();
				adorner.Update(true);
			}
		}
		public void HideSelection() {
			if(dockingHintAdornerCore == null) return;
			DockingHintAdorner adorner = GetDockingHintAdorner();
			if(adorner != null) {
				adorner.ShowSelectionHints = false;
				adorner.UpdateSelection();
				adorner.Update(adorner.Visible);
			}
		}
		internal void InvalidateDockingHintsAdorner() {
			if(dockingHintAdornerCore == null) return;
			DockingHintAdorner adorner = dockingHintAdornerCore;
			InvokeHelper.BeginInvoke(adorner, new Action(adorner.Update), InvokeHelper.Priority.Render);
		}
		public void Reset() {
			ResetDockingHints();
			ResetTabHeadersHints();
			Ref.Dispose(ref dockingHintAdornerCore);
			Ref.Dispose(ref tabHeadersAdornerCore);
			if(fUseAdornerWindow)
				helper.Reset();
		}
		public void ResetDockingHints() {
			if(dockingHintAdornerCore == null) return;
			DockingHintAdorner adorner = GetDockingHintAdorner();
			if(adorner != null) {
				adorner.ResetDocking();
				adorner.Update(adorner.Visible);
			}
		}
		public void ResetTabHeadersHints() {
			if(tabHeadersAdornerCore == null) return;
			TabHeadersAdorner adorner = GetTabHeadersAdorner();
			if(adorner != null) {
				adorner.ResetElements();
				adorner.Update(false);
			}
		}
		public DockHintHitInfo GetHitInfo(Point point) {
			if(dockingHintAdornerCore == null) return null;
			DockingHintAdorner adorner = GetDockingHintAdorner();
			return (adorner != null) ? adorner.HitTest(point) : null;
		}
		public void UpdateTabHeadersHint(DockLayoutElementDragInfo dragInfo, bool forceUpdateAdornerBounds = false) {
			delayedActions.Remove(ResetTabHeadersHints);
			TryShowAdornerWindow(forceUpdateAdornerBounds);
			TabHeadersAdorner adorner = GetTabHeadersAdorner();
			if(adorner == null || dragInfo.Item == null) return;
			IDockLayoutContainer container = dragInfo.DropTarget as IDockLayoutContainer;
			if(container != null && container.HasHeadersPanel) {
				TabHeaderInsertHelper helper = new TabHeaderInsertHelper(container, dragInfo.Point, container.Item != dragInfo.Item.Parent);
				bool canShowTabHints = CanShowTabHeaderHint(dragInfo, helper.InsertIndex);
				if(canShowTabHints)
					adorner.ShowElements(container.TabHeaderLocation, helper.Tab, helper.Header);
				else adorner.ResetElements();
				adorner.Update(true);
			}
		}
		bool CanShowTabHeaderHint(DockLayoutElementDragInfo dragInfo, int insertIndex) {
			IDockLayoutContainer container = dragInfo.DropTarget as IDockLayoutContainer;
			return container != null ? Container.RaiseShowingTabHintsEvent(dragInfo.Item, container.Item, insertIndex) : false;
		}
		public void UpdateDockingHints(DockLayoutElementDragInfo dragInfo, bool forceUpdateAdornerBounds = false) {
			delayedActions.Remove(ResetDockingHints);
			TryShowAdornerWindow(forceUpdateAdornerBounds);
			DockingHintAdorner adorner = GetDockingHintAdorner();
			if(adorner == null || dragInfo.Item == null) return;
			DockHintHitInfo adornerHitInfo = adorner.HitTest(dragInfo.Point);
			adorner.TargetRect = dragInfo.TargetRect;
			adorner.SetDockHintsConfiguration(dragInfo);
			Container.RaiseShowingDockHintsEvent(dragInfo.Item, dragInfo.Target, adorner.DockHintsConfiguration);
			adorner.UpdateHotTrack(adornerHitInfo);
			adorner.UpdateState();
			if(canDock(dragInfo, adornerHitInfo)) {
				adorner.HintRect = HintRectCalculator.Calc(dragInfo, adornerHitInfo);
			}
			else {
				adorner.HintRect = Rect.Empty;
				adorner.ClearHotTrack();
			}
			adorner.Update(true);
		}
		bool canDock(DockLayoutElementDragInfo dragInfo, DockHintHitInfo adornerHitInfo) {
			return dragInfo.AcceptDocking(adornerHitInfo) &&
					!Container.RaiseItemDockingEvent(
						DockLayoutManager.DockItemDockingEvent,
						dragInfo.Item, dragInfo.Point,
						adornerHitInfo.IsCenter ? dragInfo.Target : dragInfo.Target.GetRoot(),
						adornerHitInfo.DockType,
						adornerHitInfo.IsHideButton
					);
		}
		public static T FindAdorner<T>(UIElement container) where T : BaseSurfacedAdorner {
			AdornerLayer adornerLayer = AdornerHelper.FindAdornerLayer(container);
			if(adornerLayer == null) return null;
			Adorner[] adorners = adornerLayer.GetAdorners(container);
			if(adorners == null) return null;
			return Array.Find(adorners, (a) => a is T) as T;
		}
		public static void EnsureAdornerActivated(BaseSurfacedAdorner adorner) {
			var manager = Core.Native.LayoutHelper.FindParentObject<DockLayoutManager>(adorner.AdornedElement);
			if(manager != null && manager.IsInDesignTime) {
				if(AdornerLayer.GetAdornerLayer(manager) == null) return;
			}
			if((adorner != null) && !adorner.IsActivated)
				adorner.Activate();
		}
		public static double GetAdornerWindowIndent(UIElement element) {
			AdornerWindow aw = Window.GetWindow(element) as AdornerWindow;
			if(aw != null) {
				return aw.GetAdornerIndentWithoutTransform();
			}
			return 0d;
		}
	}
}
