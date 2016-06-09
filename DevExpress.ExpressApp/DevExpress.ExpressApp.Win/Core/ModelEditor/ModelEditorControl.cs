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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Win.Core.ModelEditor.InternalDesigner;
using DevExpress.ExpressApp.Win.Core.ModelEditor.NodesTree;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp.Win.Layout;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using DevExpress.XtraTreeList.Nodes;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor {
	[ToolboxItem(false)]
	public class ModelEditorControl : PanelControl, IDXPopupMenuHolder, IBarManagerHolder {
		internal const string ModelEditorControlSettingsPath = "ModelEditorControl";
		private const string SplitterPositionAttributeName = "SplitterPosition";
		private const string PropetrySplitterPositionAttributeName = "PropetrySplitterPosition";
		private const string SearchControllVisibleAttributeName = "SearchControllVisible";
		private const string FocusedObjectAttributeName = "FocusedObject";
		private const string KeySettingsPath = ";Key = ";
		public ActionsDXPopupMenu popupMenu;
		public ModelTreeList modelTreeList;
		public Control ModelAttributesEditor {
			get {
				return modelAttributesEditor;
			}
		}
		private ModelEditorPanelControl modelEditorPanelControl;
		private ModelEditorPropertyGrid modelAttributesEditor;
		private SettingsStorage settings;
		private SearchControl searchControl;
		private WinLayoutManager layoutManager = null;
		private BarManager barManager;
		private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerMenuBarItem cPopup;
		private bool mergeMode = false;
		private ModelEditorControl() {
			this.Tag = "testmodeleditor=ModelEditor";
			modelTreeList = new ModelTreeList();
			modelTreeList.Dock = DockStyle.Fill;
			modelTreeList.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(treeList_FocusedNodeChanged);
			modelTreeList.BeforeFocusNode += new DevExpress.XtraTreeList.BeforeFocusNodeEventHandler(modelTreeList_BeforeFocusNode);
			modelTreeList.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.modelEditorPanelControl = new ModelEditorPanelControl();
			this.searchControl = new SearchControl(this);
			modelEditorPanelControl.Dock = DockStyle.Fill;
			this.searchControl.SuspendLayout();
			this.SuspendLayout();
			modelAttributesEditor = new ModelEditorPropertyGrid();
			modelAttributesEditor.Dock = DockStyle.Fill;
			cPopup = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerMenuBarItem();
			popupMenu = new ActionsDXPopupMenu();
			this.modelEditorPanelControl.RootSplitContainer.Panel1.Controls.Add(this.modelTreeList);
			this.modelEditorPanelControl.PropertySplitContainer.Panel1.Controls.Add(this.modelAttributesEditor);
			this.modelEditorPanelControl.PropertySplitContainer.Panel2.Controls.Add(this.searchControl);
			searchControl.SearchControlClosed += new EventHandler(searchControl_SearchControlClosed);
			this.Controls.Add(modelEditorPanelControl);
			this.Name = "ModelEditorControl";
			this.searchControl.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		public ModelEditorControl(SettingsStorage settings)
			: this() {
			this.settings = settings;
		}
		protected virtual void OnBarMangerChanged() {
			if(BarManagerChanged != null) {
				BarManagerChanged(this, EventArgs.Empty);
			}
		}
		public event EventHandler OnDisposing;
		protected override void Dispose(bool disposing) {
			try {
				if(disposing) {
					if(OnDisposing != null) {
						OnDisposing(this, EventArgs.Empty);
					}
					SafeFreeLayoutManager();
					DisposeFields();
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		protected override void OnResize(EventArgs e) {
			isSetRatio = false;
			base.OnResize(e);
		}
		private void DisposeFields() {
			if(modelEditorPanelControl != null) {
				modelEditorPanelControl.RootSplitContainer.SplitterPositionChanged -= new EventHandler(SplitterPositionChanged);
				modelEditorPanelControl.RootSplitContainer.SplitterMoved -= new EventHandler(SplitterMoved);
				modelEditorPanelControl.Dispose();
				modelEditorPanelControl = null;
			}
			if(popupMenu != null) {
				popupMenu.Dispose();
				popupMenu = null;
			}
			if(modelTreeList != null) {
				modelTreeList.Dispose();
				modelTreeList = null;
			}
			if(modelAttributesEditor != null) {
				modelAttributesEditor.Dispose();
				modelAttributesEditor = null;
			}
			if(searchControl != null) {
				searchControl.SearchControlClosed -= new EventHandler(searchControl_SearchControlClosed);
				searchControl.Dispose();
				searchControl = null;
			}
			if(layoutManager != null) {
				layoutManager.Dispose();
				layoutManager = null;
			}
			if(barManager != null) {
				barManager.Dispose();
				barManager = null;
			}
			settings = null;
		}
		private void SplitterPositionChanged(object sender, EventArgs e) {
			if(sender.GetHashCode() == modelEditorPanelControl.RootSplitContainer.GetHashCode()) {
				ratioRootSplitContainer = SetSplitterPosition((SplitContainerControl)sender, SplitterPositionAttributeName, ratioRootSplitContainer);
			}
			else {
				if(sender.GetHashCode() == modelEditorPanelControl.PropertySplitContainer.GetHashCode()) {
					ratioPropertySplitContainer = SetSplitterPosition((SplitContainerControl)sender, PropetrySplitterPositionAttributeName, ratioPropertySplitContainer);
				}
			}
		}
		private double ratioRootSplitContainer = 0;
		private double ratioPropertySplitContainer = 0;
		private bool isSetRatio = true;
		private double SetSplitterPosition(SplitContainerControl splitContainer, string attName, double ratio) {
			if(Size.Height != 0) {
				if(ratio == 0 && settings != null) {
					string x = settings.LoadOption(ModelEditorControlSettingsPath, attName);
					double.TryParse(x, out ratio);
					if(ratio == 0 || ratio > 1) {
						ratio = 0.5;
					}
					SetSplitterPosition(splitContainer, ratio);
					if(splitContainer.SplitterPosition == splitContainer.Panel1.MinSize) {
						ratio = ratio + ratio / 100 * 5;
						SetSplitterPosition(splitContainer, ratio);
						if(splitContainer.SplitterPosition == splitContainer.Panel1.MinSize) {
							ratio = 0;
						}
					}
				}
				else {
					if(isSetRatio) {
						ratio = (double)splitContainer.SplitterPosition / (double)splitContainer.Size.Width;
					}
					SetSplitterPosition(splitContainer, ratio);
				}
				isSetRatio = true;
			}
			return ratio;
		}
		private void SetSplitterPosition(SplitContainerControl control, double ratio) {
			control.SplitterPositionChanged -= new EventHandler(SplitterPositionChanged);
			control.SplitterMoved -= new EventHandler(SplitterMoved);
			int newSplitterPosition = (int)((double)control.Size.Width * ratio);
			if(newSplitterPosition != control.SplitterPosition && (control.SplitterPosition < newSplitterPosition + 1 || control.SplitterPosition > newSplitterPosition - 1)) {
				control.SplitterPosition = newSplitterPosition;
			}
			control.SplitterPositionChanged += new EventHandler(SplitterPositionChanged);
			control.SplitterMoved += new EventHandler(SplitterMoved);
		}
		public void SaveSettings() {
			double ratioToSave = (double)modelEditorPanelControl.RootSplitContainer.SplitterPosition / (double)modelEditorPanelControl.RootSplitContainer.Size.Width;
			settings.SaveOption(ModelEditorControlSettingsPath, SplitterPositionAttributeName, ratioToSave.ToString());
			if(modelEditorPanelControl.PropertySplitContainer.PanelVisibility == SplitPanelVisibility.Both) {
				ratioToSave = (double)modelEditorPanelControl.PropertySplitContainer.SplitterPosition / (double)modelEditorPanelControl.PropertySplitContainer.Size.Width;
				settings.SaveOption(ModelEditorControlSettingsPath, PropetrySplitterPositionAttributeName, ratioToSave.ToString());
			}
			settings.SaveOption(ModelEditorControlSettingsPath, SearchControllVisibleAttributeName, Convert.ToString(Convert.ToInt32(modelEditorPanelControl.PropertySplitContainer.PanelVisibility == SplitPanelVisibility.Both)));
			searchControl.SaveSettings();
			SaveFocusedNode();
		}
		public void LoadSettings() {
			if(settings != null) {
				modelEditorPanelControl.RootSplitContainer.SplitterPositionChanged -= new EventHandler(SplitterPositionChanged);
				modelEditorPanelControl.RootSplitContainer.SplitterPositionChanged += new EventHandler(SplitterPositionChanged);
				modelEditorPanelControl.RootSplitContainer.SplitterMoved -= new EventHandler(SplitterMoved);
				modelEditorPanelControl.PropertySplitContainer.SplitterPositionChanged -= new EventHandler(SplitterPositionChanged);
				modelEditorPanelControl.PropertySplitContainer.SplitterPositionChanged += new EventHandler(SplitterPositionChanged);
				modelEditorPanelControl.PropertySplitContainer.SplitterMoved -= new EventHandler(SplitterMoved);
				bool searchPanelVisible = Convert.ToBoolean(settings.LoadIntOption(ModelEditorControlSettingsPath, SearchControllVisibleAttributeName, 0));
				searchControl.SettingsStorage = settings;
				StorageFocusedNode();
				if(searchPanelVisible) {
					ActivateSearchControl();
				}
			}
			SplitterPositionChanged(modelEditorPanelControl.RootSplitContainer, EventArgs.Empty);
			SplitterPositionChanged(modelEditorPanelControl.PropertySplitContainer, EventArgs.Empty);
			modelEditorPanelControl.RootSplitContainer.SplitterMoved += new EventHandler(SplitterMoved);
			modelEditorPanelControl.PropertySplitContainer.SplitterMoved += new EventHandler(SplitterMoved);
		}
		private void SplitterMoved(object sender, EventArgs e) {
			isSetRatio = true;
		}
		private void StorageFocusedNode() {
			string focusedObjectPath = settings.LoadOption(ModelEditorControlSettingsPath, FocusedObjectAttributeName);
			ModelNode node = ModelEditorHelper.FindNodeByPath(focusedObjectPath, DataSource);
			if(node != null) {
				List<ModelNode> nodePath = new List<ModelNode>();
				ModelNode parent = node.Parent;
				while(parent != null) {
					nodePath.Add(parent);
					parent = parent.Parent;
				}
				nodePath.Reverse();
				nodePath.Add(node);
				ModelTreeListNode targetNode = null;
				foreach(ModelNode nodeP in nodePath){
					targetNode = modelTreeList.Adapter.GetPrimaryNode(targetNode, nodeP, true);
				}
				CurrentModelTreeListNode = targetNode;
			}
		}
		private void SaveFocusedNode() {
			ModelTreeListNode focusedObject = (ModelTreeListNode)modelTreeList.FocusedObject;
			if(focusedObject != null) {
				if(focusedObject.VirtualTreeNode) {
					ModelTreeListNode parentVirtualRoot = focusedObject.RootVirtualTreeNode;
					if(parentVirtualRoot != null) {
						focusedObject = parentVirtualRoot;
					}
				}
				string focusedObjectPath = ModelEditorHelper.GetModelNodePath(focusedObject.ModelNode);
				settings.SaveOption(ModelEditorControlSettingsPath, FocusedObjectAttributeName, focusedObjectPath);
			}
		}
		private void PropertyGridSetSelectedObjects() {
			if(modelTreeList.FocusedObject != null && ((ModelTreeListNode)modelTreeList.FocusedObject).ModelTreeListNodeType == ModelTreeListNodeType.Group) {
				modelAttributesEditor.PropertyGrid.SelectedObject = null;
			}
			else {
				if(NeedHardRefreshPropertyGrid()) {
					modelAttributesEditor.PropertyGrid.SelectedObjects = null;
				}
				modelAttributesEditor.PropertyGrid.SelectedObjects = SelectedNodes.ToArray();
			}
		}
		private bool NeedHardRefreshPropertyGrid() {
			bool needHardRefresh = false;
			foreach(ModelTreeListNode treeNode in SelectedNodes) {
				if(treeNode.VirtualTreeNode && !treeNode.IsRootVirtualTreeNode) {
					needHardRefresh = true;
					break;
				}
			}
			if(modelAttributesEditor.PropertyGrid.SelectedObjects != null) {
				foreach(ModelTreeListNode treeNode in modelAttributesEditor.PropertyGrid.SelectedObjects) {
					if(treeNode.VirtualTreeNode && !treeNode.IsRootVirtualTreeNode) {
						needHardRefresh = true;
						break;
					}
				}
			}
			return needHardRefresh;
		}
		private void searchControl_SearchControlClosed(object sender, EventArgs e) {
			modelEditorPanelControl.PropertySplitContainer.PanelVisibility = DevExpress.XtraEditors.SplitPanelVisibility.Panel1;
		}
		private void Container_Changed(object sender, EventArgs e) {
			if(layoutManager.Container.IsModified) {
				CancelEventArgs args = new CancelEventArgs();
				OnLayoutChanged(args);
				layoutManager.Container.Changed -= new EventHandler(Container_Changed);
				try {
					layoutManager.SaveModel();
					DevExpress.ExpressApp.Win.Controls.ObjectTreeListNode focusedNode = (DevExpress.ExpressApp.Win.Controls.ObjectTreeListNode)this.modelTreeList.FocusedNode;
					modelTreeList.ReBuildChildNodes(focusedNode);
				}
				catch(Exception ex) {
					ModelEditorControllerBase.ShowError(ex);
				}
				finally {
					layoutManager.Container.Changed += new EventHandler(Container_Changed);
				}
			}
		}
		private void OnLayoutChanged(CancelEventArgs args) {
			if(LayoutChanged != null) {
				LayoutChanged(this, args);
			}
		}
		private void treeList_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e) {
			if(FocusedNodeChanged != null) {
				FocusedNodeChanged(this, EventArgs.Empty);
			}
		}
		private void DoResetLayout() {
			if(ResetLayout != null) {
				ResetLayout(this, EventArgs.Empty);
			}
		}
		private void layoutControl_DefaultLayoutLoaded(object sender, EventArgs e) {
			DoResetLayout();
		}
		private void modelTreeList_BeforeFocusNode(object sender, DevExpress.XtraTreeList.BeforeFocusNodeEventArgs e) {
			SafeFreeLayoutManager();
		}
		private void SafeFreeLayoutManager() {
			if(layoutManager != null) {
				try {
					layoutManager.SaveModel();
				}
				catch {
				}
				DestroyLayouts();
			}
		}
		private bool IsWinLayout {
			get {
				foreach(ModuleBase module in ((IModelSources)DataSource.Application).Modules) {
					if(module is DevExpress.ExpressApp.Win.SystemModule.SystemWindowsFormsModule) {
						return true;
					}
				}
				return false;
			}
		}
		public void RebuildModelNode(ModelTreeListNode modelNode) {
			modelTreeList.ReBuildChildNodes(modelTreeList.FindBuiltAncestorNode(modelNode));
		}
		public void RemoveControlNode(ModelTreeListNode removedModelNode) {
			ObjectTreeListNode removedTreeListNode = modelTreeList.FindBuiltAncestorNode(removedModelNode);
			if(removedTreeListNode != null) {
				removedTreeListNode.ParentNode.Nodes.Remove(removedTreeListNode);
			}
		}
		public void ActivateSearchControl() {
			modelEditorPanelControl.PropertySplitContainer.PanelVisibility = SplitPanelVisibility.Both;
			searchControl.Visible = true;
			searchControl.GetMRUEdit.Focus();
		}
		public void ShowLoadedModulesForm(IEnumerable<ModuleBase> modules, string formHeaderImageName) {
			GridControl grid = new GridControl();
			grid.BeginInit();
			BarManager barManager = new ModelEditorBarManager();
			barManager.Form = grid;
			grid.MenuManager = barManager;
			GridView view = new GridView();
			view.BeginInit();
			grid.MainView = view;
			view.OptionsView.ShowGroupPanel = false;
			view.OptionsView.ShowIndicator = false;
			view.Columns.AddField("Name").VisibleIndex = 0;
			view.Columns.AddField("Version").VisibleIndex = 1;
			view.Columns.AddField("AssemblyName").VisibleIndex = 2;
			view.OptionsDetail.EnableMasterViewMode = false;
			view.OptionsBehavior.Editable = false;
			view.EndInit();
			grid.EndInit();
			grid.DataSource = modules;
			PopupInfo.Show(grid, "Loaded Modules", ImageLoader.Instance.GetImageInfo(formHeaderImageName).Image, true);
		}
		private ViewItem CreateEditor(Type classType, IModelViewItem itemNode) {
			if(itemNode is IModelActionContainerViewItem) {
				WinActionContainerViewItem actionContainerViewItem = new WinActionContainerViewItem((IModelActionContainerViewItem)itemNode, classType);
				actionContainerViewItem.CreateControl();
				SimpleAction fakeAction = new SimpleAction();
				fakeAction.Caption = ((IModelActionContainerViewItem)itemNode).Id;
				fakeAction.Enabled.SetItemValue("ModelEditor", false);
				actionContainerViewItem.Register(fakeAction);
				return actionContainerViewItem;
			}
			else {
				ModelEditorDesignDetailViewItem modelEditorDesignDetailViewItem = new ModelEditorDesignDetailViewItem(itemNode, classType);
				modelEditorDesignDetailViewItem.CreateControl();
				return modelEditorDesignDetailViewItem;
			}
		}
		public bool CanShowLayout(ModelNode node) {
			return node is IModelViewLayout;
		}
		private GridListEditorDesigner gridListEditorDesigner = null;
		private void ShowListViewLayout(IModelNode node) {
			DestroyLayouts();
			if(node != null && node.Parent is IModelListView) {
				gridListEditorDesigner = new GridListEditorDesigner((IModelListView)node.Parent, node, new TypesInfoServiceProvider());
				if(gridListEditorDesigner.DesignerControl != null) {
					modelEditorPanelControl.PropertySplitContainer.Panel1.Controls.Add(gridListEditorDesigner.DesignerControl);
					modelAttributesEditor.Visible = false;
					gridListEditorDesigner.DesignerChanged += gridListEditorDesigner_DesignerChanged;
				}
			}
		}
		private void gridListEditorDesigner_DesignerChanged(object sender, EventArgs e) {
			CancelEventArgs args = new CancelEventArgs();
			OnLayoutChanged(args);
			if(!args.Cancel) {
				try {
					gridListEditorDesigner.SaveModel();
					DevExpress.ExpressApp.Win.Controls.ObjectTreeListNode focusedNode = (DevExpress.ExpressApp.Win.Controls.ObjectTreeListNode)this.modelTreeList.FocusedNode;
					ModelTreeListNode treeListFocusedNode = (ModelTreeListNode)focusedNode.Object;
					modelTreeList.BeginUpdate();
					try {
						if(treeListFocusedNode.ModelTreeListNodeType != ModelTreeListNodeType.Primary) {
							ModelTreeListNode _primaryNode = treeListFocusedNode.PrimaryNode;
							ModelTreeListNode parent = treeListFocusedNode.Parent;
							ModelTreeListNodeType sourceType = treeListFocusedNode.ModelTreeListNodeType;
							parent.Childs.Remove(treeListFocusedNode);
							treeListFocusedNode.ClearChilds(false, false, true);
							ModelTreeListNode linkItem = new ModelTreeListNode((ExtendModelInterfaceAdapter)modelTreeList.Adapter, parent, _primaryNode.ModelNode, sourceType, _primaryNode);
							parent.Childs.Add(linkItem);
							focusedNode.Object = linkItem;
						}
						else {
							treeListFocusedNode.ClearChilds(false);
						}
						modelTreeList.ReBuildChildNodes(focusedNode);
					}
					finally {
						modelTreeList.EndUpdate();
					}
				}
				catch(Exception ex) {
					ModelEditorControllerBase.ShowError(ex);
				}
			}  
		}
		private void DestroyListViewLayout() {
			if(gridListEditorDesigner != null) {
				if(gridListEditorDesigner.DesignerControl != null) {
					gridListEditorDesigner.DesignerChanged -= gridListEditorDesigner_DesignerChanged;
					modelEditorPanelControl.PropertySplitContainer.Panel1.Controls.Remove(gridListEditorDesigner.DesignerControl);
				}
				gridListEditorDesigner.Dispose();
				gridListEditorDesigner = null;
			}
		}
		private void DestroyLayouts() {
			DestroyDetailViewLayout();
			DestroyListViewLayout();
		}
		private void DestroyDetailViewLayout() {
			if(layoutManager != null) {
				layoutManager.Container.Changed -= new EventHandler(Container_Changed);
				XafLayoutControl layoutControl = layoutManager.Container as XafLayoutControl;
				modelEditorPanelControl.PropertySplitContainer.Panel1.Controls.Remove(layoutControl);
				layoutControl.DefaultLayoutLoaded -= new EventHandler(layoutControl_DefaultLayoutLoaded);
				layoutManager.Dispose();
				layoutManager = null;
			}
		}
		public void ShowDetailViewLayout(ModelNode node, bool readOnly) {
			DestroyLayouts();
			layoutManager = new WinLayoutManager();
			XafLayoutControl layoutControl = layoutManager.Container as XafLayoutControl;
			if(layoutControl is ISupportImplementor) {
				XafLayoutControlImplementor obj = ((ISupportImplementor)layoutControl).Implementor as XafLayoutControlImplementor;
				if(obj != null) {
					obj.SetWinMode(IsWinLayout);
				}
			}
			IModelCompositeView compositeViewInfo = (IModelCompositeView)node.Parent;
			layoutControl.DefaultLayoutLoaded += new EventHandler(layoutControl_DefaultLayoutLoaded);
			ViewItemsCollection detailItemsCollection = new ViewItemsCollection();
			Type classType =
				(compositeViewInfo is IModelObjectView) && (((IModelObjectView)compositeViewInfo).ModelClass != null)
					? ((IModelObjectView)compositeViewInfo).ModelClass.TypeInfo.Type
					: null;
			foreach(IModelViewItem itemNode in compositeViewInfo.Items) {
				ViewItem editor = CreateEditor(classType, itemNode);
				try {
					detailItemsCollection.Add(editor);
				}
				catch(LightDictionaryException) {
				}
			}
			layoutManager.LayoutControls(node, detailItemsCollection);
			layoutManager.CustomizationEnabled = !readOnly;
			modelEditorPanelControl.PropertySplitContainer.Panel1.Controls.Add(layoutManager.Container);
			layoutManager.Container.Changed += new EventHandler(Container_Changed);
			modelAttributesEditor.Visible = false;
		}
		public void ShowLayoutIfNeed(ModelNode node, bool readOnly) {
			if(CanShowLayout(node)) {
				ShowDetailViewLayout(node, readOnly);
			}
			else {
				if(GridListEditorDesigner.CanShowDesigner(node)) {
					ShowListViewLayout(node);
				}
				else {
					modelAttributesEditor.Visible = true;
				}
			}
		}
		private IEnumerable<ModelTreeListNode> GetNonGroupNodes(ModelTreeListNode node) {
			if(node.ModelTreeListNodeType == ModelTreeListNodeType.Group) {
				return Enumerator.Convert<ModelTreeListNode>(((ExtendModelInterfaceAdapter)modelTreeList.Adapter).GetChildren(node));
			}
			return new ModelTreeListNode[] { node };
		}
		public List<ModelTreeListNode> SelectedNodes {
			get {
				List<ModelTreeListNode> result = new List<ModelTreeListNode>();
				if(modelTreeList.CheckBoxesMode) {
					foreach(ModelTreeListNode checkedNode in modelTreeList.GetCheckedNodes()) {
						result.AddRange(GetNonGroupNodes(checkedNode));
					}
				}
				else if(modelTreeList.State != DevExpress.XtraTreeList.TreeListState.IncrementalSearch && modelTreeList.Selection.Count > 0) {
					foreach(object item in modelTreeList.Selection) {
						result.AddRange(GetNonGroupNodes((ModelTreeListNode)((ObjectTreeListNode)item).Object));
					}
				}
				else {
					if(modelTreeList.FocusedNode != null && ((ModelTreeListNode)modelTreeList.FocusedNode.Object).ModelNode != null) {
						result.AddRange(GetNonGroupNodes((ModelTreeListNode)modelTreeList.FocusedNode.Object));
					}
				}
				return result;
			}
		}
		internal void SelectNodes(List<TreeListNode> nodes) {
			modelTreeList.Selection.Clear();
			modelTreeList.Selection.Add(nodes);
			PropertyGridSetSelectedObjects();
		}
		public bool MergeMode {
			get {
				return mergeMode;
			}
			set {
				mergeMode = value;
				InfoVisible = mergeMode;
				this.modelEditorPanelControl.InfoBoxPanel.Dock = mergeMode ? System.Windows.Forms.DockStyle.Top : System.Windows.Forms.DockStyle.Bottom;
				this.modelEditorPanelControl.InfoBox.InfoImage = ImageLoader.Instance.GetImageInfo(mergeMode ? "ModelEditor_ModelMerge_64x64" : "Action_AboutInfo_32x32").Image;
				this.modelEditorPanelControl.RootSplitContainer.PanelVisibility = mergeMode ? SplitPanelVisibility.Panel1 : SplitPanelVisibility.Both;
				modelTreeList.CheckBoxesMode = mergeMode;
				modelTreeList.OptionsSelection.MultiSelect = !mergeMode;
			}
		}
		public ModelNode CurrentModelNode {
			get {
				ModelTreeListNode focusedObject = modelTreeList.FocusedObject as ModelTreeListNode;
				return focusedObject != null ? focusedObject.ModelNode : null;
			}
			set {
				CurrentModelTreeListNode = ((ExtendModelInterfaceAdapter)modelTreeList.Adapter).GetPrimaryNode(value);
			}
		}
		public ModelTreeListNode CurrentModelTreeListNode {
			get {
				return modelTreeList.FocusedObject as ModelTreeListNode;
			}
			set {
				if(modelTreeList.FocusedObject != value) {
					try {
						modelTreeList.BeginUpdate();
						modelTreeList.FocusedObject = value;
						if(modelTreeList.FocusedObject == null) {
							ObjectTreeListNode node = modelTreeList.FindBuiltAncestorNode(value);
							if(node != null) {
								modelTreeList.FocusedObject = node.Object;
							}
						}
					}
					finally {
						modelTreeList.EndUpdate();
					}
				}
				if(modelTreeList.FocusedNode != null) {
					PropertyGridSetSelectedObjects();
				}
				else {
					modelAttributesEditor.PropertyGrid.SelectedObject = null;
				}
			}
		}
		public ModelTreeListNode RootModelTreeListNode {
			get {
				return ((ExtendModelInterfaceAdapter)modelTreeList.Adapter).RootNode;
			}
		}
		public ModelNode DataSource {
			get {
				ExtendModelInterfaceAdapter extendModelInterfaceAdapter = (ExtendModelInterfaceAdapter)modelTreeList.Adapter;
				return extendModelInterfaceAdapter.RootNode != null ? extendModelInterfaceAdapter.RootNode.ModelNode : null;
			}
			set {
				if(value != null) {
					ExtendModelInterfaceAdapter adapter = (ExtendModelInterfaceAdapter)modelTreeList.Adapter;
					if(adapter.RootNode == null || !object.ReferenceEquals(adapter.RootNode.ModelNode, value)) {
						CurrentModelTreeListNode = null;
						adapter.SetRootNode((ModelNode)value);
					}
					modelTreeList.DataSource = new ModelTreeListNode[] { (adapter.RootNode) };
					searchControl.UpdateControl();
				}
			}
		}
		public SearchControl SearchControl {
			get { return searchControl; }
		}
		public ActionsDXPopupMenu PopupMenu {
			get {
				return popupMenu;
			}
		}
		public DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerMenuBarItem PopupContainer {
			get {
				return cPopup;
			}
		}
		public bool InfoVisible {
			get {
				return modelEditorPanelControl.InfoBox.Visible;
			}
			set {
				if(!modelEditorPanelControl.InfoBox.Visible) {
					modelEditorPanelControl.InfoBox.Visible = value;
					modelEditorPanelControl.InfoBox.Visible = value;
				}
			}
		}
		public string InfoText {
			get { return modelEditorPanelControl.InfoBox.Text; }
			set {
				modelEditorPanelControl.InfoBox.Text = value;
				modelEditorPanelControl.InfoBoxPanel.Height = modelEditorPanelControl.InfoBox.Height;
			}
		}
		public ModelEditorPropertyGrid ModelPropertyGrid {
			get {
				return modelAttributesEditor;
			}
		}
		public SettingsStorage SettingsStorage {
			get {
				return settings;
			}
		}
		public ModelEditorPanelControl ModelEditorPanelControl {
			get {
				return modelEditorPanelControl;
			}
		}
		#region IDXPopupMenuHolder Members
		public Control PopupSite {
			get { return this.modelTreeList; }
		}
		public bool CanShowPopupMenu(Point position) {
			return true;
		}
		public void SetMenuManager(IDXMenuManager manager) {
			this.modelTreeList.MenuManager = manager;
		}
		#endregion
		#region IBarManagerHolder Members
		public DevExpress.XtraBars.BarManager BarManager {
			get {
				if(barManager == null) {
					barManager = new BarManager();
					barManager.Form = this;
					barManager.Items.Add(cPopup);
				}
				return barManager;
			}
		}
		public event EventHandler BarManagerChanged;
		#endregion
		public event CancelEventHandler LayoutChanged;
		public event EventHandler FocusedNodeChanged;
		public event EventHandler Closed;
		public event EventHandler ResetLayout;
		public void OnClosed() {
			if(Closed != null) {
				Closed(this, EventArgs.Empty);
			}
		}
#if DebugTest
		public const string DebugTest_ModelEditorControlSettingsPath = ModelEditorControlSettingsPath;
		public const string DebugTest_FocusedObjectAttributeName = FocusedObjectAttributeName;
		public void DebugTest_SafeFreeLayoutManager() {
			SafeFreeLayoutManager();
		}
		public WinLayoutManager DebugTest_LayoutManager {
			get { return layoutManager; }
		}
#endif
		#region Obsolete 15.1
		[Obsolete("Use the RebuildModelNode method instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void RebuidModelNode(ModelTreeListNode modelNode) {
			RebuildModelNode(modelNode);
		}
		#endregion
	}
	[Browsable(false), DesignTimeVisible(false)]
	public class ModelEditorDesignDetailViewItem : ViewItem {
		private IModelViewItem model;
		protected override object CreateControlCore() {
			Control control = null;
			if((EditorType != null && typeof(ListPropertyEditor).IsAssignableFrom(EditorType)) || model is IModelDashboardViewItem) {
				control = new LargeStringEdit();
			}
			else {
				control = new StringEdit();
			}
			control.Enabled = false;
			return control;
		}
		public ModelEditorDesignDetailViewItem(IModelViewItem model, Type classType)
			: base(classType, model.Id) {
			this.model = model;
		}
		public override bool IsCaptionVisible {
			get {
				if((model is IModelStaticText)
					|| (model is IModelStaticImage)
					|| (model is IModelActionContainerViewItem)
					|| (model is IModelDashboardViewItem)) {
					return false;
				}
				return true;
			}
		}
		public Type EditorType {
			get {
				if(model is IModelPropertyEditor) {
					return ((IModelPropertyEditor)model).PropertyEditorType;
				}
				return null;
			}
		}
		public override string Caption {
			get {
				if(!String.IsNullOrEmpty(model.Caption)) {
					return model.Caption;
				}
				else if(model is IModelPropertyEditor) {
					return ((IModelPropertyEditor)model).PropertyName;
				}
				else {
					return model.Id;
				}
			}
			set {
				model.Caption = value;
			}
		}
	}
	[ToolboxItem(false)]
	[Browsable(false), DesignTimeVisible(false)]
	public class ModelEditorBarManager : BarManager {
		protected override bool IsOriginFormActive {
			get {
				return DesignerOnlyCalculator.IsRunFromDesigner || base.IsOriginFormActive;
			}
		}
	}
}
