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
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraBars.Painters;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraBars.Forms;
using DevExpress.XtraBars.Controls; 
using DevExpress.XtraBars.Ribbon;
using DevExpress.Utils.Menu;
using System.Security;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing.Animation;
namespace DevExpress.XtraBars {
	public interface PopupControl {
		object Activator { get; set; }
		void ShowPopup(BarManager manager, Point p);
		void HidePopup();
		bool CanShowPopup { get; }
		bool Visible { get;	}
		Rectangle Bounds { get; }
		IPopup IPopup { get; }
		RibbonControl Ribbon { get; set; }
		BarManager Manager { get; set; }
		event EventHandler CloseUp;
		event EventHandler Popup;
	}
	[Designer("DevExpress.XtraBars.Design.PopupControlContainerDesigner, " + AssemblyInfo.SRAssemblyBarsDesign),				
	 Description("Represents a control container that can be displayed as a drop-down window by various XtraBars controls."),
	 ToolboxTabName(AssemblyInfo.DXTabNavigation), DXToolboxItem(true),
	ToolboxBitmap(typeof(ToolboxIcons.ToolboxIconsRootNS), "PopupControlContainer")
	]
	public class PopupControlContainer : PanelControl, PopupControl, IBarObject, ISupportLookAndFeel, IDXDropDownControlEx {
		static Size DefaultFormMinimumSize = new Size(50, 50);
		static PopupControlContainer() {
			DevExpress.Utils.Design.DXAssemblyResolver.Init();
		}
		public event EventHandler CloseUp, Popup;
		object activator;
		bool closeOnLostFocus, closeOnOuterMouseClick;
		PopupContainerBarControl subControl;
		BarManager manager;
		RibbonControl ribbon;
		Size formMinimumSize = DefaultFormMinimumSize;
		bool showCloseButton, showSizeGrip;
		public PopupControlContainer(IContainer container)
			: this() {
			container.Add(this);
		}
		public PopupControlContainer() {
			this.closeOnOuterMouseClick = this.closeOnLostFocus = true;
			this.activator = null;
			this.manager = null;
			this.Visible = false;
			this.subControl = null;
			this.BorderStyle = BorderStyles.NoBorder;
		}
		protected override Size DefaultSize {
			get { return new Size(250, 130); }
		}
		bool ISupportLookAndFeel.IgnoreChildren { get { return true; } }
		bool IBarObject.IsBarObject { get { return true; } }
		bool IBarObject.ShouldCloseOnLostFocus(Control newFocus) { return CloseOnLostFocus; }
		BarMenuCloseType IBarObject.ShouldCloseMenuOnClick(MouseInfoArgs e, Control child) {
			if(ClientRectangle.Contains(PointToClient(e.ScreenPoint))) return BarMenuCloseType.None;
			return BarMenuCloseType.None;
		}
		bool IBarObject.ShouldCloseOnOuterClick(Control control, MouseInfoArgs e) {
			if(PopupHelper.IsBelowModalForm(((PopupControl)this).IPopup.PopupForm, BarManager.ClosePopupOnModalFormShow))
				return false;
			return CloseOnOuterMouseClick; 
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("PopupControlContainerCloseOnOuterMouseClick"),
#endif
 DefaultValue(true)]
		public virtual bool CloseOnOuterMouseClick {
			get { return closeOnOuterMouseClick; }
			set {
				closeOnOuterMouseClick = value;
			}
		}
		bool ShouldSerializeFormMinimumSize() { return !Size.Equals(FormMinimumSize, DefaultFormMinimumSize); }
		void ResetFormMinimumSize() { FormMinimumSize = DefaultFormMinimumSize; }
#if !SL
	[DevExpressXtraBarsLocalizedDescription("PopupControlContainerFormMinimumSize")]
#endif
		public virtual Size FormMinimumSize {
			get { return formMinimumSize; }
			set { formMinimumSize = value; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("PopupControlContainerShowCloseButton"),
#endif
 DefaultValue(false), SmartTagProperty("Show Close Button", "", SmartTagActionType.RefreshBoundsAfterExecute)]
		public virtual bool ShowCloseButton {
			get { return showCloseButton; }
			set { showCloseButton = value; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("PopupControlContainerShowSizeGrip"),
#endif
 DefaultValue(false), SmartTagProperty("Show Size Grip", "", SmartTagActionType.RefreshBoundsAfterExecute)]
		public virtual bool ShowSizeGrip {
			get { return showSizeGrip; }
			set { showSizeGrip = value; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("PopupControlContainerCloseOnLostFocus"),
#endif
 DefaultValue(true)]
		public virtual bool CloseOnLostFocus {
			get { return closeOnLostFocus; }
			set {
				closeOnLostFocus = value;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("PopupControlContainerRibbon"),
#endif
 DefaultValue(null), RefreshProperties(RefreshProperties.All), SmartTagProperty("Ribbon", "")]
		public virtual RibbonControl Ribbon {
			get { return ribbon; }
			set {
				if(Ribbon == value) return;
				if(ribbon != null)
					ribbon.Disposed -= OnRibbonDisposed;
				ribbon = value;
				if(ribbon != null)
					ribbon.Disposed += OnRibbonDisposed;
				if(Manager == null) HidePopup();
			}
		}
		void OnRibbonDisposed(object sender, EventArgs e) {
			OnRibbonDisposeCore();
		}
		bool ShouldSerializeManager() { return Ribbon == null && Manager != null; }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("PopupControlContainerManager"),
#endif
 RefreshProperties(RefreshProperties.All), SmartTagProperty("Manager", "")]
		public virtual BarManager Manager {
			get { return Ribbon == null ? manager : Ribbon.Manager; }
			set {
				if(Manager == value) return;
				if(manager != null)
					manager.Disposed -= OnManagerDisposed;
				this.manager = value;
				if(manager != null)
					manager.Disposed += OnManagerDisposed;				
				if(Manager == null) HidePopup();
			}
		}
		void OnManagerDisposed(object sender, EventArgs e) {
			OnManagerDisposeCore();
		}
		protected virtual void OnRibbonDisposeCore() {
			Ribbon = null;
		}
		protected virtual void OnManagerDisposeCore() {
			Manager = null;
		}
		protected override void Dispose(bool disposing) {
			OnRibbonDisposeCore();
			OnManagerDisposeCore();			
			base.Dispose(disposing);
		}
		protected internal PopupContainerBarControl SubControl {
			get { return subControl; }
			set {
				if(SubControl == value) return;
				PopupContainerBarControl prevControl = SubControl;
				subControl = null;
				if(prevControl != null && !prevControl.Destroying) {
					prevControl.Dispose();
				}
				subControl = value;
				if(value == null) 
					OnCloseUp(prevControl);
				else
					OnPopup();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual object Activator { 
			get { return activator; } 
			set { activator = value; }
		}
		protected override void OnPaint(PaintEventArgs e) {
			if(DesignMode) {
				Rectangle r = this.ClientRectangle;
				r.Width--;
				r.Height--;
				DXControlPaint.DrawDashedBorder(e.Graphics, this, DesignTimeGridColor);
			}
			else {
				base.OnPaint(e);
			}
		}
		Color designTimeGridColor = Color.Empty;
		protected Color DesignTimeGridColor {
			get {
				if(designTimeGridColor.IsEmpty) {
					designTimeGridColor = CreateDesignTimeGridColor();
				}
				return designTimeGridColor;
			}
		}
		protected virtual Color CreateDesignTimeGridColor() {
			return Color.FromArgb(160, 160, 160);
		}
		public virtual void HidePopup() {
			BarManager manager = SubControl == null ? null : SubControl.Manager;
			if(SubControl == null || manager == null) return;
			if(manager.SelectionInfo.TemporaryActive) manager.SelectionInfo.TemporaryActive = false;
			manager.SelectionInfo.ClosePopup(SubControl);
			Activator = null;
		}
		IPopup GetPopup() {
			BarItemLink link = Activator as BarItemLink;
			if(link != null) return link.BarControl as IPopup;
			return null;
		}
		public virtual void ShowPopup(BarManager manager, Point p) {
			if(manager == null) return;
			if(this.manager == null) this.manager = manager;
			HidePopup();
			SubControl = CreateSubControl(manager);
			if(SubControl == null) return;
			if(manager.GetPopupShowMode(SubControl) == PopupShowMode.Inplace &&
				Manager.SelectionInfo.GetLastOpenedPopupForm() != null) {
				XtraAnimator.Current.AddAnimation(new SubMenuInplaceAnimationInfo(Manager.SelectionInfo.GetLastOpenedPopupForm(), SubControl, false));
			}
			manager.SelectionInfo.OpenPopup(GetPopup(), SubControl, CalcLocationInfo(p));
			NCActivateWithFocus(manager, this);
			if(!manager.IsBarsActive) manager.SelectionInfo.TemporaryActive = true; 
		}
		public virtual void ShowPopup(Point p) { ShowPopup(Manager, p); }
		protected internal static void NCActivate(BarManager manager) {
			NCActivateForm(manager.GetForm());
		}
		protected internal static void NCActivateWithFocus(BarManager manager, Control control) {
			if(manager == null || manager.Form == null && control != null) return;
			Control form = manager.GetForm();
			if(form == null) return;
			XtraForm xform = form.FindForm() as XtraForm;
			if(xform != null) xform.SuspendRedraw();
			try {
				XtraForm.SuppressDeactivation = true;
				control.Focus();
				XtraForm.SuppressDeactivation = false;
				NCActivate(manager);
			}
			finally {
				if(xform != null) xform.ResumeRedraw();
			}
		}
		protected internal static void NCActivateForm(Control form) {
			if(form == null) return;
			BarNativeMethods.SendMessage(form.Handle, 0x86, 1, 0); 
		}
		protected virtual LocationInfo CalcLocationInfo(Point p) {
			LocationInfo loc = PopupOpenHelper.CalcLocationInfo(Activator as BarItemLink, SubControl, p);
			if(loc == null) return new PopupControlContainerLocationInfo(p);
			return loc;
		}
		protected virtual PopupContainerBarControl CreatePopupContainerBarControl(BarManager manager) {
			return new PopupContainerBarControl(manager, this);
		}
		protected virtual PopupContainerBarControl CreateSubControl(BarManager manager) {
			this.LookAndFeel.Assign(manager.PaintStyle.LinksLookAndFeel);
			PopupContainerBarControl pc = CreatePopupContainerBarControl(manager);
			pc.Init();
			pc.PopupClose += new EventHandler(PopupCloseHandler);
			DevExpress.XtraBars.Forms.SubMenuControlForm form = null;
			if(Manager.GetPopupShowMode(pc) == PopupShowMode.Inplace) {
				form = Manager.SelectionInfo.GetLastOpenedPopupForm();
			}
			if(form == null)
				form = new SubMenuControlForm(manager, pc, FormBehavior.PopupContainer);
			form.RightToLeft = manager.IsRightToLeft? RightToLeft.Yes: RightToLeft.No ;
			pc.Form = form;
			return pc;
		}
		protected void PopupCloseHandler(object sender, EventArgs e) {
			SubControl = null;
			Activator = null;
		}
		IPopup PopupControl.IPopup { get { return SubControl; } }
		bool PopupControl.CanShowPopup { 
			get { return Manager != null; } 
		}
		bool PopupControl.Visible { get { return SubControl != null && SubControl.Visible;} }
		protected virtual void OnCloseUp(CustomPopupBarControl prevControl) {
			if(CloseUp != null) 
				CloseUp(this, EventArgs.Empty);
		}
		protected virtual void OnPopup() {
			if(Popup != null) 
				Popup(this, EventArgs.Empty);
		}
		Rectangle PopupControl.Bounds { 
			get { 
				if(SubControl != null) 
					return SubControl.Form.Bounds;
				return Rectangle.Empty;
			}
		}
		bool IDXDropDownControl.Visible { get { return this.SubControl != null; } }
		void IDXDropDownControl.Show(IDXMenuManager manager, Control control, Point pos) {
			if(manager == null || control == null) return;
			pos = control.PointToScreen(pos);
			RibbonControl ribbon = manager as RibbonControl;
			BarManager m = ribbon != null ? ribbon.Manager : manager as BarManager;
			ShowPopup(m, pos);
		}
		void IDXDropDownControl.Hide() {
			HidePopup();
		}
		void IDXDropDownControlEx.Show(Control control, Point pos) {
			if(control == null) return;
			pos = control.PointToScreen(pos);
			ShowPopup(Manager, pos);
		}
	}
	public class PopupControlContainerLocationInfo : LocationInfo {
		public PopupControlContainerLocationInfo(Point pt)
			: base(pt) {
		}
		protected override int CalcX() {
			int loc = base.CalcX();
			if((loc + WindowSize.Width > WorkingArea.Right) && (Location.X - MaxRightOutsize > 0)) {
				loc = Location.X - MaxRightOutsize;
			}
			return loc;
		}
	}
	[System.ComponentModel.Design.Serialization.DesignerSerializer("DevExpress.XtraBars.Design.Serialization.RibbonItemLinksSerializer, " + AssemblyInfo.SRAssemblyBarsDesign, "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design")]
	internal class PopupMenuItemLinkCollection : BarItemLinkCollection {
		PopupMenu menu;
		public PopupMenuItemLinkCollection(object owner) : base(owner) {
			this.menu = (PopupMenu)owner;
		}
		RibbonControl Ribbon { get { return menu.Ribbon; } }
		protected internal override LinksInfo LinksPersistInfo {
			get {
				if(Ribbon != null) return null;
				return base.LinksPersistInfo;
			}
			set {
				if(Ribbon != null) return;
				base.LinksPersistInfo = value;
			}
		}
	}
	public interface ISupportMenuItemFilter {
		bool IsItemAcceptable(DXMenuItem menuItem, int count);
	}
	[ToolboxItem(false)]
	public class PopupMenuBase : Component, ISupportInitialize, BarLinksHolder, ISupportMenuItemFilter, IVisualEffectsHolder {
		BarManager manager;
		RibbonControl ribbon;
		int lockUpdate, lockInit;
		string name;
		int lockAllowFireEvents;
		MenuDrawMode menuDrawMode;
		LinksInfo defaultLinks;
		internal BarItemLinkCollection itemLinks; 
		static PopupMenuBase() {
			DevExpress.Utils.Design.DXAssemblyResolver.Init();
		}
		public event LinkEventHandler LinkAdded, LinkDeleted;
		public event EventHandler CloseUp, Popup;
		public event CancelEventHandler BeforePopup;
		public PopupMenuBase(IContainer container)
			: this() {
			container.Add(this);
		}
		public PopupMenuBase() {
			this.name = "";
			this.lockUpdate = 0;
			this.menuDrawMode = MenuDrawMode.Default;
			this.manager = null;
			this.itemLinks = CreateItemLinks();
			ItemLinks.CollectionChanged += new CollectionChangeEventHandler(OnItemLinksCollectionChanged);
		}
		protected bool CanCustomizePopupMenu {
			get {
				if(Manager == null) return false;
				return true;
			}
		}
		protected virtual void CustomizeCore() {
			Manager.Helper.CustomizationManager.CustomizeMenu(this);
		}
		public virtual void Customize() {
			if(Manager == null) return;
			if(!CanCustomizePopupMenu) {
				MessageBox.Show("You must set property 'Manager' to existing BarManager, " +
					"before you can enter into customizing");
				return;
			}
			if(ShouldForceManagerChanged) Manager.FireManagerChanged();
			CustomizeCore();
		}
		protected bool ShouldForceManagerChanged {
			get { return Manager.Bars.Count == 0; }
		}
		protected virtual BarItemLinkCollection CreateItemLinks() {
			return new PopupMenuItemLinkCollection(this);
		}
		bool BarLinksHolder.Enabled { get { return true; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void Merge(PopupMenu menu) {
			ItemLinks.Merge(menu.ItemLinks);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void UnMerge() {
			ItemLinks.UnMerge();
		}
		public PopupMenuBase(BarManager manager) : this() {
			Manager = manager;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				OnManagerDisposeCore();
				OnRibbonDisposeCore();
				ItemLinks.Clear();
			}
			base.Dispose(disposing);
		}
		protected virtual void OnRibbonDisposeCore() {
			Ribbon = null;
		}
		protected virtual void OnManagerDisposeCore() {
			Manager = null;
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("PopupMenuBaseMenuDrawMode"),
#endif
 DefaultValue(MenuDrawMode.Default), Category("Appearance")]
		public virtual MenuDrawMode MenuDrawMode {
			get { return menuDrawMode; }
			set { menuDrawMode = value; }
		}
		[Browsable(false)]
		public string Name {
			get {
				if(Site == null) return name;
				return Site.Name;
			}
			set {
				if(value == null) value = string.Empty;
				name = value;
				if(Site != null) Site.Name = name;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("PopupMenuBaseManager"),
#endif
 RefreshProperties(RefreshProperties.All), Category("Manager")]
		public virtual BarManager Manager {
			get { return Ribbon == null ? manager : Ribbon.Manager; }
			set {
				if(Manager == value) return;
				BarManager prev = Manager;
				if(manager != null )
					manager.Disposed -= OnManagerDisposed;
				this.manager = value;
				if(manager != null)
					manager.Disposed += OnManagerDisposed;
				OnManagerChanged(prev);
			}
		}
		void OnManagerDisposed(object sender, EventArgs e) {			
			OnManagerDisposeCore();
		}
		protected virtual void OnManagerChanged(BarManager prev) {
			if(Manager == prev)
				return;
			if(prev != null) {
				prev.ItemHolders.Remove(this);
			}
			if(Manager != null) {
				Manager.ItemHolders.Add(this);
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("PopupMenuBaseRibbon"),
#endif
 DefaultValue(null), RefreshProperties(RefreshProperties.All), Category("Manager")]
		public virtual RibbonControl Ribbon {
			get { return ribbon; }
			set {
				if(Ribbon == value) return;
				BarManager prev = Manager;
				if(ribbon != null)
					ribbon.Disposed -= OnRibbonDisposed;				
				ribbon = value;				
				if(ribbon != null)
					ribbon.Disposed += OnRibbonDisposed;
				OnManagerChanged(prev);
			}
		}
		void OnRibbonDisposed(object sender, EventArgs e) {
			OnRibbonDisposeCore();
		}
		BarItemLink BarLinksHolder.AddItem(BarItem item, LinkPersistInfo info) {
			BarItemLink link = ItemLinks.Add(item, info);
			return link;
		}
		public virtual void AddItems(BarItem[] items) { ItemLinks.AddRange(items); }
		public virtual BarItemLink AddItem(BarItem item) { return ItemLinks.Add(item); }
		public virtual BarItemLink InsertItem(BarItemLink beforeLink, BarItem item) { return ItemLinks.Insert(beforeLink, item); }
		public virtual void RemoveLink(BarItemLink itemLink) { ItemLinks.Remove(itemLink); }
		public virtual void ClearLinks() { ItemLinks.Clear(); }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual void BeginUpdate() {
			++lockUpdate;
			OnBeginUpdate();
		}
		protected virtual void OnBeginUpdate() {
		}
		public virtual void EndUpdate() {
			if(--lockUpdate == 0) {
				OnEndUpdate();
			}
		}
		protected virtual void OnEndUpdate() {
			OnMenuChanged();
		}
		protected bool ShouldSerializeLinksPersistInfo() { return Ribbon == null; }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Browsable(false), DevExpress.Utils.Design.InheritableCollection]
		public virtual LinksInfo LinksPersistInfo {
			get { return ItemLinks.LinksPersistInfo; }
			set { ItemLinks.LinksPersistInfo = value; }
		}
		protected virtual void OnItemLinksCollectionChanged(object sender, CollectionChangeEventArgs e) {
			BarItemLink link = e.Element as BarItemLink;
			if(e.Action == CollectionChangeAction.Add) RaiseLinkAdded(link);
			if(e.Action == CollectionChangeAction.Remove) RaiseLinkDeleted(link);
			if(ItemLinks.IsLockUpdate != 0) return;
			OnMenuChanged();
		}
		bool ShouldSerializeItemLinks() { return Ribbon != null; }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual BarItemLinkCollection ItemLinks { get { return itemLinks; } }
		protected internal virtual void OnMenuChanged() { 
		}
		public virtual void ShowPopup(Point p) { }
		public virtual void ShowPopup(Point p, object activator) { }
		public virtual void ShowPopup(BarManager manager, Point p) { }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void ShowPopup(BarManager manager, Point p, PopupControl parentPopup) { }
		public virtual void HidePopup() { }
		public virtual bool Visible { get { return false; } }
		[Browsable(false)]
		public virtual bool CanShowPopup { get { return Manager != null; } }
		public virtual void BeginInit() {
			if(lockInit++ == 0) {
			}
		}
		public virtual void EndInit() {
			if(--lockInit == 0) {
			}
		}
		protected virtual bool IsAllowFireEvents { get { return Manager != null && !Manager.IsLoading && lockAllowFireEvents == 0; } }
		protected internal virtual void CreateLinksFromLinksInfo() {
			if(Ribbon != null) return;
			lockAllowFireEvents++;
			lockInit++;
			try {
				defaultLinks = LinksPersistInfo;
				LinksPersistInfo = new LinksInfo();
				Manager.CreateLinks(this, defaultLinks);
				Manager.SynchronizeLinksInfo(LinksPersistInfo, defaultLinks);
				LinksInfo temp = defaultLinks;
				defaultLinks = LinksPersistInfo;
				LinksPersistInfo = temp;
			} finally {
				lockAllowFireEvents--;
				lockInit--;
			}
		}
		protected internal virtual void RaiseCloseUp() {
			if(CloseUp != null)
				CloseUp(this, EventArgs.Empty);
		}
		protected virtual void OnBeforePopup(CancelEventArgs e) {
			if(BeforePopup != null)
				BeforePopup(this, e);
		}
		protected virtual void OnPopup() {
			if(Popup != null)
				Popup(this, EventArgs.Empty);
		}
		protected virtual void RaiseLinkAdded(BarItemLink link) {
			if(!IsAllowFireEvents) return;
			if(LinkAdded != null) LinkAdded(this, new LinkEventArgs(link));
		}
		protected virtual void RaiseLinkDeleted(BarItemLink link) {
			if(!IsAllowFireEvents) return;
			if(LinkDeleted != null) LinkDeleted(this, new LinkEventArgs(link));
		}
		protected int LockInitCore { get { return lockInit; } }
		#region IMenuItemFilter
		bool ISupportMenuItemFilter.IsItemAcceptable(DXMenuItem menuItem, int count) {
			return IsItemAcceptableCore(menuItem, count);
		}
		#endregion
		protected virtual bool IsItemAcceptableCore(DXMenuItem menuItem, int count) {
			return true;
		}
		#region TODO
		bool IVisualEffectsHolder.VisualEffectsVisible { get { return false; } }
		DevExpress.Utils.VisualEffects.ISupportAdornerUIManager IVisualEffectsHolder.VisualEffectsOwner { get { return null; } }
		#endregion
	}
	[DXToolboxItem(true),
	 Designer("DevExpress.XtraBars.Design.PopupMenuDesigner, " + AssemblyInfo.SRAssemblyBarsDesign, typeof(IDesigner)),
	 DesignerCategory("Component"),
	 Description("Displays a shortcut menu that can be displayed at any position of screen."),
	 ToolboxTabName(AssemblyInfo.DXTabNavigation),
	ToolboxBitmap(typeof(ToolboxIcons.ToolboxIconsRootNS), "PopupMenu")
	]
	public class PopupMenu : PopupMenuBase, IHasRibbonKeyTipManager, PopupControl, IDXDropDownControlEx, IAppearanceOwner, IOptionsMultiColumnOwner {
		object activator;
		int menuBarWidth;
		bool showCaption;
		string menuCaption;
		MenuAppearance menuAppearance;
		PopupMenuBarControl subControl;
		DXPopupMenu menuCreator;
		bool internalDesignMode = false;
		bool allowRibbonQATMenu = true;
		RibbonMiniToolbar ribbonToolbar;
		public event BarCustomDrawEventHandler PaintMenuBar;
		int minWidth;
		internal PopupMenu(bool setDesignMode) : this() {
			this.internalDesignMode = setDesignMode;
		}
		public PopupMenu(IContainer container)
			: this() {
			container.Add(this);
		}
		public PopupMenu() {
			MenuDrawMode = MenuDrawMode.Default;
			this.showCaption = false;
			this.menuCaption = string.Empty;
			this.menuBarWidth = 0;
			this.activator = null;
			this.subControl = null;
			this.menuAppearance = CreateMenuAppearance();
			this.menuAppearance.Changed += new EventHandler(OnMenuAppearanceChanged);
			this.ribbonToolbar = null;
			this.minWidth = 0;
			this.ShowNavigationHeader = DefaultBoolean.Default;
			AutoFillEditorWidth = true;
		}
		OptionsMultiColumn optionsMultiColumn;
		void ResetOptionsMultiColumn() { OptionsMultiColumn.Reset(); }
		bool ShouldSerializeOptionsMultiColumn() { return OptionsMultiColumn.ShouldSerializeCore(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public OptionsMultiColumn OptionsMultiColumn {
			get {
				if(optionsMultiColumn == null)
					optionsMultiColumn = new OptionsMultiColumn(this);
				return optionsMultiColumn;
			}
		}
		DefaultBoolean multiColumn = DefaultBoolean.Default;
		[DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean MultiColumn {
			get { return multiColumn; }
			set {
				if(MultiColumn == value)
					return;
				multiColumn = value;
				OnMenuChanged();
			}
		}
		[DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean ShowNavigationHeader {
			get;
			set;
		}
		public PopupMenu(BarManager manager) : this() {
			Manager = manager;
		}
		RibbonBaseKeyTipManager IHasRibbonKeyTipManager.KeyTipManager { 
			get {
				if(SubControl == null || SubControl.KeyTipManager == null) return null;
				return SubControl.KeyTipManager;
			} 
		}
		protected virtual MenuAppearance CreateMenuAppearance() { return new MenuAppearance(this); }
		bool IAppearanceOwner.IsLoading { get { return Manager == null || Manager.IsLoading; } }
		protected override void Dispose(bool disposing) {
			if(disposing) {
				HidePopup();
				OnRibbonDisposeCore();
				OnManagerDisposeCore();
				if(MenuAppearance != null) {
					MenuAppearance.Changed -= new EventHandler(OnMenuAppearanceChanged);
					MenuAppearance.Dispose();
					this.menuAppearance = null;
				}
				ItemLinks.Clear();
			}
			base.Dispose(disposing);
		}
		internal DXPopupMenu MenuCreator {
			get { return menuCreator; }
			set { menuCreator = value; }
		}
		[DefaultValue((string)null)]
		public virtual RibbonMiniToolbar RibbonToolbar {
			get { return ribbonToolbar; }
			set {
				if(RibbonToolbar == value)
					return;
				RibbonMiniToolbar oldValue = ribbonToolbar;
				ribbonToolbar = value;
				OnRibbonToolbarChanged(oldValue);
			}
		}
		protected virtual void OnRibbonToolbarChanged(RibbonMiniToolbar oldValue) {
			if(oldValue != null && oldValue.PopupMenu == this)
				oldValue.PopupMenu = null;
			if(RibbonToolbar != null) {
				RibbonToolbar.PopupMenu = this;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("PopupMenuMinWidth"),
#endif
 DefaultValue(0), Category("Appearance"), XtraSerializableProperty]
		public virtual int MinWidth {
			get { return minWidth; }
			set { minWidth = value; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("PopupMenuAllowRibbonQATMenu"),
#endif
 DefaultValue(true), Category("Behavior"), XtraSerializableProperty]
		public virtual bool AllowRibbonQATMenu {
			get { return allowRibbonQATMenu; }
			set { allowRibbonQATMenu = value; }
		}
		bool ShouldSerializeMenuAppearance() { return MenuAppearance.ShouldSerialize(); }
		void ResetMenuAppearance() { MenuAppearance.Reset(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("PopupMenuMenuAppearance"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public virtual MenuAppearance MenuAppearance {
			get { return menuAppearance; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("PopupMenuMenuBarWidth"),
#endif
 DefaultValue(0), Category("Appearance")]
		public virtual int MenuBarWidth {
			get { return menuBarWidth; }
			set {
				if(value < 0) value = 0;
				if(MenuBarWidth == value) return;
				menuBarWidth = value;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("PopupMenuShowCaption"),
#endif
 DefaultValue(false), Category("Appearance")]
		public virtual bool ShowCaption {
			get { return showCaption; }
			set {
				showCaption = value;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("PopupMenuMenuCaption"),
#endif
 DefaultValue(""), Category("Appearance")]
		public virtual string MenuCaption {
			get { return menuCaption; }
			set {
				if(value == null) value = string.Empty;
				menuCaption = value;
			}
		}
		bool ShouldSerializeManager() { return Ribbon == null && Manager != null; }
		protected override void OnManagerChanged(BarManager prev) {
			base.OnManagerChanged(prev);
			if(Manager == prev) return;
			if(prev != null) {
				prev.RemoveContextMenu(this);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual object Activator { 
			get { return activator; } 
			set { activator = value; }
		}
		protected override void OnBeginUpdate() {
			base.OnBeginUpdate();
			if(SubControl != null)
				SubControl.BeginLayout();
		}
		protected override void OnEndUpdate() {
			base.OnEndUpdate();
			if(SubControl != null)
				SubControl.EndLayout();
		}
		protected internal override void OnMenuChanged() {
			if(Manager == null) return;
			if(SubControl != null) {
				SubControl.UpdateVisibleLinks();
				SubControl.LayoutChanged();
			}
		}
		IPopup GetPopup() {
			BarItemLink link = Activator as BarItemLink;
			if(link != null) return link.BarControl as IPopup;
			if(SubControl != null)
				return ((IPopup)SubControl).ParentPopup;
			return null;
		}
		public override void ShowPopup(Point p) {
			ShowPopup(Manager, p);
		}
		public override void ShowPopup(Point p, object activator) {
			if(Activator == null) {
				Activator = activator;
			}
			ShowPopup(Manager, p);
		}
		bool prevRibbonToolbarTransparency = false;
		public override void ShowPopup(BarManager manager, Point p, PopupControl parentPopup) {
			if(manager == null) return;
			HidePopup();
			SubControl = CreateSubControl(manager);
			if(SubControl == null) return;
			if(parentPopup != null)
				((PopupControl)this).IPopup.ParentPopup = parentPopup.IPopup;
			if(manager.GetPopupShowMode(SubControl) == PopupShowMode.Inplace &&
				manager.SelectionInfo.GetLastOpenedPopupForm() != null) {
				XtraAnimator.Current.AddAnimation(new SubMenuInplaceAnimationInfo(manager.SelectionInfo.GetLastOpenedPopupForm(), SubControl, false));
			}
			manager.SelectionInfo.OpenPopup(GetPopup(), SubControl, CalcLocationInfo(p));
			if(!manager.IsBarsActive) {
				if(Ribbon == null || !Ribbon.IsDesignMode) manager.SelectionInfo.TemporaryActive = true;
				if(SubControl != null) {
					Form form = SubControl.FindForm();
					if(form != null) { 
						form.Activate();
						form.Focus();
					}
				}
			}
			if(RibbonToolbar != null) {
				ShowRibbonToolbar(p);
			}
		}
		public override void ShowPopup(BarManager manager, Point p) {
			ShowPopup(manager, p, null);
		}
		void ShowRibbonToolbar(Point menuShowPoint) {
			Form frm = SubControl == null? null: SubControl.FindForm();
			if(frm == null)
				return;
			this.prevRibbonToolbarTransparency = RibbonToolbar.OpacityOptions.AllowTransparency;
			RibbonToolbar.OpacityOptions.AllowTransparency = false;
			Size sz = RibbonToolbar.CalcBestSize();
			Point pt = frm.Location;
			Screen sc = Screen.FromControl(SubControl);
			if(pt.Y - 15 - sz.Height < 0 || ((frm.Bottom + 15 + sz.Height < sc.WorkingArea.Bottom) && frm.Top < menuShowPoint.Y)) {
				RibbonToolbar.Alignment = ContentAlignment.BottomRight;
				pt.Y = frm.Bottom + 15;
			}
			else {
				RibbonToolbar.Alignment = ContentAlignment.TopRight;
				pt.Y = frm.Top - 15;
			}
			RibbonToolbar.Show(pt);
		}
		protected internal virtual LocationInfo CalcLocationInfo(Point p) {
			LocationInfo loc = PopupOpenHelper.CalcLocationInfo(Activator as BarItemLink, SubControl, p);
			if(loc == null) return new LocationInfo(p, p, true, Manager != null && Manager.IsRightToLeft);
			return loc;
		}
		protected virtual PopupMenuBarControl CreatePopupControl(BarManager manager) { return new PopupMenuBarControl(manager, this); }
		protected virtual SubMenuControlForm CreateForm(BarManager manager, PopupMenuBarControl pc) { return new SubMenuControlForm(manager, pc, FormBehavior.SubMenu); }
		protected virtual PopupMenuBarControl CreateSubControl(BarManager manager) {
			CancelEventArgs e = new CancelEventArgs();
			OnBeforePopup(e);
			if(e.Cancel) return null;
			PopupMenuBarControl pc = CreatePopupControl(manager);
			pc.Init();
			pc.UpdateVisibleLinks();
			if(!IsDesignMode && !pc.AllowOpenPopup) { 
				pc.Dispose();
				return null;
			}
			pc.PopupClose += new EventHandler(OnPopupCloseHandler);
			if(manager.GetPopupShowMode(pc) == PopupShowMode.Inplace && manager.SelectionInfo.OpenedPopups.Count > 0) {
				manager.SelectionInfo.CheckCanChildPopup(pc);
				if(manager.SelectionInfo.OpenedPopups.Count > 0)
					return pc;
			}
			pc.Form = CreateForm(manager, pc);
			return pc;
		}
		protected internal virtual bool IsDesignMode { get { return DesignMode || internalDesignMode; } }
		protected void OnPopupCloseHandler(object sender, EventArgs e) {
			SubControl = null;
			Activator = null;
		}
		IPopup PopupControl.IPopup { get { return SubControl; } }
		Rectangle PopupControl.Bounds { get { return SubControl != null ? SubControl.Form.Bounds : Rectangle.Empty; } }
		[Browsable(false)]
		public override bool Visible {
			get { return SubControl != null; }
		}
		public override void HidePopup() {
			base.HidePopup();
			BarManager manager = SubControl == null ? null : SubControl.Manager;
			if(manager == null || SubControl == null) return;
			if(manager.SelectionInfo.TemporaryActive) manager.SelectionInfo.TemporaryActive = false;
			manager.SelectionInfo.ClosePopup(SubControl);
			Activator = null;
		}
		[Browsable(false)]
		public virtual bool Opened { get { return SubControl != null; }  }
		protected internal PopupMenuBarControl SubControl {
			get { return subControl; }
			set {
				if(SubControl == value) return;
				PopupMenuBarControl prev = SubControl;
				if(SubControl != null && !SubControl.Destroying) SubControl.Dispose();
				subControl = value;
				if(value == null) 
					OnCloseUp(prev);
				else
					OnPopup();
			}
		}
		protected void OnMenuAppearanceChanged(object sender, EventArgs e) {
			OnMenuChanged();
		}
		bool delayedCloseUp = false;
		protected virtual void OnCloseUp(CustomPopupBarControl prevControl) {
			BarManager manager = prevControl == null ? Manager : prevControl.Manager;
			if(manager.SelectionInfo.TemporaryActive) manager.SelectionInfo.TemporaryActive = false;
			if(prevControl != null && prevControl.IsCloseUpLocked) 
				this.delayedCloseUp = true;
			else
				RaiseCloseUp();
			if(RibbonToolbar != null)
				RibbonToolbar.OpacityOptions.AllowTransparency = this.prevRibbonToolbarTransparency;
		}
		internal void RaiseDelayedCloseUp() {
			if(this.delayedCloseUp) {
				this.delayedCloseUp = false;
				RaiseCloseUp();
			}
		}
		protected virtual void RaisePaintMenuBar(BarCustomDrawEventArgs e) {
			if(PaintMenuBar != null) PaintMenuBar(this, e);
		}
		internal void RaisePaintMenuBarCore(BarCustomDrawEventArgs e) { RaisePaintMenuBar(e); }
		bool IDXDropDownControl.Visible { get { return this.Visible; } }
		void IDXDropDownControl.Show(IDXMenuManager manager, Control control, Point pos) {
			if(manager == null || control == null) return;
			pos = control.PointToScreen(pos);
			RibbonControl ribbon = manager as RibbonControl;
			BarManager m = ribbon != null ? ribbon.Manager : manager as BarManager;
			if(Activator == null) {
				Activator = control;
			}
			ShowPopup(m, pos);
		}
		void IDXDropDownControl.Hide() {
			HidePopup();
		}
		void IDXDropDownControlEx.Show(Control control, Point pos) {
			ShowDXDropDownControlCore(control, pos);
		}
		protected virtual void ShowDXDropDownControlCore(Control control, Point pos) {
			if(control == null) return;
			pos = control.PointToScreen(pos);
			if(Activator == null) {
				Activator = control;
			}
			ShowPopup(pos);
		}
		[DefaultValue(true)]
		public bool AutoFillEditorWidth { get; set; }
		void IOptionsMultiColumnOwner.OnChanged() {
			OnMenuChanged();
		}
	}
	internal class PopupOpenHelper {
		static public LocationInfo CalcLocationInfo(BarItemLink link, Point p) { return CalcLocationInfo(link, null, p); }
		static public LocationInfo CalcLocationInfo(BarItemLink link, CustomControl subControl, Point p) {
			if((link == null || link.RibbonItemInfo == null) && subControl != null && subControl.Manager.GetPopupShowMode(subControl as IPopup) == PopupShowMode.Inplace) {
				Form form = subControl.FindForm(); 
				if(form == null) form = subControl.Manager.SelectionInfo.GetLastOpenedPopupForm();
				if(link == null || !link.IsLinkInMenu)
					p.X -= form.Width / 2;
				else
					p = form.Location;
				return new LocationInfoSimple(p, p);
			}
			if(link == null) return null;
			BarLinkViewInfo li = link.LinkViewInfo;
			if(li == null) return null;
			Rectangle r = li.Bounds;
			r.Location = li.Link.LinkPointToScreen(r.Location);
			if(li.IsLinkInMenu) 
				return CalcInMenuLocationInfo(li, r, li.Link.BarControl, subControl);
			if(link.RibbonItemInfo != null) return CalcInRibbonLocationInfo(subControl, li, r);
			return CalcOnBarLocationInfo(li, r);
		}
		static LocationInfo CalcInRibbonLocationInfo(CustomControl subControl, BarLinkViewInfo li, Rectangle linkBounds) {
			if(li.Link.Ribbon.GetRibbonStyle() == RibbonControlStyle.TabletOffice) {
				int width = 0;
				if(subControl != null) {
					Form form = subControl.FindForm();
					width = form != null ? form.Width : subControl.Width;
				}
				int x = linkBounds.X + (linkBounds.Width - width) / 2;
				Rectangle screen = Screen.GetWorkingArea(new Point(linkBounds.X, 10));
				x = Math.Max(x, screen.X);
				return new LocationInfoSimple(new Point(x, linkBounds.Y + linkBounds.Height), linkBounds.Location);
			}
			return new LocationInfoSimple(new Point(linkBounds.X, linkBounds.Y + linkBounds.Height), linkBounds.Location);
		}
		static SubMenuControlForm GetPopupForm(CustomControl barControl, CustomControl subControl) {
			Form form = subControl.FindForm();
			if(form == null)
				return (SubMenuControlForm)barControl.FindForm();
			return (SubMenuControlForm)form;
		}
		static LocationInfo CalcInplaceLocationInfo(BarLinkViewInfo li, Rectangle linkBounds, CustomControl barControl, CustomControl subControl) {
			SubMenuControlForm form = GetPopupForm(subControl, barControl);
			return new LocationInfo(form.Location, form.Location, false, false, true);
		}
		static void MakeTransition(CustomControl prev, CustomControl next, Form form, bool fromLeft) {
		}
		static BarManager GetManager(BarLinkViewInfo li, CustomControl barControl, CustomControl subControl) {
			if(li != null)
				return li.Manager;
			if(barControl != null)
				return barControl.Manager;
			if(subControl != null)
				return subControl.Manager;
			return null;
		}
		public static LocationInfo CalcInMenuLocationInfo(BarLinkViewInfo li, Rectangle linkBounds, CustomControl barControl, CustomControl subControl) {
			BarManager manager = GetManager(li, barControl, subControl);
			if(manager != null && manager.GetPopupShowMode((subControl == null? barControl: subControl) as IPopup) == PopupShowMode.Inplace && manager.SelectionInfo.OpenedPopups.Count > 0) {
				return CalcInplaceLocationInfo(li, linkBounds, barControl, subControl);
			}
			Point leftLoc, rightLoc;
			rightLoc = leftLoc = linkBounds.Location;
			rightLoc.X += li.Bounds.Width;
			IPopup popup = barControl as IPopup;
			if(popup != null) {
				SubMenuControlForm form = popup.CustomControl.FindForm() as SubMenuControlForm;
				rightLoc.X = form == null ? popup.Bounds.Right : form.Bounds.Right;
				if(!popup.PopupChildForceBounds.IsEmpty) {
					LocationInfo res = new LocationInfo(leftLoc, leftLoc, false, false, true);
					res.ForceFormBounds = popup.PopupChildForceBounds;
					return res;
				}
			}
			bool isRightToLeft = manager != null && manager.IsRightToLeft;
			if(OpenSubToLeft(barControl, subControl, isRightToLeft)) {
				if(popup != null) {
					leftLoc.X = popup.CustomControl.FindForm().Bounds.Left;
				}
				return new LocationInfo(leftLoc, rightLoc, false, true, true);
			}
			return new LocationInfo(rightLoc, leftLoc, false, false, true);
		}
		static public LocationInfo CalcOnBarLocationInfo(BarLinkViewInfo li, Rectangle linkBounds) {
			return new LocationInfo(new Point(linkBounds.X, linkBounds.Y + linkBounds.Height), linkBounds.Location, true, false);
		}
		static bool OpenSubToLeft(CustomControl control, CustomControl subControl, bool isRightToLeft) {
			IPopup popup = control as IPopup;
			Rectangle screen = Screen.GetWorkingArea(control);
			if(popup != null) {
				if(popup.OwnerLink != null) {
					BarLinkViewInfo info = popup.OwnerLink.LinkViewInfo;
					Rectangle linkBounds = new Rectangle(popup.OwnerLink.LinkPointToScreen(info.Bounds.Location), info.Bounds.Size);
					if(isRightToLeft) {
						if(linkBounds.X - popup.Bounds.Width < screen.X)
							return false;
						return true;
					}
					if(popup.Bounds.Right <= linkBounds.Left) {
						return true;
					}
				}
				CustomPopupBarControl sc = subControl as CustomPopupBarControl;
				if(sc != null && sc.Form != null) {
					if(isRightToLeft) {
						if(popup.Bounds.X - sc.Form.Width < screen.X)
							return false;
						return true;
					}
					if(popup.Bounds.Right + sc.Form.Width >= screen.Right)
						return true;
				}
			}
			return false;
		}
	}
}
