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
using System.ComponentModel;
using System.Drawing;
using System.Web;
using System.Web.UI.Design;
using System.Windows.Forms;
using DevExpress.Web.Design;
using DevExpress.Web.Design.Forms;
using DevExpress.Web.Internal;
using DevExpress.Web.Design.Utils;
using EnvDTE;
namespace DevExpress.Web.Design {
	public class SiteMapNodesEditorForm : TreeEditorForm {
		private const string InvalidDataMessage_IdenticalUrls = "SiteMap contains several nodes with identical URLs";
		private const string InvalidDataMessage_InvalidXmlFormat = "Invalid web.sitemap xml file";
		private const string InvalidDataMessage_OnlyOneSiteMapNodeAtTop = "Only one SiteMapNode required at top";
		private const string ScanItemViewerSplitter_Name = "ScanItemViewerSplitter";
		private const string InvalidDataMessage_TopElementSiteMap = "Top element must be SiteMap";
		private const string AutoGenerateImageResource = "DevExpress.Web.Design.Images.AutoGenerate.bmp";
		private const string CloseButtonImage = "DevExpress.Web.Design.Images.Close.bmp";
		private const string DeleteFromTreeViewCursorResource = "Cursors.DELETE.cur";
		private const string ShowPageUrlButtonImage = "DevExpress.Web.Design.Images.URL.png";
		private const string ScanImageResource = "DevExpress.Web.Design.Images.Scan.png";
		private const string SiteMapProjectItemTemplate = @"Web Developer Project Files\Visual C#\Xml File";
		private const string RefreshImageResorce = "DevExpress.Web.Design.Images.refresh.png";
		private const int MessageLabelPadding = 5;
		private const int SiteMapEditorItemsPanelMinimizeWidth = 283;
		private const int SiteMapNodesPanelWidth = 280;
		private const int ScanMessagePanelHeight = 59;
		private const int ScanMessagePanelWidth = 10;
		private const int ScanPanelMinimumWidth = 150;
		private const int ScanTitlePanelHeight = 30;
		private const int ScanTitleLabelMargin = 10;
		private bool fDontAskSaveConfirm = false;
		ToolStripButton fFindNewPagesButton = null;
		private bool fIsDataChanged = false;
		private Splitter fScanItemViewerSplitter = null;
		private Label fScanMessageLabel = null;
		private PanelEx fScanMessagePanel = null;
		private Splitter fScanMessagePanelSplit = null;
		private Panel fScanPanel = null;
		private TreeView fScanTreeView = null;
		private EditableSiteMapProvider fSiteMapControlProvider = null;
		private EnvDTE.ProjectItem fSiteMapFileProjectItem = null;
		private string fSiteMapFileName = "";
		private SiteMapScanner fSiteMapScanner = null;
		ToolStripButton fShowPageFileNameButton = null;
		protected override Size FormDefaultSize { get { return new Size(680, 500); } }		 
		protected override Size FormMinimumSize { get { return new Size(670, 500); } } 
		protected override int LeftPanelMinimizeWidth { get { return SiteMapEditorItemsPanelMinimizeWidth; } }
		protected override int LeftPanelDefaultWidth { get { return SiteMapNodesPanelWidth; } }
		protected override int RigthPanelDefaultWidth { get { return 220; } }
		protected int ScanPanelDefautWidth { get { return 160; } }
		private IWebApplication WebApplication {
			get { return (IWebApplication)ServiceProvider.GetService(typeof(IWebApplication)); }
		}
		public SiteMapNodesEditorForm(object component, ITypeDescriptorContext context,
			IServiceProvider provider, object propertyValue, EditableSiteMapProvider siteMapProvider, string siteMapFileName)
			: base(component, context, provider, propertyValue) {
			fIsDataChanged = false;
			fSiteMapControlProvider = siteMapProvider;
			fSiteMapScanner = new SiteMapScanner(DesignerHost);
			fSiteMapFileName = siteMapFileName;
		}
		protected override void AddCustomControlsToLeftPanel(Panel panel) {
			base.AddCustomControlsToLeftPanel(panel);
			AddScanPanelToPanel(panel);
		}
		protected override void AddToolStripButtons(List<ToolStripItem> buttons) {
			base.AddToolStripButtons(buttons);
			buttons.Add(CreateToolStripSeparator());
			buttons.Add(CreatePushButton(StringResources.SiteMapControlEditor_AutoGenerate, CreateImageForButton(AutoGenerateImageResource), OnAutoGenerateButtonClick));
			fFindNewPagesButton = CreatePushButton(StringResources.SiteMapControlEditor_NewPagesBrowser, CreateImageForButton(ScanImageResource), OnScanButtonClick);
			fFindNewPagesButton.CheckOnClick = true;
			buttons.Add(fFindNewPagesButton);
		}
		protected override void FillItemsViewer() {
			base.FillItemsViewer();
			RefreshItemViewer();
		}
		protected override DialogResult GetDialogResultOkBtn() {
			return DialogResult.None;
		}
		protected override string GetFormCaption() {
			return (Designer as ASPxSiteMapDataSourceDesigner).GetFormCaption();
		}
		protected override string GetPropertyNameShowingInFormCaption() {
			return "Site Map";
		}
		protected override void AddMessageStrings(List<string> messages) {
			if ((fSiteMapControlProvider.DataState != ProviderDataState.Sample) && (fSiteMapControlProvider.UrlInfo.ContainDuplicatedStrings()))
				messages.Add(StringResources.SiteMapControl_DuplicatedUrl);
			else
				if (fSiteMapControlProvider.DataState == ProviderDataState.Sample)
					messages.Add(fSiteMapControlProvider.GetExceptionString());
			if (fSiteMapControlProvider.UrlInfo.ContainInvalidUrl() && fSiteMapControlProvider.DataState != ProviderDataState.Sample)
				messages.Add(StringResources.SiteMapControl_InvalidUrl);
		}
		protected override string GetRemoveAllConfirmString() {
			return StringResources.SiteMapControlEditor_RemoveAllConfirmDialogText;
		}
		protected override TreeNodeCollection GetRootTreeNodes(TreeView treeView) {
			return treeView.Nodes[0].Nodes;
		}
		protected override string GetPropertyStorePathPrefix() {
			return "SiteMapNodesEditorForm";
		}
		protected override void OnShown(EventArgs e) {
			base.OnShown(e);
			UpdateTools();
			SetOkButtonEnable(false);
		}
		protected override void AddChild(object parent, object child) {
			if (parent != null)
				fSiteMapControlProvider.AddSiteMapNode(child as SiteMapNode, parent as SiteMapNode);
			else {
				fSiteMapControlProvider.RootNode.Title = "RootNode";
			}
		}
		protected override void CollapseNodesInTreeView(TreeView treeView) {
			if (treeView.Nodes.Count != 0)
				treeView.Nodes[0].Expand();
			base.CollapseNodesInTreeView(treeView);
		}
		protected override object CreateNewItem() {
			if (fSiteMapControlProvider.DataState == ProviderDataState.Sample) {
				fSiteMapControlProvider.DataState = ProviderDataState.Normal;
				return fSiteMapControlProvider.RootNode;
			}
			else
				return fSiteMapControlProvider.CreateNode("", "New Node");
		}
		protected override TreeNode CreateTreeNode(object item) {
			SiteMapNode nodeItem = item as SiteMapNode;
			EditableSiteMapProvider provider = (item as SiteMapNode).Provider as EditableSiteMapProvider;
			if (provider.DataState != ProviderDataState.Sample) {
				TreeNode node = base.CreateTreeNode(item);
				TypeDescriptor.AddAttributes(node.Tag as SiteMapNode, new Attribute[] { new TypeConverterAttribute(typeof(SiteMapNodeTypeConverter)) });
				return node;
			}
			else
				return null;
		}
		protected override void DecreaseIndent(object parent, object item) {
			fSiteMapControlProvider.DecreaseIndent(item as SiteMapNode);
			base.DecreaseIndent(parent, item);
		}
		protected override void IncreaseIndent(object parent, object item) {
			fSiteMapControlProvider.IncreaseIndent(item as SiteMapNode);
			base.IncreaseIndent(parent, item);
		}
		protected override DragNodeState GetDragNodeState(TreeView dragOverTreeView, Point point, TreeNode draggedNode, TreeNode dragOverNode) {
			if ((dragOverTreeView != draggedNode.TreeView) && (dragOverTreeView == fScanTreeView))
				return DragNodeState.DropToScanTreeView;
			else {
				if (dragOverTreeView == fScanTreeView)
					return DragNodeState.None;
				else
					return base.GetDragNodeState(dragOverTreeView, point, draggedNode, dragOverNode);
			}
		}
		protected override void InsertItem(object parent, object insertingItem, object currentItem) {
			SiteMapNode parentNode = parent as SiteMapNode;
			SiteMapNode insertingNode = insertingItem as SiteMapNode;
			int currentIndex = parentNode.ChildNodes.IndexOf(currentItem as SiteMapNode);
			fSiteMapControlProvider.InsertSiteMapNode(currentIndex, parentNode, insertingNode);
		}
		protected override void MoveDownItem(object parent, object item) {
			fSiteMapControlProvider.MoveDownSiteMapNode(item as SiteMapNode);
			base.MoveDownItem(parent, item);
		}
		protected override void MoveUpItem(object parent, object item) {
			fSiteMapControlProvider.MoveUpSiteMapNode(item as SiteMapNode);
			base.MoveUpItem(parent, item);
		}
		protected override void RemoveAllItems() {
			fSiteMapControlProvider.Clear();
			base.RemoveAllItems();
			FillItemsViewer();
		}
		protected override void RemoveItem(object parent, object item) {
			fSiteMapControlProvider.RemoveSiteMapNode(item as SiteMapNode);
			base.RemoveItem(parent, item);
		}
		protected override void AddRootNode(TreeNode draggedNode) {
			SiteMapNode draggedSiteMapNode = draggedNode.Tag as SiteMapNode;
			(draggedSiteMapNode.Provider as EditableSiteMapProvider).RemoveSiteMapNode(draggedSiteMapNode);
			SiteMapNode clonedNode = CloneSiteMapNodeHierarchy(draggedSiteMapNode, fSiteMapControlProvider);
			fSiteMapControlProvider.RootNode.Title = clonedNode.Title;
			fSiteMapControlProvider.RootNode.Url = clonedNode.Url;
			TypeDescriptor.AddAttributes(fSiteMapControlProvider.RootNode, new Attribute[] { new TypeConverterAttribute(typeof(SiteMapNodeTypeConverter)) });
			SiteMapNodeCollection nodes = new SiteMapNodeCollection(clonedNode.ChildNodes);
			for (int i = 0; i < nodes.Count; i++)
				fSiteMapControlProvider.AddSiteMapNode(nodes[i]);
			RefreshTreeNodeHierarchy(draggedNode, fSiteMapControlProvider.RootNode);
			draggedNode.Tag = fSiteMapControlProvider.RootNode;
			fSiteMapControlProvider.DataState = ProviderDataState.Normal;
			base.AddRootNode(draggedNode);
			UpdateToolStrip();
		}
		protected override void ReplaceToNewParent(TreeNode draggedNode, TreeNode overNode) {
			PutDraggedNodeToSiteMapControlProvider(draggedNode, overNode);
			base.ReplaceToNewParent(draggedNode, overNode);
		}
		protected override void ReplaceInLevel(TreeNode draggedNode, TreeNode overNode) {
			PutDraggedNodeToSiteMapControlProvider(draggedNode, overNode);
			base.ReplaceInLevel(draggedNode, overNode);
		}
		protected override void ReplaceToEnd(TreeNode draggedNode, TreeNode lastDestNode) {
			PutDraggedNodeToSiteMapControlProvider(draggedNode, lastDestNode);
			base.ReplaceToEnd(draggedNode, lastDestNode);
		}
		protected override void ReplaceItemInLevel(object draggedObj, object overObj) {
			SiteMapNode overNode = overObj as SiteMapNode;
			SiteMapNode draggedNode = draggedObj as SiteMapNode;
			int index = overNode.ParentNode.ChildNodes.IndexOf(overNode);
			EditableSiteMapProvider destProvider = overNode.Provider as EditableSiteMapProvider;
			if (overNode.ParentNode == draggedNode.ParentNode) {
				int dragItemIndex = draggedNode.ParentNode.ChildNodes.IndexOf(draggedNode);
				if (index > dragItemIndex)
					index--;
			}
			destProvider.InsertSiteMapNode(index, overNode.ParentNode, draggedNode);
		}
		protected override void ReplaceItemToEnd(object item, object lastDestObj) {
			SiteMapNode lastDestNode = lastDestObj as SiteMapNode;
			EditableSiteMapProvider destProvider = lastDestNode.Provider as EditableSiteMapProvider;
			destProvider.AddSiteMapNode(item as SiteMapNode);
		}
		protected override void ReplaceItemToNewParent(object newParent, object item) {
			SiteMapNode overNode = newParent as SiteMapNode;
			SiteMapNode draggedNode = item as SiteMapNode;
			EditableSiteMapProvider destProvider = overNode.Provider as EditableSiteMapProvider;
			destProvider.AddSiteMapNode(draggedNode, overNode);
		}
		protected override void UpdateToolStrip() {
			base.UpdateToolStrip();
			ToolStripItem addRootItem = FindToolItemByText(StringResources.ItemsEditor_AddItemButtonText);
			addRootItem.Enabled = IsAddRootItemEnabled();
		}
		protected override void UpdateMenuStrip() {
			ToolStripItem item = FindPopupMenuItemByText(StringResources.ItemsEditorPopupMenu_AddItemButtonText);
			item.Enabled = IsAddRootItemEnabled();
			base.UpdateMenuStrip();
		}
		protected override bool CanDecreaseIndent() {
			SiteMapNode selectedNode = SelectedObject as SiteMapNode;
			return base.CanDecreaseIndent() && (selectedNode != null) && (selectedNode.ParentNode != null) &&
				(selectedNode.ParentNode != fSiteMapControlProvider.RootNode);
		}
		protected override bool CanDragNode(TreeView dragOverTreeView, TreeNode draggedNode, TreeNode dragOverNode) {
			if (draggedNode == draggedNode.TreeView.Nodes[0])
				return draggedNode.TreeView == fScanTreeView;
			else
				return base.CanDragNode(dragOverTreeView, draggedNode, dragOverNode);
		}
		protected override bool IsInsertButtonEnable() {
			if (SelectedObject != fSiteMapControlProvider.RootNode)
				return base.IsInsertButtonEnable();
			else
				return false;
		}
		protected override bool IsRemoveAllButtonEnable() {
			return fSiteMapControlProvider.RootNode.HasChildNodes;
		}
		protected override bool IsRemoveButtonEnable() {
			SiteMapNode selectedNode = SelectedObject as SiteMapNode;
			bool ret = true;
			if (selectedNode != null) {
				if (selectedNode.ParentNode == null)
					ret = false;
				else
					ret = fSiteMapControlProvider != null && fSiteMapControlProvider.RootNode.HasChildNodes;
			}
			else
				ret = false;
			return ret;
		}
		protected override bool IsValidData(TreeView treeView) {
			if (treeView == fScanTreeView)
				return fSiteMapScanner.SiteMapProvider.DataState != ProviderDataState.Sample;
			else
				return fSiteMapControlProvider.DataState != ProviderDataState.Sample;
		}
		protected override void ComponentChanged(bool checkChanged, bool isCollectionChangeNotification) {
			fIsDataChanged = true;
			SetOkButtonEnable(true);
			RefreshItemViewer();
			RefreshMessage();
		}
		protected override void DoDragDropTreeView(TreeNode draggedNode, TreeNode overNode, TreeView dropTreeView, DragNodeState dragNodeState) {
			if ((dragNodeState == DragNodeState.DropToScanTreeView) && (draggedNode.Tag != fSiteMapControlProvider.RootNode))
				RemoveItem();
			else
				if (dragNodeState == DragNodeState.AddRoot) {
					AddRootNode(draggedNode);
				}
				else
					base.DoDragDropTreeView(draggedNode, overNode, dropTreeView, dragNodeState);
			RefreshScanPanel();
		}
		protected override void DoGiveFeedbackTreeView(DragNodeState dragNodeState) {
			if (dragNodeState == DragNodeState.DropToScanTreeView)
				Cursor.Current = new Cursor(typeof(SiteMapNodesEditorForm), DeleteFromTreeViewCursorResource);
			else
				base.DoGiveFeedbackTreeView(dragNodeState);
		}
		protected override int GetDefaultSplitPosition() {
			return Size.Width / 2 + ScanPanelMinimumWidth;
		}
		protected override void PropertyValueChanged(PropertyValueChangedEventArgs e) {
			if (e.ChangedItem.Label == "Url") {
				fSiteMapControlProvider.UrlInfo.DeleteString(e.OldValue as string);
				fSiteMapControlProvider.UrlInfo.AddString(e.ChangedItem.Value as string);
				RefreshItemViewer();
				RefreshMessage();
			}
		}
		protected override void RestoreControlsProperties() {
			base.RestoreControlsProperties();
			fDontAskSaveConfirm = RestoreBoolProperty("DontAskSaveConfirm", fDontAskSaveConfirm);
			int splitPosition = RestoreIntProperty(ScanItemViewerSplitter_Name, 0);
			fFindNewPagesButton.Checked = RestoreBoolProperty("FindNewPagesButton", false);
			if (fFindNewPagesButton.Checked)
				OnScanButtonClick(null, null);
			if (splitPosition == 0)
				fScanItemViewerSplitter.SplitPosition = LeftPanelDefaultWidth;
			else
				fScanItemViewerSplitter.SplitPosition = splitPosition;
			fShowPageFileNameButton.Checked = RestoreBoolProperty("ShowPageFileNameButton", true);
			if (!fShowPageFileNameButton.Checked)
				OnShowPageUrlButtonClick(null, null);
		}
		protected override void SaveControlsProperties() {
			SaveIntProperty(ScanItemViewerSplitter_Name, fScanItemViewerSplitter.SplitPosition);
			SaveBoolProperty("FindNewPagesButton", fFindNewPagesButton.Checked);
			SaveBoolProperty("ShowPageFileNameButton", fShowPageFileNameButton.Checked);
			SaveBoolProperty("DontAskSaveConfirm", fDontAskSaveConfirm);
			base.SaveControlsProperties();
		}
		protected override void SaveChanges() {
			if (fIsDataChanged) {
				DialogResultEx result = DialogResultEx.Yes;
				if (IsExistSiteMapFile()) {
					if (fDontAskSaveConfirm)
						result = DialogResultEx.Yes;
					else
						result = MessageBoxEx.Show(this, StringResources.SiteMapControlEditor_SaveMessage,
							StringResources.ItemsEditor_ConfirmDialogCaption, MessageBoxButtonsEx.YesNoCancel,
							ref fDontAskSaveConfirm);
				}
				if (result == DialogResultEx.Yes) {
					Save();
					fIsDataChanged = false;
					this.DialogResult = DialogResult.Yes;
					DesignServices.ComponentChanged(ServiceProvider, Component);
					Close();
				}
				else {
					fDontAskSaveConfirm = false; 
					if (result == DialogResultEx.No) {
						this.DialogResult = DialogResult.No;
						Close();
					}
				}
			}
		}
		protected override void Undo() {
		}
		protected SiteMapNode AutoGenerateSiteMap() {
			BeginUpdateAppearance();
			try {
				DTE dte = (DTE)ServiceProvider.GetService(typeof(DTE));
				ProjectItem currentProjectItem = (ProjectItem)ServiceProvider.GetService(typeof(ProjectItem));
				if (currentProjectItem != null) {
					Project currentProject = currentProjectItem.ContainingProject;
					fSiteMapControlProvider.Clear();
					SiteMapAutoGenerator siteMapAutoGenerator = new SiteMapAutoGenerator();
					siteMapAutoGenerator.GenerateSiteMapByProject(currentProject, fSiteMapControlProvider, WebApplication);
				}
			}
			finally {
				EndUpdateAppearance();
			}
			return fSiteMapControlProvider.RootNode;
		}
		protected override string GetItemText(object item) {
			SiteMapNode node = item as SiteMapNode;
			if (node.Provider == fSiteMapScanner.SiteMapProvider)
				return (node as ScanSiteMapNode).GetText(fShowPageFileNameButton.Checked);
			else
				return base.GetItemText(item);
		}
		protected ToolStripItem[] GetScanToolStripButtons() {
			List<ToolStripItem> buttons = new List<ToolStripItem>();
			fShowPageFileNameButton = CreatePushButton(StringResources.SiteMapControlEditorScanPane_ShowPageUrl, CreateImageForButton(ShowPageUrlButtonImage), OnShowPageUrlButtonClick);
			fShowPageFileNameButton.ImageScaling = ToolStripItemImageScaling.SizeToFit;
			fShowPageFileNameButton.CheckOnClick = true;
			fShowPageFileNameButton.Checked = true;
			buttons.Add(fShowPageFileNameButton);
			ToolStripItem closeButton = CreatePushButton(StringResources.SiteMapControlEditor_Close, CreateImageForButton(CloseButtonImage), OnCloseScanPanelButton);
			closeButton.Alignment = ToolStripItemAlignment.Right;
			closeButton.ImageScaling = ToolStripItemImageScaling.None;
			buttons.Add(closeButton);
			buttons.Add(CreateToolStripSeparator());
			ToolStripButton refreshToolStripButton = CreatePushButton("Refresh", CreateImageForButton(RefreshImageResorce), OnRefreshButtonClick);
			buttons.Add(refreshToolStripButton);
			return buttons.ToArray();
		}
		protected void PutDraggedNodeToSiteMapControlProvider(TreeNode draggedNode, TreeNode overNode) {
			SiteMapNode overSiteMapNode = overNode.Tag as SiteMapNode;
			SiteMapNode draggedSiteMapNode = draggedNode.Tag as SiteMapNode;
			EditableSiteMapProvider destProvider = overSiteMapNode.Provider as EditableSiteMapProvider;
			if (overSiteMapNode.Provider != draggedSiteMapNode.Provider) {
				(draggedSiteMapNode.Provider as EditableSiteMapProvider).RemoveSiteMapNode(draggedSiteMapNode);
				SiteMapNode clonedNode = CloneSiteMapNodeHierarchy(draggedSiteMapNode, destProvider);
				RefreshTreeNodeHierarchy(draggedNode, clonedNode);
			}
		}
		protected void RefreshTreeNode(TreeNode treeNode, SiteMapNode siteMapNode) {
			treeNode.Tag = siteMapNode;
			treeNode.Text = GetItemText(treeNode.Tag);
		}
		protected void RefreshTreeNodeHierarchy(TreeNode treeNode, SiteMapNode siteMapNode) {
			int i = 0;
			RefreshTreeNode(treeNode, siteMapNode);
			foreach (TreeNode curTreeNode in treeNode.Nodes) {
				RefreshTreeNodeHierarchy(curTreeNode, siteMapNode.ChildNodes[i]);
				i++;
			}
		}
		protected bool IsAddRootItemEnabled() {
			if (fSiteMapControlProvider != null) {
				if (fSiteMapControlProvider.DataState == ProviderDataState.Sample)
					return true;
				else
					return (SelectedObject as SiteMapNode).ParentNode != null;
			}
			return true;
		}
		protected void OnAutoGenerateButtonClick(object sender, EventArgs e) {
			DialogResultEx result = DialogResultEx.Yes;
			if ((fSiteMapControlProvider.DataState != ProviderDataState.Empty) &&
				(fSiteMapControlProvider.DataState != ProviderDataState.Sample))
				result = MessageBoxEx.Show(this, StringResources.SiteMapControlEditor_DataLostMessage,
					StringResources.ItemsEditor_ConfirmDialogCaption, MessageBoxButtonsEx.YesNo);
			if (result == DialogResultEx.Yes) {
				if (fSiteMapControlProvider.DataState == ProviderDataState.Sample)
					fSiteMapControlProvider.DataState = ProviderDataState.Normal;
				try {
					AutoGenerateSiteMap();
					FillItemsViewer();
					ClearScanData();
				}
				finally {
					ComponentChanged(false);
				}
			}
		}
		protected void OnCloseScanPanelButton(object sender, EventArgs e) {
			fFindNewPagesButton.Checked = false;
			OnScanButtonClick(sender, e);
		}
		protected void OnDragDropScanTreeView(object sender, DragEventArgs e) {
			TreeNode draggedNode = (TreeNode)e.Data.GetData(typeof(TreeNode));
			OnDragDropTreeView(sender, e);
			if ((draggedNode.TreeView == null) || (draggedNode.TreeView != fScanTreeView))
				ScanWebApp(fScanTreeView);
		}
		protected void OnRefreshButtonClick(object sender, EventArgs e) {
			ScanWebApp(fScanTreeView);
		}
		protected void OnScanButtonClick(object sender, EventArgs e) {
			if (!fScanTreeView.Visible)
				ScanWebApp(fScanTreeView);
			else
				fScanTreeView.Nodes.Clear();
			fScanPanel.Visible = !fScanPanel.Visible;
			fScanItemViewerSplitter.Visible = !fScanItemViewerSplitter.Visible;
			fItemViewerPanel.SuspendLayout();
			fScanPanel.SuspendLayout();
			fSplitter.SuspendLayout();
			fSplitter.ResumeLayout(false);
			fItemViewerPanel.ResumeLayout(false);
			fItemViewerPanel.PerformLayout();
			fScanPanel.ResumeLayout(false);
			fScanPanel.PerformLayout();
			if (fScanItemViewerSplitter.Visible)
				fScanItemViewerSplitter.SplitPosition = RestoreIntProperty(ScanItemViewerSplitter_Name, 0);
			else
				SaveIntProperty(ScanItemViewerSplitter_Name, fScanItemViewerSplitter.SplitPosition);
		}
		protected void OnShowPageUrlButtonClick(object sender, EventArgs e) {
			BeginUpdateAppearance();
			try {
				List<TreeNode> treeNodeList = DesignerUtils.GetAllTreeNodes(fScanTreeView);
				fScanTreeView.BeginUpdate();
				for (int i = 0; i < treeNodeList.Count; i++ )
					treeNodeList[i].Text = GetItemText(treeNodeList[i].Tag);
			}
			finally {
				fScanTreeView.EndUpdate();
				EndUpdateAppearance();
			}
		}
		protected void RefreshItemViewer() {
			fItemsTreeView.BeginUpdate();
			List<TreeNode> treeNodeList = DesignerUtils.GetAllTreeNodes(fItemsTreeView);
			foreach (TreeNode treeNode in treeNodeList) {
				treeNode.ForeColor = Color.Empty;
				string url = (treeNode.Tag as SiteMapNode).Url;
				if (fSiteMapControlProvider.UrlInfo.IsDuplicatedString(url) ||
					!fSiteMapControlProvider.UrlInfo.IsValidUrl(url)) {
					treeNode.ForeColor = Color.Red;
					ExpandNode(treeNode);
				}
			}
			fItemsTreeView.EndUpdate();
		}
		protected void RefreshScanPanel() {
			if (fSiteMapScanner.SiteMapProvider.RootNode.HasChildNodes) {
				fScanMessageLabel.Text = StringResources.SiteMapControlEditor_NewNodesMessage;
			}
			else { 
				fScanMessageLabel.Text = "";
				fScanTreeView.Nodes.Clear();
				fScanTreeView.Update();
			}
		}
		protected void Save() {
			SMPXmlWriter xmlWriter = new SMPXmlWriter(fSiteMapControlProvider);
			string contentString = xmlWriter.GetFormattedContentString();
			DTE dte = (DTE)ServiceProvider.GetService(typeof(DTE));
			if (fSiteMapFileProjectItem == null)
				fSiteMapFileProjectItem = GetSiteMapFileProjectItem();
			ProjectItemState projItemState = EnvDTEHelper.GetProjectItemState(fSiteMapFileProjectItem);
			fSiteMapFileProjectItem.Open(Constants.vsViewKindPrimary).Activate();
			TextDocument textDocument = (TextDocument)dte.ActiveDocument.Object("TextDocument");
			EnvDTEHelper.InsertTextToDocument(textDocument, contentString);
			fSiteMapFileProjectItem.Save("");
			EnvDTEHelper.SetStateForProjectItem(fSiteMapFileProjectItem, projItemState);
			ComponentChanged(false);
		}
		protected void ScanWebApp(TreeView scanTreeView) {
			BeginUpdateAppearance();
			try {
				fSiteMapScanner.SiteMapProvider.Clear();
				Project curProject = GetCurrentProjectObject() as Project;
				if (curProject != null) {
					fSiteMapScanner.ScanSiteMap(curProject, fSiteMapControlProvider, WebApplication);
					if (fSiteMapScanner.SiteMapProvider.RootNode.HasChildNodes)
						FillTreeView(fScanTreeView, new SiteMapNodeCollection(fSiteMapScanner.SiteMapProvider.RootNode));
					RefreshScanPanel();
				}
			}
			finally {
				EndUpdateAppearance();
			}
		}
		private void AddScanPanelToPanel(Panel panel) {
			fScanPanel = new Panel();
			fScanPanel.Visible = false;
			fScanPanel.Dock = DockStyle.Right;
			fScanItemViewerSplitter = new Splitter();
			fScanItemViewerSplitter.Dock = DockStyle.Right;
			fScanItemViewerSplitter.Visible = false;
			fScanItemViewerSplitter.MinExtra = LeftPanelMinimizeWidth;
			fScanItemViewerSplitter.MinSize = ScanPanelMinimumWidth;
			fScanTreeView = new TreeView();
			fScanTreeView.AllowDrop = true;
			fScanTreeView.DragDrop += new DragEventHandler(OnDragDropScanTreeView);
			fScanTreeView.DragOver += new DragEventHandler(OnDragOverTreeView);
			fScanTreeView.ItemDrag += new ItemDragEventHandler(OnItemDragTreeView);
			fScanTreeView.GiveFeedback += new GiveFeedbackEventHandler(OnGiveFeedbackTreeView);
			fScanTreeView.Dock = DockStyle.Fill;
			fScanMessagePanel = new PanelEx();
			fScanMessagePanel.Padding = new Padding(MessageLabelPadding);
			fScanMessagePanel.Dock = DockStyle.Bottom;
			fScanMessagePanel.Height = ScanMessagePanelHeight;
			fScanMessageLabel = new Label();
			fScanMessageLabel.Dock = DockStyle.Fill;
			fScanMessagePanel.Controls.Add(fScanMessageLabel);
			fScanMessagePanelSplit = new Splitter();
			fScanMessagePanelSplit.Enabled = false;
			fScanMessagePanelSplit.Dock = DockStyle.Bottom;
			ToolStrip scanPanelToolStrip = new ToolStrip();
			scanPanelToolStrip.RenderMode = ToolStripRenderMode.System;
			scanPanelToolStrip.CanOverflow = false;
			scanPanelToolStrip.GripStyle = ToolStripGripStyle.Hidden;
			scanPanelToolStrip.Dock = DockStyle.Top;
			scanPanelToolStrip.Items.AddRange(GetScanToolStripButtons());			
			Label newPagedLabel = ControlCreator.CreateLabel("Not-in-sitemap Pages:");
			fScanPanel.Controls.Add(fScanTreeView);
			fScanPanel.Controls.Add(scanPanelToolStrip);
			fScanPanel.Controls.Add(fScanMessagePanelSplit);
			fScanPanel.Controls.Add(fScanMessagePanel);
			fScanPanel.Controls.Add(newPagedLabel);
			panel.Controls.Add(fScanItemViewerSplitter);
			panel.Controls.Add(fScanPanel);
		}
		private void ClearScanData() {
			fScanTreeView.Nodes.Clear();
			fSiteMapScanner.SiteMapProvider.Clear();
		}
		private SiteMapNode CloneSiteMapNodeHierarchy(SiteMapNode rootNode, EditableSiteMapProvider destProvider) {
			SiteMapNode ret = destProvider.CloneSiteMapNode(rootNode);
			TypeDescriptor.AddAttributes(ret, new Attribute[] { new TypeConverterAttribute(typeof(SiteMapNodeTypeConverter)) });
			if (rootNode.HasChildNodes) {
				foreach (SiteMapNode childNode in rootNode.ChildNodes)
					destProvider.AddSiteMapNode(CloneSiteMapNodeHierarchy(childNode, destProvider), ret);
			}
			return ret;
		}
		private Bitmap CreateImageForButton(string imageResource) {
			Bitmap image = CreateBitmapFromResources(imageResource, typeof(SiteMapNodesEditorForm).Assembly);
			image.MakeTransparent(Color.Magenta);
			return image;
		}
		private void ExpandNode(TreeNode node) {
			while (node.Parent != null) {
				node.Parent.Expand();
				node = node.Parent;
			}
		}
		private object GetCurrentProjectObject() {
			DTE dte = (DTE)ServiceProvider.GetService(typeof(DTE));
			ProjectItem currentProjectItem = (ProjectItem)ServiceProvider.GetService(typeof(ProjectItem));
			if (currentProjectItem != null)
				return currentProjectItem.ContainingProject;
			else
				return null;
		}
		private EnvDTE.ProjectItem GetSiteMapFileProjectItem() {
			DTE dte = (DTE)ServiceProvider.GetService(typeof(DTE));
			IProjectItem iProjectItem = WebApplication.GetProjectItemFromUrl(fSiteMapFileName);
			EnvDTE.ProjectItem curProjectItem = (EnvDTE.ProjectItem)ServiceProvider.GetService(typeof(EnvDTE.ProjectItem));
			if (IsExistSiteMapFile())
				return dte.Solution.FindProjectItem(iProjectItem.PhysicalPath);
			else {
				object newProjectItemObj = DesignerUtils.AddNewSiteMapFileToProject(curProjectItem.ContainingProject, 
					WebApplication, fSiteMapFileName, false);
				return newProjectItemObj as EnvDTE.ProjectItem;
			}
		}
		private bool IsExistSiteMapFile() {
			return (Designer as ASPxSiteMapDataSourceDesigner).IsExistSiteMapFile();			
		}
	}
}
