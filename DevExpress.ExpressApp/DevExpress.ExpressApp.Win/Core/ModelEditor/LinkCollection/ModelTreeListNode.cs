#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils.Reflection;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor {
	public class ModelTreeListNode : IDisposable {
		public const string LinksId = "Links";
		private ExtendModelInterfaceAdapter extendModelInterfaceAdapter;
		private List<ModelTreeListNode> virtualNodes = new List<ModelTreeListNode>();
		public ModelTreeListNode VirtualParent { get; set; }
		public List<ModelTreeListNode> VirtualNodes {
			get { return virtualNodes; }
			set { virtualNodes = value; }
		}
		private List<ModelTreeListNode> childs = new List<ModelTreeListNode>();
		private List<ModelTreeListNode> links = new List<ModelTreeListNode>();
		private ModelTreeListNode primaryNode;
		private ModelTreeListNode parent;
		private ModelNode _modelNode;
		private ModelTreeListNodeType modelTreeListNodeType;
		private bool groupedNodes = false;
		public static int LockModelNodeEvents = 0;
		private bool isVirtualTreeNode = false;
		private bool isRootVirtualTreeNode = false;
		public bool IsRootVirtualTreeNode {
			get { return isRootVirtualTreeNode; }
		}
		public void ResetVirtualTree(bool deleteRootVirtualTreeNode) {
			bool needResetMasterVirtualTreeNode = Childs.Count > 0;
			ResetVirtualTreeParentsCore(deleteRootVirtualTreeNode);
			if(needResetMasterVirtualTreeNode) {
				MasterVirtualTreeNode.ClearChilds(true);
			}
		}
		private void ResetVirtualTreeParentsCore(bool deleteRootVirtualTreeNode) {
			foreach(ModelTreeListNode child in Childs) {
				if(child.IsRootVirtualTreeNode) {
					child.ResetVirtualTreeParentsCore(deleteRootVirtualTreeNode);
					if(deleteRootVirtualTreeNode) {
						((IModelNode)child.ModelNode).Remove();
					}
				}
				else {
					ModelVirtualTreeSetParent(null, child);
				}
			}
		}
		public bool VirtualTreeNode {
			get {
				return isVirtualTreeNode;
			}
		}
		public static bool IsModelVirtualTreeNode(IModelNode node) {
			if(node != null) {
				ModelVirtualTreeAttribute[] attributes = AttributeHelper.GetAttributesConsideringInterfaces<ModelVirtualTreeAttribute>(node.GetType(), true);
				if(attributes != null && attributes.Length > 0) {
					return true;
				}
			}
			return false;
		}
		private void SetRootVirtualTreeNode(bool value) {
			if(!isRootVirtualTreeNode) {
				isRootVirtualTreeNode = value;
			}
		}
		private void SetVirtualTreeNode(bool value) {
			if(!isVirtualTreeNode) {
				isVirtualTreeNode = value;
			}
		}
		public bool ModelVirtualTreeSetParent(ModelTreeListNode targetNode, ModelTreeListNode draggedNode) {
			ModelVirtualTreeParentAttribute attribute = GetModelVirtualTreeParentAttribute();
			if(attribute != null) {
				Type logicType = attribute.LogicType;
				if(typeof(IModelVirtualTreeParentLogic).IsAssignableFrom(logicType)) {
					IModelVirtualTreeParentLogic logic = (IModelVirtualTreeParentLogic)Activator.CreateInstance(logicType);
					IModelNode targetModelNode = targetNode != null ? targetNode.ModelNode : null;
					return logic.SetParent(draggedNode.ModelNode, targetModelNode);
				}
			}
			return false;
		}
		private ModelVirtualTreeParentAttribute GetModelVirtualTreeParentAttributeCore() {
			ModelVirtualTreeParentAttribute result = null;
			ModelVirtualTreeParentAttribute[] attributes = AttributeHelper.GetAttributesConsideringInterfaces<ModelVirtualTreeParentAttribute>(ModelNode.GetType(), true);
			if(attributes != null && attributes.Length > 0) {
				result = attributes[0];
			}
			return result;
		}
		internal ModelTreeListNode RootVirtualTreeNode {
			get {
				if(isRootVirtualTreeNode) {
					return this;
				}
				else {
					if(Parent != null) {
						return Parent.RootVirtualTreeNode;
					}
					else {
						return null;
					}
				}
			}
		}
		private ModelTreeListNode MasterVirtualTreeNode {
			get {
				if(isRootVirtualTreeNode && !Parent.VirtualTreeNode) {
					return this;
				}
				else {
					if(Parent != null) {
						return Parent.MasterVirtualTreeNode;
					}
					else {
						return null;
					}
				}
			}
		}
		private ModelVirtualTreeParentAttribute GetModelVirtualTreeParentAttribute() {
			ModelVirtualTreeParentAttribute result = null;
			if(RootVirtualTreeNode != null) {
				result = RootVirtualTreeNode.GetModelVirtualTreeParentAttributeCore();
			}
			return result;
		}
		public ModelNode GetLogicalParent(ModelNode node) {
			IModelNode result = null;
			ModelVirtualTreeParentAttribute attribute = GetModelVirtualTreeParentAttribute();
			if(attribute != null) {
				Type logicType = attribute.LogicType;
				if(typeof(IModelVirtualTreeParentLogic).IsAssignableFrom(logicType)) {
					IModelVirtualTreeParentLogic logic = (IModelVirtualTreeParentLogic)Activator.CreateInstance(logicType);
					result = logic.GetParent((IModelNode)node);
				}
			}
			return (ModelNode)result;
		}
		public ModelTreeListNode(ExtendModelInterfaceAdapter extendModelInterfaceAdapter, ModelTreeListNode parent, ModelNode modelNode, ModelTreeListNodeType modelTreeListNodeType, ModelTreeListNode primaryNode) :
			this(extendModelInterfaceAdapter, parent, modelNode, modelTreeListNodeType) {
			this.primaryNode = primaryNode;
			this.primaryNode.links.Add(this);
		}
		public ModelTreeListNode(ExtendModelInterfaceAdapter extendModelInterfaceAdapter, ModelTreeListNode parent, ModelNode modelNode, ModelTreeListNodeType modelTreeListNodeType) {
			this.extendModelInterfaceAdapter = extendModelInterfaceAdapter;
			this.parent = parent;
			if(parent != null) {
				SetVirtualTreeNode(parent.VirtualTreeNode);
			}
			this.modelTreeListNodeType = modelTreeListNodeType;
			this.ModelNode = modelNode;
		}
		public List<ModelTreeListNode> Childs {
			get {
				return childs;
			}
		}
		public ModelTreeListNode Owner {
			get {
				if(ModelTreeListNodeType != ModelTreeListNodeType.Primary) {
					return Parent != null ? Parent.Owner : null;
				}
				return this;
			}
		}
		public ModelTreeListNode Parent {
			get { return parent; }
			internal set { parent = value; }
		}
		public ModelTreeListNode Root {
			get {
				return Parent != null ? Parent.Root : this;
			}
		}
		public ModelTreeListNodeType ModelTreeListNodeType {
			get {
				return modelTreeListNodeType;
			}
		}
		public ModelTreeListNode PrimaryNode {
			get {
				return ModelTreeListNodeType == ModelTreeListNodeType.Primary || ModelTreeListNodeType == ModelTreeListNodeType.Links ? this : primaryNode;
			}
		}
		private string modelNodeID = "";
		public ModelNode ModelNode {
			get {
				return _modelNode;
			}
			internal set {
				if(_modelNode != null) {
					UnsubscribeEvents();
				}
				_modelNode = value;
				if(_modelNode != null) {
					modelNodeID = _modelNode.Id;
				}
				SetVirtualTreeNode(IsModelVirtualTreeNode((IModelNode)_modelNode));
				SetRootVirtualTreeNode(IsModelVirtualTreeNode((IModelNode)_modelNode));
				SubscribeEvents();
			}
		}
		public int NodesCount {
			get {
				if(childs != null) {
					return childs.Count;
				}
				else {
					return 0;
				}
			}
		}
		internal bool Grouped {
			get {
				return groupedNodes;
			}
			set {
				groupedNodes = value;
			}
		}
		public void LinkRemove(ModelTreeListNode linkNode) {
			links.Remove(linkNode);
		}
		public void LinksClear() {
			links.Clear();
		}
		public int LinksCount {
			get {
				return links.Count;
			}
		}
		public IEnumerable<ModelTreeListNode> Links {
			get {
				return links;
			}
		}
		public ModelTreeListNode GetLink(int index) {
			return links[index];
		}
		public ModelTreeListNode GetChildNode(ModelTreeListNode parent, string id) {
			foreach(ModelTreeListNode item in parent.childs) {
				if(item.ModelNode.Id == id) {
					return item;
				}
			}
			return null;
		}
		public void SetModelNode(ModelNode modelNode, bool raiseEvents) {
			PrimaryNode.ClearChilds(raiseEvents);
			this.ModelNode = modelNode;
			if(raiseEvents) {
				PrimaryNode.OnObjectChanged();
			}
		}
		public void ClearChilds(bool raiseEvents) {
			ClearChilds(raiseEvents, false, false);
		}
		public void ClearChilds(bool raiseEvents, bool removeAssociation) {
			ClearChilds(raiseEvents, removeAssociation, false);
		}
		public void ClearChilds(bool raiseEvents, bool removeAssociation, bool clearChildsOnly) {
			if(VirtualParent != null && VirtualParent.VirtualNodes.Contains(this)) {
				ModelTreeListNode masterVirtualTreeNode = MasterVirtualTreeNode;
				if(masterVirtualTreeNode != null) {
					raiseEvents = false;
					masterVirtualTreeNode.ClearChilds(true);
					return;
				}
			}
			List<ModelTreeListNode> _links = new List<ModelTreeListNode>();
			List<ModelTreeListNode> _childs = new List<ModelTreeListNode>();
			ModelTreeListNode storePrimaryNode = PrimaryNode;
			storePrimaryNode.ClearChildsCore(_links, _childs);
			if(!clearChildsOnly) {
				List<ModelTreeListNode> _owners = new List<ModelTreeListNode>();
				foreach(ModelTreeListNode link in _links) {
					ModelTreeListNode _owner = link.Owner;
					if(removeAssociation) {
						extendModelInterfaceAdapter.RemoveAssociation(link.ModelNode);
					}
					if(_owner != null && !_owner.IsDisposed && !_owners.Contains(_owner)) {
						_owners.Add(_owner);
					}
				}
				foreach(ModelTreeListNode _owner in _owners) {
					_owner.ClearOwnerLinksCore();
					if(raiseEvents) {
						_owner.OnCollectionChanged();
					}
				}
			}
			foreach(ModelTreeListNode link in _links) {
				if(!link.IsDisposed) {
					if(!link.parent.IsDisposed) {
						link.parent.Childs.Remove(link);
					}
				}
			}
			foreach(ModelTreeListNode link in _links) {
				link.Dispose();
			}
			List<ModelTreeListNode> virtualNodes = new List<ModelTreeListNode>();
			foreach(ModelTreeListNode child in _childs) {
				if(child.VirtualTreeNode && !child.IsRootVirtualTreeNode && child.VirtualParent != null) {
					child.VirtualParent.VirtualNodes.Remove(child);
					child.VirtualParent = null;
				}
				else {
					if(child.VirtualNodes.Count > 0) {
						foreach(ModelTreeListNode virtualNode in child.VirtualNodes) {
							ModelTreeListNode masterVirtualTreeNode = virtualNode.MasterVirtualTreeNode;
							if(masterVirtualTreeNode != null && !virtualNodes.Contains(masterVirtualTreeNode)) {
								virtualNodes.Add(masterVirtualTreeNode);
							}
						}
					}
				}
			}
			foreach(ModelTreeListNode child in _childs) {
				child.Dispose();
			}
			foreach(ModelTreeListNode virtualNode in VirtualNodes) {
				ModelTreeListNode masterVirtualTreeNode = virtualNode.MasterVirtualTreeNode;
				if(masterVirtualTreeNode != null && !virtualNodes.Contains(masterVirtualTreeNode)) {
					virtualNodes.Add(masterVirtualTreeNode);
				}
			}
			foreach(ModelTreeListNode targetNode in virtualNodes) {
				targetNode.ClearChilds(raiseEvents, removeAssociation, clearChildsOnly);
			}
			if(raiseEvents) {
				storePrimaryNode.OnCollectionChanged();
			}
		}
		public bool HasModification {
			get {
				if(_modelNode == null) { return false; }
				if(ModelTreeListNodeType == ModelTreeListNodeType.Primary || ModelTreeListNodeType == ModelTreeListNodeType.CollectionItem) {
					return _modelNode.HasModification;
				}
				else {
					foreach(ModelTreeListNode child in childs) {
						if(child.HasModification) {
							return true;
						}
					}
				}
				return false;
			}
		}
		public override string ToString() {
			string result = "";
			if(IsDisposed) {
				result = "Disposed  - ";
			}
			result += modelNodeID + " - " + ModelTreeListNodeType.ToString();
			return result;
		}
		internal ModelTreeListNode ParentIgnoreGroup {
			get {
				return parent == null || parent.ModelTreeListNodeType != ModelTreeListNodeType.Group ? parent : parent.parent;
			}
		}
		protected virtual void OnCollectionChanged() {
			if(extendModelInterfaceAdapter != null) {
				extendModelInterfaceAdapter.OnChanged(this, NodeChangedType.CollectionChanged);
			}
		}
		protected virtual void OnObjectChanged() {
			if(extendModelInterfaceAdapter != null) {
				extendModelInterfaceAdapter.OnChanged(this, NodeChangedType.ObjectChanged);
			}
		}
		private void ClearChildsCore(List<ModelTreeListNode> _links, List<ModelTreeListNode> _childs) {
			if(links != null) {
				_links.AddRange(links);
				for(int counter = childs.Count - 1; counter > -1; counter--) {
					ModelTreeListNode node = childs[counter];
					if(node.ModelTreeListNodeType != ModelEditor.ModelTreeListNodeType.Links) { 
						_childs.Add(node);
						node.ClearChildsCore(_links, _childs);
						node.links.Clear();
						childs.RemoveAt(counter);
					}
				}
				links.Clear();
			}
		}
		private void ClearOwnerLinksCore() {
			for(int counter = childs.Count - 1; counter > -1; counter--) {
				ModelTreeListNode node = childs[counter];
				node.ClearOwnerLinksCore();
				if(node.ModelTreeListNodeType == ModelTreeListNodeType.Collection || node.ModelTreeListNodeType == ModelTreeListNodeType.CollectionItem) {
					node.PrimaryNode.LinkRemove(node);
					if(!node.parent.IsDisposed) {
						node.parent.Childs.Remove(node);
					}
					node.Dispose();
				}
			}
			if(childs.Count > 0) {
				if(ModelTreeListNodeType != ModelTreeListNodeType.Primary) {
					childs.Clear();
				}
				else {
					for(int i = 0; i < childs.Count; i++) {
						if(childs[i].ModelTreeListNodeType != ModelTreeListNodeType.Primary) {
							childs.Remove(childs[i]);
							i--;
						}
					}
				}
			}
		}
		private void SubscribeEvents() {
			if(modelTreeListNodeType == ModelTreeListNodeType.Primary) {
				if(ModelNode != null) {
					ModelEditorHelper.RemovePropertyChangedHandler(ModelNode, new PropertyChangedEventHandler(ModelNode_PropertyChanged));
					ModelEditorHelper.AddPropertyChangedHandler(ModelNode, new PropertyChangedEventHandler(ModelNode_PropertyChanged));
					ModelEditorHelper.RemovePropertyChangingHandler(ModelNode, new PropertyChangingEventHandler(ModelNode_PropertyChanging));
					ModelEditorHelper.AddPropertyChangingHandler(ModelNode, new PropertyChangingEventHandler(ModelNode_PropertyChanging));
				}
			}
		}
		private void UnsubscribeEvents() {
			if(ModelNode != null) {
				ModelEditorHelper.RemovePropertyChangedHandler(ModelNode, new PropertyChangedEventHandler(ModelNode_PropertyChanged));
				ModelEditorHelper.RemovePropertyChangingHandler(ModelNode, new PropertyChangingEventHandler(ModelNode_PropertyChanging));
			}
		}
		private ModelNode oldValue = null;
		private void ModelNode_PropertyChanging(object sender, System.ComponentModel.PropertyChangingEventArgs e) {
			oldValue = ModelNode.GetValue(e.PropertyName) as ModelNode;
		}
		private void ModelNode_PropertyChanged(object sender, PropertyChangedEventArgs e) {
			if(e.PropertyName != "FreezeLayout" && extendModelInterfaceAdapter != null) {
				RaisePropertyChanged(this, e);
				if(LockModelNodeEvents == 0) {
					extendModelInterfaceAdapter.RefreshGroupsIfNeed(this);
					if(ModelTreeListNode.LockModelNodeEvents == 0) {
						try {
							extendModelInterfaceAdapter.RefreshLinksIfNeed(this, e.PropertyName, oldValue);
						}
						finally {
							oldValue = null;
						}
					}
					foreach(ModelTreeListNode node in Links) {
						RaisePropertyChanged(node, e);
					}
				}
			}
		}
		private void RaisePropertyChanged(ModelTreeListNode node, PropertyChangedEventArgs e) {
			if(e.PropertyName == ModelValueNames.Index) {
				Parent.OnCollectionChanged();
			}
			else {
				node.OnObjectChanged();
			}
		}
		#region IDisposable Members
		bool _isDisposed = false;
		public bool IsDisposed {
			get { return _isDisposed; }
		}
		public void Dispose() {
			_isDisposed = true;
			UnsubscribeEvents();
			_modelNode = null;
			parent = null;
			primaryNode = null;
			childs = null;
			links = null;
			extendModelInterfaceAdapter = null;
		}
		#endregion
#if DebugTest
		public void DebugTest_SetParent(ModelTreeListNode parent) {
			Parent = parent;
		}
#endif
	}
	public enum ModelTreeListNodeType { Links, Collection, Primary, CollectionItem, Group }
}
