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
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Templates.ActionContainers;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.Templates.ActionContainers {
	public class CreateCustomGroupControlEventArgs : EventArgs {
		private ChoiceActionItem groupItem;
		private Control control;
		public CreateCustomGroupControlEventArgs(ChoiceActionItem groupItem) {
			this.groupItem = groupItem;
		}
		public ChoiceActionItem GroupItem {
			get { return groupItem; }
		}
		public Control Control {
			get { return control; }
			set { control = value; }
		}
	}
	public class NavBarGroupChoiceActionItem : ChoiceActionItemWrapper {
		private SingleChoiceAction action;
		private NavBarGroup group;
		private bool showImages;
		public override void SetImageName(string imageName) {
			if(showImages) {
				ASPxImageHelper.SetImageProperties(group.HeaderImage, imageName);
			}
		}
		public override void SetCaption(string caption) {
			if(WebApplicationStyleManager.IsNewStyle && WebApplicationStyleManager.NavigationGroupsUpperCase && caption != null) {
				caption = caption.ToUpper();
			}
			group.Text = caption;
			group.Name = caption;
		}
		public override void SetData(object data) { }
		public override void SetShortcut(string shortcutString) { }
		public override void SetEnabled(bool enabled) { }
		public override void SetVisible(bool visible) {
			group.Visible = visible;
		}
		public override void SetToolTip(string toolTip) {
			group.ToolTip = toolTip;
		}
		public NavBarGroupChoiceActionItem(SingleChoiceAction action, ChoiceActionItem item)
			: this(action, item, true) {
		}
		public NavBarGroupChoiceActionItem(SingleChoiceAction action, ChoiceActionItem item, bool showImages)
			: base(item, action) {
			this.showImages = showImages;
			this.action = action;
			group = new NavBarGroup();
			if(action.Items.IndexOf(item) == 0) {
				group.HeaderStyle.CssClass += " FirstHeader";
				group.HeaderStyleCollapsed.CssClass = " FirstHeaderCollapsed";
			}
			if(action.Items.IndexOf(item) == this.action.Items.Count - 1) {
				group.HeaderStyle.CssClass = " LastHeader";
				group.HeaderStyleCollapsed.CssClass = " LastHeaderCollapsed";
			}
			group.Expanded = false;
			SyncronizeWithItem();
		}
		public NavBarGroup NavBarGroup {
			get { return group; }
		}
	}
	public class NavBarItemChoiceActionItem : ChoiceActionItemWrapper {
		private NavBarItem navBarItem;
		private SingleChoiceAction action;
		private bool showImages;
		public override void SetImageName(string imageName) {
			if(showImages) {
				ASPxImageHelper.SetImageProperties(navBarItem.Image, imageName);
			}
		}
		public override void SetCaption(string caption) {
			navBarItem.Text = caption;
		}
		public override void SetData(object data) { }
		public override void SetShortcut(string shortcutString) { }
		public override void SetEnabled(bool enabled) {
			navBarItem.Enabled = enabled;
			navBarItem.ClientEnabled = enabled;
		}
		public override void SetVisible(bool visible) {
			navBarItem.Visible = visible;
		}
		public override void SetToolTip(string toolTip) {
			navBarItem.ToolTip = toolTip;
		}
		public NavBarItemChoiceActionItem(SingleChoiceAction action, ChoiceActionItem item)
			: this(action, item, true) {
		}
		public NavBarItemChoiceActionItem(SingleChoiceAction action, ChoiceActionItem item, bool showImages)
			: base(item, action) {
			this.showImages = showImages;
			this.action = action;
			navBarItem = new NavBarItem();
			navBarItem.Name = item.GetIdPath();
			SyncronizeWithItem();
		}
		public NavBarItem NavBarItem {
			get { return navBarItem; }
		}
		public void ExecuteAction() {
			if(action.Active && action.Enabled) {
				action.DoExecute(ActionItem);
			}
		}
	}
	public class NavBarTreeViewItem : NavBarItem, IDisposable {
		private TreeViewNavigationControl treeViewNavigationControl;
		public NavBarTreeViewItem(TreeViewNavigationControl treeViewNavigationControl) {
			this.treeViewNavigationControl = treeViewNavigationControl;
			Template = new TreeViewItemTemplate(treeViewNavigationControl);
		}
		public TreeViewNavigationControl TreeViewNavigationControl {
			get {
				return treeViewNavigationControl;
			}
		}
		#region IDisposable Members
		public void Dispose() {
			treeViewNavigationControl.Dispose();
		}
		#endregion
		public class TreeViewItemTemplate : ITemplate {
			TreeViewNavigationControl treeViewNavigationControl;
			public TreeViewItemTemplate(TreeViewNavigationControl treeViewNavigationControl) {
				this.treeViewNavigationControl = treeViewNavigationControl;
			}
			public void InstantiateIn(Control container) {
				container.Controls.Add((WebControl)treeViewNavigationControl.Control);
			}
		}
	}
	internal class NavBarCustomControlItem : NavBarItem, IDisposable {
		private Control control;
		public NavBarCustomControlItem(Control control) {
			this.control = control;
			Template = new NavBarCustomGroupControlTemplate(control);
		}
		#region IDisposable Members
		public void Dispose() {
			if(control is IDisposable) {
				((IDisposable)control).Dispose();
			}
		}
		#endregion
		internal class NavBarCustomGroupControlTemplate : ITemplate {
			private Control control;
			public NavBarCustomGroupControlTemplate(Control control) {
				this.control = control;
			}
			#region ITemplate Members
			public void InstantiateIn(Control container) {
				container.Controls.Add(control);
			}
			#endregion
		}
	}
	public class NavBarNavigationControl : IWebNavigationControl, INavigationControlTestable, IDisposable, ISupportCallbackStartupScriptRegistering, IXafCallbackHandler, ISupportAdditionalParametersTestControl {
		private ASPxNavBar navBar;
		private Dictionary<ChoiceActionItem, NavBarItem> actionItemToNavBarItemMap;
		private LightDictionary<ChoiceActionItem, NavBarGroup> actionItemToNavBarGroupMap;
		private Dictionary<ChoiceActionItem, TreeViewNavigationControl> actionItemToTreeNavigationContainerMap;
		private Dictionary<TreeViewNavigationControl, NavBarGroup> treeNavigationContainerToNavBarGroupMap;
		private Dictionary<NavBarItem, NavBarItemChoiceActionItem> navBarItemToWrapperMap;
		private List<NavBarGroupChoiceActionItem> groupWrappers;
		private ChoiceActionItemCollection actionItems;
		private bool showImages;
		internal SingleChoiceAction singleChoiceAction;
		private void FillGroup(NavBarGroup group, ChoiceActionItem groupValue) {
			foreach(ChoiceActionItem itemValue in groupValue.Items) {
				if(!itemValue.Active) {
					continue;
				}
				NavBarItemChoiceActionItem itemWrapper = new NavBarItemChoiceActionItem(singleChoiceAction, itemValue, showImages);
				navBarItemToWrapperMap.Add(itemWrapper.NavBarItem, itemWrapper);
				group.Items.Add(itemWrapper.NavBarItem);
				actionItemToNavBarItemMap.Add(itemValue, itemWrapper.NavBarItem);
			}
		}
		private void FillGroupByTree(NavBarGroup group, ChoiceActionItem groupValue, SingleChoiceAction singleChoiceAction) {
			TreeViewNavigationControl tree = new TreeViewNavigationControl(showImages);
			treeNavigationContainerToNavBarGroupMap.Add(tree, group);
			tree.SetNavigationActionItems(groupValue.Items, singleChoiceAction);
			group.Items.Add(new NavBarTreeViewItem(tree));
			foreach(TreeViewNode node in tree.GetAllNodes()) {
				actionItemToTreeNavigationContainerMap.Add(tree.FindActionItemByNode(node), tree);
			}
		}
		private void BuildControl(ASPxNavBar navBar, ChoiceActionItemCollection actionItems, SingleChoiceAction singleChoiceAction) {
			UnsubscribeFromTreeViews();
			actionItemToNavBarItemMap.Clear();
			actionItemToNavBarGroupMap.Clear();
			actionItemToTreeNavigationContainerMap.Clear();
			treeNavigationContainerToNavBarGroupMap.Clear();
			ClearItemWrappers();
			navBar.Groups.Clear();
			if(actionItems.Count > 0) {
				foreach(ChoiceActionItem groupValue in actionItems) {
					if(!groupValue.Active) {
						continue;
					}
					NavBarGroupChoiceActionItem groupItem = new NavBarGroupChoiceActionItem(singleChoiceAction, groupValue, showImages);
					groupWrappers.Add(groupItem);
					NavBarGroup group = groupItem.NavBarGroup;
					actionItemToNavBarGroupMap.Add(groupValue, group);
					navBar.Groups.Add(group);
					ItemsDisplayStyle itemsDisplayStyle = ItemsDisplayStyle.LargeIcons;
					if(groupValue.Model != null) {
						itemsDisplayStyle = ItemsDisplayStyle.List;
						if(groupValue.Model is IModelChoiceActionItemChildItemsDisplayStyle) {
							itemsDisplayStyle = ((IModelChoiceActionItemChildItemsDisplayStyle)groupValue.Model).ChildItemsDisplayStyle;
						}
					}
					CreateCustomGroupControlEventArgs args = new CreateCustomGroupControlEventArgs(groupValue);
					OnCreateCustomGroupControl(args);
					if(args.Control != null) {
						group.Items.Add(new NavBarCustomControlItem(args.Control));
					}
					else {
						switch(itemsDisplayStyle) {
							case ItemsDisplayStyle.LargeIcons:
							case ItemsDisplayStyle.List:
								if(groupValue.IsHierarchical()) {
									FillGroupByTree(group, groupValue, singleChoiceAction);
								}
								else {
									FillGroup(group, groupValue);
								}
								break;
						}
					}
				}
			}
			SubscribeToTreeViews();
			UpdateSelection();
		}
		private void SubscribeToTreeViews() {
			foreach(ISupportCallbackStartupScriptRegistering treeView in treeNavigationContainerToNavBarGroupMap.Keys) {
				treeView.RegisterCallbackStartupScript += new EventHandler<RegisterCallbackStartupScriptEventArgs>(treeView_RegisterCallbackStartupScript);
			}
		}
		private void UnsubscribeFromTreeViews() {
			foreach(TreeViewNavigationControl treeView in treeNavigationContainerToNavBarGroupMap.Keys) {
				treeView.RegisterCallbackStartupScript -= new EventHandler<RegisterCallbackStartupScriptEventArgs>(treeView_RegisterCallbackStartupScript);
				treeView.Dispose();
			}
		}
		private void treeView_RegisterCallbackStartupScript(object sender, RegisterCallbackStartupScriptEventArgs e) {
			OnRegisterCallbackStartupScript(e);
		}
		private void singleChoiceAction_ItemsChanged(object sender, ItemsChangedEventArgs e) {
			BuildControl(navBar, actionItems,singleChoiceAction);
		}
		private void UpdateSelection() {
			navBar.SelectedItem = null;
			if(singleChoiceAction != null && singleChoiceAction.SelectedItem != null) {
				ChoiceActionItem actionItem = singleChoiceAction.SelectedItem;
				if(actionItemToNavBarItemMap.ContainsKey(actionItem)) {
					NavBarItem itemLink = actionItemToNavBarItemMap[actionItem];
					itemLink.Group.Expanded = true;
					navBar.SelectedItem = itemLink;
				}
				else if(actionItemToTreeNavigationContainerMap.ContainsKey(actionItem)) {
					TreeViewNavigationControl tree = actionItemToTreeNavigationContainerMap[actionItem];
					treeNavigationContainerToNavBarGroupMap[tree].Expanded = true;
				}
			}
		}
		private void action_SelectedItemChanged(object sender, EventArgs e) {
			UpdateSelection();
		}
		private void ASPxNavBarActionContainer_ItemClick(object source, NavBarItemEventArgs e) {
			if(e.Item != null && navBarItemToWrapperMap.ContainsKey(e.Item)) {
				navBarItemToWrapperMap[e.Item].ExecuteAction();
			}
		}
		private void SubscribeToAction() {
			singleChoiceAction.ItemsChanged += new EventHandler<ItemsChangedEventArgs>(singleChoiceAction_ItemsChanged);
			singleChoiceAction.SelectedItemChanged += new EventHandler(action_SelectedItemChanged);
		}
		private void UnsubscribeFromAction() {
			if(singleChoiceAction != null) {
				singleChoiceAction.SelectedItemChanged -= new EventHandler(action_SelectedItemChanged);
				singleChoiceAction.ItemsChanged -= new EventHandler<ItemsChangedEventArgs>(singleChoiceAction_ItemsChanged);
			}
		}
		private void UnsubscribeAll() {
			UnsubscribeFromTreeViews();
			UnsubscribeFromAction();
			singleChoiceAction = null;
			actionItemToNavBarItemMap.Clear();
			actionItemToNavBarGroupMap.Clear();
			actionItemToTreeNavigationContainerMap.Clear();
			treeNavigationContainerToNavBarGroupMap.Clear();
		}
		private NavBarGroup FindGroupControl(ChoiceActionItem item) {
			if(item != null && actionItemToNavBarGroupMap.ContainsKey(item)) {
				return actionItemToNavBarGroupMap[item];
			}
			return null;
		}
		private void ClearItemWrappers() {
			foreach(NavBarItemChoiceActionItem itemWrapper in navBarItemToWrapperMap.Values) {
				itemWrapper.Dispose();
			}
			navBarItemToWrapperMap.Clear();
			foreach(ChoiceActionItemWrapper groupWrapper in groupWrappers) {
				groupWrapper.Dispose();
			}
			groupWrappers.Clear();
		}
		protected virtual void OnCreateCustomGroupControl(CreateCustomGroupControlEventArgs args) {
			if(CreateCustomGroupControl != null) {
				CreateCustomGroupControl(this, args);
			}
		}
		protected virtual void OnRegisterCallbackStartupScript(RegisterCallbackStartupScriptEventArgs e) {
			if(RegisterCallbackStartupScript != null) {
				RegisterCallbackStartupScript(this, e);
			}
		}
		public NavBarNavigationControl(bool showImages)  {
			this.showImages = showImages;
			navBar = RenderHelper.CreateASPxNavBar();
			navBar.ActiveGroup = null;
			navBar.AllowSelectItem = true;
			if(WebApplicationStyleManager.IsNewStyle) {
				navBar.ShowExpandButtons = false;
			}
			navBar.GroupSpacing = System.Web.UI.WebControls.Unit.Pixel(0);
			navBar.Border.BorderStyle = System.Web.UI.WebControls.BorderStyle.None;
			navBar.ID = "NB";
			navBar.ItemClick += new NavBarItemEventHandler(ASPxNavBarActionContainer_ItemClick);
			navBar.Load += new EventHandler(navBar_Load);
			navBar.CustomJSProperties += navBar_CustomJSProperties;
			actionItemToNavBarItemMap = new Dictionary<ChoiceActionItem, NavBarItem>();
			actionItemToNavBarGroupMap = new LightDictionary<ChoiceActionItem, NavBarGroup>();
			actionItemToTreeNavigationContainerMap = new Dictionary<ChoiceActionItem, TreeViewNavigationControl>();
			treeNavigationContainerToNavBarGroupMap = new Dictionary<TreeViewNavigationControl, NavBarGroup>();
			navBarItemToWrapperMap = new Dictionary<NavBarItem, NavBarItemChoiceActionItem>();
			groupWrappers = new List<NavBarGroupChoiceActionItem>();
		}
		public NavBarNavigationControl()
			: this(true) {			
		}
		void navBar_CustomJSProperties(object sender, CustomJSPropertiesEventArgs e) {
			List<NavigationNodePath> paths = new List<NavigationNodePath>();
			foreach(KeyValuePair<TreeViewNavigationControl, NavBarGroup> pair in treeNavigationContainerToNavBarGroupMap) {
				paths.AddRange(pair.Key.GetAllPaths().ConvertAll(entry => { entry.NavGroupCaption = pair.Value.Text; return entry; }));
			}
			e.Properties["cpEntriesAsJSON"] = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(paths.ToArray());
		}
		void navBar_Load(object sender, EventArgs e) {
			ICallbackManagerHolder holder = navBar.Page as ICallbackManagerHolder;
			if(holder != null) {
				holder.CallbackManager.RegisterHandler(navBar.UniqueID, this);
				navBar.ClientSideEvents.Init = @"function(s, e){
                    if(typeof xaf != 'undefined' && xaf.ConfirmUnsavedChangedController){
                        xaf.ConfirmUnsavedChangedController.CustomizeNavBar(s);
                    }
                }";
				navBar.ClientSideEvents.ItemClick =
				  @"function(s,e) {
                        e.processOnServer = false;
					    if(typeof xaf == 'undefined' || !xaf.ConfirmUnsavedChangedController || xaf.ConfirmUnsavedChangedController.CanProcessCallbackForNavigation(s,e)){" +
							holder.CallbackManager.GetScript(navBar.UniqueID, "e.item.name", String.Empty, singleChoiceAction.Model.GetValue<bool>("IsPostBackRequired")) +
					  @"}
                    }";
			}
		}
		public event EventHandler<CreateCustomGroupControlEventArgs> CreateCustomGroupControl;
		#region INavigationControl
		public void SetNavigationActionItems(ChoiceActionItemCollection actionItems, SingleChoiceAction action) {
			Guard.ArgumentNotNull(action, "action");
			UnsubscribeFromAction();
			this.actionItems = actionItems;
			singleChoiceAction = action;
			BuildControl(navBar, actionItems, action);
			SubscribeToAction();
		}
		public Control Control {
			get { return navBar; }
		}
		public Control TestControl {
			get { return navBar; }
		}
		#endregion
		#region ISupportNavigationActionContainerTesting
		public bool IsItemControlVisible(ChoiceActionItem item) {
			bool result = false;
			if(actionItemToNavBarGroupMap[item] != null) {
				result = actionItemToNavBarGroupMap[item].Visible;
			}
			return result;
		}
		public int GetGroupCount() {
			return navBar.Groups.Count;
		}
		public string GetGroupControlCaption(ChoiceActionItem item) {
			if(actionItemToNavBarGroupMap[item] != null) {
				return actionItemToNavBarGroupMap[item].Text;
			}
			throw new ArgumentOutOfRangeException();
		}
		public int GetGroupChildControlCount(ChoiceActionItem item) {
			if(actionItemToNavBarGroupMap[item] != null) {
				return actionItemToNavBarGroupMap[item].Items.Count;
			}
			throw new ArgumentOutOfRangeException();
		}
		public string GetChildControlCaption(ChoiceActionItem item) {
			if(actionItemToNavBarItemMap[item] != null) {
				return actionItemToNavBarItemMap[item].Text;
			}
			throw new ArgumentOutOfRangeException();
		}
		public bool GetChildControlEnabled(ChoiceActionItem item) {
			if(actionItemToNavBarItemMap[item] != null) {
				return actionItemToNavBarItemMap[item].ClientEnabled;
			}
			throw new ArgumentOutOfRangeException();
		}
		public bool GetChildControlVisible(ChoiceActionItem item) {
			if(actionItemToNavBarItemMap[item] != null) {
				return actionItemToNavBarItemMap[item].Visible;
			}
			throw new ArgumentOutOfRangeException();
		}
		public bool IsGroupExpanded(ChoiceActionItem item) {
			if(actionItemToNavBarGroupMap[item] != null) {
				return actionItemToNavBarGroupMap[item].Expanded;
			}
			throw new ArgumentOutOfRangeException();
		}
		public string GetSelectedItemCaption() {
			if(navBar.SelectedItem != null) {
				return navBar.SelectedItem.Text;
			}
			foreach(TreeViewNavigationControl treeControl in treeNavigationContainerToNavBarGroupMap.Keys) {
				ASPxTreeView tree = (ASPxTreeView)treeControl.Control;
				return tree.SelectedNode.Text;
			}
			return string.Empty;
		}
		#endregion
		#region ISupportNavigationActionContainerTesting
		#endregion
		#region INavigationControlTestable Members
		bool INavigationControlTestable.IsItemEnabled(ChoiceActionItem item) {
			if(actionItemToNavBarItemMap.ContainsKey(item)) {
				return actionItemToNavBarItemMap[item].Enabled;
			}
			if(actionItemToTreeNavigationContainerMap.ContainsKey(item)) {
				return ((INavigationControlTestable)actionItemToTreeNavigationContainerMap[item]).IsItemEnabled(item);
			}
			if(actionItemToNavBarGroupMap.ContainsKey(item)) {
				return actionItemToNavBarGroupMap[item].Visible;
			}
			return false;
		}
		bool INavigationControlTestable.IsItemVisible(ChoiceActionItem item) {
			if(actionItemToNavBarItemMap.ContainsKey(item)) {
				return actionItemToNavBarItemMap[item].Visible;
			}
			if(actionItemToTreeNavigationContainerMap.ContainsKey(item)) {
				return ((INavigationControlTestable)actionItemToTreeNavigationContainerMap[item]).IsItemVisible(item);
			}
			if(actionItemToNavBarGroupMap.ContainsKey(item)) {
				return actionItemToNavBarGroupMap[item].Visible;
			}
			return false;
		}
		int INavigationControlTestable.GetSubItemsCount(ChoiceActionItem item) {
			if(actionItemToNavBarItemMap.ContainsKey(item)) {
				return 0;
			}
			if(actionItemToTreeNavigationContainerMap.ContainsKey(item)) {
				return ((INavigationControlTestable)actionItemToTreeNavigationContainerMap[item]).GetSubItemsCount(item);
			}
			if(actionItemToNavBarGroupMap.ContainsKey(item)) {
				NavBarGroup group = actionItemToNavBarGroupMap[item];
				if(group.Items.Count > 0 && group.Items[0] is NavBarTreeViewItem) {
					TreeViewNavigationControl tree = ((NavBarTreeViewItem)group.Items[0]).TreeViewNavigationControl;
					if(tree != null) {
						return tree.GetAllNodes().Count;
					}
				}
				else {
					return actionItemToNavBarGroupMap[item].Items.Count;
				}
			}
			return 0;
		}
		string INavigationControlTestable.GetItemCaption(ChoiceActionItem item) {
			if(actionItemToNavBarItemMap.ContainsKey(item)) {
				return actionItemToNavBarItemMap[item].Text;
			}
			if(actionItemToTreeNavigationContainerMap.ContainsKey(item)) {
				return ((INavigationControlTestable)actionItemToTreeNavigationContainerMap[item]).GetItemCaption(item);
			}
			if(actionItemToNavBarGroupMap.ContainsKey(item)) {
				return actionItemToNavBarGroupMap[item].Text;
			}
			return string.Empty;
		}
		string INavigationControlTestable.GetItemToolTip(ChoiceActionItem item) {
			if(actionItemToNavBarItemMap.ContainsKey(item)) {
				return actionItemToNavBarItemMap[item].ToolTip;
			}
			if(actionItemToTreeNavigationContainerMap.ContainsKey(item)) {
				return ((INavigationControlTestable)actionItemToTreeNavigationContainerMap[item]).GetItemToolTip(item);
			}
			if(actionItemToNavBarGroupMap.ContainsKey(item)) {
				return actionItemToNavBarGroupMap[item].ToolTip;
			}
			return string.Empty;
		}
		int INavigationControlTestable.GetGroupCount() {
			return navBar.Groups.Count;
		}
		int INavigationControlTestable.GetSubGroupCount(ChoiceActionItem item) {
			if(actionItemToTreeNavigationContainerMap.ContainsKey(item)) {
				return ((INavigationControlTestable)actionItemToTreeNavigationContainerMap[item]).GetSubGroupCount(item);
			}
			NavBarGroup group = FindGroupControl(item);
			if(group != null && group.Items.Count > 0 && group.Items[0] is NavBarTreeViewItem) {
				INavigationControlTestable tree = ((NavBarTreeViewItem)group.Items[0]).TreeViewNavigationControl;
				return tree.GetSubGroupCount(null);
			}
			return 0;
		}
		bool INavigationControlTestable.IsGroupExpanded(ChoiceActionItem item) {
			if(actionItemToNavBarGroupMap.ContainsKey(item)) {
				return actionItemToNavBarGroupMap[item].Expanded;
			}
			if(actionItemToTreeNavigationContainerMap.ContainsKey(item)) {
				return actionItemToTreeNavigationContainerMap[item].FindNodeByActionItem(item).Expanded;
			}
			return false;
		}
		string INavigationControlTestable.GetSelectedItemCaption() {
			if(navBar.SelectedItem != null) {
				return navBar.SelectedItem.Text;
			}
			foreach(TreeViewNavigationControl treeNavigationControl in treeNavigationContainerToNavBarGroupMap.Keys) {
				ASPxTreeView tree = (ASPxTreeView)treeNavigationControl.Control;
				if(tree.SelectedNode != null) {
					return tree.SelectedNode.Text;
				}
			}
			return string.Empty;
		}
		#endregion
		#region IDisposable Members
		public void Dispose() {
			navBar.ItemClick -= new NavBarItemEventHandler(ASPxNavBarActionContainer_ItemClick);
			navBar.Load -= new EventHandler(navBar_Load);
			navBar.CustomJSProperties -= navBar_CustomJSProperties;
			ClearItemWrappers();
			UnsubscribeAll();
			foreach(NavBarGroup group in navBar.Groups) {
				foreach(NavBarItem item in group.Items) {
					if(item is IDisposable) {
						((IDisposable)item).Dispose();
					}
				}
				if(group is IDisposable) {
					((IDisposable)group).Dispose();
				}
			}
			RegisterCallbackStartupScript = null;
		}
		#endregion
		#region ISupportCallbackStartupScriptRegistering Members
		public event EventHandler<RegisterCallbackStartupScriptEventArgs> RegisterCallbackStartupScript;
		#endregion
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
		#region ISupportAdditionalParametersTestControl Members
		public ICollection<string> GetAdditionalParameters(object control) {
			Dictionary<string, string> groupToTreeViewMap = new Dictionary<string, string>();
			foreach(KeyValuePair<TreeViewNavigationControl, NavBarGroup> pair in treeNavigationContainerToNavBarGroupMap) {
				ASPxTreeView treeView = ((ASPxTreeView)pair.Key.Control);
				groupToTreeViewMap.Add(pair.Value.Text, treeView.ClientID);
			}
			string cpGroupToTreeViewMap = EasyTestTagHelper.FormatDictionary(groupToTreeViewMap);
			return new string[] { string.Format("'{0}'", cpGroupToTreeViewMap) };
		}
		#endregion
#if DebugTest
		public void DebugTest_FillGroup(NavBarGroup group, ChoiceActionItem groupValue) {
			FillGroup(group, groupValue);
		}
		public void DebugTest_FillGroupByTree(NavBarGroup group, ChoiceActionItem groupValue, SingleChoiceAction singleChoiceAction) {
			FillGroupByTree(group, groupValue, singleChoiceAction);
		}
		public void DebugTests_BuildControl(ASPxNavBar navBar, ChoiceActionItemCollection actionItems, SingleChoiceAction action) {
			BuildControl(navBar, actionItems, action);
		}
		public bool DebugTest_ShowImages { get { return showImages; } }
#endif
	}
}
