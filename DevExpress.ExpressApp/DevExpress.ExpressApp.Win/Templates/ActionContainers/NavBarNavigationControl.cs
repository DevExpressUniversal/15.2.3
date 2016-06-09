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
using System.Windows.Forms;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Templates.ActionContainers;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Win.Utils;
using DevExpress.XtraNavBar;
using DevExpress.XtraNavBar.ViewInfo;
using DevExpress.XtraTreeList.Nodes;
namespace DevExpress.ExpressApp.Win.Templates.ActionContainers {
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
		private NavBarGroup navBarGroup;
		public override void SetImageName(string imageName) {
			ImageInfo imageInfo = ImageLoader.Instance.GetImageInfo(imageName);
			if(!imageInfo.IsEmpty) {
				navBarGroup.SmallImage = imageInfo.Image;
			}
		}
		public override void SetCaption(string caption) {
			navBarGroup.Caption = caption;
		}
		public override void SetData(object data) {
		}
		public override void SetShortcut(string shortcutString) {
		}
		public override void SetEnabled(bool enabled) {
		}
		public override void SetVisible(bool visible) {
			navBarGroup.Visible = visible;
		}
		public override void SetToolTip(string toolTip) {
			navBarGroup.Hint = toolTip;
		}
		public NavBarGroupChoiceActionItem(ChoiceActionItem item, ChoiceActionBase action)
			: base(item, action) {
			navBarGroup = new NavBarGroup();
			navBarGroup.Name = item.Id;
			SyncronizeWithItem();
		}
		public NavBarGroup NavBarGroup {
			get { return navBarGroup; }
		}
	}
	public class NavBarItemChoiceActionItem : ChoiceActionItemWrapper {
		private NavBarItem navBarItem;
		private SingleChoiceAction action;
		public override void SetImageName(string imageName) {
			if(!string.IsNullOrEmpty(imageName)) {
				ImageInfo smallImageInfo = ImageLoader.Instance.GetImageInfo(imageName);
				ImageInfo largeImageInfo = ImageLoader.Instance.GetLargeImageInfo(imageName);
				navBarItem.SmallImage = smallImageInfo.Image;
				navBarItem.LargeImage = largeImageInfo.Image;
				if(largeImageInfo.IsEmpty) {
					navBarItem.LargeImage = navBarItem.SmallImage;
				}
				if(smallImageInfo.IsEmpty) {
					navBarItem.SmallImage = navBarItem.LargeImage;
				}
			}
		}
		public override void SetCaption(string caption) {
			navBarItem.Caption = caption;
		}
		public override void SetData(object data) {
		}
		public override void SetShortcut(string shortcutString) {
		}
		public override void SetEnabled(bool enabled) {
			navBarItem.Enabled = enabled;
		}
		public override void SetVisible(bool visible) {
			navBarItem.Visible = visible;
		}
		public override void SetToolTip(string toolTip) {
			navBarItem.Hint = toolTip;
		}
		public NavBarItemChoiceActionItem(SingleChoiceAction action, ChoiceActionItem actionItem)
			: base(actionItem, action) {
			Guard.ArgumentNotNull(actionItem, "actionItem");
			this.action = action;
			navBarItem = new NavBarItem();
			SyncronizeWithItem();
		}
		public void ExecuteAction() {
			if(action.Active && action.Enabled) {
				action.DoExecute(ActionItem);
			}
		}
		public NavBarItem NavBarItem {
			get { return navBarItem; }
		}
	}
	public class NavBarActionGroup : NavBarGroup {
		private ChoiceActionItem item;
		public NavBarActionGroup(ChoiceActionItem item)
			: base(item.Caption) {
			Guard.ArgumentNotNull(item, "item");
			this.item = item;
			this.Visible = item.Active;
		}
		protected override void Dispose(bool disposing) {
			item = null;
			base.Dispose(disposing);
		}
		public ChoiceActionItem Item {
			get { return item; }
		}
	}
	public class NavBarActionItem : NavBarItem {
		private SingleChoiceAction action;
		private ChoiceActionItem actionItem;
		private void UpdateState() {
			if(actionItem != null) {
				Enabled = actionItem.Enabled;
				Visible = actionItem.Active;
				Caption = actionItem.Caption;
				SetImage();
			}
		}
		private void SetImage() {
			if(!string.IsNullOrEmpty(actionItem.ImageName)) {
				LargeImage = ImageLoader.Instance.GetLargeImageInfo(actionItem.ImageName).Image;
				SmallImage = ImageLoader.Instance.GetImageInfo(actionItem.ImageName).Image;
			}
		}
		private void action_ItemsChanged(object sender, ItemsChangedEventArgs e) {
			if(e.ChangedItemsInfo.ContainsKey(actionItem)) {
				UpdateState();
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing && actionItem != null) {
				action.ItemsChanged -= new EventHandler<ItemsChangedEventArgs>(action_ItemsChanged);
				actionItem = null;
			}
			base.Dispose(disposing);
		}
		public NavBarActionItem(SingleChoiceAction action, ChoiceActionItem actionItem)
			: base() {
			Guard.ArgumentNotNull(actionItem, "actionItem");
			this.action = action;
			this.actionItem = actionItem;
			UpdateState();
			action.ItemsChanged += new EventHandler<ItemsChangedEventArgs>(action_ItemsChanged);
		}
		public void ExecuteAction() {
			if(action.Active && action.Enabled) {
				action.DoExecute(actionItem);
			}
		}
		public ChoiceActionItem ActionItem {
			get { return actionItem; }
		}
	}
	[ToolboxItem(false)]
	public class NavBarNavigationControl : NavBarControl, INavigationControl, INavigationControlTestable, ISupportUpdate {
		private static int repeatDelay = SystemInformation.DoubleClickTime;
		private Dictionary<ChoiceActionItem, NavBarItemLink> actionItemToItemLinkMap;
		private Dictionary<ChoiceActionItem, NavBarGroup> actionItemToNavBarGroupMap;
		private Dictionary<ChoiceActionItem, TreeListNavigationControl> actionItemToTreeNavigationContainerMap;
		private Dictionary<TreeListNavigationControl, NavBarGroup> treeNavigationContainerToNavBarGroupMap;
		private Dictionary<NavBarItemLink, NavBarItemChoiceActionItem> itemLinkToActionItemWrapperMap;
		private Dictionary<NavBarGroup, NavBarGroupChoiceActionItem> groupToActionItemWrapperMap;
		private NavBarGroupStyle groupStyle = NavBarGroupStyle.LargeIconsText;
		private bool isSelectionUpdating;
		private Int32 lockCount;
		private bool rebuildRequested;
		private ChoiceActionItemCollection actionItems;
		private TimeLatch linkClickedEnabler = new TimeLatch();
		protected SingleChoiceAction singleChoiceAction;
		private void Lock() {
			lockCount++;
		}
		private void Unlock() {
			if(lockCount > 0) {
				lockCount--;
			}
			if(lockCount == 0 && rebuildRequested) {
				BuildControl();
			}
		}
		private void BuildControl() {
			if(lockCount != 0) {
				rebuildRequested = true;
			}
			else{
				rebuildRequested = false;
				BeginUpdate();
				try {
					ClearMaps();
					Items.Clear();
					Groups.Clear();
					if(actionItems.Count > 0) {
						foreach(ChoiceActionItem groupActionItem in actionItems) {
							if(!groupActionItem.Active) {
								continue;
							}
							NavBarGroupChoiceActionItem groupItem = new NavBarGroupChoiceActionItem(groupActionItem, singleChoiceAction);
							NavBarGroup group = groupItem.NavBarGroup;
							actionItemToNavBarGroupMap.Add(groupActionItem, group);
							groupToActionItemWrapperMap.Add(group, groupItem);
							Groups.Add(group);
							CreateCustomGroupControlEventArgs args = new CreateCustomGroupControlEventArgs(groupActionItem);
							OnCreateCustomGroupControl(args);
							if(args.Control != null) {
								group.GroupStyle = NavBarGroupStyle.ControlContainer;
								group.ControlContainer.Controls.Add(args.Control);
							}
							else {
								ItemsDisplayStyle itemsDisplayStyle = ItemsDisplayStyle.LargeIcons;
								if(groupActionItem.Model is IModelChoiceActionItemChildItemsDisplayStyle) {
									itemsDisplayStyle = ((IModelChoiceActionItemChildItemsDisplayStyle)groupActionItem.Model).ChildItemsDisplayStyle;
								}
								switch(itemsDisplayStyle) {
									case ItemsDisplayStyle.List:
										if(groupActionItem.IsHierarchical()) {
											group.GroupStyle = NavBarGroupStyle.ControlContainer;
											FillGroupByTree(groupItem);
										}
										else {
											group.GroupStyle = NavBarGroupStyle.SmallIconsText;
											FillGroup(groupItem);
										}
										break;
									case ItemsDisplayStyle.LargeIcons:
										group.GroupStyle = GroupStyle;
										FillGroup(groupItem);
										break;
								}
							}
						}
					}
					UpdateSelection();
				}
				finally {
					EndUpdate();
				}
			}
		}
		private void ClearMaps() {
			actionItemToItemLinkMap.Clear();
			actionItemToNavBarGroupMap.Clear();
			actionItemToTreeNavigationContainerMap.Clear();
			foreach(TreeListNavigationControl control in treeNavigationContainerToNavBarGroupMap.Keys) {
				control.Dispose();
			}
			treeNavigationContainerToNavBarGroupMap.Clear();
			foreach(NavBarItemChoiceActionItem item in itemLinkToActionItemWrapperMap.Values) {
				item.Dispose();
			}
			itemLinkToActionItemWrapperMap.Clear();
			foreach(NavBarGroupChoiceActionItem item in groupToActionItemWrapperMap.Values) {
				item.Dispose();
			}
			groupToActionItemWrapperMap.Clear();
		}
		private void FillGroup(NavBarGroupChoiceActionItem groupItem) {
			foreach(ChoiceActionItem itemValue in groupItem.ActionItem.Items) {
				if(!itemValue.Active) {
					continue;
				}
				NavBarItemChoiceActionItem item = new NavBarItemChoiceActionItem(singleChoiceAction, itemValue);
				Items.Add(item.NavBarItem);
				NavBarItemLink link = groupItem.NavBarGroup.ItemLinks.Add(item.NavBarItem);
				actionItemToItemLinkMap.Add(itemValue, link);
				itemLinkToActionItemWrapperMap.Add(link, item);
			}
		}
		private void FillGroupByTree(NavBarGroupChoiceActionItem groupItem) {
			TreeListNavigationControl tree = new TreeListNavigationControl();
			treeNavigationContainerToNavBarGroupMap.Add(tree, groupItem.NavBarGroup);
			tree.SetNavigationActionItems(groupItem.ActionItem.Items, singleChoiceAction);
			tree.Dock = DockStyle.Fill;
			groupItem.NavBarGroup.ControlContainer.Controls.Add(tree);
			foreach(TreeListNode node in tree.GetAllNodes()) {
				actionItemToTreeNavigationContainerMap.Add(((ChoiceActionItem)node.Tag), tree);
			}
		}
		private void UpdateSelection() {
			if(isSelectionUpdating) return;
			isSelectionUpdating = true;
			try {
				NavBarItemLink selectedLink = null;
				ChoiceActionItem actionItem = singleChoiceAction.SelectedItem;
				if(actionItem != null) {
					if(actionItemToItemLinkMap.ContainsKey(actionItem)) {
						selectedLink = actionItemToItemLinkMap[actionItem];
						selectedLink.Group.Expanded = true;
					}
					else if(actionItemToTreeNavigationContainerMap.ContainsKey(actionItem)) {
						TreeListNavigationControl tree = actionItemToTreeNavigationContainerMap[actionItem];
						treeNavigationContainerToNavBarGroupMap[tree].Expanded = true;
					}
					SelectedLink = selectedLink;
				}
			}
			finally {
				isSelectionUpdating = false;
			}
		}
		private void UnsubscribeFromActionEvents() {
			if(singleChoiceAction != null) {
				singleChoiceAction.ItemsChanged -= new EventHandler<ItemsChangedEventArgs>(singleChoiceAction_ItemsChanged);
				singleChoiceAction.SelectedItemChanged -= new EventHandler(singleChoiceAction_SelectedItemChanged);
				singleChoiceAction.ExecuteCanceled -= new EventHandler<ActionBaseEventArgs>(singleChoiceAction_ExecuteCanceled);
				singleChoiceAction.ExecuteCompleted -= new EventHandler<ActionBaseEventArgs>(singleChoiceAction_ExecuteCompleted);
				singleChoiceAction = null;
			}
		}
		private void SubscribeToActionEvents() {
			singleChoiceAction.ItemsChanged += new EventHandler<ItemsChangedEventArgs>(singleChoiceAction_ItemsChanged);
			singleChoiceAction.SelectedItemChanged += new EventHandler(singleChoiceAction_SelectedItemChanged);
			singleChoiceAction.ExecuteCanceled += new EventHandler<ActionBaseEventArgs>(singleChoiceAction_ExecuteCanceled);
			singleChoiceAction.ExecuteCompleted += new EventHandler<ActionBaseEventArgs>(singleChoiceAction_ExecuteCompleted);
		}
		private void singleChoiceAction_ItemsChanged(object sender, ItemsChangedEventArgs e) {
			if(ShouldRebuildItems(e.ChangedItemsInfo)) {
				BuildControl();
			}
		}
		private bool ShouldRebuildItems(IDictionary<object, ChoiceActionItemChangesType> itemsChangedInfo) {
			foreach(ChoiceActionItemChangesType changesType in itemsChangedInfo.Values) {
				if(
					ChangesTypeContains(changesType, ChoiceActionItemChangesType.Add)
					|| ChangesTypeContains(changesType, ChoiceActionItemChangesType.Remove)
					|| ChangesTypeContains(changesType, ChoiceActionItemChangesType.ItemsAdd)
					|| ChangesTypeContains(changesType, ChoiceActionItemChangesType.ItemsRemove)
					|| ChangesTypeContains(changesType, ChoiceActionItemChangesType.Active)
					|| ChangesTypeContains(changesType, ChoiceActionItemChangesType.Enabled)
				) {
					return true;
				}
			}
			return false;
		}
		private bool ChangesTypeContains(ChoiceActionItemChangesType changesType, ChoiceActionItemChangesType toCheck) {
			return (changesType & toCheck) == toCheck;
		}
		private void singleChoiceAction_SelectedItemChanged(object sender, EventArgs e) {
			Lock();
			BeginUpdate();
			try {
				UpdateSelection();
			}
			finally {
				EndUpdate();
				Unlock();
			}
		}
		private void singleChoiceAction_ExecuteCanceled(object sender, ActionBaseEventArgs e) {
			linkClickedEnabler.ResetLastEventTime();
		}
		private void singleChoiceAction_ExecuteCompleted(object sender, ActionBaseEventArgs e) {
			linkClickedEnabler.ResetLastEventTime();
		}
		private void ProcessLinkAction(NavBarItemLink link) {
			if(link != null && itemLinkToActionItemWrapperMap.ContainsKey(link)) {
				ExecuteAction(itemLinkToActionItemWrapperMap[link]);
			}
		}
		private void ExecuteAction(NavBarItemChoiceActionItem item) {
			if(item != null && lockCount == 0) {
				Lock();
				try {
					if(linkClickedEnabler.IsTimeIntervalExpired) {
						linkClickedEnabler.ResetLastEventTime();
						item.ExecuteAction();
					}
					else {
						UpdateSelection();
					}
				}
				finally {
					Unlock();
				}
			}
		}
		private void barManagerHolder_BarManagerChanged(object sender, EventArgs e) {
			MenuManager = ((IBarManagerHolder)sender).BarManager;
		}
		private void ApplyGroupStyle() {
			foreach(NavBarGroup group in Groups) {
				group.GroupStyle = groupStyle;
			}
		}
		protected NavBarGroup FindGroupControl(ChoiceActionItem item) {
			if(item != null && actionItemToNavBarGroupMap.ContainsKey(item)) {
				return actionItemToNavBarGroupMap[item];
			}
			return null;
		}
		protected override void RaiseLinkClicked(NavBarItemLink link) {
			base.RaiseLinkClicked(link);
			ProcessLinkAction(link);
		}
		protected override void RaiseSelectedLinkChanged(NavBarSelectedLinkChangedEventArgs e) {
			base.RaiseSelectedLinkChanged(e);
			if(!isSelectionUpdating) {
				ProcessLinkAction(e.Link);
			}
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			IBarManagerHolder barManagerHolder = FindForm() as IBarManagerHolder;
			if(barManagerHolder != null && barManagerHolder.BarManager != null) {
				MenuManager = barManagerHolder.BarManager;
				barManagerHolder.BarManagerChanged += new EventHandler(barManagerHolder_BarManagerChanged);
			}
		}
		protected override void Dispose(bool disposing) {
			try {
				if(disposing) {
					UnsubscribeFromActionEvents();
					ClearMaps();
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		protected virtual void OnCreateCustomGroupControl(CreateCustomGroupControlEventArgs args) {
			if(CreateCustomGroupControl != null) {
				CreateCustomGroupControl(this, args);
			}
		}
		public NavBarNavigationControl() {
			linkClickedEnabler.TimeoutInMilliseconds = RepeatDelay;
			ActiveGroup = null;
			Dock = DockStyle.Fill;
			DragDropFlags = NavBarDragDrop.None;
			PaintStyleKind = NavBarViewKind.NavigationPane;
			StoreDefaultPaintStyleName = true;
			LinkSelectionMode = LinkSelectionModeType.OneInControl;
			PaintStyleName = "Default";
			OptionsNavPane.ShowExpandButton = false;
			actionItemToItemLinkMap = new Dictionary<ChoiceActionItem, NavBarItemLink>();
			actionItemToNavBarGroupMap = new Dictionary<ChoiceActionItem, NavBarGroup>();
			actionItemToTreeNavigationContainerMap = new Dictionary<ChoiceActionItem, TreeListNavigationControl>();
			treeNavigationContainerToNavBarGroupMap = new Dictionary<TreeListNavigationControl, NavBarGroup>();
			itemLinkToActionItemWrapperMap = new Dictionary<NavBarItemLink, NavBarItemChoiceActionItem>();
			groupToActionItemWrapperMap = new Dictionary<NavBarGroup, NavBarGroupChoiceActionItem>();
		}
		public void SetNavigationActionItems(ChoiceActionItemCollection actionItems, SingleChoiceAction action) {
			Guard.ArgumentNotNull(action, "action");
			this.actionItems = actionItems;
			UnsubscribeFromActionEvents();
			singleChoiceAction = action;
			BuildControl();
			SubscribeToActionEvents();
			Tag = EasyTestTagHelper.FormatTestAction(action.Caption);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static int RepeatDelay {
			get { return repeatDelay; }
			set { repeatDelay = value; }
		}
		[DefaultValue(NavBarGroupStyle.LargeIconsText)]
		public NavBarGroupStyle GroupStyle {
			get { return groupStyle; }
			set {
				if(groupStyle != value) {
					groupStyle = value;
					ApplyGroupStyle();
				}
			}
		}
		public Dictionary<ChoiceActionItem, NavBarItemLink> ActionItemToItemLinkMap {
			get { return actionItemToItemLinkMap; }
		}
		public Dictionary<ChoiceActionItem, NavBarGroup> ActionItemToNavBarGroupMap {
			get { return actionItemToNavBarGroupMap; }
		}
		public Dictionary<ChoiceActionItem, TreeListNavigationControl> ActionItemToTreeNavigationContainerMap {
			get { return actionItemToTreeNavigationContainerMap; }
		}
		public Dictionary<TreeListNavigationControl, NavBarGroup> TreeNavigationContainerToNavBarGroupMap {
			get { return treeNavigationContainerToNavBarGroupMap; }
		}
		public Dictionary<NavBarItemLink, NavBarItemChoiceActionItem> ItemLinkToActionItemWrapperMap {
			get { return itemLinkToActionItemWrapperMap; }
		}
		public Dictionary<NavBarGroup, NavBarGroupChoiceActionItem> GroupToActionItemWrapperMap {
			get { return groupToActionItemWrapperMap; }
		}
		public event EventHandler<CreateCustomGroupControlEventArgs> CreateCustomGroupControl;
		#region ISupportNavigationActionContainerTesting
		protected NavBarItemLink FindChildControl(ChoiceActionItem item) {
			NavBarGroup navBarActionGroup = FindGroupControl(item.ParentItem);
			if(navBarActionGroup != null) {
				foreach(NavBarItemLink link in navBarActionGroup.ItemLinks) {
					if(link != null && itemLinkToActionItemWrapperMap[link].ActionItem == item) {
						return link;
					}
				}
			}
			return null;
		}
		public bool IsItemControlVisible(ChoiceActionItem item) {
			bool result = false;
			NavBarGroup group = FindGroupControl(item);
			if(group != null) {
				result = group.Visible;
			}
			return result;
		}
		public string GetGroupControlCaption(ChoiceActionItem item) {
			NavBarGroup navBarActionGroup = FindGroupControl(item);
			if(navBarActionGroup != null) {
				return navBarActionGroup.Caption;
			}
			throw new ArgumentOutOfRangeException();
		}
		public int GetGroupChildControlCount(ChoiceActionItem item) {
			NavBarGroup navBarActionGroup = FindGroupControl(item);
			if(navBarActionGroup != null) {
				return navBarActionGroup.ItemLinks.Count;
			}
			throw new ArgumentOutOfRangeException();
		}
		public string GetChildControlCaption(ChoiceActionItem item) {
			NavBarItemLink link = FindChildControl(item);
			if(link != null) {
				return link.Caption;
			}
			throw new ArgumentOutOfRangeException();
		}
		public bool GetChildControlVisible(ChoiceActionItem item) {
			NavBarItemLink link = FindChildControl(item);
			if(link != null) {
				return link.Visible;
			}
			throw new ArgumentOutOfRangeException();
		}
		#endregion
		#region INavigationControlTestable Members
		bool INavigationControlTestable.IsItemEnabled(ChoiceActionItem item) {
			if(actionItemToItemLinkMap.ContainsKey(item)) {
				return actionItemToItemLinkMap[item].Enabled;
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
			if(actionItemToItemLinkMap.ContainsKey(item)) {
				return actionItemToItemLinkMap[item].Visible;
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
			if(actionItemToItemLinkMap.ContainsKey(item)) {
				return 0;
			}
			if(actionItemToTreeNavigationContainerMap.ContainsKey(item)) {
				return ((INavigationControlTestable)actionItemToTreeNavigationContainerMap[item]).GetSubItemsCount(item);
			}
			if(actionItemToNavBarGroupMap.ContainsKey(item)) {
				NavBarGroup group = actionItemToNavBarGroupMap[item];
				if(group.GroupStyle == NavBarGroupStyle.ControlContainer) {
					TreeListNavigationControl tree = group.ControlContainer.Controls[0] as TreeListNavigationControl;
					if(tree != null) {
						return tree.GetAllNodes().Count;
					}
				}
				else {
					return actionItemToNavBarGroupMap[item].ItemLinks.Count;
				}
			}
			return 0;
		}
		string INavigationControlTestable.GetItemCaption(ChoiceActionItem item) {
			if(actionItemToItemLinkMap.ContainsKey(item)) {
				return actionItemToItemLinkMap[item].Caption;
			}
			if(actionItemToTreeNavigationContainerMap.ContainsKey(item)) {
				return ((INavigationControlTestable)actionItemToTreeNavigationContainerMap[item]).GetItemCaption(item);
			}
			if(actionItemToNavBarGroupMap.ContainsKey(item)) {
				return actionItemToNavBarGroupMap[item].Caption;
			}
			return string.Empty;
		}
		string INavigationControlTestable.GetItemToolTip(ChoiceActionItem item) {
			if(actionItemToItemLinkMap.ContainsKey(item)) {
				return actionItemToItemLinkMap[item].Item.Hint;
			}
			if(actionItemToTreeNavigationContainerMap.ContainsKey(item)) {
				return ((INavigationControlTestable)actionItemToTreeNavigationContainerMap[item]).GetItemToolTip(item);
			}
			if(actionItemToNavBarGroupMap.ContainsKey(item)) {
				return actionItemToNavBarGroupMap[item].Hint;
			}
			return string.Empty;
		}
		int INavigationControlTestable.GetGroupCount() {
			return Groups.Count;
		}
		int INavigationControlTestable.GetSubGroupCount(ChoiceActionItem item) {
			if(actionItemToTreeNavigationContainerMap.ContainsKey(item)) {
				return ((INavigationControlTestable)actionItemToTreeNavigationContainerMap[item]).GetSubGroupCount(item);
			}
			NavBarGroup group = FindGroupControl(item);
			if(group != null && group.GroupStyle == NavBarGroupStyle.ControlContainer && group.ControlContainer.Controls[0] is TreeListNavigationControl) {
				INavigationControlTestable tree = (TreeListNavigationControl)group.ControlContainer.Controls[0];
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
			if(SelectedLink != null) {
				return SelectedLink.Caption;
			}
			foreach(TreeListNavigationControl tree in treeNavigationContainerToNavBarGroupMap.Keys) {
				if(tree.FocusedNode != null) {
					return tree.FocusedNode.GetDisplayText(0);
				}
			}
			return string.Empty;
		}
		#endregion
	}
}
