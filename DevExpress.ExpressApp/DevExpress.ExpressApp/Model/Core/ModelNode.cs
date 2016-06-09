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
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Utils.Reflection;
namespace DevExpress.ExpressApp.Model.Core {
	public static class ModelValueNames {
		internal const string Path = "Path";
		public const string Id = "Id";
		public const string Index = "Index";
		public const string IsNewNode = "IsNewNode";
		public const string IsRemovedNode = "Removed";
	}
#if DEBUG
	[DebuggerDisplay("{DebuggerString,nq}, Layer Index = {LayerIndex}")]
#else
	[DebuggerDisplay("{DebuggerString,nq}")]
#endif
	public class ModelNode : IModelNode, IModelNodeHiddenMethods {
		#region DebuggerDisplay
		private string DebuggerString {
			get { return string.Format("{0}, Id = {1}", GetType().Name, Id); }
		}
#if DEBUG
		private int LayerIndex {
			get { return Master != null ? Master.Layers.IndexOf(this) : -1; }
		}
#endif
		#endregion
		enum NodesGeneratorStatus { NotStarted, InProgress, Done }
		[Flags]
		enum ModelNodeState { None = 0, New = 1, Removed = 2, Loaded = 4, Updated = 8 }
		const int MinNodeCountToUseDictionary = 5;
		static readonly ModelNode[] EmptyNodes = new ModelNode[0];
		static int modelNodeCounter;
		readonly ModelNodeInfo nodeInfo;
		readonly int hashCode;
		string id;
		ModelNode parent;
		ModelMultipleMasterStoreItem masterItem;
		ModelNode root;
		ModelNode restoreNode;
		List<ModelNode> nodes;
		List<ModelNode> layers;
		Dictionary<string, ModelNode> idNodes;
		NodesGeneratorStatus nodesGeneratorStatus;
		int nodesGeneratorInProgressCounter;
		ModelNode[] sortedNodes;
		ModelNodeState state;
		PropertyChangingEventHandler propertyChanging;
		PropertyChangedEventHandler propertyChanged;
		#region ModelValueStore
		private static readonly IEnumerable<String> EmptyModelValueNames = new String[0];
		private static readonly IEnumerable<IModelValue> EmptyModelValues = new IModelValue[0];
		private IDictionary<String, IModelValue> modelValueStore;
		private HashSet<String> masterNullModelValues;
		private IEnumerable<String> ModelValueNamesCore {
			get { return modelValueStore != null ? modelValueStore.Keys : EmptyModelValueNames; }
		}
		private IEnumerable<IModelValue> ModelValuesCore {
			get { return modelValueStore != null ? modelValueStore.Values : EmptyModelValues; }
		}
		private Int32 ModelValuesCountCore {
			get { return modelValueStore != null ? modelValueStore.Count : 0; }
		}
		private void SetModelValueCore(String valueName, IModelValue value) {
			if(modelValueStore == null) {
				modelValueStore = new Dictionary<String, IModelValue>();
			}
			modelValueStore[valueName] = value;
		}
		private Boolean ContainsModelValueCore(String valueName) {
			return modelValueStore != null && modelValueStore.ContainsKey(valueName);
		}
		private IModelValue FindModelValueCore(String valueName) {
			IModelValue result;
			if(modelValueStore != null && modelValueStore.TryGetValue(valueName, out result)) {
				return result;
			}
			return null;
		}
		private void RemoveModelValueCore(String valueName) {
			if(modelValueStore != null) {
				modelValueStore.Remove(valueName);
			}
		}
		private void ClearModelValuesCore() {
			modelValueStore = null;
		}
		private void SetMasterNullModelValue(String valueName) {
			if(masterNullModelValues == null) {
				masterNullModelValues = new HashSet<String>();
			}
			masterNullModelValues.Add(valueName);
		}
		private Boolean IsMasterNullModelValue(String valueName) {
			return masterNullModelValues != null && masterNullModelValues.Contains(valueName);
		}
		private void RemoveMasterModelNullValue(String valueName) {
			if(masterNullModelValues != null) {
				masterNullModelValues.Remove(valueName);
			}
		}
		private void ClearMasterModelNullValues() {
			masterNullModelValues = null;
		}
		#endregion
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ModelNode(ModelNodeInfo nodeInfo, string nodeId) {
			Guard.ArgumentNotNull(nodeInfo, "nodeInfo");
			this.nodeInfo = nodeInfo;
			id = !string.IsNullOrEmpty(nodeId) ? nodeId : GenerateAutoId();
			root = this;
			hashCode = modelNodeCounter++;
		}
		[Description("Indicates the current node identifier.")]
		public string Id {
			get { return id; }
			set { SetId(value); }
		}
		void SetId(string value) {
			if(id == value && !string.IsNullOrEmpty(value)) {
				return;
			}
			bool hasSameRemovedNode = false;
			if(parent != null && !string.IsNullOrEmpty(value)) {
				ModelNode sameNode = Parent.GetNode(value);
				if(sameNode != null) {
					throw new DuplicateModelNodeIdException(sameNode.Id, sameNode.Path);
				}
				sameNode = parent.GetNodeCore(value);
				if(sameNode != null && sameNode != this) {
					if(sameNode.IsRemovedNode) {
						parent.RemoveNodeFromList(sameNode);
						hasSameRemovedNode = true;
					}
					else {
						throw new DuplicateModelNodeIdException(sameNode.Id, sameNode.Path);
					}
				}
			}
			OnPropertyChanging(ModelValueNames.Id);
			UpdateNodeId(value);
			if(hasSameRemovedNode) {
				IsRemovedNode = true;
			}
			OnPropertyChanged(ModelValueNames.Id);
		}
		void UpdateNodeId(string value) {
			ModelNode masterNode = null;
			if(!IsRoot) {
				masterNode = IsMaster ? this : Master;
			}
			if(masterNode != null) {
				masterNode.UpdateNodeIdCore(value);
				foreach(ModelNode layer in masterNode.Layers) {
					layer.UpdateNodeIdCore(value);
				}
			}
			else {
				UpdateNodeIdCore(value);
			}
		}
		void UpdateNodeIdCore(string value) {
			string oldId = Id;
			this.id = string.IsNullOrEmpty(value) ? GenerateAutoId() : value;
			if(!string.IsNullOrEmpty(oldId)) {
				UpdateParentIdNodes(oldId, Id);
			}
		}
		private bool GetState(ModelNodeState state) {
			return (this.state & state) == state;
		}
		private void SetState(ModelNodeState state, bool value) {
			if(value) {
				this.state |= state;
			}
			else {
				this.state &= ~state;
			}
		}
		void SetParent(ModelNode node) {
			this.parent = node;
			UpdateRoot();
		}
		void UpdateRoot() {
			this.root = parent != null ? parent.root : this;
			if(Nodes != null) {
				foreach(ModelNode node in Nodes) {
					node.UpdateRoot();
				}
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string KeyValueName { get { return NodeInfo.KeyName; } }
		[Browsable(false)]
		public ModelNode Parent { get { return parent; } }
		[Browsable(false)]
		public ModelNode Root { get { return root; } }
		ModelNode RestoreNode { get { return restoreNode; } }
		bool GetHasMultipleMasters() {
			ModelApplicationBase app = Root as ModelApplicationBase;
			return app != null && app.HasMultipleMasters;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ModelNode Master {
			get {
				if(IsNodesGeneratorInProgress) {
					return null;
				}
				ModelMultipleMasterStoreItem masterNodeItem = MasterItem;
				if(!masterNodeItem.IsMasterCalculated) {
					lock(TypesInfo.lockObject) {
						EnsureMaster(masterNodeItem);
					}
				}
				return masterNodeItem.Master;
			}
			private set { SetMaster(value); }
		}
		protected virtual void SetMaster(ModelNode value) {
			MasterItem.Master = value;
		}
		ModelMultipleMasterStoreItem MasterItem {
			get {
				if(HasMultipleMasters) {
					return ModelMultipleMasterStore.Instance.GetMasterItem(this);
				}
				if(masterItem == null) {
					masterItem = new ModelMultipleMasterStoreItem();
				}
				return masterItem;
			}
		}
		ModelNode GetMasterCore() {
			return !IsNodesGeneratorInProgress ? MasterItem.Master : null;
		}
		bool HasMultipleMasters { get { return ModelMultipleMasterStore.Instance != null && GetHasMultipleMasters(); } }
		[Browsable(false)]
		public bool IsRoot { get { return Parent == null; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsMaster { get { return Layers != null; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsSlave { get { return Root.GetIsSlave(); } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsSeparate { get { return !IsMaster && !IsSlave; } }
		[Browsable(false)]
		public IModelApplication Application { get { return Root as IModelApplication; } }
		public ModelNode this[string name] { get { return GetNode(name); } }
		public ModelNode this[int index] { get { return GetNode(index); } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public List<ModelNode> GetUnsortedChildren() {
			return new List<ModelNode>(GetUnsortedChildren(false));
		}
		protected virtual bool GetIsSlave() {
			return Master != null;
		}
		internal ModelNode[] GetNodes() {
			return GetUnsortedChildren(false);
		}
		internal ModelNode[] GetChildrenForSerialization() {
			ModelNode[] list = GetUnsortedChildren(true);
			if(!IsUnusableNode) {
				Array.Sort<ModelNode>(list, SortChildNodesHelper.DoSortNodesByDefault);
			}
			return list;
		}
		ModelNode[] GetUnsortedChildren(bool inThisLayer) {
			lock(TypesInfo.lockObject) {
				ModelNode master = Master;
				if(GetNodeOrValueInThisLayer(master, inThisLayer)) {
					EnsureNodes(false);
					return GetFirstLayers(inThisLayer);
				}
				return master.GetUnsortedChildren(inThisLayer);
			}
		}
		protected List<ModelNode> Layers { get { return layers; } }
		private int NodesCountCore { get { return Nodes != null ? Nodes.Count : 0; } }
		public ModelNodeInfo NodeInfo { get { return nodeInfo; } }
		ModelNodesGeneratorBase NodesGenerator { get { return NodeInfo.NodesGenerator; } }
		public int? Index {
			get { return GetValue<int?>(ModelValueNames.Index); }
			set { SetValue<int?>(ModelValueNames.Index, value); }
		}
		object GetNullValue(string name) {
			ModelValueInfo valueInfo = GetValueInfo(name);
			return valueInfo != null ? valueInfo.DefaultValue : null;
		}
		object GetDefaultValue(string name) {
			return NodeInfo.GetDefaultValue(this, name);
		}
		bool IsNodesGeneratorInProgressInLayer { get { return NodesGeneratorInProgressCounter > 0; } }
		bool GetNodeOrValueInThisLayer(ModelNode master, bool inThisLayer) {
			return inThisLayer || master == null || IsNodesGeneratorInProgressInLayer;
		}
		bool IsNodeGeneratorNotStarted { get { return this.nodesGeneratorStatus == NodesGeneratorStatus.NotStarted; } }
		bool IsInNodesGenerator() {
			if(IsMaster) {
				return false;
			}
			ModelNode node = this;
			while(!node.IsRoot && !node.IsNodesGeneratorInProgress) {
				node = node.Parent;
			}
			return node.IsNodesGeneratorInProgress;
		}
		internal bool IsNodesGeneratorInProgress {
			get { return nodesGeneratorStatus == NodesGeneratorStatus.InProgress; }
			set {
				if(IsNodesGeneratorInProgress == value) return;
				if(value) {
					this.nodesGeneratorStatus = NodesGeneratorStatus.InProgress;
					NodesGeneratorInProgressCounter++;
				}
				else {
					this.nodesGeneratorStatus = NodesGeneratorStatus.Done; 
					NodesGeneratorInProgressCounter--;
				}
			}
		}
		int NodesGeneratorInProgressCounter {
			get { return Root.nodesGeneratorInProgressCounter; }
			set { Root.nodesGeneratorInProgressCounter = value; }
		}
		public NodeType AddNode<NodeType>() {
			return AddNode<NodeType>(null);
		}
		public NodeType AddNode<NodeType>(string id) {
			return (NodeType)((object)AddNode(id, typeof(NodeType)));
		}
		public ModelNode AddNode(Type type) {
			return AddNode(null, type);
		}
		public ModelNode AddNode(string id, Type type) {
			ModelNode writableLayer = GetLayerForModification();
			return writableLayer.AddNodeCore(id, type);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ModelNode CreateNode() {
			return NodeInfo.CreateNode(Id);
		}
		void AddNodeCore(ModelNode node) {
			EnsureNodes(true);
			if(IsSeparate || IsInNodesGenerator()) {
				ModelNode sameNode = GetNodeCore(node.Id);
				if(sameNode != null) {
					if(sameNode.IsRemovedNode) {
						RemoveNodeFromList(sameNode);
					}
					else {
						throw new DuplicateModelNodeIdException(sameNode.Id, sameNode.Path);
					}
				}
				AddNodeIntoList(node);
				node.SetParent(this);
				node.IsNewNode = IsNewNode;
			}
			else {
				ModelNode sameNode = GetNode(node.Id);
				if(sameNode != null) {
					throw new DuplicateModelNodeIdException(sameNode.Id, sameNode.Path);
				}
				sameNode = GetNodeCore(node.Id);
				if(sameNode != null) {
					if(sameNode.IsRemovedNode) {
						RemoveNodeFromList(sameNode);
					}
					else {
						throw new DuplicateModelNodeIdException(sameNode.Id, sameNode.Path);
					}
				}
				AddNodeIntoList(node);
				node.SetParent(this);
				node.IsNewNode = !IsInFirstLayer;
				node.IsRemovedNode = sameNode != null;
				ModelNode master = GetMasterCore();
				if(master != null) {
					master.Reset();
				}
			}
			CreatorInstance.OnNodeAdded(node);
		}
		ModelNode AddNodeCore(string id, Type type) {
			string exceptionText;
			string checkedId = id;
			if(!CreatorInstance.CanCreateNodeByType(this, ref checkedId, type, out exceptionText)) {
				throw new CannotCreateModelNodeByTypeException(type, exceptionText);
			}
			ModelNode node = CreatorInstance.CreateNode(checkedId, type);
			AddNodeCore(node);
			return node;
		}
		internal void UndoSelf() {
			if(!IsRoot) {
				lock(TypesInfo.lockObject) {
					ModelNode node = GetWritableLayer();
					if(node != null) {
						ModelNode parent = node.Parent;
						node.Parent.RemoveNodeFromList(node);
						node.Parent.OnRemoveNode(node);
					}
				}
			}
		}
		public void Undo() {
			if(HasModification || RestoreNode != null) {
				ModelNode node = GetLayerForModification();
				node.UndoCore();
			}
		}
		void UndoCore() {
			this.modelValueStore = null;
			if(RestoreNode != null) {
				this.nodes = null;
				this.modelValueStore = RestoreNode.modelValueStore;
				this.nodes = RestoreNode.nodes;
				if(this.nodes != null) { 
					foreach(ModelNode node in this.nodes) {
						node.SetParent(this);
					}
				}
				RestoreNode.CreateAndSaveRestorePoint();
			}
			else {
				ResetNodes();
			}
			if(Master != null) {
				Master.Reset();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string Path {
			get {
				return ModelNodePathHelper.GetNodePath(this);
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void CreateRestorePoint() {
			if(HasModification) {
				GetWritableLayer().CreateAndSaveRestorePoint();
			}
		}
		void CreateAndSaveRestorePoint() {
			this.restoreNode = CreateRestorePointCore();
		}
		ModelNode CreateRestorePointCore() {
			ModelNode undoNode = CreateNode();
			undoNode.ApplyDiffCore(this, false, new List<ModelNode>());
			undoNode.SetIsNewNode(true);
			return undoNode;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsNewNode {
			get { return GetState(ModelNodeState.New); }
			private set { SetState(ModelNodeState.New, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void SetIsNewNode(bool value) {
			if(!value) {
				IsNewNode = false;
			}
			else {
				IsNewNode = true;
				if(Nodes != null) {
					foreach(ModelNode node in Nodes) {
						node.SetIsNewNode(value);
					}
				}
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsRemovedNode {
			get { return GetState(ModelNodeState.Removed); }
			private set { SetState(ModelNodeState.Removed, value); }
		}
		bool IsEmptyNode() {
			if(IsNewNode || IsRemovedNode) {
				if(ModelValuesCountCore == 0 || (IsListNode && IsSingleLayer)) {
					return false;
				}
			}
			else if(ModelValuesCountCore > 0) {
				return false;
			}
			EnsureNodes(false);
			if(Nodes != null) {
				foreach(ModelNode node in Nodes) {
					if(!node.IsEmptyNode()) {
						return false;
					}
				}
			}
			return true;
		}
		internal bool IsUnusableNode { get { return Root is ModelApplicationBase.UnusableModelApplication; } }
		bool CanRemoveNode() {
			if(Parent == null) return false;
			if(IsSeparate || (IsInFirstLayer && IsInLastLayer)) return true;
			ModelNode masterNode = Parent;
			if(!masterNode.IsMaster) {
				masterNode = masterNode.Master;
			}
			if(masterNode == null) return IsInLastLayer || IsNodesGeneratorInProgressInLayer;
			int foundedCount = 0;
			ModelNode layer = null;
			for(int layerIndex = 0; layerIndex < masterNode.Layers.Count; layerIndex++) {
				layer = masterNode.Layers[layerIndex];
				if(layer.GetNodeInThisLayer(Id) != null) {
					if(layerIndex == 0 || foundedCount > 0) return false;
					foundedCount++;
					if(layer.IsInLastLayer) return true;
				}
			}
			return false;
		}
		internal void Delete() {
			lock(TypesInfo.lockObject) {
				if(IsMaster && LastLayer != null) {
					LastLayer.Delete();
					return;
				}
				if(CanRemoveNode()) {
					Parent.RemoveNodeFromList(this);
					Parent.OnRemoveNode(this);
				}
				else {
					ModelNode writableLayer = GetOrCreateWritableLayer();
					writableLayer.IsNewNode = false;
					writableLayer.IsRemovedNode = true;
					if(!IsRoot) {
						ModelNode parentMaster = Parent.GetMasterCore();
						if(parentMaster != null) {
							parentMaster.Reset();
						}
					}
				}
			}
		}
		internal void AddLayerInternal(ModelNode layer) {
			InsertLayerAtInternal(layer, Layers != null ? Layers.Count : 0);
		}
		internal virtual void InsertLayerAtInternal(ModelNode layer, int insertIndex) {
			lock(TypesInfo.lockObject) {
				InsertLayerAtCoreInLock(layer, insertIndex);
			}
		}
		void InsertLayerAtCoreInLock(ModelNode layer, int insertIndex) {
			if(layer == null || (Layers != null && Layers.Contains(layer))) return;
			if(Layers == null) {
				this.layers = new List<ModelNode>();
			}
			Reset();
			layer.Master = this;
			if(insertIndex < 0 || insertIndex > Layers.Count - 1) {
				Layers.Add(layer);
			}
			else {
				Layers.Insert(insertIndex, layer);
			}
		}
		internal int RemoveLayerInternal(ModelNode layer) {
			lock(TypesInfo.lockObject) {
				return RemoveLayerInLock(layer);
			}
		}
		int RemoveLayerInLock(ModelNode layer) {
			int layerIndex = Layers.IndexOf(layer);
			Layers.Remove(layer);
			Reset();
			layer.ResetMasterNodes();
			layer.Master = null;
			return layerIndex;
		}
		int GetValueCurrentAspectIndex(string name) {
			lock(TypesInfo.lockObject) {
				return IsValueLocalizable(name) ? GetCurrentAspectIndex() : 0;
			}
		}
		public int GetCurrentAspectIndex() {
			return Root.GetRootCurrentAspectIndex();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsReadOnly { get { return !IsSeparate && !IsInLastLayer; } }
		internal bool IsInFirstLayer {
			get {
				ModelNode master = Root.Master;
				return master != null && master.FirstLayer == Root && (master.Master == null || master.IsInFirstLayer);
			}
		}
		bool IsInLastLayer {
			get {
				ModelNode master = Root.Master;
				return master != null && master.LastLayer == Root && (master.Master == null || master.IsInLastLayer);
			}
		}
		internal bool IsSingleLayer {
			get {
				ModelNode master = Master;
				return master == null || master.Layers.Count == 1;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool HasModification {
			get {
				ModelNode writableLayer = GetWritableLayer();
				return writableLayer != null && !writableLayer.IsEmptyNode();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsNewValueModified {
			get {
				ModelNode writableLayer = GetWritableLayer();
				return writableLayer != null && writableLayer.IsNewNode;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsValueModified(string name) {
			Guard.ArgumentNotNullOrEmpty(name, "name");
			ModelNode lastLayer = GetWritableLayer();
			return lastLayer != null && lastLayer.HasValueInThisLayer(name);
		}
		ModelNode FirstLayer { get { return layers != null ? layers[0] : null; } }
		ModelNode FindFirstLayer() {
			ModelNode master = Master;
			ModelNode firstLayer = master != null ? master : this;
			while(firstLayer.IsMaster) {
				firstLayer = firstLayer.FirstLayer;
			}
			return firstLayer;
		}
		ModelNode GetMasterRecursive() {
			ModelNode node = null;
			ModelNode master = Master;
			while(master != null) {
				node = master;
				master = node.Master;
			}
			return node;
		}
		protected ModelNode LastLayer { get { return layers != null ? layers[layers.Count - 1] : null; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ModelApplicationCreator CreatorInstance { get { return NodeInfo.ApplicationCreator; } }
		List<ModelNode> Nodes { get { return nodes; } }
		internal ModelNode[] GetSortedNodes() { return GetSortedNodes(false); }
		ModelNode[] GetSortedNodes(bool inThisLayer) {
			lock(TypesInfo.lockObject) {
				ModelNode master = Master;
				if(GetNodeOrValueInThisLayer(master, inThisLayer)) {
					if(this.sortedNodes == null) {
						EnsureNodes(false);
						ModelNode[] list = GetFirstLayers(inThisLayer);
						Array.Sort<ModelNode>(list, NodeInfo.ChildNodesComparison);
						this.sortedNodes = list;
					}
					return this.sortedNodes;
				}
				return master.GetSortedNodes(inThisLayer);
			}
		}
		private ModelNode GetFirstLayerRecursive() {
			ModelNode result = this;
			while(result.IsMaster) {
				result = result.Layers[0];
			}
			return result;
		}
		private ModelNode[] GetFirstLayers(bool inThisLayer) {
			if(Nodes != null) {
				if(inThisLayer) {
					return Nodes.ToArray();
				}
				else {
					ModelNode[] result = new ModelNode[Nodes.Count];
					for(int i = 0; i < Nodes.Count; ++i) {
						ModelNode node = Nodes[i];
						result[i] = node.GetFirstLayerRecursive();
					}
					return result;
				}
			}
			return EmptyNodes;
		}
		Dictionary<string, ModelNode> IdNodes { get { return idNodes; } }
		protected void MoveAspects(int[] indices) {
			MoveChildrenAspects(indices);
			MoveValuesAspects(indices);
		}
		void MoveChildrenAspects(int[] indices) {
			if(Nodes == null) return;
			foreach(ModelNode node in Nodes) {
				node.MoveAspects(indices);
			}
		}
		void MoveValuesAspects(int[] indices) {
			foreach(IModelValue value in ModelValuesCore) {
				value.Move(indices);
			}
		}
		internal void EnsureNodes() {
			lock(TypesInfo.lockObject) {
				EnsureNodes(true);
			}
		}
		void EnsureNodes(bool alwaysCreate) {
			if(IsMaster) {
				if(Nodes == null) {
					CreateMasterNodes();
				}
			}
			else {
				if(Nodes == null && alwaysCreate) {
					nodes = new List<ModelNode>();
				}
				if(RunNodesGenerator()) {
					if(Nodes != null) {
						foreach(ModelNode node in Nodes.ToArray()) {	
							node.Update();
						}
					}
				}
			}
		}
		[Browsable(false)]
		public int NodeCount { get { return GetNodeCount(false); } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public int NodeCountInThisLayer { get { return GetNodeCount(true); } }
		public ModelNode GetNode(string id) {
			return GetNode(id, false);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ModelNode GetNodeInThisLayer(int index) {
			return GetNode(index, true);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ModelNode GetNodeInThisLayer(string id) {
			return GetNode(id, true);
		}
		public ModelNode GetNode(int index) {
			return GetNode(index, false);
		}
		ModelNode GetNode(string id, bool inThisLayer) {
			lock(TypesInfo.lockObject) {
				ModelNode masterNode = Master;
				if(GetNodeOrValueInThisLayer(masterNode, inThisLayer)) {
					EnsureNodes(false);
					return GetFirstLayerNodeOnGetNode(GetNodeCore(id), inThisLayer);
				}
				return masterNode.GetNode(id, inThisLayer);
			}
		}
		ModelNode GetNode(int index, bool inThisLayer) {
			lock(TypesInfo.lockObject) {
				ModelNode[] nodes = GetSortedNodes(inThisLayer);
				return GetFirstLayerNodeOnGetNode(nodes != null && index < nodes.Length ? nodes[index] : null, inThisLayer);
			}
		}
		ModelNode GetFirstLayerNodeOnGetNode(ModelNode node, bool isThisLayer) {
			if(isThisLayer || node == null) return node;
			while(node.FirstLayer != null) {
				node = node.FirstLayer;
			}
			return node;
		}
		int GetNodeCount(bool inThisLayer) {
			lock(TypesInfo.lockObject) {
				ModelNode masterNode = Master;
				if(GetNodeOrValueInThisLayer(masterNode, inThisLayer)) {
					EnsureNodes(false);
					return Nodes != null ? Nodes.Count : 0;
				}
				return masterNode.GetNodeCount(inThisLayer);
			}
		}
		ModelNode GetNodeCore(string id) {
			lock(TypesInfo.lockObject) {
				if(id == null) return null;
				if(Nodes == null) return null;
				if(IdNodes == null && Nodes.Count >= MinNodeCountToUseDictionary) {
					CreateIdNodes();
				}
				ModelNode result = null;
				if(IdNodes != null) {
					IdNodes.TryGetValue(id, out result);
				}
				else {
					result = Nodes.Find(node => id == node.Id);
				}
				return result;
			}
		}
		void CreateIdNodes() {
			this.idNodes = new Dictionary<string, ModelNode>();
			if(Nodes != null) {
				foreach(ModelNode node in Nodes) {
					if(!string.IsNullOrEmpty(node.Id)) {
						idNodes[node.Id] = node;
					}
				}
			}
		}
		ModelNode GetLayerForModification() {
			return IsNodesGeneratorInProgressInLayer ? this : GetOrCreateWritableLayer();
		}
		ModelNode GetWritableLayer() {
			if(IsReadOnly) {
				ModelNode master = GetMasterRecursive();
				if(master == null && IsMaster) {
					master = this;
				}
				if(master != null) {
					ModelNode layer = master.LastLayer;
					return layer.IsInLastLayer ? layer : null;
				}
				return null;
			}
			return this;
		}
		internal ModelNode GetOrCreateWritableLayer() {
			lock(TypesInfo.lockObject) {
				ModelNode layer = GetWritableLayer();
				if(layer == null) {
					layer = CreateLastLayer();
				}
				return layer;
			}
		}
		ModelNode CreateLastLayer() {
			Stack<ModelNode> nodes = new Stack<ModelNode>();
			ModelNode node = this;
			while(!node.IsRoot) {
				nodes.Push(node);
				node = node.Parent;
			}
			ModelNode writableParent = Root.GetWritableLayer();
			ModelNode nodeToResetMaster = null; 
			while(nodes.Count > 0) {
				node = nodes.Pop();
				ModelNode writableLayer = writableParent.GetNodeCore(node.Id);
				if(writableLayer == null) {
					if(nodeToResetMaster == null) {
						nodeToResetMaster = writableParent;
					}
					writableLayer = node.NodeInfo.CreateNode(node.Id);
					writableParent.AddNodeIntoList(writableLayer);
					writableLayer.SetParent(writableParent);
				}
				writableParent = writableLayer;
			}
			if(nodeToResetMaster != null) {
				nodeToResetMaster.Master.Reset();
			}
			return writableParent;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void Reset() {
			lock(TypesInfo.lockObject) {
				if(IsMaster) {
					ResetMasterLayer();
					ModelNode master = Master;
					if(master != null) {
						master.ResetMasterLayer();
					}
				}
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ModelNode AddClonedNode(ModelNode source, string id) {
			lock(TypesInfo.lockObject) {
				Guard.ArgumentNotNull(source, "source");
				Guard.ArgumentNotNullOrEmpty(id, "id");
				ModelNode newNode = AddNode(id, source.GetType());
				if(Master != null) {
					Master.EnsureNodes();
				}
				if(source != this && source.GetMasterRecursive() != null) {
					source = source.GetMasterRecursive();
				}
				newNode.ApplyDiffCore(source, false, new List<ModelNode>());
				return newNode;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ModelNode Clone(string id) {
			lock(TypesInfo.lockObject) {
				if(IsRoot) {
					throw new InvalidOperationException("The root node cannot be cloned.");
				}
				return Parent.AddClonedNode(this, id);
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ModelNode ApplyDiff(ModelNode sourceNode) {
			return ApplyDiff(sourceNode, false);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ModelNode Merge(ModelNode sourceNode) {
			return ApplyDiff(sourceNode, true);
		}
		ModelNode ApplyDiff(ModelNode sourceNode, bool isMerge) {
			if(sourceNode == null)
				return null;
			if(sourceNode.NodeInfo.GeneratedClass != NodeInfo.GeneratedClass)
				throw new ArgumentException(string.Format("The 'sourceNode' must be of type '{0}' but it is of type '{1}'.", NodeInfo.GeneratedClass, sourceNode.NodeInfo.GeneratedClass), "sourceNode");
			if(!CheckAspectsOfSourceNode(sourceNode))
				throw new ArgumentException("The 'sourceNode' must have either only default aspect or same set of aspects as this node.", "sourceNode");
			lock(TypesInfo.lockObject) {
				ModelNode upToDateNode = GetUpToDateNode();
				return upToDateNode.ApplyDiff(sourceNode, isMerge, new HashSet<ModelNode>());
			}
		}
		private bool CheckAspectsOfSourceNode(ModelNode sourceNode) {
			IList<string> thisNodeAspects = ((ModelApplicationBase)Root).Aspects;
			IList<string> sourceNodeAspects = sourceNode.Root is ModelApplicationBase ? ((ModelApplicationBase)sourceNode.Root).Aspects : null;
			if(sourceNodeAspects != null && sourceNodeAspects.Count > 0) {
				if(thisNodeAspects.Count == sourceNodeAspects.Count) {
					for(int i = 0; i < thisNodeAspects.Count; ++i) {
						if(thisNodeAspects[i] != sourceNodeAspects[i]) {
							return false;
						}
					}
					return true;
				}
				return false;
			}
			return true;
		}
		private ModelNode GetUpToDateNode() {
			if(this.IsRoot) return this;
			List<string> ids = new List<string>();
			ModelNode node = this;
			while(node.Parent != null) {
				ids.Add(node.Id);
				node = node.parent;
			}
			node = Root;
			node.MasterItem.IsMasterCalculated = false;
			for(int i = ids.Count - 1; i >= 0; i--) {
				node = node[ids[i]];
				node.MasterItem.IsMasterCalculated = false;
				if(node == null) return this;
			}
			return node;
		}
		private ModelNode ApplyDiff(ModelNode sourceNode, bool isMerge, ICollection<ModelNode> addedNodes) {
			ModelNode masterNode = GetMasterRecursive();
			if(masterNode != null) {
				masterNode.ApplyDiffCore(sourceNode, isMerge, addedNodes);
			}
			else {
				ApplyDiffCore(sourceNode, isMerge, addedNodes);
			}
			masterNode = Master; 
			return masterNode != null ? masterNode.LastLayer : LastLayer;
		}
		private void ApplyDiffCore(ModelNode sourceNode, bool isMerge, ICollection<ModelNode> addedNodes) {
			if(sourceNode.IsRemovedNode && !sourceNode.IsNewNode) {
				Delete();
			}
			else {
				if(sourceNode.IsRemovedNode && sourceNode.IsNewNode) {
					ModelNode writableLayer = GetOrCreateWritableLayer();
					writableLayer.ClearModelValuesCore();
					writableLayer.ResetNodes();
					writableLayer.IsRemovedNode = writableLayer.IsRemovedNode || !writableLayer.IsNewNode;
					writableLayer.IsNewNode = true;
					if(Parent != null) {
						Parent.Reset();
					}
				}
				sourceNode.EnsureNodes();
				if(!isMerge && sourceNode.NodesCountCore == 0) {
					UndoNodesInWritableLayer();
				}
				ApplyDiffValues(sourceNode);
				ApplyDiffNodes(sourceNode, isMerge, addedNodes);
			}
		}
		private void UndoNodesInWritableLayer() {
			ModelNode writableLayer = GetWritableLayer();
			if(writableLayer != null && writableLayer.NodesCountCore > 0) {
				foreach(ModelNode node in writableLayer.Nodes.ToArray()) {
					node.Undo();
				}
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void ApplyDiffValues(ModelNode sourceNode) {
			Guard.ArgumentNotNull(sourceNode, "sourceNode");
			lock(TypesInfo.lockObject) {
				sourceNode.EnsureMasterValues();
				EnsureMasterValues();
				ModelNode targetNode = null;
				if(sourceNode.ModelValuesCountCore > 0) {
					ApplyDiffValuesMapAttribute mapAttribute = AttributeHelper.GetAttributes<ApplyDiffValuesMapAttribute>(NodeInfo.BaseInterface, true).FirstOrDefault();
					foreach(string sourceValueName in sourceNode.ModelValueNamesCore) {
						string targetValueName = sourceValueName;
						if(mapAttribute != null && targetValueName == mapAttribute.SourceValueName) {
							targetValueName = mapAttribute.TargetValueName;
						}
						if(IsValueAssignInApplyDiff(targetValueName)) {
							IModelValue sourceModelValue = sourceNode.FindModelValueCore(sourceValueName);
							IModelValue targetModelValue = FindModelValueCore(targetValueName);
							if(ShouldApplyModelValue(targetModelValue, sourceModelValue, targetValueName)) {   
								if(targetNode == null) {
									targetNode = GetLayerForModification();
								}
								IModelValue newValue = (IModelValue)sourceModelValue.Clone();
								if(targetModelValue != null) {
									newValue.CombineWith(targetModelValue);
								}
								targetNode.OnPropertyChanging(targetValueName);
								targetNode.SetModelValueCore(targetValueName, newValue);
								targetNode.OnValueChanged(targetValueName);
							}
						}
					}
				}
			}
		}
		private bool IsValueAssignInApplyDiff(string valueName) {
			ModelValueInfo valueInfo = GetValueInfo(valueName);
			if(valueInfo == null && ModelValuePersistentPathCalculator.IsHelperValueName(valueName)) {
				valueInfo = GetValueInfo(ModelValuePersistentPathCalculator.GetRealValueName(valueName));
			}
			return valueInfo != null && !valueInfo.IsReadOnly;
		}
		private bool ShouldApplyModelValue(IModelValue targetModelValue, IModelValue sourceModelValue, string valueName) {
			bool result = !sourceModelValue.Equals(targetModelValue);
			if(result && targetModelValue == null) {
				object defaultValue = GetDefaultValue(valueName);
				if(defaultValue != null) {
					if(sourceModelValue is IModelLocalizableValue) {
						result = false;
						IModelLocalizableValue localizableValue = (IModelLocalizableValue)sourceModelValue;
						for(int i = 0; i < localizableValue.AspectCount; i++) {
							if(sourceModelValue.HasValue(i) && !defaultValue.Equals(sourceModelValue.GetObjectValue(i))) {
								result = true;
								break;
							}
						}
					}
					else {
						result = !defaultValue.Equals(sourceModelValue.GetObjectValue(0));
					}
				}
			}
			return result;
		}
		private void ApplyDiffNodes(ModelNode sourceNode, bool isMerge, ICollection<ModelNode> addedNodes) {
			if(!isMerge) {
				ApplyDiffNodesRemoveOld(sourceNode);
			}
			ApplyDiffNodesForNewAndExisting(sourceNode, isMerge, addedNodes);
		}
		private void ApplyDiffNodesRemoveOld(ModelNode currentNode) {
			List<ModelNode> children = GetUnsortedChildren();
			List<ModelNode> nodesToRemove = new List<ModelNode>();
			foreach(ModelNode chilNode in children) {
				if(currentNode[chilNode.Id] == null) {
					nodesToRemove.Add(chilNode);
				}
			}
			foreach(ModelNode chilNode in nodesToRemove) {
				chilNode.Delete();
			}
		}
		private void ApplyDiffNodesForNewAndExisting(ModelNode currentNode, bool isMerge, ICollection<ModelNode> addedNodes) {
			if(currentNode.Nodes == null) return;
			foreach(ModelNode currentChildNode in currentNode.Nodes) {
				if(addedNodes.Contains(currentChildNode)) continue;
				ModelNode childNode = this[currentChildNode.Id];
				if(childNode != null && childNode.GetType() != currentChildNode.GetType()) {
					childNode.Delete();
					childNode = null;
				}
				if(childNode == null) {
					childNode = AddNode(currentChildNode.Id, currentChildNode.GetType());
					addedNodes.Add(childNode);
				}
				childNode.ApplyDiff(currentChildNode, isMerge, addedNodes);
			}
		}
		internal void ResetNodes() {
			lock(TypesInfo.lockObject) {
				this.nodesGeneratorStatus = NodesGeneratorStatus.NotStarted;
				this.nodes = null;
				this.sortedNodes = null;
				this.idNodes = null;
			}
		}
		void ResetMasterLayer() {
			ResetNodes();
			ClearModelValuesCore();
			ClearMasterModelNullValues();
			ResetLayersMasterNodes();
		}
		void ResetLayersMasterNodes() {
			if(IsMaster) {
				foreach(ModelNode layer in Layers) {
					layer.ResetMasterNodes();
				}
			}
		}
		void EnsureMaster(ModelMultipleMasterStoreItem masterNodeItem) {
			if(IsRoot) {
				masterNodeItem.IsMasterCalculated = true;
				return;
			}
			ModelNode rootMaster = Root.Master;
			if(rootMaster == null) {
				masterNodeItem.IsMasterCalculated = true;
				return;
			}
			if(IsNodesGeneratorInProgressInLayer || IsCreatingMasterNodesInThisLayer || rootMaster.IsCreatingMasterNodesInThisLayer) return;
			List<ModelNode> nodesToRecreateMasters = new List<ModelNode>();
			ModelNode node = this;
			while(node.GetMasterCore() == null) {
				nodesToRecreateMasters.Add(node);
				node = node.Parent;
			}
			for(int i = nodesToRecreateMasters.Count - 1; i >= 0; i--) {
				ModelNode masterNode = nodesToRecreateMasters[i].Parent.GetMasterCore();
				while(masterNode.GetMasterCore() != null) {
					masterNode = masterNode.GetMasterCore();
				}
				masterNode.EnsureNodes();
				if(nodesToRecreateMasters[i].GetMasterCore() == null) break;
			}
			masterNodeItem.IsMasterCalculated = true;
		}
		protected void ResetMasterNodes() {
			lock(TypesInfo.lockObject) {
				if(Nodes != null) {
					foreach(ModelNode node in Nodes) {
						ModelMultipleMasterStoreItem masterNodeItem = node.MasterItem;
						if(masterNodeItem.IsMasterCalculated) {
							node.ResetMasterNodes();
						}
						node.SetMaster(null);
					}
				}
			}
		}
		ModelNode CreateMaster(ModelNode newFirstLayer, int index) {
			Guard.Assert(0 <= index && index <= Nodes.Count, "'index' must be between 0 and " + Nodes.Count);
			ModelNode master = newFirstLayer.NodeInfo.CreateNode(newFirstLayer.Id);
			AddNodeIntoList(master);
			master.SetParent(this);
			if(index < Nodes.Count - 1) {
				Nodes.RemoveAt(Nodes.Count - 1);
				Nodes.Insert(index, master);
			}
			master.AddLayerInternal(newFirstLayer);
			return master;
		}
		int GetParentAspectIndex(int aspectIndex) {
			return Root.GetRootParentAspectIndex(aspectIndex);
		}
		protected virtual int GetRootCurrentAspectIndex() { return 0; }
		protected virtual int GetRootParentAspectIndex(int aspectIndex) { return 0; }
		internal void AddNode(ModelNode node) {
			lock(TypesInfo.lockObject) {
				Guard.ArgumentNotNull(node, "node");
				EnsureNodes(true);
				AddNodeIntoList(node);
				OnAddNode(node);
			}
		}
		void ResetSortedNodes() {
			this.sortedNodes = null;
		}
		void AddNodeIntoList(ModelNode node) {
			lock(TypesInfo.lockObject) {
				if(Nodes == null) {
					this.nodes = new List<ModelNode>();
				}
				Nodes.Add(node);
				if(idNodes != null && !string.IsNullOrEmpty(node.Id)) {
					idNodes[node.Id] = node;
				}
				ResetSortedNodes();
			}
		}
		bool RemoveNodeFromList(ModelNode node) {
			lock(TypesInfo.lockObject) {
				if(Nodes != null) {
					if(idNodes != null && !string.IsNullOrEmpty(node.Id)) {
						idNodes.Remove(node.Id);
					}
					ResetSortedNodes();
					return Nodes.Remove(node);
				}
				return false;
			}
		}
		void OnAddNode(ModelNode node) {
			if(IsNewNode || (IsSlave && !IsInFirstLayer && !IsNodesGeneratorInProgress)) {
				node.IsNewNode = true;
			}
			ModelNode master = GetMasterCore();
			if(master != null) {
				master.Reset();
			}
			CreatorInstance.OnNodeAdded(node);
		}
		void OnRemoveNode(ModelNode node) {
			ModelNode master = GetMasterCore();
			if(master != null) {
				master.Reset();
			}
			CreatorInstance.OnNodeRemoved(node);
		}
		protected void AssignTo(ModelNode node) {
			lock(TypesInfo.lockObject) {
				AssignNodesTo(node);
				AssignValuesTo(node);
			}
		}
		protected void MergeWith(ModelNode node) {
			lock(TypesInfo.lockObject) {
				MergeNodesWith(node);
				MergeValuesWith(node);
			}
		}
		void AssignNodesTo(ModelNode node) {
			EnsureNodes(false);
			if(Nodes == null) return;
			foreach(ModelNode child in Nodes) {
				ModelNode newChild = node.AddNode(child.Id, child.GetType());
				child.AssignTo(newChild);
			}
		}
		void AssignValuesTo(ModelNode node) {
			foreach(string valueName in ModelValueNamesCore) {
				if(!IsValueReadOnly(valueName)) {
					node.SetModelValueCore(valueName, (IModelValue)FindModelValueCore(valueName).Clone());
				}
			}
		}
		void MergeNodesWith(ModelNode node) {
			if(node.Nodes == null) return;
			EnsureNodes(true);
			foreach(ModelNode child in node.Nodes) {
				ModelNode mergedNode = this[child.Id];
				if(mergedNode == null) {
					child.Reset();
					child.parent = this;
					AddNodeIntoList(child);
				}
				else {
					mergedNode.MergeWith(child);
				}
			}
			node.ResetNodes();
		}
		void MergeValuesWith(ModelNode node) {
			foreach(string valueName in node.ModelValueNamesCore) {
				IModelValue modelValue = FindModelValueCore(valueName);
				if(modelValue == null) {
					SetModelValueCore(valueName, node.FindModelValueCore(valueName));
				}
				else {
					modelValue.MergeWith(node.FindModelValueCore(valueName));
				}
			}
			node.ClearModelValuesCore();
		}
		protected virtual ModelApplicationBase GetOrCreateUnusableModel() { return null; }
		internal ModelNode AddUnusableNode() {
			lock(TypesInfo.lockObject) {
				ModelNode node;
				if(IsMaster) {
					node = LastLayer.AddUnusableNode(LastLayer.parent, id, GetType());
				}
				else {
					node = AddUnusableNode(parent, id, GetType());
				}
				return node;
			}
		}
		ModelNode AddUnusableNodeRecursive() {
			ModelNode newUnusedNode = AddUnusableNode(Parent, Id, GetType());
			AssignValuesTo(newUnusedNode);
			newUnusedNode.IsRemovedNode = IsRemovedNode; 
			if(Nodes != null) {
				foreach(ModelNode node in Nodes) {
					node.AddUnusableNodeRecursive();
				}
			}
			return newUnusedNode;
		}
		ModelNode AddUnusableNode(ModelNode parent, string id, Type type) {
			return parent.CreateUnusedNode(id, type);
		}
		ModelNode AddUnusableNode(ModelNode unusedParent) {
			return AddUnusableNode(unusedParent, NodeInfo, Id);
		}
		ModelNode AddUnusableNode(ModelNode unusedParent, ModelNodeInfo nodeInfo, string nodeId) {
			ModelNode newUnusedNode = unusedParent.GetNodeCore(nodeId);
			if(newUnusedNode != null) return newUnusedNode;
			newUnusedNode = new ModelNode(nodeInfo, nodeId);
			unusedParent.EnsureNodes(true);
			unusedParent.AddNodeIntoList(newUnusedNode);
			newUnusedNode.SetParent(unusedParent);
			return newUnusedNode;
		}
		ModelNode CreateUnusedNode(string id, Type type) {
			ModelApplicationBase unusedModel = (ModelApplicationBase)Root;
			if(!HasValue("UnusableNodeName")) {
				unusedModel = Root.GetOrCreateUnusableModel();
			}
			List<ModelNode> parentList = new List<ModelNode>();
			ModelNode parent = this;
			while(parent != null && !parent.IsRoot) {
				parentList.Add(parent);
				parent = parent.Parent;
			}
			parent = unusedModel;
			for(int i = parentList.Count - 1; i >= 0; i--) {
				string nodeId = parentList[i].Id;
				if(parent[nodeId] == null) {
					parentList[i].AddUnusableNode(parent);
				}
				parent = parent[nodeId];
			}
			ModelNodeInfo nodeInfo = CreatorInstance.GetNodeInfo(type);
			if(nodeInfo == null) {
				nodeInfo = CreatorInstance.EmptyModelNodeInfo;
			}
			return AddUnusableNode(parent, nodeInfo, id);
		}
		protected void UpdateUnusableNodes() {
			lock(TypesInfo.lockObject) {
				if(Master != null) {
					Master.EnsureNodes();
					Master.ValidateMasterNodesCore(true);
				}
				if(Nodes != null) {
					foreach(ModelNode node in Nodes) {
						node.UpdateUnusableNodes();
					}
				}
			}
		}
		void CreateMasterNodes() {
			StartCreateMasterNodes();
			CreateMasterNodesCore();
			EndCreateMasterNodes();
		}
		protected virtual bool IsCreatingMasterNodesInThisLayer {
			get { return !IsRoot ? Root.IsCreatingMasterNodesInThisLayer : false; }
		}
		protected virtual void StartCreateMasterNodes() {
			if(!IsRoot) {
				Root.StartCreateMasterNodes();
			}
		}
		protected virtual void EndCreateMasterNodes() {
			if(!IsRoot) {
				Root.EndCreateMasterNodes();
			}
		}
		void CreateMasterNodesCore() {
			foreach(ModelNode nodeFromLayer in Layers) {
				nodeFromLayer.EnsureNodes(false);
			}
			if(Nodes == null) {
				this.nodes = new List<ModelNode>();
			}
			PopulateMasterNodesForChildNodes();
			if(Master == null || !Master.IsCreatingMasterNodesInThisLayer) {
				ValidateMasterNodes();
			}
		}
		void PopulateMasterNodesForChildNodes() {
			IEnumerable<KeyValuePair<bool, IList<ModelNode>>> layersForChildNodes = CollectLayersForChildNodes();
			foreach(KeyValuePair<bool, IList<ModelNode>> layersForChildNodeItem in layersForChildNodes) {
				bool hasFirstLayer = layersForChildNodeItem.Key;
				IList<ModelNode> nodeLayers = layersForChildNodeItem.Value;
				UpdateNodeLayers(nodeLayers, hasFirstLayer);
				ModelNode masterNode = CreateMasterNode(nodeLayers, hasFirstLayer);
				if(masterNode != null) {
					AddNodeIntoList(masterNode);
					if(IdNodes == null && Nodes.Count >= MinNodeCountToUseDictionary) {
						CreateIdNodes();
					}
				}
			}
		}
		void UpdateNodeLayers(IList<ModelNode> nodeLayers, bool hasFirstLayer) {
			if(hasFirstLayer) {
				ModelNode firstLayer = nodeLayers[0];
				if(!firstLayer.IsMaster && Root.FirstLayer == firstLayer.Root) {
					firstLayer.NodesGeneratorInProgressCounter++;
					try {
						firstLayer.NodeInfo.UpdateNodes(nodeLayers);
					}
					finally {
						firstLayer.NodesGeneratorInProgressCounter--;
					}
				}
			}
		}
		IEnumerable<KeyValuePair<bool, IList<ModelNode>>> CollectLayersForChildNodes() {
			Dictionary<string, KeyValuePair<bool, IList<ModelNode>>> map = new Dictionary<string, KeyValuePair<bool, IList<ModelNode>>>();
			foreach(ModelNode nodeFromLayer in Layers) {
				if(nodeFromLayer.Nodes != null) {
					foreach(ModelNode childNodeFromLayer in nodeFromLayer.Nodes) {
						KeyValuePair<bool, IList<ModelNode>> item;
						if(!map.TryGetValue(childNodeFromLayer.Id, out item)) {
							bool isInFirstLayer = Root.Layers[0] == nodeFromLayer.Root; 
							item = new KeyValuePair<bool, IList<ModelNode>>(isInFirstLayer, new List<ModelNode>());
							map.Add(childNodeFromLayer.Id, item);
						}
						item.Value.Add(childNodeFromLayer);
					}
				}
			}
			return map.Values;
		}
		ModelNode CreateMasterNode(IList<ModelNode> nodeLayers, bool hasFirstLayer) {
			int rootLayerIndex = -1; 
			if(hasFirstLayer) {
				rootLayerIndex = 0;
			}
			for(int i = 0; i < nodeLayers.Count; ++i) {
				ModelNode nodeFromLayer = nodeLayers[i];
				if(rootLayerIndex < 0 && !nodeFromLayer.IsNewNode) {
					nodeFromLayer.AddUnusableNodeAndRemoveFromList();
				}
				if(nodeFromLayer.IsRemovedNode) {
					rootLayerIndex = -1;
				}
				if(nodeFromLayer.IsNewNode) {
					if(rootLayerIndex < 0) {
						rootLayerIndex = i;
					}
				}
			}
			if(rootLayerIndex < 0) {
				return null;
			}
			if(rootLayerIndex + 1 == nodeLayers.Count) { 
				return nodeLayers[rootLayerIndex];
			}
			ModelNode masterNode = nodeLayers[rootLayerIndex].NodeInfo.CreateNode(nodeLayers[rootLayerIndex].Id);
			masterNode.SetParent(this);
			for(int i = rootLayerIndex; i < nodeLayers.Count; ++i) {
				ModelNode nodeFromLayer = nodeLayers[i];
				if(Root.Layers.Contains(nodeFromLayer.Root)) {
					masterNode.AddLayerInternal(nodeFromLayer);
				}
				else { 
					ModelNode nestedMasterParent = null;
					int index = -1;
					foreach(ModelNode parentNodeFromLayer in Layers) {
						parentNodeFromLayer.EnsureNodes();
						index = parentNodeFromLayer.Nodes.IndexOf(nodeFromLayer);
						if(index >= 0) {
							nestedMasterParent = parentNodeFromLayer;
							break;
						}
					}
					Guard.Assert(index >= 0, "'layer' must be contained by 'nestedMasterParent'.");
					nestedMasterParent.RemoveNodeFromList(nodeFromLayer);
					ModelNode nestedMaster = nestedMasterParent.CreateMaster(nodeFromLayer, index);
					nestedMaster.AddLayerInternal(nodeFromLayer);
					masterNode.AddLayerInternal(nestedMaster);
				}
			}
			return masterNode;
		}
		bool NeedValidation {
			get {
				if(IsMaster && !GetState(ModelNodeState.Loaded)) {
					foreach(ModelNode layer in Layers) {
						if(layer.GetState(ModelNodeState.Loaded)) {
							return true;
						}
					}
				}
				return GetState(ModelNodeState.Loaded);
			}
		}
		bool IsValid(IModelApplication application) {
			if(!NodeInfo.IsValid(this, application)) {
				return false;
			}
			ModelNode target = IsMaster ? LastLayer : this;
			target.CheckPersistentValues();
			return true;
		}
		void Update() {
			if(!GetState(ModelNodeState.Updated) && !IsNodesGeneratorInProgressInLayer && IsSlave && !IsMaster && !IsInFirstLayer && !IsUnusableNode) {
				try {
					NodesGeneratorInProgressCounter++;
					NodeInfo.Update(this, Application);
				}
				finally {
					NodesGeneratorInProgressCounter--;
					SetState(ModelNodeState.Updated, true);
				}
			}
		}
		bool RunNodesGenerator() {
			if(IsNodeGeneratorNotStarted && IsSlave && !IsMaster && !IsUnusableNode) {
				ModelApplicationBase model = Root as ModelApplicationBase;
				string currentAspect = string.Empty;
				if(model != null) {
					currentAspect = model.CurrentAspect;
					model.SetCurrentAspect(string.Empty);
				}
				if(Root.IsInFirstLayer) {
					RunNodesGeneratorForFirstLayer();
				}
				else {
					RunNodesGeneratorForLastLayer();
				}
				if(model != null) {
					model.SetCurrentAspect(currentAspect);
				}
				return true;
			}
			return false;
		}
		void RunNodesGeneratorForFirstLayer() {
			RunNodesGenerator(NodesGenerator);
		}
		void RunNodesGeneratorForLastLayer() {
			if(IsRemovedNode && !IsNewNode) return;
			ModelNode masterNode = GetMasterCore();
			if(masterNode == null || masterNode.FirstLayer == this) {
				RunNodesGenerator(ModelNodesDefaultInterfaceGenerator.Instance);
			}
		}
		void RunNodesGenerator(ModelNodesGeneratorBase generator) {
			if(generator == null) return;
			lock(TypesInfo.lockObject) {
				try {
					IsNodesGeneratorInProgress = true;
					if(Nodes == null) {
						this.nodes = new List<ModelNode>();
					}
					generator.GenerateNodes(this);
				}
				finally {
					IsNodesGeneratorInProgress = false;
				}
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool HasChildrenBeenGenerated {
			get {
				lock(TypesInfo.lockObject) {
					ModelNode firstLayer = FindFirstLayer();
					return firstLayer != null ? firstLayer.ChildrenHasBeenGeneratedInThisLayer : false;
				}
			}
		}
		bool ChildrenHasBeenGeneratedInThisLayer {
			get { return IsInFirstLayer && Nodes != null && NodeCount > 0 && nodesGeneratorStatus == NodesGeneratorStatus.Done; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void RunCustomGenerator(ModelNodesGeneratorBase customGenerator) {
			lock(TypesInfo.lockObject) {
				ModelNode node = GetLayerForCustomGenerator();
				node.RunNodesGenerator(customGenerator);
				ModelNode masterNode = node.Master;
				if(masterNode != null) {
					masterNode.RemoveIsNewNodeValueFromLayers();
					masterNode.Reset();
				}
			}
		}
		ModelNode GetLayerForCustomGenerator() {
			if(IsMaster) {
				return FirstLayer.GetLayerForCustomGenerator();
			}
			if(IsInFirstLayer) {
				return this;
			}
			if(Master != null) {
				ModelNode firstLayer = Master.FirstLayer;
				while(firstLayer.FirstLayer != null) {
					firstLayer = firstLayer.FirstLayer;
				}
				if(firstLayer != null && firstLayer.IsInFirstLayer && !firstLayer.IsMaster) {
					return firstLayer;
				}
			}
			ModelNode parentFirstLayer = Parent.GetLayerForCustomGenerator();
			ModelNode result = parentFirstLayer.GetNodeInThisLayer(Id);
			if(result != null) {
				return result;
			}
			result = NodeInfo.CreateNode(Id);
			parentFirstLayer.AddNodeIntoList(result);
			result.SetParent(parentFirstLayer);
			ModelNode master = parentFirstLayer.GetMasterCore();
			if(master != null) {
				master.Reset();
			}
			return result;
		}
		void RemoveIsNewNodeValueFromLayers() {
			if(Layers == null) return;
			RemoveIsNewNodeValue();
			foreach(ModelNode layer in Layers) {
				layer.RemoveIsNewNodeValue();
			}
			EnsureNodes();
			if(Nodes != null) {
				foreach(ModelNode node in Nodes) {
					node.RemoveIsNewNodeValueFromLayers();
				}
			}
		}
		void RemoveIsNewNodeValue() {
			IsNewNode = false;
		}
		void AddUnusableNodeAndRemoveFromList() {
			AddUnusableNodeRecursive();
			Parent.RemoveNodeFromList(this);
		}
		void ValidateMasterNodes() {
			ValidateMasterNodesCore(false);
		}
		void ValidateMasterNodesCore(bool validateNotLoaded) {
			if(IsUnusableNode) return;
			for(int i = Nodes.Count - 1; i >= 0; i--) {
				ModelNode node = Nodes[i];
				bool needValidate = validateNotLoaded ? !NeedValidation : node.NeedValidation;
				if(needValidate) {
					if(node.Root == Root) {
						if(!node.IsValid(Application)) {
							node.LastLayer.AddUnusableNodeAndRemoveFromList();
							node.Parent.RemoveNodeFromList(node);
						}
					}
					else if(node.Root != Root.FirstLayer) { 
						node.ValidateUniqueLastLayerNode();
					}
				}
			}
		}
		void ValidateUniqueLastLayerNode() {
			if(!IsValid(Application)) {
				ModelNode masterNode = Parent.Master;
				if(masterNode != null) {
					masterNode.RemoveNodeFromList(this);
				}
				AddUnusableNodeAndRemoveFromList();
			}
			else {
				if(Nodes == null) return;
				for(int i = Nodes.Count - 1; i >= 0; i--) {
					Nodes[i].ValidateUniqueLastLayerNode();
				}
			}
		}
		public void ClearValue(string name) {
			lock(TypesInfo.lockObject) {
				Guard.ArgumentNotNullOrEmpty(name, "name");
				ModelNode node = IsNodesGeneratorInProgressInLayer ? this : GetWritableLayer();
				if(node != null) {
					node.ClearValueInThisLayer(name);
				}
			}
		}
		private void ClearValueInThisLayer(string name) {
			if(ContainsModelValueCore(name)) {
				OnPropertyChanging(name);
				RemoveModelValueCore(name);
				OnValueChanged(name);
			}
		}
		private bool HasValue(string name, int aspectIndex, bool inThisLayer) {
			lock(TypesInfo.lockObject) {
				if(IsKeyPropertyName(name)) {
					return !string.IsNullOrEmpty(GetKeyPropertyValue(name));
				}
				IModelValue modelValue = GetModelValue(name, inThisLayer);
				return modelValue != null && modelValue.HasValue(aspectIndex);
			}
		}
		internal bool HasValueInThisLayer(string name) {
			Guard.ArgumentNotNullOrEmpty(name, "name");
			return HasValue(name, GetValueCurrentAspectIndex(name), true);
		}
		public bool HasValue(string name) {
			Guard.ArgumentNotNullOrEmpty(name, "name");
			return HasValue(name, GetValueCurrentAspectIndex(name), false);
		}
		public object GetValue(string name) {
			Guard.ArgumentNotNullOrEmpty(name, "name");
			return GetValue(name, false, GetValueCurrentAspectIndex(name));
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		internal bool HasValueInCurrentAspect(string name) {
			Guard.ArgumentNotNullOrEmpty(name, "name");
			lock(TypesInfo.lockObject) {
				Guard.Assert(IsValueLocalizable(name), "IsValueLocalizable");
				IModelValue modelValue = GetModelValue(name, false);
				if(modelValue != null) {
					return modelValue.HasValue(GetValueCurrentAspectIndex(name));
				}
				return false;
			}
		}
		public T GetValue<T>(string name) {
			Guard.ArgumentNotNullOrEmpty(name, "name");
			return GetValue<T>(name, false, GetValueCurrentAspectIndex(name));
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public T GetValueInThisLayer<T>(string name) {
			Guard.ArgumentNotNullOrEmpty(name, "name");
			return GetValue<T>(name, true, GetValueCurrentAspectIndex(name));
		}
		public void SetValue(string name, object value) {
			Guard.ArgumentNotNullOrEmpty(name, "name");
			lock(TypesInfo.lockObject) {
				ModelNodeSetValueHelper.SetValue(this, name, value, GetValueType(name));
			}
		}
		private bool IsStateValueName(string name) {
			return name == ModelValueNames.IsNewNode || name == ModelValueNames.IsRemovedNode;
		}
		private ModelNodeState GetStateByName(string name) {
			switch(name) {
				case ModelValueNames.IsNewNode:
					return ModelNodeState.New;
				case ModelValueNames.IsRemovedNode:
					return ModelNodeState.Removed;
			}
			return ModelNodeState.None;
		}
		private void SetStateInSetValue(string name, bool value) {
			if(IsSeparate) {
				SetState(GetStateByName(name), value);
			}
			else {
				throw new ArgumentException("Cannot change state of not separated node.");
			}
		}
		public void SetValue<ValueType>(string name, ValueType value) {
			SetValueCore<ValueType>(name, value, true);
		}
		internal void SetValueCore<ValueType>(string name, ValueType value, bool checkCurrentValue) {
			lock(TypesInfo.lockObject) {
				Guard.ArgumentNotNullOrEmpty(name, "name");
				if(IsStateValueName(name)) {
					SetStateInSetValue(name, (bool)(object)value);
				}
				else {
					int aspectIndex = GetValueCurrentAspectIndex(name);
					if(!checkCurrentValue || CanOverrideValueOnSet(name, aspectIndex, value)) {
						ModelNode node = GetLayerForModification();
						node.SetValueInThisLayer<ValueType>(name, aspectIndex, value);
					}
				}
			}
		}
		void SetValueInThisLayer(string name, object value) {
			ModelNodeSetValueHelper.SetValueInThisLayer(this, name, value, GetValueType(name));
		}
		internal void SetValueInThisLayer<ValueType>(string name, ValueType value) {
			lock(TypesInfo.lockObject) {
				Guard.ArgumentNotNullOrEmpty(name, "name");
				if(IsStateValueName(name)) {
					SetStateInSetValue(name, (bool)(object)value);
				}
				else {
					SetValueInThisLayer<ValueType>(name, GetValueCurrentAspectIndex(name), value);
				}
			}
		}
		bool CanOverrideValueOnSet<ValueType>(string name, int aspectIndex, ValueType value) {
			if(Root.IsSeparate) return true;
			if(name == ModelValueNames.IsNewNode || name == ModelValueNames.IsRemovedNode) return true;
			if(IsNodesGeneratorInProgressInLayer || IsValueReadOnly(name)) return true;
			return !object.Equals(value, GetValue<ValueType>(name, false, aspectIndex));
		}
		void SetValueInThisLayer<ValueType>(string name, int aspectIndex, ValueType value) {
			if(IsKeyPropertyName(name)) {
				Id = (string)(object)value;
			}
			else {
				OnPropertyChanging(name);
				GetModelValueOnSetCore<ValueType>(name).SetValue(aspectIndex, value);
				OnValueChanged(name);
			}
		}
		object GetValue(string name, bool inThisLayer, int aspectIndex) {
			lock(TypesInfo.lockObject) {
				if(IsKeyPropertyName(name)) return GetKeyPropertyValue(name);
				IModelValue modelValue = GetModelValue(name, inThisLayer);
				return GetValue(modelValue, name, aspectIndex);
			}
		}
		T GetValue<T>(string name, bool inThisLayer, int aspectIndex) {
			lock(TypesInfo.lockObject) {
				if(IsKeyPropertyName(name)) return (T)(object)GetKeyPropertyValue(name);
				IModelValue<T> modelValue = (IModelValue<T>)(inThisLayer ? GetModelValueInThisLayer(name) : GetModelValue(name));
				return GetValue(modelValue, name, aspectIndex);
			}
		}
		bool IsKeyPropertyName(string name) {
			return NodeInfo.IsKeyPropertyName(name);
		}
		string GetKeyPropertyValue(string name) {
			return name == ModelValueNames.Id || !IsAutoGeneratedId ? Id : string.Empty;
		}
		const char GUIDFirstChar = '@';
		static int GUIDLength = Guid.NewGuid().ToString().Length + 1;
		string GenerateAutoId() { return GUIDFirstChar + Guid.NewGuid().ToString(); }
		bool IsAutoGeneratedId { get { return !string.IsNullOrEmpty(Id) && Id.Length == GUIDLength && Id[0] == '@'; } }
		object GetValue(IModelValue modelValue, string name, int aspectIndex) {
			int actualAspectIndex = GetModelValueActualAspect(modelValue, aspectIndex);
			if(modelValue == null || !modelValue.HasValue(actualAspectIndex)) {
				object defaultValue = GetDefaultValue(name);
				return defaultValue != null ? defaultValue : GetNullValue(name);
			}
			return modelValue.GetObjectValue(actualAspectIndex);
		}
		ValueType GetValue<ValueType>(IModelValue<ValueType> modelValue, string name, int aspectIndex) {
			int actualAspectIndex = GetModelValueActualAspect(modelValue, aspectIndex);
			if(modelValue == null || !modelValue.HasValue(actualAspectIndex)) {
				object defaultValue = GetDefaultValue(name);
				return defaultValue != null ? (ValueType)defaultValue : default(ValueType);
			}
			return modelValue.GetValue(actualAspectIndex);
		}
		int GetModelValueActualAspect(IModelValue modelValue, int aspectIndex) {
			if(modelValue != null && modelValue.IsLocalizable) {
				int actualAspectIndex = aspectIndex;
				while(actualAspectIndex > 0 && !modelValue.HasValue(actualAspectIndex)) {
					actualAspectIndex = GetParentAspectIndex(actualAspectIndex);
				}
				return actualAspectIndex;
			}
			return 0;
		}
		void EnsureMasterValues() {
			if(IsMaster) {
				foreach(string valueName in GetUsedValueNames()) {
					GetModelValueInThisLayer(valueName);
				}
			}
		}
		IEnumerable<string> GetUsedValueNames() {
			if(!IsMaster) {
				return ModelValueNamesCore;
			}
			HashSet<string> result = new HashSet<string>();
			foreach(ModelNode layer in Layers) {
				foreach(string valueName in layer.GetUsedValueNames()) {
					result.Add(valueName);
				}
			}
			return result;
		}
		void OnValueChanged(string name) {
			CreatorInstance.OnNodePropertyChanged(this, name);
			MasterOnValueChanged(name);
			if(Parent != null && (name == ModelValueNames.Index)) {
				ResetSortedNodesAllLayers(Parent);
			}
			OnPropertyChanged(name);
		}
		void ResetSortedNodesAllLayers(ModelNode node) {
			node.ResetSortedNodes();
			ModelNode master = node.Master;
			if(!GetNodeOrValueInThisLayer(master, false)) {
				ResetSortedNodesAllLayers(master);
			}
		}
		void MasterOnValueChanged(string name) {
			ModelNode masterNode = GetMasterCore();
			if(masterNode != null) {
				masterNode.RemoveModelValueCore(name);
				masterNode.RemoveMasterModelNullValue(name);
			}
		}
		void UpdateParentIdNodes(string oldId, string newId) {
			if(Parent != null) {
				Parent.UpdateIdNodes(oldId, newId, this);
			}
		}
		void UpdateIdNodes(string oldId, string newId, ModelNode child) {
			if(oldId == newId) return;
			ResetSortedNodes();
			if(IdNodes != null) {
				if(oldId != null) {
					IdNodes.Remove(oldId);
				}
				IdNodes[newId] = child;
			}
			ModelNode masterNode = Master;
			if(masterNode != null) {
				child = oldId != null ? masterNode.GetNodeCore(oldId) : child;
				masterNode.UpdateIdNodes(oldId, newId, child);
			}
		}
		IModelValue GetModelValue(string name) {
			ModelNode source = this;
			if(!IsNodesGeneratorInProgressInLayer) {
				ModelNode master = GetMasterRecursive();
				if(master != null) {
					source = master;
				}
			}
			return source.GetModelValueInThisLayer(name);
		}
		IModelValue GetModelValueInThisLayer(string name) {
			IModelValue result = FindModelValueCore(name);
			if(result == null && IsMaster && !IsMasterNullModelValue(name)) {
				ModelValuePersistentPathCalculator pathCalculator = NodeInfo.GetPathCalculator(name);
				for(int i = Layers.Count - 1; i >= 0; --i) {
					ModelNode layer = Layers[i];
					IModelValue layerModelValue = layer.GetModelValueInThisLayer(name);
					if(layerModelValue == null && pathCalculator != null) {
						layerModelValue = layer.CreateModelValueByPathCalculator(pathCalculator, name, 0);
					}
					if(layerModelValue != null) {
						if(!layerModelValue.IsLocalizable) {
							result = layerModelValue;
							break;
						}
						else if(result == null) {
							result = (IModelValue)layerModelValue.Clone();
						}
						else {
							result.CombineWith(layerModelValue);
						}
					}
				}
				if(result == null) {
					SetMasterNullModelValue(name);
				}
				else {
					SetModelValueCore(name, result);
				}
			}
			return result;
		}
		IModelValue GetModelValue(string name, bool inThisLayer) {
			return inThisLayer ? GetModelValueInThisLayer(name) : GetModelValue(name);
		}
		IModelValue CreateModelValueByPathCalculator(ModelValuePersistentPathCalculator pathCalculator, string name, int aspectIndex) {
			if(HasValueInThisLayer(ModelValuePersistentPathCalculator.GetHelperValueName(name))) {
				object obj = pathCalculator.Calculate(this, name, false, true);
				IModelValue value = CreateModelValue(name, NodeInfo.GetValueInfo(name).PropertyType);
				value.SetObjectValue(aspectIndex, obj);
				SetModelValueCore(name, value);
				return value;
			}
			return null;
		}
		IModelValue<T> GetModelValueOnSetCore<T>(string name) {
			IModelValue<T> modelValue = GetModelValueInThisLayer(name) as IModelValue<T>;
			if(modelValue == null) {
				modelValue = (IModelValue<T>)CreateModelValue(name, typeof(T));
				SetModelValueCore(name, modelValue);
			}
			MasterOnValueChanged(name);
			return modelValue;
		}
		private IModelValue CreateModelValue(string name, Type objectType) {
			return CreatorInstance.modelValueFactory.CreateModelValue(objectType, IsValueLocalizable(name));
		}
		public ModelValueInfo GetValueInfo(string name) {
			Guard.ArgumentNotNullOrEmpty(name, "name");
			lock(TypesInfo.lockObject) {
				return NodeInfo.GetValueInfo(name);
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Type GetValueType(string name) {
			lock(TypesInfo.lockObject) {
				ModelValueInfo valueInfo = GetValueInfo(name);
				return valueInfo != null ? valueInfo.PropertyType : typeof(string);
			}
		}
		bool IsValueReadOnly(string name) {
			ModelValueInfo valueInfo = GetValueInfo(name);
			return valueInfo != null ? valueInfo.IsReadOnly : false;
		}
		bool IsValueLocalizable(string name) {
			ModelValueInfo valueInfo = GetValueInfo(name);
			return valueInfo != null ? valueInfo.IsLocalizable : false;
		}
		string GetValuePath(string valueName) {
			ModelValueInfo valueInfo = GetValueInfo(valueName);
			return valueInfo != null && valueInfo.CanPersistentPathBeUsed ? valueInfo.PersistentPath : string.Empty;
		}
		TypeConverter GetSerializationTypeConverter(string valueName) {
			ModelValueInfo valueInfo = GetValueInfo(valueName);
			return valueInfo != null ? valueInfo.TypeConverter : null;
		}
		internal bool IsListNode {
			get {
				lock(TypesInfo.lockObject) {
					if(!IsRoot) {
						return Parent.NodeInfo.IsRequireWriteId(GetType());
					}
					return false;
				}
			}
		}
		private ModelNode FindLayerByRoot(ModelNode targetRoot) {
			if(IsMaster) {
				foreach(ModelNode layer in Layers) {
					if(layer.Root == targetRoot) {
						return layer;
					}
					ModelNode subLayer = layer.FindLayerByRoot(targetRoot);
					if(subLayer != null) {
						return subLayer;
					}
				}
			}
			return null;
		}
		internal IDictionary<string, string> GetSerializedValues(int aspectIndex) {
			lock(TypesInfo.lockObject) {
				IDictionary<string, string> result = new Dictionary<string, string>();
				if(IsListNode) {
					result[KeyValueName] = Id;
				}
				EnsureMasterValues();
				foreach(string valueName in ModelValueNamesCore) {
					if(valueName == KeyValueName) {
						result[valueName] = Id;
						continue;
					}
					IModelValue modelValue = FindModelValueCore(valueName);
					if(IsValueReadOnly(valueName) || (!IsValueLocalizable(valueName) && aspectIndex > 0) || !modelValue.HasValue(aspectIndex)) continue;
					string name = null;
					string value = null;
					if(ModelValuePersistentPathCalculator.IsHelperValueName(valueName)) {
						string realName = ModelValuePersistentPathCalculator.GetRealValueName(valueName);
						if(!ContainsModelValueCore(realName)) {
							name = realName;
							value = ((IModelValue<string>)modelValue).GetValue(aspectIndex);
						}
					}
					else {
						name = valueName;
						object obj = modelValue.GetObjectValue(aspectIndex);
						if(obj == null) {
							value = string.Empty;
						}
						else {
							TypeConverter customConverter = GetSerializationTypeConverter(name);
							if(customConverter == null && !string.IsNullOrEmpty(GetValuePath(name))) {
								if(obj is ModelNode) {
									value = ((ModelNode)obj).Id;
								}
							}
							else {
								value = ModelValueConverterHelper.ConvertToString(obj, customConverter);
							}
						}
					}
					if(value != null) {
						string xmlName = NodeInfo.GetXmlNameByValueName(name);
						result[xmlName] = value;
					}
				}
				if(aspectIndex == 0) {
					if(IsNewNode) {
						result[ModelValueNames.IsNewNode] = bool.TrueString;
					}
					if(IsRemovedNode) {
						result[ModelValueNames.IsRemovedNode] = bool.TrueString;
					}
				}
				return result;
			}
		}
		internal void SetSerializedValue(string name, string value) {
			lock(TypesInfo.lockObject) {
				Guard.ArgumentNotNullOrEmpty(name, "name");
				Guard.ArgumentNotNull(value, "value");
				if(IsStateValueName(name)) {
					SetState(GetStateByName(name), bool.Parse(value));
					return;
				}
				name = NodeInfo.GetValueNameByXmlName(name);
				Type type = GetValueType(name);
				if(string.IsNullOrEmpty(value)) {
					if(type == typeof(string)) {
						SetValueInThisLayer<string>(name, "");
					}
					else if(!type.IsValueType) {
						if(!string.IsNullOrEmpty(GetValuePath(name))) {
							SetValueInThisLayer(ModelValuePersistentPathCalculator.GetHelperValueName(name), string.Empty);
						}
						else {
							SetValueInThisLayer(name, null);
						}
					}
				}
				else {
					TypeConverter customConverter = GetSerializationTypeConverter(name);
					if(customConverter != null) {
						object result = customConverter.ConvertFromInvariantString(value);
						if(result != null) {
							SetValueInThisLayer(name, result);
						}
					}
					else {
						if(!string.IsNullOrEmpty(GetValuePath(name))) {
							SetValueInThisLayer<string>(ModelValuePersistentPathCalculator.GetHelperValueName(name), value);
						}
						else {
							object result = ModelValueConverterHelper.ConvertFromString(value, type, customConverter);
							if(result != null) {
								SetValueInThisLayer(name, result);
							}
						}
					}
				}
			}
		}
		internal void SetSerializableValues(IDictionary<string, object> values) {
			lock(TypesInfo.lockObject) {
				if(values == null || values.Count == 0) return;
				foreach(string name in values.Keys) {
					string realName = NodeInfo.GetValueNameByXmlName(name);
					if(IsValueReadOnly(realName)) continue;
					if(!string.IsNullOrEmpty(GetValuePath(realName))) {
						SetValueInThisLayer(ModelValuePersistentPathCalculator.GetHelperValueName(realName), values[name]);
					}
					else {
						object value = values[name];
						if(value != null && GetValueInfo(name) != null) {
							SetValueInThisLayer(realName, value);
						}
					}
				}
			}
		}
		internal ModelNode AddNodeFromXml(string name, IDictionary<string, string> values) {
			lock(TypesInfo.lockObject) {
				Type type = NodeInfo.GetTypeByChildNodeXmlName(name);
				bool gettingTypeFromChildNodeNode = type != null;
				if(type == null) {
					type = CreatorInstance.GetModelType("Model" + name);
				}
				ConvertXmlParameters xmlParams = CreatorInstance.FillAddNodeFromXml(this, type, name, values);
				type = xmlParams.NodeType;
				ModelNode subNode = xmlParams.SubNode == null ? this : (ModelNode)xmlParams.SubNode;
				ModelNode newNode = AddNodeFromXml(subNode, type, name, values, gettingTypeFromChildNodeNode);
				foreach(KeyValuePair<string, string> pair in values) {
					newNode.SetSerializedValue(pair.Key, pair.Value);
				}
				newNode.SetState(ModelNodeState.Loaded, true);
				if(!newNode.IsUnusableNode) {
					newNode.ChechUndefinedValues();
				}
				return newNode;
			}
		}
		ModelNode AddNodeFromXml(ModelNode subNode, Type type, string name, IDictionary<string, string> values, bool gettingTypeFromChildNodeNode) {
			string id = string.Empty;
			bool isUnusableNode = type == null;
			if(!isUnusableNode) {
				string key = CreatorInstance.GetNodeInfo(type).KeyName;
				if(values.TryGetValue(key, out id)) {
					values.Remove(key);
				}
				else {
					id = GetValueCaseInsensitive(values, key);
					if(string.IsNullOrEmpty(id)) {
						id = CreatorInstance.GetIdByType(this, type);
					}
					if(string.IsNullOrEmpty(id) && NodeInfo.GetTypeByChildNodeName(name) == type) {
						id = name;
					}
				}
			}
			else {
				type = typeof(ModelNode);
				id = GetValueCaseInsensitive(values, ModelValueNames.Id);
			}
			if(!gettingTypeFromChildNodeNode && string.IsNullOrEmpty(id)) {
				id = name;
			}
			ModelNode result = subNode.GetNodeInThisLayer(id);
			if(result != null) return result;
			string checkedId = id;
			string exceptionText = null;
			if(!isUnusableNode && CreatorInstance.CanCreateNodeByType(subNode, ref checkedId, type, out exceptionText)) {
				ModelNode node = CreatorInstance.CreateNode(checkedId, type);
				subNode.AddNodeIntoList(node);
				node.SetParent(subNode);
				bool isRemoved = values.ContainsKey(ModelValueNames.IsRemovedNode) && bool.Parse(values[ModelValueNames.IsRemovedNode]);
				if(!isRemoved) {
					node.IsNewNode = subNode.IsNewNode;
				}
				return node;
			}
			ModelNode unusableNode = AddUnusableNode(subNode, id != name ? id : name + GenerateAutoId(), type);
			unusableNode.SetValue<string>("UnusableNodeName", name);
			return unusableNode;
		}
		private string GetValueCaseInsensitive(IDictionary<string, string> values, string key) {
			foreach(KeyValuePair<string, string> item in values) {
				if(string.Compare(item.Key, key, true) == 0) { 
					return item.Value;
				}
			}
			return null;
		}
		private void ChechUndefinedValues() {
			ModelNode unusableNode = null;
			foreach(string name in ModelValueNamesCore) {
				if(GetValueInfo(name) == null && !ModelValuePersistentPathCalculator.IsHelperValueName(name)) {
					if(unusableNode == null) {
						unusableNode = AddUnusableNode(parent, id, GetType());
					}
					unusableNode.SetModelValueCore(name, FindModelValueCore(name));
				}
			}
		}
		private void CheckPersistentValues() {
			ModelNode unusableNode = null;
			foreach(string name in new List<string>(ModelValueNamesCore)) {
				if(ModelValuePersistentPathCalculator.IsHelperValueName(name)) {
					IModelValue<string> modelValue = (IModelValue<string>)FindModelValueCore(name);
					string reference = modelValue.GetValue(0);
					if(!string.IsNullOrEmpty(reference)) {
						object value = GetValue(ModelValuePersistentPathCalculator.GetRealValueName(name));
						if(value == null) {
							if(unusableNode == null) {
								unusableNode = AddUnusableNode(parent, id, GetType());
							}
							unusableNode.SetModelValueCore(name, modelValue);
						}
					}
				}
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string GetXmlName() {
			lock(TypesInfo.lockObject) {
				return CreatorInstance.GetXmlName(this);
			}
		}
		void OnPropertyChanging(string propertyName) {
			if(!NodeInfo.IsDefaultValueCalculationInProgress) {
				ModelNode firstLayer = FindFirstLayer();
				PropertyChangingEventHandler handler = firstLayer.propertyChanging;
				if(handler != null) {
					handler(this, new PropertyChangingEventArgs(propertyName));
				}
			}
		}
		void OnPropertyChanged(string propertyName) {
			if(!NodeInfo.IsDefaultValueCalculationInProgress) {
				ModelNode firstLayer = FindFirstLayer();
				PropertyChangedEventHandler handler = firstLayer.propertyChanged;
				if(handler != null) {
					handler(this, new PropertyChangedEventArgs(propertyName));
				}
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public IEnumerable<ModelNode> EnumerateAllLayers() {
			lock(TypesInfo.lockObject) {
				if(Layers == null && Master == null) {
					yield return this;
				}
				else {
					ModelNode master = Master;
					while(master != null) {
						if(master.Layers != null) {
							foreach(ModelNode layer in master.Layers) {
								if(layer.Layers == null) {
									yield return layer;
								}
							}
						}
						master = master.Master;
					}
				}
			}
		}
		public override int GetHashCode() {
			return hashCode;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string Xml {
			get { return new ModelXmlWriter().WriteToString(this, GetCurrentAspectIndex()); }
		}
		#region IModelNode Members
		IModelNode IModelNode.Parent { get { return Parent; } }
		IModelNode IModelNode.Root { get { return Root; } }
		IModelNode IModelNode.GetNode(int index) { return GetNode(index); }
		IModelNode IModelNode.GetNode(string id) { return GetNode(id); }
		void IModelNode.Remove() {
			Delete();
		}
		#endregion
		internal void AddPropertyChangingHandler(PropertyChangingEventHandler handler) {
			ModelNode firstLayer = FindFirstLayer();
			firstLayer.propertyChanging += handler;
		}
		internal void RemovePropertyChangingHandler(PropertyChangingEventHandler handler) {
			ModelNode firstLayer = FindFirstLayer();
			firstLayer.propertyChanging -= handler;
		}
		internal void AddPropertyChangedHandler(PropertyChangedEventHandler handler) {
			ModelNode firstLayer = FindFirstLayer();
			firstLayer.propertyChanged += handler;
		}
		internal void RemovePropertyChangedHandler(PropertyChangedEventHandler handler) {
			ModelNode firstLayer = FindFirstLayer();
			firstLayer.propertyChanged -= handler;
		}
		internal static ModelApplicationBase GetModuleLayerById(ModelApplicationBase app, string id) {
			Guard.ArgumentNotNull(app, "app");
			Guard.ArgumentNotNullOrEmpty(id, "id");
			ModelNode master = (ModelApplicationBase)app.GetMasterRecursive();
			if(master == null) {
				master = app;
			}
			List<ModelNode> layers = new List<ModelNode>();
			foreach(ModelNode layer in master.Layers) {
				if(layer.IsMaster) {
					layers.AddRange(layer.Layers);
				}
				else {
					layers.Add(layer);
				}
			}
			foreach(ModelNode layer in layers) {
				if(layer.Id == id) {
					return (ModelApplicationBase)layer;
				}
			}
			return null;
		}
		internal static ModelNode MoveNodeToOtherLayer(ModelNode nodeToMove, ModelApplicationBase targetLayer, IModelNodeMoveInfo moveInfo) {
			lock(TypesInfo.lockObject) {
				Guard.ArgumentNotNull(nodeToMove, "nodeToMove");
				Guard.ArgumentNotNull(targetLayer, "targetLayer");
				Guard.ArgumentNotNull(moveInfo, "moveInfo");
				if(nodeToMove.CreatorInstance != targetLayer.CreatorInstance) {
					throw new ArgumentException("The node and the layer belong to different models.");
				}
				if(nodeToMove.Root == targetLayer) {
					throw new ArgumentException("The node is already in the target layer.");
				}
				if(nodeToMove.IsMaster) {
					throw new ArgumentException("Master node cannot be moved.");
				}
				if(targetLayer.IsMaster) {
					throw new ArgumentException("Master layer cannot be target layer.");
				}
				if(nodeToMove.IsInFirstLayer) {
					throw new ArgumentException("Generated node cannot be moved.");
				}
				if(targetLayer.IsInFirstLayer) {
					throw new ArgumentException("Cannot move node to generator layer.");
				}
				targetLayer.NodesGeneratorInProgressCounter++;
				nodeToMove.NodesGeneratorInProgressCounter++;
				ModelNode movedNode = null;
				ModelNode targetNodeParent = nodeToMove.Parent != null ? FindTargetNode(nodeToMove.Parent, targetLayer, moveInfo) : null;
				ModelNode targetNode;
				if(targetNodeParent != null) {
					targetNodeParent.EnsureNodes(true);
					targetNode = targetNodeParent.GetNode(nodeToMove.Id);
				}
				else {
					targetNode = FindTargetNode(nodeToMove, targetLayer, moveInfo);
				}
				if(targetNodeParent != null || targetNode != null) {
					movedNode = MoveNode(targetNodeParent, targetNode, nodeToMove, moveInfo);
				}
				nodeToMove.NodesGeneratorInProgressCounter--;
				targetLayer.NodesGeneratorInProgressCounter--;
				if(movedNode != null) {
					targetLayer.Master.Reset();
				}
				return movedNode;
			}
		}
		private static ModelNode FindTargetNode(ModelNode nodeToMove, ModelApplicationBase targetLayer, IModelNodeMoveInfo moveInfo) {
			ModelNode master = nodeToMove.Master;
			ModelNode node = null;
			if(master != null) {
				node = master.FindLayerByRoot(targetLayer);
			}
			if(node != null) {
				return node;
			}
			Stack<ModelNode> stack = new Stack<ModelNode>();
			node = nodeToMove;
			while(!node.IsRoot) {
				stack.Push(node);
				node = node.Parent;
			}
			node = targetLayer;
			while(stack.Count > 0) {
				ModelNode sourceNode = stack.Pop();
				ModelNode targetNode = node.GetNode(sourceNode.Id);
				if(targetNode == null) {
					targetNode = CreateChildTargetNode(node, sourceNode, moveInfo);
				}
				if(targetNode == null) {
					return null;
				}
				targetNode.IsRemovedNode = sourceNode.IsRemovedNode;
				sourceNode.IsNewNode = false;
				sourceNode.IsRemovedNode = false;
				node = targetNode;
			}
			return node;
		}
		private static ModelNode CreateChildTargetNode(ModelNode targetNodeParent, ModelNode childNodeToMove, IModelNodeMoveInfo moveInfo) {
			ModelNode childTargetNode = null;
			if(childNodeToMove.IsNewNode || moveInfo.DoesNodeExist(childNodeToMove)) {
				childTargetNode = targetNodeParent.AddNode(childNodeToMove.Id, childNodeToMove.NodeInfo.GeneratedClass);
				childTargetNode.IsNewNode = childNodeToMove.IsNewNode;
			}
			return childTargetNode;
		}
		private static ModelNode MoveNode(ModelNode targetNodeParent, ModelNode targetNode, ModelNode nodeToMove, IModelNodeMoveInfo moveInfo) {
			if(nodeToMove.IsRemovedNode && !nodeToMove.IsNewNode) {
				if(targetNode != null) {
					if(targetNode.IsNewNode) {
						if(targetNode.Parent != null) {
							targetNode.Parent.RemoveNodeFromList(targetNode);
						}
					}
					else {
						targetNode.IsRemovedNode = true;
					}
				}
				else {
					targetNode = CreateChildTargetNode(targetNodeParent, nodeToMove, moveInfo);
					if(targetNode != null) {
						targetNode.IsRemovedNode = true;
					}
				}
				if(nodeToMove.Parent != null) {
					nodeToMove.Parent.RemoveNodeFromList(nodeToMove);
				}
			}
			else {
				if(targetNode == null) {
					targetNode = CreateChildTargetNode(targetNodeParent, nodeToMove, moveInfo);
				}
				else {
					if(targetNode.GetType() != nodeToMove.GetType()) { 
						bool wasTargetNodeNew = targetNode.IsNewNode;
						if(targetNode.Parent != null) {
							targetNode.Parent.RemoveNodeFromList(targetNode);
						}
						targetNode = CreateChildTargetNode(targetNodeParent, nodeToMove, moveInfo);
						if(targetNode != null) {
							targetNode.IsRemovedNode = !wasTargetNodeNew;
							targetNode.IsNewNode = true;
						}
					}
					else if(nodeToMove.IsRemovedNode && nodeToMove.IsNewNode) { 
						targetNode.ClearModelValuesCore();
						targetNode.ResetNodes();
						targetNode.IsRemovedNode = targetNode.IsRemovedNode || !targetNode.IsNewNode;
						targetNode.IsNewNode = true;
					}
				}
				if(targetNode != null) {
					MoveValues(targetNode, nodeToMove, moveInfo);
					MoveNodes(targetNode, nodeToMove, moveInfo);
					nodeToMove.IsNewNode = false;
					nodeToMove.IsRemovedNode = false;
					if(nodeToMove.NodesCountCore == 0 && nodeToMove.ModelValuesCountCore == 0 && nodeToMove.Parent != null) {
						nodeToMove.Parent.RemoveNodeFromList(nodeToMove);
					}
				}
			}
			return targetNode;
		}
		private static void MoveValues(ModelNode targetNode, ModelNode nodeToMove, IModelNodeMoveInfo moveInfo) {
			foreach(string name in new List<string>(nodeToMove.ModelValueNamesCore)) {
				IModelValue modelValue = nodeToMove.FindModelValueCore(name);
				if(moveInfo.ShouldMoveValue(nodeToMove.NodeInfo, name)) {
					nodeToMove.RemoveModelValueCore(name);
					if(modelValue.IsLocalizable && targetNode.ContainsModelValueCore(name)) {
						targetNode.FindModelValueCore(name).MergeWith(modelValue);
					}
					else {
						targetNode.SetModelValueCore(name, modelValue);
					}
				}
			}
		}
		private static void MoveNodes(ModelNode targetNode, ModelNode nodeToMove, IModelNodeMoveInfo moveInfo) {
			if(nodeToMove.Nodes != null) {
				targetNode.EnsureNodes(true);
				foreach(ModelNode sourceChildNode in new List<ModelNode>(nodeToMove.Nodes)) {
					if(moveInfo.ShouldMoveNode(targetNode.NodeInfo, sourceChildNode.NodeInfo)) {
						ModelNode targetChildNode = targetNode.GetNode(sourceChildNode.Id);
						MoveNode(targetNode, targetChildNode, sourceChildNode, moveInfo);
					}
				}
			}
		}
		#region IModelNodeHiddenMethods Members
		ModelNode IModelNodeHiddenMethods.GetGeneratorLayer() {
			ModelNode node = IsMaster ? this : Master;
			return node != null ? node.GetFirstLayerRecursive() : null;
		}
		#endregion
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public interface IModelNodeMoveInfo {
		bool ShouldMoveNode(ModelNodeInfo nodeInfo, ModelNodeInfo childNodeInfo);
		bool ShouldMoveValue(ModelNodeInfo nodeInfo, string valueName);
		bool DoesNodeExist(ModelNode node);
	}
	public static class ModelNodeSetValueHelper {
		delegate void ModelNodeValueSetter(ModelNode node, string name, object value);
		static Dictionary<Type, ModelNodeValueSetter> setters;
		static Dictionary<Type, ModelNodeValueSetter> settersInThisLayer;
		static ModelNodeSetValueHelper() {
			setters = new Dictionary<Type, ModelNodeValueSetter>();
			settersInThisLayer = new Dictionary<Type, ModelNodeValueSetter>();
		}
		static ModelNodeValueSetter GetSetter(Type type) {
			return GetSetterCore(type, setters, "SetValueCore");
		}
		static ModelNodeValueSetter GetSetterInThisLayer(Type type) {
			return GetSetterCore(type, settersInThisLayer, "SetValueCoreInThisLayer");
		}
		static ModelNodeValueSetter GetSetterCore(Type type, Dictionary<Type, ModelNodeValueSetter> dictionary, string methodName) {
			ModelNodeValueSetter setter = null;
			if(dictionary.TryGetValue(type, out setter)) return setter;
			MethodInfo method = typeof(ModelNodeSetValueHelper).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(type);
			setter = (ModelNodeValueSetter)Delegate.CreateDelegate(typeof(ModelNodeValueSetter), method);
			dictionary[type] = setter;
			return setter;
		}
		static void SetValueCore<T>(ModelNode node, string name, object value) {
			node.SetValue<T>(name, (T)value);
		}
		static void SetValueCoreInThisLayer<T>(ModelNode node, string name, object value) {
			node.SetValueInThisLayer<T>(name, (T)value);
		}
		public static void SetValue(ModelNode node, string name, object value, Type objectType) {
			if(value == null) return;
			ModelNodeValueSetter setter = GetSetter(objectType);
			setter(node, name, value);
		}
		public static void SetValueInThisLayer(ModelNode node, string name, object value, Type objectType) {
			ModelNodeValueSetter setter = GetSetterInThisLayer(objectType);
			setter(node, name, value);
		}
	}
	internal static class ModelValueConverterHelper {
		public static string ConvertToString(object value, TypeConverter converter) {
			if(converter == null) {
				converter = TypeDescriptor.GetConverter(value.GetType());
			}
			if(converter == null) {
				throw new InvalidOperationException(string.Format("There is no converter for type '{0}'", value.GetType()));
			}
			return converter.ConvertToInvariantString(value);
		}
		public static object ConvertFromString(string value, Type type, TypeConverter converter) {
			if(converter == null) {
				converter = TypeDescriptor.GetConverter(type);
			}
			if(converter == null) {
				throw new InvalidOperationException(string.Format("There is no converter for type '{0}'", type));
			}
			return converter.ConvertFromInvariantString(value);
		}
	}
	[Serializable]
	public sealed class DuplicateModelNodeIdException : ModelException {
		private const string ErrorMessageTemplate = "There is already node with Id '{0}'. The node: {1}.";
		private DuplicateModelNodeIdException(SerializationInfo info, StreamingContext context) : base(info, context) { }
		internal DuplicateModelNodeIdException(string nodeId, string nodePath) : base(BuildMessage(nodeId, nodePath)) { }
		private static string BuildMessage(string nodeId, string nodePath) {
			return string.Format(ErrorMessageTemplate, nodeId, nodePath);
		}
	}
	[Serializable]
	public sealed class CannotCreateModelNodeByTypeException : ModelException {
		private const string ErrorMessageTemplate = "Cannot create model node by type '{0}'. {1}";
		private CannotCreateModelNodeByTypeException(SerializationInfo info, StreamingContext context) : base(info, context) { }
		internal CannotCreateModelNodeByTypeException(Type type, string reason) : base(BuildMessage(type, reason)) { }
		private static string BuildMessage(Type type, string reason) {
			return string.Format(ErrorMessageTemplate, type != null ? type.FullName : "null", reason);
		}
	}
	interface IModelNodeHiddenMethods {
		ModelNode GetGeneratorLayer();
	}
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class ModelNodeHiddenMethods {
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static ModelNode GetGeneratorLayer(ModelNode node) {
			return ((IModelNodeHiddenMethods)node).GetGeneratorLayer();
		}
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void RemoveUserLayers(IModelApplication model) {  
			ModelApplicationBase master = (ModelApplicationBase)model;
			while(master.LayersCount > 0) {
				master.RemoveLayerInternal(master.LastLayer);
			}
		}
	}
}
