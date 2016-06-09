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
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.Data;
using DevExpress.Data.Helpers;
using DevExpress.Utils.Drawing;
using DevExpress.XtraGrid.Registrator;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Layout;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils.Design;
using DevExpress.XtraBars;
namespace DevExpress.XtraGrid.Design {
	public class ViewNodeEventArgs : EventArgs {
		ViewNode node;
		public ViewNodeEventArgs(ViewNode node) {
			this.node = node;
		}
		public ViewNode Node { get { return node; } }
	}
	public delegate void ViewNodeEventHandler(object sender, ViewNodeEventArgs e);
	[ListBindable(false)]
	public class ViewNodeCollection : CollectionBase {
		public event CollectionChangeEventHandler CollectionChanged;
		public ViewNode this[int index] { get { return List[index] as ViewNode; } }
		public virtual ViewNode Add() {
			ViewNode res = new ViewNode();
			Add(res);
			return res;
		}
		public virtual int Add(ViewNode node) {
			return List.Add(node);
		}
		public virtual void Insert(int index, ViewNode node) {
			List.Insert(index, node);
		}
		public virtual void Remove(ViewNode node) {
			List.Remove(node);
		}
		public virtual int IndexOf(ViewNode node) { return List.IndexOf(node); }
		protected override void OnInsertComplete(int index, object item) {
			ViewNode node = item as ViewNode;
			node.SetOwnerNodes(this);
			RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, item));
		}
		protected override void OnRemoveComplete(int index, object item) {
			ViewNode node = item as ViewNode;
			RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, item));
			node.SetOwnerNodes(null);
		}
		protected override void OnClear() {
			for(int n = Count - 1; n >= 0; n--) { RemoveAt(n); }
		}
		protected internal virtual void RaiseCollectionChanged(CollectionChangeEventArgs e) {
			if(CollectionChanged != null) CollectionChanged(this, e);
		}
	}
	public enum ViewNodeType { GridControl, MainView, Level, EmptyLevel } 
	public class ViewNode : IDisposable {
		ViewNodeCollection nodes, ownerNodes;
		ViewNode parentNode;
		ViewNodeType nodeType;
		object link;
		string caption;
		bool fixedLevel;
		public ViewNode(string caption, ViewNodeType nodeType, object link) {
			this.fixedLevel = false;
			this.caption = caption;
			this.nodeType = nodeType;
			this.link = link;
			this.ownerNodes = null;
			this.parentNode = null;
			this.nodes = new ViewNodeCollection();
			this.nodes.CollectionChanged += new CollectionChangeEventHandler(OnNodes_CollectionChanged);
		}
		public virtual bool FixedLevel { get { return fixedLevel; } set { fixedLevel = value; } }
		public ViewNode(string caption, object link) : this(caption, ViewNodeType.Level, link) { }
		public ViewNode(string caption, ViewNodeType nodeType) : this(caption, nodeType, null) { }
		public ViewNode(string caption) : this(caption, ViewNodeType.Level) { }
		public ViewNode() : this("") {
		}
		public GridLevelNode LevelNode { get { return Link as GridLevelNode; } }
		public virtual object Link {
			get { return link; }
			set { link = value; }
		}
		public ViewNodeType NodeType { 
			get { return nodeType; }
			set { nodeType = value; }
		}
		public object Component {
			get {
				Component res = Link as Component;
				if(res != null) return res;
				GridLevelNode level = Link as GridLevelNode;
				if(level != null) return level.LevelTemplate;
				return null;
			}
		}
		public virtual string Caption { 
			get { return caption; }
			set {
				if(Caption == value) return;
				caption = value;
				OnChanged();
			}
		}
		protected virtual void OnChanged() {
			if(OwnerNodes != null) OwnerNodes.RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, this));
		}
		public virtual int Level { 
			get {
				int res = 0;
				ViewNode parent = this.ParentNode;
				while(parent !=null) {
					res ++;
					parent = parent.ParentNode;
				}
				return res;
			}
		}
		public virtual ViewNode ParentNode {
			get { return parentNode; }
		}
		public virtual ViewNodeCollection Nodes { get { return nodes; } }
		public virtual void Dispose() {
			if(nodes != null) {
				nodes.CollectionChanged -= new CollectionChangeEventHandler(OnNodes_CollectionChanged);
				nodes.Clear();
				nodes = null;
			}
		}
		protected virtual ViewNodeCollection OwnerNodes { get { return ownerNodes; } }
		protected internal void SetOwnerNodes(ViewNodeCollection ownerNodes) {
			this.ownerNodes = ownerNodes;
		}
		protected void SetParentNode(ViewNode node) {
			this.parentNode = node;
		}
		protected virtual void OnNodes_CollectionChanged(object sender, CollectionChangeEventArgs e) {
			ViewNode node = e.Element as ViewNode;
			if(e.Action != CollectionChangeAction.Refresh) {
				if(node == null) return;
				if(OwnerNodes != null) OwnerNodes.RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
				node.SetParentNode(e.Action == CollectionChangeAction.Add ? this : null);
			} else {
				if(OwnerNodes != null) OwnerNodes.RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
			}
		}
	}
	[ToolboxItem(false)]
	public class ViewTree : BaseStyleControl { 
		bool isEditingLevelName = false;
		public event EventHandler TreeLayoutChanged, ShownEditor, HiddenEditor;
		public event ViewNodeEventHandler SelectedNodeChanged;
		public event ViewNodeEventHandler ViewNameClick, LevelNameClick;
		public string MainViewName = Properties.Resources.MainViewCaption;
		ViewTreeAppearance paintAppearance;
		public event CollectionChangeEventHandler ViewChanged;
		Font textFont, textFontHot;
		TextEdit textEdit;
		ViewNodeCollection nodes;
		GridControl grid;
		int lockUpdate;
		ImageList images;
		ISelectionService selectionService;
		bool useInternalEditor;
		BarManager manager;
		public ViewTree() {
			this.selectionService = null;
			this.useInternalEditor = true;
			this.textEdit = CreateTextEdit();
			this.images = null;
			this.lockUpdate = 0;
			this.grid = null;
			this.nodes = new ViewNodeCollection();
			this.nodes.CollectionChanged += new CollectionChangeEventHandler(OnNodes_CollectionChanged);
			this.paintAppearance = new ViewTreeAppearance();
			this.textFont = WindowsFormsDesignTimeSettings.DefaultDesignTimeFont;
			this.textFontHot = new Font(textFont, FontStyle.Underline);
			manager = new BarManager();
			manager.Form = this;
			BarAndDockingController controller = new BarAndDockingController();
			controller.LookAndFeel.ParentLookAndFeel = LookAndFeel.ActiveLookAndFeel;
			manager.Controller = controller;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual ISelectionService SelectionService { 
			get { return selectionService; }
			set {
				if(SelectionService == value) return;
				if(SelectionService != null) SelectionService.SelectionChanged -= new EventHandler(OnSelectionService_SelectionChanged);
				this.selectionService = value;
				if(SelectionService != null) SelectionService.SelectionChanged += new EventHandler(OnSelectionService_SelectionChanged);
				LayoutChanged();
			}
		}
		public virtual bool UseInternalEditor { get { return useInternalEditor; } set { useInternalEditor = value; } }
		protected void OnSelectionService_SelectionChanged(object sender, EventArgs e) {
			LayoutChanged();
		}
		protected void FireGridChanged() {
			if(Grid == null) return;
			IComponentChangeService srv = Grid.InternalGetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(srv != null) srv.OnComponentChanged(Grid, null, null, null);
		}
		protected virtual TextEdit CreateTextEdit() {
			TextEdit res = new TextEdit();
			res.BorderStyle = BorderStyles.Simple;
			res.Properties.AutoHeight = false;
			res.Properties.AllowFocused = false;
			res.KeyDown += new KeyEventHandler(OnTextEdit_KeyDown);
			return res;
		}
		protected virtual TextEdit TextEdit { get { return textEdit; } }
		[Browsable(false)]
		public virtual bool IsEditingLevelName { get { return isEditingLevelName; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BorderStyles BorderStyle { get { return base.BorderStyle; } set { base.BorderStyle = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Font TextFont { get { return textFont; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Font TextFontHot { get { return textFontHot; } }
		public virtual ImageList Images {
			get { return images; }
			set { 
				if(Images == value) return;
				this.images = value;
				LayoutChanged();
			}
		}
		public virtual void BeginUpdate() {
			this.lockUpdate ++;
		}
		public virtual void EndUpdate() {
			if(-- this.lockUpdate == 0) LayoutChanged();
		}
		protected override void LayoutChanged() {
			if(this.lockUpdate != 0) return;
			UpdateNodeCaptions(Nodes);
			base.LayoutChanged();
			if(ViewInfo.IsReady) {
				if(TreeLayoutChanged != null) TreeLayoutChanged(this, EventArgs.Empty);
			}
		}
		protected override bool IsLayoutLocked { get { return base.IsLayoutLocked || this.lockUpdate != 0 ; } }
		public virtual GridControl Grid {
			get { return grid; } 
			set {
				if(Grid == value) return;
				if(Grid != null) {
					IComponentChangeService ich = Grid.InternalGetService(typeof(IComponentChangeService)) as IComponentChangeService;
					if(ich != null) ich.ComponentRename -= new ComponentRenameEventHandler(OnComponentRename);
				}
				grid = value;
				if(Grid != null) {
					IComponentChangeService ich = Grid.InternalGetService(typeof(IComponentChangeService)) as IComponentChangeService;
					if(ich != null) ich.ComponentRename += new ComponentRenameEventHandler(OnComponentRename);
				}
				PopulateTree();
			}
		}
		protected void OnComponentRename(object sender, ComponentRenameEventArgs e) {
			if(e.Component == null) return;
			if(e.Component == Grid || ViewInfo.FindNodeComponent(e.Component) != null) {
				LayoutChanged();
			}
		}
		protected internal virtual bool IsNodeSelected(ViewNode node) {
			if(SelectionService == null || node == null || node.Component == null) return false;
			return SelectionService.GetComponentSelected(node.Component);
		}
		protected virtual bool AllowGridControl { get { return true; } }
		protected virtual bool AllowEmptyLevels { get { return true; } }
		protected internal virtual bool AllowModify { 
			get { 
				if(Grid == null || Grid.MainView == null) return true;
				return !Grid.MainView.WorkAsLookup;
			} 
		}
		protected virtual bool AllowUnassignedViews { get { return false; } }
		public virtual void PopulateTree() {
			if(Nodes == null) return;
			BeginUpdate();
			try {
				Nodes.Clear();
				if(Grid == null || Grid.MainView == null) return;
				ViewNode gridNode = null;
				if(AllowGridControl) {
					gridNode = new ViewNode(Grid.Name == "" ? "GridControl" : Grid.Name, ViewNodeType.GridControl, Grid);
					Nodes.Add(gridNode);
				}
				ViewNode main = new ViewNode(MainViewName, ViewNodeType.MainView, Grid.LevelTree);
				if(gridNode == null) {
					Nodes.Add(main);
				} else {
					gridNode.Nodes.Add(main);
				}
				PopulateLevels(main);
				if(AllowModify && !Grid.IsSplitGrid) {
					ViewNode emptyNode = new ViewNode("", ViewNodeType.EmptyLevel, null);
					main.Nodes.Add(emptyNode);
				}
				if(AllowUnassignedViews) {
					foreach(BaseView view in Grid.ViewCollection) {
						if(view.LinkCount != 0) continue;
						ViewNode node = new ViewNode(Properties.Resources.UnassignedText, ViewNodeType.Level, view);
						if(gridNode == null) 
							Nodes.Add(node);
						else
							gridNode.Nodes.Add(node);
					}
				}
			}
			finally {
				EndUpdate();
			}
		}
		protected virtual void UpdateNodeCaptions(ViewNodeCollection nodes) {
			if(nodes == null) return;
			foreach(ViewNode node in nodes) {
				if(node.Link is GridControl) {
					node.Caption = Grid.Name == "" ? "GridControl" : Grid.Name;
				}
				UpdateNodeCaptions(node.Nodes);
			}
		}
		bool paintAppearanceReady = false;
		protected virtual void UpdatePaintAppearance() {
			PaintAppearance.Combine(PaintAppearance, appearances);
		}
		public ViewTreeAppearance PaintAppearance {
			get {
				CheckPaintAppearance();
				return paintAppearance;
			}
		}
		protected override void OnHandleCreated(EventArgs e) {
			this.paintAppearanceReady = false;
			base.OnHandleCreated(e);
		}
		void CheckPaintAppearance() {
			if(!paintAppearanceReady) {
				this.paintAppearanceReady = true;
				UpdatePaintAppearance();
			}
		}
		static AppearanceDefaultInfo[] appearances = new AppearanceDefaultInfo[] {
				new AppearanceDefaultInfo("Control", new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlText, HorzAlignment.Center, VertAlignment.Center)),
				new AppearanceDefaultInfo("Background", new AppearanceDefault(SystemColors.ControlText, Color.White, Color.Empty, Color.LightGray, LinearGradientMode.ForwardDiagonal, HorzAlignment.Center, VertAlignment.Center)),
				new AppearanceDefaultInfo("GridControl", new AppearanceDefault(Color.Black, Color.FromArgb(0xc9, 0xcc, 0x82), HorzAlignment.Center, VertAlignment.Center)),
				new AppearanceDefaultInfo("GridControlSelected", new AppearanceDefault(SystemColors.HighlightText, SystemColors.Highlight, HorzAlignment.Center, VertAlignment.Center)),
				new AppearanceDefaultInfo("MainLevel", new AppearanceDefault(Color.Black, Color.Azure, Color.Empty, Color.Azure, LinearGradientMode.Horizontal, HorzAlignment.Center, VertAlignment.Center)),
				new AppearanceDefaultInfo("Level", new AppearanceDefault(Color.Black, Color.LightSkyBlue, HorzAlignment.Center, VertAlignment.Center)),
				new AppearanceDefaultInfo("LevelFixed", new AppearanceDefault(Color.Black, Color.White, Color.Empty, Color.SkyBlue, LinearGradientMode.ForwardDiagonal, HorzAlignment.Center, VertAlignment.Center)),
				new AppearanceDefaultInfo("ViewNameSelected", new AppearanceDefault(Color.White, Color.Blue, HorzAlignment.Center, VertAlignment.Center)),
				new AppearanceDefaultInfo("ViewName", new AppearanceDefault(Color.Black, Color.FromArgb(0xfe, 0xef, 0xd5), HorzAlignment.Center, VertAlignment.Center)),
				new AppearanceDefaultInfo("Click", new AppearanceDefault(Color.Blue, Color.White, HorzAlignment.Center, VertAlignment.Center)),
		};
		protected virtual void PopulateLevels(ViewNode node, GridLevelNode startNode) {
			if(!startNode.HasChildren) return;
			foreach(GridLevelNode levelNode in startNode.Nodes) {
				if(!AllowEmptyLevels && !ContainsLevelTemplate(levelNode)) continue;
				ViewNode vn = new ViewNode(levelNode.RelationName, ViewNodeType.Level, levelNode);
				node.Nodes.Add(vn);
				if(levelNode.HasChildren) PopulateLevels(vn, levelNode);
			}
		}
		protected bool ContainsLevelTemplate(GridLevelNode node) {
			if(node.LevelTemplate != null) return true;
			if(!node.HasChildren) return false;
			foreach(GridLevelNode n in node.Nodes) {
				if(ContainsLevelTemplate(n)) return true;
			}
			return false;
		}
		int totalNodeCount = 0;
		public virtual void PopulateDetailTree() {
			if(DebuggingStateCheckHelper.PreventDebuggingCrashWhileDebugging(this.Grid, true)) return;
			CancelEdit();
			if(MessageBox.Show(this, Properties.Resources.PopulateDetailsQuestion, Properties.Resources.WarningCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
			Grid.LockFireChanged();
			try {
				Grid.LevelTree.Nodes.Clear();
				this.totalNodeCount = 0;
				DetailNodeInfo[] details = new MasterDetailHelper().GetDetailInfo(Grid.BindingContext, Grid.DataSource, Grid.DataMember, true);
				PopulateDetailTree(Grid.LevelTree, details);
				PopulateTree();
			}
			catch(Exception e) {
				Grid.LevelTree.Nodes.Clear();
				XtraMessageBox.Show(e.Message, Properties.Resources.ErrorCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally {
				Grid.UnlockFireChanged();
				Grid.FireChanged();
			}
		}
		protected void PopulateDetailTree(GridLevelNode node, DetailNodeInfo[] details) {
			if(details == null || details.Length == 0) return;
			if(this.totalNodeCount > 200) return;
			foreach(DetailNodeInfo detailNode in details) {
				GridLevelNode child = node.Nodes.Add(detailNode.Caption, null);
				this.totalNodeCount ++;
				PopulateDetailTree(child, detailNode.Nodes);
			}
		}
		protected virtual void PopulateLevels(ViewNode main) {
			PopulateLevels(main, Grid.LevelTree);
		}
		protected virtual void PopulateRelations(ViewNode main, IRelationList list, int level) {
			if(list == null || level > 7) return;
			int rc = list.RelationCount;
			for(int n = 0; n < rc; n++) {
				ViewNode node = new ViewNode(list.GetRelationName(0, n));
				node.FixedLevel = true;
				main.Nodes.Add(node);
				PopulateRelations(node, list.GetDetailList(0, n) as IRelationList, level + 1);
			}
		}
		protected virtual void PopulateRelations(ViewNode main, DataTable table, int level) {
			if(table == null || level > 7) return;
			foreach(DataRelation rel in table.ChildRelations) {
				if(CheckRecursive(table, rel.ChildTable)) continue;
				ViewNode node = new ViewNode(rel.RelationName);
				node.FixedLevel = true;
				main.Nodes.Add(node);
				PopulateRelations(node, rel.ChildTable, level + 1);
			}
		}
		bool CheckRecursive(DataTable table, DataTable childTable) {
			foreach(DataRelation rel in table.ParentRelations) {
				if(rel.ChildTable == childTable) return true;
				if(CheckRecursive(rel.ParentTable, childTable)) return true;
			}
			return false;
		}
		protected virtual ViewNode FindLevel(ViewNodeCollection nodes,  string levelName) {
			if(Grid == null) return null;
			if(nodes == null) nodes = Nodes;
			foreach(ViewNode node in nodes) {
				if(node.Caption == levelName && node.NodeType == ViewNodeType.Level) return node;
				ViewNode child = FindLevel(node.Nodes, levelName);
				if(child != null) return child;
			}
			return null;
		}
		public virtual ViewNodeCollection Nodes { get { return nodes; } }
		protected override void Dispose(bool disposing) {
			if(disposing) {
				this.SelectionService = null;
				if(this.textEdit != null) {
					this.textEdit.KeyDown -= new KeyEventHandler(OnTextEdit_KeyDown);
					this.textEdit.Dispose();
					this.textEdit = null;
				}
				if(this.nodes != null) {
					this.nodes.CollectionChanged -= new CollectionChangeEventHandler(OnNodes_CollectionChanged);
					this.nodes.Clear();
					this.nodes = null;
				}
				this.Grid = null;
			}
			base.Dispose(disposing);
		}
		protected virtual void OnNodes_CollectionChanged(object sender, CollectionChangeEventArgs e) {
			LayoutChanged();
		}
		protected override BaseControlPainter CreatePainter() { return new ViewTreePainter(); }
		protected override BaseStyleControlViewInfo CreateViewInfo() {
			return new ViewTreeViewInfo(this);
		}
		protected internal new ViewTreeViewInfo ViewInfo { get { return base.ViewInfo as ViewTreeViewInfo; } }
		protected virtual void OnNodeLevelNameClick(Point hitPoint, ViewNode node) {
			if(LevelNameClick != null) LevelNameClick(this, new ViewNodeEventArgs(node));
			if(!AllowModify) return;
			if(node.NodeType == ViewNodeType.GridControl) {
				if(SelectionService != null)
					SelectionService.SetSelectedComponents(new object[] { Grid }, ControlConstants.SelectionNormal);
				return;
			}
			DXPopupMenu menu = GenerateLevelNameMenu(node);
			if(menu == null) return;
			this.menuNode = node;
			ShowPopupMenuCore(hitPoint, menu);
		}
		protected virtual void OnNodeViewNameClick(Point hitPoint, ViewNode node) {
			if(ViewNameClick != null)
				ViewNameClick(this, new ViewNodeEventArgs(node));
			if(!AllowModify)
				return;
			if(SelectionService != null && node != null) {
				var components = (node.Component != null) ? 
					new object[] { node.Component } : new object[] { };
				SelectionService.SetSelectedComponents(components, ControlConstants.SelectionNormal);
			}
			if(SelectedNodeChanged != null)
				SelectedNodeChanged(this, new ViewNodeEventArgs(node));
		}
		protected virtual DXPopupMenu GenerateLevelNameMenu(ViewNode node) {
			if(node.NodeType == ViewNodeType.MainView) return null;
			if(node.FixedLevel) return null;
			DXPopupMenu menu = new DXPopupMenu();
			menu.Items.Add(new DXMenuItem(Properties.Resources.AddLevelCaption, new EventHandler(OnLevelNameMenu_AddLevelClick)));
			menu.Items.Add(new DXMenuItem(Properties.Resources.ChangeLevelNameCaption, new EventHandler(OnLevelNameMenu_ChangeNameClick)));
			menu.Items.Add(new DXMenuItem(Properties.Resources.DeleteLevelCaption, new EventHandler(OnLevelNameMenu_DeleteLevelClick)));
			return menu;
		}
		protected virtual void AddLevel(ViewNode viewNode) {
			if(DebuggingStateCheckHelper.PreventDebuggingCrashWhileDebugging(this.Grid, true)) return;
			if(viewNode.LevelNode == null) return;
			ViewNode newNode = new ViewNode(GenerateNewLevelName(), ViewNodeType.Level, null);
			newNode.Link = viewNode.LevelNode.Nodes.Add(newNode.Caption, null);;
			viewNode.Nodes.Add(newNode);
			LayoutChanged();
		}
		protected void OnLevelNameMenu_AddLevelClick(object sender, EventArgs e) {
			if(this.menuNode == null) return;
			AddLevel(this.menuNode);
		}
		protected void OnLevelNameMenu_DeleteLevelClick(object sender, EventArgs e) {
			if(this.menuNode == null) return;
			DeleteLevel(this.menuNode, true);
		}
		protected void OnLevelNameMenu_ChangeNameClick(object sender, EventArgs e) {
			if(this.menuNode == null) return;
			ShowTextEdit(this.menuNode);
		}
		protected virtual void DeleteLevel(ViewNode node, bool deleteFromTree) {
			if(DebuggingStateCheckHelper.PreventDebuggingCrashWhileDebugging(this.Grid, true)) return;
			GridLevelNode levelNode = (node != null) ? node.Link as GridLevelNode : null;
			if(levelNode == null) 
				return;
			BaseView view = levelNode.LevelTemplate;
			levelNode.LevelTemplate = null;
			if(deleteFromTree) {
				levelNode.Dispose();
				if(node.ParentNode != null)
					node.ParentNode.Nodes.Remove(node);
				if(Grid != null && Grid.DefaultView == view) {
					if(Grid.MainView != null)
						Grid.MainView.NormalView();
				}
			} 
			LayoutChanged();
			FireGridChanged();
			if(view != null)
				RaiseViewChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, view));
		}
		protected virtual void RaiseViewChanged(CollectionChangeEventArgs e) {
			if(ViewChanged != null) ViewChanged(this, e);
		}
		protected virtual void OnNodeClickClick(Point hitPoint, ViewNode node) {
			if(DebuggingStateCheckHelper.PreventDebuggingCrashWhileDebugging(this.Grid, false)) return;
			if(node.NodeType == ViewNodeType.EmptyLevel) {
				if (Grid != null && Grid.MainView != null && Grid.MainView is LayoutView) {
					MessageBox.Show(Properties.Resources.LayoutViewMasterWarning, Properties.Resources.WarningCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning); 
					return;
				}
				int index = node.ParentNode.Nodes.IndexOf(node);
				GridLevelNode levelNode = node.ParentNode.Link as GridLevelNode;
				if(levelNode == null) return;
				ViewNode newNode = new ViewNode(GenerateNewLevelName(), ViewNodeType.Level, null);
				newNode.Link = levelNode.Nodes.Insert(index, newNode.Caption, null);
				node.ParentNode.Nodes.Insert(index, newNode);
				LayoutChanged();
				return;
			}
			DXPopupMenu menu = GenerateClickMenu(node);
			this.menuNode = node;
			ShowPopupMenuCore(hitPoint, menu);
		}
		protected virtual void ShowPopupMenuCore(Point hitPoint, DXPopupMenu menu) {
			MenuManagerHelper.ShowMenu(menu, Grid.LookAndFeel, manager, this, hitPoint);
		}
		protected ViewNode menuNode = null;
		protected virtual void CreateViewItems(ViewNode node, DXPopupMenu menu) {
			if(node == null || grid == null) return;
			BaseView currentView = node.Component as BaseView;
			bool needBeginGroup = false;
			if(node.NodeType != ViewNodeType.MainView && currentView != null) {
				menu.Items.Add(new DXMenuItem(Properties.Resources.NoneCaption, OnViewMenu_NoneClick));
				needBeginGroup = true;
			}
			if(currentView != null) {
				DXSubMenuItem switchView = new DXSubMenuItem(Properties.Resources.ConvertToCaption);
				switchView.Image = new Bitmap(16, 16); 
				switchView.BeginGroup = needBeginGroup;
				needBeginGroup = false;
				menu.Items.Add(switchView);
				foreach(BaseInfoRegistrator regInfo in Grid.AvailableViews) {
					if(currentView != null && currentView.BaseInfo.ViewName == regInfo.ViewName) continue;
					DXMenuItem item = new DXMenuItem(regInfo.ViewName, OnViewMenu_ClickSwitch, ViewInfo.GetViewImage(regInfo.ViewName));
					item.Tag = regInfo;
					switchView.Items.Add(item);
				}
			}
			DXSubMenuItem newView = new DXSubMenuItem(Properties.Resources.CreateNewViewCaption);
			newView.BeginGroup = needBeginGroup;
			menu.Items.Add(newView);
			foreach(BaseInfoRegistrator regInfo in Grid.AvailableViews) {
				DXMenuItem item = new DXMenuItem(regInfo.ViewName, OnViewMenu_Click, ViewInfo.GetViewImage(regInfo.ViewName));
				item.Tag = regInfo;
				newView.Items.Add(item);
			}
			if(IsExistsNonConnectedView()) {
				foreach(BaseView view in Grid.ViewCollection) {
					if(view.LinkCount != 0) continue;
					DXMenuItem viewItem = new DXMenuItem(view.Name == "" ? view.BaseInfo.ViewName : view.Name, 
						OnViewMenu_ChangeViewClick, ViewInfo.GetViewImage(view.BaseInfo.ViewName));
					viewItem.Tag = view;
					menu.Items.Add(viewItem);
				}
			}
		}
		bool IsExistsNonConnectedView() {
			foreach(BaseView view in Grid.ViewCollection) {
				if(view.LinkCount == 0) return true;
			}
			return false;
		}
		protected void OnTextEdit_KeyDown(object sender, KeyEventArgs e) {
			if(e.KeyData == Keys.Escape) CancelEdit();
			if(e.KeyData == Keys.Enter) CloseEdit();
		}
		protected virtual void CloseEdit() {
			if(IsEditingLevelName) {
				if(menuNode != null) {
					if(menuNode.Caption != TextEdit.Text) ChangeLevelName(menuNode, TextEdit.Text);
				}
				CancelEdit();
			}
		}
		public virtual void CancelEdit() {
			if(IsEditingLevelName) {
				this.isEditingLevelName = false;
				TextEdit.Visible = false;
				if(HiddenEditor != null) HiddenEditor(this, EventArgs.Empty);
				LayoutChanged();
			}
		}
		protected virtual void ChangeLevelName(ViewNode node, string newLevelName) {
			if(DebuggingStateCheckHelper.PreventDebuggingCrashWhileDebugging(this.Grid,true)) return;
			newLevelName = newLevelName.Trim();
			if(node.Caption == newLevelName || newLevelName == "") return;
			GridLevelNode levelNode = node.Link as GridLevelNode;
			if(levelNode != null) {
				if(levelNode.Parent != null && levelNode.Parent.Nodes[newLevelName] != null && levelNode.Parent.Nodes[newLevelName] != levelNode) return;
				levelNode.RelationName = newLevelName;
			}
			node.Caption = newLevelName;
			FireGridChanged();
		}
		protected virtual void ShowTextEdit(ViewNode node) {
			if(DebuggingStateCheckHelper.PreventDebuggingCrashWhileDebugging(this.Grid,false)) return;
			if(IsEditingLevelName) CancelEdit();
			if(node.NodeType != ViewNodeType.Level) return;
			ViewNodeInfo info = ViewInfo.FindNode(node);
			if(info == null) return;
			if(UseInternalEditor) {
				Rectangle r = info.LevelNameBounds;
				r.Width = Math.Min(info.Bounds.Right, ViewInfo.ContentRect.Right - 4) - r.X;
				TextEdit.Bounds = r;
				TextEdit.Text = node.Caption;
				Controls.Add(TextEdit);
				TextEdit.Visible = true;
				TextEdit.Focus();
				TextEdit.SelectAll();
				this.isEditingLevelName = true;
				if(ShownEditor != null) ShownEditor(this, EventArgs.Empty);
			} else {
				LevelViewNameForm form = new LevelViewNameForm();
				form.Location = new Point(Control.MousePosition.X - 50, Control.MousePosition.Y - 50);
				form.teEdit.Text = menuNode.Caption;
				if(form.ShowDialog() == DialogResult.OK) {
					if(menuNode != null) {
						if(menuNode.Caption != form.teEdit.Text) ChangeLevelName(menuNode, form.teEdit.Text);
					}
				}
				form.Dispose();
			}
		}
		protected void OnViewMenu_NoneClick(object sender, EventArgs e) {
			DeleteLevel(menuNode, false);
		}
		protected void OnViewMenu_ChangeViewClick(object sender, EventArgs e) {
			if(DebuggingStateCheckHelper.PreventDebuggingCrashWhileDebugging(this.Grid,true)) return;
			DXMenuItem mi = sender as DXMenuItem;
			if(menuNode.NodeType == ViewNodeType.MainView) {
				Grid.MainView = mi.Tag as BaseView;
			} else {
				if(menuNode.LevelNode != null) menuNode.LevelNode.LevelTemplate = mi.Tag as BaseView;
			}
			FireGridChanged();
			LayoutChanged();
			BaseView view = mi.Tag as BaseView;
			if(SelectionService != null && view != null)
				SelectionService.SetSelectedComponents(new object[] { view });
		}
		protected void OnViewMenu_Click(object sender, EventArgs e) {
			DXMenuItem mi = sender as DXMenuItem;
			BaseInfoRegistrator regInfo = mi.Tag as BaseInfoRegistrator;
			ChangeView(menuNode, regInfo.ViewName, false);
		}
		protected void OnViewMenu_ClickSwitch(object sender, EventArgs e) {
			DXMenuItem mi = sender as DXMenuItem;
			BaseInfoRegistrator regInfo = mi.Tag as BaseInfoRegistrator;
			ChangeView(menuNode, regInfo.ViewName, true);
		}
		protected virtual void ChangeView(ViewNode node, string viewName, bool removeOld) {
			if(DebuggingStateCheckHelper.PreventDebuggingCrashWhileDebugging(this.Grid,true)) return;
			GridLevelNode levelNode = node.Link as GridLevelNode;
			if(levelNode == null) return;
			BaseView view = Grid.CreateView(viewName);
			Grid.ViewCollection.Add(view);
			BaseView prev = levelNode.LevelTemplate;
			Type contextType = typeof(BaseView).GetNestedType("ChangeContext", BindingFlags.NonPublic);
			IDisposable changeContext = (IDisposable)contextType.GetMethod("Enter").Invoke(null, new object[] { prev, view });
			using(changeContext) {
				MemoryStream ms = null;
				if(prev != null) {
					ms = new MemoryStream();
					prev.SaveLayoutToStream(ms, OptionsLayoutBase.FullLayout);
				}
				if(node.NodeType == ViewNodeType.MainView) {
					prev = Grid.MainView;
					Grid.MainView = view;
				}
				else {
					levelNode.LevelTemplate = view;
				}
				if(ms != null) {
					if(removeOld && prev != null) prev.Dispose();
					ms.Seek(0, SeekOrigin.Begin);
					view.RestoreLayoutFromStream(ms, OptionsLayoutBase.FullLayout);
					ms.Close();
					MethodInfo mi = view.GetType().GetMethod("DesignerMakeColumnsVisible", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance);
					if(mi != null) mi.Invoke(view, null);
					if(SelectionService != null)
						SelectionService.SetSelectedComponents(new object[] { view });
					if(prev != null) AssignViewProperties(prev, view);
				}
				if(removeOld && prev != null) prev.Dispose();
			}
			LayoutChanged();
			FireGridChanged();
			RaiseViewChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, view));
		}
		private void AssignViewProperties(BaseView prev, BaseView view) {
			ColumnView cprev = prev as ColumnView, cview = view as ColumnView;
			if(cprev != null && cview != null) {
				cview.Images = cprev.Images;
			}
		}
		protected virtual DXPopupMenu GenerateClickMenu(ViewNode node) {
			DXPopupMenu menu = new DXPopupMenu();
			CreateViewItems(node, menu);
			return menu;
		}
		protected virtual string GenerateNewLevelName() {
			string format = Properties.Resources.LevelCaption;
			int count = 1;
			for(;;) {
				string res = String.Format(format, count);
				if(FindLevel(null, res) == null) return res;
				count ++;
			}
		}
		public virtual bool CanHotTrack(ViewTreeHitInfo hitInfo) {
			if(ViewInfo.HotInfo.HitTest == ViewTreeHitTest.NodeClick) {
				return true;
			}
			if(ViewInfo.HotInfo.HitTest == ViewTreeHitTest.NodeLevelName) {
				if(!AllowModify || hitInfo.Node == null) return false;
				if((hitInfo.Node.NodeType == ViewNodeType.GridControl || hitInfo.Node.NodeType == ViewNodeType.Level) && !hitInfo.Node.FixedLevel) {
					return true;
				}
			}
			if(ViewInfo.HotInfo.HitTest == ViewTreeHitTest.NodeViewName) {
				return true;
			}
			return false;
		}
		protected override void OnLostFocus(EventArgs e) {
			base.OnLostFocus(e);
			CancelEdit();
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			if(IsEditingLevelName) return;
			ViewInfo.HotInfo = ViewInfo.CalcHitInfo(e.X, e.Y);
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			if(IsEditingLevelName) return;
			if(e.Button != MouseButtons.Left) return;
			Point p = new Point(e.X, e.Y);
			ViewInfo.HotInfo = ViewInfo.CalcHitInfo(e.X, e.Y);
			if(CanHotTrack(ViewInfo.HotInfo)) {
				if(ViewInfo.HotInfo.HitTest == ViewTreeHitTest.NodeClick) {
					OnNodeClickClick(p, ViewInfo.HotInfo.Node);
					return;
				}
				if(ViewInfo.HotInfo.HitTest == ViewTreeHitTest.NodeLevelName) {
					OnNodeLevelNameClick(p, ViewInfo.HotInfo.Node);
				}
				if(ViewInfo.HotInfo.HitTest == ViewTreeHitTest.NodeViewName) {
					OnNodeViewNameClick(p, ViewInfo.HotInfo.Node);
					return;
				}
			}
		}
		public override Size CalcBestSize() {
			ViewTreeViewInfo info = CreateViewInfo() as ViewTreeViewInfo;
			info.CalcViewInfo(null, MouseButtons.None, Point.Empty, new Rectangle(0, 0, 1000, 1000));
			Size size = info.CalcBestFit(null);
			info.Dispose();
			return size;
		}
	}
	public class ViewTreePainter : BaseControlPainter {
		protected virtual void DrawBackground(ControlGraphicsInfoArgs info) {
			ViewTreeViewInfo vi = info.ViewInfo as ViewTreeViewInfo;
			vi.OwnerControl.PaintAppearance.Background.FillRectangle(info.Cache, info.ViewInfo.ClientRect);
		}
		protected override void DrawContent(ControlGraphicsInfoArgs info) {
			DrawBackground(info);
			DrawTree(info);
		}
		protected virtual void DrawTree(ControlGraphicsInfoArgs info) {
			ViewTreeViewInfo vi = info.ViewInfo as ViewTreeViewInfo;
			info.Graphics.SetClip(vi.ContentRect);
			for(int n = vi.TopNodeIndex; n < vi.NodesInfo.Count; n ++) {
				DrawTreeNode(info, vi.NodesInfo[n]);
			}
		}
		protected virtual void DrawTreeNode(ControlGraphicsInfoArgs info, ViewNodeInfo node) {
			ViewTreeViewInfo vi = info.ViewInfo as ViewTreeViewInfo;
			if(!node.ImageBounds.IsEmpty) {
				info.Paint.DrawImage(info, vi.OwnerControl.Images, node.ImageIndex, node.ImageBounds, true);
			}
			bool isHotNode = vi.HotInfo.Node == node.Node;
			if(!node.LevelNameBounds.IsEmpty) {
				AppearanceObject appearance = vi.GetLevelNameStyle(node.Node);
				if(node.Node.NodeType == ViewNodeType.GridControl && vi.IsNodeSelected(node.Node)) appearance = vi.OwnerControl.PaintAppearance.GridControlSelected;
				DrawFillText(info, appearance, node.LevelNameBounds, node.LevelName, isHotNode && vi.OwnerControl.CanHotTrack(vi.HotInfo) && vi.HotInfo.HitTest == ViewTreeHitTest.NodeLevelName);
			}
			if(!node.ViewNameBounds.IsEmpty) {
				AppearanceObject appearance = vi.OwnerControl.PaintAppearance.ViewName;
				if(vi.IsNodeSelected(node.Node)) appearance = vi.OwnerControl.PaintAppearance.ViewNameSelected;
				DrawFillText(info, appearance, node.ViewNameBounds, node.ViewNameCaption, isHotNode && vi.OwnerControl.CanHotTrack(vi.HotInfo) && vi.HotInfo.HitTest == ViewTreeHitTest.NodeViewName);
			}
			if(!node.ClickBounds.IsEmpty) {
				DrawText(info, vi.OwnerControl.PaintAppearance.Click, node.ClickCaption, node.ClickBounds, isHotNode && vi.OwnerControl.CanHotTrack(vi.HotInfo) && vi.HotInfo.HitTest == ViewTreeHitTest.NodeClick);
			}
		}
		protected virtual void DrawFillText(ControlGraphicsInfoArgs info, AppearanceObject appearance, Rectangle bounds, string text, bool isHot) {
			Rectangle r = bounds;
			info.Paint.DrawRectangle(info.Graphics, Pens.Gray, r);
			r.Inflate(-1, -1);
			appearance.FillRectangle(info.Cache, r);
			DrawText(info, appearance, text, r, isHot);
		}
		protected virtual void DrawText(ControlGraphicsInfoArgs info, AppearanceObject appearance, string text, Rectangle bounds, bool isHot) {
			ViewTreeViewInfo vi = info.ViewInfo as ViewTreeViewInfo;
			info.Cache.DrawString(text, vi.GetTextFont(isHot), appearance.GetForeBrush(info.Cache), bounds, appearance.GetStringFormat());
		}
	}
	public enum ViewTreeHitTest { None, Node, NodeLevelName, NodeViewName, NodeClick};
	public class ViewTreeHitInfo {
		static ViewTreeHitInfo empty = new ViewTreeHitInfo();
		public static ViewTreeHitInfo Empty { get { return empty; } }
		protected internal ViewTreeHitTest hitTest;
		protected internal Point hitPoint;
		protected internal ViewNode node;
		public ViewTreeHitInfo() {
			Clear();
		}
		protected virtual void Clear() {
			this.node = null;
			this.hitPoint = new Point(-10000, -10000);
			this.hitTest = ViewTreeHitTest.None;
		}
		public virtual bool IsEquals(ViewTreeHitInfo info) {
			if(info == null) return false;
			return this.Node == info.Node && this.HitTest == info.HitTest;
		}
		public bool IsValid { get { return HitPoint.X != -10000; } }
		public ViewTreeHitTest HitTest { get { return hitTest; } }
		public Point HitPoint { get { return hitPoint; } }
		public ViewNode Node { get { return node; } }
	}
	public class ViewTreeViewInfo : BaseStyleControlViewInfo {
		ViewTreeHitInfo hotInfo;
		protected const int LevelIndent = 15, Indent = 4;
		int topNodeIndex;
		int nodeHeight;
		ViewNodeInfoCollection nodesInfo;
		public ViewTreeViewInfo(BaseStyleControl owner) : base(owner) {
			this.hotInfo = null;
			this.topNodeIndex = 0;
			this.nodesInfo = new ViewNodeInfoCollection();
		}
		public virtual ViewNodeInfo FindNode(ViewNode node) {
			foreach(ViewNodeInfo info in NodesInfo) {
				if(info.Node == node) return info;
			}
			return null;
		}
		public virtual ViewNodeInfo FindNodeComponent(object componentLink) {
			foreach(ViewNodeInfo info in NodesInfo) {
				if(info.Node.Component == componentLink) return info;
			}
			return null;
		}
		public override Size CalcBestFit(Graphics g) { 
			Size size = CalcContentSize(g);
			size = BorderPainter.CalcBoundsByClientRectangle(new BorderObjectInfoArgs(null, new Rectangle(Point.Empty, size), null)).Size;
			return size;
		}
		protected virtual Size CalcContentSize(Graphics g) {
			Size size = new Size(10, 10);
			foreach(ViewNodeInfo info in NodesInfo) {
				size.Width = Math.Max(size.Width, info.Bounds.Right + Indent);
				size.Height = Math.Max(size.Height, info.Bounds.Bottom + Indent);
			}
			return size;
		}
		public virtual ViewTreeHitInfo HotInfo {
			get {
				if(hotInfo == null) hotInfo = new ViewTreeHitInfo();
				return hotInfo;
			}
			set {
				if(value == null) value = ViewTreeHitInfo.Empty;
				if(HotInfo.IsEquals(value)) {
					hotInfo = value;
					return;
				}
				hotInfo = value;
				OwnerControl.Invalidate();
			}
		}
		public int TopNodeIndex { get { return topNodeIndex; } set { topNodeIndex = value; } }
		public override void Reset() {
			base.Reset();
			this.nodeHeight = 0;
		}
		protected virtual int NodeHeight { get { return nodeHeight; } }
		public virtual AppearanceObject GetLevelNameStyle(ViewNode node) {
			switch(node.NodeType) {
				case ViewNodeType.GridControl : return OwnerControl.PaintAppearance.GridControl;
				case ViewNodeType.MainView : return OwnerControl.PaintAppearance.MainLevel;
			}
			if(node.FixedLevel) return OwnerControl.PaintAppearance.LevelFixed;
			return OwnerControl.PaintAppearance.Level;
		}
		public virtual bool IsNodeSelected(ViewNode node) {
			if(node.Link == null) return false;
			return OwnerControl.IsNodeSelected(node);
		}
		public ViewTreeHitInfo CalcHitInfo(int x, int y) { return CalcHitInfo(new Point(x, y)); }
		public virtual ViewTreeHitInfo CalcHitInfo(Point hit) {
			ViewTreeHitInfo hitInfo = new ViewTreeHitInfo();
			hitInfo.hitPoint = hit;
			foreach(ViewNodeInfo info in NodesInfo) {
				if(!info.Bounds.Contains(hit)) continue;
				hitInfo.hitTest = ViewTreeHitTest.Node;
				hitInfo.node = info.Node;
				if(info.LevelNameBounds.Contains(hit)) {
					hitInfo.hitTest = ViewTreeHitTest.NodeLevelName;
					break;
				}
				if(info.ViewNameBounds.Contains(hit)) {
					hitInfo.hitTest = ViewTreeHitTest.NodeViewName;
					break;
				}
				if(info.ClickBounds.Contains(hit)) {
					hitInfo.hitTest = ViewTreeHitTest.NodeClick;
					break;
				}
				break;
			}
			return hitInfo;
		}
		protected override void CalcRects() {
			UpdateNodes();
			base.CalcRects();
		}
		protected override void CalcConstants() {
			base.CalcConstants();
			this.nodeHeight = Math.Max(GetImageSize(0).Height, OwnerControl.TextFont.Height) + 5;
		}
		protected override void CalcContentRect(Rectangle bounds) {
			base.CalcContentRect(bounds);
			bounds.Inflate(-2, -2);
			CalcNodesRect(bounds);
		}
		protected virtual void CalcNodesRect(Rectangle bounds) {
			if(TopNodeIndex >= NodesInfo.Count) TopNodeIndex = Math.Max(NodesInfo.Count - 1, 0);
			int y = bounds.Top;
			for(int n = TopNodeIndex; n < NodesInfo.Count; n++) {
				CalcNodeInfo(bounds, NodesInfo[n], ref y);
				y += Indent;
			}
		}
		public virtual Size GetImageSize(int index) {
			if(OwnerControl.Images == null || index < 0 || index >= OwnerControl.Images.Images.Count) return Size.Empty;
			return OwnerControl.Images.ImageSize;
		}
		protected virtual void CalcNodeInfo(Rectangle bounds, ViewNodeInfo info, ref int top) {
			int startX = bounds.X + info.Level * LevelIndent;
			Rectangle nodeBounds = new Rectangle(startX, top, 0, NodeHeight);
			nodeBounds.Width = bounds.Right - nodeBounds.Left;
			info.ImageBounds = Rectangle.Empty;
			if(info.LevelName == "" && info.Node.NodeType != ViewNodeType.Level) {
				info.LevelNameBounds = Rectangle.Empty;
			} else {
				Size imageSize = GetImageSize(info.ImageIndex);
				if(!imageSize.IsEmpty) {
					info.ImageBounds = new Rectangle(nodeBounds.Location.X, nodeBounds.Location.Y + (NodeHeight - imageSize.Height) / 2, imageSize.Width, imageSize.Height);
					nodeBounds.X = info.ImageBounds.Right + 3;
				}
				info.LevelNameBounds = new Rectangle(nodeBounds.Location, new Size(CalcTextWidth(info.LevelName, false), NodeHeight));
				nodeBounds.X = info.LevelNameBounds.Right;
			}
			if(info.ViewNameCaption == "") {
				info.ViewNameBounds = Rectangle.Empty;
			} else {
				info.ViewNameBounds = new Rectangle(nodeBounds.Location, new Size(CalcTextWidth(info.ViewNameCaption, false), NodeHeight));
				if(!info.LevelNameBounds.IsEmpty) info.ViewNameBounds.Offset(-1, 0);
				nodeBounds.X = info.ViewNameBounds.Right;
			}
			if(info.ClickCaption == "" || !OwnerControl.AllowModify) {
				info.ClickBounds = Rectangle.Empty;
			} else {
				info.ClickBounds = new Rectangle(nodeBounds.Location, new Size(CalcTextWidth(info.ClickCaption, false), NodeHeight));
				nodeBounds.X = info.ClickBounds.Right;
			}
			info.Bounds = new Rectangle(startX, top, nodeBounds.X - startX, NodeHeight);
			top += NodeHeight;
		}
		public virtual Font GetTextFont(bool isHot) {
			return isHot ? OwnerControl.TextFontHot : OwnerControl.TextFont;
		}
		protected int CalcTextWidth(string text, bool isHot) {
			int width = GInfo.Cache.Paint.CalcTextSize(GInfo.Graphics, text, GetTextFont(isHot), OwnerControl.PaintAppearance.Level.GetStringFormat(), 0).ToSize().Width + 6;
			return width;
		}
		protected virtual void UpdateNodes() {
			NodesInfo.Clear();
			CreateNodes(OwnerControl.Nodes);
		}
		static string[] viewNames = new string[] {"CardView", "GridView", "BandedGridView", "AdvBandedGridView", "LayoutView", "WinExplorerView", "TileView"};
		public virtual int CalcImageIndex(string viewName) {
			int index = Array.IndexOf(viewNames, viewName);
			if(index < 0) {
				if(OwnerControl.Images == null) return -1;
				index = OwnerControl.Images.Images.Count - 1;
			} else {
				index += 1;
			}
			return index;
		}
		public Image GetViewImage(string viewName) {
			int index = CalcImageIndex(viewName);
			if(index != -1) return OwnerControl.Images.Images[index];
			return null;
		}
		protected virtual void CreateNodes(ViewNodeCollection nodes) {
			foreach(ViewNode node in nodes) {
				ViewNodeInfo info = new ViewNodeInfo(this, node);
				NodesInfo.Add(info);
				CreateNodes(node.Nodes);
			}
		}
		public virtual ViewNodeInfoCollection NodesInfo { get { return nodesInfo; } }
		public new ViewTree OwnerControl { get { return base.OwnerControl as ViewTree; } }
	}
	public class ViewNodeInfoCollection : CollectionBase {
		public ViewNodeInfo this[int index] { get { return List[index] as ViewNodeInfo; } }
		public int Add(ViewNodeInfo info) { return List.Add(info); }
	}
	public class ViewNodeInfo {
		ViewNode node;
		ViewTreeViewInfo viewInfo;
		int level, imageIndex;
		string clickCaption, viewNameCaption;
		public Rectangle Bounds, LevelNameBounds, ViewNameBounds, ClickBounds, ImageBounds;
		public ViewNodeInfo(ViewTreeViewInfo viewInfo, ViewNode node) {
			this.viewInfo = viewInfo;
			this.node = node;
			this.imageIndex = -1;
			this.ImageBounds = this.Bounds = this.LevelNameBounds = this.ViewNameBounds = this.ClickBounds = Rectangle.Empty;
			this.clickCaption = this.viewNameCaption = "";
			UpdateInfo();
		}
		public ViewTreeViewInfo ViewInfo { get { return viewInfo; } }
		public virtual void UpdateInfo() {
			this.level = Node.Level;
			this.clickCaption = CalcClickCaption();
			this.viewNameCaption = CalcViewNameCaption();
			this.imageIndex = CalcImageIndex();
		}
		public int ImageIndex { get { return imageIndex; } }
		static string[] viewNames = new string[] {"CardView", "GridView", "BandedGridView", "AdvBandedGridView"};
		protected virtual int CalcImageIndex() {
			BaseView view = Node.Component as BaseView;
			if(view == null) {
				if(Node.LevelNode != null) return 0;
				return -1;
			}
			return ViewInfo.CalcImageIndex(view.BaseInfo == null ? view.GetType().Name : view.BaseInfo.ViewName);
		}
		protected virtual string CalcViewNameCaption() {
			if(Node.Link == null) return "";
			BaseView view = Node.Component as BaseView;
			if(view == null) {
				return "";
			}
			return view.Name == "" ? view.BaseInfo.ViewName : view.Name;
		}
		protected virtual string CalcClickCaption() {
			switch(Node.NodeType) {
				case ViewNodeType.GridControl : return "";
				case ViewNodeType.MainView : return Properties.Resources.ChangeViewText;
				case ViewNodeType.EmptyLevel : return Properties.Resources.CreateNewLevelText;
				case ViewNodeType.Level : return Node.Link == null ? Properties.Resources.CreateViewText : Properties.Resources.ChangeViewText;
			}
			return "";
		}
		public string ViewNameCaption { get { return viewNameCaption; } }
		public string ClickCaption { get { return clickCaption; } }
		public string LevelName { get { return Node.Caption; } }
		public int Level { get { return level; } }
		public virtual ViewNode Node { get { return node; } }
	}
	public class ViewTreeAppearance : BaseAppearanceCollection {
		AppearanceObject control, background, gridControl, gridControlSelected, mainLevel, level, levelFixed, viewNameSelected, viewName, click;
		protected override void CreateAppearances() {
			this.control = CreateAppearance("Control"); 
			this.background = CreateAppearance("Background"); 
			this.gridControl = CreateAppearance("GridControl"); 
			this.gridControlSelected = CreateAppearance("GridControlSelected"); 
			this.mainLevel = CreateAppearance("MainLevel"); 
			this.level = CreateAppearance("Level"); 
			this.levelFixed = CreateAppearance("LevelFixed"); 
			this.viewNameSelected = CreateAppearance("ViewNameSelected"); 
			this.viewName = CreateAppearance("ViewName"); 
			this.click = CreateAppearance("Click"); 
		}
		public AppearanceObject Control { get { return control; } } 
		public AppearanceObject Background { get { return background; } } 
		public AppearanceObject GridControl { get { return gridControl; } } 
		public AppearanceObject GridControlSelected { get { return gridControlSelected; } } 
		public AppearanceObject MainLevel { get { return mainLevel; } } 
		public AppearanceObject Level { get { return level; } } 
		public AppearanceObject LevelFixed { get { return levelFixed; } } 
		public AppearanceObject ViewNameSelected { get { return viewNameSelected; } } 
		public AppearanceObject ViewName { get { return viewName; } } 
		public AppearanceObject Click { get { return click; } } 
	}
	[ToolboxItem(false)]
	public class PopupLevelEdit : PopupContainerEdit {
		PopupContainerControl levelContainer;
		PopupViewTree levelTree;
		GridControl grid = null;
		public PopupLevelEdit() {
			this.levelContainer = new PopupContainerControl();
			this.levelTree = new PopupViewTree(this);
			this.levelContainer.Controls.Add(LevelTree);
			this.Properties.PopupControl = levelContainer;
			this.Properties.PopupSizeable = false;
			this.Properties.QueryResultValue += new QueryResultValueEventHandler(OnQuery_ResultValue);
			this.Properties.QueryDisplayText += new QueryDisplayTextEventHandler(OnQuery_DisplayText);
		}
		protected override void Dispose(bool disposing) {
			this.Properties.QueryResultValue -= new QueryResultValueEventHandler(OnQuery_ResultValue);
			this.Properties.QueryDisplayText -= new QueryDisplayTextEventHandler(OnQuery_DisplayText);
			if(this.levelContainer != null) {
				this.levelContainer.Dispose();
			}
			this.levelContainer = null;
			base.Dispose(disposing);			
		}
		protected override bool CanShowPopup { 
			get { 
				if(this.levelContainer == null || LevelTree == null || Grid == null) return false;
				return base.CanShowPopup; 
			} 
		} 
		protected override void DoShowPopup() {
			if(this.levelContainer == null || LevelTree == null || Grid == null) return;
			LevelTree.PopulateTree();
			Size size = LevelTree.CalcBestSize();
			size.Width = Math.Max(size.Width, Width);
			this.levelContainer.ClientSize = size;
			base.DoShowPopup();
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BaseView View {
			get { return EditValue as BaseView; }
			set { EditValue = value; }
		}
		[DefaultValue(null)]
		public GridControl Grid { 
			get { return grid; }
			set {
				if(Grid == value || LevelTree == null) return;
				grid =  value;
				LevelTree.Grid = grid;
			}
		}
		protected internal void SetViewCore(BaseView view) {
			LockEditValueChanged();
			try {
				View = view;
			} finally {
				UnLockEditValueChanged();
			}
		}
		void OnQuery_ResultValue(object sender, QueryResultValueEventArgs e) {
			e.Value = EditValue;
		}
		void OnQuery_DisplayText(object sender, QueryDisplayTextEventArgs e) {
			BaseView view = e.EditValue as BaseView;
			if(view == null) {
				e.DisplayText = DevExpress.XtraGrid.Design.Properties.Resources.NoneCaption;
				return;
			}
			if(Grid == null) return;
			if(Grid.MainView == view) {
				e.DisplayText = view.Name;
				return;
			}
			e.DisplayText = string.Format("{0}: {1}", view.LinkCount == 0 ? DevExpress.XtraGrid.Design.Properties.Resources.UnassignedCaption : view.LevelName, view.Name);
		}
		protected internal PopupContainerControl LevelContainer { get { return levelContainer; } }
		protected PopupViewTree LevelTree { get { return levelTree; } }
		public class PopupViewTree : ViewTree {
			PopupLevelEdit levelEdit;
			public PopupViewTree(PopupLevelEdit levelEdit) {
				this.levelEdit = levelEdit;
				Dock  = DockStyle.Fill;
			}
			protected override void OnNodeViewNameClick(Point hitPoint, ViewNode node) {
				if(node.Component == null) return;
				LevelEdit.EditValue = node.Component;
				LevelEdit.ClosePopup(PopupCloseMode.Normal);
			}
			protected internal override bool IsNodeSelected(ViewNode node) {
				return (LevelEdit.View != null && node.Component == LevelEdit.View);
			}
			protected PopupLevelEdit LevelEdit { get { return levelEdit; } }
			protected override bool AllowGridControl { get { return false; } }
			protected internal override bool AllowModify { get { return false; } }
			protected override bool AllowEmptyLevels { get { return false; } }
			protected override bool AllowUnassignedViews { get { return true; } }
		}
	}
	[ToolboxItem(false)]
	public class DesignViewTree : ViewTree {
		SkinMenuManager menuManagerCore;
		public SkinMenuManager MenuManager {
			get {
				if(menuManagerCore == null)
					menuManagerCore = new SkinMenuManager(LookAndFeel.ActiveLookAndFeel);
				return menuManagerCore;
			}
		}
		protected override void UpdatePaintAppearance() {
			PaintAppearance.Combine(PaintAppearance, GetDefaultAppearances());
		}
		UserLookAndFeel lf;
		AppearanceDefaultInfo[] GetDefaultAppearances() {
			XtraForm form = FindForm() as XtraForm;
			lf = form != null ? form.LookAndFeel.ActiveLookAndFeel : LookAndFeel;
			return new AppearanceDefaultInfo[] {
				new AppearanceDefaultInfo("Control", new AppearanceDefault(GetSystemColor(SystemColors.ControlText), GetSystemColor(SystemColors.ControlText), HorzAlignment.Center, VertAlignment.Center)),
				new AppearanceDefaultInfo("Background", new AppearanceDefault(GetSystemColor(SystemColors.ControlText),  GetSystemColor(SystemColors.Control), HorzAlignment.Center, VertAlignment.Center)),
				new AppearanceDefaultInfo("GridControl", new AppearanceDefault(Color.FromArgb(51, 51, 51), Color.FromArgb(245, 245, 245), HorzAlignment.Center, VertAlignment.Center)),
				new AppearanceDefaultInfo("GridControlSelected", new AppearanceDefault(GetSystemColor(SystemColors.HighlightText),  GetSystemColor(SystemColors.Highlight), HorzAlignment.Center, VertAlignment.Center)),
				new AppearanceDefaultInfo("MainLevel", new AppearanceDefault(Color.FromArgb(51, 51, 51), Color.White, HorzAlignment.Center, VertAlignment.Center)),
				new AppearanceDefaultInfo("Level", new AppearanceDefault(Color.White, Color.FromArgb(138, 138, 138), HorzAlignment.Center, VertAlignment.Center)),
				new AppearanceDefaultInfo("LevelFixed", new AppearanceDefault(Color.FromArgb(51, 51, 51), Color.White, HorzAlignment.Center, VertAlignment.Center)),
				new AppearanceDefaultInfo("ViewNameSelected", new AppearanceDefault(GetSystemColor(SystemColors.HighlightText),  GetSystemColor(SystemColors.Highlight), HorzAlignment.Center, VertAlignment.Center)),
				new AppearanceDefaultInfo("ViewName", new AppearanceDefault(Color.FromArgb(51, 51, 51), Color.FromArgb(245, 245, 245), HorzAlignment.Center, VertAlignment.Center)),
				new AppearanceDefaultInfo("Click", new AppearanceDefault(GetHyperLinkTextColor(), Color.White, HorzAlignment.Center, VertAlignment.Center)),
			};
		}
		Color GetHyperLinkTextColor() {
			Skin editorSkin = EditorsSkins.GetSkin(lf);
			Color defaultColor  = GetSystemColor(SystemColors.Highlight);
			if(editorSkin != null)
				return editorSkin.Colors.GetColor("HyperLinkTextColor", defaultColor);
			return defaultColor;
		}
		Color GetSystemColor(Color color) {
			return LookAndFeelHelper.GetSystemColor(lf, color);
		}
		protected override void ShowPopupMenuCore(Point hitPoint, DXPopupMenu menu) {
			MenuManagerHelper.GetMenuManager(LookAndFeel.ActiveLookAndFeel, this.Parent).ShowPopupMenu(menu, this, hitPoint);
		}
		protected override BaseControlPainter CreatePainter() { return new DesignViewTreePainter(); }
		protected override BaseStyleControlViewInfo CreateViewInfo() {
			return new DesignViewTreeViewInfo(this);
		}
	}
	public class DesignViewTreePainter : ViewTreePainter {
		protected override void DrawFillText(ControlGraphicsInfoArgs info, AppearanceObject appearance, Rectangle bounds, string text, bool isHot) {
			appearance.FillRectangle(info.Cache, bounds);
			DrawText(info, appearance, text, bounds, isHot);
		}
	}
	public class DesignViewTreeViewInfo : ViewTreeViewInfo {
		public DesignViewTreeViewInfo(BaseStyleControl owner) : base(owner) { }
		protected override void CalcNodeInfo(Rectangle bounds, ViewNodeInfo info, ref int top) {
			int startX = bounds.X + info.Level * LevelIndent;
			Rectangle nodeBounds = new Rectangle(startX, top, 0, NodeHeight);
			nodeBounds.Width = bounds.Right - nodeBounds.Left;
			info.ImageBounds = Rectangle.Empty;
			if(info.LevelName == "" && info.Node.NodeType != ViewNodeType.Level) {
				info.LevelNameBounds = Rectangle.Empty;
			}
			else {
				Size imageSize = GetImageSize(info.ImageIndex);
				if(!imageSize.IsEmpty) {
					info.ImageBounds = new Rectangle(nodeBounds.Location.X, nodeBounds.Location.Y + (NodeHeight - imageSize.Height) / 2, imageSize.Width, imageSize.Height);
					nodeBounds.X = info.ImageBounds.Right + 3;
				}
				info.LevelNameBounds = new Rectangle(nodeBounds.Location, new Size(CalcTextWidth(info.LevelName, false), NodeHeight));
				nodeBounds.X = info.LevelNameBounds.Right + 5;
			}
			if(info.ViewNameCaption == "") {
				info.ViewNameBounds = Rectangle.Empty;
			}
			else {
				info.ViewNameBounds = new Rectangle(nodeBounds.Location, new Size(CalcTextWidth(info.ViewNameCaption, false), NodeHeight));
				if(!info.LevelNameBounds.IsEmpty) info.ViewNameBounds.Offset(-1, 0);
				nodeBounds.X = info.ViewNameBounds.Right;
			}
			if(info.ClickCaption == "" || !OwnerControl.AllowModify) {
				info.ClickBounds = Rectangle.Empty;
			}
			else {
				info.ClickBounds = new Rectangle(nodeBounds.Location, new Size(CalcTextWidth(info.ClickCaption, false), NodeHeight));
				nodeBounds.X = info.ClickBounds.Right;
			}
			info.Bounds = new Rectangle(startX, top, nodeBounds.X - startX, NodeHeight);
			top += NodeHeight;
		}
	}
}
