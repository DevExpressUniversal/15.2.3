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
using DevExpress.Xpf.Layout.Core.Base;
using DevExpress.Xpf.Layout.Core.Dragging;
namespace DevExpress.Xpf.Layout.Core.Platform {
	public class LayoutElementHostAdapter : BaseObject, ILayoutElementHostAdapter {
		internal IDictionary<ILayoutElementHost, LayoutElementHitInfo> Context;
		protected override void OnCreate() {
			base.OnCreate();
			Context = CreateContext();
		}
		protected override void OnDispose() {
			Ref.Clear(ref Context);
			base.OnDispose();
		}
		protected virtual IDictionary<ILayoutElementHost, LayoutElementHitInfo> CreateContext() {
			return new Dictionary<ILayoutElementHost, LayoutElementHitInfo>();
		}
		public LayoutElementHitInfo CalcHitInfo(ILayoutElementHost host, Point hitPoint) {
			if(hitPoint == UIService.InvalidPoint) 
				return LayoutElementHitInfo.Empty;
			LayoutElementHitInfo hitInfo = null;
			bool hasValue = Context.TryGetValue(host, out hitInfo);
			if(!hasValue || hitInfo.HitPoint != hitPoint || hitInfo.Element.IsDisposing) {
				Ensure(host);
				hitInfo = CalcHitInfoCore(host, hitPoint);
				if(hasValue) Context[host] = hitInfo;
				else Context.Add(host, hitInfo);
			}
			return hitInfo;
		}
		public bool HitTest(ILayoutElementHost host, Point hitPoint) {
			return HitTestCore(host, hitPoint);
		}
		protected virtual bool HitTestCore(ILayoutElementHost host, Point point) {
			ILayoutElement root = (host != null) ? host.LayoutRoot : null;
			return (root != null) && HitTestRootElements(host, root, point);
		}
		static bool HitTestRootElements(ILayoutElementHost host, ILayoutElement root, Point point) {
			ILayoutElement pane = root.Nodes.Length > 0 ? root.Nodes[0] : null;
			return root.HitTest(point) ||
				(host.Type == HostType.AutoHide) && (pane != null) && pane.HitTest(point);
		}
		protected virtual LayoutElementHitInfo CalcHitInfoCore(ILayoutElementHost host, Point point) {
			ILayoutElement root = (host != null) ? host.LayoutRoot : null;
			if(root == null) return LayoutElementHitInfo.Empty;
			return root.CalcHitInfo(point);
		}
		public ILayoutElementHost ActiveHost {
			get;
			private set;
		}
		int lockEventProcessing;
		public bool IsInEvent {
			get { return lockEventProcessing > 0; }
		}
		public void BeginEvent(ILayoutElementHost host) {
			BeginEventCore(host);
			Context.Clear();
		}
		public void EndEvent() {
			foreach(var pair in Context) {
				Invalidate(pair.Key);
			}
			EndEventCore();
		}
		void BeginEventCore(ILayoutElementHost host) {
			lockEventProcessing++;
			ActiveHost = host;
			Ensure(ActiveHost);
		}
		void EndEventCore() {
			ActiveHost = null;
			--lockEventProcessing;
		}
		protected void Ensure(ILayoutElementHost host) {
			if(host != null && host.LayoutRoot == null) {
				host.EnsureLayoutRoot();
			}
		}
		protected void Invalidate(ILayoutElementHost host) {
			ILayoutElement root = (host != null) ? host.LayoutRoot : null;
			if(root != null) {
				using(var e = root.GetEnumerator()) {
					while(e.MoveNext())
						e.Current.Invalidate();
				}
			}
		}
	}
	public class ViewAdapter : LayoutElementHostAdapter, IViewAdapter {
		ViewCollection viewsCore;
		IDragService dragServiceCore;
		ISelectionService selectionServiceCore;
		IUIInteractionService uiInteractionServiceCore;
		IActionService actionServiceCore;
		IContextActionService contextActionService;
		protected override void OnCreate() {
			base.OnCreate();
			viewsCore = CreateViews();
		}
		protected override void OnDispose() {
			Ref.Dispose(ref actionServiceCore);
			Ref.Dispose(ref uiInteractionServiceCore);
			Ref.Dispose(ref selectionServiceCore);
			Ref.Dispose(ref dragServiceCore);
			Ref.Dispose(ref viewsCore);
			Ref.Dispose(ref contextActionService);
			NotificationSource = null;
			base.OnDispose();
		}
		protected virtual ViewCollection CreateViews() {
			return new ViewCollection(this);
		}
		public object NotificationSource { get; protected set; }
		public ViewCollection Views {
			get { return viewsCore; }
		}
		public IDragService DragService {
			[System.Diagnostics.DebuggerStepThrough]
			get {
				if(dragServiceCore == null)
					dragServiceCore = ResolveDragService();
				return dragServiceCore; 
			}
		}
		public ISelectionService SelectionService {
			[System.Diagnostics.DebuggerStepThrough]
			get {
				if(selectionServiceCore == null)
					selectionServiceCore = ResolveSelectionService();
				return selectionServiceCore;
			}
		}
		public IUIInteractionService UIInteractionService {
			[System.Diagnostics.DebuggerStepThrough]
			get {
				if(uiInteractionServiceCore == null)
					uiInteractionServiceCore = ResolveUIInteractionService();
				return uiInteractionServiceCore;
			}
		}
		public IActionService ActionService {
			[System.Diagnostics.DebuggerStepThrough]
			get {
				if(actionServiceCore == null)
					actionServiceCore = ResolveActionService();
				return actionServiceCore;
			}
		}
		public IContextActionService ContextActionService {
			[System.Diagnostics.DebuggerStepThrough]
			get {
				if(contextActionService == null)
					contextActionService = ResolveContextActionService();
				return contextActionService;
			}
		}
		protected virtual IDragService ResolveDragService() {
			return new Dragging.DragService();
		}
		protected virtual ISelectionService ResolveSelectionService() {
			return new Selection.SelectionService();
		}
		protected virtual IActionService ResolveActionService() {
			return new Actions.ActionService();
		}
		protected virtual IUIInteractionService ResolveUIInteractionService() {
			return new UIInteraction.UIInteractionService();
		}
		protected virtual IContextActionService ResolveContextActionService() {
			return new Actions.ContextActionService();
		}
		public void ProcessMouseEvent(IView view, MouseEventType eventType, MouseEventArgs ea) {
			if(IsDisposing || IsInEvent) return;
			BeginEvent(view);
			if(!UIInteractionService.ProcessMouse(view, eventType, ea)) {
				SelectionService.ProcessMouse(view, eventType, ea);
				DragService.ProcessMouse(view, eventType, ea);
			}
			EndEvent();
		}
		public void ProcessKey(IView view, Platform.KeyEventType eventype, System.Windows.Input.Key key) {
			if(IsDisposing || IsInEvent) return;
			BeginEvent(view);
			if(!UIInteractionService.ProcessKey(view, eventype, key)) {
				SelectionService.ProcessKey(view, eventype, key);
				DragService.ProcessKey(view, eventype, key);
			}
			EndEvent();
		}
		public void ProcessAction(ViewAction action) {
			if(IsDisposing) return;
			IView[] views = GetViewsSortedByZOrder();
			for(int i = 0; i < views.Length; i++) {
				IView view = views[i];
				ProcessActionCore(view, action);
			}
		}
		public void ProcessAction(IView view, ViewAction action) {
			if(IsDisposing) return;
			ProcessActionCore(view, action);
		}
		protected void ProcessActionCore(IView view, ViewAction action) {
			if(view == null) return;
			switch(action) {
				case ViewAction.Hiding: ActionService.Hide(view, false); break;
				case ViewAction.Hide: ActionService.Hide(view, true); break;
				case ViewAction.ShowSelection: ActionService.ShowSelection(view); break;
				case ViewAction.HideSelection: ActionService.HideSelection(view); break;
			}
		}
		public IView GetView(object rootKey) {
			IView[] views = GetViewsSortedByZOrder();
			for(int i = 0; i < views.Length; i++) {
				IView view = views[i];
				if(view.RootKey == rootKey) return view;
			}
			return null;
		}
		public IView GetView(ILayoutElement element) {
			ILayoutElement root = ElementHelper.GetRoot(element);
			IView[] views = GetViewsSortedByZOrder();
			for(int i = 0; i < views.Length; i++) {
				IView view = views[i];
				if(view.LayoutRoot == root) return view;
			}
			return null;
		}
		public IView GetView(Point screenPoint) {
			IView[] views = GetViewsSortedByZOrder();
			for(int i = 0; i < views.Length; i++) {
				IView view = views[i];
				Ensure(view);
				if(view.IsActiveAndCanProcessEvent && view.ZOrder != -1) {
					Point viewPoint = view.ScreenToClient(screenPoint);
					if(HitTest(view, viewPoint))
						return view;
				}
			}
			return null;
		}
		public IView GetBehindView(IView source, Point screenPoint) {
			return GetBehindViewOverride(source, screenPoint);
		}
		protected virtual IView GetBehindViewOverride(IView source, Point screenPoint) {
			if(source == null) return null;
			IView[] views = GetViewsSortedByZOrder();
			bool sourceFounded = false;
			for(int i = 0; i < views.Length; i++) {
				IView view = views[i];
				if(!sourceFounded) {
					sourceFounded = (view == source);
					continue;
				}
				if(sourceFounded) {
					Ensure(view);
					if(view.IsActiveAndCanProcessEvent) {
						Point viewPoint = view.ScreenToClient(screenPoint);
						if(HitTest(view, viewPoint))
							return view;
					}
				}
			}
			return null;
		}
		public Point GetBehindViewPoint(IView source, IView behindView, Point screenPoint) {
			return GetBehindViewPointOverride(source, behindView, screenPoint);
		}
		protected virtual Point GetBehindViewPointOverride(IView source, IView behindView, Point screenPoint) {
			return behindView.ScreenToClient(screenPoint);
		}
		IView[] GetViewsSortedByZOrder() {
			IView[] views = Views.ToArray();
			var keys = new KeyValuePair<int, IView>[views.Length];
			for(var i = 0; i < views.Length; i++)
				keys[i] = new KeyValuePair<int, IView>(i, views[i]);
			new StableSortingHelper(keys, views);
			return views;
		}
		class StableSortingHelper : IComparer<KeyValuePair<int, IView>> {
			KeyValuePair<int, IView>[] keys;
			public StableSortingHelper(KeyValuePair<int, IView>[] keys, IView[] views) {
				this.keys = keys;
				Array.Sort(keys, views, this);
			}
			int IComparer<KeyValuePair<int, IView>>.Compare(KeyValuePair<int, IView> pair1, KeyValuePair<int, IView> pair2) {
				if(pair1.Value == pair2.Value) return 0;
				int result = CompareZOrder(pair1.Value.ZOrder, pair2.Value.ZOrder);
				return result == 0 ? CompareZOrder(pair1.Key, pair2.Key) : result;
			}
			static int CompareZOrder(int order1, int order2) {
				return order2.CompareTo(order1);
			}
		}
	}
}
