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
using System.Windows.Forms;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars.Localization;
using DevExpress.XtraBars.Ribbon.Customization;
using DevExpress.XtraEditors;
namespace DevExpress.XtraBars.Ribbon {
	[ToolboxItem(false)]
	public partial class RibbonCustomizationUserControl : XtraUserControl, IParentUserControl, IContextCommandProcessor {
		public RibbonCustomizationUserControl() : this(null) { }
		public RibbonCustomizationUserControl(ICustomizationTopForm topForm) {
			this.TopForm = topForm;
			InitializeComponent();
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			InitDialog();
		}
		#region Init
		protected virtual void InitDialog() {
			InitFields();
			InitDialogBase();
			InitLeftTreeViewDefaultView();
			InitRightTreeView();
			InitContextMenu();
		}
		DXPopupMenu menu;
		protected virtual void InitContextMenu(){
			menu = ContextMenuManager.CreateContextMenu(Commands);
		}
		protected virtual void InitFields() {
			Commands = new CustomizationCommands(this, RightTreeView, LeftTreeView, TopForm != null ? TopForm.RibbonControl : null);
			ContextMenuManager = new ContextMenuManager(this);
			ContextMenuManager.ForceCreateItems();
		}
		protected virtual void InitDialogBase() {
			InitOptionsComboBox();
			InitFilterComboBox();
			InitDropDownButtons();
		}
		protected virtual void InitOptionsComboBox() {
			OptionComboBoxEdit.Properties.Items.Add(LeftPaneOptionBase.AllTabs);
			OptionComboBoxEdit.Properties.Items.Add(LeftPaneOptionBase.AllCommands);
			OptionComboBoxEdit.EditValue = LeftPaneOptionBase.AllTabs;
		}
		protected virtual void InitFilterComboBox() {
			FilterComboBoxEdit.Properties.Items.Add(RightPaneOptionBase.AllTabs);
			FilterComboBoxEdit.EditValue = RightPaneOptionBase.AllTabs;
		}
		protected virtual void InitDropDownButtons() {
			if(TopForm == null)
				return;
			ResetOptionsDropDownButton.DropDownControl = ContextMenuManager.ResetOptionsPopupMenu;
			ExportOptionsDropDownButton.DropDownControl = ContextMenuManager.ExportOptionsPopupMenu;
			NewOptionsDropDownButton.DropDownControl = ContextMenuManager.NewOptionsPopupMenu;
			ResetOptionsDropDownButton.MenuManager = ExportOptionsDropDownButton.MenuManager = NewOptionsDropDownButton.MenuManager = TopForm.BarManager;
		}
		protected virtual void InitLeftTreeViewDefaultView() {
			PrepareLeftTreeViewDefaultViewOptions();
			InitTreeViewCore(LeftTreeView);
		}
		protected virtual void InitLeftTreeViewCommandsView() {
			PrepareLeftTreeViewCommandsViewOptions();
			LeftTreeView.CreateTree();
		}
		protected virtual void PrepareLeftTreeViewDefaultViewOptions() {
			LeftTreeView.ShowRootLines = true;
		}
		protected virtual void PrepareLeftTreeViewCommandsViewOptions() {
			LeftTreeView.ShowRootLines = false;
		}
		protected virtual void InitRightTreeView() {
			InitTreeViewCore(RightTreeView);
			SelectFirstChildNodeOnRightTreeView();
		}
		protected void SelectFirstChildNodeOnRightTreeView() {
			if(RightTreeView.Nodes.Count == 0)
				return;
			TreeNode topNode = RightTreeView.Nodes[0];
			if(topNode.Nodes.Count == 0) {
				RightTreeView.SelectedNode = topNode;
				return;
			}
			TreeNode node = topNode.Nodes[0];
			RightTreeView.SelectedNode = node;
			node.Expand();
		}
		protected virtual void InitTreeViewCore(RunTimeRibbonTreeView treeView) {
			if(TopForm == null || RibbonControl == null)
				return;
			treeView.Ribbon = RibbonControl;
			treeView.CreateTree();
			foreach(TreeNode node in treeView.Nodes) {
				node.Expand();
			}
		}
		protected virtual void ApplyAllTabsLeftPaneOption() {
			if(LeftTreeView.ViewType == RunTimeRibbonTreeViewOriginalView.TreeViewType.Default)
				return;
			LeftTreeView.ViewType = RunTimeRibbonTreeViewOriginalView.TreeViewType.Default;
			InitLeftTreeViewDefaultView();
		}
		protected virtual void ApplyAllCommandsLeftPaneOption() {
			if(LeftTreeView.ViewType == RunTimeRibbonTreeViewOriginalView.TreeViewType.Commands)
				return;
			LeftTreeView.ViewType = RunTimeRibbonTreeViewOriginalView.TreeViewType.Commands;
			InitLeftTreeViewCommandsView();
		}
		#endregion
		#region Refresh State
		protected virtual void RefreshButtons() {
			RefreshUpButtonCore();
			RefreshDownButtonCore();
			RefreshRenameButtonCore();
			RefreshAddButtonCore();
			RefreshRemoveButtonCore();
			RefreshNewCategoryButtonCore();
			RefreshNewTabButtonCore();
			RefreshNewGroupButtonCore();
			RefreshResetOnlySelectedTabPopupMenuItem();
		}
		protected virtual bool CheckCommandAccessibility(CustomizationStrategyBase.Strategy cmd, TreeNode node) {
			return CheckCommandAccessibility(cmd, node, null);
		}
		protected virtual bool CheckCommandAccessibility(CustomizationStrategyBase.Strategy cmd, TreeNode node, TreeNode sourceNode) {
			if(node == null)
				return false;
			CustomizationStrategyBase strategy = CustomizationStrategyBase.Create(cmd, Commands);
			return strategy.ShouldProcessCommand(node, sourceNode);
		}
		protected virtual void RefreshUpButtonCore() {
			UpButton.Enabled = CheckCommandAccessibility(CustomizationStrategyBase.Strategy.MoveItemUp, RightTreeViewSelNode);
		}
		protected virtual void RefreshDownButtonCore() {
			DownButton.Enabled = CheckCommandAccessibility(CustomizationStrategyBase.Strategy.MoveItemDown, RightTreeViewSelNode);
		}
		public virtual void RefreshRenameButtonCore() {
			RenameButton.Enabled = CheckCommandAccessibility(CustomizationStrategyBase.Strategy.Rename, RightTreeViewSelNode);
		}
		protected virtual void RefreshAddButtonCore() {
			if(LeftTreeViewSelNode == null || RightTreeViewSelNode == null) {
				AddButton.Enabled = false;
				return;
			}
			AddButton.Enabled = CheckCommandAccessibility(CustomizationStrategyBase.Strategy.MoveItemToRight, RightTreeViewSelNode, LeftTreeViewSelNode);
		}
		protected virtual void RefreshRemoveButtonCore() {
			RemoveButton.Enabled = CheckCommandAccessibility(CustomizationStrategyBase.Strategy.RemoveItem, RightTreeViewSelNode);
		}
		protected virtual void RefreshNewCategoryButtonCore() {
			ContextMenuManager.NewCategoryItem.Enabled = CheckCommandAccessibility(CustomizationStrategyBase.Strategy.AddNewCategory, RightTreeViewSelNode);
		}
		protected virtual void RefreshNewTabButtonCore() {
			ContextMenuManager.NewPageItem.Enabled = CheckCommandAccessibility(CustomizationStrategyBase.Strategy.AddNewTab, RightTreeViewSelNode);
		}
		protected virtual void RefreshNewGroupButtonCore() {
			ContextMenuManager.NewGroupItem.Enabled = CheckCommandAccessibility(CustomizationStrategyBase.Strategy.AddNewGroup, RightTreeViewSelNode);
		}
		protected virtual void RefreshResetOnlySelectedTabPopupMenuItem() {
			ContextMenuManager.ResetSelectedPageSettingsItem.Enabled = CheckCommandAccessibility(CustomizationStrategyBase.Strategy.ResetSelectedPageOptions, RightTreeViewSelNode);
		}
		#endregion
		#region IParentUserControl
		public RibbonCustomizationModel GetSerializationModel() {
			return TopForm.GetResult();
		}
		public void ApplyCustomizationSettings(RibbonCustomizationModel model) {
			RibbonControl.ApplyCustomizationSettings(model);
			ReloadForm();
		}
		public void ReloadForm() {
			InitLeftTreeViewDefaultView();
			InitRightTreeView();
		}
		public void ResetRibbon() {
			TopForm.ResetRibbon();
		}
		#endregion
		#region IContextCommandProcessor
		public void NewCategory() {
			Commands.AddNewCategory();
			RefreshButtons();
		}
		public void NewPage() {
			Commands.AddNewTab();
			RefreshButtons();
		}
		public void NewGroup() {
			Commands.AddNewGroup();
			RefreshButtons();
		}
		public void ResetSelectedPageOptions() {
			Commands.ResetSelectedPageOptions();
		}
		public void ResetAllOptions() {
			Commands.ResetAllOptions();
		}
		public void Rename() {
			Commands.Rename();
		}
		public void Remove() {
			Commands.RemoveItem();
		}
		public void MoveItemToRight() {
			Commands.MoveItemToRight();
		}
		public void MoveItemDown() {
			Commands.MoveItemToDown();
		}
		public void MoveItemUp() {
			Commands.MoveItemToUp();
		}
		public void RefreshContextMenu(CustomizationItemInfoBase item) {
			ContextMenuManager.BeginInit();
			try {
				RefreshContextMenuCore(item);
			}
			finally {
				ContextMenuManager.EndInit();
			}
		}
		protected virtual void RefreshContextMenuCore(CustomizationItemInfoBase item){
			bool allowLinkCustomization = RibbonControl.OptionsCustomizationForm.AllowLinkCustomization;
			menu.Items[0].Enabled = CheckCommandAccessibility(CustomizationStrategyBase.Strategy.AddNewCategory, RightTreeViewSelNode);
			menu.Items[1].Enabled = CheckCommandAccessibility(CustomizationStrategyBase.Strategy.AddNewTab, RightTreeViewSelNode);
			menu.Items[2].Enabled = CheckCommandAccessibility(CustomizationStrategyBase.Strategy.AddNewGroup, RightTreeViewSelNode);
			menu.Items[3].Enabled = CheckCommandAccessibility(CustomizationStrategyBase.Strategy.Rename, RightTreeViewSelNode);
			menu.Items[4].Enabled = CheckCommandAccessibility(CustomizationStrategyBase.Strategy.RemoveItem, RightTreeViewSelNode);
			if(LeftTreeViewSelNode == null || RightTreeViewSelNode == null) {
				menu.Items[5].Enabled = false;
			}
			else {
				menu.Items[5].Enabled = CheckCommandAccessibility(CustomizationStrategyBase.Strategy.MoveItemToRight, RightTreeViewSelNode, LeftTreeViewSelNode);
			}
			menu.Items[6].Enabled = CheckCommandAccessibility(CustomizationStrategyBase.Strategy.MoveItemDown, RightTreeViewSelNode);
			menu.Items[7].Enabled = CheckCommandAccessibility(CustomizationStrategyBase.Strategy.MoveItemUp, RightTreeViewSelNode);
			DXMenuCheckItem updateLinkVisibilityItem = (DXMenuCheckItem)menu.Items[8];
			updateLinkVisibilityItem.Visible = allowLinkCustomization && item is RibbonItemLinkInfo;
			if(updateLinkVisibilityItem.Visible) {
				updateLinkVisibilityItem.Checked = ((RibbonItemLinkInfo)item).ItemLink.Visible;
				updateLinkVisibilityItem.Tag = item;
			}
		}
		public void ImportSettings() {
			if(TopForm.OpenDialog.ShowDialog() != DialogResult.OK)
				return;
			Commands.ImportSettingsFromXML(TopForm.OpenDialog.FileName);
		}
		public void ExportSettings() {
			if(TopForm.SaveDialog.ShowDialog(this) != DialogResult.OK)
				return;
			Commands.ExportSettingsToXML(TopForm.SaveDialog.FileName);
		}
		#endregion
		#region Handlers
		void btnRename_Click(object sender, EventArgs e) {
			Commands.Rename();
			RefreshButtons();
		}
		void rightTreeView_AfterSelect(object sender, TreeViewEventArgs e) {
			RefreshButtons();
		}
		void btnAddItem_Click(object sender, EventArgs e) {
			Commands.MoveItemToRight();
			RefreshButtons();
		}
		void btnRemoveItem_Click(object sender, EventArgs e) {
			Commands.RemoveItem();
			RefreshButtons();
		}
		void btnUp_Click(object sender, EventArgs e) {
			Commands.MoveItemToUp();
			RefreshButtons();
		}
		void btnDown_Click(object sender, EventArgs e) {
			Commands.MoveItemToDown();
			RefreshButtons();
		}
		void commands_SelectedValueChanged(object sender, EventArgs e) {
			ComboBoxEdit edit = sender as ComboBoxEdit;
			if(edit.EditValue is AllTabsLeftPaneOption)
				ApplyAllTabsLeftPaneOption();
			else if(edit.EditValue is AllCommandsLeftPaneOption)
				ApplyAllCommandsLeftPaneOption();
			RefreshButtons();
		}
		#endregion
		protected ICustomizationTopForm TopForm { get; private set; }
		protected RibbonControl RibbonControl { get { return TopForm.RibbonControl; } }
		protected CustomizationCommands Commands { get; private set; }
		protected ContextMenuManager ContextMenuManager { get; private set; }
		public RunTimeRibbonTreeViewOriginalView LeftTreeView { get { return this.leftTreeView; } }
		public RunTimeRibbonTreeView RightTreeView { get { return this.rightTreeView; } }
		public DropDownButton ResetOptionsDropDownButton { get { return TopForm.ResetRibbonDropDownButton; } }
		public DropDownButton ExportOptionsDropDownButton { get { return this.exportOptionsDropDownButton; } }
		public DropDownButton NewOptionsDropDownButton { get { return this.newOptionsDropDownButton; } }
		public ComboBoxEdit OptionComboBoxEdit { get { return this.cbeCommands; } }
		public ComboBoxEdit FilterComboBoxEdit { get { return this.cbeFilter; } }
		protected TreeNode LeftTreeViewSelNode { get { return LeftTreeView.SelectedNode; } }
		protected TreeNode RightTreeViewSelNode { get { return RightTreeView.SelectedNode; } }
		protected SimpleButton UpButton { get { return this.btnUp; } }
		protected SimpleButton DownButton { get { return this.btnDown; } }
		protected SimpleButton RenameButton { get { return this.btnRename; } }
		protected SimpleButton AddButton { get { return this.btnAddItem; } }
		protected SimpleButton RemoveButton { get { return this.btnRemoveItem; } }
		private void rightTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e) {
			if(e.Button == MouseButtons.Right) {
				((TreeView)sender).SelectedNode = e.Node;
			}
		}
		private void rightTreeView_MouseUp(object sender, MouseEventArgs e) {
			if(e.Button != MouseButtons.Right) return;
			TreeView treeView = (TreeView)sender;
			RefreshContextMenu(CustomizationHelperBase.FromNode(treeView.SelectedNode));
			((IDXDropDownControl)menu).Show(RibbonControl.Manager, null, Control.MousePosition);
		}
	}
	#region Options
	class OptionBase { }
	class LeftPaneOptionBase : OptionBase {
		static LeftPaneOptionBase() {
			AllTabs = new AllTabsLeftPaneOption();
			AllCommands = new AllCommandsLeftPaneOption();
		}
		public static AllTabsLeftPaneOption AllTabs { get; private set; }
		public static AllCommandsLeftPaneOption AllCommands { get; private set; }
	}
	class AllTabsLeftPaneOption : LeftPaneOptionBase {
		public override string ToString() {
			return BarLocalizer.Active.GetLocalizedString(BarString.RibbonCustomizationOptionAllTabs);
		}
	}
	class AllCommandsLeftPaneOption : LeftPaneOptionBase {
		public override string ToString() {
			return BarLocalizer.Active.GetLocalizedString(BarString.RibbonCustomizationOptionAllCommands);
		}
	}
	class RightPaneOptionBase : OptionBase {
		static RightPaneOptionBase() {
			AllTabs = new AllTabsRightPaneOption();
		}
		public static AllTabsRightPaneOption AllTabs { get; private set; }
	}
	class AllTabsRightPaneOption : RightPaneOptionBase {
		public override string ToString() {
			return BarLocalizer.Active.GetLocalizedString(BarString.RibbonCustomizationOptionAllTabs);
		}
	}
	#endregion
	#region Context Menus
	public interface IContextCommandProcessor {
		void NewCategory();
		void NewPage();
		void NewGroup();
		void ResetSelectedPageOptions();
		void ResetAllOptions();
		void ImportSettings();
		void ExportSettings();
		void Rename();
		void Remove();
		void MoveItemToRight();
		void MoveItemDown();
		void MoveItemUp();
	}
	public class ContextMenuManager : IDisposable {
		public ContextMenuManager(IContextCommandProcessor context) {
			this.Context = context;
		}
		~ContextMenuManager() {
			Dispose(false);
		}
		public DXPopupMenu ResetOptionsPopupMenu {
			get {
				if(ResetOptionsMenuCore == null) ResetOptionsMenuCore = CreateResetOptionsPopupMenu();
				return ResetOptionsMenuCore;
			}
		}
		public DXPopupMenu ExportOptionsPopupMenu {
			get {
				if(ExportOptionsMenuCore == null) ExportOptionsMenuCore = CreateExportPopupMenu();
				return ExportOptionsMenuCore;
			}
		}
		public DXPopupMenu NewOptionsPopupMenu {
			get {
				if(NewOptionsPopupMenuCore == null) NewOptionsPopupMenuCore = CreateNewPopupMenu();
				return NewOptionsPopupMenuCore;
			}
		}
		public void ForceCreateItems() {
			object popup = null;
			popup = ResetOptionsPopupMenu;
			popup = ExportOptionsPopupMenu;
			popup = NewOptionsPopupMenu;
		}
		public DXPopupMenu CreateContextMenu(CustomizationCommands commands){
			DXPopupMenu menu = new DXPopupMenu();
			menu.Items.Add(CreateNewCategoryMenuItem());
			menu.Items.Add(CreateNewPageMenuItem());
			menu.Items.Add(CreateNewGroupMenuItem());
			menu.Items.Add(CreateRenameItem());
			menu.Items.Add(CreateRemoveItem());
			menu.Items.Add(CreateMoveToRightItem());
			menu.Items.Add(CreateMoveDownItem());
			menu.Items.Add(CreateMoveUpItem());
			menu.Items.Add(CreateUpdateLinkVisibilityItem());
			return menu;
		}
		bool menuInitialization = false;
		public void BeginInit() {
			this.menuInitialization = true;
		}
		public void EndInit() {
			this.menuInitialization = false;
		}
		protected virtual DXPopupMenu CreateResetOptionsPopupMenu() {
			DXPopupMenu popupMenu = new DXPopupMenu();
			popupMenu.Items.Add(ResetSelectedPageSettingsItem = CreateResetSelectedItemSettingsMenuItem());
			popupMenu.Items.Add(ResetAllPageSettingsItem = CreateResetAllSettingsMenuItem());
			return popupMenu;
		}
		protected virtual DXPopupMenu CreateExportPopupMenu() {
			DXPopupMenu popupMenu = new DXPopupMenu();
			popupMenu.Items.Add(CreateImportSettingsMenuItem());
			popupMenu.Items.Add(CreateExportSettingsMenuItem());
			return popupMenu;
		}
		protected virtual DXPopupMenu CreateNewPopupMenu() {
			DXPopupMenu popupMenu = new DXPopupMenu();
			popupMenu.Items.Add(NewCategoryItem = CreateNewCategoryMenuItem());
			popupMenu.Items.Add(NewPageItem = CreateNewPageMenuItem());
			popupMenu.Items.Add(NewGroupItem = CreateNewGroupMenuItem());
			return popupMenu;
		}
		#region Handlers
		void ResetSelectedPageSettingsHandler(object sender, EventArgs e) {
			Context.ResetSelectedPageOptions();
		}
		void ResetAllSettingsHandler(object sender, EventArgs e) {
			Context.ResetAllOptions();
		}
		void ImportSettingsHandler(object sender, EventArgs e) {
			Context.ImportSettings();
		}
		void ExportSettingsHandler(object sender, EventArgs e) {
			Context.ExportSettings();
		}
		void CreateNewCategoryHandler(object sender, EventArgs e) {
			Context.NewCategory();
		}
		void CreateNewPageHandler(object sender, EventArgs e) {
			Context.NewPage();
		}
		void CreateNewGroupHandler(object sender, EventArgs e) {
			Context.NewGroup();
		}
		void CreateRenameHandler(object sender, EventArgs e) {
			Context.Rename();
		}
		void CreateRemoveHandler(object sender, EventArgs e) {
			Context.Remove();
		}
		void CreateMoveToRightHandler(object sender, EventArgs e) {
			Context.MoveItemToRight();
		}
		void CreateMoveDownHandler(object sender, EventArgs e) {
			Context.MoveItemDown();
		}
		void CreateMoveUpHandler(object sender, EventArgs e) {
			Context.MoveItemUp();
		}
		void UpdateLinkVisibilityHandler(object sender, EventArgs e) {
			if(this.menuInitialization) return;
			DXMenuCheckItem item = (DXMenuCheckItem)sender;
			RibbonItemLinkInfo linkInfo = (RibbonItemLinkInfo)item.Tag;
			linkInfo.ItemLink.Visible = item.Checked;
		}
		#endregion
		#region Init Code
		protected virtual DXMenuItem CreateResetSelectedItemSettingsMenuItem() {
			return CreateMenuItemCore(BarString.RibbonCustomizationResetSelectedTabSettingsCommand, ResetSelectedPageSettingsHandler);
		}
		protected virtual DXMenuItem CreateResetAllSettingsMenuItem() {
			return CreateMenuItemCore(BarString.RibbonCustomizationResetSettingsCommand, ResetAllSettingsHandler);
		}
		protected virtual DXMenuItem CreateImportSettingsMenuItem() {
			return CreateMenuItemCore(BarString.RibbonCustomizationImportSettingsCommand, ImportSettingsHandler);
		}
		protected virtual DXMenuItem CreateExportSettingsMenuItem() {
			return CreateMenuItemCore(BarString.RibbonCustomizationExportSettingsCommand, ExportSettingsHandler);
		}
		protected virtual DXMenuItem CreateNewCategoryMenuItem() {
			return CreateMenuItemCore(BarString.RibbonCustomizationNewCategoryCommand, CreateNewCategoryHandler);
		}
		protected virtual DXMenuItem CreateNewPageMenuItem() {
			return CreateMenuItemCore(BarString.RibbonCustomizationNewPageCommand, CreateNewPageHandler);
		}
		protected virtual DXMenuItem CreateNewGroupMenuItem() {
			return CreateMenuItemCore(BarString.RibbonCustomizationNewGroupCommand, CreateNewGroupHandler);
		}
		protected virtual DXMenuItem CreateRenameItem() {
			return CreateMenuItemCore(BarString.RibbonCustomizationRenameText, CreateRenameHandler);
		}
		protected virtual DXMenuItem CreateRemoveItem() {
			return CreateMenuItemCore(BarString.RibbonCustomizationRemoveText, CreateRemoveHandler);
		}
		protected virtual DXMenuItem CreateMoveToRightItem() {
			return CreateMenuItemCore(BarString.RibbonCustomizationAddText, CreateMoveToRightHandler);
		}
		protected virtual DXMenuItem CreateMoveDownItem() {
			return CreateMenuItemCore(BarString.RibbonCustomizationDownText, CreateMoveDownHandler);
		}
		protected virtual DXMenuItem CreateMoveUpItem() {
			return CreateMenuItemCore(BarString.RibbonCustomizationUpText, CreateMoveUpHandler);
		}
		protected virtual DXMenuItem CreateUpdateLinkVisibilityItem() {
			return CreateMenuCheckItemCore(BarString.Visible, UpdateLinkVisibilityHandler, true);
		}
		protected virtual DXMenuItem CreateMenuItemCore(BarString id, EventHandler handler) {
			return new DXMenuItem(BarLocalizer.Active.GetLocalizedString(id), handler);
		}
		protected virtual DXMenuItem CreateMenuCheckItemCore(BarString id, EventHandler handler, bool beginGroup) {
			DXMenuCheckItem item = new DXMenuCheckItem(BarLocalizer.Active.GetLocalizedString(id), false, null, handler);
			item.BeginGroup = beginGroup;
			return item;
		}
		#endregion
		public DXMenuItem ResetSelectedPageSettingsItem { get; private set; }
		public DXMenuItem ResetAllPageSettingsItem { get; private set; }
		public DXMenuItem NewCategoryItem { get; private set; }
		public DXMenuItem NewPageItem { get; private set; }
		public DXMenuItem NewGroupItem { get; private set; }
		public DXMenuItem Rename { get; private set; }
		public DXMenuItem Remove { get; private set; }
		public DXMenuItem MoveToRight { get; private set; }
		public DXMenuItem MoveDown { get; private set; }
		public DXMenuItem MoveUP { get; private set; }
		protected internal DXPopupMenu ResetOptionsMenuCore { get; set; }
		protected internal DXPopupMenu ExportOptionsMenuCore { get; set; }
		protected internal DXPopupMenu NewOptionsPopupMenuCore { get; set; }
		protected internal IContextCommandProcessor Context { get; private set; }
		#region IDisposable Members
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				if(ResetOptionsMenuCore != null)
					ResetOptionsMenuCore.Dispose();
				if(ExportOptionsMenuCore != null)
					ExportOptionsMenuCore.Dispose();
				if(NewOptionsPopupMenuCore != null)
					NewOptionsPopupMenuCore.Dispose();
			}
			Context = null;
			ResetOptionsMenuCore = ExportOptionsMenuCore = NewOptionsPopupMenuCore = null;
		}
	}
	#endregion
	#region Customization Data Provider
	public interface IParentUserControl {
		void ReloadForm();
		void ResetRibbon();
		RibbonCustomizationModel GetSerializationModel();
		void ApplyCustomizationSettings(RibbonCustomizationModel model);
	}
	public interface ICustomizationInfoProvider {
		void ReloadForm();
		void ResetRibbon();
		RibbonControl RibbonControl { get; }
		RunTimeRibbonTreeView TreeView { get; }
		RunTimeRibbonTreeView SourceTreeView { get; }
		TreeNode SelectedNode { get; }
		TreeNode SourceNode { get; }
	}
	public class CustomizationCommands : ICustomizationInfoProvider {
		IParentUserControl parentForm;
		RunTimeRibbonTreeView treeView;
		RunTimeRibbonTreeView sourceTreeView;
		RibbonSourceStateInfo ribbonSourceStateInfo;
		RibbonControl ribbonControl;
		public CustomizationCommands(IParentUserControl parentForm, RunTimeRibbonTreeView treeView, RunTimeRibbonTreeView sourceTreeView, RibbonControl ribbonControl) {
			this.treeView = treeView;
			this.sourceTreeView = sourceTreeView;
			this.parentForm = parentForm;
			this.ribbonSourceStateInfo = RibbonCustomizationForm.RibbonSourceStateInfo;
			this.ribbonControl = ribbonControl;
		}
		public void AddNewCategory() {
			Cusomize(CustomizationStrategyBase.Strategy.AddNewCategory);
		}
		public void AddNewTab() {
			Cusomize(CustomizationStrategyBase.Strategy.AddNewTab);
		}
		public void AddNewGroup() {
			Cusomize(CustomizationStrategyBase.Strategy.AddNewGroup);
		}
		public void Rename() {
			Cusomize(CustomizationStrategyBase.Strategy.Rename);
		}
		public void MoveItemToRight() {
			Cusomize(CustomizationStrategyBase.Strategy.MoveItemToRight);
		}
		public void RemoveItem() {
			Cusomize(CustomizationStrategyBase.Strategy.RemoveItem);
		}
		public void MoveItemToUp() {
			Cusomize(CustomizationStrategyBase.Strategy.MoveItemUp);
		}
		public void MoveItemToDown() {
			Cusomize(CustomizationStrategyBase.Strategy.MoveItemDown);
		}
		public void ResetSelectedPageOptions() {
			Cusomize(CustomizationStrategyBase.Strategy.ResetSelectedPageOptions);
		}
		public void ResetAllOptions() {
			Cusomize(CustomizationStrategyBase.Strategy.ResetAllOptions);
		}
		public void ImportSettingsFromXML(string path) {
			RibbonCustomizationModel model = RibbonCustomizationSerializationHelper.Deserialize(path, RibbonControl);
			if(model == null)
				return;
			parentForm.ApplyCustomizationSettings(model);
		}
		public void ExportSettingsToXML(string path) {
			RibbonCustomizationModel model = parentForm.GetSerializationModel();
			RibbonCustomizationSerializationHelper.Serialize(model, path, RibbonControl);
		}
		protected void Cusomize(CustomizationStrategyBase.Strategy usage) {
			CustomizationStrategyBase obj = CustomizationStrategyBase.Create(usage, this);
			if(obj != null) obj.Customize();
		}
		#region ICustomizationInfoProvider
		public void ReloadForm() {
			parentForm.ReloadForm();
		}
		public void ResetRibbon() {
			parentForm.ResetRibbon();
		}
		public RunTimeRibbonTreeView TreeView {
			get { return this.treeView; }
		}
		public RunTimeRibbonTreeView SourceTreeView {
			get { return this.sourceTreeView; }
		}
		public RibbonControl RibbonControl {
			get { return this.ribbonControl; }
		}
		public TreeNode SelectedNode {
			get { return TreeView.SelectedNode; }
		}
		public TreeNode SourceNode {
			get { return SourceTreeView.SelectedNode; }
		}
		#endregion
	}
	#endregion
	#region Strategy Base
	public abstract class CustomizationStrategyBase {
		public enum Strategy { AddNewCategory, AddNewTab, AddNewGroup, Rename, MoveItemToRight, RemoveItem, MoveItemUp, MoveItemDown, ResetSelectedPageOptions, ResetAllOptions }
		public CustomizationStrategyBase(ICustomizationInfoProvider info) {
			this.Info = info;
		}
		public static CustomizationStrategyBase Create(Strategy usage, ICustomizationInfoProvider info) {
			switch(usage) {
				case Strategy.AddNewCategory:
					return new CustomizationStrategyAddNewCategory(info);
				case Strategy.AddNewTab:
					return new CustomizationStrategyAddNewTab(info);
				case Strategy.AddNewGroup:
					return new CustomizationStrategyAddNewGroup(info);
				case Strategy.Rename:
					return new CustomizationStrategyRename(info);
				case Strategy.MoveItemToRight:
					return CustomizationStrategyMoveToRightBase.Create(info);
				case Strategy.RemoveItem:
					return new CustomizationStrategyRemoveItem(info);
				case Strategy.MoveItemUp:
					return CustomizationStrategyMoveItemUpBase.Create(info);
				case Strategy.MoveItemDown:
					return CustomizationStrategyMoveItemDownBase.Create(info);
				case Strategy.ResetSelectedPageOptions:
					return new CustomizationStrategyResetSelectedPageOptions(info);
				case Strategy.ResetAllOptions:
					return new CustomizationStrategyResetAllOptions(info);
			}
			throw new NotImplementedException();
		}
		public void Customize() {
			CustomizeCore();
		}
		#region Base Helpers
		protected virtual TreeNode CreateNewPageGroupNode() {
			TreeNode node = new TreeNode();
			node.Text = CustomizationHelperBase.DefaultRibbonPageGroupText;
			node.Tag = CreateNewPageGroupInfo(node);
			node.ImageIndex = node.SelectedImageIndex = CustomizationHelperBase.GetImageListIndex(NodeType.Group);
			return node;
		}
		protected virtual RibbonGroupInfo CreateNewPageGroupInfo(TreeNode node) {
			RibbonPageGroup group = new RibbonPageGroup();
			group.Text = CustomizationHelperBase.DefaultRibbonPageGroupText;
			group.Name = GeneratePageGroupName();
			return new RibbonGroupInfo(group, IdGenerator.Instance.Generate(Info.RibbonControl), IdInfo.Empty, Info.RibbonControl);
		}
		string GeneratePageGroupName() {
			for(int i = 1; true; i++) {
				string groupName = string.Format("custom{0}{1}", typeof(RibbonPageGroup).Name, i.ToString());
				if(IsUniquePageGroupName(groupName))
					return groupName;
			}
		}
		bool IsUniquePageGroupName(string groupName) {
			bool unique = true;
			CustomizationHelperBase.ForEachNode(Info.TreeView, node => {
				RibbonGroupInfo groupInfo = node.Tag as RibbonGroupInfo;
				if(groupInfo == null || groupInfo.Group == null)
					return;
				if(string.Equals(groupInfo.Group.Name, groupName, StringComparison.Ordinal))
					unique = false;
			});
			return unique;
		}
		protected virtual TreeNode CreateNewTabNode() {
			TreeNode node = new TreeNode();
			node.Text = CustomizationHelperBase.DefaultRibbonPageText;
			node.Tag = CreateNewPageInfo(node);
			node.ImageIndex = node.SelectedImageIndex = CustomizationHelperBase.GetImageListIndex(NodeType.Page);
			TreeNode childNode = CreateNewPageGroupNode();
			node.Nodes.Add(childNode);
			node.Expand();
			return node;
		}
		protected RibbonPageInfo CreateNewPageInfo(TreeNode node) {
			RibbonPage page = new RibbonPage();
			page.Text = CustomizationHelperBase.DefaultRibbonPageText;
			page.Name = GeneratePageName();
			return new RibbonPageInfo(page, IdGenerator.Instance.Generate(Info.RibbonControl), IdInfo.Empty, Info.RibbonControl);
		}
		string GeneratePageName() {
			for(int i = 1; true; i++) {
				string pageName = string.Format("custom{0}{1}", typeof(RibbonPage).Name, i.ToString());
				if(IsUniquePageName(pageName))
					return pageName;
			}
		}
		bool IsUniquePageName(string pageName) {
			bool unique = true;
			CustomizationHelperBase.ForEachNode(Info.TreeView, node => {
				RibbonPageInfo pageInfo = node.Tag as RibbonPageInfo;
				if(pageInfo == null || pageInfo.Page == null)
					return;
				if(string.Equals(pageInfo.Page.Name, pageName, StringComparison.Ordinal))
					unique = false;
			});
			return unique;
		}
		protected virtual TreeNode FindTopNode(TreeNode node, NodeType nodeType) {
			TreeNode res = node;
			while(true) {
				CustomizationItemInfoBase info = CustomizationHelperBase.FromNode(res);
				if(info.ItemType == nodeType)
					break;
				res = res.Parent;
			}
			return res;
		}
		protected virtual void ExpandParent(TreeNode clonedNode) {
			TreeNode parent = clonedNode.Parent;
			if(parent != null) parent.Expand();
		}
		#endregion
		protected ICustomizationInfoProvider Info { get; private set; }
		protected TreeNode SelectedNode { get { return Info.SelectedNode; } }
		public abstract void CustomizeCore();
		public abstract bool ShouldProcessCommand(TreeNode node, TreeNode sourceNode);
	}
	#endregion
	#region Add New Category
	class CustomizationStrategyAddNewCategory : CustomizationStrategyBase {
		public CustomizationStrategyAddNewCategory(ICustomizationInfoProvider info) : base(info) { }
		public override void CustomizeCore() {
			if(!ShouldProcessCommand(SelectedNode, null))
				return;
			TreeNode node = CreateRibbonCategoryNode();
			Info.TreeView.Nodes.Add(node);
			Info.TreeView.SelectedNode = node;
			TreeNode pageNode = CreateNewTabNode();
			node.Nodes.Add(pageNode);
			Info.TreeView.RefreshNodeState();
			node.Checked = pageNode.Checked = true;
			node.Expand();
		}
		public override bool ShouldProcessCommand(TreeNode node, TreeNode sourceNode) {
			return true;
		}
		protected TreeNode CreateRibbonCategoryNode() {
			TreeNode node = new TreeNode();
			node.Text = CustomizationHelperBase.DefaultRibbonCategoryText;
			node.Tag = CreateRibbonCategoryInfo(node);
			return node;
		}
		protected virtual RibbonPageCategoryInfo CreateRibbonCategoryInfo(TreeNode node) {
			RibbonPageCategory pageCategory = new RibbonPageCategory();
			pageCategory.Text = CustomizationHelperBase.DefaultRibbonCategoryText;
			pageCategory.Name = GenerateCategoryName();
			return new RibbonPageCategoryInfo(pageCategory, IdGenerator.Instance.Generate(Info.RibbonControl), IdInfo.Empty, Info.RibbonControl);
		}
		string GenerateCategoryName() {
			for(int i = 1; true; i++) {
				string categoryName = string.Format("custom{0}{1}", typeof(RibbonPageCategory).Name, i.ToString());
				if(IsUniqueCategoryName(categoryName))
					return categoryName;
			}
		}
		bool IsUniqueCategoryName(string categoryName) {
			bool unique = true;
			CustomizationHelperBase.ForEachNode(Info.TreeView, node => {
				RibbonPageCategoryInfo categoryInfo = node.Tag as RibbonPageCategoryInfo;
				if(categoryInfo == null || categoryInfo.Category == null)
					return;
				if(string.Equals(categoryInfo.Category.Name, categoryName, StringComparison.Ordinal))
					unique = false;
			});
			return unique;
		}
	}
	#endregion
	#region Add New Tab
	class CustomizationStrategyAddNewTab : CustomizationStrategyBase {
		public CustomizationStrategyAddNewTab(ICustomizationInfoProvider info) : base(info) { }
		public override void CustomizeCore() {
			int itemIndex;
			if(!ShouldProcessCommand(SelectedNode, null))
				return;
			TreeNodeCollection nodes = GetNodeCollection(SelectedNode, out itemIndex);
			TreeNode node = CreateNewTabNode();
			if(itemIndex == -1)
				nodes.Add(node);
			else {
				int insIndex = GetInsertionIndex(itemIndex, nodes.Count);
				nodes.Insert(insIndex, node);
			}
			node.Checked = true;
			Info.TreeView.RefreshNodeState();
		}
		public override bool ShouldProcessCommand(TreeNode node, TreeNode sourceNode) {
			if(node == null)
				throw new ArgumentException("TreeNode.SelectedNode");
			return true;
		}
		protected TreeNodeCollection GetNodeCollection(TreeNode sel, out int index) {
			index = -1;
			if(CustomizationHelperBase.IsCategory(sel))
				return sel.Nodes;
			TreeNode node = FindTopNode(sel, NodeType.Page);
			index = node.Index;
			return node.Parent.Nodes;
		}
		protected int GetInsertionIndex(int itemIndex, int itemsCount) {
			int middleItem = (int)(((double)itemsCount + 0.5) / 2);
			return itemIndex >= middleItem ? itemIndex : itemIndex + 1;
		}
	}
	#endregion
	#region Add New Group
	class CustomizationStrategyAddNewGroup : CustomizationStrategyBase {
		public CustomizationStrategyAddNewGroup(ICustomizationInfoProvider info) : base(info) { }
		public override void CustomizeCore() {
			int itemIndex;
			if(!ShouldProcessCommand(SelectedNode, null))
				return;
			TreeNodeCollection nodes = GetNodeCollection(SelectedNode, out itemIndex);
			TreeNode node = CreateNewPageGroupNode();
			if(itemIndex == -1)
				nodes.Add(node);
			else nodes.Insert(itemIndex + 1, node);
			Info.TreeView.RefreshNodeState();
			if(!node.Parent.IsExpanded) node.Parent.Expand();
		}
		public override bool ShouldProcessCommand(TreeNode node, TreeNode selectedNode) {
			if(node == null)
				throw new ArgumentException("TreeNode.SelectedNode");
			if(CustomizationHelperBase.IsCategory(node))
				return false;
			return true;
		}
		protected TreeNodeCollection GetNodeCollection(TreeNode sel, out int index) {
			index = -1;
			TreeNode node = sel;
			if(CustomizationHelperBase.IsPage(node))
				return node.Nodes;
			node = FindTopNode(node, NodeType.Group);
			index = node.Index;
			TreeNode pageNode = FindTopNode(node, NodeType.Page);
			return pageNode.Nodes;
		}
	}
	#endregion
	#region Rename
	class CustomizationStrategyRename : CustomizationStrategyBase {
		public CustomizationStrategyRename(ICustomizationInfoProvider info) : base(info) { }
		public override void CustomizeCore() {
			if(!ShouldProcessCommand(SelectedNode, null))
				return;
			string name = RequestName();
			if(string.IsNullOrEmpty(name))
				return;
			ApplyName(name);
		}
		public override bool ShouldProcessCommand(TreeNode node, TreeNode sourceNode) {
			if(CustomizationHelperBase.IsBarButtonGroupChildNode(node))
				return false;
			if(CustomizationHelperBase.FromNode(node).IsCustom)
				return true;
			return !CustomizationHelperBase.IsItem(node);
		}
		protected virtual string RequestName() {
			string name = string.Empty;
			using(RenameForm frm = new RenameForm()) {
				frm.SetName(TransformNameToEditState(SelectedNode.Text));
				DialogResult res = frm.ShowDialog();
				if(res == DialogResult.OK)
					name = TransformNameToInternalState(frm.GetName());
			}
			return name;
		}
		protected virtual string TransformNameToEditState(string source) {
			return CustomizationHelperBase.ExcludeInternalSuffix(source);
		}
		protected virtual string TransformNameToInternalState(string source) {
			if(CustomizationHelperBase.IsItem(SelectedNode) || !CustomizationHelperBase.FromNode(SelectedNode).IsCustom)
				return source;
			return CustomizationHelperBase.AddInternalSuffixToText(source);
		}
		protected virtual void ApplyName(string name) {
			ApplyNodeTextCore(name);
			ApplyModelItemTextCore(name);
		}
		protected virtual void ApplyNodeTextCore(string name) {
			SelectedNode.Text = name;
		}
		protected virtual void ApplyModelItemTextCore(string name) {
			CustomizationItemInfoBase info = SelectedNode.Tag as CustomizationItemInfoBase;
			if(info != null) info.Alias = name;
		}
	}
	#endregion
	#region Move To Right
	public abstract class CustomizationStrategyMoveToRightBase : CustomizationStrategyBase {
		public CustomizationStrategyMoveToRightBase(ICustomizationInfoProvider info) : base(info) { }
		public static CustomizationStrategyMoveToRightBase Create(ICustomizationInfoProvider info) {
			TreeNode node = info.SourceNode;
			if(node == null)
				throw new ArgumentException();
			if(CustomizationHelperBase.IsCategory(node))
				return new CustomizationStrategyMoveCategoryToRight(info);
			if(CustomizationHelperBase.IsPage(node))
				return new CustomizationStrategyMovePageToRight(info);
			if(CustomizationHelperBase.IsGroup(node))
				return new CustomizationStrategyMoveGroupToRight(info);
			if(CustomizationHelperBase.IsItem(node))
				return new CustomizationStrategyMoveItemToRight(info);
			return null;
		}
		public override void CustomizeCore() {
			int itemIndex;
			if(SelectedNode == null) throw new ArgumentException("TreeNode.SelectedNode");
			if(!ShouldProcessCommand(SelectedNode, Info.SourceNode))
				return;
			TreeNodeCollection nodes = GetNodeCollection(SelectedNode, out itemIndex);
			TreeNode clonedNode = CloneNode();
			AddItem(nodes, itemIndex, clonedNode);
			clonedNode.Checked = true;
			Info.TreeView.RefreshNodeState();
			ExpandParent(clonedNode);
			SelectNextNode();
		}
		protected virtual void SelectNextNode() {
			TreeNode nextNode = Info.SourceNode.NextNode;
			if(nextNode != null) Info.SourceTreeView.SelectedNode = nextNode;
		}
		protected virtual void AddItem(TreeNodeCollection nodes, int itemIndex, TreeNode clonedNode) {
			if(itemIndex == -1) nodes.Add(clonedNode);
			else nodes.Insert(itemIndex + 1, clonedNode);
		}
		protected TreeNode CloneNode() {
			TreeNode node = Info.SourceNode;
			return CloneNodeCore(node);
		}
		protected TreeNode CloneNodeCore(TreeNode source) {
			TreeNode res = new TreeNode();
			InitTreeNode(res, source);
			ProcessChildNodes(res, source);
			return res;
		}
		protected void ProcessChildNodes(TreeNode node, TreeNode source) {
			foreach(TreeNode currNode in source.Nodes) {
				TreeNode child = new TreeNode();
				InitTreeNode(child, currNode);
				node.Nodes.Add(child);
				ProcessChildNodes(child, currNode);
			}
		}
		protected void InitTreeNode(TreeNode node, TreeNode source) {
			node.Text = GetClonedNodeText(source);
			node.ImageIndex = source.ImageIndex;
			node.SelectedImageIndex = source.SelectedImageIndex;
			SyncExpandState(node, source);
			AssignTagObject(node, source);
			AssignAlias(node);
		}
		protected string GetClonedNodeText(TreeNode sourceNode) {
			if(CustomizationHelperBase.FromNode(sourceNode).ItemType == NodeType.Item)
				return sourceNode.Text;
			return CustomizationHelperBase.AddInternalSuffixToText(sourceNode.Text);
		}
		protected void SyncExpandState(TreeNode node, TreeNode source) {
			if(source.IsExpanded) node.Expand();
			else node.Collapse();
		}
		protected void AssignTagObject(TreeNode node, TreeNode source) {
			ICloneable clonable = source.Tag as ICloneable;
			if(clonable == null)
				throw new NotSupportedException();
			node.Tag = clonable.Clone();
		}
		protected virtual void AssignAlias(TreeNode node) {
			CustomizationItemInfoBase info = node.Tag as CustomizationItemInfoBase;
			info.Alias = node.Text;
		}
		protected abstract TreeNodeCollection GetNodeCollection(TreeNode sel, out int index);
	}
	class CustomizationStrategyMoveCategoryToRight : CustomizationStrategyMoveToRightBase {
		public CustomizationStrategyMoveCategoryToRight(ICustomizationInfoProvider info) : base(info) { }
		protected override TreeNodeCollection GetNodeCollection(TreeNode sel, out int index) {
			index = -1;
			return null;
		}
		public override bool ShouldProcessCommand(TreeNode node, TreeNode sourceNode) {
			return false;
		}
	}
	class CustomizationStrategyMovePageToRight : CustomizationStrategyMoveToRightBase {
		public CustomizationStrategyMovePageToRight(ICustomizationInfoProvider info) : base(info) { }
		protected override TreeNodeCollection GetNodeCollection(TreeNode sel, out int index) {
			index = -1;
			if(CustomizationHelperBase.IsCategory(sel))
				return sel.Nodes;
			TreeNode node = FindTopNode(sel, NodeType.Page);
			index = node.Index;
			return node.Parent.Nodes;
		}
		public override bool ShouldProcessCommand(TreeNode node, TreeNode sourceNode) {
			return true;
		}
	}
	class CustomizationStrategyMoveGroupToRight : CustomizationStrategyMoveToRightBase {
		public CustomizationStrategyMoveGroupToRight(ICustomizationInfoProvider info) : base(info) { }
		protected override TreeNodeCollection GetNodeCollection(TreeNode sel, out int index) {
			index = -1;
			if(CustomizationHelperBase.IsPage(sel))
				return sel.Nodes;
			TreeNode node = FindTopNode(sel, NodeType.Page);
			index = node.Index;
			return node.Nodes;
		}
		public override bool ShouldProcessCommand(TreeNode node, TreeNode sourceNode) {
			if(CustomizationHelperBase.IsCategory(node))
				return false;
			if(CustomizationHelperBase.IsPage(node))
				return true;
			TreeNode page = FindTopNode(node, NodeType.Page);
			if(!CustomizationHelperBase.FromNode(page).IsCustom && object.ReferenceEquals(sourceNode.Tag, page.Tag))
				return false;
			return true;
		}
	}
	class CustomizationStrategyMoveItemToRight : CustomizationStrategyMoveToRightBase {
		public CustomizationStrategyMoveItemToRight(ICustomizationInfoProvider info) : base(info) { }
		protected override TreeNodeCollection GetNodeCollection(TreeNode sel, out int index) {
			index = -1;
			TreeNode node = sel;
			if(CustomizationHelperBase.IsGroup(node))
				return node.Nodes;
			index = node.Index;
			return node.Parent.Nodes;
		}
		public override bool ShouldProcessCommand(TreeNode node, TreeNode sourceNode) {
			TreeNode groupNode = null;
			if(CustomizationHelperBase.IsBarButtonGroupChildNode(node))
				return false;
			if(CustomizationHelperBase.IsPopupItem(node))
				return false;
			if(CustomizationHelperBase.IsItem(node))
				groupNode = FindTopNode(node, NodeType.Group);
			else if(CustomizationHelperBase.IsGroup(node))
				groupNode = node;
			if(groupNode == null) return false;
			return CustomizationHelperBase.FromNode(groupNode).IsCustom;
		}
	}
	#endregion
	#region Removing
	class CustomizationStrategyRemoveItem : CustomizationStrategyBase {
		public CustomizationStrategyRemoveItem(ICustomizationInfoProvider info) : base(info) { }
		public override void CustomizeCore() {
			if(!ShouldProcessCommand(SelectedNode, null))
				return;
			TreeNode node = SelectedNode;
			node.Remove();
		}
		public override bool ShouldProcessCommand(TreeNode node, TreeNode sourceNode) {
			if(CustomizationHelperBase.IsBarButtonGroupChildNode(node))
				return false;
			if(CustomizationHelperBase.IsPopupItem(node))
				return false;
			if(CustomizationHelperBase.FromNode(node).IsCustom)
				return true;
			return CustomizationHelperBase.IsGroup(node);
		}
	}
	#endregion
	#region Moving Up
	public abstract class CustomizationStrategyMoveItemBase : CustomizationStrategyBase {
		public CustomizationStrategyMoveItemBase(ICustomizationInfoProvider info) : base(info) { }
	}
	abstract class CustomizationStrategyMoveItemUpBase : CustomizationStrategyMoveItemBase {
		public CustomizationStrategyMoveItemUpBase(ICustomizationInfoProvider info) : base(info) { }
		public static CustomizationStrategyMoveItemUpBase Create(ICustomizationInfoProvider info) {
			TreeNode node = info.SelectedNode;
			if(CustomizationHelperBase.IsCategory(node))
				return new CustomizationStrategyMoveCategoryUp(info);
			if(CustomizationHelperBase.IsPage(node))
				return new CustomizationStrategyMovePageUp(info);
			if(CustomizationHelperBase.IsGroup(node))
				return new CustomizationStrategyMoveGroupUp(info);
			if(CustomizationHelperBase.IsItem(node))
				return new CustomizationStrategyMoveItemUp(info);
			throw new NotSupportedException();
		}
	}
	class CustomizationStrategyMoveCategoryUp : CustomizationStrategyMoveItemUpBase {
		public CustomizationStrategyMoveCategoryUp(ICustomizationInfoProvider info) : base(info) { }
		public override void CustomizeCore() {
		}
		public override bool ShouldProcessCommand(TreeNode node, TreeNode sourceNode) {
			return false;
		}
	}
	class CustomizationStrategyMovePageUp : CustomizationStrategyMoveItemUpBase {
		public CustomizationStrategyMovePageUp(ICustomizationInfoProvider info) : base(info) { }
		public override void CustomizeCore() {
			if(!ShouldProcessCommand(SelectedNode, null))
				return;
			Info.TreeView.MoveUp();
			Info.TreeView.RefreshNodeState();
		}
		public override bool ShouldProcessCommand(TreeNode node, TreeNode sourceNode) {
			return node != null && node.PrevNode != null;
		}
	}
	class CustomizationStrategyMoveGroupUp : CustomizationStrategyMoveItemUpBase {
		public CustomizationStrategyMoveGroupUp(ICustomizationInfoProvider info) : base(info) { }
		public override void CustomizeCore() {
			if(!ShouldProcessCommand(SelectedNode, null))
				return;
			Info.TreeView.MoveUpCrossParent();
			Info.TreeView.RefreshNodeState();
		}
		public override bool ShouldProcessCommand(TreeNode node, TreeNode sourceNode) {
			if(node == null)
				return false;
			if(node.PrevNode != null)
				return true;
			return node.Parent != null && node.Parent.PrevNode != null;
		}
	}
	class CustomizationStrategyMoveItemUp : CustomizationStrategyMoveItemUpBase {
		public CustomizationStrategyMoveItemUp(ICustomizationInfoProvider info) : base(info) { }
		public override void CustomizeCore() {
			if(!ShouldProcessCommand(SelectedNode, null))
				return;
			Info.TreeView.MoveUp();
			Info.TreeView.RefreshNodeState();
		}
		public override bool ShouldProcessCommand(TreeNode node, TreeNode sourceNode) {
			if(CustomizationHelperBase.IsBarButtonGroupChildNode(node))
				return false;
			if(CustomizationHelperBase.IsPopupItem(node))
				return false;
			return CustomizationHelperBase.FromNode(node).IsCustom;
		}
	}
	#endregion
	#region Moving Down
	abstract class CustomizationStrategyMoveItemDownBase : CustomizationStrategyMoveItemBase {
		public CustomizationStrategyMoveItemDownBase(ICustomizationInfoProvider info) : base(info) { }
		public static CustomizationStrategyMoveItemDownBase Create(ICustomizationInfoProvider info) {
			TreeNode node = info.SelectedNode;
			if(CustomizationHelperBase.IsCategory(node))
				return new CustomizationStrategyMoveCategoryDown(info);
			if(CustomizationHelperBase.IsPage(node))
				return new CustomizationStrategyMovePageDown(info);
			if(CustomizationHelperBase.IsGroup(node))
				return new CustomizationStrategyMoveGroupDown(info);
			if(CustomizationHelperBase.IsItem(node))
				return new CustomizationStrategyMoveItemDown(info);
			throw new NotSupportedException();
		}
	}
	class CustomizationStrategyMoveCategoryDown : CustomizationStrategyMoveItemDownBase {
		public CustomizationStrategyMoveCategoryDown(ICustomizationInfoProvider info) : base(info) { }
		public override void CustomizeCore() {
		}
		public override bool ShouldProcessCommand(TreeNode node, TreeNode sourceNode) {
			return false;
		}
	}
	class CustomizationStrategyMovePageDown : CustomizationStrategyMoveItemDownBase {
		public CustomizationStrategyMovePageDown(ICustomizationInfoProvider info) : base(info) { }
		public override void CustomizeCore() {
			if(!ShouldProcessCommand(SelectedNode, null))
				return;
			Info.TreeView.MoveDown();
			Info.TreeView.RefreshNodeState();
		}
		public override bool ShouldProcessCommand(TreeNode node, TreeNode sourceNode) {
			return node != null && node.NextNode != null;
		}
	}
	class CustomizationStrategyMoveGroupDown : CustomizationStrategyMoveItemDownBase {
		public CustomizationStrategyMoveGroupDown(ICustomizationInfoProvider info) : base(info) { }
		public override void CustomizeCore() {
			if(!ShouldProcessCommand(SelectedNode, null))
				return;
			Info.TreeView.MoveDownCrossParent();
			Info.TreeView.RefreshNodeState();
		}
		public override bool ShouldProcessCommand(TreeNode node, TreeNode sourceNode) {
			if(node == null)
				return false;
			if(node.NextNode != null)
				return true;
			return node.Parent != null && node.Parent.NextNode != null;
		}
	}
	class CustomizationStrategyMoveItemDown : CustomizationStrategyMoveItemDownBase {
		public CustomizationStrategyMoveItemDown(ICustomizationInfoProvider info) : base(info) { }
		public override void CustomizeCore() {
			if(!ShouldProcessCommand(SelectedNode, null))
				return;
			Info.TreeView.MoveDown();
			Info.TreeView.RefreshNodeState();
		}
		public override bool ShouldProcessCommand(TreeNode node, TreeNode sourceNode) {
			if(CustomizationHelperBase.IsBarButtonGroupChildNode(node))
				return false;
			if(CustomizationHelperBase.IsPopupItem(node))
				return false;
			return CustomizationHelperBase.FromNode(node).IsCustom;
		}
	}
	#endregion
	#region Reset Options
	class CustomizationStrategyResetSelectedPageOptions : CustomizationStrategyBase {
		public CustomizationStrategyResetSelectedPageOptions(ICustomizationInfoProvider info) : base(info) { }
		public override void CustomizeCore() {
			if(!ShouldProcessCommand(SelectedNode, null))
				return;
			TreeNode pageNode = FindTopNode(SelectedNode, NodeType.Page);
			IdInfo info = CustomizationHelperBase.FromNode(pageNode).SourceIdInfo;
			ReplaceItem(pageNode, CreateSourcePageNode(info));
			Info.TreeView.RefreshNodeState();
		}
		public override bool ShouldProcessCommand(TreeNode node, TreeNode sourceNode) {
			if(CustomizationHelperBase.IsCategory(node))
				return false;
			TreeNode srcNode = FindTopNode(node, NodeType.Page);
			if(CustomizationHelperBase.FromNode(srcNode).IsCustom)
				return false;
			return true;
		}
		protected virtual TreeNode CreateSourcePageNode(IdInfo idSourceInfo) {
			return CustomizationHelperBase.CreateSourceNode(idSourceInfo, Info.RibbonControl);
		}
		protected virtual void ReplaceItem(TreeNode srcNode, TreeNode pageNode) {
			TreeNode clonedPage = pageNode.Clone() as TreeNode;
			int index = srcNode.Index;
			TreeNodeCollection pageNodes = srcNode.Parent.Nodes;
			srcNode.Remove();
			pageNodes.Insert(index, clonedPage);
			clonedPage.Expand();
		}
	}
	class CustomizationStrategyResetAllOptions : CustomizationStrategyBase {
		public CustomizationStrategyResetAllOptions(ICustomizationInfoProvider info) : base(info) { }
		public override void CustomizeCore() {
			ResetRibbon();
			ReloadForm();
		}
		protected virtual void ResetRibbon() {
			TreeNodeCollection nodes = Info.TreeView.Nodes;
			nodes.Clear();
			List<TreeNode> sourceNodes = CustomizationHelperBase.GetSourceTreeNodes(Info.RibbonControl);
			foreach(TreeNode categoryNode in sourceNodes) nodes.Add(categoryNode);
			Info.TreeView.RefreshNodeState();
		}
		protected virtual void ReloadForm() {
			Info.ResetRibbon();
			RaiseLayoutChanged();
			Info.ReloadForm();
		}
		public override bool ShouldProcessCommand(TreeNode node, TreeNode sourceNode) {
			return true;
		}
		protected virtual void RaiseLayoutChanged() {
			RibbonControl ribbon = Info.RibbonControl;
			ribbon.RaiseResetLayout(new ResetLayoutEventArgs(ribbon));
		}
	}
	#endregion
}
