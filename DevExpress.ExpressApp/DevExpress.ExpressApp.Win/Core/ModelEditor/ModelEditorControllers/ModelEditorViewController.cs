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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Utils.Reflection;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Win.Core.ModelEditor.NodesTree;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor {
	public class ModelEditorViewController : ModelEditorDragDropController, IModelEditorController {
		#region Fields
		private const string unusableNodesError = "Your model differences were found to contain nodes that are no longer usable, " +
					"possibly due to a version upgrade of XAF or an incompatible module list. " +
					"These unusable nodes have been saved in files called UnusableNodes*.xafml in the user model data location. " +
					"Update the Application Model using the UnusableNodes*.xafml file's content.";
		public const string LanguagesManagerLaunchItemName = "Languages Manager...";
		private const string additionLanguageMessageFormat = "To load localized property values from satellite assemblies, restart the {0}.";
		private string mergeInfoMessage = "<b>To merge model differences into one or more available targets, do the following:</b>" + Environment.NewLine +
										" 1) select nodes to merge via checkboxes;" + Environment.NewLine +
										" 2) select a merge target in the dropdown;" + Environment.NewLine +
										" 3) click <b>Merge</b> to start merge operation;" + Environment.NewLine +
										" 4) click <b>Save</b> to persist changes and close this dialog.";
		private const string CurrentAspectAttributeName = "CurrentAspect";
		private readonly object lockObj = new object();
		private ModelValidator _validator;
		private ModelDifferenceStore diffstore;
		private bool mergeDiffsMode = false;
		private bool isShowDifferences = true;
		private bool canChangeAspect = true;
		private bool updateActionStateLocked = false;
		private bool needUpdateActionState = false;
		private bool propertyChanging = false;
		private bool isUnusableNodesErrorShown = false;
		private bool linksCollecting = false;
		private bool lockUpdateActionState = false;
		private SimpleAction openAction;
		private SimpleAction saveAction;
		private SimpleAction deleteAction;
		private SimpleAction navigationBackAction;
		private SimpleAction navigationForwardAction;
		private SimpleAction findAction;
		private SimpleAction copyAction;
		private SimpleAction pasteAction;
		private SimpleAction cloneAction;
		private SimpleAction generateContentAction;
		private SimpleAction loadedModulesAction;
		private SimpleAction showUnusableNodesAction;
		private SimpleAction showDifferences;
		private SimpleAction resetDifferencesAction;
		private SingleChoiceAction chooseMergeModuleAction;
		private SimpleAction mergeDifferencesAction;
		private SimpleAction nodeDownAction;
		private SimpleAction nodeUpAction;
		private SimpleAction showLocalizationForm;
		private SimpleAction reloadAction;
		private SimpleAction groupNodesAction;
		private SimpleAction goSourceNodeAction;
		private SingleChoiceAction addNodeAction;
		private SingleChoiceAction changeAspectAction;
		private NavigationHistory<ModelTreeListNode> navigationPoints = new NavigationHistory<ModelTreeListNode>();
		private List<ModelTreeListNode> nodeBuffers = new List<ModelTreeListNode>();
		private Dictionary<ActionBase, string> mainBarActions = new Dictionary<ActionBase, string>();
		private List<ActionBase> popupMenuActions = new List<ActionBase>();
		private Dictionary<ModelApplicationBase, ModelDifferenceStore> modelDiffsStoresForSave = new Dictionary<ModelApplicationBase, ModelDifferenceStore>();
		private ModelAttributesPropertyEditorControllerWin _modelAttributesPropertyEditorController;
		private List<ModuleDiffStoreInfo> modulesDiffStoreInfoToDispose = null;
		private ModuleDiffStoreInfo currentMergeModuleInfo;
		private ModelTreeListNode newNodeForValidation = null;
		private int lockErrorMessages = 0;
		private ErrorMessages errorMessages;
		private string validationContext = "";
		private LinksNodeHelper linksNodeHelper;
		#endregion
		public static DialogResult CloseModelEditorDialog {
			get {
				return XtraMessageBox.Show("Do you want to save changes?", "You are about to leave the Model Editor", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
			}
		}
		public ModelEditorViewController(IModelApplication modelApplication, ModelDifferenceStore diffstore)
			: this(modelApplication) {
			this.diffstore = diffstore;
			OnModelLoaded();
		}
		private ModelEditorViewController(IModelApplication modelApplication)
			: base(modelApplication) {
			CreateActions();
		}
		bool isDisposing = false;
		public bool IsDisposing {
			get {
				return isDisposing;
			}
			set {
				if(value) {
					isDisposing = value;
				}
			}
		}
		public bool MergeDiffsMode {
			get {
				return mergeDiffsMode;
			}
			set {
				if(mergeDiffsMode != value) {
					mergeDiffsMode = value;
					if(mergeDiffsMode) {
						popupMenuActions.Remove(mergeDifferencesAction);
					}
					else {
						popupMenuActions.Add(mergeDifferencesAction);
					}
				}
			}
		}
		public ModelAttributesPropertyEditorControllerWin ModelAttributesPropertyEditorController {
			get {
				if(_modelAttributesPropertyEditorController == null) {
					_modelAttributesPropertyEditorController = new ModelAttributesPropertyEditorControllerWin(Adapter.fastModelEditorHelper);
				}
				return _modelAttributesPropertyEditorController;
			}
#if DebugTest
			set {
				_modelAttributesPropertyEditorController = value;
			}
#endif
		}
		public override void SetControl(ModelEditorControl _modelEditorControl) {
			base.SetControl(_modelEditorControl);
			ModelAttributesPropertyEditorController.SetControl(_modelEditorControl.ModelPropertyGrid.PropertyGrid);
			if(mergeDiffsMode) {
				ModelEditorControl.InfoVisible = true;
				ModelEditorControl.InfoText = mergeInfoMessage;
				ModelEditorControl.MergeMode = true;
				ExtendModelInterfaceAdapter.LinksEnabled = false;
			}
			ModelEditorControl.DataSource = ((ModelNode)ModelApplication);
			CurrentModelNode = _modelEditorControl.CurrentModelTreeListNode;
		}
		public void SetTemplate(IFrameTemplate frameTemplate) {
			if(frameTemplate != null) {
				IBarManagerHolder barManagerHolder;
				frameTemplate.DefaultContainer.BeginUpdate();
				foreach(KeyValuePair<ActionBase, string> action in mainBarActions) {
					if(string.IsNullOrEmpty(action.Value)) {
						frameTemplate.DefaultContainer.Register(action.Key);
					}
					else {
						IActionContainer actionContainer = FindActionContainer(action.Value, frameTemplate.GetContainers());
						if(actionContainer != null) {
							actionContainer.Register(action.Key);
						}
						else {
							frameTemplate.DefaultContainer.Register(action.Key);
						}
					}
				}
				frameTemplate.DefaultContainer.EndUpdate();
				barManagerHolder = frameTemplate as IBarManagerHolder;
				ModelEditorControl.PopupContainer.Manager = barManagerHolder.BarManager;
				ModelEditorControl.PopupContainer.BeginUpdate();
				foreach(ActionBase action in popupMenuActions) {
					ModelEditorControl.PopupContainer.Register(action);
				}
				ModelEditorControl.PopupContainer.EndUpdate();
				ModelEditorControl.PopupMenu.CreateActionItems(barManagerHolder, ModelEditorControl, new IActionContainer[] { ModelEditorControl.PopupContainer });
			}
			else {
				ContextMenuStrip contextMenu = new ContextMenuStrip();
				ModelEditorControl.modelTreeList.ContextMenuStrip = contextMenu;
				contextMenu.Opening += new CancelEventHandler(delegate(object sender, CancelEventArgs e) {
					e.Cancel = false; 
					MenuStripItemFactory.BuildMenu(contextMenu, popupMenuActions);
				});
			}
		}
		public void ReloadModel(bool askConfirmation, bool refreshAdapter) {
			if(!askConfirmation ||
					   DialogResult.OK == Messaging.GetMessaging(null).Show("All unsaved changes will be lost.", "Reloading the Application Model", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning)) {
				ReloadModelCore(refreshAdapter);
			}
		}
		public void ShowLocalizationForm() {
			ModelTreeListNode preVisibleNode = CurrentModelNode;
			using(LocalizationForm localizationForm = new LocalizationForm()) {
				LocalizationController controller = new LocalizationController(ModelApplicationCore, localizationForm, diffstore);
				try {
					controller.SaveChanges += controller_SaveChanges;
					controller.Modifying += controller_Modifying;
					controller.SetSettingsStorage(new SettingsStorageOnRegistry(@"Software\Developer Express\eXpressApp Framework\Model Editor\Localization"));
					controller.RegisterLocalizationImportExport(new CsvImportExport());
					if(localizationForm.ShowDialog() == DialogResult.Yes) {
						TrySetModified();
						ModelEditorControl.RebuildModelNode(ModelEditorControl.RootModelTreeListNode);
					}
				}
				finally {
					controller.SaveChanges -= controller_SaveChanges;
					controller.Modifying -= controller_Modifying;
				}
			}
			CurrentModelNode = preVisibleNode;
		}
		private void controller_Modifying(object sender, CancelEventArgs e) {
			e.Cancel = !TrySetModified();
		}
		private void controller_SaveChanges(object sender, EventArgs e) {
			InternalSave();
		}
		protected override void UnSubscribeEvents() {
			base.UnSubscribeEvents();
			if(ModelEditorControl != null) {
				ModelEditorControl.LayoutChanged -= new CancelEventHandler(ModelEditorControl_LayoutChanged);
				ModelEditorControl.ModelPropertyGrid.PropertyGrid.FocusedRowChanged -= new DevExpress.XtraVerticalGrid.Events.FocusedRowChangedEventHandler(modelAttributesEditor_FocusedRowChanged);
				ModelEditorControl.modelTreeList.BeforeFocusNode -= new BeforeFocusNodeEventHandler(modelTreeList_BeforeFocusNode);
				ModelEditorControl.modelTreeList.BeforeExpand -= new BeforeExpandEventHandler(modelTreeList_BeforeExpand);
				ModelEditorControl.modelTreeList.BeforeCollapse -= new BeforeCollapseEventHandler(modelTreeList_BeforeCollapse);
				ModelEditorControl.ModelPropertyGrid.PropertyGrid.HandleException -= new EventHandler<CustomHandleExceptionEventArgs>(PropertyGrid_HandleException);
				if(mergeDiffsMode) {
					ModelEditorControl.modelTreeList.AfterCheckNode -= new NodeEventHandler(modelTreeList_AfterCheckNode);
				}
				ModelEditorControl.modelTreeList.LostFocus -= lostFocusEventHandler;
				ModelEditorControl.modelTreeList.GotFocus -= gotFocusEventHandler;
				ModelEditorControl.ModelPropertyGrid.GotFocus -= modelPropertyGridGotFocusEventHandler;
				ModelEditorControl.modelTreeList.NodeIndexChanged -= nodeIndexChangedEventHandler;
				ModelEditorControl.Closed -= closedEventHandler;
				ModelEditorControl.FocusedNodeChanged -= focusedNodeChangedEventHandler;
			}
			if(_modelAttributesPropertyEditorController != null) {
				_modelAttributesPropertyEditorController.NavigateToRefProperty -= navigateToRefPropertyEventHandler;
				_modelAttributesPropertyEditorController.PropertyChanging -= propertyChangingEventHandler;
				_modelAttributesPropertyEditorController.PropertyChanged -= propertyChangedEventHandler;
			}
		}
		protected override void SubscribeEvents() {
			base.SubscribeEvents();
			ModelEditorControl.LayoutChanged += new CancelEventHandler(ModelEditorControl_LayoutChanged);
			ModelEditorControl.ResetLayout += new EventHandler(ModelEditorControl_ResetLayout);
			ModelEditorControl.ModelPropertyGrid.PropertyGrid.FocusedRowChanged += new DevExpress.XtraVerticalGrid.Events.FocusedRowChangedEventHandler(modelAttributesEditor_FocusedRowChanged);
			ModelEditorControl.modelTreeList.BeforeFocusNode += new BeforeFocusNodeEventHandler(modelTreeList_BeforeFocusNode);
			ModelEditorControl.modelTreeList.BeforeExpand += new BeforeExpandEventHandler(modelTreeList_BeforeExpand);
			ModelEditorControl.modelTreeList.BeforeCollapse += new BeforeCollapseEventHandler(modelTreeList_BeforeCollapse);
			ModelEditorControl.ModelPropertyGrid.PropertyGrid.HandleException += new EventHandler<CustomHandleExceptionEventArgs>(PropertyGrid_HandleException);
			if(mergeDiffsMode) {
				ModelEditorControl.modelTreeList.AfterCheckNode += new NodeEventHandler(modelTreeList_AfterCheckNode);
			}
			navigateToRefPropertyEventHandler = SafeHandler<NavigateToEventArgs>(new EventHandler<NavigateToEventArgs>(modelAttributesEditor_NavigateTo));
			lostFocusEventHandler = SafeHandler(new EventHandler(modelTreeList_LostFocus));
			propertyChangingEventHandler = SafeHandler<PropertyChangingEventArgs>(new EventHandler<PropertyChangingEventArgs>(propertyGridHelper_PropertyChanging));
			propertyChangedEventHandler = SafeHandler<PropertyChangingEventArgs>(new EventHandler<PropertyChangingEventArgs>(propertyGridHelper_PropertyChanged));
			gotFocusEventHandler = SafeHandler(new EventHandler(modelTreeList_GotFocus));
			modelPropertyGridGotFocusEventHandler = SafeHandler(new EventHandler(modelAttributesEditor_GotFocus));
			nodeIndexChangedEventHandler = SafeHandler<NodeIndexChangedEventArgs>(new EventHandler<NodeIndexChangedEventArgs>(modelTreeList_NodeIndexChanged));
			closedEventHandler = SafeHandler(new EventHandler(ModelEditorControl_Closed));
			focusedNodeChangedEventHandler = SafeHandler(new EventHandler(ModelEditorControl_FocusedNodeChanged));
			ModelAttributesPropertyEditorController.NavigateToRefProperty += navigateToRefPropertyEventHandler;
			ModelAttributesPropertyEditorController.PropertyChanging += propertyChangingEventHandler;
			ModelAttributesPropertyEditorController.PropertyChanged += propertyChangedEventHandler;
			ModelEditorControl.modelTreeList.LostFocus += lostFocusEventHandler;
			ModelEditorControl.modelTreeList.GotFocus += gotFocusEventHandler;
			ModelEditorControl.ModelPropertyGrid.GotFocus += modelPropertyGridGotFocusEventHandler;
			ModelEditorControl.modelTreeList.NodeIndexChanged += nodeIndexChangedEventHandler;
			ModelEditorControl.Closed += closedEventHandler;
			ModelEditorControl.FocusedNodeChanged += focusedNodeChangedEventHandler;
		}
		protected override void LinksCollecting() {
			base.LinksCollecting();
			linksCollecting = true;
		}
		public static readonly object LockDisposeObject = new object();
		protected override void LinksCollected() {
			base.LinksCollected();
			if(!Disposing && !IsDisposing) {
				linksCollecting = false;
				if(ModelEditorControl != null) {
					if(ModelEditorControl.IsHandleCreated) {
						if(ModelEditorControl.InvokeRequired) {
							MethodInvoker simpleDelegate = new MethodInvoker(delegate {
								if(!Disposing && !IsDisposing) {
									lock(LockDisposeObject) {
										if(!Disposing && !IsDisposing) {
											ModelEditorControl.BeginInvoke(new System.Threading.ThreadStart(delegate() {
												UpdateActionState();
											}));
										}
									}
								}
							});
							simpleDelegate.BeginInvoke(null, null);
						}
					}
					else {
						ModelEditorControl.HandleCreated += new EventHandler(ModelEditorControl_HandleCreated);
					}
				}
			}
		}
		private void ModelEditorControl_HandleCreated(object sender, EventArgs e) {
			ModelEditorControl.HandleCreated -= new EventHandler(ModelEditorControl_HandleCreated);
			UpdateActionState();
		}
		protected override void ModelNodePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
			base.ModelNodePropertyChanged(sender, e);
			SafeExecute(delegate() {
				try {
					lockUpdateActionState = true;
					TrySetModified();
					if(LockUpdate == 0 && ModelTreeListNode.LockModelNodeEvents == 0) {
						UpdatedTreeListCall(delegate() {
							if(ModelEditorControl != null) {
								foreach(ModelTreeListNode node in SelectedNodes) {
									if(e.PropertyName.StartsWith("Freeze")) {
										ModelTreeListNode lockCurrentModelNode = CurrentModelNode;
										string targetNodePath = ModelEditorHelper.GetModelNodePath(lockCurrentModelNode.ModelNode);
										string ownerNodePath = ModelEditorHelper.GetModelNodePath(lockCurrentModelNode.Owner.ModelNode);
										ModelTreeListNode primary = node.PrimaryNode;
										try {
											Adapter.BeginUpdate();
											primary.SetModelNode(primary.ModelNode, true);
										}
										finally {
											Adapter.EndUpdate();
										}
										FocusModelNodeIfNeed(targetNodePath, ownerNodePath);
										RefreshCurrentModelNode();
									}
								}
							}
							if(ValidateNode(CurrentModelNode, false)) {
								ApdateTreeNodesIfNeed(CurrentModelNode, e.PropertyName);
							}
						});
					}
				}
				finally {
					lockUpdateActionState = false;
					propertyChanging = false;
					UpdateActionState();
				}
			});
		}
		private void ApdateTreeNodesIfNeed(ModelTreeListNode currentModelNode, string propertyName) {
			if(currentModelNode != null) {
				ModelValueInfo modelValueInfo = currentModelNode.ModelNode.GetValueInfo(propertyName);
				if(modelValueInfo != null) {
					ModelRefreshTreeNode refreshTreeNode = Adapter.fastModelEditorHelper.GetPropertyAttribute<ModelRefreshTreeNode>(currentModelNode.ModelNode, propertyName);
					if(refreshTreeNode != null) {
						UpdatedTreeListCall(delegate() {
							ModelTreeListNode lockCurrentModelNode = CurrentModelNode;
							ModelNode currentModel = lockCurrentModelNode.ModelNode;
							Unsubscribe(CurrentModelNode.ModelNode);
							Adapter.DeleteNode(CurrentModelNode, false);
							CurrentModelNode = Adapter.GetPrimaryNode(currentModel);
							RefreshCurrentModelNode();
						});
					}
				}
			}
		}
		private ModelApplicationBase CreateModelApplicationByModule(ModuleBase module, ModelStoreBase modelStore, out ApplicationModulesManager manager) {
#if DebugTest
			if(DebugTest_CustomizeModel != null) {
				DebugTest_CustomizeModel(module, EventArgs.Empty);
			}
#endif
			DesignerModelFactory dmf = new DesignerModelFactory();
			manager = dmf.CreateModulesManager(module, null);
			return (ModelApplicationBase)dmf.CreateApplicationModel(module, manager, modelStore);
		}
		private void ReloadModelCore(bool refreshAdapter) {
			ModelEditorHelper.RereadLastLayer(ModelApplication, diffstore);  
			IsModified = false;
			if(refreshAdapter) {
				UpdatedTreeListCall(delegate() {
					if(ModelEditorControl != null) {
						try {
							lockErrorMessages++;
							lockUpdateActionState = true;
							ModelNode focusedModelNode = GetFocusedModelNodeAfterReload(GetModelNode(CurrentModelNode));
							string focusedObjectPath = ModelEditorHelper.GetModelNodePath(focusedModelNode);
							CurrentModelNode = null;
							Adapter.SetRootNode(ModelApplicationCore);
							ModelEditorControl.RebuildModelNode(Adapter.RootNode);
							focusedModelNode = ModelEditorHelper.FindNodeByPath(focusedObjectPath, Adapter.RootNode.ModelNode);
							if(focusedModelNode != null) {
								ModelEditorControl.CurrentModelTreeListNode = Adapter.GetPrimaryNode(focusedModelNode);
							}
						}
						finally {
							lockErrorMessages--;
							lockUpdateActionState = false;
						}
					}
				});
				UpdateActionState();
			}
			else {
				UnSubscribeEvents();
			}
		}
		private void OnModelLoaded() {
			if(ModelApplicationCore != null) {
				if(ModelApplicationCore.CurrentAspectProvider == null) {
					ModelApplicationCore.CurrentAspectProvider = new CurrentAspectProvider();
				}
				else {
					ModelApplicationCore.CurrentAspectProvider.CurrentAspect = CaptionHelper.DefaultLanguage;
					Guard.TypeArgumentIs(typeof(CurrentAspectProvider), ModelApplicationCore.CurrentAspectProvider.GetType(), "Wrong CurrentAspectProvider type.");
				}
				if(ModelEditorControl != null) {
					navigationPoints.Clear();
					ModelEditorControl.DataSource = ((ModelNode)ModelApplicationCore);
					CurrentModelNode = ModelEditorControl.CurrentModelTreeListNode;
				}
				else {
					UpdateActionState();
				}
			}
		}
		private void OnCustomLoadModel() {
			if(CustomLoadModel != null) {
				CustomLoadModelEventArgs args = new CustomLoadModelEventArgs();
				CustomLoadModel(this, args);
				if(args.ModelApplication != null) {
					SetModelApplication(args.ModelApplication as ModelApplicationBase);
					this.diffstore = args.ModelDifferenceStore;
					OnModelLoaded();
				}
			}
		}
		private ModelNode GetFocusedModelNodeAfterReload(ModelNode currentModelNode) {
			ModelNode result = currentModelNode;
			while(result != null && result.IsNewNode) {
				result = result.Parent;
			}
			return result;
		}
		private ModelNode GetChild(object parent, int index) {
			ModelInterfaceAdapter adapter = new ModelInterfaceAdapter();
			int counter = 0;
			foreach(ModelNode node in adapter.GetChildren(parent)) {
				if(counter == index) {
					return node;
				}
				counter++;
			}
			return null; ;
		}
		private int GetNodeIndex_Down(ModelTreeListNode node, List<ModelTreeListNode> selectedNodes, IEnumerable collection) {
			int result = -1;
			int nodeIndex = GetNodePositionInCollection(node, collection);
			if(node.Parent.ModelNode != null && nodeIndex < node.Parent.ModelNode.NodeCount - 1) {
				ModelNode child = GetChild(node.Parent.ModelNode, nodeIndex + 1);
				if(child == null) {
					return result;
				}
				int? nextNodeIndex = child.Index;
				if(selectedNodes != null && (nextNodeIndex > 0 || nextNodeIndex == null)) {
					int nodeIndexInSelectedNodes = GetNodePositionInCollection(node, selectedNodes);
					if(node.Parent.ModelNode.NodeCount - 1 - nodeIndex != nodeIndexInSelectedNodes) {
						return nodeIndex + 1;
					}
				}
			}
			return result;
		}
		private int GetNodeIndex_Up(ModelTreeListNode node, List<ModelTreeListNode> selectedNodes, IEnumerable collection) {
			int result = -1;
			int nodeIndex = GetNodePositionInCollection(node, collection);
			if(nodeIndex != 0 && selectedNodes != null) {
				int nodeIndexInSelectedNodes = GetNodePositionInCollection(node, selectedNodes);
				if(nodeIndex != nodeIndexInSelectedNodes) {
					return nodeIndex - 1;
				}
			}
			return result;
		}
		private int GetNodePositionInCollection(ModelTreeListNode node, System.Collections.IEnumerable collection) {
			if(node.Parent == null) {
				return 0;
			}
			int result = 0;
			int counter = 0;
			string nodeId = node.ModelNode.Id;
			foreach(ModelTreeListNode item in collection) {
				if(nodeId == item.ModelNode.Id) {
					result = counter;
					break;
				}
				counter++;
			}
			return result;
		}
		private void NodeIndexChanged() {
			if(ModelEditorControl != null) {
				List<ObjectTreeListNode> parentsForRefresh = new List<ObjectTreeListNode>();
				if(ModelEditorControl.modelTreeList.Selection.Count > 1) {
					foreach(ObjectTreeListNode selectedNode in ModelEditorControl.modelTreeList.Selection) {
						if(!parentsForRefresh.Contains((ObjectTreeListNode)selectedNode.ParentNode)) {
							parentsForRefresh.Add((ObjectTreeListNode)selectedNode.ParentNode);
						}
					}
				}
				else {
					ModelTreeListNode lockCurrentModelNode = CurrentModelNode;
					if(lockCurrentModelNode != null) {
						ObjectTreeListNode controlNode = ModelEditorControl.modelTreeList.FindBuiltAncestorNode(lockCurrentModelNode.Parent);
						parentsForRefresh.Add(controlNode);
					}
				}
				foreach(ObjectTreeListNode node in parentsForRefresh) {
					ModelEditorControl.modelTreeList.RefreshChildrenOrder(node);
				}
				ModelEditorControl.ModelPropertyGrid.PropertyGrid.RefreshValues(); ;
			}
		}
		private bool HasSelectedNodesModification() {
			foreach(ModelTreeListNode node in SelectedNodes) {
				if(node.ModelNode.HasModification) {
					return true;
				}
			}
			return false;
		}
		private bool CanCloneNodes(List<ModelTreeListNode> nodes) {
			bool result = false;
			foreach(ModelTreeListNode node in nodes) {
				result = IsEditableNode(node);
				if(result) {
					result = Adapter.fastModelEditorHelper.CanAddNode(GetModelNode(node.ParentIgnoreGroup), node.ModelNode);
				}
				if(!result) {
					break;
				}
			}
			return result;
		}
		private bool SelectedNodesIsOneLevel {
			get {
				if(SelectedNodes.Count > 1) {
					List<ModelTreeListNode> parents = new List<ModelTreeListNode>();
					foreach(ModelTreeListNode selectedNode in SelectedNodes) {
						parents.Add(selectedNode.Parent);
					}
					foreach(ModelTreeListNode selectedNode in SelectedNodes) {
						if(selectedNode.Parent == null) {
							return false;
						}
						foreach(ModelTreeListNode parent in parents) {
							if(!ModelEditorHelper.IsNodeEqual(parent.ModelNode, GetModelNode(selectedNode.Parent))) {
								return false;
							}
						}
					}
					return true;
				}
				else {
					return true;
				}
			}
		}
		private bool NodesIsPrimary(List<ModelTreeListNode> nodes) {
			foreach(ModelTreeListNode node in nodes) {
				if(node.ModelTreeListNodeType != ModelTreeListNodeType.Primary) {
					return false;
				}
			}
			return true;
		}
		private List<ModelTreeListNode> NodeBuffers {
			get {
				List<ModelTreeListNode> result = new List<ModelTreeListNode>();
				foreach(ModelTreeListNode node in nodeBuffers) {
					if(node.ModelNode != null) {
						result.Add(node);
					}
				}
				return result;
			}
		}
		private void AddStartNavigationPoint() {
			if(navigationPoints.Count == 0 && CurrentModelNode != null) {
				navigationPoints.Add(CurrentModelNode);
			}
		}
		private void AddNavigationPoint() {
			if(CurrentModelNode != null) {
				navigationPoints.Add(CurrentModelNode);
			}
		}
		private void modelAttributesEditor_NavigateTo(object sender, NavigateToEventArgs e) {
			NavigateRefProperty(e.Node, e.PropertyName);
		}
		private void FocusModelNodeIfNeed(string targetNodePath, string ownerNodePath) {
			bool islinkedNode = ownerNodePath != null && targetNodePath != ownerNodePath;
			if(islinkedNode) {
				CurrentModelNode = FindLinksModelNode(targetNodePath, ownerNodePath);
			}
		}
		private void ModelEditorControl_ResetLayout(object sender, EventArgs e) {
			ModelTreeListNode lockCurrentModelNode = CurrentModelNode;
			if(lockCurrentModelNode.ModelNode.HasModification) {
				if(TrySetModified()) {
					UpdatedTreeListCall(delegate() {
						string targetNodePath = ModelEditorHelper.GetModelNodePath(lockCurrentModelNode.ModelNode);
						string ownerNodePath = ModelEditorHelper.GetModelNodePath(lockCurrentModelNode.Owner.ModelNode);
						lockCurrentModelNode.ModelNode.Undo();
						try {
							Adapter.BeginUpdate();
							lockCurrentModelNode.ClearChilds(true);
						}
						finally {
							Adapter.EndUpdate();
						}
						FocusModelNodeIfNeed(targetNodePath, ownerNodePath);
						RefreshCurrentModelNode();
					});
				}
			}
		}
		private void InternalSave() {
			if(TrySetModified()) {
				if(diffstore != null) {
					IFileModelStore fStore = diffstore as IFileModelStore;
					if(fStore != null && !TrySaveBoFiles(fStore.GetBoFilesToSave(ModelApplicationCore.LastLayer).ToArray())) {
						return;
					}
					foreach(KeyValuePair<ModelApplicationBase, ModelDifferenceStore> modelDiffsStore in modelDiffsStoresForSave) {
						modelDiffsStore.Value.SaveDifference(modelDiffsStore.Key);
					}
					diffstore.SaveDifference(ModelApplicationCore.LastLayer);
					if(!isUnusableNodesErrorShown) {
						ModelApplicationBase unusableModel = ModelEditorHelper.GetUnusableModel(ModelApplicationCore.LastLayer);
						if(unusableModel != null && unusableModel.HasModification) {
							isUnusableNodesErrorShown = true;
							MessageBox.Show(unusableNodesError, "Update Model", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						}
					}
				}
				IsModified = false;
				UpdateActionState();
			}
		}
		private void ModelEditorControl_LayoutChanged(object sender, CancelEventArgs e) {
			SafeExecute(delegate() {
				e.Cancel = !TrySetModified();
			});
		}
		private void propertyGridHelper_PropertyChanging(object sender, PropertyChangingEventArgs e) {
			SafeExecute(delegate() {
				e.Cancel = !TrySetModified();
				propertyChanging = true;
			});
		}
		private void propertyGridHelper_PropertyChanged(object sender, PropertyChangingEventArgs e) {
			SafeExecute(delegate() {
				propertyChanging = false;
				UpdateActionState();
			});
		}
		private void ModelEditorControl_FocusedNodeChanged(object sender, EventArgs e) {
			if(CurrentNodeChanged != null) {
				CurrentNodeChanged(sender, e);
			}
			CurrentModelNode = ModelEditorControl.CurrentModelTreeListNode;
		}
		private void modelAttributesEditor_FocusedRowChanged(object sender, DevExpress.XtraVerticalGrid.Events.FocusedRowChangedEventArgs e) {
			SafeExecute(delegate() {
				string propertyName = null;
				if(e.Row != null) {
					propertyName = e.Row.Properties.FieldName;
				}
				SetDescription(GetModelTreeListNodeDescription(CurrentModelNode, propertyName));
			});
		}
		private void modelTreeList_NodeIndexChanged(object sender, NodeIndexChangedEventArgs e) {
			if(DesignMode) {
				if(e.IsIndexUp) {
					ExecuteAction(nodeUpAction);
				}
				else {
					ExecuteAction(nodeDownAction);
				}
			}
		}
		private string GetModelDifferencesText() {
			string result = "";
			foreach(ModelTreeListNode selectedNode in SelectedNodes) {
				ModelNode difNode = ModelEditorHelper.GetNodeInLayer(selectedNode.ModelNode, ModelApplicationCore.LastLayer);
				if(difNode != null) {
					result += difNode.Xml;
					result += Environment.NewLine;
				}
			}
			return result;
		}
		private void PropertyGrid_HandleException(object sender, CustomHandleExceptionEventArgs e) {
			ShowError(e.Exception);
		}
		private void SaveSelectedLanguage() {
			if(ModelApplicationCore != null && ModelEditorControl != null && ModelEditorControl.SettingsStorage != null) {
				ModelEditorControl.SettingsStorage.SaveOption(ModelEditorControl.ModelEditorControlSettingsPath, CurrentAspectAttributeName, CurrentAspect);
			}
		}
		private void LoadSelectedLanguage() {
			if(ModelApplicationCore != null && ModelEditorControl != null && ModelEditorControl.SettingsStorage != null) {
				string aspectName = ModelEditorControl.SettingsStorage.LoadOption(ModelEditorControl.ModelEditorControlSettingsPath, CurrentAspectAttributeName);
				if(!string.IsNullOrEmpty(aspectName)) {
					ChoiceActionItem aspectActionItem = changeAspectAction.FindItemByCaptionPath(aspectName);
					if(aspectActionItem != null) {
						changeAspectAction.DoExecute(aspectActionItem);
					}
				}
			}
		}
		private void modelAttributesEditor_GotFocus(object sender, EventArgs e) {
			if(ModelEditorControl.ModelPropertyGrid.PropertyGrid.SelectedObject != null) {
				SetDescription(GetModelTreeListNodeDescription(CurrentModelNode, ModelEditorControl.ModelPropertyGrid.PropertyGrid.FocusedPropertyName));
			}
		}
		private void modelTreeList_GotFocus(object sender, EventArgs e) {
			SetDescription(GetModelTreeListNodeDescription(CurrentModelNode, null));
			UpdateActionState();
			controlPressed = false;
			controlAltPressed = false;
		}
		private void modelTreeList_LostFocus(object sender, EventArgs e) {
			if(!IsDisposing) {
				UpdateActionState();
				controlPressed = false;
				controlAltPressed = false;
			}
		}
		private void SetDescription(string description) {
			if(!mergeDiffsMode && ModelEditorControl != null) {
				ModelEditorControl.InfoVisible = !string.IsNullOrEmpty(description);
				if(ModelEditorControl.InfoVisible) {
					ModelEditorControl.InfoText = description;
				}
			}
		}
		private string GetModelTreeListNodeDescription(ModelTreeListNode modelTreeListNode, string propertyName) {
			string result = string.Empty;
			if(modelTreeListNode != null) {
				if(modelTreeListNode.ModelTreeListNodeType == ModelTreeListNodeType.Group) {
					result = "Groups associated nodes.";
				}
				else {
					if(modelTreeListNode.ModelTreeListNodeType == ModelTreeListNodeType.Links) {
						result = "Lists related nodes.";
					}
					else {
						ModelNode node = GetModelNode(modelTreeListNode);
						result = GetDescription(node, propertyName);
					}
				}
			}
			return result;
		}
		private string GetDescription(ModelNode node, string propertyName) {
			string result = string.Empty;
			if(node != null) {
				if(!string.IsNullOrEmpty(propertyName) && !ModelEditorControl.modelTreeList.Focused) {
					result = Adapter.fastModelEditorHelper.GetPropertyDescription(node, propertyName);
				}
				else {
					result = Adapter.fastModelEditorHelper.GetNodeDescription(node);
				}
			}
			return result;
		}
		private void UpdateNewNode(ModelTreeListNode node) {
			if(node.ModelNode is IModelMember) {
				((IModelMember)node.ModelNode).IsCustom = true;
				((IModelMember)node.ModelNode).IsCalculated = true;
			}
		}
		private TreeListNode CopyNode(ModelTreeListNode parent, ModelTreeListNode source) {
			if(TrySetModified()) {
				UpdatedTreeListCall(delegate() {
					ModelTreeListNode newNode = Adapter.CloneNode(parent, source, true);
					UpdateNewNode(newNode);
					CurrentModelNode = newNode;
				});
				if(ModelEditorControl != null) {
					return ModelEditorControl.modelTreeList.FocusedNode;
				}
			}
			return null;
		}
		private void modelTreeList_AfterCheckNode(object sender, NodeEventArgs e) {
			UpdateActionState();
		}
		public void NavigateRefProperty(ModelNode sourceNode, string propertyName) {
			ModelTreeListNode node = Adapter.GetPrimaryNode(sourceNode);
			NavigateRefProperty(node, propertyName);
		}
		public void NavigateRefProperty(ModelTreeListNode sourceNode, string propertyName) {
			newNodeForValidation = null;
			if(ValidateNode(CurrentModelNode, true)) {
				string oldFocusedProperty = null;
				if(ModelEditorControl != null) {
					oldFocusedProperty = ModelEditorControl.ModelPropertyGrid.PropertyGrid.FocusedPropertyName;
				}
				CurrentModelNode = sourceNode;
				if(ModelEditorControl != null) {
					if(!string.IsNullOrEmpty(propertyName)) {
						ModelEditorControl.ModelPropertyGrid.PropertyGrid.FocusedPropertyName = propertyName;
					}
					else {
						ModelEditorControl.ModelPropertyGrid.PropertyGrid.FocusedPropertyName = oldFocusedProperty;
					}
				}
			}
		}
		public void CalculateUnusableModel() {
			ModelApplicationBase unusableModelApplication = ModelEditorHelper.GetFullUnusableModel(ModelApplicationCore);
			Dictionary<string, string> unusableModel = GetUnusableNodesForAspects(unusableModelApplication);
			if(unusableModel == null || unusableModel.Count == 0) {
				XtraMessageBox.Show("There are no unusable nodes", "There are no unusable nodes", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			else {
				if(ModelEditorControl != null) {
					UnusableNodesForm messageForm = new UnusableNodesForm(unusableModel);
					messageForm.ShowDialog();
				}
				if(DesignMode) {
					TrySetModified();
				}
			}
		}
		private Dictionary<string, string> GetUnusableNodesForAspects(ModelApplicationBase unusableModel) {
			Dictionary<string, string> result = new Dictionary<string, string>();
			if(unusableModel != null) {
				for(int i = 0; i < ModelApplicationCore.AspectCount; ++i) {
					string model = new ModelXmlWriter().WriteToString((IModelNode)unusableModel, i);
					if(!string.IsNullOrEmpty(model)) {
						result.Add(ModelApplicationCore.GetAspect(i), model);
					}
				}
			}
			return result;
		}
		#region Actions
		private void CreateActions() {
			openAction = new SimpleAction();
			openAction.Caption = "&Open";
			openAction.Category = PredefinedCategory.Edit.ToString();
			openAction.Id = "Open";
			openAction.ImageName = "MenuBar_Open";
			openAction.ToolTip = "Open";
			openAction.Shortcut = "Control+O";
			openAction.Execute += new SimpleActionExecuteEventHandler(openAction_Execute);
			mainBarActions.Add(openAction, "Save");
			saveAction = new SimpleAction();
			saveAction.Caption = "&Save";
			saveAction.Category = PredefinedCategory.Edit.ToString();
			saveAction.Id = "Save";
			saveAction.ImageName = "MenuBar_Save";
			saveAction.ToolTip = "Save the changes";
			saveAction.Shortcut = "Control+S";
			saveAction.Execute += new SimpleActionExecuteEventHandler(saveAction_Execute);
			mainBarActions.Add(saveAction, "Save");
			reloadAction = new SimpleAction();
			reloadAction.Caption = "&Reload";
			reloadAction.ToolTip = reloadAction.Caption;
			reloadAction.Shortcut = "Control+R";
			registeredShortcuts.Add(Keys.Control | Keys.R, reloadAction);
			reloadAction.ImageName = "Action_Reload";
			reloadAction.Execute += new SimpleActionExecuteEventHandler(reloadAction_Execute);
			mainBarActions.Add(reloadAction, "Save");
			addNodeAction = new SingleChoiceAction();
			addNodeAction.Caption = "&Add...";
			addNodeAction.Category = PredefinedCategory.Edit.ToString();
			addNodeAction.Id = "Add";
			addNodeAction.ImageName = "MenuBar_New";
			addNodeAction.Execute += new SingleChoiceActionExecuteEventHandler(addNodeAction_Execute);
			addNodeAction.ItemType = SingleChoiceActionItemType.ItemIsOperation;
			addNodeAction.EmptyItemsBehavior = EmptyItemsBehavior.Disable;
			popupMenuActions.Add(addNodeAction);
			groupNodesAction = new SimpleAction();
			groupNodesAction.Caption = "&Group";
			groupNodesAction.Id = "groupNodesAction";
			groupNodesAction.Shortcut = "Control+G";
			registeredShortcuts.Add(Keys.Control | Keys.G, groupNodesAction);
			groupNodesAction.ImageName = "ModelEditor_Group";
			groupNodesAction.Execute += new SimpleActionExecuteEventHandler(groupNodesAction_Execute);
			popupMenuActions.Add(groupNodesAction);
			goSourceNodeAction = new SimpleAction();
			goSourceNodeAction.Caption = "Go to &Source";
			goSourceNodeAction.Id = "goSourceNodeAction";
			goSourceNodeAction.Shortcut = "Control+Enter";
			registeredShortcuts.Add(Keys.Control | Keys.Enter, goSourceNodeAction);
			goSourceNodeAction.ImageName = "ModelEditor_GoToObject";
			goSourceNodeAction.Execute += new SimpleActionExecuteEventHandler(goSourceNodeAction_Execute);
			popupMenuActions.Add(goSourceNodeAction);
			deleteAction = new SimpleAction();
			deleteAction.Caption = "&Delete";
			deleteAction.Category = PredefinedCategory.Edit.ToString();
			deleteAction.Id = "Delete";
			deleteAction.Shortcut = "Control+D";
			registeredShortcuts.Add(Keys.Control | Keys.D, deleteAction);
			deleteAction.ImageName = "MenuBar_Delete";
			deleteAction.Execute += new SimpleActionExecuteEventHandler(deleteAction_Execute);
			popupMenuActions.Add(deleteAction);
			changeAspectAction = new SingleChoiceAction();
			changeAspectAction.Caption = "Language";
			changeAspectAction.ImageName = "BO_Localization";
			changeAspectAction.PaintStyle = ActionItemPaintStyle.Caption;
			changeAspectAction.Category = PredefinedCategory.Edit.ToString();
			changeAspectAction.ToolTip = "Choose language from list of predefined";
			changeAspectAction.Id = "Language";
			changeAspectAction.Execute += new SingleChoiceActionExecuteEventHandler(changeAspectAction_Execute);
			mainBarActions.Add(changeAspectAction, "Filters");
			navigationBackAction = new SimpleAction();
			navigationBackAction.Caption = "&Back";
			navigationBackAction.ToolTip = "Go back";
			navigationBackAction.ImageName = "Action_Navigation_History_Back";
			navigationBackAction.Shortcut = "Alt+Left";
			navigationBackAction.Execute += new SimpleActionExecuteEventHandler(navigationBack_Execute);
			mainBarActions.Add(navigationBackAction, "RecordsNavigation");
			navigationForwardAction = new SimpleAction();
			navigationForwardAction.Caption = "&Forward";
			navigationForwardAction.ImageName = "Action_Navigation_History_Forward";
			navigationForwardAction.ToolTip = "Go forward";
			navigationForwardAction.Shortcut = "Alt+Right";
			navigationForwardAction.Execute += new SimpleActionExecuteEventHandler(navigationForward_Execute);
			mainBarActions.Add(navigationForwardAction, "RecordsNavigation");
			showDifferences = new SimpleAction();
			showDifferences.Caption = "Show Differences (Current Aspect)";
			showDifferences.ImageName = "ModelEditor_Action_ShowDifferences_Xml";
			showDifferences.Execute += new SimpleActionExecuteEventHandler(showDifferences_Execute);
			popupMenuActions.Add(showDifferences);
			resetDifferencesAction = new SimpleAction();
			resetDifferencesAction.Caption = "Reset Differences";
			resetDifferencesAction.ImageName = "ModelEditor_Action_ResetDifferences_Xml";
			resetDifferencesAction.Execute += new SimpleActionExecuteEventHandler(resetDifferences_Execute);
			popupMenuActions.Add(resetDifferencesAction);
			chooseMergeModuleAction = new SingleChoiceAction();
			chooseMergeModuleAction.Id = "ChooseMergeModule";
			chooseMergeModuleAction.Caption = string.Empty;
			chooseMergeModuleAction.ItemType = SingleChoiceActionItemType.ItemIsMode;
			chooseMergeModuleAction.DefaultItemMode = DefaultItemMode.LastExecutedItem;
			chooseMergeModuleAction.EmptyItemsBehavior = EmptyItemsBehavior.Disable;
			chooseMergeModuleAction.Execute += new SingleChoiceActionExecuteEventHandler(mergeDifferencesAction_Execute);
			mergeDifferencesAction = new SimpleAction();
			mergeDifferencesAction.Id = "MergeDifferences";
			mergeDifferencesAction.Caption = "Merge Differences";
			mergeDifferencesAction.ToolTip = "Merge Differences";
			mergeDifferencesAction.ImageName = "ModelEditor_ModelMerge";
			mergeDifferencesAction.Execute += new SimpleActionExecuteEventHandler(mergeDifferencesAction_Execute);
			popupMenuActions.Add(mergeDifferencesAction);
			mainBarActions.Add(mergeDifferencesAction, "Save");
			copyAction = new SimpleAction();
			copyAction.ImageName = "Action_Copy";
			copyAction.Caption = "Copy";
			copyAction.Shortcut = "Control+C";
			registeredShortcuts.Add(Keys.Control | Keys.C, copyAction);
			copyAction.Execute += new SimpleActionExecuteEventHandler(copyAction_Execute);
			popupMenuActions.Add(copyAction);
			pasteAction = new SimpleAction();
			pasteAction.ImageName = "Action_Paste";
			pasteAction.Caption = "Paste";
			pasteAction.Shortcut = "Control+V";
			registeredShortcuts.Add(Keys.Control | Keys.V, pasteAction);
			pasteAction.Execute += new SimpleActionExecuteEventHandler(pasteAction_Execute);
			popupMenuActions.Add(pasteAction);
			cloneAction = new SimpleAction();
			cloneAction.Caption = "Clone";
			cloneAction.Shortcut = "Control+Alt+C";
			registeredShortcuts.Add(Keys.Control | Keys.Alt | Keys.C, cloneAction);
			cloneAction.Execute += new SimpleActionExecuteEventHandler(cloneAction_Execute);
			cloneAction.ImageName = "Action_CloneMerge_Clone_Object";
			popupMenuActions.Add(cloneAction);
			generateContentAction = new SimpleAction();
			generateContentAction.Caption = "Generate Content";
			generateContentAction.Execute += new SimpleActionExecuteEventHandler(generateContent_Execute);
			generateContentAction.ImageName = "ModelEditor_GenerateContent";
			popupMenuActions.Add(generateContentAction);
			loadedModulesAction = new SimpleAction();
			loadedModulesAction.Caption = "Loaded Modules";
			loadedModulesAction.ToolTip = loadedModulesAction.Caption;
			loadedModulesAction.ImageName = "ModelEditor_Action_Modules";
			loadedModulesAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
			loadedModulesAction.Execute += new SimpleActionExecuteEventHandler(loadedModulesAction_Execute);
			mainBarActions.Add(loadedModulesAction, "Search");
			findAction = new SimpleAction();
			findAction.Caption = "Search";
			findAction.ImageName = "Action_Search";
			findAction.Shortcut = "Control+Alt+F";
			findAction.ToolTip = "Open search";
			findAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
			findAction.Execute += new SimpleActionExecuteEventHandler(find_Execute);
			mainBarActions.Add(findAction, "Search");
			showUnusableNodesAction = new SimpleAction();
			showUnusableNodesAction.Caption = "Show Unusable Data";
			showUnusableNodesAction.ToolTip = showUnusableNodesAction.Caption;
			showUnusableNodesAction.ImageName = "ModelEditor_ShowUnusableNodes";
			showUnusableNodesAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
			showUnusableNodesAction.Execute += new SimpleActionExecuteEventHandler(checkUnusableNodes_Execute);
			mainBarActions.Add(showUnusableNodesAction, "Search");
			nodeUpAction = new SimpleAction();
			nodeUpAction.Caption = "Up";
			nodeUpAction.ImageName = "ModelEditor_IndexUp";
			nodeUpAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
			nodeUpAction.Shortcut = "Alt+Up";
			nodeUpAction.Execute += new SimpleActionExecuteEventHandler(nodeUp_Execute);
			popupMenuActions.Add(nodeUpAction);
			nodeDownAction = new SimpleAction();
			nodeDownAction.Caption = "Down";
			nodeDownAction.ImageName = "ModelEditor_IndexDown";
			nodeDownAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
			nodeDownAction.Shortcut = "Alt+Down";
			nodeDownAction.Execute += new SimpleActionExecuteEventHandler(nodeDown_Execute);
			popupMenuActions.Add(nodeDownAction);
			showLocalizationForm = new SimpleAction();
			showLocalizationForm.Caption = "&Localization";
			showLocalizationForm.ToolTip = "Show Localization window";
			showLocalizationForm.ImageName = "BO_Localization";
			showLocalizationForm.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
			showLocalizationForm.Shortcut = "Control+L";
			showLocalizationForm.Execute += new SimpleActionExecuteEventHandler(showLocalizationForm_Execute);
			mainBarActions.Add(showLocalizationForm, "Filters");
			UpdateActionState();
			DevExpress.XtraBars.BarManager.AllowFocusPopupForm = false;
		}
		private IActionContainer FindActionContainer(string actionContainerId, IEnumerable<IActionContainer> actionContainers) {
			foreach(IActionContainer actionContainer in actionContainers) {
				if(actionContainer.ContainerId == actionContainerId) {
					return actionContainer;
				}
			}
			return null;
		}
		public SimpleAction OpenAction {
			get { return openAction; }
		}
		public SimpleAction SaveAction {
			get { return saveAction; }
		}
		public SingleChoiceAction ChangeAspectAction {
			get { return changeAspectAction; }
		}
		public SingleChoiceAction AddNodeAction {
			get { return addNodeAction; }
		}
		public SimpleAction DeleteAction {
			get { return deleteAction; }
		}
		public SimpleAction CopyAction {
			get { return copyAction; }
		}
		public SimpleAction PasteAction {
			get { return pasteAction; }
		}
		public SimpleAction CloneAction {
			get { return cloneAction; }
		}
		public SimpleAction ResetDifferencesAction {
			get { return resetDifferencesAction; }
		}
		public SingleChoiceAction ChooseMergeModuleAction {
			get { return chooseMergeModuleAction; }
		}
		public SimpleAction MergeDifferencesAction {
			get { return mergeDifferencesAction; }
		}
		public SimpleAction NavigationBackAction {
			get { return navigationBackAction; }
		}
		public SimpleAction GenerateContentAction {
			get { return generateContentAction; }
		}
		public SimpleAction ShowUnusableNodesAction {
			get { return showUnusableNodesAction; }
		}
		public SimpleAction NodeUpAction {
			get { return nodeUpAction; }
		}
		public SimpleAction NodeDownAction {
			get { return nodeDownAction; }
		}
		public SimpleAction ReloadAction {
			get { return reloadAction; }
		}
		public SimpleAction GoSourceNodeAction {
			get {
				return goSourceNodeAction;
			}
		}
		public SimpleAction GroupNodesAction {
			get { return groupNodesAction; }
		}
		#endregion
		#region Validation
		private bool ValidateNode(ModelTreeListNode node, bool showMessage) {
			bool result = true;
			if(node != null) {
				RuleSetValidationResult validationResult = ModelValidator.ValidateNode(GetModelNode(node));
				result = validationResult == null || validationResult.State != ValidationState.Invalid;
				if(ModelEditorControl != null) {
					result &= ModelEditorControl.ModelPropertyGrid.PropertyGrid.EditorErrorState;
				}
				if(validationResult != null || node != null) { 
					if(!result && SelectedNodes.Count > 1) {
						showMessage = true;
						List<TreeListNode> selectNodes = new List<TreeListNode>();
						if(ModelEditorControl != null) {
							selectNodes.Add(ModelEditorControl.modelTreeList.FocusedNode);
						}
						SelectNodes(selectNodes);
					}
					showMessage = showMessage ? lockErrorMessages == 0 : showMessage;
					SetErrorMessages(validationResult, node, showMessage);
				}
				if(result) {
					ErrorMessages = null;
				}
			}
			return result;
		}
		private ErrorMessages ErrorMessages {
			get {
				return errorMessages;
			}
			set {
				errorMessages = value;
			}
		}
		private ModelValidator ModelValidator {
			get {
				if(_validator == null) {
					_validator = new ModelValidator(Adapter.fastModelEditorHelper);
				}
				return _validator;
			}
		}
		private void ModelEditorControl_Closed(object sender, EventArgs e) {
			lockErrorMessages++;
			newNodeForValidation = null;
		}
		private void modelTreeList_BeforeFocusNode(object sender, BeforeFocusNodeEventArgs e) {
			SafeExecute(delegate() {
				if(e.OldNode != null && lockErrorMessages == 0) {
					if(ModelEditorHelper.IsNodeEqual(((ModelTreeListNode)((ObjectTreeListNode)e.OldNode).Object).ModelNode, GetModelNode(newNodeForValidation))) {
						newNodeForValidation = null;
					}
					e.CanFocus = ValidateNode(GetModelTreeListNode(e.OldNode), true);
				}
			});
		}
		private void modelTreeList_BeforeCollapse(object sender, BeforeCollapseEventArgs e) {
			SafeExecute(delegate() {
				newNodeForValidation = null;
				e.CanCollapse = ValidateNode(CurrentModelNode, true);
			});
		}
		private void modelTreeList_BeforeExpand(object sender, BeforeExpandEventArgs e) {
			SafeExecute(delegate() {
				if(validationContext != "addNodeAction") {
					newNodeForValidation = null;
				}
				e.CanExpand = ValidateNode(CurrentModelNode, true);
			});
		}
		private bool LastValidationResult {
			get {
				bool result = !propertyChanging && (ErrorMessages == null || ErrorMessages.IsEmpty);
				if(ModelEditorControl != null) {
					result &= this.ModelEditorControl.ModelPropertyGrid.PropertyGrid.EditorErrorState;
				}
				return result;
			}
		}
		private void SetErrorMessages(RuleSetValidationResult result, ModelTreeListNode targetNode, bool showMessage) {
			ErrorMessages errorMessages = new ErrorMessages();
			if(result != null) {
				List<RuleSetValidationResultItem> resultItems = new List<RuleSetValidationResultItem>();
				foreach(RuleSetValidationResultItem resultItem in result.Results) {
					resultItems.Clear();
					if(resultItem is RuleSetValidationResultItemAggregate) {
						errorMessages.AddMessage(((IRuleCollectionPropertyProperties)resultItem.Rule.Properties).TargetCollectionPropertyName, resultItem.Target, resultItem.ErrorMessage);
						resultItems.AddRange(((RuleSetValidationResultItemAggregate)resultItem).AggregatedResults);
					}
					else {
						resultItems.Add(resultItem);
					}
					foreach(RuleSetValidationResultItem innerResultItem in resultItems) {
						if(innerResultItem.State == ValidationState.Invalid) {
							foreach(string propertyName in innerResultItem.Rule.UsedProperties) {
								errorMessages.AddMessage(propertyName, innerResultItem.Target, innerResultItem.ErrorMessage);
							}
						}
					}
				}
			}
			this.errorMessages = errorMessages;
			if(ModelEditorControl != null) {
				if(!ModelEditorHelper.IsNodeEqual(GetModelNode(newNodeForValidation), GetModelNode(targetNode))) {
					ModelEditorControl.ModelPropertyGrid.PropertyGrid.SetErrorMessages(errorMessages);
				}
				if(showMessage && !errorMessages.IsEmpty) {
					LabelControl control = new LabelControl();
					control.ImageAlignToText = ImageAlignToText.LeftCenter;
					control.Appearance.Options.UseTextOptions = true;
					control.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
					control.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
					control.Appearance.Image = ImageLoader.Instance.GetImageInfo("State_Validation_Invalid_48x48").Image;
					control.Width = 400;
					control.AutoSize = true;
					control.AutoSizeMode = LabelAutoSizeMode.Vertical;
					control.Text = result.GetFormattedErrorMessage();
					LayoutControl layoutControl = new LayoutControl();
					layoutControl.AllowCustomization = false;
					LayoutControlItem layoutControlItem = new LayoutControlItem(layoutControl, control);
					layoutControlItem.TextVisible = false;
					using(PopupForm errorForm = (PopupForm)PopupInfo.CreateForm(layoutControl, "Validation Result", ImageLoader.Instance.GetImageInfo("BO_Validation").Image)) {
						errorForm.InitialMinimumSize = new Size(420, 100);
						errorForm.MaximizeBox = false;
						errorForm.MinimizeBox = false;
						errorForm.AutoShrink = true;
						errorForm.IsSizeable = false;
						errorForm.ShowDialog();
					}
				}
			}
		}
		#endregion
		#region IModelEditorController Members
		public bool ShowCulturesManager(string newValue) {
			if(newValue == LanguagesManagerLaunchItemName) {
				using(AspectManagementForm managerForm = new AspectManagementForm()) {
					managerForm.FormBorderStyle = FormBorderStyle.FixedDialog;
					managerForm.ReadOnly = ReadOnly;
					managerForm.SetAvailableAspects(CaptionHelper.DefaultLanguage, AspectNames);
					if(managerForm.ShowDialog() == DialogResult.OK) {
						List<string> aspects = managerForm.Aspects;
						aspects.Remove(CaptionHelper.DefaultLanguage);
						foreach(string newAspect in aspects) {
							ModelApplicationCore.AddAspect(newAspect);
						}
						if(AspectNames.Count != aspects.Count) {
							string additionLanguageMessage = string.Format(additionLanguageMessageFormat, DesignMode ? "Visual Studio" : "Application");
							XtraMessageBox.Show(additionLanguageMessage, managerForm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
							IsModified = true;
						}
					}
				}
				UpdateActionState();
				return true;
			}
			return false;
		}
		public void RefreshCurrentAttributeValue() {
			throw new NotImplementedException();
		}
		public void SetCurrentAspectByName(string aspectName) {
			ModelApplicationCore.CurrentAspectProvider.CurrentAspect = aspectName;
			if(CurrentAspectChanged != null) {
				CurrentAspectChanged(this, EventArgs.Empty);
			}
			if(ModelEditorControl != null) {
				ModelEditorControl.ModelPropertyGrid.PropertyGrid.SelectedObject = null;
				ModelEditorControl.ModelPropertyGrid.PropertyGrid.SelectedObject = CurrentModelNode;
				ModelEditorControl.modelTreeList.RefreshNodesDisplayValue(ModelEditorControl.RootModelTreeListNode);
			}
		}
		public void Back() {
			ExecuteAction(navigationBackAction);
		}
		public void Forward() {
			ExecuteAction(navigationForwardAction);
		}
		public bool Save() {
			newNodeForValidation = null;
			if(ModelEditorControl != null) {
				BindingHelper.EndCurrentEdit(ModelEditorControl.ModelPropertyGrid);
			}
			bool result = ValidateNode(CurrentModelNode, true);
			if(result) {
				if(MergeDiffsMode || (saveAction.Active && saveAction.Enabled)) {
					InternalSave(); 
				}
			}
			return result;
		}
		public List<string> AspectNames {
			get {
				return ModelEditorHelper.GetAspectNames(ModelApplicationCore);
			}
		}
		public List<string> AllAspectNames {
			get {
				List<string> result = new List<string>(AspectNames);
				result.Add(LanguagesManagerLaunchItemName);
				return result;
			}
		}
		public string CurrentAspect {
			get {
				string result = ModelApplicationCore.CurrentAspect;
				return string.IsNullOrEmpty(result) ? CaptionHelper.DefaultLanguage : result;
			}
		}
		public override ModelTreeListNode CurrentModelNode {
			set {
				base.CurrentModelNode = value;
				if(value != null) {
					if(ModelEditorControl != null) {
						ModelEditorControl.ShowLayoutIfNeed(GetModelNode(ModelEditorControl.CurrentModelTreeListNode), ReadOnly);
					}
					AddStartNavigationPoint();
					AddNavigationPoint();
				}
				ValidateNode(value, false);
				SetDescription(GetModelTreeListNodeDescription(CurrentModelNode, null));
				UpdateActionState();
#if DebugTest
				Application.DoEvents();
#endif
			}
		}
		public bool CanBack {
			get { return navigationBackAction.Enabled; }
		}
		public bool CanForward {
			get { return navigationForwardAction.Enabled; }
		}
		public bool CanShowLocalizationForm {
			get { return showLocalizationForm.Enabled; }
		}
		public bool CanChangeAspect {
			get { return canChangeAspect; }
			private set {
				canChangeAspect = value;
			}
		}
		public IList<string> LastSavedFiles {
			get {
				List<string> result = new List<string>();
				if(diffstore is FileModelStore) {
					result.AddRange(((FileModelStore)diffstore).LastSavedFiles);
				}
				foreach(ModelDifferenceStore modelDifferenceStore in modelDiffsStoresForSave.Values) {
					if(modelDifferenceStore is FileModelStore) {
						result.AddRange(((FileModelStore)modelDifferenceStore).LastSavedFiles);
					}
				}
				return result;
			}
		}
		public bool FindAndFocusEntry(bool nextEntry, string text, SearchNodeOptions searchNodeOptions) {
			return false;
		}
		public void SaveSettings() {
			SaveSelectedLanguage();
			ModelEditorControl.SaveSettings();
			if(ModelEditorControl != null) {
				Adapter.SaveSettings(ModelEditorControl.SettingsStorage);
			}
		}
		public void LoadSettings() {
			LoadSelectedLanguage();
			if(ModelEditorControl != null) {
				Adapter.LoadSettings(ModelEditorControl.SettingsStorage);
			}
			ModelEditorControl.LoadSettings();
		}
		public void SetModuleDiffStore(List<ModuleDiffStoreInfo> modulesDiffStoreInfo) {
			modulesDiffStoreInfoToDispose = modulesDiffStoreInfo;
			chooseMergeModuleAction.Items.Clear();
			foreach(ModuleDiffStoreInfo moduleDiffStoreInfo in modulesDiffStoreInfo) {
				ChoiceActionItem choiceActionItem = new ChoiceActionItem(moduleDiffStoreInfo.ModuleName, moduleDiffStoreInfo);
				chooseMergeModuleAction.Items.Add(choiceActionItem);
				if(currentMergeModuleInfo == null) {
					chooseMergeModuleAction.DoExecute(choiceActionItem);
				}
			}
			UpdateActionState();
		}
		#endregion
		#region IDisposable Members
		public override void Dispose() {
			if(ModelEditorControl != null) {
				ModelEditorControl.HandleCreated -= new EventHandler(ModelEditorControl_HandleCreated);
				ModelEditorControl.ModelPropertyGrid.PropertyGrid.SelectedObject = null;
				ModelEditorControl.ModelPropertyGrid.PropertyGrid.SelectedObject = CurrentModelNode;
			}
			if(mergeDiffsMode) {
				ExtendModelInterfaceAdapter.LinksEnabled = true;
			}
			base.Dispose();
			DisposeActions();
			ClearFilds();
			DisposeHandlers();
			if(_modelAttributesPropertyEditorController != null) {
				_modelAttributesPropertyEditorController.Dispose();
				_modelAttributesPropertyEditorController = null;
			}
			linksNodeHelper = null;
		}
		private void ClearFilds() {
			if(diffstore != null) {
				ModelEditorHelper.Dispose(diffstore);   
				diffstore = null;
			}
			if(modulesDiffStoreInfoToDispose != null) {
				foreach(ModuleDiffStoreInfo item in modulesDiffStoreInfoToDispose) {
					item.Dispose();
				}
				modulesDiffStoreInfoToDispose.Clear();
				modulesDiffStoreInfoToDispose = null;
			}
			_validator = null;
			if(navigationPoints != null) {
				navigationPoints.Clear();
				navigationPoints = null;
			}
			if(nodeBuffers != null) {
				nodeBuffers.Clear();
				nodeBuffers = null;
			}
			if(modelDiffsStoresForSave != null) {
				modelDiffsStoresForSave.Clear();
				modelDiffsStoresForSave = null;
			}
			if(mergeInfoMessage != null) {
				mergeInfoMessage = null;
			}
			if(currentMergeModuleInfo != null) {
				currentMergeModuleInfo.Dispose();
				currentMergeModuleInfo = null;
			}
		}
		private void DisposeActions() {
			if(openAction != null) {
				openAction.Dispose();
				openAction = null;
			}
			if(saveAction != null) {
				saveAction.Dispose();
				saveAction = null;
			}
			if(reloadAction != null) {
				reloadAction.Dispose();
				reloadAction = null;
			}
			if(addNodeAction != null) {
				addNodeAction.Dispose();
				addNodeAction = null;
			}
			if(groupNodesAction != null) {
				groupNodesAction.Dispose();
				groupNodesAction = null;
			}
			if(goSourceNodeAction != null) {
				goSourceNodeAction.Dispose();
				goSourceNodeAction = null;
			}
			if(deleteAction != null) {
				deleteAction.Dispose();
				deleteAction = null;
			}
			if(changeAspectAction != null) {
				changeAspectAction.Dispose();
				changeAspectAction = null;
			}
			if(navigationBackAction != null) {
				navigationBackAction.Dispose();
				navigationBackAction = null;
			}
			if(navigationForwardAction != null) {
				navigationForwardAction.Dispose();
				navigationForwardAction = null;
			}
			if(showDifferences != null) {
				showDifferences.Dispose();
				showDifferences = null;
			}
			if(resetDifferencesAction != null) {
				resetDifferencesAction.Dispose();
				resetDifferencesAction = null;
			}
			if(chooseMergeModuleAction != null) {
				chooseMergeModuleAction.Dispose();
				chooseMergeModuleAction = null;
			}
			if(mergeDifferencesAction != null) {
				mergeDifferencesAction.Dispose();
				mergeDifferencesAction = null;
			}
			if(copyAction != null) {
				copyAction.Dispose();
				copyAction = null;
			}
			if(pasteAction != null) {
				pasteAction.Dispose();
				pasteAction = null;
			}
			if(cloneAction != null) {
				cloneAction.Dispose();
				cloneAction = null;
			}
			if(generateContentAction != null) {
				generateContentAction.Dispose();
				generateContentAction = null;
			}
			if(loadedModulesAction != null) {
				loadedModulesAction.Dispose();
				loadedModulesAction = null;
			}
			if(findAction != null) {
				findAction.Dispose();
				findAction = null;
			}
			if(showUnusableNodesAction != null) {
				showUnusableNodesAction.Dispose();
				showUnusableNodesAction = null;
			}
			if(nodeUpAction != null) {
				nodeUpAction.Dispose();
				nodeUpAction = null;
			}
			if(nodeDownAction != null) {
				nodeDownAction.Dispose();
				nodeDownAction = null;
			}
			if(showLocalizationForm != null) {
				showLocalizationForm.Dispose();
				showLocalizationForm = null;
			}
			if(mainBarActions != null) {
				mainBarActions.Clear();
				mainBarActions = null;
			}
			if(popupMenuActions != null) {
				popupMenuActions.Clear();
				popupMenuActions = null;
			}
			if(registeredShortcuts != null) {
				registeredShortcuts.Clear();
				registeredShortcuts = null;
			}
		}
		private void DisposeHandlers() {
			lostFocusEventHandler = null;
			navigateToRefPropertyEventHandler = null;
			propertyChangingEventHandler = null;
			propertyChangedEventHandler = null;
			gotFocusEventHandler = null;
			modelPropertyGridGotFocusEventHandler = null;
			nodeIndexChangedEventHandler = null;
			closedEventHandler = null;
			focusedNodeChangedEventHandler = null;
		}
		#endregion
		#region KeyProcessor Members
		public Dictionary<Keys, SimpleAction> registeredShortcuts = new Dictionary<Keys, SimpleAction>();
		bool controlPressed = false;
		bool controlAltPressed = false;
		private const uint WM_KEYUP = 0x0101;
		private const uint WM_KEYDOWN = 0x0100;
		private const uint V_CONTROL = 0x11;
		private const uint V_CONTROL_Alt = 0x12;
		public bool PreProcessMessage(Message m) {
			bool result = false;
			if(m.Msg == WM_KEYDOWN) {
				if((int)m.WParam == V_CONTROL) {
					controlPressed = true;
				}
				else {
					if((int)m.WParam == V_CONTROL_Alt) {
						controlAltPressed = true;
					}
				}
			}
			else if(m.Msg == WM_KEYUP) {
				if((int)m.WParam == V_CONTROL || (int)m.WParam == V_CONTROL_Alt) {
					controlPressed = false;
					controlAltPressed = false;
				}
			}
			if(m.Msg == WM_KEYDOWN && ModelEditorControl.modelTreeList.Focused) {
				result = KeyProcessorCore(m);
			}
			return result;
		}
		private bool KeyProcessorCore(Message m) {
			foreach(KeyValuePair<Keys, SimpleAction> item in registeredShortcuts) {
				if((item.Key & Keys.Control) == Keys.Control && controlPressed) {
					Keys key = (Keys)(int)m.WParam & Keys.KeyCode;
					if((item.Key & key) != Keys.None) {
						SimpleAction action = null;
						Keys mKey = controlAltPressed ? Keys.Control | Keys.Alt : Keys.Control;
						if(registeredShortcuts.TryGetValue(mKey | key, out action)) {
							SafeExecute(delegate() {
								ExecuteAction(action);
							});
							return true;
						}
					}
				}
			}
			return false;
		}
		#endregion
		#region ActionsExecute
		private void ExecuteAction(SimpleAction action) {
			try {
				lockUpdateActionState = true;
				if(action.Active && action.Enabled) {
					action.DoExecute();
				}
			}
			finally {
				lockUpdateActionState = false;
				UpdateActionState();
			}
		}
		private void openAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			try {
				lockUpdateActionState = true;
				OnCustomLoadModel();
			}
			finally {
				lockUpdateActionState = false;
				UpdateActionState();
			}
		}
		private void saveAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			try {
				lockUpdateActionState = true;
				bool result = ValidateNode(CurrentModelNode, true);
				if(result) {
					InternalSave();
				}
			}
			finally {
				lockUpdateActionState = false;
				UpdateActionState();
			}
		}
		private void deleteAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			if(SelectedNodes.Count > 0) {
				if(ConfirmDeletion()) {
					try {
						lockUpdateActionState = true;
						if(TrySetModified()) {
							UpdatedTreeListCall(delegate() {
								try {
									lockErrorMessages++;
									foreach(ModelTreeListNode targetNode in SelectedNodes) {
										navigationPoints.DeleteCurrentItem();
										Adapter.DeleteNode(targetNode);
									}
									ModelTreeListNode lockCurrentModelNode = CurrentModelNode;
									if(lockCurrentModelNode != null && lockCurrentModelNode.ModelNode == null) {
										CurrentModelNode = null;
									}
								}
								finally {
									lockErrorMessages--;
								}
							});
						}
					}
					finally {
						lockUpdateActionState = false;
						UpdateActionState();
					}
				}
			}
		}
		private void loadedModulesAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			if(ModelEditorControl != null) {
				ModelEditorControl.ShowLoadedModulesForm(((IModelSources)ModelApplicationCore).Modules, e.Action.ImageName);
			}
		}
		private void resetDifferences_Execute(object sender, SimpleActionExecuteEventArgs e) {
			if(ConfirmResetDifferences()) {
				try {
					lockUpdateActionState = true;
					if(TrySetModified()) {
						UpdatedTreeListCall(delegate() {
							ModelTreeListNode lockCurrentModelNode = CurrentModelNode;
							string targetNodePath = ModelEditorHelper.GetModelNodePath(lockCurrentModelNode.ModelNode);
							string ownerNodePath = ModelEditorHelper.GetModelNodePath(lockCurrentModelNode.Owner.ModelNode);
							foreach(ModelTreeListNode targetNode in SelectedNodes) {
								targetNode.ModelNode.Undo();
							}
							ModelTreeListNode nodeToSelect = null;
							bool isLinkNode = false;
							try {
								Adapter.BeginUpdate();
								foreach(ModelTreeListNode targetNode in SelectedNodes) {
									if(targetNode.IsRootVirtualTreeNode) {
										targetNode.ResetVirtualTree(false);
									}
									isLinkNode = targetNode.ModelTreeListNodeType != ModelTreeListNodeType.Primary || targetNode.ModelTreeListNodeType != ModelTreeListNodeType.Group;
									nodeToSelect = targetNode.Owner;
									targetNode.ClearChilds(true);
								}
							}
							finally {
								Adapter.EndUpdate();
							}
							if(isLinkNode) {
								nodeToSelect.ClearChilds(true);
							}
							CurrentModelNode = nodeToSelect;
							if(!isLinkNode) {
								FocusModelNodeIfNeed(targetNodePath, ownerNodePath);
							}
							RefreshCurrentModelNode();
							NodeIndexChanged();
						});
					}
				}
				finally {
					lockUpdateActionState = false;
					UpdateActionState();
				}
			}
		}
		private void pasteAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			try {
				lockUpdateActionState = true;
				UpdatedTreeListCall(delegate() {
					ModelTreeListNode parent = CurrentModelNode;
					List<TreeListNode> nodesForSelect = new List<TreeListNode>();
					foreach(ModelTreeListNode nodeBuffer in nodeBuffers) {
						nodesForSelect.Add(CopyNode(parent, nodeBuffer));
					}
					SelectNodes(nodesForSelect);
				});
			}
			finally {
				lockUpdateActionState = false;
				UpdateActionState();
			}
		}
		private void cloneAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			try {
				lockUpdateActionState = true;
				UpdatedTreeListCall(delegate() {
					List<TreeListNode> nodesForSelect = new List<TreeListNode>();
					foreach(ModelTreeListNode node in SelectedNodes) {
						nodesForSelect.Add(CopyNode(node.Parent, node));
					}
					SelectNodes(nodesForSelect);
				});
			}
			finally {
				lockUpdateActionState = false;
				UpdateActionState();
			}
		}
		private void navigationBack_Execute(object sender, SimpleActionExecuteEventArgs e) {
			CurrentModelNode = navigationPoints.Back().Item;
		}
		private void navigationForward_Execute(object sender, SimpleActionExecuteEventArgs e) {
			CurrentModelNode = navigationPoints.Forward().Item;
		}
		private void changeAspectAction_Execute(object sender, SingleChoiceActionExecuteEventArgs e) {
			if(!ShowCulturesManager(e.SelectedChoiceActionItem.Caption)) {
				SetCurrentAspectByName(e.SelectedChoiceActionItem.Caption);
			}
		}
		private void generateContent_Execute(object sender, SimpleActionExecuteEventArgs e) {
			if(TrySetModified()) {
				try {
					lockUpdateActionState = true;
					ModelTreeListNode lockCurrentModelNode = CurrentModelNode;
					string targetNodePath = ModelEditorHelper.GetModelNodePath(lockCurrentModelNode.ModelNode);
					string ownerNodePath = ModelEditorHelper.GetModelNodePath(lockCurrentModelNode.Owner.ModelNode);
					ModelEditorHelper.GenerateContent(GetModelNode(lockCurrentModelNode));
					try {
						Adapter.BeginUpdate();
						lockCurrentModelNode.ClearChilds(true);
					}
					finally {
						Adapter.EndUpdate();
					}
					FocusModelNodeIfNeed(targetNodePath, ownerNodePath);
					RefreshCurrentModelNode();
				}
				finally {
					lockUpdateActionState = false;
					UpdateActionState();
				}
			}
		}
		private void showLocalizationForm_Execute(object sender, SimpleActionExecuteEventArgs e) {
			ShowLocalizationForm();
		}
		private void nodeDown_Execute(object sender, SimpleActionExecuteEventArgs e) {
			if(TrySetModified()) {
				try {
					lockUpdateActionState = true;
					UpdatedTreeListCall(delegate() {
						List<ModelTreeListNode> items = new List<ModelTreeListNode>(SelectedNodes);
						if(items.Count > 0) {
							try {
								if(ModelEditorControl != null && ModelEditorControl.modelTreeList != null) {
									ModelEditorControl.modelTreeList.LockRefreshChildNodes++;
								}
								Adapter.SortNodes(items);
								items.Reverse();
								IEnumerable sourceCollection = Adapter.GetChildren(items[0].Parent);
								foreach(ModelTreeListNode node in items) {
									if(node.ModelNode.Index == -1) {
										continue;
									}
									int nodeIndex = GetNodeIndex_Down(node, items, sourceCollection);
									ChangeNodeIndex(node, nodeIndex);
								}
							}
							finally {
								if(ModelEditorControl != null && ModelEditorControl.modelTreeList != null) {
									ModelEditorControl.modelTreeList.LockRefreshChildNodes--;
									ModelEditorControl.modelTreeList.RefreshChildNodes(items[0].Parent);
								}
							}
						}
					});
				}
				finally {
					NodeIndexChanged();
					lockUpdateActionState = false;
					UpdateActionState();
				}
			}
		}
		private void nodeUp_Execute(object sender, SimpleActionExecuteEventArgs e) {
			if(TrySetModified()) {
				try {
					lockUpdateActionState = true;
					UpdatedTreeListCall(delegate() {
						List<ModelTreeListNode> items = new List<ModelTreeListNode>(SelectedNodes);
						if(items.Count > 0) {
							try {
								if(ModelEditorControl != null && ModelEditorControl.modelTreeList != null) {
									ModelEditorControl.modelTreeList.LockRefreshChildNodes++;
								}
								Adapter.SortNodes(items);
								IEnumerable sourceCollection = Adapter.GetChildren(items[0].Parent);
								foreach(ModelTreeListNode node in items) {
									int nodeIndex = GetNodeIndex_Up(node, items, sourceCollection);
									if(nodeIndex != -1) {
										ChangeNodeIndex(node, nodeIndex);
									}
								}
							}
							finally {
								if(ModelEditorControl != null && ModelEditorControl.modelTreeList != null) {
									ModelEditorControl.modelTreeList.LockRefreshChildNodes--;
									ModelEditorControl.modelTreeList.RefreshChildNodes(items[0].Parent);
								}
							}
						}
					});
				}
				finally {
					NodeIndexChanged();
					lockUpdateActionState = false;
					UpdateActionState();
				}
			}
		}
		private void mergeDifferencesAction_Execute(object sender, SingleChoiceActionExecuteEventArgs e) {
			currentMergeModuleInfo = (ModuleDiffStoreInfo)e.SelectedChoiceActionItem.Data;
		}
		private void mergeDifferencesAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			if(currentMergeModuleInfo == null) {
				return;
			}
			ModelDifferenceStore destinationDifferenceStore = currentMergeModuleInfo.ModuleStore;
			if(ValidateNode(CurrentModelNode, true) && CanMergeDifferences(currentMergeModuleInfo) && (MergeDiffsMode || ConfirmMergeDifferences(currentMergeModuleInfo.ModuleName)) && TrySetModified()) {
				try {
					lockUpdateActionState = true;
					validationContext = "moveDifferencesAction";
					MergeDifferences(destinationDifferenceStore, e.Action.ImageName);
				}
				finally {
					validationContext = "";
					lockUpdateActionState = false;
					UpdateActionState();
				}
			}
		}
		private void goSourceNodeAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			CurrentModelNode = Adapter.FindPrimaryNode(CurrentModelNode.ModelNode);
		}
		private void groupNodesAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			try {
				lockUpdateActionState = true;
				ModelTreeListNode lockCurrentModelNode = CurrentModelNode;
				if(Adapter.IsGroupChilds(lockCurrentModelNode)) {
					Adapter.UnGroup(lockCurrentModelNode);
				}
				else {
					Adapter.Group(lockCurrentModelNode);
				}
			}
			finally {
				lockUpdateActionState = false;
				UpdateActionState();
			}
		}
		private void reloadAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			try {
				lockUpdateActionState = true;
				ReloadModel(true, true);
			}
			finally {
				lockUpdateActionState = false;
				UpdateActionState();
			}
		}
		private void showDifferences_Execute(object sender, SimpleActionExecuteEventArgs e) {
			string differencesText = GetModelDifferencesText();
			if(!string.IsNullOrEmpty(differencesText)) {
				PopupInfo.Show(differencesText, "Node xml", ImageLoader.Instance.GetImageInfo(e.Action.ImageName).Image, true);
			}
		}
		private void find_Execute(object sender, SimpleActionExecuteEventArgs e) {
			ModelEditorControl.ActivateSearchControl();
		}
		private void checkUnusableNodes_Execute(object sender, SimpleActionExecuteEventArgs e) {
			((IModelEditorController)this).CalculateUnusableModel();
		}
		private void copyAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			nodeBuffers.Clear();
			foreach(ModelTreeListNode selectedNode in SelectedNodes) {
				nodeBuffers.Add(selectedNode.PrimaryNode);
			}
			UpdateActionState();
		}
		private ModelTreeListNode GetParentNodeByType(Type nodeType, ModelTreeListNode children) {
			ModelTreeListNode result = null;
			ModelTreeListNode parent = children.Parent;
			if(nodeType.IsAssignableFrom(parent.ModelNode.GetType())) {
				result = parent;
			}
			else {
				while(parent != null && result == null) {
					parent = parent.Parent;
					if(nodeType.IsAssignableFrom(parent.ModelNode.GetType())) {
						result = parent;
					}
				}
			}
			return result;
		}
		private void addNodeAction_Execute(object sender, SingleChoiceActionExecuteEventArgs e) {
			try {
				lockUpdateActionState = true;
				validationContext = "addNodeAction";
				if(TrySetModified()) {
					UpdatedTreeListCall(delegate() {
						if(typeof(Type).IsAssignableFrom(e.SelectedChoiceActionItem.Data.GetType())) {
							ModelTreeListNode newNode = null;
							ModelTreeListNode lockCurrentModelNodeForVirtualTree = CurrentModelNode;
							ModelTreeListNode lockCurrentModelNode = CurrentModelNode;
							ModelVirtualTreeAddItemAttribute[] addAttributes = AttributeHelper.GetAttributesConsideringInterfaces<ModelVirtualTreeAddItemAttribute>(CurrentModelNode.ModelNode.GetType(), true);
							ModelVirtualTreeAddItemAttribute addAttr = null;
							if(addAttributes != null && addAttributes.Length > 0) {
								addAttr = addAttributes[0];
								lockCurrentModelNode = GetParentNodeByType(addAttr.RealParentNode, CurrentModelNode);
							}
							if(lockCurrentModelNode != null) {
								if(e.SelectedChoiceActionItem.ParentItem != null) {
									newNode = Adapter.AddNode(lockCurrentModelNode, ModelEditorHelper.FindNodeByPath((string)e.SelectedChoiceActionItem.ParentItem.Data, lockCurrentModelNode.Root.ModelNode), (Type)e.SelectedChoiceActionItem.Data, null);
									if(newNode.LinksCount == 1) {
										newNode = newNode.GetLink(0);
									}
								}
								else {
									newNode = Adapter.AddNode(lockCurrentModelNode, (Type)e.SelectedChoiceActionItem.Data, null);
								}
								if(addAttr != null) {
									newNode = VirtualTreeDragDropCore(lockCurrentModelNodeForVirtualTree, newNode);
								}
								UpdateNewNode(newNode);
								bool isLinkNode = false;
								if(newNode != null && newNode.ModelNode is IModelColumn && newNode.Parent != null) {
									isLinkNode = newNode.ModelTreeListNodeType != ModelTreeListNodeType.Primary;
									string targetNodePath = ModelEditorHelper.GetModelNodePath(newNode.ModelNode);
									string ownerNodePath = ModelEditorHelper.GetModelNodePath(newNode.Owner.ModelNode);
									IModelColumn newColumn = (IModelColumn)newNode.ModelNode;
									IModelListView listViewModel = newColumn.ParentView as IModelListView;
									if(listViewModel != null) {
										ModelTreeListNode bandsTreeListNode = Adapter.FindPrimaryNode((ModelNode)listViewModel.BandsLayout);
										if(bandsTreeListNode != null) {
											bandsTreeListNode.ClearChilds(true, false, false);
										}
									}
									if(isLinkNode) {
										FocusModelNodeIfNeed(targetNodePath, ownerNodePath);
									}
								}
								if(!isLinkNode) {
									newNodeForValidation = newNode;
									CurrentModelNode = newNode;
								}
								else {
									newNodeForValidation = CurrentModelNode;
								}
							}
						}
					});
				}
			}
			finally {
				validationContext = "";
				lockUpdateActionState = false;
				UpdateActionState();
			}
		}
		#endregion
		#region MergeDifferences
		private void MergeDifferences(ModelDifferenceStore destinationDifferenceStore, string imageName) {
			UpdatedTreeListCall(delegate() {
				ModuleBase module = currentMergeModuleInfo.ModuleType != null ? ModuleFactory.WithEmptyDiffs.CreateModule(currentMergeModuleInfo.ModuleType) : null;
				IModelNodeMoveInfo modeInfo = null;
				if(module != null) {
					ApplicationModulesManager manager = null;
					ModelApplicationBase modelApplication = CreateModelApplicationByModule(module, destinationDifferenceStore, out manager);
					modeInfo = new ModelNodeMoveInfo(modelApplication, manager);
				}
				else {
					modeInfo = new RuntimeModelNodeMoveInfo(); 
				}
				foreach(ModelTreeListNode selectedNode in SelectedNodes) {
					ModelApplicationBase targetLayer = ModelEditorHelper.MoveNodeToOtherLayer(selectedNode.ModelNode, destinationDifferenceStore.Name, modeInfo);
					ModelNode newNode = ModelEditorHelper.FindNodeByPath(ModelEditorHelper.GetModelNodePath(selectedNode.ModelNode), ModelApplicationCore, false, false);
					selectedNode.SetModelNode(newNode, true);
					if(targetLayer != null) {
						modelDiffsStoresForSave[targetLayer] = destinationDifferenceStore;
					}
				}
				if(isShowDifferences) {
					foreach(ModelTreeListNode modelTreeListNode in SelectedNodes) {
						if(modelTreeListNode.ModelNode.HasModification) {
							string message = string.Format("There are differences that cannot be merged to {0}. The differences below will remain in the current XAFML file after saving.", destinationDifferenceStore.Name);
							PopupInfo.Show(message, GetModelDifferencesText(), "Model Merge", ImageLoader.Instance.GetImageInfo(imageName).Image, true);
							break;
						}
					}
				}
			});
		}
		#endregion
		#region Messages
		private bool ConfirmDeletion() {
			return !AskConfirmation || (DialogResult.Yes ==
				XtraMessageBox.Show("Do you want to delete the selected node(s)?", "Model Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning));
		}
		private bool ConfirmResetDifferences() {
			string message = "";
			if(SelectedNodes.Count == 1) {
				message = string.Format("You are about to reset all the model customizations made for the {0} node. Do you want to proceed?", Adapter.fastModelEditorHelper.GetModelNodeDisplayValue(GetModelNode(CurrentModelNode)));
			}
			else {
				message = "You are about to reset all the model customizations made for the selected nodes. Do you want to proceed?";
			}
			return !AskConfirmation || (DialogResult.Yes ==
				XtraMessageBox.Show(message, "Model Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning));
		}
		private bool ConfirmMergeDifferences(string moduleName) {
			string message = "";
			if(SelectedNodes.Count == 1) {
				message = string.Format("You are about to merge the '{0}' node differences to '{1}'.  Do you want to proceed?", Adapter.fastModelEditorHelper.GetModelNodeDisplayValue(GetModelNode(CurrentModelNode)), moduleName);
			}
			else {
				message = string.Format("You are about to merge the selected nodes differences to '{0}'.  Do you want to proceed?", moduleName);
			}
			return !AskConfirmation || (DialogResult.Yes ==
				XtraMessageBox.Show(message, "Model Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning));
		}
		private bool CanMergeDifferences(ModuleDiffStoreInfo info) {
			if(info.ModuleStore.ReadOnly) {
				Messaging.GetMessaging(null).Show("Model Merge", string.Format("Cannot merge differences into {0}. The {1} file is readonly.", info.ModuleName, info.ModuleStore.Name));
			}
			return true;
		}
		#endregion
		#region UpdateActions
#if DebugTest
		public void UpdateActionStateForTests() {
			UpdateActionState();
		}
#endif
		protected override void UpdateActionState() {
			if(IsDisposing || Disposing || lockUpdateActionState) {
				return;
			}
			if(LockUpdateActionState()) {
				try {
					UpdateActionStateCore();
				}
				finally {
					updateActionStateLocked = false;
				}
				return;
			}
		}
		private void UpdateActionStateCore() {
			bool modelApplicationSet = ModelApplicationCore != null;
			ModelTreeListNode lockCurrentModelNode = CurrentModelNode;
			openAction.Active.SetItemValue("IsStandalone", IsStandalone);
			showUnusableNodesAction.Active.SetItemValue("DesignMode", !DesignMode);
			showUnusableNodesAction.Enabled.SetItemValue("IsModelApplication", modelApplicationSet);
			if(ModelEditorControl != null) {
				CanChangeAspect = !ModelEditorControl.CanShowLayout(GetModelNode(ModelEditorControl.CurrentModelTreeListNode));
				ModelEditorControl.modelTreeList.OptionsDragAndDrop.DragNodesMode = LastValidationResult ? DragNodesMode.Single : DragNodesMode.None;
				if(MergeDiffsMode) {
					ModelEditorControl.modelTreeList.OptionsDragAndDrop.DragNodesMode = DragNodesMode.None;
				}
			}
			changeAspectAction.Enabled.SetItemValue("CanChangeAspect", CanChangeAspect);
			changeAspectAction.Enabled.SetItemValue("IsModelApplication", modelApplicationSet);
			showLocalizationForm.Enabled.SetItemValue("IsModelApplication", modelApplicationSet);
			showLocalizationForm.Active.SetItemValue("MergeDiffsMode", !MergeDiffsMode);
			if(modelApplicationSet) {
				List<string> aspectNames = new List<string>(ModelApplicationCore.GetAspectNames());
				showLocalizationForm.Enabled.SetItemValue("AspectsCount", aspectNames.Count > 0);
			}
			SaveActionUpadate(modelApplicationSet);
			ReloadActionUpdate(modelApplicationSet);
			GoSourceNodeActionUpdate(modelApplicationSet, lockCurrentModelNode);
			UpdateDeleteAction(modelApplicationSet, lockCurrentModelNode);
			bool selectedNodesIsOneLevel = SelectedNodesIsOneLevel;
			IndexActionsUpdate(modelApplicationSet, lockCurrentModelNode, selectedNodesIsOneLevel);
			FillAddNodeItems(modelApplicationSet, lockCurrentModelNode);
			UpdateNavigationItems();
			CopyActionsUpdate(modelApplicationSet, lockCurrentModelNode, selectedNodesIsOneLevel);
			ResetDifferencesActionUpdate(lockCurrentModelNode);
			GenerateContentActionUpdate(lockCurrentModelNode);
			FillAspectsActionItems(modelApplicationSet);
			groupNodesAction.Enabled.SetItemValue("CurrentModelNode", lockCurrentModelNode != null);
			if(lockCurrentModelNode != null) {
				groupNodesAction.Enabled.SetItemValue("GroupPropertyName", Adapter.CanGroupChilds(lockCurrentModelNode));
				groupNodesAction.Caption = Adapter.IsGroupChilds(lockCurrentModelNode) ? "Un&group" : "&Group";
			}
			findAction.Enabled.SetItemValue("IsModelApplication", modelApplicationSet);
			loadedModulesAction.Enabled.SetItemValue("IsModelApplication", modelApplicationSet);
			if(needUpdateActionState) {
				needUpdateActionState = false;
				UpdateActionStateCore();
			}
		}
		private bool LockUpdateActionState() {
			lock(lockObj) {
				if(!updateActionStateLocked) {
					updateActionStateLocked = true;
					return updateActionStateLocked;
				}
				needUpdateActionState = true;
				return false;
			}
		}
		private void GoSourceNodeActionUpdate(bool modelApplicationSet, ModelTreeListNode lockCurrentModelNode) {
			try {
				goSourceNodeAction.Enabled.BeginUpdate();
				goSourceNodeAction.Enabled.SetItemValue("IsModelApplication", modelApplicationSet);
				goSourceNodeAction.Active.SetItemValue("MergeDiffsMode", !MergeDiffsMode);
				if(lockCurrentModelNode != null) {
					goSourceNodeAction.Enabled.SetItemValue("IsLink", IsEditableNode(lockCurrentModelNode) &&
						lockCurrentModelNode.ModelTreeListNodeType != ModelTreeListNodeType.Primary);
				}
			}
			finally {
				goSourceNodeAction.Enabled.EndUpdate();
			}
		}
		private void SaveActionUpadate(bool modelApplicationSet) {
			try {
				saveAction.Enabled.BeginUpdate();
				saveAction.Enabled.SetItemValue("IsModified", IsModified);
				saveAction.Enabled.SetItemValue("IsModelApplication", modelApplicationSet);
				saveAction.Active.SetItemValue("MergeDiffsMode", !MergeDiffsMode);
			}
			finally {
				saveAction.Enabled.EndUpdate();
			}
		}
		private void ReloadActionUpdate(bool modelApplicationSet) {
			reloadAction.Active.SetItemValue("MergeDiffsMode", !MergeDiffsMode);
			if(reloadAction.Active) {
				reloadAction.Enabled.SetItemValue("IsModified", IsModified);
				reloadAction.Enabled.SetItemValue("IsModelApplication", modelApplicationSet);
			}
		}
		private void IndexActionsUpdate(bool modelApplicationSet, ModelTreeListNode lockCurrentModelNode, bool selectedNodesIsOneLevel) {
			try {
				nodeUpAction.Enabled.BeginUpdate();
				nodeDownAction.Enabled.BeginUpdate();
				bool isGrouped = lockCurrentModelNode != null && lockCurrentModelNode.Parent != null && lockCurrentModelNode.Parent.ModelTreeListNodeType == ModelTreeListNodeType.Group;
				nodeUpAction.Enabled.SetItemValue("NotGrouped", !isGrouped);
				nodeDownAction.Enabled.SetItemValue("NotGrouped", !isGrouped);
				if(isGrouped) {
					return;
				}
				bool enabled = false;
				if(ModelEditorControl != null) {
					enabled = ModelAttributesPropertyEditorController.IsPropertyVisible(GetModelNode(lockCurrentModelNode), ModelValueNames.Index);
				}
				else {
					enabled = true;
				}
				nodeUpAction.Enabled.SetItemValue("CanChangeIndex", enabled);
				nodeUpAction.Enabled.SetItemValue("IsModelApplication", modelApplicationSet);
				nodeUpAction.Active.SetItemValue("MergeDiffsMode", !MergeDiffsMode);
				nodeDownAction.Enabled.SetItemValue("CanChangeIndex", enabled);
				nodeDownAction.Enabled.SetItemValue("IsModelApplication", modelApplicationSet);
				nodeDownAction.Active.SetItemValue("MergeDiffsMode", !MergeDiffsMode);
				nodeUpAction.Enabled.SetItemValue("SelectedNodesIsOneLevel", selectedNodesIsOneLevel);
				nodeDownAction.Enabled.SetItemValue("SelectedNodesIsOneLevel", selectedNodesIsOneLevel);
				List<ModelTreeListNode> selectedNodes = SelectedNodes;
				bool selectedNodesIsPrimary = NodesIsPrimary(selectedNodes);
				nodeUpAction.Enabled.SetItemValue("IsPrimaryNode", selectedNodesIsPrimary);
				nodeDownAction.Enabled.SetItemValue("IsPrimaryNode", selectedNodesIsPrimary);
				if(ModelEditorControl != null) {
					nodeUpAction.Enabled.SetItemValue("ModelTreeListFocused", ModelEditorControl.modelTreeList.Focused);
					nodeDownAction.Enabled.SetItemValue("ModelTreeListFocused", ModelEditorControl.modelTreeList.Focused);
				}
				bool isEditableNode = true;
				if(enabled && lockCurrentModelNode != null) {
					foreach(ModelTreeListNode item in selectedNodes) {
						isEditableNode &= IsEditableNode(item);
						if(!isEditableNode) {
							break;
						}
					}
					IEnumerable collection = Adapter.GetChildren(lockCurrentModelNode.Parent);
					nodeDownAction.Enabled.SetItemValue("CanChangeIndex", isEditableNode && NodesCanDown(lockCurrentModelNode, collection));
					nodeUpAction.Enabled.SetItemValue("CanChangeIndex", isEditableNode && NodesCanUp(lockCurrentModelNode, collection));
					bool allItemsSelected = lockCurrentModelNode.Parent == null || selectedNodes.Count == lockCurrentModelNode.Parent.NodesCount;
					nodeUpAction.Enabled.SetItemValue("NotAllItemsSelected", !allItemsSelected);
				}
			}
			finally {
				nodeUpAction.Enabled.EndUpdate();
				nodeDownAction.Enabled.EndUpdate();
			}
		}
		private void UpdateNavigationItems() {
			navigationBackAction.Enabled.SetItemValue("CanBack", navigationPoints.CanBack);
			navigationBackAction.Enabled.SetItemValue("Validation error", LastValidationResult);
			navigationForwardAction.Enabled.SetItemValue("CanForward", navigationPoints.CanForward);
			navigationForwardAction.Enabled.SetItemValue("Validation error", LastValidationResult);
		}
		private void GenerateContentActionUpdate(ModelTreeListNode lockCurrentModelNode) {
			bool isMultiSelect = SelectedNodes.Count > 1;
			generateContentAction.Active.SetItemValue("MergeDiffsMode", !MergeDiffsMode);
			if(generateContentAction.Active) {
				try {
					generateContentAction.Enabled.BeginUpdate();
					generateContentAction.Enabled.SetItemValue("IsNotMultiSelect", !isMultiSelect);
					generateContentAction.Enabled.SetItemValue("IsNodeValid", LastValidationResult);
					generateContentAction.Enabled.SetItemValue("LinksCollected", !linksCollecting);
					ModelNode lockModelNode = GetModelNode(lockCurrentModelNode);
					generateContentAction.Enabled.SetItemValue("HasGenerator", ModelEditorHelper.IsGenerateContentNode(lockModelNode));
					if(lockModelNode != null) {
						generateContentAction.Enabled.SetItemValue("IsReadOnly", !Adapter.fastModelEditorHelper.IsReadOnly(lockModelNode, (ModelNode)null));
					}
				}
				finally {
					generateContentAction.Enabled.EndUpdate();
				}
			}
		}
		private void ResetDifferencesActionUpdate(ModelTreeListNode lockCurrentModelNode) {
			try {
				resetDifferencesAction.Enabled.BeginUpdate();
				mergeDifferencesAction.Enabled.BeginUpdate();
				showDifferences.Enabled.BeginUpdate();
				bool hasSelectedNodesModification = HasSelectedNodesModification();
				showDifferences.Enabled.SetItemValue("HasModification", hasSelectedNodesModification);
				showDifferences.Enabled.SetItemValue("IsNotLinks", lockCurrentModelNode != null ? lockCurrentModelNode.ModelTreeListNodeType != ModelTreeListNodeType.Links : true);
				bool hasPrimaryNode = lockCurrentModelNode != null &&
								!(lockCurrentModelNode.ModelTreeListNodeType == ModelTreeListNodeType.Links ||
								lockCurrentModelNode.ModelTreeListNodeType == ModelTreeListNodeType.Collection);
				mergeDifferencesAction.Active.SetItemValue("NotStandalone", !IsStandalone);
				mergeDifferencesAction.Enabled.SetItemValue("HasPrimaryNode", hasPrimaryNode);
				mergeDifferencesAction.Enabled.SetItemValue("HasModification", hasSelectedNodesModification);
				mergeDifferencesAction.Enabled.SetItemValue("DesignMode", !DesignMode);
				resetDifferencesAction.Active.SetItemValue("MergeDiffsMode", !MergeDiffsMode);
				if(resetDifferencesAction.Active) {
					resetDifferencesAction.Enabled.SetItemValue("HasModification", hasSelectedNodesModification);
					resetDifferencesAction.Enabled.SetItemValue("HasPrimaryNode", hasPrimaryNode);
				}
			}
			finally {
				resetDifferencesAction.Enabled.EndUpdate();
				mergeDifferencesAction.Enabled.EndUpdate();
				showDifferences.Enabled.EndUpdate();
			}
		}
		private void CopyActionsUpdate(bool modelApplicationSet, ModelTreeListNode lockCurrentModelNode, bool selectedNodesIsOneLevel) {
			try {
				copyAction.Enabled.BeginUpdate();
				pasteAction.Enabled.BeginUpdate();
				cloneAction.Enabled.BeginUpdate();
				copyAction.Active.SetItemValue("MergeDiffsMode", !MergeDiffsMode);
				pasteAction.Active.SetItemValue("MergeDiffsMode", !MergeDiffsMode);
				cloneAction.Active.SetItemValue("MergeDiffsMode", !MergeDiffsMode);
				copyAction.Enabled.SetItemValue("IsModelApplication", modelApplicationSet);
				pasteAction.Enabled.SetItemValue("IsModelApplication", modelApplicationSet);
				cloneAction.Enabled.SetItemValue("IsModelApplication", modelApplicationSet);
				if(ModelEditorControl != null) {
					copyAction.Enabled.SetItemValue("ModelTreeListFocused", ModelEditorControl.modelTreeList.Focused);
					pasteAction.Enabled.SetItemValue("ModelTreeListFocused", ModelEditorControl.modelTreeList.Focused);
				}
				if(lockCurrentModelNode != null) {
					List<ModelNode> modelItems = new List<ModelNode>();
					List<ModelTreeListNode> _nodeBuffers = NodeBuffers;
					foreach(ModelTreeListNode item in _nodeBuffers) {
						modelItems.Add(item.ModelNode);
					}
					copyAction.Enabled.SetItemValue("Editable", IsEditableNode(lockCurrentModelNode));
					cloneAction.Enabled.SetItemValue("Editable", IsEditableNode(lockCurrentModelNode));
					copyAction.Enabled.SetItemValue("CanCopy", selectedNodesIsOneLevel);
					pasteAction.Enabled.SetItemValue("CanPasteNodes", Adapter.fastModelEditorHelper.CanAddNode(GetModelNode(lockCurrentModelNode), modelItems.ToArray()));
					pasteAction.Enabled.SetItemValue("Validation error", LastValidationResult);
					cloneAction.Enabled.SetItemValue("CanClone", CanCloneNodes(SelectedNodes));
					cloneAction.Enabled.SetItemValue("Validation error", LastValidationResult);
				}
			}
			finally {
				copyAction.Enabled.EndUpdate();
				pasteAction.Enabled.EndUpdate();
				cloneAction.Enabled.EndUpdate();
			}
		}
		private void UpdateDeleteAction(bool modelApplicationSet, ModelTreeListNode lockCurrentModelNode) {
			bool result = false;
			if(SelectedNodes.Count > 1) {
				foreach(ModelTreeListNode node in SelectedNodes) {
					result = IsEditableNode(node);
					if(result) {
						result = Adapter.fastModelEditorHelper.CanDeleteNode(node.ModelNode, ReadOnly);
					}
					if(!result) {
						break;
					}
				}
			}
			else {
				result = IsEditableNode(lockCurrentModelNode);
				if(result) {
					result = Adapter.fastModelEditorHelper.CanDeleteNode(GetModelNode(lockCurrentModelNode), ReadOnly);
				}
			}
			try {
				deleteAction.Enabled.BeginUpdate();
				deleteAction.Enabled.SetItemValue("Editable", IsEditableNode(lockCurrentModelNode));
				deleteAction.Enabled.SetItemValue("CanDeleteNode", result);
				deleteAction.Enabled.SetItemValue("IsModelApplication", modelApplicationSet);
				deleteAction.Active.SetItemValue("MergeDiffsMode", !MergeDiffsMode);
			}
			finally {
				deleteAction.Enabled.EndUpdate();
			}
		}
		private void FillAddNodeItems(bool modelApplicationSet, ModelTreeListNode lockCurrentModelNode) {
			try {
				addNodeAction.BeginUpdate();
				addNodeAction.Enabled.SetItemValue("Validation error", LastValidationResult);
				addNodeAction.Enabled.SetItemValue("IsModelApplication", modelApplicationSet);
				addNodeAction.Active.SetItemValue("MergeDiffsMode", !MergeDiffsMode);
				addNodeAction.Items.Clear();
				if(SelectedNodes.Count > 1) {
					return;
				}
				if(lockCurrentModelNode != null) {
					if(lockCurrentModelNode.ModelNode is IModelBandsLayout) {
						addNodeAction.Enabled.SetItemValue("ModelBandsLayoutEnabled", ((IModelBandsLayout)lockCurrentModelNode.ModelNode).Enable);
					}
					else {
						addNodeAction.Enabled.SetItemValue("ModelBandsLayoutEnabled", true);
					}
					addNodeAction.Items.AddRange(LinksNodeHelper.GetCreatableItems(lockCurrentModelNode, Adapter));
				}
			}
			finally {
				addNodeAction.EndUpdate();
			}
		}
		private LinksNodeHelper LinksNodeHelper {
			get {
				if(linksNodeHelper == null) {
					linksNodeHelper = new LinksNodeHelper(Adapter.fastModelEditorHelper);
				}
				return linksNodeHelper;
			}
		}
		private void FillAspectsActionItems(bool modelApplicationSet) {
			changeAspectAction.Items.Clear();
			if(modelApplicationSet && ModelApplicationCore.CurrentAspectProvider != null) {
				foreach(string aspectName in AllAspectNames) {
					ChoiceActionItem choiceActionItem = new ChoiceActionItem(aspectName, aspectName);
					changeAspectAction.Items.Add(choiceActionItem);
					if(ModelApplicationCore.CurrentAspectProvider.CurrentAspect == aspectName) {
						changeAspectAction.SelectedItem = choiceActionItem;
					}
				}
			}
		}
		private bool NodesCanDown(ModelTreeListNode lockCurrentModelNode, IEnumerable collection) {
			if(lockCurrentModelNode == null) {
				return false;
			}
			bool nodeCanDown = lockCurrentModelNode.ModelNode.Index > -1 || !lockCurrentModelNode.ModelNode.Index.HasValue;
			if(nodeCanDown) {
				foreach(ModelTreeListNode item in SelectedNodes) {
					if(nodeCanDown && item.Parent != null) {
						nodeCanDown = item.ModelNode.Index > -1 || !item.ModelNode.Index.HasValue;
						if(nodeCanDown) {
							int nodeIndex = GetNodePositionInCollection(item, collection);
							nodeCanDown = !(item.Parent.ModelNode == null || nodeIndex == item.Parent.ModelNode.NodeCount - 1);
						}
					}
					else {
						break;
					}
				}
			}
			return nodeCanDown;
		}
		private bool NodesCanUp(ModelTreeListNode lockCurrentModelNode, IEnumerable collection) {
			if(SelectedNodes.Count > 1) {
				List<int> itemsPosition = new List<int>();
				foreach(ModelTreeListNode item in SelectedNodes) {
					if(item.ModelNode.Index < 0) {
						return true;
					}
					itemsPosition.Add(GetNodePositionInCollection(item, collection));
				}
				itemsPosition.Sort();
				if(itemsPosition[0] == 0) {
					for(int i = 0; i < itemsPosition.Count - 1; i++) {
						if(itemsPosition[i + 1] - itemsPosition[i] > 1) {
							return true;
						}
					}
				}
				else {
					return true;
				}
				return false;
			}
			else {
				return lockCurrentModelNode != null ? GetNodePositionInCollection(lockCurrentModelNode, collection) != 0 : false;
			}
		}
		#endregion
		#region Events
		public event EventHandler CurrentNodeChanged;
		public event EventHandler CurrentAspectChanged;
		public event EventHandler<CustomLoadModelEventArgs> CustomLoadModel;
#if DebugTest
		public event EventHandler DebugTest_CustomizeModel;
#endif
		private EventHandler lostFocusEventHandler;
		private EventHandler<NavigateToEventArgs> navigateToRefPropertyEventHandler;
		private EventHandler<PropertyChangingEventArgs> propertyChangingEventHandler;
		private EventHandler<PropertyChangingEventArgs> propertyChangedEventHandler;
		private EventHandler gotFocusEventHandler;
		private EventHandler modelPropertyGridGotFocusEventHandler;
		private EventHandler<NodeIndexChangedEventArgs> nodeIndexChangedEventHandler;
		private EventHandler closedEventHandler;
		private EventHandler focusedNodeChangedEventHandler;
		#endregion
#if DebugTest
		public const string DebugTest_CurrentAspectAttributeName = CurrentAspectAttributeName;
		public bool DebugTest_ValidateNode(ModelTreeListNode node, bool showMessage) {
			return ValidateNode(node, showMessage);
		}
		public string DebugTest_GetDescription(ModelNode node, string propertyName) {
			return GetDescription(node, propertyName);
		}
		public int DebugTest_GetNodeIndex_Down(ModelTreeListNode node, List<ModelTreeListNode> selectedNodes, IEnumerable collection) {
			return GetNodeIndex_Down(node, selectedNodes, collection);
		}
		public int DebugTest_GetNodePositionInCollection(ModelTreeListNode node, IEnumerable collection) {
			return GetNodePositionInCollection(node, collection);
		}
		public void DebugTest_ExecuteAction(SimpleAction action) {
			ExecuteAction(action);
		}
		public int DebugTest_LockErrorMessages {
			get { return lockErrorMessages; }
			set { lockErrorMessages = value; }
		}
		public bool IsShowDifferences {
			get { return isShowDifferences; }
			set { isShowDifferences = value; }
		}
		public NavigationHistory<ModelTreeListNode> DebugTest_NavigationPoints {
			get { return navigationPoints; }
		}
		public ErrorMessages DebugTest_ErrorMessages {
			get { return ErrorMessages; }
		}
#endif
	}
}
