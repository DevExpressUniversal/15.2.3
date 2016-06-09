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
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Data.OleDb;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils.Frames;
using DevExpress.Utils.Design;
using DevExpress.XtraNavBar;
using System.ComponentModel.Design;
using DevExpress.XtraEditors.Designer.Utils;
namespace DevExpress.XtraGrid.Design {
	public class frmDesigner : BaseDesignerForm {
		EditingGridInfo gridInfo;
		private LabelControl label1;
		private PopupLevelEdit leActiveView;
		#region Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDesigner));
			this.leActiveView = new DevExpress.XtraGrid.Design.PopupLevelEdit();
			this.label1 = new LabelControl();
			this.label1.Anchor = AnchorStyles.Right;
			((System.ComponentModel.ISupportInitialize)(this.leActiveView.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.leActiveView, "leActiveView");
			this.leActiveView.Name = "leActiveView";
			this.leActiveView.Properties.Appearance.Options.UseFont = true;
			this.leActiveView.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("leActiveView.Properties.Buttons"))))});
			this.leActiveView.Properties.PopupSizeable = false;
			this.leActiveView.EditValueChanged += new System.EventHandler(this.leActiveView_EditValueChanged);
			this.leActiveView.Anchor = AnchorStyles.Right;
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			resources.ApplyResources(this, "$this");
			this.LookAndFeel.UseDefaultLookAndFeel = false;
			this.Name = "frmDesigner";
			((System.ComponentModel.ISupportInitialize)(this.leActiveView.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		protected DevExpress.XtraGrid.GridControl EditingGrid { get { return GridInfo == null ? null : GridInfo.EditingGrid; } }
		private DevExpress.XtraGrid.Views.Base.BaseView CurrentView {
			get {
				return GridInfo == null ? null : GridInfo.SelectedView;
			}
		}
		public GridControlDesigner EditingGridDesigner {
			get {
				if(EditingGrid == null) return null;
				IDesignerHost host = EditingGrid.InternalGetService(typeof(IDesignerHost)) as IDesignerHost;
				if(host == null) return null;
				return host.GetDesigner(EditingGrid) as GridControlDesigner;
			}
		}
		public virtual EditingGridInfo GridInfo { get { return gridInfo; } }
		protected string CurrentViewName {
			get {
				if(CurrentView == null) return string.Empty;
				else return CurrentView.BaseInfo.ViewName;
			}
		}
		private BaseDesigner emptyDesigner = null;
		BaseDesigner gridDesigner;
		public frmDesigner() {
			InitializeComponent();
			this.gridInfo = null;
			this.emptyDesigner = new EmptyViewDesigner();
			this.emptyDesigner.Init();
			this.gridDesigner = new ControlGridDesigner();
			this.gridDesigner.Init();
			this.ProductInfo = new DevExpress.Utils.About.ProductInfo("XtraGrid", typeof(GridControl), DevExpress.Utils.About.ProductKind.DXperienceWin, DevExpress.Utils.About.ProductInfoStage.Registered);
		}
		IComponentChangeService changeService = null;
		public virtual void InitGrid(DevExpress.XtraGrid.GridControl grid) {
			InitGrid(grid, "", null);
		}
		public virtual void InitGrid(DevExpress.XtraGrid.GridControl grid, BaseView view) {
			InitGrid(grid, "", view);
		}
		public virtual void InitGrid(DevExpress.XtraGrid.GridControl grid, string itemLinkCaption) {
			InitGrid(grid, itemLinkCaption, null);
		}
		public virtual void InitGrid(DevExpress.XtraGrid.GridControl grid, string itemLinkCaption, BaseView view) {
			Initialize();
			this.gridInfo = new EditingGridInfo(grid, this);
			this.gridInfo.SelectionChanging += new EventHandler(gridInfo_SelectionChanging);
			this.gridInfo.SelectionChanged += new EventHandler(OnGridInfo_SelectionChanged);
			OnGridInfo_SelectionChanged(this, EventArgs.Empty);
			if(view != null) gridInfo.SelectedObject = view;
			leActiveView.Grid = grid;
			leActiveView.SetViewCore(gridInfo.SelectedView);
			InitEditingObject(grid, itemLinkCaption);
			this.changeService = EditingGrid.InternalGetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(this.changeService != null)
				this.changeService.ComponentRename += new ComponentRenameEventHandler(OnComponentRename);
			UpdatePainStyleItems();
		}
		void gridInfo_SelectionChanging(object sender, EventArgs e) {
			if(GridInfo.SelectedView != null)
				GridInfo.SelectedView.PaintStyleChanged -= new EventHandler(SelectedView_PaintStyleChanged);
		}
		protected void OnComponentRename(object sender, ComponentRenameEventArgs e) {
			OnGridInfo_SelectionChanged(this, EventArgs.Empty);
		}
		protected void OnGridInfo_SelectionChanged(object sender, EventArgs e) {
			leActiveView.SetViewCore(GridInfo.SelectedView);
			if(GridInfo.SelectedView != null)
				GridInfo.SelectedView.PaintStyleChanged += new EventHandler(SelectedView_PaintStyleChanged);
		}
		void SelectedView_PaintStyleChanged(object sender, EventArgs e) {
			UpdatePainStyleItems();
		}
		public const string GridSettings = "Software\\Developer Express\\Designer\\XtraGrid\\";
		protected override string RegistryStorePath { get { return GridSettings; } }
		protected override void InitFrame(string caption, Bitmap bitmap) {
			XF.InitFrame(CurrentView, GridInfo, caption, bitmap);
			normalText = XF.lbCaption.Text;
			pnlFrame.Controls.Add(leActiveView);
			pnlFrame.Controls.Add(label1);
			label1.BringToFront();
			leActiveView.BringToFront();
			if(IsHandleCreated)
				BeginInvoke(new Action(CalcElementsLocations));
			else
				CalcElementsLocations();
		}
		string normalText;
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			CalcElementsLocations();
		}
		protected void CalcElementsLocations() {
			if(XF == null) return;
			Rectangle bounds = XF.lbCaption.Bounds;
			DevExpress.XtraEditors.ViewInfo.LabelControlViewInfo viewInfo = null;
			System.Reflection.PropertyInfo[] properties = typeof(LabelControl).GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
			if(properties != null) {
				foreach(System.Reflection.PropertyInfo property in properties) {
					if(property.Name == "ViewInfo") {
						viewInfo = property.GetValue(XF.lbCaption, null) as DevExpress.XtraEditors.ViewInfo.LabelControlViewInfo;
					}
				}
			}
			bounds.Width = pnlFrame.Width;
			Point p = new Point();
			p.Y = (bounds.Height - leActiveView.Height) / 2;
			p.Y += 4;
			p.X = bounds.Width - leActiveView.Width;
			leActiveView.Location = p;
			p.Y += (leActiveView.Height - label1.Height) / 2;
			p.X -= 10;
			p.X -= label1.Width;
			CaclText(viewInfo, p.X - 15);
			label1.Location = p;
		}
		void CaclText(XtraEditors.ViewInfo.LabelControlViewInfo viewInfo, int p) {
			XF.lbCaption.Text = normalText;
			while(viewInfo.ContentSize.Width > p) {
				TrimLabelText();
				if(string.IsNullOrEmpty(XF.lbCaption.Text))
					break;
			}
		}
		void TrimLabelText() {
			string text = XF.lbCaption.Text;
			text = text.TrimEnd(new char[] { '.' });
			text = text.Remove(text.Length - 1);
			if(text[text.Length - 1] == ' ') { XF.lbCaption.Text = text; return; }
			text = String.Join("", new object[] { text, "..." });
			XF.lbCaption.Text = text;
		}
		protected override BaseDesigner ActiveDesigner {
			get {
				if(GridInfo != null && GridInfo.SelectedObject is GridControl) {
					return gridDesigner;
				}
				if(CurrentView == null || CurrentView.BaseInfo == null)
					return emptyDesigner;
				else
					return CurrentView.BaseInfo.Designer;
			}
		}
		protected override void OnXF_RefreshWizard(object sender, DevExpress.XtraEditors.Designer.Utils.RefreshWizardEventArgs e) {
			if(e.Condition == "ChangedView") {
				RefreshDesigner(false);
			}
		}
		protected void RefreshDesigner(bool updateActiveFrame) {
			InitNavBar(updateActiveFrame);
			UpdatePainStyleItems();
			EnabledMainItems();
		}
		void UpdatePainStyleItems() {
			foreach(NavBarItem item in NavBar.Items) {
				DesignerItem dItem = item.Tag as DesignerItem;
				if(dItem == null) continue;
				if(dItem.FrameTypeName == typeof(DevExpress.XtraGrid.Frames.SchemeDesigner).FullName)
					item.Visible = CurrentView != null && CurrentView.ActivePaintStyleName != "Skin";
			}
		}
		protected override Type ResolveType(string type) {
			Type t = typeof(frmDesigner).Assembly.GetType(type);
			if(t != null) return t;
			return base.ResolveType(type);
		}
		protected void EnabledMainItems() {
			for(int i = 0; i < NavBar.Items.Count; i++) {
				DesignerItem item = NavBar.Items[i].Tag as DesignerItem;
				if(item != null && item.Tag != null && item.Tag.ToString() == "main")
					NavBar.Items[i].Enabled = CurrentView == EditingGrid.MainView;
			}
		}
		private static int ViewType(object view, ImageList iml) {
			string[] viewNames = new string[] {"CardView", "GridView", "BandedGridView", "AdvBandedGridView"};
			if(view == null) return 0;
			int index = Array.IndexOf(viewNames, ((BaseView)view).BaseInfo.ViewName);
			return index < 0 ? iml.Images.Count - 1 : index + 1;
		}
		protected void FireGridChanged() {
			if(this.changeService != null) 
				this.changeService.OnComponentChanged(this, null, null, null);
		}
		protected override void OnClosed(EventArgs e) {	
			if(this.changeService != null) {
				this.changeService.ComponentRename -= new ComponentRenameEventHandler(OnComponentRename);
				this.changeService = null;
			}
			base.OnClosed(e);
		}
		private void leActiveView_EditValueChanged(object sender, System.EventArgs e) {
			if(GridInfo == null) return;
			GridInfo.SelectedObject = leActiveView.View;
			RefreshDesigner(true);
		}
		protected override bool AllowModuleNavigationCore { get { return true; } }
	}
	public class EditingGridInfo : ISelectionService {
		object selectedObject;
		GridControl editingGrid;
		IModuleNavigationSupports moduleNavigator;
		public EditingGridInfo(GridControl editingGrid) : this(editingGrid, null) { }
		public EditingGridInfo(GridControl editingGrid, IModuleNavigationSupports moduleNavigator) {
			this.editingGrid = editingGrid;
			this.moduleNavigator = moduleNavigator;
			this.selectedObject = GetEditingGridView();
		}
		object GetEditingGridView() {
			if(EditingGrid == null) return null;
			if(EditingGrid.DefaultView != null) return EditingGrid.DefaultView;
			return EditingGrid.MainView;
		}
		public virtual GridControl EditingGrid {
			get { return editingGrid; }
		}
		public virtual BaseView SelectedView { 
			get { return SelectedObject as BaseView; }
		}
		public virtual object SelectedObject { 
			get { return selectedObject; } 
			set {
				if(SelectedObject == value) return;
				if(SelectionChanging != null) SelectionChanging(this, EventArgs.Empty);
				this.selectedObject = value;
				if(SelectionChanged != null) SelectionChanged(this, EventArgs.Empty);
			}
		}
		bool ISelectionService.GetComponentSelected(object component) { return component == SelectedObject; }
		ICollection ISelectionService.GetSelectedComponents() {
			if(SelectedObject == null) return null;
			return new object[] { SelectedObject };
		}
		object ISelectionService.PrimarySelection {
			get { return SelectedObject; }
		}
		int ISelectionService.SelectionCount { get { return SelectedObject == null ? 0 : 1; } }
		void ISelectionService.SetSelectedComponents(ICollection components, SelectionTypes types) {
			((ISelectionService)this).SetSelectedComponents(components);
		}
		void ISelectionService.SetSelectedComponents(ICollection components) {
			if(components == null || components.Count == 0) SelectedObject = null;
			else {
				foreach(object sel in components) {
					SelectedObject = sel;
					break;
				}
			}
		}
		protected internal IModuleNavigationSupports ModuleNavigator { get { return moduleNavigator; } }
		public event EventHandler SelectionChanged, SelectionChanging;
	}
}
