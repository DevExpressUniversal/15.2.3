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
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Windows.Forms.ComponentModel;
using System.Collections;
using System.Reflection;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Design;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Columns;
using DevExpress.Utils.Design;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Card;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Blending;
using DevExpress.XtraGrid.Views.Layout;
using DevExpress.XtraGrid.Views.Layout.ViewInfo;
using DevExpress.Utils.Menu;
using DevExpress.Utils.About;
using System.Windows.Forms.Design.Behavior;
using DevExpress.XtraGrid.Views.BandedGrid.ViewInfo;
namespace DevExpress.XtraGrid.Design {
	public class BandedGridViewDesignTimeDesigner : ColumnViewDesigner {
		public override ICollection AssociatedComponents {
			get {
				BandedGridView view = Component as BandedGridView;
				if(view == null) return base.AssociatedComponents;
				ArrayList controls = new ArrayList();
				foreach(GridBand band in view.Bands) {
					AddBand(controls, band);
				}
				AddBase(controls);
				return controls;
			}
		}
		void AddBand(ArrayList controls, GridBand band) {
			if(band == null) return;
			controls.Add(band);
			foreach(GridBand b in band.Children) {
				AddBand(controls, b);
			}
		}
		new void AddBase(ArrayList controls) {
			ICollection coll = base.AssociatedComponents;
			foreach(object obj in coll) {
				if(controls.Contains(obj)) continue;
				controls.Add(obj);
			}
		}
	}
	public class GridControlDesigner : BaseControlDesigner {
		DesignerVerbCollection defaultVerbs;
		DesignerVerbCollection layoutViewVerbs;
		LevelSelector levelSelector;
		bool levelDesignerVisible;
		frmDesigner editor;
#if DXWhidbey
		protected override bool AllowHookDebugMode { get { return true; } }
#endif
		public override GlyphCollection GetGlyphs(GlyphSelectionType selectionType) {
			var res = base.GetGlyphs(selectionType);
			object selectedObject = null;
			ISelectionService service = GetService(typeof(ISelectionService)) as ISelectionService;
			if(service != null) selectedObject = service.PrimarySelection;
			AddColumnGlyph(res, selectedObject);
			AddBandGlyph(res, selectedObject);
			AddGridControlGlyph(res, selectedObject);
			return res;
		}
		void AddGridControlGlyph(GlyphCollection res, object selectedObject) {
			GridColumn column = selectedObject as GridColumn;
			GridBand band = selectedObject as GridBand;
			GridControl control = selectedObject as GridControl;
			if(band != null && band.View != null) control = band.View.GridControl;
			if(column != null && column.View != null) control = column.View.GridControl;
			if(control != null && control == Grid) {
				res.Add(new GridControlGlyph(this, Grid, (BehaviorService)GetService(typeof(BehaviorService))));
			}
		}
		void AddColumnGlyph(GlyphCollection res, object selectedObject) {
			GridColumn column = selectedObject as GridColumn;
			if(column != null && column.View.GridControl == Grid) {
				GridViewInfo viewInfo = column.View.GetViewInfo() as GridViewInfo;
				if(viewInfo == null) return;
				DevExpress.XtraGrid.Drawing.GridColumnInfoArgs ci = viewInfo.ColumnsInfo[column];
				if(ci == null) {
					ci = viewInfo.GroupPanel.Rows.GetColumnInfo(column);
					if(ci == null) return;
				}
				ControlDesignerActionListGlyphHelper.CreateDesignerGlyphWrapper(res, column, column.View.GridControl, ci.Bounds);
			}
		}
		void AddBandGlyph(GlyphCollection res, object selectedObject) {
			GridBand band = selectedObject as GridBand;
			if(band != null && band.View.GridControl == Grid) {
				BandedGridViewInfo viewInfo = band.View.GetViewInfo() as BandedGridViewInfo;
				if(viewInfo == null) return;
				DevExpress.XtraGrid.Drawing.GridBandInfoArgs ci = viewInfo.BandsInfo[band];
				if(ci == null) return;
				ControlDesignerActionListGlyphHelper.CreateDesignerGlyphWrapper(res, band, band.View.GridControl, ci.Bounds);
			}
		}
		protected override bool AllowMouseActions { get { return false; } }
		protected override bool AllowInheritanceWrapper { get { return true; } }
		public GridControlDesigner() {
			this.levelDesignerVisible = true;
			PropertyStore ps = new PropertyStore(frmDesigner.GridSettings);
			ps.Restore();
			this.levelDesignerVisible = ps.RestoreBoolProperty ("ShowLevelDesigner", this.levelDesignerVisible);
			this.levelSelector = new LevelSelector();
			this.levelSelector.Visible = this.levelDesignerVisible;
			this.levelSelector.Name = "LevelSelector";
			this.levelSelector.Tree.UseInternalEditor = false;
			this.levelSelector.btDesigner.Click += new EventHandler(OnDesignerClick);
			this.levelSelector.SizeChanged += new EventHandler(OnSelector_SizeChanged);
			this.levelSelector.Tree.ShownEditor += new EventHandler(OnSelector_ShownEditor);
			this.levelSelector.Tree.HiddenEditor += new EventHandler(OnSelector_HiddenEditor);
			this.levelSelector.Tree.SelectedNodeChanged += new ViewNodeEventHandler(OnSelected_SelectedChanged);
			defaultVerbs =	new DesignerVerbCollection(
								new DesignerVerb[] {
									   new DesignerVerb(Properties.Resources.AboutVerbName, new EventHandler(OnAboutClick)),
									   new DesignerVerb(Properties.Resources.RunDesignerVerbName, new EventHandler(OnDesignerClick))
#if !DXWhidbey
									   ,new DesignerVerb(Properties.Resources.ShowLevelDesignerVerbName, new EventHandler(OnSwitchLevelDesigner))
#endif
								   }
			);
			layoutViewVerbs =	new DesignerVerbCollection(
								new DesignerVerb[] {
									   new DesignerVerb(Properties.Resources.AboutVerbName, new EventHandler(OnAboutClick)),
									   new DesignerVerb(Properties.Resources.RunDesignerVerbName, new EventHandler(OnDesignerClick)),
									   new DesignerVerb(Properties.Resources.CustomizeLayoutVerbName, new EventHandler(OnLayoutDesignerClick))
#if !DXWhidbey
									   ,new DesignerVerb(Properties.Resources.ShowLevelDesignerVerbName, new EventHandler(OnSwitchLevelDesigner))
#endif
								   }
			);
			editor = null;
			UpdateLevelDesignerVerb();
		}
		protected override bool AllowEditInherited { get { return false; } }
		public virtual bool LevelDesignerVisible {
			get { return levelDesignerVisible; }
			set {
				if(LevelDesignerVisible == value) return;
				levelDesignerVisible = value;
				UpdateLevelDesignerVisibility();
				UpdateLevelDesignerVerb();
				UpdateDataSourceGlyph(value);
			}
		}
		public override SelectionRules SelectionRules {
			get {
				if(Grid.IsSplitGrid) return SelectionRules.Locked;
				return base.SelectionRules;
			}
		}
		protected internal void UpdateLevelDesignerVisibility() {
			if(levelSelector != null)
				levelSelector.Visible = LevelDesignerVisible && AllowDesigner && !expired;
		}
		protected internal void UpdateDataSourceGlyph(bool isLevelDesignerVisible) {
			DevExpress.Design.DataAccess.UI.DataSourceGlyph.Detach(Grid);
			if(AllowDesigner && !expired && columnsCount == 0) {
				if(Grid.MainView is GridView || Grid.MainView is Views.WinExplorer.WinExplorerView) {
					if(isLevelDesignerVisible)
						DevExpress.Design.DataAccess.UI.DataSourceGlyph.Attach(Grid, ContentAlignment.BottomLeft, false, false);
					else
						DevExpress.Design.DataAccess.UI.DataSourceGlyph.Attach(Grid);
				}
			}
		}
#if !DXWhidbey
		protected void OnSwitchLevelDesigner(object sender, EventArgs e) {
			LevelDesignerVisible = !LevelDesignerVisible;
			UpdateLevelDesignerVerb();
		}
		protected void UpdateLevelDesignerVerb() {
			this.verbs[2].Checked = LevelDesignerVisible;
		}
#else
		protected override void OnDebuggingStateChanged() {
			if(levelSelector != null) levelSelector.Enabled = !DebuggingState;
			base.OnDebuggingStateChanged();
		}
		protected void UpdateLevelDesignerVerb() { }
#endif
		protected void OnSelected_SelectedChanged(object sender, ViewNodeEventArgs e) {
		}
		protected virtual void SwitchVisibleView(BaseView view) {
			if(DebuggingStateCheckHelper.PreventDebuggingCrashWhileDebugging(this.Grid,false)) return;
			if(view == null) return;
			string methodName = "ZoomView";
			if(view == Grid.MainView) {
				view = null;
				methodName = "NormalView";
			}
			MethodInfo mi = typeof(GridControl).GetMethod(methodName, BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.NonPublic);
			mi.Invoke(Grid, new Object[] { view });
		}
		protected void OnSelector_ShownEditor(object sender, EventArgs e) {
			UnhookChildControls(Selector);
		}
		protected void OnSelector_HiddenEditor(object sender, EventArgs e) {
		}
		protected void OnAboutClick(object sender, EventArgs e) {
			GridControl.About();
		}
		public virtual void ShowDesigner(BaseView view, string activeItem) {
			if(DebuggingStateCheckHelper.PreventDebuggingCrashWhileDebugging(this.Grid,false)) return;
			if(Grid == null) return;
			if(Editor == null) {
				editor = new frmDesigner();
			}
			try {
				Editor.ShowInTaskbar = false;
				if(view == null && activeItem == null) {
					Editor.InitGrid(Grid);
				} else {
					Editor.InitGrid(Grid, activeItem, view);
				}
				if(Grid.FindForm() != null) 
					Grid.FindForm().AddOwnedForm(editor);
				Editor.ShowDialog();
			} finally {
				Editor = null;
				UpdateLevelSelector(true);
			}
		}
		public virtual void ShowLayoutDesigner(BaseView view, string activeItem) {
			if(DebuggingStateCheckHelper.PreventDebuggingCrashWhileDebugging(this.Grid,false)) return;
			if(Grid == null) return;
			if(Editor == null) editor = new frmDesigner();
			try {
				Editor.ShowInTaskbar = false;
				Editor.InitGrid(Grid,"Layout");
				if(Grid.FindForm() != null) Grid.FindForm().AddOwnedForm(editor);
				Editor.ShowDialog();
			}
			finally {
				Editor = null;
				UpdateLevelSelector(true);
			}
		}
		protected void OnLayoutDesignerClick(object sender, EventArgs e) {
			if(expired)
				OnAboutClick(sender, e);
			else
				ShowLayoutDesigner(null, null);
		}
		protected void OnDesignerClick(object sender, EventArgs e) {
			if(expired) 
				OnAboutClick(sender, e);
			else
				ShowDesigner(null, null);
		}
		protected frmDesigner Editor {  
			get { return editor; }
			set {
				if(Editor == value) return;
				if(Editor != null) Editor.Dispose();
				editor = value;
			}
		}
		public LevelSelector Selector { get { return levelSelector; } }
		protected virtual void UpdateLevelSelector(bool updateLevels) {
			if(Grid == null) return;
			if(updateLevels)
				Selector.UpdateLevels();
			Selector.UpdateSize();
			Selector.Location = new Point(Grid.ClientSize.Width - Selector.Width - 10, Grid.ClientSize.Height - Selector.Height - 22);
			this.Grid.Controls.Add(Selector);
			Selector.BringToFront();
		}
		protected override void OnCreateHandle() {
			base.OnCreateHandle();
			UnhookChildControls(Selector);
		}
		protected void OnGrid_DataSourceChanged(object sender, EventArgs e) {
			if(Grid.IsHandleCreated) 
				UpdateLevelSelector(true);
			DevExpress.Design.DataAccess.UI.DataSourceGlyph.DataSourceChanged(Grid);
		}
		protected void OnGrid_HandleCreated(object sender, EventArgs e) {
			UpdateLevelSelector(true);
			UnhookChildControls(Selector);
		}
		string mainViewType;
		int columnsCount;
		void Grid_Paint(object sender, PaintEventArgs e) {
			bool mainViewTypeChanged = false;
			bool columnsChanged = false;
			ColumnView view = Grid.MainView as ColumnView;
			if(view == null)
				mainViewType = null;
			else {
				string viewType = view.GetType().Name;
				if(viewType != mainViewType) {
					mainViewType = viewType;
					mainViewTypeChanged = true;
				}
				if(view.Columns.Count != columnsCount) {
					columnsCount = view.Columns.Count;
					columnsChanged = true;
				}
			}
			if(mainViewTypeChanged || columnsChanged)
				UpdateDataSourceGlyph(LevelDesignerVisible);
		}
		protected void OnGrid_Load(object sender, EventArgs e) {
			UpdateLevelSelector(true);
			UpdateDataSourceGlyph(LevelDesignerVisible);
		}
		protected void OnGrid_SizeChanged(object sender, EventArgs e) {
			UpdateLevelSelector(false);
		}
		protected void OnSelector_SizeChanged(object sender, EventArgs e) {
			UpdateLevelSelector(false);
		}
		protected void DisableUndoEngine() {
			if(UndoEngine == null) return;
			UndoEngine.Enabled = false;
		}
		protected void EnableUndoEngine() {
			if(UndoEngine == null) return;
			UndoEngine.Enabled = true;
		}
		protected UndoEngine UndoEngine {
			get { return (UndoEngine)((IDesignerHost)this.Component.Site.Container).GetService(typeof(UndoEngine)); }
		}
		bool expired = false;
		IDesignerHost host;
		public override void Initialize(IComponent component) {
			base.Initialize(component); 
			IInheritanceService srv = GetService(typeof(IInheritanceService)) as IInheritanceService;;
			expired = IsExpired(this);
			UpdateLevelDesignerVisibility();
			this.host = GetService(typeof(IDesignerHost)) as IDesignerHost;
			LoaderPatcherService.InstallService(host);
			ISelectionService ss = (ISelectionService)GetService(typeof(ISelectionService));
			if(ss != null) {
				ss.SelectionChanged += new EventHandler(OnSelectionChanged);
				Selector.Tree.SelectionService = ss;
			}
			Grid.SizeChanged += new EventHandler(OnGrid_SizeChanged);
			Grid.DataSourceChanged += new EventHandler(OnGrid_DataSourceChanged);
			Grid.HandleCreated += new EventHandler(OnGrid_HandleCreated);
			Grid.Load += new EventHandler(OnGrid_Load);
			Grid.Paint += new PaintEventHandler(Grid_Paint);
			Selector.EditingGrid = Grid;
			if(Grid.IsHandleCreated) 
				UpdateLevelSelector(true);
			UnhookChildControls(Selector);
		}
		protected virtual void ClearDesigner() {
			if(Grid != null) {
				Grid.Paint -= new PaintEventHandler(Grid_Paint);
				Grid.SizeChanged -= new EventHandler(OnGrid_SizeChanged);
				Grid.DataSourceChanged -= new EventHandler(OnGrid_DataSourceChanged);
				Grid.HandleCreated -= new EventHandler(OnGrid_HandleCreated);
				Grid.Load -= new EventHandler(OnGrid_Load);
			}
			Editor = null;
			ISelectionService ss = (ISelectionService)GetService(typeof(ISelectionService));
			if (ss != null) {
				ss.SelectionChanged -= new EventHandler(OnSelectionChanged);
			}
			if(Selector != null) {
				this.levelSelector.Tree.SelectionService = null;
				this.levelSelector.Tree.ShownEditor -= new EventHandler(OnSelector_ShownEditor);
				this.levelSelector.Tree.HiddenEditor -= new EventHandler(OnSelector_HiddenEditor);
				Selector.Dispose();
				this.levelSelector = null;
			}
		}
		protected override void Dispose(bool disposing) {
			GridSplitContainer splitOwner = Grid == null ? null : Grid.Parent as GridSplitContainer;
			IDesignerHost host = this.host;
			LoaderPatcherService.UnInstallService(host);
			if(disposing) {
				SaveSettings();
				ClearDesigner();
			}
			this.host = null;
			base.Dispose(disposing);
		}
		protected virtual void SaveSettings() {
			PropertyStore ps = new PropertyStore(frmDesigner.GridSettings);
			ps.AddProperty("ShowLevelDesigner", LevelDesignerVisible);
			ps.Store();
		}
		protected internal GridControl Grid { get { return Control as GridControl; } }
		public override ICollection AssociatedComponents {
			get {
				if(Grid == null) return base.AssociatedComponents;
				ArrayList controls = new ArrayList();
				foreach(RepositoryItem repository in Grid.RepositoryItems) {
					controls.Add(repository);
				}
				foreach(BaseView view in Grid.ViewCollection) {
					AddView(controls, view);
				}
				AddBase(controls);
				return controls;
			}
		}
		void AddView(ArrayList controls, BaseView view) {
			if(view == null) return;
			controls.Add(view);
		}
		void AddBase(ArrayList controls) {
			foreach(object obj in base.AssociatedComponents) {
				if(controls.Contains(obj)) continue;
				controls.Add(obj);
			}
		}
		protected internal virtual bool SelectorContains(Point point) {
			if(Selector != null && Selector.Visible && Selector.Bounds.Contains(point)) return true;
			return false;
		}
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			UpdateLevelSelector(true);
			if(Grid == null || Grid.Container == null) return;
			if(Grid.MenuManager == null)
				Grid.MenuManager = ControlDesignerHelper.GetBarManager(Grid.Container);
		}
		private void OnSelectionChanged(object sender, EventArgs e) {
			Grid.Invalidate();
			ISelectionService ss = (ISelectionService)GetService(typeof(ISelectionService));
			if(ss != null) {
				CheckSelection(ss.PrimarySelection);
			}
		}
		void CheckSelection(object selectedObject) {
			if(selectedObject == null) return;
			BaseView view = selectedObject as BaseView;
			if(view != null && view.GridControl == Grid) {
				SwitchVisibleView(view);
			}
			GridColumn column = selectedObject as GridColumn;
			if(column != null && column.View != null && column.View.GridControl == Grid) {
				var current = EditorContextHelperEx.GetLastActionUIComponent(column);
				if(current != null && current != column) {
					EditorContextHelperEx.RefreshSmartPanel(column);
				}
			}
		}
		public override DesignerVerbCollection DXVerbs { 
			get {
				if(!AllowDesigner) return null;
				if(Grid!=null && Grid.MainView is LayoutView) return layoutViewVerbs;
				return defaultVerbs;
			} 
		}
		protected internal bool IsSplitView { get { return Grid != null && Grid.IsSplitGrid; } }
		protected override void PostFilterProperties(IDictionary properties) {
			base.PostFilterProperties(properties);
			DXPropertyDescriptor.ConvertDescriptors(properties, null);
		}
		protected internal virtual Cursor GetHitTestCore(Point clientPoint) {
			if(SelectorContains(clientPoint)) {
				return Selector.GetHitTest(Grid.PointToScreen(clientPoint));
			}
			if(Grid == null) return null;
			GridView view = Grid.DefaultView as GridView;
			CardView cardView = Grid.DefaultView as CardView;
			LayoutView layoutView = Grid.DefaultView as LayoutView;
			if(view != null) return GetGridHitTest(view, clientPoint);
			if(cardView != null) return GetCardHitTest(cardView, clientPoint);
			if(layoutView != null) return GetLayoutHitTest(layoutView, clientPoint);
			return null;
		}
		Cursor GetGridHitTest(GridView view, Point clientPoint) {
			Cursor cur = null;
			if(view.IsSizingState) return Cursors.SizeWE;
			if(view.IsDraggingState) return Cursors.Arrow;
			GridHitInfo hitInfo = view.CalcHitInfo(clientPoint);
			if(hitInfo.HitTest == GridHitTest.Column || hitInfo.HitTest == GridHitTest.GroupPanelColumn) cur = Cursors.Default;
			if(hitInfo.HitTest == GridHitTest.ColumnEdge) cur = Cursors.SizeWE;
			if(hitInfo.InColumnPanel && view.Columns.Count > 0) return cur == null ? Cursors.Arrow : cur;
			if(hitInfo.InGroupPanel && hitInfo.Column != null) return cur == null ? Cursors.Arrow : cur;
			DevExpress.XtraGrid.Views.BandedGrid.ViewInfo.BandedGridHitInfo bHit = hitInfo as DevExpress.XtraGrid.Views.BandedGrid.ViewInfo.BandedGridHitInfo;
			if(bHit != null && bHit.HitTest == DevExpress.XtraGrid.Views.BandedGrid.ViewInfo.BandedGridHitTest.Band) cur = Cursors.Default;
			if(bHit != null && bHit.HitTest == DevExpress.XtraGrid.Views.BandedGrid.ViewInfo.BandedGridHitTest.BandEdge) cur = Cursors.SizeWE;
			if(bHit != null && bHit.InBandPanel) return cur == null ? Cursors.Arrow : cur;
			return null;
		}
		Cursor GetCardHitTest(CardView view, Point clientPoint) {
			DevExpress.XtraGrid.Views.Card.ViewInfo.CardHitInfo hitInfo = view.CalcHitInfo(clientPoint);
			if(hitInfo.InCard) return Cursors.Arrow;
			return null;
		}
		Cursor GetLayoutHitTest(LayoutView view, Point clientPoint) {
			LayoutViewHitInfo hitInfo = view.CalcHitInfo(clientPoint) as LayoutViewHitInfo;
			if(hitInfo.InCard) return Cursors.Arrow;
			return null;
		}
		protected override bool GetHitTest(Point point) {
			if(Grid == null || !AllowDesigner || DebuggingState) return false;
			return false;
		}
		protected override bool AlwaysCreateActionLists { get { return true; } }
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			if(AllowDesigner && Grid != null) {
				list.Add(new GridActionList(this));
				BaseView view = Grid.DefaultView;
				if(view != null && host != null) {
					BaseViewDesigner designer = host.GetDesigner(view) as BaseViewDesigner;
					if(designer != null) list.Add(designer.GetViewActionList());
				}
			}
			base.RegisterActionLists(list);
		}
		public class GridActionList : DesignerActionList {
			GridControlDesigner designer;
			public GridActionList(GridControlDesigner designer)
				: base(designer.Component) {
				this.designer = designer;
			}
			public GridControl Grid { get { return designer.Grid; } }
			public override DesignerActionItemCollection GetSortedActionItems() {
				DesignerActionItemCollection res = new DesignerActionItemCollection();
				if(Grid == null) return res;
				string dataSourceCategoryName = "DataSource";
				res.Add(new DataSourceWizardDesignerActionMethodItem(this, dataSourceCategoryName));
				res.Add(new DesignerActionPropertyItem("DataSource", Properties.Resources.ChooseDataSourceCaption, dataSourceCategoryName));
				res.Add(new DesignerActionMethodItem(this, "RunDesigner", Properties.Resources.RunDesignerVerbName));
				if(Grid!=null && Grid.MainView is LayoutView) {
					res.Add(new DesignerActionMethodItem(this, "RunLayoutDesigner", Properties.Resources.CustomizeLayoutVerbName));				
				}
				res.Add(new DesignerActionHeaderItem(Properties.Resources.OptionsCaption, "Add"));
				res.Add(new DesignerActionPropertyItem("ShowLevelDesigner", Properties.Resources.ShowLevelDesignerCaption, "Add"));
				res.Add(new DesignerActionMethodItem(this, "SwitchSplitContainer", designer.IsSplitView ? Properties.Resources.RemoveSplitContainerCaption : Properties.Resources.AddSplitContainerCaption, "Add"));
				return res;
			}
			public void SwitchSplitContainer() {
				designer.SwitchSplitContainer();
			}
			public void CreateDataSource() {
				designer.CreateDataSource();
			}
			public void RunDesigner() {
				designer.ShowDesigner(null, null);
			}
			public void RunLayoutDesigner() {
				designer.ShowLayoutDesigner(null, null);
			}
			public bool ShowLevelDesigner {
				get { return designer.LevelDesignerVisible; }
				set { designer.LevelDesignerVisible = value; }
			}
#if DXWhidbey
			[AttributeProvider(typeof(IListSource))]
#endif
			public object DataSource {
				get { return Grid.DataSource; }
				set {
					EditorContextHelper.SetPropertyValue(designer, Grid, "DataSource", value);
				}
			}
		}
		internal void SwitchSplitContainer() {
			if(host == null) return;
			DesignerTransaction transaction = host.CreateTransaction("SwitchSplitContainer");
			try {
				if(IsSplitView) {
					Grid.RemoveSplitContainer();
				} else {
					GridSplitContainer split = Grid.CreateSplitContainer();
					if(split != null && host.Container != null) host.Container.Add(split);
				}
			} catch {
				transaction.Cancel();
				throw;
			}
			transaction.Commit();
			EditorContextHelperEx.RefreshSmartPanel(Component);
			EditorContextHelperEx.HideSmartPanel(Component);
			UpdateLevelSelector(true);
		}
	}
	public class XtraGridBlendingDesigner : ComponentDesigner {
		DesignerVerbCollection verbs;
		public XtraGridBlendingDesigner() {
			verbs =	new DesignerVerbCollection(
				new DesignerVerb[] {
									   new DesignerVerb(Properties.Resources.PreviewVerbName, new EventHandler(OnPreviewClick))});
		}
		public override DesignerVerbCollection Verbs { get { return verbs; } }
		private XtraGridBlending Blending { get { return Component as XtraGridBlending; } }
		private void OnPreviewClick(object sender, EventArgs e) {
			if(Blending.GridControl != null) {
				Form dlg = new Preview(Blending);
				dlg.ShowDialog();
			} else 
				MessageBox.Show(Properties.Resources.GridInitWarning, 
					string.Format(Properties.Resources.GridInitWarningCaption, Blending.Site.Name), MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			if(Blending == null || Blending.Container == null) return;
			if(Blending.GridControl == null)
				Blending.GridControl = GetGrid(Blending.Container);
		}
		private static GridControl GetGrid(IContainer container) {
			return GetTypeFromContainer(container, typeof(GridControl)) as GridControl;
		}
		protected static object GetTypeFromContainer(IContainer container, Type type) {
			if(container == null || type == null) return null;
			foreach(object obj in container.Components) {
				if(type.IsInstanceOfType(obj)) return obj;
			}
			return null;
		}
	}
	public class BaseViewActionList : DesignerActionList {
		BaseView view;
		BaseViewDesigner viewDesigner;
		public BaseViewActionList(BaseViewDesigner viewDesigner, BaseView view)
			: base(view) {
			this.viewDesigner = viewDesigner;
			this.view = view;
		}
		protected BaseViewDesigner Designer { get { return viewDesigner; } }
		protected BaseView View { get { return view; } }
		public override DesignerActionItemCollection GetSortedActionItems() {
			if(View == null) return new DesignerActionItemCollection();
			var res = new DesignerActionItemCollection();
			res.Insert(0, new DesignerActionHeaderItem("View actions: " + View.Name));
			AddPropertyActions(res);
			AddMethodActions(res);
			return res;
		}
		protected virtual void AddPropertyActions(DesignerActionItemCollection list) { }
		protected virtual void AddMethodActions(DesignerActionItemCollection list) { }
	}
	public class BaseViewDesigner : BaseComponentDesigner {
		DesignerVerbCollection verbs = null;
		public BaseViewDesigner() {
		}
		public DesignerActionList GetViewActionList() { return CreateViewActionList(); }
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			if(!AllowDesigner || View == null) return;
			DesignerActionList res = CreateViewActionList();
			if(res != null) list.Add(res);
		}
		protected override bool  AlwaysCreateActionLists { get { return true; } }
		protected virtual DesignerActionList CreateViewActionList() {
			return null;
		}
		protected override bool AllowInheritanceWrapper { get { return true; } }
		protected override bool UseVerbsAsActionList { get { return true; } }
		public override DesignerVerbCollection DXVerbs { 
			get { 
				if(verbs == null) UpdateVerbs();
				if(!AllowDesigner) return null;
				return verbs; 
			} 
		}
		protected override bool AllowEditInherited { get { return false; } }
		protected override void PostFilterProperties(IDictionary properties) {
			base.PostFilterProperties(properties);
			DXPropertyDescriptor.ConvertDescriptors(properties, null);
		}
		protected BaseView View { get { return Component as BaseView; } }
		protected virtual void UpdateVerbs() {
			this.verbs = new DesignerVerbCollection();
			if(View == null || View.BaseInfo == null || View.BaseInfo.Designer == null) return;
			if(View.GridControl == null || !View.GridControl.IsDesignMode) return;
			foreach(DesignerGroup group in View.BaseInfo.Designer.Groups) {
				foreach(DesignerItem item in group) {
					if(!item.ShowInVerbs) continue;
					verbs.Add(new DesignerVerb(item.Caption, new EventHandler(OnVerbClick)));
				}
			}
		}
		protected virtual void OnVerbClick(object sender, EventArgs e) {
			DesignerVerb verb = sender as DesignerVerb;
			if(verb == null) return;
			if(View == null || View.BaseInfo == null || View.BaseInfo.Designer == null) return;
			DesignerItem item = null;
			foreach(DesignerGroup group in View.BaseInfo.Designer.Groups) {
				item = group.GetItemByCaption(verb.Text);
				if(item != null) break;
			}
			if(item == null) return;
			if(View.GridControl == null) return;
			IDesignerHost host = GetService(typeof(IDesignerHost)) as IDesignerHost;
			if(host == null) return;
			GridControlDesigner designer = host.GetDesigner(View.GridControl) as GridControlDesigner;
			if(designer == null) return;
			designer.ShowDesigner(View, item.Caption);
		}
	}
	public class LayoutViewFieldDesigner : ComponentDesigner {
		protected string Name {
			get { return Component.Site.Name; }
			set {
				IDesignerHost host = GetService(typeof(IDesignerHost)) as IDesignerHost;
				if(host == null || (host != null && !host.Loading))
					Component.Site.Name = value;
			}
		}
		protected override void PreFilterProperties(IDictionary properties) {
			base.PreFilterProperties(properties);
			ShadowTheProperty(properties, "Name");
		}
		static void ShadowTheProperty(IDictionary properties, string property) {
			PropertyDescriptor prop = (PropertyDescriptor)properties[property];
			if(prop != null)
				properties[property] = TypeDescriptor.CreateProperty(typeof(LayoutViewFieldDesigner), prop, new Attribute[0]);
		}
	}
	public class LayoutViewDesigner : ColumnViewDesigner {
		public override ICollection AssociatedComponents {
			get {
				LayoutView view = Component as LayoutView;
				if(view == null)
					return base.AssociatedComponents;
				ArrayList components = new ArrayList();
				foreach(LayoutViewColumn column in view.Columns) {
					AddColumn(components, column);
				}
				if(view.TemplateCard != null)
					components.Add(view.TemplateCard);
				AddBase(components);
				return components;
			}
		}
		void AddColumn(ArrayList components, LayoutViewColumn column) {
			if(column == null) return;
			components.Add(column);
			if(column.LayoutViewField != null)
				components.Add(column.LayoutViewField);
		}
	}
	public class ColumnViewActionLists : BaseViewActionList {
		public ColumnViewActionLists(ColumnViewDesigner designer, ColumnView view) : base(designer, view) { }
		public virtual void AddColumn() {
			View.Columns.Add().Visible = true;
		}
		protected override void AddMethodActions(DesignerActionItemCollection list) {
			base.AddMethodActions(list);
			list.Add(new DesignerActionMethodItem(this, "AddColumn", "Add Column", "View"));
		}
		protected new ColumnView View { get { return (ColumnView)base.View; } }
	}
	public class ColumnViewDesigner : BaseViewDesigner {
		public override ICollection AssociatedComponents {
			get {
				ColumnView view = Component as ColumnView;
				if(view == null) return base.AssociatedComponents;
				ArrayList controls = new ArrayList();
				foreach(GridColumn col in view.Columns) {
					AddColumn(controls, col);
				}
				AddBase(controls);
				return controls;
			}
		}
		protected override DesignerActionList CreateViewActionList() {
			return new ColumnViewActionLists(this, (ColumnView)View);
		}
		void AddColumn(ArrayList controls, GridColumn col) {
			if(col == null) return;
			controls.Add(col);
		}
		protected void AddBase(ArrayList controls) {
			foreach(object obj in base.AssociatedComponents) {
				if(controls.Contains(obj)) continue;
				controls.Add(obj);
			}
		}
	}
	public class WinExplorerViewComponentDesigner : ColumnViewDesigner {
	}
	public class GridViewActionLists : ColumnViewActionLists {
		public GridViewActionLists(GridViewComponentDesigner designer, GridView view) : base(designer, view) { }
		protected new GridView View { get { return (GridView)base.View; } }
		[Category("Appearance"), Description("Show Group Panel")]
		public bool ShowGroupPanel {
			get { return View.OptionsView.ShowGroupPanel; }
			set {
				EditorContextHelper.SetPropertyValue(Designer, View.OptionsView, "ShowGroupPanel", value);
			}
		}
		[Category("Appearance"), Description("Show Footer")]
		public bool ShowFooter {
			get { return View.OptionsView.ShowFooter; }
			set {
				EditorContextHelper.SetPropertyValue(Designer, View.OptionsView, "ShowFooter", value);
			}
		}
		protected override void AddPropertyActions(DesignerActionItemCollection list) {
			base.AddPropertyActions(list);
			list.Add(new DesignerActionHeaderItem("Appearance", "Appearance"));
			list.Add(new DesignerActionPropertyItem("ShowGroupPanel", "Show Group Panel", "Appearance"));
			list.Add(new DesignerActionPropertyItem("ShowFooter", "Show Footer", "Appearance"));
		}
	}
	public class GridViewComponentDesigner : ColumnViewDesigner {
		protected override DesignerActionList CreateViewActionList() {
			return new GridViewActionLists(this, (GridView)View);
		}
	}
	public class GridColumnDesigner : BaseComponentDesigner {
		protected override bool AllowEditInherited {
			get {
				return false;
			}
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new ColumnDesignerActionList(this, Component as GridColumn));
		}
		internal class ColumnDesignerActionList : DesignerActionList {
			DesignerActionItemCollection collection;
			IDesigner designer;
			public ColumnDesignerActionList(IDesigner designer, GridColumn column)
				: base(column) {
				this.designer = designer;
			}
			public override DesignerActionItemCollection GetSortedActionItems() {
				if(collection == null) collection = GetActionItems();
				return collection;
			}
			GridColumn Column { get { return Component as GridColumn; } }
			DesignerActionItemCollection GetActionItems() {
				DesignerActionItemCollection res = new DesignerActionItemCollection();
				res.Add(new DesignerActionHeaderItemEx(
					new GetDisplayNameDelegate(()=>{ return "Column: " + Column.Name;})));
				res.Add(new DesignerActionPropertyItem("Name", "Name"));
				res.Add(new DesignerActionPropertyItem("Caption", "Caption"));
				res.Add(new DesignerActionPropertyItem("FieldName", "Field Name"));
				res.Add(new DesignerActionPropertyItem("ColumnEdit", "Column Edit", "Editor"));
				res.Add(new DesignerActionPropertyItem("Image", "Image"));
				return res;
			}
			public string Caption {
				get { return Column.Caption; }
				set {
					EditorContextHelper.SetPropertyValue(designer, Column, "Caption", value);
				}
			}
			[Editor(typeof(DXImageEditor), typeof(UITypeEditor))]
			public Image Image {
				get { return Column.Image; }
				set {
					EditorContextHelper.SetPropertyValue(designer, Column, "Image", value);
				}
			}
			[RefreshProperties(System.ComponentModel.RefreshProperties.All)]
			public string Name {
				get { return Column.Name; }
				set {
					EditorContextHelper.SetPropertyValue(designer, Column, "Name", value);
				}
			}
			[Editor("DevExpress.XtraGrid.Design.GridColumnNameEditor, " + AssemblyInfo.SRAssemblyGridDesign, typeof(System.Drawing.Design.UITypeEditor)),
			TypeConverter("DevExpress.XtraGrid.TypeConverters.FieldNameTypeConverter, " + AssemblyInfo.SRAssemblyGridDesign)]
			public string FieldName {
				get { return Column.FieldName; }
				set {
					EditorContextHelper.SetPropertyValue(designer, Column, "FieldName", value);
				}
			}
			[
			TypeConverter("DevExpress.XtraGrid.TypeConverters.ColumnEditConverter, " + AssemblyInfo.SRAssemblyGridDesign),
			Editor("DevExpress.XtraGrid.Design.ColumnEditEditor, " + AssemblyInfo.SRAssemblyGridDesign, typeof(System.Drawing.Design.UITypeEditor))]
			public DevExpress.XtraEditors.Repository.RepositoryItem ColumnEdit {
				get { return Column.ColumnEdit; }
				set {
					EditorContextHelper.SetPropertyValue(designer, Column, "ColumnEdit", value);
				}
			}
		}
	}
	public class GridBandDesigner : BaseComponentDesigner {
		protected override bool AllowEditInherited {
			get {
				return false;
			}
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new BandDesignerActionList(this, Component as GridBand));
		}
		internal class BandDesignerActionList : DesignerActionList {
			DesignerActionItemCollection collection;
			IDesigner designer;
			public BandDesignerActionList(IDesigner designer, GridBand band)
				: base(band) {
				this.designer = designer;
			}
			public override DesignerActionItemCollection GetSortedActionItems() {
				if(collection == null) collection = GetActionItems();
				return collection;
			}
			GridBand Band { get { return Component as GridBand; } }
			DesignerActionItemCollection GetActionItems() {
				DesignerActionItemCollection res = new DesignerActionItemCollection();
				res.Add(new DesignerActionHeaderItemEx(
					new GetDisplayNameDelegate(() => { return "Band: " + Band.Name; })));
				res.Add(new DesignerActionPropertyItem("Caption", "Caption"));
				res.Add(new DesignerActionPropertyItem("Image", "Image"));
				if(!Band.HasChildren) 
					res.Add(new DesignerActionMethodItem(this, "AddColumn", "Add Column"));
				return res;
			}
			public string Caption {
				get { return Band.Caption; }
				set {
					EditorContextHelper.SetPropertyValue(designer, Band, "Caption", value);
				}
			}
			[Editor(typeof(DXImageEditor), typeof(UITypeEditor))]
			public Image Image {
				get { return Band.Image; }
				set {
					EditorContextHelper.SetPropertyValue(designer, Band, "Image", value);
				}
			}
			public void AddColumn() {
				BandedGridColumn column = Band.View.Columns.Add();
				column.Visible = true;
				Band.Columns.Add(column);
			}
		}
	}
	[GuidAttribute("7494682b-37a0-11d2-a273-00c04f8ef4ff"),ComVisible(true),InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IVSMDPropertyBrowser {
	}
	public class GridLookUpEditBaseDesigner : ButtonEditDesigner {
		public override ICollection AssociatedComponents {
			get {
				ArrayList controls = new ArrayList(base.AssociatedComponents);
				if(Properties != null) controls.Add(Properties.View);
				return controls;
			}
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new LookUpEditBaseDataBindingActionList(this));
			list.Add(new SingleMethodActionList(this, new MethodInvoker(DesignView), "Design View", true));
			base.RegisterActionLists(list);
		}
		public virtual void DesignView() {
			EditorContextHelper.EditValue(this, Properties, "View");
		}
		protected override void OnInitializeNew(IDictionary defaultValues) {
			base.OnInitializeNew(defaultValues);
			IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
			if(host != null) AddToContainer(Editor.Name, Properties, host.Container);
		}
		public RepositoryItemGridLookUpEditBase Properties { get { return (Editor == null ? null : Editor.Properties); } }
		public new GridLookUpEditBase Editor { get { return base.Editor as GridLookUpEditBase; } }
		internal static void AddToContainer(string name, RepositoryItemGridLookUpEditBase properties, IContainer container) {
			if(container == null) return;
			try {
				container.Add(properties.View, name + "View");
			} catch {
				try {
					container.Add(properties.View);
				} catch {
				}
			}
		}
	}
	public class GridLookUpEditRepositoryItemDesigner : BaseRepositoryItemDesigner {
		public override ICollection AssociatedComponents {
			get {
				ArrayList controls = new ArrayList(base.AssociatedComponents);
				if(Item != null) controls.Add(Item.View);
				return controls;
			}
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			if(Item != null && !Item.IsLoading && Item.View.Name == "") {
				if(Item.View.Site == null) {
					UpdateContainer();
				}
			}
		}
		protected override void OnInitializeNew(IDictionary defaultValues) {
			base.OnInitializeNew(defaultValues);
			UpdateContainer();
		}
		void UpdateContainer() {
			IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
			if(host != null && !host.Loading) GridLookUpEditBaseDesigner.AddToContainer(Item.Name, Item, host.Container);
		}
		public new RepositoryItemGridLookUpEditBase Item{ get { return base.Item as RepositoryItemGridLookUpEditBase; } }
		internal static void AddToContainer(string name, RepositoryItemGridLookUpEditBase properties, IContainer container) {
			if(container == null) return;
			try {
				container.Add(properties.View, name + "View");
			} catch {
				try {
					container.Add(properties.View);
				} catch {
				}
			}
		}
	}
	public class GridLookUpViewEditor : UITypeEditor {
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if(context != null && context.Instance != null) return UITypeEditorEditStyle.Modal;
			return base.GetEditStyle(context);
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object objValue) {
			BaseView view = objValue as BaseView;
			if(view == null || context == null || context.Instance == null || provider == null) return objValue;
			IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));	
			if(edSvc == null) return objValue;
			frmDesigner designer = new frmDesigner();
			try {
				view.GridControl.ForceInitialize();
				designer.ShowInTaskbar = true;
				designer.InitGrid(view.GridControl, view);
				IServiceProvider host = null;
				LookAndFeel.DesignService.ILookAndFeelService serv = null;
				if(view is Component)
					host = (view as Component).Site;
				if(host != null)
					serv = host.GetService(typeof(LookAndFeel.DesignService.ILookAndFeelService)) as LookAndFeel.DesignService.ILookAndFeelService;
				if(serv != null && !RegistryDesignerSkinHelper.CanUseDefaultControlDesignersSkin)
					serv.InitializeRootLookAndFeel(designer.LookAndFeel);
				else
					designer.LookAndFeel.SetSkinStyle("DevExpress Design");
				edSvc.ShowDialog(designer);
			} catch {}
			designer.Dispose();
			return objValue;
		}
	}
	public class DebuggingStateCheckHelper {
		static string errorMessage  = Properties.Resources.ComponentModifiedWarning;
		protected static IServiceProvider GetServiceProvider(object obj) {
			IServiceProvider provider = obj as IServiceProvider;
			if(provider != null) {
				return provider;
			}
			Component component = obj as Component;
			if(component != null) {
				return component.Site;
			}
			ComponentDesigner designer = obj as ComponentDesigner;
			if((designer != null) && (designer.Component != null)) {
				return designer.Component.Site;
			}
			return null;
		}
		public static bool IsDebugging(object obj) {
			IServiceProvider provider = GetServiceProvider(obj);
			if(provider == null) return false;
			object loader = provider.GetService(typeof(IResourceService));
			if(loader == null) {
				return false;
			}
			Type loaderType = loader.GetType();
			if(loaderType.Name == "VSCodeDomDesignerLoader") {
				PropertyInfo pi = loaderType.GetProperty("IsDebugging", BindingFlags.NonPublic | BindingFlags.Instance);
				if(pi != null) return (bool)pi.GetValue(loader, null);
			}
			return false;
		}
		public static bool PreventDebuggingCrashWhileDebugging(object obj, bool showMessage) {
			bool result = false;
			if(IsDebugging(obj)) {
				if (showMessage) XtraMessageBox.Show(errorMessage, Properties.Resources.ErrorCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
				result = true;
			}
			return result;
		}
	}
}
