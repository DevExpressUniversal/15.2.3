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
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.XtraTreeList.Helpers;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Blending;
using DevExpress.Utils.Design;
using DevExpress.Utils.About;
using DevExpress.XtraEditors.Design;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors;
using System.Runtime.InteropServices;
using System.Drawing.Design;
using System.Windows.Forms.Design.Behavior;
namespace DevExpress.XtraTreeList.Design {
	public class TreeListDesigner : BaseControlDesigner {
		DesignerVerbCollection verbs;
		BaseDesignerForm editor;
		ISelectionService selectionService;
		IDesignerHost designerHost;
#if DXWhidbey
		protected override bool AllowHookDebugMode { get { return true; } }
#endif
		protected override bool AllowEditInherited { get { return false; } }
		public TreeListDesigner() {
			this.verbs = CreateDesignerVerbs();
			this.editor = null;
			this.selectionService = null;
		}
		public override GlyphCollection GetGlyphs(System.Windows.Forms.Design.Behavior.GlyphSelectionType selectionType) {
			GlyphCollection result = base.GetGlyphs(selectionType);
			object selectedObject = null;
			ISelectionService service = GetService(typeof(ISelectionService)) as ISelectionService;
			if(service != null) 
				selectedObject = service.PrimarySelection;
			AddColumnGlyph(result, selectedObject);
			AddBandGlyph(result, selectedObject);
			return result;
		}
		protected virtual void AddBandGlyph(GlyphCollection result, object selectedObject) {
			TreeListBand band = selectedObject as TreeListBand;
			if(band != null && band.TreeList == TreeList) {
				ViewInfo.TreeListViewInfo viewInfo = band.TreeList.ViewInfo as ViewInfo.TreeListViewInfo;
				if(viewInfo == null) return;
				ViewInfo.BandInfo bi = viewInfo.BandsInfo.FindBand(band);
				if(bi == null) return;
				ControlDesignerActionListGlyphHelper.CreateDesignerGlyphWrapper(result, band, band.TreeList, bi.Bounds);
			}
		}
		protected virtual void AddColumnGlyph(GlyphCollection res, object selectedObject) {
			TreeListColumn column = selectedObject as TreeListColumn;
			if(column != null && column.TreeList == TreeList) {
				ViewInfo.TreeListViewInfo viewInfo = column.TreeList.ViewInfo as ViewInfo.TreeListViewInfo;
				if(viewInfo == null) return;
				ViewInfo.ColumnInfo ci = viewInfo.ColumnsInfo[column];
				if(ci == null) return;
				ControlDesignerActionListGlyphHelper.CreateDesignerGlyphWrapper(res, column, column.TreeList, ci.Bounds);
			}
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			this.selectionService = GetService(typeof(ISelectionService)) as ISelectionService;
			if (SelectionService != null)
				SelectionService.SelectionChanged += new EventHandler(OnSelectionChanged);
			this.designerHost = GetService(typeof(IDesignerHost)) as IDesignerHost;
			if (DesignerHost != null) {
				DesignerHost.LoadComplete += new EventHandler(OnDesignerHostLoadComplete);
				DesignerHost.Activated += new EventHandler(OnDesignerHostActivated);
				DesignerHost.Deactivated += new EventHandler(OnDesignerHostDeactivated);
				DevExpress.Utils.Design.LoaderPatcherService.InstallService(DesignerHost);
			}
			var fComponent = TreeList as DevExpress.Data.Filtering.IFilteredComponentBase;
			if(fComponent != null)
				fComponent.PropertiesChanged += OnDataSourceChanged;
		}
		protected override void Dispose(bool disposing) {
			var fComponent = TreeList as DevExpress.Data.Filtering.IFilteredComponentBase;
			if(fComponent != null)
				fComponent.PropertiesChanged -= OnDataSourceChanged;
			if(TreeList != null)
				DevExpress.Design.DataAccess.UI.DataSourceGlyph.Detach(TreeList);
			if (SelectionService != null) {
				SelectionService.SelectionChanged -= new EventHandler(OnSelectionChanged);
				this.selectionService = null;
			}
			if (DesignerHost != null) {
				DesignerHost.LoadComplete -= new EventHandler(OnDesignerHostLoadComplete);
				DesignerHost.Activated -= new EventHandler(OnDesignerHostActivated);
				DesignerHost.Deactivated -= new EventHandler(OnDesignerHostDeactivated);
				DevExpress.Utils.Design.LoaderPatcherService.UnInstallService(DesignerHost);
				this.designerHost = null;
			}
			if (disposing) {
				Editor = null;
			}
			base.Dispose(disposing);
		}
		protected virtual DesignerVerbCollection CreateDesignerVerbs() {
			return new DesignerVerbCollection(
							new DesignerVerb[] {
							 									   new DesignerVerb("About...", new EventHandler(OnAboutClick)),
							 									   new DesignerVerb("Run Designer", new EventHandler(OnDesignerClick)),
							 									   new DesignerVerb("Nodes Editor", new EventHandler(OnNodesEditorClick)),
							 			}
							);
		}
		protected virtual void OnDataSourceChanged(object sender, EventArgs e) {
			if(TreeList.IsLookUpMode) return;
			if(TreeList.IsUnboundMode)
				DevExpress.Design.DataAccess.UI.DataSourceGlyph.Detach(TreeList);
			else
				DevExpress.Design.DataAccess.UI.DataSourceGlyph.Attach(TreeList);
			EditorContextHelperEx.RefreshSmartPanel(TreeList);
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new TreeListDataSourceActionList(this));
			base.RegisterActionLists(list);
		}
		void OnSelectionChanged(object sender, EventArgs e) {
			Listener.OnSelectionChanged(SelectionService);
		}
		protected virtual void OnDesignerHostLoadComplete(object sender, EventArgs e) {
			if(TreeList.DataSource != null && !TreeList.IsLookUpMode)
				DevExpress.Design.DataAccess.UI.DataSourceGlyph.Attach(TreeList);
		}
		void OnDesignerHostActivated(object sender, EventArgs e) { ShowCustomizationForm(true); }
		void OnDesignerHostDeactivated(object sender, EventArgs e) { ShowCustomizationForm(false); }
		void ShowCustomizationForm(bool show) {
			if (TreeList == null || TreeList.CustomizationForm == null) return;
			if (show) TreeList.CustomizationForm.Show();
			else TreeList.CustomizationForm.Hide();
		}
		protected frmEditor Editor {
			get { return (frmEditor)editor; }
			set {
				if (Editor == value) return;
				if (Editor != null) Editor.Dispose();
				editor = value;
			}
		}
		protected TreeList TreeList { get { return Control as TreeList; } }
		protected virtual IDesignNotified Listener {
			get {
				if (TreeList == null) return NullListener.Instance;
				return TreeList.InternalGetService(typeof(IDesignNotified)) as IDesignNotified;
			}
		}
		const int WM_MOUSEMOVE = 0x200, WM_LBUTTONDOWN = 0x201, WM_LBUTTONUP = 0x202, MK_LBUTTON = 0x0001;
		protected override void WndProc(ref Message m) {
			base.WndProc(ref m);
			if(DebuggingState) return;
			if (!IsMouseMessage(m.Msg) || !IsNativeMessage(ref m)) return;
			Point p = PointFromMessage(ref m);
			MouseButtons buttons = MouseButtons.None;
			int btns = m.WParam.ToInt32();
			if ((btns & MK_LBUTTON) != 0) buttons |= MouseButtons.Left;
			MouseEventArgs e = new MouseEventArgs(buttons, 0, p.X, p.Y, 0);
			switch (m.Msg) {
				case WM_LBUTTONDOWN: Listener.OnMouseDown(e); break;
				case WM_LBUTTONUP: Listener.OnMouseUp(e); break;
				case WM_MOUSEMOVE: Listener.OnMouseMove(e); break;
			}
		}
		bool IsMouseMessage(int msg) {
			return (msg == WM_MOUSEMOVE || msg == WM_LBUTTONUP && msg == WM_LBUTTONDOWN);
		}
		Point PointFromMessage(ref Message m) {
			return new Point(m.LParam.ToInt32());
		}
		protected virtual bool IsNativeMessage(ref Message m) {
			if (TreeList != null && TreeList.IsHandleCreated &&
				m.HWnd != TreeList.Handle) return false;
			return !IsScrollBarTest(GetHitTestCore(PointFromMessage(ref m)));
		}
		protected override void OnMouseEnter() {
			base.OnMouseEnter();
			Listener.OnMouseEnter();
		}
		protected override void OnMouseLeave() {
			base.OnMouseLeave();
			Listener.OnMouseLeave();
		}
		private void OnAboutClick(object sender, EventArgs e) {
			TreeList.About();
		}
		private void OnNodesEditorClick(object sender, EventArgs e) {
			RunNodesEditor();
		}
		protected internal virtual void RunNodesEditor() {
			xNodesEditor editor = new xNodesEditor(TreeList.Nodes);
			LookAndFeel.DesignService.ILookAndFeelService lookAndFeelService = null;
			if(DesignerHost != null)
				lookAndFeelService = DesignerHost.GetService(typeof(LookAndFeel.DesignService.ILookAndFeelService)) as LookAndFeel.DesignService.ILookAndFeelService;
			if(editor != null && !RegistryDesignerSkinHelper.CanUseDefaultControlDesignersSkin)
				lookAndFeelService.InitializeRootLookAndFeel(editor.LookAndFeel);
			else
				editor.LookAndFeel.SetSkinStyle("DevExpress Design");
			if(!editor.TreeList.IsUnboundMode) {
				MessageBox.Show("The DataSource property is not null...\r\nPlease set it to '(none)' before editing the Nodes collection.", "Warning");
			}
			else {
				if(editor.TreeList.FindForm() != null) {
					editor.TreeList.FindForm().AddOwnedForm(Editor);
				}
				if(editor.ShowDialog() == DialogResult.OK) {
					TreeList tl = editor.TreeList;
					tl.FireChanged();
				}
			}
		}
		private void OnDesignerClick(object sender, EventArgs e) { RunDesigner(); }
		protected internal virtual void RunDesigner() {
			TreeList treeList = TreeList;
			if (treeList == null) return;
			if (Editor == null) {
				editor = CreateFormEditor();
			}
			try {
				Editor.ShowInTaskbar = false;
				Editor.InitEditingObject(treeList);
				if (treeList.FindForm() != null) {
					treeList.FindForm().AddOwnedForm(Editor);
				}
				Editor.ShowDialog();
			}
			finally {
				Editor = null;
			}
		}
		protected virtual void AddColumn() {
			if(TreeList == null) return;
			TreeListColumn column = DevExpress.XtraTreeList.Frames.ColumnDesigner.AddVisibleColumn(TreeList.Columns, string.Empty, TreeList.VisibleColumns.Count);
			if(SelectionService != null)
				SelectionService.SetSelectedComponents(new object[] { column });
			TreeList.FireChanged();
		}
		private void OnPopulateColumns(object sender, EventArgs e) {
			if (TreeList != null)
				TreeList.PopulateColumns();
		}
		protected override void PostFilterProperties(IDictionary properties) {
			base.PostFilterProperties(properties);
			DevExpress.Utils.Design.DXPropertyDescriptor.ConvertDescriptors(properties, null);
		}
		protected override bool GetHitTest(Point point) {
			if (TreeList != null && !DebuggingState) {
				HitInfoType hi = GetHitTestCore(TreeList.PointToClient(point));
				return (hi == HitInfoType.Column || IsScrollBarTest(hi) ||
					hi == HitInfoType.ColumnEdge || hi == HitInfoType.RowIndicatorEdge || hi == HitInfoType.Band || hi == HitInfoType.BandEdge);
			}
			return base.GetHitTest(point);
		}
		bool IsScrollBarTest(HitInfoType hi) {
			return (hi == HitInfoType.VScrollBar || hi == HitInfoType.HScrollBar);
		}
		protected HitInfoType GetHitTestCore(Point p) {
			if (TreeList == null || DebuggingState) return HitInfoType.None;
			return TreeList.CalcHitInfo(p).HitInfoType;
		}
		public override DesignerVerbCollection DXVerbs { get { return verbs; } }
		public override ICollection AssociatedComponents {
			get {
				ICollection c = base.AssociatedComponents;
				TreeList tl = TreeList;
				if (tl == null) return c;
				ArrayList components;
				if (c == null) components = new ArrayList();
				else components = new ArrayList(c);
				foreach (TreeListColumn col in tl.Columns) {
					components.Add(col);
				}
				foreach(DevExpress.XtraEditors.Repository.RepositoryItem item in tl.RepositoryItems) {
					components.Add(item);
				}
				return components;
			}
		}
		protected ISelectionService SelectionService { get { return selectionService; } }
		protected IDesignerHost DesignerHost { get { return designerHost; } }
		class NullListener : IDesignNotified {
			static NullListener instance = new NullListener();
			public static NullListener Instance { get { return instance; } }
			void IDesignNotified.OnMouseDown(MouseEventArgs e) { }
			void IDesignNotified.OnMouseMove(MouseEventArgs e) { }
			void IDesignNotified.OnMouseUp(MouseEventArgs e) { }
			void IDesignNotified.OnMouseEnter() { }
			void IDesignNotified.OnMouseLeave() { }
			void IDesignNotified.OnSelectionChanged(ISelectionService serv) { }
		}
		public class TreeListDataSourceActionList : DesignerActionList {
			TreeListDesigner designer;
			public TreeListDataSourceActionList(TreeListDesigner designer)
				: base(designer.Component) {
				this.designer = designer;
			}
			protected TreeList TreeList { get { return Designer.TreeList; } }
			protected TreeListDesigner Designer { get { return designer; } }
			public override DesignerActionItemCollection GetSortedActionItems() {
				DesignerActionItemCollection res = new DesignerActionItemCollection();
				string dataSourceCategoryName = "DataSource";
				res.Add(new DataSourceWizardDesignerActionMethodItem(this, dataSourceCategoryName));
				res.Add(new DesignerActionPropertyItem("DataSource", "Choose Data Source", dataSourceCategoryName));
				if(TreeList != null && TreeList.IsUnboundMode)
					res.Add(new DesignerActionMethodItem(this, "RunNodesEditor", "Nodes Editor"));
				res.Add(new DesignerActionMethodItem(this, "RunDesigner", "Run Designer"));
				res.Add(new DesignerActionMethodItem(this, "AddColumn", "Add Column"));
				return res;
			}
			[RefreshProperties(System.ComponentModel.RefreshProperties.All)]
			public virtual void CreateDataSource() {
				Designer.CreateDataSource();
			}
			public virtual void RunNodesEditor() {
				Designer.RunNodesEditor();
			}
			public virtual void RunDesigner() {
				Designer.RunDesigner();
			}
			public virtual void AddColumn() {
				Designer.AddColumn();
			}
#if DXWhidbey
			[AttributeProvider(typeof(IListSource))]
#endif
			public object DataSource {
				get { return TreeList.DataSource; }
				set {
					EditorContextHelper.SetPropertyValue(TreeList.Site, TreeList, "DataSource", value);
				}
			}
		}
		protected internal virtual BaseDesignerForm CreateFormEditor() {
			return new frmEditor();
		}
	}
	public class TreeListColumnDesigner : BaseComponentDesigner {
		protected override bool AllowEditInherited { get { return false; } }
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new TreeListColumnDesignerActionList(this, Component as TreeListColumn));
		}
		public class TreeListColumnDesignerActionList : DesignerActionList {
			DesignerActionItemCollection collection;
			IDesigner designer;
			public TreeListColumnDesignerActionList(IDesigner designer, TreeListColumn column)
				: base(column) {
				this.designer = designer;
			}
			public override DesignerActionItemCollection GetSortedActionItems() {
				if(collection == null)
					collection = CreateActionItems();
				return collection;
			}
			protected TreeListColumn Column { get { return Component as TreeListColumn; } }
			protected virtual DesignerActionItemCollection CreateActionItems() {
				DesignerActionItemCollection res = new DesignerActionItemCollection();
				res.Add(new DesignerActionHeaderItemEx(
					new GetDisplayNameDelegate(() => { return "Column: " + Column.Name; })));
				res.Add(new DesignerActionPropertyItem("Caption", "Caption"));
				res.Add(new DesignerActionPropertyItem("FieldName", "Field Name"));
				res.Add(new DesignerActionPropertyItem("ColumnEdit", "Column Edit", "Editor"));
				return res;
			}
			public string Caption {
				get { return Column.Caption; }
				set {
					EditorContextHelper.SetPropertyValue(designer, Column, "Caption", value);
				}
			}
			[TypeConverter("DevExpress.XtraTreeList.Design.TypeConverters.FieldNameTypeConverter, " + AssemblyInfo.SRAssemblyTreeListDesign)]
			public string FieldName {
				get { return Column.FieldName; }
				set {
					EditorContextHelper.SetPropertyValue(designer, Column, "FieldName", value);
				}
			}
			[Editor("DevExpress.XtraTreeList.Design.ColumnEditEditor, " + AssemblyInfo.SRAssemblyTreeListDesign, typeof(System.Drawing.Design.UITypeEditor))]
			public DevExpress.XtraEditors.Repository.RepositoryItem ColumnEdit {
				get { return Column.ColumnEdit; }
				set {
					EditorContextHelper.SetPropertyValue(designer, Column, "ColumnEdit", value);
				}
			}
		}
	}
	public class TreeListBandDesigner : BaseComponentDesigner {
		protected override bool AllowEditInherited { get { return false; } }
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new TreeListBandDesignerActionList(this, Component as TreeListBand));
		}
		public class TreeListBandDesignerActionList : DesignerActionList {
			DesignerActionItemCollection items;
			IDesigner designer;
			public TreeListBandDesignerActionList(IDesigner designer, TreeListBand band)
				: base(band) {
				this.designer = designer;
			}
			public override DesignerActionItemCollection GetSortedActionItems() {
				if(items == null) 
					items = CreateActionItems();
				return items;
			}
			protected TreeListBand Band { get { return Component as TreeListBand; } }
			protected virtual DesignerActionItemCollection CreateActionItems() {
				DesignerActionItemCollection res = new DesignerActionItemCollection();
				res.Add(new DesignerActionHeaderItemEx(
					new GetDisplayNameDelegate(() => { return "Band: " + Band.Name; })));
				res.Add(new DesignerActionPropertyItem("Caption", "Caption"));
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
			public void AddColumn() {
				TreeListColumn column = Band.TreeList.Columns.Add();
				column.Visible = true;
				Band.Columns.Add(column);
			}
		}
	}
	internal class XtraTreeListBlendingDesigner : ComponentDesigner {
		DesignerVerbCollection verbs;
		public XtraTreeListBlendingDesigner() {
			verbs =	new DesignerVerbCollection(
				new DesignerVerb[] {
					new DesignerVerb("Preview", new EventHandler(OnPreviewClick))});
		}
		public override DesignerVerbCollection Verbs {
			get { return verbs; }
		}
		private XtraTreeListBlending Blending { get { return Component as XtraTreeListBlending; } }
		private void OnPreviewClick(object sender, EventArgs e) {
			if(Blending.TreeListControl != null) {
				Form dlg = new DevExpress.XtraTreeList.Blending.Preview(Blending);
				dlg.ShowDialog();
			} else 
				MessageBox.Show("The TreeListControl property is not initialized."+
					"\r\nPlease select a treelist control from its dropdown list.",
					Blending.Site.Name + ".TreeListControl is null...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}
#if DXWhidbey
		public override void  InitializeNewComponent(IDictionary defaultValues) {
		 	 base.InitializeNewComponent(defaultValues);
#else
		public override void OnSetComponentDefaults() {
			base.OnSetComponentDefaults();
#endif
			if(Blending == null || Blending.Container == null) return;
			if(Blending.TreeListControl == null)
				Blending.TreeListControl = GetTreeList(Blending.Container);
		}
		private static TreeList GetTreeList(IContainer container) {
			return GetTypeFromContainer(container, typeof(TreeList)) as TreeList;
		}
		protected static object GetTypeFromContainer(IContainer container, Type type) {
			if(container == null || type == null) return null;
			foreach(object obj in container.Components) {
				if(type.IsInstanceOfType(obj)) return obj;
			}
			return null;
		}
	}
	[CLSCompliant(false)]
	public class TreeListLookUpEditDesigner : ButtonEditDesigner {
		public override ICollection AssociatedComponents {
			get {
				ArrayList controls = new ArrayList(base.AssociatedComponents);
				if(Properties != null) controls.Add(Properties.TreeList);
				return controls;
			}
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new TreeListLookUpActionList(this));
			list.Add(new SingleMethodActionList(this, new MethodInvoker(DesignTreeList), "Design TreeList", true));
			base.RegisterActionLists(list);
		}
		public virtual void DesignTreeList() {
			EditorContextHelper.EditValue(this, Properties, "TreeList");
		}
		protected override void OnInitializeNew(IDictionary defaultValues) {
			base.OnInitializeNew(defaultValues);
			IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
			if(host != null) AddToContainer(Editor.Name, Properties, host.Container);
		}
		public RepositoryItemTreeListLookUpEdit Properties { get { return (Editor == null ? null : Editor.Properties); } }
		public new TreeListLookUpEdit Editor { get { return base.Editor as TreeListLookUpEdit; } }
		internal static void AddToContainer(string name, RepositoryItemTreeListLookUpEdit properties, IContainer container) {
			if(container == null) return;
			try {
				container.Add(properties.TreeList, name + "TreeList");
			}
			catch {
				try {
					container.Add(properties.TreeList);
				}
				catch {
				}
			}
		}
		#region TreeListLookUpActionList
		[CLSCompliant(false)]
		public class TreeListLookUpActionList : LookUpEditBaseDataBindingActionList {
			public TreeListLookUpActionList(TreeListLookUpEditDesigner designer)
				: base(designer) {
			}
			protected new RepositoryItemTreeListLookUpEdit Properties { get { return (RepositoryItemTreeListLookUpEdit)base.Properties; } }
			protected TreeList TreeList { get { return Properties.TreeList; } }
			protected new TreeListLookUpEditDesigner Designer { get { return (TreeListLookUpEditDesigner)base.Designer; } }
			public override DesignerActionItemCollection GetSortedActionItems() {
				DesignerActionItemCollection res = base.GetSortedActionItems();
				res.Add(new DesignerActionPropertyItem("KeyFieldName", "KeyFieldName"));
				res.Add(new DesignerActionPropertyItem("ParentFieldName", "ParentFieldName"));
				res.Add(new DesignerActionPropertyItem("RootValue", "RootValue"));
				return res;
			}
			[Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design", typeof(UITypeEditor))]
			public string KeyFieldName {
				get { return TreeList.KeyFieldName; }
				set {
					if(KeyFieldName == value) return;
					TreeList.KeyFieldName = value;
					FireChanged();
				}
			}
			[Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design", typeof(UITypeEditor))]
			public string ParentFieldName {
				get { return TreeList.ParentFieldName; }
				set {
					if(ParentFieldName == value) return;
					TreeList.ParentFieldName = value;
					FireChanged();
				}
			}
			[Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor))]
			public object RootValue {
				get { return TreeList.RootValue; }
				set {
					if(RootValue == value) return;
					TreeList.RootValue = value;
					FireChanged();
				}
			}
		}
		#endregion
	}
	[CLSCompliant(false)]
	public class TreeListLookUpEditRepositoryItemDesigner : BaseRepositoryItemDesigner {
		public override ICollection AssociatedComponents {
			get {
				ArrayList controls = new ArrayList(base.AssociatedComponents);
				if(Item != null) controls.Add(Item.TreeList);
				return controls;
			}
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			if(Item != null && !Item.IsLoading && Item.TreeList.Name == "" && Item.TreeList.Site == null)
				AddToContainer();
		}
		protected override void OnInitializeNew(IDictionary defaultValues) {
			base.OnInitializeNew(defaultValues);
			AddToContainer();
		}
		void AddToContainer() {
			IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
			if(host != null && !host.Loading) TreeListLookUpEditDesigner.AddToContainer(Item.Name, Item, host.Container);
		}
		public new RepositoryItemTreeListLookUpEdit Item { get { return base.Item as RepositoryItemTreeListLookUpEdit; } }
	}
	public class TreeListLookUpEditor : UITypeEditor {
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if(context != null && context.Instance != null) return UITypeEditorEditStyle.Modal;
			return base.GetEditStyle(context);
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object objValue) {
			TreeList treeList = objValue as TreeList;
			if(treeList == null || context == null || context.Instance == null || provider == null) return objValue;
			IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
			if(edSvc == null) return objValue;
			frmEditor designer = new frmEditor();
			try {
				treeList.ForceInitialize();
				designer.ShowInTaskbar = true;
				designer.InitEditingObject(treeList);
				edSvc.ShowDialog(designer);
			}
			catch { }
			designer.Dispose();
			return objValue;
		}
	}
}
