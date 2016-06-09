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
using System.Windows.Forms;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates.ActionContainers;
using DevExpress.ExpressApp.Utils;
using DevExpress.Utils;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.Nodes.Operations;
using DevExpress.XtraTreeList.ViewInfo;
namespace DevExpress.ExpressApp.Win.Templates.ActionContainers {	
	[ToolboxItem(false)]
	public class TreeListNavigationControl : TreeList, INavigationControl, INavigationControlTestable, ISupportUpdate {
		private const string UpdateSelectionCallName = "UpdateSelection";
		public static int ShowViewByKeyNavigationDelay = SystemInformation.MenuShowDelay != 0 ? 2 * SystemInformation.MenuShowDelay : 1000;
		private Locker locker; 
		private SingleChoiceAction singleChoiceAction;
		private ImageCollection imageCollection;
		private Dictionary<ChoiceActionItem, TreeListNodeChoiceActionItemWrapper> actionItemToWrapperMap;
		private Dictionary<ChoiceActionItem, bool> nodesStateCache;
		private TreeListNode oldNode;
		private bool raiseFocusedNodeChanged;
		private Timer showViewByKeyNavigationTimer;
		private bool navigationByKeyOn;
		private ToolTipController toolTipController;
		private void locker_LockedChanged(object sender, LockedChangedEventArgs e) {
			if(!locker.Locked && e.PendingCalls.Contains(UpdateSelectionCallName)) {
				UpdateSelection();
			}
		}
		private void TreeNavigationContainer_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e) {
			if(locker.Locked) return;
			locker.Lock();
			try {
				if(e.Node != null && singleChoiceAction != null) {
					ChoiceActionItem actionItem = (ChoiceActionItem)e.Node.Tag;
					if(singleChoiceAction.Active && singleChoiceAction.Enabled) {
						singleChoiceAction.DoExecute(actionItem);
					}
				}
			}
			finally {
				locker.Unlock();
			}
		}
		private void SubscribeToEvents() {
			FocusedNodeChanged += new FocusedNodeChangedEventHandler(TreeNavigationContainer_FocusedNodeChanged);
		}
		private void UnsubscribeFromEvents() {
			FocusedNodeChanged -= new FocusedNodeChangedEventHandler(TreeNavigationContainer_FocusedNodeChanged);
		}
		private void SubscribeToActionEvents() {
			singleChoiceAction.ItemsChanged += new EventHandler<ItemsChangedEventArgs>(action_ItemsChanged);
			singleChoiceAction.SelectedItemChanged += new EventHandler(action_SelectedItemChanged);
		}
		private void UnsubscribeFromActionEvents() {
			if(singleChoiceAction != null) {
				singleChoiceAction.ItemsChanged -= new EventHandler<ItemsChangedEventArgs>(action_ItemsChanged);
				singleChoiceAction.SelectedItemChanged -= new EventHandler(action_SelectedItemChanged);
			}
		}
		private void focusedNodeChangedOnKeyUpTimer_Tick(object sender, EventArgs e) {
			raiseFocusedNodeChanged = true;
			RaiseFocusedNodeChanged(oldNode, FocusedNode);
		}
		private void CreateShowViewByKeyNavigationTimer() {
			showViewByKeyNavigationTimer = new Timer();
			showViewByKeyNavigationTimer.Interval = ShowViewByKeyNavigationDelay;
			showViewByKeyNavigationTimer.Tick += new EventHandler(focusedNodeChangedOnKeyUpTimer_Tick);
		}
		private void StopShowViewByKeyNavigationTimer() {
			showViewByKeyNavigationTimer.Stop();
			showViewByKeyNavigationTimer.Enabled = false;
		}
		private void StartShowViewByKeyNavigationTimer() {
			showViewByKeyNavigationTimer.Enabled = true;
			showViewByKeyNavigationTimer.Start();
		}
		private void ReleaseShowViewByKeyNavigationTimer() {
			StopShowViewByKeyNavigationTimer();
			showViewByKeyNavigationTimer.Tick -= new EventHandler(focusedNodeChangedOnKeyUpTimer_Tick);
		}
		public TreeListNavigationControl()
			: base() {
			CreateShowViewByKeyNavigationTimer();
			navigationByKeyOn = false;
			Dock = System.Windows.Forms.DockStyle.Fill;
			Columns.Add().VisibleIndex = 0;
			imageCollection = new ImageCollection();
			actionItemToWrapperMap = new Dictionary<ChoiceActionItem, TreeListNodeChoiceActionItemWrapper>();
			nodesStateCache = new Dictionary<ChoiceActionItem, bool>();
			StateImageList = imageCollection;
			OptionsBehavior.AllowIncrementalSearch = true;
			OptionsMenu.EnableColumnMenu = false;
			OptionsMenu.EnableFooterMenu = false;
			OptionsView.ShowVertLines = false;
			OptionsBehavior.Editable = false;
			OptionsBehavior.ResizeNodes = true;
			OptionsSelection.MultiSelect = false;
			OptionsSelection.EnableAppearanceFocusedCell = false;
			OptionsSelection.EnableAppearanceFocusedRow = true;
			Appearance.FocusedCell.Options.UseBackColor = true;
			OptionsView.ShowIndicator = false;
			OptionsView.FocusRectStyle = DrawFocusRectStyle.CellFocus;
			OptionsView.ShowColumns = false;
			OptionsView.ShowHorzLines = false;
			locker = new Locker();
			locker.LockedChanged += new EventHandler<LockedChangedEventArgs>(locker_LockedChanged);
			toolTipController = GetToolTipController();
			if(toolTipController != null) {
				toolTipController.GetActiveObjectInfo += new ToolTipControllerGetActiveObjectInfoEventHandler(ToolTipController_GetActiveObjectInfo);
			}
		}
		void ToolTipController_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e) {
			if(e.SelectedControl != this) {
				return;
			}
			TreeListHitInfo hitInfo = CalcHitInfo(e.ControlMousePosition);
			if(hitInfo.HitInfoType == HitInfoType.Cell) {
				ChoiceActionItem actionItem = hitInfo.Node.Tag as ChoiceActionItem;
				if(actionItem != null) {
					object cellInfo = new TreeListCellToolTipInfo(hitInfo.Node, hitInfo.Column, null);
					IModelToolTipOptions options = actionItem.Model as IModelToolTipOptions;
					if(options != null) {
						e.Info = new ToolTipControlInfo(cellInfo, actionItem.ToolTip, options.ToolTipTitle, options.ToolTipIconType);
					}
				}
			}
		}
		public TreeListNode FindNodeByActionItem(ChoiceActionItem actionItem) {
			if(actionItem != null && actionItemToWrapperMap.ContainsKey(actionItem)) {
				return actionItemToWrapperMap[actionItem].Node;
			}
			return null;			
		}
		public ArrayList GetAllNodes() {
			TreeListOperationAccumulateNodes operation = new TreeListOperationAccumulateNodes();
			NodesIterator.DoOperation(operation);
			return operation.Nodes;
		}
		private void action_SelectedItemChanged(object sender, EventArgs e) {
			UpdateSelection();
		}
		private void UpdateSelection() {
			locker.Call(UpdateSelectionCallName);
			if(locker.Locked) return;
			locker.Lock();
			try {
				if(singleChoiceAction.SelectedItem != null) {
					UnsubscribeFromEvents();
					UnsubscribeFromActionEvents();
					UpdateSelectionCore(true);
					SubscribeToEvents();
					SubscribeToActionEvents();
				}
			}
			finally {
				locker.Unlock();
			}
		}
		private void UpdateSelectionCore(bool forceUpdate) {
			if (singleChoiceAction.SelectedItem != null || forceUpdate) {
				FocusedNode = FindNodeByActionItem(singleChoiceAction.SelectedItem);
			}
		}
		private void action_ItemsChanged(object sender, ItemsChangedEventArgs e) {
			if(!IsDisposed) {
				SaveNodesState();
				BuildControl();
			}
		}
		private void SaveNodesState() {
			nodesStateCache.Clear();
			foreach(TreeListNode node in GetAllNodes()) {
				nodesStateCache.Add((ChoiceActionItem)node.Tag, node.Expanded);
			}
		}
		private void ClearActionItemToWrapperMap() {
			foreach(TreeListNodeChoiceActionItemWrapper wrapper in actionItemToWrapperMap.Values) {
				wrapper.Dispose();
			}
			actionItemToWrapperMap.Clear();
		}
		private void BuildControl() {
			Nodes.Clear();
			ClearActionItemToWrapperMap();
			UnsubscribeFromEvents();
			BeginUnboundLoad();
			foreach(ChoiceActionItem item in actionItems) {
				CreateItems(item, null);
			}
			EndUnboundLoad();
			UpdateSelectionCore(false);
			SubscribeToEvents();
		}
		private void CreateItems(ChoiceActionItem actionItem, TreeListNode parentNode) {
			IModelNavigationItem actionItemModel = actionItem.Model as IModelNavigationItem;	
			if((actionItemModel == null || actionItemModel.Visible) && actionItem.Active) {
				TreeListNode node = AppendNode(new object[] { actionItem.Caption }, parentNode, actionItem);
				TreeListNodeChoiceActionItemWrapper nodeWrapper;
				IModelRootNavigationItems modelRootNavigationItems;
				if(ModelNavigationItemsDomainLogic.TryFindRootNavigationItems(actionItem.Model, out modelRootNavigationItems)) {
					nodeWrapper = new TreeListNodeChoiceActionItemWrapper(actionItem, node, imageCollection, singleChoiceAction, modelRootNavigationItems.DefaultParentImageName, modelRootNavigationItems.DefaultLeafImageName);
				}
				else {
					nodeWrapper = new TreeListNodeChoiceActionItemWrapper(actionItem, node, imageCollection, singleChoiceAction, null, null);
				}
				actionItemToWrapperMap.Add(actionItem, nodeWrapper);
				foreach(ChoiceActionItem choiceAction in actionItem.Items) {
					CreateItems(choiceAction, node);
					if(nodesStateCache.ContainsKey(actionItem)) {
						node.Expanded = nodesStateCache[actionItem];
					}
				}
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				ReleaseShowViewByKeyNavigationTimer();
				UnsubscribeFromActionEvents();
				UnsubscribeFromEvents();
				ClearActionItemToWrapperMap();
				nodesStateCache.Clear();
				nodesStateCache = null;
				singleChoiceAction = null;
				locker.LockedChanged -= new EventHandler<LockedChangedEventArgs>(locker_LockedChanged);
				if(toolTipController != null) {
					toolTipController.GetActiveObjectInfo -= new ToolTipControllerGetActiveObjectInfoEventHandler(ToolTipController_GetActiveObjectInfo);
				}
			}
			base.Dispose(disposing);
		}
		protected override void RaiseFocusedNodeChanged(TreeListNode oldNode, TreeListNode newNode) {
			this.oldNode = oldNode;
			if(navigationByKeyOn) {
				StopShowViewByKeyNavigationTimer();
				StartShowViewByKeyNavigationTimer();
			}
			if(raiseFocusedNodeChanged) {
				StopShowViewByKeyNavigationTimer();
				raiseFocusedNodeChanged = false;
				base.RaiseFocusedNodeChanged(oldNode, newNode);
			}
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			if(e.Button == MouseButtons.Left) {
				TreeListHitInfo li = CalcHitInfo(e.Location);
				if(li.HitInfoType == HitInfoType.Row || li.HitInfoType == HitInfoType.RowIndent ||
					li.HitInfoType == HitInfoType.RowIndicator || li.HitInfoType == HitInfoType.RowPreview ||
					li.HitInfoType == HitInfoType.Cell || li.HitInfoType == HitInfoType.StateImage ||
					li.HitInfoType == HitInfoType.SelectImage || li.HitInfoType == HitInfoType.RowIndicatorEdge) {
					raiseFocusedNodeChanged = true;
					RaiseFocusedNodeChanged(oldNode,li.Node);
				}
			}
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			if(e.KeyCode == Keys.Enter) {
				raiseFocusedNodeChanged = true;
				RaiseFocusedNodeChanged(oldNode, FocusedNode);
			}
			else if(e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || (e.Control && (e.KeyCode == Keys.Right || e.KeyCode == Keys.Left))) {
				navigationByKeyOn = true;
			}
			base.OnKeyDown(e);
		}
		protected override void OnKeyUp(System.Windows.Forms.KeyEventArgs e) {
			base.OnKeyUp(e);
			if(e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.PageUp || e.KeyCode == Keys.PageDown || (e.Control && (e.KeyCode == Keys.Right || e.KeyCode == Keys.Left))) {
				StopShowViewByKeyNavigationTimer();
				StartShowViewByKeyNavigationTimer();
			}
			navigationByKeyOn = false;
		}
		#region INavigationControlTestable Members
		bool INavigationControlTestable.IsItemEnabled(ChoiceActionItem item) {
			return FindNodeByActionItem(item).Visible;
		}
		bool INavigationControlTestable.IsItemVisible(ChoiceActionItem item) {
			TreeListNode node = FindNodeByActionItem(item);
			return node == null ? false : node.Visible;
		}
		int INavigationControlTestable.GetSubItemsCount(ChoiceActionItem item) {
			return FindNodeByActionItem(item).Nodes.Count;
		}
		string INavigationControlTestable.GetItemCaption(ChoiceActionItem item) {
			return FindNodeByActionItem(item).GetDisplayText(0);
		}
		int INavigationControlTestable.GetGroupCount() {
			return Nodes.Count;
		}
		int INavigationControlTestable.GetSubGroupCount(ChoiceActionItem item) {
			int result = 0;
			TreeListNodes nodes = null;
			if(item != null) {
				TreeListNode groupNode = FindNodeByActionItem(item);
				nodes = groupNode.Nodes;
			}
			else {
				nodes = Nodes;
			}
			foreach(TreeListNode node in nodes) {
				if(node.Nodes.Count != 0) {
					result++;
				}
			}
			return result;
		}
		bool INavigationControlTestable.IsGroupExpanded(ChoiceActionItem item) {
			return FindNodeByActionItem(item).Expanded;
		}
		string INavigationControlTestable.GetSelectedItemCaption() {
			if(FocusedNode != null) {
				return FocusedNode.GetDisplayText(0);
			}
			return string.Empty;
		}
		string INavigationControlTestable.GetItemToolTip(ChoiceActionItem item) {
			return string.Empty;
		}
		#endregion
		#region INavigationControl Members
		private ChoiceActionItemCollection actionItems;
		public void SetNavigationActionItems(ChoiceActionItemCollection actionItems, SingleChoiceAction action) {
			DevExpress.ExpressApp.Utils.Guard.ArgumentNotNull(action, "action");
			this.actionItems = actionItems;
			UnsubscribeFromActionEvents();
			singleChoiceAction = action;
			BuildControl();
			SubscribeToActionEvents();
		}
		#endregion
		public bool ForceRaiseFocusedNodeChanged {
			get { return raiseFocusedNodeChanged; }
			set { raiseFocusedNodeChanged = value; }
		}
	}
	public class TreeListNodeChoiceActionItemWrapper : TreeListNodeChoiceActionItemWrapperBase {
		private TreeListNode treeListNode;
		private ImageCollection imageCollection;
		protected override void SetImageInfo(DevExpress.ExpressApp.Utils.ImageInfo imageInfo) {
			int imageIndex;
			if(!imageInfo.IsEmpty && !imageCollection.Images.Contains(imageInfo.Image)) {
				imageCollection.Images.Add(imageInfo.Image);
				imageIndex = imageCollection.Images.Count - 1;
			}
			else {
				imageIndex = imageCollection.Images.IndexOf(imageInfo.Image);
			}
			treeListNode.StateImageIndex = imageIndex;
		}
		public override void SetCaption(string caption) {
			treeListNode.SetValue(0, caption);
		}
		public override void SetEnabled(bool enabled) {
			treeListNode.Visible = enabled && ActionItem.Active;
		}
		public override void SetVisible(bool visible) {
			treeListNode.Visible = visible && ActionItem.Enabled;
		}
		public override void SetToolTip(string toolTip) {
		}
		public TreeListNodeChoiceActionItemWrapper(ChoiceActionItem item, TreeListNode node, ImageCollection imageCollection, ChoiceActionBase action, string defaultParentImageName, string defaultLeafImageName)
			: base(item, action, defaultParentImageName, defaultLeafImageName) {
			treeListNode = node;
			this.imageCollection = imageCollection;
			SyncronizeWithItem();
		}
		public TreeListNode Node {
			get { return treeListNode; }
		}
		public override void Dispose() {
			base.Dispose();
			treeListNode = null;
			imageCollection = null;
		}
	}
}
