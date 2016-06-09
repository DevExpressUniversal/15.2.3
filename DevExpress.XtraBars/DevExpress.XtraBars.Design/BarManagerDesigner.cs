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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Reflection;
using System.Runtime.Serialization.Formatters;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Utils.About;
using DevExpress.Utils.Design;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraBars.Localization;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.Design;
using DevExpress.XtraBars.Ribbon.Helpers;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraBars.Styles;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraBars.Utils;
namespace DevExpress.XtraBars.Design {
	public class BarManagerActionList : DesignerActionList {
		BarManagerDesigner designer;
		public BarManagerActionList(BarManagerDesigner designer) : base(designer.Component) {
			this.designer = designer;
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection res = new DesignerActionItemCollection();
			if(Manager.MainMenu == null) 
				res.Add(new DesignerActionMethodItem(this, "CreateMainMenu", "Create MainMenu", "Menu"));
			if(Manager.StatusBar == null)
				res.Add(new DesignerActionMethodItem(this, "CreateStatusBar", "Create StatusBar", "Menu"));
			res.Add(new DesignerActionMethodItem(this, "CreateToolbar", "Create Toolbar", "Toolbar"));
			res.Add(new DesignerActionHeaderItem("Actions"));
			res.Add(new DesignerActionMethodItem(this, "Customize", "Customize", "Actions"));
			res.Add(new DesignerActionMethodItem(this, "RunDesigner", "Run Designer", "Actions"));
			CreateConvertToRibbonItems(res);
			res.Add(new DesignerActionMethodItem(this, "ConvertToolStrip", "Convert Standard Menus", "Actions"));
			if(AllowRemoveUnassignedItems)
				res.Add(new DesignerActionMethodItem(this, "RemoveUnassignedItems", "Remove Unassigned Items", "Actions"));
			return res;
		}
		protected BarManagerDesigner Designer { get { return designer; } }
		protected BarManager Manager { get { return Component as BarManager; } }
		protected virtual void CreateConvertToRibbonItems(DesignerActionItemCollection actionItems) {
			actionItems.Add(new DesignerActionHeaderItem("Ribbon"));
			actionItems.Add(new DesignerActionMethodItem(this, "ConvertToRibbon", "Convert to Ribbon", "Ribbon"));
		}
		protected void ConvertToRibbon() {
			Designer.ConvertToRibbon();
		}
		protected void ConvertToolStrip() {
			Designer.CheckToolStrip();
		}
		protected bool AllowRemoveUnassignedItems {
			get {
				List<BarItem> list = GetUnassignedItems();
				return (list != null && list.Count > 0);
			}
		}
		public virtual List<BarItem> GetUnassignedItems() {
			BarManager manager = (BarManager)Component;
			List<BarItem> list = new List<BarItem>();
			foreach(BarItem item in manager.Items)
				list.Add(item);
			foreach(Bar bar in manager.Bars)
				foreach(BarItemLink link in bar.ItemLinks)
					list.Remove(link.Item);
			foreach(object obj in manager.Container.Components) {
				BarLinksHolder holder = obj as BarLinksHolder;
				if(holder != null)
					foreach(BarItemLink link in holder.ItemLinks)
						list.Remove(link.Item);
			}
			return list;
		}
		public virtual void RemoveUnassignedItems() {
			if(XtraMessageBox.Show("Do you want remove all unassigned items?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
			BarManager manager = Component as BarManager;
			foreach(BarItem item in GetUnassignedItems()) {
				manager.Items.Remove(item);
				item.Dispose();
			}
			DesignHelpers.RefreshSmartTag(Component);
		}
		[RefreshProperties(RefreshProperties.All)]
		public void CreateMainMenu() {
			Bar bar = new Bar();
			bar.DockStyle = BarDockStyle.Top;
			Manager.Bars.Add(bar);
			Manager.MainMenu = bar;
			bar.ApplyDockRowCol();
			EditorContextHelperEx.RefreshSmartPanel(Component);
		}
		[RefreshProperties(RefreshProperties.All)]
		public void CreateStatusBar() {
			Bar bar = new Bar();
			bar.DockStyle = BarDockStyle.Bottom;
			Manager.Bars.Add(bar);
			Manager.StatusBar = bar;
			bar.ApplyDockRowCol();
			EditorContextHelperEx.RefreshSmartPanel(Component);
		}
		public void Customize() {
			Designer.Customize();
		}
		public void RunDesigner() {
			Designer.RunDesigner();
		}
		public void CreateToolbar() {
			Bar bar = Manager.CreateDesignTimeToolbar();
			bar.DockStyle = BarDockStyle.Top;
			Manager.Bars.Add(bar);
			bar.ApplyDockRowCol();
		}
		public override bool AutoShow {
			get {
				return true;
			}
			set {
				AutoShowCore = value;
			}
		}
		protected bool AutoShowCore {
			get {
				return base.AutoShow;
			}
			set {
				base.AutoShow = value;
			}
		}
	}
	public class BarManagerDesigner : BaseComponentDesigner, IBarManagerDesigner, IBarCommandSupports, IKeyCommandProcessInfo {
		DesignerVerbCollection verbs = null;
		IComponentChangeService changeService;
		protected DevExpress.XtraBars.BarManager Manager { get { return Component as BarManager; } }
		protected IBarManagerControl BControl { get { return Component as IBarManagerControl; } }
		public BarManagerDesigner() {
			PropertyStore ps = new PropertyStore(BarEditorForm.BarSettings);
			ps.Restore();
			this.showDesignTimeEnhancements = ps.RestoreBoolProperty ("ShowDesignTimeEnhancements", this.showDesignTimeEnhancements);			
		}		
		protected virtual void ComponentRemoved(object sender, ComponentEventArgs e) {
			PopupControl popupControl = e.Component as PopupControl;
			if(popupControl == null) return;
			ClearReferencesToPopupControl(popupControl);
		}
		void ClearReferencesToPopupControl(PopupControl popup) {
			if(Manager.Items == null) return;
			foreach(var barItem in Manager.Items) {
				BarButtonItem button = barItem as BarButtonItem;
				if(button != null && button.DropDownControl == popup) button.DropDownControl = null;
			}
		}
#if DXWhidbey
		protected override bool AllowHookDebugMode { get { return true; } }
#endif
		protected override bool AllowEditInherited {
			get {
				return false;
			}
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			ISelectionService ss = (ISelectionService)GetService(typeof(ISelectionService));
			if (ss != null) SelectionService = ss;
			Manager.StartCustomization += new EventHandler(OnStartCustomization);
			Manager.EndCustomization += new EventHandler(OnEndCustomization);
			DesignerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
			SetManagerPopupMenu();
			LoaderPatcherService.InstallService(DesignerHost);
			this.changeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(changeService != null) changeService.ComponentRemoved += ComponentRemoved;
			keyCommandProcessHelper = new DesignTimeKeyCommandProcessHelperBase(this);
		}
		DesignTimeKeyCommandProcessHelperBase keyCommandProcessHelper;
		void SetManagerPopupMenu() {
			foreach(object menu in Component.Site.Container.Components) {
				PopupControl popupControl = menu as PopupControl;
				if(popupControl != null && popupControl.Manager == null)
					popupControl.Manager = Manager;
			}
		}
#if DXWhidbey
		protected override void OnDebuggingStateChanged() {
			base.OnDebuggingStateChanged();
			if(!DebuggingState) {
				OnActivated(this, EventArgs.Empty);
				foreach(Bar bar in Manager.Bars) {
					bar.ForceUpdateBar();	
				}
			}
			if(DebuggingState) Manager.HideCustomization();
		}
#endif
		protected void OnActivated(object sender, EventArgs e) {
			if(BControl != null) BControl.Activate();
		}
		protected void OnDeactivated(object sender, EventArgs e) {
			if(BControl != null) BControl.Deactivate();
		}
		protected internal void ConvertToRibbon() {
			if(DesignerHost == null || DesignerHost.Container == null || Manager.Form == null) return;
			if(MessageBox.Show("The existing bars will be converted to new Ribbon controls and then deleted. Do you want to continue?\n\nWarning: You will not be able to convert the Ribbon interface back to the standard bars model.", "Warning",  MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
#if DXWhidbey
			IDesignerLoaderService loader = DesignerHost.GetService(typeof(IDesignerLoaderService)) as IDesignerLoaderService;
#endif
			RibbonConverter cvt = new RibbonConverter();
			RibbonControl ribbon = new RibbonControl();
			Manager.Form.Controls.Add(ribbon);
			DesignerHost.Container.Add(ribbon);
			cvt.ConvertToRibbon(Manager, ribbon);
#if DXWhidbey
			if(loader != null) loader.Reload();
#endif
		}
		IDesignerHost dhost = null;
		protected IDesignerHost DesignerHost {
			get { return dhost; }
			set {
				if(DesignerHost == value) return;
				if(dhost != null) {
					dhost.Deactivated -= new EventHandler(OnDeactivated);
					dhost.Activated -= new EventHandler(OnActivated);
					dhost.LoadComplete -= new EventHandler(OnLoadComplete);
				}
				dhost = value;
				if(dhost != null) {
					dhost.Deactivated += new EventHandler(OnDeactivated);
					dhost.Activated += new EventHandler(OnActivated);
					dhost.LoadComplete += new EventHandler(OnLoadComplete);
				}
			}
		}
		protected void OnLoadComplete(object sender, EventArgs e) {
			if(BControl != null) BControl.DesignerHostLoaded();
		}
		ISelectionService service = null;
		protected ISelectionService SelectionService {
			get { return service; }
			set {
				if(SelectionService == value) return;
				if(SelectionService != null) {
					SelectionService.SelectionChanged -= new EventHandler(OnSelectionChanged); 
				}
				service = value;
				if(SelectionService != null) {
					SelectionService.SelectionChanged += new EventHandler(OnSelectionChanged); 
				}
			}
		}
		static CommandID[] toDisableCommands = 
			new CommandID[] { StandardCommands.Delete, StandardCommands.Paste, StandardCommands.Copy, StandardCommands.Cut,
								StandardCommands.SelectAll, StandardCommands.Undo,
								MenuCommands.KeyMoveDown, MenuCommands.KeyCancel, MenuCommands.KeyMoveLeft,
								MenuCommands.KeyMoveRight, MenuCommands.KeyMoveUp, MenuCommands.KeyDefaultAction,
								MenuCommands.KeySelectNext, MenuCommands.KeySelectPrevious, MenuCommands.KeyReverseCancel};
		void DisableCommands() {
			IMenuCommandService menu = (IMenuCommandService)GetService(typeof(IMenuCommandService));
			if(menu != null) {
				foreach(CommandID cmdId in toDisableCommands) {
					MenuCommand cmd = menu.FindCommand(cmdId);
					if(cmd != null)
						cmd.Enabled = false;
				}
			}
		}
		void OnStartCustomization(object sender, EventArgs e) {
		}
		void OnEndCustomization(object sender, EventArgs e) {
		}
		protected override void Dispose(bool disposing) {		   
			LoaderPatcherService.UnInstallService(DesignerHost);
			SelectionService = null;
			DesignerHost = null;
			if(disposing) {
				PropertyStore ps = new PropertyStore(BarEditorForm.BarSettings);
				ps.AddProperty("ShowDesignTimeEnhancements", this.showDesignTimeEnhancements);
				ps.Store();
				if(Manager != null) {
					Manager.StartCustomization -= OnStartCustomization;
					Manager.EndCustomization -= OnEndCustomization;
				}
				if(KeyCommandProcessHelper != null) 
					KeyCommandProcessHelper.Dispose();
			}
			if(changeService != null)
				changeService.ComponentRemoved -= ComponentRemoved;
			base.Dispose(disposing);
		}
		protected void LayoutChanged() {
			MethodInfo mi = Manager.GetType().GetMethod("DesignerLayoutChanged", BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Instance);
			if(mi != null) mi.Invoke(Manager, null);
		}
		private void OnSelectionChanged(object sender, EventArgs e) {
			if(Manager == null || DebuggingState || IsDebuggingStateChanging) return;
			ISelectionService ss = (ISelectionService)sender;
			if(IsCustomizing) {
				DisableCommands();
			}
		}
		protected bool IsCustomizing {
			get {
				if(Manager != null && Manager.InternalGetService(typeof(DevExpress.XtraBars.Customization.CustomizationForm)) != null)
					return true;
				return false;
			}
		}
		bool showDesignTimeEnhancements = true;
		public bool ShowDesignTimeEnhancements { 
			get { return showDesignTimeEnhancements; }
			set {
				if(ShowDesignTimeEnhancements == value) return;
				showDesignTimeEnhancements = value;
				OnShowDesignTimeEnhancementsChanged();
			}
		}
		protected void OnShowDesignTimeEnhancements(object sender, EventArgs e) {
			ShowDesignTimeEnhancements = !ShowDesignTimeEnhancements;
			UpdateEnhancementsVerb();
		}
		protected virtual void OnShowDesignTimeEnhancementsChanged() {
			UpdateEnhancementsVerb();
			LayoutChanged();
		}
		bool IBarManagerDesigner.AllowDesignTimeEnhancements { 
			get {
				return ShowDesignTimeEnhancements;
			}
		}
		bool IBarManagerDesigner.DebuggingState {
			get {
				return DebuggingState;
			}
		}
		protected void UpdateEnhancementsVerb() {
			if(this.verbs == null) return;
			this.verbs[3].Checked = ShowDesignTimeEnhancements;
		}
		protected virtual void CreateVerbs() {
			this.verbs = new DesignerVerbCollection(new DesignerVerb[] {
				new DesignerVerb("About", new EventHandler(OnAboutClick)), 
				new DesignerVerb("Customize", new EventHandler(OnCustomizeClick)), 
				new DesignerVerb("Designer", new EventHandler(OnEditorClick)), 
				new DesignerVerb("Show DesignTime enhancements", new EventHandler(OnShowDesignTimeEnhancements)),
				new DesignerVerb("Convert Standard Menus", new EventHandler(OnConvertToolStrip))
																											});
			CreateConvertToRibbonVerb();
			if(Manager != null && Manager.Form != null && MainMenu != null && MainMenu.MenuItems.Count > 0)
				verbs.Add(new DesignerVerb("Import from MainMenu", new EventHandler(OnImportClick)));
			UpdateEnhancementsVerb();
		}
		void OnConvertToolStrip(object sender, EventArgs e) {
			CheckToolStrip();
		}
		protected virtual void CreateConvertToRibbonVerb() {
			verbs.Add(new DesignerVerb("Convert to RibbonControl", new EventHandler(OnRibbonConvert)));
		}
		public override DesignerVerbCollection DXVerbs {
			get { 
				if(this.verbs == null) CreateVerbs();
				return verbs;
			}
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(CreateBarManagerActionList());
			base.RegisterActionLists(list);
		}
		protected virtual BarManagerActionList CreateBarManagerActionList() {
			return new BarManagerActionList(this);
		}
		void OnCustomizeClick(object sender, EventArgs e) {
			Customize();
		}
		void OnRibbonConvert(object sender, EventArgs e) {
			ConvertToRibbon();
		}
		void OnEditorClick(object sender, EventArgs e) {
			RunDesigner();
		}
		protected internal void RunDesigner() {
			if(Manager == null) return;
			IUIService srv = GetService(typeof(IUIService)) as IUIService;
			BarEditorForm form = CreateBarEditorForm();
			form.InitEditingObject(Manager);
			form.ShowDialog(srv == null ? null : srv.GetDialogOwnerWindow());
			form.Dispose();
		}
		protected virtual BarEditorForm CreateBarEditorForm() {
			return new BarEditorForm();
		}
		protected internal void Customize() {
			if(Manager == null) return;
			Manager.Customize();
			ISelectionService ss = (ISelectionService)GetService(typeof(ISelectionService));
			if(ss != null) {
			}
		}
		RepositoryDesigner repositoryEditor;
		void OnRepositoryEditorClick(object sender, EventArgs e) {
			if(Manager == null) return;
			if(repositoryEditor == null) {
				repositoryEditor = new RepositoryDesigner();
				repositoryEditor.Closed += new EventHandler(editorClosed);
			}
			repositoryEditor.InitRepositoryItems(Manager.RepositoryItems);
			repositoryEditor.ShowDialog();
		}
		protected void editorClosed(object sender, EventArgs e) {
			repositoryEditor = null;
		}
		void OnImportClick(object sender, EventArgs e) {
			MainMenu menu = MainMenu;
			if(Manager == null || Manager.Form == null || menu == null) return;
			if(MessageBox.Show(null, "Do you really want to import the current MainMenu into the BarManager ?", "BarManager Designer", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
			try {
				DoConvert(menu);
			}
			catch(Exception ex) {
				MessageBox.Show(null, "Some errors have occurred while importing (" + ex.Message + ")", "BarManager Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
		}
		void DoConvert(MainMenu menu) {
			Bar bar = new Bar(Manager);
			bar.Visible = false;
			int index = 0;
			string name;
			while(Manager.Bars[name = "GeneratedMainMenu" + (index > 0 ? index.ToString() : "")] != null) { index ++; }
			bar.BarName = name;
			CreateMenus(Manager, bar, null, menu.MenuItems);
			bar.DockStyle = BarDockStyle.Top;
			bar.DockRow = 0;
			bar.DockCol = 0;
			Manager.MainMenu = bar;
			bar.Visible = true;
		}
		static internal void CreateMenus(BarManager manager, BarLinksHolder bar, BarSubItem subItem, System.Windows.Forms.Menu.MenuItemCollection menuItems) {
			IDesignerHost host = manager.InternalGetService(typeof(IDesignerHost)) as IDesignerHost; 
			if(host == null) throw new Exception("Internal error");
			DesignerTransaction trans = host.CreateTransaction("BarManagerLayout");
			try {
				InternalCreateMenus(manager, bar, subItem, menuItems);
			}
			catch {
				trans.Cancel();
				return;
			}
			trans.Commit();
		}
		static internal void InternalCreateMenus(BarManager manager, BarLinksHolder bar, BarSubItem subItem, System.Windows.Forms.Menu.MenuItemCollection menuItems) {
			bool shouldBeginGroup = false;
			foreach(MenuItem menuItem in menuItems) {
				CreateMenuItem(manager, bar, subItem, menuItem, ref shouldBeginGroup);
			}
		}
		static internal void CreateMenuItem(BarManager manager, BarLinksHolder bar, BarSubItem subItem, MenuItem item, ref bool shouldBeginGroup) {
			if(item.Text == "-") {
				shouldBeginGroup = true;
				return;
			}
			BarItem newItem = null;
			BarSubItem newSub = null;
			if(item.MenuItems != null && item.MenuItems.Count > 0) {
				newSub = new BarSubItem();
				newItem = newSub;
			}
			else {
				BarButtonItem newBtn = new BarButtonItem();
				if(item.Checked) {
					newBtn.ButtonStyle = BarButtonStyle.Check;
					newBtn.Down = true;
				}
				newItem = newBtn;
			}
			newItem.Caption = item.Text;
			newItem.ItemShortcut = new BarShortcut(item.Shortcut);
			newItem.Manager = manager;
			newItem.Category = GetCategoryIndex(manager, bar, newSub != null);
			AddToContainer(manager, newItem);
			BarItemLink link = null;
			if(subItem == null)
				link = bar.AddItem(newItem);
			else
				link = subItem.AddItem(newItem);
			link.BeginGroup = shouldBeginGroup;
			shouldBeginGroup = false;
			if(newSub != null)
				InternalCreateMenus(manager, bar, newSub, item.MenuItems);
		}
		static BarManagerCategory GetCategoryIndex(BarManager manager, BarLinksHolder bar, bool isMenu) {
			const string menus = "Built-in Menus", commands = "Commands";
			string catName = (isMenu ? menus : commands);
			if(bar is PopupMenu) {
				PopupMenu popup = bar as PopupMenu;
				catName = popup.Site.Name;
			}
			BarManagerCategory cat = manager.Categories[catName] as BarManagerCategory;
			if(cat != null) return cat;
			return manager.Categories.Add(catName);
		}
		static void AddToContainer(BarManager manager, BarItem item) {
			string temp = item.Caption.Replace("&", ""), name = "";
			for(int n = 0; n < temp.Length; n++) {
				if(char.IsLetterOrDigit(temp[n])) name += temp[n];
			}
			if(item is BarSubItem) name = "sub" + name;
			else name = "btn" + name;
			int index = 0;
			string realName;
			while(manager.Container.Components[realName = name + (index > 0 ? index.ToString() : "")] != null) { index ++; }
			manager.Container.Add(item, realName);
		}
		protected MainMenu MainMenu {
			get {
				if(Manager == null || Manager.Container == null) return null;
				foreach(object obj in Manager.Container.Components) {
					if(obj is MainMenu) return obj as MainMenu;
				}
				return null;
			}
		}
		Assembly ResolveName(object sender, ResolveEventArgs e) {
			if(e.Name.StartsWith("DevExpress.XtraBars")) return this.Manager.GetType().Assembly;
			if(e.Name.StartsWith("DevExpress.XtraEditors.Core")) return typeof(DevExpress.XtraEditors.BaseEdit).Assembly;
			if(e.Name.StartsWith("DevExpress.XtraEditors")) return typeof(DevExpress.XtraEditors.ButtonEdit).Assembly;
			return null;
		}
		public bool RestoreLayout() {
			if(Manager == null) return false;
			object data = null;
			string fname = GetLayoutFile(true);
			if(fname == null) return false;
			try {
				System.IO.FileStream fs = new System.IO.FileStream(fname, System.IO.FileMode.Open, System.IO.FileAccess.Read);
				System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bfm = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
				bfm.AssemblyFormat = FormatterAssemblyStyle.Simple;
				data = bfm.Deserialize(fs);
				if(data == null) return false;
				if(data.ToString() != "BarLayout") {
					MessageBox.Show("Wrong layout file!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return false;
				}
				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(ResolveName);
				try {
					data = bfm.Deserialize(fs);
				}
				finally {
					AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(ResolveName);
				}
			}
			catch {
				MessageBox.Show("Can't load layout " + fname, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
			IDesignerHost host = GetService(typeof(IDesignerHost)) as IDesignerHost; 
			IDesignerSerializationService ser = GetService(typeof(IDesignerSerializationService)) as IDesignerSerializationService;
			if(ser == null || host == null) return false;
			ISelectionService sel = GetService(typeof(ISelectionService)) as ISelectionService;
			if(sel == null) return false;
			sel.SetSelectedComponents(null);
			host.DestroyComponent(Manager);
			if(Manager != null) Manager.Dispose();
			DesignerTransaction trans = host.CreateTransaction("BarManagerLayout");
			ICollection components = null;
			try {
				components = ser.Deserialize(data);
			}
			finally {
				trans.Commit();
			}
			if(components == null || components.Count == 0) return false;
			BarManager loadedManager = null;
			trans = host.CreateTransaction("BarManagerLayout");
			try {
				foreach(object obj in components) {
					IComponent comp = obj as IComponent;
					if(comp == null) continue;
					if(comp is BarManager) loadedManager = comp as BarManager;
					string name = null;
					if(comp.Site != null) name = comp.Site.Name;
					IContainer cont = host.Container;
					if(cont == null) continue;
					name = CheckName(name, cont);
					cont.Add(comp, name);
				}
			}
			finally {
				trans.Commit();
			}
			if(loadedManager != null) {
				loadedManager.Form = DesignHelpers.GetContainerControl(loadedManager.Container);
				loadedManager.ForceLinkCreate();
			}
			return true;
		}
		string CheckName(string name, IContainer cont) {
			foreach(IComponent comp in cont.Components) {
				if(comp.Site != null && comp.Site.Name == name) return null;
			}
			return name;
		}
		void OnRestoreLayout(object sender, EventArgs e) {
			if(RestoreLayout()) {
				MessageBox.Show("Layout restored.");
			}
		}
		string GetLayoutFile(bool load) {
			FileDialog fd;
			if(load)
				fd = new OpenFileDialog();
			else 
				fd = new SaveFileDialog();
			fd.Filter = "XtraBars Layouts (*.XtraLayout)|*.XtraLayout";
			fd.Title = (load ? "Restore XtraBars layout" : "Save XtraBars layout");
			fd.CheckFileExists = load;
			if(fd.ShowDialog() == DialogResult.OK) {
				return fd.FileName;
			}
			return null;
		}
		protected virtual void OnAboutClick(object sender, EventArgs e) {
			BarManager.About();
		}
		void OnSaveLayout(object sender, EventArgs e) {
			SaveLayout();
		}
		public void SaveLayout() {
			if(Manager == null) return;
			string name = GetLayoutFile(false);
			if(name == null) return;
			try {
			ArrayList list = new ArrayList();
			list.Add(Manager);
			if(Manager.Images != null) list.Add(Manager.Images);
			if(Manager.LargeImages != null) list.Add(Manager.LargeImages);
			list.AddRange(AssociatedComponents);
			IDesignerSerializationService ser = GetService(typeof(IDesignerSerializationService)) as IDesignerSerializationService;
			if(ser == null) return;
			object data = ser.Serialize(list);
			System.IO.FileStream fs = new System.IO.FileStream(name, System.IO.FileMode.Create);
			System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bfm = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
			bfm.AssemblyFormat = FormatterAssemblyStyle.Simple;
			bfm.Serialize(fs, "BarLayout");
			bfm.Serialize(fs, data);
			fs.Close();
			MessageBox.Show("Layout saved into " + name, "Info");
		}
			catch(Exception ex) {
				DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, ex.Source);
			}
		}
#if DXWhidbey
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			OnNewComponent();
		}
#else
		public override void OnSetComponentDefaults() {
			base.OnSetComponentDefaults();
			OnNewComponent();
		}
#endif
		protected virtual void OnNewComponent() {
			if(Manager == null || Manager.Container == null) return;
			if(Manager.Form != null || Manager.Container.Components == null) return;
			Manager.BeginUpdate();
			try {
				Manager.DockManager = FindDockManager();
				Manager.Form = DesignHelpers.GetContainerControl(Manager.Container);
				Manager.Controller = FindComponent<BarAndDockingController>();
				CreateDefaultBars();
			}
			finally {
				Manager.EndUpdate();
			}
			var documentManager = FindDocumentManager();
			if(documentManager != null) {
				if(documentManager.MenuManager == null)
					documentManager.MenuManager = Manager;
				if(documentManager.BarAndDockingController == null)
					documentManager.BarAndDockingController = FindComponent<BarAndDockingController>();
			}
		}
		#region Strip
		bool convertContextMenuStrip;
		bool convertToolStrip;
		bool convertStatusStrip;
		bool convertMainMenuStrip;
		bool remove;
		protected internal void CheckToolStrip() {
			ConvertToolStripForm frm = new ConvertToolStripForm();
			frm.ShowDialog();
			if(frm.DialogResult == DialogResult.OK) {
				convertContextMenuStrip = frm.ConvertContextMenuStrip;
				convertToolStrip = frm.ConvertToolStrip;
				convertMainMenuStrip = frm.ConvertMenuStrip;
				convertStatusStrip = frm.ConvertStatusStrip;
				remove = frm.DeleteItems;
				if(convertContextMenuStrip) 
					 CheckContextMenuStrip();
			   List<Control> forRemove = new List<Control>();
				foreach(Control control in Manager.Form.Controls) {
					if(control is ToolStrip)
						if(ConvertToolStrip((ToolStrip)control)) {
							forRemove.Add(control);
						}
				}
				foreach(Control control in forRemove) {
					control.Dispose();
				}
				forRemove.Clear();
			}
		}
		bool ConvertToolStrip(ToolStrip toolStrip){
			bool removed = false;
			if(toolStrip is MenuStrip && toolStrip.Items.Count == 1 && toolStrip.Items[0].Text == "ContextMenuStrip") { return removed; }
			bool isMenuStrip = toolStrip is MenuStrip && convertMainMenuStrip;
			bool isStatusStrip = toolStrip is StatusStrip && convertStatusStrip;
			bool isToolStrip = !isMenuStrip && !isStatusStrip && convertToolStrip;
			if(!isToolStrip && !isMenuStrip && !isStatusStrip) { return removed; }
			Bar bar = new Bar();
			UpdateToolStripItemCollection(bar, toolStrip.Items, null, null);
			Manager.Bars.Add(bar);
			if(isStatusStrip){
				bar.DockStyle = BarDockStyle.Bottom;
				Manager.StatusBar = bar;
			}
			else if(isMenuStrip){
				bar.DockStyle = BarDockStyle.Top;
				Manager.MainMenu = bar;
			}
			else {
				bar.DockStyle = BarDockStyle.Top;
			}
			if(remove) 
				removed = true;
			bar.ApplyDockRowCol();
			EditorContextHelperEx.RefreshSmartPanel(Component);
			return removed;
		}
		protected internal void CheckContextMenuStrip() {
			List<Component> forRemove = new List<Component>();
			foreach(Component component in Manager.Form.Container.Components) {
				if(component is ContextMenuStrip) {
					UpdateContextMenuStrip((ContextMenuStrip)component);
					if(remove && convertContextMenuStrip) forRemove.Add(component);
				}
			}
			foreach(Component component in forRemove) {
				component.Dispose();
			}
			forRemove.Clear();
		}
		void UpdateContextMenuStrip(ContextMenuStrip cmstrip) {
			PopupMenu menu = new PopupMenu();
			menu.Manager = Manager;
			UpdateItemCollection(cmstrip.Items, null, menu);
			Manager.Container.Add(menu);
		}
		void UpdateItemCollection(ToolStripItemCollection collection, BarSubItem ownerItem, PopupMenu menu) {
			bool drawDropDownSeparator = false;
			foreach(ToolStripItem item in collection) {
				BarItem barItem;
				if(item is ToolStripComboBox) {
					RepositoryItemComboBox comboBox = new RepositoryItemComboBox();
					barItem = new BarEditItem();
					(barItem as BarEditItem).Edit = comboBox;
					AssignComboBoxItem((BarEditItem)barItem, (ToolStripComboBox)item);
				}
				else if(item is ToolStripTextBox) {
					RepositoryItemTextEdit textEdit = new RepositoryItemTextEdit();
					barItem = new BarEditItem();
					((BarEditItem)barItem).Edit = textEdit;
					AssignTextEditItem((BarEditItem)barItem, (ToolStripTextBox)item);
				}
				else if(item is ToolStripSeparator) { drawDropDownSeparator = true; continue; }
				else if(item is ToolStripDropDownItem) {
					if(((ToolStripDropDownItem)item).DropDownItems.Count != 0) {
						barItem = System.Activator.CreateInstance(typeof(BarSubItem)) as BarItem;
						UpdateItemCollection(((ToolStripDropDownItem)item).DropDownItems, (BarSubItem)barItem, null);
					}
					else {
						barItem = new BarButtonItem();
					}
					UpdateItemProperties(barItem, item);
				}
				else { continue; }
				AddBarItem(barItem, null, menu, ownerItem, ref drawDropDownSeparator);
			}
		}
		void UpdateToolStripItemCollection(Bar bar, ToolStripItemCollection items, BarSubItem ownerItem, PopupMenu menu) {
			bool needBeginGroup = false;
			foreach(ToolStripItem tItem in items) {
				BarItem bItem;
				bItem = new BarEditItem();
				if(tItem is ToolStripTextBox) {
					RepositoryItemTextEdit textEdit = new RepositoryItemTextEdit();
					((BarEditItem)bItem).Edit = textEdit;
					AssignTextEditItem((BarEditItem)bItem, (ToolStripTextBox)tItem);
				}
				else if(tItem is ToolStripComboBox) {
					RepositoryItemComboBox comboBox = new RepositoryItemComboBox();
					((BarEditItem)bItem).Edit = comboBox;
					AssignComboBoxItem((BarEditItem)bItem, (ToolStripComboBox)tItem);
				}
				else if(tItem is ToolStripProgressBar) {
					RepositoryItemProgressBar progressBar = new RepositoryItemProgressBar();
					((BarEditItem)bItem).Edit = progressBar;
					AssignProgressBarItem((BarEditItem)bItem, (ToolStripProgressBar)tItem);
				}
				else if(tItem is ToolStripSeparator) {
					needBeginGroup = true;
					continue;
				}
				else {
					if(tItem is ToolStripButton) {
						bItem = new BarButtonItem();
					}
					else if(tItem is ToolStripLabel) {
						bItem = new BarStaticItem();
						bItem.Border = XtraEditors.Controls.BorderStyles.NoBorder;
					}
					else if(tItem is ToolStripDropDownButton) {
						if(((ToolStripDropDownButton)tItem).DropDownItems.Count == 0) {
							bItem = new BarButtonItem();
						}
						else {
							bItem = new BarSubItem();
							UpdateItemCollection(((ToolStripDropDownButton)tItem).DropDownItems, (BarSubItem)bItem, null);
						}
					}
					else if(tItem is ToolStripSplitButton) {
						bItem = new BarButtonItem();
						AddPopupMenu(bItem, tItem as ToolStripDropDownItem);
					}
					else if(tItem is ToolStripMenuItem) {
						bItem = new BarButtonItem();
						AddPopupMenu(bItem, tItem as ToolStripDropDownItem);
						((BarButtonItem)bItem).ActAsDropDown = true;
						bItem.Caption = tItem.Text;
					}
					else { continue; }
					UpdateItemProperties(bItem, tItem);
				}
				AddBarItem(bItem, bar, menu, ownerItem, ref needBeginGroup);
			}
		}
		void AddPopupMenu(BarItem item, ToolStripDropDownItem tItem) {
			PopupMenu menuDropDown = new PopupMenu();
			menuDropDown.Manager = Manager;
			UpdateItemCollection(((ToolStripDropDownItem)tItem).DropDownItems, null, menuDropDown); 
			Manager.Container.Add(menuDropDown);
			((BarButtonItem)item).DropDownControl = menuDropDown;
			((BarButtonItem)item).ButtonStyle = BarButtonStyle.DropDown;
		}
		void AddBarItem(BarItem item, Bar bar, PopupMenu menu, BarSubItem ownerItem, ref bool beginGroup) {
			BarItemLink link;
			Manager.Container.Add(item);
			Manager.Items.Add(item);
			if(bar != null) link = bar.AddItem(item);
			else if(menu != null) link = menu.AddItem(item);
			else link = ownerItem.AddItem(item);
			if(beginGroup) {
				link.BeginGroup = true;
				beginGroup = false;
			}
		}
		void UpdateItemProperties(BarItem bItem, ToolStripItem tItem) {
			bItem.Caption = tItem.Text;
			bItem.Glyph = tItem.Image;
			if(tItem.DisplayStyle == ToolStripItemDisplayStyle.Image) bItem.PaintStyle = BarItemPaintStyle.CaptionInMenu;
			else if(tItem.DisplayStyle == ToolStripItemDisplayStyle.ImageAndText) bItem.PaintStyle = BarItemPaintStyle.CaptionGlyph;
			else if(tItem.DisplayStyle == ToolStripItemDisplayStyle.Text) bItem.PaintStyle = BarItemPaintStyle.Caption;
			else if(tItem.DisplayStyle == ToolStripItemDisplayStyle.None) bItem.PaintStyle = BarItemPaintStyle.Standard;
			if(tItem.Alignment == ToolStripItemAlignment.Left) bItem.Alignment = BarItemLinkAlignment.Left;
			else if(tItem.Alignment == ToolStripItemAlignment.Right) bItem.Alignment = BarItemLinkAlignment.Right;
			bItem.Enabled = tItem.Enabled;
			bItem.AccessibleDescription = tItem.AccessibleDescription;
			bItem.AccessibleName = tItem.AccessibleName;
			bItem.Tag = tItem.Tag;
			bItem.Hint = tItem.ToolTipText;
		}
		void AssignTextEditItem(BarEditItem bItem, ToolStripTextBox tItem) { 
			UpdateItemProperties(bItem, tItem);
			bItem.EditValue = tItem.Text;
			bItem.Caption = "";
		}
		void AssignComboBoxItem(BarEditItem bItem, ToolStripComboBox tItem) { 
			bItem.Caption = tItem.Text;
			UpdateItemProperties(bItem, tItem);
			UpdateComboBoxItems((bItem.Edit as RepositoryItemComboBox).Items, tItem.Items);
		}
		void UpdateComboBoxItems(XtraEditors.Controls.ComboBoxItemCollection comboBoxItemCollection, System.Windows.Forms.ComboBox.ObjectCollection objectCollection) {
			foreach(object obj in objectCollection) {
				comboBoxItemCollection.Add(obj);
			}
		}
		void AssignProgressBarItem(BarEditItem bItem, ToolStripProgressBar tItem) {
			bItem.EditValue = tItem.Value;
			UpdateItemProperties(bItem, tItem);
		}
		#endregion
		protected virtual DockManager FindDockManager() {
			return FindComponent<DockManager>();
		}
		protected virtual Docking2010.DocumentManager FindDocumentManager() {
			return FindComponent<Docking2010.DocumentManager>();
		}
		T FindComponent<T>() where T : Component {
			return DesignHelpers.FindComponent(Manager.Container, typeof(T)) as T;
		}
		protected virtual void CreateDefaultBars() {
			Bar mainMenu = new Bar(), statusBar = new Bar(), bar = new Bar();
			mainMenu.Text = BarLocalizer.Active.GetLocalizedString(BarString.NewMenuName);
			mainMenu.BarName = mainMenu.Text;
			mainMenu.DockStyle = BarDockStyle.Top;
			bar.DockStyle = BarDockStyle.Top;
			bar.Text = BarLocalizer.Active.GetLocalizedString(BarString.NewToolbarName);
			bar.BarName = bar.Text;
			statusBar.DockStyle = BarDockStyle.Bottom;
			statusBar.Text = BarLocalizer.Active.GetLocalizedString(BarString.NewStatusBarName);
			statusBar.BarName = statusBar.Text;
			Manager.Bars.Add(bar);
			Manager.Bars.Add(mainMenu);
			Manager.Bars.Add(statusBar);
			Manager.StatusBar = statusBar;
			Manager.MainMenu = mainMenu;
		}
		public override ICollection AssociatedComponents {
			get {
				if(Manager == null) return base.AssociatedComponents;
				ArrayList controls = new ArrayList();
				controls.AddRange(Manager.RepositoryItems);
				controls.AddRange(Manager.DockControls);
				controls.AddRange(Manager.Items);
				controls.AddRange(Manager.Bars);
				AddBase(controls);
				return controls;
			}
		}
		void AddBase(ArrayList controls) {
			foreach(object obj in base.AssociatedComponents) {
				if(controls.Contains(obj)) continue;
				controls.Add(obj);
			}
		}
		#region IKeyCommandProcessInfo
		IServiceProvider IKeyCommandProcessInfo.ServiceProvider {
			get { return Component.Site; }
		}
		BaseDesignTimeManager IKeyCommandProcessInfo.DesignTimeManager {
			get { return null; }
		}
		#endregion
		#region IBarCommandSupports
		BarCommandContextBase IBarCommandSupports.CommandContext {
			get { return new BarManagerCommandContext(Manager); }
		}
		#endregion
		protected DesignTimeKeyCommandProcessHelperBase KeyCommandProcessHelper { get { return keyCommandProcessHelper; } }
	}
	public class PopupControlContainerDesigner : BaseScrollableControlDesigner {
		public PopupControlContainerDesigner() { }
		protected override bool CanUseComponentSmartTags { get { return true; } }
		PopupControlContainer ControlContainer { get { return Component as PopupControlContainer; } }
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
		DesignerActionListCollection listCore;
		public override DesignerActionListCollection ActionLists {
			get {
				if(listCore == null) {
					listCore = new DesignerActionListCollection();
					listCore.Add(new PopupControlContainerActionList(this));
					listCore.AddRange(base.ActionLists);
				}
				return listCore;
			}
		}
		protected virtual void OnInitialize() {
			if(ControlContainer == null || ControlContainer.Container == null) return;
			if(ControlContainer == null || ControlContainer.Container == null) return;
			if(ControlContainer.Ribbon == null && ControlContainer.Manager == null) ControlContainer.Ribbon = DesignHelpers.GetRibbon(ControlContainer.Container);
			if(ControlContainer.Ribbon == null && ControlContainer.Manager == null) ControlContainer.Manager = DesignHelpers.GetBarManager(ControlContainer.Container);
		}
		protected internal bool IsControlContainerReadyToUse {
			get {
				if(Component == null || Component.Site == null)
					return false;
				ISite site = Component.Site;
				return BarDesignTimeUtils.IsReferenceExists<BarManager>(site) || BarDesignTimeUtils.IsReferenceExists<RibbonControl>(site);
			}
		}
		protected override void OnPaintAdornments(PaintEventArgs pe) {
			base.OnPaintAdornments(pe);
			if(IsControlContainerReadyToUse)
				return;
			DrawWarningMessage(pe);
		}
		protected virtual void DrawWarningMessage(PaintEventArgs pe) {
			using(StringFormat format = new StringFormat()) {
				format.Alignment = format.LineAlignment = StringAlignment.Center;
				RectangleF rect = new RectangleF(Point.Empty, ControlContainer.Size);
				using(SolidBrush br = new SolidBrush(ControlContainer.ForeColor)) {
					pe.Graphics.DrawString(WarningMessageCore, AdornmentFont, br, rect, format);
				}
			}
		}
		protected virtual string WarningMessageCore {
			get { return "The PopupControlContainer cannot function when it's not bound either to a BarManager or RibbonControl. Please add a BarManager via the smart tag (or Toolbox) or drop a RibbonControl via the Toolbox."; }
		}
		Font adornmentFontCore = null;
		protected Font AdornmentFont {
			get {
				if(adornmentFontCore == null) {
					adornmentFontCore = CreateAdornmentFontCore();
				}
				return adornmentFontCore;
			}
		}
		protected virtual Font CreateAdornmentFontCore() {
			return new Font(ControlContainer.Font.FontFamily, 10);
		}
		protected override void PostFilterProperties(IDictionary properties) {
			properties["Manager"] = TypeDescriptor.GetProperties(this)["Manager"];
			base.PostFilterProperties(properties);
		}
		RibbonControl Ribbon { get { return ControlContainer.Ribbon; } }
		[DefaultValue(null), RefreshProperties(RefreshProperties.All), Category("Manager")]
		public BarManager Manager {
			get {
				if(Ribbon != null) return null;
				return ControlContainer.Manager;
			}
			set { ControlContainer.Manager = value; }
		}
	}
	public class PopupControlContainerActionList : DesignerActionList {
		PopupControlContainerDesigner designer;
		public PopupControlContainerActionList(PopupControlContainerDesigner designer)
			: base(designer.Component) {
			this.designer = designer;
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection res = new DesignerActionItemCollection();
			if(!Designer.IsControlContainerReadyToUse) {
				res.Add(new DesignerActionHeaderItem("Actions", "Actions"));
				res.Add(new DesignerActionMethodItem(this, "AddBarManager", "Add BarManager", "Actions"));
			}
			return res;
		}
		public void AddBarManager() {
			BarManager barManager = BarDesignTimeUtils.CreateDefaultBarManager(Component);
			ControlContainer.Manager = barManager;
			FireChanged(ControlContainer);
			EditorContextHelperEx.RefreshSmartPanel(Component);
		}
		protected void FireChanged(IComponent component) {
			IComponentChangeService srv = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(srv != null) srv.OnComponentChanged(component, null, null, null);
		}
		public override bool AutoShow {
			get {
				if(!Designer.IsControlContainerReadyToUse)
					return true;
				return base.AutoShow;
			}
			set { base.AutoShow = value; }
		}
		public PopupControlContainerDesigner Designer { get { return designer; } }
		public PopupControlContainer ControlContainer { get { return Component as PopupControlContainer; } }
	}
	public class AppPopupMenuDesigner : PopupMenuDesigner {
		protected override bool AllowManagerProperty { get { return false; } }
		protected override PopupMenuDesignerActionListBase GetActionList(IComponent component) {
			return new AppMenuDesignerActionList(component);
		}
		protected override bool AllowCustomize { get { return false; } }
	}
	public class BarSubItemDesigner : BaseComponentDesigner {
		protected override bool AllowInheritanceWrapper { get { return true; } }
	}
	public class PopupMenuDesigner : PopupMenuBaseDesigner { 
	}
	public class RadialMenuDesigner : PopupMenuBaseDesigner {
		protected IComponentChangeService srvc;
		protected IDesignerHost host;
		protected IComponentChangeService GetSrvc {
			get {
				if(srvc == null) srvc = (IComponentChangeService)GetService(typeof(IComponentChangeService));
				return srvc;
			}
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			SubscribeEvents();
		}
		DesignerActionListCollection list = null;
		public override DesignerActionListCollection ActionLists {
			get {
				if(list == null) {
					this.list = base.ActionLists;
					this.list.Add(new RadialMenuActionListDesigner(Component));
				}
				return list;
			}
		}
		protected void OnDesignerHostDeactivated(object sender, EventArgs e) {
			RadialMenu.HideInDesigner();
		}
		protected override void Dispose(bool disposing) {
			UnsubscribeEvents();
			base.Dispose(disposing);
		}
		protected void SubscribeEvents() {
			GetSrvc.ComponentAdding += OnComponentAdding;
			DesignerHost.Deactivated += OnDesignerHostDeactivated;
		}
		protected void UnsubscribeEvents() {
			GetSrvc.ComponentAdding -= OnComponentAdding;
			DesignerHost.Deactivated -= OnDesignerHostDeactivated;
		}
		protected void OnComponentAdding(object sender, ComponentEventArgs e) {
			if(Menu.Manager != null || Menu.Ribbon != null) return;
			if(e.Component is BarManager) Menu.Manager = (BarManager)e.Component;
			if(e.Component is RibbonControl) Menu.Ribbon = (RibbonControl)e.Component;
		}
		protected RadialMenu RadialMenu { get { return Component as RadialMenu; } }
		protected PopupMenuBase Menu { get { return Component as PopupMenuBase; } }
	}
	public class RadialMenuActionListDesigner: DesignerActionList {
		public RadialMenuActionListDesigner(IComponent component)
			: base(component) { }
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection items = new DesignerActionItemCollection();
			items.Add(new DesignerActionPropertyItem("PaintStyle", "Paint Style", "View Options"));
			return items;
		}
		public RadialMenu RadialMenu { get { return Component as RadialMenu; } }
		public PaintStyle PaintStyle {
			get { return RadialMenu.PaintStyle; }
			set {
				if(PaintStyle == value) return;
				RadialMenu.PaintStyle = value;
				EditorContextHelper.SetPropertyValue(Component.Site, Component, "PaintStyle", PaintStyle);
			}
		}
	}
	public class PopupMenuDesignerActionListBase: DesignerActionList {
		public PopupMenuDesignerActionListBase(IComponent component) : base(component) { }
		protected PopupMenuBase Menu { get { return Component as PopupMenuBase; } }
		RibbonControl Ribbon { get { return Menu.Ribbon; } }
		BarManager GetManager() { return (Menu == null ? null : Menu.Manager); }
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection items = new DesignerActionItemCollection();
			if(Menu.Ribbon == null)
				items.Add(new DesignerActionMethodItem(this, "OnCustomizeClick", "Customize", "View Options", true));
			else
				items.Add(new DesignerActionMethodItem(this, "OnRunDesigner", "Run Designer", "View Options", true));
			if(GetManager() != null && GetManager().Form != null && ContextMenu != null)
				items.Add(new DesignerActionMethodItem(this, "OnImportClick", "Import from ContextMenu", "View Options", true));
			return items;
		}
		protected virtual void OnCustomizeClick() {
			if(Menu == null) return;
			if(Menu.Manager == null) {
				DialogResult msgRes = MessageBox.Show(BarDesignTimeUtils.GetBarManagerRequiredWarningMessage(Component), Menu.Site.Name + ".Manager and " + Menu.Site.Name + ".Ribbon are not initialized", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
				if(msgRes != DialogResult.Yes)
					return;
				BarManager manager = BarDesignTimeUtils.CreateDefaultBarManager(Component);
				Menu.Manager = manager;
				FireChanged(Menu);
			}
			Menu.Customize();
		}
		protected virtual void OnRunDesigner() {
			IUIService srv = GetService(typeof(IUIService)) as IUIService;
			using(RibbonEditorForm form = new RibbonEditorForm(Component)) {
				form.InitEditingObject(Ribbon);
				form.ShowDialog(srv == null ? null : srv.GetDialogOwnerWindow());
			}
		}
		protected virtual void OnImportClick(object sender, EventArgs e) {
			ContextMenu[] menus = ContextMenu;
			if(GetManager() == null || GetManager().Form == null || menus == null) return;
			DevExpress.XtraBars.Design.contextMenuForm mf = new DevExpress.XtraBars.Design.contextMenuForm();
			mf.cbConvert.Items.Clear();
			foreach(ContextMenu item in menus) {
				mf.cbConvert.Items.Add(item.Site.Name);
			}
			mf.cbConvert.SelectedIndex = 0;
			if(mf.ShowDialog() != DialogResult.OK || mf.cbConvert.SelectedIndex < 0) return;
			ContextMenu menu = menus[mf.cbConvert.SelectedIndex];
			if(menu == null) return;
			if(MessageBox.Show(null, "Do you really want to import '" + menu.Site.Name + "' into the BarManager ?", "BarManager Designer", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
			try {
				DoConvert(menu);
			}
			catch(Exception ex) {
				MessageBox.Show(null, "Some errors have occurred while importing (" + ex.Message + ")", "BarManager Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
		}
		void DoConvert(ContextMenu menu) {
			BarManagerDesigner.CreateMenus(GetManager(), Menu, null, menu.MenuItems);
		}
		protected void FireChanged(IComponent component) {
			IComponentChangeService srv = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(srv != null) srv.OnComponentChanged(component, null, null, null);
		}
		protected ContextMenu[] ContextMenu {
			get {
				if(GetManager() == null || GetManager().Container == null) return null;
				ArrayList list = new ArrayList();
				foreach(object obj in GetManager().Container.Components) {
					ContextMenu menu = obj as ContextMenu;
					if(menu == null || menu.MenuItems == null || menu.MenuItems.Count < 1) continue;
					if(obj is ContextMenu) list.Add(obj);
				}
				if(list.Count == 0) return null;
				return list.ToArray(typeof(ContextMenu)) as ContextMenu[];
			}
		}
	}
	public class AppMenuDesignerActionList : PopupMenuDesignerActionListBase {
		public AppMenuDesignerActionList(IComponent component)
			: base(component) {
		}
		protected override void OnCustomizeClick() {
			if(Menu == null || Menu.Ribbon != null) return;
			DialogResult msgRes = MessageBox.Show(BarDesignTimeUtils.GetRibbonRequiredWarningMessage(Component), Component.Site.Name + ".Ribbon are not initialized", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
			if(msgRes != DialogResult.Yes)
				return;
			RibbonControl ribbon = BarDesignTimeUtils.CreateDefaultRibbonControl(Component, true);
			Menu.Ribbon = ribbon;
			ribbon.ApplicationButtonDropDownControl = Menu;
			FireChanged(Menu);
			ShowDesignerForm(Menu.Ribbon);
		}
		protected override void OnRunDesigner() {
			base.OnRunDesigner();
		}
		protected override void OnImportClick(object sender, EventArgs e) {
			base.OnImportClick(sender, e);
		}
		protected void ShowDesignerForm(RibbonControl ribbon) {
			IUIService srv = GetService(typeof(IUIService)) as IUIService;
			using(RibbonEditorForm form = new RibbonEditorForm(Component)) {
				form.InitEditingObject(ribbon);
				form.ShowDialog(srv == null ? null : srv.GetDialogOwnerWindow());
			}
		}
	}
	public class PopupMenuBaseDesigner : BaseComponentDesigner {
		IDesignerHost host;
		public PopupMenuBaseDesigner() {
			this.host = null;
		}
		PopupMenuBase Menu { get { return Component as PopupMenuBase; } }
		BarManager GetManager() { return (Menu == null ? null : Menu.Manager); }
		RibbonControl Ribbon { get { return Menu.Ribbon; } }
		protected IDesignerHost DesignerHost {
			get {
				if(host == null) host = (IDesignerHost)GetService(typeof(IDesignerHost));
				return host;
			}
		}
		protected virtual bool AllowManagerProperty { get { return true; } }
		protected override void PostFilterProperties(IDictionary properties) {
			if(AllowManagerProperty) properties["Manager"] = TypeDescriptor.GetProperties(this)["Manager"];
			base.PostFilterProperties(properties);
			DXPropertyDescriptor.ConvertDescriptors(properties, null);
		}
		protected override bool AllowInheritanceWrapper { get { return true; } }
		protected override bool UseVerbsAsActionList { get { return true; } }
		protected virtual bool AllowCustomize { get { return true; } }
		protected override bool AllowHookDebugMode { get { return true; } }
		DesignerActionListCollection list = null;
		public override DesignerActionListCollection ActionLists {
			get {
				if(list == null) {
					this.list = base.ActionLists;
					if(list == null)
						list = new DesignerActionListCollection();
					this.list.Add(GetActionList(Component));
				}
				return list;
			}
		}
		protected virtual PopupMenuDesignerActionListBase GetActionList(IComponent component) {
			return new PopupMenuDesignerActionListBase(component);
		}
		[DefaultValue(null), RefreshProperties(RefreshProperties.All)]
		public BarManager Manager {
			get {
				if(Ribbon != null) return null;
				return Menu.Manager;
			}
			set {
				Menu.Manager = value;
			}
		}
		protected override void OnInitializeNew(IDictionary defaultValues) {
			base.OnInitializeNew(defaultValues);
			if(Menu == null || Menu.Container == null) return;
			if(Menu.Ribbon == null && Menu.Manager == null) Menu.Ribbon = DesignHelpers.GetRibbon(Menu.Container);
			if(Menu.Ribbon == null && Menu.Manager == null) Menu.Manager = DesignHelpers.GetBarManager(Menu.Container);
		}
		public override ICollection AssociatedComponents {
			get {
				if(Menu.Ribbon == null && Menu.Manager == null)
					return base.AssociatedComponents;
				ArrayList list = new ArrayList();
				list.Add(Menu.Ribbon == null ? (Component)Menu.Manager : (Component)Menu.Ribbon);
				return list;
			}
		}
	}
	public class DesignHelpers {
		public static Form GetForm(IContainer container) {
			return GetTypeFromContainer(container, typeof(Form)) as Form;
		}
		public static void RefreshSmartTag(IComponent component) {
			DesignerActionUIService svc = component.Site.GetService(typeof(DesignerActionUIService)) as DesignerActionUIService;
			if(svc != null) svc.Refresh(component);
		}
		public static BarManager GetBarManager(IContainer container) {
			return GetTypeFromContainer(container, typeof(BarManager)) as BarManager;
		}
		public static RibbonControl GetRibbon(IContainer container) {
			return GetTypeFromContainer(container, typeof(RibbonControl)) as RibbonControl;
		}
		public static object FindComponent(IContainer container, Type componentType) {
			if(container == null) return null;
			foreach(object obj in container.Components) {
				if(obj.GetType().Equals(componentType)) return obj;
			}
			return null;
		}
		public static ContainerControl GetContainerControl(IContainer container) {
			if(GetForm(container) != null) return GetForm(container);
			return GetUserControl(container);
		}
		public static ContainerControl GetUserControl(IContainer container) {
			foreach(object obj in container.Components) {
				ContainerControl ctrl = obj as ContainerControl;
				if(ctrl != null && ctrl.ParentForm == null) return ctrl;
			}
			return null;
		}
		protected static object GetTypeFromContainer(IContainer container, Type type) {
			if(container == null || type == null) return null;
			foreach(object obj in container.Components) {
				if(type.IsInstanceOfType(obj)) return obj;
			}
			return null;
		}
		const string MDIWarning = "You can't use several MDI managers with a single MDI Parent form! The MDI Parent form should be cleared. Do you want to proceed?";
		const string MDIWarningException = "Can't initialize the {0} component";
		const string MDIWarningTitle = "{0}: Warning";
		public static void EnsureMDIClient(IContainer container, object newMDIManager) {
			string mdiManagerName = newMDIManager.GetType().Name;
			foreach(object obj in container.Components) {
				MdiClient client = obj as MdiClient;
				if(client != null && client.Site != null) {
					if(MessageBox.Show(null, MDIWarning, string.Format(MDIWarningTitle, mdiManagerName), 
						MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes) {
						client.Dispose();
						break;
					}
					throw new Exception(string.Format(MDIWarningException, mdiManagerName));
				}
			}
			Form mdiParentForm = DesignHelpers.GetForm(container);
			if(mdiParentForm != null) {
				MdiClient mdiClient = DevExpress.Utils.Mdi.MdiClientSubclasser.GetMdiClient(mdiParentForm);
				if(mdiClient != null) {
					var subclasser = DevExpress.Utils.Mdi.MdiClientSubclasser.FromMdiClient(mdiClient);
					if(subclasser != null && subclasser.Owner != null) {
						var subclasserOwner = subclasser.Owner;
						if(MessageBox.Show(null, MDIWarning, string.Format(MDIWarningTitle, mdiManagerName),
							MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes) {
							IDisposable mdiManager = subclasserOwner as IDisposable;
							if(mdiManager != null && newMDIManager != mdiManager)
								mdiManager.Dispose();
						}
						else throw new Exception(string.Format(MDIWarningException, mdiManagerName));
					}
				}
			}
		}
	}
	public class BarAndDockingControllerDesigner : BaseComponentDesignerSimple {
		protected BarAndDockingController Controller { get { return Component as BarAndDockingController; } }
		public override void Initialize(IComponent component) {
			base.Initialize(component);
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
		protected virtual void OnInitialize() {
			if (Controller == null || Controller.Container == null) return;
			BarManager barManager = DesignHelpers.FindComponent(Controller.Container, typeof(BarManager)) as BarManager;
			if(barManager != null && barManager.Controller == null) barManager.Controller = Controller;
			DockManager dockManager = DesignHelpers.FindComponent(Controller.Container, typeof(DockManager)) as DockManager;
			if(dockManager != null && dockManager.Controller == null) dockManager.Controller = Controller;
			XtraTabbedMdi.XtraTabbedMdiManager mdiManager = DesignHelpers.FindComponent(Controller.Container, 
				typeof(XtraTabbedMdi.XtraTabbedMdiManager)) as XtraTabbedMdi.XtraTabbedMdiManager;
			if(mdiManager != null && mdiManager.Controller == null) mdiManager.Controller = Controller;
			Docking2010.DocumentManager documentManager = DesignHelpers.FindComponent(Controller.Container, 
				typeof(Docking2010.DocumentManager)) as Docking2010.DocumentManager;
			if(documentManager != null && documentManager.BarAndDockingController == null)
				documentManager.BarAndDockingController = Controller;
			RibbonControl ribbon = DesignHelpers.FindComponent(Controller.Container, typeof(RibbonControl)) as RibbonControl;
			if(ribbon != null && ribbon.Controller == null) ribbon.Controller = Controller;
		}
	}
	public class XtraTabbedMdiManagerDesigner : ComponentDesigner {
		protected DevExpress.XtraTabbedMdi.XtraTabbedMdiManager Manager { get { return Component as DevExpress.XtraTabbedMdi.XtraTabbedMdiManager; } }
		public override void Initialize(IComponent component) {
			base.Initialize(component);
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
		protected virtual void OnInitialize() {
			if (Manager == null || Manager.Container == null) return;
			if(Manager.MdiParent != null || Manager.Container.Components == null) return;
			DesignHelpers.EnsureMDIClient(Manager.Container, Manager);
			Manager.MdiParent = DesignHelpers.GetContainerControl(Manager.Container) as Form;
			Manager.Controller = DesignHelpers.FindComponent(Manager.Container, typeof(BarAndDockingController)) as BarAndDockingController;
		}
	}
}
