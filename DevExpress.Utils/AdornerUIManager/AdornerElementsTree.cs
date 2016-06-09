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
using DevExpress.Utils.Base;
namespace DevExpress.Utils.VisualEffects {
	class AdornerElementsTree : IAdornerElementsTree, IAdornerElementNode {
		object defaultElement = new object();
		List<IAdornerElementNode> nodesCore;
		List<IAdornerElementNode> nodesModified;
		IAdornerUIManager managerCore;
		int lockUpdate = 0;
		bool isDisposing;
		public AdornerElementsTree(IAdornerUIManager manager) {
			this.nodesCore = new List<IAdornerElementNode>();
			this.nodesModified = new List<IAdornerElementNode>();
			this.managerCore = manager;
		}
		public bool AddElement(IAdornerElement element) {
			IAdornerElementNode ownerNode = GetOrCreateNode<AdornerElementOwnerNode>(element, this);
			IAdornerElementNode targetNode = GetOrCreateNode<AdornerElementTargetNode>(element, ownerNode);
			IAdornerElementNode adornerElementNode = GetNode<AdornerElementNode>(element, targetNode);
			if(adornerElementNode != null) return false;
			GenerateNode<AdornerElementNode>(element, targetNode);
			return true;
		}
		public bool RemoveElement(IAdornerElement element) {
			IAdornerElementNode ownerNode = GetNode<AdornerElementOwnerNode>(element);
			if(ownerNode == null) return false;
			IAdornerElementNode targetNode = GetNode<AdornerElementTargetNode>(element, ownerNode);
			if(targetNode == null) return false;
			IAdornerElementNode adornerElementNode = GetNode<AdornerElementNode>(element, targetNode);
			if(adornerElementNode == null) return false;
			return RemoveNode(targetNode, adornerElementNode, true);
		}
		protected bool RemoveNode(IAdornerElementNode owner, IAdornerElementNode node, bool dispose = false) {
			bool isRemove = false;
			if(owner.Nodes.Contains(node)) {
				isRemove = owner.Nodes.Remove(node);
				CheckNode(owner);
			}
			if(dispose)
				node.Dispose();
			return isRemove;
		}
		protected bool AddNode(IAdornerElementNode owner, IAdornerElementNode node) {
			if(owner.Nodes.Contains(node)) return false;
			owner.Nodes.Add(node);
			((AdornerElementNode)node).SetParent(owner);
			return true;
		}
		public bool MoveNode(IAdornerElementNode node) {
			if(node is AdornerElementOwnerNode)
				return false;
			if(lockUpdate == 0)
				RemoveNode(node.ParentNode, node);
			IAdornerElementNode ownerNode = GetOrCreateNode<AdornerElementOwnerNode>(node.Element, this);
			if(node is AdornerElementTargetNode)
				return AddNode(ownerNode, node);
			IAdornerElementNode target = GetOrCreateNode<AdornerElementTargetNode>(node.Element, ownerNode);
			return AddNode(target, node);
		}
		T GenerateNode<T>(object element, IAdornerElementNode parentNode = null) {
			T obj = CreateNode<T>(GetArgs<T>(element));
			if(parentNode != null) {
				AdornerElementNode node = obj as AdornerElementNode;
				AddNode(parentNode, node);
			}
			return obj;
		}
		void CheckNode(IAdornerElementNode node) {
			if(this == node) return;
			if(node.Nodes.Count > 0) return;
			RemoveNode(node.ParentNode, node, true);
		}
		void DeferredCheckNodes() {
			if(lockUpdate > 0) return;
			if(nodesModified == null) return;
			foreach(var node in nodesModified)
				CheckNode(node);
			nodesModified.Clear();
		}
		object[] GetArgs<T>(object element) {
			return new object[] { GetElement<T>(element) };
		}
		T CreateNode<T>(object[] args) {
			return (T)Activator.CreateInstance(typeof(T), args);
		}
		T GetOrCreateNode<T>(object element, IAdornerElementNode parentNode = null) {
			T child = GetNode<T>(element, parentNode);
			if(child == null)
				child = GenerateNode<T>(element, parentNode);
			return child;
		}
		T GetNode<T>(object element, IAdornerElementNode node = null) {
			object el = GetElement<T>(element);
			IEnumerable<IAdornerElementNode> collection = node == null ? nodesCore : node.Nodes;
			foreach(var child in collection)
				if(child.Element == el)
					return (T)child;
			return default(T);
		}
		object GetElement<T>(object element) {
			Type type = typeof(T);
			if(type.Equals(typeof(AdornerElementOwnerNode)))
				return GetOwner(element);
			if(type.Equals(typeof(AdornerElementTargetNode)))
				return GetTarget(element);
			return element;
		}
		object GetOwner(object element) {
			if(element == defaultElement) return element;
			if(element is ISupportAdornerUIManager) return element;
			ISupportAdornerElement target = GetTarget(element) as ISupportAdornerElement;
			return target == null ? defaultElement : target.Owner;
		}
		object GetTarget(object element) {
			if(element is ISupportAdornerElement) return element;
			IAdornerElement aEL = element as IAdornerElement;
			return aEL == null || aEL.TargetElement == null ? defaultElement : aEL.TargetElement;
		}
		IEnumerable<IAdornerElement> IAdornerElementNode.GetAdornerElements() {
			List<IAdornerElement> elements = new List<IAdornerElement>();
			foreach(var node in nodesCore)
				elements.AddRange(node.GetAdornerElements());
			return elements;
		}
		#region IAdornerElementNode Members
		List<IAdornerElementNode> IAdornerElementNode.Nodes { get { return nodesCore; } }
		IAdornerElementNode IAdornerElementNode.ParentNode { get { return null; } }
		object IAdornerElementNode.Element { get { return null; } }
		IAdornerElementsTree IAdornerElementNode.Tree { get { return this; } }
		#endregion
		#region IDisposable Members
		public void Dispose() {
			if(!isDisposing) {
				isDisposing = true;
				OnDispose();
			}
		}
		protected virtual void OnDispose() {
			foreach(var node in nodesCore)
				node.Dispose();
			nodesCore.Clear();
			managerCore = null;
		}
		#endregion
		#region ISupportBatchUpdate Members
		bool ISupportBatchUpdate.IsUpdateLocked {
			get { return managerCore.IsUpdateLocked; }
		}
		void ISupportBatchUpdate.BeginUpdate() {
			managerCore.BeginUpdate();
			lockUpdate++;
		}
		void ISupportBatchUpdate.EndUpdate() {
			managerCore.EndUpdate();
			if(lockUpdate == 0) return;
			if(--lockUpdate == 0)
				DeferredCheckNodes();
		}
		void ISupportBatchUpdate.CancelUpdate() {
			managerCore.CancelUpdate();
			lockUpdate = 0;
			DeferredCheckNodes();
		}
		#endregion
	}
	class AdornerElementNode : IAdornerElementNode {
		object elementCore;
		List<IAdornerElementNode> nodesCore;
		IAdornerElementNode parentNodeCore;
		bool isDisposing;
		public AdornerElementNode(object element) {
			this.nodesCore = new List<IAdornerElementNode>();
			this.elementCore = element;
			SubscribeElement();
		}
		protected virtual IEnumerable<IAdornerElement> GetAdornerElements() {
			List<IAdornerElement> elements = new List<IAdornerElement>();
			if(Element is IAdornerElement)
				elements.Add(CastElement<IAdornerElement>());
			foreach(var node in Nodes)
				elements.AddRange(node.GetAdornerElements());
			return elements;
		}
		protected T CastElement<T>() where T : class { return elementCore as T; }
		protected virtual void SubscribeElement() {
			IAdornerElement element = CastElement<IAdornerElement>();
			if(element == null) return;
			element.TargetChanged += OnElementTargetChanged;
		}
		void OnElementTargetChanged(object sender, EventArgs e) {
			Tree.MoveNode(this);
			IAdornerElement element = CastElement<IAdornerElement>();
			if(element == null) return;
			element.Update();
		}
		protected virtual void UnsubscribeElement() {
			IAdornerElement element = CastElement<IAdornerElement>();
			if(element == null) return;
			element.TargetChanged -= OnElementTargetChanged;
		}
		protected internal void SetParent(IAdornerElementNode parentNode) {
			this.parentNodeCore = parentNode;
		}
		#region IAdornerElementNode Members
		public List<IAdornerElementNode> Nodes {
			get { return nodesCore; }
		}
		public IAdornerElementNode ParentNode {
			get { return parentNodeCore; }
		}
		public object Element {
			get { return elementCore; }
		}
		IAdornerElementsTree IAdornerElementNode.Tree { get { return Tree; } }
		protected virtual IAdornerElementsTree Tree {
			get {
				if(ParentNode == null) return null;
				return ParentNode.Tree;
			}
		}
		#endregion
		#region IDisposable Members
		public void Dispose() {
			if(!isDisposing) {
				isDisposing = true;
				OnDispose();
			}
		}
		protected virtual void OnDispose() {
			UnsubscribeElement();
			foreach(var node in Nodes)
				node.Dispose();
			nodesCore.Clear();
			parentNodeCore = null;
			elementCore = null;
		}
		#endregion
		protected void ElementsUpdate() {
			Tree.BeginUpdate();
			foreach(var element in GetAdornerElements())
				element.Update();
			Tree.EndUpdate();
		}
		protected void ElementsInvalidate() {
			Tree.BeginUpdate();
			foreach(var element in GetAdornerElements())
				element.Invalidate();
			Tree.EndUpdate();
		}
		IEnumerable<IAdornerElement> IAdornerElementNode.GetAdornerElements() {
			return GetAdornerElements();
		}
	}
	class AdornerElementTargetNode : AdornerElementNode {
		public AdornerElementTargetNode(object element) : base(element) { }
		ISupportAdornerElement Target { get { return CastElement<ISupportAdornerElement>(); } }
		System.Windows.Forms.Control Control {
			get {
				if(Target != null) return null;
				return CastElement<System.Windows.Forms.Control>();
			}
		}
		protected override void SubscribeElement() {
			if(Target != null)
				Target.Changed += OnTargetChanged;
			if(Control != null) {
				Control.SizeChanged += OnControlChanged;
				Control.Move += OnControlChanged;
				Control.LocationChanged += OnControlChanged;
				Control.ParentChanged += OnControlChanged;
				Control.VisibleChanged += OnControlChanged;
				Control.Disposed += OnDisposed;
			}
		}
		void OnOwnerChanged() {
			Tree.MoveNode(this);
			ElementsUpdate();
		}
		void OnControlChanged(object sender, EventArgs e) {
			ElementsUpdate();
		}
		void OnDisposed(object sender, EventArgs e) {
			Tree.BeginUpdate();
			foreach(var element in GetAdornerElements())
				element.TargetElement = null;
			Tree.EndUpdate();
		}
		void OnTargetChanged(object sender, UpdateActionEvendArgs e) {
			switch(e.Action) {
				case UpdateAction.Invalidate:
					ElementsInvalidate();
					break;
				case UpdateAction.Dispose:
					OnDisposed(Element, EventArgs.Empty);
					break;
				case UpdateAction.OwnerChanged:
					OnOwnerChanged();
					break;
				case UpdateAction.Update:
					ElementsUpdate();
					break;
			}
		}
		protected override void UnsubscribeElement() {
			if(Target != null)
				Target.Changed -= OnTargetChanged;
			if(Control != null) {
				Control.SizeChanged -= OnControlChanged;
				Control.Move -= OnControlChanged;
				Control.LocationChanged -= OnControlChanged;
				Control.ParentChanged -= OnControlChanged;
				Control.VisibleChanged -= OnControlChanged;
				Control.Disposed -= OnDisposed;
			}
		}
	}
	class AdornerElementOwnerNode : AdornerElementNode {
		ISupportAdornerUIManager Owner { get { return CastElement<ISupportAdornerUIManager>(); } }
		public AdornerElementOwnerNode(object element) : base(element) { }
		protected override void SubscribeElement() {
			if(Owner == null) return;
			Owner.Changed += OnOwnerChanged;
		}
		void OnOwnerChanged(object sender, UpdateActionEvendArgs e) {
			switch(e.Action) {
				case UpdateAction.BeginUpdate:
					Tree.BeginUpdate();
					break;
				case UpdateAction.EndUpdate:
					ElementsUpdate();
					Tree.EndUpdate();
					break;
				case UpdateAction.Invalidate:
					ElementsInvalidate();
					break;
				case UpdateAction.Update:
					ElementsUpdate();
					break;
			}
		}
		protected override void UnsubscribeElement() {
			if(Owner == null) return;
			Owner.Changed -= OnOwnerChanged;
		}
	}
}
