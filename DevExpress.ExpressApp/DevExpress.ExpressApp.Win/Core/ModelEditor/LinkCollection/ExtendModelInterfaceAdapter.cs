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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Utils.Reflection;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor {
	public class ExtendModelInterfaceAdapter : ModelInterfaceAdapter, IDisposable {
		public const string errorMessage = "An internal exception occurs while generating the Application Model. Further usage of the Model Editor can lead to unpredictable errors.";
		public const string GroupSettings = "GroupSettings";
		public const string UnspecifiedGroupName = "Unspecified";
#if DebugTest
		public static bool LockGrouped = false;
#endif
		private ModelTreeListNode _rootNode = null;
		private AssociationCollection associationCollection = new AssociationCollection();
		private Dictionary<ModelTreeListNode, NodeChangedType> nodeChangedEvents = new Dictionary<ModelTreeListNode, NodeChangedType>();
		private Dictionary<Type, string> groupCollectionNodeTypes = new Dictionary<Type, string>();
		private List<string> groupedDefaultItems = new List<string>();
		private int lockEvents = 0;
		private Dictionary<string, bool> _groupSettings = new Dictionary<string, bool>();
		private ManualResetEvent manualEvent;
		private static bool linksEnabled = true;
		private bool modelLoaded = false;
		private SynchronizationObject synchronizationObject = new SynchronizationObject();
		private LinksCollectionCalculator schemaHelper = new LinksCollectionCalculator();
		private Dictionary<ModelNode, Dictionary<ModelNode, string>> linkData = new Dictionary<ModelNode, Dictionary<ModelNode, string>>();
		private Dictionary<ModelNode, List<ModelNode>> requiredModelNodeValues = new Dictionary<ModelNode, List<ModelNode>>();
		private Dictionary<Type, List<string>> requiredPropertyesByType = new Dictionary<Type, List<string>>();
		private Dictionary<Type, IModelIsVisible> hideChildrenInVirtualTree = new Dictionary<Type, IModelIsVisible>();
		private Dictionary<Type, bool> hideLinks = new Dictionary<Type, bool>();
		public ExtendModelInterfaceAdapter() {
			RegisterGroupAlias(typeof(IModelBOModel), "Name");
			RegisterGroupAlias(typeof(IModelControllers), "Name");
			RegisterGroupAlias(typeof(IModelViews), "ModelClass.Name");
		}
		public void OnChanged(ModelTreeListNode node, NodeChangedType nodeChangedType) {
			if(!nodeChangedEvents.ContainsKey(node)) {
				nodeChangedEvents.Add(node, nodeChangedType);
			}
			OnChanged();
		}
		internal void BeginUpdate() {
			lockEvents++;
		}
		internal void EndUpdate() {
			if(lockEvents > 0) {
				lockEvents--;
			}
			OnChanged();
		}
		public void Group(ModelTreeListNode node) {
			SetGroupFlag(node, true);
			OnChanged(node, NodeChangedType.CollectionChanged);
		}
		public void UnGroup(ModelTreeListNode node) {
			if(node.Grouped) {
				List<ModelTreeListNode> items = new List<ModelTreeListNode>();
				foreach(ModelTreeListNode nodeItem in node.Childs) {
					items.AddRange(nodeItem.Childs);
					nodeItem.Dispose();
				}
				node.Childs.Clear();
				foreach(ModelTreeListNode nodeItem in items) {
					nodeItem.Parent = node;
				}
				node.Childs.AddRange(items);
			}
			SetGroupFlag(node, false);
			OnChanged(node, NodeChangedType.CollectionChanged);
		}
		public ReadOnlyCollection<ModelTreeListNode> SortChilds(ModelTreeListNode node) {
			if(node.Root == node) {
				groupedDefaultItems.Clear();
			}
			List<ModelTreeListNode> result = new List<ModelTreeListNode>(node.Childs);
			if(result.Count > 0) {
				string groupedPropertyName = GetGroupPropertyName(node.ModelNode);
				bool isNeedGrouped = node.ModelTreeListNodeType == ModelTreeListNodeType.Primary &&
					(!groupedDefaultItems.Contains(node.ModelNode.Id));
#if DebugTest
				isNeedGrouped &= !LockGrouped;
#endif
				if(!string.IsNullOrEmpty(groupedPropertyName) && (node.Grouped || isNeedGrouped) && GetGroupSettings(node.ModelNode.Id)) {
					SetGroupFlag(node, true);
					groupedDefaultItems.Add(node.ModelNode.Id);
					GroupChildsCore(groupedPropertyName, node);
					result = new List<ModelTreeListNode>(node.Childs);
				}
				SortNodes(result);
			}
			return new ReadOnlyCollection<ModelTreeListNode>(result);
		}
		public ModelTreeListNode FindPrimaryNode(ModelNode modelNode) {
			return GetPrimaryNode(modelNode, false);
		}
		public ModelTreeListNode GetPrimaryNode(ModelNode sourceNode) {
			return GetPrimaryNode(sourceNode, true);
		}
		[Browsable(false)]
		public static bool LinksEnabled {
			get {
				return linksEnabled;
			}
			set {
				linksEnabled = value;
			}
		}
		private static bool loadLinkInInnerThread = true;
		[Browsable(false)]
		public static bool LoadLinkInInnerThread {
			get {
				return loadLinkInInnerThread;
			}
			set {
				loadLinkInInnerThread = value;
			}
		}
		public void SetRootNode(ModelNode modelApplication) {
			modelLoaded = modelApplication != null;
			if(RootNode == null) {
				RootNode = new ModelTreeListNode(this, null, modelApplication, ModelTreeListNodeType.Primary);
				schemaHelper.ModelApplication = (ModelApplicationBase)modelApplication;
			}
			else {
				groupedDefaultItems.Clear();
				RootNode.SetModelNode(modelApplication, false);
			}
			if(manualEvent != null) {
				manualEvent.WaitOne();
			}
			manualEvent = null;
			ClearAssociation();
			if(modelApplication != null) {
				if(linksEnabled) {
					CollectAll();
				}
				else {
					manualEvent = new ManualResetEvent(true);
				}
			}
		}
		public FastModelEditorHelper fastModelEditorHelper {
			get {
				return modelEditorHelper;
			}
		}
		public ModelTreeListNode RootNode {
			get {
				return _rootNode;
			}
			private set {
				_rootNode = value;
			}
		}
		public void AddAssociation(ModelNode modelNode) {
			manualEvent.WaitOne();
			CollectLinks(modelNode);
		}
		public void RemoveAssociation(ModelNode modelNode) {
			manualEvent.WaitOne();
			foreach(ModelValueInfo modelValueInfo in modelNode.NodeInfo.ValuesInfo) {
				if(typeof(IModelNode).IsAssignableFrom(modelValueInfo.PropertyType)) {
					ModelNode rootNode = modelNode.GetValue(modelValueInfo.Name) as ModelNode;
					if(rootNode != null) {
						AssociationCollection.RemoveLink(rootNode, modelNode, modelValueInfo.Name);
					}
				}
			}
		}
		public void RemoveAssociation(ModelNode rootNode, ModelNode modelNode, string propertyName) {
			manualEvent.WaitOne();
			AssociationCollection.RemoveLink(rootNode, modelNode, propertyName);
		}
		public void RefreshAssociation() {
			if(ModelTreeListNode.LockModelNodeEvents == 0 && linksEnabled) {
				StopCollect();
				ClearAssociation();
				Collect(RootNode.ModelNode.Application);
			}
		}
		public bool CanGroupChilds(ModelTreeListNode node) {
			return node.ModelTreeListNodeType == ModelTreeListNodeType.Primary && !string.IsNullOrEmpty(GetGroupPropertyName(node.ModelNode));
		}
		public bool IsGroupChilds(ModelTreeListNode node) {
			bool result;
			_groupSettings.TryGetValue(node.ModelNode.Id, out result);
			return result;
		}
		public void LoadSettings(SettingsStorage settings) {
			if(settings != null) {
				try {
					string items = settings.LoadOption(ModelEditorControl.ModelEditorControlSettingsPath, GroupSettings);
					if(!string.IsNullOrEmpty(items)) {
						List<string> values = new List<string>(items.Split(';'));
						foreach(string value in values) {
							string[] groupState = value.Split('=');
							_groupSettings.Add(groupState[0], Convert.ToBoolean(groupState[1]));
						}
					}
				}
				catch { }
			}
		}
		public void SaveSettings(SettingsStorage settings) {
			if(settings != null) {
				string items = null;
				foreach(KeyValuePair<string, bool> groupState in _groupSettings) {
					items += items == null ? groupState.Key + "=" + groupState.Value :
					   ";" + groupState.Key + "=" + groupState.Value;
				}
				if(items != null) {
					settings.SaveOption(ModelEditorControl.ModelEditorControlSettingsPath, GroupSettings, items);
				}
			}
		}
		public void RefreshGroupsIfNeed(ModelTreeListNode node) {
			if(node.Parent != null && node.Parent.ModelTreeListNodeType == ModelTreeListNodeType.Group) {
				string actualGroupName = GetGroupName(GetGroupPropertyName(node.Parent.Parent.ModelNode), node.ModelNode);
				if(actualGroupName != node.Parent.ModelNode.Id) {
					ModelTreeListNode oldGroup = node.Parent;
					ModelTreeListNode actualGroup = node.GetChildNode(node.Parent.Parent, actualGroupName);
					if(actualGroup == null) {
						actualGroup = node.Parent.Parent;
					}
					node.Parent = actualGroup;
					actualGroup.Childs.Add(node);
					oldGroup.Childs.Remove(node);
					OnChanged(oldGroup, NodeChangedType.CollectionChanged);
					OnChanged(actualGroup, NodeChangedType.CollectionChanged);
					OnChanged(node, NodeChangedType.GroupChanged);
					RemoveGoupNodeIfNeed(oldGroup);
				}
			}
		}
		private void RemoveGoupNodeIfNeed(ModelTreeListNode node) {
			if(node.ModelTreeListNodeType == ModelTreeListNodeType.Group && node.Childs.Count == 0) {
				node.Parent.Childs.Remove(node);
				OnChanged(node, NodeChangedType.DeleteNode);
				OnChanged(node.Parent, NodeChangedType.CollectionChanged);
				node.Dispose();
			}
		}
		public override IEnumerable GetChildren(object nodeObject) {
			if(!modelLoaded || nodeObject == null) {
				return new List<Object>();
			}
			ModelTreeListNode parent = (ModelTreeListNode)nodeObject;
			if(parent.ModelTreeListNodeType == ModelTreeListNodeType.Primary) {
				foreach(object item in GetChildrenCore(parent.ModelNode)) {
					ModelNode realNode = (ModelNode)item;
					GetPrimaryNode(parent, realNode, true);
				}
			}
			if(parent.ModelTreeListNodeType == ModelTreeListNodeType.CollectionItem) {
				return GetCollectionItems(GetChildren(parent.PrimaryNode), parent);
			}
			else if(LinksEnabled && !parent.VirtualTreeNode) {
				AddAndFillLinksNode(parent);
			}
			return SortChilds(parent);
		}
		public override bool HasChildren(object nodeObject) {
			if(nodeObject == null || isDisposed) { return false; }
			ModelTreeListNode modelTreeListNode = (ModelTreeListNode)nodeObject;
			if(modelTreeListNode.ModelTreeListNodeType == ModelTreeListNodeType.Links) {
				if(modelTreeListNode.NodesCount == 0) {
					if(synchronizationObject.Disposed == null) {
						return true;
					}
					else {
						GetChildren(nodeObject);
					}
				}
			}
			if(modelTreeListNode.VirtualTreeNode && !IsChildrenVisibleInVirtualTree(modelTreeListNode)) {
				return false;
			}
			else {
				return modelTreeListNode.NodesCount > 0 || base.HasChildren(modelTreeListNode.ModelNode) || HasLinks(modelTreeListNode);
			}
		}
		public override System.Drawing.Image GetImage(object nodeObject, out string imageName) {
			ModelTreeListNode modelTreeListNode = (ModelTreeListNode)nodeObject;
			if(modelTreeListNode.ModelTreeListNodeType == ModelTreeListNodeType.Group) {
				imageName = "ModelEditor_Group";
				return ImageLoader.Instance.GetImageInfo(imageName).Image;
			}
			else if(modelTreeListNode.ModelTreeListNodeType == ModelTreeListNodeType.Links) {
				imageName = "ModelEditor_Links";
				return ImageLoader.Instance.GetImageInfo(imageName).Image;
			}
			return base.GetImage(modelTreeListNode.ModelNode, out imageName);
		}
		public override object GetParent(object nodeObject) {
			return ((ModelTreeListNode)nodeObject).Parent;
		}
		public override bool IsRoot(object nodeObject) {
			return ((ModelTreeListNode)nodeObject).ModelNode is IModelApplication;
		}
		public override string GetDisplayPropertyValue(object nodeObject) {
			return base.GetDisplayPropertyValue(((ModelTreeListNode)nodeObject).ModelNode);
		}
		#region ModelTreeListNode operation
		public ModelTreeListNode AddNode(ModelTreeListNode currentNode, ModelNode modelNode, Type type, string id) {
			manualEvent.WaitOne();
			ModelNode newModelNode = modelNode.AddNode(id, type);
			string associationPropertyName = SetAssociationPropertyIfNeed(currentNode, newModelNode);
			ModelTreeListNode result = CreatePrimaryNode(currentNode, newModelNode, associationPropertyName, false);
			OnCollectionChanged(currentNode);
			foreach(ModelTreeListNode link in result.Links) {
				OnCollectionChanged(link.Parent);
			}
			return result;
		}
		public ModelTreeListNode AddNode(ModelTreeListNode currentNode, Type type, string id) {
			manualEvent.WaitOne();
			ModelNode modelNode = currentNode.ModelNode.AddNode(id, type);
			string associationPropertyName = SetAssociationPropertyIfNeed(currentNode, modelNode);
			return AddNode(currentNode, modelNode, associationPropertyName);
		}
		public ModelTreeListNode AddNode(ModelTreeListNode currentNode, ModelNode modelNode, string associationPropertyName) {
#if DebugTest
			try {
#endif
				manualEvent.WaitOne();
				if(currentNode.ModelTreeListNodeType == ModelTreeListNodeType.Primary) {
					return CreatePrimaryNode(currentNode, modelNode, associationPropertyName, true);
				}
				else {
					ModelTreeListNode primaryNode = CreatePrimaryNode(currentNode.PrimaryNode, modelNode, associationPropertyName, false);
					return CreateLinkNode(currentNode, primaryNode);
				}
#if DebugTest
			}
			finally {
				System.Windows.Forms.Application.DoEvents();
			}
#endif
		}
		public ModelTreeListNode CloneNode(ModelTreeListNode currentNode, ModelTreeListNode sourceNode, bool cloneModelNode) {
			ModelTreeListNode parentNode = currentNode.ModelTreeListNodeType == ModelTreeListNodeType.Group ? currentNode.Parent : currentNode;
			return CloneNodeCore(sourceNode, parentNode, cloneModelNode);
		}
		public ModelTreeListNode CloneNode(ModelTreeListNode currentNode) {
			return CloneNodeCore(currentNode, currentNode.ParentIgnoreGroup, true);
		}
		public void DeleteNode(ModelTreeListNode modelTreeListNode, bool deleteModelNode) {
			try {
				BeginUpdate();
				ModelTreeListNode.LockModelNodeEvents++;
				ModelTreeListNode parent = modelTreeListNode.PrimaryNode.Parent;
				DeleteNodeCore(modelTreeListNode.PrimaryNode, deleteModelNode);
				RemoveGoupNodeIfNeed(parent);
			}
			finally {
				EndUpdate();
				ModelTreeListNode.LockModelNodeEvents--;
			}
		}
		public void DeleteNode(ModelTreeListNode modelTreeListNode) {
			DeleteNode(modelTreeListNode, true);
		}
		protected virtual void OnCollectionChanged(ModelTreeListNode node) {
			OnChanged(node, NodeChangedType.CollectionChanged);
		}
		protected virtual void OnDeleteNode(ModelTreeListNode node) {
			OnChanged(node, NodeChangedType.DeleteNode);
		}
		private void DeleteNodeCore(ModelTreeListNode node, bool deleteModelNode) {
			for(int counter = node.LinksCount - 1; counter > -1; counter--) {
				ModelTreeListNode link = node.GetLink(counter);
				DeleteNodeCore(link, deleteModelNode);
			}
			node.LinksClear();
			if(node.PrimaryNode.VirtualTreeNode && !node.PrimaryNode.IsRootVirtualTreeNode) {
				node.PrimaryNode.VirtualParent.VirtualNodes.Remove(node);
			}
			if(node.ModelTreeListNodeType == ModelTreeListNodeType.Primary && deleteModelNode) {
				((IModelNode)node.ModelNode).Remove();
			}
			node.Parent.Childs.Remove(node);
			List<ModelTreeListNode> virtualNodes = new List<ModelTreeListNode>();
			foreach(ModelTreeListNode virtualNode in node.PrimaryNode.VirtualNodes) {
				virtualNodes.Add(virtualNode);
			}
			foreach(ModelTreeListNode virtualNode in virtualNodes) {
				DeleteNodeCore(virtualNode, false);
			}
			if(node.PrimaryNode.IsRootVirtualTreeNode && deleteModelNode) {
				node.ResetVirtualTree(true);
			}
			OnDeleteNode(node);
			if(node.ModelTreeListNodeType == ModelTreeListNodeType.Primary) {
				node.ClearChilds(false, true);
			}
			OnCollectionChanged(node.Parent);
			RemoveAssociation(node.ModelNode);
			node.Dispose();
		}
		private string GetCopyNodeId(ModelNode parent, string sourceId) {
			string result = sourceId;
			while(parent[result] != null) {
				result += "_Copy";
			}
			return result;
		}
		private ModelTreeListNode CloneNodeCore(ModelTreeListNode currentNode, ModelTreeListNode parentNode, bool cloneModelNode) {
			string id = GetCopyNodeId(parentNode.ModelNode, currentNode.ModelNode.Id);
			ModelNode modelNode = null;
			if(cloneModelNode) {
				modelNode = ModelEditorHelper.AddCloneNode(parentNode.ModelNode, currentNode.ModelNode, id);
			}
			else {
				modelNode = currentNode.ModelNode;
			}
			if(currentNode.ModelTreeListNodeType == ModelTreeListNodeType.Primary) {
				return ClonePrimaryNode(parentNode, currentNode, modelNode, true);
			}
			else {
				ModelTreeListNode _primaryNode = ClonePrimaryNode(parentNode.PrimaryNode, currentNode.PrimaryNode, modelNode, false);
				return CloneLinkNode(parentNode, currentNode, _primaryNode);
			}
		}
		private ModelTreeListNode ClonePrimaryNode(ModelTreeListNode parent, ModelTreeListNode sourceNode, ModelNode modelNode, bool cloneLinks) {
			ModelTreeListNode _primaryNode = CreatePrimaryNodeCore(parent, modelNode);
			if(cloneLinks) {
				foreach(ModelTreeListNode link in sourceNode.Links) {
					CloneLinkNode(link.Parent, link, _primaryNode);
				}
			}
			return _primaryNode;
		}
		private ModelTreeListNode CloneLinkNode(ModelTreeListNode currentNode, ModelTreeListNode sourceNode, ModelTreeListNode _primaryNode) {
			ModelTreeListNode result = new ModelTreeListNode(this, currentNode, _primaryNode.ModelNode, sourceNode.ModelTreeListNodeType, _primaryNode);
			currentNode.Childs.Add(result);
			OnCollectionChanged(currentNode);
			return result;
		}
		private ModelTreeListNode CreateLinkNode(ModelTreeListNode currentNode, ModelTreeListNode primaryNode) {
			ModelTreeListNode linkItem = new ModelTreeListNode(this, currentNode, primaryNode.ModelNode, ModelTreeListNodeType.CollectionItem, primaryNode);
			currentNode.Childs.Add(linkItem);
			OnCollectionChanged(currentNode);
			return linkItem;
		}
		private string SetAssociationPropertyIfNeed(ModelTreeListNode currentNode, ModelNode modelNode) {
			if(currentNode.ModelTreeListNodeType == ModelTreeListNodeType.Collection ||
				currentNode.ModelTreeListNodeType == ModelTreeListNodeType.Links) {
				Type ownerType = currentNode.Owner.ModelNode.GetType();
				foreach(ModelValueInfo modelValueInfo in modelNode.NodeInfo.ValuesInfo) {
					if(modelValueInfo.PropertyType.IsAssignableFrom(ownerType)) {
						BrowsableAttribute browsableAttribute = fastModelEditorHelper.GetPropertyAttribute<BrowsableAttribute>(modelNode, modelValueInfo.Name);
						bool isVisible = browsableAttribute == null ? true : browsableAttribute.Browsable;
						if(isVisible) {
							Object newValue = currentNode.Owner.ModelNode;
							modelNode.SetValue(modelValueInfo.Name, newValue);
							return modelValueInfo.Name;
						}
					}
				}
			}
			return null;
		}
		private ModelTreeListNode CreatePrimaryNode(ModelTreeListNode parent, ModelNode modelNode, string associationPropertyName, bool createLinks) {
			ModelTreeListNode _primaryNode = CreatePrimaryNodeCore(parent, modelNode);
			if(associationPropertyName != null && createLinks) {
				ModelTreeListNode linkCollection = GetParentLink(_primaryNode, associationPropertyName);
				if(linkCollection != null) {
					ModelTreeListNode linkItem = new ModelTreeListNode(this, linkCollection, _primaryNode.ModelNode, ModelTreeListNodeType.CollectionItem, _primaryNode);
					linkCollection.Childs.Add(linkItem);
					OnCollectionChanged(parent);
				}
			}
			return _primaryNode;
		}
		private ModelTreeListNode CreatePrimaryNodeCore(ModelTreeListNode parent, ModelNode modelNode) {
			ModelTreeListNode _primaryNode = GetPrimaryNode(parent, modelNode, true); 
			AddAssociation(modelNode);
			OnCollectionChanged(_primaryNode.Parent);
			return _primaryNode;
		}
		private ModelTreeListNode GetParentLink(ModelTreeListNode _primaryNode, string associationPropertyName) {
			ModelTreeListNode result = null;
			ModelNode value = _primaryNode.ModelNode.GetValue(associationPropertyName) as ModelNode;
			if(value != null) {
				ModelTreeListNode primaryNode = FindPrimaryNode(value);
				if(primaryNode != null) {
					result = LinkCollectionByType(primaryNode, _primaryNode.ModelNode.GetType());
				}
			}
			return result;
		}
		internal void RefreshLinksIfNeed(ModelTreeListNode currentNode, string changedPropertyName, ModelNode oldValue) {
			ModelTreeListNode linkCollection = GetParentLink(currentNode, changedPropertyName);
			ModelValueInfo modelValueInfo = currentNode.ModelNode.NodeInfo.GetValueInfo(changedPropertyName);
			if(modelValueInfo == null) {
				return;
			}
			Type propertyType = modelValueInfo.PropertyType;
			ModelTreeListNode _linkItem = null;
			for(int counter = currentNode.LinksCount - 1; counter > -1; counter--) {
				ModelTreeListNode linkItem = currentNode.GetLink(counter);
				if(propertyType.IsAssignableFrom(linkItem.Owner.ModelNode.GetType())) {
					_linkItem = linkItem;
					ModelTreeListNode parent = linkItem.Parent;
					linkItem.Parent = linkCollection;
					parent.Childs.Remove(linkItem);
					RemoveAssociation(parent.Owner.ModelNode, linkItem.ModelNode, changedPropertyName);
					AddAssociation(_linkItem.ModelNode);
					if(linkCollection == null) {
						currentNode.LinkRemove(linkItem);
						linkItem.Dispose();
					}
					OnCollectionChanged(parent);
				}
			}
			if(currentNode.LinksCount == 0) {
				ModelNode value = currentNode.ModelNode.GetValue(changedPropertyName) as ModelNode;
				if(value != null && oldValue != null) {
					associationCollection.RemoveLink(oldValue, currentNode.ModelNode, changedPropertyName);
				}
			}
			if(linkCollection != null) {
				if(_linkItem == null) {
					_linkItem = new ModelTreeListNode(this, linkCollection, currentNode.ModelNode, ModelTreeListNodeType.CollectionItem, currentNode);
				}
				AddAssociation(_linkItem.ModelNode);
				linkCollection.Childs.Add(_linkItem);
				OnCollectionChanged(linkCollection);
			}
			else {
				ModelNode value = currentNode.ModelNode.GetValue(changedPropertyName) as ModelNode;
				if(value != null) {
					AddAssociation(currentNode.ModelNode);
				}
			}
		}
		private List<ModelTreeListNode> LinksCollection(ModelTreeListNode node) {
			foreach(ModelTreeListNode item in node.Childs) {
				if(item.ModelTreeListNodeType == ModelTreeListNodeType.Links) {
					return item.Childs;
				}
			}
			return new List<ModelTreeListNode>();
		}
		private ModelTreeListNode LinkCollectionByType(ModelTreeListNode node, Type itemType) {
			foreach(ModelTreeListNode item in LinksCollection(node)) {
				foreach(Type type in item.ModelNode.NodeInfo.GetChildrenTypes().Values) {
					if(type == itemType) {
						return item;
					}
				}
			}
			return null;
		}
		#endregion
		public void SortNodes(List<ModelTreeListNode> nodes) {
			nodes.Sort(new ModelTreeListNodeComparer(modelEditorHelper));
		}
		private static ModelNodeInfo dummyNodeInfo = null;
		private static ModelNodeInfo DummyNodeInfo {
			get {
				if(dummyNodeInfo == null) {
					dummyNodeInfo = ModelEditorHelper.CreateDummyNodeInfo();
				}
				return dummyNodeInfo;
			}
		}
		private bool HasLinks(ModelTreeListNode node) {
			if(node.ModelTreeListNodeType == ModelTreeListNodeType.Primary && node.ModelNode != null && !HideLinks(node)) {
				ITypeInfo nodeTypeInfo = XafTypesInfo.Instance.FindTypeInfo(node.ModelNode.GetType());
				return schemaHelper.HasLinks(nodeTypeInfo);
			}
			return false;
		}
		internal AssociationCollection AssociationCollection {
			get {
				return associationCollection;
			}
		}
		protected override void OnChanged() {
			if(lockEvents == 0) {
				Dictionary<ModelTreeListNode, NodeChangedType> collectionChanged = new Dictionary<ModelTreeListNode, NodeChangedType>();
				foreach(KeyValuePair<ModelTreeListNode, NodeChangedType> arg in nodeChangedEvents) {
					if(Changed != null) {
						if(arg.Value == NodeChangedType.CollectionChanged) {
							collectionChanged.Add(arg.Key, arg.Value);
						}
						else {
							Changed(this, new ModelTreeListNodeChangedEventArgs(arg.Key, arg.Value));
						}
					}
				}
				foreach(KeyValuePair<ModelTreeListNode, NodeChangedType> arg in collectionChanged) {
					if(Changed != null) {
						Changed(this, new ModelTreeListNodeChangedEventArgs(arg.Key, arg.Value));
					}
				}
				nodeChangedEvents.Clear();
			}
		}
		protected virtual void OnLinksCollecting() {
			if(LinksCollecting != null) {
				LinksCollecting(this, EventArgs.Empty);
			}
		}
		protected virtual void OnLinksCollected() {
			if(LinksCollected != null) {
				LinksCollected(this, EventArgs.Empty);
			}
		}
		private IEnumerable GetCollectionItems(IEnumerable primariItems, ModelTreeListNode owner) {
			foreach(ModelTreeListNode node in primariItems) {
				if(node.ModelTreeListNodeType != ModelTreeListNodeType.Links) {
					bool needAddLinkNode = true;
					foreach(ModelTreeListNode child in owner.Childs) {
						if(node.ModelNode.Id == child.ModelNode.Id) {
							needAddLinkNode = false;
							child.ModelNode = node.ModelNode;
							break;
						}
					}
					if(needAddLinkNode) {
						ModelTreeListNode result = new ModelTreeListNode(this, owner, node.ModelNode, ModelTreeListNodeType.CollectionItem, node);
						owner.Childs.Add(result);
					}
				}
			}
			return owner.Childs;
		}
		private void AddAndFillLinksNode(ModelTreeListNode node) {
			FillLinksCollection(node);
			if(node.ModelTreeListNodeType == ModelTreeListNodeType.Primary) {
				if(HasLinks(node.Owner)) {
					bool needAddLinkNode = true;
					foreach(ModelTreeListNode child in node.Childs) {
						if(child.ModelNode.Id == ModelTreeListNode.LinksId) {
							needAddLinkNode = false;
							break;
						}
					}
					if(needAddLinkNode) {
						ModelNode linksModelNode = new ModelNode(DummyNodeInfo, ModelTreeListNode.LinksId);
						linksModelNode.Index = -1;
						node.Childs.Add(new ModelTreeListNode(this, node, linksModelNode, ModelTreeListNodeType.Links));
					}
				}
			}
		}
		private void CollectLinkData(ModelNode modelNode) {
			if(synchronizationObject.Disposing != null) {
				return;
			}
			List<string> requiredPropertyes = null;
			if(!requiredPropertyesByType.TryGetValue(modelNode.GetType(), out requiredPropertyes)) {
				requiredPropertyes = new List<string>();
				foreach(ModelValueInfo modelValueInfo in modelNode.NodeInfo.ValuesInfo) {
					if(typeof(IModelNode).IsAssignableFrom(modelValueInfo.PropertyType) && fastModelEditorHelper.IsRequired(modelNode, modelValueInfo.Name)) {
						requiredPropertyes.Add(modelValueInfo.Name);
					}
					requiredPropertyesByType[modelNode.GetType()] = requiredPropertyes;
				}
			}
			List<ModelNode> modelNodeValues = new List<ModelNode>();
			requiredModelNodeValues[modelNode] = modelNodeValues;
			string parentId = modelNode.Parent != null && modelNode.Parent.Parent != null ? modelNode.Parent.Parent.Id : null;
			foreach(ModelValueInfo modelValueInfo in modelNode.NodeInfo.ValuesInfo) {
				if(typeof(IModelNode).IsAssignableFrom(modelValueInfo.PropertyType)) {
					ModelNode rootNode = modelNode.GetValue(modelValueInfo.Name) as ModelNode;
					if(rootNode != null && rootNode != modelNode && parentId != null && rootNode.Id != parentId) {
						if(requiredPropertyes.Contains(modelValueInfo.Name)) {
							modelNodeValues.Add(rootNode);
						}
						Dictionary<ModelNode, string> linkedValuesvalues = null;
						if(!linkData.TryGetValue(rootNode, out linkedValuesvalues)) {
							linkedValuesvalues = new Dictionary<ModelNode, string>();
							linkData[rootNode] = linkedValuesvalues;
						}
						linkedValuesvalues[modelNode] = modelValueInfo.Name;
					}
				}
				else {
					if(typeof(Type).IsAssignableFrom(modelValueInfo.PropertyType)) {
						Type item = modelNode.GetValue(modelValueInfo.Name) as Type;
						ModelNode rootNode = modelNode.Application.BOModel.GetClass(item) as ModelNode;
						if(rootNode != null && rootNode.Id != parentId) {
							Dictionary<ModelNode, string> linkedValuesvalues = null;
							if(!linkData.TryGetValue(rootNode, out linkedValuesvalues)) {
								linkedValuesvalues = new Dictionary<ModelNode, string>();
								linkData[rootNode] = linkedValuesvalues;
							}
							linkedValuesvalues[modelNode] = modelValueInfo.Name;
						}
					}
				}
			}
			foreach(ModelNode childNode in GetChildrenCore(modelNode)) {
				CollectLinkData(childNode);
			}
		}
		private static bool needForceRunModelGenerators = true;
		private void ForceRunModelGenerators(ModelNode modelNode) {
			if(needForceRunModelGenerators || DesignerOnlyCalculator.IsRunFromDesigner) {
				if(synchronizationObject.Disposing != null) {
					return;
				}
				foreach(ModelNode childNode in GetChildrenCore(modelNode)) {
					ForceRunModelGenerators(childNode);
				}
			}
		}
		private void CreateAssociationCollection() {
			foreach(KeyValuePair<ModelNode, Dictionary<ModelNode, string>> valuesData in linkData) {
				ModelNode rootNode = valuesData.Key;
				List<ModelNode> requiredValues = null;
				if(!requiredModelNodeValues.TryGetValue(rootNode, out requiredValues)) {
					requiredValues = new List<ModelNode>();
				}
				foreach(KeyValuePair<ModelNode, string> values in valuesData.Value) {
					if(!requiredValues.Contains(values.Key)) {
						AssociationCollection.AddLink(rootNode, values.Key, values.Value);
					}
				}
			}
		}
		private void CollectAssosiationsCore(ModelNodeInfo nodeInfo, List<Type> collectedTypes, Stack<string> path, KeyValuePair<string, Type> childrenType) {
			if(typeof(ModelNode).IsAssignableFrom(childrenType.Value)) {
				ModelNodeInfo listTypeNodeInfo = nodeInfo.ApplicationCreator.GetNodeInfo(childrenType.Value);
				path.Push(childrenType.Key);
				CollectAssosiations(listTypeNodeInfo, collectedTypes, path);
				path.Pop();
			}
		}
		private void CollectAssosiations(ModelNodeInfo nodeInfo, List<Type> collectedTypes, Stack<string> path) {
			if(!collectedTypes.Contains(nodeInfo.GeneratedClass)) {
				collectedTypes.Add(nodeInfo.GeneratedClass);
				foreach(ModelValueInfo valueInfo in nodeInfo.ValuesInfo) {
					if(!valueInfo.IsReadOnly) {
						bool propertyTypeIsType = typeof(Type).IsAssignableFrom(valueInfo.PropertyType);
						if(typeof(IModelNode).IsAssignableFrom(valueInfo.PropertyType) || propertyTypeIsType) {
							ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(valueInfo.PropertyType);
							BrowsableAttribute browsableAttribute = this.fastModelEditorHelper.GetPropertyAttribute<BrowsableAttribute>(typeInfo, valueInfo.Name);
							bool isVisible = browsableAttribute == null ? true : browsableAttribute.Browsable;
							if(isVisible) {
								Type collectionElementType = propertyTypeIsType ? typeof(IModelClass) : valueInfo.PropertyType;
								foreach(ModelNodeInfo valueNodeInfo in nodeInfo.ApplicationCreator.GetNodeInfos(collectionElementType)) {
									string[] arrayPath = path.ToArray();
									Array.Reverse(arrayPath);
									AssociationCollection.AddAssociation(nodeInfo.GeneratedClass, valueNodeInfo.GeneratedClass, valueInfo.Name, arrayPath);
								}
							}
						}
					}
				}
				foreach(KeyValuePair<string, Type> childrenType in nodeInfo.GetPropertyChildrenTypes()) {
					CollectAssosiationsCore(nodeInfo, collectedTypes, path, childrenType);
				}
				foreach(KeyValuePair<string, Type> childrenType in fastModelEditorHelper.GetListChildNodeTypes(nodeInfo)) {
					CollectAssosiationsCore(nodeInfo, collectedTypes, path, childrenType);
				}
			}
		}
		private void CollectAssosiations(ModelNodeInfo nodeInfo) {
			CollectAssosiations(nodeInfo, new List<Type>(), new Stack<string>());
		}
		private void Collect(IModelApplication modelApplication) {
			CollectAssosiations(((ModelNode)modelApplication).NodeInfo);
			CollectLinks((ModelNode)modelApplication);
		}
		private void CollectLinks(ModelNode rootNode) {
			ForceRunModelGenerators(rootNode);
			try {
				needForceRunModelGenerators = false;
				ModelEditorHelper.ModelCalculatorsCacheEnabled = true;
				ClearLinkDataCache();
				CollectLinkData(rootNode);
				CreateAssociationCollection();
				ClearLinkDataCache();
			}
			finally {
#if DebugTest
				needForceRunModelGenerators = true;
#endif
				ModelEditorHelper.ModelCalculatorsCacheEnabled = false;
			}
		}
		private void ClearLinkDataCache() {
			linkData.Clear();
			requiredModelNodeValues.Clear();
			requiredPropertyesByType.Clear();
		}
		private AssociationItem GetLinks(ModelNode rootNode) {
			return AssociationCollection.GetLinks(rootNode);
		}
		private void FillLinksCollection(ModelTreeListNode node) {
			if(node.ModelTreeListNodeType == ModelTreeListNodeType.Links) {
				manualEvent.WaitOne();
				ModelNode realNode = node.Owner.ModelNode;
				Dictionary<string, ModelTreeListNode> items_ = new Dictionary<string, ModelTreeListNode>();
				foreach(ModelTreeListNode item in node.Childs) {
					items_.Add(item.ModelNode.Id, item);
				}
				AssociationItem _associationItem = GetLinks(realNode);
				if(_associationItem != null) {
					foreach(KeyValuePair<AssociationInfo, List<ModelNode>> associationItem in _associationItem.Links) {
						foreach(ModelNode item in associationItem.Value) {
							ModelTreeListNode linkNode = null;
							if(!items_.TryGetValue(item.Parent.Id, out linkNode)) {
								linkNode = new ModelTreeListNode(this, node, item.Parent, ModelTreeListNodeType.Collection, GetPrimaryNode(item.Parent, true));
								items_.Add(item.Parent.Id, linkNode);
								node.Childs.Add(linkNode);
							}
							bool needAdd = true;
							foreach(ModelTreeListNode linkItem in linkNode.Childs) {
								if(linkItem.ModelNode == item) {
									needAdd = false;
									break;
								}
							}
							if(needAdd) {
								ModelTreeListNode primaryNode = GetPrimaryNode(item, true);
								ModelTreeListNode listItem = new ModelTreeListNode(this, linkNode, item, ModelTreeListNodeType.CollectionItem, primaryNode);
								linkNode.Childs.Add(listItem);
							}
						}
					}
				}
			}
		}
		private ModelNode GetLogicalParent(ModelTreeListNode parentTreeNode, ModelNode node) {
			IModelNode result = null;
			if(parentTreeNode != null) {
				result = parentTreeNode.GetLogicalParent(node);
			}
			if(result == null) {
				result = node.Parent;
			}
			return (ModelNode)result;
		}
		internal ModelTreeListNode GetPrimaryNode(ModelTreeListNode parentTreeNode, ModelNode modelNode, bool isBuild) {
			List<ModelNode> nodePath = new List<ModelNode>();
			nodePath.Add(modelNode);
			ModelNode parent = GetLogicalParent(parentTreeNode, modelNode);
			while(parent != null) {
				nodePath.Add(parent);
				parent = GetLogicalParent(parentTreeNode, parent);
			}
			nodePath.Reverse();
			ModelTreeListNode result = GetPrimaryNode(nodePath, RootNode, modelNode.GetType(), isBuild);
			if(result != null && result.VirtualTreeNode && !result.IsRootVirtualTreeNode && result.VirtualParent == null) {
				ModelTreeListNode realNode = GetPrimaryNode(null, modelNode, isBuild);
				result.VirtualParent = realNode;
				realNode.VirtualNodes.Add(result);
			}
			return result;
		}
		private ModelTreeListNode GetPrimaryNode(ModelNode modelNode, bool isBuild) {
			return GetPrimaryNode(null, modelNode, isBuild);
		}
		private ModelTreeListNode GetPrimaryNode(List<ModelNode> nodePath, ModelTreeListNode parentNode, Type nodeType, bool isBuild) {
			if(nodePath[0].Id == parentNode.ModelNode.Id && nodePath.Count == 1) {
				return parentNode;
			}
			nodePath.Remove(nodePath[0]);
			if(nodePath.Count > 0) {
				return GetPrimaryNodeCore(nodePath, parentNode, nodeType, isBuild);
			}
			else {
				return null;
			}
		}
		private bool HideLinks(ModelTreeListNode node) {
			if(node != null && node.ModelNode != null) {
				bool result;
				Type modelNodeType = node.ModelNode.GetType();
				if(!hideLinks.TryGetValue(modelNodeType, out result)) {
					ModelHideLinksAttribute[] attributes = AttributeHelper.GetAttributesConsideringInterfaces<ModelHideLinksAttribute>(modelNodeType, true);
					result = attributes != null && attributes.Length > 0;
					hideLinks[modelNodeType] = result;
				}
				return result;
			}
			return false;
		}
		private bool IsChildrenVisibleInVirtualTree(ModelTreeListNode node) {
			if(node != null && node.ModelNode != null) {
				Type modelNodeType = node.ModelNode.GetType();
				IModelIsVisible logic;
				if(!hideChildrenInVirtualTree.TryGetValue(modelNodeType, out logic)) {
					ModelVirtualTreeHideChildrenAttribute[] attributes = AttributeHelper.GetAttributesConsideringInterfaces<ModelVirtualTreeHideChildrenAttribute>(modelNodeType, true);
					if(attributes != null && attributes.Length > 0) {
						Type visibilityCalculatorType = null;
						foreach(ModelVirtualTreeHideChildrenAttribute att in attributes) {
							if(att.VisibilityCalculatorType != null && typeof(IModelIsVisible).IsAssignableFrom(att.VisibilityCalculatorType)) {
								visibilityCalculatorType = att.VisibilityCalculatorType;
								break;
							}
						}
						if(visibilityCalculatorType == null) {
							hideChildrenInVirtualTree[modelNodeType] = null;
						}
						else {
							logic = (IModelIsVisible)Activator.CreateInstance(visibilityCalculatorType);
							hideChildrenInVirtualTree[modelNodeType] = logic;
						}
					}
				}
				if(logic != null) {
					return logic.IsVisible(node.ModelNode, null);
				}
			}
			return true;
		}
		private ModelTreeListNode GetPrimaryNodeCore(List<ModelNode> _nodePath, ModelTreeListNode parentNode, Type nodeType, bool isBuild) {
			ModelTreeListNode result = null;
			List<ModelNode> nodePath = new List<ModelNode>(_nodePath);
			ModelNode currentTargetNode = nodePath[0];
			nodePath.Remove(currentTargetNode);
			foreach(ModelTreeListNode item in parentNode.Childs) {
				if(item.ModelTreeListNodeType == ModelTreeListNodeType.Group) {
					if(IsTargetGroup(item.ModelNode, currentTargetNode)) {
						result = GetPrimaryNodeCore(_nodePath, item, nodeType, isBuild);
					}
				}
				if(result != null) {
					break;
				}
				if(item.ModelNode.Id == currentTargetNode.Id && currentTargetNode.GetType().IsAssignableFrom(item.ModelNode.GetType())) {
					result = item;
					if(nodePath.Count > 0) {
						result = GetPrimaryNodeCore(nodePath, result, nodeType, isBuild);
						break;
					}
					else {
						if(nodePath.Count == 0 && !nodeType.IsAssignableFrom(item.ModelNode.GetType())) {
							result = null;
						}
						if(result != null) {
							break;
						}
					}
				}
			}
			if(result == null && isBuild) {
				if(parentNode.ModelTreeListNodeType != ModelTreeListNodeType.Group || IsTargetGroup(parentNode.ModelNode, currentTargetNode)) {
					result = new ModelTreeListNode(this, parentNode, currentTargetNode, ModelTreeListNodeType.Primary);
					parentNode.Childs.Add(result);
					if(nodePath.Count > 0) {
						result = GetPrimaryNodeCore(nodePath, result, nodeType, isBuild);
					}
				}
			}
			return result;
		}
		private bool IsTargetGroup(ModelNode parentNode, ModelNode currentTargetNode) {
			return parentNode.Id == GetGroupName(GetGroupPropertyName(currentTargetNode.Parent), currentTargetNode);
		}
		private void RegisterGroupAlias(Type collectionNodeType, string propertyName) {
			groupCollectionNodeTypes.Add(collectionNodeType, propertyName);
		}
		private string GetGroupPropertyName(ModelNode node) {
			string result = null;
			foreach(Type item in groupCollectionNodeTypes.Keys) {
				if(item.IsAssignableFrom(node.GetType())) {
					groupCollectionNodeTypes.TryGetValue(item, out result);
					break;
				}
			}
			return result;
		}
		private string GetGroupName(string propertyName, ModelNode node) {
			string groupName = "";
			if(node != null) {
				ITypeInfo nodeTypeInfo = XafTypesInfo.Instance.FindTypeInfo(node.NodeInfo.GeneratedClass);
				IMemberInfo memberInfo = nodeTypeInfo.FindMember(propertyName);
				if(memberInfo != null) {
					string typeName = (string)memberInfo.GetValue(node);
					if(!string.IsNullOrEmpty(typeName)) {
						ITypeInfo valueTypeInfo = XafTypesInfo.Instance.FindTypeInfo(typeName);
						if(valueTypeInfo != null) {
							groupName = valueTypeInfo.Type.Namespace;
						}
					}
				}
			}
			if(string.IsNullOrEmpty(groupName)) {
				groupName = UnspecifiedGroupName;
			}
			return groupName;
		}
		private void GroupChildsCore(string propertyName, ModelTreeListNode parentNode) {
			Dictionary<string, ModelTreeListNode> groupNodesTable = new Dictionary<string, ModelTreeListNode>();
			foreach(ModelTreeListNode node in parentNode.Childs) {
				if(node.ModelTreeListNodeType == ModelTreeListNodeType.Group) {
					groupNodesTable.Add(node.ModelNode.Id, node);
				}
				else {
					string groupName = GetGroupName(propertyName, node.ModelNode);
					ModelTreeListNode group;
					if(!groupNodesTable.TryGetValue(groupName, out group)) {
						group = new ModelTreeListNode(this, parentNode, new ModelNode(DummyNodeInfo, groupName), ModelTreeListNodeType.Group);
						groupNodesTable.Add(groupName, group);
					}
					group.Childs.Add(node);
					node.Parent = group;
				}
			}
			parentNode.Childs.Clear();
			parentNode.Childs.AddRange(groupNodesTable.Values);
		}
		private void SetGroupSettings(string id, bool value) {
			if(_groupSettings.ContainsKey(id)) {
				_groupSettings[id] = value;
			}
			else {
				_groupSettings.Add(id, value);
			}
		}
		private bool GetGroupSettings(string id) {
			bool result;
			if(_groupSettings.TryGetValue(id, out result)) {
				return result;
			}
			return true;
		}
		private void ClearAssociation() {
			associationCollection.ClearAssociation();
		}
		private void SetGroupFlag(ModelTreeListNode node, bool isGroupChilds) {
			node.Grouped = isGroupChilds;
			SetGroupSettings(node.ModelNode.Id, node.Grouped);
		}
		private class LinksCollectionCalculator : SchemaTreeListObjectAdapter {
			private List<Type> hasLinksCashe = null;
			public bool HasLinks(ITypeInfo itemType) {
				if(!ExtendModelInterfaceAdapter.LinksEnabled) {
					return false;
				}
				if(itemType.Base.Type.IsGenericType &&
						itemType.Base.Type.GetGenericTypeDefinition() == typeof(ModelNodeList<>)) {
					return false;
				}
				if(hasLinksCashe == null) {
					hasLinksCashe = new List<Type>();
					Guard.ArgumentNotNull(ModelApplication, "ModelApplication");
					ITypeInfo applicationModelTypeInfo = XafTypesInfo.Instance.FindTypeInfo(ModelApplication.GetType());
					foreach(object item in GetAllChilder(applicationModelTypeInfo, this)) {
						ITypeInfo schemaElement = (ITypeInfo)item;
						foreach(XafMemberInfo member in schemaElement.OwnMembers) {
							if(!member.MemberTypeInfo.IsListType) { 
								foreach(TypeInfo typeInd in member.MemberTypeInfo.Implementors) {
									if(typeInd.Type.IsSubclassOf(typeof(ModelNode))) {
										hasLinksCashe.Add(typeInd.Type);
									}
								}
							}
						}
					}
				}
				return hasLinksCashe.Contains(itemType.Type);
			}
			private IEnumerable GetAllChilder(object parent, SchemaTreeListObjectAdapter adapter) {
				List<object> result = new List<object>();
				foreach(object item in adapter.GetChildren(parent)) {
					if(adapter.HasChildren(item)) {
						result.AddRange(GetChildren(item, adapter));
					}
				}
				return result;
			}
			private List<object> GetChildren(object parent, SchemaTreeListObjectAdapter adapter) {
				List<object> result = new List<object>();
				result.Add(parent);
				foreach(object item in adapter.GetChildren(parent)) {
					result.AddRange(GetChildren(item, adapter));
				}
				return result;
			}
			#region IDisposable Members
			public override void Dispose() {
				base.Dispose();
				if(hasLinksCashe != null) {
					hasLinksCashe.Clear();
					hasLinksCashe = null;
				}
			}
			#endregion
		}
		#region IDisposable Members
		private bool isDisposed = false;
		public void Dispose() {
			isDisposed = true;
			StopCollect();
			if(manualEvent != null) {
				manualEvent.WaitOne();
				manualEvent = null;
			}
			if(RootNode != null) {
				RootNode.ClearChilds(false);
				RootNode.Dispose();
				RootNode = null;
			}
			if(loader != null) {
				loader.Dispose();
				loader = null;
			}
			if(associationCollection != null) {
				associationCollection.Dispose();
				associationCollection = null;
			}
			if(_groupSettings != null) {
				_groupSettings.Clear();
				_groupSettings = null;
			}
			if(Changed != null) {
				Changed = null;
			}
			if(schemaHelper != null) {
				schemaHelper.Dispose();
				schemaHelper = null;
			}
			if(requiredModelNodeValues != null) {
				foreach(List<ModelNode> items in requiredModelNodeValues.Values) {
					items.Clear();
				}
				requiredModelNodeValues.Clear();
				requiredModelNodeValues = null;
			}
			if(requiredPropertyesByType != null) {
				foreach(List<string> items in requiredPropertyesByType.Values) {
					items.Clear();
				}
				requiredPropertyesByType.Clear();
				requiredPropertyesByType = null;
			}
			if(linkData != null) {
				foreach(Dictionary<ModelNode, string> items in linkData.Values) {
					items.Clear();
				}
				linkData.Clear();
				linkData = null;
			}
			if(groupCollectionNodeTypes != null) {
				groupCollectionNodeTypes.Clear();
				groupCollectionNodeTypes = null;
			}
			if(groupedDefaultItems != null) {
				groupedDefaultItems.Clear();
				groupedDefaultItems = null;
			}
			if(nodeChangedEvents != null) {
				nodeChangedEvents.Clear();
				nodeChangedEvents = null;
			}
			modelEditorHelper = null;
		}
		private void StopCollect() {
			synchronizationObject.Disposing = new object();
			if(loader != null) {
				int counter = 0;
				while(synchronizationObject.Disposed == null && counter < 200) {
					Thread.Sleep(10);
					counter++;
				}
			}
			synchronizationObject.Disposing = null;
			synchronizationObject.Disposed = null;
		}
		BackgroundWorker loader = null;
		private void CollectAll() {
			if(manualEvent == null) {
				if(loadLinkInInnerThread) {
					OnLinksCollecting();
					manualEvent = new ManualResetEvent(false);
					loader = new BackgroundWorker();
					ModelTreeListNode.LockModelNodeEvents++;
					loader.DoWork += new DoWorkEventHandler(loader_DoWork);
					loader.RunWorkerAsync();
				}
				else {
					manualEvent = new ManualResetEvent(false);
					Collect(RootNode.ModelNode.Application);
					manualEvent.Set();
				}
			}
		}
		private void loader_DoWork(object sender, DoWorkEventArgs e) {
			try {
				Collect(RootNode.ModelNode.Application);
			}
			catch(Exception ex) {
				if(ModelEditorViewController.AskConfirmation) {
					ModelEditorViewController.ShowError(ex, errorMessage);
				}
			}
			finally {
				try {
					manualEvent.Set();
					ModelTreeListNode.LockModelNodeEvents--;
					if(synchronizationObject.Disposing == null) {
						OnLinksCollected();
					}
				}
				finally {
					synchronizationObject.Disposed = new object();
				}
			}
		}
		#endregion
		public new event EventHandler<ModelTreeListNodeChangedEventArgs> Changed;
		public event EventHandler LinksCollecting;
		public event EventHandler LinksCollected;
		private class SynchronizationObject {
			private object disposing = null;
			private object disposed = null;
			public object Disposing {
				get {
					return disposing;
				}
				set {
					disposing = value;
				}
			}
			public object Disposed {
				get {
					return disposed;
				}
				set {
					disposed = value;
				}
			}
		}
#if DebugTest
		public static ModelNodeInfo DebugTest_DummyNodeInfo {
			get { return DummyNodeInfo; }
		}
		public bool DebugTest_HasLinks(ModelTreeListNode node) {
			return HasLinks(node);
		}
		public AssociationCollection DebugTest_AssociationCollection {
			get { return AssociationCollection; }
		}
#endif
	}
	public enum NodeChangedType { DeleteNode, AddNode, CollectionChanged, ObjectChanged, GroupChanged };
	public class ModelTreeListNodeChangedEventArgs : EventArgs {
		ModelTreeListNode node;
		NodeChangedType nodeChangedType;
		public ModelTreeListNodeChangedEventArgs(ModelTreeListNode node, NodeChangedType nodeChangedType) {
			this.node = node;
			this.nodeChangedType = nodeChangedType;
		}
		public ModelTreeListNode Node {
			get {
				return node;
			}
		}
		public NodeChangedType NodeChangedType {
			get {
				return nodeChangedType;
			}
		}
	}
	public class ModelTreeListNodeComparer : ModelNodesComparer, IComparer<ModelTreeListNode> {
		Dictionary<int, int?> indexCache = new Dictionary<int, int?>();
		Dictionary<int, string> nodesDisplayValue = new Dictionary<int, string>();
		private bool compareByIndex = true;
		private FastModelEditorHelper fastModelEditorHelper;
		private ModelTreeListNodeComparer() { }
		private string GetModelNodeDisplayValue_Cached(IModelNode modelNode) {
			string result;
			if(!nodesDisplayValue.TryGetValue(modelNode.GetHashCode(), out result)) {
				result = fastModelEditorHelper.GetModelNodeDisplayValue(modelNode);
				nodesDisplayValue.Add(modelNode.GetHashCode(), result);
			}
			return result;
		}
		protected override bool ShouldCompareByIndex() {
			return compareByIndex;
		}
		protected override int? GetModelNodeIndex(IModelNode modelNode) {
			int modelNodeHashCode = modelNode.GetHashCode();
			int? result;
			if(!indexCache.TryGetValue(modelNodeHashCode, out result)) {
				result = modelNode.Index;
				indexCache.Add(modelNodeHashCode, result);
			}
			return result;
		}
		protected override string GetModelNodeDisplayValue(IModelNode modelNode) {
			return GetModelNodeDisplayValue_Cached(modelNode);
		}
		public ModelTreeListNodeComparer(FastModelEditorHelper fastModelEditorHelper) {
			this.fastModelEditorHelper = fastModelEditorHelper;
		}
		public ModelTreeListNodeComparer(FastModelEditorHelper fastModelEditorHelper, bool compareByIndex)
			: this(fastModelEditorHelper) {
			this.compareByIndex = compareByIndex;
		}
		public int Compare(ModelTreeListNode node1, ModelTreeListNode node2) {
			return base.Compare(node1.ModelNode, node2.ModelNode);
		}
	}
}
