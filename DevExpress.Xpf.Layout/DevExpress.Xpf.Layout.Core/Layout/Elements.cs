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
namespace DevExpress.Xpf.Layout.Core {
	public class LayoutElementCollection : BaseChangeableList<ILayoutElement> {
		ILayoutContainer ownerCore;
		public LayoutElementCollection(ILayoutContainer owner) {
			ownerCore = owner;
		}
		protected override void OnElementAdded(ILayoutElement element) {
			base.OnElementAdded(element);
			AffinityHelper.SetAffinity(ownerCore, element);
			element.Disposed += OnLayoutItemDisposed;
		}
		protected override void OnElementRemoved(ILayoutElement element) {
			element.Disposed -= OnLayoutItemDisposed;
			AffinityHelper.SetAffinity(null, element);
			base.OnElementRemoved(element);
		}
		void OnLayoutItemDisposed(object sender, EventArgs ea) {
			RaiseCollectionChanged(
						new CollectionChangedEventArgs<ILayoutElement>(sender as ILayoutElement,
						CollectionChangedType.ElementDisposed)
					);
			if(List != null) Remove(sender as ILayoutElement);
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
		bool isVisualStateInitialized;
		object pressedHitResult, hotHitResult;
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
		public virtual void ResetState() {
			SetStateCore(ref pressedHitResult, null, State.Normal);
			SetStateCore(ref hotHitResult, null, State.Normal);
		}
		public virtual bool HitTestingEnabled { 
			get { return true; } 
		}
		public bool HitTest(Point pt) {
			return HitTestCore(pt);
		}
		public virtual bool IsActive { get { return true; } }
		public bool IsReady { get; private set; }
		public Rect Bounds {
			get { return ElementHelper.GetRect(this); }
		}
		protected virtual bool HitTestCore(Point pt) {
			return EnsureBounds() && Bounds.Contains(pt);
		}
		public void Invalidate() {
			IsReady = false;
		}
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
		protected virtual bool HitEquals(object prevHitResult, object hitResult) {
			return (object.Equals(prevHitResult, hitResult));
		}
		public bool EnsureBounds() {
			if(IsReady) return true;
			EnsureBoundsCore();
			IsReady = true;
			return IsReady;
		}
		protected virtual object InitPressedState() { return null; }
		protected virtual object InitHotState() { return null; }
		protected abstract void EnsureBoundsCore();
		public LayoutElementHitInfo CalcHitInfo(Point pt) {
			LayoutElementHitInfo hitInfo = null;
			using(IEnumerator<ILayoutElement> e = GetEnumerator()) {
				while(e.MoveNext()) {
					BaseLayoutElement element = (BaseLayoutElement)e.Current;
					if(element.HitTestingEnabled && element.HitTest(pt)) {
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
		protected virtual void CalcHitInfoCore(LayoutElementHitInfo hitInfo) { }
		protected internal virtual void OnStateChanged(object hitResult, State state) { }
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
		public Point Location { get; protected internal set; }
		public Size Size { get; protected internal set; }
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
		public IEnumerator<ILayoutElement> GetEnumerator()  {
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
