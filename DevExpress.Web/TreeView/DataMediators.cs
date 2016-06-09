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
using System.Text;
using System.Web.UI;
using DevExpress.Web.Internal;
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using System.ComponentModel;
using DevExpress.Web;
namespace DevExpress.Web.Internal {
	public abstract class TreeViewDataMediator {
		ASPxTreeView treeView = null;
		public TreeViewDataMediator(ASPxTreeView treeView) {
			this.treeView = treeView;
		}
		protected internal ASPxTreeView TreeView {
			get { return this.treeView; }
		}
		protected TreeViewNodeCollection Nodes {
			get { return RootNode.Nodes; }
		}
		public virtual void OnNodeClick(string nodeID) {
			TreeViewNode clickedNode = TreeView.FindNodeByID(nodeID);
			if (clickedNode != null)
				TreeView.OnNodeClick(new TreeViewNodeEventArgs(clickedNode));
		}
		public virtual void OnExpandedChanged(string nodeID) {
			TreeViewNode expandedNode = TreeView.FindNodeByID(nodeID);
			if (expandedNode != null)
				TreeView.OnExpandedChanged(new TreeViewNodeEventArgs(expandedNode));
		}
		public virtual void OnCheckedChanged(string nodeID) {
			TreeViewNode checkedNode = TreeView.FindNodeByID(nodeID);
			if (checkedNode != null)
				TreeView.OnCheckedChanged(new TreeViewNodeEventArgs(checkedNode));
		}
		public void OnExpandedChanging(string nodeID, bool expanding) {
			TreeViewNode expandingNode = TreeView.FindNodeByID(nodeID);
			if (expandingNode == null)
				return;
			TreeViewNodeCancelEventArgs e = new TreeViewNodeCancelEventArgs(expandingNode);
			TreeView.OnExpandedChanging(e);
			if (!e.Cancel) {
				expandingNode.Expanded = expanding;
				TreeView.OnExpandedChanged(new TreeViewNodeEventArgs(expandingNode));
			}
		}
		public abstract TreeViewNode RootNode { get; }
		public abstract TreeViewNode SelectedNode { get; set; }
		public string CreateSerializedNodesStateString() {
			return HtmlConvertor.ToJSON(CreateSerializedNodesState(), false, false, true);
		}
		public abstract object[] CreateSerializedNodesState();
		public void SyncNodesState(string serializedNodesState) {
			SyncNodesState(HtmlConvertor.FromJSON<ArrayList>(serializedNodesState));
		}
		public abstract void SyncNodesState(ArrayList nodesState);
		public abstract void PerformDataBinding();
		public abstract void OnLoadViewState();
		public abstract IStateManager[] GetStateManagedDataObjects();
		public abstract TreeViewNodeCollection GetNodeChildrenOnCallback(string nodeID);
		public abstract Hashtable GetNodeNames();
	}
	public class RealModeTreeViewDataMediator : TreeViewDataMediator {
		internal const string
			 DefaultNavigateUrlFieldName = "NavigateUrl",
			 DefaultTextFieldName = "Text",
			 DefaultToolTipFieldName = "ToolTip",
			 DefaultImageUrlFieldName = "ImageUrl",
			 DefaultNameFieldName = "Name",
			 EnabledFieldName = "Enabled",
			 CheckedFieldName = "Checked",
			 TargetFieldName = "Target";
		bool isDataBound = false;
		TreeViewNode rootNode = null;
		TreeViewNode selectedNode = null;
		public RealModeTreeViewDataMediator(ASPxTreeView treeView)
			: base(treeView) {
		}
		protected bool IsDataBound {
			get { return this.isDataBound; }
			set { this.isDataBound = value; }
		}
		public override TreeViewNode RootNode {
			get {
				if (this.rootNode == null)
					this.rootNode = new TreeViewNode(TreeView);
				return this.rootNode;
			}
		}
		public override TreeViewNode SelectedNode {
			get {
				if (this.selectedNode != null && this.selectedNode.TreeView != TreeView)
					return null;
				return this.selectedNode; 
			}
			set {
				if (value == null || value.TreeView == TreeView)
					this.selectedNode = value;
			}
		}
		protected string EffectiveNavigateUrlField {
			get {
				return string.IsNullOrEmpty(TreeView.NavigateUrlField) ? DefaultNavigateUrlFieldName :
					TreeView.NavigateUrlField;
			}
		}
		protected string EffectiveTextField {
			get {
				return string.IsNullOrEmpty(TreeView.TextField) ? DefaultTextFieldName :
					TreeView.TextField;
			}
		}
		protected string EffectiveToolTipField {
			get {
				return string.IsNullOrEmpty(TreeView.ToolTipField) ? DefaultToolTipFieldName :
					TreeView.ToolTipField;
			}
		}
		protected string EffectiveImageUrlField {
			get {
				return string.IsNullOrEmpty(TreeView.ImageUrlField) ? DefaultImageUrlFieldName :
					TreeView.ImageUrlField;
			}
		}
		protected string EffectiveNameField {
			get {
				return string.IsNullOrEmpty(TreeView.NameField) ? DefaultNameFieldName :
					TreeView.NameField;
			}
		}
		public override object[] CreateSerializedNodesState() {
			string selectedNodeID = string.Empty;
			if (SelectedNode != null)
				selectedNodeID = SelectedNode.GetID();
			return new object[] { GetNodesExpandedState(), selectedNodeID, GetNodesCheckedState() };
		}
		protected Hashtable GetNodesCheckedState() {
			Hashtable checkedState = new Hashtable();
			if (TreeView.AllowCheckNodes)
				GetNodesCheckedStateRecursive(Nodes, checkedState);
			return checkedState;
		}
		protected void GetNodesCheckedStateRecursive(TreeViewNodeCollection nodes, Hashtable checkedState) {
			foreach (TreeViewNode node in nodes) {
				if (node.AllowCheck)
					checkedState.Add(node.GetID(), ASPxTreeView.SerializeCheckStateEnumValue(node.CheckState));
				if (node.Nodes.Count > 0)
					GetNodesCheckedStateRecursive(node.Nodes, checkedState);
			}
		}
		protected Hashtable GetNodesExpandedState() {
			Hashtable expandedState = new Hashtable();
			GetNodesExpandedStateRecursive(Nodes, expandedState);
			return expandedState;
		}
		protected void GetNodesExpandedStateRecursive(TreeViewNodeCollection nodes, Hashtable expandedState) {
			foreach (TreeViewNode node in nodes) {
				if (node.Nodes.Count == 0)
					continue;
				expandedState.Add(node.GetID(), ASPxTreeView.SerializeBooleanValue(node.Expanded));
				GetNodesExpandedStateRecursive(node.Nodes, expandedState);
			}
		}
		public override void SyncNodesState(ArrayList nodesState) {
			if (nodesState.Count != 3) return;
			SyncNodesExpandedState(nodesState[0] as Hashtable);
			SelectedNode = TreeView.FindNodeByID(nodesState[1] as string);
			SyncNodesCheckedState(nodesState[2] as Hashtable);
		}
		protected void SyncNodesCheckedState(Hashtable checkedState) {
			foreach (DictionaryEntry entry in checkedState) {
				TreeViewNode node = TreeView.FindNodeByID(entry.Key as string);
				if (node != null && TreeView.AllowCheckNodes && node.AllowCheck)
					node.SetCheckState(ASPxTreeView.DeserializeCheckStateEnumValue(entry.Value as string));
			}
		}
		protected void SyncNodesExpandedState(Hashtable expandedState) {
			foreach (DictionaryEntry entry in expandedState) {
				TreeViewNode node = TreeView.FindNodeByID(entry.Key as string);
				if (node != null)
					node.Expanded = ASPxTreeView.DeserializeBooleanValue(entry.Value as string);
			}
		}
		public override void PerformDataBinding() {
			if (!TreeView.DesignMode && IsDataBound &&
				string.IsNullOrEmpty(TreeView.DataSourceID) && TreeView.DataSource == null) {
				Nodes.Clear();
			} else if (!string.IsNullOrEmpty(TreeView.DataSourceID) || TreeView.DataSource != null) {
				DataBindNodes();
				TreeView.ResetControlHierarchy();
			}
		}
		protected void DataBindNodes() {
			HierarchicalDataSourceView view = TreeView.GetTreeViewData(RootNode.DataPath);
			if (view == null)
				return;
			IHierarchicalEnumerable enumerable = view.Select();
			Nodes.Clear();
			if (enumerable != null)
				DataBindNodeRecursive(RootNode, enumerable);
		}
		protected void DataBindNodeRecursive(TreeViewNode node, IHierarchicalEnumerable enumerable) {
			foreach (object dataObject in enumerable) {
				IHierarchyData data = enumerable.GetHierarchyData(dataObject);
				TreeViewNode childNode = new TreeViewNode();
				childNode.DataPath = data.Path;
				childNode.DataItem = dataObject;
				DataBindNodeProperties(childNode, dataObject);
				node.Nodes.Add(childNode);
				TreeView.OnNodeDataBound(new TreeViewNodeEventArgs(childNode));
				if (data.HasChildren)
					DataBindNodeRecursive(childNode, data.GetChildren());
			}
		}
		protected void DataBindNodeProperties(TreeViewNode node, object dataObject) {
			SiteMapNode siteMapNode = dataObject as SiteMapNode;
			if (siteMapNode != null) {
				DataBindPropertiesToSiteMapNode(node, siteMapNode);
				return;
			}
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(dataObject);
			if (properties != null)
				DataBindPropertiesToDataObjectProperties(node, properties, dataObject);
		}
		protected void DataBindPropertiesToSiteMapNode(TreeViewNode node, SiteMapNode siteMapNode) {
			node.NavigateUrl = siteMapNode.Url;
			node.Text = siteMapNode.Title;
			node.ToolTip = siteMapNode.Description;
			if (siteMapNode[EffectiveNameField] != null)
				node.Name = siteMapNode[EffectiveNameField];
			if (siteMapNode[EffectiveNavigateUrlField] != null)
				node.NavigateUrl = siteMapNode[EffectiveNavigateUrlField];
			if (siteMapNode[EffectiveImageUrlField] != null)
				node.Image.Url = siteMapNode[EffectiveImageUrlField];
			if (siteMapNode[EnabledFieldName] != null)
				node.Enabled = bool.Parse(siteMapNode[EnabledFieldName]);
			if (siteMapNode[CheckedFieldName] != null)
				node.Checked = bool.Parse(siteMapNode[CheckedFieldName]);
			if (siteMapNode[TargetFieldName] != null)
				node.Target = siteMapNode[TargetFieldName];
		}
		protected void DataBindPropertiesToDataObjectProperties(TreeViewNode node,
			PropertyDescriptorCollection properties, object dataObject) {
			DataUtils.GetPropertyValue<bool>(dataObject, EnabledFieldName, properties, value => { node.Enabled = value; });
			DataUtils.GetPropertyValue<bool>(dataObject, CheckedFieldName, properties, value => { node.Checked = value; });
			DataUtils.GetPropertyValue<string>(dataObject, TargetFieldName, properties, value => { node.Target = value; });
			DataUtils.GetPropertyValue<string>(dataObject, EffectiveNameField, properties, value => { node.Name = value; });
			DataUtils.GetPropertyValue<string>(dataObject, EffectiveNavigateUrlField, properties, value => { node.NavigateUrl = value; });
			DataUtils.GetPropertyValue<string>(dataObject, EffectiveImageUrlField, properties, value => { node.Image.Url = value; });
			DataUtils.GetPropertyValue<string>(dataObject, EffectiveToolTipField, properties, value => { node.ToolTip = value; });
			if(!DataUtils.GetPropertyValue<string>(dataObject, EffectiveTextField, properties, value => { node.Text = value; }))
				node.Text = dataObject.ToString();
		}
		public override void OnLoadViewState() {
			IsDataBound = !string.IsNullOrEmpty(TreeView.DataSourceID) || TreeView.DataSource != null;
		}
		public override IStateManager[] GetStateManagedDataObjects() {
			return new IStateManager[] { Nodes };
		}
		public override TreeViewNodeCollection GetNodeChildrenOnCallback(string nodeID) {
			TreeViewNode node = TreeView.FindNodeByID(nodeID);
			if (node != null)
				return node.Nodes;
			return null;
		}
		public override Hashtable GetNodeNames() {
			return new Hashtable();
		}
	}
	public class VirtualModeTreeViewDataMediator : TreeViewDataMediator {
		TreeViewVirtualNode rootNode = null;
		string selectedNodeID = string.Empty;
		Hashtable expandedState = null;
		Hashtable checkedState = null;
		Hashtable nodeNames = null;
		public VirtualModeTreeViewDataMediator(ASPxTreeView treeView)
			: base(treeView) {
		}
		protected string SelectedNodeID {
			get { return this.selectedNodeID; }
			set { this.selectedNodeID = value; }
		}
		protected Hashtable ExpandedState {
			get {
				if (this.expandedState == null)
					this.expandedState = new Hashtable();
				return this.expandedState;
			}
		}
		protected Hashtable CheckedState {
			get {
				if (this.checkedState == null)
					this.checkedState = new Hashtable();
				return this.checkedState;
			}
		}
		protected Hashtable NodeNames {
			get {
				if (this.nodeNames == null)
					this.nodeNames = new Hashtable();
				return this.nodeNames;
			}
		}
		public override TreeViewNode RootNode {
			get {
				if (this.rootNode == null) {
					this.rootNode = new TreeViewVirtualNode(TreeView);
					this.rootNode.DataMediator = this;
				}
				return this.rootNode;
			}
		}
		public override TreeViewNode SelectedNode {
			get { return TreeView.FindNodeByID(SelectedNodeID); }
			set {
				if (value == null)
					SelectedNodeID = string.Empty;
				else if (value.TreeView == TreeView)
					SelectedNodeID = value.GetID();
			}
		}
		public void RemoveNodesFromState() {
			ExpandedState.Clear();
			CheckedState.Clear();
			NodeNames.Clear();
			SelectedNodeID = string.Empty;
		}
		public void RemoveNodesFromState(TreeViewNode startNode) {
			string startNodeID = startNode.GetID();
			List<string> removeIDs = new List<string>();
			foreach(string nodeID in NodeNames.Keys) {
				if(nodeID != startNodeID && nodeID.StartsWith(startNodeID))
					removeIDs.Add(nodeID);
			}
			foreach(string nodeID in removeIDs) {
				ExpandedState.Remove(nodeID);
				CheckedState.Remove(nodeID);
				NodeNames.Remove(nodeID);
				if (SelectedNodeID == nodeID)
					selectedNodeID = string.Empty;
			}
		}
		protected void AppendHashtable(Hashtable src, Hashtable dest) {
			foreach (string key in src.Keys)
				dest[key] = src[key];
		}
		protected internal string GetNodeName(string nodeID) {
			if (NodeNames.ContainsKey(nodeID))
				return NodeNames[nodeID] as string;
			return string.Empty;
		}
		protected internal void SetNodeName(string nodeID, string name) {
			if (NodeNames.ContainsValue(name) && (NodeNames[nodeID] as string) != name)
				throw new ArgumentException(StringResources.TreeView_NotUniqueVirtualNodeName);
			NodeNames[nodeID] = name;
		}
		protected internal bool GetNodeExpanded(string nodeID) {
			return ASPxTreeView.DeserializeBooleanValue(ExpandedState[nodeID] as string);
		}
		protected internal void SetNodeExpanded(string nodeID, bool expanded) {
			ExpandedState[nodeID] = ASPxTreeView.SerializeBooleanValue(expanded);
		}
		protected internal CheckState GetNodeCheckState(string nodeID) {
			return ASPxTreeView.DeserializeCheckStateEnumValue(CheckedState[nodeID] as string);
		}
		protected internal void SetNodeCheckState(string nodeID, CheckState checkState) {
			CheckedState[nodeID] = ASPxTreeView.SerializeCheckStateEnumValue(checkState);
		}
		protected internal List<TreeViewNode> PopulateVirtualNodes(string nodeName) {
			List<TreeViewNode> nodes = new List<TreeViewNode>();
			TreeViewVirtualModeCreateChildrenEventArgs e =
				new TreeViewVirtualModeCreateChildrenEventArgs(nodeName);
			TreeView.OnVirtualModeCreateChildren(e);
			if (e.Children != null) {
				for (int i = 0; i < e.Children.Count; i++) {
					e.Children[i].DataMediator = this;
					nodes.Add(e.Children[i]);
				}
			}
			return nodes;
		}
		protected internal void SyncNodesInnerStateWithMediator(TreeViewNodeCollection nodes) {
			foreach(TreeViewVirtualNode node in nodes) {
				string childNodeID = node.GetID();
				VirtualModeTreeViewDataMediator nodeInnerDataMediator = node.InnerDataMediator;
				if(!ExpandedState.ContainsKey(childNodeID)) {
					if(nodeInnerDataMediator.ExpandedState.Count != 0) {
						foreach(object value in nodeInnerDataMediator.ExpandedState.Values)
							ExpandedState[childNodeID] = value;
					}
				}
				if(!CheckedState.ContainsKey(childNodeID)) {
					if(nodeInnerDataMediator.CheckedState.Count != 0) {
						foreach(object value in nodeInnerDataMediator.CheckedState.Values)
							CheckedState[childNodeID] = value;
					}
				}
			}
		}
		protected internal virtual void SyncNodesWithMediator(string parentNodeID, TreeViewNodeCollection nodes) {
			foreach (TreeViewVirtualNode node in nodes) {
				string nodeID = node.GetID();
				SetNodeName(nodeID, node.Name);
				if (!ExpandedState.ContainsKey(nodeID))
					SetNodeExpanded(nodeID, node.Expanded);
				if (!CheckedState.ContainsKey(nodeID)) {
					if (TreeView.CheckNodesRecursive) {
						string parentID = TreeViewVirtualNode.GetParentID(nodeID);
						while (!string.IsNullOrEmpty(parentID)) {
							if (CheckedState.ContainsKey(parentID)) {
								CheckState parentCheckState = GetNodeCheckState(parentID);
								if(parentCheckState != CheckState.Indeterminate)
									node.CheckState = parentCheckState;
								break;
							}
							parentID = TreeViewVirtualNode.GetParentID(parentID);
						}
					}
					if(node.Nodes.Count != 0)
						SyncNodesWithMediator(nodeID, node.Nodes);
					SetNodeCheckState(nodeID, node.CheckState);
				} else {
					if(node.Nodes.Count != 0) {
						if(TreeView.CheckNodesRecursive)
							node.SyncedWithMediator = true;
						SyncNodesWithMediator(nodeID, node.Nodes);
					}
				}
				if(!node.SyncedWithMediator)
					node.SyncedWithMediator = true;
			}
			if(!nodes.IsEmpty && TreeView.CheckNodesRecursive)
				TreeViewNode.UpdateAncestorsCheckedState(nodes[0]);
		}
		public override object[] CreateSerializedNodesState() {
			return new object[] { ExpandedState, SelectedNodeID, CheckedState, NodeNames };
		}
		public override void SyncNodesState(ArrayList nodesState) {
			if (nodesState.Count != 4) return;
			AppendHashtable(nodesState[0] as Hashtable, ExpandedState);
			SelectedNodeID = nodesState[1] as string;
			AppendHashtable(nodesState[2] as Hashtable, CheckedState);
			AppendHashtable(nodesState[3] as Hashtable, NodeNames);
		}
		public override void PerformDataBinding() {
		}
		public override void OnLoadViewState() {
		}
		public override IStateManager[] GetStateManagedDataObjects() {
			return new IStateManager[] { };
		}
		public override void OnExpandedChanged(string nodeID) {
			if (TreeView.HasExpandedChangedHandler)
				base.OnExpandedChanged(nodeID);
		}
		public override void OnNodeClick(string nodeID) {
			if (TreeView.HasNodeClickHandler)
				base.OnNodeClick(nodeID);
		}
		public override void OnCheckedChanged(string nodeID) {
			if (TreeView.HasCheckedChangedHandler)
				base.OnCheckedChanged(nodeID);
		}
		public override TreeViewNodeCollection GetNodeChildrenOnCallback(string nodeID) {
			TreeViewNodeCollection children = new TreeViewNodeCollection();
			if(TreeView.SyncSelectionMode == SyncSelectionMode.None) {
				List<TreeViewNode> virtualNodesList = PopulateVirtualNodes(NodeNames[nodeID] as string);
				if(virtualNodesList != null)
					children.AddRange(virtualNodesList);
			} else
				children = TreeView.FindNodeByID(nodeID).Nodes;
			if(!children.IsEmpty) {
				foreach(TreeViewVirtualNode virtualNode in children)
					virtualNode.ParentIndexPath = TreeViewNode.GetIndexPathByID(nodeID);
				SyncNodesInnerStateWithMediator(children);
				SyncNodesWithMediator(nodeID, children);
			}
			return children;
		}
		public override Hashtable GetNodeNames() {
			return NodeNames;
		}
	}
}
