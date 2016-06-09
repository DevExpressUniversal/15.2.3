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
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates.ActionContainers;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.Templates.ActionContainers {
	public class TreeViewNodeChoiceActionItemWrapper : TreeListNodeChoiceActionItemWrapperBase {
		public const string TextFieldName = "TextField";
		public const string ImageFieldName = "ImageField";
		public const string VisibleFieldName = "VisibleField";
		public const string EnabledFieldName = "EnabledField";
		public const string HasViewFieldName = "HasViewField";
		private TreeViewNode node;
		private bool showImages;
		protected override void SetImageInfo(DevExpress.ExpressApp.Utils.ImageInfo imageInfo) {
			if(showImages) {
				node.Image.Url = imageInfo.ImageUrl;
				node.Image.Width = imageInfo.Width;
				node.Image.Height = imageInfo.Height;
			}
		}
		public override void SetCaption(string caption) {
			node.Text = caption;
		}
		public override void SetData(object data) {
			node.DataItem = data;
		}
		public override void SetEnabled(bool enabled) {
			node.Enabled = enabled;
		}
		public override void SetVisible(bool visible) {
			node.Visible = visible;
		}
		public override void SetToolTip(string toolTip) {
			node.ToolTip = toolTip;
		}
		public TreeViewNodeChoiceActionItemWrapper(ChoiceActionItem item, TreeViewNode node, ChoiceActionBase action) : this(item, node, action, null, null, true) { }
		public TreeViewNodeChoiceActionItemWrapper(ChoiceActionItem item, TreeViewNode node, ChoiceActionBase action, bool showImages) : this(item, node, action, null, null, showImages) { }
		public TreeViewNodeChoiceActionItemWrapper(ChoiceActionItem item, TreeViewNode node, ChoiceActionBase action, string defaultParentImageName, string defaultLeafImageName) : this(item, node, action, defaultParentImageName, defaultLeafImageName, true) { }
		public TreeViewNodeChoiceActionItemWrapper(ChoiceActionItem item, TreeViewNode node, ChoiceActionBase action, string defaultParentImageName, string defaultLeafImageName, bool showImages)
			: base(item, action, defaultParentImageName, defaultLeafImageName) {
			this.showImages = showImages;
			this.node = node;
			SyncronizeWithItem();
		}
		public TreeViewNode Node {
			get { return node; }
		}
	}
	public class TreeViewNavigationControl : IWebNavigationControl, INavigationControlTestable, IDisposableExt, ISupportCallbackStartupScriptRegistering, IXafCallbackHandler {
		private SingleChoiceAction singleChoiceAction;
		private ASPxTreeView tree;
		private List<TreeViewNodeChoiceActionItemWrapper> itemWappers;
		private ASPxTreeViewSupportCallbackStartupScriptRegisteringImpl startupScriptRegisteringImpl;
		private Boolean isDisposed;
		private bool showImages;
		private void action_SelectedItemChanged(object sender, EventArgs e) {
			UpdateSelection();
		}
		private void UpdateSelection() {
			TreeViewNode nodeToSelect = null;
			if(singleChoiceAction.SelectedItem != null) {
				nodeToSelect = FindNodeByActionItem(singleChoiceAction.SelectedItem);
			}
			tree.SelectedNode = nodeToSelect;
		}
		private void action_ItemsChanged(object sender, ItemsChangedEventArgs e) {
			if(!IsDisposed) {
				BuildControl((ChoiceActionBase)sender);
			}
		}
		private void ClearWrappers() {
			foreach(TreeViewNodeChoiceActionItemWrapper itemWrapper in itemWappers) {
				itemWrapper.Dispose();
			}
			itemWappers.Clear();
		}
		private void BuildControl(ChoiceActionBase action) {
			tree.Nodes.Clear();
			ClearWrappers();
			foreach(ChoiceActionItem item in actionItems) {
				CreateItems(item, tree.RootNode, action);
			}
			if(singleChoiceAction.SelectedItem != null) {
				tree.SelectedNode = FindNodeByActionItem(singleChoiceAction.SelectedItem);
				tree.ExpandToNode(tree.SelectedNode);
			}
			UpdateSelection();
		}
		private void CreateItems(ChoiceActionItem actionItem, TreeViewNode parentNode, ChoiceActionBase action) {
			IModelNavigationItem actionItemModel = actionItem.Model as IModelNavigationItem;	
			if((actionItemModel == null || actionItemModel.Visible) && actionItem.Active) {
				TreeViewNode node = parentNode.Nodes.Add(actionItem.Caption);
				node.Name = actionItem.GetIdPath();
				TreeViewNodeChoiceActionItemWrapper nodeWrapper;
				IModelRootNavigationItems modelRootNavigationItems;
				if(ModelNavigationItemsDomainLogic.TryFindRootNavigationItems(actionItem.Model, out modelRootNavigationItems)) {
					nodeWrapper = new TreeViewNodeChoiceActionItemWrapper(actionItem, node, action, modelRootNavigationItems.DefaultParentImageName, modelRootNavigationItems.DefaultLeafImageName, showImages);
				}
				else {
					nodeWrapper = new TreeViewNodeChoiceActionItemWrapper(actionItem, node, action, showImages);
				}
				itemWappers.Add(nodeWrapper);
				foreach(ChoiceActionItem choiceAction in actionItem.Items) {
					CreateItems(choiceAction, node, action);
				}
			}
		}
		private ChoiceActionItem FindActionItemByNode(TreeViewNode node, ChoiceActionItemCollection actionItems) {
			ChoiceActionItem result = null;
			foreach(ChoiceActionItem actionItem in actionItems) {
				if(node.Name == actionItem.GetIdPath()) {
					result = actionItem;
					break;
				}
				else if(actionItem.Items.Count > 0) {
					result = FindActionItemByNode(node, actionItem.Items);
					if(result != null) {
						break;
					}
				}
			}
			return result;
		}
		private void SubscribeToTreeListEvents() {
			tree.CustomJSProperties += new DevExpress.Web.CustomJSPropertiesEventHandler(tree_CustomJSProperties);
			tree.NodeClick += new TreeViewNodeEventHandler(tree_NodeClick);
			tree.Load += new EventHandler(tree_Load);
		}
		private void tree_Load(object sender, EventArgs e) {
			if(WebApplicationStyleManager.IsNewStyle) {
				ASPxTreeView view = (ASPxTreeView)sender;
				if(view.NodeTextTemplate is TreeViewNavigationTemplate && !view.ShowExpandButtons) {
					((TreeViewNavigationTemplate)view.NodeTextTemplate).CustomizeTreeView(view);
				}
			}
			ICallbackManagerHolder holder = tree.Page as ICallbackManagerHolder;
			if(holder != null) {
				tree.ClientSideEvents.NodeClick = GetNodeClickScript(holder);
				holder.CallbackManager.RegisterHandler(tree.UniqueID, this);
				if(PopupWindow.IsRefreshOnCallback) {
					holder.CallbackManager.PreRenderInternal += delegate(object s, EventArgs args) {
						HandleRefreshOnCallback();
					};
				}
			}
		}
		private string GetNodeClickScript(ICallbackManagerHolder holder) {
			string nodeClickHandlerCore =
@"var hasView = sender.cpNodeKeyToInfoMap[args.node.name];
    if(!hasView) {
        args.node.SetExpanded(!args.node.GetExpanded());
    } else {
        if(typeof xaf == 'undefined' || !xaf.ConfirmUnsavedChangedController || xaf.ConfirmUnsavedChangedController.CanProcessCallbackForNavigation(sender,args)){    
            " + holder.CallbackManager.GetScript(tree.UniqueID, "args.node.name", String.Empty, singleChoiceAction.Model.GetValue<bool>("IsPostBackRequired")) + @"
        }
    }";
			if(WebApplicationStyleManager.IsNewStyle) {
				return
@"function (sender, args) {
    args.processOnServer = false;
    if(args.htmlEvent.target.className.indexOf('dxtv-btn') == -1){
        " + nodeClickHandlerCore + @"
    }
}";
			}
			else {
				return
@"function (sender, args) {
    args.processOnServer = false;
    " + nodeClickHandlerCore + @"
}";
			}
		}
		private void HandleRefreshOnCallback() {
			if(singleChoiceAction != null) {
				UpdateSelection();
			}
			if(tree.SelectedNode != null) {
				tree.ExpandToNode(tree.SelectedNode);
			}
		}
		private void tree_NodeClick(object source, TreeViewNodeEventArgs e) {
			singleChoiceAction.SelectedItemChanged -= new EventHandler(action_SelectedItemChanged);
			ChoiceActionItem actionItem = FindActionItemByNode(e.Node);
			if(singleChoiceAction != null) { 
				singleChoiceAction.SelectedItemChanged += new EventHandler(action_SelectedItemChanged);
			}
			singleChoiceAction.DoExecute(actionItem);
		}
		private void tree_CustomJSProperties(object sender, DevExpress.Web.CustomJSPropertiesEventArgs e) {
			ICollection<TreeViewNode> allNodes = GetAllNodes();
			Dictionary<string, bool> nodeKeyToInfoMap = new Dictionary<string, bool>();
			foreach(TreeViewNode node in allNodes) {
				nodeKeyToInfoMap.Add(node.Name, node.DataItem != null);
			}
			e.Properties["cpNodeKeyToInfoMap"] = nodeKeyToInfoMap;
			if(TestScriptsManager.EasyTestEnabled) {
				Dictionary<string, List<string>> nodeCaptionToKeyMap = new Dictionary<string, List<string>>();
				Dictionary<string, List<string>> nodeFullCaptionToKeyMap = new Dictionary<string, List<string>>();
				Dictionary<string, string> nodeKeyToFullCaptionMap = new Dictionary<string, string>();
				foreach(TreeViewNode node in allNodes) {
					string caption = node.Text;
					AddValueToDictionary(nodeCaptionToKeyMap, caption, node.Name);
					string fullCaption = GetFullCaption(node);
					AddValueToDictionary(nodeFullCaptionToKeyMap, fullCaption, node.Name);
					nodeKeyToFullCaptionMap.Add(node.Name, fullCaption);
				}
				e.Properties["cpEntriesAsJSON"] = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(GetAllPaths().ToArray());
				e.Properties["cpNodeCaptionToNodeKeyMap"] = nodeCaptionToKeyMap;
				e.Properties["cpNodeFullCaptionToNodeKeyMap"] = nodeFullCaptionToKeyMap;
				e.Properties["cpNodeNodeKeyToFullCaptionMap"] = nodeKeyToFullCaptionMap;
			}
		}
		internal List<NavigationNodePath> GetAllPaths() {
			List<NavigationNodePath> result = new List<NavigationNodePath>();
			foreach(TreeViewNode node in GetAllNodes()) {
				NavigationNodePath path = new NavigationNodePath();
				TreeViewNode currentNode = node;
				do {
					path.Entries.Insert(0, new NavigationNodePathEntry(currentNode.Name, currentNode.Text));
					currentNode = currentNode.Parent;
				} while(currentNode.Parent != null);
				path.TreeId = tree.ClientID;
				result.Add(path);
			}
			return result;
		}
		private void AddValueToDictionary(Dictionary<string, List<string>> dictionary, string key, string value) {
			List<string> valueList;
			if(!dictionary.TryGetValue(key, out valueList)) {
				valueList = new List<string>();
				dictionary.Add(key, valueList);
			}
			valueList.Add(value);
		}
		private void UnsubscribeFromTreeListEvents() {
			if(tree != null) {
				tree.CustomJSProperties -= new DevExpress.Web.CustomJSPropertiesEventHandler(tree_CustomJSProperties);
				tree.NodeClick -= new TreeViewNodeEventHandler(tree_NodeClick);
				tree.Load -= new EventHandler(tree_Load);
			}
		}
		private void startupScriptRegisteringImpl_RegisterCallbackStartupScript(object sender, RegisterCallbackStartupScriptEventArgs e) {
			OnRegisterCallbackStartupScript(e);
		}
		private string GetFullCaption(TreeViewNode node) {
			string result = "";
			if(node != null && !string.IsNullOrEmpty(node.Name)) {
				result = GetFullCaption(node.Parent);
				string itemPrefix = result != "" ? "." : "";
				string itemText = node.Text;
				if(!string.IsNullOrEmpty(itemText)) {
					result += itemPrefix + itemText;
				}
			}
			return result;
		}
		protected virtual void OnRegisterCallbackStartupScript(RegisterCallbackStartupScriptEventArgs e) {
			if(RegisterCallbackStartupScript != null) {
				RegisterCallbackStartupScript(this, e);
			}
		}
		public TreeViewNode FindNodeByActionItem(ChoiceActionItem actionItem) {
			foreach(TreeViewNode node in GetAllNodes()) {
				if(node.Name == actionItem.GetIdPath()) {
					return node;
				}
			}
			return null;
		}
		public ChoiceActionItem FindActionItemByNode(TreeViewNode node) {
			return FindActionItemByNode(node, actionItems);
		}
		public TreeViewNavigationControl()
			: this(true) {
		}
		public TreeViewNavigationControl(bool showImages) {
			this.showImages = showImages;
			tree = new ASPxTreeView();
			if(WebApplicationStyleManager.IsNewStyle) {
				tree.ShowExpandButtons = false;
				tree.NodeTextTemplate = new TreeViewNavigationTemplate();
			}
			startupScriptRegisteringImpl = new ASPxTreeViewSupportCallbackStartupScriptRegisteringImpl(tree);
			startupScriptRegisteringImpl.RegisterCallbackStartupScript += new EventHandler<RegisterCallbackStartupScriptEventArgs>(startupScriptRegisteringImpl_RegisterCallbackStartupScript);
			RenderHelper.SetupASPxWebControl(tree);
			tree.EnableCallBacks = true;
			tree.EnableClientSideAPI = true;
			tree.AutoPostBack = false;
			tree.ID = "TL";
			tree.ShowTreeLines = true;
			tree.Border.BorderStyle = BorderStyle.None;
			SubscribeToTreeListEvents();
			tree.Width = Unit.Percentage(100);
			itemWappers = new List<TreeViewNodeChoiceActionItemWrapper>();
		}
		public ICollection<TreeViewNode> GetAllNodes(TreeViewNode sourceNode) {
			List<TreeViewNode> result = new List<TreeViewNode>();
			foreach(TreeViewNode node in sourceNode.Nodes) {
				result.Add(node);
				result.AddRange(GetAllNodes(node));
			}
			return result;
		}
		public ICollection<TreeViewNode> GetAllNodes() {
			return GetAllNodes(tree.RootNode);
		}
		#region IXafCallbackHandler Members
		public void ProcessAction(string parameter) {
			if(singleChoiceAction.Active && singleChoiceAction.Enabled) {
				ChoiceActionItem item = singleChoiceAction.FindItemByIdPath(parameter);
				if(item != null) {
					singleChoiceAction.DoExecute(item);
				}
			}
		}
		#endregion
		#region INavigationControlTestable Members
		bool INavigationControlTestable.IsItemEnabled(ChoiceActionItem item) {
			return FindNodeByActionItem(item).Enabled;
		}
		bool INavigationControlTestable.IsItemVisible(ChoiceActionItem item) {
			TreeViewNode node = FindNodeByActionItem(item);
			return node == null ? false : node.Visible;
		}
		int INavigationControlTestable.GetSubItemsCount(ChoiceActionItem item) {
			return FindNodeByActionItem(item).Nodes.Count;
		}
		string INavigationControlTestable.GetItemCaption(ChoiceActionItem item) {
			return FindNodeByActionItem(item).Text;
		}
		string INavigationControlTestable.GetItemToolTip(ChoiceActionItem item) {
			return FindNodeByActionItem(item).ToolTip;
		}
		int INavigationControlTestable.GetGroupCount() {
			return tree.Nodes.Count;
		}
		int INavigationControlTestable.GetSubGroupCount(ChoiceActionItem item) {
			TreeViewNode node;
			if(item != null) {
				node = FindNodeByActionItem(item);
			}
			else {
				node = tree.RootNode;
			}
			int result = 0;
			foreach(TreeViewNode childNode in node.Nodes) {
				if(childNode.Nodes.Count > 0) {
					result++;
				}
			}
			return result;
		}
		bool INavigationControlTestable.IsGroupExpanded(ChoiceActionItem item) {
			return FindNodeByActionItem(item).Expanded;
		}
		string INavigationControlTestable.GetSelectedItemCaption() {
			if(tree.SelectedNode != null) {
				return tree.SelectedNode.Text;
			}
			return string.Empty;
		}
		#endregion
		#region INavigationControl Members
		private ChoiceActionItemCollection actionItems;
		public void SetNavigationActionItems(ChoiceActionItemCollection actionItems, SingleChoiceAction action) {
			DevExpress.ExpressApp.Utils.Guard.ArgumentNotNull(action, "action");
			this.actionItems = actionItems;
			singleChoiceAction = action;
			singleChoiceAction.ItemsChanged += new EventHandler<ItemsChangedEventArgs>(action_ItemsChanged);
			singleChoiceAction.SelectedItemChanged += new EventHandler(action_SelectedItemChanged);
			BuildControl(action);
		}
		public ASPxTreeView Control {
			get { return tree; }
		}
		Control IWebNavigationControl.Control {
			get { return tree; }
		}
		Control IWebNavigationControl.TestControl {
			get { return tree; }
		}
		#endregion
		#region IDisposable Members
		public void Dispose() {
			ClearWrappers();
			if(singleChoiceAction != null) {
				singleChoiceAction.ItemsChanged -= new EventHandler<ItemsChangedEventArgs>(action_ItemsChanged);
				singleChoiceAction.SelectedItemChanged -= new EventHandler(action_SelectedItemChanged);
				singleChoiceAction = null;
			}
			actionItems = null;
			if(startupScriptRegisteringImpl != null) {
				startupScriptRegisteringImpl.Dispose();
				startupScriptRegisteringImpl = null;
			}
			UnsubscribeFromTreeListEvents();
			tree.Dispose();
			RegisterCallbackStartupScript = null;
			isDisposed = true;
		}
		#endregion
		#region ISupportCallbackStartupScriptRegistering Members
		public event EventHandler<RegisterCallbackStartupScriptEventArgs> RegisterCallbackStartupScript;
		#endregion
		#region IDisposableExt Members
		public bool IsDisposed {
			get { return isDisposed; }
		}
		#endregion
#if DebugTest
		public void DebugTest_CreateItems(ChoiceActionItem item, TreeViewNode node, ChoiceActionBase action) {
			CreateItems(item, node, action);
		}
		public bool DebugTest_ShowImages { get { return showImages; } }
#endif
	}
}
