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
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Utils.Design;
using DevExpress.XtraBars.Docking;
using System.Windows.Forms.Design.Behavior;
using System.Drawing;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraBars.Docking2010.Views.WindowsUI;
using DevExpress.XtraEditors;
namespace DevExpress.XtraBars.Design {
	public class DocumentManagerGlyphHelper {
		BaseViewGlyph glyphCore;
		Component ownerCore;
		public BaseViewGlyph Glyph { get { return GetGlyph(); } }
		public DocumentManagerGlyphHelper(Component owner) {
			ownerCore = owner;
		}
		protected virtual BaseViewGlyph GetGlyph() {
			DocumentManager owner = (ownerCore as DocumentManager);
			if(owner != null && owner.View is Docking2010.Views.Tabbed.TabbedView && !(glyphCore is TabbedViewGlyph))
				glyphCore = new TabbedViewGlyph(new TabbedViewBehavior(ownerCore));
			if(owner != null && owner.View is Docking2010.Views.WindowsUI.WindowsUIView && !(glyphCore is WindowsUIViewGlyph))
				glyphCore = new WindowsUIViewGlyph(new WindowsUIViewBehavior(ownerCore));
			return glyphCore;
		}
	}
	public class DocumentManagerDesignHost {
		Component component;
		public DocumentManagerDesignHost(Component component) {
			this.component = component;
			Initialize(component);
		}
		public virtual void Initialize(Component component) {
			Site = component.Site;
			SettingsAdorner.Glyphs.Add(new DocumentManagerGlyph(component));
			CreateSmartTag(component);
			component.Disposed += OnComponentDisposed;
		}
		void OnComponentDisposed(object sender, EventArgs e) {
			Component component = sender as Component;
			if(component != null)
				component.Disposed -= OnComponentDisposed;
			Dispose();
		}
		public ISite Site {
			get;
			private set;
		}
		BehaviorService behaviorService;
		public BehaviorService BehaviorService {
			get {
				if(behaviorService == null)
					behaviorService = GetBehaviorService();
				return behaviorService;
			}
		}
		Adorner settingsAdorner;
		protected virtual Adorner SettingsAdorner {
			get {
				if(settingsAdorner == null) {
					settingsAdorner = new Adorner();
				}
				return settingsAdorner;
			}
		}
		public virtual void Dispose() {
			if(component != null)
				component.Disposed -= OnComponentDisposed;
			component = null;
			if(settingsAdorner != null) {
				Glyph[] glyphs = new Glyph[settingsAdorner.Glyphs.Count];
				settingsAdorner.Glyphs.CopyTo(glyphs, 0);
				for(int i = 0; i < glyphs.Length; i++)
					settingsAdorner.Glyphs.Remove(glyphs[i]);
			}
			if(behaviorService != null && settingsAdorner != null)
				behaviorService.Adorners.Remove(settingsAdorner);
			if(behaviorService != null)
				behaviorService.Invalidate();
			behaviorService = null;
			settingsAdorner = null;
			Site = null;
		}
		protected virtual void CreateSmartTag(object component) {
			if(SettingsAdorner.Glyphs.Count > 0) {
				if(!BehaviorService.Adorners.Contains(SettingsAdorner))
					BehaviorService.Adorners.Add(SettingsAdorner);
			}
			else if(BehaviorService.Adorners.Contains(SettingsAdorner))
				BehaviorService.Adorners.Remove(SettingsAdorner);
			BehaviorService.Invalidate();
		}
		protected virtual BehaviorService GetBehaviorService() {
			return (BehaviorService)GetService(typeof(BehaviorService));
		}
		internal object GetService(Type serviceType) {
			return Site.GetService(serviceType);
		}
	}
	public class DocumentManagerGlyph : Glyph {
		DocumentManagerGlyphHelper behaviorHelper;
		public DocumentManagerGlyph(Component component)
			: this(new DocumentManagerGlyphHelper(component)) {
		}
		public DocumentManagerGlyph(DocumentManagerGlyphHelper behaviorHelper)
			: base(null) {
			this.behaviorHelper = behaviorHelper;
		}
		public override Behavior Behavior {
			get {
				return behaviorHelper.Glyph != null ? behaviorHelper.Glyph.Behavior : null;
			}
		}
		public override Cursor GetHitTest(Point p) {
			if(behaviorHelper == null || behaviorHelper.Glyph == null) return null;
			return behaviorHelper.Glyph.GetHitTest(p);
		}
		public override void Paint(PaintEventArgs pe) {
		}
	}
	public class BaseBehavior : Behavior {
		public BaseBehavior(Component component) {
			Component = component;
		}
		protected virtual BehaviorService GetBehaviorService() {
			return (BehaviorService)GetService(typeof(BehaviorService));
		}
		internal object GetService(Type serviceType) {
			return Component.Site.GetService(serviceType);
		}
		BehaviorService behaviorService;
		public BehaviorService BehaviorService {
			get {
				if(behaviorService == null)
					behaviorService = GetBehaviorService();
				return behaviorService;
			}
		}
		public Component Component { get; private set; }
	}
	public class DocumentManagerBehavior : BaseBehavior {
		Point oldMousePosition;
		delegate void MouseAction(ref Point p);
		public DocumentManagerBehavior(Component component) : base(component) { }
		protected virtual bool CanMove { get { return oldMousePosition != Cursor.Position; } }
		protected Point OldMousePosition { get { return oldMousePosition; } }
		public DocumentManager Manager { get { return Component as DocumentManager; } }
		public Docking2010.DragEngine.LayoutElementHitInfo GetHitInfo(Point point) {
			if(Manager == null || Manager.View == null) return null;
			var hitInfo = Manager.CalcHitInfo(point);
			return (hitInfo != null) ? hitInfo.Info : null;
		}
		protected Docking2010.DragEngine.IUIView UIView {
			get {
				if(Manager != null) {
					System.Reflection.PropertyInfo info = Manager.GetType().GetProperty("UIView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
					return info.GetValue(Manager, null) as Docking2010.DragEngine.IUIView;
				}
				return null;
			}
		}
		void OnMouseAction(Point p, MouseButtons button, MouseAction action, Action<MouseEventArgs> uiViewAction) {
			action(ref p);
			uiViewAction(new MouseEventArgs(button, 1, p.X, p.Y, 0));
		}
		public override bool OnMouseDown(Glyph g, MouseButtons button, Point mouseLoc) {
			oldMousePosition = AdornerWindowPointToScreen(mouseLoc);
			if(CanMouseEvent) {
				OnMouseAction(ScreenToClient(oldMousePosition), button, MouseDown, UIView.OnMouseDown);
				return true;
			}
			return false;
		}
		public override bool OnMouseLeave(Glyph g) {
			if(CanMouseEvent) {
				MouseLeave();
				UIView.OnMouseLeave();
				return true;
			}
			return false;
		}
		public override bool OnMouseMove(Glyph g, MouseButtons button, Point mouseLoc) {
			if(CanMouseEvent && CanMove) {
				OnMouseAction(ScreenToClient(Cursor.Position), button, MouseMove, UIView.OnMouseMove);
				return true;
			}
			return false;
		}
		public override bool OnMouseUp(Glyph g, MouseButtons button) {
			if(CanMouseEvent) {
				OnMouseAction(ScreenToClient(Cursor.Position), button, MouseUp, UIView.OnMouseUp);
				return true;
			}
			return false;
		}
		protected virtual bool CanMouseEvent { get { return Manager != null && UIView != null; } }
		protected virtual Point ScreenToClient(Point screenPoint) { return Manager.ScreenToClient(screenPoint); }
		public virtual Point AdornerWindowPointToScreen(Point screenPoint) { return BehaviorService.AdornerWindowPointToScreen(screenPoint); }
		public virtual Point ScreenToAdornerWindow(Point screenPoint) { return BehaviorService.ScreenToAdornerWindow(screenPoint); }
		protected virtual void MouseMove(ref Point clientPoint) { }
		protected virtual void MouseDown(ref Point clientPoint) { }
		protected virtual void MouseUp(ref Point clientPoint) { }
		protected virtual void MouseLeave() { }
	}
	public class BaseViewGlyph {
		public DocumentManagerBehavior Behavior { get; private set; }
		public BaseViewGlyph(DocumentManagerBehavior behaviorSvc) {
			Behavior = behaviorSvc;
		}
		protected virtual bool CanHitTest {
			get { return ValidBehavior && IsParentControlValid && !(Behavior.Manager as IProcessRunningListener).IsRunning; }
		}
		public virtual Cursor GetHitTest(Point p) {
			if(!CanHitTest) return null;
			if(HitTest(Behavior.AdornerWindowPointToScreen(p)))
				return Cursors.Default;
			return null;
		}
		bool HitTest(Point point) {
			return (Behavior == null) || !HitTestControl(point, ParentControl);
		}
		protected virtual bool HitTestControl(Point point, Control control) {
			if(control == null || !control.IsHandleCreated) return false;
			if(control.Parent != null && HitTestControl(point, control.Parent))
				return true;
			Control childControl = control.GetChildAtPoint(control.PointToClient(point));
			if(childControl is DockPanel || childControl is AutoHideControl) return true;
			return false;
		}
		Control ParentControl {
			get {
				if(ValidBehavior)
					return Behavior.Manager.ContainerControl ?? Behavior.Manager.MdiParent;
				return null;
			}
		}
		public virtual Rectangle Bounds {
			get {
				if(ValidBehavior && IsParentControlValid) {
					Control parentContainer = ParentControl.Parent;
					if(parentContainer != null) {
						Point parentLocation = parentContainer.PointToScreen(parentContainer.Location);
						Rectangle parentBounds = new Rectangle(parentLocation, parentContainer.ClientSize);
						Point managerLocation = Behavior.Manager.ClientToScreen(Behavior.Manager.Bounds.Location);
						Rectangle managerBounds = new Rectangle(managerLocation, Behavior.Manager.Bounds.Size);
						Rectangle realBounds = Rectangle.Intersect(managerBounds, parentBounds);
						return new Rectangle(Behavior.ScreenToAdornerWindow(realBounds.Location), realBounds.Size);
					}
				}
				return Rectangle.Empty;
			}
		}
		protected virtual bool IsParentControlValid {
			get { return ParentControl != null && ParentControl.IsHandleCreated; }
		}
		protected virtual bool ValidBehavior {
			get { return (Behavior != null) && (Behavior.Manager != null) && !Behavior.Manager.IsDisposing; }
		}
	}
	public class WindowsUIViewGlyph : BaseViewGlyph {
		public WindowsUIViewGlyph(WindowsUIViewBehavior behaviorSvc) : base(behaviorSvc) { }
		public override Cursor GetHitTest(Point p) {
			if(base.GetHitTest(p) == Cursors.Default) {
				if(Bounds.Contains(p)) {
					return Cursors.Default;
				}
			}
			return null;
		}
	}
	public class TabbedViewGlyph : BaseViewGlyph {
		public TabbedViewGlyph(TabbedViewBehavior behaviorSvc) : base(behaviorSvc) { }
		protected override bool CanHitTest {
			get { return base.CanHitTest && Behavior.Manager.ContainerControl != null; }
		}
		public override Cursor GetHitTest(Point p) {
			if(base.GetHitTest(p) == Cursors.Default) {
				Point point = Behavior.AdornerWindowPointToScreen(p);
				Point screenPoint = Behavior.Manager.ScreenToClient(point);
				Docking2010.DragEngine.LayoutElementHitInfo element = Behavior.GetHitInfo(screenPoint);
				if(element != null) {
					if(element.InHeader || element.InClickBounds) {
						return Cursors.Default;
					}
					if((Behavior as TabbedViewBehavior).CanResizeGroup && element.InBounds) return Cursors.VSplit;
					else
						if(element.InResizeBounds) return Cursors.VSplit;
				}
			}
			return null;
		}
	}
	public class WindowsUIViewBehavior : DocumentManagerBehavior {
		public WindowsUIViewBehavior(Component component) : base(component) { }
		public override bool OnMouseDown(Glyph g, MouseButtons button, Point mouseLoc) {
			base.OnMouseDown(g, button, mouseLoc);
			return false;
		}
		public override bool OnMouseUp(Glyph g, MouseButtons button) {
			base.OnMouseUp(g, button);
			return false;
		}
	}
	public class TabbedViewBehavior : DocumentManagerBehavior {
		bool canResizeGroupCore;
		public TabbedViewBehavior(Component component) : base(component) { }
		internal bool CanResizeGroup { get { return canResizeGroupCore; } }
		protected override void MouseDown(ref Point clientPoint) {
			var hitInfo = GetHitInfo(clientPoint);
			if(hitInfo != null && hitInfo.InResizeBounds) canResizeGroupCore = true;
		}
		protected override void MouseUp(ref Point clientPoint) {
			if(canResizeGroupCore) canResizeGroupCore = false;
		}
		protected override void MouseMove(ref Point clientPoint) {
			if(!canResizeGroupCore) clientPoint = Manager.ClientToScreen(clientPoint);
		}
	}
	public class DocumentManagerDesigner : BaseComponentDesigner {
		IComponentChangeService changeService = null;
		protected Docking2010.DocumentManager Manager {
			get { return Component as Docking2010.DocumentManager; }
		}
#if DXWhidbey
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			OnInitialize();
		}
#else
		public override void OnSetComponentDefaults() {
			base.OnSetComponentDefaults();
			OnInitialize();
		}
#endif
		public override ICollection AssociatedComponents {
			get {
				if(Manager == null)
					return base.AssociatedComponents;
				ArrayList components = new ArrayList();
				foreach(Docking2010.Views.BaseView view in Manager.ViewCollection) {
					components.Add(view);
					foreach(Docking2010.Views.BaseDocument document in view.Documents)
						components.Add(document);
					Docking2010.Views.Tabbed.TabbedView tabbedView = view as Docking2010.Views.Tabbed.TabbedView;
					if(tabbedView != null) {
						foreach(Docking2010.Views.Tabbed.DocumentGroup documentGroup in tabbedView.DocumentGroups)
							components.Add(documentGroup);
					}
					Docking2010.Views.WindowsUI.WindowsUIView windowsUIView = view as Docking2010.Views.WindowsUI.WindowsUIView;
					if(windowsUIView != null) {
						foreach(Docking2010.Views.WindowsUI.BaseTile tile in windowsUIView.Tiles)
							components.Add(tile);
						foreach(Docking2010.Views.WindowsUI.IContentContainer container in windowsUIView.ContentContainers)
							components.Add(container);
					}
#if DEBUGTEST
#endif
				}
				foreach(object obj in base.AssociatedComponents) {
					if(components.Contains(obj)) continue;
					components.Add(obj);
				}
				return components;
			}
		}
		DocumentManagerDesignHost host;
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			var processRunningListener = Manager as Docking2010.IProcessRunningListener;
			if(processRunningListener != null)
				processRunningListener.ProcessExecuting += process_Executing;
			host = new DocumentManagerDesignHost(Manager);
			changeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(changeService != null)
				changeService.ComponentRename += changeService_ComponentRename;
		}
		protected override void Dispose(bool disposing) {
			if(changeService != null)
				changeService.ComponentRename -= changeService_ComponentRename;
			var processRunningListener = Manager as Docking2010.IProcessRunningListener;
			if(processRunningListener != null)
				processRunningListener.ProcessExecuting -= process_Executing;
			if(host != null)
				host.Dispose();
			base.Dispose(disposing);
		}
		void changeService_ComponentRename(object sender, ComponentRenameEventArgs e) {
			Control control = e.Component as Control;
			if(control != null && Manager != null) {
				Docking2010.Views.BaseDocument document = Manager.GetDocument(control);
				if(document != null && document.IsDeferredControlLoad)
					document.ControlName = e.NewName;
			}
		}
		void process_Executing(object sender, Docking2010.ProcessRunningEventArgs e) {
			if(Manager != null && ProjectHelper.IsDebuggerLaunched(Manager)) e.ProcessRunning.IsRunning = true;
			else e.ProcessRunning.IsRunning = false;
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(CreateDocumentManagerActionList());
			base.RegisterActionLists(list);
		}
		protected virtual void OnInitialize() {
			if(Manager == null || Manager.Container == null) return;
			if(Manager.MdiParent != null || Manager.ContainerControl != null || Manager.Container.Components == null) return;
			DesignHelpers.EnsureMDIClient(Manager.Container, Manager);
			ContainerControl container = DesignHelpers.GetContainerControl(Manager.Container);
			Form form = container as Form;
			if(Manager.View == null)
				CreateView(Docking2010.Views.ViewType.Tabbed);
			Manager.BeginUpdate();
			if(form != null && form.IsMdiContainer)
				Manager.MdiParent = form;
			else Manager.ContainerControl = container;
			Manager.EndUpdate();
			if(Manager.MenuManager == null)
				Manager.MenuManager = ControlDesignerHelper.GetBarManager(Manager.Container);
			if(Manager.BarAndDockingController == null)
				Manager.BarAndDockingController = DesignHelpers.FindComponent(Manager.Container, typeof(BarAndDockingController)) as BarAndDockingController;
		}
		protected virtual void OnComponentRanamedInDesigner(object sender, ComponentRenameEventArgs e) {
			DevExpress.XtraBars.Docking2010.Base.INamed namedComponent = e.Component as DevExpress.XtraBars.Docking2010.Base.INamed;
			if(namedComponent != null) {
				namedComponent.Name = e.NewName;
			}
		}
		protected internal Docking2010.Views.BaseView CreateView(Docking2010.Views.ViewType type) {
			Docking2010.Views.BaseView view = Manager.CreateView(type);
			RegisterViewCore(view);
			return view;
		}
		void RegisterViewCore(Docking2010.Views.BaseView view) {
			using(DevExpress.XtraBars.Docking2010.BatchUpdate.Enter(Manager)) {
				((ISupportInitialize)view).BeginInit();
				Manager.ViewCollection.Add(view);
				Manager.View = view;
				((ISupportInitialize)view).EndInit();
			}
		}
		protected virtual DocumentManagerActionList CreateDocumentManagerActionList() {
			return new DocumentManagerActionList(this);
		}
		protected internal void RunDesigner() {
			if(Manager == null) return;
			this.changeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			BeforeStartDesigner();
			IUIService srv = GetService(typeof(IUIService)) as IUIService;
			using(DocumentManagerEditorForm form = new DocumentManagerEditorForm()) {
				form.InitDocumentManager(Manager);
				form.ShowDialog(srv == null ? null : srv.GetDialogOwnerWindow());
			}
			EditorContextHelperEx.RefreshSmartPanel(Component);
			AfterStartDesigner();
		}
		protected internal void BeforeStartDesigner() {
			if(this.changeService != null)
				this.changeService.ComponentRename += OnComponentRanamedInDesigner;
		}
		protected internal void AfterStartDesigner() {
			if(this.changeService != null)
				this.changeService.ComponentRename -= OnComponentRanamedInDesigner;
		}
		public bool SaveLayout() {
			if(Manager == null || Manager.View == null) return false;
			string fileName = GetLayoutFile(false);
			if(string.IsNullOrEmpty(fileName)) return false;
			try {
				Manager.View.SaveLayoutToXml(fileName);
				return true;
			}
			catch { return false; }
		}
		public bool RestoreLayout() {
			if(Manager == null || Manager.View == null) return false;
			DialogResult warningRes = DevExpress.XtraEditors.XtraMessageBox.Show(
				"Do you want to load a layout from an XML file and override the current layout?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
			if(warningRes != DialogResult.Yes) return false;
			string fileName = GetLayoutFile(true);
			if(string.IsNullOrEmpty(fileName)) return false;
			try {
				Manager.View.RestoreLayoutFromXml(fileName);
				return true;
			}
			catch {
				MessageBox.Show("Can't load layout " + fileName, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
		}
		static string GetLayoutFile(bool load) {
			FileDialog fileDialog;
			if(load)
				fileDialog = new OpenFileDialog();
			else
				fileDialog = new SaveFileDialog();
			using(fileDialog) {
				fileDialog.Filter = "Layouts (*.xml)|*.xml";
				fileDialog.Title = (load ? "Restore DocumentManager.View layout" : "Save DocumentManager.View layout");
				fileDialog.CheckFileExists = load;
				if(fileDialog.ShowDialog() == DialogResult.OK) {
					return fileDialog.FileName;
				}
				return null;
			}
		}
		protected internal void AddDockManager() {
			if(Manager == null && Manager.Container == null) return;
			DockManager manager = new DockManager(Manager.Container);
			manager.Form = Manager.MdiParent ?? Manager.ContainerControl;
			if(manager.Form == null && Manager.ClientControl != null)
				manager.Form = GetParentControl(Manager.ClientControl);
			Component.Site.Container.Add(manager);
			EditorContextHelperEx.RefreshSmartPanel(Component);
		}
		ContainerControl GetParentControl(Control control) {
			Control result = null;
			for(result = control; result != null; result = result.Parent) {
				if(result.Parent == null || (result.Parent != null && result.Parent.Site == null))
					return result as ContainerControl;
			}
			return null;
		}
		protected internal void AddWindowMenu() {
			Bar bar = GetMainMenuBar();
			BarDockingMenuItem dockingMenuItem = new BarDockingMenuItem();
			dockingMenuItem.Caption = "Window";
			Component.Site.Container.Add(dockingMenuItem);
			bar.AddItem(dockingMenuItem);
			EditorContextHelperEx.RefreshSmartPanel(Component);
		}
		Bar GetMainMenuBar() {
			Bar bar = BarManager.MainMenu;
			if(bar == null) {
				bar = new Bar();
				bar.DockStyle = BarDockStyle.Top;
				bar.DockRow = 0;
				BarManager.Bars.Add(bar);
				BarManager.MainMenu = bar;
				bar.ApplyDockRowCol();
			}
			return bar;
		}
		protected internal DockManager DockManager {
			get { return DesignHelpers.FindComponent(Manager.Container, typeof(DockManager)) as DockManager; }
		}
		protected internal BarManager BarManager {
			get { return DesignHelpers.FindComponent(Manager.Container, typeof(BarManager)) as BarManager; }
		}
		protected internal bool CanAddWindowMenu() {
			if(BarManager.MainMenu == null) return true;
			Bar bar = BarManager.MainMenu;
			foreach(BarItem item in BarManager.Items) {
				if(item is BarDockingMenuItem)
					return false;
			}
			return true;
		}
	}
	public class DocumentManagerViewCollectionEditor : DXCollectionEditorBase {
		public DocumentManagerViewCollectionEditor(Type type)
			: base(type) {
		}
		protected override ItemTypeInfo[] CreateNewItemTypeInfos() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewSelectorControl));
			ImageList viewsIcon = new ImageList();
			viewsIcon.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("viewsIcon.ImageStream")));
			viewsIcon.TransparentColor = System.Drawing.Color.Transparent;
			viewsIcon.Images.SetKeyName(0, "TabbedView_16x16.png");
			viewsIcon.Images.SetKeyName(1, "NativeMDIView_16x16.png");
			viewsIcon.Images.SetKeyName(2, "MetroView_16x16.png");
			viewsIcon.Images.SetKeyName(3, "WidgetdView_16x16.png");
			return new DXCollectionEditorBase.ItemTypeInfo[]{
				new DXCollectionEditorBase.ItemTypeInfo{Type = typeof(Docking2010.Views.Tabbed.TabbedView), Image = viewsIcon.Images[0]},
				new DXCollectionEditorBase.ItemTypeInfo{ Type = typeof(Docking2010.Views.NativeMdi.NativeMdiView), Image = viewsIcon.Images[1]  },
				new DXCollectionEditorBase.ItemTypeInfo{Type = typeof(Docking2010.Views.WindowsUI.WindowsUIView), Image = viewsIcon.Images[2]},
				new DXCollectionEditorBase.ItemTypeInfo{Type = typeof(Docking2010.Views.Widget.WidgetView), Image = viewsIcon.Images[3]}
			};
		}
	}
	public class WindowsUIButtonCollectionEditor : DXCollectionEditorBase {
		public WindowsUIButtonCollectionEditor(Type type)
			: base(type) {
		}
		protected override Type[] CreateNewItemTypes() {
			return new Type[] { 
				typeof(DevExpress.XtraBars.Docking2010.WindowsUIButton),
				typeof(DevExpress.XtraBars.Docking2010.WindowsUISeparator)
			};
		}
		protected override bool AllowLiveUpdates {
			get { return true; }
		}
	}
	public class TileContainerDesigner : BaseComponentDesigner {
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			(component as ISupportActivation).Activated += TileContainerActivated;
			(component as ISupportActivation).Deactivated += TileContainerDeactivated;
		}
		ITileContainerInfo activeInfo;
		void SubscribeTileContainerInfoEvents(ITileContainerInfo info) {
			this.activeInfo = info;
			info.TileControl.StartItemDragging += StartItemDragging;
			(info as ITileControlUpdateSmartTag).SmartTagUpdate += SmartTagUpdate;
		}
		void UnsubscribeTileContainerInfoEvents(ITileContainerInfo info) {
			info.TileControl.StartItemDragging -= StartItemDragging;
			(info as ITileControlUpdateSmartTag).SmartTagUpdate -= SmartTagUpdate;
			this.activeInfo = null;
		}
		void SmartTagUpdate(object sender, TileControlSmartTagEventArgs e) {
			if(e.Info is TileControlViewInfo)
				ComponentSmartTagProvider.RefreshGlyph(CalcSmartTagBounds(e.TileControl, (e.Info as TileControlViewInfo).Bounds));
			if(e.Info is TileItemViewInfo) {
				IBaseTileSmartTagDesigner baseTileDesigner = GetBaseTileDesigner(sender);
				if(baseTileDesigner != null) baseTileDesigner.OnUpdateSmartTag(CalcSmartTagBounds(e.TileControl, (e.Info as TileItemViewInfo).Bounds));
			}
		}
		void TileContainerActivated(object sender, EventArgs activate) {
			if(sender == null) return;
			ITileContainerInfo info = (sender as TileContainer).Info;
			if(info == null) return;
			SubscribeTileContainerInfoEvents(info);
		}
		void TileContainerDeactivated(object sender, EventArgs activate) {
			if(sender == null) return;
			ITileContainerInfo info = (sender as TileContainer).Info;
			if(info == null) return;
			UnsubscribeTileContainerInfoEvents(info);
		}
		Rectangle CalcSmartTagBounds(ITileControl tileControl, Rectangle bounds) {
			return new Rectangle(tileControl.PointToScreen(bounds.Location), bounds.Size);
		}
		IBaseTileSmartTagDesigner GetBaseTileDesigner(object sender) {
			IComponent component = sender as IComponent;
			if(component.Site != null) {
				IDesignerHost host = component.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;
				if(host != null)
					return host.GetDesigner(component) as IBaseTileSmartTagDesigner;
			}
			return null;
		}
		void StartItemDragging(object sender, TileItemDragEventArgs e) {
			IBaseTileSmartTagDesigner daseTileDesigner = GetBaseTileDesigner((e as BaseTileDragEventArgs).Item);
			if(daseTileDesigner != null) daseTileDesigner.OnStartDragging();
		}
		protected override bool CanUseComponentSmartTags {
			get { return true; }
		}
		protected override void Dispose(bool disposing) {
			if(activeInfo != null) {
				UnsubscribeTileContainerInfoEvents(activeInfo);
				activeInfo = null;
			}
			if(Component != null && (Component is ISupportActivation)) {
				(Component as ISupportActivation).Activated -= TileContainerActivated;
				(Component as ISupportActivation).Deactivated -= TileContainerDeactivated;
			}
			base.Dispose(disposing);
		}
	}
	public interface IBaseTileSmartTagDesigner {
		void OnStartDragging();
		void OnUpdateSmartTag(Rectangle Bounds);
	}
	public class TileDesigner : BaseComponentDesigner, IBaseTileSmartTagDesigner {
		protected override bool CanUseComponentSmartTags {
			get {
				return true;
			}
		}
		#region ITileItemSmartTagDesigner Members
		void IBaseTileSmartTagDesigner.OnStartDragging() {
			ComponentSmartTagProvider.RemoveGlyph();
		}
		void IBaseTileSmartTagDesigner.OnUpdateSmartTag(Rectangle bounds) {
			if(bounds.IsEmpty) return;
			ComponentSmartTagProvider.RefreshGlyph(bounds);
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			base.RegisterActionLists(list);
			list.Add(new TileDesignerActionList(Component));
		}
		class TileDesignerActionList : DesignerActionList {
			public TileDesignerActionList(IComponent component) : base(component) { }
			public override DesignerActionItemCollection GetSortedActionItems() {
				DesignerActionItemCollection res = new DesignerActionItemCollection();
				res.Add(new DesignerActionMethodItem(this, "OnSelectItemTemplate", "Select TileItem Template"));
				return res;
			}
			IComponentChangeService componentChangeService;
			public IComponentChangeService ComponentChangeService {
				get {
					if(componentChangeService == null)
						componentChangeService = ((ITileItem)Component).Control.Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
					return componentChangeService;
				}
			}
			protected virtual void OnSelectItemTemplate() {
				using(TileTemplateSelectorForm form = new TileTemplateSelectorForm((ITileItem)Component)) {
					ComponentChangeService.OnComponentChanging(Component, null);
					BaseDesignerActionListGlyphHelper.HideSmartPanel(Component);
					form.ShowDialog();
					ComponentChangeService.OnComponentChanged(Component, null, null, null);
				}
			}
		}
		#endregion
	}
}
