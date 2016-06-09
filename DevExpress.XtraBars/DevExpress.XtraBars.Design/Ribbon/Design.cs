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
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.Design.Behavior;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.XtraBars.Design;
using DevExpress.XtraBars.InternalItems;
using DevExpress.XtraBars.Ribbon.Customization;
using DevExpress.XtraBars.Ribbon.Gallery;
using DevExpress.XtraBars.Ribbon.Helpers;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.XtraBars.TypeConverters;
using DevExpress.XtraBars.Utils;
using DevExpress.XtraEditors;
namespace DevExpress.XtraBars.Ribbon.Design {
	public class ItemListViewType {
		View view;
		public ItemListViewType(View view) {
			this.view = view;
		}
		public View View { get { return view; } }
	}
	public class ItemsListHelper {
		RibbonControl ribbon;
		IDesignerHost designerHost = null;
		IComponentChangeService componentChangeService = null;
		public ItemsListHelper(RibbonControl ribbon) {
			this.ribbon = ribbon;
		}
		public RibbonControl Ribbon { get { return ribbon; } }
		public IDesignerHost DesignerHost {
			get { return designerHost; }
			set { designerHost = value; }
		}
		public IComponentChangeService ComponentChangeService {
			get { return componentChangeService; }
			set { componentChangeService = value; }
		}
		public void RemoveItemFromTree(TreeNodeCollection nodes, BarItem item) {
			if(nodes == null || nodes.Count == 0) return;
			object parentObject = null;
			for(int n = nodes.Count - 1; n >= 0; n--) {
				RemoveItemFromTree(nodes[n].Nodes, item);
				BarItemLink link = nodes[n].Tag as BarItemLink;
				if(link != null && (link.Item == item || link.Item == null)) {
					if(link.Item == item)link.Dispose();
					if(nodes[n].Parent != null) parentObject = nodes[n].Parent.Tag;
					nodes.RemoveAt(n);
				}
			}
			if(parentObject != null)
				ComponentChangeService.OnComponentChanged(parentObject, null, null, null);
		}
		public void RemoveItems(DXTreeView ribbonTree, RibbonItemsListBox itemsList) {
			int count = itemsList.SelectedIndices.Count;	
			itemsList.Items.BeginUpdate();
			try {
				while(count-- > 0) { 
					BarItem item = itemsList.GetBarItem(itemsList.SelectedIndices[0]);
					if(ribbonTree != null) RemoveItemFromTree(ribbonTree.Nodes, item);
					itemsList.Items.Remove(item);
					Ribbon.Items.Remove(item);
					item.Dispose();
				}
			}
			finally { itemsList.Items.EndUpdate(); }
			Ribbon.Refresh();
		}
		public void CreateNewRibbonItem(RibbonItemsListBox list) { 
		}
		protected void RemoveItem(BarItemLink link, object ownerComponent) {
			link.Dispose(); 
			ComponentChangeService.OnComponentChanged(ownerComponent, null, null, null);
		}
	}
	public abstract class BaseRibbonControlDesigner : BaseControlDesigner {
		protected override bool AllowHookDebugMode { get { return true; } }
		protected override bool AllowInheritanceWrapper { get { return true; } }
		protected override bool AllowEditInherited { get { return false; } }
		IDesignerHost host;
		ISelectionService selection;
		IComponentChangeService changeService = null;
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			this.host = GetService(typeof(IDesignerHost)) as IDesignerHost;
			this.selection = GetService(typeof(ISelectionService)) as ISelectionService;
			this.changeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(this.changeService != null) this.changeService.ComponentRemoving += new ComponentEventHandler(OnComponentRemoving);
			LoaderPatcherService.InstallService(host);			
		}
		protected virtual void OnComponentRemoving(object sender, ComponentEventArgs e) {
			GalleryObjectDescriptor desc = e.Component as GalleryObjectDescriptor;
			if(desc != null) {
				if(desc.Item is GalleryItemGroup)
					throw new ApplicationException("You can't remove GalleryItemGroup in this way");
				else
					throw new ApplicationException("You can't remove GalleryItem in this way");
			}
		}
		bool isDisposed = false;
		protected override void Dispose(bool disposing) {
			LoaderPatcherService.UnInstallService(host);
			this.host = null;
			base.Dispose(disposing);
			this.isDisposed = true;
		}
		protected bool IsDisposed { get { return isDisposed; } }
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			OnInitialize();
		}
		protected virtual void OnInitialize() { }
		protected void AddBase(ArrayList controls) {
			foreach(object obj in base.AssociatedComponents) {
				if(controls.Contains(obj)) continue;
				controls.Add(obj);
			}
		}
		protected override bool GetHitTest(Point point) {
			if(Ribbon == null) return base.GetHitTest(point);
			if(DebuggingState) return base.GetHitTest(point);
			RibbonHitInfo hitInfo = CalcHitInfo(point);
			if(hitInfo.InItem) return true;
			if(hitInfo.HitTest == RibbonHitTest.PageHeaderCategory) return true;
			if(hitInfo.HitTest == RibbonHitTest.ApplicationButton) return true;
			if(hitInfo.HitTest == RibbonHitTest.PageHeader) return true;
			if(hitInfo.HitTest == RibbonHitTest.Toolbar) return true;
			if(Ribbon.ViewInfo.Header.DesignerRect.Contains(Ribbon.PointToClient(point))) return true;
			if(hitInfo.HitTest == RibbonHitTest.GalleryDropDownButton) return true;
			if(hitInfo.InGalleryItem || hitInfo.InGalleryGroup || hitInfo.InGallery) return true;
			if(hitInfo.InPageGroup) {
				return hitInfo.HitTest != RibbonHitTest.PageGroupCaptionButton;
			}
			return base.GetHitTest(point);
		}
		protected abstract RibbonHitInfo CalcHitInfo(Point point);
		protected abstract RibbonControl Ribbon { get; }
		protected IComponentChangeService ChangeService { get { return changeService; } }
	}
	public class RibbonPageGroupDesigner : BaseComponentDesigner {
		protected override bool AllowEditInherited { get { return true; } }
		protected override bool AllowInheritanceWrapper { get { return true; } }
		protected override void OnInitializeNew(IDictionary defaultValues) {
			base.OnInitializeNew(defaultValues);
			PageGroup.Text = PageGroup.Name;
		}
		RibbonPageGroup PageGroup { get { return Component as RibbonPageGroup; } }
	}
	public class GalleryDesignerActionList : RibbonDesignerActionListBase {
		GalleryDesigner designer;
		public GalleryDesignerActionList(GalleryDesigner designer) : base(designer.Component) {
			this.designer = designer;
		}
		protected GalleryDesigner Designer { get { return designer; } }
		public override void RunDesigner() {
			ShowDesignerForm(FindRibbon(Designer.Container));
		}
		protected virtual RibbonControl FindRibbon(IContainer cont) {
			foreach(IComponent comp in cont.Components) {
				if(comp as RibbonControl != null) return comp as RibbonControl;
			}
			return null;
		}
		public override void PreInitDesignerActionItemCollectionCore(DesignerActionItemCollection items) {
		}
		public override void PostInitDesignerActionItemCollectionCore(DesignerActionItemCollection items) {
		}
	}
	public class GalleryDesigner : BaseComponentDesigner {
		protected virtual void CreateGalleryDesignerActionLists(DesignerActionListCollection list) {
			list.Add(new GalleryDesignerActionList(this));
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			CreateGalleryDesignerActionLists(list);
			base.RegisterActionLists(list);
		}
		public override DesignerVerbCollection DXVerbs {
			get {
				DesignerVerbCollection coll = new DesignerVerbCollection(
					new DesignerVerb[] { new DesignerVerb("Run Designer", new EventHandler(OnDesignerClick)) }
				);
				return coll;
			}
		}
		protected virtual void ShowDesignerForm(RibbonControl ribbon) {
			IUIService srv = GetService(typeof(IUIService)) as IUIService;
			using(RibbonEditorForm form = new RibbonEditorForm(Component)) {
				form.InitEditingObject(ribbon);
				form.ShowDialog(srv == null ? null : srv.GetDialogOwnerWindow());
			}
		}
		protected virtual RibbonControl FindRibbon(IContainer cont) {
			foreach(IComponent comp in cont.Components) {
				if(comp as RibbonControl != null) return comp as RibbonControl;
			}
			return null;
		}
		protected virtual void OnDesignerClick(object sender, EventArgs e) {
			RibbonControl ribbon = FindRibbon(Container);
			if(ribbon == null) return;
			ShowDesignerForm(ribbon);
		}
		protected internal virtual IContainer Container { get { return null; } }
	}
	public class RibbonGalleryBarItemDesigner : GalleryDesigner, IKeyCommandProcessInfo {
		InRibbonGalleryKeyCommandProcessHelper keyCommandProcessHelper;		
		protected override void OnInitializeNew(IDictionary defaultValues) {
			base.OnInitializeNew(defaultValues);
			Gallery.Caption = Gallery.Name;
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			this.keyCommandProcessHelper = new InRibbonGalleryKeyCommandProcessHelper(this);
		}
		protected InRibbonGalleryKeyCommandProcessHelper KeyCommandProcessHelper { get { return keyCommandProcessHelper; } }
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(KeyCommandProcessHelper != null) {
					KeyCommandProcessHelper.Dispose();
				}
				this.keyCommandProcessHelper = null;
			}
			base.Dispose(disposing);
		}
		DesignerActionListCollection listCore = null;
		public override DesignerActionListCollection ActionLists {
			get {
				if(listCore == null) {
					listCore = new DesignerActionListCollection();
					listCore.Add(new RibbonGalleryBarItemActionList(Component));
				}
				return listCore;
			}
		}
		#region IKeyCommandProcessInfo
		IServiceProvider IKeyCommandProcessInfo.ServiceProvider {
			get { return Component.Site; }
		}
		BaseDesignTimeManager IKeyCommandProcessInfo.DesignTimeManager {
			get { return Gallery.Gallery.GetDesignTimeManager(); }
		}
		IComponent IKeyCommandProcessInfo.Component {
			get { return Component; }
		}
		#endregion
		protected RibbonGalleryBarItem Gallery { get { return Component as RibbonGalleryBarItem; } }
		protected internal override IContainer Container { get { return Gallery.Container; } }
	}
	public class RibbonGalleryBarItemActionList : GalleryControlDesignerActionListBase {
		public RibbonGalleryBarItemActionList(IComponent component)
			: base(component) {
		}
		public override void PostInitDesignerActionItemCollectionCore(DesignerActionItemCollection items) {
			items.Add(new DesignerActionMethodItem(this, "AddGroup", "Add Group", "Actions"));
			items.Add(new DesignerActionMethodItem(this, "AddDropDownGallery", "Add Drop Down Gallery", "Actions"));
		}
		public virtual void AddDropDownGallery() {
			if(DesignTimeManager == null) return;
			GalleryBarItem.GalleryDropDown = DesignTimeManager.AddGalleryDropDown();
		}
		public override void RunDesigner() {
			RibbonControl ribbon = GetRibbon();
			if(ribbon != null) ShowDesignerForm(ribbon);
		}
		protected override void ShowDesignerForm(RibbonControl ribbon) {
			IUIService srv = GetService(typeof(IUIService)) as IUIService;
			using(RibbonEditorForm form = new RibbonEditorForm(Component)) {
				form.InitEditingObject(ribbon);
				form.ShowDialog(srv == null ? null : srv.GetDialogOwnerWindow());
			}
		}
		public void AddGroup() {
			if(DesignTimeManager == null) return;
			DesignTimeManager.OnAddGalleryGroupCore(GalleryBarItem, null, null);
		}
		RibbonDesignTimeManager designTimeManager = null;
		protected RibbonDesignTimeManager DesignTimeManager {
			get {
				if(designTimeManager == null) {
					designTimeManager = GetDesignTimeManagerCore();
				}
				return designTimeManager;
			}
		}
		protected RibbonControl GetRibbon() {
			if(GalleryBarItem == null || GalleryBarItem.Gallery == null)
				return null;
			return GalleryBarItem.Gallery.GetRibbon();
		}
		protected virtual RibbonDesignTimeManager GetDesignTimeManagerCore() {
			InRibbonGallery gallery = GalleryBarItem.Gallery;
			return gallery != null ? gallery.GetDesignTimeManager() as RibbonDesignTimeManager : null;
		}
		public override BaseGallery GalleryCore {
			get { return GalleryBarItem.Gallery; }
		}
		public RibbonGalleryBarItem GalleryBarItem { get { return Component as RibbonGalleryBarItem; } }
	}
	public class RibbonGalleryDropDownDesigner : GalleryDesigner {
		protected GalleryDropDown Gallery { get { return Component as GalleryDropDown; } }
		protected internal override IContainer Container { get { return Gallery.Container; } }
		protected override void CreateGalleryDesignerActionLists(DesignerActionListCollection list) {
			list.Add(new GalleryDropDownDesignerActionList(Component));
		}
		protected override void OnDesignerClick(object sender, EventArgs e) {
			if(ShouldTryAutoGenerateRibbon) {
				DialogResult msgRes = MessageBox.Show(BarDesignTimeUtils.GetRibbonRequiredWarningMessage(Component), Component.Site.Name + ".Ribbon are not initialized", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
				if(msgRes == DialogResult.Yes) {
					RibbonControl ribbon = BarDesignTimeUtils.CreateDefaultRibbonControl(Component, true);
					GalleryDropDown.Ribbon = ribbon;
					BarDesignTimeUtils.FireChanged(GalleryDropDown);
					ShowDesignerForm(ribbon);
					return;
				}
			}
			base.OnDesignerClick(sender, e);
		}
		protected virtual bool ShouldTryAutoGenerateRibbon {
			get { return FindRibbon(Container) == null; }
		}
		protected GalleryDropDown GalleryDropDown { get { return Component as GalleryDropDown; } }
		protected override bool UseVerbsAsActionList { get { return true; } }
		protected override void OnInitializeNew(IDictionary defaultValues) {
			base.OnInitializeNew(defaultValues);
			if(Gallery == null) return;
			if(Gallery.Ribbon == null && Gallery.Manager == null) Gallery.Ribbon = DesignHelpers.GetRibbon(Gallery.Container);
			if(Gallery.Ribbon == null && Gallery.Manager == null) Gallery.Manager = DesignHelpers.GetBarManager(Gallery.Container);
		}
	}
	public class RibbonPageDesigner : BaseComponentDesigner {
		protected override bool AllowEditInherited { get { return true; } }
		protected override bool AllowInheritanceWrapper { get { return true; } }
		protected override void OnInitializeNew(IDictionary defaultValues) {
			base.OnInitializeNew(defaultValues);
			Page.Text = Page.Name;
		}
		RibbonPage Page { get { return Component as RibbonPage; } }
	}
	public class RibbonPageCategoryDesigner : BaseComponentDesigner {
		protected override bool AllowEditInherited { get { return true; } }
		protected override bool AllowInheritanceWrapper { get { return true; } }
		protected override void OnInitializeNew(IDictionary defaultValues) {
			base.OnInitializeNew(defaultValues);
			PageCategory.Text = PageCategory.Name;
		}
		RibbonPageCategory PageCategory { get { return Component as RibbonPageCategory; } }
	}
	public class GalleryDropDownDesignerActionList : RibbonDesignerActionListBase {
		public GalleryDropDownDesignerActionList(IComponent component) : base(component) { }
		public override void PreInitDesignerActionItemCollectionCore(DesignerActionItemCollection items) {
		}
		public override void PostInitDesignerActionItemCollectionCore(DesignerActionItemCollection items) {
		}
	}
	public abstract class RibbonDesignerActionListBase : DesignerActionList {
		public RibbonDesignerActionListBase(IComponent component)
			: base(component) {
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection res = new DesignerActionItemCollection();
			PreInitDesignerActionItemCollectionCore(res);
			res.Add(new DesignerActionHeaderItem("Actions"));
			res.Add(new DesignerActionMethodItem(this, "RunDesigner", "Run Designer", "Actions"));
			PostInitDesignerActionItemCollectionCore(res);
			return res;
		}
		public abstract void PreInitDesignerActionItemCollectionCore(DesignerActionItemCollection items);
		public abstract void PostInitDesignerActionItemCollectionCore(DesignerActionItemCollection items);
		public virtual void RunDesigner() {
			RibbonControl ribbon = null;
			if(Component is RibbonControl)
				ribbon = Component as RibbonControl;
			if(Component is RibbonStatusBar)
				ribbon = (Component as RibbonStatusBar).Ribbon;
			ShowDesignerForm(ribbon);
		}
		protected virtual void ShowDesignerForm(RibbonControl ribbon) {
			if(ribbon == null) return;
			IUIService srv = Component.Site.GetService(typeof(IUIService)) as IUIService;
			using(RibbonEditorForm form = new RibbonEditorForm()) {
				form.InitEditingObject(ribbon);
				form.ShowDialog(srv == null ? null : srv.GetDialogOwnerWindow());
			}
		}
	}
	public class RibbonDesignerActionList : RibbonDesignerActionListBase {
		public RibbonDesignerActionList(IComponent component)
			: base(component) {
		}
		public override void PreInitDesignerActionItemCollectionCore(DesignerActionItemCollection items) {
			if(AllowCreateConvertFormToRibbonFormCommand) {
				items.Add(new DesignerActionMethodItem(this, "ConvertFormToRibbonForm", "Convert Form To RibbonForm", "Actions"));
			}
			items.Add(new DesignerActionHeaderItem("Application Menu", "Application Menu"));
			items.Add(new DesignerActionPropertyItem("ApplicationButtonDropDownControl", "Choose", "Application Menu"));
			items.Add(new DesignerActionMethodItem(this, "AddApplicationMenu", "Add Application Menu (Office 2007 Style Menu)", "Application Menu"));
			items.Add(new DesignerActionMethodItem(this, "AddBackstageViewControl", "Add Backstage View (Office 2010/13 Style Menu)", "Application Menu"));
			items.Add(new DesignerActionHeaderItem("View Options"));
			items.Add(new DesignerActionPropertyItem("RibbonStyle", "Ribbon Style", "View Options"));
			if(AllowCreateAllowFormGlassOption) {
				items.Add(new DesignerActionPropertyItem("AllowFormGlass", "Allow Form Glass", "View Options"));
			}
		}
		public override void PostInitDesignerActionItemCollectionCore(DesignerActionItemCollection items) {
			if(AllowCreateRemoveUnassignedItemsCommand) {
				items.Add(new DesignerActionMethodItem(this, "RemoveUnassignedItems", "Remove Unassigned Items", "Actions"));
			}
			items.Add(new DesignerActionMethodItem(this, "AddRibbonMiniToolbar", "Add Ribbon Mini Toolbar", "Actions"));
		}
		#region Options & Commands
		public virtual void ConvertFormToRibbonForm() {
			FormTypeConverter.ToType(Component.Site, typeof(RibbonForm));
		}
		public virtual void AddApplicationMenu() {
			RibbonDesignTimeManager.AddApplicationMenu();
			DesignHelpers.RefreshSmartTag(Component);
		}
		public virtual void AddBackstageViewControl() {
			RibbonDesignTimeManager.AddBackstageView();
			DesignHelpers.RefreshSmartTag(Component);
		}
		public virtual void AddRibbonMiniToolbar() {
			RibbonMiniToolbar toolbar = new RibbonMiniToolbar();
			Ribbon.Container.Add(toolbar);
			Ribbon.MiniToolbars.Add(toolbar);
		}
		public RibbonControlStyle RibbonStyle {
			get { return Ribbon.RibbonStyle; }
			set {
				if(RibbonStyle == value) return;
				EditorContextHelper.SetPropertyValue(Component.Site, Component, "RibbonStyle", value);
			}
		}
		public bool AllowFormGlass {
			get { return RibbonForm.AllowFormGlass != DefaultBoolean.False; }
			set {
				if(AllowFormGlass == value) return;
				EditorContextHelper.SetPropertyValue(Component.Site, RibbonForm, "AllowFormGlass", value ? DefaultBoolean.True : DefaultBoolean.False);
			}
		}
		[TypeConverter(typeof(RibbonActionListApplicationButtonDropDownControlTypeConverter))]
		public object ApplicationButtonDropDownControl {
			get { return Ribbon.ApplicationButtonDropDownControl; }
			set {
				object valueCore = value;
				if(value is string) {
					valueCore = Ribbon.Container.Components[(string)value];
				}
				EditorContextHelper.SetPropertyValue(Component.Site, Component, "ApplicationButtonDropDownControl", valueCore);
			}
		}
		public RibbonControl Ribbon { get { return Component as RibbonControl; } }
		protected RibbonForm RibbonForm { get { return Ribbon.Parent as RibbonForm; } }
		protected RibbonDesignTimeManager RibbonDesignTimeManager {
			get { return Ribbon.GetDesignTimeManager() as RibbonDesignTimeManager; }
		}
		protected virtual bool AllowCreateConvertFormToRibbonFormCommand {
			get {
				Form parentForm = Ribbon.Parent as Form;
				if(parentForm == null)
					return false;
				return !(parentForm is RibbonForm);
			}
		}
		protected virtual bool AllowCreateAllowFormGlassOption {
			get { return RibbonForm != null; }
		}
#if !DEBUG
		public override bool AutoShow {
			get {
				if(AllowCreateConvertFormToRibbonFormCommand)
					return true;
				return base.AutoShow;
			}
			set { base.AutoShow = value; }
		}
#endif
		protected virtual bool AllowCreateRemoveUnassignedItemsCommand {
			get {
				List<BarItem> list = GetUnassignedItems();
				return (list != null && list.Count > 0);
			}
		}
		public virtual void RemoveUnassignedItems() {
			if(XtraMessageBox.Show("Do you want remove all unassigned items?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
			RibbonControl ribbon = Component as RibbonControl;
			foreach(BarItem item in GetUnassignedItems()) {
				ribbon.Items.Remove(item);
				item.Dispose();
			}
			DesignHelpers.RefreshSmartTag(Component);
		}   
		public virtual List<BarItem> GetUnassignedItems() {
			RibbonControl ribbon = Component as RibbonControl;
			if(ribbon == null) return null;
			List<BarItem>list = new List<BarItem>();
			foreach(BarItem item in ribbon.Items) {
				if(item is RibbonExpandCollapseItem) continue;
				if(item is AutoHiddenPagesMenuItem) continue;
				list.Add(item);
			}
			foreach(RibbonPage page in ribbon.Pages)
				foreach(RibbonPageGroup group in page.Groups)
					foreach(BarItemLink link in group.ItemLinks)
						RemoveItemInList(list, link.Item);
			if(ribbon.StatusBar != null)
				foreach(BarItemLink link in ribbon.StatusBar.ItemLinks)
					RemoveItemInList(list, link.Item);
			foreach(BarItemLink link in ribbon.Toolbar.ItemLinks)
				RemoveItemInList(list, link.Item);
			foreach(RibbonMiniToolbar miniToolbar in ribbon.MiniToolbars)
				foreach(BarItemLink link in miniToolbar.ItemLinks)
					RemoveItemInList(list, link.Item);
			foreach(BarItemLink link in ribbon.PageHeaderItemLinks)
				RemoveItemInList(list, link.Item);	
			foreach(object obj in ribbon.Container.Components) {
				BarLinksHolder holder = obj as BarLinksHolder;
				if(holder != null)
					foreach(BarItemLink link in holder.ItemLinks)
						RemoveItemInList(list, link.Item);
			}
			return list;
		}
		protected void RemoveItemInList(List<BarItem> list, BarItem item) {
			list.Remove(item);
		}
		#endregion
	}
	public class RibbonDesignerWithVerbs : BaseRibbonControlDesigner { 
		DesignerVerbCollection verbs;
		protected virtual void CreateRibbonDesignerActionLists(DesignerActionListCollection list) {
			list.Add(new RibbonDesignerActionList(Component));			
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			CreateRibbonDesignerActionLists(list);
			base.RegisterActionLists(list);
		}
		public RibbonDesignerWithVerbs() {
			verbs = new DesignerVerbCollection(
				new DesignerVerb[] {
					new DesignerVerb("About", new EventHandler(OnAboutClick))
					,new DesignerVerb("Run Designer", new EventHandler(OnDesignerClick))
				}
			);
		}
		public override DesignerVerbCollection DXVerbs { get { return verbs; } }
		protected override RibbonHitInfo CalcHitInfo(Point point) { return new RibbonHitInfo();	}
		protected override RibbonControl Ribbon { get { return Control as RibbonControl; } }
		protected virtual void OnAboutClick(object sender, EventArgs e) { }
		protected virtual void OnDesignerClick(object sender, EventArgs e) { }
		protected virtual void ShowDesignerForm(RibbonControl ribbon) {
			if(ribbon == null) return;
			IUIService srv = GetService(typeof(IUIService)) as IUIService;
			using(RibbonEditorForm form = new RibbonEditorForm()) {
				form.InitEditingObject(ribbon);
				form.ShowDialog(srv == null ? null : srv.GetDialogOwnerWindow());
			}	
		}
		public override System.Collections.ICollection AssociatedComponents {
			get {
				if(Ribbon == null) return base.AssociatedComponents;
				ArrayList controls = new ArrayList();
				foreach(RibbonPage page in Ribbon.Pages) {
					controls.Add(page);
					controls.AddRange(page.Groups);
				}
				controls.AddRange(Ribbon.Items);
				if(Ribbon.Images != null) controls.Add(Ribbon.Images);
				if(Ribbon.LargeImages != null) controls.Add(Ribbon.LargeImages);
				AddBase(controls);
				return controls;
			}
		}
		protected virtual void AddStatusBar(ArrayList controls) { }
	}
	public class RibbonStatusBarDesignerActionList : RibbonDesignerActionListBase {
		public RibbonStatusBarDesignerActionList(IComponent component) : base(component) { }
		public override void PreInitDesignerActionItemCollectionCore(DesignerActionItemCollection items) {
		}
		public override void PostInitDesignerActionItemCollectionCore(DesignerActionItemCollection items) {
		}
	}
	public class RibbonStatusBarDesigner : RibbonDesignerWithVerbs {
		protected override RibbonHitInfo CalcHitInfo(Point point) {
			if(Ribbon == null) return new RibbonHitInfo();
			point = StatusBar.PointToClient(point);
			return StatusBar.CalcHitInfo(point); 
		}
		protected override void CreateRibbonDesignerActionLists(DesignerActionListCollection list) {
			 list.Add(new RibbonStatusBarDesignerActionList(Component));
		}
		protected RibbonStatusBar StatusBar { get { return Control as RibbonStatusBar; } }
		protected override RibbonControl Ribbon { get { return StatusBar.Ribbon; } }
		protected override void OnInitialize() {
			base.OnInitialize();
			if(Ribbon == null && StatusBar.Site != null) StatusBar.Ribbon = DevExpress.XtraBars.Design.DesignHelpers.GetRibbon(StatusBar.Site.Container);
		}
		protected override void OnDesignerClick(object sender, EventArgs e) { 
			ShowDesignerForm(StatusBar.Ribbon);
		}
		protected override void OnAboutClick(object sender, EventArgs e) { 
			RibbonControl.About();
		}
		protected override void AddStatusBar(ArrayList controls) {
			if(StatusBar != null)controls.Add(StatusBar);
		}
		protected override bool AllowEditInherited { get { return true; } }
		protected override bool AllowInheritanceWrapper { get { return true; } }
		public override ICollection AssociatedComponents {
			get { return GetOwnedItems(); }
		}
		protected ICollection GetOwnedItems() {
			ArrayList col = new ArrayList();
			foreach(BarItemLink link in StatusBar.ItemLinks) {
				BarItem item = link.Item;
				if(item != null && item.Links.Count <= 1 && !col.Contains(item)) col.Add(item);
			}
			return col;
		}
		protected override bool GetHitTest(Point point) {
			point = StatusBar.PointToClient(point);
			RibbonHitInfo hitInfo = StatusBar.CalcHitInfo(point);
			if(hitInfo.HitTest == RibbonHitTest.StatusBar)
				return StatusBar.ViewInfo.DesignerLeftRect.Contains(point) || StatusBar.ViewInfo.DesignerRightRect.Contains(point);
			if(hitInfo.InItem) return true;
			return false;
		}
		protected override bool CanUseComponentSmartTags { get { return true; } }
	}
	public class GalleryKeyCommandProcessHelperBase : DesignTimeKeyCommandProcessHelperBase, IDisposable {
		public GalleryKeyCommandProcessHelperBase(IKeyCommandProcessInfo info)
			: base(info) {
		}
		protected object GetSelectedItem(out object owner) {
			owner = null;
			GalleryItem item;
			GalleryItemGroup group;
			GalleryObjectDescriptor gd = SelectionService.PrimarySelection as GalleryObjectDescriptor;
			if(gd == null) {
				group = SelectionService.PrimarySelection as GalleryItemGroup;
				item = SelectionService.PrimarySelection as GalleryItem;
			}
			else {
				group = gd.GalleryGroup;
				item = gd.GalleryItem;
			}
			if(item != null) {
				owner = item.Gallery;
				return item;
			}
			if(group != null) {
				owner = group.Gallery;
				return group;
			}
			return null;
		}
	}
	public class GalleryControlKeyCommandProcessHelper : GalleryKeyCommandProcessHelperBase, IDisposable {
		public GalleryControlKeyCommandProcessHelper(IKeyCommandProcessInfo info) : base(info) { }
		public GalleryControlDesignTimeManager DesignTimeManager { get { return Info.DesignTimeManager as GalleryControlDesignTimeManager; } }
		public override void OnKeyCancel(object sender, EventArgs e) {
			object owner;
			object component = GetSelectedItem(out owner);
			if(component != null && owner == Gallery.Gallery) {
				if(component is GalleryItemGroup) {
					DesignTimeManager.SelectComponent(Info.Component);
					if(Info.Component is GalleryControl)
						((GalleryControl)Info.Component).Invalidate();
					return;
				}
				if(component is GalleryItem) {
					GalleryItem galleryItem = (GalleryItem)component;
					DesignTimeManager.SelectGalleryGroup(galleryItem.GalleryGroup);
					DesignTimeManager.SelectComponent(galleryItem.GalleryGroup);
					if(Info.Component is GalleryControl)
						((GalleryControl)Info.Component).Invalidate();
					return;
				}
			}
			PassControlToOldKeyCancelHandler();
		}
		GalleryControl Gallery { get { return Info.Component as GalleryControl; } }
	}
	public class InRibbonGalleryKeyCommandProcessHelper : GalleryKeyCommandProcessHelperBase, IDisposable {
		public InRibbonGalleryKeyCommandProcessHelper(IKeyCommandProcessInfo info)
			: base(info) {
		}
		public RibbonDesignTimeManager DesignTimeManager { get { return Info.DesignTimeManager as RibbonDesignTimeManager; } }
		public override bool NeedHandleDelete {
			get {
				return false;
			}
		}
		public override void OnKeyCancel(object sender, EventArgs e) {
			object owner;
			object component = GetSelectedItem(out owner);
			if(component != null && owner == RibbonGallery.Gallery) {
				RibbonGalleryBarItem gbi = (RibbonGalleryBarItem)Info.Component;
				if(component is GalleryItemGroup) {
					GalleryItemGroup groupRibbon = (GalleryItemGroup)component;
					DesignTimeManager.SelectComponent(Info.Component);
					DesignTimeManager.SelectRibbonGalleryBarItem(gbi);
					DesignTimeManager.Owner.Invalidate();
					return;
				}
				if(component is GalleryItem) {
					GalleryItem itemRibbon = (GalleryItem)component;
					DesignTimeManager.SelectComponent(itemRibbon.GalleryGroup);
					DesignTimeManager.SelectRibbonGalleryGroup(gbi, itemRibbon.GalleryGroup);
					DesignTimeManager.InvalidateComponent(gbi);
					return;
				}
			}
			PassControlToOldKeyCancelHandler();
		}
		RibbonGalleryBarItem RibbonGallery { get { return Info.Component as RibbonGalleryBarItem; } }
	}
	public class GalleryControlDesignerHelper {
		public GalleryControlDesignerHelper(IComponent component) {
			this.component = component;
		}
		IComponent component;
		public IComponent Component { get { return component; } }
		IComponentChangeService componentChangeService;
		public IComponentChangeService ComponentChangeService {
			get {
				if(componentChangeService == null)
					componentChangeService = Component.Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
				return componentChangeService;
			}
		}
		DesignerActionUIService designerActionUIService;
		public DesignerActionUIService DesignerActionUIService {
			get {
				if(designerActionUIService == null)
					designerActionUIService = Component.Site.GetService(typeof(DesignerActionUIService)) as DesignerActionUIService;
				return designerActionUIService;
			}
		}
		void FireChanged() {
			ComponentChangeService.OnComponentChanged(Component, null, null, null);
		}
		public GalleryControl GalleryControl { get { return Component as GalleryControl; } }
		public GalleryControlDesignTimeManager DesignTimeManager { get { return GalleryControl == null ? null : GalleryControl.GetDesignTimeManager(); } }
		public void AddGalleryGroup() {
			if(GalleryControl != null) {
				GalleryItemGroup group = new GalleryItemGroup();
				group.Caption = GalleryControlDesignTimeManager.GetGalleryGroupName();
				GalleryControl.Gallery.Groups.Add(group);
				DesignTimeManager.SelectGalleryGroup(group);
				DesignerActionUIService.Refresh(Component);
				FireChanged();
			}
		}
		public void RemoveGalleryGroup() {
			if(DesignTimeManager != null && DesignTimeManager.SelectedGroup != null) {
				GalleryItemGroup groupToRemove = DesignTimeManager.SelectedGroup;
				DesignTimeManager.DragObject = null;
				DesignTimeManager.SelectNextGalleryGroup(groupToRemove);
				GalleryControl.Gallery.Groups.Remove(groupToRemove);
				DesignerActionUIService.Refresh(Component);
				FireChanged();
			}
		}
		public void RemoveGalleryItem() {
			if(DesignTimeManager != null && DesignTimeManager.SelectedItem != null) {
				GalleryItem itemToRemove = DesignTimeManager.SelectedItem;
				DesignTimeManager.DragObject = null;
				DesignTimeManager.SelectNextGalleryItem(itemToRemove);
				itemToRemove.GalleryGroup.Items.Remove(itemToRemove);
				DesignerActionUIService.Refresh(Component);
				FireChanged();
			}
		}
		public void AddGalleryItem() {
			if(DesignTimeManager != null) {
				GalleryItemGroup selGroup = null;
				if(DesignTimeManager.SelectedGroup != null)
					selGroup = DesignTimeManager.SelectedGroup;
				if(DesignTimeManager.SelectedItem != null)
					selGroup = DesignTimeManager.SelectedItem.GalleryGroup;
				if(selGroup == null)
					return;
				GalleryItem item = new GalleryItem();
				item.Caption = GalleryControlDesignTimeManager.GetGalleryItemName();
				selGroup.Items.Add(item);
				DesignerActionUIService.Refresh(Component);
				FireChanged();
			}
		}
		public void DockInParentContainer() {
			GalleryControl.Dock = DockStyle.Fill;
			DesignerActionUIService.Refresh(Component);
		}
		public void UndockInParentContainer() {
			GalleryControl.Dock = DockStyle.None;
			DesignerActionUIService.Refresh(Component);
		}
	}
	public abstract class GalleryControlDesignerActionListBase : RibbonDesignerActionListBase {
		public GalleryControlDesignerActionListBase(IComponent component)
			: base(component) {
		}
		public abstract BaseGallery GalleryCore { get; }
		public override void PreInitDesignerActionItemCollectionCore(DesignerActionItemCollection items) {
			items.Add(new DesignerActionHeaderItem("Appearance"));
			items.Add(new DesignerActionPropertyItem("ShowItemText", "Show Item Text", "Appearance"));
			items.Add(new DesignerActionPropertyItem("ImageWidth", "Image Width", "Appearance"));
			items.Add(new DesignerActionPropertyItem("ImageHeight", "Image Height", "Appearance"));
		}
		public bool ShowItemText {
			get { return GalleryCore.ShowItemText; }
			set {
				EditorContextHelper.SetPropertyValue(Component.Site, GalleryCore, "ShowItemText", value);
			}
		}
		public int ImageWidth {
			get { return ImageSize.Width; }
			set {
				ImageSize = new Size(value, ImageSize.Height);
			}
		}
		public int ImageHeight {
			get { return ImageSize.Height; }
			set {
				ImageSize = new Size(ImageSize.Width, value);
			}
		}
		protected Size ImageSize {
			get { return GalleryCore.ImageSize; }
			set {
				EditorContextHelper.SetPropertyValue(Component.Site, GalleryCore, "ImageSize", value);
			}
		}
	}
	public class GalleryControlDesignerActionList : GalleryControlDesignerActionListBase {
		public GalleryControlDesignerActionList(IComponent component)
			: base(component) {
		}
		public override void PostInitDesignerActionItemCollectionCore(DesignerActionItemCollection items) {
			foreach(DesignerActionItem item in items) {
				DesignerActionMethodItem mitem = item as DesignerActionMethodItem;
				if(mitem != null && mitem.MemberName == "CreateGallery") {
					items.Remove(item);
					break;
				}
			}
			items.Add(new DesignerActionMethodItem(this, "AddGalleryGroup", "Add GalleryGroup", "Actions"));
			if(DesignTimeManager == null)
				return;
			if(DesignTimeManager.SelectedGroup != null)
				items.Add(new DesignerActionMethodItem(this, "RemoveGalleryGroup", "Remove GalleryGroup", "Actions"));
			if(DesignTimeManager.SelectedItem != null || DesignTimeManager.SelectedGroup != null) {
				items.Add(new DesignerActionMethodItem(this, "AddGalleryItem", "Add GalleryItem", "Actions"));
			}
			if(DesignTimeManager.SelectedItem != null) {
				items.Add(new DesignerActionMethodItem(this, "RemoveGalleryItem", "Remove GalleryItem", "Actions"));
			}
			if(GalleryControl.Dock != DockStyle.Fill)
				items.Add(new DesignerActionMethodItem(this, "DockInParentContainer", "Dock in parent container"));
			else
				items.Add(new DesignerActionMethodItem(this, "UndockInParentContainer", "Undock in parent container"));
		}
		protected override void ShowDesignerForm(RibbonControl ribbon) {
			IUIService srv = Component.Site.GetService(typeof(IUIService)) as IUIService;
			using(RibbonEditorForm form = new RibbonEditorForm(((GalleryControl)Component).Gallery)) {
				form.InitEditingObject(((GalleryControl)Component).Gallery);
				form.ShowDialog(srv == null ? null : srv.GetDialogOwnerWindow());
			}
		}
		public GalleryControl GalleryControl { get { return Component as GalleryControl; } }
		public GalleryControlDesignTimeManager DesignTimeManager { get { return GalleryControl == null ? null : GalleryControl.GetDesignTimeManager(); } }
		GalleryControlDesignerHelper helper;
		public GalleryControlDesignerHelper Helper {
			get {
				if(helper == null)
					helper = new GalleryControlDesignerHelper(Component);
				return helper;
			}
		}
		public void AddGalleryGroup() {
			Helper.AddGalleryGroup();
		}
		public void RemoveGalleryGroup() {
			Helper.RemoveGalleryGroup();
		}
		public void AddGalleryItem() {
			Helper.AddGalleryItem();
		}
		public void RemoveGalleryItem() {
			Helper.RemoveGalleryItem();
		}
		public void DockInParentContainer() {
			Helper.DockInParentContainer();
		}
		public void UndockInParentContainer() {
			Helper.UndockInParentContainer();
		}
		public override BaseGallery GalleryCore {
			get { return GalleryControl.Gallery; }
		}
	}
	public class GalleryControlClientDesigner : ParentControlDesigner {
		public override SelectionRules SelectionRules {
			get { return SelectionRules.None; }
		}
		protected override void PostFilterProperties(IDictionary properties) {
			base.PostFilterProperties(properties);
			DXPropertyDescriptor.ConvertDescriptors(properties, null);
		}
		protected virtual void ConvertDescriptors(IDictionary properties) {
			DXPropertyDescriptor.ConvertDescriptors(properties, null);
		}
	}
	public class GalleryControlDesigner : RibbonDesignerWithVerbs, IKeyCommandProcessInfo {
		GalleryControlKeyCommandProcessHelper keyCommandProcessHelper;
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			CreateHelper();
			this.keyCommandProcessHelper = new GalleryControlKeyCommandProcessHelper(this);
		}
		#region IKeyCommandProcessInfo
		IServiceProvider IKeyCommandProcessInfo.ServiceProvider {
			get { return Component.Site; }
		}
		BaseDesignTimeManager IKeyCommandProcessInfo.DesignTimeManager {
			get { return DesignTimeManager; }
		}
		#endregion
		protected GalleryControlKeyCommandProcessHelper KeyCommandProcessHelper { get { return keyCommandProcessHelper; } }
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(KeyCommandProcessHelper != null) {
					KeyCommandProcessHelper.Dispose();
				}
				this.keyCommandProcessHelper = null;
				if(Helper != null) {
					Helper.ComponentChangeService.ComponentChanged -= new ComponentChangedEventHandler(OnComponentChanged);
					Helper.ComponentChangeService.ComponentAdded -= new ComponentEventHandler(OnComponentAdded);
				}
			}
			base.Dispose(disposing);
		}
		public GalleryControl GalleryControl { get { return Component as GalleryControl; } }
		protected override void CreateRibbonDesignerActionLists(DesignerActionListCollection list) {
			list.Add(new GalleryControlDesignerActionList(Component));
		}
		public override ICollection AssociatedComponents {
			get {
				if(Ribbon == null) return base.AssociatedComponents;
				ArrayList controls = new ArrayList();
				if(GalleryControl.Gallery.Images != null) controls.Add(GalleryControl.Gallery.Images);
				if(GalleryControl.Gallery.HoverImages != null) controls.Add(GalleryControl.Gallery.HoverImages);
				AddBase(controls);
				return controls;
			}
		}
		GalleryControlClient GetControlClient(GalleryControl galleryControl) {
			foreach(Control c in galleryControl.Controls) {
				if(c is GalleryControlClient)
					return (GalleryControlClient)c;
			}
			return null;
		}
		bool allowRemoveGallery = false;
		protected override void OnComponentRemoving(object sender, ComponentEventArgs e) {
			if(!IsDisposed) {
				GalleryControlGallery gallery = GetGallery(e);
				if(gallery != null && gallery.GalleryControl == GalleryControl && !allowRemoveGallery) {
					throw new ApplicationException("Gallery cannot be removed in this way. To remove Gallery, remove associated GalleryControl");
				}
				if(e.Component == GalleryControl) {
					GalleryControlClient cl = GetControlClient(GalleryControl);
					if(cl != null) {
						GalleryControl.Container.Remove(cl);
						cl.GalleryControl = null;
						allowRemoveGallery = true;
						GalleryControl.Container.Remove(GalleryControl.Gallery);
						allowRemoveGallery = false;
					}
				}
			}
			base.OnComponentRemoving(sender, e);
		}
		protected virtual GalleryControlGallery GetGallery(ComponentEventArgs e) {
			if(e.Component is PopupGalleryEditGallery)
				return null;
			return e.Component as GalleryControlGallery;
		}
		protected override void ShowDesignerForm(RibbonControl ribbon) {
			if(GalleryControl == null)
				return;
			IUIService srv = GetService(typeof(IUIService)) as IUIService;
			using(RibbonEditorForm form = new RibbonEditorForm(GalleryControl.Gallery)) {
				form.InitEditingObject(GalleryControl.Gallery);
				form.ShowDialog(srv == null ? null : srv.GetDialogOwnerWindow());
			}
		}
		protected override void OnDesignerClick(object sender, EventArgs e) {
			ShowDesignerForm(null);
		}
		protected override RibbonHitInfo CalcHitInfo(Point point) {
			if(GalleryControl == null)
				return base.CalcHitInfo(point);
			Point pt = GalleryControl.PointToClient(point);
			return GalleryControl.CalcHitInfo(pt);
		}
		protected override bool GetHitTest(Point point) {
			if(GalleryControl == null) return base.GetHitTest(point);
			if(DebuggingState) return base.GetHitTest(point);
			RibbonHitInfo hitInfo = CalcHitInfo(point);
			if(hitInfo.InGalleryItem || hitInfo.InGalleryGroup || hitInfo.InGalleryScrollBar) return true;
			return base.GetHitTest(point);
		}
		GalleryControlDesignTimeManager DesignTimeManager { get { return GalleryControl.GetDesignTimeManager(); } }
		public override DesignerVerbCollection DXVerbs {
			get {
				DesignerVerbCollection coll = base.DXVerbs;
				DesignerVerb addGroupVerb = new DesignerVerb("Add GalleryGroup", new EventHandler(OnAddGalleryGroupClick));
				DesignerVerb removeGroupVerb = new DesignerVerb("Remove GalleryGroup", new EventHandler(OnRemoveGalleryGroupClick));
				DesignerVerb addItemVerb = new DesignerVerb("Add GalleryItem", new EventHandler(OnAddGalleryItemClick));
				DesignerVerb removeItemVerb = new DesignerVerb("Remove GalleryItem", new EventHandler(OnRemoveGalleryItemClick));
				UpdateVerbsEnabledState(removeGroupVerb, addItemVerb, removeItemVerb);
				coll.Add(addGroupVerb);
				coll.Add(removeGroupVerb);
				coll.Add(addItemVerb);
				coll.Add(removeItemVerb);
				return coll;
			}
		}
		GalleryControlDesignerHelper helper;
		public GalleryControlDesignerHelper Helper { get { return helper; } }
		private void CreateHelper() {
			this.helper = new GalleryControlDesignerHelper(Component);
			helper.ComponentChangeService.ComponentChanged += new ComponentChangedEventHandler(OnComponentChanged);
			helper.ComponentChangeService.ComponentAdded += new ComponentEventHandler(OnComponentAdded);
		}
		void OnComponentAdded(object sender, ComponentEventArgs e) {
			BarAndDockingController controller = e.Component as BarAndDockingController;
			if(controller != null && GalleryControl.Controller == null)
				GalleryControl.Controller = controller;
		}
		protected DesignerVerb GetVerbByText(string text) {
			foreach(DesignerVerb verb in Verbs) {
				if(verb.Text == text)
					return verb;
			}
			return null;
		}
		protected virtual void UpdateVerbsEnabledState(DesignerVerb removeGroupVerb, DesignerVerb addItemVerb, DesignerVerb removeItemVerb) {
			removeGroupVerb.Enabled = false;
			addItemVerb.Enabled = false;
			removeItemVerb.Enabled = false;
			if(DesignTimeManager.DragItem is GalleryItemGroup) {
				removeGroupVerb.Enabled = true;
				addItemVerb.Enabled = true;
			}
			if(DesignTimeManager.DragItem is GalleryItem) {
				removeItemVerb.Enabled = true;
				addItemVerb.Enabled = true;
			}
		}
		protected virtual void UpdateVerbsEnabledState() {
			if(DebuggingState)
				return;
			UpdateVerbsEnabledState(GetVerbByText("Remove GalleryGroup"), GetVerbByText("Add GalleryItem"), GetVerbByText("Remove GalleryItem"));
		}
		protected virtual void OnComponentChanged(object sender, ComponentChangedEventArgs e) {
			if(GalleryControl != null)
				UpdateVerbsEnabledState();
		}
		protected virtual void OnAddGalleryGroupClick(object sender, EventArgs e) {
			Helper.AddGalleryGroup();
		}
		protected virtual void OnRemoveGalleryGroupClick(object sender, EventArgs e) {
			Helper.RemoveGalleryGroup();
		}
		protected virtual void OnAddGalleryItemClick(object sender, EventArgs e) {
			Helper.AddGalleryItem();
		}
		protected virtual void OnRemoveGalleryItemClick(object sender, EventArgs e) {
			Helper.RemoveGalleryItem();
		}
		public override SelectionRules SelectionRules {
			get {
				SelectionRules res = base.SelectionRules & (SelectionRules.Locked | SelectionRules.Visible);
				if(GalleryControl.Gallery.AutoSize == GallerySizeMode.Both) {
					return res | SelectionRules.Moveable;
				}
				if(GalleryControl.Gallery.AutoSize == GallerySizeMode.Vertical) {
					return res | SelectionRules.Moveable | SelectionRules.RightSizeable | SelectionRules.LeftSizeable;
				}
				return res | SelectionRules.AllSizeable | SelectionRules.Moveable;
			}
		}
		protected override bool CanUseComponentSmartTags {
			get { return true; }
		}
	}
	public class RibbonMiniToolbarDesigner : BaseComponentDesigner {
		public RibbonMiniToolbar Toolbar { get { return Component as RibbonMiniToolbar; } }
		public RibbonControl Ribbon { get { return Toolbar.Ribbon; } }
		protected override bool AllowInheritanceWrapper { get { return true; } }
		protected override bool UseVerbsAsActionList { get { return true; } }
		protected virtual bool AllowCustomize { get { return true; } }
		protected override bool AllowHookDebugMode { get { return true; } }
		public override DesignerVerbCollection DXVerbs {
			get {
				DesignerVerbCollection verbs = new DesignerVerbCollection();
				verbs.Add(new DesignerVerb("Run Designer", new EventHandler(OnRunDesigner)));
				return verbs;
			}
		}
		protected virtual void OnRunDesigner(object sender, EventArgs e) {
			IUIService srv = GetService(typeof(IUIService)) as IUIService;
			using(RibbonEditorForm form = new RibbonEditorForm(Component)) {
				form.InitEditingObject(Ribbon);
				form.ShowDialog(srv == null ? null : srv.GetDialogOwnerWindow());
			}   
		}
	}
	public class RecentControlPanelDesigner : BaseComponentDesigner {
		protected override void PostFilterProperties(IDictionary properties) {
			base.PostFilterProperties(properties);
			DXPropertyDescriptor.ConvertDescriptors(properties, null);
		}
		protected override bool AllowInheritanceWrapper { get { return true; } }
		protected override bool AllowEditInherited {
			get {
				return false;
			}
		}
		protected override DesignerActionListCollection CreateActionLists() {
			DesignerActionListCollection list = base.CreateActionLists();
			list.Insert(0, new RecentControlPanelDesignerActionList(Component));
			return list;
		}
	}
	public class RecentControlPanelDesignerActionList : DesignerActionList {
		public RecentControlPanelDesignerActionList(IComponent component) : base(component) { }
		RecentPanelBase Panel { get { return base.Component as RecentPanelBase; } }
		RecentItemControl RecentControl { get { return Panel.RecentControl; } }
		RecentControlDesignTimeManager designTimeManagerCore = null;
		protected RecentControlDesignTimeManager DesignTimeManager {
			get {
				if(designTimeManagerCore == null) {
					designTimeManagerCore = RecentControl.GetDesignTimeManager();
				}
				return designTimeManagerCore;
			}
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection res = new DesignerActionItemCollection();
			res.Add(new DesignerActionMethodItem(this, "OnAddLabelItem", "Add LabelItem", "Commands", true));
			res.Add(new DesignerActionMethodItem(this, "OnAddButtonItem", "Add ButtonItem", "Commands", true));
			res.Add(new DesignerActionMethodItem(this, "OnAddSeparatorItem", "Add SeparatorItem", "Commands", true));
			res.Add(new DesignerActionMethodItem(this, "OnAddRecentItem", "Add RecentItem", "Commands", true));
			return res;
		}
		protected virtual void OnAddLabelItem() {
			DesignTimeManager.OnAddLabelItem(Panel);
		}
		protected virtual void OnAddButtonItem() {
			DesignTimeManager.OnAddButtonItem(Panel);
		}
		protected virtual void OnAddSeparatorItem() {
			DesignTimeManager.OnAddSeparatorItem(Panel);
		}
		protected virtual void OnAddRecentItem() {
			DesignTimeManager.OnAddRecentItem(Panel);
		}
	}
	public class RecentControlDesigner : BaseControlDesigner {
		public RecentItemControl RecentControl { get { return Control as RecentItemControl; } }
		protected override bool CanUseComponentSmartTags {
			get {
				return true;
			}
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			ComponentChangeService.ComponentAdded += OnComponentAdded;
			ComponentChangeService.ComponentRemoving += OnComponentRemoving;
		}
		void OnComponentRemoving(object sender, ComponentEventArgs e) {
			RecentItemBase item = e.Component as RecentItemBase;
			if(item == null || item.OwnerPanel == null)
				return;
			item.OwnerPanel.Items.Remove(item);
			RecentTabItem tab = item as RecentTabItem;
		}
		void OnComponentAdded(object sender, ComponentEventArgs e) {
		}
		IComponentChangeService componentChangeService;
		public IComponentChangeService ComponentChangeService {
			get {
				if(componentChangeService == null)
					componentChangeService = Component.Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
				return componentChangeService;
			}
		}
		protected override DesignerActionListCollection CreateActionLists() {
			DesignerActionListCollection list = base.CreateActionLists();
			list.Insert(0, new RecentControlDesignerActionList(Component, this));
			return list;
		}
		protected override bool GetHitTest(Point point) {
			if(RecentControl == null) return base.GetHitTest(point);
			RecentControlHitInfo hitInfo = RecentControl.CalcHitInfo(RecentControl.PointToClient(point));
			if(hitInfo.HitTest != RecentControlHitTest.None && hitInfo.HitTest != RecentControlHitTest.Title && hitInfo.HitTest != RecentControlHitTest.Splitter) return true;
			return base.GetHitTest(point);
		}
	}
	public class RecentControlDesignerActionList : DesignerActionList {
		IDesigner designer;
		public RecentControlDesignerActionList(IComponent component, IDesigner designer)
			: base(component) {
			this.designer = designer;
		}
		RecentItemControl RecentControl { get { return base.Component as RecentItemControl; } }
		RecentControlDesignTimeManager designTimeManagerCore = null;
		protected RecentControlDesignTimeManager DesignTimeManager {
			get {
				if(designTimeManagerCore == null) {
					designTimeManagerCore = RecentControl.GetDesignTimeManager();
				}
				return designTimeManagerCore;
			}
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection res = new DesignerActionItemCollection();
			res.Add(new DesignerActionPropertyItem("Title", "Title", "Appearance"));
			if(RecentControl.Dock != DockStyle.Fill)
				res.Add(new DesignerActionMethodItem(this, "DockInParentContainer", "Dock in parent container", "Behavior"));
			else
				res.Add(new DesignerActionMethodItem(this, "UndockInParentContainer", "Undock in parent container", "Behavior"));
			return res;
		}
		public virtual string Title {
			get { return RecentControl.Title; }
			set { EditorContextHelper.SetPropertyValue(designer, RecentControl, "Title", value); }
		}
		protected virtual void DockInParentContainer() {
			RecentControl.Dock = DockStyle.Fill;
			DesignerActionUIService.Refresh(Component);
		}
		protected virtual void UndockInParentContainer() {
			RecentControl.Dock = DockStyle.None;
			DesignerActionUIService.Refresh(Component);
		}
		#region Services
		DesignerActionUIService designerActionUIService;
		protected DesignerActionUIService DesignerActionUIService {
			get {
				if(designerActionUIService == null)
					designerActionUIService = Component.Site.GetService(typeof(DesignerActionUIService)) as DesignerActionUIService;
				return designerActionUIService;
			}
		}
		#endregion
	}
	public class RecentControlContentContainerDesigner : ParentControlDesigner {
		public override SelectionRules SelectionRules {
			get {
				return SelectionRules.Visible | SelectionRules.BottomSizeable;
			}
		}
		protected override bool DrawGrid {
			get {
				if(base.DrawGrid) return true; 
				return true;
			}
		}
	}
	public class BackstageViewItemDesigner : BaseComponentDesigner {
		protected override void PostFilterProperties(IDictionary properties) {
			base.PostFilterProperties(properties);
			DXPropertyDescriptor.ConvertDescriptors(properties, null);
		}
		protected override bool AllowInheritanceWrapper { get { return true; } }
		protected override bool AllowEditInherited {
			get {
				return false;
			}
		}
		BarItem Item { get { return (BarItem)Component; } }
	}
	public class BackstageViewControlDesigner : BaseControlDesigner {
		public BackstageViewControl BackstageView { get { return Control as BackstageViewControl; } }
		protected override bool AllowInheritanceWrapper { get { return true; } }
		protected override bool UseVerbsAsActionList { get { return false; } }
		protected virtual bool AllowCustomize { get { return true; } }
		protected override bool AllowHookDebugMode { get { return true; } }
		protected override bool CanUseComponentSmartTags { get { return true; } }
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			BackstageView.Controller = DesignHelpers.FindComponent(BackstageView.Container, typeof(BarAndDockingController)) as BarAndDockingController;
			ComponentChangeService.ComponentAdded += OnComponentAdded;
			ComponentChangeService.ComponentRemoving += OnComponentRemoving;
		}
		void OnComponentRemoving(object sender, ComponentEventArgs e) {
			BackstageViewItemBase item = e.Component as BackstageViewItemBase;
			if(item == null || BackstageView == null || !BackstageView.Items.Contains(item))
				return;
			BackstageView.Items.Remove(item);
			BackstageViewTabItem tab = item as BackstageViewTabItem;
			if(tab != null) {
				Control contentControl = tab.ContentControl;
				tab.ContentControl = null;
				Control.Container.Remove(contentControl);
				contentControl.Dispose();
			}
		}
		void OnComponentAdded(object sender, ComponentEventArgs e) {
			BarAndDockingController controller = e.Component as BarAndDockingController;
			if(controller != null && BackstageView != null)
				BackstageView.Controller = controller;
		}
		IComponentChangeService componentChangeService;
		public IComponentChangeService ComponentChangeService {
			get {
				if(componentChangeService == null)
					componentChangeService = Component.Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
				return componentChangeService;
			}
		}
		protected override DesignerActionListCollection CreateActionLists() {
			DesignerActionListCollection list = base.CreateActionLists();
			list.Insert(0, new BackstageViewControlDesignerActionList(Component, this));
			return list;
		}
		protected override bool GetHitTest(Point point) {
			if(BackstageView.GetViewInfo().GetItemByPoint(BackstageView.PointToClient(point), true) != null || BackstageView.GetViewInfo().DragItem != null)
				return true;
			return base.GetHitTest(point);
		}
	}
	public class BackstageViewControlDesignerActionList : DesignerActionList {
		IDesigner designer;
		public BackstageViewControlDesignerActionList(IComponent component, IDesigner designer) : base(component) {
			this.designer = designer;
		}
		protected virtual void OnAddCommand() {
			DesignTimeManager.OnAddCommand();
		}
		protected virtual void OnAddTab() {
			DesignTimeManager.OnAddTab();
		}
		protected virtual void OnAddSeparator() {
			DesignTimeManager.OnAddSeparator();
		}
		protected virtual void OnFillTabs() {
			DesignTimeManager.OnFillTabs();
			DesignHelpers.RefreshSmartTag(Component);
		}
		public virtual BackstageViewStyle BackstageViewStyle {
			get { return BackstageView.Style; }
			set { EditorContextHelper.SetPropertyValue(designer, BackstageView, "Style", value); }
		}
		protected virtual void DockInParentContainer() {
			BackstageView.Dock = DockStyle.Fill;
			DesignerActionUIService.Refresh(Component);
		}
		protected virtual void UndockInParentContainer() {
			BackstageView.Dock = DockStyle.None;
			DesignerActionUIService.Refresh(Component);
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection res = new DesignerActionItemCollection();
			res.Add(new DesignerActionMethodItem(this, "OnAddCommand", "Add Command", "Commands", true));
			res.Add(new DesignerActionMethodItem(this, "OnAddTab", "Add Tab", "Commands", true));
			res.Add(new DesignerActionMethodItem(this, "OnAddSeparator", "Add Separator", "Commands", true));
			res.Add(new DesignerActionPropertyItem("BackstageViewStyle", "Style", "Appearance"));
			if(BackstageView.Dock != DockStyle.Fill)
				res.Add(new DesignerActionMethodItem(this, "DockInParentContainer", "Dock in parent container", "Behavior"));
			else
				res.Add(new DesignerActionMethodItem(this, "UndockInParentContainer", "Undock in parent container", "Behavior"));
			res.Add(new DesignerActionMethodItem(this, "OnFillTabs", "Fill Child BackstageView", "Commands", true));
			return res;
		}
		#region Services
		DesignerActionUIService designerActionUIService;
		protected DesignerActionUIService DesignerActionUIService {
			get {
				if(designerActionUIService == null)
					designerActionUIService = Component.Site.GetService(typeof(DesignerActionUIService)) as DesignerActionUIService;
				return designerActionUIService;
			}
		}
		#endregion
		BackstageViewDesignTimeManager designTimeManagerCore = null;
		protected BackstageViewDesignTimeManager DesignTimeManager {
			get {
				if(designTimeManagerCore == null) {
					designTimeManagerCore = BackstageView.GetDesignTimeManager();
				}
				return designTimeManagerCore;
			}
		}
		BackstageViewControl BackstageView { get { return base.Component as BackstageViewControl; } }
	}
	internal class BackstageViewControlBehavior : DragDropControlBehavior {
		public BackstageViewControlBehavior(Control owner) : base(owner) { }
		protected override bool ShouldProcessDragEvent(DragEventArgs e) {
			string[] formats = e.Data.GetFormats();
			for(int i = 0; i < formats.Length; i++) {
				if(e.Data.GetData(formats[i]) is BackstageViewItemBase)
					return true;
			}
			return false;
		}
	}
	internal class BackstageViewBodyGlyph : ControlBodyGlyph {
		public BackstageViewBodyGlyph(Rectangle bounds, Cursor cursor, Control owner)
			: base(bounds, cursor, owner, new BackstageViewControlBehavior(owner)) { 
		}
	}
	public class BackstageViewControlClientDesigner : BaseParentControlDesigner {
		public override void Initialize(IComponent component) {
			base.Initialize(component);
		}
		public override SelectionRules SelectionRules {
			get { return SelectionRules.None; }
		}
		public override IList SnapLines {
			get {
				IList list = base.SnapLines;
				list.Clear();
				return list;
			}
		}
		public override bool CanBeParentedTo(IDesigner parentDesigner) {
			return parentDesigner is BackstageViewControlDesigner;
		}
		public override DesignerActionListCollection ActionLists { get { return null; } }
	}
	public class RibbonControlDesigner : RibbonDesignerWithVerbs, IBarCommandSupports, IKeyCommandProcessInfo {
		public RibbonControlDesigner()
			: base() {
			DXVerbs.Insert(1, new DesignerVerb("Add MiniToolbar", new EventHandler(OnAddMiniToolbar)));
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			if(ChangeService != null) ChangeService.ComponentRemoved += ComponentRemoved;
			keyCommandProcessHelper = new DesignTimeKeyCommandProcessHelperBase(this);
			if(DesignerHost != null) DesignerHost.LoadComplete += OnDesignerHostLoadComplete;
		}
		protected virtual void ComponentRemoved(object sender, ComponentEventArgs e) {
			PopupControl popupControl = e.Component as PopupControl;
			if(popupControl == null) return;
			ClearReferencesToPopupControl(popupControl);
		}
		void ClearReferencesToPopupControl(PopupControl popup) {
			if(Ribbon.Items == null) return;
			foreach(var barItem in Ribbon.Items) {
				BarButtonItem button = barItem as BarButtonItem;
				if(button != null && button.DropDownControl == popup) button.DropDownControl = null;
			}
		}
		DesignTimeKeyCommandProcessHelperBase keyCommandProcessHelper;
		protected override RibbonControl Ribbon { get { return Control as RibbonControl; } }
		protected override RibbonHitInfo CalcHitInfo(Point point) {
			point = Ribbon.PointToClient(point);
			return Ribbon.CalcHitInfo(point);
		}
		void OnAddMiniToolbar(object sender, EventArgs e) {
			RibbonMiniToolbar toolbar = new RibbonMiniToolbar();
			Ribbon.Container.Add(toolbar);
			Ribbon.MiniToolbars.Add(toolbar);
		}
		protected override void CreateRibbonDesignerActionLists(DesignerActionListCollection list) {
			list.Add(new RibbonDesignerActionList(Component));
		}
		protected override bool AllowEditInherited { get { return true; } }
		protected override bool AllowInheritanceWrapper { get { return true; } }
		protected override void OnAboutClick(object sender, EventArgs e) {
			RibbonControl.About();
		}
		protected override void OnDesignerClick(object sender, EventArgs e) {
			ShowDesignerForm(Ribbon);
		}
		protected override void OnInitialize() {
			base.OnInitialize();
			Ribbon.Controller = DesignHelpers.FindComponent(Ribbon.Container, typeof(BarAndDockingController)) as BarAndDockingController;
			RibbonStatusBar statusBar = DesignHelpers.FindComponent(Ribbon.Container, typeof(RibbonStatusBar)) as RibbonStatusBar;
			if(statusBar != null && statusBar.Ribbon == null) statusBar.Ribbon = Ribbon;
			try {
				if(DesignerHost != null && DesignerHost.Container != null) {
					RibbonPage page = new RibbonPage();
					RibbonPageGroup group = new RibbonPageGroup();
					DesignerHost.Container.Add(page);
					DesignerHost.Container.Add(group);
					page.Groups.Add(group);
					page.Text = page.Name;
					group.Text = group.Name;
					Ribbon.Pages.Add(page);
				}
				SetManagerPopupMenu();
			}
			catch {
			}
		}
		protected bool IsVisualInheritance() {
			if(Ribbon == null) return false;
			Form form = Ribbon.FindForm();
			if(form == null) return false;
			Type formType = form.GetType();
			return formType != typeof(Form) && formType != typeof(XtraForm) && formType != typeof(RibbonForm);
		}
		void OnDesignerHostLoadComplete(object sender, EventArgs e) {
			OnDesignerHostLoadCompleteCore();
		}
		protected virtual void OnDesignerHostLoadCompleteCore() {
			if(IsVisualInheritance())  Ribbon.ForceInitialize(true);
		}
		IDesignerHost designerHost = null;
		protected IDesignerHost DesignerHost {
			get {
				if(designerHost == null) designerHost = GetService(typeof(IDesignerHost)) as IDesignerHost;
				return designerHost;
			}
		}
		void SetManagerPopupMenu() {
			foreach(object menu in Component.Site.Container.Components) {
				PopupControl popupControl = menu as PopupControl;
				if(popupControl != null && popupControl.Ribbon == null)
					popupControl.Ribbon = Ribbon;
			}
		}
		protected override bool CanUseComponentSmartTags {
			get { return true; }
		}
		protected override void WndProc(ref Message m) {
			if(m.Msg == MSG.WM_SETCURSOR) {
				m.Result = new IntPtr(1);
				return;
			}
			base.WndProc(ref m);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(DesignerHost != null) 
					DesignerHost.LoadComplete -= OnDesignerHostLoadComplete;
				if(KeyCommandProcessHelper != null) 
					KeyCommandProcessHelper.Dispose();
				if(ChangeService != null)
					ChangeService.ComponentRemoved -= ComponentRemoved;
			}
			base.Dispose(disposing);			
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
			get { return new RibbonCommandContext(Ribbon); }
		}
		#endregion
		protected DesignTimeKeyCommandProcessHelperBase KeyCommandProcessHelper { 
			get { return keyCommandProcessHelper; } 
		}
	}
}
