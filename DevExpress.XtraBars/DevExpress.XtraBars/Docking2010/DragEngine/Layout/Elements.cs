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
using System.Drawing;
using System.Collections.Generic;
using DevExpress.XtraBars.Docking2010.Base;
namespace DevExpress.XtraBars.Docking2010.DragEngine {
	public class LayoutElementCollection : BaseMutableListEx<ILayoutElement> {
		ILayoutContainer ownerCore;
		public LayoutElementCollection(ILayoutContainer owner) {
			ownerCore = owner;
		}
		protected override void OnDispose() {
			ownerCore = null;
			base.OnDispose();
		}
		protected override void OnElementAdded(ILayoutElement element) {
			base.OnElementAdded(element);
			AffinityHelper.SetAffinity(ownerCore, element);
		}
		protected override void OnElementRemoved(ILayoutElement element) {
			AffinityHelper.SetAffinity(null, element);
			base.OnElementRemoved(element);
		}
		#region internal classes
		static class AffinityHelper {
			public static void SetAffinity(ILayoutContainer parent, ILayoutElement element) {
				if(AffinityHelperException.Assert(element)) {
					((BaseLayoutElement)element).parentCore = parent;
				}
			}
		}
		#endregion internal classes
	}
	public abstract class BaseLayoutElement : BaseObject, ILayoutElement {
		protected static readonly ILayoutElement[] EmptyNodes = new ILayoutElement[0];
		internal ILayoutContainer parentCore;
		ILayoutElement[] nodesCore;
		protected override void OnDispose() {
			nodesCore = null;
			if(!IsDragging)
				parentCore = null;
			base.OnDispose();
		}
		bool isDraggingCore;
		public bool IsDragging {
			get { return isDraggingCore; }
			set {
				if(isDraggingCore == value) return;
				isDraggingCore = value;
				if(!isDraggingCore)
					OnResetIsDragging();
			}
		}
		protected virtual void OnResetIsDragging() {
			if(IsDisposing)
				parentCore = null;
		}
		public virtual bool HitTestingEnabled {
			get { return true; }
		}
		public bool HitTest(Point pt) {
			return HitTestingEnabled && HitTestCore(pt);
		}
		bool isReadyCore;
		public bool IsReady {
			get { return isReadyCore; }
			private set { isReadyCore = value; }
		}
		Rectangle boundsCore;
		public Rectangle Bounds {
			get { return boundsCore; }
			protected internal set { boundsCore = value; }
		}
		public Point Location {
			get { return boundsCore.Location; }
		}
		public Size Size {
			get { return boundsCore.Size; }
		}
		protected static Rectangle Offset(Rectangle bounds, Point offset) {
			return Offset(bounds, offset.X, offset.Y);
		}
		protected static Rectangle Offset(Rectangle bounds, int x, int y) {
			return new Rectangle(bounds.Left + x, bounds.Top + y, bounds.Width, bounds.Height);
		}
		protected virtual bool HitTestCore(Point pt) {
			return EnsureBounds() && Include(Bounds, pt);
		}
		internal static bool Include(Rectangle r, Point pt) {
			return (r.X <= pt.X) && (pt.X <= r.Right) && (r.Y <= pt.Y) && (pt.Y <= r.Bottom);
		}
		public void Invalidate() {
			IsReady = false;
		}
		bool isVisualStateInitialized;
		object pressedHitResult, hotHitResult;
		public State GetState(object hitResult) {
			if(hitResult == null) return State.Normal;
			EnsureInitialState();
			State result = State.Normal;
			if(HitEquals(hitResult, pressedHitResult))
				result |= State.Pressed;
			if(HitEquals(hitResult, hotHitResult))
				result |= State.Hot;
			return result;
		}
		public void SetState(object hitResult, State state) {
			if(IsDisposing) return;
			EnsureInitialState();
			if((state & State.Pressed) != 0)
				SetStateCore(ref pressedHitResult, hitResult, state);
			if((state & State.Hot) != 0)
				SetStateCore(ref hotHitResult, hitResult, state);
		}
		public void ResetState() {
			SetStateCore(ref pressedHitResult, null, State.Normal);
			SetStateCore(ref hotHitResult, null, State.Normal);
		}
		void EnsureInitialState() {
			if(!isVisualStateInitialized) {
				isVisualStateInitialized = true;
				pressedHitResult = InitPressedState();
				hotHitResult = InitHotState();
			}
		}
		void SetStateCore(ref object prevHitResult, object hitResult, State state) {
			if(HitEquals(prevHitResult, hitResult)) return;
			object tmp = prevHitResult;
			prevHitResult = hitResult;
			if(tmp != null)
				OnStateChanged(tmp, GetState(tmp));
			if(hitResult != null)
				OnStateChanged(hitResult, state);
		}
		protected virtual object InitPressedState() { return null; }
		protected virtual object InitHotState() { return null; }
		protected virtual void OnStateChanged(object hitResult, State state) { }
		protected virtual bool HitEquals(object prevHitResult, object hitResult) {
			return (object.Equals(prevHitResult, hitResult));
		}
		public bool EnsureBounds() {
			if(IsReady) return true;
			EnsureBoundsCore();
			IsReady = true;
			return IsReady;
		}
		protected abstract void EnsureBoundsCore();
		public LayoutElementHitInfo CalcHitInfo(Point pt) {
			LayoutElementHitInfo hitInfo = null;
			using(IEnumerator<ILayoutElement> e = GetEnumerator()) {
				while(e.MoveNext()) {
					BaseLayoutElement element = (BaseLayoutElement)e.Current;
					if(element.HitTest(pt)) {
						hitInfo = element.CreateHitInfo(pt);
						element.CalcHitInfoCore(hitInfo);
					}
				}
			}
			return (hitInfo == null) ? LayoutElementHitInfo.Empty : hitInfo;
		}
		protected virtual LayoutElementHitInfo CreateHitInfo(Point pt) {
			return new LayoutElementHitInfo(pt, this);
		}
		protected virtual void CalcHitInfoCore(LayoutElementHitInfo hitInfo) {
			hitInfo.CheckAndSetHitTest(LayoutElementHitTest.Bounds, hitInfo.HitTest, LayoutElementHitTest.Bounds);
		}
		public ILayoutContainer Container {
			get { return parentCore; }
		}
		public ILayoutElement Parent {
			get { return parentCore; }
		}
		public ILayoutElement[] Nodes {
			get {
				if(nodesCore == null) nodesCore = GetNodesCore();
				return nodesCore;
			}
		}
		protected virtual ILayoutElement[] GetNodesCore() {
			return EmptyNodes;
		}
		public void Accept(IVisitor<ILayoutElement> visitor) {
			if(visitor == null) return;
			AcceptCore(visitor.Visit);
		}
		public void Accept(VisitDelegate<ILayoutElement> visit) {
			AcceptCore(visit);
		}
		void AcceptCore(VisitDelegate<ILayoutElement> visit) {
			if(visit == null) return;
			visit(this);
			AcceptNodes(visit);
		}
		void AcceptNodes(VisitDelegate<ILayoutElement> visit) {
			ILayoutElement[] nodes = Nodes;
			for(int i = 0; i < nodes.Length; i++)
				nodes[i].Accept(visit);
		}
		public IEnumerator<ILayoutElement> GetEnumerator() {
			return new LayoutElementEnumerator(this);
		}
	}
	public abstract class BaseLayoutContainer : BaseLayoutElement, ILayoutContainer {
		LayoutElementCollection itemsCore;
		protected override void OnCreate() {
			base.OnCreate();
			itemsCore = CreateItems();
		}
		protected override void OnDispose() {
			ILayoutElement[] items = itemsCore.ToArray();
			Ref.Dispose(ref itemsCore);
			for(int i = 0; i < items.Length; i++) {
				ILayoutElement element = items[i];
				Ref.Dispose(ref element);
			}
			base.OnDispose();
		}
		protected virtual LayoutElementCollection CreateItems() {
			return new LayoutElementCollection(this);
		}
		public LayoutElementCollection Items {
			get { return itemsCore; }
		}
		protected override ILayoutElement[] GetNodesCore() {
			return Items.ToArray();
		}
		internal void AddInternal(ILayoutElement element) {
			BeginUpdate();
			Items.Add(element);
			CancelUpdate();
		}
	}
}
