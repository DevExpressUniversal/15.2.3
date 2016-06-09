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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraBars.Docking2010.Base;
namespace DevExpress.XtraBars.Docking2010.DragEngine {
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
			if(hitPoint == UIService.InvalidPoint || Context == null)
				return LayoutElementHitInfo.Empty;
			LayoutElementHitInfo hitInfo = null;
			bool hasValue = Context.TryGetValue(host, out hitInfo);
			if(!hasValue || (hitInfo.HitPoint != hitPoint) || (hitInfo.Element != null && hitInfo.Element.IsDisposing)) {
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
			return (root != null) && root.HitTest(point);
		}
		protected virtual LayoutElementHitInfo CalcHitInfoCore(ILayoutElementHost host, Point point) {
			ILayoutElement root = (host != null) ? host.LayoutRoot : null;
			if(root == null) return LayoutElementHitInfo.Empty;
			return root.CalcHitInfo(point);
		}
		ILayoutElementHost activeHostCore;
		public ILayoutElementHost ActiveHost {
			get { return activeHostCore; }
			private set { activeHostCore = value; }
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
			if(IsDisposing) return;
			foreach(KeyValuePair<ILayoutElementHost, LayoutElementHitInfo> pair in Context) {
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
				using(IEnumerator<ILayoutElement> e = root.GetEnumerator()) {
					while(e.MoveNext())
						e.Current.Invalidate();
				}
			}
		}
	}
	public class UIViewAdapter : LayoutElementHostAdapter, IUIViewAdapter {
		IComparer<IUIView> viewComparer;
		UIViewCollection viewsCore;
		IDragService dragServiceCore;
		IUIInteractionService uiInteractionServiceCore;
		public UIViewAdapter(IComparer<IUIView> comparer) {
			this.viewComparer = comparer;
		}
		protected override void OnCreate() {
			base.OnCreate();
			viewsCore = CreateViews();
		}
		protected override void OnDispose() {
			if(dragServiceCore != null)
				dragServiceCore.Reset();
			dragServiceCore = null;
			if(uiInteractionServiceCore != null)
				uiInteractionServiceCore.Reset();
			uiInteractionServiceCore = null;
			Ref.Dispose(ref viewsCore);
			base.OnDispose();
		}
		protected virtual UIViewCollection CreateViews() {
			return new UIViewCollection(this);
		}
		public UIViewCollection Views {
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
		public IUIInteractionService UIInteractionService {
			[System.Diagnostics.DebuggerStepThrough]
			get {
				if(uiInteractionServiceCore == null)
					uiInteractionServiceCore = ResolveUIInteractionService();
				return uiInteractionServiceCore;
			}
		}
		protected virtual IDragService ResolveDragService() {
			return new DragService();
		}
		protected virtual IUIInteractionService ResolveUIInteractionService() {
			return new UIInteractionService();
		}
		public void ProcessMouseEvent(IUIView view, MouseEventType eventType, MouseEventArgs ea) {
			if(IsDisposing || IsInEvent) return;
			BeginEvent(view);
			try {
				if(!UIInteractionService.ProcessMouse(view, eventType, ea)) {
					DragService.ProcessMouse(view, eventType, ea);
				}
			}
			finally { EndEvent(); }
		}
		public void ProcessKey(IUIView view, KeyEventType eventType, Keys key) {
			if(IsDisposing || IsInEvent) return;
			BeginEvent(view);
			try {
				if(!UIInteractionService.ProcessKey(view, eventType, key)) {
					DragService.ProcessKey(view, eventType, key);
				}
			}
			finally { EndEvent(); }
		}
		public bool ProcessFlickEvent(IUIView view, Point point, DevExpress.Utils.Gesture.FlickGestureArgs args) {
			if(IsDisposing || IsInEvent) return false;
			bool result = false;
			BeginEvent(view);
			try {
				result = UIInteractionService.ProcessFlickEvent(view, point, args);
			}
			finally { EndEvent(); }
			return result;
		}
		public void ProcessGesture(IUIView view, GestureID gid, DevExpress.Utils.Gesture.GestureArgs args, object[] parameters) {
			if(IsDisposing || IsInEvent) return;
			BeginEvent(view);
			try {
				UIInteractionService.ProcessGesture(view, gid, args, parameters);
			}
			finally { EndEvent(); }
		}
		public IUIView GetView(object rootKey) {
			IUIView[] views = Views.ToArray();
			for(int i = 0; i < views.Length; i++) {
				IUIView view = views[i];
				if(object.Equals(view.RootKey, rootKey)) return view;
			}
			return null;
		}
		public IUIView GetView(ILayoutElement element) {
			ILayoutElement root = ElementHelper.GetRoot(element);
			IUIView[] views = Views.ToArray();
			for(int i = 0; i < views.Length; i++) {
				IUIView view = views[i];
				if(view.LayoutRoot == root) return view;
			}
			return null;
		}
		public IUIView GetView(Point screenPoint) {
			IUIView[] views = GetZOrderedViews();
			for(int i = 0; i < views.Length; i++) {
				IUIView view = views[i];
				Ensure(view);
				if(view.IsActiveAndCanProcessEvent) {
					Point viewPoint = view.ScreenToClient(screenPoint);
					if(HitTest(view, viewPoint))
						return view;
				}
			}
			return null;
		}
		public IUIView GetBehindView(IUIView source, Point screenPoint) {
			if(source == null) return null;
			IUIView[] views = GetZOrderedViews();
			bool sourceFounded = false;
			for(int i = 0; i < views.Length; i++) {
				IUIView view = views[i];
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
		IUIView[] GetZOrderedViews() {
			IUIView[] views = Views.ToArray();
			if(views.Length > 1) {
				KeyValuePair<IntPtr, IUIView>[] pairs = new KeyValuePair<IntPtr, IUIView>[views.Length];
				for(int i = 0; i < pairs.Length; i++)
					pairs[i] = new KeyValuePair<IntPtr, IUIView>(views[i].ZHandle, views[i]);
				Dragging.ZOrderHelper.Sort(pairs, views, viewComparer);
			}
			return views;
		}
	}
}
