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
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using DevExpress.XtraTreeList;
namespace DevExpress.XtraReports.Design {
	using System.Windows.Forms;
	using DevExpress.XtraReports.UI;
	using System.ComponentModel.Design;
	using System.Drawing.Design;
	using DevExpress.XtraReports.Native;
	using DevExpress.Utils;
	using System.Collections.Generic;
	using DevExpress.XtraPrinting.Native;
	using DevExpress.XtraTreeList.Nodes;
	using DevExpress.XtraTreeList.Handler;
	using DevExpress.XtraReports.Design.Tools;
	using DevExpress.XtraReports.UserDesigner;
	using DevExpress.XtraTreeList.Native;
	using DevExpress.XtraReports.Localization;
	using DevExpress.XtraReports.Design.Commands;
	using System.Linq;
	using DevExpress.Utils.Design;
	using DevExpress.Data.Browsing;
	using DevExpress.Data.Browsing.Design;
	public class ReportExplorerController : ReportExplorerControllerBase {
		#region inner classes
		[ToolboxBitmap(typeof(LocalResFinder), "Images.Containers16x16.ComponentContainer.png")]
		class ComponentContainer : Component { 
		}
		[ToolboxBitmap(typeof(LocalResFinder), "Images.Containers16x16.StyleContainer.png")]
		class StyleContainer : Component {
		}
		[ToolboxBitmap(typeof(LocalResFinder), "Images.Containers16x16.FormattingRuleContainer.png")]
		class FormattingRuleContainer : Component {
		} 
		protected enum ContainerNodeType {
			StyleContainerNode,
			FormattingRuleContainerNode,
			ExternalControlStyleNode,
			None,
		}
		protected class ContextMenuHelper {
			#region static
			public static ContextMenuHelper CreateInstance(ComponentNode node) {
				switch(IdentifyNode(node)) {
					case ContainerNodeType.StyleContainerNode:
						return new StyleContainerContextMenuHelper(node);
					case ContainerNodeType.FormattingRuleContainerNode:
						return new FormattingRuleContainerMenuHelper(node);
					case ContainerNodeType.ExternalControlStyleNode:
						return new ExternalControlStyleContextMenuHelper(node);
					default:
						return new ContextMenuHelper(node);
				}
			}
			protected static ContainerNodeType IdentifyNode(ComponentNode node) {
				if(node.Component is StyleContainer)
					return ContainerNodeType.StyleContainerNode;
				else if(node.Component is FormattingRuleContainer)
					return ContainerNodeType.FormattingRuleContainerNode;
				else if(node.Component is XRControlStyle && node.Component.Site == null)
					return ContainerNodeType.ExternalControlStyleNode;
				return ContainerNodeType.None;
			}
			#endregion
			ComponentNode node;
			protected ComponentNode Node { get { return node; } }
			public ContextMenuHelper(ComponentNode node) {
				this.node = node;
			}
			public virtual MenuItemDescriptionCollection CreateMenuItems() {
				return null;
			}
		}
		class StyleContainerContextMenuHelper : ContextMenuHelper {
			public StyleContainerContextMenuHelper(ComponentNode node)
				: base(node) {
			}
			public override MenuItemDescriptionCollection CreateMenuItems() {
				MenuItemDescriptionCollection items = new MenuItemDescriptionCollection();
				items.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_AddStyle), null, FormattingComponentCommands.AddStyle));
				items.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_EditStyles), null, FormattingComponentCommands.EditStyles));
				items.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_PurgeStyles), null, FormattingComponentCommands.PurgeStyles));
				items.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_ClearStyles), null, FormattingComponentCommands.ClearStyles));
				return items;
			}
		}
		class ExternalControlStyleContextMenuHelper : ContextMenuHelper {
			public ExternalControlStyleContextMenuHelper(ComponentNode node)
				: base(node) {
			}
			public override MenuItemDescriptionCollection CreateMenuItems() {
				MenuItemDescriptionCollection items = new MenuItemDescriptionCollection();
				items.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_AddStyle), null, FormattingComponentCommands.AddStyle));
				items.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_EditStyles), null, FormattingComponentCommands.EditStyles));
				items.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_SelectControlsWithStyle), null, FormattingComponentCommands.SelectControlsWithStyle));
				return items;
			}
		}
		class FormattingRuleContainerMenuHelper : ContextMenuHelper {
			public FormattingRuleContainerMenuHelper(ComponentNode node)
				: base(node) {
			}
			public override MenuItemDescriptionCollection CreateMenuItems() {
				MenuItemDescriptionCollection items = new MenuItemDescriptionCollection();
				items.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_AddFormattingRule), null, FormattingComponentCommands.AddFormattingRule));
				items.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_EditFormattingRules), null, FormattingComponentCommands.EditFormattingRules));
				items.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_PurgeFormattingRules), null, FormattingComponentCommands.PurgeFormattingRules));
				items.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_ClearFormattingRules), null, FormattingComponentCommands.ClearFormattingRules));
				return items;
			}
		}
		#endregion
		#region static
		static Bitmap boundedImage = ResLoader.LoadBitmap("Images.bounded.png", typeof(LocalResFinder), Color.Empty);
		internal static Bitmap BoundedImage {
			get { return boundedImage; }
		}
		static bool CanDropNode(IDesignerHost designerHost, ComponentNode targetNode, ComponentNode draggedNode) {
			if(targetNode == null || draggedNode == null)
				return false;
			if(draggedNode.Equals(targetNode) || ContainsNode(draggedNode, targetNode))
				return false;
			if(draggedNode.Component is XRControlStyle)
				return FormattingComponentAssignmentHelper.CanAssignStyle(targetNode.Component as XRControl);
			if(draggedNode.Component is FormattingRule)
				return FormattingComponentAssignmentHelper.CanAssignRule(targetNode.Component as XRControl);	
			XRControl container = targetNode.Component as XRControl;
			if(container == null)
				return false;
			XRControl control = draggedNode.Component as XRControl;
			if(control == null)
				return false;
			if(CanReorderControls(container, control))
				return true;
			if(!LockService.GetInstance(designerHost).CanChangeControlParent(control, container))
				return false;
			XRComponentDesigner designer = designerHost.GetDesigner(draggedNode.Component) as XRComponentDesigner;
			if(designer == null || !designer.CanDragInReportExplorer)
				return false;
			return container.CanAddComponent(control) && !container.Equals(control.Parent);
		}
		static bool CanReorderControls(XRControl c1, XRControl c2) {
			return ((c1 is XRTableCell && c2 is XRTableCell) || (c1 is SubBand && c2 is SubBand)) && c1.Parent == c2.Parent;
		}
		public static int GetImageIndex(IComponent comp, bool bounded) {
			if(!bounded)
				return GetImageIndex(comp);
			if(comp == null)
				return -1;
			int result;
			string key = GetBoundedName(GetTypeName(comp));
			if(!imageIndices.TryGetValue(key, out result)) {
				int index = GetImageIndex(comp);
				Bitmap boundedBitmap = CreateBoundedBitmap(ImageCollection.Images[index]);
				return AddImage(boundedBitmap, key);
			}
			return result;
		}
		static string GetBoundedName(string name) {
			return name + ".Bounded";
		}
		static Bitmap CreateBoundedBitmap(Image source) {
			Bitmap boundedBmp = new Bitmap(source);
			using(Graphics gr = Graphics.FromImage(boundedBmp)) {
				gr.DrawImage(BoundedImage, source.Width - BoundedImage.Width, source.Height - BoundedImage.Height);
			}
			return boundedBmp;
		}
		#endregion
		#region fields & properties
		private IMenuCommandService menuCommandService;
		public bool InMessageProcessing {
			get;
			private set;
		}
		XtraContextMenuBase fContextMenu;
		bool? draggingCursorWithinReportExplorer = null;
		#endregion
		public ReportExplorerController(IServiceProvider servProvider)
			: base(servProvider) {
			this.menuCommandService = servProvider.GetService(typeof(IMenuCommandService)) as IMenuCommandService;
		}
		protected override int GetImageIndexCore(XRControl control) {
			if(control != null && control.HasBindings())
				return GetImageIndex(control, control.HasBindings());
			return base.GetImageIndexCore(control);
		}
		protected override void OnSelectionChanged(object sender, EventArgs e) {
			if(!DesignMethods.IsDesignerInTransaction(designerHost) && !InMessageProcessing) {
				InMessageProcessing = true;
				SetTreeViewSelection();
				InMessageProcessing = false;
			}
		}
		public void SelectComponent(IComponent component) {
			if(!InMessageProcessing) {
				InMessageProcessing = true;
				selectionService.SetSelectedComponents(new object[] { component }, SelectionTypes.Replace);
				InMessageProcessing = false;
			}
		}
		public override void UpdateTreeList() {
#if DEBUGTEST
			if(TestEnvironment.IsTestRunning()) {
				UpdateViewInMessageProcessing();
				return;
			}
#endif
			Application.Idle -= new EventHandler(Application_Idle);
			Application.Idle += new EventHandler(Application_Idle);
		}
		void Application_Idle(object sender, EventArgs e) {
			Application.Idle -= new EventHandler(Application_Idle);
			if(designerHost.RootComponent != null) {
				ReportDesigner designer = designerHost.GetDesigner(designerHost.RootComponent) as ReportDesigner;
				if(designer == null || !designer.IsActive) return;
			}
			UpdateViewInMessageProcessing();
		}
		protected void UpdateViewInMessageProcessing() {
			InMessageProcessing = true;
			try {
				UpdateView();
			} finally {
				InMessageProcessing = false;
			}
		}
		protected override void UpdateView() {
			base.UpdateView();
			ReportTreeView.BeginUnboundLoad();
			bool containerNodeIsVisible;
			ReportDesigner designer = designerHost.GetDesigner(designerHost.RootComponent) as ReportDesigner;
			containerNodeIsVisible = designer != null && (designer.StylesNodeVisibility & ComponentVisibility.ReportExplorer) > 0;
			UpdateViewComponentsByPredicate(ReportLocalizer.GetString(ReportStringId.UD_Title_ReportExplorer_Styles), typeof(StyleContainer),
				comp => { return comp is XRControlStyle; }, containerNodeIsVisible, true);
			containerNodeIsVisible = designer != null && (designer.FormattingRulesNodeVisibility & ComponentVisibility.ReportExplorer) > 0;
			UpdateViewComponentsByPredicate(ReportLocalizer.GetString(ReportStringId.UD_Title_ReportExplorer_FormattingRules), typeof(FormattingRuleContainer),
				comp => { return comp is FormattingRule; }, containerNodeIsVisible, true);
			containerNodeIsVisible = designer != null && (designer.ComponentVisibility & ComponentVisibility.ReportExplorer) > 0;
			UpdateViewComponentsByPredicate(ReportLocalizer.GetString(ReportStringId.UD_Title_ReportExplorer_Components), typeof(ComponentContainer),
				comp => { return !comp.IsVisual() && comp.IsVisible(); }, containerNodeIsVisible, false);
			ReportTreeView.EndUnboundLoad();
		}
		void UpdateViewComponentsByPredicate(string nodeText, Type containerType, Predicate<IComponent> predicate, bool containerNodeIsVisible, bool isFormattingNode) {
			ComponentNode componentContainerNode = GetNodeByPredicate(ReportTreeView.Nodes, item => {
				return item.Text.Equals(nodeText);
			});
			Component container;
			bool wasCreated = false;
			if(componentContainerNode != null)
				container = (Component)componentContainerNode.Component;
			else {
				container = (Component)Activator.CreateInstance(containerType);
				componentContainerNode = new ComponentNode(container, ReportTreeView.Nodes, false, nodeText);
				((IList)ReportTreeView.Nodes).Add(componentContainerNode);
				wasCreated = true;
			}
			IList components = GetComponentsByPredicate(predicate, container is StyleContainer);
			UpdateNodes(componentContainerNode.Nodes, components);
			if(wasCreated && componentContainerNode.Nodes.Count > 0)
				componentContainerNode.Expand();
			componentContainerNode.Visible = containerNodeIsVisible && (isFormattingNode || components.Count > 0);
			if(IsControlAlive(ReportTreeView))
				SetTreeViewSelection();
		}
		IList GetComponentsByPredicate(Predicate<IComponent> predicate, bool isStyleComponents) {
			List<IComponent> components = new List<IComponent>();
			if(isStyleComponents) {
				Dictionary<string, XRControlStyle> styles = new Dictionary<string, XRControlStyle>();
				foreach(XRControlStyle style in RootReport.StyleContainer) {
					if(!styles.ContainsKey(style.Name))
						styles.Add(style.Name, style);
				}
				components.AddRange(styles.Values.Where(x => x.Site == null).OrderBy(x => x.Name));
				components.AddRange(styles.Values.Where(x => x.Site != null).OrderBy(x => x.Name));
			} else {
				components = designerHost.Container.Components.OfType<IComponent>().ToList().FindAll(predicate);
				components.Sort((x, y) => {
					string xName = x.Site != null ? x.Site.Name : string.Empty;
					string yName = y.Site != null ? y.Site.Name : string.Empty;
					return Comparer.Default.Compare(xName, yName);
				});
			}
			return components.ToArray();
		}
		private void SetTreeViewSelection() {
			if(draggingCursorWithinReportExplorer.HasValue && !(bool)draggingCursorWithinReportExplorer)
				return;
			IComponent component = selectionService.PrimarySelection as IComponent;
			if(component != null && !(component is DevExpress.XtraCharts.Design.FakeComponent))
				SetSelectedNode(component);
		}
		protected override void UnsubscribeEvents() {
			base.UnsubscribeEvents();
			Application.Idle -= new EventHandler(Application_Idle);
		}
		public override void SubscribeTreeListEvents(TreeList treeList) {
			base.SubscribeTreeListEvents(treeList);
			if(treeList != null) {
				treeList.AfterFocusNode += new NodeEventHandler(tv_AfterSelect);
				treeList.MouseUp += new MouseEventHandler(tv_MouseUp);
				treeList.KeyDown += new KeyEventHandler(tv_KeyDown);
				treeList.DragEnter += new DragEventHandler(tv_DragEnter);
				treeList.DragLeave += new EventHandler(tv_DragLeave);
				treeList.DragOver += new DragEventHandler(tv_DragOver);
				treeList.DragDrop += new DragEventHandler(tv_DragDrop);
				treeList.GiveFeedback += new GiveFeedbackEventHandler(tv_GiveFeedback);
				treeList.CalcNodeDragImageIndex += new CalcNodeDragImageIndexEventHandler(tv_CalcNodeDragImageIndex);
				((XtraTreeView)treeList).ItemDrag += ReportExplorerController_ItemDrag;
			}
		}
		public override void UnsubscribeTreeListEvents(TreeList treeList) {
			base.UnsubscribeTreeListEvents(treeList);
			if(treeList != null) {
				treeList.AfterFocusNode -= new NodeEventHandler(tv_AfterSelect);
				treeList.MouseUp -= new MouseEventHandler(tv_MouseUp);
				treeList.KeyDown -= new KeyEventHandler(tv_KeyDown);
				treeList.DragEnter -= new DragEventHandler(tv_DragEnter);
				treeList.DragLeave -= new EventHandler(tv_DragLeave);
				treeList.DragOver -= new DragEventHandler(tv_DragOver);
				treeList.DragDrop -= new DragEventHandler(tv_DragDrop);
				treeList.GiveFeedback -= new GiveFeedbackEventHandler(tv_GiveFeedback);
				treeList.CalcNodeDragImageIndex -= new CalcNodeDragImageIndexEventHandler(tv_CalcNodeDragImageIndex);
				((XtraTreeView)treeList).ItemDrag -= ReportExplorerController_ItemDrag;
			}
		}
		private void tv_CalcNodeDragImageIndex(object sender, CalcNodeDragImageIndexEventArgs e) {
			ComponentNode draggedNode = ReportTreeView.DraggedNode as ComponentNode;
			if(draggedNode == null || draggedNode.IsFormattingNode || draggedNode.Component is StyleContainer || draggedNode.Component is FormattingRuleContainer ||
				!CanDropNode(designerHost, ReportTreeView.SelectedNode as ComponentNode, ReportTreeView.DraggedNode as ComponentNode))
				e.ImageIndex = -1;
		}
		private void tv_GiveFeedback(object sender, GiveFeedbackEventArgs e) {
			IComponent draggedComponent = (ReportTreeView.DraggedNode as ComponentNode).Component;
			Point currentMousePosition = Control.MousePosition;
			XRControl parentControl = null;
			if(draggingCursorWithinReportExplorer.HasValue && (bool)draggingCursorWithinReportExplorer) {
				ComponentNode node = GetNodeByScreenPos(currentMousePosition.X, currentMousePosition.Y);
				if(node != null && node.Component != null)
					parentControl = node.Component as XRControl;
			} else {
				IBandViewInfoService bandViewSvc = (IBandViewInfoService)serviceProvider.GetService(typeof(IBandViewInfoService));
				if(bandViewSvc != null)
					parentControl = bandViewSvc.GetControlByScreenPoint(currentMousePosition);
			}
			if(parentControl != null && (Control.ModifierKeys & Keys.Control) == 0) {
				if((draggedComponent is XRControlStyle && FormattingComponentAssignmentHelper.CanAssignStyle(parentControl)) ||
					(draggedComponent is FormattingRule && FormattingComponentAssignmentHelper.CanAssignRule(parentControl))) {
					e.UseDefaultCursors = false;
					if(!Comparer.Equals(Cursor.Current, XRCursors.CanAssignFormattingComponentCursor)) {
						Cursor.Current = XRCursors.CanAssignFormattingComponentCursor;
					}
				} else {
					e.UseDefaultCursors = true;
				}
			}
		}
		private void tv_AfterSelect(object sender, NodeEventArgs e) {
			if(!InMessageProcessing && ReportTreeView.FocusedNode != null) {
				IComponent component = ((ComponentNode)ReportTreeView.FocusedNode).Component;
				if(component == null && ReportTreeView.FocusedNode.ParentNode != null)
					component = ((ComponentNode)ReportTreeView.FocusedNode.ParentNode).Component;
				if(component != null && (component.Site != null || component is XRControlStyle))
					SelectComponent(component);
			}
		}
		private void tv_MouseUp(object sender, MouseEventArgs e) {
			TreeListNode node = ReportTreeView.GetNodeAt(e.X, e.Y);
			if(node == null || designerHost.IsDebugging())
				return;
			if(e.Button.IsRight()) {
				if(node != null) 
					ReportTreeView.SelectNode(node);
				if(ReportTreeView.SelectedNode != null)
					ShowContextMenu(node as ComponentNode, System.Windows.Forms.Control.MousePosition);
			}
		}
		private void tv_KeyDown(object sender, KeyEventArgs e) {
			if(e.KeyCode == Keys.Delete)
				DeleteSelectedComponent();
		}
		private void tv_DragEnter(object sender, DragEventArgs e) {
			e.Effect = e.AllowedEffect;
			draggingCursorWithinReportExplorer = true;
		}
		private void tv_DragLeave(object sender, EventArgs e) {
			draggingCursorWithinReportExplorer = false;
			ReportTreeView.SelectNode(ReportTreeView.DraggedNode); 
		}
		private void tv_DragOver(object sender, DragEventArgs e) {
			TreeListNode nodeByPoint = GetNodeByScreenPos(e.X, e.Y);
			if((ReportTreeView.DraggedNode as ComponentNode).IsFormattingNode && nodeByPoint == null)   
				ReportTreeView.SelectNode(ReportTreeView.DraggedNode);
			else
				ReportTreeView.SelectNode(nodeByPoint);
			if(ReportTreeView.SelectedNode != null)
				((XtraListNode)ReportTreeView.SelectedNode).Expand();
			e.Effect = GetDragDropEffects();
		}
		DragDropEffects GetDragDropEffects() {
			if(CanDropNode(designerHost, ReportTreeView.SelectedNode as ComponentNode, ReportTreeView.DraggedNode as ComponentNode))
				return (ReportTreeView.DraggedNode as ComponentNode).IsFormattingNode ? DragDropEffects.Copy : DragDropEffects.Move;
			return DragDropEffects.None;
		}
		private void tv_DragDrop(object sender, DragEventArgs e) {
			ComponentNode targetNode = GetNodeByScreenPos(e.X, e.Y);
			DragInsertPosition insertPosition = e.GetDXDragEventArgs(treeList).DragInsertPosition;
			ComponentNode draggedNode = ReportTreeView.DraggedNode as ComponentNode;
			if(!CanDropNode(designerHost, targetNode, draggedNode))
				return;
			if(draggedNode.IsFormattingNode) {
				DropFormattingNode(targetNode.Component as XRControl, draggedNode.Component);
				SetSelectedNode(draggedNode.Component);
				return;
			}
			XRControl container = (XRControl)targetNode.Component;
			XRControl control = (XRControl)draggedNode.Component;
			if(CanReorderControls(container, control)) {
				PerformTransaction(String.Format(DesignSR.TransFmt_OneMove, control.Site.Name), () => {
					XRComponentDesigner designer = (XRComponentDesigner)designerHost.GetDesigner(container.Parent);
					designer.OnCollectionChanging(container);
					if(insertPosition == DragInsertPosition.Before || insertPosition == DragInsertPosition.After)
						control.Index = container.Index;
					else 
						container.SwapWith(control);
					designer.OnCollectionChanged(container);
				});
				SelectComponent(control);
			} else {
				PerformTransaction(String.Format(DesignSR.TransFmt_OneMove, control.Site.Name), () => {
					XRControlDesigner designer = (XRControlDesigner)designerHost.GetDesigner(control);
					designer.ChangeParent(container);
				});
				SetSelectedNode(control);
			}
		}
		void ReportExplorerController_ItemDrag(object sender, ItemDragEventArgs e) {
			List<FormattingDataObject> dataItems = new List<FormattingDataObject>();
			ComponentNode draggedNode = (ComponentNode)ReportTreeView.DraggedNode;
			if(draggedNode.IsFormattingNode)
				dataItems.Add(new FormattingDataObject(draggedNode.Component));
			if(dataItems.Count > 0)
				ReportTreeView.DoDragDrop(dataItems.ToArray(), DragDropEffects.Move | DragDropEffects.Copy);
			if(draggedNode.IsFormattingNode)
				selectionService.SetSelectedComponents(new object[] {draggedNode.Component});
			draggingCursorWithinReportExplorer = null;
		}
		void DropFormattingNode(XRControl targetControl, IComponent draggedComponent) {
			IXRMenuCommandService menuCommandService = serviceProvider.GetService(typeof(IXRMenuCommandService)) as IXRMenuCommandService;
			if(menuCommandService == null)
				return;
			FormattingDataObject[] data = new FormattingDataObject[] { new FormattingDataObject(draggedComponent)};
			SelectComponent(draggedComponent);
			if(draggedComponent is XRControlStyle) {
				menuCommandService.GlobalInvoke(Commands.FormattingComponentCommands.AssignStyleToXRControl, new object[] { targetControl, data });
			} else if(draggedComponent is FormattingRule) {
				menuCommandService.GlobalInvoke(Commands.FormattingComponentCommands.AssignRuleToXRControl, new object[] { targetControl, data });
			}
		}
		void PerformTransaction(string desc, Action action) {
			DesignerTransaction trans = designerHost.CreateTransaction(desc);
			try {
				action();
				trans.Commit();
			} catch {
				trans.Cancel();
			}
		}
		void ShowContextMenu(ComponentNode node, Point point) {
			if(node == null || !CanShowContextMenu()) return;
			if(node.Component is StyleContainer || node.Component is FormattingRuleContainer || node.IsExternalControlStyleNode) {
				IXRMenuCommandService menuCommandService = serviceProvider.GetService(typeof(IXRMenuCommandService)) as IXRMenuCommandService;
				if(menuCommandService == null)
					return;
				if(fContextMenu != null)
					fContextMenu.Dispose();
				fContextMenu = CreateContextMenu();
				if(fContextMenu == null)
					return;
				IList<MenuItemDescription> items = CreateMenuItems(node);
				if(items == null || items.Count == 0)
					return;
				fContextMenu.AddMenuItems(items, null, null);
				menuCommandService.ShowContextMenu(fContextMenu, point.X, point.Y);
			} else {
				IMenuCommandService menuCommandService = serviceProvider.GetService(typeof(IMenuCommandService)) as IMenuCommandService;
				if(menuCommandService != null && designerHost.GetDesigner(node.Component) != null)
					menuCommandService.ShowContextMenu(MenuCommandServiceCommands.ReportExplorerMenu, point.X, point.Y);
			}
		}
		protected MenuItemDescriptionCollection CreateMenuItems(ComponentNode node) {
			ContextMenuHelper helper = ContextMenuHelper.CreateInstance(node);
			MenuItemDescriptionCollection items = helper.CreateMenuItems();
			if(serviceProvider != null) {
				IMenuCreationService serv = serviceProvider.GetService(typeof(IMenuCreationService)) as IMenuCreationService;
				if(serv != null)
					serv.ProcessMenuItems(MenuKind.ReportExplorer, items);
			}
			return items;
		}
		protected virtual XtraContextMenuBase CreateContextMenu() {
			return new XtraContextMenu();
		}
		bool CanShowContextMenu() {
			if(ReportTreeView.Parent != null && ReportTreeView.Parent.Parent != null)
				return ReportTreeView.Parent.Parent.ContextMenuStrip == null && ReportTreeView.Parent.Parent.ContextMenu == null;
			return true;
		}
		private void DeleteSelectedComponent() {
			ComponentNode componentNode = ReportTreeView.SelectedNode as ComponentNode;
			if(componentNode == null || componentNode.Component == null)
				return;
			IMenuCommandServiceEx menuCommandServiceEx = (IMenuCommandServiceEx)designerHost.GetService(typeof(IMenuCommandService));
			selectionService.SetSelectedComponents(new object[] { componentNode.Component }, SelectionTypes.Replace);
			menuCommandServiceEx.GlobalInvoke(StandardCommands.Delete);
		}
		private ComponentNode GetNodeByScreenPos(int screenX, int screenY) {
			Point pt = ReportTreeView.PointToClient(new Point(screenX, screenY));
			return ReportTreeView.GetNodeAt(pt) as ComponentNode;
		}
		public override void Dispose() {
			if(fContextMenu != null) {
				fContextMenu.Dispose();
				fContextMenu = null;
			}
			base.Dispose();
		}
	}
	public class ReportTreeView : XtraTreeView, IToolTipControlClient, ISupportController {
		#region inner classes
		class ReportTreeViewHandler : TreeListHandler {
			public ReportTreeViewHandler(TreeList treeList)
				: base(treeList) {
			}
			protected override TreeListHandler.TreeListControlState CreateState(TreeListState state) {
				if(state == TreeListState.NodeDragging)
					return new ReportNodeDragingState(this);
				else if(state == TreeListState.NodePressed)
					return new ReportNodePassedState(this);
				return base.CreateState(state);
			}
		}
		class ReportNodeDragingState : TreeListHandler.NodeDraggingState {
			public ReportNodeDragingState(TreeListHandler handler)
				: base(handler) {
			}
			protected override void OnEndNodesDragging(DragEventArgs drgevent) { }
		}
		class ReportNodePassedState : TreeListHandler.NodePressedState { 
			public ReportNodePassedState(TreeListHandler handler)
				: base(handler) {
			}
			public override void MouseMove(MouseEventArgs e, TreeListHitTest ht) {
				if(!(Data.DownHitTest.RowInfo.Node as ReportExplorerControllerBase.ComponentNode).IsFormattingNode)
					base.MouseMove(e, ht);
			}
		}
		#endregion
		XtraReport rootReport;
		TreeListController activeController;
		public TreeListController ActiveController {
			get { return activeController; }
			set { activeController = value; }
		}
		public ReportTreeView(IServiceProvider serviceProvider) {
			if(serviceProvider != null) {
				IDesignerHost designerHost = serviceProvider.GetService(typeof(IDesignerHost)) as IDesignerHost;
				if(designerHost != null)
					rootReport = designerHost.RootComponent as XtraReport;
			}
			this.StateImageList = ReportExplorerController.ImageCollection;
			Size = new System.Drawing.Size(Math.Max(Width, 10), Math.Max(Height, 10));
			this.OptionsSelection.EnableAppearanceFocusedCell = false;
			this.OptionsDragAndDrop.DragNodesMode = XtraTreeList.DragNodesMode.Single;
			this.OptionsBehavior.AllowIncrementalSearch = true;
			this.AllowDrop = true;
			ToolTipController.DefaultController.AddClientControl(this);
		}
		public TreeListController CreateController(IServiceProvider serviceProvider) {
			return new ReportExplorerController(serviceProvider);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				ToolTipController.DefaultController.RemoveClientControl(this);
				lock(this) {
					if(this.StateImageList != null)
						this.StateImageList = null;
				}
				if(activeController != null) {
					activeController.UnsubscribeTreeListEvents(this);
					activeController = null;
				}
			}
			base.Dispose(disposing);
		}
		protected override DevExpress.XtraTreeList.Handler.TreeListHandler CreateHandler() {
			return new ReportTreeViewHandler(this);
		}
		ToolTipControlInfo IToolTipControlClient.GetObjectInfo(System.Drawing.Point point) {
			ReportExplorerController.ComponentNode node = GetNodeAt(point.X, point.Y) as ReportExplorerController.ComponentNode;
			if(node != null)
				return ToolTipService.GetListToolTipInfo(this, point, node.Bounds, node.GetToolTip(), node);
			return null;
		}
		bool IToolTipControlClient.ShowToolTips {
			get { return XtraReport != null && XtraReport.DesignerOptions.ShowDesignerHints; }
		}
		protected virtual XtraReport XtraReport {
			get { return rootReport; }
		}
	}
	public class FormattingDataObject {
		IComponent formattingComponent;
		public IComponent FormattingComponent { get { return formattingComponent; } }
		public bool IsStyle { get { return formattingComponent is XRControlStyle; } }
		public bool IsRule { get { return formattingComponent is FormattingRule; } }
		public FormattingDataObject(IComponent formattingComponent) {
			this.formattingComponent = formattingComponent;
		}
	}
	public static class FormattingComponentAssignmentHelper {
		public static bool CanAssignStyleOnly(XRControl control) {
			return CanAssignStyle(control) && !CanAssignOddStyle(control) && !CanAssignEvenStyle(control);
		}
		static bool CanAssignStyleBase(XRControl control) {
			PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(control)[XRComponentPropertyNames.Styles];
			return propertyDescriptor != null && ((BrowsableAttribute)propertyDescriptor.Attributes[typeof(BrowsableAttribute)]).Browsable;
		}
		public static bool CanAssignStyle(XRControl control) {
			if(!CanAssignStyleBase(control))
				return false;
			PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(control.Styles)["Style"];
			return propertyDescriptor != null && ((BrowsableAttribute)propertyDescriptor.Attributes[typeof(BrowsableAttribute)]).Browsable;
		}
		public static bool CanAssignOddStyle(XRControl control) {
			if(!CanAssignStyleBase(control))
				return false;
			PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(control.Styles)["OddStyle"];
			return propertyDescriptor != null && ((BrowsableAttribute)propertyDescriptor.Attributes[typeof(BrowsableAttribute)]).Browsable;
		}
		public static bool CanAssignEvenStyle(XRControl control) {
			if(!CanAssignStyleBase(control))
				return false;
			PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(control.Styles)["EvenStyle"];
			return propertyDescriptor != null && ((BrowsableAttribute)propertyDescriptor.Attributes[typeof(BrowsableAttribute)]).Browsable;
		}
		public static bool CanAssignRule(XRControl control) {
			PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(control)[XRComponentPropertyNames.FormattingRules];
			return propertyDescriptor != null && ((BrowsableAttribute)propertyDescriptor.Attributes[typeof(BrowsableAttribute)]).Browsable;
		}  
	}
}
