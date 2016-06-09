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
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;
using DevExpress.XtraBars.Ribbon;
using DevExpress.Utils.Design;
using System.ComponentModel.Design.Serialization;
using DevExpress.XtraBars.Alerter;
using DevExpress.XtraEditors;
using System.Drawing.Design;
using DevExpress.Utils.Menu;
using DevExpress.Utils;
using DevExpress.XtraBars.Ribbon.Customization;
using DevExpress.Utils.Controls;
using DevExpress.Utils.About;
using DevExpress.XtraBars.Ribbon.Design;
using System.Collections.Generic;
using System.Windows.Forms.Design.Behavior;
using DevExpress.XtraBars.Navigation;
using System.Globalization;
using System.Reflection;
using DevExpress.XtraTab;
namespace DevExpress.Utils.Design {
}
namespace DevExpress.XtraBars.Design {
	public class AlertControlDesigner : BaseComponentDesignerSimple {
		protected AlertControl Control { get { return Component as AlertControl; } }
	}
	public class BarItemDesigner : BaseComponentDesigner {
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
	public class BarDesigner : BaseComponentDesigner {
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
		Bar Bar { get { return (Bar)Component; } }
	}
	#region TILEBAR DT
	public class TileBarDesignTimeManager : TileBarDesignTimeManagerBase {
		public TileBarDesignTimeManager(IComponent component, TileBar tileBar)
			: base(component, tileBar) {
		}
		protected override void FillDesignTimePopupMenu(DXPopupMenu popupMenu, bool isGroupMenu, TileControlHitInfo hitInfo) {
			base.FillDesignTimePopupMenu(popupMenu, isGroupMenu, hitInfo);
			if(isGroupMenu) return;
			DXMenuItem editElementsItem = new DXMenuItem() { Caption = "Edit TileItem Elements", Tag = hitInfo.ItemInfo };
			editElementsItem.Click += OnEditTileItemElementsCollectionClick;
			editElementsItem.Enabled = !TileBar.DebuggingState;
			popupMenu.Items.Add(editElementsItem);
		}
		protected internal virtual void OnEditTileItemElementsCollectionClick(object sender, EventArgs e) {
			OnEditElementsCollectionClickCore(GetItemFromSender(sender));
		}
		TileBarItem GetItemFromSender(object sender) {
			TileBarItem item;
			if(sender is TileBarItem)
				item = sender as TileBarItem;
			else
				item = ((TileBarItemViewInfo)((DXMenuItem)sender).Tag).ItemCore;
			return item;
		}
		public override void OnEditElementsCollectionClickCore(TileBarItem item) {
			TileBarItemTypeDescriptorContext context = new TileBarItemTypeDescriptorContext(this, item);
			UITypeEditor editor = TypeDescriptor.GetEditor(item.Elements, typeof(UITypeEditor)) as UITypeEditor;
			if(editor != null)
				editor.EditValue(context, context, item.Elements);
		}
		class TileBarItemTypeDescriptorContext : ITypeDescriptorContext {
			TileBarDesignTimeManager designTimeManager;
			TileBarItem instance;
			public TileBarItemTypeDescriptorContext(TileBarDesignTimeManager designTimeManager, TileBarItem instance) {
				this.designTimeManager = designTimeManager;
				this.instance = instance;
			}
			public IContainer Container {
				get { return designTimeManager.DesignerHost.Container; }
			}
			public object Instance {
				get { return this.instance; }
			}
			public void OnComponentChanged() {
				designTimeManager.ComponentChangeService.OnComponentChanged(this.instance, null, null, null);
			}
			public bool OnComponentChanging() {
				designTimeManager.ComponentChangeService.OnComponentChanging(this.instance, null);
				return true;
			}
			public PropertyDescriptor PropertyDescriptor {
				get {
					PropertyDescriptorCollection propertyCollection = TypeDescriptor.GetProperties(instance);
					return propertyCollection.Find("Elements", true);
				}
			}
			public object GetService(Type serviceType) {
				object service = this.designTimeManager.GetService(serviceType);
				if(service != null) return service;
				service = this.designTimeManager.DesignerHost.GetService(serviceType);
				if(service != null) return service;
				if(instance.Site != null)
					service = instance.Site.GetService(serviceType);
				if(service != null) return service;
				if(serviceType != typeof(IWindowsFormsEditorService)) return null;
				return new TileControlDesignTimeManager.TileItemElementsWindowsFormsEditorService();
			}
		}
	}
	public class TileBarDesigner : BaseControlDesigner, ITileControlDesigner, IKeyCommandProcessInfo {
		public new TileBar Component { get { return (TileBar)base.Component; } }
		public TileBar TileBarCore { get { return Component as TileBar; } }
		public TileBarViewInfo ViewInfoCore { get { return ((TileBarCore as ITileControl).ViewInfo as TileBarViewInfo); } }
		TileBarKeyCommandProcessHelper keyCommandProcessHelper = null;
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			ViewInfoCore.DesignTimeManager = new TileBarDesignTimeManager(Component, TileBarCore);
			SubscribeEvent();
			keyCommandProcessHelper = new TileBarKeyCommandProcessHelper(this);
		}
		public override ICollection AssociatedComponents {
			get {
				ArrayList list = new ArrayList();
				if(TileBarCore.Images != null) list.Add(TileBarCore.Images);
				AddGroups(list);
				AddBase(list);
				return list;
			}
		}
		protected virtual void CreateTileDesignerActionLists(DesignerActionListCollection list) {
			list.Add(new TileBarDesignerActionsList(this));
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			CreateTileDesignerActionLists(list);
			base.RegisterActionLists(list);
		}
		protected void AddGroups(ArrayList list) {
			foreach(TileBarGroup group in TileBarCore.Groups) {
				list.Add(group);
			}
		}
		protected void AddBase(ArrayList controls) {
			foreach(object obj in base.AssociatedComponents) {
				if(controls.Contains(obj)) continue;
				controls.Add(obj);
			}
		}
		protected override bool GetHitTest(Point point) {
			ITileControl tileControl = (ITileControl)TileBarCore;
			if(tileControl.Handler.State == TileControlHandlerState.DragMode)
				return true;
			Point pt = TileBarCore.PointToClient(point);
			TileControlHitInfo hitInfo = tileControl.ViewInfo.CalcHitInfo(pt);
			if(hitInfo.InItem || hitInfo.InGroup || hitInfo.InBackArrow || hitInfo.InForwardArrow || tileControl.ScrollBar != null && tileControl.ScrollBar.Bounds.Contains(pt))
				return true;
			return base.GetHitTest(point);
		}
		public override DesignerVerbCollection DXVerbs {
			get {
				DesignerVerbCollection coll = base.DXVerbs;
				DesignerVerb addGroupVerb = new DesignerVerb("Add Group", new EventHandler(OnAddTileBarGroupClick));
				coll.Add(addGroupVerb);
				return coll;
			}
		}
		protected virtual void OnAddTileBarGroupClick(object sender, EventArgs e) {
			Helper.OnAddTileGroupClick();
		}
		void SubscribeEvent() {
			(TileBarCore as ITileControlUpdateSmartTag).SmartTagUpdate += SmartTagUpdate;
		}
		void UnsubscribeEvent() {
			(TileBarCore as ITileControlUpdateSmartTag).SmartTagUpdate -= SmartTagUpdate;
		}
		void SmartTagUpdate(object sender, TileControlSmartTagEventArgs e) {
			try {
				BehaviorService.SyncSelection();
			}
			catch { }
		}
		TileBarDesignTimeManager helper;
		public TileBarDesignTimeManager Helper {
			get {
				if(helper == null)
					helper = new TileBarDesignTimeManager(TileBarCore, TileBarCore);
				return helper;
			}
		}
		protected override bool CanUseComponentSmartTags { get { return true; } }
		IServiceProvider IKeyCommandProcessInfo.ServiceProvider { get { return Component.Site; } }
		BaseDesignTimeManager IKeyCommandProcessInfo.DesignTimeManager {
			get { return ViewInfoCore.DesignTimeManager.GetBase(); }
		}
		bool ITileControlDesigner.DebuggingState { get { return DebuggingState; } }
		protected override void Dispose(bool disposing) {
			UnsubscribeEvent();
			if(disposing) {
				if(keyCommandProcessHelper != null) keyCommandProcessHelper.Dispose();
			}
			base.Dispose(disposing);
		}
	}
	public class TileBarGroupDesigner : BaseComponentDesigner {
		public override void Initialize(IComponent component) {
			base.Initialize(component);
		}
		public override ICollection AssociatedComponents {
			get {
				ArrayList list = new ArrayList();
				AddItems(list);
				AddBase(list);
				return list;
			}
		}
		protected virtual void AddItems(ArrayList list) {
			foreach(TileBarItem item in TileBarGroup.Items) {
				list.Add(item);
			}
		}
		protected void AddBase(ArrayList controls) {
			foreach(object obj in base.AssociatedComponents) {
				if(controls.Contains(obj)) continue;
				controls.Add(obj);
			}
		}
		public TileBarGroup TileBarGroup { get { return (TileBarGroup)Component; } }
		public override DesignerVerbCollection DXVerbs {
			get {
				DesignerVerbCollection coll = base.DXVerbs;
				DesignerVerb addGroupVerb = new DesignerVerb("Add Group", new EventHandler(OnAddTileGroupClick));
				DesignerVerb removeGroupVerb = new DesignerVerb("Remove Group", new EventHandler(OnRemoveTileGroupClick));
				DesignerVerb addItemVerb = new DesignerVerb("Add Medium Item", new EventHandler(OnAddTileItemClick));
				DesignerVerb addLargeItemVerb = new DesignerVerb("Add Wide Item", new EventHandler(OnAddLargeTileItemClick));
				coll.Add(addGroupVerb);
				coll.Add(removeGroupVerb);
				coll.Add(addItemVerb);
				coll.Add(addLargeItemVerb);
				return coll;
			}
		}
		protected override bool UseVerbsAsActionList { get { return true; } }
		TileBarDesignTimeManager helper;
		public TileBarDesignTimeManager Helper {
			get {
				if(helper == null) {
					helper = new TileBarDesignTimeManager(TileBarGroup, TileBarGroup.Control.Control as TileBar);
				}
				return helper;
			}
		}
		protected virtual void OnAddTileGroupClick(object sender, EventArgs e) {
			Helper.OnAddTileGroupClick();
		}
		protected virtual void OnRemoveTileGroupClick(object sender, EventArgs e) {
			Helper.OnRemoveTileGroupClick(TileBarGroup);
		}
		protected virtual void OnAddTileItemClick(object sender, EventArgs e) {
			Helper.OnAddTileItemClick();
		}
		protected virtual void OnAddLargeTileItemClick(object sender, EventArgs e) {
			Helper.OnAddLargeTileItemClick();
		}
	}
	public class TileBarItemDesigner : BaseComponentDesigner {
		public override void Initialize(IComponent component) {
			base.Initialize(component);
		}
		public override ICollection AssociatedComponents {
			get {
				ArrayList list = new ArrayList();
				AddBase(list);
				return list;
			}
		}
		public TileBarItem TileBarItem { get { return (TileBarItem)Component; } }
		protected virtual void AddItemInfos(ArrayList list) {
			foreach(TileItemFrame info in TileBarItem.Frames) {
				list.Add(info);
			}
		}
		protected void AddBase(ArrayList controls) {
			foreach(object obj in base.AssociatedComponents) {
				if(controls.Contains(obj)) continue;
				controls.Add(obj);
			}
		}
		public override DesignerVerbCollection DXVerbs {
			get {
				DesignerVerbCollection coll = base.DXVerbs;
				DesignerVerb addItemVerb = new DesignerVerb("Add Medium Item", new EventHandler(OnAddTileItemClick));
				DesignerVerb addLargeItemVerb = new DesignerVerb("Add Wide Item", new EventHandler(OnAddLargeTileItemClick));
				DesignerVerb removeItemVerb = new DesignerVerb("Remove Item", new EventHandler(OnRemoveTileItemClick));
				DesignerVerb editElementsInfoVerb = new DesignerVerb("Edit TileItem Elements", new EventHandler(OnEditItemElementsCollection));
				coll.Add(addItemVerb);
				coll.Add(addLargeItemVerb);
				coll.Add(removeItemVerb);
				coll.Add(editElementsInfoVerb);
				return coll;
			}
		}
		TileBarDesignTimeManager helper;
		public TileBarDesignTimeManager Helper {
			get {
				if(helper == null) {
					helper = new TileBarDesignTimeManager(TileBarItem, TileBarItem.Group.Control.Control as TileBar);
				}
				return helper;
			}
		}
		protected virtual void OnEditItemElementsCollection(object sender, EventArgs e) {
			Helper.OnEditTileItemElementsCollectionClick(TileBarItem, e);
		}
		protected virtual void OnRemoveTileItemClick(object sender, EventArgs e) {
			Helper.OnRemoveTileItemClick(TileBarItem);
		}
		protected virtual void OnAddTileItemClick(object sender, EventArgs e) {
			Helper.OnAddTileItemClick();
		}
		protected virtual void OnAddLargeTileItemClick(object sender, EventArgs e) {
			Helper.OnAddLargeTileItemClick();
		}
	}
	public class TileBarDesignerActionsList : DesignerActionList {
		IDesigner designer;
		public TileBarDesignerActionsList(IDesigner designer)
			: base(designer.Component) {
			this.designer = designer;
		}
		TileBarDesignTimeManager helper;
		public TileBarDesignTimeManager Helper {
			get {
				if(helper == null)
					helper = new TileBarDesignTimeManager(ControlCore, ControlCore);
				return helper;
			}
		}
		public IDesigner Designer { get { return designer; } }
		public TileBar ControlCore { get { return Component as TileBar; } }
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection res = new DesignerActionItemCollection();
			res.Add(new DesignerActionHeaderItem("Appearance", "Appearance"));
			res.Add(new DesignerActionPropertyItem("Dock", "Choose Dock Style", "Appearance"));
			res.Add(new DesignerActionPropertyItem("BackgroundImage", "Background Image", "Appearance"));
			res.Add(new DesignerActionPropertyItem("BackgroundImageLayout", "Background Image Layout", "Appearance"));
			res.Add(new DesignerActionPropertyItem("ShowGroupText", "Show Group Text", "Appearance"));
			res.Add(new DesignerActionHeaderItem("Behavior", "Behavior"));
			res.Add(new DesignerActionPropertyItem("AllowSelectedItem", "Allow Selected Item", "Behavior"));
			res.Add(new DesignerActionPropertyItem("AllowItemHover", "Allow Item Hover", "Behavior"));
			res.Add(new DesignerActionPropertyItem("SelectionColor", "Selection Color", "Behavior"));
			res.Add(new DesignerActionHeaderItem("Actions", "Actions"));
			res.Add(new DesignerActionMethodItem(this, "AddGroup", "Add Group", "Actions"));
			return res;
		}
		[Editor(typeof(DXImageEditor), typeof(UITypeEditor))]
		public virtual Image BackgroundImage {
			get { return ControlCore.BackgroundImage; }
			set { EditorContextHelper.SetPropertyValue(Designer, Component, "BackgroundImage", value); }
		}
		public virtual ImageLayout BackgroundImageLayout {
			get { return ControlCore.BackgroundImageLayout; }
			set { EditorContextHelper.SetPropertyValue(Designer, Component, "BackgroundImageLayout", value); }
		}
		public virtual void AddGroup() {
			Helper.OnAddTileGroupClick();
		}
		public virtual bool ShowGroupText {
			get { return ControlCore.ShowGroupText; }
			set { EditorContextHelper.SetPropertyValue(Designer, Component, "ShowGroupText", value); }
		}
		public virtual bool AllowSelectedItem {
			get { return ControlCore.AllowSelectedItem; }
			set { EditorContextHelper.SetPropertyValue(Designer, Component, "AllowSelectedItem", value); }
		}
		public virtual bool AllowItemHover {
			get { return ControlCore.AllowItemHover; }
			set { EditorContextHelper.SetPropertyValue(Designer, Component, "AllowItemHover", value); }
		}
		public virtual Color SelectionColor {
			get { return ControlCore.SelectionColor; }
			set { EditorContextHelper.SetPropertyValue(Designer, Component, "SelectionColor", value); }
		}
		public virtual DockStyle Dock {
			get {
				if(ControlCore == null) return DockStyle.None;
				return ControlCore.Dock;
			}
			set { EditorContextHelper.SetPropertyValue(designer, ControlCore, "Dock", value); }
		}
	}
	#endregion TILEBAR DT
	public class TileItemDesigner : BaseComponentDesigner {
		public override void Initialize(IComponent component) {
			base.Initialize(component);
		}
		public override ICollection AssociatedComponents {
			get {
				ArrayList list = new ArrayList();
				AddBase(list);
				return list;
			}
		}
		public TileItem Item { get { return (TileItem)Component; } }
		protected virtual void AddItemInfos(ArrayList list) {
			foreach(TileItemFrame info in Item.Frames) {
				list.Add(info);
			}
		}
		protected void AddBase(ArrayList controls) {
			foreach(object obj in base.AssociatedComponents) {
				if(controls.Contains(obj)) continue;
				controls.Add(obj);
			}
		}
		public TileItem TileItem { get { return (TileItem)Component; } }
		public override DesignerVerbCollection DXVerbs {
			get {
				DesignerVerbCollection coll = base.DXVerbs;
				DesignerVerb addItemVerb = new DesignerVerb("Add Item", new EventHandler(OnAddTileItemClick));
				DesignerVerb addLargeItemVerb = new DesignerVerb("Add Large Item", new EventHandler(OnAddLargeTileItemClick));
				DesignerVerb removeItemVerb = new DesignerVerb("Remove Item", new EventHandler(OnRemoveTileItemClick));
				DesignerVerb editFrameInfoVerb = new DesignerVerb("Edit Animation Frames", new EventHandler(OnEditItemFrameCollection));
				DesignerVerb editElementsInfoVerb = new DesignerVerb("Edit TileItem Elements", new EventHandler(OnEditItemElementsCollection));
				DesignerVerb selectTileTemplateVerb = new DesignerVerb("Select TileItem Template", new EventHandler(OnSelectItemTemplate));
				coll.Add(addItemVerb);
				coll.Add(addLargeItemVerb);
				coll.Add(removeItemVerb);
				coll.Add(editFrameInfoVerb);
				coll.Add(editElementsInfoVerb);
				coll.Add(selectTileTemplateVerb);
				return coll;
			}
		}
		TileControlDesignTimeManager helper;
		public TileControlDesignTimeManager Helper {
			get {
				if(helper == null) {
					helper = TileControlDesignTimeManagerFactory.CreateManager(TileItem, TileItem.Group.Control);
				}
				return helper;
			}
		}
		protected virtual void OnEditItemFrameCollection(object sender, EventArgs e) {
			Helper.OnEditFramesInfoCollectionClick(TileItem, e);
		}
		protected virtual void OnEditItemElementsCollection(object sender, EventArgs e) {
			Helper.OnEditTileItemElementsCollectionClick(TileItem, e);
		}
		protected virtual void OnSelectItemTemplate(object sender, EventArgs e) {
			Helper.OnEditTileItemTemplateClick(TileItem, e);
		}
		protected virtual void OnRemoveTileItemClick(object sender, EventArgs e) {
			Helper.OnRemoveTileItemClick(TileItem);
		}
		protected virtual void OnAddTileItemClick(object sender, EventArgs e) {
			Helper.OnAddTileItemClick();
		}
		protected virtual void OnAddLargeTileItemClick(object sender, EventArgs e) {
			Helper.OnAddLargeTileItemClick();
		}
	}
	public class TileGroupDesigner : BaseComponentDesigner {
		public override void Initialize(IComponent component) {
			base.Initialize(component);
		}
		public override ICollection AssociatedComponents {
			get {
				ArrayList list = new ArrayList();
				AddItems(list);
				AddBase(list);
				return list;
			}
		}
		public TileGroup Group { get { return (TileGroup)Component; } }
		protected virtual void AddItems(ArrayList list) {
			foreach(TileItem item in Group.Items) {
				list.Add(item);
			}
		}
		protected void AddBase(ArrayList controls) {
			foreach(object obj in base.AssociatedComponents) {
				if(controls.Contains(obj)) continue;
				controls.Add(obj);
			}
		}
		public TileGroup TileGroup { get { return (TileGroup)Component; } }
		public override DesignerVerbCollection DXVerbs {
			get {
				DesignerVerbCollection coll = base.DXVerbs;
				DesignerVerb addGroupVerb = new DesignerVerb("Add Group", new EventHandler(OnAddTileGroupClick));
				DesignerVerb removeGroupVerb = new DesignerVerb("Remove Group", new EventHandler(OnRemoveTileGroupClick));
				DesignerVerb addItemVerb = new DesignerVerb("Add Item", new EventHandler(OnAddTileItemClick));
				DesignerVerb addLargeItemVerb = new DesignerVerb("Add Large Item", new EventHandler(OnAddLargeTileItemClick));
				coll.Add(addGroupVerb);
				coll.Add(removeGroupVerb);
				coll.Add(addItemVerb);
				coll.Add(addLargeItemVerb);
				return coll;
			}
		}
		protected override bool UseVerbsAsActionList {
			get {
				return true;
			}
		}
		TileControlDesignTimeManager helper;
		public TileControlDesignTimeManager Helper {
			get {
				if(helper == null) {
					helper = TileControlDesignTimeManagerFactory.CreateManager(TileGroup, TileGroup.Control);
				}
				return helper;
			}
		}
		protected virtual void OnAddTileGroupClick(object sender, EventArgs e) {
			Helper.OnAddTileGroupClick();
		}
		protected virtual void OnRemoveTileGroupClick(object sender, EventArgs e) {
			Helper.OnRemoveTileGroupClick(TileGroup);
		}
		protected virtual void OnAddTileItemClick(object sender, EventArgs e) {
			Helper.OnAddTileItemClick();
		}
		protected virtual void OnAddLargeTileItemClick(object sender, EventArgs e) {
			Helper.OnAddLargeTileItemClick();
		}
	}
	public class TileControlKeyCommandProcessHelper : DesignTimeKeyCommandProcessHelperBase {
		TileControlDesigner designer;
		public TileControlKeyCommandProcessHelper(TileControlDesigner tileControlDesigner)
			: base(tileControlDesigner) {
			designer = tileControlDesigner;
		}
		protected TileControlViewInfo ViewInfoCore { get { return ((ITileControl)designer.TileControl).ViewInfo; } }
		public override void OnKeyCancel(object sender, EventArgs e) {
			TileGroup group = ViewInfoCore.DesignTimeManager.GetGroup() as TileGroup;
			TileItem item = ViewInfoCore.DesignTimeManager.GetItem() as TileItem;
			if(item != null) {
				ViewInfoCore.DesignTimeManager.SelectComponent(item.Group);
				designer.Control.Invalidate();
			}
			else if(group != null) {
				ViewInfoCore.DesignTimeManager.SelectComponent(group.Control);
			}
			else PassControlToOldKeyCancelHandler();
		}
	}
	public class TileBarKeyCommandProcessHelper : DesignTimeKeyCommandProcessHelperBase {
		TileBarDesigner designer;
		public TileBarKeyCommandProcessHelper(TileBarDesigner tileBarDesigner)
			: base(tileBarDesigner) {
			designer = tileBarDesigner;
		}
		protected TileControlViewInfo ViewInfoCore { get { return ((ITileControl)designer.TileBarCore).ViewInfo; } }
		public override void OnKeyCancel(object sender, EventArgs e) {
			TileGroup group = ViewInfoCore.DesignTimeManager.GetGroup() as TileGroup;
			TileItem item = ViewInfoCore.DesignTimeManager.GetItem() as TileItem;
			if(item != null) {
				ViewInfoCore.DesignTimeManager.SelectComponent(item.Group);
				designer.Control.Invalidate();
			}
			else if(group != null) {
				ViewInfoCore.DesignTimeManager.SelectComponent(group.Control);
			}
			else PassControlToOldKeyCancelHandler();
		}
	}
	public class TileDesignerActionList : DesignerActionList {
		IDesigner designer;
		public TileDesignerActionList(IDesigner designer)
			: base(designer.Component) {
			this.designer = designer;
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection res = new DesignerActionItemCollection();
			res.Add(new DesignerActionHeaderItem("Appearance", "Appearance"));
			res.Add(new DesignerActionPropertyItem("BackgroundImage", "Background Image", "Appearance"));
			res.Add(new DesignerActionPropertyItem("BackgroundImageLayout", "Background Image Layout", "Appearance"));
			res.Add(new DesignerActionPropertyItem("ShowText", "Show Text", "Appearance"));
			res.Add(new DesignerActionPropertyItem("ShowGroupText", "Show Group Text", "Appearance"));
			res.Add(new DesignerActionHeaderItem("Behavior", "Behavior"));
			res.Add(new DesignerActionPropertyItem("AllowSelectedItem", "Allow Selected Item", "Behavior"));
			res.Add(new DesignerActionPropertyItem("AllowItemHover", "Allow Item Hover", "Behavior"));
			res.Add(new DesignerActionPropertyItem("SelectionColor", "Selection Color", "Behavior"));
			res.Add(new DesignerActionPropertyItem("ItemTextShowMode", "Item Text Show Mode", "Behavior"));
			res.Add(new DesignerActionPropertyItem("ItemCheckMode", "Item Check Mode", "Behavior"));
			res.Add(new DesignerActionPropertyItem("ItemContentAnimation", "Item Content Animation", "Behavior"));
			res.Add(new DesignerActionHeaderItem("Actions", "Actions"));
			res.Add(new DesignerActionMethodItem(this, "RunDesigner", "Run Designer", "Actions"));
			res.Add(new DesignerActionMethodItem(this, "AddGroup", "Add Group", "Actions"));
			return res;
		}
		[Editor(typeof(DXImageEditor), typeof(UITypeEditor))]
		public virtual Image BackgroundImage {
			get { return TileControl.BackgroundImage; }
			set { EditorContextHelper.SetPropertyValue(Designer, Component, "BackgroundImage", value); }
		}
		public virtual ImageLayout BackgroundImageLayout {
			get { return TileControl.BackgroundImageLayout; }
			set { EditorContextHelper.SetPropertyValue(Designer, Component, "BackgroundImageLayout", value); }
		}
		public virtual void RunDesigner() {
			Helper.ShowDesignerForm(TileControl);
		}
		public virtual void AddGroup() {
			Helper.OnAddTileGroupClick();
		}
		public virtual bool ShowText {
			get { return TileControl.ShowText; }
			set { EditorContextHelper.SetPropertyValue(Designer, Component, "ShowText", value); }
		}
		public virtual bool ShowGroupText {
			get { return TileControl.ShowGroupText; }
			set { EditorContextHelper.SetPropertyValue(Designer, Component, "ShowGroupText", value); }
		}
		public virtual TileItemContentAnimationType ItemContentAnimation {
			get { return TileControl.ItemContentAnimation; }
			set { EditorContextHelper.SetPropertyValue(Designer, Component, "ItemContentAnimation", value); }
		}
		public virtual TileItemCheckMode ItemCheckMode {
			get { return TileControl.ItemCheckMode; }
			set { EditorContextHelper.SetPropertyValue(Designer, Component, "ItemCheckMode", value); }
		}
		public virtual bool AllowSelectedItem {
			get { return TileControl.AllowSelectedItem; }
			set { EditorContextHelper.SetPropertyValue(Designer, Component, "AllowSelectedItem", value); }
		}
		public virtual bool AllowItemHover {
			get { return TileControl.AllowItemHover; }
			set { EditorContextHelper.SetPropertyValue(Designer, Component, "AllowItemHover", value); }
		}
		public virtual Color SelectionColor {
			get { return TileControl.SelectionColor; }
			set { EditorContextHelper.SetPropertyValue(Designer, Component, "SelectionColor", value); }
		}
		public virtual TileItemContentShowMode ItemTextShowMode {
			get { return TileControl.ItemTextShowMode; }
			set { EditorContextHelper.SetPropertyValue(Designer, Component, "ItemTextShowMode", value); }
		}
		TileControlDesignTimeManager helper;
		public TileControlDesignTimeManager Helper {
			get {
				if(helper == null)
					helper = TileControlDesignTimeManagerFactory.CreateManager(TileControl, TileControl);
				return helper;
			}
		}
		public IDesigner Designer { get { return designer; } }
		public TileControl TileControl { get { return Component as TileControl; } }
	}
	public class TileEditorForm : BaseDesignerForm {
		public const string RibbonSettings = "Software\\Developer Express\\Designer\\XtraBars\\Ribbon\\";
		object component = null;
		public TileEditorForm(object component)
			: this() {
			this.component = component;
		}
		public TileEditorForm() {
			InitializeComponent();
			ProductInfo = new ProductInfo("TileControl", typeof(TileControl), ProductKind.DXperienceWin, ProductInfoStage.Registered);
		}
		protected override void CreateDesigner() {
			ActiveDesigner = new TileControlEditorFormActiveDesigner(EditingComponent);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.SuspendLayout();
			this.ClientSize = new System.Drawing.Size(1000, 600);
			this.LookAndFeel.UseDefaultLookAndFeel = false;
			this.Name = "TileEditorForm";
			this.Text = "Tile Designer";
			this.ResumeLayout(false);
		}
		#endregion
		protected override string RegistryStorePath { get { return RibbonSettings; } }
		protected override Type ResolveType(string type) {
			Type t = typeof(TileEditorForm).Assembly.GetType(type);
			if(t != null) return t;
			return base.ResolveType(type);
		}
		public object ComponentObj { get { return component; } set { component = value; } }
		public virtual bool IsGallery() {
			if(ComponentObj is RibbonGalleryBarItem || ComponentObj is RibbonGalleryBarItemLink || ComponentObj is GalleryDropDown || ComponentObj is GalleryControl) return true;
			return false;
		}
		public virtual bool IsPopupMenu() {
			if(ComponentObj is PopupMenu) return true;
			return false;
		}
		public override void InitEditingObject(object editingObject) {
			base.InitEditingObject(editingObject);
			if(IsGallery()) UpdateActiveDesigner("Gallery");
			else if(IsPopupMenu()) UpdateActiveDesigner("Sub Menus & Popup Menus");
			else if(IsMiniToolbar()) UpdateActiveDesigner("Ribbon MiniToolbar Items");
		}
		bool IsMiniToolbar() {
			return ComponentObj is RibbonMiniToolbar;
		}
	}
	public class TileControlEditorFormActiveDesigner : BaseDesigner {
		static ImageCollection largeImages;
		static ImageCollection smallImages;
		object component;
		static TileControlEditorFormActiveDesigner() {
			largeImages = ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraBars.Design.Ribbon.Images.icons.png", typeof(TileControlEditorFormActiveDesigner).Assembly, new Size(32, 32));
			smallImages = ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraBars.Design.Ribbon.Images.icons-small.png", typeof(TileControlEditorFormActiveDesigner).Assembly, new Size(16, 16));
		}
		public TileControlEditorFormActiveDesigner() { }
		public TileControlEditorFormActiveDesigner(object component) {
			this.component = component;
		}
		protected override void CreateGroups() {
			Groups.Clear();
			if(TileControl != null) {
				DesignerGroup tileGroup = Groups.Add("TileControls", "Image tile controls.", null, true);
				tileGroup.Add("TileControl", "Customize controls, groups and items.", typeof(TileControlManagerPGFrame), GetDefaultLargeImage(6), GetDefaultSmallImage(11), null);
			}
		}
		protected override object LargeImageList { get { return largeImages; } }
		protected override object SmallImageList { get { return smallImages; } }
		public TileControl TileControl { get { return component as TileControl; } }
	}
	public class TileControlDesigner : BaseControlDesigner, ITileControlDesigner, IKeyCommandProcessInfo {
		TileControlKeyCommandProcessHelper keyCommandProcessHelper;
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			((ITileControl)TileControl).ViewInfo.DesignTimeManager = new TileControlDesignTimeManager(Component, TileControl);
			keyCommandProcessHelper = new TileControlKeyCommandProcessHelper(this);
			SubscribeEvent();
		}
		void SubscribeEvent() {
			(TileControl as ITileControlUpdateSmartTag).SmartTagUpdate += SmartTagUpdate;
		}
		void SmartTagUpdate(object sender, TileControlSmartTagEventArgs e) {
			BehaviorService.SyncSelection();
		}
		void UnsubscribeEvent() {
			(TileControl as ITileControlUpdateSmartTag).SmartTagUpdate -= SmartTagUpdate;
		}
		protected virtual void CreateTileDesignerActionLists(DesignerActionListCollection list) {
			list.Add(new TileDesignerActionList(this));
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			CreateTileDesignerActionLists(list);
			base.RegisterActionLists(list);
		}
		public new TileControl Component {
			get {
				return (TileControl)base.Component;
			}
		}
		public TileControl TileControl { get { return Component as TileControl; } }
		public override ICollection AssociatedComponents {
			get {
				ArrayList list = new ArrayList();
				if(TileControl.Images != null) list.Add(TileControl.Images);
				AddGroups(list);
				AddBase(list);
				return list;
			}
		}
		protected TileControlViewInfo ViewInfoCore { get { return ((ITileControl)TileControl).ViewInfo; } }
		protected override void Dispose(bool disposing) {
			UnsubscribeEvent();
			if(disposing) {
				keyCommandProcessHelper.Dispose();
			}
			base.Dispose(disposing);
		}
		protected void AddGroups(ArrayList list) {
			foreach(TileGroup group in TileControl.Groups) {
				list.Add(group);
			}
		}
		protected void AddBase(ArrayList controls) {
			foreach(object obj in base.AssociatedComponents) {
				if(controls.Contains(obj)) continue;
				controls.Add(obj);
			}
		}
		protected override bool GetHitTest(Point point) {
			ITileControl tileControl = (ITileControl)TileControl;
			if(tileControl.Handler.State == TileControlHandlerState.DragMode)
				return true;
			Point pt = TileControl.PointToClient(point);
			TileControlHitInfo hitInfo = tileControl.ViewInfo.CalcHitInfo(pt);
			if(hitInfo.InItem || hitInfo.InGroup || hitInfo.InBackArrow || hitInfo.InForwardArrow || tileControl.ScrollBar != null && tileControl.ScrollBar.Bounds.Contains(pt))
				return true;
			return base.GetHitTest(point);
		}
		public override DesignerVerbCollection DXVerbs {
			get {
				DesignerVerbCollection coll = base.DXVerbs;
				DesignerVerb addGroupVerb = new DesignerVerb("Add Group", new EventHandler(OnAddTileGroupClick));
				DesignerVerb designerVerb = new DesignerVerb("Run Designer", new EventHandler(OnRunDesignerClick));
				coll.Add(addGroupVerb);
				coll.Add(designerVerb);
				return coll;
			}
		}
		protected virtual void OnAddTileGroupClick(object sender, EventArgs e) {
			Helper.OnAddTileGroupClick();
		}
		protected virtual void OnRunDesignerClick(object sender, EventArgs e) {
			Helper.ShowDesignerForm(TileControl);
		}
		TileControlDesignTimeManager helper;
		public TileControlDesignTimeManager Helper {
			get {
				if(helper == null)
					helper = TileControlDesignTimeManagerFactory.CreateManager(TileControl, TileControl);
				return helper;
			}
		}
		protected override bool CanUseComponentSmartTags {
			get { return true; }
		}
		#region ITileControlDesigner Members
		bool ITileControlDesigner.DebuggingState {
			get { return DebuggingState; }
		}
		#endregion
		#region IKeyCommandProcessInfo
		IServiceProvider IKeyCommandProcessInfo.ServiceProvider {
			get { return Component.Site; }
		}
		BaseDesignTimeManager IKeyCommandProcessInfo.DesignTimeManager {
			get { return ((ITileControl)TileControl).ViewInfo.DesignTimeManager.GetBase(); }
		}
		#endregion
	}
	#region TileControl Design Time Manager
	public class TileControlDesignTimeManagerFactory {
		public static TileControlDesignTimeManager CreateManager(IComponent component, ITileControl tileControl) {
			return new TileControlDesignTimeManager(component, tileControl);
		}
	}
	public class TileControlDesignTimeManager : TileControlDesignTimeManagerBase {
		public TileControlDesignTimeManager(IComponent component, ITileControl tileControl)
			: base(component, tileControl) {
		}
		public virtual void ShowDesignerForm(TileControl tileControl) {
			if(tileControl == null) return;
			IUIService srv = Component.Site.GetService(typeof(IUIService)) as IUIService;
			using(TileEditorForm form = new TileEditorForm()) {
				form.InitEditingObject(tileControl);
				form.ShowDialog(srv == null ? null : srv.GetDialogOwnerWindow());
			}
		}
		protected override void FillDesignTimePopupMenu(DXPopupMenu popupMenu, bool isGroupMenu, TileControlHitInfo hitInfo) {
			base.FillDesignTimePopupMenu(popupMenu, isGroupMenu, hitInfo);
			if(isGroupMenu) return;
			DXMenuItem editFrameItem = new DXMenuItem() { Caption = "Edit Animation Frames", Tag = hitInfo.ItemInfo, BeginGroup = true };
			editFrameItem.Click += OnEditFramesInfoCollectionClick;
			editFrameItem.Enabled = !TileControl.DebuggingState;
			DXMenuItem editElementsItem = new DXMenuItem() { Caption = "Edit TileItem Elements", Tag = hitInfo.ItemInfo };
			editElementsItem.Click += OnEditTileItemElementsCollectionClick;
			editElementsItem.Enabled = !TileControl.DebuggingState;
			DXMenuItem editTemplateItem = new DXMenuItem() { Caption = "Select TileItem Template", Tag = hitInfo.ItemInfo };
			editTemplateItem.Click += OnEditTileItemTemplateClick;
			editTemplateItem.Enabled = !TileControl.DebuggingState;
			popupMenu.Items.Add(editFrameItem);
			popupMenu.Items.Add(editElementsItem);
			popupMenu.Items.Add(editTemplateItem);
		}
		protected internal virtual void OnEditFramesInfoCollectionClick(object sender, EventArgs e) {
			OnEditFramesCollectionClickCore(GetItemFromSender(sender));
		}
		public override void OnEditFramesCollectionClickCore(TileItem item) {
			TileItemTypeDescriptorContext context = new TileItemTypeDescriptorContext(this, TileItemTypeDescriptorContext.PropertyDescriptorType.Frames, item);
			UITypeEditor editor = TypeDescriptor.GetEditor(item.Frames, typeof(UITypeEditor)) as UITypeEditor;
			if(editor != null)
				editor.EditValue(context, context, item.Frames);
		}
		protected internal virtual void OnEditTileItemElementsCollectionClick(object sender, EventArgs e) {
			OnEditElementsCollectionClickCore(GetItemFromSender(sender));
		}
		public override void OnEditElementsCollectionClickCore(TileItem item) {
			TileItemTypeDescriptorContext context = new TileItemTypeDescriptorContext(this, TileItemTypeDescriptorContext.PropertyDescriptorType.Elements, item);
			UITypeEditor editor = TypeDescriptor.GetEditor(item.Elements, typeof(UITypeEditor)) as UITypeEditor;
			if(editor != null)
				editor.EditValue(context, context, item.Elements);
		}
		class TileItemTypeDescriptorContext : ITypeDescriptorContext {
			public enum PropertyDescriptorType { Elements, Frames }
			TileControlDesignTimeManager designTimeManager;
			TileItem instance;
			PropertyDescriptorType propertyDescriptorType;
			public TileItemTypeDescriptorContext(TileControlDesignTimeManager designTimeManager, PropertyDescriptorType propertyDescriptorType, TileItem instance) {
				this.designTimeManager = designTimeManager;
				this.instance = instance;
				this.propertyDescriptorType = propertyDescriptorType;
			}
			public IContainer Container {
				get { return designTimeManager.DesignerHost.Container; }
			}
			public object Instance {
				get { return this.instance; }
			}
			public void OnComponentChanged() {
				designTimeManager.ComponentChangeService.OnComponentChanged(this.instance, null, null, null);
			}
			public bool OnComponentChanging() {
				designTimeManager.ComponentChangeService.OnComponentChanging(this.instance, null);
				return true;
			}
			public PropertyDescriptor PropertyDescriptor {
				get {
					PropertyDescriptorCollection propertyCollection = TypeDescriptor.GetProperties(instance);
					return propertyCollection.Find(propertyDescriptorType.ToString(), true);
				}
			}
			public object GetService(Type serviceType) {
				object service = this.designTimeManager.GetService(serviceType);
				if(service != null) return service;
				service = this.designTimeManager.DesignerHost.GetService(serviceType);
				if(service != null) return service;
				if(instance.Site != null)
					service = instance.Site.GetService(serviceType);
				if(service != null) return service;
				if(serviceType != typeof(IWindowsFormsEditorService)) return null;
				return new TileItemElementsWindowsFormsEditorService();
			}
		}
		public class TileItemElementsWindowsFormsEditorService : IWindowsFormsEditorService {
			public void CloseDropDown() { }
			public void DropDownControl(Control control) { }
			public DialogResult ShowDialog(Form dialog) {
				return dialog.ShowDialog();
			}
		}
		protected internal virtual void OnEditTileItemTemplateClick(object sender, EventArgs e) {
			OnEditTileTemplateClickCore(GetItemFromSender(sender));
		}
		public override void OnEditTileTemplateClickCore(TileItem item) {
			using(TileTemplateSelectorForm form = new TileTemplateSelectorForm((ITileItem)item)) {
				ComponentChangeService.OnComponentChanging(Component, null);
				if(form.ShowDialog() == DialogResult.OK)
					ComponentChangeService.OnComponentChanged(Component, null, null, null);
			}
		}
		TileItem GetItemFromSender(object sender) {
			TileItem item;
			if(sender is TileItem)
				item = sender as TileItem;
			else
				item = ((TileItemViewInfo)((DXMenuItem)sender).Tag).Item;
			return item;
		}
	}
	#endregion
	public class TileItemFrameEditor : DXCollectionEditorBase {
		public TileItemFrameEditor(Type type) : base(type) { }
		ITypeDescriptorContext context;
		TileFrameEditPreviewControl previewControl;
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			this.context = context;
			object editResult = value;
			TileItemFrame currentFrame = null;
			ITileItem item = TileItemElementsEditor.GetTileItemCore(context, provider, ref currentFrame);
			using(previewControl = new TileFrameEditPreviewControl(item)) {
				editResult = base.EditValue(context, provider, value);
			}
			return editResult;
		}
		protected override object CreateCustomInstance(Type itemType) {
			TileItemFrame frame = new TileItemFrame();
			frame.UseText = true;
			frame.UseImage = true;
			frame.UseBackgroundImage = true;
			frame.AnimateText = true;
			frame.AnimateImage = true;
			frame.AnimateBackgroundImage = true;
			if(this.context != null) {
				TileItem tileItem = this.context.Instance as TileItem;
				if(tileItem != null) {
					frame.Elements.Assign(tileItem.Elements);
					PropertyInfo ownerPropertyDescriptor = frame.GetType().GetProperty("Owner", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
					if(ownerPropertyDescriptor != null)
						ownerPropertyDescriptor.SetValue(frame, tileItem, null);
				}
			}
			return frame;
		}
		protected override DXCollectionEditorBase.UISettings GetCollectionEditorUISettings() {
			return new UISettings()
			{
				PreviewControl = previewControl,
				AllowReordering = true,
				AllowSearch = false,
				ShowPreviewControlBorder = false,
				ColumnHeaders = new ColumnHeader[] { 
					new ColumnHeader() { Caption = "Name", FieldName = "Text" } }
			};
		}
		class TileFrameEditPreviewControl : IDXCollectionEditorPreviewControl, IXtraResizableControl, IDisposable {
			XtraTabControl tabControl;
			TileControl frameViewControl;
			TileControl previewViewControl;
			ITileItem SourceItem { get; set; }
			TileItem PreviewItem { get; set; }
			TileItemFrame SelectedFrame { get; set; }
			IDesignerHost DesignerHost { get; set; }
			List<TileItemFrame> Frames = new List<TileItemFrame>();
			public Control Control {
				get { return this.tabControl; }
			}
			public TileFrameEditPreviewControl(ITileItem srcItem) {
				this.SourceItem = srcItem;
				this.tabControl = InitTabControl();
				this.frameViewControl = InitFrameView(srcItem.Control);
				this.previewViewControl = InitPreview(srcItem.Control);
				this.tabControl.TabPages[0].Controls.Add(frameViewControl);
				this.tabControl.TabPages[1].Controls.Add(previewViewControl);
				if(srcItem.Control != null && srcItem.Control.Container != null)
					this.DesignerHost = srcItem.Control.Container as IDesignerHost;
				SubscribeOnDesignerHost();
			}
			void SubscribeOnDesignerHost() {
				if(this.DesignerHost != null) this.DesignerHost.TransactionClosed += DesignerHost_TransactionClosed;
			}
			void UnsubscribeOnDesignerHost() {
				if(this.DesignerHost != null) this.DesignerHost.TransactionClosed -= DesignerHost_TransactionClosed;
			}
			void DesignerHost_TransactionClosed(object sender, DesignerTransactionCloseEventArgs e) {
				if(e.TransactionCommitted) OnChanged();
			}
			XtraTabControl InitTabControl() {
				var result = new XtraTabControl();
				result.TabPages.Add("Frame");
				result.TabPages.Add("Preview");
				return result;
			}
			TileControl InitTileControl(ITileControl src) {
				TileControl result = new TileControl();
				ITileControl iTileControl = result as ITileControl;
				iTileControl.BeginUpdate();
				if(src != null) {
					iTileControl.Properties.ItemSize = src.Properties.ItemSize;
					iTileControl.AppearanceItem.Assign(src.AppearanceItem);
					iTileControl.BackgroundImage = src.BackgroundImage;
					iTileControl.Properties.BackgroundImageLayout = src.Properties.BackgroundImageLayout;
					iTileControl.Control.BackColor = src.Control.BackColor;
					iTileControl.Properties.LargeItemWidth = src.Properties.LargeItemWidth;
					iTileControl.Properties.IndentBetweenItems = src.Properties.IndentBetweenItems;
					iTileControl.Images = src.Images;
				}
				iTileControl.Properties.VerticalContentAlignment = VertAlignment.Center;
				iTileControl.Properties.HorizontalContentAlignment = HorzAlignment.Center;
				iTileControl.Properties.AllowDrag = false;
				iTileControl.Control.Dock = DockStyle.Fill;
				result.AnimateArrival = false;
				iTileControl.EndUpdate();
				return result;
			}
			TileControl InitPreview(ITileControl srcControl) {
				TileItem item = CreateSourceItemEmptyClone();
				TileControl result = InitTileControl(srcControl);
				result.Groups.Clear();
				result.Groups.Add(new TileGroup());
				result.Groups[0].Items.Add(item);
				this.PreviewItem = item;
				return result;
			}
			TileControl InitFrameView(ITileControl srcControl) {
				TileControl result = InitTileControl(srcControl);
				result.Groups.Clear();
				result.Groups.Add(new TileGroup());
				return result;
			}
			void UpdatePreview() {
				previewViewControl.BeginUpdate();
				{
					PreviewItem.StopAnimation();
					PreviewItem.Elements.Clear();
					PreviewItem.Frames.Clear();
					foreach(var frame in this.Frames)
						PreviewItem.Frames.Add(frame);
				}
				previewViewControl.EndUpdate();
				PreviewItem.StartContentAnimation();
			}
			void UpdateFrameView(TileItemFrame frame) {
				this.frameViewControl.Groups[0].Items.Clear();
				if(frame != null)
					this.frameViewControl.Groups[0].Items.Add(CreateItemForFrame(frame));
			}
			TileItem CreateSourceItemEmptyClone() {
				var newItem = Activator.CreateInstance(this.SourceItem.GetType()) as TileItem;
				if(newItem == null) newItem = new TileItem();
				AssignItem(newItem, SourceItem);
				return newItem;
			}
			ITileItem CreateItemForFrame(TileItemFrame frame) {
				var newItem = CreateSourceItemEmptyClone();
				AssignAppearanceFromFrame(newItem, frame);
				newItem.SetContent(frame, false);
				return newItem;
			}
			void AssignAppearanceFromFrame(ITileItem item, TileItemFrame frame) {
				if(frame.Appearance != AppearanceObject.EmptyAppearance) {
					item.Appearances.Normal.Assign(frame.Appearance);
					item.Appearances.Pressed.Assign(frame.Appearance);
					item.Appearances.Hovered.Assign(frame.Appearance);
					item.Appearances.Selected.Assign(frame.Appearance);
				}
			}
			public void AssignItem(ITileItem dst, ITileItem src) {
				dst.Properties.Assign(src.Properties);
				if(src is DevExpress.XtraBars.Docking2010.Views.WindowsUI.BaseTile && src.Properties.ItemSize == TileItemSize.Default)
					dst.Properties.ItemSize = TileItemSize.Wide;
				dst.Padding = src.Padding;
				dst.BackgroundImage = src.BackgroundImage;
				dst.Appearances.Assign(src.Appearances);
			}
			void OnChanged() {
				UpdateFrameView(this.SelectedFrame);
				UpdatePreview();
			}
			public void OnSelectedItemChanged(DevExpress.Utils.Design.Internal.SelectedItemChangedEventArgs args) {
				this.SelectedFrame = args.SelectedItem as TileItemFrame;
				UpdateFrameView(this.SelectedFrame);
			}
			public void OnItemChanged(DevExpress.Utils.Design.Internal.PropertyItemChangedEventArgs args) { }
			public void OnCollectionChanged(DevExpress.Utils.Design.Internal.CollectionChangedEventArgs args) {
				TileItemFrame frame = args.Item as TileItemFrame;
				if(frame == null) return;
				switch(args.Action) {
					case DevExpress.Utils.Design.Internal.CollectionAction.Add:
						if(!this.Frames.Contains(frame)) this.Frames.Add(frame);
						break;
					case DevExpress.Utils.Design.Internal.CollectionAction.Remove:
						if(this.Frames.Contains(frame)) this.Frames.Remove(frame);
						break;
					case DevExpress.Utils.Design.Internal.CollectionAction.Reorder:
						var targetFrame = args.TargetItem as TileItemFrame;
						if(targetFrame != null) {
							int targetIndex = Frames.IndexOf(targetFrame);
							Frames.Remove(frame);
							Frames.Insert(targetIndex, frame);
						}
						break;
				}
				UpdatePreview();
			}
			public void OnCollectionChanging(DevExpress.Utils.Design.Internal.CollectionChangingEventArgs args) { }
			public event EventHandler Changed { add { } remove { } }
			public bool IsCaptionVisible {
				get { return true; }
			}
			public Size MaxSize {
				get { return new Size(0, 0); }
			}
			public Size MinSize {
				get { return new Size(this.tabControl.Width, this.tabControl.Height); }
			}
			public void Dispose() {
				UnsubscribeOnDesignerHost();
				if(this.tabControl != null)
					this.tabControl.Dispose();
			}
		}
	}
	public class TileItemElementsSelectorEditor : UITypeEditor {
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.DropDown;
		}
		TileItemElement GetElement(ITypeDescriptorContext context) {
			object element = null;
			if(context != null) {
				element = DXObjectWrapper.GetInstance(context);
				return element as TileItemElement;
			}
			return null;
		}
		IWindowsFormsEditorService editorService;
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
			ListBox lb = new ListBox();
			lb.SelectedIndexChanged += lb_SelectedIndexChanged;
			lb.SelectionMode = SelectionMode.One;
			TileItemElement element = GetElement(context);
			if(element == null)
				return null;
			var item = element.Collection.Owner as TileItem;
			if(item == null) return value;
			lb.Items.Clear();
			lb.Items.Add(new ElementWrapper(null));
			foreach(TileItemElement el in item.Elements) {
				if(el != element) {
					int index = lb.Items.Add(new ElementWrapper(el));
					if(el.Equals(value)) lb.SelectedIndex = index;
				}
			}
			editorService.DropDownControl(lb);
			var ew = lb.SelectedItem as ElementWrapper;
			if(ew == null) return null;
			return ew.Element;
		}
		void lb_SelectedIndexChanged(object sender, EventArgs e) {
			if(editorService == null) return;
			editorService.CloseDropDown();
		}
		class ElementWrapper {
			public ElementWrapper(TileItemElement element) {
				this.Element = element;
			}
			public TileItemElement Element { get; private set; }
			public override string ToString() {
				if(Element == null) return "<None>";
				if(!String.IsNullOrEmpty(Element.Text))
					return Element.Text;
				if(Element.Image != null)
					return "<image>";
				return "<element>";
			}
		}
	}
	public class TileItemElementsEditor : DXCollectionEditorBase {
		public TileItemElementsEditor(Type type) : base(type) { }
		ElementsEditPreviewControl previewControl;
		DXCollectionEditorBase.DXCollectionEditorBaseForm editorForm;
		protected override bool AllowLiveUpdates {
			get { return true; }
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			TileItemFrame currentEditFrame = null;
			ITileItem tileItem = GetTileItemCore(context, provider, ref currentEditFrame);
			if(tileItem != null) {
				int frameIndex = tileItem.CurrentFrameIndex;
				object editResult = null;
				using(previewControl = CreatePreviewControl(tileItem)) {
					FillTileItemElementNames(value as TileItemElementCollection);
					editResult = base.EditValue(context, provider, value);
					if(this.editorForm != null && this.editorForm.DialogResult == DialogResult.OK) {
						ReassignCollection(tileItem);
						if(currentEditFrame == null && tileItem.Frames.Count > 0)
							tileItem.Frames[frameIndex].Elements.Assign(tileItem.Elements);
						SerializeCollection(tileItem);
					}
				}
				return editResult;
			}
			return base.EditValue(context, provider, value);
		}
		void ReassignCollection(ITileItem tileItem) {
			ITileItem copy = new TileItem();
			copy.Elements.Assign(tileItem.Elements);
			tileItem.Elements.Assign(copy.Elements);
		}
		bool IsFrameEdit(ITileItem collectionInstance, TileItemFrame currentEditFrame) {
			return currentEditFrame != null || collectionInstance.Frames.Count > 0;
		}
		public static ITileItem GetTileItemCore(ITypeDescriptorContext context, IServiceProvider provider, ref TileItemFrame currentEditFrame) {
			object instance = DXObjectWrapper.GetInstance(context);
			ITileItem item = instance as ITileItem;
			if(item != null) return item;
			TileControlObjectDescriptor od = instance as TileControlObjectDescriptor;
			if(od != null && od.Item is ITileItem) return od.Item as ITileItem;
			TileItemFrame frame = instance as TileItemFrame;
			if(frame == null) return null;
			currentEditFrame = frame;
			object owner = null;
			PropertyInfo ownerPropertyDescriptor = frame.GetType().GetProperty("Owner", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
			if(ownerPropertyDescriptor != null)
				owner = ownerPropertyDescriptor.GetValue(frame, null);
			return owner as TileItem;
		}
		protected override DXCollectionEditorBase.DXCollectionEditorBaseForm CreateCollectionForm() {
			editorForm = new TileItemElementEditorForm(this);
			return editorForm;
		}
		protected override object CreateCustomInstance(Type itemType) {
			TileItemElement element = new TileItemElement();
			if(element != null)
				element.Text = GetTileItemElementText();
			return element;
		}
		protected override DXCollectionEditorBase.UISettings GetCollectionEditorUISettings() {
			return new UISettings()
			{
				PreviewControl = previewControl,
				AllowReordering = true,
				AllowSearch = false,
				ColumnHeaders = new ColumnHeader[] { 
					new ColumnHeader() { Caption = "Name", FieldName = "Text" } }
			};
		}
		protected override void OnCollectionChanged(DevExpress.Utils.Design.Internal.CollectionChangedEventArgs e) {
			TileItemElement element = e.Item as TileItemElement;
			if(element == null) return;
			switch(e.Action) {
				case DevExpress.Utils.Design.Internal.CollectionAction.Add:
					if(!tileItemElementNames.Contains(element.Text))
						tileItemElementNames.Add(element.Text);
					break;
				case DevExpress.Utils.Design.Internal.CollectionAction.Remove:
					if(tileItemElementNames.Contains(element.Text))
						tileItemElementNames.Remove(element.Text);
					break;
			}
		}
		protected override string GetDisplayText(object value) {
			TileItemElement element = value as TileItemElement;
			if(element != null) {
				if(string.IsNullOrEmpty(element.Text))
					return element.Image == null ? "<empty>" : "<image>";
				return element.Text;
			}
			return base.GetDisplayText(value);
		}
		void FillTileItemElementNames(TileItemElementCollection tileItemElementCollection) {
			tileItemElementNames.Clear();
			if(tileItemElementCollection == null) return;
			foreach(var element in tileItemElementCollection) {
				tileItemElementNames.Add((element as TileItemElement).Text);
			}
		}
		ElementsEditPreviewControl CreatePreviewControl(ITileItem source) {
			return new ElementsEditPreviewControl(CreatePreviewContainer(source));
		}
		TileControl CreatePreviewContainer(ITileItem instance) {
			TileControl tileItemContainer = null;
			if(instance != null) {
				tileItemContainer = InitPreviewContainer(instance);
				previewControl = new ElementsEditPreviewControl(tileItemContainer);
			}
			return tileItemContainer;
		}
		public static TileControl CreatePreviewTileControl(ITileControl sourceControl) {
			TileControl res = null;
			try {
				if(sourceControl != null) res = Activator.CreateInstance(sourceControl.GetType()) as TileControl;
			}
			catch {
				res = null;
			}
			finally {
				if(res == null) res = new TileControl();
			}
			return res;
		}
		public TileControl InitPreviewContainer(ITileItem sourceItem) {
			ITileControl sourceControl = sourceItem.Control;
			TileControl previewContainer = CreatePreviewTileControl(sourceControl);
			this.InitHelper = TileItemInitHelper.Create(sourceItem.Tag is TileItemFrame);
			ITileItem itemCore = CreatePreviewTileItem(sourceItem);
			if(sourceItem != null) {
				previewContainer.BeginUpdate();
				try {
					if(previewContainer.Groups.Count == 0)
						previewContainer.Groups.Add(new TileGroup());
					previewContainer.Groups[0].Items.Clear();
					previewContainer.Groups[0].Items.Add(itemCore);
					previewContainer.AllowDrag = false;
					previewContainer.HorizontalContentAlignment = HorzAlignment.Center;
					previewContainer.VerticalContentAlignment = VertAlignment.Center;
					if(sourceControl != null) {
						previewContainer.ItemSize = sourceControl.Properties.ItemSize;
						previewContainer.IndentBetweenItems = sourceControl.Properties.IndentBetweenItems;
						previewContainer.ItemPadding = sourceControl.Properties.ItemPadding;
						previewContainer.AppearanceItem.Assign(sourceControl.AppearanceItem);
						(previewContainer as ITileControl).Properties.LargeItemWidth =
							(sourceControl as ITileControl).Properties.LargeItemWidth;
					}
				}
				finally {
					previewContainer.EndUpdate();
					previewContainer.Groups[0].Items[0].StartContentAnimation();
				}
			}
			return previewContainer;
		}
		List<string> tileItemElementNames = new List<string>();
		string GetTileItemElementText() {
			string prefix = "element";
			for(int i = 1; true; i++) {
				string res = string.Concat(prefix, i.ToString());
				if(!IsTileItemTextExist(res)) return res;
			}
		}
		bool IsTileItemTextExist(string text) {
			foreach(var item in tileItemElementNames) {
				if(string.Equals(item, text))
					return true;
			}
			return false;
		}
		void SerializeCollection(ITileItem item) {
			if(item.Control != null && item.Control.Site != null)
				item.Control.ViewInfo.DesignTimeManager.ComponentChanged(item as TileItem);
		}
		TileItem CreatePreviewTileItem(ITileItem source) {
			var res = Activator.CreateInstance(source.GetType()) as TileItem;
			if(res == null) res = new TileItem();
			try {
				InitHelper.BeginInit(source);
				InitHelper.Init(res, source);
			}
			finally {
				InitHelper.EndInit(source);
			}
			return res;
		}
		internal void ClearPreview() {
			if(previewControl != null)
				previewControl.ClearPreview();
		}
		TileItemElementsEditor.TileItemInitHelper InitHelper { get; set; }
		class TileItemInitHelper {
			public static TileItemInitHelper Create(bool frameItem) {
				if(frameItem)
					return new FrameTileItemInitHelper();
				return new TileItemInitHelper();
			}
			public virtual void Init(ITileItem res, ITileItem source) {
				res.Padding = source.Padding;
				res.BackgroundImage = source.BackgroundImage;
				res.Properties.Assign(source.Properties);
				if(source is DevExpress.XtraBars.Docking2010.Views.WindowsUI.BaseTile && source.Properties.ItemSize == TileItemSize.Default)
					res.Properties.ItemSize = TileItemSize.Wide;
				res.Appearances.Assign(source.Appearances);
			}
			public virtual void BeginInit(ITileItem source) { }
			public virtual void EndInit(ITileItem source) { }
		}
		class FrameTileItemInitHelper : TileItemInitHelper {
			TileItemFrameStateInfo frameInfo = null;
			public override void BeginInit(ITileItem source) {
				TileItemFrame frame = source.Tag as TileItemFrame;
				if(frame == null)
					return;
				this.frameInfo = new TileItemFrameStateInfo(frame);
				source.SetContent(this.frameInfo.Frame, false);
			}
			public override void EndInit(ITileItem source) {
				if(this.frameInfo == null)
					return;
				frameInfo.Restore();
				source.SetContent(this.frameInfo.Frame, false);
				this.frameInfo = null;
			}
		}
		class ElementsEditPreviewControl : IDXCollectionEditorPreviewControl, IXtraResizableControl, IDisposable {
			public ElementsEditPreviewControl(TileControl previewControl) {
				this.previewControl = previewControl;
			}
			void TileCollectionEditorPreviewControl_Changed(object sender, EventArgs e) { }
			TileControl previewControl;
			public Control Control {
				get { return this.previewControl; }
			}
			public void OnCollectionChanged(DevExpress.Utils.Design.Internal.CollectionChangedEventArgs args) {
				TileItemElement element = args.Item as TileItemElement;
				if(element == null) return;
				switch(args.Action) {
					case DevExpress.Utils.Design.Internal.CollectionAction.Add:
						previewControl.Groups[0].Items[0].Elements.Add(element);
						break;
					case DevExpress.Utils.Design.Internal.CollectionAction.Remove:
						previewControl.Groups[0].Items[0].Elements.Remove(element);
						break;
					case DevExpress.Utils.Design.Internal.CollectionAction.Reorder:
						TileItemElement targetElement = args.TargetItem as TileItemElement;
						if(targetElement != null) {
							int elementIndex = previewControl.Groups[0].Items[0].Elements.IndexOf(element);
							int targetElementIndex = previewControl.Groups[0].Items[0].Elements.IndexOf(targetElement);
							UpdateElementsPos(element, elementIndex, targetElementIndex);
						}
						break;
					default:
						return;
				}
			}
			public void OnCollectionChanging(DevExpress.Utils.Design.Internal.CollectionChangingEventArgs args) { }
			void UpdateElementsPos(TileItemElement element, int pos, int newPos) {
				var anchorReferencesOld = new List<TileItemElement>();
				var anchorReferencesNew = new List<TileItemElement>();
				CollectReferencedElements(pos, anchorReferencesOld);
				CollectReferencedElements(newPos, anchorReferencesNew);
				previewControl.Groups[0].Items[0].Elements.RemoveAt(pos);
				previewControl.Groups[0].Items[0].Elements.Insert(newPos, element);
				UpdateAnchorIndexes(pos, newPos, anchorReferencesOld, anchorReferencesNew);
			}
			void CollectReferencedElements(int index, List<TileItemElement> list) {
				foreach(TileItemElement elem in previewControl.Groups[0].Items[0].Elements)
					if(elem.AnchorElementIndex == index)
						list.Add(elem);
			}
			void UpdateAnchorIndexes(int oldIndex, int newIndex, List<TileItemElement> oldReferences, List<TileItemElement> newReferences) {
				foreach(TileItemElement elem in previewControl.Groups[0].Items[0].Elements) {
					if(newReferences.Contains(elem))
						elem.AnchorElementIndex = oldIndex;
					if(oldReferences.Contains(elem))
						elem.AnchorElementIndex = newIndex;
				}
			}
			public void OnItemChanged(DevExpress.Utils.Design.Internal.PropertyItemChangedEventArgs args) { }
			public void Dispose() {
				if(previewControl != null) {
					previewControl.Dispose();
				}
			}
			event EventHandler IXtraResizableControl.Changed { add { } remove { } }
			bool IXtraResizableControl.IsCaptionVisible {
				get { return false; }
			}
			Size IXtraResizableControl.MaxSize {
				get { return new Size(0, 0); }
			}
			Size IXtraResizableControl.MinSize {
				get { return new Size(previewControl.Size.Width, previewControl.Size.Height); }
			}
			public void OnSelectedItemChanged(DevExpress.Utils.Design.Internal.SelectedItemChangedEventArgs args) { }
			internal void ClearPreview() {
				previewControl.Groups[0].Items[0].Elements.Clear();
			}
		}
		class TileItemElementEditorForm : DXCollectionEditorBase.DXCollectionEditorBaseForm {
			public TileItemElementEditorForm(TileItemElementsEditor owner)
				: base(owner) {
					Editor = owner;
			}
			TileItemElementsEditor Editor { get; set; }
			protected override void OnClosed(EventArgs e) {
				base.OnClosed(e);
				if(Editor != null)
					Editor.ClearPreview();
			}
		}
	}
	public class WindowsUIButtonPanelControlDesigner : ParentControlDesigner {
		public virtual DevExpress.XtraBars.Docking2010.WindowsUIButtonPanel ButtonPanel { get { return Control as DevExpress.XtraBars.Docking2010.WindowsUIButtonPanel; } }
		DesignerActionListCollection actionLists;
		public override DesignerActionListCollection ActionLists {
			get {
				if(actionLists != null) return actionLists;
				actionLists = CreateActionList();
				return base.ActionLists;
			}
		}
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			if(ButtonPanel != null)
				ButtonPanel.Buttons.AddRange(
					new DevExpress.XtraEditors.ButtonPanel.IBaseButton[] { 
						new XtraBars.Docking2010.WindowsUIButton(),
						new XtraBars.Docking2010.WindowsUIButton() });
		}
		DesignerActionListCollection CreateActionList() {
			DesignerActionListCollection res = new DesignerActionListCollection();
			res.Add(new MertoUIButtonPanelActionList(this));
			DXSmartTagsHelper.CreateDefaultLinks(this, res);
			return res;
		}
		internal class MertoUIButtonPanelActionList : DesignerActionList {
			WindowsUIButtonPanelControlDesigner designer;
			public MertoUIButtonPanelActionList(WindowsUIButtonPanelControlDesigner designer)
				: base(designer.Component) {
				this.designer = designer;
			}
			protected DevExpress.XtraBars.Docking2010.WindowsUIButtonPanel ButtonPanel { get { return designer.ButtonPanel; } }
			public override DesignerActionItemCollection GetSortedActionItems() {
				DesignerActionItemCollection res = new DesignerActionItemCollection();
				res.Add(new DesignerActionPropertyItem("Dock", "Choose Dock Style", "Properties"));
				res.Add(new DesignerActionPropertyItem("ContentAlignment", "Content Alignment", "Properties"));
				res.Add(new DesignerActionPropertyItem("Orientation", "Orientation", "Properties"));
				res.Add(new DesignerActionMethodItem(this, "Buttons", "Edit Buttons...", "Buttons", true));
				return res;
			}
			public void Buttons() {
				EditorContextHelper.EditValue(designer, ButtonPanel, "Buttons");
			}
			public ContentAlignment ContentAlignment {
				get {
					if(ButtonPanel == null) return ContentAlignment.MiddleCenter;
					return ButtonPanel.ContentAlignment;
				}
				set { EditorContextHelper.SetPropertyValue(designer, ButtonPanel, "ContentAlignment", value); }
			}
			public Orientation Orientation {
				get {
					if(ButtonPanel == null) return Orientation.Horizontal;
					return ButtonPanel.Orientation;
				}
				set { EditorContextHelper.SetPropertyValue(designer, ButtonPanel, "Orientation", value); }
			}
			public DockStyle Dock {
				get {
					if(ButtonPanel == null) return DockStyle.None;
					return ButtonPanel.Dock;
				}
				set { EditorContextHelper.SetPropertyValue(designer, ButtonPanel, "Dock", value); }
			}
		}
	}
	public class TileControlItemsTree : ItemsTreeView {
		TileControl tile;
		TreeNode dropTargetNode;
		RibbonControl ribbon;
		IComponentChangeService componentChangeService;
		IDesignerHost host;
		int tileControlDropDownIndex = 1;
		int inplaceTileControlIndex = 1;
		public TileControlItemsTree() {
			this.dropTargetNode = null;
			this.ribbon = null;
			this.componentChangeService = null;
			this.host = null;
		}
		public virtual RibbonControl Ribbon { get { return ribbon; } set { ribbon = value; } }
		public virtual IComponentChangeService ComponentChangeService { get { return componentChangeService; } set { componentChangeService = value; } }
		public virtual IDesignerHost Host { get { return host; } set { host = value; } }
		public virtual TileControl Tile { get { return tile; } }
		public string TileName { get { return tile.Name; } }
		public virtual void Initialize(string name, TileControl tileControl) {
			this.tile = tileControl;
			FillTree();
		}
		protected virtual TileGroup SelGroup {
			get {
				if(SelectedNode == null) return null;
				if(SelectedNode.Parent != null && SelectedNode.Parent.Tag as TileGroup != null) return SelectedNode.Parent.Tag as TileGroup;
				return SelectedNode.Tag as TileGroup;
			}
		}
		protected virtual TileItem SelItem {
			get {
				if(SelectedNode == null || SelectedNode.Tag as TileItem == null) return null;
				return SelectedNode.Tag as TileItem;
			}
		}
		protected virtual void FillTree() {
			Nodes.Clear();
			AddTileControlNode();
			int i = 0;
			foreach(TileGroup group in Tile.Groups) {
				AddGroupNode(group);
				SelectedNode = Nodes[0].Nodes[i];
				foreach(TileItem item in group.Items)
					AddItemNode(item);
				i++;
			}
		}
		protected virtual void AddTileControlNode() {
			int imageIndex = (int)TileControlTreeImages.InRibbonTileControl;
			TreeNode node = new TreeNode(Tile.Name, imageIndex, imageIndex);
			node.Tag = Tile;
			Nodes.Add(node);
		}
		public virtual void AddGroup() {
			if(tile == null) return;
			TileGroup grp = new TileGroup();
			GetValidName name = new GetValidName(Tile);
			grp.Name = name.ValidGroupName();
			grp.Text = grp.Name;
			Tile.Groups.Add(grp);
			if(Tile.Container != null)
				Tile.Container.Add(grp);
			ComponentChangeService.OnComponentChanged(Tile, null, null, null);
			SelectedNode = AddGroupNode(grp);
		}
		public virtual void AddItem(TileItemSize itemType) {
			if(SelGroup == null)
				return;
			TileItem item = TileControlDesignTimeManagerBase.CreateTileItem(itemType);
			item.Id = Tile.MaxId++;
			GetValidName name = new GetValidName(Tile);
			item.Name = name.ValidItemName();
			item.Text = item.Name;
			SelGroup.Items.Add(item);
			if(SelGroup.Container != null)
				SelGroup.Container.Add(item);
			SelectedNode = AddItemNode(item);
		}
		protected virtual TreeNode AddItemNode(TileItem item) {
			TreeNode node = new TreeNode(item.Name, (int)TileControlTreeImages.Item, (int)TileControlTreeImages.Item);
			node.Tag = item;
			if(SelItem == null)
				SelectedNode.Nodes.Add(node);
			else
				SelectedNode.Parent.Nodes.Add(node);
			return node;
		}
		protected virtual TreeNode AddGroupNode(TileGroup grp) {
			TreeNode node = new TreeNode(grp.Name, (int)TileControlTreeImages.Group, (int)TileControlTreeImages.Group);
			node.Tag = grp;
			Nodes[0].Nodes.Add(node);
			return node;
		}
		public object[] SelectedTreeItems {
			get {
				object[] selItems = new Object[SelNodes.Length];
				for(int itemIndex = 0; itemIndex < SelNodes.Length; itemIndex++) {
					selItems[itemIndex] = SelNodes[itemIndex].Tag;
				}
				return selItems;
			}
		}
		protected virtual bool CanMoveUp() {
			if(SelNodes == null || SelNodes.Length != 1 || SelectedNode.PrevNode == null) return false;
			return true;
		}
		protected virtual bool CanMoveDown() {
			if(SelNodes == null || SelNodes.Length != 1 || SelectedNode.NextNode == null) return false;
			return true;
		}
		protected virtual void SwapGroups(TreeNode node1, TreeNode node2) {
			TileGroup cont1 = node1.Tag as TileGroup, cont2 = node2.Tag as TileGroup;
			int index = Tile.Groups.IndexOf(cont1);
			Tile.Groups.Remove(cont2);
			Tile.Groups.Insert(index, cont2);
			ComponentChangeService.OnComponentChanged(Tile, null, null, null);
			SwapNodes(node1, node2);
		}
		protected virtual void SwapItems(TreeNode node1, TreeNode node2) {
			TileItem item1 = node1.Tag as TileItem, item2 = node2.Tag as TileItem;
			int index = item1.Group.Items.IndexOf(item1);
			item1.Group.Items.Remove(item2);
			item1.Group.Items.Insert(index, item2);
			ComponentChangeService.OnComponentChanged(item1.Group, null, null, null);
			SwapNodes(node1, node2);
		}
		protected virtual void SwapNodes(TreeNode node1, TreeNode node2) {
			int index = node1.Parent.Nodes.IndexOf(node1);
			node1.Parent.Nodes.Remove(node2);
			node1.Parent.Nodes.Insert(index, node2);
		}
		protected virtual bool IsSelGroup {
			get {
				if(SelectedNode == null) return false;
				return SelectedNode.Tag is TileGroup;
			}
		}
		protected virtual bool IsSelItem {
			get {
				if(SelectedNode == null) return false;
				return SelectedNode.Tag is TileItem;
			}
		}
		public virtual void MoveUp() {
			if(!CanMoveUp()) return;
			TreeNode selNode = SelectedNode;
			if(IsSelGroup) SwapGroups(SelectedNode.PrevNode, SelectedNode);
			else if(IsSelItem) SwapItems(SelectedNode.PrevNode, SelectedNode);
			SelectedNode = selNode;
		}
		public virtual void MoveDown() {
			if(!CanMoveDown()) return;
			TreeNode selNode = SelectedNode;
			if(IsSelGroup) SwapGroups(SelectedNode, SelectedNode.NextNode);
			else if(IsSelItem) SwapItems(SelectedNode, SelectedNode.NextNode);
			SelectedNode = selNode;
		}
		protected virtual TileControlItemsDragInfo GetDragObject(IDataObject data) {
			return data.GetData(typeof(TileControlItemsDragInfo)) as TileControlItemsDragInfo;
		}
		protected virtual bool IsNodeInDragNodes(TileControlItemsDragInfo info, TreeNode node) {
			for(int itemIndex = 0; itemIndex < info.Nodes.Length; itemIndex++) {
				if(info.Nodes[itemIndex] == node) return true;
			}
			return false;
		}
		protected virtual DragDropEffects GetDragDropEffect(TileControlItemsDragInfo info, TreeNode node) {
			if(info.IsTileControl || node == null || IsNodeInDragNodes(info, node)) return DragDropEffects.None;
			if(info.IsGroups && node.Tag is TileItem) return DragDropEffects.None;
			if(info.IsItems && !(node.Tag is TileItem || node.Tag is TileGroup)) return DragDropEffects.None;
			return DragDropEffects.Move;
		}
		public override void OnDragNodeGetObject(object sender, TreeViewGetDragObjectEventArgs e) {
			if(SelNodes == null || SelNodes.Length == 0) return;
			TileControlItemsDragInfo info = new TileControlItemsDragInfo(SelNodes);
			e.DragObject = new TileControlItemsDragInfo(SelNodes);
			e.AllowEffects = DragDropEffects.Move;
		}
		public override void OnDragNodeStart(object sender, EventArgs e) { DropTargetNode = null; }
		protected TreeNode DropTargetNode { get { return dropTargetNode; } set { dropTargetNode = value; } }
		protected virtual TileControlTreeImages GetDirection(TreeNode node, TileControlItemsDragInfo dragInfo) {
			if(dragInfo.Nodes[0].Parent != node.Parent) return TileControlTreeImages.MoveUp;
			if(node.Parent.Nodes.IndexOf(node) > node.Parent.Nodes.IndexOf(dragInfo.Nodes[dragInfo.Nodes.Length - 1])) return TileControlTreeImages.MoveDown;
			return TileControlTreeImages.MoveUp;
		}
		protected virtual TileControlTreeImages GetNodeImageIndex(TreeNode node, TileControlItemsDragInfo dragInfo) {
			if(IsNodeInDragNodes(dragInfo, node)) return TileControlTreeImages.CantMove;
			if(dragInfo.IsGroups) {
				if(node.Tag is TileItem) return TileControlTreeImages.CantMove;
				else
					if(node.Tag is TileGroup) return GetDirection(node, dragInfo);
				return TileControlTreeImages.MoveIn;
			}
			if(dragInfo.IsItems) {
				if(node.Tag is TileItem) return GetDirection(node, dragInfo);
				else
					if(node.Tag is TileGroup) return TileControlTreeImages.MoveIn;
				return TileControlTreeImages.CantMove;
			}
			return TileControlTreeImages.CantMove;
		}
		protected virtual TileControlTreeImages DefaultImageIndex(TreeNode node) {
			if(node.Tag is RibbonGalleryBarItem)
				return TileControlTreeImages.InRibbonTileControl;
			else if(node.Tag is GalleryDropDown)
				return TileControlTreeImages.PopupTileControl;
			else if(node.Tag is TileGroup)
				return TileControlTreeImages.Group;
			return TileControlTreeImages.Item;
		}
		protected virtual void ResetNodeImage(TreeNode node) {
			if(node == null) return;
			node.ImageIndex = node.SelectedImageIndex = (int)DefaultImageIndex(node);
		}
		protected virtual TileControl GetTileControl(TreeNode node) {
			if(Tile != null) return Tile;
			return null;
		}
		protected virtual void RemoveObjectFromCollection(TreeNode node) {
			TileControl tileControl = GetTileControl(node.Parent);
			TileGroup tileGroup = node.Parent.Tag as TileGroup;
			if(tileGroup != null) tileGroup.Items.Remove(node.Tag as TileItem);
			else if(tileControl != null) tileControl.Groups.Remove(node.Tag as TileGroup);
		}
		protected virtual void RemoveObjectsFromCollection(TileControlItemsDragInfo info, TreeNode node) {
			for(int itemIndex = 0; itemIndex < info.Nodes.Length; itemIndex++) {
				RemoveObjectFromCollection(info.Nodes[itemIndex]);
				info.Nodes[itemIndex].Parent.Nodes.Remove(info.Nodes[itemIndex]);
			}
		}
		protected virtual int GetInsertIndex(TileControlItemsDragInfo info, TreeNode node, TileControlTreeImages direction) {
			TileControl tileControl = GetTileControl(node);
			TileGroup tileGroup = node.Tag as TileGroup;
			TileItem tileItem = node.Tag as TileItem;
			if(tileItem != null) return direction == TileControlTreeImages.MoveDown ? tileItem.Group.Items.IndexOf(tileItem) + 1 : tileItem.Group.Items.IndexOf(tileItem);
			if(tileGroup != null) {
				if(info.IsItems) return tileGroup.Items.Count;
				else return direction == TileControlTreeImages.MoveDown ? Tile.Groups.IndexOf(tileGroup) + 1 : Tile.Groups.IndexOf(tileGroup);
			}
			if(tileControl != null) return tileControl.Groups.Count;
			return 0;
		}
		protected virtual void InsertObjectsIntoCollection(TileControlItemsDragInfo info, TreeNode node, TileControlTreeImages direction) {
			int insertAt = GetInsertIndex(info, node, direction);
			if(node.Tag is TileControl) InsertObjectsIntoTileControl(info, node, insertAt);
			else if(node.Tag is TileGroup) InsertObjectsIntoGroup(info, node, insertAt);
			else if(node.Tag is TileItem) InsertObjectsIntoGroup(info, node.Parent, insertAt);
		}
		protected virtual void InsertObjectsIntoTileControl(TileControlItemsDragInfo info, TreeNode node, int insertAt) {
			for(int itemIndex = info.Nodes.Length - 1; itemIndex >= 0; itemIndex--) {
				Tile.Groups.Insert(insertAt, info.Nodes[itemIndex].Tag as TileGroup);
				if(Tile.Container != null)
					Tile.Container.Add(info.Nodes[itemIndex].Tag as TileGroup);
				Nodes[0].Nodes.Insert(insertAt, info.Nodes[itemIndex]);
			}
			ComponentChangeService.OnComponentChanged(Tile, null, null, null);
			return;
		}
		protected virtual void InsertItemsIntoGroup(TileControlItemsDragInfo info, TreeNode node, int insertAt) {
			TileGroup grp = node.Tag as TileGroup;
			for(int index = info.Nodes.Length - 1; index >= 0; index--) {
				grp.Items.Insert(insertAt, info.Nodes[index].Tag as TileItem);
				if(grp.Container != null)
					grp.Container.Add(info.Nodes[index].Tag as TileItem);
				node.Nodes.Insert(insertAt, info.Nodes[index]);
			}
			ComponentChangeService.OnComponentChanged(grp, null, null, null);
		}
		protected virtual void InsertObjectsIntoGroup(TileControlItemsDragInfo info, TreeNode node, int insertAt) {
			if(info.IsItems) InsertItemsIntoGroup(info, node, insertAt);
			else InsertObjectsIntoTileControl(info, node, insertAt);
		}
		protected virtual void MoveNodesTo(TileControlItemsDragInfo info, TreeNode node) {
			TileControlTreeImages direction = GetDirection(node, info);
			object grp = info.Nodes[0].Parent.Tag;
			RemoveObjectsFromCollection(info, node);
			ComponentChangeService.OnComponentChanged(grp, null, null, null);
			InsertObjectsIntoCollection(info, node, direction);
		}
		protected override void OnDragOver(object sender, DragEventArgs e) {
			e.Effect = DragDropEffects.None;
			TileControlItemsDragInfo dragInfo = GetDragObject(e.Data);
			if(dragInfo == null) return;
			TreeNode node = GetNodeAt(PointToClient(new Point(e.X, e.Y)));
			e.Effect = GetDragDropEffect(dragInfo, node);
			if(DropTargetNode == node) return;
			SetNodeImageIndex(node, (int)GetNodeImageIndex(node, dragInfo));
			ResetNodeImage(DropTargetNode);
			DropTargetNode = node;
		}
		public override void OnDragEnter(object sender, DragEventArgs e) { }
		public override void OnDragLeave(object sender, EventArgs e) { ResetNodeImage(DropTargetNode); }
		protected override void OnDragDrop(object sender, DragEventArgs e) {
			TileControlItemsDragInfo dragInfo = GetDragObject(e.Data);
			if(dragInfo == null) return;
			TreeNode node = GetNodeAt(PointToClient(new Point(e.X, e.Y)));
			MoveNodesTo(dragInfo, node);
			ResetNodeImage(DropTargetNode);
		}
		public virtual void RemoveItems() {
			BeginUpdate();
			try {
				if(Nodes.Count == 0 || SelNodes == null || SelNodes.Length == 0) return;
				TreeNode[] nodes = new TreeNode[SelNodes.Length];
				for(int i = 0; i < SelNodes.Length; i++) nodes[i] = SelNodes[i];
				for(int i = 0; i < nodes.Length; i++) RemoveNode(nodes[i]);
			}
			finally { EndUpdate(); }
		}
		protected virtual void RemoveNode(TreeNode node) {
			TileItem item = node.Tag as TileItem;
			TileGroup grp = node.Tag as TileGroup;
			TileControl tileControl = GetTileControl(node);
			if(item != null) {
				TileGroup itemGroup = item.Group;
				item.Group.Items.Remove(item);
				if(itemGroup.Container != null)
					itemGroup.Container.Remove(item);
				ComponentChangeService.OnComponentChanged(itemGroup, null, null, null);
				node.Parent.Nodes.Remove(node);
			}
			else if(grp != null) {
				while(node.Nodes.Count != 0) {
					RemoveNode(node.Nodes[0]);
				}
				ComponentChangeService.OnComponentChanged(Tile, null, null, null);
				grp.Dispose();
				node.Parent.Nodes.Remove(node);
			}
		}
		protected override void UpdateTreeNodeText(TreeNode node, object obj) {
			var od = obj as TileControlObjectDescriptor;
			if(od == null || node.Tag != od.Item) return;
			var item = od.Item;
			if(item is RibbonGalleryBarItem) node.Text = (item as RibbonGalleryBarItem).Caption;
			else if(item is GalleryDropDown) node.Text = (item as GalleryDropDown).Name;
			else if(item is TileGroup) node.Text = (item as TileGroup).Name;
			else if(item is TileItem) node.Text = (item as TileItem).Name;
			else if(item is Control) node.Text = (item as Control).Name;
		}
		public int InplaceTileControlIndex { get { return inplaceTileControlIndex; } set { inplaceTileControlIndex = value; } }
		public int TileControlDropDownIndex { get { return tileControlDropDownIndex; } set { tileControlDropDownIndex = value; } }
		RibbonBarManager Manager { get { return Ribbon.Manager as RibbonBarManager; } }
	}
	public class GetValidName {
		Control ctrl;
		public GetValidName(Control control) {
			this.ctrl = control;
		}
		public string ValidGroupName() {
			INameCreationService nameService;
			nameService = (INameCreationService)ctrl.Site.GetService(typeof(INameCreationService));
			return nameService.CreateName(ctrl.Container, typeof(TileGroup));
		}
		public string ValidItemName() {
			INameCreationService nameService;
			nameService = (INameCreationService)ctrl.Site.GetService(typeof(INameCreationService));
			return nameService.CreateName(ctrl.Container, typeof(TileItem));
		}
	}
	public class TileControlItemsDragInfo {
		TreeNode[] nodes;
		public TileControlItemsDragInfo(TreeNode[] nodes) {
			this.nodes = nodes;
		}
		public TreeNode[] Nodes { get { return nodes; } }
		public bool IsItems { get { return Nodes[0].Tag is TileItem; } }
		public bool IsGroups { get { return Nodes[0].Tag is TileGroup; } }
		public bool IsTileControl { get { return Nodes[0].Tag is TileControl; } }
		public TileGroup[] Groups {
			get {
				if(!IsGroups) return null;
				TileGroup[] conts = new TileGroup[Nodes.Length];
				for(int itemIndex = 0; itemIndex < Nodes.Length; itemIndex++) {
					conts[itemIndex] = Nodes[itemIndex].Tag as TileGroup;
				}
				return conts;
			}
		}
		public TileItem[] Items {
			get {
				if(!IsItems) return null;
				TileItem[] items = new TileItem[Nodes.Length];
				for(int itemIndex = 0; itemIndex < Nodes.Length; itemIndex++) {
					items[itemIndex] = Nodes[itemIndex].Tag as TileItem;
				}
				return items;
			}
		}
	}
	public class NavigationBarItemCollectionEditor : DXCollectionEditorBase {
		public NavigationBarItemCollectionEditor(Type type)
			: base(type) {
		}
		protected override Type CreateCollectionItemType() {
			return typeof(NavigationBarItem);
		}
		protected override Type[] CreateNewItemTypes() {
			return new Type[] { typeof(NavigationBarItem) };
		}
		protected override bool AllowLiveUpdates {
			get { return true; }
		}
		protected override object CreateCustomInstance(Type itemType) {
			NavigationBarItem item = new NavigationBarItem();
			if(item != null)
				item.Text = GetItemText();
			return item;
		}
		NavigationBarItemCollection Collection {
			get {
				if(this.Context == null) return null;
				var control = this.Context.Instance as OfficeNavigationBar;
				if(control == null) return null;
				return control.Items;
			}
		}
		string GetItemText() {
			string prefix = "Item";
			for(int i = 1; true; i++) {
				string res = string.Concat(prefix, i.ToString());
				if(!IsItemExist(res)) return res;
			}
		}
		bool IsItemExist(string text) {
			foreach(NavigationBarItem item in Collection)
				if(string.Equals(item.Text, text)) return true;
			return false;
		}
	}
	public class OfficeNavigationBarDesigner : ParentControlDesigner {
		public virtual OfficeNavigationBar NavigationBar { get { return Control as OfficeNavigationBar; } }
		DesignerActionListCollection actionLists;
		public override DesignerActionListCollection ActionLists {
			get {
				if(actionLists != null) return actionLists;
				actionLists = CreateActionList();
				return base.ActionLists;
			}
		}
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			if(NavigationBar == null) return;
			NavigationBar.Items.AddRange(
				new NavigationBarItem[] { 
						new NavigationBarItem(){Text = "Item1"},
						new NavigationBarItem() {Text = "Item2"} });
			NavigationBar.Dock = DockStyle.Bottom;
		}
		DesignerActionListCollection CreateActionList() {
			DesignerActionListCollection res = new DesignerActionListCollection();
			res.Add(new OfficeNavigationBarDesignerActionList(this));
			DXSmartTagsHelper.CreateDefaultLinks(this, res);
			return res;
		}
		internal class OfficeNavigationBarDesignerActionList : DesignerActionList {
			OfficeNavigationBarDesigner designer;
			public OfficeNavigationBarDesignerActionList(OfficeNavigationBarDesigner designer)
				: base(designer.Component) {
				this.designer = designer;
			}
			protected OfficeNavigationBar NavigationBar { get { return designer.NavigationBar; } }
			public override DesignerActionItemCollection GetSortedActionItems() {
				DesignerActionItemCollection res = new DesignerActionItemCollection();
				res.Add(new DesignerActionPropertyItem("Dock", "Choose Dock Style", "Properties"));
				res.Add(new DesignerActionMethodItem(this, "Items", "Edit Items...", "Items", true));
				return res;
			}
			public void CustomButtons() {
				EditorContextHelper.EditValue(designer, NavigationBar, "CustomButtons");
			}
			public void Items() {
				EditorContextHelper.EditValue(designer, NavigationBar, "Items");
			}
			public DockStyle Dock {
				get {
					if(NavigationBar == null) return DockStyle.None;
					return NavigationBar.Dock;
				}
				set { EditorContextHelper.SetPropertyValue(designer, NavigationBar, "Dock", value); }
			}
		}
	}
	public enum TileControlTreeImages { InRibbonTileControl, PopupTileControl, Group, Item, MoveIn, MoveUp, MoveDown, CantMove }
	public class TileNavPaneDesignTimeManager : TileNavPaneDesignTimeManagerBase {
		public TileNavPaneDesignTimeManager(IComponent component, TileNavPane tileNavPane)
			: base(component, tileNavPane) { }
		public virtual void ShowDesignerForm(TileNavPane tileNavPane) {
			if(tileNavPane == null) return;
			IUIService srv = Component.Site.GetService(typeof(IUIService)) as IUIService;
			using(TileNavPaneDesignerForm form = new TileNavPaneDesignerForm()) {
				form.Assign(tileNavPane, this);
				form.ShowDialog(srv == null ? null : srv.GetDialogOwnerWindow());
			}
		}
		public void OnEditDefaultCategoryItemsClick() {
			var tnp = Component as TileNavPane;
			if(tnp == null) return;
			EditItems(tnp.DefaultCategory);
		}
		public override void EditItems(TileNavCategory cat) {
			var tileNavPane = cat.Owner;
			if(tileNavPane == null) return;
			IUIService srv = Component.Site.GetService(typeof(IUIService)) as IUIService;
			using(TileNavPaneDesignerForm form = new TileNavPaneDesignerForm()) {
				form.Assign(tileNavPane, cat, this);
				form.ShowDialog(srv == null ? null : srv.GetDialogOwnerWindow());
			}
		}
	}
	public class TileNavPaneDesigner : BaseControlDesigner, ITileNavPaneDesigner, IKeyCommandProcessInfo {
		public TileNavPane TileNavPane { get { return Component as TileNavPane; } }
		TileNavPaneKeyCommandProcessHelper keyCommandHelper;
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			keyCommandHelper = new TileNavPaneKeyCommandProcessHelper(this);
			TileNavPane.ViewInfo.DesignTimeManager = new TileNavPaneDesignTimeManager(Component, TileNavPane);
		}
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			((TileNavPane)TileNavPane).ViewInfo.CreateMainButton();
			TileNavPane.Dock = DockStyle.Top;
		}
		protected override bool GetHitTest(Point point) {
			Point pt = TileNavPane.PointToClient(point);
			TileNavPaneHitInfo hitInfo = TileNavPane.ViewInfo.CalcHitInfo(pt);
			if(hitInfo.InButton)
				return true;
			return base.GetHitTest(point);
		}
		protected virtual void CreateTileNavPaneDesignerActionLists(DesignerActionListCollection list) {
			list.Add(new TileNavPaneDesignerActionList(this));
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			CreateTileNavPaneDesignerActionLists(list);
			base.RegisterActionLists(list);
		}
		protected override bool CanUseComponentSmartTags {
			get { return true; }
		}
		bool ITileNavPaneDesigner.DebuggingState {
			get { return DebuggingState; }
		}
		IServiceProvider IKeyCommandProcessInfo.ServiceProvider {
			get { return Component.Site; }
		}
		BaseDesignTimeManager IKeyCommandProcessInfo.DesignTimeManager {
			get { return TileNavPane.ViewInfo.DesignTimeManager; }
		}
		IComponent IKeyCommandProcessInfo.Component {
			get { return Component as TileNavPane; }
		}
		public override ICollection AssociatedComponents {
			get {
				ArrayList list = new ArrayList();
				AddCategories(list);
				AddButtons(list);
				AddBase(list);
				return list;
			}
		}
		protected void AddButtons(ArrayList list) {
			foreach(ITileNavButton button in TileNavPane.Buttons) {
				list.Add(button);
			}
		}
		protected void AddCategories(ArrayList list) {
			foreach(TileNavCategory cat in TileNavPane.Categories) {
				list.Add(cat);
			}
			if(TileNavPane.DefaultCategory.Items.Count == 0) return;
			list.Add(TileNavPane.DefaultCategory);
		}
		protected void AddBase(ArrayList controls) {
			foreach(object obj in base.AssociatedComponents) {
				if(controls.Contains(obj)) continue;
				controls.Add(obj);
			}
		}
	}
	public class TileNavPaneKeyCommandProcessHelper : DesignTimeKeyCommandProcessHelperBase {
		TileNavPaneDesigner designer;
		public TileNavPaneKeyCommandProcessHelper(TileNavPaneDesigner tnpDesigner)
			: base(tnpDesigner) {
			designer = tnpDesigner;
		}
		TileNavPaneDesignTimeManagerBase DTManager {
			get { return designer.TileNavPane.ViewInfo.DesignTimeManager; }
		}
		public override void OnKeyCancel(object sender, EventArgs e) {
			NavElement element = DTManager.GetElement();
			if(element != null) {
				DTManager.SelectComponent(designer.TileNavPane);
				designer.TileNavPane.Invalidate();
			}
			else PassControlToOldKeyCancelHandler();
		}
	}
	public class TileNavPaneDesignerActionList : DesignerActionList {
		IDesigner designer;
		public TileNavPaneDesignerActionList(IDesigner designer)
			: base(designer.Component) {
			this.designer = designer;
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection res = new DesignerActionItemCollection();
			res.Add(new DesignerActionHeaderItem("Actions", "Actions"));
			res.Add(new DesignerActionMethodItem(this, "RunDesigner", "Run Designer", "Actions", true));
			res.Add(new DesignerActionMethodItem(this, "EditDefaultCategoryItems", "Edit Default Category Items", "Actions", true));
			res.Add(new DesignerActionMethodItem(this, "AddButton", "Add Button", "Actions", true));
			res.Add(new DesignerActionMethodItem(this, "AddCategoryButton", "Add Category Button", "Actions", true));
			return res;
		}
		public virtual void RunDesigner() {
			Helper.ShowDesignerForm(TileNavPane);
		}
		public virtual void EditDefaultCategoryItems() {
			Helper.OnEditDefaultCategoryItemsClick();
		}
		public virtual void AddButton() {
			Helper.OnAddButtonClick();
		}
		public virtual void AddCategoryButton() {
			Helper.OnAddCategoryButtonClick();
		}
		public virtual void AddCategory() {
			Helper.OnAddCategoryClick();
		}
		TileNavPaneDesignTimeManager helper;
		public TileNavPaneDesignTimeManager Helper {
			get {
				if(helper == null)
					helper = new TileNavPaneDesignTimeManager(TileNavPane, TileNavPane);
				return helper;
			}
		}
		public IDesigner Designer { get { return designer; } }
		public TileNavPane TileNavPane { get { return Component as TileNavPane; } }
	}
	public class TileNavCategoryDesigner : BaseComponentDesigner {
		public override void Initialize(IComponent component) {
			base.Initialize(component);
		}
		public override ICollection AssociatedComponents {
			get {
				ArrayList list = new ArrayList();
				AddItems(list);
				AddBase(list);
				return list;
			}
		}
		public TileNavCategory Category { get { return (TileNavCategory)Component; } }
		protected virtual void AddItems(ArrayList list) {
			foreach(TileNavItem item in Category.Items) {
				list.Add(item);
			}
		}
		protected void AddBase(ArrayList controls) {
			foreach(object obj in base.AssociatedComponents) {
				if(controls.Contains(obj)) continue;
				controls.Add(obj);
			}
		}
		public override DesignerVerbCollection DXVerbs {
			get {
				DesignerVerbCollection coll = base.DXVerbs;
				DesignerVerb editItemVerb = new DesignerVerb("Edit Items", new EventHandler(OnEditItemsClick));
				coll.Add(editItemVerb);
				return coll;
			}
		}
		protected override bool UseVerbsAsActionList {
			get { return true; }
		}
		TileNavPaneDesignTimeManager helper;
		public TileNavPaneDesignTimeManager Helper {
			get {
				if(helper == null)
					helper = new TileNavPaneDesignTimeManager(Category, Category.Owner);
				return helper;
			}
		}
		protected virtual void OnEditItemsClick(object sender, EventArgs e) {
			Helper.EditItems(Category);
		}
	}
	public class TileNavItemDesigner : BaseComponentDesigner {
		public TileNavItem Item { get { return Component as TileNavItem; } }
		public override ICollection AssociatedComponents {
			get {
				ArrayList list = new ArrayList();
				AddSubItems(list);
				AddBase(list);
				return list;
			}
		}
		protected virtual void AddSubItems(ArrayList list) {
			foreach(TileNavSubItem subitem in Item.SubItems) {
				list.Add(subitem);
			}
		}
		protected void AddBase(ArrayList controls) {
			foreach(object obj in base.AssociatedComponents) {
				if(controls.Contains(obj)) continue;
				controls.Add(obj);
			}
		}
	}
}
namespace DevExpress.XtraEditors.Design {
	public class PopupGalleryEditEditorForm : RibbonEditorForm {
		public PopupGalleryEditEditorForm() : base() { }
		public PopupGalleryEditEditorForm(object component)
			: base(component) {
		}
		protected override void InitializeComponent() {
			base.InitializeComponent();
			Text = "Gallery Designer";
		}
	}
	[CLSCompliant(false)]
	public class PopupGalleryEditDesigner : ButtonEditDesigner {
		public PopupGalleryEditDesigner()
			: base() {
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			PopupGalleryEdit.Container.Add(PopupGalleryEdit.Properties.Gallery);
			ComponentChangeSvc.ComponentRemoving += OnComponentRemoving;
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new SingleMethodActionList(this, new MethodInvoker(OnEditGalleryCore), "Edit Gallery", true));
			base.RegisterActionLists(list);
		}
		protected PopupGalleryEdit PopupGalleryEdit { get { return (PopupGalleryEdit)Component; } }
		void OnEditGalleryCore() {
			IUIService srv = Component.Site.GetService(typeof(IUIService)) as IUIService;
			PopupGalleryEdit.Properties.Gallery.PopupGalleryEdit = PopupGalleryEdit;
			using(PopupGalleryEditEditorForm form = new PopupGalleryEditEditorForm(PopupGalleryEdit.Properties.Gallery)) {
				form.InitEditingObject(PopupGalleryEdit.Properties.Gallery);
				form.ShowDialog(srv == null ? null : srv.GetDialogOwnerWindow());
			}
		}
		protected virtual void OnComponentRemoving(object sender, ComponentEventArgs e) {
			if(e.Component == PopupGalleryEdit) {
				PopupGalleryEditGallery popupGalleryEditGallery = PopupGalleryEdit.Properties.Gallery;
				if(popupGalleryEditGallery != null) {
					PopupGalleryEdit.Container.Remove(popupGalleryEditGallery);
				}
			}
		}
		IComponentChangeService componentChangeSvc = null;
		protected IComponentChangeService ComponentChangeSvc {
			get {
				if(componentChangeSvc == null) {
					componentChangeSvc = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
				}
				return componentChangeSvc;
			}
		}
	}
}
